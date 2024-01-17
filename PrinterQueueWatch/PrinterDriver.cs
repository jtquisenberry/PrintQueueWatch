using System;
// \\ --[PrinterDriver]--------------------------------
// \\ Class wrapper for the windows API calls and constants
// \\ relating to the printer drivers ..
// \\ (c) Merrion Computing Ltd 
// \\     http://www.merrioncomputing.com
// \\ ----------------------------------------------------
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using PrinterQueueWatch.SpoolerApiConstantEnumerations;
using PrinterQueueWatch.SpoolerStructs;

namespace PrinterQueueWatch
{

    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : PrinterDriver
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// Specifies the settings for a printer driver
/// </summary>
/// <remarks>
/// </remarks>
/// <example>Lists all the printer drivers on the current server
/// <code>
///        Dim server As New PrintServer
/// 
///        Dim Driver As PrinterDriver
///        Me.ListBox1.Items.Clear()
///        For Each Driver In server.PrinterDrivers
///            Me.ListBox1.Items.Add(Driver.Name)
///        Next
/// </code>
/// </example>
/// <seealso cref="PrinterDriver" />
    [ComVisible(false)]
    public class PrinterDriver
    {

        #region Private members
        private DRIVER_INFO_3 _Driver_Info_3;
        #endregion

        #region Shared methods

        #region AddPrinterDriver
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Installs a printer driver on the named server
    /// </summary>
    /// <param name="Servername">The server to install the driver on</param>
    /// <param name="OperatingSystemVersion">The operating system version that the driver targets</param>
    /// <param name="DriverName">The name of the driver</param>
    /// <param name="Environment">The environment for which the driver was written (for example, "Windows NT x86", "Windows NT R4000", "Windows NT Alpha_AXP", or "Windows 4.0")</param>
    /// <param name="DriverFile">The driver program file</param>
    /// <param name="DriverDataFile">The file which contains the driver data</param>
    /// <param name="DriverConfigFile">file name or a full path and file name for the device-driver's configuration .dll</param>
    /// <returns></returns>
    /// <remarks>
    /// Before an application calls the AddPrinterDriver function, all files 
    /// required by the driver must be copied to the system's printer-driver directory.
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public static PrinterDriver AddPrinterDriver(string Servername, PrinterDriverOperatingSystemVersion OperatingSystemVersion, string DriverName, string Environment, FileInfo DriverFile, FileInfo DriverDataFile, FileInfo DriverConfigFile)
        {

            // \\ Validate input data
            if (!DriverFile.Exists)
            {
                throw new FileNotFoundException("File not found: " + DriverFile.FullName);
                return default;
            }

            // \\ Make a DRIVER_INFO_2 with the parameters passed in
            var di2 = new DRIVER_INFO_2();
            di2.cVersion = OperatingSystemVersion;
            di2.pName = DriverName;
            di2.pDriverPath = DriverFile.FullName;
            if (DriverConfigFile.Exists)
            {
                di2.pConfigFile = DriverConfigFile.FullName;
            }
            if (DriverDataFile.Exists)
            {
                di2.pDatafile = DriverDataFile.FullName;
            }
            di2.pEnvironment = Environment;

            // \\ Call the AddPrinterDriver API call
            if (UnsafeNativeMethods.AddPrinterDriver(Servername, 2, di2))
            {
                return new PrinterDriver(di2);
            }
            else
            {
                throw new Win32Exception();
            }

        }
        #endregion

