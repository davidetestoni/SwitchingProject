using IPLookup;
using IPLookup.Nodes;
using IPLookupTool.Dialogs;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace IPLookupTool
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Success
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Globals.Main = this;
            Globals.Generate = new GenerateDialog();

            LogLine("Initialized"); 
        }

        public void Log(string message = "", LogLevel level = LogLevel.Info)
        {
            switch (level)
            {
                default:
                case LogLevel.Info:
                    logBox.AppendText(message, Colors.Black);
                    break;

                case LogLevel.Warning:
                    logBox.AppendText(message, Colors.OrangeRed);
                    break;

                case LogLevel.Error:
                    logBox.AppendText(message, Colors.Red);
                    break;

                case LogLevel.Success:
                    logBox.AppendText(message, Colors.Green);
                    break;
            }

            logBox.ScrollToEnd();
        }

        public void LogLine(string message = "", LogLevel level = LogLevel.Info)
        {
            Log($"{message}{Environment.NewLine}", level);
        }

        public void UpdateTreeViews()
        {
            UpdateBinaryTreeView(Globals.BinaryRoot);
            UpdateCompressedTreeView(Globals.CompressedRoot);
            UpdateMultibitTreeView(Globals.MultibitRoot);
        }

        public void UpdateBinaryTreeView(BinaryNode root)
        {
            binaryTreeView.Items.Clear();
            
            TreeViewItem child = new TreeViewItem();
            child.Header = "Root";
            child.Foreground = new SolidColorBrush(Colors.OrangeRed);
            child.Tag = root;

            binaryTreeView.Items.Add(child);

            if (root.Left != null)
            {
                AddBinaryChild(root.Left, child, root.Left.NextHop != "" ? $"0 ({root.Left.NextHop})" : "0");
            }

            if (root.Right != null)
            {
                AddBinaryChild(root.Right, child, root.Right.NextHop != "" ? $"1 ({root.Right.NextHop})" : "1");
            }
        }

        public void AddBinaryChild(BinaryNode node, TreeViewItem parentItem, string name)
        {
            var child = new TreeViewItem();
            child.Header = name;
            child.Tag = node;
            if (node.NextHop != "") child.Foreground = new SolidColorBrush(Colors.DodgerBlue);

            if (node.Left != null)
            {
                AddBinaryChild(node.Left, child, node.Left.NextHop != "" ? $"0 ({node.Left.NextHop})" : "0");
            }

            if (node.Right != null)
            {
                AddBinaryChild(node.Right, child, node.Right.NextHop != "" ? $"1 ({node.Right.NextHop})" : "1");
            }

            parentItem.Items.Add(child);
        }

        public void UpdateCompressedTreeView(CompressedNode root)
        {
            compressedTreeView.Items.Clear();

            TreeViewItem child = new TreeViewItem();
            child.Header = "Root";
            child.Foreground = new SolidColorBrush(Colors.OrangeRed);
            child.Tag = root;

            compressedTreeView.Items.Add(child);

            if (root.Left != null)
            {
                AddCompressedChild(root.Left, child, root.Left.NextHop != "" ? $"0 ({root.Left.NextHop})" : "0");
            }

            if (root.Right != null)
            {
                AddCompressedChild(root.Right, child, root.Right.NextHop != "" ? $"1 ({root.Right.NextHop})" : "1");
            }
        }

        public void AddCompressedChild(CompressedNode node, TreeViewItem parentItem, string name)
        {
            var child = new TreeViewItem();
            child.Header = name;
            child.Tag = node;
            if (node.NextHop != "") child.Foreground = new SolidColorBrush(Colors.DodgerBlue);

            if (node.Left != null)
            {
                AddCompressedChild(node.Left, child, node.Left.NextHop != "" ? $"0 ({node.Left.NextHop})" : "0");
            }

            if (node.Right != null)
            {
                AddCompressedChild(node.Right, child, node.Right.NextHop != "" ? $"1 ({node.Right.NextHop})" : "1");
            }

            parentItem.Items.Add(child);
        }

        public void UpdateMultibitTreeView(MultibitNode root)
        {
            multibitTreeView.Items.Clear();

            TreeViewItem child = new TreeViewItem();
            child.Header = "Root";
            child.Foreground = new SolidColorBrush(Colors.OrangeRed);
            child.Tag = root;

            multibitTreeView.Items.Add(child);

            AddMultibitChild(Globals.MultibitRoot, child);
        }

        public void AddMultibitChild(MultibitNode node, TreeViewItem parentItem)
        {
            foreach (var childNode in node.Children)
            {
                var child = new TreeViewItem();
                if (childNode.Value.Item1 != "")
                {
                    child.Header = $"{childNode.Key} ({childNode.Value.Item1})";
                    child.Foreground = new SolidColorBrush(Colors.DodgerBlue);
                }
                else
                {
                    child.Header = childNode.Key;
                }

                child.Tag = childNode.Value.Item2;
                parentItem.Items.Add(child);

                if (childNode.Value.Item2 != null)
                {
                    AddMultibitChild(childNode.Value.Item2, child);
                }
            }
        }

        private void DatabaseLoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "IP List | *.txt";
            ofd.FilterIndex = 1;
            ofd.ShowDialog();

            if (string.IsNullOrEmpty(ofd.FileName)) return;

            Globals.CreateDatabase(File.ReadAllLines(ofd.FileName)
                .Where(l => !string.IsNullOrEmpty(l) && Regex.Match(l, Utils.MaskedIPRegex).Success)
                .Select(l => new Address(l)));
        }

        private void DatabaseGenerateButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.Generate.Target = LoadTarget.Database;
            Globals.Generate.ShowDialog();
        }

        private void TosearchLoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "IP List | *.txt";
            ofd.FilterIndex = 1;
            ofd.ShowDialog();

            if (string.IsNullOrEmpty(ofd.FileName)) return;

            Globals.SetToSearch(File.ReadAllLines(ofd.FileName)
                .Where(l => !string.IsNullOrEmpty(l) && Regex.Match(l, Utils.MaskedIPRegex).Success)
                .Select(l => new Address(l))
                .ToArray());
        }

        private void TosearchGenerateButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.Generate.Target = LoadTarget.ToSearch;
            Globals.Generate.ShowDialog();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Globals.Generate.Close();
            Environment.Exit(0);
        }

        private void ExpandBinaryTreeView_Click(object sender, RoutedEventArgs e)
        {
            foreach (TreeViewItem item in binaryTreeView.Items)
            {
                SetExpanded(item, !item.IsExpanded);
            }
        }

        private void ExpandCompressedTreeView_Click(object sender, RoutedEventArgs e)
        {
            foreach (TreeViewItem item in compressedTreeView.Items)
            {
                SetExpanded(item, !item.IsExpanded);
            }
        }

        private void ExpandMultibitTreeView_Click(object sender, RoutedEventArgs e)
        {
            foreach (TreeViewItem item in multibitTreeView.Items)
            {
                SetExpanded(item, !item.IsExpanded);
            }
        }

        private void SetExpanded(TreeViewItem item, bool expanded)
        {
            item.IsExpanded = expanded;

            foreach (TreeViewItem child in item.Items)
            {
                SetExpanded(child, expanded);
            }
        }

        private void BinaryTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var node = (e.NewValue as TreeViewItem).Tag as BinaryNode;
            var hasPrefix = node.NextHop != "" ? "YES" : "NO";
            nodeInfoLabel.Content = $"Prefix: {hasPrefix}";
        }

        private void CompressedTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var node = (e.NewValue as TreeViewItem).Tag as CompressedNode;
            var hasPrefix = node.NextHop != "" ? "YES" : "NO";
            nodeInfoLabel.Content = $"Prefix: {hasPrefix} | Skip: {node.Skip} | Segment: {node.Segment}";
        }

        private void MultibitTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var node = (e.NewValue as TreeViewItem).Tag as MultibitNode;
            nodeInfoLabel.Content = $"";
        }

        private void StrideValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Globals.MultibitStride = (int)e.NewValue;
        }

        public void UpdateToSearch()
        {
            tosearchListView.Items.Clear();

            foreach (var address in Globals.ToSearch.OrderBy(a => a.Mask))
            {
                tosearchListView.Items.Add($"{address.MaskedIPv4} -> {address.BinaryString}");
            }
        }

        private void PerformanceTestButton_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch watch = new Stopwatch();
            var ips = Globals.ToSearch.Select(a => a.BinaryString);

            /*
             * RECURSIVE
             */

            // BINARY
            watch.Restart();
            foreach (var ip in ips)
            {
                Globals.BinaryRoot.Lookup(ip);
            }
            var binaryTime = watch.ElapsedMilliseconds;

            // COMPRESSED
            watch.Restart();
            foreach (var ip in ips)
            {
                Globals.CompressedRoot.Lookup(ip);
            }
            var compressedTime = watch.ElapsedMilliseconds;

            // MULTIBIT
            watch.Restart();
            foreach(var ip in ips)
            {
                Globals.MultibitRoot.Lookup(ip, "Root");
            }
            var multibitTime = watch.ElapsedMilliseconds;

            /*
             * NON RECURSIVE
             */

            // BINARY
            watch.Restart();
            foreach (var ip in ips)
            {
                Globals.BinaryRoot.LookupNonRecursive(ip);
            }
            var binaryNRTime = watch.ElapsedMilliseconds;

            // COMPRESSED
            watch.Restart();
            foreach (var ip in ips)
            {
                Globals.CompressedRoot.LookupNonRecursive(ip);
            }
            var compressedNRTime = watch.ElapsedMilliseconds;

            // MULTIBIT
            watch.Restart();
            foreach (var ip in ips)
            {
                Globals.MultibitRoot.LookupNonRecursive(ip, "Root");
            }
            var multibitNRTime = watch.ElapsedMilliseconds;

            var results = $@"
