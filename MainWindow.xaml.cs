using LifeDB.Resources.Code;
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
using static LifeDB.Resources.Code.SqlDb;

namespace LifeDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {

            InitializeComponent();

            SqlDb.Connect();

            TableViewController.Init(this.myTable);

            //| id | item_name | item_quantity | item_category | added | expires | limit |
            
            
            
            SqlDb.Pump("id","1","item_name",null);
            //Pump(Command.add, "item_name", "Bob Bobson Statue");
           // Pump(Command.add, "item_name", "Ron Ronson Statue","item_category","statues");



            

        }

        private void MinWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaxWindow(object sender, RoutedEventArgs e)
        {

            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }


    /*
     
                  //columns get no '' values MUST BE SURROUNDED by 
            //String[] pairs  = new String[] {"id", "1", "item_name", "testName"};
            //String[] pairs1 = new String[] {"item_name", "testName2" };
            //String[] pairs2 = new String[] {"id", "3", "item_name", "testName3" };
            //String[] pairs3 = new String[] {"item_name", "testName" };
            //String[] pairs4 = new String[] {"id", "1", "item_name", "testName" };
                 
    //SqlDb.Pump();
            //SqlDb.Pump();
            //SqlDb.Pump();
            //SqlDb.Pump();

            // this.myTable.RowGroups.Add(new TableRowGroup().Rows.Add(new TableRow().));
     
     
     */
}
