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

namespace LifeDB
{
    /// <summary>
    /// Interaction logic for IntelFlowable.xaml
    /// </summary>
    public partial class Intel : Page
    {
        public Intel()
        {
            InitializeComponent();
            IntelViewController.Init(SNIPPETPANEL, SUMMARYPANEL);
            IntelViewController.RunStats();
        }
    }
}
