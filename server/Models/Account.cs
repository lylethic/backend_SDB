using System;
using System.Collections.Generic;

namespace server.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public int RoleId { get; set; }

    public int? SchoolId { get; set; }

    public string Email { get; set; } = null!;

    public byte[] Password { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

    public virtual School? School { get; set; }

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
}
