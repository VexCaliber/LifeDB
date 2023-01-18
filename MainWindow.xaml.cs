using LifeDB.Resources.Code;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading;
using System.Windows;
using System.Xml.XPath;
using static LifeDB.Resources.Code.SqlDb;

namespace LifeDB
{
   

    public partial class MainWindow : Window
    {

        public MainWindow()
        {

            InitializeComponent();

            SqlDb.Connect();

            //Thread.Sleep(5000);
            TableViewController.Init(myTable);

            //| id | item_name | item_quantity | item_category | added | expires | limit |
            
            
            
            SqlDb.Pump("id", "1", "item_name", null);
            SqlDb.Pump("id", "2", "item_name", "sprocket", "item_quantity", "12");
            SqlDb.Pump("id", "3", "item_name", "rock", "item_quantity", "1");
            //SqlDb.Pump("item_name", "smokes", "item_quantity", "25");
            //SqlDb.Pump("item_name", "glorius resumes", "item_quantity", "9001");
            SqlDb.Pump("id", "4", "item_name", "rocket", "item_quantity", "69");

            //var x = SqlDb.SelectAll();

            TableViewController.Generate();
            
           // while (true)//x.Read()
          //  {
                //int myint = x.GetInt32(0);
                //string myreader = x.GetString(1);
           //     USER_CONSOLE.Text += x.GetSchemaTable().Columns; //x.GetName(0) + myint + " " + x.GetName(1) + myreader;

           //     break;

           // }


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
