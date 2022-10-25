<%@ Page Language="C#"
    MasterPageFile="~/Views/Shared/Site_ls.Master"
    Inherits="System.Web.Mvc.ViewPage" %>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%
        var Modelx = (rptDuToanBS_207_NganhViewModel)Model;
    %>

    <div class="panel panel-danger">
        <div class="panel-heading">
            <h5 class="text-uppercase">Biểu giao bổ sung dự toán NSBĐ theo ngành năm <%= Modelx.iNamLamViec%> - Phần chi QLHC</h5>
        </div>
        <div class="panel-body">

            <div class="container">
                <!-- header -->
                <div>
                    <div class="col-xs-6">
                        <h6></h6>
                    </div>
                    <div class="col-xs-6">
                        <h6><strong class="text-uppercase">Chọn tờ:</strong></h6>
                    </div>
                </div>
                <div class="row">
                    <div class="col-xs-6">
                        <form action="" class="form-horizontal">

                            <div class="form-group">
                                <label class="control-label col-sm-4">Đợt bổ sung NSNN:</label>
                                <div class="col-sm-8">
                                    <%= Html.DropDownList("iID_MaDot", Modelx.DotList, new { @class="form-control", onchange="changeDot()"}) %>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-4">Chọn ngành:</label>
                                <div class="col-sm-8">
                                    <div>
                                        <select id="iID_MaNganh" class="form-control" onchange="change()"></select>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-4">Chọn phòng ban:</label>
                                <div class="col-sm-8">
                                        <%--<select id="iID_MaPhongBan" class="form-control" onchange="change()"></select>--%>
                                    <%= Html.DropDownList("iID_MaPhongBan", Modelx.PhongList, new { @class="form-control", onchange="change()"}) %>

                                </div>
                            </div>

                        </form>
                    </div>

                    <div class="col-xs-6">
                        <div id="iID_MaTo" class="ls-box"></div>
                    </div>
                </div>

                <hr />
                <div class="row text-center">
                    <a class="btn-mvc btn-mvc-green btn-mvc-large btn-print" data-ext="pdf"><i class="fa fa-print"></i>  In báo cáo</a>
                    <a class="btn-mvc btn-mvc-green btn-mvc-large btn-print" data-ext="xls"><i class="fa fa-file-excel"></i>  Xuất excel</a>
                    <a class="btn btn-default" id="btn-cancel" href="<%=Url.Action("Index", "DuToanBS_Report") %>">Hủy</a>
                </div>
            </div>

        </div>
    </div>

    <script type="text/javascript">

        // thay doi lua chon
        function changeDot() {
            var iID_MaDot = $("#iID_MaDot").val();
            fillNganh(iID_MaDot);
        }

        function fillNganh(iID_MaDot) {
            var url = '<%=Url.Action("Ds_Nganh")%>' + "/?"
                        + "iID_MaDot=" + iID_MaDot;
            jQuery.ajaxSetup({ cache: false });

            $.getJSON(url, function (data) {
                document.getElementById("iID_MaNganh").innerHTML = data;

                //fillPhongBan(iID_MaDot, $("#iID_MaNganh").val());

                change();
            });
        }
        function fillPhongBan(iID_MaDot, iID_MaNganh) {

            var url = '<%=Url.Action("Ds_PhongBan")%>' + "/?"
                        + "iID_MaDot=" + iID_MaDot
                        + "&iID_MaNganh=" + iID_MaNganh;
            jQuery.ajaxSetup({ cache: false });


            $.getJSON(url, function (data) {
                document.getElementById("iID_MaPhongBan").innerHTML = data;
                change();
            });
        }
        

        function change() {

            var url = '<%=Url.Action("Ds_ToIn")%>' + "/?"
                        + "iID_MaDot=" + $("#iID_MaDot").val()
                        + "&iID_MaNganh=" + $("#iID_MaNganh").val()
                        + "&iID_MaPhongBan=" + $("#iID_MaPhongBan").val();

            $.getJSON(url, function (data) {
                document.getElementById("iID_MaTo").innerHTML = data;

                checkFirstItem("To");
            });

        }

        $(".btn-print").click(function () {
                       
            var links = [];
            var ext = $(this).data("ext");

            $("input:checkbox[check-group='To']").each(function () {
                if (this.checked) {
                    var item = this.value;
                    var url = '<%=Url.Action("Print", "rptDuToanBS_207_Nganh") %>' +
                        "?ext=" + ext +
                        "&toSo=" + item +
                        "&iID_MaDot=" + $("#iID_MaDot").val() +
                        "&iID_MaNganh=" + $("#iID_MaNganh").val() +
                        "&iID_MaPhongBan=" + $("#iID_MaPhongBan").val();
                    url = unescape(url);

                    links.push(url);
                }
            });

            openLinks(links);
        });
        
    </script>

</asp:Content>


