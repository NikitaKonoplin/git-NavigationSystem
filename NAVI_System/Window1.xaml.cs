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
    public partial class Set_Window : Window
    {
        MainWindow Mwin;
        public Set_Window(object ob)
        {
            InitializeComponent();
            Mwin=((MainWindow)ob);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Mwin.label.Content = "Hello from win1";
        }
    }
}
