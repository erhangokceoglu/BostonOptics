using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Interfaces;
using Web.Models;

namespace Web.Services
{
    public class HomeViewModelService : IHomeViewModelService
    {
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Brand> _brandRepo;

        public HomeViewModelService(IRepository<Category> categoryRepo, IRepository<Product> productRepo, IRepository<Brand> brandRepo)
        {
            _categoryRepo = categoryRepo;
            _productRepo = productRepo;
            _brandRepo = brandRepo;
        }
        public async Task<HomeViewModel> GetHomeViewModelAsync(int? categoryId, int? brandId, int pageId)
        {
            var specAllProducts = new ProductsFilterSpecification(categoryId, brandId);
            var countAll = await _productRepo.CountAsync(specAllProducts);
            var specProducts = new ProductsFilterSpecification(categoryId, brandId, (pageId - 1) * Constants.ITEMS_PER_PAGE, Constants.ITEMS_PER_PAGE);
            var products = await _productRepo.GetAllAsync(specProducts);
            var vm = new HomeViewModel()
            {
                Products = products.Select(x => new ProductViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    PictureUri = x.PictureUri,
                    Price = x.Price
                }).ToList(),
                Categories = await GetCategoriesAsync(),
                Brands = await GetBrandsAsync(),
                CategoryId = categoryId,
                BrandId = brandId,
                PaginationInfo = new PaginationInfoViewModel()
                {
                    PageId = pageId,
                    ItemsOnPage = products.Count(),
                    TotalItems = countAll
                }
            };
            return vm;
        }

        private async Task<List<SelectListItem>> GetBrandsAsync()
        {
            var brands = await _brandRepo.GetAllAsync();
            return brands.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }

        private async Task<List<SelectListItem>> GetCategoriesAsync()
        {
            var categories = await _categoryRepo.GetAllAsync();
            return categories.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }
    }
}
