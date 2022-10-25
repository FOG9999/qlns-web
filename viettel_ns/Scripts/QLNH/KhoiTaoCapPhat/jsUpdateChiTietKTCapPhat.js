var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var TBL_KTCPCT = "tbListChiTietKhoiTaoCapPhat";
var arr_DataDuAn = [];
var lstNgoaiUSD = [];
var arr_DataHopDong = [];

var iID_KhoiTaoCapPhatID = $("#iID_KhoiTaoCapPhatID").val()

var idDelete = [];

$(document).ready(function () {
    LoadDataHopDong();
    LoadDataDuAn();
    LoadDataViewChitiet();
    $(".selectDuAn select").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $(".selectHopDong select").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
});


function ThemHopDong() {
    var tiGia = $("#tiGiaQuyetToan").val();
    var dongMoi = "";
    dongMoi += "<tr style='cursor: pointer;' class='parent' name='isHopDong' data-isdelete='false'>";
    dongMoi += "<td><div class='selectHopDong'>" + CreateHtmlSelectHopDong() + "</div></td>";
    dongMoi += "<input type='hidden' class='form-control'  name='iID_KhoiTaoCapPhatID' value='" + iID_KhoiTaoCapPhatID + "'/>";
    dongMoi += "<td  align='right'><input type='text' onkeydown='ValidateInputKeydown(event, this, 2)'  class='form-control txtGiaTien' " + (tiGia == "USD" ? '' : 'disabled') + " onblur='TinhLaiDongTong(event,this,2,2)' name='fQTKinhPhiDuyetCacNamTruoc_USD'  /></td>";
    dongMoi += "<td  align='right'><input type='text' onkeydown='ValidateInputKeydown(event, this, 1)'  class='form-control txtGiaTien' " + (tiGia == "USD" ? 'disabled' : '') + " onblur='TinhLaiDongTong(event,this,2,0)'   name='fQTKinhPhiDuyetCacNamTruoc_VND'  /></td>";
    dongMoi += "<td  align='right'><input type='text' onkeydown='ValidateInputKeydown(event, this, 2)'  class='form-control txtGiaTien' " + (tiGia == "USD" ? '' : 'disabled') + " onblur='TinhLaiDongTong(event,this,2,2)' name='fDeNghiQTNamNay_USD'  /></td>";
    dongMoi += "<td  align='right'><input type='text' onkeydown='ValidateInputKeydown(event, this, 1)'  class='form-control txtGiaTien' " + (tiGia == "USD" ? 'disabled' : '') + " onblur='TinhLaiDongTong(event,this,2,0)'   name='fDeNghiQTNamNay_VND'  /></td>";
    dongMoi += "<td  align='right'><input type='text' onkeydown='ValidateInputKeydown(event, this, 2)'  class='form-control txtGiaTien' " + (tiGia == "USD" ? '' : 'disabled') + " onblur='TinhLaiDongTong(event,this,2,2)' name='fLuyKeKinhPhiDuocCap_USD'  /></td>";
    dongMoi += "<td  align='right'><input type='text' onkeydown='ValidateInputKeydown(event, this, 1)'  class='form-control txtGiaTien' " + (tiGia == "USD" ? 'disabled' : '') + " onblur='TinhLaiDongTong(event,this,2,0)'   name='fLuyKeKinhPhiDuocCap_VND'  /></td>";
    dongMoi += "<td  align='center'><button class='btn-delete btn-icon'  type='button' onclick='XoaDong(this)'>" +
        "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
        "</button></td>";
    dongMoi += "</tr>";

    $("#" + TBL_KTCPCT + " tbody").append(dongMoi);
    $(".selectHopDong select").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
}
function ThemDuAn() {
    var tiGia = $("#tiGiaQuyetToan").val();
    var dongMoi = "";
    dongMoi += "<tr style='cursor: pointer;' class='parent' name='isDuAn' data-isdelete='false'>";
    dongMoi += "<td><div class='selectDuAn'>" + CreateHtmlSelectDuAn() + "</div></td>";
    dongMoi += "<input type='hidden' class='form-control'  name='iID_KhoiTaoCapPhatID' value='" + iID_KhoiTaoCapPhatID + "'/>";
    dongMoi += "<td  align='right'><input type='text' onkeydown='ValidateInputKeydown(event, this, 2)'  class='form-control txtGiaTien' " + (tiGia == "USD" ? '' : 'disabled') + " onblur='TinhLaiDongTong(event,this,2,2)'  name='fQTKinhPhiDuyetCacNamTruoc_USD'  /></td>";
    dongMoi += "<td  align='right'><input type='text' onkeydown='ValidateInputKeydown(event, this, 1)'  class='form-control txtGiaTien' " + (tiGia == "USD" ? 'disabled' : '') + " onblur='TinhLaiDongTong(event,this,2,0)'  name='fQTKinhPhiDuyetCacNamTruoc_VND'  /></td>";
    dongMoi += "<td  align='right'><input type='text' onkeydown='ValidateInputKeydown(event, this, 2)'  class='form-control txtGiaTien' " + (tiGia == "USD" ? '' : 'disabled') + " onblur='TinhLaiDongTong(event,this,2,2)' name='fDeNghiQTNamNay_USD'  /></td>";
    dongMoi += "<td  align='right'><input type='text' onkeydown='ValidateInputKeydown(event, this, 1)'  class='form-control txtGiaTien' " + (tiGia == "USD" ? 'disabled' : '') + " onblur='TinhLaiDongTong(event,this,2,0)'  name='fDeNghiQTNamNay_VND'  /></td>";
    dongMoi += "<td  align='right'><input type='text' onkeydown='ValidateInputKeydown(event, this, 2)'  class='form-control txtGiaTien' " + (tiGia == "USD" ? '' : 'disabled') + " onblur='TinhLaiDongTong(event,this,2,2)' name='fLuyKeKinhPhiDuocCap_USD'  /></td>";
    dongMoi += "<td  align='right'><input type='text' onkeydown='ValidateInputKeydown(event, this, 1)'  class='form-control txtGiaTien' " + (tiGia == "USD" ? 'disabled' : '') + " onblur='TinhLaiDongTong(event,this,2,0)'  name='fLuyKeKinhPhiDuocCap_VND'  /></td>";
    dongMoi += "<td  align='center'><button class='btn-delete btn-icon'  type='button' onclick='XoaDong(this)'>" +
        "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
        "</button></td>";
    dongMoi += "</tr>";

    $("#" + TBL_KTCPCT + " tbody").append(dongMoi);
    $(".selectDuAn select").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
}

