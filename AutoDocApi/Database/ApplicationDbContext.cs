using Microsoft.EntityFrameworkCore;
using AutoDocApi.Models;

namespace AutoDocApi.Database;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<TodoTask> TodoTasks { get; set; }
}