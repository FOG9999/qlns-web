<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site_KeToan_Default.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="Viettel.Services" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script src="<%= Url.Content("~/Scripts/NguonNS/jsNguon_CTChi_PCCT.js") %>" type="text/javascript"></script>
    <%
        string MaND = User.Identity.Name;

        string Id_CTChi = Request.QueryString["Id_CTChi"];
        if (string.IsNullOrEmpty(Id_CTChi)) Id_CTChi = Convert.ToString(ViewData["Id_CTChi"]);

        string Id_NguonBTC = Request.QueryString["Id_NguonBTC"];
        if (string.IsNullOrEmpty(Id_NguonBTC)) Id_NguonBTC = Convert.ToString(ViewData["Id_NguonBTC"]);

        INguonNSService _nguonNSService = NguonNSService.Default;
        
        var data = _nguonNSService.GetChungTu("Nguon_CTChi",Id_CTChi);
    %>
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td align="left" style="width: 9%;">
                <div style="padding-left: 22px; padding-bottom: 5px; text-transform: uppercase; color: #ec3237;">
                    <b>
                        <%=NgonNgu.LayXau("Liên kết nhanh: ")%></b>
                </div>
            </td>
            <td align="left">
                <div style="padding-bottom: 5px; color: #ec3237;">
                    <%=MyHtmlHelper.ActionLink(Url.Action("Index", "Home"), "Trang chủ")%>|                    
                    |
                    <%=MyHtmlHelper.ActionLink(Url.Action("Index", "ChungTuChi_ChungTu"), "Danh sách chứng từ chi")%>
                </div>
            </td>
            <td align="right" style="padding-bottom: 5px; color: #ec3237; font-weight: bold;
                padding-right: 20px;">
                <% Html.RenderPartial("LogOnUserControl_KeToan"); %>
            </td>
        </tr>
    </table>
    <div style="width: 100%; float: left;">
        <div style="width: 100%; float: left;">
            <div class="box_tong">
                <div class="title_tong">
                    <table cellpadding="0" cellspacing="0" border="0" width="100%">
                        <tr>
                            <td>
                                <span>Thông tin chứng từ</span>
                            </td>
                            <td align="right" style="width: 320px;">
                                <b>F2:Sao chép dòng dữ liệu hiện tại</b>
                            </td>
                            <td align="right" style="width: 120px;">
                                <b>Delete: Xóa</b>
                            </td>
                            <td align="right" style="width: 150px;">
                                <b>Backspace: Sửa </b>
                            </td>
                            <td align="right" style="width: 100px;">
                                <b>F10: Lưu</b>
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                    </table>
                </div>
                <div id="nhapform">
                    <div id="form2">
                        <table width="100%" cellpadding="0" cellspacing="0" border="0">
                            <tr>
                                <td style="width: 50%;" valign="top">
                                    <table width="100%" cellpadding="0" cellspacing="0" border="0" class="table_form3">
                                        <tr>
                                            <td class="td_form2_td1">
                                                <div><b>Loại chứng từ</b></div>
                                            </td>
                                            <td class="td_form2_td5">
                                                <div><b><%=data["LoaiCT"]%></b></div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="td_form2_td1">
                                                <div><b>Quyết định ngày</b></div>
                                            </td>
                                            <td class="td_form2_td5">
                                                <div><b><%=data["NgayQD"] %></b></div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 50%;" valign="top">
                                    <table width="100%" cellpadding="0" cellspacing="0" border="0" class="table_form3">
                                        <tr>
                                            <td class="td_form2_td1" style="width: 15%">
                                                <div><b>Số quyết định</b></div>
                                            </td>
                                            <td class="td_form2_td5">
                                                <div><b><%=data["SoQD"] %></b></div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="td_form2_td1">
                                                <div><b>Nội dung</b></div>
                                            </td>
                                            <td class="td_form2_td5">
                                                <div><b><%=data["NoiDung"]%></b></div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <%Html.RenderPartial("~/Views/NguonNS/ChungTuChi/ChungTuChiTietPhanCap/ChungTuChiTiet_Index_DanhSach.ascx", new { ControlID = "ChungTuChiTiet_Index", MaND = User.Identity.Name }); %>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            jsNguon_CTChi = "<%=Id_CTChi%>";
            jsNguon_NguonBTC = "<%=Id_NguonBTC%>";
            jsNguon_CTChi_PC_Url_Frame = '<%=Url.Action("ChungTuChiTiet_Frame", "ChungTuChi_ChungTuChiTiet_PC", new { Id_CTChi = Id_CTChi, Id_NguonBTC = Id_NguonBTC})%>';
            $("#tabs").tabs();
        });
    </script>
</asp:Content>
