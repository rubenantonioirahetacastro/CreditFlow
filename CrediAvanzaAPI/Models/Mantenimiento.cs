using System.ComponentModel.DataAnnotations;

namespace CrediAvanzaAPI.Models
{
    public class Mantenimiento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;
    }
}
