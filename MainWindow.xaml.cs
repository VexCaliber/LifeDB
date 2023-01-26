using LifeDB.Resources.Code;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
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

            MessageHandler.init(USER_CONSOLE);

            SqlDb.Connect();

            TableViewController.Init(myTable);
            

            
            //| id | item_name | item_quantity | item_category | added | expires | limit |
            
            ///SqlDb.Pump("id", "1", "item_name", null);
            ///SqlDb.Pump("id", "2", "item_name", "sprocket", "item_quantity", "12");
            ///SqlDb.Pump("id", "3", "item_name", "rock", "item_quantity", "1");
            //SqlDb.Pump("item_name", "smokes", "item_quantity", "25");
            //SqlDb.Pump("item_name", "glorius resumes", "item_quantity", "9001");
            ///SqlDb.Pump("id", "4", "item_name", "rocket", "item_quantity", "69");
            // SqlDb.Pump("item_name", "epic resumes", "item_quantity", "69", "item_category", "paperwork", "added", new DateOnly(day:12, month:6, year: 2020).ToString());
            ///SqlDb.Pump("id", "5", "item_name", "name", "item_quantity", "1000", "item_category", "myCategory", "added", "2023/12/22", "expires", "2023/12/23", "`limit`", "5");
            ///SqlDb.Pump("id", "6", "item_name", "name2", "item_quantity", "1000", "item_category", "myCategory", "added", "2023//12//22", "expires", "", "`limit`", "77"); //OK CONFIRMED LIMIT DOES HAVE TO BE BACKTICKED OUT 
            ///SqlDb.Pump("id", "7", "item_name", "name2", "item_quantity", "1000", "item_category", "myCategory", "added", null, "expires", null, "`limit`", "77"); //NOTE: TRY DOUBLE WRAPPING INTS or REPLACING single quotes with double...in the call it should tell sqlite to run it as an int.


            TableViewController.Generate(); //needs a thread!
            

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
                String limitFix = ADD_LIMIT.Content.ToString();
                limitFix = limitFix.Insert(0, "`");
                limitFix = limitFix.Insert(limitFix.Length, "`");
                //:: Edit it have pump return a bool...use the bool the set the button pass/fail color
                Pump(Command.add, ADD_ID.Content.ToString(),        ADD_ID_VALUE.Text, 
                                  ADD_NAME.Content.ToString(),      ADD_NAME_VALUE.Text,
                                  ADD_QUANT.Content.ToString(),     ADD_QUANT_VALUE.Text.ToString(),
                                  ADD_CAT.Content.ToString(),       ADD_CAT_VALUE.Text.ToString(),
                                  ADD_ADDED.Content.ToString(),     ADD_ADDED_VALUE.Text.ToString(),
                                  ADD_EXPIRES.Content.ToString(),   ADD_EXPIRES_VALUE.Text.ToString(),
                                  limitFix,                         ADD_LIMIT_VALUE.Text);

            }
            catch(Exception ex)
            {
                ADD_SUBMIT.Background = System.Windows.Media.Brushes.Red;
                USER_CONSOLE.Clear();
                USER_CONSOLE.Text = ex.ToString();
            }

            ADD_SUBMIT.Background = System.Windows.Media.Brushes.LightGreen;
            //TableViewController.Update(true);//throwing null pointer!
            //need a call to update table!

        }
    }


}
