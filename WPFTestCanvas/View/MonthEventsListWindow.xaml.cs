using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFTestCanvas.Model;
using WPFTestCanvas.ViewModel;
using static System.Net.Mime.MediaTypeNames;

namespace WPFTestCanvas.View
{
    /// <summary>
    /// Логика взаимодействия для MonthEventsListWindow.xaml
    /// </summary>
    public partial class MonthEventsListWindow : Window
    {
        private DateTime _lastClickTime = DateTime.MinValue;
        private readonly SchedulerViewModel _viewModel;
        public MonthEventsListWindow(MonthViewModel month, SchedulerViewModel viewModel)
        {
            InitializeComponent();
            DataContext = month;
            _viewModel = viewModel;
        }
        
        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var currentTime = DateTime.Now;
            TimeSpan interval = currentTime - _lastClickTime;
            _lastClickTime = currentTime;

            if (interval.TotalMilliseconds < 300) // Проверяем, что нажатие произошло в течение 300 мс
            {
                if (sender is Border border && border.DataContext is EventDayModel selectedEvent)
                {
                    EventModel eventModel = selectedEvent.OriginalEvent;
                    if (eventModel != null)
                    {
                        EventDetailsWindow detailsWindow = new EventDetailsWindow(eventModel, _viewModel);
                        detailsWindow.ShowDialog();
                    }
                }
            }
        }
    }
}
