using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
// \\ --[DataType]--------------------------------
// \\ Class wrapper for the windows API calls and constants
// \\ relating to the printer data types ..
// \\ (c) Merrion Computing Ltd 
// \\     http://www.merrioncomputing.com
// \\ ----------------------------------------------------
using PrinterQueueWatch.SpoolerStructs;

namespace PrinterQueueWatch
{

    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : DataType
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// The data type of a spool file
/// </summary>
/// <remarks>
/// This is the data type that the spool file contains.  It can be EMF or RAW.
/// </remarks>
/// <example>Lists all the data types supported by each of the print processors 
/// installed on the current server
/// <code>
///        Dim server As New PrintServer
///        Dim Processor As PrintProcessor
///        Me.ListBox1.Items.Clear()
///        For ps As Integer = 0 To server.PrintProcessors.Count - 1
///            ListBox1.Items.Add( server.PrintProcessors(ps).Name )
///            For dt As Integer = 0 to server.PrintProcessors(ps).DataTypes.Count - 1
///                Me.ListBox1.Items.Add(server.PrintProcessors(ps).DataTypes(dt).Name)
///            Next
///        Next
/// </code>
/// </example>
/// <history>
/// 	[Duncan]	19/11/2005	Created
/// </history>
/// -----------------------------------------------------------------------------
    [ComVisible(false)]
    public class DataType
    {

        #region Private members
        private DATATYPES_INFO_1 _dti1 = new DATATYPES_INFO_1();
        #endregion

        #region Public interface
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The name of the data type of the spool file
    /// </summary>
    /// <remarks>
    /// If this value is RAW then the spool file contains a printer control language
    /// (such as PostScript, PCL-5, PCL-XL etc.)
    /// </remarks>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public string Name
        {
            get
            {
                return _dti1.pName;
            }
        }
        #endregion

        #region Public constructor
        public DataType(string Name)
        {
            _dti1.pName = Name;
        }
        #endregion

    }

    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : DataTypeCollection
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// The collection of <see cref="DataType">data types</see> supported by a print processor 
/// </summary>
/// <remarks>
/// </remarks>
/// <history>
/// 	[Duncan]	19/11/2005	Created
/// </history>
/// -----------------------------------------------------------------------------
    [System.Security.SuppressUnmanagedCodeSecurity()]
    [ComVisible(false)]
    public class DataTypeCollection : System.Collections.Generic.List<DataType>
    {

        #region Public interface
        /// <summary>
    /// Returns a single <see cref="DataType">data type</see> from the collection
    /// </summary>
    /// <param name="index">The zero-based position in the collection</param>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public new DataType this[int index]
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
    /// Removes the <see cref="DataType">data type</see> from this collection
    /// </summary>
    /// <param name="obj"></param>
    /// <remarks></remarks>
        internal new void Remove(DataType obj)
        {
            base.Remove(obj);
        }
        #endregion

        #region Public constructors
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Creates a new collection containing all the data types
    /// supported by the named print processor
    /// </summary>
    /// <param name="PrintProcessorName">The name of the print processor to retrieve the supported data types for</param>
    /// <remarks>
    /// The print processor must be installed on the local machine
    /// <example >
    /// Prints the name of all the data types supported by the <see cref="PrintProcessor">print processor</see> named WinPrint
    /// <code>
    /// Dim DataTypes As New DataTypeCollection("Winprint")
    /// For Each dt As DataType In DataTypes
    ///    Trace.WriteLine(dt.Name)
    /// Next dt
    /// </code>
    /// </example>
    /// </remarks>
    /// 
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public DataTypeCollection(string PrintProcessorName)
        {
            int pcbNeeded; // \\ Holds the requires size of the output buffer (in bytes)
            int pcReturned; // \\ Holds the returned size of the output buffer 
            var pDataTypes = default(int);
            int pcbProvided;

            if (!UnsafeNativeMethods.EnumPrinterProcessorDataTypes(string.Empty, PrintProcessorName, 1, pDataTypes, 0, out pcbNeeded, out pcReturned))
            {
                if (pcbNeeded > 0)
                {
                    pDataTypes = (int)Marshal.AllocHGlobal(pcbNeeded);
                    pcbProvided = pcbNeeded;
                    if (!UnsafeNativeMethods.EnumPrinterProcessorDataTypes(string.Empty, PrintProcessorName, 1, pDataTypes, pcbProvided, out pcbNeeded, out pcReturned))
                    {
                        throw new Win32Exception();
                    }
                }
            }

            if (pcReturned > 0)
            {
                // \\ Get all the monitors for the given server
                int ptNext = pDataTypes;
                while (pcReturned > 0)
                {
                    var dti1 = new DATATYPES_INFO_1();
                    Marshal.PtrToStructure(new IntPtr(ptNext), dti1);
                    Add(new DataType(dti1.pName));
                    ptNext = ptNext + Marshal.SizeOf(dti1);
                    pcReturned -= 1;
                }
            }

            // \\ Free the allocated buffer memory
            if (pDataTypes > 0)
            {
                Marshal.FreeHGlobal((IntPtr)pDataTypes);
            }

        }

        /// <summary>
    /// Creates a new collection containing all the data types
    /// supported by the named print processor on the named server
    /// </summary>
    /// <param name="Servername">The server on which the print processor resides</param>
    /// <param name="PrintProcessorName">The print processor name</param>
    /// <remarks>
    /// <example >
    /// Prints the name of all the data types supported by the <see cref="PrintProcessor">print processor</see> named WinPrint
    /// <code>
    /// Dim DataTypes As New DataTypeCollection("DUBPDOM1","Winprint")
    /// For Each dt As DataType In DataTypes
    ///    Trace.WriteLine(dt.Name)
    /// Next dt
    /// </code>
    /// </example>
    /// </remarks>
        public DataTypeCollection(string Servername, string PrintProcessorName)
        {
            int pcbNeeded; // \\ Holds the requires size of the output buffer (in bytes)
            int pcReturned; // \\ Holds the returned size of the output buffer 
            var pDataTypes = default(int);
            int pcbProvided;

            if (UnsafeNativeMethods.EnumPrinterProcessorDataTypes(Servername, PrintProcessorName, 1, pDataTypes, 0, out pcbNeeded, out pcReturned))
            {
                if (pcbNeeded > 0)
                {
                    pDataTypes = (int)Marshal.AllocHGlobal(pcbNeeded);
                    pcbProvided = pcbNeeded;
                    if (!UnsafeNativeMethods.EnumPrinterProcessorDataTypes(Servername, PrintProcessorName, 1, pDataTypes, pcbProvided, out pcbNeeded, out pcReturned))
                    {
                        throw new Win32Exception();
                    }
                }
            }

            if (pcReturned > 0)
            {
                // \\ Get all the monitors for the given server
                int ptNext = pDataTypes;
                while (pcReturned > 0)
                {
                    var dti1 = new DATATYPES_INFO_1();
                    Marshal.PtrToStructure(new IntPtr(ptNext), dti1);
                    Add(new DataType(dti1.pName));
                    ptNext = ptNext + Marshal.SizeOf(dti1);
                    pcReturned -= 1;
                }
            }

            // \\ Free the allocated buffer memory
            if (pDataTypes > 0)
            {
                Marshal.FreeHGlobal((IntPtr)pDataTypes);
            }
        }
        #endregion
    }
}