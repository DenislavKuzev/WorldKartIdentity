using System.Reflection.Metadata;
using WorldKartIdentity.Database;

namespace WorldKartIdentity.ViewModel
{
    public class BlogReportViewModel
    {
        public int Id { get; set; }

        public int BlogId { get; set; }
        public string? BlogTitle { get; set; }

        public string? AuthorName { get; set; }

        public string? ReporterName { get; set; }

        public DateTime ReportedOn { get; set; }

        public BlogReportViewModel()
        {
        }

        public BlogReportViewModel(BlogReport report)
        {
            Id = report.Id ?? 0;
            BlogId = report.BlogId;
            BlogTitle = report.Blog?.Title;
            AuthorName = report.Blog?.Author?.UserName;
            ReporterName = report.Reporter.UserName;
            ReportedOn = report.ReportedOn;
        }

        public static BlogReport BlogReportVMToBlogReport(BlogReportViewModel brVM)
        {
            BlogReport report = new BlogReport();
            report.Id = brVM.Id;
            report.BlogId = brVM.BlogId;
            report.ReportedOn = brVM.ReportedOn;
            return report;
        }
    }
}
