// Classes/Tariff.cs
namespace Maket.Classes
{
    public class Tariff
    {
        public int TariffId { get; set; }
        public string TariffName { get; set; }
        public int MinComputers { get; set; }
        public int MaxComputers { get; set; }
        public int PricePerUnit { get; set; }
        public int FixedPrice { get; set; }
        public string Services { get; set; }

        public override string ToString()
        {
            return TariffName; // Отображение в ListBox
        }
    }
}