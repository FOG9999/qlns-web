var arrChiPhi = [];
var arrNguonVon = [];
var arrHangMuc = [];
var TBL_CHI_PHI_DAU_TU = "tblChiPhiDauTu";
var TBL_NGUON_VON_DAU_TU = "tblNguonVonDauTu";
var TBL_HANG_MUC_DAU_TU = "tblHangMucDauTu";
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var arrChiPhiBoSung = [];
var arrNguonVonBosung = [];
var arrHangMucBosung = [];

$(document).ready(function () {
    $(".setDisable").prop('disabled', true);

});

function ThemMoiChiPhiDauTu() {
    var tChiPhi = $("#txtAddCpdtChiPhi :selected").html();
    var iIdChiPhi = $("#txtAddCpdtChiPhi").val();
    var tGiaTriGoiThau = parseFloat(UnFormatNumber($("#txtAddCpdtGiatriToTrinh").val()));
    var txtLoaiDieuChinh = $("#selectDCChiPhi option:selected").text();
    var valueLoaiDieuChinh = $('#selectDCChiPhi').val();

    var tGiaTriSauDieuChinh = parseFloat(UnFormatNumber($('#txtSauDieuChinh').val() == "" ? 0 : $('#txtSauDieuChinh').val()));
    var messErr = [];
    if (tChiPhi == "") {
        messErr.push("Thông tin chi phí chưa có hoặc chưa chính xác.");
    }
    if (tGiaTriSauDieuChinh == "" || parseInt(UnFormatNumber(tGiaTriSauDieuChinh)) <= 0) {
        messErr.push("Giá trị điều chỉnh phải lơn hơn 0.");
    }

    if (messErr.length > 0) {
        alert(messErr.join("\n"));
    } else {
        if (arrChiPhi.filter(function (x) { return x.iID_ChiPhiID == iIdChiPhi }).length > 0) {
            $("#" + TBL_CHI_PHI_DAU_TU + " tbody tr").each(function (index, row) {
                if ($(row).find(".r_iID_ChiPhi").val() == iIdChiPhi) {
                    //tinh toan
                    $(row).find(".r_LoaiBoSung").html(txtLoaiDieuChinh);
                    $(row).find(".r_giaTriDieuChinh").html(FormatNumber(tGiaTriGoiThau));
                    $(row).find(".r_giaTriSauDieuChinh").html(FormatNumber(tGiaTriSauDieuChinh));

                    //them css 
                    $(row).find(".canDeleteRow").show();
                    if (valueLoaiDieuChinh < 0) {
                        $(row).find(".r_giaTriDieuChinh").addClass('dieuChinhTru');
                    } else {
                        $(row).find(".r_giaTriDieuChinh").removeClass('dieuChinhTru');
                    }
                }
            })
            arrChiPhi = arrChiPhi.filter(function (x) { return x.iID_ChiPhiID != iIdChiPhi });
        } else {
            var dongMoi = "";
            dongMoi += "<tr style='cursor: pointer;'>";
            dongMoi += "<td class='r_STT'></td>";
            dongMoi += "<td onclick='getValueRow(this)'><input type='hidden' class='r_iID_ChiPhi' value='" + iIdChiPhi + "'/>" + tChiPhi + "</td>";
            dongMoi += "<td align='right' class='r_GiaTriToTrinh'>" + (0) + "</td>";
            dongMoi += "<td class='r_LoaiBoSung'>" + (txtLoaiDieuChinh) + "</td>";
            if (valueLoaiDieuChinh == -1) {
                dongMoi += "<td align='right' class='r_giaTriDieuChinh' style='color: red'>" + (tGiaTriGoiThau == "" ? 0 : FormatNumber(tGiaTriGoiThau)) + "</td>";
            } else {
                dongMoi += "<td align='right' class='r_giaTriDieuChinh'>" + (tGiaTriGoiThau == "" ? 0 : FormatNumber(tGiaTriGoiThau)) + "</td>";
            }

            dongMoi += "<td align='right' class='r_giaTriSauDieuChinh'>" + (tGiaTriSauDieuChinh == "" ? 0 : FormatNumber(tGiaTriSauDieuChinh)) + "</td>";
            dongMoi += "<td><button class='btn btn-danger btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_CHI_PHI_DAU_TU + "\")'>" +
                "<span class='fa fa-minus fa-lg' aria-hidden='true'></span>" +
                "</button> ";
            dongMoi += "</tr>";
            $("#" + TBL_CHI_PHI_DAU_TU + " tbody").append(dongMoi);
        }

        CapNhatCotStt(TBL_CHI_PHI_DAU_TU);
        TinhLaiDongTong(TBL_CHI_PHI_DAU_TU);

        arrChiPhi.push({
            iID_ChiPhiID: iIdChiPhi,
            fTienGoiThau: parseFloat(UnFormatNumber(tGiaTriGoiThau))
        })

        // xoa text data them moi
        XoaThemMoiChiPhi();
    }
}

