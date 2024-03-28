using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace TaskHub.Models
{
    public class UserTask
    {
        //Key is an annotation that tells that this will be the Primary key
        [Key]
        public Guid TaskId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }

        //ValidateNever describes that the property should not be included in validation
        [ValidateNever]
        public string Status { get; set; }

        public string? AssignedToId { get; set; }

        [ValidateNever]
        public ApplicationUser? AssignedTo { get; set; }
    }
}
