var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var maTienTe = ["USD", "VND", "EUR"];
var tbListChiphi = "tbListChiphi"
var lstNgoaiUSD = [];
var arr_DataTenChiPhi = [];

$(document).ready(function () {
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

    $(".txtDate").keydown(function (event) {
        ValidateInputKeydown(event, this, 3);
    }).blur(function (event) {
        setTimeout(() => {
            if (!isShowing) ValidateInputFocusOut(event, this, 3);
        }, 0);
    });

    LoadDataTenChiPhi();
    if ($("#slbMaNgoaiTeKhac").val() != GUID_EMPTY) {
        $("#iDTenNgoaiTeKhac").html($("#slbMaNgoaiTeKhac option:selected").html());
    }
    if ($("#slbTiGia").val() != GUID_EMPTY) {
        var maGoc = $("#slbTiGia option:selected").data("mg");
        if (maTienTe.indexOf(maGoc.toUpperCase()) >= 0) {
            switch (maGoc.toUpperCase()) {
                case "USD":
                    $("input[name=HopDongVND]").prop("readonly", true);
                    $("input[name=HopDongEUR]").prop("readonly", true);
                    break;
                case "VND":
                    $("input[name=HopDongUSD]").prop("readonly", true);
                    $("input[name=HopDongEUR]").prop("readonly", true);
                    break;
                case "EUR":
                    $("input[name=HopDongUSD]").prop("readonly", true);
                    $("input[name=HopDongVND]").prop("readonly", true);
                    break;
                default:
                    break;
            }
            $("input[name=HopDongNgoaiTeKhac]").prop("readonly", true);
        } else {
            if ($("#slbMaNgoaiTeKhac").val() != GUID_EMPTY) {
                $("input[name=HopDongUSD]").prop("readonly", true);
                $("input[name=HopDongVND]").prop("readonly", true);
                $("input[name=HopDongEUR]").prop("readonly", true);
            }
        }
    }
});

function LoadDataTenChiPhi() {
    $.ajax({
        async: false,
        url: "/QLNH/ThongTinDuAn/GetLookupChiPhi",
        type: "POST",
        dataType: "json",
        cache: false,
        success: function (data) {
            arr_DataTenChiPhi = data.data;
        }
    });
}

function ChangeBQPSelect() {
    var id = $("#slbKHTongTheBQP").val();
    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTinDuAn/GetChuongTrinhTheoKHBQP",
        data: { id: id },
        success: function (data) {
            if (data) {
                $("#slbDonVi").empty().html(data.htmlDV);
            }
        }
    });
}

function ChangeBQPSelectImport(element) {
    var value = $(element).val();
    let idelement = $(element).attr('id');
    let index = idelement.replace('slbKHTongTheBQP', '');
    let idElementDonVi = 'slbDonVi' + index;
    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTinDuAn/GetChuongTrinhTheoKHBQP",
        data: { id: value },
        success: function (data) {
            if (data) {
                $("#" + idElementDonVi).empty().html(data.htmlDV);
            }
        }
    });
}

function ChangeDVSelectImport(element) {
    var value = $(element).val();
    let idelement = $(element).attr('id');
    let index = idelement.replace('slbDonVi', '');
    let idElementCT = 'slbChuongTrinh' + index;

    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTinDuAn/GetChuongTrinhTheoDV",
        data: { id: value },
        success: function (data) {
            if (data) {
                $("#" + idElementCT).empty().html(data.htmlCT);
            }
        }
    });
}

function ChangeDVSelect() {
    var idBQP = $("#slbKHTongTheBQP").val();
    var id = $("#slbDonVi").val();
    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTinDuAn/GetChuongTrinhTheoDV",
        data: { id: id, idBQP: idBQP},
        success: function (data) {
            if (data) {
                $("#slbChuongTrinh").empty().html(data.htmlCT);
            }
        }
    });
}