function CapNhatCotStt(idBang) {
    $("#" + idBang + " tbody tr").each(function (index, tr) {
        $(tr).find('.r_STT').text(index + 1);
    });
}

function TinhLaiDongTong(idBang) {
    var tongGiaTriGoiThau = 0;
    var tongDieuChinh = 0;
    var tongSauDieuChinh = 0;


    $("#" + idBang + " .r_GiaTriToTrinh").each(function () {
        tongGiaTriGoiThau += parseInt(UnFormatNumber($(this).html()));
    });
    $("#" + idBang + " .r_giaTriDieuChinh").each(function () {
        if ($(this).css('color') == 'rgb(255, 0, 0)') {
            tongDieuChinh -= parseInt(UnFormatNumber($(this).html() == "" ? 0 : $(this).html()));
        } else {
            tongDieuChinh += parseInt(UnFormatNumber($(this).html() == "" ? 0 : $(this).html()));
        }
    });

    $("#" + idBang + " .r_giaTriSauDieuChinh").each(function () {
        tongSauDieuChinh += parseInt(UnFormatNumber($(this).html() == "" ? 0 : $(this).html()));
    });

    $("#" + idBang + " .cpdt_tong_giatritotrinh").html(FormatNumber(tongGiaTriGoiThau));
    if (tongDieuChinh < 0) {
        $("#" + idBang + " .tong_dieuchinh").html(FormatNumber(tongDieuChinh * -1));
        $("#" + idBang + " .tong_dieuchinh").addClass('dieuChinhTru');
    } else {
        $("#" + idBang + " .tong_dieuchinh").html(FormatNumber(tongDieuChinh));
        $("#" + idBang + " .tong_dieuchinh").removeClass('dieuChinhTru');
    }

    $("#" + idBang + " .tong_saudieuchinh").html(FormatNumber(tongSauDieuChinh));
}

function XoaThemMoiChiPhi() {
    // xoa text data them moi
    $("#txtAddCpdtChiPhi").prop("selectedIndex", 0);
    $("#txtAddCpdtGiatriToTrinh").val("");
    $("#txtValueBefore").val("");
    $("#selectDCChiPhi").prop("selectedIndex", 0);
    $("#txtSauDieuChinh").val("");
}

function XoaDong(nutXoa, idBang) {
    var dongXoa = nutXoa.parentElement.parentElement;
    if (idBang == TBL_CHI_PHI_DAU_TU) {
        var rIIDChiPhi = $(dongXoa).find(".r_iID_ChiPhi").val();
        arrChiPhi = arrChiPhi.filter(function (x) { return x.iID_ChiPhiID != rIIDChiPhi });
    } else if (idBang == TBL_NGUON_VON_DAU_TU) {
        var rIIDNguonVon = $(dongXoa).find(".r_iID_NguonVon").val();
        arrNguonVon = arrNguonVon.filter(function (x) { return x.iID_NguonVonID != rIIDNguonVon });
    } else if (idBang == TBL_HANG_MUC_DAU_TU) {
        var rIIDHangMuc = $(dongXoa).find(".r_iID_HangMucID").val();
        arrHangMuc = arrHangMuc.filter(function (x) { return x.iID_HangMucID != rIIDHangMuc });
    }
    dongXoa.parentNode.removeChild(dongXoa);
    CapNhatCotStt(idBang);
    TinhLaiDongTong(idBang);
}


