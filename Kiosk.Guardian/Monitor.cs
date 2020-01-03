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
        Postman postman = null;
        bool _firstRun = true;
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

            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Tick += _timer_Tick;
            _timer.Start();
            _IsRunning = true;
            _kioskProperties.Running = true;
            _seconds = 0;

            if (_kioskProperties.SendAlerts)
            {
                postman = new Postman();
                postman.Server = _kioskProperties.Smtp;
                postman.Port = _kioskProperties.Port;
                postman.Username = _kioskProperties.Username;
                postman.Password = _kioskProperties.Password;
                postman.FromMail = _kioskProperties.FromMail;
                postman.ToMail = _kioskProperties.ToMail;
            }

            TestKiosk();
            _firstRun = false;
        }

        void SendAlert(string message)
        {
            try
            {
                if (!_firstRun && postman != null)
                {
                    postman.Send(message);
                }
            }
            catch (Exception)
            {

            }
        }

        KioskStatus KioskIsRunning()
        {
            Process[] processlist = Process.GetProcesses();

            foreach (Process process in processlist)
            {
                if (process.ProcessName.ToLower() == _kioskProperties.ProcessName.ToLower())
                {
                    if (process.Responding)
                    {
                        return KioskStatus.Alive;
                    }
                    else
                    {
                        process.Kill();
                        return KioskStatus.NotResponding;
                    }
                }
            }

            return KioskStatus.Off;
        }


        void TestKiosk()
        {
            KioskStatus kioskStatus = KioskIsRunning();

            if(kioskStatus == KioskStatus.NotResponding)
            {
                Process.Start(_kioskProperties.KioskPath);
                SendAlert("O APLICATIVO ESTAVA TRAVADO E FOI REINICIADO");
            }

            if(kioskStatus == KioskStatus.Off)
            {
                SendAlert("O APLICATIVO FECHOU E FOI ABERTO NOVAMENTE");
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
            _firstRun = true;
            postman = null;
        }

    }
}
