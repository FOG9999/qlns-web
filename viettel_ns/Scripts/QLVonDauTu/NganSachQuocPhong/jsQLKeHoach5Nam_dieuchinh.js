
var dataExclude = [1, 2, 3];
var iIdMax = 3;

//============================== Event Button ============================//
$(document).ready(function () {
    $(".setDisable").prop('disabled', true);

});

function ClearDataInsert() {
    $("#drpDuAn").prop("selectedIndex", 0);
    $("#selectDCChiPhi").prop("selectedIndex", 0);
    $("#txtValueBefore").val("");
    $("#txtGiaTriDieuChinh").val("");
    $("#txtSauDieuChinh").val("");
}

function GetDataChiTietToGridViewByNgayLap(id) {
    var dNgayLap = $('#txtNgayLap').val();
    var iGiaiDoanTu = parseInt( $("#iGiaiDoanTu").html());
    var iGiaiDoanDen = parseInt($("#iGiaiDoanDen").html());
    if (dNgayLap != "") {
        $.ajax({
            type: "POST",
            url: "/QLVonDauTu/KeHoach5Nam/GetListKhChiTietToGridView",
            data: { id: id, iGiaiDoanTu: iGiaiDoanTu, iGiaiDoanDen: iGiaiDoanDen, dNgayLap: dNgayLap },
            success: function (r) {
                if (r.data != null && r.data.length > 0) {
                    tblDataGrid = r.data;
                    iIdMax = tblDataGrid.length;
                    FormatNumberTBLChiTiet(tblDataGrid);
                    sumGiaTriDauTuVaVonUng(tblDataGrid);
                    FillDataToGridViewInsert(r.data);
                }
            }
        });
        $(".setDisable").prop('disabled', false);
    }
    
}


function InsertDetailData() {
    if ($("#drpDuAn").val() == null || $("#drpDuAn").val() == "") {
        alert("Chưa nhập thông tin chi tiết !");
        return;
    }
    if (parseFloat(UnFormatNumber( $("#txtSauDieuChinh").val())) < 0) {
        alert("Giá trị sau điều chỉnh không được nhỏ hơn 0 !");
        return;
    }
    var giaTriDieuChinh = parseInt(UnFormatNumber($("#txtGiaTriDieuChinh").val() == "" ? 0 : $("#txtGiaTriDieuChinh").val()));
    var loaiDieuChinh = $('#selectDCChiPhi').val();
    giaTriDieuChinh = giaTriDieuChinh * loaiDieuChinh;
    var data = {};
    iIdMax++;
    data.iId = iIdMax;
    data.iParentId = "1";
    data.sTenDuAn = $("#drpDuAn option:selected").text();
    data.fGiaTriKeHoach = $("#txtValueBefore").val() == "" ? 0 :$("#txtValueBefore").val();
    data.fGiaTriDieuChinh = FormatNumber(giaTriDieuChinh);
    data.fGiaTriSauDieuChinh = $("#txtSauDieuChinh").val() == "" ? 0 : $("#txtSauDieuChinh").val();
    data.sLoaiDieuChinh = $("#selectDCChiPhi option:selected").html();
    data.iID_DuAnID = $("#drpDuAn option:selected").val();
    tblDataGrid = $.map(tblDataGrid, function (n) { return n.iID_DuAnID != $("#drpDuAn option:selected").val() ? n : null });
    tblDataGrid.push(data);
    sumGiaTriDauTuVaVonUng(tblDataGrid);
    FillDataToGridViewInsert(tblDataGrid);
    ClearDataInsert();
    formatSoAm();
}

