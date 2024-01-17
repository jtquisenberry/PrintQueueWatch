
namespace PrinterQueueWatch
{
    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : PrintServer
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// Class with properties pertaining to a print server
/// </summary>
/// <remarks>
/// </remarks>
/// <history>
/// 	[Duncan]	21/11/2005	Created
/// </history>
/// -----------------------------------------------------------------------------
    [System.Security.SuppressUnmanagedCodeSecurity()]
    [System.Runtime.InteropServices.ComVisible(false)]
    public class PrintServer
    {

        #region Private properties
        private string _Servername;
        private PrinterChangeNotificationThread _NotificationThread;
        #endregion

        #region Public Interface

        // -- SERVER_EVENTS ----------------------------------------------
        // Not implemented in this release
        /* TODO ERROR: Skipped IfDirectiveTrivia
        #If SERVER_EVENTS = 1 Then
        *//* TODO ERROR: Skipped DisabledTextTrivia
        #Region "Public events"

        #Region "Event Delegates"
        #Region "Printer events"
            'TODO: Add eventArgs derived classes to give extra details for these events
            <Serializable()> _
            Public Delegate Sub PrinterAddedEventHandler( _
                          ByVal sender As Object, _
                          ByVal args As EventArgs)

            <Serializable()> _
            Public Delegate Sub PrinterSetEventHandler( _
                          ByVal sender As Object, _
                          ByVal args As EventArgs)

            <Serializable()> _
            Public Delegate Sub PrinterRemovedEventHandler( _
                          ByVal sender As Object, _
                          ByVal args As EventArgs)
        #End Region

        #Region "Port events"
            'TODO: Add eventArgs derived classes to give extra details for these events
            <Serializable()> _
            Public Delegate Sub PortAddedEventHandler( _
                          ByVal sender As Object, _
                          ByVal args As EventArgs)

            <Serializable()> _
            Public Delegate Sub PortConfiguredEventHandler( _
                          ByVal sender As Object, _
                          ByVal args As EventArgs)

            <Serializable()> _
            Public Delegate Sub PortRemovedEventHandler( _
                          ByVal sender As Object, _
                          ByVal args As EventArgs)
        #End Region

        #Region "Print Processor events"
            'TODO: Add eventArgs derived classes to give extra details for these events
            <Serializable()> _
            Public Delegate Sub PrintProcessorAddedEventHandler( _
                          ByVal sender As Object, _
                          ByVal args As EventArgs)

            <Serializable()> _
            Public Delegate Sub PrintProcessorRemovedEventHandler( _
                          ByVal sender As Object, _
                          ByVal args As EventArgs)
        #End Region

        #Region "Printer driver events"
            'TODO: Add eventArgs derived classes to give extra details for these events
            <Serializable()> _
            Public Delegate Sub PrinterDriverAddedEventHandler( _
                          ByVal sender As Object, _
                          ByVal args As EventArgs)

            <Serializable()> _
            Public Delegate Sub PrinterDriverSetEventHandler( _
                          ByVal sender As Object, _
                          ByVal args As EventArgs)

            <Serializable()> _
            Public Delegate Sub PrinterDriverRemovedEventHandler( _
                          ByVal sender As Object, _
                          ByVal args As EventArgs)
        #End Region

        #Region "Form events"
            'TODO: Add eventArgs derived classes to give extra details for these events
            <Serializable()> _
            Public Delegate Sub FormAddedEventHandler( _
                          ByVal sender As Object, _
                          ByVal args As EventArgs)

            <Serializable()> _
            Public Delegate Sub FormSetEventHandler( _
                          ByVal sender As Object, _
                          ByVal args As EventArgs)

            <Serializable()> _
            Public Delegate Sub FormRemovedEventHandler( _
                          ByVal sender As Object, _
                          ByVal args As EventArgs)
        #End Region
        #End Region

        #Region "Printer Events"
        #Region "PrinterAdded"
            ''' <summary>
            ''' A <see cref="PrinterInformation">printer</see> has been added to the server being monitored
            ''' </summary>
            ''' <remarks></remarks>
            Public Event PrinterAdded As PrinterAddedEventHandler
        #End Region

        #Region "PrinterSet"
            ''' <summary>
            ''' The device settings for a <see cref="PrinterInformation">printer</see> on the server being monitored have changed
            ''' </summary>
            ''' <remarks></remarks>
            Public Event PrinterSet As PrinterSetEventHandler
        #End Region

        #Region "PrinterRemoved"
            ''' <summary>
            ''' A <see cref="PrinterInformation">printer</see> has been removed from the print server being monitored
            ''' </summary>
            ''' <remarks></remarks>
            Public Event PrinterRemoved As PrinterRemovedEventHandler
        #End Region
        #End Region

        #Region "Port events"
        #Region "PortAdded"
            ''' <summary>
            ''' A new <see cref="Port">printer port</see> was added to the server
            ''' </summary>
            ''' <remarks></remarks>
            Public Event PortAdded As PortAddedEventHandler
        #End Region

        #Region "PortConfigured"
            ''' <summary>
            ''' A <see cref="Port">port</see> was configured on the server
            ''' </summary>
            ''' <remarks></remarks>
            Public Event PortConfigured As PortConfiguredEventHandler
        #End Region

        #Region "PortRemoved"
            ''' <summary>
            ''' A <see cref="Port">port</see> was removed from the server being monitored
            ''' </summary>
            ''' <remarks></remarks>
            Public Event PortRemoved As PortRemovedEventHandler
        #End Region
        #End Region

        #Region "PrintProcessor Events"
        #Region "PrintProcessorAdded"
            ''' <summary>
            ''' A new <see cref="PrintProcessor">print processor</see> was added to the server being monitored
            ''' </summary>
            ''' <remarks></remarks>
            Public Event PrintProcessorAdded As PrintProcessorAddedEventHandler
        #End Region

        #Region "PrintProcessorRemoved"
            ''' <summary>
            ''' A <see cref="PrintProcessor">print processor</see> was removed from this server
            ''' </summary>
            ''' <remarks></remarks>
            Public Event PrintProcessorRemoved As PrintProcessorRemovedEventHandler
        #End Region

        #End Region

        #Region "PrinterDriver events"

        #Region "PrinterDriverAdded"
            ''' <summary>
            ''' A <see cref="PrinterDriver">printer driver</see> was added to the server being monitored
            ''' </summary>
            ''' <remarks></remarks>
            Public Event PrinterDriverAdded As PrinterDriverAddedEventHandler
        #End Region

        #Region "PrinterDriverSet"
            ''' <summary>
            ''' A <see cref="PrinterDriver">printer driver's</see> settings were changed on the server
            ''' </summary>
            ''' <remarks></remarks>
            Public Event PrinterDriverSet As PrinterDriverSetEventHandler
        #End Region

        #Region "PrinterDriverRemoved"
            ''' <summary>
            ''' A <see cref="PrinterDriver">printer driver</see> was removed from the server being monitored
            ''' </summary>
            ''' <remarks></remarks>
            Public Event PrinterDriverRemoved As PrinterDriverRemovedEventHandler
        #End Region

        #End Region

        #Region "PrintForm events"
        #Region "FormAdded"
            ''' <summary>
            ''' A <see cref="PrinterForm">printer form</see> was added to this server
            ''' </summary>
            ''' <remarks></remarks>
            Public Event FormAdded As FormAddedEventHandler

            ''' <summary>
            ''' A <see cref="PrinterForm">printer form</see>'s settings were changed on this server
            ''' </summary>
            ''' <remarks></remarks>
            Public Event FormSet As FormSetEventHandler

            ''' <summary>
            ''' A <see cref="PrinterForm">printer form</see> was removed from this server
            ''' </summary>
            ''' <remarks></remarks>
            Public Event FormRemoved As FormRemovedEventHandler
        #End Region
        #End Region
        #End Region
        *//* TODO ERROR: Skipped EndIfDirectiveTrivia
        #End If
        */
        #region Print Monitors
        /// <summary>
    /// The print monitors installed on this print server
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <example> Lists the print monitors on the current machine in a list box
    /// <code>
    /// 
    ///    Dim server As New PrintServer
    /// 
    ///    Me.ListBox1.Items.Clear()
    ///    For ms As Integer = 0 To server.PrintMonitors.Count - 1
    ///        Me.ListBox1.Items.Add( server.PrintMonitors(ms).Name )
    ///    Next
    /// 
    /// </code>
    /// 
    /// </example>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
        public PrintMonitors PrintMonitors
        {
            get
            {
                if (_Servername is null || string.IsNullOrEmpty(_Servername))
                {
                    return new PrintMonitors();
                }
                else
                {
                    return new PrintMonitors(_Servername);
                }
            }
        }
        #endregion

