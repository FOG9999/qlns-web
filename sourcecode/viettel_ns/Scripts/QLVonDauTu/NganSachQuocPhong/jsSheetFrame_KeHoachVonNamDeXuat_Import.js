var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;
var ERROR = 1;
var listItemAfter = [];
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';


//function KH5NamDX_ChiTiet_BangDuLieu_Save() {
//    Bang_HamTruocKhiKetThuc();
//}


function BangDuLieu_onKeypress_F10() {
    //DMNoiDungChi_BangDuLieu_Save();
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
    //if (strKeyEvent == "F10") {
    //    //Bang_HamTruocKhiKetThuc();
    //}
}

function setFontWeight() {
    for (var j = 0; j < Bang_nH; j++) {
        var bIsParent = Bang_LayGiaTri(j, "bIsParent");
        if (bIsParent == 1) {
            Bang_GanGiaTriThatChoO_colName(j, "sFontBold", "bold");
            Bang_GanGiaTriThatChoO_colName(j, "sMauSac", "whitesmoke");
        }
    }
}

//function KH5NamDX_Import_BangDuLieu_Save() {
//    var data = {};
//    window.parent.getInfoKH5NDX(data);

//    if (!ValidateData(data)) {
//        return false;
//    }

//    $.ajax({
//        type: "POST",
//        url: "/QLVonDauTu/KeHoachTrungHanDeXuat/KH5NDXPreSaveImport",
//        data: { data: data },
//        success: function (r) {
//            if (r.bIsComplete) {
//                Bang_HamTruocKhiKetThuc();
//            } else {
//                var Title = 'Lỗi import kế hoạch trung hạn đề xuất';
//                var messErr = [];
//                messErr.push(r.sMessError);
//                $.ajax({
//                    type: "POST",
//                    url: "/Modal/OpenModal",
//                    data: { Title: Title, Messages: messErr, Category: ERROR },
//                    success: function (data) {
//                        window.parent.loadModal(data);
//                        return false;
//                    }
//                });
//            }
//        }
//    });
//}

//function ValidateData(data) {
//    var Title = 'Lỗi import kế hoạch trung hạn đề xuất';
//    var Messages = [];

//    if (data.iID_DonViQuanLyID == null || data.iID_DonViQuanLyID == "") {
//        Messages.push("Đơn vị quản lý chưa chọn !");
//    }

//    if (data.sSoQuyetDinh == null || data.sSoQuyetDinh == "") {
//        Messages.push("Số kế hoạch chưa nhập !");
//    }

//    if (data.iGiaiDoanTu == null || data.iGiaiDoanTu == "") {
//        Messages.push("Giai đoạn từ chưa nhập !");
//    }

//    if (data.iGiaiDoanDen == null || data.iGiaiDoanDen == "") {
//        Messages.push("Giai đoạn đến chưa nhập !");
//    }

//    var rowCount = Bang_nH;
//    if (rowCount == 0) {
//        Messages.push("Chưa có dữ liệu chi tiết để import !");
//    }

//    if (Messages != null && Messages != undefined && Messages.length > 0) {
//        $.ajax({
//            type: "POST",
//            url: "/Modal/OpenModal",
//            data: { Title: Title, Messages: Messages, Category: ERROR },
//            success: function (data) {
//                window.parent.loadModal(data);
//            }
//        });
//        return false;
//    }

//    return true;
//}

function SaveDataDetailImport(iID_KeHoachVonNamDeXuatID) {
    if (iID_KeHoachVonNamDeXuatID == undefined || iID_KeHoachVonNamDeXuatID == null || iID_KeHoachVonNamDeXuatID == "")
        iID_KeHoachVonNamDeXuatID = GUID_EMPTY;
    if (!ValidateBeforeSave()) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonNamDeXuat/SaveChiTietimport",
        data: {
            listDetails: listItemAfter,
            iID_KeHoachVonNamDeXuatID: iID_KeHoachVonNamDeXuatID
        },
        dataType: "json",
        cache: false,
        success: function (r) {
            if (r.bIsComplete) {
                if (window.location !== window.top.location) {
                    window.top.location.href = "/QLVonDauTu/KeHoachVonNamDeXuat/Index";

                   
                }            
            } else {
                var Title = 'Lỗi import kế hoạch vốn năm đề xuất';
                var messErr = [];
                messErr.push(r.sMessage);
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

function ValidateBeforeSave() {

    var Title = 'Lỗi import kế hoạch vốn năm đè xuất';
    var Messages = [];
    var listItemDeleted = [];
    var count = 0;
    for (var i = 0; i < Bang_arrGiaTri.length; i++) {
        var object = {
            iID_KeHoachVonNamDeXuatChiTietID: "",
            iID_KeHoachVonNamDeXuatID: "",
            iID_DuAnID: "",
            sMaDuAn: "",
            fTongMucDauTuDuocDuyet: 0,
            fLuyKeVonNamTruoc: 0,
            fKeHoachVonDuocDuyetNamNay: 0,
            fVonKeoDaiCacNamTruoc: 0,
            fUocThucHien: 0,
            fThuHoiVonUngTruoc: 0,
            fThanhToan: 0,
            iID_DonViTienTeID: "",
            iID_TienTeID: "",
            fTiGiaDonVi: 0,
            fTiGia: 0,
            sTrangThaiDuAnDangKy: "",
            iLoaiDuAn: 0,
            iID_LoaiCongTrinh: "",
            iID_DonViID: ""

        };

        object.iID_KeHoachVonNamDeXuatChiTietID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_KeHoachVonNamDeXuatChiTietID"]];
        object.iID_KeHoachVonNamDeXuatID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_KeHoachVonNamDeXuatID"]];
        object.iID_DuAnID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_DuAnID"]];
        object.sMaDuAn = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaDuAn"]];
        object.fTongMucDauTuDuocDuyet = Bang_arrGiaTri[i][Bang_arrCSMaCot["fTongMucDauTuDuocDuyet"]];
        object.fLuyKeVonNamTruoc = Bang_arrGiaTri[i][Bang_arrCSMaCot["fLuyKeVonNamTruoc"]];
        object.fKeHoachVonDuocDuyetNamNay = Bang_arrGiaTri[i][Bang_arrCSMaCot["fKeHoachVonDuocDuyetNamNay"]];
        object.fVonKeoDaiCacNamTruoc = Bang_arrGiaTri[i][Bang_arrCSMaCot["fVonKeoDaiCacNamTruoc"]];
        object.fUocThucHien = Bang_arrGiaTri[i][Bang_arrCSMaCot["fUocThucHien"]];
        object.fThuHoiVonUngTruoc = Bang_arrGiaTri[i][Bang_arrCSMaCot["fThuHoiVonUngTruoc"]];
        object.fThanhToan = Bang_arrGiaTri[i][Bang_arrCSMaCot["fThanhToan"]];
        object.iID_LoaiCongTrinh = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_LoaiCongTrinh"]];
        object.iID_DonViID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_DonViQuanLyID"]];
        if (!Bang_arrHangDaXoa[i]) {
            listItemAfter.push(object);
        }
        else listItemDeleted.push(object);
        if (object.fThanhToan == undefined || object.fThanhToan == null || object.fThanhToan == 0 || object.fThanhToan == "") {
            count++;
        }
    }
    if (count > 0) {
        Messages.push("Cột thanh toán không được để trống !");
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