function ThemMoiNguonVonDauTu() {
    var tNguonVon = $("#txtAddNvdtNguonVon :selected").html();
    var iIdNguonVon = $("#txtAddNvdtNguonVon").val();
    var tGiaTriNguonVon = parseFloat(UnFormatNumber($("#txtGiaTriDCNguonVon").val()));
    var txtLoaiDieuChinhNV = $("#selectDCNguonVon option:selected").text();
    var valueLoaiDieuChinhNV = $('#selectDCNguonVon').val();
    /* tGiaTriNguonVon = tGiaTriNguonVon * valueLoaiDieuChinhNV;*/
    var giatriSauDCNguonVon = parseFloat(UnFormatNumber($('#txtSauDCNguonVon').val() == "" ? 0 : $('#txtSauDCNguonVon').val()));
    var messErr = [];
    if (tNguonVon == "") {
        messErr.push("Thông tin nguồn vốn chưa có hoặc chưa chính xác.");
    }
    if (giatriSauDCNguonVon == "" || parseInt(UnFormatNumber(giatriSauDCNguonVon)) <= 0) {
        messErr.push("Giá trị sau điều chỉnh của nguồn vốn phải lớn hơn 0.");
    }

    if (messErr.length > 0) {
        alert(messErr.join("\n"));
    } else {
        if (arrNguonVon.filter(function (x) { return x.iID_NguonVonID == iIdNguonVon }).length > 0) {
            $("#" + TBL_NGUON_VON_DAU_TU + " tbody tr").each(function (index, row) {
                if ($(row).find(".r_iID_NguonVon").val() == iIdNguonVon) {
                    //tinh toan
                    $(row).find(".r_LoaiBoSung").html(txtLoaiDieuChinhNV);
                    $(row).find(".r_giaTriDieuChinh").html(FormatNumber(tGiaTriNguonVon));
                    $(row).find(".r_giaTriSauDieuChinh").html(FormatNumber(giatriSauDCNguonVon));

                    //them css 
                    $(row).find(".canDeleteRow").show();

                    if (valueLoaiDieuChinhNV < 0) {
                        $(row).find(".r_giaTriDieuChinh").addClass('dieuChinhTru');
                    } else {
                        $(row).find(".r_giaTriDieuChinh").removeClass('dieuChinhTru');
                    }
                }
            })
            arrNguonVon = arrNguonVon.filter(function (x) { return x.iID_NguonVonID != iIdNguonVon });
        } else {
            var dongMoi = "";
            dongMoi += "<tr>";
            dongMoi += "<td class='r_STT'></td>";
            dongMoi += "<td onclick='getValueRowNV(this)'><input type='hidden' class='r_iID_NguonVon' value='" + iIdNguonVon + "'/>" + tNguonVon + "</td>";
            dongMoi += "<td align='right' class='r_GiaTriToTrinh'>" + (0) + "</td>";
            dongMoi += "<td class='r_LoaiBoSung'>" + (txtLoaiDieuChinhNV) + "</td>";
            dongMoi += "<td align='right' class='r_giaTriDieuChinh'>" + (tGiaTriNguonVon == "" ? 0 : FormatNumber(tGiaTriNguonVon)) + "</td>";
            dongMoi += "<td align='right' class='r_giaTriSauDieuChinh'>" + (giatriSauDCNguonVon == "" ? 0 : FormatNumber(giatriSauDCNguonVon)) + "</td>";
            dongMoi += "<td><button class='btn btn-danger btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_NGUON_VON_DAU_TU + "\")'>" +
                "<span class='fa fa-minus fa-lg' aria-hidden='true'></span>" +
                "</button> ";
            dongMoi += "</tr>";

            $("#" + TBL_NGUON_VON_DAU_TU + " tbody").append(dongMoi);
        }


        CapNhatCotStt(TBL_NGUON_VON_DAU_TU);
        TinhLaiDongTong(TBL_NGUON_VON_DAU_TU);

        arrNguonVon.push({
            iID_NguonVonID: iIdNguonVon,
            fTienGoiThau: parseFloat(UnFormatNumber(tGiaTriNguonVon))

        })

        // xoa text data them moi
        XoaThemMoiNguonVon();
    }
}

function XoaThemMoiNguonVon() {
    // xoa text data them moi
    $("#txtAddNvdtNguonVon").prop("selectedIndex", 0);
    $("#selectDCNguonVon").prop("selectedIndex", 0);
    $("#txtValueNVBefore").val("");
    $("#txtGiaTriDCNguonVon").val("");
    $("#txtSauDCNguonVon").val("");


}

function XoaThemMoiHangMuc() {
    // xoa text data them moi
    $("#txtHangMucDauTu").prop("selectedIndex", 0);
    $("#selectDCHangMuc").prop("selectedIndex", 0);
    $("#txtGiaTriDCHangMuc").val("");
    $("#txtDCHangMuc").val("");
    $("#txtSauDCHangMuc").val("");

}

