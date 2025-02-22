using Microsoft.EntityFrameworkCore;
using AutoDocApi.Models;

namespace AutoDocApi.Database;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TodoTask> TodoTasks { get; set; }
}