using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LifeDB.Resources.Code
{
    internal class IntelViewController
    {

        private static StackPanel dataPanel;
        private static StackPanel summaryPanel;

        public static void Init(StackPanel data, StackPanel summary)
        {
            dataPanel    = data;
            summaryPanel = summary;
        }

    }
}
