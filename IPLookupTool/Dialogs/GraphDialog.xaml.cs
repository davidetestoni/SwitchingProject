using LiveCharts;
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
using System.Windows.Shapes;

namespace IPLookupTool.Dialogs
{
    /// <summary>
    /// Logica di interazione per GraphDialog.xaml
    /// </summary>
    public partial class GraphDialog : Window
    {
        public GraphDialog(SeriesCollection series, string[] labels)
        {
            InitializeComponent();
            DataContext = this;

            SeriesCollection = series;
            Labels = labels;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
    }
}
