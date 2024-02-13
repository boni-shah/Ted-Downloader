using System;
using System.Net;
using System.Windows;
using Microsoft.Win32;
using TEDinator.TEDClasses;

namespace TEDinator.Handler
{
    class RegHandler
    {
        RegistryKey baseRegistryKey = Registry.CurrentUser;
        String Subkey = "Software\\OUtilities";

        const String Application_Corrupt_Error = "The Application has been Corrupted!!";
        const String Registry_read_Error = "Error reading values from the Registry";
        const String Registry_write_Error = "Error writing values to the Registry";
        const String Resetting_Data_msg = "Resetting all data......\nError code : ";

        public void InitialiseReg(ApplicationSettings ObjApplicationSettings)
        {
            RegistryKey rk = baseRegistryKey;
            RegistryKey sk = rk.OpenSubKey(Subkey);
            try
            {
                if (sk == null)
                    MessageBox.Show(Constants.Welcome_msg, "Welcome to the Family");
                else
                    read_from_registry(ObjApplicationSettings);
            }
            catch (Exception e)
            {
                MessageBox.Show(Resetting_Data_msg + e.Message, Application_Corrupt_Error);
                reset_data();
            }
        }

        #region Registry Operations
        /// <summary>
        /// This are the only Functions which interact with the Registry.
        /// </summary>
        public void write_to_registry(ApplicationSettings ObjApplicationSettings)
        {
            RegistryKey rk = baseRegistryKey;
            RegistryKey sk = rk.CreateSubKey(Subkey, Microsoft.Win32.RegistryKeyPermissionCheck.ReadWriteSubTree);
            try
            {
                sk.SetValue("Created by", "Obin Shah");
                sk.SetValue("Download Folder Location", ObjApplicationSettings.Download_Folder);
                sk.SetValue("Download Quality", ObjApplicationSettings.Download_Quality);
                sk.SetValue("IP Address", ObjApplicationSettings.IPAddress);
                sk.SetValue("IP Port", ObjApplicationSettings.IPPort);
                sk.SetValue("Subtitle Language", ObjApplicationSettings.SubtitleLanguage);
                sk.SetValue("Last Run Date", ObjApplicationSettings.LastRunDate);
                sk.SetValue("No. of Runs", ObjApplicationSettings.RunCount);
                sk.SetValue("Error Count", ObjApplicationSettings.ErrorCount);
            }
            catch (Exception e)
            {
                MessageBox.Show(Resetting_Data_msg + e.Message, Registry_write_Error);
                reset_data();
            }
            sk.Close();
            rk.Close();
        }

        private void read_from_registry(ApplicationSettings ObjApplicationSettings)
        {
            RegistryKey rk = baseRegistryKey;
            RegistryKey sk = rk.OpenSubKey(Subkey, true);
            try
            {
                ObjApplicationSettings.Download_Folder = sk.GetValue("Download Folder Location").ToString();
                ObjApplicationSettings.Download_Quality = (int)sk.GetValue("Download Quality");
                ObjApplicationSettings.IPAddress = IPAddress.Parse(sk.GetValue("IP Address").ToString());
                ObjApplicationSettings.IPPort = (int)sk.GetValue("IP Port");
                ObjApplicationSettings.SubtitleLanguage = sk.GetValue("Subtitle Language").ToString();
                ObjApplicationSettings.LastRunDate = sk.GetValue("Last Run Date").ToString();
                ObjApplicationSettings.RunCount = (int)sk.GetValue("No. of Runs");
                ObjApplicationSettings.ErrorCount = (int)sk.GetValue("Error Count");
            }
            catch (Exception e)
            {
                MessageBox.Show(Resetting_Data_msg + e.Message, Registry_read_Error);
                reset_data();
            }
            sk.Close();
            rk.Close();
        }
        #endregion

        #region Utility Functions
        /// <summary>
        /// Utility Functions.
        /// </summary>

        private void reset_data()
        {
            RegistryKey rk = baseRegistryKey;
            rk.DeleteSubKey(Subkey);
            rk.Close();
        }

        #endregion
    }
}