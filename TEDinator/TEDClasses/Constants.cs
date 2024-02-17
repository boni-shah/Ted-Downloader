namespace TEDinator.TEDClasses
{
    public static class Constants
    {
        public const string API_Key = "nkhkpxs6aspa5c73rnmc2vf7";

        public const string TED_Homepage_URL = @"http://www.ted.com/";
        public const string TED_Talks_HomeURL = TED_Homepage_URL + "talks/";
        public const string TED_Downloadpage_URL = TED_Talks_HomeURL + "quick-list/";
        public const string TED_Paginated_URL = TED_Talks_HomeURL + "quick-list?page=";
        public const string TED_Subtitle_HomeURL = TED_Talks_HomeURL + "lang/";
        public const string ScenarioSolution_Homepage = @"http://obinshah.wordpress.com";
        public const string ScenarioSolution_TEDTagpage = @"http://obinshah.wordpress.com/category/my-utilities/ted-downloader/";

        public const string TED_API_Base_URL = @"https://api.ted.com/";
        public const string Version_For_API = @"v1/";

        public const string TED_API_Get_All_Talks_URL = TED_API_Base_URL + Version_For_API + "talks.json?api-key={0}";
        public const string TED_API_Get_All_Talks_Parametrised_URL = TED_API_Base_URL + Version_For_API + "talks.json?api-key={0}&limit={1}&order=created_at:desc&offset={2}";
        public const string TED_API_Get_Talk_URL = TED_API_Base_URL + Version_For_API + "talks/{0}.json?api-key={1}";

        public const string TED_API_Get_All_Languages_URL = TED_API_Base_URL + Version_For_API + "languages.json?api-key={0}";
        public const string TED_API_Get_All_Languages_Parametrised_URL = TED_API_Base_URL + Version_For_API + "languages.json?api-key={0}&limit={1}&offset={2}";
        public const string TED_API_Get_Language_URL = TED_API_Base_URL + Version_For_API + "languages/{0}.json?api-key={1}";

        public const string TED_API_Get_All_Events_URL = TED_API_Base_URL + Version_For_API + "events.json?api-key={0}";
        public const string TED_API_Get_All_Events_Parametrised_URL = TED_API_Base_URL + Version_For_API + "events.json?api-key={0}&limit={1}&offset={2}";
        public const string TED_API_Get_Event_URL = TED_API_Base_URL + Version_For_API + "events/{0}.json?api-key={1}";

        public const string NotFound = "Not Found.";

        public const string HundredPercentDonemsg = "100 %";
        public const string ZeroPercentDonemsg = "0%";

        public const string OneInstanceAllowedmsg = "Only one Instance allowed. Application is already running.";
        public const string ExceptionErrormsg = "Invalid Tag Encountered. Please restart application.";
        public const string ErrormsgHeader = "Ouch!!! We just had a Boo-Boo!!";
        public const string UserErrormsg = "User Error!!";
        public const string DownloadErrormsg = "Please wait for the current download to finish!!";
        public const string ConnectionErrormsg = "Connection Error!! Please check the proxy settings.";
        public const string ValidPathErrormsg = "Please select a valid folder path!!";
        public const string NoFileSelectedErrormsg = "No video has been selected. Please select one or more videos.";
        public const string SiteErrormsg = "There seems to be some problem with the Site. It has reported 0 Videos.Try running the app after some time.";
        public const string ErrorContactmsg = "If you are seeing this message a lot often. Contact me and do let me know. I might be blissfully unaware of this issue.";
        public const string TermnotFoundErrormsg = "The specified text was not found";
        public const string SingleItemErrormsg = "This is the only item present";

        public const string SlowProcessmsg = "Do note that this might make the download process a little slower.";
        public const string Analysingmsg = "Please wait. Getting a list of latest available videos from the TED Website...";

        public const string DownloadCompleted = "Download Completed";
        public const string DownloadCancelled = "Download Cancelled";
        public const string Success = "Success!!";
        public const string SubtitleDownloadFailed = "Subtitle Download Failed.\n";
        public const string SubtitleDownloadSuccesful = "Subtitle Download Successful.\n";
        public const string SelectedLangSubtitleNotAvailable = "Subtitles not available for Selected language. Downloading English Subtitles.\n";
        public const string SubtitleNotAvailable = "Subtitles have not been created for this item.\n";
        public const string VideoNotAvailable = "This video is not available for the selected video quality.\n";
        public const string VideoDownloadFailed = "Video download failed. Reason - {0}\n";
        public const string VideoDownloadSuccesful = "Video download successful.\n";
        public const string VideoSkippedmsg = "This video has been skipped by the user.\n";

        public const string on0 = "Nothing to see here yet!!!";
        public const string between0_10 = "This is going to take some time. Don't hold your breath!!!";
        public const string between10_20 = "We are getting closer inch by inch!!! Have Heart, my dear friend.";
        public const string between20_30 = "You are thinking that if this was going any slower, it would be going backwards, right??";
        public const string between30_40 = "Patience, young padawan!!! Remember, Slow and Steady wins the Race.";
        public const string between40_50 = "We are approaching something great....can you feel it ??";
        public const string between50_60 = "Did you feel that ?? No, not the force!!! the halfway Mark, we just crossed it!!";
        public const string between60_70 = "Have Patience, Little One. We are getting there!!!";
        public const string between70_80 = "Patience my friend, is a virtue and you are doing good!! Kudos!!!";
        public const string between80_90 = "I am really amazed at the patience you have shown!!! You have learned well, Grasshopper...!!!";
        public const string between90_100 = "Just a bit more!!! Hang on, Mac!!!";

        public const string Welcome_msg = "They say that Sharing knowledge is the greatest of all callings and this is my contribution.\n\nHi,\nThis is your first time using this utility on this machine(or you have deleted the relevant registry keys), so you will have to fill the settings form that you will see next. Don't worry it's a one time thing only.\n\n And ya, thanks for using this app. Hope it is as useful to you as it is to me :).\n\n\nWarning: This computer program is protected by copyright law and international treaties. Unauthorized reproduction or distribution of this program, or any portion of it, may result in severe civil and criminal penalties, and will be prosecuted under the maximum extent possible under law.";
        public const string OpeningNotificationmsg = "Files already present will be skipped.\nOnly files which are downloaded will be mentioned here.\n\nbtw, Thanks for using this app :)";

        public const string AnalysisDonemsg = "Link analysis done. Total {0} Videos Found.";
        public const string DisplayCountLblmsg = "Videos Selected : {0} of {1}\n";
        public const string DownloadStartmsg = "Starting Download Process...\n";
        public const string DownloadingItemmsg = "Downloading item : {0} (size : {1}) \n";
        public const string Sizemsg = "Size : {0} \n";

        public const string Search_Limit = "100";

        public static string Progressbarmsg(int ProgressPercent)
        {
            if (ProgressPercent == 0)
                return on0;
            if (ProgressPercent > 0 && ProgressPercent <= 10)
                return between0_10;
            if (ProgressPercent > 10 && ProgressPercent <= 20)
                return between10_20;
            if (ProgressPercent > 20 && ProgressPercent <= 30)
                return between20_30;
            if (ProgressPercent > 30 && ProgressPercent <= 40)
                return between30_40;
            if (ProgressPercent > 40 && ProgressPercent <= 50)
                return between40_50;
            if (ProgressPercent > 50 && ProgressPercent <= 60)
                return between50_60;
            if (ProgressPercent > 60 && ProgressPercent <= 70)
                return between60_70;
            if (ProgressPercent > 70 && ProgressPercent <= 80)
                return between70_80;
            if (ProgressPercent > 80 && ProgressPercent <= 90)
                return between80_90;
            if (ProgressPercent > 90 && ProgressPercent < 100)
                return between90_100;

            return "Invalid Value Encountered";
        }

        public static string FormatSizeToString(double len)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            var order = 0;
            while (len >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                len = len / 1024;
            }
            return string.Format("{0:0.##} {1}", len, sizes[order]);
        }

    }
}
