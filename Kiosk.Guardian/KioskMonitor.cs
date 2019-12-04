using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiosk.Guardian
{
    public class KioskMonitor
    {
        private string _proccessName;
        private string _pathToServer;

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
    }
}
