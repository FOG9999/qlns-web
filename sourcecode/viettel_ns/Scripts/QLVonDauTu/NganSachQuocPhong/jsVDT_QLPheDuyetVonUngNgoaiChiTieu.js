var typeSearch = 1;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var iIdMax = 0;
var CONFIRM = 0;
var ERROR = 1;

$(document).ready(function ($) {
    $("#iID_DonViQuanLyID").change(function () {
        var dNgayQuyetDinh = $("#dNgayQuyetDinh").val();

        if (this.value != "" && this.value != GUID_EMPTY && dNgayQuyetDinh != "") {
            LayDanhSachDuAnTheoDonViQuanLyVaNgayQuyetDinh(this.value, dNgayQuyetDinh)
            ClearThongTinHopDong();
        } else {
            $("#iID_DuAnID option:not(:first)").remove();
            $("#iID_DuAnID").trigger("change");
        }
    });

    $("#dNgayQuyetDinh").change(function () {
        var iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();

        if (this.value != "" && iID_DonViQuanLyID != "" && iID_DonViQuanLyID != null) {
            LayDanhSachDuAnTheoDonViQuanLyVaNgayQuyetDinh(iID_DonViQuanLyID, this.value)
            ClearThongTinHopDong();
        } else {
            $("#iID_DuAnID option:not(:first)").remove();
            $("#iID_DuAnID").trigger("change");
        }
    });

    $("#iID_DuAnID").change(function (e) {
        ClearThongTinHopDong();
        var select = $("#iID_DuAnID option:selected");
        var maCapPheDuyet = select.attr("smacappheduyet");
        if (maCapPheDuyet != undefined) {
            if (maCapPheDuyet == 'BD' || maCapPheDuyet == 'CD') {
                //Hien thi thong tin hop dong, get giá trị Lũy kế KHVU được duyệt
                $("#hopDongInfo").css('display', '');
                $("#goiThauInfo").css('display', '');
                LayDanhSachHopDongTheoDuAn(select.val());
                LayLuyKeKHVUDuocDuyet(select.val(), 1);
            } else {
                //Khong hien thi thong tin hop dong, get gia tri lũy kế
                $("#hopDongInfo").css('display', 'none');
                $("#goiThauInfo").css('display', 'none');
                LayLuyKeKHVUDuocDuyet(select.val(), 2);
            }
        }
    });

    $("#iID_HopDongID").change(function (e) {
        var iID_HopDongID = $("#iID_HopDongID option:selected").val();
        //Lay thong tin hop dong va Luy ke thu hoi ung, Luy ke so von da tam ung
        if (iID_HopDongID != GUID_EMPTY) {
            LayThongTinHopDongVaLuyKeUng(iID_HopDongID);
        } else {
            ClearThongTinHopDong();
        }
    });

    //RenderGridView(tblDataGrid, 1);
});


function LayDanhSachDuAnTheoDonViQuanLyVaNgayQuyetDinh(iID_DonViQuanLyID, dNgayQuyetDinh) {
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetVonUngNgoaiChiTieu/LayDanhSachDuAnTheoDonViQuanLyVaNgayQuyetDinhKHVU",
        type: "POST",
        data: { iID_DonViQuanLyID: iID_DonViQuanLyID, dNgayQuyetDinh: dNgayQuyetDinh },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null && data != "") {
                $("#iID_DuAnID").html(data);
                $("#iID_DuAnID").prop("selectedIndex", 0);
            }
        },
        error: function (data) {

        }
    })
}

function LayDanhSachHopDongTheoDuAn(iID_DuAnID) {
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetVonUngNgoaiChiTieu/LayDanhSachHopDongTheoDuAn",
        type: "POST",
        data: { iID_DuAnID: iID_DuAnID },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null && data != "") {
                $("#iID_HopDongID").html(data);
                $("#iID_HopDongID").prop("selectedIndex", 0);
            }
        },
        error: function (data) {

        }
    })
}

function LayLuyKeKHVUDuocDuyet(iID_DuAnID, type) {
    var iID_DonViQuanLyID = $("#iID_DonViQuanLyID option:selected").val();
    var iID_NhomQuanLyID = $("#iID_NhomQuanLyID option:selected").val();
    var dNgayQuyetDinh = $("#dNgayQuyetDinh").val();

    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetVonUngNgoaiChiTieu/LayLuyKeTheoDuAn",
        type: "POST",
        dataType: "json",
        data: { iID_DuAnID: iID_DuAnID, iID_DonViQuanLyID: iID_DonViQuanLyID, iID_NhomQuanLyID: iID_NhomQuanLyID, dNgayQuyetDinh: dNgayQuyetDinh },
        success: function (r) {
            if (r.bIsComplete) {
                if (type == 1) {
                    $("#fLKKHVUDuocDuyet").val(r.data.fLKKHVUDuocDuyet > 0 ? FormatNumber(r.data.fLKKHVUDuocDuyet) : 0);
                } else {
                    $("#fLKKHVUDuocDuyet").val(r.data.fLKKHVUDuocDuyet > 0 ? FormatNumber(r.data.fLKKHVUDuocDuyet) : 0);
                    $("#fLKSoVonDaTamUng").val(r.data.fLKSoVonDaTamUng > 0 ? FormatNumber(r.data.fLKSoVonDaTamUng) : 0);
                    $("#fLKThuHoiUng").val(r.data.fLKThuHoiUng > 0 ? FormatNumber(r.data.fLKThuHoiUng) : 0);
                }
            }
        }
    })
}

