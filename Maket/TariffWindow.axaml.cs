using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity; // Добавлено
using Avalonia.Controls.Selection; // Добавлено для SelectionChangedEventArgs
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using Maket.Classes;

namespace Maket;

public partial class TariffWindow : Window
{
    private readonly string _connectionString = "Server=localhost;Database=it_project_management_simple;User Id=root;Password=;";

    // Хранение всех проектов для быстрого доступа
    private List<Project> _allProjects = new();

    // Хранение всех клиентов для ComboBox
    private List<Client> _allClients = new();

    // Хранение всех сотрудников для ComboBox
    private List<Employee> _allEmployees = new();

    // Текущий выбранный проект для редактирования/удаления
    private Project _selectedProjectForEdit = null;

    public TariffWindow()
    {
        InitializeComponent();
        LoadData();
    }

    public void LoadData123(object sender, RoutedEventArgs e)
    {
        LoadData();
    }



    public void CloseWindow(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private async void LoadData()
    {
        var projects = new List<Project>();
        var clients = new List<Client>();
        var employees = new List<Employee>();

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
                                ClientName = reader.GetString("client_name"), // Для отображения
                                ClientPhone = reader.GetString("client_phone"), // Для отображения
                                ProjectManagerName = reader.GetString("project_manager_name"), // Для отображения
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
                string employeeQuery = "SELECT employee_id, first_name, last_name FROM employees WHERE position LIKE '%менеджер%' OR position LIKE '%руководитель%' OR last_name IN ('Шевченко', 'Мазалова', 'Семеняка', 'Савельев') ORDER BY last_name, first_name";
                using (var command = new MySqlCommand(employeeQuery, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var emp = new Employee
                            {
                                EmployeeId = reader.GetInt32("employee_id").ToString(), // Преобразуем в строку
                                FirstName = reader.GetString("first_name"),
                                LastName = reader.GetString("last_name")
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
        ManagerComboBox.ItemsSource = employees;

        // Устанавливаем значение по умолчанию для фазы
        if (PhaseComboBox.Items.Count > 0)
        {
            PhaseComboBox.SelectedIndex = 0;
        }

        // Сбрасываем выбор и поля при загрузке
        _selectedProjectForEdit = null;
        ProjectsListBox.SelectedIndex = -1;
        ClearInputFields();
    }

    // Метод для очистки полей ввода
    private void ClearInputFields()
    {
        ProjectNameTextBox.Text = "";
        ClientComboBox.SelectedIndex = -1;
        ManagerComboBox.SelectedIndex = -1;
        PhaseComboBox.SelectedIndex = 0; // Установить "Консультация" по умолчанию
        EndDatePicker.SelectedDate = null;
    }

    // Обработчик выбора элемента в ListBox
    private void OnProjectSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedProject = ProjectsListBox.SelectedItem as Project;
        if (selectedProject != null)
        {
            _selectedProjectForEdit = selectedProject;

            // Заполняем поля данными выбранного проекта
            ProjectNameTextBox.Text = selectedProject.ProjectName;

            // Найти соответствующего клиента в ComboBox
            var matchingClient = _allClients.FirstOrDefault(c => c.CompanyName == selectedProject.ClientName);
            ClientComboBox.SelectedItem = matchingClient;

            // Найти соответствующего менеджера в ComboBox
            // Тут нужно сопоставить полное имя менеджера из проекта с объектом Employee
            var matchingManager = _allEmployees.FirstOrDefault(emp => emp.ToString() == selectedProject.ProjectManagerName);
            ManagerComboBox.SelectedItem = matchingManager;

            // Установить фазу
            var phaseIndex = PhaseComboBox.Items.Cast<ComboBoxItem>()
                                               .ToList()
                                               .FindIndex(item => item.Content.ToString() == selectedProject.Phase);
            if (phaseIndex != -1)
            {
                PhaseComboBox.SelectedIndex = phaseIndex;
            }
            else
            {
                PhaseComboBox.SelectedIndex = 0; // Установить "Консультация", если не найдено
            }

            EndDatePicker.SelectedDate = selectedProject.EndDate;
        }
        else
        {
            _selectedProjectForEdit = null;
            ClearInputFields();
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
            string phase = (PhaseComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Консультация";
            DateTimeOffset endDate = EndDatePicker.SelectedDate ?? DateTime.Now.AddDays(30);

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
                    command.Parameters.AddWithValue("@manager_id", int.Parse(selectedManager.EmployeeId)); // Преобразуем строку в int
                    command.Parameters.AddWithValue("@phase", phase);
                    command.Parameters.AddWithValue("@end_date", endDate);

                    command.ExecuteNonQuery();
                }
            }

            Console.WriteLine("Проект успешно добавлен.");
            LoadData(); // Обновляем список и сбрасываем поля
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при добавлении проекта: {ex.Message}");
        }
    }

    public void UpdateProject(object sender, RoutedEventArgs e)
    {
        if (_selectedProjectForEdit == null)
        {
            Console.WriteLine("Ошибка: Не выбран проект для редактирования.");
            return;
        }

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
            string phase = (PhaseComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Консультация";
            DateTimeOffset endDate = EndDatePicker.SelectedDate ?? DateTime.Now.AddDays(30);

            if (selectedClient == null || selectedManager == null)
            {
                Console.WriteLine("Ошибка: Выберите клиента и менеджера.");
                return;
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"UPDATE projects 
                                 SET project_name = @project_name, 
                                     client_id = @client_id, 
                                     project_manager_id = @manager_id, 
                                     phase = @phase, 
                                     end_date = @end_date 
                                 WHERE project_id = @project_id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@project_name", projectName);
                    command.Parameters.AddWithValue("@client_id", selectedClient.ClientId);
                    command.Parameters.AddWithValue("@manager_id", int.Parse(selectedManager.EmployeeId)); // Преобразуем строку в int
                    command.Parameters.AddWithValue("@phase", phase);
                    command.Parameters.AddWithValue("@end_date", endDate);
                    command.Parameters.AddWithValue("@project_id", _selectedProjectForEdit.ProjectId); // ID проекта для обновления

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Проект успешно обновлён.");
                        LoadData(); // Обновляем список
                    }
                    else
                    {
                        Console.WriteLine("Проект не найден или не был изменён.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обновлении проекта: {ex.Message}");
        }
    }

    public void DeleteProject(object sender, RoutedEventArgs e)
    {
        if (_selectedProjectForEdit == null)
        {
            Console.WriteLine("Ошибка: Не выбран проект для удаления.");
            return;
        }

        try
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"DELETE FROM projects WHERE project_id = @project_id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@project_id", _selectedProjectForEdit.ProjectId);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Проект успешно удалён.");
                        LoadData(); // Обновляем список
                    }
                    else
                    {
                        Console.WriteLine("Проект не найден или не был удалён.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении проекта: {ex.Message}");
        }
    }
}