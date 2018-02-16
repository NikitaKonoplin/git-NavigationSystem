using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Windows;

namespace NAVI_System
{

    public static class DevicesTest
    {
        static Random ran = new Random();
        public static class TrackLinkTest
        {
            //167.11$167.2$55.48$55.43
            //b_x = 167.2751, b_y = 55.4048, s_x = 167.2338, s_y = 55.3797
            static string date = "9,05/04/17,14:54:31,  5544.63,16718.45,  5544.645,16718.47,353.7, 198.4\r";
            static string date1 = "8,05/04/17,14:54:31,  5544.63,16718.45,  5544.643,16718.465,353.7, 178.4\r";
            static int num = 0;
            public static object testread()
            {
                string[] sep = date.Split(',');
                string[] sep2 = date.Split(',');
                if (num == 0)
                {
                    sep = date.Split(',');

                }
                else
                {
                    sep = date1.Split(',');

                }

                double b = double.Parse(sep[0], System.Globalization.CultureInfo.InvariantCulture);
                double s_lat = double.Parse(sep2[3], System.Globalization.CultureInfo.InvariantCulture)/100;
                double s_long = double.Parse(sep2[4], System.Globalization.CultureInfo.InvariantCulture)/100;
                double b_lat = double.Parse(sep[5], System.Globalization.CultureInfo.InvariantCulture)/100;
                double b_long = double.Parse(sep[6], System.Globalization.CultureInfo.InvariantCulture)/100;
                double s_h = double.Parse(sep[7], System.Globalization.CultureInfo.InvariantCulture);
                double b_d = double.Parse(sep[8], System.Globalization.CultureInfo.InvariantCulture);


                s_lat += (ran.NextDouble() - 0.3) / 8000;
                s_long += (ran.NextDouble() - 0.5) / 8000;
                b_lat += (ran.NextDouble() - 0.3) / 8000;
                b_long += (ran.NextDouble() - 0.5) / 8000;
                s_h += (ran.NextDouble() - 0.2) / 4;
                b_d += (ran.NextDouble() - 0.5) * 10;


                sep[3] = (s_lat * 100).ToString().Replace(',','.');
                sep[4] = (s_long * 100).ToString().Replace(',', '.');
                sep[5] = (b_lat * 100).ToString().Replace(',', '.');
                sep[6] = (b_long * 100).ToString().Replace(',', '.');
                sep[7] = (s_h ).ToString().Replace(',', '.');
                sep[8] = (b_d ).ToString().Replace(',', '.');
                

                if (num == 0)
                {
                    date = "";
                    foreach (var item in sep)
                    {
                        date += item + ",";
                    }
                    num = 1;
                    Thread.Sleep(300);
                    return date;
                }
                else
                {
                    date1 = "";
                    foreach (var item in sep)
                    {
                        date1 += item + ",";
                    }
                    num = 0;
                    Thread.Sleep(300);
                    return date1;
                }
                
            }
        }
        public static class SonarDyneTest
        {

        }
        public static class DVLTest
        {

        }
        public static class GPSTest
        {

        }

    }
    public static class EventTest
    {
        public static TrackLink TL;
    }
    public static class COM_Factory
    {
        public static string[] device_types = { "DVL", "Test", "SonarDyne", "TrackLink" };
        static List<AC_COM> list = new List<AC_COM>();
        public static List<AC_COM> factory(List<COM_Date> com_list)
        {
            list.Clear();
            if (com_list == null) return null;
            foreach (COM_Date item in com_list)
            {
                switch (item.DeviceName)
                {
                    case "DVL":
                        list.Add(new DVL(item));
                        break;
                    case "Test":
                        list.Add(new Test(item));
                        break;
                    case "SonarDyne":
                        list.Add(new SonarDyne(item));
                        break;
                    case "TrackLink":
                        var temp = new TrackLink(item);
                        list.Add(temp);
                        EventTest.TL = temp;
                        break;
                    default:
                        break;
                }
            }
            if (list == null) return null;
            return list;
        }
    }
    public abstract class AC_COM : Task_communication
    {
        /// <summary>
        /// Определение члна события
        /// </summary>
        public event EventHandler<object> NewDate;
        /// <summary>
        /// Определение метода, ответственного за уведомление зарегестрированных объектов о событии
        /// </summary>
        /// <param name="e">Инфомация, передаваемая получателям события</param>
        protected virtual void OnNewDate(object e)
        {
            //сохранение ссылки на делегата во временой переменной (во избежание изменения делегата другим потоком)
            Volatile.Read(ref NewDate)?.Invoke(this, e);
        }
        //public AC_COM()
        //{
        //    y++;
        //    list_f_info = new List<F_info>();
        //    list_f_info.Add(new F_info(y+"read", read));
        //    list_f_info.Add(new F_info(y+"date_process", date_process));
        //}


