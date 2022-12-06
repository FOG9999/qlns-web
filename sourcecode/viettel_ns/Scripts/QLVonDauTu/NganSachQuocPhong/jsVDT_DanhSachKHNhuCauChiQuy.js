//============================== Event List ================================//
var ERROR = 1;
var tempListId = [];
function GetItemDataList(id) {
    window.location.href = "/QLVonDauTu/QLKeHoachChiQuy/Update/" + id;
}

function ViewDetailList(id) {
    window.location.href = "/QLVonDauTu/QLKeHoachChiQuy/Detail/" + id;
}

function GetListData(sSoDeNghi, iNamKeHoach, dNgayDeNghiFrom, dNgayDeNghiTo, iIDMaDonViQuanLy, iIDNguonVon, iQuy, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLKeHoachChiQuy/KHNhuCauChiQuyView",
        data: {
            sSoDeNghi: sSoDeNghi, iNamKeHoach: iNamKeHoach, dNgayDeNghiFrom: dNgayDeNghiFrom, dNgayDeNghiTo: dNgayDeNghiTo,
            iIDMaDonViQuanLy: iIDMaDonViQuanLy, iIDNguonVon: iIDNguonVon, iQuy: iQuy, _paging: _paging
        },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtSoDeNghi").val(sSoDeNghi);
            $("#txtNamKeHoach").val(iNamKeHoach);
            $("#txtNgayDeNghiFrom").val(dNgayDeNghiFrom);
            $("#txtNgayDeNghiTo").val(dNgayDeNghiTo);
            $("#drpDonViQuanLy").val(iIDMaDonViQuanLy);
            $("#drpNguonNganSach").val(iIDNguonVon);
            $("#drpQuy").val(iQuy);
        }
    });
}

function ChangePage(iCurrentPage = 1) {
    var sSoDeNghi = $("#txtSoDeNghi").val();
    var iNamKeHoach = $("#txtNamKeHoach").val();
    var dNgayDeNghiFrom = $("#txtNgayDeNghiFrom").val();
    var dNgayDeNghiTo = $("#txtNgayDeNghiTo").val();
    var iIDMaDonViQuanLy = $("#drpDonViQuanLy option:selected").val();
    var iIDNguonVon = $("#drpNguonNganSach option:selected").val();
    var iQuy = $("#drpQuy option:selected").val();
    GetListData(sSoDeNghi, iNamKeHoach, dNgayDeNghiFrom, dNgayDeNghiTo, iIDMaDonViQuanLy, iIDNguonVon, iQuy, iCurrentPage);
}

function DeleteItemList(id) {
    if (!confirm("Chấp nhận xóa bản ghi ?")) return;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKeHoachChiQuy/XoaKeHoachChiQuy",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                ChangePage();
            }
        }
    });
}

function BtnInsertDataClick() {
    location.href = "/QLVonDauTu/QLKeHoachChiQuy/Insert";
}

function XuatFile(id) {
    location.href = "/QLVonDauTu/QLKeHoachChiQuy/XuatFile?id=" + id;
}
function SendFile(id, idDonVi, nam, quy) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLKeHoachChiQuy/SendFile",
        data: { id: id, idDonVi: idDonVi, nam: nam, quy: quy },
        success: function (data) {
            $.ajax({
                type: "POST",
                url: "/Modal/OpenModal",
                data: { Title: "Thông báo", Messages: "Gửi file thành công!", Category: 1 },
                async: false,
                success: function (data) {
                    $("#divModalConfirm").html(data);
                }
            });
        }
    });
}

