<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="DomainModel.Abstract" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="VIETTEL.Models.DuToanBS" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%
        String UserID = User.Identity.Name;
        String id_chungtucp = Convert.ToString(ViewData["id_chungtucp"]);
        String Loai = Convert.ToString(ViewData["Loai"]);
        if (String.IsNullOrEmpty(Loai)) Loai = Convert.ToString(ViewData["Loai"]);

        DataTable dt = DuToanBSChungTuModels.LayDanhSachChungTuGomTLTH(UserID);

        var BackURL = Url.Action("Index", "CapPhat_ChungTuChiTiet", new { iID_MaCapPhat = id_chungtucp, Loai = Loai });
    %>
    <%
        using (Html.BeginForm("InsertData", "CapPhat_CapTheoCTGom", new { id_chungtucp = id_chungtucp, Loai = Loai }, FormMethod.Post, new { id = "InsertForm"}))
        {
    %>
    <div class="box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td><span>                        
                        <%=NgonNgu.LayXau("Cấp ngân sách theo chứng từ gom")%>
                    </span></td>
                </tr>
            </table>
        </div>
        <div id="nhapform">
            <div id="form2">
                <div style="width: 75%; float: left;">
                    <input type="hidden" id="bolDel" name= "bolDel"value="1" />
                    <table cellpadding="0" cellspacing="0" border="0" width="100%">
                        <tr>
                            <td class="td_form2_td1">
                                <div><b>Chọn đợt</b></div>
                            </td>
                            <td class="td_form2_td5">
                                <div style="overflow: scroll; width: 80%; height: 200px">
                                    <table class="mGrid" style="width: 100%">
                                        <tr>
                                            <th align="center" style="width: 40px;">
                                                <input type="checkbox" id="abc" onclick="CheckAll(this.checked)" />
                                            </th>
                                            <% for (int c = 0; c < 1 * 2 - 1; c++)
                                                {%>
                                            <th></th>
                                            <% } %>
                                        </tr>
                                        <%
                                            string strsTen = "";
                                            string MaChungTu = "";
                                            string strChecked = "";
                                            for (int i = 0; i < dt.Rows.Count; i = i + 1)
                                            {
                                        %>
                                        <tr>
                                            <% for (int c = 0; c < 1; c++)
                                                {
                                                    if (i + c < dt.Rows.Count)
                                                    {
                                                        strChecked = "";
                                                        strsTen = CommonFunction.LayXauNgay(
                                                                    Convert.ToDateTime(dt.Rows[i + c]["dNgayChungTu"])) + '-' +
                                                                    Convert.ToString(dt.Rows[i + c]["sNoiDung"]);
                                                        MaChungTu = Convert.ToString(dt.Rows[i + c]["iID_MaChungTu"]);
                                            %>
                                            <td align="center" style="width: 10px;">
                                                <input type="checkbox" value="<%= MaChungTu %>" <%= strChecked %> check-group="ChungTu"
                                                    id="iID_MaChungTu" name="iID_MaChungTu" />
                                            </td>
                                            <td align="left">
                                                <%= strsTen %>
                                            </td>
                                            <% } %>
                                            <% } %>
                                        </tr>
                                        <% } %>
                                    </table>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="td_form2_td1"></td>
                            <td class="td_form2_td5">
                                <div>
                                    <table cellpadding="0" cellspacing="0" border="0">
                                        <tr>
                                            <td style="width: 5%;" class="td_form2_td5">&nbsp;</td>
                                            <td align="left" style="width: 15px;" class="td_form2_td5">
                                                <input class="button" type="submit" value="Cấp"/>
                                            </td>
                                            <td style="width: 5px;">&nbsp;</td>
                                            <td align="left" class="td_form2_td5" style="width: 65%;">
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
        $('#InsertForm').submit(function() {
            var yes = confirm("Bạn có xóa dữ liệu cũ của chứng từ cấp phát không?");
            
            if (!yes)
            {
                document.getElementById("bolDel").value = "0";
            }
        })
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