function LayThongTinHopDongVaLuyKeUng(iID_HopDongID) {
    var iID_DuAnID = $("#iID_DuAnID option:selected").val();
    var iID_DonViQuanLyID = $("#iID_DonViQuanLyID option:selected").val();
    var iID_NhomQuanLyID = $("#iID_NhomQuanLyID option:selected").val();
    var dNgayQuyetDinh = $("#dNgayQuyetDinh").val();

    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetVonUngNgoaiChiTieu/LayThongTinHopDongVaLuyKeUng",
        type: "POST",
        dataType: "json",
        data: { iID_DuAnID: iID_DuAnID, iID_HopDongID: iID_HopDongID, iID_DonViQuanLyID: iID_DonViQuanLyID, iID_NhomQuanLyID: iID_NhomQuanLyID, dNgayQuyetDinh: dNgayQuyetDinh},
        success: function (r) {
            if (r.bIsComplete) {
                $("#iID_HopDongID").val(r.data.iID_HopDongID);
                $("#iID_NhaThauID").val(r.data.iID_NhaThauID);
                $("#dNgayHopDong").val(ConvertDatetimeToJSon(r.data.dNgayHopDong));
                $("#fGiaTriHopDong").val(FormatNumber(r.data.fTongGiaTriHD));
                $("#fTongTienTrungThau").val(FormatNumber(r.data.fTongTienTrungThau));
                $("#sTenGoiThau").val(r.data.sTenGoiThau);
                $("#sTenNhaThau").val(r.data.sTenNhaThau);
                $("#sSoTaiKhoan").val(r.data.sSoTaiKhoan);
                $("#sNganHang").val(r.data.sNganHang);
                $("#fLKSoVonDaTamUng").val(r.data.fLKSoVonDaTamUng > 0 ? FormatNumber(r.data.fLKSoVonDaTamUng) : 0);
                $("#fLKThuHoiUng").val(r.data.fLKThuHoiUng > 0 ? FormatNumber(r.data.fLKThuHoiUng) : 0);
            }
        }
    })
}

function ClearThongTinHopDong() {
    $("#iID_HopDongID").prop("selectedIndex", 0);
    $("#dNgayHopDong").val('');
    $("#fGiaTriHopDong").val('');
    $("#fTongTienTrungThau").val('');
    $("#sTenGoiThau").val('');
    $("#sTenNhaThau").val('');
    $("#sSoTaiKhoan").val('');
    $("#sNganHang").val('');
    $("#fLKKHVUDuocDuyet").val('');
    $("#fLKSoVonDaTamUng").val('');
    $("#fLKThuHoiUng").val('');
}

//function ClearLuyKe() {
//    $("#fLKKHVUDuocDuyet").val('');
//    $("#fLKSoVonDaTamUng").val('');
//    $("#fLKThuHoiUng").val('');
//}

function GetGiaTriDauTu(iID_DuAnID, dNgayQuyetDinh) {
    //console.log(iID_DuAnID + ' ' + dNgayQuyetDinh);
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/GetTongMucDauTu",
        data: { iID_DuAnID: iID_DuAnID, dNgayQuyetDinh: dNgayQuyetDinh },
        success: function (r) {
            if (r > 0) {
                $("#fGiaTriDauTu").val(FormatNumber(r));
            } else {
                $("#fGiaTriDauTu").val(r);
            }
        }
    });
}

function InsertDetailData() {
    var iID_DuAnID = $("#iID_DuAnID").val();
    if (iID_DuAnID == null || $("#iID_DuAnID").val() === GUID_EMPTY) {
        var Title = 'Lỗi thêm thông tin chi tiết';
        var messErr = [];
        messErr.push('Chưa nhập thông tin chi tiết !');
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: messErr, Category: ERROR },
            success: function (data) {
                $("#divModalConfirm").html(data);
            }
        });
        //alert("Chưa nhập thông tin chi tiết !");
        return;
    }
    var data = {};
    data.iId = tblDataGrid.length + 1;
    data.iParentId = 1;
    data.sMaKetNoi = 'NVN-2222';
    data.sTenDuAn = $("#iID_DuAnID option:selected").text();
    data.iID_DuAnID = $("#iID_DuAnID option:selected").val();

    data.fGiaTriDauTu = $("#fGiaTriDauTu").val();
    data.fGiaTriUng = $("#fGiaTriUng").val();

    tblDataGrid = $.map(tblDataGrid, function (n) { return n.iID_DuAnID != data.iID_DuAnID ? n : null });

    tblDataGrid.push(data)
    sumGiaTriDauTuVaVonUng(tblDataGrid);
    FillDataToGridViewInsert(tblDataGrid);
    ClearDataInsert();
}

