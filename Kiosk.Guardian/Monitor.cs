using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kiosk.Guardian
{
    public class Monitor
    {
        bool printerAlertSended = false;
        Postman postman = null;
        bool _firstRun = true;
        bool _isTurningOff = false;
        Timer _timer;
        Timer _timerPrinter;
        int _seconds;
        bool _IsRunning;
        bool _IsPaused;
        private KioskProperties _kioskProperties;
        public delegate void OnTickEventHandler(int second, int countdown);
        public event OnTickEventHandler OnTick;
        public delegate void OnPrintCheckEventHandler(string description, bool causesError);
        public event OnPrintCheckEventHandler OnPrintCheck;
        public delegate void OnStopEventHandler(int origin);
        public event OnStopEventHandler OnStop;
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



            if (_kioskProperties.CheckOnStartup)
            {
                PrinterCheckStart();
                TestKiosk();
            }
            else
            {
                PrinterCheckStart();
            }
        }

        private void _timerPrinter_Tick(object sender, EventArgs e)
        {
            try
            {
                PrinterStatus(_kioskProperties.PrinterName);
            }
            catch (Exception)
            {

            }
        }

        PrintQueue GetPrintQueue(string printerName)
        {
            PrintServer printServer = new PrintServer("\\\\" + Environment.MachineName, PrintSystemDesiredAccess.AdministrateServer);
            PrintQueueCollection printQueues = printServer.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local,
                    EnumeratedPrintQueueTypes.Connections });

            foreach (PrintQueue printQueue in printQueues)
            {
                printServer.Refresh();
                printQueue.Refresh();
                if (printQueue.Name.ToLower().Contains(printerName.ToLower()))
                {
                    return printQueue;
                }
            }

            return null;
        }

        void PrinterStatus(string printerName)
        {
            if (_kioskProperties.CheckPrinter)
            {
                PrintQueue printQueue = GetPrintQueue(printerName);

                string statusDescription = "AGUARDANDO INFORMAÇÕES DA IMPRESSORA";
                bool causesError = false;

                if (printQueue != null)
                {
                    PrintQueueStatus status = printQueue.QueueStatus;

                    if (printQueue.IsDoorOpened)
                    {
                        statusDescription = ("A PORTA DA IMPRESSORA ESTÁ ABERTA");
                        causesError = true;
                    }

                    if (printQueue.IsNotAvailable)
                    {
                        statusDescription = ("A IMPRESSORA ESTÁ INDISPONÍVEL");
                        causesError = true;
                    }

                    if (printQueue.IsOutOfPaper)
                    {
                        statusDescription = ("A IMPRESSORA ESTÁ SEM PAPEL");
                        causesError = true;
                    }

                    if (printQueue.IsPaperJammed)
                    {
                        statusDescription = "A IMPRESSORA ESTÁ COM PAPEL AGARRADO";
                        causesError = true;
                    }

                    if (printQueue.IsPaused)
                    {
                        statusDescription = "A IMPRESSORA ESTÁ PAUSADA";
                        causesError = true;
                    }

                    if (printQueue.IsPendingDeletion)
                    {
                        statusDescription = "A IMPRESSORA ESTÁ AGUARDANDO UM PROCESSO DE EXCLUSÃO NA FILA";
                        causesError = true;
                    }

                    if (printQueue.IsPrinting)
                    {
                        statusDescription = "A IMPRESSORA ESTÁ IMPRIMINDO";
                        printerAlertSended = false;
                    }

                    if (printQueue.IsTonerLow)
                    {
                        statusDescription = "A IMPRESSORA ESTÁ COM POUCO TONER";
                    }

                    if (printQueue.IsWaiting)
                    {
                        statusDescription = "A IMPRESSORA ESTÁ AGUARDANDO";
                        printerAlertSended = false;
                    }

                    if (printQueue.IsWarmingUp)
                    {
                        statusDescription = "A IMPRESSORA ESTÁ AQUECENDO";
                        printerAlertSended = false;
                    }

                    if (printQueue.IsInError)
                    {
                        statusDescription = "A IMPRESSORA ESTÁ EM ESTADO DE ERRO";
                        causesError = true;
                    }

                    if (printQueue.HasPaperProblem)
                    {
                        statusDescription = "ESTÁ COM PROBLEMA NO PAPEL";
                        causesError = true;
                    }

                    if (status == PrintQueueStatus.None)
                    {
                        statusDescription = "IMPRESSORA DISPONÍVEL";
                        printerAlertSended = false;
                    }

                    statusDescription += "(" + status.ToString().ToUpper() + ")";
                }
                else
                {
                    statusDescription = "IMPRESSORA NÃO DETECTADA";
                }

                if (OnPrintCheck != null)
                {
                    if (causesError && !printerAlertSended)
                    {
                        SendAlert("ERRO IMPRESSORA", statusDescription);
                        printerAlertSended = true;
                        _IsPaused = true;
                        KillKiosk();

                        MaintenanceUtils.PutOnMaintenance();
                    }

                    if (!causesError && !printerAlertSended && _IsPaused)
                    {
                        _IsPaused = false;
                        TestKiosk();
                        MaintenanceUtils.CloseMaintenance();

                        if (_kioskProperties.NotifyTelegram)
                        {
                            Postman.SendToTelegram(Environment.MachineName.ToUpper() + " -> ERRO DE IMPRESSORA RESOLVIDO!",
                            _kioskProperties.ChatID, _kioskProperties.TelegramToken);
                        }
                    }

                    OnPrintCheck(statusDescription, causesError);
                }
            }
        }

        void KillJobs(PrintQueue printQueue)
        {
            try
            {
                foreach (PrintSystemJobInfo job in printQueue.GetPrintJobInfoCollection())
                {
                    job.Cancel();
                }
            }
            catch (Exception)
            {

            }
        }

        void SendAlert(string subject, string message)
        {
            try
            {
                if (!_firstRun && postman != null)
                {
                    postman.Send(subject, message);
                }

                if (!_firstRun && _kioskProperties.NotifyTelegram)
                {
                    Postman.SendToTelegram(Environment.MachineName.ToUpper() + " - " + subject + " -> " + message.ToUpperInvariant(),
                        _kioskProperties.ChatID, _kioskProperties.TelegramToken);
                }
            }
            catch (Exception)
            {

            }
        }

        public void KillKiosk()
        {
            Process[] processlist = Process.GetProcesses();

            foreach (Process process in processlist)
            {
                if (process.ProcessName.ToLower() == _kioskProperties.ProcessName.ToLower())
                {
                    process.Kill();
                }
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
            if (_IsRunning)
            {
                KioskStatus kioskStatus = KioskIsRunning();

                if (kioskStatus == KioskStatus.NotResponding)
                {
                    MaintenanceUtils.MainForm.WindowState = FormWindowState.Minimized;
                    SendAlert("EVENTO ", "O APLICATIVO ESTAVA TRAVADO E FOI REINICIADO");
                    Process.Start(_kioskProperties.KioskPath);
                }

                if (kioskStatus == KioskStatus.Off)
                {
                    MaintenanceUtils.MainForm.WindowState = FormWindowState.Minimized;
                    SendAlert("EVENTO", "O APLICATIVO FECHOU E FOI ABERTO NOVAMENTE");
                    Process.Start(_kioskProperties.KioskPath);
                }

                _firstRun = false;
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!_IsPaused)
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
            }
            catch (Exception)
            {

            }
        }

        void PrinterCheckStart()
        {
            if (_kioskProperties.CheckPrinter)
            {
                _timerPrinter = new Timer();
                _timerPrinter.Interval = (5 * 1000);
                _timerPrinter.Tick += _timerPrinter_Tick;
                _timerPrinter.Start();
            }
        }
        void PrinterCheckStop()
        {
            if (_timerPrinter != null && !printerAlertSended)
            {
                _timerPrinter.Stop();
            }
        }
        public void Stop()
        {
            _timer.Stop();
            _IsRunning = false;
            _kioskProperties.Running = false;
            _firstRun = true;
            postman = null;
            PrinterCheckStop();
        }

    }
}
