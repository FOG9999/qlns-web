var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var maTienTe = ["USD", "VND", "EUR"];

$(document).ready(function () {
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

    $(".txtNgayGoiThau").keydown(function (event) {
        ValidateInputKeydown(event, this, 3);
    }).blur(function (event) {
        setTimeout(() => {
            if (!isShowing) ValidateInputFocusOut(event, this, 3);
        }, 0);
    });

    $("#txtGoiThauVND").keydown(function (event) {
        ValidateInputKeydown(event, this, 1);
    }).blur(function (event) {
        if (ValidateInputFocusOut(event, this, 2)) {
            ChangeGiaTien(this);
        }
    });

    $(".txtGoiThau").keydown(function (event) {
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

    $("#slbDonVi").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#slbChuongTrinh").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#slbLoai").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#slbDuAn").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#slbNhaThau").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#slbMaNgoaiTeKhac").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#slbTiGia").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#slbTienTe").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#slbHinhThucCNT").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#slbPhuongThucCNT").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#slbLoaiHopDong").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });

    if ($("#slbLoai").val() == 0 || $("#slbLoai").val() == 2 || $("#slbLoai").val() == 4) {
        $("#divTenDuAn").hide();
    }

    if ($("#slbLoai").val() == 0 || $("#slbLoai").val() == 3 || $("#slbLoai").val() == 4) {
        $("#divLoaiTrongNuoc").hide();
    }
    switch ($("#slbLoai").val()) {
        case "1":
        case "2":
            $("#lblNhaThau").html("Nhà thầu");
            break;
        case "3":
        case "4":
            $("#lblNhaThau").html("Đơn vị ủy thác");
            break;
        default:
            $("#lblNhaThau").html("Nhà thầu/Đơn vị ủy thác");
            break;
    }

    if ($("#slbMaNgoaiTeKhac").val() != GUID_EMPTY) {
        $("#iDTenNgoaiTeKhac").html($("#slbMaNgoaiTeKhac option:selected").html());
    }

    if ($("#slbTiGia").val() != GUID_EMPTY) {
        var maGoc = $("#slbTiGia option:selected").data("mg");
        if (maTienTe.indexOf(maGoc.toUpperCase()) >= 0) {
            switch (maGoc.toUpperCase()) {
                case "USD":
                    $("#txtGoiThauVND").prop("readonly", true);
                    $("#txtGoiThauEUR").prop("readonly", true);
                    break;
                case "VND":
                    $("#txtGoiThauUSD").prop("readonly", true);
                    $("#txtGoiThauEUR").prop("readonly", true);
                    break;
                case "EUR":
                    $("#txtGoiThauUSD").prop("readonly", true);
                    $("#txtGoiThauVND").prop("readonly", true);
                    break;
                default:
                    break;
            }
            $("#txtGoiThauNgoaiTeKhac").prop("readonly", true);
        } else {
            if ($("#slbMaNgoaiTeKhac").val() != GUID_EMPTY) {
                $("#txtGoiThauUSD").prop("readonly", true);
                $("#txtGoiThauVND").prop("readonly", true);
                $("#txtGoiThauEUR").prop("readonly", true);
            }
        }
    }
});

function ChangeDonViSelect() {
    var iDonVi = $("#slbDonVi").val();
    var maDonVi = $("<div/>").text($("#slbDonVi").find("option:selected").data("madonvi")).html();
    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTinGoiThau/GetChuongTrinhTheoDonVi",
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
        url: "/QLNH/ThongTinGoiThau/GetDuAnTheoChuongTrinh",
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

    switch (loaiVal) {
        case "1":
        case "2":
            $("#divLoaiTrongNuoc").show();
            $("#spanSoQuyetDinh1, #spanNgayQuyetDinh1").html("Kế hoạch lựa chọn nhà thầu");
            $("#spanSoQuyetDinh2, #spanNgayQuyetDinh2").html("Kết quả lựa chọn nhà thầu");
            $("#lblNhaThau").html("Nhà thầu");
            break;
        case "3":
        case "4":
            $("#divLoaiTrongNuoc").hide();
            $("#spanSoQuyetDinh1, #spanNgayQuyetDinh1").html("Phương án nhập khẩu");
            $("#spanSoQuyetDinh2, #spanNgayQuyetDinh2").html("Phê duyệt kết quả đàm phán");
            $("#lblNhaThau").html("Đơn vị ủy thác");
            $("#slbHinhThucCNT").val(GUID_EMPTY).trigger('change');
            $("#slbPhuongThucCNT").val(GUID_EMPTY).trigger('change');
            $("#slbLoaiHopDong").val(GUID_EMPTY).trigger('change');
            break;
        default:
            $("#divLoaiTrongNuoc").hide();
            $("#spanSoQuyetDinh1, #spanNgayQuyetDinh1").html("");
            $("#spanSoQuyetDinh2, #spanNgayQuyetDinh2").html("");
            $("#lblNhaThau").html("Nhà thầu/Đơn vị ủy thác");
            $("#slbHinhThucCNT").val(GUID_EMPTY).trigger('change');
            $("#slbPhuongThucCNT").val(GUID_EMPTY).trigger('change');
            $("#slbLoaiHopDong").val(GUID_EMPTY).trigger('change');
            break;
    }

    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTinGoiThau/GetNhaThauTheoLoai",
        data: { iLoai: loaiVal },
        success: function (data) {
            if (data) {
                $("#slbNhaThau").empty().html(data);
                $("#slbNhaThau").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
            }
        }
    });
}

