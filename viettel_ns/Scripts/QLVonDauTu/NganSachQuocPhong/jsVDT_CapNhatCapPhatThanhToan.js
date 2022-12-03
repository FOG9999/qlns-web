var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var tblDataGrid = [];
var TBL_DANH_SACH_THANH_TOAN_CHITIET = 'tblDanhSachThanhToanChiTiet';
var TBL_DANH_SACH_KHV = "tblDanhSachKHV";

var THANH_TOAN = 1;
var TAM_UNG = 2;
var THU_HOI_UNG = 3;

var NAM_TRUOC = 1;
var NAM_NAY = 2;

var iID_DeNghiThanhToanID = "";
var iID_DonViQuanLyIDOld = "";
var iID_NguonVonIDOld = "";
var iLoaiThanhToanOld = "";
var iID_DuAnIdOld = "";
var iID_HopDongIdOld = "";
var iCoQuanThanhToanOld = "";
var dNgayDeNghiOld = "";
var iNamKeHoachOld = "";
var iid_NhaThauOldId = "";
var loaiCoQuanTaiChinh = "";

var arrKeHoachVon = [];
var arrDeNghiTamUngNT = [];
var arrDeNghiTamUngNN = [];

var arrLoaiThanhToan = [];
var arrKeHoachVonThanhToan = [];
var arrMlns = {};

const LOAI_CO_QUAN_TAI_CHINH = {
    CQTC: "Cơ quan tài chính bộ quốc phòng",
    CTC: "Cục tài chính"
}

