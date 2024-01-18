Public Class PrintWatchAboutForm
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        Me.LabelAppTitle.Text = System.Windows.Forms.Application.ProductName
        Me.LabelCopyright.Text = System.Windows.Forms.Application.CompanyName
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
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents LabelAppTitle As System.Windows.Forms.Label
    Friend WithEvents LabelCopyright As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(PrintWatchAboutForm))
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.LabelAppTitle = New System.Windows.Forms.Label()
        Me.LabelCopyright = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Bitmap)
        Me.PictureBox1.Location = New System.Drawing.Point(8, 8)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(136, 224)
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'LabelAppTitle
        '
        Me.LabelAppTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelAppTitle.Location = New System.Drawing.Point(152, 8)
        Me.LabelAppTitle.Name = "LabelAppTitle"
        Me.LabelAppTitle.Size = New System.Drawing.Size(288, 24)
        Me.LabelAppTitle.TabIndex = 1
        Me.LabelAppTitle.Text = "Printer Watch Demonstration"
        Me.LabelAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LabelCopyright
        '
        Me.LabelCopyright.Location = New System.Drawing.Point(152, 216)
        Me.LabelCopyright.Name = "LabelCopyright"
        Me.LabelCopyright.Size = New System.Drawing.Size(288, 16)
        Me.LabelCopyright.TabIndex = 2
        Me.LabelCopyright.Text = "(c) 2003 - Merrion Computing Ltd"
        '
        'PrintWatchAboutForm
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(448, 246)
        Me.Controls.AddRange(New System.Windows.Forms.Control() {Me.LabelCopyright, Me.LabelAppTitle, Me.PictureBox1})
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "PrintWatchAboutForm"
        Me.Text = "About MCL Printer Monitoring"
        Me.ResumeLayout(False)

    End Sub

#End Region

End Class
