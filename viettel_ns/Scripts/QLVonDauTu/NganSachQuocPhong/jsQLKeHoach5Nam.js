
var dataExclude = [1, 2, 3];
var iIdMax = 3;
const TRUNG_SO_KE_HOACH = 0;
const TRUNG_GIAI_DOAN = 1;
var type_trung = 2;
//============================== Event Button ============================//
$(document).ready(function () {

    $("#txtGiaiDoanTu").keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            return false;
        }
    });

    $("#txtGiaiDoanDen").keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            return false;
        }
    });

    $("#iID_DonViQuanLyID").change(function () {
        var dNgayLap = $("#txtNgayLap").val();
        if (this.value != "" && this.value != null && dNgayLap != "") {
            $.ajax({
                url: "/QLVonDauTu/KeHoach5Nam/ListDuAnTheoDonViQLAndNgayLap",
                type: "POST",
                data: { iID_DonViQuanLyID: this.value, dNgayLap: dNgayLap },
                dataType: "json",
                cache: false,
                success: function (data) {
                    $("#drpDuAn").html(data);
                },
                error: function (data) {

                }
            })
        }
    });

    $("#txtNgayLap").change(function () {
        $("#iID_DonViQuanLyID").trigger("change");
    });


    $("#txtGiaiDoanTu").change(function () {
        if (this.value == "") {
            $("#txtGiaiDoanDen").val("");
            $("#txtGiaiDoanDen").trigger("change");
            return;
        }
        var giaiDoanTu = parseInt($("#txtGiaiDoanTu").val());
        $("#txtGiaiDoanDen").val(giaiDoanTu + 4);
        $("#txtGiaiDoanDen").trigger("change");
    });

    $("#txtGiaiDoanDen").change(function () {
        var giaiDoanTu = $("#txtGiaiDoanTu").val();
        if (this.value != "" && giaiDoanTu != "") {
            $.ajax({
                url: "/QLVonDauTu/KeHoach5Nam/ListLoaiNganSach",
                type: "POST",
                data: { giaiDoanTu: giaiDoanTu, giaiDoanDen: this.value },
                dataType: "json",
                cache: false,
                success: function (data) {
                    $("#drpLoaiNganSach").html(data);
                },
                error: function (data) {

                }
            })
        }
    });


});


function ClearDataInsert() {
    $("#drpDuAn").prop("selectedIndex", 0);
    $("#txtGiaTriKeHoach").val("");
    $("#txtThoiGianThucHien").html("");
    $("#txtGhiChu").val("");
}

function InsertDetailData() {
    if ($("#drpDuAn").val() == null || $("#drpDuAn").val() == "") {
        alert("Chưa nhập thông tin chi tiết !");
        return;
    }
    var data = {};
    iIdMax++;
    data.iId = iIdMax;
    data.iParentId = "1";
    /*data.sXauNoiMa = $("#drpNganh option:selected").text();*/
    /*data.sMaKetNoi = $("#txtMaKetNoi").val();*/
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
    $.each(tblDataGrid, function (index, value) {
        if (value.iParentId == 1) {
            tongGiaTri += parseFloat(UnFormatNumber(value.fGiaTriKeHoach));

        }

    });

    $.each(tblDataGrid, function (index, value) {
        if (value.iId == 1) {
            value.fGiaTriKeHoach = FormatNumber(tongGiaTri);
        }
    });

    $("#tong_giatri").html(FormatNumber(tongGiaTri));
}

function Insert() {
    if (!ValidateDataInsert()) return;
    CreateKeHoach5Nam();
}

function ValidateDataInsert() {
    var sMessError = [];
    if ($("#iID_DonViQuanLyID").val() == "") {
        sMessError.push("Chưa nhập đơn vị quản lý !");
    }
    if ($("#txtSoKeHoach").val().trim() == "") {
        sMessError.push("Chưa nhập số kế hoạch !");
    }
    if ($("#txtNgayLap").val().trim() == "") {
        sMessError.push("Chưa nhập ngày lập !");
    }
    if ($("#txtAddNvdtNguonVon").val() == null) {
        sMessError.push("Chưa nhập nguồn vốn !");
    }
    if ($("#drpLoaiNganSach").val() == null) {
        sMessError.push("Chưa nhập loại ngân sách !");
    }
    if ($("#txtGiaiDoanTu").val() == "" || $("#txtGiaiDoanDen").val() == "") {
        sMessError.push("Chưa nhập giai đoạn !");
    }
    var lstThongTinchiTiet = $.map(tblDataGrid, function (n) { return dataExclude.indexOf(n.iId) == -1 ? n : null });
    if (lstThongTinchiTiet.length == 0) {
        sMessError.push("Chưa nhập thông tin chi tiết !");
    }
    if (checkTrung()) {
        if (type_trung == TRUNG_GIAI_DOAN)
            sMessError.push("Trùng giai đoạn!");
        else (type_trung == TRUNG_SO_KE_HOACH)
            sMessError.push("Trùng số kế hoạch!");
    }
    if (sMessError.length != 0) {
        alert(sMessError.join('\n'));
        return false;
    }

    return true;
}

