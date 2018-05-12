using System.ComponentModel.DataAnnotations;

namespace ChooseYourGame.Models
{
    public class BlogTag
    {        
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}