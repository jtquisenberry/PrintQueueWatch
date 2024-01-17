using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
// \\ --[PrintServer]----------------------------------------
// \\ Class wrapper for the "port" related API 
// \\ (c) Merrion Computing Ltd 
// \\     http://www.merrioncomputing.com
// \\ ----------------------------------------------------
using PrinterQueueWatch.SpoolerStructs;

namespace PrinterQueueWatch
{

    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : Port
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// Represents information about the port to which a printer is attached
/// </summary>
/// <remarks>
/// </remarks>
/// <example>Lists all the ports on the current print server
/// <code>
///        Dim server As New PrintServer
/// 
///        ListBox1.Items.Clear()
///        For ps As Integer = 0 To server.Ports.Count - 1
///            Me.ListBox1.Items.Add(server.Ports(ps).Name)
///        Next
/// </code>
/// </example>
    [ComVisible(false)]
    public class Port
    {

        #region Private properties
        private string _Servername;
        private PORT_INFO_2 _pi2;
        #endregion

        #region Public interface

        #region Server Name
        /// <summary>
    /// The name of the server on which this port resides
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
        public string Servername
        {
            get
            {
                return _Servername;
            }
        }
        #endregion

        #region Name
        /// <summary>
    /// The name supported printer port (for example, "LPT1:").
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
        public string Name
        {
            get
            {
                return _pi2.pPortName;
            }
        }
        #endregion

        #region Monitor Name
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Identifies an installed monitor (for example, "PJL monitor"). 
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This may be empty if no port monitor associated with this port
    /// </remarks>
        public string MonitorName
        {
            get
            {
                return _pi2.pMonitorName;
            }
        }
        #endregion

        #region Description
        /// <summary>
    /// More detailed description of this printer port
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
        public string Description
        {
            get
            {
                return _pi2.pDescription;
            }
        }
        #endregion

        #region PortTypes related

        #region Read
        /// <summary>
    /// The port supports read functionality
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
        public bool Read
        {
            get
            {
                return (_pi2.PortType & SpoolerApiConstantEnumerations.PortTypes.PORT_TYPE_READ) == SpoolerApiConstantEnumerations.PortTypes.PORT_TYPE_READ;
            }
        }
        #endregion

        #region Write
        /// <summary>
    /// The port supports write operation
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
        public bool Write
        {
            get
            {
                return (_pi2.PortType & SpoolerApiConstantEnumerations.PortTypes.PORT_TYPE_WRITE) == SpoolerApiConstantEnumerations.PortTypes.PORT_TYPE_WRITE;
            }
        }
        #endregion

        #region Redirected
        /// <summary>
    /// True if the port is redirected
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
        public bool Redirected
        {
            get
            {
                return (_pi2.PortType & SpoolerApiConstantEnumerations.PortTypes.PORT_TYPE_REDIRECTED) == SpoolerApiConstantEnumerations.PortTypes.PORT_TYPE_REDIRECTED;
            }
        }
        #endregion

        #region NetAttached
        /// <summary>
    /// True if the port is network attached
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
        public bool NetAttached
        {
            get
            {
                return (_pi2.PortType & SpoolerApiConstantEnumerations.PortTypes.PORT_TYPE_NET_ATTACHED) == SpoolerApiConstantEnumerations.PortTypes.PORT_TYPE_NET_ATTACHED;
            }
        }
        #endregion

        #endregion


        #endregion

        #region public constructor
        internal Port(string Servername, PORT_INFO_2 pi2In)
        {
            _Servername = Servername;
            _pi2 = pi2In;
        }
        #endregion

    }

    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : PortCollection
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// A collection of Port objects
/// </summary>
/// <remarks>
/// </remarks>
/// <example>Lists all the ports on the current printer
/// <code>
///        Dim server As New PrintServer
/// 
///        ListBox1.Items.Clear()
///        For ps As Integer = 0 To server.Ports.Count - 1
///            Me.ListBox1.Items.Add(server.Ports(ps).Name)
///        Next
/// </code>
/// </example>
/// <history>
/// 	[Duncan]	19/11/2005	Created
/// </history>
/// -----------------------------------------------------------------------------
    [System.Security.SuppressUnmanagedCodeSecurity()]
    [ComVisible(false)]
    public class PortCollection : System.Collections.Generic.List<Port>
    {

