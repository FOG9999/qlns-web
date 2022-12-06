﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
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
    String ParentID = "MucLuc_DoiTuong";
    String MaND = User.Identity.Name;

    String iID_MaMucLucDoiTuong_Cha = Convert.ToString(Request.QueryString["iID_MaMucLucDoiTuong_Cha"]);
    
    String strThemMoi = Url.Action("Edit", "MucLucDoiTuong", new { iID_MaMucLucDoiTuong_Cha = iID_MaMucLucDoiTuong_Cha });
    String strSort = Url.Action("Sort", "MucLucDoiTuong", new { iID_MaMucLucDoiTuong_Cha = iID_MaMucLucDoiTuong_Cha });
    String MaHangMauChaTruoc = "";
    if (iID_MaMucLucDoiTuong_Cha != null && iID_MaMucLucDoiTuong_Cha != "")
    {
        SqlCommand cmd;
        cmd = new SqlCommand("SELECT iID_MaMucLucDoiTuong_Cha FROM NS_MucLucDoiTuong WHERE iID_MaMucLucDoiTuong=@iID_MaMucLucDoiTuong");
        cmd.Parameters.AddWithValue("@iID_MaMucLucDoiTuong", iID_MaMucLucDoiTuong_Cha);
        MaHangMauChaTruoc = Convert.ToString(Connection.GetValue(cmd, ""));
        cmd.Dispose();
    }
    String strBack = Url.Action("Index", "MucLucDoiTuong", new { iID_MaMucLucDoiTuong_Cha = MaHangMauChaTruoc });
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
                <%=MyHtmlHelper.ActionLink(Url.Action("Index", "MucLucDoiTuong"), "Danh sách đối tượng")%>
            </div>
        </td>
    </tr>
</table>
<div class="custom_css box_tong">
    <div class="title_tong">
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
        	<tr>
            	<td>
                	<span>Mục lục ngân sách đối tượng</span>
                </td>
                <td align="right">
                    <button id="Button2" class="btn btn-info pull-right ml-10" onclick="javascript:location.href='<%=strSort %>'">
                        <i class="fa fa-sort" aria-hidden="true"></i>Sắp xếp</button>
                    <button id="Button1" class="btn btn-primary pull-right" onclick="javascript:location.href='<%=strThemMoi %>'">
                        <i class="fa fa-plus" aria-hidden="true"></i>Thêm mới</button>
                    <%--<input id="Button3" type="button" class="button_title" value="Cấp trước" onclick="javascript:location.href='<%=strBack %>'" />--%>
                </td>
            </tr>
        </table>
    </div>
    <table class="mGrid">
        <tr>
            <th style="width: 20%;" align="center">Mã ký hiệu</th>
            <th style="width: 50%;" align="center"> Mô tả</th>
            <th style="width: 30%;" align="center">Thao tác</th>
        </tr>
        <%
            string urlCreate = Url.Action("Create", "MucLucDoiTuong", new { iID_MaMucLucDoiTuong_Cha = "##" });
            string urlDetail = Url.Action("Index", "MucLucDoiTuong", new { iID_MaMucLucDoiTuong_Cha = "##" });
            string urlEdit = Url.Action("Edit", "MucLucDoiTuong", new { iID_MaMucLucDoiTuong = "##" });
            string urlDelete = Url.Action("Delete", "MucLucDoiTuong", new { iID_MaMucLucDoiTuong = "##" });
            string urlSort = Url.Action("Sort", "MucLucDoiTuong", new { iID_MaMucLucDoiTuong_Cha = "##" });
            int ThuTu = 0;
            String XauHanhDong = "";
            String XauSapXep = "";
            XauHanhDong += MyHtmlHelper.ActionLink(urlCreate, "<i class='fa fa-plus fa-lg color-icon-edit' title='Thêm mục con'></i>", "Create", "");
            XauHanhDong += MyHtmlHelper.ActionLink(urlEdit, "<i class='fa fa-pencil-square-o fa-lg color-icon-edit' title='Sửa'></i>", "Edit", "");
            XauHanhDong += MyHtmlHelper.ActionLink(urlDelete,"<i class='fa fa-trash-o fa-lg color-icon-delete' title='Xóa'></i>", "Delete", "");
            XauSapXep = MyHtmlHelper.ActionLink(urlSort, "<i class='fa fa-sort fa-lg color-icon-sort' title='Sắp xếp'></i>", "Sort", "");
        %>
        <%=NganSach_DoiTuongModels.LayXauMucLuc(Url.Action("", ""), XauHanhDong, XauSapXep, "", 0, ref ThuTu)%>
    </table>
</div>
</asp:Content>




