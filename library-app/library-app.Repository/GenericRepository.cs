﻿using library_app.Contracts;
using library_app.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace library_app.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly LibraryAppDbContext _context;
        public GenericRepository(LibraryAppDbContext context)
        {
               _context = context; 
        }

        public async Task<T> GetAsync(int? id)
        {
            if (id is null)
            {
                return null;
            }
            return await _context.Set<T>().FindAsync(id);
        }
        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }
        public async Task<T> AddAsync(T entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var entity = await GetAsync(id);
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> Exists(int id)
        {
            var entity = await GetAsync(id);
            return entity != null;
        }
    }
}
