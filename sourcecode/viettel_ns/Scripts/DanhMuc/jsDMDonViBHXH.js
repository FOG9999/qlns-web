var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var LA_DOANH_NGHIEP = 0;
var LA_DON_VI_DU_TOAN = 1;

function Save() {
    var data = {};
    data.iID_BHXH_DonViID = $("#iID_BHXH_DonViID").val();
    data.sTen = $("#txtTenDonViModal").val();
    data.iID_MaDonViBHXH = $('#txtMaDonViModal').val();
    data.iID_ParentID = $("#cboDonViParentModal").val();
    data.iID_NS_MaDonVi = $("#cboDonViNSModal").val();
    var loaiDonVi = $("#bLoaiDonViModal").val();
    if (loaiDonVi == LA_DOANH_NGHIEP)
        data.bDoanhNghiep = false;
    else if (loaiDonVi == LA_DON_VI_DU_TOAN)
        data.bDoanhNghiep = true;

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/BaoHiemXaHoi/DanhMucDonViBHXH/DonViSave",
        data: { data: data },
        success: function (r) {
            if (r.status) {
                window.location.href = "/BaoHiemXaHoi/DanhMucDonViBHXH";
            } else {
                var Title = 'Lỗi lưu đơn vị BHXH';
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
    var Title = 'Lỗi thêm mới đơn vị BHXH';
    var Messages = [];

    if (data.sTen == null || data.sTen == "") {
        Messages.push("Tên đơn vị chưa nhập !");
    }

    if (data.iID_MaDonViBHXH == null || data.iID_MaDonViBHXH == "") {
        Messages.push("Mã đơn vị chưa nhập !");
    }

    if ((data.iID_ParentID == null || data.iID_ParentID == GUID_EMPTY || data.iID_ParentID == "") && (data.iID_NS_MaDonVi == null || data.iID_NS_MaDonVi == "")) {
        Messages.push("Chưa nhập mã đơn vị cha hoặc mã đơn vị Mapping !");
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

//function GetItemData(id) {
//    window.location.href = "/QLNguonNganSach/QLDotNhan/Update/" + id;
//}

//function SetValueFormExport(sSoChungTu, sNoiDung, sMaLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo) {
//    $("#txt_sMaLoaiDuToanExcel").val(sMaLoaiDuToan);
//    $("#txt_sSoChungTuExcel").val(sSoChungTu);
//    $("#txt_sSoQuyetDinhExcel").val(sSoQuyetDinh);
//    $("#txt_dNgayQuyetDinhTuExcel").val(dNgayQuyetDinhFrom);
//    $("#txt_dNgayQuyetDinhDenExcel").val(dNgayQuyetDinhTo);
//    $("#txt_sNoiDungExcel").val(sNoiDung);
//}

function ResetChangePage(iCurrentPage = 1) {
    var sMaDonVi = "";
    var sTenDonVi = "";
    var iID_BHXH_DonVi_ParentID = "";
    var iID_MaDonVi_NS = "";

    //SetValueFormExport(sSoChungTu, sNoiDung, sMaLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo);

    GetListData(sMaDonVi, sTenDonVi, iID_BHXH_DonVi_ParentID, iID_MaDonVi_NS, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var sMaDonVi = $("#txtMaDonVi").val();
    var sTenDonVi = $("#txtTenDonVi").val();
    var iID_BHXH_DonVi_ParentID = $("#cboDonViParent").val();
    var iID_MaDonVi_NS = $("#cboDonViNS").val();

    //SetValueFormExport(sSoChungTu, sNoiDung, sMaLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo);

    GetListData(sMaDonVi, sTenDonVi, iID_BHXH_DonVi_ParentID, iID_MaDonVi_NS, iCurrentPage);
}

function GetListData(sMaDonVi, sTenDonVi, iID_BHXH_DonVi_ParentID, iID_MaDonVi_NS, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/BaoHiemXaHoi/DanhMucDonViBHXH/DonViBHXHListView",
        data: { sMaDonVi: sMaDonVi, sTenDonVi: sTenDonVi, iID_BHXH_DonVi_ParentID: iID_BHXH_DonVi_ParentID, iID_MaDonVi_NS: iID_MaDonVi_NS, _paging: _paging},
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtMaDonVi").val(sMaDonVi);
            $("#txtTenDonVi").val(sTenDonVi);
            $("#cboDonViParent").val(iID_BHXH_DonVi_ParentID);
            $("#cboDonViNS").val(iID_MaDonVi_NS);

            //SetValueFormExport(sSoChungTu, sNoiDung, sMaLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo);
        }
    });
}

function DeleteItem(id) {
    var Title = 'Xác nhận xóa đơn vị BHXH';
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
        url: "/BaoHiemXaHoi/DanhMucDonViBHXH/DeleteDonVi",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                ChangePage();
            }
            else {
                var Title = 'Lỗi xóa đơn vị BHXH';
                var Messages = [];
                Messages.push("Hãy xóa đơn vị con trước !");
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: Messages, Category: ERROR },
                    success: function (data1) {
                        $("#divModalConfirm").html(data1);
                    }
                });
            }
        }
    });
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/BaoHiemXaHoi/DanhMucDonViBHXH/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalBHXHDonVi").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalBHXHDonViLabel").html('Thêm mới đơn vị BHXH');
            }
            else {
                $("#modalBHXHDonViLabel").html('Sửa đơn vị BHXH');
            }
            $(".date").datepicker({
                todayBtn: "linked",
                language: "it",
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
        url: "/BaoHiemXaHoi/DanhMucDonViBHXH/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalBHXHDonVi").html(data);
            $("#modalBHXHDonViLabel").html('Chi tiết đơn vị BHXH');
            $(".date").datepicker({
                todayBtn: "linked",
                language: "it",
                autoclose: true,
                todayHighlight: true,
                format: 'dd/mm/yyyy'
            });
        }
    });
}

//Import
function ImportDonViBHXH() {
    window.location.href = "/BaoHiemXaHoi/DanhMucDonViBHXH/ImportDonViBHXH/";
}
