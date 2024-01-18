'\\ --[PrintWatchMainForm]--------------------------------
'\\ Main form of the printer monitoring application demo
'\\ project
'\\ (c) 2003 Merrion Computing Ltd
'\\ ------------------------------------------------------
Imports System.Collections
Imports PrinterQueueWatch

Public Class PrintWatchMainForm
    Inherits System.Windows.Forms.Form

#Region "Private member variables"
    Dim MenusForPrinters As New SortedList()
    Friend WithEvents txtWarnings As TextBox
    Dim WithEvents PrinterMonitorComponentMain As New PrinterMonitorComponent()
#End Region

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        If Not PrintWatchTestMain.ApplicationSettings Is Nothing Then
            With PrintWatchTestMain.ApplicationSettings
                If .MonitoredPrintersCount > 0 Then
                    If .MonitoredPrintersCount = 1 Then
                        PrinterMonitorComponentMain.DeviceName = .MonitoredPrinterDevicename(0)
                    Else
                        Dim nPrinter As Integer
                        For nPrinter = 0 To .MonitoredPrintersCount - 1
                            PrinterMonitorComponentMain.DeviceName = (.MonitoredPrinterDevicename(nPrinter))
                            Exit For 'This version of the component can only monitor one printer...
                        Next
                    End If
                    Call SetStatusBarStatus(True)
                Else
                    Call SetStatusBarStatus(False)
                End If
            End With
        End If
    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        Call PrinterMonitorComponentMain.Dispose()
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents StatusBarMonitoringMain As System.Windows.Forms.StatusBar
    Friend WithEvents MainMenuPrintWatchMain As System.Windows.Forms.MainMenu
    Friend WithEvents MenuItemFile As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItemFileOptions As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItemFileExit As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItemWindow As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItemWindowArrange As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItemWindowArrangeTile As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItemWindowArrangeCascade As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItemHelp As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItemHelpIndex As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItemHelpAbout As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItemPrinters As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem2 As System.Windows.Forms.MenuItem
    Friend WithEvents txtPrintjobEvents As System.Windows.Forms.TextBox
    Friend WithEvents PanelOptionBar As System.Windows.Forms.Panel
    Protected WithEvents PrinterMonitorComponent1 As PrinterQueueWatch.PrinterMonitorComponent
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.StatusBarMonitoringMain = New System.Windows.Forms.StatusBar()
        Me.MainMenuPrintWatchMain = New System.Windows.Forms.MainMenu(Me.components)
        Me.MenuItemFile = New System.Windows.Forms.MenuItem()
        Me.MenuItemFileOptions = New System.Windows.Forms.MenuItem()
        Me.MenuItem1 = New System.Windows.Forms.MenuItem()
        Me.MenuItemFileExit = New System.Windows.Forms.MenuItem()
        Me.MenuItemPrinters = New System.Windows.Forms.MenuItem()
        Me.MenuItemWindow = New System.Windows.Forms.MenuItem()
        Me.MenuItemWindowArrange = New System.Windows.Forms.MenuItem()
        Me.MenuItemWindowArrangeTile = New System.Windows.Forms.MenuItem()
        Me.MenuItemWindowArrangeCascade = New System.Windows.Forms.MenuItem()
        Me.MenuItemHelp = New System.Windows.Forms.MenuItem()
        Me.MenuItemHelpIndex = New System.Windows.Forms.MenuItem()
        Me.MenuItem2 = New System.Windows.Forms.MenuItem()
        Me.MenuItemHelpAbout = New System.Windows.Forms.MenuItem()
        Me.txtPrintjobEvents = New System.Windows.Forms.TextBox()
        Me.PanelOptionBar = New System.Windows.Forms.Panel()
<<<<<<< Updated upstream
        Me.PrinterMonitorComponent1 = New PrinterQueueWatch.PrinterMonitorComponent(Me.components)
        Me.txtWarnings = New System.Windows.Forms.TextBox()
