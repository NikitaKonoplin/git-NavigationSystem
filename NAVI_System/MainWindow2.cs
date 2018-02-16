﻿using System.Threading;
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
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;

namespace NAVI_System
{
    public partial class MainWindow
    {
        

        public class Tracks
        {
            //class KalmanFilterSimple1D
            //{
            //    public double X0 { get; private set; } // predicted state
            //    public double P0 { get; private set; } // predicted covariance

            //    public double F { get; private set; } // factor of real value to previous real value
            //    public double Q { get; private set; } // measurement noise
            //    public double H { get; private set; } // factor of measured value to real value
            //    public double R { get; private set; } // environment noise

            //    public double State { get; private set; }
            //    public double Covariance { get; private set; }

            //    public KalmanFilterSimple1D(double q, double r, double f = 1, double h = 1)
            //    {
            //        Q = q;
            //        R = r;
            //        F = f;
            //        H = h;
            //    }

            //    public void SetState(double state, double covariance)
            //    {
            //        State = state;
            //        Covariance = covariance;
            //    }

            //    public void Correct(double data)
            //    {
            //        //time update - prediction
            //        X0 = F * State;
            //        P0 = F * Covariance * F + Q;

            //        //measurement update - correction
            //        double K = H * P0 / (H * P0 * H + R);
            //        State = X0 + K * (data - H * X0);
            //        Covariance = (1 - K * H) * P0;
            //    }
            //}
            // KalmanFilterSimple1D kalman1 = new KalmanFilterSimple1D(f: 1, h: 1, q: 2, r: 50);
            // KalmanFilterSimple1D kalman2 = new KalmanFilterSimple1D(f: 1, h: 1, q: 2, r: 50);
            Mesh mesh;
            Canvas canvas;
            Slider slider;
            Dictionary<string, Track> tracks = new Dictionary<string, Track>();
            public Dictionary<string, Track> _tracks { get { return tracks; }}
            Dictionary<string, Track> f_tracks = new Dictionary<string, Track>();
            int max_last_points = 10;
            public class Track
            {
                public Polyline track;
                Brush stroke = Brushes.Green;
                Visibility vis = Visibility.Visible;
                public Visibility Visibility { get { return vis; }
                set {
                        vis = value;
                        track.Visibility = vis;
                        foreach (Ellipse el in last_points)
                        {
                            el.Visibility = vis;
                        }
                    }
                }
                public Brush Stroke {get { return stroke; }
                   set
                    {
                        stroke = value;
                        track.Stroke = stroke;
                        foreach (Ellipse el in last_points)
                        {
                            el.Stroke = stroke;
                        }
                    }
                }
                public List<Ellipse> last_points = new List<Ellipse>();
                public Track(Polyline tr)
                {
                    track = tr;
                    track.Stroke = stroke;
                }
            }
            public void set_visibility(string track_name, Visibility vis)
            {
                if (f_tracks.ContainsKey(track_name))
                {
                    f_tracks[track_name].Visibility = vis;
                }
                if (tracks.ContainsKey(track_name))
                {
                    tracks[track_name].Visibility = vis;
                }
            }
            public Tracks(Mesh m, Canvas c, Slider s)
            {
                mesh = m;
                canvas = c;
                slider = s;
            }
            public void change_stroke(string track_name, Brush stroke)
            {
                if (f_tracks.ContainsKey(track_name))
                {
                    f_tracks[track_name].Stroke = stroke;

                }
                if (tracks.ContainsKey(track_name))
                {
                    tracks[track_name].Stroke = stroke;
                }
            }
            public Brush get_stroke(string track_name)
            {
                if (f_tracks.ContainsKey(track_name))
                {
                   return f_tracks[track_name].Stroke;
                }
                if (tracks.ContainsKey(track_name))
                {
                  return  tracks[track_name].Stroke;
                }
                return Brushes.Green;
            }
            public void update_tracks(double coef)
            {

                
                
                foreach (Track tr in f_tracks.Values)
                {
                    foreach (Ellipse el in tr.last_points)
                    {
                        el.Margin = new Thickness((el.Margin.Left+(el.Width/2))*coef - (el.Width / 2), (el.Margin.Top + (el.Height / 2)) * coef - (el.Height / 2), 0, 0);
                    }
                }
            }

            public void AddPixPoint(string track_name, double x_pix, double y_pix)
            {
                if (tracks.ContainsKey(track_name))
                {
                    tracks[track_name].track.Points.Add(new Point(x_pix, y_pix));
                }
                else
                {
                    Add(track_name, new Polyline() { Stroke = Brushes.Black, StrokeThickness = 3 });
                    AddPixPoint(track_name, x_pix, y_pix);
                }
            }
            public void AddFiltredPixPoint(string track_name, double x_pix, double y_pix)
            {
                if (f_tracks.ContainsKey(track_name))
                {
                    if (f_tracks[track_name].track.Points.Count > 0)
                    {
                        Point p = f_tracks[track_name].track.Points.Last();
                        double X = slider.Value * x_pix + (1 - slider.Value) * p.X;
                        double Y = slider.Value * y_pix + (1 - slider.Value) * p.Y;
                        f_tracks[track_name].track.Points.Add(new Point(X, Y));
                    }
                    else
                    {
                        f_tracks[track_name].track.Points.Add(new Point(x_pix, y_pix));
                    }
                    Ellipse el = new Ellipse()
                    {
                        StrokeThickness = 4,
                        Visibility = f_tracks[track_name].Visibility,
                        Height = 4,
                        Width = 4,
                        Margin = new Thickness(x_pix - 2, y_pix - 2, 0, 0),
                        Stroke = f_tracks[track_name].Stroke
                    };
                    f_tracks[track_name].last_points.Add(el);
                    canvas.Children.Add(el);
                    if (f_tracks[track_name].last_points.Count > max_last_points)
                    {
                        canvas.Children.Remove(f_tracks[track_name].last_points.First());
                        f_tracks[track_name].last_points.Remove(f_tracks[track_name].last_points.First());
                    }
                }
                else
                {
                    AddFiltred(track_name, new Polyline() { Stroke = Brushes.Green, StrokeThickness = 3, StrokeLineJoin = PenLineJoin.Round });
                    AddFiltredPixPoint(track_name, x_pix, y_pix);
                }
            }
            //public void AddKFiltredPixPoint(string track_name, double x_pix, double y_pix)
            //{
            //    if (f_tracks.ContainsKey(track_name))
            //    {
            //        if (f_tracks[track_name].Points.Count > 0)
            //        {
            //            kalman1.Correct(x_pix);
            //            kalman2.Correct(y_pix);

