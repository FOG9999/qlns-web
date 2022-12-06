<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="Viettel.Services" %>
<%@ Import Namespace="System.ComponentModel" %>

<%
    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(Model);
    string ControlID = Convert.ToString(props["ControlID"].GetValue(Model));
    bool CoTabIndex = (props["CoTabIndex"]==null) ? false:Convert.ToBoolean(props["CoTabIndex"].GetValue(Model));
    string ParentID = ControlID + "_Search";
    string MaND = Convert.ToString(props["MaND"].GetValue(Model));
    string Id_CTCap = Convert.ToString(Request.QueryString["Id_CTCap"]);

    INguonNSService _nguonNSService = NguonNSService.Default;

    var data = _nguonNSService.GetChungTu("Nguon_CTChi",Id_CTCap);
        
    string[] arrDSTruong = "CTMT,L,K,Ma_Nguon".Split(',');
    string[] arrDSTruongDoRong = "60,60,60,280".Split(',');
    Dictionary<string,string> arrGiaTriTimKiem = new Dictionary<string,string>();
    for (int i = 0; i < arrDSTruong.Length; i++)
    {
        arrGiaTriTimKiem.Add(arrDSTruong[i], Request.QueryString[arrDSTruong[i]]);
    }

%>

<div class="box_tong">
    <div id="nhapform">
        <div id="form2">
            <table class="mGrid1">
            <tr>
                <%
                    for (int j = 0; j < arrDSTruong.Length; j++)
                    {
                        int iColWidth = Convert.ToInt32(arrDSTruongDoRong[j]) + 4;
                        if (j == 0) iColWidth = iColWidth + 3;
                        string strAttr = string.Format("class='input1_4' onkeypress='jsCTCap_CT_Search_onkeypress(event)' search-control='1' search-field='{1}' style='width:{0}px;height:22px;'", iColWidth - 2, arrDSTruong[j]);
                        if (CoTabIndex)
                        {
                            strAttr += string.Format(" tab-index='-1'");
                        }
                %>
                        <td style="text-align: left; width: <%=iColWidth%>px;">
                            <%=MyHtmlHelper.TextBox(new { ParentID = ParentID, Value = arrGiaTriTimKiem[arrDSTruong[j]], TenTruong = arrDSTruong[j], LoaiTextBox = "2", Attributes = strAttr })%>
                        </td>
                <%
                    }
                %>
            </tr>
            </table>
            <iframe id="ifrChiTietChungTu" width="100%" height="538px" src="<%= Url.Action("ChungTuChiTiet_Frame", "ChungTuCap_ChungTuChiTiet", new {Id_CTCap=Id_CTCap})%>"></iframe>
        </div>
    </div>
</div>