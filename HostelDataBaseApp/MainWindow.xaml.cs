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
        private Room selectedRoom = null;
        public MainWindow() //конструктор класса
        {
           InitializeComponent();
            LoadGuestsAndRooms_OnStartup();


        }

       

        private void LoadRooms()
        {
            try
            {
                var rooms = DatabaseHelper.GetRooms();
                RoomsDataGrid.ItemsSource = rooms;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки номеров: {ex.Message}");
            }
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

        private void LoadGuestsAndRooms_OnStartup()
        {
            try
            {
                // Загружаем гостей для ComboBox
                var guests = DatabaseHelper.GetGuests();
                GuestComboBox.ItemsSource = guests;
                GuestsDataGrid.ItemsSource = guests;
                // Загружаем номера для ComboBox
                var rooms = DatabaseHelper.GetRooms();
                RoomComboBox.ItemsSource = rooms;

                // Загружаем все бронирования
                LoadBookings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void CreateBooking_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (GuestComboBox.SelectedValue == null || RoomComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Выберите гостя и номер!");
                    return;
                }

                int guestId = (int)GuestComboBox.SelectedValue;
                int roomId = (int)RoomComboBox.SelectedValue;
                DateTime checkIn = CheckInDatePicker.SelectedDate ?? DateTime.Today;
                DateTime checkOut = CheckOutDatePicker.SelectedDate ?? checkIn.AddDays(1);
                decimal totalAmount = decimal.Parse(TotalAmountTextBox.Text);

                if (checkOut <= checkIn)
                {
                    MessageBox.Show("Дата выезда должна быть позже даты заезда!");
                    return;
                }

                int bookingId = DatabaseHelper.CreateBooking(guestId, roomId, checkIn, checkOut, totalAmount);
                MessageBox.Show($"Бронирование создано успешно! ID: {bookingId}");

                // Обновляем список бронирований
                LoadBookings();

                // Очищаем форму
                ClearBookingForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании бронирования: {ex.Message}");
            }
        }

        private void FindAvailableRooms_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime checkIn = SearchCheckInDate.SelectedDate ?? DateTime.Today;
                var availableRooms = DatabaseHelper.FindAvailableRooms(checkIn, checkIn.AddDays(1));
                AvailableRoomsDataGrid.ItemsSource = availableRooms;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска свободных номеров: {ex.Message}");
            }
        }

        private void LoadBookings()
        {
            try
            {
                var bookings = DatabaseHelper.GetBookings();
                BookingsDataGrid.ItemsSource = bookings;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки бронирований: {ex.Message}");
            }
        }

        private void ClearBookingForm()
        {
            GuestComboBox.SelectedIndex = -1;
            RoomComboBox.SelectedIndex = -1;
            CheckInDatePicker.SelectedDate = null;
            CheckOutDatePicker.SelectedDate = null;
            TotalAmountTextBox.Clear();
        }

        private void AddRoom_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(RoomNumberTextBox.Text))
                {
                    MessageBox.Show("Введите номер комнаты!");
                    return;
                }

                if (!int.TryParse(CapacityTextBox.Text, out int capacity) || capacity <= 0)
                {
                    MessageBox.Show("Вместимость должна быть положительным числом!");
                    return;
                }

                if (!decimal.TryParse(PricePerNightTextBox.Text, out decimal price) || price < 0)
                {
                    MessageBox.Show("Цена за ночь должна быть неотрицательным числом!");
                    return;
                }

                var room = new Room
                {
                    RoomNumber = RoomNumberTextBox.Text.Trim(),
                    Capacity = capacity,
                    PricePerNight = price,
                    IsActive = IsActiveCheckBox.IsChecked ?? true
                };

                DatabaseHelper.AddRoom(room);
                MessageBox.Show("Номер успешно добавлен!");
                LoadRooms();
                ClearRoomForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении номера: {ex.Message}");
            }
        }

        private void UpdateRoom_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRoom == null)
            {
                MessageBox.Show("Выберите номер для обновления!");
                return;
            }

            try
            {
                selectedRoom.RoomNumber = RoomNumberTextBox.Text.Trim();
                selectedRoom.Capacity = int.Parse(CapacityTextBox.Text);
                selectedRoom.PricePerNight = decimal.Parse(PricePerNightTextBox.Text);
                selectedRoom.IsActive = IsActiveCheckBox.IsChecked ?? true;

                DatabaseHelper.UpdateRoom(selectedRoom);
                MessageBox.Show("Номер успешно обновлён!");
                LoadRooms();
                ClearRoomForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении номера: {ex.Message}");
            }
        }

        private void DeleteRoom_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRoom == null)
            {
                MessageBox.Show("Выберите номер для удаления!");
                return;
            }

            var result = MessageBox.Show($"Вы уверены, что хотите удалить номер {selectedRoom.RoomNumber}?",
                "Подтверждение удаления", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DatabaseHelper.DeleteRoom(selectedRoom.RoomId);
                    MessageBox.Show("Номер успешно удалён!");
                    LoadRooms();
                    ClearRoomForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении номера: {ex.Message}");
                }
            }
        }
        private void RoomsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedRoom = RoomsDataGrid.SelectedItem as Room;

            if (selectedRoom != null)
            {
                // Заполняем форму данными выбранного номера
                RoomNumberTextBox.Text = selectedRoom.RoomNumber;
                CapacityTextBox.Text = selectedRoom.Capacity.ToString();
                PricePerNightTextBox.Text = selectedRoom.PricePerNight.ToString("0.00");
                IsActiveCheckBox.IsChecked = selectedRoom.IsActive;
            }
            else
            {
                // Если ничего не выбрано — очищаем форму
                ClearRoomForm();
            }
        }
        private void ClearRoomForm()
        {
            RoomNumberTextBox.Clear();
            CapacityTextBox.Clear();
            PricePerNightTextBox.Clear();
            IsActiveCheckBox.IsChecked = true;
            selectedRoom = null;
        }

    }
}