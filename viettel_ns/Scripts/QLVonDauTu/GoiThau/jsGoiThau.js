var arrChiPhi = [];
var arrNguonVon = [];
var arrHangMuc = [];
var TBL_CHI_PHI_DAU_TU = "tblChiPhiDauTu";
var TBL_NGUON_VON_DAU_TU = "tblNguonVonDauTu";
var TBL_HANG_MUC_DAU_TU = "tblHangMucDauTu";
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';

$(document).ready(function () {
    $("#iID_DuAnID").change(function () {
        var txtNgayLap = $('#dNgayLap').val();
        if (this.value != "" && this.value != GUID_EMPTY && txtNgayLap != "") {
            $("#sMaDuAn").val($(this).find(":selected").data("smaduan"));

            // lay thong tin chi tiet
            $.ajax({
                url: "/QLVonDauTu/QLThongTinGoiThau/LayDataTheoDuAn",
                type: "POST",
                data: { iID_DuAnID: this.value, dNgayLap: txtNgayLap },
                dataType: "json",
                cache: false,
                success: function (data) {

                    $("#txtAddCpdtChiPhi").html(data.listChiPhi);
                    $("#txtAddNvdtNguonVon").html(data.listNguonVon);
                    $("#txtHangMucDauTu").html(data.listHangMuc);

                    if (data.duAn != null) {
                        $("#sDiaDiem").html(data.duAn.sDiaDiem);
                        $("#sKhoiCong").html(data.duAn.sKhoiCong);
                        $("#sKetThuc").html(data.duAn.sKetThuc);
                        $("#tongMucDauTu").html(FormatNumber(data.duAn.fTongMucDauTu));
                    }

                },
                error: function (data) {

                }
            })
        } else {
            $("#sMaDuAn").val("");
        }
    })

    $("#dNgayLap").change(function () {
        $("#iID_DuAnID").trigger("change");
    });

    $("#iID_DonViQuanLyID").change(function () {
        if (this.value != "" && this.value != GUID_EMPTY) {
            $.ajax({
                url: "/QLVonDauTu/QLThongTinGoiThau/ListDuAnTheoDonViQuanLy",
                type: "POST",
                data: { iID_DonViQuanLyID: this.value },
                dataType: "json",
                cache: false,
                success: function (data) {
                    //if (data != null && data != "") {
                    //    $("#iID_DuAnID").html(data);
                    //}
                    $("#iID_DuAnID").html(data);
                },
                error: function (data) {

                }
            })
        } else {
            $("#iID_DuAnID option:not(:first)").remove();
            $("#iID_DuAnID").trigger("change");
        }
    });

    $(".required").change(function () {
        var soLuongRequireDaNhap = $('.required').filter(function () {
            return (this.value != '' && this.value != GUID_EMPTY && this.value != undefined);
        });
        if (soLuongRequireDaNhap.length == $(".required").length)
            $("#btnLuu").removeAttr("disabled");
        else
            $("#btnLuu").attr("disabled", true);
    })
});

