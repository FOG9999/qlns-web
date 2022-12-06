var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';

var objDuocDuyet = {};
var lstDuAn = [];
var lstDetail = [];
var lstMucLucNganSach = [];
var id;
var isUpdate = false;
var isDieuChinh = false;

$(document).ready(function () {
    if (window.location.href.indexOf('CreateNew') > 0) {
        id = $("#id").val();
        if (id != GUID_EMPTY) {
            if ($("#isDieuChinh").val() == "true") {
                isDieuChinh = true;
                $("#iID_DonViQuanLyID").attr('disabled', true);
                $("#dNgayQuyetDinh").attr('disabled', false);
                $("#iNamKeHoach").attr('disabled', true);
                $("#iId_NguonVon").attr('disabled', true);
                $("#iId_KeHoachVonUng").attr('disabled', true);

            } else {
                $("#iID_DonViQuanLyID").attr('disabled', true);
                $("#dNgayQuyetDinh").attr('disabled', true);
                $("#iNamKeHoach").attr('disabled', true);
                $("#iId_NguonVon").attr('disabled', true);
                $("#iId_KeHoachVonUng").attr('disabled', true);
            }


            GetDataDetailById();

        }
        GetDataDropDownDonVi();

        $("#dNgayQuyetDinh").change(function () {
            GetKeHoachVonUngDeXuat();
        });

        $("#iNamKeHoach").change(function () {
            GetKeHoachVonUngDeXuat();
        });
    }
    var isShowing = false;
    $('.date')
        .datepicker({
            todayBtn: "linked",
            keyboardNavigation: false,
            forceParse: false,
            autoclose: true,
            language: 'vi',
            todayHighlight: true,
        }).on('hide', () => {
            isShowing = false;
        }).on('show', () => {
            isShowing = true;
        });
    $("tr").hover(function () {
        $(this).css("background-color", "#e7f8fe");
    }, function () {
        $(this).css("background-color", "");
    });
});

function LayDanhSachDuAnTheoDonViQuanLyVaNgayQuyetDinh(iID_DonViQuanLyID, dNgayQuyetDinh) {
    $.ajax({
        url: "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/LayDanhSachDuAnTheoDonViQuanLyVaNgayQuyetDinh",
        type: "POST",
        data: { iID_DonViQuanLyID: iID_DonViQuanLyID, dNgayQuyetDinh: dNgayQuyetDinh },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null && data != "") {
                $("#iID_DuAnID").html(data);

                $("#iID_DuAnID").prop("selectedIndex", 0);
                $("#fGiaTriDauTu").val("");
            }
        },
        error: function (data) {

        }
    })
}

function GetGiaTriDauTu(iID_DuAnID, dNgayQuyetDinh) {
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

function GetDataGridViewDefault(id, type) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/GetDataGridViewDefault",
        data: { id: id },
        success: function (r) {
            if (r.bIsComplete) {
                tblDataGrid = r.data;
                iIdMax = tblDataGrid.length;
                FormatNumberKHVUChiTiet(tblDataGrid);
                sumGiaTriDauTuVaVonUng(tblDataGrid);
                if (type == 1) {
                    FillDataToGridViewInsert(r.data);
                } else {
                    FillDataToGridViewDetail(r.data);
                }
            }
        }
    });
}

