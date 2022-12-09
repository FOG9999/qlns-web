var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;
var iIdDeNghiQuyetToanId = $("#iIdDeNghiQuyetToanId").val();
var iIdDonVi = $("#txt_DonViQuanLy_Value").val();
var iIdDuAn = $("#txt_DuAn_Value").val();
var iIdLoaiquyettoanId = $("#txt_LoaiQuyetToan_Value").val();
var iIdDuToan = $("#txt_DuToan_Value").val();
var isDetail = $("#isDetail").val();

$(document).ready(function () {
    $("#txt_DonViQuanLy").select2({
        width: 'resolve',
        matcher: matchStart
    });

    $("#txt_DuAn").select2({
        width: 'resolve',
        matcher: matchStart
    });

    $("#txt_LoaiQuyetToan").select2({
        width: 'resolve',
        matcher: matchStart
    });

    $("#iIđuToanId").select2({
        width: 'resolve',
        matcher: matchStart
    });

    LoadDataView();
    EventChangeValue();
});

function LoadDataView() {

    SetDataComboBoxDonViQuanLy();
    LoadDataComboBoxDuAn(iIdDonVi);
    LoadDataComboBoxLoaiQuyetToan(iIdDeNghiQuyetToanId); 
    LoadDataComboboxDuToan(iIdLoaiquyettoanId, iIdDuAn);
}

function EventChangeValue() {
    $("#idFormDuAn").attr('hidden', true);
    $("#txt_DonViQuanLy").attr('disabled', true);
    $("#txt_DuAn").attr('disabled', true);
    $("#idFormDonViQuanLy").attr('hidden', true);
    $("#txt_LoaiQuyetToan").attr('disabled', true);
    $("#iIđuToanId").attr('disabled', true);
    
}

function RejectViewIndex() {
    window.location.href = "/QLVonDauTu/VDT_QT_DeNghiQuyetToan";
}

function SetDataComboBoxDonViQuanLy() {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/GetDonViQuanLy",
        success: function (resp) {
            console.log(resp);
            if (resp.status == true) {
                $("#txt_DonViQuanLy").select2({
                    data: resp.data
                });

                $("#txt_DonViQuanLy").val(iIdDonVi).trigger("change");
            }
        }
    });
}

function LoadDataComboBoxDuAn(idDonVi) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/GetDuAnTheoDonViQuanLy",
        data: { idDonVi: idDonVi, iIdDeNghiQuyetToanId: iIdDeNghiQuyetToanId },
        success: function (resp) {
            if (resp.status == true) {
                $("#txt_DuAn").select2({
                    data: resp.data
                });
                $("#txt_DuAn").val(iIdDuAn).trigger("change");
                //GetListChiPhiHangMuc(iIdDuAn);
            }
        }
    });
}

function LoadDataComboboxDuToan(iIdLoaiQuyetToan, iIđuAnId) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/GetListDuToanByDuAn",
        data: { iIdLoaiQuyetToan: iIdLoaiQuyetToan, iIdDuAnId: iIđuAnId },
        success: function (resp) {
            $("#iIđuToanId").empty();
            if (resp.datas != null && resp.datas.length != 0) {
                $("#iIđuToanId").select2({
                    data: resp.datas
                });
                $("#iIđuToanId").val(iIdDuToan).trigger("change");
                if ($('#txt_LoaiQuyetToan').val() == 1)
                    GetListChiPhiHangMuc(iIdDuToan);
                if ($('#txt_LoaiQuyetToan').val() == 2)
                    GetListGoiThau(iIdDuToan);
            }
            if (resp.sKhoiCong != null && resp.sKetThuc) {
                document.getElementById("txtThoiGianKhoiCong").value = resp.sKhoiCong;
                document.getElementById("txtThoiGianHoanThanh").value = resp.sKetThuc;
            }
        }
    });
}

function LoadDataComboBoxLoaiQuyetToan(iIdDeNghiQuyetToanId) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/GetLoaiQuyetToan",
        data: {iIdDeNghiQuyetToanId: iIdDeNghiQuyetToanId },
        success: function (resp) {
            if (resp.status == true) {
                $("#txt_LoaiQuyetToan").select2({
                    data: resp.data
                });
            }
            LoadDataComboboxDuToan(iIdLoaiquyettoanId, iIdDuAn);
        }
    });

}

function GetDuLieuDuAn(idDuAn) {
    $("#txt_TenDuAn").html("");
    $("#txt_ChuDauTu").html("");
    if (idDuAn != null && idDuAn != "" && idDuAn != GUID_EMPTY) {
        $.ajax({
            type: "POST",
            url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/GetDuLieuDuAnByIdDonViQuanLy",
            data: { idDuAn: idDuAn },
            success: function (resp) {
                if (resp.status == true) {
                    if (resp.data.sTenDuAn != null && resp.data.sTenDuAn != "") {
                        $("#txt_TenDuAn").html(resp.data.sTenDuAn);
                    }
                    if (resp.data.sTenChuDauTu != null && resp.data.sTenChuDauTu != "") {
                        $("#txt_ChuDauTu").html("Chủ đầu tư: " + resp.data.sTenChuDauTu);
                    }
                    var html = "";
                    if (resp.lstNguonVon != null && resp.lstNguonVon.length > 0) {
                        resp.lstNguonVon.forEach(function (item) {
                            html += "<tr data-id='" + item.iID_NguonVonID + "'>";
                            html += "<td>" + item.sTenNguonVon + "</td>";
                            html += "<td>" + item.sTienPheDuyet + "</td>";
                            //html += "<td><input type='text' class='form-control txtTienCDTQuyetToan text-right' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
                            html += "</tr>";
                        })
                    }
                    $("#tblDanhSachNguonVon tbody").html(html);
                }
            }
        });
    }
}

