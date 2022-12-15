var TBL_CAP_THANH_TOAN_KPQP = "tbl_capthanhtoankpqp";
var TBL_TAM_UNG_KPQP = "tbl_tamungkpqp";
var TBL_THU_UNG_KPQP = "tbl_thuungkpqp";
var TBL_UNG_XDCB_KHAC = "tbl_ungxdcbkhac";
var TBL_THU_UNG_XDCB_KHAC = "tbl_thuungxdcbkhac";
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var iIDThongTriID = "";

$(document).ready(function () {
    iIDThongTriID = $("#piIdThongTri").val();
    //LayThongTinThongTri(DinhDangSo);
    layData();
    layChiTiet();
});

function DinhDangSo() {
    $(".sotien").each(function () {
        $(this).html(FormatNumber($(this).html().trim()) == "" ? 0 : FormatNumber($(this).html().trim()));
    })
}

function LayThongTinThongTri(callback) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLThongTriThanhToan/LayThongTinChiTietThongTriChiTiet",
        data: { iID_ThongTriID: iIDThongTriID },
        success: function (r) {
            var button = { bUpdate: 0, bDelete: 0, bInfo: 0 };
            if (r != null) {
                var columns = [
                    { sField: "iID_ThongTriChiTietID", bKey: true },
                    { sField: "iID_Parent", bParentKey: true },

                    { sTitle: "Mục", sField: "sM", iWidth: "7%", sTextAlign: "center" },
                    { sTitle: "Tiểu mục", sField: "sTM", iWidth: "7%", sTextAlign: "center" },
                    { sTitle: "Tiết mục", sField: "sTTM", iWidth: "7%", sTextAlign: "center" },
                    { sTitle: "Ngành", sField: "sNG", iWidth: "7%", sTextAlign: "center" },
                    { sTitle: "Nội dung", sField: "sNoiDung", iWidth: "55%", sTextAlign: "left", bHaveIcon: 1 },
                    { sTitle: "Số tiền", sField: "fSoTien", iWidth: "11%", sTextAlign: "right", sClass:"sotien" }];
                if (r.lstTab1 != null) {
                    $("#" + TBL_CAP_THANH_TOAN_KPQP).html(GenerateTreeTable(r.lstTab1, columns, button, TBL_CAP_THANH_TOAN_KPQP));
                } else {
                    $("#" + TBL_CAP_THANH_TOAN_KPQP).html("");
                }

                var columnsNoTM = [
                    { sField: "iID_ThongTriChiTietID", bKey: true },
                    { sField: "iID_Parent", bParentKey: true },
                  
                    { sTitle: "Nội dung", sField: "sNoiDung", iWidth: "55%", sTextAlign: "left", bHaveIcon: 1 },
                    { sTitle: "Số tiền", sField: "fSoTien", iWidth: "11%", sTextAlign: "right", sClass: "sotien" }];

                if (r.lstTab2 != null) {
                    $("#" + TBL_TAM_UNG_KPQP).html(GenerateTreeTable(r.lstTab2, columnsNoTM, button, TBL_TAM_UNG_KPQP));
                } else {
                    $("#" + TBL_TAM_UNG_KPQP).html("");
                }

                if (r.lstTab3 != null) {
                    $("#" + TBL_THU_UNG_KPQP).html(GenerateTreeTable(r.lstTab3, columnsNoTM, button, TBL_THU_UNG_KPQP));
                } else {
                    $("#" + TBL_THU_UNG_KPQP).html("");
                }

                if (callback)
                    callback();
            }
        }
    });
}

function Huy() {
    window.history.back();
}

function layData() {
    var piIdDonVi = $("#piIdDonVi").val();
    var psMaNguonVon = $("#psMaNguonVon").val();
    //var psMaThongTri = $("#psMaThongTri").val();
    var piNamNganSach = $("#piNamNganSach").val();
    var psMoTa = $("#psMoTa").val();

    $("#iID_DonViQuanLyID").val(piIdDonVi);
    $("#sMaNguonVon").val(psMaNguonVon);
    $("#iNamNganSach").val(piNamNganSach);
    $("#sMoTa").val(psMoTa);
}

