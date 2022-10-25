var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;
$(document).ready(function () {
    $(".table-update tbody tr[data-rowsubmit='isData']").each(function () {
        $(this).find("input:not(:disabled)").each(function (index) {
            var checkValue = $(this).val()
            if (checkValue) {
                disabledAll(this)
            }
        });
    });

});

function Save() {
    var result = [];
    $(".table-update tbody tr[data-rowsubmit='isData']").each(function () {
        var allValues = {};

        $(this).find("input").each(function (index) {
            const fieldName = $(this).attr("name");
            allValues[fieldName] = $(this).val().split('.').join("");;
        });
        $(this).find("td").each(function (index) {
            const fieldName = $(this).data("getname");
            if (fieldName !== undefined) {
                const fieldValue = $(this).data("getvalue");
                allValues[fieldName] = fieldValue;
            }
        });
        result.push(allValues);

    })
    $.ajax({
        type: "POST",
        url: "/QLNH/QuyetToanDuAnHoanThanh/SaveDetail",
        data: { data: result },
        async: false,
        success: function (r) {
            if (r && r.bIsComplete) {
                window.location.href = "/QLNH/QuyetToanDuAnHoanThanh";
            } else {
                var Title = 'Lỗi lưu thông tin quyết toán dự án';
                var messErr = [];
                messErr.push(r.sMessError);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
            }
        }
    });
}
function onBlurSum(event, text, type, num) {
    if (ValidateInputFocusOut(event, text, type, num)) {
        disabledAll(text)
    }
}
function disabledAll(event) {
    var getClass = $(event).data("getclass");
    event.value = event.value ?? "";
    //tính tỉ giá USD-VND
    var getTiGiaMa = getClass.slice(-3);
    var getName = getClass.slice(0, -3);
    var getTiGiaChiTiet = $("#tiGiaChiTiet").val();
    var dongHienTai = $(event).closest("tr");
  
    if (getTiGiaMa == "VND") {
        console.log(dongHienTai.find("input[data-getclass='" + getName + "USD" + "']"))
        let elInputUSD = dongHienTai.find("input[data-getclass='" + getName + "USD" + "']");

        let inputVND = parseFloat(UnFormatNumber($(event).val()));
        if (isNaN(inputVND)) {
            elInputUSD.val(null);
            $(event).val(null);
        } else {
            $.ajax({
                type: "POST",
                data: { number: inputVND.toString(), numTiGia: getTiGiaChiTiet.toString() },
                url: '/QLNH/KeHoachChiTietBQP/CalcMoneyByTiGia',
                async: false,
                success: function (data) {
                    let result = FormatNumber(data.result);
                    elInputUSD.val(result == '' ? null : result)
                }
            });
        }
    } else {
        let elInputVND = dongHienTai.find("input[data-getclass='" + getName + "VND" + "']");
        let inputUSD = parseFloat(UnFormatNumber($(event).val()));
        if (isNaN(inputUSD)) {
            elInputVND.val(null);
            elThuaThieuUSD.html(null);
            elThuaThieuVND.html(null);
            $(event).val(null);
        } else {

            $.ajax({
                type: "POST",
                data: { number: inputUSD.toString(), numTiGia: getTiGiaChiTiet.toString() },
                url: '/QLNH/KeHoachChiTietBQP/CalcMoneyByTiGia',
                async: false,
                success: function (data) {
                    let result = FormatNumber(data.result);
                    elInputVND.val(result == '' ? null : result)
                }
            });
        }
    }
}
function Cancel() {
    window.location.href = "/QLNH/QuyetToanDuAnHoanThanh";
}