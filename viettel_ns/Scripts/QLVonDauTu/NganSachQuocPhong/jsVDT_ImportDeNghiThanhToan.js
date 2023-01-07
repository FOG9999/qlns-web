var lstData = [];
var ERROR = 1;

function loadDataExcel() {
    lstData = [];
    //if (!ValidateData()) {
    //    return false;
    //}

    var fileInput = document.getElementById('FileUpload');
    var file = fileInput.files[0];
    var formData = new FormData();
    formData.append('file', file);
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/LoadDataExcel",
        data: formData,
        contentType: false,
        processData: false,
        cache: false,
        async: false,
        success: function (r) {
            if (r.bIsComplete) {
                lstData = r.data
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
            LoadGrid();
        }
    });
}

function LoadGrid() {
    $("#tblDeNghiThanhToan").empty();
    if (lstData == null || lstData.length == 0) return;
    var strHtml = [];
    lstData.forEach(function (obj, index) {
        strHtml.push("<tr>");
        strHtml.push("<td style='text-align:center'>" + index +"</td>");
        strHtml.push("<td>" + obj.sSoDeNghi + "</td>");
        strHtml.push("<td style='text-align:center'>" + obj.sNgayDeNghi + "</td>");
        strHtml.push("<td>" + obj.sMaLoaiDeNghi + "</td>");
        strHtml.push("<td>" + obj.sLoaiDeNghi + "</td>");
        strHtml.push("<td>" + obj.sTenDuAn + "</td>");
        strHtml.push("<td>" + obj.sMaDuAn + "</td>");
        strHtml.push("<td>" + obj.sSoHopDong + "</td>");
        strHtml.push("<td>" + obj.sMaNguonVon + "</td>");
        strHtml.push("<td>" + obj.sNguonVon + "</td>");
        strHtml.push("<td>" + obj.sMaLoaiKeHoachVon + "</td>");
        strHtml.push("<td>" + obj.sKeHoachVon + "</td>");
        strHtml.push("<td>" + obj.sNamKeHoach + "</td>");
        strHtml.push("<td>" + obj.sNoiDung + "</td>");
        strHtml.push("<td style='text-align:right'>" + obj.sSoDeNghiThanhToanTn + "</td>");
        strHtml.push("<td style='text-align:right'>" + obj.sSoDeNghiThanhToanNn + "</td>");
        strHtml.push("<td style='text-align:right'>" + obj.sSoThuHoiTamUng_CheDoTn + "</td>");
        strHtml.push("<td style='text-align:right'>" + obj.sSoThuHoiTamUng_CheDoNn + "</td>");
        strHtml.push("<td style='text-align:right'>" + obj.sSoThuHoiTamUng_UngTruocTn + "</td>");
        strHtml.push("<td style='text-align:right'>" + obj.sSoThuHoiTamUng_UngTruocNn + "</td>");
        strHtml.push("</tr>");
    });
    $("#tblDeNghiThanhToan").html(strHtml.join(""));
}

function refreshImport() {
    $("#txtNamKeHoach").val(null);
    $("#FileUpload").val(null);
    $("#tblDeNghiThanhToan").empty();
    lstData = [];
}

function ValidateImport() {
    var errors = [];
    var iIdDonViQuanLy = $("#drpDonViQuanLy").val();
    var iIdChuDauTuID = $("#iID_ChuDauTuID").val();
    var iIdCoQuanThanhToan = $("#drpCoQuanThanhToan").val();
    var iIdNguonVonId = $("#iIdNguonVonId").val();

    if (iIdDonViQuanLy == null) {
        errors.push("Đơn vị quản lý chưa được chọn !");
    }
    if (iIdChuDauTuID == null) {
        errors.push("Chủ đầu tư chưa được chọn !");
    }
    if (iIdCoQuanThanhToan == null) {
        errors.push("Cơ quan thanh toán chưa được chọn !");
    }
    if (iIdNguonVonId == null) {
        errors.push("Nguồn vốn chưa được chọn !");
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
    ValidateDataDetail();

    $("#btnSave").prop('disabled', false);
}

function ValidateDataDetail() {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/ValidateChungTuChiTietImport",
        data: { sMaChuDauTu: $("#iID_ChuDauTuID").val(), iIdNguonVonId: $("#iIdNguonVonId").val(), iIdCoQuanThanhToan: $("#drpCoQuanThanhToan").val(), iNamKeHoach: $("#txtNamKeHoach").val(), lstData: lstData  },
        success: function (r) {
            if (r.bIsSuccess) {
                SaveDataImport();
            } else {
                var Title = 'Lỗi data file import';
                var messErr = "";
                messErr = r.MessError;
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

function SaveDataImport() {
    var sMaDonVi = $("#drpDonViQuanLy").val();
    var sMaChuDauTu = $("#iID_ChuDauTuID").val();
    var iIdNguonVonId = $("#iIdNguonVonId").val();
    var iNamKeHoach = $("#txtNamKeHoach").val();
    var iIdCoQuanThanhToan = $("#drpCoQuanThanhToan").val();
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/OnSaveDeNghiThanhToanImport",
        data: { sMaDonVi: sMaDonVi, sMaChuDauTu: sMaChuDauTu, iIdNguonVonId: iIdNguonVonId, iNamKeHoach: iNamKeHoach, iIdCoQuanThanhToan: iIdCoQuanThanhToan, lstData: lstData },
        success: function (r) {
            if (r.bIsSuccess) {
                alert("Import thành công !");
                location.href = "/QLVonDauTu/GiaiNganThanhToan";
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