=======
        Me.txtWarnings = New System.Windows.Forms.TextBox()
        Me.PrinterMonitorComponent1 = New PrinterQueueWatch.PrinterMonitorComponent(Me.components)
>>>>>>> Stashed changes
        Me.SuspendLayout()
        '
        'StatusBarMonitoringMain
        '
<<<<<<< Updated upstream
        Me.StatusBarMonitoringMain.Location = New System.Drawing.Point(0, 263)
=======
        Me.StatusBarMonitoringMain.Location = New System.Drawing.Point(0, 242)
>>>>>>> Stashed changes
        Me.StatusBarMonitoringMain.Name = "StatusBarMonitoringMain"
        Me.StatusBarMonitoringMain.Size = New System.Drawing.Size(456, 24)
        Me.StatusBarMonitoringMain.TabIndex = 1
        '
        'MainMenuPrintWatchMain
        '
        Me.MainMenuPrintWatchMain.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuItemFile, Me.MenuItemPrinters, Me.MenuItemWindow, Me.MenuItemHelp})
        '
        'MenuItemFile
        '
        Me.MenuItemFile.Index = 0
        Me.MenuItemFile.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuItemFileOptions, Me.MenuItem1, Me.MenuItemFileExit})
        Me.MenuItemFile.Text = "&File"
        '
        'MenuItemFileOptions
        '
        Me.MenuItemFileOptions.Index = 0
        Me.MenuItemFileOptions.Text = "&Options"
        '
        'MenuItem1
        '
        Me.MenuItem1.Index = 1
        Me.MenuItem1.Text = "-"
        '
        'MenuItemFileExit
        '
        Me.MenuItemFileExit.Index = 2
        Me.MenuItemFileExit.Text = "E&xit"
        '
        'MenuItemPrinters
        '
        Me.MenuItemPrinters.Index = 1
        Me.MenuItemPrinters.Text = "&Printers"
        '
        'MenuItemWindow
        '
        Me.MenuItemWindow.Index = 2
        Me.MenuItemWindow.MdiList = True
        Me.MenuItemWindow.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuItemWindowArrange})
        Me.MenuItemWindow.Text = "&Window"
        '
        'MenuItemWindowArrange
        '
        Me.MenuItemWindowArrange.Index = 0
        Me.MenuItemWindowArrange.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuItemWindowArrangeTile, Me.MenuItemWindowArrangeCascade})
        Me.MenuItemWindowArrange.Text = "&Arrange"
        '
        'MenuItemWindowArrangeTile
        '
        Me.MenuItemWindowArrangeTile.Index = 0
        Me.MenuItemWindowArrangeTile.Text = "&Tile"
        '
        'MenuItemWindowArrangeCascade
        '
        Me.MenuItemWindowArrangeCascade.Index = 1
        Me.MenuItemWindowArrangeCascade.Text = "&Cascade"
        '
        'MenuItemHelp
        '
        Me.MenuItemHelp.Index = 3
        Me.MenuItemHelp.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuItemHelpIndex, Me.MenuItem2, Me.MenuItemHelpAbout})
        Me.MenuItemHelp.Text = "&Help"
        '
        'MenuItemHelpIndex
        '
        Me.MenuItemHelpIndex.Index = 0
        Me.MenuItemHelpIndex.Text = "&Index"
        '
        'MenuItem2
        '
        Me.MenuItem2.Index = 1
        Me.MenuItem2.Text = "-"
        '
        'MenuItemHelpAbout
        '
        Me.MenuItemHelpAbout.Index = 2
        Me.MenuItemHelpAbout.Text = "&About Printer Monitor"
        '
        'txtPrintjobEvents
        '
        Me.txtPrintjobEvents.Dock = System.Windows.Forms.DockStyle.Bottom
<<<<<<< Updated upstream
        Me.txtPrintjobEvents.Location = New System.Drawing.Point(0, 223)
