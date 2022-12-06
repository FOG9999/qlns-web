using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace VIETTEL.Models
{
    public static class ModelHelpers
    {
        //public static void SetProps(this NsEntity entity, HttpRequestBase request)
        //{
        //    entity.bPublic = true;
        //    if (!entity.dNgayTao.HasValue)
        //        entity.dNgayTao = DateTime.Now;

        //    entity.dNgaySua = DateTime.Now;
        //    entity.sID_MaNguoiDungSua = request.LogonUserIdentity.Name;
        //    entity.iSoLanSua = entity.iSoLanSua.HasValue ? entity.iSoLanSua.Value + 1 : 1;
        //}

        //public static T SetProps<T>(this T entity, HttpRequestBase request) where T : NEntity
        //{
        //    entity.bPublic = true;
        //    if (!entity.dNgayTao.HasValue)
        //        entity.dNgayTao = DateTime.Now;

        //    entity.dNgaySua = DateTime.Now;
        //    entity.sID_MaNguoiDungSua = request.RequestContext.HttpContext.User.Identity.Name;
        //    entity.sID_MaNguoiDungTao = request.RequestContext.HttpContext.User.Identity.Name;
        //    entity.iSoLanSua = entity.iSoLanSua.HasValue ? entity.iSoLanSua.Value + 1 : 1;
        //    entity.sIPSua = request.UserHostAddress;

        //    return entity;
        //}

        public static IEnumerable<TrangThaiDuyetViewModel> ToTrangThaiDuyetList(this DataTable dt, TrangThaiDuyetViewModel defaultValue = null)
        {
            var list = dt.AsEnumerable()
                .Select(x => new TrangThaiDuyetViewModel()
                {
                    iID_MaTrangThaiDuyet = x.Field<int>("iID_MaTrangThaiDuyet"),
                    sTen = x.Field<string>("sTen"),
                    sMauSac = x.Field<string>("sMauSac"),
                });
            return list;
        }
    }



}
