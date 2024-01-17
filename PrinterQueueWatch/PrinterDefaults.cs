using System.Runtime.InteropServices;
using PrinterQueueWatch.SpoolerApiConstantEnumerations;

namespace PrinterQueueWatch
{

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    class PrinterDefaults
    {

        #region Public interface

        public string DataType;
        public int lpDevMode;
        public PrinterAccessRights DesiredAccess;

        #endregion

    }
}