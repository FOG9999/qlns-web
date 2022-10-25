var TBL_CHI_PHI_DAU_TU = "tblChiPhiDauTu";
var TBL_NGUON_VON_DAU_TU = "tblNguonVonDauTu";
var TBL_TAI_LIEU_DINH_KEM = "tblThongTinTaiLieuDinhKem";
var TBL_HANG_MUC_CHINH = "tblHangMucChinh";
var trLoaiCongTrinh;
var arrChiPhi = [];
var arr_NguonVon_Dropdown = [];
var arrNguonVon = [];
var arr_LoaiCongTrinh = [];
var arr_HangMuc = [];
var lstHangMucs = [];
var lstNguonVons = [];
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var tCpdtGiaTriToTrinh = 0, tCpdtGiaTriThamDinh = 0, tCpdtGiaTriPheDuyet = 0;
var tNvdtGiaTriToTrinh = 0, tNvdtGiaTriThamDinh = 0, tNvdtGiaTriPheDuyet = 0;
var ERROR = 1;
var CONFIRM = 0;
var bIsSaveSuccess = false;

$(document).ready(function ($) {
    //LoadDataLoaiCongTrinh();
    LoadDataComboBoxLoaiCongTrinh();
    LoadDataComboBoxNguonVon();
    GetCbxChuDauTu();
    //ReloadDuAn();
    Event();

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

    $("#dNgayQuyetDinh").keydown(function (event) {
        ValidateInputKeydown(event, this, 3);
    }).blur(function (event) {
        setTimeout(() => {
            if (!isShowing) ValidateInputFocusOut(event, this, 3);
        }, 0);
    });

    $("#iID_DonViQuanLyID").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#iID_ChuDauTuID").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#iID_NhomDuAnID").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#iID_CapPheDuyetID").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });

    $("#sKhoiCong").keydown(function (event) {
        ValidateInputKeydown(event, this, 1);
    }).blur(function (event) {
        ValidateInputFocusOut(event, this, 6);
    });

    $("#sHoanThanh").keydown(function (event) {
        ValidateInputKeydown(event, this, 1);
    }).blur(function (event) {
        ValidateInputFocusOut(event, this, 6);
    });

    $("#sSoQuyetDinh").keyup(function (event) {
        ValidateMaxLength(this, 100);
    });

    $("#sMoTa").keyup(function (event) {
        ValidateMaxLength(this, 2000);
    });

    $("#sDiaDiemDauTu").keyup(function (event) {
        ValidateMaxLength(this, 300);
    });

    $("#sMucTieuDauTu").keyup(function (event) {
        ValidateMaxLength(this, 2000);
    });

    $("#sQuyMo").keyup(function (event) {
        ValidateMaxLength(this, 2000);
    });

    $(".selectNguonVon .form-control").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });

    $(".selectLoaiCongTrinh select").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });


});

//function ReloadDuAn() {
//    $("#iID_DonViQuanLyID").on("change", function () {
//        $("#iID_DuAnID").empty();
//        var iID_DonViQuanLyID = $(this).val();
//        if (iID_DonViQuanLyID == undefined || iID_DonViQuanLyID == null || iID_DonViQuanLyID == "") return;
//        $.ajax({
//            type: "Get",
//            url: "/QLVonDauTu/ChuTruongDauTu/FindDuAn",
//            data: { iID_MaDonViQuanLyID: iID_DonViQuanLyID },
//            success: function (data) {
//                $("#iID_DuAnID").append(data.dataDuAn);
//                var duAnId = $("#iID_DuAnID").find(":selected").data("iid_duanid");
//                if (duAnId != undefined && duAnId != null)
//                    $("#iID_DuAnID").val(duAnId);
//            }
//        });
//    });
//}

//function GetCbxDuAn() {

//    var iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
//    $.ajax({
//        url: "/QLVonDauTu/ChuTruongDauTu/FindDuAn",
//        type: "GET",
//        dataType: "json",
//        data: { iID_DonViQuanLyID: iID_DonViQuanLyID }
//        success: function (result) {
//            if (result.dataDuAn != null && result.dataDuAn != "") {
//                $("#iID_DuAnID").append(result.dataDuAn);
//            }
//            var iID_DuAnID = $("#iID_DuAnID").val();

//            if (iID_MaCDTid != null && iID_MaCDTid != '') {
//                $("#iID_DuAnID").val(iID_DuAnID);
//                $("#iID_DuAnID").change();
//            }
//        }
//    });
//}
function LoadDataLoaiCongTrinh() {
    $.ajax({
        url: "/QLVonDauTu/ChuTruongDauTu/FillDropdown",
        type: "POST",
        data: "",
        dataType: "json",
        cache: false,
        success: function (data) {
            trLoaiCongTrinh = $('#sl_LoaiCongTrinhID').comboTree({
                source: data,
                isMultiple: false,
                cascadeSelect: false,
                collapse: true,
                selectableLastNode: false
            });

            trLoaiCongTrinh.onChange(function () {
                $("#iID_LoaiCongTrinhID").val(trLoaiCongTrinh.getSelectedIds());
            });
        },
        error: function (data) {

        }
    })
}

