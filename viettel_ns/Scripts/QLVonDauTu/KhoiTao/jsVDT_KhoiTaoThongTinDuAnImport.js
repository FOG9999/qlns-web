var lstData = [];
var ERROR = 1;

function loadDataExcel() {
    lstData = [];
    var sMaDonVi = $("#drpDonViQuanLy").val();
    $("#btnSave").prop('disabled', false);
    var fileInput = document.getElementById('FileUpload');
    var file = fileInput.files[0];
    var formData = new FormData();
    formData.append('file', file);
    formData.append('iIdDonVi', sMaDonVi);
    if (sMaDonVi == null || sMaDonVi == "") {
        alert("Chưa chọn đơn vị !");
        return;
    }
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKhoiTaoThongTinDuAn/LoadDataExcel",
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
        strHtml.push("<td style='text-align:center'>" + obj.sMaDuAn + "</td>");
        strHtml.push("<td>" + obj.sTenDuAn + "</td>");
        strHtml.push("<td style='text-align:center'>" + obj.sMaLoaiCongTrinh + "</td>");
        strHtml.push("<td style='text-align:right'>" + obj.fKHVN_VonBoTriHetNamTruoc + "</td>");
        strHtml.push("<td style='text-align:right'>" + obj.fKHVN_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc + "</td>");
        strHtml.push("<td style='text-align:right'>" + obj.fKHVN_TrongDoVonTamUngTheoCheDoChuaThuHoi + "</td>");
        strHtml.push("<td style='text-align:right'>" + obj.fKHVN_LKVonTamUngTheoCheDoChuaThuHoiNopDieuChinhGiamDenHetNamTruoc + "</td>");
        strHtml.push("<td style='text-align:right'>" + obj.fKHVN_KeHoachVonKeoDaiSangNam + "</td>");
        strHtml.push("<td style='text-align:right'>" + obj.fKHUT_VonBoTriHetNamTruoc + "</td>");
        strHtml.push("<td style='text-align:right'>" + obj.fKHUT_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc + "</td>");
        strHtml.push("<td style='text-align:right'>" + obj.fKHUT_TrongDoVonTamUngTheoCheDoChuaThuHoi + "</td>");
        strHtml.push("<td style='text-align:right'>" + obj.fKHUT_KeHoachUngTruocKeoDaiSangNam + "</td>");
        strHtml.push("<td style='text-align:right'>" + obj.fKHUT_KeHoachUngTruocChuaThuHoi + "</td>");
        strHtml.push("</tr>");
    });
    $("#tblImport").html(strHtml.join(""));
}

function refreshImport() {
    $("#FileUpload").val(null);
    $("#tblImport").empty();
    lstData = [];
}

function OnSave() {
    $("#btnSave").prop('disabled', true);
    if (!ValidateForm()) {
        $("#btnSave").prop('disabled', false);
        return;
    }
    SaveDataImport();
}

function SaveDataImport() {
    var objKhoiTao = {};
    objKhoiTao.iNamKhoiTao = $("#iNamKhoiTao").val();
    objKhoiTao.iID_DonViID = $("#drpDonViQuanLy").val();
    objKhoiTao.dNgayKhoiTao = $("#dNgayKhoiTao").val();
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKhoiTaoThongTinDuAn/OnSaveDataImport",
        data: { objKhoiTao: objKhoiTao, lstData: lstData },
        success: function (r) {
            if (r.bIsSuccess) {
                alert("Import thành công !");
                location.href = "/QLVonDauTu/QLKhoiTaoThongTinDuAn";
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

function ValidateForm() {
    var iNamKhoiTao = $("#iNamKhoiTao").val();
    var iIdDonViQuanLy = $("#drpDonViQuanLy").val();
    var dNgayKhoiTao = $("#dNgayKhoiTao").val();
    var errors = [];
    if (iNamKhoiTao == null || iNamKhoiTao == "") {
        errors.push("Chưa chọn năm khởi tạo !");
    }
    if (iIdDonViQuanLy == null || iIdDonViQuanLy == "") {
        errors.push("Chưa chọn đơn vị !");
    }
    if (dNgayKhoiTao == null || dNgayKhoiTao == "") {
        errors.push("Chưa chọn ngày tạo !");
    }
    if (errors.length != 0) {
        alert(errors.join("\n"));
        return false;
    }
    return true;
}