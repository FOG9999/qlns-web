var TBL_CAP_THANH_TOAN_KPQP = "tbl_capthanhtoankpqp";
var TBL_TAM_UNG_KPQP = "tbl_tamungkpqp";
var TBL_THU_UNG_KPQP = "tbl_thuungkpqp";
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var iIDThongTriID = "";
var isClickLoc = false;

var lstThanhToan = [];
var lstThuHoi = [];
var lstTamUng = [];
var lstKinhPhi = [];
var lstHopThuc = [];

$(document).ready(function () {
    iIDThongTriID = $("#piIdThongTri").val();
    LayThongTinThongTriCu(DinhDangSo);
    layData();
    layChiTiet();
    DinhDangSo();
    $("#sMaThongTri").keyup(function (event) {
        ValidateMaxLength(this, 50);
    })
    $("#sMoTa").keyup(function (event) {
        ValidateMaxLength(this, 500);
    })
});

//function DinhDangSo() {
//    $(".currently").each(function () {
//        $(this).html(FormatNumber($(this).html().trim()) == "" ? 0 : FormatNumber($(this).html().trim()));
//    })
//}

function LayThongTinThongTriCu(callback) {
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
                    { sTitle: "Số tiền", sField: "fSoTien", iWidth: "11%", sTextAlign: "right", sClass: "sotien" }];
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

function Luu() {
    var thongTri = {};
    thongTri.iID_ThongTriID = iIDThongTriID;
    //thongTri.sMaThongTri = $("#sMaThongTri").val();
    //thongTri.sNguoiLap = $("#sNguoiLap").val();
    //thongTri.sTruongPhong = $("#sTruongPhong").val();
    //thongTri.sThuTruongDonVi = $("#sThuTruongDonVi").val();
    //thongTri.dNgayLapGanNhat = $("#dNgayLapGanNhat").html();
    var psMoTa = $("#sMoTa").val();
    var psMaThongTri = $("#sMaThongTri").val();

    $.ajax({
        url: "/QLVonDauTu/QLThongTriThanhToan/CapNhatThongTri",
        type: "POST",
        data: { id: thongTri.iID_ThongTriID, sMoTa: psMoTa, sMaThongTri: psMaThongTri },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null && data != '') {
                $.ajax({
                    url: "/QLVonDauTu/QLThongTriThanhToan/SaveThongTriChiTiet",
                    type: "POST",
                    data: {
                        thongTriId: data,
                        lstThanhToan: lstThanhToan,
                        lstThuHoi: lstThuHoi,
                        lstTamUng: lstTamUng,
                        lstKinhPhi: GetValueDataInGrid(lstKinhPhi),
                        lstHopThuc: GetValueDataInGrid(lstHopThuc)
                    },
                    dataType: "json",
                    cache: false,
                    async: false,
                    success: function (data) {
                        console.log(data);
                        window.location.href = "/QLVonDauTu/QLThongTriThanhToan/ChiTiet/" + iIDThongTriID;
                    },
                    error: function (data) {
                        console.log(data);
                        window.location.href = "/QLVonDauTu/QLThongTriThanhToan/ChiTiet/" + iIDThongTriID;
                    }
                });
            }
            //window.location.href = "/QLVonDauTu/QLThongTriThanhToan/Index";
        },
        error: function (data) {
            console.log(data);
        }
    });
}

function GetValueDataInGrid(lstData) {
    if (lstData == null || lstData.length == 0) return [];
    var results = [];
    $.each(lstData, function (index, item) {
        var obj = item;
        //obj.FSoTien = parseInt($("#" + item.id).val());
        obj.FSoTien = UnFormatNumber($("#" + item.id).val());
        results.push(obj);
    });
    return results;
}

