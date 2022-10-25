var GUID_EMPTY = "00000000-0000-0000-0000-000000000000";
var TBL_DANH_SACH = "tblDanhsachchuyendoidlchitiet";
var id = $("#txt_ID_TraoDoiDL").val();
var isUpdate = false;
var isDetails = $("#txtIsDetails").val();
var CONFIRM = 0;
var ERROR = 1;
if (isDetails == 1) {
    var isDetail = true;
}

$(document).ready(function () {
    $("#drpDonViQuanLy, #txtNamKeHoach, #drpQuy, #drpNguonNganSach").change(function (e) {
        GetKinhPhiCucTaiChinhCap();
        $("#" + TBL_DANH_SACH + " tbody").html("");
    });
    EventValidate();
    GetDataChiTietTable();
    if (id != undefined || id != null || id != "" || id != GUID_EMPTY) {
        isUpdate = true;
    }


    $("#iLoaiTraoDoi").on("change", function () {
        if ($("#iLoaiTraoDoi").val() == "1") {
            $("#divLoaiThongTri").hide();
            $("#divChung").show();
            $("#divLoaiDuToan").show();

        } else {
            if ($("#iLoaiTraoDoi").val() == "3") {
                $("#divLoaiDuToan").hide();
                $("#divChung").show();
                $("#divLoaiThongTri").show();

            } else {
                $("#divLoaiDuToan").hide();
                $("#divChung").hide();
                $("#divLoaiThongTri").hide();
            }

        }
    });


    if (!(id == GUID_EMPTY || id == undefined || id == "" || id == null)) {
        if (isDetail) {
            GetTraoDoiDuLieuChiTiet(id, isDetail, EventValidate);
        } else {
            GetTraoDoiDuLieuChiTiet(id, EventValidate);
            isUpdate = true;
            $(".btn_loc").hide();
        }
    } else {
        GetTraoDoiDuLieuChiTiet(EventValidate);
    }
    TinhLaiDongTong('tblDanhsachchuyendoidlchitiet');

    var isShowing = false;
    $('.date').datepicker({
        todayBtn: "linked",
        keyboardNavigation: false,
        forceParse: false,
        autoclose: true,
        language: 'vi',
        todayHighlight: true,
        format: "dd/mm/yyyy"
    }).on('hide', () => {
        isShowing = false;
    }).on('show', () => {
        isShowing = true;
    });

});

//===================== Event button ==========================//

function CancelSaveData() {
    location.href = "/QLVonDauTu/TraoDoiDuLieu";
}

