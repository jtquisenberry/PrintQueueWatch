using System;
// \\ --[SpoolerApiConstantEnumerations]-------------------------------------------------------
// \\ Namespace for all the spooler related enumerated types, from winspool.drv
// \\ (c) 2003 Merrion Computing Ltd
// \\ http://www.merrioncomputing.com
// \\ ------------------------------------------------------------------------------------------
namespace PrinterQueueWatch.SpoolerApiConstantEnumerations
{

    #region PrinterChangeNotificationGeneralFlags
    [Flags()]
    public enum PrinterChangeNotificationGeneralFlags
    {
        PRINTER_CHANGE_JOB = 0xFF00,
        PRINTER_CHANGE_PRINTER = 0xFF,
        PRINTER_CHANGE_FORM = 0x70000,
        PRINTER_CHANGE_PORT = 0x700000,
        PRINTER_CHANGE_PRINT_PROCESSOR = 0x7000000,
        PRINTER_CHANGE_PRINTER_DRIVER = 0x70000000
    }
    #endregion

    #region PrinterChangeNotificationFormFlags
    [Flags()]
    public enum PrinterChangeNotificationFormFlags
    {
        PRINTER_CHANGE_NO_FORM_CHANGE = 0x0,
        PRINTER_CHANGE_ADD_FORM = 0x10000,
        PRINTER_CHANGE_SET_FORM = 0x20000,
        PRINTER_CHANGE_DELETE_FORM = 0x40000
    }
    #endregion

    #region PrinterChangeNotificationPortFlags
    [Flags()]
    public enum PrinterChangeNotificationPortFlags
    {
        PRINTER_CHANGE_NO_PORT_CHANGE = 0x0,
        PRINTER_CHANGE_ADD_PORT = 0x100000,
        PRINTER_CHANGE_CONFIGURE_PORT = 0x200000,
        PRINTER_CHANGE_DELETE_PORT = 0x400000
    }
    #endregion

    #region PrinterChangeNotificationJobFlags
    [Flags()]
    public enum PrinterChangeNotificationJobFlags
    {
        PRINTER_CHANGE_NO_JOB_CHANGE = 0x0,
        PRINTER_CHANGE_ADD_JOB = 0x100,
        PRINTER_CHANGE_SET_JOB = 0x200,
        PRINTER_CHANGE_DELETE_JOB = 0x400,
        PRINTER_CHANGE_WRITE_JOB = 0x800
    }
    #endregion

    #region PrinterChangeNotificationPrinterFlags
    [Flags()]
    public enum PrinterChangeNotificationPrinterFlags
    {
        PRINTER_CHANGE_NO_PRINTER_CHANGE = 0x0,
        PRINTER_CHANGE_ADD_PRINTER = 0x1,
        PRINTER_CHANGE_SET_PRINTER = 0x2,
        PRINTER_CHANGE_DELETE_PRINTER = 0x4,
        PRINTER_CHANGE_FAILED_CONNECTION_PRINTER = 0x8
    }
    #endregion

    #region PrinterChangeNotificationProcessorFlags
    [Flags()]
    public enum PrinterChangeNotificationProcessorFlags
    {
        PRINTER_CHANGE_NO_PRINT_PROCESSOR_CHANGE = 0x0,
        PRINTER_CHANGE_ADD_PRINT_PROCESSOR = 0x1000000,
        PRINTER_CHANGE_DELETE_PRINT_PROCESSOR = 0x4000000
    }
    #endregion

    #region PrinterChangeNotificationDriverFlags
    [Flags()]
    public enum PrinterChangeNotificationDriverFlags
    {
        PRINTER_CHANGE_NO_DRIVER_CHANGE = 0x0,
        PRINTER_CHANGE_ADD_PRINTER_DRIVER = 0x10000000,
        PRINTER_CHANGE_SET_PRINTER_DRIVER = 0x20000000,
        PRINTER_CHANGE_DELETE_PRINTER_DRIVER = 0x40000000
    }
    #endregion