function ThemMoiHangMucDauTu() {
    var tHangMuc = $("#txtHangMucDauTu :selected").html();
    var iIdHangMuc = $("#txtHangMucDauTu").val();
    var tGiaTriHangMuc = parseFloat(UnFormatNumber($("#txtDCHangMuc").val()));
    var txtLoaiDieuChinhHM = $("#selectDCHangMuc option:selected").text();
    var valueLoaiDieuChinhHM = $('#selectDCHangMuc').val();

    var giatriSauDCHangMuc = parseFloat(UnFormatNumber($('#txtSauDCHangMuc').val() == "" ? 0 : $('#txtSauDCHangMuc').val()));

    var messErr = [];
    if (tHangMuc == "") {
        messErr.push("Thông tin hạng mục chưa có hoặc chưa chính xác.");
    }
    if (giatriSauDCHangMuc == "" || parseInt(UnFormatNumber(giatriSauDCHangMuc)) <= 0) {
        messErr.push("Giá trị điều chỉnh của hạng mục phải lớn hơn 0.");
    }

    if (messErr.length > 0) {
        alert(messErr.join("\n"));
    } else {
        if (arrHangMuc.filter(function (x) { return x.iID_HangMucID == iIdHangMuc }).length > 0) {
            $("#" + TBL_HANG_MUC_DAU_TU + " tbody tr").each(function (index, row) {
                if ($(row).find(".r_iID_HangMucID").val() == iIdHangMuc) {
                    //tinh toan
                    $(row).find(".r_LoaiBoSung").html(txtLoaiDieuChinhHM);
                    $(row).find(".r_giaTriDieuChinh").html(FormatNumber(tGiaTriHangMuc));
                    $(row).find(".r_giaTriSauDieuChinh").html(FormatNumber(giatriSauDCHangMuc));

                    //them css 
                    $(row).find(".canDeleteRow").show();
                    if (valueLoaiDieuChinhHM < 0) {
                        $(row).find(".r_giaTriDieuChinh").addClass('dieuChinhTru');
                    } else {
                        $(row).find(".r_giaTriDieuChinh").removeClass('dieuChinhTru');
                    }
                }
            })
            arrHangMuc = arrHangMuc.filter(function (x) { return x.iID_HangMucID != iIdHangMuc });
        } else {
            var dongMoi = "";
            dongMoi += "<tr>";
            dongMoi += "<td class='r_STT'></td>";
            dongMoi += "<td onclick='getValueRowHM(this)'><input type='hidden' class='r_iID_HangMuc' value='" + iIdHangMuc + "'/>" + tHangMuc + "</td>";
            dongMoi += "<td align='right' class='r_GiaTriToTrinh'>" + (0) + "</td>";
            dongMoi += "<td class='r_LoaiBoSung'>" + (txtLoaiDieuChinhHM) + "</td>";
            dongMoi += "<td align='right' class='r_giaTriDieuChinh'>" + (tGiaTriHangMuc == "" ? 0 : FormatNumber(tGiaTriHangMuc)) + "</td>";
            dongMoi += "<td align='right' class='r_giaTriSauDieuChinh'>" + (giatriSauDCHangMuc == "" ? 0 : FormatNumber(giatriSauDCHangMuc)) + "</td>";
            dongMoi += "<td><button class='btn btn-danger btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_HANG_MUC_DAU_TU + "\")'>" +
                "<span class='fa fa-minus fa-lg' aria-hidden='true'></span>" +
                "</button> ";
            dongMoi += "</tr>";

            $("#" + TBL_HANG_MUC_DAU_TU + " tbody").append(dongMoi);
        }


        CapNhatCotStt(TBL_HANG_MUC_DAU_TU);
        TinhLaiDongTong(TBL_HANG_MUC_DAU_TU);

        arrHangMuc.push({
            iID_HangMucID: iIdHangMuc,
            fTienGoiThau: parseFloat(UnFormatNumber(tGiaTriHangMuc))

        })

        // xoa text data them moi
        XoaThemMoiHangMuc();
    }
}

