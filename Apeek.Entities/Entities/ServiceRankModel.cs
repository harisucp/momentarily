namespace Apeek.Entities.Entities
{
    public class ServiceRankModel
    {
        [RankField]
        public string RawPrice { get; set; }
        [RankFieldDescription]
        public string Description { get; set; }
        [RankField]
        public string UserServiceImagesCount { get; set; }
    }
}