var arrChiPhi = [];
var arrHangMuc = [];
var arrGoiThau = [];
//function GetListChiPhiHangMuc(idDuAn) {
//    arrChiPhi = [];
//    arrHangMuc = [];
//    if (idDuAn != null && idDuAn != "" && idDuAn != GUID_EMPTY) {
//        $.ajax({
//            type: "POST",
//            url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/GetListChiPhiHangMucTheoDuAn",
//            data: { idDuAn: idDuAn, iIdDeNghiQuyetToan: iIdDeNghiQuyetToanId },
//            success: function (resp) {
//                if (resp.lstChiPhi != null && resp.lstChiPhi.length > 0) {
//                    arrChiPhi = resp.lstChiPhi;
//                }
//                if (resp.lstHangMuc != null && resp.lstHangMuc.length > 0) {
//                    arrHangMuc = resp.lstHangMuc;
//                }
//                DrawTableChiPhiHangMuc();
//            }
//        });
//    }
//}
function GetListGoiThau(iIdKhlcNhaThau) {
    arrChiPhi = [];
    arrHangMuc = [];
    arrGoiThau = [];
    var iIdDeNghiQuyetToan = $("#iIdDeNghiQuyetToanId").val();
    if (iIdKhlcNhaThau != null && iIdKhlcNhaThau != "" && iIdKhlcNhaThau != GUID_EMPTY) {
        $.ajax({
            type: "POST",
            url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/GetListGoiThau",
            data: { iIdKhlcNhaThau: iIdKhlcNhaThau, iIdDeNghiQuyetToan: iIdDeNghiQuyetToan },
            success: function (resp) {
                if (resp.lstGoiThau != null && resp.lstGoiThau.length > 0) {
                    arrGoiThau = resp.lstGoiThau;
                }
                DrawTableGoiThau(resp.sumGiaTriQuyetToanAB, resp.sumKetQuaKiemToan, resp.sumCDTDeNghiQuyetToan);
            }
        });
    }
}

function GetListChiPhiHangMuc(iIdDuToan) {
    arrChiPhi = [];
    arrHangMuc = [];
    if (iIdDuToan != null && iIdDuToan != "" && iIdDuToan != GUID_EMPTY) {
        $.ajax({
            type: "POST",
            url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/GetListChiPhiHangMucTheoDuAn",
            data: { iIdDuToanId: iIdDuToan, iIdDeNghiQuyetToan: iIdDeNghiQuyetToanId },
            success: function (resp) {
                if (resp.lstChiPhi != null && resp.lstChiPhi.length > 0) {
                    arrChiPhi = resp.lstChiPhi;
                }
                if (resp.lstHangMuc != null && resp.lstHangMuc.length > 0) {
                    arrHangMuc = resp.lstHangMuc;
                }
                DrawTableChiPhiHangMuc(resp.sumGiaTriQuyetToanAB, resp.sumKetQuaKiemToan, resp.sumCDTDeNghiQuyetToan);
            }
        });
    }
}

