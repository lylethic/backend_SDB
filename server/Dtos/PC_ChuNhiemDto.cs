﻿namespace server.Dtos
{
  public class PC_ChuNhiemDto
  {
    public int PhanCongChuNhiemId { get; set; }

    public int TeacherId { get; set; }

    public int ClassId { get; set; }

    public int AcademicYearId { get; set; }

    public int SemesterId { get; set; }

    public bool Status { get; set; }

    public string? Description { get; set; }
  }
}