
$(".currently").trigger("change");

function GetListData(iIdDonViQuanLy, iIdNguonVon, dNgayDeNghiFrom, dNgayDeNghiTo, iNamKeHoach, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QuyetToanNienDoTroLy/QuyetToanNienDoTroLyView",
        data: {
            iIdDonViQuanLy: iIdDonViQuanLy,
            iIdNguonVon: iIdNguonVon,
            dNgayDeNghiFrom: dNgayDeNghiFrom,
            dNgayDeNghiTo: dNgayDeNghiTo,
            iNamKeHoach: iNamKeHoach,
            _paging: _paging
        },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#drpDonViQuanLy").val(iIdDonViQuanLy);
            $("#drpNguonNganSach").val(iIdNguonVon);
            $("#txtNgayLapFrom").val(dNgayDeNghiFrom);
            $("#txtNgayLapTo").val(dNgayDeNghiTo);
            $("#txtNamKeHoach").val(iNamKeHoach);
        }
    });
}

function GetQuyetToanNienDoChiTiet() {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QuyetToanNienDoTroLy/GetQuyetToanNienDoChiTietByParentId",
        data: {
            iId: $("#iID_DeNghiQuyetToanNienDoID").val()
        },
        success: function (r) {
            ConvertListDetailToGrid(r.data);
        }
    });
}

function ChangePage(iCurrentPage = 1) {
    var iIdDonViQuanLy = $("#drpDonViQuanLy option:selected").val();
    var iIdNguonVon = $("#drpNguonNganSach option:selected").val();
    var dNgayDeNghiFrom = $("#txtNgayLapFrom").val();
    var dNgayDeNghiTo = $("#txtNgayLapTo").val();
    var iNamKeHoach = $("#txtNamKeHoach").val();

    GetListData(iIdDonViQuanLy, iIdNguonVon, dNgayDeNghiFrom, dNgayDeNghiTo, iNamKeHoach, iCurrentPage);
}


function InsertItemToGridView() {
    if (!ValidateDetailInsert()) { return; }
    maxId++;
    var item = {};
    item.iId = maxId;
    item.iParentId = 1;
    item.iID_NganhID = $("#drpNganh option:selected").val();
    item.sNganh = $("#drpNganh option:selected").text();
    item.iId_LoaiCongTrinh = $("#drpLoaicongTrinh").val();
    item.iID_DuAnID = $("#drpDuAn option:selected").val().split("|")[0];
    item.sTenDuAn = $("#drpDuAn option:selected").text();
    item.fTongMucDauTuPheDuyet = $("#txtTongMucDauTuPheDuyet").val();
    item.fChiTieuNganSachNam = $("#txtChiTieuNganSachNam").val();
    item.fCapPhatVonNamTruoc = $("#txtCapPhatVonNamTruoc").val();
    item.fDonViDeNghiCapPhatVonNamTruoc = $("#txtDonViDeNghiCapPhatVonNamTruoc").val();
    item.fTroLyDeNghiCapPhatVonNamTruoc = $("#txtTroLyDeNghiCapPhatVonNamTruoc").val();
    item.fChiTieuNganSachNamNay = $("#txtChiTieuNganSachNamNay").val();
    item.fCapPhatVonNamNay = $("#txtCapPhatVonNamNay").val();
    item.fDonViDeNghiCapPhatVonNamNay = $("#txtDonViDeNghiCapPhatVonNamNay").val();
    item.fTroLyDeNghiCapPhatVonNamNay = $("#txtTroLyDeNghiCapPhatVonNamNay").val();

    tblDataGrid = $.map(tblDataGrid, function (n) { return n.iID_DuAnID == item.iID_DuAnID && n.iID_NganhID == item.iID_NganhID ? null : n });

    tblDataGrid.push(item);
    RenderGridViewInsert(tblDataGrid);
    ClearDataDetail();
}