function updateArrayChiPhiBoSung() {
    arrChiPhiBoSung = [];
    $("#tblChiPhiDauTu tbody tr").each(function (index, row) {
        var idChiPhi = $(row).find(".r_iID_ChiPhi").val();
        var soTien = parseFloat(UnFormatNumber($(row).find(".r_giaTriDieuChinh").html() == "" ? 0 : $(row).find(".r_giaTriDieuChinh").html()));
        if ($(row).find(".r_giaTriDieuChinh").css('color') == 'rgb(255, 0, 0)') {
            soTien = soTien * (-1);
        }

        arrChiPhiBoSung.push({
            iID_ChiPhiID: idChiPhi,
            fTienGoiThau: soTien
        })

    });
}
function updateArrayNguonVonBosung() {
    arrNguonVonBosung = [];
    $("#tblNguonVonDauTu tbody tr").each(function (index, row) {
        var idNguonVon = $(row).find(".r_iID_NguonVon").val();
        var soTien = parseFloat(UnFormatNumber($(row).find(".r_giaTriDieuChinh").html() == "" ? 0 : $(row).find(".r_giaTriDieuChinh").html()));
        if ($(row).find(".r_giaTriDieuChinh").css('color') == 'rgb(255, 0, 0)') {
            soTien = soTien * (-1);
        }

        arrNguonVonBosung.push({
            iID_NguonVonID: idNguonVon,
            fTienGoiThau: soTien
        })


    });
}
function updateArrayHangMucBosung() {
    arrHangMucBosung = [];
    $("#tblHangMucDauTu tbody tr").each(function (index, row) {
        var idHangMuc = $(row).find(".r_iID_HangMucID").val();
        var soTien = parseFloat(UnFormatNumber($(row).find(".r_giaTriDieuChinh").html() == "" ? 0 : $(row).find(".r_giaTriDieuChinh").html()));
        if ($(row).find(".r_giaTriDieuChinh").css('color') == 'rgb(255, 0, 0)') {
            soTien = soTien * (-1);
        }

        arrHangMucBosung.push({
            iID_HangMucID: idHangMuc,
            fTienGoiThau: soTien
        })


    });

}

function Luu() {
    updateArrayChiPhiBoSung();
    updateArrayNguonVonBosung();
    updateArrayHangMucBosung();
    var goiThau = {};
    var iID_GoiThauID = $("#iID_GoiThauID").val();
    goiThau.iID_GoiThauGocID = $("#iID_GoiThauGocID").val();
    goiThau.iID_DuAnID = $("#iID_DuAnID").val();
    goiThau.iID_NhaThauID = $("#iID_NhaThauID").val();
    goiThau.sTenGoiThau = $("#sTenGoiThau").val();
    goiThau.sPhuongThucDauThau = $("#sPhuongThucDauThau").val();
    goiThau.iThoiGianThucHien = $("#iThoiGianThucHien").val();
    goiThau.fTienTrungThau = parseFloat(UnFormatNumber($("#tblChiPhiDauTu .tong_dieuchinh").html() == "" ? 0 : $("#tblChiPhiDauTu .tong_dieuchinh").html()));
    goiThau.dNgayLap = $("#dNgayLapMoi").val();

    var doiTuong = { goiThau: goiThau, listChiPhi: arrChiPhiBoSung, listNguonVon: arrNguonVonBosung, listHangMuc: arrHangMucBosung };
    if (checkLoi()) {
        $.ajax({
            url: "/QLVonDauTu/QLThongTinGoiThau/AddGoiThauDieuChinh",
            type: "POST",
            data: { model: doiTuong, iID_GoiThauID: iID_GoiThauID },
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data.status == true) {
                    window.location.href = "/QLVonDauTu/QLThongTinGoiThau/Index";
                }
            },
            error: function (data) {

            }
        })
    }

}

function checkLoi() {
    var tongChiPhi = parseFloat(UnFormatNumber($('#tblChiPhiDauTu .tong_saudieuchinh').html() == "" ? 0 : $('#tblChiPhiDauTu .tong_saudieuchinh').html()));
    var tongNguonVon = parseFloat(UnFormatNumber($('#tblNguonVonDauTu .tong_saudieuchinh').html() == "" ? 0 : $('#tblNguonVonDauTu .tong_saudieuchinh').html()));
    var txtTenGoiThau = $('#sTenGoiThau').val();
    var txtNgayLap = $("#dNgayLapMoi").val();
    /*var tongChiPhi = $()*/

    var messErr = [];
    if (txtTenGoiThau == "") {
        messErr.push("Chưa nhập tên gói thầu.");
    }
    if (txtNgayLap == "") {
        messErr.push("Chưa nhập ngày lâp.");
    }
    if (tongChiPhi != tongNguonVon) {
        messErr.push("Nguồn vốn phải bằng chi phí.");
    }
    if (messErr.length > 0) {
        alert(messErr.join("\n"));
        return false;
    } else {
        return true;
    }

}