        protected SerialPort mySerialPort = new SerialPort();
        public string devicename;
        protected Queue<object> queue = new Queue<object>();
        public bool open_port()
        {
            if (mySerialPort.IsOpen)
            {
                //Status_List.push("SerialPort " + mySerialPort.PortName + " IsOpen " + DateTimeOffset.Now.ToString());
                return true;
            }
            else
            {
               try
                {
                    mySerialPort.Open();
                    Status_List.push("SerialPort " + mySerialPort.PortName + " IsOpen " + DateTimeOffset.Now.ToString());
                    return true;
                }
               catch
                {
                    return false;
                }
            }
        }
        public bool close_port()
        {

            if (mySerialPort.IsOpen)
            {
                mySerialPort.Close();
                Status_List.push("SerialPort " + mySerialPort.PortName + " IsClosed " + DateTimeOffset.Now.ToString());
                return true;
            }
            else
            {
                return false;
            }

        }
        public abstract void read();
        public abstract void date_process();

    }
    public class Test : AC_COM
    {
        private int status; //0-все хорошо, 1 - исключение открытия порта, 2 - исключение чтения
        void clean()
        {
            close_port();
        }
        public Test(COM_Date com)
        {
            devicename = com.DeviceName;
            mySerialPort.PortName = "azaza";//com.PortName;
            mySerialPort.BaudRate = (int)com.BaudRate;
            mySerialPort.Parity = Parity.None;
            mySerialPort.StopBits = StopBits.One;
            mySerialPort.DataBits = (int)com.DataBits;
            mySerialPort.Handshake = Handshake.None;
            mySerialPort.RtsEnable = true;
            mySerialPort.ReadTimeout = 300;
            list_f_info = new List<F_info>();
            list_f_info.Add(new F_info(devicename + " read", read, clean, 500));
            list_f_info.Add(new F_info(devicename + " date_process", date_process, null, 100));

        }
        double count = 0;
        struct Test_D
        {
            public double b_x;
            public double b_y;
            public double s_x;
            public double s_y;
        }
        Test_D te = new Test_D() { b_x = 167.2751, b_y = 55.4048, s_x = 167.2338, s_y = 55.3797 };
        Queue<Test_D> que = new Queue<Test_D>();
        Random ran = new Random();
       // Queue<double> que = new Queue<double>();
        public override void read()
        {
            //Console.WriteLine("read");
            /*
            if (!open_port()) return;

            byte[] buf = new byte[4];
            byte[] date = new byte[4];
            //byte a = ;
            int count = 4;
            for (int i = 0; i < count; i++)
            {
                if (i > 0 && buf[i - 1] == 12)
                {
                    i = 0;
                    //count=4;
                }

                try
                {
                    buf[i] = (byte)mySerialPort.ReadByte();
                }
                catch (Exception) { }
                
            } */
            te.b_x+= (ran.NextDouble()) / 4000;
            te.b_y += (ran.NextDouble()-0.5) / 2000;
            te.s_y += (ran.NextDouble()) / 4000;
            te.s_x += (ran.NextDouble() - 0.5) / 2000;

            que.Enqueue(new Test_D()
            {
                b_x = te.b_x,
                b_y = te.b_y,
                s_x = te.s_x,
                s_y = te.s_y


        });
            //Thread.Sleep(100);
            //DATE.task1 = count.ToString();
            
            
            //Action update = async () => // синхронно запускаем обновление интенрфейса исключительно async
            //{
            //    await Objects_Links.M_Window?.Dispatcher.InvokeAsync(Objects_Links.M_Window.update_Test1);
            //};
            //update();
            //else return null;
        }
        public override void date_process()
        {
            //Console.WriteLine("date_process");
            //string date;
            Action update = async () => // синхронно запускаем обновление интенрфейса исключительно async
            {
                await Objects_Links.M_Window?.Dispatcher.InvokeAsync(() => {
                    //Objects_Links.M_Window.device_updater.HANS_update(true);
                    Objects_Links.M_Window.device_updater.Ship_update(true);
                }

                ); 
            };
            while (que.Count!=0)
            {
                lock (que)
               {
                    if(!Navigation_Date.HANS.beacons.ContainsKey(1)) Navigation_Date.HANS.beacons.Add(1, new Device_Date.HANS.Beacon());
                    Device_Date.HANS.Beacon beac = Navigation_Date.HANS.beacons[1];
                    beac.Number = 1;
                    Test_D p = (Test_D)que.Dequeue();
                    //Point p1 = Objects_Links.M_Window.mesh.PixToGeo(p, p);
                    beac.Position = new Geo_Pos(p.b_y, p.b_x, null);
                    Navigation_Date.Ship.Position = new Geo_Pos(p.s_y, p.s_x, null);
                }
                update();
            }
            return;
        }
    }
    public class SonarDyne : AC_COM
    {
        void clean()
        {
            close_port();
        }
        public SonarDyne(COM_Date com)
        {
            devicename = com.DeviceName;
            mySerialPort.PortName = com.PortName;
            mySerialPort.BaudRate = (int)com.BaudRate;
            mySerialPort.Parity = Parity.None;
            mySerialPort.StopBits = StopBits.One;
            mySerialPort.DataBits = (int)com.DataBits;
            mySerialPort.Handshake = Handshake.None;
            mySerialPort.RtsEnable = true;
            mySerialPort.ReadTimeout = 300;
            list_f_info = new List<F_info>();
            list_f_info.Add(new F_info(devicename + " read", read, clean));
            list_f_info.Add(new F_info(devicename + " date_process", date_process));

        }
        public override void read()
        {
            if (!open_port()) return;

            string buffer;
            for (;;)
            {
                try
                {
                    buffer = mySerialPort.ReadLine();
                    if (buffer.Length > 5) break;
                }
                catch (Exception)
                {
                    return;
                }
            }
            queue.Enqueue(buffer);
        }

