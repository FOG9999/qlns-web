<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {

    }
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        int i;
        String MaND = User.Identity.Name;
        String ParentID = "CapPhat_ChungTu_DonVi";

        //Lấy thông tin tìm kiếm
        String iSoCapPhat = Request.QueryString["SoCapPhat"];
        String sTuNgay = Request.QueryString["TuNgay"];
        String sDenNgay = Request.QueryString["DenNgay"];
        String iID_MaTrangThaiDuyet = Request.QueryString["iID_MaTrangThaiDuyet"];
        String iDM_MaLoaiCapPhat = Request.QueryString["iDM_MaLoaiCapPhat"];
        String iID_MaTinhChatCapThu = Request.QueryString["iID_MaTinhChatCapThu"];
        String iID_MaDonVi = Request.QueryString["iID_MaDonVi"];
        String sLNS = Request.QueryString["sLNS"];

        String page = Request.QueryString["page"];

        String Loai = Request.QueryString["Loai"];
        if (string.IsNullOrEmpty(Loai))
            Loai = "1";
        if (HamChung.isDate(sTuNgay) == false) sTuNgay = "";
        if (HamChung.isDate(sDenNgay) == false) sDenNgay = "";
        int CurrentPage = 1;
        SqlCommand cmd;

        if (String.IsNullOrEmpty(iID_MaTrangThaiDuyet) || iID_MaTrangThaiDuyet == "-1")
        {
            iID_MaTrangThaiDuyet = "";
        }

        DataTable dtTrangThai_All = LuongCongViecModel.Get_dtDSTrangThaiDuyet(PhanHeModels.iID_MaPhanHeCapPhat);

        DataTable dtTrangThai = LuongCongViecModel.Get_dtDSTrangThaiDuyet_DuocXem(PhanHeModels.iID_MaPhanHeCapPhat, MaND);
        dtTrangThai.Rows.InsertAt(dtTrangThai.NewRow(), 0);
        dtTrangThai.Rows[0]["iID_MaTrangThaiDuyet"] = "-1";
        dtTrangThai.Rows[0]["sTen"] = "-- Chọn trạng thái --";
        SelectOptionList slTrangThai = new SelectOptionList(dtTrangThai, "iID_MaTrangThaiDuyet", "sTen");

        if (String.IsNullOrEmpty(page) == false)
        {
            CurrentPage = Convert.ToInt32(page);
        }

        DataTable dtALL = CapPhat_ChungTuModels.LayDanhSachChungTuDonVi(Loai, iSoCapPhat, iDM_MaLoaiCapPhat, iID_MaTinhChatCapThu, sLNS, iID_MaDonVi, iID_MaTrangThaiDuyet, sTuNgay, sDenNgay, MaND, true, CurrentPage, Globals.PageSize);

        double nums = dtALL.Rows.Count;
        var dt = dtALL.Copy();
        if (nums > 0)
            dt = dtALL.AsEnumerable().Skip((CurrentPage - 1) * Globals.PageSize).Take(Globals.PageSize).CopyToDataTable();

        int TotalPages = (int)Math.Ceiling(nums / Globals.PageSize);
        String strPhanTrang = MyHtmlHelper.PageLinks(String.Format("Trang {0}/{1}:", CurrentPage, TotalPages), CurrentPage, TotalPages, x => Url.Action("Index", new { iID_MaDonVi = iID_MaDonVi, MaND = MaND, SoCapPhat = iSoCapPhat, TuNgay = sTuNgay, DenNgay = sDenNgay, iID_MaTrangThaiDuyet = iID_MaTrangThaiDuyet, page = x, sLNS = sLNS }));
        String strThemMoi = Url.Action("SuaChungTu", "CapPhat_ChungTu", new { sLNS = sLNS, DonVi = iID_MaDonVi, Loai = Loai });

        //Lấy danh sách loại ngân sách quốc phòng
        String iID_MaPhongBan = NganSach_HamChungModels.MaPhongBanCuaMaND(MaND);
        DataTable dtLNS = new DataTable();
        if (Loai == "1")
        {
            dtLNS = DanhMucModels.NS_LoaiNganSachQuocPhong(iID_MaPhongBan);
        }
        else if (Loai == "2")
        {
            dtLNS = DanhMucModels.NS_LoaiNganSachNhaNuoc_PhongBan(iID_MaPhongBan);
        }
        else
        {
            dtLNS = DanhMucModels.NS_LoaiNganSachNghiepVuKhac_PhongBan(iID_MaPhongBan);
        }
        SelectOptionList slLNS = new SelectOptionList(dtLNS, "sLNS", "TenHT");
        dtLNS.Rows.InsertAt(dtLNS.NewRow(), 0);
        dtLNS.Rows[0]["sLNS"] = "-1";
        dtLNS.Rows[0]["TenHT"] = "--Chọn loại ngân sách--";
        dtLNS.Dispose();

        //Danh sách đơn vị
        DataTable dtDonVi = NganSach_HamChungModels.DSDonViCuaNguoiDung(MaND);
        SelectOptionList slDonVi = new SelectOptionList(dtDonVi, "iID_MaDonVi", "sTen");
        dtDonVi.Rows.InsertAt(dtDonVi.NewRow(), 0);
        dtDonVi.Rows[0]["iID_MaDonVi"] = "-1";
        dtDonVi.Rows[0]["sTen"] = "--Chọn đơn vị--";
        dtDonVi.Dispose();

        using (Html.BeginForm("TimKiemChungTu", "CapPhat_ChungTu_DonVi", new { ParentID = ParentID, Loai = Loai }))
        {
    %>
    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Thông tin tìm kiếm</span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="nhapform">
            <div id="form2">
                <table border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td valign="top" align="left" style="width: 45%; float: left">
                            <table cellpadding="5" cellspacing="5" width="100%">
                                <tr>
                                    <td class="td_form2_td1">
                                        <div><b>Số cấp phát</b></div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%=MyHtmlHelper.TextBox(ParentID, iSoCapPhat, "iSoCapPhat", "", "class=\"form-control\"")%>
                                        </div>
                                    </td>
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

                                <tr>
                                    <td class="td_form2_td1">
                                        <div><b>Loại ngân sách</b></div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%=MyHtmlHelper.DropDownList(ParentID, slLNS, sLNS, "sLNS", "", "class=\"form-control\"")%>
                                        </div>
                                    </td>
                                </tr>

                                
                            </table>
                        </td>
                        <td valign="top" align="left" style="width: 45%; float: left">
                            <table cellpadding="5" cellspacing="5" width="100%">

                                <tr>
                                    <td class="td_form2_td1">
                                        <div><b>Từ ngày</b></div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                             <%=MyHtmlHelper.DatePicker(ParentID, sTuNgay, "dTuNgay", "", "class=\"form-control\" onblur=isDate(this);")%>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1">
                                        <div><b>Đến ngày</b></div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%=MyHtmlHelper.DatePicker(ParentID, sDenNgay, "dDenNgay", "", "class=\"form-control\" onblur=isDate(this);")%>
                                        </div>
                                    </td>
                                </tr>

                                <tr>
                                    <td class="td_form2_td1">
                                        <div><b>Trạng thái</b></div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%=MyHtmlHelper.DropDownList(ParentID, slTrangThai, iID_MaTrangThaiDuyet, "iID_MaTrangThaiDuyet", "", "class=\"form-control\"")%>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1" colspan="2">&nbsp</td>
                                </tr>

                                <%--<tr>
                                    <td class="td_form2_td1" colspan="2">&nbsp</td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1" colspan="2">&nbsp</td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1" colspan="2">&nbsp</td>
                                </tr>--%>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center" class="td_form2_td1" style="height: 10px;"></td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center" style="padding: 0px 0px 10px 0px;">
                            <table border="0" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <button type="submit" class="btn btn-info"><i class="fa fa-search"></i>Tìm kiếm</button>
                                        <%--<input type="submit" class="button" value="Tìm kiếm" />--%>
                                    </td>
                                    <td style="width: 10px;"></td>
                                    <td>
                                        <button type="button"  class="btn btn-primary" id="TaoMoi" onclick="javascript: location.href = '<%=strThemMoi %>'" ><i class="fa fa-plus"></i>Thêm mới</button>
                                        <%--<input id="TaoMoi" type="button" class="button" value="Tạo mới" onclick="javascript: location.href = '<%=strThemMoi %>'" />--%>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    <%  } %>
    <br />
    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Danh sách cấp phát đơn vị</span>
                    </td>
                </tr>
            </table>
        </div>
        <table class="mGrid" id="<%= ParentID %>_thList">
            <tr>
                <th style="width: 3%;" align="center">STT</th>
                <th style="width: 7%;" align="center">Ngày cấp phát</th>
                <th style="width: 5%;" align="center">Số cấp phát</th>
                <th style="width: 7%;" align="center">Đơn vị</th>
                <th style="width: 4%;" align="center">LNS</th>
                <th style="width: 7%;" align="center">Chi tiết đến</th>
                <th style="width: 17%;" align="center">Nội dung</th>
                <th style="width: 10%;" align="center">Số tiền
                </th>
                <th style="width: 10%;" align="center">Trạng thái</th>
                <th style="width: 16%;" align="center">Lý do</th>
                <th style="width: 5%;" align="center"></th>
            </tr>
            <%
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow R = dt.Rows[i];
                    String classtr = "";
                    int STT = i + 1;
                    String NgayChungTu = CommonFunction.LayXauNgay(Convert.ToDateTime(R["dNgayCapPhat"]));
                    String sTrangThai = "", strColor = "";
                    int TrangThaiDuyet = Convert.ToInt16(R["iID_MaTrangThaiDuyet"]);
                    int DaDuyet = LuongCongViecModel.Get_iID_MaTrangThaiDuyet_DaDuyet(PhanHeModels.iID_MaPhanHeCapPhat);
                    String iID_MaCapPhat = Convert.ToString(R["iID_MaCapPhat"]);
                    for (int j = 0; j < dtTrangThai_All.Rows.Count; j++)
                    {
                        if (Convert.ToString(R["iID_MaTrangThaiDuyet"]) == Convert.ToString(dtTrangThai_All.Rows[j]["iID_MaTrangThaiDuyet"]))
                        {
                            sTrangThai = Convert.ToString(dtTrangThai_All.Rows[j]["sTen"]);
                            strColor = String.Format("style='background-color: {0}; background-repeat: repeat;'", dtTrangThai_All.Rows[j]["sMauSac"]);
                            break;
                        }
                    }

                    //VungNV: Lấy LNS
                    String sDSLNS = Convert.ToString(R["sDSLNS"]);
                    if (sDSLNS.Length > 15)
                    {
                        sDSLNS = sDSLNS.Substring(0, 15) + ",...";
                    }

                    //Lấy tên đơn vị
                    String strTenDonVi = DonViModels.Get_TenDonVi(Convert.ToString(R["iID_MaDonVi"]), MaND);
                    String strTongTien = "";
                    strTongTien = CommonFunction.DinhDangSo(CapPhat_ChungTuModels.ThongKeTongSoTien_ChungTu(Convert.ToString(R["iID_MaCapPhat"])));
                    //Thông tin chứng từ chi tiết đến
                    String chiTietDen = "";
                    DataTable dtLoai = CapPhat_ChungTuModels.LayLoaiNganSachCon();
                    for (int j = 0; j < dtLoai.Rows.Count; j++)
                    {
                        if (Convert.ToString(R["sLoai"]) == Convert.ToString(dtLoai.Rows[j]["iID_Loai"]))
                        {
                            chiTietDen = Convert.ToString(dtLoai.Rows[j]["TenHT"]);
                            break;
                        }
                    }

                    String strEdit = "";
                    String strDelete = "";

                    if (LuongCongViecModel.NguoiDung_DuocThemChungTu(PhanHeModels.iID_MaPhanHeCapPhat, MaND) &&
                                        LuongCongViecModel.KiemTra_TrangThaiKhoiTao(PhanHeModels.iID_MaPhanHeCapPhat, Convert.ToInt32(R["iID_MaTrangThaiDuyet"])))
                    {
                        strEdit = MyHtmlHelper.ActionLink(Url.Action("SuaChungTu", "CapPhat_ChungTu", new { iID_MaCapPhat = R["iID_MaCapPhat"], Loai = Loai }).ToString(), "<i class='fa fa-pencil-square-o fa-lg color-icon-edit'></i>", "Edit", "");

                        strDelete = MyHtmlHelper.ActionLink(Url.Action("XoaChungTu", "CapPhat_ChungTu_DonVi", new { iID_MaCapPhat = R["iID_MaCapPhat"], Loai = Loai }).ToString(), "<i class='fa fa-trash-o fa-lg color-icon-delete'></i>", "Delete", "");
                    }                
            %>
            <tr <%=strColor %>>
                <td align="center"><%=R["rownum"]%></td>
                <td align="center"><%=NgayChungTu %></td>
                <td align="center"><b><%=MyHtmlHelper.ActionLink(Url.Action("Index", "CapPhat_ChungTuChiTiet", new { iID_MaCapPhat = R["iID_MaCapPhat"], Loai = Loai }).ToString(), Convert.ToString(R["sTienToChungTu"]) + Convert.ToString(R["iSoCapPhat"]), "Detail", "")%></b></td>
                <td><%=strTenDonVi%></td>
                <td><%=sDSLNS%></td>
                <td><%=chiTietDen%></td>
                <td align="left"><%=dt.Rows[i]["sNoiDung"]%></td>
                <td align="right" style="color: black;">
                    <b>
                        <%=strTongTien%></b>
                </td>
                <td align="center"><%=sTrangThai %></td>
                <td align="left"><%=dt.Rows[i]["sLyDo"]%></td>
               
                <td align="center">
                    <%=strEdit%>                   
                    <%=strDelete%>                                       
                </td>
            </tr>
            <%} %>
            <tr class="pgr">
                <td colspan="13" align="right">
                    <%=strPhanTrang%>
                </td>
            </tr>
        </table>
    </div>
    <%
        dt.Dispose();
        dtTrangThai.Dispose();
        dtTrangThai_All.Dispose();
    %>
    <script type="text/javascript">
        function CallSuccess_CT() {
            location.reload();
            return false;
        }
        function OnInit_CT() {
            $("#idDialog").dialog("destroy");
            document.getElementById("idDialog").title = 'Thông tri';
            document.getElementById("idDialog").innerHTML = "";
            $("#idDialog").dialog({
                resizeable: false,
                width: 400,
                modal: true
            });
        }
        function OnLoad_CT(v) {
            document.getElementById("idDialog").innerHTML = v;
        }
    </script>

    <div id="idDialog" style="display: none;">
    </div>
</asp:Content>
