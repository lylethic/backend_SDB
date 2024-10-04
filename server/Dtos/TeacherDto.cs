﻿namespace server.Dtos
{
  public class TeacherDto
  {
    public int TeacherId { get; set; }

    public int AccountId { get; set; }

    public string Fullname { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public bool Gender { get; set; }

    public string Address { get; set; } = null!;

    public bool Status { get; set; }

    public DateTime? DateCreate { get; set; }

    public DateTime? DateUpdate { get; set; }

    // School Table
    public int SchoolId { get; set; }

    public string? NameSchool { get; set; }

    public bool SchoolType { get; set; }

  }
}
