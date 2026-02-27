using System.Configuration;
using System.Data;
using System.Windows;

namespace HostelDataBaseApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            CurrentUser = null; // Изначально пользователь не авторизован
        }

        public static User CurrentUser { get; set; }
    }

}
