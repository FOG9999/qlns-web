var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;
var ERROR = 1;


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

function KH5NamDX_Import_BangDuLieu_Save() {
    var data = {};
    window.parent.getInfoKH5NDX(data);

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachTrungHanDeXuat/KH5NDXPreSaveImport",
        data: { data: data },
        success: function (r) {
            if (r.bIsComplete) {
                Bang_HamTruocKhiKetThuc();
            } else {
                var Title = 'Lỗi import kế hoạch trung hạn đề xuất';
                var messErr = [];
                messErr.push(r.sMessError);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (data) {
                        window.parent.loadModal(data);
                        return false;
                    }
                });
            }
        }
    });
}

function ValidateData(data) {
    var Title = 'Lỗi import kế hoạch trung hạn đề xuất';
    var Messages = [];

    if (data.iID_DonViQuanLyID == null || data.iID_DonViQuanLyID == "") {
        Messages.push("Đơn vị quản lý chưa chọn !");
    }

    if (data.sSoQuyetDinh == null || data.sSoQuyetDinh == "") {
        Messages.push("Số kế hoạch chưa nhập !");
    }

    if (data.iGiaiDoanTu == null || data.iGiaiDoanTu == "") {
        Messages.push("Giai đoạn từ chưa nhập !");
    }

    if (data.iGiaiDoanDen == null || data.iGiaiDoanDen == "") {
        Messages.push("Giai đoạn đến chưa nhập !");
    }

    if (data.iGiaiDoanTu % 5 != 1) {
        Messages.push("Giai đoạn không hợp lệ.");
    }

    var rowCount = Bang_nH;
    if (rowCount == 0) {
        Messages.push("Chưa có dữ liệu chi tiết để import !");
    }

    if (Messages != null && Messages != undefined && Messages.length > 0) {
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: Messages, Category: ERROR },
            success: function (data) {
                window.parent.loadModal(data);
            }
        });
        return false;
    }

    return true;
}