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
        KioskProperties kioskProperties;
        Monitor monitor = new Monitor();
        public frmMain()
        {
            InitializeComponent();
            ini = new IniFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                , "Kiosk", "settings.ini"));
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
             kioskProperties = new KioskProperties();
            propertyGrid1.SelectedObject = kioskProperties;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (!monitor.IsRunning)
                {
                    monitor.Run(kioskProperties);
                    btnStart.Text = "Parar";
                    propertyGrid1.Enabled = false;
                }
                else
                {
                    monitor.Stop();
                    btnStart.Text = "Iniciar";
                    propertyGrid1.Enabled = true;
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message, "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }

        }
    }
}
