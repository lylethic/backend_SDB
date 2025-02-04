using server.Data;
using server.Dtos;
using server.IService;
using server.Models;

namespace server.Repositories
{
  public class AbsencesRepositories : IAbsence
  {
    private readonly SoDauBaiContext _context;
    public AbsencesRepositories(SoDauBaiContext context)
    {
      this._context = context;
    }
    public async Task<Absence> CreateAsync(AbsenceDto model)
    {
      //try
      //{
      //  var data = new Absence
      //  {
      //    AbsenceId = model.AbsenceId,
      //    CallRollId = model.CallRollId,
      //    Description = model.Description,
      //    StudentId = model.StudentId,
      //  };
      //  var absence = await _context.Absences.AddAsync(data);
      //}
      //catch (Exception ex)
      //{
      //  throw new Exception(ex.Message);
      //}
      throw new NotImplementedException();

    }

    public Task<Absence> GetAbsencesAsync()
    {
      throw new NotImplementedException();
    }
  }
}