// Khi giá trị dropdown Dự án thay đổi
function GetValueChangeDuAn(value) {
    // Tự điền giá trị cho 1 số field ở phần Thông tin nội dung
    $.ajax({
        url: "/QLVonDauTu/ChuTruongDauTu/LayThongTinChiTietDuAn",
        type: "POST",
        data: { iID: value },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null) {
                // fill thong tin chi tiet du an
                if (data.sMaDuAn != null) {
                    $("#txt_MaDuAn").val(data.sMaDuAn);
                    //ReloadMaHangMucAuToWhenChangeDuAn(data.sMaDuAn);
                    LoadViewHangMuc();
                    //EventChangeSetValueAutoMaHangMuc(data.sMaDuAn);
                }
                if (data.iID_ChuDauTuID != null)
                    $("#iID_ChuDauTuID").find(":selected").data(data.iID_ChuDauTuID);

                $("#sDiaDiemDauTu").val(data.sDiaDiem);
                $("#sSuCanThietDauTu").val(data.sSuCanThietDauTu);
                $("#sMucTieu").val(data.sMucTieu);
                $("#sDienTichSuDungDat").val(data.sDienTichSuDungDat);
                $("#sNguonGocSuDungDat").val(data.sNguonGocSuDungDat);
                $("#sQuyMo").val(data.sQuyMo);
                $("#sKhoiCong").val(data.sKhoiCong);
                $("#sHoanThanh").val(data.sKetThuc);
                if (data.iID_NhomDuAnID != null)
                    $("#iID_NhomDuAnID").val(data.iID_NhomDuAnID);
                if (data.iID_HinhThucQuanLyID != null)
                    $("#iID_HinhThucQuanLyID").val(data.iID_HinhThucQuanLyID);
                if (data.iID_CapPheDuyetID != null)
                    $("#iID_CapPheDuyetID").val(data.iID_CapPheDuyetID);
                //if (data.iID_LoaiCongTrinhID != null) {
                //    trLoaiCongTrinh.setSelection([data.iID_LoaiCongTrinhID]);
                //}
            }
        }
    });

    // Tạo danh sách ở phần Nguồn vốn đầu tư
    $.ajax({
        url: "/QLVonDauTu/ChuTruongDauTu/LayDanhSachNguonVonTheoDuAnId",
        type: "POST",
        data: { iID: value },
        dataType: "json",
        cache: false,
        success: function (data) {
            $("#tblNguonVonDauTu tbody tr").remove();
            arrNguonVon = [];

            if (data != null) {
                for (var i = 0; i < data.length; i++) {
                    var dongMoi = "";
                    dongMoi += "<tr>";
                    dongMoi += "<td class='r_STT width-50'>" + (i + 1) + "</td>";
                    /*dongMoi += "<td style='text-align: left'>" + data[i].sTen + "</td>";*/
                    dongMoi += "<td><input type='hidden' class='r_iID_NguonVonID' value='" + data[i].iID_NguonVonID + "'/><div class='sTenNguonVon' hidden></div><div class='selectNguonVon'>" + CreateHtmlSelectNguonVon(data[i].iID_NguonVonID) + "</div></td>";
                    /*dongMoi += "<td style='text-align: right'>" + FormatNumber(data[i].TongHanMucDauTu) + "</td>";*/
                    dongMoi += "<td class='r_GiaTriNguonVon' ><div hidden></div><input type='text' style='text-align: right' class='form-control txtGiaTriNguonVon' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value='" + FormatNumber(data[i].TongHanMucDauTu) + "'/></td>"
                    dongMoi += "<td class='width-150' align='center'>";
                    dongMoi += "<button class='btn-save btn-icon' hidden type = 'button' onclick = 'CreateNguonVon(this, \"" + TBL_NGUON_VON_DAU_TU + "\")'> " +
                        "<i class='fa fa-floppy-o fa-lg' aria-hidden='true'></i>" +
                        "</button> ";
                    dongMoi += "<button class='btn-edit btn-icon' hidden type = 'button' onclick = 'EditNguonVon(this, \"" + TBL_NGUON_VON_DAU_TU + "\")'> " +
                        "<i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i>" +
                        "</button> ";
                    dongMoi += "<button class='btn-delete btn-icon' type = 'button' onclick='XoaDong(this, \"" + TBL_NGUON_VON_DAU_TU + "\")' > " +
                        "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span></button></td>";
                    dongMoi += "</tr>";

                    $("#tblNguonVonDauTu tbody").append(dongMoi);

                }
            }

            TinhLaiDongTong(TBL_NGUON_VON_DAU_TU);
        }
    });

    // Tạo danh sách ở phần Hạng mục
    $.ajax({
        url: "/QLVonDauTu/ChuTruongDauTu/LayDanhSachHangMucTheoDuAnId",
        type: "POST",
        data: { iID: value },
        dataType: "json",
        cache: false,
        success: function (data) {
            $("#tblHangMucChinh tbody tr").remove();
            arr_HangMuc = [];

            if (data != null) {
                for (var i = 0; i < data.length; i++) {
                    var dongMoi = "";
                    dongMoi += "<tr style='cursor: pointer;' class='parent'>";
                    dongMoi += "<td class='r_STT width-50'>" + data[i].smaOrder + "</td><input type='hidden' class='r_iID_DuAn_HangMucID' value='" + data[i].iID_DuAn_HangMucID + "'data-idDuAn_HangMuc='" + data[i].iID_DuAn_HangMucID + "'/> <input type='hidden' class='r_HangMucID' value='" + data[i].iID_DuAn_HangMucID + "'/> <input type='hidden' class='r_HangMucParentID' value='" + data[i].iID_ParentID + "'/> <input type = 'hidden' class='r_iID_ChuTruongDauTu_HangMucID' value = '" + data[i].iID_ChuTruongDauTu_HangMucID + "'/>" +
                        "<input type = 'hidden' class='r_iID_ChuTruongDauTuID' value = '" + data[i].iID_ChuTruongDauTuID + "' />";
                    dongMoi += "<td class='r_sTenHangMuc'><div class='sTenHangMuc' hidden></div><input type='text' class='form-control txtTenHangMuc' value='" + (!data[i].sTenHangMuc ? "" : data[i].sTenHangMuc) + "'/>"
                    dongMoi += "<td><input type='hidden' class='r_iID_LoaiCongTrinhID' value=''/><div class='sTenLoaiCongTrinh' hidden></div><div class='selectLoaiCongTrinh'>" + CreateHtmlSelectLoaiCongTrinh(data[i].iID_LoaiCongTrinhID) + "</div></td>";
                    dongMoi += "<td class='width-150' align='center'>";

                    dongMoi += "<button class='btn-add-child btn-icon' type = 'button' onclick = 'ThemMoiHangMucCon(this)' > " +
                        "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" +
                        "</button> ";

                    dongMoi += "<button class='btn-save btn-icon' hidden type = 'button' onclick = 'CreateHangMuc(this, \"" + TBL_HANG_MUC_CHINH + "\")' > " +
                        "<i class='fa fa-floppy-o fa-lg' aria-hidden='true'></i>" +
                        "</button> ";
                    dongMoi += "<button class='btn-edit btn-icon' hidden type = 'button' onclick = 'EditHangMuc(this, \"" + TBL_HANG_MUC_CHINH + "\")' > " +
                        "<i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i>" +
                        "</button> ";
                    dongMoi += "<button class='btn-delete btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_HANG_MUC_CHINH + "\")'>" +
                        "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
                        "</button></td>";
                    dongMoi += "</tr>";

                    $("#tblHangMucChinh tbody").append(dongMoi);
                }
            }
            CapNhatCotStt(TBL_HANG_MUC_CHINH);
            TinhLaiDongTong(TBL_HANG_MUC_CHINH);
            ShowHideButtonHangMuc();
        }
    });
}


function LoadDataComboBoxDuAn(maDonVi) {
    $('#iID_DuAnID').empty();
    $.ajax({
        url: "/QLVonDauTu/ChuTruongDauTu/GetDataComboBoxDuAn",
        type: "POST",
        data: { maDonVi: maDonVi },
        dataType: "json",
        success: function (resp) {
            if (resp.data == null || resp.data.length == 0) {
                $('#iID_DuAnID').empty();
            } else {
                $('#iID_DuAnID').select2({
                    data: resp.data
                });
            }
        }
    })
}

function Event() {
    $('#iID_DuAnID').select2({
        width: 'resolve',
        matcher: matchStart
    });

    $("#iID_DonViQuanLyID").change(function () {
        if (this.value != "") {
            LoadDataComboBoxDuAn(this.value);
        }
    });

    $("#iID_DuAnID").change(function () {
        if (this.value != "") {
            GetValueChangeDuAn(this.value);
        }
    });

    $("#txt_HangMucCha").select({
        width: 'resolve',
        matcher: matchStart
    });

    ResetDataComboBoxHangMucCha();
}

