<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site_KeToan_Default.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        int i;
        String MaND = User.Identity.Name;
        String ParentID = "DuToan_ChungTu";
        String ChiNganSach = Request.QueryString["ChiNganSach"];
        String iID_MaChungTu_TLTH = Request.QueryString["iID_MaChungTu"];
        String bTLTH = Request.QueryString["bTLTH"];
        String MaDotNganSach = Convert.ToString(ViewData["MaDotNganSach"]);
        String MaLoaiNganSach = Request.QueryString["sLNS"];
        String sLNS = Request.QueryString["sLNS"];
        String iID_MaDonVi = Request.QueryString["iID_MaDonVi"];
        if (String.IsNullOrEmpty(sLNS)) sLNS = "-1";
        String iSoChungTu = Request.QueryString["SoChungTu"];
        String sTuNgay = Request.QueryString["TuNgay"];
        String sDenNgay = Request.QueryString["DenNgay"];
        String sLNS_TK = Request.QueryString["sLNS_TK"];
        String iTrangThai_TK = Request.QueryString["iTrangThai_TK"];
        String iID_MaTrangThaiDuyet = Request.QueryString["iID_MaTrangThaiDuyet"];
        String page = Request.QueryString["page"];
        String iTrangThai = Request.QueryString["iTrangThai"];
        DataTable dtTrangThai_All;
        String iID_MaPhongBan = "";
        DataTable dtPhongBan = NganSach_HamChungModels.DSBQLCuaNguoiDung(MaND);
        if (dtPhongBan != null && dtPhongBan.Rows.Count > 0)
        {
            DataRow drPhongBan = dtPhongBan.Rows[0];
            iID_MaPhongBan = Convert.ToString(drPhongBan["sKyHieu"]);
            dtPhongBan.Dispose();
        }
        int CurrentPage = 1;
        SqlCommand cmd;

        if (HamChung.isDate(sTuNgay) == false) sTuNgay = "";
        if (HamChung.isDate(sDenNgay) == false) sDenNgay = "";

        if (String.IsNullOrEmpty(iID_MaTrangThaiDuyet) || iID_MaTrangThaiDuyet == "-1") iID_MaTrangThaiDuyet = "";
        Boolean bThemMoi = false;
        String iThemMoi = "";
        if (ViewData["bThemMoi"] != null)
        {
            bThemMoi = Convert.ToBoolean(ViewData["bThemMoi"]);
            if (bThemMoi)
                iThemMoi = "on";
        }
        String dNgayChungTu = CommonFunction.LayXauNgay(DateTime.Now);
        //neu la ngan sach bao dam
        if (sLNS == "1040100")
            dtTrangThai_All = LuongCongViecModel.Get_dtDSTrangThaiDuyet(PhanHeModels.iID_MaPhanHeChiTieu);
        else
            dtTrangThai_All = LuongCongViecModel.Get_dtDSTrangThaiDuyet(DuToanModels.iID_MaPhanHe);


        DataTable dtTrangThai = LuongCongViecModel.Get_dtDSTrangThaiDuyet_DuocXem(DuToanModels.iID_MaPhanHe, MaND);
        dtTrangThai.Rows.InsertAt(dtTrangThai.NewRow(), 0);
        dtTrangThai.Rows[0]["iID_MaTrangThaiDuyet"] = -1;
        dtTrangThai.Rows[0]["sTen"] = "-- Chọn trạng thái --";
        SelectOptionList slTrangThai = new SelectOptionList(dtTrangThai, "iID_MaTrangThaiDuyet", "sTen");
        dtTrangThai.Dispose();

        DataTable dtTrangThaiSoLieu = DuToan_ChungTuModels.getTrangThai(sLNS);
        //dtTrangThaiSoLieu = dtTrangThaiSoLieu.Select("iTrangThai = 2").CopyToDataTable();
        SelectOptionList slTrangThaiSoLieu = new SelectOptionList(dtTrangThaiSoLieu, "iTrangThai", "Text");
        dtTrangThaiSoLieu.Dispose();

        DataTable dtLoaiNganSach = NganSach_HamChungModels.DSLNS_LocCuaPhongBan(MaND, sLNS);// DanhMucModels.NS_LoaiNganSach();
        if (sLNS != "2" && sLNS != "4")
        {
            dtLoaiNganSach.Rows.InsertAt(dtLoaiNganSach.NewRow(), 0);
            dtLoaiNganSach.Rows[0]["sLNS"] = "";
            dtLoaiNganSach.Rows[0]["sTen"] = "-- Chọn loại ngân sách --";
        }
        SelectOptionList slLoaiNganSach = new SelectOptionList(dtLoaiNganSach, "sLNS", "sTen");
        dtLoaiNganSach.Dispose();

        DataTable dtDonVi = NganSach_HamChungModels.DSDonViCuaNguoiDung(MaND);
       
        SelectOptionList slDonVi = new SelectOptionList(dtDonVi, "iID_MaDonVi", "TenHT");
        DataRow R1 = dtDonVi.NewRow();
        R1["iID_MaDonVi"] = "-1";
        R1["TenHT"] = "--- Chọn đơn vị (Không chọn sẽ cấp cho tất cả đơn vị quản lý) ---";
        dtDonVi.Rows.InsertAt(R1, 0);
        dtDonVi.Dispose();

        if (String.IsNullOrEmpty(page) == false)
        {
            CurrentPage = Convert.ToInt32(page);
        }
        //kiem tra nguoi dung co phan tro ly phong ban
        Boolean check = LuongCongViecModel.KiemTra_TroLyPhongBan(MaND);
        Boolean checkTroLyTongHop = LuongCongViecModel.KiemTra_TroLyTongHop(MaND);
        Boolean CheckNDtao = false;
        if (check) CheckNDtao = true;
        if (checkTroLyTongHop) check = true;
        DataTable dtALL = DuToan_ChungTuModels.Get_DanhSachChungTu(iID_MaChungTu_TLTH, bTLTH, iID_MaPhongBan, sLNS, MaDotNganSach, MaND, iSoChungTu, sTuNgay, sDenNgay, sLNS_TK, iTrangThai_TK, iID_MaTrangThaiDuyet, CheckNDtao, "0", CurrentPage, Globals.PageSize);

        double nums = dtALL.Rows.Count;
        var dt = dtALL.Copy();
        if (nums > 0)
            dt = dtALL.AsEnumerable().Skip((CurrentPage - 1) * Globals.PageSize).Take(Globals.PageSize).CopyToDataTable();

        int TotalPages = (int)Math.Ceiling(nums / Globals.PageSize);
        String strPhanTrang = MyHtmlHelper.PageLinks(String.Format("Trang {0}/{1}:", CurrentPage, TotalPages), CurrentPage, TotalPages, x => Url.Action("Index", new { SoChungTu = iSoChungTu, TuNgay = sTuNgay, DenNgay = sDenNgay, iID_MaTrangThaiDuyet = iID_MaTrangThaiDuyet, page = x, sLNS = sLNS }));
        String strThemMoi = Url.Action("Edit", "DuToan_ChungTu", new { MaDotNganSach = MaDotNganSach, sLNS = sLNS, ChiNganSach = ChiNganSach });

        int SoCot = 1;
        String[] arrMaDonVi = sLNS.Split(',');

        String iKhoa = DuToan_ChungTuModels.GetiKhoa(MaND);

    %>
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td align="left" style="width: 9%;">
                <div style="padding-left: 22px; padding-bottom: 5px; text-transform: uppercase; color: #ec3237;">
                    <b>
                        <%=NgonNgu.LayXau("Liên kết nhanh: ")%></b>
                </div>
            </td>
            <td align="left">
                <div style="padding-bottom: 5px; color: #ec3237;">
                    <%=MyHtmlHelper.ActionLink(Url.Action("Index", "Home"), "Trang chủ")%>
                </div>
            </td>
            <td align="right" style="padding-bottom: 5px; color: #ec3237; font-weight: bold; padding-right: 20px;">
                <% Html.RenderPartial("LogOnUserControl_KeToan"); %>
            </td>
        </tr>
    </table>
    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Tìm kiếm đợt dự toán</span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="nhapform">
            <div id="form2">
                <%
                    using (Html.BeginForm("SearchSubmit", "DuToan_ChungTu", new { ParentID = ParentID, sLNS = sLNS, iID_MaChungTu_TLTH = iID_MaChungTu_TLTH }))
                    {
                %>
                <table border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td class="td_form2_td1" style="width: 15%">
                            <div>
                                <b>Chọn loại chứng từ nhập</b>
                            </div>
                        </td>
                        <td class="td_form2_td5" style="width: 15%">
                            <div>
                                <%=MyHtmlHelper.DropDownList(ParentID, slTrangThaiSoLieu,iTrangThai_TK,"iTrangThai_TK", "", "class=\"form-control\"")%>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="width: 10%">
                            <div>
                                <b>Chọn LNS</b>
                            </div>
                        </td>
                        <td class="td_form2_td5" style="width: 15%">
                            <div>
                                <%=MyHtmlHelper.DropDownList(ParentID, slLoaiNganSach,sLNS_TK,"sLNS_TK", "", "class=\"form-control\"")%>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="width: 15%">
                            <div>
                                <b>Đợt dự toán từ ngày</b>
                            </div>
                        </td>
                        <td class="td_form2_td5" style="width: 15%">
                            <div>
                                <%=MyHtmlHelper.DatePicker(ParentID, sTuNgay, "dTuNgay", "", "class=\"form-control\"")%>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="td_form2_td1" style="width: 15%">
                            <div>
                                <b>Đến ngày</b>
                            </div>
                        </td>
                        <td class="td_form2_td5" style="width: 15%">
                            <div>
                                <%=MyHtmlHelper.DatePicker(ParentID, sDenNgay, "dDenNgay", "", "class=\"form-control\"")%>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="width: 10%">
                            <div>
                                <b>Trạng thái</b>
                            </div>
                        </td>
                        <td class="td_form2_td5" style="width: 15%">
                            <div>
                                <%=MyHtmlHelper.DropDownList(ParentID, slTrangThai, iID_MaTrangThaiDuyet, "iID_MaTrangThaiDuyet", "", "class=\"form-control\"")%>
                            </div>
                        </td>
                        <td class="td_form2_td1" style="width: 15%">
                        </td>
                        <td class="td_form2_td5" style="width: 15%">
                            <%--<input type="submit" class="button" value="Tìm kiếm" />--%>
                            <button class="btn btn-info" type="submit" style="float: right"><i class="fa fa-search"></i>Tìm kiếm</button>
                        </td>
                    </tr>
                </table>
                <%} %>
            </div>
        </div>
    </div>
    <br />
    <%if (check || checkTroLyTongHop)
        {%>
    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Thêm một đợt cấp mới </span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="Div1">
            <div id="Div2">
                <%
                    using (Html.BeginForm("EditSubmit", "DuToan_ChungTu", new { ParentID = ParentID, sLNS = sLNS }))
                    {
                %>
                <%= Html.Hidden(ParentID + "_DuLieuMoi", 1)%>
                <table cellpadding="0" cellspacing="0" width="100%" class="table_form2">
                    <tr>
                        <td style="width: 80%">
                            <table cellpadding="0" cellspacing="0" width="50%" class="table_form2">
                                <tr>
                                    <td class="td_form2_td1" style="width: 15%;">
                                        <div>
                                            <b>
                                                <%=NgonNgu.LayXau("Bổ sung đợt mới")%></b>
                                        </div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <%if (iKhoa == "0")
                                            {%>
                                        <div>
                                            <%=MyHtmlHelper.CheckBox(ParentID, iThemMoi, "iThemMoi", "", "onclick=\"CheckThemMoi(this.checked)\"")%>
                                            <span style="color: brown;">(Trường hợp bổ sung đợt mới, đánh dấu chọn "Bổ sung đợt
                                                mới". Nếu không chọn đợt bổ sung dưới lưới) </span>
                                        </div>
                                        <%} %>
                                    </td>
                                </tr>
                            </table>
                            <table cellpadding="0" cellspacing="0" border="0" width="50%" class="table_form2"
                                id="tb_DotNganSach">
                                <tr>
                                    <td class="td_form2_td1">
                                        <div><b>Số liệu nhập</b></div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%=MyHtmlHelper.DropDownList(ParentID, slTrangThaiSoLieu, iTrangThai, "iTrangThai", "", "class=\"form-control\"")%>
                                        </div>
                                    </td>
                                </tr>
                                <%if (sLNS.Substring(0, 1) == "8")
                                    {%>
                                <%= Html.Hidden(ParentID + "_sLNS", sLNS)%>
                                <%}
                                    else
                                    { %>
                                <tr>
                                    <td class="td_form2_td1">
                                        <div>
                                            <b>Loại ngân sách</b>
                                        </div>
                                    </td>
                                    <%if (sLNS == "2" || sLNS == "4")
                                        { %>
                                    <td class="td_form2_td5">
                                        <div style="overflow: scroll; width: 50%; height: 400px">
                                            <table class="mGrid" style="width: 100%">
                                                <tr>
                                                    <th align="center" style="width: 40px;">
                                                        <input type="checkbox" id="abc" onclick="CheckAll(this.checked)" />
                                                    </th>
                                                    <%for (int c = 0; c < SoCot * 2 - 1; c++)
                                                        {%>
                                                    <th></th>
                                                    <%} %>
                                                </tr>
                                                <%

                                                    String strsTen = "", MaDonVi = "", strChecked = "";
                                                    for (i = 0; i < dtLoaiNganSach.Rows.Count; i = i + SoCot)
                                                    {


                                                %>
                                                <tr>
                                                    <%for (int c = 0; c < SoCot; c++)
                                                        {
                                                            if (i + c < dtLoaiNganSach.Rows.Count)
                                                            {
                                                                strChecked = "";
                                                                strsTen = Convert.ToString(dtLoaiNganSach.Rows[i + c]["sTen"]);
                                                                MaDonVi = Convert.ToString(dtLoaiNganSach.Rows[i + c]["sLNS"]);
                                                                for (int j = 0; j < arrMaDonVi.Length; j++)
                                                                {
                                                                    if (MaDonVi.Equals(arrMaDonVi[j]))
                                                                    {
                                                                        strChecked = "checked=\"checked\"";
                                                                        break;
                                                                    }
                                                                }
                                                    %>
                                                    <td align="center" style="width: 40px;">
                                                        <input type="checkbox" value="<%=MaDonVi %>" <%=strChecked %> check-group="LNS" id="DuToan_ChungTu_sLNS"
                                                            name="DuToan_ChungTu_sLNS" />
                                                    </td>
                                                    <td align="left">
                                                        <%=strsTen%>
                                                    </td>
                                                    <%} %>
                                                    <%} %>
                                                </tr>
                                                <%}%>
                                            </table>
                                    </td>
                                    <%}
                                        else
                                        { %>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%=MyHtmlHelper.DropDownList(ParentID, slLoaiNganSach, sLNS, "sLNS", "", "class=\"form-control\"")%>
                                        </div>
                                        <%= Html.ValidationMessage(ParentID + "_" + "err_sLNS")%>
                                    </td>
                                    <%} %>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1">
                                        <div><b>Đơn vị</b></div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%=MyHtmlHelper.DropDownList(ParentID, slDonVi, iID_MaDonVi, "iID_MaDonVi", "", "class=\"form-control\"")%>
                                        </div>
                                    </td>
                                </tr>
                                <%} %>
                                <tr>
                                    <td class="td_form2_td1">
                                        <div>
                                            <b>Ngày tháng</b>
                                        </div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div style="width: 200px; float: left;">
                                            <%=MyHtmlHelper.DatePicker(ParentID, dNgayChungTu, "dNgayChungTu", "", "class=\"form-control\"  style=\"width: 200px;\"")%>
                                            <%= Html.ValidationMessage(ParentID + "_" + "err_dNgayChungTu")%>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1">
                                        <div>
                                            <b>Nội dung đợt</b>
                                        </div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%=MyHtmlHelper.TextArea(ParentID, "", "sNoiDung", "", "class=\"form-control\" style=\"height: 100px;\"")%>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1" style="width: 15%;">
                                        <div>
                                        </div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%--<input type="submit" class="button" id="Submit1" value="<%=NgonNgu.LayXau("Thêm mới")%>" />--%>
                                            <button class="btn btn-primary" id="Submit1" type="submit" ><i class="fa fa-plus"></i>Thêm mới</button>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <%  } %>
            </div>
        </div>
    </div>
    <%} %>
    <br />
    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Danh sách đợt dự toán</span>
                    </td>
                    <td align="right" style="padding-right: 10px;">
                        <%--<input id="Button1" type="submit" class="button_title" value="Trình duyệt" onclick="javascript:location.href=''" />--%>
                    </td>
                </tr>
            </table>
        </div>
        <table class="mGrid" id="<%= ParentID %>_thList">
            <tr>
                <th style="width: 20px;" align="center">STT
                </th>
                <th style="width: 60px;" align="center">LNS
                </th>
                <th style="width: 30px;" align="center">Đ.vị
                </th>
                <th style="width: 150px;" align="center">Đợt dự toán
                </th>
                <th style="width: 100px;" align="center">Chứng từ nhập
                </th>
                <th align="center">Nội dung đợt dự toán
                </th>
                <th style="width: 60px;" align="center">Chi tiết
                </th>
                <th style="width: 140px;" align="center">Số tiền
                </th>
                <th style="width: 60px;" align="center">Sửa
                </th>
                <th style="width: 60px;" align="center">Xóa
                </th>
                <th style="width: 60px;" align="center">Người tạo
                </th>
                <th style="width: 150px;" align="center">Trạng thái
                </th>
                <th align="center">Lý do
                </th>
                <th style="width: 60px;" align="center">Duyệt
                </th>
                <th style="width: 60px;" align="center">Từ chối
                </th>
            </tr>
            <%
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow R = dt.Rows[i];
                    String classtr = "";
                    int STT = i + 1;
                    String NgayChungTu = CommonFunction.LayXauNgay(Convert.ToDateTime(R["dNgayChungTu"]));
                    String sTrangThai = "";
                    String strColor = "";
                    for (int j = 0; j < dtTrangThai_All.Rows.Count; j++)
                    {
                        if (Convert.ToString(R["iID_MaTrangThaiDuyet"]) == Convert.ToString(dtTrangThai_All.Rows[j]["iID_MaTrangThaiDuyet"]))
                        {
                            sTrangThai = Convert.ToString(dtTrangThai_All.Rows[j]["sTen"]);
                            strColor = String.Format("style='background-color: {0}; background-repeat: repeat;'", dtTrangThai_All.Rows[j]["sMauSac"]);
                            break;
                        }
                    }
                    String strEdit = "";
                    String strDelete = "";
                    String strduyet = "", strTuChoi;
                    if (iKhoa == "0" && checkTroLyTongHop && (LuongCongViecModel.KiemTra_TrangThaiKhoiTao(DuToanModels.iID_MaPhanHe, Convert.ToInt32(R["iID_MaTrangThaiDuyet"])) || (LuongCongViecModel.KiemTra_TrangThaiTuChoi(DuToanModels.iID_MaPhanHe, Convert.ToInt32(R["iID_MaTrangThaiDuyet"])))))
                    {
                        strEdit = MyHtmlHelper.ActionLink(Url.Action("Edit", "DuToan_ChungTu", new { iID_MaChungTu = R["iID_MaChungTu"], sLNS = sLNS }).ToString(), "<i class='fa fa-pencil-square-o fa-lg color-icon-edit'></i>", "Edit", "", "title=\"Sửa chứng từ\"");
                        strDelete = MyHtmlHelper.ActionLink(Url.Action("Delete", "DuToan_ChungTu", new { iID_MaChungTu = R["iID_MaChungTu"], sLNS = sLNS, MaDotNganSach = MaDotNganSach, ChiNganSach = ChiNganSach }).ToString(), "<i class='fa fa-trash-o fa-lg color-icon-delete'></i>", "Delete", "", "title=\"Xóa chứng từ\"");
                    }
                    else
                    {
                        if ((iKhoa == "0" && LuongCongViecModel.NguoiDung_DuocThemChungTu(DuToanModels.iID_MaPhanHe, MaND) &&
                                            (LuongCongViecModel.KiemTra_TrangThaiKhoiTao(DuToanModels.iID_MaPhanHe, Convert.ToInt32(R["iID_MaTrangThaiDuyet"])) || (LuongCongViecModel.KiemTra_TrangThaiTuChoi(DuToanModels.iID_MaPhanHe, Convert.ToInt32(R["iID_MaTrangThaiDuyet"]))))) && check)
                        {
                            strEdit = MyHtmlHelper.ActionLink(Url.Action("Edit", "DuToan_ChungTu", new { iID_MaChungTu = R["iID_MaChungTu"], sLNS = sLNS }).ToString(), "<i class='fa fa-pencil-square-o fa-lg color-icon-edit'></i>", "Edit", "", "title=\"Sửa chứng từ\"");
                            strDelete = MyHtmlHelper.ActionLink(Url.Action("Delete", "DuToan_ChungTu", new { iID_MaChungTu = R["iID_MaChungTu"], sLNS = sLNS, MaDotNganSach = MaDotNganSach, ChiNganSach = ChiNganSach }).ToString(), "<i class='fa fa-trash-o fa-lg color-icon-delete'></i>", "Delete", "", "title=\"Xóa chứng từ\"");
                        }
                    }

                    string strTongTien = CommonFunction.DinhDangSo(Convert.ToString(R["SoTien"]));
                    string strDonvis = Convert.ToString(R["MaDonVi"]).Length > 6 ? Convert.ToString(R["MaDonVi"]).Substring(0, 6) + "..." : Convert.ToString(R["MaDonVi"]);
                    String strURL = MyHtmlHelper.ActionLink(Url.Action("Index", "DuToan_ChungTuChiTiet", new { iID_MaChungTu = R["iID_MaChungTu"] }).ToString(), "<img src='../Content/Themes/images/btnSetting.png' alt='' />", "Detail", null, "title=\"Xem chi tiết chứng từ\"");
                    String strURLTuChoi = "", strTex = "", strSoLieu = "";
                    if (Convert.ToString(R["iTrangThai"]) == "2")
                    {                       
                        strSoLieu = "Bổ sung & điều chỉnh";                      
                    }
                    else if (Convert.ToString(R["iTrangThai"]) == "3")
                    {                       
                        strSoLieu = "Ngành thẩm định";                      
                    }
                    else
                    {
                        strSoLieu = "Theo số kiểm tra";                        
                    }
                    if (LuongCongViecModel.KiemTra_NguoiDungDuocDuyet(MaND, PhanHeModels.iID_MaPhanHeDuToan) && Convert.ToInt16(R["iID_MaTrangThaiDuyet"]) == LuongCongViecModel.layTrangThaiDuyet(PhanHeModels.iID_MaPhanHeDuToan))
                    {
                        strURLTuChoi = Url.Action("TuChoi", "DuToan_ChungTuChiTiet", new { ChiNganSach = ChiNganSach, iID_MaChungTu = R["iID_MaChungTu"] });
                        strTex = "Từ chối";

                    }
                    bool DaDuyet = false;
                    if (LuongCongViecModel.KiemTra_TrangThaiDaDuyet(DuToanModels.iID_MaPhanHe, Convert.ToInt16(R["iID_MaTrangThaiDuyet"])))
                    {
                        DaDuyet = true;
                    }
                    if (DaDuyet == false)
                        strduyet = MyHtmlHelper.ActionLink(Url.Action("TrinhDuyet", "DuToan_ChungTuChiTiet", new { iID_MaChungTu = R["iID_MaChungTu"], sLNS = sLNS, iLoai = 1 }).ToString(), "<img src='../Content/Themes/images/arrow_up.png' alt='' />", "", "", "title=\"Duyệt chứng từ\"");
                    strTuChoi = MyHtmlHelper.ActionLink(Url.Action("TuChoi", "DuToan_ChungTuChiTiet", new { iID_MaChungTu = R["iID_MaChungTu"], sLNS = sLNS, iLoai = 1 }).ToString(), "<img src='../Content/Themes/images/arrow_down.png' alt='' />", "", "", "title=\"Từ chối chứng từ\"");
                    string LNS = R["sDSLNS"].ToString().Length > 7 ? R["sDSLNS"].ToString().Substring(0, 7) + ",..." : R["sDSLNS"].ToString();
                    if (LNS.Length >= 15)
                        LNS = LNS.Substring(0, 15);
            %>
            <tr <%=strColor %>>
                <td align="center">
                    <b>
                        <%=STT%></b>
                </td>
                <td align="center">
                    <b>
                        <%=MyHtmlHelper.ActionLink(Url.Action("Index", "DuToan_ChungTuChiTiet", new { iID_MaChungTu = R["iID_MaChungTu"] }).ToString(), LNS, "Detail", "")%></b>
                </td>
                <td align="left">
                    <%=strDonvis %>
                </td>
                <td align="center">
                    <b>
                        <%=MyHtmlHelper.ActionLink(Url.Action("Index", "DuToan_ChungTuChiTiet", new { ChiNganSach = ChiNganSach, MaDotNganSach = MaDotNganSach, iID_MaChungTu = R["iID_MaChungTu"] }).ToString(),"Đợt ngày: " +  NgayChungTu, "Detail", "")%></b>
                </td>
                <td align="center">
                    <b><%=strSoLieu%></b>
                </td>
                <td align="left">
                    <%=HttpUtility.HtmlEncode(dt.Rows[i]["sNoiDung"])%>
                </td>
                <td align="center">
                    <%=strURL %>
                </td>
                <td align="right" style="color: black;">
                    <b>
                        <%=strTongTien%></b>
                </td>
                <td align="center">
                    <%=strEdit%>
                </td>
                <td align="center">
                    <%=strDelete%>
                </td>
                <td align="center">
                    <%=R["sID_MaNguoiDungTao"]%>
                </td>
                <td align="center">
                    <%=sTrangThai%>
                </td>
                <td align="left">
                    <%=R["sLyDo"]%>
                </td>
                <td align="center">
                    <%if (iKhoa == "0")
                        {%>
                    <div onclick="OnInit_CT_NEW(500, 'Duyệt chứng từ');">
                        <%= Ajax.ActionLink("Trình Duyệt", "Index", "NhapNhanh", new { id = "DUTOAN_TRINHDUYETCHUNGTU", OnLoad = "OnLoad_CT", OnSuccess = "CallSuccess_CT", iID_MaChungTu = R["iID_MaChungTu"], sLNS = sLNS, iID_MaChungTu_TLTH = iID_MaChungTu_TLTH }, new AjaxOptions { }, new { @class = "buttonDuyet" })%>
                    </div>
                    <%} %>
                </td>
                <td align="center">
                    <%if (iKhoa == "0")
                        {%>
                    <div onclick="OnInit_CT_NEW(500, 'Từ chối chứng từ');">
                        <%= Ajax.ActionLink("Từ Chối", "Index", "NhapNhanh", new { id = "DUTOAN_TUCHOICHUNGTU", OnLoad = "OnLoad_CT", OnSuccess = "CallSuccess_CT", iID_MaChungTu = R["iID_MaChungTu"], iID_MaChungTu_TLTH = iID_MaChungTu_TLTH,sLNS=sLNS }, new AjaxOptions { }, new { @class = "button123" })%>
                    </div>
                    <%} %>
                </td>
            </tr>
            <%} %>
            <tr class="pgr">
                <td colspan="15" align="right">
                    <%=strPhanTrang%>
                </td>
            </tr>
        </table>
    </div>
    <%  
        dt.Dispose();
        dtTrangThai.Dispose();
    %>
    <script type="text/javascript">

        $(function () {
            $('.button123').text('');
        });
        $(function () {
            $('.buttonDuyet').text('');
        });
        function OnInit_CT_NEW(value, title) {
            $("#idDialog").dialog("destroy");
            document.getElementById("idDialog").title = title;
            document.getElementById("idDialog").innerHTML = "";
            $("#idDialog").dialog({
                resizeable: false,
                draggable: true,
                width: value,
                modal: true,
                open: function (event, ui) {
                    $(event.target).parent().css('position', 'fixed');
                    $(event.target).parent().css('top', '10px');
                }
            });
        }
        function CheckAll(value) {
            $("input:checkbox[check-group='LNS']").each(function (i) {
                this.checked = value;
            });
        } 
        function OnLoad_CT(v) {
            document.getElementById("idDialog").innerHTML = v;
        }
        CheckThemMoi(false);
        function CheckThemMoi(value) {
            if (value == true) {
                document.getElementById('tb_DotNganSach').style.display = '';
            } else {
                document.getElementById('tb_DotNganSach').style.display = 'none';
            }
        }
        if(<%=iKhoa%>=="1")
            alert("Đã khóa số liệu");
    </script>
    <div id="idDialog" style="display: none;">
    </div>
</asp:Content>
