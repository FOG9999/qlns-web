var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var ADD_ROW = 1;
var REMOVE_ROW = 0;

$(document).ready(function () {
    DinhDangSo();
});

function DinhDangSo() {
    $(".sotien").each(function () {
        $(this).val(FormatNumber($(this).val()));
    })
}

function DinhDangSo1() {
    $(".sotien1").each(function () {
        $(this).val(FormatNumber($(this).val()));
    })
}

function SaveData() {
    var data = [];

    $('#bodyChiTietKTTTDA tr').each(function () {
        var item = {};
        item.iID_KhoiTao_ChiTietID = $(this).find(".iID").val();
        item.iID_KhoiTaoDuLieuID = $("#iID_KhoiTaoID").val();
        item.iID_DuAnID = $(this).find(".duan").val();
        item.iID_LoaiCongTrinh = $(this).find(".loaicongtrinh").val();
        item.iID_NguonVonID = $(this).find(".cbxNguonVon").val();
        item.iCoQuanThanhToan = $(this).find(".coquantt").val();
        item.fKHVN_VonBoTriHetNamTruoc = UnFormatNumber($(this).find(".vn-botrihetnamtruoc").val());
        item.fKHVN_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc = UnFormatNumber($(this).find(".vn-luykevondatt").val());
        item.fKHVN_TrongDoVonTamUngTheoCheDoChuaThuHoi = UnFormatNumber($(this).find(".vn-trongdovontamung").val());
        item.fKHVN_KeHoachVonKeoDaiSangNam = UnFormatNumber($(this).find(".vn-kehoachvonkeodai").val());
        item.fKHUT_VonBoTriHetNamTruoc = UnFormatNumber($(this).find(".ut-botrihetnamtruoc").val());
        item.fKHUT_LKVonDaThanhToanTuKhoiCongDenHetNamTruoc = UnFormatNumber($(this).find(".ut-luykevondatt").val());
        item.fKHUT_TrongDoVonTamUngTheoCheDoChuaThuHoi = UnFormatNumber($(this).find(".ut-trongdovontamung").val());
        item.fKHUT_KeHoachUngTruocKeoDaiSangNam = UnFormatNumber($(this).find(".ut-kehoachungtruoc").val());
        item.fKHUT_KeHoachUngTruocChuaThuHoi = UnFormatNumber($(this).find(".ut-ungtruocchuathuhoi").val());
        data.push(item);
    });

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKhoiTaoThongTinDuAn/ChiTietKhoiTaoTTDASave",
        data: { data: data },
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/QLVonDauTu/QLKhoiTaoThongTinDuAn";
            } else {
                var Title = 'Lỗi lưu chi tiết khởi tạo thông tin dự án';
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

function ValidateData(data) {
    var Title = 'Lỗi lưu dữ liệu chi tiết khởi tạo thông tin dự án';
    var Messages = [];

    if (data == null || data.length == 0) {
        Messages.push("Vui lòng thêm chi tiết khởi tạo dự án !");
    }

    for (var i = 0; i < data.length; i++) {
        if (data[i].iID_DuAnID == null || data[i].iID_DuAnID == GUID_EMPTY) {
            Messages.push("Chưa chọn dự án cho dòng thứ " + (i + 1) + " !");
        }        

        for (var j = 0; j < i; j++) {
            if ((data[j].iID_DuAnID != null && data[j].iID_DuAnID != GUID_EMPTY && data[j].iID_DuAnID == data[i].iID_DuAnID)
                && (data[j].iID_NguonVonID != null && data[j].iID_NguonVonID != GUID_EMPTY && data[j].iID_NguonVonID == data[i].iID_NguonVonID)
                && (data[j].iCoQuanThanhToan != null && data[j].iCoQuanThanhToan != GUID_EMPTY && data[j].iCoQuanThanhToan == data[i].iCoQuanThanhToan)) {
                Messages.push("Có dự án trùng nguồn vốn và cơ quan thanh toán.");
            }
        }
    }

    if (Messages != null && Messages != undefined && Messages.length > 0) {
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: Messages, Category: ERROR },
            success: function (data) {
                $("#divModalConfirm").html(data);
            }
        });
        return false;
    }

    return true;
}
function ResetChangePage() {
    location.reload();
}

function GetListDataChiTietKhoiTaoTTDA(type, STT, iID_KhoiTao) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLKhoiTaoThongTinDuAn/ChiTietKhoiTaoTTDAListView",
        data: { type: type, STT: STT, iID_KhoiTao: iID_KhoiTao, sMaDonVi: $("#sMaDonVi").val() },
        success: function (data) {
            $("#lstDataView").html(data);
            ReloadNguonVon();
        }
    });
}

