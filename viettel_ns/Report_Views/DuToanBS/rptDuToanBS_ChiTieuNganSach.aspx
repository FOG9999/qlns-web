<%@ Page Language="C#"
    MasterPageFile="~/Views/Shared/Site_ls.Master"
    Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%
        var Modelx = (rptDuToanBS_ChiTieuNganSachViewModel)Model;
    %>

    <div class="panel panel-danger">
        <div class="panel-heading">
            <h5 class="text-uppercase">Báo cáo so sánh chỉ tiêu năm <%= Modelx.iNamLamViec%></h5>
        </div>
        <div class="panel-body">

            <div class="container">
                <div class="row">
                    <div class="col-xs-6">
                        <h6></h6>
                    </div>
                    <div class="col-xs-6">
                        <h6><strong class="text-uppercase">Đơn vị</strong></h6>
                    </div>
                    <%--  <div class="col-xs-4">
                        <h6><strong class="text-uppercase">LNS</strong></h6>
                    </div>--%>
                </div>
                <div class="row">
                    <div class="col-xs-6">
                        <form action="" class="form-horizontal">

                            <div class="form-group">
                                <label class="control-label col-sm-4">Đợt bổ sung NS:</label>
                                <div class="col-sm-8">
                                    <%= Html.DropDownList("iID_MaDot", Modelx.DotList, new { @class="form-control", onchange="changeDot()"}) %>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-4">Phòng ban:</label>
                                <div class="col-sm-8">
                                    <%= Html.DropDownList("iID_MaPhongBan", Modelx.PhongBanList, new { @class="form-control", onchange="changeDot()"}) %>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-4">Tên phụ lục:</label>
                                <div class="col-sm-8">
                                    <%= Html.TextArea("tenPhuLuc", Modelx.TieuDe, new { @class = "form-control" }) %>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-4">Quyết định:</label>
                                <div class="col-sm-8">
                                    <%= Html.TextArea("quyetDinh", Modelx.QuyetDinh, new { @class = "form-control" }) %>
                                </div>
                            </div>
                        </form>
                    </div>

                    <div class="col-xs-6">
                        <div id="iID_MaDonVi" class="ls-box"></div>
                    </div>
                    <%-- <div class="col-xs-4">
                        <div id="sLNS" class="ls-box"></div>
                    </div>--%>
                </div>

                <hr />
                <div class="row text-center">
                    <a class="btn-mvc btn-mvc-green btn-mvc-large btn-print" data-ext="pdf"><i class="fa fa-print"></i>In báo cáo</a>
                    <a class="btn-mvc btn-mvc-green btn-mvc-large btn-print" data-ext="xls"><i class="fa fa-file-excel"></i>Xuất excel</a>
                    <a class="btn btn-default" id="btn-cancel" href="<%=Url.Action("Index", "DuToanBS_Report") %>">Hủy</a>
                </div>
            </div>

        </div>
    </div>

    <script type="text/javascript">

        function changeDot() {

            // clear
            $("#iID_DonVi").empty();
            $("#sLNS").empty();

            var iID_MaDot = $("#iID_MaDot").val();
            var iID_MaPhongBan = $("#iID_MaPhongBan").val();
            if (iID_MaDot == "-1")
                return;

            fillDonVi(iID_MaDot, iID_MaPhongBan);
        }


        function fillDonVi(iID_MaDot, iID_MaPhongBan) {
            var url = '<%=Url.Action("Ds_DonVi")%>' +
                       "/?iID_MaDot=" + iID_MaDot +
                       "&iID_MaPhongBan=" + iID_MaPhongBan;

            console.log(url);

            fillCheckboxList("iID_MaDonVi", "DonVi", url);
        }

       <%-- function fillLNS() {
            // lay ds don vi duoc chon
            var url = '<%=Url.Action("Ds_LNS")%>' +
                        "/?iID_MaDot=" + $("#iID_MaDot").val() +
                        "&iID_MaPhongBan=" + $("#iID_MaPhongBan").val() +
                        "&iID_MaDonVi=" + getCheckedItems("DonVi");

            console.log(url);

            $.getJSON(url, function (data) {
                document.getElementById("sLNS").innerHTML = data;
                checkFirstItem("LNS");
            });
        }

        function checkItem(item) {
            var group = $(item).data("group");
            if (group == "DonVi") {
                fillLNS();
            }
        }

        function changeListAll(group) {
            if (group == "DonVi") {
                fillLNS();
            }
        }--%>

        $(".btn-print").click(function () {

            var links = [];
            var ext = $(this).data("ext");

            $("input:checkbox[check-group='DonVi']").each(function () {
                if (this.checked) {
                    var item = this.value;
                    var url = '<%=Url.Action("Print", "rptDuToanBS_ChiTieuNganSach") %>' +
                        "?ext=" + ext +
                        "&iID_MaDot=" + $("#iID_MaDot").val() +
                        "&iID_MaPhongBan=" + $("#iID_MaPhongBan").val() +
                        "&iID_MaDonVi=" + item +
                        //"&sLNS=" + getCheckedItems("LNS") +
                        "&tenPhuLuc=" + $("#tenPhuLuc").val() +
                        "&quyetDinh=" + $("#quyetDinh").val() +
                        "&trinhky=" + '<%= Modelx.TrinhKy%>';

                    url = unescape(url);
                    links.push(url);
                }
            });

            openLinks(links);
        });

    </script>

</asp:Content>


