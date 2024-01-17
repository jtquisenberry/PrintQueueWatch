using System;
// \\ --[SpoolerStructs]--------------------------------------------------------------
// \\ The structures used by the Win32 API calls concerned with the spooler as 
// \\ defined in winspool.drv
// \\ (c) 2003 Merrion Computing Ltd
// \\ http://www.merrioncomputing.com
// \\ ---------------------------------------------------------------------------------
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using PrinterQueueWatch.SpoolerApiConstantEnumerations;


namespace PrinterQueueWatch.SpoolerStructs
{

    #region SYSTEMTIME STRUCTURE
    [StructLayout(LayoutKind.Sequential)]
    internal class SYSTEMTIME
    {
        public short wYear;
        public short wMonth;
        public short wDayOfWeek;
        public short wDay;
        public short wHour;
        public short wMinute;
        public short wSecond;
        public short wMilliseconds;

        public DateTime ToDateTime()
        {

            return new DateTime(wYear, wMonth, wDay, wHour, wMinute, wSecond, wMilliseconds);

        }

    }
    #endregion

    #region JOB_INFO_1 STRUCTURE
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    internal class JOB_INFO_1
    {
        public int JobId;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pPrinterName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pMachineName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pUserName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDocument;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDatatype;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pStatus;
        [MarshalAs(UnmanagedType.U4)]
        public int Status;
        public int Priority;
        public int Position;
        public int TotalPage;
        public int PagesPrinted;
        [MarshalAs(UnmanagedType.Struct)]
        public SYSTEMTIME Submitted;

        public JOB_INFO_1()
        {
            // \\ If this structure is not "Live" then a printer handle and job id are not used
        }

        public JOB_INFO_1(IntPtr hPrinter, int dwJobId)
        {

            IntPtr BytesWritten;
            IntPtr ptBuf;


            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("JOB_INFO_1 new(" + hPrinter.ToString() + "," + dwJobId.ToString() + ")");
            }


            // \\ Get the required buffer size
            if (!UnsafeNativeMethods.GetJob(hPrinter, dwJobId, 1, ptBuf, 0, out BytesWritten))
            {
                if (BytesWritten.ToInt64() == 0L)
                {
                    if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                    {
                        Trace.WriteLine("GetJob for JOB_INFO_1 failed on handle: " + hPrinter.ToString() + " for job: " + dwJobId, GetType().ToString());
                    }
                    throw new Win32Exception();
                    return;
                }
            }

            // \\ Allocate a buffer the right size
            if (BytesWritten.ToInt64() > 0L)
            {
                ptBuf = Marshal.AllocHGlobal(BytesWritten);
            }

            // \\ Populate the JOB_INFO_1 structure
            if (!UnsafeNativeMethods.GetJob(hPrinter, dwJobId, 1, ptBuf, BytesWritten.ToInt32(), out BytesWritten))
            {
                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                {
                    Trace.WriteLine("GetJob for JOB_INFO_1 failed on handle: " + hPrinter.ToString() + " for job: " + dwJobId, GetType().ToString());
                }
                throw new Win32Exception();
                return;
            }
            else
            {
                Marshal.PtrToStructure(ptBuf, this);
            }

            // \\ Free the allocated memory
            Marshal.FreeHGlobal(ptBuf);

        }

