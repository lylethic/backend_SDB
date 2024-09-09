using System;
using System.Collections.Generic;

namespace server.Models;

public partial class PhanCongGiangDay
{
    public int PhanCongGiangDayId { get; set; }

    public int TeacherId { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<BiaSoDauBai> BiaSoDauBais { get; set; } = new List<BiaSoDauBai>();

    public virtual Teacher Teacher { get; set; } = null!;
}