        delegate void For_Date_Proc(Device_Date.HANS.Beacon beacon);
        public override void date_process()
        {
            string date = "";
            //string[] b = new string[5];
            lock (queue)
            {
                if (queue.Count>0)
                {
                    //while (queue.Count > 0)
                    // {
                    date = (string)queue.Dequeue();
                       // i++;
                   // }
                    //date = (string)queue.Dequeue();
                }
                else return;
            }

            
            if (date == null) return;
            try
            {
                int B_ind = date.IndexOf("B", 8);
                int X_ind = date.IndexOf("X", 8);
                int Y_ind = date.IndexOf("Y", 8);
                int D_ind = date.IndexOf("D", 8);
                int P_ind = date.IndexOf("P", 8);
                string beacon = date.Substring(B_ind + 2, X_ind - 1 - B_ind - 2);
                string X_m = date.Substring(X_ind + 2, Y_ind - 1 - X_ind - 2);
                string Y_m = date.Substring(Y_ind + 2, D_ind - 1 - Y_ind - 2);
                string D_m = date.Substring(D_ind + 2, P_ind - 1 - D_ind - 2);
                double Beacon_number = double.Parse(beacon, System.Globalization.CultureInfo.InvariantCulture);
                double x = double.Parse(X_m, System.Globalization.CultureInfo.InvariantCulture);
                double y = double.Parse(Y_m, System.Globalization.CultureInfo.InvariantCulture);
                double d = double.Parse(D_m, System.Globalization.CultureInfo.InvariantCulture);

                For_Date_Proc Date_Proc = Beac =>
                {
                    Beac.Number = Beacon_number;
                    Beac.Depth = d;
                    Beac.X_offset = x;
                    Beac.Y_offset = y;
                    Beac.Position = new Geo_Pos(x, y);
                    Action update = async () => // синхронно запускаем обновление интенрфейса исключительно async
                    {
                        await Objects_Links.M_Window?.Dispatcher.InvokeAsync(Objects_Links.M_Window.update_SonarDyne);
                    };
                    update();
                };

                if (Navigation_Date.HANS.beacons.ContainsKey(Beacon_number))
                {
                    Date_Proc(Navigation_Date.HANS.beacons[Beacon_number]);
                    Status_List.push("Beacon number " + Beacon_number + " Updated" + DateTimeOffset.Now.ToString());
                    return;
                }
                else
                {
                    Device_Date.HANS.Beacon beac = new Device_Date.HANS.Beacon();
                    Date_Proc(beac);
                    Navigation_Date.HANS.beacons.Add(Beacon_number, beac);
                    Status_List.push("Beacon number " + Beacon_number + " Created and Updated" + DateTimeOffset.Now.ToString());
                    return;
                }

            }
            catch (Exception e)
            {
                Status_List.push("Beacon date processing FALSE " + e.Message + " " + DateTimeOffset.Now.ToString());
                return;
            }
        }
    }
    public class DVL : AC_COM
    {
        public void clean()
        {
            close_port();
        }