function CreateHtmlSelectTenChiPhi(value) {
    var htmlOption = "<option value='" + GUID_EMPTY + "' selected>--Chọn chi phí--</option>";
    arr_DataTenChiPhi.forEach(x => {
        if (value != undefined && value == x.ID)
            htmlOption += "<option value='" + x.ID + "' selected>" + $("<div/>").text(x.sTenChiPhi).html() + "</option>";
        else
            htmlOption += "<option value='" + x.ID + "' >" + $("<div/>").text(x.sTenChiPhi).html() + "</option>";
    })
    return "<select class='form-control slbChiPhi' onclick='CheckTonTaiChiPhi(this)' name='iID_ChiPhiID'>" + htmlOption + "</option>";
}

function CheckTonTaiChiPhi(element) {
    var currentValue = $(element).val();
    var listDropDown = arr_DataTenChiPhi;
    var htmlOption = "";
    var revarr_DataTenChiPhi = [];
    $("#tbodyTableChiPhi tr").each(function () {
        $(this).find("select").each(function (index) {
            var data = $(this).val();
            if (data != GUID_EMPTY && data != currentValue) {
                revarr_DataTenChiPhi.push(data);
            }
        });
    });
    htmlOption += "<option value='" + GUID_EMPTY + "' selected>--Chọn chi phí--</option>";
    listDropDown.filter(x => {
        return !revarr_DataTenChiPhi.includes(x.ID);
    }).forEach(x => {
        if (currentValue != undefined && currentValue == x.ID)
            htmlOption += "<option value='" + x.ID + "' selected>" + $("<div/>").text(x.sTenChiPhi).html() + "</option>";
        else
            htmlOption += "<option value='" + x.ID + "'>" + $("<div/>").text(x.sTenChiPhi).html() + "</option>";
    })
    $(element).empty().append(htmlOption);
}

function GetListDataChitietJson() {
    var items = $("#arr_DataTenChiPhi").val();

    if (!items) {
        return [];
    }
    items = JSON.parse(items);

    if (items != undefined && items.length > 0) {
        for (var i = 0; i < items.length; i++) {
            items[i].id = (i + 1).toString();
        }
    }
    return items;
}

function arrHasValue(x) {
    if (x == null || x == undefined || x.length <= 0) {
        return false;
    }
    return true;
}

function GetListDataChiPhi() {
    return lstNgoaiUSD;
}

function CapNhatCotSttTS() {
    $("#tbListChiphi tbody tr").each(function (index, tr) {
        $(tr).find('.r_STT').text(index + 1);
    });
}

function ThemMoi() {
    var numberOfRow = $("#tbListChiphi tbody tr").length;
    if (numberOfRow >= arr_DataTenChiPhi.length) {
        return;
    }
    var dongMois = "";
    dongMois += "<tr style='cursor: pointer;' class='parent'>";
    dongMois += "<td class='text-center r_STT'><input type='hidden' name='ID' /></td>";
    dongMois += "<td class='text-center'>" + CreateHtmlSelectTenChiPhi() + "</td>";
    dongMois += "<td class='text-center'><input name='HopDongUSD' type='text' class='form-control' onkeydown='ValidateInputKeydown(event, this, 2);' onblur='ChangeGiaTien(event, this, 2, 2);' /></td>";
    dongMois += "<td class='text-center'><input name='HopDongVND' type='text' class='form-control' onkeydown='ValidateInputKeydown(event, this, 1);' onblur='ChangeGiaTien(event, this, 2, 0);' /></td>";
    dongMois += "<td class='text-center'><input name='HopDongEUR' type='text' class='form-control' onkeydown='ValidateInputKeydown(event, this, 2);' onblur='ChangeGiaTien(event, this, 2, 2);' /></td>";
    dongMois += "<td class='text-center'><input name='HopDongNgoaiTeKhac' type='text' class='form-control' onkeydown='ValidateInputKeydown(event, this, 2);' onblur='ChangeGiaTien(event, this, 2, 2);' /></td>";
    dongMois += "<td align='center'><button class='btn-delete btn-icon' type='button' onclick='XoaDong(this);'><i class='fa fa-trash' aria-hidden='true'></i></button></td>";
    dongMois += "</tr>";
    $("#tbListChiphi tbody").append(dongMois);
    CapNhatCotSttTS();
}

