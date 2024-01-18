'\\ --[PrinterMonitorComponent]-------------------------
'\\ Top level component to allow plug-in monitoring of
'\\ the windows print spool for a given printer 
'\\ (c) Merrion Computing Ltd 
'\\     http://www.merrioncomputing.com
'\\ ----------------------------------------------------
Imports System.Runtime.InteropServices
Imports System.ComponentModel
Imports System.Threading
Imports System.Collections
Imports PrinterQueueWatch.SpoolerApiConstantEnumerations


<ToolboxBitmap(GetType(PrinterQueueWatch.PrinterMonitorComponent), "toolboximage.bmp")> _
Public Class PrinterMonitorComponent
    Inherits System.ComponentModel.Component

#Region "Tracing"
    Public Shared ComponentTraceSwitch As New TraceSwitch("PrinterMonitorComponent", "Printer Monitor Component Tracing")
#End Region

#Region "Localisation"
    Public Shared ComponentLocalisationResourceManager As New Resources.ResourceManager("PrinterMonitorComponent", System.Reflection.Assembly.Load("PrinterQueueWatch.Resources"))
#End Region

#Region "Public enumerated types"

    Public Enum MonitorJobEventInformationLevels
        MaximumJobInformation = 1
        MinimumJobInformation = 2
        NoJobInformation = 3
    End Enum

#End Region

#Region "Private constants"

#If EVALUATION Then
    Private Const LICENSE_EXPIRY_DATE As Date = #02/02/2004#
#End If
    Private Const DEFAULT_THREAD_TIMEOUT As Integer = 500
    Private Const PRINTER_NOTIFY_OPTIONS_REFRESH = &H1
#End Region

#Region "Private Member Variables"

    '\\ Printer handle - returned by the OpenPrinter API call
    Private mhPrinter As IntPtr
    Private msDeviceName As String

    '\\ A combination of PrinterChangeNotificationGeneralFlags that describe what to monitor
    Private _WatchFlags As Integer = PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_JOB Or PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_PRINTER

    Private _MonitorJobInfoLevel As MonitorJobEventInformationLevels = MonitorJobEventInformationLevels.MaximumJobInformation

    Private _ThreadTimeout As Integer = DEFAULT_THREAD_TIMEOUT

    Private piOut As PrinterInformation

    Private _MonitoredPrinters As MonitoredPrinters

#End Region

#Region "Public events"

#Region "Job events"

    Public Event JobAdded As EventHandler
    Public Event JobDeleted As EventHandler
    Public Event JobWritten As EventHandler
    Public Event JobSet As EventHandler

    Protected Sub OnJobEvent(ByVal e As PrintJobEventArgs)
        If e.EventType = PrintJobEventArgs.PrintJobEventTypes.JobAddedEvent Then
            RaiseEvent JobAdded(Me, e)
        ElseIf e.EventType = PrintJobEventArgs.PrintJobEventTypes.JobSetEvent Then
            RaiseEvent JobSet(Me, e)
        ElseIf e.EventType = PrintJobEventArgs.PrintJobEventTypes.JobWrittenEvent Then
            RaiseEvent JobWritten(Me, e)
        ElseIf e.EventType = PrintJobEventArgs.PrintJobEventTypes.JobDeletedEvent Then
            RaiseEvent JobDeleted(Me, e)
        End If
    End Sub

#End Region

#Region "Printer events"
    Public Event PrinterInformationChanged As EventHandler
    Protected Sub OnPrinterInformationChanged(ByVal e As PrinterEventArgs)
        RaiseEvent PrinterInformationChanged(Me, e)
    End Sub
#End Region

#End Region

#Region "Public delegates"
    '\\ Print job events
    Public Delegate Sub JobEvent(ByVal e As PrintJobEventArgs)
    '\\ Print server events
    Public Delegate Sub PrinterEvent(ByVal e As PrinterEventArgs)

#End Region



    Public ReadOnly Property Monitoring() As Boolean
        Get
            If _MonitoredPrinters Is Nothing Then
                Return False
            Else
                Return (_MonitoredPrinters.Count > 0)
            End If
        End Get
    End Property


#Region "Public interface"

