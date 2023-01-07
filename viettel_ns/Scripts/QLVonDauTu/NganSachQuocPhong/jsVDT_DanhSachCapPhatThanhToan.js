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
            $("#lstDataViewThanhToan").html(data);
            $("#txtSoDeNghi").val(sSoDeNghi);
            $("#txtNamKeHoach").val(iNamKeHoach);
            $("#drpDonViQuanLy").val(sDonViQuanLy);
            $('#drpDuAn').val(iid_DuAnID);
            $('#drpLoaiThanhToan').val(iLoaiThanhToan);

            $("#sSoDeNghiXuatDanhSach").val(sSoDeNghi);
            $("#dFromNgayDeNghiXuatDanhSach").val(dNgayDeNghiFrom);
            $("#dToNgayDeNghiXuatDanhSach").val(dNgayDeNghiTo);
            $("#iNamKeHoachXuatDanhSach").val(iNamKeHoach);
            $("#sMaDonViXuatDanhSach").val(sDonViQuanLy);
            tabThongTri();
        }
    });
}

function ChangePage(iCurrentPage = 1) {
    if (checkIfTabThongTriActive()) {
        var sMaDonVi = $("#txtDonViQuanLy").val();
        var sMaThongTri = $("#txtMaThongTri").val();
        var dNgayThongTri = $("#txtNgayTaoThongTri").val();
        var iNamThongTri = $("#txtNamThucHien").val();

        GetListDataThongTri(sMaDonVi, sMaThongTri, dNgayThongTri, iNamThongTri, iCurrentPage);
    }
    else {
        var sSoDeNghi = $("#txtSoDeNghi").val().trim();
        var iNamKeHoach = $("#txtNamKeHoach").val();
        var dNgayDeNghiFrom = $("#txtNgayDeNghiFrom").val();
        var dNgayDeNghiTo = $("#txtNgayDeNghiTo").val();
        var sDonViQuanLy = $("#drpDonViQuanLy option:selected").val();
        var iid_DuAnID = $("#drpDuAn option:selected").val();
        var iLoaiThanhToan = $("#drpLoaiThanhToan option:selected").val();
        GetListData(sSoDeNghi, iNamKeHoach, iLoaiThanhToan, sDonViQuanLy, iid_DuAnID, iCurrentPage, dNgayDeNghiFrom, dNgayDeNghiTo);        
    }
    
}

