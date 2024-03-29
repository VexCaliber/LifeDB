﻿using LifeDB.Resources.Code;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Xml.XPath;
using static LifeDB.Resources.Code.SqlDb;

namespace LifeDB
{
   

    public partial class MainWindow : Window
    {

        public Page stock = new Stock();
        public Page intel = new Intel();

        public MainWindow()
        {
            
            InitializeComponent();
            MAIN.Content = new Intro();

            /*

            InitializeComponent();

            ConsoleHandler.init(USER_CONSOLE);

            SqlDb.Connect();

            TableViewController.Init(myTable);
            
            //| id | item_name | item_quantity | item_category | added | expires | limit |
    
            TableViewController.Generate();
            */

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

        protected void MinWindow(object sender, RoutedEventArgs e)
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

        private void ToIntel(object sender, RoutedEventArgs e)
        {

            MAIN.Content = intel;
            IntelViewController.Refresh();

        }

        private void ToStock(object sender, RoutedEventArgs e)
        {

            MAIN.Content = stock;

        }

        public Page getStock() { return stock; }


        //-----------------------------------------------------------------//

        /*
        private void ADD_SUBMIT_EXECUTE(object sender, RoutedEventArgs e)
        {
            try
            {
                String limitFix = ADD_LIMIT.Content.ToString();
                limitFix = limitFix.Insert(0, "`");
                limitFix = limitFix.Insert(limitFix.Length, "`");
                //:: Edit it have pump return a bool...use the bool the set the button pass/fail color
                Pump(Command.add, ADD_ID.Content.ToString(),        ADD_ID_VALUE.Text.ToString(), 
                                  ADD_NAME.Content.ToString(),      ADD_NAME_VALUE.Text.ToString(),
                                  ADD_QUANT.Content.ToString(),     ADD_QUANT_VALUE.Text.ToString(),
                                  ADD_CAT.Content.ToString(),       ADD_CAT_VALUE.Text.ToString(),
                                  ADD_ADDED.Content.ToString(),     ADD_ADDED_VALUE.Text.ToString(),
                                  ADD_EXPIRES.Content.ToString(),   ADD_EXPIRES_VALUE.Text.ToString(),
                                  limitFix,                         ADD_LIMIT_VALUE.Text.ToString());

            }
            catch(Exception ex)
            {
                ADD_SUBMIT.Background = System.Windows.Media.Brushes.Red;
                USER_CONSOLE.Clear();
                USER_CONSOLE.Text = ex.ToString();
            }

            ADD_SUBMIT.Background = System.Windows.Media.Brushes.LightGreen;
            TableViewController.Update(true);

        }

        private void EDIT_SUBMIT_EXECUTE(object sender, RoutedEventArgs e)
        {
            try
            {
                String limitFix = EDIT_LIMIT.Content.ToString();
                limitFix = limitFix.Insert(0, "`");
                limitFix = limitFix.Insert(limitFix.Length, "`");
                
                Pump(Command.edit, EDIT_ID.Content.ToString(),      EDIT_ID_VALUE.Text.ToString(),
                                   EDIT_NAME.Content.ToString(),    EDIT_NAME_VALUE.Text.ToString(),
                                   EDIT_QUANT.Content.ToString(),   EDIT_QUANT_VALUE.Text.ToString(),
                                   EDIT_CAT.Content.ToString(),     EDIT_CAT_VALUE.Text.ToString(),
                                   EDIT_ADDED.Content.ToString(),   EDIT_ADDED_VALUE.Text.ToString(),
                                   EDIT_EXPIRES.Content.ToString(), EDIT_EXPIRES_VALUE.Text.ToString(),
                                   limitFix,                        ADD_LIMIT_VALUE.Text.ToString());

            }
            catch (Exception ex)
            {
                EDIT_SUBMIT.Background = System.Windows.Media.Brushes.Red;
                USER_CONSOLE.Clear();
                USER_CONSOLE.Text = ex.ToString();
            }

            EDIT_SUBMIT.Background = System.Windows.Media.Brushes.LightGreen;
            TableViewController.Update(true);
            

        }

        private void REMOVE_SUBMIT_EXECUTE(object sender, RoutedEventArgs e)
        {
            try
            {
                String limitFix = REMOVE_SELECTION.Text.ToString();
                limitFix = limitFix.Insert(0, "`");
                limitFix = limitFix.Insert(limitFix.Length, "`");

                if(REMOVE_SELECTION.Text == "limit")
                    Pump(Command.remove, limitFix, REMOVE_INPUT.Text.ToString());
                else
                    Pump(Command.remove, REMOVE_SELECTION.Text.ToString(),REMOVE_INPUT.Text.ToString());


            }catch (Exception ex)
            {
                REMOVE_SUBMIT.Background = System.Windows.Media.Brushes.Red;
                USER_CONSOLE.Clear();
                USER_CONSOLE.Text = ex.ToString();
            }

            REMOVE_SUBMIT.Background = System.Windows.Media.Brushes.LightGreen;
            //TableViewController.Update(true);

        }

        private void ToIntel(object sender, RoutedEventArgs e)
        {

            //NavigationWindow window = new NavigationWindow();
            //window.Source = new Uri("\\Intel.xaml", UriKind.Relative);
            //window.Show();

            //NavigationService.GetNavigationService(this).NavigateTo("Intel.xaml");

            // Get a reference to the NavigationService that navigated to this Page
            //NavigationService.GetNavigationService(this).Source = new Uri("\\Intel.xaml", UriKind.Relative); //failed

            //NavigationService.GetNavigationService(this).Navigate(new Uri("Intel.xaml", UriKind.Relative));

            //Window.GetWindow(this).Content = new Uri("Intel.xaml", UriKind.Relative);

            //Page intel = new Page(new Uri("Intel.xaml", UriKind.Relative));

            //NavigationService.GetNavigationService(this).Source = new Uri("\\Intel.xaml", UriKind.Relative);

            //MainWindow m = new MainWindow();
            //m.Show();

            


            //NavigationWindow.GetWindow(this).Content = new Uri("\\Intel.xaml", UriKind.Relative).;

            //NavigationService.Navigate(new Uri("/OtherViewPage.xaml", UriKind.Relative));



        }
        */


    }


}
