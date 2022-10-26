var GUID_EMPTY = "00000000-0000-0000-0000-000000000000";
var TBL_DANH_SACH_THANH_TOAN_CHITIET = "tblDanhSachThanhToanChiTiet";
var TBL_DANH_SACH_KHV = "tblDanhSachKHV";
var TBL_DANH_SACH_CHIPHI = "tblDanhSachChiPhi";

var THANH_TOAN = 1;
var TAM_UNG = 2;
var THU_HOI_UNG = 3
var THU_HOI_NAM_TRUOC = 4;
var THU_HOI_NAM_NAY = 5;

var NAM_TRUOC = 1;
var NAM_NAY = 2;

var arrKeHoachVon = [];
var arrDeNghiTamUngNT = [];
var arrDeNghiTamUngNN = [];
var bLoaded = true;
var iIdDeNghiThanhToanId;
var statusSave = 1;

const LOAI_CO_QUAN_TAI_CHINH = {
    CQTC: "Cơ quan tài chính bộ quốc phòng",
    CTC: "Cục tài chính"
}

//====================== trigger ===============================//
$("#txtNamKeHoach").trigger("change");
$("#drpLoaiKhoan").trigger("change");
$("#drpNganh").trigger("change");
$("#drpDuAn").trigger("change");
$("#drpHopDong").trigger("change");

$(document).ready(function () {
    $("#iID_ChuDauTuID").change(GetDuAnByDVQLandCDT);
    $("#drpDonViQuanLy").change(GetDuAnByDVQLandCDT);

    //$("#drpDonViQuanLy").select2({ width: "80%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    //$("#iID_ChuDauTuID").select2({ width: "80%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    //$("#drpDuAn").select2({ width: "80%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    //$("#drpLoaiThanhToan").select2({ width: "80%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    //$("#drpHopDong").select2({ width: "80%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    //$("#drpNhaThau").select2({ width: "80%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    //$("#drpNguonNganSach").select2({ width: "80%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    //$("#drpCoQuanThanhToan").select2({ width: "80%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    //$("#txtSoDeNghi").keyup(function () {
    //    ValidateMaxLength(this, 50);
    //});
    //$("#txtSoCanCu").keyup(function () {
    //    ValidateMaxLength(this, 50);
    //});
    //$("#txtNgayDeNghi").keydown(function (event) {
    //    ValidateInputKeydown(event, this, 3);
    //}).blur(function (event) {
    //    setTimeout(() => {
    //        ValidateInputFocusOut(event, this, 3);
    //    }, 0);
    //});
    //$("#txtLuyKeGiaTriKLNghiemThu").keydown(function (event) {
    //    ValidateInputKeydown(event, this, 2);
    //}).blur(function (event) {
    //    setTimeout(() => {
    //        ValidateInputFocusOut(event, this, 2);
    //    }, 0);
    //});
    //$("#txtNgayCanCu").keydown(function (event) {
    //    ValidateInputKeydown(event, this, 3);
    //}).blur(function (event) {
    //    setTimeout(() => {
    //        ValidateInputFocusOut(event, this, 3);
    //    }, 0);
    //});
    //$("#sThongTinCanCu").keyup(function (event) {
    //    ValidateMaxLength(this, 1000);
    //});
    //$("#txtNguoiLap").keyup(function (event) {
    //    ValidateMaxLength(this, 255);
    //});
    //$("#txtNamKeHoach").keydown(function (event) {
    //    ValidateInputKeydown(event, this, 1);
    //}).blur(function (event) {
    //    setTimeout(() => {
    //        ValidateInputFocusOut(event, this, 6);
    //    }, 0);
    //});
    //$("#txtNoiDung").keyup(function (event) {
    //    ValidateMaxLength(this, 2000);
    //});

    $("#txtdntuVonTrongNuoc").on('keyup', () => {
        recalculateTyLeThanhToan();
    })

    $("#txtdntuVonNgoaiNuoc").on('keyup', () => {
        recalculateTyLeThanhToan();
    })

    $("#txtthtuUngTruocVonTrongNuoc").on('keyup', () => {
        recalculateTyLeThanhToan();
    })

    $("#txtthtuUngTruocVonNgoaiNuoc").on('keyup', () => {
        recalculateTyLeThanhToan();
    })

    $("#txtNamKeHoach").change(function (e) {
        $("#drpNganh").empty();
        ResetThanhToanChiTiet();
        GetDataKeHoachVon();
    });

    $("#drpHopDong").change(function (e) {
        GetDataDropdownNhaThau();
        LoadLuyKeThanhToan();
        var fTienHopDong = $(this).find(":selected").attr("data-ftienhopdong");
        $("#txtGiaTriHopDong").val(FormatNumber(fTienHopDong));
    });

    $("#drpNguonNganSach").change(function (e) {
        ResetThanhToanChiTiet();
        GetDataKeHoachVon();
        LoadLuyKeThanhToan();
    });

    $("#drpDuAn").change(function (e) {
        ResetThanhToanChiTiet();
        $("#drpNganh").empty();
        $("#drpNguonNganSach").empty();
        // $("#drpHopDong").empty();
        GetDataDropdownHopDong();
        GetDataDropdownNguonVon();
        GetDataKeHoachVon();
        GetDataChiPhi();
    });

    $("#txtNgayDeNghi").change(function (e) {
        LoadLuyKeThanhToan();
        GetDataKeHoachVon();
        GetDataChiPhi();
    })

    $("#drpLoaiThanhToan").change(function (e) {
        ResetThanhToanChiTiet();

        var iLoaiThanhToan = $("#drpLoaiThanhToan option:selected").val();
        if (iLoaiThanhToan != TAM_UNG) {
            // display cot Loai
            $("#" + TBL_DANH_SACH_THANH_TOAN_CHITIET + " tr th:eq(1)").show();
            $("#" + TBL_DANH_SACH_THANH_TOAN_CHITIET + " tr td:eq(1)").show();
        } else {
            // hidden cot Loai
            $("#" + TBL_DANH_SACH_THANH_TOAN_CHITIET + " tr th:eq(1)").hide();
            $("#" + TBL_DANH_SACH_THANH_TOAN_CHITIET + " tr td:eq(1)").hide();
        }
    })

    $("#drpCoQuanThanhToan").change(function (e) {
        GetDataKeHoachVon();
        LoadLuyKeThanhToan();
    })

    $("#bThanhToanTheoHopDong").change(function () {
        var isChecked = $(this).is(":checked");
        if (isChecked) {
            $(".divThongTinHopDong").show();
            $(".divDanhSachChiPhi").hide();
        } else {
            $(".divThongTinHopDong").hide();
            $(".divDanhSachChiPhi").show();
        }
        LoadLuyKeThanhToan();
    })

});

