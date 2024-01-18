using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
// \\ --[PrintJob]----------------------------------------
// \\ Class wrapper for the windows API calls and constants
// \\ relating to individual jobs in a printer queue..
// \\ (c) Merrion Computing Ltd 
// \\     http://www.merrioncomputing.com
// \\ ----------------------------------------------------
using System.Runtime.InteropServices;
using System.Text;
using PrinterQueueWatch.PrinterMonitoringExceptions;
using PrinterQueueWatch.SpoolerApiConstantEnumerations;

using PrinterQueueWatch.SpoolerStructs;

namespace PrinterQueueWatch
{


    #region PrintJob class

    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : PrintJob
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// Represents the properties of a single print job queued against a print device
/// </summary>
/// <remarks>
/// </remarks>
/// <history>
/// 	[Duncan]	20/11/2005	Created
/// </history>
/// -----------------------------------------------------------------------------
    [System.Security.SuppressUnmanagedCodeSecurity()]
    [ComVisible(false)]
    public class PrintJob : IDisposable
    {

        #region Tracing
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Print Job specific tracing switch
    /// </summary>
    /// <remarks>Add a trace flag named "PrintJob" in the application .config file to 
    /// trace print job related processes
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public static TraceSwitch TraceSwitch = new TraceSwitch("PrintJob", "Printer Monitor Component Print Job Tracing");
        #endregion

        #region Private member variables

        private IntPtr mhPrinter;
        private int midJob;

        private bool bHandleOwnedByMe;

        private string mPrinterName;
        private string mDocument;

        private JOB_INFO_1 ji1 = new JOB_INFO_1();
        private JOB_INFO_2 ji2 = new JOB_INFO_2();

        private TimeWindow _TimeWindow = new TimeWindow();

        private bool _PositionChanged;
        private bool _Changed_Ji1;
        private bool _Changed_Ji2;

        private string _UrlString;

        private bool _Populated = false;

        #endregion

        #region Public interface

        #region JobId
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The unique identifier of the print job
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This id is only unique for the printer
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public int JobId
        {
            get
            {
                return midJob;
            }
        }
        #endregion

        #region JOB_INFO_1 properties

