'\\ --[PrinterHistoryForm]--------------------------------
'\\ Displays the printer event history for the selected 
'\\ printer
'\\ (c) 2003 Merrion Computing Ltd
'\\ ------------------------------------------------------
Public Class PrinterHistoryForm
    Inherits PrintWatchTest.PrinterHeaderForm

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        MyBase.PrinterHeaderFunction = PrinterHeaderForm.PrinterHeaderFuntionTypes.History

        Me.DataGridJobHistory.DataSource = PrintWatchTestMain.PrintJobEventList

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
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
    Friend WithEvents TabControlHistory As System.Windows.Forms.TabControl
    Friend WithEvents TabPagePrintJobEvents As System.Windows.Forms.TabPage
    Friend WithEvents TabPagePrinterEvents As System.Windows.Forms.TabPage
    Friend WithEvents DataGridPrinterHistory As System.Windows.Forms.DataGrid
    Friend WithEvents DataGridJobHistory As System.Windows.Forms.DataGrid
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.TabControlHistory = New System.Windows.Forms.TabControl()
        Me.TabPagePrintJobEvents = New System.Windows.Forms.TabPage()
        Me.TabPagePrinterEvents = New System.Windows.Forms.TabPage()
        Me.DataGridPrinterHistory = New System.Windows.Forms.DataGrid()
        Me.DataGridJobHistory = New System.Windows.Forms.DataGrid()
        Me.TabControlHistory.SuspendLayout()
        Me.TabPagePrintJobEvents.SuspendLayout()
        Me.TabPagePrinterEvents.SuspendLayout()
        CType(Me.DataGridPrinterHistory, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataGridJobHistory, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TabControlHistory
        '
        Me.TabControlHistory.Alignment = System.Windows.Forms.TabAlignment.Left
        Me.TabControlHistory.Controls.AddRange(New System.Windows.Forms.Control() {Me.TabPagePrintJobEvents, Me.TabPagePrinterEvents})
        Me.TabControlHistory.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.TabControlHistory.Location = New System.Drawing.Point(0, 62)
        Me.TabControlHistory.Multiline = True
        Me.TabControlHistory.Name = "TabControlHistory"
        Me.TabControlHistory.SelectedIndex = 0
        Me.TabControlHistory.Size = New System.Drawing.Size(456, 184)
        Me.TabControlHistory.TabIndex = 1
        '
        'TabPagePrintJobEvents
        '
        Me.TabPagePrintJobEvents.Controls.AddRange(New System.Windows.Forms.Control() {Me.DataGridJobHistory})
        Me.TabPagePrintJobEvents.Location = New System.Drawing.Point(23, 4)
        Me.TabPagePrintJobEvents.Name = "TabPagePrintJobEvents"
        Me.TabPagePrintJobEvents.Size = New System.Drawing.Size(429, 176)
        Me.TabPagePrintJobEvents.TabIndex = 0
        Me.TabPagePrintJobEvents.Text = "Print Job Events"
        '
        'TabPagePrinterEvents
        '
        Me.TabPagePrinterEvents.Controls.AddRange(New System.Windows.Forms.Control() {Me.DataGridPrinterHistory})
        Me.TabPagePrinterEvents.Location = New System.Drawing.Point(23, 4)
        Me.TabPagePrinterEvents.Name = "TabPagePrinterEvents"
        Me.TabPagePrinterEvents.Size = New System.Drawing.Size(429, 176)
        Me.TabPagePrinterEvents.TabIndex = 1
        Me.TabPagePrinterEvents.Text = "Printer Events"
        '
        'DataGridPrinterHistory
        '
        Me.DataGridPrinterHistory.DataMember = ""
        Me.DataGridPrinterHistory.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridPrinterHistory.HeaderForeColor = System.Drawing.SystemColors.ControlText
        Me.DataGridPrinterHistory.Name = "DataGridPrinterHistory"
        Me.DataGridPrinterHistory.Size = New System.Drawing.Size(429, 176)
        Me.DataGridPrinterHistory.TabIndex = 0
        '
        'DataGridJobHistory
        '
        Me.DataGridJobHistory.DataMember = ""
        Me.DataGridJobHistory.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridJobHistory.HeaderForeColor = System.Drawing.SystemColors.ControlText
        Me.DataGridJobHistory.Name = "DataGridJobHistory"
        Me.DataGridJobHistory.Size = New System.Drawing.Size(429, 176)
        Me.DataGridJobHistory.TabIndex = 0
        '
        'PrinterHistoryForm
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(456, 246)
        Me.Controls.AddRange(New System.Windows.Forms.Control() {Me.TabControlHistory})
        Me.MinimumSize = New System.Drawing.Size(416, 114)
        Me.Name = "PrinterHistoryForm"
        Me.TabControlHistory.ResumeLayout(False)
        Me.TabPagePrintJobEvents.ResumeLayout(False)
        Me.TabPagePrinterEvents.ResumeLayout(False)
        CType(Me.DataGridPrinterHistory, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataGridJobHistory, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "Base form events"
    Private Sub PrinterAdministrationForm_PrinterSelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PrinterSelectionChanged

    End Sub
#End Region

End Class
