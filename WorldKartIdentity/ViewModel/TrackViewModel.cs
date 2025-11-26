using WorldKartIdentity.Database;

namespace WorldKartIdentity.ViewModel
{
    public class TrackViewModel
    {
        public string Name { get; set; } = string.Empty;

        public string Picture { get; set; } = string.Empty;

        public int Length { get; set; }

        public TrackViewModel(Track track)
        {
            Name = track.Name;
            Picture = track.Picture;
            Length = track.Length;
        }

        public static Track TrackVMToTrack(TrackViewModel trackVM)
        {
            Track track = new Track();
            track.Name = trackVM.Name;
            track.Picture = trackVM.Picture;
            track.Length = trackVM.Length;
            return track;
        }
    }
}
