using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using server.Models;

namespace server.Data;

public partial class SoDauBaiContext : DbContext
{
    public SoDauBaiContext()
    {
    }

    public SoDauBaiContext(DbContextOptions<SoDauBaiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AcademicYear> AcademicYears { get; set; }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<BiaSoDauBai> BiaSoDauBais { get; set; }

    public virtual DbSet<ChiTietSoDauBai> ChiTietSoDauBais { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Classification> Classifications { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<PhanCongChuNhiem> PhanCongChuNhiems { get; set; }

    public virtual DbSet<PhanCongGiangDay> PhanCongGiangDays { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<School> Schools { get; set; }

    public virtual DbSet<Semester> Semesters { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<SubjectAssignment> SubjectAssignments { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<TokenStored> TokenStoreds { get; set; }

    public virtual DbSet<Week> Weeks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:SoDauBaiContext");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AcademicYear>(entity =>
        {
            entity.HasKey(e => e.AcademicYearId).HasName("PK__Academic__F8DBC2842A191ACA");

            entity.ToTable("AcademicYear");

            entity.Property(e => e.AcademicYearId).HasColumnName("academicYearId");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.DisplayAcademicYearName)
                .HasMaxLength(100)
                .HasColumnName("displayAcademicYear_Name");
            entity.Property(e => e.YearEnd).HasColumnName("yearEnd");
            entity.Property(e => e.YearStart).HasColumnName("yearStart");
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__F267251E98D34857");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Email, "UQ__Account__AB6E616424F67397").IsUnique();

            entity.Property(e => e.AccountId).HasColumnName("accountId");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(200)
                .HasColumnName("password");
            entity.Property(e => e.PasswordSalt)
                .HasMaxLength(200)
                .HasColumnName("passwordSalt");
            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.SchoolId).HasColumnName("schoolId");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Account__roleId__59063A47");

            entity.HasOne(d => d.School).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.SchoolId)
                .HasConstraintName("FK__Account__schoolI__59FA5E80");
        });

        modelBuilder.Entity<BiaSoDauBai>(entity =>
        {
            entity.HasKey(e => e.BiaSoDauBaiId).HasName("PK__BiaSoDau__B84AE35E5B82570B");

            entity.ToTable("BiaSoDauBai");

            entity.Property(e => e.BiaSoDauBaiId).HasColumnName("biaSoDauBaiId");
            entity.Property(e => e.AcademicyearId).HasColumnName("academicyearId");
            entity.Property(e => e.ClassId).HasColumnName("classId");
            entity.Property(e => e.PhanCongGiangDayId).HasColumnName("phanCongGiangDayId");
            entity.Property(e => e.SchoolId).HasColumnName("schoolId");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");

            entity.HasOne(d => d.Academicyear).WithMany(p => p.BiaSoDauBais)
                .HasForeignKey(d => d.AcademicyearId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BiaSoDauB__acade__282DF8C2");

            entity.HasOne(d => d.Class).WithMany(p => p.BiaSoDauBais)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BiaSoDauB__class__2A164134");

            entity.HasOne(d => d.PhanCongGiangDay).WithMany(p => p.BiaSoDauBais)
                .HasForeignKey(d => d.PhanCongGiangDayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BiaSoDauB__phanC__29221CFB");

            entity.HasOne(d => d.School).WithMany(p => p.BiaSoDauBais)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BiaSoDauB__schoo__2739D489");
        });

        modelBuilder.Entity<ChiTietSoDauBai>(entity =>
        {
            entity.HasKey(e => e.ChiTietSoDauBaiId).HasName("PK__ChiTietS__684F0A5B180E3479");

            entity.ToTable("ChiTietSoDauBai");

            entity.Property(e => e.ChiTietSoDauBaiId).HasColumnName("chiTietSoDauBaiId");
            entity.Property(e => e.AcademicYearId).HasColumnName("academicYearId");
            entity.Property(e => e.Attend).HasColumnName("attend");
            entity.Property(e => e.BiaSoDauBaiId).HasColumnName("biaSoDauBaiId");
            entity.Property(e => e.ClassId).HasColumnName("classId");
            entity.Property(e => e.ClassificationId).HasColumnName("classificationId");
            entity.Property(e => e.Comment)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("comment");
            entity.Property(e => e.Content)
                .HasMaxLength(100)
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.CreatedBy).HasColumnName("createdBy");
            entity.Property(e => e.DaysOfTheWeek).HasMaxLength(10);
            entity.Property(e => e.Ngay).HasColumnName("ngay");
            entity.Property(e => e.Period).HasColumnName("period");
            entity.Property(e => e.SemesterId).HasColumnName("semesterId");
            entity.Property(e => e.Sesion)
                .HasMaxLength(10)
                .HasColumnName("sesion");
            entity.Property(e => e.SubjectId).HasColumnName("subjectId");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.WeekId).HasColumnName("weekId");

            entity.HasOne(d => d.AcademicYear).WithMany(p => p.ChiTietSoDauBais)
                .HasForeignKey(d => d.AcademicYearId)
                .HasConstraintName("FK__ChiTietSo__acade__2EDAF651");

            entity.HasOne(d => d.BiaSoDauBai).WithMany(p => p.ChiTietSoDauBais)
                .HasForeignKey(d => d.BiaSoDauBaiId)
                .HasConstraintName("FK__ChiTietSo__biaSo__2CF2ADDF");

            entity.HasOne(d => d.Class).WithMany(p => p.ChiTietSoDauBais)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__ChiTietSo__class__2DE6D218");

            entity.HasOne(d => d.Classification).WithMany(p => p.ChiTietSoDauBais)
                .HasForeignKey(d => d.ClassificationId)
                .HasConstraintName("FK__ChiTietSo__class__32AB8735");

            entity.HasOne(d => d.Semester).WithMany(p => p.ChiTietSoDauBais)
                .HasForeignKey(d => d.SemesterId)
                .HasConstraintName("FK__ChiTietSo__semes__2FCF1A8A");

            entity.HasOne(d => d.Subject).WithMany(p => p.ChiTietSoDauBais)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK__ChiTietSo__subje__31B762FC");

            entity.HasOne(d => d.Week).WithMany(p => p.ChiTietSoDauBais)
                .HasForeignKey(d => d.WeekId)
                .HasConstraintName("FK__ChiTietSo__weekI__30C33EC3");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Class__7577347EDC9E01BE");

            entity.ToTable("Class");

            entity.Property(e => e.ClassId).HasColumnName("classId");
            entity.Property(e => e.AcademicYearId).HasColumnName("academicYearId");
            entity.Property(e => e.ClassName)
                .HasMaxLength(50)
                .HasColumnName("className");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.GradeId).HasColumnName("gradeId");
            entity.Property(e => e.SchoolId).HasColumnName("schoolId");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            entity.Property(e => e.TeacherId).HasColumnName("teacherId");

            entity.HasOne(d => d.AcademicYear).WithMany(p => p.Classes)
                .HasForeignKey(d => d.AcademicYearId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Class__academicY__02FC7413");

            entity.HasOne(d => d.Grade).WithMany(p => p.Classes)
                .HasForeignKey(d => d.GradeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Class__gradeId__01142BA1");

            entity.HasOne(d => d.School).WithMany(p => p.Classes)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Class__schoolId__03F0984C");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Classes)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Class__teacherId__02084FDA");
        });

        modelBuilder.Entity<Classification>(entity =>
        {
            entity.HasKey(e => e.ClassificationId).HasName("PK__Classifi__93F59C967AB49F0A");

            entity.ToTable("Classification");

            entity.Property(e => e.ClassificationId).HasColumnName("classificationId");
            entity.Property(e => e.ClassifyName)
                .HasMaxLength(100)
                .HasColumnName("classifyName");
            entity.Property(e => e.Score).HasColumnName("score");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("PK__Grade__FB4362F9A98275B9");

            entity.ToTable("Grade");

            entity.Property(e => e.GradeId).HasColumnName("gradeId");
            entity.Property(e => e.AcademicYearId).HasColumnName("academicYearId");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.GradeName)
                .HasMaxLength(50)
                .HasColumnName("gradeName");

            entity.HasOne(d => d.AcademicYear).WithMany(p => p.Grades)
                .HasForeignKey(d => d.AcademicYearId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Grade__academicY__6D0D32F4");
        });

        modelBuilder.Entity<PhanCongChuNhiem>(entity =>
        {
            entity.HasKey(e => e.PhanCongChuNhiemId).HasName("PK__PhanCong__B11B5963FDC5D885");

            entity.ToTable("PhanCongChuNhiem");

            entity.Property(e => e.PhanCongChuNhiemId).HasColumnName("phanCongChuNhiemId");
            entity.Property(e => e.AcademicYearId).HasColumnName("academicYearId");
            entity.Property(e => e.ClassId).HasColumnName("classId");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.SemesterId).HasColumnName("semesterId");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            entity.Property(e => e.TeacherId).HasColumnName("teacherId");

            entity.HasOne(d => d.AcademicYear).WithMany(p => p.PhanCongChuNhiems)
                .HasForeignKey(d => d.AcademicYearId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhanCongC__acade__1EA48E88");

            entity.HasOne(d => d.Class).WithMany(p => p.PhanCongChuNhiems)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhanCongC__class__1DB06A4F");

            entity.HasOne(d => d.Semester).WithMany(p => p.PhanCongChuNhiems)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhanCongC__semes__1F98B2C1");

            entity.HasOne(d => d.Teacher).WithMany(p => p.PhanCongChuNhiems)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhanCongC__teach__1CBC4616");
        });

        modelBuilder.Entity<PhanCongGiangDay>(entity =>
        {
            entity.HasKey(e => e.PhanCongGiangDayId).HasName("PK__PhanCong__6B45110FD6A6E7DF");

            entity.ToTable("PhanCongGiangDay");

            entity.Property(e => e.PhanCongGiangDayId).HasColumnName("phanCongGiangDayId");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            entity.Property(e => e.TeacherId).HasColumnName("teacherId");

            entity.HasOne(d => d.Teacher).WithMany(p => p.PhanCongGiangDays)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhanCongG__teach__236943A5");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__CD98462ADDF7E78D");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .HasColumnName("description");
            entity.Property(e => e.NameRole)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nameRole");
        });

        modelBuilder.Entity<School>(entity =>
        {
            entity.HasKey(e => e.SchoolId).HasName("PK__School__129B9799CF486F48");

            entity.ToTable("School");

            entity.HasIndex(e => e.PhoneNumber, "UQ__School__4849DA0155FC7936").IsUnique();

            entity.Property(e => e.SchoolId).HasColumnName("schoolId");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .HasColumnName("address");
            entity.Property(e => e.Desription)
                .HasMaxLength(100)
                .HasColumnName("desription");
            entity.Property(e => e.DistrictId).HasColumnName("districtId");
            entity.Property(e => e.NameSchcool)
                .HasMaxLength(200)
                .HasColumnName("nameSchcool");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("phoneNumber");
            entity.Property(e => e.ProvinceId).HasColumnName("provinceId");
            entity.Property(e => e.SchoolType)
                .HasDefaultValue(true)
                .HasColumnName("schoolType");
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.SemesterId).HasName("PK__Semester__F2F37E870A6790A8");

            entity.ToTable("Semester");

            entity.Property(e => e.SemesterId).HasColumnName("semesterId");
            entity.Property(e => e.AcademicYearId).HasColumnName("academicYearId");
            entity.Property(e => e.DateEnd).HasColumnName("dateEnd");
            entity.Property(e => e.DateStart).HasColumnName("dateStart");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.SemesterName)
                .HasMaxLength(100)
                .HasColumnName("semesterName");

            entity.HasOne(d => d.AcademicYear).WithMany(p => p.Semesters)
                .HasForeignKey(d => d.AcademicYearId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Semester__academ__6A30C649");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Student__4D11D63C8FD22332");

            entity.ToTable("Student");

            entity.Property(e => e.StudentId).HasColumnName("studentId");
            entity.Property(e => e.AccountId).HasColumnName("accountId");
            entity.Property(e => e.ClassId).HasColumnName("classId");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .HasColumnName("fullname");
            entity.Property(e => e.GradeId).HasColumnName("gradeId");
            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.ShoolId).HasColumnName("shoolId");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");

