using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WPFTestCanvas.Model
{
    public class EventModel : INotifyPropertyChanged
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        private DateTime _startDate;
        private DateTime _endDate;
        private string _title;
        public int CreationIndex { get; set; }
        public string Color { get; set; } = "Gray";

        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged(nameof(StartDate));
                }
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged(nameof(EndDate));
                }
            }
        }

        private int _assignedRow = -1;
        public int AssignedRow
        {
            get => _assignedRow;
            set { _assignedRow = value; OnPropertyChanged(nameof(AssignedRow)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
