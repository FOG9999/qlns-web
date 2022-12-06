<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="DomainModel.Abstract" %>
<%@ Import Namespace="VIETTEL.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%
        String ParentID = "Edit";
        String UserID = User.Identity.Name;
        String DonVi = Convert.ToString(ViewData["DonVi"]);
        String iID_MaCapPhat = Convert.ToString(ViewData["iID_MaCapPhat"]);
        String sLNS = "";
        String MaPhongBanNguoiDung = NganSach_HamChungModels.MaPhongBanCuaMaND(UserID);

        String Loai = Request.QueryString["Loai"];
        if (String.IsNullOrEmpty(Loai)) Loai = Convert.ToString(ViewData["Loai"]);

        String iID_MaPhongBan = NganSach_HamChungModels.MaPhongBanCuaMaND(UserID);
        DataTable dtLNS = new DataTable();
        DataTable dtNganh = new DataTable();
        if (Loai == "4")
        {
            dtNganh = CapPhatModels.getDanhSachNganh(UserID);
            SelectOptionList slNganh = new SelectOptionList(dtNganh, "iID_MaNganh", "sTenNganh");
            dtNganh.Dispose();
        }
        else
        {
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
        }

        DataTable dtLoai = CapPhat_ChungTuModels.LayLoaiNganSachCon();
        SelectOptionList slLoai = new SelectOptionList(dtLoai, "iID_Loai", "TenHT");

        dtLoai.Dispose();

        DataTable dtCapPhat = CapPhat_ChungTuModels.LayChungTuCapPhat(iID_MaCapPhat);
        DataRow R;
        String sLoai = "", iSoCapPhat = "", sTienToChungTu = "", dNgayCapPhat = "", sNoiDung = "", sLyDo = "", iID_MaTrangThaiDuyet = "", iID_MaDonVi = "", iID_MaTinhChatCapThu = "", iID_NguonDuToan = "";
        // HungPX: Cài đặt TT readonly cho control. "ChiTietDen" dropdown list để disable vì chức năng chưa phát triển xong
        String ReadOnly = "", ReadOnly1 = "";
        if (dtCapPhat != null && dtCapPhat.Rows.Count > 0)
        {
            R = dtCapPhat.Rows[0];
            //iDM_MaLoaiCapPhat = Convert.ToString(R["iDM_MaLoaiCapPhat"]);
            dNgayCapPhat = CommonFunction.LayXauNgay(Convert.ToDateTime(R["dNgayCapPhat"]));
            sNoiDung = Convert.ToString(R["sNoiDung"]);
            iID_MaDonVi = Convert.ToString(R["iID_MaDonVi"]);
            iSoCapPhat = Convert.ToString(R["iSoCapPhat"]);
            sLyDo = Convert.ToString(R["sLyDo"]);
            iID_MaTrangThaiDuyet = Convert.ToString(R["iID_MaTrangThaiDuyet"]);
            sTienToChungTu = Convert.ToString(R["sTienToChungTu"]);
            sLoai = Convert.ToString(R["sLoai"]);
            iID_MaTinhChatCapThu = Convert.ToString(R["iID_MaTinhChatCapThu"]);
            iID_NguonDuToan = Convert.ToString(R["iID_NguonDuToan"]);
            ReadOnly = "disabled=\"disabled\"";
            sLNS = Convert.ToString(R["sDSLNS"]);
        }
        else
        {
            dNgayCapPhat = CommonFunction.LayXauNgay(DateTime.Now);
        }
        String[] arrLNS = sLNS.Split(',');
        if (Loai == "4")
        {
            ReadOnly1 = "disabled=\"disabled\"";
        }
        if (ViewData["DuLieuMoi"] == "1")
        {
            if (Loai == "1")
            {
                sLoai = "sTM";
            }
            else
            {
                sLoai = "sNG";
            }
            if (Loai == "4")
            {
                sTienToChungTu = "Séc-ĐMNB-";
            }
            else
            {
                sTienToChungTu = PhanHeModels.LayTienToChungTu(CapPhatModels.iID_MaPhanHe);
            }
            iSoCapPhat = Convert.ToString(CapPhat_ChungTuModels.GetMaxSoCapPhat() + 1);
        }
        DataTable dtTinhChatCapThu = CapPhatModels.Get_dtTinhChatCapThu();
        SelectOptionList slTinhChatCapThu = new SelectOptionList(dtTinhChatCapThu, "iID_MaTinhChatCapThu", "sTen");
        dtTinhChatCapThu.Dispose();

        DataTable dtChiTieu = CapPhatModels.getDanhSachChiTieu();
        SelectOptionList slNguonChiTieu = new SelectOptionList(dtChiTieu, "MaLoai", "sTen");
        dtTinhChatCapThu.Dispose();

        DataTable dtDonVi = NganSach_HamChungModels.DSDonViCuaNguoiDung(UserID);
        SelectOptionList slDonVi = new SelectOptionList(dtDonVi, "iID_MaDonVi", "TenHT");
        DataRow R1 = dtDonVi.NewRow();
        R1["iID_MaDonVi"] = "-1";
        R1["TenHT"] = "--- Chọn đơn vị (Không chọn sẽ cấp cho tất cả đơn vị quản lý) ---";
        dtDonVi.Rows.InsertAt(R1, 0);
        dtDonVi.Dispose();

        //DataTable dtLoaiCapPhat = CommonFunction.Lay_dtDanhMuc("LoaiCapPhat");
        //SelectOptionList slLoaiCapPhat = new SelectOptionList(dtLoaiCapPhat, "iID_MaDanhMuc", "sTen");

        String BackURL = Url.Action("Index", "CapPhat_ChungTu", new { Loai = Loai });

        using (Html.BeginForm("LuuChungTu", "CapPhat_ChungTu", new { ParentID = ParentID, iID_MaCapPhat = iID_MaCapPhat, sLNS = sLNS, DonVi = DonVi, Loai = Loai }))
        {
    %>
    <%= Html.Hidden(ParentID + "_DuLieuMoi", ViewData["DuLieuMoi"])%>
    <div class="custom_css box_tong">
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
                                    <b><%=sTienToChungTu %><%=iSoCapPhat%></b>
                                </div>
                            </td>
                        </tr>
                        <%--<tr>
                            <td class="td_form2_td1">
                                <div><b>Loại cấp phát<b></div>
                            </td>
                            <td class="td_form2_td5">
                                <div>
                                    <%=MyHtmlHelper.DropDownList(ParentID, slLoaiCapPhat, iDM_MaLoaiCapPhat, "iDM_MaLoaiCapPhat", "", "style=\"width:50%\"")%><br />
                                    <%= Html.ValidationMessage(ParentID + "_" + "err_iDM_MaLoaiCapPhat")%>
                                </div>
                            </td>
                        </tr>--%>
                        <tr>
                            <td class="td_form2_td1">
                                <div><b>Tính chất cấp thu</b></div>
                            </td>
                            <td class="td_form2_td5">
                                <div>
                                    <%=MyHtmlHelper.DropDownList(ParentID, slTinhChatCapThu, iID_MaTinhChatCapThu, "iID_MaTinhChatCapThu", ReadOnly + ReadOnly1, "class=\"form-control\" style=\" width: 100%; \"")%>
                                    <%= Html.ValidationMessage(ParentID + "_" + "err_iID_MaTinhChatCapThu")%>
                                </div>
                            </td>
                        </tr>

                        <tr>
                            <td class="td_form2_td1">
                                <div><b>Đơn vị</b></div>
                            </td>
                            <td class="td_form2_td5">
                                <div>
                                    <%=MyHtmlHelper.DropDownList(ParentID, slDonVi, iID_MaDonVi, "iID_MaDonVi", ReadOnly + ReadOnly1 , "class=\"form-control\" style=\"width: 100%; \"")%>
                                </div>
                            </td>
                        </tr>
                        <% if (Loai == "4")
                            { %>
                        <tr>
                            <td class="td_form2_td1">
                                <div><b>Chọn Ngành: </b></div>
                            </td>
                            <td class="td_form2_td5">
                                <div style="width: 100%; overflow: scroll; height: 250px;">
                                    <table class="mGrid">
                                        <tr>
                                            <td style="width: 15%; text-align: center">
                                                <input type="checkbox" id="Checkbox1" onclick="Chonall(this.checked)" />
                                            </td>
                                            <td>Chọn tất cả
                                            </td>
                                        </tr>
                                        <%
                                            String TenLoaiNS = "";
                                            String LoaiNS = "";
                                            String _CheckedC = "checked=\"checked\"";
                                            for (int i = 0; i < dtNganh.Rows.Count; i++)
                                            {
                                                _CheckedC = "";
                                                TenLoaiNS = Convert.ToString(dtNganh.Rows[i]["sTenNganh"]);
                                                LoaiNS = Convert.ToString(dtNganh.Rows[i]["iID_MaNganh"]);
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
                                            <td style="width: 15%; text-align: center">
                                                <input type="checkbox" value="<%= LoaiNS %>" <%= _CheckedC %> check-group="sLNS" id="Checkbox2"
                                                    name="sLNS" />
                                            </td>
                                            <td>
                                                <%= TenLoaiNS%>
                                            </td>
                                        </tr>
                                        <% }%>
                                    </table>
                                </div>
                                <div>
                                    <%= Html.ValidationMessage(ParentID + "_" + "err_sLNS")%>
                                </div>
                            </td>
                        </tr>
                        <%}
                            else
                            { %>
                        <tr>
                            <td class="td_form2_td1">
                                <div><b>Chọn LNS</b></div>
                            </td>
                            <td class="td_form2_td5">
                                <div style="width: 100%; overflow: scroll; height: 250px;">
                                    <table class="mGrid">
                                        <tr>
                                            <td style="width: 15%; text-align: center">
                                                <input type="checkbox" id="checkAll" onclick="Chonall(this.checked)" />
                                            </td>
                                            <td>Chọn tất cả
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
                                            <td style="width: 15%; text-align: center">
                                                <input type="checkbox" value="<%= LoaiNS %>" <%= _CheckedC %> check-group="sLNS" id="sLNS"
                                                    name="sLNS" />
                                            </td>
                                            <td>&nbsp;&nbsp;<% =TenLoaiNS%>
                                            </td>
                                        </tr>
                                        <% }%>
                                    </table>
                                </div>
                                <div>
                                    <%= Html.ValidationMessage(ParentID + "_" + "err_sLNS")%>
                                </div>
                            </td>
                        </tr>
                        <%} %>
                        <tr>
                            <td class="td_form2_td1">
                                <div><b>Nguồn chỉ tiêu</b></div>
                            </td>
                            <td class="td_form2_td5">
                                <div><%=MyHtmlHelper.DropDownList(ParentID, slNguonChiTieu, iID_NguonDuToan, "iID_NguonDuToan", ReadOnly, "class=\"form-control\" style=\"width:100%\"")%></div>
                            </td>
                        </tr>
                        <tr>
                            <td class="td_form2_td1">
                                <div><b>Chi tiết đến</b></div>
                            </td>
                            <td class="td_form2_td5">
                                <div><%=MyHtmlHelper.DropDownList(ParentID, slLoai, sLoai, "iID_Loai", ReadOnly + ReadOnly1, "class=\"form-control\" style=\"width:100%\"")%></div>
                            </td>
                        </tr>

                        <%if (String.IsNullOrEmpty(DonVi) == false)
                            { %>
                        <tr>
                            <td class="td_form2_td1">
                                <div>&nbsp;</div>
                            </td>
                            <td class="td_form2_td5">
                                <div>
                                    <%=MyHtmlHelper.Option(ParentID, "sLNS", sLoai, "sLoai", "")%>
                            &nbsp;LNS                            
                            <%=MyHtmlHelper.Option(ParentID, "sM", sLoai, "sLoai", "")%>
                            &nbsp;Mục
                            
                            <%=MyHtmlHelper.Option(ParentID, "sTM", sLoai, "sLoai", "")%>
                            &nbsp;T.Mục
                                <%= Html.ValidationMessage(ParentID + "_" + "err_sLoai")%>
                                </div>
                            </td>
                        </tr>
                        <%} %>
                        <tr>
                            <td class="td_form2_td1">
                                <div><b>Ngày chứng từ</b></div>
                            </td>
                            <td class="td_form2_td5">
                                <div style="width: 35%">
                                    <%=MyHtmlHelper.DatePicker(ParentID, dNgayCapPhat, "dNgayCapPhat", "", "style=\"width:95%\" class=\"form-control\" onblur=isDate(this);")%><br />
                                    <%= Html.ValidationMessage(ParentID + "_" + "err_dNgayCapPhat")%>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="td_form2_td1">
                                <div><b>Nội dung</b></div>
                            </td>
                            <td class="td_form2_td5">
                                <div><%=MyHtmlHelper.TextArea(ParentID, sNoiDung, "sNoiDung", "", "class=\"form-control\" style=\"height: 100px;resize: none;\"")%></div>
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
                                                   <%--<button type="submit" class="btn btn-primary" id="Submit1" ><i class="fa fa-download"></i>Lưu</button>--%>
                                            </td>
                                            <td width="5px">&nbsp;</td>
                                            <td class="td_form2_td5">
                                                <input class="button" type="button" value="Hủy" onclick="Huy()" />
                                                   <%--<button type="button" class="btn btn-default" onclick="Huy()"><i class="fa fa-ban"></i>Hủy</button>--%>
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
        //if(dtCapPhat!=null) dtCapPhat.Dispose();
        //dtLoaiCapPhat.Dispose();    
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
