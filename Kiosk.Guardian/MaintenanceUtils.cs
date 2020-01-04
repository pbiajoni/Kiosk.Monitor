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

        public static void CloseMaintenance()
        {
            if(_maintenance != null)
            {
                _maintenance.Close();
                _maintenance = null;
            }
        }
    }
}
