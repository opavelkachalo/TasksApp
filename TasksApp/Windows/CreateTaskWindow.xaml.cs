using System;
using System.Windows;

namespace TasksApp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        public Window1(DateTime? someDate=null)
        {
            InitializeComponent();
            Create.Click += CreateTaskOnClick;
            DeadLine.DisplayDateStart = DateTime.Now;
            if (someDate != null)
                DeadLine.SelectedDate = someDate;
        }

        private void CreateTaskOnClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(NameOfTask) || (DeadlineOfTask is null))
            {
                Warn.Content = "You didn't fill all fields!";
                return;
            }
            DialogResult = true;
        }

        public string NameOfTask => TaskName.Text;
        public DateTime? DeadlineOfTask => DeadLine.SelectedDate;
    }
}
