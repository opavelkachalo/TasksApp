using System;
using System.Collections.ObjectModel;

namespace TasksApp
{
    public class User
    {
        public string Name { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Login { get; }
        public byte[] Password { get; set; }
        public ObservableCollection<Task> Tasks { get; set; }


        public User(string name, DateTime? birthDate, string login, byte[] password)
        {
            Name = name;
            BirthDate = birthDate;
            Login = login;
            Password = password;
            Tasks = new ObservableCollection<Task>();
        }

        public ObservableCollection<Task> DailyTasksCollection(DateTime today)
        {
            ObservableCollection<Task> tasksCollection = new ObservableCollection<Task>();
            foreach (Task task in Tasks)
            {
                if (task.DeadLine == today.Date)
                {
                    tasksCollection.Add(task);
                }
            }

            return tasksCollection;
        }

        public ObservableCollection<Task> MonthlyTasksCollection(DateTime month)
        {
            ObservableCollection<Task> monthlyTasksCollection = new ObservableCollection<Task>();
            foreach (Task task in Tasks)
            {
                if (task.DeadLine != null && task.DeadLine.Value.Month == month.Month)
                {
                    monthlyTasksCollection.Add(task);
                }
            }

            return monthlyTasksCollection;
        }
    }
}