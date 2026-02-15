using WorldKartIdentity.Database;

namespace WorldKartIdentity.ViewModel
{
    public class TrackRequestViewModel
    {
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; }
        public string? LocationUrl { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public TrackRequestViewModel()
        {
            Id = 0;
            Name = string.Empty;
            LocationUrl = string.Empty;
            Country = string.Empty;
        }

        public TrackRequestViewModel(TrackRequest trackrequest)
        {
            Id = trackrequest.Id;
            Name = trackrequest.Name;
            LocationUrl = trackrequest.LocationUrl;
            Country = trackrequest.Country;
        }

        public static TrackRequest TrackRequestVMToTrackRequest(TrackRequestViewModel trackrequestVM)
        {
            TrackRequest trackrequest = new TrackRequest();
            trackrequestVM.Id = trackrequest.Id;
            trackrequest.Name = trackrequestVM.Name;
            trackrequest.LocationUrl = trackrequestVM.LocationUrl;
            trackrequest.Country = trackrequestVM.Country;
            return trackrequest;
        }
    }
}