function layChiTiet() {
    var iLoaiThongTri = $("#piLoaiThongTri").val();
    $.ajax({
        url: "/QLVonDauTu/QLThongTriThanhToan/GetThongTriChiTiet",
        type: "POST",
        data: { id: iIDThongTriID, iLoaiThongTri: iLoaiThongTri },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null && data != "") {
                var bIsActive = false;

                if (data.lstThanhToan.length > 0) {
                    lstThanhToan = data.lstThanhToan;
                    var htmlThanhToan = "";
                    data.lstThanhToan.forEach(function (x) {
                        htmlThanhToan += "<tr>";
                        htmlThanhToan += " <td align=\"center\">" + x.SM + "</td>";
                        htmlThanhToan += " <td align=\"center\">" + x.STm + "</td>";
                        htmlThanhToan += " <td align=\"center\">" + x.STtm + "</td>";
                        htmlThanhToan += " <td align=\"center\">" + x.SNg + "</td>";
                        htmlThanhToan += " <td align=\"center\">" + x.STenLoaiCongTrinh + "</td>";
                        htmlThanhToan += " <td align=\"center\">" + x.STenDuAn + "</td>";
                        htmlThanhToan += " <td align=\"center\">" + FormatNumber(x.FSoTien) + "</td>";
                        htmlThanhToan += "</tr>";
                    });
                    $("#tblThanhToan tbody").html(htmlThanhToan);
                    $("#thanhtoan").addClass("active");
                    $("#tabThanhToan").removeClass("hidden");
                    $("#tabThanhToan").addClass("active");
                    bIsActive = true;
                }
                //else {
                //    $("#tblThanhToan").html('');
                //}
                if (data.lstThuHoi.length > 0) {
                    lstThuHoi = data.lstThuHoi;
                    var htmlThuHoi = "";
                    data.htmlThuHoi.forEach(function (x) {
                        htmlThuHoi += "<tr>";
                        htmlThuHoi += " <td align=\"center\">" + x.SM + "</td>";
                        htmlThuHoi += " <td align=\"center\">" + x.STm + "</td>";
                        htmlThuHoi += " <td align=\"center\">" + x.STtm + "</td>";
                        htmlThuHoi += " <td align=\"center\">" + x.SNg + "</td>";
                        htmlThuHoi += " <td align=\"center\">" + x.STenLoaiCongTrinh + "</td>";
                        htmlThuHoi += " <td align=\"center\">" + x.STenDuAn + "</td>";
                        htmlThuHoi += " <td align=\"center\">" + FormatNumber(x.FSoTien) + "</td>";
                        htmlThuHoi += "</tr>";
                    });
                    $("#tblThuHoi tbody").html(htmlThuHoi);
                    if (bIsActive == false) {
                        $("#thuhoi").addClass("active");
                    }
                    bIsActive = true;
                    $("#tabThuHoi").removeClass("hidden");
                }
                //else {
                //    $("#tblThuHoi").html('');
                //}
                if (data.lstTamUng.length > 0) {
                    lstTamUng = data.lstTamUng;
                    var htmlTamUng = "";
                    data.lstTamUng.forEach(function (x) {
                        htmlTamUng += "<tr>";
                        htmlTamUng += " <td align=\"center\">" + x.SM + "</td>";
                        htmlTamUng += " <td align=\"center\">" + x.STm + "</td>";
                        htmlTamUng += " <td align=\"center\">" + x.STtm + "</td>";
                        htmlTamUng += " <td align=\"center\">" + x.SNg + "</td>";
                        htmlTamUng += " <td align=\"center\">" + x.STenLoaiCongTrinh + "</td>";
                        htmlTamUng += " <td align=\"center\">" + x.STenDuAn + "</td>";
                        htmlTamUng += " <td align=\"center\">" + FormatNumber(x.FSoTien) + "</td>";
                        htmlTamUng += "</tr>";

                    });
                    $("#tblTamUng tbody").html(htmlTamUng);
                    if (bIsActive == false) {
                        $("#tabTamUng").addClass("active");
                        $("#tamung").addClass("active");
                    }
                    bIsActive = true;
                    $("#tabTamUng").removeClass("hidden");
                }
                //else {
                //    $("#tblThanhToan").html('');
                //}
                if (data.lstKinhPhi.length > 0) {
                    lstKinhPhi = data.lstKinhPhi;
                    var htmlKinhPhi = "";
                    data.lstKinhPhi.forEach(function (x) {
                        htmlKinhPhi += "<tr>";
                        htmlKinhPhi += " <td align=\"center\">" + x.SM + "</td>";
                        htmlKinhPhi += " <td align=\"center\">" + x.STm + "</td>";
                        htmlKinhPhi += " <td align=\"center\">" + x.STtm + "</td>";
                        htmlKinhPhi += " <td align=\"center\">" + x.SNg + "</td>";
                        htmlKinhPhi += " <td align=\"center\">" + x.STenLoaiCongTrinh + "</td>";
                        htmlKinhPhi += " <td align=\"center\">" + x.STenDuAn + "</td>";
                        htmlKinhPhi += " <td align=\"center\">" + FormatNumber(x.FSoTien) + "</td>";
                        htmlKinhPhi += "</tr>";

                    });
                    $("#tblKinhPhi tbody").html(htmlKinhPhi);
                    if (bIsActive == false) {
                        $("#kinhphi").addClass("active");
                        $("#tabKinhPhi").addClass("active");
                    }
                    bIsActive = true;
                    $("#tabKinhPhi").removeClass("hidden");
                }
                //else {
                //    $("#tblThanhToan").html('');
                //}
                if (data.lstHopThuc.length > 0) {
                    lstHopThuc = data.lstHopThuc;
                    var htmlHopThuc = "";
                    data.lstHopThuc.forEach(function (x) {
                        htmlHopThuc += "<tr>";
                        htmlHopThuc += " <td align=\"center\">" + x.SM + "</td>";
                        htmlHopThuc += " <td align=\"center\">" + x.STm + "</td>";
                        htmlHopThuc += " <td align=\"center\">" + x.STtm + "</td>";
                        htmlHopThuc += " <td align=\"center\">" + x.SNg + "</td>";
                        htmlHopThuc += " <td align=\"center\">" + x.STenLoaiCongTrinh + "</td>";
                        htmlHopThuc += " <td align=\"center\">" + x.STenDuAn + "</td>";
                        htmlHopThuc += " <td align=\"center\">" + FormatNumber(x.FSoTien) + "</td>";
                        htmlHopThuc += "</tr>";
                    });
                    $("#tblHopThuc tbody").html(htmlHopThuc);
                    if (bIsActive == false) {
                        $("#hopthuc").addClass("active");
                        $("#tabHopThuc").addClass("active");
                    }
                    $("#tabHopThuc").removeClass("hidden");
                    bIsActive = true;
                }
                //else {
                //    $("#tblThanhToan").html('');
                //}
            }

        },
        error: function (data) {

        }
    });
}