    #region PrintJobStatuses
    [Flags()]
    public enum PrintJobStatuses
    {
        // JOB_STATUS_PAUSED The print job has been paused during printing
        JOB_STATUS_PAUSED = 0x1,
        // JOB_STATUS_ERROR The print job is stalled by an error
        JOB_STATUS_ERROR = 0x2,
        // JOB_STATUS_DELETING The print job is being deleted from the queue
        JOB_STATUS_DELETING = 0x4,
        // JOB_STATUS_SPOOLING The print job is being added to the spooler queue
        JOB_STATUS_SPOOLING = 0x8,
        // JOB_STATUS_PRINTING The print job is in the midst of being printed
        JOB_STATUS_PRINTING = 0x10,
        // JOB_STATUS_OFFLINE The print job is stalled because the printer is off line
        JOB_STATUS_OFFLINE = 0x20,
        // JOB_STATUS_PAPEROUT The print job is stalled because the printer has run out of paper
        JOB_STATUS_PAPEROUT = 0x40,
        // JOB_STATUS_PRINTER The print job is waiting on the printer
        JOB_STATUS_PRINTED = 0x80,
        // JOB_STATUS_DELETED The print job has been deleted
        JOB_STATUS_DELETED = 0x100,
        // JOB_STATUS_BLOCKED_DEVICEQUEUE The device queue is blocked so the print job is also blocked
        JOB_STATUS_BLOCKED_DEVICEQUEUE = 0x200,
        // JOB_STATUS_USER_INTERVENTION The queued job is paused by user intervention
        JOB_STATUS_USER_INTERVENTION = 0x400,
        // JOB_STATUS_RESTART The queued print job is restarting after being paused
        JOB_STATUS_RESTART = 0x800
    }
    #endregion

    #region DeviceOrientations
    // DeviceOrientations describe the orientation of the device
    public enum DeviceOrientations
    {
        // DMORIENT_PORTRAIT The device is in portait (short edge on top) mode
        DMORIENT_PORTRAIT = 1,
        // DMORIENT_LANDSCAPE The device is in landscape (long edge on top) mode
        DMORIENT_LANDSCAPE = 2
    }
    #endregion

    #region PrintJobControlCommands
    public enum PrintJobControlCommands
    {
        // JOB_CONTROL_SETJOB Set the job info
        JOB_CONTROL_SETJOB = 0,
        // JOB_CONTROL_PAUSE Pause printing of the current job
        JOB_CONTROL_PAUSE = 1,
        // JOB_CONTROL_RESUME Resume printing a paused job
        JOB_CONTROL_RESUME = 2,
        // JOB_CONTROL_CANCEL Cancel a queued or printing job
        JOB_CONTROL_CANCEL = 3,
        // JOB_CONTROL_RESTART Restart a paused job that was printing
        JOB_CONTROL_RESTART = 4,
        // JOB_CONTROL_DELETE Remove a job from the queue
        JOB_CONTROL_DELETE = 5,
        // JOB_CONTROL_SENT_TO_PRINTER End a print job
        JOB_CONTROL_SENT_TO_PRINTER = 6,
        // JOB_CONTROL_LAST_PAGE_EJECTED End a print job
        JOB_CONTROL_LAST_PAGE_EJECTED = 7
    }
    #endregion

    #region DeviceModeResolutions
    // DeviceModeResolutions are the different resolution settings for devices that _
    // support multiple resolutions
    public enum DeviceModeResolutions
    {
        // DMRES_DRAFT Device is set to draft quality
        DMRES_DRAFT = -1,
        // DMRES_LOW Device is set to low resolution quality
        DMRES_LOW = -2,
        // DMRES_MEDIUM Device is set to an intermediate resolution quality
        DMRES_MEDIUM = -3,
        // DMRES_HIGH Devide is set to high resolution quality
        DMRES_HIGH = -4
    }
    #endregion

    #region DeviceColourModes
    // DeviceColourModes are the possible colour use settings for colour devices
    public enum DeviceColourModes
    {
        // DMCOLOR_MONOCHROME Do not use colour
        DMCOLOR_MONOCHROME = 1,
        // DMCOLOR_COLOR Do use colour
        DMCOLOR_COLOR = 2
    }
    #endregion

    #region DeviceDuplexSettings
    // DeviceDuplexSettings is how a duplex device is set for duplex printing
    public enum DeviceDuplexSettings
    {
        // DMDUP_SIMPLEX Device is not using double sided printing
        DMDUP_SIMPLEX = 1,
        // DMDUP_VERTICAL Device is performing double sided printing on the short edge
        DMDUP_VERTICAL = 2,
        // DMDUP_HORIZONTAL Device is performing double sided printing on the long edge
        DMDUP_HORIZONTAL = 3
    }
    #endregion

    #region DeviceTrueTypeFontSettings
    // DeviceTrueTypeFontSettings specifies how the device handles TrueType fonts
    public enum DeviceTrueTypeFontSettings
    {
        // DMTT_BITMAP Device treats TrueType fonts as bitmaps
        DMTT_BITMAP = 1,
        // DMTT_DOWNLOAD Device downloads TrueType fonts as soft fonts
        DMTT_DOWNLOAD = 2,
        // DMTT_SUBDEV Device substitutes TrueType fonts for the nearest printer font
        DMTT_SUBDEV = 3,
        // DMTT_DOWNLOAD_OUTLINE Device downloads TrueType fonts as outline soft fonts
        DMTT_DOWNLOAD_OUTLINE = 4
    }
    #endregion