function InsertDetailData() {
    var iID_DuAnID = $("#iID_DuAnID").val();
    if (iID_DuAnID == null || $("#iID_DuAnID").val() === GUID_EMPTY || $("#fGiaTriUng").val() == '') {
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
    iIdMax++;
    data.iId = iIdMax;
    //data.iId = tblDataGrid.length + 1;
    data.iParentId = 1;
    data.sMaKetNoi = '';
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
    $("#tong_giatriung").html(fTongGiaTriUng == 0 ? 0 : FormatNumber(fTongGiaTriUng))
}

function FillDataToGridViewInsert(data) {
    var columns = [
        { sField: "iId", bKey: true },
        { sField: "iParentId", bParentKey: true },
        { sTitle: "Mã kết nối", sField: "sMaKetNoi", iWidth: "10%", sTextAligsn: "left" },
        { sTitle: "Dự án", sField: "sTenDuAn", iWidth: "50%", sTextAlign: "left", bHaveIcon: 1 },
        { sTitle: "Tổng mức đầu tư", sField: "fGiaTriDauTu", iWidth: "10%", sTextAlign: "right" },
        { sTitle: "Kế hoạch vốn ứng được duyệt", sField: "fGiaTriUng", iWidth: "15%", sTextAlign: "right" }];

    var button = { bCreateButtonInParent: false, bUpdate: 1, bDelete: 1 };
    var sHtml = GenerateTreeTable(data, columns, button)
    $("#ViewTable").html(sHtml);
    ClearButtonInParent();
}

function FillDataToGridViewDetail(data) {
    var columns = [
        { sField: "iId", bKey: true },
        { sField: "iParentId", bParentKey: true },
        { sTitle: "Mã kết nối", sField: "sMaKetNoi", iWidth: "10%", sTextAligsn: "left" },
        { sTitle: "Dự án", sField: "sTenDuAn", iWidth: "50%", sTextAlign: "left", bHaveIcon: 1 },
        { sTitle: "Tổng mức đầu tư", sField: "fGiaTriDauTu", iWidth: "10%", sTextAlign: "right" },
        { sTitle: "Kế hoạch vốn ứng được duyệt", sField: "fGiaTriUng", iWidth: "15%", sTextAlign: "right" }];

    var button = { bCreateButtonInParent: false, bUpdate: 0, bDelete: 0 };
    var sHtml = GenerateTreeTable(data, columns, button)
    $("#ViewTable").html(sHtml);
    ClearButtonInParent();
}

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

function GetItemData(iId) {
    var data = $.map(tblDataGrid, function (n) { return n.iId == iId ? n : null })[0];
    $("#iID_DuAnID").val(data.iID_DuAnID);
    $("#fGiaTriDauTu").val(data.fGiaTriDauTu);
    $("#fGiaTriUng").val(data.fGiaTriUng);
}

function DeleteItem(iId) {
    tblDataGrid = $.map(tblDataGrid, function (n) { return n.iId != iId ? n : null });
    sumGiaTriDauTuVaVonUng(tblDataGrid);
    FillDataToGridViewInsert(tblDataGrid);
}

function Luu() {
    if (CheckLoi()) {
        var dataKHVU = {};
        var listKHVUChiTiet = [];
        var data = {};

        dataKHVU.iID_KeHoachUngID = $("#iID_KeHoachUngID").val();
        dataKHVU.iID_DonViQuanLyID = $("#iID_DonViQuanLyID option:selected").val();
        dataKHVU.sSoQuyetDinh = $("#sSoQuyetDinh").val();
        dataKHVU.dNgayQuyetDinh = $("#dNgayQuyetDinh").val();
        dataKHVU.iID_NhomQuanLyID = $("#iID_NhomQuanLyID option:selected").val();
        dataKHVU.fGiaTriUng = UnFormatNumber($("#tong_giatriung").html());

        tblDataGrid = $.map(tblDataGrid, function (n) { return n.iParentId == 1 ? n : null });
        $.each(tblDataGrid, function (index, value) {
            var dataKHVUChiTiet = {};
            dataKHVUChiTiet.iID_DuAnID = value.iID_DuAnID;
            dataKHVUChiTiet.fGiaTriUng = UnFormatNumber(value.fGiaTriUng);

            listKHVUChiTiet.push(dataKHVUChiTiet);
        });

        data = {
            dataKHVU: dataKHVU,
            listKHVUChiTiet: listKHVUChiTiet
        };

        $.ajax({
            type: "POST",
            url: "/QLKeHoachVonUngDuocDuyet/CheckExistSoQuyetDinh",
            data: { iID_KeHoachUngID: dataKHVU.iID_KeHoachUngID, sSoQuyetDinh: dataKHVU.sSoQuyetDinh },
            success: function (r) {
                if (r == "True") {
                    var Title = 'Lỗi lưu kế hoạch vốn ứng được duyệt';
                    var messErr = [];
                    messErr.push('Số kế hoạch đã tồn tại!');
                    $.ajax({
                        type: "POST",
                        url: "/Modal/OpenModal",
                        data: { Title: Title, Messages: messErr, Category: ERROR },
                        success: function (data) {
                            $("#divModalConfirm").html(data);
                        }
                    });
                    //alert("Số kế hoạch đã tồn tại!");
                    return false;
                } else {
                    $.ajax({
                        type: "POST",
                        url: "/QLKeHoachVonUngDuocDuyet/QLKeHoachVonUngSave",
                        data: { data: data },
                        success: function (r) {
                            if (r == "True") {
                                window.location.href = "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/Index";
                            }
                        }
                    });
                }
            }
        });

        //$.ajax({
        //    type: "POST",
        //    url: "/QLKeHoachVonUngDuocDuyet/QLKeHoachVonUngSave",
        //    data: { data: data },
        //    success: function (r) {
        //        if (r == "True") {
        //            window.location.href = "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/Index";
        //        }
        //    }
        //});
    }
}

function CheckLoi() {
    var messErr = [];
    var iID_DonViQuanLyID = $("#iID_DonViQuanLyID option:selected").val();
    var sSoQuyetDinh = $("#sSoQuyetDinh").val();
    var dNgayQuyetDinh = $("#dNgayQuyetDinh").val();
    var iID_NhomQuanLyID = $("#iID_NhomQuanLyID option:selected").val();

    if (iID_DonViQuanLyID == null) {
        messErr.push("Chưa chọn đơn vị quản lý.");
    }
    if (sSoQuyetDinh.trim() == "") {
        messErr.push("Thông tin Số kế hoạch chưa có hoặc chưa chính xác.");
    }
    if (dNgayQuyetDinh.trim() == "") {
        messErr.push("Thông tin Ngày lập chưa có hoặc chưa chính xác.");
    }
    if (iID_NhomQuanLyID == null) {
        messErr.push("Chưa chọn nhóm quản lý.");
    }

    if (messErr.length > 0) {
        var Title = 'Lỗi lưu kế hoạch vốn ứng được duyệt';
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
    var sSoQuyetDinh = $("#sSoQuyetDinh").val();
    var dNgayQuyetDinhFrom = $("#dNgayQuyetDinhFrom").val();
    var dNgayQuyetDinhTo = $("#dNgayQuyetDinhTo").val();
    var iID_MaDonViQuanLyID = null;
    if ($("#iID_DonViQuanLyID").val() != undefined && $("#iID_DonViQuanLyID").val() != null) {
        iID_MaDonViQuanLyID = $("#iID_DonViQuanLyID").val().split("|")[0];
    }
    var iNamKeHoach = $("#iNamKeHoach").val();
    var iIdNguonVon = $("#iId_NguonVon").val();
    GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iID_MaDonViQuanLyID, iNamKeHoach, iIdNguonVon, iCurrentPage);
}
function SearchDataReset(iCurrentPage = 1) {
    var sSoQuyetDinh = null;
    var dNgayQuyetDinhFrom = null;
    var dNgayQuyetDinhTo = null;
    var iID_MaDonViQuanLyID = null;
    var iNamKeHoach = null;
    var iIdNguonVon = null;

    GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iID_MaDonViQuanLyID, iNamKeHoach, iIdNguonVon, iCurrentPage);
}

