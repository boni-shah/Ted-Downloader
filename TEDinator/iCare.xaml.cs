using System.Diagnostics;
using System.Windows;

namespace TEDinator
{
    /// <summary>
    /// Interaction logic for iCare.xaml
    /// </summary>
    public partial class iCare : Window
    {
        public iCare()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles click navigation on the hyperlink in the About dialog.
        /// </summary>
        /// <param name="sender">Object the sent the event.</param>
        /// <param name="e">Navigation events arguments.</param>
        private void hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (e.Uri != null && string.IsNullOrEmpty(e.Uri.OriginalString) == false)
            {
                string uri = e.Uri.AbsoluteUri;
                Process.Start(new ProcessStartInfo(uri));
                e.Handled = true;
            }
        }
    }
}
