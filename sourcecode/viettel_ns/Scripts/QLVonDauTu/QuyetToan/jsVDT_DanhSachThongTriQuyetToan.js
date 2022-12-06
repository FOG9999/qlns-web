function ChangePage(iCurrentPage = 1) {
    var sMaDonVi = $("#txtDonViQuanLy").val();
    var sMaThongTri = $("#txtMaThongTri").val();
    var dNgayThongTri = $("#txtNgayTaoThongTri").val();
    var iNamThongTri = $("#txtNamThucHien").val();

    GetListData(sMaDonVi, sMaThongTri, dNgayThongTri, iNamThongTri, iCurrentPage);
}

function GetListData(sMaDonVi, sMaThongTri, dNgayThongTri, iNamThongTri, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: {
            sMaDonVi: sMaDonVi, sMaThongTri: sMaThongTri, dNgayThongTri: dNgayThongTri, iNamThongTri: iNamThongTri, _paging: _paging
        },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtDonViQuanLy").val(sMaDonVi);
            $("#txtMaThongTri").val(sMaThongTri);
            $("#txtNgayTaoThongTri").val(dNgayThongTri);
            $("#txtNamThucHien").val(iNamThongTri);

        }
    });
}

function ResetChangePage(iCurrentPage = 1) {
    var sMaDonVi = "";
    var sMaThongTri = "";
    var dNgayThongTri = "";
    var iNamThongTri = "";

    GetListData(sMaDonVi, sMaThongTri, dNgayThongTri, iNamThongTri, iCurrentPage);
}

function themMoi() {
    window.location.href = "/QLVonDauTu/QLThongTriQuyetToan/TaoMoi/";
}

function xemChiTiet(id) {
    window.location.href = "/QLVonDauTu/QLThongTriQuyetToan/Sua/" + id + "?bIsViewDetail=1";
}

function sua(id) {
    window.location.href = "/QLVonDauTu/QLThongTriQuyetToan/Sua/" + id;
}

function xoa(id) {
    if (!confirm("Bạn có chắc chắn muốn xóa?")) return;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLThongTriQuyetToan/Xoa",
        data: { id: id },
        success: function (r) {
            if (r == true) {
                ChangePage();
            }
        }
    });
}

function OnExportExcel() {
    var count = 0;
    $.each($("input[name=ck_id]:checked"),function () {
        window.location.href = "/QLVonDauTu/QLThongTriQuyetToan/XuatFile?Id=" + $(this).val();
    });
}