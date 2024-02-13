using System.ComponentModel;

namespace TEDinator.TEDClasses
{
    public class TED_Video : INotifyPropertyChanged
    {
        public string Video_Title { get; set; }
        public string Event_Name { get; set; }
        public string Duration { get; set; }
        public string Date { get; set; }
        public string Video_Homepage { get; set; }
        public string Download_Location_low { get; set; }
        public string Download_Location_med { get; set; }
        public string Download_Location_high { get; set; }

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

        public bool IsAddedThroughBulk { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public TED_Video() { }

        public TED_Video(TED_Video item)
        {
            Video_Title = item.Video_Title;
            Event_Name = item.Event_Name;
            Duration = item.Duration;
            Date = item.Date;
            Video_Homepage = item.Video_Homepage;
            Download_Location_low = item.Download_Location_low;
            Download_Location_med = item.Download_Location_med;
            Download_Location_high = item.Download_Location_high;
            Status = item.Status;
            IsAddedThroughBulk = item.IsAddedThroughBulk;
        }
    }
}
