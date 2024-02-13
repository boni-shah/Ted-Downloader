using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml;
using TEDinator.Handler;
using TEDinator.TEDClasses;

/*  
 * EnableColumnVirtualization="True" EnableRowVirtualization="True" instead of VirtualizingStackPanel.IsVirtualizing="True"
 * To-do : Compare timings between wc.download string/stream and httpwebrequest 
 * hit same if error count reaches 20, show video count in settings, show statistics button, hit site on every 50 download anonymously
 */

namespace TEDinator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Global Variables

        int total;
        int current;
        int currentSearch = -1;

        private bool isinCancel;

        private static BackgroundWorker TED_Analyser_Worker;
        private Dispatcher Main_Dispathcer;

        public ObservableCollection<TED_Video> TEDLinks_All { get; set; }
        public ObservableCollection<TED_Video> TEDLinks_Selected { get; set; }

        static WebClient Video_WC = new WebClient();
        //static WebClient Subtitle_WC = new WebClient();

        ApplicationSettings ObjApplicationSettings = new ApplicationSettings();
        RegHandler objReghandler = new RegHandler();

        About Aboutbox;
        Settings SettingsWindow;
        iCare iCarebox;

        string TempfileName = String.Empty;
        string ActualFileName = String.Empty;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            Video_WC.DownloadProgressChanged += Video_WC_DownloadProgressChanged;
            Video_WC.DownloadFileCompleted += Video_WC_DownloadFileCompleted;

            //Subtitle_WC.DownloadStringCompleted += new DownloadStringCompletedEventHandler(Subtitle_WC_DownloadStringCompleted);

            objReghandler.InitialiseReg(ObjApplicationSettings);
            if (ObjApplicationSettings.RunCount % 10 == 0)
                Process.Start(new ProcessStartInfo(Constants.ScenarioSolution_TEDTagpage));

            ObjApplicationSettings.RunCount++;
            ObjApplicationSettings.LastRunDate = DateTime.Today.ToShortDateString();
            if (ObjApplicationSettings.Download_Folder.Equals(Constants.NotFound) || String.IsNullOrEmpty(ObjApplicationSettings.Download_Folder))
                SettingsWindow_Show();
            if (!ObjApplicationSettings.IPAddress.Equals(IPAddress.Parse("0.0.0.0")))
                SetProxy();

            Main_Dispathcer = Dispatcher.CurrentDispatcher;
            TED_Analyser_Worker = new BackgroundWorker {WorkerSupportsCancellation = true};

            TEDLinks_All = new ObservableCollection<TED_Video>();
            TEDLinks_Selected = new ObservableCollection<TED_Video>();

            TED_Progress_Notification_txt.Text = Constants.OpeningNotificationmsg;
            TED_Progress_Notification_txt.ScrollToEnd();

            TED_Analyser_Worker.DoWork += (s, args) =>
            {
                Main_Dispathcer.BeginInvoke(new Action(() =>
                {
                    ButtonCancel.IsEnabled = false;
                    ButtonSkip.IsEnabled = false;
                    enableScreen(false);
                    msglbl.Content = Constants.Analysingmsg;
                    TEDLinks_Selected.Clear();
                    TEDLinks_All.Clear();
                }));
                var v = StaticLogic.TED_Analysis();
                Main_Dispathcer.BeginInvoke(new Action(() => { TEDLinks_All = v; }));
            };
            TED_Analyser_Worker.RunWorkerCompleted += (s, args) =>
            {
                enableScreen(true);
                if (TEDLinks_All.Count != 0)
                    Selection_Screen.ItemsSource = TEDLinks_All;

                msglbl.Content = String.Format(Constants.AnalysisDonemsg, TEDLinks_All.Count());
            };

            TED_Analyser_Worker.RunWorkerAsync();
        }

        #region Utility Functions

            private void enableScreen(bool flag)
            {
                ButtonStart.IsEnabled = flag;
                ButtonRefresh.IsEnabled = flag;
                Download_All_Videos_chkbx.IsEnabled = flag;
                Exportlinksbtn.IsEnabled = flag;
                if (flag)
                    enableSearchGrid(Download_All_Videos_chkbx.IsChecked != null && !((bool)Download_All_Videos_chkbx.IsChecked));
                else
                    enableSearchGrid(false);
            }

            private ObservableCollection<TED_Video> GetRow(String Temp_Title)
            {
                ObservableCollection<TED_Video> filteredClients = null;
                IEnumerable<TED_Video> qry = from c in TEDLinks_All
                                             where c.Video_Title.ToUpper().Equals(Temp_Title.ToUpper())
                                             orderby c.Video_Title
                                             select c;

                filteredClients = new ObservableCollection<TED_Video>(qry);
                return filteredClients;
            }

            private String CreateProperName(String Old_Target_Path)
            {
                String New_Target_Path = String.Empty;
                foreach (Char c in Old_Target_Path)
                    if (c == '\\' || c == '/' || c == ':' || c == '*' || c == '?' || c == '<' || c == '\"' || c == '>' || c == '|')     //Removes  \/:*?"<>|
                        New_Target_Path += " - ";
                    else
                        New_Target_Path += c;
                return New_Target_Path;
            }

            private void SetProxy()
            {
                if (ObjApplicationSettings.IPAddress.Equals(IPAddress.Parse("0.0.0.0"))) 
                    return;

                var prxy = ObjApplicationSettings.IPPort == 0 ? 
                    new WebProxy(ObjApplicationSettings.IPAddress.ToString()) : 
                    new WebProxy(ObjApplicationSettings.IPAddress.ToString(), ObjApplicationSettings.IPPort);

                prxy.Credentials = CredentialCache.DefaultCredentials;
                WebRequest.DefaultWebProxy = prxy;
            }

            private string FromMilliSecondstoString(int millisecs)
            {
                int hours = millisecs / 1000 / 60 / 60;
                int mins = millisecs / 1000 / 60 % 60;
                int secs = millisecs / 1000 % 60;
                int ms = millisecs % 1000;

                return (String.Format("{0:00}:{1:00}:{2:00},{3:000}", hours, mins, secs, ms));
            }

        #endregion

        #region Click Events

            private void Chk_Checked(object sender, RoutedEventArgs e)
            {
                var Temp_Title = ((sender as CheckBox).Tag as TED_Video).Video_Title;

                foreach (var item in GetRow(Temp_Title))
                {
                    if (TEDLinks_Selected.Contains(item))
                        continue;

                    var index = TEDLinks_All.IndexOf(item);
                    TEDLinks_Selected.Add(item);

                    if (index != -1)
                        TEDLinks_All[index].Status = true;
                }
                DisplayCountLbl();
            }

            private void Chk_UnChecked(object sender, RoutedEventArgs e)
            {
                var Temp_Title = ((sender as CheckBox).Tag as TED_Video).Video_Title;

                foreach (var item in GetRow(Temp_Title))
                {
                    if (TEDLinks_Selected.Contains(item))
                    {
                        var index = TEDLinks_All.IndexOf(item);
                        TEDLinks_Selected.Remove(item);

                        if (index != -1)
                            TEDLinks_All[index].Status = false;
                    }
                    DisplayCountLbl();
                }
            }
        
            private void Infobtn_Click(object sender, RoutedEventArgs e)
            {
                if (Aboutbox == null)
                {
                    Aboutbox = new About(this);
                    Aboutbox.Show();
                    Aboutbox.Closed += Aboutbox_Closed;
                }
                else
                    Aboutbox.Focus();
            }

            void Aboutbox_Closed(object sender, EventArgs e)
            {
                Aboutbox = null;
            }

            private void Settingsbtn_Click(object sender, RoutedEventArgs e)
            {
                SettingsWindow_Show();
            }

            private void SettingsWindow_Show()
            {
                if (SettingsWindow == null)
                {
                    SettingsWindow = new Settings(ObjApplicationSettings);
                    SettingsWindow.Closed += SettingsWindow_Closed;
                    SettingsWindow.ShowDialog();
                }
                else
                    SettingsWindow.Focus();
            }

            void SettingsWindow_Closed(object sender, EventArgs e)
            {
                if (SettingsWindow.Save_Succesful)
                {
                    ObjApplicationSettings.Download_Folder = SettingsWindow.download_folder;
                    ObjApplicationSettings.Download_Quality = SettingsWindow.download_quality;
                    ObjApplicationSettings.IPAddress = SettingsWindow.ipaddress;
                    ObjApplicationSettings.IPPort = SettingsWindow.ipport;
                    //ObjApplicationSettings.SubtitleLanguage = SettingsWindow.subtitle_language;

                    objReghandler.write_to_registry(ObjApplicationSettings);
                    SetProxy();
                }
                SettingsWindow = null;
            }

            private void Refreshbtn_Click(object sender, RoutedEventArgs e)
            {
                IndividualProgressBar.Value = 0;
                OverallProgressBar.Value = 0;
                IndividualProgresslbl.Content = Constants.ZeroPercentDonemsg;
                OverallProgresslbl.Content = Constants.ZeroPercentDonemsg;

                TED_Progress_Notification_txt.Text = "";
                Searchtxt.Text = "";
                currentSearch = -1;

                TED_Analyser_Worker.RunWorkerAsync();
            }

            private void All_Videos_Select(object sender, RoutedEventArgs e)
            {
                enableSearchGrid(false);
                msglbl.Content = String.Format(Constants.DisplayCountLblmsg, TEDLinks_All.Count, TEDLinks_All.Count);
            }

            private void All_Videos_UnSelect(object sender, RoutedEventArgs e)
            {
                enableSearchGrid(true);
                DisplayCountLbl();
            }

            void Exportlinksbtn_Click(object sender, RoutedEventArgs e)
            {
                ExportPopup.IsOpen = !ExportPopup.IsOpen;
            }

            void ExportList_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                var selectedval = ((ListBoxItem)ExportList.SelectedItem).Content.ToString();
                ExportList.SelectedItem = -1;
                ExportPopup.IsOpen = false;

                if (String.IsNullOrEmpty(ObjApplicationSettings.Download_Folder) || ObjApplicationSettings.Download_Folder.Equals(Constants.NotFound))
                    MessageBox.Show(Application.Current.MainWindow, Constants.ValidPathErrormsg, Constants.UserErrormsg);
                else
                {
                    if (!Directory.Exists(ObjApplicationSettings.Download_Folder))  Directory.CreateDirectory(ObjApplicationSettings.Download_Folder);
                    if (Download_All_Videos_chkbx.IsChecked != null && ((TEDLinks_Selected.Count > 0) || ((bool)Download_All_Videos_chkbx.IsChecked)))
                    {
                        var qualtempval = ObjApplicationSettings.Download_Quality;
                        var qualitytemp = ((Enums.DownloadQuality)qualtempval).ToString();

                        ObservableCollection<TED_Video> temptedcoll;
                        if ((bool)Download_All_Videos_chkbx.IsChecked)
                            temptedcoll = TEDLinks_All;
                        else
                            temptedcoll = TEDLinks_Selected;

                        switch (selectedval)
                        {
                            case "Text File": ExporttoText(temptedcoll, qualtempval, qualitytemp); break;
                            case "HTML File": ExporttoHTML(temptedcoll, qualtempval, qualitytemp); break;
                            case "XML File": ExporttoXML(temptedcoll, qualtempval, qualitytemp); break;
                        }

                        MessageBox.Show(Application.Current.MainWindow, Constants.Success);
                    }
                    else
                        MessageBox.Show(Application.Current.MainWindow, Constants.NoFileSelectedErrormsg, Constants.UserErrormsg);
                }
            }

            private void ExporttoText(ObservableCollection<TED_Video> tempted, int tempqual, string qualitytemp)
            {
                String TED_txt_File = ObjApplicationSettings.Download_Folder + "\\ExportedTEDLinks_" + qualitytemp + ".txt";
                StreamWriter TED_SW = new StreamWriter(TED_txt_File);

                TED_SW.BaseStream.Seek(0, SeekOrigin.End);
                TED_SW.WriteLine("");

                foreach (TED_Video item in tempted)
                    if (tempqual == 1)
                        TED_SW.WriteLine(item.Download_Location_low);
                    else if (tempqual == 2)
                        TED_SW.WriteLine(item.Download_Location_med);
                    else
                        TED_SW.WriteLine(item.Download_Location_high);

                TED_SW.Flush();
                TED_SW.Close();
            }

            private void ExporttoHTML(ObservableCollection<TED_Video> tempted, int tempqual, string qualitytemp)
            {
                var TED_html_File = ObjApplicationSettings.Download_Folder + "\\ExportedTEDLinks_" + qualitytemp + ".html";
                var TED_SW = new StreamWriter(TED_html_File);

                TED_SW.BaseStream.Seek(0, SeekOrigin.End);

                TED_SW.Write("<html><head><title>Exported TED Files</title></head><body><table align='center' border='1'><tr><tr><th>Name</th><th>Date</th><th>Event</th><th>Duration</th><th>Download Link</th></tr></tr>");

                foreach (var item in tempted)
                {
                    TED_SW.Write("<tr style='text-align: center;'>");
                    TED_SW.Write("<td style='text-align: left;'><a href='" + item.Video_Homepage + "'>" + item.Video_Title + "</a></td>");
                    TED_SW.Write("<td>" + item.Date + "</td>");
                    TED_SW.Write("<td>" + item.Event_Name + "</td>");
                    TED_SW.Write("<td>" + item.Duration + "</td>");

                    TED_SW.Write("<td><a href='");
                    switch (tempqual)
                    {
                        case 1:
                            TED_SW.WriteLine(item.Download_Location_low);
                            break;
                        case 2:
                            TED_SW.WriteLine(item.Download_Location_med);
                            break;
                        default:
                            TED_SW.WriteLine(item.Download_Location_high);
                            break;
                    }
                    TED_SW.Write("'>Download</a></td></tr>");
                }

                TED_SW.Write("</table></body></html>");
                TED_SW.Flush();
                TED_SW.Close();
            }

            private void ExporttoXML(ObservableCollection<TED_Video> tempted, int tempqual, string qualitytemp)
            {
                var XDoc = new XmlDocument();
                var TED_XML_File = ObjApplicationSettings.Download_Folder + "\\ExportedTEDLinks_" + qualitytemp + ".xml";
                var writer = new XmlTextWriter(TED_XML_File, null) {Formatting = Formatting.Indented};

                var declaration = XDoc.CreateNode(XmlNodeType.XmlDeclaration, null, null);// Create XML declaration
                XDoc.AppendChild(declaration);
                var XElemRoot = XDoc.CreateElement("TED_Download_List");// Create root node.                    
                XDoc.AppendChild(XElemRoot);//Add the node to the document.

                foreach (var item in tempted)
                {
                    var Xsource = XDoc.CreateElement("Video_Details");
                    XElemRoot.AppendChild(Xsource);

                    var XTemp = XDoc.CreateElement("Video_Title");
                    XTemp.InnerText = item.Video_Title;
                    Xsource.AppendChild(XTemp);

                    XTemp = XDoc.CreateElement("Video_HomePage");
                    XTemp.InnerText = item.Video_Homepage;
                    Xsource.AppendChild(XTemp);

                    XTemp = XDoc.CreateElement("Event_Name");
                    XTemp.InnerText = item.Event_Name;
                    Xsource.AppendChild(XTemp);

                    XTemp = XDoc.CreateElement("Date");
                    XTemp.InnerText = item.Date;
                    Xsource.AppendChild(XTemp);

                    XTemp = XDoc.CreateElement("Duration");
                    XTemp.InnerText = item.Duration;
                    Xsource.AppendChild(XTemp);

                    XTemp = XDoc.CreateElement("Download_Location");
                    if (tempqual == 1)
                        XTemp.InnerText = item.Download_Location_low;
                    else if (tempqual == 2)
                        XTemp.InnerText = item.Download_Location_med;
                    else
                        XTemp.InnerText = item.Download_Location_high;
                    Xsource.AppendChild(XTemp);
                }
                XDoc.Save(writer);
            }

            private void DisplayCountLbl()
            {
                msglbl.Content = String.Format(Constants.DisplayCountLblmsg, TEDLinks_Selected.Count, TEDLinks_All.Count);
            }

            private void Startbtn_Click(object sender, RoutedEventArgs e)
            {
                currentSearch = -1;
                if (String.IsNullOrEmpty(ObjApplicationSettings.Download_Folder) || ObjApplicationSettings.Download_Folder.Equals(Constants.NotFound))
                    MessageBox.Show(Application.Current.MainWindow, Constants.ValidPathErrormsg, Constants.UserErrormsg);
                else
                {
                    if (Download_All_Videos_chkbx.IsChecked != null && (bool)Download_All_Videos_chkbx.IsChecked)
                    {
                        //if (TEDLinks_All.Count != TEDLinks_Selected.Count)
                        //    TEDLinks_Selected.Clear();

                        foreach (var item in TEDLinks_All.Where(item => !item.Status))
                        {
                            if (TEDLinks_Selected.Contains(item))
                                continue;

                            item.IsAddedThroughBulk = true;
                            TEDLinks_Selected.Add(item);
                        }
                    }

                    if (TEDLinks_Selected.Count != 0)
                    {
                        current = 1;//or zero
                        total = TEDLinks_Selected.Count;//or minus 1
                        TED_Progress_Notification_txt.Text = Constants.DownloadStartmsg;
                        TED_Progress_Notification_txt.Text += String.Format(Constants.DisplayCountLblmsg, TEDLinks_Selected.Count, TEDLinks_All.Count);
                        OverallProgressBar.Maximum = total;
                        OverallProgressBar.Value = 0;
                        IndividualProgressBar.Value = 0;

                        enableScreen(false);
                        isinCancel = false;
                        ButtonCancel.IsEnabled = true;
                        ButtonSettings.IsEnabled = false;
                        QueueDownloadFiles();
                    }
                    else
                        MessageBox.Show(Application.Current.MainWindow, Constants.NoFileSelectedErrormsg, Constants.UserErrormsg);
                }
            }

            private void QueueDownloadFiles()
            {
                if (isinCancel || Video_WC.IsBusy) 
                    return;

                var VideoFileName = String.Empty;
                var TargetPath = String.Empty;
                while (current <= total)
                {
                    var TED_Object = TEDLinks_Selected[current - 1];
                    TargetPath = ObjApplicationSettings.Download_Folder + @"\TED_" + (Enums.DownloadQuality)ObjApplicationSettings.Download_Quality + "\\" + TED_Object.Event_Name + "_" + TED_Object.Date;

                    VideoFileName = TargetPath + "\\" + CreateProperName(TED_Object.Video_Title) + ".mp4";

                    if (File.Exists(VideoFileName))
                        current++;
                    else
                        break;
                }
                if (current <= total)
                {
                    var index = current - 1;

                    OverallProgressBar.Value = index;
                    var PercentDone = (int)((float)(index) / total * 100);
                    OverallProgresslbl.Content = PercentDone + "%";
                    OverallProgressBar.ToolTip = Constants.Progressbarmsg(PercentDone);

                    IndividualProgressBar.Value = 0;
                    IndividualProgresslbl.Content = Constants.ZeroPercentDonemsg;
                    msglbl.Content = "Downloading " + TEDLinks_Selected[index].Video_Title + "...";

                    TED_Progress_Notification_txt.Text += String.Format(Constants.DownloadingItemmsg, TEDLinks_Selected[index].Video_Title);
                    TED_Progress_Notification_txt.ScrollToEnd();

                    if (!Directory.Exists(TargetPath)) Directory.CreateDirectory(TargetPath);

                    if (File.Exists(VideoFileName)) 
                        return;

                    ActualFileName = VideoFileName;
                    switch (ObjApplicationSettings.Download_Quality)
                    {
                        case 1:
                            DownloadVideo(TEDLinks_Selected[index].Download_Location_low, current);
                            break;
                        case 2:
                            DownloadVideo(TEDLinks_Selected[index].Download_Location_med, current);
                            break;
                        default:
                            DownloadVideo(TEDLinks_Selected[index].Download_Location_high, current);
                            break;
                    }
                }
                else
                {
                    IndividualProgressBar.Value = IndividualProgressBar.Maximum;
                    OverallProgressBar.Value = OverallProgressBar.Maximum;
                    IndividualProgresslbl.Content = Constants.HundredPercentDonemsg;
                    OverallProgresslbl.Content = Constants.HundredPercentDonemsg;
                    OverallProgressBar.ToolTip = null;
                    msglbl.Content = Constants.DownloadCompleted;

                    TED_Progress_Notification_txt.Text += Constants.DownloadCompleted + ".\n";
                    TED_Progress_Notification_txt.ScrollToEnd();

                    var Explorer = new ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = ObjApplicationSettings.Download_Folder
                    };
                    Process.Start(Explorer);

                    WindowState = WindowState.Normal;
                    enableScreen(true);
                    isinCancel = false;
                    ButtonCancel.IsEnabled = false;
                    ButtonSkip.IsEnabled = false;
                    ButtonSettings.IsEnabled = true;
                    MessageBox.Show(Application.Current.MainWindow, Constants.DownloadCompleted);
                    current = 0;
                    total = 0;
                }
            }

            private void DownloadVideo(String url, int id)
            {
                ButtonSkip.IsEnabled = true;
                TempfileName = Path.GetTempFileName();
                if (url.Equals(Constants.NotFound))
                {
                    TED_Progress_Notification_txt.Text += Constants.VideoNotAvailable;
                    TED_Progress_Notification_txt.ScrollToEnd();
                    if (current == id)
                        current++;
                    QueueDownloadFiles();
                }
                else
                    Video_WC.DownloadFileAsync(new Uri(url), TempfileName, id);
            }

            public void Video_WC_DownloadProgressChanged(Object sender, DownloadProgressChangedEventArgs e)
            {
                IndividualProgressBar.Value = e.ProgressPercentage;
                IndividualProgresslbl.Content = IndividualProgressBar.Value + " %";
            }

            void Video_WC_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
            {
                ButtonSkip.IsEnabled = false;
                if (isinCancel)
                    return;

                if (e.Error != null)
                {
                    int id = Convert.ToInt32(e.UserState);
                    if (id == current)
                        current++;
                    if (!e.Cancelled)
                    {
                        TED_Progress_Notification_txt.Text += Constants.VideoDownloadFailed;
                        TED_Progress_Notification_txt.ScrollToEnd();
                    }
                }
                else
                {
                    File.Move(TempfileName, ActualFileName);
                    File.Delete(TempfileName);
                    TED_Progress_Notification_txt.Text += Constants.VideoDownloadSuccesful;
                    TED_Progress_Notification_txt.ScrollToEnd();
                }
                QueueDownloadFiles();
            }

            private void Cancelbtn_Click(object sender, RoutedEventArgs e)
            {
                isinCancel = true;
                OverallProgressBar.ToolTip = null;
                if (Video_WC.IsBusy)
                    Video_WC.CancelAsync();

                var SelectedList = TEDLinks_Selected.Where(item => item.IsAddedThroughBulk).ToList();
                foreach (var item in SelectedList)
                {
                    item.IsAddedThroughBulk = false;
                    TEDLinks_Selected.Remove(item);
                }

                msglbl.Content = Constants.DownloadCancelled;
                TED_Progress_Notification_txt.Text += Constants.DownloadCancelled + ".\n";
                TED_Progress_Notification_txt.ScrollToEnd();

                enableScreen(true);
                ButtonCancel.IsEnabled = false;
                ButtonSkip.IsEnabled = false;
                ButtonSettings.IsEnabled = true;

                current = 0;
                total = 0;
            }

            private void Skipbtn_Click(object sender, RoutedEventArgs e)
            {
                if (Video_WC.IsBusy)
                {
                    Video_WC.CancelAsync();

                    TED_Progress_Notification_txt.Text += Constants.VideoSkippedmsg;
                    TED_Progress_Notification_txt.ScrollToEnd();
                }
            }

            private void Carebtn_Click(object sender, RoutedEventArgs e)
            {
                if (iCarebox == null)
                {
                    iCarebox = new iCare();
                    iCarebox.Show();
                    iCarebox.Closed += iCarebox_Closed;
                }
                else
                    iCarebox.Focus();
            }

            void iCarebox_Closed(object sender, EventArgs e)
            {
                iCarebox = null;
            }

            private void TED_Closing(object sender, CancelEventArgs e)
            {
                if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length == 1)
                {
                    isinCancel = true;
                    if (Video_WC.IsBusy)
                        Video_WC.CancelAsync();
                    objReghandler.write_to_registry(ObjApplicationSettings);
                }
            }

            private void Searchtxt_TextChanged(object sender, TextChangedEventArgs e)
            {
                if (!string.IsNullOrEmpty(Searchtxt.Text))
                {
                    var Suggestionlist = new List<string>();

                    for (var i = 0; i < TEDLinks_All.Count; i++)
                    {
                        var item = TEDLinks_All[i];
                        if (item.Video_Title.IndexOf(Searchtxt.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                            Suggestionlist.Add(item.Video_Title);
                    }

                    if (Suggestionlist.Count > 0)
                    {
                        Suggestionlstbx.ItemsSource = Suggestionlist;
                        SuggestionPopup.IsOpen = true;
                    }
                    else
                    {
                        Suggestionlstbx.ItemsSource = null;
                        SuggestionPopup.IsOpen = false;
                    }
                }
                else
                {
                    Suggestionlstbx.ItemsSource = null;
                    SuggestionPopup.IsOpen = false;
                }
            }

            private void enableSearchGrid(bool flag)
            {
                Selection_Screen.IsEnabled = flag;
                Searchtxt.IsEnabled = flag;
                ButtonSearch.IsEnabled = flag;
            }

            private void ButtonSearch_Click(object sender, RoutedEventArgs e)
            {
                if (!String.IsNullOrEmpty(Searchtxt.Text))
                {
                    bool termfound = false;
                    for (int i = currentSearch + 1; i < TEDLinks_All.Count; i++)
                    {
                        if (currentSearch == i)
                        {
                            termfound = true;
                            MessageBox.Show(Application.Current.MainWindow, Constants.SingleItemErrormsg);
                            break;
                        }
                        var item = TEDLinks_All[i];
                        if (item.Video_Title.IndexOf(Searchtxt.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            termfound = true;
                            currentSearch = i;
                            Selection_Screen.SelectedItem = item;
                            Selection_Screen.ScrollIntoView(item);
                            break;
                        }
                        if (i == TEDLinks_All.Count - 1)
                            i = 0;
                    }
                    if (!termfound)
                        MessageBox.Show(Application.Current.MainWindow, Constants.TermnotFoundErrormsg);
                }
            }

            private void Suggestionlstbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                if (Suggestionlstbx.ItemsSource != null)
                {
                    Searchtxt.TextChanged -= Searchtxt_TextChanged;
                    Searchtxt.Text = Suggestionlstbx.SelectedItem.ToString();
                    Searchtxt.TextChanged += Searchtxt_TextChanged;
                    Suggestionlstbx.ItemsSource = null;
                    SuggestionPopup.IsOpen = false;
                    ButtonSearch_Click(this, new RoutedEventArgs());
                }
            }
    }
        #endregion
}