function AddRowChiTiet(iID_KhoiTaoID, sMaDonVi) {
    GetListDataChiTietKhoiTaoTTDA(ADD_ROW, 0, iID_KhoiTaoID);
}

function RemoveRowChiTiet(STT, iID_KhoiTaoID, sMaDonVi) {
    GetListDataChiTietKhoiTaoTTDA(REMOVE_ROW, STT, iID_KhoiTaoID);
}

function GoBack() {
    window.location.href = "/QLVonDauTu/QLKhoiTaoThongTinDuAn";
}

function AddChiTietThanhToan(iID_KhoiTao_ChiTietID, iID_DuAnID) {
    var iID_DuAnID = $("#tblListChiTietKhoiTaoTTDA [data-id='" + iID_KhoiTao_ChiTietID + "']").find(".duan").val();
    var sTenDuAn = $("#tblListChiTietKhoiTaoTTDA [data-id='" + iID_KhoiTao_ChiTietID + "']").find(".duan option:selected").text();
    var sNguonVon = $("#tblListChiTietKhoiTaoTTDA [data-id='" + iID_KhoiTao_ChiTietID + "']").find(".cbxNguonVon option:selected").text();
    var sCoQuanThanhToan = $("#tblListChiTietKhoiTaoTTDA [data-id='" + iID_KhoiTao_ChiTietID + "']").find(".coquantt option:selected").text();
    if (iID_DuAnID == undefined || iID_DuAnID == null || iID_DuAnID == "" || iID_DuAnID == GUID_EMPTY) {
        alert("Chưa chọn dự án !");
        return;
    }
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLKhoiTaoThongTinDuAn/GetModalChiTietThanhToan",
        data: { iID_KhoiTao_ChiTietID: iID_KhoiTao_ChiTietID, iID_DuAnID: iID_DuAnID },
        success: function (data) {
            $("#contentModalChiTietThanhToan").html(data);

            $("#modalChiTietThanhToanLabel").html('Chi tiết hợp đồng khởi tạo thông tin dự án');
            $("#sTenDuAn").html(sTenDuAn);
            $("#sNguonVon").html(sNguonVon);
            $("#sCoQuanThanhToan").html(sCoQuanThanhToan);
            $(".date").datepicker({
                todayBtn: "linked",
                language: "it",
                autoclose: true,
                todayHighlight: true,
                format: 'dd/mm/yyyy'
            });
        }
    });
    $('#modalChiTietThanhToan').modal('show');
}

function AddRowHopDong(iID_KhoiTaoDuLieuChiTietID, iID_DuAnID) {
    GetListDataHopDong(ADD_ROW, 0, iID_KhoiTaoDuLieuChiTietID, iID_DuAnID);
}

function RemoveRowHopDong(STT, iID_KhoiTaoDuLieuChiTietID, iID_DuAnID) {
    GetListDataHopDong(REMOVE_ROW, STT, iID_KhoiTaoDuLieuChiTietID, iID_DuAnID);
}

function GetListDataHopDong(type, STT, iID_KhoiTaoDuLieuChiTietID, iID_DuAnID) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLKhoiTaoThongTinDuAn/ChiTietHopDongListView",
        data: { type: type, STT: STT, iID_KhoiTaoDuLieuChiTietID: iID_KhoiTaoDuLieuChiTietID, iID_DuAnID: iID_DuAnID },
        success: function (data) {
            $("#bodyHopDongChiTiet").html(data);
            FindGiaTriHopDongAndTenNhaThau();
        }
    });
}

