<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="VIETTEL.Report_Controllers.QuyetToan" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript" src="../../../Scripts/jquery-latest.js"></script>
</head>
<body>
    <%
        String ParentID = "QuyetToanNganSach";
        String MaND = User.Identity.Name;
        String iNamLamViec = ReportModels.LayNamLamViec(MaND);
       

        //dt Danh sách phòng ban
        String iID_MaPhongBan = NganSach_HamChungModels.MaPhongBanCuaMaND(MaND);

        DataTable dtPhongBan = QuyetToanModels.getDSPhongBan(iNamLamViec, MaND);
        SelectOptionList slPhongBan = new SelectOptionList(dtPhongBan, "iID_MaPhongBan", "sTenPhongBan");
        dtPhongBan.Dispose();

        //dt Loại ngân sách
        String sLNS = Convert.ToString(ViewData["sLNS"]);
        DataTable dtLNS = DanhMucModels.NS_LoaiNganSach_PhongBan(iID_MaPhongBan);
        SelectOptionList slLNS = new SelectOptionList(dtLNS, "sLNS", "TenHT");
        if (String.IsNullOrEmpty(sLNS))
        {
            if (dtLNS.Rows.Count > 0)
            {
                sLNS = Convert.ToString(dtLNS.Rows[0]["sLNS"]);
            }
            else
            {
                sLNS = Guid.Empty.ToString();
            }
        }
        dtLNS.Dispose();


        String sL = Convert.ToString(ViewData["sL"]);
        String sK = Convert.ToString(ViewData["sK"]);
        String sM = Convert.ToString(ViewData["sM"]);
        String sTM = Convert.ToString(ViewData["sTM"]);
        String sTTM = Convert.ToString(ViewData["sTTM"]);
        String sNG = Convert.ToString(ViewData["sNG"]);
        
        String BackURL = Url.Action("Index", "QuyetToan_Report", new { Loai = 0 });

        //VungNV: add new MaPhongBan
        String MaPhongBan = Convert.ToString(ViewData["MaPhongBan"]);
        //String[] arrLNS = sLNS.Split(',');

        //VungNV: add new iID_MaDonVi
        String iID_MaDonVi = Convert.ToString(ViewData["iID_MaDonVi"]);
        if (String.IsNullOrEmpty(iID_MaDonVi)) iID_MaDonVi = "-100";
        String[] arrDonVi = iID_MaDonVi.Split(',');
        String[] arrView = new String[1];
        String Chuoi = "";
        String PageLoad = Convert.ToString(ViewData["PageLoad"]);
        if (String.IsNullOrEmpty(PageLoad))
            PageLoad = "0";
        if (String.IsNullOrEmpty(sLNS)) PageLoad = "0";
        if (PageLoad == "1")
        {

            for (int i = 0; i < 1; i++)
            {
                arrView[i] =
                    String.Format(
                        @"/rptDuToanInKiemToanBo/viewpdf?iID_MaDonVi={0}&sLNS={1}&sL={2}&sK={3}&sM={4}&sTM={5}&sTTM={6}&sNG={7}&MaPhongBan={8}",
                        iID_MaDonVi, sLNS, sL, sK,sM,sTM,sTTM,sNG, MaPhongBan);
                Chuoi += arrView[i];
                if (i < arrDonVi.Length - 1)
                    Chuoi += "+";
            }

        }


        int SoCot = 1;
        String[] arrMaNS = sLNS.Split(',');
        String urlExport = Url.Action("ExportToExcel", "rptDuToanInKiemToanBo", new { });
        using (Html.BeginForm("EditSubmit", "rptDuToanInKiemToanBo", new { ParentID = ParentID, }))
        {
    %>
    <div class="box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Báo cáo kiểm số liệu dự toán năm
                            <%=iNamLamViec%></span>
                    </td>
                    <td width="52%" style="text-align: left;">
                        <div class="login1" style="width: 50px; height: 20px; text-align: left;">
                            <a style="cursor: pointer;"></a>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div id="rptMain" style="background-color: #F0F9FE;">
            <div id="Div2" style="margin-left: 10px;" class="table_form2">
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td class="td_form2_td1" style="width: 10%">
                            <b>Loại Ngân Sách : </b>
                        </td>
                        <td style="width: 25%; height: 300px">
                            <div style="overflow: scroll; height: 400px">
                                <table class="mGrid" style="width: 100%">
                                    <tr>
                                        <th align="center" style="width: 40px;">
                                            <input type="checkbox" id="abc" onclick="CheckAllLNS(this.checked)" />
                                        </th>
                                        <%for (int c = 0; c < SoCot * 2 - 1; c++)
                                          {%>
                                        <th>
                                        </th>
                                        <%} %>
                                    </tr>
                                    <%
                
String strsTen = "", MaNS = "", strChecked = "";
for (int i = 0; i < dtLNS.Rows.Count; i = i + SoCot)
{
                                       
                                    %>
                                    <tr>
                                        <%for (int c = 0; c < SoCot; c++)
                                          {
                                              if (i + c < dtLNS.Rows.Count)
                                              {
                                                  strChecked = "";
                                                  strsTen = Convert.ToString(dtLNS.Rows[i + c]["TenHT"]);
                                                  MaNS = Convert.ToString(dtLNS.Rows[i + c]["sLNS"]);
                                                  for (int j = 0; j < arrMaNS.Length; j++)
                                                  {
                                                      if (MaNS.Equals(arrMaNS[j]))
                                                      {
                                                          strChecked = "checked=\"checked\"";
                                                          break;
                                                      }
                                                  }
                                        %>
                                        <td align="center" style="width: 40px;">
                                            <input type="checkbox" value="<%=MaNS %>" <%=strChecked %> check-group="LNS" onclick="Chon()"
                                                id="sLNS" name="sLNS" />
                                        </td>
                                        <td align="left">
                                            <%=strsTen%>
                                        </td>
                                        <%} %>
                                        <%} %>
                                    </tr>
                                    <%}%>
                                </table>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="width: 10%; height: 10px">
                            <b>Đơn vị :</b>
                        </td>
                        <td style="width: 30%;" colspan="11">
                            <div id="<%= ParentID %>_tdDonVi" style="overflow: scroll; height: 400px">
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="td_form2_td1" style="width: 10%; height: 20px">
                            <div>
                                <b>Chọn phòng ban :</b>
                            </div>
                        </td>
                        <td>
                            <div>
                                <%=MyHtmlHelper.DropDownList(ParentID, slPhongBan, MaPhongBan, "iID_MaPhongBan", "", "class=\"input1_2\" style=\"width:100%;\"onchange=Chon() ")%>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="width: 5%; height: 20px">
                            <div>
                                <b>L :</b>
                            </div>
                        </td>
                        <td style="width: 5%">
                            <div>
                                <%=MyHtmlHelper.TextBox(ParentID, sL, "sL", "", "class=\"input1_2\" style=\"width:100%;\" ")%>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="width: 5%; height: 20px">
                            <div>
                                <b>K :</b>
                            </div>
                        </td>
                        <td style="width: 5%">
                            <div>
                                <%=MyHtmlHelper.TextBox(ParentID, sK, "sK", "", "class=\"input1_2\" style=\"width:100%;\" ")%>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="width: 5%; height: 20px">
                            <div>
                                <b>M :</b>
                            </div>
                        </td>
                        <td style="width: 5%">
                            <div>
                                <%=MyHtmlHelper.TextBox(ParentID, sM, "sM", "", "class=\"input1_2\" style=\"width:100%;\" ")%>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="width: 5%; height: 20px">
                            <div>
                                <b>TM :</b>
                            </div>
                        </td>
                        <td style="width: 5%">
                            <div>
                                <%=MyHtmlHelper.TextBox(ParentID, sTM, "sTM", "", "class=\"input1_2\" style=\"width:100%;\" ")%>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="width: 5%; height: 20px">
                            <div>
                                <b>TTM :</b>
                            </div>
                        </td>
                        <td style="width: 5%">
                            <div>
                                <%=MyHtmlHelper.TextBox(ParentID, sTTM, "sTTM", "", "class=\"input1_2\" style=\"width:100%;\" ")%>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="width: 5%; height: 20px">
                            <div>
                                <b>NG :</b>
                            </div>
                        </td>
                        <td style="width: 5%">
                            <div>
                                <%=MyHtmlHelper.TextBox(ParentID, sNG, "sNG", "", "class=\"input1_2\" style=\"width:100%;\" ")%>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="12" align="center">
                            <table cellpadding="0" cellspacing="0" border="0" align="center">
                                <tr>
                                    <td>
                                        <input type="submit" class="button" id="Submit2" value="<%=NgonNgu.LayXau("Thực hiện")%>" />
                                    </td>
                                    <td width="5px">
                                    </td>
                                    <td>
                                        <input class="button" type="button" value="<%=NgonNgu.LayXau("Hủy")%>" onclick="Huy()" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <script type="text/javascript">
            function CheckAllLNS(value) {
                $("input:checkbox[check-group='LNS']").each(function (i) {
                    this.checked = value;
                });
                Chon();
            }                                            
        </script>
        <script type="text/javascript">
            function CheckAll(value) {
                $("input:checkbox[check-group='DonVi']").each(function (i) {
                    this.checked = value;
                });
            }                                            
        </script>
        <script type="text/javascript">
            $(document).ready(function () {
                   $('.title_tong a').click(function () {
                    $('div#rptMain').slideToggle('normal');
                    $(this).toggleClass('active');
                    return false;
                });
                var count = <%=arrView.Length%>;
                var Chuoi = '<%=Chuoi%>';
                var Mang = Chuoi.split("+");
                var pageLoad = <%=PageLoad %>;
                
                if(pageLoad=="1") 
                {
                  var siteArray = new Array(count);
                  for (var i = 0; i < count; i++) {
                       siteArray[i] = Mang[i];
                    }
                  for (var i = 0; i < count; i++) {
                        window.open(siteArray[i], '_blank');
                    }
                } 
            });

            Chon();
            function Chon() {
                 var sLNS = "";
                     $("input:checkbox[check-group='LNS']").each(function (i) {
                         if (this.checked) {
                             if (sLNS != "") sLNS += ",";
                             sLNS += this.value;
                         }
                     });

                 var MaPhongBan = document.getElementById("<%=ParentID %>_iID_MaPhongBan").value;
                 
                jQuery.ajaxSetup({ cache: false });

                var url = unescape('<%= Url.Action("Ds_DonVi?ParentID=#0&iID_MaDonVi=#1&sLNS=#2&&iID_MaPhongBan=#3", "rptDuToanInKiemToanBo") %>');
                url = unescape(url.replace("#0", "<%= ParentID %>"));
                url = unescape(url.replace("#1", "<%= iID_MaDonVi %>"));
                url = unescape(url.replace("#2", sLNS));
                url = unescape(url.replace("#3", MaPhongBan));
                
                $.getJSON(url, function (data) {
                    document.getElementById("<%= ParentID %>_tdDonVi").innerHTML = data;
                });
            }
                                                     
        </script>
        <script type="text/javascript">
            function Huy() {
                window.location.href = '<%=BackURL%>';
            }
        </script>
        <div>
            <%=MyHtmlHelper.ActionLink(urlExport, "Export To Excel") %>
        </div>
    </div>
    <%} %>
</body>
</html>
