var guidEmpty = "00000000-0000-0000-0000-000000000000";
var objDuToan = {};
var lstDuAn = [];
var listHM = [];
var lstChiPhi = [];
var listdata = [];
var lstHangMuc = [];
var lstHangMucTemp = []; // mảng để lưu tạm các giá trị hạng mục khi bật popup, đồng bộ với lstHangMuc khi save
var strCbxNguonVon = "";
var TBL_HANG_MUC_CHINH = "tblHangMucChinh";
var bIsDieuChinh = false;
var iIndexNguonVon = -1;
var iIndexHangMuc = -1;
var arrHangMucSave = [];
var deleteRowClass = "error-row";
var arr_LoaiCongTrinh = [];
var bIsSaveSuccess = false;
var ERROR = 1;
var fTongNguonVon = 0, fTongChiPhi = 0;

$(document).ready(function () {
    bIsDieuChinh = $("#bIsDieuChinh").val();
    GetCbxDonVi();
    GetDropDownNguonVon();
    LoadDataComboBoxLoaiCongTrinh();
    SetupItem();

    $("#bLaTongDuToan").val(parseInt($('#bLaTongDuToan_val').val()));

    $("#bLaTongDuToan").on("change", function () {
        if ($("#bLaTongDuToan").val() == "0") {
            $("#divTenDuToan").show();
        } else {
            $("#divTenDuToan").hide();
        }
    });
    $("#bLaTongDuToan").change();
    setTimeout(() => {
        $(".footer").appendTo(".detail-placehold")
    });
});

function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

function SetupItem() {
    var iIdDuToanId = $("#iIdDuToanId").val();
    if (iIdDuToanId != guidEmpty) {
        $("#bLaTongDuToan").attr('disabled', true);
        $("#iID_DonViQuanLyID").attr('disabled', true);
        $("#iID_DuAnID").attr('disabled', true);
    } else {
        $("#bLaTongDuToan").attr('disabled', false);
        $("#iID_DonViQuanLyID").attr('disabled', false);
        $("#iID_DuAnID").attr('disabled', false);
    }
}

function GetCbxDonVi() {
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/GetCbxDonViQuanLy",
        type: "GET",
        dataType: "json",
        success: function (result) {
            if (result.data != null && result.data != "") {
                $("#iID_DonViQuanLyID").append(result.data);
            }
            var sMaDonVi = $("#sMaDonVi").val();
            if (sMaDonVi != null && sMaDonVi != '') {
                $("#iID_DonViQuanLyID").val(sMaDonVi);
                $("#iID_DonViQuanLyID").change();
            }
        }
    });
}

function comparesMaOrder(a, b) {
    var result;

    a = a.smaOrder.split('-'),
        b = b.smaOrder.split('-');

    while (a.length) {
        result = a.shift() - (b.shift() || 0);

        if (result) {
            return result;
        }
    }

    return -b.length;
}

function TaoMaOrderHangMucMoi(parentId) {
    var iIdDuAnChiPhi = $("#txtIdChiPhiHangMuc").val();
    var sMaOrder = "";
    var indexRow = -1;
    var arrHangMucOrder = lstHangMuc.filter(x => x.iID_DuAn_ChiPhi == "" || x.iID_DuAn_ChiPhi == null || x.iID_DuAn_ChiPhi == iIdDuAnChiPhi).sort(comparesMaOrder);
    if (parentId == "" || parentId == null) {
        var arrHangMucParent = arrHangMucOrder.filter(x => x.iID_ParentID == null);
        var sMaOrderLast = "0";
        if (arrHangMucParent != null && arrHangMucParent.length > 0)
            sMaOrderLast = arrHangMucParent[arrHangMucParent.length - 1].smaOrder;
        sMaOrder = (parseInt(sMaOrderLast) + 1).toString();
        indexRow = arrHangMucOrder.findLastIndex(x => x.smaOrder.startsWith(sMaOrderLast));
    } else {
        var objParent = arrHangMucOrder.filter(x => x.iID_HangMucID == parentId)[0];
        var arrHangMucChild = arrHangMucOrder.filter(x => x.iID_ParentID == parentId);
        if (arrHangMucChild.length == 0) {
            sMaOrder = objParent.smaOrder + "-1";
            indexRow = arrHangMucOrder.findLastIndex(x => x.smaOrder.startsWith(objParent.smaOrder));
        } else {
            var sMaOrderLast = arrHangMucChild[arrHangMucChild.length - 1].smaOrder;
            var arrMaOrderSplit = sMaOrderLast.split("-");
            sMaOrder = objParent.smaOrder + "-" + (parseInt(arrMaOrderSplit[arrMaOrderSplit.length - 1]) + 1).toString();
            indexRow = arrHangMucOrder.findLastIndex(x => x.smaOrder.startsWith(sMaOrderLast));
        }
    }
    return {
        sMaOrder: sMaOrder,
        indexRow: indexRow
    }
}

function ThemMoiHangMuc() {
    var hangMucId = uuidv4();
    var iIdDuAnChiPhi = $("#txtIdChiPhiHangMuc").val();
    var objInfoDongMoi = TaoMaOrderHangMucMoi("");
    var dongMoi = "";
    dongMoi += "<tr style='cursor: pointer;' class='parent' data-id='" + hangMucId + "' data-iidduanchiphi='" + iIdDuAnChiPhi + "'>";
    dongMoi += "<td class='r_STT width-50'>" + objInfoDongMoi.sMaOrder + "</td>";
    dongMoi += "<td class='r_sTenHangMuc'><input type='text' id='sTenHangMuc' onblur='UpdateHangMuc(this)' class='form-control sTenHangMuc'/></td>"
    //dongMoi += "<td><div class='selectLoaiCongTrinh'>" + CreateHtmlSelectLoaiCongTrinh(objInfoDongMoi.iID_LoaiCongTrinhID) + "</div></td>";
    dongMoi += "<td class='fGiaTriPDDA' align='right'><input type='text' onblur='UpdateHangMuc(this)' style='text-align: right' class='form-control txtGiaTriPDDA' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' disabled /></td>";
    dongMoi += "<td class='fGiaTriPDTKTC' align='right'><input type='text' onblur='UpdateHangMuc(this)' style='text-align: right' class='form-control fTienPheDuyet' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' /></td>";
    if ($("#bIsDieuChinh").val() == "True") {
        dongMoi += "<td class='fGiaTriPDTKTC' align='right'><input type='text' onblur='UpdateHangMuc(this)' style='text-align: right' class='form-control txtGiaTriPDDA' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' /></td>";
    }
    dongMoi += "<td class='fChenhLech' align='right'><input type='text' onblur='UpdateHangMuc(this)' style='text-align: right' class='form-control fChechLech' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' disabled /></td>";
    dongMoi += "<td align='center' class='width-100'>";
    dongMoi += "<button class='btn-add-child btn-icon' type = 'button' onclick = 'ThemMoiHangMucCon(this)' > " +
        "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-delete btn-icon' type='button' onclick='DeleteHangMuc(this, \"" + TBL_HANG_MUC_CHINH + "\")'>" +
        "<i class='fa fa-trash-o fa-lg' aria-hidden='true'></i>" +
        "</button></td>";
    dongMoi += "</tr>";

    if (objInfoDongMoi.indexRow < 0)
        $("#tblHangMucChinh > tbody").append(dongMoi);
    else
        $("#tblHangMucChinh > tbody > tr").eq(objInfoDongMoi.indexRow).after(dongMoi);
    var objHangMuc = Object.assign({}, lstHangMucTemp.filter(x => x.iID_DuAn_ChiPhi == iIdDuAnChiPhi)[0]);
    objHangMuc.iID_HangMucID = hangMucId;
    objHangMuc.smaOrder = objInfoDongMoi.sMaOrder;
    objHangMuc.sTenHangMuc = "";
    objHangMuc.iID_LoaiCongTrinhID = null;
    objHangMuc.sTenLoaiCongTrinh = "";
    objHangMuc.fTienPheDuyet = 0;
    objHangMuc.fTienPheDuyetQDDT = 0;
    objHangMuc.iID_DuAn_ChiPhi = iIdDuAnChiPhi;

    lstHangMucTemp.push(objHangMuc);
    ShowHideButtonChiPhi();
}