    #region DeviceCollateSettings
    // DeviceCollateSettings set whether or not multi-copy print jobs should be collated
    public enum DeviceCollateSettings
    {
        // DMCOLLATE_FALSE Do not collate the jobs
        DMCOLLATE_FALSE = 0,
        // DMCOLLATE_TRUE Do collate the jobs
        DMCOLLATE_TRUE = 1
    }
    #endregion

    #region EnumPrinterFlags
    [Flags()]
    public enum EnumPrinterFlags
    {
        PRINTER_ENUM_DEFAULT = 0x1,
        PRINTER_ENUM_LOCAL = 0x2,
        PRINTER_ENUM_CONNECTIONS = 0x4,
        PRINTER_ENUM_FAVORITE = 0x4,
        PRINTER_ENUM_NAME = 0x8,
        PRINTER_ENUM_REMOTE = 0x10,
        PRINTER_ENUM_SHARED = 0x20,
        PRINTER_ENUM_NETWORK = 0x40
    }
    #endregion

    #region JobInfoLevels
    // JobInfoLevels - The level (JOB_LEVEL_n) structure to read from the spooler
    public enum JobInfoLevels
    {
        // JobInfoLevel1 - Read a JOB_INFO_1 structure
        JobInfoLevel1 = 0x1,
        // JobInfoLevel2 - Read a JOB_INFO_2 structure
        JobInfoLevel2 = 0x2,
        // JobInfoLevel3 - Read a JOB_INFO_3 structure
        JobInfoLevel3 = 0x3
    }
    #endregion

    #region PrinterAccessRights
    [Flags()]
    public enum PrinterAccessRights : int
    {
        // READ_CONTROL - Allowed to read printer information
        READ_CONTROL = 0x20000,
        // WRITE_DAC - Allowed to write device access control info
        WRITE_DAC = 0x40000,
        // WRITE_OWNER - Allowed to change the object owner
        WRITE_OWNER = 0x80000,
        // SERVER_ACCESS_ADMINISTER 
        SERVER_ACCESS_ADMINISTER = 0x1,
        // SERVER_ACCESS_ENUMERATE
        SERVER_ACCESS_ENUMERATE = 0x2,
        // PRINTER_ACCESS_ADMINISTER Allows administration of a printer
        PRINTER_ACCESS_ADMINISTER = 0x4,
        // PRINTER_ACCESS_USE Allows printer general use (printing, querying)
        PRINTER_ACCESS_USE = 0x8,
        // PRINTER_ALL_ACCESS Allows use and administration.
        PRINTER_ALL_ACCESS = 0xF000C,
        // SERVER_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | SERVER_ACCESS_ADMINISTER | SERVER_ACCESS_ENUMERATE)
        SERVER_ALL_ACCESS = 0xF0003
    }
    #endregion

    #region PrinterControlCommands
    public enum PrinterControlCommands
    {
        // PRINTER_CONTROL_SETPRINTERINFO - Update the printer info data
        PRINTER_CONTROL_SETPRINTERINFO = 0,
        // PRINTER_CONTROL_PAUSE Pause the printing of the currently active job
        PRINTER_CONTROL_PAUSE = 1,
        // PRINTER_CONTROL_RESUME Resume printing if paused
        PRINTER_CONTROL_RESUME = 2,
        // PRINTER_CONTROL_PURGE Terminate and delete the currently printing job
        PRINTER_CONTROL_PURGE = 3,
        // PRINTER_CONTROL_SET_STATUS Set the printer status for the current job
        PRINTER_CONTROL_SET_STATUS = 4
    }
    #endregion

