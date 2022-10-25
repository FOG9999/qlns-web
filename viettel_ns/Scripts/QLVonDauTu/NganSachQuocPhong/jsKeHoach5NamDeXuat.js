var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;

var data = [];

$(document).ready(function () {
    //$("#iGiaiDoanTu").change(function () {
    //    if (this.value != "") {
    //        $("#iGiaiDoanDen").val(parseInt(this.value) + 4);
    //    } else {
    //        $("#iGiaiDoanDen").val("");
    //    }
    //});

    $("#iID_DonViQuanLy").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#iLoaiLst").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });

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

    $("#dNgayQuyetDinhTo").keydown(function (event) {
        ValidateInputKeydown(event, this, 3);
    }).blur(function (event) {
        setTimeout(() => {
            if (!isShowing) ValidateInputFocusOut(event, this, 3);
        }, 0);
    });

    $("#dNgayQuyetDinhFrom").keydown(function (event) {
        ValidateInputKeydown(event, this, 3);
    }).blur(function (event) {
        setTimeout(() => {
            if (!isShowing) ValidateInputFocusOut(event, this, 3);
        }, 0);
    });

    $("#iGiaiDoanTuSearch").keydown(function (event) {
        ValidateInputKeydown(event, this, 1);
    }).blur(function (event) {
        ValidateInputFocusOut(event, this, 6);
    });

    $("#iGiaiDoanDenSearch").keydown(function (event) {
        ValidateInputKeydown(event, this, 1);
    }).blur(function (event) {
        ValidateInputFocusOut(event, this, 6);
    });

    $("#txt_SoQuyetdinh").keyup(function (event) {
        ValidateMaxLength(this, 100);
    });
});