//====================== Dropdown event ========================//
function GetDataDropdownLoaiNganSach(isEdit) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKeHoachVonNam/GetDataDropdownLoaiNganSach",
        data: {
            iNamKeHoach: $("#txtNamKeHoach").val()
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#drpLoaiNganSach").empty()
                $.each(r.data, function (index, value) {
                    $("#drpLoaiNganSach").append("<option value='" + value.Value + "'>" + value.Text + "</option>");
                });
                $("#drpLoaiNganSach").prop("selectedIndex", -1);

                if (isEdit) {
                    var itemChoose = $.map(r.data, function (n) { return n.Value.indexOf($("#drpLoaiNganSachEdit").val()) != -1 ? n.Value : null })[0];
                    $("#drpLoaiNganSach").val(itemChoose);
                    $("#drpLoaiNganSach").change();
                }
            }
        }
    });
}

function GetDataDropdownNganh(isEdit) {
    if ($("#drpLoaiNganSach").val() == undefined) return;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QuyetToanNienDoTroLy/GetDataThongTinNganhByLoaiNganSach",
        data: {
            sLNS: $("#drpLoaiNganSach").val().split("|")[0],
            iNamKeHoach: $("#txtNamKeHoach").val()
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#drpNganh").empty();
                $.each(r.data, function (index, value) {
                    $("#drpNganh").append("<option value='" + value.Value + "'>" + value.Text + "</option>");
                });
                $("#drpNganh").prop("selectedIndex", -1);
                if (isEdit) {
                    $("#drpNganh").val($("#drpNganhOld").val());
                    GetDataDropdownDuAn(isEdit);
                }
            }
        }
    });
}

function GetDataDropdownDuAn(isEdit) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QuyetToanNienDoTroLy/GetDataDropDownDuAn",
        data: {
            iID_DonViQuanLyID: $("#drpDonViQuanLy option:selected").val(),
            iID_NguonVonID: $("#drpNguonNganSach option:selected").val(),
            iID_LoaiNguonVonID: $("#drpLoaiNganSach option:selected").val().split("|")[1],
            dNgayQuyetDinh: $("#txtNgayPheDuyet").val(),
            iNamKeHoach: $("#txtNamKeHoach").val(),
            iID_NganhID: $("#drpNganh option:selected").val()
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#drpDuAn").empty()
                $.each(r.data, function (index, value) {
                    $("#drpDuAn").append("<option value='" + value.Value + "'>" + value.Text + "</option>");
                });
                $("#drpDuAn").prop("selectedIndex", -1);

                if (isEdit && r.data.length != 0) {
                    var itemChoose = $.map(r.data, function (n) { return n.Value.indexOf($("#drpDuAnOld").val()) != -1 ? n.Value : null })[0];
                    $("#drpDuAn").val(itemChoose);
                    GetDetailDuAn();
                }
            }
        }
    });
}

function GetDropdownLoaiCongTrinh() {
    $.ajax({
        url: "/QLVonDauTu/ChuTruongDauTu/FillDropdown",
        type: "POST",
        data: "",
        dataType: "json",
        cache: false,
        success: function (data) {
            trLoaiCongTrinh = $('#txtLoaiCongTrinh').comboTree({
                source: data,
                isMultiple: false,
                cascadeSelect: false,
                collapse: true,
                selectableLastNode: false
            });
            trLoaiCongTrinh.onChange(function () {
                $("#drpLoaicongTrinh").val(trLoaiCongTrinh.getSelectedIds());
            });
        },
        error: function (data) {

        }
    });
}

function GetDetailDuAn() {
    var sMessError = [];
    if ($("#drpDonViQuanLy option:selected").val() == null) {
        sMessError.push("Chưa chọn đơn vị quản lý !");
    }
    if ($("#drpNguonNganSach option:selected").val() == null) {
        sMessError.push("Chưa chọn nguồn vốn !");
    }
    if ($("#drpLoaiNganSach option:selected").val() == null) {
        sMessError.push("Chưa chọn loại nguồn vốn !");
    }
    if (sMessError.length != 0) {
        alert(sMessError.join("\n"));
        return;
    }

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QuyetToanNienDoTroLy/GetDetailDuAn",
        data: {
            iIdDonViQuanLyId: $("#drpDonViQuanLy option:selected").val(),
            iIdNguonVonId: $("#drpNguonNganSach option:selected").val(),
            iIdLoaiNguonVon: $("#drpLoaiNganSach option:selected").val().split("|")[1],
            dNgayQuyetDinh: $("#txtNgayPheDuyet").val(),
            iNamKeHoach: $("#txtNamKeHoach").val(),
            iIdNganh: $("#drpNganh option:selected").val(),
            iIdDuAnId: $("#drpDuAn").val().split("|")[0]
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#txtCapPhatVonNamNay").val(r.fCapPhatVonNamNay.toLocaleString('vi-VN'));
                $("#txtChiTieuNganSachNamNay").val(r.fChiTieuNganSachNam.toLocaleString('vi-VN'));
                $("#txtTongMucDauTuPheDuyet").val(r.fTongMucDauTuPheDuyet.toLocaleString("vi-VN"))
            }
        }
    });
}

