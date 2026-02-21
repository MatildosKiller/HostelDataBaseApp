using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HostelDataBaseApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() //конструктор класса
        {
           InitializeComponent();
            this.Btn.Click += AddGuest_Click;
        }


        private void AddGuest_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(FullNameTextBox.Text))
            {
                try
                {
                    



                    var newGuest = new Guest
                    {
                        FullName = FullNameTextBox.Text.Trim(),
                        Phone = PhoneTextBox.Text.Trim() ?? "Не указан",
                        Email = EmailTextBox.Text.Trim() ?? "Не указан",
                        PassportData = PassportTextBox.Text.Trim() ?? "Не указаны"
                    };

                    DatabaseHelper.AddGuest(newGuest);
                    LoadGuests_Click(sender, e); // Обновляем список гостей
                    FullNameTextBox.Clear(); // Очищаем поле ввода ФИО
                    PhoneTextBox.Clear();     // Очищаем поле телефона
                    EmailTextBox.Clear();    // Очищаем поле email
                    PassportTextBox.Clear();  // Очищаем поле паспорта
                    MessageBox.Show("Гость успешно добавлен!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении гостя: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Заполните поле ФИО гостя!");
            }
        }


        private void LoadGuests_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var guests = DatabaseHelper.GetGuests();
                GuestsDataGrid.ItemsSource = guests;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }
    }
}