function Save() {
    var data = {};
    data.iID_KeHoach5Nam_DeXuatID = $("#txt_ID_KH5NamDeXuat").val();
    data.iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
    data.sSoQuyetDinh = $.trim($("#txtsSoQuyetDinh").val());
    data.dNgayQuyetDinh = $("#dNgayQuyetDinh").val();
    data.iGiaiDoanTu = $("#iGiaiDoanTu").val();
    data.iGiaiDoanDen = $("#iGiaiDoanDen").val();
    data.sMoTaChiTiet = $.trim($("#sMoTaChiTiet").val());
    data.iLoai = $("#iLoai").val();

    var isModified = $('#txtDxModified').val();
    var isAggregate = $('#txtDxAggregate').val();
    //var bIsDetail = $('#txtDetail').val()

    if (!ValidateData(data)) {
        return false;
    }
    var lstDuAnChecked = JSON.parse(sessionStorage.getItem('DuAnChecked'));

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachTrungHanDeXuat/KeHoach5NamDeXuatSave",
        data: { data: data, isModified: isModified, isAggregate: isAggregate, lstDuAnChecked: lstDuAnChecked },
        success: function (r) {
            if (r.bIsComplete) {
                //thêm cảnh báo khi tạo trùng
                if (r.bIsTrung) {
                    var Title = 'Cảnh báo';
                    var messErr = [];
                    var FunctionName = "SaveTrung()";
                    messErr.push(r.sMessWarning);
                    $.ajax({
                        type: "POST",
                        url: "/Modal/OpenModal",
                        data: { Title: Title, Messages: messErr, Category: CONFIRM, FunctionName: FunctionName },
                        success: function (data) {
                            $("#divModalConfirm").html(data);
                        }
                    });
                } else {
                    if (isAggregate) {
                        window.location.href = "/QLVonDauTu/KeHoachTrungHanDeXuat/Detail/?id=" + r.iID_KeHoach5Nam_DeXuatID + "&isTongHop=" + isAggregate;
                    } else {
                        window.location.href = "/QLVonDauTu/KeHoachTrungHanDeXuat/Detail/?id=" + r.iID_KeHoach5Nam_DeXuatID;
                    }
                }

            } else {
                var Title = 'Lỗi lưu kế hoạch trung hạn đề xuất';
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

//hàm save trùng đơn vị, trùng năm
function SaveTrung() {
    var data = {};
    data.iID_KeHoach5Nam_DeXuatID = $("#txt_ID_KH5NamDeXuat").val();
    data.iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
    data.sSoQuyetDinh = $("#txtsSoQuyetDinh").val();
    data.dNgayQuyetDinh = $("#dNgayQuyetDinh").val();
    data.iGiaiDoanTu = $("#iGiaiDoanTu").val();
    data.iGiaiDoanDen = $("#iGiaiDoanDen").val();
    data.sMoTaChiTiet = $("#sMoTaChiTiet").val();
    data.iLoai = $("#iLoai").val();

    var isModified = $('#txtDxModified').val();
    var isAggregate = $('#txtDxAggregate').val();
    //var bIsDetail = $('#txtDetail').val()

    if (!ValidateData(data)) {
        return false;
    }
    var lstDuAnChecked = JSON.parse(sessionStorage.getItem('DuAnChecked'));
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachTrungHanDeXuat/KeHoach5NamDeXuatSaveTrung",
        data: { data: data, isModified: isModified, isAggregate: isAggregate, lstDuAnChecked: lstDuAnChecked },
        success: function (r) {
            if (r.bIsComplete) {
                if (isAggregate) {
                    window.location.href = "/QLVonDauTu/KeHoachTrungHanDeXuat/Detail/?id=" + r.iID_KeHoach5Nam_DeXuatID + "&isTongHop=" + isAggregate;
                } else {
                    window.location.href = "/QLVonDauTu/KeHoachTrungHanDeXuat/Detail/?id=" + r.iID_KeHoach5Nam_DeXuatID;
                }

            } else {
                var Title = 'Lỗi lưu kế hoạch trung hạn đề xuất';
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

function ValidateData(data) {
    var Title = 'Lỗi thêm mới kế hoạch trung hạn đề xuất';
    var Messages = [];

    if (data.iID_DonViQuanLyID == null || data.iID_DonViQuanLyID == "") {
        Messages.push("Đơn vị quản lý chưa chọn.");
    }

    if (data.sSoQuyetDinh == null || data.sSoQuyetDinh == "") {
        Messages.push("Số kế hoạch chưa nhập.");
    }

    if (data.dNgayQuyetDinh == null || data.dNgayQuyetDinh == "") {
        Messages.push("Ngày lập chưa nhập.");
    }

    if (data.iGiaiDoanTu == null || data.iGiaiDoanTu == "") {
        Messages.push("Giai đoạn từ chưa nhập.");
    }

    if (data.iGiaiDoanDen == null || data.iGiaiDoanDen == "") {
        Messages.push("Giai đoạn đến chưa nhập.");
    }

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


function SetValueFormExport(sSoChungTu, sNoiDung, sMaLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo) {
    $("#txt_sMaLoaiDuToanExcel").val(sMaLoaiDuToan);
    $("#txt_sSoChungTuExcel").val(sSoChungTu);
    $("#txt_sSoQuyetDinhExcel").val(sSoQuyetDinh);
    $("#txt_dNgayQuyetDinhTuExcel").val(dNgayQuyetDinhFrom);
    $("#txt_dNgayQuyetDinhDenExcel").val(dNgayQuyetDinhTo);
    $("#txt_sNoiDungExcel").val(sNoiDung);
}

function ResetChangePage(iCurrentPage = 1) {
    var sSoQuyetDinh = "";
    var dNgayQuyetDinhFrom = "";
    var dNgayQuyetDinhTo = "";
    var iID_DonViQuanLyID = GUID_EMPTY;
    var sMoTaChiTiet = "";
    var iGiaiDoanTu = ""
    var iGiaiDoanDen = "";
    var iLoai = 0;
    var tabIndex = $('input[name=groupVoucher]:checked').val();

    //SetValueFormExport(sSoChungTu, sNoiDung, sMaLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo);

    GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iID_DonViQuanLyID, sMoTaChiTiet, iGiaiDoanTu, iGiaiDoanDen, iLoai, iCurrentPage, tabIndex);
}

function ChangePage(iCurrentPage = 1) {
    var tabIndex = $('input[name=groupVoucher]:checked').val();
    var sSoQuyetDinh = $.trim($("#txt_SoQuyetdinh").val());
    var dNgayQuyetDinhFrom = $("#dNgayQuyetDinhFrom").val();
    var dNgayQuyetDinhTo = $("#dNgayQuyetDinhTo").val();
    var iID_DonViQuanLyID = $("#iID_DonViQuanLy").val();
    var sMoTaChiTiet = $.trim($("#txt_MoTaChiTiet").val());
    var iGiaiDoanTu = $("#iGiaiDoanTuSearch").val();
    var iGiaiDoanDen = $("#iGiaiDoanDenSearch").val();
    var iLoai = $("#iLoaiLst").val();

    //SetValueFormExport(sSoChungTu, sNoiDung, sMaLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo);

    GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iID_DonViQuanLyID, sMoTaChiTiet, iGiaiDoanTu, iGiaiDoanDen, iLoai, iCurrentPage, tabIndex);
}

function GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iID_DonViQuanLyID, sMoTaChiTiet, iGiaiDoanTu, iGiaiDoanDen, iLoai, iCurrentPage, tabIndex) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/KeHoachTrungHanDeXuat/KeHoach5NamDeXuatListView",
        data: { _paging: _paging, sSoQuyetDinh: sSoQuyetDinh, dNgayQuyetDinhFrom: dNgayQuyetDinhFrom, dNgayQuyetDinhTo: dNgayQuyetDinhTo, iID_DonViQuanLyID: iID_DonViQuanLyID, sMoTaChiTiet: sMoTaChiTiet, iGiaiDoanTu: iGiaiDoanTu, iGiaiDoanDen: iGiaiDoanDen, iLoai: iLoai, tabIndex: tabIndex },
        success: function (data) {
            $("#lstDataView").html(data);

            $("#txt_SoQuyetdinh").val(sSoQuyetDinh);
            $("#dNgayQuyetDinhFrom").val(dNgayQuyetDinhFrom);
            $("#dNgayQuyetDinhTo").val(dNgayQuyetDinhTo);
            $("#iID_DonViQuanLy").val(iID_DonViQuanLyID);
            $("#txt_MoTaChiTiet").val(sMoTaChiTiet);
            $("#iGiaiDoanTuSearch").val(iGiaiDoanTu);
            $("#iGiaiDoanDenSearch").val(iGiaiDoanDen);
            $("#iLoaiLst").val(iLoai);
            //SetValueFormExport(sSoChungTu, sNoiDung, sMaLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo);
        }
    });
}

ChangeVoucher = () => {
    ChangePage(1);
}

function DeleteItem(id, sSoQuyetDinh = '') {
    var Title = 'Xác nhận xóa kế hoạch trung hạn đề xuất';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn xóa chứng từ ' + sSoQuyetDinh + '?');
    var FunctionName = "Delete('" + id + "')";
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
        success: function (data) {
            $("#divModalConfirm").html(data);
        }
    });
}