function DeleteItemList(id,sSoDeNghi) {
    if (!confirm("Chấp nhận xóa bản ghi ?")) return;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/DeleteDeNghiThanhToan",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                alert("Xóa bản ghi " + sSoDeNghi + " thành công.");
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
    var Title = 'Lỗi thêm mới thông tri thanh toán';
    var Messages = [];

    var iNguonVon = -1;
    var iNamKeHoach = -1;
    var iLoaiThanhToan = -1;
    var iID_DonViQuanLyID = -1;

    if ($("#lstDataViewThanhToan input[type=checkbox]:checked").length == 0) {
        Messages.push("Chọn ít nhất một trường");
    } else {
        jQuery.each($("#lstDataViewThanhToan input[type=checkbox]:checked"), function (index, item) {
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
            let dNgayPheDuyet = $(item).data('ngaypheduyet');
            if (!dNgayPheDuyet) {
                Messages.push("Chứng từ chưa được tạo phê duyệt không thể tạo thông tri");
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
                $("#txtDonViQuanLy").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
                $('#txtDonViQuanLy').on('change', e => {
                    ChangePage();
                })
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

function XuatDanhSach() {
    var sSoDeNghiXuatDanhSach = $("#sSoDeNghiXuatDanhSach").val();
    var dFromNgayDeNghiXuatDanhSach = $("#dFromNgayDeNghiXuatDanhSach").val().split("/").reverse().join("/");;
    var dToNgayDeNghiXuatDanhSach = $("#dToNgayDeNghiXuatDanhSach").val().split("/").reverse().join("/");;;
    var iNamKeHoachXuatDanhSach = $("#iNamKeHoachXuatDanhSach").val();
    var sMaDonViXuatDanhSach = $("#sMaDonViXuatDanhSach").val();

    window.location.href = `/QLVonDauTu/GiaiNganThanhToan/XuatDanhSach?sSoDeNghi=${sSoDeNghiXuatDanhSach}&iNamKeHoach=${iNamKeHoachXuatDanhSach}&iLoaiThanhToan=${null}&sDonViQuanLy=${sMaDonViXuatDanhSach}&dNgayDeNghiFrom=${dFromNgayDeNghiXuatDanhSach}&iid_DuAnID=${null}&dNgayDeNghiTo=${dToNgayDeNghiXuatDanhSach}`;
}



// Thông tri thanh toán
// các hàm CRUD bên trên liên quan đến thông tri k sử dụng

//var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
//var CONFIRM = 0;
//var ERROR = 1;

//var _paging = {};

//function ResetChangePage(iCurrentPage = 1) {
//    var iID_DonViQuanLy = "";
//    var sMaThongTri = "";
//    var dNgayTaoThongTri = "";
//    var iNamThucHien = "";
//    var sNguoiLap = "";
//    var sTruongPhong = "";
//    var sThuTruongDonVi = "";

//    $("#txt_DonViQuanLySearch").val("");
//    $("#txt_MaThongTriSearch").val("");
//    $("#txt_NgayTaoThongTriSearch").val("");
//    $("#txt_NamThucHienSearch").val("");
//    $("#txt_NgayLapSearch").val("");
//    $("#txt_TruongPhongBanTaiChinhSearch").val("");
//    $("#txt_ThuTruongDonViSearch").val("");

//    GetListData(iID_DonViQuanLy, sMaThongTri, dNgayTaoThongTri, iNamThucHien, sNguoiLap, sTruongPhong, sThuTruongDonVi, iCurrentPage);
//    SetValueFormExportExcel(iID_DonViQuanLy, sMaThongTri, dNgayTaoThongTri, iNamThucHien, sNguoiLap, sTruongPhong, sThuTruongDonVi);
//}

//function SetValueFormExportExcel(iID_DonViQuanLy, sMaThongTri, dNgayTaoThongTri, iNamThucHien, sNguoiLap, sTruongPhong, sThuTruongDonVi) {
//    $("#txt_iIDonViQuanLyExcel").val(iID_DonViQuanLy);
//    $("#txt_sMaThongTriExcel").val(sMaThongTri);
//    $("#txt_dNgayTaoThongTriExcel").val(dNgayTaoThongTri);
//    $("#txt_iNamThucHienExcel").val(iNamThucHien);
//    $("#txt_sNguoiLapExcel").val(sNguoiLap);
//    $("#txt_sTruongPhongExcel").val(sTruongPhong);
//    $("#txt_sThuTruongDonViExcel").val(sThuTruongDonVi);
//}

//function ChangePage(iCurrentPage = 1) {
//    var iID_DonViQuanLy = $("#txt_DonViQuanLySearch").val();
//    var sMaThongTri = $("#txt_MaThongTriSearch").val();
//    var dNgayTaoThongTri = $("#txt_NgayTaoThongTriSearch").val();
//    var iNamThucHien = $("#txt_NamThucHienSearch").val();
//    var sNguoiLap = $("#txt_NgayLapSearch").val();
//    var sTruongPhong = $("#txt_TruongPhongBanTaiChinhSearch").val();
//    var sThuTruongDonVi = $("#txt_ThuTruongDonViSearch").val();

//    GetListData(iID_DonViQuanLy, sMaThongTri, dNgayTaoThongTri, iNamThucHien, sNguoiLap, sTruongPhong, sThuTruongDonVi, iCurrentPage);
//    SetValueFormExportExcel(iID_DonViQuanLy, sMaThongTri, dNgayTaoThongTri, iNamThucHien, sNguoiLap, sTruongPhong, sThuTruongDonVi);
//}

//function GetListData(iID_DonViQuanLy, sMaThongTri, dNgayTaoThongTri, iNamThucHien, sNguoiLap, sTruongPhong, sThuTruongDonVi, iCurrentPage) {
//    _paging.CurrentPage = iCurrentPage;
//    $.ajax({
//        type: "POST",
//        dataType: "html",
//        url: sUrlListView,
//        data: { _paging: _paging, iID_DonViQuanLy: iID_DonViQuanLy, sMaThongTri: sMaThongTri, dNgayTaoThongTri: dNgayTaoThongTri, iNamThucHien: iNamThucHien, sNguoiLap: sNguoiLap, sTruongPhong: sTruongPhong, sThuTruongDonVi: sThuTruongDonVi },
//        success: function (data) {
//            $("#lstDataViewThanhToan").html(data);
//            $("#txt_DonViQuanLySearch").val(iID_DonViQuanLy);
//            $("#txt_MaThongTriSearch").val(sMaThongTri);
//            $("#txt_NgayTaoThongTriSearch").val(dNgayTaoThongTri);
//            $("#txt_NamThucHienSearch").val(iNamThucHien);
//            $("#txt_NgayLapSearch").val(sNguoiLap);
//            $("#txt_TruongPhongBanTaiChinhSearch").val(sTruongPhong);
//            $("#txt_ThuTruongDonViSearch").val(sThuTruongDonVi);

//            SetValueFormExportExcel(iID_DonViQuanLy, sMaThongTri, dNgayTaoThongTri, iNamThucHien, sNguoiLap, sTruongPhong, sThuTruongDonVi);
//        }
//    });
//}

//function ViewCreate() {
//    window.location.href = "/QLVonDauTu/VDT_ThongTri/Create";
//}

//function ViewUpdate(id) {
//    window.location.href = "/QLVonDauTu/VDT_ThongTri/Update/" + id;
//}

//function ViewDetail(id) {
//    window.location.href = "/QLVonDauTu/VDT_ThongTri/Detail/" + id;
//}

//function DeleteItem(id) {
//    var Title = 'Xác nhận xóa giao dự toán cho đơn vị';
//    var Messages = [];
//    Messages.push('Bạn có chắc chắn muốn xóa?');
//    var FunctionName = "Delete('" + id + "')";
//    $.ajax({
//        type: "POST",
//        url: "/Modal/OpenModal",
//        data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
//        success: function (data) {
//            $("#divModalConfirm").html(data);
//        }
//    });
//}

//function Delete(id) {
//    $.ajax({
//        type: "POST",
//        url: "/QLVonDauTu/VDT_ThongTri/Delete",
//        data: { id: id },
//        success: function (r) {
//            if (r.status == true) {
//                ChangePage();
//            }
//        }
//    });
//}


// tab thông tri

function xemChiTiet(id) {
    window.location.href = "/QLVonDauTu/QLThongTriThanhToan/ChiTiet/" + id;
}

function sua(id) {
    window.location.href = "/QLVonDauTu/QLThongTriThanhToan/Sua/" + id;
}

function xoa(id) {
    if (!confirm("Bạn có chắc chắn muốn xóa?")) return;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLThongTriThanhToan/Xoa",
        data: { id: id },
        success: function (r) {
            if (r == true) {
                ChangePage();
            }
        }
    });
}

function XuatFileThongTri() {
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

function XuatDanhSachThongTri() {
    var sMaDonVi = $("#sMaDonViXuatDanhSach").val();
    var sMaThongTri = $("#sMaThongTriXuatDanhSach").val();
    var dNgayThongTri = $("#dNgayThongTriXuatDanhSach").val();
    var iNamThongTri = $("#iNamThongTriXuatDanhSach").val();

    window.location.href =
        `/QLVonDauTu/QLThongTriThanhToan/XuatDanhSachThanhToanView?sMaDonVi=${sMaDonVi ? sMaDonVi : ''}&sMaThongTri=${sMaThongTri ? sMaThongTri : ''}&dNgayThongTri=${dNgayThongTri ? dNgayThongTri : ''}&iNamThongTri=${iNamThongTri ? iNamThongTri : ''
}`;
}

function GetListDataThongTri(sMaDonVi, sMaThongTri, dNgayThongTri, iNamThongTri, iCurrentPage) {
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

            $("#txtDonViQuanLy").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
            $('#txtDonViQuanLy').on('change', e => {
                ChangePage();
            })
        }
    });
}

function checkIfTabThongTriActive() {
    return $("#thongtri").attr('class').split(/\s+/).indexOf('active') >= 0;
}

function OnExportExcel() {
    var lstId = [];
    $("#tblThanhToan input[type=checkbox]:checked").each(function () {
        lstId.push($(this).val());
    });
    if (lstId.length == 0) {
        alert("Chưa chọn chứng từ thanh toán !");
        return;
    }
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/ExportExcelPhanGhiCQTC",
        data: { lstId: lstId },
        success: function (data) {
            if (data.status) {
                window.open("/QLVonDauTu/GiaiNganThanhToan/ExportReport?pdf=" + data.isPdf);
            }
            else {
                var Title = 'Lỗi in báo cáo';
                var messErr = [];
                messErr.push(data.listErrMess);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
                return false;
            }
        }
    });
    return true;
}