        private int status; //0-все хорошо, 1 - исключение открытия порта, 2 - исключение чтения
        public DVL(COM_Date com)
        {
            devicename = com.DeviceName;
            mySerialPort.PortName = com.PortName;
            mySerialPort.BaudRate = (int)com.BaudRate;
            mySerialPort.Parity = Parity.None;
            mySerialPort.StopBits = StopBits.One;
            mySerialPort.DataBits = (int)com.DataBits;
            mySerialPort.Handshake = Handshake.None;
            mySerialPort.RtsEnable = true;
            mySerialPort.ReadTimeout = 300;
            list_f_info = new List<F_info>();
            list_f_info.Add(new F_info(devicename + " read", read, clean));
            list_f_info.Add(new F_info(devicename + " date_process", date_process));
        }
        public override void read()
        {
            if (!open_port()) return;

            byte[] buf = new byte[88];
            int count = 88;
            for (int i = 0; i < count; i++)
            {

                try
                {
                    buf[i] = (byte)mySerialPort.ReadByte();
                }
                catch (Exception)
                {
                    Console.WriteLine("Ошибка чтения");
                    //throw;
                }

                if (i > 0 && buf[i - 1] == 125)
                {
                    if (buf[i] == 1)
                    {
                        buf[0] = buf[i - 1];
                        buf[1] = buf[i];
                        i = 1;
                        count = 88;
                    }
                    else if (buf[i] == 0)
                    {
                        buf[0] = buf[i - 1];
                        buf[1] = buf[i];
                        i = 1;
                        count = 47;
                    }
                }
            }
            if (buf.Length > 46) queue.Enqueue(buf);
            else return;
        }
        public override void date_process()
        {
            byte[] date = new byte[100];
            byte[] date_tmp = new byte[90];
            lock (queue)
            {
                if (queue.Count > 0)
                {
                    date = (byte[])queue.Dequeue();
                }
                else return;
            }

                
            if (date == null) return;

            bool state = false;
            short crc = new short();
            int crc1 = new int();

            for (int i = 0; i < date.Length; i++)
            {
                if (date[i] == 125)
                {

                    if (date[i + 1] == 1 && date.Length - i >= 88)
                    {
                        for (int i1 = 0; i1 < 88; i1++)
                        {
                            date_tmp[i1] = date[i + i1];
                            if (i1 != 86 && i1 != 87)
                            {
                                crc1 += date_tmp[i1];
                                crc = (short)crc1;
                            }

                        }
                        if (crc == BitConverter.ToInt16(date_tmp, 86))
                        {
                            state = true;
                        }
                        break;
                    }
                    else if (date[i + 1] == 0 && date.Length - i >= 47)
                    {
                        for (int i2 = 0; i2 < 47; i2++)
                        {
                            date_tmp[i2] = date[i + i2];
                            if (i2 != 45 && i2 != 46)
                            {
                                crc1 += date_tmp[i2];
                                crc = (short)crc1;
                            }

                        }
                        if (crc == BitConverter.ToInt16(date_tmp, 45))
                        {
                            state = true;
                        }
                        break;
                    }
                }
                state = false;
            }
            if (state == false) return; //устройство не читается
            else  // устройство прочитано (формируем данные)
            {
                Navigation_Date.DVL.Heading = BitConverter.ToInt16(date_tmp, 52) * 0.01;
                Navigation_Date.DVL.X_Vel = BitConverter.ToInt16(date_tmp, 5);
                Navigation_Date.DVL.Y_Vel = BitConverter.ToInt16(date_tmp, 7);
                Navigation_Date.DVL.Z_Vel = BitConverter.ToInt16(date_tmp, 9);
                Navigation_Date.DVL.Pitch = BitConverter.ToInt16(date_tmp, 48) * 0.01;
                Navigation_Date.DVL.Roll = BitConverter.ToInt16(date_tmp, 50) * 0.01;
                Navigation_Date.DVL.X_Ref_Vel = BitConverter.ToInt16(date_tmp, 22);
                Navigation_Date.DVL.Y_Ref_Vel = BitConverter.ToInt16(date_tmp, 24);
                Navigation_Date.DVL.Z_Ref_Vel = BitConverter.ToInt16(date_tmp, 26);
                Navigation_Date.DVL.Depth = BitConverter.ToInt16(date_tmp, 46);
                Navigation_Date.DVL.Temperature = BitConverter.ToInt16(date_tmp, 43) * 0.01;
                //Navigation_Date.DVL.Measured_Points; ***************************************обязательно добавить!!!!!

                Action update = async () => // асинхронно запуцскаем обновление интенрфейса
                {
                    await Objects_Links.M_Window?.Dispatcher.InvokeAsync(() => { Objects_Links.M_Window.update_DVL(); });
                };
                update();
                state = false;
            }
        }

    }

