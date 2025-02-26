using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFTestCanvas.Model;
using WPFTestCanvas.View;

namespace WPFTestCanvas.ViewModel
{
    public class SchedulerViewModel : INotifyPropertyChanged
    {
        private DateTime _currentDate;

        // Новый год для месячного календаря
        private int _currentMonthlyYear = DateTime.Now.Year;
        public int CurrentMonthlyYear
        {
            get => _currentMonthlyYear;
            set
            {
                if (_currentMonthlyYear != value)
                {
                    _currentMonthlyYear = value;
                    OnPropertyChanged(nameof(CurrentMonthlyYear));
                    // При смене года для месячного календаря пересчитываем месяцы
                    UpdateMonthsWithEvents();
                }
            }
        }

        public string CurrentMonth => _currentDate.ToString("MMMM yyyy", CultureInfo.CurrentCulture);

        public ObservableCollection<DayModel> Days { get; set; }
        public ObservableCollection<EventModel> Events { get; set; }
        public ObservableCollection<string> Months { get; set; } = new ObservableCollection<string>
        {
            "Январь", "Февраль", "Март", "Апрель",
            "Май", "Июнь", "Июль", "Август",
            "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"
        };
        public ObservableCollection<MonthViewModel> MonthsWithEvents { get; set; } = new();

        public ICommand PreviousMonthCommand { get; }
        public ICommand NextMonthCommand { get; }
        public ICommand AddEventCommand { get; }
        public ICommand ShowDayEventsCommand { get; }
        public ICommand PreviousYearCommand { get; }
        public ICommand NextYearCommand { get; }
        public ICommand ShowMonthEventsCommand { get; }


        public event PropertyChangedEventHandler PropertyChanged;
        public SchedulerViewModel()
        {
            _currentDate = DateTime.Today;
            Days = new ObservableCollection<DayModel>();
            Events = new ObservableCollection<EventModel>();

            PreviousMonthCommand = new RelayCommand(_ => ChangeMonth(-1));
            NextMonthCommand = new RelayCommand(_ => ChangeMonth(1));
            AddEventCommand = new RelayCommand(_ => AddEvent());
            ShowDayEventsCommand = new RelayCommand(ShowDayEvents);

            PreviousYearCommand = new RelayCommand(_ => CurrentMonthlyYear--);
            NextYearCommand = new RelayCommand(_ => CurrentMonthlyYear++);

            ShowMonthEventsCommand = new RelayCommand(OpenMonthEventsWindow);

            UpdateDays();
            UpdateMonthsWithEvents();
        }

        private void OpenMonthEventsWindow(object parameter)
        {
            // Ожидаем, что параметр – это MonthViewModel
            if (parameter is MonthViewModel month)
            {
                MonthEventsListWindow window = new MonthEventsListWindow(month, this);
                window.ShowDialog();
            }
        }

        private void UpdateMonthsWithEvents()
        {
            // Пересоздаем коллекцию месяцев (12 штук)
            MonthsWithEvents.Clear();
            for (int i = 0; i < 12; i++)
            {
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i + 1);
                MonthsWithEvents.Add(new MonthViewModel(monthName));
            }

            // Отбираем события, активные в выбранном году
            foreach (var ev in Events.Where(e => e.StartDate.Year <= CurrentMonthlyYear && e.EndDate.Year >= CurrentMonthlyYear))
            {
                // Для текущего месяца определяем эффективное начало и конец события
                int effectiveMonthStart = (ev.StartDate.Year == CurrentMonthlyYear) ? ev.StartDate.Month : 1;
                int effectiveMonthEnd = (ev.EndDate.Year == CurrentMonthlyYear) ? ev.EndDate.Month : 12;

                for (int m = effectiveMonthStart; m <= effectiveMonthEnd; m++)
                {
                    DateTime monthStart = new DateTime(CurrentMonthlyYear, m, 1);
                    int daysInMonth = DateTime.DaysInMonth(CurrentMonthlyYear, m);
                    DateTime monthEnd = new DateTime(CurrentMonthlyYear, m, daysInMonth);

                    // Если событие началось до выбранного года, effectiveStart – начало месяца
                    DateTime effectiveStart = ev.StartDate < monthStart ? monthStart : ev.StartDate;
                    // Если событие заканчивается после выбранного года, effectiveEnd – конец месяца
                    DateTime effectiveEnd = ev.EndDate > monthEnd ? monthEnd : ev.EndDate;

                    int minWeek = (effectiveStart.Day - 1) / 7;
                    int maxWeek = (effectiveEnd.Day - 1) / 7;
                    int span = maxWeek - minWeek + 1;

                    var eventModel = new EventDayModel
                    {
                        OriginalEvent = ev,
                        ShowTitle = true,
                        Row = 0, // распределим ниже
                        Column = minWeek,
                        ColumnSpan = span
                    };

                    MonthsWithEvents[m - 1].MultiWeekEvents.Add(eventModel);
                }
            }