function sumGiaTriDauTuVaVonUng(tblDataGrid) {
    var fTongGiaTriDauTu = 0;
    var fTongGiaTriUng = 0;

    $.each(tblDataGrid, function (index, value) {
        if (value.iParentId == 1) {
            fTongGiaTriDauTu += parseFloat(UnFormatNumber(value.fGiaTriDauTu));
            fTongGiaTriUng += parseFloat(UnFormatNumber(value.fGiaTriUng));
        }
    });
    $.each(tblDataGrid, function (index, value) {
        if (value.iId == 1) {
            value.fGiaTriDauTu = fTongGiaTriDauTu == 0 ? 0 : FormatNumber(fTongGiaTriDauTu);
            value.fGiaTriUng = fTongGiaTriUng == 0 ? 0 : FormatNumber(fTongGiaTriUng);
        }
    });

    $("#tong_gaitridautu").html(fTongGiaTriDauTu == 0 ? '0' : FormatNumber(fTongGiaTriDauTu))
    $("#tong_giatriung").html(fTongGiaTriUng == 0 ? 0 :FormatNumber(fTongGiaTriUng))
}
function RenderGridView(data, typeButton) {
    var columns = [
        { sField: "iId", bKey: true },
        { sField: "iIdRefer", bForeignKey: true },
        { sField: "iParentId", bParentKey: true },
        { sTitle: "Tên dự án, nhà thầu", sField: "sTenDuAn", iWidth: "500px", sTextAligsn: "left", bHaveIcon: 1 },
        { sTitle: "Dự toán, giá gói thầu được duyệt", sField: "fTongTienTrungThau", iWidth: "400px", sTextAlign: "right" },
        { sTitle: "KH vốn ứng được duyệt", sField: "fLKKHVUDuocDuyet", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Lũy kế số vốn đã cấp ứng", sField: "fLKSoVonDaTamUng", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Giá trị cấp ứng", sField: "fGiaTriThanhToan", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Giá trị thu hồi ứng XDCB khác", sField: "fGiaTriThuHoiUngNgoaiChiTieu", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Đơn vị thụ hưởng, tài khoản, ngân hàng", sField: "sDonViThuHuong", iWidth: "200px", sTextAlign: "left" },
        { sTitle: "Ghi chú", sField: "sGhiChu", iWidth: "200px", sTextAlign: "left" }];
    var button;
    if (typeButton == 1) {
        button = { bCreateButtonInParent: false, bUpdate: 1, bDelete: 1 };
    } else {
        button = { bCreateButtonInParent: false, bUpdate: 0, bDelete: 0 };
    }
    //var button = { bCreateButtonInParent: false, bUpdate: 1, bDelete: 1 };
    var sHtml = GenerateTreeTable(data, columns, button)
    $("#ViewTable").css("width", $("#ViewTable").width() + "px");
    $("#ViewTable").html(sHtml);
    RemoveBtnOfParent();
}

function RemoveBtnOfParent() {
    $(".table-parent .fa-caret-down").closest("tr").find("[type=button].btn-edit").remove();
}

