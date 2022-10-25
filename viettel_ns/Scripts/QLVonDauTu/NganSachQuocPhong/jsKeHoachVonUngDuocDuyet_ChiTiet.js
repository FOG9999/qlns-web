var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var EMPTY = "";

var objDuocDuyet = {};
var lstDuAn = [];
var lstDetail = [];
var lstMucLucNganSach = [];
var id = GUID_EMPTY;
var isUpdate = false;

var iID_NganhID = GUID_EMPTY;
var iID_KeHoachUng = GUID_EMPTY;


$(document).ready(function () {
    if ($(".isUpdate").val() == 1) {
        isUpdate = true;
    }
    iID_KeHoachUng = $(".iIdKHVonUng").val();
});

function GetDataChiTietTable() {
    var lstChiTiet = [];

    $.each($("#tblDanhSachDuAn tbody tr"), function (index, item) {
        var obj = {};
        // var bIsDelete = $(this).hasClass("error-row");//
        obj.id = $(item).find("#idChiTiet").val();
        obj.iID_DuAnID = $(item).find(".txt_sTenDuAn").data("iid_duan");
        obj.sMaDuAn = $(item).find(".txt_sMaDuAn").text();
        obj.sTenDuAn = $(item).find(".txt_sTenDuAn").text();
        obj.sLNS = $(item).find(".txt_sLNS").val();
        obj.sL = $(item).find(".txt_sL").val();
        obj.sK = $(item).find(".txt_sK").val();
        obj.sM = $(item).find(".txt_sM").val();
        obj.sTM = $(item).find(".txt_sTM").val();
        obj.sTTM = $(item).find(".txt_sTTM").val();
        obj.sNG = $(item).find(".txt_sNG").val();
        obj.fTongMucDauTu = UnFormatNumber($(item).find(".txt_sTongMucDauTu").text()) != 0 ? UnFormatNumber($(item).find(".txt_sTongMucDauTu").text()) : 0;
        obj.fGiaTriDeNghi = UnFormatNumber($(item).find(".txt_sGiaTriDeNghi").text()) != 0 ? UnFormatNumber($(item).find(".txt_sGiaTriDeNghi").text()) : 0;
        obj.fCapPhatTaiKhoBac = UnFormatNumber($(item).find(".txt_fGiaTriCapKhoBac").val()) != 0 ? UnFormatNumber($(item).find(".txt_fGiaTriCapKhoBac").val()) : 0;
        obj.fCapPhatBangLenhChi = UnFormatNumber($(item).find(".txt_fGiaTriCaplenhChi").val()) != 0 ? UnFormatNumber($(item).find(".txt_fGiaTriCaplenhChi").val()) : 0;
        obj.sGhiChu = $(item).find(".txt_sGhiChu").val();
        obj.sTrangThaiDuAnDangKy = $(item).find("#sTrangThaiDuAnDangKy").val();
        if (isUpdate) {
            obj.iID_NganhID = $(item).data("iidnganhid");

        }
        obj.iID_NganhID = iID_NganhID;
        obj.iID_KeHoachUngID = iID_KeHoachUng;
        var sXauNoiMa = obj.sLNS + "-" + obj.sL + "-" + obj.sK + "-" + obj.sM + "-" + obj.sTM + "-" + obj.sTTM + "-" + obj.sNG;
        sXauNoiMa = sXauNoiMa.replace(/[-]+$/g, '');
        obj.sXauNoiMa = sXauNoiMa;
        var bIsDelete = $(this).hasClass("error-row");
        obj.IsDelete = bIsDelete;
        //if (obj.iID_KeHoachUngID == null || obj.iID_KeHoachUngID == "" || obj.iID_KeHoachUngID == GUID_EMPTY || obj.iID_KeHoachUngID == undefined) {
        //    obj.iID_KeHoachUngID = iIDKHVN;
        //}
        //if (!(obj.fGiaTriDeNghi == undefined || obj.fGiaTriDeNghi == null || obj.fGiaTriDeNghi == EMPTY)) {
        //    lstChiTiet.push(obj);
        //}
        lstChiTiet.push(obj);

    });
    return lstChiTiet;
}


