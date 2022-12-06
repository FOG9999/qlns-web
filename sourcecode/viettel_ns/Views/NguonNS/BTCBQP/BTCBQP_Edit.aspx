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
    <script src="<%= Url.Content("~/Scripts/NguonNS/jsNguon_BTCBQP_CT.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>"
        type="text/javascript"></script>    
    <%        
        string loai = Request.QueryString["loai"];
        string Id_MaNguon = Request.QueryString["Id_MaNguon"];
        var text = Id_MaNguon == "1" ? "chưa map" : "đã map";
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
    <div style="width: 100%; float: left; margin-top: 10px;">        
        <div id="divChungTuChiTietHT" style="width: 100%; float: left; position: relative">
            <div class="box_tong">
                <div class="title_tong">
                    <table cellpadding="0" cellspacing="0" border="0" width="100%">
                        <tr>
                            <td align="left">
                                <span>Danh mục Bộ Tài chính cấp <%=text%></span>
                            </td>
                        </tr>
                    </table>
                </div>                
                <div>
                    <%Html.RenderPartial("~/Views/NguonNS/BTCBQP/BTCBQP_Edit_Search.ascx", new { ControlID = "BTCBQP_Edit", Id_MaNguon = Id_MaNguon, loai = loai});  %>
                </div>        
            </div>
        </div>
    </div>                 
        <script type="text/javascript">
            $(document).ready(function () {
                jsBTCBQP_Edit_Id_MaNguon = "<%=Id_MaNguon%>";
                jsBTCBQP_Edit_Loai = "<%=loai%>";
                jsBTCBQP_Edit_Url_Frame = '<%=Url.Action("BTCBQP_Edit_Frame", "Nguon_BTCBQP", new { Id_MaNguon = Id_MaNguon, loai = loai})%>';                    
                $("#tabs").tabs();
            });            
        </script>                
</asp:Content>