function ChangePage(iCurrentPage) {
    SearchData(iCurrentPage);
}

function GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iID_MaDonViQuanLyID, iNamKeHoach, iIdNguonVon, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: {
            sSoQuyetDinh: sSoQuyetDinh,
            dNgayQuyetDinhFrom: dNgayQuyetDinhFrom,
            dNgayQuyetDinhTo: dNgayQuyetDinhTo,
            iID_DonViQuanLyID: iID_MaDonViQuanLyID,
            iNamKeHoach: iNamKeHoach,
            iIdNguonVon: iIdNguonVon,
            _paging: _paging
        },
        success: function (data) {
            $("#lstDataView").html(data);
            formatMoneyOfLstKHVU();
        }
    });
}

function formatMoneyOfLstKHVU() {
    $('#tblListVDTKHVU tr').each(function () {
        var fGiaTriUng = $(this).find(".fGiaTriUng").text();
        if (fGiaTriUng) {
            fGiaTriUng = FormatNumber(fGiaTriUng);
            $(this).find(".fGiaTriUng").text(fGiaTriUng);
        }
    });
}

function GetItemKHVU(id) {
    window.location.href = "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/CreateNew/" + id;
}

function BtnCreateDataClick() {
    window.location.href = "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/CreateNew/";
}

function BtnDieuChinh(id, flagDieuChinh) {
    isDieuChinh = flagDieuChinh;
    window.location.href = "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/CreateNew?id=" + id + "&isDieuChinh=" + isDieuChinh;
}

function DeleteItemKHVU(id) {
    var Title = 'Xác nhận xóa kế hoạch vốn ứng được duyệt';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn xóa?');
    var FunctionName = "DeleteKHVU('" + id + "')";
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: Messages, Category: Title, FunctionName: FunctionName },
        success: function (data) {
            $("#divModalConfirm").html(data);
        }
    });
}

function DeleteKHVU(id) {
    $.ajax({
        type: "POST",
        url: "/QLKeHoachVonUngDuocDuyet/VDTKHVUDelete",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                SearchData();
            }
        }
    });
}

function ViewItemDetail(id) {
    window.location.href = "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/ViewDetail/" + id;
}

/*NinhNV end*/

//================== LongDT ======================//
//================== Index ======================//

