namespace WebStore.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100,ErrorMessage ="Name Must Be > 100")]
        [MinLength(4,ErrorMessage = "Name Must Be <= 4")]
        [Display(Name ="Category Name")]
        public string Name { get; set; }

        [Range(1,100,ErrorMessage ="Display Order May Be Between 1-100")]
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }
    }
}