        public JOB_INFO_1(IntPtr lpJob)
        {
            Marshal.PtrToStructure(lpJob, this);

            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("JOB_INFO_1 new(" + lpJob.ToString() + ")");
            }

        }

    }
    #endregion

    #region DEVMODE STRUCTURE
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal class DEVMODE
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string pDeviceName;
        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;
        public short dmOrientation;
        public short dmPaperSize;
        public short dmPaperLength;
        public short dmPaperWidth;
        public short dmScale;
        public short dmCopies;
        public short dmDefaultSource;
        public short dmPrintQuality;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmFormName;
        public short dmUnusedPadding;
        public int dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;
        public int dmNup;
        public int dmDisplayFrequency;
        public int dmICMMethod;
        public int dmICMIntent;
        public int dmMediaType;
        public int dmDitherType;
        public int dmReserved1;
        public int dmReserved2;
        public int dmPanningWidth;
        public int dmPanningHeight;


    }
    #endregion

    #region JOB_INFO_2 STRUCTURE
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    internal class JOB_INFO_2
    {

        public int JobId;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pPrinterName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pMachineName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pUserName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pDocument;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pNotifyName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pDatatype;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pPrintProcessor;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pParameters;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pDriverName;
        public IntPtr LPDeviceMode;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pStatus;
        public IntPtr lpSecurity;
        [MarshalAs(UnmanagedType.U4)]
        public PrintJobStatuses Status;
        public int Priority;
        public int Position;
        public int StartTime;
        public int UntilTime;
        public int TotalPage;
        public int JobSize;
        [MarshalAs(UnmanagedType.Struct)]
        public SYSTEMTIME Submitted;
        public int Time;
        public int PagesPrinted;

        #region Private member variables
        private DEVMODE dmOut = new DEVMODE();
        #endregion

        public JOB_INFO_2()
        {
            // \\ If this structure is not "Live" then a printer handle and job id are not used
        }

        public JOB_INFO_2(IntPtr hPrinter, int dwJobId)
        {

            IntPtr BytesWritten;
            IntPtr ptBuf;

            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("JOB_INFO_1 new(" + hPrinter.ToString() + "," + dwJobId.ToString() + ")");
            }

            // \\ Get the required buffer size
            if (!UnsafeNativeMethods.GetJob(hPrinter, dwJobId, 2, ptBuf, 0, out BytesWritten))
            {
                if (BytesWritten.ToInt64() == 0L)
                {
                    if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                    {
                        Trace.WriteLine("GetJob for JOB_INFO_2 failed on handle: " + hPrinter.ToString() + " for job: " + dwJobId, GetType().ToString());
                    }
                    throw new Win32Exception();
                    return;
                }
            }

            // \\ Allocate a buffer the right size
            if (BytesWritten.ToInt64() > 0L)
            {
                ptBuf = Marshal.AllocHGlobal(BytesWritten);
            }

            // \\ Populate the JOB_INFO_2 structure
            if (!UnsafeNativeMethods.GetJob(hPrinter, dwJobId, 2, ptBuf, BytesWritten.ToInt32(), out BytesWritten))
            {
                throw new Win32Exception();
                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                {
                    Trace.WriteLine("GetJob for JOB_INFO_2 failed on handle: " + hPrinter.ToString() + " for job: " + dwJobId, GetType().ToString());
                }
            }
            else
            {
                Marshal.PtrToStructure(ptBuf, this);
                // \\ And get the DEVMODE before the memory is freed...
                Marshal.PtrToStructure(LPDeviceMode, dmOut);
            }
            // \\ Free the allocated memory
            Marshal.FreeHGlobal(ptBuf);

            // \\ Prevent Null Reference exceptions
            if (pPrinterName is null)
                pPrinterName = "";
            if (pUserName is null)
                pUserName = "";
            if (pDocument is null)
                pDocument = "";
            if (pNotifyName is null)
                pNotifyName = "";
            if (pDatatype is null)
                pDatatype = "";
            if (pPrintProcessor is null)
                pPrintProcessor = "";
            if (pParameters is null)
                pParameters = "";
            if (pDriverName is null)
                pDriverName = "";
            if (pStatus is null)
                pStatus = "";

        }

        public DEVMODE DeviceMode
        {
            get
            {
                return dmOut;
            }
        }

    }
    #endregion

    #region PRINTER_DEFAULTS STRUCTURE
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal class PRINTER_DEFAULTS
    {
        public IntPtr lpDataType;
        public IntPtr lpDevMode;
        [MarshalAs(UnmanagedType.U4)]
        public PrinterAccessRights DesiredAccess;

        #region Public constructor
        // \\ If the device name is known we need to know what access rights we have to this device..
        public PRINTER_DEFAULTS(PrinterInformation PrinterInfo)
        {

            if (PrinterInfo.CanLoggedInUserAdministerPrinter())
            {
                DesiredAccess = PrinterAccessRights.PRINTER_ALL_ACCESS;
            }
            else
            {
                DesiredAccess = PrinterAccessRights.PRINTER_ACCESS_USE;
            }

        }

        public PRINTER_DEFAULTS()
        {

            if (MCLApiUtilities.LoggedInAsAdministrator())
            {
                DesiredAccess = PrinterAccessRights.PRINTER_ALL_ACCESS;
            }
            else
            {
                DesiredAccess = PrinterAccessRights.PRINTER_ACCESS_USE;
            }

        }

        public PRINTER_DEFAULTS(PrinterAccessRights DefaultDesiredAccess)
        {

            DesiredAccess = DefaultDesiredAccess;

        }

        #endregion

    }
    #endregion

    #region PRINTER_INFO_1
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    internal class PRINTER_INFO_1
    {
        [MarshalAs(UnmanagedType.U4)]
        public int Flags;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDescription;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pComment;

        #region Public constructors
        public PRINTER_INFO_1(IntPtr hPrinter)
        {

            IntPtr BytesWritten;
            IntPtr ptBuf;

            ptBuf = Marshal.AllocHGlobal(1);

            if (!UnsafeNativeMethods.GetPrinter(hPrinter, 1, ptBuf, 1, out BytesWritten))
            {
                if (BytesWritten.ToInt64() > 0L)
                {
                    // \\ Free the buffer allocated
                    Marshal.FreeHGlobal(ptBuf);
                    ptBuf = Marshal.AllocHGlobal(BytesWritten);
                    if (UnsafeNativeMethods.GetPrinter(hPrinter, 1, ptBuf, BytesWritten.ToInt32(), out BytesWritten))
                    {
                        Marshal.PtrToStructure(ptBuf, this);
                    }
                    else
                    {
                        /* TODO ERROR: Skipped IfDirectiveTrivia
                        #If ERROR_BUBBLING Then
                        *//* TODO ERROR: Skipped DisabledTextTrivia
                                                Throw New Win32Exception()
                        *//* TODO ERROR: Skipped EndIfDirectiveTrivia
                        #End If
                        */
                    }
                    // \\ Free this buffer again
                    Marshal.FreeHGlobal(ptBuf);
                }
                else
                {
                    /* TODO ERROR: Skipped IfDirectiveTrivia
                    #If ERROR_BUBBLING Then
                    *//* TODO ERROR: Skipped DisabledTextTrivia
                                        Throw New Win32Exception()
                    *//* TODO ERROR: Skipped EndIfDirectiveTrivia
                    #End If
                    */
                }
            }
            else
            {
                /* TODO ERROR: Skipped IfDirectiveTrivia
                #If ERROR_BUBBLING Then
                *//* TODO ERROR: Skipped DisabledTextTrivia
                                Throw New Win32Exception()
                *//* TODO ERROR: Skipped EndIfDirectiveTrivia
                #End If
                */
            }

        }

        public PRINTER_INFO_1()
        {

        }
        #endregion
    }
    #endregion

    #region PRINTER_INFO_2 STRUCTURE
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    internal class PRINTER_INFO_2
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pServerName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pPrinterName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pShareName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pPortName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDriverName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pComment;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pLocation;
        public IntPtr lpDeviceMode;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pSeperatorFilename;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pPrintProcessor;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDataType;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pParameters;
        public IntPtr lpSecurityDescriptor;
        public int Attributes;
        public int Priority;
        public int DefaultPriority;
        public int StartTime;
        public int UntilTime;
        public int Status;
        public int JobCount;
        public int AveragePPM;

        #region Private member variables
        private DEVMODE dmOut = new DEVMODE();
        #endregion

        #region Public constructors
        public PRINTER_INFO_2(IntPtr hPrinter)
        {

            var BytesWritten = IntPtr.Zero;
            IntPtr ptBuf;

            ptBuf = Marshal.AllocHGlobal(1);

            if (!UnsafeNativeMethods.GetPrinter(hPrinter, 2, ptBuf, 1, out BytesWritten))
            {
                if (BytesWritten.ToInt64() > 0L)
                {
                    // \\ Free the buffer allocated
                    Marshal.FreeHGlobal(ptBuf);
                    ptBuf = Marshal.AllocHGlobal(BytesWritten);
                    if (UnsafeNativeMethods.GetPrinter(hPrinter, 2, ptBuf, BytesWritten.ToInt32(), out BytesWritten))
                    {
                        Marshal.PtrToStructure(ptBuf, this);
                        // \\ Fill any missing members
                        if (pServerName is null)
                        {
                            pServerName = "";
                        }
                        // \\ If the devicemode is available, get it
                        if (lpDeviceMode.ToInt64() > 0L)
                        {
                            Marshal.PtrToStructure(lpDeviceMode, dmOut);
                        }
                    }
                    else
                    {
                        /* TODO ERROR: Skipped IfDirectiveTrivia
                        #If ERROR_BUBBLING Then
                        *//* TODO ERROR: Skipped DisabledTextTrivia
                                                Throw New Win32Exception()
                        *//* TODO ERROR: Skipped EndIfDirectiveTrivia
                        #End If
                        */
                    }
                    // \\ Free this buffer again
                    Marshal.FreeHGlobal(ptBuf);
                }
                else
                {
                    /* TODO ERROR: Skipped IfDirectiveTrivia
                    #If ERROR_BUBBLING Then
                    *//* TODO ERROR: Skipped DisabledTextTrivia
                                        Throw New Win32Exception()
                    *//* TODO ERROR: Skipped EndIfDirectiveTrivia
                    #End If
                    */
                }
            }
            else
            {
                /* TODO ERROR: Skipped IfDirectiveTrivia
                #If ERROR_BUBBLING Then
                *//* TODO ERROR: Skipped DisabledTextTrivia
                                Throw New Win32Exception()
                *//* TODO ERROR: Skipped EndIfDirectiveTrivia
                #End If
                */
            }

        }

        public DEVMODE DeviceMode
        {
            get
            {
                return dmOut;
            }
        }

        public PRINTER_INFO_2()
        {

        }


        #endregion

    }

    #endregion

    #region PRINTER_INFO_3 STRUCTURE
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    internal class PRINTER_INFO_3
    {
        public IntPtr pSecurityDescriptor;

        #region Public constructors
        public PRINTER_INFO_3(IntPtr hPrinter)
        {

            IntPtr BytesWritten;
            IntPtr ptBuf;

            ptBuf = Marshal.AllocHGlobal(1);

            if (!UnsafeNativeMethods.GetPrinter(hPrinter, 3, ptBuf, 1, out BytesWritten))
            {
                if (BytesWritten.ToInt64() > 0L)
                {
                    // \\ Free the buffer allocated
                    Marshal.FreeHGlobal(ptBuf);
                    ptBuf = Marshal.AllocHGlobal(BytesWritten);
                    if (UnsafeNativeMethods.GetPrinter(hPrinter, 3, ptBuf, BytesWritten.ToInt32(), out BytesWritten))
                    {
                        Marshal.PtrToStructure(ptBuf, this);
                    }
                    else
                    {
                        throw new Win32Exception();
                    }
                    // \\ Free this buffer again
                    Marshal.FreeHGlobal(ptBuf);
                }
                else
                {
                    throw new Win32Exception();
                }
            }
            else
            {
                throw new Win32Exception();
            }

        }

        public PRINTER_INFO_3()
        {

        }

        #endregion

    }

    #endregion

    #region PRINTER_NOTIFY_OPTIONS_TYPE STRUCTURE
    [StructLayout(LayoutKind.Sequential)]
    struct PRINTER_NOTIFY_OPTIONS_TYPE
    {
        public short wType;
        public short wReserved0;
        public int dwReserved1;
        public int dwReserved2;
        public int Count;
        public IntPtr pFields;  // \\ Pointer to an array of (Count * PRINTER_NOTIFY_INFO_DATA) 
    }
    #endregion

    #region PRINTER_NOTIFY_INFO_DATA_DATA
    [StructLayout(LayoutKind.Sequential)]
    struct PRINTER_NOTIFY_INFO_DATA_DATA
    {
        public int cbBuf;
        public IntPtr pBuf;
    }
    #endregion

    #region PRINTER_NOTIFY_INFO_DATA_UNION
    [StructLayout(LayoutKind.Explicit)]
    struct PRINTER_NOTIFY_INFO_DATA_UNION
    {
        [FieldOffset(0)]
        private uint adwData0;
        [FieldOffset(4)]
        private uint adwData1;
        [FieldOffset(0)]
        public PRINTER_NOTIFY_INFO_DATA_DATA Data;
        public uint[] adwData
        {
            get
            {
                return new uint[] { adwData0, adwData1 };
            }
        }
    }
    #endregion

    #region PRINTER_NOTIFY_INFO_DATA
    [StructLayout(LayoutKind.Sequential)]
    internal struct PRINTER_NOTIFY_INFO_DATA
    {
        public ushort wType;
        public ushort wField;
        public uint dwReserved;
        public uint dwId;
    }
    #endregion


    #region PRINTER_NOTIFY_INFO STRUCTURE
    [StructLayout(LayoutKind.Sequential)]
    public class PRINTER_NOTIFY_INFO
    {
        public int Version;
        public int Flags;
        public int Count;
    }
    #endregion

    #region DRIVER_INFO_2
    [StructLayout(LayoutKind.Sequential)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    internal class DRIVER_INFO_2
    {
        [MarshalAs(UnmanagedType.U4)]
        public PrinterDriverOperatingSystemVersion cVersion;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pEnvironment;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDriverPath;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDatafile;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pConfigFile;


        #region Public constructors
        public DRIVER_INFO_2(IntPtr hPrinter)
        {

            var BytesWritten = IntPtr.Zero;
            IntPtr ptBuf;

            ptBuf = Marshal.AllocHGlobal(1);

            if (!UnsafeNativeMethods.GetPrinterDriver(hPrinter, null, 2, ptBuf, new IntPtr(1), out BytesWritten))
            {
                if (BytesWritten.ToInt64() > 0L)
                {
                    // \\ Free the buffer allocated
                    Marshal.FreeHGlobal(ptBuf);
                    ptBuf = Marshal.AllocHGlobal(BytesWritten);
                    if (UnsafeNativeMethods.GetPrinterDriver(hPrinter, null, 2, ptBuf, BytesWritten, out BytesWritten))
                    {
                        Marshal.PtrToStructure(ptBuf, this);
                    }
                    else
                    {
                        throw new Win32Exception();
                    }
                    // \\ Free this buffer again
                    Marshal.FreeHGlobal(ptBuf);
                }
                else
                {
                    throw new Win32Exception();
                }
            }
            else
            {
                throw new Win32Exception();
            }

        }

        public DRIVER_INFO_2()
        {

        }
        #endregion

    }
    #endregion

    #region DRIVER_INFO_3
    [StructLayout(LayoutKind.Sequential)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    internal class DRIVER_INFO_3
    {
        [MarshalAs(UnmanagedType.U4)]
        public PrinterDriverOperatingSystemVersion cVersion;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pEnvironment;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDriverPath;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDatafile;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pConfigFile;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pHelpFile;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDependentFiles;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pMonitorName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDefaultDataType;

        #region Public constructors
        public DRIVER_INFO_3(IntPtr hPrinter)
        {

            var BytesWritten = IntPtr.Zero;
            IntPtr ptBuf;

            ptBuf = Marshal.AllocHGlobal(1);

            if (!UnsafeNativeMethods.GetPrinterDriver(hPrinter, null, 3, ptBuf, new IntPtr(1), out BytesWritten))
            {
                if (BytesWritten.ToInt64() > 0L)
                {
                    // \\ Free the buffer allocated
                    Marshal.FreeHGlobal(ptBuf);
                    ptBuf = Marshal.AllocHGlobal(BytesWritten);
                    if (UnsafeNativeMethods.GetPrinterDriver(hPrinter, null, 3, ptBuf, BytesWritten, out BytesWritten))
                    {
                        Marshal.PtrToStructure(ptBuf, this);
                    }
                    else
                    {
                        throw new Win32Exception();
                    }
                    // \\ Free this buffer again
                    Marshal.FreeHGlobal(ptBuf);
                }
                else
                {
                    throw new Win32Exception();
                }
            }
            else
            {
                throw new Win32Exception();
            }

        }

        public DRIVER_INFO_3()
        {

        }
        #endregion

    }
    #endregion

    #region MONITOR_INFO_2
    [StructLayout(LayoutKind.Sequential)]
    internal class MONITOR_INFO_2
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pEnvironment;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDLLName;

        #region Public constructors
        public MONITOR_INFO_2(int lpMonitorInfo2)
        {
            Marshal.PtrToStructure(new IntPtr(lpMonitorInfo2), this);
        }

        public MONITOR_INFO_2()
        {

        }
        #endregion

    }
    #endregion

    #region PRINTPROCESSOR_INFO_1
    [StructLayout(LayoutKind.Sequential)]
    internal class PRINTPROCESSOR_INFO_1
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pName;
    }
    #endregion

    #region DATATYPES_INFO_1
    [StructLayout(LayoutKind.Sequential)]
    internal class DATATYPES_INFO_1
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pName;
    }
    #endregion

    #region PORT_INFO_1
    [StructLayout(LayoutKind.Sequential)]
    internal class PORT_INFO_1
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pPortName;
    }
    #endregion

    #region PORT_INFO_2
    [StructLayout(LayoutKind.Sequential)]
    internal class PORT_INFO_2
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pPortName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pMonitorName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDescription;
        [MarshalAs(UnmanagedType.U4)]
        public PortTypes PortType;
        [MarshalAs(UnmanagedType.U4)]
        public int Reserved;

    }
    #endregion

    #region PORT_INFO_3
    [StructLayout(LayoutKind.Sequential)]
    internal class PORT_INFO_3
    {
        [MarshalAs(UnmanagedType.U4)]
        public int dwStatus;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszStatus;
        [MarshalAs(UnmanagedType.U4)]
        public int dwSeverity;

    }
    #endregion

    #region FORM_INFO_1
    [StructLayout(LayoutKind.Sequential)]
    internal class FORM_INFO_1
    {
        [MarshalAs(UnmanagedType.U4)]
        public int Flags; // FormTypeFlags
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Name;
        [MarshalAs(UnmanagedType.U4)]
        public int Width;
        [MarshalAs(UnmanagedType.U4)]
        public int Height;
        [MarshalAs(UnmanagedType.U4)]
        public int Left;
        [MarshalAs(UnmanagedType.U4)]
        public int Top;
        [MarshalAs(UnmanagedType.U4)]
        public int Right;
        [MarshalAs(UnmanagedType.U4)]
        public int Bottom;

    }

    #endregion

    #region DOC_INFO_1
    [StructLayout(LayoutKind.Sequential)]
    internal class DOC_INFO_1
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string DocumentName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string OutputFilename;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string DataType;
    }
    #endregion

}