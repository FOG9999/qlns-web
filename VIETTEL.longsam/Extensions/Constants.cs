using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Extensions
{
    public static class Constants
    {
        public const int ITEMS_PER_PAGE = 20;
        public enum LOAI_MAN_HINH
        {
            BINH_THUONG = 1,
            DIEU_CHINH = 2
        };

        public const byte TONG_DU_TOAN = 1;
        public const byte DU_TOAN = 0;

        public enum LOAI_CAP
        {
            THANH_TOAN = 1,
            UNG_NGOAI = 0
        }


        public enum VDT_KIEULOAI_THONGTRI
        {
            THONG_TRI_THANH_TOAN = 0,
            THONG_TRI_QUYET_TOAN = 1
        }

        public const string CAP_TT_KPQP = "TT_CTT_KPQP";
        public const string CAP_TAM_UNG_KPQP = "TT_TamUng_KPQP";
        public const string THU_UNG_KPQP = "TT_ThuUng_KPQP";
        public const string CAP_UNG_KHAC = "TT_Cap_UngKhac";
        public const string THU_UNG_KP_KHAC = "TT_ThuUng_KPK";

        public enum NS_NGUON_NGAN_SACH
        {
            NS_QUOC_PHONG = 1,
            NS_NHA_NUOC = 2,
            NS_DIA_PHUONG = 3,
            NS_NUOC_NGOAI_VIEN_TRO = 4,
            NS_DAC_BIET = 5,
            NS_TRAI_PHIEU_CHINH_PHU = 6,
            NS_CHUYEN_DOI_DAT = 7,
            NS_TU_CO = 8,
            NS_KHAC = 9,
            NGUON_DU_PHONG = 10
        }

        public enum KIEU_LOAI_THONG_TRI
        {
            THANH_TOAN = 0,
            QUYET_TOAN = 1
        }

        public enum CHECK_TRUNG
        {
            SO_KE_HOACH = 0,
            GIAI_DOAN = 1
        }

        public struct LOAI_CHUNG_TU
        {
            public static string CHU_DAU_TU = "000";
            public static string NAM_TRUOC_CHUYEN_SANG = "NAM_TRUOC_CHUYEN";

            public static string KE_HOACH_VON_NAM = "KHVN";
            public static string KHVN_KHOBAC = "101";
            public static string KHVN_LENHCHI = "102";
            public static string KHVN_THUHOI_KHOBAC_NAMTRUOC = "121b";
            public static string KHVN_THUHOI_LENHCHI_NAMTRUOC = "122b";

            public static string KE_HOACH_VON_UNG = "KHVU";
            public static string KHVU_KHOBAC = "121a";
            public static string KHVU_LENHCHI = "122a";

            public static string CAP_THANH_TOAN = "THANH_TOAN";
            public static string TT_THANHTOAN_KHOBAC = "201";
            public static string TT_THANHTOAN_LENHCHI = "202";
            public static string TT_UNG_KHOBAC = "211a";
            public static string TT_UNG_LENHCHI = "212a";

            public static string TT_THUHOI_KHOBAC_NAMTRUOC = "211b1";
            public static string TT_THUHOI_LENHCHI_NAMTRUOC = "212b1";
            public static string TT_THUHOI_KHOBAC_NAMNAY = "211b2";
            public static string TT_THUHOI_LENHCHI_NAMNAY = "212b2";

            public static string QUYET_TOAN = "QUYET_TOAN";
            public static string QT_KHOBAC_CHUYENNAMTRUOC = "111";
            public static string QT_LENHCHI_CHUYENNAMTRUOC = "112";
            public static string QT_UNG_KHOBAC_CHUYENNAMTRUOC = "131";
            public static string QT_UNG_LENHCHI_CHUYENNAMTRUOC = "132";
            public static string QT_KHOBAC_DIEUCHINHGIAM = "211c";
            public static string QT_LENHCHI_DIEUCHINHGIAM = "212c";

            public static string QT_LUYKE_KHVN_KHOBAC = "301";
            public static string QT_LUYKE_KHVN_LENHCHI = "302";
            public static string QT_LUYKE_TT_KHVN_KHOBAC = "403";
            public static string QT_LUYKE_TT_KHVN_LENHCHI = "404";
            public static string QT_LUYKE_TU_CHUATH_KHOBAC = "413a";
            public static string QT_LUYKE_TU_CHUATH_LENHCHI = "414a";

            public static string QT_LUYKE_KHVU_KHOBAC = "321a";
            public static string QT_LUYKE_KHVU_LENHCHI = "322a";
            public static string QT_LUYKE_TT_KHVU_KHOBAC = "403";
            public static string QT_LUYKE_TT_KHVU_LENHCHI = "404";
            public static string QT_LUYKE_UNGTRUOC_CHUATHUHOI_KHOBAC = "321b";
            public static string QT_LUYKE_UNGTRUOC_CHUATHUHOI_LENHCHI = "322b";

            public static string QT_LUYKE_TTKLHT_CHUA_PHANBO_KHOBAC = "403a";
            public static string QT_LUYKE_TTKLHT_CHUA_PHANBO_LENHCHI = "404a";
        }

        public enum TypeExecute
        {
            Insert = 1,
            Update = 2,
            Delete = 3,
            Adjust = 4
        }

        public static class CoQuanThanhToan
        {
            public enum Type
            {
                KHO_BAC = 1,
                CQTC = 2
            }
            public struct TypeName
            {
                public static string KHO_BAC = "Kho bạc";
                public static string CQTC = "Cơ quan tài chính bộ quốc phòng";
            }
        }

        public static class LoaiThanhToan
        {
            public enum Type
            {
                THANH_TOAN = 1,
                TAM_UNG = 2,
                THU_HOI = 3
            }

            public struct TypeName
            {
                public const string THANH_TOAN = "Thanh toán";
                public const string TAM_UNG = "Tạm ứng";
                public const string THU_HOI = "Thu hồi ứng";
            }

            public static string Get(int type)
            {
                switch (type)
                {
                    case (int)Type.THANH_TOAN:
                        return TypeName.THANH_TOAN;
                    case (int)Type.TAM_UNG:
                        return TypeName.TAM_UNG;
                    case (int)Type.THU_HOI:
                        return TypeName.THU_HOI;
                }
                return string.Empty;
            }
        }

        public static class LoaiNamKeHoach
        {
            public enum Type
            {
                NAM_TRUOC = 1,
                NAM_NAY = 2,
                NAM_SAU = 3
            }

            public struct TypeName
            {
                public const string NAM_TRUOC = "Năm trước";
                public const string NAM_NAY = "Năm nay";
                public const string NAM_SAU = "Năm sau";
            }
        }

        public enum NguonVonStatus
        {
            ChuaSuDung = 0,
            DangSuDung = 1,
            DaSuDung = 2
        }

        public enum LoaiXuLy
        {
            TaoMoi = 1,
            CapNhat = 2,
            Xoa = 3,
            DieuChinh = 4
        }

        public static class BaoHiemXHType
        {
            public enum ITypes
            {
                String = 0,
                Int = 1,
                Date = 2
            }
        }

        public static class PaymentTypeEnum
        {
            public enum Type
            {
                THANH_TOAN = 1,
                TAM_UNG = 2,
                THU_HOI = 3,
                THU_HOI_NAM_TRUOC = 4,
                THU_HOI_NAM_NAY = 5,
                THU_HOI_UNG_TRUOC_NAM_TRUOC = 6,
                THU_HOI_UNG_TRUOC_NAM_NAY = 7
            }

            public struct TypeName
            {
                public const string THANH_TOAN = "Thanh toán";
                public const string TAM_UNG = "Tạm ứng";
                public const string THU_HOI = "Thu hồi ứng";
                public const string THU_HOI_UNG_TRUOC_NAM_TRUOC = "Thu hồi ứng trước năm trước";
                public const string THU_HOI_UNG_TRUOC_NAM_NAY = "Thu hồi ứng trước năm nay";
                public const string THU_HOI_NAM_TRUOC = "Thu hồi ứng chế độ năm trước";
                public const string THU_HOI_NAM_NAY = "Thu hồi ứng chế độ năm nay";
                public const string THANH_TOAN_KLHT = "Thanh toán KLHT";
            }

            public static string Get(int type)
            {
                switch (type)
                {
                    case (int)Type.THANH_TOAN:
                        return TypeName.THANH_TOAN;
                    case (int)Type.TAM_UNG:
                        return TypeName.TAM_UNG;
                    case (int)Type.THU_HOI:
                        return TypeName.THU_HOI;
                    case (int)Type.THU_HOI_NAM_TRUOC:
                        return TypeName.THU_HOI_NAM_TRUOC;
                    case (int)Type.THU_HOI_NAM_NAY:
                        return TypeName.THU_HOI_NAM_NAY;
                    case (int)Type.THU_HOI_UNG_TRUOC_NAM_TRUOC:
                        return TypeName.THU_HOI_UNG_TRUOC_NAM_TRUOC;
                    case (int)Type.THU_HOI_UNG_TRUOC_NAM_NAY:
                        return TypeName.THU_HOI_UNG_TRUOC_NAM_NAY;
                }
                return string.Empty;
            }
        }

        public static class LOAI_KHV
        {
            public struct TypeName
            {
                public const string KE_HOACH_VON_NAM = "Kế hoạch vốn năm";
                public const string KE_HOACH_VON_UNG = "Kế hoạch vốn ứng";
                public const string KE_HOACH_VON_NAM_CHUYEN_SANG = "Kế hoạch năm trước chuyển sang";
                public const string KE_HOACH_VON_UNG_CHUYEN_SANG = "Kế hoạch ứng trước năm trước chuyển sang";
            }

            public enum Type
            {
                KE_HOACH_VON_NAM = 1,
                KE_HOACH_VON_UNG = 2,
                KE_HOACH_VON_NAM_CHUYEN_SANG = 3,
                KE_HOACH_VON_UNG_CHUYEN_SANG = 4
            }
        }

        public static class NamKeHoachEnum
        {
            public enum Type
            {
                NAM_TRUOC = 1,
                NAM_NAY = 2,
                NAM_SAU = 3
            }

            public struct TypeName
            {
                public const string NAM_TRUOC = "Năm trước";
                public const string NAM_NAY = "Năm nay";
                public const string NAM_SAU = "Năm sau";
            }
        }

        public static class LOAI_QUYET_TOAN_NIEN_DO
        {
            public enum Type
            {
                THANH_TOAN = 1,
                TAM_UNG = 2
            }

            public struct TypeName
            {
                public const string THANH_TOAN = "Báo cáo quyết toán nguồn vốn đầu tư năm";
                public const string TAM_UNG = "Báo cáo kế hoạch và thanh toán vốn đầu tư - ứng trước kế hoạch vốn năm sau";
            }
        }

        public static class LoaiTraoDoi
        {
            public enum Type
            {
                DU_TOAN = 1,
                QUYET_TOAN = 2,
                THONG_TRI = 3
            }

            public struct TypeName
            {
                public const string DU_TOAN = "Dự toán";
                public const string QUYET_TOAN = "Quyết toán";
                public const string THONG_TRI = "Thông tri";
            }
        }

        public static class TrangThai
        {
            public enum Type
            {
                TAO_MOI = 1,
                DA_CHUYEN = 2,
                DANG_XU_LY = 3,
                HOAN_THANH = 4
            }

            public struct TypeName
            {
                public const string TAO_MOI = "Tạo mới";
                public const string DA_CHUYEN = "Đã chuyển";
                public const string DANG_XU_LY = "Đang xử lý";
                public const string HOAN_THANH = "Hoàn thành";
            }
        }

        public static class LoaiThongTri
        {
            public enum Type
            {
                TAM_UNG = 1,
                THANH_TOAN = 2,
                CAP_HOP_THUC = 3,
                CAP_KINH_DOANH = 4,
            }

            public struct TypeName
            {
                public const string TAM_UNG = "Tạm ứng";
                public const string THANH_TOAN = "Thanh toán";
                public const string CAP_HOP_THUC = "Cấp hợp thức";
                public const string CAP_KINH_DOANH = "Cấp kinh doanh";
            }
        }

        public static class LoaiDuToan
        {
            public enum Type
            {
                DAU_NAM = 1,
                BO_SUNG = 2,
                NAM_TRUOC_CHUYEN_SANG = 3
            }

            public struct TypeName
            {
                public const string DAU_NAM = "Đầu năm";
                public const string BO_SUNG = "Bổ sung";
                public const string NAM_TRUOC_CHUYEN_SANG = "Năm Trước chuyển sang";
            }
        }

        public static class LoaiQuy
        {
            public enum Type
            {
                QUY_1 = 1,
                QUY_2 = 2,
                QUY_3 = 3,
                QUY_4 = 4
            }

            public struct TypeName
            {
                public const string QUY_1 = "Quý I";
                public const string QUY_2 = "Quý II";
                public const string QUY_3 = "Quý III";
                public const string QUY_4 = "Quý IV";
            }
        }

        public static class LoaiThongTriThanhToan
        {
            public enum Type
            {
                CAP_THANH_TOAN = 1,
                CAP_TAM_UNG = 2,
                CAP_KINH_PHI = 3,
                CAP_HOP_THUC = 4
            }
            public struct TypeName
            {
                public const string CAP_THANH_TOAN = "Cấp thanh toán";
                public const string CAP_TAM_UNG = "Cấp tạm ứng";
                public const string CAP_KINH_PHI = "Cấp kinh phí";
                public const string CAP_HOP_THUC = "Cấp hợp thức";
            }
        }
    }
}
