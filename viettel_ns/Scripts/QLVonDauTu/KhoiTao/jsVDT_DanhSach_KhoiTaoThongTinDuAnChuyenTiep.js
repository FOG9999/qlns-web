$(document).ready(function () {
    DinhDangSo();
})

function GetItemDataList(id) {
    window.location.href = "/QLVonDauTu/QLKhoiTaoThongTinDuAnChuyenTiep/Sua/" + id;
}

function BtnInsertDataClick() {
    location.href = "/QLVonDauTu/QLKhoiTaoThongTinDuAnChuyenTiep/TaoMoi";
}

function GetListData(iNamKhoiTao, sTenDuAn, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLKhoiTaoThongTinDuAnChuyenTiep/QLKhoiTaoDuAnChuyenTiepView",
        data: { iNamKhoiTao: iNamKhoiTao, sTenDuAn: sTenDuAn, _paging: _paging },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtiNamKhoiTao").val(iNamKhoiTao);
            $("#txtsTenDuAn").val(sTenDuAn);
            DinhDangSo();
        }
    });
}

function ChangePage(iCurrentPage = 1) {
    var iNamKhoiTao = $("#txtiNamKhoiTao").val();
    var sTenDuAn = $("#txtsTenDuAn").val();
    GetListData(iNamKhoiTao, sTenDuAn, iCurrentPage);
}

function ResetChangePage(iCurrentPage = 1) {
    var iNamKhoiTao = "";
    var sTenDuAn = "";

    GetListData(iNamKhoiTao, sTenDuAn, iCurrentPage);
}


function ViewDetailList(id) {
    window.location.href = "/QLVonDauTu/QLKhoiTaoThongTinDuAnChuyenTiep/ChiTiet/" + id;
}

function DeleteItemList(id) {
    if (!confirm("Chấp nhận xóa bản ghi ?")) return;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKhoiTaoThongTinDuAnChuyenTiep/DeleteKhoiTao",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                ChangePage();
            }
        }
    });
}

function DinhDangSo() {
    $(".sotien").each(function () {
        $(this).html(FormatNumber($(this).html().trim()) == "" ? 0 : FormatNumber($(this).html().trim()));
    })
}