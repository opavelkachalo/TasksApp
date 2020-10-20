using System;
using System.Windows;

namespace TasksApp
{
    /// <summary>
    /// Interaction logic for EditTaskWindow.xaml
    /// </summary>
    public partial class EditTaskWindow : Window
    {
        public string NameOfTask { get; set; }
        public DateTime? DeadLineOfTask { get; set; }
        public EditTaskWindow(string nameOfTask, DateTime? deadLineOfTask)
        {
            InitializeComponent();
            NameOfTask = nameOfTask;
            DeadLineOfTask = deadLineOfTask;
            TaskName.Text = NameOfTask;
            DeadLine.SelectedDate = DeadLineOfTask;
            Edit.Click += EditOnClick;
        }

        private void EditOnClick(object sender, RoutedEventArgs e)
        {
            NameOfTask = TaskName.Text;
            DeadLineOfTask = DeadLine.SelectedDate;
            if (String.IsNullOrWhiteSpace(NameOfTask) || (DeadLineOfTask is null))
            {
                Warn.Content = "You didn't fill all fields!";
                return;
            }
            DialogResult = true;
        }
    }
}
