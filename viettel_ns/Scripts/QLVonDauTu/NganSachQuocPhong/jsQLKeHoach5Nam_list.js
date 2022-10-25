function ChangePage(iCurrentPage = 1) {
    var sSoQuyetDinh = $("#txtSoQuyetDinh").val();
    var iGiaiDoanTu = $("#txtGiaiDoanTu").val() == "" ? 0 : parseInt(UnFormatNumber($("#txtGiaiDoanTu").val()));
    var iGiaiDoanDen = $("#txtGiaiDoanDen").val() == "" ? 0 : parseInt(UnFormatNumber($("#txtGiaiDoanDen").val()));
    var dNgayQuyetDinhFrom = $("#txtNgayQuyetDinhFrom").val();
    var dNgayQuyetDinhTo = $("#txtNgayQuyetDinhTo").val();
    var sMaDonVi = $("#txtDonViQuanLy").val();
    GetListData(sSoQuyetDinh, iGiaiDoanTu, iGiaiDoanDen, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, sMaDonVi, iCurrentPage);
}

function GetListData(sSoQuyetDinh, iGiaiDoanTu, iGiaiDoanDen, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, sMaDonVi, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: {
            sSoQuyetDinh: sSoQuyetDinh, iGiaiDoanTu: iGiaiDoanTu, iGiaiDoanDen: iGiaiDoanDen, dNgayQuyetDinhFrom: dNgayQuyetDinhFrom,
            dNgayQuyetDinhTo: dNgayQuyetDinhTo, sMaDonVi: sMaDonVi, _paging: _paging
        },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtSoQuyetDinh").val(sSoQuyetDinh);
            $("#txtGiaiDoanTu").val(iGiaiDoanTu == 0 ? "" : parseInt(UnFormatNumber(iGiaiDoanTu)));
            $("#txtGiaiDoanDen").val(iGiaiDoanDen == 0 ? "" : parseInt(UnFormatNumber(iGiaiDoanDen)));
            $("#txtNgayQuyetDinhFrom").val(dNgayQuyetDinhFrom);
            $("#txtNgayQuyetDinhTo").val(dNgayQuyetDinhTo);
            $("#txtDonViQuanLy").val(sMaDonVi);
        }
    });
}


function themMoi() {
    window.location.href = "/QLVonDauTu/KeHoach5Nam/Add/";
}

function xemChiTiet(id) {
    window.location.href = "/QLVonDauTu/KeHoach5Nam/Detail/" + id;
}

function sua(id) {
    window.location.href = "/QLVonDauTu/KeHoach5Nam/Update/" + id;
}

function dieuChinh(id) {
    window.location.href = "/QLVonDauTu/KeHoach5Nam/DieuChinh/" + id;
}

function xoa(id) {
    if (!confirm("Bạn có chắc chắn muốn xóa?")) return;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoach5Nam/Delete",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                ChangePage();
            }
        }
    });
}