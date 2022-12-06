<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="VIETTEL.Report_Controllers.DuToan" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
   <%
       var MaND = User.Identity.Name;
       String iNamLamViec = ReportModels.LayNamLamViec(MaND);
       DataTable dtPhongBan = DuToan_ReportModels.getDSPhongBan(iNamLamViec, MaND, "1010000");
       SelectOptionList slPhongBan = new SelectOptionList(dtPhongBan, "iID_MaPhongBan", "sTenPhongBan");
       dtPhongBan.Dispose();

       var iID_MaPhongBan = Convert.ToString(ViewData["iID_MaPhongBan"]);






       using (Html.BeginForm("Index", "rptTongHop_GiaoBan", new { loai = "nsqp" }))
       {
    %>   
    <div class="box_tong">
         <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
        	    <tr>
            	    <td>
                	    <span>Tổng hợp tình hình thực hiện ngân sách năm <%=iNamLamViec %></span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="Div1">
            <div id="Div2">
                 <table width="100%" cellpadding="0"  cellspacing="0" border="0" class="table_form2">
                    <tr>
                        <td class="td_form2_td1" style="text-align:center;width:60%">
                            <div><b>Chọn phòng ban: </b>   
                                <%=MyHtmlHelper.DropDownList("", slPhongBan, iID_MaPhongBan, "iID_MaPhongBan", "", "class=\"input1_2\" style=\"width: 20%;height:24px;\"")%>

                                <% //Html.DropDownList("iID_MaPhongBan", (SelectList)ViewData["iID_MaPhongBan"]);%>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="td_form2_td1" style="margin:0 auto;" colspan = "2">
                            <table cellpadding="0" cellspacing="0" border="0" style="margin-left:45%;">
                                <tr>
                                    <td><%--<input type="submit" class="button" id="Submit1" value="<%=NgonNgu.LayXau("Thực hiện")%>" />--%></td>
                                    <td><input type="button" class="button" id="btnNSQP" value="<%=NgonNgu.LayXau("NSQP")%>" /></td>
                                    <td><input type="button" class="button" id="btnNSNN" value="<%=NgonNgu.LayXau("NS")%>" /></td>
                                    <td width="5px"></td>
                                    <td><input class="button" type="button" value="<%=NgonNgu.LayXau("Hủy")%>" onclick="Huy()" /></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                 </table>
            </div>
        </div>
    </div>
    <%}
       
         %>
 
    <script type="text/javascript">

       

        $("#btnNSQP").click(function () {

            var iID_MaPhongBan = $("#_iID_MaPhongBan").val();
            var url = unescape('<%= Url.Action("ViewPDF", "rptDuToan_B7_NSQP", new { nam=iNamLamViec}) %>');

            if (iID_MaPhongBan != "-1")
                url = url + "&iID_MaPhongBan=" + iID_MaPhongBan;

            window.open(url, '_blank');
        });

           $("#btnNSNN").click(function () {

            var iID_MaPhongBan = $("#_iID_MaPhongBan").val();
            var url = unescape('<%= Url.Action("ViewPDF", "rptDuToan_B7_NSNN", new { nam=iNamLamViec}) %>');

            if (iID_MaPhongBan != "-1")
                url = url + "&iID_MaPhongBan=" + iID_MaPhongBan;

            window.open(url, '_blank');
        });

    </script>
      
</body>
</html>
