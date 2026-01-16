
using lab1.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace lab1.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [JsonIgnore] // Ngăn vòng lặp JSON
        public List<Product> Products { get; set; } = new List<Product>(); 
    }

}