function TinhLaiDongTong(event, idDong, type, num) {
    ValidateInputFocusOut(event, idDong, type, num);
    var tiGia = $("#tiGiaQuyetToan").val();
    var tiGiaChiTiet = Number($("#tiGiaChiTiet").val());
    var getName = $(idDong).attr("name").slice(0, -3);
    var returnName = "";

    var sData = $(idDong).val();
    var fData = 0;
    var dongHienTai = $(idDong).closest("tr");
    if (sData != "" && sData != null)
        fData = parseInt(UnFormatNumber(sData))
    var returnData = tiGiaChiTiet * fData;

    if (tiGia == "VND") {
        returnName = getName + "USD";
        returnData = FormatNumber(returnData.toFixed(2))
        //$(idDong).val(FormatNumber(fData));
    } else {
        returnName = getName + "VND";
        returnData = FormatNumber(returnData)
        //$(idDong).val(FormatNumber(fData.toFixed(2)));
    }
    $(dongHienTai).find("input[name=" + returnName + "]").val(returnData);
}

function XoaDong(idDong) {
    var dongHienTai = $(idDong).closest("tr");
    $(dongHienTai).addClass("hidden");
    $(dongHienTai).removeAttr("data-isdelete");
    $(dongHienTai).attr("data-isdelete", 'true');
}
function Save() {
    var result = [];
    var resultDelete = [];

    $("#" + TBL_KTCPCT + " tbody[name='tableCreate'] tr[data-isdelete='false']").each(function () {
        var allValues = {};
        $(this).find("select").each(function (index) {
            const fieldName = $(this).attr("name");
            allValues[fieldName] = $(this).val();
        });
        $(this).find("input").each(function (index) {
            const fieldName = $(this).attr("name");
            allValues[fieldName] = $(this).val().split('.').join("");
        });
        result.push(allValues);
    })
    $("#" + TBL_KTCPCT + " tbody[name='tableCreate'] tr[data-isdelete='true']").each(function () {
        var allValuesDelete = {};
        $(this).find("select").each(function (index) {
            const fieldName = $(this).attr("name");
            allValuesDelete[fieldName] = $(this).val();
        });
        $(this).find("input").each(function (index) {
            const fieldName = $(this).attr("name");
            allValuesDelete[fieldName] = $(this).val().split('.').join("");
        });
        resultDelete.push(allValuesDelete);
    })
    var error = 0
    result.forEach((x) => {
        if (x.iID_DuAnID == GUID_EMPTY || x.iID_HopDongID == GUID_EMPTY) {
            error++;
        }
    })
    if (error > 0) {
        var Title = 'Thông tin chưa đủ';
        var Error = "Vui lòng chọn dự án/hợp đồng!"
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: [Error], Category: ERROR },
            success: function (res) {
                $("#divModalConfirm").html(res);
            }
        });
    }
    else {
        $.ajax({
            type: "POST",
            url: "/QLNH/KhoiTaoCapPhat/SaveDetail",
            data: { data: result, dataDelete: resultDelete },
            async: false,
            success: function (r) {
                if (r && r.bIsComplete) {
                    window.location.href = "/QLNH/KhoiTaoCapPhat";
                } else {
                    var Title = 'Lỗi lưu thông tin khởi tạo cấp phát';
                    var messErr = [];
                    messErr.push(r.sMessError);
                    $.ajax({
                        type: "POST",
                        url: "/Modal/OpenModal",
                        data: { Title: Title, Messages: messErr, Category: ERROR },
                        success: function (data) {
                            $("#divModalConfirm").html(data);
                        }
                    });
                }
            }
        });
    }

}

