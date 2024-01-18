Public Class PrinterHeaderForm
    Inherits System.Windows.Forms.Form

    Public Enum PrinterHeaderFuntionTypes
        Administer = 0
        History = 1
        Monitoring = 2
        Reports = 3
    End Enum

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        With PrintWatchTestMain.ApplicationSettings
            If .MonitoredPrintersCount > 0 Then
                Dim nPrinter As Integer
                For nPrinter = 0 To .MonitoredPrintersCount - 1
                    ComboBoxPrinters.Items.Add(.MonitoredPrinterDevicename(nPrinter))
                Next
            End If
        End With
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
    Friend WithEvents PictureBoxFunction As System.Windows.Forms.PictureBox
    Friend WithEvents LabelFunction As System.Windows.Forms.Label
    Friend WithEvents ComboBoxPrinters As System.Windows.Forms.ComboBox
    Friend WithEvents ImageListPrintOptions As System.Windows.Forms.ImageList
    Friend WithEvents PanelPrinterSelection As System.Windows.Forms.Panel
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(PrinterHeaderForm))
        Me.PanelPrinterSelection = New System.Windows.Forms.Panel()
        Me.PictureBoxFunction = New System.Windows.Forms.PictureBox()
        Me.ComboBoxPrinters = New System.Windows.Forms.ComboBox()
        Me.LabelFunction = New System.Windows.Forms.Label()
        Me.ImageListPrintOptions = New System.Windows.Forms.ImageList(Me.components)
        Me.PanelPrinterSelection.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelPrinterSelection
        '
        Me.PanelPrinterSelection.BackColor = System.Drawing.SystemColors.Window
        Me.PanelPrinterSelection.Controls.AddRange(New System.Windows.Forms.Control() {Me.PictureBoxFunction, Me.ComboBoxPrinters, Me.LabelFunction})
        Me.PanelPrinterSelection.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelPrinterSelection.Name = "PanelPrinterSelection"
        Me.PanelPrinterSelection.Size = New System.Drawing.Size(456, 56)
        Me.PanelPrinterSelection.TabIndex = 0
        '
        'PictureBoxFunction
        '
        Me.PictureBoxFunction.Image = CType(resources.GetObject("PictureBoxFunction.Image"), System.Drawing.Bitmap)
        Me.PictureBoxFunction.Location = New System.Drawing.Point(8, 8)
        Me.PictureBoxFunction.Name = "PictureBoxFunction"
        Me.PictureBoxFunction.Size = New System.Drawing.Size(54, 43)
        Me.PictureBoxFunction.TabIndex = 0
        Me.PictureBoxFunction.TabStop = False
        '
        'ComboBoxPrinters
        '
        Me.ComboBoxPrinters.Anchor = (System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right)
        Me.ComboBoxPrinters.Location = New System.Drawing.Point(208, 16)
        Me.ComboBoxPrinters.Name = "ComboBoxPrinters"
        Me.ComboBoxPrinters.Size = New System.Drawing.Size(240, 21)
        Me.ComboBoxPrinters.TabIndex = 2
        Me.ComboBoxPrinters.Text = "(All printers)"
        '
        'LabelFunction
        '
        Me.LabelFunction.Anchor = (System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right)
        Me.LabelFunction.Location = New System.Drawing.Point(56, 16)
        Me.LabelFunction.Name = "LabelFunction"
        Me.LabelFunction.Size = New System.Drawing.Size(152, 24)
        Me.LabelFunction.TabIndex = 1
        Me.LabelFunction.Text = "Administration of"
        Me.LabelFunction.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ImageListPrintOptions
        '
        Me.ImageListPrintOptions.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit
        Me.ImageListPrintOptions.ImageSize = New System.Drawing.Size(54, 43)
        Me.ImageListPrintOptions.ImageStream = CType(resources.GetObject("ImageListPrintOptions.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageListPrintOptions.TransparentColor = System.Drawing.Color.Transparent
        '
        'PrinterHeaderForm
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(456, 246)
        Me.Controls.AddRange(New System.Windows.Forms.Control() {Me.PanelPrinterSelection})
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(416, 112)
        Me.Name = "PrinterHeaderForm"
        Me.Text = "Printer"
        Me.PanelPrinterSelection.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "Public properties"

#Region "PrinterHeaderFunction"
    Public WriteOnly Property PrinterHeaderFunction() As PrinterHeaderFuntionTypes
        Set(ByVal Value As PrinterHeaderFuntionTypes)
            Select Case Value
                Case PrinterHeaderFuntionTypes.Administer
                    PictureBoxFunction.Image = ImageListPrintOptions.Images(0)
                    LabelFunction.Text = PrintWatchTestApplicationSettings.ApplicationLocalisationResourceManager.GetString("LabelFunctionAdminister")
                Case PrinterHeaderFuntionTypes.History
                    PictureBoxFunction.Image = ImageListPrintOptions.Images(1)
                    LabelFunction.Text = PrintWatchTestApplicationSettings.ApplicationLocalisationResourceManager.GetString("LabelFunctionHistory")
                Case PrinterHeaderFuntionTypes.Monitoring
                    PictureBoxFunction.Image = ImageListPrintOptions.Images(2)
                    LabelFunction.Text = PrintWatchTestApplicationSettings.ApplicationLocalisationResourceManager.GetString("LabelFunctionMonitoring")
                Case PrinterHeaderFuntionTypes.Reports
                    PictureBoxFunction.Image = ImageListPrintOptions.Images(3)
                    LabelFunction.Text = PrintWatchTestApplicationSettings.ApplicationLocalisationResourceManager.GetString("LabelFunctionReports")
            End Select
        End Set
    End Property
#End Region

    Public WriteOnly Property AllowPrinterChange() As Boolean
        Set(ByVal Value As Boolean)
            Me.ComboBoxPrinters.Enabled = Value
        End Set
    End Property

    Public Property Devicename() As String
        Get
            Return ComboBoxPrinters.SelectedText
        End Get
        Set(ByVal Value As String)
            ComboBoxPrinters.SelectedIndex = ComboBoxPrinters.FindStringExact(Value)
        End Set
    End Property
#End Region

#Region "Public events"
    Public Event PrinterSelectionChanged As EventHandler
    Protected Sub OnPrinterSelectionChanged(ByVal NewSelection As String)
        RaiseEvent PrinterSelectionChanged(Me, New PrinterSelectionChangedEventArgs(NewSelection))
    End Sub
#End Region


    Private Sub ComboBoxPrinters_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ComboBoxPrinters.SelectedValueChanged
        OnPrinterSelectionChanged(ComboBoxPrinters.SelectedValue)
    End Sub

End Class

#Region "PrinterSelectionChangedEventArgs"
Public Class PrinterSelectionChangedEventArgs
    Inherits System.EventArgs

#Region "Private member variables"
    Private _DeviceName As String
#End Region

#Region "Public properties"
    Public ReadOnly Property Devicename() As String
        Get
            Return _DeviceName
        End Get
    End Property
#End Region

#Region "Public constructor"
    Public Sub New(ByVal Devicename As String)
        _DeviceName = Devicename
    End Sub
#End Region

End Class
#End Region