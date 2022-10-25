using AutoMapper.Attributes;
using AutoMapper.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using VIETTEL.Application;
using VIETTEL.Models;

namespace VIETTEL.Areas.DuToanKT.Models
{
    [MapsFrom(typeof(DTKT_ChungTu), ReverseMap = true)]
    public class ChungTuViewModel
    {
        public virtual Guid Id { get; set; }

        //public virtual string iID_MaPhongBan { get; set; }
        //public virtual string sTenPhongBan { get; set; }
        //public virtual string iID_MaPhongBanDich { get; set; }

        [Required]
        public virtual string Id_DonVi { get; set; }
        public virtual string Id_PhongBan { get; set; }
        public virtual string Id_PhongBanDich { get; set; }

        [Required]
        [MvcDisplayName("NS.Input")]
        public virtual int iRequest { get; set; }
        public virtual string Request { get; set; }
        public virtual int iLan { get; set; }

        [MvcDisplayName("NS.DonVi")]
        public virtual string TenDonVi { get; set; }


        [MvcDisplayName("NS.NgayChungTu")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd'/'MM'/'yyyy}")]
        public DateTime NgayChungTu { get; set; }

        [MvcDisplayName("NS.NoiDung")]
        public virtual string NoiDung { get; set; }

        public int iLoai { get; set; }
    }


    [MapsFrom(typeof(DTKT_ChungTu), ReverseMap = true)]
    public class ChungTuEditViewModel : ChungTuViewModel
    {
        [MvcDisplayName("NS.DonVi")]
        public IEnumerable<SelectListItem> PhongBanList { get; set; }
        public IEnumerable<SelectListItem> DonViList { get; set; }
        public IEnumerable<SelectListItem> RequestList { get; set; }
    }

    [MapsFrom(typeof(DTKT_ChungTu), ReverseMap = true)]
    public class ChungTuDetailsViewModel : ChungTuViewModel
    {
        public virtual int? SoChungTu { get; set; }

        public virtual DateTime? DateCreated { get; set; }
        public virtual string UserCreator { get; set; }
    }


    public class ChungTuListViewModel
    {
        public IEnumerable<ChungTuDetailsViewModel> Items { get; set; }

        public IEnumerable<KeyViewModel> OrderBys { get; set; }

        public int Loai { get; set; }
    }

    public static partial class ModelHelpers
    {
        public static ChungTuViewModel ToViewModel(this DTKT_ChungTu entity)
        {
            var vm = entity.MapTo<ChungTuViewModel>();

            return vm;
        }



    }
}
