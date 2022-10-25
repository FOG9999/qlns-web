using AutoMapper.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using VIETTEL.Application;
using VIETTEL.Models;

namespace VIETTEL.Areas.QuyetToan.Models
{
    public class ChungTuSearchViewModel
    {
        public string MaPhongBan { get; set; }
        public string Loai { get; set; }
        public string MaDonVi { get; set; }
        public string iSoChungTu { get; set; }
        public string sTuNgay { get; set; }
        public string sDenNgay { get; set; }
        public string iID_MaTrangThaiDuyet { get; set; }
        public string sThangQuy { get; set; }
        public bool LayTheoMaNDTao { get; set; }

        public int CurrentPage { get; set; }
    }

    public class ChungTuItemViewModel
    {
        public ChungTuItemViewModel()
        {
            TrangThaiDuyet = new TrangThaiDuyetViewModel();
        }
        public string iLoai { get; set; }

        public int iNamLamViec { get; set; }

        public Guid iID_MaChungTu { get; set; }

        public string iID_MaDonVi { get; set; }

        public string sTenDonVi { get; set; }

        public int iSoChungTu { get; set; }

        public string sDSLNS { get; set; }

        public DateTime dNgayChungTu { get; set; }

        public int iThang_Quy { get; set; }

        public int bLoaiThang_Quy { get; set; }

        public string strThoiGianQuyetToan
        {
            get
            {
                var thoigian = "";
                switch (iLoai)
                {
                    case "-1":
                        thoigian = $"Tháng: {iThang_Quy}/{iNamLamViec}";
                        break;
                    default:
                        if (iThang_Quy == 10)
                        {
                            thoigian = $"Tháng: {iThang_Quy}/{iNamLamViec}";
                        }
                        else
                        {
                            if (iThang_Quy == 1)
                            {
                                thoigian = $"Quý: I/ {iNamLamViec}";
                            }
                            else if (iThang_Quy == 2)
                            {
                                thoigian = $"Quý: I/ {iNamLamViec}";
                            }
                            else if (iThang_Quy == 3)
                            {
                                thoigian = $"Quý: I/ {iNamLamViec}";
                            }
                            else if (iThang_Quy == 4)
                            {
                                thoigian = $"Quý: I/ {iNamLamViec}";
                            }
                            else if (iThang_Quy == 5)
                            {
                                thoigian = $"Quý: Bổ sung/ {iNamLamViec}";
                            }

                        }
                        break;
                }

                return thoigian;
            }
        }

        public string sNoiDung { get; set; }

        public int iSoTien { get; set; }

        public int iID_MaTrangThaiDuyet { get; set; }

        public TrangThaiDuyetViewModel TrangThaiDuyet { get; set; }
    }


    public class ChungTuItemDetailsViewModel
    {
        public ChungTuItemDetailsViewModel()
        {
        }
        public string iLoai { get; set; }

        public int iNamLamViec { get; set; }

        public Guid iID_MaChungTu { get; set; }

        public string iID_MaDonVi { get; set; }

        //[AutoMapper.IgnoreMap]
        //public string sTenDonVi { get; set; }

        public int iSoChungTu { get; set; }

        public string sDSLNS { get; set; }

        public DateTime dNgayChungTu { get; set; }

        public int iThang_Quy { get; set; }

        public int bLoaiThang_Quy { get; set; }

        public string strThoiGianQuyetToan
        {
            get
            {
                var thoigian = "";
                switch (iLoai)
                {
                    case "-1":
                        thoigian = $"Tháng: {iThang_Quy}/{iNamLamViec}";
                        break;
                    default:
                        if (iThang_Quy == 10)
                        {
                            thoigian = $"Tháng: {iThang_Quy}/{iNamLamViec}";
                        }
                        else
                        {
                            if (iThang_Quy == 1)
                            {
                                thoigian = $"Quý: I/ {iNamLamViec}";
                            }
                            else if (iThang_Quy == 2)
                            {
                                thoigian = $"Quý: I/ {iNamLamViec}";
                            }
                            else if (iThang_Quy == 3)
                            {
                                thoigian = $"Quý: I/ {iNamLamViec}";
                            }
                            else if (iThang_Quy == 4)
                            {
                                thoigian = $"Quý: I/ {iNamLamViec}";
                            }
                            else if (iThang_Quy == 5)
                            {
                                thoigian = $"Quý: Bổ sung/ {iNamLamViec}";
                            }

                        }
                        break;
                }

                return thoigian;
            }
        }

        public string sNoiDung { get; set; }

        //public int iSoTien { get; set; }

        public int iID_MaTrangThaiDuyet { get; set; }
    }


    //public class ChungTuViewModel
    //{
    //    public ChungTuViewModel()
    //    {
    //        Items = new List<ChungTuItemViewModel>();
    //    }
    //    public ChungTuSearchViewModel Search { get; set; }

    //    public IEnumerable<ChungTuItemViewModel> Items { get; set; }
    //}


    [MapsFrom(typeof(QTA_ChungTu), ReverseMap = true)]
    public class ChungTuEditViewModel
    {
        [MvcDisplayName("NS.SoChungTu")]
        public int iSoChungTu { get; set; }

        public Guid? iID_MaChungTu { get; set; }

        public int iLoai { get; set; }

        [MvcDisplayName("NS.DonVi")]
        public string iID_MaDonVi { get; set; }
        public IEnumerable<SelectListItem> DonViList { get; set; }

        [MvcDisplayName("NS.LNS")]
        public string sDSLNS { get; set; }
        public IEnumerable<SelectListItem> LNSList { get; set; }

        [MvcDisplayName("NS.NgayChungTu")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd'/'MM'/'yyyy}")]
        public DateTime dNgayChungTu { get; set; }

        [MvcDisplayName("NS.Quy")]
        public int iThang_Quy { get; set; }
        public IEnumerable<SelectListItem> QuyList { get; set; }

        [MvcDisplayName("NS.NoiDung")]
        public string sNoiDung { get; set; }
    }
}
