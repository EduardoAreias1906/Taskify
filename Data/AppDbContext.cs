using Microsoft.EntityFrameworkCore;
using Taskify.Models;

namespace Taskify.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
}
