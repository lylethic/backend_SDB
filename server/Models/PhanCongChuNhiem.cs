using System;
using System.Collections.Generic;

namespace server.Models;

public partial class PhanCongChuNhiem
{
    public int PhanCongChuNhiemId { get; set; }

    public int TeacherId { get; set; }

    public int ClassId { get; set; }

    public int SemesterId { get; set; }

    public bool Status { get; set; }

    public string? Description { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual Semester Semester { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;
}
