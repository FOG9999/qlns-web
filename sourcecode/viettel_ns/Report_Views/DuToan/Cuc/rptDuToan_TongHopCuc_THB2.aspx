<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="VIETTEL.Report_Controllers.DuToan" %>
<%@ Import Namespace="System.Data.SqlClient" %>
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
    String iID_MaPhongBan = Convert.ToString(ViewData["iID_MaPhongBan"]);
    String MaND = User.Identity.Name;
    String iNamLamViec = ReportModels.LayNamLamViec(MaND);
    if (PageLoad == "1")
    {
        URLView = Url.Action("ViewPDF", "rptDuToan_TongHopCuc_THB2", new { MaND = MaND, iID_MaPhongBan = iID_MaPhongBan });
    }  
    String BackURL = Url.Action("Index", "DuToan_Report", new { sLoai="0"});
    DataTable dtPhongBan = DuToan_ReportModels.getDSPhongBan(iNamLamViec, MaND,"1,2,4");
    SelectOptionList slPhongBan = new SelectOptionList(dtPhongBan, "iID_MaPhongBan", "sTenPhongBan");
    dtPhongBan.Dispose();

    String sMoTa = Convert.ToString(ViewData["sMoTa"]);
    if (String.IsNullOrEmpty(sMoTa))
    {
        String SQL = String.Format("SELECT sTen FROM  KT_DanhMucThamSo_BaoCao WHERE sTenBaoCao=@sTenBaoCao AND iNamLamViec=@iNamLamViec");
        SqlCommand cmd = new SqlCommand(SQL);
        cmd.Parameters.AddWithValue("@sTenBaoCao", "rptDuToan_TongHop_THB2");
        cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
        sMoTa = Connection.GetValueString(cmd, "");
    }

    String iID_MaPhongBanND = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);
    String disable = "";
    if (iID_MaPhongBanND != "02" && iID_MaPhongBanND != "11")
    {
        disable="disabled=\"disabled\"";
    }
    using (Html.BeginForm("EditSubmit", "rptDuToan_TongHopCuc_THB2", new { ParentID = ParentID }))
    {
    %>   
    <div class="box_tong">
         <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
        	    <tr>
            	    <td>
                	    <span>Báo cáo tổng hợp chi ngân sách </span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="Div1">
            <div id="Div2">
                 <table width="100%" cellpadding="0"  cellspacing="0" border="0" class="table_form2">
                    <tr>
                     <td class="td_form2_td1" style="width: 30%">
                            <div>
                               <b>Chọn phòng ban: </b>  </div>
                        </td>
                        <td style="width:60%">
                            <div> <%=MyHtmlHelper.DropDownList(ParentID, slPhongBan, iID_MaPhongBan, "iID_MaPhongBan", "", "class=\"input1_2\" style=\"width: 20%;height:24px;\"")%>    
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="td_form2_td1" style="width: 30%">
                            <div>
                                Mô tả:</div>
                        </td>
                        <td>
                            <div>
                                <%=MyHtmlHelper.TextArea(ParentID, sMoTa, "sMoTa", "", "class=\"input1_2\" style=\"width: 100%;height:80px;\" onchange=\"changeTest(this.value)\"" + disable)%>
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
          function changeTest(value) {
              var sMoTa = document.getElementById("<%= ParentID %>_sMoTa").value;

              var arrGhiChu = sMoTa.split('\n');

              sMoTa = "";
              for (var i = 0; i < arrGhiChu.length; i++) {
                  sMoTa += arrGhiChu[i] + '^';
              }
              var url = unescape('<%= Url.Action("Update_GhiChu?sMoTa=#0", "rptDuToan_TongHopCuc_THB2") %>');
              url = unescape(url.replace("#0", sMoTa));
              


              $.getJSON(url, function (data) {

              });
          }
    </script> 
    <%=MyHtmlHelper.ActionLink(Url.Action("ExportToExcel", "rptDuToan_TongHopCuc_THB2", new { MaND = MaND, iID_MaPhongBan = iID_MaPhongBan }), "Export to Excel")%>
      <iframe src="<%=URLView%>" height="600px" width="100%"></iframe>
</body>
</html>
