using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using TEDinator.TEDClasses;

namespace TEDinator
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        internal System.Windows.Forms.FolderBrowserDialog downloadfolderbrowser = new System.Windows.Forms.FolderBrowserDialog();

        public String download_folder = Constants.NotFound;
        public int download_quality = 1;   //0=Not Selected, 1=Low, 2=Medium, 3=High, By default Low Quality
        public IPAddress ipaddress;
        public int ipport = 0;
        public String subtitle_language = String.Empty;
        public bool Save_Succesful = false;

        public Settings(ApplicationSettings objApplicationSettings)
        {
            InitializeComponent();

            download_folder = objApplicationSettings.Download_Folder;
            download_quality = objApplicationSettings.Download_Quality;
            ipaddress = objApplicationSettings.IPAddress;
            ipport = objApplicationSettings.IPPort;
            subtitle_language = objApplicationSettings.SubtitleLanguage;

            if (!download_folder.Equals(Constants.NotFound))
                DownloadFolderpath.Text = download_folder;
            if (download_quality == 1)
                TED_VidQual_Low.IsChecked = true;
            else if (download_quality == 2)
                TED_VidQual_Reg.IsChecked = true;
            else
                TED_VidQual_High.IsChecked = true;

            if (!ipaddress.Equals(IPAddress.Parse("0.0.0.0")))
                IPAddresstxt.Text = ipaddress.ToString();

            if (ipport != 0)
                IPPorttxt.Text = ipport.ToString();

            Subtitle_combo.SelectedValue = subtitle_language;
            if (Subtitle_combo.SelectedValue == null)
                Subtitle_combo.SelectedValue = "en";
            Totalrun_value_lbl.Content = objApplicationSettings.RunCount;
        }

        private void DownloadFolderbtn_Click(object sender, RoutedEventArgs e)
        {
            if (downloadfolderbrowser.ShowDialog().Equals(System.Windows.Forms.DialogResult.OK))
                DownloadFolderpath.Text = downloadfolderbrowser.SelectedPath;
        }

        private void SaveSettingsbtn_Click(object sender, RoutedEventArgs e)
        {
            int _ipport;
            IPAddress _ipaddressvalue;

            if (String.IsNullOrEmpty(DownloadFolderpath.Text) || !DownloadFolderpath.Text.Contains("\\"))
                MessageBox.Show(Constants.ValidPathErrormsg, Constants.UserErrormsg);
            else
            {
                if (String.IsNullOrEmpty(IPPorttxt.Text) && String.IsNullOrEmpty(IPAddresstxt.Text))
                { ; }
                else
                {
                    if (IPAddress.TryParse(IPAddresstxt.Text, out _ipaddressvalue) && int.TryParse(IPPorttxt.Text, out _ipport))
                    {
                        ipaddress = _ipaddressvalue;
                        ipport = _ipport;
                    }
                    else
                    {
                        IPAddresstxt.Text = String.Empty;
                        IPPorttxt.Text = String.Empty;
                        ErrroLbl.Visibility = System.Windows.Visibility.Visible;
                        return;
                    }
                }
                download_folder = DownloadFolderpath.Text;
                if (TED_VidQual_Low.IsChecked == true)
                    download_quality = 1;
                else if (TED_VidQual_Reg.IsChecked == true)
                    download_quality = 2;
                else
                    download_quality = 3;
                subtitle_language = ((ComboBoxItem)Subtitle_combo.SelectedItem).Tag.ToString();
                Save_Succesful = true;
                this.Close();
            }
        }
    }
}