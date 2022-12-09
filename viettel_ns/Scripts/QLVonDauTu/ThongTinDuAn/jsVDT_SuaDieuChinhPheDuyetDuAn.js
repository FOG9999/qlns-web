var TBL_CHI_PHI_DAU_TU = "tblChiPhiDauTu";
var TBL_NGUON_VON_DAU_TU = "tblNguonVonDauTu";
var TBL_HANG_MUC_CHINH = "tblHangMucChinh";
var TBL_TAI_LIEU_DINH_KEM = "tblThongTinTaiLieuDinhKem";
var DUAN_HANGMUC = "duAnHangMuc";
var arrChiPhiSave = [];
var arrNguonVonSave = [];
var arr_NguonVon_Dropdown = [];
var arrHangMucChuTruong = [];
var arrHangMucSave = [];
var arrHangMucTheoDuAn = [];
var elementChiPhi = null;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var tCpdtGiaTriToTrinh = 0, tCpdtGiaTriThamDinh = 0, tCpdtGiaTriPheDuyet = 0;
var tNvdtGiaTriToTrinh = 0, tNvdtGiaTriThamDinh = 0, tNvdtGiaTriPheDuyet = 0;
var tHmcGiaTri = 0;
var ERROR = 1;
var CONFIRM = 0;

$(document).ready(function () {
    LoadDataComboBoxNguonVon();
    LoadDataComboBoxLoaiCongTrinh();
    LoadDataFirst();

    DinhDangSo();
});

function LoadDataFirst() {
    GetNguonVonTheoQDDauTu();
    GetChiPhiHangMucTheoQDDauTu();
}

function GetChiPhiHangMucTheoQDDauTu() {
    var qdDauTuId = $("#iIDQuyetDinhDauTuId").val();
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetDuAn/GetListChiPhiVaHangMucByQDDauTuDieuChinh",
        type: "POST",
        data: { qdDauTuId: qdDauTuId },
        dataType: "json",
        cache: false,
        success: function (resp) {
            if (resp.status == false) {
                return;
            }
            if (arrHasValue(resp.dataChiPhi)) {
                resp.dataChiPhi.forEach(x => {
                    var item = {
                        id: x.Id,
                        iID_QDDauTu_ChiPhiID: x.iID_QDDauTu_ChiPhiID,
                        iID_ChiPhiID: x.iID_ChiPhi,
                        iID_DuAn_ChiPhi: x.iID_DuAn_ChiPhi,
                        sTenChiPhi: x.sTenChiPhi,
                        iID_ChiPhi_Parent: x.iID_ChiPhi_Parent,
                        fTienPheDuyet: x.fTienPheDuyet,
                        iThuTu: x.iThuTu,
                        fGiaTriDieuChinh: x.fGiaTriDieuChinh,
                        fGiaTriTruocDieuChinh: x.fGiaTriTruocDieuChinh
                    };
                    arrChiPhiSave.push(item);
                });
                LoadDataViewChiPhi();
                ShowHideButtonChiPhi();
                CalculateTienConLaiNguonVon();
            }

            if (arrHasValue(resp.dataHangMuc)) {
                resp.dataHangMuc.forEach(x => {
                    var item = {
                        id: x.iID_QDDauTu_HangMuciID,
                        iID_QDDauTu_HangMuciID: x.iID_QDDauTu_HangMuciID,
                        iID_HangMucID: x.iID_HangMucID,
                        iID_DuAn_ChiPhi: x.iID_DuAn_ChiPhi,
                        iID_LoaiCongTrinhID: x.iID_LoaiCongTrinhID,
                        iID_QDDauTu_DM_HangMucID: x.iID_QDDauTu_DM_HangMucID,
                        iID_ParentID: x.iID_ParentID,
                        fTienPheDuyet: x.fTienPheDuyet,
                        fGiaTriDieuChinh: x.fGiaTriDieuChinh,
                        fGiaTriTruocDieuChinh: x.fGiaTriTruocDieuChinh,
                        fTienPheDuyet: x.fTienPheDuyet,
                        sMaHangMuc: x.sMaHangMuc,
                        sTenHangMuc: x.sTenHangMuc,
                        sMaOrder: x.smaOrder
                    }
                    arrHangMucSave.push(item);
                })
            }
        }
    });
}

function GetNguonVonTheoQDDauTu() {
    var qdDauTuId = $("#iIDQuyetDinhDauTuId").val();
    arrNguonVonSave = [];
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetDuAn/GetListNguonVonTheoQDDauTuDauTu",
        type: "POST",
        data: { qdDauTuId: qdDauTuId },
        dataType: "json",
        cache: false,
        success: function (resp) {
            if (resp.status == false) {
                return;
            }
            if (arrHasValue(resp.data)) {
                resp.data.forEach(x => {
                    var item = {
                        id: x.iID_QDDauTu_NguonVonID,
                        iID_QDDauTu_NguonVonID: x.iID_QDDauTu_NguonVonID,
                        iID_NguonVonID: x.iID_NguonVonID,
                        sTenNguonVon: x.sTenNguonVon,
                        fTienPheDuyet: x.fTienPheDuyet,
                        fGiaTriDieuChinh: x.fGiaTriDieuChinh,
                        fGiaTriTruocDieuChinh: x.fGiaTriTruocDieuChinh,
                        fTienPheDuyetCTDT: x.fTienPheDuyetCTDT
                    };
                    arrNguonVonSave.push(item);
                });
                SetValueTMTDPheDuyetTheoNguonVon();
                LoadDataViewNguonVon();

                // call function to apply common validators
                CallFunctionCommon();
            }
        }
    });
}

// Hàm chung

function PopupModalSave(title, message, category) {
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: title, Messages: message, Category: category },
        success: function (data) {
            $("#divModalConfirm").html(data);
            $('#confirmModal').on('hidden.bs.modal', function () {
                window.location.href = "/QLVonDauTu/QLPheDuyetDuAn/Index";
            })
        }
    });
}

function SetSelectById(id) {
    $("#" + id).select({
        width: 'resolve',
        matcher: matchStart
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

function PopupModal(title, message, category) {
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: title, Messages: message, Category: category },
        success: function (data) {
            $("#divModalConfirm").html(data);
        }
    });
}

function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

