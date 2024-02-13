using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Data;
using System.Windows.Threading;


namespace TED_Downloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables

            public DataTable TED_Links = new DataTable();

            const String TED_DownloadPage_URL = @"http://www.ted.com/talks/quick-list";
            const String TED_HomePage_URL = @"http://www.ted.com";
            const String NotFound = "Video Not Found.";
            const String Exception_Error_Text = "Invalid Tag Encountered in table Downloads. Please Restart Application.";            
            const String Error_Text = "Ouch!!! We just had a Boo-Boo!!";

            String DownloadFolderPathvar;
            String Download_Quality = "Low";
            String File_Name = String.Empty;

            const String between0_10 = "This is Going to take some time. Don't hold your breath!!!";
            const String between10_20 = "We are getting closer inch by inch!!! Have Heart, my dear friend.";
            const String between20_30 = "You are thinking that if this was going any slower, it would be going backwards, right??";
            const String between30_40 = "Remember, Slow and Steady wins the Race.";
            const String between40_50 = "We are approaching something great....can you feel it ??";
            const String between50_60 = "Phew!!! We just crossed the halfway Mark!!";
            const String between60_70 = "Have Patience, Little One. We are getting there!!!";
            const String between70_80 = "I am really amazed at the patience you have shown!!! Mama's Proud, dear!!!";
            const String between80_90 = "Patience has its virtue and you are doing good!! Kudos!!!";
            const String between90_100 = "Just a bit more!!! Hang on, Mac!!!";

            int TED_Videos_Count = 0;

            bool isDownload_Active = false;            
            //bool Shutdown_System = false;
        
            Exception InvalidTagException = new Exception(Exception_Error_Text);
            System.Windows.Forms.FolderBrowserDialog DownloadFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            About Aboutbox;
            private delegate void SimpleTEDDelegate();

        #endregion

        #region App Related Functions

            public MainWindow()
            {
                InitializeComponent();
            }

            private void GtTED_Click(object sender, RoutedEventArgs e)
            {
                InitialiseParameters();

                if (String.IsNullOrEmpty(DownloadFolderPathvar))
                    MessageBox.Show("Please Select a Valid Path!!", "User Error!!");
                else
                {                    
                    if (!isDownload_Active)
                    {
                        isDownload_Active = true;                        
                        if (TED_Analysis())
                        {                            
                            dataGrid1.ItemsSource = TED_Links.DefaultView;

                            //SimpleTEDDelegate Download_Invoke_Delegate = new SimpleTEDDelegate(TED_Download);   ///This will delegate TED_Download();
                            //Download_Invoke_Delegate.BeginInvoke(null, null);                            
                        }
                    }
                    else
                        MessageBox.Show("Download is already Active!!!!", "User Error!!");
                }
            }

            private void DownloadFolderbtn_Click(object sender, RoutedEventArgs e)
            {
                if (DownloadFolderBrowser.ShowDialog().Equals(System.Windows.Forms.DialogResult.OK))
                    DownloadFolderpath.Text = DownloadFolderBrowser.SelectedPath;
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
        #endregion

        #region Logic Functions

            private bool TED_Analysis()
            {
                try
                {
                    int start_pos = 0;
                    int end_pos = 0;

                    String Date = NotFound;
                    String Place = NotFound;
                    String Video_Title = NotFound;
                    String Video_HomePage = NotFound;
                    String Duration = NotFound;
                    String Download_Location = NotFound;

                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(TED_DownloadPage_URL);
                    request.Method = WebRequestMethods.Http.Get;
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    String text = reader.ReadLine();

                    TED_Links.Columns.Add("Date", typeof(System.String));
                    TED_Links.Columns.Add("Place", typeof(System.String));
                    TED_Links.Columns.Add("Video_Title", typeof(System.String));
                    TED_Links.Columns.Add("Video_HomePage", typeof(System.String));
                    TED_Links.Columns.Add("Duration", typeof(System.String));
                    TED_Links.Columns.Add("Download_Location", typeof(System.String));

                    while (text != null)
                    {
                        if (text.Contains("<table class=\"downloads\">"))
                        {
                            for (int i = 0; i <= 7; i++)
                                text = reader.ReadLine();

                            while (!text.Contains("</table>"))
                            {
                                if (!text.Contains("\t<tr>"))
                                    throw InvalidTagException;

                                text = reader.ReadLine(); //this will read <td> e.g. date "\t\t<td>Oct 2011</td>"
                                if (text.Contains("\t\t<td>"))
                                {
                                    start_pos = text.IndexOf("\t\t<td>") + ("\t\t<td>").Length;
                                    end_pos = text.IndexOf("</td>");
                                    Date = text.Substring(start_pos, end_pos - start_pos);
                                }
                                else
                                    throw InvalidTagException;

                                text = reader.ReadLine(); //this will read <td> Place e.g. "\t\t<td>TEDGlobal 2011</td>"
                                if (text.Contains("\t\t<td>"))
                                {
                                    start_pos = text.IndexOf("\t\t<td>") + ("\t\t<td>").Length;
                                    end_pos = text.IndexOf("</td>");
                                    Place = text.Substring(start_pos, end_pos - start_pos);
                                }
                                else
                                    throw InvalidTagException;

                                text = reader.ReadLine(); //this will read the blank Space inbetween e.g. "\t\t<td>"

                                text = reader.ReadLine(); //this will read <td> Video Title  e.g. "\t\t\t<a href=\"/talks/todd_kuiken_a_prosthetic_arm_that_feels.html\">Todd Kuiken: A prosthetic arm that \"feels\"</a>\t\t</td>"
                                if (text.Contains("\t\t\t<a href=\""))
                                {
                                    start_pos = text.IndexOf("\t\t\t<a href=\"") + ("\t\t\t<a href=\"").Length;
                                    end_pos = text.IndexOf("\">");
                                    Video_HomePage = TED_HomePage_URL + text.Substring(start_pos, end_pos - start_pos);

                                    start_pos = end_pos + 2;
                                    end_pos = text.IndexOf("</a>\t\t</td>");
                                    Video_Title = text.Substring(start_pos, end_pos - start_pos);
                                }
                                else
                                    throw InvalidTagException;

                                text = reader.ReadLine(); //this will read <td> Duration  e.g. "\t\t<td>18:51</td>"
                                if (text.Contains("\t\t<td>"))
                                {
                                    start_pos = text.IndexOf("\t\t<td>") + ("\t\t<td>").Length;
                                    end_pos = text.IndexOf("</td>");
                                    Duration = text.Substring(start_pos, end_pos - start_pos);
                                }
                                else
                                    throw InvalidTagException;

                                text = reader.ReadLine(); //this will read <td> download location  e.g. "\t\t<td><a href=\"http://download.ted.com/talks/ToddKuiken_2011G-light.mp4\">Low</a> | <a href=\"http://download.ted.com/talks/ToddKuiken_2011G.mp4\">Regular</a> | <a href=\"http://download.ted.com/talks/ToddKuiken_2011G-480p.mp4\">High</a></td>"
                                if (text.Contains("\t\t<td>"))
                                {
                                    if (text.Contains("<td></td>"))
                                        Download_Location = NotFound;
                                    else
                                    {
                                        start_pos = text.IndexOf("\t\t<td><a href=\"") + ("\t\t<td><a href=\"").Length;
                                        end_pos = text.IndexOf("\">Low</a> | <a href=\"");
                                        if ((bool)TED_VidQual_Low.IsChecked)
                                            Download_Location = text.Substring(start_pos, end_pos - start_pos);

                                        start_pos = end_pos + ("\">Low</a> | <a href=\"").Length;
                                        end_pos = text.IndexOf("\">Regular</a> | <a href=\"");
                                        if ((bool)TED_VidQual_Reg.IsChecked)
                                            Download_Location = text.Substring(start_pos, end_pos - start_pos);

                                        start_pos = end_pos + ("\">Regular</a> | <a href=\"").Length;
                                        end_pos = text.IndexOf("\">High</a></td>");
                                        if ((bool)TED_VidQual_High.IsChecked)
                                            Download_Location = text.Substring(start_pos, end_pos - start_pos);
                                    }
                                }
                                else
                                    throw InvalidTagException;

                                text = reader.ReadLine(); //this will read </tr>  e.g. "\t</tr>"
                                if (!text.Contains("\t</tr>"))
                                    throw InvalidTagException;

                                TED_Links.Rows.Add(Date, Place, Video_Title, Video_HomePage, Duration, Download_Location);
                                TED_Videos_Count++;
                                TED_Progress_Bar_lbl.Content = "Generating List of Links to be Downloaded.  Link Added : " + TED_Videos_Count;

                                text = reader.ReadLine(); //this will read <tr> e.g."\t<tr>" and try to catch the </table> tag.
                            }
                        }
                        text = reader.ReadLine();
                    }
                    response.Close();
                    return true;
                }
                catch (System.Net.WebException Wex)
                {
                    MessageBox.Show(Wex.Message, "Connection Error!!");
                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Error_Text);
                    return false;
                }
            }

            private void TED_Download()
            {
                bool Result = true;
                try
                {
                    SimpleTEDDelegate TED_Initialisation = delegate()
                    {
                        TED_Progress_Bar_lbl.Content = "Download Inventory Created. Total Videos to be Downloaded = " + TED_Videos_Count;
                                        
                        ChangeStateto(false);
                        TED_Progress_Bar.Maximum = TED_Videos_Count;
                        TED_Progress_Bar.Value = 0.0;
                    };
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Send, TED_Initialisation);

                    for (int i = 0; i < TED_Videos_Count; i++)
                    {
                        DataRow TED_Row = TED_Links.Rows[i];
                        String DownloadLink = NotFound;
                        String DisplayText = String.Empty;

                        String TargetPath = DownloadFolderPathvar + @"\TED_" + Download_Quality + "\\" + TED_Row["Place"].ToString() + "_" + TED_Row["Date"].ToString();
                        String File_Name = TargetPath + "\\" + CreateProperName(TED_Row["Video_Title"].ToString()) + ".mp4";

                        if (!System.IO.Directory.Exists(TargetPath))
                            System.IO.Directory.CreateDirectory(TargetPath);

                        CreateTextFile(TED_Row, TargetPath);
                        DownloadLink = TED_Row["Download_Location"].ToString();
                        DisplayText = "Downloading Video - " + TED_Row["Video_Title"].ToString() + " (Video No.: " + (i + 1) + " )....";

                        SimpleTEDDelegate TED_Notification1 = delegate()
                        {
                            TED_Progress_Bar_lbl.Content = DisplayText;
                        };
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Send, TED_Notification1);

                        Result = Download(DownloadLink, File_Name);

                        SimpleTEDDelegate TED_Notification2 = delegate()
                        {
                            if (Result)
                                TED_Progress_Bar_lbl.Content += "Download Succesful.";
                            else
                                TED_Progress_Bar_lbl.Content += "Download Failed.";

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
                        TED_Progress_Bar_lbl.Content = "Fatal Error Encountered. Download Aborted. Please try again.";
                    };
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Send, TED_Termination);
                    MessageBox.Show(ex.Message, Error_Text);
                }
                finally
                {
                    SimpleTEDDelegate TED_Termination = delegate()
                    {
                        ChangeStateto(true);
                    };
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Send, TED_Termination);
                    TED_Videos_Count = 0;
                    TED_Links.Clear();
                    TED_Links.Columns.Clear();
                    isDownload_Active = false;
                }
            }

        #endregion

        #region Small Utility Functions

            private void InitialiseParameters()
            {
                DownloadFolderPathvar = DownloadFolderpath.Text;

                if ((bool)TED_VidQual_Low.IsChecked)
                    Download_Quality = "Low";
                if ((bool)TED_VidQual_Reg.IsChecked)
                    Download_Quality = "Regular";
                if ((bool)TED_VidQual_High.IsChecked)
                    Download_Quality = "High";
                //Shutdown_System = (bool)TED_ShutDowncb.IsChecked;

                SetProxy();
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
                System.Net.WebProxy pry = new System.Net.WebProxy("148.87.67.174", 80);
                pry.Credentials = CredentialCache.DefaultCredentials;
                WebRequest.DefaultWebProxy = pry;
            }

            private void ChangeStateto(bool State)
            {            
                TED_VidQual_Low.IsEnabled = State;
                TED_VidQual_Reg.IsEnabled = State;
                TED_VidQual_High.IsEnabled = State;
            }

            private void CreateTextFile(DataRow TED_Row, String TargetPath)
            {
                try
                {
                    String TED_Details_File = TargetPath + "\\" + CreateProperName(TED_Row["Video_Title"].ToString()) + ".txt";
                    StreamWriter TED_SW = new StreamWriter(TED_Details_File);

                    TED_SW.BaseStream.Seek(0, SeekOrigin.End);
                    TED_SW.WriteLine("Video Title : " + TED_Row["Video_Title"].ToString());
                    TED_SW.WriteLine("Video HomePage : " + TED_Row["Video_HomePage"].ToString());
                    TED_SW.WriteLine("Date : " + TED_Row["Date"].ToString());
                    TED_SW.WriteLine("Place : " + TED_Row["Place"].ToString());
                    TED_SW.WriteLine("Duration : " + TED_Row["Duration"].ToString());
                    TED_SW.WriteLine("Download Location : " + TED_Row["Download_Location"].ToString());

                    TED_SW.Flush();
                    TED_SW.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Error_Text);
                }
            }

            private bool Download(String url, String FileName)
            {
                try
                {
                    if (url.Equals(NotFound))
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

        #region Redundant Functions

        private void WhenLoaded()
        {
            //Duration duration = new Duration(TimeSpan.FromSeconds(5));
            //DoubleAnimation doubleanimation = new DoubleAnimation(200.0, duration);
            //TED_Progress_Bar.BeginAnimation(ProgressBar.ValueProperty, doubleanimation);            
        }

        #endregion                
    }
}