function ResetThanhToanChiTiet() {
    $("#" + TBL_DANH_SACH_THANH_TOAN_CHITIET + " tbody").html("");
}

function GetDuAnByDVQLandCDT() {
    var sidCDT = $("#iID_ChuDauTuID :selected").html();
    var sidDVQL = $("#drpDonViQuanLy :selected").val();
    var sId_CDT = sidCDT.split('-')[0].trim();
    if (sId_CDT != "" && sId_CDT != String.empty) {
        $.ajax({
            url: "/GiaiNganThanhToan/LayDanhSachDuAnTheoChuDauTu",
            type: "POST",
            data: { iIDChuDauTuID: sId_CDT, sId_DonViQuanLy: sidDVQL },
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data) {
                    $("#drpDuAn").html(data);
                    $("#drpDuAn").trigger("change");
                } else {
                    $("#drpDuAn").html('');
                    $("#drpDuAn").trigger("change");
                }
            },
            error: function (data) {

            }
        })
    } else {
        $("#drpDuAn option:not(:first)").remove();
        $("#drpDuAn ").trigger("change");
    }
}

function GetDataDropdownNguonVon() {
    var iIdDuAn = $("#drpDuAn").val();
    if (iIdDuAn == undefined || iIdDuAn == null || iIdDuAn == GUID_EMPTY || iIdDuAn == "") return;


    $.ajax({
        type: "POST",
        url: "/GiaiNganThanhToan/GetDataNguonVon",
        data: {
            iIdDuAn: iIdDuAn
        },
        success: function (data) {
            if (data != null && data != "") {
                $("#drpNguonNganSach").html(data);
                $("#drpNguonNganSach").trigger("change");
            }
        }
    });
}

function GetDataDropdownHopDong() {
    var iIdDuAn = $("#drpDuAn option:selected").val();
    if (iIdDuAn == undefined || iIdDuAn == null || iIdDuAn == GUID_EMPTY || iIdDuAn == "") return;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/GetDataDropdownHopDong",
        data: {
            iIdDuAn: iIdDuAn
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#drpHopDong").empty()
                $.each(r.data, function (index, value) {
                    $("#drpHopDong").append("<option value='" + value.iID_HopDongID + "' data-fTienHopDong='" + (value.fTienHopDong == null ? 0 : value.fTienHopDong) + "'>" + value.sSoHopDong + "</option>");
                });
                $("#drpHopDong").prop("selectedIndex", -1);
            }
            if ($("#bIsEdit").val() == "1") {
                $("#drpHopDong").val($("#drpHopDongEdit").val());
                $("#drpHopDong").change();
            }
        }
    });
}

function GetDataDropdownNhaThau() {
    var iIdHopDong = $("#drpHopDong").val();
    if (iIdHopDong == GUID_EMPTY || iIdHopDong == null || iIdHopDong == "")
        return false;

    $.ajax({
        type: "POST",
        url: "/GiaiNganThanhToan/GetDataNhaThau",
        data: {
            iIdHopDong: iIdHopDong
        },
        success: function (data) {
            if (data != null && data != "") {
                $("#drpNhaThau").html(data);
                $("#drpNhaThau").trigger("change");
            }
        }
    });
}

function GetDataChiPhi() {
    arrChiPhi = [];
    $("#" + TBL_DANH_SACH_CHIPHI + " tbody").html("");
    var iIdDuAn = $("#drpDuAn").val();
    var dNgayDeNghi = $("#txtNgayDeNghi").val();

    if (iIdDuAn == null || iIdDuAn == "" || iIdDuAn == GUID_EMPTY
        || dNgayDeNghi == "")
        return false;

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/LoadChiPhi",
        data: {
            iIdDuAnId: iIdDuAn,
            dNgayDeNghi: dNgayDeNghi,
            iID_DeNghiThanhToanID: GUID_EMPTY
        },
        success: function (r) {
            if (r.bIsComplete) {
                if (r.data != null && r.data.length > 0) {
                    var html = "";
                    r.data.forEach(function (item) {
                        var dongMoi = "";
                        dongMoi += "<tr>";
                        dongMoi += "<td class='r_checkbox' align='center'><input type='checkbox' data-id='" + item.IIdChiPhiId + "' name='cb_chiphi' class='cb_chiphi'></td>";
                        dongMoi += "<td class='r_stenchiphi' align='left'>" + item.STenChiPhi + "</td>";
                        dongMoi += "<td class='r_giatriqddt' align='right'>" + item.SGiaTriPheDuyetQdDauTu + "</td>";
                        dongMoi += "<td class='r_giatridutoan' align='right'>" + item.SGiaTriPheDuyetDuToan + "</td>";
                        dongMoi += "</tr>";
                        html += dongMoi;
                    })
                    $("#" + TBL_DANH_SACH_CHIPHI + " tbody").html(html);

                    EventCheckboxChiPhi();
                }
            }
        }
    });
}

function EventCheckboxChiPhi() {
    $(".cb_chiphi").change(function () {
        $(".cb_chiphi").prop('checked', false);
        $(this).prop('checked', true);
    });
}

