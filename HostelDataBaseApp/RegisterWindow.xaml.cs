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
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
       
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            string fullName = FullNameTextBox.Text;

            // Валидация
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullName))
            {
                ErrorTextBlock.Text = "Заполните все поля";
                return;
            }

            if (password != confirmPassword)
            {
                ErrorTextBlock.Text = "Пароли не совпадают";
                return;
            }

            // Регистрация пользователя
            if (DatabaseHelper.RegisterUser(username, password, fullName))
            {
                MessageBox.Show("Регистрация успешна! Теперь можно войти в систему.");
                this.Close();
            }
            else
            {
                ErrorTextBlock.Text = "Пользователь с таким логином уже существует";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
