using Microsoft.EntityFrameworkCore;
using PinThePlace.Models;

namespace PinThePlace.DAL;

public class PinRepository : IPinRepository
{
    private readonly PinDbContext _db;
    private readonly ILogger<PinRepository> _logger;

    public PinRepository(PinDbContext db, ILogger<PinRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IEnumerable<Pin>?> GetAll()
    {
        try{
          // to sort by date, newest on top of the screen
         return await _db.Pins.OrderByDescending(p => p.DateCreated).ToListAsync();
        }
        catch (Exception e){
            _logger.LogError("[PinRepository] Pins ToList Async() failed when GetAll(), error message: {e}", e.Message);
            return null; 
        }
    }

    public async Task<Pin?> GetItemById(int id)
    {
        try{
          return await _db.Pins.FindAsync(id);  
        }
        catch (Exception e){
            _logger.LogError("[PinRepository] Pin FindAsync(id) failed when GetItemById for PinId {PinId:0000}, error message: {e}", id, e.Message);
            return null;
        }
        
    }

    public async Task<bool> Create(Pin pin)
    {
        try{
        _db.Pins.Add(pin);
        await _db.SaveChangesAsync();
        return true;
        }catch (Exception e){
            _logger.LogError("[PinRepository] Pin create failed for Pin {@pin}, error message: {e}", pin, e.Message);
            return false;
        }
    }

    public async Task<bool> Update(Pin pin)
    {
        try{
        _db.Pins.Update(pin);
        await _db.SaveChangesAsync();
        return true;
        } catch (Exception e){
            _logger.LogError("[PinRepository] Pin FindAsync(id) failed when updating the PinId {PinId:0000}, error message: {e}", pin, e.Message);
            return false;
        }
    }

    public async Task<bool> Delete(int id)
    {
        try{
        var item = await _db.Pins.FindAsync(id);
        if (item == null)
        {
            return false;
        }

        _db.Pins.Remove(item);
        await _db.SaveChangesAsync();
        return true;
        } catch (Exception e){
            _logger.LogError("[PinRepository] Pin deletion failed for PinId {PinId:0000}, error message: {e}", id, e.Message);
            return false;
        }
    }
}