//===================== Table ================================//

function RenderGridViewInsert(data) {
    var columns = [
        { sField: "iId", bKey: true },
        { sField: "iIdRefer", bForeignKey: true },
        { sField: "iParentId", bParentKey: true },
        { sTitle: "M-TM-TM-N", sField: "sNganh", iWidth: "200px", sTextAlign: "left" },
        { sTitle: "Dự án", sField: "sTenDuAn", iWidth: "500px", sTextAligsn: "left", bHaveIcon: 1 },
        { sTitle: "Tổng mức đầu tư", sField: "fTongMucDauTuPheDuyet", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Luỹ kế vốn thanh toán từ K/c đến hết niên độ năm trước", sField: "fLuyKeThanhToanKLHT", iWidth: "200px", sTextAlign: "right" },

        { sTitle: "Chỉ tiêu năm trước chuyển đã cấp", sField: "fChiTieuNganSachNam", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Cấp phát vốn", sField: "fCapPhatVonNamTruoc", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Giá trị đề nghị của đơn vị", sField: "fDonViDeNghiCapPhatVonNamTruoc", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Giá trị đề nghị của trợ lý", sField: "fTroLyDeNghiCapPhatVonNamTruoc", iWidth: "200px", sTextAlign: "right" },

        { sTitle: "Chỉ tiêu ngân sách năm nay", sField: "fChiTieuNganSachNamNay", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Cấp phát vốn", sField: "fCapPhatVonNamNay", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Giá trị đề nghị của đơn vị", sField: "fDonViDeNghiCapPhatVonNamNay", iWidth: "300px", sTextAlign: "right" },
        { sTitle: "Giá trị đề nghị của trợ lý", sField: "fTroLyDeNghiCapPhatVonNamNay", iWidth: "300px", sTextAlign: "right" }];


    var button = { bCreateButtonInParent: false, bUpdate: 1, bDelete: 1 };
    var sHtml = GenerateTreeTable(data, columns, button)
    $("#ViewTable").css("width", $("#ViewTable").width() + "px");
    $("#ViewTable").html(sHtml);
    AddColspanTable();
    ClearButtonInParent();
}

function CreateDefaultTable() {
    tblDataGrid.push({ iIdRefer: 1, iId: 1, sTenDuAn: "Dự án chuyển tiếp" });
    tblDataGrid.push({ iIdRefer: 2, iId: 2, sTenDuAn: "Dự án khởi công mới" });
    tblDataGrid.push({ iIdRefer: 3, iId: 3, sTenDuAn: "Dự án kết thúc đầu tư" });
}

function ConvertListDetailToGrid(data) {
    tblDataGrid = [];
    CreateDefaultTable();
    $.each(data, function (index, value) {
        maxId++;
        var item = {};
        item.iId = maxId;
        item.iParentId = 1;
        item.iID_NganhID = value.iID_NganhID;
        item.sNganh = value.sXauNoiChuoi;
        item.iID_DuAnID = value.iID_DuAnID
        item.sTenDuAn = value.sTenDuAn
        item.fTongMucDauTuPheDuyet = value.fTongMucDauTuPheDuyet;
        item.fChiTieuNganSachNam = 0;
        item.fCapPhatVonNamTruoc = 0;
        item.fDonViDeNghiCapPhatVonNamTruoc = value.fGiaTriQuyetToanNamTruocDonVi.toLocaleString('vi-VN');
        item.fTroLyDeNghiCapPhatVonNamTruoc = value.fGiaTriQuyetToanNamTruoc.toLocaleString('vi-VN');
        item.fChiTieuNganSachNamNay = value.fChiTieuNganSachNam.toLocaleString('vi-VN');
        item.fCapPhatVonNamNay = value.fCapPhatVonNamNay.toLocaleString('vi-VN');
        item.fDonViDeNghiCapPhatVonNamNay = value.fGiaTriQuyetToanNamNayDonVi.toLocaleString('vi-VN');
        item.fTroLyDeNghiCapPhatVonNamNay = value.fGiaTriQuyetToanNamNay.toLocaleString('vi-VN');

        tblDataGrid = $.map(tblDataGrid, function (n) { return n.iID_DuAnID == item.iID_DuAnID && n.iID_NganhID == item.iID_NganhID ? null : n });

        tblDataGrid.push(item);
        RenderGridViewInsert(tblDataGrid);
        ClearDataDetail();
        $("#ViewTable button").css("display", "none");
    })
}

//===================== event button =========================//
function ClearDataDetail() {
    trLoaiCongTrinh.clearSelection();
    $("#drpNganh").prop("selectedIndex", -1);
    $("#drpDuAn").prop("selectedIndex", -1);
    $("#txtChiTieuNganSachNam").val("");
    $("#txtCapPhatVonNamTruoc").val("");
    $("#txtChiTieuNganSachNamNay").val("");
    $("#txtCapPhatVonNamNay").val("");
    $("#txtDonViDeNghiCapPhatVonNamTruoc").val("");
    $("#txtTroLyDeNghiCapPhatVonNamTruoc").val("");
    $("#txtDonViDeNghiCapPhatVonNamNay").val("");
    $("#txtTroLyDeNghiCapPhatVonNamNay").val("");
}

function CancelSaveData() {
    location.href = "/QLVonDauTu/QuyetToanNienDoTroLy/";
}

function BtnInsertDataClick() {
    location.href = "/QLVonDauTu/QuyetToanNienDoTroLy/Insert";
}

function Insert() {
    debugger
    data = {};
    lstData = [];

    if (!ValidateDataInsert()) return;

    data.sSoDeNghi = $("#txtSoPheDuyet").val();
    data.dNgayDeNghi = $("#txtNgayPheDuyet").val();
    data.iID_DonViDeNghiID = $("#drpDonViQuanLy").val();
    data.sNguoiDeNghi = $("#txtNguoiLap").val();
    data.iNamKeHoach = $("#txtNamKeHoach").val();
    data.iID_NguonVonID = $("#drpNguonNganSach").val();
    data.iID_LoaiNguonVonID = $("#drpLoaiNganSach").val().split("|")[1];

    $.each($.map(tblDataGrid, function (n) { return dataExclude.indexOf(n.iId) != -1 ? null : n }), function (index, item) {
        var itemData = {};
        itemData.iID_DuAnID = item.iID_DuAnID
        itemData.iID_NganhID = item.iID_NganhID
        itemData.fGiaTriQuyetToanNamTruocDonVi = item.fDonViDeNghiCapPhatVonNamTruoc.replaceAll(".", "");
        itemData.fGiaTriQuyetToanNamNayDonVi = item.fDonViDeNghiCapPhatVonNamNay.replaceAll(".", "");
        itemData.fGiaTriQuyetToanNamTruoc = item.fTroLyDeNghiCapPhatVonNamTruoc.replaceAll(".", "");
        itemData.fGiaTriQuyetToanNamNay = item.fTroLyDeNghiCapPhatVonNamNay.replaceAll(".", "");
        lstData.push(itemData);
    });


    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QuyetToanNienDoTroLy/InsertNienDoTroLy",
        data: {
            data: data,
            lstDataDetail: lstData
        },
        success: function (r) {
            if (r.bIsComplete) {
                alert("Thêm mới thành công !")
                location.href = "/QLVonDauTu/QuyetToanNienDoTroLy";
            } else {
                alert("Có lỗi xảy ra trong quá trình thêm mới !");
            }
        }
    });
}

