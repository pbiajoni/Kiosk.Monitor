using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kiosk.Guardian
{
    public class MaintenanceUtils
    {
        public static string Passwords { get; set; }
        public static Form MainForm { get; set; }
        public static frmMaintenance _maintenance;
        public static PropertyGrid MainPropertyGrid { get; set; }

        public static void SetReadyOnly()
        {
            TypeDescriptor.AddAttributes(MainPropertyGrid.SelectedObject, new Attribute[] { new ReadOnlyAttribute(true) });
        }
        public static void PutOnMaintenance(string message = "")
        {
            if (_maintenance == null)
            {
                _maintenance = new frmMaintenance();
                _maintenance.ShowInTaskbar = false;
                _maintenance.Passwords = Passwords;
                _maintenance.Message = message;
                Console.WriteLine("Entrando em modo manutenção");
                _maintenance.Show();
            }
        }

        public static void CloseMaintenance(bool showMonitor = false)
        {
            if (_maintenance != null)
            {
                _maintenance.Close();
                _maintenance = null;
                Console.WriteLine("Saindo do modo de manutenção");
                if (showMonitor)
                {
                    MainForm.WindowState = FormWindowState.Normal;
                }
            }
        }

        public static string Codificar(string entrada)
        {
            TripleDESCryptoServiceProvider tripledescryptoserviceprovider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider md5cryptoserviceprovider = new MD5CryptoServiceProvider();

            try
            {
                if (entrada.Trim() != "")
                {
                    Console.WriteLine(entrada);
                    string myKey = "39204391000100";  //Aqui vc inclui uma chave qualquer para servir de base para cifrar, que deve ser a mesma no método Decodificar
                    tripledescryptoserviceprovider.Key = md5cryptoserviceprovider.ComputeHash(ASCIIEncoding.ASCII.GetBytes(myKey));
                    tripledescryptoserviceprovider.Mode = CipherMode.ECB;
                    ICryptoTransform desdencrypt = tripledescryptoserviceprovider.CreateEncryptor();
                    ASCIIEncoding MyASCIIEncoding = new ASCIIEncoding();
                    byte[] buff = Encoding.ASCII.GetBytes(entrada);

                    return Convert.ToBase64String(desdencrypt.TransformFinalBlock(buff, 0, buff.Length));

                }
                else
                {
                    return "";
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                tripledescryptoserviceprovider = null;
                md5cryptoserviceprovider = null;
            }

        }

        public static string Decodificar(string entrada)
        {
            TripleDESCryptoServiceProvider tripledescryptoserviceprovider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider md5cryptoserviceprovider = new MD5CryptoServiceProvider();

            try
            {
                if (entrada.Trim() != "")
                {
                    string myKey = "39204391000100";  //Aqui vc inclui uma chave qualquer para servir de base para cifrar, que deve ser a mesma no método Codificar
                    tripledescryptoserviceprovider.Key = md5cryptoserviceprovider.ComputeHash(ASCIIEncoding.ASCII.GetBytes(myKey));
                    tripledescryptoserviceprovider.Mode = CipherMode.ECB;
                    ICryptoTransform desdencrypt = tripledescryptoserviceprovider.CreateDecryptor();
                    byte[] buff = Convert.FromBase64String(entrada);

                    return ASCIIEncoding.ASCII.GetString(desdencrypt.TransformFinalBlock(buff, 0, buff.Length));
                }
                else
                {
                    return "";
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                tripledescryptoserviceprovider = null;
                md5cryptoserviceprovider = null;
            }

        }
    }
}
