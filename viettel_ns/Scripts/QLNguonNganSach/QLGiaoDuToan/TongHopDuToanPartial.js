$(document).ready(function () {
    $("#drpLoaiNganSach").change(function () {
        if (this.value != "") {
            $.ajax({
                url: "/BaoCaoNguonNS/LayDanhSachDMNguonTheoLoaiNganSach",
                type: "POST",
                data: { iLoaiNganSach: this.value },
                dataType: "json",
                cache: false,
                success: function (data) {
                    if (data != null && data != "") {
                        $("#iID_Nguon").html(data);
                    }
                },
                error: function (data) {

                }
            })
        } else {
            $("#iID_Nguon option:not(:first)").remove();
        }
    });

    $("#drpLoaiNganSach").trigger("change");
});

function GetBaoCao() {
    $("#btnGetBaoCao").attr("disabled", "disabled");
    $.ajax({
        type: "POST",
        url: "/QLNguonNganSach/BaoCaoNguonNS/TongHopGiaoDuToanNewPartial",
        data: {
            iLoaiNganSach: $("#drpLoaiNganSach option:selected").val(), iID_Nguon: $("#iID_Nguon option:selected").val(),
            dDateFrom: $("#txtTuNgay").val(), dDateTo: $("#txtDenNgay").val(), sSoQuyetDinh: $("#txtsSoQuyetDinh").val(),
            dvt: $("#dvt").val()
        },
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

                var iTotal = iToTalMoney + iMoney;

                $('.row-II .' + sColClass).append(iTotal == 0 ? "" : iTotal.toLocaleString('vi-VN'));
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
                sColClass = sColClass.split(" ")[0];
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

function getTongCongNguonGhiChu() {
    $("#showData .row-b").each(function () {
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

        $(this).find(".tong-cong-nguon-chi-tiet").append(iToTalMoney == 0 ? "" : iToTalMoney.toLocaleString('vi-VN'));
    });
}

function OpenModalChiTietQuyetDinh(sSoQuyetDinh) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNguonNganSach/BaoCaoNguonNS/ChiTietQuyetDinhPartial",
        data: { sSoQuyetDinh: sSoQuyetDinh },
        success: function (data) {
            $("#contentModalChiTietQuyetDinh").html(data);
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

function chuyenTrangBaoCao() {
    var listNDC = JSON.parse($("#listNDC").val());

    $("[class*='td-']").hide();
    $(".td-" + listNDC[0].iID_NoiDungChi).show();

    $('.paging').bootpag({
        total: listNDC.length,
        page: 1
    }).on("page", function (event, num) {
        $("[class*='td-']").hide();
        $(".td-" + listNDC[num - 1].iID_NoiDungChi).show();

        // chỉ show tổng cộng nguồn từ tờ 2 (trang 5)
        if (num >= 5) {
            $('.col-tongcong').hide();
        }
    });
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
                "&iLoaiNganSach=" + $("#drpLoaiNganSach option:selected").val() +
                "&to=" + item;

            url = unescape(url);
            links.push(url);
        }
    })
    openLinks(links);
});

function GetSoLuongTo() {
    var iLoaiNganSach = $("#drpLoaiNganSach option:selected").val();
    var iID_Nguon = $("#iID_Nguon option:selected").val();
    var dDateFrom = $("#txtTuNgay").val();
    var dDateTo= $("#txtDenNgay").val();
    var sSoQuyetDinh = $("#txtsSoQuyetDinh").val();
    var dvt = $("#dvt").val();
    
    var url = '/QLNguonNganSach/BaoCaoNguonNS/Ds_To_BaoCao3/?iLoaiNganSach=' + iLoaiNganSach +
        '&iID_Nguon=' + iID_Nguon+
        '&dDateFrom=' + dDateFrom +
        '&dDateTo=' + dDateTo +
        '&sSoQuyetDinh=' + sSoQuyetDinh +
        '&dvt=' + dvt;
    fillCheckboxList("To", "To", url);
}