function ThemMoiHangMucCon(nutThem) {
    var dongHienTai = nutThem.closest("tr");
    var idHangMucHienTai = $(dongHienTai).attr("data-id");
    var objHangMucHienTai = lstHangMucTemp.filter(x => x.iID_HangMucID == idHangMucHienTai)[0];

    var objInfoDongMoi = TaoMaOrderHangMucMoi(objHangMucHienTai.iID_HangMucID);
    var hangMucId = uuidv4();
    var iIdDuAnChiPhi = $("#txtIdChiPhiHangMuc").val();
    var dongMoi = "";
    dongMoi += "<tr style='cursor: pointer;' class='parent' data-id='" + hangMucId + "' data-iidduanchiphi='" + iIdDuAnChiPhi + "'>";
    dongMoi += "<td class='r_STT width-50'>" + objInfoDongMoi.sMaOrder + "</td>";
    dongMoi += "<td class='r_sTenHangMuc'><input type='text' id='sTenHangMuc' onblur='UpdateHangMuc(this)' class='form-control sTenHangMuc'/></td>"
    dongMoi += "<td><div class='selectLoaiCongTrinh'>" + CreateHtmlSelectLoaiCongTrinh() + "</div></td>";
    dongMoi += "<td class='fGiaTriPDDA' align='right'><input type='text' onblur='UpdateHangMuc(this)' style='text-align: right' class='form-control txtGiaTriPDDA' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' disabled/></td>";
    dongMoi += "<td class='fGiaTriPDTKTC' align='right'><input type='text' id='fTienPheDuyet' onblur='UpdateHangMuc(this)' style='text-align: right' class='form-control fTienPheDuyet' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' /></td>";
    dongMoi += "<td class='fChenhLech' align='right'><input type='text' onblur='UpdateHangMuc(this)' style='text-align: right' class='form-control txtChenhLech' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' disabled/></td>";
    dongMoi += "<td align='center' class='width-100'>";

    dongMoi += "<button class='btn-add-child btn-icon' type='button' onclick='ThemMoiHangMucCon(this)' > " +
        "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" +
        "</button> ";

    dongMoi += "<button class='btn-delete btn-icon' type='button' onclick='DeleteHangMuc(this, \"" + TBL_HANG_MUC_CHINH + "\")'>" +
        "<i class='fa fa-trash-o fa-lg' aria-hidden='true'></i>" +
        "</button></td>";
    dongMoi += "</tr>";

    $("#tblHangMucChinh > tbody > tr").eq(objInfoDongMoi.indexRow).after(dongMoi);

    var objHangMuc = Object.assign({}, lstHangMucTemp.filter(x => x.iID_DuAn_ChiPhi == iIdDuAnChiPhi)[0]);
    objHangMuc.iID_HangMucID = hangMucId;
    objHangMuc.smaOrder = objInfoDongMoi.sMaOrder;
    objHangMuc.sTenHangMuc = "";
    objHangMuc.iID_ParentID = idHangMucHienTai;
    objHangMuc.iID_LoaiCongTrinhID = null;
    objHangMuc.sTenLoaiCongTrinh = "";
    objHangMuc.fTienPheDuyet = 0;
    objHangMuc.fTienPheDuyetQDDT = 0;
    objHangMuc.iID_DuAn_ChiPhi = iIdDuAnChiPhi;
    lstHangMucTemp.push(objHangMuc);
    ShowHideButtonChiPhi();
    // disable hang muc cha
    ShowHideButtonHangMuc();
}

function GetDuAn() {
    lstDuAn = [];
    $("#fTongMucDauTuTKTC").val(0);
    $("#iID_DuAnID").empty();
    var sDonVi = $("#iID_DonViQuanLyID").val();
    var sLoaiQuyetDinh = $("#bLaTongDuToan").val();
    var iIdDuToanId = $("#iIdDuToanId").val();
    if (sDonVi == null || sDonVi == "" || sLoaiQuyetDinh == null || sLoaiQuyetDinh == "") return;

    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/FindDuAn",
        type: "GET",
        dataType: "json",
        data: { iID_DonViQuanLyID: sDonVi, iLoaiQuyetDinh: sLoaiQuyetDinh, iIdDuToanId: iIdDuToanId },
        success: function (result) {
            lstDuAn = result.data;
            if (lstDuAn != null || lstDuAn != []) {
                $.each(lstDuAn, function (index, item) {
                    $("#iID_DuAnID").append("<option value='" + item.iID_DuAnID + "' data-iIdDuAn='" + item.sMaDuAn + "'>" + item.sMaDuAn + " - " + item.sTenDuAn + "</option>");
                });
                var iIdDuAn = $("#iIdDuAnId").val();
                if (iIdDuAn != guidEmpty) {
                    $("#iID_DuAnID").val(iIdDuAn);
                } else {
                    $("#iID_DuAnID").val(null);
                }
                $("#iID_DuAnID").change();
            }
        }
    });
}

function ChooseDuAn() {
    $("#fTongMucDauTuTKTC").val(0);
    var iIdDuAn = $("#iID_DuAnID option:selected").val();
    ClearDataDuAn();
    if (iIdDuAn != undefined && iIdDuAn != null && lstDuAn != null && lstDuAn != []) {
        var objDuAn = $.map(lstDuAn, function (n) { return n.iID_DuAnID == iIdDuAn ? n : null });
        if (objDuAn != null && objDuAn != []) {
            $("#sDiaDiem").val(objDuAn[0].sDiaDiem);
            $("#sKhoiCong").val(objDuAn[0].sKhoiCong);
            $("#sHoanThanh").val(objDuAn[0].sKetThuc);
            $("#fTongMucDauTu").val(FormatNumber(objDuAn[0].fTongMucDauTu));
            GetHangMuc();
            GetNguonVon();
            GetChiPhi();
            $("#tonggiatriconlainguonvon").html(FormatNumber(fTongNguonVon - fTongChiPhi));
        }
    }
}

function GetNguonVon() {
    var lstNguonVon = [];
    var iIdDuAnId = $("#iID_DuAnID").val();
    var iIdDuToanId = $("#iIdDuToanId").val();
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/GetNguonVonByDuToan",
        type: "GET",
        dataType: "json",
        async: false,
        data: { iIdDuAnId: iIdDuAnId, iIdDuToan: iIdDuToanId, bIsDieuChinh: bIsDieuChinh },
        success: function (result) {
            lstNguonVon = result.data;
            RenderNguonVon(lstNguonVon);

            // call function to apply common validators
            CallFunctionCommon();
        }
    });
}

function GetChiPhi() {
    var lstChiPhi = [];
    var iIdDuAnId = $("#iID_DuAnID").val();
    var iIdDuToanId = $("#iIdDuToanId").val();
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/GetChiPhiByDuToan",
        type: "GET",
        dataType: "json",
        async: false,
        data: { iIdDuAnId: iIdDuAnId, iIdDuToan: iIdDuToanId, bIsDieuChinh: bIsDieuChinh },
        success: function (result) {
            lstChiPhi = result.data;
            RenderChiPhi(lstChiPhi);
            ShowHideButtonChiPhi();
        }
    });
}

