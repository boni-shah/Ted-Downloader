using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace TED_Downloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables

            String DownloadFolderPathvar;
            String Download_Quality = Constants.Low;
            String File_Name = String.Empty;

            bool isDownload_Active = false;           
            int IPPortValue = 0;
            //int IPPortValue = 80;
            int TED_Videos_Count = 0;

            IPAddress IPAddressValue = IPAddress.Parse("0.0.0.0");  
            //IPAddress IPAddressValue = IPAddress.Parse("148.87.67.174");
            
            System.Windows.Forms.FolderBrowserDialog DownloadFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            About Aboutbox;
            
            Exception InvalidTagException = new Exception(Constants.Exception_Error_Text);

            private delegate void SimpleTEDDelegate();

            public ObservableCollection<TED_DList> TEDLinks_All { get; set; }
            public ObservableCollection<TED_DList> TEDLinks_Selected { get; set; }

        #endregion

        #region App Related Functions

            public MainWindow()
            {
                InitializeComponent();
                
                TEDLinks_All = new ObservableCollection<TED_DList>();
                TEDLinks_Selected = new ObservableCollection<TED_DList>();

                DisplayCountLbl();
               // SetProxy(); //Remove Later
            }

            private void DownloadFolderbtn_Click(object sender, RoutedEventArgs e)
            {
                if (!isDownload_Active)
                {
                    if (DownloadFolderBrowser.ShowDialog().Equals(System.Windows.Forms.DialogResult.OK))
                        DownloadFolderpath.Text = DownloadFolderBrowser.SelectedPath;
                }
                else
                    MessageBox.Show(Constants.Download_Error, Constants.User_Error);
            }

            private void InfoButton_Click(object sender, RoutedEventArgs e)
            {
                if (Aboutbox == null)
                {
                    Aboutbox = new About(this);
                    Aboutbox.Show();                    
                    Aboutbox.Closed += new EventHandler(Aboutbox_Closed);                    
                }
                else
                    Aboutbox.Focus();
            }

            private void ProxyButton_Click(object sender, RoutedEventArgs e)
            {
                if (!isDownload_Active)
                {
                    Proxy prxy = new Proxy(IPAddressValue.ToString(), IPPortValue);

                    prxy.ProxyWindow.ShowDialog();

                    if (prxy.IPEntered)
                    {
                        IPAddressValue = prxy.IPAddressValue;
                        IPPortValue = prxy.IPPortValue;
                        SetProxy();
                    }
                }
                else
                    MessageBox.Show(Constants.Download_Error, Constants.User_Error);
            }

            void SelectAllBtn_Clicked(object sender, RoutedEventArgs e)
            {
                TEDLinks_Selected.Clear();
                foreach (TED_DList item in TEDLinks_All)
                {
                    item.Status = true;
                    //if (!TEDLinks_Selected.Contains(new TED_DList(item)))   //bcos of this the item is added twice, once due to item.status and other this
                    //    TEDLinks_Selected.Add(new TED_DList(item));                        

                    Selection_Screen.SelectedItem = item;
                    Selection_Screen.ScrollIntoView(item);
                }
                TED_Selected_Videos_Count_lbl.Content = String.Format("Videos Selected : {0} of {1}", TEDLinks_All.Count, TEDLinks_All.Count);
            }

            private void Chk_Checked(object sender, RoutedEventArgs e)
            {
                String Temp_Title = ((sender as CheckBox).Tag as TED_DList).Video_Title.ToString();
                int index = -1;

                foreach (TED_DList item in GetRow(Temp_Title))
                {
                    if (!TEDLinks_Selected.Contains(item))
                    {
                        index = TEDLinks_All.IndexOf(item);
                        TEDLinks_Selected.Add(item);

                        if (index != -1)
                            TEDLinks_All[index].Status = true;

                        DisplayCountLbl();                        
                    }
                }                
            }

            private void Chk_UnChecked(object sender, RoutedEventArgs e)
            {
                String Temp_Title = ((sender as CheckBox).Tag as TED_DList).Video_Title.ToString();
                int index = -1;

                foreach (TED_DList item in GetRow(Temp_Title))
                {
                    if (TEDLinks_Selected.Contains(item))
                    {
                        index = TEDLinks_All.IndexOf(item);
                        TEDLinks_Selected.Remove(item);

                        if (index != -1)
                            TEDLinks_All[index].Status = false;

                        DisplayCountLbl();
                    }
                }
            }

            private void VidQual_Changed(object sender, RoutedEventArgs e)
            {
                try
                {
                if(TEDLinks_All != null)
                    TEDLinks_All.Clear();
                if (TEDLinks_Selected != null)
                    TEDLinks_Selected.Clear();
                    
                    DisplayCountLbl();
                }
                catch
                { }
            } 
        
            private void DownloadTedbtn_Click(object sender, RoutedEventArgs e)
            {
                InitialiseParameters();
                
                if (String.IsNullOrEmpty(DownloadFolderPathvar))
                    MessageBox.Show(Constants.Valid_Path_Error, Constants.User_Error);
                else
                {
                    if (!isDownload_Active)
                    {                        
                        try
                        {
                            if (TEDLinks_Selected.Count > 0)
                            {
                                isDownload_Active = true;

                                TED_Videos_Count = TEDLinks_Selected.Count;

                                Selection_Screen.Visibility = System.Windows.Visibility.Hidden;
                                TED_Selected_Videos_Count_lbl.Visibility = System.Windows.Visibility.Hidden;
                                Download_Screen_List.Visibility = System.Windows.Visibility.Visible;
                                TED_Download_Videos_Count_lbl.Visibility = System.Windows.Visibility.Visible;
                                TED_Progress_Notification_txt.Visibility = System.Windows.Visibility.Visible;

                                Download_Screen_List.ItemsSource = this.TEDLinks_Selected;

                                TED_Selected_Videos_Count_lbl.Content = "Videos remaining to be Downloaded = " + TEDLinks_Selected.Count;

                                SimpleTEDDelegate Download_Invoke_Delegate = new SimpleTEDDelegate(TED_Download);   ///This will delegate TED_Download();
                                Download_Invoke_Delegate.BeginInvoke(null, null);                            
                            }
                            else
                                MessageBox.Show(Constants.No_File_Selected_Error, Constants.User_Error);
                            
                        }
                        catch (System.Net.WebException Wex)
                        {
                            MessageBox.Show(Wex.Message, Constants.Connection_Error);
                            isDownload_Active = false;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, Constants.Error_Text);
                            isDownload_Active = false;
                        }                        
                    }
                    else
                        MessageBox.Show(Constants.Download_Error, Constants.User_Error);
                }
            }

            private void GetTedLinksbtn_Click(object sender, System.Windows.RoutedEventArgs e)
            {
                InitialiseParameters();

                if (String.IsNullOrEmpty(DownloadFolderPathvar))
                    MessageBox.Show(Constants.Valid_Path_Error, Constants.User_Error);
                else
                {
                    if (!isDownload_Active)
                    {
                        TED_Progress_Bar_lbl.Content = "Updating Links....Please Wait....";
                        TED_Progress_Bar.Value = 0.0;
                        TEDLinks_Selected.Clear();
                        try
                        {
                            if (StaticLogic.TED_Analysis(TEDLinks_All, Download_Quality))
                            {
                                Selection_Screen.Visibility = System.Windows.Visibility.Visible;
                                TED_Selected_Videos_Count_lbl.Visibility = System.Windows.Visibility.Visible;
                                Download_Screen_List.Visibility = System.Windows.Visibility.Hidden;
                                TED_Download_Videos_Count_lbl.Visibility = System.Windows.Visibility.Hidden;
                                TED_Progress_Notification_txt.Visibility = System.Windows.Visibility.Hidden;
                                
                                Selection_Screen.ItemsSource = TEDLinks_All;
                                                                
                                DisplayCountLbl();
                            }
                        }
                        catch (System.Net.WebException Wex)
                        {
                            MessageBox.Show(Wex.Message, Constants.Connection_Error);
                            isDownload_Active = false;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, Constants.Error_Text);
                            isDownload_Active = false;
                        }
                        TED_Progress_Bar_lbl.Content = "";
                    }
                    else
                        MessageBox.Show(Constants.Download_Error, Constants.User_Error);
                }
            }

            private void ExportTedLinksbtn_Click(object sender, System.Windows.RoutedEventArgs e)
            {
                InitialiseParameters();
                if (String.IsNullOrEmpty(DownloadFolderPathvar))
                    MessageBox.Show(Constants.Valid_Path_Error, Constants.User_Error);
                else
                {
                    if (!isDownload_Active)
                    {
                        if (TEDLinks_Selected.Count > 0)
                        {
                            ExportLinks();
                            MessageBox.Show(Constants.Success);
                        }
                        else
                            MessageBox.Show(Constants.No_File_Selected_Error, Constants.User_Error);
                    }
                    else
                        MessageBox.Show(Constants.Download_Error, Constants.User_Error);
                }
            }          

        #endregion

        #region Logic Functions
           
            private void TED_Download()
            {
                bool Result = true;
                ObservableCollection<TED_DList> Temp_Coll = new ObservableCollection<TED_DList>(TEDLinks_Selected);

                try
                {
                    SimpleTEDDelegate TED_Initialisation = delegate()
                    {                        
                        TEDLinks_Selected.Cast<TED_DList>();
                        ChangeStateto(false);

                        TED_Progress_Notification_txt.Text = "Download Inventory Created. Total Videos to be Downloaded = " + TED_Videos_Count + "\n\n";
                        TED_Progress_Bar_lbl.Content = "";
                        TED_Progress_Bar.Maximum = TED_Videos_Count;
                        TED_Progress_Bar.Value = 0.0;
                    };
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Send, TED_Initialisation);

                    foreach (TED_DList item in Temp_Coll)
                    {                        
                        String DownloadLink = Constants.NotFound;
                        String DisplayText = String.Empty;
                        String ProgressDisplayText = String.Empty;

                        String TargetPath = DownloadFolderPathvar + @"\TED_" + Download_Quality + "\\" + item.Event_Name.ToString() + "_" + item.Date.ToString();
                        String File_Name = TargetPath + "\\" + CreateProperName(item.Video_Title.ToString()) + ".mp4";
                        Double ProgressBarValue = 0; 
                        if (!System.IO.Directory.Exists(TargetPath))
                            System.IO.Directory.CreateDirectory(TargetPath);

                        CreateTextFile(item, TargetPath);
                        DownloadLink = item.Download_Location.ToString();
                        DisplayText = "Downloading " + item.Video_Title.ToString() + "........";

                        SimpleTEDDelegate TED_Notification1 = delegate()
                        {
                            TED_Progress_Notification_txt.Text += DisplayText;
                            ProgressBarValue = TED_Progress_Bar.Value;                            
                        };
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Send, TED_Notification1);

                        ProgressDisplayText = GetProgressText(((ProgressBarValue / Temp_Coll.Count) * 100));
                        Result = Download(DownloadLink, File_Name);
                        item.Status = false;
                        

                        SimpleTEDDelegate TED_Notification2 = delegate()
                        {
                            TED_Progress_Bar_lbl.Content = ProgressDisplayText;
                            if (Result)
                                TED_Progress_Notification_txt.Text += "Succesful.\n";
                            else
                                TED_Progress_Notification_txt.Text += "Failed.\n";

                            TED_Progress_Bar.Value += 1;
                        };
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Send, TED_Notification2);
                    }                    
                    MessageBox.Show("And we are done!!!!\nThe High Speed!! AirDownload has landed. Thank you for Flying with us. Please Watch your Step.","Success!!Yay!!");                    
                }
                catch (Exception ex)
                {
                    SimpleTEDDelegate TED_Termination = delegate()
                    {
                        TED_Progress_Notification_txt.Text += "Fatal Error Encountered. Download Aborted. Please try again.";
                    };
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Send, TED_Termination);
                    MessageBox.Show(ex.Message, Constants.Error_Text);
                }
                finally
                {
                    SimpleTEDDelegate TED_Termination = delegate()
                    {
                        TED_Progress_Bar_lbl.Content = "";
                        TED_Progress_Notification_txt.Text += "\n\nDownload Completed.";
                        ChangeStateto(true);                        
                    };
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Send, TED_Termination);
                    TED_Videos_Count = 0;                    
                    isDownload_Active = false;
                }
            }

        #endregion

        #region Small Utility Functions

            private String GetProgressText(Double Value)
            {
                String MessageText = String.Empty;
                                
                if (0 <= Value && Value <= 10)
                    MessageText = Constants.between0_10 ;
                else if (10 < Value && Value <= 20)
                    MessageText = Constants.between10_20;
                else if (20 < Value && Value <= 30)
                    MessageText = Constants.between20_30;
                else if (30 < Value && Value <= 40)
                    MessageText = Constants.between30_40;
                else if (40 < Value && Value <= 50)
                    MessageText = Constants.between40_50;
                else if (50 < Value && Value <= 60)
                    MessageText = Constants.between50_60;
                else if (60 < Value && Value <= 70)
                    MessageText = Constants.between60_70;
                else if (70 < Value && Value <= 80)
                    MessageText = Constants.between70_80;
                else if (80 < Value && Value <= 90)
                    MessageText = Constants.between80_90;
                else if (90 < Value && Value <= 99)
                    MessageText = Constants.between90_100;
                return MessageText;
            }

            private void InitialiseParameters()
            {
                DownloadFolderPathvar = DownloadFolderpath.Text;

                if ((bool)TED_VidQual_Low.IsChecked)
                    Download_Quality = Constants.Low;
                if ((bool)TED_VidQual_Reg.IsChecked)
                    Download_Quality = Constants.Regular;
                if ((bool)TED_VidQual_High.IsChecked)
                    Download_Quality = Constants.High;
            }

            private void DisplayCountLbl()
            {
                var TEDLinks_All_Count = TEDLinks_All == null ? 0 : TEDLinks_All.Count;
                var TEDLinks_Selected_Count = TEDLinks_Selected == null ? 0 : TEDLinks_Selected.Count;
                if(TED_Selected_Videos_Count_lbl !=null)
                    TED_Selected_Videos_Count_lbl.Content = String.Format("Videos Selected : {0} of {1}", TEDLinks_Selected_Count, TEDLinks_All_Count);
            }
        
            private void ExportLinks()
            {
                String TED_Details_File = DownloadFolderPathvar + "\\TED_Links_Exported_" + Download_Quality.ToString() + ".txt";
                StreamWriter TED_SW = new StreamWriter(TED_Details_File);

                TED_SW.BaseStream.Seek(0, SeekOrigin.End);

                TED_SW.WriteLine("");                 
                foreach (TED_DList item in TEDLinks_Selected)
                    TED_SW.WriteLine(item.Download_Location);

                TED_SW.Flush();
                TED_SW.Close();
            }

            private ObservableCollection<TED_DList> GetRow(String Temp_Title)
            {
                ObservableCollection<TED_DList> filteredClients = null;
                IEnumerable<TED_DList> qry = from c in TEDLinks_All
                                             where c.Video_Title.ToUpper().Equals(Temp_Title.ToUpper())
                                             orderby c.Video_Title
                                             select c;

                filteredClients = new ObservableCollection<TED_DList>(qry);
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
                WebProxy prxy;

                if (IPPortValue == 0)
                    prxy = new WebProxy(IPAddressValue.ToString());
                else
                    prxy = new WebProxy(IPAddressValue.ToString(), IPPortValue);

                prxy.Credentials = CredentialCache.DefaultCredentials;
                WebRequest.DefaultWebProxy = prxy;
            }

            private void ChangeStateto(bool State)
            {            
                TED_VidQual_Low.IsEnabled = State;
                TED_VidQual_Reg.IsEnabled = State;
                TED_VidQual_High.IsEnabled = State;
            }

            private void CreateTextFile(TED_DList TED_Row, String TargetPath)
            {
                try
                {
                    String TED_Details_File = TargetPath + "\\" + CreateProperName(TED_Row.Video_Title.ToString()) + ".txt";
                    StreamWriter TED_SW = new StreamWriter(TED_Details_File);

                    TED_SW.BaseStream.Seek(0, SeekOrigin.End);
                    TED_SW.WriteLine("Video Title : " + TED_Row.Video_Title.ToString());
                    TED_SW.WriteLine("Video HomePage : " + TED_Row.Video_HomePage.ToString());
                    TED_SW.WriteLine("Date : " + TED_Row.Date.ToString());
                    TED_SW.WriteLine("Event Name : " + TED_Row.Event_Name.ToString());
                    TED_SW.WriteLine("Duration : " + TED_Row.Duration.ToString());
                    TED_SW.WriteLine("Download Location : " + TED_Row.Download_Location.ToString());

                    TED_SW.Flush();
                    TED_SW.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Constants.Error_Text);
                }
            }

            private bool Download(String url, String FileName)
            {
                try
                {
                    if (url.Equals(Constants.NotFound))
                        return false;

                    if (File.Exists(FileName))
                        return true;

                    WebClient Download_Client = new WebClient();
                    Download_Client.DownloadFile(url, FileName);
                    return true;
                }
                catch
                { return false; }
            }

            void Aboutbox_Closed(object sender, EventArgs e)
            {
                Aboutbox = null;
            }

        #endregion             
    }
}