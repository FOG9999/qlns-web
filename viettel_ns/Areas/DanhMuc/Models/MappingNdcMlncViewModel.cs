using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using VIETTEL.Models;

namespace VIETTEL.Areas.DanhMuc.Models
{
    public class MappingNdcMlncViewModel
    {
        public SheetViewModel Sheet { get; set; }

        public int Filter { get; set; }

        public Dictionary<int, string> FilterOptions { get; set; }

        public IEnumerable<NNSDMNoiDungChiViewModel> lstDMNoiDungChi { get; set; }

        public Guid iID_NoiDungChi { get; set; }

        public string sMaNoiDungChi { get; set; }

        public string sTenNoiDungChi { get; set; }
    }
}