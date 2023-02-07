using System;
namespace Apeek.ViewModels.Dtos
{
    public class UserDto
    {
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Age
        {
            get
            {
                if (DateOfBirth.HasValue)
                {
                    var dob = DateOfBirth.Value.Date;
                    DateTime today = DateTime.Today;
                    int age = today.Year - dob.Year;
                    if (dob > today.AddYears(-age)) 
                        age--;
                    return age;
                }
                return null;
            }
        }
        public string Email { get; set; }
    }
}