function AddThanhToan() {
    var iID_DuAnID = $("#iID_DuAnID option:selected").val();
    if (iID_DuAnID == null || $("#iID_DuAnID").val() == GUID_EMPTY) {
        var Title = 'Lỗi thêm thông tin chi tiết';
        var messErr = [];
        messErr.push('Chưa nhập thông tin chi tiết !');
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: messErr, Category: ERROR },
            success: function (data) {
                $("#divModalConfirm").html(data);
            }
        });
        //alert("Chưa nhập thông tin chi tiết !");
        return;
    }

    var objThanhToan = {};
    objThanhToan.iID_DuAnID = $("#iID_DuAnID option:selected").val();
    objThanhToan.sTenDuAn = $("#iID_DuAnID option:selected").text();
    objThanhToan.iID_HopDongID = $("#iID_HopDongID option:selected").val();
    objThanhToan.iID_NhaThauID = $("#iID_NhaThauID").val();
    objThanhToan.sTenNhaThau = $("#sTenNhaThau").val();
    objThanhToan.sGhiChu = $("#sGhiChu2").val();
    objThanhToan.fGiaTriHopDong = $("#fGiaTriHopDong").val();
    objThanhToan.fTongTienTrungThau = $("#fTongTienTrungThau").val() != '' ? $("#fTongTienTrungThau").val() : 0;
    objThanhToan.fLKKHVUDuocDuyet = $("#fLKKHVUDuocDuyet").val() != '' ? $("#fLKKHVUDuocDuyet").val() : 0;
    objThanhToan.fLKSoVonDaTamUng = $("#fLKSoVonDaTamUng").val() != '' ? $("#fLKSoVonDaTamUng").val() : 0;
    objThanhToan.fGiaTriThanhToan = $("#fGiaTriThanhToan").val() != '' ? $("#fGiaTriThanhToan").val() : 0;
    objThanhToan.fGiaTriThuHoiUngNgoaiChiTieu = $("#fGiaTriThuHoiUngNgoaiChiTieu").val() != '' ? $("#fGiaTriThuHoiUngNgoaiChiTieu").val() : 0;
    if (objThanhToan.iID_HopDongID != undefined && objThanhToan.iID_HopDongID != GUID_EMPTY) {
        objThanhToan.sDonViThuHuong = '- ' + $("#sTenNhaThau").val() + '<br>' + '- TK: ' + $("#sSoTaiKhoan").val() + '<br>' + '- NH: ' + $("#sNganHang").val();
    }
    
    //Thong tin them khi edit
    objThanhToan.dNgayHopDong = $("#dNgayHopDong").val();
    objThanhToan.sTenGoiThau = $("#sTenGoiThau").val();
    objThanhToan.sSoTaiKhoan = $("#sSoTaiKhoan").val();
    objThanhToan.sNganHang = $("#sNganHang").val();
    objThanhToan.fLKThuHoiUng = $("#fLKThuHoiUng").val();

    tblDataGrid = $.map(tblDataGrid, function (n) { return n.iID_DuAnID == objThanhToan.iID_DuAnID && n.iID_NhaThauID == objThanhToan.iID_NhaThauID ? null : n });

    if (objThanhToan.iID_DuAnID != null && objThanhToan.iID_DuAnID != "" && objThanhToan.iID_DuAnID != GUID_EMPTY && objThanhToan.iID_NhaThauID != null && objThanhToan.iID_NhaThauID != "" && objThanhToan.iID_NhaThauID != GUID_EMPTY) {
        var dataCheck = $.map(tblDataGrid, function (n) { return n.iIdRefer == objThanhToan.iID_DuAnID ? n : null });
        if (dataCheck.length == 0) {
            var objParent = {};
            objParent.iIdRefer = objThanhToan.iID_DuAnID;
            iIdMax++;
            objParent.iId = iIdMax;
            //objParent.iId = tblDataGrid.length + 1;
            objParent.sTenDuAn = objThanhToan.sTenDuAn;
            tblDataGrid.push(objParent);
        }
        objThanhToan.iParentId = objThanhToan.iID_DuAnID;
        objThanhToan.sTenDuAn = objThanhToan.sTenNhaThau;
    } else {
        objThanhToan.iParentId = null;
        objThanhToan.iIdRefer = null;
    }

    iIdMax++;
    objThanhToan.iId = iIdMax;
    //objThanhToan.iId = tblDataGrid.length + 1;
    tblDataGrid.push(objThanhToan);
    sumGiaTriChoDuAn(tblDataGrid);
    RenderGridView(tblDataGrid, 1);
    ResetDuAnDetail();
    $("#bIsEdit").val("0");
}

