var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var QT_Tai_San = "qttaisan";
var arr_DataDonVi = [];
var ListTaiSan = [];
var dataLoaiTaiSan;
var arr_DataDuAn = [];
var arr_DataHopDong = [];
var arr_DataTinhTrang = [];
var arr_DataLoaiTaiSan = [];
var arr_DataTinhTrangSuDung = [];

function GetDropdownData() {
    $.ajax({
        type: "POST",
        async: false,
        url: "/QLNH/TaiSan/GetDropdownData",
        success: function (result) {
            arr_DataDonVi = result.donViList;
            arr_DataDuAn = result.duAnList;
            arr_DataHopDong = result.hopDongList;
            arr_DataTinhTrang = result.tinhTrangList;
            arr_DataLoaiTaiSan = result.loaiTaiSanList;
            arr_DataTinhTrangSuDung = result.tinhTrangSuDungList;
        }
    });
}

function BindingValidateAndSelect2() {
    $("#tbListTaiSan tbody tr .listdonvi").select2({ dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#tbListTaiSan tbody tr .listduan").select2({ dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#tbListTaiSan tbody tr .listhopdong").select2({ dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#tbListTaiSan tbody tr .slbTrangThai").select2({ dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#tbListTaiSan tbody tr .slbLoaiTaiSan").select2({ dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#tbListTaiSan tbody tr .slbTinhTrangSuDung").select2({ dropdownAutoWidth: true, matcher: FilterInComboBox });

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
}

function CreateHtmlSelectDonVi(value) {
    var htmlOption = "";
   htmlOption += "<option value='' selected>--Chọn đơn vị--</option>";
    arr_DataDonVi.forEach(x => {
        if (value != undefined && value == x.iID_Ma)
            htmlOption += "<option data-madonvi='" + $("<div/>").text(x.iID_MaDonVi).html() + "' value='" + x.iID_Ma + "'selected >" + $("<div/>").text(x.sTen).html() + "</option>";
        else
            htmlOption += "<option data-madonvi='" + $("<div/>").text(x.iID_MaDonVi).html() + "' value='" + x.iID_Ma + "' >" + $("<div/>").text(x.sTen).html() + "</option>";
    })
    return "<select class='form-control listdonvi' name='iID_DonViID'>" + htmlOption + "</option>";
}

function CreateHtmlSelectDuAn(value) {
    var htmlOption = "";
   htmlOption += "<option value=''selected>--Chọn dự án--</option>";
    arr_DataDuAn.forEach(x => {
        if (value != undefined && value == x.ID)
            htmlOption += "<option value='" + x.ID + "' selected>" + $("<div/>").text(x.sTenDuAn).html() + "</option>";
        else
            htmlOption += "<option value='" + x.ID + "' >" + $("<div/>").text(x.sTenDuAn).html() + "</option>";
    })

    return "<select class='form-control listduan' name='iID_DuAnID'>" + htmlOption + "</option>";
}

function CreateHtmlSelectHopDong(value) {
    var htmlOption = "";
   htmlOption += "<option value=''selected >--Chọn hợp đồng--</option>";
    arr_DataHopDong.forEach(x => {
        if (value != undefined && value == x.ID)
            htmlOption += "<option value='" + x.ID + "' selected>" + $("<div/>").text(x.sTenHopDong).html() + "</option>";
        else
            htmlOption += "<option value='" + x.ID + "' >" + $("<div/>").text(x.sTenHopDong).html() + "</option>";
    })
    return "<select class='form-control listhopdong' name='iID_HopDongID'>" + htmlOption + "</option>";
}

function CreateHtmlSelectTinhTrang(value) {
    var htmlOption = "";
    arr_DataTinhTrang.forEach(x => {
        if (value != undefined && value == x.valueId)
            htmlOption += "<option value='" + x.valueId + "' selected>" + $("<div/>").text(x.labelName).html() + "</option>";
        else
            htmlOption += "<option value='" + x.valueId + "' >" + $("<div/>").text(x.labelName).html() + "</option>";
    })
    return "<select class='form-control slbTrangThai' name='iTrangThai'>" + htmlOption + "</option>";
}

function CreateHtmlSelectLoaiTaiSan(value) {
    var htmlOption = "";
    arr_DataLoaiTaiSan.forEach(x => {
        if (value != undefined && value == x.valueId)
            htmlOption += "<option value='" + x.valueId + "' selected>" + $("<div/>").text(x.labelName).html() + "</option>";
        else
            htmlOption += "<option value='" + x.valueId + "' >" + $("<div/>").text(x.labelName).html() + "</option>";
    })
    return "<select class='form-control slbLoaiTaiSan' name = 'iLoaiTaiSan'>" + htmlOption + "</option>";
}

function CreateHtmlSelectTinhTrangSuDung(value) {
    var htmlOption = "";
    arr_DataTinhTrangSuDung.forEach(x => {
        if (value != undefined && value == x.valueId)
            htmlOption += "<option value='" + x.valueId + "' selected>" + $("<div/>").text(x.labelName).html() + "</option>";
        else
            htmlOption += "<option value='" + x.valueId + "' >" + $("<div/>").text(x.labelName).html() + "</option>";
    })
    return "<select class='form-control slbTinhTrangSuDung' name='iTinhTrangSuDung'>" + htmlOption + "</option>";
}

function ResetChangePage(iCurrentPage = 1) {
    var sTenChungTu = "";
    var sSoChungTu = "";
    var dNgayChungTu = "";

    GetListData(sTenChungTu, sSoChungTu, dNgayChungTu, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var sTenChungTu = $("<div/>").text($.trim($("#txtsTenChungTuFillter").val())).html();
    var sSoChungTu = $("<div/>").text($.trim($("#txtsSochungTuFillter").val())).html();
    var dNgayChungTu = $("#dNgayChungTuFillter").val();

    GetListData(sTenChungTu, sSoChungTu, dNgayChungTu, iCurrentPage);
}

function GetListData(sTenChungTu, sSoChungTu, dNgayChungTu, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/TaiSan/DanhMucTaiSanSearch",
        data: { _paging: _paging, sTenChungTu: sTenChungTu, sSoChungTu: sSoChungTu, dNgayChungTu: dNgayChungTu},
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtsTenChungTuFillter").val($("<div/>").html(sTenChungTu).text());
            $("#txtsSochungTuFillter").val($("<div/>").html(sSoChungTu).text());
            $("#dNgayChungTuFillter").val(dNgayChungTu);
        }
    });
}

function OpenDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/TaiSan/OpenDetail",
        data: { id: id },
        success: function (data) {
            $("#lstDataView").html(data);
        }
    });
}

function OpenUpdate(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/TaiSan/OpenUpdate",
        data: { id: id },
        success: function (data) {
            $("#lstDataView").html(data);
            GetDropdownData();
            LoadDataViewChitiet();

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

            $("#txtdNgayChungTu").keydown(function (event) {
                ValidateInputKeydown(event, this, 3);
            }).blur(function (event) {
                setTimeout(() => {
                    if (!isShowing) ValidateInputFocusOut(event, this, 3);
                }, 0);
            });
        }
    });
}

function OpenCreate() {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/TaiSan/OpenCreate",
        success: (data) => {
            $("#lstDataView").html(data);
            GetDropdownData();

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

            $("#txtdNgayChungTu").keydown(function (event) {
                ValidateInputKeydown(event, this, 3);
            }).blur(function (event) {
                setTimeout(() => {
                    if (!isShowing) ValidateInputFocusOut(event, this, 3);
                }, 0);
            });
        }
    });
}

function Back() {
    window.location.href = "/QLNH/TaiSan";
}

function ResetChange() {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/TaiSan/ReLoadListTaiSan",
        success: (e) => {
            var x = JSON.parse(e);
            $("#tbListLoaiTaiSan tbody tr").remove();
            x.forEach((y, index) => {
                var dongMoi = "";
                dongMoi += "<tr style='cursor: pointer;' ondblclick='Choose(this);'>";
                dongMoi += "<td class='text-center r_STT'><input type='hidden' name='sSTT' value='" + (index + 1) + "'/></td>";
                dongMoi += "<td class='text-center hidden'><input type='hidden' name='ID' value='" + y.ID + "' /></td>";
                dongMoi += "<td class='text-center hidden'><input type='hidden' name='dNgayTao' value='" + y.dNgayTao + "' /></td>";
                dongMoi += "<td class='text-center'><input type='text' name='sMaLoaiTaiSan' class='form-control' value='" + EscapeHtml(y.sMaLoaiTaiSan) + "' /></td>";
                dongMoi += "<td class='text-center'><input type='text' name='sTenLoaiTaiSan' class='form-control' value='" + EscapeHtml(y.sTenLoaiTaiSan) + "' /></td>";
                dongMoi += "<td class='text-center'><input type='text' name='sMoTa' class='form-control' value='" + EscapeHtml(y.sMoTa) + "' /></td>";
                dongMoi += "<td class='text-center'><button class='btn btn-delete btn-icon' type='button' onclick='XoaDong(this)'><i class='fa fa-trash-o fa-lg' aria-hidden='true'></i>Xóa</button></td>";
                dongMoi += "</tr>";
                $("#tbListLoaiTaiSan tbody").append(dongMoi);
            });
            UpdateSequenceColumn("tbListLoaiTaiSan");
        }
    });
}

