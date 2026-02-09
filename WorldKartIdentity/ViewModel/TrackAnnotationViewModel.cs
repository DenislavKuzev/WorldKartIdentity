using WorldKartIdentity.Database;

namespace WorldKartIdentity.ViewModel
{
    public class TrackAnnotationViewModel
    {

        public int TrackId { get; set; }

        public string AnnotationJson { get; set; } = null!;

    }
}
