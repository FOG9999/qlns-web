<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="VIETTEL.Models.ThuNop" %>
<asp:Content ID="Content3" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <%
        string ParentID = "TN_LoaiHinh";
        string sTenLoaiHinh = "", sKyHieu = "";
        int i;
        sTenLoaiHinh = Request.QueryString["sTenLoaiHinh"];
        sKyHieu = Request.QueryString["sKyHieu"];

        //đoạn code để khi chọn thêm mới
        String strThemMoi = Url.Action("Edit", "ThuNop_LoaiHinh", new { ID = string.Empty });
        //
        String strSort = Url.Action("Sort", "ThuNop_LoaiHinh", new { iID_MaLoaiHinh_Cha = string.Empty });
        
        //sự kiện tìm kiếm được chọn
        using (Html.BeginForm("SearchSubmit", "ThuNop_LoaiHinh", new { ParentID = ParentID }))
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
                    <%=MyHtmlHelper.ActionLink(Url.Action("Index", "TN_LoaiHinh"), "Danh sách mục lục loại hình hoạt động có thu")%>
                </div>
            </td>
        </tr>
    </table>   
    <%  } %>
    <br />
    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                      <span>Danh sách loại hình hoạt động có thu</span>
                    </td>
                    <td align="right">
                         <button class="btn btn-primary" type="button" id="Button1" onclick="javascript:location.href='<%=strThemMoi %>'" ><i class="fa fa-plus"></i><%=NgonNgu.LayXau("Thêm mới")%></button>
                        <%--<input id="Button1" type="button" class="button_title" value="Thêm mới" onclick="javascript:location.href='<%=strThemMoi %>'" />--%>
                        <%--<input id="Button2" type="button" class="button_title" value="Sắp xếp" onclick="javascript:location.href='<%=strSort %>'" />--%>
                         <button class="btn btn-info" type="button" id="Button2" onclick="javascript:location.href='<%=strSort %>'" ><i class="fa fa-sort"></i><%=NgonNgu.LayXau("Sắp xếp")%></button>
                       
                    </td>
                </tr>
            </table>
            
        </div>
        <table class="mGrid">
            <tr>
                <th style="width: 10%;" align="center">
                    Thứ tự
                </th> 
                <th style="width: 10%;" align="center">
                    Mã loại hình
                </th>
                <th style="width: 45%;" align="center">
                    Tên loại hình hoạt động có thu
                </th>                                 
                <th style="width: 15%;" align="center">
                </th>
            </tr>
            <%
                string urlCreate = Url.Action("Create", "ThuNop_LoaiHinh", new { iID_MaLoaiHinh_Cha = "##" });
                string urlDetail = Url.Action("Index", "ThuNop_LoaiHinh", new { iID_MaLoaiHinh_Cha = "##" });
                string urlEdit = Url.Action("Edit", "ThuNop_LoaiHinh", new { iID_MaLoaiHinh = "##" });
                string urlDelete = Url.Action("Delete", "ThuNop_LoaiHinh", new { iID_MaLoaiHinh = "##" });
                string urlSort = Url.Action("Sort", "ThuNop_LoaiHinh", new { iID_MaLoaiHinh_Cha = "##" });
                int ThuTu = 0;
                String XauThemMoi = "";
                String XauHanhDong = "";
                String XauSapXep = "";
                XauThemMoi += MyHtmlHelper.ActionLink(urlCreate, "<i class='fa fa-plus fa-lg color-icon-edit' title='Thêm mục con'></i>", "Create", "");
                XauHanhDong += MyHtmlHelper.ActionLink(urlEdit, "<i class='fa fa-pencil-square-o fa-lg color-icon-edit' title='Sửa'></i>", "Edit", "");
                XauHanhDong += MyHtmlHelper.ActionLink(urlDelete, "<i class='fa fa-trash-o fa-lg color-icon-delete' title='Xóa'></i>", "Delete", "");
                XauSapXep = MyHtmlHelper.ActionLink(urlSort, "<i class='fa fa-trash-o fa-lg color-icon-sort' title='Sắp xếp'></i>", "Sort", "");
            %>
            <%=ThuNop_LoaiHinhModels.LayXauLoaiHinh(sTenLoaiHinh, sKyHieu, Url.Action("", ""), XauThemMoi, XauHanhDong, XauSapXep, "", 0, ref ThuTu)%>
        </table>
    </div>
</asp:Content>