function Cancel() {
    window.location.href = "/QLNH/KhoiTaoCapPhat";
}
function LoadDataViewChitiet() {
    var data = GetListDataCapPhat();
    var tiGia = $("#tiGiaQuyetToan").val();

    if (data != null) {
        for (var i = 0; i < data.length; i++) {
            var dongMoi = "";
            console.log(data[i])
            dongMoi += "<tr style='cursor: pointer;' class='parent' name='" + (data[i].iID_DuAnID != undefined ? 'isDuAn' : 'isHopDong') + "' data-isdelete='false'>";
            dongMoi += "<td><div class='" + (data[i].iID_DuAnID != undefined ? 'selectDuAn' : 'selectHopDong') + "'>" + (data[i].iID_DuAnID != undefined ? CreateHtmlSelectDuAn(data[i].iID_DuAnID) : CreateHtmlSelectHopDong(data[i].iID_HopDongID)) + "</div></td>";
            dongMoi += "<input type='hidden' class='form-control'  name='iID_KhoiTaoCapPhatID' value='" + data[i].iID_KhoiTaoCapPhatID + "'/>";
            dongMoi += "<input type='hidden' class='form-control'  name='ID' value='" + data[i].ID + "'/>";

            dongMoi += "<td  align='right'><input type='text' onkeydown='ValidateInputKeydown(event, this, 2)' class='form-control txtGiaTien' " + (tiGia == "USD" ? '' : 'disabled') + "   value='" + FormatNumber(data[i].fQTKinhPhiDuyetCacNamTruoc_USD.toString().replace(",", "."),2) + "' onblur='TinhLaiDongTong(event,this,2,2)'  name='fQTKinhPhiDuyetCacNamTruoc_USD'   /></td>";
            dongMoi += "<td  align='right'><input type='text' onkeydown='ValidateInputKeydown(event, this, 1)' class='form-control txtGiaTien' " + (tiGia == "USD" ? 'disabled' : '') + "   value='" + FormatNumber(data[i].fQTKinhPhiDuyetCacNamTruoc_VND.toString().replace(",", ".")) + "'   onblur='TinhLaiDongTong(event,this,2,0)'   name='fQTKinhPhiDuyetCacNamTruoc_VND'  /></td>";
            dongMoi += "<td  align='right'><input type='text' onkeydown='ValidateInputKeydown(event, this, 2)' class='form-control txtGiaTien' " + (tiGia == "USD" ? '' : 'disabled') + "   value='" + FormatNumber(data[i].fDeNghiQTNamNay_USD.toString().replace(",", "."),2) + "'            onblur='TinhLaiDongTong(event,this,2,2)' name='fDeNghiQTNamNay_USD'  /></td>";
            dongMoi += "<td  align='right'><input type='text' onkeydown='ValidateInputKeydown(event, this, 1)' class='form-control txtGiaTien' " + (tiGia == "USD" ? 'disabled' : '') + "   value='" + FormatNumber(data[i].fDeNghiQTNamNay_VND.toString().replace(",", ".")) + "'              onblur='TinhLaiDongTong(event,this,2,0)'   name='fDeNghiQTNamNay_VND'  /></td>";
            dongMoi += "<td  align='right'><input type='text' onkeydown='ValidateInputKeydown(event, this, 2)' class='form-control txtGiaTien' " + (tiGia == "USD" ? '' : 'disabled') + "   value='" + FormatNumber(data[i].fLuyKeKinhPhiDuocCap_USD.toString().replace(",", "."),2) + "'       onblur='TinhLaiDongTong(event,this,2,2)' name='fLuyKeKinhPhiDuocCap_USD'  /></td>";
            dongMoi += "<td  align='right'><input type='text' onkeydown='ValidateInputKeydown(event, this, 1)' class='form-control txtGiaTien' " + (tiGia == "USD" ? 'disabled' : '') + "   value='" + FormatNumber(data[i].fLuyKeKinhPhiDuocCap_VND.toString().replace(",", ".")) + "'         onblur='TinhLaiDongTong(event,this,2,0)'   name='fLuyKeKinhPhiDuocCap_VND'  /></td>";
            dongMoi += "<td  align='center'><button class='btn-delete btn-icon'  type='button' onclick='XoaDong(this)'>" +
                "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
                "</button></td>";
            dongMoi += "</tr>";

            $("#" + TBL_KTCPCT + " tbody[name='tableCreate']").append(dongMoi);
        }
    }
}

