CREATE TABLE Role (
  roleId INT IDENTITY(1,1) PRIMARY KEY,
  nameRole varchar(20) NOT NULL,
  description nvarchar(50) NOT NULL,
	dateCreated Datetime,
	dateUpdated Datetime,
);

CREATE TABLE School (
    schoolId INT IDENTITY(1,1) PRIMARY KEY,
    provinceId TINYINT NOT NULL,
    districtId TINYINT NOT NULL,
    nameSchcool NVARCHAR(200) NOT NULL,
    address NVARCHAR(200) NOT NULL,
    phoneNumber CHAR(10) NOT NULL UNIQUE,
    schoolType BIT NOT NULL DEFAULT 1,
		description NVARCHAR(100) NULL,
		dateCreated Datetime,
		dateUpdated Datetime,
);

-- Renaming the column 'desription' to 'description' in the 'School' table
EXEC sp_rename 'School.desription', 'description', 'COLUMN';


CREATE TABLE Account (
    accountId INT IDENTITY(1,1) PRIMARY KEY,
    roleId INT NOT NULL,
    schoolId INT NULL,
    email NVARCHAR(50) NOT NULL UNIQUE,  
		matKhau VARBINARY(200) NOT NULL, 
    passwordSalt VARBINARY(200) NOT NULL,
		dateCreated Datetime,
		dateUpdated Datetime,
		FOREIGN KEY (roleId) REFERENCES Role(roleId),
		FOREIGN KEY (schoolId) REFERENCES School(schoolId),
);


CREATE TABLE Session (
    tokenId INT IDENTITY(1,1) PRIMARY KEY,
		token VARCHAR(MAX) NOT NULL,
    accountId INT NOT NULL,
		expiresAt DateTime,
		createdAt DateTime,
		FOREIGN KEY (accountId) REFERENCES Account(accountId),
);

CREATE TABLE Teacher (
    teacherId INT IDENTITY(1,1) PRIMARY KEY,
    accountId INT NOT NULL,
    schoolId INT NOT NULL,
    fullname NVARCHAR(100) NOT NULL,
    dateOfBirth DATETIME NOT NULL,
    gender BIT NOT NULL DEFAULT 1,
    address NVARCHAR(200) NOT NULL,
    status BIT NOT NULL DEFAULT 1,
		dateCreate DATETIME,
		dateUpdate DATETIME,
		FOREIGN KEY (accountId) REFERENCES Account(accountId),
		FOREIGN KEY (schoolId) REFERENCES School(schoolId),
);

--// nien khoa
CREATE TABLE AcademicYear (
    academicYearId INT IDENTITY(1,1) PRIMARY KEY,
    displayAcademicYear_Name NVARCHAR(100) NOT NULL,
    yearStart DATE NOT NULL,
    yearEnd DATE NOT NULL,
		description NVARCHAR(100) NULL,
);

CREATE TABLE Semester (
    semesterId INT IDENTITY(1,1) PRIMARY KEY,
    academicYearId INT NOT NULL,
    semesterName NVARCHAR(100) NOT NULL,
    dateStart DATE NOT NULL,
    dateEnd DATE NOT NULL,
		description NVARCHAR(100) NULL,
    FOREIGN KEY (academicYearId) REFERENCES AcademicYear(academicYearId),
);

CREATE TABLE Grade (
    gradeId INT IDENTITY(1,1) PRIMARY KEY,
		academicYearId INT NOT NULL,
    gradeName NVARCHAR(50) NOT NULL,
		description NVARCHAR(100) NULL,
		dateCreated Datetime,
		dateUpdated Datetime,
    FOREIGN KEY (academicYearId) REFERENCES AcademicYear(academicYearId),
);

CREATE TABLE Class (
    classId INT IDENTITY(1,1) PRIMARY KEY,
    gradeId INT NOT NULL,
    teacherId INT NOT NULL,
    academicYearId INT NOT NULL,
    schoolId INT NOT NULL,
    className NVARCHAR(50) NOT NULL,
    status BIT NOT NULL DEFAULT 1,
		description NVARCHAR(100) NULL,
		dateCreated Datetime,
		dateUpdated Datetime,
    FOREIGN KEY (gradeId) REFERENCES Grade(gradeId),
    FOREIGN KEY (teacherId) REFERENCES Teacher(teacherId),
    FOREIGN KEY (academicYearId) REFERENCES AcademicYear(academicYearId),
    FOREIGN KEY (schoolId) REFERENCES School(schoolId),
);

CREATE TABLE Student (
    studentId INT IDENTITY(1,1) PRIMARY KEY,
    classId INT NOT NULL,
    gradeId INT NOT NULL,
    accountId INT NOT NULL,
    fullname NVARCHAR(100) NOT NULL,
    status BIT NOT NULL DEFAULT 1, 
		dateCreated Datetime,
		dateUpdated Datetime,
		description NVARCHAR(100) NULL
    FOREIGN KEY (gradeId) REFERENCES Grade(gradeId) ,
    FOREIGN KEY (classId) REFERENCES Class(classId),
    FOREIGN KEY (accountId) REFERENCES Account(accountId),
);

ALTER TABLE Student DROP COLUMN roleId;
ALTER TABLE Student DROP COLUMN shoolId;

-- Drop foreign key constraints
ALTER TABLE [dbo].[Student] DROP CONSTRAINT FK__Student__roleId__0B91BA14;  -- Replace with actual constraint name if different
ALTER TABLE [dbo].[Student] DROP CONSTRAINT FK__Student__shoolId__0A9D95DB;  -- Replace with actual constraint name if different


