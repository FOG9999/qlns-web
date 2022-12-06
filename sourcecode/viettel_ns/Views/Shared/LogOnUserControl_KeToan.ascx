<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="DomainModel.Abstract" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%--<td style="float:right; text-align:center; font-weight:bold; padding-right:20px; vertical-align:middle;">
    <% 
        String iNamLamViec = ReportModels.LayNamLamViec(System.Web.HttpContext.Current.User.Identity.Name);
    %>

    <span style="color: Gray;">Xin chào:</span>
    <span style="color: Red;"><%= Html.Encode(Page.User.Identity.Name) %></span>
    <span style="color: Gray;">(Thời gian:</span>
    <span style="color: Red;"><%= DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")%></span>
    <span style="color: Gray;">- Năm làm việc:</span>
    <span style="color: Red;"><%= iNamLamViec%></span>
    <span style="color: Gray;">- IP:</span>
    <span style="color: Red;"><%= Request.UserHostAddress%></span>
    <span style="color: Gray;">)</span>&nbsp;| <a href="<%=Url.Action("ChangePassword", "Account")%>">
        Đổi mật khẩu</a> | <a href="<%=Url.Action("SSOLogOff", "Account")%>">Thoát</a>
</td>--%>
