using System;
using System.Collections.Generic;

namespace server.Models;

public partial class BiaSoDauBai
{
    public int BiaSoDauBaiId { get; set; }

    public int SchoolId { get; set; }

    public int AcademicyearId { get; set; }

    public int ClassId { get; set; }

    public int PhanCongGiangDayId { get; set; }

    public bool Status { get; set; }

    public virtual AcademicYear Academicyear { get; set; } = null!;

    public virtual ICollection<ChiTietSoDauBai> ChiTietSoDauBais { get; set; } = new List<ChiTietSoDauBai>();

    public virtual Class Class { get; set; } = null!;

    public virtual PhanCongGiangDay PhanCongGiangDay { get; set; } = null!;

    public virtual School School { get; set; } = null!;
}
