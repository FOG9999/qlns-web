<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site_KeToan.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="System.ComponentModel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<script src="<%= Url.Content("~/Scripts/KeToanCongSan/jsBang_KeToanCongSan_ChungTu.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>" type="text/javascript"></script>
<script src="<%= Url.Content("~/Scripts/jsBang_Editable.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>" type="text/javascript"></script>
<script src="<%= Url.Content("~/Scripts/jsBang_KeyTable.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>" type="text/javascript"></script>   
<script src="<%= Url.Content("~/Scripts/jsBang_Data.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>" type="text/javascript"></script>   
<script src="<%= Url.Content("~/Scripts/jsBang.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>" type="text/javascript"></script> 

<%
    String MaND = User.Identity.Name;
    String IPSua = Request.UserHostAddress;
    String ControlID = "Parent";
    String ParentID = ControlID + "_Search";
    DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(MaND);
    int iNam = Convert.ToInt32(dtCauHinh.Rows[0]["iNamLamViec"]);
    int iThang = Convert.ToInt32(dtCauHinh.Rows[0]["iThangLamViec"]);
    dtCauHinh.Dispose();
    String iID_MaChungTu = Request.QueryString["iID_MaChungTu"];
    String sSoChungTu = Request.QueryString["sSoChungTu"];
    String TimKiem = Request.QueryString["TimKiem"];
    int csH = 0, csC = 0;
    
    //Cập nhập các thông tin tìm kiếm
    String DSTruong = "sSoChungTu,iID_MaTrangThaiDuyet";
    String[] arrDSTruong = DSTruong.Split(',');
    Dictionary<String, String> arrGiaTriTimKiem = new Dictionary<string, string>();
    for (int i = 0; i < arrDSTruong.Length; i++)
    {
        arrGiaTriTimKiem.Add(arrDSTruong[i], Request.QueryString[arrDSTruong[i]]);
    }

    KTCS_ChungTu_BangDuLieu bang = new KTCS_ChungTu_BangDuLieu(iThang, iNam, arrGiaTriTimKiem, MaND, IPSua);

    if (String.IsNullOrEmpty(iID_MaChungTu) == false)
    {
        for (int i = 0; i < bang.dtChiTiet.Rows.Count; i++)
        {
            if (iID_MaChungTu == Convert.ToString(bang.dtChiTiet.Rows[i]["iID_MaChungTu"]))
            {
                csH = i;
                break;
            }
        }
    }
    else
    {
        csH = bang.dtChiTiet.Rows.Count-1;
    }
    csH = bang.dtChiTiet.Rows.Count - 1;
    String BangID = "BangDuLieu";
    int Bang_Height = 200;
    int Bang_FixedRow_Height = 25;
%>
<div class="box_tong">
    <div id="nhapform">
        <div id="form2">
   

<%Html.RenderPartial("~/Views/Shared/BangDuLieu/BangDuLieu.ascx", new { BangID = BangID, bang = bang, Bang_Height = Bang_Height, Bang_FixedRow_Height = Bang_FixedRow_Height }); %>    


<div style="display:none;">
<input type="hidden" id="idXauDoRongCot" value="<%=HttpUtility.HtmlEncode(bang.strDSDoRongCot)%>" />
<input type="hidden" id="idXauKieuDuLieu" value="<%=HttpUtility.HtmlEncode(bang.strType)%>" />
<input type="hidden" id="idXauChiSoCha" value="<%=HttpUtility.HtmlEncode(bang.strCSCha)%>" />
<input type="hidden" id="idBangChiDoc" value="<%=HttpUtility.HtmlEncode(bang.strChiDoc)%>" />
<input type="hidden" id="idXauEdit" value="<%=HttpUtility.HtmlEncode(bang.strEdit)%>" />
<input type="hidden" id="idViewport_N" value="<%=HttpUtility.HtmlEncode(bang.Viewport_N)%>" />
<input type="hidden" id="idNC_Fixed" value="<%=HttpUtility.HtmlEncode(bang.nC_Fixed)%>" />
<input type="hidden" id="idNC_Slide" value="<%=HttpUtility.HtmlEncode(bang.nC_Slide)%>" />
<input type="hidden" id="idCoCotTongSo" value="0" />
<%  
    if (bang.ChiDoc == false)
    {
    %>
    
    <form action="<%=Url.Action("DetailSubmit", "KTCS_ChungTu", new{iNam = iNam, iThang = iThang})%>" method="post">
    <%
    } %>
        <input type="hidden" id="idAction" name="idAction" value="0" />
        <input type="hidden" id="id_iID_MaChungTu_Action" name="id_iID_MaChungTu_Action" value="" />
        <input type="hidden" id="id_iID_MaChungTu_Focus" name="id_iID_MaChungTu_Focus" value="" />
        <input type="hidden" id="idXauDuLieuThayDoi" name="idXauDuLieuThayDoi" value="<%=HttpUtility.HtmlEncode(bang.strThayDoi)%>" />
        <input type="hidden" id="idXauLaHangCha" name="idXauLaHangCha" value="<%=HttpUtility.HtmlEncode(bang.strLaHangCha)%>" />
        <input type="hidden" id="idXauMaCacHang" name="idXauMaCacHang" value="<%=HttpUtility.HtmlEncode(bang.strDSMaHang)%>" />
        <input type="hidden" id="idXauMaCacCot" name="idXauMaCacCot" value="<%=HttpUtility.HtmlEncode(bang.strDSMaCot)%>" />
        <input type="hidden" id="idXauGiaTriChiTiet" name="idXauGiaTriChiTiet" value="<%=HttpUtility.HtmlEncode(bang.strDuLieu)%>" />
        <input type="submit" id="btnXacNhanGhi" value="XN" />
        <input type="hidden" id="idXauCacHangDaXoa" name="idXauCacHangDaXoa" value="" />
<%
    if (bang.ChiDoc == false)
    {
    %>
    </form>    
    <%
    }
        %>

</div>




<%
    if (bang.ChiDoc == false)
    {
%>
    <table width="100%" cellpadding="0" cellspacing="0" border="0" align="right" class="table_form2">
        <tr><td>&nbsp;</td></tr>
        <tr>
            <td align=center>
                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td align="right">
                            <input type="button" class="button" onclick="javascript:return BangDuLieu_onKeypress_F10();" value="<%=NgonNgu.LayXau("Thực hiện")%>"/>
                        </td>
                        <%
                        if (LuongCongViecModel.KiemTra_TroLyPhongBan(MaND) == false)
                        {
                            %>
                            <td>&nbsp;</td>
                            <td align="right" width="100px">
                            <%
                            if (LuongCongViecModel.KiemTra_ThuTruong(MaND))
                            {
                                %>
                                <input type="button" class="button" onclick="javascript:return BangDuLieu_onKeypress_F10(-1,-1,2);" value="<%=NgonNgu.LayXau("Phê duyệt")%>"/>
                                <%
                            } 
                            else
                            {
                                %>
                                <input type="button" class="button" onclick="javascript:return BangDuLieu_onKeypress_F10(-1,-1,2);" value="<%=NgonNgu.LayXau("Trình duyệt")%>"/>
                            <%} %>
                            </td>
                            <%
                            if (LuongCongViecModel.KiemTra_TroLyTongHop(MaND) == false)
                            {
                                %>
                                <td>&nbsp;</td>
                                <td align="right" width="100px">
                                    <input type="button" class="button" onclick="javascript:return BangDuLieu_onKeypress_F10(-1,-1,1);" value="<%=NgonNgu.LayXau("Từ chối")%>"/>
                                </td>
                                <%
                            } 
                        }
                             %>
                    </tr>
                </table>
            </td>
        </tr>
        <tr><td >&nbsp;</td></tr>
    </table>
<%
}
%>


<script type="text/javascript">
    $(document).ready(function () {
        Bang_arrDSTruongTien = '<%=MucLucNganSachModels.strDSTruongTien%>'.split(',');
        Bang_Url_getGiaTri = '<%=Url.Action("get_GiaTri", "Public")%>';
        Bang_Url_getDanhSach = '<%=Url.Action("get_DanhSach", "Public")%>';
        BangDuLieu_iID_MaChungTu = '<%=iID_MaChungTu%>';
        BangDuLieu_DuocSuaChiTiet = <%=bang.DuocSuaChiTiet?"true":"false"%>;
        if (Bang_nH > 0) {
           Bang_keys.fnSetFocus(<%=csH %> ,<%=csC %>); 
        }        
        Bang_keys.focus();
        parent.GoiHam_ChungTu_BangDuLieu_fnSetFocus();
        <%
        if (TimKiem == "1")
        {
            %>           
            parent.ChungTu_ThayDoiMaChungTu();
            <%
        }
        %>
      
    });
</script>

        </div>
    </div>
</div>
</asp:Content>
