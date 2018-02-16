using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Threading;

namespace NAVI_System
{
    public partial class MainWindow
    {

        public class DeviceUpdater
        {
            HANSUpdater hans_up = new HANSUpdater();
            class HANSUpdater
            {
                public struct M_date
                {
                    public double vert_distance;
                    public double hor_distance;
                    public double cable_lenght;
                    public double pix_rad;
                }
                Dictionary<string, Beacon> beacons = new Dictionary<string, Beacon>();
                MyTreeViewItem hans_viewitem;

                class Beacon
                {
                    public event EventHandler<M_date> newMesure;
                    void OnNewNesure(M_date e)
                    {
                        Volatile.Read(ref newMesure)?.Invoke(this, e);
                    }

                    Device_Date.HANS.Beacon my_beacon;
                    double number;

                    static Dictionary<string, Label> Nolist = new Dictionary<string, Label>();
                    static bool has_ROV = false;
                    static bool has_DP = false;

                    static double maxCableLenght = 90;
                    public static double MaxCableLenght { set { if (value > 0) maxCableLenght = value; } get { return maxCableLenght; } }

                    static Point ROV_p;
                    static Point DP_p;
                    static double ROV_d;
                    static double DP_d;


                    static M_date mesures;

                    bool filtred = true;
                    MyTreeViewItem hans;
                    Brush colour;
                    Ellipse el = new Ellipse() { Height = 20, Width = 20, Stroke = Brushes.Black};
                    string track_name;
                    Visibility vis = Visibility.Visible;
                    MyTreeViewItem viewitem;
                    Point label_pos = new Point();
                    FrameworkElement label;
                    Device_Date.HANS.Type T = Device_Date.HANS.Type.No;
                    Device_Date.HANS.Type set_type (Device_Date.HANS.Type value)
                    {
                            switch (value)
                            {
                                case Device_Date.HANS.Type.ROV:
                                    {
                                        switch (T)
                                        {
                                            case Device_Date.HANS.Type.DP:
                                                {
                                                    if(has_ROV) break;
                                                    else
                                                    {
                                                        T = value;
                                                        label.Visibility = Visibility.Hidden;
                                                        label = Objects_Links.M_Window.ROV;
                                                        label.Visibility = Visibility.Visible;
                                                        has_ROV = true;
                                                        
                                                        has_DP = false;
                                                    }
                                                }
                                                break;
                                            case Device_Date.HANS.Type.No:
                                                {
                                                    if (has_ROV) break;
                                                    else
                                                    {
                                                        if (Nolist.ContainsKey(track_name))
                                                        {
                                                            Objects_Links.M_Window.canvas.Children.Remove(Nolist[track_name]);
                                                            Objects_Links.M_Window.icons_for_zoom.Remove(Nolist[track_name]);
                                                            Nolist.Remove(track_name);
                                                        }
                                                        label = Objects_Links.M_Window.ROV;
                                                        label.Visibility = Visibility.Visible;
                                                        has_ROV = true;
                                                        
                                                        T = value;
                                                    }
                                                }
                                                break;
                                        }
                                        
                                    }
                                    break;
                                case Device_Date.HANS.Type.DP:
                                    switch (T)
                                    {
                                        case Device_Date.HANS.Type.ROV:
                                            {
                                                if (has_DP) break;
                                                else
                                                {
                                                    T = value;
                                                    label.Visibility = Visibility.Hidden;
                                                    label = Objects_Links.M_Window.DP;
                                                    label.Visibility = Visibility.Visible;
                                                    has_DP = true;
                                                    
                                                    has_ROV = false;
                                                }
                                            }
                                            break;
                                        case Device_Date.HANS.Type.No:
                                            {
                                                if (has_DP) break;
                                                else
                                                {
                                                    if (Nolist.ContainsKey(track_name))
                                                    {
                                                        Objects_Links.M_Window.canvas.Children.Remove(Nolist[track_name]);
                                                        Objects_Links.M_Window.icons_for_zoom.Remove(Nolist[track_name]);
                                                        Nolist.Remove(track_name);
                                                    }
                                                    label = Objects_Links.M_Window.DP;
                                                    label.Visibility = Visibility.Visible;
                                                    has_DP = true;
                                                    
                                                    T = value;
                                                }
                                            }
                                            break;
                                    }
                                    break;
                                case Device_Date.HANS.Type.No:
                                    {
                                        switch (T)
                                        {
                                            case Device_Date.HANS.Type.ROV:
                                                {
                                                    has_ROV = false;
                                                    label.Visibility = Visibility.Hidden;

                                                }
                                                break;
                                            case Device_Date.HANS.Type.DP:
                                                {
                                                    has_DP = false;
                                                    
                                                    label.Visibility = Visibility.Hidden;
                                                }
                                                break;
                                        }
                                        if (!Nolist.ContainsKey(track_name))
                                        {
                                            Label No = new Label() { Content = number.ToString(), FontSize = 16, Foreground = Objects_Links.Tracks.get_stroke(track_name) };
                                            Nolist.Add(track_name, No);
                                            Objects_Links.M_Window.canvas.Children.Add(No);
                                            Objects_Links.M_Window.icons_for_zoom.Add(No);
                                            label = No;
                                        }
                                        
                                        T = value;
                                    }
                                    break;
                            }

                        return T;
                    }
                    void set_lab_margin(FrameworkElement lab, Point pos)
                    {
                        try
                        {
                            if (lab != null)
                            {
                                if (T == Device_Date.HANS.Type.No) lab.Margin = new Thickness(pos.X, pos.Y, 0, 0);
                                else lab.Margin = new Thickness(pos.X - (lab.Width / 2), pos.Y - (lab.Height / 2), 0, 0);
                            }
                        }
                        catch (Exception e)
                        {
                            Status_List.push("set_lab_margin (type: "+ lab.GetType()+ " ): " +e.ToString());
                        }
                        
                    }
                    void set_vis(Visibility vis)
                    {
                        if (T == Device_Date.HANS.Type.DP)
                        {
                            label.Visibility = vis;
                            Objects_Links.Tracks.set_visibility(track_name, vis);
                            el.Visibility = vis;
                        }
                        else
                        {
                            label.Visibility = vis;
                            Objects_Links.Tracks.set_visibility(track_name, vis);
                        }
                    }
                    void mesure_update()
                    {
                        
                        if (has_ROV & has_DP)
                        {
                            mesures.vert_distance = Math.Round(Math.Abs(ROV_d - DP_d), 1);
                            mesures.hor_distance = Objects_Links.M_Window.mesh.l_mesure(ROV_p, DP_p);
                            double temp = Math.Round(Math.Sqrt(mesures.vert_distance * mesures.vert_distance + mesures.hor_distance * mesures.hor_distance), 2);

                            mesures.cable_lenght = temp > (maxCableLenght + 50) ? mesures.cable_lenght : temp;

                            double work_area_rad = Math.Round(Math.Sqrt(-mesures.vert_distance * mesures.vert_distance + maxCableLenght * maxCableLenght), 2);
                            mesures.pix_rad = Math.Round(work_area_rad * Objects_Links.M_Window.mesh.get_m_to_pix_coef(), 0);

                            if (mesures.pix_rad < 20) mesures.pix_rad=20;

                            el.Width = mesures.pix_rad * 2;
                            el.Height = mesures.pix_rad * 2;

                            OnNewNesure(mesures);
                        }

                        
                    }
                    public void Update()
                    {
                        if (filtred) label_pos = Objects_Links.Tracks.AddFiltredGeoPoint(track_name, my_beacon.Position.Longitude, my_beacon.Position.Latitude);
                        else label_pos = Objects_Links.Tracks.AddGeoPoint(track_name, my_beacon.Position.Longitude, my_beacon.Position.Latitude);
                        switch (T)
                        {
                            case Device_Date.HANS.Type.ROV:
                                {
                                    ROV_p = label_pos;
                                    ROV_d = my_beacon.Depth;
                                    Objects_Links.M_Window.ROV.RenderTransform = new RotateTransform(my_beacon.Heading);
                                }
                                break;
                            case Device_Date.HANS.Type.DP:
                                {
                                    if (!Objects_Links.M_Window.icons_for_zoom.Contains(el))
                                    {
                                        Objects_Links.M_Window.icons_for_zoom.Add(el);
                                        Objects_Links.M_Window.canvas.Children.Add(el);
                                    }
                                    DP_p = label_pos;
                                    DP_d = my_beacon.Depth;

                                    if (has_ROV & has_DP) set_lab_margin(el, label_pos);
                                }
                                break;
                        }

                        mesure_update();

                        set_lab_margin(label, label_pos);
                        viewitem.update();
                    }
                    public void delete()
                    {
                        hans.Items.Remove(viewitem);
                        set_type(Device_Date.HANS.Type.No);
                        if (Nolist.ContainsKey(track_name))
                        {
                            Objects_Links.M_Window.canvas.Children.Remove(Nolist[track_name]);
                            Objects_Links.M_Window.icons_for_zoom.Remove(Nolist[track_name]);
                            Nolist.Remove(track_name);
                        }

                    }
                    public Beacon(Device_Date.HANS.Beacon beacon, MyTreeViewItem h)
                    {
                        my_beacon = beacon;
                        viewitem = new MyTreeViewItem(beacon);
                        track_name = beacon.get_name(); number = beacon.Number; T = beacon.type;
                        if (!Objects_Links.M_Window.icons_for_zoom.Contains(Objects_Links.M_Window.DP)) Objects_Links.M_Window.icons_for_zoom.Add(Objects_Links.M_Window.DP);
                        if (!Objects_Links.M_Window.icons_for_zoom.Contains(Objects_Links.M_Window.ROV)) Objects_Links.M_Window.icons_for_zoom.Add(Objects_Links.M_Window.ROV);
                        (hans = h).Items.Add(viewitem);
                        set_type(T);
                        viewitem.typebox.SelectionChanged += Typebox_SelectionChanged;
                        viewitem.checkvis.Click += Checkvis_Click;
                    }

