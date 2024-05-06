using Microsoft.EntityFrameworkCore;

namespace pruebaCortaIDWM
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {}
        
        public  DbSet<Chair> Chairs => Set<Chair>();
    }
}