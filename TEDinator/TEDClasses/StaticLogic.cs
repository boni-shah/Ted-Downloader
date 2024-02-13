using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace TEDinator.TEDClasses
{
    static class StaticLogic
    {
        public static ObservableCollection<TED_Video> TED_Analysis()
        {
            Exception InvalidTagException = new Exception(Constants.ExceptionErrormsg);
            WebClient Analyser_WC = null;
            MemoryStream stream = null;
            StreamReader reader = null;
            ObservableCollection<TED_Video> TEDLinks_All = null;

            try
            {
                TEDLinks_All = new ObservableCollection<TED_Video>();

                int start_pos = 0;
                int end_pos = 0;

                String Date = Constants.NotFound;
                String Event_Name = Constants.NotFound;
                String Video_Title = Constants.NotFound;
                String Video_HomePage = Constants.NotFound;
                String Duration = Constants.NotFound;
                String Download_Location_low = Constants.NotFound;
                String Download_Location_med = Constants.NotFound;
                String Download_Location_high = Constants.NotFound;

                Analyser_WC = new WebClient();
                Analyser_WC.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                byte[] byteArray = Encoding.Default.GetBytes(Analyser_WC.DownloadString(Constants.TED_Downloadpage_URL));
                stream = new MemoryStream(byteArray);
                reader = new StreamReader(stream);
                
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
                                start_pos = text.IndexOf("\t\t\t<a href=\"/") + ("\t\t\t<a href=\"/").Length;
                                end_pos = text.IndexOf("\">");
                                Video_HomePage = Constants.TED_Homepage_URL + text.Substring(start_pos, end_pos - start_pos);

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
                                if (text.Contains("<td></td>") || !text.Contains("Low") || !text.Contains("Regular") || !text.Contains("High"))
                                {
                                    Download_Location_low = Constants.NotFound;
                                    Download_Location_med = Constants.NotFound;
                                    Download_Location_high = Constants.NotFound;
                                }
                                else
                                {
                                    start_pos = text.IndexOf("\t\t<td><a href=\"") + ("\t\t<td><a href=\"").Length;
                                    end_pos = text.IndexOf("\">Low</a> | <a href=\"");
                                    Download_Location_low = text.Substring(start_pos, end_pos - start_pos);

                                    start_pos = end_pos + ("\">Low</a> | <a href=\"").Length;
                                    end_pos = text.IndexOf("\">Regular</a> | <a href=\"");
                                    Download_Location_med = text.Substring(start_pos, end_pos - start_pos);

                                    start_pos = end_pos + ("\">Regular</a> | <a href=\"").Length;
                                    end_pos = text.IndexOf("\">High</a></td>");
                                    Download_Location_high = text.Substring(start_pos, end_pos - start_pos);
                                }
                            }
                            else
                                throw InvalidTagException;

                            text = reader.ReadLine(); //this will read </tr>  e.g. "\t</tr>"
                            if (!text.Contains("\t</tr>"))
                                throw InvalidTagException;
                            TEDLinks_All.Add(new TED_Video { Date = Date, Video_Homepage = Video_HomePage, Duration = Duration, Download_Location_low = Download_Location_low, Download_Location_med = Download_Location_med, Download_Location_high = Download_Location_high, Video_Title = Video_Title, Event_Name = Event_Name, Status = false });
                            //TED_Links.Rows.Add(Date, Event_Name, Video_Title, Video_HomePage, Duration, Download_Location); 

                            text = reader.ReadLine(); //this will read <tr> e.g."\t<tr>" and try to catch the </table> tag.
                        }
                    }
                    text = reader.ReadLine();
                }

                if (TEDLinks_All.Count == 0)
                    MessageBox.Show(Constants.SiteErrormsg + "\n\n" + Constants.ErrorContactmsg, Constants.ErrormsgHeader);

                Analyser_WC.Dispose();
                stream.Dispose();
                reader.Dispose();
            }
            catch (System.Net.WebException Wex)
            {
                MessageBox.Show(Wex.Message, Constants.ErrormsgHeader);
                return new ObservableCollection<TED_Video>();
            }
            catch (Exception)
            {
                MessageBox.Show(Constants.ErrorContactmsg, Constants.ErrormsgHeader);
                return new ObservableCollection<TED_Video>();
            }
            return TEDLinks_All;
        }
    }
}