function GetDataKeHoachVon() {
    arrKeHoachVon = [];
    $("#" + TBL_DANH_SACH_KHV + " tbody").html("");
    var iIdDuAn = $("#drpDuAn").val();
    var iCoQuanThanhToan = $("#drpCoQuanThanhToan").val();
    var iIdNguonVon = $("#drpNguonNganSach").val();
    var dNgayDeNghi = $("#txtNgayDeNghi").val();
    var iNamKeHoach = $("#txtNamKeHoach").val();

    if (iIdDuAn == "" || iIdDuAn == GUID_EMPTY
        || iCoQuanThanhToan == ""
        || iIdNguonVon == "" || iIdNguonVon == GUID_EMPTY || iIdNguonVon == null
        || dNgayDeNghi == ""
        || iNamKeHoach == "")
        return false;

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/LoadListKehoachVonByDuAn",
        data: {
            iidDuAn: iIdDuAn,
            iidNguonVon: iIdNguonVon,
            dNgayDeNghiString: dNgayDeNghi,
            iNamKeHoach: iNamKeHoach,
            iCoQuanThanhToan: iCoQuanThanhToan,
            iidDeNghiThanhToanString: GUID_EMPTY
        },
        success: function (r) {
            if (r.bIsComplete) {
                if (r.data != null && r.data.length > 0) {
                    var html = "";
                    r.data.forEach(function (item) {
                        var dongMoi = "";
                        dongMoi += "<tr>";
                        dongMoi += "<td class='r_checkbox' align='center'><input type='checkbox' data-id='" + item.Id + "' data-phanloai='" + item.iPhanLoai + "' data-smanguoncha='" + item.sMaNguonCha + "' name='cb_KHV' class='cb_KHV'></td>";
                        dongMoi += "<td class='r_stenloai' align='left'>" + item.sTenLoai + "</td>";
                        dongMoi += "<td class='r_ssoquyetdinh' align='left'>" + item.sSoQuyetDinh + "</td>";
                        dongMoi += "<td align='center'>" + item.sNgayQuyetDinh + "</td>";
                        dongMoi += "<td align='right'>" + item.sTongGiaTri + "</td>";
                        dongMoi += "</tr>";

                        html += dongMoi;
                    })
                    $("#" + TBL_DANH_SACH_KHV + " tbody").html(html);

                    EventCheckboxKHV();
                }
            }
        }
    });
}

function EventCheckboxKHV() {
    $("#" + TBL_DANH_SACH_KHV + " .cb_KHV").change(function (e) {
        arrKeHoachVon = [];
        $(".cb_KHV").prop('checked', false);
        $(this).prop('checked', true);

        var thisDong = $(this).closest("tr");
        if (this.checked) {
            var iid_KHV = $(this).data('id');
            var sSoQuyetDinh = $(thisDong).find(".r_ssoquyetdinh").html().trim();
            var iPhanLoai = $(this).data('phanloai');
            var sMaNguonCha = $(this).data('smanguoncha');
            arrKeHoachVon.push({
                Id: iid_KHV,
                sSoQuyetDinh: sSoQuyetDinh,
                iPhanLoai: iPhanLoai,
                sMaNguonCha: sMaNguonCha
            })
            LoadLuyKeThanhToan();
        } else {
            var iid_KHV = $(this).data('id');
            arrKeHoachVon = arrKeHoachVon.filter(function (x) { return x.Id != iid_KHV });
        }
        $("#" + TBL_DANH_SACH_THANH_TOAN_CHITIET + " tbody").html("");
    })
}

function LoadLuyKeThanhToan() {
    if (bLoaded) {
        $("#txtluyKeTTTN").val("");
        $("#txtluyKeTTNN").val("");
        $("#txtluyKeTUUngTruocTN").val("");
        $("#txtluyKeTUUngTruocNN").val("");
        $("#txtluyKeTUTN").val("");
        $("#txtluyKeTUNN").val("");

        var iID_HopDongID = $("#drpHopDong option:selected").val();
        var iID_ChiPhiID = $("input[name=cb_chiphi]:checked").attr('data-id');
        var dNgayDeNghi = $("#txtNgayDeNghi").val();
        var iNamKeHoach = $("#txtNamKeHoach").val();
        var iID_NguonVonID = $("#drpNguonNganSach option:selected").val();
        var iCoQuanThanhToan = $("#drpCoQuanThanhToan option:selected").val();
        var loaiCoQuanTaiChinh = null;
        if (iCoQuanThanhToan == 2) {
            loaiCoQuanTaiChinh = $("#drpCoQuanThanhToan option:selected").text() == LOAI_CO_QUAN_TAI_CHINH.CQTC ? 0 : 1;
        }
        var bThanhToanTheoHopDong = $("#bThanhToanTheoHopDong").is(":checked") ? true : false;
        var keHoachVon = arrKeHoachVon.length ? arrKeHoachVon[0].iPhanLoai : null;
        var iIdChungTu = "";
        if (bThanhToanTheoHopDong == true)
            iIdChungTu = iID_HopDongID;
        else
            iIdChungTu = iID_ChiPhiID;

        if (iIdChungTu == undefined || iIdChungTu == "" || iIdChungTu == GUID_EMPTY
            || dNgayDeNghi == ""
            || iNamKeHoach == "" || iNamKeHoach == null
            || iID_NguonVonID == "" || iID_NguonVonID == null || iID_NguonVonID == GUID_EMPTY
            || iCoQuanThanhToan == "" || iCoQuanThanhToan == null || iCoQuanThanhToan == GUID_EMPTY
        )
            return false;

        bLoaded = false;
        $.ajax({
            type: "POST",
            url: "/QLVonDauTu/GiaiNganThanhToan/LoadGiaTriThanhToanNew",
            data: {
                bThanhToanTheoHopDong: bThanhToanTheoHopDong,
                iIdChungTu: iIdChungTu,
                dNgayDeNghi: dNgayDeNghi,
                iIDNguonVon: iID_NguonVonID,
                iNamKeHoach: iNamKeHoach,
                iCoQuanThanhToan: iCoQuanThanhToan,
                loaiCoQuanTaiChinh,
                keHoachVon
            },
            success: function (r) {
                $("#txtluyKeTTTN").val(FormatNumber(r.luyKeTTTN));
                $("#txtluyKeTTNN").val(FormatNumber(r.luyKeTTNN));
                $("#txtluyKeTUUngTruocTN").val(FormatNumber(r.luyKeTUUngTruocTN));
                $("#txtluyKeTUUngTruocNN").val(FormatNumber(r.luyKeTUUngTruocNN));
                $("#txtluyKeTUTN").val(FormatNumber(r.luyKeTUTN));
                $("#txtluyKeTUNN").val(FormatNumber(r.luyKeTUNN));
                bLoaded = true;
            }
        });
    }
}

//===================== Event button ==========================//

function CancelSaveData() {
    location.href = "/QLVonDauTu/GiaiNganThanhToan";
}

function Insert() {
    if (statusSave == 1) {
        if (!ValidateDataInsert()) return;
        SaveDeNghiThanhToan();
    } else if (statusSave == 2) {
        if ($('#txtNgayPheDuyet').val()) { SavePheDuyetThanhToanChiTiet(); }
        else {
            alert("Chưa nhập ngày phê duyệt");
        }
    }
}