function CheckLoi(doiTuong) {
    var messErr = [];
    if (doiTuong.sMaThongTri == "")
        messErr.push("Mã thông tri chưa có hoặc chưa chính xác.");

    if (doiTuong.sNguoiLap == "")
        messErr.push("Người lập thông tri chưa có hoặc chưa chính xác.");

    if (doiTuong.sThuTruongDonVi == "")
        messErr.push("Thủ trưởng đơn vị chưa có hoặc chưa chính xác.");

    if (KiemTraTrungMaThongTri(doiTuong.sMaThongTri) == true)
        messErr.push("Mã thông tri đã tồn tại, vui lòng nhập mã khác.");

    if ($("#" + TBL_CAP_THANH_TOAN_KPQP + " table tr").length == 0
        && $("#" + TBL_TAM_UNG_KPQP + " table tr").length == 0
        && $("#" + TBL_TAM_UNG_KPQP + " table tr").length == 0
    )
        messErr.push("Không có dữ liệu chi tiết.");

    if (messErr.length > 0) {
        alert(messErr.join("\n"));
        return false;
    } else {
        return true;
    }
}

function KiemTraTrungMaThongTri(sMaThongTri) {
    var check = false;
    $.ajax({
        url: "/QLVonDauTu/QLThongTriThanhToan/KiemTraTrungMaThongTri",
        type: "POST",
        data: { sMaThongTri: sMaThongTri, iID_ThongTriID: iIDThongTriID },
        dataType: "json",
        async: false,
        cache: false,
        success: function (data) {
            check = data;
        },
        error: function (data) {

        }
    })
    return check;
}

function Loc() {
    var iIDMaDonVi = $("#iID_DonViID").val();
    var iNguonVon = $("#iNguonVon").val();
    var dNgayLapGanNhat = $("#dNgayLapGanNhat").html();
    var dNgayTaoThongTri = $("#dNgayThongTri").html();
    var iNamThongTri = $("#iNamThongTri").html();
    isClickLoc = true;
    LayThongTinDeNghiThanhToan(iIDMaDonVi, iNguonVon, dNgayLapGanNhat, dNgayTaoThongTri, iNamThongTri, DinhDangSo);
}

function LayThongTinDeNghiThanhToan(iIDMaDonVi, iNguonVon, dNgayLapGanNhat, dNgayTaoThongTri, iNamThongTri, callback) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLThongTriThanhToan/GetDeNghiThanhToanChiTiet",
        data: { iID_MaDonVi: iIDMaDonVi, iNguonVon: iNguonVon, dNgayLapGanNhat: dNgayLapGanNhat, dNgayTaoThongTri: dNgayTaoThongTri, iNamThongTri: iNamThongTri },
        success: function (data) {
            var button = { bUpdate: 0, bDelete: 0, bInfo: 0 };
            if (data != null && data.data != null) {
                var columnsTab1 = [
                    { sField: "iID_DeNghiThanhToan_ChiTietID", bKey: true },
                    { sField: "iID_Parent", bParentKey: true },

                    { sTitle: "Mục", sField: "sM", iWidth: "7%", sTextAlign: "center" },
                    { sTitle: "Tiểu mục", sField: "sTM", iWidth: "7%", sTextAlign: "center" },
                    { sTitle: "Tiết mục", sField: "sTTM", iWidth: "7%", sTextAlign: "center" },
                    { sTitle: "Ngành", sField: "sNG", iWidth: "7%", sTextAlign: "center" },
                    { sTitle: "Nội dung", sField: "sNoiDung", iWidth: "55%", sTextAlign: "left", bHaveIcon: 1 },
                    { sTitle: "Số tiền", sField: "fGiaTriThanhToan", iWidth: "11%", sTextAlign: "right", sClass: "sotien" }];
                $("#" + TBL_CAP_THANH_TOAN_KPQP).html(GenerateTreeTable(data.data, columnsTab1, button, TBL_CAP_THANH_TOAN_KPQP));

                var columnsTab2 = [
                    { sField: "iID_DeNghiThanhToan_ChiTietID", bKey: true },
                    { sField: "iID_Parent", bParentKey: true },

                    { sTitle: "Nội dung", sField: "sNoiDung", iWidth: "55%", sTextAlign: "left", bHaveIcon: 1 },
                    { sTitle: "Số tiền", sField: "fGiaTriTamUng", iWidth: "11%", sTextAlign: "right", sClass: "sotien" }];
                $("#" + TBL_TAM_UNG_KPQP).html(GenerateTreeTable(data.data, columnsTab2, button, TBL_TAM_UNG_KPQP));

                var columnsTab3 = [
                    { sField: "iID_DeNghiThanhToan_ChiTietID", bKey: true },
                    { sField: "iID_Parent", bParentKey: true },

                    { sTitle: "Nội dung", sField: "sNoiDung", iWidth: "55%", sTextAlign: "left", bHaveIcon: 1 },
                    { sTitle: "Số tiền", sField: "fGiaTriThuHoi", iWidth: "11%", sTextAlign: "right", sClass: "sotien" }];
                $("#" + TBL_THU_UNG_KPQP).html(GenerateTreeTable(data.data, columnsTab3, button, TBL_THU_UNG_KPQP));
            } else {
                $("#" + TBL_CAP_THANH_TOAN_KPQP).html("");
                $("#" + TBL_TAM_UNG_KPQP).html("");
                $("#" + TBL_THU_UNG_KPQP).html("");
            }

            if (callback)
                callback();
        }
    });
}

