//============================== Event List ================================//
var LstGuidChecked = [];

$(document).ready(function () {
    tabThongTri();
    var isBackFromThongTri = $("#isBackFromThongTri").val();
    if (isBackFromThongTri == 1) {
        $("ul.nav li:first-child").removeClass("active");
        $("#thanhtoan").removeClass("active");
        $("ul.nav li:nth-child(2)").addClass("active");
        $("#thongtri").addClass("active");
        $("#btnShowConfirmDelete").click();
    }
    //$("#iID_ChuDauTuID").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });

    //$("#txtSoDeNghi").keyup(function (event) {
    //    ValidateMaxLength(this, 50);
    //});

    //$("#txtNgayDeNghiFrom").keydown(function (event) {
    //    ValidateInputKeydown(event, this, 3);
    //}).blur(function (event) {
    //    setTimeout(() => {
    //        ValidateInputFocusOut(event, this, 3);
    //    }, 0);
    //});
    //$("#txtNgayDeNghiTo").keydown(function (event) {
    //    ValidateInputKeydown(event, this, 3);
    //}).blur(function (event) {
    //    setTimeout(() => {
    //        ValidateInputFocusOut(event, this, 3);
    //    }, 0);
    //});
    //$("#txtNamKeHoach").keydown(function (event) {
    //    ValidateInputKeydown(event, this, 1);
    //}).blur(function (event) {
    //    setTimeout(() => {
    //        ValidateInputFocusOut(event, this, 6);
    //    }, 0);
    //});
    //$("#drpDonViQuanLy").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
});

function GetItemDataList(id) {
    window.location.href = "/QLVonDauTu/GiaiNganThanhToan/Update/" + id;
}

function ViewDetailList(id) {
    window.location.href = "/QLVonDauTu/GiaiNganThanhToan/Detail/" + id;
}

function GetListData(sSoDeNghi, iNamKeHoach, iLoaiThanhToan, sDonViQuanLy, iid_DuAnID, iCurrentPage, dNgayDeNghiFrom, dNgayDeNghiTo) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/GiaiNganThanhToan/GiaiNganThanhToanView",
        data: { sSoDeNghi: sSoDeNghi, iNamKeHoach: iNamKeHoach, iLoaiThanhToan: iLoaiThanhToan, sDonViQuanLy: sDonViQuanLy, iid_DuAnID: iid_DuAnID, _paging: _paging, dNgayDeNghiFrom: dNgayDeNghiFrom, dNgayDeNghiTo: dNgayDeNghiTo },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtSoDeNghi").val(sSoDeNghi);
            $("#txtNamKeHoach").val(iNamKeHoach);
            $("#drpDonViQuanLy").val(sDonViQuanLy);
            $('#drpDuAn').val(iid_DuAnID);
            $('#drpLoaiThanhToan').val(iLoaiThanhToan);
        }
    });
}

function ChangePage(iCurrentPage = 1) {
    var sSoDeNghi = $("#txtSoDeNghi").val().trim();
    var iNamKeHoach = $("#txtNamKeHoach").val();
    var dNgayDeNghiFrom = $("#txtNgayDeNghiFrom").val();
    var dNgayDeNghiTo = $("#txtNgayDeNghiTo").val();
    var sDonViQuanLy = $("#drpDonViQuanLy option:selected").val();
    var iid_DuAnID = $("#drpDuAn option:selected").val();
    var iLoaiThanhToan = $("#drpLoaiThanhToan option:selected").val();
    GetListData(sSoDeNghi, iNamKeHoach, iLoaiThanhToan, sDonViQuanLy, iid_DuAnID, iCurrentPage, dNgayDeNghiFrom, dNgayDeNghiTo);
}

function DeleteItemList(id) {
    if (!confirm("Chấp nhận xóa bản ghi ?")) return;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/DeleteDeNghiThanhToan",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                ChangePage();
            }
        }
    });
}

function BtnInsertDataClick() {
    location.href = "/QLVonDauTu/GiaiNganThanhToan/Insert";
}

var iIdDeNghiThanhToanId = "";
function XuatFile(id) {
    iIdDeNghiThanhToanId = id;
    $('#configBaocao').modal('show');
}

$(".btn-print").click(function () {
    var links = [];
    var ext = $(this).data("ext");
    var typeBC = $('input[name=loaiBC]:checked').val();
    var dvt = $("#dvt").val();
    var url = $("#urlExport").val() +
        "?ext=" + ext +
        "&dvt=" + dvt +
        "&type=" + typeBC +
        "&id=" + iIdDeNghiThanhToanId;

    url = unescape(url);
    links.push(url);
    openLinks(links);
});

