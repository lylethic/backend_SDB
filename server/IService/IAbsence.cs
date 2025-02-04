using server.Dtos;
using server.Models;

namespace server.IService
{
  public interface IAbsence
  {
    Task<Absence> GetAbsencesAsync();
    Task<Absence> CreateAsync(AbsenceDto absence);
  }
}
