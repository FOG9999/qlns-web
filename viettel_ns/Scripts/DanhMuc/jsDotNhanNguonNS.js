var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
function CancelSaveData() {
    window.location.href = "/QLNguonNganSach/QLDotNhan/Index";
}

function BtnInsertDataClick() {
    window.location.href = "/QLNguonNganSach/QLDotNhan/Update/";
}

function Save() {
    var data = {};
    data.iID_DotNhan = $("#txt_IDDotNhan").val();
    data.sMaLoaiDuToan = $("#sMaLoaiDuToan").val();
    data.sTenLoaiDuToan = $('#sMaLoaiDuToan option:selected').html();
    data.sNoiDung = $("#txtNoiDungModal").val();
    data.sSoChungTu = $("#txtSoChungTuModal").val();
    data.dNgayChungTu = $("#txtNgayChungTuModal").val();
    data.sSoQuyetDinh = $("#txtSoQuyetDinhModal").val();
    data.dNgayQuyetDinh = $("#txtNgayQuyetDinhModal").val();

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLDotNhan/DotNhanNguonNSSave",
        data: { data: data },
        success: function (r) {
            if (r.status) {
                window.location.href = "/QLNguonNganSach/QLDotNhan/Detail/" + r.iID_DotNhan;
            } else {
                var Title = 'Lỗi lưu đợt nhận từ BTC';
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
    var Title = 'Lỗi thêm mới đợt nhận';
    var Messages = [];

    if (data.sMaLoaiDuToan == null || data.sMaLoaiDuToan == "") {
        Messages.push("Mã loại dự toán chưa chọn.");
    }

    if (data.sNoiDung == null || data.sNoiDung == "") {
        Messages.push("Nội dung chưa nhập.");
    } else if (data.sNoiDung.length > 250) {
        Messages.push("Nội dung vượt quá 250 kí tự.");
    }

    if (data.sSoQuyetDinh == null || data.sSoQuyetDinh == "") {
        Messages.push("Số quyết định chưa nhập.");
    }

    if (data.dNgayQuyetDinh == null || data.dNgayQuyetDinh == "") {
        Messages.push("Ngày quyết định chưa nhập.");
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

function GetItemData(id) {
    window.location.href = "/QLNguonNganSach/QLDotNhan/Update/" + id;
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
    var sSoChungTu = "";
    var sNoiDung = "";
    var sMaLoaiDuToan = "";
    var sSoQuyetDinh = "";
    var dNgayQuyetDinhFrom = "";
    var dNgayQuyetDinhTo = "";

    SetValueFormExport(sSoChungTu, sNoiDung, sMaLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo);

    GetListData(sSoChungTu, sNoiDung, sMaLoaiDuToan, iCurrentPage, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo);
}

function ChangePage(iCurrentPage = 1) {
    var sSoChungTu = $("#txt_SoChungtu").val();
    var sNoiDung = $("#txt_Noidung").val();
    var sMaLoaiDuToan = $("#selectLoaiDuToan").val();
    var sSoQuyetDinh = $("#txt_SoQuyetdinh").val();
    var dNgayQuyetDinhFrom = $("#dNgayQuyetDinhFrom").val();
    var dNgayQuyetDinhTo = $("#dNgayQuyetDinhTo").val();

    SetValueFormExport(sSoChungTu, sNoiDung, sMaLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo);

    GetListData(sSoChungTu, sNoiDung, sMaLoaiDuToan, iCurrentPage, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo);
}

function GetListData(sSoChungTu, sNoiDung, sMaLoaiDuToan, iCurrentPage, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNguonNganSach/QLDotNhan/NNSDotNhanListView",
        data: { sSoChungTu: sSoChungTu, sNoiDung: sNoiDung, sMaLoaiDuToan: sMaLoaiDuToan, _paging: _paging, sSoQuyetDinh: sSoQuyetDinh, dNgayQuyetDinhFrom: dNgayQuyetDinhFrom, dNgayQuyetDinhTo: dNgayQuyetDinhTo },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txt_SoChungtu").val(sSoChungTu);
            $("#txt_Noidung").val(sNoiDung);
            $("#selectLoaiDuToan").val(sMaLoaiDuToan);
            $("#txt_SoQuyetdinh").val(sSoQuyetDinh);
            $("#dNgayQuyetDinhFrom").val(dNgayQuyetDinhFrom);
            $("#dNgayQuyetDinhTo").val(dNgayQuyetDinhTo);

            SetValueFormExport(sSoChungTu, sNoiDung, sMaLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo);
        }
    });
}

function DeleteItem(id) {
    var Title = 'Xác nhận xóa đợt nhận nguồn';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn xóa?');
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
        url: "/QLNguonNganSach/QLDotNhan/DMNguonDelete",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                ChangePage();
            }
        }
    });
}

function SaveDetail() {
    var data = [];


    $('#tblListDotNhanChiTiet tr').each(function () {
        var objDotNhanChiTiet = {}
        objDotNhanChiTiet.iID_DotNhanChiTiet = $(this).find(".iDDotNhan").val();
        objDotNhanChiTiet.SoTien = $(this).find(".soTienChitiet").val();
        objDotNhanChiTiet.GhiChu = $(this).find(".ghiChuChiTiet").val();
        if (objDotNhanChiTiet.iID_DotNhanChiTiet != undefined) {
            data.push(objDotNhanChiTiet);
        }

    });

    $.ajax({
        type: "POST",
        url: "/QLDotNhan/DotNhanNguonChiTietSave",
        data: { data: data },
        success: function (r) {
            if (r == "True") {
                window.location.href = "/QLNguonNganSach/QLDotNhan/Index";
            } else {
                alert("Có lỗi trong quá trình thêm mới!");
            }
        }
    });
}

function getAllTotalMoney() {
    var total = 0;
    $("input[type='text']").each(function () {

        total += parseInt(this.value);
    });
    return total;
}

function UpdateChiTietDotNhan(id) {
    window.location.href = "/QLNguonNganSach/QLDotNhan/Detail/" + id;
}


function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNguonNganSach/QLDotNhan/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalDotNhan").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalDotNhanLabel").html('Thêm mới đợt nhận');
            }
            else {
                $("#modalDotNhanLabel").html('Sửa đợt nhận');
            }
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

function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNguonNganSach/QLDotNhan/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalDotNhan").html(data);
            $("#modalDotNhanLabel").html('Chi tiết đợt nhận');
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

