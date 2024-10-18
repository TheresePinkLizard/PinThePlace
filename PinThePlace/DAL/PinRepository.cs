using Microsoft.EntityFrameworkCore;
using PinThePlace.Models;

namespace PinThePlace.DAL;

public class PinRepository : IPinRepository
{
    private readonly PinDbContext _db;

    public PinRepository(PinDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Pin>> GetAll()
    {
        return await _db.Pins.ToListAsync();
    }

    public async Task<Pin?> GetItemById(int id)
    {
        return await _db.Pins.FindAsync(id);
    }

    public async Task Create(Pin pin)
    {
        _db.Pins.Add(pin);
        await _db.SaveChangesAsync();
    }

    public async Task Update(Pin pin)
    {
        _db.Pins.Update(pin);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> Delete(int id)
    {
        var item = await _db.Pins.FindAsync(id);
        if (item == null)
        {
            return false;
        }

        _db.Pins.Remove(item);
        await _db.SaveChangesAsync();
        return true;
    }
}