function GetThongTinDeNghi() {
    var data = {};

    data.iID_DeNghiThanhToanID = iIdDeNghiThanhToanId;
    data.sSoDeNghi = $("#txtSoDeNghi").val().trim();
    data.dNgayDeNghi = $("#txtNgayDeNghi").val();
    data.iID_DonViQuanLyID = $("#drpDonViQuanLy option:selected").val();
    data.sNguoiLap = $("#txtNguoiLap").val().trim();
    data.iNamKeHoach = $("#txtNamKeHoach").val();
    data.iID_NguonVonID = $("#drpNguonNganSach option:selected").val();
    data.sSoBangKLHT = $("#txtSoCanCu").val().trim();
    data.dNgayBangKLHT = $("#txtNgayCanCu").val();

    data.fLuyKeGiaTriNghiemThuKLHT = parseInt($("#txtLuyKeGiaTriKLNghiemThu").val() == "" ? 0 : UnFormatNumber($("#txtLuyKeGiaTriKLNghiemThu").val()));

    data.fGiaTriThanhToanTN = parseInt($("#txtdntuVonTrongNuoc").val() == "" ? 0 : UnFormatNumber($("#txtdntuVonTrongNuoc").val()));
    data.fGiaTriThanhToanNN = parseInt($("#txtdntuVonNgoaiNuoc").val() == "" ? 0 : UnFormatNumber($("#txtdntuVonNgoaiNuoc").val()));


    data.fGiaTriThuHoiTN = parseInt($("#txtthtuVonTrongNuoc").val() == "" ? 0 : UnFormatNumber($("#txtthtuVonTrongNuoc").val()));
    data.fGiaTriThuHoiNN = parseInt($("#txtthtuVonNgoaiNuoc").val() == "" ? 0 : UnFormatNumber($("#txtthtuVonNgoaiNuoc").val()));

    data.fGiaTriThuHoiUngTruocTN = parseInt($("#txtthtuUngTruocVonTrongNuoc").val() == "" ? 0 : UnFormatNumber($("#txtthtuUngTruocVonTrongNuoc").val()));
    data.fGiaTriThuHoiUngTruocNN = parseInt($("#txtthtuUngTruocVonNgoaiNuoc").val() == "" ? 0 : UnFormatNumber($("#txtthtuUngTruocVonNgoaiNuoc").val()));
    data.sGhiChu = $("#txtNoiDung").val().trim();

    data.iLoaiThanhToan = $("#drpLoaiThanhToan option:selected").val();
    data.iID_DuAnId = $("#drpDuAn option:selected").val();

    data.iCoQuanThanhToan = $("#drpCoQuanThanhToan option:selected").val();
    data.fThueGiaTriGiaTang = parseInt($("#txtthtuThueGTGT").val() == "" ? 0 : UnFormatNumber($("#txtthtuThueGTGT").val()));
    data.fChuyenTienBaoHanh = parseInt($("#txtthtuTienBaoHanh").val() == "" ? 0 : UnFormatNumber($("#txtthtuTienBaoHanh").val()));

    data.bThanhToanTheoHopDong = $("#bThanhToanTheoHopDong").is(":checked") ? true : false;
    if (data.bThanhToanTheoHopDong == true) {
        data.iID_HopDongId = $("#drpHopDong option:selected").val();
        data.iID_NhaThauId = $("#drpNhaThau option:selected").val();
    } else {
        data.iID_ChiPhiID = $("input[name=cb_chiphi]:checked").attr('data-id');
    }
    data.sThongTinCanCu = $('#sThongTinCanCu').val().trim();
    data.loaiCoQuanTaiChinh = $("#drpCoQuanThanhToan option:selected").text() == LOAI_CO_QUAN_TAI_CHINH.CQTC ? 0 : 1;

    if (!iIdDeNghiThanhToanId) {
        data = {
            ...data,
            fLuyKeThanhToanTN: parseIntEmptyStr(UnFormatNumber($('#txtluyKeTTTN').val())),
            fLuyKeThanhToanNN: parseIntEmptyStr(UnFormatNumber($('#txtluyKeTTNN').val())),
            fLuyKeTUChuaThuHoiTN: parseIntEmptyStr(UnFormatNumber($('#txtluyKeTUUngTruocTN').val())),
            fLuyKeTUChuaThuHoiNN: parseIntEmptyStr(UnFormatNumber($('#txtluyKeTUUngTruocNN').val())),
            fLuyKeTUChuaThuHoiKhacTN: parseIntEmptyStr(UnFormatNumber($('#txtluyKeTUTN').val())),
            fLuyKeTUChuaThuHoiKhacNN: parseIntEmptyStr(UnFormatNumber($('#txtluyKeTUNN').val())),
        }
    }

    return data;
}

function SaveDeNghiThanhToan() {
    var data = GetThongTinDeNghi();
    data.listKeHoachVon = arrKeHoachVon;

    $.ajax({
        type: "POST",
        url: "/GiaiNganThanhToan/InsertDeNghiThanhToan",
        data: {
            data: data
        },
        success: function (r) {
            if (r.bIsComplete) {
                statusSave = 2;
                alert("Tạo mới thành công phiếu đề nghị thanh toán.");
                iIdDeNghiThanhToanId = r.iIdDeNghiThanhToanId;
                $(".div_ThongTinThanhToan").show();
                GetListLoaiThanhToan();
                GetListKeHoachVonThanhToan();
            } else {
                alert("Tạo thanh toán thất bại.");
            }
        }
    });
}

function SavePheDuyetThanhToanChiTiet() {
    var dataChiTiet = GetListThanhToanChiTiet();
    var dNgayPheDuyet = $('#txtNgayPheDuyet').val();
    var fThueGiaTriGiaTangDuocDuyet = parseIntEmptyStr(UnFormatNumber($('#txtthtuThueGTGTDuocDuyet').val()));
    var fChuyenTienBaoHanhDuocDuyet = parseIntEmptyStr(UnFormatNumber($('#txtthtuTienBaoHanhDuocDuyet').val()));

    $.ajax({
        type: "POST",
        url: "/GiaiNganThanhToan/InsertPheDuyetThanhToanChiTiet",
        data: {
            data: dataChiTiet,
            iIdDeNghiThanhToanId: iIdDeNghiThanhToanId,
            dNgayPheDuyet,
            fThueGiaTriGiaTangDuocDuyet,
            fChuyenTienBaoHanhDuocDuyet
        },
        success: function (r) {
            if (r.bIsComplete) {
                alert("Tạo mới thành công phiếu phê duyệt thanh toán.");
                location.href = "/QLVonDauTu/GiaiNganThanhToan";
            } else {
                alert("Tạo thanh toán thất bại.");
            }
        }
    });
}

