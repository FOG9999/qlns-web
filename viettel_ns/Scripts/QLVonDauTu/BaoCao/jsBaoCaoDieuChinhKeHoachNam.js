$(document).ready(function () {

    $("#txtNamThucHien").keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            return false;
        }
    });

});

function GetBaoCao() {
    $("#btnGetBaoCao").attr("disabled", "disabled");
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/BaoCaoDieuChinhKeHoach/LayBaoCaoDieuChinhKeHoach",
        data: { iID_NguonVon: $("#iID_NguonVon").val(), sLNS: $("#sLNS_LoaiNguonVon").val(), iNamThucHien: $("#txtNamThucHien").val() },
        success: function (data) {
            $("#showData").html(data);
            /*tinhTong();*/
            $("#contentReport").removeClass('hidden');
            $("#btnGetBaoCao").removeAttr("disabled");
        }
    });
}