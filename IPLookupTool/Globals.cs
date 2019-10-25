using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPLookup;
using IPLookup.Nodes;
using IPLookupTool.Dialogs;

namespace IPLookupTool
{
    public enum LoadTarget
    {
        Database,
        ToSearch
    }

    public static class Globals
    {
        public static int MultibitStride { get; set; } = 3;

        // Tries
        public static BinaryNode BinaryRoot { get; set; } = new BinaryNode();
        public static CompressedNode CompressedRoot { get; set; } = new CompressedNode();
        public static MultibitNode MultibitRoot { get; set; } = new MultibitNode();

        public static MainWindow Main { get; set; }
        public static GenerateDialog Generate { get; set; }

        // Addresses to search
        public static Address[] ToSearch = new Address[] { };

        public static void CreateDatabase(IEnumerable<Address> ips)
        {
            BinaryRoot = new BinaryNode() { NextHop = "Root" };
            CompressedRoot = new CompressedNode() { NextHop = "Root" };
            MultibitRoot = new MultibitNode() { Stride = MultibitStride };

            foreach (var ip in ips)
            {
                BinaryRoot.AddChild(ip.MaskedIPv4, ip.BinaryString);
                CompressedRoot.AddChild(ip.MaskedIPv4, ip.BinaryString);
                MultibitRoot.AddChild(ip.MaskedIPv4, ip.BinaryString);
            }

            CompressedRoot.Compress();

            Main.UpdateTreeViews();
        }

        public static void SetToSearch(IEnumerable<Address> ips)
        {
            ToSearch = ips.ToArray();

            Main.UpdateToSearch();
        }

        public static void LinearRegression(
            double[] xVals,
            double[] yVals,
            out double rSquared,
            out double yIntercept,
            out double slope)
        {
            if (xVals.Length != yVals.Length)
            {
                throw new Exception("Input values should be with the same length.");
            }

            double sumOfX = 0;
            double sumOfY = 0;
            double sumOfXSq = 0;
            double sumOfYSq = 0;
            double sumCodeviates = 0;

            for (var i = 0; i < xVals.Length; i++)
            {
                var x = xVals[i];
                var y = yVals[i];
                sumCodeviates += x * y;
                sumOfX += x;
                sumOfY += y;
                sumOfXSq += x * x;
                sumOfYSq += y * y;
            }

            var count = xVals.Length;
            var ssX = sumOfXSq - ((sumOfX * sumOfX) / count);
            var ssY = sumOfYSq - ((sumOfY * sumOfY) / count);

            var rNumerator = (count * sumCodeviates) - (sumOfX * sumOfY);
            var rDenom = (count * sumOfXSq - (sumOfX * sumOfX)) * (count * sumOfYSq - (sumOfY * sumOfY));
            var sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

            var meanX = sumOfX / count;
            var meanY = sumOfY / count;
            var dblR = rNumerator / Math.Sqrt(rDenom);

            rSquared = dblR * dblR;
            yIntercept = meanY - ((sCo / ssX) * meanX);
            slope = sCo / ssX;
        }
    }
}