            entity.HasOne(d => d.Account).WithMany(p => p.Students)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Student__account__09A971A2");

            entity.HasOne(d => d.Class).WithMany(p => p.Students)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Student__classId__08B54D69");

            entity.HasOne(d => d.Grade).WithMany(p => p.Students)
                .HasForeignKey(d => d.GradeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Student__gradeId__07C12930");

            entity.HasOne(d => d.Role).WithMany(p => p.Students)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Student__roleId__0B91BA14");

            entity.HasOne(d => d.Shool).WithMany(p => p.Students)
                .HasForeignKey(d => d.ShoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Student__shoolId__0A9D95DB");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__Subject__ACF9A7606A2A51FA");

            entity.ToTable("Subject");

            entity.Property(e => e.SubjectId).HasColumnName("subjectId");
            entity.Property(e => e.AcademicYearId).HasColumnName("academicYearId");
            entity.Property(e => e.SubjectName)
                .HasMaxLength(100)
                .HasColumnName("subjectName");

            entity.HasOne(d => d.AcademicYear).WithMany(p => p.Subjects)
                .HasForeignKey(d => d.AcademicYearId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subject__academi__0F624AF8");
        });

        modelBuilder.Entity<SubjectAssignment>(entity =>
        {
            entity.HasKey(e => e.SubjectAssignmentId).HasName("PK__SubjectA__803AC446A9FA67AB");

            entity.ToTable("SubjectAssignment");

            entity.Property(e => e.SubjectAssignmentId).HasColumnName("subjectAssignmentId");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.SubjectId).HasColumnName("subjectId");
            entity.Property(e => e.TeacherId).HasColumnName("teacherId");

            entity.HasOne(d => d.Subject).WithMany(p => p.SubjectAssignments)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SubjectAs__subje__1332DBDC");

            entity.HasOne(d => d.Teacher).WithMany(p => p.SubjectAssignments)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SubjectAs__teach__123EB7A3");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PK__Teacher__98E93895DE7F4185");