    #region PrinterStatuses
    // PrinterStatuses are the various possible states a printer can be in. Note that this information is provided by the printer driver and in many cases the status will always be "Ready" unless there is one or more jobs in error on that printer.
    [Flags()]
    public enum PrinterStatuses
    {
        // PRINTER_STATUS_READY The printer is free and ready to print
        PRINTER_STATUS_READY = 0x0,
        // PRINTER_STATUS_PAUSED Printing is paused
        PRINTER_STATUS_PAUSED = 0x1,
        // PRINTER_STATUS_ERROR Printing is suspended due to a general error
        PRINTER_STATUS_ERROR = 0x2,
        // PRINTER_STATUS_PENDING_DELETION Printing is suspended while a job is being deleted
        PRINTER_STATUS_PENDING_DELETION = 0x4,
        // PRINTER_STATUS_PAPER_JAM Printing is suspended because there has been a paper jam
        PRINTER_STATUS_PAPER_JAM = 0x8,
        // PRINTER_STATUS_PAPER_OUT Printing is suspended because the printer has run out of paper
        PRINTER_STATUS_PAPER_OUT = 0x10,
        // PRINTER_STATUS_MANUAL_FEED Printing is suspended pending a manual paper feed
        PRINTER_STATUS_MANUAL_FEED = 0x20,
        // PRINTER_STATUS_PAPER_PROBLEM Printing is supended due to a paper problem
        PRINTER_STATUS_PAPER_PROBLEM = 0x40,
        // PRINTER_STATUS_OFFLINE The printer is off line
        PRINTER_STATUS_OFFLINE = 0x80,
        // PRINTER_STATUS_IO_ACTIVE The printer is reading data from the IO port
        PRINTER_STATUS_IO_ACTIVE = 0x100,
        // PRINTER_STATUS_BUSY The printer is busy
        PRINTER_STATUS_BUSY = 0x200,
        // PRINTER_STATUS_PRINTING The printer is active and printing a job
        PRINTER_STATUS_PRINTING = 0x400,
        // PRINTER_STATUS_OUTPUT_BIN_FULL The output tray is full and the printer has paused until it is emptied
        PRINTER_STATUS_OUTPUT_BIN_FULL = 0x800,
        // PRINTER_STATUS_NOT_AVAILABLE The printer status is not known
        PRINTER_STATUS_NOT_AVAILABLE = 0x1000,
        // PRINTER_STATUS_WAITING The printer is waiting for a job
        PRINTER_STATUS_WAITING = 0x2000,
        // PRINTER_STATUS_PROCESSING The printer is active and processing a print job
        PRINTER_STATUS_PROCESSING = 0x4000,
        // PRINTER_STATUS_INITIALIZING The printer is initialising and not yet ready to print
        PRINTER_STATUS_INITIALIZING = 0x8000,
        // PRINTER_STATUS_WARMING_UP The printer is warming up and not yet ready to print
        PRINTER_STATUS_WARMING_UP = 0x10000,
        // PRINTER_STATUS_TONER_LOW Printing is suspended because the level of toner or ink is too low for a reasonable quality print
        PRINTER_STATUS_TONER_LOW = 0x20000,
        // PRINTER_STATUS_NO_TONER Printing has been suspended because the printer is out of toner or ink
        PRINTER_STATUS_NO_TONER = 0x40000,
        // PRINTER_STATUS_PAGE_PUNT (Win95 only) The printer is suspended while a page is deleted
        PRINTER_STATUS_PAGE_PUNT = 0x80000, // win95
        // PRINTER_STATUS_USER_INTERVENTION The printer has been paused by the user intervention
        PRINTER_STATUS_USER_INTERVENTION = 0x100000,
        // PRINTER_STATUS_OUT_OF_MEMORY Printing is suspended because the printer has run out of memory
        PRINTER_STATUS_OUT_OF_MEMORY = 0x200000,
        // PRINTER_STATUS_DOOR_OPEN Printing is supended because a door on the printer is open
        PRINTER_STATUS_DOOR_OPEN = 0x400000,
        // PRINTER_STATUS_SERVER_UNKNOWN Printing is suspended due to an unknown server error
        PRINTER_STATUS_SERVER_UNKNOWN = 0x800000,
        // PRINTER_STATUS_POWER_SAVE The printer is suspended in power saving mode
        PRINTER_STATUS_POWER_SAVE = 0x1000000
    }
    #endregion

    #region PrinterAttributes
    [Flags()]
    public enum PrinterAttributes
    {
        // PRINTER_ATTRIBUTE_QUEUED
        PRINTER_ATTRIBUTE_QUEUED = 0x1,
        // PRINTER_ATTRIBUTE_DIRECT
        PRINTER_ATTRIBUTE_DIRECT = 0x2,
        // PRINTER_ATTRIBUTE_DEFAULT
        PRINTER_ATTRIBUTE_DEFAULT = 0x4,
        // PRINTER_ATTRIBUTE_SHARED
        PRINTER_ATTRIBUTE_SHARED = 0x8,
        // PRINTER_ATTRIBUTE_NETWORK
        PRINTER_ATTRIBUTE_NETWORK = 0x10,
        // PRINTER_ATTRIBUTE_HIDDEN
        PRINTER_ATTRIBUTE_HIDDEN = 0x20,
        // PRINTER_ATTRIBUTE_LOCAL
        PRINTER_ATTRIBUTE_LOCAL = 0x40,
        // PRINTER_ATTRIBUTE_ENABLE_DEVQ
        PRINTER_ATTRIBUTE_ENABLE_DEVQ = 0x80,
        // PRINTER_ATTRIBUTE_KEEPPRINTEDJOBS
        PRINTER_ATTRIBUTE_KEEPPRINTEDJOBS = 0x100,
        // PRINTER_ATTRIBUTE_DO_COMPLETE_FIRST
        PRINTER_ATTRIBUTE_DO_COMPLETE_FIRST = 0x200,
        // PRINTER_ATTRIBUTE_WORK_OFFLINE
        PRINTER_ATTRIBUTE_WORK_OFFLINE = 0x400,
        // PRINTER_ATTRIBUTE_ENABLE_BIDI The printer can operate in bidirectional mode
        PRINTER_ATTRIBUTE_ENABLE_BIDI = 0x800,
        // PRINTER_ATTRIBUTE_RAW_ONLY The printer can only accept raw data
        PRINTER_ATTRIBUTE_RAW_ONLY = 0x1000,
        // PRINTER_ATTRIBUTE_PUBLISHED The printer is shared as a network resource
        PRINTER_ATTRIBUTE_PUBLISHED = 0x2000
    }
    #endregion