function CheckExistMLNS(sXauNoiMa) {

    var regex = /-{2,}/;
    if (regex.test(sXauNoiMa))
        return false;

    var check = false;
    $.ajax({
        url: "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/CheckExistMLNSAndGetIId",
        type: "GET",
        dataType: "json",
        data: { sXauNoiMa: sXauNoiMa },
        async: false,
        success: function (resp) {
            if (check = resp.status) {
                iID_NganhID = resp.iID_NganhID;
            }
        }
    })
    return check;
}

function GetDataModel() {
    var model = {};
    model.iID_DonViQuanLyID = $(".label_DonViql").data("iiddonviql");
    model.iID_MaDonViQuanLy = $(".label_DonViql").data("idmadonvi");
    model.sSoQuyetDinh = $(".label_sSoQUyetDinh").text();
    model.sNgayQuyetDinh = $(".label_sNgayQuyetDinh").text();
    model.iNamKeHoach = $(".label_iNamKeHoach").text();
    model.iID_NguonVonID = $(".label_NguonVon").data("iidnguonvon");
    model.iID_KeHoachVonUngDeXuatID = $(".label_KehoachDeXuat").data("iidkehoachdexuat");
    model.iID_KeHoachUngID = $(".iIdKHVonUng").val();

    return model;

}
function Save() {
    var listObjSave = [];
    var data = GetDataModel();
    if (!ValidationBeforeSave()) return;
    listObjSave = GetDataChiTietTable();
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/KeHoachVonUngDuocDuyetChiTietSave",
        data: {
            listData: listObjSave,
            isUpdate: isUpdate,
            iId_KeHoachUngID: iID_KeHoachUng
        },
        success: function (r) {
            if (r.status) {
                if (!status) {
                    alert(r.desc);
                    window.location.href = "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/Index";
                } else {
                    alert(r.desc);
                    window.location.href = "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/Index";
                }
            }
        }
    });

}

function ValidationBeforeSave() {
    var listObjChiTiet = [];
    var listObjPass = [];
    var listObjfault = [];
    listObjChiTiet = GetDataChiTietTable();
    listObjChiTiet.forEach(function (item, index) {
        if (CheckExistMLNS((item).sXauNoiMa)) {
            (item).iID_NganhID = iID_NganhID;
            listObjPass.push((item));
        }
        if (item.fGiaTriDeNghi == undefined) {
            listObjfault.push((item));
        }
    });
    if (listObjChiTiet.length != listObjPass.length) {
        alert("Mục lục ngân sách không tồn tại!");
        return false;
    }
    //if (isUpdate) {
    //    $.ajax({
    //        type: "GET",
    //        url: "/QLVonDauTu/QLKeHoachVonUngDuocDuyet/KeHoachVonUngDuocDuyetChiTietSave",
    //        data: {
    //            sXauNoiMa: sXauNoiMa
    //        },
    //        success: function (resp) {
    //            if (!resp.status) {
    //                alert(resp.ErrorMessage);
    //            }
    //        }
    //    })
    /*   }*/
    return true;

}

function Refresh() {
    Location.Reload();
}

function XoaDong() {
    $.each($("#tblDanhSachDuAn tbody tr"), function (index, item) {
        var obj = {};
        if ($(item).hasClass("_borderCustom")) {
            if ($(item).hasClass("error-row")) {
                $(item).removeClass("error-row");

            } else {
                $(item).addClass("error-row");


            }
        }
        //$(item).css({ "border- color": "blue", "border- style": "outset" })

    });
}

function RowClick(iID_DuAnID) {

    $.each($("#tblDanhSachDuAn tbody tr"), function (index, item) {
        var obj = {};
        // var bIsDelete = $(this).hasClass("error-row");//
        if (iID_DuAnID == $(item).find(".txt_sTenDuAn").data("iid_duan")) {
            if ($(item).hasClass("_borderCustom")) {
                $(item).removeClass("_borderCustom");
            } else {
                $(item).addClass("_borderCustom");
            }
            //$(item).css({ "border- color": "blue", "border- style": "outset" })
        }
    });
}