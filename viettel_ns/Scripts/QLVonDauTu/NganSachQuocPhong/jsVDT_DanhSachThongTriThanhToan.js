$(document).ready(function () {
    $("#txtDonViQuanLy").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    $("#txtMaThongTri").keyup(function (event) {
        ValidateMaxLength(this, 50);
    });
    $("#txtNgayTaoThongTri").keydown(function (event) {
        ValidateInputKeydown(event, this, 3);
    }).blur(function (event) {
        setTimeout(() => {
            ValidateInputFocusOut(event, this, 3);
        }, 0);
    });
    $("#txtNamThucHien").keydown(function (event) {
        ValidateInputKeydown(event, this, 1);
    }).blur(function (event) {
        setTimeout(() => {
            ValidateInputFocusOut(event, this, 6);
        }, 0);
    });

    $('#txtDonViQuanLy').on('change', e => {
        searchOnChangeSelect();
    })
});

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

            $("#sMaDonViXuatDanhSach").val(sMaDonVi);
            $("#sMaThongTriXuatDanhSach").val(sMaThongTri);
            $("#dNgayThongTriXuatDanhSach").val(dNgayThongTri);
            $("#iNamThongTriXuatDanhSach").val(iNamThongTri);

            $('#txtDonViQuanLy').on('change', e => {
                searchOnChangeSelect();
            })
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

function searchOnChangeSelect() {
    ChangePage();
}

function themMoi() {
    window.location.href = "/QLVonDauTu/QLThongTriThanhToan/TaoMoi/";
}

function xemChiTiet(id) {
    window.location.href = "/QLVonDauTu/QLThongTriThanhToan/ChiTiet/" + id;
}

function sua(id) {
    window.location.href = "/QLVonDauTu/QLThongTriThanhToan/Sua/" + id;
}

function xoa(id,sMaThongTri) {
    if (!confirm("Bạn có chắc chắn muốn xóa?")) return;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLThongTriThanhToan/Xoa",
        data: { id: id },
        success: function (r) {
            if (r == true) {
                alert("Xóa bản ghi " + sMaThongTri + "thành công.");
                ChangePage();
            }
        }
    });
}

function XuatFile() {
    var Messages = [];
    var lstGuidChecked = [];

    if ($("#tblListVDTThongTri  input[type=checkbox]:checked").length == 0) {
        Messages.push("Chọn ít nhất một trường");
    } else {
        jQuery.each($("#lstDataView input[type=checkbox]:checked"), function (index, item) {
            lstGuidChecked[index] = $(item).val();
        });
    }

    if (lstGuidChecked.length == 0) {
        Messages.push("Hãy chọn ít nhất một cấp phát thanh toán");
    }

    lstGuidChecked.forEach(id => {
        window.location.href = "/QLVonDauTu/QLThongTriThanhToan/ExportReport?id=" + id;
    });
}

function OpenXuatBaoCao() {
    lstGuidChecked = [];
    if ($("#tblListVDTThongTri  input[type=checkbox]:checked").length != 1) {
        alert("Chọn một trường");
    } else {
        jQuery.each($("#lstDataView input[type=checkbox]:checked"), function (index, item) {
            lstGuidChecked[index] = $(item).val();
        });
    }

    lstGuidChecked.forEach(id => {
        window.location.href = `/QLVonDauTu/QLThongTriThanhToan/XuatFilePage?id=${id}`;
    });
}

function XuatDanhSach() {
    var sMaDonVi = $("#sMaDonViXuatDanhSach").val();
    var sMaThongTri = $("#sMaThongTriXuatDanhSach").val();
    var dNgayThongTri = $("#dNgayThongTriXuatDanhSach").val();
    var iNamThongTri = $("#iNamThongTriXuatDanhSach").val();

    window.location.href = `/QLVonDauTu/QLThongTriThanhToan/XuatDanhSach?sMaDonVi=${sMaDonVi}&sMaThongTri=${sMaThongTri}&dNgayThongTri=${dNgayThongTri}&iNamThongTri=${iNamThongTri}`;
}