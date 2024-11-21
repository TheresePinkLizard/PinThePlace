using Microsoft.AspNetCore.Http.Features;
using PinThePlace.Models;

namespace PinThePlace.DAL;

public interface IPinRepository
{
    Task<IEnumerable<Pin>> GetAll();
    Task<Pin?> GetItemById(int id);
    Task Create(Pin pin);
    Task Update(Pin pin);
    Task<bool> Delete(int id);
}