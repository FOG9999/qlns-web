//====================== trigger ===============================//
$("#txtNamKeHoach").trigger("change");
$("#drpLoaiNganSach").trigger("change");
$("#drpLoaiKhoan").trigger("change");
$("#drpNganh").trigger("change");
$("#drpDuAn").trigger("change");
$("#drpHopDong").trigger("change");

$("#txtThanhToanTrongNam").on("change", function () {
    if ($("#txtThanhToanTrongNam").val() == null) {
        $("#txtThanhToanTrongNam").val(0);
    } else {
        if ($.isNumeric($("#txtThanhToanTrongNam").val())) {
            var sNumber = Number($("#txtThanhToanTrongNam").val()).toLocaleString('vi-VN');
            $("#txtThanhToanTrongNam").val(sNumber);
        }
        else {
            $("#txtThanhToanTrongNam").val(0);
        }
    }
});

$("#txtTamUng").on("change", function () {
    if ($("#txtTamUng").val() == null) {
        $("#txtTamUng").val(0);
    } else {
        if ($.isNumeric($("#txtTamUng").val())) {
            var sNumber = Number($("#txtTamUng").val()).toLocaleString('vi-VN');
            $("#txtTamUng").val(sNumber);
        }
        else {
            $("#txtTamUng").val(0);
        }
    }
});

$("#txtThuHoiTamUng").on("change", function () {
    if ($("#txtThuHoiTamUng").val() == null) {
        $("#txtThuHoiTamUng").val(0);
    } else {
        if ($.isNumeric($("#txtThuHoiTamUng").val())) {
            var sNumber = Number($("#txtThuHoiTamUng").val()).toLocaleString('vi-VN');
            $("#txtThuHoiTamUng").val(sNumber);
        }
        else {
            $("#txtThuHoiTamUng").val(0);
        }
    }
});

//====================== Dropdown event ========================//
function GetDataDropdownLoaiNganSach() {
    $.ajax({
        type: "POST",
        url: "/QLKeHoachVonNam/GetDataDropdownLoaiNganSach",
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

                if ($("#bIsEdit").val() == "1") {
                    var itemChoose = $.map(r.data, function (n) { return n.Value.indexOf($("#drpLoaiNganSachEdit").val()) != -1 ? n.Value : null })[0];
                    $("#drpLoaiNganSach").val(itemChoose);
                    $("#drpLoaiNganSach").change();
                }
            }
        }
    });
}

function GetDataDropdownLoaiAndKhoanByLoaiNganSach() {
    $.ajax({
        type: "POST",
        url: "/QLKeHoachVonNam/GetDataDropdownLoaiAndKhoanByLoaiNganSach",
        data: {
            sLNS: $("#drpLoaiNganSach").val().split("|")[0],
            iNamKeHoach: $("#txtNamKeHoach").val()
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

function GetDataDropdownNganh() {
    if ($("#drpLoaiKhoan").val() == undefined) return;
    var sLoai = $("#drpLoaiKhoan").val().split('|')[0];
    var sKhoan = $("#drpLoaiKhoan").val().split('|')[1];
    $.ajax({
        type: "POST",
        url: "/QLKeHoachVonNam/GetDataThongTinChiTietLoaiNganSach",
        data: {
            sLNS: $("#drpLoaiNganSach").val().split("|")[0],
            sLoai: sLoai,
            sKhoan: sKhoan,
            iNamKeHoach: $("#txtNamKeHoach").val()
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#drpNganh").empty();
                $.each(r.data, function (index, value) {
                    $("#drpNganh").append("<option value='" + value.Value + "'>" + value.Text + "</option>");
                });
                $("#drpNganh").prop("selectedIndex", -1);
                if ($("#bIsEdit").val() == "1") {
                    $("#drpNganh").val($("#drpNganhEdit").val());
                    $("#drpNganh").change();
                }
            }
        }
    });
}

function GetDataDropdownNganhEdit(iId) {
    $.ajax({
        type: "POST",
        url: "/GiaiNganThanhToan/GetDataThongTinChiTietLoaiNganSachByNganh",
        data: {
            iId_Nganh: iId,
            iNamKeHoach: $("#txtNamKeHoach").val()
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#drpNganh").empty();
                $.each(r.data, function (index, value) {
                    $("#drpNganh").append("<option value='" + value.Value + "'>" + value.Text + "</option>");
                });
                $("#drpNganh").prop("selectedIndex", -1);
                if ($("#bIsEdit").val() == "1") {
                    $("#drpNganh").val($("#drpNganhEdit").val());
                    $("#drpNganh").change();
                }
            }
        }
    });
}

function GetDataDropdownDuAn() {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/GetDataDropDownDuAn",
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

                if ($("#bIsEdit").val() == "1" && r.data.length != 0) {
                    var itemChoose = $.map(r.data, function (n) { return n.Value.indexOf($("#drpDuAnEdit").val()) != -1 ? n.Value : null })[0];
                    $("#drpDuAn").val(itemChoose);
                    $("#drpDuAn").change();
                }
            }
        }
    });
}