function LoadDataViewChitiet(ID, state) {
    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTinDuAn/GetListChiPhiThongTinDuAn",
        async: false,
        data: { id: ID },
        success: function (data) {
            if (data != null) {
                for (var i = 0; i < data.length; i++) {
                    var dongMoi = "";
                    dongMoi += "<tr style='cursor: pointer;' class='parent'>";
                    dongMoi += "<td class='text-center r_STT'>" + (i + 1) + "</td>";
                    dongMoi += "<td hidden><input type='hidden' name='ID' value='" + data[i].ID + "'></td>";
                    dongMoi += "<td class='text-center'>" + CreateHtmlSelectTenChiPhi(data[i].iID_ChiPhiID) + "</td>";
                    dongMoi += "<td class='text-center'><input type='text' name='HopDongUSD' class='form-control' onkeydown='ValidateInputKeydown(event, this, 2);' onblur='ChangeGiaTien(event, this, 2, 2);' value='" + FormatNumber(data[i].fGiaTriUSD.toString().replace(",", "."), 2) + "' /></td>";
                    dongMoi += "<td class='text-center'><input type='text' name='HopDongVND' class='form-control' onkeydown='ValidateInputKeydown(event, this, 1);' onblur='ChangeGiaTien(event, this, 2, 0);' value='" + FormatNumber(data[i].fGiaTriVND.toString().replace(",", "."), 0) + "' /></td>";
                    dongMoi += "<td class='text-center'><input type='text' name='HopDongEUR' class='form-control' onkeydown='ValidateInputKeydown(event, this, 2);' onblur='ChangeGiaTien(event, this, 2, 2);' value='" + FormatNumber(data[i].fGiaTriEUR.toString().replace(",", "."), 2) + "' /></td>";
                    dongMoi += "<td class='text-center'><input type='text' name='HopDongNgoaiTeKhac' class='form-control' onkeydown='ValidateInputKeydown(event, this, 2);' onblur='ChangeGiaTien(event, this, 2, 2);' value='" + FormatNumber(data[i].fGiaTriNgoaiTeKhac.toString().replace(",", "."), 2) + "' /></td>";
                    dongMoi += "<td class='text-center'><button class='btn-delete btn-icon' type='button' onclick='XoaDong(this)'><i class='fa fa-trash' aria-hidden='true'></i></button></td>";
                    dongMoi += "</tr>";
                    $("#tbListChiphi tbody").append(dongMoi);
                    CapNhatCotSttTS();
                }
            }
        }
    });
}

function LoadDataTableChiPhi() {
    lstNgoaiUSD = GetListDataChitietJson();

    let state = $("#hState").val();
    let ID = $("#hidDuAnID").val();
    if (state == 'CREATE' || state == 'ADJUST' || state == 'UPDATE') {
        if (ID == GUID_EMPTY) {
            ID = null;
        }
        LoadDataViewChitiet(ID);
    }
}

function XoaDong(nutXoa) {
    var dongXoa = nutXoa.parentElement.parentElement;
    dongXoa.parentNode.removeChild(dongXoa);
    CapNhatCotSttTS();
    ChangeGiaTien(nutXoa);
}

