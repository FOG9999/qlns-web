<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="VIETTEL.Models.ThuNop" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        String iID_MaBaoCao = Convert.ToString(ViewData["iID_MaBaoCao"]);
        String iID_MaBaoCao_Cha = Convert.ToString(ViewData["iID_MaBaoCao_Cha"]);
        String iLoai = Convert.ToString(ViewData["iLoai"]);
        String ParentID = "Edit";
        String sLoaiHinh = "", sTen = "", sLoaiNS = "", iID_MaCotBaoCao = "", sTenVietTat = "";
        DataTable dmLoaiHinh = ThuNop_CauHinhBaoCaoModels.Get_MucLucLoaiHinh(iLoai);
               
        //Chi tiết cấu hình báo cáo nếu trong trường hợp sửa
        DataTable dt = ThuNop_CauHinhBaoCaoModels.Get_ChiTietBaoCao_Row(iID_MaBaoCao);
        if (dt.Rows.Count > 0 && iID_MaBaoCao != null && iID_MaBaoCao != "")
        {
            DataRow R = dt.Rows[0];
            sLoaiNS = HamChung.ConvertToString(R["sLoaiNS"]);
            sLoaiHinh = HamChung.ConvertToString(R["sLoaiHinh"]);
            sTen = HamChung.ConvertToString(R["sTen"]);
            sTenVietTat = HamChung.ConvertToString(R["sTenVietTat"]);
            iID_MaCotBaoCao = Convert.ToString(R["iID_MaCotBaoCao"]);            
        }       
        String[] arrLoaiHinh = sLoaiHinh.Split(',');

        DataTable dtLNS = new DataTable();
        dtLNS.Columns.Add("sLoaiNS", typeof(String));
        dtLNS.Columns.Add("sTen", typeof(String));
        DataRow dr = dtLNS.NewRow();
        dr["sLoaiNS"] = "NSQP";
        dr["sTen"] = "Nộp ngân sách quốc phòng";
        dtLNS.Rows.InsertAt(dr, 0);

        dr = dtLNS.NewRow();
        dr["sLoaiNS"] = "NSNN";
        dr["sTen"] = "Nộp ngân sách nhà nước";
        dtLNS.Rows.InsertAt(dr, 1);
        
        using (Html.BeginForm("EditSubmit", "ThuNop_CauHinhBaoCao", new { ParentID = ParentID, iID_MaBaoCao = iID_MaBaoCao, iLoai = iLoai }))
        {
    %>
    <%= Html.Hidden(ParentID + "_DuLieuMoi", ViewData["DuLieuMoi"])%>
    <%= Html.Hidden(ParentID + "_iID_MaBaoCao_Cha", iID_MaBaoCao_Cha)%>
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td style="width: 10%">
                <div style="padding-left: 20px; padding-top: 5px; padding-bottom: 5px; text-transform: uppercase;
                    color: #ec3237;">
                    <b>
                        <%=NgonNgu.LayXau("Liên kết nhanh: ")%></b>
                </div>
            </td>
            <td align="left">
                <div style="padding-top: 5px; padding-bottom: 5px; color: #ec3237;">
                    <%=MyHtmlHelper.ActionLink(Url.Action("Index", "Home"), "Trang chủ")%>
                    |
                    <%=MyHtmlHelper.ActionLink(Url.Action("Index", "ThuNop_CauHinhBaoCao"), "Danh sách cấu hình báo cáo")%>
                </div>
            </td>
        </tr>
    </table>
    <div class="custom_css box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                     <% if (ViewData["DuLieuMoi"] == "1")
                   {
                       %>
                	 <span>Nhập thông tin cấu hình báo cáo</span>
                    <% 
                   }
                   else
                   { %>
                    <span>Sửa thông tin cấu hình báo cáo</span>
                    <% } %>
                       
                    </td>
                </tr>
            </table>
        </div>
        <div id="nhapform">
            <div id="form2">
                <table cellpadding="5" cellspacing="5" width="50%">
                    <tr>
                        <td class="td_form2_td1">
                            <div>
                                <b>Mã cột báo cáo</b>&nbsp;</div>
                        </td>
                        <td class="td_form2_td5">
                            <div>
                                <%=MyHtmlHelper.TextBox(ParentID, iID_MaCotBaoCao, "iID_MaCotBaoCao", "", "class=\"form-control\"", 2)%>
                            </div>
                        </td>
                    </tr> 
                    <% if (iLoai == "2")
                        {%>
                    <tr>
                        <td class="td_form2_td1">
                            <div>
                                <b>Loại hình ngân sách thu nộp</b>&nbsp;</div>
                        </td>
                        <td class="td_form2_td5">                        
                           <div style="width: 100%; height: 65; border: 1px solid #ccc; background-color:Aqua">
                                <table class="mGrid">                                    
                                    <%
                            String TenLoaiNS = "";
                            String LoaiNS = "";
                            String _Checked = "checked=\"checked\"";
                            for (int i = 0; i < dtLNS.Rows.Count; i++)
                            {
                                _Checked = "";
                                TenLoaiNS = Convert.ToString(dtLNS.Rows[i]["sTen"]);
                                LoaiNS = Convert.ToString(dtLNS.Rows[i]["sLoaiNS"]);

                                if (LoaiNS == sLoaiNS)
                                {
                                    _Checked = "checked=\"checked\"";
                                }
                                            
                                    %>
                                    <tr>
                                        <td style="width: 15%; text-align:center" >
                                            <input type="radio" value="<%= LoaiNS %>" <%= _Checked %> id="sLoaiNS"
                                                name="sLoaiNS" />
                                        </td>
                                        <td>
                                            <%= TenLoaiNS%> 
                                        </td>
                                    </tr>
                                    <% }%>
                                </table>
                            </div>                                                
                    </tr> 
                    <% }%> 
                    <tr>
                        <td class="td_form2_td1">
                            <div>
                                <b>Mã báo cáo</b></div>
                        </td>
                        <td class="td_form2_td5">
                            <div style="width: 100%; overflow: scroll; height: 400px; 
                                /*border: 1px solid #ccc;*/
                                ">
                                <table class="mGrid">                                    
                                    <%
                                        String TenLoaiHinh = "";
                                        String LoaiHinh = "";
                                        String _CheckedC = "checked=\"checked\"";
                                        for (int i = 0; i < dmLoaiHinh.Rows.Count; i++)
                                        {
                                            _CheckedC = "";
                                            TenLoaiHinh = Convert.ToString(dmLoaiHinh.Rows[i]["sTen"]);
                                            LoaiHinh = Convert.ToString(dmLoaiHinh.Rows[i]["sLoaiHinh"]);
                                            for (int j = 0; j < arrLoaiHinh.Length; j++)
                                            {
                                                if (LoaiHinh == arrLoaiHinh[j])
                                                {
                                                    _CheckedC = "checked=\"checked\"";
                                                    break;
                                                }
                                            }
                                    %>
                                    <tr>
                                        <td style="width: 15%; text-align:center" >
                                            <input type="checkbox" value="<%= LoaiHinh %>" <%= _CheckedC %> id="sLoaiHinh"
                                                name="sLoaiHinh" />
                                        </td>
                                        <td>
                                            <%= TenLoaiHinh %>
                                        </td>
                                    </tr>
                                    <% }%>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="td_form2_td1">
                            <div>
                                <b>Tên báo cáo</b>&nbsp;<span  style="color:Red;">*</span></div>
                        </td>
                        <td class="td_form2_td5">
                            <div>
                                <%=MyHtmlHelper.TextBox(ParentID, sTen, "sTen", "", "class=\"form-control\"", 2)%>
                                <%= Html.ValidationMessage(ParentID + "_" + "err_sTen")%>
                            </div>
                        </td>
                    </tr>    
                    <tr>
                        <td class="td_form2_td1">
                            <div>
                                <b>Tên báo cáo viết tắt</b>&nbsp;<span  style="color:Red;">*</span></div>
                        </td>
                        <td class="td_form2_td5">
                            <div>
                                <%=MyHtmlHelper.TextBox(ParentID, sTenVietTat, "sTenVietTat", "", "class=\"form-control\"", 2)%>
                                <%= Html.ValidationMessage(ParentID + "_" + "err_sTen")%>
                            </div>
                        </td>
                    </tr>                 
                </table>
            </div>
        </div>
   
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td width="70%">
                &nbsp;
            </td>
            <td width="30%" align="right">
                <table cellpadding="0" cellspacing="0" border="0" align="right">
                    <tr>
                        <td>
                            <button type="submit" class="btn btn-primary"><i class="fa fa-download"></i>Lưu</button>
                        </td>
                        <td width="5px">
                        </td>
                        <td>
                           <button type="button" class="btn btn-default" onclick="javascript:history.go(-1)"><i class="fa fa-ban"></i>Hủy</button>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
         </div>
    <br />
    <%
        } if (dt != null) { dt.Dispose(); };
        if (dmLoaiHinh != null) { dmLoaiHinh.Dispose(); };
        if (dtLNS != null) { dtLNS.Dispose(); };     
    %>
     
</asp:Content>
