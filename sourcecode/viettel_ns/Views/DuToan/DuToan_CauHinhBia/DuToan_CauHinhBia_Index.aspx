<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site_KeToan_Default.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

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
        String ParentID = "Index";
        String page = Request.QueryString["page"];
        int CurrentPage = 1;
        SqlCommand cmd;

        if (String.IsNullOrEmpty(page) == false)
        {
            CurrentPage = Convert.ToInt32(page);
        }


        String MaND = User.Identity.Name;
        String iNamLamViec = ReportModels.LayNamLamViec(MaND);
        String BackURL = Url.Action("Index", "Home");

        Dictionary<string, object> dicData = (Dictionary<string, object>) ViewData["dicData"];
        NameValueCollection data = (NameValueCollection)dicData["data"];
        string TruongKhoa = (string)dicData["TruongKhoa"];
        String strDanhMuc = Url.Action("ThemNamTruoc", "DuToan_CauHinhBia");  
        using (Html.BeginForm("EditSubmit", "DuToan_CauHinhBia", new { ParentID = ParentID }))
        {
    %>
    <%=MyHtmlHelper.Hidden(ParentID,iNamLamViec,"iNamLamViec","") %>
    <%=MyHtmlHelper.Hidden(ParentID,ViewData["DuLieuMoi"],"DuLieuMoi","") %>
     <%= Html.Hidden(ParentID + "_" + TruongKhoa, data[TruongKhoa])%>
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td align="left" style="width: 12%;">
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

    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Cấu hình Bìa dự toán Năm
                            <%=iNamLamViec%></span>
                    </td>
                    <td align="right" style="padding-right: 10px;">
                    <!-- <input id="Button1" type="button" class="button_title" value="Thêm mới từ năm trước" onclick="javascript:location.href='<%=strDanhMuc %>'" /> -->
                    <button class="btn btn-primary" type="button" onclick="javascript:location.href='<%=strDanhMuc %>'"><i class="fa fa-plus"></i>Thêm mới từ năm trước</button>
                </td>
                </tr>

            </table>
        </div>
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
            <tr>
                <td style="width: 10%" class="td_form2_td1">
                </td>
                <td class="td_form2_td1" style="width: 30%">
                    <div>
                        Tỷ giá:</div>
                </td>
                <td style="width: 30%">
                    <div>
                        <%=MyHtmlHelper.TextBox(ParentID, data["sTyGia"], "sTyGia", "", "class=\"form-control\" style=\"width: 100%\" onchange=\"ChoniThang()\"")%><br/>
                    </div>
                </td>
                <td class="td_form2_td1">
                </td>
            </tr>
            <tr>
                <td style="width: 10%" class="td_form2_td1">
                </td>
                <td class="td_form2_td1" style="width: 30%">
                    <div>
                        Số quyết định:</div>
                </td>
                <td style="width: 30%">
                    <div>
                        <%=MyHtmlHelper.TextBox(ParentID, data["sSoQuyetDinh"], "sSoQuyetDinh", "", "class=\"form-control\" style=\"width: 100%\" onchange=\"ChoniThang()\"")%><br/>
                    </div>
                </td>
               
                <td class="td_form2_td1">
                </td>
            </tr>
            <tr>
                <td style="width: 10%" class="td_form2_td1">
                </td>
                <td class="td_form2_td1" style="width: 30%">
                    <div>
                        Số công văn phòng kế hoạch ngân sách:</div>
                </td>
                <td style="width: 30%">
                    <div>
                        <%=MyHtmlHelper.TextBox(ParentID, data["sSoCVKHNS"], "sSoCVKHNS", "", "class=\"form-control\" style=\"width: 100%\" onchange=\"ChoniThang()\"")%><br/>
                    </div>
                </td>
               
                <td class="td_form2_td1">
                </td>
            </tr>
            <tr>
             <td style="width: 10%" class="td_form2_td1">
                </td>
             <td class="td_form2_td1" style="width: 30%">
                    <div>
                        Số công khai phân bổ:</div>
                </td>
                <td style="width: 30%">
                    <div>
                        <%=MyHtmlHelper.TextBox(ParentID, data["sSoCongKhai"], "sSoCongKhai", "", "class=\"form-control\" style=\"width: 100%\"")%><br/>
                    </div>
                </td>
                <td class="td_form2_td1">
                </td>
            </tr>
            <tr>
                <td style="width: 10%" class="td_form2_td1">
                </td>
                <td class="td_form2_td1" style="width: 30%">
                    <div>
                        Thu cân đối:</div>
                </td>
                <td style="width: 30%">
                    <div>
                        <%=MyHtmlHelper.TextBox(ParentID, data["sThuCanDoi"], "sThuCanDoi", "", "class=\"form-control\" style=\"width: 100%\" onchange=\"ChoniThang()\"")%><br/>
                    </div>
                </td>
                <td class="td_form2_td1">
                </td>
            </tr>
            <tr>
                <td style="width: 10%" class="td_form2_td1">
                </td>
                <td class="td_form2_td1" style="width: 30%">
                    <div>
                        Thu quản lý:</div>
                </td>
                <td style="width: 30%">
                    <div>
                        <%=MyHtmlHelper.TextBox(ParentID, data["sThuQuanLy"], "sThuQuanLy", "", "class=\"form-control\" style=\"width: 100%\" onchange=\"ChoniThang()\"")%><br/>
                    </div>
                </td>
                <td class="td_form2_td1">
                </td>
            </tr>
            <tr>
                <td style="width: 10%" class="td_form2_td1">
                </td>
                <td class="td_form2_td1" style="width: 30%">
                    <div>
                        Thu nộp NSNN:</div>
                </td>
                <td style="width: 30%">
                    <div>
                        <%=MyHtmlHelper.TextBox(ParentID, data["sThuNSNN"], "sThuNSNN", "", "class=\"form-control\" style=\"width: 100%\" onchange=\"ChoniThang()\"")%><br/>
                    </div>
                </td>
                <td class="td_form2_td1">
                </td>
            </tr>
            <tr>
                <td style="width: 10%" class="td_form2_td1">
                </td>
                <td class="td_form2_td1" style="width: 30%">
                    <div>
                        Ngân sách bảo đảm:</div>
                </td>
                <td style="width: 30%">
                    <div>
                        <%=MyHtmlHelper.TextBox(ParentID, data["sNganSachBaoDam"], "sNganSachBaoDam", "", "class=\"form-control\" style=\"width: 100%\" onchange=\"ChoniThang()\"")%><br/>
                    </div>
                </td>
                <td class="td_form2_td1">
                </td>
            </tr>
            <tr>
                <td style="width: 10%" class="td_form2_td1">
                </td>
                <td class="td_form2_td1" style="width: 30%">
                    <div>
                        Lương,phụ cấp, trợ cấp tiền ăn:</div>
                </td>
                <td style="width: 30%">
                    <div>
                        <%=MyHtmlHelper.TextBox(ParentID, data["sLuong"], "sLuong", "", "class=\"form-control\" style=\"width: 100%\" onchange=\"ChoniThang()\"")%><br/>
                    </div>
                </td>
                <td class="td_form2_td1">
                </td>
            </tr>
            <tr>
                <td style="width: 10%" class="td_form2_td1">
                </td>
                <td class="td_form2_td1" style="width: 30%">
                    <div>
                        Nghiệp vụ:</div>
                </td>
                <td style="width: 30%">
                    <div>
                        <%=MyHtmlHelper.TextBox(ParentID, data["sNghiepVu"], "sNghiepVu", "", "class=\"form-control\" style=\"width: 100%\" onchange=\"ChoniThang()\"")%><br/>
                    </div>
                </td>
                <td class="td_form2_td1">
                </td>
            </tr>
            <tr>
                <td style="width: 10%" class="td_form2_td1">
                </td>
                <td class="td_form2_td1" style="width: 30%">
                    <div>
                        Xây dựng cơ bản:</div>
                </td>
                <td style="width: 30%">
                    <div>
                        <%=MyHtmlHelper.TextBox(ParentID, data["sXDCB"], "sXDCB", "", "class=\"form-control\" style=\"width: 100%\" onchange=\"ChoniThang()\"")%><br/>
                    </div>
                </td>
                <td class="td_form2_td1">
                </td>
            </tr>
            <tr>
                <td style="width: 10%" class="td_form2_td1">
                </td>
                <td class="td_form2_td1" style="width: 30%">
                    <div>
                        Hỗ trợ doanh nghiệp:</div>
                </td>
                <td style="width: 30%">
                    <div>
                        <%=MyHtmlHelper.TextBox(ParentID, data["sDoanhNghiep"], "sDoanhNghiep", "", "class=\"form-control\" style=\"width: 100%\" onchange=\"ChoniThang()\"")%><br/>
                    </div>
                </td>
                <td class="td_form2_td1">
                </td>
            </tr>
            <tr>
                <td style="width: 10%" class="td_form2_td1">
                </td>
                <td class="td_form2_td1" style="width: 30%">
                    <div>
                        Ngân sách khác:</div>
                </td>
                <td style="width: 30%">
                    <div>
                        <%=MyHtmlHelper.TextBox(ParentID, data["sNganSachKhac"], "sNganSachKhac", "", "class=\"form-control\" style=\"width: 100%\" onchange=\"ChoniThang()\"")%><br/>
                    </div>
                </td>
                <td class="td_form2_td1">
                </td>
            </tr>
            <tr>
                <td style="width: 10%" class="td_form2_td1">
                </td>
                <td class="td_form2_td1" style="width: 30%">
                    <div>
                        Nhà nước giao:</div>
                </td>
                <td style="width: 30%">
                    <div>
                        <%=MyHtmlHelper.TextBox(ParentID, data["sNhaNuocGiao"], "sNhaNuocGiao", "", "class=\"form-control\" style=\"width: 100%\" onchange=\"ChoniThang()\"")%><br/>
                    </div>
                </td>
                <td class="td_form2_td1">
                </td>
            </tr>
            <tr>
                <td style="width: 10%" class="td_form2_td1">
                </td>
                <td class="td_form2_td1" style="width: 30%">
                    <div>
                        Kinh phí khác:</div>
                </td>
                <td style="width: 30%">
                    <div>
                        <%=MyHtmlHelper.TextBox(ParentID, data["sKinhPhiKhac"], "sKinhPhiKhac", "", "class=\"form-control\" style=\"width: 100%\" onchange=\"ChoniThang()\"")%><br/>
                    </div>
                </td>
                <td class="td_form2_td1">
                </td>
            </tr>
            <tr>
                <td style="width: 10%" class="td_form2_td1">
                </td>
                <td class="td_form2_td1" style="width: 30%">
                    <div>
                        LNS chi tại BQP:</div>
                </td>
                <td style="width: 30%">
                    <div>
                        <%=MyHtmlHelper.TextBox(ParentID, data["xLNS"], "xLNS", "", "class=\"form-control\" style=\"width: 100%\" onchange=\"ChoniThang()\"")%><br/>
                    </div>
                </td>
                <td class="td_form2_td1">
                </td>
            </tr>
            <tr>
                <td style="width: 10%" class="td_form2_td1">
                </td>
                <td class="td_form2_td1" style="width: 30%">
                    <div>
                        Đơn vị chi tại BQP:</div>
                </td>
                <td style="width: 30%">
                    <div>
                        <%=MyHtmlHelper.TextBox(ParentID, data["xDonVi"], "xDonVi", "", "class=\"form-control\" style=\"width: 100%\" onchange=\"ChoniThang()\"")%><br/>
                    </div>
                </td>
                <td class="td_form2_td1">
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <div style="margin-top: 10px;">
                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td style="width: 40%">
                                </td>
                                <td align="right">
                                    <!-- <input type="submit" class="button" value="Lưu" /> -->
                                    <button class="btn btn-primary" type="submit" ><i class="fa fa-download"></i>Lưu</button>
                                </td>
                                <td style="width: 1%">
                                    &nbsp;
                                </td>
                                <td align="left">
                                    <button class="btn btn-default" type="button" onclick="Huy()" ><i class="fa fa-ban"></i>Hủy</button>
                                    <!-- <input type="button" class="button" value="Hủy" onclick="Huy()" /> -->
                                </td>
                                <td style="width: 40%">
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <%} %>
    <script type="text/javascript">
        function Huy() {
            window.location.href = '<%=BackURL%>';
        }
</script>
</asp:Content>

