using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InventoryModels;
using InventoryTesting;

namespace InventorySimulation
{
    public partial class Form1 : Form
    {
        public Form1(SimulationSystem system)
        {
            InitializeComponent();
            dataGridView.ColumnCount = 11;
            dataGridView.Columns[0].Name = "Day";
            dataGridView.Columns[1].Name = "Cycle";
            dataGridView.Columns[2].Name = "Day Within Cycle";
            dataGridView.Columns[3].Name = "Beggining Inventory";
            dataGridView.Columns[4].Name = "Random Demand";
            dataGridView.Columns[5].Name = "Demand";
            dataGridView.Columns[6].Name = "Ending Inventory";
            dataGridView.Columns[7].Name = "Shortage Quantity";
            dataGridView.Columns[8].Name = "Order Quantity";
            dataGridView.Columns[9].Name = "Random Lead";
            dataGridView.Columns[10].Name = "Lead";



            for (int i = 0; i < system.SimulationCases.Count; i++)
            {
                dataGridView.Rows.Add(
                    system.SimulationCases[i].Day,
                    system.SimulationCases[i].Cycle,
                    system.SimulationCases[i].DayWithinCycle,
                    system.SimulationCases[i].BeginningInventory,
                    system.SimulationCases[i].RandomDemand,
                    system.SimulationCases[i].Demand,
                    system.SimulationCases[i].EndingInventory,
                    system.SimulationCases[i].ShortageQuantity,
                    system.SimulationCases[i].OrderQuantity,
                    system.SimulationCases[i].RandomLeadDays,
                    system.SimulationCases[i].LeadDays);
            }

            Value1.Text = system.PerformanceMeasures.EndingInventoryAverage.ToString();
            Value2.Text = system.PerformanceMeasures.ShortageQuantityAverage.ToString();
        }
    }
}
