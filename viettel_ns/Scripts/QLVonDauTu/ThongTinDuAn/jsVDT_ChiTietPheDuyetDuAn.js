var TBL_CHI_PHI_DAU_TU = "tblChiPhiDauTu";
var TBL_NGUON_VON_DAU_TU = "tblNguonVonDauTu";
var TBL_HANG_MUC_CHINH = "tblHangMucChinh";
var TBL_TAI_LIEU_DINH_KEM = "tblThongTinTaiLieuDinhKem";
var arrChiPhi = [];
var arrChiPhiSave = [];
var arrNguonVon = [];
var arr_NguonVon_Dropdown = [];
var arr_HangMuc = [];
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';

$(document).ready(function () {
    LoadDataComboBoxNguonVon();
    $("tr").hover(function () {
        $(this).css("background-color", "#e7f8fe");
    }, function () {
        $(this).css("background-color", "");
    });

    var duanId = $("#iID_DuAnID").val();
    GetTMPDTheoChuTruongDauTu(duanId);
    DinhDangSo();

    Event();
});

function GetTMPDTheoChuTruongDauTu(id) {
    console.log(id);
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetDuAn/GetTMPDTheoChuTruongDT",
        type: "POST",
        data: { idDuAn: id },
        dataType: "json",
        cache: false,
        success: function (resp) {
            if (resp.status == true) {
                $("#fTongMucPheDuyetTheoChuTruong").text(FormatNumber(resp.data));
            }
        }
    });
}

function DinhDangSo() {
    $(".sotien").each(function () {
        $(this).html(FormatNumber($(this).html()));
    })
}



