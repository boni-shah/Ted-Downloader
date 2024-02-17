
using System;
using System.ComponentModel;
using System.Reflection;

namespace TEDinator.TEDClasses
{
    public static class Enums
    {
        public enum DownloadQuality
        {
            [Description("Low")]
            Low = 1,
            [Description("Medium")]
            Medium = 2,
            [Description("High")]
            High = 3
        }

        public enum RevisedDownloadQuality
        {
            [Description("podcast-light")]
            PodcastLight = 1,
            [Description("podcast-regular")]
            PodcastRegular = 2,
            [Description("podcast-high")]
            PodcastHigh = 3,


            [Description("64k")]
            SixtyFourK = 4,
            [Description("180k")]
            OneEightyK = 5,
            [Description("320k")]
            ThreeTwentyK = 6,
            [Description("450k")]
            FourFiftyK = 7,
            [Description("600k")]
            SixHundredK = 8,
            [Description("950k")]
            NineFiftyK = 9,
            //[Description("podcast-light")]
            //PodcastLight = 10,
            //[Description("podcast-regular")]
            //PodcastRegular = 11,
            //[Description("podcast-high")]
            //PodcastHigh = 12,
            [Description("audio-podcast")]
            AudioPodcast = 13,
            [Description("podcast-low-en")]
            PodcastLowEn = 14,
            [Description("podcast-high-en")]
            PodcastHighEn = 15
        }

        public enum APIRequestStatus
        {
            Successful = 1,
            Failed = 2
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}