function SaveHopDong(iID_KhoiTaoDuLieuChiTietID) {
    var data = [];

    $('#bodyHopDongChiTiet tr').each(function () {
        var item = {};
        item.iId_KhoiTaoDuLieuChiTietId = iID_KhoiTaoDuLieuChiTietID;
        item.iID_KhoiTaoDuLieuChiTietThanhToanID = $(this).find(".iIDThanhToan").val();
        item.iID_HopDongId = $(this).find(".hopdong").val();
        item.fLuyKeTTKLHTTN_KHVN = UnFormatNumber($(this).find(".fluykethanhtoantnvn ").val());
        item.fLuyKeTUChuaThuHoiTN_KHVN = UnFormatNumber($(this).find(".fluykechuathuhoitnvn ").val());
        item.fLuyKeTTKLHTNN_KHVN = UnFormatNumber($(this).find(".fluykethanhtoannnvn ").val());
        item.fLuyKeTUChuaThuHoiNN_KHVN = UnFormatNumber($(this).find(".fluykechuathuhoinnvn ").val());

        item.fLuyKeTTKLHTTN_KHVU = UnFormatNumber($(this).find(".fluykethanhtoantnvn ").val());
        item.fLuyKeTUChuaThuHoiTN_KHVU = UnFormatNumber($(this).find(".fluykechuathuhoitnvn ").val());
        item.fLuyKeTTKLHTNN_KHVU = UnFormatNumber($(this).find(".fluykethanhtoannnvn ").val());
        item.fLuyKeTUChuaThuHoiNN_KHVU = UnFormatNumber($(this).find(".fluykechuathuhoinnvn ").val());
        data.push(item);
    });

    if (data == null || data.length == 0) {
        $('#modalChiTietThanhToan').modal('hide');
        return false;
    }

    if (!ValidateDataHopDong(data)) {
        return false;
    }
    CaculatorLuyKeThanhToanTuKCHTVU();
    CaculatorLuyKeThanhToanTuKCHTVN();
    CaculatorVonTamUngTCDCTHVN();
    CaculatorVonTamUngTCDCTHVU();

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKhoiTaoThongTinDuAn/ChangeListHopDong",
        data: { data: data },
        success: function (r) {
            if (r.bIsComplete) {
                $('#modalChiTietThanhToan').modal('hide');
            } else {
                var Title = 'Lỗi lưu chi tiết khởi tạo thông tin dự án';
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

function ValidateDataHopDong(data) {
    var Title = 'Lỗi lưu dữ liệu hợp đồng chi tiết khởi tạo thông tin dự án';
    var Messages = [];

    for (var i = 0; i < data.length; i++) {
        if (data[i].iID_HopDongId == null || data[i].iID_HopDongId == GUID_EMPTY) {
            Messages.push("Chưa chọn hợp đồng cho dòng thứ " + (i + 1) + " !");
        }

        for (var j = 0; j < i; j++) {
            if (data[j].iID_HopDongId != null && data[j].iID_HopDongId != GUID_EMPTY && data[j].iID_HopDongId == data[i].iID_HopDongId) {
                Messages.push("Không được chọn trùng hợp đồng !");
            }
        }
    }

    if (Messages != null && Messages != undefined && Messages.length > 0) {
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: Messages, Category: ERROR },
            success: function (data) {
                $("#divModalConfirm").html(data);
            }
        });
        return false;
    }

    return true;
}

function ReloadNguonVon() {
    $(".duan").on("change", function () {
        var tagTr = this.closest("tr");
        var iIDDuAnId = $(this).val();
        if (iIDDuAnId == undefined || iIDDuAnId == null || iIDDuAnId == "") return;
        $.ajax({
            type: "Get",
            url: "/QLVonDauTu/QLKhoiTaoThongTinDuAn/GetQdDauTuNguonVonByDuAn",
            data: { iIdDuAn: iIDDuAnId },
            success: function (data) {
                $(tagTr).find(".cbxNguonVon").empty();
                $(tagTr).find(".cbxNguonVon").append(data.strCombobox);
                var nguonVonId = $(tagTr).data('nguonvonid');
                if (nguonVonId != undefined && nguonVonId != null)
                    $(tagTr).find(".cbxNguonVon").val(nguonVonId);
            }
        });


    });
}
function SumGiaTriLuykeTTVN() {
    $("#tblListChiTietKhoiTaoTTDA .fGiaTriLuyKeDaTTTN_VN").on("change", function () {
        CaculatorLuyKeThanhToanTuKCHTVN();
    })
}
function SumGiaTriLuykeTTVU() {
    $("#tblListChiTietKhoiTaoTTDA .fGiaTriLuyKeDaTTTN_VU").on("change", function () {
        CaculatorLuyKeThanhToanTuKCHTVU();
    })
}
function SumGiaTriTrongDoVTUVN() {
    $("#tblListChiTietKhoiTaoTTDA .fGiaTriTrongDoVTU_VN").on("change", function () {
        CaculatorVonTamUngTCDCTHVN();
    })
}
function SumGiaTriTrongDoVTUVU() {
    $("#tblListChiTietKhoiTaoTTDA .fGiaTriTrongDoVTU_VU").on("change", function () {
        CaculatorVonTamUngTCDCTHVU();
    })
}

function CaculatorLuyKeThanhToanTuKCHTVU() {
    var fTong = 0;
    $.each($("#contentModalChiTietThanhToan tbody").find("tr"), function (index, item) {
        var fluykevondatttn = UnFormatNumber($(item).find(".fluykethanhtoantnvu").val());
        var fluykevondattnn = UnFormatNumber($(item).find(".fluykethanhtoannnvu").val());
        if (fluykevondatttn != undefined && fluykevondatttn != null && fluykevondatttn != "" && (fluykevondattnn != undefined && fluykevondattnn != null && fluykevondattnn != ""))
            fTong += (parseFloat(fluykevondatttn) + parseFloat(fluykevondattnn));
    });
    $("[data-id='" + $("#iID_KhoiTaoDuLieuChiTietID").val() + "']").find(".fGiaTriLuyKeDaTTTN_VU").val(FormatNumber(fTong));
}

