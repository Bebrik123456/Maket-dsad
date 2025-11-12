using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.VisualBasic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Input;
using MySql.Data.MySqlClient;
namespace Maket;

public partial class LoginWindow : Window
{
    // Это временная заглушка. На экзамене счётчик должен быть в БД.
    private int ExeptionCount = 0;
    // Добавим переменную для хранения текущего логина, чтобы сбрасывать счётчик при смене логина
    private string _currentAttemptedLogin = "";

    public LoginWindow()
    {
        InitializeComponent();
    }
    
    
    
    private async void LoginButton(object sender, RoutedEventArgs e)
    {
        await MailCheck();
    }
    private async Task MailCheck()
    {
        string email = PochtaTextbox.Text;
        try
        {
            if (ExeptionCount < 3)
            {
                var password = PasswordTextBox.Text;
                var b = password.ToCharArray();

                if (b.Length >= 1)
                {
                    string stringconnection =
                        "Server=localhost;Database=it_project_management_simple;User Id=root;Password=;";
                    string query = "SELECT login,password_hash  FROM users WHERE `login` = @username AND `password_hash` = @password";
                    string passwordDB = PasswordTextBox.Text;
                    string loginDB = PochtaTextbox.Text;

                    MySqlConnection con = new MySqlConnection(stringconnection);
                    await con.OpenAsync();

                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@username", loginDB);
                    cmd.Parameters.AddWithValue("@password", passwordDB);

                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        Hide();
                        MainWindow empty = new MainWindow();
                        empty.Show();
                        this.Close();
                    }
                    else
                    {  Label.Content = "Неправильно введен логин или пароль";
                        ExeptionCount++;
                    }
                }
                else
                {
                    Label.Content = "Неправильно введен логин или пароль";
                    ExeptionCount++;
                }
            }
            else
            {
              Console.WriteLine("Количество ошибок больше допустимого");
              this.Close();
            }
        }
        catch (Exception ex)
        {Console.WriteLine(ex.Message);
        }
    }


    private void PasswordTextBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        PasswordTextBox.Text = "";
    }
}