//================== Create =====================//
function GetDataDetailById() {
    var id = $("#id").val();

    $.ajax({
        url: "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/GetKeHoachVonUngDetail",
        type: "GET",
        data: { id: id },
        dataType: "json",
        async: false,
        success: function (result) {
            lstDetail = [];
            if (result.status) {
                $.each(result.datas, function (index, item) {
                    var objData = {};
                    objData.iID_DuAnID = item.iID_DuAnID;
                    objData.sTenDuAn = item.sTenDuAn;
                    objData.sMaDuAn = item.sMaDuAn;

                    objData.sLNS = item.sLNS;
                    objData.sL = item.sL;
                    objData.sK = item.sK;
                    objData.sM = item.sM;
                    objData.sTM = item.sTM;
                    objData.sTTM = item.sTTM;
                    objData.sNG = item.sNG;

                    objData.sTrangThaiDuAnDangKy = item.sTrangThaiDuAnDangKy;
                    objData.sXauNoiMa = item.sXauNoiMa;
                    objData.iID_MucID = item.iID_MucID;
                    objData.iID_TieuMucID = item.iID_TieuMucID;
                    objData.iID_TietMucID = item.iID_TietMucID;
                    objData.iID_NganhID = item.iID_NganhID;
                    objData.fGiaTriDeNghi = item.fGiaTriDeNghi;
                    objData.sGiaTriDeNghi = FormatNumber(item.fGiaTriDeNghi);
                    objData.fTongMucDauTu = item.fTongMucDauTu;
                    objData.sTongMucDauTu = FormatNumber(item.fTongMucDauTu);
                    objData.fCapPhatTaiKhoBac = item.fCapPhatTaiKhoBac;
                    objData.sCapPhatTaiKhoBac = FormatNumber(item.fCapPhatTaiKhoBac);
                    objData.fCapPhatBangLenhChi = item.fCapPhatBangLenhChi;
                    objData.sCapPhatBangLenhChi = FormatNumber(item.fCapPhatBangLenhChi);
                    objData.sGhiChu = item.sGhiChu;
                    objData.isDelete = false;
                    lstDetail.push(objData);
                });
                DrawDetailTableCT();
            }
        }
    })
}

function GetKeHoachVonUngDeXuat() {
    var sMaDonVi = $("#iID_DonViQuanLyID").val();
    var dNgayDenghi = $("#dNgayQuyetDinh").val();
    var iNamKeHoach = $("#iNamKeHoach").val();
    $.ajax({
        url: "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/GetKeHoachVonUngDeXuat",
        type: "POST",
        data: { sMaDonVi: sMaDonVi, iNamKeHoach: iNamKeHoach, dNgayDenghi: dNgayDenghi },
        dataType: "json",
        cache: false,
        success: function (result) {
            $("#iId_KeHoachVonUng").empty();
            if (result.status) {
                $.each(result.datas, function (index, value) {
                    lstDetail.push(value);
                    $("<option />", {
                        val: value.Id,
                        text: value.sSoDeNghi
                    }).appendTo($("#iId_KeHoachVonUng"));
                });
                $("#iId_KeHoachVonUng").val(null);
                $("#iId_KeHoachVonUng").change(function () {
                    GetDuAnByKeHoachVonUngDeXuat();
                });
            }

            $("#iId_KeHoachVonUng").val($("#iIdKeHoachVonUngDeXuatId").val());
            if (id == GUID_EMPTY)
                $("#iId_KeHoachVonUng").change();
        }
    });
}

function GetDuAnByKeHoachVonUngDeXuat() {
    var id = $("#iId_KeHoachVonUng").val();
    $("#iID_DuAnID").empty();
    lstDetail = [];
    if (id == null || id == "") return;
    $.ajax({
        url: "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/GetDuAnByKeHoachVonUngDeXuat",
        type: "GET",
        data: { id: id },
        dataType: "json",
        cache: false,
        success: function (result) {
            lstDetail = [];
            if (result.status) {
                result.datas.forEach(x => {
                    var newItem = {
                        iID_DuAnID: x.iID_DuAnID,
                        sMaDuAn: x.sMaDuAn,
                        sTenDuAn: x.sTenDuAn,
                        sLNS: '',
                        sL: '',
                        sK: '',
                        sM: '',
                        sTM: '',
                        sTTM: '',
                        sNG: '',
                        iID_NganhID: GUID_EMPTY,
                        fGiaTriDeNghi: x.fGiaTriDeNghi,
                        sGiaTriDeNghi: FormatNumber(x.fGiaTriDeNghi),
                        fTongMucDauTu: x.fTongMucDauTu,
                        sTongMucDauTu: FormatNumber(x.fTongMucDauTu),
                        fCapPhatTaiKhoBac: x.fCapPhatTaiKhoBac,
                        sCapPhatTaiKhoBac: FormatNumber(x.fCapPhatTaiKhoBac),
                        fCapPhatBangLenhChi: x.fCapPhatBangLenhChi,
                        sCapPhatBangLenhChi: FormatNumber(x.fCapPhatBangLenhChi),
                        sTrangThaiDuAnDangKy: x.sTrangThaiDuAnDangKy,
                        sGhiChu: x.sGhiChu,
                        isDelete: false
                    };
                    lstDetail.push(newItem);
                });

                DrawDetailTable();
                SumValueDetailTable();
            }
        }
    });
}

