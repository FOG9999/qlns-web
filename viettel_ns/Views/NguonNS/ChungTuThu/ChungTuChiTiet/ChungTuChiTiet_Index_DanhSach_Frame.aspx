<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site_KeToan.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="System.ComponentModel" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <%  
        string LoadLai = Convert.ToString(ViewData["LoadLai"]);
        if (LoadLai == "1")
        {
    %>
    <script type="text/javascript">
        $(document).ready(function () {
            parent.reloadPage();

        });
    </script>
    <%
        }
        else
        {
            string MaND = User.Identity.Name;
            string IPSua = Request.UserHostAddress;
            string Id_CTThu = Request.QueryString["Id_CTThu"];
            if (string.IsNullOrEmpty(Id_CTThu)) Id_CTThu = Convert.ToString(ViewData["Id_CTThu"]);
            
            //Cập nhập các thông tin tìm kiếm
            string[] arrDSTruong = "CTMT,L,K,Ma_Nguon".Split(',');
            Dictionary<string, string> arrGiaTriTimKiem = new Dictionary<string, string>();
            for (int i = 0; i < arrDSTruong.Length; i++)
            {
                arrGiaTriTimKiem.Add(arrDSTruong[i], Convert.ToString(ViewData[arrDSTruong[i]]));
            }

            Nguon_CTThuChiTiet_BangDuLieu bang = new Nguon_CTThuChiTiet_BangDuLieu(Id_CTThu, arrGiaTriTimKiem, MaND, IPSua);
            string BangID = "BangDuLieu";
            int Bang_Height = 470;
            int Bang_FixedRow_Height = 50;
            string BackURL = Url.Action("Index", "ChungTuThu_ChungTu");           
    %>
            <%Html.RenderPartial("~/Views/Shared/BangDuLieu/BangDuLieu.ascx", new { BangID = BangID, bang = bang, Bang_Height = Bang_Height, Bang_FixedRow_Height = Bang_FixedRow_Height }); %>
            <div style="display: none;">
            <input type="hidden" id="idXauHienThiCot" value="<%=HttpUtility.HtmlEncode(bang.strDSHienThiCot)%>" />
            <input type="hidden" id="idXauDoRongCot" value="<%=HttpUtility.HtmlEncode(bang.strDSDoRongCot)%>" />
            <input type="hidden" id="idXauKieuDuLieu" name="idXauKieuDuLieu" value="<%=HttpUtility.HtmlEncode(bang.strType)%>" />
            <input type="hidden" id="idXauChiSoCha" value="<%=HttpUtility.HtmlEncode(bang.strCSCha)%>" />
            <input type="hidden" id="idBangChiDoc" value="<%=HttpUtility.HtmlEncode(bang.strChiDoc)%>" />
            <input type="hidden" id="idXauEdit" value="<%=HttpUtility.HtmlEncode(bang.strEdit)%>" />
            <input type="hidden" id="idViewport_N" value="<%=HttpUtility.HtmlEncode(bang.Viewport_N)%>" />        
        <%  
        if (bang.ChiDoc == false)
        {
        %>
            <form id="formDuyet" action="<%=Url.Action("DetailSubmit", "ChungTuThu_ChungTuChiTiet", new{ Id_CTThu=Id_CTThu })%>" method="post">            
            <input type="hidden" id="L" name="L" value="<%=HttpUtility.HtmlEncode(Convert.ToString(arrGiaTriTimKiem["L"]))%>" />
            <input type="hidden" id="K" name="K" value="<%=HttpUtility.HtmlEncode(Convert.ToString(arrGiaTriTimKiem["K"]))%>" />
            <input type="hidden" id="Ma_Nguon" name="Ma_Nguon" value="<%=HttpUtility.HtmlEncode(Convert.ToString(arrGiaTriTimKiem["Ma_Nguon"]))%>" />
            <input type="hidden" id="idXauDuLieuThayDoi" name="idXauDuLieuThayDoi" value="<%=HttpUtility.HtmlEncode(bang.strThayDoi)%>" />
            <input type="hidden" id="idXauLaHangCha" name="idXauLaHangCha" value="<%=HttpUtility.HtmlEncode(bang.strLaHangCha)%>" />
            <input type="hidden" id="idXauMaCacHang" name="idXauMaCacHang" value="<%=HttpUtility.HtmlEncode(bang.strDSMaHang)%>" />
            <input type="hidden" id="idXauMaCacCot" name="idXauMaCacCot" value="<%=HttpUtility.HtmlEncode(bang.strDSMaCot)%>" />
            <input type="hidden" id="idXauGiaTriChiTiet" name="idXauGiaTriChiTiet" value="<%=HttpUtility.HtmlEncode(bang.strDuLieu)%>" />
            <input type="hidden" id="idNC_Fixed" name="idNC_Fixed" value="<%=HttpUtility.HtmlEncode(bang.nC_Fixed)%>" />
            <input type="hidden" id="idNC_Slide" name="idNC_Slide" value="<%=HttpUtility.HtmlEncode(bang.nC_Slide)%>" />
            <input type="submit" id="btnXacNhanGhi" name="btnXacNhanGhi" value="XN" />
            <input type="hidden" id="idXauCacHangDaXoa" name="idXauCacHangDaXoa" value="" />            
            </form>
        <%
        }
        %>
    </div>

    <table width="100%" cellpadding="0" cellspacing="0" border="0" class="table_form2">
        <tr>
            <td>&nbsp;
            </td>
        </tr>
        <tr>
            <%
                if (bang.ChiDoc == false)
                {
            %>
                <td align="right" style="padding-right: 10px; width: 40%;">
                    <input type="button" id="btnLuu" class="button" onclick="javascript:return Bang_HamTruocKhiKetThuc();"
                        value="<%=NgonNgu.LayXau("Thực hiện")%>" />
                </td>
                <td align="right" style="padding-right: 10px; width: 6%">

                    <input class="button" type="button" value="<%=NgonNgu.LayXau("Quay lại")%>" onclick="Huy()" />
                </td>
            <%} %>
                <td align="right" style="padding-right: 10px; width: 40%;"></td>
        </tr>
        <tr>
            <td>&nbsp;
            </td>
        </tr>
    </table>
        
    <script src="<%= Url.Content("~/Scripts/jsBang_Editable.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>"
        type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/NguonNS/jsNguon_CTThu_CT.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>"
        type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jsBang_KeyTable.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>"
        type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jsBang_Data.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>"
        type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jsBang.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>"
        type="text/javascript"></script>
    <script type="text/javascript">
        function Huy() {
            window.parent.location.href = '<%=BackURL %>';
         }
         $(document).ready(function () {
            Bang_arrDSTruongTien = '<%="SoTien"%>'.split(',');
            Bang_Url_getGiaTri = '<%=Url.Action("get_GiaTri", "Public")%>';
            BangDuLieu_Id_CTThu = '<%=Id_CTThu%>';
            BangDuLieu_DuocSuaChiTiet = <%=bang.DuocSuaChiTiet?"true":"false"%>;
            <%=bang.DuocSuaChiTiet?"":"Bang_keys.fnSetFocus(null, null);"%>
        });
    </script>
    <div id="dvText" class="popup_block">
        <img src="../../../Content/ajax-loader.gif" /><br />
        <p>
            Hệ thống đang thực hiện yêu cầu...
        </p>
    </div>
    <%} %>
</asp:Content>
