<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="DomainModel.Abstract" %>
<%@ Import Namespace="VIETTEL.Models" %>
<div style="margin-top: 15px;">
    <ul id="user_details_menu">
        <%
            if (Request.IsAuthenticated)
            {
                String MaND = Page.User.Identity.Name;
                DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(MaND);
                String iNamLamViec = Convert.ToString(dtCauHinh.Rows[0]["iNamLamViec"]);
        %>
        <li><a href="#">Xin chào: <%= Html.Encode(Page.User.Identity.Name) %></a></li>
        <li><a href="<%=Url.Action("SSOLogOff", "Account")%>">Thoát</a> | <a href="<%=Url.Action("ChangePassword", "Account")%>">Đổi mật khẩu</a></li>
        <li><a href="#">Năm làm việc: <%= iNamLamViec%></a></li>
        <li><a href="#">Thời gian: <%= DateTime.Now.ToString("dd/MM/yyyy")%></a></li>
        <li><a href="#">IP: <%= Request.UserHostAddress%></a></li>
        <%
            }
            else
            {
        %>
        <li><a href="<%=Url.Action("LogOn", "Account")%>">Đăng nhập</a></li>
        <%
            }
        %>
    </ul>
</div>
