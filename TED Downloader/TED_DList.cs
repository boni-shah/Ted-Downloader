using System.ComponentModel;

namespace TED_Downloader
{
	public class TED_DList : INotifyPropertyChanged
	{
        public string Video_Title { get; set; }
		public string Event_Name{ get; set;}
        public string  Duration { get; set; }
        public string  Date { get; set; }
        public string  Video_HomePage { get; set; }
        public string Download_Location  { get; set; }
                
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
		
		public TED_DList()
		{
			// Insert code required on object creation below this point.
		}

        public TED_DList(TED_DList item)
        {
            Video_Title = item.Video_Title;
            Event_Name = item.Event_Name;
            Duration = item.Duration;
            Date = item.Date;
            Video_HomePage = item.Video_HomePage;
            Download_Location = item.Download_Location;
            Status = item.Status;
        }
	}
}