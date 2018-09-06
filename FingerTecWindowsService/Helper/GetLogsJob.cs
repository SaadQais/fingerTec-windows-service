using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FingerTecWindowsService.Helper
{
    
    public class GetLogsJob : IJob
    {
        LogWriter lw = new LogWriter();
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var t = new Thread(MyThreadStartMethod);
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }
            catch (System.Exception ex)
            {
                lw.LogWrite(ex.Message);
            }
        }

        private void MyThreadStartMethod()
        {
            new Operations().Run();
        }
    }
}