function GetListThanhToanChiTiet() {
    var lstData = [];
    $("#" + TBL_DANH_SACH_THANH_TOAN_CHITIET + " tbody tr").each(function () {
        var iID_PheDuyetThanhToan_ChiTietID = $(this).find(".r_iID_ThanhToanChiTietID").val();

        var isDelete = $(this).hasClass('error-row');
        var iLoai = $(this).find(".r_Loai select").val();

        var iID_KeHoachVonID = $(this).find(".r_KeHoachVon select").val();
        var iLoaiKeHoachVon = $(this).find(".r_KeHoachVon select option:selected").attr("data-iloaikehoachvon");
        var iCoQuanThanhToanKHV = $(this).find(".r_KeHoachVon select option:selected").attr("data-icoquanthanhtoankhv");
        var iLoaiNamKeHoach = $(this).attr("data-iloainamkehoach");
        var iLoaiDeNghi = $(this).attr("data-iloaidenghi");

        var fGiaTriNgoaiNuoc = parseInt($(this).find(".r_vonngoainuoc").text() == "" ? 0 : UnFormatNumber($(this).find(".r_vonngoainuoc").text()));
        var fGiaTriTrongNuoc = parseInt($(this).find(".r_vontrongnuoc").text() == "" ? 0 : UnFormatNumber($(this).find(".r_vontrongnuoc").text()));

        //var fGiaTriNgoaiNuoc = parseInt(UnFormatNumber($(this).find(".r_vonngoainuoc").text()));
        //var fGiaTriTrongNuoc = parseInt(UnFormatNumber($(this).find(".r_vontrongnuoc").text()));
        var sGhiChu = $(this).find(".r_ghichu").text();

        var sLNS = $(this).find(".r_Lns option:selected").text();
        var sL = $(this).find(".r_L option:selected").text();
        var sK = $(this).find(".r_K option:selected").text();
        var sM = $(this).find(".r_M option:selected").text();
        var sTM = $(this).find(".r_Tm option:selected").text();
        var sTTM = $(this).find(".r_Ttm option:selected").text();
        var sNG = $(this).find(".r_Ng option:selected").text();

        var sXauNoiMa = sLNS + "-" + sL + "-" + sK + "-" + sM + "-" + sTM + "-" + sTTM + "-" + sNG;
        sXauNoiMa = sXauNoiMa.replace(/[-]+$/g, '');

        lstData.push({
            iID_PheDuyetThanhToan_ChiTietID: iID_PheDuyetThanhToan_ChiTietID,
            iLoai: iLoai,
            sLNS: sLNS,
            sL: sL,
            sK: sK,
            sM: sM,
            sTM: sTM,
            sTTM: sTTM,
            sNG: sNG,
            sXauNoiMa: sXauNoiMa,
            iLoaiNamKH: iLoaiNamKeHoach,
            iLoaiKeHoachVon: iLoaiKeHoachVon,
            iCoQuanThanhToanKHV: iCoQuanThanhToanKHV,
            iLoaiDeNghi: iLoaiDeNghi,
            iID_KeHoachVonID: iID_KeHoachVonID,
            fGiaTriNgoaiNuoc: fGiaTriNgoaiNuoc,
            fGiaTriTrongNuoc: fGiaTriTrongNuoc,
            sGhiChu: sGhiChu,
            isDelete: isDelete
        })
    })
    return lstData;
}

//=========================== validate ===========//
function ValidateDataInsert() {
    var sMessError = [];

    if (CheckTrungSoDeNghi()) {
        sMessError.push("Trùng số đề nghị, vui lòng nhập số khác");
    }
    if ($("#drpDonViQuanLy").val() == null || $("#drpDonViQuanLy").val() == GUID_EMPTY) {
        sMessError.push("Chưa nhập đơn vị quản lý.");
    }
    if ($("#iID_ChuDauTuID").val() == null || $("#iID_ChuDauTuID").val() == GUID_EMPTY) {
        sMessError.push("Chưa nhập chủ đầu tư.");
    }
    if ($("#txtSoDeNghi").val().trim() == "") {
        sMessError.push("Chưa nhập số đề nghị.");
    }
    if ($("#txtNgayDeNghi").val().trim() == "") {
        sMessError.push("Chưa nhập ngày đề nghị.");
    }
    if ($("#txtNamKeHoach").val().trim() == "") {
        sMessError.push("Chưa nhập năm kế hoạch.");
    }
    if ($("#drpNguonNganSach").val() == null) {
        sMessError.push("Chưa nhập nguồn vốn.");
    }
    if ($("#drpDuAn").val() == null || $("#drpDuAn").val() == GUID_EMPTY) {
        sMessError.push("Chưa nhập nhập dự án.");
    }

    if (sMessError.length != 0) {
        alert(sMessError.join('\n'));
        return false;
    }
    return true;
}

