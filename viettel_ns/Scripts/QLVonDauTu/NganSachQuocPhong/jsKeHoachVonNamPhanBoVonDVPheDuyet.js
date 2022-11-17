var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var bIsSaveSuccess = false;

var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;
var ERROR = 1;

function BangDuLieu_onKeypress_Delete(h, c) {

}

function BangDuLieu_XoaHang(cs) {
    if (cs != null && 0 <= cs && cs < Bang_nH) {
        Bang_arrHangDaXoa[cs] = !Bang_arrHangDaXoa[cs];
        Bang_HienThiDuLieu();
        return true;
    }

    return false;
}

function ValidateBeforeSave() {
    var listItem = [];

    for (var i = 0; i < Bang_arrThayDoi.length; i++) {
        var object = {
            Id: "",
            iID_PhanBoVon_DonVi_PheDuyet_ID: "",
            iID_DuAnID: "",
            fGiaTriPhanBo: 0,
            fGiaTriThuHoi: 0,
            iID_DonViTienTeID: "",
            iID_TienTeID: 0,
            fTiGiaDonVi: 0,
            fTiGia: "",
            iID_LoaiCongTrinh: "",
            iId_Parent: "",
            bActive: "",
            ILoaiDuAn: "",
            sGhiChu: ""
        };
        // gán giá trị mới cho kế hoạch vđt được duyệt chi tiết
        object.Id = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID"]];
        object.iID_PhanBoVon_DonVi_PheDuyet_ID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_PhanBoVon_DonVi_PheDuyet_ID"]];
        object.iID_DuAnID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_DuAnID"]];
        object.sMaDuAn = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaDuAn"]];
        object.sTenDuAn = Bang_arrGiaTri[i][Bang_arrCSMaCot["sTenDuAn"]];
        object.fGiaTriPhanBo = Bang_arrGiaTri[i][Bang_arrCSMaCot["fGiaTriPhanBo"]];
        object.fGiaTriThuHoi = Bang_arrGiaTri[i][Bang_arrCSMaCot['fGiaTriThuHoi']];
        object.iID_DonViTienTeID = Bang_arrGiaTri[i][Bang_arrCSMaCot['iID_DonViTienTeID']];
        object.iID_TienTeID = Bang_arrGiaTri[i][Bang_arrCSMaCot['iID_TienTeID']];
        object.fTiGiaDonVi = Bang_arrGiaTri[i][Bang_arrCSMaCot['fTiGiaDonVi']];
        object.fTiGia = Bang_arrGiaTri[i][Bang_arrCSMaCot['fTiGia']];
        object.iID_LoaiCongTrinh = Bang_arrGiaTri[i][Bang_arrCSMaCot['iID_LoaiCongTrinh']];
        object.iId_Parent = Bang_arrGiaTri[i][Bang_arrCSMaCot['iId_Parent']];
        object.bActive = Bang_arrGiaTri[i][Bang_arrCSMaCot['bActive']];
        object.iLoaiDuAn = Bang_arrGiaTri[i][Bang_arrCSMaCot["iLoaiDuAn"]];
        object.sGhiChu = Bang_arrGiaTri[i][Bang_arrCSMaCot["sGhiChu"]];

        listItem.push(object);
    }

    AjaxRequire(listItem);
}

function KHVonNamPBVDVPheDuyet_ChiTiet_BangDuLieu_Save() {
    ValidateBeforeSave();
}

ResetChangePage = (iCurrentPage = 1) => {
    var sSoQuyetDinh = "";
    var dNgayQuyetDinhFrom = "";
    var dNgayQuyetDinhTo = "";
    var iNamKeHoach = "";
    var iID_NguonVonID = "";
    var iID_DonViQuanLyID = "";

    GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iNamKeHoach, iID_NguonVonID, iID_DonViQuanLyID, iCurrentPage);
}

ChangePage = (iCurrentPage = 1) => {
    var sSoQuyetDinh = $("#txtSoQuyetdinh").val();
    var dNgayQuyetDinhFrom = $("#dNgayQuyetDinhFrom").val();
    var dNgayQuyetDinhTo = $("#dNgayQuyetDinhTo").val();
    var iNamKeHoach = $("#txtNamKeHoach").val();
    var iID_NguonVonID = $("#iID_NguonVonID").val();
    var iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();

    GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iNamKeHoach, iID_NguonVonID, iID_DonViQuanLyID, iCurrentPage);
}


GetListData = (sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iNamKeHoach, iID_NguonVonID, iID_DonViQuanLyID, iCurrentPage) => {
    _paging.CurrentPage = iCurrentPage;

    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/KeHoachVonNamPhanBoVonDVPheDuyet/KeHoachVonNamPhanBoVonDVPheDuyetSearch",
        data: { _paging: _paging, sSoQuyetDinh: sSoQuyetDinh, dNgayQuyetDinhFrom: dNgayQuyetDinhFrom, dNgayQuyetDinhTo: dNgayQuyetDinhTo, iNamKeHoach: iNamKeHoach, iID_NguonVonID: iID_NguonVonID, iID_DonViQuanLyID: iID_DonViQuanLyID },
        success: function (data) {
            $("#lstDataView").html(data);

            $("#txtSoQuyetdinh").val(sSoQuyetDinh);
            $("#dNgayQuyetDinhFrom").val(dNgayQuyetDinhFrom);
            $("#dNgayQuyetDinhTo").val(dNgayQuyetDinhTo);
            $("#txtNamKeHoach").val(iNamKeHoach);
            $("#iID_NguonVonID").val(iID_NguonVonID);
            $("#iID_DonViQuanLy").val(iID_DonViQuanLyID);
        }
    });
}

