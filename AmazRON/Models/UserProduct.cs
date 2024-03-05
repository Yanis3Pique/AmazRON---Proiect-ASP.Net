using System.ComponentModel.DataAnnotations.Schema;

namespace AmazRON.Models
{
    public class UserProduct
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Bucati { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public int? ProductId { get; set; }
        public virtual Product? Product { get; set; }
    }
}
