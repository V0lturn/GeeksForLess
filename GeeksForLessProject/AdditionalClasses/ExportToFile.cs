using Microsoft.EntityFrameworkCore;
using System.Text;

namespace WebApplication1.AdditionalClasses
{
    public static class ExportToFile
    {
        public static string ExportCatalogHierarchy(AppDBContext _dbContext)
        {
            StringBuilder result = new StringBuilder();

            var rootCatalogs = _dbContext.Catalogs.Where(c => c.ParentId == null).ToList();                                                 // корневые каталоги

            foreach (var rootCatalog in rootCatalogs)
            {
                result.AppendLine($"Root directory: {rootCatalog.Name}");

                var subCatalogs = _dbContext.Catalogs.Where(c => c.ParentId == rootCatalog.CatalogId).ToList();                             // дочерние каталоги для кожного корневого каталога

                if (subCatalogs.Any())
                {
                    result.AppendLine($"Subdirectories of \"{rootCatalog.Name}\": {string.Join(", ", subCatalogs.Select(c => c.Name))}");

                    foreach (var subCatalog in subCatalogs)
                    {
                        result.Append(GetSubdirectories(subCatalog.CatalogId, _dbContext));                                               // рекурсивно обрабатываем вложенные каталоги
                    }
                }
            }

            return result.ToString();
        }

        private static string GetSubdirectories(Guid catalogId, AppDBContext _dbContext)
        {
            StringBuilder result = new StringBuilder();

            var currentCatalog = _dbContext.Catalogs.FirstOrDefault(c => c.CatalogId == catalogId);
            var subCatalogs = _dbContext.Catalogs.Where(c => c.ParentId == currentCatalog.CatalogId).ToList();

            if (subCatalogs.Any() && currentCatalog != null)
            {
                result.Append($"Subdirectories of \"{currentCatalog.Name}\": ");

                var subCatalogNames = subCatalogs.Select(subCatalog => subCatalog.Name);                                                // получение имен папок
                string subDirectoriesString = string.Join(", ", subCatalogNames);                                                       // запись в нужном формате

                result.AppendLine(subDirectoriesString);

                foreach (var subCatalog in subCatalogs)
                {
                    result.Append(GetSubdirectories(subCatalog.CatalogId, _dbContext));                                                 // Рекурсивный вызов для обработки подкаталогов
                }
            }

            return result.ToString();
        }
    }
}