function sumGiaTriDauTuVaVonUng(tblDataGrid) {
    var tongGiaTri = 0;
    var tongGiaTriDC = 0;
    var tongGiaTriSauDC = 0;
    $.each(tblDataGrid, function (index, value) {
        if (value.iParentId == 1) {
            tongGiaTri += parseFloat(UnFormatNumber(value.fGiaTriKeHoach));
            tongGiaTriDC += parseFloat(UnFormatNumber(value.fGiaTriDieuChinh));
            tongGiaTriSauDC += parseFloat(UnFormatNumber(value.fGiaTriSauDieuChinh));
        }
    });

    $.each(tblDataGrid, function (index, value) {
        if (value.iId == 1) {
            value.fGiaTriKeHoach = FormatNumber(tongGiaTri);
            value.fGiaTriDieuChinh = FormatNumber(tongGiaTriDC);
            value.fGiaTriSauDieuChinh = FormatNumber(tongGiaTriSauDC);
        }
    });

    $("#tong_giatri").html(FormatNumber(tongGiaTri));
    $("#tong_giatriDC").html(FormatNumber(tongGiaTriDC));
    $("#tong_giatrisauDC").html(FormatNumber(tongGiaTriSauDC));
}

function Insert() {
    if (!ValidateDataInsert()) return;
    DieuChinhKeHoach5Nam();
}

function ValidateDataInsert() {
    var sMessError = [];
    if ($("#txtSoKeHoach").val().trim() == "") {
        sMessError.push("Chưa nhập số kế hoạch !");
    }
    if ($("#txtNgayLap").val().trim() == "") {
        sMessError.push("Chưa nhập ngày lập !");
    }

    var lstThongTinchiTiet = $.map(tblDataGrid, function (n) { return dataExclude.indexOf(n.iId) == -1 ? n : null });
    if (lstThongTinchiTiet.length == 0) {
        sMessError.push("Chưa nhập thông tin chi tiết !");
    }
    if (sMessError.length != 0) {
        alert(sMessError.join('\n'));
        return false;
    }
    return true;
}

function DieuChinhKeHoach5Nam() {
    var data = {};
    var listKHChiTiet = [];
    data.iID_KeHoach5NamID = $("#iID_KeHoach5NamID").val();
    data.sSoQuyetDinh = $("#txtSoKeHoach").val();
    data.dNgayQuyetDinh = $("#txtNgayLap").val();
    data.fGiaTriKeHoach = parseFloat(UnFormatNumber($("#tong_giatriDC").html()));
    
    tblDataGrid = $.map(tblDataGrid, function (n) { return n.iParentId == 1 ? n : null });
    $.each(tblDataGrid, function (index, value) {
        var dataKHChiTiet = {};
        dataKHChiTiet.iID_DuAnID = value.iID_DuAnID;
        dataKHChiTiet.fGiaTriKeHoach = UnFormatNumber(value.fGiaTriDieuChinh);

        listKHChiTiet.push(dataKHChiTiet);
    });

    data.listChiTiet = listKHChiTiet;

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoach5Nam/SaveDieuChinh",
        dataType: "json",
        data: {
            data: data
        },
        success: function (data) {
            if (data.status) {
                window.location.href = "/QLVonDauTu/KeHoach5Nam/Index";
            }
        }
    });
}

$("#drpDuAn").on("change", function () {
    if (this.value != "" || this.value != null) {
        $.ajax({
            url: "/QLVonDauTu/ChuTruongDauTu/LayThongTinChiTietDuAn",
            type: "POST",
            data: { iID: this.value },
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null) {
                    var txtBatDau = data.sKhoiCong == null ? "" : (data.sKhoiCong + "- ");
                    var txtKetThuc = data.sKetThuc == null ? "" : data.sKetThuc;
                    $('#txtThoiGianThucHien').html(txtBatDau + txtKetThuc);
                }
            },
            error: function (data) {

            }
        })
    }
});

function Huy() {
    location.href = "/QLVonDauTu/KeHoach5Nam/Index";
}

function FillDataToGridViewInsert(data) {
    var columns = [
        { sField: "iId", bKey: true },
        { sField: "iParentId", bParentKey: true },
        { sTitle: "Dự án", sField: "sTenDuAn", iWidth: "50%", sTextAlign: "left", bHaveIcon: 1 },
        { sTitle: "Giá trị trước điều chỉnh", sField: "fGiaTriKeHoach", iWidth: "10%", sTextAlign: "right" },
        { sTitle: "Loại điều chỉnh", sField: "sLoaiDieuChinh", iWidth: "10%", sTextAlign: "center" ,sClass: "c_loaiDC"},
        { sTitle: "Giá trị điều chỉnh", sField: "fGiaTriDieuChinh", iWidth: "10%", sTextAlign: "right", sClass :"c_giatriDC"},
        { sTitle: "Giá trị sau điều chỉnh", sField: "fGiaTriSauDieuChinh", iWidth: "10%", sTextAlign: "right" }];

    var button = { bCreateButtonInParent: false, bUpdate: 1, bDelete: 1 };
    var sHtml = GenerateTreeTable(data, columns, button)
    $("#ViewTable").html(sHtml);
    ClearButtonInParent();
}