            //            f_tracks[track_name].Points.Add(new Point(kalman1.State, kalman2.State));
            //        }
            //        else
            //        {
            //            kalman1.SetState(x_pix, 0.1);
            //            kalman2.SetState(y_pix, 0.1);

            //            kalman1.Correct(x_pix);
            //            kalman2.Correct(y_pix);

            //            f_tracks[track_name].Points.Add(new Point(kalman1.State, kalman2.State));
                        
            //        }
            //    }
            //}
            public Point AddGeoPoint(string track_name, double lon, double lat)
            {
                Point p = mesh.GeoToPix(lon, lat);
                if (tracks.ContainsKey(track_name))
                {
                    AddPixPoint(track_name, p.X, p.Y);
                }
                else
                {
                    Add(track_name, new Polyline() { Stroke = Brushes.Black, StrokeThickness = 3 });
                    AddPixPoint(track_name, p.X, p.Y);
                }
                if (tracks.ContainsKey(track_name)) p = tracks[track_name].track.Points.Last();
                return p;
            }

            public Point AddFiltredGeoPoint(string track_name, double lon, double lat)
            {
                Point p = mesh.GeoToPix(lon, lat);
                AddFiltredPixPoint(track_name, p.X, p.Y);
                if (f_tracks.ContainsKey(track_name)) p = f_tracks[track_name].track.Points.Last();
                return p;
            }
            void Add(string track_name, Polyline line)
            {
                Track tr = new Track(line);
                if (!tracks.ContainsKey(track_name))
                {
                    tracks.Add(track_name, tr);
                    canvas.Children.Add(tr.track);
                }
            }
            void AddFiltred(string track_name, Polyline line)
            {
                Track tr = new Track(line);
                if (!f_tracks.ContainsKey(track_name))
                {
                    f_tracks.Add(track_name, tr);
                    canvas.Children.Add(tr.track);
                }
            }
            public void Remove(string track_name)
            {
                if (tracks.ContainsKey(track_name))
                { 
                    canvas.Children.Remove(tracks[track_name].track);
                    foreach (Ellipse item in tracks[track_name].last_points)
                    {
                        canvas.Children.Remove(item);
                    }
                        tracks.Remove(track_name);
                }
            }
        
            public void RemoveAll()
            {
                foreach (Track item in tracks.Values)
                {
                    canvas.Children.Remove(item.track);
                    foreach (Ellipse item1 in item.last_points)
                    {
                        canvas.Children.Remove(item1);
                    }
                }
                tracks.Clear();
            }
            public void RemoveFiltred(string track_name)
            {
                if (f_tracks.ContainsKey(track_name))
                {
                    canvas.Children.Remove(f_tracks[track_name].track);
                    foreach (Ellipse item in f_tracks[track_name].last_points)
                    {
                        canvas.Children.Remove(item);
                    }
                    f_tracks.Remove(track_name);
                }
            }
            public void RemoveAllFiltred()
            {
                foreach (Track item in f_tracks.Values)
                {
                    canvas.Children.Remove(item.track);
                    foreach (Ellipse item1 in item.last_points)
                    {
                        canvas.Children.Remove(item1);
                    }
                }
                f_tracks.Clear();
            }
        }

        public class Mesh
        {
            Grid grid = new Grid();
            Canvas canvas;
            Grid h_lab_can = new Grid();
            Grid v_lab_can = new Grid();
            Grid s_viewer;
            int pix_step = 100;
            double current_pix_step = 30;
            double grid_size = 3;
            int devision_coef = 2;
            double grid_old_width=new double();
            Grid grid_h_label;
            Grid grid_v_label;
            TextBlock t_b;
            //double lab_width = 0;
            double X1 = 0;
            double X2 = 100;
            double Y1 = 0;
            double Y2 = 100;
            double coefficient_X1 = 0;
            double coefficient_Y1 = 0;
            Label h_lab;
            Label v_lab;

