<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site_KeToan_Default.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="VIETTEL.Models.ThuNop" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        int i;
        String ParentID = "TN_DuLieuDaNhap";
        String MaND = User.Identity.Name;
        String page = Request.QueryString["page"];

        DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(MaND);
        DataRow RCauHinh = dtCauHinh.Rows[0];

        String strNamLamViec = Convert.ToString(RCauHinh["iNamLamViec"]);

        int CurrentPage = 1;
        if (String.IsNullOrEmpty(page) == false)
        {
            CurrentPage = Convert.ToInt32(page);
        }

        DataTable dt = ThuNop_DuLieuDaNhapModels.Get_DanhSachDuLieu(MaND, CurrentPage, Globals.PageSize);
        
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
                </div>
            </td>
            <td align="right" style="padding-bottom: 5px; color: #ec3237; font-weight: bold; padding-right: 20px;">
                <% Html.RenderPartial("LogOnUserControl_KeToan"); %>
            </td>
        </tr>
    </table>
    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Danh sách dữ liệu Thu Nộp đã nhập <%=strNamLamViec%></span>
                    </td>
                </tr>
            </table>
        </div>
        <table class="mGrid">
            <tr>
                <th style="width: 5%;" align="center">STT
                </th>
                <th style="width: 5%;" align="center">Mã Đơn vị
                </th>
                <th style="width: 10%;" align="center">Tên Đơn vị
                </th>
                <th style="width: 5%;" align="center">B5
                </th>
                <th style="width: 5%;" align="center">B5_Đã nhập
                </th>
                <th style="width: 5%;" align="center">B6
                </th>
                <th style="width: 5%;" align="center">B6_Đã nhập
                </th>
                <th style="width: 5%;" align="center">B7
                </th>
                <th style="width: 5%;" align="center">B7_Đã nhập
                </th>
                <th style="width: 5%;" align="center">B10
                </th>
                <th style="width: 5%;" align="center">B10_Đã nhập
                </th>
                <th style="width: 5%;" align="center">B17
                </th>
                <th style="width: 5%;" align="center">B17_Đã nhập
                </th>
            </tr>
            <%
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow R = dt.Rows[i];
                    int STT = i + 1;
                    String strB5 = "", strB5_DaNhap = "", strB6 = "", strB6_DaNhap = "", strB7 = "", strB7_DaNhap = "", strB10 = "", strB10_DaNhap = "", strB16 = "", strB16_DaNhap = "", strB17 = "", strB17_DaNhap = "";
                    String strColor = "";

                    strColor = String.Format("style='background-color: #dff0fb; background-repeat: repeat;'");

                    String strMaDV = Convert.ToString(R["iID_MaDonVi"]);
                    String strTenDonvi = strMaDV + " - " + Convert.ToString(DonViModels.Get_TenDonVi(strMaDV, MaND));

                    if (Convert.ToBoolean(R["B5"]))
                    {
                        strB5 = string.Format("<div class=\"check\"></div>");
                    }
                    if (Convert.ToBoolean(R["B5_DaNhap"]))
                    {
                        strB5_DaNhap = string.Format("<div class=\"check\"></div>");
                    }
                    if (Convert.ToBoolean(R["B6"]))
                    {
                        strB6 = string.Format("<div class=\"check\"></div>");
                    }
                    if (Convert.ToBoolean(R["B6_DaNhap"]))
                    {
                        strB6_DaNhap = string.Format("<div class=\"check\"></div>");
                    }
                    if (Convert.ToBoolean(R["B7"]))
                    {
                        strB7 = string.Format("<div class=\"check\"></div>");
                    }
                    if (Convert.ToBoolean(R["B7_DaNhap"]))
                    {
                        strB7_DaNhap = string.Format("<div class=\"check\"></div>");
                    }
                    if (Convert.ToBoolean(R["B10"]))
                    {
                        strB10 = string.Format("<div class=\"check\"></div>");
                    }
                    if (Convert.ToBoolean(R["B10_DaNhap"]))
                    {
                        strB10_DaNhap = string.Format("<div class=\"check\"></div>");
                    }                    
                    if (Convert.ToBoolean(R["B17"]))
                    {
                        strB17 = string.Format("<div class=\"check\"></div>");
                    }
                    if (Convert.ToBoolean(R["B17_DaNhap"]))
                    {
                        strB17_DaNhap = string.Format("<div class=\"check\"></div>");
                    }

            %>
            <tr <%=strColor %>>
                <td align="center">
                    <%=STT%>
                </td>
                <td align="center">
                    <%=strMaDV %>
                </td>
                <td align="left">
                    <%=strTenDonvi %>
                </td>
                <td align="center">
                    <%=strB5 %>
                </td>
                <td align="center">
                    <%=strB5_DaNhap %>
                </td>
                <td align="center">
                    <%=strB6 %>
                </td>
                <td align="center">
                    <%=strB6_DaNhap %>
                </td>
                <td align="center">
                    <%=strB7 %>
                </td>
                <td align="center">
                    <%=strB7_DaNhap %>
                </td>
                <td align="center">
                    <%=strB10 %>
                </td>
                <td align="center">
                    <%=strB10_DaNhap %>
                </td>
                <td align="center">
                    <%=strB17 %>
                </td>
                <td align="center">
                    <%=strB17_DaNhap %>
                </td>
            </tr>
            <%} %>
        </table>
    </div>
    <%  
        dt.Dispose();
    %>
</asp:Content>
