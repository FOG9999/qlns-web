var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var maTienTe = ["USD", "VND", "EUR"];

$(document).ready(function () {
    $("#txtHopDongVND").keydown(function (event) {
        ValidateInputKeydown(event, this, 1);
    }).blur(function (event) {
        if (ValidateInputFocusOut(event, this, 2)) {
            ChangeGiaTien(this);
        }
    });

    $(".txtHopDong").keydown(function (event) {
        ValidateInputKeydown(event, this, 2);
    }).blur(function (event) {
        if (ValidateInputFocusOut(event, this, 2, 2)) {
            ChangeGiaTien(this);
        }
    });

    $("#txtThoiGianThucHien").keydown(function (event) {
        ValidateInputKeydown(event, this, 1);
    }).blur(function (event) {
        ValidateInputFocusOut(event, this, 1);
    });

    var isShowing = false;
    $('.date').datepicker({
        todayBtn: "linked",
        keyboardNavigation: false,
        forceParse: false,
        autoclose: true,
        language: 'vi',
        todayHighlight: true,
        format: "dd/mm/yyyy"
    }).on('hide', () => {
        isShowing = false;
    }).on('show', () => {
        isShowing = true;
    });

    $(".txtNgayHopDong").keydown(function (event) {
        ValidateInputKeydown(event, this, 3);
    }).blur(function (event) {
        setTimeout(() => {
            if (!isShowing) ValidateInputFocusOut(event, this, 3);
        }, 0);
    });

    $("#slbDonVi").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#slbChuongTrinh").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#slbLoai").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#slbDuAn").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#slbLoaiHopDong").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#slbNhaThau").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#slbMaNgoaiTeKhac").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#slbTiGia").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });

    if ($("#slbLoai").val() == 0 || $("#slbLoai").val() == 2 || $("#slbLoai").val() == 4) {
        $("#divTenDuAn").hide();
    }

    if ($("#slbMaNgoaiTeKhac").val() != GUID_EMPTY) {
        $("#iDTenNgoaiTeKhac").html($("#slbMaNgoaiTeKhac option:selected").html());
    }

    if ($("#slbTiGia").val() != GUID_EMPTY) {
        var maGoc = $("#slbTiGia option:selected").data("mg");
        if (maTienTe.indexOf(maGoc.toUpperCase()) >= 0) {
            switch (maGoc.toUpperCase()) {
                case "USD":
                    $("#txtHopDongVND").prop("readonly", true);
                    $("#txtHopDongEUR").prop("readonly", true);
                    break;
                case "VND":
                    $("#txtHopDongUSD").prop("readonly", true);
                    $("#txtHopDongEUR").prop("readonly", true);
                    break;
                case "EUR":
                    $("#txtHopDongUSD").prop("readonly", true);
                    $("#txtHopDongVND").prop("readonly", true);
                    break;
                default:
                    break;
            }
            $("#txtHopDongNgoaiTeKhac").prop("readonly", true);
        } else {
            if ($("#slbMaNgoaiTeKhac").val() != GUID_EMPTY) {
                $("#txtHopDongUSD").prop("readonly", true);
                $("#txtHopDongVND").prop("readonly", true);
                $("#txtHopDongEUR").prop("readonly", true);
            }
        }
    }
});

function ChangeDonViSelect() {
    var iDonVi = $("#slbDonVi").val();
    var maDonVi = $("<div/>").text($("#slbDonVi").find("option:selected").data("madonvi")).html();
    $.ajax({
        type: "POST",
        url: "/QLNH/QLThongTinHopDong/GetChuongTrinhTheoDonVi",
        data: { iDonVi: iDonVi, maDonVi: maDonVi },
        success: function (data) {
            if (data) {
                $("#slbChuongTrinh").empty().html(data.htmlCT);
                $("#slbDuAn").empty().html(data.htmlDA);

                $("#slbChuongTrinh").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
                $("#slbDuAn").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
            }
        }
    });
}

function ChangeChuongTrinhSelect() {
    var iChuongTrinh = $("#slbChuongTrinh").val();
    $.ajax({
        type: "POST",
        url: "/QLNH/QLThongTinHopDong/GetDuAnTheoChuongTrinh",
        data: { iChuongTrinh: iChuongTrinh },
        success: function (data) {
            if (data) {
                $("#slbDuAn").empty().html(data);
                $("#slbDuAn").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
            }
        }
    });
}