function SortOrderDataComboBox(items) {
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
// end Hàm chung

// Quyết định đầu tư
function GetFormDataQuyetDinhDauTu() {
    var iID_QDDauTuID = $("#iIDQuyetDinhDauTuId").val();
    var soQuyetDinh = $("#sSoQuyetDinh").val();
    var ngayQuyetDinh = $("#dNgayQuyetDinh").val();
    var SMoTa = $("#sMoTa").val();
    var donViQuanLy = $("#iID_DonViQuanLyID").val();
    var duAn = $("#iID_DuAnID").val();
    var maDuAn = $("#sMaDuAn").val();
    var chuDauTu = $("#iID_ChuDauTuID").val();
    var nhomDuAn = $("#iID_NhomDuAnID").val();
    var thoiGianThucHienTu = $("#sKhoiCong").val();
    var thoiGianThucHienDen = $("#sKetThuc").val();
    var hinhThuc = $("#iID_HinhThucQuanLyID").val();
    var giaPheDuyet = $("#fTongMucDauTuPheDuyet").val() == "" ? 0 : parseFloat(UnFormatNumber($("#fTongMucDauTuPheDuyet").val()));
    var tongPheDuyet = $("#fTongMucPheDuyetTheoChuTruong").val() == "" ? 0 : parseFloat(UnFormatNumber($("#fTongMucPheDuyetTheoChuTruong").val()));
    var diaDiem = $("#sDiaDiem").val();

    return {
        iID_QDDauTuID: iID_QDDauTuID,
        sSoQuyetDinh: soQuyetDinh,
        dNgayQuyetDinh: ngayQuyetDinh,
        SMoTa: SMoTa,
        iID_DonViQuanLyID: donViQuanLy,
        iID_DuAnID: duAn,
        sMaDuAn: maDuAn,
        iID_ChuDauTuID: chuDauTu,
        iID_NhomDuAnID: nhomDuAn,
        sKhoiCong: thoiGianThucHienTu,
        sKetThuc: thoiGianThucHienDen,
        iID_HinhThucQuanLyID: hinhThuc,
        fTongMucDauTuPheDuyet: giaPheDuyet,
        sDiaDiem: diaDiem,
        ListChiPhi: arrChiPhiSave,
        ListNguonVon: arrNguonVonSave,
        ListHangMuc: arrHangMucSave
    };
}

function Luu() {
    var item = GetFormDataQuyetDinhDauTu();
    if (!ValidateBeforeSaveQuyetDinhDauTu(item)) {
        return false;
    }

    if (!ValidateBeforeTongTienNguonVonWithChiPhi()) {
        return false;
    }

    //if (!ValidateBeforeTongTienHangMucWithChiPhi()) {
    //    return false;
    //}

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLPheDuyetDuAn/Save",
        data: { model: item, isDieuChinh: false, fTongMucPheDuyetTheoChuTruong: $("#fTongMucDauTuPheDuyet").val() },
        dataType: "json",
        cache: false,
        success: function (resp) {
            if (resp == null || resp.status == false) {
                PopupModal("Lỗi thêm mới phê duyệt dự án !", [resp.message], ERROR);
                return false;
            }
            var objectId = resp.ID;
            UploadFile(objectId, 103);
            PopupModalSave("Thông báo", ["Lưu dữ liệu thành công"], ERROR);
        }
    })

}

function Huy() {
    window.location.href = "/QLVonDauTu/QLPheDuyetDuAn/Index";
}

function ValidateBeforeSaveQuyetDinhDauTu(item) {
    var message = [];

    if (item == null || item == undefined) {
        PopupModal("Lỗi thêm mới phê duyệt dự án !", ["Dữ liệu cập nhật không chính xác !"], ERROR);
        return false;
    }

    if (isStringEmpty(item.sSoQuyetDinh)) {
        message.push("Vui lòng nhập số quyết định !");
    }

    if (isStringEmpty(item.dNgayQuyetDinh)) {
        message.push("Vui lòng nhập ngày phê duyệt !");
    }

    if (isStringEmpty(item.iID_DonViQuanLyID)) {
        message.push("Vui lòng chọn đơn vị quản lý !");
    }

    if (isStringEmpty(item.iID_DuAnID)) {
        message.push("Vui lòng chọn dự án !");
    }

    if (CheckTrungTenNguonVon()) {
        message.push("Nguồn vốn đã tồn tại. Vui lòng chọn lại!");
    }

    if (message.length > 0) {
        PopupModal("Lỗi thêm mới phê duyệt dự án !", message, ERROR);
        return false;
    }
    return true;
}

function CheckTrungTenNguonVon() {
    var isCheck = false;
    for (var i = 0; i < arrNguonVonSave.length; i++) {
        if (arrNguonVonSave[i].isDelete == null || arrNguonVonSave[i].isDelete == false) {
            if (arrNguonVonSave.filter(y => (y.isDelete == undefined || y.isDelete == false) && y.iID_NguonVonID == arrNguonVonSave[i].iID_NguonVonID).length > 1) {
                isCheck = true;
                break;
            }
        }
    }
    return isCheck;
}

function ValidateBeforeTongTienNguonVonWithChiPhi() {
    var conLaiNguonVon = $("#tonggiatriconlainguonvon").text();
    var giaTriConLai = 0;
    if (conLaiNguonVon == "") {
        return true
    } else {
        giaTriConLai = parseFloat(UnFormatNumber(conLaiNguonVon));
    }

    if (giaTriConLai != 0) {
        //PopupModal("Lỗi lưu phê duyệt dự án", ["Tổng tiền nguồn vốn và tổng tiền chi phí không bằng nhau"], ERROR);
        //return false;
        return confirm("Tổng chi phí không bằng tổng nguồn vốn, bạn có chắc chắn muốn lưu dữ liệu?");
    }
    return true;
}
// End quyết định đầu tư

function CheckLoi(doiTuong) {
    var messErr = [];
    if (tCpdtGiaTriToTrinh != 0 && tCpdtGiaTriToTrinh != tNvdtGiaTriToTrinh)
        messErr.push("Giá trị tờ trình của danh sách chi phí và nguồn vốn không bằng nhau");
    if (tCpdtGiaTriThamDinh != 0 && tCpdtGiaTriThamDinh != tNvdtGiaTriThamDinh)
        messErr.push("Giá trị thẩm định của danh sách chi phí và nguồn vốn không bằng nhau");
    if (tCpdtGiaTriPheDuyet != 0 && tCpdtGiaTriPheDuyet != tNvdtGiaTriPheDuyet)
        messErr.push("Giá trị phê duyệt của danh sách chi phí và nguồn vốn không bằng nhau");
    if (KiemTraTrungSoQuyetDinh(doiTuong.sSoQuyetDinh) == true)
        messErr.push("Số quyết định đã tồn tại");

    if (messErr.length > 0) {
        alert(messErr.join("\n"));
        return false;
    } else {
        return true;
    }
}

