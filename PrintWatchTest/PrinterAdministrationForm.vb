'\\ --[PrinterAdministrationForm]-------------------------
'\\ Allows the PrinterInformation for the selected printer
'\\ to be modified
'\\ (c) 2003 Merrion Computing Ltd
'\\ ------------------------------------------------------
Public Class PrinterAdministrationForm
    Inherits PrintWatchTest.PrinterHeaderForm

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        MyBase.PrinterHeaderFunction = PrinterHeaderForm.PrinterHeaderFuntionTypes.Administer

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
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        components = New System.ComponentModel.Container()
    End Sub

#End Region

#Region "Base form events"
    Private Sub PrinterAdministrationForm_PrinterSelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PrinterSelectionChanged

    End Sub
#End Region

End Class