function GetDataDropDownDonVi() {
    $("#iID_DonViQuanLyID").empty();
    $.ajax({
        url: "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/GetDonViQuanLy",
        type: "GET",
        dataType: "json",
        cache: false,
        async: true,
        success: function (result) {
            if (result.status) {
                var strOption = [];
                $.each(result.datas, function (index, item) {
                    strOption.push("<option value='" + item.iID_MaDonVi + "' data-idDonVi='" + item.iID_Ma + "'>" + item.iID_MaDonVi + " - " + item.sTen + "</option>")
                });
                $("#iID_DonViQuanLyID").append(strOption.join(""));

                $("#iID_DonViQuanLyID").change(function () {
                    GetKeHoachVonUngDeXuat();
                });
            }
            $("#iID_DonViQuanLyID").val($("#sMaDonViQuanLy").val());
            $("#iID_DonViQuanLyID").change();
        }
    })
}

var lstDuAnId = [];
function EventCheckbox() {
    $("#tblDanhSachDuAn [type=checkbox]").on("change", function () {
        lstDuAnId = [];
        $.each($("#tblDanhSachDuAn [type=checkbox]"), function (index, item) {
            if (item.checked) {
                lstDuAnId.push($(item).data('id'));
            }
        })
    })
    lstDuAnId = [];
    $.each($("#tblDanhSachDuAn [type=checkbox]"), function (index, item) {
        if (item.checked) {
            lstDuAnId.push($(item).data('id'));
        }
    })
}

function LoadDefaultDuAnChecked() {
    if (lstDetail != undefined && lstDetail != "") {
        $.each(lstDuAnId, function (item, value) {
            $("[data-id='" + value.iID_DuAnID + "']").prop("checked", false);
        });
    }
}

function CheckDupicateSoQuyetDinh(iID_KeHoachUngID, sSoQuyetDinh) {
    $.ajax({
        url: "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/CheckExistSoQuyetDinh",
        type: "POST",
        data: { iID_KeHoachUngID: iID_KeHoachUngID, sSoQuyetDinh: sSoQuyetDinh },
        dataType: "json",
        async: false,
        success: function (data) {
            if (data.status) {
                alert("Số kế hoạch " + sSoQuyetDinh + " đã tồn tại .")
            }
            return !status;
        }
    });
}

function DrawDetailTable() {
    $("#tblDanhSachDuAn tbody").empty();
    var sHtml = [];
    $.each(lstDetail, function (index, value) {
        sHtml.push("<tr data-id='" + value.iID_DuAnID + "' data-xoa='0'>");
        sHtml.push(`    <td><input type='checkbox' data-id='${value.iID_DuAnID}'/></td>'`);
        sHtml.push("    <td class='text-center'>" + value.sMaDuAn + "</td>");
        sHtml.push("    <td>" + value.sTenDuAn + "</td>");
        //sHtml.push("    <td><input type='text' onblur='UpdateDuAn(this)' class='txtLNS form-control text-center' value='" + (value.sLNS != null ? value.sLNS : '') + "'/></td>");
        //sHtml.push("    <td><input type='text' onblur='UpdateDuAn(this)' class='txtL form-control text-center' value='" + (value.sL != null ? value.sL : '') + "'/></td>");
        //sHtml.push("    <td><input type='text' onblur='UpdateDuAn(this)' class='txtK form-control text-center' value='" + (value.sK != null ? value.sK : '') + "'/></td>");
        //sHtml.push("    <td><input type='text' onblur='UpdateDuAn(this)' class='txtM form-control text-center' value='" + (value.sM != null ? value.sM : '') + "'/></td>");
        //sHtml.push("    <td><input type='text' onblur='UpdateDuAn(this)' class='txtTM form-control text-center' value='" + (value.sTM != null ? value.sTM : '') + "'/></td>");
        //sHtml.push("    <td><input type='text' onblur='UpdateDuAn(this)' class='txtTTM form-control text-center' value='" + (value.sTTM != null ? value.sTTM : '') + "'/></td>");
        //sHtml.push("    <td><input type='text' onblur='UpdateDuAn(this)' class='txtNG form-control text-center' value='" + (value.sNG != null ? value.sNG : '') + "'/></td>");
        //sHtml.push("    <td class='text-right'>" + value.sTongMucDauTu + "</td>");
        //sHtml.push("    <td class='text-right'>" + value.sGiaTriDeNghi + "</td>");
        //sHtml.push("    <td class='text-right'><input type='text' onblur='UpdateDuAn(this)' class='txtCapPhatTaiKhoBac form-control text-right' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value='" + value.sCapPhatTaiKhoBac + "'/></td>");
        //sHtml.push("    <td class='text-right'><input type='text' onblur='UpdateDuAn(this)' class='txtCapPhatBangLenhChi form-control text-right' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value='" + value.sCapPhatBangLenhChi + "'/></td>");
        //sHtml.push("    <td><input type='text' class='txtGhiChu form-control' onblur='UpdateDuAn(this)' value='" + (value.sGhiChu != null ? value.sGhiChu : '') + "'/></td>");
        //sHtml.push("    <td><button class= 'btn-delete'><i class='fa fa-trash-o fa-lg' aria-hidden='true' onclick=\"DeleteDetailItem(this)\"></i></button></td>");
        sHtml.push("</tr>");
    });
    $("#tblDanhSachDuAn tbody").html(sHtml.join());
    EventCheckbox();
    LoadDefaultDuAnChecked();
}