function DrawTableChiPhiHangMuc(sumGiaTriQuyetToanAB, sumKetQuaKiemToan, sumCDTDeNghiQuyetToan) {
    var html = "";

    //them dong tong so
    html += "<tr style='font-weight:bold' data-id='-1'>";
    html += "<td></td>";
    html += "<td>Tổng số</td>";
    html += "<td></td>";
    html += "<td></td>";
    html += "<td class='text-right txtTongGiaTriQuyetToanAB'>" + sumGiaTriQuyetToanAB + "</td>";
    html += "<td class='text-right txtTongKetQuaKiemToan'>" + sumKetQuaKiemToan + "</td>";
    html += "<td class='text-right txtTongCDTDeNghiQuyetToan'>" + sumCDTDeNghiQuyetToan + "</td>";
    html += "<td></td>";
    html += "<td></td>";
    html += "<td></td>";
    html += "</tr>";

    arrChiPhi.forEach(function (itemCp) {
        var arrChiPhiChild = arrChiPhi.filter(x => x.iID_ChiPhi_Parent == itemCp.iID_DuAn_ChiPhi);
        var arrHangMucByChiPhi = arrHangMuc.filter(x => x.iID_DuAn_ChiPhi == itemCp.iID_DuAn_ChiPhi);
        var disabled = "", isBold = "";
        if ((arrChiPhiChild != null && arrChiPhiChild.length > 0) || (arrHangMucByChiPhi != null && arrHangMucByChiPhi.length > 0)) {
            
            disabled = "disabled";
        }
        isBold = "font-weight: bold;";
        html += "<tr data-loai='1' style='" + isBold+"' data-id='" + itemCp.iID_DuAn_ChiPhi + "' data-parentid='" + itemCp.iID_ChiPhi_Parent+"'>";
        html += "<td class='stt'></td>";
        html += "<td>Chi phí</td>";
        html += "<td>" + itemCp.sTenChiPhi + "</td>";
        html += "<td class='text-right'>" + itemCp.sTienPheDuyet + "</td>";
        html += "<td><input type='text' class='form-control clearable text-right txtGiaTriQuyetToanAB' " + disabled + " value='" + itemCp.sGiaTriQuyetToanAB + "'   onchange='changeGiaTri(this)' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
        html += "<td><input type='text' class='form-control clearable text-right txtKetQuaKiemToan' " + disabled + " value='" + itemCp.sGiaTriKiemToan + "'  onchange='changeGiaTri(this)' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
        html += "<td><input type='text' class='form-control clearable text-right txtCDTDeNghiQuyetToan' " + disabled + " value='" + itemCp.sGiaTriDeNghiQuyetToan + "'  onchange='changeGiaTri(this)' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
        html += "<td class='text-right txtChenhLechSoVoiDuToan'>" + itemCp.sChenhLenhSoVoiDuToan + "</td>";
        html += "<td class='text-right txtChenhLechSoVoiQuyetToanAB'>" + itemCp.sChenhLenhSoVoiQuyetToanAB + "</td>";
        html += "<td class='text-right txtChenhLechSoVoiKetQuaKiemToan'>" + itemCp.sChenhLechSoVoiKetQuaKiemToan + "</td>";
        html += "</tr>";

        // list hang muc
        if (arrHangMucByChiPhi != null && arrHangMucByChiPhi.length > 0) {
            arrHangMucByChiPhi.forEach(function (itemHm) {
                disabled = "";
                isBold = "";
                var arrHangMucChild = arrHangMucByChiPhi.filter(x => x.iID_ParentID == itemHm.iID_HangMucID);
                if (arrHangMucChild != null && arrHangMucChild.length > 0) {
                    //isBold = "font-weight: bold;";
                    disabled = "disabled";
                }
                html += "<tr style='" + isBold+"' data-loai='2' data-id='" + itemHm.iID_HangMucID + "' data-parentid='" + itemHm.iID_ParentID + "' data-chiphi='" + itemHm.iID_DuAn_ChiPhi + "'>";
                html += "<td class='stt'></td>";
                html += "<td style='font-style: italic'>Hạng mục</td>";
                html += "<td style='font-style: italic'>" + itemHm.sTenHangMuc + "</td>";
                html += "<td class='text-right'>" + itemHm.sTienPheDuyet + "</td>";
                html += "<td><input type='text' class='form-control clearable text-right txtGiaTriQuyetToanAB' value='" + itemHm.sGiaTriQuyetToanAB + "' " + disabled + " onchange='changeGiaTri(this)' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
                html += "<td><input type='text' class='form-control clearable text-right txtKetQuaKiemToan' value='" + itemHm.sGiaTriKiemToan + "' " + disabled + " onchange='changeGiaTri(this)' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
                html += "<td><input type='text' class='form-control clearable text-right txtCDTDeNghiQuyetToan' value='" + itemHm.sGiaTriDeNghiQuyetToan + "' " + disabled + " onchange='changeGiaTri(this)' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
                html += "<td class='text-right txtChenhLechSoVoiDuToan'>" + itemHm.sChenhLenhSoVoiDuToan + "</td>";
                html += "<td class='text-right txtChenhLechSoVoiQuyetToanAB'>" + itemHm.sChenhLenhSoVoiQuyetToanAB + "</td>";
                html += "<td class='text-right txtChenhLechSoVoiKetQuaKiemToan'>" + itemHm.sChenhLechSoVoiKetQuaKiemToan + "</td>";
                html += "</tr>";
            })
        }
    })
    $("#tblDanhSachChiTiet tbody").html(html);
    $(".div_ChiTiet").show();

    if (isDetail == 1) {
        $(".detail-panel").find("input").attr("disabled", "disabled");
        $(".btn-save").hide();
    }
}

function DrawTableGoiThau(sumGiaTriQuyetToanAB, sumKetQuaKiemToan, sumCDTDeNghiQuyetToan) {
    var html = "";
    var index = 0;

    //them dong tong so
    html += "<tr style='font-weight:bold' data-id='-1'>";
    html += "<td></td>";
    html += "<td>Tổng số</td>";
    html += "<td></td>";
    html += "<td></td>";
    html += "<td class='text-right txtTongGiaTriQuyetToanAB'>" + sumGiaTriQuyetToanAB + "</td>";
    html += "<td class='text-right txtTongKetQuaKiemToan'>" + sumKetQuaKiemToan + "</td>";
    html += "<td class='text-right txtTongCDTDeNghiQuyetToan'>" + sumCDTDeNghiQuyetToan + "</td>";
    html += "<td></td>";
    html += "<td></td>";
    html += "<td></td>";
    html += "</tr>";

    arrGoiThau.forEach(function (itemCp) {
        index++;
        var disabled = "", isBold = "";
        html += "<tr data-loai='1' style='" + isBold + "' data-id='" + itemCp.iID_DuAn_GoiThau + "' data-parentid='" + itemCp.iID_GoiThau_Parent + "'>";
        html += "<td class='stt text-center'>" + index + "</td>";
        html += "<td>Gói thầu</td>";
        html += "<td>" + itemCp.sTenGoiThau + "</td>";
        html += "<td class='text-right'>" + itemCp.sTienPheDuyet + "</td>";
        html += "<td><input type='text' class='form-control clearable text-right txtGiaTriQuyetToanAB' value='" + itemCp.sGiaTriDeNghiQuyetToan + "' " + disabled + " onchange='changeGiaTriGoiThau(this)' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
        html += "<td><input type='text' class='form-control clearable text-right txtKetQuaKiemToan' value='" + itemCp.sGiaTriKiemToan + "' " + disabled + " onchange='changeGiaTriGoiThau(this)' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
        html += "<td><input type='text' class='form-control clearable text-right txtCDTDeNghiQuyetToan' value='" + itemCp.sGiaTriQuyetToanAB + "' " + disabled + " onchange='changeGiaTriGoiThau(this)' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
        html += "<td class='text-right txtChenhLechSoVoiDuToan'>" + itemCp.sChenhLenhSoVoiDuToan + "</td>";
        html += "<td class='text-right txtChenhLechSoVoiQuyetToanAB'>" + itemCp.sChenhLenhSoVoiQuyetToanAB + "</td>";
        html += "<td class='text-right txtChenhLechSoVoiKetQuaKiemToan'>" + itemCp.sChenhLechSoVoiKetQuaKiemToan + "</td>";
        html += "</tr>";
        $("#tblDanhSachChiTiet tbody").html(html);
        $(".div_ChiTiet").show();
    })
}

