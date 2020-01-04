using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kiosk.Guardian
{
    public class MaintenanceUtils
    {
        public static Form MainForm { get; set; }
        public static frmMaintenance _maintenance;
        public static void PutOnMaintenance()
        {
            if (_maintenance == null)
            {
                _maintenance = new frmMaintenance();
                _maintenance.ShowInTaskbar = false;
                _maintenance.Show();
            }
        }

        public static void CloseMaintenance(bool showMonitor = false)
        {
            if(_maintenance != null)
            {
                _maintenance.Close();
                _maintenance = null;

                if (showMonitor)
                {
                    MainForm.WindowState = FormWindowState.Normal;
                }
            }
        }
    }
}
