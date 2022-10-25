<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="System.ComponentModel" %>

<%
    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(Model);
    String ControlID = Convert.ToString(props["ControlID"].GetValue(Model));
    Boolean CoTabIndex = (props["CoTabIndex"]==null) ? false:Convert.ToBoolean(props["CoTabIndex"].GetValue(Model));
    String ParentID = ControlID + "_Search";
    String MaND = Convert.ToString(props["MaND"].GetValue(Model));
    String IPSua = Request.UserHostAddress;
    String iID_MaChungTu = Convert.ToString(Request.QueryString["iID_MaChungTu"]);
    String sLNS = Convert.ToString(props["sLNS"].GetValue(Model));
    if (String.IsNullOrEmpty(sLNS))
    {
        sLNS = Convert.ToString(Request.QueryString["sLNS"]);
    }
    String iLoai = Convert.ToString(props["iLoai"].GetValue(Model));
    String iChiTapTrung = Convert.ToString(props["iChiTapTrung"].GetValue(Model));
    Boolean ChucNangCapNhap = (props["ChucNangCapNhap"] == null) ? false : Convert.ToBoolean(props["ChucNangCapNhap"].GetValue(Model));
    NameValueCollection dt;
    if (iLoai == "4")
    {
        dt = DuToan_ChungTuModels.LayThongTin_KyThuatLan2(iID_MaChungTu);
    }
    else {
        dt = DuToan_ChungTuModels.LayThongTin(iID_MaChungTu);
    }

    //Cập nhập các thông tin tìm kiếm
    String iID_MaDonVi = Convert.ToString(dt["iID_MaDonVi"]);
    String strDSTruong = MucLucNganSachModels.strDSTruong;
    String strDSTruongDoRong = MucLucNganSachModels.strDSTruongDoRong;
    if (String.IsNullOrEmpty(iID_MaDonVi))
    {
        strDSTruong += ",iID_MaDonVi";
        strDSTruongDoRong += ",200";
    }
    if (iChiTapTrung == "1")
    {
        strDSTruong = MucLucNganSachModels.strDSTruong;
        strDSTruongDoRong = MucLucNganSachModels.strDSTruongDoRong;
    }
    //}
    //else
    //{
    //    if (sLNS.Substring(0, 2) == "80")
    //    {
    //        strDSTruong = "iID_MaDonVi," + MucLucNganSachModels.strDSTruong_80;
    //        strDSTruongDoRong = "250," + MucLucNganSachModels.strDSTruongDoRong_80;
    //    }
    //    else
    //    {


    //        if (sLNS.Length == 7)
    //        {
    //            strDSTruong = "iID_MaDonVi," + MucLucNganSachModels.strDSTruong_LNS;
    //            strDSTruongDoRong = "250," + MucLucNganSachModels.strDSTruongDoRong_LNS;
    //        }
    //        else
    //        {
    //            strDSTruong = "iID_MaDonVi," + MucLucNganSachModels.strDSTruong_LNS_New;
    //            strDSTruongDoRong = "250," + MucLucNganSachModels.strDSTruongDoRong_LNS_New;
    //        }
    //    }
    //}
    String[] arrDSTruong = strDSTruong.Split(',');
    String[] arrDSTruongDoRong = strDSTruongDoRong.Split(',');
    Dictionary<String,String> arrGiaTriTimKiem = new Dictionary<string,string>();
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
                        if (String.IsNullOrEmpty(iID_MaDonVi)) {
                            if (iChiTapTrung == "1")
                            {
                                if (j == arrDSTruong.Length - 2)
                                    continue;
                            }
                            else
                            {
                                if (j == arrDSTruong.Length - 3)
                                    continue;
                            }
                        }
                        else
                        {
                            if (iChiTapTrung == "1")
                            {
                                if (j == arrDSTruong.Length - 1)
                                    continue;
                            }
                            else
                            {
                                if (j == arrDSTruong.Length - 2)
                                    continue;
                            }
                        }
                        if (arrDSTruong[j].ToString() == "sTNG")
                        {
                            break;
                        }
                        int iColWidth = Convert.ToInt32(arrDSTruongDoRong[j]) + 4;
                        if (j == 0) iColWidth = iColWidth + 3;
                        String strAttr = String.Format("class='input1_4' onkeypress='jsDuToan_Search_onkeypress(event)' search-control='1' search-field='{1}' style='width:{0}px;height:22px;'", iColWidth - 2, arrDSTruong[j]);
                        if (CoTabIndex)
                        {
                            strAttr += String.Format(" tab-index='-1'");
                        }
                    %>
                    <%if(iLoai!="4"){ %>
                    <td style="text-align:left;width:<%=iColWidth%>px;">
                        <%=MyHtmlHelper.TextBox(new { ParentID = ParentID, Value = arrGiaTriTimKiem[arrDSTruong[j]], TenTruong = arrDSTruong[j], LoaiTextBox = "2", Attributes = strAttr })%>
                    </td>
                    <%} %>
                    <%
                }
                %>
            </tr>
            </table>
            <iframe id="ifrChiTietChungTu" width="100%" height="538px" src="<%= Url.Action("ChungTuChiTiet_Frame", "DuToan_ChungTuChiTiet", new {iID_MaChungTu=iID_MaChungTu, iID_MaDonVi=iID_MaDonVi, sLNS=sLNS,iLoai=iLoai,iChiTapTrung=iChiTapTrung})%>"></iframe>
        </div>
    </div>
</div>