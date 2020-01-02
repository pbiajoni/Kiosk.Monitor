using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kiosk.Guardian
{
    public class Monitor
    {
        bool _isTurningOff = false;
        Timer _timer;
        int _seconds;
        bool _IsRunning;
        private KioskProperties _kioskProperties;
        public delegate void OnTickEventHandler(int second, int countdown);
        public event OnTickEventHandler OnTick;
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

            if (_kioskProperties.Interval < 30)
            {
                throw new Exception("O valor de intervalo deve ser igual ou maior que 30 segundos");
            }

            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Tick += _timer_Tick;
            _timer.Start();
            _IsRunning = true;
            _kioskProperties.Running = true;
            _seconds = 0;

            TestKiosk();
        }

        bool KioskIsRunning()
        {
            Process[] processlist = Process.GetProcesses();

            foreach (Process process in processlist)
            {
                if (process.ProcessName.ToLower() == _kioskProperties.ProcessName.ToLower())
                {
                    if (process.Responding)
                    {
                        return true;
                    }
                    else
                    {
                        process.Kill();
                        return false;
                    }
                }
            }

            return false;
        }


        void TestKiosk()
        {
            if (!KioskIsRunning())
            {
                Process.Start(_kioskProperties.KioskPath);
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            try
            {
                int hour = DateTime.Now.Hour;
                int minute = DateTime.Now.Minute;

                if (_kioskProperties.TurnOff && !_isTurningOff && (_kioskProperties.Hour == hour) && (_kioskProperties.Minute == minute))
                {
                    _isTurningOff = true;
                    Process.Start("shutdown.exe", "-s -t 00");
                }

                if (OnTick != null)
                {
                    OnTick(_seconds, (_kioskProperties.Interval - _seconds));
                }

                if (!_isTurningOff)
                {
                    if (_seconds == _kioskProperties.Interval)
                    {
                        _seconds = 0;
                        TestKiosk();
                    }
                    else
                    {
                        _seconds++;
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public void Stop()
        {
            _timer.Stop();
            _IsRunning = false;
            _kioskProperties.Running = false;
        }

    }
}
