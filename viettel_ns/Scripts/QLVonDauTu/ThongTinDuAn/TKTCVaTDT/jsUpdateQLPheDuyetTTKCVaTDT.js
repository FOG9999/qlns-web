var TBL_CHI_PHI_DAU_TU = "tblChiPhiDauTu";
var TBL_NGUON_VON_DAU_TU = "tblNguonVonDauTu";
var TBL_TAI_LIEU_DINH_KEM = "tblThongTinTaiLieuDinhKem";
var TBL_HANG_MUC_CHINH = "tblHangMucChinh";
var arrChiPhi = [];
var arrChiPhiSave = [];
var arrNguonVon = [];
var arrNguonVonSave = [];
var arrHangMuc = [];
var arrHangMucSave = [];
var arrHangMucTheoDuAn = [];
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var tCpdtGiaTriToTrinh = 0, tCpdtGiaTriThamDinh = 0, tCpdtGiaTriPheDuyet = 0;
var tNvdtGiaTriToTrinh = 0, tNvdtGiaTriThamDinh = 0, tNvdtGiaTriPheDuyet = 0;
var ERROR = 1;
var CONFIRM = 0;
var tongNguonVonBanDau = 0;

$(document).ready(function () {
    LoadDataComboBoxNguonVon();
    GetDataNguonVonByJson();
    GetDataChiPhiByJson();
    GetDataHangMucByJson();
    // Tính tổng nguồn vốn ban đầu

    Event();
});


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
    if (idBang == TBL_CHI_PHI_DAU_TU) {
        var rIIDChiPhi = $(dongXoa).find(".r_iID_ChiPhi").val();
        arrChiPhi = arrChiPhi.filter(function (x) { return x.iID_ChiPhiID != rIIDChiPhi });
    } else if (idBang == TBL_NGUON_VON_DAU_TU) {
        var rIIDNguonVon = $(dongXoa).find(".r_iID_NguonVon").val();
        arrNguonVon = arrNguonVon.filter(function (x) { return x.iID_NguonVonID != rIIDNguonVon });
    }
    dongXoa.parentNode.removeChild(dongXoa);
    CapNhatCotStt(idBang);
    TinhLaiDongTong(idBang);
}

function CheckLoi(doiTuong) {
    var messErr = [];
    if (tCpdtGiaTriToTrinh != 0 && tCpdtGiaTriToTrinh != tNvdtGiaTriToTrinh)
        messErr.push("Giá trị tờ trình của danh sách chi phí và nguồn vốn không bằng nhau");
    if (tCpdtGiaTriThamDinh != 0 && tCpdtGiaTriThamDinh != tNvdtGiaTriThamDinh)
        messErr.push("Giá trị thẩm định của danh sách chi phí và nguồn vốn không bằng nhau");
    if (tCpdtGiaTriPheDuyet != 0 && tCpdtGiaTriPheDuyet != tNvdtGiaTriPheDuyet)
        messErr.push("Giá trị phê duyệt của danh sách chi phí và nguồn vốn không bằng nhau");
    if (KiemTraTrungSoQuyetDinh(doiTuong.sSoQuyetDinh, doiTuong.iID_DuToanID) == true)
        messErr.push("Số quyết định đã tồn tại");
    if (doiTuong.sSoToTrinh == "")
        messErr.push("Số tờ trình không được để trống");
    if (doiTuong.dNgayToTrinh == "")
        messErr.push("Ngày tờ trình không được để trống hoặc sai định dạng");
    if (doiTuong.sSoQuyetDinh == "")
        messErr.push("Số quyết định không được để trống");
    if (doiTuong.dNgayQuyetDinh == "")
        messErr.push("Ngày phê duyệt không được để trống hoặc sai định dạng");
    if (messErr.length > 0) {
        alert(messErr.join("\n"));
        return false;
    } else {
        return true;
    }
}

function Save() {
    var pheduyetDuToan = {};

    pheduyetDuToan.iID_DuToanID = $("#iIDDuToanId").val();
    //Thông tin tờ trình
    pheduyetDuToan.sSoToTrinh = $("#sSoToTrinh").val();
    pheduyetDuToan.dNgayToTrinh = $("#dNgayToTrinh").val();

    //Thông tin thẩm định
    pheduyetDuToan.sSoThamDinh = $("#sSoThamDinh").val();
    pheduyetDuToan.dNgayThamDinh = $("#dNgayThamDinh").val();
    pheduyetDuToan.sCoQuanThamDinh = $("#sCoQuanThamDinh").val();

    //Thông tin phê duyệt
    pheduyetDuToan.sSoQuyetDinh = $("#sSoQuyetDinh").val();
    pheduyetDuToan.dNgayQuyetDinh = $("#dNgayQuyetDinh").val();
    pheduyetDuToan.sCoQuanPheDuyet = $("#sCoQuanPheDuyet").val();
    pheduyetDuToan.sNguoiKy = $("#sNguoiKy").val();

    //Chi phí đầu tư
    pheduyetDuToan.listDuToanChiPhi = arrChiPhi;

    //Nguồn vốn đầu tư
    pheduyetDuToan.listDuToanNguonVon = arrNguonVon;

    if (CheckLoi(pheduyetDuToan)) {
        $.ajax({
            url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/Save",
            type: "POST",
            data: { model: pheduyetDuToan },
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data.status == true) {
                    window.location.href = "/QLVonDauTu/QLPheDuyetTKTCVaTDT";
                }
            },
            error: function (data) {

            }
        })
    }
}

