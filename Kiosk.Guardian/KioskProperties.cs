using iniSettings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kiosk.Guardian
{
    public class KioskProperties
    {
        IniFile ini;
        IniFile iniRemote;
        private string _proccessName;
        private string _pathToServer;
        Timer timer = new Timer();

        public KioskProperties()
        {
            ini = new IniFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                , "Kiosk", "settings.ini"));            
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            
        }

        [Browsable(false)]
        public bool Running { get; set; }
        [Browsable(false)]
        public string Passwords { get; set; }

        [DefaultValueAttribute("MultiClubes.Kiosk.UI")]
        [Category("MultiClubes")]
        [DisplayName("Nome do Processo")]
        [Description("Nome do processo do Multiclubes Kiosk")]
        public string ProcessName
        {
            get
            {
                if (string.IsNullOrEmpty(_proccessName))
                {
                    return "MultiClubes.Kiosk.UI";
                }

                return _proccessName;
            }

            set
            {
                _proccessName = value;
            }
        }

        [Category("MultiClubes")]
        [DisplayName("Caminho do Kiosk")]
        [Description("Caminho do aplicativo Kiosk no servidor")]
        public string KioskPath
        {
            get
            {
                if (string.IsNullOrEmpty(_pathToServer))
                {
                    return @"\\srv-mc-rj\multiclubessistemas$\Kiosk\MultiClubes.Kiosk.UI.application";
                }

                return _pathToServer;
            }

            set
            {
                _pathToServer = value;
            }
        }
        [DefaultValueAttribute(30)]
        [Category("Verificação")]
        [DisplayName("Intervalo")]
        [Description("Intervalo em que será verificado se o Kiosk está em operação")]
        public int Interval { get; set; }

        [DefaultValueAttribute(false)]
        [Category("Verificação")]
        [DisplayName("Verificar ao iniciar")]
        [Description("Verifica o kiosk ao iniciar o guardian, caso não, ele verificará apenas na próxima tentativa")]
        public bool CheckOnStartup { get; set; }

        [Category("Ferramentas")]
        [DisplayName("Desligar ATM")]
        [Description("Define que o ATM será desligado no horário agendado")]
        public bool TurnOff { get; set; }

        [Category("Ferramentas")]
        [DisplayName("Hora para desligar")]
        [Description("Define a hora de desligamento")]
        public int Hour { get; set; }
        [Category("Ferramentas")]
        [DisplayName("Minutos para desligar")]
        [Description("Define os minutos do desligamento")]
        public int Minute { get; set; }

        [Category("Ferramentas")]
        [DisplayName("Monitorar impressora")]
        [Description("Define que o monitor irá verificar o status da impressora padrão")]
        public bool CheckPrinter { get; set; }

        [Category("Ferramentas")]
        [DisplayName("Nome da impressora")]
        [Description("Nome da impressora de cupom configurada no kiosk")]
        public string PrinterName { get; set; }

        //[Category("Ferramentas")]
        //[DisplayName("Fechar Processos")]
        //[Description("Fecha os processos que estão na lista ex:Processo01.exe,Processo02.exe")]
        //public string CloseProccess { get; set; }

        [Category("Log")]
        [DisplayName("Alertar por email")]
        [Description("Define se o guardian enviará alertas por email")]
        public bool SendAlerts { get; set; }

        [Category("Log")]
        [DisplayName("Servidor SMTP")]
        [Description("Endereço ip ou nome do host smtp")]
        public string Smtp { get; set; }

        [Category("Log")]
        [DefaultValueAttribute(587)]
        [DisplayName("Porta")]
        [Description("Porta do serviço smtp")]
        public int Port { get; set; }

        [Category("Log")]
        [DisplayName("Usuário")]
        [Description("Nome do usuário SMTP")]
        public string Username { get; set; }

        [Category("Log")]
        [PasswordPropertyText(true)]
        [DisplayName("Senha")]
        [Description("Senha SMTP")]
        public string Password { get; set; }

        [Category("Log")]
        [DisplayName("De")]
        [Description("Endereço de email que enviará os logs")]
        public string FromMail { get; set; }

        [Category("Log")]
        [DisplayName("Para")]
        [Description("Endereço de email que receberá os logs")]
        public string ToMail { get; set; }

        [Category("Telegram")]
        [DisplayName("Notificar via Telegram")]
        [Description("Se o guardian deve notificar via telegram")]
        public bool NotifyTelegram { get; set; }

        [Category("Telegram")]
        [DisplayName("ID do Chat")]
        [Description("ID do chat em que devem ser recebidas as notificações")]
        public string ChatID { get; set; }

        [Category("Telegram")]
        [DisplayName("Token")]
        [Description("Token de autorização do Bot")]
        public string TelegramToken { get; set; }

        //[Category("Configurações")]
        //[DisplayName("Buscar configurações remotamente")]
        //[Description("Busca as configurações no settings.ini em uma pasta remota")]
        //public bool RemoteCfg { get; set; }

        //[Category("Configurações")]
        //[DisplayName("Caminho das configurações")]
        //[Description("O caminho do arquivo settings.ini")]
        //public string SettingsPath { get; set; }

        void ValidateDirectory()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Kiosk");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        public void Save()
        {
            ValidateDirectory();

            if (Interval < 30)
            {
                throw new Exception("O valor de intervalo deve ser igual ou maior que 30 segundos");
            }

            if (CheckPrinter && string.IsNullOrEmpty(PrinterName))
            {
                throw new Exception("O nome da impressora de cupom deve ser especificado");
            }

            if (SendAlerts && string.IsNullOrEmpty(Smtp))
            {
                throw new Exception("O servidor smtp deve estar especificado");
            }

            if (SendAlerts && string.IsNullOrEmpty(Port.ToString()))
            {
                throw new Exception("A porta deve estar especificada");
            }

            if (SendAlerts && string.IsNullOrEmpty(Username))
            {
                throw new Exception("O usuário smtp deve estar especificado");
            }

            if (SendAlerts && string.IsNullOrEmpty(Password))
            {
                throw new Exception("A senha smtp deve estar especificada");
            }

            if (SendAlerts && string.IsNullOrEmpty(FromMail))
            {
                throw new Exception("O email de origem deve estar especificado");
            }

            if (SendAlerts && string.IsNullOrEmpty(ToMail))
            {
                throw new Exception("O email de destino de email deve estar especificado");
            }

            if (NotifyTelegram && string.IsNullOrEmpty(TelegramToken))
            {
                throw new Exception("O token do bot telegram deve ser especificado");
            }

            if (NotifyTelegram && string.IsNullOrEmpty(ChatID))
            {
                throw new Exception("A ID do chat telegram deve ser especificado");
            }

            ini.IniWriteValue("MultiClubes", "KioskPath", KioskPath);
            ini.IniWriteValue("MultiClubes", "ProccessName", ProcessName);

            ini.IniWriteValue("Validation", "Interval", Interval.ToString());
            ini.IniWriteValue("Validation", "CheckOnStartup", CheckOnStartup.ToString());

            ini.IniWriteValue("Tools", "TurnOff", TurnOff.ToString());
            ini.IniWriteValue("Tools", "Hour", Hour.ToString());
            ini.IniWriteValue("Tools", "Minute", Minute.ToString());
            ini.IniWriteValue("Tools", "CheckPrinter", CheckPrinter.ToString());
            ini.IniWriteValue("Tools", "PrinterName", PrinterName);

            ini.IniWriteValue("Guardian", "Running", Running.ToString());

            ini.IniWriteValue("Log", "SendAlerts", SendAlerts.ToString());
            ini.IniWriteValue("Log", "Smtp", Smtp);
            ini.IniWriteValue("Log", "Port", Port.ToString());
            ini.IniWriteValue("Log", "Username", Username);
            ini.IniWriteValue("Log", "Password", !string.IsNullOrEmpty(Password) ? MaintenanceUtils.Codificar(Password) : "");
            ini.IniWriteValue("Log", "FromMail", FromMail);
            ini.IniWriteValue("Log", "ToMail", ToMail);

            ini.IniWriteValue("Telegram", "ChatID", ChatID);
            ini.IniWriteValue("Telegram", "NotifyTelegram", NotifyTelegram.ToString());
            ini.IniWriteValue("Telegram", "Token", !string.IsNullOrEmpty(TelegramToken) ? MaintenanceUtils.Codificar(TelegramToken) : "");

        }

        public void Get()
        {
            bool RemoteCfg = false;
            string SettingsPath = "";

            if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                , "Kiosk", "settings.ini")))
            {
                KioskPath = ini.IniReadValue("MultiClubes", "KioskPath");
                ProcessName = ini.IniReadValue("MultiClubes", "ProccessName");

                Interval = Convert.ToInt32(ini.IniReadValue("Validation", "Interval"));
                try { CheckOnStartup = Convert.ToBoolean(ini.IniReadValue("Validation", "CheckOnStartup")); } catch (Exception) { CheckOnStartup = false; };

                TurnOff = Convert.ToBoolean(ini.IniReadValue("Tools", "TurnOff"));
                Hour = Convert.ToInt32(ini.IniReadValue("Tools", "Hour"));
                Minute = Convert.ToInt32(ini.IniReadValue("Tools", "Minute"));
                PrinterName = ini.IniReadValue("Tools", "PrinterName");
                try { CheckPrinter = Convert.ToBoolean(ini.IniReadValue("Tools", "CheckPrinter")); } catch (Exception) { CheckPrinter = false; };

                try { Running = Convert.ToBoolean(ini.IniReadValue("Guardian", "Running")); } catch (Exception) { Running = false; };
                try { SendAlerts = Convert.ToBoolean(ini.IniReadValue("Log", "SendAlerts")); } catch (Exception) { SendAlerts = false; }
                Smtp = ini.IniReadValue("Log", "Smtp");
                try { Port = Convert.ToInt32(ini.IniReadValue("Log", "Port")); } catch (Exception) { Port = 587; }
                Username = ini.IniReadValue("Log", "Username");
                Password = !string.IsNullOrEmpty(ini.IniReadValue("Log", "Password")) ? MaintenanceUtils.Decodificar(ini.IniReadValue("Log", "Password")) : "";
                FromMail = ini.IniReadValue("Log", "FromMail");
                ToMail = ini.IniReadValue("Log", "ToMail");

                try { NotifyTelegram = Convert.ToBoolean(ini.IniReadValue("Telegram", "NotifyTelegram")); } catch (Exception) { NotifyTelegram = false; }
                TelegramToken = !string.IsNullOrEmpty(ini.IniReadValue("Telegram", "Token")) ? MaintenanceUtils.Decodificar(ini.IniReadValue("Telegram", "Token")) : "";
                ChatID = ini.IniReadValue("Telegram", "ChatID");

                Passwords = ini.IniReadValue("Guardian", "Passwords");
                MaintenanceUtils.Passwords = Passwords;
            }
            else
            {
                Save();
            }
        }
    }
}