        #region Public interface
        public new Port this[int index]
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

        public new void Remove(Port obj)
        {
            base.Remove(obj);
        }
        #endregion

        #region Public constructors
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Returns the ports on the current machine
    /// </summary>
    /// <remarks>
    /// To get the ports of another machine use the overloaded constructor and
    /// pass the server name as a reference
    /// </remarks>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PortCollection()
        {
            int pcbNeeded; // \\ Holds the requires size of the output buffer (in bytes)
            int pcReturned; // \\ Holds the returned size of the output buffer 
            var pPorts = default(int);
            int pcbProvided;

            if (!UnsafeNativeMethods.EnumPorts(0, 2, 0, 0, out pcbNeeded, out pcReturned))
            {
                // \\ Allocate the required buffer to get all the monitors into...
                if (pcbNeeded > 0)
                {
                    pPorts = (int)Marshal.AllocHGlobal(pcbNeeded);
                    pcbProvided = pcbNeeded;
                    if (!UnsafeNativeMethods.EnumPorts(0, 2, pPorts, pcbProvided, out pcbNeeded, out pcReturned))
                    {
                        throw new Win32Exception();
                    }
                }
            }

            if (pcReturned > 0)
            {
                // \\ Get all the monitors for the given server
                int ptNext = pPorts;
                while (pcReturned > 0)
                {
                    var mi2 = new PORT_INFO_2();
                    Marshal.PtrToStructure(new IntPtr(ptNext), mi2);
                    Add(new Port("", mi2));
                    ptNext = ptNext + Marshal.SizeOf(mi2);
                    pcReturned -= 1;
                }
            }

            // \\ Free the allocated buffer memory
            if (pPorts > 0)
            {
                Marshal.FreeHGlobal((IntPtr)pPorts);
            }

        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Enumerates the ports on the named server
    /// </summary>
    /// <param name="Servername">The server to list the ports on</param>
    /// <remarks>
    /// </remarks>
    /// <exception cref="System.ComponentModel.Win32Exception">
    /// The server does not exist of the user does not have access to it
    /// </exception>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PortCollection(string Servername)
        {

            int pcbNeeded; // \\ Holds the requires size of the output buffer (in bytes)
            int pcReturned; // \\ Holds the returned size of the output buffer 
            var pPorts = default(int);
            int pcbProvided;

            if (!UnsafeNativeMethods.EnumPorts(Servername, 2, 0, 0, out pcbNeeded, out pcReturned))
            {
                if (pcbNeeded > 0)
                {
                    pPorts = (int)Marshal.AllocHGlobal(pcbNeeded);
                    pcbProvided = pcbNeeded;
                    if (!UnsafeNativeMethods.EnumPorts(Servername, 2, pPorts, pcbProvided, out pcbNeeded, out pcReturned))
                    {
                        throw new Win32Exception();
                    }
                }
            }

            if (pcReturned > 0)
            {
                // \\ Get all the monitors for the given server
                int ptNext = pPorts;
                while (pcReturned > 0)
                {
                    var mi2 = new PORT_INFO_2();
                    Marshal.PtrToStructure(new IntPtr(ptNext), mi2);
                    Add(new Port(Servername, mi2));
                    ptNext = ptNext + Marshal.SizeOf(mi2);
                    pcReturned -= 1;
                }
            }

            // \\ Free the allocated buffer memory
            if (pPorts > 0)
            {
                Marshal.FreeHGlobal((IntPtr)pPorts);
            }

        }
        #endregion

    }
}