function KiemTraTrungSoQuyetDinh(sSoQuyetDinh, iID_DuToanID) {
    var check = false;
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/KiemTraTrungSoQuyetDinh",
        type: "POST",
        data: { sSoQuyetDinh: sSoQuyetDinh, iID_DuToanID: iID_DuToanID },
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

function Cancel() {
    window.location.href = "/QLVonDauTu/QLPheDuyetTKTCVaTDT/Index";
}

// Hàm chung
async function Event() {
    LoadDataViewNguonVon();
    LoadDataViewChiPhi();
    
    LoadDataComboBoxLoaiCongTrinh();
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

function SetSelectById(id) {
    $("#" + id).select({
        width: 'resolve',
        matcher: matchStart
    });
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

// End hàm chung

// Nguồn vốn thiết kế thi công

function GetDataNguonVonByJson() {
    var duToanId = $("#iIDDuToanId").val();
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/GetListNguonVonByTKTC",
        type: "POST",
        data: { id: duToanId },
        dataType: "json",
        cache: false,
        success: function (resp) {
            if (resp.status == false) {
                return;
            }

            if (arrHasValue(resp.data)) {
                arrNguonVon = resp.data;
               
                for (var i = 0; i < arrNguonVon.length; i++) {
                    arrNguonVon[i].id = (i + 1).toString();
                }
                arrNguonVonSave = arrNguonVon;
            }

            LoadDataViewNguonVon();
        }
    });

}

function GetDataChiPhiByJson() {
    var duToanId = $("#iIDDuToanId").val();
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/GetListChiPhiByTKTC",
        type: "POST",
        data: { id: duToanId },
        dataType: "json",
        cache: false,
        success: function (resp) {
            if (resp.status == false) {
                return;
            }

            if (arrHasValue(resp.data)) {
                arrChiPhi = resp.data;
                arrChiPhiSave = resp.data;
            }
            UpdateChiPhiCanEdit(arrChiPhi);
            LoadDataViewChiPhi();
        }
    });

}

function UpdateChiPhiCanEdit(arrayChiPhiParam) {

    arrayChiPhiParam.forEach(obj => {
        var checkIsParent = CheckChiPhiIsParent(obj.iID_DuAn_ChiPhi);
        if (checkIsParent) {
            obj.isEdit = false;
        } else {
            obj.isEdit = true;
        }

        if (obj.iID_ChiPhi_Parent == null) {
            obj.iID_ChiPhi_Parent = "";
        }

    });
}

function CheckChiPhiIsParent(itemId) {

    var listChild = [];
    listChild = arrChiPhi.filter(function (x) { return x.iID_ChiPhi_Parent == itemId });
    if (arrHasValue(listChild)) {
        return true;
    } else {
        return false;
    }
}



function GetListDataNguonVon() {
    return arrNguonVon;
}

function LoadDataViewNguonVon() {
    var data = GetListDataNguonVon();

    if (data != null) {
        for (var i = 0; i < data.length; i++) {
            var dongMoi = "";
            dongMoi += "<tr>";
            dongMoi += "<td class='r_STT'>" + (i + 1) + "</td>";
            dongMoi += "<td><input type='hidden' class='r_iID_NguonVonID' value='" + data[i].iID_NguonVonID + "'/><div class='sTenNguonVon' hidden></div><div class='selectNguonVon'>" + CreateHtmlSelectNguonVon(data[i].iID_NguonVonID) + "</div></td>";
            dongMoi += "<td class='r_GiaTriNguonVon' ><div hidden></div><input type='text' disabled style='text-align: right' class='form-control txtGiaTriNguonVon' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value='" + FormatNumber(data[i].fTienPheDuyet) + "'/></td>"
            dongMoi += "<td align='center'>";
            dongMoi += "<button class='btn-save btn-icon' hidden  type = 'button' onclick = 'CreateNguonVon(this, \"" + TBL_NGUON_VON_DAU_TU + "\")'> " +
                "<i class='fa fa-floppy-o fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            dongMoi += "<button class='btn-edit btn-icon' type = 'button' onclick = 'EditNguonVon(this, \"" + TBL_NGUON_VON_DAU_TU + "\")'> " +
                "<i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            dongMoi += "<button class='btn-delete btn-icon' type = 'button' onclick='XoaDong(this, \"" + TBL_NGUON_VON_DAU_TU + "\")' > " +
                "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span></button> <input type='hidden' class='r_iID_ChuTruongNguonVonId' value='" + data[i].iID_ChuTruongDauTu_NguonVonID + "'/> <input type='hidden' class='r_iID_ChuTruongId' value='" + data[i].iID_ChuTruongDauTuID + "'/></td>";
            dongMoi += "</tr>";

            $("#tblNguonVonDauTu tbody").append(dongMoi);

            //LoadDataViewTongTienNguonVon();
            CalculateTienConLaiNguonVon();
        }
    }

}

function CreateNguonVon(nutCreate, idBang) {
    var messErr = [];
    var dongHienTai = nutCreate.parentElement.parentElement;

    var iID_NguonVonID = $(dongHienTai).find(".selectNguonVon select").val();
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
    if (arrNguonVonSave.filter(function (x) { return x.iID_NguonVonID == iID_NguonVonID && x.id != id }).length > 0) {
        messErr.push("Nguồn vốn đã tồn tại !");
    }

    if (messErr.length > 0) {
        PopupModal("Lỗi", messErr, ERROR);
        return;
    }

    arrNguonVonSave = arrNguonVonSave.filter(function (x) { return x.id != id });

    $(dongHienTai).find(".selectNguonVon select").attr("disabled", true);
    $(dongHienTai).find(".txtGiaTriNguonVon").attr("disabled", true);

    $(dongHienTai).find("button.btn-edit").show();
    $(dongHienTai).find("button.btn-save").hide();

    arrNguonVonSave.push({
        id: id,
        //iID_QDDauTu_NguonVonID: iID_ChuTruongDauTuID,
        iID_NguonVonID: iID_NguonVonID,
        fGiaTriTruocDieuChinh: giaTriTruocDC,
        fTienPheDuyet: giaTriPheDuyet,
        fGiaTriDieuChinh: giaTriDC
    })

    CalculateTienConLaiNguonVon();
}