function ChangeGiaTien(event, element, type, num) {
    ValidateInputFocusOut(event, element, type, num)
    console.log(ValidateInputFocusOut)
    var dongHienTai = $(element).closest("tr"); //*khi bao dongHienTai, dong element the tr

    if ($(element).prop("readonly")) return;//*neu o element chi doc thi return
    var idTiGia = $("#slbTiGia").val();
    var idNgoaiTeKhac = $("#slbMaNgoaiTeKhac").val();
    var maNgoaiTeKhac = $("#slbMaNgoaiTeKhac option:selected").html();//* chon ma ntk option:selected
    if (idTiGia == "" || idTiGia == GUID_EMPTY) {//* id rong hoac khong gt
        return;
    } else {
        if (element.name == "HopDongNgoaiTeKhac" && idNgoaiTeKhac == GUID_EMPTY) {//*neu name hopdongntk rong va intk gt bang empty
            return;
        }
    }
    var txtBlur = "";//* khai bao txtBlur
    switch (element.name) { //* name trong element
        case "HopDongUSD"://* bnag hopdongusd
            txtBlur = "USD";//*
            break;
        case "HopDongVND":
            txtBlur = "VND";
            break;
        case "HopDongEUR":
            txtBlur = "EUR";
            break;
        case "HopDongNgoaiTeKhac":
            break;
        default:
            break;
    }
    $("input[name=HopDongUSD]").prop("readonly", true);
    $("input[name=HopDongVND]").prop("readonly", true);
    $("input[name=HopDongEUR]").prop("readonly", true);
    $("input[name=HopDongNgoaiTeKhac]").prop("readonly", true);
    $("#btnLuuModal").prop("disabled", true);
    $("#btnHuyModal").prop("disabled", true);

    var giaTriTienData = {};//* khai bao var convert kieu du lieu giaTriTienData = {}object
    giaTriTienData.sGiaTriUSD = UnFormatNumber($(dongHienTai).find("input[name=HopDongUSD]").val()); //gtusd khi nhan tu html chuyen tu dinh dang ve khong dinh dang  de thuc hien tinh toan voi ti gia
    giaTriTienData.sGiaTriVND = UnFormatNumber($(dongHienTai).find("input[name=HopDongVND]").val());
    giaTriTienData.sGiaTriEUR = UnFormatNumber($(dongHienTai).find("input[name=HopDongEUR]").val());
    giaTriTienData.sGiaTriNgoaiTeKhac = UnFormatNumber($(dongHienTai).find("input[name=HopDongNgoaiTeKhac]").val());

    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTinDuAn/ChangeGiaTien",
        async: false,
        data: { idTiGia: idTiGia, idNgoaiTeKhac: idNgoaiTeKhac, maNgoaiTeKhac: maNgoaiTeKhac, txtBlur: txtBlur, giaTriTienData: giaTriTienData },
        success: function (data) {
            $("input[name=HopDongUSD]").prop("readonly", data.isChangeInputUSD);
            $("input[name=HopDongVND]").prop("readonly", data.isChangeInputVND);
            $("input[name=HopDongEUR]").prop("readonly", data.isChangeInputEUR);
            $("input[name=HopDongNgoaiTeKhac]").prop("readonly", data.isChangeInputNgoaiTe);
            $("#btnLuuModal").prop("disabled", false);
            $("#btnHuyModal").prop("disabled", false);
            if (data && data.bIsComplete) {
                if (data.isChangeInputUSD) $(dongHienTai).find("input[name=HopDongUSD]").val(data.sGiaTriUSD).prop("readonly", true);
                if (data.isChangeInputVND) $(dongHienTai).find("input[name=HopDongVND]").val(data.sGiaTriVND).prop("readonly", true);
                if (data.isChangeInputEUR) $(dongHienTai).find("input[name=HopDongEUR]").val(data.sGiaTriEUR).prop("readonly", true);
                if (data.isChangeInputNgoaiTe) $(dongHienTai).find("input[name=HopDongNgoaiTeKhac]").val(data.sGiaTriNTKhac).prop("readonly", true);
            } else {
                var Title = 'Lỗi tính giá trị thông tin dự án';
                var messErr = [];
                messErr.push(data.sMessError);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (res) {
                        $("#divModalConfirm").html(res);
                    }
                });
            }
        }
    });
    let resultUSD = .00;
    let resultVND = 0;
    let resultEUR = .00;
    let resultNTK = .00;

    var lstNgoaiUSD = GetTableChiTiet();
    if (arrHasValue(lstNgoaiUSD)) {
        lstNgoaiUSD.forEach(x => {

            resultUSD += parseFloat(UnFormatNumber(x.sGiaTriUSD == "" ? 0 : x.sGiaTriUSD));

            resultVND += parseFloat(UnFormatNumber(x.sGiaTriVND == "" ? 0 : x.sGiaTriVND));

            resultEUR += parseFloat(UnFormatNumber(x.sGiaTriEUR == "" ? 0 : x.sGiaTriEUR));

            resultNTK += parseFloat(UnFormatNumber(x.sGiaTriNgoaiTeKhac == "" ? 0 : x.sGiaTriNgoaiTeKhac));
        });
    }
    $("input[name=tmdt_USD]").val(FormatNumber(resultUSD, 2));
    $("input[name=tmdt_VND]").val(FormatNumber(resultVND, 0));
    $("input[name=tmdt_EUR]").val(FormatNumber(resultEUR, 2));
    $("input[name=tmdt_NTK]").val(FormatNumber(resultNTK, 2));
}

