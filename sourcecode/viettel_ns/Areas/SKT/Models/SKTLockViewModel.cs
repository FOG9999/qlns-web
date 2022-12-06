using AutoMapper.Attributes;
using System.Collections.Generic;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using VIETTEL.Application;

namespace VIETTEL.Areas.SKT.Models
{
    [MapsFrom(typeof(SKT_Lock), ReverseMap = true)]
    public class SKTLockViewModel
    {
        public virtual int Id { get; set; }
        public virtual string Id_DonVi { get; set; }
        [MvcDisplayName("NS.DonVi")]
        public virtual string TenDonVi { get; set; }
        [MvcDisplayName("SKT.Lock_PhongBan")]
        public virtual string Id_PhongBan { get; set; }
        [MvcDisplayName("SKT.Lock_Account")]
        public virtual string Id_User { get; set; }
        [MvcDisplayName("NS.GhiChu")]
        public virtual string GhiChu { get; set; }


        public SelectList PhongBanList { get; set; }
    }

    public class SKTLockIdViewModel
    {
        public SelectList LockList { get; set; }
    }

    public class SKTLockListViewModel
    {
        public IEnumerable<SKTLockViewModel> Items { get; set; }
    }
}
