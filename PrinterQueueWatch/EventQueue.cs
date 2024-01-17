using System;
using System.Diagnostics;
// \\ --[EventQueue]-----------------------------------------------------
// \\ Queues printer events and printjob events to be 
// \\ raised asynchronously thus allowing the printer monitor
// \\ thread to get on with its business while existing events are 
// \\ being processed.
// \\ (c) 2003 Merrion Computing Ltd
// \\ -------------------------------------------------------------------
using System.Threading;

namespace PrinterQueueWatch
{

    internal class EventQueue : IDisposable
    {

        #region PrinterEventQueue class
        private class PrinterEventQueue : System.Collections.Concurrent.ConcurrentQueue<EventArgs>
        {

            public bool Contains(PrintJobEventArgs JobEventArgs)
            {

                // \\ Duplicate JobWritten events can be ignored
                if (JobEventArgs.EventType == PrintJobEventArgs.PrintJobEventTypes.JobWrittenEvent)
                {
                    return default;

                    // \\ Because an EventQueue pertains to only one printer we can compare on job id and event type
                }
                try
                {
                    foreach (var oItem in this)
                    {
                        if (oItem is PrintJobEventArgs)
                        {
                            {
                                var withBlock = (PrintJobEventArgs)oItem;
                                if (JobEventArgs.EventType == withBlock.EventType && JobEventArgs.PrintJob.JobId == withBlock.PrintJob.JobId && JobEventArgs.PrintJob.QueuedTime == withBlock.PrintJob.QueuedTime)
                                {
                                    return true;
                                    return default;
                                }
                            }
                        }
                    }
                }
                catch (Exception es)
                {
                    return false;
                    return default;
                }

                return default;
            }

        }
        #endregion

        #region Private member variables

        private PrinterEventQueue _EventQueue = new PrinterEventQueue();
        private Thread _EventQueueWorker;

        private PrinterMonitorComponent.JobEvent _JobEvent;
        private PrinterMonitorComponent.PrinterEvent _PrinterEvent;

        private AutoResetEvent _WaitHandle;
        private static bool _Cancelled;


        #endregion

        #region Public interface
        #region AddJobEvent
        public void AddJobEvent(PrintJobEventArgs JobEventArgs)
        {
            // \\ Job events must be unique
            if (!_EventQueue.Contains(JobEventArgs))
            {
                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
                {
                    Trace.WriteLine("Job event enqueued : " + JobEventArgs.EventType.ToString(), GetType().ToString());
                }
                // \\ If the job didn't populate properly, try again
                {
                    var withBlock = JobEventArgs.PrintJob;
                    if (!withBlock.Populated)
                    {
                        withBlock.Refresh();
                    }
                }
                _EventQueue.Enqueue(JobEventArgs);
            }
        }
        #endregion

        #region AddPrinterEvent
        public void AddPrinterEvent(PrinterEventArgs PrinterEventArgs)
        {
            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("Printer event enqueued", GetType().ToString());
            }
            _EventQueue.Enqueue(PrinterEventArgs);
        }
        #endregion