function DeleteItem(id) {
    $.ajax({
        type: "POST",
        url: "/QLNH/TaiSan/TaiSanDelete",
        data: { id: id },
        success: function (r) {
            if (r.bIsComplete) {
                ChangePage();
            } else {
                var Title = 'Lỗi xóa tài sản';
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

function Delete(id) {
    $.ajax({
        type: "POST",
        url: "/QLNH/TaiSan/ChungTuTaiSanDelete",
        data: { id: id },
        success: function (r) {
            if (r.bIsComplete) {
                ChangePage();
            } else {
                var Title = 'Lỗi';
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

function Xoa(id) {
    var Title = 'Xác nhận xóa chứng từ tài sản';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn xóa?');
    var FunctionName = "Delete('" + id + "')";
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
        success: function (data) {
            $("#divModalConfirm").empty().html(data);
        }
    });
}

function Save() {
    let result = [];

    $("#tbListTaiSan tbody tr").each(function () {
        var objectTaiSan = {};

        $(this).find("input").each(function (index) {
            const fieldName = $(this).attr("name");
            objectTaiSan[fieldName] = $("<div/>").text($.trim($(this).val())).html();
        });
        $(this).find("select").each(function (index) {
            const fieldName = $(this).attr("name");
            if (fieldName == "iID_DonViID") {
                objectTaiSan["iID_MaDonVi"] = $("<div/>").text($.trim($(this).find("option:selected").data("madonvi"))).html();
            }
            objectTaiSan[fieldName] = $(this).val();
        });
        result.push(objectTaiSan);
    })

    var data = {};
    data.ID = $("#idChungTu").val();
    data.dNgayTao = $("#dNgayTao").val();
    data.sTenChungTu = $("<div/>").text($.trim($("#txtsTenChungTu").val())).html();
    data.sSoChungTu = $("<div/>").text($.trim($("#txtsSoChungTu").val())).html();
    data.dNgayChungTu = $("#txtdNgayChungTu").val();
    if (!ValidateData(data)) {
        return false;
    }
    $.ajax({
        type: "POST",
        url: "/QLNH/TaiSan/TaiSanSave",
        data: { datactts: data, datats: result },
        success: function (r) {
            if (r && r.bIsComplete) {
                window.location.href = "/QLNH/TaiSan";
            } else {
                var title = 'lỗi lưu dữ liệu';
                var messerr = [];
                messerr.push(r.smesserror);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { title: title, messages: messerr, category: error },
                    success: function (data) {
                        $("#divmodalconfirm").html(data);
                    }
                });
            }
        }
    });
}

function ValidateData(data) {
    var Title = 'Lỗi thêm mới/chỉnh sửa tài sản';
    var Messages = [];

    if (data.sSoChungTu == null || data.sSoChungTu == "") {
        Messages.push("Số chứng từ chưa nhập !");
    }
    if ($.trim($("#txtsSoChungTu").val()).length > 50) {
        Messages.push("Số chứng từ nhập quá 50 kí tự !");
    }
    if ($.trim($("#txtdNgayChungTu").val()) != "" && !dateIsValid($.trim($("#txtdNgayChungTu").val()))) {
        Messages.push("Ngày chứng từ không hợp lệ !");
    }

    if (Messages.length > 0) {
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

function AddRow() {
    var dongMoi = "";
    dongMoi += "<tr>";
    dongMoi += "<td class='text-center r_STT'></td>";
    dongMoi += "<td class='text-center' style='cursor: pointer;' onclick='Openlist(this);' ondblclick='Openlist(this);'><input type='text' name='sMaTaiSan' class='form-control' style='cursor: pointer;' readonly /></td>";
    dongMoi += "<td class='text-center' style='cursor: pointer;' onclick='Openlist(this);' ondblclick='Openlist(this);'><input type='text' name='sTenTaiSan' class='form-control' style='cursor: pointer;' readonly /></td>";
    dongMoi += "<td class='text-center' style='cursor: pointer;' onclick='Openlist(this);' ondblclick='Openlist(this);'><input type='text' name='sMoTaTaiSan' class='form-control' style='cursor: pointer;' readonly /></td>";
    dongMoi += "<td class='text-center'>" + CreateHtmlSelectLoaiTaiSan() + "</td>";
    dongMoi += "<td class='text-center'><div class='input-group date'><input type='text' name='dNgayBatDauSuDung' class='form-control txtDate' value=''autocomplete='off' placeholder='dd/MM/yyyy' /><span class='btn-default input-group-addon input-calendar'><i class='fa fa-calendar' style='margin:0;' aria-hidden='true'></i></span></div></td>";
    dongMoi += "<td class='text-center'>" + CreateHtmlSelectTinhTrang() + "</td>";
    dongMoi += "<td class='text-center'><input type='text' name='fSoLuong' class='form-control' onkeydown='ValidateInputKeydown(event, this, 1);' /></td>";
    dongMoi += "<td class='text-center'><input type='text' name='sDonViTinh' class='form-control' /></td>";
    dongMoi += "<td class='text-center'><input type='text' name='fNguyenGia' class='form-control' onkeydown='ValidateInputKeydown(event, this, 2);' /></td>";
    dongMoi += "<td class='text-center'>" + CreateHtmlSelectTinhTrangSuDung() + "</td>";
    dongMoi += "<td class='text-center'>" + CreateHtmlSelectDonVi() + "</td>";
    dongMoi += "<td class='text-center'>" + CreateHtmlSelectDuAn() + "</td>";
    dongMoi += "<td class='text-center'>" + CreateHtmlSelectHopDong() + "</td>";
    dongMoi += "<td class='text-center'><button class='btn btn-delete btn-icon' type='button' onclick='XoaDong(this);'><i class='fa fa-trash-o fa-lg' aria-hidden='true'></i>Xóa</button></td>";
    dongMoi += "</tr>";
    $("#tbListTaiSan tbody").append(dongMoi);
    UpdateSequenceColumn("tbListTaiSan");

    BindingValidateAndSelect2();
}

function XoaDong(nutXoa) {
    var dongXoa = nutXoa.parentElement.parentElement;
    dongXoa.parentNode.removeChild(dongXoa);
}

function UpdateSequenceColumn(elementid) {
    $("#" + elementid +" tbody tr").each(function (index, tr) {
        $(tr).find('.r_STT').text(index + 1);
    });
}

function LoadDataViewChitiet() {
    var data = GetListDataChitietJson();
    if (data != null) {
        for (var i = 0; i < data.length; i++) {
            var dongMoi = "";
            dongMoi += "<tr class='parent'>";
            dongMoi += "<td class='text-center r_STT'>" + (i + 1) + "</td>";
            dongMoi += "<td class='text-center' hidden><input type='hidden' name='ID' value='" + data[i].ID + "' /></td>";
            dongMoi += "<td class='text-center' style='cursor: pointer;' onclick='Openlist(this);' ondblclick='Openlist(this);'><input type='text' name='sMaTaiSan' class='form-control' style='cursor: pointer;' value='" + EscapeHtml(data[i].sMaTaiSan) + "' readonly /></td>";
            dongMoi += "<td class='text-center' style='cursor: pointer;' onclick='Openlist(this);' ondblclick='Openlist(this);'><input type='text' name='sTenTaiSan' class='form-control' style='cursor: pointer;' value='" + EscapeHtml(data[i].sTenTaiSan) + "' readonly /></td>";
            dongMoi += "<td class='text-center' style='cursor: pointer;' onclick='Openlist(this);' ondblclick='Openlist(this);'><input type='text' name='sMoTaTaiSan' class='form-control' style='cursor: pointer;' value='" + EscapeHtml(data[i].sMoTaTaiSan) + "' readonly /></td>";
            dongMoi += "<td class='text-center'>" + CreateHtmlSelectLoaiTaiSan(data[i].iLoaiTaiSan) + "</td>";
            dongMoi += "<td class='text-center'><div class='input-group date'><input type='text' name='dNgayBatDauSuDung' class='form-control txtDate' value='" + data[i].dNgayBatDauSuDungStr + "' autocomplete='off' placeholder='dd/MM/yyyy' /><span class='btn-default input-group-addon'><i class='fa fa-calendar' style='margin:0;' aria-hidden='true'></i></span></div></td>";
            dongMoi += "<td class='text-center'>" + CreateHtmlSelectTinhTrang(data[i].iTrangThai) + "</td>";
            dongMoi += "<td class='text-center'><input type='text' name='fSoLuong' class='form-control' onkeydown='ValidateInputKeydown(event, this, 1);' value='" + EscapeHtml(data[i].fSoLuong) + "' /></td>";
            dongMoi += "<td class='text-center'><input type='text' name='sDonViTinh' class='form-control' value='" + EscapeHtml(data[i].sDonViTinh) + "' /></td>";
            dongMoi += "<td class='text-center'><input type='text' name='fNguyenGia' class='form-control' onkeydown='ValidateInputKeydown(event, this, 2);' value='" + EscapeHtml(data[i].fNguyenGia) + "'/></td>";
            dongMoi += "<td class='text-center'>" + CreateHtmlSelectTinhTrangSuDung(data[i].iTinhTrangSuDung) + "</td>";
            dongMoi += "<td class='text-center'>" + CreateHtmlSelectDonVi(data[i].iID_DonViID) + "</td>";
            dongMoi += "<td class='text-center'>" + CreateHtmlSelectDuAn(data[i].iID_DuAnID) + "</td>";
            dongMoi += "<td class='text-center'>" + CreateHtmlSelectHopDong(data[i].iID_HopDongID) + "</td>";
            dongMoi += "<td class='text-center'><button class='btn btn-delete btn-icon' type='button' onclick='XoaDong(this);'><i class='fa fa-trash-o fa-lg' aria-hidden='true'></i> Xóa</button></td>";
            dongMoi += "</tr>";

            $("#tbListTaiSan tbody").append(dongMoi);
            UpdateSequenceColumn("tbListTaiSan");

            BindingValidateAndSelect2();
        }
    }
}

function GetListDataChitietJson() {
    var items = $("#arrTaiSan").val();
    if (items == undefined || items == null || items == "") {
        return [];
    }

    items = JSON.parse(items);

    if (items != undefined && items != null && items.length > 0) {
        for (var i = 0; i < items.length; i++) {
            items[i].ID = (i + 1).toString();
        }
    }

    return items;
}

function Openlist(indextr) {
    dataLoaiTaiSan = indextr;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/TaiSan/OpenListTaiSan",
        success: function (data) {
            $("#modaListlTaiSan").modal("show");
            $("#contentModalListTaiSan").html(data);
            $("#modalListTaiSanLabel").html('Danh sách tài sản');
        }
    }); 
}

function ThemDong() {
    var dongMoi = "";
    dongMoi += "<tr style='cursor: pointer' ondblclick='CheckBeforeSave()'>";
    dongMoi += "<td class='text-center r_STT'></td>";
    dongMoi += "<td class='text-center hidden'><input type='hidden' name='ID' /></td>";
    dongMoi += "<td class='text-center'><input type='text' name='sMaLoaiTaiSan' class='form-control' /></td>";
    dongMoi += "<td class='text-center'><input type='text' name='sTenLoaiTaiSan' class='form-control' /></td>";
    dongMoi += "<td class='text-center'><input type='text' name='sMoTa' class='form-control' /></td>";
    dongMoi += "<td class='text-center'><button class='btn btn-delete btn-icon' type='button' onclick='XoaDong(this);'><i class='fa fa-trash-o fa-lg' aria-hidden='true'></i>Xóa</button></td>";
    dongMoi += "</tr>";
    $("#tbListLoaiTaiSan tbody").append(dongMoi);
    UpdateSequenceColumn("tbListLoaiTaiSan");
}

function Cancel() {
    window.location.href = "/QLNH/TaiSan";
}

function SaveLoaiTaiSan() {
    let result = [];

    $("#tbListLoaiTaiSan tbody tr").each(function () {
        var allValues = {};
        $(this).find("input").each(function (index) {
            const fieldName = $(this).attr("name");
            allValues[fieldName] = $("<div/>").text($(this).val()).html();
        });
        result.push(allValues);
    })

    if (!ValidateDataLoaiTaiSan(result)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLNH/TaiSan/ListTaiSanSave",
        data: { data: result, },
        success: function (r) {
            if (r && r.bIsComplete) {
                ResetChange();
                alert("Lưu dữ liệu thành công");
            } else {
                alert(r.sMessError);
            }
        }
    });
}

function ValidateDataLoaiTaiSan(data) {
    var Title = 'Lỗi thêm mới/chỉnh sửa loại tài sản';
    var Messages = [];
    for (var i = 0; i < data.length; i++) {
        if (data[i].sMaLoaiTaiSan == null || data[i].sMaLoaiTaiSan == "") {
            Messages.push("Mã loại tài sản chưa nhập !");
        }
        if (data[i].sMaLoaiTaiSan.length > 50) {
            Messages.push("Mã loại tài sản nhập quá 50 kí tự !");
        }
        if (data[i].sTenLoaiTaiSan == null || data[i].sTenLoaiTaiSan == "") {
            Messages.push("Tên loại tài sản chưa nhập !");
        }
        if (data[i].sTenLoaiTaiSan.length > 50) {
            Messages.push("Tên loại tài sản nhập quá 50 kí tự !");
        }

        if (Messages.length > 0) break;
    }

    if (Messages.length > 0) {
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

function Choose(ele) {
    var tr = $(dataLoaiTaiSan).parent("tr");
    tr.find("input[name='sMaTaiSan']").val($("<div/>").html($("<div/>").text($.trim($(ele).find("input[name='sMaLoaiTaiSan']").val())).html()).text());
    tr.find("input[name='sTenTaiSan']").val($("<div/>").html($("<div/>").text($.trim($(ele).find("input[name='sTenLoaiTaiSan']").val())).html()).text());
    tr.find("input[name='sMoTaTaiSan']").val($("<div/>").html($("<div/>").text($.trim($(ele).find("input[name='sMoTa']").val())).html()).text());
    $('#modaListlTaiSan').modal('hide');
}

function CheckBeforeSave() {
    alert("Vui lòng lưu dữ liệu trước khi chọn");
}