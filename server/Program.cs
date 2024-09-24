using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using server;
using server.IService;
using server.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//Connection DB Local
builder.Services.AddDbContext<server.Data.SoDauBaiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SoDauBaiContext"))
    .EnableDetailedErrors()
    .LogTo(Console.WriteLine));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cors
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

//Inject app Dependencies (Dependecy Injecteion)
builder.Services.AddScoped<IAuth, AuthRepositories>();
builder.Services.AddScoped<ITokenService, TokenRepositories>();
builder.Services.AddScoped<IAccount, AccountRespositories>();
builder.Services.AddScoped<IRole, RoleRepositories>();
builder.Services.AddScoped<ISchool, SchoolRepositories>();
builder.Services.AddScoped<ITeacher, TeacherRepositories>();
builder.Services.AddScoped<IStudent, StudentRepositories>();
builder.Services.AddScoped<IAcademicYear, AcademicYearRepositories>();
builder.Services.AddScoped<ISemester, SemesterRepositories>();
builder.Services.AddScoped<ISubject, SubjectRepositories>();
builder.Services.AddScoped<ISubject_Assgm, SubjectAssgmRepositories>();
builder.Services.AddScoped<IGrade, GradeRepositories>();
builder.Services.AddScoped<IClass, ClassRepositories>();

// Add AutoMapper and configure profiles
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>().AddEntityFrameworkStores<server.Data.SoDauBaiContext>();

// Load configuration from appsettings.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

//Add JWT authentication
builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
  options.SaveToken = true;
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = configuration["JwtSettings:Issuer"],
    ValidAudience = configuration["JwtSettings:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]!))
  };
});


//
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseCors();

app.UseRouting();
app.UseMiddleware<JWTHeaderMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
