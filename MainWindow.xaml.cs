using LifeDB.Resources.Code;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Input;
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
            SqlDb.Pump("item_name", "epic resumes", "item_quantity", "69", "item_category", "paperwork", "added", new DateOnly(day:12, month:6, year: 2020).ToString());

            //var x = SqlDb.SelectAll();

            TableViewController.Generate(); //needs a thread!
            
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

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            //if (Mouse.LeftButton == MouseButtonState.Pressed) //e.ChangedButton == MouseButton.Left
            //    this.DragMove();
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
                e.Handled = true;
            }
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

        private void MoveWindow(object sender, RoutedEventArgs e)
        {

        }

        private void ADD_SUBMIT_EXECUTE(object sender, RoutedEventArgs e)
        {
            try
            {
                //must add default space? :: Edit it have pump return a bool...use the bool the set the button pass/fail color
                Pump(Command.add, ADD_ID.Content.ToString(), ADD_ID_VALUE.Text.ToString(), 
                                  ADD_NAME.Content.ToString(), ADD_NAME_VALUE.Text.ToString(),
                                  ADD_QUANT.Content.ToString(), ADD_QUANT_VALUE.Text.ToString(),
                                  ADD_CAT.Content.ToString(), ADD_CAT_VALUE.Text.ToString(),
                                  ADD_ADDED.Content.ToString(), ADD_ADDED_VALUE.Text.ToString(),
                                  ADD_EXPIRES.Content.ToString(), ADD_EXPIRES_VALUE.Text.ToString(),
                                  ADD_LIMIT.Content.ToString(), ADD_LIMIT_VALUE.Text.ToString());

            }
            catch(Exception ex)
            {
                ADD_SUBMIT.Background = System.Windows.Media.Brushes.Red;
                USER_CONSOLE.Clear();
                USER_CONSOLE.Text = ex.ToString();
            }

            ADD_SUBMIT.Background = System.Windows.Media.Brushes.LightGreen;
            //need a call to update table!

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
