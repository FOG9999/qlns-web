var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var bIsSaveSuccess = false;

// Index

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
    var sSoQuyetDinh = "";
    var dNgayQuyetDinhFrom = "";
    var dNgayQuyetDinhTo = "";
    var iNamKeHoach = "";
    var iID_NguonVonID = "";
    var iID_DonViQuanLyID = "";

    GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iNamKeHoach, iID_NguonVonID, iID_DonViQuanLyID, iCurrentPage);
}

ChangePage = (iCurrentPage = 1) => {
    var sSoQuyetDinh = $("<div/>").text($.trim($("#txtSoQuyetdinh").val())).html();
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
        url: "/QLVonDauTu/KeHoachVonNamDuocDuyet/KeHoachVonNamDuocDuyetSearch",
        data: { _paging: _paging, sSoQuyetDinh: sSoQuyetDinh, dNgayQuyetDinhFrom: dNgayQuyetDinhFrom, dNgayQuyetDinhTo: dNgayQuyetDinhTo, iNamKeHoach: iNamKeHoach, iID_NguonVonID: iID_NguonVonID, iID_DonViQuanLyID: iID_DonViQuanLyID },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtSoQuyetdinh").val($("<div/>").html(sSoQuyetDinh).text());
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
        url: "/QLVonDauTu/KeHoachVonNamDuocDuyet/KeHoach5NamDuocDuyetDelete",
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
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/KeHoachVonNamDuocDuyet/GetModal",
        data: { id: id, idDonViQuanLy: idDonViQuanLy, isModified: isModified, isView: isView },
        success: function (data) {
            $("#contentModalKHVonNamDuocDuyetPoupup").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalKHVonNamDuocDuyetLabel").html('Thêm mới dự toán được giao');
                var dt = new Date().toLocaleDateString('en-GB');
                $("#dNgayQuyetDinhModal").val(dt);
            }
            else {
                if ($("#VoucherModified").val() == "true") {
                    $("#modalKHVonNamDuocDuyetLabel").html('Điều chỉnh dự toán được giao');
                    $("#txtSoKeHoachModal").val("");
                    var dt = new Date().toLocaleDateString('en-GB');
                    $("#dNgayQuyetDinhModal").val(dt);
                }
                else if ($("#VoucherView").val() == "true") {
                    $("#modalKHVonNamDuocDuyetLabel").html('Xem dự toán được giao');
                }
                else {
                    $("#modalKHVonNamDuocDuyetLabel").html('Sửa dự toán được giao');
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

ValidateData = (data) => {
    var Title = 'Lỗi thêm mới dự toán được giao';
    var Messages = [];

    if(data.l)

    if (data.iID_DonViQuanLyID == null || data.iID_DonViQuanLyID == "" || data.iID_DonViQuanLyID == GUID_EMPTY) {
        Messages.push("Đơn vị quản lý chưa chọn !");
    }

    if (data.sSoQuyetDinh == null || data.sSoQuyetDinh == "") {
        Messages.push("Số quyết định chưa nhập !");
    }
    if (data.sSoQuyetDinh.length>100 ) {
        Messages.push("Số quyết định nhiều hơn 100 kí tự !");
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
    data.iID_KeHoachVonNam_DuocDuyetID = $("#iID_KHVonNamDuocDuyetIDModal").val();
    data.iID_DonViQuanLyID = $("#iID_DonViQuanLyIDModal").val();
    data.sSoQuyetDinh = $("<div/>").text($.trim($("#txtSoKeHoachModal").val())).html();
    data.dNgayQuyetDinh = $("#dNgayQuyetDinhModal").val();
    data.iID_NguonVonID = $("#iID_NguonVonIDModal").val()
    data.iNamKeHoach = $("#txtNamKeHoachModal").val();

    var isModified = $("#VoucherModified").val() == "true" ? true : false;

    if (!ValidateData(data)) {
        return false;
    }
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonNamDuocDuyet/KeHoachVonNamDuocDuyetSave",
        data: { data: data, isModified: isModified },
        success: function (r) {
            if (r.bIsComplete) {
                bIsSaveSuccess = true;
                $("#modalKHVonNamDeXuat").modal("hide");
                PopupModal("Thông báo", "Lưu dữ liệu thành công", ERROR, r.iID);
            } else {
                var Title = 'Lỗi lưu dự toán được giao';
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
                    window.location.href = "/QLVonDauTu/KeHoachVonNamDuocDuyet/Detail?id=" + iID + "&isDetail=" + false;
                }
            })
        }
    });
}

