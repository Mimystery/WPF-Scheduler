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
using Syncfusion.Blazor.Inputs;
using WPFTestCanvas.Model;
using WPFTestCanvas.ViewModel;

namespace WPFTestCanvas.View
{
    /// <summary>
    /// Логика взаимодействия для EventsListWindow.xaml
    /// </summary>
    public partial class EventsListWindow : Window
    {
        private readonly SchedulerViewModel _viewModel;
        public EventsListWindow(DayModel day, SchedulerViewModel viewModel)
        {
            InitializeComponent();
            DataContext = day;
            _viewModel = viewModel;
            Title = $"События на {day.Date:dd.MM.yyyy}";
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is EventDayModel selectedEvent)
            {
                EventModel eventModel = selectedEvent.OriginalEvent; // Достаём EventModel
                if (eventModel != null)
                {
                    EventDetailsWindow detailsWindow = new EventDetailsWindow(eventModel, _viewModel);
                    detailsWindow.ShowDialog();
                }

                listBox.SelectedItem = null; // Сбрасываем выделение
            }
        }

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is EventDayModel selectedEvent)
            {
                EventModel eventModel = selectedEvent.OriginalEvent; // Достаём EventModel
                if (eventModel != null)
                {
                    EventDetailsWindow detailsWindow = new EventDetailsWindow(eventModel, _viewModel);
                    detailsWindow.ShowDialog();
                }

                listBox.SelectedItem = null; // Сбрасываем выделение
            }
        }
    }
}
