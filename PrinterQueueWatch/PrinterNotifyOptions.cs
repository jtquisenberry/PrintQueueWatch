using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using PrinterQueueWatch.SpoolerApiConstantEnumerations;

namespace PrinterQueueWatch
{

    // \\--PrinterNotifyOptionsType---------------------------------------------------
    // \\ Implements the API structure PRINTER_NOTIFY_OPTIONS_TYPE used in monitoring
    // \\ a print queue using FindFirstPrinterChangeNotification and 
    // \\ FindNextPrinterChangeNotification loop
    // \\ (c) 2002-2003 Merrion Computing Ltd
    // \\     http://www.merrioncomputing.com
    // \\------------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential)]
    class PrinterNotifyOptionsType : IDisposable
    {

        public short wJobType;
        public short wJobReserved0;
        public int dwJobReserved1;
        public int dwJobReserved2;
        public int JobFieldCount;
        public IntPtr pJobFields;
        public short wPrinterType;
        public short wPrinterReserved0;
        public int dwPrinterReserved1;
        public int dwPrinterReserved2;
        public int PrinterFieldCount;
        public IntPtr pPrinterFields;

        #region Public Enumerated Types

        private const int JOB_FIELDS_COUNT = 24;
        private const int PRINTER_FIELDS_COUNT = 8;

        #endregion

        private void SetupFields(bool MinimumJobInfoRequired)
        {

            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("SetupFields()", GetType().ToString());
            }
            wJobType = (short)Printer_Notification_Types.JOB_NOTIFY_TYPE;
            wPrinterType = (short)Printer_Notification_Types.PRINTER_NOTIFY_TYPE;

            // \\ Free up the global memory
            if (pJobFields.ToInt64() != 0L)
            {
                Marshal.FreeHGlobal(pJobFields);
            }
            if (pPrinterFields.ToInt64() != 0L)
            {
                Marshal.FreeHGlobal(pPrinterFields);
            }

            if (MinimumJobInfoRequired)
            {
                JobFieldCount = 2;
            }
            else
            {
                JobFieldCount = JOB_FIELDS_COUNT;
            }

            pJobFields = Marshal.AllocHGlobal(JOB_FIELDS_COUNT * 2 - 1);
            var PrintJobFields = new short[24];

            PrintJobFields[0] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_DOCUMENT;
            PrintJobFields[1] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_STATUS;
            if (!MinimumJobInfoRequired)
            {
                PrintJobFields[2] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_MACHINE_NAME;
                PrintJobFields[3] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_PORT_NAME;
                PrintJobFields[4] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_USER_NAME;
                PrintJobFields[5] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_NOTIFY_NAME;
                PrintJobFields[6] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_DATATYPE;
                PrintJobFields[7] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_PRINT_PROCESSOR;
                PrintJobFields[8] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_PARAMETERS;
                PrintJobFields[9] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_DRIVER_NAME;
                PrintJobFields[10] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_DEVMODE;
                PrintJobFields[11] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_STATUS_STRING;
                PrintJobFields[12] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_SECURITY_DESCRIPTOR;
                PrintJobFields[13] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_PRINTER_NAME;
                PrintJobFields[14] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_PRIORITY;
                PrintJobFields[15] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_POSITION;
                PrintJobFields[16] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_SUBMITTED;
                PrintJobFields[17] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_START_TIME;
                PrintJobFields[18] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_UNTIL_TIME;
                PrintJobFields[19] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_TIME;
                PrintJobFields[20] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_TOTAL_PAGES;
                PrintJobFields[21] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_PAGES_PRINTED;
                PrintJobFields[22] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_TOTAL_BYTES;
                PrintJobFields[23] = (short)Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_BYTES_PRINTED;
            }
            Marshal.Copy(PrintJobFields, 0, pJobFields, PrintJobFields.GetLength(0));

            // \\ Request less printer notification details for economy sake...
            if (MinimumJobInfoRequired)
            {
                PrinterFieldCount = 1;
            }
            else
            {
                PrinterFieldCount = PRINTER_FIELDS_COUNT;
            }

