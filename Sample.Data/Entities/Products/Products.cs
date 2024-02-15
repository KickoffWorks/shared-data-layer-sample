using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sample.Data.Entities;

[Table("products", Schema = "sample")]
public class Products
{
    [Key] 
    public Guid Id { get; set; }
    
    public string Name { get; set; }
}