    #region PrinterInfoLevels
    // PrinterInfoLevels - The level (PRINTER_INFO_n) structure to read from the spooler
    public enum PrinterInfoLevels
    {
        // PrinterInfoLevel1 - Read a PRINT_INFO_1 structure
        PrinterInfoLevel1 = 1,
        // PrinterInfoLevel2 - Read a PRINT_INFO_2 structure
        PrinterInfoLevel2 = 2,
        // PrinterInfoLevel3 - Read a PRINT_INFO_3 structure (Windows NT/2000/XP only)
        PrinterInfoLevel3 = 3,
        // PrinterInfoLevel4 - Read a PRINT_INFO_4 structure (Windows NT/2000/XP only)
        PrinterInfoLevel4 = 4,
        // PrinterInfoLevel5 - Read a PRINT_INFO_5 structure
        PrinterInfoLevel5 = 5,
        // PrinterInfoLevel6 - Read a PRINT_INFO_6 structure
        PrinterInfoLevel6 = 6,
        // PrinterInfoLevel7 - Read a PRINT_INFO_7 structure (Windows 2000/XP only)
        PrinterInfoLevel7 = 7,
        // PrinterInfoLevel8 - Read a PRINT_INFO_8 structure (Windows 2000/XP only)
        PrinterInfoLevel8 = 8,
        // PrinterInfoLevel9 - Read a PRINT_INFO_9 structure (Windows 2000/XP only)
        PrinterInfoLevel9 = 9
    }
    #endregion

    #region Printer_Notification_Types
    public enum Printer_Notification_Types
    {
        PRINTER_NOTIFY_TYPE = 0x0,
        JOB_NOTIFY_TYPE = 0x1
    }
    #endregion

    #region Printer_Notify_Field_Indexes
    public enum Printer_Notify_Field_Indexes : short
    {
        PRINTER_NOTIFY_FIELD_SERVER_NAME = 0x0,
        PRINTER_NOTIFY_FIELD_PRINTER_NAME = 0x1,
        PRINTER_NOTIFY_FIELD_SHARE_NAME = 0x2,
        PRINTER_NOTIFY_FIELD_PORT_NAME = 0x3,
        PRINTER_NOTIFY_FIELD_DRIVER_NAME = 0x4,
        PRINTER_NOTIFY_FIELD_COMMENT = 0x5,
        PRINTER_NOTIFY_FIELD_LOCATION = 0x6,
        PRINTER_NOTIFY_FIELD_DEVMODE = 0x7,
        PRINTER_NOTIFY_FIELD_SEPFILE = 0x8,
        PRINTER_NOTIFY_FIELD_PRINT_PROCESSOR = 0x9,
        PRINTER_NOTIFY_FIELD_PARAMETERS = 0xA,
        PRINTER_NOTIFY_FIELD_DATATYPE = 0xB,
        PRINTER_NOTIFY_FIELD_SECURITY_DESCRIPTOR = 0xC,
        PRINTER_NOTIFY_FIELD_ATTRIBUTES = 0xD,
        PRINTER_NOTIFY_FIELD_PRIORITY = 0xE,
        PRINTER_NOTIFY_FIELD_DEFAULT_PRIORITY = 0xF,
        PRINTER_NOTIFY_FIELD_START_TIME = 0x10,
        PRINTER_NOTIFY_FIELD_UNTIL_TIME = 0x11,
        PRINTER_NOTIFY_FIELD_STATUS = 0x12,
        PRINTER_NOTIFY_FIELD_STATUS_STRING = 0x13,
        PRINTER_NOTIFY_FIELD_CJOBS = 0x14,
        PRINTER_NOTIFY_FIELD_AVERAGE_PPM = 0x15,
        PRINTER_NOTIFY_FIELD_TOTAL_PAGES = 0x16,
        PRINTER_NOTIFY_FIELD_PAGES_PRINTED = 0x17,
        PRINTER_NOTIFY_FIELD_TOTAL_BYTES = 0x18,
        PRINTER_NOTIFY_FIELD_BYTES_PRINTED = 0x19,
        PRINTER_NOTIFY_FIELD_OBJECT_GUID = 0x1A
    }
    #endregion