function GetItemDataList(id) {
    location.href = "/QLVonDauTu/QuyetToanNienDoTroLy/Update/" + id;
}

function DeleteItemList(id) {
    if (!confirm("Chấp nhận xóa bản ghi ?")) return;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QuyetToanNienDoTroLy/DeleteQuyetToanNienDoTroLy",
        data: { id: id },
        success: function (r) {
            if (r.data) {
                ChangePage();
            }
        }
    });
}

//===================== update css ===========================//
function AddColspanTable() {
    var index = 0;
    $(".table-parent th").each(function () {
        ++index;
        if (index == 6) {
            $(this).attr('colspan', 4);
            $(this).html("Trợ lý đề nghị quyết toán niên độ năm trước")
            return true;
        } else if (index == 7) {
            $(this).attr('colspan', 4);
            $(this).html("Trợ lý đề nghị quyết toán niên độ năm nay")
            return true;
        }
        if (index >= 8 && index <= 13) {
            $(this).remove();
            return true;
        }
        $(this).attr('rowspan', 2);
    });
    var row = [];
    row.push("<tr>");
    row.push("  <th width = '200px' > Chỉ tiêu năm trước chuyển đã cấp</th >");
    row.push("  <th width = '200px' > Cấp phát vốn</th >");
    row.push("  <th width = '200px' > Giá trị đề nghị của đơn vị</th >");
    row.push("  <th width = '200px' > Giá trị đề nghị của trợ lý</th >");
    row.push("  <th width = '200px' > Chỉ tiêu ngân sách năm nay</th >");
    row.push("  <th width = '200px' > Cấp phát vốn</th >");
    row.push("  <th width = '200px' > Giá trị đề nghị của đơn vị</th >");
    row.push("  <th width = '200px' > Giá trị đề nghị của trợ lý</th >");
    row.push("</tr>");

    $(".table-parent tr:first").after(row.join(""));
    $(".table-parent .fa-caret-down").closest("tr").find("[type=button]").remove();
}

