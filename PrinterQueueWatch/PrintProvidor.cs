using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
// \\ --[PrintProvidor]--------------------------------------
// \\ Class wrapper for the top level "Print Providor"
// \\ (c) Merrion Computing Ltd 
// \\     http://www.merrioncomputing.com
// \\ ----------------------------------------------------
using PrinterQueueWatch.SpoolerStructs;

namespace PrinterQueueWatch
{

    /// Project	 : PrinterQueueWatch
/// Class	 : PrintProvidor
/// 
/// <summary>
/// Class representing the properties of a print provider on this domain
/// </summary>
/// <remarks>
/// </remarks>
/// <example>Lists all  the printer providors visible from this process
/// <code>
/// 
///        Me.ListBox1.Items.Clear()
///        For ps As Integer = 0 To server.PrintProvidors.Count - 1
///            Me.ListBox1.Items.Add( server.PrintProvidors(ps).Name )
///        Next
/// 
/// </code>
/// </example>
/// <history>
/// 	[Duncan]	21/11/2005	Created
/// </history>
    [ComVisible(false)]
    public class PrintProvidor
    {

        #region Private properties
        private PRINTER_INFO_1 _pi1 = new PRINTER_INFO_1();
        #endregion

        #region Public interface

        #region Name
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The name of the print provider
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// e.g. "Windows NT Local provider" (for local printers),
    /// "Windows NT Remote provider" (for network printers) etc.
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public string Name
        {
            get
            {
                return _pi1.pName;
            }
        }
        #endregion

        #region Description
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The description of the print provider
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// e.g. "Windows NT Local Printers" etc.
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public string Description
        {
            get
            {
                return _pi1.pDescription;
            }
        }
        #endregion

        #region Comment
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The comment text (if any) associated with this provider
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public string Comment
        {
            get
            {
                return _pi1.pComment;
            }
        }
        #endregion

        #region PrintDomains
        /// <summary>
    /// The print domains serviced by this print provider
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
        public PrintDomainCollection PrintDomains
        {
            get
            {
                return new PrintDomainCollection(Name);
            }
        }
        #endregion

        #endregion

        #region Public constructor
        internal PrintProvidor(string Name, string Description, string Comment, int Flags)
        {
            {
                ref var withBlock = ref _pi1;
                withBlock.pName = Name;
                withBlock.pDescription = Description;
                withBlock.pComment = Comment;
                withBlock.Flags = Flags;
            }
        }
        #endregion

    }

    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : PrintProvidorCollection
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// The collection of print providors accessible from this machine
/// </summary>
/// <remarks>
/// </remarks>
/// <example>Lists all the printer providors visible from this process
/// <code>
///       Dim server As New PrintServer
/// 
///        Me.ListBox1.Items.Clear()
///        For ps As Integer = 0 To server.Providors.Count - 1 
///            Me.ListBox1.Items.Add( server.Providors(ps).Name )
///        Next
/// 
/// </code>
/// </example>
/// <history>
/// 	[Duncan]	21/11/2005	Created
/// </history>
/// -----------------------------------------------------------------------------
    [System.Security.SuppressUnmanagedCodeSecurity()]
    [ComVisible(false)]
    public class PrintProvidorCollection : System.Collections.Generic.List<PrintProvidor>
    {

        #region Public interface
        public new PrintProvidor this[int index]
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

        public new void Remove(PrintProvidor obj)
        {
            base.Remove(obj);
        }
        #endregion

        #region Public constructor
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Creates a collection of all the print providors accessible from this machine
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    ///     [Duncan]    01/05/2014  Use IntPtr for 32/64 bit compatibility
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrintProvidorCollection()
        {
            // \\ Return all the print providors visible from this machine
            int pcbNeeded; // \\ Holds the requires size of the output buffer (in bytes)
            int pcReturned; // \\ Holds the returned size of the output buffer 
            IntPtr pProvidors;
            int pcbProvided;

            // If Not EnumPrinters(0,string.Empty  1, 0, 0, pcbNeeded, pcReturned) Then
            if (!UnsafeNativeMethods.EnumPrinters(SpoolerApiConstantEnumerations.EnumPrinterFlags.PRINTER_ENUM_NAME, 0, 1, pProvidors, 0, out pcbNeeded, out pcReturned))
            {
                // \\ Allocate the required buffer to get all the monitors into...
                if (pcbNeeded > 0)
                {
                    pProvidors = Marshal.AllocHGlobal(pcbNeeded);
                    pcbProvided = pcbNeeded;
                    if (!UnsafeNativeMethods.EnumPrinters(SpoolerApiConstantEnumerations.EnumPrinterFlags.PRINTER_ENUM_NAME, 0, 1, pProvidors, pcbProvided, out pcbNeeded, out pcReturned))
                    {
                        throw new Win32Exception();
                    }
                }
            }

            if (pcReturned > 0)
            {
                // \\ Get all the monitors for the given server
                var ptNext = pProvidors;
                while (pcReturned > 0)
                {
                    var pi1 = new PRINTER_INFO_1();
                    Marshal.PtrToStructure(ptNext, pi1);
                    Add(new PrintProvidor(pi1.pName, pi1.pDescription, pi1.pComment, pi1.Flags));
                    ptNext = ptNext + Marshal.SizeOf(pi1);
                    pcReturned -= 1;
                }
            }

            // \\ Free the allocated buffer memory
            if (pProvidors.ToInt64() > 0L)
            {
                Marshal.FreeHGlobal(pProvidors);
            }

        }
        #endregion

    }
}