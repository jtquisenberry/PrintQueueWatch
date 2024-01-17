using System;
using System.Runtime.InteropServices;
using PrinterQueueWatch.SpoolerApiConstantEnumerations;
using PrinterQueueWatch.SpoolerStructs;

namespace PrinterQueueWatch
{

    static class UnsafeNativeMethods
    {
        // -- Notes: -------------------------------------------------------------------------------
        // Always use <InAttribute()> and <OutAttribute()> to cut down on unnecessary marshalling
        // -----------------------------------------------------------------------------------------
        #region Api Declarations

        #region OpenPrinter
        [DllImport("winspool.drv", EntryPoint = "OpenPrinter", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool OpenPrinter([In()] string pPrinterName, out IntPtr phPrinter, [In()][MarshalAs(UnmanagedType.LPStruct)] PRINTER_DEFAULTS pDefault);



        [DllImport("winspool.drv", EntryPoint = "OpenPrinter", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool OpenPrinter([In()] string pPrinterName, out IntPtr phPrinter, [In()] int pDefault);





        [DllImport("winspool.drv", EntryPoint = "OpenPrinter", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool OpenPrinter([In()] string pPrinterName, out IntPtr phPrinter, [In()] PrinterDefaults pDefault);




        #endregion

        #region ClosePrinter
        [DllImport("winspool.drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]




        public static extern bool ClosePrinter([In()] IntPtr hPrinter);
        #endregion

        #region GetPrinter
        [DllImport("winspool.drv", EntryPoint = "GetPrinter", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool GetPrinter([In()] IntPtr hPrinter, [In()] int Level, IntPtr lpPrinter, [In()] int cbBuf, out IntPtr lpbSizeNeeded);




        #endregion

        #region EnumPrinters
        [DllImport("winspool.drv", EntryPoint = "EnumPrinters", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool EnumPrinters([In()] EnumPrinterFlags Flags, [In()] string Name, [In()] int Level, IntPtr lpBuf, [In()] int cbBuf, out int pcbNeeded, out int pcbReturned);






        [DllImport("winspool.drv", EntryPoint = "EnumPrinters", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool EnumPrinters([In()] EnumPrinterFlags Flags, [In()] int Name, [In()] int Level, IntPtr lpBuf, [In()] int cbBuf, out int pcbNeeded, out int pcbReturned);






        #endregion

        #region GetPrinterDriver
        [DllImport("winspool.drv", EntryPoint = "GetPrinterDriver", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool GetPrinterDriver([In()] IntPtr hPrinter, [In()] string pEnvironment, [In()] int Level, IntPtr lpDriverInfo, [In()] IntPtr cbBuf, out IntPtr lpbSizeNeeded);





        #endregion

        #region EnumPrinterDrivers
        [DllImport("winspool.drv", EntryPoint = "EnumPrinterDrivers", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool EnumPrinterDrivers([In()] string ServerName, [In()] string Environment, [In()] int Level, IntPtr lpBuf, [In()] int cbBuf, out int pcbNeeded, out int pcbReturned);





        #endregion

        #region SetPrinter
        [DllImport("winspool.drv", EntryPoint = "SetPrinter", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool SetPrinter([In()] IntPtr hPrinter, [In()] int Level, [In()] IntPtr pPrinter, [In()] PrinterControlCommands Command);




        [DllImport("winspool.drv", EntryPoint = "SetPrinter", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool SetPrinter([In()] IntPtr hPrinter, [In()][MarshalAs(UnmanagedType.U4)] PrinterInfoLevels Level, [In()][MarshalAs(UnmanagedType.LPStruct)] PRINTER_INFO_1 pPrinter, [In()][MarshalAs(UnmanagedType.U4)] PrinterControlCommands Command);




        [DllImport("winspool.drv", EntryPoint = "SetPrinter", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool SetPrinter([In()] IntPtr hPrinter, [In()][MarshalAs(UnmanagedType.U4)] PrinterInfoLevels Level, [In()][MarshalAs(UnmanagedType.LPStruct)] PRINTER_INFO_2 pPrinter, [In()][MarshalAs(UnmanagedType.U4)] PrinterControlCommands Command);




        [DllImport("winspool.drv", EntryPoint = "SetPrinter", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool SetPrinter([In()] IntPtr hPrinter, [In()][MarshalAs(UnmanagedType.U4)] PrinterInfoLevels Level, [In()][MarshalAs(UnmanagedType.LPStruct)] PRINTER_INFO_3 pPrinter, [In()][MarshalAs(UnmanagedType.U4)] PrinterControlCommands Command);



        #endregion

        #region GetJob
        [DllImport("winspool.drv", EntryPoint = "GetJob", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool GetJob([In()] IntPtr hPrinter, [In()] int dwJobId, [In()] int Level, IntPtr lpJob, [In()] int cbBuf, out IntPtr lpbSizeNeeded);








        #endregion

        #region FindFirstPrinterChangeNotification


        [DllImport("winspool.drv", EntryPoint = "FindFirstPrinterChangeNotification", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern Microsoft.Win32.SafeHandles.SafeWaitHandle FindFirstPrinterChangeNotification([In()] IntPtr hPrinter, [In()] int fwFlags, [In()] int fwOptions, [In()][MarshalAs(UnmanagedType.LPStruct)] PrinterNotifyOptions pPrinterNotifyOptions);





        [DllImport("winspool.drv", EntryPoint = "FindFirstPrinterChangeNotification", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern IntPtr UnsafeFindFirstPrinterChangeNotification([In()] IntPtr hPrinter, [In()] int fwFlags, [In()] int fwOptions, [In()] IntPtr pPrinterNotifyOptions);





        [DllImport("winspool.drv", EntryPoint = "FindFirstPrinterChangeNotification", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern Microsoft.Win32.SafeHandles.SafeWaitHandle FindFirstPrinterChangeNotification([In()] IntPtr hPrinter, [In()] int fwFlags, [In()] int fwOptions, [In()] IntPtr pPrinterNotifyOptions);




        #endregion

        #region FindNextPrinterChangeNotification
        [DllImport("winspool.drv", EntryPoint = "FindNextPrinterChangeNotification", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]



        public static extern bool FindNextPrinterChangeNotification([In()] IntPtr hChangeObject, out int pdwChange, [In()][MarshalAs(UnmanagedType.LPStruct)] PrinterNotifyOptions pPrinterNotifyOptions, out IntPtr lppPrinterNotifyInfo);





        [DllImport("winspool.drv", EntryPoint = "FindNextPrinterChangeNotification", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]



        public static extern bool FindNextPrinterChangeNotification([In()] Microsoft.Win32.SafeHandles.SafeWaitHandle hChangeObject, out int pdwChange, [In()][MarshalAs(UnmanagedType.LPStruct)] PrinterNotifyOptions pPrinterNotifyOptions, out IntPtr lppPrinterNotifyInfo);




        #endregion

        #region FreePrinterNotifyInfo
        [DllImport("winspool.drv", EntryPoint = "FreePrinterNotifyInfo", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool FreePrinterNotifyInfo([In()] IntPtr lppPrinterNotifyInfo);
        #endregion

        #region FindClosePrinterChangeNotification
        [DllImport("winspool.drv", EntryPoint = "FindClosePrinterChangeNotification", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool FindClosePrinterChangeNotification([In()] int hChangeObject);

        [DllImport("winspool.drv", EntryPoint = "FindClosePrinterChangeNotification", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool FindClosePrinterChangeNotification([In()] Microsoft.Win32.SafeHandles.SafeWaitHandle hChangeObject);
        #endregion

        #region EnumJobs
        [DllImport("winspool.drv", EntryPoint = "EnumJobs", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]





        public static extern bool EnumJobs([In()] IntPtr hPrinter, [In()] int FirstJob, [In()] int NumberOfJobs, [In()][MarshalAs(UnmanagedType.U4)] JobInfoLevels Level, IntPtr pbOut, [In()] int cbIn, out int pcbNeeded, out int pcReturned);








        #endregion

        #region SetJob
        [DllImport("winspool.drv", EntryPoint = "SetJob", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool SetJob([In()] IntPtr hPrinter, [In()] int dwJobId, [In()] int Level, [In()] IntPtr lpJob, [In()][MarshalAs(UnmanagedType.U4)] PrintJobControlCommands dwCommand);






        [DllImport("winspool.drv", EntryPoint = "SetJob", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool SetJob(IntPtr hPrinter, int dwJobId, int Level, [MarshalAs(UnmanagedType.LPStruct)] JOB_INFO_1 lpJob, PrintJobControlCommands dwCommand);






        [DllImport("winspool.drv", EntryPoint = "SetJob", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool SetJob(IntPtr hPrinter, int dwJobId, int Level, [MarshalAs(UnmanagedType.LPStruct)] JOB_INFO_2 lpJob, PrintJobControlCommands dwCommand);





        #endregion

        #region EnumMonitors
        [DllImport("winspool.drv", EntryPoint = "EnumMonitors", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]





        public static extern bool EnumMonitors([In()] string ServerName, [In()] int Level, int lpBuf, [In()] int cbBuf, out int pcbNeeded, out int pcReturned);





        [DllImport("winspool.drv", EntryPoint = "EnumMonitors", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]





        public static extern bool EnumMonitors([In()] int pServerName, [In()] int Level, int lpBuf, [In()] int cbBuf, out int pcbNeeded, out int pcReturned);




        #endregion

        #region DocumentProperties
        [DllImport("winspool.drv", EntryPoint = "DocumentProperties", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern int DocumentProperties([In()] int hwnd, [In()] IntPtr hPrinter, [In()] string pPrinterName, out int pDevModeOut, [In()] int pDevModeIn, [In()] DocumentPropertiesModes Mode);





        #endregion

        #region DeviceCapabilities
        [DllImport("winspool.drv", EntryPoint = "DeviceCapabilities", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern int DeviceCapabilities([In()] string pPrinterName, [In()] string pPortName, [In()][MarshalAs(UnmanagedType.U4)] PrintDeviceCapabilitiesIndexes CapbilityIndex, out int lpOut, [In()] int pDevMode);




        #endregion

        #region GetPrinterDriverDirectory
        [DllImport("winspool.drv", EntryPoint = "GetPrinterDriverDirectory", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool GetPrinterDriverDirectory([In()] string ServerName, [In()] string Environment, [In()] int Level, out string DriverDirectory, [In()] int BufferSize, out int BytesNeeded);






        [DllImport("winspool.drv", EntryPoint = "GetPrinterDriverDirectory", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool GetPrinterDriverDirectory([In()] int ServerName, [In()] int Environment, [In()] int Level, out string DriverDirectory, [In()] int BufferSize, out int BytesNeeded);





        #endregion

        #region AddPrinterDriver
        [DllImport("winspool.drv", EntryPoint = "AddPrinterDriver", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool AddPrinterDriver([In()] string ServerName, [In()] int Level, [In()][MarshalAs(UnmanagedType.LPStruct)] DRIVER_INFO_2 pDriverInfo);



        #endregion

        #region EnumPorts
        [DllImport("winspool.drv", EntryPoint = "EnumPorts", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool EnumPorts([In()] string ServerName, [In()] int Level, int pbOut, [In()] int cbIn, out int pcbNeeded, out int pcReturned);







        [DllImport("winspool.drv", EntryPoint = "EnumPorts", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool EnumPorts([In()] int ServerName, [In()] int Level, int pbOut, [In()] int cbIn, out int pcbNeeded, out int pcReturned);






        #endregion

        #region SetPort
        [DllImport("winspool.drv", EntryPoint = "SetPort", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]





        public static extern bool SetPort([In()] string ServerName, [In()] string PortName, [In()] long Level, [In()][MarshalAs(UnmanagedType.LPStruct)] PORT_INFO_3 PortInfo);




        #endregion

        #region EnumPrintProcessors
        [DllImport("winspool.drv", EntryPoint = "EnumPrintProcessors", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]





        public static extern bool EnumPrintProcessors([In()] string ServerName, [In()] string Environment, [In()] int Level, int lpBuf, [In()] int cbBuf, out int pcbNeeded, out int pcbReturned);






        #endregion

        #region EnumPrintProcessorDataTypes
        [DllImport("winspool.drv", EntryPoint = "EnumPrintProcessorDatatypes", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool EnumPrinterProcessorDataTypes([In()] string ServerName, [In()] string PrintProcessorName, [In()] int Level, int pDataTypes, [In()] int cbBuf, out int pcbNeeded, out int pcReturned);






        #endregion

        #region GetForm
        [DllImport("winspool.drv", EntryPoint = "GetForm", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]





        public static extern bool GetForm([In()] int PrinterHandle, [In()] string FormName, [In()] int Level, out FORM_INFO_1 pForm, [In()] int cbBuf, out int pcbNeeded);





        #endregion

        #region SetForm
        [DllImport("winspool.drv", EntryPoint = "SetForm", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool SetForm([In()] IntPtr PrinterHandle, [In()] string FormName, [In()] int Level, [In()] ref FORM_INFO_1 pForm);



        #endregion

        #region EnumForms
        [DllImport("winspool.drv", EntryPoint = "EnumForms", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool EnumForms([In()] IntPtr hPrinter, [In()] int Level, IntPtr pForm, [In()] int cbBuf, out int pcbNeeded, out int pcFormsReturned);





        #endregion

        #region ReadPrinter
        [DllImport("winspool.drv", EntryPoint = "ReadPrinter", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool ReadPrinter([In()] IntPtr hPrinter, IntPtr pBuffer, [In()] int cbBuf, out int pcbNeeded);



        #endregion

        #region WritePrinter
        [DllImport("winspool.drv", EntryPoint = "WritePrinter", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool WritePrinter([In()] IntPtr hPrinter, IntPtr pBuffer, [In()] int cbBuf, out int pcbNeeded);


        #endregion

        #region StartDocPrinter
        [DllImport("winspool.drv", EntryPoint = "StartDocPrinter", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool StartDocPrinter([In()] IntPtr hPrinter, [In()] int Level, [In()][MarshalAs(UnmanagedType.LPStruct)] DOC_INFO_1 DocInfo);

        #endregion

        #region EndDocPrinter
        [DllImport("winspool.drv", EntryPoint = "EndDocPrinter", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool EndDocPrinter([In()] IntPtr hPrinter);
        #endregion

        #region StartPagePrinter
        [DllImport("winspool.drv", EntryPoint = "StartPagePrinter", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool StartPagePrinter([In()] IntPtr hPrinter);
        #endregion

        #region EndPagePrinter
        [DllImport("winspool.drv", EntryPoint = "EndPagePrinter", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]




        public static extern bool EndPagePrinter([In()] IntPtr hPrinter);
        #endregion

        #endregion

    }
}