                    private void Checkvis_Click(object sender, RoutedEventArgs e)
                    {
                        if ((bool)viewitem.checkvis.IsChecked)
                        {
                            set_vis(Visibility.Hidden);
                        }
                        else
                        {
                            set_vis(Visibility.Visible);
                        }
                    }

                    private void Typebox_SelectionChanged(object sender, SelectionChangedEventArgs e)
                    {
                        if (((Label)viewitem.typebox.SelectedItem).Content.ToString() == "ROV")
                            switch (set_type(Device_Date.HANS.Type.ROV))
                            {
                                case Device_Date.HANS.Type.DP:
                                    viewitem.typebox.SelectedIndex=1;
                                    break;
                                case Device_Date.HANS.Type.No:
                                    viewitem.typebox.SelectedIndex = 2;
                                    break;
                            }
                        
                        if (((Label)viewitem.typebox.SelectedItem).Content.ToString() == "No")
                            switch (set_type(Device_Date.HANS.Type.No))
                            {
                                case Device_Date.HANS.Type.ROV:
                                    viewitem.typebox.SelectedIndex = 0;
                                    break;
                                case Device_Date.HANS.Type.DP:
                                    viewitem.typebox.SelectedIndex = 1;
                                    break;
                            }
                        if (((Label)viewitem.typebox.SelectedItem).Content.ToString() == "DP")
                            switch (set_type(Device_Date.HANS.Type.DP))
                            {
                                case Device_Date.HANS.Type.ROV:
                                    viewitem.typebox.SelectedIndex = 0;
                                    break;
                                case Device_Date.HANS.Type.No:
                                    viewitem.typebox.SelectedIndex = 2;
                                    break;
                            }
                    }
                }

