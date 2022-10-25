var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;
var ERROR = 1;

function BangDuLieu_onKeypress_Delete(h, c) {
    
}

function BangDuLieu_XoaHang(cs) {
    if (cs != null && 0 <= cs && cs < Bang_nH) {
        Bang_arrHangDaXoa[cs] = !Bang_arrHangDaXoa[cs];
        Bang_HienThiDuLieu();
        return true;
    }

    return false;
}

function KHVonNamDD_ChiTiet_BangDuLieu_Save() {
    ValidateBeforeSave();
}

function ValidateBeforeSave() {
    var listItem = [];

    for (var i = 0; i < Bang_arrThayDoi.length; i++) {
        var object = {
            iID_DuAnID: "",
            sMaDuAn: "",
            sTenDuAn: "",
            iID_KeHoachVonNam_DuocDuyetID: "",
            iID_KeHoachVonNam_DuocDuyet_ChiTietID: "",
            iID_Parent: "",
            iLoaiDuAn: "",
            sLoaiDuAn: "",
            sGhiChu: "",
            iID_LoaiCongTrinh: "",
            LNS: "",
            L: "",
            K: "",
            M: "",
            TM: "",
            TTM: "",
            NG: "",
            fCapPhatTaiKhoBac: 0,
            fCapPhatBangLenhChi: 0,
            fGiaTriThuHoiNamTruocKhoBac: 0,
            fGiaTriThuHoiNamTruocLenhChi: 0
        };

        object.iID_DuAnID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_DuAnID"]];
        object.sMaDuAn = $("<div/>").text($.trim(Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaDuAn"]])).html();
        object.sTenDuAn = $("<div/>").text($.trim(Bang_arrGiaTri[i][Bang_arrCSMaCot["sTenDuAn"]])).html();
        object.iID_KeHoachVonNam_DuocDuyetID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_KeHoachVonNam_DuocDuyetID"]];
        object.iID_KeHoachVonNam_DuocDuyet_ChiTietID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_KeHoachVonNam_DuocDuyet_ChiTietID"]];
        object.iID_Parent = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_Parent"]];
        object.iLoaiDuAn = $("<div/>").text($.trim(Bang_arrGiaTri[i][Bang_arrCSMaCot["iLoaiDuAn"]])).html();
        object.sLoaiDuAn = $("<div/>").text($.trim(Bang_arrGiaTri[i][Bang_arrCSMaCot["sLoaiDuAn"]])).html();
        object.sGhiChu = $("<div/>").text($.trim(Bang_arrGiaTri[i][Bang_arrCSMaCot["sGhiChu"]])).html();
        object.iID_LoaiCongTrinh = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_LoaiCongTrinh"]];
        object.LNS = $("<div/>").text($.trim(Bang_arrGiaTri[i][Bang_arrCSMaCot["LNS"]])).html();
        object.L = $("<div/>").text($.trim(Bang_arrGiaTri[i][Bang_arrCSMaCot["L"]])).html();
        object.K = $("<div/>").text($.trim(Bang_arrGiaTri[i][Bang_arrCSMaCot["K"]])).html();
        object.M = $("<div/>").text($.trim(Bang_arrGiaTri[i][Bang_arrCSMaCot["M"]])).html();
        object.TM = $("<div/>").text($.trim(Bang_arrGiaTri[i][Bang_arrCSMaCot["TM"]])).html();
        object.TTM = $("<div/>").text($.trim(Bang_arrGiaTri[i][Bang_arrCSMaCot["TTM"]])).html();
        object.NG = $("<div/>").text($.trim(Bang_arrGiaTri[i][Bang_arrCSMaCot["NG"]])).html();

        object.fCapPhatTaiKhoBac = Bang_arrGiaTri[i][Bang_arrCSMaCot["fCapPhatTaiKhoBac"]];
        object.fCapPhatBangLenhChi = Bang_arrGiaTri[i][Bang_arrCSMaCot["fCapPhatBangLenhChi"]];
        object.fGiaTriThuHoiNamTruocKhoBac = Bang_arrGiaTri[i][Bang_arrCSMaCot["fGiaTriThuHoiNamTruocKhoBac"]];
        object.fGiaTriThuHoiNamTruocLenhChi = Bang_arrGiaTri[i][Bang_arrCSMaCot["fGiaTriThuHoiNamTruocLenhChi"]];

        object.fCapPhatTaiKhoBacDC = Bang_arrGiaTri[i][Bang_arrCSMaCot["fCapPhatTaiKhoBacSauDC"]];
        object.fCapPhatBangLenhChiDC = Bang_arrGiaTri[i][Bang_arrCSMaCot["fCapPhatBangLenhChiSauDC"]];
        object.fGiaTriThuHoiNamTruocKhoBacDC = Bang_arrGiaTri[i][Bang_arrCSMaCot["fGiaTriThuHoiNamTruocKhoBacSauDC"]];
        object.fGiaTriThuHoiNamTruocLenhChiDC = Bang_arrGiaTri[i][Bang_arrCSMaCot["fGiaTriThuHoiNamTruocLenhChiSauDC"]];

        listItem.push(object);
    }

    AjaxRequire(listItem);
}

function AjaxRequire(listItem) {
    var iID_KeHoachVonNamDeXuatID = $("#iIdKeHoachDeXuatId").val();
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonNamDuocDuyet/ValidateBeforeSave",
        data: { aListModel: listItem, iID_KeHoachVonNamDeXuatID: iID_KeHoachVonNamDeXuatID },
        async: false,
        success: function (data) {
            if (data.status == false) {
                var Title = "Lỗi lưu dự toán được giao !"
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: data.listErrMess, Category: 1 },
                    async: false,
                    success: function (data) {
                        window.parent.loadModal(data);
                        return false;
                    }
                });
            } else {
                sessionStorage.removeItem("IIdParent");
                Bang_HamTruocKhiKetThuc();
            }
        }
    });
}

function BangDuLieu_onKeypress_F9(h, c) {
    $(".sheet-search input.input-search").val("");
    $(".sheet-search input.input-search").filter(":visible:first").focus();
    $(".btn-mvc").click();
}

function BangDuLieu_onKeypress_F10() {
}

function Bang_onKeypress_F(strKeyEvent) {
    var h = Bang_keys.Row();
    var c = Bang_keys.Col() - Bang_nC_Fixed;
    var TenHam = Bang_ID + '_onKeypress_' + strKeyEvent;
    var fn = window[TenHam];
    if (typeof fn == 'function') {
        if (fn(h, c) == false) {
            return false;
        }
    }
}

function setFontWeight() {
    for (var j = 0; j < Bang_nH; j++) {
        var numChild = Bang_LayGiaTri(j, "numChild");
        if (numChild > 0) {
            Bang_GanGiaTriThatChoO_colName(j, "sFontBold", "bold");
            Bang_GanGiaTriThatChoO_colName(j, "sMauSac", "whitesmoke");
        }
    }
}
