using System.ComponentModel.DataAnnotations;

namespace MovieBox.Models
{
    public class Comment
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        public string UserName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El comentario es requerido")]
        [StringLength(500, ErrorMessage = "El comentario no puede exceder 500 caracteres")]
        public string Text { get; set; } = string.Empty;
        
        [Range(1, 5, ErrorMessage = "La calificación debe ser entre 1 y 5 estrellas")]
        public int Rating { get; set; }
        
        //Usar DateTime.UtcNow en lugar de DateTime.Now
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public int HelpfulVotes { get; set; }
        public int TotalVotes { get; set; }
        public int MovieId { get; set; }

        public double HelpfulScore => TotalVotes > 0 ? (double)HelpfulVotes / TotalVotes * 100 : 0;
    }
}