function EditNguonVon(nutCreateHangMuc, idBang) {
    var dongHienTai = nutCreateHangMuc.parentElement.parentElement;
    $(dongHienTai).find(".selectNguonVon select").attr("disabled", false);
    $(dongHienTai).find(".txtGiaTriNguonVon").attr("disabled", false);

    $(dongHienTai).find("button.btn-edit").hide();
    $(dongHienTai).find("button.btn-save").show();
}

function ThemMoiNguonVon() {
    var dongMoi = "";
    dongMoi += "<tr>";
    dongMoi += "<td class='r_STT'>" + "</td>";
    dongMoi += "<td><input type='hidden' class='r_iID_NguonVonID' /><div class='sTenNguonVon' hidden></div><div class='selectNguonVon'>" + CreateHtmlSelectNguonVon() + "</div></td>";
    dongMoi += "<td class='r_GiaTriNguonVon' ><div hidden></div><input type='text' style='text-align: right' class='form-control txtGiaTriNguonVon' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);'/></td>"
    dongMoi += "<td align='center'>";
    dongMoi += "<button class='btn-save btn-icon' type = 'button' onclick = 'CreateNguonVon(this, \"" + TBL_NGUON_VON_DAU_TU + "\")'> " +
        "<i class='fa fa-floppy-o fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-edit btn-icon' hidden type = 'button' onclick = 'EditNguonVon(this, \"" + TBL_NGUON_VON_DAU_TU + "\")'> " +
        "<i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-delete btn-icon' type = 'button' onclick='XoaDong(this, \"" + TBL_NGUON_VON_DAU_TU + "\")' > " +
        "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span></button></td>";
    dongMoi += "</tr>";

    $("#" + TBL_NGUON_VON_DAU_TU + " tbody").append(dongMoi);
    CapNhatCotSttNguonVon();
}