=======
        Me.txtPrintjobEvents.Location = New System.Drawing.Point(0, 202)
>>>>>>> Stashed changes
        Me.txtPrintjobEvents.Multiline = True
        Me.txtPrintjobEvents.Name = "txtPrintjobEvents"
        Me.txtPrintjobEvents.Size = New System.Drawing.Size(456, 40)
        Me.txtPrintjobEvents.TabIndex = 3
        '
        'PanelOptionBar
        '
        Me.PanelOptionBar.Dock = System.Windows.Forms.DockStyle.Left
        Me.PanelOptionBar.Location = New System.Drawing.Point(0, 0)
        Me.PanelOptionBar.Name = "PanelOptionBar"
<<<<<<< Updated upstream
        Me.PanelOptionBar.Size = New System.Drawing.Size(144, 223)
=======
        Me.PanelOptionBar.Size = New System.Drawing.Size(144, 202)
>>>>>>> Stashed changes
        Me.PanelOptionBar.TabIndex = 5
        '
        'txtWarnings
        '
        Me.txtWarnings.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtWarnings.ForeColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.txtWarnings.Location = New System.Drawing.Point(164, 12)
        Me.txtWarnings.Multiline = True
        Me.txtWarnings.Name = "txtWarnings"
        Me.txtWarnings.Size = New System.Drawing.Size(259, 150)
        Me.txtWarnings.TabIndex = 7
        Me.txtWarnings.Text = "The PrintWatchTest project was replaced with the PrinterQueueWatch.TestApplicatio" &
    "n.vb project. \nThe printer to monitor is configured using XML in PrintWatchTest" &
    "ApplicationSettings.vb"
        '
        'PrintWatchMainForm
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
<<<<<<< Updated upstream
        Me.ClientSize = New System.Drawing.Size(456, 287)
=======
        Me.ClientSize = New System.Drawing.Size(456, 266)
>>>>>>> Stashed changes
        Me.Controls.Add(Me.txtWarnings)
        Me.Controls.Add(Me.PanelOptionBar)
        Me.Controls.Add(Me.txtPrintjobEvents)
        Me.Controls.Add(Me.StatusBarMonitoringMain)
        Me.IsMdiContainer = True
        Me.Menu = Me.MainMenuPrintWatchMain
        Me.Name = "PrintWatchMainForm"
        Me.Text = "MCL Printer Monitor"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

#Region "Private methods"

#Region "SetStatusBarStatus"
    Private Sub SetStatusBarStatus(ByVal Active As Boolean)
        If Active Then
            Me.StatusBarMonitoringMain.Text = "Monitoring"
        Else
            Me.StatusBarMonitoringMain.Text = "Inactive"
        End If
    End Sub
#End Region

#Region "CreatePrinterMenus"
    Private Sub CreatePrintermenus()
        With PrintWatchTestMain.ApplicationSettings
            Dim nPrinter As Integer
            For nPrinter = 0 To .MonitoredPrintersCount - 1
                Dim mnuItemPrinter As MenuItem
                Dim mnuItemAdminister As MenuItem
                Dim mnuItemHistory As MenuItem
                mnuItemPrinter = MenuItemPrinters.MenuItems.Add(.MonitoredPrinterDevicename(nPrinter), AddressOf MenuItemPrinter_Click)
                ' '\\ Add an Administer submenu
                mnuItemAdminister = mnuItemPrinter.MenuItems.Add("Administer", AddressOf MenuItemPrinterAdminister_Click)
                MenusForPrinters.Add(MakeMenuKey(mnuItemAdminister), .MonitoredPrinterDevicename(nPrinter)) '

                '\\ Add a History submenu
                mnuItemHistory = mnuItemPrinter.MenuItems.Add("History", AddressOf MenuItemPrinterHistory_Click)
                MenusForPrinters.Add(MakeMenuKey(mnuItemHistory), .MonitoredPrinterDevicename(nPrinter))


            Next

        End With
    End Sub
#End Region

