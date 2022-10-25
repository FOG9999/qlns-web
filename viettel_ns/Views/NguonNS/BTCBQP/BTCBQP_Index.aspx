<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site_KeToan_Default.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="Viettel.Services" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script src="<%= Url.Content("~/Scripts/NguonNS/jsNguon_BTCBQP.js") %>" type="text/javascript"></script>    
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
            <td align="right" style="padding-bottom: 5px; color: #ec3237; font-weight: bold;
                padding-right: 20px;">
                <% Html.RenderPartial("LogOnUserControl_KeToan"); %>
            </td>
        </tr>
    </table>
    <div style="width: 100%; float: left;">
        <div style="width: 100%; float: left;">
            <div class="box_tong">
                <div class="title_tong">
                    <table cellpadding="0" cellspacing="0" border="0" width="100%">
                        <tr>    
                            <td>&nbsp;</td>
                                                    
                            <td align="right" style="width: 320px;">
                                <b>F2:Danh sách danh mục Bộ Tài chính chưa map</b>
                            </td>
                            <td align="right" style="width: 500px;">
                                <b>F3:Danh sách danh mục Bộ Tài chính đã map với mục chi hiện tại</b>
                            </td>                            
                            <td align="right" style="width: 150px;">
                                <b>Backspace: Sửa </b>
                            </td>
                            <td align="right" style="width: 100px;">
                                <b>F10: Lưu</b>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="nhapform">
                    <div id="form2">                        
                        <%Html.RenderPartial("~/Views/NguonNS/BTCBQP/BTCBQP_Index_DanhSach.ascx", new { ControlID = "BTCBQP_Index" });%>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            jsBTCBQP_Index_Url_Frame = '<%=Url.Action("BTCBQP_Index_Frame", "Nguon_BTCBQP")%>';
            $("#tabs").tabs();
        });
    </script>
</asp:Content>