function ChangeLoaiSelect() {
    var loaiVal = $("#slbLoai").val();
    if (loaiVal == 1 || loaiVal == 3) {
        $("#divTenDuAn").show();
    } else {
        $("#slbDuAn").val(GUID_EMPTY);
        $("#divTenDuAn").hide();
    }
    $("#slbDuAn").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
}

function ChangeTiGiaSelect() {
    $("#txtHopDongUSD").prop("readonly", true);
    $("#txtHopDongVND").prop("readonly", true);
    $("#txtHopDongEUR").prop("readonly", true);
    $("#txtHopDongNgoaiTeKhac").prop("readonly", true);
    $("#btnLuuModal").prop("disabled", true);
    $("#btnHuyModal").prop("disabled", true);

    var giaTriTienData = {};
    giaTriTienData.sGiaTriUSD = UnFormatNumber($("#txtHopDongUSD").val());
    giaTriTienData.sGiaTriVND = UnFormatNumber($("#txtHopDongVND").val());
    giaTriTienData.sGiaTriEUR = UnFormatNumber($("#txtHopDongEUR").val());
    var idTiGia = $("#slbTiGia").val();
    $.ajax({
        type: "POST",
        url: "/QLNH/QLThongTinHopDong/ChangeTiGia",
        data: { idTiGia: idTiGia, giaTriTienData: giaTriTienData },
        success: function (data) {
            $("#txtHopDongUSD").prop("readonly", false);
            $("#txtHopDongVND").prop("readonly", false);
            $("#txtHopDongEUR").prop("readonly", false);
            $("#txtHopDongNgoaiTeKhac").prop("readonly", false);
            $("#btnLuuModal").prop("disabled", false);
            $("#btnHuyModal").prop("disabled", false);
            if (data && data.bIsComplete) {
                if (idTiGia != "" && idTiGia != GUID_EMPTY) {
                    $("#slbMaNgoaiTeKhac").empty().html(data.htmlMNTK);
                    if (data.isChangeInputUSD) $("#txtHopDongUSD").val(data.sGiaTriUSD).prop("readonly", true);
                    if (data.isChangeInputVND) $("#txtHopDongVND").val(data.sGiaTriVND).prop("readonly", true);
                    if (data.isChangeInputEUR) $("#txtHopDongEUR").val(data.sGiaTriEUR).prop("readonly", true);
                    $("#txtHopDongNgoaiTeKhac").val("").prop("disabled", data.isReadonlyTxtMaNTKhac);
                } else {
                    $("#slbMaNgoaiTeKhac").val(GUID_EMPTY);
                    $("#txtHopDongNgoaiTeKhac").val("").prop("disabled", false);
                }
                $("#slbMaNgoaiTeKhac").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
                $("#iDTenNgoaiTeKhac").html("Ngoại tệ khác");
                $("#tienTeQuyDoiID").html(data.htmlTienTe);
            } else {
                var Title = 'Lỗi tính giá trị thông tin hợp đồng';
                var messErr = [];
                messErr.push(data.sMessError);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (res) {
                        $("#divModalConfirm").html(res);
                    }
                });
            }
        }
    });
}