function DinhDangSo() {
    $(".currently").each(function () {
        $(this).val(FormatNumber($(this).val().trim()) == "" ? 0 : FormatNumber($(this).val().trim()));
    })
}

function XoaTextThongTri() {
    $("#sMaThongTri").val("");
    $("#sNguoiLap").val("");
    $("#sTruongPhong").val("");
    $("#sThuTruongDonVi").val("");
}

function Huy() {
    var isFromThongTri = $("#isFromThongTri").val();

    if (parseInt(isFromThongTri) == 0) {
        window.location.href = "/QLVonDauTu/GiaiNganThanhToan/Index?isBackFromThongTri="+1;
    } else if(isFromThongTri == ""){
        window.location.href = "/QLVonDauTu/QLThongTriThanhToan/Index";
    }
}

function layData() {
    var piIdDonVi = $("#piIdDonVi").val();
    //var piNamThongTri = $("#piNamThongTri").val();
    var psMaNguonVon = $("#psMaNguonVon").val();
    //var piLoaiThongTri = $("#piLoaiThongTri").val();
    var psMaThongTri = $("#psMaThongTri").val();
    //var pdNgayThongTri = $("#pdNgayThongTri").val();
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
                //var temp = Html.Raw(x.FSoTien == 0 ? "" : x.FSoTien.ToString("###,###"));
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
                        htmlThanhToan += " <td> <input style=' text-align:right' class='form-control currently'  value=" + x.FSoTien + " id='" + x.id + "'></td>";
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
                        htmlThuHoi += "<input class='id_input' value=" + x.IIdDeNghiThanhToanId + "/>";
                        htmlThuHoi += " <td align=\"center\">" + x.SM + "</td>";
                        htmlThuHoi += " <td align=\"center\">" + x.STm + "</td>";
                        htmlThuHoi += " <td align=\"center\">" + x.STtm + "</td>";
                        htmlThuHoi += " <td align=\"center\">" + x.SNg + "</td>";
                        htmlThuHoi += " <td align=\"center\">" + x.STenLoaiCongTrinh + "</td>";
                        htmlThuHoi += " <td align=\"center\">" + x.STenDuAn + "</td>";
                        htmlThuHoi += " <td> <input style=' text-align:right' class='form-control currently'  value=" + x.FSoTien + " id='" + x.id + "'></td>";
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
                        htmlTamUng += " <td> <input style=' text-align:right' class='form-control currently'  value=" + x.FSoTien + " id='" + x.id + "'></td>";
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
                        htmlKinhPhi += " <td> <input style=' text-align:right' class='form-control currently' value=" + x.FSoTien + " id='" + x.id + "'></td>";
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
                        htmlHopThuc += " <td> <input style=' text-align:right' class='form-control currently' value=" + x.FSoTien + " id='" + x.id + "'></td>";
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
                //triggerCurrently();
                DinhDangSo();
            }

        },
        error: function (data) {

        }
    });
}

function triggerCurrently() {
    $(".currently").on("change", function () {
        if ($(this).val() == null) {
            $(this).val(0);
        } else {
            if ($.isNumeric($(this).val())) {
                var sNumber = Number($(this).val()).toLocaleString('vi-VN');
                $(this).val(sNumber);
            }
            else {
                $(this).val(0);
            }
        }
    });
}