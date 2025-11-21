using System;

namespace Maket.Classes
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public string ProjectManagerName { get; set; }
        public string Phase { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }

        // Переопределяем ToString() для корректного отображения в ListBox
        public override string ToString()
        {
            return ProjectName; // Или можно сделать более информативный формат
            // Например: $"{ProjectName} ({ClientName}) - {Phase}"
        }
    }
}