namespace Apeek.ViewModels.Models
{
    public interface IGoodViewModel
    {
        int GoodId { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        double Price { get; set; }
        double PricePerWeek { get; set; }
        double PricePerMonth { get; set; }
        bool RentPeriodDay { get; set; }
        bool RentPeriodWeek { get; set; }
        bool RentPeriodMonth { get; set; }
        bool AgreeToDeliver { get; set; }
        bool AgreeToShareImmediately { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }
        string Location { get; set; }
        string Type { get; set; }
        double Deposit { get; set; }
        int MinimumRentPeriod { get; set; }

    }
}