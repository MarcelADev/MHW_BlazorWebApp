using Microsoft.EntityFrameworkCore;
using MyMHWApp2.Components.Models;

namespace MyMHWApp2.Data.Services
{
    public class EntryServiceTesting : IEntryService
    {
        private readonly List<Entry> entries;

        public EntryServiceTesting(AppDbContext dbContext)
        {
            entries = [];
            int idCounter = 1;

            entries.Add(new Entry
            {
                Id = idCounter++,
                Name = "Small Monster Bone",
                CategoryId = 1,
                Count = 0
            });

            entries.Add(new Entry
            {
                Id = idCounter++,
                Name = "Medium Monster Bone",
                CategoryId = 1,
                Count = 0
            });

            entries.Add(new Entry
            {
                Id = idCounter++,
                Name = "Large Monster Bone",
                CategoryId = 1,
                Count = 0
            });
        }

        // CREATE
        public async Task AddEntryAsync(Entry entry)
        {
            entries.Add(entry);
        }

        // READ
        public async Task<List<Entry>> GetEntriesAsync()
        {
            return entries;
        }

        // UPDATE
        public async Task UpdateEntryAsync(Entry entry)
        {
            // Find Bob by name and change his age
            var entryToUpdate = entries.FirstOrDefault(e => e.Id == entry.Id);

            if (entryToUpdate != null)
            {
                if(entryToUpdate.Name != entry.Name) 
                    entryToUpdate.Name = entry.Name;

                if (entryToUpdate.CategoryId != entry.CategoryId) 
                    entryToUpdate.CategoryId = entry.CategoryId;

                if (entryToUpdate.Count != entry.Count) 
                    entryToUpdate.Count = entry.Count;
            }
        }

        // DELETE
        public async Task DeleteEntryAsync(int entryId)
        {
            var entryToDelete = entries.FirstOrDefault(e => e.Id == entryId);
            if (entryToDelete != null)
            {
                entries.Remove(entryToDelete);
            }
        }
    }
}
