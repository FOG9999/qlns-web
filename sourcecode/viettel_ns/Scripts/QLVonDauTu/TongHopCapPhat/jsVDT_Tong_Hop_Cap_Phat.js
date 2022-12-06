var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var arrError = [];
var CONFIRM = 0;
var ERROR = 1;

function LoadThongTinDonVi() {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/TongHopCapPhat/GetNSDonVi",
        success: function (result) {
            if (result) {
                $('#idDonVi').empty().html(result.htmlCT);
            }
        }
    });
}

function GetBaoCao() {
    if (Validate()) {
        var iID_DonViQuanLyID = $("#idDonVi").val();
        var iIdMaDonViQuanLy = "";
        if (iID_DonViQuanLyID != null && iID_DonViQuanLyID != "") {
            iIdMaDonViQuanLy = iID_DonViQuanLyID.split("|")[0];
        }
        var iIDNguonNganSach = $("#iIDNguonNganSach").val();
        var iNamKeHoach = $("#iNamKeHoach").val();
        $.ajax({
            type: "POST",
            url: "/QLVonDauTu/TongHopCapPhat/GetThongTinBaoCaoCapPhat",
            data: { iID_DonViQuanLyID: iIdMaDonViQuanLy, iIDNguonNganSach: iIDNguonNganSach, iNamKeHoach: iNamKeHoach },
            success: function (data) {
                $("#showData").html(data);
                var sTenNganSach = $("#iIDNguonNganSach :selected").html();
                var arrTen = sTenNganSach.split('.');
                $("#idngansach").html(arrTen[1]);
                TinhLaiDongTong();
                $("#contentReport").removeClass('hidden');
            }
        });
    }
    else {
        showErr(ERROR);
    }
   
}