function ClearButtonInParent() {
    var iIdExlude = [1, 2, 3];
    $("tr").each(function (index, value) {
        var dataRow = $(this).data("row");
        if (iIdExlude.indexOf(dataRow) != -1) {
            $(this).find("button").css("display", "none");
            $(this).find("i").removeClass("fa-file-text");
            $(this).find("i").addClass("fa-folder-open");
        }
    });
}

function GetItemData(iId) {
    var data = $.map(tblDataGrid, function (n) { return n.iId == iId ? n : null })[0];
    $("#drpDuAn").val(data.iID_DuAnID);
    $("#txtValueBefore").val(data.fGiaTriKeHoach);
    $("#txtSauDieuChinh").val(data.fGiaTriKeHoach);
}

function DeleteItem(iId) {
    tblDataGrid = $.map(tblDataGrid, function (n) { return n.iId != iId ? n : null });
    sumGiaTriDauTuVaVonUng(tblDataGrid);
    FillDataToGridViewInsert(tblDataGrid);
}

function FormatNumberTBLChiTiet(tblDataGrid) {
    $.each(tblDataGrid, function (index, value) {
        if (value.iParentId == 1) {
            value.fGiaTriKeHoach = value.fGiaTriKeHoach == 0 ? 0 : FormatNumber(value.fGiaTriKeHoach);
            value.fGiaTriSauDieuChinh = value.fGiaTriSauDieuChinh == 0 ? 0 : FormatNumber(value.fGiaTriSauDieuChinh);
        }
    });
}

function tinhGiaTriSauDieuChinh() {
    var giaTriTruoc = parseInt(UnFormatNumber($('#txtValueBefore').val() == "" ? 0 : $('#txtValueBefore').val()));
    var giaDieuChinh = parseInt(UnFormatNumber($('#txtGiaTriDieuChinh').val()));
    var loaiDieuChinh = $('#selectDCChiPhi').val();
    giaDieuChinh = giaDieuChinh * loaiDieuChinh;

    $('#txtSauDieuChinh').val(FormatNumber(giaTriTruoc + giaDieuChinh));
}

function formatSoAm() {
    $('#ViewTable .table .table-child .c_giatriDC').each(function () {
        var stringGiatriDC = $(this).html().trim("");
        if (parseFloat(UnFormatNumber(stringGiatriDC)) < 0) {
            $(this).html(stringGiatriDC.substring(1))
            $(this).addClass('dieuChinhTru');
        } else {
            $(this).removeClass('dieuChinhTru');
        }
    });

    var stringTongDC_DAChuyenTiep = $('#ViewTable .table-parent tr').eq(1).find('.c_giatriDC').html().trim("");
    if (parseFloat(UnFormatNumber(stringTongDC_DAChuyenTiep)) < 0) {
        $('#ViewTable .table-parent tr').eq(1).find('.c_giatriDC').html(stringTongDC_DAChuyenTiep.substring(1))
        $('#ViewTable .table-parent tr').eq(1).find('.c_giatriDC').addClass('dieuChinhTru');
    } else {
        $('#ViewTable .table-parent tr').eq(1).find('.c_giatriDC').removeClass('dieuChinhTru');
    }

    var stringTongDC = $("#tong_giatriDC").text().trim("");
    if (parseFloat(UnFormatNumber(stringTongDC)) < 0) {
        $("#tong_giatriDC").html(stringTongDC.substring(1))
        $("#tong_giatriDC").addClass('dieuChinhTru');
    } else {
        $("#tong_giatriDC").removeClass('dieuChinhTru');
    }

}

