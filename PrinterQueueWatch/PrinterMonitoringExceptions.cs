using System;
// --[PrinterMonitoringExceptions]-----------------------------------------
// \\ Namespace for all the spooler related exceptions 
// \\ (c) 2003-2012 Merrion Computing Ltd
// \\ http://www.merrioncomputing.com
// \\ ------------------------------------------------------------------
namespace PrinterQueueWatch.PrinterMonitoringExceptions
{

    #region InsufficentPrinterAccessRightsException

    /// -----------------------------------------------------------------------------
    /// Project	 : PrinterQueueWatch
    /// Class	 : PrinterMonitoringExceptions.InsufficentPrinterAccessRightsException
    /// 
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Thrown when an attempt is made to access the printer by a process that does not
    /// have sufficient access rights  
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public class InsufficentPrinterAccessRightsException : Exception
    {

        public InsufficentPrinterAccessRightsException() : base(PrinterMonitorComponent.resources.GetString("pem_NoPrinterAccess"))
        {
        }

        public InsufficentPrinterAccessRightsException(string Message) : base(Message)
        {
        }

        public InsufficentPrinterAccessRightsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InsufficentPrinterAccessRightsException(Exception innerException) : base("", innerException)
        {
        }
    }

    #endregion

    #region InsufficientPrintJobAccessRightsException

    /// -----------------------------------------------------------------------------
    /// Project	 : PrinterQueueWatch
    /// Class	 : PrinterMonitoringExceptions.InsufficentPrintJobAccessRightsException
    /// 
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Thrown when an attempt is made to access the print job by a process that does not
    /// have sufficient access rights
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public class InsufficentPrintJobAccessRightsException : Exception
    {

        public InsufficentPrintJobAccessRightsException() : base(PrinterMonitorComponent.resources.GetString("pem_NoJobAccess"))
        {
        }

        public InsufficentPrintJobAccessRightsException(string Message) : base(Message)
        {
        }

        public InsufficentPrintJobAccessRightsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    #endregion

    #region PrintJobTransferException
    /// <summary>
    /// An exception that is thrown when an error occurs transfering a print job from one print queue to another
    /// </summary>
    [Serializable()]
    public class PrintJobTransferException : Exception
    {

        public PrintJobTransferException() : base(My.Resources.Resources.pem_JobTransferFailed)
        {
        }

        public PrintJobTransferException(string Message) : base(Message)
        {
        }

        public PrintJobTransferException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    #endregion

}