function ChangeNgoaiTeKhacSelect() {
    var idTiGia = $("#slbTiGia").val();
    var idNgoaiTeKhac = $("#slbMaNgoaiTeKhac").val();
    var maNgoaiTeKhac = $("#slbMaNgoaiTeKhac option:selected").html();
    if (idNgoaiTeKhac == GUID_EMPTY) {
        $("#iDTenNgoaiTeKhac").html("Ngoại tệ khác");
        if (idTiGia != "" && idTiGia != GUID_EMPTY) {
            if (maTienTe.indexOf($("#slbTiGia option:selected").data("mg").toUpperCase()) >= 0) {
                $("#txtHopDongNgoaiTeKhac").val("").prop("disabled", true);
            } else {
                $("#txtHopDongNgoaiTeKhac").val("").prop("disabled", false);
                $("#txtHopDongUSD").val("").prop("readonly", true);
                $("#txtHopDongVND").val("").prop("readonly", true);
                $("#txtHopDongEUR").val("").prop("readonly", true);
            }
        }
    } else {
        $("#iDTenNgoaiTeKhac").html(maNgoaiTeKhac);
    }
    if (idTiGia == "" || idTiGia == GUID_EMPTY) {
        return false;
    }

    $("#txtHopDongNgoaiTeKhac").prop("readonly", true);
    $("#btnLuuModal").prop("disabled", true);
    $("#btnHuyModal").prop("disabled", true);

    var giaTriTienData = {};
    giaTriTienData.sGiaTriUSD = UnFormatNumber($("#txtHopDongUSD").val());
    giaTriTienData.sGiaTriVND = UnFormatNumber($("#txtHopDongVND").val());
    giaTriTienData.sGiaTriEUR = UnFormatNumber($("#txtHopDongEUR").val());
    giaTriTienData.sGiaTriNgoaiTeKhac = UnFormatNumber($("#txtHopDongNgoaiTeKhac").val());
    $.ajax({
        type: "POST",
        url: "/QLNH/QLThongTinHopDong/ChangeTiGiaNgoaiTeKhac",
        data: { idTiGia: idTiGia, idNgoaiTeKhac: idNgoaiTeKhac, maNgoaiTeKhac: maNgoaiTeKhac, giaTriTienData: giaTriTienData },
        success: function (data) {
            $("#txtHopDongNgoaiTeKhac").prop("readonly", false);
            $("#btnLuuModal").prop("disabled", false);
            $("#btnHuyModal").prop("disabled", false);
            if (data && data.bIsComplete) {
                if (data.isChangeInputNgoaiTe) $("#txtHopDongNgoaiTeKhac").val(data.sGiaTriNTKhac);
                $("#txtHopDongNgoaiTeKhac").prop("readonly", data.isReadonlyTxtMaNTKhac);
                if (data.isChangeInputCommon) {
                    $("#txtHopDongUSD").val(data.sGiaTriUSD).prop("readonly", true);
                    $("#txtHopDongVND").val(data.sGiaTriVND).prop("readonly", true);
                    $("#txtHopDongEUR").val(data.sGiaTriEUR).prop("readonly", true);
                }
                $("#tienTeQuyDoiID").empty().html(data.htmlTienTe);
            } else {
                var Title = 'Lỗi tính giá trị ngoại tệ khác thông tin hợp đồng';
                var messErr = [];
                messErr.push(data.sMessError);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (res) {
                        $("#divModalConfirm").html(res);
                    }
                });
            }
        }
    });
}

function ChangeGiaTien(element) {
    if ($(element).prop("readonly")) return;
    var idTiGia = $("#slbTiGia").val();
    var idNgoaiTeKhac = $("#slbMaNgoaiTeKhac").val();
    var maNgoaiTeKhac = $("#slbMaNgoaiTeKhac option:selected").html();
    if (idTiGia == "" || idTiGia == GUID_EMPTY) {
        return;
    } else {
        if (element.id == "txtHopDongNgoaiTeKhac" && idNgoaiTeKhac == GUID_EMPTY) {
            return;
        }
    }
    var txtBlur = "";
    switch (element.id) {
        case "txtHopDongUSD":
            txtBlur = "USD";
            break;
        case "txtHopDongVND":
            txtBlur = "VND";
            break;
        case "txtHopDongEUR":
            txtBlur = "EUR";
            break;
        case "txtHopDongNgoaiTeKhac":
            break;
        default:
            break;
    }

    $("#txtHopDongUSD").prop("readonly", true);
    $("#txtHopDongVND").prop("readonly", true);
    $("#txtHopDongEUR").prop("readonly", true);
    $("#txtHopDongNgoaiTeKhac").prop("readonly", true);
    $("#btnLuuModal").prop("disabled", true);
    $("#btnHuyModal").prop("disabled", true);

    var giaTriTienData = {};
    giaTriTienData.sGiaTriUSD = UnFormatNumber($("#txtHopDongUSD").val());
    giaTriTienData.sGiaTriVND = UnFormatNumber($("#txtHopDongVND").val());
    giaTriTienData.sGiaTriEUR = UnFormatNumber($("#txtHopDongEUR").val());
    giaTriTienData.sGiaTriNgoaiTeKhac = UnFormatNumber($("#txtHopDongNgoaiTeKhac").val());

    $.ajax({
        type: "POST",
        url: "/QLNH/QLThongTinHopDong/ChangeGiaTien",
        data: { idTiGia: idTiGia, idNgoaiTeKhac: idNgoaiTeKhac, maNgoaiTeKhac: maNgoaiTeKhac, txtBlur: txtBlur, giaTriTienData: giaTriTienData },
        success: function (data) {
            $("#txtHopDongUSD").prop("readonly", false);
            $("#txtHopDongVND").prop("readonly", false);
            $("#txtHopDongEUR").prop("readonly", false);
            $("#txtHopDongNgoaiTeKhac").prop("readonly", false);
            $("#btnLuuModal").prop("disabled", false);
            $("#btnHuyModal").prop("disabled", false);
            if (data && data.bIsComplete) {
                if (data.isChangeInputUSD) $("#txtHopDongUSD").val(data.sGiaTriUSD).prop("readonly", true);
                if (data.isChangeInputVND) $("#txtHopDongVND").val(data.sGiaTriVND).prop("readonly", true);
                if (data.isChangeInputEUR) $("#txtHopDongEUR").val(data.sGiaTriEUR).prop("readonly", true);
                if (data.isChangeInputNgoaiTe) $("#txtHopDongNgoaiTeKhac").val(data.sGiaTriNTKhac).prop("readonly", true);
            } else {
                var Title = 'Lỗi tính giá trị thông tin hợp đồng';
                var messErr = [];
                messErr.push(data.sMessError);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (res) {
                        $("#divModalConfirm").html(res);
                    }
                });
            }
        }
    });
}

