using System;
using System.Collections.Generic;

namespace WebApplication1.Models.Db;

public partial class Labstudent
{
    public string StdId { get; set; } = null!;

    public string? StdPassword { get; set; }

    public string? StdName { get; set; }

    public string? StdKastname { get; set; }
}
