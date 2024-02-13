using System;

namespace TEDinator.TEDClasses
{
    public static class Constants
    {
        public const String TED_Homepage_URL = @"http://www.ted.com/";
        public const String TED_Talks_HomeURL = TED_Homepage_URL + "talks/";
        public const String TED_Downloadpage_URL = TED_Talks_HomeURL + "quick-list/";
        public const String TED_Paginated_URL = TED_Talks_HomeURL + "quick-list?sort=date&order=desc&page=";
        public const String TED_Subtitle_HomeURL = TED_Talks_HomeURL + "lang/";
        public const String ScenarioSolution_Homepage = @"http://obinshah.wordpress.com";
        public const String ScenarioSolution_TEDTagpage = @"http://obinshah.wordpress.com/category/my-utilities/ted-downloader/";

        public const String Low = "Low";
        public const String Regular = "Regular";
        public const String High = "High";

        public const String NotFound = "Not Found.";

        public const String HundredPercentDonemsg = "100 %";
        public const String ZeroPercentDonemsg = "0%";

        public const String OneInstanceAllowedmsg = "Only one Instance allowed. Application is already running.";
        public const String ExceptionErrormsg = "Invalid Tag Encountered. Please Restart Application.";
        public const String ErrormsgHeader = "Ouch!!! We just had a Boo-Boo!!";
        public const String UserErrormsg = "User Error!!";
        public const String DownloadErrormsg = "Please wait for the current download to finish!!";
        public const String ConnectionErrormsg = "Connection Error!! Please Check the Proxy Settings.";
        public const String ValidPathErrormsg = "Please Select a Valid Folder Path!!";
        public const String NoFileSelectedErrormsg = "No Video has been selected. Please select One or More Videos.";
        public const String SiteErrormsg = "There seems to be some problem with the Site. It has reported 0 Videos.Try running the app after some time.";
        public const String ErrorContactmsg = "If you are seeing this msg a lot often. Contact me and do let me know. I might be blissfully unaware of this issue.";
        public const String TermnotFoundErrormsg ="The Specified text was not found";
        public const String SingleItemErrormsg= "This is the only item present";

        public const String SlowProcessmsg = "Do note that this might make the Download Process a little slower.";
        public const String Analysingmsg = "Please wait. Getting a list of latest available videos from the TED Website...";

        public const String DownloadCompleted = "Download Completed";
        public const String DownloadCancelled = "Download Cancelled";
        public const String Success = "Success!!";
        public const String SubtitleDownloadFailed = "Subtitle Download Failed.\n";
        public const String SubtitleDownloadSuccesful = "Subtitle Download Succesful.\n";
        public const String SelectedLangSubtitleNotAvailable = "Subtitles not available for Selected language. Downloading English Subtitles.\n";
        public const String SubtitleNotAvailable = "Subtitles have not been created for this item.\n";
        public const String VideoNotAvailable = "Video not Available.\n";
        public const String VideoDownloadFailed = "Video Download Failed.\n";
        public const String VideoDownloadSuccesful = "Video Download Succesful.\n";
        public const String VideoSkippedmsg = "This Video has been skipped by the User.\n";

        public const String on0 = "Nothing to see here yet!!!";
        public const String between0_10 = "This is going to take some time. Don't hold your breath!!!";
        public const String between10_20 = "We are getting closer inch by inch!!! Have Heart, my dear friend.";
        public const String between20_30 = "You are thinking that if this was going any slower, it would be going backwards, right??";
        public const String between30_40 = "Patience, young padawan!!! Remember, Slow and Steady wins the Race.";
        public const String between40_50 = "We are approaching something great....can you feel it ??";
        public const String between50_60 = "Did you feel that ?? No, not the force!!! the halfway Mark, we just crossed it!!";
        public const String between60_70 = "Have Patience, Little One. We are getting there!!!";
        public const String between70_80 = "Patience my friend, is a virtue and you are doing good!! Kudos!!!";
        public const String between80_90 = "I am really amazed at the patience you have shown!!! You have learned well, Grasshopper...!!!";
        public const String between90_100 = "Just a bit more!!! Hang on, Mac!!!";

        public const String Welcome_msg = "They say that Sharing knowledge is the greatest of all callings and this is my contribution.\n\nHi,\nThis is your first time using this utility on this machine(or you have deleted the relevant registry keys), so you will have to fill the settings form that you will see next. Don't worry it's a one time thing only.\n\n And ya, thanks for using this app. Hope it is as useful to you as it is to me :).\n\n\nWarning: This computer program is protected by copyright law and international treaties. Unauthorized reproduction or distribution of this program, or any portion of it, may result in severe civil and criminal penalties, and will be prosecuted under the maximum extent possible under law.";
        public const String OpeningNotificationmsg = "Files already present will be skipped.\nOnly files which are downloaded will be mentioned here.\n\nbtw, Thanks for using this app :)";

        public const String AnalysisDonemsg = "Link analysis done. Total {0} Videos Found.";
        public const String DisplayCountLblmsg = "Videos Selected : {0} of {1}\n";
        public const String DownloadStartmsg = "Starting Download Process...\n";
        public const String DownloadingItemmsg = "Downloading item : {0} \n";

        public static string Progressbarmsg(int ProgressPercent)
        {
            if (ProgressPercent == 0)
                return on0;
            else if (ProgressPercent > 0 && ProgressPercent <= 10)
                return between0_10;
            else if (ProgressPercent > 10 && ProgressPercent <= 20)
                return between10_20;
            else if (ProgressPercent > 20 && ProgressPercent <= 30)
                return between20_30;
            else if (ProgressPercent > 30 && ProgressPercent <= 40)
                return between30_40;
            else if (ProgressPercent > 40 && ProgressPercent <= 50)
                return between40_50;
            else if (ProgressPercent > 50 && ProgressPercent <= 60)
                return between50_60;
            else if (ProgressPercent > 60 && ProgressPercent <= 70)
                return between60_70;
            else if (ProgressPercent > 70 && ProgressPercent <= 80)
                return between70_80;
            else if (ProgressPercent > 80 && ProgressPercent <= 90)
                return between80_90;
            else if (ProgressPercent > 90 && ProgressPercent < 100)
                return between90_100;

            return "Invalid Value Encountered"; ;
        }

    }
}
