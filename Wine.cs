using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Winery
{
    public class Wine
    {
        public Wine()
        {
            Containers = new HashSet<Container>();
        }

        public enum WineType
        {
            Rose,
            White,
            Red,
            Sparkling,
            Orange
        }

        [Key]
        [Required]
        public string WineID { get; set; }

        [Required]
        public WineType Type { get; set; }

        [Required]
        [Range(0, 100)]
        public int Sweetness { get; set; }

        [Required]
        [Range(0, 200)]
        public int SulfurLevel { get; set; }

        [Range(0, 10)]
        public int Pressure { get; set; }

        [Required]
        public string Vineyard { get; set; }

        [Required]
        public string Region { get; set; }

        [Range(0, 100)]
        public int AlcoholContent { get; set; }

        public string Notes { get; set; }

        // Navigation property to Containers
        public virtual ICollection<Container> Containers { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Wine ID: {WineID}");
            sb.AppendLine($"Type: {Type}");
            sb.AppendLine($"Sweetness: {Sweetness}");
            sb.AppendLine($"Sulfur Level: {SulfurLevel}");
            sb.AppendLine($"Pressure: {Pressure}");
            sb.AppendLine($"Vineyard: {Vineyard}");
            sb.AppendLine($"Region: {Region}");
            sb.AppendLine($"Alcohol Content: {AlcoholContent}");
            sb.AppendLine($"Notes: {Notes}");
            sb.AppendLine("Containers:");

            foreach (var container in Containers)
            {
                sb.AppendLine(container.ToString());
            }

            return sb.ToString();
        }
    }

}