function GetHangMuc() {
    lstHangMuc = [];
    var iIdDuAnId = $("#iID_DuAnID").val();
    var iIdDuToanId = $("#iIdDuToanId").val();
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/GetHangMucByDuToan",
        type: "GET",
        dataType: "json",
        data: { iIdDuAnId: iIdDuAnId, iIdDuToan: iIdDuToanId, bIsDieuChinh: bIsDieuChinh },
        success: function (result) {
            lstHangMuc = result.data;
        }
    });
}

//----- Helper
function ClearDataDuAn() {
    $("#sDiaDiem").val(null);
    $("#sKhoiCong").val(null);
    $("#sHoanThanh").val(null);
    $("#fTongMucDauTu").val(null);
}

function GetDropDownNguonVon() {
    strCbxNguonVon = "<option value=''>--Chọn--</option>";
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/GetNguonVonDauTu",
        type: "GET",
        dataType: "json",
        async: false,
        success: function (result) {
            if (result.data != null && result.data != "") {
                $.each(result.data, function (index, item) {
                    strCbxNguonVon += "<option value='" + item.iID_MaNguonNganSach + "'>" + item.sTen + "</option>";
                });
            }
        }
    });
}

function RenderNguonVon(lstNguonVon) {
    var lstStrNguonVon = [];
    var fTongDauTuTKTC = 0;
    $.each(lstNguonVon, function (index, item) {
        iIndexNguonVon++;
        lstStrNguonVon.push("<tr data-fTienPheDuyetQDDT='" + item.fTienPheDuyetQDDT + "'>");
        lstStrNguonVon.push("<td class='width-50'>" + (index + 1) + "</td>");
        lstStrNguonVon.push("<td class='iID_NguonVonID'><select class='form-control'>" + strCbxNguonVon + "<select></td>");
        lstStrNguonVon.push("<td style='text-align:right'>" + FormatNumber(item.fTienPheDuyetQDDT) + "</td>");
        if (bIsDieuChinh == "True") {
            lstStrNguonVon.push("<td class='fGiaTriDieuChinh' style='text-align:right'>" + FormatNumber(item.fGiaTriTruocDieuChinh) + "</td>");
        }
        lstStrNguonVon.push("<td><input type='text' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' class='fTienPheDuyet form-control' style='text-align:right' onchange='GetTongMucDauTuTKTC();' value='" + FormatNumber(item.fTienPheDuyet) + "'/></td>");
        lstStrNguonVon.push("<td class='width-100 text-center'><button class='btn-delete'><i class='fa fa-trash-o fa-lg' aria-hidden='true' onclick='DeleteNguonVon($(this))'></i></button></td>");
        lstStrNguonVon.push("</tr>");
        fTongDauTuTKTC += item.fTienPheDuyet;
    });
    fTongNguonVon = fTongDauTuTKTC;
    $("#fTongMucDauTuTKTC").val(FormatNumber(fTongDauTuTKTC));
    $("#grdNguonVon").html(lstStrNguonVon.join(""));
    $.each($("#grdNguonVon tr"), function (index, item) {
        $(item).find(".iID_NguonVonID select").val(lstNguonVon[index].iID_NguonVonID);
    });
}

function RenderChiPhi(lstChiPhi) {
    var lstStrChiPhi = [];
    $.each(lstChiPhi, function (index, item) {
        lstStrChiPhi.push("<tr data-id='" + item.iID_DuAn_ChiPhi + "' data-parent='" + item.iID_ChiPhi_Parent + "' data-fTienPheDuyetQDDT='" + item.fTienPheDuyetQDDT + "' data-iIDChiPhiID='" + item.iID_ChiPhiID + "'>");
        lstStrChiPhi.push("<td><input type='text' class='sTenChiPhi form-control' value='" + item.sTenChiPhi + "'></td>");
        lstStrChiPhi.push("<td style='text-align:right'>" + FormatNumber(item.fTienPheDuyetQDDT) + "</td>");
        if (bIsDieuChinh == "True") {
            lstStrChiPhi.push("<td class='fGiaTriDieuChinh' style='text-align:right'>" + FormatNumber(item.fTienPheDuyet) + "</td>");
        }
        if (!item.isParent) {
            lstStrChiPhi.push("<td><input type='text' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' class='fTienPheDuyet form-control' style='text-align:right' value='" + FormatNumber(item.fTienPheDuyet) + "'/></td>");
        } else {
            lstStrChiPhi.push("<td><input type='text' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' class='fTienPheDuyet form-control' style='text-align:right' value='" + FormatNumber(item.fTienPheDuyet) + "' readonly='readonly'/></td>");
        }

        lstStrChiPhi.push("<td class='width-200 text-center'>");
        lstStrChiPhi.push("<button class='btn-add-child'><i class='fa fa-plus fa-lg' aria-hidden='true' onclick='AddChildChiPhi(\"" + item.iID_DuAn_ChiPhi + "\", $(this).closest(\"tr\").index())'></i></button>");
        if (!item.isParent) {
            lstStrChiPhi.push("<button class='btn-detail' onclick='ViewHangMuc(\"" + item.iID_DuAn_ChiPhi + "\")'><i class='fa fa-eye fa-lg' aria-hidden='true'></i></button>")
        }
        lstStrChiPhi.push("<button class='btn-delete'><i class='fa fa-trash-o fa-lg' aria-hidden='true' onclick='DeleteChiPhi($(this))'></i></button>");
        lstStrChiPhi.push("</td>");
        lstStrChiPhi.push("</tr>");

        if (!item.isParent) fTongChiPhi += item.fTienPheDuyet;
    });
    $("#grdChiPhi").html(lstStrChiPhi.join(""));
    UpdateParentRowChiPhi();
    TriggerChangeGiaTriChiPhi();
}

function UpdateParentRowChiPhi() {
    var lstChiPhi = GetChiPhiByTable();
    $.each($("#grdChiPhi tr"), function (index, item) {
        var iID_ChiPhiID = $(item).data("id");
        var iID_ChiPhi_Parent = $(item).data("parent");
        var countChild = lstChiPhi.filter(x => x.iID_ChiPhi_Parent == iID_ChiPhiID);
        if (countChild.length > 0) {
            $(item).addClass("parent-row");
        } else {
            $(item).removeClass("parent-row");
        }
    });
}