    #region Job_Notify_Field_Indexes
    public enum Job_Notify_Field_Indexes : short
    {
        JOB_NOTIFY_FIELD_PRINTER_NAME = 0x0,
        JOB_NOTIFY_FIELD_MACHINE_NAME = 0x1,
        JOB_NOTIFY_FIELD_PORT_NAME = 0x2,
        JOB_NOTIFY_FIELD_USER_NAME = 0x3,
        JOB_NOTIFY_FIELD_NOTIFY_NAME = 0x4,
        JOB_NOTIFY_FIELD_DATATYPE = 0x5,
        JOB_NOTIFY_FIELD_PRINT_PROCESSOR = 0x6,
        JOB_NOTIFY_FIELD_PARAMETERS = 0x7,
        JOB_NOTIFY_FIELD_DRIVER_NAME = 0x8,
        JOB_NOTIFY_FIELD_DEVMODE = 0x9,
        JOB_NOTIFY_FIELD_STATUS = 0xA,
        JOB_NOTIFY_FIELD_STATUS_STRING = 0xB,
        JOB_NOTIFY_FIELD_SECURITY_DESCRIPTOR = 0xC,
        JOB_NOTIFY_FIELD_DOCUMENT = 0xD,
        JOB_NOTIFY_FIELD_PRIORITY = 0xE,
        JOB_NOTIFY_FIELD_POSITION = 0xF,
        JOB_NOTIFY_FIELD_SUBMITTED = 0x10,
        JOB_NOTIFY_FIELD_START_TIME = 0x11,
        JOB_NOTIFY_FIELD_UNTIL_TIME = 0x12,
        JOB_NOTIFY_FIELD_TIME = 0x13,
        JOB_NOTIFY_FIELD_TOTAL_PAGES = 0x14,
        JOB_NOTIFY_FIELD_PAGES_PRINTED = 0x15,
        JOB_NOTIFY_FIELD_TOTAL_BYTES = 0x16,
        JOB_NOTIFY_FIELD_BYTES_PRINTED = 0x17
    }
    #endregion

    #region PrinterPaperBins
    public enum PrinterPaperBins
    {
        // DMBIN_UPPER The upper (or only) tray
        DMBIN_UPPER = 1,
        // DMBIN_LOWER The lower paper tray
        DMBIN_LOWER = 2,
        // DMBIN_MIDDLE The middle paper tray
        DMBIN_MIDDLE = 3,
        // DMBIN_MANUAL The manual form feed
        DMBIN_MANUAL = 4,
        // DMBIN_ENVELOPE The envelope tray
        DMBIN_ENVELOPE = 5,
        // DMBIN_ENVMANUAL Manual envelope feed
        DMBIN_ENVMANUAL = 6,
        // DMBIN_AUTO The printer automatically selects the source
        DMBIN_AUTO = 7,
        // DMBIN_TRACTOR The tractor feed paper source
        DMBIN_TRACTOR = 8,
        // DMBIN_SMALLFMT The smaller format paper tray
        DMBIN_SMALLFMT = 9,
        // DMBIN_LARGEFMT The larger format paper tray
        DMBIN_LARGEFMT = 10,
        // DMBIN_LARGECAPACITY The high capacity tray
        DMBIN_LARGECAPACITY = 11,
        // DMBIN_CASSETTE The paper cassete source
        DMBIN_CASSETTE = 14,
        // DMBIN_FORMSOURCE The preprinted from feed
        DMBIN_FORMSOURCE = 15,
        // DMBIN_USER Paper source is device specific
        DMBIN_USER = 256
    }
    #endregion


    #region SpoolerWin32ErrorCodes
    public enum SpoolerWin32ErrorCodes
    {
        ERROR_ACCESS_DENIED = 5
    }
    #endregion

    #region PrinterDriverOperatingSystemVersion
    public enum PrinterDriverOperatingSystemVersion
    {
        Driver_Win9x = 0,
        Driver_NT351 = 1,
        Driver_NT4 = 2,
        Driver_2000_XP = 3
    }
    #endregion

    #region DocumentPropertiesModes
    [Flags()]
    public enum DocumentPropertiesModes
    {
        DM_GETSIZE = 0,
        DM_COPY = 2,
        DM_PROMPT = 4,
        DM_MODIFY = 8
    }
    #endregion

