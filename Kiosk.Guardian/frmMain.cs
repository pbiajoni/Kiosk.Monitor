using iniSettings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
            this.FormClosing += FrmMain_FormClosing;
            monitor.OnTick += Monitor_OnTick;
            monitor.OnPrintCheck += Monitor_OnPrintCheck;
        }

        private void Monitor_OnPrintCheck(string description, bool causesError)
        {
            lblPrinterStatus.Text = " - " + description;

            if (causesError)
            {
                lblCountDown.Text = "EM MANUTENÇÃO";
            }
        }

        private void Monitor_OnTick(int second, int countdown)
        {
            lblCountDown.Text = ("Verificando em: " + countdown + " segundo(s)").ToUpper();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            lblCountDown.Text = "";
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            MaintenanceUtils.MainForm = this;
            MaintenanceUtils.MainPropertyGrid = propertyGrid1;

            lblCountDown.Text = "Parado";
            lblPrinterStatus.Text = "";
            kioskProperties = new KioskProperties();
            propertyGrid1.SelectedObject = kioskProperties;
            kioskProperties.Get();


            //if (kioskProperties.RemoteCfg)
            //{
            //    btnSalvar.Enabled = false;
            //}

            if (kioskProperties.Running)
            {
                btnStart.PerformClick();
                this.WindowState = FormWindowState.Minimized;
            }
        }

        void HideMe()
        {
            //notifyIcon1.ShowBalloonTip(5000, "Atenção!", "Estou aqui fazendo o monitoramento", ToolTipIcon.Info);
            this.ShowInTaskbar = false;
            this.Hide();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (!monitor.IsRunning)
                {
                    kioskProperties.Save();
                    monitor.Run(kioskProperties);
                    kioskProperties.Save();
                    btnStart.Text = "Parar";
                    propertyGrid1.Enabled = false;
                    btnSalvar.Enabled = false;
                    btnStartAndMinimize.Enabled = false;
                }
                else
                {
                    monitor.Stop();
                    btnStart.Text = "Iniciar";
                    propertyGrid1.Enabled = true;
                    btnSalvar.Enabled = true;
                    kioskProperties.Save();
                    Show();
                    lblCountDown.Text = "Parado";
                    btnStartAndMinimize.Enabled = true;
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

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStart.PerformClick();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
            this.Show();
        }

        private void btnStartAndMinimize_Click(object sender, EventArgs e)
        {
            btnStart.PerformClick();
            if (kioskProperties.Running)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void btnMaintenance_Click(object sender, EventArgs e)
        {
            MaintenanceUtils.PutOnMaintenance();
        }

        private void btnOpenQueuePrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (kioskProperties.Running)
                {
                    MessageBox.Show("Você deve primeiro parar o guadian", "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    Process.Start("rundll32.exe", "printui.dll, PrintUIEntry /o /n \"" + kioskProperties.PrinterName + "\"");
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message, "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPrintTestPage_Click(object sender, EventArgs e)
        {
            try
            {
                if (kioskProperties.Running)
                {
                    MessageBox.Show("Você deve primeiro parar o guadian", "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    Process.Start("rundll32.exe", "printui.dll, PrintUIEntry /k /n \"" + kioskProperties.PrinterName + "\"");
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message, "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTestTelegram_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(kioskProperties.TelegramToken))
                {
                    throw new Exception("O Token não deve estar vazio");
                }

                if (string.IsNullOrEmpty(kioskProperties.ChatID))
                {
                    throw new Exception("O chat não deve estar vazio");
                }

                Postman.SendToTelegram("MENSAGEM DE TESTE TELEGRAM - "
                    + Environment.MachineName.ToUpper(), kioskProperties.ChatID, kioskProperties.TelegramToken);
                MessageBox.Show("Mensagem enviada!", "Ok!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message, "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
