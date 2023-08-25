using System.Diagnostics;

namespace MsGraphEmailsFramework
{
    internal class TestMailReader //: MailReader
    {
        public void Execute()
        {
            Trace.TraceInformation("-------------------------------------------------------------------------------------------------------------");
            
            new MsGraphMailReader().Execute().GetAwaiter().GetResult();

            Trace.TraceInformation("-------------------------------------------------------------------------------------------------------------");
        }
    }
}
