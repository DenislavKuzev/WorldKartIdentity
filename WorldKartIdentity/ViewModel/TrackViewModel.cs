using WorldKartIdentity.Database;

namespace WorldKartIdentity.ViewModel
{
    public class TrackViewModel
    {
        public IFormFile? PictureFile { get; set; }
        public string Name { get; set; } = string.Empty;

        public string PictureBase64 { get; set; } = string.Empty;

        public int Length { get; set; }

        public TrackViewModel()
        {
            PictureFile = null!;
            Length = 0;
        }

        public TrackViewModel(Track track)
        {
            Name = track.Name;
            PictureBase64 = track.Picture;
            Length = track.Length;
        }

        public static Track TrackVMToTrack(TrackViewModel trackVM)
        {
            Track track = new Track();
            track.Name = trackVM.Name;
            track.Picture = trackVM.PictureBase64;
            track.Length = trackVM.Length;
            return track;
        }
        
        public static TrackViewModel TrackToTrackVM(Track track)
        {
            TrackViewModel trackVM = new TrackViewModel();
            trackVM.Name = track.Name;
            trackVM.PictureBase64 = track.Picture;
            trackVM.Length = track.Length;
            return trackVM;
        }
    }
}
