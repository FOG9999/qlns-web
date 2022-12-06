using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Viettel.Services;

namespace Viettel.Domain.DomainModel
{
    public partial class PhienLamViec
    {
        protected PhienLamViec()
        {
            PhongBan = new NS_PhongBan();
        }

        public virtual string iNamLamViec { get; set; }
        public virtual int NamLamViec
        {
            get
            {
                return string.IsNullOrWhiteSpace(iNamLamViec) ?
                    DateTime.Now.Year :
                    iNamLamViec.ToValue<int>();
            }
        }
        public virtual int iThangLamViec { get; set; }
        public virtual int iID_MaNamNganSach { get; set; }
        public virtual int iID_MaNguonNganSach { get; set; }

        public NS_PhongBan PhongBan { get; set; }

        public Dictionary<string, string> DonViList { get; set; }
        public Dictionary<string, string> LNSList { get; set; }
        public int userRole { get; set; }
    }

    public partial class PhienLamViecViewModel : PhienLamViec
    {
        protected PhienLamViecViewModel() : base()
        {
        }

        public virtual string iID_MaPhongBan { get { return PhongBan == null ? "-1" : PhongBan.sKyHieu; } }
        public virtual string sTenPhongBan { get { return PhongBan == null ? "" : PhongBan.sMoTa; } }
        public virtual string sTenPhongBanFull { get { return PhongBan == null ? "" : PhongBan.sTen + " - " + PhongBan.sMoTa; } }

        public string iID_MaDonVi
        {
            get
            {
                return DonViList == null ? string.Empty : DonViList.Select(x => x.Key).ToList().Join();
            }
        }

        public string sLNS
        {
            get
            {
                return LNSList == null ? string.Empty : LNSList.Select(x => x.Key).ToList().Join();
            }
        }

        public static PhienLamViecViewModel Current
        {
            get
            {
                var vm = HttpContext.Current.Session["NS_PhienLamViec"];
                if (vm == null)
                {
                    vm = PhienLamViecViewModel.Create(HttpContext.Current.User.Identity.Name);
                }

                return (PhienLamViecViewModel)vm;
            }
        }
    }

    public partial class PhienLamViecViewModel
    {
        private readonly PhienLamViecViewModel _default;

        public static PhienLamViecViewModel Create(string username)
        {
            var ngansachService = NganSachService.Default;
            var cauhinh = ngansachService.GetCauHinh(username);

            var phongban = ngansachService.GetPhongBan(username);

            var donvis = ngansachService.GetDonviListByUser(username, cauhinh.iNamLamViec)
                .ToDictionary(x => x.iID_MaDonVi, x => x.iID_MaDonVi + " - " + x.sTen);

            var lns = ngansachService.GetLNS(phongban.sKyHieu, cauhinh.iNamLamViec.ToString())
                .AsEnumerable()
                .ToDictionary(x => x.Field<string>("sLNS"), x => x.Field<string>("sLNS") + " - " + x.Field<string>("sMoTa"));

            var userrol = ngansachService.GetUserRoleType(username);

            var vm = new PhienLamViecViewModel()
            {
                iNamLamViec = cauhinh.iNamLamViec.ToString(),
                iID_MaNamNganSach = cauhinh.iID_MaNamNganSach,
                iID_MaNguonNganSach = cauhinh.iID_MaNguonNganSach,
                iThangLamViec = cauhinh.iThangLamViec,

                PhongBan = phongban,
                DonViList = donvis,
                LNSList = lns,
                userRole = userrol,
            };

            return vm;
        }
    }
}