function CapNhatCotSttNguonVon() {
    $("#" + TBL_NGUON_VON_DAU_TU + " tbody tr").each(function (index, tr) {
        $(tr).find('.r_STT').text(index + 1);
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

    if (value != undefined && value != "") {
        return "<select disabled class='form-control'>" + htmlOption + "</option>";
    } else {
        return "<select class='form-control'>" + htmlOption + "</option>";
    }
}

function CalculateTienConLaiNguonVon() {
    var result = 0;
    var listNguonVon = arrNguonVonSave;
    if (arrHasValue(listNguonVon)) {
        listNguonVon.forEach(x => {
            if (!isStringEmpty(x.fTienPheDuyet)) {
                result += parseFloat(x.fTienPheDuyet);
            }
        });
    }

    var listChiPhi = arrChiPhiSave.filter(function (x) { return x.iID_ChiPhi_Parent == "" });
    if (arrHasValue(listChiPhi)) {
        listChiPhi.forEach(x => {
            if (!isStringEmpty(x.fTienPheDuyet)) {
                result -= parseFloat(x.fTienPheDuyet);
            }
        });
    }

    $("#tonggiatriconlainguonvon").html(FormatNumber(result));

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


function ValidateTongTienChiPhiVoiNguonVon(item) {
    if (isStringEmpty(item.fTienPheDuyet)) {
        return true;
    }

    var soTien = 0;
    var listNguonVon = GetListDataNguonVon();
    if (arrHasValue(listNguonVon)) {
        listNguonVon.forEach(x => {
            soTien += parseFloat(x.fTienPheDuyet);
        });
    }

    soTien -= parseFloat(UnFormatNumber(item.fTienPheDuyet));

    var listChiPhi = GetListDataChiPhi();
    if (arrHasValue(listChiPhi)) {
        listChiPhi.forEach(x => {
            if (x.Id != item.Id) {
                soTien -= parseFloat(x.fTienPheDuyet);
            }
        });
    }

    if (soTien < 0) {
        PopupModal("Lỗi thêm chi phí", ["Tổng tiền chi phí đã vượt quá tổng tiền nguồn vốn !"], ERROR);
        return false;
    }

    return true;
}

function ValidateBeforeHaveChildWithTienHangMucChiPhi(item) {
    if (isStringEmpty(item)) {
        return true;
    }

    if (isStringEmpty(item.Id)) {
        return true;
    }

    var items = GetListDataChiPhi();
    if (arrHasValue(items)) {
        var check = items.find(x => x.iID_ChiPhiCha == item.Id);
        if (!isStringEmpty(check)) {
            return false;
        }
    }

    return true;
}

function ValidateBeforeSaveChiTietChiPhiTienHangMucOld(items) {
    if (!arrHasValue(items)) {
        return true;
    }

    var message = [];

    items.forEach(x => {
        if (isStringEmpty(x.sHangMucCha)) {
            var tongTien = GetTongTienHangMucChiPhiChild(x.sMaHangMuc);
            if (x.fTienHangMucCheckValidate < tongTien) {
                message.push("Hạng mục " + x.sTenHangMuc + " đã có tổng tiền mới " + FormatNumber(tongTien) + " vượt quá tổng tiền cũ " + FormatNumber(x.fTienHangMucCheckValidate));
            }
        }
    });

    if (message.length > 0) {
        PopupModalWithChiTiet("Lỗi lưu hạng mục chi phí", message, ERROR);
        return false;
    }

    return true;
}

function AddChiPhi(item) {
    if (item == null || item == undefined) {
        return;
    }

    item.fTienPheDuyet = parseFloat(UnFormatNumber(item.fTienPheDuyet));

    if (isStringEmpty(item.Id)) {
        item.Id = uuidv4();
        arrChiPhi.push(item);
        return;
    }

    var items = GetListDataChiPhi();

    if (!isStringEmpty(item.iID_ChiPhiCha)) {
        if (arrHasValue(items)) {
            items.forEach(x => {
                if (!isStringEmpty(item.Id)) {
                    if (x.iID_ChiPhiCha == item.Id) {
                        item.fTienPheDuyet = 0;
                    }
                }

                if (x.Id == item.iID_ChiPhiCha) {
                    x.fTienPheDuyet = 0;
                }
            });
        }
    }

    if (!arrHasValue(items)) {
        return;
    }

    items.forEach(x => {
        if (x.Id == item.Id) {
            x.iID_ChiPhiID = item.iID_ChiPhiID;
            x.sTenChiPhi = item.sTenChiPhi;
            x.sTenChiPhiCha = item.sTenChiPhiCha;
            x.iID_ChiPhiCha = item.iID_ChiPhiCha;
            x.fTienPheDuyet = item.fTienPheDuyet;
            x.IsDefault = item.IsDefault;
        }
    });

    arrChiPhi = items;
}



function GetListDataChiPhi() {
    return arrChiPhi;
}



function LoadDataViewChiPhi() {
    var items = GetListDataChiPhi();

    var html = "";

    if (!arrHasValue(items)) {

        $("#tblChiPhiDauTu tbody").html(html);
        return;
    }

    $("#tblChiPhiDauTu tbody").html(html);

    var count = 1;
    items.forEach(x => {

        var duanChiPhiId = x.iID_DuAn_ChiPhi;
        //var duanChiPhiString = duanChiPhiId.toString();
        var stt = count;
        html += '<tr>';
        html += '<input type="hidden" class="r_IThuTu" value = "' + x.iThuTu + '"/> <input type="hidden" class="r_ID" value = "' + x.Id + '"/>';
        html += '<td style="font-weight:bold;text-align:left;" > <input type = "text" disabled class="form-control r_TenChiPhi" value = "' + x.sTenChiPhi + '" /> </td> <input type="hidden" id="' + duanChiPhiId + '" class="r_iID_DuAn_ChiPhiID" value="' + duanChiPhiId + '"/> <input type="hidden" class="r_iID_ChiPhiParentID" value="' + x.iID_ChiPhi_Parent + '"/> <input type="hidden" class="r_iID_ChiPhiID" value = "' + x.iID_ChiPhiID + '"/>';
        html += '<td style="text-align:right"><input type="text" disabled style="text-align: right" class="form-control txtGiaTriChiPhi" onkeyup="ValidateNumberKeyUp(this); " onkeypress="return ValidateNumberKeyPress(this, event); " value="' + FormatNumber(x.fTienPheDuyet) + '"/> </td>';
        html += "<td align='center'>";
        if (x.isEdit) {
            
            html += "<button class='btn-detail' type = 'button' onclick = 'DetailChiPhi(this)' > " +
                "<i class='fa fa-eye fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            html += "<button class='btn-add-child btn-icon' type = 'button' onclick = 'ThemMoiChiPhiCon(this)' > " +
                "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            html += "<button class='btn-save btn-icon' hidden  type = 'button' onclick = 'CreateChiPhi(this)' > " +
                "<i class='fa fa-floppy-o fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            html += "<button class='btn-edit btn-icon'  type = 'button' onclick = 'EditChiPhi(this)' > " +
                "<i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            html += "<button class='btn-delete btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_CHI_PHI_DAU_TU + "\")'>" +
                "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
                "</button>";
        } else {
           
            html += "<button class='btn-detail' hidden type = 'button' onclick = 'DetailChiPhi(this)' > " +
                "<i class='fa fa-eye fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            html += "<button class='btn-add-child btn-icon' type = 'button' onclick = 'ThemMoiChiPhiCon(this)' > " +
                "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            html += "<button class='btn-save btn-icon' hidden  type = 'button' onclick = 'CreateChiPhi(this)' > " +
                "<i class='fa fa-floppy-o fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            html += "<button class='btn-edit btn-icon' hidden  type = 'button' onclick = 'EditChiPhi(this)' > " +
                "<i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            html += "<button class='btn-delete btn-icon' hidden type='button' onclick='XoaDong(this, \"" + TBL_CHI_PHI_DAU_TU + "\")'>" +
                "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
                "</button>";
        }

        html += "</td></tr>";

        count++;
    });

    $("#tblChiPhiDauTu tbody").append(html);

}


function DetailChiPhi(nutDetail) {
    var dongChiPhiHienTai = nutDetail.parentElement.parentElement;
    var giaTriChiPhi = $(dongChiPhiHienTai).find(".txtGiaTriChiPhi").val();
    var tenChiPhi = $(dongChiPhiHienTai).find(".r_TenChiPhi").val();
    var chiPhiId = $(dongChiPhiHienTai).find(".r_iID_DuAn_ChiPhiID").val();

    $("#txtTenChiPhi").html(tenChiPhi);
    $("#txtIdHangMuc").val(chiPhiId);
    $("#txtGiaTriChiPhi").val(giaTriChiPhi);
    $("#spGiaTriChiPhi").html(giaTriChiPhi);

    ShowModalChiTietChiPhi();
    //tìm xem chi phí đã lưu hạng mục chưa, nếu chưa có hạng mục thì lấy data hạng mục từ chủ trương sang, nếu có thì show list hạng mục theo chủ trương
    if (arrHangMucSave.some(x => x.iID_DuAn_ChiPhi == chiPhiId)) {
        var arrHangMucByChiPhi = arrHangMucSave.filter(function (x) { return x.iID_DuAn_ChiPhi == chiPhiId });
        //arrHangMuc = arrHangMucByChiPhi;
        LoadViewHangMuc(arrHangMucByChiPhi);
        CalculateTienConLaiHangMuc(arrHangMucByChiPhi);
    } else {
        //arrHangMuc = arrHangMucChuTruong;
        var arrHangMucByChiPhi = arrHangMuc.filter(function (x) { return x.iID_DuAn_ChiPhi == chiPhiId });
        LoadViewHangMuc(arrHangMucByChiPhi);
        CalculateTienConLaiHangMuc(arrHangMucByChiPhi);
    }
}

function ShowModalChiTietChiPhi() {
    $("#modal-listdetailchiphi").modal("show");
}

function HideModalChiTietChiPhi() {
    $("#modal-listdetailchiphi").modal("hide");
}

function GetDataHangMucByJson() {
    var duToanId = $("#iIDDuToanId").val();
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/GetListHangMucByTKTC",
        type: "POST",
        data: { id: duToanId },
        dataType: "json",
        cache: false,
        success: function (resp) {
            if (resp.status == false) {
                return;
            }

            if (arrHasValue(resp.data)) {
                arrHangMuc = resp.data;
                arrHangMucSave = resp.data;
                UpdateHangMucCanEdit(arrHangMucSave);
            }
            
        }
    });

}

function UpdateHangMucCanEdit(arrayHangMucParam) {

    arrayHangMucParam.forEach(obj => {
        var checkIsParent = CheckHangMucIsParent(obj.iID_DuToan_DM_HangMucID, arrayHangMucParam);
        if (checkIsParent) {
            obj.isEdit = false;
        } else {
            obj.isEdit = true;
        }
        
        if (obj.iID_ParentID == null) {
            obj.iID_ParentID = '';
        }
    });
}

function CheckHangMucIsParent(itemId, arrayHangMucParam) {

    var listChild = [];
    listChild = arrayHangMucParam.filter(function (x) { return x.iID_ParentID == itemId });
    if (arrHasValue(listChild)) {
        return true;
    } else {
        return false;
    }
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

function CreateHtmlSelectLoaiCongTrinh(value) {
    var htmlOption = "";
    arr_LoaiCongTrinh.forEach(x => {
        if (value != undefined && value == x.id)
            htmlOption += "<option value='" + x.id + "' selected>" + x.text + "</option>";
        else
            htmlOption += "<option value='" + x.id + "'>" + x.text + "</option>";
    })
    if (value != undefined && value != "") {
        return "<select disabled class='form-control'>" + htmlOption + "</option>";
    } else {
        return "<select class='form-control'>" + htmlOption + "</option>";
    }
}

function LoadViewHangMuc(data) {
    $("#tblHangMucChinh tbody").html("");
    //var data = GetListDataHangMuc();
    for (var i = 0; i < data.length; i++) {
        var dongMoi = "";
        dongMoi += "<tr style='cursor: pointer;' class='parent'>";

        if (data[i].isEdit) {
            dongMoi += "<td class='r_STT'>" + data[i].smaOrder + "</td><input type='hidden' id='" + data[i].iID_DuToan_DM_HangMucID + "' class='r_iID_DuAn_HangMucID' value='" + data[i].iID_DuToan_DM_HangMucID + "'/> <input type='hidden' class='r_HangMucID' value='" + data[i].iID_HangMucID + "'/> <input type='hidden' class='r_HangMucParentID' value='" + data[i].iID_ParentID + "'/> <input type='hidden' class='r_IsEdit' value='" + data[i].isEdit + "'/>";
            dongMoi += "<td><input type='text' disabled class='form-control txtTenHangMuc' value='" + data[i].sTenHangMuc + "'/></td>"
            dongMoi += "<td><input type='hidden' class='r_iID_LoaiCongTrinhID' value=''/><div class='selectLoaiCongTrinh'>" + CreateHtmlSelectLoaiCongTrinh(data[i].iID_LoaiCongTrinhID) + "</div></td>";
            dongMoi += "<td  align='right'><div hidden></div><input type='text' style='text-align: right' disabled  class='form-control txtHanMucDauTu' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value='" + FormatNumber(data[i].fTienPheDuyet) + "'/></td>";
            dongMoi += "<td align='center'>";

            dongMoi += "<button class='btn-add-child btn-icon' type = 'button' onclick = 'ThemMoiHangMucCon(this)' > " +
                "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            dongMoi += "<button class='btn-save btn-icon' hidden type = 'button' onclick = 'CreateHangMuc(this, \"" + TBL_HANG_MUC_CHINH + "\")' > " +
                "<i class='fa fa-floppy-o fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            dongMoi += "<button class='btn-edit btn-icon' type = 'button' onclick = 'EditHangMuc(this, \"" + TBL_HANG_MUC_CHINH + "\")' > " +
                "<i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            dongMoi += "<button class='btn-delete btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_HANG_MUC_CHINH + "\")'>" +
                "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
                "</button>";
        } else {
            dongMoi += "<td class='r_STT'>" + data[i].smaOrder + "</td><input type='hidden' id='" + data[i].iID_DuToan_DM_HangMucID + "' class='r_iID_DuAn_HangMucID' value='" + data[i].iID_DuToan_DM_HangMucID + "'/> <input type='hidden' class='r_HangMucID' value='" + data[i].iID_HangMucID + "'/> <input type='hidden' class='r_HangMucParentID' value='" + data[i].iID_ParentID + "'/> <input type='hidden' class='r_IsEdit' value='" + data[i].isEdit + "'/>";
            dongMoi += "<td><input type='text' class='form-control txtTenHangMuc' disabled value='" + data[i].sTenHangMuc + "'/></td>"
            dongMoi += "<td><input type='hidden' class='r_iID_LoaiCongTrinhID' value=''/><div class='sTenLoaiCongTrinh' hidden></div><div class='selectLoaiCongTrinh'>" + CreateHtmlSelectLoaiCongTrinh(data[i].iID_LoaiCongTrinhID) + "</div></td>";
            dongMoi += "<td  align='right'><div hidden></div><input type='text' style='text-align: right' disabled  class='form-control txtHanMucDauTu' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value='" + FormatNumber(data[i].fTienPheDuyet) + "'/></td>";
            dongMoi += "<td align='center'>";

            dongMoi += "<button class='btn-add-child btn-icon' type = 'button' onclick = 'ThemMoiHangMucCon(this)' > " +
                "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            dongMoi += "<button class='btn-save btn-icon' hidden type = 'button' onclick = 'CreateHangMuc(this, \"" + TBL_HANG_MUC_CHINH + "\")' > " +
                "<i class='fa fa-floppy-o fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            dongMoi += "<button class='btn-edit btn-icon' hidden type = 'button' onclick = 'EditHangMuc(this, \"" + TBL_HANG_MUC_CHINH + "\")' > " +
                "<i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            dongMoi += "<button class='btn-delete btn-icon' hidden type='button' onclick='XoaDong(this, \"" + TBL_HANG_MUC_CHINH + "\")'>" +
                "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
                "</button>";
        }

        dongMoi += "</td>";
        dongMoi += "</tr>";

        $("#tblHangMucChinh tbody").append(dongMoi);
    }

}

function CalculateTienConLaiHangMuc(arrHM) {
    var giaTriChiPhi = parseFloat(UnFormatNumber($("#txtGiaTriChiPhi").val()));
    var chiPhiDuAnId = $("#txtIdHangMuc").val();
    var result = giaTriChiPhi;

    var arrHMParent = arrHM.filter(function (x) { return x.iID_ParentID == "" && x.iID_DuAn_ChiPhi == chiPhiDuAnId });
    if (arrHasValue(arrHMParent)) {
        arrHMParent.forEach(x => {
            if (x.fTienPheDuyet != null || x.fTienPheDuyet != "") {
                result -= x.fTienPheDuyet;
            }

        });
    }

    $("#conlaihangmucpheduyet").text(FormatNumber(result));
}

function ThemMoiHangMucCon(nutThem) {
    var dongHienTai = nutThem.parentElement.parentElement;

    // Lấy mã hạng mục của item cha
    var iIDDuAnHangMucID = $(dongHienTai).find(".r_iID_DuAn_HangMucID").val();

    // Tạo mới item con
    var dongMoi = "";
    dongMoi += "<tr style='cursor: pointer;' class='child' data-idparent='" + iIDDuAnHangMucID + "'>";
    dongMoi += "<td class='r_STT'></td><input type='hidden' class='r_iID_DuAn_HangMucID' value=''/>";
    dongMoi += "<td class='r_sTenHangMuc'><div class='sTenHangMuc' hidden></div><input type='text' class='form-control txtTenHangMuc'/></td>"
    dongMoi += "<td><input type='hidden' class='r_iID_LoaiCongTrinhID' value=''/><div class='sTenLoaiCongTrinh' hidden></div><div class='selectLoaiCongTrinh'>" + CreateHtmlSelectLoaiCongTrinh() + "</div></td>";
    dongMoi += "<td class='r_HanMucDauTu' align='right'><div class='fHanMucDauTu' hidden></div><input type='text' class='form-control txtHanMucDauTu' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);'/></td>";
    dongMoi += "<td align='center'>";

    dongMoi += "<button class='btn-add-child btn-icon' hidden type = 'button' onclick = 'ThemMoiHangMucCon(this)' > " +
        "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" +
        "</button> ";

    dongMoi += "<button class='btn-save btn-icon' type = 'button' onclick = 'CreateHangMuc(this, \"" + TBL_HANG_MUC_CHINH + "\",\"" + iIDDuAnHangMucID + "\",\"" + "" + "\")' > " +
        "<i class='fa fa-floppy-o fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-edit btn-icon' hidden type = 'button' onclick = 'EditHangMucModal(this, \"" + TBL_HANG_MUC_CHINH + "\")' > " +
        "<i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-delete btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_HANG_MUC_CHINH + "\")'>" +
        "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
        "</button></td>";
    dongMoi += "</tr>";

    $(dongHienTai).after(dongMoi);
    CapNhatCotSttCon_HangMuc(TBL_HANG_MUC_CHINH);
}

function CapNhatCotStt_HangMuc(idBang) {
    $("#" + idBang + " tbody tr.parent").each(function (index, tr) {
        $(tr).find('.r_STT').text(index + 1);
    });
}

function CapNhatCotSttCon_HangMuc(idBang) {
    $("#" + idBang + " tbody tr").each(function (index, tr) {
        var sttParent = $(tr).find('.r_STT').text();
        var iIDParentID = $(tr).find('.r_iID_DuAn_HangMucID').val();
        $("#" + idBang + " tbody tr.child[data-idparent='" + iIDParentID + "']").each(function (index, tr) {
            $(tr).find('.r_STT').text(sttParent + "-" + (index + 1));
        });
    });
}

function CreateHangMuc(nutCreateHangMuc, idBang, iDuAnHangMucID_Parent, id, iDHangMucTheoDuAn) {
    var item = GetDataFormChiTietChiPhi(nutCreateHangMuc, iDuAnHangMucID_Parent, id, iDHangMucTheoDuAn);

    if (!ValidateBeforeSaveChiTietChiPhiExist(item)) {
        return;
    }

    AddChiTietChiPhi(item);
    //ResetFormChiTietChiPhi();
    ReloadViewChiTietChiPhi(GetValueIdChiPhiModal());

    //LoadDataComboBoxHangMucChaChiPhi(GetValueIdChiPhiModal());
    //ResetDataComboBoxHangMucChaChiPhi();
}


function ValidateBeforeSaveChiTietChiPhi(item) {
    //if (isStringEmpty(item)) {
    //    PopupModalWithChiTiet("Lỗi thêm hạng mục chi phí", ["Dữ liệu không chính xác !"], ERROR);
    //    return false;
    //}

    //var message = [];
    //if (isStringEmpty(item.sMaHangMuc)) {
    //    message.push("Vui lòng nhập mã hạng mục !");
    //}

    //if (isStringEmpty(item.sTenHangMuc)) {
    //    message.push("Vui lòng nhập tên hạng mục !");
    //}

    //if (!isStringEmpty(item.sHangMucCha)) {
    //    if (ValidateBeforeHaveChildWithTienHangMucChiPhi(item)) {
    //        if (isStringEmpty(item.fTienHangMuc)) {
    //            message.push("Vui lòng nhập giá trị phê duyệt hạng mục !");
    //        } else {
    //            if (UnFormatNumber(item.fTienHangMuc) <= 0) {
    //                message.push("Giá trị phê duyệt hạng mục phải lớn hơn 0 !");
    //            }
    //        }
    //    }
    //} else {
    //    message.push("Không thể thêm dữ liệu hạng mục cha !");
    //}

    //if (message.length > 0) {
    //    PopupModalWithChiTiet("Lỗi thêm hạng mục chi phí", message, ERROR);
    //    return false;
    //}

    //return true;

    if (isStringEmpty(item)) {
        PopupModalWithChiTiet("Lỗi thêm hạng mục chi phí", ["Dữ liệu không chính xác !"], ERROR);
        return false;
    }

    var message = [];

    if (isStringEmpty(item.sTenHangMuc)) {
        message.push("Vui lòng nhập tên hạng mục !");
    }

    if (isNaN(item.fTienHangMuc)) {
        message.push("Vui lòng nhập tiền phê duyệt!");
    }
    else if (parseInt(UnFormatNumber(item.fTienHangMuc)) < 0) {
        message.push("Giá trị phê duyệt phải lớn hơn 0!");
    }

    if (message.length > 0) {
        PopupModalWithChiTiet("Lỗi thêm hạng mục chi phí", message, ERROR);
        return false;
    }

    return true;
}


function ReloadViewChiTietChiPhiWhenOpenModal(id) {
    var items = GetListChiTietChiPhiWhenOpenModalById(id);
    SetValueDataChiTietChiPhiWhenOpenModal(items);
    LoadViewChiTietChiPhi(items);
}

function GetListChiTietChiPhiWhenOpenModalById(id) {
    var result = [];

    if (isStringEmpty(id)) {
        return result;
    }

    var items = GetListDataChiTietChiPhiSave();
    if (!arrHasValue(items)) {
        return result;
    }

    items.forEach(x => {
        if (x.iID_ChiPhiID == id) {
            result.push(x);
        }
    });

    return result;
}

function GetListDataChiTietChiPhiSave() {
    return arr_ChiTietChiPhiSave;
}

function ThemMoiChiTietChiPhi() {
    var item = GetDataFormChiTietChiPhi();

    if (!ValidateBeforeSaveChiTietChiPhi(item)) {
        return false;
    }

    if (!ValidateBeforeSaveChiTietChiPhiExist(item)) {
        return false;
    }

    AddChiTietChiPhi(item);
    ResetFormChiTietChiPhi();
    ReloadViewChiTietChiPhi(GetValueIdChiPhiModal());

    LoadDataComboBoxHangMucChaChiPhi(GetValueIdChiPhiModal());
    ResetDataComboBoxHangMucChaChiPhi();
}


function GetFormDataThietKeThiCong() {
    var id = $("#iIDDuToanId").val();
    var soQuyetDinh = $("#sSoQuyetDinh").val();
    var ngayPheDuyet = $("#dNgayQuyetDinh").val();
    //var coQuanPheDuyet = $("#sCoQuanPheDuyet").val();
    //var nguoiKy = $("#sNguoiKy").val();
    var loaiQuyetDinh = $("#bLaTongDuToan").val();
    var donViQuanLy = $("#iID_DonViQuanLyID").val();
    var duAn = $("#iID_DuAnID").val();
    var diaDiem = $("#sDiaDiem").val();
    var tenDuToan = $("#sTenDuToan").val();

    return {
        iID_DuToanID: id,
        sSoQuyetDinh: soQuyetDinh,
        dNgayQuyetDinh: ngayPheDuyet,
        sTenDuToan: tenDuToan,
        //sCoQuanPheDuyet: coQuanPheDuyet,
        //sNguoiKy: nguoiKy,
        bLaTongDuToan: loaiQuyetDinh,
        iID_DuAnID: duAn,
        sDiaDiem: diaDiem,
        ListChiPhi: GetListDataChiPhi(),
        ListNguonVon: GetListDataNguonVon(),
        ListHangMuc: GetListDataChiTietChiPhiSave()
    };
}

function SaveThietKeThiCong() {
    var item = GetFormDataThietKeThiCong();

    if (!ValidateBeforeSaveThietKeThiCong(item)) {
        return false;
    }

    if (!ValidateBeforeTongTienNguonVonWithChiPhi()) {
        return false;
    }

    if (!ValidateBeforeTongTienHangMucWithChiPhi()) {
        return false;
    }

    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/Save",
        type: "POST",
        data: { model: item },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null && data.status == true) {
                window.location.href = "/QLVonDauTu/QLPheDuyetTKTCVaTDT";
            }
        },
        error: function (data) {

        }
    })
}

