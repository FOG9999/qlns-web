<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%

        var items = new List<dynamic>()
        {
            new {
                name = "Thu nộp",
                url = Url.Action("Index", "rptDuToan_ThuNop_TongHop", new { trinhky = 1}),
                status = "XONG",
                code = "Thu"},

            new {
                name = "Ngân sách bảo đảm - Tổng hợp",
                url = Url.Action("Index", "rptDuToan_1040100_TungDonVi_TrinhKy", new { trinhky = 1}),
                status = "XONG",
                code = "1040100"},
             new {
                name = "Ngân sách bảo đảm - Phân cấp",
                url = Url.Action("Index", "rptDuToan_1040100_TungNganh"),
                status = "",
                code = "1040100-Pc"},

            new {
                name = "Thường xuyên",
                url = Url.Action("Index", "rptDuToan_1010000_ChonTo"),
                status = "XONG",
                code = "101"},

            new {
                name = "Nghiệp vụ 00",
                url = Url.Action("Index", "rptDuToan_1020000_ChonTo"),
                status = "XONG",
                code = "102"},


            new {
                name = "Kinh phí nghiệp vụ các ngành",
                url = Url.Action("Index", "rptDuToan_1020100_ChonTo"),
                status = "XONG",
                code = "1020100"},


              //new {
              //  name = "Phần phân cấp ngân sách bảo đảm toàn quân   ",
              //  url = Url.Action("Index", "rptDuToan_1040100_TungNganh"),
              //  status = "",
              //  code = "104-Nganh"},

            //new {
            //    name =  "Tùy viên Quốc phòng",
            //    url = Url.Action("Index", "rptDuToan_1020200_ChonTo"),
            //    status = "XONG",
            //    code = "1020200"},

            new {
                name =  "Nghiệp vụ ngành kỹ thuật",
                url = Url.Action("Index", "rptDuToan_1020100_KT_TungDonVi", new { trinhky = 1}),
                status = "XONG",
                code = "1020100-KT"},

            new {
                name =  "Tùy viên Quốc phòng",
                url = Url.Action("Index", "rptDuToan_1020200_TungDonVi", new { trinhky = 1}),
                status = "XONG",
                code = "1020200"},

             new {
                name = "Chi ngân sách xây dựng cơ bản", url =
                Url.Action("Index", "rptDuToan_103_ChonTo", new { area = "DuToan"}),
                status = "XONG",
                code = "103"},


             //new {
             //   name =  "Dự toán chi việc nhà nước giao tính vào NSQP - donvi",
             //   url = Url.Action("Index", "rptDuToan_109_TungDonVi", new { loai = "trinhky"}),
             //   code = "109"},

             new {
                name =  "Ngân sách NN giao tính và NSQP - Tổng hợp",
                url = Url.Action("Index", "rptDuToan_109_TongHop", new { trinhky = 1}),
                status = "XONG",
                code = "109-Th"},

              new {
                name =  "Ngân sách NN giao tính và NSQP - Chi tiết",
                url = Url.Action("Index", "rptDuToan_109_ChonTo"),
                status = "",
                code = "109-Ct"},

              new {
                name =  "Ngân sách NN giao tính và NSQP - Phân cấp",
                url = Url.Action("Index", "rptDuToan_109_PhanCap", new { trinhky = 1}),
                status = "XONG",
                code = "109-Pc"},


             new {
                name =  "Ngân sách nhà nước - Tổng hợp",
                url = Url.Action("Index", "rptDuToan_2_TongHop", new {  trinhky = 1 }),
                status = "XONG",
                code = "2-Th"},

              //new {
              //  name =  "Dự toán chi ngân sách nhà nước giao",
              //  url = Url.Action("Index", "rptDuToan_2_TungDonVi", new {  trinhky = 1  }),
              //  status = "XONG",
              //  code = "2"},

               new {
                name =  "Dự toán chi việc nhà nước - chi QLHC",
                url = Url.Action("Index", "rptDuToan_207_TungDonVi", new { trinhky = 1}),
                status = "XONG",
                code = "207-Dv"},


               new {
                name =  "Dự toán chi việc nhà nước - chi QLHC - Phân cấp",
                url = Url.Action("Index", "rptDuToan_207_TungNganh", new { trinhky = 1}),
                status = "",
                code = "207-Ng"},


            new {
                name =  "Người có công",
                url = Url.Action("Index", "rptDuToan_TongHop_2060101", new { trinhky = 1 }),
                status = "XONG",
                code = "2060101"},

            new {
                name =  "Nhiệm vụ C",
                url = Url.Action("Index", "rptDuToan_1050100_ChonTo", new { trinhky = 1}),
                status = "",
                code = "105-C"},
            new {
                name =  "Doanh nghiệp (B6)",
                url = Url.Action("Index", "rptDuToan_1050000_TongHop", new { trinhky = 1}),
                status = "",
                code = "1050000"},

            new {
                name =  "Dự toán kinh phí khác",
                url = Url.Action("Index", "rptDuToan_4_TongHop", new { trinhky=1}),
                status = "",
                code = "4"},

            //new {
            //    name = "Nghiệp vụ B8", url =
            //    Url.Action("Index", "rptDuToan_2_B8_TongHop", new { trinhky = 1, iID_MaPhongBan = "08"}),
            //    status = "XONG",
            //    code = "2-B8"},
            
           new {
                name = "Nghiệp vụ B8 - Bảo hiểm y tế - Chọn tờ", url =
                Url.Action("Index", "rptDuToan_1020700_ChonTo", new { area = "DuToan"}),
                status = "XONG",
                code = "1020700"},
             //new {
             //   name = "Nghiệp vụ B8 - Bảo hiểm y tế - Từng đơn vị", url =
             //   Url.Action("Index", "rptDuToan_1010100_TungDonVi", new { }),
             //   status = "XONG",
             //   code = "BHYT"},

           new {
                name = "Chi nghiên cứu khoa học nền", url =
                Url.Action("Index", "rptDuToan_1020800_ChonTo", new { area = "DuToan"}),
                status = "XONG",
                code = "1020800"},
           new {
                name = "Chi tập trung tại BQP", url =
                Url.Action("Index", "rptDuToan_101_ChonTo", new { area = "DuToan"}),
                status = "XONG",
                code = "101"},

        };

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
                        <th style="width: 420px;">Tên báo cáo
                        </th>
                        <%-- <th style="width: 12%;">
                            LNS
                        </th>--%>
                        <th style="width: 120px">Ghi chú
                        </th>
                        <th></th>
                    </tr>

                    <%
                        for (int i = 0; i < items.Count; i++)
                        {
                            var item = items[i];
                            var color = i % 2 == 0 ? "" : "#f1f1f1";
                    %>

                    <tr style="background-color: <%=color%>">
                        <td align="center" style="padding: 3px 2px;">
                            <% = i + 1 %>
                        </td>
                        <td style="padding: 3px 2px;">
                            <%=MyHtmlHelper.ActionLink(item.url, item.name)%>
                        </td>
                        <%-- <td align="center" >
                            <%=item.status %>
                        </td>--%>
                        <td align="center" style="padding: 3px 2px; text-align: left">
                            <%--   <%=item.code %>--%>
                            <a href="<%=item.url %>" style="color: black; text-decoration: none; font: 8px;" target="_blank"><%=item.code %></a>
                        </td>
                        <td></td>
                    </tr>
                    <%
                        }
                    %>
                </table>
            </div>
        </div>
    </div>
</asp:Content>
