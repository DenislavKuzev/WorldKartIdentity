using System.Text.RegularExpressions;
using System.Web;
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

        public static string ToShareLink(string embedUrl)
        {
            string decoded = HttpUtility.UrlDecode(embedUrl);

            var latMatch = Regex.Match(decoded, @"!3d(-?\d+\.\d+)");
            var lngMatch = Regex.Match(decoded, @"!2d(-?\d+\.\d+)");

            if (latMatch.Success && lngMatch.Success)
            {
                double lat = double.Parse(latMatch.Value.Replace("!3d", ""),
                    System.Globalization.CultureInfo.InvariantCulture);
                double lng = double.Parse(lngMatch.Value.Replace("!2d", ""),
                    System.Globalization.CultureInfo.InvariantCulture);

                return $"https://www.google.com/maps?q={lat},{lng}";
            }

            throw new FormatException("Could not extract coordinates from embed URL.");
        }
    }
}
