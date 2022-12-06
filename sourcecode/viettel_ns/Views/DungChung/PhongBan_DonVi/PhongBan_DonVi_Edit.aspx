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
    <script src="<%= Url.Content("~/Scripts/PhongBan_DonVi/jsPBDVCT.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>"
        type="text/javascript"></script>    
    <%        
        String IPSua = Request.UserHostAddress;
        String phongban = Request.QueryString["phongban"];                
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
                                <span>Cấu hình phân quyền phòng ban</span>
                            </td>
                        </tr>
                    </table>
                </div>                
                <div>
                    <%Html.RenderPartial("~/Views/DungChung/PhongBan_DonVi/PhongBan_DonVi_Edit_Search.ascx", new { ControlID = "PhongBan_DonVi", phongban = phongban});  %>
                </div>        
            </div>
        </div>
    </div>                 
        <script type="text/javascript">
            $(document).ready(function () {
                jsPBDVEdit_phongban = "<%=phongban%>";
                jsPBDVEdit_Url_Frame = '<%=Url.Action("PhongBan_DonVi_Edit_Frame", "PhongBan_DonVi", new { phongban = phongban})%>';                    
                $("#tabs").tabs();
            });            
        </script>                
</asp:Content>
