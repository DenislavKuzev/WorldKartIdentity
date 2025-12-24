using System;
using System.Collections.Generic;

namespace WorldKartIdentity.Database;

public partial class Track
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Picture { get; set; } = null!;

    public int Length { get; set; }

    public ICollection<TrackLike> Likes { get; set; } = new List<TrackLike>();
}