function ThemMoiThanhToanChiTiet() {
    var dataDeNghi = GetThongTinDeNghi();
    var iLoaiThanhToan = $("#drpLoaiThanhToan option:selected").val();
    var iID_ThanhToanChiTietID = uuidv4();

    var iLoaiDeNghi = "";
    var iLoaiNamKeHoach = "";
    if (iLoaiThanhToan == THANH_TOAN)
        iLoaiDeNghi = THANH_TOAN;
    else
        iLoaiDeNghi = TAM_UNG;

    if (arrKeHoachVonThanhToan != null && arrKeHoachVonThanhToan.length > 0) {
        iLoaiNamKeHoach = arrKeHoachVonThanhToan[0].ILoaiNamKhv
    }

    var fDefaultValueTN, fDefaultValueNN;
    if (iLoaiDeNghi == THANH_TOAN || iLoaiDeNghi == TAM_UNG) {
        fDefaultValueTN = dataDeNghi.fGiaTriThanhToanTN;
        fDefaultValueNN = dataDeNghi.fGiaTriThanhToanNN;
    }

    var dongMoi = "";
    dongMoi += "<tr style='cursor: pointer;' class='parent' data-xoa='0' data-iloaidenghi='" + iLoaiDeNghi + "' data-iloainamkehoach='" + iLoaiNamKeHoach + "'>";
    dongMoi += "<td class='r_STT' align='center'></td>";
    dongMoi += "<input type='hidden' class='r_iID_ThanhToanChiTietID' value='" + iID_ThanhToanChiTietID + "'/>";
    if (iLoaiThanhToan != TAM_UNG)
        dongMoi += "<td class='r_Loai'>" + CreateHtmlSelectLoai() + "</td>";
    dongMoi += "<td class='r_KeHoachVon'><select class='form-control' onchange='onChangeKeHoachVon(this)'></option></td>";
    dongMoi += "<td class='r_Lns'><select class='form-control' onchange='onChangeLNS(this)'></option></td>";
    dongMoi += "<td class='r_L'><select class='form-control' onchange='onChangeL(this)'></option></td>";
    dongMoi += "<td class='r_K'><select class='form-control' onchange='onChangeK(this)'></option></td>";
    dongMoi += "<td class='r_M'><select class='form-control' onchange='onChangeM(this)'></option></td>";
    dongMoi += "<td class='r_Tm'><select class='form-control' onchange='onChangeTM(this)'></option></td>";
    dongMoi += "<td class='r_Ttm'><select class='form-control' onchange='onChangeTTM(this)'></option></td>";
    dongMoi += "<td class='r_Ng'><select class='form-control'></option></td>";
    dongMoi += "<td class='r_NoiDung'></td>";
    dongMoi += "<td class='r_fGiaTriDeNghiTN sotien' align='right'>" + FormatNumber(fDefaultValueTN) + "</td>";
    dongMoi += "<td class='r_vontrongnuoc sotien' contenteditable='true' align='right'></td>";
    dongMoi += "<td class='r_fGiaTriDeNghiNN sotien' align='right'>" + FormatNumber(fDefaultValueNN) + "</td>";
    dongMoi += "<td class='r_vonngoainuoc sotien' contenteditable='true' align='right'></td>";

    dongMoi += "<td class='r_tongtien sotien' align='right'></td>";
    dongMoi += "<td class='r_ghichu' contenteditable='true'>";
    dongMoi += "<td align='center'><button class='btn-delete btn-icon' type='button' onclick='XoaDong(this)'><span class='fa fa-trash-o fa-lg' aria-hidden='true'></span></button></td>";
    $("#" + TBL_DANH_SACH_THANH_TOAN_CHITIET + " tbody").append(dongMoi);

    CapNhatCotStt(TBL_DANH_SACH_THANH_TOAN_CHITIET);
    EventValidate();

    $("#" + TBL_DANH_SACH_THANH_TOAN_CHITIET + " tbody tr:last-child td.r_Loai select").trigger("change");
    if (iLoaiThanhToan == TAM_UNG) {
        GetKeHoachVonTT();
    }
}

function CreateHtmlSelectLoai(value) {
    var htmlOption = "";

    arrLoaiThanhToan.forEach(function (item) {
        htmlOption += "<option value='" + item.ValueItem + "'>" + item.DisplayItem + "</option>";
    })
    return "<select class='form-control' onchange='onChangeLoaiThanhToan(this)'>" + htmlOption + "</option>";
}

var arrKeHoachVonThanhToan = [];
var arrMlns = {};
function GetListKeHoachVonThanhToan() {
    var dataDenghi = GetThongTinDeNghi();
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/GetListKeHoachVonThanhToan",
        data: {
            model: dataDenghi
        },
        async: false,
        success: function (r) {
            if (r.data != null && r.data.length > 0) {
                arrKeHoachVonThanhToan = r.data;
            }

            if (r.dataMlns != null && r.dataMlns.length > 0) {
                arrMlns = JSON.parse(r.dataMlns);
            }
        }
    });
}

var arrLoaiThanhToan = [];
function GetListLoaiThanhToan() {
    var dataDenghi = GetThongTinDeNghi();
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/LayDanhSachLoaiThanhToan",
        data: {
            model: dataDenghi
        },
        async: false,
        success: function (r) {
            if (r.bStatus) {
                arrLoaiThanhToan = r.listLoaiThanhToan;
            }
        }
    });
}

function CreateHtmlSelectKeHoachVon() {
    var htmlOption = "";
    arrKeHoachVonThanhToan.forEach(function (item) {
        htmlOption += "<option value='" + item.IIdKeHoachVonId + "' data-phanloai='" + item.iPhanLoai + "' data-smanguoncha='" + item.sMaNguonCha + "'>" + item.SDisplayName + "</option>";
    })

    return "<select class='form-control'>" + htmlOption + "</option>";
}

function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

function CapNhatCotStt(idBang) {
    $("#" + idBang + " tbody tr.parent").each(function (index, tr) {
        $(tr).find('.r_STT').text(index + 1);
    });
}

function XoaDong(nutXoa) {
    var dongXoa = $(nutXoa).closest("tr");

    var checkXoa = $(dongXoa).attr('data-xoa');
    if (checkXoa == 1) {
        $(dongXoa).attr('data-xoa', '0');
        $(dongXoa).removeClass('error-row');
    } else {
        $(dongXoa).attr('data-xoa', '1');
        $(dongXoa).addClass('error-row');
    }

    if (checkXoa == 1) {
        $(dongXoa).find('input, select').prop("disabled", "");
    } else {
        $(dongXoa).find('input, select').prop("disabled", "disabled");
    }
}

// event
function EventValidate() {
    $("td.sotien[contenteditable='true']").on("keypress", function (event) {
        return ValidateNumberKeyPress(this, event);
    })
    $("td.sotien[contenteditable='true']").on("focusout", function (event) {
        $(this).html(FormatNumber($(this).html() == "" ? 0 : UnFormatNumber($(this).html())));

        // tinh tong so tien
        var thisDong = this.parentElement;

        var fTrongNuoc = $(thisDong).find(".r_vontrongnuoc").html().trim();
        var fNuocNgoai = $(thisDong).find(".r_vonngoainuoc").html().trim();

        var fTong = parseInt(fTrongNuoc == "" ? 0 : UnFormatNumber(fTrongNuoc))
            + parseInt(fNuocNgoai == "" ? 0 : UnFormatNumber(fNuocNgoai));

        $(thisDong).find(".r_tongtien").html(FormatNumber(fTong));
    })

    $("td[contenteditable='true']").on("keydown", function (e) {
        var key = e.keyCode || e.charCode;
        if (key == 13) {
            $(this).blur();
        }
    });
}