function GetDataDropdownHopDong() {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/GetDataDropdownHopDong",
        data: {
            iIdDuAn: $("#drpDuAn option:selected").val().split("|")[0]
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#drpHopDong").empty()
                $.each(r.data, function (index, value) {
                    $("#drpHopDong").append("<option value='" + value.Value + "'>" + value.Text + "</option>");
                });
                $("#drpHopDong").prop("selectedIndex", -1);
            }
            if ($("#bIsEdit").val() == "1") {
                $("#drpHopDong").val($("#drpHopDongEdit").val());
                $("#drpHopDong").change();
            }
        }
    });
}

//============================== Event List ================================//
function GetItemDataList(id) {
    window.location.href = "/QLVonDauTu/GiaiNganThanhToan/Update/" + id;
}

function GetListData(sSoKeHoach, iNamKeHoach, dNgayLapFrom, dNgayLapTo, sDonViQuanLy, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/GiaiNganThanhToan/GiaiNganThanhToanView",
        data: { sSoKeHoach: sSoKeHoach, iNamKeHoach: iNamKeHoach, dNgayLapFrom: dNgayLapFrom, dNgayLapTo: dNgayLapTo, sDonViQuanLy: sDonViQuanLy, _paging: _paging },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtSoKeHoach").val(sSoKeHoach);
            $("#txtNamKeHoach").val(iNamKeHoach);
            $("#txtNgayLapFrom").val(dNgayLapFrom);
            $("#txtNgayLapTo").val(dNgayLapTo);
            $("#drpDonViQuanLy").val(sDonViQuanLy);
        }
    });
}

function ChangePage(iCurrentPage = 1) {
    var sSoKeHoach = $("#txtSoKeHoach").val();
    var iNamKeHoach = $("#txtNamKeHoach").val();
    var dNgayLapFrom = $("#txtNgayLapFrom").val();
    var dNgayLapTo = $("#txtNgayLapTo").val();
    var sDonViQuanLy = $("#drpDonViQuanLy option:selected").val();
    GetListData(sSoKeHoach, iNamKeHoach, dNgayLapFrom, dNgayLapTo, sDonViQuanLy, iCurrentPage);
}

//=============================================================//
function GetDetailHopDong() {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/GetDetailHopDongDuAn",
        data: {
            iID_DuAnID: $("#drpDuAn option:selected").val().split("|")[0],
            iID_HopDongID: $("#drpHopDong option:selected").val(),
            dNgayDeNghi: $("#txtNgayPheDuyet").val(),
            iNamKeHoach: $("#txtNamKeHoach").val(),
            iID_NguonVonID: $("#drpNguonNganSach option:selected").val(),
            iID_LoaiNguonVonID: $("#drpLoaiNganSach option:selected").val().split("|")[1],
            iID_NganhID: $("#drpNganh option:selected").val()
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#txtGiaTriHD").val(r.data.fGiaTriHD.toLocaleString('vi-VN'));
                $("#txtNgayHD").val(ConvertDatetimeToJSon(r.data.dNgayHopDong));
                $("#txtNganHang").val(r.data.sNganHang);
                $("#txtGoiThau").val(r.data.sTenGoiThau);
                $("#txtNhaThau").val(r.data.sTenNhaThau);
                $("#txtTaiKhoanNhaThau").val(r.data.sSoTaiKhoanNhaThau);
                $("#txtDaThanhToanTrongNam").val(r.data.fDaThanhToanTrongNam.toLocaleString('vi-VN'));
                $("#txtDaTamUng").val(r.data.fDaTamUng.toLocaleString('vi-VN'));
                $("#txtDaThuHoi").val(r.data.fDaThuHoi.toLocaleString('vi-VN'));
                $("#txtLuyKeThanhToan").val(r.data.fLuyKeThanhToanKLHT.toLocaleString('vi-VN'));
                $("#iIdNhaThau").val(r.data.iID_NhaThauThucHienID);
                $("#txtDuToanGoiThau").val(r.data.fDuToanGoiThau.toLocaleString('vi-VN'))
            }
        }
    });
}