$(document).ready(function () {
    iID_DeNghiThanhToanID = $("#iID_DeNghiThanhToanID").val();
    iID_DonViQuanLyIDOld = $("#iID_DonViQuanLyID").val();
    iID_NguonVonIDOld = $("#iID_NguonVonID").val();
    iLoaiThanhToanOld = $("#iLoaiThanhToan").val();
    iID_DuAnIdOld = $("#iID_DuAnId").val();
    iID_HopDongIdOld = $("#iID_HopDongId").val();
    iCoQuanThanhToanOld = $("#iCoQuanThanhToan").val();
    dNgayDeNghiOld = $("#dNgayDeNghi").val();
    iNamKeHoachOld = $("#iNamKeHoach").val();
    iid_NhaThauOldId = $("#iID_NhaThauId").val();
    loaiCoQuanTaiChinh = $("#loaiCoQuanTaiChinh").val();

    GetListDropdownPheDuyet();

    $("#iID_ChuDauTuID").change(function () {
        var sidCDT = $("#iID_ChuDauTuID :selected").html();
        var sId_CDT = sidCDT.split('-')[0].trim();
        if (sId_CDT != "" && sId_CDT != String.empty) {
            $.ajax({
                url: "/GiaiNganThanhToan/LayDanhSachDuAnTheoChuDauTu",
                type: "POST",
                data: { iIDChuDauTuID: sId_CDT },
                dataType: "json",
                cache: false,
                success: function (data) {
                    if (data != null && data != "") {
                        $("#drpDuAn").html(data);
                        $('#drpDuAn').val($('#iID_DuAnId').val());
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
    });

    $("#drpHopDong").change(function (e) {
        GetDetailHopDong();
        GetDataDropdownNhaThau();
        //LoadLuyKeThanhToan();

        LoadPheDuyetChiTiet();
        recalculateTyLeThanhToan();
    });

    $("#drpDuAn").change(function (e) {
        $("#drpNguonNganSach").empty();
        $("#drpHopDong").empty();
        GetDataDropdownHopDong();
        GetDataDropdownNguonVon();
    });

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

    $("#txtNgayDeNghi").change(function (e) {
        //LoadLuyKeThanhToan();
    })

    $("#drpDonViQuanLy").val(iID_DonViQuanLyIDOld);

    $("#iID_ChuDauTuID").trigger("change");

    //setTimeout(function () {
    // $("#iID_ChuDauTuID").trigger("change");
    // setTimeout(function () {
    // $("#drpDuAn").val(iID_DuAnIdOld).trigger("change");
    // setTimeout(function () {
    // $("#drpHopDong").val(iID_HopDongIdOld).trigger("change");
    // setTimeout(function () {
    // //$("#drpNguonNganSach").val(iID_NguonVonIDOld).trigger("change");
    // LoadPheDuyetChiTiet();
    // recalculateTyLeThanhToan();
    // }, 200);
    // }, 200);

    // }, 200);
    //}, 200);
    $("#drpLoaiThanhToan").val(iLoaiThanhToanOld);
    if (iCoQuanThanhToanOld == 1) {
        $("#drpCoQuanThanhToan").val(iCoQuanThanhToanOld)
    }
    else {
        if (loaiCoQuanTaiChinh == 0) {
            $("#drpCoQuanThanhToan").prop("selectedIndex", 1);
        }
        else $("#drpCoQuanThanhToan").prop("selectedIndex", 2);
    }
    $("#drpCoQuanThanhToan").change(function (e) {
        GetArrKeHoachVon();
        ////LoadLuyKeThanhToan();
    })

    $("#drpNguonNganSach").change(function (e) {
        ResetThanhToanChiTiet();
        GetArrKeHoachVon();
        //LoadLuyKeThanhToan();
    });

    $("#bThanhToanTheoHopDong").change(function () {
        //var isChecked = $(this).is(":checked");
        //if (isChecked) {
        // $(".divThongTinHopDong").show();
        // $(".divDanhSachChiPhi").hide();
        //} else {
        // $(".divThongTinHopDong").hide();
        // $(".divDanhSachChiPhi").show();
        //}
        //LoadLuyKeThanhToan();
    })
    $("#drpCoQuanThanhToan").trigger("change")
    // $("#drpCoQuanThanhToan").val(iCoQuanThanhToanOld).trigger("change");
});

function GetArrKeHoachVon() {
    $("#" + TBL_DANH_SACH_KHV + " .cb_KHV:checked").each(function (item) {
        var thisDong = this.parentElement.parentElement;
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
    })
}

function GetDataDeNghiTamUng() {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/LoadDeNghiTamUng",
        data: {
            iID_DeNghiThanhToanID: iID_DeNghiThanhToanID,
            iIdDuAn: iID_DuAnIdOld,
            iIdNguonVon: iID_NguonVonIDOld,
            dNgayDeNghi: dNgayDeNghiOld,
            iNamKeHoach: iNamKeHoachOld,
            iCoQuanThanhToan: iCoQuanThanhToanOld
        },
        success: function (r) {
            if (r.bIsComplete) {
                if (r.lstDeNghiTamUngNamTruoc != null && r.lstDeNghiTamUngNamTruoc.length > 0)
                    arrDeNghiTamUngNT = r.lstDeNghiTamUngNamTruoc;
                if (r.lstDeNghiTamUngNamNay != null && r.lstDeNghiTamUngNamNay.length > 0)
                    arrDeNghiTamUngNN = r.lstDeNghiTamUngNamNay;
            }
        }
    });
}

function LoadPheDuyetChiTiet() {
    $.ajax({
        type: "POST",
        url: "/GiaiNganThanhToan/GetPheDuyetThanhToanChiTiet",
        data: {
            iID_DeNghiThanhToanID: iID_DeNghiThanhToanID
        },
        success: function (r) {
            if (r.lstPheDuyet != null && r.lstPheDuyet.length > 0) {
                var html = "";
                r.lstPheDuyet.forEach(function (item) {
                    var dongMoi = "";
                    dongMoi += "<tr style='cursor: pointer;' class='parent' data-xoa='0' data-iloaidenghi='" + item.iLoaiDeNghi + "' data-iloainamkehoach='" + item.iLoaiNamKH + "'>";
                    dongMoi += "<td class='r_STT' align='center'></td>";
                    dongMoi += "<input type='hidden' class='r_iID_ThanhToanChiTietID' value='" + item.iID_PheDuyetThanhToan_ChiTietID + "'/>";
                    if (iLoaiThanhToan != TAM_UNG)
                        dongMoi += "<td class='r_Loai'>" + CreateHtmlSelectLoai(item.iLoai) + "</td>";
                    dongMoi += "<td class='r_KeHoachVon' data-value='" + item.iID_KeHoachVonID + "'><select class='form-control' onchange='onChangeKeHoachVon(this)'></option></td>";
                    dongMoi += "<td class='r_Lns' data-value='" + item.sLNS + "'><select class='form-control' onchange='onChangeLNS(this)'></option></td>";
                    dongMoi += "<td class='r_L' data-value='" + item.sL + "'><select class='form-control' onchange='onChangeL(this)'></option></td>";
                    dongMoi += "<td class='r_K' data-value='" + item.sK + "'><select class='form-control' onchange='onChangeK(this)'></option></td>";
                    dongMoi += "<td class='r_M' data-value='" + item.sM + "'><select class='form-control' onchange='onChangeM(this)'></option></td>";
                    dongMoi += "<td class='r_Tm' data-value='" + item.sTM + "'><select class='form-control' onchange='onChangeTM(this)'></option></td>";
                    dongMoi += "<td class='r_Ttm' data-value='" + item.sTTM + "'><select class='form-control' onchange='onChangeTTM(this)'></option></td>";
                    dongMoi += "<td class='r_Ng' data-value='" + item.sNG + "'><select class='form-control'></option></td>";

                    dongMoi += "<td class='r_NoiDung'>" + (item.sMoTa != null ? item.sMoTa : "") + "</td>";
                    dongMoi += "<td class='r_fGiaTriDeNghiTN sotien' align='right'>" + item.sDefaultValueTN + "</td>";
                    //dongMoi += "<td class='r_vontrongnuoc sotien' contenteditable='true' align='right'>" + item.sGiaTriTrongNuoc + "</td>";
                    dongMoi += "<td> <input class='r_vontrongnuoc sotien' style='border: none; background: none; height: 30px; text-align: right' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value=" + item.sGiaTriTrongNuoc + " > </td>"


                    dongMoi += "<td class='r_fGiaTriDeNghiNN sotien' align='right'>" + item.sDefaultValueNN + "</td>";
                    //dongMoi += "<td class='r_vonngoainuoc sotien' contenteditable='true' align='right'>" + item.sGiaTriNgoaiNuoc + "</td>";
                    dongMoi += "<td> <input class='r_vonngoainuoc sotien' style='border: none; background: none; height: 30px; text-align: right' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value=" + item.sGiaTriNgoaiNuoc + " > </td>"

                    dongMoi += "<td class='r_tongtien sotien' align='right'>" + item.sTongSo + "</td>";
                    dongMoi += "<td class='r_ghichu' contenteditable='true'>" + (item.sGhiChu != null ? item.sGhiChu : "") + "</td>";
                    dongMoi += "<td align='center'><button class='btn-delete btn-icon' type='button' onclick='XoaDong(this)'><span class='fa fa-trash-o fa-lg' aria-hidden='true'></span></button></td>";

                    html += dongMoi;
                })
                $("#" + TBL_DANH_SACH_THANH_TOAN_CHITIET + " tbody").append(html);
                CapNhatCotStt(TBL_DANH_SACH_THANH_TOAN_CHITIET);
                EventValidate();

                $("#" + TBL_DANH_SACH_THANH_TOAN_CHITIET + " tbody tr td.r_Loai select").trigger("change");
            }
        }
    });
}

function ResetThanhToanChiTiet() {
    $("#divBtnAdd").hide();
    $("#" + TBL_DANH_SACH_THANH_TOAN_CHITIET + " tbody").html("");
}

function GetItemData(id) {
    var dataCheck = $.map(tblDataGrid, function (n) { return n.iId == id ? n : null })[0];
    $("#bIsEdit").val("1");
    $("#drpNganhEdit").val(dataCheck.iID_NganhID)
    $("#drpDuAnEdit").val(dataCheck.iID_DuAnID);
    $("#drpHopDongEdit").val(dataCheck.iID_HopDongID);
    $("#txtThanhToanTrongNam").val(dataCheck.fGiaTriThanhToan);
    $("#txtTamUng").val(dataCheck.fGiaTriTamUng);
    $("#txtThuHoiTamUng").val(dataCheck.fGiaTriThuHoi);
    $("#drpLoaiKhoan").change();
}

function GetDataDropdownNguonVon() {
    var iIdDuAn = $("#drpDuAn").val();
    if (iIdDuAn == null || iIdDuAn == "" || iIdDuAn == GUID_EMPTY)
        return false;

    $.ajax({
        type: "POST",
        url: "/GiaiNganThanhToan/GetDataNguonVon",
        data: {
            iIdDuAn: iIdDuAn
        },
        success: function (data) {
            if (data != null && data != "") {
                $("#drpNguonNganSach").html(data);
                $("#drpNguonNganSach").val(iID_NguonVonIDOld).trigger("change");
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
        url: "/GiaiNganThanhToan/GetDataNhaThauEdit",
        data: {
            iIdHopDong: iIdHopDong,
            selectedNhaThau: iid_NhaThauOldId
        },
        success: function (data) {
            if (data != null && data != "") {
                $("#drpNhaThau").html(data);
                $("#drpNhaThau").trigger("change");
            }
        }
    });
}

function GetDataDropdownHopDong() {
    var iIdDuAn = $("#drpDuAn").val();
    if (iIdDuAn == null || iIdDuAn == "" || iIdDuAn == GUID_EMPTY)
        return false;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/GetDataDropdownHopDong",
        data: {
            iIdDuAn: $("#drpDuAn option:selected").val().split("|")[0]
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#drpHopDong").empty()
                $.each(r.data, function (index, value) {
                    if (value.iID_HopDongID == iID_HopDongIdOld) {
                        $("#drpHopDong").append("<option value='" + value.iID_HopDongID + "' selected>" + value.sSoHopDong + ' - ' + value.sTenHopDong + "</option>");
                        $('#drpHopDong').trigger('change');
                    }
                    else $("#drpHopDong").append("<option value='" + value.iID_HopDongID + "'>" + value.sSoHopDong + ' - ' + value.sTenHopDong + "</option>");
                });
            }
            if ($("#bIsEdit").val() == "1") {
                $("#drpHopDong").val(iID_HopDongIdOld).trigger("change");
            }
        }
    });
}

var iID_NhaThauID;
function GetDetailHopDong() {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/GetDetailHopDongDuAn",
        data: {
            iID_DuAnID: $("#drpDuAn option:selected").val().split("|")[0],
            iID_HopDongID: $("#drpHopDong option:selected").val(),
            dNgayDeNghi: $("#txtNgayPheDuyet").val(),
            iNamKeHoach: $("#txtNamKeHoach").val(),
            iID_NguonVonID: $("#drpNguonNganSach option:selected").val(),
            iID_NganhID: $("#drpNganh option:selected").val()
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#txtGiaTriHD").val(r.data.fGiaTriHD.toLocaleString('vi-VN'));
                $("#txtNgayHD").val(ConvertDatetimeToJSon(r.data.dNgayHopDong));
                $("#txtNganHang").val(r.data.sNganHang);
                $("#txtGoiThau").val(r.data.sTenGoiThau);
                $("#txtNhaThau").val(r.data.sTenNhaThau);
                $("#txtTaiKhoanNhaThau").val(r.data.sSoTaiKhoanNhaThau);
                $("#txtDaThanhToanTrongNam").val(r.data.fDaThanhToanTrongNam.toLocaleString('vi-VN'));
                $("#txtDaTamUng").val(r.data.fDaTamUng.toLocaleString('vi-VN'));
                $("#txtDaThuHoi").val(r.data.fDaThuHoi.toLocaleString('vi-VN'));
                $("#txtLuyKeThanhToan").val(r.data.fLuyKeThanhToanKLHT.toLocaleString('vi-VN'));
                iID_NhaThauID = r.data.iID_NhaThauThucHienID;
                $("#txtDuToanGoiThau").val(r.data.fDuToanGoiThau.toLocaleString('vi-VN'))
            }
        }
    });
}

