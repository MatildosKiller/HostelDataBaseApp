using System.Data;
using HostelDataBaseApp;
using Microsoft.Data.SqlClient;

public static class DatabaseHelper
{
    private static string connectionString = "Server=.\\SQLEXPRESS;Database=HostelDB;Trusted_Connection=True;TrustServerCertificate=True;";

    // Получение списка гостей
    public static List<Guest> GetGuests()
    {
        var guests = new List<Guest>();
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand("SELECT * FROM Guests", connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    guests.Add(new Guest
                    {
                        GuestId = reader.GetInt32("GuestId"),
                        FullName = reader.GetString("FullName"),
                        Phone = reader.GetString("Phone"),
                        Email = reader.GetString("Email"),
                        PassportData = reader.GetString("PassportData")
                    });
                }
            }
        }
        return guests;
    }

    // Добавление гостя
    public static void AddGuest(Guest guest)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand(
                "INSERT INTO Guests (FullName, Phone, Email, PassportData) " +
                "VALUES (@FullName, @Phone, @Email, @PassportData)", connection);

            command.Parameters.AddWithValue("@FullName", guest.FullName);
            command.Parameters.AddWithValue("@Phone", guest.Phone);
            command.Parameters.AddWithValue("@Email", guest.Email);
            command.Parameters.AddWithValue("@PassportData", guest.PassportData);

            command.ExecuteNonQuery();
        }
    }

    public static List<Room> GetRooms()
    {
        var rooms = new List<Room>();
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand("SELECT * FROM Rooms", connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    rooms.Add(new Room
                    {
                        RoomId = reader.GetInt32("RoomId"),
                        RoomNumber = reader.GetString("RoomNumber"),
                        Capacity = reader.GetInt32("Capacity"),
                        PricePerNight = reader.GetDecimal("PricePerNight"),
                        IsActive = reader.GetBoolean("IsActive")
                    });
                }
            }
        }
        return rooms;
    }


    // Поиск свободных номеров на период
    public static List<Room> FindAvailableRooms(DateTime checkIn, DateTime checkOut)
    {
        var availableRooms = new List<Room>();
        //LINQ

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            var sql = @"
                SELECT r.*
                FROM Rooms r
                WHERE r.IsActive = 1
                AND r.RoomId NOT IN (
                    SELECT b.RoomId
                    FROM Bookings b
                    WHERE b.Status = 'Active'
                    AND (
                        (b.CheckInDate <= @CheckOut AND b.CheckOutDate >= @CheckIn)
                    )
                )";

            var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@CheckIn", checkIn);
            command.Parameters.AddWithValue("@CheckOut", checkOut);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    availableRooms.Add(new Room
                    {
                        RoomId = reader.GetInt32("RoomId"),
                        RoomNumber = reader.GetString("RoomNumber"),
                        Capacity = reader.GetInt32("Capacity"),
                        PricePerNight = reader.GetDecimal("PricePerNight")
                    });
                }
            }
        }

        return availableRooms;
    }

    // Создание бронирования
    public static int CreateBooking(int guestId, int roomId, DateTime checkIn, DateTime checkOut, decimal totalAmount)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            var sql = @"
                INSERT INTO Bookings (GuestId, RoomId, CheckInDate, CheckOutDate, TotalAmount)
                VALUES (@GuestId, @RoomId, @CheckIn, @CheckOut, @TotalAmount);
                SELECT SCOPE_IDENTITY();";

            var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@GuestId", guestId);
            command.Parameters.AddWithValue("@RoomId", roomId);
            command.Parameters.AddWithValue("@CheckIn", checkIn);
            command.Parameters.AddWithValue("@CheckOut", checkOut);
            command.Parameters.AddWithValue("@TotalAmount", totalAmount);

            return Convert.ToInt32(command.ExecuteScalar());
        }
    }

    // Получение бронирований
    public static List<Booking> GetBookings()
    {
        var bookings = new List<Booking>();
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var sql = @"
                SELECT b.*, g.FullName, r.RoomNumber
                FROM Bookings b
                JOIN Guests g ON b.GuestId = g.GuestId
                JOIN Rooms r ON b.RoomId = r.RoomId";

            var command = new SqlCommand(sql, connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    bookings.Add(new Booking
                    {
                        BookingId = reader.GetInt32("BookingId"),
                        GuestId = reader.GetInt32("GuestId"),
                        RoomId = reader.GetInt32("RoomId"),
                        CheckInDate = reader.GetDateTime("CheckInDate"),
                        CheckOutDate = reader.GetDateTime("CheckOutDate"),
                        TotalAmount = reader.GetDecimal("TotalAmount"),
                        Status = reader.GetString("Status"),
                        CreatedDate = reader.GetDateTime("CreatedDate"),
                        GuestName = reader.GetString("FullName"),
                        RoomNumber = reader.GetString("RoomNumber")
                    });
                }
            }
        }
        return bookings;
    }

    // Добавление номера
  
    public static void AddRoom(Room room)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            
            connection.Open();
            var command = new SqlCommand(
     "INSERT INTO Rooms (RoomNumber, Capacity, PricePerNight, IsActive) " +
     "VALUES (@RoomNumber, @Capacity, @PricePerNight, @IsActive)",
     connection);

            command.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
            command.Parameters.AddWithValue("@Capacity", room.Capacity);
            command.Parameters.AddWithValue("@PricePerNight", room.PricePerNight);
            command.Parameters.AddWithValue("@IsActive", room.IsActive);

            command.ExecuteNonQuery();
        }
    }


    // Обновление номера
    public static void UpdateRoom(Room room)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand
                ("UPDATE Rooms SET RoomNumber = @RoomNumber, Capacity = @Capacity, " +
                "PricePerNight = @PricePerNight, IsActive = @IsActive WHERE RoomId = @RoomId", connection);

            command.Parameters.AddWithValue("@RoomId", room.RoomId);
            command.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
            command.Parameters.AddWithValue("@Capacity", room.Capacity);
            command.Parameters.AddWithValue("@PricePerNight", room.PricePerNight);
            command.Parameters.AddWithValue("@IsActive", room.IsActive);

            command.ExecuteNonQuery();
        }
    }

    // Удаление номера
    public static void DeleteRoom(int roomId)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand("DELETE FROM Rooms WHERE RoomId = @RoomId", connection);
            command.Parameters.AddWithValue("@RoomId", roomId);
            command.ExecuteNonQuery();
        }
    }

}
