<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="Viettel.Services" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
</head>
<body>
    <%
        string ParentID = "rptNguonNS_THNguonBTC";
        string Mang = "";
        string PageLoad = Convert.ToString(ViewData["PageLoad"]);
        ISharedService _sharedService = SharedService.Default;

        string _nam = _sharedService.LayNamLamViec(User.Identity.Name);
        if (string.IsNullOrEmpty(PageLoad))
            PageLoad = "0";
        if (PageLoad == "1")
        {
           Mang ="/rptNguonNS_THNguonBTC/ViewPDF";
        }

        string BackURL = Url.Action("Index", "Nguon_NS");
        using (Html.BeginForm("EditSubmit", "rptNguonNS_THNguonBTC", new {ParentID=ParentID}))
        {
    %>
     <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Báo cáo tổng hợp nguồn ngân sách Bộ Tài chính cấp cho Bộ Quốc phòng năm <%=_nam%>
                           </span>
                    </td>
                   
                </tr>
            </table>
        </div>
    <table cellpadding="0" cellspacing="0" border="0" width="100%">                 
        <tr>
            <td colspan="3">
                <div style="margin-top: 10px;">
                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                        <tr>
                            <td style="width: 30%">
                            </td>
                            <td align="right">
                                <input type="submit" class="button" value="Tiếp tục" onclick="Huy()"/>
                            </td>
                            <td style="width: 1%">
                                &nbsp;
                            </td>
                            <td align="left">
                               <input type="button" class="button" value="Hủy" onclick="Huy()" />
                            </td>
                            <td style="width: 30%">
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
          <script type="text/javascript">
            var pageLoad = <%=PageLoad %>;
            if(pageLoad=="1") {
                window.open(Mang, '_blank');
            } 
              
            function Huy() {
                window.location.href = '<%=BackURL%>';
            }
          </script>
    </table>
    <%} %>
    <div>
        <%=MyHtmlHelper.ActionLink(Url.Action("DownloadExcel", "rptNguonNS_THNguonBTC"), "Xuất ra Excels")%>
    </div>
</body>
</html>