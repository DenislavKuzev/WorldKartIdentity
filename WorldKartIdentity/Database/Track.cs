using System;
using System.Collections.Generic;

namespace WorldKartIdentity.Database;

public class Track
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string TelNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? GoogleMapsLink { get; set; }

    public string Worktime { get; set; } = null!;

    public string Picture { get; set; } = null!;

    public int Length { get; set; }

    public ICollection<TrackLike> Likes { get; set; } = new List<TrackLike>();
}
