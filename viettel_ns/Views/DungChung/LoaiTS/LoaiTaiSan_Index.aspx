<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

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
        String ParentID = "Loai";
        String MaND = User.Identity.Name;
        String KyHieu = Request.QueryString["KyHieu"];
        String Ten = Request.QueryString["Ten"];      
        String page = Request.QueryString["page"];

        String sKyHieu = "", sTen = "";
        //đoạn lệnh nhảy đến phần thêm mới
        String strThemMoi = Url.Action("Edit", "LoaiTaiSan");
        using (Html.BeginForm("SearchSubmit", "LoaiTaiSan", new { ParentID = ParentID }))
        {
    %>
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td align="left" style="width: 10%;">
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
    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Thông tin tìm kiếm</span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="nhapform">
            <div id="form2">
                <table border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td valign="top" align="left" style="width: 45%;">
                            <table cellpadding="5" cellspacing="5" width="100%">
                                <tr>
                                    <td class="td_form2_td1">
                                        <div>
                                            <b>Ký hiệu</b></div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%=MyHtmlHelper.TextBox(ParentID, sKyHieu, "sKyHieu", "", "class=\"form-control\" tab-index='-1'")%>
                                        </div>
                                    </td>
                                </tr>
                              
                            </table>
                        </td>
                        <td valign="top" align="left" style="width: 45%;">
                            <table cellpadding="5" cellspacing="5" width="100%">
                                <tr>
                                    <td class="td_form2_td1">
                                        <div>
                                            <b>Tên loại tài sản</b></div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%=MyHtmlHelper.TextBox(ParentID, sTen, "sTen", "", "class=\"form-control\"")%>
                                        </div>
                                    </td>
                                </tr>
                               
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center" class="td_form2_td1" style="height: 10px;">
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center" style="padding: 0px 0px 10px 0px;">
                            <table border="0" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <%--<input type="submit" class="button" value="Tìm kiếm" />--%>
                                        <button class="btn btn-info" type="submit"><i class="fa fa-search"></i> Tìm kiếm</button>
                                    </td>
                                    <td style="width: 10px;">
                                    </td>
                                    <td>
                                        <%--<input id="TaoMoi" type="button" class="button" value="Tạo mới" onclick="javascript:location.href='<%=strThemMoi %>'" />--%>
                                        <button class="btn btn-primary" id="TaoMoi" type="button"  onclick="javascript:location.href='<%=strThemMoi %>'"><i class="fa fa-plus"></i>Thêm mới</button>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    <%  } %>
    <br />
    <script type="text/javascript">
        List_QuyetToan_ChungTu();
        function List_QuyetToan_ChungTu() {
            jQuery.ajaxSetup({ cache: false });
            var url = unescape('<%= Url.Action("get_List?KyHieu=#0&Ten=#1&page=#2", "LoaiTaiSan")%>');
            url = unescape(url.replace("#0", "<%=KyHieu %>"));
            url = unescape(url.replace("#1", "<%=Ten %>"));     
            url = unescape(url.replace("#2", "<%=page %>"));

            $.getJSON(url, function (data) {
                document.getElementById("divListQuyetToan").innerHTML = data;
            });
        }      
    
    </script>
    <div id="divListQuyetToan">
    </div>
</asp:Content>
