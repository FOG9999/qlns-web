var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;

function GetValueExport(sSoChungTu, sNoiDung, sLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhTu, dNgayQuyetDinhDen, sSoCongVan, dNgayCongVanTu, dNgayCongVanDen) {
    $("#txt_sSoChungTuExcel").val(sSoChungTu);
    $("#txt_sNoiDungExcel").val(sNoiDung);
    $("#txt_sMaLoaiDuToanExcel").val(sLoaiDuToan);
    $("#txt_sSoQuyetDinhExcel").val(sSoQuyetDinh);
    $("#txt_dNgayQuyetDinhTuExcel").val(dNgayQuyetDinhTu);
    $("#txt_dNgayQuyetDinhDenExcel").val(dNgayQuyetDinhDen);
    $("#txt_sSoCongVanExcel").val(sSoCongVan);
    $("#txt_dNgayCongVanTuExcel").val(dNgayCongVanTu);
    $("#txt_dNgayCongVanDenExcel").val(dNgayCongVanDen);
}

function ResetChangePage(iCurrentPage = 1) {
    var sSoChungTu = "";
    var sNoiDung = "";
    var sLoaiDuToan = "";
    var sSoQuyetDinh = "";
    var dNgayQuyetDinhTu = "";
    var dNgayQuyetDinhDen = "";
    var sSoCongVan = "";
    var dNgayCongVanTu = "";
    var dNgayCongVanDen = "";

    GetValueExport(sSoChungTu, sNoiDung, sLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhTu, dNgayQuyetDinhDen, sSoCongVan, dNgayCongVanTu, dNgayCongVanDen);

    GetListData(sSoChungTu, sNoiDung, sLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhTu, dNgayQuyetDinhDen, sSoCongVan, dNgayCongVanTu, dNgayCongVanDen, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var sLoaiDuToan = $("#txtLoaiDuToan").val();
    var sSoChungTu = $("#txtSoChungTu").val();
    var sSoQuyetDinh = $("#txt_SoQuyetDinh").val();
    var dNgayQuyetDinhTu = $("#dNgayQuyetDinhFrom").val();
    var dNgayQuyetDinhDen = $("#dNgayQuyetDinhTo").val();
    var sSoCongVan = $("#txt_SoCongvan").val();
    var dNgayCongVanTu = $("#dNgayCongVanFrom").val();
    var dNgayCongVanDen = $("#dNgayCongVanTo").val();
    var sNoiDung = $("#txtNoiDung").val();

    GetValueExport(sSoChungTu, sNoiDung, sLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhTu, dNgayQuyetDinhDen, sSoCongVan, dNgayCongVanTu, dNgayCongVanDen);

    GetListData(sSoChungTu, sNoiDung, sLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhTu, dNgayQuyetDinhDen, sSoCongVan, dNgayCongVanTu, dNgayCongVanDen, iCurrentPage);
}

function GetListData(sSoChungTu, sNoiDung, sLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhTu, dNgayQuyetDinhDen, sSoCongVan, dNgayCongVanTu, dNgayCongVanDen, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { _paging: _paging, sSoChungTu: sSoChungTu, sNoiDung: sNoiDung, sLoaiDuToan: sLoaiDuToan, sSoQuyetDinh: sSoQuyetDinh, dNgayQuyetDinhTu: dNgayQuyetDinhTu, dNgayQuyetDinhDen: dNgayQuyetDinhDen, sSoCongVan: sSoCongVan, dNgayCongVanTu: dNgayCongVanTu, dNgayCongVanDen: dNgayCongVanDen },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtSoChungTu").val(sSoChungTu);
            $("#txtNoiDung").val(sNoiDung);
            $("#txtLoaiDuToan").val(sLoaiDuToan);
            $("#txt_SoQuyetDinh").val(sSoQuyetDinh);
            $("#dNgayQuyetDinhFrom").val(dNgayQuyetDinhTu);
            $("#dNgayQuyetDinhTo").val(dNgayQuyetDinhDen);
            $("#txt_SoCongvan").val(sSoCongVan);
            $("#dNgayCongVanFrom").val(dNgayCongVanTu);
            $("#dNgayCongVanTo").val(dNgayCongVanDen);

            GetValueExport(sSoChungTu, sNoiDung, sLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhTu, dNgayQuyetDinhDen, sSoCongVan, dNgayCongVanTu, dNgayCongVanDen);
        }
    });
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNguonNganSach/QLGiaoDuToanChoDV/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalGiaoDuToan").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalGiaoDuToanLabel").html('Thêm mới giao dự toán cho đơn vị');
            }
            else {
                $("#modalGiaoDuToanLabel").html('Sửa giao dự toán cho đơn vị');
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
    $("#viewdetailChungTuDotGomModel").attr("hidden", true);
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNguonNganSach/QLGiaoDuToanChoDV/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalGiaoDuToan").html(data);
            $("#modalGiaoDuToanLabel").html('Chi tiết giao dự toán cho đơn vị');
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

function Cancel() {
    window.location.href = "/QLNguonNganSach/QLGiaoDuToanChoDV/Index";
}

function Validate(data) {
    debugger
    var Title = 'Lỗi thêm mới giao dự toán cho đơn vị';
    var Messages = [];

    if (data.sMaLoaiDuToan == null || data.sMaLoaiDuToan == "") {
        Messages.push("Loại dự toán chưa chọn.");
    }

    if (data.sSoCongVan == null || data.sSoCongVan == "") {
        Messages.push("Số công văn chưa nhập.");
    }

    if (data.dNgayCongVan == null || data.dNgayCongVan == "") {
        Messages.push("Ngày công văn chưa nhập.");
    }

    if (data.sNoiDung == null || data.sNoiDung == "") {
        Messages.push("Nội dung chưa nhập.");
    } else if (data.sNoiDung.length > 250) {
        Messages.push("Nội dung vượt quá 250 kí tự.");
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

function Save() {
    var data = {};

    data.iID_DuToan = $("#txtIdDuToan").val();
    data.sMaLoaiDuToan = $("#txtLoaiDuToanModal").val();
    data.sTenLoaiDuToan = $("#txtLoaiDuToanModal option:selected").text();
    data.sNoiDung = $("#txtNoiDungModal").val();
    data.sSoChungTu = $("#txtSoChungTuModal").val();
    data.dNgayChungTu = $("#txtNgayChungTuModal").val();
    data.sSoQuyetDinh = $("#txtSoQuyetDinhModal").val();
    data.dNgayQuyetDinh = $("#txtNgayQuyetDinhModal").val();
    data.sSoCongVan = $("#txtSoCongVanModal").val();
    data.dNgayCongVan = $("#txtNgayCongVanModal").val();

    if (!Validate(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLGiaoDuToanChoDV/NNSGiaoDuToanSave",
        data: { data: data },
        success: function (data) {
            if (data.status == false) {
                var Title = "Lỗi lưu dữ liệu dự toán"
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: [data.sMessage], Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
                return false;
            }

            window.location.href = "/QLNguonNganSach/QLGiaoDuToanChoDV/ChiTietDuToan/" + data.iID_DuToan;
        }
    });
}

function DetailItem(id) {
    window.location.href = "/QLNguonNganSach/QLGiaoDuToanChoDV/Detail/" + id;
}

function DeleteItem(id) {
    var Title = 'Xác nhận xóa giao dự toán cho đơn vị';
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
        url: "/QLGiaoDuToanChoDV/NNSGiaoDuToanDelete",
        data: { idDuToan: id },
        success: function (r) {
            if (r == "True") {
                ChangePage();
            }
        }
    });
}

function UpdateChiTietDuToan(id) {
    window.location.href = "/QLNguonNganSach/QLGiaoDuToanChoDV/ChiTietDuToan/" + id;
}

function formatMoney() {
    $('#tblListNNSDuToan tr').each(function () {
        var soTien = $(this).find(".sotien").text();
        if (soTien) {
            soTien = formatNumber(soTien);
            $(this).find(".sotien").text(soTien);
        }
    });
}

function formatNumber(n) {
    // format number 1000000 to 1,234,567
    if (Number(n) >= 0) {
        return n.replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ".")
    }
    return "-" + n.replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ".")
}