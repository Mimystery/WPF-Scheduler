using Microsoft.Extensions.Logging;
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

namespace WPFTestCanvas.View
{
    /// <summary>
    /// Логика взаимодействия для AddEventWindow.xaml
    /// </summary>
    public partial class AddEventWindow : Window
    {
        private readonly SchedulerViewModel _viewModel;
        public EventModel NewEvent { get; private set; }
        public EventModel Event { get; set; } // Новый параметр для редактирования события
        private string SelectedColor = "#03bafc";

        public AddEventWindow(SchedulerViewModel viewModel, EventModel eventToEdit = null)
        {
            InitializeComponent();
            _viewModel = viewModel; // Сохраняем ссылку на ViewModel

            if (eventToEdit != null)
            {
                // Режим изменения события
                Event = eventToEdit;
                TitleTextBox.Text = Event.Title;
                StartDatePicker.SelectedDate = Event.StartDate;
                EndDatePicker.SelectedDate = Event.EndDate;
            }
            else
            {
                // Режим добавления нового события
                Event = new EventModel();
            }
        }

        private void AddSave_Click(object sender, RoutedEventArgs e)
        {
            if (TitleTextBox.Text.Length > 0 && StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.HasValue)
            {
                EventModel newEvent = new EventModel
                {
                    Title = TitleTextBox.Text,
                    StartDate = StartDatePicker.SelectedDate.Value,
                    EndDate = EndDatePicker.SelectedDate.Value,
                    Color = SelectedColor // Сохраняем цвет в событии
                };

                _viewModel.SaveEvent(newEvent);

                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please fill in all fields.");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ColorPickerControl_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                SelectedColor = e.NewValue.Value.ToString(); // Сохраняем цвет в HEX формате
            }
        }
    }
}
