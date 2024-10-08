﻿using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class BookListViewModel : ParticipantPartialViewModel
    {
        public BookListViewModel()
        {
        }

        public BookListViewModel(ParticipantPartialViewModel viewModel) : base(viewModel)
        {
        }

        public bool OpenToLog { get; set; }
        public Domain.Model.Book Book { get; set; }
        public List<Domain.Model.Book> Books { get; set; }
        public bool CanEditBooks { get; set; }
        public bool HasPendingQuestionnaire { get; set; }
        public int? HeadOfHouseholdId { get; set; }
        public bool IsDescending { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public string Sort { get; set; }
        public System.Array SortBooks { get; set; }
    }
}
