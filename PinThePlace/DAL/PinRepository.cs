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

    public async Task<IEnumerable<Pin>> GetAll()
    {
        try{
          // to sort by date, newest on top of the screen
         return await _db.Pins.OrderByDescending(p => p.DateCreated).ToListAsync();

        }
        catch (Exception e){
            _logger.LogError(e,"[PinRepository] Pins ToListAsync() failed when GetAll()");
            return new List<Pin>(); 
        }
    }

    public async Task<Pin?> GetItemById(int id)
    {
        try{
          return await _db.Pins.FindAsync(id);  
        }
        catch (Exception e){
            _logger.LogError(e,"[PinRepository] Pin FindAsync(id) failed when GetItemById for PinId {PinId:0000}", id);
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
            _logger.LogError(e,"[PinRepository] Pin create failed for Pin {@pin}", pin);
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
            _logger.LogError(e, "[PinRepository] Pin FindAsync(id) failed when updating the PinId {PinId:0000}", pin);
            return false;
        }
    }

    public async Task<bool> Delete(int id)
    {
        try{
        var item = await _db.Pins.FindAsync(id);
        if (item == null)
        {
            _logger.LogWarning("[PinRepository] Pin not found for deletion, PinId {PinId:0000}", id);
            return false;
        }

        _db.Pins.Remove(item);
        await _db.SaveChangesAsync();
        return true;
        } catch (Exception e)
        {
            _logger.LogError(e, "[PinRepository] Failed to delete pin with ID {PinId:0000}", id);
            return false;
        }
    }

    public async Task<Favorite?> GetFavoriteById(int id)
    {
        try
        {
            return await _db.Favorites.FindAsync(id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[FavoriteRepository] Favorite FindAsync(id) failed when GetFavoriteById for FavId {FavId:0000}", id);
            return null;
        }
    }

    public async Task<IEnumerable<Favorite>> GetAllFavorites()
    {
        try{
         
         return await _db.Favorites.Include(f=> f.Pin).Include(f=> f.User).ToListAsync();

        }
        catch (Exception e){
            _logger.LogError(e,"[PinRepository] Favorites ToListAsync() failed when GetAllFavorites()");
            return new List<Favorite>(); 
        }
    }

    public async Task<bool> SaveFavorite(Favorite favorite)
    {
        try
        {
            _db.Favorites.Add(favorite);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[PinRepository] Failed to save favorite with ID {@favorite}", favorite);
            return false;
        }
    }

     public async Task<bool> UpdateFavorite(Favorite favorite)
    {   
        try{
        _db.Favorites.Update(favorite);
        await _db.SaveChangesAsync();
        return true;
        } catch (Exception e){
            _logger.LogError(e, "[PinRepository] Failed when updating the FavoriteId {FavoriteId:0000}", favorite);
            return false;
        }
    }

     public async Task<bool> DeleteFavorite(int id)
    {
        try{
        var fav = await _db.Favorites.FindAsync(id);
        if (fav == null)
        {
            _logger.LogWarning("[PinRepository] Favorite not found for deletion, FavoriteId {FavoriteId:0000}", id);
            return false;
        }

        _db.Favorites.Remove(fav);
        await _db.SaveChangesAsync();
        return true;
        } catch (Exception e)
        {
            _logger.LogError(e, "[PinRepository] Failed to delete favorite with ID {FavoriteId:0000}", id);
            return false;
        }
    }






}