function DrawDetailTableCT() {
    $("#tblDanhSachDuAn tbody").empty();
    var sHtml = [];
    $.each(lstDetail, function (index, value) {
        sHtml.push("<tr data-id='" + value.iID_DuAnID + "' data-xoa='0'>");
        sHtml.push(`    <td><input type='checkbox' checked data-id='${value.iID_DuAnID}' /></td>'`);
        sHtml.push("    <td class='text-center'>" + value.sMaDuAn + "</td>");
        sHtml.push("    <td>" + value.sTenDuAn + "</td>");
        //sHtml.push("    <td><input type='text' onblur='UpdateDuAn(this)' class='txtLNS form-control text-center' value='" + (value.sLNS != null ? value.sLNS : '') + "'/></td>");
        //sHtml.push("    <td><input type='text' onblur='UpdateDuAn(this)' class='txtL form-control text-center' value='" + (value.sL != null ? value.sL : '') + "'/></td>");
        //sHtml.push("    <td><input type='text' onblur='UpdateDuAn(this)' class='txtK form-control text-center' value='" + (value.sK != null ? value.sK : '') + "'/></td>");
        //sHtml.push("    <td><input type='text' onblur='UpdateDuAn(this)' class='txtM form-control text-center' value='" + (value.sM != null ? value.sM : '') + "'/></td>");
        //sHtml.push("    <td><input type='text' onblur='UpdateDuAn(this)' class='txtTM form-control text-center' value='" + (value.sTM != null ? value.sTM : '') + "'/></td>");
        //sHtml.push("    <td><input type='text' onblur='UpdateDuAn(this)' class='txtTTM form-control text-center' value='" + (value.sTTM != null ? value.sTTM : '') + "'/></td>");
        //sHtml.push("    <td><input type='text' onblur='UpdateDuAn(this)' class='txtNG form-control text-center' value='" + (value.sNG != null ? value.sNG : '') + "'/></td>");
        //sHtml.push("    <td class='text-right'>" + value.sTongMucDauTu + "</td>");
        //sHtml.push("    <td class='text-right'>" + value.sGiaTriDeNghi + "</td>");
        //sHtml.push("    <td class='text-right'><input type='text' onblur='UpdateDuAn(this)' class='txtCapPhatTaiKhoBac form-control text-right' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value='" + value.sCapPhatTaiKhoBac + "'/></td>");
        //sHtml.push("    <td class='text-right'><input type='text' onblur='UpdateDuAn(this)' class='txtCapPhatBangLenhChi form-control text-right' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' value='" + value.sCapPhatBangLenhChi + "'/></td>");
        //sHtml.push("    <td><input type='text' class='txtGhiChu form-control' onblur='UpdateDuAn(this)' value='" + (value.sGhiChu != null ? value.sGhiChu : '') + "'/></td>");
        //sHtml.push("    <td><button class= 'btn-delete'><i class='fa fa-trash-o fa-lg' aria-hidden='true' onclick=\"DeleteDetailItem(this)\"></i></button></td>");
        sHtml.push("</tr>");
    });
    $("#tblDanhSachDuAn tbody").html(sHtml.join());
    EventCheckbox();
    LoadDefaultDuAnChecked();
}

