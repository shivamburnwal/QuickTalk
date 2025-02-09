﻿using Microsoft.EntityFrameworkCore;
using QuickTalk.Api.Data;
using QuickTalk.Api.Repositories.Interfaces;

namespace QuickTalk.Api.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ChatDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ChatDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
