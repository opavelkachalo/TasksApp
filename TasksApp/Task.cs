using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TasksApp
{
    public class Task : INotifyPropertyChanged
    {
        private string _name;
        private DateTime? _deadLine;
        private bool _isSelected;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public DateTime? DeadLine
        {
            get => _deadLine;
            set
            {
                _deadLine = value;
                OnPropertyChanged("DeadLine");
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public Task(string name, DateTime? deadline)
        {
            Name = name;
            DeadLine = deadline;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
