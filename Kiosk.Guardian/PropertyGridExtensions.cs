using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kiosk.Guardian
{
    public static class PropertyGridExtensions
    {
        public static GridItemCollection GetAllGridEntries(this PropertyGrid grid)
        {
            if (grid == null)
                throw new ArgumentNullException("grid");

            object view = grid.GetType().GetField("gridView", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(grid);
            return (GridItemCollection)view.GetType().InvokeMember("GetAllGridEntries", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, view, null);
        }

        public static void SetAllReadyOnly(this PropertyGrid grid)
        {
            try
            {
                foreach (GridItemCollection gridItem in MaintenanceUtils.MainPropertyGrid.GetAllGridEntries())
                {
                    TypeDescriptor.AddAttributes(gridItem, new Attribute[] { new ReadOnlyAttribute(true) });
                }
            }
            catch (Exception)
            {

            }

        }
    }
}