            public List<Line> mes_lines = new List<Line>();
            public List<Ellipse> mes_elipse = new List<Ellipse>();
            public Label mes_lab = new Label();
            public double distance = 0;
            public bool mes_state = false;
            public void update_mes(double coef)
            {
               
                for (int i = 0; i < mes_lines.Count; i++)
                {
                    mes_lines[i].X1 *= coef;
                    mes_lines[i].Y1 *= coef;
                    mes_lines[i].X2 *= coef;
                    mes_lines[i].Y2 *= coef;
                }
                for (int i = 0; i < mes_elipse.Count; i++)
                {
                    Ellipse el = mes_elipse[i];
                    el.Margin = new Thickness((el.Margin.Left + (el.Width / 2)) * coef - (el.Width / 2), (el.Margin.Top + (el.Height / 2)) * coef - (el.Height / 2), 0, 0);
                    //mes_elipse[i].Margin = new Thickness(mes_lines[i].X1 - mes_elipse[i].Width / 2, mes_lines[i].Y1 - mes_elipse[i].Height / 2, 0, 0);
                }
                mes_lab.Margin = new Thickness(mes_lab.Margin.Left * coef, mes_lab.Margin.Top * coef, 0, 0);
            }
            public void show_m_track(Point p)
            {
                if (Objects_Links.M_Window.button8.IsChecked == true && mes_state && mes_lines.Count > 0)
                {
                    mes_lines.Last().X2 = p.X;
                    mes_lines.Last().Y2 = p.Y;

                    mes_lab.Content = "d=" + ((int)distance + (int)l_mesure(new Point(mes_lines.Last().X1, mes_lines.Last().Y1),
                        new Point(mes_lines.Last().X2, mes_lines.Last().Y2)));
                    mes_lab.Margin = new Thickness(p.X + 4, p.Y, 0, 0);
                }
            }
            public void add_mes_lines(Point p)
            {
                if (Objects_Links.M_Window.button8.IsChecked == true && mes_state)
                {
                    if (mes_lines.Count > 0)
                    {
                        mes_lines.Last().X2 = p.X;
                        mes_lines.Last().Y2 = p.Y;

                        distance += l_mesure(new Point(mes_lines.Last().X1, mes_lines.Last().Y1),
                        new Point(mes_lines.Last().X2, mes_lines.Last().Y2));
                        Ellipse el = new Ellipse()
                        {
                            StrokeThickness = 4,
                            Height = 4,
                            Width = 4,
                            Margin = new Thickness(p.X - 2, p.Y - 2, 0, 0),
                            Stroke = Brushes.Brown
                        };
                        Line l = new Line()
                        {
                            Stroke = Brushes.Brown,
                            StrokeThickness = 2,
                            StrokeDashArray = { 4 },
                            X1 = p.X,
                            Y1 = p.Y,
                            X2 = p.X,
                            Y2 = p.Y
                        };

                        mes_lines.Add(l);
                        mes_elipse.Add(el);
                        canvas.Children.Add(l);
                        canvas.Children.Add(el);
                    }
                    else
                    {
                        Line l = new Line()
                        {
                            Stroke = Brushes.Brown,
                            StrokeThickness = 2,
                            StrokeDashArray = { 4 },
                            X1 = p.X,
                            Y1 = p.Y,
                            X2 = p.X,
                            Y2 = p.Y
                        };
                        Ellipse el = new Ellipse()
                        {
                            StrokeThickness = 4,
                            Height = 4,
                            Width = 4,
                            Margin = new Thickness(p.X - 2, p.Y - 2, 0, 0),
                            Stroke = Brushes.Brown
                        };

                        mes_elipse.Add(el);
                        mes_lines.Add(l);
                        canvas.Children.Add(l);
                        canvas.Children.Add(el);

                        mes_lab.Margin = new Thickness(p.X + 4, p.Y, 0, 0);
                        if (!canvas.Children.Contains(mes_lab)) canvas.Children.Add(mes_lab);
                        if (!canvas.Children.Contains(mes_lab))
                        {
                            canvas.Children.Add(mes_lab);
                        }
                    }
                }
            }

