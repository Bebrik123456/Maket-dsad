using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Maket.Classes;

namespace Maket;

public partial class MainWindow : Window
{
    private readonly DataUser _currentUser;

    // Конструктор, принимающий экземпляр DataUser
    public MainWindow(DataUser currentUser)
    {
        // Сохраняем переданный экземпляр в поле класса
        _currentUser = currentUser;

        InitializeComponent();
        LoadData(); // Вызываем метод после инициализации
    }


    public void LoadData()
    {
        // Проверяем, не является ли _currentUser null (хотя в твоём случае вряд ли)
        if (_currentUser != null)
        {
            NameTextBlock.Text = _currentUser.UserName;
            RoleTextBlock.Text = _currentUser.Role; 

            ManageUsersButton.IsVisible = (_currentUser.Role == "Администратор");
        }
        else
        {
            // На случай, если передали null (хотя не должно)
            NameTextBlock.Text = "Ошибка: Данные пользователя отсутствуют";
            RoleTextBlock.Text = "";
            ManageUsersButton.IsVisible = false;
        }
    }
    
    public void TariffWinClick(object sender, RoutedEventArgs e)
    {
    TariffWindow tariffWindow = new TariffWindow(); 
    tariffWindow.Show();
    this.Close();
    }
    
    public void OpenProjectWindow(object sender, RoutedEventArgs e)
    {
        var projectWindow = new TariffWindow();
        projectWindow.Show(); 
        this.Close(); 
    }

    public void ToUserManagment(object sender, RoutedEventArgs e)
    {
        UserManagementWindow userManagementWindow = new UserManagementWindow();
        this.Hide();
        userManagementWindow.Show();
        this.Close();
    }

}