            pPrinterFields = Marshal.AllocHGlobal((PRINTER_FIELDS_COUNT - 1) * 2);
            var PrinterFields = new short[8];
            PrinterFields[0] = (short)Printer_Notify_Field_Indexes.PRINTER_NOTIFY_FIELD_STATUS;

            if (!MinimumJobInfoRequired)
            {
                PrinterFields[1] = (short)Printer_Notify_Field_Indexes.PRINTER_NOTIFY_FIELD_CJOBS;
                PrinterFields[2] = (short)Printer_Notify_Field_Indexes.PRINTER_NOTIFY_FIELD_ATTRIBUTES;
                PrinterFields[3] = (short)Printer_Notify_Field_Indexes.PRINTER_NOTIFY_FIELD_COMMENT;
                PrinterFields[4] = (short)Printer_Notify_Field_Indexes.PRINTER_NOTIFY_FIELD_DEVMODE;
                PrinterFields[5] = (short)Printer_Notify_Field_Indexes.PRINTER_NOTIFY_FIELD_LOCATION;
                PrinterFields[6] = (short)Printer_Notify_Field_Indexes.PRINTER_NOTIFY_FIELD_SECURITY_DESCRIPTOR;
                PrinterFields[7] = (short)Printer_Notify_Field_Indexes.PRINTER_NOTIFY_FIELD_SEPFILE;
            }
            Marshal.Copy(PrinterFields, 0, pPrinterFields, PrinterFields.GetLength(0));

        }

        public PrinterNotifyOptionsType(bool MinimumJobInfoRequired)
        {
            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("New(" + MinimumJobInfoRequired.ToString() + ")", GetType().ToString());
            }
            SetupFields(MinimumJobInfoRequired);

        }


        public void ReleaseResources()
        {
            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("ReleaseResources()", GetType().ToString());
            }
            if (pJobFields.ToInt64() != 0L)
            {
                Marshal.FreeHGlobal(pJobFields);
                pJobFields = default;
            }
            if (pPrinterFields.ToInt64() != 0L)
            {
                Marshal.FreeHGlobal(pPrinterFields);
                pPrinterFields = default;
            }
        }

        #region IDisposable interface implementation
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        public void Dispose()
        {
            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("Dispose()", GetType().ToString());
            }
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // \\ Free up the global memory
                ReleaseResources();
            }
        }

        ~PrinterNotifyOptionsType()
        {
            Dispose(false);
        }

        #endregion
    }

    // \\--PrinterNotifyOptions-------------------------------------------------------
    // \\ Implements the API structure PRINTER_NOTIFY_OPTIONS used in monitoring
    // \\ a print queue using FindFirstPrinterChangeNotification and 
    // \\ FindNextPrinterChangeNotification loop
    // \\ (c) 2002-2003 Merrion Computing Ltd
    // \\     http://www.merrioncomputing.com
    // \\------------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential)]
    class PrinterNotifyOptions : IDisposable
    {

        public int dwVersion;
        public int dwFlags;
        public int Count;
        public IntPtr lpTypes;

        public PrinterNotifyOptions(bool MinimumJobInfoRequired)
        {

            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("New(" + MinimumJobInfoRequired.ToString() + ")", GetType().ToString());
            }

            // \\ As it stands, version is always 2
            dwVersion = 2;
            Count = 2;
            PrinterNotifyOptionsType pJobTypes;
            int BytesNeeded;

            pJobTypes = new PrinterNotifyOptionsType(MinimumJobInfoRequired);
            BytesNeeded = (2 + pJobTypes.JobFieldCount + pJobTypes.PrinterFieldCount) * 2;

            lpTypes = Marshal.AllocHGlobal(BytesNeeded);

            Marshal.StructureToPtr(pJobTypes, lpTypes, true);

        }

        #region IDisposable interface
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // \\ Free up the global memory
                if (lpTypes.ToInt64() != 0L)
                {
                    Marshal.FreeHGlobal(lpTypes);
                }
            }
        }

        ~PrinterNotifyOptions()
        {
            Dispose(false);
        }

        #endregion

    }
}