            public Point GeoToPix(double lon, double lat) // Работает некорректно
            {
                coefficient_X1 = canvas.Width / Math.Abs(X2 - X1);
                coefficient_Y1 = canvas.Height / Math.Abs(Y2 - Y1);
                
                try
                {
                    return new Point((lon - X1) * coefficient_X1, (Y1 - lat) * coefficient_Y1);
                }
                catch (Exception e)
                {
                    Status_List.push("GeoToPix() ERROR " + e.Message + " " + DateTimeOffset.Now.ToString());
                    return new Point();
                }
            }
            public Point PixToGeo(double x, double y)
            {
                coefficient_X1 = Math.Abs(X2 - X1) / canvas.ActualWidth;
                coefficient_Y1 = Math.Abs(Y2 - Y1) / canvas.ActualHeight;
                try
                {
                    return new Point(X1 + (x* coefficient_X1), Y1 - (y * coefficient_Y1));
                }
                catch (Exception e)
                {
                    Status_List.push("PixToGeo() ERROR " + e.Message + " " + DateTimeOffset.Now.ToString());
                    return new Point();
                }
            }
            /// <summary>
            /// Измерение растояния между точками на карте заданными в географических координатах
            /// </summary>
            /// <param name="p1">точка 1 (x-long,y-lat)</param>
            /// <param name="p2">точка 2 (x-long,y-lat)</param>
            /// <returns>растояние в метрах</returns>
            public double l_mesure_geo(Point p1, Point p2)
            {
                try
                {
                    double[] h_v_UTM1 = GIS.GEO_to_UTM(p1.Y, p1.X);
                    double[] h_v_UTM2 = GIS.GEO_to_UTM(p2.Y, p2.X);
                    return Math.Sqrt(Math.Pow(Math.Abs(h_v_UTM1[0] - h_v_UTM2[0]), 2) +
                        Math.Pow(Math.Abs(h_v_UTM1[1] - h_v_UTM2[1]), 2));
                }
                catch (Exception e)
                {
                    Status_List.push("l_mesure() ERROR " + e.Message + " " + DateTimeOffset.Now.ToString());
                    return 0;
                }
            }
            /// <summary>
            /// Измерение растояния между точками на карте заданными в пикселях
            /// </summary>
            /// <param name="p1">точка 1</param>
            /// <param name="p2">точка 2</param>
            /// <returns>растояние в метрах</returns>
            public double l_mesure(Point p1, Point p2)
            {
                try
                {
                    Point _p1 = PixToGeo(p1.X, p1.Y);
                    Point _p2 = PixToGeo(p2.X, p2.Y);
                    double[] h_v_UTM1 = GIS.GEO_to_UTM(_p1.Y, _p1.X);
                    double[] h_v_UTM2 = GIS.GEO_to_UTM(_p2.Y, _p2.X);
                    return Math.Sqrt(Math.Pow(Math.Abs(h_v_UTM1[0] - h_v_UTM2[0]), 2) +
                        Math.Pow(Math.Abs(h_v_UTM1[1] - h_v_UTM2[1]), 2));
                }
                catch (Exception e)
                {
                    Status_List.push("l_mesure() ERROR " + e.Message + " " + DateTimeOffset.Now.ToString());
                    return 0;
                }
            }
            public void empty_map(double lon = 0, double lat = 0, int size = 10000)
            {

                if(lon==0 && lat == 0 ) 
                {
                    if (Navigation_Date.Ship.Position==null 
                        || Navigation_Date.Ship.Position.Latitude == 0 
                        && Navigation_Date.Ship.Position.Longitude == 0) return;

                    empty_map(Navigation_Date.Ship.Position.Longitude, Navigation_Date.Ship.Position.Latitude);

                }
                else
                {
                    Objects_Links.Tracks.RemoveAllFiltred(); 
                    double[] h_v_UTM = GIS.GEO_to_UTM(lat, lon);
                    if (h_v_UTM == null) return;

                    double[] x1_y1_UTM = { h_v_UTM[0] - (size / 2), h_v_UTM[1] + (size / 2) };
                    double[] x2_y2_UTM = { h_v_UTM[0] + (size / 2), h_v_UTM[1] - (size / 2) };
                    double[] x1_y1_GEO = GIS.UTM_to_GEO(x1_y1_UTM[0], x1_y1_UTM[1], lat > 0);
                    double[] x2_y2_GEO = GIS.UTM_to_GEO(x2_y2_UTM[0], x2_y2_UTM[1], lat > 0);


                    Objects_Links.M_Window.Map.Source = Objects_Links.M_Window.new_map.Source;
                    Objects_Links.M_Window.Map.Width = canvas.ActualWidth;
                    canvas.Height = canvas.Width;
                    Objects_Links.M_Window.Map.Height = canvas.ActualWidth;
                    Objects_Links.M_Window.Map.Tag = ("/" + x1_y1_GEO[0].ToString() + "$" +
                        x2_y2_GEO[0].ToString() + "$" + x1_y1_GEO[1].ToString() + "$" +
                        x2_y2_GEO[1].ToString() + "!azaza.jpg").Replace(',', '.');

                    this.mesh();
                }
            }
            public Mesh(Canvas can, Grid viewer, Grid g_h, Grid g_v, Label l1, Label l2)
            {
                canvas = can;
                s_viewer = viewer;
                grid_h_label = g_h;
                grid_v_label = g_v;
                h_lab = l1;
                v_lab = l2;


                grid_h_label.Children.Add(h_lab_can);
                grid_v_label.Children.Add(v_lab_can);

                h_lab_can.HorizontalAlignment = HorizontalAlignment.Left;
                v_lab_can.HorizontalAlignment = HorizontalAlignment.Right;

                canvas.Children.Add(grid);
            }
            bool set_long_lat()
            {
                try
                {
                    string path;
                    if (Objects_Links.M_Window.Map.Tag!=null)
                        path = (string)Objects_Links.M_Window.Map.Tag;
                    else
                        path = Objects_Links.M_Window.Map.Source.ToString();
                    
                    if ( path!= null)
                    {
                        string imagepath = path;
                        int ind = imagepath.LastIndexOf("/");
                        //imagepath.Substring(ind);
                        imagepath = imagepath.Substring(ind).Remove(0, 1);
                        imagepath = imagepath.Remove(imagepath.LastIndexOf("!"));
                        string sY2 = imagepath.Substring(imagepath.LastIndexOf("$")).Remove(0, 1);
                        Y2 = double.Parse(sY2, System.Globalization.CultureInfo.InvariantCulture);
                        imagepath = imagepath.Remove(imagepath.LastIndexOf("$"));
                        string sY1 = imagepath.Substring(imagepath.LastIndexOf("$")).Remove(0, 1);
                        Y1 = double.Parse(sY1, System.Globalization.CultureInfo.InvariantCulture);
                        imagepath = imagepath.Remove(imagepath.LastIndexOf("$"));
                        string sX2 = imagepath.Substring(imagepath.LastIndexOf("$")).Remove(0, 1);
                        X2 = double.Parse(sX2, System.Globalization.CultureInfo.InvariantCulture);
                        imagepath = imagepath.Remove(imagepath.LastIndexOf("$"));
                        X1 = double.Parse(imagepath, System.Globalization.CultureInfo.InvariantCulture);
                        //textBlock1.Text = X1.ToString() +"-" + X2.ToString()+"-"+Y1.ToString()+"-"+Y2.ToString();
                    }
                    //-----------------------------------------------------------------------------------------------------
                    double[] h_v_UTM1 = GIS.GEO_to_UTM(Y1, X1);
                    double[] h_v_UTM2 = GIS.GEO_to_UTM(Y2, X2);
                    double v = Math.Abs(h_v_UTM2[1] - h_v_UTM1[1]);
                    double h = Math.Abs(h_v_UTM2[0] - h_v_UTM1[0]);

                    canvas.Width = s_viewer.ActualWidth>0? (s_viewer.ActualWidth * 2):800;

                    double coef = (canvas.Width * (v / h)) / canvas.Height;

                    coefficient_X1 = Math.Abs(X2 - X1) / canvas.Width;
                    canvas.Height = canvas.Width*(v/h);
                    coefficient_Y1 = v / canvas.Height;

                    Objects_Links.M_Window.Map.Width = canvas.Width;
                    Objects_Links.M_Window.Map.Height = canvas.Height;

                    foreach (var item in canvas.Children)
                    {
                        if (item is Polyline && item != null)
                        {
                            Polyline line = (Polyline)item;
                            line.Height = line.Height * coef;
                            line.Width = line.Width * coef;
                            for (int i = 0; i < line.Points.Count; i++)
                            {
                                line.Points[i] = new Point(line.Points[i].X * coef, line.Points[i].Y * coef);
                            }
                        }

                    }



                    return true;
                }
                catch (Exception e)
                {
                    Status_List.push("set_long_lat() ERROR " + e.Message + " " + DateTimeOffset.Now.ToString());
                    return false;
                }
                
            }
            /// <summary>
            /// Обновляет подписи на координатных осях (h_lab_can, v_lab_can) соответственно изменениям сетки
            /// </summary>
            public void update_labels()
            {
                coefficient_X1 = Math.Abs(X2 - X1) / canvas.Width;
                coefficient_Y1 = Math.Abs(Y2 - Y1) / canvas.Height;
                int i = 0;
                foreach (Label item in h_lab_can.Children)
                {
                    Point p = PixToGeo(grid.Margin.Left + (i * current_pix_step), grid.Margin.Top);
                    item.Content = Math.Round(p.X, 4);
                    i++;
                }
                i = 0;
                foreach (Label item in v_lab_can.Children)
                {
                    Point p = PixToGeo((grid.Margin.Left), (grid.Margin.Top + ((i) * current_pix_step)));
                    item.Content = Math.Round(p.Y, 4);
                    //lab_width = item.ActualWidth;
                    i++;
                }

                
                double[] h_v_UTM1 = GIS.GEO_to_UTM(Y1, X1);
                double[] h_v_UTM2 = GIS.GEO_to_UTM(Y1+ (current_pix_step * ((Y2 - Y1) / canvas.Height)), X1+(current_pix_step * (X2 - X1) / canvas.Width));
                double vv = Math.Abs(h_v_UTM2[1] - h_v_UTM1[1]);
                double hh = Math.Abs(h_v_UTM2[0] - h_v_UTM1[0]);

                //h_lab.Content = Math.Round(Math.Abs(h_v_UTM1[1] - h_v_UTM2[1]), 0).ToString() + "m";
                //v_lab.Content = Math.Round(Math.Abs(h_v_UTM1[0] - h_v_UTM2[0]), 0).ToString() + "m";
                h_lab.Content = Math.Round(l_mesure(new Point(0, 0), new Point(current_pix_step, 0)), 0) + "m";
                v_lab.Content = Math.Round(l_mesure(new Point(0, 0), new Point(0, current_pix_step)),0) + "m";
            }
            public double get_m_to_pix_coef()
            {
                return current_pix_step / (l_mesure(new Point(0, 0), new Point(current_pix_step, 0)));
            }   
            public void mesh()
            {

                //grid_v_label.Width = s_viewer.ActualHeight;

                set_long_lat();

                grid.Children.Clear();
                h_lab_can.Children.Clear();
                v_lab_can.Children.Clear();

                grid.HorizontalAlignment = HorizontalAlignment.Left;
                grid.VerticalAlignment = VerticalAlignment.Top;

                grid.Width = s_viewer.ActualWidth * grid_size;
                grid.Height = s_viewer.ActualHeight * grid_size;

                h_lab_can.Width = grid.Width;
                v_lab_can.Width = grid.Height;


                for (int i = 0; i < grid.Width; i += pix_step)
                {
                    Line line = new Line()
                    {
                        Name = "V",
                        Stroke = Brushes.Black,
                        X1 = i,
                        X2 = i,
                        Y1 = 0,
                        Y2 = s_viewer.ActualHeight * grid_size
                    };
                    h_lab_can.Children.Add(new Label()
                    {
                        Margin = new Thickness(i,0,0,0)
                    });
                    grid.Children.Add(line);
                }
                for (int i = 0; i < grid.Height; i += pix_step)
                {
                    Line line = new Line()
                    {
                        Name = "H",
                        Stroke = Brushes.Black,
                        Y1 = i,
                        Y2 = i,
                        X1 = 0,
                        X2 = s_viewer.ActualWidth * grid_size
                    };
                    v_lab_can.Children.Add(new Label()
                    {
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Content = i.ToString(),
                        Margin = new Thickness(0,0,i,0)
                    });
                    grid.Children.Add(line);
                }
                current_pix_step = pix_step;
                grid_old_width = grid.Width;
                update_labels();
            }
            public void zoom(double coef)
            {
                if (grid.Width >= grid_old_width * devision_coef)
                {

                    grid.Width /= devision_coef;
                    grid.Height /= devision_coef;
                    current_pix_step /= devision_coef;
                    foreach (Line item in grid.Children)
                    {
                        item.X1 /= devision_coef;
                        item.X2 /= devision_coef;
                        item.Y1 /= devision_coef;
                        item.Y2 /= devision_coef;
                    }

                    h_lab_can.Width /= devision_coef;
                    foreach (Label item in h_lab_can.Children)
                    {
                        item.Margin = new Thickness(item.Margin.Left/devision_coef,0,0,0);
                    }

                    v_lab_can.Width /= devision_coef;
                    foreach (Label item in v_lab_can.Children)
                    {
                        item.Margin = new Thickness(0, 0, item.Margin.Right / devision_coef, 0);
                    }

                    move_to_center();
                }
                 else if(grid.Width < grid_old_width)// / devision_coef)
                {
                    grid.Width *= devision_coef;
                    grid.Height *= devision_coef;
                    current_pix_step *= devision_coef;
                    foreach (Line item in grid.Children)
                    {
                        item.X1 *= devision_coef;
                        item.X2 *= devision_coef;
                        item.Y1 *= devision_coef;
                        item.Y2 *= devision_coef;
                    }

                    h_lab_can.Width *= devision_coef;
                    foreach (Label item in h_lab_can.Children)
                    {
                        item.Margin = new Thickness(item.Margin.Left * devision_coef, 0, 0, 0);
                    }

                    v_lab_can.Width *= devision_coef;
                    foreach (Label item in v_lab_can.Children)
                    {
                        item.Margin = new Thickness(0, 0, item.Margin.Right * devision_coef, 0);
                    }

                   move_to_center();
                }
                grid.Width *= coef;
                grid.Height *= coef;
                grid.Margin = new Thickness(grid.Margin.Left  * coef, grid.Margin.Top * coef, 0, 0);

                foreach (Line item in grid.Children)
                {
                    item.X1 *= coef;
                    item.X2 *= coef;
                    item.Y1 *= coef;
                    item.Y2 *= coef;
                }



                h_lab_can.Width = grid.Width;
                foreach (Label item in h_lab_can.Children)
                {
                    item.Margin = new Thickness(item.Margin.Left * coef, 0, 0, 0);
                }

                v_lab_can.Width = grid.Height;
                foreach (Label item in v_lab_can.Children)
                {
                    item.Margin = new Thickness(0, 0, item.Margin.Right * coef, 0);
                }
                current_pix_step *= coef;
                move_to_center();
                //test_grind_pos();
                sync();
            }
            /// <summary>
            /// Выполняет перемещение координатных осей (h_lab_can, v_lab_can) соответственно сетке
            /// </summary>
            void sync()
            {
                //Point h = grid.TranslatePoint(new Point(0, 0), grid_h_label);
                //h_lab_can.Margin = new Thickness(h.X, 0, 0, 0);
                //v_lab_can.Margin = new Thickness(0, 0, -v.X + grid_v_label.Width - ((Label)v_lab_can.Children[1]).ActualWidth, 0);

                h_lab_can.Margin = new Thickness(canvas.Margin.Left + grid.Margin.Left, 0, 0, 0);
                v_lab_can.Margin = new Thickness(0, 0, canvas.Margin.Top + grid.Margin.Top - ((Label)v_lab_can.Children[1]).ActualWidth, 0);
            }
            /// <summary>
            /// Перемещает сетку (grid) в центр окна просмотра (s_viewer)
            /// </summary>
            public void move_to_center()
            {
                double grid_h_center = grid.Margin.Left + (grid.Width / 2);
                double view_h_center = -canvas.Margin.Left + (s_viewer.ActualWidth / 2);
                double h_steps_number = Math.Truncate((view_h_center - grid_h_center) / current_pix_step);

                double grid_v_center = grid.Margin.Top + (grid.Height / 2);
                double view_v_center = -canvas.Margin.Top + (s_viewer.ActualHeight / 2);
                double v_steps_number = Math.Truncate((view_v_center - grid_v_center) / current_pix_step);

                grid.Margin = new Thickness(grid.Margin.Left + h_steps_number * current_pix_step,
    grid.Margin.Top + v_steps_number * current_pix_step, 0, 0);

                sync();
                update_labels();
            }
            void test_grind_pos()
            {
                double hor = h_lab_can.ActualWidth - Math.Abs(h_lab_can.Margin.Left) - s_viewer.ActualWidth;
                double ver = v_lab_can.ActualWidth - Math.Abs(v_lab_can.Margin.Right) - s_viewer.ActualHeight;

                if (((Math.Abs(h_lab_can.Margin.Left) < current_pix_step) || (hor < current_pix_step)) ||
                    ((Math.Abs(v_lab_can.Margin.Right) < current_pix_step) || (ver < current_pix_step)))
                {
                    t++;
                    //t_b.Text = t.ToString();
                    move_to_center();
                }
            }
            int t = 0;
            public void mouse_move(MouseEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    sync();
                    test_grind_pos();
                }
            }
        }

