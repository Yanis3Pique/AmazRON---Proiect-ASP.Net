
using System.ComponentModel.DataAnnotations;

namespace AmazRON.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele categoriei este obligatoriu")]
        public string CategoryName { get; set; }

        public virtual ICollection<Product>? Products { get; set; }
    }

}
