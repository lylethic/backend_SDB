using System;
using System.Collections.Generic;

namespace server.Models;

public partial class Semester
{
    public int SemesterId { get; set; }

    public int AcademicYearId { get; set; }

    public string SemesterName { get; set; } = null!;

    public DateOnly DateStart { get; set; }

    public DateOnly DateEnd { get; set; }

    public string? Description { get; set; }

    public virtual AcademicYear AcademicYear { get; set; } = null!;

    public virtual ICollection<ChiTietSoDauBai> ChiTietSoDauBais { get; set; } = new List<ChiTietSoDauBai>();

    public virtual ICollection<PhanCongChuNhiem> PhanCongChuNhiems { get; set; } = new List<PhanCongChuNhiem>();

    public virtual ICollection<Week> Weeks { get; set; } = new List<Week>();
}
