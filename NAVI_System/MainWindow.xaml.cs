using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Windows.Threading;

//!!!!!!!!!!!!!завершить все потоки перед закрытием!!!!!!!!!!!!!!!!!!!!!

namespace NAVI_System
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public Set_Date da = new Set_Date();
        Set_window win;
        //Task_manager tsk = new Task_manager();
        T_manager tsk = new T_manager();
        DVL dvl;
        Set_Date set_date = new Set_Date();
       public  Mesh mesh;
        Set_window s_w;
        public DeviceUpdater device_updater = new DeviceUpdater();
        public Tracks tracks;
        public MainWindow()
        {
            
            

            InitializeComponent();
            Objects_Links.M_Window = this;
            mesh = new Mesh(canvas, ScrollViewer1, grid_h_lab, grid_v_lab, label1, label2);
            Objects_Links.S_Date = new Set_Date();
            tracks = new Tracks(mesh, canvas, slider);
            Objects_Links.Tracks = tracks;
            Objects_Links.T_Manager = tsk;
            
            mesh.mesh();
            //mesh_grind(Map.Source.ToString());
            line.Stroke = Brushes.Red;
            canvas.Children.Add(line);
            //Navigation_Date.SonarDyne.beacons = new Dictionary<double, Device_Date.SonarDyne.Beacon>();
            //Device_Date.SonarDyne.Beacon beac = new Device_Date.SonarDyne.Beacon();
            //tes = new Test(dat.get_COM_list()[0]);
            //COM = COM_Factory.factory(dat.get_COM_list());
            Navigation_Date.DVL.Heading = 22;
            Navigation_Date.DVL.X_Vel = 22;
            Navigation_Date.DVL.Y_Vel = 33;
            Navigation_Date.DVL.Z_Vel = 44;
            Navigation_Date.DVL.Pitch = 34;
            Navigation_Date.DVL.Roll = 34;
            Navigation_Date.DVL.X_Ref_Vel = 34;
            Navigation_Date.DVL.Y_Ref_Vel = 34;
            Navigation_Date.DVL.Z_Ref_Vel = 34;
            Navigation_Date.DVL.Depth = 34;
            Navigation_Date.DVL.Temperature = 34;
            s_w = new Set_window(this);
            //DispatcherTimer dispatcherTimer = new DispatcherTimer();
            //dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            //dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            //dispatcherTimer.Start();
            grid_v_lab.Children.Add(can);
        }

        //private void dispatcherTimer_Tick(object sender, EventArgs e)
        //{
        //    label.Content = count;
        //    count = 0;
        //}

        int count = 0;
        public void  update_DVL()
        {
            //label_Copy.Content ="Heading: " +
            //Navigation_Date.DVL.Heading.ToString() + Environment.NewLine +
            // "X_vel: " +
            //Navigation_Date.DVL.X_Vel + Environment.NewLine +
            //"Y_vel: " + 
            //Navigation_Date.DVL.Y_Vel + Environment.NewLine +
            //"Z_vel: " + 
            //Navigation_Date.DVL.Z_Vel + Environment.NewLine +
            //"Pitch: " +
            //Navigation_Date.DVL.Pitch + Environment.NewLine +
            //"Roll: " +
            //Navigation_Date.DVL.Roll + Environment.NewLine +
            //"Temperature: " +
            //Navigation_Date.DVL.Temperature;
            //label.Dispatcher.InvokeAsync(() => label.Content += "azaza");
            //Console.WriteLine("azaza");
            //label.Content += "azaza_фаыфаыфафыафававфыаыфваы";
            //label.Content = count;

            count++;
            //if (Navigation_Date.DVL.Heading == 999) count = 0;
        }

        Polyline line = new Polyline();
        int x = 0;
        int y = 0;
        public void update_Test2()
        {
            
            label_Copy1.Content = DATE.task2; 

            //label.Dispatcher.InvokeAsync(() => label.Content += "azaza");
            //Console.WriteLine("azaza");
            //label.Content += "azaza_фаыфаыфафыафававфыаыфваы";
            //label.Content = count;

           // count++;
            //if (Navigation_Date.DVL.Heading == 999) count = 0;
        }
        int ab = 0;
        public void update_SonarDyne()
        {
            ab++;
            label_Copy1.Content = ab+ "__";
            foreach (var Beac in Navigation_Date.HANS.beacons)
            {
                label_Copy1.Content += "Beacon number: " +
            Beac.Value.Number.ToString() + Environment.NewLine +
            "Depth: " +
             Beac.Value.Depth.ToString() + Environment.NewLine +
             "X_offset: " +
             Beac.Value.X_offset.ToString() + Environment.NewLine +
            "Y_offset: " +
             Beac.Value.Y_offset.ToString() + Environment.NewLine +
             "Latitude "; //+
            //Beac.Value.Position.Latitude.ToString() + Environment.NewLine +
            //"Longitude " +
            //Beac.Value.Position.Longitude.ToString() + Environment.NewLine;
            }
            
            
            //label.Dispatcher.InvokeAsync(() => label.Content += "azaza");
            //Console.WriteLine("azaza");
            //label.Content += "azaza_фаыфаыфафыафававфыаыфваы";
            //label.Content = count;

            count++;
            //if (Navigation_Date.DVL.Heading == 999) count = 0;
        }

        public void update_HANS()
        {
            ab++;
            label_Copy1.Content = ab + "__";
            foreach (var Beac in Navigation_Date.HANS.beacons)
            {
                label_Copy1.Content += "Beacon number: " +
            Beac.Value.Number.ToString() + Environment.NewLine +
            "Depth: " +
             Beac.Value.Depth.ToString() + Environment.NewLine +
             "ROV Lat: " +
             Beac.Value.Position.Latitude.ToString() + Environment.NewLine +
            "ROV Long: " +
             Beac.Value.Position.Longitude.ToString() + Environment.NewLine +
              "Vessel Lat: " +
             Navigation_Date.Ship.Position.Latitude.ToString() + Environment.NewLine +
            "Vessel Long: " +
             Navigation_Date.Ship.Position.Longitude.ToString() + Environment.NewLine +
             "Vessel Head: " +
             Navigation_Date.Ship.Heading.ToString() + Environment.NewLine;
            }


            //label.Dispatcher.InvokeAsync(() => label.Content += "azaza");
            //Console.WriteLine("azaza");
            //label.Content += "azaza_фаыфаыфафыафававфыаыфваы";
            //label.Content = count;

            //count++;
            //if (Navigation_Date.DVL.Heading == 999) count = 0;
        }

        public void tas()
        {
            Action update = async () => // асинхронно запуцскаем обновление интенрфейса
            {
                await Objects_Links.M_Window?.Dispatcher.InvokeAsync(() => { Objects_Links.M_Window.update_DVL(); });
                //int a = 3;
            };
            update();
            //Thread.Sleep(10);
            //label.Dispatcher.InvokeAsync(()=> label.Content = Thread.CurrentThread.Name);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Objects_Links.Tracks.RemoveAllFiltred();
            Objects_Links.Tracks.RemoveAll();
            //string date = "9,05/04/17,14:54:31,  4250.5598,-14718.5044,  4250.5538,-14718.5110,153.7, 198.4\r";

            //string[] sep = date.Split(',');

            //double b = double.Parse(sep[0], System.Globalization.CultureInfo.InvariantCulture);
            //double s_lat = double.Parse(sep[3], System.Globalization.CultureInfo.InvariantCulture);
            //double s_long = double.Parse(sep[4], System.Globalization.CultureInfo.InvariantCulture);
            //double b_lat = double.Parse(sep[5], System.Globalization.CultureInfo.InvariantCulture);
            //double b_long = double.Parse(sep[6], System.Globalization.CultureInfo.InvariantCulture);
            //double s_h = double.Parse(sep[7], System.Globalization.CultureInfo.InvariantCulture);
            //double b_d= double.Parse(sep[8], System.Globalization.CultureInfo.InvariantCulture);



            //string ship_lat = date.Substring(date.IndexOf(',', 0, 3), date.IndexOf(',', 0, 4) - date.IndexOf(',', 0, 3));
            //string ship_long = date.Substring(date.IndexOf(',', 0, 4), date.IndexOf(',', 0, 5) - date.IndexOf(',', 0, 4));
            //string beak_lat = date.Substring(date.IndexOf(',', 0, 5), date.IndexOf(',', 0, 6) - date.IndexOf(',', 0, 5));
            //string beak_long = date.Substring(date.IndexOf(',', 0, 6), date.IndexOf(',', 0, 7) - date.IndexOf(',', 0, 6));
            //string ship_head = date.Substring(date.IndexOf(',', 0, 7), date.IndexOf(',', 0, 8) - date.IndexOf(',', 0, 7));
            //string beak_depth = date.Substring(date.IndexOf(',', 0, 8), date.IndexOf(',', 0, 9) - date.IndexOf(',', 0, 8));

            //int aa =4;
            //Task task = new Task(tas);
            //task.Start();
            //task.Wait();

            ////label.Content +="bebebe";
            ////task.Wait();
            //Console.WriteLine("azaza");
            //label.Content += task.IsCompleted.ToString();
            ////win = new Set_window(this);
            ////win.Show();
        } 

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (s_w == null)
            {
                s_w = new Set_window(this);
            }
            s_w.Show();


        }
        List<AC_COM> COM;
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            mesh.empty_map();
            //foreach (var item in COM)
            //{
            //    tsk.start(item);
            //}

            //Set_Date dat = new Set_Date();

            //Task.Run(()=> tsk.start_tasks(dat.get_COM_list()));
        }

        private void button3k_Click(object sender, RoutedEventArgs e)
        {
            //foreach (var item in COM)
            //{
            //    tsk.cansel(item);
            //}
            device_updater.stopall_devices();

            //Set_Date dat = new Set_Date();
            //await Task.Run(() => tsk.cancel_tasks(dat.get_COM_list()));
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Desired_map"; // Default file name
            dlg.DefaultExt = ".jpg"; // Default file extension
            dlg.Filter = "Map type (.jpg)|*.jpg"; // Filter files by extension

            // Show open file dialog box
            bool? result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                Map.Source = new BitmapImage(new Uri(dlg.FileName));
                Map.Tag = null;

                //mesh_grind(Map.Source.ToString());
                mesh.mesh();
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            mouse_scroll(e);
            mesh.mouse_move(e);
            mesh.show_m_track(e.GetPosition(canvas));
        }
        public List<FrameworkElement> icons_for_zoom = new List<FrameworkElement>();
        private void button4_Click(object sender, RoutedEventArgs e)
        {
           zoom(1.1);
           
        }

        private void button4_Copy_Click(object sender, RoutedEventArgs e)
        {
            zoom(0.9);
            //mesh.zoom(0.9);
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            mesh.move_to_center();
            
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            //mesh.move_to_center();
            //mesh.update_labels();
        }
        int i = 1;
        Grid can = new Grid()
        {
            HorizontalAlignment = HorizontalAlignment.Right,
        };

        private void ScrollViewer1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //zoom((ScrollViewer1.ActualHeight/canvas.ActualHeight));
            grid_v_lab.Width = ScrollViewer1.ActualHeight;
            mesh.mesh();
        }

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(button8.IsChecked==true)mesh.mes_state = true;
            mesh.add_mes_lines(e.GetPosition(canvas));
        }

        private void canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (button8.IsChecked == true)
            {
                if (mesh.mes_lines.Count > 0)
                {
                    if (!mesh.mes_state)
                    {
                        foreach (Line item in mesh.mes_lines)
                        {
                            canvas.Children.Remove(item);
                        }
                        foreach (Ellipse item in mesh.mes_elipse)
                        {
                            canvas.Children.Remove(item);
                        }
                        mesh.mes_lines.Clear();
                        mesh.mes_elipse.Clear();
                        if (canvas.Children.Contains(mesh.mes_lab)) canvas.Children.Remove(mesh.mes_lab);

                        mesh.distance = 0;
                    }
                    else
                    {
                        mesh.add_mes_lines(new Point(mesh.mes_lines.Last().X1, mesh.mes_lines.Last().Y1));
                        mesh.show_m_track(new Point(mesh.mes_lines.Last().X1, mesh.mes_lines.Last().Y1));
                        mesh.mes_state = false;
                    }
                    

                }
                

            }
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            device_updater.set_COM_list();
            device_updater.start_all_devices();

            //mesh.empty_map(133, 45);



            //Ellipse el = new Ellipse()
            //{
            //    Stroke = Brushes.Black,
            //    Fill = Brushes.Black,
            //    StrokeThickness = 5,
            //    Height = 10,
            //    Width = 10,
            //    Margin = new Thickness(100 , 100 , 0, 0),

            //};
            //canvas.Children.Add(el);
        }

        private void button8_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void button8_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (s_w != null)
            {
                s_w.to_close = true;
                s_w.Close();
            }
        }

        private void button7_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            device_updater.stopall_devices();
        }
        public class AAA
        {
            public string name = "AAAA";
            public override string ToString()
            {
                return "AAAA";
            }
        }
        private void button9_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