function ConvertListDetailToGrid(lstData, type) {
    tblDataGrid = [];
    //iIdMax = lstData.length;
    $.each(lstData, function (index, item) {
        var objThanhToan = {};
        objThanhToan.iId = item.iID_DeNghiThanhToan_ChiTietID;
        objThanhToan.iID_DuAnID = item.iID_DuAnID;
        objThanhToan.sTenDuAn = item.sTenDuAn;
        objThanhToan.iID_HopDongID = item.iID_HopDongID;
        objThanhToan.iID_NhaThauID = item.iID_NhaThauID;
        objThanhToan.sTenNhaThau = item.sTenNhaThau;
        objThanhToan.sGhiChu = "";
        objThanhToan.fGiaTriHopDong = FormatNumber(item.fTongGiaTriHD);
        objThanhToan.fTongTienTrungThau = item.fTongTienTrungThau > 0 ? FormatNumber(item.fTongTienTrungThau) : 0;
        objThanhToan.fLKKHVUDuocDuyet = item.fLKKHVUDuocDuyet > 0 ? FormatNumber(item.fLKKHVUDuocDuyet) : 0;
        objThanhToan.fLKSoVonDaTamUng = item.fLKSoVonDaTamUng > 0 ? FormatNumber(item.fLKSoVonDaTamUng) : 0;
        objThanhToan.fGiaTriThanhToan = item.fGiaTriThanhToan > 0 ? FormatNumber(item.fGiaTriThanhToan) : 0;
        objThanhToan.fGiaTriThuHoiUngNgoaiChiTieu = item.fGiaTriThuHoiUngNgoaiChiTieu > 0 ? FormatNumber(item.fGiaTriThuHoiUngNgoaiChiTieu) : 0;
        if (item.iID_HopDongID != GUID_EMPTY) {
            var sTenNhaThau = item.sTenNhaThau != null ? item.sTenNhaThau : '';
            var sSoTaiKhoan = item.sSoTaiKhoan != null ? item.sSoTaiKhoan : '';
            var sNganHang = item.sNganHang != null ? item.sNganHang : '';
            objThanhToan.sDonViThuHuong = '- ' + sTenNhaThau + '<br>' + '- TK: ' + sSoTaiKhoan + '<br>' + '- NH: ' + sNganHang;
        }
        //Thong tin them khi edit
        //objThanhToan.sTenGoiThau = $("#sTenGoiThau").val();
        //objThanhToan.sSoTaiKhoan = $("#sSoTaiKhoan").val();
        //objThanhToan.sNganHang = $("#sNganHang").val();
        //objThanhToan.fLKThuHoiUng = $("#fLKThuHoiUng").val();

        tblDataGrid = $.map(tblDataGrid, function (n) { return n.iID_DuAnID == objThanhToan.iID_DuAnID && n.iID_NhaThauID == objThanhToan.iID_NhaThauID ? null : n });

        if (objThanhToan.iID_DuAnID != null && objThanhToan.iID_DuAnID != "" && objThanhToan.iID_NhaThauID != null && objThanhToan.iID_NhaThauID != "" && objThanhToan.iID_NhaThauID != GUID_EMPTY) {
            var dataCheck = $.map(tblDataGrid, function (n) { return n.iIdRefer == objThanhToan.iID_DuAnID ? n : null });
            if (dataCheck.length == 0) {
                var objParent = {};
                objParent.iIdRefer = objThanhToan.iID_DuAnID;
                iIdMax++;
                objParent.iId = iIdMax;
                //objParent.iId = tblDataGrid.length + 1;
                objParent.sTenDuAn = objThanhToan.sTenDuAn;
                tblDataGrid.push(objParent);
            }
            objThanhToan.iParentId = objThanhToan.iID_DuAnID;
            objThanhToan.sTenDuAn = objThanhToan.sTenNhaThau;
        } else {
            objThanhToan.iParentId = null;
            objThanhToan.iIdRefer = null;
        }

        //iIdMax++;
        //objThanhToan.iId = iIdMax;
        tblDataGrid.push(objThanhToan);
        sumGiaTriChoDuAn(tblDataGrid);
        RenderGridView(tblDataGrid, type);
        ResetDuAnDetail();
        $("#bIsEdit").val("0");
    });
    //$("#ViewTable button").css("display", "none");
}

function sumGiaTriChoDuAn(tblDataGrid) {
    var dataRefer = $.map(tblDataGrid, function (n) { return n.iIdRefer != null ? n : null });

    $.each(dataRefer, function (index, itemRefer) {
        fTongTienTrungThau = 0;
        fLKKHVUDuocDuyet = 0;
        fLKSoVonDaTamUng = 0;
        fGiaTriThanhToan = 0;
        fGiaTriThuHoiUngNgoaiChiTieu = 0;
        $.each(tblDataGrid, function (index, item) {
            if (item.iParentId == itemRefer.iIdRefer) {
                fTongTienTrungThau += parseFloat(UnFormatNumber(item.fTongTienTrungThau));
                fLKKHVUDuocDuyet += parseFloat(UnFormatNumber(item.fLKKHVUDuocDuyet));
                fLKSoVonDaTamUng += parseFloat(UnFormatNumber(item.fLKSoVonDaTamUng));
                fGiaTriThanhToan += parseFloat(UnFormatNumber(item.fGiaTriThanhToan));
                fGiaTriThuHoiUngNgoaiChiTieu += parseFloat(UnFormatNumber(item.fGiaTriThuHoiUngNgoaiChiTieu));
            }
        });
        itemRefer.fTongTienTrungThau = fTongTienTrungThau > 0 ? FormatNumber(fTongTienTrungThau) : 0;
        itemRefer.fLKKHVUDuocDuyet = fLKKHVUDuocDuyet > 0 ? FormatNumber(fLKKHVUDuocDuyet) : 0;
        itemRefer.fLKSoVonDaTamUng = fLKSoVonDaTamUng > 0 ? FormatNumber(fLKSoVonDaTamUng) : 0;
        itemRefer.fGiaTriThanhToan = fGiaTriThanhToan > 0 ? FormatNumber(fGiaTriThanhToan) : 0;
        itemRefer.fGiaTriThuHoiUngNgoaiChiTieu = fGiaTriThuHoiUngNgoaiChiTieu > 0 ? FormatNumber(fGiaTriThuHoiUngNgoaiChiTieu) : 0;
    });
}