function ResetDataComboBoxHangMucCha() {
    $("#txt_HangMucCha").empty();
    $('#txt_HangMucCha').select2({
        data: GetListDataHangMucCha(),
        sorter: function (data) {
            return SortHangMucCha(data);
        }
    });
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

function CapNhatCotSttNguonVon(idBang) {
    $("#" + idBang + " tbody tr").each(function (index, tr) {
        $(tr).find('.r_STT').text(index + 1);
    });
}

function XoaDong(nutXoa, idBang) {
    var dongXoa = nutXoa.parentElement.parentElement;

    if (idBang == TBL_NGUON_VON_DAU_TU) {
        var rIIDNguonVon = $(dongXoa).find(".selectNguonVon select").val();
        arrNguonVon = arrNguonVon.filter(function (x) { return x.iID_NguonVonID != rIIDNguonVon });

        if ($(dongXoa).hasClass('error-row')) {
            $(dongXoa).removeClass('error-row');
        } else {
            $(dongXoa).addClass('error-row');
        }
    }
    else if (idBang == TBL_HANG_MUC_CHINH) {
        var ID_DuAn_HangMucID = $(dongXoa).find(".r_iID_DuAn_HangMucID")[0].value;
        arr_HangMuc = arr_HangMuc.filter(function (x) { return x.iID_DuAn_HangMucID != ID_DuAn_HangMucID });

        if ($(dongXoa).hasClass('error-row')) {
            $(dongXoa).removeClass('error-row');
        } else {
            $(dongXoa).addClass('error-row');
        }
        ShowHideButtonHangMuc();
    }
    // dongXoa.parentNode.removeChild(dongXoa);
    // CapNhatCotStt(idBang);
    TinhLaiDongTong(idBang);
}

function Huy() {
    window.location.href = "/QLVonDauTu/ChuTruongDauTu/Index";
}

function CheckTrungNguonVon(lstNguonVon) {
    var isCheck = false;
    for (var i = 0; i < lstNguonVon.length; i++) {
        if (lstNguonVon[i].isDelete == null || !lstNguonVon[i].isDelete) {
            if (lstNguonVon.filter(y => (y.isDelete == undefined || y.isDelete == false) && y.iID_NguonVonID == lstNguonVon[i].iID_NguonVonID).length > 1) {
                isCheck = true;
                break;
            }
        }
    }
    return isCheck;
}

function ValidateTongTienTheoTMDTPheDuyet(object) {
    var giaTriPheDuyet = object.fTMDTDuKienPheDuyet;
    var messErr = [];
    var items = object.ListNguonVon;
    if (object.sSoQuyetDinh == "") {
        messErr.push("Vui lòng nhập số quyết định");
    }

    if (object.dNgayQuyetDinh == "") {
        messErr.push("Vui lòng nhập ngày quyết định !");
    }

    if (object.iID_DuAnID == "") {
        messErr.push("Vui lòng chọn dự án !");
    }

    if (object.iID_ChuDauTuID == null || object.iID_ChuDauTuID == GUID_EMPTY) {
        messErr.push("Vui lòng chọn chủ đầu tư !");
    }

    if (object.iID_NhomDuAnID == GUID_EMPTY) {
        messErr.push("Vui lòng chọn nhóm dự án !");
    }
    if (object.sKhoiCong == "" || object.sHoanThanh == "") {
        messErr.push("Vui lòng nhập thời gian thực hiện !");
    }

    if (messErr.length > 0) {
        PopupModal("Lỗi lưu chủ trương đầu tư", messErr, ERROR);
        return false;
    }

    if (CheckTrungNguonVon(items)) {
        PopupModal("Lỗi lưu chủ trương đầu tư", ["Nguồn vốn đã tồn tại. Vui lòng chọn lại!"], ERROR);
        return false;
    }

    if (!arrHasValue(items) && !isStringEmpty(giaTriPheDuyet)) {
        PopupModal("Lỗi lưu chủ trương đầu tư", ["Giá trị của danh sách nguồn vốn và tổng mức đầu tư phê duyệt không bằng nhau !"], ERROR);
        return false;
    }

    var tongTien = 0;

    items.forEach(x => {
        if ((x.isDelete == undefined || x.isDelete == false) && !isStringEmpty(x.fTienPheDuyet)) {
            tongTien += parseFloat(x.fTienPheDuyet);
        }

    });

    if (tongTien < giaTriPheDuyet || tongTien > giaTriPheDuyet) {
        PopupModal("Lỗi lưu chủ trương đầu tư", ["Giá trị của danh sách nguồn vốn và tổng mức đầu tư phê duyệt không bằng nhau !"], ERROR);
        return false;
    }

    return true;
}

function GetDataFormChuTruongDauTu() {
    var soToTrinh = $.trim($("#sSoQuyetDinh").val());
    var ngayPheDuyet = $("#dNgayQuyetDinh").val();

    var sMa = $("#iID_DonViQuanLyID :selected").html();
    var sMaDonviQl = sMa.split('-')[0].trim();


    var iID_ChuDauTuID = $("#iID_ChuDauTuID").find(":selected").data("sid_cdt");
    var iID_MaChuDauTuID = $("#iID_ChuDauTuID").val();
    var iID_MaDonVi = $("#iID_ChuDauTuID").val();
    var iID_MaCDT = $("#iID_ChuDauTuID").val();

    var duAn = $("#iID_DuAnID").val();
    var chuDauTu = $("#iID_ChuDauTuID").val();
    var nhomDuAn = $("#iID_NhomDuAnID").val();
    var thoiGianThucHienTu = $("#sKhoiCong").val();
    var thoiGianThucHienDen = $("#sHoanThanh").val();
    var loaiCongTrinh = $("#iID_LoaiCongTrinhID").val();
    var phanCapPheDuyet = $("#iID_CapPheDuyetID").val();
    var tmdtPheDuyet = $("#fTMDTDuKienPheDuyet").val();
    var diaDiemDauTu = $.trim($("#sDiaDiemDauTu").val());
    var mucTieuDauTu = $.trim($("#sMucTieuDauTu").val());
    var quyMoDauTu = $.trim($("#sQuyMo").val());
    var moTa = $.trim($("#sMoTa").val());
    SetClassDeleteHangMuc();
    var lst_NguonVon = GetNguonVonByTable();
    CapNhatCotStt(TBL_HANG_MUC_CHINH);
    CapNhatCotSttCon(TBL_HANG_MUC_CHINH);
    var lst_HangMuc = GetHangMucByTable();


    return {
        sSoQuyetDinh: soToTrinh,
        dNgayQuyetDinh: ngayPheDuyet,
        iID_DuAnID: duAn,
        iID_MaDonViQuanLy: sMaDonviQl,
        iID_ChuDauTuID: chuDauTu,
        iID_NhomDuAnID: nhomDuAn,
        sKhoiCong: thoiGianThucHienTu,
        sHoanThanh: thoiGianThucHienDen,
        iID_LoaiCongTrinhID: loaiCongTrinh,
        iID_CapPheDuyetID: phanCapPheDuyet,
        fTMDTDuKienPheDuyet: UnFormatNumber(tmdtPheDuyet),
        sDiaDiem: diaDiemDauTu,
        sMucTieu: mucTieuDauTu,
        sQuyMo: quyMoDauTu,
        sMoTa: moTa,
        iID_ChuDauTuID: iID_ChuDauTuID,
        iID_MaChuDauTuID: iID_MaChuDauTuID,
        iID_MaCDT: iID_MaCDT,
        iID_MaDonVi: iID_MaDonVi,
        ListNguonVon: lst_NguonVon,
        ListHangMuc: lst_HangMuc
    };
}

function Luu() {
    var object = GetDataFormChuTruongDauTu();

    if (!ValidateTongTienTheoTMDTPheDuyet(object)) {
        SetClassDeleteHangMuc(true);
        return;
    }


    $.ajax({
        url: "/QLVonDauTu/ChuTruongDauTu/Save",
        type: "POST",
        data: { model: object },
        dataType: "json",
        cache: false,
        success: function (resp) {
            if (resp == null || resp.status == false) {
                PopupModal("Lỗi lưu dữ liệu chủ trương đầu tư", [resp.message], ERROR);
                return;
            }
            else {
                bIsSaveSuccess = true;
                UploadFile(resp.iID_ChuTruongDauTuID, 102);
                PopupModal("Thông báo", "Lưu dữ liệu thành công", ERROR);
            }
            /*window.location.href = "/QLVonDauTu/ChuTruongDauTu/Index";*/
        },
        error: function (data) {

        }
    })

}

function KiemTraTrungSoQuyetDinh(sSoQuyetDinh) {
    var check = false;
    $.ajax({
        url: "/QLVonDauTu/ChuTruongDauTu/KiemTraTrungSoQuyetDinh",
        type: "POST",
        data: { sSoQuyetDinh: sSoQuyetDinh },
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

function PopupModal(title, message, category) {
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: title, Messages: message, Category: category },
        success: function (data) {
            $("#divModalConfirm").html(data);
            $('#confirmModal').on('hidden.bs.modal', function () {
                if (bIsSaveSuccess) {
                    window.location.href = "/QLVonDauTu/ChuTruongDauTu/Index";
                }
            })
        }
    });
}

function isStringEmpty(x) {
    if (x == null || x == undefined || x == "") {
        return true;
    }

    return false;
}

function arrHasValue(x) {
    if (x == null || x == undefined || x.length <= 0) {
        return false;
    }

    return true;
}

function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

function ThemMoiNguonVon() {
    var dongMoi = "";
    dongMoi += "<tr class='new-row'>";
    dongMoi += "<td class='r_STT width-50'>" + "</td>";
    dongMoi += "<td><input type='hidden' class='r_iID_NguonVonID' /><div class='sTenNguonVon' hidden></div><div class='selectNguonVon'>" + CreateHtmlSelectNguonVon() + "</div></td>";
    dongMoi += "<td class='r_GiaTriNguonVon' ><div hidden></div><input type='text' onblur='TinhLaiDongTong(TBL_NGUON_VON_DAU_TU)' style='text-align: right' class='form-control txtGiaTriNguonVon' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);'/></td>"
    dongMoi += "<td class='width-150' align='center'>";
    dongMoi += "<button class='btn-save btn-icon' hidden type = 'button' onclick = 'CreateNguonVon(this, \"" + TBL_NGUON_VON_DAU_TU + "\")'> " +
        "<i class='fa fa-floppy-o fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-edit btn-icon' hidden type = 'button' onclick = 'EditNguonVon(this, \"" + TBL_NGUON_VON_DAU_TU + "\")'> " +
        "<i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-delete btn-icon' type = 'button' onclick='XoaDong(this, \"" + TBL_NGUON_VON_DAU_TU + "\")' > " +
        "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span></button></td>";
    dongMoi += "</tr>";

    $("#" + TBL_NGUON_VON_DAU_TU + " tbody").append(dongMoi);
    CapNhatCotSttNguonVon(TBL_NGUON_VON_DAU_TU);
    // TinhLaiDongTong(TBL_NGUON_VON_DAU_TU);

    $(".selectNguonVon .form-control").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
}

function ValidateBeforeSaveNguonVon(item) {
    var message = [];

    if (isStringEmpty(item)) {
        PopupModal("Lỗi thêm nguồn vốn", ["Dữ liệu nguồn vốn không hợp lê !"], ERROR);
        return false;
    }

    if (isStringEmpty(item.iID_NguonVonID)) {
        message.push("Vui lòng chọn loại nguồn vốn !");
    }

    if (isStringEmpty(item.fTienPheDuyet)) {
        message.push("Vui lòng nhập giá trị phê duyệt nguồn vốn !");
    }
    else {
        if (item.fTienPheDuyet <= 0) {
            message.push("Giá trị phê duyệt nguồn vốn phải lớn hơn 0 !");
        }
    }

    for (var i = 0; i < arrNguonVon.length; i++) {
        if (arrNguonVon[i].sTenNguonVon == item.sTenNguonVon) {
            message.push("Nguồn vốn đã tồn tại");
            break;
        }
    }

    if (message.length > 0) {
        PopupModal("Lỗi thêm nguồn vốn", message, ERROR);
        return false;
    }

    return true;
}

function GetListDataNguonVon() {
    return arrNguonVon;
}

function LoadViewNguonVon() {
    var html = "";
    var htmlTongTien = "";
    var items = GetListDataNguonVon();

    if (!arrHasValue(items)) {
        htmlTongTien += '<tr>';
        htmlTongTien += '<th></th>';
        htmlTongTien += '<th>Tổng cộng</th>';
        htmlTongTien += '<th class="cpdt_tong_giatripheduyet text-right"></th>';
        htmlTongTien += '<th></th>';
        htmlTongTien += '</tr>';

        $("#tblNguonVonDauTu tbody").html(html);
        $("#tblNguonVonDauTu tfoot").html(htmlTongTien);
        return;
    }

    $("#tblNguonVonDauTu tbody").html(html);
    $("#tblNguonVonDauTu tfoot").html(htmlTongTien);

    var count = 1;
    items.forEach(x => {
        html += "<tr>";
        html += '<td>' + count + '</td>';
        html += '<td style="text-align:left">' + x.sTenNguonVon + '</td>';
        html += '<td style="text-align:right">' + FormatNumber(x.fTienPheDuyet) + '</td>';
        html += "<td>";
        html += "<button style='margin-right:5px' class='btn-edit btn-icon' type='button' onclick='EditNguonVon(\"" + x.Id + "\")'>" +
            "<span class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></span>" +
            "</button>";
        html += "<button class='btn-delete btn-icon' type='button' onclick='ComfirmDeleteNguonVon(\"" + x.Id + "\")'>" +
            "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
            "</button>";
        html += "</td>";

        html += '</tr>';

        count++;
    });

    htmlTongTien += '<tr>';
    htmlTongTien += '<th></th>';
    htmlTongTien += '<th style="text-align:center">Tổng cộng</th>';
    htmlTongTien += '<th style="text-align:right">' + FormatNumber(TinhTong("PHE_DUYET")) + '</th>';
    htmlTongTien += '<th></th>';
    htmlTongTien += '</tr>';

    $("#tblNguonVonDauTu tbody").append(html);
    $("#tblNguonVonDauTu tfoot").html(htmlTongTien);
    TinhLaiDongTong(TBL_NGUON_VON_DAU_TU);
}

function TinhTong(type) {
    var result = 0;
    var items = GetListDataNguonVon();

    if (!arrHasValue(items)) {
        return result;
    }

    items.forEach(x => {
        if (type == "TO_TRINH") {
            if (!isStringEmpty(x.fTienToTrinh)) {
                result += x.fTienToTrinh;
            }
        }

        if (type == "THAM_DINH") {
            if (!isStringEmpty(x.fTienThamDinh)) {
                result += x.fTienThamDinh;
            }
        }

        if (type == "PHE_DUYET") {
            if (!isStringEmpty(x.fTienPheDuyet)) {
                result += x.fTienPheDuyet;
            }
        }
    });

    return result;
}

function DeleteNguonVon(id) {
    if (isStringEmpty(id)) {
        return;
    }

    var items = GetListDataNguonVon();
    if (!arrHasValue(items)) {
        return;
    }

    var result = [];
    items.forEach(x => {
        if (x.Id != id) {
            result.push(x);
        }
    });
    arrNguonVon = result;

    ResetDataFormNguonVon();

    LoadViewNguonVon();
}


function ComfirmDeleteNguonVon(id) {
    if (confirm("Bạn có chắc chắn muốn xóa không ?")) {
        DeleteNguonVon(id);
    }
}

// End nguồn vốn

// Hạng mục
function LoadDataComboBoxLoaiCongTrinh() {
    $.ajax({
        url: "/QLVonDauTu/QLDuAn/GetLoaiCongTrinh",
        type: "POST",
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data.status == false) {
                return;
            }

            if (data.data != null)
                arr_LoaiCongTrinh = data.data;
        }
    });
}