function RenderHangMuc(lstHangMucByChiPhi) {
    var lstStrHangMuc = [];
    var fTienPheDuyetQDDT = 0;
    var fTienPheDuyet = 0;
    var fGiaTriChenhLech = 0;
    var fGiaTriDieuChinh = 0;
    if (lstHangMucByChiPhi === null) {
        $("#tblHangMucChinh tbody").html(lstStrHangMuc);
        return;
    }
    $("#tblHangMucChinh tbody").html(lstStrHangMuc);
    console.log(lstHangMucByChiPhi);

    $.each(lstHangMucByChiPhi, function (index, item) {
        lstStrHangMuc.push("<tr data-id='" + item.iID_HangMucID + "' data-parent='" + item.iID_ParentID + "' data-fTienPheDuyetQDDT='" + item.fTienPheDuyetQDDT + "' data-iIDLoaiCongTrinhID='" + item.iID_LoaiCongTrinhID + "' data-iIdHangMucPhanChia='" + item.iID_HangMucPhanChia + "' data-bIsParent='" + item.isParent + "'>");
        lstStrHangMuc.push("<td class='width-50 smaOrder' name='smaOrder'>" + item.smaOrder + "</td>")
        lstStrHangMuc.push("<td class='hidden iID_HangMucID' id='iID_HangMucID' value='" + item.iID_HangMucID + "' ></td>")
        lstStrHangMuc.push("<td class='hidden iID_DuAn_ChiPhi' id='iID_DuAn_ChiPhi' value='" + item.iID_DuAn_ChiPhi + "' ></td>")
        lstStrHangMuc.push("<td class='hidden iID_ParentID' id='iID_ParentID' value='" + item.iID_ParentID + "' ></td>")
        lstStrHangMuc.push("<td class='hidden iID_LoaiCongTrinhID' id='iID_LoaiCongTrinhID' value='" + item.iID_LoaiCongTrinhID + "' ></td>")
        lstStrHangMuc.push("<td><input type='text' id='sTenHangMuc' class='sTenHangMuc form-control' value='" + item.sTenHangMuc + "'></td>");
        //lstStrHangMuc.push("<td class='sLoaiCongTrinh' id='iID_LoaiCongTrinhID'>" + CreateHtmlSelectLoaiCongTrinh(item.iID_LoaiCongTrinhID) + "</td>");
        lstStrHangMuc.push("<td class='fGiaTriPDDA' style='text-align:right' id='fTienPheDuyet' value='" + FormatNumber(item.fTienPheDuyetQDDT) + "' >" + FormatNumber(item.fTienPheDuyetQDDT) + "</td>");
        if (bIsDieuChinh == "True") {
            lstStrHangMuc.push("<td class='fGiaTriDieuChinh' style='text-align:right'  id='fGiaTriDieuChinh' value='" + FormatNumber(item.fGiaTriDieuChinh ? item.fGiaTriDieuChinh : 0) + "'>" + FormatNumber(item.fGiaTriDieuChinh ? item.fGiaTriDieuChinh : 0) + "</td>");
        }
        if (!item.isParent) {
            lstStrHangMuc.push("<td><input type='text' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' class='fTienPheDuyet form-control' id='fTienPheDuyet' style='text-align:right' value='" + FormatNumber(item.fTienPheDuyet ? item.fTienPheDuyet : 0) + "'/></td>");
        } else {
            lstStrHangMuc.push("<td><input type='text' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' class='fTienPheDuyet form-control' id='fTienPheDuyet' style='text-align:right' value='" + FormatNumber(item.fTienPheDuyet ? item.fTienPheDuyet : 0) + "' readonly='readonly'/></td>");
        }
        if (item.fTienPheDuyet != null && item.fTienPheDuyet != 0) {
            var fChenhLech = item.fTienPheDuyet - item.fTienPheDuyetQDDT;
            lstStrHangMuc.push("<td class='fChenhLech'id='fChenhLech' style='text-align:right'>" + FormatNumber(fChenhLech ? fChenhLech : 0) + "</td>");
        } else {
            lstStrHangMuc.push("<td class='fChenhLech'id='fChenhLech' style='text-align:right'></td>");
        }

        lstStrHangMuc.push("<td class='width-100 text-center'>");
        lstStrHangMuc.push("<button class='btn-add-child btn-icon' type = 'button' onclick = 'ThemMoiHangMucCon(this)' > " + "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" + "</button> ");
        lstStrHangMuc.push("<button class='btn-delete'><i class='fa fa-trash-o fa-lg' aria-hidden='true' onclick='DeleteHangMuc($(this))'></i></button>");
        lstStrHangMuc.push("</td>");
        lstStrHangMuc.push("</tr>");
        if (item.iID_ParentID == null) {
            fTienPheDuyetQDDT = fTienPheDuyetQDDT + item.fTienPheDuyetQDDT;
            fTienPheDuyet = fTienPheDuyet + item.fTienPheDuyet;
            fGiaTriDieuChinh = fGiaTriDieuChinh + item.fGiaTriDieuChinh;
        }

    });
    $("#tblHangMucChinh tbody").html(lstStrHangMuc.join(""));
    fGiaTriChenhLech = fTienPheDuyet - fTienPheDuyetQDDT;
    $(".fSumGiaTriPDDA").html('<b>' + FormatNumber(fTienPheDuyetQDDT) + '</b>');
    $(".fSumGiaTriPDTKTC").html('<b>' + FormatNumber(fTienPheDuyet) + '</b>');
    $(".fSumGiaTriTrcDieuChinh").html('<b>' + FormatNumber(fGiaTriDieuChinh) + '</b>');
    if (fGiaTriChenhLech > 0) {
        $(".fSumChenhLech").html('<b>' + FormatNumber(fGiaTriChenhLech) + '</b>');
    }
    /* In đậm với các  root cha*/
    $("#tblHangMucChinh tr").each(function (tr) {
        if ($(this).data('parent') == null) {
            $(this).find('td').css('font-weight', 'bold');
        }
    })


    TriggerChangeGiaTriHangMuc();
    UpdateRowTongHangMuc();
}

function AddNguonVon() {
    var lstStrNguonVon = [];
    iIndexNguonVon++;
    lstStrNguonVon.push("<tr data-fTienPheDuyetQDDT='0'>");
    lstStrNguonVon.push("<td class='width-50'>" + (iIndexNguonVon + 1) + "</td>");
    lstStrNguonVon.push("<td class='iID_NguonVonID'><select class='form-control'>" + strCbxNguonVon + "<select></td>");
    lstStrNguonVon.push("<td style='text-align:right'></td>");
    if (bIsDieuChinh == "True") {
        lstStrNguonVon.push("<td class='fGiaTriDieuChinh' style='text-align:right'></td>");
    }
    lstStrNguonVon.push("<td><input type='text' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' class='fTienPheDuyet form-control' style='text-align:right' onchange='GetTongMucDauTuTKTC();' value=''/></td>");
    lstStrNguonVon.push("<td class='width-100 text-center'><button class='btn-delete'><i class='fa fa-trash-o fa-lg' aria-hidden='true' onclick='DeleteNguonVon($(this))'></i></button></td>");
    lstStrNguonVon.push("</tr>");
    $("#grdNguonVon").append(lstStrNguonVon.join(""));

    GetConLai();
}

function AddChildChiPhi(iIdParentId, parentIndex) {
    var currentIndex = RecusiveIndexChiPhiByParent(iIdParentId);
    var iIdChiPhiId = $("#grdChiPhi [data-id='" + iIdParentId + "']").closest("tr").data("iidchiphiid");

    if (currentIndex == -1) currentIndex = parentIndex;

    var iIdChiPhi = NewGuid();
    var lstStrChiPhi = [];
    lstStrChiPhi.push("<tr data-id='" + iIdChiPhi + "' data-parent='" + iIdParentId + "' data-fTienPheDuyetQDDT='" + 0 + "'' data-iIDChiPhiID='" + iIdChiPhiId + "'>");
    lstStrChiPhi.push("<td><input type='text' class='sTenChiPhi form-control' value=''></td>");
    lstStrChiPhi.push("<td style='text-align:right'></td>");
    if (bIsDieuChinh == "True") {
        lstStrChiPhi.push("<td class='fGiaTriDieuChinh' style='text-align:right'></td>");
    }
    lstStrChiPhi.push("<td><input type='text' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' class='fTienPheDuyet form-control' style='text-align:right' value=''/></td>");
    lstStrChiPhi.push("<td class='width-200 text-center'>");
    lstStrChiPhi.push("<button class='btn-add-child'><i class='fa fa-plus fa-lg' aria-hidden='true' onclick='AddChildChiPhi(\"" + iIdChiPhi + "\", $(this).closest(\"tr\").index())'></i></button>");
    lstStrChiPhi.push("<button class='btn-detail' onclick='ViewHangMuc(\"" + iIdChiPhi + "\")'><i class='fa fa-eye fa-lg' aria-hidden='true'></i></button>")
    lstStrChiPhi.push("<button class='btn-delete'><i class='fa fa-trash-o fa-lg' aria-hidden='true' onclick='DeleteChiPhi($(this))'></i></button>");
    lstStrChiPhi.push("</td>");
    lstStrChiPhi.push("</tr>");
    $("#grdChiPhi > tr").eq(currentIndex).after(lstStrChiPhi.join(""));
    $("#grdChiPhi [data-id='" + iIdParentId + "']").closest("tr").addClass("parent-row");
    $("#grdChiPhi [data-id='" + iIdParentId + "'] .btn-edit").hide();
    $("#grdChiPhi [data-id='" + iIdParentId + "'] .fTienPheDuyet").attr("readonly", true);
    TriggerChangeGiaTriChiPhi();

    GetConLai();
    ShowHideButtonChiPhi();
}