function ResetDuAnDetail() {
    $("#iID_DuAnID").prop("selectedIndex", 0);
    $("#iID_HopDongID").prop("selectedIndex", 0);
    $("#dNgayHopDong").val('');
    $("#fGiaTriHopDong").val('');
    $("#fTongTienTrungThau").val('');
    $("#iID_NhaThauID").val('');
    $("#sTenGoiThau").val('');
    $("#sTenNhaThau").val('');
    $("#sSoTaiKhoan").val('');
    $("#sNganHang").val('');
    $("#fLKKHVUDuocDuyet").val('');
    $("#fLKSoVonDaTamUng").val('');
    $("#fLKThuHoiUng").val('');
    $("#sGhiChu2").val('');
    $("#fGiaTriThanhToan").val('');
    $("#fGiaTriThuHoiUngNgoaiChiTieu").val('');
}

function DeleteItem(id) {
    var iIdDuAnCheck = $.map(tblDataGrid, function (n) { return n.iId == id ? n : null })[0].iID_DuAnID;
    var iIdRefer = $.map(tblDataGrid, function (n) { return n.iId == id ? n : null })[0].iIdRefer;
    tblDataGrid = $.map(tblDataGrid, function (n) { return n.iId != id ? n : null });
    if (iIdDuAnCheck != undefined) {
        if ($.map(tblDataGrid, function (n) { return n.iID_DuAnID == iIdDuAnCheck ? n : null }).length == 0) {
            tblDataGrid = $.map(tblDataGrid, function (n) { return n.iIdRefer != iIdDuAnCheck ? n : null });
        }
    } else {
        tblDataGrid = $.map(tblDataGrid, function (n) { return n.iIdRefer != iIdRefer ? n : null });
        tblDataGrid = $.map(tblDataGrid, function (n) { return n.iID_DuAnID != iIdRefer ? n : null });
    }
    
    RenderGridView(tblDataGrid, 1);
}
// =================================
function FormatNumberKHVUChiTiet(tblDataGrid) {
    $.each(tblDataGrid, function (index, value) {
        if (value.iParentId == 1) {
            value.fGiaTriDauTu = value.fGiaTriDauTu == 0 ? 0 : FormatNumber(value.fGiaTriDauTu);
            value.fGiaTriUng = value.fGiaTriUng == 0 ? 0 : FormatNumber(value.fGiaTriUng);
        }
    });
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

function ClearDataInsert() {
    $("#iID_DuAnID").prop("selectedIndex", 0);
    $("#fGiaTriDauTu").val('');
    $("#fGiaTriUng").val('');
}

function Luu() {
    if (CheckLoi()) {
        var dataDNTTU = {};
        var listDNTTUChiTiet = [];
        var data = {};

        dataDNTTU.iID_DeNghiThanhToanID = $("#iID_DeNghiThanhToanID").val();
        dataDNTTU.iID_DonViQuanLyID = $("#iID_DonViQuanLyID option:selected").val();
        dataDNTTU.sSoDeNghi = $("#sSoDeNghi").val();
        dataDNTTU.dNgayDeNghi = $("#dNgayQuyetDinh").val();
        dataDNTTU.sNguoiLap = $("#sNguoiLap").val();
        dataDNTTU.iID_NhomQuanLyID = $("#iID_NhomQuanLyID option:selected").val();
        dataDNTTU.sGhiChu = $("#sGhiChu").val();

        var fTongGiaTriThanhToan = 0;
        var fTongGiaTriThuHoiUngNgoaiChiTieu = 0;
        $.each(tblDataGrid, function (index, value) {
            if (value.iIdRefer != undefined) { return true; }
            var dataDNTTUChiTiet = {};

            dataDNTTUChiTiet.iID_DuAnID = value.iID_DuAnID;
            dataDNTTUChiTiet.iID_HopDongID = value.iID_HopDongID;
            dataDNTTUChiTiet.iID_NhaThauID = value.iID_NhaThauID;
            dataDNTTUChiTiet.fGiaTriThanhToan = UnFormatNumber(value.fGiaTriThanhToan);
            fTongGiaTriThanhToan += parseFloat(dataDNTTUChiTiet.fGiaTriThanhToan);
            dataDNTTUChiTiet.fGiaTriThuHoiUngNgoaiChiTieu = UnFormatNumber(value.fGiaTriThuHoiUngNgoaiChiTieu);
            fTongGiaTriThuHoiUngNgoaiChiTieu += parseFloat(dataDNTTUChiTiet.fGiaTriThuHoiUngNgoaiChiTieu);
            listDNTTUChiTiet.push(dataDNTTUChiTiet);
        });
        dataDNTTU.fGiaTriThanhToan = fTongGiaTriThanhToan;
        dataDNTTU.fGiaTriThuHoiUngNgoaiChiTieu = fTongGiaTriThuHoiUngNgoaiChiTieu;

        data = {
            dataDNTTU: dataDNTTU,
            listDNTTUChiTiet: listDNTTUChiTiet
        };

        $.ajax({
            type: "POST",
            url: "/QLPheDuyetVonUngNgoaiChiTieu/CheckExistSoDeNghi",
            data: { iID_DeNghiThanhToanID: dataDNTTU.iID_DeNghiThanhToanID, sSoDeNghi: dataDNTTU.sSoDeNghi },
            success: function (r) {
                if (r == "True") {
                    var Title = 'Lỗi lưu phê duyệt vốn ứng ngoài chỉ tiêu';
                    var messErr = [];
                    messErr.push('Số phê duyệt đã tồn tại!');
                    $.ajax({
                        type: "POST",
                        url: "/Modal/OpenModal",
                        data: { Title: Title, Messages: messErr, Category: ERROR },
                        success: function (data) {
                            $("#divModalConfirm").html(data);
                        }
                    });
                    //alert("Số phê duyệt đã tồn tại!");
                    return false;
                } else {
                    $.ajax({
                        type: "POST",
                        url: "/QLPheDuyetVonUngNgoaiChiTieu/QLPheDuyetVonUngNCTSave",
                        data: { data: data },
                        success: function (r) {
                            if (r == "True") {
                                window.location.href = "/QLVonDauTu/QLPheDuyetVonUngNgoaiChiTieu/Index";
                            }
                        }
                    });
                }
            }
        });
        //$.ajax({
        //    type: "POST",
        //    url: "/QLPheDuyetVonUngNgoaiChiTieu/QLPheDuyetVonUngNCTSave",
        //    data: { data: data },
        //    success: function (r) {
        //        if (r == "True") {
        //            window.location.href = "/QLVonDauTu/QLPheDuyetVonUngNgoaiChiTieu/Index";
        //        }
        //    }
        //});
    }
}

function CheckLoi() {
    var messErr = [];
    var iID_DonViQuanLyID = $("#iID_DonViQuanLyID option:selected").val();
    var sSoDeNghi = $("#sSoDeNghi").val();
    var dNgayQuyetDinh = $("#dNgayQuyetDinh").val();
    var iID_NhomQuanLyID = $("#iID_NhomQuanLyID option:selected").val();

    if (iID_DonViQuanLyID == null) {
        messErr.push("Chưa chọn đơn vị quản lý!");
    }
    if (sSoDeNghi.trim() == "") {
        messErr.push("Thông tin Số phê duyệt chưa có hoặc chưa chính xác!");
    }
    if (dNgayQuyetDinh.trim() == "") {
        messErr.push("Thông tin Ngày lập chưa có hoặc chưa chính xác!");
    }
    if (iID_NhomQuanLyID == null) {
        messErr.push("Chưa chọn nhóm quản lý!");
    }

    if (messErr.length > 0) {
        var Title = 'Lỗi lưu phê duyệt vốn ứng ngoài chỉ tiêu';
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: messErr, Category: ERROR },
            success: function (data) {
                $("#divModalConfirm").html(data);
            }
        });
        return false;
    } else {
        return true;
    }
}