function Delete(id) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachTrungHanDeXuat/KeHoach5NamDeXuatDelete",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                ChangePage(1);
            }
        }
    });
}

function LockItem(id, sSoQuyetDinh, iKhoa) {
    var Title = 'Xác nhận ' + (iKhoa ? 'mở' : 'khóa') + ' kế hoạch trung hạn đề xuất';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn ' + (iKhoa ? 'mở' : 'khóa') + ' chứng từ ' + sSoQuyetDinh + '?');
    var FunctionName = "Lock('" + id + "')";
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
        success: function (data) {
            $("#divModalConfirm").html(data);
        }
    });
}

function Lock(id) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachTrungHanDeXuat/KeHoach5NamDeXuatLock",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                ChangePage(1);
            }
        }
    });
}

SetState = (key) => {
    var idItem = "." + key + ":checked";
    var elementValue = $(idItem).val();

    var itemValue = {
        isChecked: false,
        iID_KeHoach5Nam_DeXuatID: null
    };
    itemValue.isChecked = (elementValue == "on") ? true : false;
    itemValue.iID_KeHoach5Nam_DeXuatID = key;
    data.push(itemValue);
    sessionStorage.setItem('dataChecked', JSON.stringify(data));
}

ChooseDuAn = (key) => {
    var idItem = "." + key + ":checked";
    var elementValue = $(idItem).val();

    var itemValue = {
        IsChecked: false,
        IDDuAnID: null
    };

    itemValue.IsChecked = (elementValue == "on") ? true : false;
    itemValue.IDDuAnID = key;
    data.push(itemValue);
    sessionStorage.setItem('DuAnChecked', JSON.stringify(data));
}

