using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;

namespace TEDinator.TEDClasses
{
    public class TED_Talk_Display : INotifyPropertyChanged
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Event_Id { get; set; }

        public string Event_Name { get; set; }

        public string Date { get; set; }

        public string Video_Homepage { get; set; }

        public bool IsAddedThroughBulk { get; set; }

        private bool status;
        public bool Status
        {
            get { return status; }
            set
            {
                status = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Status"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TED_Talk_Display() { }

        public TED_Talk_Display(TED_Talk_Display item)
        {
            Id = item.Id;
            Title = item.Title;
            Description = item.Description;
            Event_Id = item.Event_Id;
            Date = item.Date;
            Video_Homepage = item.Video_Homepage;
            Status = item.Status;
        }
    }

    public class TED_Talk
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Event_Id { get; set; }

        public string Duration { get; set; }

        public string Date { get; set; }

        public string Video_Homepage { get; set; }

        public TED_Media Video_Download_URL { get; set; }

        public TED_Talk() { }

        public TED_Talk(TED_Talk item)
        {
            Id = item.Id;
            Title = item.Title;
            Description = item.Description;
            Event_Id = item.Event_Id;
            Duration = item.Duration;
            Date = item.Date;
            Video_Homepage = item.Video_Homepage;
            Video_Download_URL = item.Video_Download_URL;
        }

        public TED_Talk(TED_Talk_Display item)
        {
            Id = item.Id;
            Title = item.Title;
            Description = item.Description;
            Event_Id = item.Event_Id;
            Date = item.Date;
            Video_Homepage = item.Video_Homepage;
        }
    }

    public class TED_Event
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string StartDate { get; set; }
    }

    public class TED_Language
    {
        public string Code { get; set; }

        public string Name { get; set; }
    }

    public class TED_Media
    {
        public string URL { get; set; }
        public string Size { get; set; }
        public string MIME_Type { get; set; }

        public TED_Media() { }

        public TED_Media(JToken TEDMediaJSON)
        {
            try
            {
                var url = TEDMediaJSON["uri"].ToString();
                long size = 0;
                long.TryParse(TEDMediaJSON["filesize_bytes"].ToString(), out size);
                var mime_type = TEDMediaJSON["mime_type"].ToString();

                URL = url;
                Size = FormatBytes(size);
                MIME_Type = mime_type;
            }
            catch (Exception) { }
        }

        private string FormatBytes(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }
    }
}