using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTestCanvas.Model;

namespace WPFTestCanvas.ViewModel
{
    public class Day
    {
        public DateTime Date { get; set; }
        public ObservableCollection<SchedulerEvent> Events { get; set; } = new();
    }
}
