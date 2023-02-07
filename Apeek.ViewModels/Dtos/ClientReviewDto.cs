using System;
using System.Collections.Generic;
using Apeek.Entities.Entities;
namespace Apeek.ViewModels.Dtos
{
    public class ClientReviewDto
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public bool IsRoot { get; set; }
        public int UserId { get; set; }
        public int? ServiceId { get; set; }
        public int ClientId { get; set; }
        public UserDto Client { get; set; }
        public UserAccountAssociation ClientAccAssoc { get; set; }
        public string Text { get; set; }
        public DateTime CreateDate { get; set; }
        public List<ClientReviewDto> Answers { get; set; }
        public ClientReviewDto()
        {
            Answers = new List<ClientReviewDto>();
        }
    }
}