using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace PrinterQueueWatch.TestApplication
{
    public partial class Form_PrintQueueWatchTest
    {
        public Form_PrintQueueWatchTest()
        {
            InitializeComponent();
        }

        private void Form_PrintQueueWatchTest_Disposed(object sender, EventArgs e)
        {
            // Don't leave any printers being monitored when the form is gone...
            PrinterMonitorComponent1.Disconnect();
        }

        private void Form_PrintQueueWatchTest_Load(object sender, EventArgs e)
        {

            foreach (var p in new PrinterInformationCollection())
                CheckedListBox_Printers.Items.Add(p.PrinterName);

        }


        private void CheckedListBox_Printers_ItemCheck(object sender, ItemCheckEventArgs e)
        {

            if (e.NewValue == CheckState.Checked)
            {
                PrinterMonitorComponent1.AddPrinter(Conversions.ToString(CheckedListBox_Printers.Items[e.Index]));
            }
            else
            {
                PrinterMonitorComponent1.RemovePrinter(Conversions.ToString(CheckedListBox_Printers.Items[e.Index]));
            }

        }



        private void PrinterMonitorComponent1_JobAdded(object sender, PrintJobEventArgs e)
        {
            Trace.TraceInformation("Job added " + e.PrintJob.JobId + " called " + e.PrintJob.Document + " on " + e.PrintJob.PrinterName);

            // Do any other fundctionality here...

        }


        private void PrinterMonitorComponent1_JobDeleted(object sender, PrintJobEventArgs e)
        {
            Trace.TraceInformation("Job deleted " + e.PrintJob.JobId + " called " + e.PrintJob.Document + " on " + e.PrintJob.PrinterName);

            // Do any other fundctionality here...
        }


        private void PrinterMonitorComponent1_PrinterInformationChanged(object sender, PrinterEventArgs e)
        {
            Trace.TraceInformation("Printer information changed " + e.PrinterInformation.PrinterName + " - " + e.PrinterChangeFlags.ToString());
        }
    }
}