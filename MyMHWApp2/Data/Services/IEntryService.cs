using MyMHWApp2.Components.Models;

namespace MyMHWApp2.Data.Services
{
    public interface IEntryService
    {
        Task AddEntryAsync(Entry entry);
        Task<List<Entry>> GetEntriesAsync();
        Task UpdateEntryAsync(Entry entry);
        Task DeleteEntryAsync(int entryId);
    }
}