function DeleteNguonVon(item) {
    if ($(item).closest("tr").hasClass(deleteRowClass))
        $(item).closest("tr").removeClass(deleteRowClass);
    else
        $(item).closest("tr").addClass(deleteRowClass);
    GetTongMucDauTuTKTC();
}

function DeleteChiPhi(item) {
    if ($(item).closest("tr").hasClass(deleteRowClass))
        $(item).closest("tr").removeClass(deleteRowClass);
    else
        $(item).closest("tr").addClass(deleteRowClass);

    var iIdChiPhi = $(item).closest("tr").data('id');
    var iIdParent = $(item).closest("tr").data('parent');

    //$.each($("#grdChiPhi").find("[data-parent='" + iIdChiPhi + "']"), function (index, item) {
    //    DeleteChiPhi($(item));
    //});
    if (iIdParent != null && iIdParent != "") {
        var childLength = $("#grdChiPhi").find("[data-parent='" + iIdParent + "']").length;
        if (childLength == 0) {
            $("#grdChiPhi [data-id='" + iIdParent + "'] .fTienPheDuyet").attr("readonly", false);
            $("#grdChiPhi [data-id='" + iIdParent + "']").removeClass("parent-row");
            $("#grdChiPhi [data-id='" + iIdParent + "'] .btn-edit").show();
        }
        ResumChiPhi(iIdParent);
    }

    GetConLai();
}

function DeleteHangMuc(item) {
    var iIdHangMuc = $(item).closest("tr").data('id');
    var iIdParent = $(item).closest("tr").data('parent');
    if ($(item).closest("tr").hasClass(deleteRowClass))
        $(item).closest("tr").removeClass(deleteRowClass);
    else
        $(item).closest("tr").addClass(deleteRowClass);

    if (iIdParent != null && iIdParent != "") {
        var childLength = $("#tblHangMucChinh").find("[data-parent='" + iIdParent + "']").length;
        if (childLength == 0) {
            $("#tblHangMucChinh [data-id='" + iIdParent + "'] .fTienPheDuyet").attr("readonly", false);
            $("#tblHangMucChinh [data-id='" + iIdParent + "']").removeClass("parent-row");
        }
        ResumHangMuc(iIdParent);
    }
    UpdateRowTongHangMuc();
}

function GetTongMucDauTuTKTC() {
    var fSum = 0;
    $("#fTongMucDauTuTKTC").val(fSum);
    $.each($("#grdNguonVon tr").find(".fTienPheDuyet"), function (index, item) {
        var dongHienTai = $(item).closest("tr");
        if (!$(dongHienTai).hasClass(deleteRowClass)) {
            var sItem = $(item).val().replaceAll(".", "");
            if (sItem == "") return false;
            fSum += parseFloat(sItem);
            $("#fTongMucDauTuTKTC").val(FormatNumber(fSum));
        }
    })
    GetConLai();
}

function GetConLai() {
    var lstNguonVon = GetNguonVonByTable();
    var lstChiPhi = GetChiPhiByTable();
    var fConLai = 0;
    var fSumNguonVon = 0, fSumChiPhi = 0;
    for (var i = 0; i < lstNguonVon.length; i++) {
        if (lstNguonVon[i].isDelete == undefined || lstNguonVon[i].isDelete == false)
            fSumNguonVon += lstNguonVon[i].fTienPheDuyet;
    }

    for (var i = 0; i < lstChiPhi.length; i++) {
        if ((lstChiPhi[i].isDelete == undefined || lstChiPhi[i].isDelete == false) && lstChiPhi[i].iID_ChiPhi_Parent == null)
            fSumChiPhi += lstChiPhi[i].fTienPheDuyet;
    }

    $("#tonggiatriconlainguonvon").html(FormatNumber(fSumNguonVon - fSumChiPhi));
}

function GetNguonVonByTable() {
    var lstNguonVon = [];
    $.each($("#grdNguonVon tr"), function (index, item) {
        var obj = {};
        obj.isDelete = $(this).hasClass(deleteRowClass);
        obj.fTienPheDuyetQDDT = $(item).data("ftienpheduyetqddt");
        if (obj.fTienPheDuyetQDDT == null || obj.fTienPheDuyetQDDT == "") {
            obj.fTienPheDuyetQDDT = 0;
        } else {
            obj.fTienPheDuyetQDDT = parseFloat(obj.fTienPheDuyetQDDT.toString().replaceAll(".", ""));
        }
        obj.iID_NguonVonID = $(item).find(".iID_NguonVonID select").val();
        obj.fGiaTriDieuChinh = $(item).find(".fGiaTriDieuChinh").text();
        if (obj.fGiaTriDieuChinh == null || obj.fGiaTriDieuChinh == "") {
            obj.fGiaTriDieuChinh = 0;
        } else {
            obj.fGiaTriDieuChinh = parseFloat(obj.fGiaTriDieuChinh.toString().replaceAll(".", ""));
        }
        obj.fTienPheDuyet = $(item).find(".fTienPheDuyet").val();
        if (obj.fTienPheDuyet == null || obj.fTienPheDuyet == "") {
            obj.fTienPheDuyet = 0;
        } else {
            obj.fTienPheDuyet = parseFloat(obj.fTienPheDuyet.toString().replaceAll(".", ""));
        }
        lstNguonVon.push(obj);
    });
    return lstNguonVon;
}

function GetChiPhiByTable() {
    var lstChiPhi = [];
    $.each($("#grdChiPhi tr"), function (index, item) {
        var obj = {};
        obj.isDelete = $(this).hasClass(deleteRowClass);
        obj.iID_DuAn_ChiPhi = $(item).data("id");
        obj.sTenChiPhi = $(item).find(".sTenChiPhi").val();
        obj.iID_ChiPhiID = $(item).data("iidchiphiid");
        obj.iID_ChiPhi_Parent = $(item).data("parent");
        obj.fTienPheDuyet = $(item).find(".fTienPheDuyet").val();
        if (obj.fTienPheDuyet == null || obj.fTienPheDuyet == "") {
            obj.fTienPheDuyet = 0;
        } else {
            obj.fTienPheDuyet = parseFloat(obj.fTienPheDuyet.toString().replaceAll(".", ""));
        }
        obj.fGiaTriDieuChinh = $(item).find(".fGiaTriDieuChinh").text();
        if (obj.fGiaTriDieuChinh == null || obj.fGiaTriDieuChinh == "") {
            obj.fGiaTriDieuChinh = 0;
        } else {
            obj.fGiaTriDieuChinh = parseFloat(obj.fGiaTriDieuChinh.toString().replaceAll(".", ""));
        }
        obj.fTienPheDuyetQDDT = $(item).data("ftienpheduyetqddt");
        if (obj.fTienPheDuyetQDDT == null || obj.fTienPheDuyetQDDT == "") {
            obj.fTienPheDuyetQDDT = 0;
        } else {
            obj.fTienPheDuyetQDDT = parseFloat(obj.fTienPheDuyetQDDT.toString().replaceAll(".", ""));
        }
        lstChiPhi.push(obj);
    });
    return lstChiPhi;
}