function CreateHtmlSelectLoaiCongTrinh(value) {
    var htmlOption = "";
    arr_LoaiCongTrinh.forEach(x => {
        if (value != undefined && value == x.id)
            htmlOption += "<option value='" + x.id + "' selected>" + x.text + "</option>";
        else
            htmlOption += "<option value='" + x.id + "'>" + x.text + "</option>";
    })
    return "<select class='form-control'>" + htmlOption + "</option>";
}

async function LoadDataComboBoxNguonVon() {
    $.ajax({
        url: "/QLVonDauTu/ChuTruongDauTu/LoadComboboxNguonVon",
        type: "POST",
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data.status == false) {
                return;
            }

            if (data.data != null)
                arr_NguonVon_Dropdown = data.data;
        }
    });
}

function CreateHtmlSelectNguonVon(value) {
    var htmlOption = "";
    arr_NguonVon_Dropdown.forEach(x => {
        if (value != undefined && value == x.id)
            htmlOption += "<option value='" + x.id + "' selected>" + x.text + "</option>";
        else
            htmlOption += "<option value='" + x.id + "'>" + x.text + "</option>";
    })
    return "<select class='form-control' id='idnguonvonchild'>" + htmlOption + "</option>";
}


function CreateNguonVon(nutCreate, idBang) {
    var messErr = [];
    var dongHienTai = nutCreate.parentElement.parentElement;

    //var iID_DuAnID = $(dongHienTai).find(".r_iID_DuAn_HangMucID").val();
    var iID_NguonVonID = $(dongHienTai).find(".selectNguonVon select").val();
    var id = $(dongHienTai).find(".r_STT").html();

    var sHanMucDauTu = $(dongHienTai).find(".txtGiaTriNguonVon").val();
    if (sHanMucDauTu == "") {
        messErr.push("Giá trị phê duyệt phải lớn hơn 0");
    }
    var TongHanMucDauTu = parseInt(UnFormatNumber(sHanMucDauTu));

    if (arrNguonVon.filter(function (x) { return x.iID_NguonVonID == iID_NguonVonID && x.id != id }).length > 0) {
        messErr.push("Nguồn vốn đã tồn tại !");
    }

    if (messErr.length > 0) {
        PopupModal("Lỗi", messErr, ERROR);
        return;
    }

    arrNguonVon = arrNguonVon.filter(function (x) { return x.id != id });

    $(dongHienTai).find(".selectNguonVon select").attr("disabled", true);
    $(dongHienTai).find(".txtGiaTriNguonVon").attr("disabled", true);

    $(dongHienTai).find("button.btn-edit").show();
    $(dongHienTai).find("button.btn-save").hide();

    arrNguonVon.push({
        id: id,
        iID_NguonVonID: iID_NguonVonID,
        fTienPheDuyet: TongHanMucDauTu,
        isDelete: false
    })

    TinhLaiDongTong(idBang);

}

