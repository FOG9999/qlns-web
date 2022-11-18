var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var TBL_DANH_SACH_NGUON_VON_DV = 'tblListDMNguonNganSachDV';
var TBL_DANH_SACH_NGUON_VON_TH = 'tblListDMNguonNganSachTH';
var TBL_DANH_SACH_DVQL = 'tblListDonViThucHienDuAn';

$(document).ready(function () {
    $("#" + TBL_DANH_SACH_NGUON_VON_DV + " .cbAll_NguonVon").change(function () {
        if (this.checked)
            $("#" + TBL_DANH_SACH_NGUON_VON_DV + " .cb_NguonVonModal").prop('checked', true).trigger("change");
        else
            $("#" + TBL_DANH_SACH_NGUON_VON_DV + " .cb_NguonVonModal").prop('checked', false).trigger("change");
    })

    $("#" + TBL_DANH_SACH_NGUON_VON_TH + " .cbAll_NguonVon").change(function () {
        if (this.checked)
            $("#" + TBL_DANH_SACH_NGUON_VON_TH + " .cb_NguonVonModal").prop('checked', true).trigger("change");
        else
            $("#" + TBL_DANH_SACH_NGUON_VON_TH + " .cb_NguonVonModal").prop('checked', false).trigger("change");
    })

    $("#" + TBL_DANH_SACH_DVQL + " .cbAll_DVQL").change(function () {
        if (this.checked)
            $("#" + TBL_DANH_SACH_DVQL + " .cb_DVQL").prop('checked', true).trigger("change");
        else
            $("#" + TBL_DANH_SACH_DVQL + " .cb_DVQL").prop('checked', false).trigger("change");
    })
});

function LayDanhSachChungTuDeXuatTheoDonViQuanLy(iID_DonViQuanLyID, iGiaiDoanTu, iGiaiDoanDen, isModified = 'false', isCt = 'false') {
    $.ajax({
        url: "/QLVonDauTu/KeHoachTrungHanDeXuat/LayDanhSachChungTuDeXuatTheoDonViQuanLy",
        type: "POST",
        data: { iID_DonViQuanLyID: iID_DonViQuanLyID, iGiaiDoanTu: iGiaiDoanTu, iGiaiDoanDen: iGiaiDoanDen, isModified: isModified, isCt: isCt},
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null && data != "") {
                $("#iID_KeHoach5Nam_DeXuatID").html(data);
            }
        },
        error: function (data) {

        }
    })
}

function backViewKH5NDX() {
    window.location.href = "/QLVonDauTu/KeHoachTrungHanDeXuat/Index/";
}

function exportBCTongHop(data, arrIdNguonVon, arrIdDVQL, isExcel) {
    console.log(data);
    console.log(arrIdNguonVon);
    var isModified = $("#txtRptModified").val();
    if (isModified == "" || isModified == undefined) isModified = false;
    var isCt = $("#txtRptCt").val();
    if (isCt == "" || isCt == undefined) isCt = false;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachTrungHanDeXuat/ExportBCTongHop",
        data: { data: data, arrIdNguonVon: arrIdNguonVon, arrIdDVQL: arrIdDVQL, isModified: isModified, isCt: isCt},
        success: function (r) {
            if (r == "True") {
                if (isExcel) {
                    window.open("/QLVonDauTu/KeHoachTrungHanDeXuat/ExportExcelFile/?isModified=" + isModified + '&isCt=' + isCt + "&isBaoCaoTheoDV=False", "_blank");
                }
                else window.open("/QLVonDauTu/KeHoachTrungHanDeXuat/ExportExcel/?isModified=" + isModified + '&isCt=' + isCt, "_blank");
            }
            else {
                window.location.href = "/QLVonDauTu/KeHoachTrungHanDeXuat/ViewInBaoCao/?isModified=" + isModified + '&isCt=' + isCt;
            }
        }
    });
}

function exportBCTheoDonVi(data, arrIdNguonVon, arrIdDVQL, isExcel) {
    console.log(data);
    console.log(arrIdNguonVon);
    console.log(arrIdDVQL);
    var isModified = $("#txtRptModified").val();
    if (isModified == "" || isModified == undefined) isModified = false;
    var isCt = $("#txtRptCt").val();
    if (isCt == "" || isCt == undefined) isCt = false;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachTrungHanDeXuat/ExportBCTheoDonVi",
        data: { data: data, arrIdNguonVon: arrIdNguonVon, arrIdDVQL: arrIdDVQL, isModified: isModified, isCt: isCt},
        success: function (r) {
            if (r == "True") {
                if (isExcel) {
                    window.open("/QLVonDauTu/KeHoachTrungHanDeXuat/ExportExcelFile/?isModified=" + isModified + '&isCt=' + isCt + "&isBaoCaoTheoDV=True", '_blank');
                }
                else window.open("/QLVonDauTu/KeHoachTrungHanDeXuat/ExportExcel/?isModified=" + isModified + '&isCt=' + isCt, '_blank');
            }
            else {
                var Title = 'Lỗi in báo cáo tổng hợp';
                var iGiaiDoanTu = $("#iGiaiDoanTu_DV").val();
                var iGiaiDoanDen = $("#iGiaiDoanDen_DV").val();
                var Messages = "Giai đoạn từ " + iGiaiDoanTu + " đến " + iGiaiDoanDen + " không có chứng từ!";

                if (Messages != null && Messages != undefined && Messages.length > 0) {
                    $.ajax({
                        type: "POST",
                        url: "/Modal/OpenModal",
                        data: { Title: Title, Messages: Messages, Category: ERROR },
                        success: function (data) {
                            $("#divModalConfirm").html(data);
                        }
                    });        
                }
            //    window.location.href = "/QLVonDauTu/KeHoachTrungHanDeXuat/ViewInBaoCao/?isModified=" + isModified + '&isCt=' + isCt;
            }
        }
    });
}