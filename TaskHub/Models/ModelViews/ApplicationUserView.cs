using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TaskHub.Models.ModelViews
{
    public class ApplicationUserView
    {
        [ValidateNever]
        public string Id { get; set; }

        //The Remote data annotations will trigger a method called IsEmailInUser on the ApplicationUsers controller to check unique email.
        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailInUse", "ApplicationUsers")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and confirmation do not match.")]
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [NotMapped]
        public string Role { get; set; }
    }
}
