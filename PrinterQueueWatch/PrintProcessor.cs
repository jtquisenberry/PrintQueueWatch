using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
// \\ --[PrintProcessor]--------------------------------
// \\ Class wrapper for the windows API calls and constants
// \\ relating to the printer processors ..
// \\ (c) Merrion Computing Ltd 
// \\     http://www.merrioncomputing.com
// \\ ----------------------------------------------------
using PrinterQueueWatch.SpoolerStructs;

namespace PrinterQueueWatch
{

    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : PrintProcessor
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// Class wrapper for the properties relating to the printer processors ..
/// </summary>
/// <remarks>
/// </remarks>
/// <example>
/// <code>
///        Dim server As New PrintServer
/// 
///        Dim Processor As PrintProcessor
///        Me.ListBox1.Items.Clear()
///        For ps As Integer = 0 To server.PrintProcessors.Count - 1
///            Me.ListBox1.Items.Add( server.PrintProcessors(ps).Name )
///        Next
/// </code>
/// </example>
/// <history>
/// 	[Duncan]	21/11/2005	Created
/// </history>
/// -----------------------------------------------------------------------------
    [ComVisible(false)]
    public class PrintProcessor
    {

        #region Private properties
        private string _ServerName;
        private PRINTPROCESSOR_INFO_1 _PPInfo1;
        #endregion

        #region Public interface

        #region Name
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The name of the print server
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
                return _PPInfo1.pName;
            }
        }
        #endregion

        #region Server
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The server name on which the print processor is installed
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public string ServerName
        {
            get
            {
                return _ServerName;
            }
        }
        #endregion

        #region Data Types
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The collection of data types that this print processor can process
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public DataTypeCollection DataTypes
        {
            get
            {
                if (_ServerName is null || string.IsNullOrEmpty(_ServerName))
                {
                    return new DataTypeCollection(_PPInfo1.pName);
                }
                else
                {
                    return new DataTypeCollection(_ServerName, _PPInfo1.pName);
                }
            }
        }
        #endregion

        #endregion

        #region Public constructors
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ServerName">The name of the server that this processor is installed on</param>
    /// <param name="PrintProcessorname">The name of this print processor</param>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrintProcessor(string ServerName, string PrintProcessorname)
        {
            _ServerName = ServerName;
            _PPInfo1 = new PRINTPROCESSOR_INFO_1();
            _PPInfo1.pName = PrintProcessorname;
        }
        #endregion

    }

    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : PrintProcessorCollection
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// The collection of print processors installed on a print server
/// </summary>
/// <remarks>
/// </remarks>
/// <example>
/// <code>
///        Dim server As New PrintServer
/// 
///        Me.ListBox1.Items.Clear()
///        For ps As Integer = 0 To server.PrintProcessors.Count - 1
///            Me.ListBox1.Items.Add( server.PrintProcessors(ps).Name )
///        Next
/// </code>
/// </example>
/// <history>
/// 	[Duncan]	21/11/2005	Created
/// </history>
/// -----------------------------------------------------------------------------
    [System.Security.SuppressUnmanagedCodeSecurity()]
    [ComVisible(false)]
    public class PrintProcessorCollection : System.Collections.Generic.List<PrintProcessor>
    {

        #region Public interface
        public new PrintProcessor this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                base[index] = value;
            }
        }

        internal new void Remove(PrintProcessor obj)
        {
            base.Remove(obj);
        }
        #endregion

        #region Public constructors
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Creates a collection of print processors installed on the current print server
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrintProcessorCollection()
        {
            int pcbNeeded; // \\ Holds the requires size of the output buffer (in bytes)
            int pcReturned; // \\ Holds the returned size of the output buffer 
            var pPrintProcessors = default(int);
            int pcbProvided = 0;


            if (!UnsafeNativeMethods.EnumPrintProcessors(string.Empty, string.Empty, 1, pPrintProcessors, 0, out pcbNeeded, out pcReturned))
            {
                // \\ Allocate the required buffer to get all the monitors into...
                if (pcbNeeded > 0)
                {
                    pPrintProcessors = (int)Marshal.AllocHGlobal(pcbNeeded);
                    pcbProvided = pcbNeeded;
                    if (!UnsafeNativeMethods.EnumPrintProcessors(string.Empty, string.Empty, 1, pPrintProcessors, pcbProvided, out pcbNeeded, out pcReturned))
                    {
                        throw new Win32Exception();
                    }
                }
            }

            if (pcReturned > 0)
            {
                // \\ Get all the monitors for the given server
                int ptNext = pPrintProcessors;
                while (pcReturned > 0)
                {
                    var pi1 = new PRINTPROCESSOR_INFO_1();
                    Marshal.PtrToStructure(new IntPtr(ptNext), pi1);
                    Add(new PrintProcessor("", pi1.pName));
                    ptNext = ptNext + Marshal.SizeOf(pi1);
                    pcReturned -= 1;
                }
            }

            // \\ Free the allocated buffer memory
            if (pPrintProcessors > 0)
            {
                Marshal.FreeHGlobal((IntPtr)pPrintProcessors);
            }


        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Creates a collection of print processors installed on the named print server
    /// </summary>
    /// <param name="Servername">The name of the print server to return the print processors for</param>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrintProcessorCollection(string Servername)
        {
            int pcbNeeded; // \\ Holds the requires size of the output buffer (in bytes)
            int pcReturned; // \\ Holds the returned size of the output buffer 
            var pPrintProcessors = default(int);
            int pcbProvided;

            if (!UnsafeNativeMethods.EnumPrintProcessors(Servername, string.Empty, 1, 0, 0, out pcbNeeded, out pcReturned))
            {
                // \\ Allocate the required buffer to get all the monitors into...
                if (pcbNeeded > 0)
                {
                    pPrintProcessors = (int)Marshal.AllocHGlobal(pcbNeeded);
                    pcbProvided = pcbNeeded;
                    if (!UnsafeNativeMethods.EnumPrintProcessors(Servername, string.Empty, 2, pPrintProcessors, pcbProvided, out pcbNeeded, out pcReturned))
                    {
                        throw new Win32Exception();
                    }
                }
            }

            if (pcReturned > 0)
            {
                // \\ Get all the monitors for the given server
                int ptNext = pPrintProcessors;
                while (pcReturned > 0)
                {
                    var pi1 = new PRINTPROCESSOR_INFO_1();
                    Marshal.PtrToStructure(new IntPtr(ptNext), pi1);
                    Add(new PrintProcessor("", pi1.pName));
                    ptNext = ptNext + Marshal.SizeOf(pi1);
                    pcReturned -= 1;
                }
            }

            // \\ Free the allocated buffer memory
            if (pPrintProcessors > 0)
            {
                Marshal.FreeHGlobal((IntPtr)pPrintProcessors);
            }
        }
        #endregion
    }
}