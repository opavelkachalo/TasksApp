using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;

namespace TasksApp
{
    /// <summary>
    /// Interaction logic for AppWindow.xaml
    /// </summary>
    public partial class AppWindow : Window
    {
        public static string StringUser;
        public User ActiveUser;
        public DateTime DTaskDay;

        public AppWindow()
        {
            InitializeComponent();

            // initializing ActiveUser
            StringUser = File.ReadAllText(Directory.GetCurrentDirectory() + @"\..\..\active.json");
            ActiveUser = JsonConvert.DeserializeObject<User>(StringUser);

            // Adding handlers for events
            SidebarClose.Click += SideBar;
            AddTask.MouseLeftButtonDown += AddNewTask;
            AddDaily.MouseLeftButtonDown += AddNewTask;
            ATasksBtn.Click += OpenAllTasks;
            DTasksBtn.Click += OpenDailyTasks;
            MTasksBtn.Click += OpenMonthlyTasks;
            ProfileBtn.Click += OpenMyProfile;

            // Initializing date for Daily Tasks date
            DTaskDay = DateTime.Now.Date;

            // Binding ActiveUser.Tasks to AllTasksList ListBox
            AllTasksList.ItemsSource = ActiveUser.Tasks;

            // Binding all tasks of dTaskDay to DailyTasksList ListBox
            DailyTasksList.ItemsSource = ActiveUser.DailyTasksCollection(DTaskDay);
        }

        // Opening and closing sidebar
        private void SideBar(object sender, RoutedEventArgs e)
        {
            if ((Button) sender == SidebarClose)
            {
                SideBarRect.Visibility = Visibility.Collapsed;
                SideBarGrid.Visibility = Visibility.Collapsed;
                SidebarOpenPanel.Visibility = Visibility.Visible;
            }
            else if ((Button) sender == SidebarOpen)
            {
                SideBarRect.Visibility = Visibility.Visible;
                SideBarGrid.Visibility = Visibility.Visible;
                SidebarOpenPanel.Visibility = Visibility.Collapsed;
            }
        }

        // Opens new dialog window, that crates new task
        private void AddNewTask(object sender, MouseButtonEventArgs e)
        {
            Window1 createTaskWindow = (TextBlock)sender == AddTask ? new Window1() : new Window1(DTaskDay);
            if (createTaskWindow.ShowDialog() == true)
            {
                Task newTask = new Task(createTaskWindow.NameOfTask, createTaskWindow.DeadlineOfTask);

                // adding newTask to ActiveUser.Tasks and sorting it by DeadLine
                ActiveUser.Tasks.Add(newTask);
                ActiveUser.Tasks = new ObservableCollection<Task>(ActiveUser.Tasks.OrderBy(t => t.DeadLine));
                AllTasksList.ItemsSource = ActiveUser.Tasks;

                DailyTasksList.ItemsSource = ActiveUser.DailyTasksCollection(DTaskDay);
                SaveData();
            }
        }

        // Saving all changed data to active.json and users.json
        private void SaveData()
        {
            string activeUser = JsonConvert.SerializeObject(ActiveUser);
            File.WriteAllText(Directory.GetCurrentDirectory() + @"\..\..\active.json", activeUser);
            List<User> listOfUsers = MainWindow.UsersList();
            foreach (User user in listOfUsers)
            {
                if (user.Login == ActiveUser.Login)
                {
                    user.Tasks = ActiveUser.Tasks;
                    user.Name = ActiveUser.Name;
                    user.BirthDate = ActiveUser.BirthDate;
                    user.Password = ActiveUser.Password;
                }
            }
            string jsonUsers = JsonConvert.SerializeObject(listOfUsers);
            File.WriteAllText(Directory.GetCurrentDirectory() + @"\..\..\users.json", jsonUsers);
        }

        // Marking task as done (Removing task)
        private void DoneBtn_OnClick(object sender, RoutedEventArgs e)
        {
            for (int i = ActiveUser.Tasks.Count - 1; i >= 0; i--)
            {
                Task task = ActiveUser.Tasks[i];
                if (task.IsSelected)
                {
                    ActiveUser.Tasks.Remove(task);
                }
            }
            DailyTasksList.ItemsSource = ActiveUser.DailyTasksCollection(DTaskDay);
            SaveData();
        }

