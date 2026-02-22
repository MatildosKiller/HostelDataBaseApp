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
}