#Region "DeviceName"
    <Browsable(False)> _
    Public Property DeviceName() As String
        Set(ByVal Value As String)
            Dim tmpHandle As Int32

            '\\ Set the callbacks for the printers list
            If _MonitoredPrinters Is Nothing Then
                _MonitoredPrinters = New MonitoredPrinters(AddressOf OnPrinterInformationChanged, AddressOf OnJobEvent)
            End If

            '\\ If the name changes, destroy and recreate the printer watch worker
            If Not Value.Equals(msDeviceName) Then
                If msDeviceName <> "" Then
                    _MonitoredPrinters.Remove(msDeviceName)
                End If
                msDeviceName = Value

                '\\ Only need to watch a printer in runtime mode...
                If MyBase.DesignMode = False Then
                    If msDeviceName <> "" Then
                        _MonitoredPrinters.Add(msDeviceName, New PrinterInformation(msDeviceName, PrinterAccessRights.PRINTER_ALL_ACCESS Or PrinterAccessRights.SERVER_ALL_ACCESS, _ThreadTimeout, _MonitorJobInfoLevel, _WatchFlags))
                    End If
                End If
            End If
        End Set
        Get
            If _MonitoredPrinters.Count > 0 Then
                Return _MonitoredPrinters.Item(0).PrinterName
            Else
                Return ""
            End If
        End Get
    End Property
#End Region

#Region "PrintJobs"
    <Browsable(False)> _
    Public Overloads ReadOnly Property PrintJobs() As PrintJobCollection
        Get
            If _MonitoredPrinters.Count > 0 Then
                Return _MonitoredPrinters(0).PrintJobs
            End If
        End Get
    End Property
    <Browsable(False)> _
    Public Overloads ReadOnly Property PrintJobs(ByVal DeviceName As String) As PrintJobCollection
        Get
            If _MonitoredPrinters.Contains(DeviceName) Then
                Return _MonitoredPrinters(DeviceName).PrintJobs
            End If
        End Get
    End Property
#End Region

#Region "Printer Information"
    <Browsable(False)> _
    Public Overloads ReadOnly Property PrinterInformation() As PrinterInformation
        Get
            If _MonitoredPrinters.Count > 0 Then
                Return _MonitoredPrinters(0)
            End If
        End Get
    End Property

    <Browsable(False)> _
        Public Overloads ReadOnly Property PrinterInformation(ByVal DeviceName As String) As PrinterInformation
        Get
            If _MonitoredPrinters.Count > 0 Then
                Return _MonitoredPrinters(DeviceName)
            End If
        End Get
    End Property
#End Region

#Region "Printer info properties"

    <Description("The number of jobs on the queued on the printer being monitored")> _
    Public ReadOnly Property JobCount() As Int32
        Get
            If mhPrinter.ToInt32 <> 0 Then
                Return Me.PrinterInformation.JobCount
            Else
                Return 0
            End If
        End Get
    End Property

#End Region

#Region "AddPrinter"
    <Description("Adds the printer to the internal list and strats monitoring it")> _
    Public Sub AddPrinter(ByVal DeviceName As String)
        If PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose Then
            Trace.WriteLine("AddPrinter(" & DeviceName & ")", Me.GetType.ToString)
        End If
        If _MonitoredPrinters Is Nothing Then
            _MonitoredPrinters = New MonitoredPrinters(AddressOf OnPrinterInformationChanged, AddressOf OnJobEvent)
        End If
        _MonitoredPrinters.Add(DeviceName, New PrinterInformation(DeviceName, PrinterAccessRights.PRINTER_ALL_ACCESS Or PrinterAccessRights.SERVER_ALL_ACCESS, _ThreadTimeout, _MonitorJobInfoLevel, _WatchFlags))
    End Sub
#End Region

#Region "RemovePrinter"
    <Description("Removes a printer from the internal list, stopping monitoring as appropriate ")> _
    Public Sub RemovePrinter(ByVal DeviceName As String)
        If PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose Then
            Trace.WriteLine("RemovePrinter(" & DeviceName & ")", Me.GetType.ToString)
        End If
        _MonitoredPrinters.Remove(DeviceName)
    End Sub
#End Region

#End Region

#Region " Component Designer generated code "

    Public Sub New(ByVal Container As System.ComponentModel.IContainer)
        MyClass.New()

        'Required for Windows.Forms Class Composition Designer support
        Container.Add(Me)
    End Sub

    Public Sub New()
        MyBase.New()

        'This call is required by the Component Designer.
        InitializeComponent()

        '\\ Checking license
