using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIETTEL.Common
{
    public static class Constants
    {
        public const byte ACTIVED = 1;

        public const byte DELETED = 0;

        public const string TAT_CA = "--Tất cả--";

        public const int ITEMS_PER_PAGE = 20;

        public const byte TONG_DU_TOAN = 1;

        public const byte DU_TOAN = 0;

        public const int LA_CHON = -1;

        public const string CHON = "--Chọn--";

        public const string LA_TONG_DU_TOAN = "Là tổng dự toán";

        public const string LA_DU_TOAN = "Là dự toán";

        public const string CHON_DON_VI_NS = "--Chọn đơn vị NS--";

        public const string CHON_DON_VI_BHXH = "--Chọn đơn vị BHXH--";

        public enum LOAI_HOP_DONG
        {
            HOP_DONG_GIAO_VIEC = 0,
            HOP_DONG_KINH_TE = 1
        };

        public enum LOAI_QUYET_DINH
        {
            DU_TOAN = 0,
            TONG_DU_TOAN = 1
        };

        public struct PTDauThauTypeName
        {
            public const string PT_1 = "1 Giai đoạn 1 túi hồ sơ";
            public const string PT_2 = "1 Giai đoạn 2 túi hồ sơ";
            public const string PT_3 = "2 Giai đoạn 1 túi hồ sơ";
            public const string PT_4 = "2 Giai đoạn 2 túi hồ sơ";
        }

        public struct HTChonNhaThauTypeName
        {
            public const string HT_1 = "Đấu thầu rộng rãi";
            public const string HT_2 = "Đấu thầu hạn chế";
            public const string HT_3 = "Chỉ định thầu";
            public const string HT_4 = "Chào hàng cạnh tranh";
            public const string HT_5 = "Mua sắm trực tiếp";
            public const string HT_6 = "Tự thực hiện";
            public const string HT_7 = "Lựa chọn NT, NĐT trong trường hợp đặc biệt";
            public const string HT_8 = "Tham gia thực hiện của cộng đồng";
            public const string HT_9 = "Tham gia thực hiện của cộng đồng";
            public const string HT_10 = "Chỉ định thầu rút gọn";
        }

        public struct HTHopDongTypeName
        {
            public const string HD_1 = "Hợp đồng trọn gói";
            public const string HD_2 = "Hợp đồng theo đơn giá cố định";
            public const string HD_3 = "Hợp đồng theo đơn giá điều chỉnh";
        }

        public enum LOAI_DON_VI
        {
            DOANH_NGHIEP = 0,
            DON_VI_DU_TOAN = 1
        };

        public static class CoQuanThanhToan
        {
            public enum Type
            {
                KHO_BAC = 1,
                CQTC = 2,
                CTC = 2
            }
            public struct TypeName
            {
                public static string KHO_BAC = "Kho bạc";
                public static string CQTC = "Cơ quan tài chính bộ quốc phòng";
                public static string CTC = "Cục tài chính";
            }
        }

        public static class LoaiCoQuanTaiChinh
        {
            public enum Type
            {
                CQTC = 0,
                CTC = 1
            }
            public struct TypeName
            {
                public static string KHO_BAC = "Kho bạc";
                public static string CQTC = "Cơ quan tài chính bộ quốc phòng";
                public static string CTC = "Cục tài chính";
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
        }

        public enum LoaiXuLy
        {
            TaoMoi = 1,
            CapNhat = 2,
            Xoa = 3,
            DieuChinh = 4
        }

        public static class LoaiNganSach
        {
            public enum Type
            {
                CHI_NGAN_SACH_NHA_NUOC = 0,
                CHI_THUONG_XUYEN_QP = 1
            }

            public struct TypeName
            {
                public const string CHI_NGAN_SACH_NHA_NUOC = "Chi ngân sách nhà nước";
                public const string CHI_THUONG_XUYEN_QP = "Chi thường xuyên quốc phòng";
            }

            public static string Get(int type)
            {
                switch (type)
                {
                    case (int)Type.CHI_NGAN_SACH_NHA_NUOC:
                        return TypeName.CHI_NGAN_SACH_NHA_NUOC;
                    case (int)Type.CHI_THUONG_XUYEN_QP:
                        return TypeName.CHI_THUONG_XUYEN_QP;
                }
                return string.Empty;
            }
        }

        public struct DuToanType
        {
            public enum Type
            {
                TONG_DU_TOAN = 1,
                DU_TOAN = 0
            }

            public struct TypeName
            {
                public const string TONG_DU_TOAN = "Tổng dự toán";
                public const string DU_TOAN = "Dự toán";
            }
        }

        public struct CanCuType
        {
            public enum Type
            {
                TKTC_TONG_DU_TOAN = 1,
                QUYET_DINH_DAU_TU = 2,
                CHU_TRUONG_DAU_TU = 3
            }

            public struct TypeName
            {
                public const string TKTC_TONG_DU_TOAN = "TKTC và tổng dự toán";
                public const string QUYET_DINH_DAU_TU = "Phê duyệt dự án";
                public const string CHU_TRUONG_DAU_TU = "Chủ trương đầu tư";
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
        public static class NamNganSach
        {
            public enum Type
            {
                NAM_TRUOC_CHUYEN_SANG = 1,
                NAM_NAY = 2
            }
            public struct TypeName
            {
                public const string NAM_TRUOC_CHUYEN_SANG = "Năm trước chuyển sang";
                public const string NAM_NAY = "Năm nay";
            }
        }
        public static class LoaiThongTriEnum
        {
            public enum Type
            {
                CAP_THANH_TOAN = 1,
                CAP_TAM_UNG = 2,
                CAP_KINH_PHI = 3,
                CAP_HOP_THUC = 4
            }

            public struct Name
            {
                public static string CAP_THANH_TOAN = "Cấp thanh toán";
                public static string CAP_TAM_UNG = "Cấp tạm ứng";
                public static string CAP_KINH_PHI = "Cấp kinh phí";
                public static string CAP_HOP_THUC = "Cấp hợp thức";
            }
        }

        public struct KieuThongTri
        {
            public static string TT_Thu_UngKhac = "TT_Thu_UngKhac";
            public static string TT_KPQP = "TT_CTT_KPQP";
            public static string TT_TamUng_KPQP = "TT_TamUng_KPQP";
            public static string QT_KPQP_CS = "QTKPQP_CS";
            public static string C_KPQP_CS = "CKPQP_CS";
            public static string TN_NSQP = "TNNSQP";
            public static string TT_ThuUng_KPQP = "TT_ThuUng_KPQP";
            public static string TT_TamUng_KPNN = "TT_TamUng_KPNN";
            public static string TT_Cap_KPNN = "TT_Cap_KPNN";
            public static string TT_TamUng_KPK = "TT_TamUng_KPK";
            public static string TT_ThuUng_KPNN = "TT_ThuUng_KPNN";
            public static string QT_KPQP = "QTKPQP";
            public static string TT_Cap_UngKhac = "TT_Cap_UngKhac";
            public static string TT_Cap_KPK = "TT_Cap_KPK";
            public static string TKP_QP_CS = "TKPQP_CS";
            public static string QT_KPK = "QTKPK";
            public static string CTK = "CTK";
            public static string TTK = "TTK";
            public static string QT_NSNN = "QTNSNN";
            public static string TT_ThuUng_KPK = "TT_ThuUng_KPK";
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
                public const string NAM_TRUOC_CHUYEN_SANG = "Năm trước chuyển sang";
            }
        }

        public static class DonViTinh
        {
            public enum Type
            {
                DONG = 1,
                NGHIN_DONG = 1000,
                NGHIN_DONG_LOWER = 1001,
                TRIEU_DONG = 1000000,
                TY_DONG = 1000000000,
            }

            public struct TypeName
            {
                public const string DONG = "Đồng";
                public const string NGHIN_DONG = "Nghìn đồng";
                public const string TRIEU_DONG = "Triệu đồng";
                public const string TY_DONG = "Tỷ đồng";
            }
        }
        public static class Attachment
        {
            public enum ModuleType
            {
                QUAN_LY_KE_HOACH_TRUNG_HAN_DE_XUAT=100,
                QUAN_LY_KE_HOACH_TRUNG_HAN_DUOC_DUYET=101,
                CHU_TRUONG_DAU_TU=102,
                PHE_DUYET_DU_AN=103,
                THIET_KE_THI_CONG_VA_TONG_DU_AN=104,
                KE_HOACH_LUA_CHON_NHA_THAU=105,
                THONG_TIN_GOI_THAU=106,
                THONG_TIN_HOP_DONG=107,
                THONG_TIN_CHUNG_DU_AN=108,
                KE_HOACH_VON_NAM_DE_XUAT=109,
                KE_HOACH_VON_NAM_DUOC_DUYET = 110,
                DU_TOAN_NAM_DUOC_GIAO = 111,
                KE_HOACH_CHI_QUY = 112,
                KE_HOACH_VON_UNG_DE_XUAT = 113,
                KE_HOACH_VON_UNG_DUOC_DUYET = 114,
                THUC_HIEN_THANH_TOAN = 115,
                THONG_TRI = 116,
                THONG_HOP_THONG_TIN_DU_AN = 117,
                BAO_CAO_TINH_HINH_THUC_HIEN_DU_AN = 118,
                BAO_CAO_THEO_DOI_CHI_TIEU_CAP_PHAT_DU_AN = 119,
                BAO_CAO_CAP_PHAT_NGAN_SACH_NAM = 120,
                BAO_CAO_KET_QUA_GIAI_NGAN_KINH_PHI_DAU_TU_NAM = 121,
                BAO_CAO_QUYET_TOAN_CAC_NGUON_VON_DAU_TU = 122,
                THONG_TRI_QUYET_TOAN = 123,
                DE_NGHI_QUYET_TOAN_DU_AN_HOAN_THANH = 124,
                PHE_DUYET_QUYET_TOAN_DU_AN_HOAN_THANH = 125,
                KHOI_TAO_THONG_TIN_DU_AN = 126,
            }
            public struct ModuleName
            {
                public const string QUAN_LY_KE_HOACH_TRUNG_HAN_DE_XUAT = "Quản lý Kế hoạch trung hạn đề xuất";
                public const string QUAN_LY_KE_HOACH_TRUNG_HAN_DUOC_DUYET = "Quản lý Kế hoạch trung hạn đề xuất";
                public const string CHU_TRUONG_DAU_TU = "Chủ trương đầu tư";
                public const string PHE_DUYET_DU_AN = "Phê duyệt dự án";
                public const string THIET_KE_THI_CONG_VA_TONG_DU_AN = "Thiết kế thi công và tổng dự toán";
                public const string KE_HOACH_LUA_CHON_NHA_THAU = "Kế hoạch lựa chọn nhà thầu";
                public const string THONG_TIN_GOI_THAU = "Thông tin gói thầu";
                public const string THONG_TIN_HOP_DONG = "Thông tin hợp đồng";
                public const string THONG_TIN_CHUNG_DU_AN = "Thông tin chung dự án";
                public const string KE_HOACH_VON_NAM_DE_XUAT = "Kế hoạch vốn năm đề xuất";
                public const string KE_HOACH_VON_NAM_DUOC_DUYET = "Kế hoạch vốn năm được duyệt";
                public const string DU_TOAN_NAM_DUOC_GIAO = "Dự toán năm được giao";
                public const string KE_HOACH_CHI_QUY = "Kế hoạch chi quý";
                public const string KE_HOACH_VON_UNG_DE_XUAT = "Kế hoạch vốn ứng đề xuất";
                public const string KE_HOACH_VON_UNG_DUOC_DUYET = "Kế hoạch vốn ứng được duyệt";
                public const string THUC_HIEN_THANH_TOAN = "Thực hiện thanh toán";
                public const string THONG_TRI = "Thông tri";
                public const string THONG_HOP_THONG_TIN_DU_AN = "Tổng hợp thông tin dự án";
                public const string BAO_CAO_TINH_HINH_THUC_HIEN_DU_AN = "Báo cáo tình hình thực hiện dự án";
                public const string BAO_CAO_THEO_DOI_CHI_TIEU_CAP_PHAT_DU_AN = "Báo cáo theo dõi chỉ tiêu cấp phát dự án";
                public const string BAO_CAO_CAP_PHAT_NGAN_SACH_NAM = "Báo cáo cấp phát ngân sách năm";
                public const string BAO_CAO_KET_QUA_GIAI_NGAN_KINH_PHI_DAU_TU_NAM = "Báo cáo kết quả giải ngân kinh phí đầu tư năm";
                public const string BAO_CAO_QUYET_TOAN_CAC_NGUON_VON_DAU_TU = "Báo cáo quyết toán các nguồn vốn đầu tư";
                public const string THONG_TRI_QUYET_TOAN = "Thông tri quyết toán";
                public const string DE_NGHI_QUYET_TOAN_DU_AN_HOAN_THANH = "Đề nghị quyết toán dự án hoàn thành";
                public const string PHE_DUYET_QUYET_TOAN_DU_AN_HOAN_THANH = "Phê duyệt quyết toán dự án hoàn thành";
                public const string KHOI_TAO_THONG_TIN_DU_AN = "KHỞI TẠO THÔNG TIN DỰ ÁN";
            }
        }
    }
}