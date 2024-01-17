using System;
// \\ --[PrinterInformation]--------------------------------
// \\ Class wrapper for the windows API calls and constants
// \\ relating to getting and setting printer info..
// \\ (c) Merrion Computing Ltd 
// \\     http://www.merrioncomputing.com
// \\ ----------------------------------------------------
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.CompilerServices;
using PrinterQueueWatch.PrinterMonitoringExceptions;
using PrinterQueueWatch.SpoolerApiConstantEnumerations;
using PrinterQueueWatch.SpoolerStructs;

namespace PrinterQueueWatch
{

    /// -----------------------------------------------------------------------------
/// Project	 : PrinterQueueWatch
/// Class	 : PrinterInformation
/// 
/// -----------------------------------------------------------------------------
/// <summary>
/// Class which holds the settings for a printer 
/// </summary>
/// <remarks>
/// These settings can apply to physical printers and also to virtual print devices
/// </remarks>
/// <history>
/// 	[Duncan]	20/11/2005	Created
/// </history>
/// -----------------------------------------------------------------------------
    [System.Security.SuppressUnmanagedCodeSecurity()]
    [ComVisible(false)]
    public class PrinterInformation : IDisposable
    {

        #region Private member variables

        private IntPtr mhPrinter;

        // \\ PRINTER_INFO_ structures
        private PRINTER_INFO_2 mPrinter_Info_2;
        private PRINTER_INFO_3 mPrinter_Info_3;

        private bool bHandleOwnedByMe;

        private PrintJobCollection _PrintJobs;
        private PrinterChangeNotificationThread _NotificationThread;
        private bool _Monitored;
        private PrinterMonitorComponent.MonitorJobEventInformationLevels _MonitorLevel;
        private int _ThreadTimeout;
        private int _WatchFlags;

        private PrinterMonitorComponent.JobEvent _JobEvent;
        private PrinterMonitorComponent.PrinterEvent _PrinterEvent;

        private TimeWindow _TimeWindow;

        #endregion

        #region Friend member variables

        private EventQueue _EventQueue;

        internal EventQueue EventQueue
        {
            get
            {
                return _EventQueue;
            }
        }
        #endregion

        #region IDisposable implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Monitored = false;
                if (_NotificationThread is not null)
                {
                    _NotificationThread.Dispose();
                }
                if (_EventQueue is not null)
                {
                    _EventQueue.Dispose();
                }
                if (_JobEvent is not null)
                {
                    _JobEvent = null;
                }
                if (_PrinterEvent is not null)
                {
                    _PrinterEvent = null;
                }
                if (bHandleOwnedByMe)
                {
                    try
                    {
                        if (!UnsafeNativeMethods.ClosePrinter(mhPrinter))
                        {
                            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                            {
                                Trace.WriteLine("Error in PrinterInformation:Dispose", GetType().ToString());
                            }
                        }
                        else
                        {
                            bHandleOwnedByMe = false;
                        }
                    }
                    catch (Win32Exception ex32)
                    {
                        if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                        {
                            Trace.WriteLine("Error in PrinterInformation:Dispose " + ex32.ToString(), GetType().ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                        {
                            Trace.WriteLine("Error in PrinterInformation:Dispose " + ex.ToString(), GetType().ToString());
                        }
                    }

                }
            }
        }

        ~PrinterInformation()
        {
            Dispose(false);
        }

        #endregion

        #region Public interface

        #region PRINTER_INFO_2 properties

