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
using Maket.Classes;
using MySql.Data.MySqlClient;
namespace Maket;

public partial class LoginWindow : Window
{
    private int ExeptionCount = 0;
    
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
                    string stringconnection = "Server=localhost;Database=it_project_management_simple;User Id=root;Password=;";
                    string query = "SELECT login,password_hash,role  FROM users WHERE `login` = @username AND `password_hash` = @password ";
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
                        reader.Read();
                        string userRole = reader["role"].ToString(); 
                        Console.WriteLine(userRole);
                        DataUser loadedUserData = await LoadUserDataAndSetSession(loginDB, userRole);

                        if (loadedUserData != null) 
                        {
                            Hide();
                            MainWindow mainWindow = new MainWindow(loadedUserData);
                            mainWindow.Show();
                            this.Close();
                        }
                        else
                        {
                            Console.WriteLine("Ошибка серьезная ошибка");
                            this.Close();
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
        {
            Console.WriteLine(ex.Message);
            Label.Content = "Ошибка на стороне сервера";
        }
    }


    public async Task<DataUser> LoadUserDataAndSetSession(string loginDB,string role)
    {
        DataUser user = new DataUser();
        user.UserName = loginDB;
        user.Role = role;
        return user; 
    }


}