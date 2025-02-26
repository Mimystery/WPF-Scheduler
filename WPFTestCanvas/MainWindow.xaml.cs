using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WPFTestCanvas.Model;
using WPFTestCanvas.View;
using WPFTestCanvas.ViewModel;

namespace WPFTestCanvas
{
    public partial class MainWindow : Window
    {
        private DateTime _lastClickTime = DateTime.MinValue;
        private SchedulerViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new SchedulerViewModel();  // Присваиваем экземпляр в переменную _viewModel
            DataContext = _viewModel;  // Связываем DataContext с _viewModel
        }

        private void Event_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is EventDayModel eventDay)
            {
                DateTime now = DateTime.Now;
                TimeSpan timeSinceLastClick = now - _lastClickTime;

                if (timeSinceLastClick.TotalMilliseconds < 300)
                {
                    OpenEventDetails(eventDay.OriginalEvent);
                }

                _lastClickTime = now;
            }
        }

        private void OpenEventDetails(EventModel eventModel)
        {
            EventDetailsWindow detailsWindow = new EventDetailsWindow(eventModel, _viewModel);

            if (detailsWindow.ShowDialog() == true)
            {
                _viewModel.SaveEvent(eventModel); // Сохраняем изменения
            }
        }

        private void MonthEvent_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is EventDayModel eventDay)
            {
                DateTime now = DateTime.Now;
                TimeSpan timeSinceLastClick = now - _lastClickTime;

                if (timeSinceLastClick.TotalMilliseconds < 300)
                {
                    OpenEventDetails(eventDay.OriginalEvent);
                }

                _lastClickTime = now;
            }
        }
    }
}
