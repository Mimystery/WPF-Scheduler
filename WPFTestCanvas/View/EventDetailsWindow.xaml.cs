using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace WPFTestCanvas.View
{
    /// <summary>
    /// Логика взаимодействия для EventDetailsWindow.xaml
    /// </summary>
    public partial class EventDetailsWindow : Window
    {
        public EventModel Event { get; private set; }

        private readonly SchedulerViewModel _viewModel;

        private string SelectedColor = "#03bafc";

        public EventDetailsWindow(EventModel eventModel, SchedulerViewModel viewModel)
        {
            InitializeComponent();
            Event = eventModel;
            _viewModel = viewModel;
            DataContext = Event;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        private void ChangeSave_Click(object sender, RoutedEventArgs e)
        {
            if (TitleTextBox.Text.Length > 0 && StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.HasValue)
            {
                // Обновляем данные события
                Event.Title = TitleTextBox.Text;
                Event.StartDate = StartDatePicker.SelectedDate.Value;
                Event.EndDate = EndDatePicker.SelectedDate.Value;

                // Если выбран новый цвет, обновляем его, иначе оставляем прежний
                if (EventColorPicker.SelectedColor.HasValue)
                {
                    Event.Color = EventColorPicker.SelectedColor.Value.ToString();
                }

                // Обновляем событие в ViewModel
                _viewModel.UpdateEvent(Event);

                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please fill in all fields.");
            }
        }
    }
}