            // Распределяем полоски по строкам, чтобы они не наслаивались
            DistributeMultiWeekEventRows();

            OnPropertyChanged(nameof(MonthsWithEvents));
        }

        private void DistributeMultiWeekEventRows()
        {
            foreach (var month in MonthsWithEvents)
            {
                // Создадим список для каждого строки, где будем хранить колонные интервалы уже размещённых ивентов
                List<List<(int start, int end)>> rows = new List<List<(int, int)>>();

                // Сортируем ивенты по Column (или по дате начала)
                var events = month.MultiWeekEvents.OrderBy(e => e.Column).ThenBy(e => e.OriginalEvent.StartDate).ToList();
                foreach (var ev in events)
                {
                    int evStart = ev.Column;
                    int evEnd = ev.Column + ev.ColumnSpan - 1;

                    int assignedRow = 0;
                    bool placed = false;
                    while (!placed)
                    {
                        if (rows.Count <= assignedRow)
                        {
                            // Если строки ещё нет, создаём новую
                            rows.Add(new List<(int, int)>());
                            ev.Row = assignedRow;
                            rows[assignedRow].Add((evStart, evEnd));
                            placed = true;
                        }
                        else
                        {
                            // Проверяем, не пересекается ли этот ивент с уже размещёнными в этой строке
                            bool conflict = rows[assignedRow].Any(r => evStart <= r.end && r.start <= evEnd);
                            if (!conflict)
                            {
                                ev.Row = assignedRow;
                                rows[assignedRow].Add((evStart, evEnd));
                                placed = true;
                            }
                            else
                            {
                                assignedRow++;
                            }
                        }
                    }
                }
            }
        }

        private List<int> GetEventWeeks(EventModel ev)
        {
            List<int> weeks = new List<int>();
            DateTime current = ev.StartDate;

            while (current <= ev.EndDate)
            {
                int weekNumber = (current.Day - 1) / 7; // Определяем номер недели (0 - первая, 1 - вторая и т.д.)
                if (!weeks.Contains(weekNumber))
                {
                    weeks.Add(weekNumber);
                }
                current = current.AddDays(1);
            }

            return weeks;
        }

        

        private void ShowDayEvents(object parameter)
        {
            if (parameter is DayModel day && day.Events.Count > 5)
            {
                EventsListWindow window = new EventsListWindow(day, this);
                window.ShowDialog();
            }
        }

        private void ChangeMonth(int offset)
        {
            _currentDate = _currentDate.AddMonths(offset);
            OnPropertyChanged(nameof(CurrentMonth));
            UpdateDays();
        }

        private void UpdateDays()
        {
            Days.Clear();

            int daysInMonth = DateTime.DaysInMonth(_currentDate.Year, _currentDate.Month);
            DateTime firstDayOfMonth = new DateTime(_currentDate.Year, _currentDate.Month, 1);
            int dayOfWeek = (int)firstDayOfMonth.DayOfWeek;

            // Если первый день месяца — это воскресенье, сдвигаем на следующий день (для выравнивания с понедельника)
            if (dayOfWeek == 0)
            {
                dayOfWeek = 7;
            }

            // Добавляем пустые дни для выравнивания с первым днем недели
            for (int i = 0; i < dayOfWeek - 1; i++)
            {
                Days.Add(null);  // Эти элементы должны быть скрыты с помощью конвертера
            }

            // Добавляем все реальные дни месяца
            for (int i = 1; i <= daysInMonth; i++)
            {
                var day = new DayModel
                {
                    Day = i,
                    Date = new DateTime(_currentDate.Year, _currentDate.Month, i),
                    Events = new ObservableCollection<EventDayModel>()
                };
                Days.Add(day);
            }

            // Привязываем события к дням
            BindEventsToDays();
        }

        private void AddEvent()
        {
            AddEventWindow addEventWindow = new AddEventWindow(this);
            if (addEventWindow.ShowDialog() == true)
            {
                EventModel newEvent = addEventWindow.Event;

                // Проверяем, есть ли у события заголовок и корректные даты
                if (string.IsNullOrWhiteSpace(newEvent.Title) || newEvent.StartDate == default || newEvent.EndDate == default)
                {
                    Debug.WriteLine("Попытка добавить пустое событие — отменяем.");
                    return;
                }

                // Добавляем новое событие в коллекцию
                SaveEvent(newEvent);
                BindEventsToDays(); // Обновляем отображение событий на календаре
            }
        }