var bLoaded = true;
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

function Update() {
    if (!ValidateDataInsert()) return;
    UpdateDeNghiThanhToan();
}

function UpdateDeNghiThanhToan() {
    var data = {};
    data.iID_DeNghiThanhToanID = iID_DeNghiThanhToanID;

    data.sSoDeNghi = $("#txtSoDeNghi").val();
    data.sSoBangKLHT = $("#txtSoCanCu").val();
    data.dNgayBangKLHT = $("#txtNgayCanCu").val();
    data.fLuyKeGiaTriNghiemThuKLHT = parseInt($("#txtLuyKeGiaTriKLNghiemThu").val() == "" ? 0 : UnFormatNumber($("#txtLuyKeGiaTriKLNghiemThu").val()));
    data.sNguoiLap = $("#txtNguoiLap").val();

    data.fGiaTriThanhToanTN = parseInt($("#txtdntuVonTrongNuoc").val() == "" ? 0 : UnFormatNumber($("#txtdntuVonTrongNuoc").val()));
    data.fGiaTriThanhToanNN = parseInt($("#txtdntuVonNgoaiNuoc").val() == "" ? 0 : UnFormatNumber($("#txtdntuVonNgoaiNuoc").val()));

    data.fGiaTriThuHoiTN = parseInt($("#txtthtuVonTrongNuoc").val() == "" ? 0 : UnFormatNumber($("#txtthtuVonTrongNuoc").val()));
    data.fGiaTriThuHoiNN = parseInt($("#txtthtuVonNgoaiNuoc").val() == "" ? 0 : UnFormatNumber($("#txtthtuVonNgoaiNuoc").val()));

    data.fGiaTriThuHoiUngTruocTN = parseInt($("#txtthtuUngTruocVonTrongNuoc").val() == "" ? 0 : UnFormatNumber($("#txtthtuUngTruocVonTrongNuoc").val()));
    data.fGiaTriThuHoiUngTruocNN = parseInt($("#txtthtuUngTruocVonNgoaiNuoc").val() == "" ? 0 : UnFormatNumber($("#txtthtuUngTruocVonNgoaiNuoc").val()));
    data.sGhiChu = $("#txtNoiDung").val();

    data.fThueGiaTriGiaTang = parseInt($("#txtthtuThueGTGT").val() == "" ? 0 : UnFormatNumber($("#txtthtuThueGTGT").val()));
    data.fChuyenTienBaoHanh = parseInt($("#txtthtuTienBaoHanh").val() == "" ? 0 : UnFormatNumber($("#txtthtuTienBaoHanh").val()));
    data.sThongTinCanCu = $('#sThongTinCanCu').val();
    data.loaiCoQuanTaiChinh = parseInt($('#loaiCoQuanTaiChinh').val());

    $.ajax({
        type: "POST",
        url: "/GiaiNganThanhToan/UpdateDeNghiThanhToan",
        data: {
            data: data
        },
        success: function (r) {
            if (r.bIsComplete && $('#txtNgayPheDuyet').val()) {
                SavePheDuyetThanhToanChiTiet();
            } else {
                alert("Cập nhật thanh toán thất bại.");
            }
        }
    });
}

