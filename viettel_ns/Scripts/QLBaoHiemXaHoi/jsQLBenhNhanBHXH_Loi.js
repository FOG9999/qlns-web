//#region Save data file XML to database
function GoBack() {
    window.location.href = "/BaoHiemXaHoi/QLBenhNhanBHXH";
}



function GetDataTemp(iCurrentPage = 1) {
    $.ajax({
        type: "POST",
        url: "/BaoHiemXaHoi/QLBenhNhanBHXH/LoadDataImportError",
        data: { lstImportId: lstImportId, iPage: iCurrentPage },
        success: function (data) {
            if (data.bIsComplete) {
                RenderDataTable(data.lstDataTable, data.lstError);
            }
        }
    });
}

function RenderDataTable(lstDanhSach, lstError) {
    var htmlTableSai = [];
    lstDanhSach.forEach(function (value, index) {
        if (value.bError) {
            var currentErros = $.map(lstError.filter(obj => obj.iLine == value.iLine && obj.iID_ImportID == value.iID_ImportID), function (obj) { return obj.sPropertyName; });

            var txt = "<tr id='" + (value.iLine + "_" + value.iID_ImportID) + "'>"
                + "<td align='center' class='iSTT " + (currentErros.indexOf("STT") != -1 ? "error" : "") + "'>" + (value.sSTT == null ? "" : value.sSTT) + "</td>"
                + "<td align='left' class='sTen " + (currentErros.indexOf("ho_ten") != -1 ? "error" : "") + "'>" + (value.sHoTen == null ? "" : value.sHoTen) + "</td>"
                + "<td align='center' class='sNgaySinh " + (currentErros.indexOf("NGAYSINH") != -1 ? "error" : "") + "'>" + (value.sNgaySinh == null ? "" : value.sNgaySinh) + "</td>"
                + "<td align='center' class='sGioiTinh " + (currentErros.indexOf("GIOITINH") != -1 ? "error" : "") + "'>" + (value.sGioiTinh == null ? "" : value.sGioiTinh) + "</td>"
                + "<td align='left' class='sCapBac " + (currentErros.indexOf("CAPBAC") != -1 ? "error" : "") + "'>" + (value.sCapBac == null ? "" : value.sCapBac) + "</td>"
                + "<td align='left' class='sMaThe " + (currentErros.indexOf("MATHE") != -1 ? "error" : "") + "'>" + (value.sMaThe == null ? "" : value.sMaThe) + "</td>"
                + "<td align='center' class='sNgayVaoVien " + (currentErros.indexOf("NGAY_VAO_VIEN") != -1 ? "error" : "") + "'>" + (value.sNgayVaoVien == null ? "" : value.sNgayVaoVien) + "</td>"
                + "<td align='center' class='sNgayRaVien " + (currentErros.indexOf("NGAY_RA_VIEN") != -1 ? "error" : "") + "'>" + (value.sNgayRaVien == null ? "" : value.sNgayRaVien) + "</td>"
                + "<td align='center' class='sSoNgayDieuTri " + (currentErros.indexOf("SO_NGAY_DTRI") != -1 ? "error" : "") + "'>" + (value.sSoNgayDieuTri == null ? "" : value.sSoNgayDieuTri) + "</td>"
                + "<td align='left' class='sMaBV " + (currentErros.indexOf("MABV") != -1 ? "error" : "") + "'>" + (value.sMaBV == null ? "" : value.sMaBV) + "</td>"
                + "<td align='left' class='sTenBenhVien " + (currentErros.indexOf("TENBV") != -1 ? "error" : "") + "'>" + (value.sTenBV == null ? "" : value.sTenBV) + "</td>"
                + "<td align='left' class='sMaDV " + (currentErros.indexOf("MADV") != -1 ? "error" : "") + "'>" + (value.sMaDV == null ? "" : value.sMaDV) + "</td>"
                + "<td align='left' class='sTenDonVi " + (currentErros.indexOf("TENDV") != -1 ? "error" : "") + "'>" + (value.sTenDonVi == null ? "" : value.sTenDonVi) + "</td>"
                + "<td align='left' class='sGhiChu " + (currentErros.indexOf("GHICHU") != -1 ? "error" : "") + "'>" + (value.sGhiChu == null ? "" : value.sGhiChu) + "</td>"
                + "</tr>";
            htmlTableSai.push(txt);
        }
    });
    $("#tBodyLoi").append(htmlTableSai.join());
}