function CaculatorLuyKeThanhToanTuKCHTVN() {
    var fTong = 0;
    $.each($("#contentModalChiTietThanhToan tbody").find("tr"), function (index, item) {
        var fluykevondatttn = UnFormatNumber($(item).find(".fluykethanhtoantnvn").val());
        var fluykevondattnn = UnFormatNumber($(item).find(".fluykethanhtoannnvn").val());
        if (fluykevondatttn != undefined && fluykevondatttn != null && fluykevondatttn != "" && (fluykevondattnn != undefined && fluykevondattnn != null && fluykevondattnn != ""))
            fTong += (parseFloat(fluykevondatttn) + parseFloat(fluykevondattnn));
    });
    $("[data-id='" + $("#iID_KhoiTaoDuLieuChiTietID").val() + "']").find(".fGiaTriLuyKeDaTTTN_VN").val(FormatNumber(fTong));
}

function CaculatorVonTamUngTCDCTHVN() {
    var fTong = 0;
    $.each($("#contentModalChiTietThanhToan tbody").find("tr"), function (index, item) {
        var fluykevondatttn = UnFormatNumber($(item).find(".fluykechuathuhoitnvn").val());
        var fluykevondattnn = UnFormatNumber($(item).find(".fluykechuathuhoinnvn").val());
        if (fluykevondatttn != undefined && fluykevondatttn != null && fluykevondatttn != "" && (fluykevondattnn != undefined && fluykevondattnn != null && fluykevondattnn != ""))
            fTong += (parseFloat(fluykevondatttn) + parseFloat(fluykevondattnn));
    });
    $("[data-id='" + $("#iID_KhoiTaoDuLieuChiTietID").val() + "']").find(".fGiaTriTrongDoVTU_VN").val(FormatNumber(fTong));
}
function CaculatorVonTamUngTCDCTHVU() {
    var fTong = 0;
    $.each($("#contentModalChiTietThanhToan tbody").find("tr"), function (index, item) {
        var fluykevondatttn = UnFormatNumber($(item).find(".fluykechuathuhoitnvu").val());
        var fluykevondattnn = UnFormatNumber($(item).find(".fluykechuathuhoinnvu").val());
        if (fluykevondatttn != undefined && fluykevondatttn != null && fluykevondatttn != "" && (fluykevondattnn != undefined && fluykevondattnn != null && fluykevondattnn != ""))
            fTong += (parseFloat(fluykevondatttn) + parseFloat(fluykevondattnn));
    });
    $("[data-id='" + $("#iID_KhoiTaoDuLieuChiTietID").val() + "']").find(".fGiaTriTrongDoVTU_VU").val(FormatNumber(fTong));
}

function FindGiaTriHopDongAndTenNhaThau() {
    //$(".duan").on("change", function () {
    //    var tagtr = this.closest("tr");
    //    $(tagtr).find(".stennhathau").empty();
    //    $(tagtr).find(".sgiatrihopdong").empty();

    //    var iid_hopdongid = $(this).val();
    //    if (iid_hopdongid == undefined || iid_hopdongid == null || iid_hopdongid == "") return;
    //    $.ajax({
    //        type: "get",
    //        url: "/qlvondautu/qlkhoitaothongtinduan/getvdtnhathauandgiatrihdbyhopdong",
    //        data: { iid_hopdongid: iid_hopdongid },
    //        success: function (data) {
    //            $(tagtr).find(".stennhathau").html(data.sTenNhaThau);
    //            $(tagtr).find(".sgiatrihopdong").html(FormatNumber(data.fGiaTriHopDong));
    //        }
    //    });
    //});
    $(".hopdong").on("change", function () {
        var tagTr = this.closest("tr");
        $(tagTr).find(".sTenNhaThau").empty();
        $(tagTr).find(".SGiaTriHopDong").empty();

        var iID_HopDongID = $(this).val();
        if (iID_HopDongID == undefined || iID_HopDongID == null || iID_HopDongID == "") return;
        $.ajax({
            type: "Get",
            url: "/QLVonDauTu/QLKhoiTaoThongTinDuAn/GetVDTNhaThauAndGiaTriHDByHopDong",
            data: { iID_HopDongID: iID_HopDongID },
            success: function (data) {
                $(tagTr).find(".sTenNhaThau").html(data.sTenNhaThau);
                $(tagTr).find(".sGiaTriHopDong").html(FormatNumber(data.fGiaTriHopDong));
            }
        });
    });
}



