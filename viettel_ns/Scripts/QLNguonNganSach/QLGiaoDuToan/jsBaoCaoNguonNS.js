function GetBaoCao() {
    $("#btnGetBaoCao").attr("disabled", "disabled");
    $.ajax({
        type: "POST",
        url: "/QLNguonNganSach/BaoCaoNguonNS/BaoCaoTongHopNguonPartial",
        data: { iNguonNganSach: $("#iID_Nguon").val(), dDateFrom: $("#txtTuNgay").val(), dDateTo: $("#txtDenNgay").val(), dvt: $("#dvt").val() },
        success: function (data) {
            $("#showData").html(data);
            tinhTong();
            $("#contentReport").removeClass('hidden');
            $("#btnGetBaoCao").removeAttr("disabled");
        }
    });
}

function tinhTong() {
    var soCotBoSung = parseInt($('#soCotBoSung').val());
    var soCotDaGiao = parseInt($('#SoCotDuToanDaGiao').val());
    var classTong = [];
    var classColumBoSung = [];
    for (var i = 0; i < soCotBoSung + soCotDaGiao + 13; i++) {
        var cls = "#baocaoTong .tong" + (i + 8);
        var clsBosung = "#baocaoTong .cha0 ." + (i + 8);
        classTong.push(cls);
        classColumBoSung.push(clsBosung);
    };
    for (var i = 0; i < soCotBoSung + soCotDaGiao + 13; i++) {
        var tong = 0;
        $(classColumBoSung[i]).each(function (index, r) {
            tong += parseInt(UnFormatNumber($(r).html() == "" ? 0 : $(r).html()));
        });
        $(classTong[i]).html(FormatNumber(tong));
    }
}

//Giao diện
function ShowTimKiem() {
    if ($("#card-condition").is(":hidden")) {
        $("#card-condition").removeClass("hidden");
    } else {
        $("#card-condition").toggleClass("hidden");
    }
}

function Cancel() {
    $("#card-condition").toggleClass("hidden");
}

function ShowModalConfig() {
    GetSoLuongTo();
    $('#rowChonTo').show();
    $('#configBaocao').modal('show');
}

$(".btn-print").click(function () {
    var links = [];
    var ext = $(this).data("ext");
    var dvt = $("#dvt").val();
    $("input:checkbox[check-group='To']").each(function () {
        if (this.checked) {
            var item = this.value;
            var url = $("#urlExport").val() +
                "?ext=" + ext +
                "&dvt=" + dvt +
                "&to=" + item;

            url = unescape(url);
            links.push(url);
        }
    })
    openLinks(links);
});

function GetSoLuongTo() {
    var iNguonNganSach = $("#iID_Nguon").val();
    var dDateFrom = $("#txtTuNgay").val();
    var dDateTo = $("#txtDenNgay").val();
    var dvt = $("#dvt").val();
    var url = '/QLNguonNganSach/BaoCaoNguonNS/Ds_To_BaoCao1/?iNguonNganSach=' + iNguonNganSach +
        '&dDateFrom=' + dDateFrom + '&dDateTo=' + dDateTo + '&dvt=' + dvt;
    fillCheckboxList("To", "To", url);
}