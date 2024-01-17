using System;
// \\ --[PrinterChangeNotificationThread]-----------------------------
// \\ Thread worker to operate a single FindFirstPrinterChange / 
// \\ FindNextPrinterChangeNotification loop for a given printer so
// \\ that a single PrinterMonitorComponent may monitor a number of
// \\ different printer queues.
// \\ (c) Merrion Computing Ltd 
// \\     http://www.merrioncomputing.com
// \\ ----------------------------------------------------------------
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualBasic.CompilerServices;
using PrinterQueueWatch.SpoolerApiConstantEnumerations;

namespace PrinterQueueWatch
{

    internal class PrinterChangeNotificationThread : IDisposable
    {

        #region Private constants

        private const int INFINITE_THREAD_TIMEOUT = int.MinValue + 0x7FFFFFFF;
        private const int PRINTER_NOTIFY_OPTIONS_REFRESH = 0x1;

        #endregion

        #region Private members

        private Thread _Thread;
        private IntPtr _PrinterHandle;

        private int _ThreadTimeout = INFINITE_THREAD_TIMEOUT;

        private bool _Cancelled = false;
        private AutoResetEvent _WaitHandle;

        private PrinterMonitorComponent.MonitorJobEventInformationLevels _MonitorLevel;
        private int _WatchFlags;

        private PrinterNotifyOptions _PrinterNotifyOptions;
        private PrinterInformation _PrinterInformation;

        private bool _PauseAllNewPrintJobs;

        private static object _NotificationLock = new object();
        #endregion

        #region Public methods

        // \\ Start a watching thread
        public void StartWatching() // ByVal PrinterHandle As Int32)
        {

            _Thread = new Thread(StartThread);
            try
            {
                _Thread.SetApartmentState(ApartmentState.STA);
            }
            catch (Exception e)
            {
                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
                {
                    Trace.WriteLine("Failed to set Single Threaded Apartment for : " + _Thread.Name, GetType().ToString());
                }
            }
            _Thread.Name = GetType().Name + ":" + _PrinterHandle.ToString();
            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("StartWatching created new thread: " + _Thread.Name, GetType().ToString());
            }
            _Thread.Start();

        }

