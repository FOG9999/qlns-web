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
        DataTable dtDonVi = DuToan_ReportModels.dtTo(MaND, "1010000");

        if (String.IsNullOrEmpty(iID_MaDonVi))
        {
            if (dtDonVi.Rows.Count > 0)
            {
                //iID_MaDonVi = Convert.ToString(dtDonVi.Rows[0]["iID_MaDonVi"]);
            }
            else
            {
                iID_MaDonVi = Guid.Empty.ToString();
            }
        }
        dtDonVi.Dispose();

        DataTable dtDot = DuToanBS_ReportModels.LayDSDot(iNamLamViec, MaND, "");
        SelectOptionList slDot = new SelectOptionList(dtDot, "iID_MaDot", "iID_MaDot");
        String idDot = Convert.ToString(ViewData["iID_MaDot"]);
        String iID_MaTo = Convert.ToString(ViewData["iID_MaTo"]);
        if (String.IsNullOrEmpty(idDot))
        {
            if (dtDot.Rows.Count > 0)
                idDot = Convert.ToString(dtDot.Rows[0]["iID_MaDot"]);
            else
                idDot = Guid.Empty.ToString();
        }
        dtDot.Dispose();

        DataTable dtPhongBan = DuToanBS_ReportModels.LayDSPhongBan(iNamLamViec, MaND);
        SelectOptionList slPhongBan = new SelectOptionList(dtPhongBan, "iID_MaPhongBan", "sTenPhongBan");

        String MaPhongBan = Convert.ToString(ViewData["iID_MaPhongBan"]);
        if (MaPhongBan == null)
            MaPhongBan = "-1";

        dtPhongBan.Dispose();

        String MaTo = Convert.ToString(ViewData["MaTo"]);
        String[] arrMaDonVi = MaTo.Split(',');
        String[] arrMaTo = MaTo.Split(',');
        String[] arrView = new String[arrMaTo.Length];
        String Chuoi = "";
        String PageLoad = Convert.ToString(ViewData["PageLoad"]);
        if (String.IsNullOrEmpty(PageLoad))
            PageLoad = "0";
        if (String.IsNullOrEmpty(MaTo)) PageLoad = "0";
        if (PageLoad == "1")
        {

            for (int i = 0; i < arrMaTo.Length; i++)
            {
                arrView[i] =
                    String.Format(
                        @"/rptDuToanBS_TongHop_ChonTo/viewpdf?MaTo={0}&sLNS={1}&MaND={2}",
                        arrMaTo[i], "1010000", MaND);
                Chuoi += arrView[i];
                if (i < arrMaTo.Length - 1)
                    Chuoi += "+";
            }

        }
        using (Html.BeginForm("EditSubmit", "rptDuToanBS_TongHop_ChonTo", new { ParentID = ParentID }))
        {
    %>
    <div class="title_tong">
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
            <tr>
                <td>
                    <span>Báo cáo tổng hợp in kiểm chỉ tiêu bổ sung năm
                        <%=iNamLamViec %>
                    </span>
                </td>
            </tr>
        </table>
    </div>
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td class="td_form2_td1" style="width: 10%; height: 20px">
                <div>
                    <b>Chọn đợt</b></div>
            </td>
            <td class="td_form2_td5" style="width: 14%; height: 20px">
                <div>
                    <%=MyHtmlHelper.DropDownList(ParentID, slDot, idDot, "iID_MaDot", "", "class=\"input1_2\" style=\"width:100%;\"onchange=Chon()")%>
                </div>
            </td>
            <td class="td_form2_td1" style="width: 20%">
                <div>
                    Chọn tờ:</div>
            </td>
            <td rowspan="2" style="width: 20%;" class="td_form2_td5">
                <div id="<%= ParentID %>_tdTo" style="overflow: scroll; height: 400px">
                </div>
            </td>
            <td class="td_form2_td1">
            </td>
        </tr>
        <tr>
            <td class="td_form2_td1" style="width: 10%; height: 20px">
                <div>
                    <b>Chọn phòng ban </b>
                </div>
            </td>
            <td class="td_form2_td5" style="width: 14%; height: 20px">
                <div>
                    <%=MyHtmlHelper.DropDownList(ParentID, slPhongBan, MaPhongBan, "iID_MaPhongBan", "", "class=\"input1_2\" style=\"width:100%;\"onchange=Chon() ")%>
                </div>
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
                                <input type="button" class="button" value="Hủy" onclick="Dialog_close('<%=ParentID %>');" />
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
                  $("input:checkbox[check-group='MaTo']").each(function (i) {
                      this.checked = value;
                  });
              }
              Chon();
              function Chon() {
//                 var iID_MaDonVi = "";
//                     $("input:checkbox[check-group='DonVi']").each(function () {
//                         if (this.checked) {
//                             if (iID_MaDonVi != "") iID_MaDonVi += ",";
//                             iID_MaDonVi += this.value;
//                         }
//                     });
               
                 var iID_MaDot = document.getElementById("<%=ParentID %>_iID_MaDot").value;
                 var MaPhongBan = document.getElementById("<%=ParentID %>_iID_MaPhongBan").value;
                 var iID_MaTo="";
                jQuery.ajaxSetup({ cache: false });
                var url = unescape('<%= Url.Action("LayDanhSachDot?ParentID=#0&iID_MaDot=#1&iID_MaPhongBan=#2&iID_MaTo=#3", "rptDuToanBS_TongHop_ChonTo") %>');
                url = unescape(url.replace("#0", "<%= ParentID %>"));
                url = unescape(url.replace("#1", iID_MaDot));
                url = unescape(url.replace("#2", MaPhongBan));
                var pageLoad = <%=PageLoad %>;
                if(pageLoad=="1") 
                    url = unescape(url.replace("#3", "<%= iID_MaTo %>"));
                else
                    url = unescape(url.replace("#3", iID_MaTo));

                    
                $.getJSON(url, function (data) {
                    document.getElementById("<%= ParentID %>_tdTo").innerHTML = data;
                });
            }
        </script>
    </table>
    <%} %>
</body>
</html>