function ClearButtonInParent() {
    var iIdExlude = [1, 2, 3];
    $("tr").each(function (index, value) {
        var dataRow = $(this).data("row");
        if (iIdExlude.indexOf(dataRow) != -1) {
            $(this).find("button").remove();
            $(this).find("i").removeClass("fa-file-text");
            $(this).find("i").addClass("fa-folder-open");
        }
    });
}

function CancelSaveThongTinChung() {
    $("#txtSoPheDuyet").val($("#txtSoPheDuyetEdit").val());
    $("#txtNgayPheDuyet").val($("#txtNgayPheDuyetEdit").val());
    $("#txtNguoiLap").val($("#txtNguoiLapEdit").val());
    EditThongTinChung_Off();
}

function EditThongTinChung_On() {
    $("button.show-edit-on").css("display", "");
    $("button.show-edit-off").css("display", "none");

    $("#txtSoPheDuyet").removeAttr("disabled");
    $("#txtNgayPheDuyet").removeAttr("disabled");
    $("#txtNguoiLap").removeAttr("disabled");
}

function EditThongTinChung_Off() {
    $("button.show-edit-on").css("display", "none");
    $("button.show-edit-off").css("display", "");

    $("#txtSoPheDuyet").attr("disabled", "disabled")
    $("#txtNgayPheDuyet").attr("disabled", "disabled")
    $("#txtNguoiLap").attr("disabled", "disabled")
}

function EditThongTinChiTiet_Off() {
    $("button.show-edit-detail-on").css("display", "none");
    $("button.show-edit-detail-off").css("display", "");

    $("#pnlThongTinChiTiet").css("display", "none");
    $("#ViewTable button").css("display", "none");
}

function EditThongTinChiTiet_On() {
    $("button.show-edit-detail-on").css("display", "");
    $("button.show-edit-detail-off").css("display", "none");

    $("#pnlThongTinChiTiet").css("display", "");
    $("#ViewTable button").css("display", "");
}

function CancelSaveThongChiTiet() {
    EditThongTinChiTiet_Off();
    GetQuyetToanNienDoChiTiet();
}