                public void Update(Device_Date.HANS.Beacon beacon)
                {
                    if (hans_viewitem == null)
                    {
                        hans_viewitem = new MyTreeViewItem(Navigation_Date.HANS);
                        Objects_Links.M_Window.treeView.Items.Add(hans_viewitem);
                    }
                    string b_name = beacon.get_name();
                    if (beacons.ContainsKey(b_name)) beacons[b_name].Update();
                    else
                    {
                        
                        var b = new Beacon(beacon, hans_viewitem);
                        b.newMesure += B_newMesure;
                        beacons.Add(b_name, b);
                    }
                }

                private void B_newMesure(object sender, M_date e)
                {
                    Objects_Links.M_Window.CabletextBlock.Text = "Натяжение кабеля " +
                                    Environment.NewLine + e.cable_lenght.ToString(); /* + Environment.NewLine +
                                    "Горизонтальнo: " + e.hor_distance.ToString()+ Environment.NewLine +
                                    "Радиус: " + e.pix_rad.ToString();*/

                    Objects_Links.M_Window.DepthtextBlock.Text = "Разница глубин " +
                        Environment.NewLine + ((int)e.vert_distance).ToString();
                    if (Beacon.MaxCableLenght - 5 < e.cable_lenght) Objects_Links.M_Window.CabletextBlock.Background = Brushes.OrangeRed;
                    else Objects_Links.M_Window.CabletextBlock.Background = new SolidColorBrush(Color.FromRgb(241, 241, 241));
                }
            }
            
            

            double maxCableLenght = 80;
            int count = 0;
            Ellipse DPel = new Ellipse() { Width = 20, Height= 20, Stroke = Brushes.Black};
            List<AC_COM> list = new List<AC_COM>();
            public bool set_correctly { private set; get; }
            Dictionary<string, Label> Nolist = new Dictionary<string, Label>();

