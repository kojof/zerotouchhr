using System.ComponentModel.DataAnnotations;

namespace ZeroTouchHR.Models
{
    public class Employee
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string FName { get; set; }
        public string LName { get; set; }

        public string PhoneNumber { get; set; }

        public string title { get; set; }
    }
}
