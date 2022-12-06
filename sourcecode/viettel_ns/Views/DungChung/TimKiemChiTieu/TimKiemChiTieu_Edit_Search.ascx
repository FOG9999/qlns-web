<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models.TimKiemChiTieu" %>
<%@ Import Namespace="System.ComponentModel" %>

<%       
    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(Model);
    //Cập nhập các thông tin tìm kiếm
    Boolean CoTabIndex = (props["CoTabIndex"] == null) ? false : Convert.ToBoolean(props["CoTabIndex"].GetValue(Model));
    String MaND = Convert.ToString(props["MaND"].GetValue(Model));
    String ControlID = Convert.ToString(props["ControlID"].GetValue(Model));
    String IPSua = Request.UserHostAddress;
    String ParentID = ControlID + "_Search";
        
    String DSTruong = "dDotNgay,sQD," + ChiTieuModels.strDSTruong + ",iID_MaPhongBan,sID_MaNguoiDungTao";
    String strDSTruongDoRong = ChiTieuModels.strDSTruongDoRong; 
    String[] arrDSTruong = DSTruong.Split(',');
    String[] arrDSTruongDoRong = strDSTruongDoRong.Split(',');
    Dictionary<String, String> arrGiaTriTimKiem = new Dictionary<string, string>();
    for (int i = 0; i < arrDSTruong.Length; i++)
    {
        arrGiaTriTimKiem.Add(arrDSTruong[i], Request.QueryString[arrDSTruong[i]]);
    }

    TimKiemChiTieu_BangDuLieu bang = new TimKiemChiTieu_BangDuLieu(arrGiaTriTimKiem, MaND, IPSua);       
    
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
                    String strAttr = String.Format("class='input1_4' onkeypress='jsTimKiemCT_Search_onkeypress(event)' search-control='1' search-field='{1}' style='width:{0}px;height:22px;'", iColWidth - 2, arrDSTruong[j]);                    
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
            <iframe id="ifrChiTietMLNS" width="100%" height="538px" src="<%= Url.Action("TimKiemChiTieu_Frame", "TimKiemChiTieu")%>"></iframe>                    
        </div>

    </div>             