        #region Awaken
        // \\ --[Awaken]--------------------------------------------------------
        // \\ Wakes the thread which dequeues and processes the events
        // \\ ------------------------------------------------------------------
        public void Awaken()
        {
            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("Event queue awakened - " + _EventQueue.Count.ToString() + " events ", GetType().ToString());
            }
            if (_WaitHandle is not null && EventsPending)
            {
                _WaitHandle.Set();
            }
        }
        #endregion

        #region EventsPending
        /// <summary>
    /// Returns True if there are any printer or print job events queued that should be processed 
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public bool EventsPending
        {
            get
            {
                bool eventsPendingProcessing = _EventQueue.Count > 0;
                return eventsPendingProcessing;
            }
        }
        #endregion

        #region OnEndInvokeJobEvent
        public void OnEndInvokeJobEvent(IAsyncResult ar)
        {
            _JobEvent.EndInvoke(ar);
            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("Job event returned", GetType().ToString());
            }
        }
        #endregion

        #region OnEndInvokePriterEvent
        public void OnEndInvokePrinterEvetnt(IAsyncResult ar)
        {
            _PrinterEvent.EndInvoke(ar);
            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("Printer event returned", GetType().ToString());
            }
        }
        #endregion

        #endregion

        #region Private methods

        private void StartThread()
        {
            if (_WaitHandle is null)
            {
                _WaitHandle = new AutoResetEvent(false);
            }

            while (!_Cancelled)
            {
                try
                {
                    // \\ Wait for the WaitHandle to trigger
                    if (_WaitHandle.WaitOne(500, false))
                    {
                        // \\ if the wait handle has not timed out
                        ProcessQueue();
                    }
                }
                catch (ThreadAbortException eTA)
                {
                    // \\ The thread was aborted prematurely
                    if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
                    {
                        Trace.WriteLine("EventQueue loop aborted prematurely", GetType().ToString());
                    }
                    _Cancelled = true;
                }
            }
            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("EventQueue loop ended", GetType().ToString());
            }

        }

        private void ProcessQueue()
        {
            EventArgs oEvent = null;
            IAsyncResult ar;

            _WaitHandle.Reset();


            while (_EventQueue.Count > 0)
            {
                if (_EventQueue.TryDequeue(out oEvent))
                {
                    if (oEvent is PrintJobEventArgs)
                    {
                        if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
                        {
                            Trace.WriteLine("Job event dequeued : " + ((PrintJobEventArgs)oEvent).EventType.ToString(), GetType().ToString());
                        }
                        ar = _JobEvent.BeginInvoke((PrintJobEventArgs)oEvent, OnEndInvokeJobEvent, null);
                    }
                    else if (oEvent is PrinterEventArgs)
                    {
                        if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
                        {
                            Trace.WriteLine("Printer event dequeued", GetType().ToString());
                        }
                        ar = _PrinterEvent.BeginInvoke((PrinterEventArgs)oEvent, OnEndInvokePrinterEvetnt, null);
                    }
                }
            }

            // \\ If events have arrived since we started dequeueing them then triger the wait handle again to deal with them
            if (EventsPending)
            {
                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
                {
                    Trace.WriteLine("Events pending while dequeueing ", GetType().ToString());
                }
                _WaitHandle.Set();
            }
        }
        #endregion

        #region Public constructor
        public EventQueue(PrinterMonitorComponent.JobEvent JobEvent, PrinterMonitorComponent.PrinterEvent PrinterEvent)
        {

            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("New()", GetType().ToString());
            }

            _JobEvent = JobEvent;
            _PrinterEvent = PrinterEvent;

            _EventQueueWorker = new Thread(StartThread);
            try
            {
                _EventQueueWorker.SetApartmentState(ApartmentState.STA);
            }
            catch (Exception e)
            {
                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
                {
                    Trace.WriteLine("Failed to set Single Threaded Apartment for : " + _EventQueueWorker.Name, GetType().ToString());
                }
            }
            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("StartWatching created new EventQueueWorker", GetType().ToString());
            }
            _EventQueueWorker.Start();
        }
        #endregion

        #region IDisposable interface
        public void Dispose()
        {
            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("Dispose()", GetType().ToString());
            }
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Shutdown();
                if (_WaitHandle is not null)
                {
                    _WaitHandle.Dispose();
                    _WaitHandle = null;
                }
            }


        }

        ~EventQueue()
        {
            Dispose(false);
        }

        #region Shutdown
        public void Shutdown()
        {

            _Cancelled = true;
            if (_WaitHandle is not null)
            {
                _WaitHandle.Set();
            }
            if (_EventQueueWorker is not null)
            {
                if (_EventQueueWorker.IsAlive)
                {
                    _EventQueueWorker.Join();
                }
            }
            _WaitHandle = null;
            _EventQueueWorker = null;
            _JobEvent = null;
            _PrinterEvent = null;
        }

        #endregion
        #endregion

    }
}