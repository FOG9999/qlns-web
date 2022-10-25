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
</head>
<body>
    <% 
        String MaND = User.Identity.Name;
        String ParentID = "QuyetToanNganSach";
        String iID_MaNamNganSach = Convert.ToString(ViewData["iID_MaNamNganSach"]);
        DataTable dtNamNganSach = QuyetToanModels.getDSNamNganSach();
        SelectOptionList slNamNganSach = new SelectOptionList(dtNamNganSach, "MaLoai", "sTen");
        dtNamNganSach.Dispose();

        String iNamLamViec = Convert.ToString(ViewData["iNamLamViec"]);
        DataTable dtNamLamViec = new DataTable();
        dtNamLamViec.Columns.Add("iNamLamViec", typeof(String));
        DataRow dr = dtNamLamViec.NewRow();
        dr[0] = "2015";
        dtNamLamViec.Rows.Add(dr);
        dr = dtNamLamViec.NewRow();
        dr[0] = "2016";
        dtNamLamViec.Rows.Add(dr);
        SelectOptionList slNamLamViec = new SelectOptionList(dtNamLamViec, "iNamLamViec", "iNamLamViec");
        
       
        String URLView = "";
        String PageLoad = Convert.ToString(ViewData["PageLoad"]);
        if (PageLoad == "1")
            URLView = Url.Action("ViewPDF", "rptQuyetToan_KiemToan101102", new { iNamLamViec = iNamLamViec, iID_MaNamNganSach = iID_MaNamNganSach });
        using (Html.BeginForm("EditSubmit", "rptQuyetToan_KiemToan101102", new { ParentID = ParentID }))
        {
    %>
    <div class="box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Báo cáo Quyết toán</span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="Div1">
            <div id="Div2">
                <table width="100%" cellpadding="0" cellspacing="0" border="0" class="table_form2">
                    <tr>
                         <td width="20%"></td>
                        <td class="td_form2_td1" style="width: 10%;">
                            <div>
                                <%=NgonNgu.LayXau("Chọn năm ngân sách")%></div>
                        </td>
                        <td width="10%">
                            <div>
                                <%=MyHtmlHelper.DropDownList(ParentID, slNamNganSach, iID_MaNamNganSach, "iID_MaNamNganSach", "", "class=\"input1_2\" style=\"width: 100%\" onchange=\"ChonQuy()\"")%>
                            </div>
                        </td>
                    
                        <td class="td_form2_td1" style="width: 10%">
                            <div>
                                <%=NgonNgu.LayXau("Chọn năm làm việc: ")%></div>
                        </td>
                       <td width="10%">
                            <div>
                                <%=MyHtmlHelper.DropDownList(ParentID, slNamLamViec, iNamLamViec, "iNamLamViec", "", "class=\"input1_2\" style=\"width: 100%\" onchange=\"ChonQuy()\"")%>
                            </div>
                        </td>
                   
                    
                   
                </tr>
                     <tr>
                        <td>
                        </td>
                         
                        <td colspan="6"><table cellpadding="0" cellspacing="0" border="0" align="left" style="margin: 10px;" width="100%">
                            <tr>
                                    <td style="width: 49%;" align="right">
                                        <input type="submit" class="button" id="Submit1" value="<%=NgonNgu.LayXau("Thực hiện")%>" />                                    </td>
                                    <td width="2%">                                    </td>
                                    <td style="width: 49%;" align="left">
                                        <input class="button" type="button" value="<%=NgonNgu.LayXau("Hủy")%>" onclick="Huy()" />                                    </td>
                                </tr>
                           </table></td> 
<td>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
     <%=MyHtmlHelper.ActionLink(Url.Action("ExportToExcel", "rptQuyetToan_KiemToan101102", new { iNamLamViec = iNamLamViec, iID_MaNamNganSach = iID_MaNamNganSach }), "Xuất ra Excels")%>
     
        
    <%} %>
       
    <iframe src="<%=URLView%>" height="600px" width="100%"></iframe>
</body>
</html>
