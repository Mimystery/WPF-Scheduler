using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WPFTestCanvas.Model
{
    public class EventDayModel
    {
        public EventModel OriginalEvent { get; set; }
        public bool ShowTitle { get; set; }
        public Brush EventColor => (Brush)new BrushConverter().ConvertFromString(OriginalEvent.Color);
        public int Row { get; set; }

        public int Column { get; set; }      // Начальная колонка (0..3)
        public int ColumnSpan { get; set; }
    }
}