function GetTableChiTiet() {
    var lstNgoaiUSD = [];
    $.each($("#tbListChiphi tbody tr"), function (_index, item) {
        var obj = {};
        obj.sGiaTriUSD = $(item).find("input[name=HopDongUSD]").val();
        obj.sGiaTriVND = $(item).find("input[name=HopDongVND]").val();
        obj.sGiaTriEUR = $(item).find("input[name=HopDongEUR]").val();
        obj.sGiaTriNgoaiTeKhac = $(item).find("input[name=HopDongNgoaiTeKhac]").val();
        lstNgoaiUSD.push(obj);
    });
    return lstNgoaiUSD;
}

function ChangeTiGiaSelect() {
    $("input[name=HopDongUSD]").prop("readonly", true);
    $("input[name=HopDongVND]").prop("readonly", true);
    $("input[name=HopDongEUR]").prop("readonly", true);
    $("input[name=HopDongNgoaiTeKhac]").prop("readonly", true);
    $("#btnLuuModal").prop("disabled", true);
    $("#btnHuyModal").prop("disabled", true);

    var giaTriTienData = {};
    //giaTriTienData.sGiaTriUSD = UnFormatNumber($("input[name=HopDongUSD]").val());
    //giaTriTienData.sGiaTriVND = UnFormatNumber($("input[name=HopDongVND]").val());
    //giaTriTienData.sGiaTriEUR = UnFormatNumber($("input[name=HopDongEUR]").val());
    var idTiGia = $("#slbTiGia").val();
    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTinDuAn/ChangeTiGia",
        data: { idTiGia: idTiGia, giaTriTienData: giaTriTienData },
        success: function (data) {
            $("input[name=HopDongUSD]").prop("readonly", false);
            $("input[name=HopDongVND]").prop("readonly", false);
            $("input[name=HopDongEUR]").prop("readonly", false);
            $("input[name=HopDongNgoaiTeKhac]").prop("readonly", false);
            $("#btnLuuModal").prop("disabled", false);
            $("#btnHuyModal").prop("disabled", false);
            if (data && data.bIsComplete) {
                if (idTiGia != "" && idTiGia != GUID_EMPTY) {
                    $("#slbMaNgoaiTeKhac").empty().html(data.htmlMNTK);
                    if (data.isChangeInputUSD) $("input[name=HopDongUSD]").val(data.sGiaTriUSD).prop("readonly", true);
                    if (data.isChangeInputVND) $("input[name=HopDongVND]").val(data.sGiaTriVND).prop("readonly", true);
                    if (data.isChangeInputEUR) $("input[name=HopDongEUR]").val(data.sGiaTriEUR).prop("readonly", true);
                    $("input[name=HopDongNgoaiTeKhac]").val("").prop("disabled", data.isReadonlyTxtMaNTKhac);
                } else {
                    $("#slbMaNgoaiTeKhac").val(GUID_EMPTY);
                    $("#iDTenNgoaiTeKhac").html("Ngoại tệ khác");
                    $("input[name=HopDongNgoaiTeKhac]").val("").prop("disabled", false);
                }
                $("#tienTeQuyDoiID").html(data.htmlTienTe);
            } else {
                var Title = 'Lỗi tính giá trị thông tin dự án';
                var messErr = [];
                messErr.push(data.sMessError);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (res) {
                        $("#divModalConfirm").html(res);
                    }
                });
            }
        }
    });
}

