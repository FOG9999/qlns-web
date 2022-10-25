var DONVI_BQL = "theodonvibql";
var BQL_DONVI = "theobqldonvi";

function GetBaoCao() {
    $.ajax({
        type: "POST",
        url: "/QLNguonNganSach/BaoCaoNguonNS/TongHopGiaoDuToanNSTheoDonViPartial",
        data: { iNguonNganSach: $("#iID_Nguon").val(), sSoQuyetDinh: $("#txtsSoQuyetDinh").val(), dDateFrom: $("#txtTuNgay").val(), dDateTo: $("#txtDenNgay").val(), dvt: $("#dvt").val() },
        success: function (data) {
            $("#theodonvi #showData").html(data);
        }
    });

    $.ajax({
        type: "POST",
        url: "/QLNguonNganSach/BaoCaoNguonNS/TongHopGiaoDuToanNSTheoDonViBQLPartial",
        data: { iNguonNganSach: $("#iID_Nguon").val(), sSoQuyetDinh: $("#txtsSoQuyetDinh").val(), dDateFrom: $("#txtTuNgay").val(), dDateTo: $("#txtDenNgay").val(), dvt: $("#dvt").val() },
        success: function (data) {
            $("#" + DONVI_BQL + " #showData").html(data);
            TinhTongHangCha(DONVI_BQL);
        }
    });

    $.ajax({
        type: "POST",
        url: "/QLNguonNganSach/BaoCaoNguonNS/TongHopGiaoDuToanNSTheoBQLDonViPartial",
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
        var tongDaGiao = 0, tongBoSung = 0, tongDT = 0;
        $("#" + div + " table tr.child[data-parent='" + iIDParent + "']").each(function () {
            tongDaGiao += $(this).find(".dagiao").html() == "" ? 0 : parseInt(UnFormatNumber($(this).find(".dagiao").html()));
            tongBoSung += $(this).find(".bosung").html() == "" ? 0 : parseInt(UnFormatNumber($(this).find(".bosung").html()));
            tongDT += $(this).find(".tongdt").html() == "" ? 0 : parseInt(UnFormatNumber($(this).find(".tongdt").html()));
        })
        $(this).find(".dagiao").html(tongDaGiao == 0 ? "" : FormatNumber(tongDaGiao));
        $(this).find(".bosung").html(tongBoSung == 0 ? "" : FormatNumber(tongBoSung));
        $(this).find(".tongdt").html(tongDT == 0 ? "" : FormatNumber(tongDT));
    })
}

$(".btn-print").click(function () {
    var ext = $(this).data("ext");
    var dvt = $("#dvt").val();
    var arrLink = [];
    var tabActive = $('.tab-content .active').attr('id');
    if (tabActive == "theodonvi")
        arrLink.push("/QLNguonNganSach/BaoCaoNguonNS/XuatFileTongHopGiaoDuToanNSTheoDonVi?ext=" + ext + "&dvt=" + dvt);
    else if (tabActive == "theodonvibql")
        arrLink.push("/QLNguonNganSach/BaoCaoNguonNS/XuatFileTongHopGiaoDuToanNSTheoDonViBQL?ext=" + ext + "&dvt=" + dvt);
    else if (tabActive == "theobqldonvi")
        arrLink.push("/QLNguonNganSach/BaoCaoNguonNS/XuatFileTongHopGiaoDuToanNSTheoBQLDonVi?ext=" + ext + "&dvt=" + dvt);

    openLinks(arrLink);
});



