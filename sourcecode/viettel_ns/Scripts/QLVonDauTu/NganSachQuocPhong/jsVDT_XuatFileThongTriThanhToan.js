var id;

$(document).ready(function () {
    id = $("#idInput").val();

});

function Huy() {
    window.location.href = "/QLVonDauTu/QLThongTriThanhToan/Index";
}

function InBaoCao() {
    var id = $("#idInput").val();
    var sTieuDeMot = $("#sTieuDeMot").val();
    var sTieuDeHai = $("#sTieuDeHai").val();
    var iDonViTinh = $("#iDonViTinh").val();

    window.location.href = `/QLVonDauTu/QLThongTriThanhToan/ExportReportPDF?id=${id}&sTieuDeMot=${sTieuDeMot}&sTieuDeHai=${sTieuDeHai}&iDonViTinh=${iDonViTinh}`;

}