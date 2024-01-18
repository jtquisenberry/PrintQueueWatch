'\\ --[PrintWatchTestApplicationSettings]--------------------
'\\ A class for reading and writing the application settings
'\\ for the PrintWatchTest application
'\\ (c) 2003 - Merrion Computing Ltd
'\\ ---------------------------------------------------------
Imports PrinterQueueWatch.PrinterMonitorComponent
Imports System.Collections

Public Class PrintWatchTestApplicationSettings

#Region "Application trace switch"
    Public Shared ApplicationTracing As New System.Diagnostics.TraceSwitch("Application", "Printer Watch test application tracing")
#End Region



#Region "Private member variables"



    '\\ -- Printers to track
    Private _MonitoredPrintersCount As Integer
    Private _MonitoredPrinters As New ArrayList()

    '\\ -- Application tracing settings
    Private _PrinterMonitorComponentTraceLevel As System.Diagnostics.TraceLevel
    Private _ApplicationTraceLevel As System.Diagnostics.TraceLevel

    '\\ -- Class state
    Private _Changed As Boolean

    Dim pPrinter As PrinterQueueWatch.PrinterMonitorComponent
#End Region

#Region "Public properties"


    Public ReadOnly Property MonitoredPrintersCount() As Integer
        Get
            If Not _MonitoredPrinters Is Nothing Then
                Return _MonitoredPrinters.Count
            End If
        End Get
    End Property

    Public ReadOnly Property MonitoredPrinterDevicename(ByVal Index As Integer) As String
        Get
            Return _MonitoredPrinters.Item(Index)
        End Get
    End Property

    Public ReadOnly Property Changed() As Boolean
        Get
            Return _Changed
        End Get
    End Property
#End Region

#Region "Public methods"
    Public Sub ReadSettings()
        Dim configSettings As New System.Configuration.AppSettingsReader()
        Dim nPrinter As Integer

        With configSettings
            _MonitoredPrintersCount = .GetValue("MonitoredPrintersCount", GetType(System.Int32))
            If _MonitoredPrintersCount > 0 Then
                For nPrinter = 1 To _MonitoredPrintersCount
                    _MonitoredPrinters.Add(.GetValue("MonitoredPrinter" & nPrinter.ToString, GetType(System.String)))
                Next nPrinter
            End If
        End With

    End Sub

    Public Sub SaveSettings()
        If _Changed Then

        End If
    End Sub

    Public Sub AddPrinterToMonitor(ByVal Devicename As String)
        If Not _MonitoredPrinters.Contains(Devicename) Then
            _MonitoredPrinters.Add(Devicename)
        End If
    End Sub

    Public Sub RemovePrinterToMonitor(ByVal DeviceName As String)
        If _MonitoredPrinters.Contains(DeviceName) Then
            _MonitoredPrinters.Remove(DeviceName)
        End If
    End Sub

#End Region

#Region "Public constructor"
    Public Sub New()
        '\\ Read in the settings for this application
        Call ReadSettings()
    End Sub
#End Region

End Class
