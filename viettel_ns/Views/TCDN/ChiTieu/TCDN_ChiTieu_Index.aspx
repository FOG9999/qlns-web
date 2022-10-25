<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<asp:Content ID="Content3" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <%
        string ParentID = "TCDN_ChiTieu";
        string sTenChiTieu = "", sKyHieu = "";
        int i;
        sTenChiTieu = Request.QueryString["sTenChiTieu"];
        sKyHieu = Request.QueryString["KyHieu"];

        //đoạn code để khi chọn thêm mới
        String strThemMoi = Url.Action("Edit", "TCDN_ChiTieu", new { ID = string.Empty });
        //
        String strSort = Url.Action("Sort", "TCDN_ChiTieu", new { iID_MaChiTieu_Cha = string.Empty });
        //sự kiện tìm kiếm được chọn
        using (Html.BeginForm("SearchSubmit", "TCDN_ChiTieu", new{ ParentID = ParentID}))
        {
    %>
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td align="left" style="width: 10%;">
                <div style="padding-left: 22px; padding-bottom: 5px; text-transform:uppercase; color:#ec3237;">
                    <b><%=NgonNgu.LayXau("Liên kết nhanh: ")%></b>
                </div>         
            </td>
            <td align="left">
                <div style="padding-bottom: 5px; color:#ec3237;">
                    <%=MyHtmlHelper.ActionLink(Url.Action("Index", "Home"), "Trang chủ")%> |
                    <%=MyHtmlHelper.ActionLink(Url.Action("Index", "TCDN_ChiTieu"), "Danh sách chỉ tiêu")%>
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
                        <td valign="top" align="left" style="width: 30%;">
                            <table cellpadding="5" cellspacing="5" width="100%">
                                <tr>
                                    <td class="td_form2_td1">
                                        <div>
                                            <b>
                                                <%=NgonNgu.LayXau("Tên chỉ tiêu")%></b></div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%=MyHtmlHelper.TextBox(ParentID, sTenChiTieu, "sTenChiTieu", "", "class=\"form-control\" tab-index='-1' ")%>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td valign="top" align="left" style="width: 30%;">
                            <table cellpadding="5" cellspacing="5" width="100%">
                                <tr>
                                    <td class="td_form2_td1">
                                        <div>
                                            <b>
                                                <%=NgonNgu.LayXau("Ký hiệu")%></b></div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%=MyHtmlHelper.TextBox(ParentID, sKyHieu, "sKyHieu", "", "class=\"form-control\"")%>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td align="left" style="width: 5%;">&nbsp;</td>
                        <td align="left" style="width: 35%;">    
                            <%--<input type="submit" class="button" value="Tìm kiếm" />--%>
                            <button class="btn btn-info" type="submit"><i class="fa fa-search"></i>Tìm kiếm</button>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    <%  } %>
    <br />
    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                      <span>Danh sách chỉ tiêu</span>
                    </td>
                    <td align="right">
       <%--                 <input id="Button1" type="button" class="button_title" value="Thêm mới" onclick="javascript:location.href='<%=strThemMoi %>'" />
                        <input id="Button1" type="button" class="button_title" value="Sắp xếp" onclick="javascript:location.href='<%=strSort %>'" />--%>

                     <button id="Button1" class="btn btn-primary" type="button" onclick="javascript:location.href='<%=strThemMoi %>'"><i class="fa fa-plus"></i>Thêm mới</button>
                     <button id="" class="btn btn-info" type="button" onclick="javascript:location.href='<%=strSort %>'"><i class="fa fa-sort"></i>Sắp xếp</button>
                       
                    </td>
                </tr>
            </table>
            
        </div>
        <table class="mGrid">
            <tr>
                <th style="width: 10%;" align="center">
                    Mã  chỉ tiêu
                </th>
                <th style="width: 45%;" align="center">
                    Tên chỉ tiêu
                </th>
               <%-- <th style="width: 15%;" align="center">
                    Ký hiệu thuyết minh
                </th>
                <th style="width: 15%;" align="center">
                    Loại chỉ tiêu
                </th>--%>
                <th style="width: 15%;" align="center">
                    Hành động
                </th>
            </tr>
            <%
                string urlCreate = Url.Action("Create", "TCDN_ChiTieu", new { iID_MaChiTieu_Cha = "##" });
                string urlDetail = Url.Action("Index", "TCDN_ChiTieu", new { iID_MaChiTieu_Cha = "##" });
                string urlEdit = Url.Action("Edit", "TCDN_ChiTieu", new { iID_MaChiTieu = "##" });
                string urlDelete = Url.Action("Delete", "TCDN_ChiTieu", new { iID_MaChiTieu = "##" });
                string urlSort = Url.Action("Sort", "TCDN_ChiTieu", new { iID_MaChiTieu_Cha = "##" });
                int ThuTu = 0;
                String XauHanhDong = "";
                String XauSapXep = "";

                XauHanhDong += MyHtmlHelper.ActionLink(urlCreate, "<i class='fa fa-plus fa-lg color-icon-edit' title='Thêm mục con'></i>", "Create", "");
                XauHanhDong += MyHtmlHelper.ActionLink(urlEdit, "<i class='fa fa-pencil-square-o fa-lg color-icon-edit' title='Sửa'></i>", "Edit", "");
                XauHanhDong += MyHtmlHelper.ActionLink(urlDelete, "<i class='fa fa-trash-o fa-lg color-icon-delete' title='Xóa'></i>", "Delete", "");
                XauSapXep = MyHtmlHelper.ActionLink(urlSort, "<i class='fa fa-sort fa-lg color-icon-sort' title='Sắp xếp'></i>", "Sort", "");
            %>
            <%=TCDN_ChiTieuModels.LayXauChiTieu(sTenChiTieu, sKyHieu, Url.Action("", ""), XauHanhDong, XauSapXep, "", 0, ref ThuTu)%>
        </table>
    </div>
</asp:Content>