        private Dictionary<Guid, int> eventRowMap = new Dictionary<Guid, int>();
        private const int MAX_ROWS = 5;
        public void BindEventsToDays()
        {
            Debug.WriteLine("--- Начало BindEventsToDays ---");

            // Очищаем события во всех днях
            foreach (var day in Days)
            {
                day?.Events.Clear();
            }

            // Сортируем события по дате начала
            var sortedEvents = Events.OrderBy(e => e.StartDate).ToList();

            // Словарь для отслеживания занятых строк, но теперь ПО НЕДЕЛЯМ
            Dictionary<int, List<int>> weekRowMapping = new Dictionary<int, List<int>>();

            foreach (var ev in sortedEvents)
            {
                // Пересчитываем строку в КАЖДОЙ новой неделе
                int assignedRow = -1;
                int? lastWeek = null;

                for (DateTime date = ev.StartDate; date <= ev.EndDate; date = date.AddDays(1))
                {
                    int currentWeek = GetWeekNumber(date);

                    // Если мы зашли в новую неделю, ищем новую строку
                    if (lastWeek == null || lastWeek != currentWeek)
                    {
                        assignedRow = 0; // Начинаем искать место заново
                        if (!weekRowMapping.ContainsKey(currentWeek))
                        {
                            weekRowMapping[currentWeek] = new List<int>();
                        }

                        // Ищем первый свободный ряд в этой неделе
                        while (weekRowMapping[currentWeek].Contains(assignedRow))
                        {
                            assignedRow++;
                        }

                        weekRowMapping[currentWeek].Add(assignedRow);
                        lastWeek = currentWeek;
                    }

                    // Добавляем событие в соответствующий день
                    var day = Days.FirstOrDefault(d => d?.Date.Date == date.Date);
                    if (day != null)
                    {
                        bool showTitle = (date == ev.StartDate || date.DayOfWeek == DayOfWeek.Monday);
                        day.Events.Add(new EventDayModel
                        {
                            OriginalEvent = ev,
                            ShowTitle = showTitle,
                            Row = assignedRow
                        });
                    }
                }
            }

            OnPropertyChanged(nameof(Days));
            UpdateMonthsWithEvents();
            Debug.WriteLine("--- Конец BindEventsToDays ---");
        }

        private int GetWeekNumber(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            return culture.Calendar.GetWeekOfYear(date, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
        }
        /// Получает номер недели для конкретной даты.
        private bool EventsOverlap(EventModel e1, EventModel e2)
        {
            return e1.StartDate <= e2.EndDate && e1.EndDate >= e2.StartDate;
        }

        public void SaveEvent(EventModel updatedEvent)
        {
            // Проверяем, не является ли событие пустым
            if (string.IsNullOrWhiteSpace(updatedEvent.Title) || updatedEvent.StartDate == default || updatedEvent.EndDate == default)
            {
                Debug.WriteLine("Попытка сохранить пустое событие — отменяем.");
                return;
            }

            var existingEvent = Events.FirstOrDefault(e => e.Id == updatedEvent.Id);
            if (existingEvent != null)
            {
                existingEvent.Title = updatedEvent.Title;
                existingEvent.StartDate = updatedEvent.StartDate;
                existingEvent.EndDate = updatedEvent.EndDate;
            }
            else
            {
                Events.Add(updatedEvent);
            }

            RefreshEvents();
        }

        public void UpdateEvent(EventModel updatedEvent)
        {
            var existingEvent = Events.FirstOrDefault(e => e.Id == updatedEvent.Id);

            if (existingEvent != null)
            {
                // Удаляем старое событие из календаря
                foreach (var day in Days.Where(d => d != null))
                {
                    var eventToRemove = day.Events.FirstOrDefault(e => e.OriginalEvent.Id == existingEvent.Id);
                    if (eventToRemove != null)
                    {
                        day.Events.Remove(eventToRemove);
                    }
                }

                // Обновляем данные существующего события
                existingEvent.Title = updatedEvent.Title;
                existingEvent.StartDate = updatedEvent.StartDate;
                existingEvent.EndDate = updatedEvent.EndDate;
            }

            // Перерисовываем события в календаре
            BindEventsToDays();
        }

        public void RefreshEvents()
        {
            Debug.WriteLine("RefreshEvents called");
            // Не очищаем eventRowMap, чтобы сохранить строки
            var updatedEvents = new ObservableCollection<EventModel>(Events);
            Events.Clear();
            foreach (var ev in updatedEvents)
            {
                Events.Add(ev);
            }
            Days.Clear();
            UpdateDays();
            OnPropertyChanged(nameof(Days));
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }



    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;

        public RelayCommand(Action<object> execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged;

    }
}
