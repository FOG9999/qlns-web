var DONVI_BQL = "theodonvibql";
var BQL_DONVI = "theobqldonvi";

function GetBaoCao() {
    $.ajax({
        type: "POST",
        url: "/QLNguonNganSach/BaoCaoNguonNS/TongHopQDBSDuToanNSNamTheoDonViPartial",
        data: { iNguonNganSach: $("#iID_Nguon").val(), sSoQuyetDinh: $("#txtsSoQuyetDinh").val(), dDateFrom: $("#txtTuNgay").val(), dDateTo: $("#txtDenNgay").val(), dvt: $("#dvt").val() },
        success: function (data) {
            $("#theodonvi #showData").html(data);
        }
    });

    $.ajax({
        type: "POST",
        url: "/QLNguonNganSach/BaoCaoNguonNS/TongHopQDBSDuToanNSNamTheoDonViBQLPartial",
        data: { iNguonNganSach: $("#iID_Nguon").val(), sSoQuyetDinh: $("#txtsSoQuyetDinh").val(), dDateFrom: $("#txtTuNgay").val(), dDateTo: $("#txtDenNgay").val(), dvt: $("#dvt").val() },
        success: function (data) {
            $("#" + DONVI_BQL + " #showData").html(data);
            TinhTongHangCha(DONVI_BQL);
        }
    });

    $.ajax({
        type: "POST",
        url: "/QLNguonNganSach/BaoCaoNguonNS/TongHopQDBSDuToanNSNamTheoBQLDonViPartial",
        data: { iNguonNganSach: $("#iID_Nguon").val(), sSoQuyetDinh: $("#txtsSoQuyetDinh").val(), dDateFrom: $("#txtTuNgay").val(), dDateTo: $("#txtDenNgay").val(), dvt: $("#dvt").val() },
        success: function (data) {
            $("#" + BQL_DONVI + " #showData").html(data);
            TinhTongHangCha(BQL_DONVI);
        }
    });
}

function TinhTongHangCha(div) {
    $("#" + div + " table tr.parent").each(function () {
        var iIDParent = this.dataset["iid"];
        $(this).find(".cellQD").each(function () {
            var iID_DuToan = this.dataset["iiddutoan"];
            var tong = 0;
            $("#" + div + " table tr.child[data-parent='" + iIDParent + "']").each(function () {
                tong += parseInt(UnFormatNumber($(this).find("[data-iiddutoan='" + iID_DuToan + "']").html() == "" ? "0" : $(this).find("[data-iiddutoan='" + iID_DuToan + "']").html()));
            })
            $(this).html(tong == 0 ? "" : FormatNumber(tong));
        })
    })
}

function ShowModalConfig() {
    GetSoLuongTo();
    $('#rowChonTo').show();
    $('#configBaocao').modal('show');
}

$(".btn-print").click(function () {
    var ext = $(this).data("ext");
    var dvt = $("#dvt").val();
    var tabActive = $('.tab-content .active').attr('id');
    var arrLink = [];
    $("input:checkbox[check-group='To']").each(function () {
        if (this.checked) {
            var item = this.value;

            if (tabActive == "theodonvi")
                arrLink.push("/QLNguonNganSach/BaoCaoNguonNS/ExportBaoCaoTongHopQDBSDuToanNSNam?ext=" + ext + "&dvt=" + dvt + "&to=" + item + "&iTypeBC=1");
            else if (tabActive == "theodonvibql")
                arrLink.push("/QLNguonNganSach/BaoCaoNguonNS/ExportBaoCaoTongHopQDBSDuToanNSNam?ext=" + ext + "&dvt=" + dvt + "&to=" + item + "&iTypeBC=2");
            else if (tabActive == "theobqldonvi")
                arrLink.push("/QLNguonNganSach/BaoCaoNguonNS/ExportBaoCaoTongHopQDBSDuToanNSNam?ext=" + ext + "&dvt=" + dvt + "&to=" + item + "&iTypeBC=3");
        }
    });

    openLinks(arrLink);
});

function GetSoLuongTo() {
    var tabActive = $('.tab-content .active').attr('id');
    var iTypeBc = 1;
    if (tabActive == "theodonvi")
        iTypeBc = 1;
    else if (tabActive == "theodonvibql")
        iTypeBc = 2;
    else if (tabActive == "theobqldonvi")
        iTypeBc = 3;

    var iNguonNganSach = $("#iID_Nguon").val();
    var sSoQuyetDinh = $("#txtsSoQuyetDinh").val();
    var dDateFrom = $("#txtTuNgay").val();
    var dDateTo = $("#txtDenNgay").val();
    var dvt = $("#dvt").val();
    var url = '/QLNguonNganSach/BaoCaoNguonNS/Ds_To_BaoCao6/?iNguonNganSach=' + iNguonNganSach + '&sSoQuyetDinh=' + sSoQuyetDinh +
        '&dDateFrom=' + dDateFrom + '&dDateTo=' + dDateTo + '&dvt=' + dvt + '&iTypeBc=' + iTypeBc;
    fillCheckboxList("To", "To", url);
}
