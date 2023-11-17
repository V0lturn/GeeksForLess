using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;   
namespace WebApplication1
{
    public class AppDBContext : DbContext               
    {
        public DbSet<Catalog> Catalogs { get; set; }

        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating (ModelBuilder modelBuilder)
        {
            var RootCatalog = new Catalog("Creating Digital Images");                                               // создание структуры, данной в ТЗ

            var ResourcesCatalog = new Catalog("Resources", RootCatalog.CatalogId);
            var EvidenceCatalog = new Catalog("Evidence", RootCatalog.CatalogId);
            var GraphicProductsCatalog = new Catalog("Graphic Products", RootCatalog.CatalogId);

            var PrimarySourcesCatalog = new Catalog("Primary Sources", ResourcesCatalog.CatalogId);
            var SecondarySourcesCatalog = new Catalog("Secondary Sources", ResourcesCatalog.CatalogId);

            var ProcessCatalog = new Catalog("Process", GraphicProductsCatalog.CatalogId);
            var FinalProductCatalog = new Catalog("Final Product", GraphicProductsCatalog.CatalogId);

            modelBuilder.Entity<Catalog>().HasData(RootCatalog, ResourcesCatalog, EvidenceCatalog, GraphicProductsCatalog,  // добавления записей в таблицу 
                    PrimarySourcesCatalog, SecondarySourcesCatalog, ProcessCatalog, FinalProductCatalog);
        }
    }
}