function ValidateBeforeSaveThietKeThiCong(item) {
    var message = [];

    if (isStringEmpty(item.sSoQuyetDinh)) {
        message.push("Vui lòng nhập số quyết định !");
    }

    if (isStringEmpty(item.dNgayQuyetDinh)) {
        message.push("Vui lòng nhập ngày quyết định !");
    }

    //if (isStringEmpty(item.sCoQuanPheDuyet)) {
    //    message.push("Vui lòng nhập cơ quan phê duyệt !");
    //}

    //if (isStringEmpty(item.sNguoiKy)) {
    //    message.push("Vui lòng nhập người ký !");
    //}

    if (message.length > 0) {
        PopupModal("Lỗi lưu thiết kế thi công tổng dự toán", message, ERROR);
        return false;
    }

    return true;
}

function ValidateBeforeTongTienNguonVonWithChiPhi() {
    var chiPhi = 0;
    var nguonVon = 0;
    var listNguonVon = GetListDataNguonVon();
    var listChiPhi = GetListDataChiPhi();

    if (arrHasValue(listNguonVon)) {
        listNguonVon.forEach(x => {
            if (!isStringEmpty(x.fTienPheDuyet)) {
                nguonVon += parseFloat(x.fTienPheDuyet);
            }
        });
    }

    if (arrHasValue(listChiPhi)) {
        listChiPhi.forEach(x => {
            if (!isStringEmpty(x.fTienPheDuyet)) {
                chiPhi += parseFloat(x.fTienPheDuyet);
            }
        });
    }

    if (chiPhi != nguonVon) {
        PopupModal("Lỗi lưu phê duyệt dự án", ["Tổng tiền nguồn vốn và tổng tiền chi phí không bằng nhau !"], ERROR);
        return false;
    }

    return true;
}



