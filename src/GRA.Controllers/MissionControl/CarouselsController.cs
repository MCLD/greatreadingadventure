using System;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Carousels;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageCarousels)]
    public class CarouselsController : Base.MCController
    {
        private const string DetailViewName = "Detail";
        private const string ItemDetailViewName = "ItemDetail";

        private readonly ILogger _logger;
        private readonly CarouselService _carouselService;

        public CarouselsController(ILogger<CarouselsController> logger,
            ServiceFacade.Controller context,
            CarouselService carouselService) : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _carouselService = carouselService
                ?? throw new ArgumentNullException(nameof(carouselService));
            PageTitle = "Carousels";
        }

        public async Task<IActionResult> Index(int page = 1, bool archived = false)
        {
            var filter = new BaseFilter(page)
            {
                IsActive = !archived
            };

            var carousels = await _carouselService.GetPaginatedListAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = carousels.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };

            if (paginateModel.PastMaxPage)
            {
                return RedirectToRoute(new { page = paginateModel.LastPage ?? 1 });
            }

            var viewModel = new ListViewModel
            {
                Carousels = carousels.Data,
                PaginateModel = paginateModel
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new DetailViewModel
            {
                PageAction = ViewModel.PageAction.New
            };
            return View(DetailViewName, viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DetailViewModel viewModel)
        {
            viewModel.Carousel.IsForDashboard = true;
            viewModel.Carousel.SiteId = GetCurrentSiteId();
            // create
            if (ModelState.IsValid)
            {
                var addedCarousel = await _carouselService.AddAsync(viewModel.Carousel);
                AlertSuccess = $"Added carousel '<strong>{addedCarousel.Name}</strong>'";

                return RedirectToAction(nameof(Edit), new { id = addedCarousel.Id });
            }
            else
            {
                AlertDanger = "Found {ModelState.ErrorCount.ToString()} errors";
                return View(DetailViewName, viewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var carousel = await _carouselService.GetCarouselAsync(id);
            if (carousel == null)
            {
                AlertDanger = $"Could not find carousel '<strong>{id}</strong>'.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(DetailViewName, new DetailViewModel
                {
                    Carousel = carousel,
                    PageAction = ViewModel.PageAction.Edit
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DetailViewModel viewModel)
        {
            var updatedCarousel = await _carouselService.EditAsync(viewModel.Carousel);

            AlertSuccess = $"Updated carousel '<strong>{updatedCarousel.Name}</strong>'";

            return RedirectToAction(nameof(Edit), new { id = updatedCarousel.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var carousel = await _carouselService.GetCarouselAsync(id);
            if (carousel == null)
            {
                AlertDanger = $"Could not find carousel '<strong>{id}</strong>'.";
            }
            else
            {
                string carouselName = carousel.Name;
                await _carouselService.RemoveAsync(id);
                AlertWarning = $"Deleted carousel '<strong>{carouselName}</strong>'.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> AddItem(int id)
        {
            var carousel = await _carouselService.GetCarouselAsync(id);
            if (carousel == null)
            {
                AlertDanger = $"Could not find carousel '<strong>{id}</strong>'.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(ItemDetailViewName, new ItemDetailViewModel
                {
                    CarouselId = carousel.Id,
                    CarouselName = carousel.Name,
                    PageAction = ViewModel.PageAction.New
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(int id, ItemDetailViewModel viewModel)
        {
            viewModel.CarouselItem.CarouselId = id;
            var addedItem = await _carouselService.AddItemAsync(viewModel.CarouselItem);
            AlertSuccess = $"Added carousel item '<strong>{addedItem.Title}</strong>'";
            return RedirectToAction(nameof(Edit), new { id = addedItem.CarouselId });
        }

        [HttpGet]
        public async Task<IActionResult> EditItem(int id)
        {
            var item = await _carouselService.GetItemAsync(id);
            var carousel = await _carouselService.GetCarouselAsync(item.CarouselId);
            return View(ItemDetailViewName, new ItemDetailViewModel
            {
                CarouselId = carousel.Id,
                CarouselName = carousel.Name,
                PageAction = ViewModel.PageAction.Edit,
                CarouselItem = item
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditItem(int id, ItemDetailViewModel viewModel)
        {
            viewModel.CarouselItem.Id = id;
            var updatedItem = await _carouselService.EditItemAsync(viewModel.CarouselItem);

            AlertSuccess = $"Updated carousel item '<strong>{updatedItem.Title}</strong>'";

            return RedirectToAction(nameof(Edit), new { id = updatedItem.CarouselId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var carouselItem = await _carouselService.GetItemAsync(id);
            if (carouselItem == null)
            {
                AlertDanger = $"Could not find carousel item '<strong>{id}</strong>'.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                string itemTitle = carouselItem.Title;
                int carouselId = carouselItem.CarouselId;
                await _carouselService.DeleteItemAsync(id);
                AlertWarning = $"Deleted carousel item '<strong>{itemTitle}</strong>'.";
                return RedirectToAction(nameof(Edit), new { id = carouselId });
            }
        }
    }
}
