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
        DataTable dtDonVi = QuyetToan_ReportModels.dtDonVi_QS(MaND);

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

        String bCheckTongHop = Convert.ToString(ViewData["bCheckTongHop"]); ;

        String iID_MaPhongBan = Convert.ToString(ViewData["iID_MaPhongBan"]);
        if (String.IsNullOrEmpty(iID_MaPhongBan)) iID_MaPhongBan = "-2";
        String iThang = Convert.ToString(ViewData["iThang"]);
        if (String.IsNullOrEmpty(iThang))
            iThang = "-1";
        DataTable dtThang = DanhMucModels.DT_Thang(true);
        SelectOptionList slThang = new SelectOptionList(dtThang, "MaThang", "TenThang");
        dtThang.Dispose();

        String LoaiThang_Quy = Convert.ToString(ViewData["LoaiThang_Quy"]);
        if (String.IsNullOrEmpty(LoaiThang_Quy))
        {
            LoaiThang_Quy = "0";
        }
        String iQuy = Convert.ToString(ViewData["iQuy"]);
        DataTable dtQuy = DanhMucModels.DT_Quy();
        SelectOptionList slQuy = new SelectOptionList(dtQuy, "MaQuy", "TenQuy");

        String iThangQuy = "";
        if (LoaiThang_Quy.Equals("0"))
        {
            iThangQuy = iThang;
        }
        else
            iThangQuy = iQuy;
        DataTable dtPhongBan = QuyetToanModels.getDSPhongBan_QuanSo(iNamLamViec, MaND);
        DataRow dr = dtPhongBan.NewRow();
        dr["iID_MaPhongBan"] = "-2";
        dr["sTenPhongBan"] = "--Chọn phòng ban--";
        dtPhongBan.Rows.InsertAt(dr, 0);
        SelectOptionList slPhongBan = new SelectOptionList(dtPhongBan, "iID_MaPhongBan", "sTenPhongBan");
        if (String.IsNullOrEmpty(iID_MaPhongBan))
        {
            if (dtDonVi.Rows.Count > 0)
            {
                iID_MaPhongBan = Convert.ToString(dtPhongBan.Rows[0]["iID_MaPhongBan"]);
            }
            else
            {
                iID_MaPhongBan = "-1";
            }
        }
        dtPhongBan.Dispose();

        dtQuy.Dispose();
        String Chuoi = "";
        String PageLoad = Convert.ToString(ViewData["PageLoad"]);
        if (String.IsNullOrEmpty(PageLoad))
            PageLoad = "0";
        String[] arrMaDonVi = iID_MaDonVi.Split(',');

        String[] arrView = new String[arrMaDonVi.Length];
        if (String.IsNullOrEmpty(iID_MaDonVi)) PageLoad = "0";
        if (PageLoad == "1")
        {

            for (int i = 0; i < arrMaDonVi.Length; i++)
            {
                arrView[i] =
                    String.Format(
                        @"/rptQTQS_THQS_TungDonVi_Nam/viewpdf?iID_MaDonVi={0}&MaND={1}&iID_MaPhongBan={2}&bCheckTongHop={3}",
                        arrMaDonVi[i], MaND, iID_MaPhongBan, bCheckTongHop);
                Chuoi += arrView[i];
                if (i < arrMaDonVi.Length - 1)
                    Chuoi += "+";
            }

        }
        String URL = Url.Action("Index", "QuyetToan_QuanSo_Report");
        using (Html.BeginForm("EditSubmit", "rptQTQS_THQS_TungDonVi_Nam", new { ParentID = ParentID }))
        {
    %>
    <div class="title_tong">
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
            <tr>
                <td>
                    <span>Tổng hợp tình hình quân số năm
                        <%=iNamLamViec %>
                    </span>
                </td>
            </tr>
        </table>
    </div>
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td class="td_form2_td1" style="width: 10%">
                <div>
                    Chọn phòng ban:</div>
            </td>
            <td class="td_form2_td1" style="width: 20%">
                <div>
                    <%=MyHtmlHelper.DropDownList(ParentID, slPhongBan, iID_MaPhongBan, "iID_MaPhongBan", "", "class=\"input1_2\" style=\"width: 100%\" onchange=\"ChonDonVi1()\"")%>
                </div>
            </td>
            <td class="td_form2_td1" style="width: 10%" rowspan="2">
                <div>
                    Chọn đơn vị:</div>
            </td>
            <td style="width: 40%" rowspan="2">
                <div id="<%= ParentID %>_tdDonVi" style="overflow: scroll; height: 400px">
                </div>
            </td>
            <td class="td_form2_td1">
            </td>
        </tr>
        <tr>
            <td class="td_form2_td1" style="width: 10%">
                <div>
                    Tổng hợp:</div>
            </td>
            <td class="td_form2_td1" style="width: 20%; text-align: left">
                <div>
                    <%=MyHtmlHelper.CheckBox(ParentID, bCheckTongHop, "bCheckTongHop", "", "class=\"input1_2\" style=\"width: 100%\"")%>
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
              function Huy() {
               window.location.href = '<%=URL %>';
               }
           ChonDonVi1();
           function ChonDonVi1() {
                jQuery.ajaxSetup({ cache: false });
                var MaPhongBan = document.getElementById("<%=ParentID %>_iID_MaPhongBan").value;
                var url = unescape('<%= Url.Action("LayDanhSachDonVi?ParentID=#0&iID_MaPhongBan=#1&iID_MaDonVi=#2", "rptQTQS_THQS_TungDonVi_Nam") %>');
                url = unescape(url.replace("#0", "<%= ParentID %>"));
                url = unescape(url.replace("#1", MaPhongBan));        
                url = unescape(url.replace("#2", "<%=iID_MaDonVi %>"));  
                    
                $.getJSON(url, function (data) {
                    document.getElementById("<%= ParentID %>_tdDonVi").innerHTML = data;
                });
            } 
        </script>
         <script type="text/javascript">
             function CheckAll(value) {
                 $("input:checkbox[check-group='DonVi']").each(function (i) {
                     this.checked = value;
                 });
             }                                            
 </script>
    </table>
    <%} %>
</body>
</html>
