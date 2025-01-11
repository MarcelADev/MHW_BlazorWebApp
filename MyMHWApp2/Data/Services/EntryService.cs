using Microsoft.EntityFrameworkCore;
using MyMHWApp2.Components.Models;

namespace MyMHWApp2.Data.Services
{
    public class EntryService : IEntryService
    {
        private readonly AppDbContext _dbContext;

        public EntryService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // CREATE
        public async Task AddEntryAsync(Entry entry)
        {
            _dbContext.Entries.Add(entry);
            await _dbContext.SaveChangesAsync();
        }

        // READ
        public async Task<List<Entry>> GetEntriesAsync()
        {
            return await _dbContext.Entries.ToListAsync();
        }

        // UPDATE
        public async Task UpdateEntryAsync(Entry entry)
        {
            _dbContext.Entries.Update(entry);
            await _dbContext.SaveChangesAsync();
        }

        // DELETE
        public async Task DeleteEntryAsync(int entryId)
        {
            var entry = await _dbContext.Entries.FindAsync(entryId);
            if (entry != null)
            {
                _dbContext.Entries.Remove(entry);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
