var lstData = [];
var ERROR = 1;

function loadDataExcel() {
    lstData = [];
    $("#btnSave").prop('disabled', false);
    var fileInput = document.getElementById('FileUpload');
    var file = fileInput.files[0];
    var formData = new FormData();
    formData.append('file', file);
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLDuAn/LoadDataExcel",
        data: formData,
        contentType: false,
        processData: false,
        cache: false,
        async: false,
        success: function (r) {
            if (r.bIsComplete) {
                lstData = r.data
                LoadGrid();
                if (r.ListError != null && r.ListError.length != 0) {
                    $("#btnSave").prop('disabled', true);
                    var Title = 'Lỗi dữ liệu file excel';
                    $.ajax({
                        type: "POST",
                        url: "/Modal/OpenModal",
                        data: { Title: Title, Messages: r.ListError, Category: ERROR },
                        success: function (data) {
                            $("#divModalConfirm").html(data);
                        }
                    });
                }
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

function LoadGrid() {
    $("#tblImport").empty();
    if (lstData == null || lstData.length == 0) return;
    var strHtml = [];
    lstData.forEach(function (obj, index) {
        strHtml.push("<tr>");
        if (obj.bIsError) {
            strHtml.push("<td style='text-align:center'><span class='dot dot-error'><i class='fa fa-close' aria-hidden='true'></i></span></td>");
        } else {
            strHtml.push("<td style='text-align:center'><span class='dot dot-success'><i class='fa fa-check' aria-hidden='true'></i></span></td>");
        }
        strHtml.push("<td style='text-align:center'>" + obj.IStt + "</td>");
        strHtml.push("<td>" + obj.sTenDuAn + "</td>");
        strHtml.push("<td style='text-align:center'>" + obj.sMaDuAn + "</td>");
        strHtml.push("<td style='text-align:center'>" + obj.sKhoiCong + "</td>");
        strHtml.push("<td style='text-align:center'>" + obj.sKetThuc + "</td>");
        strHtml.push("<td>" + obj.sTenHangMuc + "</td>");
        strHtml.push("<td>" + obj.sMaLoaiCongTrinh + "</td>");
        strHtml.push("<td>" + obj.iID_NguonVonID + "</td>");
        strHtml.push("<td>" + obj.sDiaDiem + "</td>");
        strHtml.push("<td>" + obj.sMucTieu + "</td>");
        strHtml.push("<td style='text-align:right'>" + obj.fHanMucDauTu + "</td>");
        strHtml.push("</tr>");
    });
    $("#tblImport").html(strHtml.join(""));
}

function refreshImport() {
    $("#drpDonViQuanLy").val(null);
    $("#FileUpload").val(null);
    $("#tblImport").empty();
    lstData = [];
}

function ValidateImport() {
    var errors = [];
    var iIdDonViQuanLy = $("#drpDonViQuanLy").val();

    if (iIdDonViQuanLy == null) {
        errors.push("Đơn vị quản lý chưa được chọn !");
    }
    if (lstData == null || lstData.length == 0) {
        errors.push("Không có chứng từ import !");
    }
    if (errors.length != 0) {
        alert(errors.join("\n"));
        return false;
    }
    return true;
}

function OnSave() {
    $("#btnSave").prop('disabled', true);
    if (!ValidateImport()) {
        $("#btnSave").prop('disabled', false);
        return;
    }
    SaveDataImport();
}

function SaveDataImport() {
    var iIDDonViQuanLy = $("#drpDonViQuanLy").val();
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLDuAn/OnSaveDataImport",
        data: { iIDDonViQuanLy: iIDDonViQuanLy, lstData: lstData },
        success: function (r) {
            if (r.bIsSuccess) {
                alert("Import thành công !");
                location.href = "/QLVonDauTu/QLDuAn";
            } else {
                var Title = 'Lỗi data file import';
                var messErr = "";
                messErr = "Import thất bại !";
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