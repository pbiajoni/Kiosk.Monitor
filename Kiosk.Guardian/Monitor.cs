using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kiosk.Guardian
{
    public class Monitor
    {
        Timer _timer;
        private KioskMonitor _kioskMonitor;
        public Monitor(KioskMonitor kioskMonitor)
        {
            _kioskMonitor = kioskMonitor;
        }

        public void Run()
        {
            if(_kioskMonitor.Interval < 60)
            {
                throw new Exception("O valor de intervalo deve ser maior que 60 segundos");
            }

            _timer.Interval = _kioskMonitor.Interval;
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            _timer.Stop();
        }

    }
}
