using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using System.Collections.Generic;
using Maket.Classes;

namespace Maket;

public partial class TariffWindow : Window
{
    // Список тарифов, можно загружать из БД или хранить в коде
    private List<Tariff> _tariffs = new List<Tariff>();

    public TariffWindow()
    {
        InitializeComponent();
        LoadData();
    }

    public void CloseWindow(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    // Метод загрузки данных (в данном случае - из кода)
    private void LoadData()
    {
        // Очищаем старые данные
        _tariffs.Clear();

        // Заполняем данными из файла "Па†©б.pdf"
        _tariffs.Add(new Tariff
        {
            Name = "Малый офис",
            MinComputers = 0,
            MaxComputers = 4,
            FixedPrice = 6000,
            Services = "Удаленное обслуживание\n2 плановых выезда\n10 экстренных"
        });

        _tariffs.Add(new Tariff
        {
            Name = "Популярный тариф",
            MinComputers = 5,
            MaxComputers = 15,
            PricePerUnit = 2000,
            Services = "Удаленное обслуживание\n3 плановых выезда\n10 экстренных"
        });

        _tariffs.Add(new Tariff
        {
            Name = "Премиум",
            MinComputers = 16,
            MaxComputers = 25,
            PricePerUnit = 1600,
            Services = "Удаленное обслуживание\n4 плановых выезда\n15 экстренных"
        });

        _tariffs.Add(new Tariff
        {
            Name = "Корпорация",
            MinComputers = 26,
            MaxComputers = int.MaxValue, // или другое большое число
            PricePerUnit = 1200,
            Services = "Удаленное обслуживание\n5 плановых выезда\n20 экстренных"
        });

        _tariffs.Add(new Tariff
        {
            Name = "Монтаж систем видеонаблюдения",
            MinComputers = 0,
            MaxComputers = 0, // Не зависит от кол-ва компов
            FixedPrice = 15000,
            Services = "Установка камер в помещении и вне помещений.\nМонтаж линий и оборудования.\nУстановка и настройка видеорегистраторов."
        });

        _tariffs.Add(new Tariff
        {
            Name = "Проектирование локальной сети",
            MinComputers = 0,
            MaxComputers = 0,
            FixedPrice = 5000,
            Services = "Прокладка линий.\nУстановка и настройка сетевого оборудования."
        });

        _tariffs.Add(new Tariff
        {
            Name = "Обслуживание серверов",
            MinComputers = 0,
            MaxComputers = 0,
            FixedPrice = 10000,
            Services = "Настройка серверов, настройка систем резервного копирования данных, управление пользователями."
        });

        _tariffs.Add(new Tariff
        {
            Name = "Установка СКУД",
            MinComputers = 0,
            MaxComputers = 0,
            FixedPrice = 50000,
            Services = "Монтаж и настройка систем контроля доступа. Обучение персонала."
        });

        _tariffs.Add(new Tariff
        {
            Name = "Монтаж и настройка АТС",
            MinComputers = 0,
            MaxComputers = 0,
            FixedPrice = 10000,
            Services = "Установка и настройка АТС.\nПодключение дополнительного оборудования."
        });

        // Обновляем ListBox
        TariffsListBox.ItemsSource = _tariffs;

        // Устанавливаем первый элемент как выбранный, чтобы заполнить поля
        if (_tariffs.Count > 0)
        {
            TariffsListBox.SelectedIndex = 0;
        }
    }


    public void RefreshButtonClicked(object sender, RoutedEventArgs e)
    {
        LoadData();
    }

    // Обработчик изменения выбора в ListBox
    private void OnTariffSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedTariff = TariffsListBox.SelectedItem as Tariff;
        if (selectedTariff != null)
        {
            NameTextBox.Text = selectedTariff.Name;
            ComputersRangeTextBox.Text = selectedTariff.MinComputers == 0 && selectedTariff.MaxComputers == 0 ?
                "Не зависит" :
                $"{selectedTariff.MinComputers} - {selectedTariff.MaxComputers}";
            PricePerUnitTextBox.Text = selectedTariff.PricePerUnit > 0 ? selectedTariff.PricePerUnit.ToString() : "Нет";
            FixedPriceTextBox.Text = selectedTariff.FixedPrice > 0 ? selectedTariff.FixedPrice.ToString() : "Нет";
            ServicesTextBox.Text = selectedTariff.Services;
        }
    }
}