function EditNguonVon(nutCreateHangMuc, idBang) {
    var dongHienTai = nutCreateHangMuc.parentElement.parentElement;
    $(dongHienTai).find(".selectNguonVon select").attr("disabled", false);
    $(dongHienTai).find(".txtGiaTriNguonVon").attr("disabled", false);

    $(dongHienTai).find("button.btn-edit").hide();
    $(dongHienTai).find("button.btn-save").show();
}

function ThemMoiHangMuc() {
    //var hangMucIdNew = createGuid();
    var dongMoi = "";
    dongMoi += "<tr style='cursor: pointer;' class='parent'>";
    dongMoi += "<td class='r_STT width-50'></td><input type='hidden' class='r_iID_DuAn_HangMucID' value='" + uuidv4() + "'/>";
    dongMoi += "<td class='r_sTenHangMuc'><div class='sTenHangMuc' hidden></div><input type='text' class='form-control txtTenHangMuc'/></td>"
    dongMoi += "<td><input type='hidden' class='r_iID_LoaiCongTrinhID' value=''/><div class='sTenLoaiCongTrinh' hidden></div><div class='selectLoaiCongTrinh'>" + CreateHtmlSelectLoaiCongTrinh() + "</div></td>";
    dongMoi += "<td class='width-150' align='center'>";

    dongMoi += "<button class='btn-add-child btn-icon' type = 'button' onclick = 'ThemMoiHangMucCon(this)' > " +
        "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" +
        "</button> ";

    dongMoi += "<button class='btn-save btn-icon' hidden type = 'button' onclick = 'CreateHangMuc(this, \"" + TBL_HANG_MUC_CHINH + "\")' > " +
        "<i class='fa fa-floppy-o fa-lg' aria-hidden='false'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-edit btn-icon' hidden type = 'button' onclick = 'EditHangMuc(this, \"" + TBL_HANG_MUC_CHINH + "\")' > " +
        "<i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-delete btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_HANG_MUC_CHINH + "\")'>" +
        "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
        "</button></td>";
    dongMoi += "</tr>";

    $("#" + TBL_HANG_MUC_CHINH + " tbody").append(dongMoi);
    //CapNhatCotStt(TBL_HANG_MUC_CHINH);
    CapNhatCotSttHangMuc();
    TinhLaiDongTong(TBL_NGUON_VON_DAU_TU);
    ShowHideButtonHangMuc();
    if (arr_STT.length > 0) {
        var indexMax = arr_STT.sort().pop();
        $("#tblHangMucChinh tbody tr.parent:last").find(".r_STT").text(indexMax + 1);
    }

    $(".txtTenHangMuc").keyup(function (event) {
        ValidateMaxLength(this, 300);
    });
    $(".selectLoaiCongTrinh select").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    TrimSpaceAllTextInput();
}

