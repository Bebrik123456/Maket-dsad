using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Avalonia.Controls.Selection; 
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using Maket.Classes;

namespace Maket;

public partial class TariffWindowOriginale : Window
{
    private readonly string _connectionString =
        "Server=localhost;Database=it_project_management_simple;User Id=root;Password=;"; // Убедись, что БД правильная

    // Хранение всех тарифов
    private List<Tariff> _allTariffs = new();

    // Текущий выбранный тариф для редактирования/удаления
    private Tariff _selectedTariffForEdit = null;

    public TariffWindowOriginale()
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
        var tariffs = new List<Tariff>();

        try
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT tariff_id, tariff_name, min_computers, max_computers, price_per_unit, fixed_price, services FROM tariffs ORDER BY tariff_name";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var tariff = new Tariff
                            {
                                TariffId = reader.GetInt32("tariff_id"),
                                TariffName = reader.GetString("tariff_name"),
                                MinComputers = reader.GetInt32("min_computers"),
                                MaxComputers = reader.GetInt32("max_computers"),
                                PricePerUnit = reader.GetInt32("price_per_unit"),
                                FixedPrice = reader.GetInt32("fixed_price"),
                                Services = reader.IsDBNull(reader.GetOrdinal("services")) ? "" : reader.GetString("services")
                            };
                            tariffs.Add(tariff);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки тарифов: {ex.Message}");
            return;
        }

        _allTariffs = tariffs;
        TariffsListBox.ItemsSource = tariffs;

        // Сбрасываем выбор и поля при загрузке
        _selectedTariffForEdit = null;
        TariffsListBox.SelectedIndex = -1;
        ClearInputFields();
    }

    private void ClearInputFields()
    {
        TariffNameTextBox.Text = "";
        MinComputersTextBox.Text = "";
        MaxComputersTextBox.Text = "";
        PricePerUnitTextBox.Text = "";
        FixedPriceTextBox.Text = "";
        ServicesTextBox.Text = "";
    }

    private void OnTariffSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedTariff = TariffsListBox.SelectedItem as Tariff;
        if (selectedTariff != null)
        {
            _selectedTariffForEdit = selectedTariff;

            // Заполняем поля данными выбранного тарифа
            TariffNameTextBox.Text = selectedTariff.TariffName;
            MinComputersTextBox.Text = selectedTariff.MinComputers.ToString();
            MaxComputersTextBox.Text = selectedTariff.MaxComputers.ToString();
            PricePerUnitTextBox.Text = selectedTariff.PricePerUnit.ToString();
            FixedPriceTextBox.Text = selectedTariff.FixedPrice.ToString();
            ServicesTextBox.Text = selectedTariff.Services;
        }
        else
        {
            _selectedTariffForEdit = null;
            ClearInputFields();
        }
    }

    public void AddTariff(object sender, RoutedEventArgs e)
    {
        try
        {
            string name = TariffNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Ошибка: Название тарифа обязательно.");
                return;
            }

            // Пытаемся распарсить числовые поля
            if (!int.TryParse(MinComputersTextBox.Text, out int minComp) ||
                !int.TryParse(MaxComputersTextBox.Text, out int maxComp) ||
                !int.TryParse(PricePerUnitTextBox.Text, out int pricePerUnit) ||
                !int.TryParse(FixedPriceTextBox.Text, out int fixedPrice))
            {
                Console.WriteLine("Ошибка: Проверьте правильность ввода числовых полей.");
                return;
            }

            string services = ServicesTextBox.Text;

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"INSERT INTO tariffs (tariff_name, min_computers, max_computers, price_per_unit, fixed_price, services)
                                 VALUES (@name, @minComp, @maxComp, @pricePerUnit, @fixedPrice, @services)";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@minComp", minComp);
                    command.Parameters.AddWithValue("@maxComp", maxComp);
                    command.Parameters.AddWithValue("@pricePerUnit", pricePerUnit);
                    command.Parameters.AddWithValue("@fixedPrice", fixedPrice);
                    command.Parameters.AddWithValue("@services", services);

                    command.ExecuteNonQuery();
                }
            }

            Console.WriteLine("Тариф успешно добавлен.");
            LoadData(); // Обновляем список
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при добавлении тарифа: {ex.Message}");
        }
    }

    public void UpdateTariff(object sender, RoutedEventArgs e)
    {
        if (_selectedTariffForEdit == null)
        {
            Console.WriteLine("Ошибка: Не выбран тариф для редактирования.");
            return;
        }

        try
        {
            string name = TariffNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Ошибка: Название тарифа обязательно.");
                return;
            }

            if (!int.TryParse(MinComputersTextBox.Text, out int minComp) ||
                !int.TryParse(MaxComputersTextBox.Text, out int maxComp) ||
                !int.TryParse(PricePerUnitTextBox.Text, out int pricePerUnit) ||
                !int.TryParse(FixedPriceTextBox.Text, out int fixedPrice))
            {
                Console.WriteLine("Ошибка: Проверьте правильность ввода числовых полей.");
                return;
            }

            string services = ServicesTextBox.Text;

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"UPDATE tariffs
                                 SET tariff_name = @name,
                                     min_computers = @minComp,
                                     max_computers = @maxComp,
                                     price_per_unit = @pricePerUnit,
                                     fixed_price = @fixedPrice,
                                     services = @services
                                 WHERE tariff_id = @tariffId";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@minComp", minComp);
                    command.Parameters.AddWithValue("@maxComp", maxComp);
                    command.Parameters.AddWithValue("@pricePerUnit", pricePerUnit);
                    command.Parameters.AddWithValue("@fixedPrice", fixedPrice);
                    command.Parameters.AddWithValue("@services", services);
                    command.Parameters.AddWithValue("@tariffId", _selectedTariffForEdit.TariffId);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Тариф успешно обновлён.");
                        LoadData(); // Обновляем список
                    }
                    else
                    {
                        Console.WriteLine("Тариф не найден или не был изменён.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обновлении тарифа: {ex.Message}");
        }
    }

    public void DeleteTariff(object sender, RoutedEventArgs e)
    {
        if (_selectedTariffForEdit == null)
        {
            Console.WriteLine("Ошибка: Не выбран тариф для удаления.");
            return;
        }

        try
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "DELETE FROM tariffs WHERE tariff_id = @tariffId";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tariffId", _selectedTariffForEdit.TariffId);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Тариф успешно удалён.");
                        LoadData(); // Обновляем список
                    }
                    else
                    {
                        Console.WriteLine("Тариф не найден или не был удалён.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении тарифа: {ex.Message}");
        }
    }

}