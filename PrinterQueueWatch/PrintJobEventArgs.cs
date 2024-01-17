using System;

namespace PrinterQueueWatch
{
    // \\ --[PrintJobEventArgs]---------------------------------------------
    // \\ (c) Merrion Computing Ltd 
    // \\     http://www.merrioncomputing.com
    // \\ ------------------------------------------------------------------

    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : PrintJobEventArgs
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// Class wrapper for the event arguments used in the change events 
/// relating to individual jobs in a printer queue..
/// </summary>
/// <remarks>
/// </remarks>
/// <example>Prints the user name of a job when it is added 
/// <code>
///   Private WithEvents pmon As New PrinterMonitorComponent
/// 
///   pmon.AddPrinter("Microsoft Office Document Image Writer")
///   pmon.AddPrinter("HP Laserjet 5")
/// 
///    Private Sub pmon_JobAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles pmon.JobAdded
/// 
///    With CType(e, PrintJobEventArgs)
///        Trace.WriteLine( .PrintJob.Username )
///     End With
/// 
/// End Sub
/// </code>
/// </example>
/// <history>
/// 	[Duncan]	21/11/2005	Created
/// </history>
/// -----------------------------------------------------------------------------
    public class PrintJobEventArgs : EventArgs, IDisposable
    {

        #region Private member variables
        private PrintJob _Job;
        private DateTime _EventTime;
        private PrintJobEventTypes _EventType;
        #endregion

        #region Public enumerated types

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public enum PrintJobEventTypes
        {
            JobAddedEvent = 1,
            JobDeletedEvent = 2,
            JobSetEvent = 3,
            JobWrittenEvent = 4
        }

        #endregion

        #region Public Properties
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The print job for which this event occured
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrintJob PrintJob
        {
            get
            {
                return _Job;
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The date and time at which the print job event occured
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public DateTime EventTime
        {
            get
            {
                return _EventTime;
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The type of job event that occured
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrintJobEventTypes EventType
        {
            get
            {
                return _EventType;
            }
        }
        #endregion

        #region Public Constructors
        internal PrintJobEventArgs(PrintJob Job, DateTime Time, PrintJobEventTypes EventType)
        {
            _Job = Job;
            _EventTime = Time;
            _EventType = EventType;
        }

        internal PrintJobEventArgs(PrintJob Job, PrintJobEventTypes EventType) : this(Job, DateTime.Now, EventType)
        {
        }
        #endregion

        #region Public Methods
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Frees up any system resources used by this job event notification
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	21/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _Job.Dispose();
            }

        }

        public bool Equals(PrintJobEventArgs PrintJobEventArgs)
        {
            if (PrintJobEventArgs is not null)
            {
                if (PrintJobEventArgs.EventType == _EventType && PrintJobEventArgs.PrintJob.JobId == _Job.JobId && (PrintJobEventArgs.PrintJob.PrinterName ?? "") == (_Job.PrinterName ?? ""))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return default;
        }

        #endregion

    }
}