function Huy() {
    window.location.href = "/QLVonDauTu/QLPheDuyetDuAn/Index";
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

async function Event() {
    var respChiPhiDefault = await GetListDataChiPhiDefaultAjax();
    SetListDataChiPhiDefaultAjax(respChiPhiDefault);
    GetNguonVonTheoQDDauTu();

    LoadDataViewChiPhi();
    GetHangMucTheoChuTruongDauTu();
    LoadDataComboBoxLoaiCongTrinh();
}

// Chi phí

function GetListDataChiPhiDefaultAjax() {
    var qdDauTuId = $("#iIDQuyetDinhDauTuId").val();
    return $.ajax({
        url: "/QLVonDauTu/QLPheDuyetDuAn/GetListChiPhiByQDDauTu",
        type: "POST",
        data: { qdDauTuId: qdDauTuId },
        dataType: "json",
        cache: false
    });
}

function SetListDataChiPhiDefaultAjax(resp) {
    if (resp.status == false) {
        arrChiPhi = [];
        return;
    }

    if (!arrHasValue(resp.data)) {
        arrChiPhi = [];
        return;
    }

    resp.data.forEach(x => {
        if (x.iID_ChiPhi_Parent == null) {
            x.iID_ChiPhi_Parent = "";
        }
        var item = {
            Id: x.Id,
            iID_DuAn_ChiPhi: x.iID_DuAn_ChiPhi,
            iID_ChiPhi_Parent: x.iID_ChiPhi_Parent,
            iID_QDDauTu_ChiPhiID: x.iID_QDDauTu_ChiPhiID,
            iID_ChiPhiID: x.iID_ChiPhi,
            //sMaChiPhi: x.sMaChiPhi,
            sTenChiPhi: x.sTenChiPhi,
            sTenChiPhiCha: "",
            iID_ChiPhiCha: "",
            fTienPheDuyet: x.fTienPheDuyet,
            iThuTu: x.iThuTu,
            IsDefault: true
        };

        item.fTienPheDuyet = parseFloat(UnFormatNumber(item.fTienPheDuyet));
        arrChiPhi.push(item);
        UpdateChiPhiCanEdit(arrChiPhi);
        arrChiPhiSave = arrChiPhi;

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

function LoadDataViewChiPhi() {
    var items = arrChiPhi;

    var html = "";

    if (!arrHasValue(items)) {

        $("#tblChiPhiDauTu tbody").html(html);
        return;
    }

    $("#tblChiPhiDauTu tbody").html(html);

    var count = 1;
    items.forEach(x => {
        var fontWeight = x.iID_ChiPhi_Parent ? "" : "font-weight: bold;";
        var duanChiPhiId = x.iID_DuAn_ChiPhi;
        //var duanChiPhiString = duanChiPhiId.toString();
        var stt = count;
        html += '<tr>';
        html += '<input type="hidden" class="r_IThuTu" value = "' + x.iThuTu + '"/> <input type="hidden" class="r_IsLoaiChiPhi" value = "' + x.IsDefault + '"/>';
        html += '<td class="width-50" style="text-align:center;">' + x.iThuTu + "</td >"; 
        html += `<td style="${fontWeight} text-align:left;" > <input type = "text" disabled class="form-control r_TenChiPhi" value = "` + x.sTenChiPhi + '" /> </td> <input type="hidden" id="' + duanChiPhiId + '" class="r_iID_DuAn_ChiPhiID" value="' + duanChiPhiId + '"/> <input type="hidden" class="r_iID_ChiPhiParentID" value="' + x.iID_ChiPhi_Parent + '"/> <input type="hidden" class="r_iID_ChiPhiID" value = "' + x.iID_ChiPhiID + '"/>';
        html += '<td style="text-align:right"><input type="text" style="text-align: right" disabled class="form-control txtGiaTriChiPhi" onkeyup="ValidateNumberKeyUp(this); " onkeypress="return ValidateNumberKeyPress(this, event); " value="' + FormatNumber(x.fTienPheDuyet) + '"/> </td>';
        html += "<td align='center' class='width-150'>";
        if (x.isEdit) {
            html += "<button class='btn-detail' type = 'button' onclick = 'DetailChiPhi(this)' > " +
                "<i class='fa fa-eye fa-lg' aria-hidden='true'></i>" +
                "</button> ";

        } else {
            html += "<button class='btn-detail' hidden type = 'button' onclick = 'DetailChiPhi(this)' > " +
                "<i class='fa fa-eye fa-lg' aria-hidden='true'></i>" +
                "</button> ";

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

    var arrHangMucByChiPhi = arrHangMucSave.filter(x => x.iID_DuAn_ChiPhi == chiPhiId);
    arrHangMuc = arrHangMucByChiPhi;
    LoadViewHangMuc(arrHangMucByChiPhi);

    ShowModalChiTietChiPhi();
}

function ShowModalChiTietChiPhi() {
    $("#modal-listdetailchiphi").modal("show");
}

function HideModalChiTietChiPhi() {
    $("#modal-listdetailchiphi").modal("hide");
}

// End chi phí

// Nguồn vốn


function GetNguonVonTheoQDDauTu(id) {
    var qdDauTuId = $("#iIDQuyetDinhDauTuId").val();
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
                        Id: x.iID_QDDauTu_NguonVonID,
                        iID_NguonVonID: x.iID_NguonVonID,
                        sTenNguonVon: x.sTenNguonVon,
                        fTienPheDuyet: x.fTienPheDuyet
                    };

                    arrNguonVon.push(item);
                });
            }
            arrNguonVonSave = arrNguonVon;
            //SetValueTMTDPheDuyetTheoNguonVon();
            LoadDataViewNguonVon();
        }
    });
}

function GetListDataNguonVon() {
    return arr_NguonVon;
}

