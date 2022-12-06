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
    <script src="<%= Url.Content("~/Scripts/NguonNS/jsNguon_DMBQP.js") %>?id=<%=DateTime.Now.ToString("YYYYMMddHHmmss") %>"
        type="text/javascript"></script>    
    <%        
        String IPSua = Request.UserHostAddress;
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
                                <span>Danh mục chi Bộ Quốc phòng</span>
                            </td>
                            <td align="right" style="width: 350px;">
                                <span>F2:Thêm dòng cùng cấp với dòng đang chọn</span>
                            </td>
                            <td align="right" style="width: 320px;">
                                <span>F3:Thêm dòng là con với dòng đang chọn</span>
                            </td>
                            <td align="right" style="width: 120px;">
                                <span>Delete: Xóa</span>
                            </td>
                            <td align="right" style="width: 150px;">
                                <span>Backspace: Sửa </span>
                            </td>
                            <td align="left" style="width: 100px;">
                                <span>F10: Lưu</span>
                            </td>
                        </tr>
                    </table>
                </div>                
                <div>
                    <%Html.RenderPartial("~/Views/NguonNS/Nguon_DMBQP/Nguon_DMBQP_Edit_Search.ascx", new { ControlID = "Nguon_DMBQP"});  %>
                </div>        
            </div>
        </div>
    </div>                 
        <script type="text/javascript">
            $(document).ready(function () {
                jsDMBQPEdit_Url_Frame = '<%=Url.Action("Nguon_DMBQP_Edit_Frame", "Nguon_DMBQP")%>';                    
                $("#tabs").tabs();
            });            
        </script>                
</asp:Content>
