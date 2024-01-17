using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
// \\ --[PrinterNotifyInfoData]----------------------------------------
// \\ Reads the information buffer provided by the print spooler in
// \\ response to FindFirstPrinterChangeNotification and 
// \\ FindNextPrinterChangeNotification
// \\ (c) Merrion Computing Ltd 
// \\     http://www.merrioncomputing.com
// \\ -----------------------------------------------------------------
using System.Runtime.InteropServices;
using PrinterQueueWatch.SpoolerApiConstantEnumerations;
using PrinterQueueWatch.SpoolerStructs;

namespace PrinterQueueWatch
{

    [StructLayout(LayoutKind.Sequential)]
    class PrinterNotifyInfoData
    {
        public short wType;
        public short wField;
        public int dwReserved;
        public int dwId;
        public IntPtr cbBuff;
        public IntPtr pBuff;

        public PrinterNotifyInfoData(IntPtr lpAddress)
        {

            if (TraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("New(" + lpAddress.ToString() + ")", GetType().ToString());
            }
            Marshal.PtrToStructure(lpAddress, this);

        }

        public Printer_Notification_Types Type
        {
            get
            {
                return (Printer_Notification_Types)wType;
            }
        }

        public Job_Notify_Field_Indexes Field
        {
            get
            {
                return (Job_Notify_Field_Indexes)wField;
            }
        }

        public override string ToString()
        {
            return Marshal.PtrToStringAnsi(pBuff);
        }

        public int ToInt32()
        {
            return cbBuff.ToInt32();
        }

        #region Tracing
        public static TraceSwitch TraceSwitch = new TraceSwitch("PrinterNotifyInfoData", "Printer Monitor Component Tracing");
        #endregion

    }

    [StructLayout(LayoutKind.Sequential)]
    class PrinterNotifyInfo
    {

        #region Private member variables

        private ArrayList colPrintJobs;
        private PRINTER_NOTIFY_INFO msInfo = new PRINTER_NOTIFY_INFO();
        private int mlPrinterInfoChanged = 0;

        #endregion

        public PrinterNotifyInfo(IntPtr mhPrinter, IntPtr lpAddress, ref PrintJobCollection PrintJobs)
        {

            if (PrinterNotifyInfoData.TraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("New(" + mhPrinter.ToString() + "," + lpAddress.ToString() + ")", GetType().ToString());
            }

            if (!(lpAddress.ToInt64() == 0L))
            {
                // \\ Create the array list of jobs involved in this event
                colPrintJobs = new ArrayList();

                // \\ Read the data of this printer notification event
                Marshal.PtrToStructure(lpAddress, msInfo);

                int nInfoDataItem;
                // \\ Offset the pointer by the size of this class
                var lOffset = lpAddress + Marshal.SizeOf(msInfo) + (IntPtr.Size == 8 ? 4 : 0); // Fix 64bit data allignment issue

                // \\ Process the .adata array
                var loopTo = msInfo.Count - 1;
                for (nInfoDataItem = 0; nInfoDataItem <= loopTo; nInfoDataItem++)
                {
                    var itemdata = new PrinterNotifyInfoData(lOffset);
                    if (itemdata.Type == Printer_Notification_Types.JOB_NOTIFY_TYPE)
                    {

                        if (itemdata.dwId == 0)
                        {
                            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceWarning)
                            {
                                Trace.WriteLine("JOB_NOTIFY_TYPE has zero job id ");
                            }
                        }

                        PrintJob pjThis;
                        // \\ If this job is not on the printer job list, add it...
                        pjThis = PrintJobs.get_AddOrGetById(itemdata.dwId, mhPrinter);

                        if (!colPrintJobs.Contains(itemdata.dwId))
                        {
                            colPrintJobs.Add(itemdata.dwId);
                        }


                        if (pjThis is not null)
                        {
                            switch (itemdata.Field)
                            {
                                case Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_PRINTER_NAME:
                                    {
                                        pjThis.InitPrinterName = Marshal.PtrToStringUni(itemdata.pBuff);
                                        break;
                                    }
                                case Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_USER_NAME:
                                    {
                                        pjThis.InitUsername = Marshal.PtrToStringUni(itemdata.pBuff);
                                        break;
                                    }
                                case Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_MACHINE_NAME:
                                    {
                                        pjThis.InitMachineName = Marshal.PtrToStringUni(itemdata.pBuff);
                                        break;
                                    }
                                case Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_DATATYPE:
                                    {
                                        pjThis.InitDataType = Marshal.PtrToStringUni(itemdata.pBuff);
                                        break;
                                    }
                                case Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_DOCUMENT:
                                    {
                                        pjThis.InitDocument = Marshal.PtrToStringUni(itemdata.pBuff);
                                        break;
                                    }
                                case Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_DRIVER_NAME:
                                    {
                                        pjThis.InitDrivername = Marshal.PtrToStringUni(itemdata.pBuff);
                                        break;
                                    }
                                case Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_NOTIFY_NAME:
                                    {
                                        pjThis.InitNotifyUsername = Marshal.PtrToStringUni(itemdata.pBuff);
                                        break;
                                    }
                                case Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_PAGES_PRINTED:
                                    {
                                        pjThis.InitPagesPrinted = itemdata.ToInt32();
                                        break;
                                    }
                                case Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_PARAMETERS:
                                    {
                                        pjThis.InitParameters = Marshal.PtrToStringUni(itemdata.pBuff);
                                        break;
                                    }
                                case Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_POSITION:
                                    {
                                        pjThis.InitPosition = itemdata.ToInt32();
                                        break;
                                    }
                                case Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_PRINT_PROCESSOR:
                                    {
                                        pjThis.InitPrintProcessorName = Marshal.PtrToStringUni(itemdata.pBuff);
                                        break;
                                    }
                                case Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_PRIORITY:
                                    {
                                        pjThis.InitPriority = itemdata.ToInt32();
                                        break;
                                    }
                                case Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_STATUS:
                                    {
                                        pjThis.InitStatus = itemdata.ToInt32();
                                        break;
                                    }
                                case Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_STATUS_STRING:
                                    {
                                        pjThis.InitStatusDescription = Marshal.PtrToStringUni(itemdata.pBuff);
                                        break;
                                    }
                                case Job_Notify_Field_Indexes.JOB_NOTIFY_FIELD_TOTAL_PAGES:
                                    {
                                        pjThis.InitTotalPages = itemdata.ToInt32();
                                        break;
                                    }

                                default:
                                    {
                                        break;
                                    }
                                    // \\ These are not available except where the print job is "live" 
                            }
                        }
                    }
                    else
                    {
                        // \\ Printer Info changed event
                        mlPrinterInfoChanged = (int)Math.Round(mlPrinterInfoChanged + Math.Pow(2d, (double)itemdata.Field));
                    }
                    lOffset = lOffset + Marshal.SizeOf(itemdata);
                }

                // \\ And free the associated memory
                if (!UnsafeNativeMethods.FreePrinterNotifyInfo(lpAddress))
                {
                    if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                    {
                        Trace.WriteLine("FreePrinterNotifyInfo(" + lpAddress.ToString() + ") failed", GetType().ToString());
                    }
                    throw new Win32Exception();
                }
            }

        }

        public int Flags
        {
            get
            {
                return msInfo.Flags;
            }
        }

        public ArrayList PrintJobs
        {
            get
            {
                return colPrintJobs;
            }
        }

        public bool PrinterInfoChanged
        {
            get
            {
                return mlPrinterInfoChanged > 0;
            }
        }

        internal int PrinterInfoChangeFlags
        {
            get
            {
                return mlPrinterInfoChanged;
            }
        }

    }
}