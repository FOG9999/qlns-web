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
        String iID_MaNguon = Convert.ToString(ViewData["iID_MaNguon"]);
        String iID_Muc = Convert.ToString(ViewData["iID_Muc"]);
        String MaND = User.Identity.Name;
        String iNamLamViec = ReportModels.LayNamLamViec(MaND);
        
        String BackURL = Url.Action("Index", "DuToan_Report", new { sLoai = "0" });
        DataTable dtPhongBan = DuToan_ReportModels.getDSPhongBan(iNamLamViec, MaND, "1");
        SelectOptionList slPhongBan = new SelectOptionList(dtPhongBan, "iID_MaPhongBan", "sTenPhongBan");
        dtPhongBan.Dispose();

        DataTable dt = new DataTable();
        dt.Columns.Add("iID", typeof(String));
        dt.Columns.Add("sTen", typeof(String));
        DataRow dr = dt.NewRow();
        dr[0] = "1";
        dr[1] = "Ngân sách quốc phòng";
        dt.Rows.Add(dr);
        dr = dt.NewRow();
        dr[0] = "2";
        dr[1] = "Ngân sách nhà nước";
        dt.Rows.Add(dr);
        SelectOptionList slNganSach = new SelectOptionList(dt, "iID", "sTen");


        dt = new DataTable();
        dt.Columns.Add("iID", typeof(String));
        dt.Columns.Add("sTen", typeof(String));
         dr = dt.NewRow();
        dr[0] = "1";
        dr[1] = "Chi tiết";
        dt.Rows.Add(dr);
        dr = dt.NewRow();
        dr[0] = "2";
        dr[1] = "Tổng hợp";
        dt.Rows.Add(dr);
        SelectOptionList slMuc = new SelectOptionList(dt, "iID", "sTen");


        if (PageLoad == "1")
        {
            URLView = Url.Action("ViewPDF", "rptDuToan_TongHop_KiemKhoBac", new { MaND = MaND, iID_MaPhongBan = iID_MaPhongBan,iID_Muc=iID_Muc,iID_MaNguon=iID_MaNguon });
        }
        using (Html.BeginForm("EditSubmit", "rptDuToan_TongHop_KiemKhoBac", new { ParentID = ParentID }))
        {
    %>
    <div class="box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Báo cáo in kiếm số phẩn bổ ra kho bạc</span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="Div1">
            <div id="Div2">
                <table width="100%" cellpadding="0" cellspacing="0" border="0" class="table_form2">
                    <tr>
                       <td class="td_form2_td1" style="text-align: center; width: 10%">
                        
                        </td>
                        <td class="td_form2_td1" style="text-align: center; width: 20%">
                            <div>
                                <b>Chọn phòng ban: </b>
                                <%=MyHtmlHelper.DropDownList(ParentID, slPhongBan, iID_MaPhongBan, "iID_MaPhongBan", "", "class=\"input1_2\" style=\"width: 40%;height:24px;\"")%>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="text-align: center; width: 20%">
                            <div>
                                <b>Chọn ngân sách: </b>
                                <%=MyHtmlHelper.DropDownList(ParentID, slNganSach, iID_MaNguon, "iID_MaNguon", "", "class=\"input1_2\" style=\"width: 50%;height:24px;\"")%>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="text-align: center; width: 20%">
                            <div>
                                <b>Chon mức: </b>
                                <%=MyHtmlHelper.DropDownList(ParentID, slMuc, iID_Muc, "iID_Muc", "", "class=\"input1_2\" style=\"width: 50%;height:24px;\"")%>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="td_form2_td1" style="margin: 0 auto;" colspan="4">
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
    <%=MyHtmlHelper.ActionLink(Url.Action("ExportToExcel", "rptDuToan_TongHop_KiemKhoBac", new { MaND = MaND, iID_MaPhongBan = iID_MaPhongBan, iID_MaNguon = iID_MaNguon,iID_Muc=iID_Muc }), "Export to Excel")%>
    <iframe src="<%=URLView%>" height="600px" width="100%"></iframe>
</body>
</html>