function changeGiaTriGoiThau(input) {
    var dongHienTai = $(input).closest("tr");
    var loai = $(dongHienTai).attr("data-loai");
    var id = $(dongHienTai).attr("data-id");
    var iIdDuAnChiPhi = "";

    var objThis = "";
    objThis = arrGoiThau.filter(x => x.iID_GoiThauID == id)[0];

    var sGiaTriQuyetToanAB = $(dongHienTai).find(".txtGiaTriQuyetToanAB").val();
    var fGiaTriQuyetToanAB = sGiaTriQuyetToanAB == "" ? 0 : parseInt(UnFormatNumber(sGiaTriQuyetToanAB));

    var sGiaTriKiemToan = $(dongHienTai).find(".txtKetQuaKiemToan").val();
    var fGiaTriKiemToan = sGiaTriKiemToan == "" ? 0 : parseInt(UnFormatNumber(sGiaTriKiemToan));

    var sGiaTriDeNghiQuyetToan = $(dongHienTai).find(".txtCDTDeNghiQuyetToan").val();
    var fGiaTriDeNghiQuyetToan = sGiaTriDeNghiQuyetToan == "" ? 0 : parseInt(UnFormatNumber(sGiaTriDeNghiQuyetToan));

    var fChenhLenhSoVoiDuToan = fGiaTriDeNghiQuyetToan - objThis.fTienPheDuyet;
    var fChenhLenhSoVoiQuyetToanAB = fGiaTriDeNghiQuyetToan - fGiaTriQuyetToanAB;
    var fChenhLechSoVoiKetQuaKiemToan = fGiaTriDeNghiQuyetToan - fGiaTriKiemToan;

    objThis.fGiaTriQuyetToanAB = fGiaTriQuyetToanAB;
    objThis.fGiaTriKiemToan = fGiaTriKiemToan;
    objThis.fGiaTriDeNghiQuyetToan = fGiaTriDeNghiQuyetToan;

    objThis.fChenhLenhSoVoiDuToan = fChenhLenhSoVoiDuToan;
    objThis.fChenhLenhSoVoiQuyetToanAB = fChenhLenhSoVoiQuyetToanAB;
    objThis.fChenhLechSoVoiKetQuaKiemToan = fChenhLechSoVoiKetQuaKiemToan;

    arrGoiThau = arrGoiThau.filter(function (x) { return x.iID_DuAn_GoiThau != objThis.iID_DuAn_GoiThau });
    arrGoiThau.push(objThis);

    var sumGiaTriDeNghiQuyetToan = 0;
    var sumGiaTriQuyetToanAB = 0;
    var sumKetQuaKiemToan = 0;
    arrGoiThau.forEach(x => {
        sumGiaTriDeNghiQuyetToan += x.fGiaTriDeNghiQuyetToan;
        sumGiaTriQuyetToanAB += x.fGiaTriQuyetToanAB;
        sumKetQuaKiemToan += x.fGiaTriKiemToan;
    });
    document.getElementById("txtGiaTriQuyetToan").value = sumGiaTriDeNghiQuyetToan;
    $('*[data-id="' + "-1" + '"]').find('.txtTongGiaTriQuyetToanAB').html(FormatNumber(sumGiaTriQuyetToanAB));
    $('*[data-id="' + "-1" + '"]').find('.txtTongKetQuaKiemToan').html(FormatNumber(sumKetQuaKiemToan));
    $('*[data-id="' + "-1" + '"]').find('.txtTongCDTDeNghiQuyetToan').html(FormatNumber(sumGiaTriDeNghiQuyetToan));

    $(dongHienTai).find(".txtChenhLechSoVoiDuToan").html(FormatNumber(fChenhLenhSoVoiDuToan));
    $(dongHienTai).find(".txtChenhLechSoVoiQuyetToanAB").html(FormatNumber(fChenhLenhSoVoiQuyetToanAB));
    $(dongHienTai).find(".txtChenhLechSoVoiKetQuaKiemToan").html(FormatNumber(fChenhLechSoVoiKetQuaKiemToan));
}

