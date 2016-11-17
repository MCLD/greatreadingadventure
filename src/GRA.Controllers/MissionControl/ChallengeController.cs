using GRA.Controllers.ViewModel.Challenge;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Repository;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    public class ChallengeController : Base.Controller
    {
        private readonly ILogger<ChallengeController> logger;
        private readonly ChallengeService challengeService;
        public ChallengeController(ILogger<ChallengeController> logger,
            ServiceFacade.Controller context,
            ChallengeService challengeService)
            : base(context)
        {
            this.logger = logger;

            if (challengeService == null)
            {
                throw new ArgumentNullException(nameof(challengeService));
            }
            this.challengeService = challengeService;
        }

        public IActionResult Index(string Search, string FilterBy, int? page)
        {
            PaginateViewModel paginateModel = new PaginateViewModel();

            int currentPage = page ?? 1;
            int take = 15;
            int skip = take * (currentPage - 1);

            var challengeList = challengeService.GetPaginatedChallengeList(null, 0, 50);

            ChallengeListViewModel viewModel = new ChallengeListViewModel();

            viewModel.Challenges = challengeList.Skip(skip).Take(take).ToList();
            paginateModel.ItemCount = challengeList.Count();
            paginateModel.CurrentPage = currentPage;
            paginateModel.ItemsPerPage = take;

            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }
            viewModel.PaginateModel = paginateModel;
            return View(viewModel);
        }

        public IActionResult Create()
        {
            ChallengeDetailViewModel viewModel = new ChallengeDetailViewModel();
            return View("Detail", viewModel);
        }

        [HttpPost]
        public IActionResult Create(ChallengeDetailViewModel viewModel)
        {
            
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            return View("Detail");
        }

        [HttpPost]
        public IActionResult Edit(ChallengeDetailViewModel viewModel)
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            return RedirectToAction("Index");
        }
    }
}
