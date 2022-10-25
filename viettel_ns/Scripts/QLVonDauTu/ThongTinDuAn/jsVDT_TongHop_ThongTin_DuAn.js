var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var arrError = [];
var CONFIRM = 0;
var ERROR = 1;
function GetBaoCao() {
    if (Validate()) {
        var iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
        var iNamKeHoach = $("#iNamKeHoach").val();
        $.ajax({
            type: "POST",
            url: "/QLVonDauTu/TongHopThongTinDuAn/GetThongTinTongHopDuAn",
            data: { iID_DonViQuanLyID: iID_DonViQuanLyID, iNamKeHoach: iNamKeHoach },
            success: function (data) {
                $("#showData").html(data);
                TinhLaiDongTong();
                var sTenDonViQL = $("#iID_DonViQuanLyID :selected").html();
                $("#id_donviquanly").html(sTenDonViQL);
                $("#contentReport").removeClass('hidden');
            }
        });
    }
    else {
        showErr(ERROR);
    }
   
}

function TinhLaiDongTong() {
    var tong_fNganSachQP_PD = 0;
    var tong_fNganSachKhac_PD = 0;
    var tong_fNganSachQP_QD = 0;
    var tong_fNganSachKhac_QD = 0;
    var tong_LuyKeVonNamTruoc = 0;
    var tong_KeHoachVonNamNay = 0;
    var tong_LuyKeVonNamNay = 0;
    var tong_DaThanhToan = 0;
    var tong_ChuaThanhToan = 0;
    var tong_GiaTriQuyetToan = 0;
    var tong_fKHTH = 0;
    var tong_fChenhLech = 0;


    $("#baocaoTHThongTinDuAn" + " tbody tr").each(function (index, row) {
        var r_fNganSachQPCCDT = $(row).find(".r_fNganSachQPCCDT").html();
        if (r_fNganSachQPCCDT != undefined && $.isNumeric(UnFormatNumber(r_fNganSachQPCCDT))) {
            tong_fNganSachQP_PD += parseFloat(UnFormatNumber(r_fNganSachQPCCDT));
        }
        
        var fNganSachKhacCCDT = $(row).find(".fNganSachKhacCCDT").html();
        if (fNganSachKhacCCDT != undefined && $.isNumeric(UnFormatNumber(fNganSachKhacCCDT))) {
            tong_fNganSachKhac_PD += parseFloat(UnFormatNumber(fNganSachKhacCCDT));
        }

        var r_fNganSachQPQDDT = $(row).find(".r_fNganSachQPQDDT").html();
        if (r_fNganSachQPQDDT != undefined && $.isNumeric(UnFormatNumber(r_fNganSachQPQDDT))) {
            tong_fNganSachQP_QD += parseFloat(UnFormatNumber(r_fNganSachQPQDDT));
        }

        var r_fNganSachKhacQDDT = $(row).find(".r_fNganSachKhacQDDT").html();
        if (r_fNganSachKhacQDDT != undefined && $.isNumeric(UnFormatNumber(r_fNganSachKhacQDDT))) {
            tong_fNganSachKhac_QD += parseFloat(UnFormatNumber(r_fNganSachKhacQDDT));
        }

        var r_fKHTTDPD = $(row).find(".r_fKHTTDPD").html();
        if (r_fKHTTDPD != undefined && $.isNumeric(UnFormatNumber(r_fKHTTDPD))) {
            tong_fKHTH += parseFloat(UnFormatNumber(r_fKHTTDPD));
        }

        var r_fLuyKeVonNamTruoc = $(row).find(".r_fLuyKeVonNamTruoc").html();
        if (r_fLuyKeVonNamTruoc != undefined && $.isNumeric(UnFormatNumber(r_fLuyKeVonNamTruoc))) {
            tong_LuyKeVonNamTruoc += parseFloat(UnFormatNumber(r_fLuyKeVonNamTruoc));
        }

        var r_fKeHoachVonNamNay = $(row).find(".r_fKeHoachVonNamNay").html();
        if (r_fKeHoachVonNamNay != undefined && $.isNumeric(UnFormatNumber(r_fKeHoachVonNamNay))) {
            r_fKeHoachVonNamNay += parseFloat(UnFormatNumber(r_fKeHoachVonNamNay));
        }

        var r_fLuyKeVonNamNay = $(row).find(".r_fLuyKeVonNamNay").html();
        if (r_fLuyKeVonNamNay != undefined && $.isNumeric(UnFormatNumber(r_fLuyKeVonNamNay))) {
            tong_LuyKeVonNamNay += parseFloat(UnFormatNumber(r_fLuyKeVonNamNay));
        }

        var r_fDaThanhToan = $(row).find(".r_fDaThanhToan").html();
        if (r_fDaThanhToan != undefined && $.isNumeric(UnFormatNumber(r_fDaThanhToan))) {
            tong_DaThanhToan += parseFloat(UnFormatNumber(r_fDaThanhToan));
        }

        var r_fChuaThanhToan = $(row).find(".r_fChuaThanhToan").html();
        if (r_fChuaThanhToan != undefined && $.isNumeric(UnFormatNumber(r_fChuaThanhToan))) {
            tong_ChuaThanhToan += parseFloat(UnFormatNumber(r_fChuaThanhToan));
        }

        var r_fGiaTriQuyetToan = $(row).find(".r_fGiaTriQuyetToan").html();
        if (r_fGiaTriQuyetToan != undefined && $.isNumeric(UnFormatNumber(r_fGiaTriQuyetToan))) {
            tong_GiaTriQuyetToan += parseFloat(UnFormatNumber(r_fGiaTriQuyetToan));
        }

        var r_fChenhLech = $(row).find(".r_fChenhLech").html();
        if (r_fChenhLech != undefined && $.isNumeric(UnFormatNumber(r_fChenhLech))) {
            tong_fChenhLech += parseFloat(UnFormatNumber(r_fChenhLech));
        }
    })

    tong_fNganSachQP_PD = tong_fNganSachQP_PD == 0 ? "" : tong_fNganSachQP_PD;
    tong_fNganSachKhac_PD = tong_fNganSachKhac_PD == 0 ? "" : tong_fNganSachKhac_PD;
    tong_fNganSachQP_QD = tong_fNganSachQP_QD == 0 ? "" : tong_fNganSachQP_QD;
    tong_fNganSachKhac_QD = tong_fNganSachKhac_QD == 0 ? "" : tong_fNganSachKhac_QD;
    tong_LuyKeVonNamTruoc = tong_LuyKeVonNamTruoc == 0 ? "" : tong_LuyKeVonNamTruoc;
    tong_KeHoachVonNamNay = tong_KeHoachVonNamNay == 0 ? "" : tong_KeHoachVonNamNay;
    tong_LuyKeVonNamNay = tong_LuyKeVonNamNay == 0 ? "" : tong_LuyKeVonNamNay;
    tong_DaThanhToan = tong_DaThanhToan == 0 ? "" : tong_DaThanhToan;
    tong_ChuaThanhToan = tong_ChuaThanhToan == 0 ? "" : tong_ChuaThanhToan;
    tong_GiaTriQuyetToan = tong_GiaTriQuyetToan == 0 ? "" : tong_GiaTriQuyetToan;
    tong_fKHTH = tong_fKHTH == 0 ? "" : tong_fKHTH;
    tong_fChenhLech = tong_fChenhLech == 0 ? "" : tong_fChenhLech;

    $("#baocaoTHThongTinDuAn" + " .tong_fNganSachQP_PD").html(tong_fNganSachQP_PD.toLocaleString('vi-VN'));
    $("#baocaoTHThongTinDuAn" + " .tong_fNganSachKhac_PD").html(tong_fNganSachKhac_PD.toLocaleString('vi-VN'));
    $("#baocaoTHThongTinDuAn" + " .tong_fNganSachQP_QD").html(tong_fNganSachQP_QD.toLocaleString('vi-VN'));
    $("#baocaoTHThongTinDuAn" + " .tong_fNganSachKhac_QD").html(tong_fNganSachKhac_QD.toLocaleString('vi-VN'));
    $("#baocaoTHThongTinDuAn" + " .tong_LuyKeVonNamTruoc").html(tong_LuyKeVonNamTruoc.toLocaleString('vi-VN'));
    $("#baocaoTHThongTinDuAn" + " .tong_KeHoachVonNamNay").html(tong_KeHoachVonNamNay.toLocaleString('vi-VN'));
    $("#baocaoTHThongTinDuAn" + " .tong_LuyKeVonNamNay").html(tong_LuyKeVonNamNay.toLocaleString('vi-VN'));
    $("#baocaoTHThongTinDuAn" + " .tong_DaThanhToan").html(tong_DaThanhToan.toLocaleString('vi-VN'));
    $("#baocaoTHThongTinDuAn" + " .tong_ChuaThanhToan").html(tong_ChuaThanhToan.toLocaleString('vi-VN'));
    $("#baocaoTHThongTinDuAn" + " .tong_GiaTriQuyetToan").html(tong_GiaTriQuyetToan.toLocaleString('vi-VN'));
    $("#baocaoTHThongTinDuAn" + " .tong_fKHTH").html(tong_fKHTH.toLocaleString('vi-VN'));
    $("#baocaoTHThongTinDuAn" + " .tong_ChenhLech").html(tong_fChenhLech.toLocaleString('vi-VN'));

}