function ThemMoiHangMucCon(nutThem) {
    var dongHienTai = nutThem.parentElement.parentElement;
    var iIDDuAnHangMucID = $(dongHienTai).find(".r_iID_DuAn_HangMucID").val();
    res = [];
    var rowChild = FindLastChildHangMuc(iIDDuAnHangMucID);
    var sttParent = $(dongHienTai).find(".r_STT").text();

    var dongMoi = "";
    dongMoi += "<tr style='cursor: pointer;' class='child' data-idparent='" + iIDDuAnHangMucID + "'>";
    dongMoi += "<td class='r_STT width-50'></td><input type='hidden' class='r_iID_DuAn_HangMucID' value='" + uuidv4() + "'/> <input type='hidden' class='r_HangMucParentID' value='" + iIDDuAnHangMucID + "'/>";
    dongMoi += "<td class='r_sTenHangMuc'><div class='sTenHangMuc' hidden></div><input type='text' class='form-control txtTenHangMuc'/></td>"
    dongMoi += "<td><input type='hidden' class='r_iID_LoaiCongTrinhID' value=''/><div class='sTenLoaiCongTrinh' hidden></div><div class='selectLoaiCongTrinh'>" + CreateHtmlSelectLoaiCongTrinh() + "</div></td>";
    dongMoi += "<td class='width-150' align='center'>";

    dongMoi += "<button class='btn-add-child btn-icon'  type = 'button' onclick = 'ThemMoiHangMucCon(this)' > " +
        "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" +
        "</button> ";

    dongMoi += "<button class='btn-save btn-icon' hidden type = 'button' onclick = 'CreateHangMuc(this, \"" + TBL_HANG_MUC_CHINH + "\")' > " +
        "<i class='fa fa-floppy-o fa-lg' aria-hidden='false'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-edit btn-icon'  hidden type = 'button' onclick = 'EditHangMuc(this, \"" + TBL_HANG_MUC_CHINH + "\")' > " +
        "<i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-delete btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_HANG_MUC_CHINH + "\")'>" +
        "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
        "</button></td>";
    dongMoi += "</tr>";

    if (rowChild != undefined) {
        var a = rowChild.closest("tr");
        $(a).after(dongMoi);
    } else {
        $(dongHienTai).after(dongMoi);
    }

    arr_STT_Child = [];
    //CapNhatCotSttCon(TBL_HANG_MUC_CHINH);
    CapNhatCotSttHangMucCon(iIDDuAnHangMucID, sttParent);
    ShowHideButtonHangMuc();
    var sttHangMucNew = "";
    if (arr_STT_Child.length > 0) {
        var indexMax = arr_STT_Child.sort().pop();
        sttHangMucNew = sttParent + "-" + (indexMax + 1).toString();
    } else {
        sttHangMucNew = sttParent + "-" + "1";
    }
    rowChild.closest("tr").next("tr").find('.r_STT').text(sttHangMucNew);
    $(".selectLoaiCongTrinh select").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    TrimSpaceAllTextInput();
}

var res = [];
var arr_hangMucUpdate = [];
var findChildren = function (id) {
    arr_hangMucUpdate.forEach(obj => {
        if (obj.iID_ParentID === id) {
            res.push(obj);
            findChildren(obj.iID_DuAn_HangMucID)
        }
    })
}

function UpdateArrayHangMuc() {
    arr_hangMucUpdate = [];
    $("#" + TBL_HANG_MUC_CHINH + " tbody tr").each(function (index, tr) {
        var iIDParentID = $(tr).find(".r_HangMucParentID").val();
        var iIDDuAnHangMucID = $(tr).find(".r_iID_DuAn_HangMucID").val();

        arr_hangMucUpdate.push({
            iID_DuAn_HangMucID: iIDDuAnHangMucID,
            iID_ParentID: iIDParentID

        })
    });
}

function FindLastChildHangMuc(parentId) {
    var hangMucLast = "";
    var row;
    if (parentId != undefined) {
        UpdateArrayHangMuc();
        findChildren(parentId);
        if (res.length > 0) {
            hangMucLast = res[res.length - 1].iID_DuAn_HangMucID;
        } else {
            hangMucLast = parentId;
        }
        // tìm dòng con cuối cùng trong bảng
        $("#" + TBL_HANG_MUC_CHINH + " .r_iID_DuAn_HangMucID").each(function () {
            var hangMucId = $(this).val();
            if (hangMucId == hangMucLast && hangMucId != "") {
                row = $(this);
                return row;
                //return row.closest("tr");
            }
        });
    }
    return row;

}

var arr_STT = [];
function CapNhatCotSttHangMuc() {
    $("#" + TBL_HANG_MUC_CHINH + " tbody tr.parent").each(function (index, tr) {
        var sttHangMuc = $(tr).find('.r_STT').text();
        if (sttHangMuc != "" && sttHangMuc != undefined && !sttHangMuc.includes("-")) {
            arr_STT.push(parseInt(sttHangMuc));
        } else {
            arr_STT.push(0);
        }
    });

}

var arr_STT_Child = [];
function CapNhatCotSttHangMucCon(parentId, sttParent) {
    $("#" + TBL_HANG_MUC_CHINH + " tbody tr").each(function (index, tr) {

        var iIDParentID = $(tr).find('.r_HangMucParentID').val();
        if (iIDParentID == parentId) {
            var sttChild = $(tr).find('.r_STT').text();
            if (sttChild != undefined && sttChild != "") {
                var indexChild = parseInt(sttChild.substring(sttParent.length + 1));
                arr_STT_Child.push(indexChild);
            }
        }
    });

}

function CapNhatCotStt(idBang) {
    var count = 0;
    $("#" + idBang + " tbody tr.parent").each(function (index, tr) {
        if (!$(tr).hasClass("error-row")) {
            $(tr).find('.r_STT').text(count + 1);
            count++;
        }
    });
}

function CapNhatCotSttCon(idBang) {
    $("#" + idBang + " tbody tr").each(function (index, tr) {
        var sttParent = $(tr).find('.r_STT').text();
        var iIDParentID = $(tr).find('.r_iID_DuAn_HangMucID').val();
        var count = 0;
        $("#" + idBang + " tbody tr.child[data-idparent='" + iIDParentID + "']").each(function (index, tr) {
            if (!$(tr).hasClass("error-row")) {
                $(tr).find('.r_STT').text(sttParent + "-" + (count + 1));
                count++;
            }
        });
    });
}


function CheckMaHangMuc(item) {
    var items = GetListDataHangMuc();

    if (!arrHasValue(items)) {
        return true;
    }

    var message = [];

    items.forEach(x => {
        if (isStringEmpty(item.Id)) {
            if (item.sMaHangMuc == x.sMaHangMuc) {
                message.push("Mã hạng mục " + x.sMaHangMuc + " đã tồn tại !");
            }
        } else {
            if (x.Id != item.Id && x.sMaHangMuc == item.sMaHangMuc) {
                message.push("Mã hạng mục " + x.sMaHangMuc + " đã tồn tại !");
            }
        }
    });

    if (message.length > 0) {
        PopupModal("Lỗi lưu hạng mục", message, ERROR);
        return false;
    }

    return true;
}

