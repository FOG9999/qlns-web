<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site_KeToan_Default.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="VIETTEL.Models.ThuNop" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="<%= Url.Content("~/Scripts/ThuNop/jsBang_ThuNop_h.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>"
        type="text/javascript"></script>
         <style type="text/css">

#menu1 ul{
    border: 1px solid #4297d7;
    color: #fff;
    border-radius: 5px;
    font-weight: bold;
    margin: 0;
    padding: 0em 0.2em 0;
}
 
#menu1 ul li
{
     float: left;
    padding: 4px 2em;
    text-decoration: none;
    cursor: pointer;
display:inline;
}

#menu1 ul a
{
  
}
   </style>
    <%
        String iID_MaChungTu = Request.QueryString["iID_MaChungTu"];
        if (String.IsNullOrEmpty(iID_MaChungTu)) iID_MaChungTu = Convert.ToString(ViewData["iID_MaChungTu"]);
        NameValueCollection data = ThuNop_ChungTuModels.LayThongTin(iID_MaChungTu);
        String strThang = "";
        switch (Convert.ToString(data["iThang_Quy"]))
        {
            case ("0"):
                strThang = "Năm trước chuyển sang";
                break;
            case ("13"):
                strThang = "Cả năm";
                break;
            default:
                strThang = "Tháng " + Convert.ToString(data["iThang_Quy"]);
                break;
        }
        String strTenDonVi = DonViModels.Get_TenDonVi(data["iID_MaDonVi"],User.Identity.Name);
        int iLoai = Convert.ToInt32(data["iLoai"]);
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
                    <%=MyHtmlHelper.ActionLink(Url.Action("Index", "Home"), "Trang chủ")%>
                    |
                    <%=MyHtmlHelper.ActionLink(Url.Action("Index", "ThuNop_ChungTu", new { iLoai = 1 }), "Kế hoạch thu")%>
                    |
                    <%=MyHtmlHelper.ActionLink(Url.Action("Index", "ThuNop_ChungTu", new { iLoai = 2 }), "Tổng hợp thu năm")%>
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
            <div class="custom_css box_tong">
                <div class="title_tong">
                    <table cellpadding="0" cellspacing="0" border="0" width="100%">
                        <tr>
                            <td>
                                <span>Thông tin chứng từ:
                                    <%
                                        switch (iLoai)
                                        {
                                            case 1: %>
                                   Kế hoạch thu
                                    <%    break;
                                            case 2: %>
                                    Tổng hợp thu năm
                                    <%   break;

                                        }
                                    %>
                                </span>
                            </td>
                            <td align="right">
                                <span>F2:Thêm dòng</span>
                            </td>
                            <td align="right" style="width: 125px;">
                                <span>Delete: Xóa</span>
                            </td>
                            <td align="right" style="width: 150px;">
                                <span>Backspace: Sửa </span>
                            </td>
                             <td align="right" style="width: 110px;">
                                <span>F12: In kiểm </span>
                            </td>
                            <td align="left">
                                <span>F10: Lưu</span>
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="nhapform">
                    <div id="form2">
                        <table width="100%" cellpadding="0" cellspacing="0" border="0" class="table_form3">
                            <tr>
                                <td class="td_form2_td1" style="width: 10%;">
                                    <div>
                                        <b>Số chứng từ</b></div>
                                </td>
                                <td class="td_form2_td5" style="width: 10%;">
                                    <div>
                                        <b>
                                            <%=data["sTienToChungTu"]%><%=data["iSoChungTu"]%></b></div>
                                </td>
                                 <td class="td_form2_td1" style="width: 10%;">
                                    <div>
                                        <b>Đơn vị</b></div>
                                </td>
                                <td class="td_form2_td5" style="width: 10%;">
                                    <div>
                                        <b>
                                            <%=strTenDonVi%></b></div>
                                </td>

                                <td class="td_form2_td1" style="width: 15%;">
                                    <div>
                                        <b>Số liệu thu nộp của</b></div>
                                </td>
                                <td class="td_form2_td5" style="width: 10%;">
                                    <div>
                                        <b>
                                            <%=strThang%></b></div>
                                </td>
                                <td class="td_form2_td1" style="width: 10%;">
                                    <div>
                                        <b>Nội dung</b></div>
                                </td>
                                <td class="td_form2_td5" style="width: 50%;">
                                    <div>
                                        <b>
                                            <%=data["sNoiDung"]%></b></div>
                                </td>
                            </tr>
                        </table>
                         
                         <div style="clear:both ">
                        <%Html.RenderPartial("~/Views/ThuNop/ChungTuChiTiet/ThuNopChiTiet_Index_DanhSach.ascx", new { ControlID = "ChiTieuChiTiet", MaND = User.Identity.Name, iLoai = iLoai }); %>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>    
</asp:Content>
