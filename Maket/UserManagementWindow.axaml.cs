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
using System.Collections.Generic;
using System.Data;

namespace Maket;



public partial class UserManagementWindow : Window
{
    private readonly string _connectionString = "Server=localhost;Database=it_project_management_simple;User Id=root;Password=;"; 


    public UserManagementWindow()
    {
        InitializeComponent();
        LoadData();
    }



    public void CloseWindow(object sender, RoutedEventArgs e)
    {
        this.Close();
    }


    private async void LoadData()
    {
        var employees = new List<Employee>(); // Создаём список для хранения сотрудников

        try
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync(); // Открываем соединение асинхронно

                string query = "SELECT employee_id, first_name, last_name, position, department FROM Employees ";

                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync()) // Выполняем запрос асинхронно
                    {
                        while (await reader.ReadAsync()) 
                        {
                            var emp = new Employee
                            {
                                //  EmployeeId = reader.GetInt32("employee_id"),
                                FirstName = reader.GetString("first_name"),
                                LastName = reader.GetString("last_name"),
                                Position = reader.GetString("position"),
                                Department = reader.GetString("department")
                            };
                            employees.Add(emp);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ну вот и поиграли: {ex.Message}");

        }


        EmployeesListBox.ItemsSource = employees;
    }


    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public void RegUser(object sender, RoutedEventArgs e)
    {
       
        try
        {
             string stringconnection = "Server=localhost;Database=it_project_management_simple;User Id=root;Password=;"; 
             string query = "INSERT INTO users (login,password_hash) VALUES (@login,@password_hash)";
             string login = LoginTextBox.Text;
             string password = PasswordBox.Text;
             
             MySqlConnection connection = new MySqlConnection(stringconnection);
             connection.Open();
             MySqlCommand command = new MySqlCommand(query, connection);
             command.Parameters.AddWithValue("@login", login);
             command.Parameters.AddWithValue("@password_hash", password);
             command.ExecuteNonQuery();
             connection.Close();
             
             Console.WriteLine("Пользователь успешно добавлен ");
             LoginTextBox.Text = "";
             PasswordBox.Text = "";
             
             LoadData();

        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }

    }


    public void UpdateButton(object sender, RoutedEventArgs e)
    {
        
      //  LoadData();
      UserManagementWindow userManagementWindow = new UserManagementWindow();
      userManagementWindow.Show();
      this.Close();
    }

}

