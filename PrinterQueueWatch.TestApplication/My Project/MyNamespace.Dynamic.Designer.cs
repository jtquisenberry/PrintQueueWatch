using System;
using System.ComponentModel;
using System.Diagnostics;

namespace PrinterQueueWatch.TestApplication.My
{
    internal static partial class MyProject
    {
        internal partial class MyForms
        {

            [EditorBrowsable(EditorBrowsableState.Never)]
            public Form_PrintQueueWatchTest m_Form_PrintQueueWatchTest;

            public Form_PrintQueueWatchTest Form_PrintQueueWatchTest
            {
                [DebuggerHidden]
                get
                {
                    m_Form_PrintQueueWatchTest = Create__Instance__(m_Form_PrintQueueWatchTest);
                    return m_Form_PrintQueueWatchTest;
                }
                [DebuggerHidden]
                set
                {
                    if (ReferenceEquals(value, m_Form_PrintQueueWatchTest))
                        return;
                    if (value is not null)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_Form_PrintQueueWatchTest);
                }
            }

        }


    }
}