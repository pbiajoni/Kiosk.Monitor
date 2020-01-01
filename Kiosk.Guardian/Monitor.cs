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
        bool _IsRunning;
        private KioskProperties _kioskProperties;
        public Monitor()
        {

        }

        public bool IsRunning
        {
            get
            {
                return _IsRunning;
            }
        }
        public void Run(KioskProperties properties)
        {
            _kioskProperties = properties;

            if(_kioskProperties.Interval < 60)
            {
                throw new Exception("O valor de intervalo deve ser maior que 60 segundos");
            }

            _timer = new Timer();
            _timer.Interval = _kioskProperties.Interval;
            _timer.Tick += _timer_Tick;
            _timer.Start();
            _IsRunning = true;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            
        }

        public void Stop()
        {
            _timer.Stop();
            _IsRunning = false;
        }

    }
}