// Index

// Delete voucher
DeleteItem = (id) => {
    var Title = 'Xác nhận xóa chứng từ';
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
        url: "/QLVonDauTu/KeHoachVonNamPhanBoVonDVPheDuyet/KeHoachVonNamPhanBoVonDVPheDuyetDelete",
        data: { id: id },
        success: function (data) {
            if (data.status == true) {
                ChangePage();
            }
        }
    });
}
// Delete voucher


// Insert And Update

OpenModal = (id, idDonViQuanLy = GUID_EMPTY, isModified = false, isView = false) => {
    $("#modalKHVonNamDeXuat").attr('tabindex', '');
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/KeHoachVonNamPhanBoVonDVPheDuyet/GetModal",
        data: { id: id, idDonViQuanLy: idDonViQuanLy, isModified: isModified, isView: isView },
        success: function (data) {
            $("#contentModalKHVonNamDuocDuyetPoupup").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalKHVonNamDuocDuyetLabel").html('Thêm mới kế hoạch vốn năm được duyệt');
                var dt = new Date().toLocaleDateString('en-GB');
                $("#dNgayQuyetDinhModal").val(dt);
            }
            else {
                if ($("#VoucherModified").val() == "true") {
                    $("#modalKHVonNamDuocDuyetLabel").html('Điều chỉnh kế hoạch vốn năm được duyệt');
                    $("#txtSoKeHoachModal").val("");
                    var dt = new Date().toLocaleDateString('en-GB');
                    $("#dNgayQuyetDinhModal").val(dt);

                }
                else if ($("#VoucherView").val() == "true") {
                    $("#modalKHVonNamDuocDuyetLabel").html('Xem kế hoạch vốn năm được duyệt');
                }
                else {
                    $("#modalKHVonNamDuocDuyetLabel").html('Sửa kế hoạch vốn năm được duyệt');
                }
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
        url: "/QLVonDauTu/KeHoachVonNamPhanBoVonDVPheDuyet/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalKHVonNamDuocDuyetPoupup").html(data);
            $("#modalKHVonNamDuocDuyetLabel").html('Chi tiết kế hoạch vốn năm phân bổ vốn đơn vị phê duyệt');
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

ValidateData = (data) => {
    var Title = 'Lỗi thêm mới kế hoạch vốn năm được duyệt';
    var Messages = [];

    if (data.l)

        if (data.iID_DonViQuanLyID == null || data.iID_DonViQuanLyID == "" || data.iID_DonViQuanLyID == GUID_EMPTY) {
            Messages.push("Đơn vị quản lý chưa chọn !");
        }

    if (data.sSoQuyetDinh == null || data.sSoQuyetDinh == "") {
        Messages.push("Số kế hoạch chưa nhập !");
    }

    if (data.dNgayQuyetDinh == null || data.dNgayQuyetDinh == "") {
        Messages.push("Ngày lập chưa nhập !");
    }

    if (data.iID_NguonVonID == null || data.iID_NguonVonID == "" || data.iID_NguonVonID == GUID_EMPTY) {
        Messages.push("Nguồn vốn chưa chọn !");
    }

    if (data.iNamKeHoach == null || data.iNamKeHoach == "") {
        Messages.push("Năm kế hoạch chưa nhập !");
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
// Insert And Update

Save = () => {
    var data = {};
    data.Id = $("#iID_KHVPBVDonViPheDuyetIDModal").val();
    data.iID_DonViQuanLyID = $("#iID_DonViQuanLyIDModal").val();
    data.sSoQuyetDinh = $("#txtSoKeHoachModal").val();
    data.dNgayQuyetDinh = $("#dNgayQuyetDinhModal").val();
    data.iID_NguonVonID = $("#iID_NguonVonIDModal").val()
    data.iNamKeHoach = $("#txtNamKeHoachModal").val();

    var isModified = $("#VoucherModified").val() == "true" ? true : false;

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonNamPhanBoVonDVPheDuyet/KeHoachVonNamPhanBoVonDVPheDuyetSave",
        data: { data: data, isModified: isModified },
        success: function (r) {
            if (r.bIsComplete) {
                bIsSaveSuccess = true;
                window.location.href = "/QLVonDauTu/KeHoachVonNamPhanBoVonDVPheDuyet/Detail/" + r.iID;
            } else {
                var Title = 'Lỗi lưu kế hoạch vốn năm được duyệt';
                var messErr = [];
                messErr.push(r.sMessError);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                        location.window.href('/QLVonDauTu/KeHoachVonNamPhanBoVonDVPheDuyet/Detail/' + r.iID);
                    }
                });
            }
        }
    });
}