function ChangeTiGiaSelect() {
    $("#txtGoiThauUSD").prop("readonly", true);
    $("#txtGoiThauVND").prop("readonly", true);
    $("#txtGoiThauEUR").prop("readonly", true);
    $("#txtGoiThauNgoaiTeKhac").prop("readonly", true);
    $("#btnLuuModal").prop("disabled", true);
    $("#btnHuyModal").prop("disabled", true);

    var giaTriTienData = {};
    giaTriTienData.sGiaTriUSD = UnFormatNumber($("#txtGoiThauUSD").val());
    giaTriTienData.sGiaTriVND = UnFormatNumber($("#txtGoiThauVND").val());
    giaTriTienData.sGiaTriEUR = UnFormatNumber($("#txtGoiThauEUR").val());
    var idTiGia = $("#slbTiGia").val();
    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTinGoiThau/ChangeTiGia",
        data: { idTiGia: idTiGia, giaTriTienData: giaTriTienData },
        success: function (data) {
            $("#txtGoiThauUSD").prop("readonly", false);
            $("#txtGoiThauVND").prop("readonly", false);
            $("#txtGoiThauEUR").prop("readonly", false);
            $("#txtGoiThauNgoaiTeKhac").prop("readonly", false);
            $("#btnLuuModal").prop("disabled", false);
            $("#btnHuyModal").prop("disabled", false);
            if (data && data.bIsComplete) {
                if (idTiGia != "" && idTiGia != GUID_EMPTY) {
                    $("#slbMaNgoaiTeKhac").empty().html(data.htmlMNTK);
                    if (data.isChangeInputUSD) $("#txtGoiThauUSD").val(data.sGiaTriUSD).prop("readonly", true);
                    if (data.isChangeInputVND) $("#txtGoiThauVND").val(data.sGiaTriVND).prop("readonly", true);
                    if (data.isChangeInputEUR) $("#txtGoiThauEUR").val(data.sGiaTriEUR).prop("readonly", true);
                    $("#txtGoiThauNgoaiTeKhac").val("").prop("disabled", data.isReadonlyTxtMaNTKhac);
                } else {
                    $("#slbMaNgoaiTeKhac").val(GUID_EMPTY);
                    $("#txtGoiThauNgoaiTeKhac").val("").prop("disabled", false);
                }
                $("#slbMaNgoaiTeKhac").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
                $("#iDTenNgoaiTeKhac").html("Ngoại tệ khác");
                $("#tienTeQuyDoiID").html(data.htmlTienTe);
            } else {
                var Title = 'Lỗi tính giá trị thông tin gói thầu';
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
                $("#txtGoiThauNgoaiTeKhac").val("").prop("disabled", true);
            } else {
                $("#txtGoiThauNgoaiTeKhac").val("").prop("disabled", false);
                $("#txtGoiThauUSD").val("").prop("readonly", true);
                $("#txtGoiThauVND").val("").prop("readonly", true);
                $("#txtGoiThauEUR").val("").prop("readonly", true);
            }
        }
    } else {
        $("#iDTenNgoaiTeKhac").html(maNgoaiTeKhac);
    }
    if (idTiGia == "" || idTiGia == GUID_EMPTY) {
        return false;
    }

    $("#txtGoiThauNgoaiTeKhac").prop("readonly", true);
    $("#btnLuuModal").prop("disabled", true);
    $("#btnHuyModal").prop("disabled", true);

    var giaTriTienData = {};
    giaTriTienData.sGiaTriUSD = UnFormatNumber($("#txtGoiThauUSD").val());
    giaTriTienData.sGiaTriVND = UnFormatNumber($("#txtGoiThauVND").val());
    giaTriTienData.sGiaTriEUR = UnFormatNumber($("#txtGoiThauEUR").val());
    giaTriTienData.sGiaTriNgoaiTeKhac = UnFormatNumber($("#txtGoiThauNgoaiTeKhac").val());
    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTinGoiThau/ChangeTiGiaNgoaiTeKhac",
        data: { idTiGia: idTiGia, idNgoaiTeKhac: idNgoaiTeKhac, maNgoaiTeKhac: maNgoaiTeKhac, giaTriTienData: giaTriTienData },
        success: function (data) {
            $("#txtGoiThauNgoaiTeKhac").prop("readonly", false);
            $("#btnLuuModal").prop("disabled", false);
            $("#btnHuyModal").prop("disabled", false);
            if (data && data.bIsComplete) {
                if (data.isChangeInputNgoaiTe) $("#txtGoiThauNgoaiTeKhac").val(data.sGiaTriNTKhac);
                $("#txtGoiThauNgoaiTeKhac").prop("readonly", data.isReadonlyTxtMaNTKhac);
                if (data.isChangeInputCommon) {
                    $("#txtGoiThauUSD").val(data.sGiaTriUSD).prop("readonly", true);
                    $("#txtGoiThauVND").val(data.sGiaTriVND).prop("readonly", true);
                    $("#txtGoiThauEUR").val(data.sGiaTriEUR).prop("readonly", true);
                }
                $("#tienTeQuyDoiID").empty().html(data.htmlTienTe);
            } else {
                var Title = 'Lỗi tính giá trị ngoại tệ khác thông tin gói thầu';
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
        if (element.id == "txtGoiThauNgoaiTeKhac" && idNgoaiTeKhac == GUID_EMPTY) {
            return;
        }
    }
    var txtBlur = "";
    switch (element.id) {
        case "txtGoiThauUSD":
            txtBlur = "USD";
            break;
        case "txtGoiThauVND":
            txtBlur = "VND";
            break;
        case "txtGoiThauEUR":
            txtBlur = "EUR";
            break;
        case "txtGoiThauNgoaiTeKhac":
            break;
        default:
            break;
    }

    $("#txtGoiThauUSD").prop("readonly", true);
    $("#txtGoiThauVND").prop("readonly", true);
    $("#txtGoiThauEUR").prop("readonly", true);
    $("#txtGoiThauNgoaiTeKhac").prop("readonly", true);
    $("#btnLuuModal").prop("disabled", true);
    $("#btnHuyModal").prop("disabled", true);

    var giaTriTienData = {};
    giaTriTienData.sGiaTriUSD = UnFormatNumber($("#txtGoiThauUSD").val());
    giaTriTienData.sGiaTriVND = UnFormatNumber($("#txtGoiThauVND").val());
    giaTriTienData.sGiaTriEUR = UnFormatNumber($("#txtGoiThauEUR").val());
    giaTriTienData.sGiaTriNgoaiTeKhac = UnFormatNumber($("#txtGoiThauNgoaiTeKhac").val());

    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTinGoiThau/ChangeGiaTien",
        data: { idTiGia: idTiGia, idNgoaiTeKhac: idNgoaiTeKhac, maNgoaiTeKhac: maNgoaiTeKhac, txtBlur: txtBlur, giaTriTienData: giaTriTienData },
        success: function (data) {
            $("#txtGoiThauUSD").prop("readonly", false);
            $("#txtGoiThauVND").prop("readonly", false);
            $("#txtGoiThauEUR").prop("readonly", false);
            $("#txtGoiThauNgoaiTeKhac").prop("readonly", false);
            $("#btnLuuModal").prop("disabled", false);
            $("#btnHuyModal").prop("disabled", false);
            if (data && data.bIsComplete) {
                if (data.isChangeInputUSD) $("#txtGoiThauUSD").val(data.sGiaTriUSD).prop("readonly", true);
                if (data.isChangeInputVND) $("#txtGoiThauVND").val(data.sGiaTriVND).prop("readonly", true);
                if (data.isChangeInputEUR) $("#txtGoiThauEUR").val(data.sGiaTriEUR).prop("readonly", true);
                if (data.isChangeInputNgoaiTe) $("#txtGoiThauNgoaiTeKhac").val(data.sGiaTriNTKhac).prop("readonly", true);
            } else {
                var Title = 'Lỗi tính giá trị thông tin gói thầu';
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
    var data = GetDataGoiThau();

    if (!ValidateData(data)) {
        return false;
    }

    var giaTriTienData = {};
    giaTriTienData.sGiaTriUSD = UnFormatNumber($("#txtGoiThauUSD").val());
    giaTriTienData.sGiaTriVND = UnFormatNumber($("#txtGoiThauVND").val());
    giaTriTienData.sGiaTriEUR = UnFormatNumber($("#txtGoiThauEUR").val());
    giaTriTienData.sGiaTriNgoaiTeKhac = UnFormatNumber($("#txtGoiThauNgoaiTeKhac").val());

    var isDieuChinh = $("#hidIsDieuChinh").val();
    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTinGoiThau/Save",
        data: { data: data, giaTriTienData: giaTriTienData, isDieuChinh: isDieuChinh },
        success: function (r) {
            if (r && r.bIsComplete) {
                window.location.href = "/QLNH/ThongTinGoiThau";
            } else {
                var Title = 'Lỗi lưu thông tin gói thầu';
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

function GetDataGoiThau() {
    var data = {};
    data.ID = $("#hidTTGoiThauID").val();
    data.iID_HopDongGocID = ($("#hidTTGoiThauGocID").val() == "" || $("#hidTTGoiThauGocID").val() == GUID_EMPTY) ? null : $("#hidTTGoiThauGocID").val();
    data.iLanDieuChinh = $("#hidLanDieuChinh").val() == '' ? 0 : parseInt($("#hidLanDieuChinh").val());
    data.iID_DonViID = $("#slbDonVi").val();
    data.iID_MaDonVi = $("<div/>").text($("#slbDonVi").find("option:selected").data("madonvi")).html();
    data.iID_KHCTBQP_ChuongTrinhID = $("#slbChuongTrinh").val();
    var loai = $("#slbLoai").val();
    data.iPhanLoai = loai;
    if (loai == 1 || loai == 3) {
        data.iID_DuAnID = $("#slbDuAn").val();
    }
    if (loai == 1 || loai == 2) {
        data.sSoKeHoachLCNT = $("<div/>").text($.trim($("#txtSoQuyetDinh1").val())).html();
        data.dNgayKeHoachLCNT = $("<div/>").text($.trim($("#txtNgayQuyetDinh1").val())).html();
        data.sSoKetQuaLCNT = $("<div/>").text($.trim($("#txtSoQuyetDinh2").val())).html();
        data.dNgayKetQuaLCNT = $("<div/>").text($.trim($("#txtNgayQuyetDinh2").val())).html();
        data.iID_HinhThucChonNhaThauID = $("#slbHinhThucCNT").val() == GUID_EMPTY ? null : $("#slbHinhThucCNT").val();
        data.iID_PhuongThucChonNhaThauID = $("#slbPhuongThucCNT").val() == GUID_EMPTY ? null : $("#slbPhuongThucCNT").val();
        data.iID_LoaiHopDongID = $("#slbLoaiHopDong").val() == GUID_EMPTY ? null : $("#slbLoaiHopDong").val();
    } else if (loai == 3 || loai == 4) {
        data.sSoPANK = $("<div/>").text($.trim($("#txtSoQuyetDinh1").val())).html();
        data.dNgayPANK = $("<div/>").text($.trim($("#txtNgayQuyetDinh1").val())).html();
        data.sSoKetQuaDamPhan = $("<div/>").text($.trim($("#txtSoQuyetDinh2").val())).html();
        data.dNgayKetQuaDamPhan = $("<div/>").text($.trim($("#txtNgayQuyetDinh2").val())).html();
    }
    data.sTenGoiThau = $("<div/>").text($.trim($("#txtTenGoiThau").val())).html();
    data.sThanhToanBang = $("#slbTienTe").val() == "" ? null : $("<div/>").text($("#slbTienTe").val()).html();
    data.iThoiGianThucHien = $("<div/>").text($.trim($("#txtThoiGianThucHien").val())).html();
    data.iID_NhaThauThucHienID = $("#slbNhaThau").val() == GUID_EMPTY ? null : $("#slbNhaThau").val();
    data.iID_TiGiaID = $("#slbTiGia").val() == GUID_EMPTY ? null : $("#slbTiGia").val();
    data.iID_TiGia_ChiTietID = $("#slbMaNgoaiTeKhac").val() == GUID_EMPTY ? null : $("#slbMaNgoaiTeKhac").val();
    data.sMaNgoaiTeKhac = $("#slbMaNgoaiTeKhac").val() == GUID_EMPTY ? null : $("#slbMaNgoaiTeKhac option:selected").html();

    return data;
}

function ValidateData(data) {
    var Title = 'Lỗi nhập dữ liệu thông tin gói thầu';
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

    if (data.sTenGoiThau == null || data.sTenGoiThau == "") {
        Messages.push("Tên gói thầu chưa nhập !");
    }

    if ($.trim($("#txtTenGoiThau").val()) != "" && $.trim($("#txtTenGoiThau").val()).length > 300) {
        Messages.push("Tên gói thầu vượt quá 300 kí tự !");
    }

    if (data.iPhanLoai == 1 || data.iPhanLoai == 2) {
        if ($.trim($("#txtSoQuyetDinh1").val()) != "" && $.trim($("#txtSoQuyetDinh1").val()).length > 100) {
            Messages.push("Số quyết định " + $("#spanSoQuyetDinh1").text() + " vượt quá 100 kí tự !");
        }
        if (data.dNgayKeHoachLCNT && data.dNgayKeHoachLCNT != "" && !dateIsValid(data.dNgayKeHoachLCNT)) {
            Messages.push("Ngày quyết định " + $("#spanSoQuyetDinh1").text() + " không hợp lệ !");
        }
        if ($.trim($("#txtSoQuyetDinh2").val()) != "" && $.trim($("#txtSoQuyetDinh2").val()).length > 100) {
            Messages.push("Số quyết định " + $("#spanSoQuyetDinh2").text() + " vượt quá 100 kí tự !");
        }
        if (data.dNgayKetQuaLCNT && data.dNgayKetQuaLCNT != "" && !dateIsValid(data.dNgayKetQuaLCNT)) {
            Messages.push("Ngày quyết định " + $("#spanSoQuyetDinh2").text() + " không hợp lệ !");
        }
    } else if (data.iPhanLoai == 3 || data.iPhanLoai == 4) {
        if ($.trim($("#txtSoQuyetDinh1").val()) != "" && $.trim($("#txtSoQuyetDinh1").val()).length > 100) {
            Messages.push("Số quyết định " + $("#spanSoQuyetDinh1").text() + " vượt quá 100 kí tự !");
        }
        if (data.dNgayPANK && data.dNgayPANK != "" && !dateIsValid(data.dNgayPANK)) {
            Messages.push("Ngày quyết định " + $("#spanSoQuyetDinh1").text() + " không hợp lệ !");
        }
        if ($.trim($("#txtSoQuyetDinh2").val()) != "" && $.trim($("#txtSoQuyetDinh2").val()).length > 100) {
            Messages.push("Số quyết định " + $("#spanSoQuyetDinh2").text() + " vượt quá 100 kí tự !");
        }
        if (data.dNgayKetQuaDamPhan && data.dNgayKetQuaDamPhan != "" && !dateIsValid(data.dNgayKetQuaDamPhan)) {
            Messages.push("Ngày quyết định " + $("#spanSoQuyetDinh2").text() + " không hợp lệ !");
        }
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
    window.location.href = "/QLNH/ThongTinGoiThau";
}