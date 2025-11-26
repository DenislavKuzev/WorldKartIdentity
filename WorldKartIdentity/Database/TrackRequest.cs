using System;
using System.Collections.Generic;

namespace WorldKartIdentity.Database;

public partial class TrackRequest
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? LocationUrl { get; set; }

    public string Country { get; set; } = null!;
}