        #region ServerName
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The name of the server on which this printer is installed
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This value may be blank if the printer is attached to the current machine
    /// </remarks>
    /// <example>Prints the name of the server that the named printer is installed on
    /// <code>
    ///    Dim pi As New PrinterInformation("Microsoft Office Document Image Writer", SpoolerApiConstantEnumerations.PrinterAccessRights.PRINTER_ALL_ACCESS, True)
    ///    Trace.WriteLine(pi.ServerName)
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("The name of the physical server this printer is attached to")]
        public virtual string ServerName
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                if (mPrinter_Info_2.pServerName is null)
                {
                    return "";
                }
                else
                {
                    return mPrinter_Info_2.pServerName;
                }
            }
        }
        #endregion

        #region PrinterName
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The unique name by which the printer is known
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("The unique name of the printer itself")]
        public virtual string PrinterName
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                if (mPrinter_Info_2.pPrinterName is null)
                {
                    return "";
                }
                else
                {
                    return mPrinter_Info_2.pPrinterName;
                }
            }
        }
        #endregion

        #region ShareName
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// identifies the sharepoint for the printer
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This will only be set if the Shared property is set to True
    /// </remarks>
    /// <example>Prints the name of the share (if any) that the named printer is shared out on
    /// <code>
    ///    Dim pi As New PrinterInformation("Microsoft Office Document Image Writer", SpoolerApiConstantEnumerations.PrinterAccessRights.PRINTER_ALL_ACCESS, True)
    ///    Trace.WriteLine(pi.ShareName)
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("The name that the printer is shared under (if it is shared)")]
        public virtual string ShareName
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                if (mPrinter_Info_2.pShareName is null)
                {
                    return "";
                }
                else
                {
                    return mPrinter_Info_2.pShareName;
                }
            }
        }
        #endregion

        #region PortName
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The name of the port the printer is connected to
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <example>Prints the name of the port that the named printer is installed on
    /// <code>
    ///    Dim pi As New PrinterInformation("Microsoft Office Document Image Writer", SpoolerApiConstantEnumerations.PrinterAccessRights.PRINTER_ALL_ACCESS, True)
    ///    Trace.WriteLine(pi.PortName)
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("The name of the port the printer is connected to")]
        public virtual string PortName
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                if (mPrinter_Info_2.pPortName is null)
                {
                    return "";
                }
                else
                {
                    return mPrinter_Info_2.pPortName;
                }
            }
        }
        #endregion

        #region DriverName
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The name of the printer driver software used by this printer
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <example>Prints the name of the driver that the named printer is using
    /// <code>
    ///    Dim pi As New PrinterInformation("Microsoft Office Document Image Writer", SpoolerApiConstantEnumerations.PrinterAccessRights.PRINTER_ALL_ACCESS, True)
    ///    Trace.WriteLine(pi.DriverName)
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("The name of the printer driver software used by this printer")]
        public virtual string DriverName
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                if (mPrinter_Info_2.pDriverName is null)
                {
                    return "";
                }
                else
                {
                    return mPrinter_Info_2.pDriverName;
                }
            }
        }
        #endregion

        #region Comment
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The administrator defined comment for this printer
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This can be useful for giving extra information about a printer to the user
    /// </remarks>
    /// <example>Changes the comment assigned for this printer
    /// <code>
    ///    Dim pi As New PrinterInformation("Microsoft Office Document Image Writer", SpoolerApiConstantEnumerations.PrinterAccessRights.PRINTER_ALL_ACCESS, True)
    ///    pi.Comment = "Monitored by PUMA"
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("The administrator defined comment for this printer")]
        public virtual string Comment
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                if (mPrinter_Info_2.pComment is null)
                {
                    return "";
                }
                else
                {
                    return mPrinter_Info_2.pComment;
                }
            }
            set
            {
                if ((value ?? "") != (mPrinter_Info_2.pComment ?? ""))
                {
                    mPrinter_Info_2.pComment = value;
                    SavePrinterInfo(PrinterInfoLevels.PrinterInfoLevel2, false);
                }
            }
        }
        #endregion

        #region Location
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The administrator defined location for this printer
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <example>Prints the location that the named printer is installed on
    /// <code>
    ///    Dim pi As New PrinterInformation("Microsoft Office Document Image Writer", SpoolerApiConstantEnumerations.PrinterAccessRights.PRINTER_ALL_ACCESS, True)
    ///    Trace.WriteLine(pi.Location)
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("The administrator defined location for this printer")]
        public virtual string Location
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                if (mPrinter_Info_2.pLocation is null)
                {
                    return "";
                }
                else
                {
                    return mPrinter_Info_2.pLocation;
                }
            }
            set
            {
                if ((value ?? "") != (mPrinter_Info_2.pLocation ?? ""))
                {
                    mPrinter_Info_2.pLocation = value;
                    SavePrinterInfo(PrinterInfoLevels.PrinterInfoLevel2, false);
                }
            }
        }
        #endregion

        #region SeperatorFilename
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The name of the file (if any) that is printed to seperate print jobs on this printer
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <example>Prints the name of the print job seperator that the named printer using
    /// <code>
    ///    Dim pi As New PrinterInformation("Microsoft Office Document Image Writer", SpoolerApiConstantEnumerations.PrinterAccessRights.PRINTER_ALL_ACCESS, True)
    ///    Trace.WriteLine(pi.SeperatorFilename)
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("The name of the file (if any) that is printed to seperate print jobs on this printer")]
        public virtual string SeperatorFilename
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                if (mPrinter_Info_2.pSeperatorFilename is null)
                {
                    return "";
                }
                else
                {
                    return mPrinter_Info_2.pSeperatorFilename;
                }
            }
            set
            {
                if ((value ?? "") != (mPrinter_Info_2.pSeperatorFilename ?? ""))
                {
                    mPrinter_Info_2.pSeperatorFilename = value;
                    SavePrinterInfo(PrinterInfoLevels.PrinterInfoLevel2, false);
                }
            }
        }
        #endregion

        #region PrintProcessor
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The name of the print processor associated to this printer
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <example>Prints the name of the print processor that the named printer using
    /// <code>
    ///    Dim pi As New PrinterInformation("Microsoft Office Document Image Writer", SpoolerApiConstantEnumerations.PrinterAccessRights.PRINTER_ALL_ACCESS, True)
    ///    Trace.WriteLine(pi.PrintProcessor)
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("The name of the print processor associated to this printer")]
        public virtual string PrintProcessor
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                if (mPrinter_Info_2.pPrintProcessor is null)
                {
                    return "";
                }
                else
                {
                    return mPrinter_Info_2.pPrintProcessor;
                }
            }
        }
        #endregion

        #region DefaultDataType
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The default spool data type (e.g. RAW, EMF etc.) used by this printer
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// If this value is set to RAW the printer will spool data in a printer control language 
    /// (such as PCL or PostScript)
    /// </remarks>
    /// <example>Prints the name of the default data type that the named printer using
    /// <code>
    ///    Dim pi As New PrinterInformation("Microsoft Office Document Image Writer", SpoolerApiConstantEnumerations.PrinterAccessRights.PRINTER_ALL_ACCESS, True)
    ///    Trace.WriteLine(pi.DefaultDataType)
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("The default spool data type (e.g. RAW, EMF etc.) used by this printer spooler")]
        public virtual string DefaultDataType
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                if (mPrinter_Info_2.pDataType is null)
                {
                    return "";
                }
                else
                {
                    return mPrinter_Info_2.pDataType;
                }
            }
        }
        #endregion

        #region Parameters
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Additional parameter used when printing on this printer
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The possible values and meanings of these extra parameters depend on the 
    /// printer driver being used
    /// </remarks>
    /// <example>Prints the extra parameters (if any) that the named printer using
    /// <code>
    ///    Dim pi As New PrinterInformation("Microsoft Office Document Image Writer", SpoolerApiConstantEnumerations.PrinterAccessRights.PRINTER_ALL_ACCESS, True)
    ///    Trace.WriteLine(pi.Parameters)
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("Additional parameter used when printing on this printer")]
        public virtual string Parameters
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                if (mPrinter_Info_2.pParameters is null)
                {
                    return "";
                }
                else
                {
                    return mPrinter_Info_2.pParameters;
                }
            }
        }
        #endregion

        #region Attributes related
        #region IsDefault
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if this printer is the default printer on this machine
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("True if this printer is the default printer on this machine")]
        public virtual bool IsDefault
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return Conversions.ToBoolean(mPrinter_Info_2.Attributes & (int)PrinterAttributes.PRINTER_ATTRIBUTE_DEFAULT);
            }
        }
        #endregion

        #region IsShared
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if this printer is a shared device
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("True if this printer is a shared device")]
        public virtual bool IsShared
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return Conversions.ToBoolean(mPrinter_Info_2.Attributes & (int)PrinterAttributes.PRINTER_ATTRIBUTE_SHARED);
            }
        }
        #endregion

        #region IsNetworkPrinter
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if this is a network printer
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("True if this is a network printer")]
        public virtual bool IsNetworkPrinter
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return Conversions.ToBoolean(mPrinter_Info_2.Attributes & (int)PrinterAttributes.PRINTER_ATTRIBUTE_NETWORK);
            }
        }
        #endregion

        #region IsLocalPrinter
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if this printer is local to this machine
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("True if this printer is local to this machine")]
        public virtual bool IsLocalPrinter
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return Conversions.ToBoolean(mPrinter_Info_2.Attributes & (int)PrinterAttributes.PRINTER_ATTRIBUTE_LOCAL);
            }
        }
        #endregion

        #endregion

        #region Priority
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The default priority of print jobs sent to this printer
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// Priority can range from 1 (lowest) to 99 (highest).  
    /// Attempting to set the value outside the range will be reset to the nearest range bounds
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("The default priority of print jobs sent to this printer")]
        public virtual int Priority
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return mPrinter_Info_2.Priority;
            }
            set
            {
                if (value != mPrinter_Info_2.Priority)
                {
                    if (value < 1)
                    {
                        mPrinter_Info_2.Priority = 1;
                    }
                    else if (value > 99)
                    {
                        mPrinter_Info_2.Priority = 99;
                    }
                    else
                    {
                        mPrinter_Info_2.Priority = value;
                        SavePrinterInfo(PrinterInfoLevels.PrinterInfoLevel2, false);
                    }
                }
            }
        }
        #endregion

        #region Status related

        #region IsReady
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the printer is ready to print
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The status is updated when a job is sent to the printer so may not match the true state of the printer 
    /// if there are no jobs in the queue
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("True if the printer is ready to print")]
        public virtual bool IsReady
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return mPrinter_Info_2.Status == 0;
            }
        }
        #endregion

        #region IsDoorOpen
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the printer is stalled because a door or papaer tray is open
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The status is updated when a job is sent to the printer so may not match the true state of the printer 
    /// if there are no jobs in the queue
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("True if the printer is stalled because a door or papaer tray is open")]
        public virtual bool IsDoorOpen
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return (mPrinter_Info_2.Status & (int)PrinterStatuses.PRINTER_STATUS_DOOR_OPEN) == (int)PrinterStatuses.PRINTER_STATUS_DOOR_OPEN;
            }
        }
        #endregion

        #region IsInError
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the printer has an error
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The status is updated when a job is sent to the printer so may not match the true state of the printer 
    /// if there are no jobs in the queue
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("True if the printer has an error")]
        public virtual bool IsInError
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return (mPrinter_Info_2.Status & (int)PrinterStatuses.PRINTER_STATUS_ERROR) == (int)PrinterStatuses.PRINTER_STATUS_ERROR;
            }
        }
        #endregion

        #region IsInitialising
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the printer is initialising
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The status is updated when a job is sent to the printer so may not match the true state of the printer 
    /// if there are no jobs in the queue
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("True if the printer is initialising")]
        public virtual bool IsInitialising
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return (mPrinter_Info_2.Status & (int)PrinterStatuses.PRINTER_STATUS_INITIALIZING) == (int)PrinterStatuses.PRINTER_STATUS_INITIALIZING;
            }
        }
        #endregion

        #region IsAwaitingManualFeed
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the printer is stalled awaiting a manual paper feed
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The status is updated when a job is sent to the printer so may not match the true state of the printer 
    /// if there are no jobs in the queue
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("True if the printer is stalled awaiting a manual paper feed")]
        public virtual bool IsAwaitingManualFeed
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return (mPrinter_Info_2.Status & (int)PrinterStatuses.PRINTER_STATUS_MANUAL_FEED) == (int)PrinterStatuses.PRINTER_STATUS_MANUAL_FEED;
            }
        }
        #endregion

        #region IsOutOfToner
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the printer is stalled because it is out of toner or ink
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The status is updated when a job is sent to the printer so may not match the true state of the printer 
    /// if there are no jobs in the queue
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("True if the printer is stalled because it is out of toner or ink")]
        public virtual bool IsOutOfToner
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return (mPrinter_Info_2.Status & (int)PrinterStatuses.PRINTER_STATUS_NO_TONER) == (int)PrinterStatuses.PRINTER_STATUS_NO_TONER;
            }
        }
        #endregion

        #region IsTonerLow
        /// <summary>
    /// True if the printer is stalled because it is low on toner or ink
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks>
    /// The status is updated when a job is sent to the printer so may not match the true state of the printer 
    /// if there are no jobs in the queue
    /// </remarks>
        [MonitoringDescription("True if the printer is stalled because it is low on toner or ink")]
        public virtual bool IsTonerLow
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return (mPrinter_Info_2.Status & (int)PrinterStatuses.PRINTER_STATUS_TONER_LOW) == (int)PrinterStatuses.PRINTER_STATUS_NO_TONER;
            }
        }
        #endregion

        #region IsUnavailable
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the printer is currently unnavailable
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The status is updated when a job is sent to the printer so may not match the true state of the printer 
    /// if there are no jobs in the queue
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("True if the printer is currently unnavailable")]
        public virtual bool IsUnavailable
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return (mPrinter_Info_2.Status & (int)PrinterStatuses.PRINTER_STATUS_NOT_AVAILABLE) == (int)PrinterStatuses.PRINTER_STATUS_NOT_AVAILABLE;
            }
        }
        #endregion

        #region IsOffline
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the printer is offline
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The status is updated when a job is sent to the printer so may not match the true state of the printer 
    /// if there are no jobs in the queue
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("True if the printer is offline")]
        public virtual bool IsOffline
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return (mPrinter_Info_2.Status & (int)PrinterStatuses.PRINTER_STATUS_OFFLINE) == (int)PrinterStatuses.PRINTER_STATUS_OFFLINE;
            }
        }
        #endregion

        #region IsOutOfMemory
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the printer is stalled because it has run out of memory
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The status is updated when a job is sent to the printer so may not match the true state of the printer 
    /// if there are no jobs in the queue
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("True if the printer is stalled because it has run out of memory")]
        public virtual bool IsOutOfmemory
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return (mPrinter_Info_2.Status & (int)PrinterStatuses.PRINTER_STATUS_OUT_OF_MEMORY) == (int)PrinterStatuses.PRINTER_STATUS_OUT_OF_MEMORY;
            }
        }
        #endregion

        #region IsOutputBinFull
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the printer is stalled because it's output tray is full
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The status is updated when a job is sent to the printer so may not match the true state of the printer 
    /// if there are no jobs in the queue
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("True if the printer is stalled because it's output tray is full")]
        public virtual bool IsOutputBinFull
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return (mPrinter_Info_2.Status & (int)PrinterStatuses.PRINTER_STATUS_OUTPUT_BIN_FULL) == (int)PrinterStatuses.PRINTER_STATUS_OUTPUT_BIN_FULL;
            }
        }
        #endregion

        #region IsPaperJammed
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the printer is stalled because it has a paper jam
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The status is updated when a job is sent to the printer so may not match the true state of the printer 
    /// if there are no jobs in the queue
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("True if the printer is stalled because it has a paper jam")]
        public virtual bool IsPaperJammed
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return (mPrinter_Info_2.Status & (int)PrinterStatuses.PRINTER_STATUS_PAPER_JAM) == (int)PrinterStatuses.PRINTER_STATUS_PAPER_JAM;
            }
        }
        #endregion

        #region IsOutOfPaper
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the printer is stalled because it is out of paper
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The status is updated when a job is sent to the printer so may not match the true state of the printer 
    /// if there are no jobs in the queue
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("True if the printer is stalled because it is out of paper")]
        public virtual bool IsOutOfPaper
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return (mPrinter_Info_2.Status & (int)PrinterStatuses.PRINTER_STATUS_PAPER_OUT) == (int)PrinterStatuses.PRINTER_STATUS_PAPER_OUT;
            }
        }
        #endregion

        #region Paused
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the printer is paused
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The status is updated when a job is sent to the printer so may not match the true state of the printer 
    /// if there are no jobs in the queue
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("True if the printer is paused")]
        public virtual bool Paused
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return (mPrinter_Info_2.Status & (int)PrinterStatuses.PRINTER_STATUS_PAUSED) == (int)PrinterStatuses.PRINTER_STATUS_PAUSED;
            }
        }
        #endregion

        #region IsDeletingJob
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the printer is deleting a job
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The status is updated when a job is sent to the printer so may not match the true state of the printer 
    /// if there are no jobs in the queue
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("True if the printer is deleting a job")]
        public virtual bool IsDeletingJob
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return (mPrinter_Info_2.Status & (int)PrinterStatuses.PRINTER_STATUS_PENDING_DELETION) == (int)PrinterStatuses.PRINTER_STATUS_PENDING_DELETION;
            }
        }
        #endregion

        #region IsInPowerSave
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the printer is in power saving mode
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The status is updated when a job is sent to the printer so may not match the true state of the printer 
    /// if there are no jobs in the queue
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("True if the printer is in power saving mode")]
        public virtual bool IsInPowerSave
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return (mPrinter_Info_2.Status & (int)PrinterStatuses.PRINTER_STATUS_POWER_SAVE) == (int)PrinterStatuses.PRINTER_STATUS_POWER_SAVE;
            }
        }
        #endregion

        #region IsPrinting
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the printer is currently printing a job
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The status is updated when a job is sent to the printer so may not match the true state of the printer 
    /// if there are no jobs in the queue
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("True if the printer is currently printing a job")]
        public virtual bool IsPrinting
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return (mPrinter_Info_2.Status & (int)PrinterStatuses.PRINTER_STATUS_PRINTING) == (int)PrinterStatuses.PRINTER_STATUS_PRINTING;
            }
        }
        #endregion

        #region IsWaitingOnUserIntervention
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the printer is stalled awaiting manual intervention
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The status is updated when a job is sent to the printer so may not match the true state of the printer 
    /// if there are no jobs in the queue
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("True if the printer is stalled awaiting manual intervention")]
        public virtual bool IsWaitingOnUserIntervention
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return (mPrinter_Info_2.Status & (int)PrinterStatuses.PRINTER_STATUS_USER_INTERVENTION) == (int)PrinterStatuses.PRINTER_STATUS_USER_INTERVENTION;
            }
        }
        #endregion

        #region IsWarmingUp
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// True if the printer is warming up to be ready to print
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The status is updated when a job is sent to the printer so may not match the true state of the printer 
    /// if there are no jobs in the queue
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("True if the printer is warming up to be ready to print")]
        public virtual bool IsWarmingUp
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return (mPrinter_Info_2.Status & (int)PrinterStatuses.PRINTER_STATUS_WARMING_UP) == (int)PrinterStatuses.PRINTER_STATUS_WARMING_UP;
            }
        }
        #endregion
        #endregion

        #region JobCount
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The number of print jobs queued to be printed by this printer
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("The number of jobs waiting on this printer")]
        public virtual int JobCount
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return mPrinter_Info_2.JobCount;
            }
        }
        #endregion

        #region AveragePagesPerMonth
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The average throughput of this printer in pages per month
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("The average throughput of this printer in pages per month")]
        public virtual int AveragePagesPerMonth
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return mPrinter_Info_2.AveragePPM;
            }
        }
        #endregion

        #region TimeWindow
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The time window within which jobs can be scheduled against this printer
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [Obsolete("Use Availablity instead.")]
        [MonitoringDescription("The time window within which jobs can be scheduled against this printer")]
        public TimeWindow TimeWindow
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                _TimeWindow = new TimeWindow(mPrinter_Info_2.StartTime, mPrinter_Info_2.UntilTime);
                return _TimeWindow;
            }
            set
            {
                mPrinter_Info_2.StartTime = value.StartTimeMinutes;
                mPrinter_Info_2.UntilTime = value.EndTimeMinutes;
                SavePrinterInfo(PrinterInfoLevels.PrinterInfoLevel2, false);
            }
        }
        #endregion

        #region DefaultPaperSource
        /// <summary>
    /// The default tray used by this printer 
    /// </summary>
    /// <value></value>
    /// <remarks>This value can be overriden for each individual print job
    /// </remarks>
    /// <history>
    /// 	[Duncan]	11/02/2006	Created
    /// </history>
        [MonitoringDescription("The default paper tray for this printer")]
        public System.Drawing.Printing.PaperSourceKind DefaultPaperSource
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return (System.Drawing.Printing.PaperSourceKind)mPrinter_Info_2.DeviceMode.dmDefaultSource;
            }
            set
            {
                mPrinter_Info_2.DeviceMode.dmDefaultSource = (short)value;
                SavePrinterInfo(PrinterInfoLevels.PrinterInfoLevel2, false);
            }
        }
        #endregion

        #region Copies
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The number of copies of each print job to produce 
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This value can be overridden for each print job
    /// </remarks>
    /// <history>
    /// 	[Duncan]	11/02/2006	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public int Copies
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                if (mPrinter_Info_2.DeviceMode.dmCopies > 0)
                {
                    return mPrinter_Info_2.DeviceMode.dmCopies;
                }
                else
                {
                    return 1;
                }
            }
        }
        #endregion

        #region Landscape
        /// <summary>
    /// True if the printer orientation is set to Landscape
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks>
    /// This value can be overriden by the individual print job's orientation
    /// </remarks>
        public bool Landscape
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return mPrinter_Info_2.DeviceMode.dmOrientation == (int)DeviceOrientations.DMORIENT_LANDSCAPE;
            }
            set
            {
                if (value)
                {
                    mPrinter_Info_2.DeviceMode.dmOrientation = (short)DeviceOrientations.DMORIENT_LANDSCAPE;
                }
                else
                {
                    mPrinter_Info_2.DeviceMode.dmOrientation = (short)DeviceOrientations.DMORIENT_PORTRAIT;
                }
                SavePrinterInfo(PrinterInfoLevels.PrinterInfoLevel2, false);
            }
        }
        #endregion

        #region Colour
        /// <summary>
    /// True if a colour printer is set to print in colour, false for monochrome
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
        public bool Colour
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return mPrinter_Info_2.DeviceMode.dmColor == (int)DeviceColourModes.DMCOLOR_COLOR;
            }
            set
            {
                if (value)
                {
                    mPrinter_Info_2.DeviceMode.dmColor = (short)DeviceColourModes.DMCOLOR_COLOR;
                }
                else
                {
                    mPrinter_Info_2.DeviceMode.dmColor = (short)DeviceColourModes.DMCOLOR_MONOCHROME;
                }
                SavePrinterInfo(PrinterInfoLevels.PrinterInfoLevel2, false);
            }
        }
        #endregion

        #region Collate
        /// <summary>
    /// Specifies whether collation should be used when printing multiple copies.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks>
    /// Not all printers support collation.  Those that don't will ignore this setting
    /// </remarks>
        public bool Collate
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return mPrinter_Info_2.DeviceMode.dmCollate == (int)DeviceCollateSettings.DMCOLLATE_TRUE;
            }
            set
            {
                if (value)
                {
                    mPrinter_Info_2.DeviceMode.dmCollate = (short)DeviceCollateSettings.DMCOLLATE_TRUE;
                }
                else
                {
                    mPrinter_Info_2.DeviceMode.dmCollate = (short)DeviceCollateSettings.DMCOLLATE_FALSE;
                }
                SavePrinterInfo(PrinterInfoLevels.PrinterInfoLevel2, false);
            }
        }
        #endregion

        #region PrintQuality
        public DeviceModeResolutions PrintQuality
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return (DeviceModeResolutions)mPrinter_Info_2.DeviceMode.dmPrintQuality;
            }
            set
            {
                mPrinter_Info_2.DeviceMode.dmPrintQuality = (short)value;
                SavePrinterInfo(PrinterInfoLevels.PrinterInfoLevel2, false);
            }
        }
        #endregion

        #region Scale
        /// <summary>
    /// The scale (percentage) to print the page at
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
        public short Scale
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return mPrinter_Info_2.DeviceMode.dmScale;
            }
            set
            {
                if (value <= 0 || value > 100)
                {
                    throw new ArgumentException("Scale cannot be less than or equal to zero nor greater than 100");
                }
                mPrinter_Info_2.DeviceMode.dmScale = value;
                SavePrinterInfo(PrinterInfoLevels.PrinterInfoLevel2, false);
            }
        }
        #endregion

        #endregion

        #region PRINTER_INFO_3 properties

        #region SecurityDescriptorPointer
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public IntPtr SecurityDescriptorPointer
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel3);
                if (mPrinter_Info_3 is not null)
                {
                    return mPrinter_Info_3.pSecurityDescriptor;
                }
                else
                {
                    return IntPtr.Zero;
                }
            }
        }
        #endregion

        #endregion

        #region Public Methods

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Pauses the printer 
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <example>Pauses the named printer
    /// <code>
    ///    Dim pi As New PrinterInformation("Microsoft Office Document Image Writer", SpoolerApiConstantEnumerations.PrinterAccessRights.PRINTER_ALL_ACCESS, False)
    ///    pi.PausePrinting
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public void PausePrinting()
        {
            // \\ Do not attempt to pause and already paused printer
            if (!Paused)
            {
                try
                {
                    if (!UnsafeNativeMethods.SetPrinter(mhPrinter, 0, IntPtr.Zero, PrinterControlCommands.PRINTER_CONTROL_PAUSE))
                    {
                        throw new Win32Exception();
                    }
                }
                catch (Win32Exception e)
                {
                    if (e.NativeErrorCode == (int)SpoolerWin32ErrorCodes.ERROR_ACCESS_DENIED)
                    {
                        throw new InsufficentPrinterAccessRightsException(My.Resources.Resources.pem_NoPause, e);
                    }
                    else
                    {
                        if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                        {
                            Trace.WriteLine("SetPrinter (Pause) failed", GetType().ToString());
                        }
                        throw;
                    }
                }
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Restart a printer that has been paused
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <example>Resumes printing on the named printing
    /// <code>
    ///    Dim pi As New PrinterInformation("Microsoft Office Document Image Writer", SpoolerApiConstantEnumerations.PrinterAccessRights.PRINTER_ALL_ACCESS, True)
    ///    pi.ResumePrinting
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public void ResumePrinting()
        {
            // \\ Do not attempt to resume if the printer is not paused
            if (Paused)
            {
                try
                {
                    if (!UnsafeNativeMethods.SetPrinter(mhPrinter, 0, IntPtr.Zero, PrinterControlCommands.PRINTER_CONTROL_RESUME))
                    {
                        throw new Win32Exception();
                    }
                }
                catch (Win32Exception e)
                {
                    if (e.NativeErrorCode == (int)SpoolerWin32ErrorCodes.ERROR_ACCESS_DENIED)
                    {
                        throw new InsufficentPrinterAccessRightsException(My.Resources.Resources.pem_NoResume, e);
                    }
                    else
                    {
                        if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                        {
                            Trace.WriteLine("SetPrinter (Resume) failed", GetType().ToString());
                        }
                        throw;
                    }
                }
            }
        }

        #endregion

        #region PrintJobs collection
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// The collection of PrintJobs queued for printing on this printer
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <example>Prints the name of the documents in the print queue
    /// <code>
    ///    Dim pi As New PrinterInformation("Microsoft Office Document Image Writer", SpoolerApiConstantEnumerations.PrinterAccessRights.PRINTER_ALL_ACCESS, True)
    ///    For Each pj As PrintJob In pi.PrintJobs
    ///       Trace.WriteLine(pj.Document)
    ///    Next pj
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("The collection of PrintJobs queued for printing on this printer")]
        public PrintJobCollection PrintJobs
        {
            get
            {
                if (_PrintJobs is null)
                {
                    _PrintJobs = new PrintJobCollection(mhPrinter, JobCount);
                }
                if (_PrintJobs.JobPendingDeletion > 0)
                {
                    _PrintJobs.RemoveByJobId(_PrintJobs.JobPendingDeletion);
                    _PrintJobs.JobPendingDeletion = 0;
                }
                return _PrintJobs;
            }
        }
        #endregion

        #region Monitored
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Sets whether or not events occuring on this printer are raised by the component
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        [MonitoringDescription("Sets whether or not events occuring on this printer are raised by the component")]
        public bool Monitored
        {
            get
            {
                if (_NotificationThread is not null)
                {
                    return _Monitored;
                }
                else
                {
                    _Monitored = false;
                    return false;
                }
            }
            set
            {
                if (value != _Monitored)
                {
                    if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
                    {
                        Trace.WriteLine("Monitored set to : " + value.ToString(), GetType().ToString());
                    }

                    if (value)
                    {
                        if (_NotificationThread is not null)
                        {
                            _NotificationThread.Dispose();
                        }
                        try
                        {
                            var argPrinterInformation = this;
                            _NotificationThread = new PrinterChangeNotificationThread(mhPrinter, _ThreadTimeout, _MonitorLevel, _WatchFlags, ref argPrinterInformation);
                        }
                        catch (Exception e)
                        {
                            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                            {
                                Trace.WriteLine(e.Message + " creating new PrinterChangeNotificationThread for handle : " + mhPrinter.ToString(), GetType().ToString());
                            }
                        }
                        _EventQueue = new EventQueue(_JobEvent, _PrinterEvent);
                        if (_NotificationThread is not null)
                        {
                            _NotificationThread.StartWatching();
                        }
                    }
                    else
                    {
                        if (_NotificationThread is not null)
                        {
                            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
                            {
                                Trace.WriteLine("Stop monitoring  _NotificationThread : " + _NotificationThread.ToString(), GetType().ToString());
                            }
                            _NotificationThread.StopWatching();
                        }
                        if (_EventQueue is not null)
                        {
                            _EventQueue.Shutdown();
                        }
                    }
                    _Monitored = value;
                }
            }
        }
        #endregion

        #region PauseAllNewJobs
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// If true and the printer is being monitored, all print jobs are paused as they 
    /// are added to the spool queue
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// This is useful for print quota type applications as the print job is immediately 
    /// paused allowing the quota program to decide whether or not to delete or resume it
    /// </remarks>
    /// <history>
    /// 	[Duncan]	07/01/2006	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public bool PauseAllNewJobs
        {
            get
            {
                if (_NotificationThread is not null)
                {
                    return _NotificationThread.PauseAllNewPrintJobs;
                }

                return default;
            }
            set
            {
                if (_NotificationThread is not null)
                {
                    _NotificationThread.PauseAllNewPrintJobs = value;
                }
            }
        }
        #endregion

        #region SetEvents
        internal void SetEvents(PrinterMonitorComponent.JobEvent JobEventCallback, PrinterMonitorComponent.PrinterEvent PrinterEventCallback)
        {
            _JobEvent = JobEventCallback;
            _PrinterEvent = PrinterEventCallback;
        }
        #endregion

        #region CanLoggedInUserAdministerPrinter
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public virtual bool CanLoggedInUserAdministerPrinter()
        {
            // \\ Need to get the DACL for the printer and see if the logged in user is allowed the administer role..?
            return true;
        }
        #endregion

        #region Printer Driver
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Returns information about the printer driver used by a given printer
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <example>Prints the name of the printer driver 
    /// <code>
    ///    Dim pi As New PrinterInformation("Microsoft Office Document Image Writer", SpoolerApiConstantEnumerations.PrinterAccessRights.PRINTER_ALL_ACCESS, True)
    ///    Trace.WriteLine(pi.PrinterDriver.Name)
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrinterDriver PrinterDriver
        {
            get
            {
                return new PrinterDriver(mhPrinter);
            }
        }
        #endregion

        #region PrinterForms
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Returns the collection of print forms installed on the printer
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// </remarks>
    /// <example>List the print forms on the named printer
    /// <code>
    ///        Dim pi As New PrinterInformation("HP Laserjet 5L", SpoolerApiConstantEnumerations.PrinterAccessRights.PRINTER_ALL_ACCESS, False)
    /// 
    ///        For pf As Integer = 0 To pi.PrinterForms.Count - 1
    ///            Me.ListBox1.Items.Add( pi.PrinterForms(pf).Name )
    ///        Next
    /// </code>
    /// </example>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrinterFormCollection PrinterForms
        {
            get
            {
                return new PrinterFormCollection(mhPrinter);
            }
        }
        #endregion

        #region Public overrides
        public override string ToString()
        {
            return PrinterName;
        }
        #endregion

        #region Advanced Printer Properties

        /// <summary>
    /// Is the printer for 24-hour availability.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    /// <history>
    /// 	[solidcrip]	13/11/2006	Created
    /// </history>
        public bool IsAlwaysAvailable
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return mPrinter_Info_2.StartTime.CompareTo(mPrinter_Info_2.UntilTime) == 0;
            }
            // Set(ByVal value As Boolean)
            // If Not IsAlwaysAvailable = value Then
            // Availability = New TimeWindow(0, 0)
            // End If
            // End Set
        }

        /// <summary>
    /// Configures the printer for limited availability. 
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks>
    /// If you send a document to a printer when it is unavailable, the document will be held (spooled) until the printer is available.
    /// </remarks>
    /// <history>
    /// 	[solidcrip]	13/11/2006	Created
    /// </history>
        public TimeWindow Availability
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return new TimeWindow(mPrinter_Info_2.StartTime, mPrinter_Info_2.UntilTime);
            }
            set
            {
                if (!(mPrinter_Info_2.StartTime == value.StartTimeMinutes))
                {
                    mPrinter_Info_2.StartTime = value.StartTimeMinutes;
                }
                if (!(mPrinter_Info_2.UntilTime == value.EndTimeMinutes))
                {
                    mPrinter_Info_2.UntilTime = value.EndTimeMinutes;
                }
                SavePrinterInfo(PrinterInfoLevels.PrinterInfoLevel2, false);
            }
        }

        /// <summary>
    /// Specifies that documents should be spooled before being printed.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// Spooling is the process of first storing the document on the hard disk and then sending the document to the print device. You can continue working with your program as soon as the document is stored on the disk. The spooler sends the document to the print device in the background.
    /// <remarks></remarks>
    /// <history>
    /// 	[solidcrip]	13/11/2006	Created
    /// </history>
        public bool SpoolPrintJobs
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return !Conversions.ToBoolean(mPrinter_Info_2.Attributes & (int)PrinterAttributes.PRINTER_ATTRIBUTE_DIRECT);
            }
            set
            {
                // value must be different, otherwise a StackOverflowException will be throwed
                if (!(SpoolPrintJobs == value))
                {
                    if (value)
                    {
                        mPrinter_Info_2.Attributes = mPrinter_Info_2.Attributes ^ (int)PrinterAttributes.PRINTER_ATTRIBUTE_DIRECT;
                    }
                    else
                    {
                        mPrinter_Info_2.Attributes = mPrinter_Info_2.Attributes | (int)PrinterAttributes.PRINTER_ATTRIBUTE_DIRECT;
                    }
                }
                SavePrinterInfo(PrinterInfoLevels.PrinterInfoLevel2, false);
            }
        }

        /// <summary>
    /// Specifies that the print device should wait to begin printing until after the last page of the document is spooled. 
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks>
    /// The printing program is unavailable until the document has finished spooling. 
    /// However, using this option ensures that the whole document is available to the print device.
    /// </remarks>
    /// <history>
    /// 	[solidcrip]	13/11/2006	Created
    /// </history>
        public bool EnableSpoolBeforePrint
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return Conversions.ToBoolean(mPrinter_Info_2.Attributes & (int)PrinterAttributes.PRINTER_ATTRIBUTE_QUEUED);
            }
            set
            {
                // value must be different, otherwise a StackOverflowException will be throwed
                if (!(EnableSpoolBeforePrint == value))
                {
                    if (value)
                    {
                        mPrinter_Info_2.Attributes = mPrinter_Info_2.Attributes | (int)PrinterAttributes.PRINTER_ATTRIBUTE_QUEUED;
                    }
                    else
                    {
                        mPrinter_Info_2.Attributes = mPrinter_Info_2.Attributes ^ (int)PrinterAttributes.PRINTER_ATTRIBUTE_QUEUED;
                    }
                }
                SavePrinterInfo(PrinterInfoLevels.PrinterInfoLevel2, false);
            }
        }

        /// <summary>
    /// Directs the spooler to check the printer setup and match it to the document setup before sending the document to the print device. 
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks>
    /// If the information does not match, the document is held in the queue.
    /// A mismatched document in the queue will not prevent correctly matched documents from printing.
    /// </remarks>
    /// <history>
    /// 	[solidcrip]	13/11/2006	Created
    /// </history>
        public bool HoldMismatchedDocuments
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return Conversions.ToBoolean(mPrinter_Info_2.Attributes & (int)PrinterAttributes.PRINTER_ATTRIBUTE_ENABLE_DEVQ);
            }
            set
            {
                // value must be different, otherwise a StackOverflowException will be throwed
                if (!(HoldMismatchedDocuments == value))
                {
                    if (value)
                    {
                        mPrinter_Info_2.Attributes = mPrinter_Info_2.Attributes | (int)PrinterAttributes.PRINTER_ATTRIBUTE_ENABLE_DEVQ;
                    }
                    else
                    {
                        mPrinter_Info_2.Attributes = mPrinter_Info_2.Attributes ^ (int)PrinterAttributes.PRINTER_ATTRIBUTE_ENABLE_DEVQ;
                    }
                }
                SavePrinterInfo(PrinterInfoLevels.PrinterInfoLevel2, false);
            }
        }

        /// <summary>
    /// Specifies that the spooler should favor documents that have completed spooling when deciding which document to print next, even if the completed documents are a lower priority than documents that are still spooling. 
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks>
    /// If there are no documents that have completed spooling, the spooler will favor larger spooling documents over smaller ones. 
    /// Use this option if you want to maximize printer efficiency.
    /// When this option is disabled, the spooler picks documents based only on priority.
    /// </remarks>
    /// <history>
    /// 	[solidcrip]	13/11/2006	Created
    /// </history>
        public bool PrintSpooledDocumentsFirst
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return Conversions.ToBoolean(mPrinter_Info_2.Attributes & (int)PrinterAttributes.PRINTER_ATTRIBUTE_DO_COMPLETE_FIRST);
            }
            set
            {
                // value must be different, otherwise a StackOverflowException will be throwed
                if (!(PrintSpooledDocumentsFirst == value))
                {
                    if (value)
                    {
                        mPrinter_Info_2.Attributes = mPrinter_Info_2.Attributes | (int)PrinterAttributes.PRINTER_ATTRIBUTE_DO_COMPLETE_FIRST;
                    }
                    else
                    {
                        mPrinter_Info_2.Attributes = mPrinter_Info_2.Attributes ^ (int)PrinterAttributes.PRINTER_ATTRIBUTE_DO_COMPLETE_FIRST;
                    }
                }
                SavePrinterInfo(PrinterInfoLevels.PrinterInfoLevel2, false);
            }
        }

        /// <summary>
    /// Specifies that the spooler should not delete documents after they are printed. 
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks>
    /// This allows a document to be resubmitted to the printer from the printer queue instead of from the program.
    /// </remarks>
    /// <history>
    /// 	[solidcrip]	13/11/2006	Created
    /// </history>
        public bool KeepPrintedDocuments
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return Conversions.ToBoolean(mPrinter_Info_2.Attributes & (int)PrinterAttributes.PRINTER_ATTRIBUTE_KEEPPRINTEDJOBS);
            }
            set
            {
                // value must be different, otherwise a StackOverflowException will be throwed
                if (!(KeepPrintedDocuments == value))
                {
                    if (value)
                    {
                        mPrinter_Info_2.Attributes = mPrinter_Info_2.Attributes | (int)PrinterAttributes.PRINTER_ATTRIBUTE_KEEPPRINTEDJOBS;
                    }
                    else
                    {
                        mPrinter_Info_2.Attributes = mPrinter_Info_2.Attributes ^ (int)PrinterAttributes.PRINTER_ATTRIBUTE_KEEPPRINTEDJOBS;
                    }
                }
                SavePrinterInfo(PrinterInfoLevels.PrinterInfoLevel2, false);
            }
        }

        /// <summary>
    /// Specifies whether the advanced printing feature is enabled. 
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks>
    /// When enabled, metafile spooling is turned on and options such as Page Order, Booklet Printing, and Pages Per Sheet are available, depending on your printer. For normal printing, leave the advanced printing feature set to the default (Enabled). If compatibility problems occur, you can disable the feature. When disabled, metafile spooling is turned off and the printing options might be unavailable.
    /// </remarks>
    /// <history>
    /// 	[solidcrip]	13/11/2006	Created
    /// </history>
        public bool EnableAdvancedPrintingFeatures
        {
            get
            {
                RefreshPrinterInformation(PrinterInfoLevels.PrinterInfoLevel2);
                return Conversions.ToBoolean(mPrinter_Info_2.Attributes & (int)PrinterAttributes.PRINTER_ATTRIBUTE_RAW_ONLY);
            }
            set
            {
                // value must be different, otherwise a StackOverflowException will be thrown
                if (!(EnableAdvancedPrintingFeatures == value))
                {
                    if (value)
                    {
                        mPrinter_Info_2.Attributes = mPrinter_Info_2.Attributes | (int)PrinterAttributes.PRINTER_ATTRIBUTE_RAW_ONLY;
                    }
                    else
                    {
                        mPrinter_Info_2.Attributes = mPrinter_Info_2.Attributes ^ (int)PrinterAttributes.PRINTER_ATTRIBUTE_RAW_ONLY;
                    }
                }
                SavePrinterInfo(PrinterInfoLevels.PrinterInfoLevel2, false);
            }
        }
        #endregion

        #endregion

        #region Private methods
        private void RefreshPrinterInformation(PrinterInfoLevels level)
        {

            if (mhPrinter.Equals(0))
            {
                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                {
                    Trace.WriteLine("RefreshPrinterInformation failed: Handle is invalid", GetType().ToString());
                }
                return;
            }

            switch (level)
            {
                case PrinterInfoLevels.PrinterInfoLevel2:
                    {
                        try
                        {
                            mPrinter_Info_2 = new PRINTER_INFO_2(mhPrinter);
                        }
                        catch (Exception e)
                        {
                            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                            {
                                Trace.WriteLine(e.Message + " getting PRINTER_INFO_2 for handle: " + mhPrinter.ToString(), GetType().ToString());
                            }
                        }

                        break;
                    }

                case PrinterInfoLevels.PrinterInfoLevel3:
                    {
                        try
                        {
                            mPrinter_Info_3 = new PRINTER_INFO_3(mhPrinter);
                        }
                        catch (Exception e)
                        {
                            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                            {
                                Trace.WriteLine(e.Message + " getting PRINTER_INFO_2 for handle: " + mhPrinter.ToString(), GetType().ToString());
                            }
                        }

                        break;
                    }

                default:
                    {
                        break;
                    }
                    // \\ Not yet implemented...
            }
        }

        private void SavePrinterInfo(PrinterInfoLevels Level, bool ModifySecurityDescriptor)
        {

            switch (Level)
            {
                case PrinterInfoLevels.PrinterInfoLevel2:
                    {
                        if (!ModifySecurityDescriptor)
                        {
                            mPrinter_Info_2.lpSecurityDescriptor = IntPtr.Zero;
                        }
                        try
                        {
                            UnsafeNativeMethods.SetPrinter(mhPrinter, PrinterInfoLevels.PrinterInfoLevel2, mPrinter_Info_2, 0);
                        }
                        catch (Exception e)
                        {
                            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                            {
                                Trace.WriteLine(e.Message + " setting PRINTER_INFO_2 for handle: " + mhPrinter.ToString(), GetType().ToString());
                            }
                            throw new InsufficentPrinterAccessRightsException(e);
                        }

                        break;
                    }


                case PrinterInfoLevels.PrinterInfoLevel3:
                    {
                        try
                        {
                            UnsafeNativeMethods.SetPrinter(mhPrinter, PrinterInfoLevels.PrinterInfoLevel3, mPrinter_Info_3, 0);
                        }
                        catch (Exception e)
                        {
                            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                            {
                                Trace.WriteLine(e.Message + " setting PRINTER_INFO_3 for handle: " + mhPrinter.ToString(), GetType().ToString());
                            }
                            throw new InsufficentPrinterAccessRightsException(e);
                        }

                        break;
                    }

                default:
                    {
                        break;
                    }
                    // \\ Not yet implemented...
            }

        }
        #endregion

        #region Public constructors

        private void InitPrinterInfo(bool GetJobs)
        {
            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("InitPrinterInfo()", GetType().ToString());
            }
            // \\ Get the current printer information
            try
            {
                mPrinter_Info_2 = new PRINTER_INFO_2(mhPrinter);
            }
            catch (Exception e)
            {
                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                {
                    Trace.WriteLine(e.Message + " creating new PRINTER_INFO_2 for handle : " + mhPrinter.ToString(), GetType().ToString());
                }
                throw;
                return;
            }
            if (GetJobs)
            {
                try
                {
                    _PrintJobs = new PrintJobCollection(mhPrinter, JobCount);
                }
                catch (Exception e)
                {
                    if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                    {
                        Trace.WriteLine(e.Message + " creating new PRINTER_INFO_3 for handle : " + mhPrinter.ToString(), GetType().ToString());
                    }
                    throw;
                }
            }
        }

        internal PrinterInformation(IntPtr PrinterHandle)
        {
            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("New(" + PrinterHandle.ToString() + ")", GetType().ToString());
            }
            mhPrinter = PrinterHandle;
            InitPrinterInfo(true);
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Creates a new printer information class for the named printer
    /// </summary>
    /// <param name="DeviceName">The name of the print device</param>
    /// <param name="DesiredAccess">The required access rights for that printer</param>
    /// <param name="GetJobs">True to return the collection of print jobs
    /// queued against this print device
    /// </param>
    /// <remarks>
    /// 
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrinterInformation(string DeviceName, PrinterAccessRights DesiredAccess, bool GetJobs) : this(DeviceName, DesiredAccess, (DesiredAccess & PrinterAccessRights.SERVER_ACCESS_ADMINISTER | DesiredAccess & PrinterAccessRights.PRINTER_ACCESS_ADMINISTER) != 0, GetJobs)
        {
            bHandleOwnedByMe = true;
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Creates a new printer information class for the named printer
    /// </summary>
    /// <param name="DeviceName">The name of the print device</param>
    /// <param name="DesiredAccess">The required access rights for that printer</param>
    /// <param name="GetSecurityInfo"></param>
    /// <param name="GetJobs">True to return the collection of print jobs
    /// queued against this print device
    /// </param>
    /// <remarks>
    /// 
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrinterInformation(string DeviceName, PrinterAccessRights DesiredAccess, bool GetSecurityInfo, bool GetJobs)
        {
            var hPrinter = IntPtr.Zero;
            if (UnsafeNativeMethods.OpenPrinter(DeviceName, out hPrinter, new PRINTER_DEFAULTS(DesiredAccess)))
            {
                mhPrinter = hPrinter;
                InitPrinterInfo(GetJobs);
            }
            else if (UnsafeNativeMethods.OpenPrinter(DeviceName, out hPrinter, new PRINTER_DEFAULTS(PrinterAccessRights.PRINTER_ALL_ACCESS)))
            {
                mhPrinter = hPrinter;
                InitPrinterInfo(GetJobs);
            }
            else if (UnsafeNativeMethods.OpenPrinter(DeviceName, out hPrinter, 0))
            {
                mhPrinter = hPrinter;
                InitPrinterInfo(GetJobs);
            }
            else
            {
                if (PrinterMonitorComponent.ComponentTraceSwitch.TraceError)
                {
                    Trace.WriteLine("OpenPrinter() failed", GetType().ToString());
                }
                throw new Win32Exception();
            }
        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Creates a new printer information class for the named printer
    /// </summary>
    /// <param name="DeviceName">The name of the print device</param>
    /// <param name="DesiredAccess">The required access rights for that printer</param>
    /// <param name="ThreadTimeout">No longer used</param>
    /// <param name="MonitorLevel"></param>
    /// <param name="WatchFlags"></param>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrinterInformation(string DeviceName, PrinterAccessRights DesiredAccess, int ThreadTimeout, PrinterMonitorComponent.MonitorJobEventInformationLevels MonitorLevel, int WatchFlags) : this(DeviceName, DesiredAccess, true)
        {
            _ThreadTimeout = ThreadTimeout;
            _MonitorLevel = MonitorLevel;
            _WatchFlags = WatchFlags;
        }

        internal PrinterInformation(string DeviceName, string Description, string Comment, string ServerName, int Index)



        {

            // \\ -- Don't try to refresh this printer info...
            bHandleOwnedByMe = false;
            mPrinter_Info_2 = new PRINTER_INFO_2();
            {
                ref var withBlock = ref mPrinter_Info_2;
                withBlock.pPrinterName = DeviceName;
                withBlock.pComment = Comment;
                withBlock.pLocation = Description;
                withBlock.pServerName = ServerName;
            }

        }
        #endregion

        #region Friend methods
        internal string MakeUrl(string TransportProtocol, int Port)
        {
            // \\ Return the URL  
            string sRet;
            if (!string.IsNullOrEmpty(ServerName))
            {
                sRet = TransportProtocol + "://" + ServerName + ":" + Port.ToString() + "/SpoolMonitorService";
            }
            else
            {
                sRet = TransportProtocol + "://localhost:" + Port.ToString() + "/SpoolMonitorService";
            }
            if (PrinterMonitorComponent.ComponentTraceSwitch.TraceVerbose)
            {
                Trace.WriteLine("MakeUrl : " + sRet, GetType().ToString());
            }
            return sRet;
        }

        #endregion

    }

    #region PrinterInformationCollection
    /// -----------------------------------------------------------------------------
#Region "PrinterInformationCollection"/// Project	 : PrinterQueueWatch
#Region "PrinterInformationCollection"/// Class	 : PrinterInformationCollection
#Region "PrinterInformationCollection"/// 
#Region "PrinterInformationCollection"/// -----------------------------------------------------------------------------
#Region "PrinterInformationCollection"/// <summary>
#Region "PrinterInformationCollection"/// A collection of printer information classes
#Region "PrinterInformationCollection"/// </summary>
#Region "PrinterInformationCollection"/// <remarks>
#Region "PrinterInformationCollection"/// </remarks>
#Region "PrinterInformationCollection"/// <history>
#Region "PrinterInformationCollection"/// 	[Duncan]	20/11/2005	Created
#Region "PrinterInformationCollection"/// </history>
#Region "PrinterInformationCollection"/// -----------------------------------------------------------------------------
    [System.Security.SuppressUnmanagedCodeSecurity()]
    [ComVisible(false)]
    public class PrinterInformationCollection : System.Collections.Generic.List<PrinterInformation>
    {

        #region Public interface
        public new void Remove(PrinterInformation obj)
        {
            base.Remove(obj);
        }
        #endregion

        #region Public constructors
        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Creates a collection of printer information for the current machine 
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    ///     [Duncan]    23/02/2008  Changed to use PRINTER_INFO_1 structure
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrinterInformationCollection()
        {
            int pcbNeeded; // \\ Holds the requires size of the output buffer (in bytes)
            int pcReturned; // \\ Holds the returned size of the output buffer 
            IntPtr pPrinters;
            int pcbProvided = 0;

            if (!UnsafeNativeMethods.EnumPrinters(EnumPrinterFlags.PRINTER_ENUM_NAME, string.Empty, 1, pPrinters, 0, out pcbNeeded, out pcReturned))
            {
                if (pcbNeeded > 0)
                {
                    pPrinters = Marshal.AllocHGlobal(pcbNeeded);
                    pcbProvided = pcbNeeded;
                    if (!UnsafeNativeMethods.EnumPrinters(EnumPrinterFlags.PRINTER_ENUM_NAME, string.Empty, 1, pPrinters, pcbProvided, out pcbNeeded, out pcReturned))
                    {
                        throw new Win32Exception();
                    }
                }
            }

            if (pcReturned > 0)
            {
                // \\ Get all the monitors for the given server
                var ptNext = pPrinters;
                while (pcReturned > 0)
                {
                    var pi1 = new PRINTER_INFO_1();
                    Marshal.PtrToStructure(ptNext, pi1);
                    if (pi1.pName is not null)
                    {
                        Add(new PrinterInformation(pi1.pName, PrinterAccessRights.PRINTER_ACCESS_USE, false)); // , pi2.pLocation, pi2.pComment, pi2.pServerName, 1))
                    }
                    ptNext = ptNext + Marshal.SizeOf(typeof(PRINTER_INFO_1));
                    pcReturned -= 1;
                }
            }

            // \\ Free the allocated buffer memory
            if (pPrinters.ToInt64() > 0L)
            {
                Marshal.FreeHGlobal(pPrinters);
            }

        }

        /// -----------------------------------------------------------------------------
    /// <summary>
    /// Creates a collection of printer information objects for the named machine 
    /// </summary>
    /// <param name="Servername">The name of the server to list the printer devices</param>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Duncan]	20/11/2005	Created
    ///     [Duncan]    01/05/2014  Use IntPtr for 32/64 bit compatibility
    /// </history>
    /// -----------------------------------------------------------------------------
        public PrinterInformationCollection(string Servername)
        {
            int pcbNeeded; // \\ Holds the requires size of the output buffer (in bytes)
            int pcReturned; // \\ Holds the returned size of the output buffer 
            IntPtr pPrinters;
            int pcbProvided = 0;

            if (!UnsafeNativeMethods.EnumPrinters(EnumPrinterFlags.PRINTER_ENUM_NAME, Servername, 1, pPrinters, 0, out pcbNeeded, out pcReturned))
            {
                if (pcbNeeded > 0)
                {
                    pPrinters = Marshal.AllocHGlobal(pcbNeeded);
                    pcbProvided = pcbNeeded;
                    if (!UnsafeNativeMethods.EnumPrinters(EnumPrinterFlags.PRINTER_ENUM_NAME, Servername, 1, pPrinters, pcbProvided, out pcbNeeded, out pcReturned))
                    {
                        throw new Win32Exception();
                    }
                }
            }

            if (pcReturned > 0)
            {
                // \\ Get all the monitors for the given server
                var ptNext = pPrinters;
                while (pcReturned > 0)
                {
                    var pi2 = new PRINTER_INFO_2();
                    Marshal.PtrToStructure(ptNext, pi2);
                    Add(new PrinterInformation(pi2.pPrinterName, pi2.pLocation, pi2.pComment, pi2.pServerName, 1));
                    ptNext = ptNext + Marshal.SizeOf(pi2) - Marshal.SizeOf(typeof(int));
                    pcReturned -= 1;
                }
            }

            // \\ Free the allocated buffer memory
            if (pPrinters.ToInt64() > 0L)
            {
                Marshal.FreeHGlobal(pPrinters);
            }

        }
        #endregion

    }
}
#endregion