function TinhLaiDongTong() {
    var fTongCapTaiKhoBac_DT = 0;
    var fTongCapBangLenhChi_DT = 0;
    var fTong_VU = 0;
    var fTongCapTaiKhoBac_VU = 0;
    var fTongCapBangLenhChi_VU = 0;
    var fTongKhac_Vu = 0;
    var fTongKLHT = 0;
    var fTongTamUng = 0;
    var fTongThuHoiTamUng = 0;



    $("#baocaoTHCAPPHAT" + " tbody tr").each(function (index, row) {
        var r_fCapBacTaiKhoBac_DTNN = $(row).find(".r_fCapBacTaiKhoBac_DTNN").html();
        if (r_fCapBacTaiKhoBac_DTNN != undefined && $.isNumeric(UnFormatNumber(r_fCapBacTaiKhoBac_DTNN))) {
            fTongCapTaiKhoBac_DT += parseFloat(UnFormatNumber(r_fCapBacTaiKhoBac_DTNN));
        }

        var r_fCapBangLenhChi_DTNN = $(row).find(".r_fCapBangLenhChi_DTNN").html();
        if (r_fCapBangLenhChi_DTNN != undefined && $.isNumeric(UnFormatNumber(r_fCapBangLenhChi_DTNN))) {
            fTongCapBangLenhChi_DT += parseFloat(UnFormatNumber(r_fCapBangLenhChi_DTNN));
        }

        var r_fTong_VUNCT = $(row).find(".r_fTong_VUNCT").html();
        if (r_fTong_VUNCT != undefined && $.isNumeric(UnFormatNumber(r_fTong_VUNCT))) {
            fTong_VU += parseFloat(UnFormatNumber(r_fTong_VUNCT));
        }

        var r_fCapBacTaiKhoBac_VUNCT = $(row).find(".r_fCapBacTaiKhoBac_VUNCT").html();
        if (r_fCapBacTaiKhoBac_VUNCT != undefined && $.isNumeric(UnFormatNumber(r_fCapBacTaiKhoBac_VUNCT))) {
            fTongCapTaiKhoBac_VU += parseFloat(UnFormatNumber(r_fCapBacTaiKhoBac_VUNCT));
        }

        var r_fCapBangLenhChi_VUNCT = $(row).find(".r_fCapBangLenhChi_VUNCT").html();
        if (r_fCapBangLenhChi_VUNCT != undefined && $.isNumeric(UnFormatNumber(r_fCapBangLenhChi_VUNCT))) {
            fTongCapBangLenhChi_VU += parseFloat(UnFormatNumber(r_fCapBangLenhChi_VUNCT));
        }

        var r_fThanhToanKLHT = $(row).find(".r_fThanhToanKLHT").html();
        if (r_fThanhToanKLHT != undefined && $.isNumeric(UnFormatNumber(r_fThanhToanKLHT))) {
            fTongKLHT += parseFloat(UnFormatNumber(r_fThanhToanKLHT));
        }

        var r_fTamUng = $(row).find(".r_fTamUng").html();
        if (r_fTamUng != undefined && $.isNumeric(UnFormatNumber(r_fTamUng))) {
            fTongTamUng += parseFloat(UnFormatNumber(r_fTamUng));
        }

        var r_fThuHoiTamUng = $(row).find(".r_fThuHoiTamUng").html();
        if (r_fThuHoiTamUng != undefined && $.isNumeric(UnFormatNumber(r_fThuHoiTamUng))) {
            fTongThuHoiTamUng += parseFloat(UnFormatNumber(r_fThuHoiTamUng));
        }

     
    })

    fTongCapTaiKhoBac_DT = fTongCapTaiKhoBac_DT == 0 ? "" : fTongCapTaiKhoBac_DT;
    fTongCapBangLenhChi_DT = fTongCapBangLenhChi_DT == 0 ? "" : fTongCapBangLenhChi_DT;
    fTong_VU = fTong_VU == 0 ? "" : fTong_VU;
    fTongCapTaiKhoBac_VU = fTongCapTaiKhoBac_VU == 0 ? "" : fTongCapTaiKhoBac_VU;
    fTongCapBangLenhChi_VU = fTongCapBangLenhChi_VU == 0 ? "" : fTongCapBangLenhChi_VU;
    fTongKhac_Vu = fTongKhac_Vu == 0 ? "" : fTongKhac_Vu;
    fTongKLHT = fTongKLHT == 0 ? "" : fTongKLHT;
    fTongTamUng = fTongTamUng == 0 ? "" : fTongTamUng;
    fTongThuHoiTamUng = fTongThuHoiTamUng == 0 ? "" : fTongThuHoiTamUng;

    $("#baocaoTHCAPPHAT" + " .fTongCapTaiKhoBac_DT").html(fTongCapTaiKhoBac_DT.toLocaleString('vi-VN'));
    $("#baocaoTHCAPPHAT" + " .fTongCapBangLenhChi_DT").html(fTongCapBangLenhChi_DT.toLocaleString('vi-VN'));
    $("#baocaoTHCAPPHAT" + " .fTong_VU").html(fTong_VU.toLocaleString('vi-VN'));
    $("#baocaoTHCAPPHAT" + " .fTongCapTaiKhoBac_VU").html(fTongCapTaiKhoBac_VU.toLocaleString('vi-VN'));
    $("#baocaoTHCAPPHAT" + " .fTongCapBangLenhChi_VU").html(fTongCapBangLenhChi_VU.toLocaleString('vi-VN'));
    $("#baocaoTHCAPPHAT" + " .fTongKhac_Vu").html(fTongKhac_Vu.toLocaleString('vi-VN'));
    $("#baocaoTHCAPPHAT" + " .fTongKLHT").html(fTongKLHT.toLocaleString('vi-VN'));
    $("#baocaoTHCAPPHAT" + " .fTongTamUng").html(fTongTamUng.toLocaleString('vi-VN'));
    $("#baocaoTHCAPPHAT" + " .fTongThuHoiTamUng").html(fTongThuHoiTamUng.toLocaleString('vi-VN'));
}

function exportBCKetQuaGiaiNganVonDauTuNguonVonNSQP() {
    if (Validate()) {
        var iID_DonViQuanLyID = $("#idDonVi").val();
        var iIDNguonNganSach = $("#iIDNguonNganSach").val();
        var iNamKeHoach = $("#iNamKeHoach").val();
        var sTenNganSach = $("#iIDNguonNganSach :selected").html();
        var arrTen = sTenNganSach.split('.');
        sTenNganSach = arrTen[1];
        window.location.href = "/QLVonDauTu/TongHopCapPhat/ExportExcelBaoCaoCapPhat?iID_DonViQuanLyID=" + iID_DonViQuanLyID + '&iIDNguonNganSach=' + iIDNguonNganSach + '&iNamKeHoach=' + iNamKeHoach + '&sTenNganSach=' + sTenNganSach;
    }
    else {
        showErr(ERROR);
    }
 
}

function Validate() {
    var result = true;
    if ($("#iNamKeHoach").val() == "") {
        arrError.push("Nhập năm kế hoạch")
        result = false;
    }
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