function Huy() {
    window.location.href = "/QLVonDauTu/QLThongTinGoiThau/Index";
}

function showListTables() {
    var dateOld = convertDate($('#txtNgayLapCu').val());
    var dateNew = convertDate($('#dNgayLapMoi').val());
    if (dateNew >= dateOld) {
        $('.canAddRow').show();
        $(".setDisable").prop('disabled', false);
    } else {
        $('.canAddRow').hide();
        $(".setDisable").prop('disabled', true);
    }
}

function convertDate(date) {
    var parts = date.split("/");
    return new Date(parts[2], parts[1] - 1, parts[0]);
}

function getValueRow(r) {
    var getSotien = $(r).parent('tr').find(".r_GiaTriToTrinh").html();
    var getSotienDieuChinh = $(r).parent('tr').find(".r_giaTriDieuChinh").html();
    var getSotienSauDC = $(r).parent('tr').find(".r_giaTriSauDieuChinh").html();
    var getId = $(r).parent('tr').find(".r_iID_ChiPhi").val();
    $('#txtAddCpdtChiPhi').val(getId);
    $('#txtValueBefore').val(getSotien);
    $('#txtAddCpdtGiatriToTrinh').val(getSotienDieuChinh);
    $('#txtSauDieuChinh').val(getSotienSauDC);

}

function getValueRowNV(r) {

    var getSotienNV = $(r).parent('tr').find(".r_GiaTriToTrinh").html();
    var getSotienDieuChinh = $(r).parent('tr').find(".r_giaTriDieuChinh").html();
    var getSotienSauDC = $(r).parent('tr').find(".r_giaTriSauDieuChinh").html();
    var getIdNV = $(r).parent('tr').find(".r_iID_NguonVon").val();
    $('#txtAddNvdtNguonVon').val(getIdNV);
    $('#txtValueNVBefore').val(getSotienNV);
    $('#txtGiaTriDCNguonVon').val(getSotienDieuChinh);
    $('#txtSauDCNguonVon').val(getSotienSauDC);

}

function getValueRowHM(r) {
    var getSotienHM = $(r).parent('tr').find(".r_GiaTriToTrinh").html();
    var getSotienDieuChinh = $(r).parent('tr').find(".r_giaTriDieuChinh").html();
    var getSotienSauDC = $(r).parent('tr').find(".r_giaTriSauDieuChinh").html();
    var getIdHM = $(r).parent('tr').find(".r_iID_HangMucID").val();
    $('#txtHangMucDauTu').val(getIdHM);
    $('#txtGiaTriDCHangMuc').val(getSotienHM);
    $('#txtDCHangMuc').val(getSotienDieuChinh);
    $('#txtSauDCHangMuc').val(getSotienSauDC);

}

function tinhGiaTriSauDieuChinh() {
    var giaTriTruoc = parseInt(UnFormatNumber($('#txtValueBefore').val() == "" ? 0 : $('#txtValueBefore').val()));
    var giaDieuChinh = parseInt(UnFormatNumber($('#txtAddCpdtGiatriToTrinh').val()));
    var loaiDieuChinh = $('#selectDCChiPhi').val();
    giaDieuChinh = giaDieuChinh * loaiDieuChinh;

    $('#txtSauDieuChinh').val(FormatNumber(giaTriTruoc + giaDieuChinh));
}

function tinhGiaTriSauDieuChinhNguonVon() {
    var giaTriTruocNV = parseInt(UnFormatNumber($('#txtValueNVBefore').val() == "" ? 0 : $('#txtValueNVBefore').val()));
    var giaDieuChinhNV = parseInt(UnFormatNumber($('#txtGiaTriDCNguonVon').val()));
    var loaiDieuChinhNV = $('#selectDCNguonVon').val();
    giaDieuChinhNV = giaDieuChinhNV * loaiDieuChinhNV;

    $('#txtSauDCNguonVon').val(FormatNumber(giaTriTruocNV + giaDieuChinhNV));
}
function tinhGiaTriSauDieuChinhHangMuc() {
    var giaTriTruocHM = parseInt(UnFormatNumber($('#txtGiaTriDCHangMuc').val() == "" ? 0 : $('#txtGiaTriDCHangMuc').val()));
    var giaDieuChinhHM = parseInt(UnFormatNumber($('#txtDCHangMuc').val()));
    var loaiDieuChinhHM = $('#selectDCHangMuc').val();
    giaDieuChinhHM = giaDieuChinhHM * loaiDieuChinhHM;

    $('#txtSauDCHangMuc').val(FormatNumber(giaTriTruocHM + giaDieuChinhHM));
}

