using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace PrinterQueueWatch.TestApplication
{
    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class Form_PrintQueueWatchTest : Form
    {

        // Form overrides dispose to clean up the component list.
        [DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components is not null)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        // Required by the Windows Form Designer
        private System.ComponentModel.IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            CheckedListBox_Printers = new CheckedListBox();
            CheckedListBox_Printers.ItemCheck += new ItemCheckEventHandler(CheckedListBox_Printers_ItemCheck);
            Label1 = new Label();
            PrinterMonitorComponent1 = new PrinterMonitorComponent();
            PrinterMonitorComponent1.JobAdded += new PrinterMonitorComponent.PrintJobEventHandler(PrinterMonitorComponent1_JobAdded);
            PrinterMonitorComponent1.JobDeleted += new PrinterMonitorComponent.PrintJobEventHandler(PrinterMonitorComponent1_JobDeleted);
            PrinterMonitorComponent1.PrinterInformationChanged += new PrinterMonitorComponent.PrinterEventHandler(PrinterMonitorComponent1_PrinterInformationChanged);
            SuspendLayout();
            // 
            // CheckedListBox_Printers
            // 
            CheckedListBox_Printers.FormattingEnabled = true;
            CheckedListBox_Printers.Location = new Point(3, 27);
            CheckedListBox_Printers.Name = "CheckedListBox_Printers";
            CheckedListBox_Printers.Size = new Size(218, 304);
            CheckedListBox_Printers.TabIndex = 0;
            // 
            // Label1
            // 
            Label1.AutoSize = true;
            Label1.Location = new Point(0, 9);
            Label1.Name = "Label1";
            Label1.Size = new Size(37, 13);
            Label1.TabIndex = 1;
            Label1.Text = "Printer";
            // 
            // PrinterMonitorComponent1
            // 
            PrinterMonitorComponent1.MonitorJobAddedEvent = true;
            PrinterMonitorComponent1.MonitorJobDeletedEvent = true;
            PrinterMonitorComponent1.MonitorJobSetEvent = true;
            PrinterMonitorComponent1.MonitorJobWrittenEvent = true;
            PrinterMonitorComponent1.MonitorPrinterChangeEvent = true;
            // 
            // Form_PrintQueueWatchTest
            // 
            AutoScaleDimensions = new SizeF(6.0f, 13.0f);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(671, 344);
            Controls.Add(Label1);
            Controls.Add(CheckedListBox_Printers);
            Name = "Form_PrintQueueWatchTest";
            Text = "PQW Test";
            Disposed += new EventHandler(Form_PrintQueueWatchTest_Disposed);
            Load += new EventHandler(Form_PrintQueueWatchTest_Load);
            ResumeLayout(false);
            PerformLayout();

        }
        internal CheckedListBox CheckedListBox_Printers;
        internal Label Label1;
        internal PrinterMonitorComponent PrinterMonitorComponent1;

    }
}