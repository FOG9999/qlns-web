var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var TBL_NCCQCT = "tblNhuCauChiQuyChiTiet";
var arrChiQuy = [];
var lstNgoaiUSD = [];
var arr_DataHopDong = [];

$(document).ready(function ($) {
    LoadDataHopDong();
    EventLoadData();
    RebindingValidateMoney();
});
function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/NhuCauChiQuy/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalNhuCauChiQuy").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalNhuCauChiQuyLabel").html('Thêm mới nhu cầu chi quý');
            }
            else {
                $("#modalNhuCauChiQuyLabel").html('Sửa thông tin nhu cầu chi quý');
            }
        }
    });
}

function RebindingValidateMoney() {
    $(".txtChiTrongnuocVND").keydown(function (event) {
        ValidateInputKeydown(event, this, 1);
    }).blur(function (event) {
        if (ValidateInputFocusOut(event, this, 2)) {
            TinhLaiDongTong(this);
        }
    });

    $(".txtChingoaiteUSD").keydown(function (event) {
        ValidateInputKeydown(event, this, 2);
    }).blur(function (event) {
        if (ValidateInputFocusOut(event, this, 2, 2)) {
            TinhLaiDongTong(this);
        }
    });
}

function EventLoadData() {
    arrChiQuy = GetListDataChitietJson();
    lstNgoaiUSD = GetListDataChitietJson();
    LoadDataViewChitiet();

    DinhDangSo();
}

function DinhDangSo() {
    $(".sotien").each(function () {
        if ($(this).is('input'))
            $(this).val(FormatNumber(UnFormatNumber($(this).val())));
        else
            $(this).html(FormatNumber(UnFormatNumber($(this).html())));
    })
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

function GetListDataChitietJson() {
    var items = $("#arrChiQuy").val();
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
        }
    }

    return items;
}