function Save() {
    var data = GetDataHopDong();

    if (!ValidateData(data)) {
        return false;
    }

    var giaTriTienData = {};
    giaTriTienData.sGiaTriUSD = UnFormatNumber($("#txtHopDongUSD").val());
    giaTriTienData.sGiaTriVND = UnFormatNumber($("#txtHopDongVND").val());
    giaTriTienData.sGiaTriEUR = UnFormatNumber($("#txtHopDongEUR").val());
    giaTriTienData.sGiaTriNgoaiTeKhac = UnFormatNumber($("#txtHopDongNgoaiTeKhac").val());

    var isDieuChinh = $("#hidIsDieuChinh").val();
    $.ajax({
        type: "POST",
        url: "/QLNH/QLThongTinHopDong/Save",
        data: { data: data, giaTriTienData: giaTriTienData, isDieuChinh: isDieuChinh },
        success: function (r) {
            if (r && r.bIsComplete) {
                window.location.href = "/QLNH/QLThongTinHopDong";
            } else {
                var Title = 'Lỗi lưu thông tin hợp đồng';
                var messErr = [];
                messErr.push(r.sMessError);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
            }
        }
    });
}

function GetDataHopDong() {
    var data = {};
    data.ID = $("#hidTTHopDongID").val();
    data.iID_HopDongGocID = ($("#hidTTHopDongGocID").val() == "" || $("#hidTTHopDongGocID").val() == GUID_EMPTY) ? null : $("#hidTTHopDongGocID").val();
    data.iLanDieuChinh = $("#hidLanDieuChinh").val() == '' ? 0 : parseInt($("#hidLanDieuChinh").val());
    data.iID_DonViID = $("#slbDonVi").val();
    data.iID_MaDonVi = $("<div/>").text($("#slbDonVi").find("option:selected").data("madonvi")).html();
    data.iID_KHCTBQP_ChuongTrinhID = $("#slbChuongTrinh").val();
    var loai = $("#slbLoai").val();
    data.iPhanLoai = loai;
    if (loai == 1 || loai == 3) {
        data.iID_DuAnID = $("#slbDuAn").val();
        data.iID_BQuanLyID = $("#slbDuAn").find("option:selected").data("bql");
    } else {
        data.iID_DuAnID = null;
        data.iID_BQuanLyID = null;
    }
    data.sSoHopDong = $("<div/>").text($.trim($("#txtSoHopDong").val())).html();
    data.dNgayHopDong = $("<div/>").text($.trim($("#txtNgayKyHopDong").val())).html();
    data.sTenHopDong = $("<div/>").text($.trim($("#txtTenHopDong").val())).html();
    data.iID_LoaiHopDongID = $("#slbLoaiHopDong").val() == GUID_EMPTY ? null : $("#slbLoaiHopDong").val();
    data.dKhoiCongDuKien = $("<div/>").text($.trim($("#txtKhoiCongDuKien").val())).html();
    data.dKetThucDuKien = $("<div/>").text($.trim($("#txtKetThucDuKien").val())).html();
    data.iThoiGianThucHien = $.trim($("#txtThoiGianThucHien").val());
    data.iID_NhaThauThucHienID = $("#slbNhaThau").val() == GUID_EMPTY ? null : $("#slbNhaThau").val();
    data.iID_TiGiaID = $("#slbTiGia").val() == GUID_EMPTY ? null : $("#slbTiGia").val();
    data.iID_TiGia_ChiTietID = $("#slbMaNgoaiTeKhac").val() == GUID_EMPTY ? null : $("#slbMaNgoaiTeKhac").val();
    data.sMaNgoaiTeKhac = $("#slbMaNgoaiTeKhac").val() == GUID_EMPTY ? null : $("#slbMaNgoaiTeKhac option:selected").html();

    return data;
}