//popup
function PopupModal(title, message, category, iID) {
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: title, Messages: message, Category: category },
        success: function (data) {
            $("#divModalConfirm").html(data);
            $('#confirmModal').on('hidden.bs.modal', function () {
                if (bIsSaveSuccess) {
                    //window.location.href = "/QLVonDauTu/KeHoachVonNamPhanBoVonDVPheDuyet/Index";
                    window.location.href = "/QLVonDauTu/KeHoachVonNamPhanBoVonDVPheDuyet/Detail?id=" + iID;
                }
            })
        }
    });
}




// Details
ChiTietKeHoachVonNamDuocDuyet = (id) => {
    window.location.href = "/QLVonDauTu/KeHoachVonNamPhanBoVonDVPheDuyet/Detail/" + id;
}

XemChiTietKeHoachVonNamDuocDuyet = (id, isViewDetail) => {
    window.location.href = "/QLVonDauTu/KeHoachVonNamPhanBoVonDVPheDuyet/Detail?id=" + id + "&isViewDetail=" + isViewDetail;
}
// Details

// Export
OpenExport = (id, idDonVi, nam) => {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonNamPhanBoVonDVPheDuyet/KeHoachVonNamPhanBoVonDVPheDuyetExport",
        data: { idKeHoachVonNamDuocDuyet: id },
        success: function (data) {
            if (data.status) {
                window.location.href = "/QLVonDauTu/KeHoachVonNamPhanBoVonDVPheDuyet/ExportExcel?idDonVi=" + idDonVi + "&nam=" + nam;
            }
        }
    });
}
//Export

// View In báo cáo
ViewInBaoCao = () => {
    $("#ValueItemLoaiChungTu").attr('tabindex', '');
    window.location.href = "/QLVonDauTu/KeHoachVonNamPhanBoVonDVPheDuyet/ViewInBaoCao/";
}

BackIndex = () => {
    window.location.href = "/QLVonDauTu/KeHoachVonNamPhanBoVonDVPheDuyet/Index";
}

ValidateDataBaoCao = (data, arrIdDVQL) => {
    var Title = 'Lỗi in báo cáo';
    var Messages = [];

    data.sLoaiCongTrinh = $("#iIdLoaiCongTrinh").val();
    if (data.sLoaiCongTrinh == null || data.sLoaiCongTrinh == "") {
        Messages.push("Loại công trình chưa chọn !");
    }

    data.sLoaiChungTu = $("#iIdLoaiChungTu").val();
    if (data.sLoaiChungTu == null || data.sLoaiChungTu == "") {
        Messages.push("Loại chứng từ chưa chọn !");
    }

    data.iNamLamViec = $("#txtNamKeHoach").val();
    if (data.iNamLamViec == null || data.iNamLamViec == "") {
        Messages.push("Năm kế hoạch chưa chọn !");
    }

    data.sNguonVon = $("#iID_MaNguonNganSachDv").val();
    if (data.sNguonVon == null || data.sNguonVon == "") {
        Messages.push("Loại nguồn vốn chưa chọn !");
    }

    data.sValueDonViTinh = $("#ValueItemDonViTinhDv").val();
    if (data.sValueDonViTinh == null || data.sValueDonViTinh == "") {
        Messages.push("Đơn vị tính chưa chọn !");
    } else {
        data.sDonViTinh = $("#ValueItemDonViTinhDv :selected").text();
    }

    if (arrIdDVQL.length == 0) {
        Messages.push("Đơn vị quản lý chưa chọn !");
    }

    data.txtHeader1 = $("#txtHeader1").val();
    if (data.txtHeader1 == null || data.txtHeader1 == "") {
        Messages.push("Tiêu đề 1 không được để trống !");
    }

    data.txtHeader2 = $("#txtHeader2").val();
    if (data.txtHeader2 == null || data.txtHeader2 == "") {
        Messages.push("Tiêu đề 2 không được để trống !");
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

PrintBaoCao = (isPdf = true) => {
    debugger;
    var data = {};
    var arrIdDVQL = [];


    $("#tblListDonViQuanLy input[type=checkbox]:checked").each(function () {
        var rowValue = $(this).val();
        if (rowValue != 'on') {
            arrIdDVQL.push(rowValue);
        }
    });

    if (!ValidateDataBaoCao(data, arrIdDVQL)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonNamPhanBoVonDVPheDuyet/PrintBaoCao",
        data: { dataReport: data, arrIdDVQL: arrIdDVQL, isPdf: isPdf },
        success: function (data) {
            if (data.status) {
                //window.location.href = "/QLVonDauTu/KeHoachVonNamPhanBoVonDVPheDuyet/ExportReport/?pdf=" + data.isPdf;
                window.open("/QLVonDauTu/KeHoachVonNamPhanBoVonDVPheDuyet/ExportReport/?pdf=" + data.isPdf, '_blank');
            }
            else {
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: "Lỗi in báo cáo", Messages: data.listErrMess, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
                return false;
            }
        }
    });
}
// View In báo cáo