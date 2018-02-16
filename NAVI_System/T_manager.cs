using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NAVI_System
{
    public class T_manager
    {
        //CancellationTokenSource tokenSource2 = new CancellationTokenSource();
        //Dictionary<string, Task_and_TokenSource> current_tasks_dic = new Dictionary<string, Task_and_TokenSource>();
        Dictionary<string, Task_and_TokenSource> current_tasks_dic = new Dictionary<string, Task_and_TokenSource>();

        struct Task_and_TokenSource  // структура с сылками на задачу и источник ее отмены
        {
            public string DeviceName;
            public CancellationTokenSource TSource;
            public Task tsk;
            public Task_and_TokenSource(CancellationTokenSource S, Task T, string name)
            {
                DeviceName = name;
                TSource = S;
                tsk = T;
            }
        }

        public void cansel(Task_communication t_request)
        {
            for (int i=0; i< t_request.list_f_info.Count; i++)
            {
                if (current_tasks_dic.ContainsKey(t_request.list_f_info[i].name))
                {
                    current_tasks_dic[t_request.list_f_info[i].name].TSource.Cancel();
                    current_tasks_dic[t_request.list_f_info[i].name].tsk.Wait();
                    Status_List.push("Task for " + t_request.list_f_info[i].name + " CANCELED " + DateTimeOffset.Now.ToString());
                    //освободить ресурсы
                    current_tasks_dic[t_request.list_f_info[i].name].TSource.Dispose();
                    current_tasks_dic[t_request.list_f_info[i].name].tsk.Dispose();
                    current_tasks_dic.Remove(t_request.list_f_info[i].name);
                }
            }
        }
        public void start(Task_communication t_request)
        {
            foreach (Task_communication.F_info item in t_request.list_f_info)
            {
                CancellationTokenSource cancelTokSource = new CancellationTokenSource(); // источник признака отмены задачи
                Task t = new Task((object n) =>
                {
                    Status_List.push("Task for " + item.name + " STARTED " + DateTimeOffset.Now.ToString());
                    while (!cancelTokSource.IsCancellationRequested)
                    {
                        item.function();  // запуск функции
                        Thread.Sleep(item.sleep_time);
                    }
                    item.clean?.Invoke();

                }, cancelTokSource.Token, cancelTokSource.Token);

                if (!current_tasks_dic.ContainsKey(item.name))
                {
                    current_tasks_dic.Add(item.name, new Task_and_TokenSource(cancelTokSource, t, item.name)); // структура содержащая источник признака отмены, саму задачу, name
                    t.Start();
                }
            }
        }


    }
}
