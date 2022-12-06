<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="System.ComponentModel" %>

<%      
    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(Model);
    //Cập nhập các thông tin tìm kiếm
    Boolean CoTabIndex = (props["CoTabIndex"] == null) ? false : Convert.ToBoolean(props["CoTabIndex"].GetValue(Model));
    String MaND = Convert.ToString(props["MaND"].GetValue(Model));
    String ControlID = Convert.ToString(props["ControlID"].GetValue(Model));
    String ParentID = ControlID + "_Search";
    String IPSua = Request.UserHostAddress;
        
    String DSTruong = "sMaNguoiDung,iID_MaDonVi";
    String strDSTruongDoRong = "150,1200";
    String[] arrDSTruong = DSTruong.Split(',');
    String[] arrDSTruongDoRong = strDSTruongDoRong.Split(',');
    Dictionary<String, String> arrGiaTriTimKiem = new Dictionary<string, string>();
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
                    String strAttr = "";                   
                    strAttr = String.Format("class='input1_4' onkeypress='jsNDDV_Search_onkeypress(event)' search-control='1' search-field='{1}' style='width:{0}px;height:22px;'", iColWidth - 2, arrDSTruong[j]);
                    
                    if (CoTabIndex)
                    {
                        strAttr += String.Format(" tab-index='-1'");
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
            <iframe id="ifrChiTiet" width="100%" height="538px" src="<%= Url.Action("NguoiDung_DonVi_Index_Frame", "NguoiDung_DonVi")%>"></iframe>                    
        </div>

    </div>             