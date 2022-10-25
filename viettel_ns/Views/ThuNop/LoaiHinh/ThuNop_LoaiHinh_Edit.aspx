<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="VIETTEL.Models.ThuNop" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        String iID_MaLoaiHinh = Convert.ToString(ViewData["iID_MaLoaiHinh"]);
        String iID_MaLoaiHinh_Cha = Convert.ToString(ViewData["iID_MaLoaiHinh_Cha"]);
        String ParentID = "Edit";
        String sKyHieu = "", sTen = "", sTT="";
        bool bLaHangCha = false;
        bool bLaTong = false;
        bool bLaText = false;
        //chi tiết chỉ tiêu nếu trong trường hợp sửa
        DataTable dt = ThuNop_LoaiHinhModels.Get_ChiTietLoaiHinh_Row(iID_MaLoaiHinh);
        if (dt.Rows.Count > 0 && iID_MaLoaiHinh != null && iID_MaLoaiHinh != "")
        {
            DataRow R = dt.Rows[0];
            sKyHieu = HamChung.ConvertToString(R["sKyHieu"]);
            sTen = HamChung.ConvertToString(R["sTen"]);
            bLaHangCha = Convert.ToBoolean(R["bLaHangCha"]);
            bLaTong = Convert.ToBoolean(R["bLaTong"]);
            bLaText = Convert.ToBoolean(R["bLaText"]);            
            sTT = Convert.ToString(R["sTT"]);            
        }
        String tgLaHangCha = "";
        String tgLaTong = "";
        String tgLaText = "";
        if (bLaHangCha == true)
        {
            tgLaHangCha = "on";
        }
        if (bLaTong == true)
        {
            tgLaTong = "on";
        }
        if (bLaText == true)
        {
            tgLaText = "on";
        }

        using (Html.BeginForm("EditSubmit", "ThuNop_LoaiHinh", new { ParentID = ParentID, iID_MaLoaiHinh = iID_MaLoaiHinh }))
        {
    %>
    <%= Html.Hidden(ParentID + "_DuLieuMoi", ViewData["DuLieuMoi"])%>
    <%= Html.Hidden(ParentID + "_iID_MaLoaiHinh_Cha", iID_MaLoaiHinh_Cha)%>
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td style="width: 10%">
                <div style="padding-left: 20px; padding-top: 5px; padding-bottom: 5px; text-transform: uppercase;
                    color: #ec3237;">
                    <b>
                        <%=NgonNgu.LayXau("Liên kết nhanh: ")%></b>
                </div>
            </td>
            <td align="left">
                <div style="padding-top: 5px; padding-bottom: 5px; color: #ec3237;">
                    <%=MyHtmlHelper.ActionLink(Url.Action("Index", "Home"), "Trang chủ")%>
                    |
                    <%=MyHtmlHelper.ActionLink(Url.Action("Index", "ThuNop_LoaiHinh"), "Danh sách mục lục loại hình hoạt động có thu")%>
                </div>
            </td>
        </tr>
    </table>
    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                     <% if (ViewData["DuLieuMoi"] == "1")
                   {
                       %>
                	 <span>Nhập thông tin danh sách loại hình hoạt động có thu</span>
                    <% 
                   }
                   else
                   { %>
                    <span>Sửa thông tin danh sách loại hình hoạt động có thu</span>
                    <% } %>
                       
                    </td>
                </tr>
            </table>
        </div>
        <div id="nhapform">
            <div id="form2">
                <table cellpadding="5" cellspacing="5" width="50%">
                    <tr>
                        <td class="td_form2_td1">
                            <div>
                                <b>Mã loại hình</b></div>
                        </td>
                        <td class="td_form2_td5">
                            <div>
                                <%
                                String strReadonly = "";
                                if (ViewData["DuLieuMoi"] == "0") {
                                    strReadonly = "readonly=\"readonly\""; 
                                }    
                                %>
                                <%=MyHtmlHelper.TextBox(ParentID, sKyHieu, "sKyHieu", "", " " + strReadonly + " class=\"form-control\" tab-index='-1'", 2)%>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="td_form2_td1">
                            <div>
                                <b>Tên loại hình</b>&nbsp;<span  style="color:Red;">*</span></div>
                        </td>
                        <td class="td_form2_td5">
                            <div>
                                <%=MyHtmlHelper.TextBox(ParentID, sTen, "sTen", "", "class=\"form-control\"", 2)%>
                                <%= Html.ValidationMessage(ParentID + "_" + "err_sTen")%>
                            </div>
                        </td>
                    </tr>                     
                    <tr>
                        <td class="td_form2_td1">
                            <div>
                                <b>STT</b>&nbsp;</div>
                        </td>
                        <td class="td_form2_td5">
                            <div>
                                <%=MyHtmlHelper.TextBox(ParentID, sTT, "sTT", "", "class=\"form-control\"", 2)%>
                            </div>
                        </td>
                    </tr>                 
                    <tr>
                        <td class="td_form2_td1"><div><b>Là tổng</b></div></td>
                        <td class="td_form2_td5">
                            <div><%=MyHtmlHelper.CheckBox(ParentID, tgLaTong, "bLaTong", String.Format("value='{0}'", bLaHangCha))%></div>
                        </td>
                    </tr>
                    <tr>
                        <td class="td_form2_td1"><div><b>In đậm</b></div></td>
                        <td class="td_form2_td5">
                            <div><%=MyHtmlHelper.CheckBox(ParentID, tgLaHangCha, "bLaHangCha", String.Format("value='{0}'", bLaHangCha))%></div>
                        </td>
                    </tr>
                     <tr>
                        <td class="td_form2_td1"><div><b>Là Text</b></div></td>
                        <td class="td_form2_td5">
                            <div><%=MyHtmlHelper.CheckBox(ParentID, tgLaText, "bLaText","")%></div>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
 
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td width="70%">
                &nbsp;
            </td>
            <td width="30%" align="right">
                <table cellpadding="0" cellspacing="0" border="0" align="right">
                    <tr>
                        <td>
                              <button class="btn btn-primary" type="submit" id="Submit1"><i class="fa fa-download"></i>Lưu</button>
                        </td>
                        <td width="5px">
                        </td>
                        <td>
                               <button class="btn btn-default" type="button" onclick="javascript:history.go(-1)"><i class="fa fa-ban"></i>Hủy</button>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
           </div>
    <br />
    <%
        } if (dt != null) { dt.Dispose(); };    
    %>
</asp:Content>
