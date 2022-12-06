<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site_KeToan.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="System.ComponentModel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script src="<%= Url.Content("~/Scripts/jsBang_CapPhat.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jsBang_Editable.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jsBang_KeyTable.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jsBang_Data.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jsBang.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>" type="text/javascript"></script>
    <%
        String ControlID = "Parent";
        String ParentID = ControlID + "_Search";
        String iID_MaCapPhat = Request.QueryString["iID_MaCapPhat"];
        if (String.IsNullOrEmpty(iID_MaCapPhat)) iID_MaCapPhat = Convert.ToString(ViewData["iID_MaCapPhat"]);
        String MaND = User.Identity.Name;
        String IPSua = Request.UserHostAddress;
        String ChiNganSach = Request.QueryString["ChiNganSach"];
        String DSTruong = MucLucNganSachModels.strDSTruong;
        String[] arrDSTruong = (DSTruong + ",iID_MaDonVi").Split(',');
        Dictionary<String, String> arrGiaTriTimKiem = new Dictionary<string, string>();
        DataTable dtCapPhat = CapPhat_ChungTuModels.LayToanBoThongTinChungTu(iID_MaCapPhat);
        String Loai = Convert.ToString(dtCapPhat.Rows[0]["iLoai"]);
        for (int i = 0; i < arrDSTruong.Length; i++)
        {
            arrGiaTriTimKiem.Add(arrDSTruong[i], Convert.ToString(ViewData[arrDSTruong[i]]));
        }
        String MaLoai = Request.QueryString["MaLoai"];
        if (String.IsNullOrEmpty(MaLoai)) MaLoai = Convert.ToString(ViewData["MaLoai"]);

        CapPhat_BangDuLieu bang = new CapPhat_BangDuLieu(iID_MaCapPhat, arrGiaTriTimKiem, MaLoai, MaND, IPSua);

        String BangID = "BangDuLieu";
        int Bang_Height = 470;
        int Bang_FixedRow_Height = 50;
        String strDSDonVi = bang.strDSDonVi;

        int iID_MaTrangThaiDuyet_TuChoi = CapPhat_ChungTuChiTietModels.LayMaTrangThaiDuyetTuChoi(MaND, iID_MaCapPhat);
        int iID_MaTrangThaiDuyet_TrinhDuyet = CapPhat_ChungTuChiTietModels.LayMaTrangThaiDuyetTrinhDuyet(MaND, iID_MaCapPhat);

        string BackURL = "";
        if (String.IsNullOrEmpty(bang.chungTu["iID_MaDonVi"].ToString()))
        {
            BackURL = Url.Action("Index", "CapPhat_ChungTu", new { Loai = Loai });
        }
        else
        {
            BackURL = Url.Action("Index", "CapPhat_ChungTu_DonVi", new { Loai = Loai });
        }
        string InsertUrl = Url.Action("Index", "CapPhat_CapTheoCTGom", new { id_chungtucp = iID_MaCapPhat, Loai = Loai });

    %>

    <%Html.RenderPartial("~/Views/Shared/BangDuLieu/BangDuLieu.ascx", new { BangID = BangID, bang = bang, Bang_Height = Bang_Height, Bang_FixedRow_Height = Bang_FixedRow_Height }); %>

    <div style="display: none;">
        <input type="hidden" id="idXauDoRongCot" value="<%=HttpUtility.HtmlEncode(bang.strDSDoRongCot)%>" />
        <input type="hidden" id="idXauKieuDuLieu" value="<%=HttpUtility.HtmlEncode(bang.strType)%>" />
        <input type="hidden" id="idXauChiSoCha" value="<%=HttpUtility.HtmlEncode(bang.strCSCha)%>" />
        <input type="hidden" id="idBangChiDoc" value="<%=HttpUtility.HtmlEncode(bang.strChiDoc)%>" />
        <input type="hidden" id="idXauEdit" value="<%=HttpUtility.HtmlEncode(bang.strEdit)%>" />
        <input type="hidden" id="idViewport_N" value="<%=HttpUtility.HtmlEncode(bang.Viewport_N)%>" />        
        <input type="hidden" id="idDSChiSoNhom" value="<%=HttpUtility.HtmlEncode(bang.strDSChiSoNhom)%>" />
        <input type="hidden" id="idDSDonVi" value="<%=strDSDonVi%>" />
        <%  
            if (bang.ChiDoc == false)
            {
        %>
        <form id="formDuyet" action="<%=Url.Action("LuuChungTuChiTiet", "CapPhat_ChungTuChiTiet", new {ChiNganSach=ChiNganSach,iID_MaCapPhat=iID_MaCapPhat})%>" method="post">
            <%
                } %>
            <input type="hidden" id="sLNS" name="sLNS" value="<%=HttpUtility.HtmlEncode(Convert.ToString(arrGiaTriTimKiem["sLNS"]))%>" />
            <input type="hidden" id="sL" name="sL" value="<%=HttpUtility.HtmlEncode(Convert.ToString(arrGiaTriTimKiem["sL"]))%>" />
            <input type="hidden" id="sK" name="sK" value="<%=HttpUtility.HtmlEncode(Convert.ToString(arrGiaTriTimKiem["sK"]))%>" />
            <input type="hidden" id="sM" name="sM" value="<%=HttpUtility.HtmlEncode(Convert.ToString(arrGiaTriTimKiem["sM"]))%>" />
            <input type="hidden" id="sTM" name="sTM" value="<%=HttpUtility.HtmlEncode(Convert.ToString(arrGiaTriTimKiem["sTM"]))%>" />
            <input type="hidden" id="sTTM" name="sTTM" value="<%=HttpUtility.HtmlEncode(Convert.ToString(arrGiaTriTimKiem["sTTM"]))%>" />
            <input type="hidden" id="sNG" name="sNG" value="<%=HttpUtility.HtmlEncode(Convert.ToString(arrGiaTriTimKiem["sNG"]))%>" />
            <input type="hidden" id="sTNG" name="sTNG" value="<%=HttpUtility.HtmlEncode(Convert.ToString(arrGiaTriTimKiem["sTNG"]))%>" />
            <input type="hidden" id="iID_MaDonVi" name="iID_MaDonVi" value="<%=HttpUtility.HtmlEncode(Convert.ToString(arrGiaTriTimKiem["iID_MaDonVi"]))%>" />
            <input type="hidden" id="idAction" name="idAction" value="0" />
            <input type="hidden" id="sLyDo" name="sLyDo" />
            <input type="hidden" id="idXauDuLieuThayDoi" name="idXauDuLieuThayDoi" value="<%=HttpUtility.HtmlEncode(bang.strThayDoi)%>" />
            <input type="hidden" id="idXauLaHangCha" name="idXauLaHangCha" value="<%=HttpUtility.HtmlEncode(bang.strLaHangCha)%>" />
            <input type="hidden" id="idXauMaCacHang" name="idXauMaCacHang" value="<%=HttpUtility.HtmlEncode(bang.strDSMaHang)%>" />
            <input type="hidden" id="idXauMaCacCot" name="idXauMaCacCot" value="<%=HttpUtility.HtmlEncode(bang.strDSMaCot)%>" />
            <input type="hidden" id="idNC_Fixed" name="idNC_Fixed" value="<%=HttpUtility.HtmlEncode(bang.nC_Fixed)%>" />
            <input type="hidden" id="idNC_Slide" name="idNC_Slide" value="<%=HttpUtility.HtmlEncode(bang.nC_Slide)%>" />
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
    <table width="100%" cellpadding="0" cellspacing="0" border="0" class="table_form2">
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td align="left" style="padding-left: 10px; width: 40%"></td>
            <td align="right" style="padding-right: 10px; width: 6%">
                <div onclick="OnInit_CT_NEW(500, 'Tùy chỉnh');">
                    <%= Ajax.ActionLink("Tùy chỉnh", "Index", "NhapNhanh", new { id = "CAPPHAT_TUYCHINH", OnLoad = "OnLoad_CT", OnSuccess = "CallSuccess_CT", iID_MaChungTu = iID_MaCapPhat }, new AjaxOptions { }, new { @class = "button" })%>
                </div>
            </td>
            <td align="right" style="padding-right: 10px; width: 6%">
                <input type="button" id="btnLuu" class="button" onclick="javascript: return Bang_HamTruocKhiKetThuc();" value="<%=NgonNgu.LayXau("Lưu")%>" />
            </td>
            <td align="center" style="width: 1%">
                <input class="button" type="button" value="<%=NgonNgu.LayXau("Quay lại")%>" onclick="Huy()" />
            </td>

            <%
                if (iID_MaTrangThaiDuyet_TuChoi > 0)
                {
            %>
            <%--<div style="float: left; padding-right: 10px;">
                        <button class='button' style="float:left;"  onclick="javascript:return Bang_HamTruocKhiKetThuc(1);">Từ chối</button>
                    </div>--%>
            <td align="center" style="padding-left: 10px; width: 1%">
                <div onclick="OnInit_CT_NEW(500, 'Từ chối chứng từ');">
                    <%= Ajax.ActionLink("Từ chối", "Index", "NhapNhanh", new { id = "CapPhat_TuChoiChiTiet", OnLoad = "OnLoad_CT", OnSuccess = "CallSuccess_CT", iID_MaChungTu = iID_MaCapPhat }, new AjaxOptions { }, new { @class = "button" })%>
                </div>
            </td>
            <%        
                }
            %>
            <%
                if (iID_MaTrangThaiDuyet_TrinhDuyet > 0)
                {
                    String trinhDuyet = "Trình duyệt";
                    if (LuongCongViecModel.KiemTra_NguoiDungDuocDuyet(MaND, PhanHeModels.iID_MaPhanHeCapPhat))
                    {
                        trinhDuyet = "Phê duyệt";
                    }
            %>
            <%--<div style="float: left;">
                    <button class='button' style="float: left;" onclick="javascript:return Bang_HamTruocKhiKetThuc(2);">
                        <%=TrinhDuyet %></button>
                </div>--%>
            <td align="left" style="padding-left: 10px; width: 6%">
                <div onclick="OnInit_CT_NEW(500, 'Duyệt chứng từ');">
                    <%= Ajax.ActionLink(trinhDuyet, "Index", "NhapNhanh", new { id = "CapPhat_TrinhDuyetChiTiet", OnLoad = "OnLoad_CT", OnSuccess = "CallSuccess_CT", iID_MaChungTu = iID_MaCapPhat }, new AjaxOptions { }, new { @class = "button" })%>
                </div>
            </td>
            <%
                }
            %>
            <td style="padding-right: 10px;">
                <div onclick="OnInit_CT_NEW(700, 'Thông tri');">
                    <% if (Loai == "4")
                        { %>
                    <%= Html.ActionLink("Thông tri", "Index", new { controller = "rptCapPhat_ThongTri_SecDMNB", iID_MaCapPhat = iID_MaCapPhat }, new { @class = "button", target = "_blank" })%>
                    <%}
                        else
                        {  %>
                    <%= Html.ActionLink("Thông tri", "Index", new { controller = "rptCapPhat_ThongTri", iID_MaCapPhat = iID_MaCapPhat }, new { @class = "button", target = "_blank" })%>
                    <%} %>
                </div>
            </td> 
            <td style="padding-right: 10px;">
                <div onclick="OnInit_CT_NEW(700, 'Cấp QDTNH');">                   
                    <%= Html.ActionLink("Cấp QDTNH", "Index", new { controller = "rptCapPhat_ThongTri_QDTNH", iID_MaCapPhat = iID_MaCapPhat }, new { @class = "button", target = "_blank" })%>
                </div>
            </td>           
            <td align="left" style="padding-left: 10px; width: 40%"></td>
            <% if (User.Identity.Name == "chuctc" || User.Identity.Name == "b10")
            { %>
            <td style="padding-right: 10px;">
                <input class="button" type="button" value="Cấp gom" onclick="Insert()" />
            </td>
            <%}%>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
    </table>
    <%
        }
    %>
    <%
        if (bang.ChiDoc == true)
        {
    %>
    <table width="100%" cellpadding="0" cellspacing="0" border="0" class="table_form2">
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td align="left" style="padding-left: 10px; width: 40%"></td>
            <td align="right" style="padding-right: 10px; width: 6%">
                <div onclick="OnInit_CT_NEW(500, 'Tùy chỉnh');">
                    <%= Ajax.ActionLink("Tùy chỉnh", "Index", "NhapNhanh", new { id = "CAPPHAT_TUYCHINH", OnLoad = "OnLoad_CT", OnSuccess = "CallSuccess_CT", iID_MaChungTu = iID_MaCapPhat }, new AjaxOptions { }, new { @class = "button" })%>
                </div>
            </td>
            <td align="left" style="padding-right: 10px; width: 6%">
                <input class="button" type="button" value="<%=NgonNgu.LayXau("Quay lại")%>" onclick="Huy()" />
            </td>
            <td align="left" style="padding-left: 10px; width: 40%"></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
    </table>
    <%
        }
    %>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#btnDuyet').live("click", function () {
                Bang_GanMangGiaTri_Bang_arrGiaTri();
                if (document.getElementById("idAction")) document.getElementById("idAction").value = 2;
                if (document.getElementById("sLyDo")) {
                    document.getElementById("sLyDo").value = document.getElementById("CapPhat_sLyDo").value;
                }
                document.getElementById("formDuyet").submit();
            });

            $('#btnTuChoi').live("click", function () {
                Bang_GanMangGiaTri_Bang_arrGiaTri();
                if (document.getElementById("idAction")) document.getElementById("idAction").value = 1;
                if (document.getElementById("sLyDo")) {
                    document.getElementById("sLyDo").value = document.getElementById("CapPhat_sLyDo").value;
                }
                document.getElementById("formDuyet").submit();
            });
        });
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            Bang_arrDSTruongTien = '<%=MucLucNganSachModels.strDSTruongTien%>'.split(',');
            Bang_Url_getGiaTri = '<%=Url.Action("get_GiaTri", "Public")%>';
            Bang_Url_getDanhSach = '<%=Url.Action("get_DanhSach_LNS", "Public")%>';
            BangDuLieu_Url_getGiaTri = '<%=Url.Action("get_GiaTri", "CapPhat_ChungTuChiTiet")%>';
            BangDuLieu_Url_getDanhSach = '<%=Url.Action("get_DanhSach", "CapPhat_ChungTuChiTiet")%>';
            BangDuLieu_iID_MaChungTu = '<%=iID_MaCapPhat%>';
            BangDuLieu_DuocSuaChiTiet = <%=bang.DuocSuaChiTiet?"true":"false"%>;
            Bang_Cell_OrRow = false;
            <%=bang.DuocSuaChiTiet?"":"Bang_keys.fnSetFocus(null, null);"%>
        });
    </script>
    <script type="text/javascript">
        function Insert() {
            window.parent.location.href = '<%=InsertUrl%>';
        }
        function Huy() {
            window.parent.location.href = '<%=BackURL%>';
        }

        function OnInit_CT_NEW(value, title) {
            $("#idDialog").dialog("destroy");
            document.getElementById("idDialog").title = title;
            document.getElementById("idDialog").innerHTML = "";
            $("#idDialog").dialog({
                resizeable: false,
                draggable: true,
                width: value,
                modal: true,
                open: function (event, ui) {
                    $(event.target).parent().css('position', 'fixed');
                    $(event.target).parent().css('top', '10px');

                }
            });
        }
        function OnLoad_CT(v) {
            document.getElementById("idDialog").innerHTML = v;
        }
    </script>
    <div id="idDialog" style="display: none;"></div>
</asp:Content>
