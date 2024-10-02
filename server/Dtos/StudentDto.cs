﻿namespace server.Dtos
{
  public class StudentDto
  {
    public int StudentId { get; set; }

    public int ClassId { get; set; }

    public int GradeId { get; set; }

    public int AccountId { get; set; }

    public string Fullname { get; set; } = null!;

    public bool Status { get; set; }

    public string? Description { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public AccountDto? Account { get; set; }
  }
}
