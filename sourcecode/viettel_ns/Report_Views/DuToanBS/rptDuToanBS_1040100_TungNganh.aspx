<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="VIETTEL.Report_Controllers.DuToan" %>
<%@ Import Namespace="VIETTEL.Models.DuToanBS" %>
<%@ Import Namespace="FlexCel.Core" %>
<%@ Import Namespace="FlexCel.Render" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <%
   
        String ParentID = "BaoCaoNganSachNam";
        int SoCot = 1;
        String MaND = User.Identity.Name;
        String iNamLamViec = ReportModels.LayNamLamViec(MaND);
        String PageLoad = Convert.ToString(ViewData["PageLoad"]);
        String ToSo = Request.QueryString["ToSo"];
        String Nganh = Convert.ToString(ViewData["Nganh"]);
        String sLNS = Convert.ToString(ViewData["sLNS"]);
        String iID_MaPhongBan = Convert.ToString(ViewData["iID_MaPhongBan"]);

        if (String.IsNullOrEmpty(sLNS))
        {
            sLNS = "1040100";
        }
        if (String.IsNullOrEmpty(iID_MaPhongBan))
        {
            iID_MaPhongBan = "0";
        }
        if (String.IsNullOrEmpty(ToSo))
        {
            ToSo = "1";
        }


        //Nguon bao dam

        DataTable dtNguonBD = DuToan_ReportModels.dtNguonInBaoDam();
        SelectOptionList slNguonBD = new SelectOptionList(dtNguonBD, "iID", "sTen");
        dtNguonBD.Dispose();

        //dot
        String iID_MaDot = Convert.ToString(ViewData["iID_MaDot"]);
        DataTable dtDot = DuToanBS_ReportModels.LayDSDot(iNamLamViec, MaND, sLNS);
        SelectOptionList slDot = new SelectOptionList(dtDot, "iID_MaDot", "iID_MaDot");

        if (String.IsNullOrEmpty(iID_MaDot))
        {
            if (dtDot.Rows.Count > 0)
                iID_MaDot = Convert.ToString(dtDot.Rows[0]["iID_MaDot"]);
            else
                iID_MaDot = Guid.Empty.ToString();
        }
        dtDot.Dispose();

        DataTable dtNganh = DuToanBS_ReportModels.dtNganh(iID_MaDot, MaND, "1");
        SelectOptionList slNganh = new SelectOptionList(dtNganh, "iID", "sTenNganh");
        if (String.IsNullOrEmpty(Nganh))
        {
            if (dtNganh.Rows.Count > 0)
            {
                Nganh = Convert.ToString(dtNganh.Rows[0]["iID"]);
            }
            else
            {
                Nganh = "-1";
            }
        }
        //B dich
        DataTable dtBDich = DuToan_ReportModels.dtPhongBanInBaoDamBS(iID_MaDot, Nganh, MaND);
        SelectOptionList slBDich = new SelectOptionList(dtBDich, "iID_MaPhongBan", "iID_MaPhongBan");
        dtBDich.Dispose();
        DataTable dtTo = rptDuToanBS_1040100_TungNganhController.DanhSachToIn(MaND, Nganh, ToSo, sLNS, iID_MaPhongBan, iID_MaDot);
        String MaTo = Convert.ToString(ViewData["MaTo"]);
        String[] arrMaDonVi = MaTo.Split(',');
        String[] arrMaTo = MaTo.Split(',');
        String Chuoi = "";
        String[] arrView = new String[arrMaTo.Length];
        if (String.IsNullOrEmpty(PageLoad))
            PageLoad = "0";
        if (String.IsNullOrEmpty(MaTo)) PageLoad = "0";
        if (PageLoad == "1")
        {
            for (int i = 0; i < arrMaTo.Length; i++)
            {
                arrView[i] =
                    String.Format(
                        @"/rptDuToanBS_1040100_TungNganh/viewpdf?ToSo={0}&MaND={1}&Nganh={2}&sLNS={3}&iID_MaPhongBan={4}&iID_MaDot={5}",
                        arrMaTo[i], MaND, Nganh, sLNS, iID_MaPhongBan, iID_MaDot);
                Chuoi += arrView[i];
                if (i < arrMaTo.Length - 1)
                    Chuoi += "+";
            }

        }

        String BackURL = Url.Action("Index", "DuToanBS_Report", new { sLoai = "1" });
        using (Html.BeginForm("EditSubmit", "rptDuToanBS_1040100_TungNganh", new { ParentID = ParentID }))
        {
    %>
    <div class="box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Báo cáo dự toán chi ngân sách quốc phòng năm
                            <%=iNamLamViec%>
                            -Phần phân cấp </span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="Div1">
            <div id="Div2">
                <table cellpadding="0" cellspacing="0" border="0" width="100%">
                    <tr>
                        <td style="width: 10%" class="td_form2_td1">
                            <b>Chọn đợt: </b>
                        </td>
                        <td class="td_form2_td1" style="text-align: center; width: 30%">
                            <div>
                                <%=MyHtmlHelper.DropDownList(ParentID, slDot, iID_MaDot, "iID_MaDot", "", "class=\"input1_2\" style=\"width: 40%;height:24px;\" onchange=Nganh()")%>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="width: 10%">
                            <div>
                                Chọn tờ:</div>
                        </td>
                        <td style="width: 30%" rowspan="4">
                            <div id="<%= ParentID %>_tdDonVi" style="overflow: scroll; height: 200px">
                            </div>
                        </td>
                        <td class="td_form2_td1">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 10%" class="td_form2_td1">
                            <b>Chọn ngành: </b>
                        </td>
                        <td class="td_form2_td1" style="text-align: center; width: 30%">
                            <div id="<%=ParentID %>_tdNganh">
                                <%-- <%=MyHtmlHelper.DropDownList(ParentID, slNganh, Nganh, "Nganh", "", "class=\"input1_2\" style=\"width: 40%;height:24px;\" onchange=Chon()")%>--%>
                            </div>
                        </td>
                        <td class="td_form2_td1">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 10%" class="td_form2_td1">
                            <b>Chọn phòng ban: </b>
                        </td>
                        <td class="td_form2_td1" style="text-align: center; width: 30%">
                            <div id="<%=ParentID %>_tdPhongBan">
                                <%--<%=MyHtmlHelper.DropDownList(ParentID, slBDich, iID_MaPhongBan, "iID_MaPhongBan", "", "class=\"input1_2\" style=\"width: 40%;height:24px;\" onchange=Chon()")%>--%>
                            </div>
                        </td>
                        <td class="td_form2_td1">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 10%" class="td_form2_td1">
                            <b>Chọn nguồn: </b>
                        </td>
                        <td class="td_form2_td1" style="text-align: center; width: 30%">
                            <div>
                                <%=MyHtmlHelper.DropDownList(ParentID, slNguonBD, sLNS, "sLNS", "", "class=\"input1_2\" style=\"width: 40%;height:24px;\" onchange=Chon()")%>
                            </div>
                        </td>
                        <td class="td_form2_td1">
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5">
                            <div style="margin-top: 10px;">
                                <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                    <tr>
                                        <td style="width: 40%">
                                        </td>
                                        <td align="right">
                                            <input type="submit" class="button" value="Tiếp tục" />
                                        </td>
                                        <td style="width: 1%">
                                            &nbsp;
                                        </td>
                                        <td align="left">
                                            <input type="button" class="button" value="Hủy" onclick="Huy()" />
                                        </td>
                                        <td style="width: 40%">
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <script type="text/javascript">
               
                var count = <%=arrView.Length%>;
                var Chuoi = '<%=Chuoi%>';
                var Mang=Chuoi.split("+");
                   var pageLoad = <%=PageLoad %>;
                   if(pageLoad=="1") {
                var siteArray = new Array(count);
                for (var i = 0; i < count; i++) {
                    siteArray[i] = Mang[i];
                }
                    for (var i = 0; i < count; i++) {
                        window.open(siteArray[i], '_blank');
                    }
                } 
         
               function CheckAllTO(value) {
         $("input:checkbox[check-group='To']").each(function (i) {
             this.checked = value;
         });
     }         
                Nganh();
                function Nganh(){
                 var iID_MaDot = document.getElementById("<%=ParentID %>_iID_MaDot").value;
                    jQuery.ajaxSetup({ cache: false });
                    var url = unescape('<%= Url.Action("Ds_Nganh?ParentID=#0&Nganh=#1&iID_MaDot=#2", "rptDuToanBS_1040100_TungNganh") %>');
                url = unescape(url.replace("#0", "<%= ParentID %>"));
                url = unescape(url.replace("#1","<%= Nganh %>"));
                url = unescape(url.replace("#2",iID_MaDot));
                $.getJSON(url, function (data) {
                    document.getElementById("<%= ParentID %>_tdNganh").innerHTML = data;
                    PhongBan();
                });
                }
                function PhongBan(){
                 var Nganh = document.getElementById("<%=ParentID %>_Nganh").value;
                  var iID_MaDot = document.getElementById("<%=ParentID %>_iID_MaDot").value;
                   var url = unescape('<%= Url.Action("Ds_PhongBan?ParentID=#0&Nganh=#1&iID_MaDot=#2&iID_MaPhongBan=#3", "rptDuToanBS_1040100_TungNganh") %>');
                url = unescape(url.replace("#0", "<%= ParentID %>"));
                url = unescape(url.replace("#1",Nganh));
                url = unescape(url.replace("#2",iID_MaDot));
                url = unescape(url.replace("#3","<%= iID_MaPhongBan %>"));
                 $.getJSON(url, function (data) {
                    document.getElementById("<%= ParentID %>_tdPhongBan").innerHTML = data;
                   Chon();
                });
                }
            function Chon() {
                 var Nganh = document.getElementById("<%=ParentID %>_Nganh").value;
                    var sLNS = document.getElementById("<%=ParentID %>_sLNS").value;
                       var iID_MaPhongBan = document.getElementById("<%=ParentID %>_iID_MaPhongBan").value;
                       var iID_MaDot = document.getElementById("<%=ParentID %>_iID_MaDot").value;
                jQuery.ajaxSetup({ cache: false });
                var url = unescape('<%= Url.Action("Ds_DonVi?ParentID=#0&Nganh=#1&ToSo=#2&sLNS=#3&iID_MaPhongBan=#4&iID_MaDot=#5", "rptDuToanBS_1040100_TungNganh") %>');
                url = unescape(url.replace("#0", "<%= ParentID %>"));
                url = unescape(url.replace("#1", Nganh));
                url = unescape(url.replace("#2","<%= MaTo %>"));
                url = unescape(url.replace("#3",sLNS));
                url = unescape(url.replace("#4",iID_MaPhongBan));
                url = unescape(url.replace("#5",iID_MaDot));
                $.getJSON(url, function (data) {
                    document.getElementById("<%= ParentID %>_tdDonVi").innerHTML = data;
                });
            }
                    </script>
                </table>
            </div>
        </div>
        <script type="text/javascript">
            function Huy() {
                window.location.href = '<%=BackURL%>';
            }
        </script>
    </div>
    <%}
        
    %>
    <div>
        <%=MyHtmlHelper.ActionLink(Url.Action("ExportToExcel", "rptDuToanBS_1040100_TungNganh", new { MaND = MaND, Nganh = Nganh, ToSo = MaTo, iID_MaPhongBan = iID_MaPhongBan, sLNS = sLNS, iID_MaDot = iID_MaDot }), "Xuất ra Excels")%>
    </div>
</body>
</html>
