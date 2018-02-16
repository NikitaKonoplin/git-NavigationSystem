using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace NAVI_System
{
    public static class DATE
    {
        public static string task1;
        public static string task2;
    }
    public abstract class Task_communication
    {
        
        public struct F_info
        {
            public F_info(string n_name, Action n_function, Action n_clean = null, int n_sleep_time = 30)
            {
                name = n_name;
                function = n_function;
                clean = n_clean;
                sleep_time = n_sleep_time;
            }
            public int sleep_time;
            public string name;
            public Action function;
            public Action clean; // действие перед остановкой задачи

        }
        public List<F_info> list_f_info;
    }
    public static class Objects_Links //одержит ссылки на объекты
    {
        public static MainWindow M_Window;
        public static Set_window S_Window;
        public static T_manager T_Manager;
        public static Set_Date S_Date;
        public static MainWindow.Tracks Tracks;
    }
    public static class Status_List // список событий системы
    {
        public static List<string> list = new List<string>();
        public static void push(string status)
        {
            list.Add(status);
            Console.WriteLine(status);
        }
    }
    public class Device_Date
    {
        public class Ship
        {
            public double Heading;
            public double V_Direction;
            public double Velocity;
            public double Altitude;
            public double Roll;
            public double Trim;
            public Geo_Pos Position;
            public string get_name()
            {
               return "Device_Date.Ship";
            }
            public override string ToString()
            {
                if (Position != null)
                    return (new StringBuilder().AppendFormat("Vessel Lat:{0} Lon:{1} H:{2}", Math.Round(Position.Latitude, 4), Math.Round(Position.Longitude, 4), Math.Round(Heading, 2))).ToString();
                else return "Device_Date.Ship";
            }
        }
        public class HANS
        {
            public enum Type
            {
                ROV, DP, No
            }
            public string get_name()
            {
                return "Acoustic system";
            }
            public class Beacon
            {
                public double X_offset;
                public double Y_offset;
                public double Depth;
                public double Heading;
                public Type type = Type.No;
                public double Number;
                public Geo_Pos Position;
                public override string ToString()
                {
                    if (Position != null)
                        return new StringBuilder().AppendFormat("B {0} Lat:{1} Lon:{2} D:{3}",
                            Number.ToString(), Math.Round(Position.Latitude, 4), Math.Round(Position.Longitude, 4), Math.Round(Depth, 1)).ToString();
                    else return "Device_Date.HANS.Beacon";
                    
                }
                public string get_name()
                {
                    return  Number.ToString();
                }
            }
            public HANS()
            {
                beacons = new Dictionary<double, Beacon>();
            }
            public Dictionary<double, Beacon> beacons;
        }
        public class DVL
        {
            public double Heading;
            public double X_Vel;
            public double Y_Vel;
            public double Z_Vel;
            public double Pitch;
            public double Roll;
            public double X_Ref_Vel;
            public double Y_Ref_Vel;
            public double Z_Ref_Vel;
            public double Depth;
            public double Temperature;
            public double[,] Measured_Points;
        }
    }
    public class COM_Date // содержит настройки порта
    {
        public string DeviceName;
        public string PortName;
        public double BaudRate;
        public double DataBits;
        //public double UV_Beacon;
        //public double DP_Beacon;
    }
    public static class Navigation_Date
    {
        public static Device_Date.Ship Ship = new Device_Date.Ship();
        public static Device_Date.DVL DVL = new Device_Date.DVL();
        public static Device_Date.HANS HANS = new Device_Date.HANS();

    }
    public class Geo_Pos
    {
       public double Latitude;
       public double Longitude;
        bool? N;
        bool? E;

        public Geo_Pos(double Lat, double Long, bool? _N)
        {
            //if (true)
            //{
            //    double[] Ship_E_N = GIS.GEO_to_UTM(Navigation_Date.Ship.Position.Latitude, Navigation_Date.Ship.Position.Longitude);
            //    double[] Beacon_E_N = GIS.GEO_to_UTM(Lat, Long);
            //    double[] X_Y_offset = { Ship_E_N[0] - Beacon_E_N[0], Ship_E_N[1] - Beacon_E_N[1] };

            //}
            Latitude=Lat;
            Longitude= Long;
            N = _N;
        }
        public Geo_Pos(double X_offset, double Y_offset) // пересчет смешения маяков относительно судна в географические координаты
        {
            if (Navigation_Date.Ship.Position.N == null) return;

            double[] Ship_E_N = GIS.GEO_to_UTM(Navigation_Date.Ship.Position.Latitude, Navigation_Date.Ship.Position.Longitude);
            double[] Beacon_E_N = new double[2];

            Beacon_E_N[1] = Ship_E_N[1] + (-X_offset * Math.Sin(Navigation_Date.Ship.Heading * 3.142 / 180) +
                        Y_offset * Math.Cos(Navigation_Date.Ship.Heading * 3.142 / 180));
            Beacon_E_N[0] = Ship_E_N[0] + (Y_offset * Math.Sin(Navigation_Date.Ship.Heading * 3.142 / 180) +
                X_offset * Math.Cos(Navigation_Date.Ship.Heading * 3.142 / 180));

            double[] long_lat = GIS.UTM_to_GEO(Beacon_E_N[0], Beacon_E_N[1], Navigation_Date.Ship.Position.Latitude > 0);
            Longitude = long_lat[0];
            Latitude = long_lat[1];
            N = Navigation_Date.Ship.Position.Latitude > 0;
        }
    }

   public class Set_Date // Данные настроек
    {
        public bool set_correctly;
        public int device_number {get; private set; }
        List<COM_Date> COM_List = new List<COM_Date>(); // список портов устройств
        System.IO.FileStream set_file;// = new System.IO.FileStream(Environment.CurrentDirectory.ToString() + "\\Settings.txt", System.IO.FileMode.OpenOrCreate);
        public Set_Date()
        {
            //init_set_date();
        }
        public void init_set_date() // чтение настроек из файла
        {
            
            StreamReader r_file = new StreamReader(Environment.CurrentDirectory.ToString() + "\\Settings.txt");
            COM_List.Clear();
            try
            {             
                List<string> file_lines = new List<string>();
                device_number = 0;
                string s;
                while ((s=r_file.ReadLine())!=null) { file_lines.Add(s); }; 
                for (int i = 0; i < file_lines.Count; i += 4)
                {
                    COM_Date COM = new COM_Date();
                    COM.DeviceName = file_lines[i].Substring(9);
                    if (COM.DeviceName.Length < 3)
                    {
                        break;
                    }
                    COM.PortName = file_lines[i + 1].Substring(9);
                    COM.BaudRate = double.Parse(file_lines[i + 2].Substring(9), System.Globalization.CultureInfo.InvariantCulture);
                    COM.DataBits = double.Parse(file_lines[i + 3].Substring(9), System.Globalization.CultureInfo.InvariantCulture);
                    /* Проверка правильности подключния устройств (при эксплуатации разкоментировать)
                    if (existing_ports.Contains(COM.PortName)) COM_List.Add(COM);
                    else Status_List.push(COM.PortName + " Does Not Exist " + DateTimeOffset.Now.ToString()); */
                    COM_List.Add(COM); // при эксплуатации закоментировать
                    device_number++;
                }
                set_correctly = true;
            }
            catch (Exception e)
            {
                Status_List.push("Read Settings File Error " +e.Message+ DateTimeOffset.Now.ToString());
                set_correctly = false;
            }
            finally{ r_file.Close(); }
        }
        public void manual_set(List<COM_Date> arr) // ручное формирование списка портов устройств (пользователем в Set_window)
        {
            //if (!init_set_file("2")) { set_correctly = false; return; };
            StreamWriter w_file = new StreamWriter(Environment.CurrentDirectory.ToString() + "\\Settings.txt", false);
            COM_List.Clear();
            try
            {
                int count = 1;
                device_number = 0;
                foreach (var item in arr)
                {
                    COM_List.Add(item);
                    w_file.WriteLine(count.ToString() + "-Device:" + item.DeviceName);
                    w_file.WriteLine("PortName:" + item.PortName);
                    w_file.WriteLine("BaudRate:" + item.BaudRate.ToString());
                    w_file.WriteLine("DataBits:" + item.DataBits.ToString());
                    count++;
                    device_number++;
                }
                set_correctly = true;
            }
            catch (IOException exc)
            {
                Status_List.push("Write Settings File Error " + exc.Message + DateTimeOffset.Now.ToString());
                set_correctly = false;
            }
            finally { w_file.Close(); }
            

        }
        public List<COM_Date> get_COM_list() // возвращает список портов устройств, если он сформирован
        {
            if (set_correctly) return COM_List;
            else return null;
        }
    }
}
