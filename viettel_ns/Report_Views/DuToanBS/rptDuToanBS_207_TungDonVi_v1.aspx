<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="VIETTEL.Report_Controllers.ThuNop" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
</head>
<body>
    <%
        String ParentID = "DuToan";
        int SoCot = 1;
        //dt Loại ngân sách
        String MaND = User.Identity.Name;
        String iNamLamViec = ReportModels.LayNamLamViec(MaND);
        String iID_MaDonVi = Convert.ToString(ViewData["iID_MaDonVi"]);
        DataTable dtDonVi = DuToanBS_ReportModels.dtNganh_NhaNuoc(MaND, "207");

        if (String.IsNullOrEmpty(iID_MaDonVi))
        {
            if (dtDonVi.Rows.Count > 0)
            {
                iID_MaDonVi = Convert.ToString(dtDonVi.Rows[0]["iID_MaNganh"]);
            }
            else
            {
                iID_MaDonVi = Guid.Empty.ToString();
            }
        }
        dtDonVi.Dispose();

        String iID_MaDot = Convert.ToString(ViewData["iID_MaDot"]);
        DataTable dtDot = DuToanBS_ReportModels.LayDSDot(iNamLamViec, MaND, "207");
        SelectOptionList slDot = new SelectOptionList(dtDot, "iID_MaDot", "iID_MaDot");

        if (String.IsNullOrEmpty(iID_MaDot))
        {
            if (dtDot.Rows.Count > 0)
                iID_MaDot = Convert.ToString(dtDot.Rows[0]["iID_MaDot"]);
            else
                iID_MaDot = Guid.Empty.ToString();
        }
        dtDot.Dispose();
        String[] arrMaDonVi = iID_MaDonVi.Split(',');
        String[] arrView = new String[arrMaDonVi.Length];
        String Chuoi = "";
        String PageLoad = Convert.ToString(ViewData["PageLoad"]);
        if (String.IsNullOrEmpty(PageLoad))
            PageLoad = "0";
        if (String.IsNullOrEmpty(iID_MaDonVi)) PageLoad = "0";
        if (PageLoad == "1")
        {

            for (int i = 0; i < arrMaDonVi.Length; i++)
            {
                arrView[i] =
                    String.Format(
                        @"/rptDuToanBS_207_TungDonVi/viewpdf?iID_MaDonVi={0}&MaND={1}&iID_MaDot={2}",
                        arrMaDonVi[i], MaND,iID_MaDot);
                Chuoi += arrView[i];
                if (i < arrMaDonVi.Length - 1)
                    Chuoi += "+";
            }

        }

        String BackURL = Url.Action("Index", "DuToan_Report", new { sLoai = "1" });
        using (Html.BeginForm("EditSubmit", "rptDuToanBS_207_TungDonVi", new { ParentID = ParentID }))
        {
    %>
    <div class="title_tong">
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
            <tr>
                <td>
                    <span>Báo cáo dự toán chi ngân sách quốc phòng (phần chi quản lý hành chính) năm
                        <%=iNamLamViec %>
                    </span>
                </td>
            </tr>
        </table>
    </div>
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td class="td_form2_td1" style="width: 20%">
                <div>
                    <b>Chọn đợt: </b>
                    <%=MyHtmlHelper.DropDownList(ParentID, slDot, iID_MaDot, "iID_MaDot", "", "class=\"input1_2\" style=\"width: 50%;height:24px;\" onchange=Refresh()")%>
                </div>
            </td>
            <td class="td_form2_td1" style="width: 20%">
                <div>
                    Chọn ngành:</div>
            </td>
            <td style="width: 40%;" class="td_form2_td5">
                <div id="<%= ParentID %>_tdNganh" style="overflow: scroll; height: 400px">
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
            $(document).ready(function () {
            Refresh();
                  
               });
                 function Refresh() {
                        var iID_MaDonvi = "";
//                        $("input:checkbox[check-group='Nganh']").each(function () {
//                            if (this.checked) {
//                                if (iID_MaDonVi != "") iID_MaDonVi += ",";
//                                iID_MaDonVi += this.value;
//                            }
//                        });
                        var iID_MaDot = document.getElementById("<%=ParentID %>_iID_MaDot").value;
                        var MaPhongBan ="";
                        jQuery.ajaxSetup({ cache: false });
                        var url = unescape('<%= Url.Action("LayDanhSachNganh?ParentID=#0&iID_MaDot=#1&iID_MaPhongBan=#2&iID_MaDonVi=#3", "rptDuToanBS_207_TungDonVi") %>');
                        url = unescape(url.replace("#0", "<%= ParentID %>"));
                        url = unescape(url.replace("#1", iID_MaDot));
                        url = unescape(url.replace("#2", MaPhongBan));
                        var pageLoad = <%=PageLoad %>;
                        url = unescape(url.replace("#3", "<%= iID_MaDonVi %>"));
                        $.getJSON(url, function (data) {
                            document.getElementById("<%= ParentID %>_tdNganh").innerHTML = data;
                        });
                }
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
              function CheckAllDV(value) {
                  $("input:checkbox[check-group='DonVi']").each(function (i) {
                      this.checked = value;
                  });
              }
              function Huy() {
                 window.location.href = '<%=BackURL%>';
             }
        </script>
    </table>
    <%} %>
</body>
</html>