$("#txtAddCpdtChiPhi").change(function () {
    var tChiPhi = $("#txtAddCpdtChiPhi").val();
    var cp = arrChiPhi.filter(function (x) { return x.iID_ChiPhiID == tChiPhi });
    var vTruocDieuChinh = 0, vGiaTriDieuChinh = 0, vGiaTriSauDieuChinh = 0;
    if (cp.length > 0) {
        // fill len o chinh sua
        vTruocDieuChinh = FormatNumber(UnFormatNumber(cp[0].fTienGoiThau)) == "" ? 0 : FormatNumber(UnFormatNumber(cp[0].fTienGoiThau));
    }

    $("#txtValueBefore").val(vTruocDieuChinh);
    $("#txtSauDieuChinh").val(vTruocDieuChinh);
})

$("#txtAddNvdtNguonVon").change(function () {
    var tNguonVon = $("#txtAddNvdtNguonVon").val();
    var nv = arrNguonVon.filter(function (x) { return x.iID_NguonVonID == tNguonVon });
    var vTruocDieuChinh = 0, vGiaTriDieuChinh = 0, vGiaTriSauDieuChinh = 0;

    if (nv.length > 0) {
        // fill len o chinh sua
        vTruocDieuChinh = FormatNumber(UnFormatNumber(nv[0].fTienGoiThau)) == "" ? 0 : FormatNumber(UnFormatNumber(nv[0].fTienGoiThau));
    }

    $("#txtValueNVBefore").val(vTruocDieuChinh);
    $("#txtSauDCNguonVon").val(vTruocDieuChinh);
})

$("#txtHangMucDauTu").change(function () {
    var tHangMuc = $("#txtHangMucDauTu").val();
    var hm = arrHangMuc.filter(function (x) { return x.iID_HangMucID == tHangMuc });
    var vTruocDieuChinh = 0, vGiaTriDieuChinh = 0, vGiaTriSauDieuChinh = 0;

    if (hm.length > 0) {
        // fill len o chinh sua
        vTruocDieuChinh = FormatNumber(UnFormatNumber(hm[0].fTienGoiThau)) == "" ? 0 : FormatNumber(UnFormatNumber(hm[0].fTienGoiThau));
    }

    $("#txtGiaTriDCHangMuc").val(vTruocDieuChinh);
    $("#txtSauDCHangMuc").val(vTruocDieuChinh);
})

