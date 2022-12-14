<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="VIETTEL.Models.DuToanBS" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        string ParentID = "DTBS_ChungTu";
        string UserID = User.Identity.Name;

        string MaChungTu = Convert.ToString(ViewData["MaChungTu"]);
        string sLNS = Convert.ToString(ViewData["sLNS"]);
        string iKyThuat = Convert.ToString(ViewData["iKyThuat"]);
        string MaPhongBanNguoiDung = NganSach_HamChungModels.MaPhongBanCuaMaND(UserID);

        DataTable dtChungTu = DuToanBSChungTuModels.LayChungTu(MaChungTu);
        string dNgayChungTu = "";
        string sNoiDung = "";

        if (ViewData["dNgayChungTu"] != null)
        {
            dNgayChungTu = Convert.ToString(ViewData["dNgayChungTu"]);
        }
        else if (dtChungTu.Rows.Count > 0)
        {
            dNgayChungTu = CommonFunction.LayXauNgay(Convert.ToDateTime(dtChungTu.Rows[0]["dNgayChungTu"]));
            sNoiDung = Convert.ToString(dtChungTu.Rows[0]["sNoiDung"]);
        }
        else
        {
            dNgayChungTu = CommonFunction.LayXauNgay(DateTime.Now);
        }
        string BackURL = Url.Action("Index", "DuToanBSChungTu", new { sLNS = sLNS, iKyThuat = iKyThuat });

        using (Html.BeginForm("ThemSuaChungTu", "DuToanBSChungTu", new { parentID = ParentID, maChungTu = MaChungTu, sLNS = sLNS, iKyThuat = iKyThuat }))
        {
    %>
    <%= Html.Hidden(ParentID + "_DuLieuMoi", 0)%>
    <div class="box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span><%=NgonNgu.LayXau("Sửa thông tin chứng từ")%>&nbsp; &nbsp;</span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="nhapform">
            <div id="form2">
                <div style="width: 50%; float: left;">
                    <table cellpadding="0" cellspacing="0" border="0" width="100%">
                        <tr>
                            <td class="td_form2_td1">
                                <div>Ngày chứng từ</div>
                            </td>
                            <td class="td_form2_td5">
                                <div style="width: 200px; float: left;">
                                    <%=MyHtmlHelper.DatePicker(ParentID, dNgayChungTu, "dNgayChungTu", "", "class=\"input1_2\"")%>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="td_form2_td1">&nbsp;
                            </td>
                            <td class="td_form2_td5">
                                <div>
                                    <%= Html.ValidationMessage(ParentID + "_" + "err_dNgayChungTu")%>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="td_form2_td1">
                                <div>Nội dung</div>
                            </td>
                            <td class="td_form2_td5">
                                <div style="width: 550px; float: left;">
                                    <%=MyHtmlHelper.TextArea(ParentID, sNoiDung, "sNoiDung", "", "class=\"input1_2\" style=\"height: 100px; resize: none\"")%>
                                </div>
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
                                                <input type="submit" class="button" id="btLuu" value="Lưu" />
                                            </td>
                                            <td width="5px">&nbsp;</td>
                                            <td class="td_form2_td5">
                                                <input class="button" type="button" id="btHuy" value="Hủy" onclick="Huy()" />
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
        dtChungTu.Dispose();
    %>
    <script type="text/javascript">
        function Huy() {
            window.location.href = '<%=BackURL%>';
        }
    </script>
</asp:Content>
