using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace TEDinator.TEDClasses
{
    static class RevisedStaticLogic
    {
        static string Totalstr = "\tShowing <span class=\"notranslate\">1 - 100</span> of  <span class=\"notranslate\">";
        static int TotalReqdCount = 1;
        public const int ERROR_THRESHOLD = 5;

        private static int ReturnTotalCount(string text)
        {
            int start_pos = 0;
            int end_pos = 0;
            int divider = 100;

            var remainderstr = "\tShowing <span class=\"notranslate\">1 - ";

            start_pos = text.IndexOf(remainderstr) + remainderstr.Length;
            end_pos = text.IndexOf("</span>");
            divider = Convert.ToInt32(text.Substring(start_pos, end_pos - start_pos));

            start_pos = text.IndexOf(Totalstr) + Totalstr.Length;
            end_pos = text.LastIndexOf("</span>");
            TotalReqdCount = Convert.ToInt32(text.Substring(start_pos, end_pos - start_pos));

            if (TotalReqdCount % divider > 0)
                TotalReqdCount = TotalReqdCount / divider + 1;
            else
                TotalReqdCount = TotalReqdCount / divider;

            return TotalReqdCount;
        }

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

                int CurrentRunCount = 0;
                String Date = Constants.NotFound;
                String Event_Name = Constants.NotFound;
                String Video_Title = Constants.NotFound;
                String Video_HomePage = Constants.NotFound;
                String Duration = Constants.NotFound;
                String Download_Location_low = Constants.NotFound;
                String Download_Location_med = Constants.NotFound;
                String Download_Location_high = Constants.NotFound;

                while (CurrentRunCount < TotalReqdCount)
                {
                    CurrentRunCount++;
                    Analyser_WC = new WebClient();
                    Analyser_WC.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                    var str = Analyser_WC.DownloadString(Constants.TED_Paginated_URL + CurrentRunCount);
                    byte[] byteArray = Encoding.Default.GetBytes(str);
                    //byte[] byteArray = Encoding.Default.GetBytes(Analyser_WC.DownloadString(Constants.TED_Downloadpage_URL));
                    stream = new MemoryStream(byteArray);
                    reader = new StreamReader(stream);

                    String text = reader.ReadLine();
                    while (text != null)
                    {
                        if (CurrentRunCount == 1 && text.Contains(Totalstr))
                        {
                            TotalReqdCount = ReturnTotalCount(text);
                        }

                        if (text.Contains("<table class=\"downloads notranslate\">"))
                        {
                            for (int i = 0; i <= 7; i++) text = reader.ReadLine();

                            while (!text.Contains("</table>"))
                            {
                                try
                                {
                                    if (!text.Contains("\t<tr>")) throw InvalidTagException;

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

                                    if (!TEDLinks_All.Any(x => x.Video_Title == Video_Title && x.Date == Date && x.Event_Name == Event_Name))
                                        TEDLinks_All.Add(new TED_Video { Date = Date, Video_Homepage = Video_HomePage, Duration = Duration, Download_Location_low = Download_Location_low, Download_Location_med = Download_Location_med, Download_Location_high = Download_Location_high, Video_Title = Video_Title, Event_Name = Event_Name, Status = false });
                                    //TED_Links.Rows.Add(Date, Event_Name, Video_Title, Video_HomePage, Duration, Download_Location); 
                                }
                                catch (Exception) { int i = 0; while (!text.Contains("\t</tr>") && i < 20) { text = reader.ReadLine(); i++; } }
                                text = reader.ReadLine(); //this will read <tr> e.g."\t<tr>" and try to catch the </table> tag.
                            }
                        }
                        text = reader.ReadLine();
                    }

                    Analyser_WC.Dispose();
                    stream.Dispose();
                    reader.Dispose();
                }
                if (TEDLinks_All.Count == 0) MessageBox.Show(Constants.SiteErrormsg + "\n\n" + Constants.ErrorContactmsg, Constants.ErrormsgHeader);
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

        public static ObservableCollection<TED_Talk_Display> TED_Analyse_Talks(bool logEverything, ref BackgroundWorker TED_Analyser_Worker)
        {
            var totalTalksCount = 0;
            var currentCount = -1;
            var lastCurrentCount = 0;
            var errorCount = 0;

            var TED_Events = TED_Get_All_Events(logEverything);

            ObservableCollection<TED_Talk_Display> TEDLinks_All = new ObservableCollection<TED_Talk_Display>();

            StringBuilder sb = new StringBuilder();

            using (WebClient client = new WebClient())
            {
                var talksInResponseCount = 0;

                while (((currentCount + 1) <= totalTalksCount) && (lastCurrentCount != currentCount))
                {
                    try
                    {
                        var Get_All_Talks_URL = string.Format(Constants.TED_API_Get_All_Talks_Parametrised_URL, Constants.API_Key, Constants.Search_Limit, (currentCount + 1));
                        var response = client.DownloadString(Get_All_Talks_URL);

                        JObject joResponse = JObject.Parse(response);

                        int.TryParse(joResponse["counts"]["total"].ToString(), out totalTalksCount);
                        int.TryParse(joResponse["counts"]["this"].ToString(), out talksInResponseCount);

                        var talksJson = joResponse["talks"];
                        for (var i = 0; i < talksInResponseCount; i++)
                        {
                            var talk = talksJson[i]["talk"];

                            var date = new DateTime();
                            DateTime.TryParse(talk["recorded_at"].ToString(), out date);
                            var event_id = talk["event_id"].ToString();
                            var event_name = TED_Events.SingleOrDefault(x => x.Id.Equals(event_id)).Name;

                            TEDLinks_All.Add(new TED_Talk_Display
                            {
                                Id = talk["id"].ToString(),
                                Title = talk["name"].ToString(),
                                Description = talk["description"].ToString(),
                                Date = date != DateTime.MinValue ? date.Date.ToString("MMM dd, yyyy") : "N/A",
                                //Date = talk["recorded_at"].ToString().Split(' ')[0],
                                Event_Id = event_id,
                                Event_Name = event_name,
                                Video_Homepage = ""
                            });
                            sb.Append(string.Format("Id: {0} \nName: {1}", talk["id"].ToString(), talk["name"].ToString()));
                        }

                        lastCurrentCount = currentCount;
                        currentCount += talksInResponseCount;

                        try
                        {
                            var percerntage = (int)(0.5f + ((100f * currentCount) / totalTalksCount));
                            TED_Analyser_Worker.ReportProgress(percerntage);
                        }
                        catch (Exception e) { sb.Append("This was a big mistake - " + e.Message); }

                        sb.Append(string.Format("\n\nTotal Count: {0} Current Count: {1} talksInResponseCount : {2}\n\n", totalTalksCount, currentCount, talksInResponseCount));
                    }
                    catch (Exception e)
                    {
                        errorCount++;
                        sb.Append("This was a big mistake - " + e.Message);

                        if (errorCount >= ERROR_THRESHOLD)
                            break;
                    }
                }

                //File.AppendAllText("log.txt", sb.ToString());
                sb.Clear();
            }

            return TEDLinks_All;

        }

        public static List<TED_Event> TED_Get_All_Events(bool logEverything)
        {
            var totalEventsCount = 0;
            var currentCount = -1;
            var lastCurrentCount = 0;
            var errorCount = 0;

            var TEDEvents_All = new List<TED_Event>();

            StringBuilder sb = new StringBuilder();

            using (WebClient client = new WebClient())
            {
                var eventsInResponseCount = 0;

                while (((currentCount + 1) <= totalEventsCount) && (lastCurrentCount != currentCount))
                {
                    try
                    {
                        var Get_All_Talks_URL = string.Format(Constants.TED_API_Get_All_Events_Parametrised_URL, Constants.API_Key, Constants.Search_Limit, (currentCount + 1));
                        var response = client.DownloadString(Get_All_Talks_URL);

                        JObject joResponse = JObject.Parse(response);

                        int.TryParse(joResponse["counts"]["total"].ToString(), out totalEventsCount);
                        int.TryParse(joResponse["counts"]["this"].ToString(), out eventsInResponseCount);

                        var eventsJson = joResponse["events"];
                        for (var i = 0; i < eventsInResponseCount; i++)
                        {
                            var ted_Event = eventsJson[i]["event"];

                            //var date = new DateTime();
                            //DateTime.TryParse(ted_Event["recorded_at"].ToString(), out date);

                            TEDEvents_All.Add(new TED_Event
                            {
                                Id = ted_Event["id"].ToString(),
                                Name = ted_Event["name"].ToString(),
                                Description = ted_Event["description"].ToString(),
                                //StartDate = date != DateTime.MinValue ? date.Date.ToString() : "N/A"
                                StartDate = ted_Event["starts_at"].ToString().Split(' ')[0]
                            });
                            sb.Append(string.Format("Id: {0} \nName: {1}", ted_Event["id"].ToString(), ted_Event["name"].ToString()));
                        }

                        lastCurrentCount = currentCount;
                        currentCount += eventsInResponseCount;


                        sb.Append(string.Format("\n\nTotal Count: {0} Current Count: {1} eventsInResponseCount : {2}\n\n", totalEventsCount, currentCount, eventsInResponseCount));
                    }
                    catch (Exception e)
                    {
                        errorCount++;
                        sb.Append("This was a big mistake - " + e.Message);

                        if (errorCount >= ERROR_THRESHOLD)
                            break;
                    }
                }

                //File.AppendAllText("log.txt", sb.ToString());
                sb.Clear();
            }

            return TEDEvents_All;

        }
                
        public static TED_Media TED_Get_Media_URL_From_Talk(string id, int downloadQuality, bool logEverything, ref bool isSuccessful)
        {
            TED_Media mediaObj = new TED_Media();

            using (WebClient client = new WebClient())
            {
                try
                {
                    var Get_Talk_URL = string.Format(Constants.TED_API_Get_Talk_URL, id, Constants.API_Key);
                    var response = client.DownloadString(Get_Talk_URL);
                    JObject joResponse = JObject.Parse(response);

                    var media = joResponse["talk"]["media"]["internal"];
                    mediaObj = new TED_Media(media[Enums.GetEnumDescription( (Enums.RevisedDownloadQuality)downloadQuality)]);
                }
                catch (Exception)
                {
                    isSuccessful = false;
                }
            }

            return mediaObj;
        }
    }
}