function ViewHangMuc(iIdChiPhi) {
    var sTenChiPhi = $($("#grdChiPhi").find("[data-id='" + iIdChiPhi + "'] .sTenChiPhi")[0]).val();
    $("#txtTenChiPhi").text(sTenChiPhi);
    $("#txtIdChiPhiHangMuc").val(iIdChiPhi);
    lstHangMucTemp = lstHangMuc.filter(x => x.iID_DuAn_ChiPhi == "" || x.iID_DuAn_ChiPhi == null || x.iID_DuAn_ChiPhi == iIdChiPhi).sort(comparesMaOrder);
    RenderHangMuc(lstHangMucTemp);
    ShowHideButtonHangMuc();
    $("#modal-listdetailchiphi").modal("show");
}

//$('#modal-listdetailchiphi').on('hidden.bs.modal', function () {
//    SavehangMuc();
//});

function SavehangMuc() {
    var iIdChiPhi = $("#txtIdChiPhiHangMuc").val();
    // tính toán giá trị của tất cả các dòng đang hiển thị (kể cả đã xóa)
    $("#tblHangMucChinh tbody tr").each(function () {
        let currHM = lstHangMucTemp.find(x => x.iID_HangMucID == $(this).data("id")); // hạng mục trong lstHangMuc ban đầu lấy từ DB        

        $(this).find("input").each(function (index) {
            const fieldName = $(this).attr("id");

            if (fieldName == "fGiaTriDieuChinh") {
                currHM[fieldName] = parseFloat(UnFormatNumber($(this).val()));
            }
            if (fieldName == "fTienPheDuyet") {
                currHM[fieldName] = parseFloat(UnFormatNumber($(this).val()));
            }
            else {
                currHM[fieldName] = $(this).val();
            }

        });
        
        $(this).find("select").each(function (index) {
            const fieldName = $(this).attr("id");
            currHM[fieldName] = $(this).find("option:selected").data("madonvi")
        });

        lstHangMucTemp = lstHangMucTemp.filter(x => x.iID_HangMucID != currHM.iID_HangMucID);
        lstHangMucTemp.push(currHM);
    })

    
    var fSumTong = 0;

    // loại bỏ các hàng bị xóa
    $.each($("#tblHangMucChinh tbody tr"), function (index, item) {
        var id;
        if ($(item).closest("tr").hasClass(deleteRowClass)) {
            id = $(item).data("id");
            lstHangMucTemp = lstHangMucTemp.filter(n => n.iID_HangMucID != id);
        }
    });

    // update lstHangmuc
    lstHangMuc = lstHangMuc.filter(x => x.iID_DuAn_ChiPhi != iIdChiPhi);
    lstHangMuc = [...lstHangMuc, ...lstHangMucTemp];

    // tính toán giá trị chi phí = tổng các hạng mục còn lại chưa bị xóa
    $.each($("#tblHangMucChinh tr"), function (index, item) {
        if (!$(item).hasClass(deleteRowClass)) {
            var parent = $(item).data("parent");
            if (parent == null || parent == "") {
                var sSumTong = $(item).find(".fTienPheDuyet").val();
                if (sSumTong != null && sSumTong != "") {
                    fSumTong += parseFloat(sSumTong.replaceAll(".", ""));
                }
            }
        }
    });
    if ($("#tblHangMucChinh tr").length > 0) {
        $($("#grdChiPhi").find("[data-id='" + iIdChiPhi + "'] .fTienPheDuyet")[0]).val(FormatNumber(fSumTong));

        $($("#grdChiPhi").find("[data-id='" + iIdChiPhi + "'] .fTienPheDuyet")[0]).change();
    }
    listHM = JSON.stringify(listdata);
    HideModalChiTietChiPhi();
    ShowHideButtonChiPhi();
    addlist(iIdChiPhi);
}
function addlist(iIdChiPhi) {
    $("#tblChiPhiDauTu tr[data-id='" + iIdChiPhi + "']").attr("data-list", listHM);

}

function HideModalChiTietChiPhi() {
    $("#modal-listdetailchiphi").modal("hide");
}
function SaveThietKeThiCong() {
    var lstNguonVon = GetNguonVonByTable();
    arrHangMucSave = lstHangMuc.filter(y => y.fTienPheDuyet != 0);
    for (var i = 0; i < arrHangMucSave.length; i++) {
        if (bIsDieuChinh == 'True') {
            arrHangMucSave[i].fTienPheDuyet = lstHangMuc[i].fGiaTriDieuChinh;
        }
        else {
            arrHangMucSave[i].fTienPheDuyet = lstHangMuc[i].fTienPheDuyet;
        }
    }

    if (!ValidateDuToan(lstNguonVon)) return;

    var dataHM = [];
    $("#tblChiPhiDauTu").find("tr").each(function (index) {

        var obj = {
            id: $(this).data("id") ? $(this).data("id") : null,
            listHM: $(this).data("list") ? $(this).data("list") : []
        };
        dataHM.push(obj)
    });

    console.log(objDuToan);

    var lstChiPhi = GetChiPhiByTable();
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/Save",
        type: "POST",
        data: { objDuToan: objDuToan, lstNguonVon: lstNguonVon, lstChiPhi: lstChiPhi, lstHangMuc: arrHangMucSave, bIsDieuChinh: bIsDieuChinh },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null) {
                if (data.status == false) {
                    PopupModal("Lỗi", (data.sMessError && data.sMessError != "") ? data.sMessError : "Lỗi lưu dữ liệu phê duyệt TKTC và TDT", ERROR);
                    return false;
                }
                else {
                    bIsSaveSuccess = true;
                    var objectId = data.iID_DuToanID;
                    DieuChinhUploadFile(objectId, 104);
                    PopupModal("Thông báo", "Lưu dữ liệu thành công", ERROR);
                }
            }

        },
        error: function (data) {

        }
    })


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
                    window.location.href = "/QLVonDauTu/QLPheDuyetTKTCVaTDT";
                }
            })
        }
    });
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