function AddThanhToan() {
    var objThanhToan = {};
    objThanhToan.iID_NganhID = $("#drpNganh option:selected").val();
    objThanhToan.sNganh = $("#drpNganh option:selected").text();
    objThanhToan.iID_DuAnID = $("#drpDuAn option:selected").val().split("|")[0];
    objThanhToan.sTenDuAn = $("#drpDuAn option:selected").text();
    objThanhToan.iID_HopDongID = $("#drpHopDong option:selected").val();
    objThanhToan.sSoHopDong = $("#drpHopDong option:selected").text();
    objThanhToan.iID_NhaThauID = $("#iIdNhaThau").val();
    objThanhToan.sTenNhaThau = $("#txtNhaThau").val();
    objThanhToan.fGiaTriDaThanhToan = $("#txtDaThanhToanTrongNam").val();
    objThanhToan.fGiaTriTamUng = $("#txtTamUng").val().replaceAll(".", "");
    objThanhToan.fGiaTriThuHoi = $("#txtThuHoiTamUng").val().replaceAll(".", "");
    objThanhToan.fLuyKeThanhToanKLHT = $("#txtLuyKeThanhToan").val();
    objThanhToan.fChiTieuNganSachNam = $("#txtChiTieuNganSachNam").val();
    objThanhToan.fGiaTriThanhToan = $("#txtThanhToanTrongNam").val().replaceAll(".", "");
    objThanhToan.fDuToanGoiThau = $("#txtDuToanGoiThau").val();
    objThanhToan.sSoTaiKhoanNhaThau = $("#txtTaiKhoanNhaThau").val();
    objThanhToan.sNganHang = $("#txtNganHang").val();
    objThanhToan.fSoThucThanhToan = (((objThanhToan.fGiaTriThanhToan ?? 0) + (objThanhToan.fGiaTriTamUng ?? 0)) - (objThanhToan.fGiaTriThuHoi ?? 0)).toLocaleString('vi-VN');
    objThanhToan.sGhiChu = $("#txtGhiChu").val();
    objThanhToan.sDonViThuHuong = "- " + $("#txtNhaThau").val() + "<br>- TK " + $("#txtTaiKhoanNhaThau").val() + "<br>- NH " + $("#txtNganHang").val(); 

    tblDataGrid = $.map(tblDataGrid, function (n) { return n.iID_DuAnID == objThanhToan.iID_DuAnID && n.iID_NhaThauID == objThanhToan.iID_NhaThauID ? null : n });

    if (objThanhToan.iID_DuAnID != null && objThanhToan.iID_DuAnID != "" && objThanhToan.iID_NhaThauID != null && objThanhToan.iID_NhaThauID != "") {
        var dataCheck = $.map(tblDataGrid, function (n) { return n.iIdRefer == objThanhToan.iID_DuAnID ? n : null });
        if (dataCheck.length == 0) {
            var objParent = {};
            objParent.iIdRefer = objThanhToan.iID_DuAnID;
            objParent.iId = tblDataGrid.length + 1;
            objParent.sTenDuAn = objThanhToan.sTenDuAn;
            tblDataGrid.push(objParent);
        }
        objThanhToan.iParentId = objThanhToan.iID_DuAnID;
        objThanhToan.sTenDuAn = objThanhToan.sTenNhaThau;
    } else {
        objThanhToan.iParentId = null;
        objThanhToan.iIdRefer = null;
    }

    objThanhToan.iId = tblDataGrid.length + 1;
    tblDataGrid.push(objThanhToan);
    RenderGridView(tblDataGrid);
    ResetDuAnDetail();
    $("#bIsEdit").val("0");
}

