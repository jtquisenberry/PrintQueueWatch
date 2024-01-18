using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using PrinterQueueWatch.SpoolerApiConstantEnumerations;

namespace PrinterQueueWatch
{

    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : PrinterMonitorComponent
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// Top level component to allow plug-in monitoring of the windows print spool
/// for one or more <see cref="PrinterInformation">printers</see>
/// </summary>
/// <remarks>
/// </remarks>
/// <history>
/// 	[Duncan]	19/11/2005	Created
/// </history>
/// -----------------------------------------------------------------------------
    [ToolboxBitmap(typeof(PrinterMonitorComponent), "toolboximage.bmp")]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public class PrinterMonitorComponent : Component
    {

        #region Tracing
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Specifies the level of trace information output by the PrinterMonitorComponent
    /// </summary>
    /// <remarks>
    /// You can alter the trace switch by adding the switch to the application.exe.config
    /// </remarks>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public static TraceSwitch ComponentTraceSwitch = new TraceSwitch("PrinterMonitorComponent", "Printer Monitor Component Tracing");
        #endregion


        #region Public enumerated types
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Set to configure how much information is returned with a print job event
    /// </summary>
    /// <remarks>
    /// Typically this should be set to MaximumJobInformation except in cases of
    /// very low bandwidth networks
    /// </remarks>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public enum MonitorJobEventInformationLevels
        {
            MaximumJobInformation = 1,
            MinimumJobInformation = 2,
            NoJobInformation = 3
        }

        #endregion

        #region Private constants
        private const int DEFAULT_THREAD_TIMEOUT = 1000;
        private const int INFINITE_THREAD_TIMEOUT = int.MinValue + 0x7FFFFFFF;
        private const int PRINTER_NOTIFY_OPTIONS_REFRESH = 0x1;
        #endregion

        #region Private Member Variables

        // \\ Printer handle - returned by the OpenPrinter API call
        private IntPtr mhPrinter;
        private string msDeviceName;

        // \\ A combination of PrinterChangeNotificationGeneralFlags that describe what to monitor
        private int _WatchFlags = (int)(PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_JOB | PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_PRINTER);

        private MonitorJobEventInformationLevels _MonitorJobInfoLevel = MonitorJobEventInformationLevels.MaximumJobInformation;

        private int _ThreadTimeout = DEFAULT_THREAD_TIMEOUT;

        private PrinterInformation piOut;

        private MonitoredPrinters _MonitoredPrinters;

        private bool _SpoolMonitoringDisabled = false; // \\ Switch off spool monitoring if a communications error occurs

        public static ComponentResourceManager resources = new ComponentResourceManager(typeof(PrinterMonitorComponent));

        #endregion

        #region Public events

        #region Job events

        #region Event Delegates
        [Serializable()]
        public delegate void PrintJobEventHandler(object sender, PrintJobEventArgs e);

        [Serializable()]
        public delegate void PrinterEventHandler(object sender, PrinterEventArgs e);
        #endregion

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Raised when a job is added to one of the print spool queues being monitored
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// -----------------------------------------------------------------------------
        public event PrintJobEventHandler JobAdded;

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Raised when a job is removed from one of the print spool queues being monitored
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// -----------------------------------------------------------------------------
        public event PrintJobEventHandler JobDeleted;

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Raised when a job is written to on one of the print spool queues being monitored
    /// </summary>
    /// <remarks>
    /// This event is fired when an application writes to the spool file or when a spool
    /// file writes to the print device
    /// </remarks>
    /// -----------------------------------------------------------------------------
        public event PrintJobEventHandler JobWritten;

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Raised when a job's properties are changed in one of the print spool queues being monitored
    /// </summary>
    /// <remarks>
    /// Be careful altering the print job in response to this event as you might get an endless loop
    /// </remarks>
    /// -----------------------------------------------------------------------------
        public event PrintJobEventHandler JobSet;

        #region PrintJobSpoolfileParsed
        /* TODO ERROR: Skipped IfDirectiveTrivia
        #If SPOOL_MONITORING_ENABLED Then
        *//* TODO ERROR: Skipped DisabledTextTrivia
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' Raised when a request to parse a print job spool file completes
            ''' </summary>
            ''' <remarks>
            ''' Spool file parsing is asynchronous and non blocking therefore this event may 
            ''' occur some time after the request was sent
            ''' </remarks>
            ''' -----------------------------------------------------------------------------
            Public Event PrintJobSpoolfileParsed As EventHandler
        *//* TODO ERROR: Skipped EndIfDirectiveTrivia
        #End If
        */
        #endregion

        protected void OnJobEvent(PrintJobEventArgs e)
        {
            if (e.EventType == PrintJobEventArgs.PrintJobEventTypes.JobAddedEvent)
            {
                JobAdded?.Invoke(this, e);
            }

            else if (e.EventType == PrintJobEventArgs.PrintJobEventTypes.JobSetEvent)
            {
                JobSet?.Invoke(this, e);
            }
            else if (e.EventType == PrintJobEventArgs.PrintJobEventTypes.JobWrittenEvent)
            {
                JobWritten?.Invoke(this, e);
            }
            else if (e.EventType == PrintJobEventArgs.PrintJobEventTypes.JobDeletedEvent)
            {
                JobDeleted?.Invoke(this, e);
            }
        }

        #region OnSpoolfileParsed
        /* TODO ERROR: Skipped IfDirectiveTrivia
        #If SPOOL_MONITORING_ENABLED Then
        *//* TODO ERROR: Skipped DisabledTextTrivia
            Protected Sub OnSpoolfileParsed(ByVal e As SpoolResponseEventArgs)
                RaiseEvent PrintJobSpoolfileParsed(Me, e)
            End Sub
        *//* TODO ERROR: Skipped EndIfDirectiveTrivia
        #End If
        */
        #endregion

        #endregion

        #region Printer events
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Raised when the properties of a printer being monitored are changed
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// -----------------------------------------------------------------------------
        public event PrinterEventHandler PrinterInformationChanged;
        protected void OnPrinterInformationChanged(PrinterEventArgs e)
        {
            PrinterInformationChanged?.Invoke(this, e);
        }
        #endregion

        #endregion

        #region Public delegates
        // \\ Print job events
        public delegate void JobEvent(PrintJobEventArgs e);
        // \\ Print server events
        public delegate void PrinterEvent(PrinterEventArgs e);
        #endregion

        #region Public interface

        #region Monitoring
        /// <summary>
    /// True if the component is monitoring any printers
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public bool Monitoring
        {
            get
            {
                if (_MonitoredPrinters is null)
                {
                    return false;
                }
                else
                {
                    return _MonitoredPrinters.Count > 0;
                }
            }
        }
        #endregion

        #region DeviceName
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Set to the name of the printer to monitor
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This property is for backward compatibility with the component versions which 
    /// did not support monitoring multiple printers.  
    /// Replaced by AddPrinter
    /// </remarks>
    /// <exception cref="System.ComponentModel.Win32Exception">
    /// Thrown when the printer does not exist or the user has no access rights to monitor it
    /// </exception>
    /// <seealso cref="PrinterMonitorComponent.AddPrinter"/>
    /// <seealso cref="PrinterMonitorComponent.RemovePrinter"/>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DeviceName
        {
            set
            {

                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                // \\ Set the callbacks for the printers list
                if (_MonitoredPrinters is null)
                {
                    /* TODO ERROR: Skipped IfDirectiveTrivia
                    #If SPOOL_MONITORING_ENABLED Then
                    *//* TODO ERROR: Skipped DisabledTextTrivia
                                    _MonitoredPrinters = New MonitoredPrinters(AddressOf OnPrinterInformationChanged, AddressOf OnJobEvent, AddressOf OnSpoolfileParsed)
                    *//* TODO ERROR: Skipped ElseDirectiveTrivia
                    #Else
                    */
                    _MonitoredPrinters = new MonitoredPrinters(OnPrinterInformationChanged, OnJobEvent);
                    /* TODO ERROR: Skipped EndIfDirectiveTrivia
                    #End If
                    */
                }

                // \\ If the name changes, destroy and recreate the printer watch worker
                if (!value.Equals(msDeviceName))
                {
                    if (!string.IsNullOrEmpty(msDeviceName))
                    {
                        _MonitoredPrinters.Remove(msDeviceName);
                    }
                    msDeviceName = value;

                    // \\ Only need to watch a printer in runtime mode...
                    if (DesignMode == false)
                    {
                        if (!string.IsNullOrEmpty(msDeviceName))
                        {
                            _MonitoredPrinters.Add(msDeviceName, new PrinterInformation(msDeviceName, PrinterAccessRights.PRINTER_ALL_ACCESS | PrinterAccessRights.SERVER_ALL_ACCESS, _ThreadTimeout, _MonitorJobInfoLevel, _WatchFlags));
                        }
                    }
                }
            }
            get
            {
                if (_MonitoredPrinters is not null || _MonitoredPrinters.Count > 0)
                {
                    return _MonitoredPrinters[0].PrinterName;
                }
                else
                {
                    return "";
                }
            }
        }
        #endregion

        #region PrintJobs
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Retruns the print job collection of the printer being monitored
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// If more that one printer is being monitored this will return the print jobs 
    /// of the first one added.  Use the overloaded version to get the named print device's print jobs
    /// </remarks>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Browsable(false)]
        public PrintJobCollection PrintJobs
        {
            get
            {
                if (_MonitoredPrinters.Count > 0)
                {
                    return _MonitoredPrinters[0].PrintJobs;
                }
                else
                {
                    return new PrintJobCollection();
                }
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Retruns the print job collection of the named printer being monitored
    /// </summary>
    /// <param name="DeviceName">The name of the printer being monitored that you want to retrieve the print jobs for</param>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrintJobCollection get_PrintJobs(string DeviceName)
        {
            if (_MonitoredPrinters.Contains(DeviceName))
            {
                return _MonitoredPrinters[DeviceName].PrintJobs;
            }
            else
            {
                return new PrintJobCollection();
            }
        }
        #endregion

        #region Printer Information
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Returns the printer settings for the printer being monitored
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// If more that one printer is being monitored this will return the details 
    /// of the first one added.  Use the overloaded version to get the named print device settings
    /// </remarks>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Browsable(false)]
        public PrinterInformation PrinterInformation
        {
            get
            {
                if (_MonitoredPrinters.Count > 0)
                {
                    return _MonitoredPrinters[0];
                }
                else
                {
                    return null;
                }
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Returns the printer settings for the named printer being monitored
    /// </summary>
    /// <param name="DeviceName">The name of the print device to return the information for</param>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrinterInformation get_PrinterInformation(string DeviceName)
        {
            if (_MonitoredPrinters.Count > 0)
            {
                return _MonitoredPrinters[DeviceName];
            }
            else
            {
                throw new ArgumentException("Printer information not found for this device");
            }
        }
        #endregion

        #region Printer info properties

        [Description("The number of jobs on the queued on the printer being monitored")]
        public int JobCount
        {
            get
            {
                if (mhPrinter.ToInt64() != 0L)
                {
                    return PrinterInformation.JobCount;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion

        #region AddPrinter
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Adds the printer to the internal list and starts monitoring it
    /// </summary>
    /// <param name="DeviceName">The name of the printer to monitor.
    /// This can be a UNC name or the name of a printer share
    /// </param>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="System.ComponentModel.Win32Exception">
    /// Thrown when the printer does not exist or the user has no access rights to monitor it
    /// </exception>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Description("Adds the printer to the internal list and starts monitoring it")]
        public void AddPrinter(string DeviceName)
        {
            if (ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("AddPrinter(" + DeviceName + ")", GetType().ToString());
            }
            if (_MonitoredPrinters is null)
            {
                /* TODO ERROR: Skipped IfDirectiveTrivia
                #If SPOOL_MONITORING_ENABLED Then
                *//* TODO ERROR: Skipped DisabledTextTrivia
                            _MonitoredPrinters = New MonitoredPrinters(AddressOf OnPrinterInformationChanged, AddressOf OnJobEvent, AddressOf OnSpoolfileParsed)
                *//* TODO ERROR: Skipped ElseDirectiveTrivia
                #Else
                */
                _MonitoredPrinters = new MonitoredPrinters(OnPrinterInformationChanged, OnJobEvent);
                /* TODO ERROR: Skipped EndIfDirectiveTrivia
                #End If
                */
            }
            // \\ Find out if it is a network printer
            var piTest = new PrinterInformation(DeviceName, PrinterAccessRights.PRINTER_ACCESS_USE | PrinterAccessRights.READ_CONTROL, false, false);
            try
            {
                if (piTest.IsNetworkPrinter)
                {
                    _MonitoredPrinters.Add(DeviceName, new PrinterInformation(DeviceName, PrinterAccessRights.PRINTER_ALL_ACCESS | PrinterAccessRights.SERVER_ALL_ACCESS | PrinterAccessRights.READ_CONTROL, _ThreadTimeout, _MonitorJobInfoLevel, _WatchFlags));
                }
                else
                {
                    _MonitoredPrinters.Add(DeviceName, new PrinterInformation(DeviceName, PrinterAccessRights.PRINTER_ALL_ACCESS | PrinterAccessRights.READ_CONTROL, _ThreadTimeout, _MonitorJobInfoLevel, _WatchFlags));
                }
            }
            catch (Win32Exception ea)
            {
                if (ComponentTraceSwitch.TraceVerbose)
                {
                    Trace.WriteLine("AddPrinter(" + DeviceName + ") failed. " + ea.ToString(), GetType().ToString());
                }
            }
            piTest.Dispose();

        }
        #endregion

        #region RemovePrinter
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Removes a printer from the internal list, stopping monitoring as appropriate
    /// </summary>
    /// <param name="DeviceName">The name of the printer to remove and stop monitoring</param>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Description("Removes a printer from the internal list, stopping monitoring as appropriate ")]
        public void RemovePrinter(string DeviceName)
        {
            if (ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("RemovePrinter(" + DeviceName + ")", GetType().ToString());
            }
            _MonitoredPrinters.Remove(DeviceName);
        }
        #endregion

        #region Disconnect
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Disconnects from all printers being monitored
    /// </summary>
    /// <remarks>
    /// You should disconnect from all the printers being monitored before exiting 
    /// your application to ensure all the resources are released cleanly
    /// </remarks>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Description("Disconnects from all printers being monitored")]
        public void Disconnect()
        {
            if (_MonitoredPrinters is not null)
            {
                int nCount = _MonitoredPrinters.Count;
                for (int n = nCount; n >= 1; n -= 1)
                    _MonitoredPrinters[n - 1].Monitored = false;
                _MonitoredPrinters.Clear();
                _MonitoredPrinters = null;
            }
        }
        #endregion

        #region RequestSpoolParse
        /* TODO ERROR: Skipped IfDirectiveTrivia
        #If SPOOL_MONITORING_ENABLED Then
        *//* TODO ERROR: Skipped DisabledTextTrivia
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' Sent to try and get the print job parsed asynchronously by the
            ''' SpoolMonitorService (if installed)
            ''' </summary>
            ''' <param name="Printername">The printer which is printing the job you want to parse</param>
            ''' <param name="JobId">The job number of the job you want to parse</param>
            ''' <param name="GetPageCount">True if you want to get the page count from the spool file</param>
            ''' <remarks>
            ''' Parsing the spool file is a slow operation and is undertaken asynchronously
            ''' </remarks>
            ''' <seealso cref="PrinterMonitorComponent.PrintJobSpoolfileParsed"/>
            ''' <history>
            ''' 	[Duncan]	19/11/2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Sub RequestSpoolParse(ByVal Printername As String, _
                                         ByVal JobId As Integer, _
                                         ByVal GetPageCount As Boolean)

                If ComponentTraceSwitch.TraceVerbose Then
                    Trace.WriteLine("RequestSpoolParse", Me.GetType.ToString)
                End If
                If _SpoolMonitoringDisabled Then
                    If ComponentTraceSwitch.TraceWarning Then
                        Trace.WriteLine("RequestSpoolParse : SpoolMonitoringDisabled = True", Me.GetType.ToString)
                    End If
                    Exit Sub
                End If
                If Not _MonitoredPrinters Is Nothing Then
                    If _MonitoredPrinters.Count > 0 Then
                        Try
                            _MonitoredPrinters.RequestSpoolParse(Printername, JobId, GetPageCount)
                        Catch ex As Exception
                            If ComponentTraceSwitch.TraceError Then
                                Trace.WriteLine(ex.ToString, Me.GetType.ToString)
                            End If
                            '\\ Once an error has occured talking to the spool monitor, don't try again
                            SpoolMonitoringDisabled = True
                        End Try
                    End If
                End If
            End Sub
        *//* TODO ERROR: Skipped EndIfDirectiveTrivia
        #End If
        */
        #endregion

        #region SpoolMonitoringDisabled
        /* TODO ERROR: Skipped IfDirectiveTrivia
        #If SPOOL_MONITORING_ENABLED Then
        *//* TODO ERROR: Skipped DisabledTextTrivia
            Public WriteOnly Property SpoolMonitoringDisabled() As Boolean
                Set(ByVal Value As Boolean)
                    _SpoolMonitoringDisabled = Value
                End Set
            End Property
        *//* TODO ERROR: Skipped EndIfDirectiveTrivia
        #End If
        */
        #endregion

        #endregion

        #region  Component Designer generated code 

        public PrinterMonitorComponent(IContainer Container) : this()
        {

            // Required for Windows.Forms Class Composition Designer support
            Container.Add(this);
        }

        public PrinterMonitorComponent() : base()
        {

            // This call is required by the Component Designer.
            InitializeComponent();

            // \\ Trace the version number and other info of use
            if (ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("-- Printer Monitor Component ----------------- ");
                Trace.WriteLine(" Started : " + DateTime.Now.ToLongDateString());
                Trace.WriteLine(" Version : " + Application.ProductVersion);
                Trace.WriteLine(" --------------------------------------------- ");
            }

            /* TODO ERROR: Skipped IfDirectiveTrivia
            #If SPOOL_MONITORING_ENABLED Then
            *//* TODO ERROR: Skipped DisabledTextTrivia
                    '\\ Start listening for any incoming spool responses
                    Dim _Port As Integer = 8913
                    Dim configSettings As New System.Configuration.AppSettingsReader
                    Try
                        With configSettings
                            _Port = Convert.ToInt32(.GetValue("ReturnPort", GetType(System.Int32)))
                        End With
                        SpoolReciever = New SpoolTCPReceiver(_Port)
                    Catch ex As Exception
                        If ComponentTraceSwitch.TraceError Then
                            Trace.WriteLine(ex.ToString, Me.GetType.ToString)
                        End If
                        '\\ Don't try and parse spool files if no return port specified
                        SpoolMonitoringDisabled = True
                    End Try
            *//* TODO ERROR: Skipped EndIfDirectiveTrivia
            #End If
            */
        }

        // Component overrides dispose to clean up the component list.
        protected override void Dispose(bool disposing)
        {


            if (disposing)
            {
                Disconnect();
                if (components is not null)
                {
                    try
                    {
                        components.Dispose();
                    }
                    catch (Exception ex)
                    {
                        if (ComponentTraceSwitch.TraceError)
                        {
                            Trace.WriteLine("Error in Dispose " + ex.ToString(), GetType().ToString());
                        }
                    }
                }
                if (_MonitoredPrinters is not null)
                {
                    _MonitoredPrinters.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        // Required by the Component Designer
        private IContainer components;

        // NOTE: The following procedure is required by the Component Designer
        // It can be modified using the Component Designer.
        // Do not modify it using the code editor.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {

        }

        #endregion

        #region Design/pre watching interface

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Set to make the component monitor jobs being added to the job queue
    /// </summary>
    /// <value>True to make the component raise a JobAdded event
    /// when a job is added to a printer being monitored
    /// </value>
    /// <remarks>
    /// Selecting only the notifications you want to be informed about can improve performance
    /// in low network bandwidth situations
    /// </remarks>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Category("Performance Tuning")]
        [Description("Set to make the component monitor jobs being added to the job queue")]
        [DefaultValue(typeof(bool), "True")]
        public bool MonitorJobAddedEvent
        {
            get
            {
                return (_WatchFlags & (int)PrinterChangeNotificationJobFlags.PRINTER_CHANGE_ADD_JOB) != 0;
            }
            set
            {
                if (Monitoring)
                {
                    throw new ReadOnlyException("This property cannot be set once the component is monitoring a print queue");
                }
                else if (value)
                {
                    _WatchFlags = _WatchFlags | (int)PrinterChangeNotificationJobFlags.PRINTER_CHANGE_ADD_JOB;
                }
                else
                {
                    _WatchFlags = _WatchFlags & (int)~PrinterChangeNotificationJobFlags.PRINTER_CHANGE_ADD_JOB;
                }
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Set to make the component monitor jobs being removed from the job queue
    /// </summary>
    /// <value>True to make the component raise a JobDeleted event
    /// when a job is added to a printer being monitored</value>
    /// <remarks>
    /// Selecting only the notifications you want to be informed about can improve performance
    /// in low network bandwidth situations
    /// </remarks>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Category("Performance Tuning")]
        [Description("Set to make the component monitor jobs being removed from the job queue")]
        [DefaultValue(typeof(bool), "True")]
        public bool MonitorJobDeletedEvent
        {
            get
            {
                return (_WatchFlags & (int)PrinterChangeNotificationJobFlags.PRINTER_CHANGE_DELETE_JOB) != 0;
            }
            set
            {
                if (Monitoring)
                {
                    throw new ReadOnlyException("This property cannot be set once the component is monitoring a print queue");
                }
                else if (value)
                {
                    _WatchFlags = _WatchFlags | (int)PrinterChangeNotificationJobFlags.PRINTER_CHANGE_DELETE_JOB;
                }
                else
                {
                    _WatchFlags = _WatchFlags & (int)~PrinterChangeNotificationJobFlags.PRINTER_CHANGE_DELETE_JOB;
                }
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Set to make the component monitor jobs being written on the job queue
    /// </summary>
    /// <value>True to make the component raise a JobWritten event
    /// when a job is written to on a printer being monitored</value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Category("Performance Tuning")]
        [Description("Set to make the component monitor jobs being written on the job queue")]
        [DefaultValue(typeof(bool), "True")]
        public bool MonitorJobWrittenEvent
        {
            get
            {
                return (_WatchFlags & (int)PrinterChangeNotificationJobFlags.PRINTER_CHANGE_WRITE_JOB) != 0;
            }
            set
            {
                if (Monitoring)
                {
                    throw new ReadOnlyException("This property cannot be set once the component is monitoring a print queue");
                }
                else if (value)
                {
                    _WatchFlags = _WatchFlags | (int)PrinterChangeNotificationJobFlags.PRINTER_CHANGE_WRITE_JOB;
                }
                else
                {
                    _WatchFlags = _WatchFlags & (int)~PrinterChangeNotificationJobFlags.PRINTER_CHANGE_WRITE_JOB;
                }
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Set to make the component monitor changes to the jobs on the job queue
    /// </summary>
    /// <value>True to make the component raise a JobSet event
    /// when a job is altered on a printer being monitored</value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Category("Performance Tuning")]
        [Description("Set to make the component monitor changes to the jobs on the job queue")]
        [DefaultValue(typeof(bool), "True")]
        public bool MonitorJobSetEvent
        {
            get
            {
                return (_WatchFlags & (int)PrinterChangeNotificationJobFlags.PRINTER_CHANGE_SET_JOB) != 0;
            }
            set
            {
                if (Monitoring)
                {
                    throw new ReadOnlyException("This property cannot be set once the component is monitoring a print queue");
                }
                else if (value)
                {
                    _WatchFlags = _WatchFlags | (int)PrinterChangeNotificationJobFlags.PRINTER_CHANGE_SET_JOB;
                }
                else
                {
                    _WatchFlags = _WatchFlags & (int)~PrinterChangeNotificationJobFlags.PRINTER_CHANGE_SET_JOB;
                }
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Set to make the component monitor printer setup change events
    /// </summary>
    /// <value>True to make the component raise a PrinterInformationChanged event
    /// when a printer being monitored has its settings changed</value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Category("Performance Tuning")]
        [Description("Set to make the component monitor printer setup change events")]
        [DefaultValue(typeof(bool), "True")]
        public bool MonitorPrinterChangeEvent
        {
            get
            {
                return (_WatchFlags & (int)PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_PRINTER) != 0;
            }
            set
            {
                if (Monitoring)
                {
                    throw new ReadOnlyException("This property cannot be set once the component is monitoring a print queue");
                }
                else if (value)
                {
                    _WatchFlags = _WatchFlags | (int)PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_PRINTER;
                }
                else
                {
                    _WatchFlags = _WatchFlags & (int)~PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_PRINTER;
                }
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Set to fine tune the job information required for networks
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Category("Performance Tuning")]
        [Description("Set to fine tune the job information required for networks")]
        [DefaultValue(MonitorJobEventInformationLevels.MaximumJobInformation)]
        public MonitorJobEventInformationLevels MonitorJobEventInformationLevel
        {
            get
            {
                return _MonitorJobInfoLevel;
            }
            set
            {
                if (Monitoring)
                {
                    throw new ReadOnlyException("This property cannot be set once the component is monitoring a print queue");
                }
                else
                {
                    _MonitorJobInfoLevel = value;
                }
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Set to tune the printer watch refresh interval
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This property is obsolete and only included for backward compatibility.
    /// It has no effect on the operation of the component
    /// </remarks>
    /// <history>
    /// 	[Duncan]	19/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Category("Performance Tuning")]
        [Description("Set to tune the printer watch refresh interval")]
        [DefaultValue(DEFAULT_THREAD_TIMEOUT)]
        [Obsolete("This property no longer affects the operation of the component")]
        public int ThreadTimeout
        {
            get
            {
                if (_ThreadTimeout == 0 | _ThreadTimeout < -1)
                {
                    _ThreadTimeout = INFINITE_THREAD_TIMEOUT;
                }
                return _ThreadTimeout;
            }
            set
            {
                if (ComponentTraceSwitch.TraceWarning)
                {
                    Trace.WriteLine("Obsolete property ThreadTimeout set", GetType().ToString());
                }
                _ThreadTimeout = INFINITE_THREAD_TIMEOUT;
            }
        }

        #endregion



        #region Private methods

        #region SpoolReciever.SpoolResponse
        /* TODO ERROR: Skipped IfDirectiveTrivia
        #If SPOOL_MONITORING_ENABLED Then
        *//* TODO ERROR: Skipped DisabledTextTrivia
            Private Sub SpoolReciever_SpoolResponse(ByVal sender As Object, ByVal e As System.EventArgs) Handles SpoolReciever.SpoolResponse
                If ComponentTraceSwitch.TraceVerbose OrElse SpoolSocket.SpoolSocketTraceSwitch.TraceVerbose Then
                    Trace.WriteLine("Spoolresponse recieved", Me.GetType.ToString)
                End If
                Call OnSpoolfileParsed(DirectCast(e, SpoolResponseEventArgs))
            End Sub
        *//* TODO ERROR: Skipped EndIfDirectiveTrivia
        #End If
        */
        #endregion

        #region LicenseExpiryDate
        private DateTime LicenseExpiryDate()
        {
            try
            {
                var foo = Assembly.GetAssembly(typeof(PrinterMonitorComponent));
                var fi = new FileInfo(foo.GetLoadedModules(false)[0].FullyQualifiedName);
                return fi.LastWriteTime.AddMonths(6);
            }
            catch (Exception ex)
            {
                return DateTime.Parse("2006-01-01");
            }
        }
        #endregion
        #endregion

    }

    #region PrinterEventFlagDecoder class
    internal class PrinterEventFlagDecoder
    {
        #region Private Member Variables

        private const int PRINTER_NOTIFY_INFO_DISCARDED = 0x1;

        private int mflags;

        #endregion

        #region Public interface
        /// <summary>
    /// Returns true if the printer notification message was complete
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public bool IsInfoComplete
        {
            get
            {
                return (mflags & PRINTER_NOTIFY_INFO_DISCARDED) == 0;
            }
        }

        public bool ChangesOccured
        {
            get
            {
                return !(mflags == 0);
            }
        }

        internal int JobChange
        {
            get
            {
                return mflags & (int)PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_JOB;
            }
        }

        internal int PrinterChange
        {
            get
            {
                return mflags & (int)PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_PRINTER;
            }
        }

        internal int ProcessorChange
        {
            get
            {
                return mflags & (int)PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_PRINT_PROCESSOR;
            }
        }

        internal int DriverChange
        {
            get
            {
                return mflags & (int)PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_PRINTER_DRIVER;
            }
        }

        internal int FormChange
        {
            get
            {
                return mflags & (int)PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_FORM;
            }
        }

        internal int PortChange
        {
            get
            {
                return mflags & (int)PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_PORT;
            }
        }

        #region Job Change events
        public bool JobAdded
        {
            get
            {
                return (mflags & (int)PrinterChangeNotificationJobFlags.PRINTER_CHANGE_ADD_JOB) != 0;
            }
        }

        public bool JobDeleted
        {
            get
            {
                return (mflags & (int)PrinterChangeNotificationJobFlags.PRINTER_CHANGE_DELETE_JOB) != 0;
            }
        }

        public bool JobWritten
        {
            get
            {
                return (mflags & (int)PrinterChangeNotificationJobFlags.PRINTER_CHANGE_WRITE_JOB) != 0;
            }
        }

        public bool JobSet
        {
            get
            {
                return (mflags & (int)PrinterChangeNotificationJobFlags.PRINTER_CHANGE_SET_JOB) != 0;
            }
        }
        #endregion

        #region Printer Change events
        /// <summary>
    /// A printer was added to the server being monitored
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public bool PrinterAdded
        {
            get
            {
                return (mflags & (int)PrinterChangeNotificationPrinterFlags.PRINTER_CHANGE_ADD_PRINTER) != 0;
            }
        }

        /// <summary>
    /// A printer was removed from the server being monitored
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public bool PrinterDeleted
        {
            get
            {
                return (mflags & (int)PrinterChangeNotificationPrinterFlags.PRINTER_CHANGE_DELETE_PRINTER) != 0;
            }
        }

        /// <summary>
    /// The settings of a printer on the server being monitored were changed
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public bool PrinterSet
        {
            get
            {
                return (mflags & (int)PrinterChangeNotificationPrinterFlags.PRINTER_CHANGE_SET_PRINTER) != 0;
            }
        }

        /// <summary>
    /// A printer connection error occured on the server being monitored
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public bool PrinterConnectionFailed
        {
            get
            {
                return (mflags & (int)PrinterChangeNotificationPrinterFlags.PRINTER_CHANGE_FAILED_CONNECTION_PRINTER) != 0;
            }
        }
        #endregion

        #region Server change events
        #region Server Port Events
        /// <summary>
    /// A new printer port was added to the server being monitored
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public bool ServerPortAdded
        {
            get
            {
                return (mflags & (int)PrinterChangeNotificationPortFlags.PRINTER_CHANGE_ADD_PORT) != 0;
            }
        }

        /// <summary>
    /// The settings for one of the ports on the server being monitored changed
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public bool ServerPortSet
        {
            get
            {
                return (mflags & (int)PrinterChangeNotificationPortFlags.PRINTER_CHANGE_CONFIGURE_PORT) != 0;
            }
        }

        /// <summary>
    /// A port was removed from the server being monitored
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public bool ServerPortDeleted
        {
            get
            {
                return (mflags & (int)PrinterChangeNotificationPortFlags.PRINTER_CHANGE_DELETE_PORT) != 0;
            }
        }
        #endregion

        #region Server form events
        /// <summary>
    /// A from was added to the server being monitored
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public bool ServerFormAdded
        {
            get
            {
                return (mflags & (int)PrinterChangeNotificationFormFlags.PRINTER_CHANGE_ADD_FORM) != 0;
            }
        }

        /// <summary>
    /// A form was removed from the server being monitored
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public bool ServerFormDeleted
        {
            get
            {
                return (mflags & (int)PrinterChangeNotificationFormFlags.PRINTER_CHANGE_DELETE_FORM) != 0;
            }
        }

        /// <summary>
    /// The properties of a form on the server being monitored were changed
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public bool ServerFormSet
        {
            get
            {
                return (mflags & (int)PrinterChangeNotificationFormFlags.PRINTER_CHANGE_SET_FORM) != 0;
            }
        }
        #endregion

        #region Server processor events
        /// <summary>
    /// A print processor was added to the server being monitored
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public bool ServerProcessorAdded
        {
            get
            {
                return (mflags & (int)PrinterChangeNotificationProcessorFlags.PRINTER_CHANGE_ADD_PRINT_PROCESSOR) != 0;
            }
        }

        /// <summary>
    /// A print processor was removed from the printer being monitored
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public bool ServerProcessorDeleted
        {
            get
            {
                return (mflags & (int)PrinterChangeNotificationProcessorFlags.PRINTER_CHANGE_DELETE_PRINT_PROCESSOR) != 0;
            }
        }
        #endregion

        #region Server driver events
        /// <summary>
    /// A new printer driver was added to the printer being monitored
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public bool ServerDriverAdded
        {
            get
            {
                return (mflags & (int)PrinterChangeNotificationDriverFlags.PRINTER_CHANGE_ADD_PRINTER_DRIVER) != 0;
            }
        }

        /// <summary>
    /// A printer driver was uninstalled from the server being monitored
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public bool ServerDriverDeleted
        {
            get
            {
                return (mflags & (int)PrinterChangeNotificationDriverFlags.PRINTER_CHANGE_DELETE_PRINTER_DRIVER) != 0;
            }
        }

        /// <summary>
    /// The settings for a printer driver installed on the server being monitored were changed
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public bool ServerDriverSet
        {
            get
            {
                return (mflags & (int)PrinterChangeNotificationDriverFlags.PRINTER_CHANGE_SET_PRINTER_DRIVER) != 0;
            }
        }
        #endregion

        #endregion
        #endregion

        #region Public constructor
        public PrinterEventFlagDecoder(IntPtr flags)
        {
            mflags = flags.ToInt32();
        }

        public PrinterEventFlagDecoder(int flags)
        {
            mflags = flags;
        }
        #endregion

    }
    #endregion

    #region MonitoredPrinters collection class
    /// <summary>
    /// A type safe collection of PrinterInformation objects representing 
    /// the printers being monitored by any given PrinterMonitorComponent
    /// Unique key is Printer.DeviceName
    /// </summary>
    /// <remarks></remarks>
    internal class MonitoredPrinters : IDisposable
    {

        #region Private member variables
        private System.Collections.Generic.SortedList<string, PrinterInformation> _PrinterList = new System.Collections.Generic.SortedList<string, PrinterInformation>();
        private PrinterMonitorComponent.JobEvent _JobEvent;
        private PrinterMonitorComponent.PrinterEvent _PrinterEvent;
        private SortedList _SpoolfileParsers = new SortedList();
        #endregion

        #region Public properties
        public PrinterInformation this[string DeviceName]
        {
            get
            {
                if (DeviceName is null)
                {
                    throw new ArgumentNullException("DeviceName");
                }
                else if (string.IsNullOrEmpty(DeviceName))
                {
                    throw new ArgumentException("DeviceName cannot be blank");
                }
                else
                {
                    return _PrinterList[DeviceName];
                }
            }
        }

        public PrinterInformation this[int Index]
        {
            get
            {
                return _PrinterList.Values[Index];
            }
        }

        public int Count
        {
            get
            {
                return _PrinterList.Count;
            }
        }

        #endregion

        #region Public methods
        #region Add
        public void Add(string DeviceName, PrinterInformation PrinterInformation)
        {
            if (!_PrinterList.ContainsKey(DeviceName))
            {
                _PrinterList.Add(DeviceName, PrinterInformation);
                {
                    var withBlock = this[DeviceName];
                    // \\ Make the PrinterInformation class know that this component is it's event target
                    withBlock.SetEvents(_JobEvent, _PrinterEvent);
                    // \\ Make the printerinformation class start monitoring
                    withBlock.Monitored = true;
                }
            }
        }
        #endregion
        #region Remove
        public void Remove(string DeviceName)
        {
            if (_PrinterList.ContainsKey(DeviceName))
            {
                _PrinterList[DeviceName].Monitored = false;
                RemoveAt(_PrinterList.IndexOfKey(DeviceName));
            }
            if (_SpoolfileParsers.ContainsKey(DeviceName))
            {
                _SpoolfileParsers.RemoveAt(_SpoolfileParsers.IndexOfKey(DeviceName));
            }
        }

        public void RemoveAt(int Index)
        {
            _PrinterList.Values[Index].Monitored = false;
            _PrinterList.RemoveAt(Index);
        }
        #endregion
        #region Contains
        public bool Contains(string Devicename)
        {
            return _PrinterList.ContainsKey(Devicename);
        }
        #endregion
        #region Clear
        public void Clear()
        {

            foreach (PrinterInformation p in _PrinterList.Values)
            {
                try
                {
                    p.Dispose();
                }
                catch (Exception e)
                {
                    if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                    {
                        Trace.WriteLine("Error in Dispose of " + p.PrinterName + ":: " + e.ToString(), GetType().ToString());
                    }
                }
            }
            _PrinterList.Clear();
        }
        #endregion

        #region IDisposable interface implementation
        public void Dispose()
        {
            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("Dispose()", GetType().ToString());
            }
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Clear();
            }
        }

        ~MonitoredPrinters()
        {
            Dispose(false);
        }

        #endregion

        #endregion

        #region Public constructor
        public MonitoredPrinters(PrinterMonitorComponent.PrinterEvent PrinterEventCallback, PrinterMonitorComponent.JobEvent JobEventCallback)
        {
            _PrinterEvent = PrinterEventCallback;
            _JobEvent = JobEventCallback;
        }
        #endregion
    }
}
#endregion