function ThemMoiChiPhiDauTu() {
    var tChiPhi = $("#txtAddCpdtChiPhi :selected").html();
    var iIdChiPhi = $("#txtAddCpdtChiPhi").val();
    var tGiaTriGoiThau = $("#txtAddCpdtGiatriToTrinh").val();

    var messErr = [];
    if (tChiPhi == "" || tChiPhi == "--Chọn--") {
        messErr.push("Thông tin chi phí chưa có hoặc chưa chính xác.");
    }
    if (tGiaTriGoiThau == "" || parseInt(UnFormatNumber(tGiaTriGoiThau) <= 0)) {
        messErr.push("Giá trị gói thầu của chi phí phải lớn hơn 0.");
    }

    if (messErr.length > 0) {
        alert(messErr.join("\n"));
    } else {
        if (arrChiPhi.filter(function (x) { return x.iID_ChiPhiID == iIdChiPhi }).length > 0) {
            $("#" + TBL_CHI_PHI_DAU_TU + " tbody tr").each(function (index, row) {
                if ($(row).find(".r_iID_ChiPhi").val() == iIdChiPhi) {
                    $(row).remove();
                }
            })
            arrChiPhi = arrChiPhi.filter(function (x) { return x.iID_ChiPhiID != iIdChiPhi });
        }

        var dongMoi = "";
        dongMoi += "<tr style='cursor: pointer;'>";
        dongMoi += "<td class='r_STT'></td>";
        dongMoi += "<td><input type='hidden' class='r_iID_ChiPhi' value='" + iIdChiPhi + "'/>" + tChiPhi + "</td>";
        dongMoi += "<td align='right' class='r_GiaTriToTrinh'>" + (tGiaTriGoiThau == "" ? 0 : tGiaTriGoiThau) + "</td>";
        dongMoi += "<td><button class='btn btn-danger btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_CHI_PHI_DAU_TU + "\")'>" +
            "<span class='fa fa-minus fa-lg' aria-hidden='true'></span>" +
            "</button> ";
        dongMoi += "</tr>";

        $("#" + TBL_CHI_PHI_DAU_TU + " tbody").append(dongMoi);
        CapNhatCotStt(TBL_CHI_PHI_DAU_TU);
        TinhLaiDongTong(TBL_CHI_PHI_DAU_TU);

        arrChiPhi.push({
            iID_ChiPhiID: iIdChiPhi,
            fTienGoiThau: parseFloat(UnFormatNumber(tGiaTriGoiThau))
        })

        var tongChiPhi = $('#tblChiPhiDauTu .cpdt_tong_giatritotrinh').html();
        $('#fTienTrungThau').val(tongChiPhi);
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

    $("#" + idBang + " .r_GiaTriToTrinh").each(function () {
        tongGiaTriGoiThau += parseInt(UnFormatNumber($(this).html()));
    });

    $("#" + idBang + " .cpdt_tong_giatritotrinh").html(FormatNumber(tongGiaTriGoiThau));


    //update gia tri readonly
    if (idBang == TBL_CHI_PHI_DAU_TU) {
        $("#fTienTrungThau").val(FormatNumber(tongGiaTriGoiThau));

    }
}

function XoaThemMoiChiPhi() {
    // xoa text data them moi
    $("#txtAddCpdtChiPhi").prop("selectedIndex", 0);
    $("#txtAddCpdtGiatriToTrinh").val("");
    $("#txtTongMucDauTuChiPhi").html("");

}

function XoaDong(nutXoa, idBang) {
    var dongXoa = nutXoa.parentElement.parentElement;
    if (idBang == TBL_CHI_PHI_DAU_TU) {
        var rIIDChiPhi = $(dongXoa).find(".r_iID_ChiPhi").val();
        arrChiPhi = arrChiPhi.filter(function (x) { return x.iID_ChiPhiID != rIIDChiPhi });
    } else if (idBang == TBL_NGUON_VON_DAU_TU) {
        var rIIDNguonVon = $(dongXoa).find(".r_iID_NguonVon").val();
        arrNguonVon = arrNguonVon.filter(function (x) { return x.iID_NguonVonID != rIIDNguonVon });
    } else if ((idBang == TBL_HANG_MUC_DAU_TU)) {
        var rIIDHangMuc = $(dongXoa).find(".r_iID_NguonVon").val();
        arrHangMuc = arrHangMuc.filter(function (x) { return x.iID_HangMucID != rIIDHangMuc });
    }
    dongXoa.parentNode.removeChild(dongXoa);
    CapNhatCotStt(idBang);
    TinhLaiDongTong(idBang);

}


function ThemMoiNguonVonDauTu() {
    var tNguonVon = $("#txtAddNvdtNguonVon :selected").html();
    var iIdNguonVon = $("#txtAddNvdtNguonVon").val();
    var tGiaTriNguonVon = $("#txtGiaTriNguonVon").val();

    var messErr = [];
    if (tNguonVon == "" || tNguonVon == "--Chọn--") {
        messErr.push("Thông tin nguồn vốn chưa có hoặc chưa chính xác.");
    }
    if (tGiaTriNguonVon == "" || parseInt(UnFormatNumber(tGiaTriNguonVon) <= 0)) {
        messErr.push("Giá trị của nguồn vốn phải lớn hơn 0.");
    }

    if (messErr.length > 0) {
        alert(messErr.join("\n"));
    } else {
        if (arrNguonVon.filter(function (x) { return x.iID_NguonVonID == iIdNguonVon }).length > 0) {
            $("#" + TBL_NGUON_VON_DAU_TU + " tbody tr").each(function (index, row) {
                if ($(row).find(".r_iID_NguonVon").val() == iIdNguonVon) {
                    $(row).remove();
                }
            })
            arrNguonVon = arrNguonVon.filter(function (x) { return x.iID_NguonVonID != iIdNguonVon });
        }

        var dongMoi = "";
        dongMoi += "<tr>";
        dongMoi += "<td class='r_STT'></td>";
        dongMoi += "<td><input type='hidden' class='r_iID_NguonVon' value='" + iIdNguonVon + "'/>" + tNguonVon + "</td>";
        dongMoi += "<td align='right' class='r_GiaTriToTrinh'>" + (tGiaTriNguonVon == "" ? 0 : tGiaTriNguonVon) + "</td>";
        dongMoi += "<td><button class='btn btn-danger btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_NGUON_VON_DAU_TU + "\")'>" +
            "<span class='fa fa-minus fa-lg' aria-hidden='true'></span>" +
            "</button> ";
        dongMoi += "</tr>";

        $("#" + TBL_NGUON_VON_DAU_TU + " tbody").append(dongMoi);
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
    $("#txtGiaTriNguonVon").val("");
    $("#txtTongMucDauTuNV").html("");

}

function XoaThemMoiHangMuc() {
    // xoa text data them moi
    $("#txtHangMucDauTu").prop("selectedIndex", 0);
    $("#txtGiaTriHangMuc").val("");
    $("#txtTongMucDauTuHM").html("");

}

function ThemMoiHangMucDauTu() {
    var tHangMuc = $("#txtHangMucDauTu :selected").html();
    var iIdHangMuc = $("#txtHangMucDauTu").val();
    var tGiaTriHangMuc = $("#txtGiaTriHangMuc").val();

    var messErr = [];
    if (tHangMuc == "" || tHangMuc == "--Chọn--") {
        messErr.push("Thông tin hạng mục chưa có hoặc chưa chính xác.");
    }
    if (tGiaTriHangMuc == "" || parseInt(UnFormatNumber(tGiaTriHangMuc) <= 0)) {
        messErr.push("Giá trị của hạng mục phải lớn hơn 0.");
    }

    if (messErr.length > 0) {
        alert(messErr.join("\n"));
    } else {
        if (arrHangMuc.filter(function (x) { return x.iID_HangMucID == iIdHangMuc }).length > 0) {
            $("#" + TBL_NGUON_VON_DAU_TU + " tbody tr").each(function (index, row) {
                if ($(row).find(".r_iID_NguonVon").val() == iIdHangMuc) {
                    $(row).remove();
                }
            })
            arrHangMuc = arrHangMuc.filter(function (x) { return x.iID_HangMucID != iIdHangMuc });
        }

        var dongMoi = "";
        dongMoi += "<tr>";
        dongMoi += "<td class='r_STT'></td>";
        dongMoi += "<td><input type='hidden' class='r_iID_NguonVon' value='" + iIdHangMuc + "'/>" + tHangMuc + "</td>";
        dongMoi += "<td align='right' class='r_GiaTriToTrinh'>" + (tGiaTriHangMuc == "" ? 0 : tGiaTriHangMuc) + "</td>";
        dongMoi += "<td><button class='btn btn-danger btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_HANG_MUC_DAU_TU + "\")'>" +
            "<span class='fa fa-minus fa-lg' aria-hidden='true'></span>" +
            "</button> ";
        dongMoi += "</tr>";

        $("#" + TBL_HANG_MUC_DAU_TU + " tbody").append(dongMoi);
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


function Luu() {
    var goiThau = {};

    goiThau.iID_DuAnID = $("#iID_DuAnID").val();
    goiThau.sTenGoiThau = $("#sTenGoiThau").val();
    goiThau.sHinhThucChonNhaThau = $("#sHinhThucChonNhaThau").val();
    goiThau.sPhuongThucDauThau = $("#sPhuongThucDauThau").val();
    goiThau.dBatDauChonNhaThau = $("#dBatDauChonNhaThau").val();
    goiThau.dKetThucChonNhaThau = $("#dKetThucChonNhaThau").val();
    goiThau.sHinhThucHopDong = $("#sHinhThucHopDong").val();
    goiThau.iThoiGianThucHien = $("#iThoiGianThucHien").val();
    goiThau.dNgayLap = $("#dNgayLap").val();
    goiThau.iID_NhaThauID = $("#iID_NhaThauID").val();
    goiThau.fTienTrungThau = parseFloat(UnFormatNumber($('#fTienTrungThau').val() == "" ? 0 : $('#fTienTrungThau').val()));


    var doiTuong = { goiThau: goiThau, listChiPhi: arrChiPhi, listNguonVon: arrNguonVon, listHangMuc: arrHangMuc };
    if (checkLoi()) {
        $.ajax({
            url: "/QLVonDauTu/QLThongTinGoiThau/Save",
            type: "POST",
            data: { model: doiTuong },
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
    var tongChiPhi = parseFloat(UnFormatNumber($('#tblChiPhiDauTu .cpdt_tong_giatritotrinh').html() == "" ? 0 : $('#tblChiPhiDauTu .cpdt_tong_giatritotrinh').html()));
    var tongNguonVon = parseFloat(UnFormatNumber($('#tblNguonVonDauTu .cpdt_tong_giatritotrinh').html() == "" ? 0 : $('#tblNguonVonDauTu .cpdt_tong_giatritotrinh').html()));

    var messErr = [];
    if (arrChiPhi.length < 1) {
        messErr.push("Chưa có hạng mục chi phí cho gói thầu.");
    }
    if (arrNguonVon.length < 1) {
        messErr.push("Chưa có nguồn vốn cho gói thầu.");
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

function getTongMucDauTu() {
    var chiPhiId = $('#txtAddCpdtChiPhi').val();
    var duAnId = $("#iID_DuAnID").val();
    var ngayLap = $("#dNgayLap").val();

    $.ajax({
        url: "/QLVonDauTu/QLThongTinGoiThau/GetTongMucDauTuChiPhi",
        type: "POST",
        data: { iID: chiPhiId, iID_DuAnID: duAnId, dNgayLap: ngayLap },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data.tongMucDauTuChiPhi != null) {
                $('#txtTongMucDauTuChiPhi').html(FormatNumber(data.tongMucDauTuChiPhi));
            }
        },
        error: function (data) {

        }
    })
}

function getTongMucDauTuNV() {
    var nguonVonId = $('#txtAddNvdtNguonVon').val();
    var duAnId = $("#iID_DuAnID").val();
    var ngayLap = $("#dNgayLap").val();

    $.ajax({
        url: "/QLVonDauTu/QLThongTinGoiThau/GetTongMucDauTuNV",
        type: "POST",
        data: { iID: nguonVonId, iID_DuAnID: duAnId, dNgayLap: ngayLap },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data.tongMucDauTuNV != null) {
                $('#txtTongMucDauTuNV').html(FormatNumber(data.tongMucDauTuNV));
            }
        },
        error: function (data) {

        }
    })
}

function getTongMucDauTuHM() {
    var hangMucId = $('#txtHangMucDauTu').val();
    var duAnId = $("#iID_DuAnID").val();
    var ngayLap = $("#dNgayLap").val();

    $.ajax({
        url: "/QLVonDauTu/QLThongTinGoiThau/GetTongMucDauTuHM",
        type: "POST",
        data: { iID: hangMucId, iID_DuAnID: duAnId, dNgayLap: ngayLap },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data.tongMucDauTuHM != null) {
                $('#txtTongMucDauTuHM').html(FormatNumber(data.tongMucDauTuHM));
            }
        },
        error: function (data) {

        }
    })
}

