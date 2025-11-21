using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity; // Добавь этот using
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using Maket.Classes; // Для доступа к Client

namespace Maket; // Убедимся, что мы в основном namespace

public partial class ProjectWindow : Window
{
    private readonly string _connectionString =
        "Server=localhost;Database=it_project_management_simple;User Id=root;Password=;";

    // Хранение всех проектов для быстрого доступа
    private List<Project> _allProjects = new();

    // Хранение всех клиентов для ComboBox
    private List<Client> _allClients = new();

    // Хранение всех сотрудников для ComboBox
    private List<Employee> _allEmployees = new();

    public ProjectWindow()
    {
        InitializeComponent();
        LoadData();
    }

    public void CloseWindow(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    public void RefreshButtonClick(object sender, RoutedEventArgs e)
    {LoadData();
    }

    private async void LoadData()
    {
        var projects = new List<Project>();
        var clients = new List<Client>();
        var employees = new List<Employee>(); // Используем существующий класс

        try
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Загрузка проектов
                string projectQuery = "SELECT * FROM project_details ORDER BY created_at DESC";
                using (var command = new MySqlCommand(projectQuery, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var proj = new Project
                            {
                                ProjectId = reader.GetInt32("project_id"),
                                ProjectName = reader.GetString("project_name"),
                                ClientName = reader.GetString("client_name"),
                                ClientPhone = reader.GetString("client_phone"),
                                ProjectManagerName = reader.GetString("project_manager_name"), // Полное имя из представления
                                Phase = reader.GetString("phase"),
                                EndDate = reader.GetDateTime("end_date"),
                                CreatedAt = reader.GetDateTime("created_at")
                            };
                            projects.Add(proj);
                        }
                    }
                }

                // Загрузка клиентов
                string clientQuery = "SELECT client_id, company_name, phone FROM clients ORDER BY company_name";
                using (var command = new MySqlCommand(clientQuery, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var client = new Client
                            {
                                ClientId = reader.GetInt32("client_id"),
                                CompanyName = reader.GetString("company_name"),
                                Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? "" : reader.GetString("phone")
                            };
                            clients.Add(client);
                        }
                    }
                }

                // Загрузка сотрудников (для менеджеров)
                // Используем данные из брифинга/таблицы employees для фильтрации
                // ВНИМАНИЕ: Твой класс Employee ожидает EmployeeId как string
                string employeeQuery = @"SELECT employee_id, first_name, last_name, position, department 
                                         FROM employees 
                                         WHERE position LIKE '%менеджер%' OR position LIKE '%руководитель%' 
                                         OR last_name IN ('Шевченко', 'Мазалова', 'Семеняка', 'Савельев') 
                                         ORDER BY last_name, first_name";
                using (var command = new MySqlCommand(employeeQuery, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Создаём экземпляр существующего класса Employee
                            var emp = new Employee
                            {
                                // Преобразуем int в string для EmployeeId
                                EmployeeId = reader.GetInt32("employee_id").ToString(),
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
            Console.WriteLine($"Ошибка загрузки данных: {ex.Message}");
            return;
        }

        // Сохраняем списки
        _allProjects = projects;
        _allClients = clients;
        _allEmployees = employees;

        // Обновляем UI
        ProjectsListBox.ItemsSource = projects;

        // Заполняем ComboBox'ы
        ClientComboBox.ItemsSource = clients;
        ManagerComboBox.ItemsSource = employees; // Теперь привязывается к списку Employee

        // Устанавливаем значение по умолчанию для фазы
        if (PhaseComboBox.Items.Count > 0)
        {
            PhaseComboBox.SelectedIndex = 0;
        }
    }

  public void AddProject(object sender, RoutedEventArgs e)
{
    try
    {
        string projectName = ProjectNameTextBox.Text.Trim();
        if (string.IsNullOrEmpty(projectName))
        {
            Console.WriteLine("Ошибка: Название проекта обязательно.");
            return;
        }

        var selectedClient = ClientComboBox.SelectedItem as Client;
        var selectedManager = ManagerComboBox.SelectedItem as Employee;
        string phase = PhaseComboBox.SelectedItem?.ToString() ?? "Консультация";
        // Исправленная строка: проверяем, что SelectedDate не null, иначе используем DateTime.Now.AddDays(30)
      //  DateTime endDate = EndDatePicker.SelectedDate ?? DateTime.Now.AddDays(30);

        if (selectedClient == null || selectedManager == null)
        {
            Console.WriteLine("Ошибка: Выберите клиента и менеджера.");
            return;
        }

        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string query = @"INSERT INTO projects (project_name, client_id, project_manager_id, phase, end_date, created_at) 
                             VALUES (@project_name, @client_id, @manager_id, @phase, @end_date, NOW())";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@project_name", projectName);
                command.Parameters.AddWithValue("@client_id", selectedClient.ClientId);
                command.Parameters.AddWithValue("@manager_id", selectedManager.EmployeeId);
                command.Parameters.AddWithValue("@phase", phase);
             //   command.Parameters.AddWithValue("@end_date", endDate);

                command.ExecuteNonQuery();
            }
        }

        Console.WriteLine("Проект успешно добавлен.");
        LoadData(); // Обновляем список

        // Очищаем поля
        ProjectNameTextBox.Text = "";
        ClientComboBox.SelectedIndex = -1;
        ManagerComboBox.SelectedIndex = -1;
        PhaseComboBox.SelectedIndex = 0;
        EndDatePicker.SelectedDate = null;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при добавлении проекта: {ex.Message}");
    }
}}

   