        int Mouse_state = new int();
        Point Base_Map = new Point();
        double VerticalOffset = new double();
        double HorizontalOffset = new double();
        void mouse_scroll(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && Mouse_state != 1)
            {
                Base_Map = e.GetPosition(ScrollViewer1);
                VerticalOffset = canvas.Margin.Top;
                HorizontalOffset = canvas.Margin.Left;
                Mouse_state = 1;
            }

            if (e.LeftButton == MouseButtonState.Pressed && Mouse_state == 1)
            {
                Point cur_Map = e.GetPosition(ScrollViewer1);
                double new_h_off = HorizontalOffset - (-cur_Map.X + Base_Map.X);
                double new_v_off = VerticalOffset - (-cur_Map.Y + Base_Map.Y);

                if (new_h_off>0) 
                {
                    Base_Map.X = cur_Map.X;
                    HorizontalOffset = 0;
                }
                else if(canvas.ActualWidth + new_h_off < ScrollViewer1.ActualWidth)
                {
                    Base_Map.X = cur_Map.X;
                    HorizontalOffset = ScrollViewer1.ActualWidth - canvas.ActualWidth;
                }
                   else canvas.Margin = new Thickness(new_h_off, canvas.Margin.Top, 0, 0);

                if (new_v_off > 0)
                {
                    Base_Map.Y = cur_Map.Y;
                    VerticalOffset = 0;
                }
                else if (canvas.ActualHeight + new_v_off < ScrollViewer1.ActualHeight)
                {
                    Base_Map.Y = cur_Map.Y;
                    VerticalOffset = ScrollViewer1.ActualHeight - canvas.ActualHeight;
                }
                else canvas.Margin = new Thickness(canvas.Margin.Left, new_v_off, 0, 0);


                //if (canvas.Margin.Left >= 0 && h > 0 ||
                //    ((canvas.Margin.Left - ScrollViewer1.ActualWidth) + canvas.ActualWidth <= 0 && h < 0) ||
                //    canvas.Margin.Top >= 0 && v > 0 ||
                //    ((canvas.Margin.Top - ScrollViewer1.ActualHeight) + canvas.ActualHeight <= 0 && v < 0))
                //{
                //    //Base_Map = cur_Map;

                //    //if (canvas.Margin.Left > 0) canvas.Margin = new Thickness(0, canvas.Margin.Top, 0, 0);

                //    //if ((canvas.Margin.Left - ScrollViewer1.ActualWidth) + canvas.ActualWidth < 0)
                //    //    canvas.Margin = new Thickness(ScrollViewer1.ActualWidth - canvas.ActualWidth, canvas.Margin.Top, 0, 0);
                //    //if (canvas.Margin.Top > 0) canvas.Margin = new Thickness(canvas.Margin.Left, 0, 0, 0);
                //    //if ((canvas.Margin.Top - ScrollViewer1.ActualHeight) + canvas.ActualHeight < 0)
                //    //    canvas.Margin = new Thickness(canvas.Margin.Left, ScrollViewer1.ActualHeight - canvas.ActualHeight, 0, 0);

                //    //VerticalOffset = canvas.Margin.Top;
                //    //HorizontalOffset = canvas.Margin.Left;
                //}
                //else
                //{
                //    canvas.Margin = new Thickness(HorizontalOffset + h, VerticalOffset + v, 0, 0);
                //}
            }
            if (e.LeftButton == MouseButtonState.Released && Mouse_state == 1)
            {
                Mouse_state = 0;
            }
        }
        void zoom(double coeficient)
        {
            if ((canvas.Height * coeficient > ScrollViewer1.ActualHeight & canvas.Width * coeficient > ScrollViewer1.ActualWidth) | coeficient>=1)
            {
                canvas.Height = canvas.Height * coeficient;
                Map.Height = canvas.Height;
                canvas.Width = canvas.Width * coeficient;
                Map.Width = canvas.Width;
                double h = (canvas.Margin.Left
                - ScrollViewer1.ActualWidth / 2) * coeficient + ScrollViewer1.ActualWidth / 2;
                double v = ((canvas.Margin.Top
                - ScrollViewer1.ActualHeight / 2) * coeficient + ScrollViewer1.ActualHeight / 2);
                if (h > 0) h = 0;
                if (v > 0) v = 0;
                    

                if (canvas.Width + h < ScrollViewer1.ActualWidth)
                    h = ScrollViewer1.ActualWidth - canvas.Width;
                if (canvas.Height + v < ScrollViewer1.ActualHeight)
                    v = ScrollViewer1.ActualHeight - canvas.Height;

                canvas.Margin = new Thickness(h, v, 0, 0);
                
                foreach (var item in canvas.Children)
                {
                    if (item is Polyline && item!=null)
                    {
                        Polyline line = (Polyline)item;
                        line.Height = line.Height * coeficient;
                        line.Width = line.Width * coeficient;
                        for (int i = 0; i < line.Points.Count; i++)
                        {
                            line.Points[i] = new Point(line.Points[i].X * coeficient, line.Points[i].Y * coeficient);
                        }
                    }
                }
                foreach (var sh in Objects_Links.M_Window.icons_for_zoom)
                {
                    sh.Margin = new Thickness((sh.Margin.Left + (sh.ActualWidth / 2)) * coeficient - (sh.ActualWidth / 2), (sh.Margin.Top + (sh.ActualHeight / 2)) * coeficient - (sh.ActualHeight / 2), 0, 0);
                }

                mesh.zoom(coeficient);
                mesh.update_mes(coeficient);
                tracks.update_tracks(coeficient);
            }
            
        }

        void scroll_to_center()
        {
            canvas.Margin = new Thickness((canvas.Width - ScrollViewer1.ActualWidth) / 2, (canvas.Height - ScrollViewer1.ActualHeight) / 2, 0, 0);
        }



    }
    public class MyTreeViewItem : TreeViewItem
    {
        object target;
        bool isHANS = false;
        public string myname;
        
        public ComboBox colorbox = new ComboBox();
        public ComboBox typebox = new ComboBox();
        public CheckBox checkvis = new CheckBox() {Content = "Скрыть" };
        public void update()
        {
            //if (isHANS)
            //{
            //    if(((Device_Date.HANS)target).beacons.Count != Items.Count)
            //    {
            //        foreach (var item in ((Device_Date.HANS)target).beacons)
            //        {
            //            int state = 0;
            //            foreach (MyTreeViewItem item1 in Items)
            //            {
            //                if (item1.myname == item.Value.get_name()) state = 1;
            //            }
            //            if (state == 0) Items.Add(new MyTreeViewItem(item.Value));
            //        }
            //    }
                
            //    foreach (MyTreeViewItem item in Items)
            //    {
            //        item.Header = item.target.ToString();
            //    }
            //}
            //else
                Header = target.ToString();
        }
        void setcolorbox(Brush br)
        {
            if (br != null)
            {
                colorbox.Items.Add(new Rectangle() { Width = 30, Height = 10, Fill = br });
                colorbox.SelectedIndex = 0;
            }
            colorbox.Items.Add(new Rectangle() { Width = 30, Height = 10, Fill = Brushes.Black });
            colorbox.Items.Add(new Rectangle() { Width = 30, Height = 10, Fill = Brushes.Blue });
            colorbox.Items.Add(new Rectangle() { Width = 30, Height = 10, Fill = Brushes.Brown });
            colorbox.Items.Add(new Rectangle() { Width = 30, Height = 10, Fill = Brushes.Chocolate });
            colorbox.Items.Add(new Rectangle() { Width = 30, Height = 10, Fill = Brushes.Coral });
            colorbox.Items.Add(new Rectangle() { Width = 30, Height = 10, Fill = Brushes.Green });
            colorbox.Items.Add(new Rectangle() { Width = 30, Height = 10, Fill = Brushes.Red });
            colorbox.Items.Add(new Rectangle() { Width = 30, Height = 10, Fill = Brushes.Pink });
        }
        public MyTreeViewItem(Device_Date.Ship ship)
        {
            target = ship;
            myname = ship.get_name();
            Header = ship.ToString();
            setcolorbox(Objects_Links.Tracks.get_stroke(myname));
            Items.Add(colorbox);
            Items.Add(checkvis);
            colorbox.SelectionChanged += Colorbox_SelectionChanged;
            checkvis.Click += Checkvis_Checked;
        }

        private void Checkvis_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)checkvis.IsChecked)
            {
                if (target is Device_Date.Ship) Objects_Links.M_Window.SVessel.Visibility = Visibility.Hidden;
                Objects_Links.Tracks.set_visibility(myname, Visibility.Hidden);
                //if (target is Device_Date.HANS.Beacon && ((Device_Date.HANS.Beacon)target).type == Device_Date.HANS.Type.ROV)
                //    Objects_Links.M_Window.ROV.Visibility = Visibility.Hidden;
                //if (target is Device_Date.HANS.Beacon && ((Device_Date.HANS.Beacon)target).type == Device_Date.HANS.Type.DP)
                //    Objects_Links.M_Window.DP.Visibility = Visibility.Hidden;
                //foreach (var item in Objects_Links.M_Window.icons_for_zoom)
                //{
                //    if (item is Label && ((Label)item).Content.ToString() == myname) item.Visibility = Visibility.Hidden;

                //}
            }
            else
            {
                if (target is Device_Date.Ship) Objects_Links.M_Window.SVessel.Visibility = Visibility.Visible;
                Objects_Links.Tracks.set_visibility(myname, Visibility.Visible);
                //if (target is Device_Date.HANS.Beacon && ((Device_Date.HANS.Beacon)target).type == Device_Date.HANS.Type.ROV)
                //    Objects_Links.M_Window.ROV.Visibility = Visibility.Visible;
                //if (target is Device_Date.HANS.Beacon && ((Device_Date.HANS.Beacon)target).type == Device_Date.HANS.Type.DP)
                //    Objects_Links.M_Window.DP.Visibility = Visibility.Visible;
                //foreach (var item in Objects_Links.M_Window.icons_for_zoom)
                //{
                //    if (item is Label && ((Label)item).Content.ToString() == myname) item.Visibility = Visibility.Visible;
                //}
            }
        }

        private void Colorbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var br = colorbox.SelectedItem as Rectangle;
            if (br != null) Objects_Links.Tracks.change_stroke(myname, br.Fill);

        }

        public MyTreeViewItem(Device_Date.DVL dvl)
        {
            myname = dvl.GetType().ToString();
        }
        public MyTreeViewItem(Device_Date.HANS hans)
        {
            target = hans;
            myname = hans.get_name();
            Header = "Acoustic system";
            isHANS = true;
            //foreach (var item in hans.beacons)
            //{
            //    Items.Add(new MyTreeViewItem(item.Value));
            //}

        }
        public MyTreeViewItem(Device_Date.HANS.Beacon beacon)
        {
            target = beacon;
            myname = beacon.get_name();
            Header = beacon.ToString();
            setcolorbox(Objects_Links.Tracks.get_stroke(myname));
            Items.Add(colorbox);
            Items.Add(checkvis);
            typebox.Items.Add(new Label() { Content = "ROV"});
            typebox.Items.Add(new Label() { Content = "DP" });
            typebox.Items.Add(new Label() { Content = "No" });
            typebox.SelectedIndex = 2;
            Items.Add(typebox);
            colorbox.SelectionChanged += Colorbox_SelectionChanged;
            //checkvis.Click += Checkvis_Checked;
            //typebox.SelectionChanged += Typebox_SelectionChanged;
        }

        private void Typebox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //if (((Label)typebox.SelectedItem).Content.ToString() == "ROV")
            //{ ((Device_Date.HANS.Beacon)target).type = Device_Date.HANS.Type.ROV; }
            //if (((Label)typebox.SelectedItem).Content.ToString() == "No")
            //{ ((Device_Date.HANS.Beacon)target).type = Device_Date.HANS.Type.No; }
            //if (((Label)typebox.SelectedItem).Content.ToString() == "DP")
            //{ ((Device_Date.HANS.Beacon)target).type = Device_Date.HANS.Type.DP; }
            //Checkvis_Checked(null, null);

        }
    }

}
