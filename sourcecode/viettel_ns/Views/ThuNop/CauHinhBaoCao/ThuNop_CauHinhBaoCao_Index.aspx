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
        string ParentID = "TN_CauHinhBaoCao";
        string sTenBaoCao = "", sKyHieu = "", sMaCotBaoCao = "";        
        int i;
        sTenBaoCao = Request.QueryString["sTenBaoCao"];
        sKyHieu = Request.QueryString["sKyHieu"];
        sMaCotBaoCao = Request.QueryString["sMaCotBaoCao"];
        String iLoai = Request.QueryString["iLoai"];
        if (String.IsNullOrEmpty(iLoai))
            iLoai = "1";

        //đoạn code để khi chọn thêm mới
        String strThemMoi = Url.Action("Edit", "ThuNop_CauHinhBaoCao", new { ID = string.Empty, iLoai = iLoai });
       
        //sự kiện tìm kiếm được chọn
        using (Html.BeginForm("SearchSubmit", "ThuNop_CauHinhBaoCao", new { ParentID = ParentID }))
        {
    %>
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td align="left" style="width: 9%;">
                <div style="padding-left: 22px; padding-bottom: 5px; text-transform:uppercase; color:#ec3237;">
                    <b><%=NgonNgu.LayXau("Liên kết nhanh: ")%></b>
                </div>         
            </td>
            <td align="left">
                <div style="padding-bottom: 5px; color:#ec3237;">
                    <%=MyHtmlHelper.ActionLink(Url.Action("Index", "Home"), "Trang chủ")%> |
                    <%=MyHtmlHelper.ActionLink(Url.Action("Index", "TN_CauHinhBaoCao"), "Danh sách cấu hình báo cáo")%>
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
                      <span>Danh sách cấu hình báo cáo</span>
                    </td>
                    <td align="right">
                         <button class="btn btn-primary" type="button" id="Button1" onclick="javascript:location.href='<%=strThemMoi %>'" ><i class="fa fa-plus"></i><%=NgonNgu.LayXau("Thêm mới")%></button>
                        <%--<input id="Button1" type="button" class="button_title" value="Thêm mới" onclick="javascript:location.href='<%=strThemMoi %>'" />--%>                       
                    </td>
                </tr>
            </table>
            
        </div>
            <table class="mGrid">
                <tr>
                <th style="width: 5%;" align="center">
                    Thứ tự
                </th> 
                <th style="width: 10%;" align="center">
                    Loại ngân sách thu nộp
                </th>
                <th style="width: 10%;" align="center">
                    Mã thứ tự cột trong báo cáo
                </th>                
                <th style="width: 10%;" align="center">
                    Mã cấu hình báo cáo
                </th>
                <th style="width: 30%;" align="center">
                    Tên cấu hình báo cáo
                </th>                                 
                <th style="width: 5%;" align="center">
                </th>
            </tr>
            <%                
                string urlEdit = Url.Action("Edit", "ThuNop_CauHinhBaoCao", new { iID_MaBaoCao = "##" , iLoai = iLoai });
                string urlDelete = Url.Action("Delete", "ThuNop_CauHinhBaoCao", new { iID_MaBaoCao = "##",  iLoai = iLoai });
                int ThuTu = 0;
                String XauHanhDong = "";
                XauHanhDong += MyHtmlHelper.ActionLink(urlEdit, "<i class='fa fa-pencil-square-o fa-lg color-icon-edit' title='Sửa'></i>", "Edit", "");
                XauHanhDong += MyHtmlHelper.ActionLink(urlDelete, "<i class='fa fa-trash-o fa-lg color-icon-delete' title='Xóa'></i>", "Delete", "");
            %>
            <%=ThuNop_CauHinhBaoCaoModels.LayXauBaoCao(iLoai,sTenBaoCao, sKyHieu, Url.Action("", ""), XauHanhDong, "", 0, ref ThuTu)%>
        </table>    
        
    </div>
</asp:Content>
