using System;
using System.ComponentModel;
using PrinterQueueWatch.SpoolerApiConstantEnumerations;

namespace PrinterQueueWatch
{

    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : PrinterEventArgs
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// Class representing the event arguments used in the change events 
/// relating to the printer ..
/// </summary>
/// <remarks>
/// This is passed as an argument in the 
/// PrinterInformationChanged event of the PrinterMonitorComponent
/// </remarks>
/// <example>Prints the user name of a printer if an error occurs
/// <code>
///   Private WithEvents pmon As New PrinterMonitorComponent
/// 
///   pmon.AddPrinter("Microsoft Office Document Image Writer")
///   pmon.AddPrinter("HP Laserjet 5")
/// 
///     Private Sub pmon_PrinterInformationChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.PrinterInformationChanged
///        With CType(e, PrinterEventArgs)
///            If .PrinterInformation.IsInError Then
///                Trace.WriteLine(.PrinterInformation.PrinterName)
///            End If
///        End With
///    End Sub
/// </code>
/// </example>
    public class PrinterEventArgs : EventArgs
    {

        #region Private member variables
        private PrinterInformation mPrinterInfo;
        private DateTime mEventTime;
        private PrinterInfoChangeFlagDecoder mPrinterChangeFlags;
        #endregion

        #region Public Properties
        #region PrinterInformation
        /// <summary>
    /// The PrinterInformation class 
    /// that represents the settings of the printer for which the event was triggered
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This holds the printer information as it was when the event occured
    /// </remarks>
        public PrinterInformation PrinterInformation
        {
            get
            {
                return mPrinterInfo;
            }
        }
        #endregion

        #region EventTime
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The date and time at which this printer information changed event was triggered
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
        public DateTime EventTime
        {
            get
            {
                return mEventTime;
            }
        }
        #endregion

        #region PrinterChangeFlags
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The PrinterInfoChangeFlagDecoder class that holds details of what printer settings changed to trigger this 
    /// printer change event
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
        public PrinterInfoChangeFlagDecoder PrinterChangeFlags
        {
            get
            {
                return mPrinterChangeFlags;
            }
        }
        #endregion

        public bool Equals(PrinterEventArgs PrinterEventArgs)
        {
            if (PrinterEventArgs is not null)
            {
                if (PrinterEventArgs.PrinterChangeFlags.ChangeFlags == mPrinterChangeFlags.ChangeFlags)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return default;
        }
        #endregion

        #region Public Constructors
        internal PrinterEventArgs(int dwFlags, PrinterInformation PrinterInfo, DateTime time)
        {
            mPrinterInfo = PrinterInfo;
            mPrinterChangeFlags = new PrinterInfoChangeFlagDecoder(dwFlags);
            mEventTime = time;
        }

        internal PrinterEventArgs(int dwFlags, PrinterInformation PrinterInfo) : this(dwFlags, PrinterInfo, DateTime.Now)
        {
        }

        #endregion

    }

    // \\ --[PrinterInfoChangeFlagDecoder]-------------------------------
    // \\ Splits the printer change flags up into components to allow 
    // \\ developers to respond differently depending on the nature of
    // \\ the printer change event
    // \\ ---------------------------------------------------------------
    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : PrinterInfoChangeFlagDecoder
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// This class holds details of what printer settings changed to trigger this 
/// printer change event
/// </summary>
/// <remarks>
/// A single printer change event may have more than one cause 
/// </remarks>
    public class PrinterInfoChangeFlagDecoder
    {

        #region Private member variables
        private int _mdwFlags;
        #endregion

        #region Public interface

        /// <summary>
    /// True if the number of jobs on the print queue changed
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The new value will be held in the JobCount member of the PrinterInformation passed with the event
    /// </remarks>
        public bool JobCountChanged
        {
            get
            {
                return (_mdwFlags & (long)Math.Round(Math.Pow(2d, (double)Printer_Notify_Field_Indexes.PRINTER_NOTIFY_FIELD_CJOBS))) != 0L;
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the printer status has changed
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The new value will be held in the status fields of the 
    /// PrinterInformation passed with the event
    /// </remarks>
        public bool StatusChanged
        {
            get
            {
                return (_mdwFlags & (long)Math.Round(Math.Pow(2d, (double)Printer_Notify_Field_Indexes.PRINTER_NOTIFY_FIELD_STATUS))) != 0L;
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The printer attributes have changed
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The new value will be held in the attributes fields of the 
    /// PrinterInformation passed with the event
    /// </remarks>
        [Description("The printer attributes have changed")]
        public bool AttributesChanged
        {
            get
            {
                return (_mdwFlags & (long)Math.Round(Math.Pow(2d, (double)Printer_Notify_Field_Indexes.PRINTER_NOTIFY_FIELD_ATTRIBUTES))) != 0L;
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The comment text associated with this printer has been changed
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The new value will be held in the Comment
    /// member of the PrinterInformation passed with the event
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Description("The comment text has changed")]
        public bool CommentChanged
        {
            get
            {
                return (_mdwFlags & (long)Math.Round(Math.Pow(2d, (double)Printer_Notify_Field_Indexes.PRINTER_NOTIFY_FIELD_COMMENT))) != 0L;
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The default device mode settings of the printer have changed
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The new value will be held in the default device mode related fields of the 
    /// PrinterInformation passed with the event
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Description("The default DEVMODE has changed")]
        public bool DeviceModeChanged
        {
            get
            {
                return (_mdwFlags & (long)Math.Round(Math.Pow(2d, (double)Printer_Notify_Field_Indexes.PRINTER_NOTIFY_FIELD_DEVMODE))) != 0L;
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The printer location text has been changed
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The new value will be held in the Location
    /// member of the PrinterInformation passed with the event
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Description("The printer location text has changed")]
        public bool LocationChanged
        {
            get
            {
                return (_mdwFlags & (long)Math.Round(Math.Pow(2d, (double)Printer_Notify_Field_Indexes.PRINTER_NOTIFY_FIELD_LOCATION))) != 0L;
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The discretionary access control settings for the printer has changed
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Description("The discretionary access control for the printer has changed")]
        public bool SecurityChanged
        {
            get
            {
                return (_mdwFlags & (long)Math.Round(Math.Pow(2d, (double)Printer_Notify_Field_Indexes.PRINTER_NOTIFY_FIELD_SECURITY_DESCRIPTOR))) != 0L;
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The file used as a job seperator was changed
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The new value will be held in the SeperatorFilename
    /// member of the PrinterInformation passed with the event.
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Description("The file used as a job seperator was changed")]
        public bool SeperatorFileChanged
        {
            get
            {
                return (_mdwFlags & (long)Math.Round(Math.Pow(2d, (double)Printer_Notify_Field_Indexes.PRINTER_NOTIFY_FIELD_SEPFILE))) != 0L;
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Unknown change - insufficient change information was provided by the printer
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This will be true if the printer driver does not issue details of why a printer change 
    /// event occurs
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Description("Unknown change - insufficient change information was provided by the printer")]
        public bool UnknownChange
        {
            get
            {
                return _mdwFlags == 0;
            }
        }

        internal int ChangeFlags
        {
            get
            {
                return _mdwFlags;
            }
        }

        #endregion

        #region Public constructors
        internal PrinterInfoChangeFlagDecoder(int dwFlags)
        {
            _mdwFlags = dwFlags;
        }
        #endregion

    }
}