--//
CREATE TABLE Subject (
    subjectId INT IDENTITY(1,1) PRIMARY KEY,
		academicYearId INT NOT NULL,
    subjectName NVARCHAR(100) NOT NULL,
		FOREIGN KEY (academicYearId) REFERENCES AcademicYear(academicYearId),
);

--// Phan cong giang day mon hoc cho giao vien
CREATE TABLE SubjectAssignment (
    subjectAssignmentId INT IDENTITY(1,1) PRIMARY KEY,
		teacherId INT NOT NULL,
		subjectId INT NOT NULL,
		description NVARCHAR(100) NULL,
		dateCreated Datetime,
		dateUpdated Datetime,
    FOREIGN KEY (teacherId) REFERENCES Teacher(teacherId),
    FOREIGN KEY (subjectId) REFERENCES Subject(subjectId),
);

--// diem xep loai
CREATE TABLE Classification (
    classificationId INT IDENTITY(1,1) PRIMARY KEY,
    classifyName NVARCHAR(100) NOT NULL,
		score INT NULL,
);

CREATE TABLE Week (
    weekId INT IDENTITY(1,1) PRIMARY KEY,
    semesterId INT NOT NULL,
    weekName NVARCHAR(50) NOT NULL,
    weekStart DATE NOT NULL,
    weekEnd DATE NOT NULL,
		status BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (semesterId) REFERENCES Semester(semesterId),
);

CREATE TABLE PhanCongChuNhiem (
    phanCongChuNhiemId INT IDENTITY(1,1) PRIMARY KEY,
    teacherId INT NOT NULL,
    classId INT NOT NULL,
    semesterId INT NOT NULL,
		status BIT NOT NULL DEFAULT 1,
		dateCreated Datetime,
		dateUpdated Datetime,
		description NVARCHAR(100) NULL,
    FOREIGN KEY (teacherId) REFERENCES Teacher(teacherId),
    FOREIGN KEY (classId) REFERENCES Class(classId),
    FOREIGN KEY (semesterId) REFERENCES Semester(semesterId),
);

-- Drop foreign key constraints
ALTER TABLE [dbo].PhanCongChuNhiem DROP CONSTRAINT FK__PhanCongC__acade__1EA48E88; 
ALTER TABLE PhanCongChuNhiem DROP COLUMN academicYearId;


--//
CREATE TABLE PhanCongGiangDay (
    phanCongGiangDayId INT IDENTITY(1,1) PRIMARY KEY,
		biaSoDauBaiId INT NOT NULL,
    teacherId INT NOT NULL,
		status BIT NOT NULL DEFAULT 1,
		dateCreated Datetime,
		dateUpdated Datetime,
    FOREIGN KEY (teacherId) REFERENCES Teacher(teacherId),
		Foreign KEY (biaSoDauBaiId) REFERENCES BiaSoDauBai(biaSoDauBaiId)
);


CREATE TABLE BiaSoDauBai (
    biaSoDauBaiId INT IDENTITY(1,1) PRIMARY KEY,
    schoolId INT NOT NULL,
    academicyearId INT NOT NULL,
    classId INT NOT NULL,
    status BIT NOT NULL DEFAULT 1,
		dateCreated Datetime,
		dateUpdated Datetime,
    FOREIGN KEY (schoolId) REFERENCES School(schoolId),
    FOREIGN KEY (academicyearId) REFERENCES AcademicYear(academicyearId),
    FOREIGN KEY (classId) REFERENCES Class(classId),
);

ALTER TABLE [dbo].BiaSoDauBai DROP CONSTRAINT FK__BiaSoDauB__phanC__29221CFB; 
ALTER TABLE BiaSoDauBai
DROP COLUMN phanCongGiangDayId;


CREATE TABLE ChiTietSoDauBai (
  chiTietSoDauBaiId INT IDENTITY(1,1) PRIMARY KEY,
  biaSoDauBaiId INT NOT NULL,
  semesterId INT NOT NULL,
  weekId INT NOT NULL,
  subjectId INT NOT NULL,
  classificationId INT NOT NULL,
  DaysOfTheWeek NVARCHAR(10) NOT NULL,
  thoiGian Datetime NOT NULL,
  buoiHoc NVARCHAR(10) NOT NULL,
  tietHoc INT NOT NULL,
  lessonContent NVARCHAR(Max) NOT NULL,
  attend INT NOT NULL,
  noteComment NVARCHAR(255),
  createdBy INT, -- Ma so cua Giao vien/Admin gi do
  createdAt DATETIME DEFAULT GETDATE(),
	updatedAt DATETIME DEFAULT NULL,
  FOREIGN KEY (biaSoDauBaiId) REFERENCES BiaSoDauBai(biaSoDauBaiId) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY (semesterId) REFERENCES Semester(semesterId) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY (weekId) REFERENCES Week(weekId) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY (subjectId) REFERENCES Subject(subjectId) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY (classificationId) REFERENCES Classification(classificationId) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE INDEX IDX_ChiTietSoDauBai_WeekId ON ChiTietSoDauBai(weekId);

DECLARE @i int =1
WHILE (@i < 1000)
BEGIN
	INSERT dbo.Classification(classifyName, score)
	VALUES ('Test' + CAST(@i as nvarchar(12)), @i *2)
SET @i = @i + 1
END;

-- Drop foreign key constraints
ALTER TABLE [dbo].ChiTietSoDauBai DROP CONSTRAINT FK__ChiTietSo__acade__7D439ABD; 
ALTER TABLE ChiTietSoDauBai DROP COLUMN academicYearId;