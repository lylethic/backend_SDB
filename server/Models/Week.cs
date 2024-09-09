using System;
using System.Collections.Generic;

namespace server.Models;

public partial class Week
{
    public int WeekId { get; set; }

    public int SemesterId { get; set; }

    public string WeekName { get; set; } = null!;

    public DateOnly WeekStart { get; set; }

    public DateOnly WeekEnd { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<ChiTietSoDauBai> ChiTietSoDauBais { get; set; } = new List<ChiTietSoDauBai>();

    public virtual Semester Semester { get; set; } = null!;
}