        #region GetPrinterDriverDirectory
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Returns the directory on the current machine in which the printer drivers and
    /// their support files are kept
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public static DirectoryInfo GetPrinterDriverDirectory()
        {
            return GetPrinterDriverDirectory(string.Empty, string.Empty);
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Returns the directory on the named machine in which the printer drivers and
    /// their support files are kept
    /// </summary>
    /// <param name="Servername">The name of the machine to get the directory for</param>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public static DirectoryInfo GetPrinterDriverDirectory(string Servername)
        {
            return GetPrinterDriverDirectory(Servername, string.Empty);
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Returns the directory on the named machine in which the printer drivers and
    /// their support files are kept for the named environment
    /// </summary>
    /// <param name="Servername">The name of the machine to get the directory for</param>
    /// <param name="Environment">The environment for which the driver was written (for example, "Windows NT x86", "Windows NT R4000", "Windows NT Alpha_AXP", or "Windows 4.0")</param>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public static DirectoryInfo GetPrinterDriverDirectory(string Servername, string Environment)
        {

            string DriverDirectory = "";
            int BytesNeeded;

            if (UnsafeNativeMethods.GetPrinterDriverDirectory(Servername, Environment, 1, out DriverDirectory, DriverDirectory.Length, out BytesNeeded))
            {
                // \\ Empty string should not return any values...
                return new DirectoryInfo(DriverDirectory);
            }
            else
            {
                DriverDirectory = new string(char.Parse(" "), BytesNeeded);
                if (UnsafeNativeMethods.GetPrinterDriverDirectory(Servername, Environment, 1, out DriverDirectory, DriverDirectory.Length, out BytesNeeded))
                {
                    // \\ Empty string should not return any values...
                    return new DirectoryInfo(DriverDirectory);
                }
                else
                {
                    throw new Win32Exception();
                }
            }

        }
        #endregion

        #endregion

        #region Public interface

        #region OperatingSystemVersion
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The operating system for which this driver was written
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
        public PrinterDriverOperatingSystemVersion OperatingSystemVersion
        {
            get
            {
                if (_Driver_Info_3 is not null)
                {
                    return _Driver_Info_3.cVersion;
                }

                return default;
            }
        }
        #endregion

        #region Name
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The unique name by which this printer driver is known
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public string Name
        {
            get
            {
                if (_Driver_Info_3 is not null)
                {
                    return _Driver_Info_3.pName;
                }
                else
                {
                    return "";
                }
            }
        }
        #endregion

        #region Environment
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The environment for which the driver was written 
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// For example, "Windows NT x86", "Windows NT R4000", "Windows NT Alpha_AXP", or "Windows 4.0"
    /// </remarks>
        public string Environment
        {
            get
            {
                if (_Driver_Info_3 is not null)
                {
                    return _Driver_Info_3.pEnvironment;
                }
                else
                {
                    return "";
                }
            }
        }
        #endregion

        #region DriverPath
        /// <summary>
    /// The file name or full path and file name for the file that contains the device driver 
    /// </summary>
    /// <value></value>
    /// <remarks>"
    /// For example, "c:\drivers\pscript.dll"
    /// This value will be relative to the server
    /// </remarks>
        public string DriverPath
        {
            get
            {
                if (_Driver_Info_3 is not null)
                {
                    return _Driver_Info_3.pDriverPath;
                }
                else
                {
                    return "";
                }
            }
        }
        #endregion

        #region DataFile
        /// <summary>
    /// The file name or a full path and file name for the file that contains driver data
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// For example, "c:\drivers\Qms810.ppd" 
    /// </remarks>
        public string DataFile
        {
            get
            {
                if (_Driver_Info_3 is not null)
                {
                    return _Driver_Info_3.pDatafile;
                }
                else
                {
                    return "";
                }
            }
        }
        #endregion

        #region ConfigurationFile
        /// <summary>
    /// The file name or a full path and file name for the device-driver's configuration .dll
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// These configuration files provide the user interface for the extra 
    /// </remarks>
        public string ConfigurationFile
        {
            get
            {
                if (_Driver_Info_3 is not null)
                {
                    return _Driver_Info_3.pConfigFile;
                }
                else
                {
                    return "";
                }
            }
        }
        #endregion

        #region HelpFile
        /// <summary>
    /// The file name or a full path and file name for the device driver's help file.
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This member may be blank if the driver has no help file
    /// </remarks>
        public string HelpFile
        {
            get
            {
                if (_Driver_Info_3 is not null)
                {
                    return _Driver_Info_3.pHelpFile;
                }
                else
                {
                    return "";
                }
            }
        }
        #endregion

        #region MonitorName
        /// <summary>
    /// The name of a language monitor attached to this driver (for example, "PJL monitor")
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This member can be empty and is specified only for printers capable of bidirectional communication
    /// </remarks>
        public string MonitorName
        {
            get
            {
                if (_Driver_Info_3 is not null)
                {
                    return _Driver_Info_3.pMonitorName;
                }
                else
                {
                    return "";
                }
            }
        }
        #endregion

        #region DefaultDataType
        /// <summary>
    /// The default data type used by this printer driver in writing spool files for print jobs
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This can be EMF or RAW.  The latter indicates that a printer control language 
    /// (such as PCL or PostScript) is used.
    /// </remarks>
        public string DefaultDataType
        {
            get
            {
                if (_Driver_Info_3 is not null)
                {
                    return _Driver_Info_3.pDefaultDataType;
                }
                else
                {
                    return "";
                }
            }
        }
        #endregion

        #endregion

        #region Public constructors
        internal PrinterDriver(IntPtr hPrinter)
        {
            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("New(" + hPrinter.ToString() + ")", GetType().ToString());
            }
            _Driver_Info_3 = new DRIVER_INFO_3(hPrinter);
        }

        internal PrinterDriver(DRIVER_INFO_3 dInfo3)
        {
            _Driver_Info_3 = dInfo3;
        }

        internal PrinterDriver(DRIVER_INFO_2 dInfo2)
        {
            {
                ref var withBlock = ref _Driver_Info_3;
                withBlock.cVersion = dInfo2.cVersion;
                withBlock.pConfigFile = dInfo2.pConfigFile;
                withBlock.pDatafile = dInfo2.pDatafile;
                withBlock.pDriverPath = dInfo2.pDriverPath;
                withBlock.pEnvironment = dInfo2.pEnvironment;
                withBlock.pName = dInfo2.pName;
            }
        }
        #endregion

    }

    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : PrinterDriverCollection
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// The collection of printer drivers on a server
/// </summary>
/// <remarks>
/// </remarks>
/// <example>Lists all the printer drivers on the current server
/// <code>
///        Dim server As New PrintServer
/// 
///        Dim Driver As PrinterDriver
///        Me.ListBox1.Items.Clear()
///        For Each Driver In server.PrinterDrivers
///            Me.ListBox1.Items.Add(Driver.Name)
///        Next
/// </code>
/// </example>
    [ComVisible(false)]
    public class PrinterDriverCollection : System.Collections.Generic.List<PrinterDriver>
    {

