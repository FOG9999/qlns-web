﻿<%@ Page Language="C#"
    MasterPageFile="~/Views/Shared/Site_ls.Master"
    Inherits="System.Web.Mvc.ViewPage" %>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%
        var Modelx = (rptDuToanBS_1040100_ToBiaViewModel)Model;
    %>

    <div class="panel panel-danger">
        <div class="panel-heading">
            <h5 class="text-uppercase">Biểu giao bổ sung dự toán ngân sách bảo đảm theo ngành năm <%= Modelx.iNamLamViec%></h5>
        </div>
        <div class="panel-body">

            <div class="container">

                <div class="row">
                    <div class="col-xs-6">
                    </div>
                    <div class="col-xs-6">
                        <h6><strong class="text-uppercase">Ngành</strong></h6>
                    </div>
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

                        </form>
                    </div>
                    <div class="col-xs-6">
                        <div id="iID_MaNganh" class="ls-box" data-check="first-only"></div>
                    </div>
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

        // thay doi lua chon
        function changeDot() {
            var iID_MaDot = $("#iID_MaDot").val();
            fillNganh(iID_MaDot);
        }

        // dien danh sach don vi
        function fillNganh(iID_MaDot) {
            var url = '<%=Url.Action("Ds_Nganh")%>' + "/?"
                        + "iID_MaDot=" + iID_MaDot;
            fillCheckboxList("iID_MaNganh", "Nganh", url);
        }

        $(".btn-print").click(function () {

            var links = [];

            var ext = $(this).data("ext");
            $("input:checkbox[check-group='Nganh']").each(function () {
                if (this.checked) {
                    var item = this.value;
                    var url = unescape('<%=Url.Action("Print", "rptDuToanBS_1040100_ToBia") %>' +
                        "?ext=" + ext +
                        "&iID_MaNganh=" + item +
                        "&iID_MaDot=" + $("#iID_MaDot").val());

                    links.push(url);
                }
            });

            openLinks(links);
        });

    </script>

</asp:Content>


