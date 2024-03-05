
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmazRON.Models
{
    public class Review
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Continutul review-ului este obligatoriu")]
        public string Content { get; set; }
        public DateTime Date { get; set; }

        [Range(1, 5, ErrorMessage = "Rating-ul trebuie sa fie intre 1 si 5.")]
        [Required(ErrorMessage = "Ratingul review-ului este obligatoriu")]
        public int Rating {  get; set; }


        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public int? ProductId { get; set; }
        public virtual Product? Product { get; set; }

    }

}
