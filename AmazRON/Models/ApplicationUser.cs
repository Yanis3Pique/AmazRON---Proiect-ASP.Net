using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;


// PASUL 1 - useri si roluri 

namespace AmazRON.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Prenume { get; set; }
        public string? Nume { get; set; }

        public virtual ICollection<Review>? Reviews { get; set; }
        public virtual ICollection<Product>? Products { get; set; }
        public virtual ICollection<UserProduct>? UserProducts { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }

    }
}