var iIDHopDongID;

$(document).ready(function () {
    iIDHopDongID = $("#iIDHopDongID").val();
    KiemTraHienThiDieuChinh();
});

$("#dNgayHopDong").change(function () {
    KiemTraHienThiDieuChinh();
});

function KiemTraHienThiDieuChinh() {
    var dNgayHopDongGoc = $("#dNgayHopDongGoc").val();
    var bIsGoc = $("#bIsGoc").val();
    if (bIsGoc != "True" || (bIsGoc == "True" && SoSanh2NgayString($("#dNgayHopDong").val(), dNgayHopDongGoc) == 1)) {
        $("#txtLoaiDieuChinh").removeAttr("disabled");
        $("#txtGiaTriDieuChinh").removeAttr("disabled");
        $("#txtGiaTriSauDieuChinh").removeAttr("disabled");

        var tienTruoc = LayGiaTriTruocDieuChinh();
        $("#txtGiaTriTruocDieuChinh").html(FormatNumber(tienTruoc) == "" ? 0 : FormatNumber(tienTruoc));
    } else {
        $("#txtLoaiDieuChinh").attr("disabled", "disabled");
        $("#txtGiaTriDieuChinh").attr("disabled", "disabled");
        $("#txtGiaTriSauDieuChinh").attr("disabled", "disabled");

        $("#txtLoaiDieuChinh").val("");
        $("#txtGiaTriDieuChinh").val("");
        $("#txtGiaTriSauDieuChinh").val("");
        $("#txtGiaTriTruocDieuChinh").html("");
    }
}

function LayGiaTriTruocDieuChinh() {
    var value = 0;
    $.ajax({
        url: "/QLVonDauTu/QLThongTinHopDong/LayGiaTriTruocDieuChinh",
        type: "POST",
        data: { id: iIDHopDongID, dNgayHopDong: $("#dNgayHopDong").val() },
        dataType: "json",
        async: false,
        cache: false,
        success: function (data) {
            if (data != null) {
                value = data;
            }
        },
        error: function (data) {

        }
    })
    return value;
}

function CheckLoi(doiTuong) {
    var messErr = [];
    if (doiTuong.sSoHopDong == "")
        messErr.push("Số hợp đồng chưa có hoặc chưa chính xác.");
    if (doiTuong.dNgayHopDong == "")
        messErr.push("Ngày hợp đồng chưa có hoặc chưa chính xác.");
    if (doiTuong.fGiaTriDieuChinh == 0 || parseFloat(UnFormatNumber($("#txtGiaTriSauDieuChinh").val() == "" ? 0 : $("#txtGiaTriSauDieuChinh").val())) <= 0)
        messErr.push("Thông tin điều chỉnh chưa có hoặc chưa chính xác.");

    if (messErr.length > 0) {
        alert(messErr.join("\n"));
        return false;
    } else {
        return true;
    }
}

function Huy() {
    window.location.href = "/QLVonDauTu/QLThongTinHopDong/Index";
}

function Luu() {
    var hopDong = {};
    hopDong.iID_HopDongID = iIDHopDongID;
    hopDong.sSoHopDong = $("#sSoHopDong").val();
    hopDong.dNgayHopDong = $("#dNgayHopDong").val();
    hopDong.iThoiGianThucHien = $("#iThoiGianThucHien").val();
    var giaTriDieuChinh = parseFloat(UnFormatNumber($("#txtGiaTriDieuChinh").val() == "" ? 0 : $("#txtGiaTriDieuChinh").val()));
    hopDong.fGiaTriDieuChinh = $("#txtLoaiDieuChinh").val() == 0 ? (0 - giaTriDieuChinh) : giaTriDieuChinh;

    if (CheckLoi(hopDong)) {
        $.ajax({
            url: "/QLVonDauTu/QLThongTinHopDong/Save",
            type: "POST",
            data: { model: hopDong, isDieuChinh: true },
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data.status == true) {
                    window.location.href = "/QLVonDauTu/QLThongTinHopDong/Index";
                }
            },
            error: function (data) {

            }
        })
    }
}