function CheckMaHangMucCha(sMaHangMuc) {
    if (isStringEmpty(sMaHangMuc)) {
        return true;
    }

    var items = GetListDataHangMuc();

    var message = [];

    if (!arrHasValue(items)) {
        message.push("Không tồn tại mã hạng mục " + sMaHangMuc + " !");
        PopupModal("Lỗi lưu hạng mục", message, ERROR);
        return false;
    }

    var isCheck = false;
    items.forEach(x => {
        if (x.sMaHangMuc == sMaHangMuc) {
            isCheck = true;
        }
    });

    if (!isCheck) {
        message.push("Không tồn tại mã hạng mục " + sMaHangMuc + " !");
        PopupModal("Lỗi lưu hạng mục", message, ERROR);
        return false;
    }

    return true;
}

function GetListDataHangMuc() {
    return arr_HangMuc;
}

function EditHangMuc(nutEdit, idBang) {
    var dongHienTai = nutEdit.parentElement.parentElement;
    var messErr = [];
    var iIDDuAnHangMucID = $(dongHienTai).find(".r_iID_DuAn_HangMucID").val();
    var sTenHangMuc = $(dongHienTai).find(".txtTenHangMuc").val();
    var iIDLoaiCongTrinhID = $(dongHienTai).find(".selectLoaiCongTrinh select").val();
    var sTenLoaiCongTrinh = $(dongHienTai).find(".selectLoaiCongTrinh select :selected").html();

    $(dongHienTai).find(".sTenHangMuc").hide();
    $(dongHienTai).find(".txtTenHangMuc").show();

    $(dongHienTai).find(".sTenLoaiCongTrinh").hide();
    $(dongHienTai).find(".selectLoaiCongTrinh").html(CreateHtmlSelectLoaiCongTrinh(iIDLoaiCongTrinhID));
    $(dongHienTai).find(".selectLoaiCongTrinh select").show();

    $(dongHienTai).find("button.btn-edit").hide();
    $(dongHienTai).find("button.btn-save").show();
    $(dongHienTai).find("button.btn-add-child").hide();
}


function CreateHangMuc(nutCreateHangMuc, idBang) {
    var dongHienTai = nutCreateHangMuc.parentElement.parentElement;
    var messErr = [];
    var iIDDuAnHangMucID = $(dongHienTai).find(".r_iID_DuAn_HangMucID").val();
    var iIDParentID = $(dongHienTai).data("idparent");
    var sTenHangMuc = $(dongHienTai).find(".txtTenHangMuc").val();
    var iIDLoaiCongTrinhID = $(dongHienTai).find(".selectLoaiCongTrinh select").val();
    var sTenLoaiCongTrinh = $(dongHienTai).find(".selectLoaiCongTrinh select :selected").html();
    var smaOrder = $(dongHienTai).find(".r_STT").html();


    if (iIDDuAnHangMucID == "")
        iIDDuAnHangMucID = uuidv4();

    if (sTenHangMuc == "") {
        messErr.push("Tên hạng mục chưa có hoặc chưa chính xác.");
    }

    if (arr_HangMuc.filter(function (x) { return x.sTenHangMuc == sTenHangMuc && x.iID_DuAn_HangMucID != iIDDuAnHangMucID }).length > 0) {
        messErr.push("Tên hạng mục đã tồn tại.");
    }

    if (iIDLoaiCongTrinhID == 0 || iIDLoaiCongTrinhID == "") {
        messErr.push("Thông tin loại công trình chưa có hoặc chưa chính xác.");
    }

    if (messErr.length > 0) {
        PopupModal("Lỗi", messErr, ERROR);
        return;
    }

    $(dongHienTai).find(".r_iID_DuAn_HangMucID").val(iIDDuAnHangMucID);

    $(dongHienTai).find(".sTenHangMuc").html(sTenHangMuc);
    $(dongHienTai).find(".sTenHangMuc").show();
    $(dongHienTai).find(".txtTenHangMuc").hide();

    $(dongHienTai).find(".r_iID_LoaiCongTrinhID").val(iIDLoaiCongTrinhID);
    $(dongHienTai).find(".sTenLoaiCongTrinh").html(sTenLoaiCongTrinh);
    $(dongHienTai).find(".sTenLoaiCongTrinh").show();
    $(dongHienTai).find(".selectLoaiCongTrinh select").hide();

    $(dongHienTai).find("button.btn-edit").show();
    $(dongHienTai).find("button.btn-save").hide();
    $(dongHienTai).find("button.btn-add-child").show();

    arr_HangMuc = arr_HangMuc.filter(function (x) { return x.iID_DuAn_HangMucID != iIDDuAnHangMucID });

    arr_HangMuc.push({
        iID_DuAn_HangMucID: iIDDuAnHangMucID,
        iID_ParentID: iIDParentID,
        sTenHangMuc: sTenHangMuc,
        iID_LoaiCongTrinhID: iIDLoaiCongTrinhID,
        sTenLoaiCongTrinh: sTenLoaiCongTrinh,
        smaOrder: smaOrder,
        isDelete: false
    })

    TinhLaiDongTong(idBang);
}

//function TinhLainguonvon(idBang) {
//    var fTong = 0;
//    if (idBang == TBL_NGUON_VON_DAU_TU) {
//        $.each($("#tblNguonVonDauTu tbody .txtGiaTriNguonVon"), function (index, item) {
//            if (!$(item).hasClass('error-row')) {
//                var sItem = $(item).val().replaceAll(".", "");
//                if (sItem == "") return false;
//                fTong += parseInt(UnFormatNumber($(item).val()));
//                $("#fHanMucDauTu").val(FormatNumber(fTong));
//                $("#fTMDTDuKienPheDuyet").val(FormatNumber(fTong));
//            }
//        });
//    }
//    $("#" + idBang + " .cpdt_tong_giatripheduyet").html(FormatNumber(fTong));
//}

function TinhLaiDongTong(idBang) {
    var result = 0;
    var lstNguonVon = GetNguonVonByTable();
    if (arrHasValue(lstNguonVon)) {
        lstNguonVon.forEach(x => {
            if ((x.isDelete == undefined || x.isDelete == false) && !isStringEmpty(x.fTienPheDuyet)) {

                result += parseFloat(x.fTienPheDuyet);
                $("#fHanMucDauTu").val(FormatNumber(result));
                $("#fTMDTDuKienPheDuyet").val(FormatNumber(result));
            }
        });
    }
    $("#" + idBang + " .cpdt_tong_giatripheduyet").html(FormatNumber(result));
}