        // \\ Cancel the watching thread
        public void CancelWatching()
        {
            _Cancelled = true;
            if (_WaitHandle is not null)
            {
                if (!(_WaitHandle.SafeWaitHandle.IsInvalid || _WaitHandle.SafeWaitHandle.IsClosed))
                {
                    _WaitHandle.Set();
                }
            }
            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("CancelWatching()", GetType().ToString());
            }

        }

        public void StopWatching()
        {
            CancelWatching();


            // \\ And free the printer change notification handle (if it exists)
            if (_WaitHandle is not null)
            {
                if (!_WaitHandle.SafeWaitHandle.IsInvalid || _WaitHandle.SafeWaitHandle.IsClosed)
                {
                    // \\ Stop monitoring the printer
                    try
                    {
                        if (UnsafeNativeMethods.FindClosePrinterChangeNotification(_WaitHandle.SafeWaitHandle))
                        {
                            _WaitHandle.Close();
                            _WaitHandle = null;
                        }
                        else if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                        {
                            Trace.WriteLine("FindClosePrinterChangeNotification() failed", GetType().ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                        {
                            Trace.WriteLine(ex.ToString(), GetType().ToString());
                        }
                    }
                }
            }

            // \\ Wait for the monitoring thread to terminate
            if (_Thread is not null)
            {
                if (_Thread.IsAlive)
                {
                    _Thread.Join();
                }
            }

            GC.KeepAlive(_WaitHandle);
            // \\ And explicitly unset it
            _Thread = null;

        }

        public override string ToString()
        {
            if (_WaitHandle is not null)
            {
                return GetType().ToString() + " handle is valid";
            }
            else
            {
                return GetType().ToString() + " handle invalid ";
            }
        }
        #endregion

        #region Public interface
        #region PauseAllNewPrintJobs
        public bool PauseAllNewPrintJobs
        {
            get
            {
                return _PauseAllNewPrintJobs;
            }
            set
            {
                _PauseAllNewPrintJobs = value;
            }
        }
        #endregion
        #endregion

        #region Public constructors

        public PrinterChangeNotificationThread(IntPtr PrinterHandle, int ThreadTimeout, PrinterMonitorComponent.MonitorJobEventInformationLevels MonitorLevel, int WatchFlags, ref PrinterInformation PrinterInformation)
        {

            // \\ Save a local copy of the information passed in...
            _PrinterHandle = PrinterHandle;
            _ThreadTimeout = ThreadTimeout;
            _MonitorLevel = MonitorLevel;
            _WatchFlags = WatchFlags;
            _PrinterInformation = PrinterInformation;

        }

        public PrinterChangeNotificationThread()
        {

        }

        #endregion

        #region Private methods
        // \\ StartThread - The entry point for this thread
        private void StartThread()
        {

            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("StartThread() of printer handle :" + _PrinterHandle.ToString(), GetType().ToString());
            }

            if (_PrinterHandle.ToInt64() == 0L)
            {
                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                {
                    Trace.WriteLine("StartThread(): _PrinterHandle not set", GetType().ToString());
                }
                _Cancelled = true;
                return;
            }

            // \\ Initialise the printer change notification
            Microsoft.Win32.SafeHandles.SafeWaitHandle mhWait = null;

            if (_WatchFlags == 0)
            {
                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceWarning)
                {
                    Trace.WriteLine("StartWatch: No watch flags set - defaulting to PRINTER_CHANGE_JOB or PRINTER_CHANGE_PRINTER ", GetType().ToString());
                }
                _WatchFlags = (int)(PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_JOB | PrinterChangeNotificationGeneralFlags.PRINTER_CHANGE_PRINTER);
            }

            // \\ Specify what we want to be notified about...
            if (_MonitorLevel == PrinterMonitorComponent.MonitorJobEventInformationLevels.MaximumJobInformation)
            {
                _PrinterNotifyOptions = new PrinterNotifyOptions(false);
            }
            else
            {
                _PrinterNotifyOptions = new PrinterNotifyOptions(true);
            }

            if (_MonitorLevel == PrinterMonitorComponent.MonitorJobEventInformationLevels.MaximumJobInformation | _MonitorLevel == PrinterMonitorComponent.MonitorJobEventInformationLevels.MinimumJobInformation)
            {
                try
                {
                    mhWait = UnsafeNativeMethods.FindFirstPrinterChangeNotification(_PrinterHandle, _WatchFlags, 0, _PrinterNotifyOptions);



                }

                catch (Win32Exception e)
                {
                    // An operating system error has been trapped
                    if (PrinterMonitorComponent.ComponentTraceSwitch.Level >= TraceLevel.Error)
                    {
                        Trace.WriteLine(e.Message + " for printer handle: " + _PrinterHandle.ToString(), GetType().ToString());
                    }
                }
                catch (Exception e2)
                {
                    if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                    {
                        Trace.WriteLine(e2.Message + " for printer handle: " + _PrinterHandle.ToString(), GetType().ToString());
                    }
                }
                finally
                {
                    if (mhWait is not null && mhWait.IsInvalid)
                    {
                        if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                        {
                            Trace.WriteLine("StartWatch: FindFirstPrinterChangeNotification failed - handle: " + mhWait.ToString() + " for printer handle: " + _PrinterHandle.ToString(), GetType().ToString());
                        }
                        throw new Win32Exception();
                    }
                    else if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                    {
                        Trace.WriteLine("StartWatch: FindFirstPrinterChangeNotification succeeded - handle: " + mhWait.ToString() + " for printer handle: " + _PrinterHandle.ToString(), GetType().ToString());
                    }
                }
            }

            else if (_MonitorLevel == PrinterMonitorComponent.MonitorJobEventInformationLevels.NoJobInformation)
            {
                try
                {
                    mhWait = UnsafeNativeMethods.FindFirstPrinterChangeNotification(_PrinterHandle, _WatchFlags, 0, IntPtr.Zero);



                }

                catch (Win32Exception e)
                {
                    // \\ An operating system error has been trapped and returned
                    if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                    {
                        Trace.WriteLine(e.Message + " for printer handle: " + _PrinterHandle.ToString(), GetType().ToString());
                    }
                }
                catch (Exception e2)
                {
                    if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                    {
                        Trace.WriteLine(e2.Message + " for printer handle: " + _PrinterHandle.ToString(), GetType().ToString());
                    }
                }
                finally
                {
                    if (mhWait.IsInvalid)
                    {
                        if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                        {
                            Trace.WriteLine("StartWatch: FindFirstPrinterChangeNotification failed - handle: " + mhWait.ToString() + " for printer handle: " + _PrinterHandle.ToString(), GetType().ToString());
                        }
                        throw new Win32Exception();
                    }
                    else if (PrinterMonitorComponent.ComponentTraceSwitch.TraceInfo)
                    {
                        Trace.WriteLine("StartWatch: FindFirstPrinterChangeNotification succeeded - handle: " + mhWait.ToString() + " for printer handle: " + _PrinterHandle.ToString(), GetType().ToString());
                    }
                }
            }

            if (mhWait.IsInvalid || mhWait.IsClosed)
            {
                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                {
                    Trace.WriteLine("StartWatch: FindFirstPrinterChangeNotification failed for printer handle: " + _PrinterHandle.ToString(), GetType().ToString());
                }
                _Cancelled = true;
                return;
            }
            else
            {
                if (_WaitHandle is null)
                {
                    _WaitHandle = new AutoResetEvent(false);
                }
                _WaitHandle.SafeWaitHandle = mhWait;
            }

            while (!_Cancelled)
            {
                try
                {
                    // \\ Wait for the WaitHandle to trigger
                    if (_WaitHandle.WaitOne(-1, false))
                    {
                        // \\ if the wait handle has not timed out
                        if (!_Cancelled)
                        {
                            DecodePrinterChangeInformation();
                        }
                        else if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                        {
                            Trace.WriteLine("StartThread: Cancelled monitoring raised an event " + _PrinterHandle.ToString(), GetType().ToString());
                        }
                    }
                    else if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
                    {
                        Trace.WriteLine("Wait handle timed out", GetType().ToString());
                    }
                }
                catch (ThreadAbortException eTAE)
                {
                    // \\ This occurs if the thread was aborted from an external source
                    if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
                    {
                        Trace.WriteLine("PrinterNotificationThread aborted", GetType().ToString());
                    }
                    // \\ Therefore stop watching
                    _Cancelled = true;
                }
            }

            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("PrinterNotificationThread loop ended", GetType().ToString());
            }

        }

        /// <summary>
    /// When the spooler notifies the monitoring thread that there is a printer change event 
    /// signalled, this sub decodes that change event and posts it on the event queue
    /// </summary>
    /// <remarks></remarks>
        private void DecodePrinterChangeInformation()
        {
            int mpdChangeFlags;
            IntPtr mlpPrinter;
            PrinterNotifyInfo pInfo;
            PrinterEventFlagDecoder piEventFlags;

            if (_WaitHandle is null || _WaitHandle.SafeWaitHandle.IsClosed || _WaitHandle.SafeWaitHandle.IsInvalid)
            {
                return;
            }

            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("DecodePrinterChangeInformation() for printer handle: " + _PrinterHandle.ToString(), GetType().ToString());
            }


            // \\ Prevent this code being re-entrant...
            lock (_NotificationLock)
            {


                // \\ A printer change notification has occured.....
                try
                {
                    if (UnsafeNativeMethods.FindNextPrinterChangeNotification(_WaitHandle.SafeWaitHandle, out mpdChangeFlags, _PrinterNotifyOptions, out mlpPrinter))


                    {


                        if (mlpPrinter.ToInt64() != 0L)
                        {
                            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
                            {
                                Trace.WriteLine("FindNextPrinterChangeNotification returned a pointer to PRINTER_NOTIFY_INFO :" + mlpPrinter.ToString() + " for printer handle: " + _PrinterHandle.ToString(), GetType().ToString());
                            }

                            var argPrintJobs = _PrinterInformation.PrintJobs;
                            pInfo = new PrinterNotifyInfo(_PrinterHandle, mlpPrinter, ref argPrintJobs);

                            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
                            {
                                Trace.WriteLine("FindNextPrinterChangeNotification notify info " + pInfo.Flags.ToString());
                            }

                            piEventFlags = new PrinterEventFlagDecoder(pInfo.Flags);

                            // \\ If the flags indicate that there was insufficient space to store all
                            // \\ the changes we need to ask again
                            if (!piEventFlags.IsInfoComplete)
                            {
                                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
                                {
                                    Trace.WriteLine("FindNextPrinterChangeNotification returned incomplete PRINTER_NOTIFY_INFO" + " for printer handle: " + _PrinterHandle.ToString(), GetType().ToString());
                                }

                                _PrinterNotifyOptions.dwFlags = _PrinterNotifyOptions.dwFlags | PRINTER_NOTIFY_OPTIONS_REFRESH;

                                if (!UnsafeNativeMethods.FindNextPrinterChangeNotification(_WaitHandle.SafeWaitHandle, out mpdChangeFlags, _PrinterNotifyOptions, out mlpPrinter))


                                {
                                    if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                                    {
                                        Trace.WriteLine("FindNextPrinterChangeNotification failed to PRINTER_NOTIFY_OPTIONS_REFRESH for printer handle: " + _PrinterHandle.ToString(), GetType().ToString());
                                    }
                                    throw new Win32Exception();
                                }
                                else
                                {
                                    _PrinterNotifyOptions.dwFlags = _PrinterNotifyOptions.dwFlags & ~PRINTER_NOTIFY_OPTIONS_REFRESH;
                                    if (mlpPrinter.ToInt64() != 0L)
                                    {
                                        var argPrintJobs1 = _PrinterInformation.PrintJobs;
                                        pInfo = new PrinterNotifyInfo(_PrinterHandle, mlpPrinter, ref argPrintJobs1);
                                    }
                                }
                            }


                            // \\ Raise the appropriate event...
                            piEventFlags = new PrinterEventFlagDecoder(mpdChangeFlags);
                            int nIndex;
                            PrintJob thisJob;

                            if (piEventFlags.ChangesOccured)
                            {

                                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
                                {
                                    Trace.WriteLine("piEventFlags set - changes have occured ");
                                }

                                var loopTo = pInfo.PrintJobs.Count - 1;
                                for (nIndex = 0; nIndex <= loopTo; nIndex++)
                                {
                                    thisJob = _PrinterInformation.PrintJobs.get_ItemByJobId(Conversions.ToInteger(pInfo.PrintJobs[nIndex]));
                                    if (thisJob is not null)
                                    {
                                        if (thisJob.JobId != 0)
                                        {
                                            if (piEventFlags.JobAdded)
                                            {
                                                if (_PauseAllNewPrintJobs)
                                                {
                                                    thisJob.Paused = true;
                                                }
                                                var e = new PrintJobEventArgs(thisJob, PrintJobEventArgs.PrintJobEventTypes.JobAddedEvent);
                                                _PrinterInformation.EventQueue.AddJobEvent(e);
                                            }
                                            if (piEventFlags.JobSet)
                                            {
                                                var e = new PrintJobEventArgs(thisJob, PrintJobEventArgs.PrintJobEventTypes.JobSetEvent);
                                                _PrinterInformation.EventQueue.AddJobEvent(e);
                                            }
                                            if (piEventFlags.JobWritten)
                                            {
                                                var e = new PrintJobEventArgs(thisJob, PrintJobEventArgs.PrintJobEventTypes.JobWrittenEvent);
                                                _PrinterInformation.EventQueue.AddJobEvent(e);
                                            }
                                            if (piEventFlags.JobDeleted)
                                            {
                                                var e = new PrintJobEventArgs(thisJob, PrintJobEventArgs.PrintJobEventTypes.JobDeletedEvent);
                                                _PrinterInformation.EventQueue.AddJobEvent(e);
                                                {
                                                    var withBlock = _PrinterInformation.PrintJobs;
                                                    // \\ Remove this item from the printjobs collection
                                                    withBlock.JobPendingDeletion = Conversions.ToInteger(pInfo.PrintJobs[nIndex]);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (piEventFlags.PrinterChange > 0)
                                {
                                    // \\ If the printer info changed throw that event
                                    var pe = new PrinterEventArgs(pInfo.PrinterInfoChangeFlags, _PrinterInformation);
                                    _PrinterInformation.EventQueue.AddPrinterEvent(pe);
                                }

                                // -- SERVER_EVENTS ----------------------------------------------
                                // Not implemented in this release
                                /* TODO ERROR: Skipped IfDirectiveTrivia
                                #If SERVER_EVENTS = 1 Then
                                *//* TODO ERROR: Skipped DisabledTextTrivia
                                                        If piEventFlags.DriverChange > 0 Then
                                                            'TODO: Pass on the appropriate driver event
                                                            Dim pde As New PrintServerDriverEventArgs(piEventFlags.DriverChange)

                                                        End If

                                                        If piEventFlags.FormChange > 0 Then
                                                            'TODO: Pass on the appropriate form change event
                                                            Dim pfe As New PrintServerFormEventArgs(piEventFlags.FormChange)

                                                        End If

                                                        If piEventFlags.PortChange > 0 Then
                                                            'TODO: Pass on the appropriate port change event
                                                            Dim ppe As New PrintServerPortEventArgs(piEventFlags.PortChange)

                                                        End If

                                                        If piEventFlags.ProcessorChange > 0 Then
                                                            'TODO: Pass on the appropriate processor change event
                                                            Dim ppce As New PrintServerProcessorEventArgs(piEventFlags.ProcessorChange)
                                                        End If
                                *//* TODO ERROR: Skipped EndIfDirectiveTrivia
                                #End If
                                */
                            }
                            piEventFlags = null;
                        }
                        else
                        {
                            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
                            {
                                Trace.WriteLine("FindNextPrinterChangeNotification did not return a pointer to PRINTER_NOTIFY_INFO - the change flag was:" + _WatchFlags.ToString(), GetType().ToString());
                            }
                            piEventFlags = new PrinterEventFlagDecoder(mpdChangeFlags);
                            if (piEventFlags.PrinterChange > 0)
                            {
                                // \\ If the printer info changed throw that event
                                var pe = new PrinterEventArgs(0, _PrinterInformation);
                                _PrinterInformation.EventQueue.AddPrinterEvent(pe);
                            }
                        }

                        _PrinterInformation.EventQueue.Awaken();
                    }
                    else
                    {
                        // \\ Failed to get the next printer change notification set up...
                        if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                        {
                            Trace.WriteLine("FindNextPrinterChangeNotification failed", GetType().ToString());
                        }
                        CancelWatching();
                    }
                }
                catch (Exception exP)
                {
                    if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                    {
                        Trace.WriteLine("FindNextPrinterChangeNotification failed : " + exP.ToString(), GetType().ToString());
                    }
                }
            }
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
                StopWatching();
                if (_PrinterNotifyOptions is not null)
                {
                    _PrinterNotifyOptions.Dispose();
                    _PrinterNotifyOptions = null;
                }

            }
        }

        ~PrinterChangeNotificationThread()
        {
            Dispose(false);
        }

        #endregion

    }
}