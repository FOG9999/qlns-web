<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="System.ComponentModel" %>

<%        
    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(Model);
    //Cập nhập các thông tin tìm kiếm
    bool CoTabIndex = (props["CoTabIndex"] == null) ? false : Convert.ToBoolean(props["CoTabIndex"].GetValue(Model));
    string ControlID = Convert.ToString(props["ControlID"].GetValue(Model));
    string ParentID = ControlID + "_Search";
    string IPSua = Request.UserHostAddress;
        
    string DSTruong = "CTMT,L,K,Ma_Nguon";
    string strDSTruongDoRong = "60,60,60,100";
    string[] arrDSTruong = DSTruong.Split(',');
    string[] arrDSTruongDoRong = strDSTruongDoRong.Split(',');
    Dictionary<string, string> arrGiaTriTimKiem = new Dictionary<string, string>();
    for (int i = 0; i < arrDSTruong.Length; i++)
    {
        arrGiaTriTimKiem.Add(arrDSTruong[i], Request.QueryString[arrDSTruong[i]]);
    }
    %>  
    <div id="nhapform">
        <div id="form2">            
            <table class="mGrid1">
            <tr>
                <%
                for (int j = 0; j < arrDSTruong.Length; j++)
                {                                
                    int iColWidth = Convert.ToInt32(arrDSTruongDoRong[j]) + 4;
                    if (j == 0) iColWidth = iColWidth + 3;
                    string strAttr = "";                   
                    strAttr = string.Format("class='input1_4' onkeypress='jsDMBTC_Search_onkeypress(event)' search-control='1' search-field='{1}' style='width:{0}px;height:22px;'", iColWidth - 2, arrDSTruong[j]);
                    
                    if (CoTabIndex)
                    {
                        strAttr += string.Format(" tab-index='-1'");
                    }
                    %> 
                    <td style="text-align:left;width:<%=iColWidth%>px;">
                        <%=MyHtmlHelper.TextBox(new { ParentID = ParentID, Value = arrGiaTriTimKiem[arrDSTruong[j]], TenTruong = arrDSTruong[j], LoaiTextBox = "2", Attributes = strAttr })%>
                    </td>                               
                    <%
                }
                %>
            </tr>
            </table>
            <iframe id="ifrChiTiet" width="100%" height="538px" src="<%= Url.Action("Nguon_DMBTC_Edit_Frame", "Nguon_DMBTC")%>"></iframe>  
        </div>

    </div>             