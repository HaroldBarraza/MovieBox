using System.ComponentModel.DataAnnotations.Schema;

namespace MovieBox.Models;

[Table("movies")]
public class Movie
{
    [Column("id")]
    public int Id { get; set; }
    [Column("title")]
    public string Title { get; set; }
    [Column("description")]
    public string Description { get; set; }
}