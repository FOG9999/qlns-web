<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        String sLoaiBaoCao = Request.QueryString["sLoai"];
        String name = User.Identity.Name;
    %>
    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Danh sách báo cáo</span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="nhapform">
            <div id="form2">
                <table class="mGrid">
                    <tr>
                        <th style="width: 3%;">STT
                        </th>
                        <th>Tên báo cáo
                        </th>
                        <th style="width: 12%;">Phụ lục số
                        </th>
                    </tr>
                    <%
                        switch (sLoaiBaoCao)
                        {
                            case "0":
                    %>
                    <%--<tr class="alt">
                        <td align="center" style="padding: 3px 2px;">1
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_TongHop_TongHop"), "Tổng hợp Bìa")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 4c-C
                        </td>
                    </tr>--%>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">1
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_TongHopCuc_Bia"), "Tổng hợp Bìa mới")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 4c-C
                        </td>
                    </tr>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">2
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_ThuNop_TongHop_4c_C"), "Tổng hợp dự toán ngân sách  Phần thu")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 4c-C
                        </td>
                    </tr>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">3
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_ThuNop_TongHop"), "Tổng hợp phần thu ngân sách (Biểu tổng họp theo đơn vị)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 4c2-C
                        </td>
                    </tr>
                    <%--<tr class="alt">
                        <td align="center" style="padding: 3px 2px;">4
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_TongHop_1"), "Số phân bổ dự toán ngân sách các đơn vị (Phần ngân sách quốc phòng)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 4d1-C
                        </td>
                    </tr>--%>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">4
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_TongHopCuc_PBQP"), "Số phân bổ dự toán ngân sách các đơn vị (Phần ngân sách quốc phòng) mới")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 4d1-C
                        </td>
                    </tr>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">4_1
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Print", "rptDuToan_KhoBac_TachB", new { area = "DuToan", ext = "pdf"}), "Số phân bổ dự toán ngân sách các đơn vị (Phần ngân sách quốc phòng) mới tách BQL")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 4d1-C
                        </td>
                    </tr>                   
                    <%--<tr class="alt">
                        <td align="center" style="padding: 3px 2px;">4_1
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_TongHopCuc_PBQP_TB"), "Số phân bổ dự toán ngân sách các đơn vị (Phần ngân sách quốc phòng) tách BQL")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 4d1s-C
                        </td>
                    </tr>--%>
                    <%--<tr class="alt">
                        <td align="center" style="padding: 3px 2px;">5
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_TongHop_2"), "Số phân bổ dự toán ngân sách các đơn vị (Phần ngân sách nhà nước)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 4d2-C
                        </td>
                    </tr>--%>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">5
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_TongHopCuc_PBNN"), "Số phân bổ dự toán ngân sách các đơn vị (Phần ngân sách nhà nước) mới")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 4d2-C
                        </td>
                    </tr>
                    <%--<tr class="alt">
                        <td align="center" style="padding: 3px 2px;">5_1
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_TongHopCuc_PBNN_TB"), "Số phân bổ dự toán ngân sách các đơn vị (Phần ngân sách nhà nước) tách BQL")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 4d2s-C
                        </td>
                    </tr>--%>
                    <%--<tr class="alt">
                        <td align="center" style="padding: 3px 2px;">6
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_TongHop_THB2"), "Tổng hợp phần chi ngân sách")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Biểu THB-2
                        </td>
                    </tr>--%>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">6
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_TongHopCuc_THB2"), "Tổng hợp phần chi ngân sách mới")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Biểu THB-2
                        </td>
                    </tr>
                    <%--<tr class="alt">
                        <td align="center" style="padding: 3px 2px;">6_1
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_TongHopCuc_THB2_Min"), "Tổng hợp phần chi ngân sách gộp")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Biểu THB-2_b
                        </td>
                    </tr>--%>
                    <%--<tr class="alt">
                        <td align="center" style="padding: 3px 2px;">7
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptPhanBoDuToanNganSachNam"), "Công khai phân bổ dự toán ngân sách")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">PL số 2
                        </td>
                    </tr>--%>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">7
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_TongHopCuc_PhanBoDTNS"), "Công khai phân bổ dự toán ngân sách mới")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">PL số 2
                        </td>
                    </tr>
                    <%--<tr class="alt">
                        <td align="center" style="padding: 3px 2px;">8
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1040100_TongHop"), "Dự toán chi ngân sách quốc phòng (Phần ngân sách bảo đảm)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 1a-C
                        </td>
                    </tr>--%>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">8
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_BaoDam_TongHop_Bia"), "Dự toán chi ngân sách quốc phòng (Phần ngân sách bảo đảm) mới")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 1a-C
                        </td>
                    </tr>
                    <%--<tr class="alt">
                        <td align="center" style="padding: 3px 2px;">8_2
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_BaoDam_TongHop_ChiTiet", new {area = "DuToan"}), "Dự toán chi ngân sách quốc phòng (Phần ngân sách bảo đảm - phần phân cấp)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 1a-C
                        </td>
                    </tr>--%>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">9
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1010000_TongHop"), "Dự toán chi ngân sách sử dụng (phần thường xuyên)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 2a-C
                        </td>
                    </tr>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">10
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_NghiepVu_TongHop"), "Tổng hợp ngân sách nghiệp vụ mới")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 2c-C
                        </td>
                    </tr>
                    <%--<tr class="alt">
                        <td align="center" style="padding: 3px 2px;">22_1
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_NghiepVu_TongHopDT"), "Tổng hợp ngân sách nghiệp vụ đơn vị dự toán")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 2c-C_1
                        </td>
                    </tr>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">22_2
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_NghiepVu_TongHopDN"), "Tổng hợp ngân sách nghiệp vụ doanh nghiệp")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 2c-C_2
                        </td>
                    </tr>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">22_3
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_NghiepVu_TongHopBV"), "Tổng hợp ngân sách nghiệp vụ bệnh viện")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 2c-C_3
                        </td>
                    </tr>--%>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">11
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1020200_TongHop"), "Dự toán chi ngân sách quốc phòng (phần ngân sách tùy viên quốc phòng)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 2d-C
                        </td>
                    </tr>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">12
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1020700_TongHop"), "Dự toán chi ngân sách quốc phòng (Phần BHYT) ")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">2e-C
                        </td>
                    </tr>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">13
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1020800_TongHop"), "Nghiệp vụ - Phần chi khoa học nền")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">2f-C
                        </td>
                    </tr>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">14
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1030100_TongHop_XDCB"), "Dự toán chi ngân sách quốc phòng (phần ngân sách XDCB)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 3-C
                        </td>
                    </tr>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">15
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1030100_TongHop_3bC"), "Dự toán chi ngân sách quốc phòng (phần ngân sách XDCB biểu 2)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">3b-C
                        </td>
                    </tr>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">16
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1050000_TongHop"), "Dự toán chi ngân sách quốc phòng (phần chi cho Doanh Nghiệp)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">4a-C
                        </td>
                    </tr>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">17
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1050000_TongHop_ChonTo"), "Dự toán chi ngân sách quốc phòng (phần chi cho Doanh Nghiệp chọn tờ) ")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">4a2-C
                        </td>
                    </tr>                    
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">18
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1050100_TongHop"), "Dự toán chi ngân sách quốc phòng (phần chi hỗ trợ doanh nghiệp làm nhiệm vụ C) ")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">2gC
                        </td>
                    </tr>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">19
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_109_TongHop"), "Tổng hợp dự toán chi việc nhà nước giao tính vào NSQP")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 4b-C
                        </td>
                    </tr>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">20
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_2_TongHop"), "Tổng hợp dự toán ngân sách nhà nước ")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 5C
                        </td>
                    </tr>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">20_1
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_207_TongHop", new { area = "DuToan"}), "Tổng hợp dự toán ngân sách ngành pháp chế ")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 5C_1
                        </td>
                    </tr>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">21
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_TongHop_2060101"), "Tổng hợp ngân sách người có công")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 5d-C
                        </td>
                    </tr>
                    <%--<tr class="alt">
                        <td align="center" style="padding: 3px 2px;">22
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1020000_01_TongHop"), "Tổng hợp ngân sách nghiệp vụ chung")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 2c-C
                        </td>
                    </tr>--%>                    
                    <%--<tr class="alt">
                        <td align="center" style="padding: 3px 2px;">23
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_109_PhanCap"), "Tổng hợp ngân sách nhà nước giao tính vào NSQP (phần phân cấp)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 4b-C
                        </td>
                    </tr>--%>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">24
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptCongKhaiPhanBoVonDauTuCongTrinhPhoThong"), "Công khai phân bổ XDCB")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 3
                        </td>
                    </tr>
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">25
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_2_B8_TongHop"), "Nghiệp vụ B8")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">B8
                        </td>
                    </tr>                    
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">26
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_4_TongHop"), "Điều tiết lợi nhuận các DN sau thuế")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">PL6a-C
                        </td>
                    </tr>                    
                    <tr class="alt">
                        <td align="center" style="padding: 3px 2px;">28
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_5_TongHop"), "Hỗ trợ đơn vị sự nghiệp")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">PL7-C
                        </td>
                    </tr>
                    <%--<tr class="alt">
                        <td align="center" style="padding: 3px 2px;">
                            22
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1020000_TongHop"), "Tổng hợp ngân sách nghiệp vụ chung")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">
                            Phụ lục số 2b-C
                        </td>
                    </tr>--%>
                    <%
                            break;
                        case "1":
                    %>
                    <%--<tr>
                        <td align="center" style="padding: 3px 2px;">1
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_TongHop_TungDonVi"), "Bìa")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Biểu số 01
                        </td>
                    </tr>--%>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">1
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_TongHop_TungDonVi_Bia"), "Bìa mới")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Biểu số 01
                        </td>
                    </tr>
                    <tr>
                    <td align="center" style="padding: 3px 2px;">2
                        </td>
                    <td style="padding: 3px 2px;">
                        <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_ThuNop_TungDonVi"), "Dự toán ngân sách năm - Phần thu")%>
                        </td>
                    <td align="center" style="padding: 3px 2px;">Phụ lục số 1T
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">3
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_KhoBac", new { area = "DuToan"}), "DỰ TOÁN CHI NGÂN SÁCH QUỐC PHÒNG - Phần chi tại kho bạc theo Mục - Tiểu mục")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 1b
                        </td>
                    </tr> 
                   
                   

                   



                    <%--<tr>
                        <td align="center" style="padding: 3px 2px;">3
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1040100_TungDonVi"), "DỰ TOÁN CHI NGÂN SÁCH QUỐC PHÒNG - Phần ngân sách bảo đảm toàn quân")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 1a
                        </td>
                    </tr>--%>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">4
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_BaoDam_TungDonVi_Bia"), "DỰ TOÁN CHI NGÂN SÁCH QUỐC PHÒNG - Phần ngân sách bảo đảm toàn quân tờ bìa")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 1a
                        </td>
                    </tr>
                    <%--<tr><td align="center" style="padding: 3px 2px;">4
                        </td>
                    <td style="padding: 3px 2px;">
                        <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1040100_TungNganh"), "DỰ TOÁN CHI NGÂN SÁCH QUỐC PHÒNG - Phần phân cấp ngân sách bảo đảm toàn quân")%>
                        </td>
                    <td align="center" style="padding: 3px 2px;">Phụ lục số 1b
                        </td>
                    </tr>--%>

                    <tr>
                        <td align="center" style="padding: 3px 2px;">5
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_BaoDam_TungDonVi_ChiTiet", new { area = "DuToan"}), "DỰ TOÁN CHI NGÂN SÁCH QUỐC PHÒNG - Phần phân cấp ngân sách bảo đảm toàn quân theo ngành")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 1b - skt
                        </td>
                    </tr>                    
                    <tr>
                        <td align="center" style="padding: 3px 2px;">5_2
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_BaoDam_KyThuat_ChiTiet", new { area = "DuToan"}), "DỰ TOÁN CHI NGÂN SÁCH QUỐC PHÒNG - Phần phân cấp ngân sách bảo đảm kĩ thuật đơn vị lập dự toán ")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 1d - KT
                        </td>
                    </tr> 
                    <tr>
                        <td align="center" style="padding: 3px 2px;">5_3
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_BaoDam_KyThuat_ChiTietNg", new { area = "DuToan"}), "DỰ TOÁN CHI NGÂN SÁCH QUỐC PHÒNG - Phần phân cấp ngân sách bảo đảm kĩ thuật đơn vị lập dự toán chi tiết đến ngành")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 1d - KT
                        </td>
                    </tr>                  
                    <tr>
                        <td align="center" style="padding: 3px 2px;">6
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1010000_TungDonVi"), "Dự toán chi ngân sách sử dụng (Phần lương, phụ cấp, trợ cấp, tiền ăn)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 2a
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">6_1
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1010000_TungDonVi_DoanhNghiep"), "Dự toán chi ngân sách sử dụng (Phần lương, phụ cấp, trợ cấp, tiền ăn)_Doanh Nghiệp")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 2a
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">7
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1020100_TungDonVi"), "Dự toán chi ngân sách sử dụng (Phần ngân sách nghiệp vụ)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 2b
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">7_1
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1020100_TungDonVi_DoanhNghiep"), "Dự toán chi ngân sách sử dụng (Phần ngân sách nghiệp vụ)_Doanh nghiệp")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 2b
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">8
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1020200_TungDonVi"), "Dự toán chi ngân sách quốc phòng (Phần ngân sách Tùy viên Quốc phòng)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 5d
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">8_1
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1020100_KT_TungDonVi"), "Dự toán chi ngân sách sử dụng (Phần ngân sách nghiệp vụ ngành kỹ thuật)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 2d
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">9
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1020500_TungDonVi"), "DỰ TOÁN CHI NGÂN SÁCH QUỐC PHÒNG (Hỗ trợ các đoàn KTQP & Đơn vị sự nghiệp công lập)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 2e
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">10
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1050100_TungDonVi"), "DỰ TOÁN CHI NGÂN SÁCH QUỐC PHÒNG (Phần chi hỗ trợ doanh nghiệp làm nhiệm vụ C)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 2g
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">11
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1030100_TungDonVi"), "Dự toán chi ngân sách xây dựng cơ bản (Nguồn ngân sách Quốc phòng)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 3
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">12
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1050000_TungDonVi"), "Dự toán chi ngân sách Quốc phòng (Ngân sách hỗ trợ Doanh nghiệp)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 4a
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">13
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_109_TungDonVi"), "Dự toán chi việc nhà nước giao tính vào NSQP")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 4b
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">13_1
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_109_PhanCap_DonVi"), "Dự toán chi việc nhà nước giao tính vào NSQP _Phân cấp")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 4b
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">14
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_2_TungDonVi"), "Dự toán chi ngân sách nhà nước giao")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 5
                        </td>
                    </tr>                    
                    <%--<tr>
                        <td align="center" style="padding: 3px 2px;">14
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_206_TungDonVi"), "Dự toán chi ngân sách nhà nước giao(Phần chi người có công)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 5d
                        </td>
                    </tr>--%>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">14_1
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_207_TungDonVi"), "DỰ TOÁN CHI NGÂN SÁCH NHÀ NƯỚC- Phần Chi QLHC")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 5a
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">15
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_207_TungNganh"), "DỰ TOÁN CHI NGÂN SÁCH NHÀ NƯỚC - Phần phân cấp")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 5b
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">16
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1020000_TungDonVi"), "DỰ TOÁN CHI NGÂN SÁCH QUỐC PHÒNG- nghiệp vụ 00")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 2d
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">17
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1020000_TungDonVi_DoanhNghiep"), "DỰ TOÁN CHI NGÂN SÁCH QUỐC PHÒNG- nghiệp vụ 00- Doanh nghiệp")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 2d
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">18
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_4_TungDonVi"), "Dự toán kinh phí khác")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 6
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">19
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1020700_TungDonVi"), "Nghiệp vụ B8 - Bảo hiểm y tế - Từng đơn vị")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">B8
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">20
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1020800_TungDonVi"), "Chi khoa học nền - Từng đơn vị")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục 2f
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">21
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_5_TungDonVi"), "Hỗ trợ đơn vị sự nghiệp")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục số 7
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">XX
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_DonVi"), "In tổng hợp báo cáo đơn vị")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">IN DON VI
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">XX_1
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_THNSNN_ToBia", new { area = "DuToan"}), "Dự toán chi ngân sách nhà nước tờ bìa")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục in gửi ngành</td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">XX_2
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_THNSNN_Phancap", new { area = "DuToan"}), "Dự toán chi ngân sách nhà nước phân cấp")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">Phụ lục in gửi ngành</td>
                    </tr>
                    <%
                            break;
                        //bieu kiem
                        case "2":

                    %>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">1
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_ChiTiet_TongHop"), "Chi tiết Bìa ngân sách")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">2
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1040100_TungDonVi_DenMuc"), "In kiểm ngân sách bảo đảm đến mục")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">3
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_TongHop_KiemKhoBac"), "In kiểm số liêu chi ra kho bạc")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <%--<tr>
                        <td align="center" style="padding: 3px 2px;">4
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_Kiem_TongHop"), "Báo cáo kiểm tổng hợp")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>--%>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">4
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_TongHop_ChiTapTrung"), "Báo cáo kiểm số chi tập trung")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">5
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1010000_ML_TungDonViKiem"), "Báo cáo kiểm số thường xuyên")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <%--<tr>
                        <td align="center" style="padding: 3px 2px;">6
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1020100_TungDonViKiem"), "Báo cáo kiểm số nghiệp vụ")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>--%>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">6_1
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1020100_ML_TungDonViKiem"), "Báo cáo kiểm số nghiệp vụ theo mục lục")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">6_2
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1020100_Tach_TungDonViKiem"), "Báo cáo kiểm số nghiệp vụ (Số đơn vị đề nghị | Số ngành thẩm định | Số ngành phân cấp)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">7
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_XuatEx"), "Báo cáo Xuât excel từng đơn vị")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>


                    <tr>
                        <td align="center" style="padding: 3px 2px;">9
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_K11_M00", new { area = "DuToan"}), "Báo cáo kiểm - Ngành 00 (DTKT - Dự toán)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">9_1
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_K11_M104b", new { area = "DuToan"}), "Báo cáo kiểm các Ngành (DTKT - Dự toán)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">9_2
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_K11_M01", new { area = "DuToan"}), "Báo cáo kiểm tổng hợp- DTKT - Dự toán (Đơn vị hàng ngang)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>


                    <tr>
                        <td align="center" style="padding: 3px 2px;">8
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_K11_NG", new { area = "DuToan"}), "Báo cáo kiểm - Dự toán theo ngành")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">9
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_K11_02b", new { area = "DuToan"}), "    Báo cáo dự toán theo ngành - Tổng hợp đơn vị (Đơn vị hàng dọc - Ngành hàng ngang)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">10
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_K11_03a", new { area = "DuToan"}), "    Báo cáo dự toán chi tiết theo ngành - Tổng hợp tới Mục (Đơn vị hàng dọc - Mục hàng ngang)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">11
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_K11_03b", new { area = "DuToan"}), "    Báo cáo dự toán chi tiết theo ngành - Tổng hợp tới Ngành (Đơn vị hàng dọc - Ngành hàng ngang)")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>

                    <tr>
                        <td align="center" style="padding: 3px 2px;">12
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_K11_03b99", new { area = "DuToan"}), "    Báo cáo dự toán chi tiết theo ngành - Tổng cục kỹ thuật")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>

                    <tr>
                        <td align="center" style="padding: 3px 2px;">13
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_DuToan_SKT", new { area = "DuToan"}), "Báo cáo so sánh dự toán chi tiết và số kiểm tra theo mục lục số kiểm tra")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">14
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_DuToan_SKT_A3", new { area = "DuToan"}), "Báo cáo so sánh dự toán chi tiết và số kiểm tra theo mục lục số kiểm tra biểu A3")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">15
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_Kiem_A3", new { area = "DuToan"}), "Báo cáo số chi tiết dự toán bị âm biểu A3")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">16
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_KiemTH", new { area = "DuToan"}), "Báo cáo số chi tiết dự toán sau khi ngành điều chỉnh")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">17
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_KiemTH_A3", new { area = "DuToan"}), "Báo cáo số chi tiết dự toán sau khi ngành điều chỉnh biểu A3")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">18
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_DuToan_SKTCuc_A3", new { area = "DuToan"}), "Báo cáo tổng hợp dự toán chi tiết và số kiểm tra theo mục lục số kiểm tra")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                     <tr>
                        <td align="center" style="padding: 3px 2px;">18_1
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_DuToan_SKTCucTG", new { area = "DuToan"}), "Báo cáo tổng hợp số tăng và giảm đợt dự toán")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                     <tr>
                        <td align="center" style="padding: 3px 2px;">19
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_KiemNganh"), "Báo cáo kiểm số nghiệp vụ ngành")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">20
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_THNSNN", new { area = "DuToan"}), "Báo cáo kiểm - Ngân sách nhà nước năm trước")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                     <tr>
                        <td align="center" style="padding: 3px 2px;">21
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_KIEM_NN", new { area = "DuToan"}), "Báo cáo kiểm - Ngân sách nhà nước")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">22
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_NN", new { area = "DuToan"}), "Báo cáo kiểm - Ngân sách nhà nước theo BQL ngân sách")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <% break;
                        //bieu trinh ky
                        case "3":
                    %>
                    <%--<tr>
                        <td align="center" style="padding: 3px 2px;">
                            1
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1010000_TungDonVi", new { sTrinhKy="true"}), "Ngân sách thường xuyên-theo từng đơn vị")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;">
                        </td>
                    </tr>--%>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">1
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1010000_ChonTo"), "Ngân sách thường xuyên-tổng hợp")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">2
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1020000_ChonTo"), "Ngân sách nghiệp vụ 00 -tổng hợp -Đơn vị ngang")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">3
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1040100_TungDonVi_TrinhKy"), "Ngân sách bảo đảm")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>

                    <tr>
                        <td align="center" style="padding: 3px 2px;">4
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", " rptDuToan_2_B8_TongHop"), "Nghiệp vụ B8")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>

                    <tr>
                        <td align="center" style="padding: 3px 2px;">5
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", " rptDuToan_1020200_ChonTo"), "Tùy viên Quốc phòng")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>

                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">6
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", " rptDuToan_1050100_ChonTo"), "Nhiệm vụ C")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>

                    <tr>
                        <td align="center" style="padding: 3px 2px;">7
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_1050000_TongHop", new { sTrinhKy=1}), "Doanh nghiệp")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 3px 2px;">8
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(Url.Action("Index", "rptDuToan_TongHop_2060101", new { sTrinhKy = 1 }), "Người có công")%>
                        </td>
                        <td align="center" style="padding: 3px 2px;"></td>
                    </tr>

                    <%break;
                        } %>
                </table>
            </div>
        </div>
    </div>
</asp:Content>
