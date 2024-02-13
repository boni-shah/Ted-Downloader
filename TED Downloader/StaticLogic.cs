using System;
using System.Net;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace TED_Downloader
{
    class StaticLogic
    {
        public static bool TED_Analysis(ObservableCollection<TED_DList> TEDLinks_All, String Download_Quality)
        {                        
            Exception InvalidTagException = new Exception(Constants.Exception_Error_Text);

            try
            {
                TEDLinks_All.Clear();

                int start_pos = 0;
                int end_pos = 0;

                String Date = Constants.NotFound;
                String Event_Name = Constants.NotFound;
                String Video_Title = Constants.NotFound;
                String Video_HomePage = Constants.NotFound;
                String Duration = Constants.NotFound;
                String Download_Location = Constants.NotFound;

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Constants.TED_DownloadPage_URL);
                request.Method = WebRequestMethods.Http.Get;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader reader = new StreamReader(response.GetResponseStream());
                String text = reader.ReadLine();

                while (text != null)
                {
                    if (text.Contains("<table class=\"downloads notranslate\">"))
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

                            text = reader.ReadLine(); //this will read <td> Event_Name e.g. "\t\t<td>TEDGlobal 2011</td>"
                            if (text.Contains("\t\t<td>"))
                            {
                                start_pos = text.IndexOf("\t\t<td>") + ("\t\t<td>").Length;
                                end_pos = text.IndexOf("</td>");
                                Event_Name = text.Substring(start_pos, end_pos - start_pos);
                            }
                            else
                                throw InvalidTagException;

                            text = reader.ReadLine(); //this will read the blank Space inbetween e.g. "\t\t<td>"

                            text = reader.ReadLine(); //this will read <td> Video Title  e.g. "\t\t\t<a href=\"/talks/todd_kuiken_a_prosthetic_arm_that_feels.html\">Todd Kuiken: A prosthetic arm that \"feels\"</a>\t\t</td>"
                            if (text.Contains("\t\t\t<a href=\""))
                            {
                                start_pos = text.IndexOf("\t\t\t<a href=\"") + ("\t\t\t<a href=\"").Length;
                                end_pos = text.IndexOf("\">");
                                Video_HomePage = Constants.TED_HomePage_URL + text.Substring(start_pos, end_pos - start_pos);

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
                                    Download_Location = Constants.NotFound;
                                else
                                {
                                    start_pos = text.IndexOf("\t\t<td><a href=\"") + ("\t\t<td><a href=\"").Length;
                                    end_pos = text.IndexOf("\">Low</a> | <a href=\"");
                                    if (Download_Quality.Equals(Constants.Low))
                                        Download_Location = text.Substring(start_pos, end_pos - start_pos);

                                    start_pos = end_pos + ("\">Low</a> | <a href=\"").Length;
                                    end_pos = text.IndexOf("\">Regular</a> | <a href=\"");
                                    if (Download_Quality.Equals(Constants.Regular))
                                        Download_Location = text.Substring(start_pos, end_pos - start_pos);

                                    start_pos = end_pos + ("\">Regular</a> | <a href=\"").Length;
                                    end_pos = text.IndexOf("\">High</a></td>");
                                    if (Download_Quality.Equals(Constants.High))
                                        Download_Location = text.Substring(start_pos, end_pos - start_pos);
                                }
                            }
                            else
                                throw InvalidTagException;

                            text = reader.ReadLine(); //this will read </tr>  e.g. "\t</tr>"
                            if (!text.Contains("\t</tr>"))
                                throw InvalidTagException;
                            TEDLinks_All.Add(new TED_DList { Date= Date, Video_HomePage= Video_HomePage, Duration= Duration, Download_Location= Download_Location, Video_Title = Video_Title, Event_Name = Event_Name, Status = false });
                            //TED_Links.Rows.Add(Date, Event_Name, Video_Title, Video_HomePage, Duration, Download_Location);                           

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
                throw Wex;                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,Constants.Error_Text);
                return false;
            }
        }

    }
}
