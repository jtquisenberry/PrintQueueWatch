'\\ --[PrintWatchTestMain]--------------------------------------------
'\\ Small application to demonstrate using the PrinterMonitorComponent
'\\ (c) 2003 Merrion Computing Ltd
'\\ ------------------------------------------------------------------
Module PrintWatchTestMain

    Public ApplicationSettings As PrintWatchTestApplicationSettings

    Public PrintJobEventList As PrintJobEventList

    Public Sub Main()

        If PrintWatchTestApplicationSettings.ApplicationTracing.TraceVerbose Then
            Trace.WriteLine("Start" & System.DateTime.Now.ToString)
        End If

        '\\ Load the application settings
        ApplicationSettings = New PrintWatchTestApplicationSettings()

        '\\ Load the print job history list
        PrintJobEventList = New PrintJobEventList()

        '\\ Show the main monitoring form
        Dim PrintWatchMainForm As New PrintWatchMainForm()
        PrintWatchMainForm.ShowDialog()
        PrintWatchMainForm.Dispose()
        If PrintWatchTestApplicationSettings.ApplicationTracing.TraceVerbose Then
            Trace.WriteLine("End" & System.DateTime.Now.ToString)
        End If

        Application.Exit()

    End Sub

End Module
