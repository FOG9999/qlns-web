using System;

namespace Viettel.Domain.DomainModel
{
    public class PagingInfo
    {
        public int TotalItems { get; set; }

        public int ItemsPerPage { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage); }
        }

        public int ItemForm
        {
            get
            {
                return (CurrentPage - 1) * ItemsPerPage + 1; 
            }
        }

        public int ItemTo
        {
            get
            {
                //if (CurrentPage * ItemsPerPage > TotalItems) return TotalItems - ((CurrentPage - 1) * ItemsPerPage);
                if (CurrentPage * ItemsPerPage > TotalItems) return TotalItems;
                return CurrentPage * ItemsPerPage;
            }
        }
    }
}