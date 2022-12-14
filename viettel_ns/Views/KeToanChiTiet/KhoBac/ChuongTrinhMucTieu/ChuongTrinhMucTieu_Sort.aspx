<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">
    function moveOptionUp(obj) {
        var selectedCount = 0;
        for (i = 0; i < obj.options.length; i++) {
            if (obj.options[i].selected) {
                selectedCount++;
            }
        }
        if (selectedCount > 1) {
            return;
        }
        var i = obj.selectedIndex;
        if (i == 0) {
            return;
        }
        swapOptions(obj, i, i - 1);
        obj.options[i - 1].selected = true;
    }

    function moveOptionDown(obj) {
        var selectedCount = 0;
        for (i = 0; i < obj.options.length; i++) {
            if (obj.options[i].selected) {
                selectedCount++;
            }
        }
        if (selectedCount > 1) {
            return;
        }
        var i = obj.selectedIndex;
        if (i == (obj.options.length - 1)) {
            return;
        }
        swapOptions(obj, i, i + 1);
        obj.options[i + 1].selected = true;
    }

    function swapOptions(obj, i, z) {

        var o = obj.options;
        var i_selected = o[i].selected;
        var z_selected = o[z].selected;

        var temp = new Option(o[i].text, o[i].value, o[i].defaultSelected, o[i].selected);
        var temp2 = new Option(o[z].text, o[z].value, o[z].defaultSelected, o[z].selected);
        o[i] = temp2;
        o[z] = temp;
        o[i].selected = z_selected;
        o[z].selected = i_selected;
    }

    function checkOrderSubmit(objText, obj) {
        doicho(objText, obj);
        return true;
    }

    function doicho(objText, obj) {
        var k;
        var tx;
        tx = "";

        for (k = 0; k < obj.options.length; ++k) {
            tx = tx + obj.options[k].value + "$";
        }

        objText.value = tx;
    }
</script>
<%
    SqlCommand cmd = new SqlCommand();
    cmd.CommandText = "SELECT * FROM KT_ChuongTrinhMucTieu WHERE iTrangThai = 1 ORDER BY iSTT";
    DataTable dt = Connection.GetDataTable(cmd);
    cmd.Dispose();
    using (Html.BeginForm("SortSubmit", "KTCT_KhoBac_ChuongTrinhMucTieu"))
    {   
%>
<table cellpadding="0" cellspacing="0" border="0" width="100%">
    <tr>
        <td style="width: 10%">
            <div style="padding-left: 20px; padding-top: 5px; padding-bottom: 5px; text-transform:uppercase; color:#ec3237;">
                <b><%=NgonNgu.LayXau("Liên kết nhanh: ")%></b>
            </div>         
        </td>
        <td align="left">
            <div style="padding-top: 5px; padding-bottom: 5px; color:#ec3237;">
                <%=MyHtmlHelper.ActionLink(Url.Action("Index", "Home"), "Trang chủ")%> |
                <%=MyHtmlHelper.ActionLink(Url.Action("Index", "KTCT_KhoBac_ChuongTrinhMucTieu"), "Chương trình mục tiêu")%>
            </div>
        </td>
    </tr>
</table>
<%--<div id="divMenuLeft" style="width: 20%; float:left; position:relative;">
    <%Html.RenderPartial("~/Views/KeToanChiTiet/KhoBac/KTCT_KhoBac_Menu.ascx"); %>
</div>--%>
<div id="divNoiDung" style="width: 100%; float:left; position:relative;">
<div class="custom_css box_tong">
    <div class="title_tong">
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
        	<tr>
            	<td>
                	<span>Sắp xếp danh sách chương trình mục tiêu</span>
                </td>
                <td>
                    <table cellpadding="0" cellspacing="0" border="0" width="100%">
	                    <tr>
		                    <td width="30%" align="right">
		                        <div style="display:none;">
                                    <input id="btnLuu" onclick="javascript:checkOrderSubmit(hiddenOrder, SOrder);" type="submit" value="Lưu" />
                                    <script type="text/javascript">
                                        function btnLuu_click() {
                                            document.getElementById('btnLuu').click();
                                            return false;
                                        }
                                    </script>
                                </div>
                                <table cellpadding="0" cellspacing="0" border="0" align="right">
        	                        <tr>
            	                        <td>
            	                          <%--  <a href="#" onclick="javascript:return btnLuu_click();" class="button4"><%=NgonNgu.LayXau("Lưu")%></a>--%>
                                                   <button class="btn btn-primary" type="button" onclick="javascript:return btnLuu_click();"><i class="fa fa-download"></i> Lưu</button>
            	                        </td>
                                        <td width="5px"></td>
                                        <td>
                                            <%--<input type="button" value="Hủy" class="button4" onclick="javascript:history.go(-1);" />--%>
                                            <button class="btn btn-default" type="button" onclick="javascript:history.go(-1)" ><i class="fa fa-ban"></i> Hủy</button>
                                        </td>
                                    </tr>
                                </table>
		                    </td>
	                    </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <div id="nhapform">
        <div id="form2">
            <table cellpadding="0" cellspacing="0" border="0" class="table_form3">
                <tr>
                    <td>
                        <input type='hidden' name='hiddenOrder'>
                        <select name="SOrder" size="20" style="width: 100%">
                            <%
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                            DataRow Row = dt.Rows[i];
                            string strSelected = null;
                            if (i == 0)
                            {
                                strSelected = "selected";
                            }%>                                       
                                <option value="<%= Row["iID_MaChuongTrinhMucTieu"]%>" "<%= strSelected%>"><%= Row["iID_MaChuongTrinhMucTieu"]%> - <%= Row["sTen"]%></option>
                            <%}
                            dt.Dispose();    
                            %>                                 
                        </select>
                    </td>
                    <td>
                        <input type='button' class='btn btn-primay textbox' value='↑' onClick='moveOptionUp(SOrder);'><br><br>
                        <input type='button' class='btn btn-primay textbox' value='↓' onClick='moveOptionDown(SOrder);'><br><br>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>
</div>
<%}%> 
</asp:Content>

