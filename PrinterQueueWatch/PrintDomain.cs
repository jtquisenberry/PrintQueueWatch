using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
// \\ --[PrintProvidor]--------------------------------------
// \\ Class wrapper for the top level "Print Domain"
// \\ (c) Merrion Computing Ltd 
// \\     http://www.merrioncomputing.com
// \\ ----------------------------------------------------
using PrinterQueueWatch.SpoolerStructs;

namespace PrinterQueueWatch
{

    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : PrintDomain
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// </summary>
/// <remarks>
/// </remarks>
/// <example>Lists all the print domains on all the printer providors visible from this process
/// <code>
///       Dim Providors As New PrintProvidorCollection
/// 
///        Me.ListBox1.Items.Clear()
///        For ps As Integer = 0 To Providors.Count - 1
///            Me.ListBox1.Items.Add( Providors(ps).Name )
///            For ds As Integer = 0 To  Providors(ps).PrintDomains.Count - 1
///                Me.ListBox1.Items.Add( Providors(ps).PrintDomains(ds).Name )
///            Next
///        Next
/// 
/// </code>
/// </example>
    [ComVisible(false)]
    public class PrintDomain
    {

        #region Private properties
        private PRINTER_INFO_1 _pi1 = new PRINTER_INFO_1();
        private string _ProvidorName;
        #endregion

        #region Public interface

        #region Name
        /// <summary>
    /// The name of the print domain
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	17/02/2006	Created
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
        /// <summary>
    /// The description of the print domain
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public string Description
        {
            get
            {
                return _pi1.pDescription;
            }
        }
        #endregion

        #region Comment
        /// <summary>
    /// The comment associated with this print domain
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public string Comment
        {
            get
            {
                return _pi1.pComment;
            }
        }
        #endregion

        #endregion

        #region Public constructor
        internal PrintDomain(string ProvidorName, string Name, string Description, string Comment, int Flags)
        {

            _ProvidorName = ProvidorName;
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
/// Class	 : PrintDomainCollection
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// A collection of PrintDomain objects for a given print providor
/// </summary>
/// <remarks>
/// </remarks>
/// <example>Lists all the print domains on all the printer providors visible from this process
/// <code>
///       Dim Providors As New PrintProvidorCollection
/// 
///        Me.ListBox1.Items.Clear()
///        For ps As Integer = 0 To Providors.Count - 1
///            Me.ListBox1.Items.Add( Providors(ps).Name )
///            For ds As Integer = 0 To  Providors(ps).PrintDomains.Count - 1
///                Me.ListBox1.Items.Add( Providors(ps).PrintDomains(ds).Name )
///            Next
///        Next
/// 
/// </code>
/// </example>
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public class PrintDomainCollection : System.Collections.Generic.List<PrintDomain>
    {

        #region Public interface
        /// <summary>
    /// The Item property returns a single <see cref="PrintDomain">print domain</see> from a print domain collection.
    /// </summary>
    /// <param name="index">The zero-based item position</param>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public new PrintDomain this[int index]
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

        /// <summary>
    /// Removes a print domain from this collection
    /// </summary>
    /// <param name="obj"></param>
    /// <remarks></remarks>
        internal new void Remove(PrintDomain obj)
        {
            base.Remove(obj);
        }
        #endregion

        #region Public constructor
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Creates a list of all the print domains visible from this machine for the named providor
    /// </summary>
    /// <param name="ProvidorName">The name of the PrintProvidor to get the list of print domains for</param>
    /// <remarks>
    /// </remarks>
        public PrintDomainCollection(string ProvidorName)
        {
            // \\ Return all the print providors visible from this machine
            int pcbNeeded; // \\ Holds the requires size of the output buffer (in bytes)
            int pcReturned; // \\ Holds the returned size of the output buffer 
            IntPtr pDomains;
            int pcbProvided;

            if (!UnsafeNativeMethods.EnumPrinters(SpoolerApiConstantEnumerations.EnumPrinterFlags.PRINTER_ENUM_NAME, ProvidorName, 1, pDomains, 0, out pcbNeeded, out pcReturned))
            {
                // \\ Allocate the required buffer to get all the monitors into...
                if (pcbNeeded > 0)
                {
                    pDomains = Marshal.AllocHGlobal(pcbNeeded);
                    pcbProvided = pcbNeeded;
                    if (!UnsafeNativeMethods.EnumPrinters(SpoolerApiConstantEnumerations.EnumPrinterFlags.PRINTER_ENUM_NAME, ProvidorName, 1, pDomains, pcbProvided, out pcbNeeded, out pcReturned))
                    {
                        throw new Win32Exception();
                    }
                }
            }

            if (pcReturned > 0)
            {
                // \\ Get all the domains for the given print providor
                var ptNext = pDomains;
                while (pcReturned > 0)
                {
                    var pi1 = new PRINTER_INFO_1();
                    Marshal.PtrToStructure(ptNext, pi1);
                    Add(new PrintDomain(ProvidorName, pi1.pName, pi1.pDescription, pi1.pComment, pi1.Flags));
                    ptNext = ptNext + Marshal.SizeOf(pi1);
                    pcReturned -= 1;
                }
            }

            // \\ Free the allocated buffer memory
            if (pDomains.ToInt64() > 0L)
            {
                Marshal.FreeHGlobal(pDomains);
            }

        }
        #endregion

    }
}