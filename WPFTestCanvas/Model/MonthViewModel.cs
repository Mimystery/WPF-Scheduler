using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTestCanvas.Model
{
    public class MonthViewModel
    {
        public string Name { get; set; }  // Название месяца (Январь, Февраль и т.д.)
        // Существующая коллекция для ежедневных ивентов по неделям (может использоваться для другой логики)
        public ObservableCollection<ObservableCollection<EventDayModel>> WeeklyEvents { get; set; } = new();

        // Новая коллекция для отображения "полосок" ивентов в месячном календаре
        public ObservableCollection<EventDayModel> MultiWeekEvents { get; set; } = new();

        public MonthViewModel(string name)
        {
            Name = name;
            // Инициализация 4 колонок для WeeklyEvents (если нужно)
            for (int i = 0; i < 4; i++)
            {
                WeeklyEvents.Add(new ObservableCollection<EventDayModel>());
            }
        }
    }
}
