'\\ --[PrintJobEvent]------------------------------------
'\\ Holds the information about a single print job event
'\\ (job added, deleted, set or written) for display in
'\\ the print job history form
'\\ ----------------------------------------------------
Imports PrinterQueueWatch
Imports System.Collections

Public Class PrintJobEvent

#Region "Public enumerated types"
    Public Enum PrintJobEventTypes
        JobAddedEvent
        JobDeletedEvent
        JobSetEvent     '--not implemented in this version of the component
        JobWrittenEvent '--not implemented in this version of the component
    End Enum
#End Region

#Region "Private member variables"
    Private _EventType As PrintJobEventTypes

    Private _JobId As Integer
    Private _PrinterName As String
    Private _MachineName As String
    Private _UserName As String
    Private _NotifyName As String
    Private _Priority As Int32
    Private _Position As Int32
    Private _TotalPages As Int32
    Private _PagesPrinted As Int32

#End Region

#Region "Public properties"
#Region "EventType"
    Public ReadOnly Property EventType() As String
        Get
            'TODO : For an international app replace these strings with 
            Select Case _EventType
                Case PrintJobEventTypes.JobAddedEvent
                    Return "Job added"
                Case PrintJobEventTypes.JobDeletedEvent
                    Return "Job Deleted"
                Case PrintJobEventTypes.JobSetEvent
                    Return "Job Set"
                Case PrintJobEventTypes.JobWrittenEvent
                    Return "Job Written"
            End Select
        End Get
    End Property
#End Region

#Region "JobId"
    Public ReadOnly Property JobId() As Integer
        Get
            Return _JobId
        End Get
    End Property
#End Region

    Public ReadOnly Property Printername() As String
        Get
            Return _PrinterName
        End Get
    End Property

    Public ReadOnly Property Machinename() As String
        Get
            Return _MachineName
        End Get
    End Property

    Public ReadOnly Property Username() As String
        Get
            Return _UserName
        End Get
    End Property

    Public ReadOnly Property NotifyUsername() As String
        Get
            Return _NotifyName
        End Get
    End Property



    Public ReadOnly Property Priority() As Integer
        Get
            Return _Priority
        End Get
    End Property

    Public ReadOnly Property Position() As Integer
        Get
            Return _Position
        End Get
    End Property

    Public ReadOnly Property PagesPrinted() As Integer
        Get
            Return _PagesPrinted
        End Get
    End Property

    Public ReadOnly Property TotalPages() As Integer
        Get
            Return _TotalPages
        End Get
    End Property
#End Region

#Region "Public constructor"
    Public Sub New(ByVal EventType As PrintJobEventTypes, ByVal PrintJob As PrintJob)
        _EventType = EventType
        With PrintJob
            _JobId = .JobId
            _PrinterName = .PrinterName
            _MachineName = .MachineName
            _UserName = .UserName
            _NotifyName = .NotifyUserName
            _Priority = .Priority
            _Position = .Position
            _TotalPages = .TotalPages
            _PagesPrinted = .PagesPrinted
        End With
        If ApplicationSettings.ApplicationTracing.TraceVerbose Then
            Trace.WriteLine(_EventType.ToString & ": " & _PrinterName)
        End If
    End Sub
#End Region

#Region "ToString override"
    Public Overrides Function ToString() As String
        Dim sOut As New System.Text.StringBuilder
        Select Case _EventType
            Case PrintJobEventTypes.JobAddedEvent
                sOut.Append("New print job added ")
            Case PrintJobEventTypes.JobDeletedEvent
                sOut.Append("Print job deleted ")
            Case PrintJobEventTypes.JobSetEvent
                sOut.Append("Print job details changed ")
            Case PrintJobEventTypes.JobWrittenEvent
                sOut.Append("Print job written ")
        End Select
        sOut.Append("on printer ")
        sOut.Append(_PrinterName)
        sOut.Append(" by user ")
        sOut.Append(_UserName)
        sOut.Append(" on machine ")
        sOut.Append(_MachineName)
        sOut.Append(", pages printed ")
        sOut.Append(_PagesPrinted.ToString)
        sOut.Append(" of total ")
        sOut.Append(_TotalPages.ToString)
        Return sOut.ToString
    End Function
#End Region

End Class

'\\ --[PrintJobEventList]---------------------------------
'\\ Collection of print job events that can be bound to a 
'\\ data grid.
'\\ ------------------------------------------------------
Public Class PrintJobEventList
    Inherits ArrayList

#Region "ArrayList overloads"
    Public Overloads Sub Add(ByVal Value As PrintJobEvent)
        MyBase.Add(Value)
    End Sub

    Public Overrides ReadOnly Property IsReadOnly() As Boolean
        Get
            Return True
        End Get
    End Property

    Default Public Shadows Property Item(ByVal index As Integer) As PrintJobEvent
        Get
            Return CType(MyBase.Item(index), PrintJobEvent)
        End Get
        Set(ByVal Value As PrintJobEvent)
            MyBase.Item(index) = Value
        End Set
    End Property


#End Region

End Class