    #region PrintDeviceCapabilitiesIndexes
    public enum PrintDeviceCapabilitiesIndexes
    {
        // DC_FIELDS Which fields of the device mode are used
        DC_FIELDS = 1,
        // DC_PAPERS Which Printer Paper Sizes the device supports
        DC_PAPERS = 2,
        // DC_PAPERSIZE The dimensions of the paper in 10ths of a millimeter
        DC_PAPERSIZE = 3,
        // DC_MINEXTENT The minimum paper width and height the printer can support
        DC_MINEXTENT = 4,
        // DC_MAXEXTENT The maximum paper width and height the printer can support
        DC_MAXEXTENT = 5,
        // DC_BINS The standard paper bins supported by this printer
        DC_BINS = 6,
        // DC_DUPLEX Whether the printer supports duplex printing
        DC_DUPLEX = 7,
        // DC_SIZE
        DC_SIZE = 8,
        // DC_EXTRA
        DC_EXTRA = 9,
        // DC_VERSION
        DC_VERSION = 10,
        // DC_DRIVER
        DC_DRIVER = 11,
        // DC_BINNAMES
        DC_BINNAMES = 12,
        // DC_ENUMRESOLUTIONS
        DC_ENUMRESOLUTIONS = 13,
        // DC_FILEDEPENDENCIES
        DC_FILEDEPENDENCIES = 14,
        // DC_TRUETYPE
        DC_TRUETYPE = 15,
        // DC_PAPERNAMES
        DC_PAPERNAMES = 16,
        // DC_ORIENTATION
        DC_ORIENTATION = 17,
        // DC_COPIES
        DC_COPIES = 18,
        // DC_BINADJUST
        DC_BINADJUST = 19,
        // DC_EMF_COMPLIANT
        DC_EMF_COMPLIANT = 20,
        // DC_DATATYPE_PRODUCED
        DC_DATATYPE_PRODUCED = 21,
        // DC_COLLATE - Returns non zero if the device supports collation
        DC_COLLATE = 22,
        // DC_MANUFACTURER
        DC_MANUFACTURER = 23,
        // DC_MODEL
        DC_MODEL = 24,
        // DC_PERSONALITY
        DC_PERSONALITY = 25,
        // DC_PRINTRATE
        DC_PRINTRATE = 26,
        // DC_PRINTRATEUNIT
        DC_PRINTRATEUNIT = 27,
        // DC_PRINTERMEM
        DC_PRINTERMEM = 28,
        // DC_MEDIAREADY
        DC_MEDIAREADY = 29,
        // DC_STAPLE - Returns non zero if the device supports stapling
        DC_STAPLE = 30,
        // DC_PRINTRATEPPM
        DC_PRINTRATEPPM = 31,
        // DC_COLORDEVICE
        DC_COLORDEVICE = 32,
        // DC_NUP
        DC_NUP = 33
    }
    #endregion

    #region PortTypes
    [Flags()]
    public enum PortTypes
    {
        PORT_TYPE_WRITE = 0x1,
        PORT_TYPE_READ = 0x2,
        PORT_TYPE_REDIRECTED = 0x4,
        PORT_TYPE_NET_ATTACHED = 0x8
    }
    #endregion

    #region PortStatuses
    public enum PortStatuses
    {
        PORT_STATUS_OK = 0,
        PORT_STATUS_OFFLINE = 1,
        PORT_STATUS_PAPER_JAM = 2,
        PORT_STATUS_PAPER_OUT = 3,
        PORT_STATUS_OUTPUT_BIN_FULL = 4,
        PORT_STATUS_PAPER_PROBLEM = 5,
        PORT_STATUS_NO_TONER = 6,
        PORT_STATUS_DOOR_OPEN = 7,
        PORT_STATUS_USER_INTERVENTION = 8,
        PORT_STATUS_OUT_OF_MEMORY = 9,
        PORT_STATUS_TONER_LOW = 10,
        PORT_STATUS_WARMING_UP = 11,
        PORT_STATUS_POWER_SAVE = 12
    }
    #endregion

    #region PortStatusSeverity
    public enum PortStatusSeverity
    {
        PORT_STATUS_TYPE_ERROR = 1,
        PORT_STATUS_TYPE_WARNING = 2,
        PORT_STATUS_TYPE_INFO = 3
    }
    #endregion

    #region FormTypeFlags
    public enum FormTypeFlags
    {
        FORM_USER = 0x0,
        FORM_BUILTIN = 0x1,
        FORM_PRINTER = 0x2
    }
    #endregion

    #region SpoolerRegisterKeys
    // \\ --[SpoolerRegisterKeys]-------------------------------
    // \\ Predefined registry key strings used by GetPrinterData 
    // \\ and SetPrinterData API functions
    // \\ ------------------------------------------------------
    public class SpoolerRegisterKeys
    {

