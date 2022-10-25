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
        String page = Request.QueryString["page"];
        int CurrentPage = 1;
        SqlCommand cmd;

        if (String.IsNullOrEmpty(page) == false)
        {
            CurrentPage = Convert.ToInt32(page);
        }


        String MaND = User.Identity.Name;
        String iNamLamViec = ReportModels.LayNamLamViec(MaND);
        String BackURL = Url.Action("Index", "Home");

        Dictionary<string, object> dicData = (Dictionary<string, object>) ViewData["dicData"];
        NameValueCollection data = (NameValueCollection)dicData["data"];
         string TruongKhoa = (string)dicData["TruongKhoa"];
         String strDanhMuc = Url.Action("ThemNamTruoc", "DuToan_CauHinhBia");
         DataTable dt = PhongBanModels.GetDanhSachPhongBan();
         if (dt.Rows.Count == 0)
         {
             DuToan_ChungTuModels.ThemNamTruoc(dt,MaND);
             dt = DuToan_ChungTuModels.getDanhSachKhoa(MaND);
         }
        using (Html.BeginForm("EditSubmit", "DuToan_CauHinhBia", new { ParentID = ParentID }))
        {
    %>
    <%=MyHtmlHelper.Hidden(ParentID,iNamLamViec,"iNamLamViec","") %>
    <%=MyHtmlHelper.Hidden(ParentID,ViewData["DuLieuMoi"],"DuLieuMoi","") %>
     <%= Html.Hidden(ParentID + "_" + TruongKhoa, data[TruongKhoa])%>
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
                    Phòng ban
                </th>
            </tr>
             <%
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow R = dt.Rows[i];
                    %>
                    <tr>
                <td align="center">
                    <b>
                        <%=i+1%></b>
                </td>
                <td align="center">
                    <b>
                        <%=MyHtmlHelper.ActionLink(Url.Action("Detail", "DuToan_KhoaSoLieu", new { iID_MaPhongBan = R["sKyHieu"] }).ToString(), R["sKyHieu"], "Detail", "")%></b>
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

