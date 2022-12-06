using AutoMapper.Attributes;
using AutoMapper.Extensions;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using VIETTEL.Application;

namespace VIETTEL.Areas.QuyetToan.Models
{
    [MapsFrom(typeof(QTA_ChungTu))]
    public class ChungTuViewModel
    {
        public ChungTuViewModel()
        {
        }

        #region properties

        public virtual Guid iID_MaChungTu { get; set; }
        public virtual string iID_MaDonVi { get; set; }


        public virtual string sTienToChungTu { get; set; }
        public virtual int iLoai { get; set; }

        public virtual int iSoChungTu { get; set; }

        [MvcDisplayName("NS.LNS")]
        public virtual string sDSLNS { get; set; }
        [MvcDisplayName("NS.NgayChungTu")]
        public virtual DateTime dNgayChungTu { get; set; }

        [MvcDisplayName("NS.Quy")]
        public virtual int iThang_Quy { get; set; }
        [MvcDisplayName("NS.NoiDung")]
        public virtual string sNoiDung { get; set; }
        public virtual string sLyDo { get; set; }

        [IgnoreMapFrom(typeof(QTA_ChungTu))]
        public string sTrangThaiDuyet { get; set; }
        [IgnoreMapFrom(typeof(QTA_ChungTu))]
        [MvcDisplayName("NS.TenDonVi")]
        public virtual string sTenDonVi { get; set; }
        [IgnoreMapFrom(typeof(QTA_ChungTu))]
        public virtual string sSoChungTu { get; set; }

        #endregion

        public SelectList DonViList { get; set; }
        public SelectList LoaiNganSachList { get; set; }
        public SelectList QuyList { get; set; }

    }

    public class ChungTuListViewModel
    {
        public IEnumerable<ChungTuViewModel> Items { get; set; }

        public string Type { get; set; }
    }


    public static partial class ModelHelpers
    {
        public static ChungTuViewModel ToViewModel(this QTA_ChungTu entity)
        {
            var vm = entity.MapTo<ChungTuViewModel>();

            vm.sSoChungTu = vm.sTienToChungTu + vm.iSoChungTu;

            return vm;
        }



    }


}
