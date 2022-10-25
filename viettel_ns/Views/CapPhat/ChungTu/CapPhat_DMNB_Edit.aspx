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
        DataTable dtNganh = CapPhatModels.getDanhSachNganh(UserID);
        SelectOptionList slNganh = new SelectOptionList(dtNganh, "iID_MaNganh", "sTenNganh");
        dtNganh.Dispose();

        DataTable dtLoai = CapPhat_ChungTuModels.LayLoaiNganSachCon();
        SelectOptionList slLoai = new SelectOptionList(dtLoai, "iID_Loai", "TenHT");

        dtLoai.Dispose();

        DataTable dtCapPhat = CapPhat_ChungTuModels.LayChungTuCapPhat(iID_MaCapPhat);
        DataRow R;
        String sLoai = "", iSoCapPhat = "", sTienToChungTu = "", dNgayCapPhat = "", sNoiDung = "", sLyDo = "", iID_MaTrangThaiDuyet = "", iDM_MaLoaiCapPhat = "", iID_MaTinhChatCapThu = ""; ;
        // HungPX: Cài đặt TT readonly cho control. "ChiTietDen" dropdown list để disable vì chức năng chưa phát triển xong
        String ReadOnly = "";
        if (dtCapPhat != null && dtCapPhat.Rows.Count > 0)
        {
            R = dtCapPhat.Rows[0];
            iDM_MaLoaiCapPhat = Convert.ToString(R["iDM_MaLoaiCapPhat"]);
            dNgayCapPhat = CommonFunction.LayXauNgay(Convert.ToDateTime(R["dNgayCapPhat"]));
            sNoiDung = Convert.ToString(R["sNoiDung"]);
            iSoCapPhat = Convert.ToString(R["iSoCapPhat"]);
            sLyDo = Convert.ToString(R["sLyDo"]);
            iID_MaTrangThaiDuyet = Convert.ToString(R["iID_MaTrangThaiDuyet"]);
            sTienToChungTu = Convert.ToString(R["sTienToChungTu"]);
            sLoai = Convert.ToString(R["sLoai"]);
            iID_MaTinhChatCapThu = Convert.ToString(R["iID_MaTinhChatCapThu"]);
            ReadOnly = "disabled=\"disabled\"";
            sLNS = Convert.ToString(R["sDSLNS"]);
        }
        else
        {
            dNgayCapPhat = CommonFunction.LayXauNgay(DateTime.Now);
        }
        String[] arrLNS = sLNS.Split(',');
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
            sTienToChungTu = "Séc-ĐMNB-";
            iSoCapPhat = Convert.ToString(CapPhat_ChungTuModels.GetMaxSoCapPhat() + 1);
        }
        //DataTable dtTinhChatCapThu = TinhChatCapThuModels.Get_dtTinhChatCapThu();
        //SelectOptionList slTinhChatCapThu = new SelectOptionList(dtTinhChatCapThu, "iID_MaTinhChatCapThu", "sTen");
        //DataRow R2 = dtTinhChatCapThu.NewRow();
        //R2["iID_MaTinhChatCapThu"] = "-1";
        //R2["sTen"] = "--Chọn tính chất cấp thu--";
        //dtTinhChatCapThu.Rows.InsertAt(R2, 0);
        //dtTinhChatCapThu.Dispose();

        //DataTable dtLoaiCapPhat=CommonFunction.Lay_dtDanhMuc("LoaiCapPhat");         
        //SelectOptionList slLoaiCapPhat= new SelectOptionList(dtLoaiCapPhat,"iID_MaDanhMuc","sTen");

        String BackURL = Url.Action("Index", "CapPhat_ChungTu", new { Loai = Loai });

        using (Html.BeginForm("LuuChungTu", "CapPhat_ChungTu", new { ParentID = ParentID, iID_MaCapPhat = iID_MaCapPhat, sLNS = sLNS, DonVi = DonVi, Loai = Loai }))
        {
    %>
    <%= Html.Hidden(ParentID + "_DuLieuMoi", ViewData["DuLieuMoi"])%>
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
                    </tr>
                     <tr>
                        <td class="td_form2_td1">
                            <div><b>Tính chất cấp thu</b></div>
                        </td>
                        <td class="td_form2_td5">
                            <div>
                                <%=MyHtmlHelper.DropDownList(ParentID, slTinhChatCapThu, iID_MaTinhChatCapThu, "iID_MaTinhChatCapThu", "", "style=\"width:50%\"")%><br />
                                <%= Html.ValidationMessage(ParentID + "_" + "err_iID_MaTinhChatCapThu")%>
                            </div>
                        </td>
                    </tr>--%>
                        <tr>
                            <td class="td_form2_td1">
                                <div><b>Chọn LNS</b></div>
                            </td>
                            <td class="td_form2_td5">
                                <div style="width: 100%; overflow: scroll; height: 250px;">
                                    <table class="mGrid">
                                        <tr>
                                            <td style="width: 15%; text-align: center">
                                                <input type="checkbox" id="checkAll" onclick="Chonall(this.checked)">
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
                                <div><%=MyHtmlHelper.DropDownList(ParentID, slLoai, sLoai, "iID_Loai", "class=\"input1_2\" style=\"height: 100px;width:100%\"", ReadOnly)%></div>
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
                                    <%=MyHtmlHelper.DatePicker(ParentID, dNgayCapPhat, "dNgayCapPhat", "", "style=\"width:95%\" class=\"input1_2\" onblur=isDate(this);")%><br />
                                    <%= Html.ValidationMessage(ParentID + "_" + "err_dNgayCapPhat")%>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="td_form2_td1">
                                <div><b>Nội dung</b></div>
                            </td>
                            <td class="td_form2_td5">
                                <div><%=MyHtmlHelper.TextArea(ParentID, sNoiDung, "sNoiDung", "", "class=\"input1_2\" style=\"height: 100px;resize: none;\"")%></div>
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