Test completed on {ips.Count()} IPs!

RECURSIVE:
Binary: {binaryTime} ms
Compressed: {compressedTime} ms
Multibit: {multibitTime} ms

NON RECURSIVE:
Binary: {binaryNRTime} ms
Compressed: {compressedNRTime} ms
Multibit: {multibitNRTime} ms
";

            LogLine(results, LogLevel.Success);
        }

        private void GraphicTestButton_Click(object sender, RoutedEventArgs e)
        {
            var samples = (int)samplesValue.Value;
            var ips = Globals.ToSearch.Select(i => i.BinaryString);
            var total = Globals.ToSearch.Count();

            int size = total / samples;

            List<long> binaryResults = new List<long>();
            List<long> compressedResults = new List<long>();
            List<long> multibitResults = new List<long>();

            List<long> binaryNRResults = new List<long>();
            List<long> compressedNRResults = new List<long>();
            List<long> multibitNRResults = new List<long>();

            for (int i = 1; i <= samples; i++)
            {
                ips = Globals.ToSearch.Take(i * size).Select(ip => ip.BinaryString).ToArray();
                var watch = new Stopwatch();

                /*
                * RECURSIVE
                */

                // BINARY
                watch.Restart();
                foreach (var ip in ips)
                {
                    Globals.BinaryRoot.Lookup(ip);
                }
                binaryResults.Add(watch.ElapsedMilliseconds);

                // COMPRESSED
                watch.Restart();
                foreach (var ip in ips)
                {
                    Globals.CompressedRoot.Lookup(ip);
                }
                compressedResults.Add(watch.ElapsedMilliseconds);

                // MULTIBIT
                watch.Restart();
                foreach (var ip in ips)
                {
                    Globals.MultibitRoot.Lookup(ip, "Root");
                }
                multibitResults.Add(watch.ElapsedMilliseconds);

                /*
                 * NON RECURSIVE
                 */

                // BINARY
                watch.Restart();
                foreach (var ip in ips)
                {
                    Globals.BinaryRoot.LookupNonRecursive(ip);
                }
                binaryNRResults.Add(watch.ElapsedMilliseconds);

                // COMPRESSED
                watch.Restart();
                foreach (var ip in ips)
                {
                    Globals.CompressedRoot.LookupNonRecursive(ip);
                }
                compressedNRResults.Add(watch.ElapsedMilliseconds);

                // MULTIBIT
                watch.Restart();
                foreach (var ip in ips)
                {
                    Globals.MultibitRoot.LookupNonRecursive(ip, "Root");
                }
                multibitNRResults.Add(watch.ElapsedMilliseconds);
            }

            var xAxis = Enumerable.Range(1, samples)
                .Select(x => x * size);

            var series = new SeriesCollection();

            // BINARY
            if ((bool)plotBinary.IsChecked)
            {
                series.Add(new LineSeries() { Title = "Binary", Values = new ChartValues<long>(binaryResults) });

                if ((bool)plotRegression.IsChecked)
                {
                    series.Add(new LineSeries() { Title = "Binary (Regression)", Values = new ChartValues<double>(GetRegression(xAxis, binaryResults)) });
                }

                if ((bool)plotNR.IsChecked)
                {
                    series.Add(new LineSeries() { Title = "Binary (NR)", Values = new ChartValues<long>(binaryNRResults) });

                    if ((bool)plotRegression.IsChecked)
                    {
                        series.Add(new LineSeries() { Title = "Binary (NR) (Regression)", Values = new ChartValues<double>(GetRegression(xAxis, binaryNRResults)) });
                    }
                }
            }
            
            // COMPRESSED
            if ((bool)plotCompressed.IsChecked)
            {
                series.Add(new LineSeries() { Title = "Compressed", Values = new ChartValues<long>(compressedResults) });

                if ((bool)plotRegression.IsChecked)
                {
                    series.Add(new LineSeries() { Title = "Compressed (Regression)", Values = new ChartValues<double>(GetRegression(xAxis, compressedResults)) });
                }

                if ((bool)plotNR.IsChecked)
                {
                    series.Add(new LineSeries() { Title = "Compressed (NR)", Values = new ChartValues<long>(compressedNRResults) });

                    if ((bool)plotRegression.IsChecked)
                    {
                        series.Add(new LineSeries() { Title = "Compressed (NR) (Regression)", Values = new ChartValues<double>(GetRegression(xAxis, compressedNRResults)) });
                    }
                }
            }

            // MULTIBIT
            if ((bool)plotMultibit.IsChecked)
            {
                series.Add(new LineSeries() { Title = "Multibit", Values = new ChartValues<long>(multibitResults) });

                if ((bool)plotRegression.IsChecked)
                {
                    series.Add(new LineSeries() { Title = "Multibit (Regression)", Values = new ChartValues<double>(GetRegression(xAxis, multibitResults)) });
                }

                if ((bool)plotNR.IsChecked)
                {
                    series.Add(new LineSeries() { Title = "Multibit (NR)", Values = new ChartValues<long>(multibitNRResults) });

                    if ((bool)plotRegression.IsChecked)
                    {
                        series.Add(new LineSeries() { Title = "Multibit (NR) (Regression)", Values = new ChartValues<double>(GetRegression(xAxis, multibitNRResults)) });
                    }
                }
            }

            var labels = xAxis
                .Select(x => x.ToString())
                .ToArray();

            // Write to console at the end otherwise it might impact the timing
            LogLine();
            LogLine("RECURSIVE:");
            LogLine($"Binary: {string.Join(",", binaryResults)} (Slope: {GetSlope(xAxis, binaryResults)})");
            LogLine($"Compressed: {string.Join(",", compressedResults)} (Slope: {GetSlope(xAxis, compressedResults)})");
            LogLine($"Multibit: {string.Join(",", multibitResults)} (Slope: {GetSlope(xAxis, multibitResults)})");

            LogLine("NON RECURSIVE:");
            LogLine($"Binary: {string.Join(",", binaryNRResults)} (Slope: {GetSlope(xAxis, binaryNRResults)})");
            LogLine($"Compressed: {string.Join(",", compressedNRResults)} (Slope: {GetSlope(xAxis, compressedNRResults)})");
            LogLine($"Multibit: {string.Join(",", multibitNRResults)} (Slope: {GetSlope(xAxis, multibitNRResults)})");

            (new GraphDialog(series, labels) { Title = $"Graph | Searched: {total} IPs | Samples: {samples}" }).Show();
        }

        private double[] GetRegression(IEnumerable<int> xAxis, IEnumerable<long> yAxis)
        {
            double rSquared, intercept, slope;

            Globals.LinearRegression(
                xAxis.Select(x => (double)x).ToArray(), 
                yAxis.Select(y => (double)y).ToArray(), 
                out rSquared, 
                out intercept, 
                out slope);

            List<double> interp = new List<double>();
            foreach (var x in xAxis)
            {
                var predictedValue = (slope * x) + intercept;
                interp.Add(predictedValue);
            }

            return interp.ToArray();
        }

        private double GetSlope(IEnumerable<int> xAxis, IEnumerable<long> yAxis)
        {
            double rSquared, intercept, slope;

            Globals.LinearRegression(
                xAxis.Select(x => (double)x).ToArray(),
                yAxis.Select(y => (double)y).ToArray(),
                out rSquared,
                out intercept,
                out slope);

            return slope;
        }
    }

    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            TextRange tr = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
            tr.Text = text;
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
        }
    }
}
