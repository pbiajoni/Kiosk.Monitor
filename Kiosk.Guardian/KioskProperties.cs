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
        private string _proccessName;
        private string _pathToServer;

        public KioskProperties()
        {
            ini = new IniFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                , "Kiosk", "settings.ini"));
        }

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
                    return @"\\srv-multiclubes\multiclubessistemas$\Kiosk\MultiClubes.Kiosk.UI.application";
                }

                return _pathToServer;
            }

            set
            {
                _pathToServer = value;
            }
        }

        [Category("Verificação")]
        [DisplayName("Intervalo")]
        [Description("Intervalo em que será verificado se o Kiosk está em operação")]
        public int Interval { get; set; }
        [Category("Ferramentas")]
        [DisplayName("Desligar ATM")]
        [Description("Define que o ATM será desligado no horário agendado")]
        public bool TurnOff { get; set; }

        [Category("Ferramentas")]
        [DisplayName("Hora")]
        [Description("Define a hora de desligamento")]
        public int Hour { get; set; }
        [Category("Ferramentas")]
        [DisplayName("Minutos")]
        [Description("Define os minutos do desligamento")]
        public int Minute { get; set; }

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

            ini.IniWriteValue("MultiClubes", "KioskPath", KioskPath);
            ini.IniWriteValue("MultiClubes", "ProccessName", ProcessName);

            ini.IniWriteValue("Validation", "Interval", Interval.ToString());

            ini.IniWriteValue("Tools", "TurnOff", TurnOff.ToString());
            ini.IniWriteValue("Tools", "Hour", Hour.ToString());
            ini.IniWriteValue("Tools", "Minute", Minute.ToString());
        }

        public void Get()
        {
            if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                , "Kiosk", "settings.ini"))){
                KioskPath = ini.IniReadValue("MultiClubes", "KioskPath");
                ProcessName = ini.IniReadValue("MultiClubes", "ProccessName");

                Interval = Convert.ToInt32(ini.IniReadValue("Validation", "Interval"));
                TurnOff = Convert.ToBoolean(ini.IniReadValue("Tools", "TurnOff"));
                Hour = Convert.ToInt32(ini.IniReadValue("Tools", "Hour"));
                Minute = Convert.ToInt32(ini.IniReadValue("Tools", "Minute"));
            }
        }
    }
}
