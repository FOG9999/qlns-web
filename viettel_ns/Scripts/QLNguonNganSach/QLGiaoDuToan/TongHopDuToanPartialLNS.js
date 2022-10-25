function GetBaoCao() {
    $("#btnGetBaoCao").attr("disabled", "disabled");
    $.ajax({
        type: "POST",
        url: "/QLNguonNganSach/BaoCaoNguonNS/TongHopGiaoDuToanLNSPartial",
        data: { dDateFrom: $("#txtTuNgay").val(), dDateTo: $("#txtDenNgay").val(), dvt: $("#dvt").val() },
        success: function (data) {
            $("#showData").html(data);
            $("#contentReport").removeClass('hidden');
            $("#btnGetBaoCao").removeAttr("disabled");
        }
    });
}

function getDaGiaoChoCacNganhDonVi() {
    $("#showData").find(".row-II-1 td").each(function () {
        var sColClass = $(this).attr("class");
        if (sColClass != '' && sColClass != null && sColClass != undefined) {
            if (sColClass.indexOf("col-") != -1) {
                sColClass = sColClass.split(" ")[0];

                var iMoneyRow1 = 0;
                if ($.isNumeric($(".row-II-1-1 ." + sColClass).text().replace(/\./g, ''))) {
                    iMoneyRow1 = parseFloat($(".row-II-1-1 ." + sColClass).text().replace(/\./g, ''));
                }

                var iMoneyRow2 = 0;
                if ($.isNumeric($(".row-II-1-2 ." + sColClass).text().replace(/\./g, ''))) {
                    iMoneyRow2 = parseFloat($(".row-II-1-2 ." + sColClass).text().replace(/\./g, ''));
                }

                var iTotal = iMoneyRow1 + iMoneyRow2;

                $('.row-II-1 .' + sColClass).append(iTotal == 0 ? "" : iTotal.toLocaleString('vi-VN'));
            }
        }
    });
}

function getDataChoPhancap() {
    $("#showData").find(".row-ChoPhanCap td").each(function () {
        var sColClass = $(this).attr("class");
        if (sColClass != '' && sColClass != null && sColClass != undefined) {
            if (sColClass.indexOf("col-") != -1) {
                sColClass = sColClass.split(" ")[0];
                var iToTalMoney = 0;
                if ($.isNumeric($(".row-I ." + sColClass).text().replace(/\./g, ''))) {
                    iToTalMoney = parseFloat($(".row-I ." + sColClass).text().replace(/\./g, ''));
                }

                var iMoney = 0;
                if ($.isNumeric($(".row-a ." + sColClass).text().replace(/\./g, ''))) {
                    iMoney = parseFloat($(".row-a ." + sColClass).text().replace(/\./g, ''));
                }

                var iTotal = iToTalMoney - iMoney;

                $('.row-ChoPhanCap .' + sColClass).append(iTotal == 0 ? "" : iTotal.toLocaleString('vi-VN'));
            }
        }
    });
}

function getDataGiaoChoDonVi() {
    $("#showData").find(".row-II th").each(function () {
        var sColClass = $(this).attr("class");
        if (sColClass != '' && sColClass != null && sColClass != undefined) {
            if (sColClass.indexOf("col-") != -1) {
                sColClass = sColClass.split(" ")[0];

                var iToTalMoney = 0;
                if ($.isNumeric($(".row-II-1 ." + sColClass).text().replace(/\./g, ''))) {
                    iToTalMoney = parseFloat($(".row-II-1 ." + sColClass).text().replace(/\./g, ''));
                }
                var iMoney = 0;
                if ($.isNumeric($(".row-II-2 ." + sColClass).text().replace(/\./g, ''))) {
                    iMoney = parseFloat($(".row-II-2 ." + sColClass).text().replace(/\./g, ''));
                }

                var iToTal = iToTalMoney + iMoney;
                $('.row-II .' + sColClass).append(iToTal == 0 ? "" : iToTal.toLocaleString('vi-VN'));
            }
        }
    });
}

function getChoPheDuyet() {
    $("#showData").find(".row-II-2 td").each(function () {
        var sColClass = $(this).attr("class");
        if (sColClass != '' && sColClass != null && sColClass != undefined) {
            if (sColClass.indexOf("col-") != -1) {
                sColClass = sColClass.split(" ")[0];
                var iToTalMoney = 0;

                $('.' + sColClass).each(function () {
                    var itemMoney = this.innerText.replace(/\./g, '');
                    if ($.isNumeric(itemMoney)) {
                        iToTalMoney += parseFloat(itemMoney);
                    }
                });

                $('.row-II-2 .' + sColClass).append(iToTalMoney == 0 ? "" : iToTalMoney.toLocaleString('vi-VN'));
            }
        }
    });
}

function getDataConLaiTaiBo() {
    $("#showData").find(".row-III th").each(function () {
        var sColClass = $(this).attr("class");
        if (sColClass != '' && sColClass != null && sColClass != undefined) {
            if (sColClass.indexOf("col-") != -1) {
                sColClass = sColClass.split(" ")[0];

                var iToTalMoney = 0;
                if ($.isNumeric($(".row-I ." + sColClass).text().replace(/\./g, ''))) {
                    iToTalMoney = parseFloat($(".row-I ." + sColClass).text().replace(/\./g, ''));
                }
                var iMoney = 0;
                if ($.isNumeric($(".row-II ." + sColClass).text().replace(/\./g, ''))) {
                    iMoney = parseFloat($(".row-II ." + sColClass).text().replace(/\./g, ''));
                }

                var iTotal = iToTalMoney - iMoney;

                $('.row-III .' + sColClass).append(iTotal == 0 ? "" : iTotal.toLocaleString('vi-VN'));
            }
        }
    });
}

