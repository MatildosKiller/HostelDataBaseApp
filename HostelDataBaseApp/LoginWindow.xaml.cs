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

namespace HostelDataBaseApp
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
       


        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            var user = DatabaseHelper.AuthenticateUser(username, password);
            if (user!=null)
            {
                this.DialogResult = true;
                App.CurrentUser = user; 
                this.Close();
            }
            else
            {
                ErrorTextBlock.Text = "Неверный логин или пароль";
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }
    }
}
