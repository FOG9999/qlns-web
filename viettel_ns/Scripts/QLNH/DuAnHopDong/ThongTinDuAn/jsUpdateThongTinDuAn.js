var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var maTienTe = ["USD", "VND", "EUR"];
var tbListChiphi = "tbListChiphi"
var lstNgoaiUSD = [];
var arr_DataTenChiPhi = [];

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

function ChangeChuongTrinhSelect() {
    var id = $("#slbKHTongTheBQP").val();
    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTinDuAn/GetChuongTrinhTheoQDTongThe",
        data: { id: id },
        success: function (data) {
            if (data) {
                $("#slbChuongTrinh").empty().html(data.htmlChuongTrinh);
                $("#slbBQuanLy").empty().html(data.htmlQuanLy);
                $("#slbDonVi").empty().html(data.htmlDonVi);
                $("#slbChuongTrinh").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
                $("#slbBQuanLy").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
                $("#slbDonVi").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
            }
        }
    });
}
function ChangeBQuanLySelect() {
    var id = $("#slbChuongTrinh").val();
    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTinDuAn/GetBQuanLyTheoChuongTrinh",
        data: { id: id },
        success: function (data) {
            if (data) {
                $("#slbBQuanLy").empty().html(data.htmlQuanLy);
                $("#slbDonVi").empty().html(data.htmlDonVi);
                $("#slbBQuanLy").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
                $("#slbDonVi").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
            }
        }
    });
}
function ChangeDonViSelect() {
    var id = $("#slbBQuanLy").val();
    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTinDuAn/GetDonViTheoBQuanLy",
        data: { id: id },
        success: function (data) {
            if (data) {
                $("#slbDonVi").empty().html(data.htmlDonVi);
                $("#slbDonVi").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
            }
        }
    });
}

function ChangeBQPSelectImport(element) {
    var value = $(element).val();
    let index = $(element).data('index');
    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTinDuAn/GetChuongTrinhTheoQDTongThe",
        data: { id: value },
        success: function (data) {
            if (data) {
                $("#slbChuongTrinh" + index).empty().html(data.htmlChuongTrinh);
                $("#slbBQuanLy" + index).empty().html(data.htmlQuanLy);
                $("#slbDonVi" + index).empty().html(data.htmlDonVi);
                $("#slbChuongTrinh" + index).select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
                $("#slbBQuanLy" + index).select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
                $("#slbDonVi" + index).select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
            }
        }
    });
}

function ChangeChuongTrinhSelectImport(element) {
    var value = $(element).val();
    let index = $(element).data('index');
    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTinDuAn/GetBQuanLyTheoChuongTrinh",
        data: { id: value },
        success: function (data) {
            if (data) {
                $("#slbBQuanLy" + index).empty().html(data.htmlQuanLy);
                $("#slbDonVi" + index).empty().html(data.htmlDonVi);
                $("#slbBQuanLy" + index).select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
                $("#slbDonVi" + index).select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
            }
        }
    });
}