function SavePheDuyetThanhToanChiTiet() {
    var data = GetListThanhToanChiTiet(iID_DeNghiThanhToanID);
    var dNgayPheDuyet = $('#txtNgayPheDuyet').val();
    var fThueGiaTriGiaTangDuocDuyet = $('#txtthtuThueGTGTDuocDuyet').val();
    var fChuyenTienBaoHanhDuocDuyet = $('#txtthtuTienBaoHanhDuocDuyet').val();

    $.ajax({
        type: "POST",
        url: "/GiaiNganThanhToan/UpdatePheDuyetThanhToanChiTiet",
        data: {
            lstData: data,
            iID_DeNghiThanhToanID: iID_DeNghiThanhToanID,
            dNgayPheDuyet,
            fThueGiaTriGiaTangDuocDuyet,
            fChuyenTienBaoHanhDuocDuyet
        },
        success: function (r) {
            if (r.bIsComplete) {
                alert("Cập nhật thành công phiếu thanh toán.");
                location.href = "/QLVonDauTu/GiaiNganThanhToan";
            }
            else {
                alert("Cập nhật phiếu thanh toán chi tiết thất bại.");
            }
        }
    });
}

function GetListThanhToanChiTiet(iID_DeNghiThanhToanID) {
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

        var fGiaTriNgoaiNuoc = parseInt((UnFormatNumber($(this).find(".r_vonngoainuoc").val() ?? '')) == "" ? 0 : UnFormatNumber($(this).find(".r_vonngoainuoc").val() ?? ''));
        var fGiaTriTrongNuoc = parseIntEmptyStr(UnFormatNumber($(this).find(".r_vontrongnuoc").val() ?? ''));
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
    //dongMoi += "<td class='r_vontrongnuoc sotien' contenteditable='true' align='right'></td>";
    dongMoi += "<td > <input class='r_vontrongnuoc sotien' style='border: none; background: none; height: 30px; text-align: right' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value='' /> </td>"
    dongMoi += "<td class='r_fGiaTriDeNghiNN sotien' align='right'>" + FormatNumber(fDefaultValueNN) + "</td>";
    //dongMoi += "<td class='r_vonngoainuoc sotien' contenteditable='true' align='right'></td>";
    dongMoi += "<td > <input class='r_vonngoainuoc sotien' style='border: none; background: none; height: 30px; text-align: right' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value='' /> </td>"
    dongMoi += "<td class='r_tongtien sotien' align='right'></td>";
    dongMoi += "<td class='r_ghichu' contenteditable='true'>";
    dongMoi += "<td align='center'><button class='btn-delete btn-icon' type='button' onclick='XoaDong(this)'><span class='fa fa-trash-o fa-lg' aria-hidden='true'></span></button></td>";
    $("#" + TBL_DANH_SACH_THANH_TOAN_CHITIET + " tbody").append(dongMoi);
    CapNhatCotStt(TBL_DANH_SACH_THANH_TOAN_CHITIET);

    EventValidate();

    $("#" + TBL_DANH_SACH_THANH_TOAN_CHITIET + " tbody tr:last-child td.r_Loai select").trigger("change");
}

