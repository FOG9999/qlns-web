var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var TBL_DANH_SACH_DVTHDA = 'tblListDonViThucHienDuAnDd';

$(document).ready(function () {
    $("#" + TBL_DANH_SACH_DVTHDA + " .cbAll_DVTHDA").change(function () {
        if (this.checked)
            $("#" + TBL_DANH_SACH_DVTHDA + " .cb_DVTHDA").prop('checked', true).trigger("change");
        else
            $("#" + TBL_DANH_SACH_DVTHDA + " .cb_DVTHDA").prop('checked', false).trigger("change");
    })
})

backViewKH5NDD = () => {
    window.location.href = "/QLVonDauTu/KeHoachTrungHanDuocDuyet/Index/";
}

LayDanhSachChungTuDuocDuyetTheoDonViQuanLy = (iID_DonViQuanLyID, isCt = 'false') => {
    $.ajax({
        url: "/QLVonDauTu/KeHoachTrungHanDuocDuyet/LayDanhSachChungTuDuocDuyetTheoDonViQuanLy",
        type: "POST",
        data: { iID_DonViQuanLyID: iID_DonViQuanLyID, isCt: isCt },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null && data != "") {
                $("#iID_KeHoach5NamID").html(data);
            }
        },
        error: function (data) {

        }
    })
}

exportBCTongHop = (data) => {
    console.log(data);
    var isCt = $("#txtRptCt").val();
    if (isCt == "" || isCt == undefined) isCt = false;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachTrungHanDuocDuyet/ExportBCTongHop",
        data: { data: data, isCt: isCt },
        success: function (r) {
            if (r == "True") {
                console.log(1);
                window.open("/QLVonDauTu/KeHoachTrungHanDuocDuyet/ExportExcel/?isCt=" + isCt);
            }
            else {
                window.open("/QLVonDauTu/KeHoachTrungHanDuocDuyet/ViewInBaoCao/?isCt=" + isCt);
            }
        }
    });
}

exportBCTheoDonVi = (data, arrDVTHDA) => {
    console.log(data);
    console.log(arrDVTHDA);
    var isCt = $("#txtRptCt").val();
    if (isCt == "" || isCt == undefined) isCt = false;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachTrungHanDuocDuyet/ExportBCTheoDonVi",
        data: { data: data, arrDVTHDA: arrDVTHDA, isCt: isCt},
        success: function (r) {
            if (r == "True") {
                window.open("/QLVonDauTu/KeHoachTrungHanDuocDuyet/ExportExcel/?isCt=" + isCt);
            }
            else {
                window.open("/QLVonDauTu/KeHoachTrungHanDuocDuyet/ViewInBaoCao/?isCt=" + isCt);
            }
        }
    });
}