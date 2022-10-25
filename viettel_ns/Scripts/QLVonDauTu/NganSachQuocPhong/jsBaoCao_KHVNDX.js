var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var TBL_DANH_SACH_NGUON_VON_DV = 'tblListDMNguonNganSachDV';
var TBL_DANH_SACH_NGUON_VON_TH = 'tblListDMNguonNganSachTH';
var TBL_DANH_SACH_DVQL = 'tblListDonViQuanLy';

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

function backViewKHVNDX() {
    window.location.href = "/QLVonDauTu/KeHoachVonNamDeXuat";
}

function LayDanhSachChungTuDeXuatTheoDonViQuanLy(iID_DonViQuanLyID) {
    var isStatus = $("#txtStatusReport").val();
    $.ajax({
        url: "/QLVonDauTu/KeHoachVonNamDeXuat/LayDanhSachChungTuDeXuatTheoDonViQuanLy",
        type: "POST",
        data: { iID_DonViQuanLyID: iID_DonViQuanLyID, isStatus: isStatus },
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

function exportBCTheoDonVi(data, arrIdNguonVon, arrDonVi) {
    console.log(data);
    console.log(arrIdNguonVon);
    console.log(arrDonVi);
    var isStatus = $("#txtStatusReport").val();

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonNamDeXuat/ExportBCTheoDonVi",
        data: { data: data, arrIdNguonVon: arrIdNguonVon, arrDonVi: arrDonVi, isStatus: isStatus},
        success: function (r) {
            if (r) {
                console.log(1);
       /*         window.location.href = "/QLVonDauTu/KeHoachVonNamDeXuat/ExportExcel/?isStatus=" + isStatus;*/
                window.open("/QLVonDauTu/KeHoachVonNamDeXuat/ExportExcel/?isStatus=" + isStatus, '_blank');
            }
        }
    });
}

function exportBCTongHop(data, arrIdNguonVon, arrIdDVQL) {
    console.log(data);
    console.log(arrIdNguonVon);
    console.log(arrIdDVQL);
    var isStatus = $("#txtStatusReport").val();
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonNamDeXuat/ExportBCTongHop",
        data: { data: data, arrIdNguonVon: arrIdNguonVon, arrIdDVQL: arrIdDVQL, isStatus: isStatus},
        success: function (r) {
            if (r) {
                console.log(2);
                //window.location.href = "/QLVonDauTu/KeHoachVonNamDeXuat/ExportExcel/?isStatus=" + isStatus;
                window.open("/QLVonDauTu/KeHoachVonNamDeXuat/ExportExcel/?isStatus=" + isStatus, '_blank');
            }
        }
    });
}