function ConvertListDetailToGrid(lstData) {
    tblDataGrid = [];
    $.each(lstData, function (index, item) {
        var objThanhToan = {};
        objThanhToan.iId = item.iID_DeNghiThanhToan_ChiTietID;
        objThanhToan.iID_NganhID = item.iID_NganhID;
        objThanhToan.sNganh = item.sNganh;
        objThanhToan.iID_DuAnID = item.iID_DuAnID;
        objThanhToan.sTenDuAn = item.sTenDuAn;
        objThanhToan.iID_HopDongID = item.iID_HopDongID;
        objThanhToan.sSoHopDong = item.sSoHopDong;
        objThanhToan.iID_NhaThauID = item.iID_NhaThauID;
        objThanhToan.sTenNhaThau = item.sTenNhaThau;
        objThanhToan.fGiaTriDaThanhToan = 0;
        objThanhToan.fLuyKeThanhToanKLHT = (item.fLuyKeThanhToanKLHT??0).toLocaleString('vi-VN');
        objThanhToan.fChiTieuNganSachNam = (item.fChiTieuNganSachNam ?? 0).toLocaleString('vi-VN');
        objThanhToan.sNganHang = item.sNganHang;
        objThanhToan.sDonViThuHuong = item.sDonViThuHuong;

        objThanhToan.fGiaTriTamUng = (item.fGiaTriTamUng??0).toLocaleString('vi-VN');
        objThanhToan.fGiaTriThuHoi = (item.fGiaTriThuHoi??0).toLocaleString('vi-VN');
        objThanhToan.fGiaTriThanhToan = (item.fGiaTriThanhToan??0).toLocaleString('vi-VN');
        objThanhToan.fDuToanGoiThau = (item.fDuToanGiaGoiThau ?? 0).toLocaleString('vi-VN');

        objThanhToan.fSoThucThanhToan = ((item.fGiaTriThanhToan ?? 0) + (item.fGiaTriTamUng ?? 0)) - (item.fGiaTriThuHoi ?? 0).toLocaleString('vi-VN');

        tblDataGrid = $.map(tblDataGrid, function (n) { return n.iID_DuAnID == objThanhToan.iID_DuAnID && n.iID_NhaThauID == objThanhToan.iID_NhaThauID ? null : n });

        if (objThanhToan.iID_DuAnID != null && objThanhToan.iID_DuAnID != "" && objThanhToan.iID_NhaThauID != null && objThanhToan.iID_NhaThauID != "") {
            var dataCheck = $.map(tblDataGrid, function (n) { return n.iIdRefer == objThanhToan.iID_DuAnID ? n : null });
            if (dataCheck.length == 0) {
                var objParent = {};
                objParent.iIdRefer = objThanhToan.iID_DuAnID;
                objParent.iId = tblDataGrid.length + 1;
                objParent.sTenDuAn = objThanhToan.sTenDuAn;
                tblDataGrid.push(objParent);
            }
            objThanhToan.iParentId = objThanhToan.iID_DuAnID;
            objThanhToan.sTenDuAn = objThanhToan.sTenNhaThau;
        } else {
            objThanhToan.iParentId = null;
            objThanhToan.iIdRefer = null;
        }

        tblDataGrid.push(objThanhToan);
        RenderGridView(tblDataGrid);
        ResetDuAnDetail();
        $("#bIsEdit").val("0");
    });
    $("#ViewTable button").css("display", "none");
}

