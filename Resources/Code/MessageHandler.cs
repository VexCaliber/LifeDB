using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LifeDB.Resources.Code
{
    public static class MessageHandler
    {

        public static TextBox userConsole { get; private set; }

        public static void init(TextBox tx) 
        {
            userConsole = tx;
        }

    }

}
