using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Kiosk.Guardian
{
    public class PrinterUtils
    {
        public void GetPrinterProperties()
        {
            int statComplete = 0;
            string[] printerStatus = { "Other", "Unknown", "Idle", "Printing", "WarmUp", "Stopped Printing", "Offline" };
            string[] printerState = {"Paused","Error","Pending Deletion","Paper Jam","Paper Out","Manual Feed","Paper Problem", "Offline","IO Active","Busy","Printing",
            "Output Bin Full","Not Available","Waiting", "Processing","Initialization","Warming Up","Toner Low","No Toner","Page Punt", "User Intervention Required",
            "Out of Memory","Door Open","Server_Unknown","Power Save"};

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");

            //now loop thorugh all the objects in the searcher  
            foreach (ManagementObject obj in searcher.Get())
            {
                if ((bool)obj["Default"])
                {
                    //now loop through all the properties  
                    foreach (PropertyData data in obj.Properties)
                    {
                        switch (data.Name.ToLower())
                        {
                            case "name":
                                //memoPrintDetail.AppendText("Default Printer Name : " + data.Value + "\n"); //This Code is Working
                                statComplete += 1;
                                break;
                            case "printerstate":
                                //memoPrintDetail.AppendText("Printer State : " + printerState[Convert.ToInt32(data.Value)] + "\n"); //Always give "Paused" state
                                statComplete += 1;
                                break;
                            case "printerstatus":
                                //memoPrintDetail.AppendText("Printer Status : " + printerStatus[Convert.ToInt32(data.Value)] + "\n");//Always give "Printing" status
                                statComplete += 1;
                                break;
                        }
                    }
                }
                if (statComplete == 3)
                    break;
            }
        }
    }
}
