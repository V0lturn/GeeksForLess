using WebApplication1.Models;
using WebApplication1.AdditionalClasses;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace WebApplication1.Controllers
{
    public class MainPageController : Controller
    {
        private readonly AppDBContext _dbContext;

        public MainPageController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var RootCatalogs = _dbContext.Catalogs.Where(c => c.ParentId == null).ToList();

            if (!RootCatalogs.Any())
            {
                ViewBag.ErrorMessage = "Корневих каталогів не знайдено";
                return View();
            }
            else
            {
                ViewBag.Title = "Корневі каталоги";
                return View(RootCatalogs);
            }
        }

        public IActionResult GetFolderStructureFromFile(Guid catalogId)
        {
            var folderContent = _dbContext.Catalogs.Where(c => c.ParentId == catalogId).ToList();
            var folder = _dbContext.Catalogs.FirstOrDefault(c => c.CatalogId == catalogId);

            if (!folderContent.Any())
            {
                ViewBag.ErrorMessage = "Дочірніх каталогів не знайдено";
                return View("Index");
            }
            else
            {
                ViewBag.Title = $"Вміст папки {folder.Name}";
                return View("Index", folderContent);
            }
        }

        [HttpPost]
        public IActionResult GetFolderFromPC(IFormFile folderPath)
        {
            if (folderPath != null)
            {
                using (var streamReader = new StreamReader(folderPath.OpenReadStream()))
                {
                    string line;
                    List<Catalog> catalogs = new List<Catalog>();

                    while ((line = streamReader.ReadLine()) != null)
                    {
                        Guid? parentId = null;

                        if (line.StartsWith("Root directory:"))
                        {
                            string folderName = line.Replace("Root directory: ", "");               // имя каталога
                            var rootFolder = new Catalog(folderName);
                            catalogs.Add(rootFolder);
                        }
                        else if (line.StartsWith("Subdirectories of"))
                        {
                            string[] parts = line.Split('"');
                            string rootFolderOfSubDir = parts[1];                                   // имя каталога, в котором находятся дочерние

                            string[] subCatalogs = parts[2].Split(",");                             // все дочерние каталоги
                            subCatalogs[0] = subCatalogs[0].Replace(":", "");

                            foreach (var catalog in catalogs)                                       // определение parentId для дочерних каталогов
                            {
                                if (catalog.Name == rootFolderOfSubDir)
                                {
                                    parentId = catalog.CatalogId;
                                    break;
                                }
                            }

                            foreach (var catalog in subCatalogs)
                            {
                                var subFolder = new Catalog(catalog.Trim(), parentId);
                                catalogs.Add(subFolder);
                            }

                        }
                    }

                    _dbContext.Catalogs.AddRange(catalogs);
                    _dbContext.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult ExportToTxt()
        {
            var data = ExportToFile.ExportCatalogHierarchy(_dbContext);                             // данные для записи в файл

            var contentType = "text/plain";                                                         // MIME тип для текстового файла

            var result = new ContentResult
            {
                Content = data,
                ContentType = contentType,
                StatusCode = 200
            };

            var contentDisposition = new Microsoft.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = "export.txt"
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

            return result;
        }
    }
}