function GetDataBeforeSave() {
    var data = {};

    data.iID_DeNghiQuyetToanID = iIdDeNghiQuyetToanId;
    data.sSoBaoCao = $("#txt_SoBaoCao").val();
    data.dThoiGianLapBaoCao = $("#txtNgayBaoCao").val();
   
    data.dThoiGianKhoiCong = $("#txtThoiGianKhoiCong").val();
    data.dThoiGianHoanThanh = $("#txtThoiGianHoanThanh").val();
    data.fGiaTriDeNghiQuyetToan = $("#txtGiaTriQuyetToan").val() == "" ? null : parseFloat(UnFormatNumber($("#txtGiaTriQuyetToan").val()));

    data.iID_DuAnID = iIdDuAn;
    data.iID_DonViID = iIdDonVi;
    data.iId_LoaiQuyetToan = $("#txt_LoaiQuyetToan").val();
    data.sMoTa = $("#txtGhiChu").val();

    data.fChiPhiThietHai = $("#txtChiPhiThietHai").val() == "" ? null : parseFloat(UnFormatNumber($("#txtChiPhiThietHai").val()));
    data.fChiPhiKhongTaoNenTaiSan = $("#txtChiPhiKhongTaoNenTaiSan").val() == "" ? null : parseFloat(UnFormatNumber($("#txtChiPhiKhongTaoNenTaiSan").val()));

    data.fTaiSanDaiHanThuocCDTQuanLy = $("#txtTaiSanDaiHanThuocCDTQuanLy").val() == "" ? null : parseFloat(UnFormatNumber($("#txtTaiSanDaiHanThuocCDTQuanLy").val()));
    data.fTaiSanDaiHanDonViKhacQuanLy = $("#txtTaiSanDaiHanDvKhacQuanLy").val() == "" ? null : parseFloat(UnFormatNumber($("#txtTaiSanDaiHanDvKhacQuanLy").val()));

    data.fTaiSanNganHanThuocCDTQuanLy = $("#txtTaiSanNganHanThuocCDTQuanLay").val() == "" ? null : parseFloat(UnFormatNumber($("#txtTaiSanNganHanThuocCDTQuanLay").val()));
    data.fTaiSanNganHanDonViKhacQuanLy = $("#txtTaiSanNganHanDvKhacQuanLy").val() == "" ? null : parseFloat(UnFormatNumber($("#txtTaiSanNganHanDvKhacQuanLy").val()));

    data.iID_DuToanID = $("#iIđuToanId").val();
    data.listNguonVon = GetDataNguonVon();
    data.listChiPhi = arrChiPhi;
    data.listHangMuc = arrHangMuc;
    data.listGoiThau = arrGoiThau;
    return data;
}

function GetDataNguonVon() {
    var lstData = [];
    $("#tblDanhSachNguonVon tbody tr").each(function (index, row) {
        var iIdNguonVonId = $(row).attr("data-id");
        //var fTienToTrinh = $(row).find(".txtTienCDTQuyetToan").val() == "" ? null : parseFloat(UnFormatNumber($(row).find(".txtTienCDTQuyetToan").val()));
        lstData.push({
            //fTienToTrinh: fTienToTrinh,
            iID_NguonVonID: iIdNguonVonId
        })
    })
    return lstData;
}

function SaveData() {
    var data = GetDataBeforeSave();

    if (!ValidateBeforeSave(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/SaveData",
        data: { data: data },
        success: function (r) {
            if (r.status) {
                alert("Cập nhật thành công.");
                window.location.href = "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/Index";
            } else {
                alert("Lỗi khi lưu.");
            }
        }
    });
}

function ValidateBeforeSave(data) {
    var message = [];
    var title = 'Lỗi lưu đề nghị quyết toán';

    if (data.sSoBaoCao == null || data.sSoBaoCao == "") {
        message.push("Vui lòng nhập số báo cáo.");
    }
    if (data.dThoiGianLapBaoCao == null || data.dThoiGianLapBaoCao == "") {
        message.push("Vui lòng chọn ngày báo cáo.");
    }

    //if (message.length > 0) {
    //    popupModal(title, message, ERROR);

    //    return false;
    //}
    if (message != null && message != undefined && message.length > 0) {
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: title, Messages: message, Category: 1 },
            success: function (data) {
                $("#divModalConfirm").html(data);
            }
        });
        return false;
    }

    return true;
}

function matchStart(params, data) {
    if ($.trim(params.term) === '') {
        return data;
    }

    if (typeof data.children === 'undefined') {
        return null;
    }


    var filteredChildren = [];
    $.each(data.children, function (idx, child) {
        if (child.text.toUpperCase().indexOf(params.term.toUpperCase()) == 0) {
            filteredChildren.push(child);
        }
    });

    if (filteredChildren.length) {
        var modifiedData = $.extend({}, data, true);
        modifiedData.children = filteredChildren;

        return modifiedData;
    }

    return null;
}

function formatNumber(n) {
    return n.toFixed(2).replace(/./g, function (c, i, a) {
        return i > 0 && c !== "." && (a.length - i) % 3 === 0 ? "." + c : c;
    });
}

function popupModal(title, message, category) {
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: title, Messages: message, Category: category },
        success: function (data) {
            $("#divModalConfirm").html(data);
        }
    });
}