// Details
ChiTietKeHoachVonNamDuocDuyet = (id,isDetail) => {
    window.location.href = "/QLVonDauTu/KeHoachVonNamDuocDuyet/Detail?id=" + id + "&isDetail=" + isDetail;
}
// Details

// Export
OpenExport = (id) => {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonNamDuocDuyet/KeHoachVonNamDuocDuyetExport",
        data: { idKeHoachVonNamDuocDuyet: id },
        success: function (data) {
            if (data.status) {
                window.location.href = "/QLVonDauTu/KeHoachVonNamDuocDuyet/ExportExcel";
            }
        }
    });
}
//Export

function SendFile(id, idDonVi, nam) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/KeHoachVonNamDuocDuyet/SendFile",
        data: { id: id, idDonVi: idDonVi, nam: nam },
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

// View In báo cáo
ViewInBaoCao = () => {
    window.location.href = "/QLVonDauTu/KeHoachVonNamDuocDuyet/ViewInBaoCao/";
}

BackIndex = () => {
    window.location.href = "/QLVonDauTu/KeHoachVonNamDuocDuyet/Index";
}

ValidateDataBaoCao = (data, arrIdDVQL) => {
    var Title = 'Lỗi in báo cáo';
    var Messages = [];

    if (data.sLoaiChungTu == null || data.sLoaiChungTu == "") {
        Messages.push("Loại chứng từ chưa chọn !");
    }

    if (data.sLoaiChungTu == "1") {
        if (data.sLoaiCongTrinh == null || data.sLoaiCongTrinh == "") {
            Messages.push("Loại công trình chưa chọn !");
        }
        if (data.sNguonVon == null || data.sNguonVon == "") {
            Messages.push("Nguồn vốn chưa chọn !");
        }
    }

    if (data.iNamLamViec == null || data.iNamLamViec == "") {
        Messages.push("Năm kế hoạch chưa nhập !");
    }

    if (arrIdDVQL.length == 0) {
        Messages.push("Đơn vị quản lý chưa chọn !");
    }

    if (data.sValueDonViTinh == null || data.sValueDonViTinh == "") {
        Messages.push("Đơn vị tính chưa chọn !");
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
    var data = {};
    var strIdLoaiCongTrinhTH = "";
    if ($("#iID_LoaiCongTrinh :selected").val() == GUID_EMPTY) {
        $("#iID_LoaiCongTrinh option").each(function () {
            if (this.value != GUID_EMPTY) {
                if (strIdLoaiCongTrinhTH == "") {
                    strIdLoaiCongTrinhTH += this.value;
                } else {
                    strIdLoaiCongTrinhTH += "," + this.value;
                }
            }
        });
        data.sLoaiCongTrinh = strIdLoaiCongTrinhTH;
    } else {
        data.sLoaiCongTrinh = $("#iID_LoaiCongTrinh :selected").val();
    }

    data.txtHeader1 = $("#txtHeader1").val();
    data.txtHeader2 = $("#txtHeader2").val();
    data.iNamLamViec = $("#txtNamKeHoach").val();
    data.sValueDonViTinh = $("#ValueItemDonViTinh :selected").val();
    data.sLoaiChungTu = $("#ValueItemLoaiChungTu :selected").val();
    data.sNguonVon = $("#iID_MaNguonNganSach :selected").val();
    data.sDonViTinh = $("#ValueItemDonViTinh :selected").html();

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
        url: "/QLVonDauTu/KeHoachVonNamDuocDuyet/PrintBaoCao",
        data: { dataReport: data, arrIdDVQL: arrIdDVQL, isPdf: isPdf},
        success: function (data) {
            if (data.status) {
                //window.location.href = "/QLVonDauTu/KeHoachVonNamDuocDuyet/ExportReport/?pdf=" + data.isPdf;
                window.open("/QLVonDauTu/KeHoachVonNamDuocDuyet/ExportReport/?pdf=" + data.isPdf, '_blank');
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