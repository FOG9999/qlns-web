<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site_KeToan_Default.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        int i;
        String ParentID = "Index";
        String iID_MaPhongBan =(String) ViewData["iID_MaPhongBan"];    
        String MaND = User.Identity.Name;
        String iNamLamViec = ReportModels.LayNamLamViec(MaND);
        String BackURL = Url.Action("Index", "Home");
        DataTable dt = DuToan_ChungTuModels.getNguoiDung_CauHinhKhoa(iID_MaPhongBan,MaND);
         if (dt.Rows.Count == 0)
         {
             //String SQL = String.Format(@"");
         }
        using (Html.BeginForm("EditSubmit", "DuToan_CauHinhBia", new { ParentID = ParentID }))
        {
    %>
    <%=MyHtmlHelper.Hidden(ParentID,iNamLamViec,"iNamLamViec","") %>
    <%=MyHtmlHelper.Hidden(ParentID,ViewData["DuLieuMoi"],"DuLieuMoi","") %>
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td align="left" style="width: 12%;">
                <div style="padding-left: 22px; padding-bottom: 5px; text-transform: uppercase; color: #ec3237;">
                    <b>
                        <%=NgonNgu.LayXau("Liên kết nhanh: ")%></b>
                </div>
            </td>
            <td align="left">
                <div style="padding-bottom: 5px; color: #ec3237;">
                    <%=MyHtmlHelper.ActionLink(Url.Action("Index", "Home"), "Trang chủ")%>
                </div>
            </td>
            <td align="right" style="padding-bottom: 5px; color: #ec3237; font-weight: bold;
                padding-right: 20px;">
                <% Html.RenderPartial("LogOnUserControl_KeToan"); %>
            </td>
        </tr>
    </table>

    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Cấu hình khóa số liệu Năm
                            <%=iNamLamViec%></span>
                    </td>
                </tr>
            </table>
        </div>
        <table class="mGrid" cellpadding="0" cellspacing="0" border="0" width="100%">
         <tr>
                <th style="width: 2%;" align="center">
                    STT
                </th>
                <th style="width: 5%;" align="center">
                    Người dùng
                </th>
                <th style="width: 15%;" align="center">
                    Khóa
                </th>
            </tr>
             <%
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow R = dt.Rows[i];
                    String sKhoa = "Mở";
                    if (R["iKhoa"].ToString() == "1") sKhoa = "Khóa";
                    %>
                    <tr>
                <td align="center">
                    <b>
                        <%=i+1%></b>
                </td>
                <td align="left">
                    <b>
                        <%=MyHtmlHelper.ActionLink(Url.Action("Edit", "DuToan_KhoaSoLieu", new { iID_MaPhongBan = iID_MaPhongBan, sMaNguoiDung = R["sMaNguoiDung"], iKhoa = R["iKhoa"] }).ToString(), R["sMaNguoiDung"], "Detail", "")%></b>
                </td>
                <td align="center">
                    <b>
                        <%=MyHtmlHelper.ActionLink(Url.Action("Edit", "DuToan_KhoaSoLieu", new { iID_MaPhongBan = iID_MaPhongBan, sMaNguoiDung = R["sMaNguoiDung"], iKhoa = R["iKhoa"] }).ToString(), sKhoa, "Detail", "")%></b>
                </td>
               </tr>
                    <%
                }%>
          
        </table>
    </div>
    <%} %>
    <script type="text/javascript">
        function Huy() {
            window.location.href = '<%=BackURL%>';
        }
</script>
</asp:Content>