function ValidateDuToan(lstNguonVon) {
    objDuToan = {};
    var iIdDuToanId = $("#iIdDuToanId").val();
    var sSoQuyetDinh = $("#sSoQuyetDinh").val();
    var dNgayQuyetDinh = $("#dNgayQuyetDinh").val();
    var sMoTa = $("#sMoTa").val();
    var bLaTongDuToan = parseInt($("#bLaTongDuToan").val()) == 1 ? true : false;
    var sTenDuToan = $("#sTenDuToan").val();
    var iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
    var iID_DuAnID = $("#iID_DuAnID").val();
    var sTongDuToanPheDuyet = $("#fTongMucDauTuTKTC").val();
    var sTongMucPDDA = $("#fTongMucDauTu").val();

    var sMessError = [];
    if (sSoQuyetDinh == undefined || sSoQuyetDinh == null || sSoQuyetDinh == "") {
        sMessError.push("Số quyết định chưa được nhập.");
    }
    if (dNgayQuyetDinh == undefined || dNgayQuyetDinh == null || dNgayQuyetDinh == "") {
        sMessError.push("Ngày phê duyệt chưa được nhập.");
    }
    if (bLaTongDuToan == undefined || bLaTongDuToan == null) {
        sMessError.push("Loại quyết định chưa được chọn.");
    }
    if (iID_DonViQuanLyID == undefined || iID_DonViQuanLyID == null) {
        sMessError.push("Đơn vị chưa được chọn.");
    }
    if (iID_DuAnID == undefined || iID_DuAnID == null) {
        sMessError.push("Dự án chưa được chọn.");
    }

    // tổng giá trị phê duyệt TKTC > tổng giá trị phê duyệt QDDT
    var fTongDuToanPheDuyet = 0;
    if (sTongDuToanPheDuyet != undefined && sTongDuToanPheDuyet != null && sTongDuToanPheDuyet != '') {
        fTongDuToanPheDuyet = parseFloat(sTongDuToanPheDuyet.toString().replaceAll(".", ""));
    }
    var fTongMucPDDA = 0;
    if (sTongMucPDDA != undefined && sTongMucPDDA != null && sTongMucPDDA != '') {
        fTongMucPDDA = parseFloat(sTongMucPDDA.toString().replaceAll(".", ""));
    }
    if (fTongDuToanPheDuyet > fTongMucPDDA) {
        sMessError.push("Tổng tiền nguồn vốn không được lớn hơn tổng tiền nguồn vốn đã được phê duyệt.");
    }

    if (sMessError != null && sMessError.length != 0) {
        alert(sMessError.join("\n"));
        return false;
    }

    if (CheckTrungNguonVon(lstNguonVon)) {
        PopupModal("Lỗi", "Nguồn vốn đã tồn tại. Vui lòng chọn lại!", ERROR);
        return false;
    }

    var isContinue = true;
    // message confirm
    if ($("#tonggiatriconlainguonvon").html() != "" || $("#tonggiatriconlainguonvon").html() != 0) {
        if (confirm("Tổng chi phí không bằng tổng nguồn vốn, bạn có chắc chắn muốn lưu dữ liệu?") == true)
            isContinue = true;
        else {
            isContinue = false;
            return false;
        }
    }

    if (isContinue) {
        objDuToan.iID_DuToanID = iIdDuToanId;
        objDuToan.iID_DuAnID = iID_DuAnID;
        objDuToan.sSoQuyetDinh = sSoQuyetDinh;
        objDuToan.dNgayQuyetDinh = dNgayQuyetDinh;
        if (sTongDuToanPheDuyet != undefined && sTongDuToanPheDuyet != null && sTongDuToanPheDuyet != "") {
            objDuToan.fTongDuToanPheDuyet = sTongDuToanPheDuyet.toString().replaceAll(".", "");
        }
        objDuToan.bLaTongDuToan = bLaTongDuToan;
        objDuToan.sTenDuToan = sTenDuToan;
        objDuToan.sMoTa = sMoTa;
        objDuToan.iID_QDDauTuID = $("#IIdQDDauTuID").val();
        objDuToan.iID_DuToanGocID = $("#iIdDuToanGocID").val();
        return true;
    }
}

function RecusiveIndexHangMucByParent(iIdParentId) {
    var itemLast = $("#tblHangMucChinh").find("[data-parent='" + iIdParentId + "']").last();
    var iId = $(itemLast).data('id');
    var currentIndex = $(itemLast).index();
    var childLength = $("#tblHangMucChinh").find("[data-parent='" + iId + "']").length;
    if (childLength == 0) return currentIndex;
    return RecusiveIndexHangMucByParent(iIdParentId);
}

function RecusiveIndexChiPhiByParent(iIdParentId) {
    var itemLast = $("#grdChiPhi").find("[data-parent='" + iIdParentId + "']").last();
    var iId = $(itemLast).data('id');
    var currentIndex = $(itemLast).index();
    var childLength = $("#grdChiPhi").find("[data-parent='" + iId + "']").length;
    if (childLength == 0) return currentIndex;
    return RecusiveIndexChiPhiByParent(iIdParentId);
}

function ResumChiPhi(id) {
    ResumGrid("grdChiPhi", id);
}

function TriggerChangeGiaTriChiPhi() {
    $("#grdChiPhi .fTienPheDuyet").on("change", function () {
        ResumChiPhi($(this).closest("tr").data("parent"));
        GetConLai();
    });
}

function ResumHangMuc(id) {
    ResumGrid("tblHangMucChinh", id);
}

function TriggerChangeGiaTriHangMuc() {
    $("#tblHangMucChinh .fTienPheDuyet").on("change", function () {
        SetChenhLechHangMuc($(this).closest("tr"))
        ResumHangMuc($(this).closest("tr").data("parent"));
        UpdateRowTongHangMuc();
    });
}

function SetChenhLechHangMuc(row) {
    var fTienPheDuyet = 0;
    var sTienPheDuyet = $(row).find(".fTienPheDuyet").val();
    if (sTienPheDuyet != undefined && sTienPheDuyet != null && sTienPheDuyet != '') {
        fTienPheDuyet = parseFloat(sTienPheDuyet.toString().replaceAll(".", ""));
    }

    var fGiaTriQDDT = 0
    var sGiaTriQDDT = $(row).data("ftienpheduyetqddt");
    if (sGiaTriQDDT != undefined && sGiaTriQDDT != null && sGiaTriQDDT != '') {
        fGiaTriQDDT = parseFloat(sGiaTriQDDT);
    }

    $(row).find(".fChenhLech").text(FormatNumber(fTienPheDuyet - fGiaTriQDDT));
}

function ResumGrid(iIdGrid, id) {
    if (id == undefined || id == null || id == "") return;
    var fSum = 0;
    var fSumChenhLech = 0;
    $.each($("#" + iIdGrid + " [data-parent='" + id + "']").closest("tr"), function (index, item) {
        if (!$(item).hasClass(deleteRowClass)) {
            var sGiaTri = $(item).find(".fTienPheDuyet").val();
            if (sGiaTri != null && sGiaTri != "") {
                fSum += parseFloat(sGiaTri.replaceAll(".", ""));
            }
            if (iIdGrid == "tblHangMucChinh") {
                var sGiaTri = $(item).find(".fChenhLech").html();
                if (sGiaTri != null && sGiaTri != "") {
                    fSumChenhLech += parseFloat(sGiaTri.replaceAll(".", ""));
                }
            }
        }
    });
    $("#" + iIdGrid + " [data-id='" + id + "'] .fTienPheDuyet").val(FormatNumber(fSum));
    if (iIdGrid == "tblHangMucChinh") {
        $("#" + iIdGrid + " [data-id='" + id + "'] .fChenhLech").html(FormatNumber(fSumChenhLech));
    }
    var parentId = $("#" + iIdGrid + " [data-id='" + id + "']").data("parent");
    ResumGrid(iIdGrid, parentId);
}

function UpdateRowTongHangMuc() {
    var fSumGiaTriPDDA = 0, fSumGiaTriPDTKTC = 0, fSumChenhLenh = 0;
    $.each($("#tblHangMucChinh tr"), function (index, item) {
        if (!$(item).hasClass(deleteRowClass)) {
            var parent = $(item).data("parent");
            if (parent == null || parent == "") {
                var sGiaTriPheDuyet = $(item).find(".fGiaTriPDDA").text();
                if (sGiaTriPheDuyet != null && sGiaTriPheDuyet != "") {
                    fSumGiaTriPDDA += parseFloat(sGiaTriPheDuyet.replaceAll(".", ""));
                }
                var sGiaTriPheDuyetTKTC = $(item).find(".fTienPheDuyet").val();
                if (sGiaTriPheDuyetTKTC != null && sGiaTriPheDuyetTKTC != "") {
                    fSumGiaTriPDTKTC += parseFloat(sGiaTriPheDuyetTKTC.replaceAll(".", ""));
                }
                var sGiaTriChenhLech = $(item).find(".fChenhLech").val();
                if (sGiaTriChenhLech != null && sGiaTriChenhLech != "") {
                    fSumChenhLenh += parseFloat(sGiaTriChenhLech.replaceAll(".", ""));
                }
            }
        }
    });
    $("#tblHangMucChinh  .fSumGiaTriPDDA").html(FormatNumber(fSumGiaTriPDDA));
    $("#tblHangMucChinh  .fSumGiaTriPDTKTC").html(FormatNumber(fSumGiaTriPDTKTC));
    //$("#tblHangMucChinh  .fSumChenhLech").html(FormatNumber(fSumChenhLenh == Number.NaN ? fSumChenhLenh == 0 : fSumChenhLenh));
    $("#tblHangMucChinh  .fSumChenhLech").html(FormatNumber(fSumGiaTriPDTKTC - fSumGiaTriPDDA));
}