/*NinhNV start*/

function SearchData(iCurrentPage = 1) {
    var iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
    var sSoDeNghi = $("#sSoDeNghi").val();
    var dNgayDeNghiFrom = $("#dNgayDeNghiFrom").val();
    var dNgayDeNghiTo = $("#dNgayDeNghiTo").val();
    GetListData(iID_DonViQuanLyID, sSoDeNghi, dNgayDeNghiFrom, dNgayDeNghiTo, iCurrentPage);
}

function SearchDataReset(iCurrentPage = 1) {
    var iID_DonViQuanLyID = "";
    var sSoDeNghi = "";
    var dNgayDeNghiFrom = null;
    var dNgayDeNghiTo = null;
    GetListData(iID_DonViQuanLyID, sSoDeNghi, dNgayDeNghiFrom, dNgayDeNghiTo, iCurrentPage);
}

function ChangePage(iCurrentPage) {
    var iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
    var sSoDeNghi = $("#sSoDeNghi").val();
    var dNgayDeNghiFrom = $("#dNgayDeNghiFrom").val();
    var dNgayDeNghiTo = $("#dNgayDeNghiTo").val();
    GetListData(iID_DonViQuanLyID, sSoDeNghi, dNgayDeNghiFrom, dNgayDeNghiTo, iCurrentPage);
}

function GetListData(iID_DonViQuanLyID, sSoDeNghi, dNgayDeNghiFrom, dNgayDeNghiTo, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { iID_DonViQuanLyID: iID_DonViQuanLyID, sSoDeNghi: sSoDeNghi, dNgayDeNghiFrom: dNgayDeNghiFrom, dNgayDeNghiTo: dNgayDeNghiTo, _paging: _paging },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#iID_DonViQuanLyID").val(iID_DonViQuanLyID);
            $("#sSoDeNghi").val(sSoDeNghi);
            $("#dNgayDeNghiFrom").val(dNgayDeNghiFrom);
            $("#dNgayDeNghiTo").val(dNgayDeNghiTo);
        }
    });
}

