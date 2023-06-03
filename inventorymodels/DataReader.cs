using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace InventoryModels
{
    public class DataReader
    {
        public int OrderUpTo { get; set; }
        public int ReviewPeriod { get; set; }
        public int StartInventoryQuantity { get; set; }
        public int StartLeadDays { get; set; }
        public int StartOrderQuantity { get; set; }
        public int NumberOfDays { get; set; }
        public List<Distribution> DemandDistribution { get; set; }
        public List<Distribution> LeadDaysDistribution { get; set; }

        public DataReader()
        {
            DemandDistribution = new List<Distribution>();
            LeadDaysDistribution = new List<Distribution>();
        }

        public void Read(int i)
        {
            string txt = File.ReadAllText($"../../../InventorySimulation/TestCases/TestCase{i}.txt");
            string[] parts = Regex.Split(txt, @"\r\n\r\n");

            OrderUpTo = Convert.ToInt32(Iterator(parts[0]));
            ReviewPeriod = Convert.ToInt32(Iterator(parts[1]));
            StartInventoryQuantity = Convert.ToInt32(Iterator(parts[2]));
            StartLeadDays = Convert.ToInt32(Iterator(parts[3]));
            StartOrderQuantity = Convert.ToInt32(Iterator(parts[4]));
            NumberOfDays = Convert.ToInt32(Iterator(parts[5]));

            SetDemandDistributions(parts[6]);
            SetLeadDaysDistributions(parts[7]);
        }

        private string Iterator(string input)
        {
            string tmp = "";
            foreach (var c in input)
            {
                if (char.IsLetter(c) || c == '\n' || c == ' ') continue;
                else tmp += c;
            }
            return tmp;
        }

        private void SetDemandDistributions(string input)
        {
            string[] rows = Regex.Split(input, @"\r\n");
            for (int i = 1; i < rows.Length; i++)
            {
                string[] tmp = Regex.Split(rows[i], @", ");
                DemandDistribution.Add(new Distribution
                {
                    Value = Convert.ToInt32(tmp[0]),
                    Probability = Convert.ToDecimal(tmp[1]),
                });
            }
        }

        private void SetLeadDaysDistributions(string input)
        {
            string[] rows = Regex.Split(input, @"\r\n");
            for (int i = 1; i < rows.Length; i++)
            {
                string[] tmp = Regex.Split(rows[i], @", ");
                LeadDaysDistribution.Add(new Distribution
                {
                    Value = Convert.ToInt32(tmp[0]),
                    Probability = Convert.ToDecimal(tmp[1]),
                });
            }
        }
    }
}
