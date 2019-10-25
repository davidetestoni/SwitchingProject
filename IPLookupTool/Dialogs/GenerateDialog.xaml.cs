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
using IPLookup;
using IPLookup.Nodes;

namespace IPLookupTool.Dialogs
{
    /// <summary>
    /// Logica di interazione per GenerateDialog.xaml
    /// </summary>
    public partial class GenerateDialog : Window
    {
        public LoadTarget Target { get; set; } = LoadTarget.Database;

        public GenerateDialog()
        {
            InitializeComponent();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (minMask.Value > maxMask.Value)
            {
                MessageBox.Show("Max Mask is higher than Min Mask!");
            }

            if (minOctet1.Value > maxOctet1.Value)
            {
                MessageBox.Show("Max Octet1 is higher than Min Octet1!");
            }

            if (minOctet2.Value > maxOctet2.Value)
            {
                MessageBox.Show("Max Octet2 is higher than Min Octet2!");
            }

            if (minOctet3.Value > maxOctet3.Value)
            {
                MessageBox.Show("Max Octet3 is higher than Min Octet3!");
            }

            if (minOctet4.Value > maxOctet4.Value)
            {
                MessageBox.Show("Max Octet4 is higher than Min Octet4!");
            }

            var ips = new AddressGenerator()
                .GenerateMaskedIP(
                    (int)minOctet1.Value, (int)maxOctet1.Value,
                    (int)minOctet2.Value, (int)maxOctet2.Value,
                    (int)minOctet3.Value, (int)maxOctet3.Value,
                    (int)minOctet4.Value, (int)maxOctet4.Value,
                    (int)minMask.Value, (int)maxMask.Value)
                .Take((int)amount.Value)
                .Select(i => new Address(i));

            if (Target == LoadTarget.Database)
            {
                Globals.CreateDatabase(ips);
            }
            else
            {
                Globals.SetToSearch(ips);
            }

            Hide();
        }

        private void Amount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AcceptButton_Click(this, e);
            }
        }
    }
}