        // Editing Task
        private void EditBtn_OnClick(object sender, RoutedEventArgs e)
        {
            for (int i = ActiveUser.Tasks.Count - 1; i >= 0; i--)
            {
                Task task = ActiveUser.Tasks[i];
                if (task.IsSelected)
                {
                    EditTaskWindow editTaskWindow = new EditTaskWindow(task.Name, task.DeadLine);
                    if (editTaskWindow.ShowDialog() == true)
                    {
                        task.Name = editTaskWindow.NameOfTask;
                        task.DeadLine = editTaskWindow.DeadLineOfTask;
                    }
                }
            }
            DailyTasksList.ItemsSource = ActiveUser.DailyTasksCollection(DTaskDay);
            SaveData();
        }

        // TODO: add logic with timing of deadlines of tasks
        // Opening panel with all tasks
        private void OpenAllTasks(object sender, RoutedEventArgs e)
        {
            foreach (UIElement border in MainGrid.Children)
            {
                border.Visibility = border == ATasks ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        // Opening panel with daily tasks
        private void OpenDailyTasks(object sender, RoutedEventArgs e)
        {
            foreach (UIElement border in MainGrid.Children)
            {
                border.Visibility = border == DTasks ? Visibility.Visible : Visibility.Collapsed;
            }
            DTaskDay = DateTime.Now.Date;
            DateOfDTasks.Text = DTaskDay.ToString("D", new CultureInfo("en"));
            DateOfDTasks.Foreground = new SolidColorBrush(Colors.Red);
            DailyTasksList.ItemsSource = ActiveUser.DailyTasksCollection(DTaskDay);
        }

        // Switching date on daily tasks page
        private void SwitchDate(object sender, RoutedEventArgs e)
        {
            if ((Button) sender == Fwd)
            {
                DTaskDay = DTaskDay.AddDays(1);
            }
            else if ((Button) sender == Bwd)
            {
                DTaskDay = DTaskDay.AddDays(-1);
            }

            DateOfDTasks.Foreground = DTaskDay == DateTime.Now.Date ? 
                new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);

            DateOfDTasks.Text = DTaskDay.ToString("D", new CultureInfo("en"));
            DailyTasksList.ItemsSource = ActiveUser.DailyTasksCollection(DTaskDay);
        }

        // Opening panel with monthly tasks
        private void OpenMonthlyTasks(object sender, RoutedEventArgs e)
        {
            foreach (UIElement border in MainGrid.Children)
            {
                border.Visibility = border == MTasks ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        // Opening panel with profile settings
        private void OpenMyProfile(object sender, RoutedEventArgs e)
        {
            foreach (UIElement border in MainGrid.Children)
            {
                border.Visibility = border == MyProfile ? Visibility.Visible : Visibility.Collapsed;
            }

            UsersName.Text = ActiveUser.Name;
            BirthDay.SelectedDate = ActiveUser.BirthDate;
        }

        // Save changes of profile
        private void SaveChanges_OnClick(object sender, RoutedEventArgs e)
        {
            ActiveUser.Name = UsersName.Text;
            ActiveUser.BirthDate = BirthDay.SelectedDate;
            if (Password1.Password != "" && Password2.Password != "")
            {
                byte[] password1 = MainWindow.HashPassword(Password1.Password);
                byte[] password2 = MainWindow.HashPassword(Password2.Password);
                if (MainWindow.IsEqual(password1, password2))
                {
                    ActiveUser.Password = password2;
                    Password1.Password = "";
                    Password2.Password = "";
                }
                else
                {
                    MessageBox.Show("Passwords doesn't match", "Warning!");
                    return;
                }
            }
            SaveData();
        }
        
        // Exit from account
        private void ExitAccount_OnClick(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(Directory.GetCurrentDirectory() + @"\..\..\active.json", "");
            MainWindow mainWindow = new MainWindow {Left = Left, Top = Top};
            mainWindow.Show();
            Close();
        }
    }
}
// TODO: Add history for done marks