function KiemTraTrungSoQuyetDinh(sSoQuyetDinh) {
    var check = false;
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetDuAn/KiemTraTrungSoQuyetDinh",
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

function LoadDataViewChiPhi() {
    var html = "";
    if (!arrHasValue(arrChiPhiSave)) {
        $("#tblChiPhiDauTu tbody").html(html);
        return;
    }

    $("#tblChiPhiDauTu tbody").html(html);

    arrChiPhiSave.forEach(x => {
        var fontWeight = x.iID_ChiPhi_Parent ? "" : "font-weight: bold;";
        html += '<tr data-id="' + x.id + '" data-iidduanchiphi="' + x.iID_DuAn_ChiPhi + '">';
        html += '<input type="hidden" class="r_IThuTu" value = "' + x.iThuTu + '"/> <input type="hidden" class="r_IsLoaiChiPhi" value = "' + x.IsDefault + '"/>';
        html += '<td class="width-50" style="text-align:center;">' + x.iThuTu + "</td >"; 
        html += `<td style="${fontWeight}text-align:left;" ><input type = "text" onblur="UpdateChiPhi(this)" class="form-control r_TenChiPhi" value = "` + x.sTenChiPhi + '" /> </td> <input type="hidden" class="r_iID_DuAn_ChiPhiID" value=""/> <input type="hidden" class="r_iID_ChiPhiParentID" /> <input type="hidden" class="r_iID_ChiPhiID" value = "' + x.iID_ChiPhiID + '"/>';
        html += '<td style="text-align:right"><input type="text" disabled style="text-align: right" class="form-control txtGiaTriTruocDieuChinh" value="' + FormatNumber(x.fGiaTriTruocDieuChinh) + '"/> </td>';
        html += '<td style="text-align:right"><input type="text" onblur="UpdateChiPhi(this)" style="text-align: right" class="form-control txtGiaTriChiPhi" onkeyup="ValidateNumberKeyUp(this); " onkeypress="return ValidateNumberKeyPress(this, event); " value="' + FormatNumber(x.fTienPheDuyet) + '"/> </td>';
        html += "<td class='width-150' align='center'>";
        html += "<button class='btn-detail' type = 'button' onclick = 'DetailChiPhi(this)' > " +
            "<i class='fa fa-eye fa-lg' aria-hidden='true'></i>" +
            "</button> ";
        //html += "<button class='btn-add-child btn-icon' type = 'button' onclick = 'ThemMoiChiPhiCon(this)' > " +
        //    "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" +
        //    "</button> ";
        html += "<button class='btn-delete btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_CHI_PHI_DAU_TU + "\")'>" +
            "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
            "</button></td>";
        html += "</tr>";
    });

    $("#tblChiPhiDauTu tbody").append(html);
    ShowHideButtonChiPhi();

}

function UpdateChiPhi(nutCreate) {
    var dongHienTai = nutCreate.parentElement.parentElement;
    var messErr = [];
    var id = $(dongHienTai).data('id');
    var objChiPhiHienTai = arrChiPhiSave.filter(x => x.id == id)[0];

    var sTenChiPhi = $(dongHienTai).find(".r_TenChiPhi").val();
    var sGiaTriChiPhi = $(dongHienTai).find(".txtGiaTriChiPhi").val();
    var fGiaTriChiPhi = 0;
    if (sGiaTriChiPhi != "") {
        fGiaTriChiPhi = parseInt(UnFormatNumber(sGiaTriChiPhi));
    }

    objChiPhiHienTai.sTenChiPhi = sTenChiPhi;
    objChiPhiHienTai.fTienPheDuyet = fGiaTriChiPhi;
    arrChiPhiSave = arrChiPhiSave.filter(function (x) { return x.id != id });

    arrChiPhiSave.push(objChiPhiHienTai)

    ShowHideButtonChiPhi();
    CalculateDataChiPhi(id);
    CalculateTienConLaiNguonVon();
}

function ThemMoiChiPhiCon(nutThem) {
    var dongHienTai = nutThem.closest("tr");
    var idParent = $(dongHienTai).data('id');
    var objParent = arrChiPhiSave.filter(x => x.id == idParent)[0];

    var iThuTu = $(dongHienTai).find(".r_IThuTu").val();
    resChiPhi = [];
    var objChiPhi = {
        id: uuidv4(),
        iID_DuAn_ChiPhi: uuidv4(),
        iID_ChiPhiID: null,
        iID_ChiPhi_Parent: objParent.iID_DuAn_ChiPhi,
        sTenChiPhi: '',
        fTienPheDuyet: null,
        iThuTu: parseInt(iThuTu)
    }

    var dongMoi = "";
    dongMoi += '<tr data-id="' + objChiPhi.id + '" data-iidduanchiphi="' + objChiPhi.iID_DuAn_ChiPhi + '">';
    dongMoi += '<input type="hidden" class="r_IThuTu" value = "' + iThuTu + '"/>';
    dongMoi += '<td style="text-align:left;"><input type="text" onblur="UpdateChiPhi(this)" class="form-control r_TenChiPhi"/></td>';
    dongMoi += '<td style="text-align:right"><input type="text" disabled style="text-align: right" class="form-control txtGiaTriTruocDieuChinh" /></td>';
    dongMoi += '<td style="text-align:right"><input type="text" onblur="UpdateChiPhi(this)" style="text-align: right" class="form-control txtGiaTriChiPhi" onkeyup="ValidateNumberKeyUp(this); " onkeypress="return ValidateNumberKeyPress(this, event); " /></td>';
    dongMoi += "<td class='width-150' align='center'>";

    dongMoi += "<button class='btn-detail' hidden type = 'button' onclick = 'DetailChiPhi(this)' > " +
        "<i class='fa fa-eye fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-add-child btn-icon' type = 'button' onclick = 'ThemMoiChiPhiCon(this)' > " +
        "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-delete btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_CHI_PHI_DAU_TU + "\")'>" +
        "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
        "</button></td>";
    dongMoi += "</tr>";

    $(dongHienTai).after(dongMoi);

    arrChiPhiSave.push(objChiPhi);
    ShowHideButtonChiPhi();
}

function ShowHideButtonChiPhi() {
    arrChiPhiSave.forEach(x => {
        var countChild = arrChiPhiSave.filter(y => y.iID_ChiPhi_Parent == x.iID_DuAn_ChiPhi && (y.isDelete == null || y.isDelete == false));
        if (countChild.length > 0) {
            $("#tblChiPhiDauTu *[data-id='" + x.id + "']").find("button.btn-delete").hide();
            $("#tblChiPhiDauTu *[data-id='" + x.id + "']").find("button.btn-detail").hide();
            $("#tblChiPhiDauTu *[data-id='" + x.id + "']").find(".txtGiaTriChiPhi").attr('disabled', 'disabled');
        } else {
            $("#tblChiPhiDauTu *[data-id='" + x.id + "']").find("button.btn-delete").show();
            $("#tblChiPhiDauTu *[data-id='" + x.id + "']").find("button.btn-detail").show();
            $("#tblChiPhiDauTu *[data-id='" + x.id + "']").find(".txtGiaTriChiPhi").removeAttr('disabled');
            $("#tblChiPhiDauTu *[data-id='" + x.id + "']").find("button.btn-add-child").show();
            var countHangMuc = arrHangMucSave.filter(y => y.iID_DuAn_ChiPhi == x.iID_DuAn_ChiPhi && (y.isDelete == null || y.isDelete == false));
            if (countHangMuc.length > 0) {
                $("#tblChiPhiDauTu *[data-id='" + x.id + "']").find(".txtGiaTriChiPhi").attr('disabled', 'disabled');
                $("#tblChiPhiDauTu *[data-id='" + x.id + "']").find("button.btn-add-child").hide();
            }
        }
    })
}

function CalculateDataChiPhi(itemId) {
    var objItem = arrChiPhiSave.find(x => x.id == itemId);
    if (objItem == undefined) {
        return;
    }
    if (objItem.iID_ChiPhi_Parent != null || objItem.iID_ChiPhi_Parent != "") {
        var objParentItem = arrChiPhiSave.find(x => x.iID_DuAn_ChiPhi == objItem.iID_ChiPhi_Parent);
        if (objParentItem != null) {
            var arrChildSameParent = arrChiPhiSave.filter(function (x) { return x.iID_ChiPhi_Parent == objItem.iID_ChiPhi_Parent && x.iID_ChiPhi_Parent != "" });
            if (arrHasValue(arrChildSameParent)) {
                CalculateTotalParent(objParentItem, arrChildSameParent);
            }
        }

        CalculateDataChiPhi(objItem.iID_ChiPhi_Parent);
    }
}

function CalculateTotalParent(objParentItem, arrChild) {
    var parentId = objParentItem.id;

    var result = 0;
    arrChild.forEach(x => {
        result += x.fTienPheDuyet;
    });

    $('*[data-id="' + parentId + '"]').find('.txtGiaTriChiPhi').val(FormatNumber(result));
    var parentItemNew = objParentItem;
    parentItemNew.fTienPheDuyet = result;
    arrChiPhiSave = arrChiPhiSave.filter(function (x) { return x.id != objParentItem.id });
    arrChiPhiSave.push(parentItemNew);
}

function ValidateBeforeDelete(id) {
    if (isStringEmpty(id)) {
        return;
    }

    var items = GetListDataChiPhi();
    if (!arrHasValue(items)) {
        return;
    }

    var item = items.find(x => x.Id == id);
    if (isStringEmpty(item)) {
        return;
    }

    var child = items.find(x => x.iID_ChiPhiCha == item.Id);
    if (!isStringEmpty(child)) {
        PopupModal("Lỗi xóa chi phí", ["Chi phí " + item.sTenChiPhi + " đang là chi phí cha nên không thể xóa."], ERROR);
        return false;
    }
    return true;
}

function DetailChiPhi(nutDetail) {
    var dongChiPhiHienTai = $(nutDetail).closest("tr");
    var giaTriChiPhi = $(dongChiPhiHienTai).find(".txtGiaTriChiPhi").val();
    var tenChiPhi = $(dongChiPhiHienTai).find(".r_TenChiPhi").val();
    var iIdDuAnChiPhi = $(dongChiPhiHienTai).attr("data-iidduanchiphi");

    $("#txtTenChiPhi").html(tenChiPhi);
    $("#txtIIdDuAnChiPhi").val(iIdDuAnChiPhi);
    $("#txtGiaTriChiPhi").val(giaTriChiPhi);

    // hiển thị hạng mục theo chi phí
    var arrHangMucByChiPhi = arrHangMucSave.filter(x => x.iID_DuAn_ChiPhi == iIdDuAnChiPhi);
    LoadViewHangMuc(arrHangMucByChiPhi.sort(compareSMaOrder), true);
    CalculateTienConLaiHangMuc();
    ShowModalChiTietChiPhi();
}

function ShowModalChiTietChiPhi() {
    $("#modal-listdetailchiphi").modal("show");
}

function HideModalChiTietChiPhi() {
    $("#modal-listdetailchiphi").modal("hide");
}

function SetValueIdChiPhiModal(id) {
    $("#txtIdChiPhiHangMuc").val(id);
}

function GetValueIdChiPhiModal() {
    return $("#txtIdChiPhiHangMuc").val();
}

function PopupModalWithChiTiet(title, message, category) {
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: title, Messages: message, Category: category },
        success: function (data) {
            $("#divModalMessage").html(data);
        }
    });
}
// end chi phí

