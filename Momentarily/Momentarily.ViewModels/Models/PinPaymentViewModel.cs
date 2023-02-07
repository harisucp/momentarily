using System.Collections.Generic;
using System.Globalization;
using Apeek.ViewModels.Models;
using Apeek.ViewModels.Models.Impl;
using System.Linq;
namespace Momentarily.ViewModels.Models
{
    public class PinPaymentViewModel
    {
        public PinPaymentViewModel()
        {
            Countries = GetAllCountries();
        }
        public GoodRequestViewModel Request { get; set; }
        public PinPaymentPublicKeys PublicKeys { get; set; }
        public List<PinPaymentCardViewModel> Cards { get; set; }
        public PinPaymentCardViewModel SelectedCard { get; set; }
        public int GoodRequestId { get; set; }
        public bool IsNewCard { get; set; }
        public List<string> Countries { get; set; }
        private List<string> GetAllCountries()
        {
            var countryList = new List<string>();
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                try
                {
                    RegionInfo ri = new RegionInfo(ci.LCID);
                    if (!countryList.Contains(ri.EnglishName))
                    {
                        countryList.Add(ri.EnglishName);
                    }
                    //countryList.Add(ri.EnglishName);
                }
                catch
                {
                    // ignored
                }
            }
            return countryList.OrderBy(c=>c).ToList();
        }
    }
}