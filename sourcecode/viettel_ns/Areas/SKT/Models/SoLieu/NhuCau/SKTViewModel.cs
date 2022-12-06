using AutoMapper.Attributes;
using AutoMapper.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using VIETTEL.Application;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Models
{
    [MapsFrom(typeof(SKT_ChungTu), ReverseMap = true)]
    public class ChungTuViewModel
    {
        public virtual Guid Id { get; set; }        

        [Required]
        public virtual string Id_DonVi { get; set; }
        public virtual string Id_PhongBan { get; set; }
        public virtual string Id_PhongBanDich { get; set; }

        [Required]
        [MvcDisplayName("NS.Input")]
        public virtual int iLan { get; set; }
        [MvcDisplayName("NS.DonVi")]
        public virtual string TenDonVi { get; set; }
        [MvcDisplayName("SKT.PhongBan")]
        public virtual string TenPhongBan { get; set; }
        [MvcDisplayName("SKT.PhongBanDich")]
        public virtual string TenPhongBanDich { get; set; }
        
        [MvcDisplayName("NS.NoiDung")]
        public virtual string NoiDung { get; set; }
        public int iLoai { get; set; }
    }


    [MapsFrom(typeof(SKT_ChungTu), ReverseMap = true)]
    public class ChungTuEditViewModel : ChungTuViewModel
    {
        [MvcDisplayName("NS.DonVi")]
        public IEnumerable<SelectListItem> PhongBanList { get; set; }
        public IEnumerable<SelectListItem> DonViList { get; set; }
    }

    [MapsFrom(typeof(SKT_ChungTu), ReverseMap = true)]
    public class ChungTuDetailsViewModel : ChungTuViewModel
    {
        public virtual int? SoChungTu { get; set; }
        public DateTime NgayChungTu { get; set; }
        public virtual DateTime? DateCreated { get; set; }
        public virtual string UserCreator { get; set; }
        public virtual bool B_Locked { get; set; }
    }


    public class ChungTuListViewModel
    {
        public IEnumerable<ChungTuDetailsViewModel> Items { get; set; }
        public IEnumerable<KeyViewModel> OrderBys { get; set; }
        public IEnumerable<KeyViewModel> Dvis { get; set; }
        public int Loai { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
    
    public static partial class ModelHelpers
    {
        public static ChungTuViewModel ToViewModel(this SKT_ChungTu entity)
        {
            var vm = entity.MapTo<ChungTuViewModel>();

            return vm;
        }

    }

    public class SKTChuyenViewModel
    {
        public IEnumerable<SelectListItem> DonViList { get; set; }
    }
}
