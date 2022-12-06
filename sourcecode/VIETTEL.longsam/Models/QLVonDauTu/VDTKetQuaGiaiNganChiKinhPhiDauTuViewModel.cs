using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTKetQuaGiaiNganChiKinhPhiDauTuViewModel
    {
        public string iID_DonViQuanLyID { get; set; }

        //public string sTenDVQL { get; set; }
        public string sTen { get; set; }   
        public double? fDuToanDonViDeNghiChuyenSangNamSau { get; set; }
        public double? fDuToanDonViNamNay { get; set; }
        public double? sSumDuToan
        {
            get
            {
                return fDuToanDonViDeNghiChuyenSangNamSau + fDuToanDonViNamNay;
            }
        }
        public double? sCucTaiChinh {
            get
            {
                return 0;
            }
        }

        //chi tai BQP
        public  double? fDuToanDuocThongBaoBQP{ get; set; }
                //Ket qua giai ngan
        public double? fThanhToanBQP { get; set; }
        public double? fTamUngBQP { get; set; }
        public double? sSumBQP 
        { 
            get 
            {
                return fThanhToanBQP + fTamUngBQP;
            } 
        }
        public double? sRatioBQP {
            get
            {
                if (sSumBQP == 0|| fDuToanDuocThongBaoBQP ==0)
                {
                    return 0;
                }
                return sSumBQP/ fDuToanDuocThongBaoBQP;
            }
        }
        /// <summary>
        //Du toan tai KBNN
        /// </summary>
        public double? fDuDoanDuocThongBaoKBNN { get; set; }
            //Ket qua giai ngan
        public double? fThanhToanKBNN { get; set; }
        public double? fTamUngKBNN { get; set; }
        public double? sSumKBNN
        {
            get
            {
                return fThanhToanKBNN + fTamUngKBNN;
            }
        }
        public double? sRatioKBNN
        {
            get
            {
                if (sSumKBNN == 0 || fDuDoanDuocThongBaoKBNN == 0)
                {
                    return 0;
                }
                return sSumKBNN / fDuDoanDuocThongBaoKBNN;
            }
        }

    }

    public class VDTKetQuaGiaiNganChiKinhPhiDauTuViewModel_v2
    {
        public Guid? ID { get; set; }
        public string sNoiDung { get; set; }
        public double? fDuToanDonViDeNghiChuyenSangNamSau { get; set; }
        public double? fDuToanDonViNamNay { get; set; }
        public double? fTongCongDuToanNam { get; set; }
        public double? fDuToanDuocThongBaoBQP { get; set; }
        public double? fCucTaiChinhThanhToanTrucTiepBQP { get; set; }
        public double? fThanhToanBQP { get; set; }
        public double? fTamUngBQP { get; set; }
        public double? fTongCongBQP { get; set; }
        public double? fDatTiLeBQP { get; set; }
        public double? fDuDoanDuocThongBaoKBNN { get; set; }
        public double? fThanhToanKBNN { get; set; }
        public double? fTamUngKBNN { get; set; }
        public double? fTongCongKBNN { get; set; }
        public double? fDatTiLeKBNN { get; set; }
        public double? depth { get; set; }
        public Guid?   IDParent { get; set; }
        public string Position { get; set; }

        public int IsBold { get; set; }
    }   
}