function GetListDataCapPhat() {
    var dataDetail = [];
    var table = $("#" + TBL_KTCPCT + " tbody:hidden tr")
    table.each(function () {
        var allValues = {};

        $(this).find("td").each(function (index) {
            const fieldName = $(this).data("name");
            allValues[fieldName] = $(this).data("value")
        });
        dataDetail.push(allValues);
    })
    return dataDetail
}
function LoadDataHopDong() {
    $.ajax({
        url: "/QLNH/KhoiTaoCapPhat/GetHopDongAll",
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

function LoadDataDuAn() {
    $.ajax({
        url: "/QLNH/KhoiTaoCapPhat/GetDuAnAll",
        type: "POST",
        dataType: "json",
        cache: false,
        async: false,
        success: function (data) {
            if (data.status == false) {
                return;
            }

            if (data.data != null) {
                arr_DataDuAn = data.data;
            }
        }
    });
}

function CreateHtmlSelectHopDong(value) {
    var htmlOption = "<option value='" + GUID_EMPTY + "' selected>--Chọn hợp đồng--</option>";
    arr_DataHopDong.forEach(x => {
        if (value != undefined && value == x.id)
            htmlOption += "<option value='" + x.id + "' selected>" + $("<div/>").text(x.text).html() + "</option>";
        else
            htmlOption += "<option value='" + x.id + "'>" + $("<div/>").text(x.text).html() + "</option>";
    })
    return "<select class='form-control' onclick='checkTonTaiHopDong(this)' name='iID_HopDongID' >" + htmlOption + "</option>";
}

function CreateHtmlSelectDuAn(value) {
    var htmlOption = "<option value='" + GUID_EMPTY + "' selected>--Chọn dự án--</option>";
    arr_DataDuAn.forEach(x => {
        if (value != undefined && value == x.id)
            htmlOption += "<option value='" + x.id + "' selected>" + $("<div/>").text(x.text).html() + "</option>";
        else
            htmlOption += "<option value='" + x.id + "'>" + $("<div/>").text(x.text).html() + "</option>";
    })
    return "<select class='form-control' onclick='checkTonTaiDuAn(this)' name='iID_DuAnID' >" + htmlOption + "</option>";
}
function checkTonTaiHopDong(event) {
    var getData = $(event).val();
    var listDropDown = arr_DataHopDong;
    var htmlOption = "";
    var revArr_DataHopDong = [];
    $("#" + TBL_KTCPCT + " tbody[name='tableCreate'] tr[name='isHopDong']").each(function () {
        $(this).find(".selectHopDong select").each(function (index) {
            var data = $(this).val();
            if (data != GUID_EMPTY && data != getData) {
                revArr_DataHopDong.push(data)
            }
        });
    });
    htmlOption += "<option value = '" + GUID_EMPTY + "' selected >--Chọn hợp đồng--</option >";
    listDropDown.filter(x => {
        return !revArr_DataHopDong.includes(x.id);
    }).forEach(x => {
        if (getData != undefined && getData == x.id)
            htmlOption += "<option value='" + x.id + "' selected>" + $("<div/>").text(x.text).html() + "</option>";
        else
            htmlOption += "<option value='" + x.id + "'>" + $("<div/>").text(x.text).html() + "</option>";
    })
    $(event).html("").append(htmlOption);
}

function checkTonTaiDuAn(event) {
    var getData = $(event).val();
    var listDropDown = arr_DataDuAn;
    var htmlOption = "";
    var revArr_DataDuAn = [];
    $("#" + TBL_KTCPCT + " tbody[name='tableCreate'] tr[name='isDuAn']").each(function () {
        $(this).find(".selectDuAn select").each(function (index) {
            var data = $(this).val();
            if (data != GUID_EMPTY && data != getData) {
                revArr_DataDuAn.push(data)
            }
        });
    });
    htmlOption += "<option value = '" + GUID_EMPTY + "' selected >--Chọn dự án--</option >";
    listDropDown.filter(x => {
        return !revArr_DataDuAn.includes(x.id);
    }).forEach(x => {
        if (getData != undefined && getData == x.id)
            htmlOption += "<option value='" + x.id + "' selected>" + $("<div/>").text(x.text).html() + "</option>";
        else
            htmlOption += "<option value='" + x.id + "'>" + $("<div/>").text(x.text).html() + "</option>";
    })
    $(event).html("").append(htmlOption);
}