function Loc() {
    //var iID_DonViQuanLyID = $("#drpDonViQuanLy option:selected").val();
    //var iNamKeHoach = $("#txtNamKeHoach").val();
    //var iQuy = $("#drpQuy option:selected").val();
    //var iIDNguonVon = $("#drpNguonNganSach option:selected").val();
    //data.ID = $("#txt_ID_TraoDoiDL").val();
    //data.iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
    //data.sSoChungTu = $("#txt_SoChungTu").val();
    //data.dNgayChungTu = $("#txt_NgayChungTu").val();
    //data.iNamLamViec = $("#txtiNamkeHoach").val();
    //data.iThoiGian = $("#iQuy").val();
    //data.iID_NguonVonID = $("#iID_NguonVon").val();
    //data.iLoaiTraoDoi = $("#iLoaiTraoDoi").val();
    //data.iLoaiDuToan = $("#iLoaiDuToan").val();
    //data.iLoaiThongtri = $("#iLoaiThongtri").val();
    //ValidateData(data);


    GetTraoDoiDuLieuChiTiet(EventValidate);
}
function ValidateData(data) {
    var Title = 'Lỗi thêm mới trao đổi dữ liệu';
    var Messages = [];

    if (data.iID_DonViQuanLyID == null || data.iID_DonViQuanLyID == "") {
        Messages.push("Đơn vị quản lý chưa chọn !");
    }

    if (data.sSoChungTu == null || data.sSoChungTu == "") {
        Messages.push("Số kế hoạch chưa nhập !");
    } else {
        if (data.sSoChungTu.trim().lenght > 50) {
            Messages.push("Số kế hoạch không được quá 50 ký tự !");
        }
    }

    if (data.dNgayChungTu == null || data.dNgayChungTu == "") {
        Messages.push("Ngày chứng từ chưa nhập !");
    }

    if (data.iNamLamViec == null || data.iNamLamViec == "") {
        Messages.push("Năm kế hoạch chưa nhập !");
    }

    if (data.iLoaiTraoDoi == null || data.iLoaiTraoDoi == "") {
        Messages.push("Loại trao đổi chưa chọn !");
    } else {
        if (data.iNamLamViec.trim().lenght > 4) {
            Messages.push("Năm kế hoạch không được quá 4 ký tự !");
        }
    }


    if (Messages != null && Messages != undefined && Messages.length > 0) {
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

// event
function EventValidate() {
    $("td.sotien[contenteditable='true']").on("keypress", function (event) {
        return ValidateNumberKeyPress(this, event);
    })
    $("td.sotien[contenteditable='true']").on("focusout", function (event) {
        $(this).html(FormatNumber($(this).html() == "" ? 0 : UnFormatNumber($(this).html())));
    })

    $("td[contenteditable='true']").on("keydown", function (e) {
        var key = e.keyCode || e.charCode;
        if (key == 13) {
            $(this).blur();
        }
    });
}

function GetTraoDoiDuLieuChiTiet(iID_NhuCauChiID, isDetail, callback) {
    $.ajax({
        type: "Get",
        url: "/QLVonDauTu/TraoDoiDuLieu/GetdataTraoDoiDuLieuChiTiet",
        data: { id: id, isDetail: isDetail },
        success: function (data) {
            $("#" + TBL_DANH_SACH + " tbody").html(data);
            if (callback)
                callback();
        }
    });
    GetDataChiTietTable();
    TinhLaiDongTong('tblDanhsachchuyendoidlchitiet');
}

function Insert() {
    if (!ValidateDataInsert()) return;
    LuuTraoDoiDuLieu();
}

function LuuTraoDoiDuLieu() {
    var data = GetDataModel();

    var dataChiTiet = GetDataChiTietTable();
    var Title = "";
    var Messages = [];

    $.ajax({
        type: "POST",
        url: "/TraoDoiDuLieu/TraoDoiDuLieuSave",
        data: {
            data: data,
            isUpdate: isUpdate,
            lstDataChiTiet: dataChiTiet
        },
        success: function (r) {
            if (r.bIsComplete) {
                if (!isUpdate) {
                    alert("Thêm mới dữ liệu thành công !");
                    window.location.href = "/QLVonDauTu/TraoDoiDuLieu/Index";
                } else {
                    alert("cập nhật dữ liệu thành công !");
                    window.location.href = "/QLVonDauTu/TraoDoiDuLieu/Index";
                }

            } else {
                if (!isUpdate) {
                  
                    if (r.Messages != null || r.Messages != undefined || r.Messages != GUID_EMPTY || r.Messages != "") {
                        Messages.push(r.sMessError);
                        alert(Messages);
                    } else {
                        alert("Thêm mới trao đổi dữ liệu thất bại");
                        window.location.href = "/QLVonDauTu/TraoDoiDuLieu/Index";
                    }                 
                } else {
                    if (r.Messages != null || r.Messages != undefined || r.Messages != GUID_EMPTY || r.Messages != "") {
                        Messages.push(r.sMessError);
                        alert(Messages);
                    } else {
                        alert("cập nhật trao đổi dữ liệu thất bại");
                        window.location.href = "/QLVonDauTu/TraoDoiDuLieu/Index";

                    }                 
                }
            }
        }
    });
}


//=========================== validate ===========//
function ValidateDataInsert() {
    var Messages = [];
    var isAdd = true;
    var data = {};
    data.ID = $("#txt_ID_TraoDoiDL").val();
    data.iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
    data.sSoChungTu = $("#txt_SoChungTu").val();
    data.dNgayChungTu = $("#txt_NgayChungTu").val();
    data.iNamLamViec = $("#txtiNamkeHoach").val();
    data.iThoiGian = $("#iQuy").val();
    data.iID_NguonVonID = $("#iID_NguonVon").val();
    data.iLoaiTraoDoi = $("#iLoaiTraoDoi").val();
    data.iLoaiDuToan = $("#iLoaiDuToan").val();
    data.iLoaiThongtri = $("#iLoaiThongtri").val();
    if (data.iID_DonViQuanLyID == null || data.iID_DonViQuanLyID == "") {
        Messages.push("Đơn vị quản lý chưa chọn !");
    }

    if (data.sSoChungTu == null || data.sSoChungTu == "") {
        Messages.push("Số kế hoạch chưa nhập !");
    } else {
        if (data.sSoChungTu.lenght > 50) {
            Messages.push("Số kế hoạch không được quá 50 ký tự !");
        }
    }

    if (data.dNgayChungTu == null || data.dNgayChungTu == "") {
        Messages.push("Ngày chứng từ chưa nhập !");
    }

    if (data.iNamLamViec == null || data.iNamLamViec == "") {
        Messages.push("Năm kế hoạch chưa nhập !");
    }

    if (data.iLoaiTraoDoi == null || data.iLoaiTraoDoi == "") {
        Messages.push("Loại trao đổi chưa chọn !");
    }

    if (Messages.length != 0) {
        alert(Messages.join('\n'));
        return false;
    }
    //} else {
    //    $.ajax({
    //        type: "POST",
    //        url: "/QLKeHoachChiQuy/ValidattionKHChiQuy",
    //        async: false,
    //        data: {
    //            iNamKeHoach: iNamKeHoach, iIdDonVi: iID_DonViQuanLyID,
    //            iIdNguonVon: iIDNguonVon, iQuy: iQuy
    //        },
    //        success: function (r) {
    //            if (r == "False") {
    //                sMessError.push(`Đơn vị ${nameDonViQL} đã có kế hoạch chi quý năm ${iNamKeHoach}!`);
    //                alert(sMessError.join('\n'));
    //                isAdd = false;
    //            }
    //        }
    //    });
    //}
    //if (!isAdd) {
    //    return false;
    //}
    return true;

}


function Xoa() {

    //var today = new Date();
    //var dd = String(today.getDate()).padStart(2, '0');
    //var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    //var yyyy = today.getFullYear();

    //today = mm + '/' + dd + '/' + yyyy;
    //$("#iID_DonViQuanLyID").prop("selectedIndex", 1);
    //$("#txt_SoChungTu").val("");
    //$("#txt_NgayChungTu").val(today);
    //$("#txtiNamkeHoach").val("");
    //$("#iID_NguonVon").prop("selectedIndex", 1);
    //$("#iLoaiThongtri").prop("selectedIndex", 1);
    //$("#iLoaiDuToan").prop("selectedIndex", 1);
    //$("#iThoiGian").prop("selectedIndex", 1);;
    //$("#iLoaiTraoDoi").prop("selectedIndex", 1);
    location.reload();

}

function GetDataModel() {
    var obj = {};
    obj.ID = $("#txt_ID_TraoDoiDL").val();
    obj.sSoChungTu = $("#txt_SoChungTu").val();
    obj.dNgayChungTu = $("#txt_NgayChungTu").val();
    obj.iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
    obj.iID_NguonVonID = $("#iID_NguonVon").val();
    obj.iNamLamViec = $("#txtiNamkeHoach").val();
    obj.iThoiGian = $("#iThoiGian").val();
    obj.sMoTa = $("#txt_ID_TraoDoiDL").val();
    obj.iID_MaDonVi = $("#txt_ID_TraoDoiDL").val();
    obj.iLoaiTraoDoi = $("#iLoaiTraoDoi").val();
    obj.iLoaiThongtri = $("#iLoaiThongtri").val();
    obj.iLoaiDuToan = $("#iLoaiDuToan").val();
    obj.fSoTien = UnFormatNumber($(".c_fTongSotien").text());
    return obj;
}

function TinhLaiDongTong(idBang) {
    var result = 0;
    var lstChiTiet = GetDataChiTietTable();
    if (arrHasValue(lstChiTiet)) {
        lstChiTiet.forEach(x => {
            if (!isStringEmpty(x.fSoTien)) {

                result += parseFloat(UnFormatNumber(x.fSoTien));
                $(".c_fTongSotien").val(FormatNumber(result));
            }
        });
    }
    //var fTien = formatNumber(result);
    $("#" + idBang + " .c_fTongSotien").html(FormatNumber(result));
}

function GetDataChiTietTable() {
    var lstChiTiet = [];

    $.each($("#tblDanhsachchuyendoidlchitiet tbody tr"), function (index, item) {
        var obj = {};
        // var bIsDelete = $(this).hasClass("error-row");//
        obj.iID_TraodoiDuLieuChiTietID = $(item).find(".c_iID_TraodoiDuLieuChiTietID").val();
        obj.iID_MaMucLucNganSach = $(item).find(".c_iID_iID_MaMucLucNganSach").val();
        obj.iID_MaMucLucNganSach_Cha = $(item).find(".c_iID_MaMucLucNganSach_Cha").val();
        obj.bLaHangCha = $(item).find(".c_bLaHangCha").val();
        obj.sXauNoiMa = $(item).find(".c_sXauNoiMa").val();
        obj.sLNS = $(item).find(".c_sLNS").text();
        obj.sL = $(item).find(".c_sL").text();
        obj.sK = $(item).find(".c_sK").text();
        obj.sM = $(item).find(".c_sM").text();
        obj.sTM = $(item).find(".c_sTM").text();
        obj.sTTM = $(item).find(".c_sTTM").text();
        obj.sNG = $(item).find(".c_sNG").text();
        obj.sTNG = $(item).find(".c_sTNG").text();
        obj.fSoTien = UnFormatNumber($(item).find(".ctxt_sfSotien").val());
        if (!(obj.fSoTien == null || obj.fSoTien == undefined || obj.fSoTien == "")) {
            lstChiTiet.push(obj);
        }

    });
    return lstChiTiet;
}
function arrHasValue(x) {
    if (x == null || x == undefined || x.length <= 0) {
        return false;
    }

    return true;
}

////Hàm chuyển từ số có định dạng về dạng không định dạng
//function UnFormatNumber(value) {
//    //value = value.toString();
//    if (value == "") return value;
//    if (true) {
//        value = value.replace(/\./gi, "")//hàm đã sửa
//        value = value.replace(/\,/gi, ".")//hàm đã sửa
//    }
//    else {
//        value = value.replace(/\,/gi, "")//hàm đã sửa
//    }
//    //obj.value = obj.value.replace(/,/gi, ""); //hàm ban đầu
//    return value;
//}



//function formatnumber(n) {
//    // format number 1000000 to 1,234,567
//    return n.tostring().replace(/\d/g, "").replace(/\b(?=(\d{3})+(?!\d))/g, ".")
//}


function isStringEmpty(value) {
    if (value == null || value == undefined || value == "") {
        return true;
    }
    return false;
}
