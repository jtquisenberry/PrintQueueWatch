using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
// \\ --[PrintMonitor]----------------------------------------
// \\ (c) Merrion Computing Ltd 
// \\     http://www.merrioncomputing.com
// \\ --------------------------------------------------------
using PrinterQueueWatch.SpoolerStructs;

namespace PrinterQueueWatch
{

    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : PrintMonitor
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// Class wrapper for the windows API calls and constants relating to print monitors
/// </summary>
/// <remarks>
/// </remarks>
/// <example>Lists teh details of all the print monitors on the current server
/// <code>
///        Dim server As New PrintServer
/// 
///        Me.ListBox1.Items.Clear()
///        For ms As Integer = 0 To server.PrintMonitors.Count - 1
///            Me.ListBox1.Items.Add( server.PrintMonitors(ms).Name )
///        Next
/// </code>
/// </example>
/// <history>
/// 	[Duncan]	21/11/2005	Created
/// </history>
/// -----------------------------------------------------------------------------
    [ComVisible(false)]
    public class PrintMonitor
    {

        #region Private members
        private string _Name;
        private string _Environment;
        private string _DLLName;
        #endregion

        #region Public properties
        #region Name
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The name of the print monitor
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public string Name
        {
            get
            {
                return _Name;
            }
        }
        #endregion

        #region Environment
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The environment for which the print monitor was created
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// For example, "Windows NT x86", "Windows NT R4000", "Windows NT Alpha_AXP", or "Windows 4.0"
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public string Environment
        {
            get
            {
                return _Environment;
            }
        }
        #endregion

        #region DLLName
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The dynamic link library that implements this print monitor
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public string DLLName
        {
            get
            {
                return _DLLName;
            }
        }
        #endregion
        #endregion

        #region Public constructors
        internal PrintMonitor(string Name, string Environment, string DLLName)
        {
            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("New(" + Name + "," + Environment + "," + DLLName + ")", GetType().ToString());
            }
            _Name = Name;
            _Environment = Environment;
            _DLLName = DLLName;
        }
        #endregion

        #region ToString override
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Text description of this print monitor instance
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public override string ToString()
        {
            var sOut = new System.Text.StringBuilder();
            sOut.Append(_Name + ",");
            sOut.Append(_Environment + ",");
            sOut.Append(_DLLName + ",");
            sOut.Append(GetType().ToString());
            return sOut.ToString();
        }
        #endregion

    }

    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : PrintMonitors
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// A collection of print monitors on a server
/// </summary>
/// <remarks>
/// </remarks>
/// <example>Lists the details of all the print monitors on the current server
/// <code>
///        Dim server As New PrintServer
/// 
///        Me.ListBox1.Items.Clear()
///        For ms As Integer = 0 To server.PrintMonitors.Count - 1
///            Me.ListBox1.Items.Add( server.PrintMonitors(ms).Name )
///        Next
/// </code>
/// </example>
/// <history>
/// 	[Duncan]	21/11/2005	Created
/// </history>
/// -----------------------------------------------------------------------------
    [ComVisible(false)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public class PrintMonitors : System.Collections.Generic.List<PrintMonitor>
    {

        #region Public constructors
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Gets a collection of print monitors on the current server
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrintMonitors()
        {

            int pcbNeeded; // \\ Holds the requires size of the output buffer (in bytes)
            int pcReturned; // \\ Holds the returned size of the output buffer 
            var pMonitors = default(int);
            int pcbProvided;

            if (!UnsafeNativeMethods.EnumMonitors(0, 2, 0, 0, out pcbNeeded, out pcReturned))
            {
                // \\ Allocate the required buffer to get all the monitors into...
                if (pcbNeeded > 0)
                {
                    pMonitors = (int)Marshal.AllocHGlobal(pcbNeeded);
                    pcbProvided = pcbNeeded;
                    if (!UnsafeNativeMethods.EnumMonitors(0, 2, pMonitors, pcbProvided, out pcbNeeded, out pcReturned))
                    {
                        throw new Win32Exception();
                    }
                }
            }

            if (pcReturned > 0)
            {
                // \\ Get all the monitors for the given server
                int ptNext = pMonitors;
                while (pcReturned > 0)
                {
                    var mi2 = new MONITOR_INFO_2();
                    Marshal.PtrToStructure(new IntPtr(ptNext), mi2);
                    Add(new PrintMonitor(mi2.pName, mi2.pEnvironment, mi2.pDLLName));
                    ptNext = ptNext + Marshal.SizeOf(mi2);
                    pcReturned -= 1;
                }
            }

            // \\ Free the allocated buffer memory
            if (pMonitors > 0)
            {
                Marshal.FreeHGlobal((IntPtr)pMonitors);
            }

        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Gets a collection of print monitors on the named server
    /// </summary>
    /// <param name="Servername">The name of the server to get the print monitors from</param>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrintMonitors(string Servername)
        {

            int pcbNeeded; // \\ Holds the requires size of the output buffer (in bytes)
            int pcReturned; // \\ Holds the returned size of the output buffer 
            var pMonitors = default(int);
            int pcbProvided;

            if (!UnsafeNativeMethods.EnumMonitors(Servername, 2, 0, 0, out pcbNeeded, out pcReturned))
            {
                if (pcbNeeded > 0)
                {
                    pMonitors = (int)Marshal.AllocHGlobal(pcbNeeded);
                    pcbProvided = pcbNeeded;
                    if (!UnsafeNativeMethods.EnumMonitors(Servername, 2, pMonitors, pcbProvided, out pcbNeeded, out pcReturned))
                    {
                        throw new Win32Exception();
                    }
                }
            }

            if (pcReturned > 0)
            {
                // \\ Get all the monitors for the given server
                int ptNext = pMonitors;
                while (pcReturned > 0)
                {
                    var mi2 = new MONITOR_INFO_2();
                    Marshal.PtrToStructure(new IntPtr(ptNext), mi2);
                    Add(new PrintMonitor(mi2.pName, mi2.pEnvironment, mi2.pDLLName));
                    ptNext = ptNext + Marshal.SizeOf(mi2);
                    pcReturned -= 1;
                }
            }

            // \\ Free the allocated buffer memory
            if (pMonitors > 0)
            {
                Marshal.FreeHGlobal((IntPtr)pMonitors);
            }

        }
        #endregion

    }
}