function VeDanhSachChiPhi() {

    var dNgayLap = $('#dNgayLapMoi').val();
    if (dNgayLap != "") {
        $(".setDisable").prop('disabled', false);
        $.ajax({
            url: "/QLVonDauTu/QLThongTinGoiThau/LayThongTinChiPhiDieuChinh",
            type: "POST",
            data: { iID_GoiThauGoc: $("#iID_GoiThauGocID").val(), dNgayLap: dNgayLap },
            dataType: "json",
            cache: false,
            async: false,
            success: function (data) {
                if (data != null && data.lstCp.length > 0) {
                    var htmlChiPhi = "";
                    arrChiPhi = data.lstCp;
                    data.lstCp.forEach(function (cp) {
                        var dongMoi = "";
                        dongMoi += "<tr style='cursor: pointer;'>";
                        dongMoi += "<td class='r_STT'></td>";
                        dongMoi += "<td onclick='getValueRow(this)'><input type='hidden' class='r_iID_ChiPhi' value='" + cp.iID_ChiPhiID + "'/>" + cp.sTenChiPhi + "</td>";
                        dongMoi += "<td align='right' class='r_GiaTriToTrinh sotien'>" + ((cp.fTienGoiThau == "" || cp.fTienGoiThau == null) ? 0 : FormatNumber(cp.fTienGoiThau)) + "</td>";
                        dongMoi += "<td align='center' class='r_LoaiBoSung'></td>";
                        dongMoi += "<td align='right' class='r_giaTriDieuChinh sotien'>0</td>";
                        dongMoi += "<td align='right' class='r_giaTriSauDieuChinh sotien'>" + ((cp.fTienGoiThau == "" || cp.fTienGoiThau == null) ? 0 : FormatNumber(cp.fTienGoiThau)) + "</td>";
                        dongMoi += "<td></td>";
                        dongMoi += "</tr>";
                        htmlChiPhi += dongMoi;
                    })

                    $("#" + TBL_CHI_PHI_DAU_TU + " tbody").html(htmlChiPhi);
                    
                } else {
                    $("#" + TBL_CHI_PHI_DAU_TU + " tbody").html("");
                }
                CapNhatCotStt(TBL_CHI_PHI_DAU_TU);
                TinhLaiDongTong(TBL_CHI_PHI_DAU_TU);

                if (data != null && data.lstNguonVon.length > 0) {
                    var htmlNguonVon = "";
                    arrNguonVon = data.lstNguonVon;
                    data.lstNguonVon.forEach(function (nv) {
                        var dongMoi = "";
                        dongMoi += "<tr style='cursor: pointer;'>";
                        dongMoi += "<td class='r_STT'></td>";
                        dongMoi += "<td onclick='getValueRowNV(this)'><input type='hidden' class='r_iID_NguonVon' value='" + nv.iID_NguonVonID + "'/>" + nv.sTenNguonVon + "</td>";
                        dongMoi += "<td align='right' class='r_GiaTriToTrinh sotien'>" + ((nv.fTienGoiThau == "" || nv.fTienGoiThau == null) ? 0 : FormatNumber(nv.fTienGoiThau)) + "</td>";
                        dongMoi += "<td align='center' class='r_LoaiBoSung'></td>";
                        dongMoi += "<td align='right' class='r_giaTriDieuChinh sotien'>0</td>";
                        dongMoi += "<td align='right' class='r_giaTriSauDieuChinh sotien'>" + ((nv.fTienGoiThau == "" || nv.fTienGoiThau == null) ? 0 : FormatNumber(nv.fTienGoiThau)) + "</td>";
                        dongMoi += "<td></td>";
                        dongMoi += "</tr>";
                        htmlNguonVon += dongMoi;
                    })

                    $("#" + TBL_NGUON_VON_DAU_TU + " tbody").html(htmlNguonVon);
                   
                } else {
                    $("#" + TBL_NGUON_VON_DAU_TU + " tbody").html("");
                }
                CapNhatCotStt(TBL_NGUON_VON_DAU_TU);
                TinhLaiDongTong(TBL_NGUON_VON_DAU_TU);

                if (data != null && data.lstHangMuc.length > 0) {
                    var htmlHangMuc = "";
                    arrHangMuc = data.lstHangMuc;
                    data.lstHangMuc.forEach(function (nv) {
                        var dongMoi = "";
                        dongMoi += "<tr style='cursor: pointer;'>";
                        dongMoi += "<td class='r_STT'></td>";
                        dongMoi += "<td onclick='getValueRowHM(this)'><input type='hidden' class='r_iID_HangMucID' value='" + nv.iID_HangMucID + "'/>" + nv.sTenHangMuc + "</td>";
                        dongMoi += "<td align='right' class='r_GiaTriToTrinh sotien'>" + ((nv.fTienGoiThau == "" || nv.fTienGoiThau == null) ? 0 : FormatNumber(nv.fTienGoiThau)) + "</td>";
                        dongMoi += "<td align='center' class='r_LoaiBoSung'></td>";
                        dongMoi += "<td align='right' class='r_giaTriDieuChinh sotien'>0</td>";
                        dongMoi += "<td align='right' class='r_giaTriSauDieuChinh sotien'>" + ((nv.fTienGoiThau == "" || nv.fTienGoiThau == null) ? 0 : FormatNumber(nv.fTienGoiThau)) + "</td>";
                        dongMoi += "<td></td>";
                        dongMoi += "</tr>";
                        htmlHangMuc += dongMoi;
                    })

                    $("#" + TBL_HANG_MUC_DAU_TU + " tbody").html(htmlHangMuc);
                   
                } else {
                    $("#" + TBL_HANG_MUC_DAU_TU + " tbody").html("");
                }
                CapNhatCotStt(TBL_HANG_MUC_DAU_TU);
                TinhLaiDongTong(TBL_HANG_MUC_DAU_TU);

            },
            error: function (data) {

            }
        })

    } else {
        $(".setDisable").prop('disabled', true);
    }
    
}