#If EVALUATION Then
        If System.DateTime.Now > LICENSE_EXPIRY_DATE Then
            Trace.WriteLine(ComponentLocalisationResourceManager.GetString("ev_expired")  & LICENSE_EXPIRY_DATE)
            Trace.WriteLine(ComponentLocalisationResourceManager.GetString("ev_contact"))  
            Throw new LicenseException(me.GetType)
        Else
            Trace.WriteLine(ComponentLocalisationResourceManager.GetString("ev_expires") & LICENSE_EXPIRY_DATE)
        End If
#End If


        '\\ Setting the tracing stuff
        Trace.AutoFlush = True

    End Sub

    'Component overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not _MonitoredPrinters Is Nothing Then
                _MonitoredPrinters.Clear()
                _MonitoredPrinters = Nothing
            End If
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        components = New System.ComponentModel.Container()
    End Sub

#End Region

#Region "Design/pre watching interface"

    <Category("Performance Tuning"), _
    Description("Set to make the component monitor jobs being added to the job queue"), _
    DefaultValue(GetType(Boolean), "True")> _
    Public Property MonitorJobAddedEvent() As Boolean
        Get
            Return (_WatchFlags And PrinterChangeNotificationJobFlags.PRINTER_CHANGE_ADD_JOB)
        End Get
        Set(ByVal Value As Boolean)
            If Monitoring Then
                Throw New ReadOnlyException("This property cannot be set once the component is monitoring a print queue")
            Else
                If Value Then
                    _WatchFlags = _WatchFlags Or PrinterChangeNotificationJobFlags.PRINTER_CHANGE_ADD_JOB
                Else
                    _WatchFlags = _WatchFlags And Not (PrinterChangeNotificationJobFlags.PRINTER_CHANGE_ADD_JOB)
                End If
            End If
        End Set
    End Property

    <Category("Performance Tuning"), _
    Description("Set to make the component monitor jobs being removed from the job queue"), _
      DefaultValue(GetType(Boolean), "True")> _
    Public Property MonitorJobDeletedEvent() As Boolean
        Get
            Return (_WatchFlags And PrinterChangeNotificationJobFlags.PRINTER_CHANGE_DELETE_JOB)
        End Get
        Set(ByVal Value As Boolean)
            If Monitoring Then
                Throw New ReadOnlyException("This property cannot be set once the component is monitoring a print queue")
            Else
                If Value Then
                    _WatchFlags = _WatchFlags Or PrinterChangeNotificationJobFlags.PRINTER_CHANGE_DELETE_JOB
                Else
                    _WatchFlags = _WatchFlags And Not (PrinterChangeNotificationJobFlags.PRINTER_CHANGE_DELETE_JOB)
                End If
            End If
        End Set
    End Property

    <Category("Performance Tuning"), _
    Description("Set to make the component monitor jobs being written on the job queue"), _
      DefaultValue(GetType(Boolean), "True")> _
    Public Property MonitorJobWrittenEvent() As Boolean
        Get
            Return (_WatchFlags And PrinterChangeNotificationJobFlags.PRINTER_CHANGE_WRITE_JOB)
        End Get
        Set(ByVal Value As Boolean)
            If Monitoring Then
                Throw New ReadOnlyException("This property cannot be set once the component is monitoring a print queue")
            Else
                If Value Then
                    _WatchFlags = _WatchFlags Or PrinterChangeNotificationJobFlags.PRINTER_CHANGE_WRITE_JOB
                Else
                    _WatchFlags = _WatchFlags And Not (PrinterChangeNotificationJobFlags.PRINTER_CHANGE_WRITE_JOB)
                End If
            End If
        End Set
    End Property

    <Category("Performance Tuning"), _
    Description("Set to make the component monitor changes to the jobs on the job queue"), _
      DefaultValue(GetType(Boolean), "True")> _
     Public Property MonitorJobSetEvent() As Boolean
        Get
            Return (_WatchFlags And PrinterChangeNotificationJobFlags.PRINTER_CHANGE_SET_JOB)
        End Get
        Set(ByVal Value As Boolean)
            If Monitoring Then
                Throw New ReadOnlyException("This property cannot be set once the component is monitoring a print queue")
            Else
                If Value Then
                    _WatchFlags = _WatchFlags Or PrinterChangeNotificationJobFlags.PRINTER_CHANGE_SET_JOB
                Else
                    _WatchFlags = _WatchFlags And Not (PrinterChangeNotificationJobFlags.PRINTER_CHANGE_SET_JOB)
                End If
            End If
        End Set
    End Property

    <Category("Performance Tuning"), _
    Description("Set to make the component monitor printer setup change events"), _
    DefaultValue(GetType(Boolean), "True")> _
    Public Property MonitorPrinterChangeEvent() As Boolean
        Get
            Return (_WatchFlags And PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_PRINTER)
        End Get
        Set(ByVal Value As Boolean)
            If Monitoring Then
                Throw New ReadOnlyException("This property cannot be set once the component is monitoring a print queue")
            Else
                If Value Then
                    _WatchFlags = _WatchFlags Or PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_PRINTER
                Else
                    _WatchFlags = _WatchFlags And Not (PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_PRINTER)
                End If
            End If
        End Set
    End Property

    <Category("Performance Tuning"), _
    Description("Set to fine tune the job information required for networks"), _
    DefaultValue(MonitorJobEventInformationLevels.MaximumJobInformation)> _
    Public Property MonitorJobEventInformationLevel() As MonitorJobEventInformationLevels
        Get
            Return _MonitorJobInfoLevel
        End Get
        Set(ByVal Value As MonitorJobEventInformationLevels)
            If Monitoring Then
                Throw New ReadOnlyException("This property cannot be set once the component is monitoring a print queue")
            Else
                _MonitorJobInfoLevel = Value
            End If
        End Set
    End Property

    <Category("Performance Tuning"), _
    Description("Set to tune the printer watch refresh interval"), _
    DefaultValue(DEFAULT_THREAD_TIMEOUT)> _
    Public Property ThreadTimeout() As Integer
        Get
            If _ThreadTimeout = 0 Or _ThreadTimeout < -1 Then
                _ThreadTimeout = DEFAULT_THREAD_TIMEOUT
            End If
            Return _ThreadTimeout
        End Get
        Set(ByVal Value As Integer)
            _ThreadTimeout = Value
        End Set
    End Property

