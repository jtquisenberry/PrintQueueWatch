using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
// \\ --[PrinterForm]----------------------------------------
// \\ Class wrapper for the "form" related API 
// \\ (c) Merrion Computing Ltd 
// \\     http://www.merrioncomputing.com
// \\ ----------------------------------------------------
using PrinterQueueWatch.SpoolerStructs;

namespace PrinterQueueWatch
{

    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : PrinterForm
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// Represents a form that has been installed on a printer
/// </summary>
/// <remarks>
/// </remarks>
/// <example>List the print forms on the named printer
/// <code>
///        Dim pi As New PrinterInformation("Microsoft Office Document Image Writer", SpoolerApiConstantEnumerations.PrinterAccessRights.PRINTER_ALL_ACCESS, True)
/// 
///        For pf As Integer = 0 to pi.PrinterForms.Count -1
///            pi.PrinterForms(pf).Name
///        Next
/// </code>
/// </example>
/// <history>
/// 	[Duncan]	19/11/2005	Created
/// </history>
/// -----------------------------------------------------------------------------
    [ComVisible(false)]
    public class PrinterForm
    {

        #region Private properties
        private IntPtr _hPrinter;
        private FORM_INFO_1 _fi1 = new FORM_INFO_1();
        #endregion

        #region Public interface

        #region Name
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The name of the printer form 
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public string Name
        {
            get
            {
                return _fi1.Name;
            }
        }
        #endregion

        #region Width
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The width of the form
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This value is measured in millimeters
    /// </remarks>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public int Width
        {
            get
            {
                return _fi1.Width;
            }
            set
            {
                if (value != _fi1.Width)
                {
                    _fi1.Width = value;
                    SaveForm();
                }
            }
        }
        #endregion

        #region Height
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The height of the printer form
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This value is measured in millimeters
    /// </remarks>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public int Height
        {
            get
            {
                return _fi1.Height;
            }
            set
            {
                if (value != _fi1.Height)
                {
                    _fi1.Height = value;
                    SaveForm();
                }
            }
        }
        #endregion

        #region ImageableArea
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Specifies the width and height, in thousandths of millimeters, of the form. 
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This may be smaller than the height and width if there is a non-printable margin
    /// on the form
    /// </remarks>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public Rectangle ImageableArea
        {
            get
            {
                return new Rectangle(_fi1.Left, _fi1.Top, _fi1.Right - _fi1.Left, _fi1.Bottom - _fi1.Top);
            }
            set
            {
                _fi1.Left = value.Left;
                _fi1.Top = value.Top;
                _fi1.Right = value.Width + value.Left;
                _fi1.Bottom = value.Top + value.Height;
                SaveForm();
            }
        }
        #endregion

        #region Flags related
        public bool IsBuiltInForm
        {
            get
            {
                return _fi1.Flags == (int)SpoolerApiConstantEnumerations.FormTypeFlags.FORM_BUILTIN;
            }
        }

        public bool IsUserForm
        {
            get
            {
                return _fi1.Flags == (int)SpoolerApiConstantEnumerations.FormTypeFlags.FORM_USER;
            }
        }

        public bool IsPrinterForm
        {
            get
            {
                return _fi1.Flags == (int)SpoolerApiConstantEnumerations.FormTypeFlags.FORM_PRINTER;
            }
        }
        #endregion

        #endregion

        #region Public constructor
        internal PrinterForm(IntPtr hPrinter, int Flags, string Name, int Width, int Height, int Left, int Top, int Right, int Bottom)







        {

            _hPrinter = hPrinter;

            {
                ref var withBlock = ref _fi1;
                withBlock.Name = Name;
                withBlock.Flags = Flags;
                withBlock.Bottom = Bottom;
                withBlock.Top = Top;
                withBlock.Left = Left;
                withBlock.Right = Right;
                withBlock.Height = Height;
                withBlock.Width = Width;
            }

        }

        public PrinterForm(string Name)
        {

        }
        #endregion

        #region Private methods
        private void SaveForm()
        {
            if (!UnsafeNativeMethods.SetForm(_hPrinter, _fi1.Name, 1, ref _fi1))
            {
                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                {
                    Trace.WriteLine("SetForm call failed", GetType().ToString());
                }
                throw new Win32Exception();
            }
        }
        #endregion

    }

    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : PrinterFormCollection
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// A collection of PrintForm objects supported by a printer
/// </summary>
/// <remarks>
/// </remarks>
/// <example>List the print forms on the named printer
/// <code>
///        Dim pi As New PrinterInformation("Microsoft Office Document Image Writer", SpoolerApiConstantEnumerations.PrinterAccessRights.PRINTER_ALL_ACCESS, True)
/// 
///        For pf As Integer = 0 To In pi.PrinterForms.Count - 1
///            Me.ListBox1.Items.Add( pi.PrinterForms(pf).Name )
///        Next
/// </code>
/// </example>
/// <seealso cref="PrinterQueueWatch.PrinterInformation.PrinterForms" />
/// <history>
/// 	[Duncan]	19/11/2005	Created
///     [Duncan]    01/05/2014  Use IntPtr for 32/64 bit compatibility
/// </history>
/// -----------------------------------------------------------------------------
    [ComVisible(false)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public class PrinterFormCollection : System.Collections.Generic.List<PrinterForm>
    {

        #region Public interface
        public new PrinterForm this[int index]
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

        public new void Remove(PrinterForm obj)
        {
            base.Remove(obj);
        }
        #endregion

        #region Public constructors

        internal PrinterFormCollection(IntPtr hPrinter)
        {

            IntPtr pForm = IntPtr.Zero;
            int pcbNeeded;
            int pcFormsReturned;
            int pcbProvided;

            if (!UnsafeNativeMethods.EnumForms(hPrinter, 1, pForm, 0, out pcbNeeded, out pcFormsReturned))
            {
                if (pcbNeeded > 0)
                {
                    pForm = Marshal.AllocHGlobal(pcbNeeded);
                    pcbProvided = pcbNeeded;
                    if (!UnsafeNativeMethods.EnumForms(hPrinter, 1, pForm, pcbProvided, out pcbNeeded, out pcFormsReturned))
                    {
                        throw new Win32Exception();
                    }
                }
            }

            if (pcFormsReturned > 0)
            {
                // \\ Get all the monitors for the given server
                var ptNext = pForm;
                while (pcFormsReturned > 0)
                {
                    var fi1 = new FORM_INFO_1();
                    Marshal.PtrToStructure(ptNext, fi1);
                    Add(new PrinterForm(hPrinter, fi1.Flags, fi1.Name, fi1.Width, fi1.Height, fi1.Left, fi1.Top, fi1.Right, fi1.Bottom));
                    ptNext = ptNext + Marshal.SizeOf(fi1);
                    pcFormsReturned -= 1;
                }
            }

            // \\ Free the allocated buffer memory
            if (pForm.ToInt64() > 0L)
            {
                Marshal.FreeHGlobal(pForm);
            }

        }
        #endregion

    }
}