using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PinPayments.Infrastructure;
namespace PinPayments.Actions
{
    public class ChargeSearch
    {
        [JsonProperty("query")]
        public string Query{get;set;}
        [JsonProperty("start_date")]
        public DateTime? StartDate{get;set;}
        [JsonProperty("end_date")]
        public DateTime? EndDate{get;set;}
        [JsonIgnore]
        public ChargeSearchSortEnum Sort {get;set;}
        [JsonProperty("sort")]
        private string sort
        {
            get
            {
                switch (Sort)
                {
                    case ChargeSearchSortEnum.Amount:
                        return "amount";
                    case ChargeSearchSortEnum.Description:
                        return "description";
                    case ChargeSearchSortEnum.Created:
                    default:
                        return "created_at";
                }
            }
        }
        [JsonIgnore]
        public SortDirectionEnum SortDirection { get; set; }
        [JsonProperty("direction")]
        private int direction
        {
            get
            {
                if (SortDirection == SortDirectionEnum.Ascending)
                {
                    return 1;
                }
                return -1;
            }
        }
    }
    public enum ChargeSearchSortEnum
    {
        Created, Description, Amount
    }
    public enum SortDirectionEnum
    {
        Ascending = 1, Descending =-1
    }
}