function LoadViewHangMuc(data) {
    $("#tblHangMucChinh tbody").html("");
    for (var i = 0; i < data.length; i++) {
        var dongMoi = "";
        if (data[i].isDelete == true) {
            var parent = data[i].iID_ParentID ? "child" : "parent";
            dongMoi += `<tr style='cursor: pointer;' class='${parent} error-row' data-xoa='1' data-id='` + data[i].id + "' data-iidduanchiphi='" + data[i].iID_DuAn_ChiPhi + "'>";
            dongMoi += "<td class='r_STT width-100'>" + data[i].sMaOrder + "</td><input type='hidden' class='r_HangMucID' value='" + data[i].iID_HangMucID + "'/>";
            dongMoi += "<td class='r_sTenHangMuc'><input type='text' disabled onblur='UpdateHangMuc(this)' class='form-control txtTenHangMuc' value='" + data[i].sTenHangMuc + "'/></td>"
            //dongMoi += "<td><div class='selectLoaiCongTrinh'>" + CreateHtmlSelectLoaiCongTrinh(data[i].iID_LoaiCongTrinhID, true) + "</div></td>";
            dongMoi += "<td class='r_HanMucDauTu' align='right'><input type='text' style='text-align: right' disabled class='form-control txtGiaTriTruocDieuChinh' value='" + FormatNumber(data[i].fGiaTriTruocDieuChinh) + "'/></td>";
            dongMoi += "<td class='r_HanMucDauTu' align='right'><input type='text' disabled onblur='UpdateHangMuc(this)' style='text-align: right' class='form-control txtHanMucDauTu' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value='" + FormatNumber(data[i].fTienPheDuyet) + "'/></td>";
            dongMoi += "<td align='center' class='width-200'>";
            //if (data[i].iIdDuAnChiPhi === null) {
            dongMoi += "<button class='btn-add-child btn-icon' type = 'button' onclick = 'ThemMoiHangMucCon(this)' > " +
                "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            //}
            dongMoi += "<button class='btn-delete btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_HANG_MUC_CHINH + "\")'>" +
                "<i class='fa fa-trash-o fa-lg' aria-hidden='true'></i>" +
                "</button>";

            dongMoi += "</td></tr>";
        }
        else {
            var parent = data[i].iID_ParentID ? "" : "parent";
            dongMoi += `<tr style='cursor: pointer;' class='${parent}' data-id='` + data[i].id + "' data-iidduanchiphi='" + data[i].iID_DuAn_ChiPhi + "'>";
            dongMoi += "<td class='r_STT width-100'>" + data[i].sMaOrder + "</td><input type='hidden' class='r_HangMucID' value='" + data[i].iID_HangMucID + "'/>";
            dongMoi += "<td class='r_sTenHangMuc'><input type='text' onblur='UpdateHangMuc(this)' class='form-control txtTenHangMuc' value='" + data[i].sTenHangMuc + "'/></td>"
            //dongMoi += "<td><div class='selectLoaiCongTrinh'>" + CreateHtmlSelectLoaiCongTrinh(data[i].iID_LoaiCongTrinhID) + "</div></td>";
            dongMoi += "<td class='r_HanMucDauTu' align='right'></div><input type='text' style='text-align: right' disabled class='form-control txtGiaTriTruocDieuChinh' value='" + FormatNumber(data[i].fGiaTriTruocDieuChinh) + "'/></td>";
            dongMoi += "<td class='r_HanMucDauTu' align='right'></div><input type='text' onblur='UpdateHangMuc(this)' style='text-align: right' class='form-control txtHanMucDauTu' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value='" + FormatNumber(data[i].fTienPheDuyet) + "'/></td>";
            dongMoi += "<td align='center' class='width-200'>";
            //if (data[i].iIdDuAnChiPhi === null) {
            dongMoi += "<button class='btn-add-child btn-icon' type = 'button' onclick = 'ThemMoiHangMucCon(this)' > " +
                "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            //}
            dongMoi += "<button class='btn-delete btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_HANG_MUC_CHINH + "\")'>" +
                "<i class='fa fa-trash-o fa-lg' aria-hidden='true'></i>" +
                "</button>";

            dongMoi += "</td></tr>";
        }

        $("#tblHangMucChinh tbody").append(dongMoi);
    }

    ShowHideButtonHangMuc();
}

