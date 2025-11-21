// Classes/Tariff.cs
namespace Maket.Classes
{
    public class Tariff
    {
        public string Name { get; set; }
        public int MinComputers { get; set; }
        public int MaxComputers { get; set; }
        public int PricePerUnit { get; set; } = 0; // Цена за единицу, если применимо
        public int FixedPrice { get; set; } = 0;   // Фиксированная цена, если применимо
        public string Services { get; set; }

        // Метод для отображения в ListBox
        public override string ToString()
        {
            // Можно вернуть просто Name, или Name + Price и т.п.
            return Name;
        }
    }
}