    public class TrackLink : AC_COM
    {
        


        public void clean()
        {
            close_port();
        }
        public TrackLink(COM_Date com)
        {
            devicename = com.DeviceName;
            //mySerialPort.PortName = com.PortName;
            //mySerialPort.BaudRate = (int)com.BaudRate;
            //mySerialPort.Parity = Parity.None;
            //mySerialPort.StopBits = StopBits.One;
            //mySerialPort.DataBits = (int)com.DataBits;
            //mySerialPort.Handshake = Handshake.None;
            //mySerialPort.RtsEnable = true;
            //mySerialPort.ReadTimeout = 300;
            list_f_info = new List<F_info>();
            list_f_info.Add(new F_info(devicename + " read", read, clean));
            list_f_info.Add(new F_info(devicename + " date_process", date_process));
        }
        public override void read()
        {
            string buffer =  (string)DevicesTest.TrackLinkTest.testread();
            //if (!open_port()) return;

            //string buffer;
            //for (;;)
            //{
            //    try
            //    {
            //        buffer = mySerialPort.ReadLine();
            //        if (buffer.Length > 5) break;
            //    }
            //    catch (Exception)
            //    {
            //        return;
            //    }
            //}
            queue.Enqueue(buffer);
        }

        delegate void For_Date_Proc(Device_Date.HANS.Beacon beacon);
        public override void date_process()
        {
            //LQF "9,05/04/17,14:54:31,  4250.5598,-14718.5044,  4250.5538,-14718.5110,153.7, 198.4\r"
            string date = "";
            //string[] b = new string[5];
            lock (queue)
            {
                if (queue.Count > 0)
                {
                    date = (string)queue.Dequeue();
                }
                else return;
            }


            if (date == null) return;
            try
            {
                string[] sep = date.Split(',');

                double b = double.Parse(sep[0], System.Globalization.CultureInfo.InvariantCulture);
                double s_lat = double.Parse(sep[3], System.Globalization.CultureInfo.InvariantCulture)/100;
                double s_long = double.Parse(sep[4], System.Globalization.CultureInfo.InvariantCulture)/100;
                double b_lat = double.Parse(sep[5], System.Globalization.CultureInfo.InvariantCulture)/100;
                double b_long = double.Parse(sep[6], System.Globalization.CultureInfo.InvariantCulture)/100;
                double s_h = double.Parse(sep[7], System.Globalization.CultureInfo.InvariantCulture);
                double b_d = double.Parse(sep[8], System.Globalization.CultureInfo.InvariantCulture);

                Navigation_Date.Ship.Heading = s_h;
                Navigation_Date.Ship.Position = new Geo_Pos(s_lat, s_long, null);


                For_Date_Proc Date_Proc = Beac =>
                {
                    Beac.Number = b;
                    Beac.Depth = b_d;
                    //Beac.X_offset = x;
                    //Beac.Y_offset = y;
                    Beac.Position = new Geo_Pos(b_lat, b_long, null);
                    //Action update = async () => // синхронно запускаем обновление интенрфейса исключительно async
                    //{
                    //    //await Objects_Links.M_Window?.Dispatcher.InvokeAsync(Objects_Links.M_Window.update_HANS);
                    //    await Objects_Links.M_Window?.Dispatcher.InvokeAsync(() => {
                    //        Objects_Links.M_Window.device_updater.HANS_update(true);
                    //        Objects_Links.M_Window.update_HANS();
                    //        Objects_Links.M_Window.device_updater.Ship_update(true);
                    //    }
                    //      Action update =  () => // синхронно запускаем обновление интенрфейса исключительно async
                    //      {
                    //          //await Objects_Links.M_Window?.Dispatcher.InvokeAsync(Objects_Links.M_Window.update_HANS);
                    //          Objects_Links.M_Window?.Dispatcher.InvokeAsync(() => {
                    //              Objects_Links.M_Window.device_updater.HANS_update(true);
                    //              Objects_Links.M_Window.update_HANS();
                    //              Objects_Links.M_Window.device_updater.Ship_update(true);
                    //          }

                    // );
                    //};
                    //update();
                    OnNewDate(Beac);
                };

                if (Navigation_Date.HANS.beacons.ContainsKey(b))
                {
                    Date_Proc(Navigation_Date.HANS.beacons[b]);
                    Status_List.push("Beacon number " + b + " Updated" + DateTimeOffset.Now.ToString());
                    return;
                }
                else
                {
                    Device_Date.HANS.Beacon beac = new Device_Date.HANS.Beacon();
                    Date_Proc(beac);
                    Navigation_Date.HANS.beacons.Add(b, beac);
                    Status_List.push("Beacon number " + b + " Created and Updated" + DateTimeOffset.Now.ToString());
                    return;
                }

            }
            catch (Exception e)
            {
                Status_List.push("Beacon date processing FALSE " + e.Message + " " + DateTimeOffset.Now.ToString());
                return;
            }
        }
    }
}