function ChangeBQuanLySelectImport(element) {
    var value = $(element).val();
    let index = $(element).data('index');
    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTinDuAn/GetDonViTheoBQuanLy",
        data: { id: value },
        success: function (data) {
            if (data) {
                $("#slbDonVi" + index).empty().html(data.htmlDonVi);
                $("#slbDonVi" + index).select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
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
    var idTiGia = $("#slbTiGia").val();
    var maGocTiGia = $("#slbTiGia option:selected").data("mg");
    var dongMois = "";
    dongMois += "<tr style='cursor: pointer;' class='parent'>";
    dongMois += "<td class='text-center r_STT'><input type='hidden' name='ID' /></td>";
    dongMois += "<td class='text-center'>" + CreateHtmlSelectTenChiPhi() + "</td>";
    dongMois += "<td class='text-center'><input name='HopDongUSD' type='text' class='form-control' onkeydown='ValidateInputKeydown(event, this, 2);' onblur='ChangeGiaTien(event, this, 2, 2);' " + (idTiGia != GUID_EMPTY && maGocTiGia.toUpperCase() != "USD" ? "readonly" : "") + "/></td>";
    dongMois += "<td class='text-center'><input name='HopDongVND' type='text' class='form-control' onkeydown='ValidateInputKeydown(event, this, 1);' onblur='ChangeGiaTien(event, this, 2, 0);' " + (idTiGia != GUID_EMPTY && maGocTiGia.toUpperCase() != "VND" ? "readonly" : "") + "/></td>";
    dongMois += "<td class='text-center'><input name='HopDongEUR' type='text' class='form-control' onkeydown='ValidateInputKeydown(event, this, 2);' onblur='ChangeGiaTien(event, this, 2, 2);' " + (idTiGia != GUID_EMPTY && maGocTiGia.toUpperCase() != "EUR" ? "readonly" : "") + "/></td>";
    dongMois += "<td class='text-center'><input name='HopDongNgoaiTeKhac' type='text' class='form-control' onkeydown='ValidateInputKeydown(event, this, 2);' onblur='ChangeGiaTien(event, this, 2, 2);' " + (idTiGia != GUID_EMPTY && maTienTe.indexOf(maGocTiGia.toUpperCase()) >= 0 ? "readonly" : "") + "/></td>";
    dongMois += "<td align='center'><button class='btn-delete btn-icon' type='button' onclick='XoaDong(this);'><i class='fa fa-trash' aria-hidden='true'></i></button></td>";
    dongMois += "</tr>";
    $("#tbListChiphi tbody").append(dongMois);
    CapNhatCotSttTS();
}

function LoadDataViewChitiet(ID) {
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
                    dongMoi += "<td class='text-center'><input type='text' name='HopDongUSD' class='form-control' onkeydown='ValidateInputKeydown(event, this, 2);' onblur='ChangeGiaTien(event, this, 2, 2);' value='" + (data[i].fGiaTriUSD != null ? FormatNumber(data[i].fGiaTriUSD.toString().replace(",", "."), 2) : "") + "' /></td>";
                    dongMoi += "<td class='text-center'><input type='text' name='HopDongVND' class='form-control' onkeydown='ValidateInputKeydown(event, this, 1);' onblur='ChangeGiaTien(event, this, 2, 0);' value='" + (data[i].fGiaTriVND != null ? FormatNumber(data[i].fGiaTriVND.toString().replace(",", "."), 0) : "") + "' /></td>";
                    dongMoi += "<td class='text-center'><input type='text' name='HopDongEUR' class='form-control' onkeydown='ValidateInputKeydown(event, this, 2);' onblur='ChangeGiaTien(event, this, 2, 2);' value='" + (data[i].fGiaTriEUR != null ? FormatNumber(data[i].fGiaTriEUR.toString().replace(",", "."), 2) : "") + "' /></td>";
                    dongMoi += "<td class='text-center'><input type='text' name='HopDongNgoaiTeKhac' class='form-control' onkeydown='ValidateInputKeydown(event, this, 2);' onblur='ChangeGiaTien(event, this, 2, 2);' value='" + (data[i].fGiaTriNgoaiTeKhac != null ? FormatNumber(data[i].fGiaTriNgoaiTeKhac.toString().replace(",", "."), 2) : "") + "' /></td>";
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
    CalculateSum();
}

function ChangeGiaTien(event, element, type, num) {
    ValidateInputFocusOut(event, element, type, num)
    var dongHienTai = $(element).closest("tr"); //*khi bao dongHienTai, dong element the tr

    if ($(element).prop("readonly")) return;//*neu o element chi doc thi return
    var idTiGia = $("#slbTiGia").val();
    var idNgoaiTeKhac = $("#slbMaNgoaiTeKhac").val();
    var maNgoaiTeKhac = $("#slbMaNgoaiTeKhac option:selected").data("tqd");//* chon ma ntk option:selected
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
    giaTriTienData.sGiaTriUSD = UnFormatNumber($(dongHienTai).find("input[name=HopDongUSD]").val());
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
    CalculateSum();
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
    //var giaTriTienData = {};
    //giaTriTienData.sGiaTriUSD = UnFormatNumber($("input[name=HopDongUSD]").val());
    //giaTriTienData.sGiaTriVND = UnFormatNumber($("input[name=HopDongVND]").val());
    //giaTriTienData.sGiaTriEUR = UnFormatNumber($("input[name=HopDongEUR]").val());
    //giaTriTienData.sGiaTriNgoaiTeKhac = UnFormatNumber($("input[name=HopDongNgoaiTeKhac]").val());

    var tienChiPhiList = GetTableChiTiet();

    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTinDuAn/ChangeTiGiaNgoaiTeKhac",
        //data: { idTiGia: idTiGia, idNgoaiTeKhac: idNgoaiTeKhac, maNgoaiTeKhac: maNgoaiTeKhac, giaTriTienData: giaTriTienData, tienChiPhiList: tienChiPhiList },
        data: { idTiGia: idTiGia, idNgoaiTeKhac: idNgoaiTeKhac, maNgoaiTeKhac: maNgoaiTeKhac, tienChiPhiList: tienChiPhiList },
        success: function (data) {
            $("input[name=HopDongNgoaiTeKhac]").prop("readonly", false);
            $("#btnLuuModal").prop("disabled", false);
            $("#btnHuyModal").prop("disabled", false);
            if (data && data.bIsComplete) {
                if (data.isChangeInputNgoaiTe) {
                    //$("input[name=HopDongNgoaiTeKhac]").val(data.sGiaTriNTKhac);
                    $.each($("#tbListChiphi tbody tr"), function (index, item) {
                        $(item).find("input[name=HopDongNgoaiTeKhac]").val(data.tienChiPhiList.filter(x => x.index == index)[0].sGiaTriNgoaiTeKhac);
                    });
                }
                $("input[name=HopDongNgoaiTeKhac]").prop("readonly", data.isReadonlyTxtMaNTKhac);
                if (data.isChangeInputCommon) {
                    //$("input[name=HopDongUSD]").val(data.sGiaTriUSD).prop("readonly", true);
                    //$("input[name=HopDongVND]").val(data.sGiaTriVND).prop("readonly", true);
                    //$("input[name=HopDongEUR]").val(data.sGiaTriEUR).prop("readonly", true);

                    $("input[name=HopDongUSD]").prop("readonly", true);
                    $("input[name=HopDongVND]").prop("readonly", true);
                    $("input[name=HopDongEUR]").prop("readonly", true);

                    $.each($("#tbListChiphi tbody tr"), function (index, item) {
                        $(item).find("input[name=HopDongUSD]").val(data.tienChiPhiList.filter(x => x.index == index)[0].sGiaTriUSD);
                        $(item).find("input[name=HopDongVND]").val(data.tienChiPhiList.filter(x => x.index == index)[0].sGiaTriVND);
                        $(item).find("input[name=HopDongEUR]").val(data.tienChiPhiList.filter(x => x.index == index)[0].sGiaTriEUR);
                    });
                }
                CalculateSum();
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

function CalculateSum() {
    let resultUSD = .00;
    let resultVND = 0;
    let resultEUR = .00;
    let resultNTK = .00;

    var tienChiPhiList = GetTableChiTiet();
    if (arrHasValue(tienChiPhiList)) {
        tienChiPhiList.forEach(x => {
            resultUSD += parseFloat(x.sGiaTriUSD == "" ? 0 : x.sGiaTriUSD);
            resultVND += parseFloat(x.sGiaTriVND == "" ? 0 : x.sGiaTriVND);
            resultEUR += parseFloat(x.sGiaTriEUR == "" ? 0 : x.sGiaTriEUR);
            resultNTK += parseFloat(x.sGiaTriNgoaiTeKhac == "" ? 0 : x.sGiaTriNgoaiTeKhac);
        });
    }
    $("input[name=tmdt_USD]").val(FormatNumber(resultUSD, 2));
    $("input[name=tmdt_VND]").val(FormatNumber(resultVND, 0));
    $("input[name=tmdt_EUR]").val(FormatNumber(resultEUR, 2));
    $("input[name=tmdt_NTK]").val(FormatNumber(resultNTK, 2));
}

function GetTableChiTiet() {
    var tienChiPhiList = [];
    $.each($("#tbListChiphi tbody tr"), function (index, item) {
        var obj = {};
        obj.index = index;
        obj.sGiaTriUSD = UnFormatNumber($(item).find("input[name=HopDongUSD]").val());
        obj.sGiaTriVND = UnFormatNumber($(item).find("input[name=HopDongVND]").val());
        obj.sGiaTriEUR = UnFormatNumber($(item).find("input[name=HopDongEUR]").val());
        obj.sGiaTriNgoaiTeKhac = UnFormatNumber($(item).find("input[name=HopDongNgoaiTeKhac]").val());
        tienChiPhiList.push(obj);
    });
    return tienChiPhiList;
}