function GetKeHoachVonTT() {
    /*var thisDong = $(obj).closest("tr");*/


    var htmlOption = "";
    arrKeHoachVonThanhToan.forEach(function (item) {
        htmlOption += "<option value='" + item.IIdKeHoachVonId + "' data-iloaikehoachvon='" + item.ILoaiKeHoachVon + "' data-icoquanthanhtoankhv='" + item.ICoQuanThanhToan + "'>" + item.SDisplayName + "</option>";
    })

    $('#tblDanhSachThanhToanChiTiet').find(".r_KeHoachVon select").html(htmlOption);
    $('#tblDanhSachThanhToanChiTiet').find(".r_KeHoachVon select").trigger("change");
}

function onChangeLoaiThanhToan(obj) {
    var thisDong = $(obj).closest("tr");

    var htmlOption = "";
    arrKeHoachVonThanhToan.forEach(function (item) {
        htmlOption += "<option value='" + item.IIdKeHoachVonId + "' data-iloaikehoachvon='" + item.ILoaiKeHoachVon + "' data-icoquanthanhtoankhv='" + item.ICoQuanThanhToan + "'>" + item.SDisplayName + "</option>";
    })

    $(thisDong).find(".r_KeHoachVon select").html(htmlOption);
    $(thisDong).find(".r_KeHoachVon select").trigger("change");
}

function onChangeKeHoachVon(obj) {
    var thisDong = $(obj).closest("tr");
    var iIdKeHoachVonId = $(obj).val();
    var lstMlns = arrMlns[iIdKeHoachVonId];
    if (lstMlns != null && lstMlns.length > 0) {
        var arrSelect = [];
        arrSelect.push({
            id: '',
            text: ''
        })
        lstMlns.forEach(function (item) {
            arrSelect.push({
                id: item.LNS,
                text: item.LNS
            })
        })
        $(thisDong).find(".r_Lns select").select2({
            data: arrSelect
        })

        $(thisDong).find(".r_Lns select").trigger("change");
    }
}

function onChangeLNS(obj) {
    var thisDong = $(obj).closest("tr");
    var iIdKeHoachVonId = $(thisDong).find(".r_KeHoachVon select").val();
    var sLNS = $(obj).val();
    var lstMlns = arrMlns[iIdKeHoachVonId];
    if (lstMlns != null && lstMlns.length > 0) {
        var arrSelect = [];
        arrSelect.push({
            id: '',
            text: ''
        })
        lstMlns.forEach(function (item) {
            if (item.LNS == sLNS) {
                arrSelect.push({
                    id: item.L,
                    text: item.L
                })
            }
        })
        $(thisDong).find(".r_L select").select2({
            data: arrSelect
        })

        $(thisDong).find(".r_L select").trigger("change");
    }
}

function onChangeL(obj) {
    var thisDong = $(obj).closest("tr");
    var iIdKeHoachVonId = $(thisDong).find(".r_KeHoachVon select").val();
    var sLNS = $(thisDong).find(".r_Lns select").val();
    var sL = $(obj).val();
    var lstMlns = arrMlns[iIdKeHoachVonId];
    if (lstMlns != null && lstMlns.length > 0) {
        var arrSelect = [];
        arrSelect.push({
            id: '',
            text: ''
        })
        lstMlns.forEach(function (item) {
            if (item.LNS == sLNS && item.L == sL) {
                arrSelect.push({
                    id: item.K,
                    text: item.K
                })
            }
        })
        $(thisDong).find(".r_K select").select2({
            data: arrSelect
        })

        $(thisDong).find(".r_K select").trigger("change");
    }
};

function onChangeK(obj) {
    var thisDong = $(obj).closest("tr");
    var iIdKeHoachVonId = $(thisDong).find(".r_KeHoachVon select").val();
    var sLNS = $(thisDong).find(".r_Lns select").val();
    var sL = $(thisDong).find(".r_L select").val();
    var sK = $(obj).val();
    var lstMlns = arrMlns[iIdKeHoachVonId];
    if (lstMlns != null && lstMlns.length > 0) {
        var arrSelect = [];
        arrSelect.push({
            id: '',
            text: ''
        })
        lstMlns.forEach(function (item) {
            if (item.LNS == sLNS && item.L == sL && item.K == sK) {
                arrSelect.push({
                    id: item.M,
                    text: item.M
                })
            }

        })
        $(thisDong).find(".r_M select").select2({
            data: arrSelect
        })

        $(thisDong).find(".r_M select").trigger("change");
    }
}

function onChangeM(obj) {
    var thisDong = $(obj).closest("tr");
    var iIdKeHoachVonId = $(thisDong).find(".r_KeHoachVon select").val();
    var sLNS = $(thisDong).find(".r_Lns select").val();
    var sL = $(thisDong).find(".r_L select").val();
    var sK = $(thisDong).find(".r_K select").val();
    var sM = $(obj).val();
    var lstMlns = arrMlns[iIdKeHoachVonId];
    if (lstMlns != null && lstMlns.length > 0) {
        var arrSelect = [];
        arrSelect.push({
            id: '',
            text: ''
        })
        lstMlns.forEach(function (item) {
            if (item.LNS == sLNS && item.L == sL && item.K == sK && item.M == sM) {
                arrSelect.push({
                    id: item.TM,
                    text: item.TM
                })
            }
        })
        $(thisDong).find(".r_Tm select").select2({
            data: arrSelect
        })

        GetNoiDungMLNS(obj);
        $(thisDong).find(".r_Tm select").trigger("change");
    }
};

function onChangeTM(obj) {
    var thisDong = $(obj).closest("tr");
    var iIdKeHoachVonId = $(thisDong).find(".r_KeHoachVon select").val();
    var sLNS = $(thisDong).find(".r_Lns select").val();
    var sL = $(thisDong).find(".r_L select").val();
    var sK = $(thisDong).find(".r_K select").val();
    var sM = $(thisDong).find(".r_M select").val();
    var sTM = $(obj).val();
    var lstMlns = arrMlns[iIdKeHoachVonId];
    if (lstMlns != null && lstMlns.length > 0) {
        var arrSelect = [];
        arrSelect.push({
            id: '',
            text: ''
        })
        lstMlns.forEach(function (item) {
            if (item.LNS == sLNS && item.L == sL && item.K == sK && item.M == sM && item.TM == sTM) {
                arrSelect.push({
                    id: item.TTM,
                    text: item.TTM
                })
            }
        })
        $(thisDong).find(".r_Ttm select").select2({
            data: arrSelect
        })

        GetNoiDungMLNS(obj);
        $(thisDong).find(".r_Ttm select").trigger("change");
    }
}

