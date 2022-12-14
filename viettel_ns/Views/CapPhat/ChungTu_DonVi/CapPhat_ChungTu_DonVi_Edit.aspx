<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="DomainModel.Abstract" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="VIETTEL.Controllers.CapPhat" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

 <%
    String ParentID = "Edit";
    String UserID = User.Identity.Name;
    String iID_MaCapPhat = Convert.ToString(ViewData["iID_MaCapPhat"]);    
    String MaPhongBanNguoiDung = NganSach_HamChungModels.MaPhongBanCuaMaND(UserID);
    DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(UserID);
    String iNamLamViec = Convert.ToString(dtCauHinh.Rows[0]["iNamLamViec"]);
    dtCauHinh.Dispose();
    
    String Loai = Convert.ToString(Request.QueryString["Loai"]);

    if(String.IsNullOrEmpty(Loai)){
        Loai = Convert.ToString(ViewData["Loai"]);
    }
    
     //Danh sách đơn vị
    DataTable dtDonVi = NganSach_HamChungModels.DSDonViCuaNguoiDung(UserID);
    SelectOptionList slDonVi = new SelectOptionList(dtDonVi, "iID_MaDonVi", "sTen");
    dtDonVi.Dispose();

    ////Danh sách loại cấp phát 
    //DataTable dtLoaiCapPhat = CommonFunction.Lay_dtDanhMuc("LoaiCapPhat");
    //SelectOptionList slLoaiCapPhat = new SelectOptionList(dtLoaiCapPhat, "iID_MaDanhMuc", "sTen");
    //dtLoaiCapPhat.Dispose();
     
    ////Danh sách tính chất cấp thu
    //DataTable dtTinhChatCapThu = TinhChatCapThuModels.Get_dtTinhChatCapThu();
    //SelectOptionList slTinhChatCapThu = new SelectOptionList(dtTinhChatCapThu, "iID_MaTinhChatCapThu", "sTen");
    //DataRow R2 = dtTinhChatCapThu.NewRow();
    //R2["iID_MaTinhChatCapThu"] = "-1";
    //R2["sTen"] = "--- Danh sách tính chất cấp thu ---";
    //dtTinhChatCapThu.Rows.InsertAt(R2, 0);
    //dtTinhChatCapThu.Dispose();

    //Danh sách loại ngân sách
    DataTable dtLNS = new DataTable();
    if (Loai == "1")
    {
        dtLNS = DanhMucModels.NS_LoaiNganSachQuocPhong(MaPhongBanNguoiDung);
    }
    else if (Loai == "2")
    {
        dtLNS = DanhMucModels.NS_LoaiNganSachNhaNuoc_PhongBan(MaPhongBanNguoiDung);
    }
    else
    {
        dtLNS = DanhMucModels.NS_LoaiNganSachKhac_PhongBan(MaPhongBanNguoiDung);
    }
    SelectOptionList slLNS = new SelectOptionList(dtLNS, "sLNS", "TenHT");  
    dtLNS.Dispose();

    //Danh sách chi tiết đến
    DataTable dtLoai = CapPhat_ChungTuModels.LayLoaiNganSachCon();
    SelectOptionList slLoai = new SelectOptionList(dtLoai, "iID_Loai", "TenHT");
    dtLoai.Dispose();
     
    //Khai báo biến
    String iID_MaTinhChatCapThu = "";
    String iID_MaDonVi = "";
    String iSoCapPhat = "";
    String sTienToChungTu = "";
    String dNgayCapPhat = "";
    String sNoiDung = "";
    String sLNS = "";
    String sLoai = "";
    String iDM_MaLoaiCapPhat = "";
    String ReadOnly = "";
    //Nếu là thêm mới chứng từ
    if (ViewData["DuLieuMoi"] == "1")
    {
         if (Loai == "1")
         {
             sLoai = "sM";
         }
         else
         {
             sLoai = "sNG";
         }
         sTienToChungTu = PhanHeModels.LayTienToChungTu(CapPhatModels.iID_MaPhanHe);
         iSoCapPhat = Convert.ToString(CapPhat_ChungTuModels.GetMaxSoCapPhat() + 1);
         dNgayCapPhat = CommonFunction.LayXauNgay(DateTime.Now);
     }
     //Nếu là update chứng từ
     else {
         //Lấy thông tin chứng từ cấp phát. 
         DataTable dtCapPhat = CapPhat_ChungTuModels.LayChungTuCapPhat(iID_MaCapPhat);
         if (dtCapPhat != null && dtCapPhat.Rows.Count > 0)
         {
             DataRow R = dtCapPhat.Rows[0];
             iSoCapPhat = Convert.ToString(R["iSoCapPhat"]);
             sTienToChungTu = Convert.ToString(R["sTienToChungTu"]);
             //iDM_MaLoaiCapPhat = Convert.ToString(R["iDM_MaLoaiCapPhat"]);
             //iID_MaTinhChatCapThu = Convert.ToString(R["iID_MaTinhChatCapThu"]);
             iID_MaDonVi = Convert.ToString(R["iID_MaDonVi"]);
             sLNS = Convert.ToString(R["sDSLNS"]);
             sLoai = Convert.ToString(R["sLoai"]);
             dNgayCapPhat = CommonFunction.LayXauNgay(Convert.ToDateTime(R["dNgayCapPhat"]));
             sNoiDung = Convert.ToString(R["sNoiDung"]);
             ReadOnly = "disabled=\"disabled\"";             
             dtCapPhat.Dispose();
         }          
     }
     String[] arrLNS = sLNS.Split(',');

     String BackURL = Url.Action("Index", "CapPhat_ChungTu_DonVi", new { Loai = Loai });
     
     using (Html.BeginForm("LuuChungTu", "CapPhat_ChungTu_DonVi", new { ParentID = ParentID, iID_MaCapPhat = iID_MaCapPhat, Loai = Loai }))
    {
%>
<%= Html.Hidden(ParentID + "_DuLieuMoi", ViewData["DuLieuMoi"])%>
<%= Html.Hidden(ParentID + "_iID_MaPhongBan", MaPhongBanNguoiDung)%>
<div class="box_tong">
    <div class="title_tong">
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
            <tr>
                <td><span>
                    <%
                    if (ViewData["DuLieuMoi"] == "1")
                    {
                        %>
                        <%=NgonNgu.LayXau("Thêm mới chứng từ")%>
                        <%
                    }
                    else
                    {
                        %>
                        <%=NgonNgu.LayXau("Sửa thông tin chứng từ")%>
                        <%
                    }
                    %>&nbsp; &nbsp;
                </span></td>
            </tr>
        </table>
    </div>
    <div id="nhapform">
        <div id="form2">
            <div style="width: 50%; float: left;">
                <table cellpadding="0" cellspacing="0" border="0" width="100%">
                    <tr>
                        <td class="td_form2_td1" style="width: 15%;">
                            <div><b>Số cấp phát</b></div>
                        </td>
                        <td class="td_form2_td5">
                            <div>
                                <b><%=sTienToChungTu %><%=iSoCapPhat%> </b>
                            </div>
                        </td>
                    </tr>
                    <%--<tr>
                        <td class="td_form2_td1">
                            <div><b>Loại cấp phát</b></div>
                        </td>
                        <td class="td_form2_td5">
                            <div >
                                <%=MyHtmlHelper.DropDownList(ParentID, slLoaiCapPhat, iDM_MaLoaiCapPhat, "iDM_MaLoaiCapPhat", "", "style =\"width: 50%\"")%><br />
                                <%= Html.ValidationMessage(ParentID + "_" + "err_iDM_MaLoaiCapPhat")%>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="td_form2_td1">
                            <div><b>Tính chất cấp thu</b></div>
                        </td>
                        <td class="td_form2_td5">
                            <div >
                                <%=MyHtmlHelper.DropDownList(ParentID, slTinhChatCapThu, iID_MaTinhChatCapThu, "iID_MaTinhChatCapThu", "", "style =\"width: 50%\"")%><br />
                                <%= Html.ValidationMessage(ParentID + "_" + "err_iID_MaTinhChatCapThu")%>
                            </div>
                        </td>
                    </tr>--%>
                 
                    <tr>
                        <td class="td_form2_td1">
                            <div><b>Đơn vị</b></div>
                        </td>
                        <td class="td_form2_td5">
                            <div id="<%= ParentID %>_divDonVi" style ="width: 49%">
                                <%= CapPhat_ChungTu_DonViController.LayDoiTuongDonVi_PhongBan(ParentID, UserID, iID_MaDonVi, iNamLamViec)%>
                                <%= Html.ValidationMessage(ParentID + "_" + "err_iID_MaDonVi")%>
                            </div>
                            
                        </td>
                    </tr>
                    <!--VungNV: 2015/10/20 Nếu là loại NSQP thì có thêm dropdown list Chọn LNS:-->
                      <tr>
                        <td class="td_form2_td1">
                            <div>
                                <b>Loại ngân sách</b>&nbsp;</div>
                        </td>
                        <td class="td_form2_td5">
                            <div style="width: 100%; overflow: scroll; height: 250px;">
                                <table class="mGrid"> 
                                    <tr>
                                        <td style="width: 15%; text-align:center">
                                            <input type="checkbox" id="checkAll" onclick="Chonall(this.checked)">
                                        </td>
                                        <td>
                                            Chọn tất cả
                                        </td>
                                    </tr>                                   
                                    <%
                                        String TenLoaiNS = "";
                                        String LoaiNS = "";
                                        String _CheckedC = "checked=\"checked\"";
                                        for (int i = 0; i < dtLNS.Rows.Count; i++)
                                        {
                                            _CheckedC = "";
                                            TenLoaiNS = Convert.ToString(dtLNS.Rows[i]["TenHT"]);
                                            LoaiNS = Convert.ToString(dtLNS.Rows[i]["sLNS"]);
                                            for (int j = 0; j < arrLNS.Length; j++)
                                            {
                                                if (LoaiNS == arrLNS[j])
                                                {
                                                    _CheckedC = "checked=\"checked\"";
                                                    break;
                                                }
                                            }
                                    %>
                                    <tr>
                                        <td style="width: 15%; text-align:center" >
                                            <input type="checkbox" value="<%= LoaiNS %>" <%= _CheckedC %> check-group="sLNS" id="sLNS"
                                                name="sLNS" />
                                        </td>
                                        <td>
                                            <%= TenLoaiNS%>
                                        </td>
                                    </tr>
                                    <% }%>
                                </table>
                            </div>
                        </td>                                             
                    </tr> 
                     <tr>
                        <td class="td_form2_td1">
                            <div><b>Chi tiết đến</b></div>
                        </td>
                        <td class="td_form2_td5">
                            <div><%=MyHtmlHelper.DropDownList(ParentID, slLoai, sLoai, "iID_Loai", "class=\"input1_2\" style=\"height: 100px; width: 30%; \"", ReadOnly)%></div>
                        </td>
                    </tr>

                    <tr>
                        <td class="td_form2_td1">
                            <div><b>Ngày chứng từ</b></div>
                        </td>
                        <td class="td_form2_td5">
                            <div style="width: 35%;">
                                <%=MyHtmlHelper.DatePicker(ParentID, dNgayCapPhat, "dNgayCapPhat", "", "class=\"input1_2\" onblur=isDate(this);")%><br />
                                <%= Html.ValidationMessage(ParentID + "_" + "err_dNgayCapPhat")%>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="td_form2_td1">
                            <div><b>Nội dung</b></div>
                        </td>
                        <td class="td_form2_td5">
                            <div><%=MyHtmlHelper.TextArea(ParentID, sNoiDung, "sNoiDung", "", "class=\"input1_2\" style=\"height: 100px; resize: none;\"")%></div>
                        </td>
                    </tr>
                    <tr>
                        <td class="td_form2_td1"></td>
                        <td class="td_form2_td5">
                            <div>
                                <table cellpadding="0" cellspacing="0" border="0">
                                    <tr>
                                        <td width="65%" class="td_form2_td5">&nbsp;</td>   
                                        <td width="30%" align="right" class="td_form2_td5">
                                            <input type="submit" class="button" id="Submit1" value="Lưu" />
                                        </td>          
                                            <td width="5px">&nbsp;</td>          
                                        <td class="td_form2_td5">
                                            <input class="button" type="button" value="Hủy" onclick="Huy()" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>
<%
    }
%>
<script type="text/javascript">
        function Huy() {
             window.parent.location.href = '<%=BackURL%>';
         }
         function Chonall(value) {
             $("input:checkbox[check-group='sLNS']").each(function (i) {
                 this.checked = value;
             });
         }    
</script>
</asp:Content>