function Cancel() {
    window.location.href = "/QLVonDauTu/QLPheDuyetTKTCVaTDT/Index";
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
            htmlOption += "<option data-madonvi='" + x.id + "' value='" + x.id + "'  id ='iID_LoaiCongTrinhID' selected>" + x.text + "</option>";
        else
            htmlOption += "<option data-madonvi='" + x.id + "' value='" + x.id + "' id ='iID_LoaiCongTrinhID' >" + x.text + "</option>";
    })

    return "<select class='form-control' id ='iID_LoaiCongTrinhID' name ='sLoaiCongTrinh' >" + htmlOption + "</option>";
}

function ShowHideButtonChiPhi() {
    var lstChiPhi = GetChiPhiByTable();
    lstChiPhi.forEach(x => {
        var countChild = lstChiPhi.filter(y => y.iID_ChiPhi_Parent == x.iID_DuAn_ChiPhi && (y.isDelete == null || y.isDelete == false));
        if (countChild.length > 0) {
            $("#tblChiPhiDauTu *[data-id='" + x.iID_DuAn_ChiPhi + "']").find("button.btn-delete").hide();
            $("#tblChiPhiDauTu *[data-id='" + x.iID_DuAn_ChiPhi + "']").find("button.btn-detail").hide();
            $("#tblChiPhiDauTu *[data-id='" + x.iID_DuAn_ChiPhi + "']").find(".fTienPheDuyet").attr('disabled', 'disabled');
        } else {
            $("#tblChiPhiDauTu *[data-id='" + x.iID_DuAn_ChiPhi + "']").find("button.btn-delete").show();
            $("#tblChiPhiDauTu *[data-id='" + x.iID_DuAn_ChiPhi + "']").find("button.btn-detail").show();
            $("#tblChiPhiDauTu *[data-id='" + x.iID_DuAn_ChiPhi + "']").find(".fTienPheDuyet").removeAttr('disabled');
            $("#tblChiPhiDauTu *[data-id='" + x.iID_DuAn_ChiPhi + "']").find("button.btn-add-child").show();

        }
    })
}

function ShowHideButtonHangMuc() {
    var lstHangMucShow = [];
    var iIdChiPhi = $("#txtIdChiPhiHangMuc").val();
    $.each($("#tblHangMucChinh tr"), function (index, item) {
        var objhangMuc = {};
        objhangMuc.iID_HangMucID = $(item).data("id");
        objhangMuc.iID_ParentID = $(item).data("parent");
        objhangMuc.sTenHangMuc = $(item).find(".sTenHangMuc").val();
        objhangMuc.sTenLoaiCongTrinh = $(item).find(".sLoaiCongTrinh select :selected").html();
        objhangMuc.smaOrder = $(item).find(".smaOrder").text();
        objhangMuc.iID_LoaiCongTrinhID = $(item).find(".sLoaiCongTrinh select").val();
        objhangMuc.iID_DuAn_ChiPhi = iIdChiPhi;
        objhangMuc.fTienPheDuyet = $(item).find(".fTienPheDuyet").val();
        if (objhangMuc.fTienPheDuyet == null || objhangMuc.fTienPheDuyet == "") {
            objhangMuc.fTienPheDuyet = 0;
        } else {
            objhangMuc.fTienPheDuyet = parseFloat(objhangMuc.fTienPheDuyet.toString().replaceAll(".", ""));
        }
        objhangMuc.fTienPheDuyetQDDT = $(item).data("ftienpheduyetqddt");
        objhangMuc.iID_HangMucPhanChia = $(item).data("iidhangmucphanchia");
        objhangMuc.isParent = $(item).data("bisparent");

        lstHangMucShow.push(objhangMuc);
    });

    lstHangMucShow.forEach(x => {
        var countChild = lstHangMucShow.filter(y => y.iID_ParentID == x.iID_HangMucID && (y.isDelete == null || y.isDelete == false));
        if (countChild.length > 0) {
            $("#tblHangMucChinh *[data-id='" + x.iID_HangMucID + "']").addClass("parent-row");
            $("#tblHangMucChinh *[data-id='" + x.iID_HangMucID + "']").find("button.btn-delete").hide();
            $("#tblHangMucChinh *[data-id='" + x.iID_HangMucID + "']").find("input, select").attr('disabled', 'disabled');
        } else {
            $("#tblHangMucChinh *[data-id='" + x.iID_HangMucID + "']").removeClass("parent-row");
            $("#tblHangMucChinh *[data-id='" + x.iID_HangMucID + "']").find("button.btn-delete").show();
            //$("#tblHangMucChinh *[data-id='" + x.iID_HangMucID + "']").find("input, select").removeAttr('disabled');
        }
    })
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

function UpdateHangMuc(nutCreateHangMuc) {
    var dongHienTai = nutCreateHangMuc.closest("tr");
    var id = $(dongHienTai).attr("data-id");
    var duAnChiPhiId = $("#txtIdChiPhiHangMuc").val();
    var fTienPheDuyet = $(dongHienTai).find(".txtGiaTriPDDA").val();
    var smaOrder = $(dongHienTai).find(".r_STT").text();
    var sTenHangMuc = $(dongHienTai).find(".sTenHangMuc").val();
    var iIDLoaiCongTrinhID = $(dongHienTai).find(".selectLoaiCongTrinh select").val();
    var sTenLoaiCongTrinh = $(dongHienTai).find(".selectLoaiCongTrinh select :selected").html();
    var sTienPheDuyetQDDT = $(dongHienTai).find(".fTienPheDuyet").val();
    var fTienPheDuyetQDDT = sTienPheDuyetQDDT == "" ? 0 : parseInt(UnFormatNumber(sTienPheDuyetQDDT));

    var objHangMuc = lstHangMucTemp.filter(x => x.iID_DuAn_ChiPhi == duAnChiPhiId && x.iID_HangMucID == id)[0];
    objHangMuc.iID_HangMucID = id;
    objHangMuc.smaOrder = smaOrder;
    objHangMuc.sTenHangMuc = sTenHangMuc;
    objHangMuc.iID_LoaiCongTrinhID = iIDLoaiCongTrinhID;
    objHangMuc.sTenLoaiCongTrinh = sTenLoaiCongTrinh;
    objHangMuc.fTienPheDuyet = fTienPheDuyetQDDT;
    objHangMuc.fTienPheDuyetQDDT = fTienPheDuyet;
    objHangMuc.iID_DuAn_ChiPhi = duAnChiPhiId;
    objHangMuc.fChenhLechSoVoiDuToan = fTienPheDuyetQDDT - fTienPheDuyet;
    UpdateRowTongHangMuc();
    //CalculateDataHangMucByChiPhi(objHangMuc.iID_QDDauTu_DM_HangMucID);
    //CalculateTienConLaiHangMuc();
}
function UnFormatNumber(value) {
    value = value.toString();
    if (value == "") return value;
    if (jsNumber_KieuSoTiengViet) {
        value = value.replace(/\./gi, "")//hàm đã sửa
        value = value.replace(/\,/gi, ".")//hàm đã sửa
    }
    else {
        value = value.replace(/\,/gi, "")//hàm đã sửa
    }
    //obj.value = obj.value.replace(/,/gi, ""); //hàm ban đầu

    console.log("kiemtraso: " + value);
    return value;
}