function onChangeTTM(obj) {
    var thisDong = $(obj).closest("tr");
    var iIdKeHoachVonId = $(thisDong).find(".r_KeHoachVon select").val();
    var sLNS = $(thisDong).find(".r_Lns select").val();
    var sL = $(thisDong).find(".r_L select").val();
    var sK = $(thisDong).find(".r_K select").val();
    var sM = $(thisDong).find(".r_M select").val();
    var sTM = $(thisDong).find(".r_Tm select").val();
    var sTTM = $(obj).val();
    var lstMlns = arrMlns[iIdKeHoachVonId];
    if (lstMlns != null && lstMlns.length > 0) {
        var arrSelect = [];
        arrSelect.push({
            id: '',
            text: ''
        })
        lstMlns.forEach(function (item) {
            if (item.LNS == sLNS && item.L == sL && item.K == sK && item.M == sM && item.TM == sTM && item.TTM == sTTM) {
                arrSelect.push({
                    id: item.NG,
                    text: item.NG
                })
            }
        })
        $(thisDong).find(".r_Ng select").select2({
            data: arrSelect
        })

        GetNoiDungMLNS(obj);
    }
}

function onChangeNG(obj) {
    var thisDong = $(obj).closest("tr");
    var iIdKeHoachVonId = $(thisDong).find(".r_KeHoachVon select").val();
    var lstMlns = arrMlns[iIdKeHoachVonId];
    if (lstMlns != null && lstMlns.length > 0) {
        GetNoiDungMLNS(obj);
    }
}

function GetNoiDungMLNS(obj) {
    var thisDong = $(obj).closest("tr");
    var iIdKeHoachVonId = $(thisDong).find(".r_KeHoachVon select").val();
    var sLNS = $(thisDong).find(".r_Lns select").val();
    var sL = $(thisDong).find(".r_L select").val();
    var sK = $(thisDong).find(".r_K select").val();
    var sM = $(thisDong).find(".r_M select").val();
    var sTM = $(thisDong).find(".r_Tm select").val();
    var sTTM = $(thisDong).find(".r_Ttm select").val();
    var sNG = $(thisDong).find(".r_Ng select").val();

    var sNoiDung = "";
    var lstMlns = arrMlns[iIdKeHoachVonId];
    if (lstMlns != null && lstMlns.length > 0) {
        var objMlns = lstMlns.filter(x => x.LNS == sLNS && x.L == sL && x.K == sK && x.M == sM && x.TM == sTM && x.TTM == sTTM && x.NG == sNG);
        if (objMlns != null && objMlns.length > 0) {
            sNoiDung = objMlns[0].SMoTa;
        }
    }

    $(thisDong).find(".r_NoiDung").html(sNoiDung);
}

function parseIntEmptyStr(str) {
    if (str === "") {
        return 0;
    }
    else return parseInt(str);
}

function recalculateTyLeThanhToan() {
    try {
        let fGiaTriThanhToanTN = parseIntEmptyStr(UnFormatNumber($('#txtdntuVonTrongNuoc').val() ? $('#txtdntuVonTrongNuoc').val() : 0));
        let fGiaTriThanhToanNN = parseIntEmptyStr(UnFormatNumber($('#txtdntuVonNgoaiNuoc').val() ? $('#txtdntuVonNgoaiNuoc').val() : 0));
        let fGiaTriThuHoiTN = parseIntEmptyStr(UnFormatNumber($('#txtthtuUngTruocVonTrongNuoc').val() ? $('#txtthtuUngTruocVonTrongNuoc').val() : 0));
        let fGiaTriThuHoiNN = parseIntEmptyStr(UnFormatNumber($('#txtthtuUngTruocVonNgoaiNuoc').val() ? $('#txtthtuUngTruocVonNgoaiNuoc').val() : 0));
        let LuyKeThanhToanKLHTVonTN = parseIntEmptyStr(UnFormatNumber($('#txtluyKeTTTN').val()));
        let LuyKeThanhToanKLHTVonNN = parseIntEmptyStr(UnFormatNumber($('#txtluyKeTTNN').val()));
        let LuyKeTamUngChuaThuHoiVonTN = parseIntEmptyStr(UnFormatNumber($('#txtluyKeTUUngTruocTN').val()));
        let LuyKeTamUngChuaThuHoiVonNN = parseIntEmptyStr(UnFormatNumber($('#txtluyKeTUUngTruocNN').val()));
        let LuyKeTamUngChuaThuHoiVonUngTruocTN = parseIntEmptyStr(UnFormatNumber($('#txtluyKeTUTN').val()));
        let LuyKeTamUngChuaThuHoiVonUngTruocNN = parseIntEmptyStr(UnFormatNumber($('#txtluyKeTUNN').val()));
        let FGiaTriThuHoiUngTruocTn = parseIntEmptyStr(UnFormatNumber($('#txtthtuVonTrongNuoc').val() ? $('#txtthtuVonTrongNuoc').val() : 0));
        let FGiaTriThuHoiUngTruocNn = parseIntEmptyStr(UnFormatNumber($('#txtthtuVonNgoaiNuoc').val() ? $('#txtthtuVonNgoaiNuoc').val() : 0));
        let GiaTriHopDong = parseIntEmptyStr(UnFormatNumber($('#txtGiaTriHopDong').val()));

        let tyLeThanhToan = (LuyKeThanhToanKLHTVonTN + LuyKeThanhToanKLHTVonNN + LuyKeTamUngChuaThuHoiVonTN +
            LuyKeTamUngChuaThuHoiVonNN + LuyKeTamUngChuaThuHoiVonUngTruocTN + LuyKeTamUngChuaThuHoiVonUngTruocNN +
            fGiaTriThanhToanTN + fGiaTriThanhToanNN - (fGiaTriThuHoiTN + fGiaTriThuHoiNN +
                FGiaTriThuHoiUngTruocTn +
                FGiaTriThuHoiUngTruocNn)) / GiaTriHopDong;

        $('#fTyLeThanhToan').val(tyLeThanhToan);
    }
    catch (e) {
        console.error(e)
    }
}

function CheckTrungSoDeNghi() {
    var sSoDeNghi = $("#txtSoDeNghi").val().trim();
    var check = false;
    $.ajax({
        url: "/QLVonDauTu/Common/KiemTraTrung",
        type: "POST",
        data: { sTable: "VDT_TT_DeNghiThanhToan", sColumn: "sSoDeNghi", val: sSoDeNghi },
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