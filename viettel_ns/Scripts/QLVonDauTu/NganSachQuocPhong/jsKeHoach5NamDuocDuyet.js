var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;

function Save() {
    var data = {};
    data.iID_KeHoach5NamID = $("#txt_ID_KH5NamDuocDuyet").val();
    data.iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
    data.iID_KeHoach5NamDeXuatID = $("#Id_KHTHDX").val();
    data.sSoQuyetDinh = $("#txtsSoQuyetDinh").val();
    data.dNgayQuyetDinh = $("#dNgayQuyetDinh").val();
    data.iGiaiDoanTu = $("#iGiaiDoanTu").val();
    data.iGiaiDoanDen = $("#iGiaiDoanDen").val();
    data.sMoTaChiTiet = $("#MoTaChiTiet").val();
    data.iLoai = $("#iLoai").val();
    var isDieuChinh = $("#txt_DieuChinh").val();

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachTrungHanDuocDuyet/KeHoach5NamSave",
        data: { data: data, isDieuChinh: isDieuChinh },
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/QLVonDauTu/KeHoachTrungHanDuocDuyet/Detail/?id=" + r.iID_KeHoach5NamID;
            } else {
                var Title = 'Lỗi lưu kế hoạch trung hạn được duyệt';
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
    var Title = 'Lỗi thêm mới kế hoạch trung hạn được duyệt';
    var Messages = [];

    if (data.iID_DonViQuanLyID == null || data.iID_DonViQuanLyID == "") {
        Messages.push("Đơn vị quản lý chưa chọn !");
    }

    //if (data.iID_KeHoach5NamDeXuatID == null || data.iID_KeHoach5NamDeXuatID == "") {
    //    Messages.push("Chứng từ đề xuất chưa chọn !");
    //}

    if (data.sSoQuyetDinh == null || data.sSoQuyetDinh == "") {
        Messages.push("Số kế hoạch chưa nhập !");
    }

    if (data.dNgayQuyetDinh == null || data.dNgayQuyetDinh == "") {
        Messages.push("Ngày lập chưa nhập !");
    }

    if (data.iGiaiDoanTu == null || data.iGiaiDoanTu == "") {
        Messages.push("Giai đoạn từ chưa nhập !");
    }

    if (data.iGiaiDoanDen == null || data.iGiaiDoanDen == "") {
        Messages.push("Giai đoạn đến chưa nhập !");
    }

    if (data.iGiaiDoanTu % 5 != 1) {
        Messages.push("Giai đoạn không hợp lệ.");
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
    var sSoQuyetDinh = "";
    var dNgayQuyetDinhFrom = "";
    var dNgayQuyetDinhTo = "";
    var iID_DonViQuanLyID = GUID_EMPTY;
    var MoTaChiTiet = "";
    var iGiaiDoanTu = "";
    var iGiaiDoanDen = "";
    var iLoai = "0";

    //SetValueFormExport(sSoChungTu, sNoiDung, sMaLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo);

    GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iID_DonViQuanLyID, MoTaChiTiet, iGiaiDoanTu, iGiaiDoanDen, iLoai, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var sSoQuyetDinh = $("#txt_SoQuyetdinh").val();
    var dNgayQuyetDinhFrom = $("#dNgayQuyetDinhFrom").val();
    var dNgayQuyetDinhTo = $("#dNgayQuyetDinhTo").val();
    var iID_DonViQuanLyID = $("#iID_DonViQuanLy").val();
    var MoTaChiTiet = $("#txt_MoTaChiTiet").val();
    var iGiaiDoanTu = $("#iGiaiDoanTuSearch").val();
    var iGiaiDoanDen = $("#iGiaiDoanDenSearch").val();
    var iLoai = $("#iLoaiLst").val();

    //SetValueFormExport(sSoChungTu, sNoiDung, sMaLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo);

    GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iID_DonViQuanLyID, MoTaChiTiet, iGiaiDoanTu, iGiaiDoanDen, iLoai, iCurrentPage);
}

function GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iID_DonViQuanLyID, MoTaChiTiet, iGiaiDoanTu, iGiaiDoanDen, iLoai, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/KeHoachTrungHanDuocDuyet/KeHoach5NamDuocDuyetListView",
        data: { _paging: _paging, sSoQuyetDinh: sSoQuyetDinh, dNgayQuyetDinhFrom: dNgayQuyetDinhFrom, dNgayQuyetDinhTo: dNgayQuyetDinhTo, iID_DonViQuanLyID: iID_DonViQuanLyID, sMoTaChiTiet: MoTaChiTiet, iGiaiDoanTu: iGiaiDoanTu, iGiaiDoanDen: iGiaiDoanDen, iLoai: iLoai },
        success: function (data) {
            $("#lstDataView").html(data);
            
            $("#txt_SoQuyetdinh").val(sSoQuyetDinh);
            $("#dNgayQuyetDinhFrom").val(dNgayQuyetDinhFrom);
            $("#dNgayQuyetDinhTo").val(dNgayQuyetDinhTo);
            $("#iID_DonViQuanLy").val(iID_DonViQuanLyID);
            $("#txt_MoTaChiTiet").val(MoTaChiTiet);
            $("#iGiaiDoanTuSearch").val(iGiaiDoanTu);
            $("#iGiaiDoanDenSearch").val(iGiaiDoanDen);
            $("#iLoaiLst").val(iLoai);

            //SetValueFormExport(sSoChungTu, sNoiDung, sMaLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo);
        }
    });
}

function DeleteItem(id, sSoQuyetDinh) {
    var Title = 'Xác nhận xóa kế hoạch trung hạn được duyệt';
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
        url: "/QLVonDauTu/KeHoachTrungHanDuocDuyet/KeHoach5NamDuocDuyetDelete",
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

function ChiTietKeHoach5NamDuocDuyet(id) {
    window.location.href = "/QLVonDauTu/KeHoachTrungHanDuocDuyet/Detail/" + id;
}


function OpenModal(id, isDieuChinh) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/KeHoachTrungHanDuocDuyet/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalKH5NamDuocDuyet").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalKH5NamDuocDuyetLabel").html('Thêm mới kế hoạch trung hạn được duyệt');
                $("#txt_DieuChinh").val(false);
            }
            else {
                if (isDieuChinh) {
                    $("#modalKH5NamDuocDuyetLabel").html('Điều chỉnh kế hoạch trung hạn được duyệt');
                } else {
                    $("#modalKH5NamDuocDuyetLabel").html('Sửa kế hoạch trung hạn được duyệt');
                }
                $("#txt_DieuChinh").val(isDieuChinh);
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
        url: "/QLVonDauTu/KeHoachTrungHanDuocDuyet/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalKH5NamDuocDuyet").html(data);
            $("#modalKH5NamDuocDuyetLabel").html('Chi tiết kế hoạch trung hạn được duyệt');
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

function LayDanhSachChungTuDeXuatTheoDonViQuanLy(iID_DonViQuanLyID, Id_KHTHDX) {
    $.ajax({
        url: "/QLVonDauTu/KeHoachTrungHanDuocDuyet/LayDanhSachChungTuDeXuatTheoDonViQuanLy",
        type: "POST",
        data: { iID_DonViQuanLyID: iID_DonViQuanLyID},
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null && data != "") {
                $("#Id_KHTHDX").html(data);

                //$("#Id_KHTHDX").prop("selectedIndex", 0);
                $("#Id_KHTHDX").val(Id_KHTHDX);
                //$("#fGiaTriDauTu").val("615a5272-6363-4b09-af7f-ad9500b8c729");
            }
        },
        error: function (data) {

        }
    })
}

function LayThongTinChungTuDeXuat(Id_KHTHDX) {
    $.ajax({
        url: "/QLVonDauTu/KeHoachTrungHanDuocDuyet/LayThongTinChungTuDeXuat",
        type: "POST",
        dataType: "json",
        data: { Id_KHTHDX: Id_KHTHDX},
        success: function (r) {
            if (r.bIsComplete) {
                $("#iGiaiDoanTu").val(r.data.iGiaiDoanTu);
                $("#iGiaiDoanDen").val(r.data.iGiaiDoanDen);
            }
        }
    })
}

function ViewInBaoCao(isCt = 'false') {
    window.location.href = "/QLVonDauTu/KeHoachTrungHanDuocDuyet/ViewInBaoCao/?isCt=" + isCt;
}

OpenExport = (id) => {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachTrungHanDuocDuyet/KeHoach5NamDuocDuyetExport",
        data: { idKeHoach5NamDuocDuyet: id},
        success: function (data) {
            if (data.status) {
                window.location.href = "/QLVonDauTu/KeHoachTrungHanDuocDuyet/ExportExcel5NDuocDuyet" ;
            }
        }
    });
}

function SendFile(id, idDonVi, iGiaiDoanTu, iGiaiDoanDen) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/KeHoachTrungHanDuocDuyet/SendFile",
        data: { id: id, idDonVi: idDonVi, giaiDoanTu: iGiaiDoanTu, giaiDoanDen: iGiaiDoanDen },
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