$("#txtGiaTriDieuChinh").change(function () {
    var rGiaTriTruocDieuChinh = parseFloat(UnFormatNumber($("#txtGiaTriTruocDieuChinh").text() == "" ? 0 : $("#txtGiaTriTruocDieuChinh").text()));
    var rGiaTriDieuChinh = parseFloat(UnFormatNumber($("#txtGiaTriDieuChinh").val() == "" ? 0 : $("#txtGiaTriDieuChinh").val()));
    var rLoaiDieuChinh = $("#txtLoaiDieuChinh").val();
    var rGiaTriSauDieuChinh = 0;
    if (rLoaiDieuChinh == "") {
        alert("Thông tin loại điều chỉnh chưa có hoặc chưa chính xác.");
        return;
    } else {
        if (rLoaiDieuChinh == 0) {
            rGiaTriSauDieuChinh = rGiaTriTruocDieuChinh - rGiaTriDieuChinh;
        } else if (rLoaiDieuChinh == 1) {
            rGiaTriSauDieuChinh = rGiaTriTruocDieuChinh + rGiaTriDieuChinh;
        }
        $("#txtGiaTriSauDieuChinh").val(FormatNumber(rGiaTriSauDieuChinh) == "" ? 0 : FormatNumber(rGiaTriSauDieuChinh));
    }
})

$("#txtGiaTriSauDieuChinh").change(function () {
    var rGiaTriTruocDieuChinh = parseFloat(UnFormatNumber($("#txtGiaTriTruocDieuChinh").text() == "" ? 0 : $("#txtGiaTriTruocDieuChinh").text()));
    var rGiaTriSauDieuChinh = parseFloat(UnFormatNumber($("#txtGiaTriSauDieuChinh").val() == "" ? 0 : $("#txtGiaTriSauDieuChinh").val()));
    var rLoaiDieuChinh = $("#txtLoaiDieuChinh").val();
    var rGiaTriDieuChinh = 0;
    if (rLoaiDieuChinh == "") {
        alert("Thông tin loại điều chỉnh chưa có hoặc chưa chính xác.");
        return;
    } else {
        if (rLoaiDieuChinh == 0) {
            rGiaTriDieuChinh = rGiaTriSauDieuChinh - rGiaTriTruocDieuChinh;
        } else if (rLoaiDieuChinh == 1) {
            rGiaTriDieuChinh = rGiaTriSauDieuChinh - rGiaTriTruocDieuChinh;
        }
        $("#txtGiaTriDieuChinh").val(FormatNumber(rGiaTriDieuChinh) == "" ? 0 : FormatNumber(rGiaTriDieuChinh));
    }
})

$("#txtLoaiDieuChinh").change(function () {
    var rGiaTriTruocDieuChinh = parseFloat(UnFormatNumber($("#txtGiaTriTruocDieuChinh").text() == "" ? 0 : $("#txtGiaTriTruocDieuChinh").text()));
    var rGiaTriDieuChinh = parseFloat(UnFormatNumber($("#txtGiaTriDieuChinh").val() == "" ? 0 : $("#txtGiaTriDieuChinh").val()));
    var rLoaiDieuChinh = $("#txtLoaiDieuChinh").val();
    var rGiaTriSauDieuChinh = 0;
    if (rLoaiDieuChinh == "") {
        alert("Thông tin loại điều chỉnh chưa có hoặc chưa chính xác.");
        return;
    } else {
        if (rLoaiDieuChinh == 0) {
            rGiaTriSauDieuChinh = rGiaTriTruocDieuChinh - rGiaTriDieuChinh;
        } else if (rLoaiDieuChinh == 1) {
            rGiaTriSauDieuChinh = rGiaTriTruocDieuChinh + rGiaTriDieuChinh;
        }
        $("#txtGiaTriSauDieuChinh").val(FormatNumber(rGiaTriSauDieuChinh) == "" ? 0 : FormatNumber(rGiaTriSauDieuChinh));
    }
})

function SoSanh2NgayString(sDate1, sDate2) {
    var arrDate1 = sDate1.split("/");
    var arrDate2 = sDate2.split("/");
    if (arrDate1.length != 3 || arrDate2.length != 3)
        return 0;
    var date1 = new Date(arrDate1[2], arrDate1[1], arrDate1[0]);
    var date2 = new Date(arrDate2[2], arrDate2[1], arrDate2[0]);
    if (date1 >= date2)
        return 1;
    return 2;
} 