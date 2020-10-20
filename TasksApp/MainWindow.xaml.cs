using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace TasksApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LogInBtn.Click += LogIn;
            RegisterBtn.Click += Register;
            RegisterNew.Click += Register;

            string active = File.ReadAllText(Directory.GetCurrentDirectory() + @"\..\..\active.json");
            if (active != "")
            {
                LogIn(JsonConvert.DeserializeObject<User>(active));
            }
        }

        public void LogIn(object sender, RoutedEventArgs e)
        {
            string username = LoginBox.Text;
            string password = PasswordBox.Password;

            if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password))
            {
                LoginWarn.Content = "You didn't fill all fields!";
            }
            else
            {
                if (LoginExists(username))
                {
                    byte[] hashPassword = HashPassword(password);
                    User user = FindUser(username);
                    if (IsEqual(hashPassword, user.Password))
                    {
                        LogIn(user);
                    }
                    else
                    {
                        LoginWarn.Content = "Entered password is incorrect!";
                    }
                }
                else
                {
                    LoginWarn.Content = "Entered username doesn't exist!";
                }
            }
        }

        public void Register(object sender, RoutedEventArgs e)
        {
            if ((Button)sender == RegisterBtn)
            {
                // Closing authorization page and opening registration page
                MainEntrance.Visibility = Visibility.Collapsed;
                Registration.Visibility = Visibility.Visible;
            }
            else if ((Button)sender == RegisterNew)
            {
                // reading data from TextBlock
                string newName = RegName.Text;
                DateTime? birthDay = RegBirthdate.SelectedDate;
                string newUsername = RegLogin.Text;
                string password1 = RegPassword1.Password;
                string password2 = RegPassword2.Password;

                // Checking if all fields are filled
                if (String.IsNullOrWhiteSpace(newName) || (birthDay == null) || String.IsNullOrWhiteSpace(newUsername) 
                    || String.IsNullOrWhiteSpace(password1) || String.IsNullOrWhiteSpace(password2))
                {
                    RegWarn.Content = "You didn't fill all fields!";
                }
                else
                {
                    // Checking if two passwords are equal
                    if (IsEqual(Encoding.ASCII.GetBytes(password1), Encoding.ASCII.GetBytes(password2)))
                    {
                        // Hashing password and creating new user
                        byte[] newPassword = HashPassword(password2);
                        User newUser = new User(newName, birthDay, newUsername, newPassword);

                        // Checking if username already exists
                        if (LoginExists(newUser.Login))
                        { 
                            RegWarn.Content = "Your username is not available!";
                            return;
                        }
                        
                        // Adding new user to list and re-writing users.json file
                        List<User> users = UsersList();
                        users.Add(newUser);
                        string jsonUsers = JsonConvert.SerializeObject(users);
                        File.WriteAllText(Directory.GetCurrentDirectory() + @"\..\..\users.json", jsonUsers);

                        // Logging in
                        LogIn(newUser);
                    }
                    else
                    {
                        RegWarn.Content = "Passwords doesn't match!";
                    }
                }
            }
            
        }

        // Makes hashed byte array from given string
        public static byte[] HashPassword(string password)
        {
            byte[] tempBytes = Encoding.ASCII.GetBytes(password);
            byte[] byteHash = new MD5CryptoServiceProvider().ComputeHash(tempBytes);
            return byteHash;
        }

        // Checks if two given byte arrays are equal
        public static bool IsEqual(byte[] hash1, byte[] hash2)
        {
            if (hash1.Length != hash2.Length) return false;
            for (int i = 0; i < hash1.Length; i++)
            {
                if (hash1[i] != hash2[i])
                    return false;
            }

            return true;
        }

        // Checks if given login exists in UsersList
        public bool LoginExists(string login)
        {
            foreach (User user in UsersList())
            {
                if (user.Login == login)
                {
                    return true;
                }
            }

            return false;
        }

        // Returns list of users from users.json file
        public static List<User> UsersList()
        {
            string allUsers = File.ReadAllText(Directory.GetCurrentDirectory() + @"\..\..\users.json");
            return JsonConvert.DeserializeObject<List<User>>(allUsers);
        }

        // Returns user with given login
        public User FindUser(string login)
        {
            foreach (User user in UsersList())
            {
                if (user.Login == login)
                {
                    return user;
                }
            }

            return null;
        }

        // Logging in method
        public void LogIn(User user)
        {
            string activeUser = JsonConvert.SerializeObject(user);
            File.WriteAllText(Directory.GetCurrentDirectory() + @"\..\..\active.json", activeUser);
            AppWindow appWindow = new AppWindow();
            if (Application.Current.MainWindow != null)
            {
                appWindow.Left = Application.Current.MainWindow.Left;
                appWindow.Top = Application.Current.MainWindow.Top;
            }
            appWindow.Show();
            Close();
        }
    }
}
