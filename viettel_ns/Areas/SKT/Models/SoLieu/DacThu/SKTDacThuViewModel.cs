using AutoMapper.Attributes;
using AutoMapper.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using VIETTEL.Application;

namespace VIETTEL.Areas.SKT.Models
{
    [MapsFrom(typeof(SKT_DacThuChungTu), ReverseMap = true)]
    public class DacThuChungTuViewModel
    {
        public virtual Guid Id { get; set; }  
        [Required]
        public virtual string Id_Nganh { get; set; }
        public virtual int iLan { get; set; }
        [MvcDisplayName("Ng")]
        public virtual string TenNganh { get; set; }
        [MvcDisplayName("NS.NgayChungTu")]
        public DateTime NgayChungTu { get; set; }
        [MvcDisplayName("NS.NoiDung")]
        public virtual string NoiDung { get; set; }
    }

    [MapsFrom(typeof(SKT_DacThuChungTu), ReverseMap = true)]
    public class DacThuChungTuEditViewModel : DacThuChungTuViewModel
    {
        public IEnumerable<SelectListItem> NganhList { get; set; }
    }

    [MapsFrom(typeof(SKT_DacThuChungTu), ReverseMap = true)]
    public class DacThuChungTuDetailsViewModel : DacThuChungTuViewModel
    {
        public virtual int? SoChungTu { get; set; }
        public virtual DateTime? DateCreated { get; set; }
        public virtual string UserCreator { get; set; }
    }

    public class DacThuChungTuListViewModel
    {
        public IEnumerable<DacThuChungTuDetailsViewModel> Items { get; set; }
    }

    public static partial class ModelHelpers
    {
        public static DacThuChungTuViewModel ToViewModel(this SKT_DacThuChungTu entity)
        {
            var vm = entity.MapTo<DacThuChungTuViewModel>();

            return vm;
        }



    }
}
