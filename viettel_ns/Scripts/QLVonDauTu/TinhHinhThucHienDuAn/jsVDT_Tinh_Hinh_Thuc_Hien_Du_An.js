var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var arrError = [];
var CONFIRM = 0;
var ERROR = 1;
function GetBaoCao() {
    if (Validate()) {
        var iID_DonViQuanLyID = $("#idDonVi").val();
        var iID_DuAn = $("#idDuAn").val();
        var iDenNgay = $("#iDenNgay").val();
        $.ajax({
            type: "POST",
            url: "/QLVonDauTu/TinhHinhThucHienDuAn/GetTinhHinhThucHienDuAn",
            data: { iID_DonViQuanLyID: iID_DonViQuanLyID, iID_DuAn: iID_DuAn, iDenNgay: iDenNgay },
            success: function (data) {
                $("#showData").html(data);
                TinhLaiDongTong();
                var sTenDonViQL = $("#idDonVi :selected").html();
                $("#id_donviquanly").html(sTenDonViQL);
                $("#contentReport").removeClass('hidden');
            }
        });
    }
    else {
        showErr(ERROR);
    }
  
}

function LoadThongTinDonVi() {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/TinhHinhThucHienDuAn/GetNSDonVi",
        success: function (result) {
            if (result) {
                $('#idDonVi').empty().html(result.htmlCT);
                $('#idDonVi').change();
            }
        }
    });
}

function LoadThongTinDuAn() {
    var iIDDonVi = $("#idDonVi").val();
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/TinhHinhThucHienDuAn/GetDuAnTheoDonVi",
        data: { iIDDonVi: iIDDonVi },
        success: function (result) {
            $('#idDuAn').empty().html(result.htmlCT);
        }
    });
}


function exportTinhHinhThucHienDuAn() {
    if (Validate()) {
        var iID_DonViQuanLyID = $("#idDonVi").val();
        var iID_DuAn = $("#idDuAn").val();
        var iDenNgay = $("#iDenNgay").val();
        window.location.href = "/QLVonDauTu/TinhHinhThucHienDuAn/ExportExcelTinhHinhThucHienDuAn?iID_DonViQuanLyID=" + iID_DonViQuanLyID + '&iID_DuAn=' + iID_DuAn + '&iDenNgay=' + iDenNgay;
    }
    else {
        showErr(ERROR);
    }
   
}

function TinhLaiDongTong() {
    var fTongGiaTriHopDong = 0;
    var fTongSoThanhToan = 0;
    var fTongSoTamUng = 0;
    var fTongSoThuHoiTamUng = 0;
    var fTongGiaiNgan = 0;
    var fTongSoDaCapUng = 0;

    $("#baocaoTHTHDA" + " tbody tr").each(function (index, row) {
        var r_fTienHopDong = $(row).find(".r_fTienHopDong").html();
        if (r_fTienHopDong != undefined && $.isNumeric(UnFormatNumber(r_fTienHopDong))) {
            fTongGiaTriHopDong += parseFloat(UnFormatNumber(r_fTienHopDong));
        }

        var r_fSoThanhToan = $(row).find(".r_fSoThanhToan").html();
        if (r_fSoThanhToan != undefined && $.isNumeric(UnFormatNumber(r_fSoThanhToan))) {
            fTongSoThanhToan += parseFloat(UnFormatNumber(r_fSoThanhToan));
        }

        var r_fSoTamUng = $(row).find(".r_fSoTamUng").html();
        if (r_fSoTamUng != undefined && $.isNumeric(UnFormatNumber(r_fSoTamUng))) {
            fTongSoTamUng += parseFloat(UnFormatNumber(r_fSoTamUng));
        }

        var r_fThuHoiTamUng = $(row).find(".r_fThuHoiTamUng").html();
        if (r_fThuHoiTamUng != undefined && $.isNumeric(UnFormatNumber(r_fThuHoiTamUng))) {
            fTongSoThuHoiTamUng += parseFloat(UnFormatNumber(r_fThuHoiTamUng));
        }

        var r_fTongCongGiaiNgan = $(row).find(".r_fTongCongGiaiNgan").html();
        if (r_fTongCongGiaiNgan != undefined && $.isNumeric(UnFormatNumber(r_fTongCongGiaiNgan))) {
            fTongGiaiNgan += parseFloat(UnFormatNumber(r_fTongCongGiaiNgan));
        }

        var r_fSoDaCapUng = $(row).find(".r_fSoDaCapUng").html();
        if (r_fSoDaCapUng != undefined && $.isNumeric(UnFormatNumber(r_fSoDaCapUng))) {
            fTongSoDaCapUng += parseFloat(UnFormatNumber(r_fSoDaCapUng));
        }

       
    })

    fTongGiaTriHopDong = fTongGiaTriHopDong == 0 ? "" : fTongGiaTriHopDong;
    fTongSoThanhToan = fTongSoThanhToan == 0 ? "" : fTongSoThanhToan;
    fTongSoTamUng = fTongSoTamUng == 0 ? "" : fTongSoTamUng;
    fTongSoThuHoiTamUng = fTongSoThuHoiTamUng == 0 ? "" : fTongSoThuHoiTamUng;
    fTongGiaiNgan = fTongGiaiNgan == 0 ? "" : fTongGiaiNgan;
    fTongSoDaCapUng = fTongSoDaCapUng == 0 ? "" : fTongSoDaCapUng;


    $("#baocaoTHTHDA" + " .fTongGiaTriHopDong").html(fTongGiaTriHopDong.toLocaleString('vi-VN'));
    $("#baocaoTHTHDA" + " .fTongSoThanhToan").html(fTongSoThanhToan.toLocaleString('vi-VN'));
    $("#baocaoTHTHDA" + " .fTongSoThanhToan").html(fTongSoThanhToan.toLocaleString('vi-VN'));
    $("#baocaoTHTHDA" + " .fTongSoThuHoiTamUng").html(fTongSoThuHoiTamUng.toLocaleString('vi-VN'));
    $("#baocaoTHTHDA" + " .fTongGiaiNgan").html(fTongGiaiNgan.toLocaleString('vi-VN'));
    $("#baocaoTHTHDA" + " .fTongSoDaCapUng").html(fTongSoDaCapUng.toLocaleString('vi-VN'));
   
}

function Validate() {
    var result = true;
    if ($("#idDonVi").val() == null || $("#idDonVi").val() == "") {
        arrError.push("Vui lòng chọn đơn vị quản lý");
        result = false;
    } else if ($("#idDuAn").val() == null || $("#idDuAn").val() == "") {
        arrError.push("Vui lòng chọn dự án");
        result = false;
    }
    else { }
    return result;
}

function showErr(type) {
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: "Thông báo", Messages: arrError, Category: type },
        success: function (data) {
            $("#divModalConfirm").html(data);
            arrError = [];
        }
    });
}