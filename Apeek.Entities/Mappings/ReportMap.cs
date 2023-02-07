using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class ReportMap : ClassMap<Report>
    {
        public ReportMap()
        {
            Table("s_report");
            Id(x => x.Id, "report_id");
            Map(x => x.Xml, "xml");
        }
    }
}