function getTongCongNguonCon() {
    $("#showData").find(".tong-cong-nguon-con td").each(function () {
        var sColClass = $(this).attr("class");
        if (sColClass != '' && sColClass != null && sColClass != undefined) {
            if (sColClass.indexOf("col-") != -1) {
                sColClass = sColClass.split(" ")[0];
                var iToTalMoney = 0;

                $('.row-b .' + sColClass).each(function () {
                    var itemMoney = this.innerText.replace(/\./g, '');
                    if ($.isNumeric(itemMoney)) {
                        iToTalMoney += parseFloat(itemMoney);
                    }
                });

                $('.tong-cong-nguon-con .' + sColClass).append(iToTalMoney == 0 ? "" : iToTalMoney.toLocaleString('vi-VN'));
            }
        }
    });

}

function getTongCongNguonCha() {
    $("#showData").find(".tong-cong-nguon-cha td").each(function () {
        var sColClass = $(this).attr("class");
        if (sColClass != '' && sColClass != null && sColClass != undefined) {
            if (sColClass.indexOf("nguon-") != -1) {
                sColClass = sColClass.split(" ")[1];
                var iToTalMoney = 0;

                $('.tong-cong-nguon-con .' + sColClass).each(function () {
                    var itemMoney = this.innerText.replace(/\./g, '');
                    if ($.isNumeric(itemMoney)) {
                        iToTalMoney += parseFloat(itemMoney);
                    }
                });

                $('.tong-cong-nguon-cha .' + sColClass).append(iToTalMoney == 0 ? "" : iToTalMoney.toLocaleString('vi-VN'));
            }
        }
    });

}

// for popup chi tiet quyetdinh
var idContent = 'contentModalChiTietQuyetDinh';
function getTongCongNguonPhongBan() {
    $("#" + idContent).find(".tong-cong-phongban td").each(function () {
        var sColClass = $(this).attr("class");
        if (sColClass != '' && sColClass != null && sColClass != undefined) {
            if (sColClass.indexOf("col-") != -1) {
                sColClass = sColClass.split(" ")[0];
                var iToTalMoney = 0;

                $("#" + idContent + " .row-b ." + sColClass).each(function () {
                    var itemMoney = this.innerText.replace(/\./g, '');
                    if ($.isNumeric(itemMoney)) {
                        iToTalMoney += parseFloat(itemMoney);
                    }
                });
                $("#" + idContent + " .tong-cong-phongban ." + sColClass).html(iToTalMoney == 0 ? "" : iToTalMoney.toLocaleString('vi-VN'));
            }
        }
    });
}

function getTongCongNguonNhiemVu() {
    $("#" + idContent).find(".tong-cong-nhiemvu td").each(function () {
        var sColClass = $(this).attr("class");
        if (sColClass != '' && sColClass != null && sColClass != undefined) {
            if (sColClass.indexOf("col-") != -1) {
                sColClass = sColClass.split(" ")[0];
                var iToTalMoney = 0;

                $("#" + idContent + " .tong-cong-phongban ." + sColClass).each(function () {
                    var itemMoney = this.innerText.replace(/\./g, '');
                    if ($.isNumeric(itemMoney)) {
                        iToTalMoney += parseFloat(itemMoney);
                    }
                });
                $("#" + idContent + " .tong-cong-nhiemvu ." + sColClass).html(iToTalMoney == 0 ? "" : iToTalMoney.toLocaleString('vi-VN'));
            }
        }
    });
}

function getTongCongNguonDonVi() {
    $("#" + idContent+" .row-b").each(function () {
        var iToTalMoney = 0;
        $(this).find("td").each(function () {
            var sColClass = $(this).attr("class");
            if (sColClass != undefined && sColClass.indexOf("col-") != -1) {
                var itemMoney = this.innerText.replace(/\./g, '');
                if ($.isNumeric(itemMoney)) {
                    iToTalMoney += parseFloat(itemMoney);
                }
            }
        });
        $(this).find(".tong-cong-nguon-chi-tiet").html(iToTalMoney == 0 ? "" : iToTalMoney.toLocaleString('vi-VN'));
    });
}

function getTongCong() {
    $("#" + idContent).find(".row-III th").each(function () {
        var sColClass = $(this).attr("class");
        if (sColClass != '' && sColClass != null && sColClass != undefined) {
            if (sColClass.indexOf("col-") != -1) {
                sColClass = sColClass.split(" ")[0];
                var iToTalMoney = 0;

                $("#" + idContent + " .tong-cong-nhiemvu ." + sColClass).each(function () {
                    var itemMoney = this.innerText.replace(/\./g, '');
                    if ($.isNumeric(itemMoney)) {
                        iToTalMoney += parseFloat(itemMoney);
                    }
                });

                $("#" + idContent + " .row-III ." + sColClass).html(iToTalMoney == 0 ? "" : iToTalMoney.toLocaleString('vi-VN'));
            }
        }
    });
}

$(".btn-print").click(function () {
    var links = [];
    var ext = $(this).data("ext");
    var dvt = $("#dvt").val();
    var url = $("#urlExport").val() +
        "?ext=" + ext +
        "&dvt=" + dvt;

    url = unescape(url);
    links.push(url);

    openLinks(links);
});