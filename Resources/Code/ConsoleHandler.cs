using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LifeDB.Resources.Code
{
    public static class ConsoleHandler
    {

        private static TextBox userConsole;

        public static void init(TextBox tx) 
        {
            userConsole = tx;
        }

        public static String GetText()
        {
            return userConsole.Text;
        }

        public static void SetText(String text) 
        { 
            userConsole.Text = text;
        }

        public static void Append(String text)
        {
            userConsole.Text += text; 
        }

        public static void Clear() 
        { 
            userConsole.Clear(); 
        }

        //add a method to parse command if a user inputs command (prepended with #/$) to parse.
        //depending on the prepend, context will switch.
        //SQL = $ and Program = #
        //Program state modification will require an overhaul, but will be achieved at some point
    }

}
