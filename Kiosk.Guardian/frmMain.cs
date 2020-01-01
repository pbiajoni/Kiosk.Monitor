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
        KioskProperties kioskProperties;
        Monitor monitor = new Monitor();
        public frmMain()
        {
            InitializeComponent();
            
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
             kioskProperties = new KioskProperties();
            propertyGrid1.SelectedObject = kioskProperties;
            kioskProperties.Get();
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
                    btnSalvar.Enabled = false;
                }
                else
                {
                    monitor.Stop();
                    btnStart.Text = "Iniciar";
                    propertyGrid1.Enabled = true;
                    btnSalvar.Enabled = true;
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message, "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }

        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                kioskProperties.Save();
                MessageBox.Show("Salvo com sucesso!", "Ok", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message, "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
