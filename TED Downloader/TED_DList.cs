using System.ComponentModel;

namespace TED_Downloader
{
	public class TED_DList : INotifyPropertyChanged
	{		
		public string Title{ get; set;}
		public string Event_Name{ get; set;}

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
	}
}