<%@ Page Language="C#"
    MasterPageFile="~/Views/Shared/Site_ls.Master"
    Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%
        var Modelx = (rptQuyetToanNam_2A_ViewModel)Model;
    %>

    <div class="panel panel-danger">
        <div class="panel-heading">
            <h5 class="text-uppercase">Báo cáo kết luận quyết toán ngân sách năm <%= Modelx.iNamLamViec%> - Biểu QTN_2A</h5>
        </div>
        <div class="panel-body">

            <div class="container">
                <div class="row">
                    <form action="" class="form-horizontal">

                        <div class="form-group">
                            <label class="control-label col-sm-4">Phòng ban:</label>
                            <div class="col-sm-8">
                                <%= Html.DropDownList("iID_MaPhongBan", Modelx.PhongBanList, new { @class="form-control", onchange="changePhongBan()"}) %>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="control-label col-sm-4">Năm ngân sách:</label>
                            <div class="col-sm-8">
                                <%= Html.DropDownList("iID_MaNamNganSach", Modelx.NamNganSachList, new { @class = "form-control" }) %>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="control-label col-sm-4">Đơn vị</label>
                            <div class="col-sm-8">
                                <select id="iID_MaDonVi" class="form-control"></select>
                                <hr />

                            </div>
                        </div>

                    </form>
                    <div class="row text-center">
                        <a class="btn-mvc btn-mvc-green btn-print" data-ext="pdf"><i class="fa fa-print"></i>In báo cáo</a>
                        <a class="btn-mvc btn-mvc-green btn-print" data-ext="xls"><i class="fa fa-file-excel"></i>Xuất excel</a>
                        <a class="btn btn-default" id="btn-cancel" href="<%=Url.Action("Index", "QuyetToan_Report") %>">Hủy</a>
                    </div>
                </div>

            </div>
        </div>

        <script type="text/javascript">

            // fill the first time
            //changeData();

            // thay doi lua chon
            function changePhongBan() {

                var iID_MaPhongBan = $("#iID_MaPhongBan").val();

                var url = '<%=Url.Action("Ds_DonVi")%>' + "/?"
                        + "iID_MaPhongBan=" + iID_MaPhongBan;


                jQuery.ajaxSetup({ cache: false });

                $.getJSON(url, function (data) {
                    document.getElementById("iID_MaDonVi").innerHTML = data;
                });
            }

            $(".btn-print").click(function () {

                var ext = $(this).data("ext");

                var url = '<%=Url.Action("Print", "rptQuyetToanNam_2a1") %>' +
                            "/?ext=" + ext +
                            "&iID_MaDonVi=" + $("#iID_MaDonVi").val() +
                            "&iID_MaNamNganSach=" + $("#iID_MaNamNganSach").val() +
                            "&iID_MaPhongBan=" + $("#iID_MaPhongBan").val();
                url = unescape(url);
                window.open(url, "_blank");

            });

        </script>
</asp:Content>


