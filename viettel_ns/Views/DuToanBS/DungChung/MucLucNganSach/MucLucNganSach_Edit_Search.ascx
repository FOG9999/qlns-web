<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="System.ComponentModel" %>

<%        
    String iID_MaMucLucNganSach = Request.QueryString["iID_MaMucLucNganSach"];
    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(Model);
    //Cập nhập các thông tin tìm kiếm
    Boolean CoTabIndex = (props["CoTabIndex"] == null) ? false : Convert.ToBoolean(props["CoTabIndex"].GetValue(Model));
    String MaND = Convert.ToString(props["MaND"].GetValue(Model));
    String ControlID = Convert.ToString(props["ControlID"].GetValue(Model));
    String IPSua = Request.UserHostAddress;
    String ParentID = ControlID + "_Search";
        
    String DSTruong = MucLucNganSachModels.strDSTruong;
    String strDSTruongDoRong = MucLucNganSachModels.strDSTruongDoRong;
    //MucLucNganSachModels.CapNhapLai();
    String[] arrDSTruong = DSTruong.Split(',');
    String[] arrDSTruongDoRong = strDSTruongDoRong.Split(',');
    Dictionary<String, String> arrGiaTriTimKiem = new Dictionary<string, string>();
    String sLNS = Request.QueryString["sLNS"];    
    if (String.IsNullOrEmpty(sLNS)) sLNS = "0";
    for (int i = 0; i < arrDSTruong.Length; i++)
    {
        arrGiaTriTimKiem.Add(arrDSTruong[i], Request.QueryString[arrDSTruong[i]]);
    }
    if (sLNS == "0")
    {
        arrGiaTriTimKiem["sLNS"] = null;
    }
    DataTable dtsLNS = HamChung.getSLNS();
    SelectOptionList slLNS = new SelectOptionList(dtsLNS, "sLNS", "TenLNS");
    MucLucNganSach_BangDuLieu bang = new MucLucNganSach_BangDuLieu(arrGiaTriTimKiem, MaND, IPSua);
    dtsLNS.Dispose();

    String DetailSubmit = Url.Action("DetailSubmit", "MucLucNganSach", new { sLNS = sLNS });
    
    %>  
    <div id="nhapform">
        <div id="form2">
            <form action="<%=Url.Action("LocSubmit","MucLucNganSach",new {ParentID = ParentID, iID_MaMucLucNganSach = iID_MaMucLucNganSach})%>"
            method="post">
            <table class="mGrid1">
                <tr>
                    <td>
                        <%=MyHtmlHelper.DropDownList(ParentID, slLNS, sLNS, "sLNS", "","style=\"width:400px;\"")%>
                    </td>
                    <td style="padding: 1px 5px; text-align: left;">
                        <input type="submit" id="<%=ParentID%>_btnTimKiem" <%=bang.DuocSuaChiTiet? "":"tab-index='-1'" %>
                            value="<%=NgonNgu.LayXau("Tìm kiếm")%>" style="font-size: 11px; padding: 0px 3px;"
                            class="button4" />
                    </td>
                </tr>
            </table>
            </form>
            <table class="mGrid1">
            <tr>
                <%
                for (int j = 0; j < arrDSTruong.Length; j++)
                {                                
                    int iColWidth = Convert.ToInt32(arrDSTruongDoRong[j]) + 4;
                    if (j == 0) iColWidth = iColWidth + 3;
                    String strAttr = "";
                    if (arrDSTruong[j] == "sLNS" && !String.IsNullOrEmpty(arrGiaTriTimKiem["sLNS"]))
                    {
                        if (arrDSTruong[j] == "sLNS" && arrGiaTriTimKiem["sLNS"].Length == 7)
                        {
                            strAttr = String.Format("class='input1_4' onkeypress='jsMLNS_Search_onkeypress(event)' search-control='1' search-field='{1}' style='width:{0}px;height:22px;' readonly='readonly'", iColWidth - 2, arrDSTruong[j]);
                        }
                        else
                        {
                            strAttr = String.Format("class='input1_4' onkeypress='jsMLNS_Search_onkeypress(event)' search-control='1' search-field='{1}' style='width:{0}px;height:22px;'", iColWidth - 2, arrDSTruong[j]);
                        }
                    }
                    else
                    {
                        strAttr = String.Format("class='input1_4' onkeypress='jsMLNS_Search_onkeypress(event)' search-control='1' search-field='{1}' style='width:{0}px;height:22px;'", iColWidth - 2, arrDSTruong[j]);
                    }
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
            <iframe id="ifrChiTietMLNS" width="100%" height="538px" src="<%= Url.Action("MLNSChiTiet_Frame", "MucLucNganSach", new { sLNS = sLNS})%>"></iframe>                    
        </div>

    </div>             