function LoadViewHangMuc() {
    var items = GetListDataHangMuc();

    var html = "";
    if (!arrHasValue(items)) {
        $("#tblDuAnHangMuc tbody").html(html);
        return;
    }

    $("#tblDuAnHangMuc tbody").html(html);

    var listParent = [];

    items.forEach(x => {
        if (isStringEmpty(x.sHangMucCha)) {
            listParent.push(x);
        }
    });

    let count = 1;
    listParent.forEach(x => {
        var isCheckParent = items.find(k => k.sHangMucCha == x.sMaHangMuc);
        if (isCheckParent == null || isCheckParent == undefined) {
            html += '<tr>';
        } else {
            html += '<tr style="font-weight:bold">';
        }
        var stt = count;
        html += '<td>' + stt + '</td>';
        html += '<td>' + x.sMaHangMuc + '</td>';
        html += '<td>' + x.sTenHangMuc + '</td>';
        html += '<td>' + x.sHangMucCha + '</td>';
        html += "<td>";
        html += "<button style='margin-right:5px' class='btn-edit btn-icon' hidden type='button' onclick='EditHangMuc(\"" + x.Id + "\")'>" +
            "<i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i>" +
            "</button>";
        html += "<button class='btn-delete btn-icon' type='button' onclick='ComfirmDeleteHangMuc(\"" + x.Id + "\")'>" +
            "<i class='fa fa-trash-o fa-lg' aria-hidden='true'></i>" +
            "</button>";
        html += "</td>";
        html += '</tr>';
        count++;
        //html += LoadViewHangMucTree(stt, items, x.sMaHangMuc);
    });

    $("#tblDuAnHangMuc tbody").append(html);
    ShowHideButtonHangMuc();
}


function ValidateHangMucBeforeSave(item) {
    var message = [];

    if (isStringEmpty(item.sMaHangMuc)) {
        message.push("Vui lòng nhập mã hạng mục !");
    }

    if (isStringEmpty(item.sTenHangMuc)) {
        message.push("Vui lòng nhập tên hàng mục !");
    }

    if (!isStringEmpty(item.sHangMucCha)) {
        if (item.sMaHangMuc == item.sHangMucCha) {
            message.push("Mã hạng mục " + item.sMaHangMuc + " không thể chọn hạng mục cha là mã hạng mục " + item.sHangMucCha);
        }
    }

    if (message.length > 0) {
        PopupModal("Lỗi thêm hàng mục", message, ERROR);
        return false;
    }

    return true;
}

function GetDataFormHangMuc() {
    var id = $("#txt_IdHangMuc").val();
    var maHangMuc = $("#txt_MaHangMuc").val();
    var tenHangMuc = $("#txt_TenHangMuc").val();
    var hangMucCha = $("#txt_HangMucCha").val();
    var tenHangMucCha = $("#txt_HangMucCha :selected").html();

    if (isStringEmpty(hangMucCha)) {
        hangMucCha = "";
        tenHangMucCha = "";
    }

    return {
        Id: id,
        sMaHangMuc: maHangMuc,
        sTenHangMuc: tenHangMuc,
        sHangMucCha: hangMucCha,
        sTenHangMucCha: tenHangMucCha
    };
}

function GetListDataHangMucCha() {
    return arr_HangMucCha;
}

var arr_HangMucCha = [
    {
        id: "",
        text: "--Chọn--"
    }
];

// End Hạng mục
function GetCbxChuDauTu() {
    $.ajax({
        url: "/QLVonDauTu/ChuTruongDauTu/GetCbxChuTruongDauTu",
        type: "GET",
        dataType: "json",
        success: function (result) {
            if (result.data != null && result.data != "") {
                $("#iID_ChuDauTuID").append(result.data);
            }
            var iID_MaCDTid = $("#iID_MaCDT").val();

            if (iID_MaCDTid != null && iID_MaCDTid != '') {
                $("#iID_ChuDauTuID").val(iID_MaCDTid);
                $("#iID_ChuDauTuID").change();
            }
        }
    });
}


function GetNguonVonByTable() {
    var lst_NguonVon = [];

    $.each($("#tblNguonVonDauTu tbody tr"), function (index, item) {
        var obj = {};
        var bIsDelete = $(this).hasClass("error-row");
        obj.id = $(item).find(".r_STT").html();
        obj.iID_NguonVonID = $(item).find(".selectNguonVon select").val();
        obj.fTienPheDuyet = $(item).find(".txtGiaTriNguonVon").val();
        obj.isDelete = bIsDelete;
        if (obj.fTienPheDuyet == null || obj.fTienPheDuyet == "") {
            obj.fTienPheDuyet = 0;
        } else {
            obj.fTienPheDuyet = parseFloat(obj.fTienPheDuyet.toString().replaceAll(".", ""));
        }
        lst_NguonVon.push(obj);
    });
    return lst_NguonVon;
}

function GetHangMucByTable() {
    var lst_HangMuc = [];

    $.each($("#tblHangMucChinh tbody tr"), function (index, item) {
        var obj = {};
        var bIsDelete = $(this).hasClass("error-row");
        obj.iID_DuAn_HangMucID = $(item).find(".r_iID_DuAn_HangMucID").val();
        obj.sTenHangMuc = $.trim($(item).find(".txtTenHangMuc").val());
        obj.iID_ParentID = $(item).data("idparent");
        obj.iID_LoaiCongTrinhID = $(item).find(".selectLoaiCongTrinh select").val();
        obj.sTenLoaiCongTrinh = $(item).find(".selectLoaiCongTrinh select :selected").html();
        obj.smaOrder = $(item).find(".r_STT").html();
        obj.isDelete = bIsDelete;
        if (obj.iID_DuAn_HangMucID == "" || obj.iID_DuAn_HangMucID == undefined || obj.iID_DuAn_HangMucID == null)
            obj.iID_DuAn_HangMucID = uuidv4();
        lst_HangMuc.push(obj);
    });
    return lst_HangMuc;
}

function ShowHideButtonHangMuc() {
    var ListHangMucMois = GetHangMucByTable();
    ListHangMucMois.forEach(x => {
        var countChild = ListHangMucMois.filter(y => y.iID_ParentID == x.iID_DuAn_HangMucID && (y.isDelete == null || y.isDelete == false));
        var dongHienTai = $("#tblHangMucChinh .r_iID_DuAn_HangMucID[value='" + x.iID_DuAn_HangMucID + "']").closest('tr');
        if (countChild.length > 0) {
            $(dongHienTai).find("button.btn-delete").hide();
            //$(dongHienTai).find(".selectLoaiCongTrinh, select").attr('disabled');
            //$(dongHienTai).find(".txtTenHangMuc").attr('disabled');

        } else {
            $(dongHienTai).find("button.btn-delete").show();
            $(dongHienTai).find(".txtTenHangMuc").removeAttr('disabled');
        }
    })
}

var arrRowHM = [];
function SetClassDeleteHangMuc(removeClass) {
    if (!removeClass) {
        var lst_HangMuc = GetHangMucByTable();
        var filterHangMucCon = [];
        $.each($("#tblHangMucChinh tbody tr"), function (index, item) {
            if ($(item).find(".txtTenHangMuc").val() == "" && $(item).find(".selectLoaiCongTrinh select").val() == "") {
                filterHangMucCon = lst_HangMuc.filter(x => x.iID_ParentID == $(item).find(".r_iID_DuAn_HangMucID").val()
                    && !x.isDelete && x.sTenHangMuc != "" && x.iID_LoaiCongTrinhID != "");
                if (filterHangMucCon.length <= 0) {
                    $(this).addClass("error-row");
                    arrRowHM.push($(this));
                } else {
                    $(this).removeClass("error-row");
                }
            }
        });
    } else {
        if (arrRowHM.length > 0) {
            $.each(arrRowHM, function (index, item) {
                item.removeClass("error-row");
            });
        }
        arrRowHM = [];
    }

}