function ValidateData(data) {
    var Title = 'Lỗi nhập dữ liệu thông tin hợp đồng';
    var Messages = [];

    if (data.iID_DonViID == null || data.iID_DonViID == GUID_EMPTY) {
        Messages.push("Đơn vị quản lý chưa chọn !");
    }

    if (data.iID_KHCTBQP_ChuongTrinhID == null || data.iID_KHCTBQP_ChuongTrinhID == GUID_EMPTY) {
        Messages.push("Tên chương trình chưa chọn !");
    }

    if (data.iPhanLoai == null || data.iPhanLoai == 0) {
        Messages.push("Loại chưa chọn !");
    }

    if (data.iPhanLoai == 1 || data.iPhanLoai == 3) {
        if (data.iID_DuAnID == null || data.iID_DuAnID == GUID_EMPTY) {
            Messages.push("Tên dự án chưa chọn !");
        }
    }

    if (data.sSoHopDong == null || data.sSoHopDong == "") {
        Messages.push("Số hợp đồng chưa nhập !");
    }

    if ($.trim($("#txtSoHopDong").val()) != "" && $.trim($("#txtSoHopDong").val()).length > 100) {
        Messages.push("Số hợp đồng vượt quá 100 kí tự !");
    }

    if ($.trim($("#txtTenHopDong").val()) != "" && $.trim($("#txtTenHopDong").val()).length > 300) {
        Messages.push("Tên hợp đồng vượt quá 300 kí tự !");
    }

    if ($.trim($("#txtNgayKyHopDong").val()) != "" && !dateIsValid($.trim($("#txtNgayKyHopDong").val()))) {
        Messages.push("Ngày kí hợp đồng không hợp lệ !");
    }
    var isStartDateInvalid = $.trim($("#txtKhoiCongDuKien").val()) != "" && !dateIsValid($.trim($("#txtKhoiCongDuKien").val()));
    if (isStartDateInvalid) {
        Messages.push("Thời gian thực hiện từ không hợp lệ !");
    }
    var isEndDateInvalid = $.trim($("#txtKetThucDuKien").val()) != "" && !dateIsValid($.trim($("#txtKetThucDuKien").val()));
    if (isEndDateInvalid) {
        Messages.push("Thời gian thực hiện đến không hợp lệ !");
    }
    if (!isStartDateInvalid && !isEndDateInvalid && !CompareDatetime($.trim($("#txtKhoiCongDuKien").val()), $.trim($("#txtKetThucDuKien").val()))) {
        Messages.push("Thời gian bắt đầu thực hiện hợp đồng phải nhỏ hơn thời gian kết thúc thực hiện hợp đồng !");
    }

    if (data.iID_TiGiaID == null || data.iID_TiGiaID == GUID_EMPTY) {
        Messages.push("Tỉ giá chưa chọn !");
    }

    if (Messages.length > 0) {
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: Messages, Category: ERROR },
            success: function (data) {
                $("#divModalConfirm").html(data);
            }
        });
        return false;
    }

    return true;
}

function Cancel() {
    window.location.href = "/QLNH/QLThongTinHopDong";
}