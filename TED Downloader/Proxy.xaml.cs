using System;
using System.Windows;
using System.Net;

namespace TED_Downloader
{
    /// <summary>
    /// Interaction logic for Proxy.xaml
    /// </summary>
    public partial class Proxy : Window
    {
        public IPAddress IPAddressValue {get; set;}
        public int IPPortValue { get; set; }
        public bool IPEntered { get; set; }

        public Proxy()
        {
            InitializeComponent();
            IPEntered = false;
        }

        public Proxy(String IPAddressstrValue, int IPPortValue)
        {
            InitializeComponent();

            if (!String.IsNullOrEmpty(IPAddressstrValue))
            {
                this.IPAddressValue = IPAddress.Parse(IPAddressstrValue);
                IPAddresstxt.Text = IPAddressValue.ToString();
            }
            if (IPPortValue != 0)
            {
                this.IPPortValue = IPPortValue;
                IPPorttxt.Text = IPPortValue.ToString();
            }
            IPEntered = false;
        }
        
        private void SubmitProxy(object sender, RoutedEventArgs e)
        {
            int _ipport;
            IPAddress _ipaddressvalue;

            if (IPAddress.TryParse(IPAddresstxt.Text, out _ipaddressvalue) && (int.TryParse(IPPorttxt.Text, out _ipport) || String.IsNullOrEmpty(IPPorttxt.Text)))
            {                
                IPAddressValue = _ipaddressvalue;
                IPPortValue = _ipport;
                IPEntered = true;
                this.Close();
            }
            else
            {
                IPAddresstxt.Text = String.Empty;
                IPPorttxt.Text = String.Empty;
                ErrroLbl.Visibility = System.Windows.Visibility.Visible;
            }            
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            IPEntered = false;
            this.Close();
        }      
    }
}
