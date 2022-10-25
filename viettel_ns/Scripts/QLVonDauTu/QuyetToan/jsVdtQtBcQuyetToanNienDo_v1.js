$(".currently").trigger("change");

//************** Index **************//
function ChangePage(iCurrentPage = 1) {
    var drpDonVi = $("#drpDonViQuanLy option:selected").val();
    var iIdNguonVon = $("#drpNguonNganSach option:selected").val();
    var dNgayDeNghiFrom = $("#txtNgayLapFrom").val();
    var dNgayDeNghiTo = $("#txtNgayLapTo").val();
    var iNamKeHoach = $("#txtNamKeHoach").val();
    var sMaDonVi = "";
    var iIdDonVi = ""

    if (drpDonVi != undefined && drpDonVi != null && drpDonVi != "") {
        iIdDonVi = drpDonVi.split("|")[0];
        sMaDonVi = drpDonVi.split("|")[1];
    }

    GetListData(iIdDonViQuanLy, iIdMaDonViQuanLy, iIdNguonVon, dNgayDeNghiFrom, dNgayDeNghiTo, iNamKeHoach, iCurrentPage);
}

function GetListData(iIdDonViQuanLy, iIdMaDonViQuanLy, iIdNguonVon, dNgayDeNghiFrom, dNgayDeNghiTo, iNamKeHoach, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/BcQuyetToanNienDo/QuyetToanNienDoView",
        data: {
            iIdMaDonViQuanLy: iIdMaDonViQuanLy,
            iIdNguonVon: iIdNguonVon,
            dNgayDeNghiFrom: dNgayDeNghiFrom,
            dNgayDeNghiTo: dNgayDeNghiTo,
            iNamKeHoach: iNamKeHoach,
            _paging: _paging
        },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#drpDonViQuanLy").val(iIdDonViQuanLy + "|" + iIdMaDonViQuanLy);
            $("#drpNguonVon").val(iIdNguonVon);
            $("#txtNgayLapFrom").val(dNgayDeNghiFrom);
            $("#txtNgayLapTo").val(dNgayDeNghiTo);
            $("#txtNamKeHoach").val(iNamKeHoach);
        }
    });
}

function BtnInsertDataClick() {
    location.href = "/QLVonDauTu/BcQuyetToanNienDo/Update";
}

function GetItemDataList(id) {
    location.href = "/QLVonDauTu/BcQuyetToanNienDo/Update/" + id;
}

function DeleteItemList(iId) {
    if (confirm("Bạn có chắc chắn muốn xóa quyết toán này ?")) {
        $.ajax({
            url: "/QLVonDauTu/BcQuyetToanNienDo/DeleteBCQuyetToanNienDo",
            type: "GET",
            data: { iId: iId },
            success: function (data) {
                alert("Xóa thành công !");
                ChangePage();
            }
        });
    }
}

//************** Update **************//
function RenderGridView() {
    var drpDonViQuanLy = $("#drpDonViQuanLy").val();
    var sMaDonViQuanLy = "";
    if (drpDonViQuanLy != null && drpDonViQuanLy != "") {
        sMaDonViQuanLy = drpDonViQuanLy.split("|")[1]
    }
    var iNamKeHoach = $("#txtNamKeHoach").val();
    var iIDNguonVonID = $("#drpNguonVon").val();
    var iCoQuanTaiChinh = $("#drpCoQuanThanhToan").val();
    var iIDBcQuyetToan = $("").val();
    var drpLoaiThanhToan = $("#drpLoaiThanhToan").val();
    if (drpLoaiThanhToan == null) return;
    var sUrl = "";
    if (drpLoaiThanhToan == 1)
        sUrl = "QuyetToanNienDoKHVN"
    else
        sUrl = "QuyetToanNienDoKHU";
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/BcQuyetToanNienDo/" + sUrl,
        data: {
            iIDBcQuyetToan: iIDBcQuyetToan,
            sMaDonVi: sMaDonViQuanLy,
            iNamKeHoach: iNamKeHoach,
            iIDNguonVonID: iIDNguonVonID,
            iCoQuanTaiChinh: iCoQuanTaiChinh,
        },
        success: function (data) {
            $("#ViewTable").html(data);
        }
    });
}

function ValidateForm() {
    var lstErrors = [];
    if ($("#drpDonViQuanLy").val() == undefined || $("#drpDonViQuanLy").val() == null) {
        lstErrors.push("Đơn vị quản lý chưa được nhập !");
    }
    if ($("#txtNamKeHoach").val() == undefined || $("#txtNamKeHoach").val() == null) {
        lstErrors.push("Năm kế hoạch chưa được nhập !");
    }
    if ($("#txtSoPheDuyet").val() == undefined || $("#txtSoPheDuyet").val() == null || $("#txtSoPheDuyet").val() == "") {
        lstErrors.push("Số quyết định chưa được nhập !");
    }
    if ($("#txtNgayPheDuyet").val() == undefined || $("#txtNgayPheDuyet").val() == null || $("#txtNgayPheDuyet").val() == "") {
        lstErrors.push("Ngày đề nghị chưa được nhập !");
    }
    if ($("#drpNguonVon").val() == undefined || $("#drpNguonVon").val() == null) {
        lstErrors.push("Nguồn vốn chưa được nhập !");
    }
    if ($("#drpLoaiThanhToan").val() == undefined || $("#drpLoaiThanhToan").val() == null) {
        lstErrors.push("Loại thanh toán chưa được nhập !");
    }
    if ($("#drpCoQuanThanhToan").val() == undefined || $("#drpCoQuanThanhToan").val() == null) {
        lstErrors.push("Cơ quan thanh toán chưa được nhập !");
    }
    if (lstErrors.length != 0) {
        alert(lstErrors.join("\n"));
        return false;
    }
    return true;
}

function Insert() {
    var data = {};
    var lstData = [];
    if (!ValidateForm()) return;

    data.sSoDeNghi = $("#txtSoPheDuyet").val();
    data.iCoQuanThanhToan = $("#drpCoQuanThanhToan").val();
    data.dNgayDeNghi = $("#txtNgayPheDuyet").val();
    data.iNamKeHoach = $("#txtNamKeHoach").val();
    data.iID_NguonVonID = $("#drpNguonVon").val();
    data.iID_MaDonViQuanLy = $("#drpDonViQuanLy").val().split("|")[1];
    data.iID_DonViQuanLyID = $("#drpDonViQuanLy").val().split("|")[0]
    data.iLoaiThanhToan = $("#drpLoaiThanhToan").val();

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/BcQuyetToanNienDo/UpdateBcQuyetToanNienDo",
        data: {
            data: data,
        },
        success: function (r) {
            if (r.bIsComplete) {
                alert("Thêm mới thành công !")
                location.href = "/QLVonDauTu/BcQuyetToanNienDo";
            } else {
                alert("Có lỗi xảy ra trong quá trình thêm mới !");
            }
        }
    });
}