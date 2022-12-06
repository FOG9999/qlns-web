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

                $("#" + idContent + " .tong-cong-phongban ." + sColClass).each(function () {
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
                "&iLoaiNganSach=" + $("#iLoaiNganSach").val() +
                "&to=" + item;

            url = unescape(url);
            links.push(url);
        }
    });

    openLinks(links);
});

function GetSoLuongTo() {
    var url = '/QLNguonNganSach/BaoCaoNguonNS/Ds_To_BaoCaoChiTietQuyetDinhPhanCap/';
    fillCheckboxList("To", "To", url);
}