#End Region

    Protected Overrides Sub Finalize()
        '\\ Stop watching the current printer
        Call Dispose(True)
        MyBase.Finalize()
    End Sub
End Class

#Region "PrinterEventFlagDecoder class"
Friend Class PrinterEventFlagDecoder
#Region "Private Member Variables"

    Private Const PRINTER_NOTIFY_INFO_DISCARDED As Integer = &H1

    Private mflags As Integer

#End Region

    Public ReadOnly Property IsInfoComplete() As Boolean
        Get
            Return ((mflags And PRINTER_NOTIFY_INFO_DISCARDED) = 0)
        End Get
    End Property

    Public ReadOnly Property ChangesOccured() As Boolean
        Get
            Return Not (CType(mflags, Integer) = 0)
        End Get
    End Property

    Friend ReadOnly Property JobChange() As Integer
        Get
            Return (CType(mflags, Integer) And PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_JOB)
        End Get
    End Property

    Friend ReadOnly Property PrinterChange() As Integer
        Get
            Return (CType(mflags, Integer) And PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_PRINTER)
        End Get
    End Property

    Friend ReadOnly Property ProcessorChange() As Integer
        Get
            Return (CType(mflags, Integer) And PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_PRINT_PROCESSOR)
        End Get
    End Property

    Friend ReadOnly Property DriverChange() As Integer
        Get
            Return (CType(mflags, Integer) And PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_PRINTER_DRIVER)
        End Get
    End Property

    Friend ReadOnly Property FormChange() As Integer
        Get
            Return (mflags And PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_FORM)
        End Get
    End Property

    Friend ReadOnly Property PortChange() As Integer
        Get
            Return (mflags And PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_PORT)
        End Get
    End Property

    Public ReadOnly Property JobAdded() As Boolean
        Get
            Return (mflags And PrinterChangeNotificationJobFlags.PRINTER_CHANGE_ADD_JOB)
        End Get
    End Property

    Public ReadOnly Property JobDeleted() As Boolean
        Get
            Return (mflags And PrinterChangeNotificationJobFlags.PRINTER_CHANGE_DELETE_JOB)
        End Get
    End Property

    Public ReadOnly Property JobWritten() As Boolean
        Get
            Return (mflags And PrinterChangeNotificationJobFlags.PRINTER_CHANGE_WRITE_JOB)
        End Get
    End Property

    Public ReadOnly Property JobSet() As Boolean
        Get
            Return (mflags And PrinterChangeNotificationJobFlags.PRINTER_CHANGE_SET_JOB)
        End Get
    End Property

    Public Sub New(ByVal flags As Int32)
        mflags = flags
    End Sub

