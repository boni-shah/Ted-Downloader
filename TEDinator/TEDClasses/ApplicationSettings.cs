using System;
using System.Net;

namespace TEDinator.TEDClasses
{
    public class ApplicationSettings
    {
        public String Download_Folder { set; get; }
        public int Download_Quality { set; get; }    //1=Low, 2=Medium, 3=High
        public IPAddress IPAddress { get; set; }
        public int IPPort { get; set; }
        //public String SubtitleLanguage { set; get; }
        public String LastRunDate { set; get; }
        public int RunCount { set; get; }
        public int ErrorCount { set; get; }

        public ApplicationSettings()
        {
            Download_Folder = Constants.NotFound;
            Download_Quality = 1;
            IPAddress = IPAddress.Parse("0.0.0.0");
            IPPort = 0;
            //SubtitleLanguage = "en";
            LastRunDate = DateTime.Today.ToShortDateString();
            RunCount = 0;
            ErrorCount = 0;
        }
    }
}
