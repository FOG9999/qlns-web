<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="System.ComponentModel" %>
<%
    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(Model);
    Boolean CoTabIndex = (props["CoTabIndex"] == null) ? false : Convert.ToBoolean(props["CoTabIndex"].GetValue(Model));
    String ControlID = Convert.ToString(props["ControlID"].GetValue(Model));
    String ParentID = ControlID + "_Search";
    String iID_MaCapPhat = Request.QueryString["iID_MaCapPhat"];

    //HungPX: lấy giá trị loại chi tiết đến của chứng từ
    DataTable dtCapPhat = CapPhat_ChungTuModels.LayChungTuCapPhat(iID_MaCapPhat);
    String sLoai = Convert.ToString(dtCapPhat.Rows[0]["sLoai"]);
    String Loai = Convert.ToString(dtCapPhat.Rows[0]["iLoai"]);
    String DonVi = Convert.ToString(dtCapPhat.Rows[0]["iID_MaDonVi"]);
    int indexChiTietDen = CapPhatModels.getIndex(sLoai);
    String[] arrDSTruong = MucLucNganSachModels.arrDSTruong;
    String[] arrDSTruongDoRong = MucLucNganSachModels.arrDSTruongDoRong;

    //Hungpx lấy tên và size các cột có mặt trong vùng fix
    string dsTruong = "", dsTruongDoRong = "";

    if (Loai == "4")
    {
        dsTruong = "sNG,";
        dsTruongDoRong = "60,";
    }
    else
    {
        for (int i = 0; i <= indexChiTietDen; i++)
        {
            dsTruong += arrDSTruong[i] + ",";
            dsTruongDoRong += arrDSTruongDoRong[i] + ",";
        }
    }
    if (String.IsNullOrEmpty(DonVi))
    {
        dsTruong += "sMoTa,iID_MaDonVi";
        dsTruongDoRong += "280,150";
    }
    else
    {
        dsTruong += "sMoTa";
        dsTruongDoRong += "280";
    }

    //Cập nhập các thông tin tìm kiếm
    string _dsTruong = dsTruong;
    string _dsTruongDoRong = dsTruongDoRong;

    String DSTruong = _dsTruong;
    arrDSTruong = DSTruong.Split(',');
    String strDSTruongDoRong = _dsTruongDoRong;
    arrDSTruongDoRong = strDSTruongDoRong.Split(',');


    Dictionary<String, String> arrGiaTriTimKiem = new Dictionary<string, string>();
    for (int i = 0; i < arrDSTruong.Length; i++)
    {
        arrGiaTriTimKiem.Add(arrDSTruong[i], Request.QueryString[arrDSTruong[i]]);
    }
%>
<div class="box_tong">
    <div id="nhapform">
        <div id="form2">
            <form action="<%=Url.Action("SearchSubmit","CapPhat_ChungTuChiTiet",new {ParentID = ParentID, iID_MaCapPhat = iID_MaCapPhat})%>"
                method="post">   
                <table class="mGrid1" style="margin-left:2px">
                    <tr>
                        <%
                            for (int j = 0; j < arrDSTruong.Length; j++)
                            {
                                int iColWidth = Convert.ToInt32(arrDSTruongDoRong[j]) + 1;
                                if (arrDSTruong[j] == "iID_MaDonVi") iColWidth = iColWidth + 2;
                                String strAttr = String.Format("class='input1_4' onkeypress='jsCapPhat_Search_onkeypress(event)' search-control='1' search-field='{1}' style='width:{0}px;height:22px;margin-left:1px;'", iColWidth, arrDSTruong[j]);
                                if (CoTabIndex)
                                {
                                    strAttr += " tab-index='-1'";
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
                <iframe id="ifrChiTietChungTu" width="100%" height="530px" src="<%= Url.Action("CapPhatChiTiet_Frame", "CapPhat_ChungTuChiTiet", new {iID_MaCapPhat=iID_MaCapPhat})%>"></iframe>
            </form>
        </div>
    </div>
</div>