        #region Print Processors
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The print processors installed on this print server
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrintProcessorCollection PrintProcessors
        {
            get
            {
                if (_Servername is null || string.IsNullOrEmpty(_Servername))
                {
                    return new PrintProcessorCollection();
                }
                else
                {
                    return new PrintProcessorCollection(_Servername);
                }
            }
        }
        #endregion

        #region Printer Drivers
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The printer drivers installed on this print server
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrinterDriverCollection PrinterDrivers
        {
            get
            {
                if (_Servername is null || string.IsNullOrEmpty(_Servername))
                {
                    return new PrinterDriverCollection();
                }
                else
                {
                    return new PrinterDriverCollection(_Servername);
                }
            }
        }
        #endregion

        #region Ports
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The print ports installed on this print server
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PortCollection Ports
        {
            get
            {
                if (_Servername is null || string.IsNullOrEmpty(_Servername))
                {
                    return new PortCollection();
                }
                else
                {
                    return new PortCollection(_Servername);
                }
            }
        }
        #endregion

        #region Printers
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The print devices installed on this print server
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// These can be both physical printers and also software print devices
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrinterInformationCollection Printers
        {
            get
            {
                if (_Servername is null || string.IsNullOrEmpty(_Servername))
                {
                    return new PrinterInformationCollection();
                }
                else
                {
                    return new PrinterInformationCollection(_Servername);
                }
            }
        }
        #endregion

        #region ServerName
        /// <summary>
    /// The name of the server for which this component returns printer information
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks>
    /// Set this value to blank to return information about the current server
    /// </remarks>
        public string ServerName
        {
            get
            {
                return _Servername;
            }
            set
            {
                _Servername = value;
            }
        }
        #endregion

        #endregion

        #region Public constructors
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Gets print server information for the named server
    /// </summary>
    /// <param name="Servername">The server to query information for</param>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrintServer(string Servername)
        {
            if (Servername.StartsWith(@"\\"))
            {
                _Servername = Servername;
            }
            else
            {
                _Servername = @"\\" + Servername;
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Gets print server information for the current machine
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrintServer()
        {

        }
        #endregion

    }
}