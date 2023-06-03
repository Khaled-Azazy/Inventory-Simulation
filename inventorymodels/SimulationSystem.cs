using System;
using System.Collections.Generic;
using System.Linq;

namespace InventoryModels
{
    public class SimulationSystem
    {
        #region VARs
        private Random _random;
        ///////////// INPUTS /////////////
        public int OrderUpTo { get; set; }
        public int ReviewPeriod { get; set; }
        public int NumberOfDays { get; set; }
        public int StartInventoryQuantity { get; set; }
        public int StartLeadDays { get; set; }
        public int StartOrderQuantity { get; set; }
        public List<Distribution> DemandDistribution { get; set; }
        public List<Distribution> LeadDaysDistribution { get; set; }

        ///////////// OUTPUTS /////////////
        public List<SimulationCase> SimulationCases { get; set; }
        public PerformanceMeasures PerformanceMeasures { get; set; }
        #endregion

        public SimulationSystem()
        {
            DemandDistribution = new List<Distribution>();
            LeadDaysDistribution = new List<Distribution>();
            SimulationCases = new List<SimulationCase>();
            PerformanceMeasures = new PerformanceMeasures();
        }

        public void Simulate(int i)
        {
            _random = new Random();

            DataReader reader = new DataReader();
            reader.Read(i);
            OrderUpTo = reader.OrderUpTo;
            ReviewPeriod = reader.ReviewPeriod;
            NumberOfDays = reader.NumberOfDays;
            StartInventoryQuantity = reader.StartInventoryQuantity;
            StartLeadDays = reader.StartLeadDays;
            StartOrderQuantity = reader.StartOrderQuantity;
            DemandDistribution = reader.DemandDistribution;
            LeadDaysDistribution = reader.LeadDaysDistribution;

            CalcDemandDistributionsAccumilative();
            CalcLeadDaysDistributionsAccumilative();
            Simulator();
            CalcPerformance();
        }

        private void Simulator()
        {
            int accumShortage = 0;//total shortage
            int inProcessing = StartOrderQuantity; //num refrig. coming
            int daysLeft = StartLeadDays;
            string[] tmp;
            for (int i = 0; i < NumberOfDays; i++)
            {
                var simulationCase = new SimulationCase { Day = i + 1, Cycle = (i / ReviewPeriod) + 1, DayWithinCycle = (i % ReviewPeriod) + 1 };

                if (daysLeft == 0) {StartInventoryQuantity += inProcessing; inProcessing = 0; }

                simulationCase.BeginningInventory = StartInventoryQuantity;

                tmp = FakeDemand().Split(',');
                simulationCase.RandomDemand = Convert.ToInt32(tmp[0]);
                simulationCase.Demand = Convert.ToInt32(tmp[1]);

                //Shortage and Inventory
                if(simulationCase.Demand + accumShortage <= simulationCase.BeginningInventory)
                {
                    simulationCase.EndingInventory = simulationCase.BeginningInventory - simulationCase.Demand - accumShortage;
                    simulationCase.ShortageQuantity = 0;
                    accumShortage = 0;
                }
                else
                {
                    simulationCase.EndingInventory = 0;
                    simulationCase.ShortageQuantity = simulationCase.Demand + accumShortage - simulationCase.BeginningInventory;
                    accumShortage = simulationCase.ShortageQuantity;
                }

                //Ordering
                if ((i+1)%ReviewPeriod == 0)
                {
                    simulationCase.OrderQuantity = OrderUpTo - simulationCase.EndingInventory + simulationCase.ShortageQuantity;
                    tmp = FakeLeadDay().Split(',');
                    simulationCase.RandomLeadDays = Convert.ToInt32(tmp[0]);
                    simulationCase.LeadDays = Convert.ToInt32(tmp[1]);

                    inProcessing = simulationCase.OrderQuantity;
                    daysLeft = simulationCase.LeadDays+1;
                }
                else
                {
                    simulationCase.OrderQuantity = simulationCase.RandomLeadDays = simulationCase.LeadDays = 0;
                }

                StartInventoryQuantity = simulationCase.EndingInventory;
                daysLeft--;
                SimulationCases.Add(simulationCase);
            }
        }

        private void CalcDemandDistributionsAccumilative()
        {
            DemandDistribution[0].CummProbability = DemandDistribution[0].Probability;
            DemandDistribution[0].MinRange = 0;
            DemandDistribution[0].MaxRange = (int)(DemandDistribution[0].CummProbability * 100);
            for (int i = 1; i < DemandDistribution.Count; i++)
            {
                DemandDistribution[i].CummProbability = DemandDistribution[i].Probability + DemandDistribution[i - 1].CummProbability;
                DemandDistribution[i].MinRange = DemandDistribution[i - 1].MaxRange + 1;
                DemandDistribution[i].MaxRange = (int)(DemandDistribution[i].CummProbability * 100);
            }
        }

        private void CalcLeadDaysDistributionsAccumilative()
        {
            LeadDaysDistribution[0].CummProbability = LeadDaysDistribution[0].Probability;
            LeadDaysDistribution[0].MinRange = 0;
            LeadDaysDistribution[0].MaxRange = (int)(LeadDaysDistribution[0].CummProbability * 100);
            for (int i = 1; i < LeadDaysDistribution.Count; i++)
            {
                LeadDaysDistribution[i].CummProbability = LeadDaysDistribution[i].Probability + LeadDaysDistribution[i - 1].CummProbability;
                LeadDaysDistribution[i].MinRange = LeadDaysDistribution[i - 1].MaxRange + 1;
                LeadDaysDistribution[i].MaxRange = (int)(LeadDaysDistribution[i].CummProbability * 100);
            }
        }

        private string FakeLeadDay()
        {
            int r = _random.Next(100) + 1;

            for (int i = 0; i < LeadDaysDistribution.Count; i++)
            {
                if (r >= LeadDaysDistribution[i].MinRange && r <= LeadDaysDistribution[i].MaxRange)
                    return string.Format(@"{0},{1}", r, LeadDaysDistribution[i].Value);
            }

            throw new Exception("ERROR IN RANDOM NUMBER GENERATION");
        }

        private string FakeDemand()
        {
            int r = _random.Next(100) + 1;

            for (int i = 0; i < DemandDistribution.Count; i++)
            {
                if (r >= DemandDistribution[i].MinRange && r <= DemandDistribution[i].MaxRange)
                    return string.Format(@"{0},{1}", r, DemandDistribution[i].Value);
            }

            throw new Exception("ERROR IN RANDOM NUMBER GENERATION");
        }

        private void CalcPerformance()
        {
            PerformanceMeasures.EndingInventoryAverage = (decimal)SimulationCases.Sum(c => c.EndingInventory) / SimulationCases.Count;
            PerformanceMeasures.ShortageQuantityAverage = (decimal)SimulationCases.Sum(c => c.ShortageQuantity) / SimulationCases.Count;
        }
    }
}
