using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kiosk.Guardian
{
    public class KioskMonitor
    {
        private string _proccessName;
        private string _pathToServer;
        private ComboBox _hour;

        public KioskMonitor()
        {
            _hour = new ComboBox();
            _hour.Items.Add("01");
            _hour.Items.Add(2);
            _hour.Items.Add(3);
            _hour.Items.Add(4);
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
        public string Path
        {
            get
            {
                if (string.IsNullOrEmpty(_pathToServer))
                {
                    return @"\\servidor\multiclubessistemas$\Kiosk\MultiClubes.Kiosk.UI.application";
                }

                return _pathToServer;
            }

            set
            {
                _pathToServer = value;
            }
        }

        [Category("Verificãção")]
        [DisplayName("Intervalo")]
        [Description("Intervalo em que será verificado se o Kiosk está em operação")]
        public int Interval { get; set; }
        [Category("Ferramentas")]
        [DisplayName("Desligar ATM")]
        [Description("Define que o ATM será desligado no horário agendado")]
        public bool TurnOff { get; set; }
        public ComboBox Hour
        {
            get
            {
                return _hour;
            }

            set
            {
                _hour = value;
            }
        }
    }
}
