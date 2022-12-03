var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var listItemAfter = []
var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;

$(document).ready(function () {

});

function loadDataExcel() {
    if (!ValidateData()) {
        return false;
    }

    var fileInput = document.getElementById('FileUpload');
    var file = fileInput.files[0];
    var formData = new FormData();
    formData.append('file', file);
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonNamDeXuat/LoadDataExcel",
        data: formData,
        contentType: false,
        processData: false,
        cache: false,
        async: false,
        success: function (r) {
            if (r.bIsComplete) {
                loadFrame();
            } else {
                var Title = 'Lỗi lấy dữ liệu từ file excel';
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

function ValidateData() {
    var Title = 'Lỗi lấy dữ liệu từ file excel';
    var Messages = [];

    var has_file = $("#FileUpload").val() != '';
    if (!has_file) {
        Messages.push("Đ/c chưa chọn file excel dữ liệu !");
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

function loadGridListExcel() {
    var id_DonVi = document.getElementById('iID_DonViQuanLyID').value;
    var iNam = document.getElementById('txtNamKeHoach').value;
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
            data: { Title: Title, Messages: Messages, Category: ERROR },
            success: function (data) {
                $("#divModalConfirm").html(data);
            }
        });
        return false;
    }
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/KeHoachVonNamDeXuat/GetGridListExcelFromFTP",
        data: { idDonVi: id_DonVi, nam: iNam},
        success: function (data) {
            $("#contentModalKH5NamDeXuat").html(data);
            $("#modalKH5NamDeXuatLabel").html('Danh sách file kế hoạch vốn năm đề xuất');
            $('#modalKH5NamDeXuat').modal('show');
        }
    });
}

function ImportFile() {
    //if (!ValidateData()) {
    //    return false;
    //}
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
            data: { Title: Title, Messages: Messages, Category: ERROR },
            success: function (data) {
                $("#divModalConfirm").empty().html(data);
            }
        });
    } else {
        let url = $("input[type='checkbox'][name='checkboxInRow']:checked").first().val();
        $.ajax({
            type: "POST",
            url: "/QLVonDauTu/KeHoachVonNamDeXuat/ImportDataExcel",
            data: { url: url },
            success: function (r) {
                if (r.bIsComplete) {
                    loadFrame();
                    $('#modalKH5NamDeXuat').modal('hide');
                } else {
                    var Title = 'Lỗi lấy dữ liệu từ file excel';
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
}

function SaveKhvn() {
    var obj = {};
    obj.iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
    obj.sSoQuyetDinh = $("#txtsSoQuyetDinh").val();
    obj.dNgayQuyetDinh = $("#dNgayQuyetDinh").val();
    obj.iID_NguonVonID = $("#iID_NguonVonID").val();
    obj.iNamKeHoach = $("#txtNamKeHoach").val();

    if (!ValidatePreImport(obj)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonNamDeXuat/SaveImport",
        data: { data: obj },
        dataType: "json",
        cache: false,
        success: function (r) {
            if (r.bIsComplete) {
                $("#txt_ID_KHVonNamDeXuat").val(r.Id);
                var url = loadFrameImport(obj.iNamKeHoach, true);
                $("#sheetImport").attr("src", url);
                $(".btnSaveKhvn").attr('disabled', 'disabled');


            } else {
                var Title = 'Lỗi lưu kế hoạch vốn năm đề xuất';
                var messErr = [];
                messErr.push(r.sMessage);
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

function ValidatePreImport(obj) {

    var Title = 'Lỗi lưu kế hoạch vốn năm đè xuất';
    var Messages = [];
    var check = true;
    //var obj = {};
    //obj.iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
    //obj.sSoQuyetDinh = $("#txtsSoQuyetDinh").val();
    //obj.dNgayQuyetDinh = $("#dNgayQuyetDinh").val();
    //obj.iID_NguonVonID = $("#iID_NguonVonID").val();
    //obj.iNamKeHoach = $("#txtNamKeHoach").val();

    if (obj.iID_DonViQuanLyID == undefined || obj.iID_DonViQuanLyID == null || obj.iID_DonViQuanLyID == "" || obj.iID_DonViQuanLyID == GUID_EMPTY) {
        Messages.push("Đơn vị không được để trống.");
    }

    if (obj.sSoQuyetDinh == undefined || obj.sSoQuyetDinh == null || obj.sSoQuyetDinh == "") {
        Messages.push("Số quyết định không được để trống.");
    } else {
        if ($.trim(obj.sSoQuyetDinh).length > 100) {
            Messages.push("Số quyết định không được quá 100 ký tự.");

        }
    }

    if (obj.dNgayQuyetDinh == undefined || obj.dNgayQuyetDinh == null || obj.dNgayQuyetDinh == "") {
        Messages.push("Ngày quyết định không được để trống.");
    }

    if (obj.iID_NguonVonID == undefined || obj.iID_NguonVonID == null || obj.iID_NguonVonID == "" || obj.iID_NguonVonID == GUID_EMPTY) {
        Messages.push("Nguồn vốn không được để trống.");
    }

    if (obj.iNamKeHoach == undefined || obj.iNamKeHoach == null || obj.iNamKeHoach == "" || obj.iNamKeHoach == GUID_EMPTY) {
        Messages.push("Nguồn vốn không được để trống.");
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
    } else {
        $.ajax({
            type: "POST",
            url: "/QLVonDauTu/KeHoachVonNamDeXuat/ValidatePreImport",
            data: { data: obj },
            dataType: "json",
            cache: false,
            success: function (r) {
                if (r.bIsComplete) {
                    check = true;
                } else {
                    Messages.push(r.sMessError);
                    $.ajax({
                        type: "POST",
                        url: "/Modal/OpenModal",
                        data: { Title: Title, Messages: Messages, Category: ERROR },
                        success: function (data) {
                            $("#divModalConfirm").html(data);
                        }
                    });
                    check = false;
                }
            }
        });
    }
    if (!check) {
        return false;
    }

    return true;
}

function loadFrameImport(iNamKeHoach,bIsImport) {
    return "/QLVonDauTu/KeHoachVonNamDeXuat/SheetFrameImport?id=" + iNamKeHoach + "&bIsImport=" + bIsImport ;
}

