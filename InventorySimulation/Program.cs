using InventoryModels;
using InventoryTesting;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace InventorySimulation
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SimulationSystem system;
            system  = new SimulationSystem();
            system.Simulate(1);
            string result = TestingManager.Test(system, Constants.FileNames.TestCase1);
            MessageBox.Show(result);            
            

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(system));
        }
    }
}
