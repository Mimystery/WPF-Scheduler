using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTestCanvas.Model
{
    public class DayModel
    {
        public int Day { get; set; }
        public DateTime Date { get; set; }
        public ObservableCollection<EventDayModel> Events { get; set; } = new ObservableCollection<EventDayModel>();
        public bool IsToday => Date.Date == DateTime.Today.Date;
        public bool HasMoreEvents => Events.Count > 5;
    }
}
