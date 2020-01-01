using iniSettings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kiosk.Guardian
{
    public partial class frmMain : Form
    {
        IniFile ini;
        KioskMonitor kioskMonitor;
        public frmMain()
        {
            InitializeComponent();
            ini = new IniFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                , "Kiosk", "settings.ini"));
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
             kioskMonitor = new KioskMonitor();
            propertyGrid1.SelectedObject = kioskMonitor;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Monitor monitor = new Monitor(kioskMonitor);
            monitor.Run();
        }
    }
}
