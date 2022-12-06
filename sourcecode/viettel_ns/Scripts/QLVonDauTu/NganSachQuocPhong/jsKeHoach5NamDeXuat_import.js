var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;

$(document).ready(function () {
    var inputGiaiDoanTu = $('#iGiaiDoanTu');
    inputGiaiDoanTu.on('keyup', function () {
        var key = event.keyCode || event.charCode;
        var iGiaiDoanTu = $("#iGiaiDoanTu").val();
        if (iGiaiDoanTu != "") {
            $("#iGiaiDoanDen").val(parseInt(this.value) + 4);
        } else {
            $("#iGiaiDoanDen").val("");
        }
    });

    var inputGiaiDoanDen = $('#iGiaiDoanDen');
    inputGiaiDoanDen.on('keyup', function () {
        var key = event.keyCode || event.charCode;
        var iGiaiDoanDen = $("#iGiaiDoanDen").val();
        if (iGiaiDoanDen != "") {
            $("#iGiaiDoanTu").val(parseInt(this.value) - 4);
        } else {
            $("#iGiaiDoanTu").val("");
        }
    });
});

$("#ValueItem").change(function () {
    var value = $("#ValueItem").val();
    if (value == "1") {
        document.getElementById('iID_DonViQuanLyID').removeAttribute("disabled");
        document.getElementById('iGiaiDoanTu').removeAttribute("disabled");
        document.getElementById('iGiaiDoanDen').removeAttribute("disabled");
        document.getElementById('ValueItemCt').removeAttribute("disabled");
        document.getElementById('iID_KeHoach5Nam_DeXuatIDDx').setAttribute("disabled", "disabled");
        document.getElementById('dvVoucher').style.visibility = 'collapse';
    }
    else {
        document.getElementById('iID_DonViQuanLyID').setAttribute("disabled", "disabled");        
        document.getElementById('iGiaiDoanTu').setAttribute("disabled", "disabled");
        document.getElementById('iGiaiDoanTu').setAttribute("value", "");
        document.getElementById('iGiaiDoanDen').setAttribute("disabled", "disabled");
        document.getElementById('iGiaiDoanDen').setAttribute("value", "");
        document.getElementById('ValueItemCt').setAttribute("disabled", "disabled");        
        document.getElementById('iID_KeHoach5Nam_DeXuatIDDx').removeAttribute("disabled");
        document.getElementById('dvVoucher').style.visibility = 'visible';

        //duonglt thay đổi import điều chỉnh theo chứng từ được chọn
        $("#iID_KeHoach5Nam_DeXuatIDDx").change(function () {
            if (this.value != "" && this.value != GUID_EMPTY) {
                LayGiaTriThayDoiTheoChungTuDuocChon(this.value);
            }
        });
    }
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
        url: "/QLVonDauTu/KeHoachTrungHanDeXuat/LoadDataExcel",
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
//duonglt test 
function downloadImpExp() {
    window.location.href = "/QLVonDauTu/KeHoachTrungHanDeXuat/DownloadImportExample";
}
//end test
//function loadGridListExcel() {
//    window.location.href = "/QLVonDauTu/KeHoachTrungHanDeXuat/ImportListFileExcel";
//}
function ValidateData() {
    var Title = 'Lỗi lấy dữ liệu từ file excel';
    var Messages = [];

    var has_file = $("#FileUpload").val() != '';
    if (!has_file) {
        Messages.push("Đ/c chưa chọn file excel dữ liệu !");
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
function LayGiaTriThayDoiTheoChungTuDuocChon(iID_KeHoach5Nam_DeXuatID) {
    $.ajax({
        url: "/QLVonDauTu/KeHoachTrungHanDeXuat/LayGiaTriThayDoiTheoChungTuDuocChon",
        type: "POST",
        data: { iID_KeHoach5Nam_DeXuatID: iID_KeHoach5Nam_DeXuatID },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null) {                
                document.getElementById('iGiaiDoanTu').setAttribute("value", data.iGiaiDoanTu);
                document.getElementById('iGiaiDoanDen').setAttribute("value", data.iGiaiDoanDen);
                $("#iID_DonViQuanLyID option").each(function (index, opt) {
                    if (opt.value == data.iIdMaDonVi) {
                        $(opt).prop("selected", true);                        
                    }
                });
                FilterSelectList();
            }
        },        
    })
}

function loadGridListExcel() {    
    var id_DonVi = document.getElementById('iID_DonViQuanLyID').value;
    var iGiaiDoanTu = document.getElementById('iGiaiDoanTu').value;
    var iGiaiDoanDen = document.getElementById('iGiaiDoanDen').value;
    var iLoaiCongTrinh = document.getElementById('ValueItemCt').value;
    var Title = 'Thông báo';
    var Messages = [];
    if (id_DonVi == "" || id_DonVi == null || id_DonVi == GUID_EMPTY) {
        Messages.push('Vui lòng chọn đơn vị!');
    }
    if (iGiaiDoanTu == "" || iGiaiDoanTu == null) {
        Messages.push('Vui lòng nhập Giai đoạn từ!');
    }
    if (iGiaiDoanTu == "" || iGiaiDoanTu == null) {
        Messages.push('Vui lòng nhập Giai đoạn đến!');
    }
    if (iLoaiCongTrinh == 0) {
        Messages.push('Vui lòng chọn loại công trình!');
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
        url: "/QLVonDauTu/KeHoachTrungHanDeXuat/GetGridListExcelFromFTP",
        data: { idDonVi: id_DonVi, giaiDoanTu: iGiaiDoanTu, giaiDoanDen: iGiaiDoanDen },
        success: function (data) {
            $("#contentModalKH5NamDeXuat").html(data);
            $("#modalKH5NamDeXuatLabel").html('Danh sách file kế hoạch trung hạn đề xuất');
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
            url: "/QLVonDauTu/KeHoachTrungHanDeXuat/ImportDataExcel",
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