            void treeview_update(object target, string name)
            {
                int state = 0;
                foreach (var item in Objects_Links.M_Window.treeView.Items)
                {
                    if (item is MyTreeViewItem && ((MyTreeViewItem)item).myname == name)
                    {
                        ((MyTreeViewItem)item).update();
                        state = 1;
                    }
                }
                if(state == 0)
                {
                    if (target is Device_Date.Ship) Objects_Links.M_Window.treeView.Items.Add(new MyTreeViewItem(Navigation_Date.Ship));
                    if (target is Device_Date.DVL) Objects_Links.M_Window.treeView.Items.Add(new MyTreeViewItem(Navigation_Date.DVL));
                    if (target is Device_Date.HANS) Objects_Links.M_Window.treeView.Items.Add(new MyTreeViewItem(Navigation_Date.HANS));
        
                }
            }
            public void Ship_update(bool filtred = true)
            {
                
                if (Navigation_Date.Ship != null)
                {
                        if (Objects_Links.Tracks != null)
                        {
                        Point p = new Point();
                        if (filtred) p = Objects_Links.Tracks.AddFiltredGeoPoint(Navigation_Date.Ship.get_name(), Navigation_Date.Ship.Position.Longitude, Navigation_Date.Ship.Position.Latitude);
                        else p = Objects_Links.Tracks.AddGeoPoint(Navigation_Date.Ship.get_name(), Navigation_Date.Ship.Position.Longitude, Navigation_Date.Ship.Position.Latitude);
                        if (!Objects_Links.M_Window.icons_for_zoom.Contains(Objects_Links.M_Window.SVessel)) Objects_Links.M_Window.icons_for_zoom.Add(Objects_Links.M_Window.SVessel);
                        Objects_Links.M_Window.SVessel.Margin = new Thickness(p.X - (Objects_Links.M_Window.SVessel.Width/2), p.Y - (Objects_Links.M_Window.SVessel.Height / 2), 0,0);
                        Objects_Links.M_Window.SVessel.RenderTransform = new RotateTransform(Navigation_Date.Ship.Heading - 90);
                        Status_List.push("Ship_update" + Navigation_Date.Ship.Position.Longitude.ToString() + "  " + Navigation_Date.Ship.Position.Latitude.ToString());
                        treeview_update(Navigation_Date.Ship, Navigation_Date.Ship.get_name());
                        if (Objects_Links.M_Window.SVessel.Tag == null)
                        {
                            Objects_Links.M_Window.SVessel.Visibility = Visibility.Visible;
                            Objects_Links.M_Window.SVessel.Tag = 1;
                        }
                    }
                }
            }

            public void set_COM_list()
            {
                if (Objects_Links.S_Date.set_correctly)
                {
                    list = COM_Factory.factory(Objects_Links.S_Date.get_COM_list());
                    set_correctly = true;
                    foreach (var item in list)
                    {
                        if(item is TrackLink) item.NewDate += TrackLinkMsg;
                    }
                }
            }

            private async void TrackLinkMsg(object sender, object e)
            {
                try
                {
                    await Objects_Links.M_Window?.Dispatcher.InvokeAsync(() =>
                    {
                        //HANS_update(true);
                        hans_up.Update((Device_Date.HANS.Beacon)e);
                        Objects_Links.M_Window.update_HANS();
                        Ship_update(true);
                    });
                }
                catch(Exception e1) { Status_List.push("DeviceUpdater.TrackLinkMsg " + e1.Message + " " + DateTimeOffset.Now.ToString()); }
                
            }

            public void start_all_devices()
            {
                if (set_correctly)
                {
                    foreach (var item in list)
                    {
                        Objects_Links.T_Manager.start(item);
                    }
                }
            }
            public void start_device_by_name(string name)
            {
                if (set_correctly)
                {
                    foreach (var item in list)
                    {
                        if(item.devicename==name)Objects_Links.T_Manager.start(item);
                    }
                }
            }
            public void stop_device_by_name(string name)
            {
                if (set_correctly)
                {
                    foreach (var item in list)
                    {
                        if (item.devicename == name) Objects_Links.T_Manager.cansel(item);
                    }
                }
            }
            public void stopall_devices()
            {
                if (set_correctly)
                {
                    foreach (var item in list)
                    {
                        Objects_Links.T_Manager.cansel(item);
                    }
                }
            }



        }
    }
}
