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
    }
}