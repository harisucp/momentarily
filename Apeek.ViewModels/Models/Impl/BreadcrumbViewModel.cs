using System.Collections.Generic;
using Apeek.Entities.Web;
namespace Apeek.ViewModels.Models
{
    public class BreadcrumbViewModel
    {
        public BreadcrumbViewModel(List<BreadcrumbsEntry> breadcrumbs, List<BreadcrumbsEntry> breadcrumbsLocation)
        {
            Breadcrumbs = breadcrumbs;
            BreadcrumbsLocation = breadcrumbsLocation;
        }
        public BreadcrumbViewModel(List<BreadcrumbsEntry> breadcrumbs)
        {
            Breadcrumbs = breadcrumbs;
        }
        public List<BreadcrumbsEntry> Breadcrumbs { get; set; }
        public List<BreadcrumbsEntry> BreadcrumbsLocation { get; set; }
    }
}