function OpenModal() {
    var Title = 'Lỗi thêm mới kế hoạch vốn năm đề xuất';
    var Messages = [];

    var iNguonVon = -1;
    var iNamKeHoach = -1;
    var iLoaiThanhToan = -1;
    var iID_DonViQuanLyID = -1;

    if ($("#lstDataView input[type=checkbox]:checked").length == 0) {
        Messages.push("Chọn ít nhất một trường");
    } else {
        jQuery.each($("#lstDataView input[type=checkbox]:checked"), function (index, item) {
            if (iNguonVon == -1) {
                iNguonVon = $(item).data("iidnguonvon");
            } else {
                if (iNguonVon != $(item).data("iidnguonvon")) {
                    Messages.push("Chọn cùng nguồn vốn");
                }
            }
            if (iNamKeHoach == -1) {
                iNamKeHoach = $(item).data("inamkehoach");
            } else {
                if (iNamKeHoach != $(item).data("inamkehoach")) {
                    Messages.push("Chọn cùng năm kế hoạch");
                }
            }
            if (iLoaiThanhToan == -1) {
                iLoaiThanhToan = $(item).data("iloaithanhtoan");
            } else {
                if (iLoaiThanhToan != $(item).data("iloaithanhtoan")) {
                    Messages.push("Chọn cùng loại thanh toán");
                }
            }
            if (iID_DonViQuanLyID == -1) {
                iID_DonViQuanLyID = $(item).data("donviquanlyid");
            } else {
                if (iID_DonViQuanLyID != $(item).data("donviquanlyid")) {
                    Messages.push("Chọn cùng đơn vị");
                }
            }
            LstGuidChecked[index] = $(item).val();
        });
    }

    if (LstGuidChecked.length == 0) {
        Messages.push("Hãy chọn ít nhất một cấp phát thanh toán");
    }

    if (Messages != null && Messages != undefined && Messages.length > 0) {
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { model: { Title: Title, Messages: Messages, Category: 1 } },
            success: function (data) {
                $("#divModalTaoMoi").html(data);
            }
        });
        //return false;
    } else {
        $.ajax({
            type: "POST",
            dataType: "html",
            url: "/QLVonDauTu/GiaiNganThanhToan/GetModal",
            data: { lstItem: LstGuidChecked, iIdNguonVon: iNguonVon, iNamKeHoach: iNamKeHoach, iLoaiThanhToan: iLoaiThanhToan, iID_DonViQuanLyID: iID_DonViQuanLyID },
            success: function (data) {
                $("#divModalTaoMoi").html(data);
                $("#sMaThongTri").keyup(function () {
                    ValidateMaxLength(this, 50);
                });
                $("#dNgayThongTri").keydown(function (event) {
                    ValidateInputKeydown(event, this, 3);
                }).blur(function (event) {
                    setTimeout(() => {
                        ValidateInputFocusOut(event, this, 3);
                    }, 0);
                });
                $("#sMoTa").keyup(function () {
                    ValidateMaxLength(this, 500);
                });
            }
        });
    }
}

function Huy() {
    //window.location.href = "/QLVonDauTu/GiaiNganThanhToan/Index";
    //$("#divModalTaoMoi").hide();

}

function Luu() {
    var thongTri = {};
    thongTri.bThanhToan = true;
    thongTri.iID_DonViID = $("#iID_DonViQuanLyID").val();
    thongTri.iNamThongTri = $("#sNamThongTri").val();
    thongTri.sMaNguonVon = $("#sMaNguonVon").val();
    thongTri.iLoaiThongTri = $("#iLoaiThongTri").val();
    thongTri.sMaThongTri = $("#sMaThongTri").val().trim();
    thongTri.dNgayThongTri = $("#dNgayThongTri").val();
    thongTri.sMoTa = $("#sMoTa").val().trim();

    if (CheckLoi(thongTri)) {
        $.ajax({
            url: "/QLVonDauTu/GiaiNganThanhToan/Luu",
            type: "POST",
            data: { model: thongTri, lstGuidChecked: LstGuidChecked },
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data.status == true) {
                   // tabThongTri();
                    //$("ul.nav li:first-child").removeClass("active");
                    //$("#thanhtoan").removeClass("active");
                    //$("ul.nav li:nth-child(2)").addClass("active");
                    //$("#thongtri").addClass("active");
                    //$("#btnShowConfirmDelete").click();
                    window.location.href = "/QLVonDauTu/GiaiNganThanhToan/DetailChiTiet?id=" + data.iID ;

                }
            },
            error: function (data) {
                $("#btnShowConfirmDelete").click();
            }
        });
    }

}

function CheckLoi(thongTri) {
    Messages = [];
    if (!thongTri.sMaThongTri) {
        Messages.push("Mã thông tri trống");
    }
    if (!thongTri.dNgayThongTri) {
        Messages.push("Ngày lập trống");
    }
    if (KiemTraTrungMaThongTri(thongTri.sMaThongTri))
        Messages.push("Mã thông tri đã tồn tại, vui lòng nhập mã khác.");
    if (Messages.length > 0) {
        alert(Messages.join("\n"));
        return false;
    }
    return true;
}

function KiemTraTrungMaThongTri(sMaThongTri) {
    var check = false;
    $.ajax({
        url: "/QLVonDauTu/QLThongTriThanhToan/KiemTraTrungMaThongTri",
        type: "POST",
        data: { sMaThongTri: sMaThongTri },
        dataType: "json",
        async: false,
        cache: false,
        success: function (data) {
            check = data;
        },
        error: function (data) {

        }
    })
    return check;
}

function tabThongTri() {
    $.ajax({
        url: "/QLVonDauTu/GiaiNganThanhToan/GetTabThongTri",
        type: "GET",
        //data: { model: thongTri, lstGuidChecked: LstGuidChecked },
        dataType: "html",
        cache: false,
        success: function (data) {
            if (data != null && data != "") {
                $("#thongtri").html(data);
            }
        },
        error: function (data) {

        }
    });
}

function OpenDetail(id) {
    window.location.href = "/QLVonDauTu/QLThongTriThanhToan/Sua?id=" + id + "&isFromThongTri=" + 0;
    //$.ajax({
    //    url: "/QLVonDauTu/QLThongTriThanhToan/Sua",
    //    type: "GET",
    //    data: { id: id },
    //    dataType: "html",
    //    cache: false,
    //    success: function (data) {
    //        if (data != null && data != "") {
    //            //window.location.href = "/QLVonDauTu/GiaiNganThanhToan/Index";
    //            $("#thongtri").html(data);
    //        }
    //    },
    //    error: function (data) {}
    //});
}