function GetTongTienHangMucByIdChiPhi(id) {
    var result = 0;

    if (isStringEmpty(id)) {
        return result;
    }

    var items = GetListDataChiTietChiPhiSave();
    if (!arrHasValue(items)) {
        return result;
    }

    items.forEach(x => {
        if (x.iID_ChiPhiID == id) {
            result += x.fTienHangMuc;
        }
    });

    return result;
}

function EventChangeLoaiQuyetDinh() {
    $("#bLaTongDuToan").change(function () {
        if (this.value != "") {
            $.ajax({
                url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/GetDonviListByUser",
                type: "POST",
                dataType: "json",
                cache: false,
                success: function (data) {
                    if (data != null && data != "") {
                        $("#iID_DonViQuanLyID").html(data);
                    }
                },
                error: function (data) {

                }
            })
        } else {
            $("#iID_DonViQuanLyID option:not(:first)").remove();
            $("#iID_DonViQuanLyID").trigger("change");
        }
    });
}

function EventChangeDonViQuanLy() {
    $("#iID_DonViQuanLyID").change(function () {
        if (this.value != "" && this.value != GUID_EMPTY) {
            $.ajax({
                url: "/QLVonDauTu/QLPheDuyetDuAn/LayDanhSachDuAnTheoDonViQuanLy",
                type: "POST",
                data: { iID_DonViQuanLyID: this.value },
                dataType: "json",
                cache: false,
                success: function (data) {
                    if (data != null && data != "") {
                        $("#iID_DuAnID").html(data);
                    }
                }
            })
        } else {
            $("#iID_DuAnID option:not(:first)").remove();
            $("#iID_DuAnID").trigger("change");
        }
    });
}

