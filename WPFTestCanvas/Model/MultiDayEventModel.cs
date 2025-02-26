using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTestCanvas.Model
{
    public class MultiDayEventModel
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int StartColumn { get; set; }
        public int ColumnSpan { get; set; }
        public string EventColor { get; set; }
    }
}
