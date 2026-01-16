using lab1.Models;


namespace lab1.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty; 
        public int ProductId { get; set; }
        public Product? Product { get; set; }  
    }
    

}
