var TBL_CHI_PHI_DAU_TU = "tblChiPhiDauTu";
var TBL_NGUON_VON_DAU_TU = "tblNguonVonDauTu";
var TBL_TAI_LIEU_DINH_KEM = "tblThongTinTaiLieuDinhKem";
var TBL_HANG_MUC_CHINH = "tblHangMucChinh";
var trLoaiCongTrinh;
var arrNguonVon = [];
var arr_NguonVon_Dropdown = [];
var arr_HangMuc = [];
var arr_LoaiCongTrinh = [];
var lstHangMucs = [];
var lstNguonVons = [];
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var tCpdtGiaTriToTrinh = 0, tCpdtGiaTriThamDinh = 0, tCpdtGiaTriPheDuyet = 0;
var tNvdtGiaTriToTrinh = 0, tNvdtGiaTriThamDinh = 0, tNvdtGiaTriPheDuyet = 0;
var ERROR = 1;
var CONFIRM = 0;
var bIsSaveSuccess = false;


$(document).ready(function () {
    LoadDataComboBoxNguonVon();
    LoadDataComboBoxLoaiCongTrinh();
    GetlistNguonVonListHangMucTheoCTDauTu();
    SetValueMaskMaHangMucWhenViewUpdate(arr_HangMuc);
    EventChangeSetValueAutoMaHangMuc($("#txt_MaDuAn").val());
    LoadViewHangMuc();
    GetCbxChuDauTu();

    Event();

   // LayDshangMuc();

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

});

function LayDshangMuc() {

    var data = lstHangMucs;

    if (data != null) {
        for (var i = 0; i < data.length; i++) {
            var dongMoi = "";
            var currentChild = data.filter(n => n.iID_ParentID == data[i].iID_DuAn_HangMucID);
            if ((currentChild != null && currentChild.length != 0 && data[i].iID_ParentID == null) || data[i].iID_ParentID == null) {
                dongMoi += "<tr style='cursor: pointer;' class='parent' data-idparent='" + data[i].iID_ParentID + "'>";
            } else {
                dongMoi += "<tr style='cursor: pointer;' class='child' data-idparent='" + data[i].iID_ParentID + "'>";
            }
            dongMoi += "<td class='r_STT width-50'>" + data[i].sMaOrder + "</td><input type='hidden' class='r_iID_DuAn_HangMucID' value='" + data[i].iID_DuAn_HangMucID + "'data-idDuAn_HangMuc='" + data[i].iID_DuAn_HangMucID + "'/> <input type='hidden' class='r_HangMucID' value='" + data[i].iID_DuAn_HangMucID + "'/> <input type='hidden' class='r_HangMucParentID' value='" + data[i].iID_ParentID + "'/> <input type = 'hidden' class='r_iID_ChuTruongDauTu_HangMucID' value = '" + data[i].iID_ChuTruongDauTu_HangMucID + "'/>" +
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
        arr_HangMuc = data;
        lstHangMucs = data;

    }
    ShowHideButtonHangMuc();
}


function Event() {

    arrNguonVon = GetListDataNguonVonJson();
    lstNguonVons = GetListDataNguonVonJson();
    // LoadDataViewNguonVon();
    TinhLaiDongTong(TBL_NGUON_VON_DAU_TU);

    DinhDangSo();
    $("#txt_HangMucCha").select({
        width: 'resolve',
        matcher: matchStart
    });

    var idDuAn = $("#iID_DuAnID").val();

    ResetDataComboBoxHangMucCha();
}

function SetValueAutoSoQuyetDinh() {
    var result = [];
    var maDuAn = $("#txt_MaDuAn").val();

    if (!isStringEmpty(maDuAn)) {
        result.push(maDuAn.slice(0, 3));
    } else {
        result.push("");
    }

    var items = GetListDataHangMuc();

    if (arrHasValue(items)) {
        items.forEach(x => {
            result.push(x.sMaHangMuc.slice(x.sMaHangMuc.length - 2, x.sMaHangMuc.length));
        });
    }
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

function EnableButtonLuu() {
    var soLuongRequireDaNhap = $('.requireInput').filter(function () {
        return (this.value != '' && this.value != GUID_EMPTY);
    });
    if (soLuongRequireDaNhap.length == $(".requireInput").length)
        $("#btnLuu").removeAttr("disabled");
    else
        $("#btnLuu").attr("disabled", true);
}


function CapNhatCotStt(idBang) {
    $("#" + idBang + " tbody tr").each(function (index, tr) {
        $(tr).find('.r_STT').text(index + 1);
    });
}

function DinhDangSo() {
    $(".sotien").each(function () {
        if ($(this).is('input'))
            $(this).val(FormatNumber(UnFormatNumber($(this).val())));
        else
            $(this).html(FormatNumber(UnFormatNumber($(this).html())));
    })
}


function XoaDong(nutXoa, idBang) {
    var dongXoa = nutXoa.parentElement.parentElement;

    if (idBang == TBL_NGUON_VON_DAU_TU) {
        var rIIDNguonVon = $(dongXoa).find(".selectNguonVon select").val();
        var chuTruongNguonVonId = $(dongXoa).find(".r_iID_ChuTruongNguonVonId").val();
        if (chuTruongNguonVonId != undefined && chuTruongNguonVonId != "" && lstNguonVons.length > 0) {

            for (var i = 0; i < lstNguonVons.length; i++) {
                if (lstNguonVons[i].iID_ChuTruongDauTu_NguonVonID == chuTruongNguonVonId) {
                    lstNguonVons[i].isDelete = true;
                }
            }
        } else {
            lstNguonVons = lstNguonVons.filter(function (x) { return x.iID_NguonVonID != rIIDNguonVon });
        }

        if ($(dongXoa).hasClass('error-row')) {
            $(dongXoa).removeClass('error-row');
        } else {
            $(dongXoa).addClass('error-row');
        }
        TinhLaiDongTong(idBang);
    }

    else if (idBang == TBL_HANG_MUC_CHINH) {
        var ID_DuAn_HangMucID = $(dongXoa).find(".r_iID_DuAn_HangMucID")[0].value;
        var iIdParent = $(dongXoa).find("r_HangMucParentID").val();
        var hangMucId = $(dongXoa).find(".r_HangMucID").val();
        if (hangMucId != undefined && hangMucId != "" && lstHangMucs.length > 0) {
            for (var i = 0; i < lstHangMucs.length; i++) {
                if (lstHangMucs[i].iID_HangMucID == hangMucId) {
                    lstHangMucs[i].isDelete = true;
                }
            }
        } else {
            lstHangMucs = lstHangMucs.filter(function (x) { return x.iID_DuAn_HangMucID != ID_DuAn_HangMucID });
        }

        if ($(dongXoa).hasClass('error-row')) {
            $(dongXoa).removeClass('error-row');
        } else {
            $(dongXoa).addClass('error-row');
        }
        ShowHideButtonHangMuc();
    }

    //dongXoa.parentNode.removeChild(dongXoa);
    //CapNhatCotStt(idBang);

}


function XoaThemMoiNguonVon() {
    // xoa text data them moi
    $("#txtAddNvdtNguonVon").prop("selectedIndex", 0);
    $("#txtAddNvdtGiaTriToTrinh").val("");
    $("#txtAddNvdtGiaTriThamDinh").val("");
    $("#txtAddNvdtGiaTriPheDuyet").val("");
}

function CheckLoi(doiTuong) {
    var messErr = [];
    if (tCpdtGiaTriToTrinh != 0 && tCpdtGiaTriToTrinh != tNvdtGiaTriToTrinh)
        messErr.push("Giá trị tờ trình của danh sách chi phí và nguồn vốn không bằng nhau");
    if (tCpdtGiaTriThamDinh != 0 && tCpdtGiaTriThamDinh != tNvdtGiaTriThamDinh)
        messErr.push("Giá trị thẩm định của danh sách chi phí và nguồn vốn không bằng nhau");
    if (tCpdtGiaTriPheDuyet != 0 && tCpdtGiaTriPheDuyet != tNvdtGiaTriPheDuyet)
        messErr.push("Giá trị phê duyệt của danh sách chi phí và nguồn vốn không bằng nhau");
    //if (KiemTraTrungSoQuyetDinh(doiTuong.sSoQuyetDinh, doiTuong.iID_ChuTruongDauTuID) == true)
    //    messErr.push("Số quyết định đã tồn tại");

    if (messErr.length > 0) {
        alert(messErr.join("\n"));
        return false;
    } else {
        return true;
    }
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
        tongTien += (x.fTienToTrinh + x.fTienThamDinh + x.fTienPheDuyet);
    });

    if (tongTien < giaTriPheDuyet || tongTien > giaTriPheDuyet) {
        PopupModal("Lỗi lưu chủ trương đầu tư", ["Giá trị của danh sách nguồn vốn và tổng mức đầu tư phê duyệt không bằng nhau !"], ERROR);
        return false;
    }

    return true;
}