function ChangeNgoaiTeKhacSelect() {
    var idTiGia = $("#slbTiGia").val();
    var idNgoaiTeKhac = $("#slbMaNgoaiTeKhac").val();
    var maNgoaiTeKhac = $("#slbMaNgoaiTeKhac option:selected").html();
    if (idNgoaiTeKhac == GUID_EMPTY) {
        $("#iDTenNgoaiTeKhac").html("Ngoại tệ khác");
        if (idTiGia != "" && idTiGia != GUID_EMPTY) {
            if (maTienTe.indexOf($("#slbTiGia option:selected").data("mg").toUpperCase()) >= 0) {
                $("input[name=HopDongNgoaiTeKhac]").val("").prop("disabled", true);
            } else {
                $("input[name=HopDongNgoaiTeKhac]").val("").prop("disabled", false);
                $("input[name=HopDongUSD]").val("").prop("readonly", true);
                $("input[name=HopDongVND]").val("").prop("readonly", true);
                $("input[name=HopDongEUR]").val("").prop("readonly", true);
            }
        }
    } else {
        $("#iDTenNgoaiTeKhac").html(maNgoaiTeKhac);
    }
    if (idTiGia == "" || idTiGia == GUID_EMPTY) {
        return false;
    }

    $("input[name=HopDongNgoaiTeKhac]").prop("readonly", true);
    $("#btnLuuModal").prop("disabled", true);
    $("#btnHuyModal").prop("disabled", true);
    var giaTriTienData = {};
    //giaTriTienData.sGiaTriUSD = UnFormatNumber($("input[name=HopDongUSD]").val());
    //giaTriTienData.sGiaTriVND = UnFormatNumber($("input[name=HopDongVND]").val());
    //giaTriTienData.sGiaTriEUR = UnFormatNumber($("input[name=HopDongEUR]").val());
    //giaTriTienData.sGiaTriNgoaiTeKhac = UnFormatNumber($("input[name=HopDongNgoaiTeKhac]").val());
    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTinDuAn/ChangeTiGiaNgoaiTeKhac",
        data: { idTiGia: idTiGia, idNgoaiTeKhac: idNgoaiTeKhac, maNgoaiTeKhac: maNgoaiTeKhac, giaTriTienData: giaTriTienData },
        success: function (data) {
            $("input[name=HopDongNgoaiTeKhac]").prop("readonly", false);
            $("#btnLuuModal").prop("disabled", false);
            $("#btnHuyModal").prop("disabled", false);
            if (data && data.bIsComplete) {
                if (data.isChangeInputNgoaiTe) $("input[name=HopDongNgoaiTeKhac]").val(data.sGiaTriNTKhac);
                $("input[name=HopDongNgoaiTeKhac]").prop("readonly", data.isReadonlyTxtMaNTKhac);
                if (data.isChangeInputCommon) {
                    $("input[name=HopDongUSD]").val(data.sGiaTriUSD).prop("readonly", true);
                    $("input[name=HopDongVND]").val(data.sGiaTriVND).prop("readonly", true);
                    $("input[name=HopDongEUR]").val(data.sGiaTriEUR).prop("readonly", true);
                }
                $("#tienTeQuyDoiID").empty().html(data.htmlTienTe);
            } else {
                var Title = 'Lỗi tính giá trị ngoại tệ khác thông tin dự án';
                var messErr = [];
                messErr.push(data.sMessError);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (res) {
                        $("#divModalConfirm").html(res);
                    }
                });
            }
        }
    });
}
