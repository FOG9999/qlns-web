<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="VIETTEL.Report_Controllers.PhanBo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
     <script type="text/javascript" language="javascript" src="../../../Scripts/jquery-latest.js"></script>
 <style type="text/css">
     div.login1 {
            text-align : center;    
        }    
        div.login1 a {
            color: #545998;
            text-decoration: none;
            font: bold 12px "Museo 700";
            display: block;
            width: 250px; height: 20px;
            line-height: 20px;
            margin: 0px auto;
            -webkit-border-radius:2px;
            border-radius:2px;
        }
        div.login1 a:hover
        {
            text-decoration:underline;
            color:#471083;
        }    
        div.login1 a.active {
            background-position:  20px 1px;
        }
        div.login1 a:active, a:focus {
            outline: none;
        }
         .style2
     {
         width: 126px;
     }
    
 </style>
</head>
<body>
       <%
        String srcFile = Convert.ToString(ViewData["srcFile"]);
        String ParentID = "TongHopChiTieu";
        String PageLoad = Convert.ToString(ViewData["PageLoad"]);
        String iID_MaDotPhanBo =  Convert.ToString(ViewData["iID_MaDotPhanBo"]);
        if (String.IsNullOrEmpty(iID_MaDotPhanBo))
        {
            iID_MaDotPhanBo = Guid.Empty.ToString();
        }
        String sLNS =  Convert.ToString(ViewData["sLNS"]);
        if (String.IsNullOrEmpty(sLNS))
        {
            sLNS = Convert.ToString(ViewData["sLNS"]);
        }
        
        String TruongTien =  Convert.ToString(ViewData["TruongTien"]);
        if(String.IsNullOrEmpty(TruongTien))
        {
            TruongTien=Convert.ToString(ViewData["TruongTien"]);
        }
        String MaND = User.Identity.Name;
        DateTime dNgayHienTai = DateTime.Now;
        String NamHienTai = Convert.ToString(dNgayHienTai.Year);
        int NamMin = Convert.ToInt32(dNgayHienTai.Year) - 10;
        int NamMax = Convert.ToInt32(dNgayHienTai.Year) + 10;
        DataTable dtNam = new DataTable();
        dtNam.Columns.Add("MaNam", typeof(String));
        dtNam.Columns.Add("TenNam", typeof(String));
        DataRow R;
        for (int i = NamMin; i < NamMax; i++)
        {
            R = dtNam.NewRow();
            dtNam.Rows.Add(R);
            R[0] = Convert.ToString(i);
            R[1] = Convert.ToString(i);
        }
        dtNam.Rows.InsertAt(dtNam.NewRow(), 0);
        dtNam.Rows[0]["TenNam"] = "-- Bạn chọn năm ngân sách --";
        SelectOptionList slNam = new SelectOptionList(dtNam, "MaNam", "TenNam");
        //dt Trạng thái duyệt
        String iID_MaTrangThaiDuyet =  Convert.ToString(ViewData["iID_MaTrangThaiDuyet"]);
        if (String.IsNullOrEmpty(iID_MaTrangThaiDuyet))
        {
            iID_MaTrangThaiDuyet = Convert.ToString(ViewData["iID_MaTrangThaiDuyet"]);
        }
        DataTable dtTrangThai = ReportModels.Get_dtDSTrangThaiDuyet(PhanHeModels.iID_MaPhanHePhanBo);
        SelectOptionList slTrangThai = new SelectOptionList(dtTrangThai, "iID_MaTrangThaiDuyet", "sTen");
        if (String.IsNullOrEmpty(iID_MaTrangThaiDuyet))
        {
            if (dtTrangThai.Rows.Count > 0)
            {
                iID_MaTrangThaiDuyet = Convert.ToString(dtTrangThai.Rows[0]["iID_MaTrangThaiDuyet"]);
            }
            else
            {
                iID_MaTrangThaiDuyet = Guid.Empty.ToString();
            }
        }
        dtTrangThai.Dispose();
        String ToSo = Convert.ToString(ViewData["ToSo"]);
        if (String.IsNullOrEmpty(ToSo))
        {
            PageLoad = "0";
            ToSo = "1";
        }
        String  ToDaXem = Convert.ToString(ViewData["ToDaXem"]);
       
        String KhoGiay =  Convert.ToString(ViewData["KhoGiay"]);
        if (String.IsNullOrEmpty(KhoGiay))
        {
            KhoGiay = "1";
        }
       
        String iID_MaPhongBan = NguoiDung_PhongBanModels.getMaPhongBan_NguoiDung(MaND);
        DataTable dtLNS = DanhMucModels.NS_LoaiNganSach_PhongBan(iID_MaPhongBan);
        SelectOptionList slLoaiNganSach = new SelectOptionList(dtLNS, "sLNS", "sLNS");
        if (String.IsNullOrEmpty(sLNS))
        {
            if (dtLNS.Rows.Count > 0)
            {
                sLNS = dtLNS.Rows[0]["sLNS"].ToString();
            }
            else
            {
                sLNS = Guid.Empty.ToString();
            }
        }
        dtLNS.Dispose();
        String[] arrLNS = sLNS.Split(',');
        DataTable dtDotPhanBo = rptPB_TongHopChiTieuNganSachQuocPhong_9Controller.DanhSach_DotPhanBo(MaND, sLNS, iID_MaTrangThaiDuyet, TruongTien);
        SelectOptionList slDotPhanBo = new SelectOptionList(dtDotPhanBo, "iID_MaDotPhanBo", "dNgayDotPhanBo");
        if (String.IsNullOrEmpty(iID_MaDotPhanBo))
        {
            if (dtDotPhanBo.Rows.Count > 0)
            {
                iID_MaDotPhanBo = Convert.ToString(dtDotPhanBo.Rows[0]["iID_MaDotPhanBo"]);
            }
            else
            {
                iID_MaDotPhanBo = Guid.Empty.ToString();
            }
        }
        
        String[] arrDSDuocNhapTruongTien = { "rTuChi", "rHienVat" };
        if(String.IsNullOrEmpty(TruongTien))
        TruongTien = arrDSDuocNhapTruongTien[0];
        
        String URLView = "";
        if (PageLoad == "1")
            URLView = Url.Action("ViewPDF", "rptPB_TongHopChiTieuNganSachQuocPhong_9", new {MaND=MaND,sLNS = sLNS, iID_MaDotPhanBo = iID_MaDotPhanBo, iID_MaTrangThaiDuyet = iID_MaTrangThaiDuyet, TruongTien = TruongTien, KhoGiay = KhoGiay,ToSo=ToSo });
        String BackURL = Url.Action("Index", "PhanBo_Report");
        String urlExport = Url.Action("ExportToExcel", "rptPB_TongHopChiTieuNganSachQuocPhong_9", new { MaND = MaND, sLNS = sLNS, iID_MaDotPhanBo = iID_MaDotPhanBo, iID_MaTrangThaiDuyet = iID_MaTrangThaiDuyet, TruongTien = TruongTien, KhoGiay = KhoGiay, ToSo = ToSo });
        using (Html.BeginForm("EditSubmit", "rptPB_TongHopChiTieuNganSachQuocPhong_9", new { ParentID = ParentID,ToDaXem=ToDaXem}))
        {
    %>
    <div class="box_tong">
        <div class="title_tong">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td>
                        <span>Báo cáo tổng hợp số chỉ tiêu ngân sách quốc phòng</span>
                    </td>
                     <td width="52%" style=" text-align:left;">
                           <div class="login1" style=" width:50px; height:20px; text-align:left;"><a style="cursor:pointer;"></a></div>
                </td>
                </tr>
            </table>
        </div>
        <div id="Div1" style="background-color:#F0F9FE;">
            <div id="rptMain" style="padding-top:5px;">
<table width="100%" border="0" cellpadding="0" cellspacing="0" style="height: 105px">
  <tr>
    <td width="500" rowspan="14" align="center" valign="top"><fieldset style="text-align:left;padding:3px 6px;font-size:11px;width:500px; -moz-border-radius: 3px;-webkit-border-radius: 3px;-khtml-border-radius: 3px;border-radius: 3px;border:1px #C0C0C0 solid;">
                            <legend><b><%=NgonNgu.LayXau("Chọn Loại ngân sách ") %></b></legend>
                           <div style="width: 100%; height: 400px; overflow: scroll; border:1px solid black;">
                            <table class="mGrid">
                                <tr>
                               <td><input type="checkbox" id="checkAll" onclick="Chonall(this.checked)" onchange="ChonNLV()"></td>
                                <td> Chọn tất cả LNS </td>
                                </tr>                                                                                 
                                    <%
                                    String TenLNS = ""; String sLNS1 = "";
                                    String _Checked = "checked=\"checked\"";  
                                    for (int i = 0; i < dtLNS.Rows.Count; i++)
                                    {
                                        _Checked = "";
                                        TenLNS = Convert.ToString(dtLNS.Rows[i]["TenHT"]);
                                        sLNS1 = Convert.ToString(dtLNS.Rows[i]["sLNS"]);
                                        for (int j = 0; j < arrLNS.Length; j++)
                                        {
                                            if (sLNS1 == arrLNS[j])
                                            {
                                                _Checked = "checked=\"checked\"";
                                                break;
                                            }
                                        }    
                                    %>
                                    <tr>
                                        <td style="width: 10%;">
                                            <input type="checkbox" value="<%=sLNS1 %>" <%=_Checked %> check-group="sLNS" id="Checkbox1" onchange="ChonNLV()"
                                                name="sLNS" />                                        </td>
                                        <td>
                                            <%=TenLNS%>                                        </td>
                                    </tr>
                                  <%}%>
                                </table> 
                            </div>
                           </fieldset></td>
    <td align="right" class="style2"><b>Trạng thái  : </b></td>
    <td width="142"><%=MyHtmlHelper.DropDownList(ParentID, slTrangThai, iID_MaTrangThaiDuyet, "iID_MaTrangThaiDuyet", "", "class=\"input1_2\" style=\"width: 80%;heigh:22px;\"onchange=ChonNLV()")%></td>
    <td width="184" rowspan="13" valign="top" align="center"><fieldset style="text-align:left;padding:5px 5px 8px 8px;font-size:11px;width:160px; -moz-border-radius: 3px;-webkit-border-radius: 3px;-khtml-border-radius: 3px;border-radius: 3px;border:1px #C0C0C0 solid;">
                            <legend><b><%=NgonNgu.LayXau("In cho loại") %></b></legend>
                            &nbsp;&nbsp;<%=MyHtmlHelper.Option(ParentID, "rTuChi", TruongTien, "TruongTien", "", "onchange=ChonNLV()")%>&nbsp;&nbsp;Tự Chi<br />
                            &nbsp;&nbsp;<%=MyHtmlHelper.Option(ParentID, "rHienVat", TruongTien, "TruongTien", "", "onchange=ChonNLV()")%>&nbsp;&nbsp;Hiện Vật
                           </fieldset>
                           <br />Các tờ đã xem : (<%=ToDaXem %>)</td>
    <td width="170" rowspan="13" valign="top" align="center"><fieldset style="text-align:left;padding:5px 5px 8px 8px;font-size:11px;width:160px; -moz-border-radius: 3px;-webkit-border-radius: 3px;-khtml-border-radius: 3px;border-radius: 3px;border:1px #C0C0C0 solid;">
                            <legend><b><%=NgonNgu.LayXau("In trên giấy") %></b></legend>
                            &nbsp;&nbsp;<%=MyHtmlHelper.Option(ParentID, "1", KhoGiay, "KhoGiay", "","onchange=ChonNLV()")%>&nbsp;&nbsp;A3 - Giấy to<br />
                         &nbsp; <%=MyHtmlHelper.Option(ParentID, "2", KhoGiay, "KhoGiay", "","onchange=ChonNLV()")%>&nbsp;&nbsp;A4 - Giấy nhỏ
                           </fieldset></td>
  </tr>
  <tr>
    <td align="right" class="style2"><b>Đợt phân bổ : </b></td>
    <td id="<%=ParentID %>_divDotPhanBo">
                                     <% rptPB_TongHopChiTieuNganSachQuocPhong_9Controller rpt=new rptPB_TongHopChiTieuNganSachQuocPhong_9Controller();
                                        rptPB_TongHopChiTieuNganSachQuocPhong_9Controller._Data _data=new rptPB_TongHopChiTieuNganSachQuocPhong_9Controller._Data();
                                        _data=rpt.obj_DSDotPhanBo(ParentID,MaND,sLNS,iID_MaDotPhanBo,iID_MaTrangThaiDuyet,TruongTien,KhoGiay,ToSo);
                                         %>
                                         <%=_data.iID_MaDotPhanBo %>
                                     &nbsp;&nbsp;</td>
  </tr>
  <tr>
    <td align="right" class="style2"><b>Chọn tờ :</b></td>
    <td id="<%=ParentID %>_divToSo"><%=_data.ToSo %></td>
  </tr>
  <tr>
    <td align="right" class="style2">&nbsp;</td>
    <td>&nbsp;</td>
  </tr>
  <tr>
    <td align="right" class="style2">&nbsp;</td>
    <td>&nbsp;</td>
  </tr>
  <tr>
    <td align="right" class="style2">&nbsp;</td>
    <td>&nbsp;</td>
  </tr>
  <tr>
    <td align="right" class="style2">&nbsp;</td>
    <td>&nbsp;</td>
  </tr>
  <tr>
    <td align="right" class="style2">&nbsp;</td>
    <td>&nbsp;</td>
  </tr>
  <tr>
    <td align="right" class="style2">&nbsp;</td>
    <td>&nbsp;</td>
  </tr>
  <tr>
    <td align="right" class="style2">&nbsp;</td>
    <td>&nbsp;</td>
  </tr>
  <tr>
    <td align="right" class="style2">&nbsp;</td>
    <td>&nbsp;</td>
  </tr>
  <tr>
    <td align="right" class="style2">&nbsp;</td>
    <td>&nbsp;</td>
  </tr>
  <tr>
    <td align="right" class="style2">&nbsp;</td>
    <td>&nbsp;</td>
  </tr>
  <tr>
    <td colspan="4"> <table cellpadding="0" cellspacing="0" border="0" align="left" style="margin-left: 100px;">
                                <tr>
                                    <td><input type="submit" class="button" id="Submit2" value="<%=NgonNgu.LayXau("Thực hiện")%>" /></td>
                                    <td width="5px"></td>
                                    <td><input class="button" type="button" value="<%=NgonNgu.LayXau("Hủy")%>" onclick="Huy()" /></td>
                                </tr>
                            </table></td>
  </tr>
</table>
            </div>
        </div>
    </div>
   <script type="text/javascript">
       function Huy() {
           window.location.href = '<%=BackURL%>';
       }
   </script>
   <script type="text/javascript">
       $(function () {
           $("div#rptMain").hide();
           $('div.login1 a').click(function () {
               $('div#rptMain').slideToggle('fast');
               $(this).toggleClass('active');
               return false;
           });
       });
    </script>
    <script type="text/javascript">
        function ChonNLV() {
            var sLNS = "";
            $("input:checkbox[check-group='sLNS']").each(function (i) {
                if (this.checked) {
                    if (sLNS != "") sLNS += ",";
                    sLNS += this.value;
                }
            });
            var iID_MaTrangThaiDuyet = document.getElementById("<%=ParentID %>_iID_MaTrangThaiDuyet").value
            var iID_MaDotPhanBo = document.getElementById("<%=ParentID %>_iID_MaDotPhanBo").value
            var ToSo = document.getElementById("<%=ParentID %>_ToSo").value

            var bTruongTien = document.getElementById("<%= ParentID %>_TruongTien").checked;
            var TruongTien = "";
            if (bTruongTien) TruongTien = "rTuChi";
            else TruongTien = "rHienVat";

            var bKhoGiay = document.getElementById("<%= ParentID %>_KhoGiay").checked;
            var KhoGiay = "";
            if (bKhoGiay) KhoGiay = "1";
            else KhoGiay = "2";          

            jQuery.ajaxSetup({ cache: false });
            var url = unescape('<%= Url.Action("Get_dsDotPhanBo?ParentID=#0&MaND=#1&sLNS=#2&iID_MaDotPhanBo=#3&iID_MaTrangThaiDuyet=#4&TruongTien=#5&KhoGiay=#6&ToSo=#7", "rptPB_TongHopChiTieuNganSachQuocPhong_9") %>');
            url = unescape(url.replace("#0", "<%= ParentID %>"));
            url = unescape(url.replace("#1", "<%=MaND %>"));
            url = unescape(url.replace("#2", sLNS));
            url = unescape(url.replace("#3", iID_MaDotPhanBo));
            url = unescape(url.replace("#4", iID_MaTrangThaiDuyet));
            url = unescape(url.replace("#5", TruongTien));
            url = unescape(url.replace("#6", KhoGiay));
            url = unescape(url.replace("#7", ToSo));

            $.getJSON(url, function (data) {
                document.getElementById("<%= ParentID %>_divDotPhanBo").innerHTML = data.iID_MaDotPhanBo;
                document.getElementById("<%= ParentID %>_divToSo").innerHTML = data.ToSo;
            });
        }                                            
    </script>
      <script type="text/javascript">
          function Chonall(sLNS) {
              $("input:checkbox[check-group='sLNS']").each(function (i) {
                  if (sLNS) {
                      this.checked = true;
                  }
                  else {
                      this.checked = false;
                  }

              });
              ChonNLV();
          }                                            
      </script>  
    <%} %>
    <div>
     <div>
    <%=MyHtmlHelper.ActionLink(urlExport, "Export To Excel") %>
    </div>
    </div>
    <%
    %>
    <iframe src="<%=URLView%>" height="600px" width="100%">
    </iframe>
</body>
</html>