function RenderGridView(data) {
    var columns = [
        { sField: "iId", bKey: true },
        { sField: "iIdRefer", bForeignKey: true },
        { sField: "iParentId", bParentKey: true },
        { sTitle: "M-TM-TM-N", sField: "sNganh", iWidth: "200px", sTextAlign: "left" },
        { sTitle: "Tên dự án, nhà thầu", sField: "sTenDuAn", iWidth: "500px", sTextAligsn: "left", bHaveIcon: 1 },
        { sTitle: "Dự toán, giá gói thầu được duyệt", sField: "fDuToanGoiThau", iWidth: "400px", sTextAlign: "right" },
        { sTitle: "Lũy kế thanh toán KLHT", sField: "fLuyKeThanhToanKLHT", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Chỉ tiêu ngân sách năm", sField: "fChiTieuNganSachNam", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Giá trị đã thanh toán trong năm", sField: "fGiaTriDaThanhToan", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Số thanh toán", sField: "fGiaTriThanhToan", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Số tạm ứng", sField: "fGiaTriTamUng", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Số thu hồi tạm ứng", sField: "fGiaTriThuHoi", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Số thực thanh toán đợt này", sField: "fSoThucThanhToan", iWidth: "200px", sTextAlign: "right"},
        { sTitle: "Đơn vị thụ hưởng, tài khoản, ngân hàng", sField: "sDonViThuHuong", iWidth: "300px", sTextAlign: "left"},
        { sTitle: "Ghi chú", sField: "sGhiChu", iWidth: "400px", sTextAlign: "left" } ];
    
    var button = { bCreateButtonInParent: false, bUpdate: 1, bDelete: 1 };
    var sHtml = GenerateTreeTable(data, columns, button)
    $("#ViewTable").css("width", $("#ViewTable").width() + "px");
    $("#ViewTable").html(sHtml);
    AddColspanTable();
}

//===================== Event button ==========================//

function CancelSaveData() {
    location.href = "/QLVonDauTu/GiaiNganThanhToan";
}

function Insert() {
    if (!ValidateDataInsert()) return;
    CreatePhieuThanhToan();
}

function UpdateDataDetail() {
    var data = [];
    var iParentId = $("#iID_DeNghiThanhToanID").val();
    $.each(tblDataGrid, function (index, value) {
        if (value.iIdRefer != undefined) { return true; }
        var item = {};
        item.iID_DeNghiThanhToanID = iParentId;

        item.iID_NganhID = value.iID_NganhID;
        item.iID_DuAnID = value.iID_DuAnID;
        item.iID_HopDongID = value.iID_HopDongID;
        item.iID_NhaThauID = value.iID_NhaThauID;
        item.fGiaTriThanhToan = value.fGiaTriThanhToan;
        item.fGiaTriTamUng = value.fGiaTriTamUng;
        item.fGiaTriThuHoi = value.fGiaTriThuHoi;
        data.push(item);
    });
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/DeleteGiaiNganThanhToanChiTietByThanhToanId",
        data: {
            iId: iParentId
        },
        success: function (r) {
            if (!r.bIsComplete) {
                alert("Cập nhật phiếu thanh toán chi tiết thất bại !");
                return false;
            } else {
                CreatePhieuChiTietInEditView(data);
            }
        }
    });
}

function CreatePhieuChiTietInEditView(data) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/InsertDeNghiThanhToanChiTiet",
        data: {
            lstData: data
        },
        success: function (r) {
            if (r.bIsComplete) {
                alert("Cập nhật thành công phiếu thanh toán !");
            }
            else {
                alert("Cập nhật phiếu thanh toán chi tiết thất bại !");
                return false;
            }
        }
    });
    return true;
}

function CreatePhieuThanhToan() {
    var data = {};
    data.sSoDeNghi = $("#txtSoPheDuyet").val();
    data.dNgayDeNghi = $("#txtNgayPheDuyet").val();
    data.iID_DonViQuanLyID = $("#drpDonViQuanLy option:selected").val();
    data.sNguoiLap = $("#txtNguoiLap").val();
    data.iNamKeHoach = $("#txtNamKeHoach").val();
    data.iID_NguonVonID = $("#drpNguonNganSach option:selected").val();
    data.iID_LoaiNguonVonID = $("#drpLoaiNganSach option:selected").val().split("|")[1];
    data.sGhiChu = $("#txtGhiChuChung").val();

    $.ajax({
        type: "POST",
        url: "/GiaiNganThanhToan/InsertDeNghiThanhToan",
        data: {
            data: data
        },
        success: function (r) {
            if (r.bIsComplete) {
                CreatePhieuThanhToanChiTiet(r.data);
            } else {
                alert("Tạo thanh toán thất bại !");
            }
        }
    });
}