        #region Public interface
        /// <summary>
    /// The Item property returns a single <see cref="PrinterDriver">printer driver</see> from this collection.
    /// </summary>
    /// <param name="index"></param>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public new PrinterDriver this[int index]
        {
            get
            {
                return base[index];
            }
            internal set
            {
                base[index] = value;
            }
        }

        internal new void Remove(PrinterDriver obj)
        {
            base.Remove(obj);
        }
        #endregion

        #region Public constructors

        public PrinterDriverCollection()
        {
            int pcbNeeded; // \\ Holds the requires size of the output buffer (in bytes)
            int pcReturned; // \\ Holds the returned size of the output buffer (in bytes)
            IntPtr pDriverInfo;
            int nItem;
            IntPtr pNextDriverInfo;

            if (!UnsafeNativeMethods.EnumPrinterDrivers(string.Empty, string.Empty, 3, pDriverInfo, 0, out pcbNeeded, out pcReturned))
            {
                if (pcbNeeded > 0)
                {
                    pDriverInfo = Marshal.AllocHGlobal(pcbNeeded);
                    if (UnsafeNativeMethods.EnumPrinterDrivers(string.Empty, string.Empty, 3, pDriverInfo, pcbNeeded, out pcbNeeded, out pcReturned))
                    {
                        if (pcReturned > 0)
                        {
                            pNextDriverInfo = pDriverInfo;
                            var loopTo = pcReturned;
                            for (nItem = 1; nItem <= loopTo; nItem++)
                            {
                                var pdInfo3 = new DRIVER_INFO_3();
                                // \\ Read the DRIVER_INFO_3 from the buffer
                                Marshal.PtrToStructure(pNextDriverInfo, pdInfo3);
                                // \\ Add this to the return list
                                Add(new PrinterDriver(pdInfo3));
                                // \\ Move the buffer pointer on to the next DRIVER_INFO_3 structure
                                pNextDriverInfo = pNextDriverInfo + Marshal.SizeOf(pdInfo3);
                            }
                        }
                    }
                    Marshal.FreeHGlobal(pDriverInfo);
                }
            }

        }

        public PrinterDriverCollection(string Servername)
        {

            int pcbNeeded; // \\ Holds the requires size of the output buffer (in bytes)
            int pcReturned; // \\ Holds the returned size of the output buffer (in bytes)
            IntPtr pDriverInfo;
            int nItem;
            IntPtr pNextDriverInfo;

            if (!UnsafeNativeMethods.EnumPrinterDrivers(Servername, string.Empty, 3, pDriverInfo, 0, out pcbNeeded, out pcReturned))
            {
                if (pcbNeeded > 0)
                {
                    pDriverInfo = Marshal.AllocHGlobal(pcbNeeded);
                    if (UnsafeNativeMethods.EnumPrinterDrivers(Servername, string.Empty, 3, pDriverInfo, pcbNeeded, out pcbNeeded, out pcReturned))
                    {
                        if (pcReturned > 0)
                        {
                            pNextDriverInfo = pDriverInfo;
                            var loopTo = pcReturned;
                            for (nItem = 1; nItem <= loopTo; nItem++)
                            {
                                var pdInfo3 = new DRIVER_INFO_3();
                                // \\ Read the DRIVER_INFO_3 from the buffer
                                Marshal.PtrToStructure(pNextDriverInfo, pdInfo3);
                                // \\ Add this to the return list
                                Add(new PrinterDriver(pdInfo3));
                                // \\ Move the buffer pointer on to the next DRIVER_INFO_3 structure
                                pNextDriverInfo = pNextDriverInfo + Marshal.SizeOf(pdInfo3);
                            }
                        }
                    }
                    Marshal.FreeHGlobal(pDriverInfo);
                }
            }

        }
        #endregion

    }
}