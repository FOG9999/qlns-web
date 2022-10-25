using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class VDT_QT_TongHopSoLieuCreateViewModel
    {
        public VDT_QT_TongHopSoLieuCreateViewModel()
        {

        }
        public TongHopSoLieuChiTietViewModel TongHopSoLieu { get; set; }
        public VDT_QT_XuLySoLieuViewModel XuLySoLieuNamTruoc { get; set; }
        public VDT_QT_XuLySoLieuViewModel XuLySoLieuNamSau { get; set; }

        public void FormatNumber()
        {
            // Tổng hợp số liệu

            if(TongHopSoLieu != null)
            {

                TongHopSoLieu.TongChiTieu = TongHopSoLieu.ChiTieuNamNay + TongHopSoLieu.ChiTieuNamTruoc;
                TongHopSoLieu.TongCapPhat = TongHopSoLieu.CapPhatNamNay + TongHopSoLieu.CapPhatNamTruoc;

                if (TongHopSoLieu.CapPhatNamNay != 0)
                {
                    TongHopSoLieu.CapPhatNamNayAsString = string.Format("{0:0,0}", TongHopSoLieu.CapPhatNamNay);
                }

                if (TongHopSoLieu.CapPhatNamTruoc != 0)
                {
                    TongHopSoLieu.CapPhatNamTruocAsString = string.Format("{0:0,0}", TongHopSoLieu.CapPhatNamTruoc);
                }

                if (TongHopSoLieu.ChiTieuNamNay != 0)
                {
                    TongHopSoLieu.ChiTieuNamNayAsString = string.Format("{0:0,0}", TongHopSoLieu.ChiTieuNamNay);
                }

                if (TongHopSoLieu.ChiTieuNamTruoc != 0)
                {
                    TongHopSoLieu.ChiTieuNamTruocAsString = string.Format("{0:0,0}", TongHopSoLieu.ChiTieuNamTruoc);
                }

                if (TongHopSoLieu.fDonViiDeNghiQuyetToanNamNay != 0)
                {
                    TongHopSoLieu.fDonViiDeNghiQuyetToanNamNayAsString = string.Format("{0:0,0}", TongHopSoLieu.fDonViiDeNghiQuyetToanNamNay);
                }

                if (TongHopSoLieu.fDonViiDeNghiQuyetToanNamTruoc != 0)
                {
                    TongHopSoLieu.fDonViiDeNghiQuyetToanNamTruocAsString = string.Format("{0:0,0}", TongHopSoLieu.fDonViiDeNghiQuyetToanNamTruoc);
                }

                if (TongHopSoLieu.fTroLyDeNghiQuyetToanNamNay != 0)
                {
                    TongHopSoLieu.fTroLyDeNghiQuyetToanNamNayAsString = string.Format("{0:0,0}", TongHopSoLieu.fTroLyDeNghiQuyetToanNamNay);
                }

                if (TongHopSoLieu.fTroLyDeNghiQuyetToanNamTruoc != 0)
                {
                    TongHopSoLieu.fTroLyDeNghiQuyetToanNamTruocAsString = string.Format("{0:0,0}", TongHopSoLieu.fTroLyDeNghiQuyetToanNamTruoc);
                }
            }

            // Xử lý số liệu năm sau

            if(XuLySoLieuNamSau != null)
            {

                if (XuLySoLieuNamSau.fGiaTriChuyenNamSauDaCap.HasValue)
                {
                    XuLySoLieuNamSau.fTongGiaTriChuyenNamSau = XuLySoLieuNamSau.fGiaTriChuyenNamSauDaCap.Value;
                }
                else
                {
                    XuLySoLieuNamSau.fTongGiaTriChuyenNamSau = 0;
                }
                if (XuLySoLieuNamSau.fGiaTriChuyenNamSauChuaCap.HasValue)
                {
                    XuLySoLieuNamSau.fTongGiaTriChuyenNamSau += XuLySoLieuNamSau.fGiaTriChuyenNamSauChuaCap.Value;
                }

                if (XuLySoLieuNamSau.fCapPhat != 0)
                {
                    XuLySoLieuNamSau.fCapPhatAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fCapPhat);
                }

                if (XuLySoLieuNamSau.fCapPhatNamTruoc != 0)
                {
                    XuLySoLieuNamSau.fCapPhatNamTruocAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fCapPhatNamTruoc);
                }

                if (XuLySoLieuNamSau.fCapThanhKhoan != 0)
                {
                    XuLySoLieuNamSau.fCapThanhKhoanAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fCapThanhKhoan);
                }

                if (XuLySoLieuNamSau.fChiTieu != 0)
                {
                    XuLySoLieuNamSau.fChiTieuAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fChiTieu);
                }

                if (XuLySoLieuNamSau.fChiTieuNamTruoc != 0)
                {
                    XuLySoLieuNamSau.fChiTieuNamTruocAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fChiTieuNamTruoc);
                }

                if (XuLySoLieuNamSau.fDuocBu != 0)
                {
                    XuLySoLieuNamSau.fDuocBuAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fDuocBu);
                }

                if (XuLySoLieuNamSau.fGiaTriChuyenNamSauChuaCap != 0)
                {
                    XuLySoLieuNamSau.fGiaTriChuyenNamSauChuaCapAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fGiaTriChuyenNamSauChuaCap);
                }

                if (XuLySoLieuNamSau.fGiaTriChuyenNamSauDaCap != 0)
                {
                    XuLySoLieuNamSau.fGiaTriChuyenNamSauDaCapAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fGiaTriChuyenNamSauDaCap);
                }

                if (XuLySoLieuNamSau.fGiaTriNamTruocChuyenNamSauChuaCap != 0)
                {
                    XuLySoLieuNamSau.fGiaTriNamTruocChuyenNamSauChuaCapAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fGiaTriNamTruocChuyenNamSauChuaCap);
                }

                if (XuLySoLieuNamSau.fGiaTriNamTruocChuyenNamSauDaCap != 0)
                {
                    XuLySoLieuNamSau.fGiaTriNamTruocChuyenNamSauDaCapAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fGiaTriNamTruocChuyenNamSauDaCap);
                }

                if (XuLySoLieuNamSau.fNgoaiChiTieu != 0)
                {
                    XuLySoLieuNamSau.fNgoaiChiTieuAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fNgoaiChiTieu);
                }

                if (XuLySoLieuNamSau.fNgoaiChiTieuNamTruoc != 0)
                {
                    XuLySoLieuNamSau.fNgoaiChiTieuNamTruocAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fNgoaiChiTieuNamTruoc);
                }

                if (XuLySoLieuNamSau.fQuyetToan != 0)
                {
                    XuLySoLieuNamSau.fQuyetToanAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fQuyetToan);
                }

                if (XuLySoLieuNamSau.fQuyetToanNamTruoc != 0)
                {
                    XuLySoLieuNamSau.fQuyetToanNamTruocAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fQuyetToanNamTruoc);
                }

                if (XuLySoLieuNamSau.fSoBu != 0)
                {
                    XuLySoLieuNamSau.fSoBuAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fSoBu);
                }

                if (XuLySoLieuNamSau.fThieu != 0)
                {
                    XuLySoLieuNamSau.fThieuAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fThieu);
                }

                if (XuLySoLieuNamSau.fThieuNamTruoc != 0)
                {
                    XuLySoLieuNamSau.fThieuNamTruocAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fThieuNamTruoc);
                }

                if (XuLySoLieuNamSau.fThua != 0)
                {
                    XuLySoLieuNamSau.fThuaAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fThua);
                }

                if (XuLySoLieuNamSau.fThuaNamTruoc != 0)
                {
                    XuLySoLieuNamSau.fThuaNamTruocAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fThuaNamTruoc);
                }

                // anhht comment 16/09/2021: k con truong fThuLai
                //if (XuLySoLieuNamSau.fThuLai != 0)
                //{
                //    XuLySoLieuNamSau.fThuLaiAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fThuLai);
                //}

                if (XuLySoLieuNamSau.fThuThanhKhoan != 0)
                {
                    XuLySoLieuNamSau.fThuThanhKhoanAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fThuThanhKhoan);
                }

                if (XuLySoLieuNamSau.fThuThanhKhoanNamTruoc != 0)
                {
                    XuLySoLieuNamSau.fThuThanhKhoanNamTruocAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fThuThanhKhoanNamTruoc);
                }

                if (XuLySoLieuNamSau.fTongGiaTriChuyenNamSau != 0)
                {
                    XuLySoLieuNamSau.fTongGiaTriChuyenNamSauAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fTongGiaTriChuyenNamSau);
                }

                if (XuLySoLieuNamSau.fTrongChiTieu != 0)
                {
                    XuLySoLieuNamSau.fTrongChiTieuAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fTrongChiTieu);
                }

                if (XuLySoLieuNamSau.fTrongChiTieuNamTruoc != 0)
                {
                    XuLySoLieuNamSau.fTrongChiTieuNamTruocAsString = string.Format("{0:0,0}", XuLySoLieuNamSau.fTrongChiTieuNamTruoc);
                }
            }

            // Xử lý số liệu năm trước

            if(XuLySoLieuNamTruoc != null)
            {

                if (XuLySoLieuNamTruoc.fGiaTriNamTruocChuyenNamSauDaCap.HasValue)
                {
                    XuLySoLieuNamTruoc.fTongGiaTriChuyenNamSau = XuLySoLieuNamTruoc.fGiaTriNamTruocChuyenNamSauDaCap.Value;
                }
                else
                {
                    XuLySoLieuNamTruoc.fTongGiaTriChuyenNamSau = 0;
                }
                if (XuLySoLieuNamTruoc.fGiaTriChuyenNamSauChuaCap.HasValue)
                {
                    XuLySoLieuNamTruoc.fTongGiaTriChuyenNamSau += XuLySoLieuNamTruoc.fGiaTriChuyenNamSauChuaCap.Value;
                }

                if (XuLySoLieuNamTruoc.fCapPhat != 0)
                {
                    XuLySoLieuNamTruoc.fCapPhatAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fCapPhat);
                }

                if (XuLySoLieuNamTruoc.fCapPhatNamTruoc != 0)
                {
                    XuLySoLieuNamTruoc.fCapPhatNamTruocAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fCapPhatNamTruoc);
                }

                if (XuLySoLieuNamTruoc.fCapThanhKhoan != 0)
                {
                    XuLySoLieuNamTruoc.fCapThanhKhoanAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fCapThanhKhoan);
                }

                if (XuLySoLieuNamTruoc.fChiTieu != 0)
                {
                    XuLySoLieuNamTruoc.fChiTieuAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fChiTieu);
                }

                if (XuLySoLieuNamTruoc.fChiTieuNamTruoc != 0)
                {
                    XuLySoLieuNamTruoc.fChiTieuNamTruocAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fChiTieuNamTruoc);
                }

                if (XuLySoLieuNamTruoc.fDuocBu != 0)
                {
                    XuLySoLieuNamTruoc.fDuocBuAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fDuocBu);
                }

                if (XuLySoLieuNamTruoc.fGiaTriChuyenNamSauChuaCap != 0)
                {
                    XuLySoLieuNamTruoc.fGiaTriChuyenNamSauChuaCapAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fGiaTriChuyenNamSauChuaCap);
                }

                if (XuLySoLieuNamTruoc.fGiaTriChuyenNamSauDaCap != 0)
                {
                    XuLySoLieuNamTruoc.fGiaTriChuyenNamSauDaCapAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fGiaTriChuyenNamSauDaCap);
                }

                if (XuLySoLieuNamTruoc.fGiaTriNamTruocChuyenNamSauChuaCap != 0)
                {
                    XuLySoLieuNamTruoc.fGiaTriNamTruocChuyenNamSauChuaCapAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fGiaTriNamTruocChuyenNamSauChuaCap);
                }

                if (XuLySoLieuNamTruoc.fGiaTriNamTruocChuyenNamSauDaCap != 0)
                {
                    XuLySoLieuNamTruoc.fGiaTriNamTruocChuyenNamSauDaCapAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fGiaTriNamTruocChuyenNamSauDaCap);
                }

                if (XuLySoLieuNamTruoc.fNgoaiChiTieu != 0)
                {
                    XuLySoLieuNamTruoc.fNgoaiChiTieuAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fNgoaiChiTieu);
                }

                if (XuLySoLieuNamTruoc.fNgoaiChiTieuNamTruoc != 0)
                {
                    XuLySoLieuNamTruoc.fNgoaiChiTieuNamTruocAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fNgoaiChiTieuNamTruoc);
                }

                if (XuLySoLieuNamTruoc.fQuyetToan != 0)
                {
                    XuLySoLieuNamTruoc.fQuyetToanAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fQuyetToan);
                }

                if (XuLySoLieuNamTruoc.fQuyetToanNamTruoc != 0)
                {
                    XuLySoLieuNamTruoc.fQuyetToanNamTruocAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fQuyetToanNamTruoc);
                }

                if (XuLySoLieuNamTruoc.fSoBu != 0)
                {
                    XuLySoLieuNamTruoc.fSoBuAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fSoBu);
                }

                if (XuLySoLieuNamTruoc.fThieu != 0)
                {
                    XuLySoLieuNamTruoc.fThieuAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fThieu);
                }

                if (XuLySoLieuNamTruoc.fThieuNamTruoc != 0)
                {
                    XuLySoLieuNamTruoc.fThieuNamTruocAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fThieuNamTruoc);
                }

                if (XuLySoLieuNamTruoc.fThua != 0)
                {
                    XuLySoLieuNamTruoc.fThuaAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fThua);
                }

                if (XuLySoLieuNamTruoc.fThuaNamTruoc != 0)
                {
                    XuLySoLieuNamTruoc.fThuaNamTruocAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fThuaNamTruoc);
                }

                // anhht comment 16/09/2021: k con truong fThuLai
                //if (XuLySoLieuNamTruoc.fThuLai != 0)
                //{
                //    XuLySoLieuNamTruoc.fThuLaiAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fThuLai);
                //}

                if (XuLySoLieuNamTruoc.fThuThanhKhoan != 0)
                {
                    XuLySoLieuNamTruoc.fThuThanhKhoanAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fThuThanhKhoan);
                }

                if (XuLySoLieuNamTruoc.fThuThanhKhoanNamTruoc != 0)
                {
                    XuLySoLieuNamTruoc.fThuThanhKhoanNamTruocAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fThuThanhKhoanNamTruoc);
                }

                if (XuLySoLieuNamTruoc.fTongGiaTriChuyenNamSau != 0)
                {
                    XuLySoLieuNamTruoc.fTongGiaTriChuyenNamSauAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fTongGiaTriChuyenNamSau);
                }

                if (XuLySoLieuNamTruoc.fTrongChiTieu != 0)
                {
                    XuLySoLieuNamTruoc.fTrongChiTieuAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fTrongChiTieu);
                }

                if (XuLySoLieuNamTruoc.fTrongChiTieuNamTruoc != 0)
                {
                    XuLySoLieuNamTruoc.fTrongChiTieuNamTruocAsString = string.Format("{0:0,0}", XuLySoLieuNamTruoc.fTrongChiTieuNamTruoc);
                }
            }
        }
    }

    public class TongHopSoLieuChiTietViewModel : VDT_QT_TongHopSoLieu_ChiTiet
    {
        public TongHopSoLieuChiTietViewModel()
        {
            NoiDungDanhMuc = "";
            M_TM_TM_N = "";
            ChiTieuNamTruocAsString = "0";
            ChiTieuNamNayAsString = "0";
            TongChiTieuAsString = "0";
            CapPhatNamTruocAsString = "0";
            CapPhatNamNayAsString = "0";
            TongCapPhatAsString = "0";
            fDonViiDeNghiQuyetToanNamNay = 0;
            fDonViiDeNghiQuyetToanNamNayAsString = "0";
            fDonViiDeNghiQuyetToanNamTruoc = 0;
            fDonViiDeNghiQuyetToanNamTruocAsString = "0";
            fTroLyDeNghiQuyetToanNamNay = 0;
            fTroLyDeNghiQuyetToanNamNayAsString = "0";
            fTroLyDeNghiQuyetToanNamTruoc = 0;
            fTroLyDeNghiQuyetToanNamTruocAsString = "0";
        }
        public string M_TM_TM_N { get; set; }
        public string NoiDungDanhMuc { get; set; }
        public double ChiTieuNamTruoc { get; set; }
        public string ChiTieuNamTruocAsString { get; set; }
        public double ChiTieuNamNay { get; set; }
        public string ChiTieuNamNayAsString { get; set; }
        public double TongChiTieu { get; set; }
        public string TongChiTieuAsString { get; set; }
        public double CapPhatNamTruoc { get; set; }
        public string CapPhatNamTruocAsString { get; set; }
        public double CapPhatNamNay { get; set; }
        public string CapPhatNamNayAsString { get; set; }
        public double TongCapPhat { get; set; }
        public string TongCapPhatAsString { get; set; }
        public string fDonViiDeNghiQuyetToanNamNayAsString { get; set; }
        public string fDonViiDeNghiQuyetToanNamTruocAsString { get; set; }
        public string fTroLyDeNghiQuyetToanNamNayAsString { get; set; }
        public string fTroLyDeNghiQuyetToanNamTruocAsString { get; set; }
    }

    public class VDT_QT_XuLySoLieuViewModel : VDT_QT_XuLySoLieu
    {
        public VDT_QT_XuLySoLieuViewModel()
        {
            M_TM_TM_N = "";
            NoiDungDanhMuc = "";
            fThua = 0;
            fThuaAsString = "0";
            fThuaNamTruoc = 0;
            fThuaNamTruocAsString = "0";
            fThieu = 0;
            fThieuAsString = "0";
            fThieuNamTruoc = 0;
            fThieuNamTruocAsString = "0";
            fTrongChiTieu = 0;
            fTrongChiTieuAsString = "0";
            fTrongChiTieuNamTruoc = 0;
            fTrongChiTieuNamTruocAsString = "0";
            fNgoaiChiTieu = 0;
            fNgoaiChiTieuAsString = "0";
            fNgoaiChiTieuNamTruoc = 0;
            fNgoaiChiTieuNamTruocAsString = "0";
            // anhht comment 16/09/2021: k con truong fThuLai
            //fThuLai = 0;
            fThuLaiAsString = "0";
            fGiaTriChuyenNamSauChuaCap = 0;
            fGiaTriChuyenNamSauChuaCapAsString = "0";
            fGiaTriChuyenNamSauDaCap = 0;
            fGiaTriChuyenNamSauDaCapAsString = "0";
            fGiaTriNamTruocChuyenNamSauChuaCap = 0;
            fGiaTriNamTruocChuyenNamSauChuaCapAsString = "0";
            fGiaTriNamTruocChuyenNamSauDaCap = 0;
            fGiaTriNamTruocChuyenNamSauDaCapAsString = "0";
            fThuThanhKhoan = 0;
            fThuThanhKhoanAsString = "0";
            fThuThanhKhoanNamTruoc = 0;
            fThuThanhKhoanNamTruocAsString = "0";
            fDuocBu = 0;
            fDuocBuAsString = "0";
            fSoBu = 0;
            fSoBuAsString = "0";
            fChiTieu = 0;
            fChiTieuAsString = "0";
            fChiTieuNamTruoc = 0;
            fChiTieuNamTruocAsString = "0";
            fCapPhat = 0;
            fCapPhatAsString = "0";
            fCapPhatNamTruoc = 0;
            fCapPhatNamTruocAsString = "0";
            fQuyetToan = 0;
            fQuyetToanAsString = "0";
            fQuyetToanNamTruoc = 0;
            fQuyetToanNamTruocAsString = "0";
            fCapThanhKhoan = 0;
            fCapThanhKhoanAsString = "0";
            fTongGiaTriChuyenNamSau = 0;
            fTongGiaTriChuyenNamSauAsString = "0";
        }
        public double fTongGiaTriChuyenNamSau { get; set; }
        public string fTongGiaTriChuyenNamSauAsString { get; set; }
        public string M_TM_TM_N { get; set; }
        public string NoiDungDanhMuc { get; set; }
        public string fThuaAsString { get; set; }
        public string fThuaNamTruocAsString { get; set; }
        public string fThieuAsString { get; set; }
        public string fThieuNamTruocAsString { get; set; }
        public string fTrongChiTieuAsString { get; set; }
        public string fTrongChiTieuNamTruocAsString { get; set; }
        public string fNgoaiChiTieuAsString { get; set; }
        public string fNgoaiChiTieuNamTruocAsString { get; set; }
        public string fThuLaiAsString { get; set; }
        public string fGiaTriChuyenNamSauChuaCapAsString { get; set; }
        public string fGiaTriChuyenNamSauDaCapAsString { get; set; }
        public string fGiaTriNamTruocChuyenNamSauChuaCapAsString { get; set; }
        public string fGiaTriNamTruocChuyenNamSauDaCapAsString { get; set; }
        public string fThuThanhKhoanAsString { get; set; }
        public string fThuThanhKhoanNamTruocAsString { get; set; }
        public string fDuocBuAsString { get; set; }
        public string fSoBuAsString { get; set; }
        public string fChiTieuAsString { get; set; }
        public string fChiTieuNamTruocAsString { get; set; }
        public string fCapPhatAsString { get; set; }
        public string fCapPhatNamTruocAsString { get; set; }
        public string fQuyetToanAsString { get; set; }
        public string fQuyetToanNamTruocAsString { get; set; }
        public string fCapThanhKhoanAsString { get; set; }
    }

    public class VDT_QT_TongHopSoLieuChiTietCreateModel : VDT_QT_TongHopSoLieu_ChiTiet
    {
        public string Type { get; set; }
    }

    public class VDT_QT_XuLySoLieuCreateModel : VDT_QT_XuLySoLieu
    {
        public string Type { get; set; }
    }

    public static class Type
    {
        public static string CREATE = "CREATE";
        public static string UPDATE = "UPDATE";
        public static string DELETE = "DELETE";
    }
}