function CreatePhieuThanhToanChiTiet(iParentId) {
    var data = [];
    $.each(tblDataGrid, function (index, value) {
        if (value.iIdRefer != undefined) { return true; }
        var item = {};
        item.iID_DeNghiThanhToanID = iParentId;

        item.iID_NganhID = value.iID_NganhID;
        item.iID_DuAnID = value.iID_DuAnID;
        item.iID_HopDongID = value.iID_HopDongID;
        item.iID_NhaThauID = value.iID_NhaThauID;
        item.fGiaTriThanhToan = value.fGiaTriThanhToan.replaceAll(".", "");
        item.fGiaTriTamUng = value.fGiaTriTamUng.replaceAll(".", "");
        item.fGiaTriThuHoi = value.fGiaTriThuHoi.replaceAll(".", "");
        data.push(item);
    });
    $.ajax({
        type: "POST",
        url: "/GiaiNganThanhToan/InsertDeNghiThanhToanChiTiet",
        data: {
            lstData: data
        },
        success: function (r) {
            if (r.bIsComplete) {
                alert("Tạo mới thành công phiếu thanh toán !");
                location.href = "/QLVonDauTu/GiaiNganThanhToan";
            }
            else {
                alert("Tạo phiếu thanh toán chi tiết thất bại !");
            }
        }
    });
}

function DeleteItemList(id) {
    if (!confirm("Chấp nhận xóa bản ghi ?")) return;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/DeleteNganSachThanhToan",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                ChangePage();
            }
        }
    });
}

function BtnInsertDataClick() {
    location.href = "/QLVonDauTu/GiaiNganThanhToan/Insert";
}

//===================== Reset data ============================//
function ClearDuAnDetail() {
    $("#drpNganh").empty();
    $("#drpDuAn").empty();
    $("#drpHopDong").empty();
    $("#txtChiTieuNganSachNam").val("");
    $("#txtNganHang").val("");
    $("#txtGoiThau").val("");
    $("#txtNhaThau").val("");
    $("#txtTaiKhoanNhaThau").val("");
    $("#txtGiaTriHD").val("");
    $("#txtNgayHD").val("");
    $("#txtDaThanhToanTrongNam").val("");
    $("#txtDaTamUng").val("");
    $("#txtDaThuHoi").val("");
    $("#txtLuyKeThanhToan").val("");
    $("#txtThanhToanTrongNam").val("");
    $("#txtTamUng").val("");
    $("#txtThuHoiTamUng").val("");
    $("#txtGhiChu").val("");
    $("#txtDuToanGoiThau").val("");
    $("#iIdNhaThau").val("");
}

function ResetDuAnDetail() {
    $("#drpNganh").prop("selectedIndex", -1);
    $("#drpDuAn").empty();
    $("#drpHopDong").empty();
    $("#txtChiTieuNganSachNam").val("");
    $("#txtNganHang").val("");
    $("#txtGoiThau").val("");
    $("#txtNhaThau").val("");
    $("#txtTaiKhoanNhaThau").val("");
    $("#txtGiaTriHD").val("");
    $("#txtNgayHD").val("");
    $("#txtDaThanhToanTrongNam").val("");
    $("#txtDaTamUng").val("");
    $("#txtDaThuHoi").val("");
    $("#txtLuyKeThanhToan").val("");
    $("#txtThanhToanTrongNam").val("");
    $("#txtTamUng").val("");
    $("#txtThuHoiTamUng").val("");
    $("#txtGhiChu").val("");
    $("#txtDuToanGoiThau").val("");
    $("#iIdNhaThau").val("");
}