function UpdateDuAn(hienTai) {
    var dongHienTai = hienTai.closest("tr");
    var id = $(dongHienTai).attr("data-id");

    var sLNS = $(dongHienTai).find(".txtLNS").val();
    var sL = $(dongHienTai).find(".txtL").val();
    var sK = $(dongHienTai).find(".txtK").val();
    var sM = $(dongHienTai).find(".txtM").val();
    var sTM = $(dongHienTai).find(".txtTM").val();
    var sTTM = $(dongHienTai).find(".txtTTM").val();
    var sNG = $(dongHienTai).find(".txtNG").val();

    var sCapPhatTaiKhoBac = $(dongHienTai).find(".txtCapPhatTaiKhoBac").val();
    var fCapPhatTaiKhoBac = sCapPhatTaiKhoBac == "" ? 0 : parseInt(UnFormatNumber(sCapPhatTaiKhoBac));

    var sCapPhatBangLenhChi = $(dongHienTai).find(".txtCapPhatBangLenhChi").val();
    var fCapPhatBangLenhChi = sCapPhatBangLenhChi == "" ? 0 : parseInt(UnFormatNumber(sCapPhatBangLenhChi));

    var sGhiChu = $(dongHienTai).find(".txtGhiChu").val();

    var objDuAn = lstDetail.filter(x => x.iID_DuAnID == id)[0];

    objDuAn.sLNS = sLNS;
    objDuAn.sL = sL;
    objDuAn.sK = sK;
    objDuAn.sM = sM;
    objDuAn.sTM = sTM;
    objDuAn.sTTM = sTTM;
    objDuAn.sNG = sNG;

    objDuAn.fCapPhatTaiKhoBac = fCapPhatTaiKhoBac;
    objDuAn.fCapPhatBangLenhChi = fCapPhatBangLenhChi;

    objDuAn.sGhiChu = sGhiChu;

    var sXauNoiMa = sLNS + "-" + sL + "-" + sK + "-" + sM + "-" + sTM + "-" + sTTM + "-" + sNG;
    sXauNoiMa = sXauNoiMa.replace(/[-]+$/g, '');

    // neu sua tien thi check ton tai MLNS
    if (sXauNoiMa != '' && ($(hienTai).hasClass('txtCapPhatTaiKhoBac') || $(hienTai).hasClass('txtCapPhatBangLenhChi'))) {
        if (CheckExistMLNS(sXauNoiMa) == false) {
            alert("Mục lục ngân sách không tồn tại.");
            DeleteDetailItem(hienTai);
        } else {
            objDuAn.sXauNoiMa = sXauNoiMa;
        }
    }

    lstDetail = lstDetail.filter(function (x) { return x.iID_DuAnID != id });
    lstDetail.push(objDuAn);
    SumValueDetailTable();
}

function SumValueDetailTable() {
    var fSumTongMucDauTu = 0;
    var fSumCapPhatTaiKhoBac = 0;
    var fSumCapPhatBangLenhChi = 0;
    $.each(lstDetail, function (index, item) {
        if (item.isDelete == undefined || item.isDelete == false) {
            fSumTongMucDauTu += parseFloat(ConvertNumber(item.fTongMucDauTu));
            fSumCapPhatTaiKhoBac += parseFloat(ConvertNumber(item.fCapPhatTaiKhoBac));
            fSumCapPhatBangLenhChi += parseFloat(ConvertNumber(item.fCapPhatBangLenhChi));
        }
    });
    $("#txtSumTongMucDauTu").text(Number(fSumTongMucDauTu).toLocaleString('vi-VN'));
    $("#txtSumCapPhatTaiKhoBac").text(Number(fSumCapPhatTaiKhoBac).toLocaleString('vi-VN'));
    $("#txtSumCapPhatBangLenhChi").text(Number(fSumCapPhatBangLenhChi).toLocaleString('vi-VN'));
}

function DeleteDetailItem(nutXoa) {
    var dongXoa = nutXoa.closest('tr');
    var id = $(dongXoa).data('id');
    var checkXoa = $(dongXoa).attr('data-xoa');
    if (checkXoa == 1) {
        $(dongXoa).attr('data-xoa', '0');
        $(dongXoa).removeClass('error-row');
    } else {
        $(dongXoa).attr('data-xoa', '1');
        $(dongXoa).addClass('error-row');
    }

    var objXoa = lstDetail.filter(function (x) { return x.iID_DuAnID == id })[0];
    objXoa.isDelete = !objXoa.isDelete;
    lstDetail = lstDetail.filter(function (x) { return x.iID_DuAnID != id });
    lstDetail.push(objXoa);

    if (checkXoa == 1) {
        $(dongXoa).find('input').prop("disabled", "");
    } else {
        $(dongXoa).find('input').prop("disabled", "disabled");
    }

    SumValueDetailTable();
}

function CancelSaveData() {
    window.location.href = "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/Index";
}