function CalculateTienConLaiHangMuc() {
    var chiPhiDuAnId = $("#txtIIdDuAnChiPhi").val();
    var objChiPhi = arrChiPhiSave.filter(x => x.iID_DuAn_ChiPhi == chiPhiDuAnId)[0];
    //var result = giaTriChiPhi;
    var result = 0;

    var arrHMParent = arrHangMucSave.filter(function (x) { return (x.iID_ParentID == "" || x.iID_ParentID == null) && x.iID_DuAn_ChiPhi == chiPhiDuAnId });
    if (arrHasValue(arrHMParent)) {
        arrHMParent.forEach(x => {
            if (x.fTienPheDuyet != null || x.fTienPheDuyet != "") {
                result += x.fTienPheDuyet;
            }
        });
    } else {
        result = objChiPhi.fTienPheDuyet;
    }

    $("#conlaihangmucpheduyet").html(FormatNumber(result));

    // update gia tri phe duyet cho chi phi
    $("#tblChiPhiDauTu tbody [data-id='" + objChiPhi.id + "']").find(".txtGiaTriChiPhi").val(FormatNumber(result));

    objChiPhi.fTienPheDuyet = result;
    arrChiPhiSave = arrChiPhiSave.filter(x => x.id != objChiPhi.id);
    arrChiPhiSave.push(objChiPhi);

    CalculateDataChiPhi(objChiPhi.id);
}

function UpdateHangMuc(nutCreateHangMuc) {
    var dongHienTai = nutCreateHangMuc.closest("tr");
    var id = $(dongHienTai).data('id');
    var iIDParentID = $(dongHienTai).find(".r_HangMucParentID").val();
    var hangMucId = $(dongHienTai).find(".r_HangMucID").val();
    var duAnChiPhiId = $("#txtIIdDuAnChiPhi").val();

    var sTenHangMuc = $(dongHienTai).find(".txtTenHangMuc").val();
    var iIDLoaiCongTrinhID = $(dongHienTai).find(".selectLoaiCongTrinh select").val();
    var sTenLoaiCongTrinh = $(dongHienTai).find(".selectLoaiCongTrinh select :selected").html();
    var sHanMucDauTu = $(dongHienTai).find(".txtHanMucDauTu").val();
    var fHanMucDauTu = sHanMucDauTu == "" ? 0 : parseInt(UnFormatNumber(sHanMucDauTu));

    var objHangMuc = arrHangMucSave.filter(x => x.id == id)[0];
    arrHangMucSave = arrHangMucSave.filter(function (x) {
        if (x.id != id) return x;
        x.sTenHangMuc = sTenHangMuc;
        x.iID_LoaiCongTrinhID = iIDLoaiCongTrinhID;
        x.sTenLoaiCongTrinh = sTenLoaiCongTrinh;
        x.fTienPheDuyet = fHanMucDauTu;
        x.iID_DuAn_ChiPhi = duAnChiPhiId;
        objHangMuc.iID_QDDauTu_DM_HangMucID = x.iID_QDDauTu_DM_HangMucID
        return x;
    });

    //arrHangMucSave.push(objHangMuc);

    CalculateDataHangMucByChiPhi(objHangMuc.iID_QDDauTu_DM_HangMucID);
    CalculateTienConLaiHangMuc();

    CalculateTienConLaiNguonVon();
}



function compareSMaOrder(a, b) {
    var result;

    a = a.sMaOrder.split('-'),
        b = b.sMaOrder.split('-');

    while (a.length) {
        result = a.shift() - (b.shift() || 0);

        if (result) {
            return result;
        }
    }

    return -b.length;
}

function compare(a, b) {
    if (a.sMaOrder < b.sMaOrder) {
        return -1;
    }
    if (a.sMaOrder > b.sMaOrder) {
        return 1;
    }
    return 0;
}

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

function CreateHtmlSelectLoaiCongTrinh(value, isDisabled) {
    var htmlOption = "";
    arr_LoaiCongTrinh.forEach(x => {
        if (value != undefined && value == x.id)
            htmlOption += "<option value='" + x.id + "' selected>" + x.text + "</option>";
        else
            htmlOption += "<option value='" + x.id + "'>" + x.text + "</option>";
    })

    if (isDisabled != undefined && isDisabled == true)
        return "<select disabled onblur='UpdateHangMuc(this)' class='form-control'>" + htmlOption + "</option>";
    else
        return "<select onblur='UpdateHangMuc(this)' class='form-control'>" + htmlOption + "</option>";
}