function GetDataFormChuTruongDauTu() {
    var id = $("#iIDChuTruongDauTuID").val();
    var soToTrinh = $.trim($("#sSoQuyetDinh").val());
    var ngayPheDuyet = $("#dNgayQuyetDinh").val();
    var iID_MaDonViQuanLy = $("#iID_MaDonViQuanLy").val();
    var iID_ChuDauTuID = $("#iID_ChuDauTuID").find(":selected").data("sid_cdt");
    var iID_MaChuDauTuID = $("#iID_ChuDauTuID").val();
    var iID_MaDonVi = $("#iID_ChuDauTuID").val();
    var iID_MaCDT = $("#iID_ChuDauTuID").val();

    var duAn = $("#iID_DuAnID").val();
    var donViQuanLy = $("#iID_DonViQuanLyID").val();
    var nhomDuAn = $("#iID_NhomDuAnID").val();
    var thoiGianThucHienTu = $("#sKhoiCong").val();
    var thoiGianThucHienDen = $("#sHoanThanh").val();
    var loaiCongTrinh = $("#iID_LoaiCongTrinhID").val();
    var phanCapPheDuyet = $("#iID_CapPheDuyetID").val();
    var tmdtPheDuyet = $("#fTMDTDuKienPheDuyet").val();
    var diaDiemDauTu = $.trim($("#sDiaDiem").val());
    var mucTieuDauTu = $.trim($("#sMucTieu").val());
    var quyMoDauTu = $.trim($("#sQuyMo").val());
    var moTa = $.trim($("#sMoTa").val());
    SetClassDeleteHangMuc();
    var lst_NguonVon = GetNguonVonByTable();
    CapNhatCotStt(TBL_HANG_MUC_CHINH);
    CapNhatCotSttCon(TBL_HANG_MUC_CHINH);
    var lst_HangMuc = GetHangMucByTable();

    return {
        iID_ChuTruongDauTuID: null,
        iID_ParentID: id,
        sSoQuyetDinh: soToTrinh,
        dNgayQuyetDinh: ngayPheDuyet,
        iID_DuAnID: duAn,
        iID_DonViQuanLyID: donViQuanLy,
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
        iID_MaDonViQuanLy: iID_MaDonViQuanLy,
        isDieuChinh: true,
        ListNguonVon: lst_NguonVon,
        ListHangMuc: lst_HangMuc
    };
}