function getAllTotalMoney() {
    var total = 0;
    $("input[type='text']").each(function () {

        total += parseInt(this.value);
    });
    return total;
}

function ChiTietKeHoach5NamDeXuat(id) {
    window.location.href = "/QLVonDauTu/KeHoachTrungHanDeXuat/Detail/?id=" + id + "&isDetail=true";
}

function ImportKH5NDX() {
    window.location.href = "/QLVonDauTu/KeHoachTrungHanDeXuat/ImportKH5NDX/";
}

function OpenModalCt(idDonVi, id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/KeHoachTrungHanDeXuat/GetModalCt",
        data: { idDonVi: idDonVi, id: id },
        success: function (data) {
            $("#modalDuAnDeXuat").html(data);
        }
    });
}

function OpenModal(id, isModified, isAggregate) {
    var lstDataChecked = JSON.parse(sessionStorage.getItem('dataChecked'));
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/KeHoachTrungHanDeXuat/GetModal",
        data: { id: id, isAggregate: isAggregate, lstItem: lstDataChecked },
        success: function (data) {
            if (isAggregate == "true") {
                var jsonData = null;
                try {
                    jsonData = JSON.parse(data);
                }
                catch (error) {
                    jsonData = null;
                }

                if (jsonData != null && jsonData.bIsComplete == false) {
                    var Title = 'Lỗi tổng hợp kế hoạch trung hạn đề xuất.';
                    var messErr = [];
                    messErr.push(jsonData.sMessError);
                    $.ajax({
                        type: "POST",
                        url: "/Modal/OpenModal",
                        data: { Title: Title, Messages: messErr, Category: ERROR },
                        success: function (data) {
                            $("#divModalConfirm").html(data);

                        }
                    });
                    return;
                }
            }
            $("#contentModalKH5NamDeXuat").html(data);
            $("#txtDxModified").val(isModified);
            $("#txtDxAggregate").val(isAggregate);

            if (id == null || id == GUID_EMPTY || id == undefined) {
                if (isAggregate == "true") {
                    $("#modalKH5NamDeXuatLabel").html('Tổng hợp kế hoạch trung hạn đề xuất');
                }
                else {
                    $("#modalKH5NamDeXuatLabel").html('Thêm mới kế hoạch trung hạn đề xuất');
                }
            }
            else {
                if (isModified == "true") {
                    $("#modalKH5NamDeXuatLabel").html('Điều chỉnh kế hoạch trung hạn đề xuất');
                }
                else {
                    $("#modalKH5NamDeXuatLabel").html('Sửa kế hoạch trung hạn đề xuất');
                }
            }
            $(".date").datepicker({
                todayBtn: "linked",
                language: "vi",
                autoclose: true,
                todayHighlight: true,
                format: 'dd/mm/yyyy'
            });
            $('#modalKH5NamDeXuat').modal('show');
        }
    });
}

function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/KeHoachTrungHanDeXuat/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalKH5NamDeXuat").html(data);
            $("#modalKH5NamDeXuatLabel").html('Chi tiết kế hoạch trung hạn đề xuất');
            $(".date").datepicker({
                todayBtn: "linked",
                language: "vi",
                autoclose: true,
                todayHighlight: true,
                format: 'dd/mm/yyyy'
            });
        }
    });
}

function ViewInBaoCao(isModified = 'false', isCt = 'false') {
    window.location.href = "/QLVonDauTu/KeHoachTrungHanDeXuat/ViewInBaoCao/?isModified=" + isModified + '&' + 'isCt=' + isCt;
}

OpenExport = (id) => {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachTrungHanDeXuat/KeHoach5NamDeXuatExport",
        data: { idKeHoach5NamDeXuat: id },
        success: function (data) {
            if (data.status) {
                window.location.href = "/QLVonDauTu/KeHoachTrungHanDeXuat/ExportExcel5NDeXuat";
            }
        }
    });
}