function ThemMoiHangMuc() {
    var hangMucId = uuidv4();
    var iIdDuAnChiPhi = $("#txtIIdDuAnChiPhi").val();
    var objInfoDongMoi = TaoMaOrderHangMucMoi("");
    var dongMoi = "";
    dongMoi += "<tr style='cursor: pointer;' class='parent' data-id='" + hangMucId + "' data-iidduanchiphi='" + iIdDuAnChiPhi + "'>";
    dongMoi += "<td class='r_STT width-100'>" + objInfoDongMoi.sMaOrder + "</td>";
    dongMoi += "<td class='r_sTenHangMuc'><input type='text' onblur='UpdateHangMuc(this)' class='form-control txtTenHangMuc'/></td>"
    //dongMoi += "<td><div class='selectLoaiCongTrinh'>" + CreateHtmlSelectLoaiCongTrinh() + "</div></td>";
    dongMoi += "<td class='r_HanMucDauTu' align='right'><input type='text' disabled style='text-align: right' class='form-control txtGiaTriTruocDieuChinh' /></td>";
    dongMoi += "<td class='r_HanMucDauTu' align='right'><input type='text' onblur='UpdateHangMuc(this)' style='text-align: right' class='form-control txtHanMucDauTu' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' /></td>";
    dongMoi += "<td align='center' class='width-200'>";
    if (arrHangMucSave.filter(x => x.iID_DuAn_ChiPhi == iIdDuAnChiPhi).length == 0) {
        dongMoi += "<button class='btn-add-child btn-icon' type = 'button' onclick = 'ThemMoiHangMucCon(this)' > " +
            "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" +
            "</button> ";
    }
    dongMoi += "<button class='btn-delete btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_HANG_MUC_CHINH + "\")'>" +
        "<i class='fa fa-trash-o fa-lg' aria-hidden='true'></i>" +
        "</button></td>";
    dongMoi += "</tr>";

    if (objInfoDongMoi.indexRow < 0)
        $("#tblHangMucChinh > tbody").append(dongMoi);
    else
        $("#tblHangMucChinh > tbody > tr").eq(objInfoDongMoi.indexRow).after(dongMoi);

    arrHangMucSave.push({
        id: hangMucId,
        iID_DuAn_ChiPhi: iIdDuAnChiPhi,
        iID_HangMucID: null,
        iID_LoaiCongTrinhID: null,
        iID_QDDauTu_DM_HangMucID: uuidv4(),
        iID_ParentID: null,
        fTienPheDuyet: 0,
        sMaHangMuc: "",
        sTenHangMuc: "",
        sMaOrder: objInfoDongMoi.sMaOrder
    })
    ShowHideButtonChiPhi();
}

function ThemMoiHangMucCon(nutThem) {
    var dongHienTai = nutThem.closest("tr");
    var idHangMucHienTai = $(dongHienTai).attr("data-id");
    var objHangMucHienTai = arrHangMucSave.filter(x => x.id == idHangMucHienTai)[0];

    var objInfoDongMoi = TaoMaOrderHangMucMoi(objHangMucHienTai.iID_QDDauTu_DM_HangMucID);
    var hangMucId = uuidv4();
    var iIdDuAnChiPhi = $("#txtIIdDuAnChiPhi").val();
    var dongMoi = "";
    dongMoi += "<tr style='cursor: pointer;' data-id='" + hangMucId + "' data-iidduanchiphi='" + iIdDuAnChiPhi + "'>";
    dongMoi += "<td class='r_STT width-100'>" + objInfoDongMoi.sMaOrder + "</td>";
    dongMoi += "<td class='r_sTenHangMuc'><input type='text' onblur='UpdateHangMuc(this)' class='form-control txtTenHangMuc'/></td>"
    //dongMoi += "<td><div class='selectLoaiCongTrinh'>" + CreateHtmlSelectLoaiCongTrinh() + "</div></td>";
    dongMoi += "<td class='r_GiaTriTruocDieuChinh' align='right'><input type='text' disabled style='text-align: right' class='form-control txtGiaTriTruocDieuChinh'/></td>";
    dongMoi += "<td class='r_HanMucDauTu' align='right'><input type='text' onblur='UpdateHangMuc(this)' style='text-align: right' class='form-control txtHanMucDauTu' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);'/></td>";
    dongMoi += "<td align='center' class='width-200'>";

    dongMoi += "<button class='btn-add-child btn-icon' type='button' onclick='ThemMoiHangMucCon(this)' > " +
        "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" +
        "</button> ";

    dongMoi += "<button class='btn-delete btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_HANG_MUC_CHINH + "\")'>" +
        "<i class='fa fa-trash-o fa-lg' aria-hidden='true'></i>" +
        "</button></td>";
    dongMoi += "</tr>";

    $("#tblHangMucChinh > tbody > tr").eq(objInfoDongMoi.indexRow).after(dongMoi);

    arrHangMucSave.push({
        id: hangMucId,
        iID_DuAn_ChiPhi: iIdDuAnChiPhi,
        iID_HangMucID: null,
        iID_LoaiCongTrinhID: null,
        iID_QDDauTu_DM_HangMucID: uuidv4(),
        iID_ParentID: objHangMucHienTai.iID_QDDauTu_DM_HangMucID,
        fTienPheDuyet: 0,
        sMaHangMuc: "",
        sTenHangMuc: "",
        sMaOrder: objInfoDongMoi.sMaOrder
    })

    // disable hang muc cha
    //ShowHideButtonHangMuc();
}

function ShowHideButtonHangMuc() {
    arrHangMucSave.forEach(x => {
        var countChild = arrHangMucSave.filter(y => y.iID_ParentID == x.iID_QDDauTu_DM_HangMucID && (y.isDelete == null || y.isDelete == false));
        if (countChild.length > 0) {
            $("#tblHangMucChinh *[data-id='" + x.id + "']").find("button.btn-delete").hide();
            $("#tblHangMucChinh *[data-id='" + x.id + "']").find(".txtTenHangMuc, select, .txtHanMucDauTu").attr('disabled', 'disabled');
        } else {
            $("#tblHangMucChinh *[data-id='" + x.id + "']").find("button.btn-delete").show();
            $("#tblHangMucChinh *[data-id='" + x.id + "']").find("button.btn-detail").show();
            $("#tblHangMucChinh *[data-id='" + x.id + "']").find(".txtGiaTriChiPhi").removeAttr('disabled');

            var countHangMuc = arrHangMucSave.filter(y => y.iID_DuAn_ChiPhi == x.iID_DuAn_ChiPhi && (y.isDelete == null || y.isDelete == false));
            if (countHangMuc.length > 0) {
                $("#tblChiPhiDauTu *[data-id='" + x.id + "']").find(".txtGiaTriChiPhi").attr('disabled', 'disabled');
            }
        }
    })
}