function changeGiaTri(input) {
    var dongHienTai = $(input).closest("tr");
    var loai = $(dongHienTai).attr("data-loai");
    var id = $(dongHienTai).attr("data-id");
    var parentId = $(dongHienTai).attr("data-parentid");
    var iIdDuAnChiPhi = "";

    var objThis = "";
    if(loai == 1)
        objThis = arrChiPhi.filter(x => x.iID_DuAn_ChiPhi == id)[0];
    else if (loai == 2)
        objThis = arrHangMuc.filter(x => x.iID_HangMucID == id)[0];

    var sGiaTriQuyetToanAB = $(dongHienTai).find(".txtGiaTriQuyetToanAB").val();
    var fGiaTriQuyetToanAB = sGiaTriQuyetToanAB == "" ? 0 : parseInt(UnFormatNumber(sGiaTriQuyetToanAB));

    var sGiaTriKiemToan = $(dongHienTai).find(".txtKetQuaKiemToan").val();
    var fGiaTriKiemToan = sGiaTriKiemToan == "" ? 0 : parseInt(UnFormatNumber(sGiaTriKiemToan));

    var sGiaTriDeNghiQuyetToan = $(dongHienTai).find(".txtCDTDeNghiQuyetToan").val();
    var fGiaTriDeNghiQuyetToan = sGiaTriDeNghiQuyetToan == "" ? 0 : parseInt(UnFormatNumber(sGiaTriDeNghiQuyetToan));

    var fChenhLenhSoVoiDuToan = fGiaTriDeNghiQuyetToan - objThis.fTienPheDuyet;
    var fChenhLenhSoVoiQuyetToanAB = fGiaTriDeNghiQuyetToan - fGiaTriQuyetToanAB;
    var fChenhLechSoVoiKetQuaKiemToan = fGiaTriDeNghiQuyetToan - fGiaTriKiemToan;

    objThis.fGiaTriQuyetToanAB = fGiaTriQuyetToanAB;
    objThis.fGiaTriKiemToan = fGiaTriKiemToan;
    objThis.fGiaTriDeNghiQuyetToan = fGiaTriDeNghiQuyetToan;

    objThis.fChenhLenhSoVoiDuToan = fChenhLenhSoVoiDuToan;
    objThis.fChenhLenhSoVoiQuyetToanAB = fChenhLenhSoVoiQuyetToanAB;
    objThis.fChenhLechSoVoiKetQuaKiemToan = fChenhLechSoVoiKetQuaKiemToan;

    if (loai == 2) {
        iIdDuAnChiPhi = $(dongHienTai).attr("data-chiphi");

        arrHangMuc = arrHangMuc.filter(function (x) { return x.iID_HangMucID != objThis.iID_HangMucID });
        arrHangMuc.push(objThis);

        CalculateDataHangMucByChiPhi(id);
        CalculateTienConLaiHangMuc(iIdDuAnChiPhi);
    }

    if (loai == 1) {
        arrChiPhi = arrChiPhi.filter(function (x) { return x.iID_DuAn_ChiPhi != objThis.iID_DuAn_ChiPhi });
        arrChiPhi.push(objThis);

        CalculateDataChiPhi(id);
    }

    var sumGiaTriDeNghiQuyetToan = 0;
    var sumGiaTriQuyetToanAB = 0;
    var sumKetQuaKiemToan = 0;
    arrChiPhi.forEach(x => {
        sumGiaTriDeNghiQuyetToan += x.fGiaTriDeNghiQuyetToan;
        sumGiaTriQuyetToanAB += x.fGiaTriQuyetToanAB;
        sumKetQuaKiemToan += x.fGiaTriKiemToan;
    });
    document.getElementById("txtGiaTriQuyetToan").value = sumGiaTriDeNghiQuyetToan;
    $('*[data-id="' + "-1" + '"]').find('.txtTongGiaTriQuyetToanAB').html(FormatNumber(sumGiaTriQuyetToanAB));
    $('*[data-id="' + "-1" + '"]').find('.txtTongKetQuaKiemToan').html(FormatNumber(sumKetQuaKiemToan));
    $('*[data-id="' + "-1" + '"]').find('.txtTongCDTDeNghiQuyetToan').html(FormatNumber(sumGiaTriDeNghiQuyetToan));

    $(dongHienTai).find(".txtChenhLechSoVoiDuToan").html(FormatNumber(fChenhLenhSoVoiDuToan));
    $(dongHienTai).find(".txtChenhLechSoVoiQuyetToanAB").html(FormatNumber(fChenhLenhSoVoiQuyetToanAB));
    $(dongHienTai).find(".txtChenhLechSoVoiKetQuaKiemToan").html(FormatNumber(fChenhLechSoVoiKetQuaKiemToan));
}

function CalculateDataChiPhi(itemId) {
    var objItem = arrChiPhi.find(x => x.iID_DuAn_ChiPhi == itemId);
    if (objItem == undefined) {
        return;
    }
    if (objItem.iID_ChiPhi_Parent != null || objItem.iID_ChiPhi_Parent != "") {
        var objParentItem = arrChiPhi.find(x => x.iID_DuAn_ChiPhi == objItem.iID_ChiPhi_Parent);
        if (objParentItem != null) {
            var arrChildSameParent = arrChiPhi.filter(function (x) { return x.iID_ChiPhi_Parent == objItem.iID_ChiPhi_Parent && x.iID_ChiPhi_Parent != "" && x.iID_ChiPhi_Parent != null });
            if (arrChildSameParent != null && arrChildSameParent.length > 0) {
                CalculateTotalParent(objParentItem, arrChildSameParent);
            }
        }

        CalculateDataChiPhi(objItem.iID_ChiPhi_Parent);
    }
}