function BtnDownloadDataClick() {
    $("#modalKeHoachChiQuy").attr('tabindex', '');
    $("#modalKH5NamDeXuat").attr('tabindex', '');
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLKeHoachChiQuy/ImportData",
        success: function (data) {
            $("#contentModalKH5NamDeXuat").html(data);
            $("#modalKH5NamDeXuatLabel").html('Danh sách file kế hoạch chi quý đề xuất');
            $('#modalKH5NamDeXuat').modal('show');
        }
    });
}
function loadGridListExcel() {
    var id_DonVi = document.getElementById("drpDonViQuanLyImport").value;
    var iNam = document.getElementById('txtNamKeHoachImport').value;
    var iQuy = document.getElementById("drpQuyImport").value;
    var Title = 'Thông báo';
    var Messages = [];
    if (id_DonVi == "" || id_DonVi == null) {
        Messages.push('Vui lòng chọn đơn vị!');
    }
    if (iNam == "" || iNam == null) {
        Messages.push('Vui lòng nhập Năm kế hoạch!');
    }

    if (Messages != null && Messages != undefined && Messages.length > 0) {
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: Messages, Category: 1 },
            success: function (data) {
                $("#divModalConfirm").empty().html(data);
            }
        });
        return false;
    }
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLKeHoachChiQuy/GetGridListExcelFromFTP",
        data: { idDonVi: id_DonVi, nam: iNam, quy: iQuy},
        success: function (data) {
            $("#contentModalKH5NamDeXuat").html(data);
            $("#modalKH5NamDeXuatLabel").html('Danh sách file kế hoạch chi quý đề xuất');
            $('#modalKH5NamDeXuat').modal('show');
            $("#drpDonViQuanLy").val(id_DonVi);
            $("#txtNamKeHoach").val(iNam);
            $("#drpQuy").val(iQuy);
        }
    });
}

function DownloadFile() {
    let lg = $("input[type='checkbox'][name='checkboxInRow']:checked").length;
    if (lg != 1) {
        var Title = 'Thông báo';
        var Messages = [];

        if (lg < 1) {
            Messages.push('Vui lòng chọn một file để thực hiện đồng bộ dữ liệu!');
        } else {
            Messages.push('Vui lòng chỉ chọn một file để thực hiện đồng bộ dữ liệu!');
        }

        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: Messages, Category: 1 },
            success: function (data) {
                $("#divModalConfirm").empty().html(data);
            }
        });
    } else {
        let url = $("input[type='checkbox'][name='checkboxInRow']:checked").first().val(); 
        $('#modalKH5NamDeXuat').modal('hide');
        location.href = "/QLVonDauTu/QLKeHoachChiQuy/DownloadFileExcel?url=" + url;
    }
}

//Báo cáo kế hoạch chi quý
function printBaoCao(ext) {
    var links = [];
    /*var ext = $(this).data("ext");*/
    var txtTieuDe1 = $("#txtTieuDe1").val();
    var txtTieuDe2 = $("#txtTieuDe2").val();
    var iVND = $("#iVND").val();
    //var dvt = $("#dvt").val();
    var Title = 'Lỗi nhập dữ liệu nhu cầu chi quý';
    var Messages = [];

    if (txtTieuDe1 == null || txtTieuDe1 == "") {
        Messages.push("Chưa nhập tiêu đề 1 !");
    }
    if (txtTieuDe2 == null || txtTieuDe2 == "") {
        Messages.push("Chưa nhập tiêu đề 2 !");
    }
    else if (txtTieuDe2 < 0) {
        Messages.push("Nguồn vốn phải lớn hơn 0 !");
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

    for (var i = 0; i < tempListId.length; i++) {
        var iId = tempListId[i];

        var url = $("#urlExportKeHoachChiQuy").val() +
            "?ext=" + ext + "&txtTieuDe1=" + txtTieuDe1
            + "&txtTieuDe2=" + txtTieuDe2
            + "&iVND=" + iVND
            + "&iId=" + iId;

        url = unescape(url);
        links = [];
        links.push(url);

        openLinks(links);
    }
}

function InBaoCaoModal() {
    var returnError = 0;
    listId = [];
    var setTable;
    setTable = $("#tbListKeHoachChiQuy");
    setTable.find('tr').each(function () {
        if ($(this).find('input[type="checkbox"]').is(':checked')) {
            var id = $(this).find('input[type="checkbox"]').data("id")
            listId.push(id);
        }
    });
    tempListId = [];
    tempListId = listId;
    //alert error
    if (listId.length == 0) {
        var Title = 'Vui lòng chọn kế hoạch chi quý';
        var Error = "Bạn phải chọn kế hoạch chi quý mới được In báo cáo!"
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: [Error], Category: ERROR },
            success: function (res) {
                $("#divModalConfirm").html(res);
            }
        });
    } else {
        //show modal tong hop
        $.ajax({
            type: "POST",
            dataType: "html",
            url: "/QLVonDauTu/QLKeHoachChiQuy/GetModalInBaoCaoChiQuy",
            data: { listId: listId },
            success: function (data) {
                $("#modalKeHoachChiQuy").modal("show")
                $("#contentModalKeHoachChiQuy").empty().html(data);
                $("#modalKeHoachChiQuyLabel").empty().html('Báo cáo kế hoạch chi quý');
            }
        });
    }
}