function TaoMaOrderHangMucMoi(parentId) {
    var iIdDuAnChiPhi = $("#txtIIdDuAnChiPhi").val();
    var sMaOrder = "";
    var indexRow = -1;
    var arrHangMucOrder = arrHangMucSave.filter(x => x.iID_DuAn_ChiPhi == "" || x.iID_DuAn_ChiPhi == null || x.iID_DuAn_ChiPhi == iIdDuAnChiPhi).sort(compareSMaOrder);
    if (parentId == "" || parentId == null) {
        var arrHangMucParent = arrHangMucOrder.filter(x => x.iID_ParentID == null || x.iID_ParentID == "");
        var sMaOrderLast = "0";
        if (arrHangMucParent != null && arrHangMucParent.length > 0)
            sMaOrderLast = arrHangMucParent[arrHangMucParent.length - 1].sMaOrder;
        sMaOrder = (parseInt(sMaOrderLast) + 1).toString();
        indexRow = arrHangMucOrder.findLastIndex(x => x.sMaOrder.startsWith(sMaOrderLast));
    } else {
        var objParent = arrHangMucOrder.filter(x => x.iID_QDDauTu_DM_HangMucID == parentId)[0];
        var arrHangMucChild = arrHangMucOrder.filter(x => x.iID_ParentID == parentId);
        if (arrHangMucChild.length == 0) {
            sMaOrder = objParent.sMaOrder + "-1";
            indexRow = arrHangMucOrder.findLastIndex(x => x.sMaOrder.startsWith(objParent.sMaOrder));
        } else {
            var sMaOrderLast = arrHangMucChild[arrHangMucChild.length - 1].sMaOrder;
            var arrMaOrderSplit = sMaOrderLast.split("-");
            sMaOrder = objParent.sMaOrder + "-" + (parseInt(arrMaOrderSplit[arrMaOrderSplit.length - 1]) + 1).toString();
            indexRow = arrHangMucOrder.findLastIndex(x => x.sMaOrder.startsWith(sMaOrderLast));
        }
    }
    return {
        sMaOrder: sMaOrder,
        indexRow: indexRow
    }
}

function CalculateDataHangMucByChiPhi(itemId) {
    var objItem = arrHangMucSave.find(x => x.iID_QDDauTu_DM_HangMucID == itemId);
    if (objItem == undefined || objItem.iID_ParentID == "" || objItem.iID_ParentID == null) {
        return;
    }
    var objParentItem = arrHangMucSave.find(x => x.iID_QDDauTu_DM_HangMucID == objItem.iID_ParentID);
    var arrChildSameParent = arrHangMucSave.filter(function (x) { return x.iID_ParentID == objItem.iID_ParentID && x.iID_ParentID != "" && (x.isDelete == undefined || x.isDelete == false) });
    if (arrHasValue(arrChildSameParent)) {
        CalculateTotalParentHangMuc(objParentItem, arrChildSameParent);
    }

    if (objParentItem.iID_ParentID != "" && objParentItem.iID_ParentID != null) {
        CalculateDataHangMucByChiPhi(objParentItem.iID_QDDauTu_DM_HangMucID, arrHangMuc);
    }
}

function CalculateTotalParentHangMuc(objParentItem, arrChild) {
    var result = 0;
    arrChild.forEach(x => {
        result += x.fTienPheDuyet;
    });

    $("#tblHangMucChinh [data-id='" + objParentItem.id + "']").find('.txtHanMucDauTu').val(FormatNumber(result));
    objParentItem.fTienPheDuyet = result;
    objParentItem.iID_DuAn_ChiPhi = arrChild[0].iID_DuAn_ChiPhi;
    arrHangMucSave = arrHangMucSave.filter(x => x.id != objParentItem.id);
    arrHangMucSave.push(objParentItem);
}

// Nguồn vốn đầu tư
function SetValueTMTDPheDuyetTheoNguonVon() {
    var result = 0;
    if (arrHasValue(arrNguonVonSave)) {
        arrNguonVonSave.forEach(x => {
            if (x.isDelete == undefined || x.isDelete == false)
                result += x.fTienPheDuyet;
        });
    }
    $("#fTongMucDauTuPheDuyet").val(FormatNumber(result));
}


function ThemMoiNguonVon() {
    var objNguonVon = {
        id: uuidv4(),
        iID_NguonVonID: null,
        fTienPheDuyet: 0
    }

    var dongMoi = "";
    dongMoi += "<tr data-id='" + objNguonVon.id + "'>";
    dongMoi += "<td class='r_STT width-50'>" + "</td>";
    dongMoi += "<td><input type='hidden' class='r_iID_NguonVonID' /><div class='sTenNguonVon' hidden></div><div class='selectNguonVon'>" + CreateHtmlSelectNguonVon() + "</div></td>";
    dongMoi += "<td class='r_TienPheDuyetCTDT'><input type='text' disabled style='text-align: right' class='form-control txtTienPheDuyetCTDT sotien'/></td>";
    dongMoi += "<td class='r_GiaTriTruocDieuChinh'><input type='text' disabled style='text-align: right' class='form-control txtGiaTriTruocDieuChinh sotien'/></td>";
    dongMoi += "<td class='r_GiaTriNguonVon'><input type='text' onblur='UpdateNguonVon(this)' style='text-align: right' class='form-control txtGiaTriNguonVon' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);'/></td>";
    dongMoi += "<td class='width-150' align='center'>";
    dongMoi += "<button class='btn-delete btn-icon' type = 'button' onclick='XoaDong(this, \"" + TBL_NGUON_VON_DAU_TU + "\")' > " +
        "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span></button></td>";
    dongMoi += "</tr>";

    $("#" + TBL_NGUON_VON_DAU_TU + " tbody").append(dongMoi);
    CapNhatCotSttNguonVon();

    arrNguonVonSave.push(objNguonVon);
}

function CapNhatCotSttNguonVon() {
    $("#" + TBL_NGUON_VON_DAU_TU + " tbody tr").each(function (index, tr) {
        $(tr).find('.r_STT').text(index + 1);
    });
}

function UpdateNguonVon(nutCreate) {
    var dongHienTai = $(nutCreate).closest('tr');

    var iID_NguonVonID = $(dongHienTai).find(".selectNguonVon select").val();
    var id = $(dongHienTai).data('id');

    var sHanMucDauTu = $(dongHienTai).find(".txtGiaTriNguonVon").val();
    var giaTriPheDuyet = sHanMucDauTu == "" ? 0 : parseInt(UnFormatNumber(sHanMucDauTu));
    arrNguonVonSave = arrNguonVonSave.filter(function (x) {
        if (x.id != id) return x;
        x.iID_NguonVonID = iID_NguonVonID,
            x.sHanMucDauTu = sHanMucDauTu,
            x.fTienPheDuyet = giaTriPheDuyet
        return x;

    });

    //    arrNguonVonSave.push({
    //        id: id,
    //        iID_NguonVonID: iID_NguonVonID,
    //        fTienPheDuyet: giaTriPheDuyet
    //    })


    CalculateTienConLaiNguonVon();
    SetValueTMTDPheDuyetTheoNguonVon();
}

