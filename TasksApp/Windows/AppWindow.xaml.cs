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
        public DateTime TasksMonth;
        public bool SideBarIsOpen;

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
            AddMonthly.MouseLeftButtonDown += AddNewTask;
            ATasksBtn.Click += OpenAllTasks;
            DTasksBtn.Click += OpenDailyTasks;
            MTasksBtn.Click += OpenMonthlyTasks;
            ProfileBtn.Click += OpenMyProfile;

            // Initializing date for Daily Tasks date
            DTaskDay = DateTime.Now.Date;

            // Initializing date to observe month of Monthly Tasks
            TasksMonth = DateTime.Now;

            // Binding ActiveUser.Tasks to AllTasksList ListBox
            AllTasksList.ItemsSource = ActiveUser.Tasks;

            // Binding all tasks of dTaskDay date to DailyTasksList ListBox
            DailyTasksList.ItemsSource = ActiveUser.DailyTasksCollection(DTaskDay);

            // Binding all tasks of TaskMonth month to MonthlyTasksList ListBox
            MonthlyTasksList.ItemsSource = ActiveUser.MonthlyTasksCollection(TasksMonth);
        }

        // Opening and closing sidebar
        private void SideBar(object sender, RoutedEventArgs e)
        {
            if ((Button) sender == SidebarClose)
            {
                CloseSideBar();
            }
            else if ((Button) sender == SidebarOpen)
            {
                OpenSideBar();
            }
        }

        private void OpenSideBar()
        {
            SideBarIsOpen = true;
            SideBarRect.Visibility = Visibility.Visible;
            SideBarGrid.Visibility = Visibility.Visible;
            SidebarOpenPanel.Visibility = Visibility.Collapsed;
            BackgroundRectangle.Visibility = Visibility.Visible;
        }

        private void CloseSideBar()
        {
            SideBarIsOpen = false;
            SideBarRect.Visibility = Visibility.Collapsed;
            SideBarGrid.Visibility = Visibility.Collapsed;
            SidebarOpenPanel.Visibility = Visibility.Visible;
            BackgroundRectangle.Visibility = Visibility.Collapsed;
        }

        // Opens new dialog window, that creates new task
        private void AddNewTask(object sender, MouseButtonEventArgs e)
        {
            Window1 createTaskWindow;

            // initialization of dialog window for creating task
            if ((TextBlock) sender == AddTask || (TextBlock) sender == AddMonthly)
            {
                createTaskWindow = new Window1();
            }
            else
            {
                createTaskWindow = new Window1(DTaskDay);
            }

            if (createTaskWindow.ShowDialog() == true)
            {
                Task newTask = new Task(createTaskWindow.NameOfTask, createTaskWindow.DeadlineOfTask);

                // adding newTask to ActiveUser.Tasks and sorting it by DeadLine
                ActiveUser.Tasks.Add(newTask);
                ActiveUser.Tasks = new ObservableCollection<Task>(ActiveUser.Tasks.OrderBy(t => t.DeadLine));
                
                // updating ListBoxes on all pages
                AllTasksList.ItemsSource = ActiveUser.Tasks;
                DailyTasksList.ItemsSource = ActiveUser.DailyTasksCollection(DTaskDay);
                MonthlyTasksList.ItemsSource = ActiveUser.MonthlyTasksCollection(TasksMonth);

                // saving data
                SaveData();
            }
        }

        // Saving all changed data to active.json and users.json
        private void SaveData()
        {
            // updating and saving active.json
            string activeUser = JsonConvert.SerializeObject(ActiveUser);
            File.WriteAllText(Directory.GetCurrentDirectory() + @"\..\..\active.json", activeUser);
            // updating users.json
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
            // saving users.json
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
            MonthlyTasksList.ItemsSource = ActiveUser.MonthlyTasksCollection(TasksMonth);
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
            MonthlyTasksList.ItemsSource = ActiveUser.MonthlyTasksCollection(TasksMonth);
            SaveData();
        }

        // Opening panel with all tasks
        private void OpenAllTasks(object sender, RoutedEventArgs e)
        {
            foreach (UIElement border in MainGrid.Children)
            {
                border.Visibility = border == ATasks ? Visibility.Visible : Visibility.Collapsed;
            }

            CloseSideBar();
        }

        // Opening panel with daily tasks
        private void OpenDailyTasks(object sender, RoutedEventArgs e)
        {
            foreach (UIElement border in MainGrid.Children)
            {
                border.Visibility = border == DTasks ? Visibility.Visible : Visibility.Collapsed;
            }

            CloseSideBar();

            DTaskDay = DateTime.Now.Date;
            DateOfDTasks.Text = DTaskDay.ToString("D", new CultureInfo("en"));
            DateOfDTasks.Foreground = new SolidColorBrush(Colors.Red);
            DailyTasksList.ItemsSource = ActiveUser.DailyTasksCollection(DTaskDay);
        }

        // Switching date on daily tasks page
        private void SwitchDate(object sender, RoutedEventArgs e)
        {
            // Cases for Daily Tasks page
            if ((Button) sender == Fwd)
            {
                DTaskDay = DTaskDay.AddDays(1);
            }
            else if ((Button) sender == Bwd)
            {
                DTaskDay = DTaskDay.AddDays(-1);
            }
            // Cases for Monthly Tasks page
            else if ((Button) sender == FwdBtn)
            {
                TasksMonth = TasksMonth.AddMonths(1);
            }
            else if ((Button)sender == BwdBtn)
            {
                TasksMonth = TasksMonth.AddMonths(-1);
            }
            // Updating Label and ListBox for Daily Tasks page
            if ((Button) sender == Fwd || (Button) sender == Bwd)
            {
                DateOfDTasks.Foreground = DTaskDay == DateTime.Now.Date
                    ? new SolidColorBrush(Colors.Red)
                    : new SolidColorBrush(Colors.Black);

                DateOfDTasks.Text = DTaskDay.ToString("D", new CultureInfo("en"));
                DailyTasksList.ItemsSource = ActiveUser.DailyTasksCollection(DTaskDay);
            }
            // Updating Label and Listbox for Monthly Tasks page
            else
            {
                MonthOfTasks.Foreground = TasksMonth.Month == DateTime.Now.Month
                    ? new SolidColorBrush(Colors.Red)
                    : new SolidColorBrush(Colors.Black);
                MonthOfTasks.Text = TasksMonth.ToString("yyyy MMMM", new CultureInfo("en"));
                MonthlyTasksList.ItemsSource = ActiveUser.MonthlyTasksCollection(TasksMonth);
            }
        }

        // Opening panel with monthly tasks
        private void OpenMonthlyTasks(object sender, RoutedEventArgs e)
        {
            foreach (UIElement border in MainGrid.Children)
            {
                border.Visibility = border == MTasks ? Visibility.Visible : Visibility.Collapsed;
            }

            CloseSideBar();

            TasksMonth = DateTime.Now.Date;
            MonthOfTasks.Text = DTaskDay.ToString("yyyy MMMM", new CultureInfo("en"));
            MonthOfTasks.Foreground = new SolidColorBrush(Colors.Red);
            MonthlyTasksList.ItemsSource = ActiveUser.MonthlyTasksCollection(TasksMonth);
        }

        // Opening panel with profile settings
        private void OpenMyProfile(object sender, RoutedEventArgs e)
        {
            foreach (UIElement border in MainGrid.Children)
            {
                border.Visibility = border == MyProfile ? Visibility.Visible : Visibility.Collapsed;
            }

            CloseSideBar();

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

        private void Background_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SideBarIsOpen)
                CloseSideBar();
        }
    }
}
// TODO: Add history for done marks
// TODO: Add logic with timing of deadlines of tasks
// TODO: Add animations to sidebar
// TODO: Add icon to app
// TODO: Make an .exe file
