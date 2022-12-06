<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="System.ComponentModel" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="<%= Url.Content("~/Scripts/jsMucLucNganSach.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>"
        type="text/javascript"></script>    
    <%        
        String MaND = User.Identity.Name;
        String IPSua = Request.UserHostAddress;
        String sLNS = Request.QueryString["sLNS"];                
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
        </tr>
    </table>
    <div style="width: 100%; margin-top: 10px;">        
        <div id="divChungTuChiTietHT" style="width: 100%; position: relative">
            <div class="box_tong">
                <div class="title_tong">
                    <table cellpadding="0" cellspacing="0" border="0" width="100%">
                        <tr>
                            <td align="left">
                                <span>Mục lục ngân sách</span>
                            </td>
                        </tr>
                    </table>
                </div>                
                <div>
                    <%Html.RenderPartial("~/Views/DungChung/MucLucNganSach/MucLucNganSach_Edit_Search.ascx", new { ControlID = "MucLucNganSach", MaND = User.Identity.Name, ChucNangCapNhap = true}); %>
                </div>        
            </div>
        </div>
    </div>                 
        <script type="text/javascript">
            $(document).ready(function () {
                jsMLNS_LNS = "<%=sLNS%>";
                jsMLNS_Url_Frame = '<%=Url.Action("MLNSChiTiet_Frame", "MucLucNganSach")%>?';                    
                jsMLNS_Url = '<%=Url.Action("Edit", "MucLucNganSach", new { sLNS = sLNS})%>';
                $("#tabs").tabs();
            });            
        </script>              
</asp:Content>
