
var dataExclude = [1, 2, 3];

//============================== Event Button ============================//

function ClearDataInsert() {
    $("#drpDuAn").prop("selectedIndex", 0);
    $("#txtGiaTriKeHoach").val("");
    $("#txtGhiChu").val("");
}

function InsertDetailData() {
    if ($("#drpDuAn").val() == null || $("#drpDuAn").val() == "") {
        alert("Chưa nhập thông tin chi tiết !");
        return;
    }
    var data = {};
    data.iId = tblDataGrid.length + 1;
    data.iParentId = "1";
    data.sKhoiCong = $("#txtKhoiCong").val();
    data.sTenDuAn = $("#drpDuAn option:selected").text();
    data.sThoiGianThucHien = $("#txtThoiGianThucHien").html();
    data.fGiaTriKeHoach = $("#txtGiaTriKeHoach").val();
    data.sGhiChu = $("#txtGhiChu").val();
    data.iID_DuAnID = $("#drpDuAn option:selected").val();
    tblDataGrid = $.map(tblDataGrid, function (n) { return n.iID_DuAnID != data.iID_DuAnID ? n : null });
    tblDataGrid.push(data);
    sumGiaTriDauTuVaVonUng(tblDataGrid);
    FillDataToGridViewInsert(tblDataGrid);
    ClearDataInsert();
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
    EditKeHoach5Nam();
}

function ValidateDataInsert() {
    var sMessError = [];
    if ($("#txtSoKeHoach").val().trim() == "") {
        sMessError.push("Chưa nhập số kế hoạch !");
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

function EditKeHoach5Nam() {
    var data = {};
    var listKHChiTiet = [];
    data.iID_KeHoach5NamID = $("#iID_KeHoach5NamID").val();
    data.sSoQuyetDinh = $("#txtSoKeHoach").val();
    data.fGiaTriKeHoach = parseFloat(UnFormatNumber($("#tong_giatri").html()));

    tblDataGrid = $.map(tblDataGrid, function (n) { return n.iParentId == 1 ? n : null });
    $.each(tblDataGrid, function (index, value) {
        var dataKHChiTiet = {};
        dataKHChiTiet.iID_DuAnID = value.iID_DuAnID;
        dataKHChiTiet.fGiaTriKeHoach = UnFormatNumber(value.fGiaTriKeHoach);

        listKHChiTiet.push(dataKHChiTiet);
    });

    data.listChiTiet = listKHChiTiet;

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoach5Nam/EditKeHoach5Nam",
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

function GetDataDropdownLoaiAndKhoanByLoaiNganSach() {
    $.ajax({
        type: "POST",
        url: "/KeHoach5Nam/GetThongTinLoaiKhoan",
        data: {
            sLNS: $("#drpLoaiNganSach").val().split("|")[0]
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#drpLoaiKhoan").empty()
                $.each(r.data, function (index, value) {
                    $("#drpLoaiKhoan").append("<option value='" + value.Value + "'>" + value.Text + "<option>");
                });
                $("#drpLoaiKhoan").prop("selectedIndex", -1);
            }
        }
    });
}

function Huy() {
    location.href = "/QLVonDauTu/KeHoach5Nam/Index";
}

//=============================== Gridview ==============================//

function FillDataToGridViewInsert(data) {
    var columns = [
        { sField: "iId", bKey: true },
        { sField: "iParentId", bParentKey: true },
        { sTitle: "Dự án", sField: "sTenDuAn", iWidth: "500px", sTextAlign: "left", bHaveIcon: 1 },
        { sTitle: "Giá trị trước điều chỉnh", sField: "fGiaTriKeHoach", iWidth: "10%", sTextAlign: "right" },
        { sTitle: "Loại điều chỉnh", sField: "sLoaiDieuChinh", iWidth: "10%", sTextAlign: "left" },
        { sTitle: "Giá trị điều chỉnh", sField: "fGiaTriDieuChinh", iWidth: "10%", sTextAlign: "right" },
        { sTitle: "Giá trị sau điều chỉnh", sField: "fGiaTriSauDieuChinh", iWidth: "10%", sTextAlign: "right" }];

    var button = { bCreateButtonInParent: false };
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
    $("#txtThoiGianThucHien").html(data.sThoiGianThucHien);
    $("#txtGiaTriKeHoach").val(data.fGiaTriKeHoach);
    $("#txtGhiChu").val(data.sGhiChu);
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
            value.fGiaTriDieuChinh = value.fGiaTriDieuChinh == 0 ? 0 : FormatNumber(value.fGiaTriDieuChinh);
            value.fGiaTriSauDieuChinh = value.fGiaTriSauDieuChinh == 0 ? 0 : FormatNumber(value.fGiaTriSauDieuChinh);
        }
    });
}