function checkTrung() {
    var donVi = $("#iID_DonViQuanLyID").val();
    var giaiDoanTu = $("#txtGiaiDoanTu").val();
    var giaiDoanDen = $("#txtGiaiDoanDen").val();
    var soKeHoach = $("#txtSoKeHoach").val();
    var check = false;
    if (donVi != "" && giaiDoanTu != "" && giaiDoanDen != "") {
        $.ajax({
            type: "POST",
            url: "/QLVonDauTu/KeHoach5Nam/CheckDuplicate",
            dataType: "json",
            async: false,
            cache: false,
            data: {
                iDDonVi: donVi, giaiDoanTu: giaiDoanTu, giaiDoanDen: giaiDoanDen, soKeHoach: soKeHoach
            },
            success: function (r) {
                check = r.result;
                type_trung = r.error;
            }
        });
    }
    return check;

}

function CreateKeHoach5Nam() {
    var data = {};
    data.iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
    data.sSoQuyetDinh = $("#txtSoKeHoach").val();
    data.dNgayQuyetDinh = $("#txtNgayLap").val();
    data.iGiaiDoanTu = $("#txtGiaiDoanTu").val();
    data.iGiaiDoanDen = $("#txtGiaiDoanDen").val();
    data.iID_NguonVonID = $("#txtAddNvdtNguonVon").val();
    data.iID_LoaiNganSachID = $("#drpLoaiNganSach").find(':selected').val();
    //data.iID_KhoanNganSachID = $("#drpLoaiKhoan").val();
    data.fGiaTriKeHoach = parseFloat(UnFormatNumber($("#tong_giatri").html()));

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoach5Nam/AddKeHoach5Nam",
        dataType: "json",
        data: {
            data: data
        },
        success: function (r) {
            if (r.bIsComplete) {
                AddKeHoachChiTiet(r.data);
            } else {
                alert("Phân bổ vốn thất bại !");
            }
        }
    });
}

function AddKeHoachChiTiet(iParentId) {
    var data = [];
    $.each(tblDataGrid, function (index, value) {
        if (dataExclude.indexOf(value.iId) != -1) { return; }
        var item = {};
        item.iID_KeHoach5NamID = iParentId;
        item.iID_DuAnID = value.iID_DuAnID;
        item.fGiaTriKeHoach = parseFloat(UnFormatNumber(value.fGiaTriKeHoach));
        item.sGhiChu = value.sGhiChu;
        item.sTrangThai = 1;
        data.push(item);
    });
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoach5Nam/AddKeHoach5NamChiTiet",
        data: {
            lstData: data
        },
        success: function (r) {
            if (r.bIsComplete) {
                location.href = "/QLVonDauTu/KeHoach5Nam/Index";
            }
            else {
                alert("Tạo mới thất bại !");
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
    var loaiNS = $('#drpLoaiNganSach').find(':selected').data('sl');
    if (loaiNS != null && loaiNS != "") {
        $.ajax({
            type: "POST",
            url: "/KeHoach5Nam/GetThongTinLoaiKhoan",
            data: {
                sLNS: loaiNS
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

}

function Huy() {
    location.href = "/QLVonDauTu/KeHoach5Nam/Index";
}

//=============================== Gridview ==============================//

function FillDataToGridViewInsert(data) {
    var columns = [
        { sField: "iId", bKey: true },
        { sField: "iParentId", bParentKey: true },
        { sTitle: "Mã kết nối", sField: "sMaKetNoi", iWidth: "10%", sTextAligsn: "left" },
        { sTitle: "Dự án", sField: "sTenDuAn", iWidth: "40%", sTextAlign: "left", bHaveIcon: 1 },
        { sTitle: "Thời gian thực hiện", sField: "sThoiGianThucHien", iWidth: "10%", sTextAlign: "left" },
        { sTitle: "Giá trị kế hoạch", sField: "fGiaTriKeHoach", iWidth: "10%", sTextAlign: "right" },
        { sTitle: "Ghi chú", sField: "sGhiChu", iWidth: "20%", sTextAlign: "left" }];

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
    $("#txtThoiGianThucHien").html(data.sThoiGianThucHien);
    $("#txtGiaTriKeHoach").val(data.fGiaTriKeHoach);
    $("#txtGhiChu").val(data.sGhiChu);
}

function DeleteItem(iId) {
    tblDataGrid = $.map(tblDataGrid, function (n) { return n.iId != iId ? n : null });
    sumGiaTriDauTuVaVonUng(tblDataGrid);
    FillDataToGridViewInsert(tblDataGrid);
}


