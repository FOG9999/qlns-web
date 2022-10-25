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
        String iID_MaPhongBan = Convert.ToString(ViewData["iID_MaPhongBan"]);
        String iCapTongHop = Convert.ToString(ViewData["iCapTongHop"]);
        if (String.IsNullOrEmpty(iCapTongHop)) iCapTongHop = "1";
        String iID_MaDonVi = Convert.ToString(ViewData["iID_MaDonVi"]);
        DataTable dtDonVi = DuToan_ReportModels.dtDonVi(MaND, "ALL");
        if (String.IsNullOrEmpty(iID_MaDonVi))
        {
            if (dtDonVi.Rows.Count > 0)
            {
                iID_MaDonVi = Convert.ToString(dtDonVi.Rows[0]["iID_MaDonVi"]);
            }
            else
            {
                iID_MaDonVi = Guid.Empty.ToString();
            }
        }
        dtDonVi.Dispose();


        String Chuoi = "";
        String PageLoad = Convert.ToString(ViewData["PageLoad"]);
        if (String.IsNullOrEmpty(PageLoad))
            PageLoad = "0";
        if (String.IsNullOrEmpty(iID_MaDonVi)) PageLoad = "0";
        String[] arrMaDonVi = iID_MaDonVi.Split(',');
        String[] arrView = new String[arrMaDonVi.Length];
        if (PageLoad == "1")
        {
            if (iCapTongHop == "1")
            {
                iID_MaDonVi = "-1";
            }
            arrMaDonVi = iID_MaDonVi.Split(',');
             arrView = new String[arrMaDonVi.Length];

            for (int i = 0; i < arrMaDonVi.Length; i++)
            {
                arrView[i] =
                    String.Format(
                        @"/rptDuToan_ChiTiet_TongHop/viewpdf?iID_MaDonVi={0}&MaND={1}&iCapTongHop={2}",
                        arrMaDonVi[i], MaND, iCapTongHop);
                Chuoi += arrView[i];
                if (i < arrMaDonVi.Length - 1)
                    Chuoi += "+";
            }


        }
        String BackURL = Url.Action("Index", "DuToan_Report", new { sLoai = "2" });
        using (Html.BeginForm("EditSubmit", "rptDuToan_ChiTiet_TongHop", new { ParentID = ParentID }))
        {
    %>
    <div class="title_tong">
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
            <tr>
                <td>
                    <span>Báo cáo chi tiết bìa
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
                    Mức tổng hợp:</div>
            </td>
            <td style="width: 20%">
                <div>
                    <%=MyHtmlHelper.Option(ParentID, "1", iCapTongHop, "iCapTongHop", "")%>Tổng hợp&nbsp;&nbsp;&nbsp;&nbsp
                    <%=MyHtmlHelper.Option(ParentID, "2", iCapTongHop, "iCapTongHop", "")%>Chi tiết&nbsp;&nbsp;&nbsp;&nbsp
                </div>
            </td>
            <td class="td_form2_td1" style="width: 10%; height: 10px">
                <b>Đơn vị :</b>
            </td>
            <td rowspan="10" style="width: 30%;">
                <div id="<%= ParentID %>_tdDonVi" style="overflow: scroll; height: 400px">
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
                $(document).ready(function () {
               Chon();
            function Chon() {
                jQuery.ajaxSetup({ cache: false });
                var url = unescape('<%= Url.Action("Ds_DonVi?ParentID=#0&iID_MaDonVi=#1", "rptDuToan_ChiTiet_TongHop") %>');
                url = unescape(url.replace("#0", "<%= ParentID %>"));
                url = unescape(url.replace("#1", "<%= iID_MaDonVi %>"));
                $.getJSON(url, function (data) {
                    document.getElementById("<%= ParentID %>_tdDonVi").innerHTML = data;
                });}
                });
          
        </script>
    </table>
    <%} %>
</body>
</html>