function SaveThongTinChung() {
    data = {};
    data.iID_DeNghiQuyetToanNienDoID = $("#iID_DeNghiQuyetToanNienDoID").val();
    data.sSoDeNghi = $("#txtSoPheDuyet").val();
    data.dNgayDeNghi = $("#txtNgayPheDuyet").val();
    data.sNguoiDeNghi = $("#txtNguoiLap").val();
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QuyetToanNienDoTroLy/UpdateQuyetToanNienDoTroLy",
        data: {
            data: data
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#txtSoPheDuyetEdit").val(data.sSoPheDuyet);
                $("#txtNgayPheDuyetEdit").val(data.sSoPheDuyet);
                $("#txtNguoiLapEdit").val(data.sSoPheDuyet);
                EditThongTinChung_Off();
            }
        }
    });
}

function SaveThongTinChiTiet() {
    lstData = [];
    $.each($.map(tblDataGrid, function (n) { return dataExclude.indexOf(n.iId) != -1 ? null : n }), function (index, item) {
        var itemData = {};
        itemData.iID_DuAnID = item.iID_DuAnID
        itemData.iID_NganhID = item.iID_NganhID
        itemData.fGiaTriQuyetToanNamTruocDonVi = item.fDonViDeNghiCapPhatVonNamTruoc.replaceAll(".", "");
        itemData.fGiaTriQuyetToanNamNayDonVi = item.fDonViDeNghiCapPhatVonNamNay.replaceAll(".", "");
        itemData.fGiaTriQuyetToanNamTruoc = item.fTroLyDeNghiCapPhatVonNamTruoc.replaceAll(".", "");
        itemData.fGiaTriQuyetToanNamNay = item.fTroLyDeNghiCapPhatVonNamNay.replaceAll(".", "");
        lstData.push(itemData);
    });

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QuyetToanNienDoTroLy/UpdateQuyetToanNienDoTroLyChiTiet",
        data: {
            iId: $("#iID_DeNghiQuyetToanNienDoID").val(),
            data: lstData
        },
        success: function (r) {
            if (r.bIsComplete) {
                location.href = "/QLVonDauTu/QuyetToanNienDoTroLy/";
            } else {
                alert("Có lỗi xảy ra trong quá trình cập nhật !");
            }
        }
    });
}

//===================== validate ===========================//

function ValidateDetailInsert() {
    var sMessError = [];
    if ($("#drpNganh").val() == null) {
        sMessError.push("Chưa nhập Mục - Tiểu mục - Tiết mục - Ngành !");
    }
    if ($("#drpDuAn").val() == null) {
        sMessError.push("Chưa nhập dự án !");
    }
    if ($("#txtDonViDeNghiCapPhatVonNamNay").val() == null) {
        sMessError.push("Chưa nhập Đơn vị đề nghị quyết toán năm nay !");
    }
    if ($("#txtTroLyDeNghiCapPhatVonNamNay").val() == null) {
        sMessError.push("Chưa nhập Trợ lý đề nghị quyết toán năm nay !");
    }
    if (sMessError.length != 0) {
        alert(sMessError.join("\n"));
        return false;
    }
    return true;
}

function ValidateDataInsert() {
    var sMessError = [];

    if ($("#drpDonViQuanLy").val() == null) {
        sMessError.push("Chưa nhập đơn vị quản lý !");
    }
    if ($("#txtNamKeHoach").val() == null || $("#txtNamKeHoach").val().trim() == "") {
        sMessError.push("Chưa nhập năm kế hoạch !");
    }
    if ($("#txtSoPheDuyet").val() == null || $("#txtSoPheDuyet").val().trim() == "") {
        sMessError.push("Chưa nhập số văn bản !");
    }
    if ($("#txtNgayPheDuyet").val() == null || $("#txtNgayPheDuyet").val().trim() == "") {
        sMessError.push("Chưa nhập ngày đề nghị !");
    }
    if ($("#drpNguonNganSach").val() == null) {
        sMessError.push("Chưa nhập nguồn vốn !");
    }
    if ($("#drpLoaiNganSach").val() == null) {
        sMessError.push("Chưa nhập loại nguồn vốn !");
    }
    if ($.map(tblDataGrid, function (n) { return dataExclude.indexOf(n.iId) != -1 ? null : n }).length == 0) {
        sMessError.push("Chưa nhập thông tin chi tiết !");
    }
    if (sMessError != 0) {
        alert(sMessError.join("\n"));
        return false;
    }
    return true;
}