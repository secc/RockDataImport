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

namespace org.secc.Rock.DataImport
{
    /// <summary>
    /// Interaction logic for SelectImportsPage.xaml
    /// </summary>
    public partial class SelectImportsPage : Page
    {
        private Dictionary<string,string> ConnectionSettings { get; set; }

        private SelectImportsPage()
        {
            InitializeComponent();
        }

        public SelectImportsPage( Dictionary<string, string> connectionSettings )
        {
            ConnectionSettings = connectionSettings;
        }
    }
}