            entity.ToTable("Teacher");

            entity.Property(e => e.TeacherId).HasColumnName("teacherId");
            entity.Property(e => e.AccountId).HasColumnName("accountId");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .HasColumnName("address");
            entity.Property(e => e.DateOfBirth)
                .HasColumnType("datetime")
                .HasColumnName("dateOfBirth");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .HasColumnName("fullname");
            entity.Property(e => e.Gender)
                .HasDefaultValue((byte)1)
                .HasColumnName("gender");
            entity.Property(e => e.SchoolId).HasColumnName("schoolId");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");

            entity.HasOne(d => d.Account).WithMany(p => p.Teachers)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Teacher__account__6477ECF3");

            entity.HasOne(d => d.School).WithMany(p => p.Teachers)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Teacher__schoolI__656C112C");
        });

        modelBuilder.Entity<TokenStored>(entity =>
        {
            entity.HasKey(e => e.TokenStoredId).HasName("PK__TokenSto__639EAA858B26CD37");

            entity.ToTable("TokenStored");

            entity.Property(e => e.TokenStoredId).HasColumnName("tokenStoredId");
            entity.Property(e => e.AccountId).HasColumnName("accountId");
            entity.Property(e => e.TokenString)
                .IsUnicode(false)
                .HasColumnName("tokenString");

            entity.HasOne(d => d.Account).WithMany(p => p.TokenStoreds)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TokenStor__accou__625A9A57");
        });

        modelBuilder.Entity<Week>(entity =>
        {
            entity.HasKey(e => e.WeekId).HasName("PK__Week__982269FEE6B6DA03");

            entity.ToTable("Week");

            entity.Property(e => e.WeekId).HasColumnName("weekId");
            entity.Property(e => e.SemesterId).HasColumnName("semesterId");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            entity.Property(e => e.WeekEnd).HasColumnName("weekEnd");
            entity.Property(e => e.WeekName)
                .HasMaxLength(50)
                .HasColumnName("weekName");
            entity.Property(e => e.WeekStart).HasColumnName("weekStart");

            entity.HasOne(d => d.Semester).WithMany(p => p.Weeks)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Week__semesterId__18EBB532");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