        public enum SpoolerRegisterKeyIndexes
        {
            SPLREG_DEFAULT_SPOOL_DIRECTORY = 1, // "DefaultSpoolDirectory"
            SPLREG_PORT_THREAD_PRIORITY_DEFAULT = 2,      // "PortThreadPriorityDefault"
            SPLREG_PORT_THREAD_PRIORITY = 3,              // "PortThreadPriority"
            SPLREG_SCHEDULER_THREAD_PRIORITY_DEFAULT = 4, // "SchedulerThreadPriorityDefault"
            SPLREG_SCHEDULER_THREAD_PRIORITY = 5,         // "SchedulerThreadPriority"
            SPLREG_BEEP_ENABLED = 6, // "BeepEnabled"
            SPLREG_NET_POPUP = 7,        // "NetPopup"
            SPLREG_RETRY_POPUP = 8, // "RetryPopup"
            SPLREG_NET_POPUP_TO_COMPUTER = 9, // "NetPopupToComputer"
            SPLREG_EVENT_LOG = 10, // "EventLog"
            SPLREG_MAJOR_VERSION = 11, // "MajorVersion"
            SPLREG_MINOR_VERSION = 12, // "MinorVersion"
            SPLREG_ARCHITECTURE = 13, // "Architecture"
            SPLREG_OS_VERSION = 14, // "OSVersion"
            SPLREG_OS_VERSIONEX = 15, // "OSVersionEx"
            SPLREG_DS_PRESENT = 16, // "DsPresent"
            SPLREG_DS_PRESENT_FOR_USER = 17, // "DsPresentForUser"
            SPLREG_REMOTE_FAX = 18,          // "RemoteFax"
            SPLREG_RESTART_JOB_ON_POOL_ERROR = 19, // "RestartJobOnPoolError"
            SPLREG_RESTART_JOB_ON_POOL_ENABLED = 20, // "RestartJobOnPoolEnabled"
            SPLREG_DNS_MACHINE_NAME = 21  // "DNSMachineName"
        }

        public static string PredefinedKeyName(SpoolerRegisterKeyIndexes KeyIndex)
        {
            switch (KeyIndex)
            {
                case SpoolerRegisterKeyIndexes.SPLREG_DEFAULT_SPOOL_DIRECTORY:
                    {
                        return "DefaultSpoolDirectory";
                    }
                case SpoolerRegisterKeyIndexes.SPLREG_PORT_THREAD_PRIORITY_DEFAULT:
                    {
                        return "PortThreadPriorityDefault";
                    }
                case SpoolerRegisterKeyIndexes.SPLREG_PORT_THREAD_PRIORITY:
                    {
                        return "PortThreadPriority";
                    }
                case SpoolerRegisterKeyIndexes.SPLREG_SCHEDULER_THREAD_PRIORITY_DEFAULT:
                    {
                        return "SchedulerThreadPriorityDefault";
                    }
                case SpoolerRegisterKeyIndexes.SPLREG_SCHEDULER_THREAD_PRIORITY:
                    {
                        return "SchedulerThreadPriority";
                    }
                case SpoolerRegisterKeyIndexes.SPLREG_BEEP_ENABLED:
                    {
                        return "BeepEnabled";
                    }
                case SpoolerRegisterKeyIndexes.SPLREG_NET_POPUP:
                    {
                        return "NetPopup";
                    }
                case SpoolerRegisterKeyIndexes.SPLREG_RETRY_POPUP:
                    {
                        return "RetryPopup";
                    }
                case SpoolerRegisterKeyIndexes.SPLREG_NET_POPUP_TO_COMPUTER:
                    {
                        return "NetPopupToComputer";
                    }
                case SpoolerRegisterKeyIndexes.SPLREG_EVENT_LOG:
                    {
                        return "EventLog";
                    }
                case SpoolerRegisterKeyIndexes.SPLREG_MAJOR_VERSION:
                    {
                        return "MajorVersion";
                    }
                case SpoolerRegisterKeyIndexes.SPLREG_MINOR_VERSION:
                    {
                        return "MinorVersion";
                    }
                case SpoolerRegisterKeyIndexes.SPLREG_ARCHITECTURE:
                    {
                        return "Architecture";
                    }
                case SpoolerRegisterKeyIndexes.SPLREG_OS_VERSION:
                    {
                        return "OSVersion";
                    }
                case SpoolerRegisterKeyIndexes.SPLREG_OS_VERSIONEX:
                    {
                        return "OSVersionEx";
                    }
                case SpoolerRegisterKeyIndexes.SPLREG_DS_PRESENT:
                    {
                        return "DsPresent";
                    }
                case SpoolerRegisterKeyIndexes.SPLREG_DS_PRESENT_FOR_USER:
                    {
                        return "DsPresentForUser";
                    }
                case SpoolerRegisterKeyIndexes.SPLREG_REMOTE_FAX:
                    {
                        return "RemoteFax";
                    }
                case SpoolerRegisterKeyIndexes.SPLREG_RESTART_JOB_ON_POOL_ERROR:
                    {
                        return "RestartJobOnPoolError";
                    }
                case SpoolerRegisterKeyIndexes.SPLREG_RESTART_JOB_ON_POOL_ENABLED:
                    {
                        return "RestartJobOnPoolEnabled";
                    }
                case SpoolerRegisterKeyIndexes.SPLREG_DNS_MACHINE_NAME:
                    {
                        return "DNSMachineName";
                    }

                default:
                    {
                        return "";
                    }
            }
        }

    }
    #endregion

}