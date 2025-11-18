// Classes/Employee.cs
namespace Maket.Classes
{
    public class Employee
    {
        public string EmployeeId { get; set; } // Изменили тип на string
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        // Переопределяем ToString для отображения в ListBox
        public override string ToString()
        {
            // Можно настроить формат, как нравится. Например, "Фамилия Имя (Должность)"
            return $"{LastName} {FirstName} ({Position})";
        }
    }
}