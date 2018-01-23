using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.Shared
{
    public class PaginateViewModel
    {
        public int CurrentPage { get; set; }
        public int ItemCount { get; set; }
        public int ItemsPerPage { get; set; }

        public int? NextPage
        {
            get
            {
                if (this.CurrentPage < this.LastPage)
                {
                    return this.CurrentPage + 1;
                }
                return null;
            }
        }
        public int? PreviousPage
        {
            get
            {
                if (this.CurrentPage > 1)
                {
                    return this.CurrentPage - 1;
                }
                return null;
            }
        }
        public int? FirstPage
        {
            get
            {
                if (this.CurrentPage > 1)
                {
                    return 1;
                }
                return null;
            }
        }
        public int? LastPage
        {
            get
            {
                if (this.ItemCount > this.ItemsPerPage)
                {
                    int last = this.MaxPage;
                    if (this.CurrentPage != last)
                    {
                        return last;
                    }
                }
                return null;
            }
        }
        public int MaxPage
        {
            get
            {
                if (this.ItemsPerPage == 0)
                {
                    return 0;
                }
                int last = this.ItemCount / this.ItemsPerPage;
                if (this.ItemCount % this.ItemsPerPage > 0)
                {
                    last++;
                }
                return last;
            }
        }

        public bool PastMaxPage
        {
            get
            {
                return MaxPage > 0 && CurrentPage > MaxPage;
            }
        }
    }
}