function Luu() {
    var object = GetDataFormChuTruongDauTu();

    if (!ValidateTongTienTheoTMDTPheDuyet(object)) {
        SetClassDeleteHangMuc(true);
        return false;
    }


    $.ajax({
        url: "/QLVonDauTu/ChuTruongDauTu/Save",
        type: "POST",
        data: { model: object },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null) {
                if (data.status == false) {
                    PopupModal("Lỗi lưu dữ liệu chủ trương đầu tư", [data.message], ERROR);
                    return false;
                }
                else {
                    bIsSaveSuccess = true;
                    var objectId = data.iID_ChuTruongDauTuID;
                    DieuChinhUploadFile(objectId, 102);
        success: function (data) {
            PopupModal("Thông báo", data.sMessage, ERROR);
                }
            }
        },
        error: function (data) {

        }
    })
}

function KiemTraTrungSoQuyetDinh(sSoQuyetDinh, iID_ChuTruongDauTuID) {
    var check = false;
    $.ajax({
        url: "/QLVonDauTu/ChuTruongDauTu/KiemTraTrungSoQuyetDinh",
        type: "POST",
        data: { sSoQuyetDinh: sSoQuyetDinh, iID_ChuTruongDauTuID: iID_ChuTruongDauTuID },
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

// Nguồn vốn đầu tư

function GetListDataNguonVonJson() {
    var items = $("#arrNguonVon").val();

    if (isStringEmpty(items)) {
        return [];
    }

    items = JSON.parse(items);

    if (!arrHasValue(items)) {
        return [];
    }
    if (items != undefined && items.length > 0) {
        for (var i = 0; i < items.length; i++) {
            items[i].id = (i + 1).toString();
            items[i].fGiaTriTruocDieuChinh = items[i].fTienPheDuyet;
            items[i].fGiaTriDieuChinh = 0;
            items[i].iID_ChuTruongDauTu_NguonVonID = null;
        }
    }

    return items;
}

function GetListDataNguonVon() {
    return arrNguonVon;
}

function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

function GetDataFormNguonVon() {
    var id = $("#txt_IdNguonVon").val();
    var iIdNguonVon = $("#txtAddNvdtNguonVon").val();
    var tenNguonVon = $("#txtAddNvdtNguonVon :selected").html();
    var giaTriPheDuyet = $("#txtAddNvdtGiaTriPheDuyet").val();


    if (isStringEmpty(giaTriPheDuyet)) {
        giaTriPheDuyet = 0;
    }

    return {
        Id: id,
        iID_NguonVonID: iIdNguonVon,
        sTenNguonVon: tenNguonVon,
        fTienPheDuyet: parseFloat(UnFormatNumber(giaTriPheDuyet))
    };
}

function ValidateNguonVon(item) {
    if (item == null || item == undefined) {
        PopupModal("Lỗi thêm nguồn vốn đầu tư", ["Dữ liệu không chính xác !"], ERROR);
        return false;
    }

    var message = [];
    if (isStringEmpty(item.iID_NguonVonID)) {
        message.push("Vui lòng chọn nguồn vốn !");
    }

    if (item.fTienPheDuyet == "" || parseInt(UnFormatNumber(item.fTienPheDuyet) <= 0)) {
        message.push("Giá trị phê duyệt của nguồn vốn phải lớn hơn 0.");
    }

    for (var i = 0; i < arrNguonVon.length; i++) {
        if (arrNguonVon[i].sTenNguonVon == item.sTenNguonVon) {
            message.push("Nguồn vốn đã tồn tại");
            break;
        }
    }

    if (message.length > 0) {
        PopupModal("Lỗi thêm nguồn vốn đầu tư", message, ERROR);
        return false;
    }

    return true;
}

function ThemMoiNguonVon() {
    var dongMoi = "";
    dongMoi += "<tr>";
    dongMoi += "<td class='r_STT width-50'>" + "</td> ";
    dongMoi += "<td><input type='hidden' class='r_iID_NguonVonID' /><div class='sTenNguonVon' hidden></div><div class='selectNguonVon'>" + CreateHtmlSelectNguonVon() + "</div></td>";
    dongMoi += "<td class='r_GiaTriNguonVonTruocDC' ><div hidden></div><input type='text' disabled style='text-align: right' class='form-control txtGiaTriNguonVonTruocDC' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' /></td>"
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
    $(".selectNguonVon .form-control").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
}

function CreateHtmlSelectNguonVon(value) {
    var htmlOption = "";
    arr_NguonVon_Dropdown.forEach(x => {
        if (value != undefined && value == x.id)
            htmlOption += "<option value='" + x.id + "' selected>" + x.text + "</option>";
        else
            htmlOption += "<option value='" + x.id + "'>" + x.text + "</option>";
    })

    return "<select class='form-control'>" + htmlOption + "</option>";

    //if (value != undefined && value != "") {
    //    return "<select disabled class='form-control'>" + htmlOption + "</option>";
    //} else {
    //    return "<select class='form-control'>" + htmlOption + "</option>";
    //}

}

function CreateNguonVon(nutCreate, idBang) {
    var messErr = [];
    var dongHienTai = nutCreate.parentElement.parentElement;

    //var iID_DuAnID = $(dongHienTai).find(".r_iID_DuAn_HangMucID").val();
    var iID_NguonVonID = $(dongHienTai).find(".selectNguonVon select").val();
    var iID_ChuTruongDauTu_NguonVonID = $(dongHienTai).find(".r_iID_ChuTruongNguonVonId").val();
    var iID_ChuTruongDauTuID = $(dongHienTai).find(".r_iID_ChuTruongId").val();
    var id = $(dongHienTai).find(".r_STT").html();
    var sGiaTriTruocDC = $(dongHienTai).find(".txtGiaTriNguonVonTruocDC").val();
    var giaTriTruocDC = 0;
    if (sGiaTriTruocDC != "" && sGiaTriTruocDC != undefined) {
        giaTriTruocDC = parseInt(UnFormatNumber(sGiaTriTruocDC));
    }

    var sHanMucDauTu = $(dongHienTai).find(".txtGiaTriNguonVon").val();
    if (sHanMucDauTu == "") {
        messErr.push("Giá trị phê duyệt phải lớn hơn 0");
    }
    var giaTriPheDuyet = parseInt(UnFormatNumber(sHanMucDauTu));
    var giaTriDC = giaTriPheDuyet - giaTriTruocDC;
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
        //iID_ChuTruongDauTu_NguonVonID: iID_ChuTruongDauTu_NguonVonID,
        iID_ChuTruongDauTuID: iID_ChuTruongDauTuID,
        iID_NguonVonID: iID_NguonVonID,
        fGiaTriTruocDieuChinh: giaTriTruocDC,
        fTienPheDuyet: giaTriPheDuyet,
        fGiaTriDieuChinh: giaTriDC
    })

    TinhLaiDongTong(idBang);

}

function LoadDataComboBoxNguonVon() {
    $.ajax({
        url: "/QLVonDauTu/ChuTruongDauTu/LoadComboboxNguonVon",
        type: "POST",
        dataType: "json",
        async: false,
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

function CapNhatCotSttNguonVon(idBang) {
    $("#" + idBang + " tbody tr").each(function (index, tr) {
        $(tr).find('.r_STT').text(index + 1);
    });
}

function EditNguonVon(nutCreateHangMuc, idBang) {
    var dongHienTai = nutCreateHangMuc.parentElement.parentElement;
    $(dongHienTai).find(".selectNguonVon select").attr("disabled", false);
    $(dongHienTai).find(".txtGiaTriNguonVon").attr("disabled", false);

    $(dongHienTai).find("button.btn-edit").hide();
    $(dongHienTai).find("button.btn-save").show();
}


function LoadDataViewNguonVon() {
    var data = GetListDataNguonVon();

    if (data != null) {
        for (var i = 0; i < data.length; i++) {
            var dongMoi = "";
            dongMoi += "<tr>";
            dongMoi += "<td  class='r_STT width-50'>" + (i + 1) + "</td>";
            dongMoi += "<td><input type='hidden' class='r_iID_NguonVonID' value='" + data[i].iID_NguonVonID + "'/><div class='sTenNguonVon' hidden></div><div class='selectNguonVon'>" + CreateHtmlSelectNguonVon(data[i].iID_NguonVonID) + "</div></td>";
            /*dongMoi += "<td class='r_GiaTriNguonVonTruocDC' ><div hidden></div><input type='text' disabled style='text-align: right' class='form-control txtGiaTriNguonVonTruocDC' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value='" + FormatNumber(data[i].fTienPheDuyet) + "'/></td>"*/
            dongMoi += "<td class='r_GiaTriNguonVonTruocDC' ><div hidden></div><input type='text' disabled style='text-align: right' class='form-control txtGiaTriNguonVonTruocDC' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value='" + FormatNumber(data[i].fGiaTriTruocDieuChinh) + "'/></td>"
            //dongMoi += "<td class='r_GiaTriNguonVon' ><div hidden></div><input type='text' disabled style='text-align: right' class='form-control txtGiaTriNguonVon' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value='" + FormatNumber(data[i].fTienPheDuyet) + "'/></td>"
            dongMoi += "<td class='r_GiaTriNguonVon' ><div hidden></div><input type='text' style='text-align: right' class='form-control txtGiaTriNguonVon' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value='" + FormatNumber(data[i].fTienPheDuyet) + "' onblur='TinhLaiDongTong(TBL_NGUON_VON_DAU_TU)'/></td>"
            dongMoi += "<td class='width-150' align='center'>";
            //dongMoi += "<button class='btn-save btn-icon' hidden type = 'button' onclick = 'CreateNguonVon(this, \"" + TBL_NGUON_VON_DAU_TU + "\")'> " +
            //    "<i class='fa fa-floppy-o fa-lg' aria-hidden='true'></i>" +
            //    "</button> ";
            dongMoi += "<button class='btn-edit btn-icon'  hidden type = 'button' onclick = 'EditNguonVon(this, \"" + TBL_NGUON_VON_DAU_TU + "\")'> " +
                "<i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            dongMoi += "<button class='btn-delete btn-icon' type = 'button' onclick='XoaDong(this, \"" + TBL_NGUON_VON_DAU_TU + "\")' > " +
                "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span></button> <input type='hidden' class='r_iID_ChuTruongNguonVonId' value='" + data[i].iID_ChuTruongDauTu_NguonVonID + "'/> <input type='hidden' class='r_iID_ChuTruongId' value='" + data[i].iID_ChuTruongDauTuID + "'/></td>";
            dongMoi += "</tr>";

            $("#tblNguonVonDauTu tbody").append(dongMoi);

            TinhLaiDongTong(TBL_NGUON_VON_DAU_TU);
        }
    }
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

    LoadDataViewNguonVon();
}

function ComfirmDeleteNguonVon(id) {
    if (confirm("Bạn có chắc chắn muốn xóa không ?")) {
        DeleteNguonVon(id);
    }
}



function SetValueDataFormNguonVon(item) {
    $("#txt_IdNguonVon").val(item.Id);
    $("#txtAddNvdtNguonVon").val(item.iID_NguonVonID);
    $("#txtAddNvdtGiaTriPheDuyet").val(FormatNumber(item.fTienPheDuyet));
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

function LoadDataComboBoxLoaiCongTrinh() {
    $.ajax({
        url: "/QLVonDauTu/QLDuAn/GetLoaiCongTrinh",
        type: "POST",
        dataType: "json",
        async: false,
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

    //if (value != undefined && value != "") {
    //    return "<select disabled class='form-control'>" + htmlOption + "</option>";
    //} else {
    //    return "<select class='form-control'>" + htmlOption + "</option>";
    //}

}

function ThemMoiHangMuc() {
    var iID_DuAnID = $("#iID_DuAnID").val();
    var dongMoi = "";
    dongMoi += "<tr style='cursor: pointer;' class='parent'>";
    dongMoi += "<td class='r_STT width-50'></td><input type='hidden' class='r_iID_DuAn_HangMucID' value='" + uuidv4() + "'data-iId_hangmuc='" + iID_DuAnID + "'/>";
    dongMoi += "<td class='r_sTenHangMuc'><div class='sTenHangMuc' hidden></div><input type='text' class='form-control txtTenHangMuc'/></td>"
    dongMoi += "<td><input type='hidden' class='r_iID_LoaiCongTrinhID' value=''/><div class='sTenLoaiCongTrinh' hidden></div><div class='selectLoaiCongTrinh'>" + CreateHtmlSelectLoaiCongTrinh() + "</div></td>";
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

    $("#" + TBL_HANG_MUC_CHINH + " tbody").append(dongMoi);
    CapNhatCotSttHangMuc();
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

var arr_STT = [];
function CapNhatCotSttHangMuc() {
    $("#" + TBL_HANG_MUC_CHINH + " tbody tr.parent").each(function (index, tr) {
        var sttHangMuc = $(tr).find('.r_STT').text();
        if (sttHangMuc != "" && sttHangMuc != undefined && !sttHangMuc.includes("-")) {
            arr_STT.push(parseInt(sttHangMuc));
        }
        else {
            arr_STT.push(0);
        }
    });

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
    $("#tblHangMucChinh [data-idparent='" + iIDDuAnHangMucID + "'] .btn-delete").hide();
    if (rowChild != undefined) {
        var a = rowChild.closest("tr");
        $(a).after(dongMoi);
    } else {
        $(dongHienTai).after(dongMoi);
    }

    arr_STT_Child = [];
    CapNhatCotSttHangMucCon(iIDDuAnHangMucID, sttParent);
    ShowHideButtonHangMuc();
    //CapNhatCotSttCon(TBL_HANG_MUC_CHINH);
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


function XoaHangMuc() {
    ResetFormHangMuc();
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
            if (item.Id != x.Id && item.sMaHangMuc == x.sMaHangMuc) {
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

function AddHangMuc(item) {
    if (isStringEmpty(item)) {
        return;
    }

    if (isStringEmpty(item.Id)) {
        item.Id = uuidv4();
        arr_HangMuc.push(item);
        SaveMaskMaHangMuc();
        return;
    }

    var items = GetListDataHangMuc();
    if (!arrHasValue(items)) {
        return;
    }

    items.forEach(x => {
        if (x.Id == item.Id) {
            x.sMaHangMuc = item.sMaHangMuc;
            x.sTenHangMuc = item.sTenHangMuc;
            x.sHangMucCha = item.sHangMucCha;
            x.sTenHangMucCha = item.sTenHangMucCha;
        }
    });

    arr_HangMuc = items;
}

function ResetFormHangMuc() {
    $("#txt_IdHangMuc").val("");
    //$("#txt_MaHangMuc").val("");
    $("#txt_TenHangMuc").val("");
    $("#txt_HangMucCha").val("");
    $('#txt_HangMucCha').trigger('change');
}

function DeleteRowHangMuc(id) {
    if (isStringEmpty(id)) {
        return false;
    }

    var items = GetListDataHangMuc();

    if (!arrHasValue(items)) {
        return false;
    }

    var item = items.find(x => x.Id == id);
    if (item == null || item == undefined) {
        return false;
    }

    if (!CheckDeleteHangMuc(item)) {
        return false;
    }

    if (!DeleteHangMuc(item)) {
        return false;
    }

    LoadViewHangMuc();
    ResetFormHangMuc();
    EventChangeSetValueAutoMaHangMuc($("#txt_MaDuAn").val());

    return true;
}

function CreateHangMuc(nutCreateHangMuc, idBang) {
    var dongHienTai = nutCreateHangMuc.parentElement.parentElement;
    var messErr = [];
    var iIDDuAnHangMucID = $(dongHienTai).find(".r_iID_DuAn_HangMucID").val();
    var iIDParentID = $(dongHienTai).data("idparent");
    var hangMucId = $(dongHienTai).find(".r_HangMucID").val();

    var sTenHangMuc = $(dongHienTai).find(".txtTenHangMuc").val();
    var iIDLoaiCongTrinhID = $(dongHienTai).find(".selectLoaiCongTrinh select").val();
    var sTenLoaiCongTrinh = $(dongHienTai).find(".selectLoaiCongTrinh select :selected").html();
    var smaOrder = $(dongHienTai).find(".r_STT").html();

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

    $(dongHienTai).find(".txtTenHangMuc").attr("disabled", true);

    $(dongHienTai).find(".selectLoaiCongTrinh select").attr("disabled", true);

    $(dongHienTai).find("button.btn-edit").show();
    $(dongHienTai).find("button.btn-save").hide();
    $(dongHienTai).find("button.btn-add-child").show();

    arr_HangMuc = arr_HangMuc.filter(function (x) { return x.iID_DuAn_HangMucID != iIDDuAnHangMucID });

    arr_HangMuc.push({
        iID_DuAn_HangMucID: iIDDuAnHangMucID,
        iID_HangMucID: hangMucId,
        iID_ParentID: iIDParentID,
        sTenHangMuc: sTenHangMuc,
        iID_LoaiCongTrinhID: iIDLoaiCongTrinhID,
        sTenLoaiCongTrinh: sTenLoaiCongTrinh,
        smaOrder: smaOrder
    })

    TinhLaiDongTong(idBang);
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

function ComfirmDeleteHangMuc(id) {
    if (confirm("Bạn có chắc chắn muốn xóa không ?")) {
        DeleteRowHangMuc(id);
    }
}

function CheckDeleteHangMuc(item) {
    if (item == null || item == undefined) {
        return false;
    }

    var items = GetListDataHangMuc();

    if (!arrHasValue(items)) {
        return false;
    }

    var check = items.find(x => x.sHangMucCha == item.sMaHangMuc);
    if (check != null && check != undefined) {
        PopupModal("Lỗi xóa hạng mục", ["Hạng mục " + item.sMaHangMuc + " đang có danh hạng mục con !"], ERROR);
        return false;
    }

    return true;
}

function DeleteHangMuc(item) {
    if (item == null || item == undefined) {
        return false;
    }

    if (isStringEmpty(item.sMaHangMuc)) {
        return false;
    }

    var items = GetListDataHangMuc();
    if (!arrHasValue(items)) {
        return false;
    }

    var result = [];

    items.forEach(x => {
        if (x.sMaHangMuc != item.sMaHangMuc) {
            result.push(x);
        }
    });

    arr_HangMuc = result;

    ResetFormHangMuc();
    DeleteItemDataComboBoxHangMuc(item);
    ResetDataComboBoxHangMucCha();

    return true;
}

function EditHangMuc(nutEdit, idBang) {
    var dongHienTai = nutEdit.parentElement.parentElement;
    var iIDLoaiCongTrinhID = $(dongHienTai).find(".selectLoaiCongTrinh select").val();

    $(dongHienTai).find(".txtTenHangMuc").attr("disabled", false);


    $(dongHienTai).find(".selectLoaiCongTrinh select").attr("disabled", false);

    $(dongHienTai).find("button.btn-edit").hide();
    $(dongHienTai).find("button.btn-save").show();
    $(dongHienTai).find("button.btn-add-child").hide();
}

function SetValueDataFormHangMuc(item) {
    $("#txt_IdHangMuc").val(item.Id);
    $("#txt_MaHangMuc").val(item.sMaHangMuc);
    $("#txt_TenHangMuc").val(item.sTenHangMuc);
    $("#txt_HangMucCha").val(item.sHangMucCha);
    $('#txt_HangMucCha').trigger('change');
}

function GetDataChildByMaHangMuc(sMaHangMuc) {
    var result = [];

    if (isStringEmpty(sMaHangMuc)) {
        return result;
    }

    var items = GetListDataHangMuc();
    if (!arrHasValue(items)) {
        return result;
    }

    items.forEach(x => {
        if (x.sHangMucCha == sMaHangMuc) {
            result.push(x);

            var listChild = GetDataChildByMaHangMucTree(items, x.sMaHangMuc);
            if (arrHasValue(listChild)) {
                listChild.forEach(c => {
                    result.push(c);
                });
            }
        }
    });

    return result;
}

function GetDataChildByMaHangMucTree(items, sMaHangMuc) {
    var result = [];

    if (isStringEmpty(sMaHangMuc)) {
        return result;
    }

    if (!arrHasValue(items)) {
        return result;
    }

    items.forEach(x => {
        if (x.sHangMucCha == sMaHangMuc) {
            result.push(x);

            var listChild = GetDataChildByMaHangMucTree(items, x.sMaHangMuc);
            if (arrHasValue(listChild)) {
                listChild.forEach(c => {
                    result.push(c);
                });
            }
        }
    });

    return result;
}

function GetDataRemoveComboBoxChild(item) {
    var result = [
        {
            id: "",
            text: "--Chọn--"
        }
    ];

    if (isStringEmpty(item)) {
        return result;
    }

    var items = GetListDataHangMuc();
    if (!arrHasValue(items)) {
        return result;
    }

    var list = GetDataChildByMaHangMuc(item.sMaHangMuc);
    if (!arrHasValue(list)) {
        items.forEach(x => {
            if (x.sMaHangMuc != item.sMaHangMuc) {
                var hm = {
                    id: x.sMaHangMuc,
                    text: '(' + x.sMaHangMuc + ') - ' + x.sTenHangMuc + ''
                };
                result.push(hm);
            }
        });
    } else {
        items.forEach(x => {
            var check = list.find(i => i.sMaHangMuc == x.sMaHangMuc);
            if (isStringEmpty(check) && x.sMaHangMuc != item.sMaHangMuc) {
                var hm = {
                    id: x.sMaHangMuc,
                    text: '(' + x.sMaHangMuc + ') - ' + x.sTenHangMuc + ''
                };
                result.push(hm);
            }
        });
    }

    return result;
}

function RefreshComboBoxWithDataChild(item) {
    $("#txt_HangMucCha").empty();
    $('#txt_HangMucCha').select2({
        data: GetDataRemoveComboBoxChild(item),
        sorter: function (data) {
            return SortHangMucCha(data);
        }
    });
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
        html += "<button style='margin-right:5px' class='btn-edit btn-icon' type='button' onclick='EditHangMuc(\"" + x.Id + "\")'>" +
            "<span class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></span>" +
            "</button>";
        html += "<button class='btn-delete btn-icon' type='button' onclick='ComfirmDeleteHangMuc(\"" + x.Id + "\")'>" +
            "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
            "</button>";
        html += "</td>";
        html += '</tr>';
        count++;
        html += LoadViewHangMucTree(stt, items, x.sMaHangMuc);
    });

    $("#tblDuAnHangMuc tbody").append(html);
}

function LoadViewHangMucTree(stt, items, id) {
    var html = "";

    var listParent = [];

    items.forEach(x => {
        if (x.sHangMucCha == id) {
            listParent.push(x);
        }
    });

    if (listParent.length <= 0) {
        return;
    }

    var countChild = 1;
    listParent.forEach(x => {
        var isCheckParent = items.find(k => k.sHangMucCha == x.sMaHangMuc);
        if (isCheckParent == null || isCheckParent == undefined) {
            html += '<tr>';
        } else {
            html += '<tr style="font-weight:bold">';
        }

        var sttChild = stt + "." + countChild;
        html += '<td>' + sttChild + '</td>';
        html += '<td>' + x.sMaHangMuc + '</td>';
        html += '<td>' + x.sTenHangMuc + '</td>';
        html += '<td>' + x.sHangMucCha + '</td>';
        html += "<td>";
        html += "<button style='margin-right:5px' class='btn-edit btn-icon' type='button' onclick='EditHangMuc(\"" + x.Id + "\")'>" +
            "<span class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></span>" +
            "</button>";
        html += "<button class='btn-delete btn-icon' type='button' onclick='ComfirmDeleteHangMuc(\"" + x.Id + "\")'>" +
            "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
            "</button>";
        html += "</td>";
        html += '</tr>';

        countChild++;

        html += LoadViewHangMucTree(sttChild, items, x.sMaHangMuc);
    });

    return html;
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
        if (item.sHangMucCha == item.sMaHangMuc) {
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
    }

    if (isStringEmpty(tenHangMucCha)) {
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

function DeleteItemDataComboBoxHangMuc(item) {
    if (isStringEmpty(item)) {
        return;
    }

    var items = GetListDataHangMucCha();
    if (!arrHasValue(items)) {
        return;
    }

    var result = [];

    items.forEach(x => {
        if (x.id != item.sMaHangMuc) {
            result.push(x);
        }
    });

    arr_HangMucCha = result;
}

function UpdateItemDataComboBoxHangMuc(item) {
    if (isStringEmpty(item)) {
        return;
    }

    var x = {
        id: item.sMaHangMuc,
        text: '(' + item.sMaHangMuc + ') - ' + item.sTenHangMuc + ''
    }

    var items = GetListDataHangMucCha();
    if (!arrHasValue(items)) {
        return;
    }

    var model = items.find(i => i.id == x.id);
    if (isStringEmpty(model)) {
        arr_HangMucCha.push(x);
        return;
    }

    items.forEach(i => {
        if (i.id == x.id) {
            i.text = x.text;
        }
    });

    arr_HangMucCha = items;
}

function AddItemDataComboBoxHangMucSingle(item) {
    if (item == null || item == undefined) {
        return;
    }

    var object = {
        id: item.sMaHangMuc,
        text: '(' + item.sMaHangMuc + ') - ' + item.sTenHangMuc + ''
    };

    arr_HangMucCha.push(object);
}

function AddItemDataComboBoxHangMuc(data) {
    SetValuearr_HangMucDefault();

    if (arrHasValue(data)) {
        arr_HangMucCha.push(data);
    }

    var items = GetListDataHangMuc();

    if (arrHasValue(items)) {
        items.forEach(x => {
            UpdateItemDataComboBoxHangMuc(x);
        });
    }

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

function SetValuearr_HangMucDefault() {
    arr_HangMucCha = [{
        id: "",
        text: "--Chọn--"
    }];
}

function GetListDataHangMucCha() {
    return arr_HangMucCha;
}

function SortHangMucCha(items) {
    if (!arrHasValue(items)) {
        return [];
    }

    items.sort(function (a, b) {
        var codeA = a.id.toUpperCase();
        var codeB = b.id.toUpperCase();

        if (codeA < codeB) {
            return -1;
        }

        if (codeA > codeB) {
            return 1;
        }

        return 0;
    });

    return items;
}

var arr_HangMucCha = [
    {
        id: "",
        text: "--Chọn--"
    }
];


// Set auto mã hạng mục

function ReloadMaHangMucAuToWhenChangeDuAn(sMaDuAn) {
    var items = GetListDataHangMuc();

    if (!arrHasValue(items)) {
        return;
    }

    var v = GetWithEndMaDuAn(sMaDuAn);

    items.forEach(x => {
        x.sMaHangMuc = v + GetEndWithMaHangMuc(x.sMaHangMuc);
    });

    arr_HangMuc = items;
}

function GetEndWithMaHangMuc(v) {
    return v.substr(v.length - 3, v.length);
}

function EventChangeSetValueAutoMaHangMuc(sMaDuAn) {
    $("#txt_MaHangMuc").val(GetValueMaHangMucAuTo(sMaDuAn));
}

function GetValueMaHangMucAuTo(sMaDuAn) {
    return GetWithEndMaDuAn(sMaDuAn) + GetFormatNumberMaHangMuc(GetNextMaskMaHangMuc());
}

function GetWithEndMaDuAn(v) {
    var result = v.substr(v.length - 4, v.length);
    return result;
}

var mask_MaHangMuc = 0;

function SetValueMaskMaHangMucWhenViewUpdate(items) {
    if (!arrHasValue(items)) {
        return;
    }

    mask_MaHangMuc = items.length;
}

function GetNextMaskMaHangMuc() {
    return mask_MaHangMuc + 1;
}

function SaveMaskMaHangMuc() {
    mask_MaHangMuc = mask_MaHangMuc + 1;
}

function GetFormatNumberMaHangMuc(v) {
    if (v < 10) {
        return "00" + v;
    }

    if (10 <= v < 100) {
        return "0" + v;
    }

    return v;
}

// End set auto mã hạng mục
function GetCbxChuDauTu() {
    $.ajax({
        url: "/QLVonDauTu/ChuTruongDauTu/GetCbxChuTruongDauTu",
        type: "GET",
        dataType: "json",
        success: function (result) {
            if (result.data != null && result.data != "") {
                $("#iID_ChuDauTuID").append(result.data);
                $(".selectNguonVon .form-control").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
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
        obj.fGiaTriTruocDieuChinh = $(item).find(".txtGiaTriNguonVonTruocDC").val();
        obj.isDelete = bIsDelete;

        if (obj.fTienPheDuyet == null || obj.fTienPheDuyet == "") {
            obj.fTienPheDuyet = 0;
        } else {
            obj.fTienPheDuyet = parseFloat(obj.fTienPheDuyet.toString().replaceAll(".", ""));
        }

        if (obj.fGiaTriTruocDieuChinh == null || obj.fGiaTriTruocDieuChinh == "") {
            obj.fGiaTriTruocDieuChinh = 0;
        } else {
            obj.fGiaTriTruocDieuChinh = parseFloat(obj.fGiaTriTruocDieuChinh.toString().replaceAll(".", ""));
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
        //if (obj.iID_DuAn_HangMucID == "" || obj.iID_DuAn_HangMucID == undefined || obj.iID_DuAn_HangMucID == null)
        //    obj.iID_DuAn_HangMucID = uuidv4();
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

function GetlistNguonVonListHangMucTheoCTDauTu() {
    var CTDauTuId = $("#iIDChuTruongDauTuID").val();
    $.ajax({
        url: "/QLVonDauTu/ChuTruongDauTu/GetListNguonVOnVaHangMucByCTDauTuDieuChinh",
        type: "POST",
        data: { CTDauTuId: CTDauTuId, isDieuChinh: true },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data.status == false) {
                return;
            }
            if (arrHasValue(data.dataNguonVon)) {
                data.dataNguonVon.forEach(x => {
                    var item = {
                        id: uuidv4(),
                        iID_QDDauTu_NguonVonID: null,
                        iID_NguonVonID: x.iID_NguonVonID,
                        sTenNguonVon: x.sTenNguonVon,
                        fTienPheDuyet: x.fTienPheDuyet,
                        fGiaTriDieuChinh: x.fGiaTriDieuChinh,
                        fGiaTriTruocDieuChinh: x.fGiaTriTruocDieuChinh
                    };
                    lstNguonVons.push(item);
                });
                LoadDataViewNguonVon();
            }

            if (arrHasValue(data.dataHangMuc)) {
                data.dataHangMuc.forEach(x => {
                    var item = {
                        id: uuidv4(),
                        iID_ChuTruongDauTu_HangMucID: null,
                        iID_HangMucID: x.iID_HangMucID,
                        iID_DuAn_HangMucID: x.iID_DuAn_HangMucID,
                        iID_LoaiCongTrinhID: x.iID_LoaiCongTrinhID,
                        iID_ParentID: x.iID_ParentID,
                        sMaHangMuc: x.sMaHangMuc,
                        sTenHangMuc: x.sTenHangMuc,
                        sTenLoaiCongTrinh: x.sTenLoaiCongTrinh,
                        sMaOrder: x.smaOrder
                    }
                    lstHangMucs.push(item);
                })
                LayDshangMuc();

                $(".selectLoaiCongTrinh select").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
            }
        }
    });
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