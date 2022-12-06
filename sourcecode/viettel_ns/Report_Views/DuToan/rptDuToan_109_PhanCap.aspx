<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="VIETTEL.Report_Controllers.DuToan" %>
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
        var trinhky = (int)ViewData["trinhky"];

        String sLNS = "2";
        String iID_MaPhongBan = Convert.ToString(ViewData["iID_MaPhongBan"]);
              if (String.IsNullOrEmpty(sLNS))
        {
            sLNS = "0";
        }
        if (String.IsNullOrEmpty(iID_MaPhongBan))
        {
            iID_MaPhongBan = "0";
        }
        if (String.IsNullOrEmpty(ToSo))
        {
            ToSo = "1";
        }
    
        DataTable dtTo = rptDuToan_109_PhanCapController.DanhSachToIn(MaND, "", ToSo,sLNS,iID_MaPhongBan);
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
                        @"/rptDuToan_109_PhanCap/viewpdf?ToSo={0}&MaND={1}&Nganh={2}&sLNS={3}&iID_MaPhongBan={4}&trinhky={5}",
                        arrMaTo[i], MaND, "",sLNS,iID_MaPhongBan, trinhky);
                Chuoi += arrView[i];
                if (i < arrMaTo.Length - 1)
                    Chuoi += "+";
            }

        }

        String BackURL = Url.Action("Index", "DuToan_Report", new { sLoai = "0" });
        using (Html.BeginForm("EditSubmit", "rptDuToan_109_PhanCap", new { ParentID, trinhky }))
        {
    %>
    <div class="box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Báo cáo dự toán nhà nước giao tính vào ngân sách quốc phòng
                            <%=iNamLamViec%>
                            (Phần phụ lục 4b- C)</span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="Div1">
            <div id="Div2">
                <table cellpadding="0" cellspacing="0" border="0" width="100%">
                    <tr>
                        <td class="td_form2_td1" style="width: 10%">
                            <div>
                                Chọn tờ:</div>
                        </td>
                        <td style="width: 30%" rowspan="1">
                            <div id="<%= ParentID %>_tdDonVi" style="overflow: scroll; height: 200px">
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
                Chon();
            function Chon() {
                jQuery.ajaxSetup({ cache: false });
                var url = unescape('<%= Url.Action("Ds_DonVi?ParentID=#0&Nganh=#1&ToSo=#2&sLNS=#3&iID_MaPhongBan=#4", "rptDuToan_109_PhanCap") %>');
                url = unescape(url.replace("#0", "<%= ParentID %>"));
                url = unescape(url.replace("#1", ""));
                url = unescape(url.replace("#2","<%= MaTo %>"));
                url = unescape(url.replace("#3","<%= sLNS %>"));
                url = unescape(url.replace("#4","<%= iID_MaPhongBan %>"));
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
    <%--<div>
        <%=MyHtmlHelper.ActionLink(Url.Action("ExportToExcel", "rptDuToan_109_PhanCap", new { MaND = MaND, Nganh = "", ToSo = MaTo, iID_MaPhongBan = iID_MaPhongBan, sLNS = sLNS }), "Xuất ra Excels")%>
    </div>--%>
</body>
</html>