function CalculateTotalParent(objParentItem, arrChild) {
    var parentId = objParentItem.iID_DuAn_ChiPhi;

    var sumGiaTriQuyetToanAB = 0, sumGiaTriKiemToan = 0, sumGiaTriDeNghiQuyetToan = 0;
    arrChild.forEach(x => {
        sumGiaTriQuyetToanAB += x.fGiaTriQuyetToanAB;
        sumGiaTriKiemToan += x.fGiaTriKiemToan;
        sumGiaTriDeNghiQuyetToan += x.fGiaTriDeNghiQuyetToan;
    });

    var fChenhLenhSoVoiDuToan = sumGiaTriDeNghiQuyetToan - objParentItem.fTienPheDuyet;
    var fChenhLenhSoVoiQuyetToanAB = sumGiaTriDeNghiQuyetToan - sumGiaTriQuyetToanAB;
    var fChenhLechSoVoiKetQuaKiemToan = sumGiaTriDeNghiQuyetToan - sumGiaTriKiemToan;

    $('*[data-id="' + parentId + '"]').find('.txtGiaTriQuyetToanAB').val(FormatNumber(sumGiaTriQuyetToanAB));
    $('*[data-id="' + parentId + '"]').find('.txtKetQuaKiemToan').val(FormatNumber(sumGiaTriKiemToan));
    $('*[data-id="' + parentId + '"]').find('.txtCDTDeNghiQuyetToan').val(FormatNumber(sumGiaTriDeNghiQuyetToan));

    $('*[data-id="' + parentId + '"]').find(".txtChenhLechSoVoiDuToan").html(FormatNumber(fChenhLenhSoVoiDuToan));
    $('*[data-id="' + parentId + '"]').find(".txtChenhLechSoVoiQuyetToanAB").html(FormatNumber(fChenhLenhSoVoiQuyetToanAB));
    $('*[data-id="' + parentId + '"]').find(".txtChenhLechSoVoiKetQuaKiemToan").html(FormatNumber(fChenhLechSoVoiKetQuaKiemToan));

    var parentItemNew = objParentItem;

    objParentItem.fGiaTriQuyetToanAB = sumGiaTriQuyetToanAB;
    objParentItem.fGiaTriKiemToan = sumGiaTriKiemToan;
    objParentItem.fGiaTriDeNghiQuyetToan = sumGiaTriDeNghiQuyetToan;

    objParentItem.fChenhLenhSoVoiDuToan = fChenhLenhSoVoiDuToan;
    objParentItem.fChenhLenhSoVoiQuyetToanAB = fChenhLenhSoVoiQuyetToanAB;
    objParentItem.fChenhLechSoVoiKetQuaKiemToan = fChenhLechSoVoiKetQuaKiemToan;

    arrChiPhi = arrChiPhi.filter(function (x) { return x.iID_DuAn_ChiPhi != objParentItem.iID_DuAn_ChiPhi });
    arrChiPhi.push(parentItemNew);
}

function CalculateDataHangMucByChiPhi(itemId) {
    var objItem = arrHangMuc.find(x => x.iID_HangMucID == itemId);
    if (objItem == undefined || objItem.iID_ParentID == "" || objItem.iID_ParentID == null) {
        return;
    }
    var objParentItem = arrHangMuc.find(x => x.iID_HangMucID == objItem.iID_ParentID);
    var arrChildSameParent = arrHangMuc.filter(function (x) { return x.iID_ParentID == objItem.iID_ParentID && x.iID_ParentID != "" && x.iID_ParentID != null });
    if (arrChildSameParent != null && arrChildSameParent.length > 0) {
        CalculateTotalParentHangMuc(objParentItem, arrChildSameParent);
    }

    if (objParentItem.iID_ParentID != "" && objParentItem.iID_ParentID != null) {
        CalculateDataHangMucByChiPhi(objParentItem.iID_HangMucID, arrHangMuc);
    }
}

