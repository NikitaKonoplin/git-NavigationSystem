using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace NAVI_System
{
//    public class Task_manager
//    {
//        // Коллекция ключей (COM_Date) и значений (Task_and_TokenSource) для хранении информации о порте, его задаче и источнике отмены
//        Dictionary<string, Task_and_TokenSource> current_tasks_dic = new Dictionary<string, Task_and_TokenSource>();
//        List<string> list_for_cancel = new List<string>();
//        //List<Task_and_TokenSource> current_tasks_dic = new List<Task_and_TokenSource>();

//        struct Task_and_TokenSource  // структура с сылками на задачу и источник ее отмены
//        {
//            public string DeviceName;
//            public CancellationTokenSource TSource;
//            public Task tsk;
//            public Task_and_TokenSource(CancellationTokenSource S, Task T, string name)
//            {
//                DeviceName = name;
//                TSource = S;
//                tsk = T;
//            }
//        }

//        public bool start_tasks(List<COM_Date> COM_List)
//        {

//            if (COM_List == null) return false;
//            List<AC_COM> list = COM_Factory.factory(COM_List);
//            foreach (AC_COM item in list)
//            {
//                if (start_t(item)) Status_List.push("Task for " + item.devicename+ " started correctly " + DateTimeOffset.Now.ToString());
//                else Status_List.push("Task Start Error for " + item.devicename + " " + DateTimeOffset.Now.ToString());
//            }
//            return true; 
//        }

//        public bool cancel_tasks(List<COM_Date> COM_List) 
//        {
//            ICollection<string> dic = current_tasks_dic.Keys;
            
//            foreach (COM_Date COM_item in COM_List) // поиск из списка запущенных
//            {
//                foreach (string TaT_key in dic)
//                {
//                    if (TaT_key == COM_item.DeviceName)
//                    {
//                        try
//                        {
//                            current_tasks_dic[TaT_key].TSource.Cancel(); // отмена задачи
//                            bool iscompl = current_tasks_dic[TaT_key].tsk.IsCompleted;
//                            current_tasks_dic[TaT_key].tsk.Wait(); // приостановка потока до отмены задачи
//                            if (current_tasks_dic[TaT_key].tsk.IsCompleted) // проверка отмены
//                            {
//                                Status_List.push("Task for " + TaT_key + " CANCELED " + DateTimeOffset.Now.ToString());
//                                current_tasks_dic[TaT_key].tsk.Dispose(); // освобождение ресурсов
//                                current_tasks_dic[TaT_key].TSource.Dispose();
//                                list_for_cancel.Add(TaT_key);
//                            }
//                            else Status_List.push("Task for " + TaT_key + " NO CANCELED " + DateTimeOffset.Now.ToString());
//                        }
//                        catch (Exception)
//                        {
//                            Status_List.push("Task CANCELED ERROR for " + TaT_key + " " + DateTimeOffset.Now.ToString());
//                        }
//                    }
//                }
//            }
//            foreach (string item in list_for_cancel)
//            {
//                current_tasks_dic.Remove(item);
//            }
//            list_for_cancel.Clear();
//            return true;
//        }

//        bool start_t(AC_COM com)
//        {
//            CancellationTokenSource cancelTokSource = new CancellationTokenSource(); // источник признака отмены задачи
//            try
//            {
//                Task tsk = new Task((object n) => { MyTask(com, n); }, cancelTokSource.Token, cancelTokSource.Token);
//                // задача с передачей признака отмены
//                Task_and_TokenSource T_and_S = new Task_and_TokenSource(cancelTokSource, tsk, com.devicename); // структура содержащая источник признака отмены и саму задачу
//                current_tasks_dic.Add(com.devicename, T_and_S); // добавление пары ключь-значение
//                tsk.Start();
//                return true;
//            }
//            catch (Exception e)
//            {
//                Status_List.push(e.Message + " " + DateTimeOffset.Now.ToString());
//                return false;
//            }
            
//        }

//        delegate void end_com(bool is_exc); //деленгат для лямбды, которая завершает передачу данных, завершает внутреннюю задачу обработки
//        void MyTask(AC_COM com, object ct) // тело задачи
//        {
//            Queue<object> MyQueue = new Queue<object>();
//            CancellationToken cancelTok = (CancellationToken)ct;

//            bool st_op = com.open_port();
//            Status_List.push("Opening port for " + com.devicename + " " + st_op.ToString() + " " + DateTimeOffset.Now.ToString());
//            if (!st_op) // если порт не удалость открыть
//            {
//                current_tasks_dic.Remove(com.devicename);
//                return;
//            }

//            Task tsk = Task.Factory.StartNew(() => // внутренняя задача обработки данных
//            {
//                while (!cancelTok.IsCancellationRequested)
//                {
//                    while (MyQueue.Count>0)
//                    {//***************processing*****************
//                        bool st_proc = com.date_process(MyQueue);
//                    }//******************************************
//                    //Console.WriteLine("внутренняя задача спит");
//                    Thread.Sleep(50);
//                }
//            });

//            while (!cancelTok.IsCancellationRequested) // задача чтения данных порта
//                {
//                    //***************************reading************************************
//                    object date = com.read();
//                    if (date != null) lock (MyQueue) MyQueue.Enqueue(date);
//                }
//            bool st_cl = com.close_port();
//            tsk.Wait();
//            tsk.Dispose();
//            Status_List.push("Close port for " + com.devicename + " " + st_cl.ToString() + " " + DateTimeOffset.Now.ToString());
//            Console.WriteLine("из внешней и внутренней задач вышли");
//        }
//    }
}