function LoadDataViewNguonVon() {
    if (arrNguonVonSave != null) {
        for (var i = 0; i < arrNguonVonSave.length; i++) {
            var dongMoi = "";
            dongMoi += "<tr data-id='" + arrNguonVonSave[i].id + "'>";
            dongMoi += "<td class='r_STT width-50'>" + (i + 1) + "</td>";
            dongMoi += "<td><input type='hidden' class='r_iID_NguonVonID' value='" + arrNguonVonSave[i].iID_NguonVonID + "'/><div class='selectNguonVon'>" + CreateHtmlSelectNguonVon(arrNguonVonSave[i].iID_NguonVonID) + "</div></td>";
            dongMoi += "<td class='r_TienPheDuyetCTDT'><input type='text' style='text-align: right' disabled class='form-control txtTienPheDuyetCTDT sotien' value='" + FormatNumber(arrNguonVonSave[i].fTienPheDuyetCTDT) + "'/></td>"
            //dongMoi += "<td class='r_GiaTriTruocDieuChinh'><input type='text' style='text-align: right' disabled class='form-control txtGiaTriTruocDieuChinh sotien' value='" + FormatNumber(arrNguonVonSave[i].fGiaTriTruocDieuChinh) + "'/></td>"
            dongMoi += "<td class='r_GiaTriTruocDieuChinh'><input type='text' style='text-align: right' disabled class='form-control txtGiaTriTruocDieuChinh sotien' value='" + FormatNumber(arrNguonVonSave[i].fTienPheDuyet) + "'/></td>"
            dongMoi += "<td class='r_GiaTriNguonVon'><input type='text' onblur='UpdateNguonVon(this)' style='text-align: right' class='form-control txtGiaTriNguonVon' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value='" + FormatNumber(arrNguonVonSave[i].fTienPheDuyet) + "'/></td>"
            dongMoi += "<td class='width-150' align='center'>";
            dongMoi += "<button class='btn-delete btn-icon' type = 'button' onclick='XoaDong(this, \"" + TBL_NGUON_VON_DAU_TU + "\")' > " +
                "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span></button>";
            dongMoi += "</td>";
            dongMoi += "</tr>";

            $("#" + TBL_NGUON_VON_DAU_TU + " tbody").append(dongMoi);
            CalculateTienConLaiNguonVon();
        }
    }
}

function CreateHtmlSelectNguonVon(value) {
    var htmlOption = "";
    arr_NguonVon_Dropdown.forEach(x => {
        if (value != undefined && value == x.id)
            htmlOption += "<option value='" + x.id + "' selected>" + x.text + "</option>";
        else
            htmlOption += "<option value='" + x.id + "'>" + x.text + "</option>";
    })

    return "<select onblur='UpdateNguonVon(this)' class='form-control'>" + htmlOption + "</option>";
}

function CalculateTienConLaiNguonVon() {
    var result = 0;
    if (arrHasValue(arrNguonVonSave)) {
        arrNguonVonSave.forEach(x => {
            if ((x.isDelete == undefined || x.isDelete == false) && !isStringEmpty(x.fTienPheDuyet)) {
                result += parseFloat(x.fTienPheDuyet);
            }
        });
    }

    var listChiPhi = arrChiPhiSave.filter(function (x) { return x.iID_ChiPhi_Parent == "" || x.iID_ChiPhi_Parent == null });
    if (arrHasValue(listChiPhi)) {
        listChiPhi.forEach(x => {
            if ((x.isDelete == undefined || x.isDelete == false) && !isStringEmpty(x.fTienPheDuyet)) {
                result -= parseFloat(x.fTienPheDuyet);
            }
        });
    }
    $("#tonggiatriconlainguonvon").html(result == 0 ? 0 : FormatNumber(result));
    return result;
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

function XoaDong(nutXoa, idBang) {
    var dongXoa = nutXoa.parentElement.parentElement;
    var id = $(dongXoa).data('id');
    var checkXoa = $(dongXoa).attr('data-xoa');
    if (checkXoa == 1) {
        $(dongXoa).attr('data-xoa', '0');
        $(dongXoa).removeClass('error-row');
    } else {
        $(dongXoa).attr('data-xoa', '1');
        $(dongXoa).addClass('error-row');
    }

    if (idBang == TBL_CHI_PHI_DAU_TU) {
        var objXoa = arrChiPhiSave.filter(function (x) { return x.id == id })[0];
        objXoa.isDelete = !objXoa.isDelete;
        arrChiPhiSave = arrChiPhiSave.filter(function (x) { return x.id != id });
        arrChiPhiSave.push(objXoa);

        if (checkXoa == 1) {
            $(dongXoa).find('input, select').prop("disabled", "");
        } else {
            $(dongXoa).find('input, select').prop("disabled", "disabled");
        }

        ShowHideButtonChiPhi();
    } else if (idBang == TBL_NGUON_VON_DAU_TU) {
        var objXoa = arrNguonVonSave.filter(function (item) { return item.id == id; })[0];
        objXoa.isDelete = !objXoa.isDelete;
        arrNguonVonSave = arrNguonVonSave.filter(function (item) { return item.id != id; });
        arrNguonVonSave.push(objXoa);

        if (checkXoa == 1) {
            $(dongXoa).find('input:not(.txtTienPheDuyetCTDT, .txtGiaTriTruocDieuChinh ), select').prop("disabled", "");
        } else {
            $(dongXoa).find('input, select').prop("disabled", "disabled");
        }

        SetValueTMTDPheDuyetTheoNguonVon();
    } else if (idBang == TBL_HANG_MUC_CHINH) {
        var objXoa = arrHangMucSave.filter(function (item) { return item.id == id; })[0];
        objXoa.isDelete = !objXoa.isDelete;
        arrHangMucSave = arrHangMucSave.filter(function (item) { return item.id != id; });
        arrHangMucSave.push(objXoa);

        if (checkXoa == 1) {
            $(dongXoa).find('input:not(.txtGiaTriTruocDieuChinh), select').prop("disabled", "");
        } else {
            $(dongXoa).find('input, select').prop("disabled", "disabled");
        }

        CalculateDataHangMucByChiPhi(objXoa.iID_QDDauTu_DM_HangMucID);
        CalculateTienConLaiHangMuc();
    }
    CalculateTienConLaiNguonVon();
}

function DinhDangSo() {
    $(".sotien").each(function () {
        $(this).val(FormatNumber($(this).val()));
    })
}

// trim space for all text input
function trimspaceText(target) {
    let originalVal = $(target).val();
    $(target).val(originalVal.trim());
}

$("input[type=text]").blur((e) => {
    trimspaceText(e.target);
})