using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Catalog
    {
        [Column(TypeName = "uniqueidentifier")]
        public Guid CatalogId { get; set; }
        [Column(TypeName = "uniqueidentifier")]
        public Guid? ParentId { get; set; }

        public string Name { get; set; }

        public Catalog(string name, Guid? parentId = null)
        {
            CatalogId = Guid.NewGuid();
            ParentId = parentId;
            Name = name;
        }
    }

}
