using Apeek.Entities.Web;
namespace Apeek.Common
{
    public interface IShape<TModel> : IShapeResult
    {
        string ViewName { get; set; }
        TModel ViewModel { get; set; }
        string Layout { get; set; }
        SeoEntry SeoEntry { get; set; }
        PagingInfo PagingInfo { get; set; }
        BrowseEntry BrowseEntry { get; set; }
        string PageName { get; set; }
    }
    public class Shape<TModel> : IShape<TModel>
    {
        public string ViewName { get; set; }
        public TModel ViewModel { get; set; }
        public string Layout { get; set; }
        public SeoEntry SeoEntry { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public BrowseEntry BrowseEntry { get; set; }
        public string PageName { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }
        public Shape()
        {
            SeoEntry = new SeoEntry();
            BrowseEntry = new BrowseEntry();
        }
        public Shape(string viewName, TModel viewModel, SeoEntry seoEntry, PagingInfo pagingInfo = null, string pageName = null)
        {
            if (string.IsNullOrEmpty(pageName))
                pageName = Common.PageName.Other.ToString();
            ViewName = viewName;
            ViewModel = viewModel;
            SeoEntry = seoEntry;
            PagingInfo = pagingInfo;
            PageName = pageName;
            BrowseEntry = new BrowseEntry();
        }
    }
}
