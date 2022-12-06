<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site_ls.Master" Inherits="System.Web.Mvc.ViewPage" %>



<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%

        var items = new List<dynamic>()
        {
           //new {
           //     id="",
           //     name = "DTBS - 1040100 theo đơn vị (Tờ bìa)",
           //     url = Url.Action("Index", "rptDuToanBS_1040100_TungDonVi"),
           //     note = ""},

           new {
                id="",
                name = "DTBS - 1040100 theo đơn vị - Tờ bìa (trình ký)",
                url = Url.Action("Index", "rptDuToanBS_1040100_ToBia"),
                note = ""},

           new {
                id="",
                name = "DTBS - 1040100 theo ngành - Chọn tờ (trình ký)",
                url = Url.Action("Index", "rptDuToanBS_1040100_Nganh"),
                note = ""},

           //new {
           //     id="",
           //     name = "DTBS - 1040100 theo ngành - Chọn tờ (trình ký)",
           //     url = Url.Action("Index", "rptDuToanBS_1040100_TungNganh"),
           //     note = ""},

           new {
                id="",
                name = "DTBS - 1040100 theo đợt",
                url = Url.Action("Index", "rptDuToanBS_1040100_Dot"),
                note = ""},
            new {
                id="",
                name = "DTBS - Báo cáo bảng kiểm số liệu phân cấp",
                url = Url.Action("Index", "rptDuToanBS_PhanCap"),
                note = ""},


            new {
                id="",
                name = "BTBS - 207 Chi QLHC theo đơn vị - Tờ bìa",
                url = Url.Action("Index", "rptDuToanBS_207_ToBia"),
                note = ""},
             new {
                id="",
                name = "BTBS - 207 Chi QLHC theo ngành - Chọn tờ",
                url = Url.Action("Index", "rptDuToanBS_207_Nganh"),
                note = ""},


            new {
                id="",
                name = "DTBS - Thông báo dự toán cho đơn vị - Phụ lục QĐ",
                url = Url.Action("Index", "rptDuToanBS_ChiTieuNganSach"),
                note = ""},
            new {
                id="",
                name = "DTBS - Thông báo dự toán cho đơn vị - Trình ký",
                url = Url.Action("Index", "rptDuToanBS_ChiTieuNganSach", new { trinhky = 1}),
                note = ""},



             new {
                id="",
                name = "Biểu tổng hợp chỉ tiêu đơn vị",
                url = Url.Action("Index", "rptTongHop_ChiTieu_DonVi"),
                note = ""},

        };


    %>

    <div class="">
        <h5 class="text-uppercase">Danh sách báo cáo DTBS</h5>
    </div>

    <table class="table table-bordered">
        <thead>
            <th>#</th>
            <th>ID</th>
            <th>Tên báo cáo</th>
            <th>Ghi chú</th>
        </thead>
        <tbody>

            <%
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var color = i % 2 == 0 ? "" : "#f1f1f1";
            %>

            <tr style="background-color: <%=color%>">
                <td><%= i+ 1 %></td>
                <td><%= item.id %></td>
                <td>
                    <a href="<%= item.url %>" style="font-size: 14px;"><%= item.name %></a>
                </td>
                <td><%= item.note %></td>
            </tr>
            <%
                }
            %>
        </tbody>
    </table>

</asp:Content>

