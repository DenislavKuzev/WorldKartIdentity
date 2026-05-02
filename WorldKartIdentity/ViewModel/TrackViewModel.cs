using WorldKartIdentity.Database;

namespace WorldKartIdentity.ViewModel
{
    public class TrackViewModel
    {
        public int Id { get; set; }
        public IFormFile? RoutePictureFile { get; set; }

        public IFormFile? PhotographFile { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } 

        public string Location { get; set; } 

        public string TelNumber { get; set; } 

        public string Email { get; set; } 

        public string? GoogleMapsLink { get; set; }

        public string Worktime { get; set; } 

        public string RoutePictureBase64 { get; set; } = string.Empty;

        public string PhotographBase64 { get; set; } = string.Empty;

        public int Length { get; set; }

        public int TurnCount { get; set; }

        public int Width { get; set; }

        public int LikesCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }

        public ICollection<TrackTrajectoryViewModel> Trajectories { get; set; } = new List<TrackTrajectoryViewModel>();

        public TrackViewModel()
        {
            RoutePictureFile = null!;
            PhotographFile = null!;
            Length = 0;
            Description = null!;
            Location = null!;
            TelNumber = null!;
            Email = null!;
            GoogleMapsLink = null!;
            Worktime = null!;
        }

        public TrackViewModel(Track track)
        {
            Id = track.Id;
            Name = track.Name;
            RoutePictureBase64 = track.RoutePicture;
            PhotographBase64 = track.Photograph;
            Length = track.Length;
            Description = track.Description;
            Location = track.Location;
            TelNumber = track.TelNumber;
            Email = track.Email;
            TurnCount = track.TurnCount;
            LikesCount = track.Likes.Count;
            Width = track.Width;
            GoogleMapsLink = track.GoogleMapsLink;
            Worktime = track.Worktime;
        }

        public static Track TrackVMToTrack(TrackViewModel trackVM)
        {
            Track track = new Track();
            track.Id = trackVM.Id;
            track.Name = trackVM.Name;
            track.RoutePicture = trackVM.RoutePictureBase64;
            track.Photograph = trackVM.PhotographBase64;
            track.Length = trackVM.Length;
            track.Description = trackVM.Description;
            track.Location = trackVM.Location;
            track.TelNumber = trackVM.TelNumber;
            track.Worktime = trackVM.Worktime;
            track.Email = trackVM.Email;
            track.TurnCount = trackVM.TurnCount;
            track.Width = trackVM.Width;
            track.GoogleMapsLink = trackVM.GoogleMapsLink;
            return track;
        }
        
        public static TrackViewModel TrackToTrackVM(Track track)
        {
            TrackViewModel trackVM = new TrackViewModel();
            trackVM.Id = track.Id;
            trackVM.Name = track.Name;
            trackVM.RoutePictureBase64 = track.RoutePicture;
            trackVM.PhotographBase64 = track.Photograph;
            trackVM.Length = track.Length;
            trackVM.TelNumber = track.TelNumber;
            trackVM.Worktime = track.Worktime;
            trackVM.Location = track.Location;
            trackVM.Description = track.Description;
            trackVM.Email = track.Email;
            trackVM.TurnCount = track.TurnCount;
            trackVM.LikesCount = track.Likes.Count;
            trackVM.Width = track.Width;
            trackVM.GoogleMapsLink = track.GoogleMapsLink;
            return trackVM;
        }
    }
}
