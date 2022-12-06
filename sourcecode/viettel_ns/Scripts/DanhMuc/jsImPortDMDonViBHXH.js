var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var LA_DOANH_NGHIEP = 0;
var LA_DON_VI_DU_TOAN = 1;

function SaveData() {
    //var data = {};
    //data.iID_BHXH_DonViID = $("#iID_BHXH_DonViID").val();
    //data.sTen = $("#txtTenDonViModal").val();
    //data.iID_MaDonViBHXH = $('#txtMaDonViModal').val();
    //data.iID_ParentID = $("#cboDonViParentModal").val();
    //data.iID_NS_MaDonVi = $("#cboDonViNSModal").val();
    //var loaiDonVi = $("#bLoaiDonViModal").val();
    //if (loaiDonVi == LA_DOANH_NGHIEP)
    //    data.bDoanhNghiep = true;
    //else if (loaiDonVi == LA_DON_VI_DU_TOAN)
    //    data.bDoanhNghiep = false;

    //if (!ValidateData(data)) {
    //    return false;
    //}

    $.ajax({
        type: "POST",
        url: "/BaoHiemXaHoi/DanhMucDonViBHXH/DonViSaveFile",
        //data: { data: data },
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

//Import
function ImportDonViBHXH() {
    window.location.href = "/BaoHiemXaHoi/DanhMucDonViBHXH/ImportDonViBHXH/";
}

function GoBack() {
    window.location.href = "/BaoHiemXaHoi/DanhMucDonViBHXH";
}

function LoadDataXLSX() {
    if (!ValidateXLSX()) {
        return false;
    }

    var fileInput = document.getElementById('FileUpload');
    var file = fileInput.files[0];
    var formData = new FormData();
    formData.append('file', file);
    $.ajax({
        type: "POST",
        url: "/BaoHiemXaHoi/DanhMucDonViBHXH/LoadDataXLSX",
        data: formData,
        contentType: false,
        processData: false,
        cache: false,
        async: false,
        success: function (data) {
            $("#lstDataView").html(data);
            //$('.nav-tabs a[href="#danhsachDung"]').tab('show');
        }
    });
}

function ValidateXLSX() {
    var Title = 'Lỗi lấy dữ liệu từ file XLSX';
    var Messages = [];

    var has_file = $("#FileUpload").val() != '';
    if (!has_file) {
        Messages.push("Đ/c chưa chọn file XLSX dữ liệu !");
    }
    else {
        var ext = $("#FileUpload").val().split(".");
        ext = ext[ext.length - 1].toLowerCase();
        var arrayExtensions = ["xls", "xlsx"];
        if (arrayExtensions.lastIndexOf(ext) == -1) {
            Messages.push("Đ/c chọn file không đúng định dạng !");
        }
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
