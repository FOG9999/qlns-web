<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="VIETTEL.Report_Controllers.DuToan" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <%
        String ParentID = "DuToan";

        String URLView = "";
        String PageLoad = Convert.ToString(ViewData["PageLoad"]);
        String iID_MaPhongBan = Convert.ToString(ViewData["iID_MaPhongBan"]);
        String MaLoai = Convert.ToString(ViewData["MaLoai"]);
        var trinhky = (int)ViewData["trinhky"];
        String MaND = User.Identity.Name;
        String iNamLamViec = ReportModels.LayNamLamViec(MaND);
        DataTable dtLoai = new DataTable();
        dtLoai.Columns.Add("MaLoai", typeof(String));
        dtLoai.Columns.Add("sTen", typeof(String));
        DataRow dr = dtLoai.NewRow();
        dr["MaLoai"] = "QP";
        dr["sTen"] = "Thu nộp NSQP";
        dtLoai.Rows.Add(dr);
        dr = dtLoai.NewRow();
        dr["MaLoai"] = "NN";
        dr["sTen"] = "Thu nộp NSNN";
        dtLoai.Rows.Add(dr);

        SelectOptionList slLoai = new SelectOptionList(dtLoai, "MaLoai", "sTen");
        if (PageLoad == "1")
        {
            URLView = Url.Action("ViewPDF", "rptDuToan_ThuNop_TongHop", new { MaND, iID_MaPhongBan, trinhky,MaLoai });
        }
        String BackURL = Url.Action("Index", "DuToan_Report", new { sLoai = "0" });
        DataTable dtPhongBan = DuToan_ReportModels.getDSPhongBan(iNamLamViec, MaND, "8");
        SelectOptionList slPhongBan = new SelectOptionList(dtPhongBan, "iID_MaPhongBan", "sTenPhongBan");
        dtPhongBan.Dispose();
        using (Html.BeginForm("EditSubmit", "rptDuToan_ThuNop_TongHop", new { ParentID, trinhky }))
        {
    %>
    <div class="box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Báo cáo tổng hợp phần thu ngân sách ( Biểu tổng hợp theo đơn vị )</span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="Div1">
            <div id="Div2">
                <table width="100%" cellpadding="0" cellspacing="0" border="0" class="table_form2">
                    <tr>
                        <td class="td_form2_td1" style="text-align: center; width: 50%">
                            <div>
                                <b>Chọn phòng ban: </b>
                                <%=MyHtmlHelper.DropDownList(ParentID, slPhongBan, iID_MaPhongBan, "iID_MaPhongBan", "", "class=\"input1_2\" style=\"width: 30%;height:24px;\"")%>
                            </div>
                        </td>
                         <td class="td_form2_td1" style="text-align: left; width: 60%">
                            <div>
                                <b>Chọn loại: </b>
                                <%=MyHtmlHelper.DropDownList(ParentID, slLoai, MaLoai, "MaLoai", "", "class=\"input1_2\" style=\"width: 30%;height:24px;\"")%>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="td_form2_td1" style="margin: 0 auto;" colspan="2">
                            <table cellpadding="0" cellspacing="0" border="0" style="margin-left: 45%;">
                                <tr>
                                    <td>
                                        <input type="submit" class="button" id="Submit1" value="<%=NgonNgu.LayXau("Thực hiện")%>" />
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
    </div>
    <%}
       
    %>
    <script type="text/javascript">
        function Huy() {
            window.location.href = '<%=BackURL%>';
        }
    </script>
    <%=MyHtmlHelper.ActionLink(Url.Action("ExportToExcel", "rptDuToan_ThuNop_TongHop", new { MaND, iID_MaPhongBan, trinhky,MaLoai }), "Export to Excel")%>
    <iframe src="<%=URLView%>" height="600px" width="100%"></iframe>
</body>
</html>
