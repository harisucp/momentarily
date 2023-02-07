using Apeek.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Apeek.Entities.Entities;

namespace Momentarily.ViewModels.Models
{
    public class MomentarilyRegisterModel:RegisterModel, IRegisterModel
    {
        //[Required(ErrorMessage = "Date of birth is required.")]
        //[Display(Name = "Date of birthday")]
        ////[DisplayFormat(DataFormatString = "{0:mm/dd/yyyy}", ApplyFormatInEditMode = true)]
        //public DateTime DateOfBirthday { get; set; }
        public List<Countries> countries { get; set; }
    }
    public class GmailToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }
        [JsonProperty("id_token")]
        public string IdToken { get; set; }
    }
    public class UserProfiles
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("verified_email")]
        public bool VerifiedEmail { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("given_name")]
        public string GivenName { get; set; }
        [JsonProperty("family_name")]
        public string FamilyName { get; set; }
        [JsonProperty("link")]
        public string Link { get; set; }
        [JsonProperty("picture")]
        public string Picture { get; set; }
        [JsonProperty("gender")]
        public string Gender { get; set; }
        [JsonProperty("locale")]
        public string Locale { get; set; }
        [JsonProperty("birth_date")]
        public DateTime? BirthDate { get; set; }
    }
}
