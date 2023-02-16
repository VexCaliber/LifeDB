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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LifeDB
{
    /// <summary>
    /// Interaction logic for Intel.xaml
    /// </summary>
    public partial class Intel : Page
    {
        public Intel()
        {
            InitializeComponent();
        }

        //-------------------------------------//

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            
            if (e.ChangedButton == MouseButton.Left)
            {
                MainWindow.GetWindow(this).DragMove();
                e.Handled = true;
            }

        }

        private void MinWindow(object sender, RoutedEventArgs e)
        {
            
            MainWindow.GetWindow(this).WindowState = WindowState.Minimized;

        }

        private void MaxWindow(object sender, RoutedEventArgs e)
        {

            if (MainWindow.GetWindow(this).WindowState == WindowState.Maximized)
            {
                MainWindow.GetWindow(this).WindowState = WindowState.Normal;
            }
            else
            {
                MainWindow.GetWindow(this).WindowState = WindowState.Maximized;
            }

        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {

            MainWindow.GetWindow(this).Close(); 

        }

        private void MoveWindow(object sender, RoutedEventArgs e)
        {

        }

        private void FixedPage_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }
}
