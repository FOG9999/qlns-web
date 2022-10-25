var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var TBL_CHI_PHI_DAU_TU = "tblChiPhiDauTu";
var TBL_NGUON_VON_DAU_TU = "tblNguonVonDauTu";
var TBL_HANG_MUC_CHINH = "tblHangMucChinh";
var TBL_TAI_LIEU_DINH_KEM = "tblThongTinTaiLieuDinhKem";
var arrChiPhi = [];
var arrChiPhiSave = [];
var arrNguonVon = [];
var arrNguonVonSave = [];
var arrHangMuc = [];
var arrHangMucSave = [];
var arrHangMucTheoDuAn = [];
var arr_NguonVon_Dropdown = [];
var tCpdtGiaTriToTrinh = 0, tCpdtGiaTriThamDinh = 0, tCpdtGiaTriPheDuyet = 0;
var tNvdtGiaTriToTrinh = 0, tNvdtGiaTriThamDinh = 0, tNvdtGiaTriPheDuyet = 0;
var ERROR = 1;
var CONFIRM = 0;
var tongNguonVonBanDau = 0;

$(document).ready(function ($) {
    
    Event();
});

//dùng chung (nhóm lại sau)
async function Event() {
    EventChangeLoaiQuyetDinh();
    EventChangeDonViQuanLy();
    EventChangeDuAn();

    SetSelectById("txtChiPhiCha");
    LoadDataComboBoxNguonVon();
    LoadDataComboBoxLoaiCongTrinh();
    
    LoadDataViewChiPhi();
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
//end dùng chung

function KiemTraTrungSoQuyetDinh(sSoQuyetDinh) {
    var check = false;
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/KiemTraTrungSoQuyetDinh",
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
    if (doiTuong.sSoToTrinh == "")
        messErr.push("Số tờ trình không được để trống");
    if (doiTuong.dNgayToTrinh == "")
        messErr.push("Ngày tờ trình không được để trống hoặc sai định dạng");
    if (doiTuong.sSoQuyetDinh == "")
        messErr.push("Số quyết định không được để trống");
    if (doiTuong.dNgayQuyetDinh == "")
        messErr.push("Ngày phê duyệt không được để trống hoặc sai định dạng");
    if (doiTuong.iID_DuAnID == "" || doiTuong.iID_DuAnID == GUID_EMPTY)
        messErr.push("Hãy chọn dự án");
    if (messErr.length > 0) {
        alert(messErr.join("\n"));
        return false;
    } else {
        return true;
    }
}

function Save() {
    var pheduyetDuToan = {};
    var bLaTongDuToan;
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
    bLaTongDuToan = $("#bLaTongDuToan").val();
    if (bLaTongDuToan == 0)
        pheduyetDuToan.bLaTongDuToan = false;
    else
        pheduyetDuToan.bLaTongDuToan = true;

    //Thông tin nội dung
    pheduyetDuToan.iID_DuAnID = $("#iID_DuAnID").val();
    //pheduyetDuToan.sTenDuAn = $("#iID_DuAnID :selected").html();

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

function Cancel() {
    window.location.href = "/QLVonDauTu/QLPheDuyetTKTCVaTDT";
}

// Nguồn vốn thiết kế thi công



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
            dongMoi += "<td class='r_GiaTriNguonVon' ><div hidden></div><input type='text' style='text-align: right' class='form-control txtGiaTriNguonVon' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value='" + FormatNumber(data[i].fTienPheDuyet) + "'/></td>"
            dongMoi += "<td align='center'>";
            dongMoi += "<button class='btn-save btn-icon'  type = 'button' onclick = 'CreateNguonVon(this, \"" + TBL_NGUON_VON_DAU_TU + "\")'> " +
                "<i class='fa fa-floppy-o fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            dongMoi += "<button class='btn-edit btn-icon' hidden type = 'button' onclick = 'EditNguonVon(this, \"" + TBL_NGUON_VON_DAU_TU + "\")'> " +
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

function CreateNguonVon(nutCreate, idBang) {
    var messErr = [];
    var dongHienTai = nutCreate.parentElement.parentElement;

    var iID_NguonVonID = $(dongHienTai).find(".selectNguonVon select").val();
    var id = $(dongHienTai).find(".r_STT").html();

    if (iID_NguonVonID == "0") {
        messErr.push("Chưa chọn nguồn vốn");
    }

    var sHanMucDauTu = $(dongHienTai).find(".txtGiaTriNguonVon").val();
    if (sHanMucDauTu == "") {
        messErr.push("Giá trị phê duyệt phải lớn hơn 0");
    }
    var giaTriPheDuyet = parseInt(UnFormatNumber(sHanMucDauTu));

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
        iID_NguonVonID: iID_NguonVonID,
        fTienPheDuyet: giaTriPheDuyet
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



function GetListDataChiPhi() {
    return arrChiPhi;
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
        
        if (x.isEdit) {
            html += '<td style="text-align:right"><input type="text" style="text-align: right" class="form-control txtGiaTriChiPhi" onkeyup="ValidateNumberKeyUp(this); " onkeypress="return ValidateNumberKeyPress(this, event); " value="' + FormatNumber(x.fTienPheDuyet) + '"/> </td>';
            html += "<td align='center'>";
            html += "<button class='btn-detail' hidden type = 'button' onclick = 'DetailChiPhi(this)' > " +
                "<i class='fa fa-eye fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            html += "<button class='btn-add-child btn-icon' hidden type = 'button' onclick = 'ThemMoiChiPhiCon(this)' > " +
                "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            html += "<button class='btn-save btn-icon'  type = 'button' onclick = 'CreateChiPhi(this)' > " +
                "<i class='fa fa-floppy-o fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            html += "<button class='btn-edit btn-icon' hidden  type = 'button' onclick = 'EditChiPhi(this)' > " +
                "<i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            html += "<button class='btn-delete btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_CHI_PHI_DAU_TU + "\")'>" +
                "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
                "</button>";
        } else {
            html += '<td style="text-align:right"><input type="text" disabled style="text-align: right" class="form-control txtGiaTriChiPhi" onkeyup="ValidateNumberKeyUp(this); " onkeypress="return ValidateNumberKeyPress(this, event); " value="' + FormatNumber(x.fTienPheDuyet) + '"/> </td>';
            html += "<td align='center'>";
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

function ThemMoiChiPhiCon(nutThem) {
    var dongHienTai = nutThem.parentElement.parentElement;
    //$(dongHienTai).find(".txtGiaTriChiPhi").val(0);
    var iIDDuAnChiPhiID = $(dongHienTai).find(".r_iID_DuAn_ChiPhiID").val();
    var iThuTu = $(dongHienTai).find(".r_IThuTu").val();
    resChiPhi = [];
    var rowChild = FindLastChildChiPhi(iIDDuAnChiPhiID);
    var duanChiPhiId = uuidv4();
    var dongMoi = "";
    dongMoi += '<tr>';
    dongMoi += '<input type="hidden" class="r_IThuTu" value = "' + iThuTu + '"/> <input type="hidden" class="r_ID" value = ""/>';
    dongMoi += '<td style="text-align:left;" > <input type = "text" class="form-control r_TenChiPhi"/> </td> <input type="hidden" id="' + duanChiPhiId + '"  class="r_iID_DuAn_ChiPhiID" value="' + duanChiPhiId + '"/> <input type="hidden" class="r_iID_ChiPhiParentID" value="' + iIDDuAnChiPhiID + '"/> <input type="hidden" class="r_iID_ChiPhiID"/>';
    dongMoi += '<td style="text-align:right"><input type="text" style="text-align: right" class="form-control txtGiaTriChiPhi" onkeyup="ValidateNumberKeyUp(this); " onkeypress="return ValidateNumberKeyPress(this, event); " /> </td>';
    dongMoi += "<td align='center'>";

    dongMoi += "<button class='btn-detail' hidden type = 'button' onclick = 'DetailChiPhi(this)' > " +
        "<i class='fa fa-eye fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-add-child btn-icon' hidden type = 'button' onclick = 'ThemMoiChiPhiCon(this)' > " +
        "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-save btn-icon' type = 'button' onclick = 'CreateChiPhi(this)' > " +
        "<i class='fa fa-floppy-o fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-edit btn-icon' hidden  type = 'button' onclick = 'EditChiPhi(this)' > " +
        "<i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-delete btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_CHI_PHI_DAU_TU + "\")'>" +
        "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
        "</button></td>";
    dongMoi += "</tr>";
    if (rowChild != undefined) {
        var a = rowChild.closest("tr");
        $(a).after(dongMoi);
    } else {
        $(dongHienTai).after(dongMoi);
    }
}

function FindLastChildChiPhi(parentId) {
    var chiPhiIdLast = "";
    var row;
    if (parentId != undefined) {
        UpdateArrayChiPhi();
        findChildrenChiPhi(parentId);
        if (resChiPhi.length > 0) {
            chiPhiIdLast = resChiPhi[resChiPhi.length - 1].iID_DuAn_ChiPhiID;
        } else {
            chiPhiIdLast = parentId;
        }
        // tìm dòng con chi phí cuối cùng trong bảng
        $("#" + TBL_CHI_PHI_DAU_TU + " .r_iID_DuAn_ChiPhiID").each(function () {
            var chiPhiId = $(this).val();
            if (chiPhiId == chiPhiIdLast && chiPhiIdLast != "") {
                row = $(this);
                return row;
            }
        });
    }
    return row;

}

var arr_chiPhiUpdate = [];
var resChiPhi = [];
function UpdateArrayChiPhi() {
    arr_chiPhiUpdate = [];
    $("#" + TBL_CHI_PHI_DAU_TU + " tbody tr").each(function (index, tr) {
        var iIDParentID = $(tr).find(".r_iID_ChiPhiParentID").val();
        var iIDDuAnChiPhiID = $(tr).find(".r_iID_DuAn_ChiPhiID").val();

        arr_chiPhiUpdate.push({
            iID_DuAn_ChiPhiID: iIDDuAnChiPhiID,
            iID_ParentID: iIDParentID

        })
    });
}

var findChildrenChiPhi = function (id) {
    arr_chiPhiUpdate.forEach(obj => {
        if (obj.iID_ParentID === id) {
            resChiPhi.push(obj);
            findChildrenChiPhi(obj.iID_DuAn_ChiPhiID)
        }
    })

}

function CreateChiPhi(nutCreate) {
    var dongHienTai = nutCreate.parentElement.parentElement;
    var messErr = [];
    var iIDDuAnChiPhiID = $(dongHienTai).find(".r_iID_DuAn_ChiPhiID").val();
    var iIDParentID = $(dongHienTai).find(".r_iID_ChiPhiParentID").val();
    var chiPhiId = $(dongHienTai).find(".r_iID_ChiPhiID").val();
    var thuTuCP = $(dongHienTai).find(".r_IThuTu").val();
    var loaiChiPhi = $(dongHienTai).find(".r_IsLoaiChiPhi").val();
    var id = $(dongHienTai).find(".r_ID").val();
    

    var sTenChiPhi = $(dongHienTai).find(".r_TenChiPhi").val();
    var sGiaTriChiPhi = $(dongHienTai).find(".txtGiaTriChiPhi").val();
    var fGiaTriChiPhi = parseInt(UnFormatNumber(sGiaTriChiPhi));

    if (sTenChiPhi == "") {
        messErr.push("Tên chi phí chưa được tạo");
    }

    if (fGiaTriChiPhi <= 0) {
        messErr.push("Giá trị chi phí phải lớn hơn 0");
    }

    if (messErr.length > 0) {
        PopupModal("Lỗi", messErr, ERROR);
        return;
    }

    $(dongHienTai).find(".r_TenChiPhi").attr("disabled", true);
    $(dongHienTai).find(".txtGiaTriChiPhi").attr("disabled", true);

    $(dongHienTai).find("button.btn-edit").show();
    $(dongHienTai).find("button.btn-detail").show();
    $(dongHienTai).find("button.btn-save").hide();
    $(dongHienTai).find("button.btn-add-child").show();

    arrChiPhiSave = arrChiPhiSave.filter(function (x) { return x.iID_DuAn_ChiPhi != iIDDuAnChiPhiID });

    arrChiPhiSave.push({
        Id: id,
        iID_DuAn_ChiPhi: iIDDuAnChiPhiID,
        iID_ChiPhiID: chiPhiId,
        iID_ChiPhi_Parent: iIDParentID,
        sTenChiPhi: sTenChiPhi,
        fTienPheDuyet: fGiaTriChiPhi,
        iThuTu: parseInt(thuTuCP)
    })
    AddParrentInArrayChiPhiSave(iIDParentID);
    CalculateDataChiPhi(iIDDuAnChiPhiID);
    CalculateTienConLaiNguonVon();
}

function AddParrentInArrayChiPhiSave(parentId) {
    chiPhiParent = arrChiPhi.find(x => x.iID_DuAn_ChiPhi == parentId);
    if (chiPhiParent == undefined) {
        return;
    }
    if (chiPhiParent != undefined) {
        arrChiPhiSave = arrChiPhiSave.filter(function (x) { return x.iID_DuAn_ChiPhi != parentId });
        arrChiPhiSave.push(chiPhiParent);
       
    }

    if (chiPhiParent.iID_ChiPhi_Parent != "" && chiPhiParent.iID_ChiPhi_Parent != undefined) {
        AddParrentInArrayChiPhiSave(chiPhiParent.iID_ChiPhi_Parent);
    }

}

function CalculateDataChiPhi(itemId) {
    var objItem = arrChiPhiSave.find(x => x.iID_DuAn_ChiPhi == itemId);
    if (objItem == undefined) {
        return;
    }
    var objParentItem = arrChiPhiSave.find(x => x.iID_DuAn_ChiPhi == objItem.iID_ChiPhi_Parent);
   
    var arrChildSameParent = arrChiPhiSave.filter(function (x) { return x.iID_ChiPhi_Parent == objItem.iID_ChiPhi_Parent && x.iID_ChiPhi_Parent != "" });
    if (arrHasValue(arrChildSameParent)) {
        CalculateTotalParent(objParentItem, arrChildSameParent);
    }

    if (objItem.iID_ChiPhi_Parent != "") {
        CalculateDataChiPhi(objItem.iID_ChiPhi_Parent);
    }
}

function CalculateTotalParent(objParentItem, arrChild) {
    if (objParentItem == undefined || !arrHasValue(arrChild)) {
        return;
    }
    var parentId = objParentItem.iID_DuAn_ChiPhi;

    var result = 0;
    arrChild.forEach(x => {
        result += x.fTienPheDuyet;
    });

    $("#" + parentId).closest("tr").find('.txtGiaTriChiPhi').val(FormatNumber(result));
    var parentItemNew = objParentItem;
    parentItemNew.fTienPheDuyet = result;
    arrChiPhiSave = arrChiPhiSave.filter(function (x) { return x.iID_DuAn_ChiPhi != objParentItem.iID_DuAn_ChiPhi });
    arrChiPhiSave.push(parentItemNew);
}

function EditChiPhi(nutEdit) {
    var dongHienTai = nutEdit.parentElement.parentElement;

    $(dongHienTai).find(".txtTenChiPhi").attr("disabled", false);
    $(dongHienTai).find(".txtGiaTriChiPhi").attr("disabled", false);

    $(dongHienTai).find("button.btn-edit").hide();
    $(dongHienTai).find("button.btn-save").show();
    $(dongHienTai).find("button.btn-add-child").hide();
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
        PopupModal("Lỗi xóa chi phí", ["Chi phí " + item.sTenChiPhi + " đang là chi phí cha nên không thể xóa !"], ERROR);
        return false;
    }

    return true;
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

function LoadViewHangMuc(data) {
    $("#tblHangMucChinh tbody").html("");
    //var data = GetListDataHangMuc();
    for (var i = 0; i < data.length; i++) {
        var dongMoi = "";
        dongMoi += "<tr style='cursor: pointer;' class='parent'>";

        if (data[i].isEdit) {
            dongMoi += "<td class='r_STT'>" + data[i].smaOrder + "</td><input type='hidden' id='" + data[i].iID_DuToan_DM_HangMucID + "' class='r_iID_DuAn_HangMucID' value='" + data[i].iID_DuToan_DM_HangMucID + "'/> <input type='hidden' class='r_HangMucID' value='" + data[i].iID_HangMucID + "'/> <input type='hidden' class='r_HangMucParentID' value='" + data[i].iID_ParentID + "'/> <input type='hidden' class='r_IsEdit' value='" + data[i].isEdit + "'/>";
            dongMoi += "<td><input type='text' class='form-control txtTenHangMuc' value='" + data[i].sTenHangMuc + "'/></td>"
            dongMoi += "<td><input type='hidden' class='r_iID_LoaiCongTrinhID' value=''/><div class='sTenLoaiCongTrinh' hidden></div><div class='selectLoaiCongTrinh'>" + CreateHtmlSelectLoaiCongTrinh(data[i].iID_LoaiCongTrinhID) + "</div></td>";
            dongMoi += "<td  align='right'><div hidden></div><input type='text' style='text-align: right'  class='form-control txtHanMucDauTu' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value='" + FormatNumber(data[i].fTienPheDuyet) + "'/></td>";
            dongMoi += "<td align='center'>";

            dongMoi += "<button class='btn-add-child btn-icon' hidden type = 'button' onclick = 'ThemMoiHangMucCon(this)' > " +
                "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            dongMoi += "<button class='btn-save btn-icon' type = 'button' onclick = 'CreateHangMuc(this, \"" + TBL_HANG_MUC_CHINH + "\")' > " +
                "<i class='fa fa-floppy-o fa-lg' aria-hidden='true'></i>" +
                "</button> ";
            dongMoi += "<button class='btn-edit btn-icon' hidden type = 'button' onclick = 'EditHangMuc(this, \"" + TBL_HANG_MUC_CHINH + "\")' > " +
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

    var arrHMParent = arrHM.filter(function (x) { return x.iID_ParentID == "" && x.iID_DuAn_ChiPhi == chiPhiDuAnId});
    if (arrHasValue(arrHMParent)) {
        arrHMParent.forEach(x => {
            if (x.fTienPheDuyet != null || x.fTienPheDuyet != "") {
                result -= x.fTienPheDuyet;
            }

        });
    }

    $("#conlaihangmucpheduyet").text(FormatNumber(result));
}


function ShowModalChiTietChiPhi() {
    $("#modal-listdetailchiphi").modal("show");
}

function HideModalChiTietChiPhi() {
    $("#modal-listdetailchiphi").modal("hide");
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

function ThemMoiHangMucCon(nutThem) {
    var dongHienTai = nutThem.parentElement.parentElement;
    var iIDDuAnHangMucID = $(dongHienTai).find(".r_iID_DuAn_HangMucID").val();
    res = [];
    var rowChild = FindLastChildHangMuc(iIDDuAnHangMucID);
    var sttParent = $(dongHienTai).find(".r_STT").text();
    var hangMucId = uuidv4();
    var dongMoi = "";
    dongMoi += "<tr style='cursor: pointer;' class='parent' data-idparent='" + iIDDuAnHangMucID + "'>";
    dongMoi += "<td class='r_STT'></td><input type='hidden' id='" + hangMucId + "' class='r_iID_DuAn_HangMucID' value='" + hangMucId + "'/> <input type='hidden' class='r_HangMucParentID' value='" + iIDDuAnHangMucID + "'/> <input type='hidden' class='r_IsEdit' value='true'/>";
    dongMoi += "<td class='r_sTenHangMuc'><div class='sTenHangMuc' hidden></div><input type='text' class='form-control txtTenHangMuc'/></td>"
    dongMoi += "<td><input type='hidden' class='r_iID_LoaiCongTrinhID' value=''/><div class='sTenLoaiCongTrinh' hidden></div><div class='selectLoaiCongTrinh'>" + CreateHtmlSelectLoaiCongTrinh() + "</div></td>";
    dongMoi += "<td class='r_HanMucDauTu' align='right'><div class='fHanMucDauTu' hidden></div><input type='text' style='text-align: right' class='form-control txtHanMucDauTu' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);'/></td>";
    dongMoi += "<td align='center'>";

    dongMoi += "<button class='btn-add-child btn-icon' hidden type = 'button' onclick = 'ThemMoiHangMucCon(this)' > " +
        "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-save btn-icon' type = 'button' onclick = 'CreateHangMuc(this, \"" + TBL_HANG_MUC_CHINH + "\")' > " +
        "<i class='fa fa-floppy-o fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-edit btn-icon' hidden type = 'button' onclick = 'EditHangMuc(this, \"" + TBL_HANG_MUC_CHINH + "\")' > " +
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
    CapNhatCotSttHangMucCon(iIDDuAnHangMucID, sttParent);
    //CapNhatCotSttCon(TBL_HANG_MUC_CHINH);
    var sttHangMucNew = "";
    if (arr_STT_Child.length > 0) {
        var indexMax = arr_STT_Child.sort().pop();
        var sttHangMucNew = sttParent + "-" + (indexMax + 1).toString();

    } else {
        sttHangMucNew = sttParent + "-" + "1";
    }
    rowChild.closest("tr").next("tr").find('.r_STT').text(sttHangMucNew);

    $(dongHienTai).find(".txtHanMucDauTu").attr("disabled", true);
    $(dongHienTai).find(".r_IsEdit").val("false");

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

function CapNhatCotStt_HangMuc(idBang) {
    $("#" + idBang + " tbody tr.parent").each(function (index, tr) {
        $(tr).find('.r_STT').text(index + 1);
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

function CreateHangMuc(nutCreateHangMuc, idBang) {
    var dongHienTai = nutCreateHangMuc.parentElement.parentElement;
    var messErr = [];
    var iIDDuAnHangMucID = $(dongHienTai).find(".r_iID_DuAn_HangMucID").val();
    var iIDParentID = $(dongHienTai).find(".r_HangMucParentID").val();
    var hangMucId = $(dongHienTai).find(".r_HangMucID").val();
    var duAnChiPhiId = $("#txtIdHangMuc").val();

    var sTenHangMuc = $(dongHienTai).find(".txtTenHangMuc").val();
    var iIDLoaiCongTrinhID = $(dongHienTai).find(".selectLoaiCongTrinh select").val();
    var sTenLoaiCongTrinh = $(dongHienTai).find(".selectLoaiCongTrinh select :selected").html();
    var sHanMucDauTu = $(dongHienTai).find(".txtHanMucDauTu").val();
    var fHanMucDauTu = parseInt(UnFormatNumber(sHanMucDauTu));
    var smaOrder = $(dongHienTai).find(".r_STT").html();
    var sIsEdit = $(dongHienTai).find('.r_IsEdit').val();
    var isEdit = true;
    if (sIsEdit == "true") {
        isEdit = true;
    } else {
        isEdit = false;
    }

    if (sTenHangMuc == "") {
        messErr.push("Tên hạng mục chưa có hoặc chưa chính xác.");
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
    $(dongHienTai).find(".txtHanMucDauTu").attr("disabled", true);

    $(dongHienTai).find(".selectLoaiCongTrinh select").attr("disabled", true);

    $(dongHienTai).find("button.btn-edit").show();
    $(dongHienTai).find("button.btn-save").hide();
    $(dongHienTai).find("button.btn-add-child").show();

    arrHangMucSave = arrHangMucSave.filter(function (x) { return x.iID_DuToan_DM_HangMucID != iIDDuAnHangMucID });

    arrHangMucSave.push({
        //iID_QDDauTu_DM_HangMucID: iIDDuAnHangMucID,
        iID_DuToan_DM_HangMucID: iIDDuAnHangMucID,
        //iID_HangMucID: hangMucId,
        iID_DuAn_ChiPhi: duAnChiPhiId,
        iID_ParentID: iIDParentID,
        sTenHangMuc: sTenHangMuc,
        iID_LoaiCongTrinhID: iIDLoaiCongTrinhID,
        sTenLoaiCongTrinh: sTenLoaiCongTrinh,
        fTienPheDuyet: fHanMucDauTu,
        smaOrder: smaOrder,
        isEdit: isEdit
    })

    AddParrentInArrayHangMucSave(iIDParentID);
    arrHangMucSave.sort(compare);
    CalculateDataHangMucByChiPhi(iIDDuAnHangMucID);
    CalculateTienConLaiHangMuc(arrHangMucSave);
}

function AddParrentInArrayHangMucSave(parentId) {
    var hangMucParent = arrHangMuc.find(x => x.iID_DuToan_DM_HangMucID == parentId);
    if (hangMucParent == undefined) {
        return;
    }
    if (hangMucParent != undefined) {
        arrHangMucSave = arrHangMucSave.filter(function (x) { return x.iID_DuToan_DM_HangMucID != parentId });
        arrHangMucSave.push(hangMucParent);

    }

    if (hangMucParent.iID_ParentID != "" && hangMucParent.iID_ParentID != undefined) {
        AddParrentInArrayHangMucSave(hangMucParent.iID_ParentID);
    }

}

function compare(a, b) {
    if (a.smaOrder < b.smaOrder) {
        return -1;
    }
    if (a.smaOrder > b.smaOrder) {
        return 1;
    }
    return 0;
}

function CalculateDataHangMucByChiPhi(itemId) {
    var objItem = arrHangMucSave.find(x => x.iID_DuToan_DM_HangMucID == itemId);

    if (objItem == undefined || objItem.iID_ParentID == "" || objItem.iID_ParentID == null) {
        return;
    }
    var objParentItem = arrHangMucSave.find(x => x.iID_DuToan_DM_HangMucID == objItem.iID_ParentID);
    var arrChildSameParent = arrHangMucSave.filter(function (x) { return x.iID_ParentID == objItem.iID_ParentID && x.iID_ParentID != "" });
    if (arrHasValue(arrChildSameParent)) {
        CalculateTotalParentHangMuc(objParentItem, arrChildSameParent);
    }

    if (objParentItem.iID_ParentID != "" && objParentItem.iID_ParentID != null) {
        CalculateDataHangMucByChiPhi(objParentItem.iID_DuToan_DM_HangMucID, arrHangMuc);
    }
}

function CalculateTotalParentHangMuc(objParentItem, arrChild) {
    var parentId = objParentItem.iID_DuToan_DM_HangMucID;

    var result = 0;
    arrChild.forEach(x => {
        result += x.fTienPheDuyet;
    });

    $("#" + parentId).closest("tr").find('.txtHanMucDauTu').val(FormatNumber(result));
    var parentItemNew = objParentItem;
    parentItemNew.fTienPheDuyet = result;
    //ChangeValueGiaTriPheDuyetHangMuc(parentItemNew, arrHangMuc);
}


function EditHangMuc(nutEdit, idBang) {
    var dongHienTai = nutEdit.parentElement.parentElement;
    var iIDLoaiCongTrinhID = $(dongHienTai).find(".selectLoaiCongTrinh select").val();

    $(dongHienTai).find(".txtTenHangMuc").attr("disabled", false);
    $(dongHienTai).find(".txtHanMucDauTu").attr("disabled", false);


    $(dongHienTai).find(".selectLoaiCongTrinh select").attr("disabled", false);

    $(dongHienTai).find("button.btn-edit").hide();
    $(dongHienTai).find("button.btn-save").show();
    $(dongHienTai).find("button.btn-add-child").hide();
}



function EventChangeLoaiQuyetDinh() {
    $("#bLaTongDuToan").change(function () {
        if (this.value != "") {
            // Hiển thị hoặc Ẩn textbox Tên dự toán
            if (this.value == 0) {
                $("#divTenDuToan").show();
            }
            else {
                $("#divTenDuToan").hide();
            }

            $.ajax({
                url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/GetDonviListByUser",
                type: "POST",
                dataType: "json",
                cache: false,
                success: function (data) {
                    if (data != null && data != "") {
                        $("#iID_DonViQuanLyID").html(data);
                    }
                    else {
                        $("#iID_DuAnID").html("<option>--Chọn--</option>");
                        $("#iID_DonViQuanLyID").html("<option>--Chọn--</option>");
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
        var loaiQuyetDinh = $("#bLaTongDuToan").val();

        if (this.value != "" && this.value != GUID_EMPTY) {
            //$.ajax({
            //    url: "/QLVonDauTu/QLPheDuyetDuAn/LayDanhSachDuAnTheoDonViQuanLy",
            //    type: "POST",
            //    data: { iID_DonViQuanLyID: this.value },
            //    dataType: "json",
            //    cache: false,
            //    success: function (data) {
            //        if (data != null && data != "") {
            //            $("#iID_DuAnID").html(data);
            //        }
            //    }
            //})

            $.ajax({
                url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/LayDuAnByDonViQLVaLoaiQuyetDinh",
                type: "POST",
                data: { iID_DonViQuanLyID: this.value, loaiQuyetDinh: loaiQuyetDinh },
                dataType: "json",
                cache: false,
                success: function (data) {
                    if (data != null && data != "") {
                        $("#iID_DuAnID").html(data);
                    }
                    else {
                        $("#iID_DuAnID").html("<option>--Chọn--</option>");
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

function ResetListNguonVonTheoPheDuyetDuAnWhenChangeDuAn() {
    var items = GetListDataNguonVon();

    if (!arrHasValue(items)) {
        return;
    }

    var result = [];

    items.forEach(x => {
        if (isStringEmpty(x.type)) {
            result.push(x);
        }
    });

    arrNguonVon = result;
}

function GetDanhSachNguonVonTheoDuAn(id) {
    ResetListNguonVonTheoPheDuyetDuAnWhenChangeDuAn();
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/GetListNguonVonByPheDuyetDuAn",
        type: "POST",
        data: { idDuAn: id },
        dataType: "json",
        cache: false,
        success: function (resp) {
            if (resp.status == false) {
                return;
            }

            if (arrHasValue(resp.data)) {
                resp.data.forEach(x => {
                    var item = {
                        Id: uuidv4(),
                        iID_NguonVonID: x.iID_NguonVonID,
                        sTenNguonVon: x.sTenNguonVon,
                        fTienPheDuyet: x.fTienPheDuyet,
                        type: "CHU_TRUONG_DAU_TU"
                    };

                    tongNguonVonBanDau += item.fTienPheDuyet;
                    arrNguonVon.push(item);
                });
            }

            LoadDataViewNguonVon();
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

    if (value != undefined && value != "") {
        return "<select disabled class='form-control'>" + htmlOption + "</option>";
    } else {
        return "<select class='form-control'>" + htmlOption + "</option>";
    }
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


function GetDanhSachChiPhiTheoDuAn(id) {

    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/GetListChiPhiByPheDuyetDuAn",
        type: "POST",
        data: { idDuAn: id },
        dataType: "json",
        cache: false,
        success: function (resp) {
            if (resp.status == false) {
                return;
            }

            if (arrHasValue(resp.data)) {
                arrChiPhi = resp.data;
                
            }
            UpdateChiPhiCanEdit(arrChiPhi);
            LoadDataViewChiPhi();
        }
    });
}



function GetDanhSachHangMucTheoDuAn(id) {

    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/GetListHangMucByPheDuyetDuAn",
        type: "POST",
        data: { idDuAn: id },
        dataType: "json",
        cache: false,
        success: function (resp) {
            if (resp.status == false) {
                return;
            }

            if (arrHasValue(resp.data)) {
                arrHangMuc = resp.data;
                UpdateHangMucCanEdit(arrHangMuc);
            }

        }
    });
}

function UpdateHangMucCanEdit(arrayHangMucParam) {

    arrayHangMucParam.forEach(obj => {
        var checkIsParent = CheckHangMucIsParent(obj.iID_QDDauTu_DM_HangMucID, arrayHangMucParam);
        if (checkIsParent) {
            obj.isEdit = false;
            obj.isEdit = false;
        } else {
            obj.isEdit = true;
        }

        obj.iID_HangMucID = "";
        obj.iID_DuToan_DM_HangMucID = obj.iID_QDDauTu_DM_HangMucID;
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

function SaveThietKeThiCong() {
    var item = GetFormDataThietKeThiCong();

    if (!ValidateBeforeSaveThietKeThiCong(item)) {
        return false;
    }

    if (!ValidateBeforeTongTienNguonVonWithChiPhi()) {
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

function GetFormDataThietKeThiCong() {
    var soQuyetDinh = $("#sSoQuyetDinh").val();
    var ngayPheDuyet = $("#dNgayQuyetDinh").val();
    var stongDuToanPheDuyet = $("#fTongMucDauTu").val();
    var fTongDuToanPheDuyet = parseFloat(UnFormatNumber(stongDuToanPheDuyet));
    
    var loaiQuyetDinh = $("#bLaTongDuToan").val();
    var donViQuanLy = $("#iID_DonViQuanLyID").val();
    var duAn = $("#iID_DuAnID").val();
    var diaDiem = $("#sDiaDiem").val();
    var tenDuToan = $("#sTenDuToan").val();

    return {
        sSoQuyetDinh: soQuyetDinh,
        dNgayQuyetDinh: ngayPheDuyet,
        fTongDuToanPheDuyet: fTongDuToanPheDuyet,
        sTenDuToan: tenDuToan,
        iLoaiQuyetDinh: loaiQuyetDinh,
        iID_DuAnID: duAn,
        sDiaDiem: diaDiem,
        ListChiPhi: arrChiPhiSave,
        ListNguonVon: arrNguonVonSave,
        ListHangMuc: arrHangMucSave
    };
}


function ValidateBeforeSaveThietKeThiCong(item) {
    var message = [];

    if (isStringEmpty(item.sSoQuyetDinh)) {
        message.push("Vui lòng nhập số quyết định !");
    }

    if (isStringEmpty(item.dNgayQuyetDinh)) {
        message.push("Vui lòng nhập ngày quyết định !");
    }

    if (isStringEmpty(item.iLoaiQuyetDinh)) {
        message.push("Vui lòng chọn loại quyết định !");
    }

    if (message.length > 0) {
        PopupModal("Lỗi lưu thiết kế thi công tổng dự toán", message, ERROR);
        return false;
    }

    return true;
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
        PopupModal("Lỗi lưu phê duyệt dự án", ["Tổng tiền nguồn vốn và tổng tiền chi phí không bằng nhau !"], ERROR);
        return false;
    }

    return true;
}


