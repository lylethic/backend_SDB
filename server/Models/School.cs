﻿using System;
using System.Collections.Generic;

namespace server.Models;

public partial class School
{
    public int SchoolId { get; set; }

    public byte ProvinceId { get; set; }

    public byte DistrictId { get; set; }

    public string NameSchcool { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public bool SchoolType { get; set; }

    public string? Desription { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<BiaSoDauBai> BiaSoDauBais { get; set; } = new List<BiaSoDauBai>();

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
}