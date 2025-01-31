using Risk_Work.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Risk_Work.Pages
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Page
    {
        public Authorization()
        {
            InitializeComponent();
        }

        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            string login = TxtLogin.Text;
            string password = TxtPassword.Password;

            try
            {
                var user = RiskBDEntities.GetContext().Users.FirstOrDefault(u => u.Username == login);

                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Необходимо заполнить все поля", "Ошибка при авторизации",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (user == null)
                {
                    TxtLogin.ToolTip = "Неверный логин";
                    TxtLogin.Background = Brushes.Red;
                    MessageBox.Show("Неверный логин", "Ошибка при авторизации",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (user.PasswordHash != password)
                {
                    TxtPassword.Clear();
                    TxtPassword.ToolTip = "Неправильный пароль";
                    TxtPassword.Background = Brushes.Red;
                    MessageBox.Show("Неправильный пароль", "Ошибка при авторизации",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ClassFrame.UserID = user.UserID;
                CurrentUser.Username = user.Username;
                CurrentUser.PasswordHash = user.PasswordHash;
                CurrentUser.Role = RiskBDEntities.GetContext().Users.FirstOrDefault(r => r.UserID == user.UserID)?.Role;
                ClassFrame.frmObj.Navigate(new MainRisk());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка при авторизации",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
