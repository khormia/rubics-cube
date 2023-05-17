using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;
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

namespace RubicsCube.View
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        MySqlConnection con = new MySqlConnection();
        MySqlCommand cmd;
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public static string login;

        public AuthorizationWindow()
        {
            InitializeComponent();
        }

        private void AuthClick(object sender, RoutedEventArgs e)
        {
            login = ComboBoxAccess.Text.ToString();
            string pass = PasswordText.Password.ToString();
            try
            {
                con.ConnectionString = "server=localhost;port=3306;database=<our-database-name>;username=root;password=123";
                con.Open();
                cmd = new MySqlCommand("SELECT login, password from employees WHERE BINARY login=@log and BINARY password=@pass", con);
                cmd.Parameters.Add("@log", MySqlDbType.VarChar).Value = login;
                cmd.Parameters.Add("@pass", MySqlDbType.VarChar).Value = pass;
                adapter = new MySqlDataAdapter(cmd);//Указание на команду, которая будет выполняться
                var dt = new DataTable();
                adapter.Fill(dt);//Заносятся в кэш значения из бд
                if (dt.Rows.Count > 0)
                {
                    cmd.Dispose();
                    dt.Clear();
                    cmd = new MySqlCommand("SELECT login, employees.id_department from employees join departments on (employees.id_department=departments.id_department) where BINARY login=leader and BINARY login=@log", con);
                    cmd.Parameters.Add("@log", MySqlDbType.VarChar).Value = login;
                    adapter = new MySqlDataAdapter(cmd);//Указание на команду, которая будет выполняться
                    adapter.Fill(dt);//Заносятся в кэш значения из бд

                    if (dt.Rows.Count > 0)
                    {
                        View.LeaderWindow leaderWindow = new View.LeaderWindow();
                        leaderWindow.Show();
                    }
                    else
                    {
                        View.UserWindow userWindow = new View.UserWindow();
                        userWindow.Show();
                    }
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Не удалось осуществить вход! Проверьте логин и пароль!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось подключиться к БД!");
            }
            finally
            { con.Close(); }
        }
    }
}