        #region PrinterName
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The name of the print device that this job is queued against
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <example>Prints the name of the printer whenever a new job is added to the monitored printers
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs)
    ///        Trace.WriteLine(.PrintJob.PrinterName)
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual string PrinterName
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel1, false);
                if (ji1.pPrinterName is null)
                {
                    return "";
                }
                else
                {
                    return ji1.pPrinterName;
                }
            }
        }
        internal string InitPrinterName
        {
            set
            {
                ji1.pPrinterName = value;
            }
        }
        #endregion

        #region UserName
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The name of the user that sent the print job for printing
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <example>Prints the name of the user that originated the job whenever a new job is added to the monitored printers
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs)
    ///        Trace.WriteLine(.PrintJob.Username)
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual string UserName
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel1, false);
                if (ji1.pUserName is null)
                {
                    return "";
                }
                else
                {
                    return ji1.pUserName;
                }
            }
            set
            {
                ji1.pUserName = value;
                _Changed_Ji1 = true;
                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
                {
                    Trace.WriteLine("Username changed to " + value, GetType().ToString());
                }
            }
        }
        internal string InitUsername
        {
            set
            {
                ji1.pUserName = value;
            }
        }
        #endregion

        #region MachineName
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The name of the workstation that sent the print job to print
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <example>Prints the name of the machine that originated a job whenever a new job is added to the monitored printers
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs)
    ///        Trace.WriteLine(.PrintJob.MachineName)
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual string MachineName
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel1, false);
                if (ji1.pMachineName is null)
                {
                    return "";
                }
                else
                {
                    return ji1.pMachineName;
                }
            }
        }
        internal string InitMachineName
        {
            set
            {
                ji1.pMachineName = value;
            }
        }
        #endregion

        #region Document
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The document name being printed
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This value is set by the application which sends the job to be printed.  Many 
    /// applications put the application name at the start of the document name to aid 
    /// identification
    /// </remarks>
    /// <example>Prints the name of the documents in the print queue
    /// <code>
    ///    Dim pi As New PrinterInformation("Microsoft Office Document Image Writer", SpoolerApiConstantEnumerations.PrinterAccessRights.PRINTER_ALL_ACCESS, True)
    ///    For Each pj As PrintJob In pi.PrintJobs
    ///       Trace.WriteLine(pj.Document)
    ///    Next pj
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual string Document
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel1, false);
                if (ji1.pDocument is null)
                {
                    return "";
                }
                else
                {
                    return ji1.pDocument;
                }
            }
            set
            {
                ji1.pDocument = value;
                _Changed_Ji1 = true;
                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
                {
                    Trace.WriteLine("Document name changed to " + value, GetType().ToString());
                }
            }
        }
        internal string InitDocument
        {
            set
            {
                ji1.pDocument = value;
            }
        }
        #endregion

        #region StatusDescription
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The description of the current status of the print job
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <example>Prints the status of a job when it is added to the monitored printers
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs)
    ///        Trace.WriteLine(.PrintJob.StatusDescription)
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual string StatusDescription
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel1, false);
                if (ji1.pStatus is null || string.IsNullOrEmpty(ji1.pStatus))
                {
                    return DerivedStatusDescription();
                }
                else
                {
                    return ji1.pStatus;
                }
            }
        }
        internal string InitStatusDescription
        {
            set
            {
                ji1.pStatus = value;
            }
        }
        #endregion

        #region DataType
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The name of the data type that is used for this print job
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This can be RAW or EMF.  if the data type is RAW then the spool file contains 
    /// a printer control language such as PCL or PostScript
    /// </remarks>
    /// <example>Prints the data type of each job as it is added to the monitored printers
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs)
    ///        Trace.WriteLine(.PrintJob.DataType)
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual string DataType
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel1, false);
                if (ji1.pDatatype is null)
                {
                    return "";
                }
                else
                {
                    return ji1.pDatatype;
                }
            }
        }
        internal string InitDataType
        {
            set
            {
                ji1.pDatatype = value;
            }
        }
        #endregion

        #region Status (Private)
        private int Status
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel1, false);
                return ji1.Status;
            }
        }
        #endregion

        #region PagesPrinted
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The number of pages that have been printed
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <example>Prints the number of pages in each job as it is deleted
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///   Private Sub pmon_JobDeleted(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobDeleted
    /// 
    ///    With CType(e, PrintJobEventArgs)
    ///        Trace.WriteLine(.PrintJob.PagesPrinted.ToString)
    ///    End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual int PagesPrinted
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel1, false);
                return ji1.PagesPrinted;
            }
        }
        internal int InitPagesPrinted
        {
            set
            {
                ji1.PagesPrinted = value;
            }
        }
        #endregion

        #region Position
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The position of the job in the print device job queue
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <example>Prints the position in the queue of each new job as it is added to the monitored printers
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs)
    ///        Trace.WriteLine(.PrintJob.Position.ToString)
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual int Position
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel1, false);
                return ji1.Position;
            }
            set
            {
                if (value > 0)
                {
                    ji1.Position = value;
                    _PositionChanged = true;
                }
            }
        }
        internal int InitPosition
        {
            set
            {
                ji1.Position = value;
            }
        }
        #endregion

        #region Update
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Get the latest state of the print job from the print device spool queue
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public void Update()
        {
            RefreshJobInfo(JobInfoLevels.JobInfoLevel1, true);
            RefreshJobInfo(JobInfoLevels.JobInfoLevel2, true);
            _PositionChanged = false;
            _Changed_Ji1 = false;
            _Changed_Ji2 = false;
            _TimeWindow.Changed = false;
        }
        #endregion
        #region Commit
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Update the print spool with changes made to this print job class
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <example>Changes the user name for each new job as it is added to the monitored 
    /// printers and commits this to the print queue
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs)
    ///        .PrintJob.Username = "New user name"
    ///        .PrintJob.Commit
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public void Commit()
        {
            if (_Changed_Ji1 | _TimeWindow.Changed)
            {
                SaveJobInfo(JobInfoLevels.JobInfoLevel1);
                _Changed_Ji1 = false;
                _TimeWindow.Changed = false;
            }
            if (_Changed_Ji2 | _TimeWindow.Changed)
            {
                SaveJobInfo(JobInfoLevels.JobInfoLevel2);
                _Changed_Ji2 = false;
                _TimeWindow.Changed = false;
            }
        }
        #endregion

        #endregion

        #region JOB_INFO_2 derived properties

        #region TotalPages
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The total number of pages in this print job
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <example>Prints the number of pages in each new job as it is added to the monitored printers
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs)
    ///        Trace.WriteLine(.PrintJob.TotalPages.ToString)
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual int TotalPages
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel1, false);
                return ji2.TotalPage;
            }
        }
        internal int InitTotalPages
        {
            set
            {
                if (value > 0)
                {
                    ji2.TotalPage = value;
                }
            }
        }
        #endregion

        #region PaperKind
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The paper type that the job is intended to be printed on
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This could be a standard paper size (A4, A5 etc) or custom paper size if the printer 
    /// supports this
    /// </remarks>
    /// <example>Prints the paper type of each job as it is added to the monitored printers
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs)
    ///        Trace.WriteLine(.PrintJob.PaperKind.ToString)
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual PaperKind PaperKind
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel2, false);
                {
                    var withBlock = ji2.DeviceMode;
                    if (withBlock.dmPaperSize > 118 | withBlock.dmPaperSize < 0)
                    {
                        return PaperKind.Custom;
                    }
                    else
                    {
                        return (PaperKind)withBlock.dmPaperSize;
                    }
                }
            }
        }
        #endregion

        #region PaperWidth
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The width of the selected paper (if PaperKind is PaperKind.Custom)
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This value is measured in millimeters
    /// </remarks>
    /// <example>Prints the paper width of each job as it is added to the monitored printers
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs)
    ///        Trace.WriteLine(.PrintJob.PaperWidth.ToString)
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual int PaperWidth
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel2, false);
                return ji2.DeviceMode.dmPaperWidth;
            }
        }
        #endregion

        #region PaperLength
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The height of the selected paper (if PaperKind is PaperKind.Custom)
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This value is measured in millimeters
    /// </remarks>
    /// <example>Prints the paper length of each job as it is added to the monitored printers
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs)
    ///        Trace.WriteLine(.PrintJob.PaperLength.ToString)
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual int PaperLength
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel2, false);
                return ji2.DeviceMode.dmPaperLength;
            }
        }
        #endregion

        #region Landscape
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the print job is to be printed in landscape mode
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <example>Prints the landacape mode of each job as it is added to the monitored printers
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs)
    ///        Trace.WriteLine(IIf(.PrintJob.Landsape,"Landscape", "Portrait"))
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual bool Landscape
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel2, false);
                return ji2.DeviceMode.dmOrientation == (int)DeviceOrientations.DMORIENT_LANDSCAPE;
            }
        }
        #endregion

        #region Color
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the print job is to be printed in colour
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This will be true if the setting is set to rpint in colour even if
    /// the actual document has no colour elements 
    /// </remarks>
    /// <example>Prints the colour / monochrome setting of each job as it is added to the monitored printers
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs)
    ///        Trace.WriteLine(Iif(.PrintJob.Color,"Colour", "Monochrome"))
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual bool Color
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel2, false);
                return ji2.DeviceMode.dmColor == (int)DeviceColourModes.DMCOLOR_COLOR;
            }
        }
        #endregion

        #region PaperSource
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The input source (tray or bin) requested for the print job
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// 
    /// </remarks>
    /// <example>Prints the paper source of each job as it is added to the monitored printers
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs)
    ///        Trace.WriteLine(.PrintJob.PaperSource.ToString)
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual PaperSourceKind PaperSource
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel2, false);
                {
                    var withBlock = ji2.DeviceMode;
                    if (withBlock.dmDefaultSource >= (int)PrinterPaperBins.DMBIN_USER | withBlock.dmDefaultSource < 0)
                    {
                        return PaperSourceKind.Custom;
                    }
                    else
                    {
                        return (PaperSourceKind)ji2.DeviceMode.dmDefaultSource;
                    }
                }
            }
        }
        #endregion

        #region PrinterResolutionKind
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The resolution to use for the print document
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// Can be draft, low, medium or high quality or custom quality
    /// </remarks>
    /// <example>Prints the print resolution of each job as it is added to the monitored printers
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs)
    ///        Trace.WriteLine(.PrintJob.PrinterResolutionKind)
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual PrinterResolutionKind PrinterResolutionKind
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel2, false);
                if (ji2.DeviceMode.dmPrintQuality == (int)DeviceModeResolutions.DMRES_DRAFT)
                {
                    return PrinterResolutionKind.Draft;
                }
                else if (ji2.DeviceMode.dmPrintQuality == (int)DeviceModeResolutions.DMRES_HIGH)
                {
                    return PrinterResolutionKind.High;
                }
                else if (ji2.DeviceMode.dmPrintQuality == (int)DeviceModeResolutions.DMRES_LOW)
                {
                    return PrinterResolutionKind.Low;
                }
                else if (ji2.DeviceMode.dmPrintQuality == (int)DeviceModeResolutions.DMRES_MEDIUM)
                {
                    return PrinterResolutionKind.Medium;
                }
                else
                {
                    return PrinterResolutionKind.Custom;
                }
            }
        }
        #endregion

        #region PrinterResolutionX
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The printer resolution in the horizontal dimension 
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This value is set if PrinterResolutionKind is PrinterResolutionKind.Custom
    /// This is measured in dots per inch
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual int PrinterResolutionX
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel2, false);
                return ji2.DeviceMode.dmPrintQuality;
            }
        }
        #endregion

        #region PrinterResolutionY
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The printer resolution in the vertical dimension 
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This valuse is set if PrinterResolutionKind is PrinterResolutionKind.Custom.
    /// This is measured in dots per inch
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual int PrinterResolutionY
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel2, false);
                return ji2.DeviceMode.dmYResolution;
            }
        }
        #endregion

        #region Copies
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The number of copies of each page to print
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// Some applications misreport the number of copies to the spooler which will 
    /// result in incorrect values being returned
    /// </remarks>
    /// <example>Prints the number of copies of each job as it is added to the monitored printers
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs)
    ///        Trace.WriteLine(.PrintJob.Copies.ToString)
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual int Copies
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel2, false);
                if (ji2.DeviceMode.dmCopies > 0)
                {
                    return ji2.DeviceMode.dmCopies;
                }
                else if (ji2.PagesPrinted > ji2.TotalPage)
                {
                    return ji2.PagesPrinted / ji2.TotalPage;
                }
                else
                {
                    return 1;
                }
            }
        }
        #endregion

        #region NotifyUserName
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The user to notify about the progress of this print job
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This should be set to the network login name of the user 
    /// </remarks>
    /// <example>Changes the notify user of each job as it is added to the monitored printers
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs)
    ///        .PrintJob.NotifyUserName = "Administrator"
    ///        .PrintJob.Commit
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual string NotifyUserName
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel2, false);
                if (ji2.pNotifyName is null)
                {
                    return "";
                }
                else
                {
                    return ji2.pNotifyName;
                }
            }
            set
            {
                ji2.pNotifyName = value;
                _Changed_Ji2 = true;
            }
        }
        internal string InitNotifyUsername
        {
            set
            {
                ji2.pNotifyName = value;
            }
        }
        #endregion

        #region PrintProcessorName
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The name of the print processor 
    /// which is responsible for printing this job
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual string PrintProcessorName
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel2, false);
                if (ji2.pPrintProcessor is null)
                {
                    return "";
                }
                else
                {
                    return ji2.pPrintProcessor;
                }
            }
        }
        internal string InitPrintProcessorName
        {
            set
            {
                ji2.pPrintProcessor = value;
            }
        }
        #endregion

        #region Drivername
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The name of the printer driver
    /// that is responsible for producing this print job
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual string DriverName
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel2, false);
                if (ji2.pDriverName is null)
                {
                    return "";
                }
                else
                {
                    return ji2.pDriverName;
                }
            }
        }
        internal string InitDrivername
        {
            set
            {
                ji2.pDriverName = value;
            }
        }
        #endregion

        #region Priority
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The priority of this print job.  Higher priority jobs will be processed ahead
    /// of lower priority jobs
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// Valid values are in the range of 1 (lowest priority) to 99 (highest priority)
    /// </remarks>
    /// <example>Prints the priority of each job as it is added to the monitored printers
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs)
    ///        Trace.WriteLine(.PrintJob.Priority.ToString)
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual int Priority
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel2, false);
                return ji2.Priority;
            }
            set
            {
                if (value > 99)
                {
                    // \\ Priority cannot exceed 99
                    ji2.Priority = 99;
                }
                else if (value < 1)
                {
                    // \\ Priority cannot be less than 1
                    ji2.Priority = 1;
                }
                else
                {
                    ji2.Priority = value;
                }
                _Changed_Ji2 = true;
            }
        }
        internal int InitPriority
        {
            set
            {
                ji2.Priority = value;
            }
        }
        #endregion

        #region Parameters
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Extra driver specific parameters for this print job
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The acceptable parameters and values depend on the print driver being used to 
    /// print this print job
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual string Parameters
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel2, false);
                if (ji2.pParameters is null)
                {
                    return "";
                }
                else
                {
                    return ji2.pParameters;
                }
            }
        }
        internal string InitParameters
        {
            set
            {
                ji2.pParameters = value;
            }
        }
        #endregion

        #region Submitted
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Specifies the date and time at which the job was submitted for printing
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The time value is returned in the local time of the machine on which the PrintQueueWatch
    /// component is installed 
    /// </remarks>
    /// <example>Prints the date and time submitted of each job as it is added to the monitored printers
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs)
    ///        Trace.WriteLine(.PrintJob.Submitted.ToString)
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Description("Specifies the date and time at which the job was submitted for printing")]
        public virtual DateTime Submitted
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel2, false);
                // \\ Submitted is held in FileTime so needs to be converted to the localised time to take account of daylight saving etc.
                return ji2.Submitted.ToDateTime().ToLocalTime();
            }
        }
        #endregion

        #region TimeWindow
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Specifies the earliest time and latest times that the job can be printed
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// See the TimeWindow class for 
    /// details of the settings of this class
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Description("Specifies the earliest time and latest times that the job can be printed")]
        public virtual TimeWindow TimeWindow
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel2, false);
                _TimeWindow = new TimeWindow(ji2.StartTime, ji2.UntilTime);
                return _TimeWindow;
            }
            set
            {
                ji2.StartTime = value.StartTimeMinutes;
                ji2.UntilTime = value.EndTimeMinutes;
                _Changed_Ji2 = true;
            }
        }
        #endregion

        #region QueuedTime
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Specifies the total time, in milliseconds, that has elapsed since the job began printing
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Description("Specifies the total time, in milliseconds, that has elapsed since the job began printing")]
        public virtual int QueuedTime
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel2, true);
                return ji2.Time;
            }
        }
        #endregion

        #region JobSize
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Specifies the size, in bytes, of the job
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// While the job is being spooled this will contain the current size of the spool file
    /// </remarks>
    /// <example>Prints the job size of each job as it is added to the monitored printers
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs)
    ///        Trace.WriteLine(.PrintJob.DataType)
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Description("Specifies the size, in bytes, of the job")]
        public virtual int JobSize
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel2, true);
                return ji2.JobSize;
            }
        }
        #endregion

        #region LogicalPagesPerPhysicalPage
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The number of logical pages for each physical page
    /// </summary>
    /// <value></value>
    /// <remarks>This value should be 1, 2, 4, 8 or 16
    /// </remarks>
    /// <history>
    /// 	[Duncan]	11/02/2006	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public int LogicalPagesPerPhysicalPage
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel2, false);
                if (ji2.DeviceMode.dmNup > 0)
                {
                    return ji2.DeviceMode.dmNup;
                }
                else
                {
                    return 1;
                }
            }
        }
        #endregion

        #region DefaultPaperSource
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The paper tray (or input bin) to use for this print job
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	11/02/2006	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PaperSourceKind DefaultPaperSource
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel2, false);
                return (PaperSourceKind)ji2.DeviceMode.dmDefaultSource;
            }
        }
        #endregion

        #region Duplex
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the print job is to be printed in colour
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This will be true if the setting is set to rpint in colour even if
    /// the actual document has no colour elements 
    /// </remarks>
    /// <example>Prints the colour / monochrome setting of each job as it is added to the monitored printers
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs)
    ///        Trace.WriteLine(Iif(.PrintJob.Duplex,"Duplex", "Simplex"))
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// -----------------------------------------------------------------------------
        public virtual bool Duplex
        {
            get
            {
                RefreshJobInfo(JobInfoLevels.JobInfoLevel2, false);
                return !(ji2.DeviceMode.dmDuplex == (int)DeviceDuplexSettings.DMDUP_SIMPLEX);
            }
        }
        #endregion

        #endregion

        #region Cancel
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Cancels this print job and removes it from the spool queue
    /// </summary>
    /// <remarks>
    /// Only the originator of a print job or a user with administrator rights on the
    /// print device may cancel the job
    /// </remarks>
    /// <exception cref="InsufficentPrintJobAccessRightsException">
    /// Thrown if the user has no access rights to delete this job
    /// </exception>
    /// <example>Cancels any jobs that have more than 8 copies 
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs).PrintJob
    ///        If .Copies > 8 Then
    ///           .Cancel
    ///        End If
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public void Cancel()
        {

            if (Environment.OSVersion.Version.Major < 4)   // \\ For systems less than windows NT 4...
            {
                if (!UnsafeNativeMethods.SetJob(mhPrinter, midJob, 0, IntPtr.Zero, PrintJobControlCommands.JOB_CONTROL_CANCEL))
                {
                    if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                    {
                        Trace.WriteLine("SetJob (Cancel) failed", GetType().ToString());
                    }
                    throw new InsufficentPrintJobAccessRightsException(PrinterMonitorComponent.resources.GetString("pjerr_cancel"), new Win32Exception());
                }
                else if (TraceSwitch.TraceVerbose)
                {
                    Trace.WriteLine("SetJob (Cancel) succeeded", GetType().ToString());
                }
            }
            else if (!UnsafeNativeMethods.SetJob(mhPrinter, midJob, 0, IntPtr.Zero, PrintJobControlCommands.JOB_CONTROL_CANCEL))
            {
                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                {
                    Trace.WriteLine("SetJob (Cancel) failed", GetType().ToString());
                }
                throw new InsufficentPrintJobAccessRightsException(PrinterMonitorComponent.resources.GetString("pjerr_cancel"), new Win32Exception());
            }
            else if (TraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("SetJob (Cancel) succeeded", GetType().ToString());
            }

        }
        #endregion

        #region Delete
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Deletes this print job
    /// </summary>
    /// <remarks>
    /// Only the originator of a print job or a user with administrator rights on the
    /// print device may delete it
    /// </remarks>    
    /// <exception cref="InsufficentPrintJobAccessRightsException">
    /// Thrown if the user has no access rights to delete this job
    /// </exception>
    /// <example>Cancels any jobs that have more than 8 copies 
    /// <code>
    ///   Private WithEvents pmon As New PrinterMonitorComponent
    /// 
    ///   pmon.AddPrinter("Microsoft Office Document Image Writer")
    ///   pmon.AddPrinter("HP Laserjet 5")
    /// 
    ///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
    /// 
    ///    With CType(e, PrintJobEventArgs).PrintJob
    ///        If .Copies > 8 Then
    ///           .Delete
    ///        End If
    ///     End With
    /// 
    /// End Sub
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public void Delete()
        {

            if (!UnsafeNativeMethods.SetJob(mhPrinter, midJob, 0, IntPtr.Zero, PrintJobControlCommands.JOB_CONTROL_DELETE))
            {
                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                {
                    Trace.WriteLine("SetJob (Delete) failed", GetType().ToString());
                }
                throw new InsufficentPrintJobAccessRightsException(PrinterMonitorComponent.resources.GetString("pjerr_delete"), new Win32Exception());
            }
            else if (TraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("SetJob (Delete) succeeded", GetType().ToString());
            }

        }
        #endregion

        #region Print job status settings 
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Whether the print job is paused
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <exception cref="System.ComponentModel.Win32Exception">
    /// Thrown if the job does not exist or the user has no access rights to pause it
    /// </exception>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public bool Paused
        {
            get
            {
                return (Status & (int)PrintJobStatuses.JOB_STATUS_PAUSED) == (int)PrintJobStatuses.JOB_STATUS_PAUSED;
            }
            set
            {
                if (!value.Equals(Paused))
                {
                    // \\ The paused state has changed: Call the pause or resume command as appropriate
                    if (value)
                    {
                        if (!UnsafeNativeMethods.SetJob(mhPrinter, midJob, 0, IntPtr.Zero, PrintJobControlCommands.JOB_CONTROL_PAUSE))
                        {
                            throw new Win32Exception();
                            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                            {
                                Trace.WriteLine("SetJob (Cancel) failed", GetType().ToString());
                            }
                        }
                        else if (TraceSwitch.TraceVerbose)
                        {
                            Trace.WriteLine("SetJob (Pause) succeeded", GetType().ToString());
                        }
                    }
                    else if (!UnsafeNativeMethods.SetJob(mhPrinter, midJob, 0, IntPtr.Zero, PrintJobControlCommands.JOB_CONTROL_RESUME))
                    {
                        throw new Win32Exception();
                        if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                        {
                            Trace.WriteLine("SetJob (Resume) failed", GetType().ToString());
                        }
                    }
                    else if (TraceSwitch.TraceVerbose)
                    {
                        Trace.WriteLine("SetJob (Resume) succeeded", GetType().ToString());
                    }
                }
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the print job has been deleted
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public bool Deleted
        {
            get
            {
                return (Status & (int)PrintJobStatuses.JOB_STATUS_DELETED) == (int)PrintJobStatuses.JOB_STATUS_DELETED;
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the print job is being deleted
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public bool Deleting
        {
            get
            {
                return (Status & (int)PrintJobStatuses.JOB_STATUS_DELETING) == (int)PrintJobStatuses.JOB_STATUS_DELETING;
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the job has been printed
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This will be true once the job has been completely sent to the printer.  This 
    /// does not mean that the physical print out has necessarily appeared.
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public bool Printed
        {
            get
            {
                return (Status & (int)PrintJobStatuses.JOB_STATUS_PRINTED) == (int)PrintJobStatuses.JOB_STATUS_PRINTED;
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the print job is currently printing
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public bool Printing
        {
            get
            {
                return (Status & (int)PrintJobStatuses.JOB_STATUS_PRINTING) == (int)PrintJobStatuses.JOB_STATUS_PRINTING;
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if there is an error with this print job that prevents it from 
    /// printing
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public bool InError
        {
            get
            {
                return (Status & (int)PrintJobStatuses.JOB_STATUS_ERROR) == (int)PrintJobStatuses.JOB_STATUS_ERROR;
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the job is in error because the printer is off line
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public bool Offline
        {
            get
            {
                return (Status & (int)PrintJobStatuses.JOB_STATUS_OFFLINE) == (int)PrintJobStatuses.JOB_STATUS_OFFLINE;
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the job is in error because the printer has run out of paper
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public bool PaperOut
        {
            get
            {
                return (Status & (int)PrintJobStatuses.JOB_STATUS_PAPEROUT) == (int)PrintJobStatuses.JOB_STATUS_PAPEROUT;
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the print job is in error because the printer requires user intervention
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This can be caused by a print job that requires manual paper feed
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public bool UserInterventionRequired
        {
            get
            {
                return (Status & (int)PrintJobStatuses.JOB_STATUS_USER_INTERVENTION) == (int)PrintJobStatuses.JOB_STATUS_USER_INTERVENTION;
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the print job is spooling to a spool file
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public bool Spooling
        {
            get
            {
                return (Status & (int)PrintJobStatuses.JOB_STATUS_SPOOLING) == (int)PrintJobStatuses.JOB_STATUS_SPOOLING;
            }
        }


        internal int InitStatus
        {
            set
            {
                ji1.Status = value;
            }
        }
        #endregion

        #region Transfer
        // TODO: Transfer fails to open the print job consistently - this will need to be 
        // raised as an MS support incident before this can be achieved
        /* TODO ERROR: Skipped DefineDirectiveTrivia
        #Const TRANSFER_SUPPORTED = True
        */
        /* TODO ERROR: Skipped IfDirectiveTrivia
        #If TRANSFER_SUPPORTED Then
        */
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Transfers this print job to the named printer 
    /// </summary>
    /// <param name="NewPrinter">Name of the new printer to move the job to</param>
    /// <param name="RemoveLocal">True to remove this copy of the job</param>
    /// <remarks>If the DataType is RAW then the target printer may not print the job if it does not
    /// support the printer control language that the job contains
    /// </remarks>
    /// <history>
    /// 	[Duncan]	05/12/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public void Transfer(string NewPrinter, bool RemoveLocal)
        {



            var NewDoc = new DOC_INFO_1();

            NewDoc.DocumentName = Document;
            NewDoc.DataType = DataType;
            NewDoc.OutputFilename = string.Empty;

            IntPtr phPrinter;
            var pDefault = new PRINTER_DEFAULTS(PrinterAccessRights.PRINTER_ACCESS_USE);

            if (UnsafeNativeMethods.OpenPrinter(UniquePrinterObject(), out phPrinter, pDefault))
            {
                if (phPrinter.ToInt64() != 0L)
                {
                    // Read this print job into a bit of memory....
                    IntPtr ptBuf;
                    try
                    {
                        ptBuf = Marshal.AllocHGlobal(JobSize);
                    }
                    catch (OutOfMemoryException exMem)
                    {
                        throw new PrintJobTransferException("Print job is too large", exMem);
                        return;
                    }

                    // \\ Read the print job in to memory
                    int pcbneeded;
                    if (!UnsafeNativeMethods.ReadPrinter(phPrinter, ptBuf, JobSize, out pcbneeded))
                    {
                        throw new PrintJobTransferException("Failed to read the print job", new Win32Exception());
                        return;
                    }

                    var DataFile = new PrinterDataFile(ptBuf, DataType);

                    // Open the target printer
                    IntPtr phPrinterTarget;
                    if (UnsafeNativeMethods.OpenPrinter(NewPrinter, out phPrinterTarget, pDefault))
                    {
                        // Start the new document
                        if (UnsafeNativeMethods.StartDocPrinter(phPrinterTarget, 1, NewDoc))
                        {
                            // Print each page in the print job...
                            for (int CurrentPage = 1, loopTo = DataFile.TotalPages; CurrentPage <= loopTo; CurrentPage++)
                            {
                                // 
                                // Print this page
                                if (!UnsafeNativeMethods.WritePrinter(phPrinterTarget, ptBuf, JobSize, out pcbneeded))
                                {
                                    throw new PrintJobTransferException("Failed to write the print job", new Win32Exception());
                                    return;
                                }
                                // Notify the spooler that the page is finished
                                UnsafeNativeMethods.EndPagePrinter(phPrinterTarget);
                            }
                            UnsafeNativeMethods.EndDocPrinter(phPrinterTarget);
                        }
                        else
                        {
                            throw new PrintJobTransferException("Failed to write the print job", new Win32Exception());
                            return;
                        }

                        UnsafeNativeMethods.ClosePrinter(phPrinterTarget);
                    }

                    // Free this buffer again
                    Marshal.FreeHGlobal(ptBuf);
                }
                else
                {
                    throw new InsufficentPrinterAccessRightsException("Could not read the print job");
                }
            }
            else
            {
                throw new Win32Exception();
            }

        }
        /* TODO ERROR: Skipped EndIfDirectiveTrivia
        #End If
        */
        #endregion

        #region PrinterHandle
        private IntPtr PrinterHandle
        {
            get
            {
                var pDef = new PrinterDefaults();

                pDef.DesiredAccess = PrinterAccessRights.PRINTER_ACCESS_USE;

                if (mhPrinter.ToInt64() == 0L)
                {
                    if (!UnsafeNativeMethods.OpenPrinter(PrinterName, out mhPrinter, pDef))
                    {
                        throw new Win32Exception();
                        if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                        {
                            Trace.WriteLine("OpenPrinter() failed", GetType().ToString());
                        }
                    }
                    else
                    {
                        bHandleOwnedByMe = true;
                    }
                }
                return mhPrinter;
            }
        }
        #endregion

        #region DerivedStatusDescription
        private string DerivedStatusDescription()
        {

            var TempDescription = new StringBuilder();

            if (Paused)
            {
                TempDescription.Append(My.Resources.Resources.jsd_paused);
            }

            if (InError)
            {
                TempDescription.Append(My.Resources.Resources.jsd_error);
                if (PaperOut)
                {
                    TempDescription.Append(My.Resources.Resources.jsd_paperout);
                }
            }

            if (Offline)
            {
                TempDescription.Append(My.Resources.Resources.jsd_offline);
            }

            if (Printed)
            {
                TempDescription.Append(My.Resources.Resources.jsd_printed);
            }

            if (Printing)
            {
                TempDescription.Append(My.Resources.Resources.jsd_printing);
            }

            return TempDescription.ToString();

        }
        #endregion

        #region InitJobInfo
        private void InitJobInfo()
        {
            // \\ Get the first cut of the job info..
            _Populated = true;
            try
            {
                ji1 = new JOB_INFO_1(mhPrinter, midJob);
            }
            catch (Win32Exception e32)
            {
                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                {
                    Trace.WriteLine(e32.ToString(), GetType().ToString());
                }
                _Populated = false;
            }
            try
            {
                ji2 = new JOB_INFO_2(mhPrinter, midJob);
            }
            catch (Win32Exception e32_2)
            {
                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                {
                    Trace.WriteLine(e32_2.ToString(), GetType().ToString());
                }
                _Populated = false;
            }

        }
        #endregion

        #region Public constructor
        internal PrintJob(IntPtr hPrinter, int idJob)
        {

            if (TraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("New(" + hPrinter.ToString() + "," + idJob.ToString() + ")", GetType().ToString());
            }

            // \\ Take a local copy of the printer handle and job id
            mhPrinter = hPrinter;
            bHandleOwnedByMe = false;
            midJob = idJob;
            InitJobInfo();
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Returns the job identified by JobId queued against the device named
    /// </summary>
    /// <param name="DeviceName">The name of the device the job is queued against</param>
    /// <param name="idJob">The unique job identifier</param>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrintJob(string DeviceName, int idJob)
        {
            IntPtr hPrinter;
            bHandleOwnedByMe = true;
            if (UnsafeNativeMethods.OpenPrinter(DeviceName, out hPrinter, 0))
            {
                mhPrinter = hPrinter;
                midJob = idJob;
                InitJobInfo();
            }
            else
            {
                throw new Win32Exception();
                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                {
                    Trace.WriteLine("OpenPrinter() failed", GetType().ToString());
                }
            }
        }

        public PrintJob()
        {

        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Frees up system resources used by this PrintJob class
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (bHandleOwnedByMe)
                {
                    if (!UnsafeNativeMethods.ClosePrinter(mhPrinter))
                    {
                        if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                        {
                            Trace.WriteLine("Error in PrinterInformation:Dispose");
                        }
                    }
                }
            }
        }

        ~PrintJob()
        {
            Dispose(false);
        }

        #endregion

        #region RefreshJobInfo
        private void RefreshJobInfo(JobInfoLevels Index, bool ForceReload)
        {

            JOB_INFO_1 ji1Temp;
            JOB_INFO_2 ji2Temp;


            if (TraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("RefreshJobInfo(" + Index.ToString() + ")", GetType().ToString());
            }

            switch (Index)
            {
                case JobInfoLevels.JobInfoLevel3:
                    {
                        break;
                    }

                case JobInfoLevels.JobInfoLevel2:
                    {
                        if (ForceReload || ji2 is null)
                        {
                            try
                            {
                                ji2Temp = new JOB_INFO_2(mhPrinter, midJob);
                            }
                            catch (Win32Exception e)
                            {
                                return;
                            }
                            ji2 = ji2Temp;
                        }

                        break;
                    }

                default:
                    {
                        if (ForceReload || ji1 is null)
                        {
                            try
                            {
                                ji1Temp = new JOB_INFO_1(mhPrinter, midJob);
                            }
                            catch (Win32Exception e)
                            {
                                return;
                            }
                            ji1 = ji1Temp;
                        }

                        break;
                    }
            }

        }
        #endregion

        #region SaveJobInfo
        private void SaveJobInfo(JobInfoLevels Index)
        {
            const int JOB_POSITION_UNSPECIFIED = 0;

            if (TraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("SaveJobInfo(" + Index.ToString() + ")", GetType().ToString());
            }

            switch (Index)
            {
                case JobInfoLevels.JobInfoLevel3:
                    {
                        break;
                    }

                case JobInfoLevels.JobInfoLevel2:
                    {
                        // \\ Update the JOB_INFO_2 structure held in the spool file
                        if (!_PositionChanged)
                        {
                            ji2.Position = JOB_POSITION_UNSPECIFIED;
                        }
                        if (!UnsafeNativeMethods.SetJob(mhPrinter, midJob, (int)JobInfoLevels.JobInfoLevel2, ji2, PrintJobControlCommands.JOB_CONTROL_SETJOB))
                        {
                            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                            {
                                Trace.WriteLine("SetJob() failed", GetType().ToString());
                            }
                            throw new InsufficentPrintJobAccessRightsException(PrinterMonitorComponent.resources.GetString("pjerr_update"), new Win32Exception());
                        }
                        else
                        {
                            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
                            {
                                Trace.WriteLine("Job Info (2) saved ", GetType().ToString());
                            }
                            _Changed_Ji2 = false;
                        }

                        break;
                    }

                default:
                    {
                        // \\ Update the JOB_INFO_1 structure held in the spool file
                        if (!_PositionChanged)
                        {
                            ji1.Position = JOB_POSITION_UNSPECIFIED;
                        }
                        if (!UnsafeNativeMethods.SetJob(mhPrinter, midJob, (int)Index, ji1, PrintJobControlCommands.JOB_CONTROL_SETJOB))
                        {
                            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                            {
                                Trace.WriteLine("SetJob() failed", GetType().ToString());
                            }
                            throw new InsufficentPrintJobAccessRightsException(PrinterMonitorComponent.resources.GetString("pjerr_update"), new Win32Exception());
                        }
                        else
                        {
                            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
                            {
                                Trace.WriteLine("Job Info (1) saved ", GetType().ToString());
                            }
                            _Changed_Ji1 = false;
                        }

                        break;
                    }
            }
            _PositionChanged = false;

            _TimeWindow.Changed = false;
        }
        #endregion

        #endregion

        #region Friend properties
        #region UrlString
        internal string UrlString
        {
            set
            {
                _UrlString = value;
            }
        }
        #endregion

        #region Populated
        internal bool Populated
        {
            get
            {
                return _Populated;
            }
        }
        #endregion

        #region Refresh
        // \\--[Refresh]--------------------------------------------------
        // \\ Repopulate the PrintJob from the spooler [API] on demand
        // \\ for the case that it was not succesfully filled on creation
        // \\ ------------------------------------------------------------
        internal void Refresh()
        {
            InitJobInfo();
        }
        #endregion

        #region UniquePrinterObject
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Returns the unique name of this job which can be passed to get a handle to it
    /// </summary>
    /// <returns>[PrinterNmae], PrintJob xxxxx </returns>
    /// <remarks>Used internally for ReadPrinter api call
    /// </remarks>
    /// <history>
    /// 	[Duncan]	05/12/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        internal string UniquePrinterObject()
        {

            return PrinterName + ",Job " + JobId.ToString();

        }
        #endregion

        #endregion

    }

    #endregion

    #region Print Job type safe collection

    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : PrintJobCollection
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// A collection of print jobs
/// </summary>
/// <remarks>
/// </remarks>
/// <history>
/// 	[Duncan]	21/11/2005	Created
/// </history>
/// -----------------------------------------------------------------------------
    [ComVisible(false)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public class PrintJobCollection : System.Collections.Concurrent.ConcurrentDictionary<int, PrintJob>
    {

        #region Private member variables
        private bool bHandleOwnedByMe;
        private IntPtr hPrinter;
        #endregion

        #region JobPendingDeletion
        // \\ Because the delete event is asynchronous, jobs have to be removed from the
        // \\ job list one in arrears.  This public variable tells which one is to be removed
        // \\ next.
        internal int JobPendingDeletion;
        #endregion

        #region Finalize
        ~PrintJobCollection()
        {
            if (bHandleOwnedByMe)
            {
                if (!UnsafeNativeMethods.ClosePrinter(hPrinter))
                {
                    if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                    {
                        Trace.WriteLine("Error in PrinterInformation:Dispose");
                    }
                }
            }
        }
        #endregion

        #region Public interface

        internal PrintJob get_AddOrGetById(int dwJobId, IntPtr mhPrinter)
        {
            PrintJob pjThis;
            if (!get_ContainsJobId(dwJobId))
            {
                try
                {
                    pjThis = new PrintJob(mhPrinter, dwJobId);
                    Add(dwJobId, pjThis);
                }
                catch (Win32Exception e)
                {
                    if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                    {
                        Trace.WriteLine("AddOrGetById(" + dwJobId.ToString() + ") failed - " + e.Message + "::" + e.NativeErrorCode, GetType().ToString());
                    }
                }
            }
            return get_ItemByJobId(dwJobId);
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Gets the print job by its unique Job Id
    /// </summary>
    /// <param name="dwJob"></param>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrintJob get_ItemByJobId(int dwJob)
        {
            return this[dwJob];
        }

        public void Add(PrintJob pjIn)
        {
            Add(pjIn.JobId, pjIn);
        }

        public void Add(int jobId, PrintJob pjIn)
        {
            TryAdd(pjIn.JobId, pjIn);
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if this collection contains the given Job Id
    /// </summary>
    /// <param name="pjTestId"></param>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public bool get_ContainsJobId(int pjTestId)
        {
            return ContainsKey(pjTestId);
        }

        [Description("Removes the print job identified by the given job id from the printjobs collection")]
        public void RemoveByJobId(int pjId)
        {
            var removed = new PrintJob();
            TryRemove(pjId, out removed);
        }



        #endregion
        #region Public constructors

        private void InitJobList(IntPtr mhPrinter, int JobCount)
        {
            int pcbNeeded; // \\ Holds the requires size of the output buffer (in bytes)
            int pcReturned; // \\ Holds the returned size of the output buffer (in bytes)
            IntPtr pJobInfo;

            // \\ Save the printer handle
            hPrinter = mhPrinter;

            // \\ If the current jobcount is unknown, try 255
            if (JobCount == 0)
            {
                JobCount = 255;
            }

            if (!UnsafeNativeMethods.EnumJobs(mhPrinter, 0, JobCount, JobInfoLevels.JobInfoLevel1, IntPtr.Zero, 0, out pcbNeeded, out pcReturned))
            {
                if (pcbNeeded > 0)
                {
                    pJobInfo = Marshal.AllocHGlobal(pcbNeeded);
                    int pcbProvided = pcbNeeded;
                    int pcbTotalNeeded; // \\ Holds the requires size of the output buffer (in bytes)
                    int pcTotalReturned; // \\ Holds the returned size of the output buffer (in bytes)
                    if (UnsafeNativeMethods.EnumJobs(mhPrinter, 0, JobCount, JobInfoLevels.JobInfoLevel1, pJobInfo, pcbProvided, out pcbTotalNeeded, out pcTotalReturned))
                    {
                        if (pcTotalReturned > 0)
                        {
                            int item;
                            var pnextJob = pJobInfo;
                            var loopTo = pcTotalReturned - 1;
                            for (item = 0; item <= loopTo; item++)
                            {
                                var jiTemp = new JOB_INFO_1(pnextJob);
                                Add(new PrintJob(mhPrinter, jiTemp.JobId));
                                pnextJob = pnextJob + Marshal.SizeOf(jiTemp);
                            }
                        }
                    }
                    else
                    {
                        throw new Win32Exception();
                        if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                        {
                            Trace.WriteLine("EnumJobs() failed", GetType().ToString());
                        }
                    }
                    Marshal.FreeHGlobal(pJobInfo);
                }
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Creates a new empty PrintJobs collection
    /// </summary>
    /// <remarks>
    /// This constructor is not meant for use except by the PrinterQueueWatch component
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Description("Creates a new empty PrintJobs collection")]
        public PrintJobCollection()
        {

        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Creates a new list and fills it with all the jobs currently on a given printer's queue by printer handle
    /// </summary>
    /// <param name="mhPrinter">The handle of the printer to get the print jobs for</param>
    /// <param name="JobCount">The number of jobs to retrieve</param>
    /// <remarks>
    /// If JobCount is less than the number of jobs in teh queue only the first JobCount number will 
    /// be returned
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Description("Creates a new list and fills it with all the jobs currently on a given printer's queue by printer handle")]
        public PrintJobCollection(IntPtr mhPrinter, int JobCount)
        {

            if (PrintJob.TraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("New(" + mhPrinter.ToString() + "," + JobCount.ToString() + ")", GetType().ToString());
            }
            InitJobList(mhPrinter, JobCount);

        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Creates a new list and fills it with all the jobs currently on a given printer's queue by printer device name
    /// </summary>
    /// <param name="DeviceName">The name of the print device to get the jobs for</param>
    /// <param name="JobCount">The number of print jobs to return</param>
    /// <remarks>
    /// If JobCount is less than the number of jobs in the queue only the first JobCount number will 
    /// be returned
    /// </remarks>
    /// <exception cref="System.ComponentModel.Win32Exception">
    /// Thrown if the print device does not exist or the user has no access rights to retrieve the job queue from it
    /// </exception>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Description("Creates a new list and fills it with all the jobs currently on a given printer's queue by printer device name ")]
        public PrintJobCollection(string DeviceName, int JobCount)
        {

            if (PrintJob.TraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("New(" + DeviceName + "," + JobCount.ToString() + ")", GetType().ToString());
            }

            IntPtr hPrinter;
            bHandleOwnedByMe = true;
            if (UnsafeNativeMethods.OpenPrinter(DeviceName, out hPrinter, 0))
            {
                InitJobList(hPrinter, JobCount);
            }
            else
            {
                throw new Win32Exception();
                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                {
                    Trace.WriteLine("OpenPrinter() failed", GetType().ToString());
                }
            }

        }

        #endregion

    }

    #endregion

    #region TimeWindow class
    // \\ --[TimeWindow]--------------------------------------------------
    // \\ Specifies a time window during which an event can be scheduled -
    // \\ for example when a print job can be printed
    // \\ (c) 2003 Merrion Computing Ltd
    // \\ ----------------------------------------------------------------

    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : TimeWindow
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// Specifies a time window during which an event can be scheduled -
/// for example when a print job can be printed
/// </summary>
/// <remarks>
/// </remarks>
/// <history>
/// 	[Duncan]	21/11/2005	Created
/// </history>
/// -----------------------------------------------------------------------------
    public class TimeWindow
    {

        #region Static functions

        public static int LocalTimeToMinutesPastGMT(DateTime LocalTime)
        {

            var dtGMT = LocalTime.ToUniversalTime();
            return dtGMT.Hour * 60 + dtGMT.Minute;

        }

        public static DateTime MinutesPastGMTToLocalTime(int MinutesPastGMT)
        {

            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, MinutesPastGMT / 60, MinutesPastGMT % 60, 0, 0).ToLocalTime();

        }

        #endregion

        #region Private members
        private int _StartTime; // Minutes past GMT
        private int _EndTime; // Minutes past GMT
        private bool _Changed;
        #endregion

        #region Public constructors
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Creates a new time window with the given range
    /// </summary>
    /// <param name="StartTime">The start of the time range in minutes past midnight GMT</param>
    /// <param name="EndTime">The end of the time range in minutes past midnight GMT</param>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public TimeWindow(int StartTime, int EndTime)
        {

            if (PrintJob.TraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("New(" + StartTime.ToString() + "," + EndTime.ToString() + ")", GetType().ToString());
            }

            if (StartTime > EndTime)
            {
                _StartTime = EndTime;
                _EndTime = StartTime;
            }
            else
            {
                _StartTime = StartTime;
                _EndTime = EndTime;
            }
            _Changed = true;

        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Creates a new time window with the given range
    /// </summary>
    /// <param name="StartTime">The start of the time range in local time</param>
    /// <param name="EndTime">The end of the time range in local time</param>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public TimeWindow(DateTime StartTime, DateTime EndTime) : this(LocalTimeToMinutesPastGMT(StartTime), LocalTimeToMinutesPastGMT(EndTime))
        {
            _Changed = true;
        }

        public TimeWindow()
        {
            // \\ Initialise to an unrestricted time window
            _StartTime = 0;
            _EndTime = 0;
            _Changed = true;
        }
        #endregion

        #region Public interface
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The time of the start of this time range
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This value is in the system local time 
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual DateTime StartTime
        {
            get
            {
                return MinutesPastGMTToLocalTime(_StartTime);
            }
            set
            {
                _StartTime = LocalTimeToMinutesPastGMT(value);
                if (_StartTime > _EndTime)
                {
                    _EndTime = _StartTime;
                }
                _Changed = true;
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The end of the time range
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This time is in local system time
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual DateTime EndTime
        {
            get
            {
                return MinutesPastGMTToLocalTime(_EndTime);
            }
            set
            {
                _EndTime = LocalTimeToMinutesPastGMT(value);
                if (_EndTime < _StartTime)
                {
                    _StartTime = _EndTime;
                }
                _Changed = true;
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the time range is unrestricted
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// If the time range is unrestricted it is from midnight to midnight
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual bool Unrestricted
        {
            get
            {
                return _StartTime == 0 & _EndTime == 0;
            }
            set
            {
                if (value)
                {
                    _StartTime = 0;
                    _EndTime = 0;
                }
                _Changed = true;
            }
        }

        internal bool Changed
        {
            get
            {
                return _Changed;
            }
            set
            {
                _Changed = value;
            }
        }
        #endregion

        #region Hidden interface
        internal int StartTimeMinutes
        {
            get
            {
                return _StartTime;
            }
        }
        internal int EndTimeMinutes
        {
            get
            {
                return _EndTime;
            }
        }
        #endregion

        #region ToString override
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Returns a text description of the tiem range
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
            var sOut = new StringBuilder();
            if (PrinterMonitorComponent.resources is not null)
            {
                {
                    ref var withBlock = ref PrinterMonitorComponent.resources;
                    // \\ Return the data in this TimeWindow class
                    sOut.Append(" ");
                    if (Unrestricted)
                    {
                        sOut.Append(withBlock.GetString("tw_ts_Unrestricted"));
                    }
                    else
                    {
                        sOut.Append(withBlock.GetString("tw_ts_From"));
                        sOut.Append(" ");
                        sOut.Append(StartTime.ToString());
                        sOut.Append(" ");
                        sOut.Append(withBlock.GetString("tw_ts_Until"));
                        sOut.Append(" ");
                        sOut.Append(EndTime.ToString());
                    }
                }
            }
            else
            {
                sOut.Append(GetType().ToString());
            }
            return sOut.ToString();
        }
        #endregion

    }

    #endregion

    #region Friend helper classes
    #region PrinterDataFile
    /// -----------------------------------------------------------------------------
    /// Project	 : PrinterQueueWatch
    /// Class	 : PrinterDataFile
    /// 
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// An in-memory representation of a print spool file
    /// </summary>
    /// <remarks>
    /// This class is for internal use of the PrintQueueWatch component
    /// </remarks>
    /// <history>
    /// 	[Duncan]	05/12/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
    internal class PrinterDataFile
    {

        #region Constants
        // \\ PJL Notes :--
        // \\ Ways of storing the job language
        private const string PJL_LANGUAGE = "@PJL ENTER LANGUAGE";
        private const string PJL_LANGUAGE_POSTSCRIPT = "POSTSCRIPT";
        private const string PJL_LANGUAGE_PCL = "PCL";
        private const string PJL_LANGUAGE_PCLXL = "PCLXL";
        private const string PJL_LANGUAGE_HPGL = "HPGL";
        private const string RAW_POSTSCRIPT = "%!PS-Adobe";
        private const string RAW_PCL_NUMERIC = "%-12345X";

        // \\ Ways of storing the number of copies
        private const string PJL_SET_COPIES = "@PJL SET COPIES";
        private const string PJL_SET_COPIES_QTY = "@PJL SET QTY";
        private const string PJL_COMMENT_QTY = "@PJL COMMENT @@CPY";

        // \\ PostScript notes:----
        // \\ "%%Page" comment marks the start of each printed page
        // \\ "NumCopies" or "#copies" precedes number of copies
        private const string POSTSCRIPT_COPIES = "#copies";
        private const string POSTSCRIPT_NUMCOPIES = "NumCopies";
        // \\ HPGL notes :----
        // \\ "RP" precedes number of copies
        private const string HPGL_COPIES = "RP";

        #endregion

        #region Public enumerated types
        public enum SpoolFileFormats
        {
            EMF = 1,
            PCL_5 = 2,
            PCL_6 = 3,
            Postscript = 4,
            HPGL = 5
        }
        #endregion

        #region Private members
        private IntPtr _ptBuf;
        private string _DataType;

        private int _TotalPages;
        #endregion

        #region Public interface

        #region TotalPages
        public int TotalPages
        {
            get
            {
                return _TotalPages;
            }
        }
        #endregion

        // todo: get the start and size of each page in memory...


        #endregion

        #region Public constructors
        public PrinterDataFile(IntPtr Buffer, string DataType)
        {
            _ptBuf = Buffer;
            _DataType = DataType;
        }
        #endregion

    }
    #endregion

    #region EMF Spool File
    /// -----------------------------------------------------------------------------
    /// Project	 : PrinterQueueWatch
    /// Class	 : EMF_SpoolFile
    /// 
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Helper class for EMF format spool file
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	12/12/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
    internal class EMF_SpoolFile
    {

        #region Private helper classes
        #region EMFPAGE
        private class EMFPAGE
        {

            #region Private members
            private int _StartOffset;
            private int _EndOffset;
            private int _BasePtr;

            private EMFMETAHEADER _Header;
            #endregion

            #region Public Interface
            public int StartOffset
            {
                get
                {
                    return _StartOffset;
                }
            }

            public int EndOffset
            {
                get
                {
                    return _EndOffset;
                }
            }
            #endregion

            #region Public constructor
            public EMFPAGE(int MemPtr, int Start)
            {

                _BasePtr = MemPtr;
                _StartOffset = Start;
                _Header = new EMFMETAHEADER(MemPtr, Start);
                _EndOffset = Start + _Header.FileSize;

            }
            #endregion
        }
        #endregion
        #region EMFMETAHEADER
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        private class EMFMETAHEADER
        {

            #region Properties
            // \\ EMR record header
            private int iType;
            private int nSize;
            // \\ Boundary rectangle
            private int rclBounds_Left;
            private int rclBounds_Top;
            private int rclBounds_Right;
            private int rclBounds_Bottom;
            // \\ Frame rectangle
            private int _rclFrame_Left;
            private int _rclFrame_Top;
            private int _rclFrame_Right;
            private int _rclFrame_Bottom;
            // \\ "Signature"
            private byte _signature_1;
            private byte _signature_2; // E
            private byte _signature_3; // M
            private byte _signature_4; // F
                                       // \\ nVersion
            private int _nVersion;
            private int _nBytes;
            private int _nRecords;
            private int _nHandles;
            private short _sReversed;
            private short _nDescription;
            private int _offDescription;
            private int _nPalEntries;
            private int _szlDeviceWidth;
            private int _szlDeviceHeight;
            private int _szlDeviceWidthMilimeters;
            private int _szlDeviceHeightMilimeters;
            private int _cbPixelFormat;
            private int _offPixelFormat;
            private bool _bOpenGL;
            private int _szlMicrometersWidth;
            private int _szlMicrometersHeight;
            private string _Description;

            #endregion

            #region Public properties
            public Rectangle BoundaryRect
            {
                get
                {
                    return new Rectangle(rclBounds_Left, rclBounds_Top, rclBounds_Right, rclBounds_Bottom);
                }
            }

            public Rectangle FrameRect
            {
                get
                {
                    return new Rectangle(_rclFrame_Left, _rclFrame_Top, _rclFrame_Right, _rclFrame_Bottom);
                }
            }

            public int Size
            {
                get
                {
                    return nSize;
                }
            }

            public int RecordCount
            {
                get
                {
                    return _nRecords;
                }
            }

            public int FileSize
            {
                get
                {
                    return _nBytes;
                }
            }

            #endregion

            #region Public constructor
            public EMFMETAHEADER(int MemPtr, int Start)
            {

                iType = Marshal.ReadInt32(MemPtr, Start);
                Start += Marshal.SizeOf(typeof(int));
                nSize = Marshal.ReadInt32(MemPtr, Start);
                Start += Marshal.SizeOf(typeof(int));
                // \\ Boundary rectangle
                rclBounds_Left = Marshal.ReadInt32(MemPtr, Start);
                Start += Marshal.SizeOf(typeof(int));
                rclBounds_Top = Marshal.ReadInt32(MemPtr, Start);
                Start += Marshal.SizeOf(typeof(int));
                rclBounds_Right = Marshal.ReadInt32(MemPtr, Start);
                Start += Marshal.SizeOf(typeof(int));
                rclBounds_Bottom = Marshal.ReadInt32(MemPtr, Start);
                Start += Marshal.SizeOf(typeof(int));
                // \\ Frame rectangle
                _rclFrame_Left = Marshal.ReadInt32(MemPtr, Start);
                Start += Marshal.SizeOf(typeof(int));
                _rclFrame_Top = Marshal.ReadInt32(MemPtr, Start);
                Start += Marshal.SizeOf(typeof(int));
                _rclFrame_Right = Marshal.ReadInt32(MemPtr, Start);
                Start += Marshal.SizeOf(typeof(int));
                _rclFrame_Bottom = Marshal.ReadInt32(MemPtr, Start);
                Start += Marshal.SizeOf(typeof(int));
                // \\ "Signature"
                _signature_1 = Marshal.ReadByte(MemPtr, Start);
                Start += 1;
                _signature_2 = Marshal.ReadByte(MemPtr, Start);
                Start += 1;
                _signature_3 = Marshal.ReadByte(MemPtr, Start);
                Start += 1;
                _signature_4 = Marshal.ReadByte(MemPtr, Start);
                Start += 1;
                // \\ nVersion
                _nVersion = Marshal.ReadInt32(MemPtr, Start);
                Start += Marshal.SizeOf(typeof(int));
                _nBytes = Marshal.ReadInt32(MemPtr, Start);
                Start += Marshal.SizeOf(typeof(int));
                _nRecords = Marshal.ReadInt32(MemPtr, Start);
                Start += Marshal.SizeOf(typeof(int));
                _nHandles = Marshal.ReadInt16(MemPtr, Start);
                Start += Marshal.SizeOf(typeof(short));
                _sReversed = Marshal.ReadInt16(MemPtr, Start);
                Start += Marshal.SizeOf(typeof(short));
                _nDescription = Marshal.ReadInt16(MemPtr, Start);
                Start += Marshal.SizeOf(typeof(short));
                _offDescription = Marshal.ReadInt32(MemPtr, Start);
                Start += Marshal.SizeOf(typeof(int));
                _nPalEntries = Marshal.ReadInt32(MemPtr, Start);
                Start += Marshal.SizeOf(typeof(int));
                _szlDeviceWidth = Marshal.ReadInt32(MemPtr, Start);
                Start += Marshal.SizeOf(typeof(int));
                _szlDeviceHeight = Marshal.ReadInt32(MemPtr, Start);
                Start += Marshal.SizeOf(typeof(int));
                _szlDeviceWidthMilimeters = Marshal.ReadInt32(MemPtr, Start);
                Start += Marshal.SizeOf(typeof(int));
                _szlDeviceHeightMilimeters = Marshal.ReadInt32(MemPtr, Start);
                Start += Marshal.SizeOf(typeof(int));
                if (nSize > Start)
                {
                    _cbPixelFormat = Marshal.ReadInt32(MemPtr, Start);
                    Start += Marshal.SizeOf(typeof(int));
                    _offPixelFormat = Marshal.ReadInt32(MemPtr, Start);
                    Start += Marshal.SizeOf(typeof(int));
                    _bOpenGL = Marshal.ReadInt32(MemPtr, Start) != 0;
                }
                if (nSize > Start)
                {
                    _szlMicrometersWidth = Marshal.ReadInt32(MemPtr, Start);
                    Start += Marshal.SizeOf(typeof(int));
                    _szlMicrometersHeight = Marshal.ReadInt32(MemPtr, Start);
                    Start += Marshal.SizeOf(typeof(int));
                }
                if (_nDescription > 0)
                {
                    _Description = Marshal.PtrToStringAuto((IntPtr)(MemPtr + Start), _nDescription);
                }

            }
            #endregion

        }
        #endregion
        #region DEVMODE
        private class DEVMODE
        {
            #region Private properties
            private char[] dmDeviceName = new char[65];
            private short dmSpecVersion;
            private short dmDriverVersion;
            private short dmSize;
            private short dmDriverExtra;
            private int dmFields;
            private short dmOrientation;
            private short dmPaperSize;
            private short dmPaperLength;
            private short dmPaperWidth;
            private short dmScale;
            private short dmCopies;
            private short dmDefaultSource;
            private short dmPrintQuality;
            private short dmColor;
            private short dmDuplex;
            private short dmYResolution;
            private short dmTTOption;
            private short dmCollate;
            private char[] dmFormName = new char[33];
            private short dmUnusedPadding;
            private int dmBitsPerPel;
            private int dmPelsWidth;
            private int dmPelsHeight;
            private int dmNup;
            private int dmDisplayFrequency;
            private int dmICMMethod;
            private int dmICMIntent;
            private int dmMediaType;
            private int dmDitherType;
            private int dmReserved1;
            private int dmReserved2;
            private int dmPanningWidth;
            private int dmPanningHeight;
            private byte[] DriverExtra;
            #endregion

            #region Public properties
            #region Fields
            private DevModeFields Fields
            {
                get
                {
                    return new DevModeFields(dmFields);
                }
            }
            #endregion
            #region DeviceName
            public string DeviceName
            {
                get
                {
                    // \\ Remove the balnk chars...
                    if (dmDeviceName.Length == 64)
                    {
                        foreach (var c in dmDeviceName)
                        {

                        }
                    }
                    return new string(dmDeviceName);
                }
            }
            #endregion
            #region FormName
            /// -----------------------------------------------------------------------------
        /// <summary>
        /// The name of the form used to print the print job
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Duncan]	17/02/2006	Created
        /// </history>
        /// -----------------------------------------------------------------------------
            public string FormName
            {
                get
                {
                    if (dmFormName is null)
                    {
                        return "";
                    }
                    else
                    {
                        return new string(dmFormName);
                    }
                }
            }
            #endregion

            #region Copies
            public short Copies
            {
                get
                {
                    if (dmCopies < 1)
                    {
                        dmCopies = 1;
                    }
                    return dmCopies;
                }
            }
            #endregion
            #region Collate
            public bool Collate
            {
                get
                {
                    return dmCollate > 0;
                }
            }
            #endregion

            #region DriverVersion
            /// -----------------------------------------------------------------------------
        /// <summary>
        /// The version number of the driver used to print this job
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Duncan]	17/02/2006	Created
        /// </history>
        /// -----------------------------------------------------------------------------
            public int DriverVersion
            {
                get
                {
                    return dmDriverVersion;
                }
            }
            #endregion

            #endregion

            #region Public constructors
            public DEVMODE()
            {


            }
            #endregion

            #region DevModeFields
            private class DevModeFields
            {

                #region Private properties
                private int _dmFields;
                #endregion

                #region Private enumerated types
                [Flags()]
                private enum DeviceModeFieldFlags
                {
                    DM_POSITION = 0x20,
                    DM_COLLATE = 0x8000,
                    DM_COLOR = 0x800,
                    DM_COPIES = 0x100,
                    DM_DEFAULTSOURCE = 0x200,
                    DM_DITHERTYPE = 0x10000000,
                    DM_DUPLEX = 0x1000,
                    DM_FORMNAME = 0x10000,
                    DM_ICMINTENT = 0x4000000,
                    DM_ICMMETHOD = 0x2000000,
                    DM_MEDIATYPE = 0x8000000,
                    DM_ORIENTATION = 0x1,
                    DM_PAPERLENGTH = 0x4,
                    DM_PAPERSIZE = 0x2,
                    DM_PAPERWIDTH = 0x8,
                    DM_PRINTQUALITY = 0x400,
                    DM_RESERVED1 = 0x800000,
                    DM_RESERVED2 = 0x1000000,
                    DM_SCALE = 0x10
                }
                #endregion

                #region Public constructor
                public DevModeFields(int Flags)
                {
                    _dmFields = Flags;
                }
                #endregion

                #region Public interface
                #region Orientation
                public bool Orientation
                {
                    get
                    {
                        return (_dmFields & (int)DeviceModeFieldFlags.DM_ORIENTATION) > 0;
                    }
                }
                #endregion

                #region PaperSize
                public bool PaperSize
                {
                    get
                    {
                        return (_dmFields & (int)DeviceModeFieldFlags.DM_PAPERSIZE) > 0;
                    }
                }
                #endregion

                #region PaperLength
                public bool PaperLength
                {
                    get
                    {
                        return (_dmFields & (int)DeviceModeFieldFlags.DM_PAPERLENGTH) > 0;
                    }
                }
                #endregion

                #region PaperWidth
                public bool PaperWidth
                {
                    get
                    {
                        return (_dmFields & (int)DeviceModeFieldFlags.DM_PAPERWIDTH) > 0;
                    }
                }
                #endregion

                #region Scale
                public bool Scale
                {
                    get
                    {
                        return (_dmFields & (int)DeviceModeFieldFlags.DM_SCALE) > 0;
                    }
                }
                #endregion

                #region Copies
                public bool Copies
                {
                    get
                    {
                        return (_dmFields & (int)DeviceModeFieldFlags.DM_COPIES) > 0;
                    }
                }
                #endregion

                #region DefaultSource
                public bool DefaultSource
                {
                    get
                    {
                        return (_dmFields & (int)DeviceModeFieldFlags.DM_DEFAULTSOURCE) > 0;
                    }
                }
                #endregion

                #region Quality
                public bool Quality
                {
                    get
                    {
                        return (_dmFields & (int)DeviceModeFieldFlags.DM_PRINTQUALITY) > 0;
                    }
                }
                #endregion

                #region Position
                public bool Position
                {
                    get
                    {
                        return (_dmFields & (int)DeviceModeFieldFlags.DM_POSITION) > 0;
                    }
                }
                #endregion

                #region Colour
                public bool Colour
                {
                    get
                    {
                        return (_dmFields | (int)DeviceModeFieldFlags.DM_COLOR) > 0;
                    }
                }
                #endregion

                #region Duplex
                public bool Duplex
                {
                    get
                    {
                        return (_dmFields | (int)DeviceModeFieldFlags.DM_DUPLEX) > 0;
                    }
                }
                #endregion

                #region Collation
                public bool Collation
                {
                    get
                    {
                        return (_dmFields | (int)DeviceModeFieldFlags.DM_COLLATE) > 0;
                    }
                }
                #endregion

                #region Formname
                public bool FormName
                {
                    get
                    {
                        return (_dmFields | (int)DeviceModeFieldFlags.DM_FORMNAME) > 0;
                    }
                }
                #endregion

                #region MediaType
                public bool MediaType
                {
                    get
                    {
                        return (_dmFields | (int)DeviceModeFieldFlags.DM_MEDIATYPE) > 0;
                    }
                }
                #endregion

                #endregion

            }
            #endregion

        }
        #endregion
        #endregion
    }
    #endregion

    #region PCL XL Spool File
    /// -----------------------------------------------------------------------------
    /// Project	 : PrinterQueueWatch
    /// Class	 : PCLXLSpoolFile
    /// 
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Helper class for PCL XL format spool file
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	13/12/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
    internal class PCLXLSpoolFile
    {

    }
}
#endregion

#endregion
