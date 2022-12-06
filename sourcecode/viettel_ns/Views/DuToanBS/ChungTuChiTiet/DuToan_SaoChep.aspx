<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="VIETTEL.Models.DuToanBS" %>
<%@ Import Namespace="VIETTEL.Report_Controllers.ThuNop" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
</head>
<body>
    <%
        string ParentID = "DuToanBS";
        string iID_MaChungTu = Convert.ToString(Request.QueryString["iID_MaChungTu"]);
        string iLoai = Convert.ToString(Request.QueryString["iLoai"]);
        string MaLoai = Convert.ToString(CommonFunction.LayTruong("DTBS_ChungTu", "iID_MaChungTu", iID_MaChungTu, "MaLoai"));
        DataTable dtChungTu = DuToanBSModels.GetDots(User.Identity.Name,iID_MaChungTu);
        SelectOptionList slChungTu = new SelectOptionList(dtChungTu, "iID_MaChungTu", "sMoTa");     
        dtChungTu.Dispose();
        DataTable dtCapDv = DuToanBSModels.GetCapDV();
        SelectOptionList slCapDv = new SelectOptionList(dtCapDv, "Cap", "TenCap");     
        dtCapDv.Dispose();
        DataTable dtDvt = DuToanBSModels.GetDonviTinh();
        SelectOptionList slDvt = new SelectOptionList(dtDvt, "Dvt", "TenDvt");     
        dtDvt.Dispose();
        string iID_MaChungTuOld = "", len = "", dvt = "";

        using (Html.BeginForm("ChuyenSoLieu", "DuToanBSChungTuChiTiet", new {  ParentID = ParentID}))
        {
    %>
    <%=MyHtmlHelper.Hidden(ParentID,iID_MaChungTu,"iID_MaChungTu","") %>
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td class="td_form2_td1" style="width: 10%"></td>
            <td class="td_form2_td1" style="width: 20%" >
                <div> Chọn số liệu từ chứng từ:</div> 
            </td>
            <td class="td_form2_td1" style="width: 60%">
                <%=MyHtmlHelper.DropDownList(ParentID,slChungTu,iID_MaChungTuOld,"iID_MaChungTuOld","","class=\"input1_2\" style=\"width: 100%\"") %>
            </td>
            <td class="td_form2_td1" style="width: 10%"></td>
        </tr>
        <tr>
            <td class="td_form2_td1" style="width: 10%"></td>
            <td class="td_form2_td1" style="width: 20%" >
                <div> Số liệu chuyển cho đơn vị cấp:</div> 
            </td>
            <td class="td_form2_td1" style="width: 60%">
                <%=MyHtmlHelper.DropDownList(ParentID,slCapDv,len,"len","","class=\"input1_2\" style=\"width: 100%\"") %>
            </td>
            <td class="td_form2_td1" style="width: 10%"></td>
        </tr>
        <tr>
            <td class="td_form2_td1" style="width: 10%"></td>
            <td class="td_form2_td1" style="width: 20%" >
                <div> Số liệu nhận là số:</div> 
            </td>
            <td class="td_form2_td1" style="width: 60%">
                <%=MyHtmlHelper.DropDownList(ParentID,slDvt,dvt,"dvt","","class=\"input1_2\" style=\"width: 100%\"") %>
            </td>
            <td class="td_form2_td1" style="width: 10%"></td>
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
                                <input type="button" class="button" value="Hủy" onclick="Dialog_close('<%=ParentID %>');" />
                            </td>
                            <td style="width: 40%">
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>
    <%} %>
</body>
</html>