function getDataReport() {
    var count = $("#showData tbody").children().length;
    console.log(count);
    var dataTongReport = [];
    var dataReport = [];

    $("#baocaoTHThongTinDuAn" + " tbody tr").each(function (indexR, row) {
        var lstChild = [];
        var ttDuAn = {};
        if (indexR < 2) {
            $(row).find("td").each(function (index) {
                switch (index) {
                    case 0:
                        ttDuAn.stt = $(this).text();
                        break;
                    case 1:
                        ttDuAn.sTenDuAn = $(this).text();
                        break;
                    case 2:
                        ttDuAn.sKhoiCong = $(this).text();
                        ttDuAn.sKetThuc = $(this).text();
                        break;
                    case 3:
                        ttDuAn.sSoQuyetDinhCTDT = $(this).text();
                        break;
                    case 4:
                        ttDuAn.dNgayQuyetDinhCTDT = $(this).text();
                        break;
                    case 5:
  
                        ttDuAn.fNganSachQPCCDT = UnFormatNumber($(this).text());
                        break;
                    case 6:
                        ttDuAn.fNganSachKhacCCDT = UnFormatNumber($(this).text());
                        break;
                    case 7:
                        ttDuAn.sSoQuyetDinhQDDT = $(this).text();
                        break;
                    case 8:
                        ttDuAn.dNgayQuyetDinhQDDT = $(this).text();
                        break;
                    case 9:
                        ttDuAn.fNganSachQPQDDT = UnFormatNumber($(this).text());
                        break;
                    case 10:
                        ttDuAn.fNganSachKhacQDDT = UnFormatNumber($(this).text());
                        break;
                    case 11:
                        ttDuAn.fKHTTDPD = UnFormatNumber($(this).text());
                        break;
                    case 12:
                        ttDuAn.fLuyKeVonNamTruoc = UnFormatNumber($(this).text());
                        break;
                    case 13:
                        ttDuAn.fKeHoachVonNamNay = UnFormatNumber($(this).text());
                        break;
                    case 14:
                        ttDuAn.fLuyKeVonNamNay = UnFormatNumber($(this).text());
                        break;
                    case 15:
                        ttDuAn.fDaThanhToan = UnFormatNumber($(this).text());
                        break;
                    case 16:
                        ttDuAn.fChuaThanhToan = UnFormatNumber($(this).text());
                        break;
                    case 17:
                        ttDuAn.sSoQuyetDinhQT = UnFormatNumber($(this).text());
                        break;
                    case 18:
                        ttDuAn.dNgayQuyetDinhQT = UnFormatNumber($(this).text());
                        break;
                    case 19:
                        ttDuAn.fGiaTriQuyetToan = UnFormatNumber($(this).text());
                        break;
                    default:

                }

                lstChild.push($(this).text());
            });
            dataReport.push(ttDuAn);
        }
    });
    console.log(dataReport);

    exportBCTongHopThongTinDuAn(dataReport);
}

function exportBCTongHopThongTinDuAn(dataReport) {
    if (Validate()) {
        $.ajax({
            type: "POST",
            url: "/QLVonDauTu/TongHopThongTinDuAn/ExportBCTongHopTTDuAn",
            data: { dataReport: dataReport },
            success: function (r) {
                if (r) {
                    var iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
                    var iNamKeHoach = $("#iNamKeHoach").val();
                    window.location.href = "/QLVonDauTu/TongHopThongTinDuAn/ExportExcel?iID_DonViQuanLyID=" + iID_DonViQuanLyID + '&iNamKeHoach=' + iNamKeHoach;
                }
            }
        });
    }
    else {
        showErr(ERROR);
    }
    
}

function Validate() {
    var result = true;
    if ($("#iID_DonViQuanLyID").val() == "") {
        arrError.push("Vui lòng chọn đơn vị quản lý");
        result = false;
    } else if ($("#iNamKeHoach").val() == "") {
        arrError.push("Vui lòng nhập năm kế hoạch");
        result = false;
    }
    else {

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