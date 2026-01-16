using WorldKartIdentity.Database;

namespace WorldKartIdentity.ViewModel
{
    public class TrackRequestViewModel
    {
        public string Name { get; set; } = string.Empty;


        public string LocationUrl { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public TrackRequestViewModel()
        {
            Name = string.Empty;
            LocationUrl = string.Empty;
            Country = string.Empty;
        }

        public TrackRequestViewModel(TrackRequest trackrequest)
        {
            Name = trackrequest.Name;
            LocationUrl = trackrequest.LocationUrl;
            Country = trackrequest.Country;
        }

        public static TrackRequest TrackRequestVMToTrackRequest(TrackRequestViewModel trackrequestVM)
        {
            TrackRequest trackrequest = new TrackRequest();
            trackrequest.Name = trackrequestVM.Name;
            trackrequest.LocationUrl = trackrequestVM.LocationUrl;
            trackrequest.Country = trackrequestVM.Country;
            return trackrequest;
        }
    }
}
