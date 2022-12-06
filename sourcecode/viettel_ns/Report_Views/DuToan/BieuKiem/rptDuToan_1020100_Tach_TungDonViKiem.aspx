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
    String ParentID = "DuToan";
    String URLView = "";
    String PageLoad = Convert.ToString(ViewData["PageLoad"]);
    String iID_MaDonVi = Convert.ToString(ViewData["iID_MaDonVi"]);
    String MaND = User.Identity.Name;
    String iNamLamViec = ReportModels.LayNamLamViec(MaND);

    DataTable dtDonVi = DuToan_ReportModels.dtDonVi(MaND,"1020100,1020000");  
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

    if (PageLoad == "1")
    {
        URLView = Url.Action("ViewPDF", "rptDuToan_1020100_Tach_TungDonViKiem", new { MaND = MaND, iID_MaDonVi = iID_MaDonVi, iCapTongHop = "" });
    }  
    String BackURL = Url.Action("Index", "DuToan_Report", new { sLoai="0"});
    SelectOptionList slDonVi = new SelectOptionList(dtDonVi, "iID_MaDonVi", "TenHT");
    dtDonVi.Dispose();
    using (Html.BeginForm("EditSubmit", "rptDuToan_1020100_Tach_TungDonViKiem", new { ParentID = ParentID }))
    {
    %>   
    <div class="box_tong">
         <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
        	    <tr>
            	    <td>
                	    <span>Báo cáo số dự toán ngân sách nghiệp vụ năm các đơn vị</span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="Div1">
            <div id="Div2">
                 <table width="100%" cellpadding="0"  cellspacing="0" border="0" class="table_form2">
                    <tr>
                        <td class="td_form2_td1" style="text-align:center;width:60%">
                            <div><b>Chọn đơn vị: </b>   <%=MyHtmlHelper.DropDownList(ParentID, slDonVi, iID_MaDonVi, "iID_MaDonVi", "", "class=\"input1_2\" style=\"width: 20%;height:24px;\"")%>    
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="td_form2_td1" style="margin:0 auto;" colspan = "2">
                            <table cellpadding="0" cellspacing="0" border="0" style="margin-left:45%;">
                                <tr>
                                    <td><input type="submit" class="button" id="Submit1" value="<%=NgonNgu.LayXau("Thực hiện")%>" /></td>
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
          function Huy() {
              window.location.href = '<%=BackURL%>';
          }
    </script> 
    <%=MyHtmlHelper.ActionLink(Url.Action("ExportToExcel", "rptDuToan_1020100_Tach_TungDonViKiem", new { MaND = MaND, iID_MaDonVi = iID_MaDonVi, iCapTongHop = "" }), "Export to Excel")%>
      <iframe src="<%=URLView%>" height="600px" width="100%"></iframe>
</body>
</html>
