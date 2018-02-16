using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NAVI_System
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Set_window : Window
    {
        public bool to_close = false;

        MainWindow Mwin;
        public Set_window(object ob)
        {
            InitializeComponent();
            Mwin=((MainWindow)ob);
            updateDList();

        }
       // Grid gr = new Grid();

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //listBox.ItemTemplate = new DataTemplate(g1);
            My_Grid gr = new My_Grid();
            //gr.Children.Add(new Button());
            listBox.Items.Add(gr);
        }
        private void updateDList()
        {
            listBox.Items.Clear();
            Objects_Links.S_Date.init_set_date();
            List<COM_Date> lis = Objects_Links.S_Date.get_COM_list();
            foreach (COM_Date item in lis)
            {
                My_Grid gr = new My_Grid(item);
                listBox.Items.Add(gr);
            }
        }
        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            List<My_Grid> obj = new List<My_Grid>();
            foreach (My_Grid item in listBox.Items)
            {
                if ((bool)item.check.IsChecked) obj.Add(item);
            }
            foreach (My_Grid item in obj)
            {
                listBox.Items.Remove(item);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            List<COM_Date> list = new List<COM_Date>();
            foreach (My_Grid item in listBox.Items)
            {
                if ((bool)item.check.IsChecked)
                {
                    COM_Date com = new COM_Date()
                    {
                        DeviceName = item.combo_Device.SelectedItem.ToString(),
                        BaudRate = double.Parse(item.combo_BaudRate.SelectedItem.ToString(), System.Globalization.CultureInfo.InvariantCulture),
                        PortName = item.combo_PortName.SelectedItem?.ToString(),
                        DataBits = double.Parse(item.combo_DataBits.SelectedItem.ToString(), System.Globalization.CultureInfo.InvariantCulture)
                    };
                    list.Add(com);
                }
            }
            Objects_Links.S_Date.manual_set(list);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!to_close)
            {
                e.Cancel = true;
                this.Visibility = Visibility.Hidden;
            }
            
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            updateDList();

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // listBox.Items.Clear();
            Objects_Links.S_Date.init_set_date();
        }
    }
    public class My_Grid : Grid
    {
        public CheckBox check = new CheckBox();
        public ComboBox combo_Device = new ComboBox();
        public ComboBox combo_PortName = new ComboBox();
        public ComboBox combo_BaudRate = new ComboBox();
        public ComboBox combo_DataBits = new ComboBox();
        CheckBox set_Check()
        {
            //check.Height = 25;
            //check.Width = 20;
            check.HorizontalAlignment = HorizontalAlignment.Left;
            check.Margin = new Thickness(2,5,0,0);
            return check;
        }
        CheckBox set_Check(bool state)
        {
            //check.Height = 25;
            //check.Width = 20;
            check.HorizontalAlignment = HorizontalAlignment.Left;
            check.Margin = new Thickness(2, 5, 0, 0);
            check.IsChecked = state;
            return check;
        }

        public ComboBox set_Divice()
        {
            combo_Device.SelectedIndex = 0;
            foreach (string item in COM_Factory.device_types)
            {
                combo_Device.Items.Add(item);
            }
            combo_Device.Height = 25;
            combo_Device.Width = 100;
            combo_Device.HorizontalAlignment = HorizontalAlignment.Left;
            combo_Device.Margin = new Thickness(20, 0, 0, 0);
            return combo_Device;
        }
        public ComboBox set_Divice(string type)
        {
            combo_Device.SelectedIndex = 0;
            foreach (string item in COM_Factory.device_types)
            {
                combo_Device.Items.Add(item);
            }
            if (combo_Device.Items.Contains(type))
            {
                combo_Device.Items.IndexOf(type);
                combo_Device.SelectedIndex = combo_Device.Items.IndexOf(type);
            }
            combo_Device.Height = 25;
            combo_Device.Width = 100;
            combo_Device.HorizontalAlignment = HorizontalAlignment.Left;
            combo_Device.Margin = new Thickness(20, 0, 0, 0);
            return combo_Device;
        }

        ComboBox set_PortName()
        {
            combo_PortName.Height = 25;
            combo_PortName.Width = 100;
            combo_PortName.HorizontalAlignment = HorizontalAlignment.Left;
            combo_PortName.Margin = new Thickness(20 + combo_Device.Width +5, 0, 0, 0);
            string[] st = System.IO.Ports.SerialPort.GetPortNames();
            foreach (string item in st)
            {
                combo_PortName.Items.Add(item);
            }
            return combo_PortName;
        }
        ComboBox set_PortName(string name)
        {
            combo_PortName.Height = 25;
            combo_PortName.Width = 100;
            combo_PortName.HorizontalAlignment = HorizontalAlignment.Left;
            combo_PortName.Margin = new Thickness(20 + combo_Device.Width + 5, 0, 0, 0);
            string[] st = System.IO.Ports.SerialPort.GetPortNames();
            foreach (string item in st)
            {
                combo_PortName.Items.Add(item);
            }
            if (combo_Device.Items.Contains(name))
            {
                combo_Device.Items.IndexOf(name);
                combo_Device.SelectedIndex = combo_Device.Items.IndexOf(name);
            }
            return combo_PortName;
        }

        ComboBox set_BaudRate()
        {
            combo_BaudRate.SelectedIndex = 0;
            combo_BaudRate.Items.Add("9600");
            combo_BaudRate.Items.Add("4800");
            combo_BaudRate.Height = 25;
            combo_BaudRate.Width = 100;
            combo_BaudRate.HorizontalAlignment = HorizontalAlignment.Left;
            combo_BaudRate.Margin = new Thickness(20 + combo_Device.Width+ combo_PortName.Width + 5 + 5, 0, 0, 0);
            return combo_BaudRate;
        }

        ComboBox set_BaudRate(double br)
        {
            combo_BaudRate.SelectedIndex = 0;
            combo_BaudRate.Items.Add(br.ToString());
            //combo_BaudRate.Items.Add("4800");
            combo_BaudRate.Height = 25;
            combo_BaudRate.Width = 100;
            combo_BaudRate.HorizontalAlignment = HorizontalAlignment.Left;
            combo_BaudRate.Margin = new Thickness(20 + combo_Device.Width + combo_PortName.Width + 5 + 5, 0, 0, 0);
            return combo_BaudRate;
        }

        ComboBox set_DataBits()
        {
            combo_DataBits.SelectedIndex = 0;
            combo_DataBits.Items.Add("8");
            //combo_DataBits.Items.Add("16");
            combo_DataBits.Height = 25;
            combo_DataBits.Width = 50;
            combo_DataBits.HorizontalAlignment = HorizontalAlignment.Left;
            combo_DataBits.Margin = new Thickness(20 + combo_Device.Width + combo_PortName.Width+ combo_BaudRate.Width + 5 + 5 + 5, 0, 0, 0);
            return combo_DataBits;
        }

        ComboBox set_DataBits(double db)
        {
            combo_DataBits.SelectedIndex = 0;
            combo_DataBits.Items.Add(db.ToString());
            //combo_DataBits.Items.Add("16");
            combo_DataBits.Height = 25;
            combo_DataBits.Width = 50;
            combo_DataBits.HorizontalAlignment = HorizontalAlignment.Left;
            combo_DataBits.Margin = new Thickness(20 + combo_Device.Width + combo_PortName.Width + combo_BaudRate.Width + 5 + 5 + 5, 0, 0, 0);
            return combo_DataBits;
        }

        public My_Grid()
        {
            this.Children.Add(set_Check());
            this.Children.Add(set_Divice());
            this.Children.Add(set_PortName());
            this.Children.Add(set_BaudRate());
            this.Children.Add(set_DataBits());
            this.Height = 25;
            this.Width = combo_DataBits.Margin.Left+ combo_DataBits.Width+5;
            this.Background = Brushes.DarkGray;
            //Children.Add
        }
        public My_Grid(COM_Date com)
        {
            this.Children.Add(set_Check(true));
            this.Children.Add(set_Divice(com.DeviceName));
            this.Children.Add(set_PortName(com.PortName));
            this.Children.Add(set_BaudRate(com.BaudRate));
            this.Children.Add(set_DataBits(com.DataBits));
            this.Height = 25;
            this.Width = combo_DataBits.Margin.Left + combo_DataBits.Width + 5;
            this.Background = Brushes.DarkGray;
            //Children.Add
        }
    }
}
