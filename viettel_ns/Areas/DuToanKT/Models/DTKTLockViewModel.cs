using AutoMapper.Attributes;
using System.Collections.Generic;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using VIETTEL.Application;

namespace VIETTEL.Areas.DuToanKT.Models
{
    [MapsFrom(typeof(DTKT_Lock), ReverseMap = true)]
    public class DTKTLockViewModel
    {
        public virtual int Id { get; set; }
        public virtual string Id_DonVi { get; set; }
        [MvcDisplayName("NS.DonVi")]
        public virtual string TenDonVi { get; set; }
        [MvcDisplayName("DTKT.Lock_PhongBan")]
        public virtual string Id_PhongBan { get; set; }
        [MvcDisplayName("DTKT.Lock_Account")]
        public virtual string Id_User { get; set; }
        [MvcDisplayName("DTKT.Lock_Times")]
        public virtual string iRequest { get; set; }

        [MvcDisplayName("NS.GhiChu")]
        public virtual string GhiChu { get; set; }


        public SelectList PhongBanList { get; set; }
    }

    public class DTKTLockIdViewModel
    {
        public SelectList LockList { get; set; }
    }

    public class DTKTLockListViewModel
    {
        public IEnumerable<DTKTLockViewModel> Items { get; set; }
    }
}