function EventChangeDuAn() {
    $("#iID_DuAnID").change(function () {
        if (this.value != "" && this.value != GUID_EMPTY) {
            GetThongTinDuAn(this.value);
            GetDanhSachNguonVonTheoDuAn(this.value);
            GetDanhSachChiPhiTheoDuAn(this.value);
            GetDanhSachHangMucTheoDuAn(this.value);
        } else {
            $("#sMaDuAn").val("");
        }
    });
}

function GetThongTinDuAn(id) {
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/LayThongTinChiTietDuAn",
        type: "POST",
        data: { iID: id },
        dataType: "json",
        cache: false,
        success: function (resp) {
            if (!isStringEmpty(resp)) {
                if (!isStringEmpty(resp.sKhoiCong)) {
                    $("#sKhoiCong").val(resp.sKhoiCong);
                }

                if (!isStringEmpty(resp.sKetThuc)) {
                    $("#sHoanThanh").val(resp.sKetThuc);
                }

                if (!isStringEmpty(resp.sDiaDiem)) {
                    $("#sDiaDiem").val(resp.sDiaDiem);
                }

                if (!isStringEmpty(resp.fTongMucDauTu)) {
                    $("#fTongMucDauTu").val(FormatNumber(resp.fTongMucDauTu));
                }
            }
        }
    })
}

