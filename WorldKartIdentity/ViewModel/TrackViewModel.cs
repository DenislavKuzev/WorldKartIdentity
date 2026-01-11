using WorldKartIdentity.Database;

namespace WorldKartIdentity.ViewModel
{
    public class TrackViewModel
    {
        public int Id { get; set; }
        public IFormFile? PictureFile { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } 

        public string Location { get; set; } 

        public string TelNumber { get; set; } 

        public string Email { get; set; } 

        public string? GoogleMapsLink { get; set; }

        public string Worktime { get; set; } 

        public string PictureBase64 { get; set; } = string.Empty;

        public int Length { get; set; }


        public int LikesCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }

        public TrackViewModel()
        {
            PictureFile = null!;
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
            PictureBase64 = track.Picture;
            Length = track.Length;
            Description = track.Description;
            Location = track.Location;
            TelNumber = track.TelNumber;
            Email = track.Email;
            GoogleMapsLink = track.GoogleMapsLink;
            Worktime = track.Worktime;
        }

        public static Track TrackVMToTrack(TrackViewModel trackVM)
        {
            Track track = new Track();
            track.Id = trackVM.Id;
            track.Name = trackVM.Name;
            track.Picture = trackVM.PictureBase64;
            track.Length = trackVM.Length;
            track.Description = trackVM.Description;
            track.Location = trackVM.Location;
            track.TelNumber = trackVM.TelNumber;
            track.Worktime = trackVM.Worktime;
            track.Email = trackVM.Email;
            track.GoogleMapsLink = trackVM.GoogleMapsLink;
            return track;
        }
        
        public static TrackViewModel TrackToTrackVM(Track track)
        {
            TrackViewModel trackVM = new TrackViewModel();
            trackVM.Id = track.Id;
            trackVM.Name = track.Name;
            trackVM.PictureBase64 = track.Picture;
            trackVM.Length = track.Length;
            trackVM.TelNumber = track.TelNumber;
            trackVM.Worktime = track.Worktime;
            trackVM.Location = track.Location;
            trackVM.Description = track.Description;
            trackVM.Email = track.Email;
            trackVM.GoogleMapsLink = track.GoogleMapsLink;
            return trackVM;
        }
    }
}