//===================== update css ===========================//
function AddColspanTable() {
    var index = 0;
    $(".table-parent th").each(function () {
        ++index;
        if (index == 8) {
            $(this).attr('colspan', 3);
            $(this).html("Giá trị phê duyệt thanh toán kì này")
            return true;
        }
        if (index == 9 || index == 10) {
            $(this).remove();
            return true;
        }
        if (index)

            $(this).attr('rowspan', 2);
    });
    var row = "<tr><th width='200px'>Số thanh toán</th><th width='200px'>Số tạm ứng</th><th width='200px'>Số thu hồi tạm ứng</th></tr>";
    $(".table-parent tr:first").after(row);
    $(".table-parent .fa-caret-down").closest("tr").find("[type=button]").remove();
}

//=========================== validate ===========//
function ValidateDataInsert() {
    var sMessError = [];
    if ($("#drpDonViQuanLy").val() == null) {
        sMessError.push("Chưa nhập đơn vị quản lý !");
    }
    if ($("#txtSoPheDuyet").val().trim() == "") {
        sMessError.push("Chưa nhập số phê duyệt !");
    }
    if ($("#txtNgayPheDuyet").val().trim() == "") {
        sMessError.push("Chưa nhập ngày phê duyệt !");
    }
    if ($("#txtNamKeHoach").val().trim() == "") {
        sMessError.push("Chưa nhập năm kế hoạch !");
    }
    if ($("#drpNguonNganSach").val() == null) {
        sMessError.push("Chưa nhập nguồn vốn !");
    }
    if ($("#drpLoaiNganSach").val() == null) {
        sMessError.push("Chưa nhập loại nguồn vốn !");
    }
    var lstThongTinchiTiet = $.map(tblDataGrid, function (n) { return n.iID_DuAnID == undefined ? null : n });
    if (lstThongTinchiTiet.length == 0) {
        sMessError.push("Chưa nhập thông tin chi tiết !");
    }
    if (sMessError.length != 0) {
        alert(sMessError.join('\n'));
        return false;
    }
    return true;
}

//============================== Event Edit =================================//
function SaveThongTinChung() {
    data = {};
    data.iID_DeNghiThanhToanID = $("#iID_DeNghiThanhToanID").val();
    data.sSoPheDuyet = $("#txtSoPheDuyet").val();
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/UpdateDeNghiThanhToan",
        data: {
            data: data
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#txtSoPheDuyetOld").val(data.sSoPheDuyet);
                EditThongTinChung_Off();
            }
        }
    });
}

function GetNganSachThanhToanChiTiet() {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/GetGiaiNganThanhToanChiTiet",
        data: {
            iId: $("#iID_DeNghiThanhToanID").val()
        },
        success: function (r) {
            if (r.bIsComplete) {
                ConvertListDetailToGrid(r.data);
            }
        }
    });
}

function CancelSaveThongTinChung() {
    $("#txtSoPheDuyet").val($("#txtSoPheDuyetOld").val());
    EditThongTinChung_Off();
}

function EditThongTinChung_On() {
    $("button.show-edit-on").css("display", "");
    $("button.show-edit-off").css("display", "none");

    $("#txtSoPheDuyet").removeAttr("disabled");
}

function EditThongTinChung_Off() {
    $("button.show-edit-on").css("display", "none");
    $("button.show-edit-off").css("display", "");

    $("#txtSoPheDuyet").attr("disabled", "disabled")
}

function SaveThongTinChiTiet() {
    if (UpdateDataDetail()) {
        EditThongTinChiTiet_Off();
    }
}

function CancelSaveThongChiTiet() {
    EditThongTinChiTiet_Off();
    GetNganSachThanhToanChiTiet();
}

function EditThongTinChiTiet_On() {
    $("button.show-edit-detail-on").css("display", "");
    $("button.show-edit-detail-off").css("display", "none");

    $("#pnlThongTinChiTiet").css("display", "");
    $("#ViewTable button").css("display", "");
}

function EditThongTinChiTiet_Off() {
    $("button.show-edit-detail-on").css("display", "none");
    $("button.show-edit-detail-off").css("display", "");

    $("#pnlThongTinChiTiet").css("display", "none");
    $("#ViewTable button").css("display", "none");
}