function LoadDataViewNguonVon() {
    var data = arrNguonVon;

    if (data != null) {
        for (var i = 0; i < data.length; i++) {
            var dongMoi = "";
            dongMoi += "<tr>";
            dongMoi += "<td class='r_STT width-50' style='text-align: center'>" + (i + 1) + "</td>";
            dongMoi += "<td><input type='hidden' class='r_iID_NguonVonID' value='" + data[i].iID_NguonVonID + "'/><div class='sTenNguonVon' hidden></div><div class='selectNguonVon'>" + CreateHtmlSelectNguonVon(data[i].iID_NguonVonID) + "</div></td>";
            dongMoi += "<td class='r_GiaTriNguonVon' ><div hidden></div><input type='text' style='text-align: right' disabled class='form-control txtGiaTriNguonVon' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value='" + FormatNumber(data[i].fTienPheDuyet) + "'/></td>"
            dongMoi += "</tr>";

            $("#tblNguonVonDauTu tbody").append(dongMoi);
        }
    }
    CalculateTienConLaiNguonVonVaChiPhi();
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

function GetTongTienConLaiNguonVonVoiChiPhi() {
    var result = 0;

    var listNguonVon = GetListDataNguonVon();
    if (arrHasValue(listNguonVon)) {
        listNguonVon.forEach(x => {
            if (!isStringEmpty(x.fTienPheDuyet)) {
                result += parseFloat(x.fTienPheDuyet);
            }
        });
    }

    var listChiPhi = GetListDataChiPhi();
    if (arrHasValue(listChiPhi)) {
        listChiPhi.forEach(x => {
            if (!isStringEmpty(x.fTienPheDuyet)) {
                result -= parseFloat(x.fTienPheDuyet);
            }
        });
    }

    return result;
}

// End nguồn vốn

// Hạng mục 
function GetHangMucTheoChuTruongDauTu() {
    var qdDauTuId = $("#iIDQuyetDinhDauTuId").val();
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetDuAn/GetListHangMucTheoQDDauTu",
        type: "POST",
        data: { qdDauTuId: qdDauTuId },
        dataType: "json",
        cache: false,
        success: function (resp) {

            if (arrHasValue(resp.data)) {
                arrHangMucChuTruong = resp.data;
                arrHangMuc = resp.data;
                arrHangMucSave = resp.data;
            }
            //GetListDataHangMuc();
            //LoadViewHangMuc();
        }
    });

}

function LoadViewHangMuc(data) {
    $("#tblHangMucChinh tbody").html("");
    //var data = GetListDataHangMuc();
    for (var i = 0; i < data.length; i++) {
        var dongMoi = "";
        var parent = data[i].iID_ParentID ? "" : "parent";
        dongMoi += `<tr style='cursor: pointer;' class='${parent}'>`;
        dongMoi += "<td class='r_STT width-50' style='text-align: center'>" + data[i].smaOrder + "</td><input type='hidden' id='" + data[i].iID_QDDauTu_DM_HangMucID + "' class='r_iID_DuAn_HangMucID' value='" + data[i].iID_QDDauTu_DM_HangMucID + "'/> <input type='hidden' class='r_HangMucID' value='" + data[i].iID_HangMucID + "'/> <input type='hidden' class='r_HangMucParentID' value='" + data[i].iID_ParentID + "'/> <input type='hidden' class='r_IsEdit' value='" + data[i].isEdit + "'/>";
        dongMoi += "<td class='r_sTenHangMuc'><div class='sTenHangMuc' hidden></div><input type='text' class='form-control txtTenHangMuc' disabled value='" + data[i].sTenHangMuc + "'/></td>"
        //dongMoi += "<td><input type='hidden' class='r_iID_LoaiCongTrinhID' value=''/><div class='sTenLoaiCongTrinh' hidden></div><div disabled class='selectLoaiCongTrinh'>" + CreateHtmlSelectLoaiCongTrinh(data[i].iID_LoaiCongTrinhID) + "</div></td>";
        if (data[i].isEdit) {
            dongMoi += "<td class='r_HanMucDauTu' align='right'><div class='fHanMucDauTu' hidden></div><input type='text' disabled style='text-align: right'  class='form-control txtHanMucDauTu' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value='" + FormatNumber(data[i].fTienPheDuyet) + "'/></td>";

        } else {
            dongMoi += "<td class='r_HanMucDauTu' align='right'><div class='fHanMucDauTu' hidden></div><input type='text' style='text-align: right' disabled  class='form-control txtHanMucDauTu' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value='" + FormatNumber(data[i].fTienPheDuyet) + "'/></td>";

        }


        dongMoi += "</tr>";

        $("#tblHangMucChinh tbody").append(dongMoi);
    }

    //$("#conlaihangmucpheduyet").html()
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
        return "<select disabled class='form-control'>" + htmlOption + "</option>";
    }
}
function CalculateTienConLaiNguonVonVaChiPhi() {
    var result = 0;
    var resultcp = 0;
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
                resultcp += parseFloat(x.fTienPheDuyet);
            }
        });
    }
    $("#cpdt_tong_giatritotrinh").html(result == 0 ? 0 : FormatNumber(result));
    $("#cpdt_tong_giatritotrinh_chiphi").html(resultcp == 0 ? 0 : FormatNumber(resultcp));   
    return result;
}




// End hạng mục