function formatMoneyOfLstPDVUNCT() {
    $('#tblListVDTPDVUNCT tr').each(function () {
        var fGiaTriThanhToan = $(this).find(".fGiaTriThanhToan").text();
        if (fGiaTriThanhToan) {
            fGiaTriThanhToan = FormatNumber(fGiaTriThanhToan);
            $(this).find(".fGiaTriThanhToan").text(fGiaTriThanhToan);
        }
        var fGiaTriThuHoiUngNgoaiChiTieu = $(this).find(".fGiaTriThuHoiUngNgoaiChiTieu").text();
        if (fGiaTriThuHoiUngNgoaiChiTieu) {
            fGiaTriThuHoiUngNgoaiChiTieu = FormatNumber(fGiaTriThuHoiUngNgoaiChiTieu);
            $(this).find(".fGiaTriThuHoiUngNgoaiChiTieu").text(fGiaTriThuHoiUngNgoaiChiTieu);
        }
    });
}

function GetItemDNTTU(id) {
    window.location.href = "/QLVonDauTu/QLPheDuyetVonUngNgoaiChiTieu/CreateNew/" + id;
}

function BtnCreateDataClick() {
    window.location.href = "/QLVonDauTu/QLPheDuyetVonUngNgoaiChiTieu/CreateNew/";
}

function GetDNTTUChiTiet() {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLPheDuyetVonUngNgoaiChiTieu/GetDNTTUChiTiet",
        data: {
            iId: $("#iID_DeNghiThanhToanID").val()
        },
        success: function (r) {
            if (r.bIsComplete) {
                ConvertListDetailToGrid(r.data, 1);
            }
        }
    });
}

function GetDNTTUChiTietViewDetail() {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLPheDuyetVonUngNgoaiChiTieu/GetDNTTUChiTiet",
        data: {
            iId: $("#iID_DeNghiThanhToanID").val()
        },
        success: function (r) {
            if (r.bIsComplete) {
                ConvertListDetailToGrid(r.data, 2);
            }
        }
    });
}

function DeleteItemPDVUNCT(id) {
    var Title = 'Xác nhận xóa phê duyệt vốn ứng ngoài chỉ tiêu';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn xóa?');
    var FunctionName = "DeletePDVUNCT('" + id + "')";
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
        success: function (data) {
            $("#divModalConfirm").html(data);
        }
    });
}

function DeletePDVUNCT(id) {
    $.ajax({
        type: "POST",
        url: "/QLPheDuyetVonUngNgoaiChiTieu/VDTPDVUNCTDelete",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                SearchData();
            }
        }
    });
}

function ViewItemDetail(id) {
    window.location.href = "/QLVonDauTu/QLPheDuyetVonUngNgoaiChiTieu/ViewDetail/" + id;
}

function CancelSaveData() {
    window.location.href = "/QLVonDauTu/QLPheDuyetVonUngNgoaiChiTieu/Index";
}

function GetItemData(id) {
    var dataCheck = $.map(tblDataGrid, function (n) { return n.iId == id ? n : null })[0];
    $("#bIsEdit").val("1");
    $("#iID_DuAnID").val(dataCheck.iID_DuAnID);
    $("#iID_DuAnID").change();

    $("#iID_HopDongID").val(dataCheck.iID_HopDongID);
    $("#iID_NhaThauID").val(dataCheck.iID_NhaThauID);
    $("#sGhiChu2").val(dataCheck.sGhiChu);
    $("#fGiaTriThanhToan").val(dataCheck.fGiaTriThanhToan);
    $("#fGiaTriThuHoiUngNgoaiChiTieu").val(dataCheck.fGiaTriThuHoiUngNgoaiChiTieu);

    //$("#fLKKHVUDuocDuyet").val(dataCheck.fLKKHVUDuocDuyet);
    //$("#sTenNhaThau").val(dataCheck.sTenNhaThau);
    //$("#fGiaTriHopDong").val(dataCheck.fGiaTriHopDong);
    //$("#fLKSoVonDaTamUng").val(dataCheck.fLKSoVonDaTamUng);
    //$("#sTenGoiThau").val(dataCheck.sTenGoiThau);
    //$("#sSoTaiKhoan").val(dataCheck.sSoTaiKhoan);
    //$("#sNganHang").val(dataCheck.sNganHang);
    //$("#fLKThuHoiUng").val(dataCheck.fLKThuHoiUng);

    if (dataCheck.iID_HopDongID != GUID_EMPTY) {
        LayThongTinHopDongVaLuyKeUng(dataCheck.iID_HopDongID);
    }
}
/*NinhNV end*/