End Class
#End Region

#Region "MonitoredPrinters collection class"
'\\ --[MonitoredPrinters]---------------------------------------------
'\\ A type safe collection of PrinterInformation objects representing 
'\\ the printers being monitored by any given PrinterMonitorComponent
'\\ Unique key is Printer.DeviceName
'\\ (c) 2003 Merrion Computing Ltd
'\\ ------------------------------------------------------------------
Friend Class MonitoredPrinters
    Implements IDisposable

#Region "Private member variables"
    Private _PrinterList As New SortedList()
    Private _JobEvent As PrinterMonitorComponent.JobEvent
    Private _PrinterEvent As PrinterMonitorComponent.PrinterEvent

#End Region

#Region "Public properties"
    Default Public Overloads ReadOnly Property Item(ByVal DeviceName As String) As PrinterInformation
        Get
            If DeviceName Is Nothing Then
                Throw New ArgumentNullException("DeviceName")
            ElseIf DeviceName = "" Then
                Throw New ArgumentException("DeviceName cannot be blank")
            Else
                Return CType(_PrinterList.Item(DeviceName), PrinterInformation)
            End If
        End Get
    End Property

    Default Public Overloads ReadOnly Property Item(ByVal Index As Integer) As PrinterInformation
        Get
            If Index <= _PrinterList.Count Then
                Return CType(_PrinterList.GetByIndex(Index), PrinterInformation)
            End If
        End Get
    End Property

    Public ReadOnly Property Count() As Integer
        Get
            Return _PrinterList.Count
        End Get
    End Property

#End Region

#Region "Public methods"
    Public Sub Add(ByVal DeviceName As String, ByVal PrinterInformation As PrinterInformation)
        If Not _PrinterList.ContainsKey(DeviceName) Then
            _PrinterList.Add(DeviceName, PrinterInformation)
            With Item(DeviceName)
                '\\ Make the PrinterInformation class know that this component is it's event target
                Call .InitialiseEventQueue(_JobEvent, _PrinterEvent)
                '\\ Make the printerinformation class start monitoring
                .Monitored = True
            End With
        End If
    End Sub

#Region "Remove"
    Public Sub Remove(ByVal DeviceName As String)
        If _PrinterList.ContainsKey(DeviceName) Then
            DirectCast(_PrinterList.Item(DeviceName), PrinterInformation).Monitored = False
            Call RemoveAt(_PrinterList.IndexOfKey(DeviceName))
        End If
    End Sub

    Public Sub RemoveAt(ByVal Index As Integer)
        _PrinterList.RemoveAt(Index)
    End Sub
#End Region
#Region "Contains"
    Public Function Contains(ByVal Devicename As String) As Boolean
        Return _PrinterList.Contains(Devicename)
    End Function
#End Region

    Public Sub Clear()
        Dim Key As String
        For Each Key In _PrinterList.Keys
            With DirectCast(_PrinterList.Item(Key), PrinterInformation)
                .Monitored = False
                .Dispose()
            End With
        Next
        _PrinterList.Clear()
    End Sub

#Region "IDisposable interface implementation"
    Public Overloads Sub Dispose() Implements IDisposable.Dispose
        If PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose Then
            Trace.WriteLine("Dispose()", Me.GetType.ToString)
        End If
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overridable Overloads Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            Call Clear()
        End If
    End Sub

    Protected Overrides Sub Finalize()
        Dispose(False)
    End Sub

#End Region

#End Region

#Region "Public constructor"
    Public Sub New(ByVal PrinterEventCallback As PrinterMonitorComponent.PrinterEvent, ByVal JobEventCallback As PrinterMonitorComponent.JobEvent)
        _PrinterEvent = PrinterEventCallback
        _JobEvent = JobEventCallback
    End Sub
#End Region
End Class
#End Region
