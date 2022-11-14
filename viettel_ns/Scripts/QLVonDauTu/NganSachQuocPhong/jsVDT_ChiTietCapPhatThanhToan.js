var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var tblDataGrid = [];
var TBL_DANH_SACH_THANH_TOAN_CHITIET = 'tblDanhSachThanhToanChiTiet';

var THANH_TOAN = 1;
var TAM_UNG = 0;
var THU_HOI_UNG = 2

function CancelSaveData() {
    location.href = "/QLVonDauTu/GiaiNganThanhToan";
}

function recalculateTyLeThanhToan() {
    try {
        let fGiaTriThanhToanTN = parseIntEmptyStr(UnFormatNumber($('#txtdntuVonTrongNuoc').html() ? $('#txtdntuVonTrongNuoc').html() : 0));
        let fGiaTriThanhToanNN = parseIntEmptyStr(UnFormatNumber($('#txtdntuVonNgoaiNuoc').html() ? $('#txtdntuVonNgoaiNuoc').html() : 0));
        let fGiaTriThuHoiTN = parseIntEmptyStr(UnFormatNumber($('#txtthtuUngTruocVonTrongNuoc').html() ? $('#txtthtuUngTruocVonTrongNuoc').html() : 0));
        let fGiaTriThuHoiNN = parseIntEmptyStr(UnFormatNumber($('#txtthtuUngTruocVonNgoaiNuoc').html() ? $('#txtthtuUngTruocVonNgoaiNuoc').html() : 0));
        let LuyKeThanhToanKLHTVonTN = parseIntEmptyStr(UnFormatNumber($('#txtluyKeTTTN').html()));
        let LuyKeThanhToanKLHTVonNN = parseIntEmptyStr(UnFormatNumber($('#txtluyKeTTNN').html()));
        let LuyKeTamUngChuaThuHoiVonTN = parseIntEmptyStr(UnFormatNumber($('#txtluyKeTUUngTruocTN').html()));
        let LuyKeTamUngChuaThuHoiVonNN = parseIntEmptyStr(UnFormatNumber($('#txtluyKeTUUngTruocNN').html()));
        let LuyKeTamUngChuaThuHoiVonUngTruocTN = parseIntEmptyStr(UnFormatNumber($('#txtluyKeTUTN').html()));
        let LuyKeTamUngChuaThuHoiVonUngTruocNN = parseIntEmptyStr(UnFormatNumber($('#txtluyKeTUNN').html()));
        let FGiaTriThuHoiUngTruocTn = parseIntEmptyStr(UnFormatNumber($('#txtthtuVonTrongNuoc').html() ? $('#txtthtuVonTrongNuoc').html() : 0));
        let FGiaTriThuHoiUngTruocNn = parseIntEmptyStr(UnFormatNumber($('#txtthtuVonNgoaiNuoc').html() ? $('#txtthtuVonNgoaiNuoc').html() : 0));
        let GiaTriHopDong = parseIntEmptyStr(UnFormatNumber($('#txtGiaTriHopDong').html()));

        let tyLeThanhToan = (LuyKeThanhToanKLHTVonTN + LuyKeThanhToanKLHTVonNN + LuyKeTamUngChuaThuHoiVonTN +
            LuyKeTamUngChuaThuHoiVonNN + LuyKeTamUngChuaThuHoiVonUngTruocTN + LuyKeTamUngChuaThuHoiVonUngTruocNN +
            fGiaTriThanhToanTN + fGiaTriThanhToanNN - (fGiaTriThuHoiTN + fGiaTriThuHoiNN +
                FGiaTriThuHoiUngTruocTn +
                FGiaTriThuHoiUngTruocNn)) / GiaTriHopDong;

        $('#fTyLeThanhToan').html(tyLeThanhToan.toFixed(2));
    }
    catch (e) {
        console.error(e)
    }
}

function parseIntEmptyStr(str) {
    if (str === "") {
        return 0;
    }
    else return parseInt(str);
}

$(document).ready(function () {
    recalculateTyLeThanhToan();
})