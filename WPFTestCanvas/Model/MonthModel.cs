using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTestCanvas.Model
{
    public class MonthModel
    {
        public string Name { get; set; }
        public List<EventModel> Events { get; set; }
        public ObservableCollection<string> Months { get; set; } = new ObservableCollection<string>
        {
            "Январь", "Февраль", "Март", "Апрель",
            "Май", "Июнь", "Июль", "Август",
            "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"
        };
    }

}