#Region "MakeMenuKey"
    Private Function MakeMenuKey(ByVal Item As MenuItem) As String
        Return "MenuItem:" & Item.Handle.ToString
    End Function
#End Region

#Region "ShowPrinterHistory"
    '\\ --[ShowPrinterHistory]-------------------------------
    '\\ Shows the printer history form for the given printer,
    '\\ creating a new form if neccessary
    '\\ -----------------------------------------------------
    Private Sub ShowPrinterHistory(ByVal Devicename As String)


    End Sub
#End Region

#Region "ShowPrinterAdministration"
    '\\ --[ShowPrinterAdministration]-------------------------
    '\\ Shows the printer admin form for the given printer,
    '\\ creating a new form if neccessary
    '\\ -----------------------------------------------------
    Private Sub ShowPrinterAdministration(ByVal Devicename As String)

    End Sub
#End Region
#End Region

#Region "Dynamic event handlers"
    Private Sub MenuItemPrinter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Debug.WriteLine("MenuItemPrinter")
    End Sub

    Private Sub MenuItemPrinterHistory_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Call ShowPrinterHistory(MenusForPrinters.Item(MakeMenuKey(CType(sender, MenuItem))))
    End Sub

    Private Sub MenuItemPrinterAdminister_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Call ShowPrinterAdministration(MenusForPrinters.Item(MakeMenuKey(CType(sender, MenuItem))))
    End Sub
#End Region

#Region "Menu events"
#Region "MenuItemFileExit"
    Private Sub MenuItemFileExit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuItemFileExit.Click
        Me.PrinterMonitorComponentMain.Dispose()
    End Sub
#End Region

#Region "MenuItemHelpAbout"
    Private Sub MenuItemHelpAbout_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuItemHelpAbout.Click
        Dim fAbout As New PrintWatchAboutForm()
        fAbout.TopMost = True
        fAbout.Show()
    End Sub
#End Region

#End Region

#Region "Printer monitoring events"


#Region "JobAdded"
    Private Sub PrinterMonitorComponentMain_JobAdded(sender As Object, e As PrintJobEventArgs) Handles PrinterMonitorComponentMain.JobAdded
        If PrintWatchTestMain.ApplicationSettings.ApplicationTracing.TraceVerbose Then
            Trace.WriteLine("Job Added event", Me.GetType.ToString)
        End If
        '\\ Add this job added event to the event list

        Dim Job As PrintJob = e.PrintJob
        With Job
            Try
                .Paused = True
                .Priority = 12
            Catch ex As Exception
                Debug.WriteLine(ex.ToString)
            End Try
        End With
        PrintWatchTestMain.PrintJobEventList.Add(New PrintJobEvent(PrintJobEvent.PrintJobEventTypes.JobAddedEvent, Job))
        txtPrintjobEvents.Text = PrintWatchTestMain.PrintJobEventList.Item(PrintWatchTestMain.PrintJobEventList.Count - 1).ToString
    End Sub
#End Region

#Region "JobDeleted"
    Private Sub PrinterMonitorComponentMain_JobDeleted(sender As Object, e As PrintJobEventArgs) Handles PrinterMonitorComponentMain.JobDeleted
        If PrintWatchTestMain.ApplicationSettings.ApplicationTracing.TraceVerbose Then
            Trace.WriteLine("Job Deleted event", Me.GetType.ToString)
        End If
        Dim Job As PrintJob = e.PrintJob
        With Job
            Debug.WriteLine("In Job delete " & .JobId)
        End With
        '\\ Add this job added event to the event list
        PrintWatchTestMain.PrintJobEventList.Add(New PrintJobEvent(PrintJobEvent.PrintJobEventTypes.JobDeletedEvent, Job))
        txtPrintjobEvents.Text = PrintWatchTestMain.PrintJobEventList.Item(PrintWatchTestMain.PrintJobEventList.Count - 1).ToString

    End Sub
#End Region


#End Region

End Class