function CreateHtmlSelectLoai(value) {
    var htmlOption = "";

    arrLoaiThanhToan.forEach(function (item) {
        // Nếu loại thanh toán = 'thanh toán' => hiện dropdown loại thanh toán nên cần map đúng giá trị
        // còn lại thì k hiện nên k cần map đúng, hiện tại item.iLoai = 1,4,5 nếu loại thanh toán = 'Thanh toán', 2 nếu loại thanh toán = 'Tạm ứng'
        if (iLoaiThanhToanOld == '1') {
            if (value != undefined && value != "" && item.ValueItem == value)
                htmlOption += "<option value='" + item.ValueItem + "' selected>" + item.DisplayItem + "</option>";
            else
                htmlOption += "<option value='" + item.ValueItem + "'>" + item.DisplayItem + "</option>";
        }
        else {
            if (value != undefined && value != "" && item.ValueItem == item)
                htmlOption += "<option value='" + item.ValueItem + "' selected>" + item.DisplayItem + "</option>";
            else
                htmlOption += "<option value='" + item.ValueItem + "'>" + item.DisplayItem + "</option>";
        }
    })
    return "<select class='form-control select_loaiThanhToan' onchange='onChangeLoaiThanhToan(this)'>" + htmlOption + "</option>";
}

function GetListDropdownPheDuyet() {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/LayDataDropdownPheDuyet",
        data: {
            iIdDeNghiThanhToanId: iID_DeNghiThanhToanID
        },
        async: false,
        success: function (r) {
            if (r.bStatus) {
                arrLoaiThanhToan = r.listLoaiThanhToan;
                arrKeHoachVonThanhToan = r.listKHVTT;
                arrMlns = JSON.parse(r.listMLNS);
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
    //$("td.sotien[contenteditable='true']").on("keypress", function (event) {
    // return ValidateNumberKeyPress(this, event);
    //})
    $("td .sotien").on("focusout", function (event) {

        // tinh tong so tien
        var thisDong = this.parentElement.parentElement;

        var fTrongNuoc = $(thisDong).find(".r_vontrongnuoc").val() ?? '';
        var fNuocNgoai = $(thisDong).find(".r_vonngoainuoc").val() ?? '';

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
        var sLNSValue = $(thisDong).find(".r_Lns").attr("data-value");
        var arrSelect = [];
        arrSelect.push({
            id: '',
            text: ''
        })
        lstMlns.forEach(function (item) {
            if (sLNSValue != null && sLNSValue != "" && sLNSValue == item.LNS) {
                arrSelect.push({
                    id: item.LNS,
                    text: item.LNS,
                    selected: true
                })
            } else {
                arrSelect.push({
                    id: item.LNS,
                    text: item.LNS
                })
            }
        })
        $(thisDong).find(".r_Lns select").select2({
            data: arrSelect
        })

        //$(thisDong).find(".r_Lns select").val(arrSelect.filter(x => x.selected)[0].id);

        $(thisDong).find(".r_Lns select").trigger("change");
    }
}

function onChangeLNS(obj) {
    var thisDong = $(obj).closest("tr");
    var iIdKeHoachVonId = $(thisDong).find(".r_KeHoachVon select").val();
    var sLNS = $(obj).val();
    var lstMlns = arrMlns[iIdKeHoachVonId];
    if (lstMlns != null && lstMlns.length > 0) {
        var sLValue = $(thisDong).find(".r_L").attr("data-value");
        var arrSelect = [];
        arrSelect.push({
            id: '',
            text: ''
        })
        lstMlns.forEach(function (item) {
            if (item.LNS == sLNS) {
                if (sLValue != null && sLValue != "" && sLValue == item.L) {
                    arrSelect.push({
                        id: item.L,
                        text: item.L,
                        selected: true
                    })
                } else {
                    arrSelect.push({
                        id: item.L,
                        text: item.L
                    })
                }

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
        var sKValue = $(thisDong).find(".r_K").attr("data-value");
        var arrSelect = [];
        arrSelect.push({
            id: '',
            text: ''
        })
        lstMlns.forEach(function (item) {
            if (item.LNS == sLNS && item.L == sL) {
                if (sKValue != null && sKValue != "" && sKValue == item.K) {
                    arrSelect.push({
                        id: item.K,
                        text: item.K,
                        selected: true
                    })
                }
                else {
                    arrSelect.push({
                        id: item.K,
                        text: item.K
                    })
                }
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
        var sMValue = $(thisDong).find(".r_M").attr("data-value");
        var arrSelect = [];
        arrSelect.push({
            id: '',
            text: ''
        })
        lstMlns.forEach(function (item) {
            if (item.LNS == sLNS && item.L == sL && item.K == sK) {
                if (sMValue != null && sMValue != "" && sMValue == item.M) {
                    arrSelect.push({
                        id: item.M,
                        text: item.M,
                        selected: true
                    })
                } else {
                    arrSelect.push({
                        id: item.M,
                        text: item.M
                    })
                }
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
        var sTmValue = $(thisDong).find(".r_Tm").attr("data-value");
        var arrSelect = [];
        arrSelect.push({
            id: '',
            text: ''
        })
        lstMlns.forEach(function (item) {
            if (item.LNS == sLNS && item.L == sL && item.K == sK && item.M == sM) {
                if (sTmValue != null && sTmValue != "" && sTmValue == item.TM) {
                    arrSelect.push({
                        id: item.TM,
                        text: item.TM,
                        selected: true
                    })
                } else {
                    arrSelect.push({
                        id: item.TM,
                        text: item.TM
                    })
                }
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
        var sTtmValue = $(thisDong).find(".r_Ttm").attr("data-value");
        var arrSelect = [];
        arrSelect.push({
            id: '',
            text: ''
        })
        lstMlns.forEach(function (item) {
            if (item.LNS == sLNS && item.L == sL && item.K == sK && item.M == sM && item.TM == sTM) {
                if (sTtmValue != null && sTtmValue != "" && sTtmValue == item.TTM) {
                    arrSelect.push({
                        id: item.TTM,
                        text: item.TTM,
                        selected: true
                    })
                } else {
                    arrSelect.push({
                        id: item.TTM,
                        text: item.TTM
                    })
                }
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
        var sNgValue = $(thisDong).find(".r_Ng").attr("data-value");
        var arrSelect = [];
        arrSelect.push({
            id: '',
            text: ''
        })
        lstMlns.forEach(function (item) {
            if (item.LNS == sLNS && item.L == sL && item.K == sK && item.M == sM && item.TM == sTM && item.TTM == sTTM) {
                if (sNgValue != null && sNgValue != "" && sNgValue == item.NG) {
                    arrSelect.push({
                        id: item.NG,
                        text: item.NG,
                        selected: true
                    })
                } else {
                    arrSelect.push({
                        id: item.NG,
                        text: item.NG
                    })
                }
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

function GetThongTinDeNghi() {
    var data = {};

    data.iID_DeNghiThanhToanID = iID_DeNghiThanhToanID;
    data.sSoDeNghi = $("#txtSoDeNghi").val();
    data.dNgayDeNghi = $("#txtNgayDeNghi").val();
    data.iID_DonViQuanLyID = $("#drpDonViQuanLy option:selected").val();
    data.sNguoiLap = $("#txtNguoiLap").val();
    data.iNamKeHoach = $("#txtNamKeHoach").val();
    data.iID_NguonVonID = $("#drpNguonNganSach option:selected").val();
    data.sSoBangKLHT = $("#txtSoCanCu").val();
    data.dNgayBangKLHT = $("#txtNgayCanCu").val();

    data.fLuyKeGiaTriNghiemThuKLHT = parseInt($("#txtLuyKeGiaTriKLNghiemThu").val() == "" ? 0 : UnFormatNumber($("#txtLuyKeGiaTriKLNghiemThu").val()));

    data.fGiaTriThanhToanTN = parseInt($("#txtdntuVonTrongNuoc").val() == "" ? 0 : UnFormatNumber($("#txtdntuVonTrongNuoc").val()));
    data.fGiaTriThanhToanNN = parseInt($("#txtdntuVonNgoaiNuoc").val() == "" ? 0 : UnFormatNumber($("#txtdntuVonNgoaiNuoc").val()));

    data.fGiaTriThuHoiTN = parseInt($("#txtthtuVonTrongNuoc").val() == "" ? 0 : UnFormatNumber($("#txtthtuVonTrongNuoc").val()));
    data.fGiaTriThuHoiNN = parseInt($("#txtthtuVonNgoaiNuoc").val() == "" ? 0 : UnFormatNumber($("#txtthtuVonNgoaiNuoc").val()));

    data.fGiaTriThuHoiUngTruocTN = parseInt($("#txtthtuUngTruocVonTrongNuoc").val() == "" ? 0 : UnFormatNumber($("#txtthtuUngTruocVonTrongNuoc").val()));
    data.fGiaTriThuHoiUngTruocNN = parseInt($("#txtthtuUngTruocVonNgoaiNuoc").val() == "" ? 0 : UnFormatNumber($("#txtthtuUngTruocVonNgoaiNuoc").val()));
    data.sGhiChu = $("#txtNoiDung").val();

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

    return data;
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

        $('#fTyLeThanhToan').val(tyLeThanhToan.toFixed(2));
    }
    catch (e) {
        console.error(e)
    }
}