function GetListDataNguonVon() {
    return lstNgoaiUSD;
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


//function LoadDataComboBoxChitiet() {
//    $.ajax({
//        url: "/QLNH/NhuCauChiQuy/LoadComboboxNCCQChitiet",
//        type: "POST",
//        dataType: "json",
//        async: false,
//        cache: false,
//        success: function (data) {
//            if (data.status == false) {
//                return;
//            }

//            if (data.data != null)
//                arr_NguonVon_Dropdown = data.data;
//        }
//    });
//}


function Themhopdong() {
    var dongMoi = "";
    dongMoi += "<tr style='cursor: pointer;' class='parent'>";
    dongMoi += "<td class='r_STT'></td><input type='hidden' value=''/>";
    dongMoi += "<td>" + CreateHtmlSelectChiquy() + "</td>";
    dongMoi += "<td class='r_ChingoaiteUSD' align='right'><div class='fChingoaiteUSD' hidden></div><input type='text' class='form-control txtChingoaiteUSD'/></td>";
    dongMoi += "<td class='text-right txtChingoaiteVND'></td>";
    dongMoi += "<td class='r_ChitrongnuocVND' align='right'><div class='fChitrongnuocVND' hidden></div><input type='text' class='form-control txtChiTrongnuocVND'/></td>";
    dongMoi += "<td class='text-right txtChiTrongnuocUSD'></td>";
    dongMoi += "<td><button class='btn-delete btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_NCCQCT + "\")'>" +
        "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
        "</button></td>";
    dongMoi += "</tr>";

    $("#" + TBL_NCCQCT + " tbody").append(dongMoi);
    CapNhatCotSttChiQuy(TBL_NCCQCT);
}

function Themnoidungchi() {
    var dongMoi = "";
    dongMoi += "<tr style='cursor: pointer;' class='parent'>";
    dongMoi += "<td class='r_STT'></td><input type='hidden' class='r_iID_HopDongID' value=''/>";
    dongMoi += "<td class='r_Noidung' align='right'><div class='sNoiDung' hidden></div><input type='text' maxlength='500' class='form-control txtNoidung'/></td>";
    dongMoi += "<td class='r_ChingoaiteUSD' align='right'><div class='fChingoaiteUSD' hidden></div><input type='text' class='form-control txtChingoaiteUSD'/></td>";
    dongMoi += "<td class='text-right txtChingoaiteVND'></td>";
    dongMoi += "<td class='r_ChitrongnuocVND' align='right'><div class='fChitrongnuocVND' hidden></div><input type='text' class='form-control txtChiTrongnuocVND'/></td>";
    dongMoi += "<td class='text-right txtChiTrongnuocUSD'></td>";
    //dongMoi += "<button class='btn-save btn-icon' type = 'button' onclick = 'Luu(this, \"" + TBL_NCCQCT + "\")' > " +
    //    "<i class='fa fa-floppy-o fa-lg' aria-hidden='true'></i>" +
    //    "</button> ";
    //dongMoi += "<button class='btn-edit btn-icon' hidden type = 'button' onclick = 'Sua(this, \"" + TBL_NCCQCT + "\")' > " +
    //    "<i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i>" +
    //    "</button> ";
    dongMoi += "<td><button class='btn-delete btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_NCCQCT + "\")'>" +
        "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
        "</button></td>";
    dongMoi += "</tr>";

    $("#" + TBL_NCCQCT + " tbody").append(dongMoi);
    CapNhatCotSttChiQuy(TBL_NCCQCT);
}

function CapNhatCotSttChiQuy(idBang) {
    $("#" + idBang + " tbody tr").each(function (index, tr) {
        $(tr).find('.r_STT').text(index + 1);
    });
    RebindingValidateMoney();
}


function LoadDataHopDong() {
    $.ajax({
        url: "/QLNH/NhuCauChiQuy/GetHopDongAll",
        type: "POST",
        dataType: "json",
        cache: false,
        async: false,
        success: function (data) {
            if (data.status == false) {
                return;
            }

            if (data.data != null) {
                arr_DataHopDong = data.data;
            }
        }
    });
}
function CreateHtmlSelectChiquy(value) {

    var htmlOption = "";
    arr_DataHopDong.forEach(x => {
        if (value != undefined && value == x.id)
            htmlOption += "<option value='" + x.id + "' selected>" + $("<div/>").text(x.text).html() + "</option>";
        else
            htmlOption += "<option value='" + x.id + "'>" + $("<div/>").text(x.text).html() + "</option>";
    })
    return "<select class='form-control selectHopDong'>" + htmlOption + "</option>";
}

function XoaDong(nutXoa, idBang) {
    var dongXoa = nutXoa.parentElement.parentElement;
    if (idBang == TBL_NCCQCT) {
        var iIDHopDongID = $(dongXoa).find(".r_iID_HopDongID").val();
        arrChiQuy = arrChiQuy.filter(function (x) { return x.r_iID_HopDongID != iIDHopDongID });
    } else if (idBang == TBL_HANG_MUC_CHINH) {
        var iiDDuAnHangMucID = $(dongXoa).find('.r_iID_DuAn_HangMucID').val();
        if ($("#" + idBang + " tbody tr.child[data-idparent='" + iiDDuAnHangMucID + "']").length > 0) {
            popupModal("Lỗi", "Tồn tại hạng mục con, không thể xóa.", ERROR);
            return;
        }
        arrHangMuc = arrHangMuc.filter(function (x) { return x.iID_DuAn_HangMucID != iiDDuAnHangMucID });
    }
    dongXoa.parentNode.removeChild(dongXoa);
    CapNhatCotSttChiQuy(idBang);
    TinhLaiDongTong(idBang);
}


function XoaDong(nutXoa, idBang) {
    var dongXoa = nutXoa.parentElement.parentElement;

    if (idBang == TBL_NCCQCT) {
        var rIIDHopDong = $(dongXoa).find(".selectHopDong select").val();
        var chitietId = $(dongXoa).find(".r_ID").val();
        if (chitietId != undefined && chitietId != "" && lstNgoaiUSD.length > 0) {
            for (var i = 0; i < lstNgoaiUSD.length; i++) {
                if (lstNgoaiUSD[i].Id == chitietId) {
                    lstNgoaiUSD[i].isDelete = true;
                }
            }
        } else {
            lstNgoaiUSD = lstNgoaiUSD.filter(function (x) { return x.iID_NguonVonID != rIIDHopDong });
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

function TinhLaiDongTong(idDong) {
    var sTigia = $("#fTiGia").val();
    var fTigia = sTigia == "" ? 0 : parseInt(UnFormatNumber(sTigia));
    var dongHienTai = $(idDong).closest("tr");

    var fChiNgoaiteUSD = 0;
    var sChiNgoaiteUSD = $(dongHienTai).find(".txtChingoaiteUSD").val();
    if (sChiNgoaiteUSD != "" && sChiNgoaiteUSD != null) {
        fChiNgoaiteUSD = parseInt(UnFormatNumber(sChiNgoaiteUSD));
    }
    var fChingoaiteVND = fChiNgoaiteUSD * fTigia;
    $(dongHienTai).find(".txtChingoaiteVND").html(FormatNumber(fChingoaiteVND));

    var sChiTrongnuocVND = $(dongHienTai).find(".txtChiTrongnuocVND").val();
    var fChiTrongnuocVND = 0;
    if (sChiTrongnuocVND != "" && sChiTrongnuocVND != null) {
        fChiTrongnuocVND = parseInt(UnFormatNumber(sChiTrongnuocVND));
    }
    var fChitrongnuocUSD = fTigia > 0 ? fChiTrongnuocVND / fTigia : 0;
    $(dongHienTai).find(".txtChiTrongnuocUSD").html(FormatNumber(fChitrongnuocUSD.toFixed(2)));
    var resultNgoaiUSD = 0;
    var resultNgoaiVND = 0;
    var resultTrongVND = 0;
    var resultTrongUSD = 0;
    var lstNgoaiUSD = GetNgoaiUSDTable();
    if (arrHasValue(lstNgoaiUSD)) {
        lstNgoaiUSD.forEach(x => {
            if ((x.isDelete == undefined || x.isDelete == false) && !isStringEmpty(x.fChiNgoaiTeUSD)) {
                resultNgoaiUSD += parseFloat(x.fChiNgoaiTeUSD);
                //$("#fTMDTDuKienPheDuyet").val(FormatNumber(result));
            }

            if ((x.isDelete == undefined || x.isDelete == false) && !isStringEmpty(x.fChiTrongNuocVND)) {
                resultTrongVND += parseFloat(x.fChiTrongNuocVND);
                //$("#fTMDTDuKienPheDuyet").val(FormatNumber(result));
            }
        });
    }
    $(".cpdt_tong_ngoaiUSD").html(FormatNumber(resultNgoaiUSD));
    $(".cpdt_tong_ngoaiVND").html(FormatNumber(resultNgoaiUSD * fTigia));

    $(".cpdt_tong_trongVND").html(FormatNumber(resultTrongVND));
    $(".cpdt_tong_trongUSD").html(FormatNumber((fTigia > 0 ? resultTrongVND / fTigia : 0).toFixed(2)));
}

function GetNgoaiUSDTable() {
    var lstNgoaiUSD = [];
    var sTigia = $("#fTiGia").val();
    var fTigia = sTigia == "" ? 0 : parseInt(UnFormatNumber(sTigia));
    $.each($("#tblNhuCauChiQuyChiTiet tbody tr"), function (index, item) {
        var obj = {};
        var bIsDelete = $(this).hasClass("error-row");
        var sID_NhuCauChiQuyID = $("#iID_NhuCauChiQuyID").val();
        obj.iID_NhuCauChiQuyID = sID_NhuCauChiQuyID;
        obj.IDDs = $(item).find(".r_STT").html();
        obj.ID = $(item).find(".r_ID").val();
        
        obj.fChiNgoaiTeUSD = $(item).find(".txtChingoaiteUSD").val();
        obj.fChiTrongNuocVND = $(item).find(".txtChiTrongnuocVND").val();
        obj.isDelete = bIsDelete;
        obj.iID_HopDongID = $(item).find(".selectHopDong select").val();
        obj.sNoiDung = $(item).find(".txtNoidung").val();
        if (obj.fChiNgoaiTeUSD == null || obj.fChiNgoaiTeUSD == "") {
            obj.fChiNgoaiTeUSD = 0;
        } else {
            obj.fChiNgoaiTeUSD = parseFloat(obj.fChiNgoaiTeUSD.toString().replaceAll(".", ""));
            obj.fChiNgoaiTeVND = obj.fChiNgoaiTeUSD * fTigia;
            obj.sChiNgoaiTeUSD = obj.fChiNgoaiTeUSD.toString();
            obj.sChiNgoaiTeVND = obj.fChiNgoaiTeVND.toString();
        }


        if (obj.fChiTrongNuocVND == null || obj.fChiTrongNuocVND == "") {
            obj.fChiTrongNuocVND = 0;
        } else {
            obj.fChiTrongNuocVND = parseFloat(obj.fChiTrongNuocVND.toString().replaceAll(".", ""));
            obj.fChiTrongNuocUSD = parseFloat((fTigia > 0 ? obj.fChiTrongNuocVND / fTigia : 0).toFixed(2));
            obj.sChiTrongNuocVND = obj.fChiTrongNuocVND.toString();
            obj.sChiTrongNuocUSD = obj.fChiTrongNuocUSD.toString();
        }
        lstNgoaiUSD.push(obj);
    });
    return lstNgoaiUSD;
}

function GetDataBeforeSave() {
    var resultNgoaiUSD = 0;
    var resultTrongVND = 0;
    var sTigia = $("#fTiGia").val();
    var fTigia = sTigia == "" ? 0 : parseInt(UnFormatNumber(sTigia));
    var data = {};
    var lstNgoaiUSD = data.ListNCCQChiTiet = GetNgoaiUSDTable();

    if (arrHasValue(lstNgoaiUSD)) {
        lstNgoaiUSD.forEach(x => {
            if ((x.isDelete == undefined || x.isDelete == false) && !isStringEmpty(x.fChiNgoaiTeUSD)) {
                resultNgoaiUSD += parseFloat(x.fChiNgoaiTeUSD);
                //$("#fTMDTDuKienPheDuyet").val(FormatNumber(result));
            }

            if ((x.isDelete == undefined || x.isDelete == false) && !isStringEmpty(x.fChiTrongNuocVND)) {
                resultTrongVND += parseFloat(x.fChiTrongNuocVND);
                //$("#fTMDTDuKienPheDuyet").val(FormatNumber(result));
            }
        });
    }
    data.sTongChiNgoaiTeUSD = resultNgoaiUSD.toString();
    data.sTongChiNgoaiTeVND = (resultNgoaiUSD * fTigia).toString();
    data.sTongChiTrongNuocVND = resultTrongVND.toString();
    data.sTongChiTrongNuocUSD = ((resultTrongVND / fTigia).toFixed(2)).toString();
    return data;
}

function LoadDataViewChitiet() {
    var data = GetListDataNguonVon();

    if (data != null) {
        for (var i = 0; i < data.length; i++) {
            var dongMoi = "";
            dongMoi += "<tr style='cursor: pointer;' class='parent'>";
            dongMoi += "<td class='r_STT width-50'>" + (i + 1) + "</td>";
            if (data[i].sNoiDung == null || data[i].sNoiDung == "") {
                dongMoi += "<td>" + CreateHtmlSelectChiquy(data[i].iID_HopDongID) + "</td>";
            } else {
                dongMoi += "<td class='r_Noidung' align='right'><div class='sNoiDung' hidden></div><input type='text' class='form-control txtNoidung 'value='" + data[i].sNoiDung + "'/></td>";
            }
            dongMoi += "<td class='r_ChingoaiteUSD' align='right'><div class='fChingoaiteUSD' hidden></div><input type='text' class='form-control txtChingoaiteUSD' value='" + FormatNumber(data[i].fChiNgoaiTeUSD) + "'/></td>";
            dongMoi += "<td class='text-right txtChingoaiteVND'>" + FormatNumber(data[i].fChiNgoaiTeVND) + "</td>";
            dongMoi += "<td class='r_ChitrongnuocVND' align='right'><div class='fChitrongnuocVND' hidden></div><input type='text' class='form-control txtChiTrongnuocVND' value='" + FormatNumber(data[i].fChiTrongNuocVND) + "'/></td>";
            dongMoi += "<td class='text-right txtChiTrongnuocUSD'>" + FormatNumber(data[i].fChiTrongNuocUSD) + "</td>";
            //dongMoi += "<td class='width-150' align='left'>";
            //dongMoi += "<button class='btn-delete btn-icon' type = 'button' onclick='XoaDong(this, \"" + TBL_NCCQCT + "\")' > " +
            //    "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span></button><input type='hidden' class='r_ID' value='" + data[i].ID + "'/></td>";
            dongMoi += "</tr>";

            $("#tblNhuCauChiQuyChiTiet tbody").append(dongMoi);
            TinhLaiDongTong(TBL_NCCQCT);
            BindingValidateAndSelect2();
        }
    }
}

function ValidateBeforeSave(data) {
    var message = [];
    var title = 'Lỗi lưu nhu cầu chi quý chi tiết';
    var ListNCCQChiTiet = data.ListNCCQChiTiet;
    if (ListNCCQChiTiet.length == 0) {
        message.push("Vui lòng thêm nội dung chi hoặc hợp đồng.");
    }
    for (var i = 0; i < ListNCCQChiTiet.length; i++) {
        if ((ListNCCQChiTiet[i].iID_HopDongID == null || ListNCCQChiTiet[i].iID_HopDongID == "") && (ListNCCQChiTiet[i].sNoiDung == null || ListNCCQChiTiet[i].sNoiDung == ""))  {
            message.push("Vui lòng chọn hợp đồng hoặc nhập nội dung chi.");
        }
        if (ListNCCQChiTiet[i].fChiTrongNuocVND == null || ListNCCQChiTiet[i].fChiTrongNuocVND == "") {
            message.push("Vui lòng nhập số chi trong nước.");
        }

        if (ListNCCQChiTiet[i].fChiNgoaiTeUSD == null || ListNCCQChiTiet[i].fChiNgoaiTeUSD == "") {
            message.push("Vui lòng nhập số chi ngoại tệ.");
        }
    }

    if (data.sTongChiTrongNuocVND == null || data.sTongChiTrongNuocVND == "") {
        message.push("Vui lòng nhập số chi trong nước.");
    }

    if (data.sTongChiNgoaiTeUSD == null || data.sTongChiNgoaiTeUSD == "") {
        message.push("Vui lòng nhập số chi ngoại tệ.");
    }

    if (message.length > 0) {
        popupModal(title, message, ERROR);
        return false;
    }

    return true;
}

function Luu() {
    var data = GetDataBeforeSave();
    if (!ValidateBeforeSave(data)) {
        return false;
    }
    $.ajax({
        url: "/QLNH/NhuCauChiQuy/LuuNhuCauChiQuyChiTiet",
        type: "POST",
        data: { data: data },
        dataType: "json",
        cache: false,
        success: function (resp) {
            if (resp == null || resp.status == false) {
                popupModal("Lỗi lưu dữ liệu nhu cầu chi quý chi tiết", [resp.message], ERROR);
                return;
            }
            else {
                bIsSaveSuccess = true;
                popupModal("Thông báo", "Lưu dữ liệu thành công", ERROR);
            }
        },
        error: function (data) {
        }
    })
}

function Huy() {
    window.location.href = "/QLNH/NhuCauChiQuy";
}

function ValidateData(data) {
    var Title = 'Lỗi thêm mới/chỉnh sửa nhu cầu chi quý';
    var Messages = [];

    if (data.sSodenghi == null || data.sSodenghi == "") {
        Messages.push("Chưa nhập số đề nghị");
    }

    if (data.iID_BQuanLyID == null || data.iBQuanly == GUID_EMPTY) {
        Messages.push("Chưa chọn ban quản lý !");
    }
    if (data.iID_DonViID == null || data.iDonvi == GUID_EMPTY) {
        Messages.push("Chưa chọn đơn vị quản lý !");
    }
    if (data.iNamKeHoach == null || data.inam == "") {
        Messages.push("Chưa chọn năm !");
    }
    if (data.iID_TiGiaID == null || data.iTygia == GUID_EMPTY) {
        Messages.push("Chưa chọn tỷ giá !");
    }

    //if (CheckExistMaChuDauTu(data.sId_CDT)) {
    //    Messages.push("Đã tồn tại mã nhà thầu !");
    //}

    if (Messages != null && Messages != undefined && Messages.length > 0) {
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


function popupModal(title, message, category) {
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: title, Messages: message, Category: category },
        success: function (data) {
            $("#divModalConfirm").html(data);
        }
    });
}

function BindingValidateAndSelect2() {
    $("#tblNhuCauChiQuyChiTiet tbody tr .selectHopDong").select2({ dropdownAutoWidth: true, matcher: FilterInComboBox });
}