function Insert() {
    objDuocDuyet = {};
    var selectedDuAn = [];
    var iId_KHVUDX_ID = $("#iId_KeHoachVonUng :selected").val();
    //var idKHVU = $("#id").val();
    lstDetail.forEach(duAn => {
        if (lstDuAnId.indexOf(duAn.iID_DuAnID) >= 0) {
            selectedDuAn.push(duAn);
        }
    })
    if (!ValidateData()) return;

    objDuocDuyet.listKHVUChiTiet = selectedDuAn;
    $.ajax({
        url: "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/QLKeHoachVonUngSave",
        type: "POST",
        data: { data: objDuocDuyet, isDieuChinh: isDieuChinh },
        dataType: "json",
        async: false,
        success: function (result) {
            if (result.status) {
                var messAlert = "Tạo mới dữ liệu thành công.";
                if (isDieuChinh) {
                    messAlert = "Điều chỉnh dữ liệu thành công.";

                } else {
                    if (!(id == GUID_EMPTY || id == undefined || id == "" || id == null)) {
                        messAlert = "Cập nhật dữ liệu thành công.";
                        isUpdate = true;
                    }
                }
                alert(messAlert);
                $.ajax({
                    type: "POST",
                    dataType: "json",
                    url: "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/GetListDuAnKHVUDX",
                    data: {
                        listData: lstDuAnId,
                        iId_KHVUDX_ID: iId_KHVUDX_ID
                    },
                    success: function (data) {
                        if (data.status) {
                            window.location.href = "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/Detail?iID_KeHoachUngID=" + result.ID + "&isUpdate=" + isUpdate + "&isDieuChinh=" + isDieuChinh;

                        } else {
                            alert("Không có dữ liệu.");
                        }

                    }
                });
            } else {
                if (messError)
                    alert(messError);
                else
                    alert("Có lỗi xảy ra khi lưu dữ liệu.");
            }
        }
    })
}

function ValidateData() {
    var messErrors = [];
    var iIdMaDonVi = $("#iID_DonViQuanLyID").val();
    var sSoQuyetDinh = $("#sSoQuyetDinh").val();
    var dNgayQuyetDinh = $("#dNgayQuyetDinh").val();
    var iNamKeHoach = $("#iNamKeHoach").val();
    var iId_NguonVon = $("#iId_NguonVon").val();
    var iId_KeHoachDeXuat = $("#iId_KeHoachVonUng").val();

    if (iIdMaDonVi == null || iIdMaDonVi == "") {
        messErrors.push("Chưa chọn đơn vị.");
    }
    if (sSoQuyetDinh == null || sSoQuyetDinh == "") {
        messErrors.push("Chưa nhập số kế hoạch.");
    }
    if (dNgayQuyetDinh == null || dNgayQuyetDinh == "") {
        messErrors.push("Chưa nhập ngày lập.");
    }
    if (iNamKeHoach == null || iNamKeHoach == "") {
        messErrors.push("Chưa nhập năm kế hoạch.");
    }
    if (iId_NguonVon == null || iId_NguonVon == "") {
        messErrors.push("Chưa nhập nguồn vốn.");
    }
    if (iId_KeHoachDeXuat == null || iId_KeHoachDeXuat == "") {
        messErrors.push("Chưa nhập kế hoạch vốn ứng đề xuất.");
    }
    if (messErrors.length != 0) {
        alert(messErrors.join("\n"));
        return false;
    }
    objDuocDuyet.dataKHVU = {};
    objDuocDuyet.dataKHVU.Id = $("#id").val();
    objDuocDuyet.dataKHVU.sSoQuyetDinh = sSoQuyetDinh;
    objDuocDuyet.dataKHVU.dNgayQuyetDinh = dNgayQuyetDinh;
    objDuocDuyet.dataKHVU.iNamKeHoach = iNamKeHoach;
    objDuocDuyet.dataKHVU.iID_DonViQuanLyID = $("#iID_DonViQuanLyID").find(":selected").data("iddonvi")
    objDuocDuyet.dataKHVU.iID_MaDonViQuanLy = iIdMaDonVi;
    objDuocDuyet.dataKHVU.iID_NguonVonID = iId_NguonVon;
    objDuocDuyet.dataKHVU.iID_KeHoachVonUngDeXuatID = iId_KeHoachDeXuat;
    return true;
}

function CheckExistMLNS(sXauNoiMa) {
    var regex = /-{2,}/;
    if (regex.test(sXauNoiMa))
        return false;

    var check = true;
    $.ajax({
        url: "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/CheckExistMLNS",
        type: "GET",
        dataType: "json",
        data: { sXauNoiMa: sXauNoiMa },
        async: false,
        success: function (resp) {
            check = resp.status;
        }
    })
    return check;
}

function ConvertNumber(number) {
    if (number === undefined || number == null || number == '')
        return 0;
    else return number;
}

//export
function ExportKHVUDuocDuyet(id) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/KeHoachVonUngDuocDuyetExport",
        data: { id: id },
        success: function (data) {
            if (data.bIsComplete) {
                window.location.href = "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/ExportExcelKHVUDuocDuyet";
            }
        }
    });
}