function CalculateTotalParentHangMuc(objParentItem, arrChild) {
    var parentId = objParentItem.iID_HangMucID;

    var sumGiaTriQuyetToanAB = 0, sumGiaTriKiemToan = 0, sumGiaTriDeNghiQuyetToan = 0;
    arrChild.forEach(x => {
        sumGiaTriQuyetToanAB += x.fGiaTriQuyetToanAB;
        sumGiaTriKiemToan += x.fGiaTriKiemToan;
        sumGiaTriDeNghiQuyetToan += x.fGiaTriDeNghiQuyetToan;
    });

    var fChenhLenhSoVoiDuToan = sumGiaTriDeNghiQuyetToan - objParentItem.fTienPheDuyet;
    var fChenhLenhSoVoiQuyetToanAB = sumGiaTriDeNghiQuyetToan - sumGiaTriQuyetToanAB;
    var fChenhLechSoVoiKetQuaKiemToan = sumGiaTriDeNghiQuyetToan - sumGiaTriKiemToan;

    $('*[data-id="' + parentId + '"]').find('.txtGiaTriQuyetToanAB').val(FormatNumber(sumGiaTriQuyetToanAB));
    $('*[data-id="' + parentId + '"]').find('.txtKetQuaKiemToan').val(FormatNumber(sumGiaTriKiemToan));
    $('*[data-id="' + parentId + '"]').find('.txtCDTDeNghiQuyetToan').val(FormatNumber(sumGiaTriDeNghiQuyetToan));

    $('*[data-id="' + parentId + '"]').find(".txtChenhLechSoVoiDuToan").html(FormatNumber(fChenhLenhSoVoiDuToan));
    $('*[data-id="' + parentId + '"]').find(".txtChenhLechSoVoiQuyetToanAB").html(FormatNumber(fChenhLenhSoVoiQuyetToanAB));
    $('*[data-id="' + parentId + '"]').find(".txtChenhLechSoVoiKetQuaKiemToan").html(FormatNumber(fChenhLechSoVoiKetQuaKiemToan));

    var parentItemNew = objParentItem;

    objParentItem.fGiaTriQuyetToanAB = sumGiaTriQuyetToanAB;
    objParentItem.fGiaTriKiemToan = sumGiaTriKiemToan;
    objParentItem.fGiaTriDeNghiQuyetToan = sumGiaTriDeNghiQuyetToan;

    objParentItem.fChenhLenhSoVoiDuToan = fChenhLenhSoVoiDuToan;
    objParentItem.fChenhLenhSoVoiQuyetToanAB = fChenhLenhSoVoiQuyetToanAB;
    objParentItem.fChenhLechSoVoiKetQuaKiemToan = fChenhLechSoVoiKetQuaKiemToan;

    arrHangMuc = arrHangMuc.filter(function (x) { return x.iID_HangMucID != objParentItem.iID_HangMucID });
    arrHangMuc.push(parentItemNew);
}

function CalculateTienConLaiHangMuc(idChiPhi) {
    var sumGiaTriQuyetToanAB = 0, sumGiaTriKiemToan = 0, sumGiaTriDeNghiQuyetToan = 0;

    var arrHMParent = arrHangMuc.filter(function (x) { return (x.iID_ParentID == "" || x.iID_ParentID == null) && x.iID_DuAn_ChiPhi == idChiPhi });
    if (arrHMParent != null && arrHMParent.length > 0) {
        arrHMParent.forEach(x => {
            if (x.fTienPheDuyet != null || x.fTienPheDuyet != "") {
                sumGiaTriQuyetToanAB += x.fGiaTriQuyetToanAB;
                sumGiaTriKiemToan += x.fGiaTriKiemToan;
                sumGiaTriDeNghiQuyetToan += x.fGiaTriDeNghiQuyetToan;
            }
        });
    }

    // ipdate gia tri cho chiphi
    var objChiPhi = arrChiPhi.filter(x => x.iID_DuAn_ChiPhi == idChiPhi)[0];

    var fChenhLenhSoVoiDuToan = sumGiaTriDeNghiQuyetToan - objChiPhi.fTienPheDuyet;
    var fChenhLenhSoVoiQuyetToanAB = sumGiaTriDeNghiQuyetToan - sumGiaTriQuyetToanAB;
    var fChenhLechSoVoiKetQuaKiemToan = sumGiaTriDeNghiQuyetToan - sumGiaTriKiemToan;

    $('*[data-id="' + idChiPhi + '"]').find('.txtGiaTriQuyetToanAB').val(FormatNumber(sumGiaTriQuyetToanAB));
    $('*[data-id="' + idChiPhi + '"]').find('.txtKetQuaKiemToan').val(FormatNumber(sumGiaTriKiemToan));
    $('*[data-id="' + idChiPhi + '"]').find('.txtCDTDeNghiQuyetToan').val(FormatNumber(sumGiaTriDeNghiQuyetToan));

    $('*[data-id="' + idChiPhi + '"]').find(".txtChenhLechSoVoiDuToan").html(FormatNumber(fChenhLenhSoVoiDuToan));
    $('*[data-id="' + idChiPhi + '"]').find(".txtChenhLechSoVoiQuyetToanAB").html(FormatNumber(fChenhLenhSoVoiQuyetToanAB));
    $('*[data-id="' + idChiPhi + '"]').find(".txtChenhLechSoVoiKetQuaKiemToan").html(FormatNumber(fChenhLechSoVoiKetQuaKiemToan));

    objChiPhi.fGiaTriQuyetToanAB = sumGiaTriQuyetToanAB;
    objChiPhi.fGiaTriKiemToan = sumGiaTriKiemToan;
    objChiPhi.fGiaTriDeNghiQuyetToan = sumGiaTriDeNghiQuyetToan;

    objChiPhi.fChenhLenhSoVoiDuToan = fChenhLenhSoVoiDuToan;
    objChiPhi.fChenhLenhSoVoiQuyetToanAB = fChenhLenhSoVoiQuyetToanAB;
    objChiPhi.fChenhLechSoVoiKetQuaKiemToan = fChenhLechSoVoiKetQuaKiemToan;

    arrChiPhi = arrChiPhi.filter(x => x.iID_DuAn_ChiPhi != objChiPhi.iID_DuAn_ChiPhi);
    arrChiPhi.push(objChiPhi);

    CalculateDataChiPhi(objChiPhi.iID_DuAn_ChiPhi);
}