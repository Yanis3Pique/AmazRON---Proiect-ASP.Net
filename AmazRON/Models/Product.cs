using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmazRON.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [StringLength(100, ErrorMessage = "Titlul nu poate avea mai mult de 100 de caractere")]
        [MinLength(5, ErrorMessage = "Titlul trebuie sa aiba mai mult de 5 caractere")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Descrierea produsului este obligatorie")]
        [StringLength(100000, ErrorMessage = "Descrierea nu poate avea mai mult de 100000 de caractere")]
        [MinLength(10, ErrorMessage = "Descrierea trebuie sa aiba mai mult de 10 caractere")]
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public float Price { get; set; }
        public double? Rating { get; set; }
        public string Photo { get; set; }

        public int Stoc {  get; set; }

        //Acceptat de admin
        public bool Pending { get; set; }
        public bool Accepted { get; set; }


        //COLABORATORUL CARE A ADAUGAT
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        //CATEGORY
        [Required(ErrorMessage = "Categoria este obligatorie")]
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public virtual ICollection<UserProduct>? UserProducts { get; set; }

        public virtual ICollection<Review>? Reviews { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Categ { get; set; }
    }

}