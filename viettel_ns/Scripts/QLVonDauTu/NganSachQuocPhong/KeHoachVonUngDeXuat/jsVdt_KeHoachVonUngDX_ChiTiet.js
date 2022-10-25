var iIDKHVN = "";
var isUpdate = false;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var EMPTY = "";
$(document).ready(function () {
    iIDKHVN = $("#iID_KeHoachUngID").val();
    var update = $("#idUpdate").val();
    var tongHop = $("#txtIsTongHop").val();
    if (tongHop == 1) {
        HiddenLabelTongHop();
    }
    if (update == 1) {
        isUpdate = true;
    }
    GetDataKHVUDX();
    GetDonViQuanLy();
    $("#FileUpload").change(function () {
        if (document.getElementById("FileUpload").files[0].size > 10048576) {
            alert("File quá lớn!");
            document.getElementById("FileUpload").value = "";
        };
    });
});

function HiddenLabelTongHop() {
    //$(".divbutton").hide();
    //$(".btnsaveChiTiet").hide();
    $(".ctxt_sfGiaTriDeNghi").attr('disabled', true);
    $(".ctxt_sGhiChu").attr('disabled', true);
}

function GetDataChiTietTable() {
    var lstChiTiet = [];

    $.each($("#tblkhvuChiTiet tbody tr"), function (index, item) {
        var obj = {};
        // var bIsDelete = $(this).hasClass("error-row");//
        obj.iID_DuAnID = $(item).find(".c_iID_DuAnID").val();
        obj.iID_KeHoachUngID = $(item).find(".c_iID_KeHoachUngID").val();
        obj.Id = $(item).find(".c_iID_KeHoachUngChiTiet").val();
        obj.sMaDuAn = $(item).find(".c_sMaDuAn").text();
        obj.sTenDuAn = $(item).find(".c_sTenDuAn").text();
        obj.fTongMucDauTu = $(item).find(".c_fTongMucDauTu").text() != 0 ? UnFormatNumber($(item).find(".c_fTongMucDauTu").text()) : 0;
        obj.fGiaTriDeNghi = $(item).find(".ctxt_sfGiaTriDeNghi").val() != 0 ? UnFormatNumber($(item).find(".ctxt_sfGiaTriDeNghi").val()) : 0;
        obj.sGhiChu = $(item).find(".ctxt_sGhiChu").val();
        var bIsDelete = $(this).hasClass("error-row");
        obj.IsDelete = bIsDelete;
        if (obj.iID_KeHoachUngID == undefined || obj.iID_KeHoachUngID == null || obj.iID_KeHoachUngID == "" || obj.iID_KeHoachUngID == GUID_EMPTY) {
            obj.iID_KeHoachUngID = iIDKHVN;
        }
        if (!(obj.fGiaTriDeNghi == undefined || obj.fGiaTriDeNghi == null || obj.fGiaTriDeNghi == EMPTY)) {
            lstChiTiet.push(obj);
        }
    });
    return lstChiTiet;
}

function SaveKVUCT() {
    var dataChiTiet = GetDataChiTietTable();
    var Title = "";
    var Messages = [];
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonUngDeXuat/KeHoachVonUngChiTietSave",
        data: {
            listdata: dataChiTiet,
            isUpdate: isUpdate,
            iID_KeHoachUngID: iIDKHVN
        },
        success: function (r) {
            if (r.status) {
                if (!status) {
                    alert(r.desc);
                    window.location.href = "/QLVonDauTu/KeHoachVonUngDeXuat/Index";
                } else {
                    alert(r.desc);
                    window.location.href = "/QLVonDauTu/KeHoachVonUngDeXuat/Index";
                }
            }
        }
    });

}
function CancelSaveData() {
    window.location.href = "/QLVonDauTu/KeHoachVonUngDeXuat/Index";
}

function Refresh() {
    Location.Reload();
}
//function GetDataKHVUDX() {
//    $.ajax({
//        type: "GET",
//        url: "/QLVonDauTu/KeHoachTrungHanDeXuat/GetDataKHVUDXById",
//        data: {
//            iID_KeHoachUngID: iID_KeHoachUngID
//        },
//        success: function (data) {
//            if (data.status) {
//                $(".lbDonVi").val(data.iID_MaDonViQuanLy);
//                $(".lbSoKeHoach").val(data.sSoDeNghi);
//                $(".lbNgayDeNghi").val(data.dNgayDeNghi);
//                $(".lbNamKeHoach").val(data.iNamKeHoach);
//                $(".lbNguonVon").val(data.iID_NguonVonID);
//            }
//        }
//    });

//}

function GetDataKHVUDX() {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonUngDeXuat/GetDataKHVUDXById",
        data: {
            Id: iIDKHVN
        },
        dataType: "json",
        success: function (data) {
            if (data.status) {
                $(".lbDonVi").html(data.data.sTenDonVi);
                $(".lbSoKeHoach").html(data.data.sSoDeNghi);
                $(".lbNgayDeNghi").html(data.dNgayDeNghi);
                $(".lbNamKeHoach").html(data.data.iNamKeHoach);
                $(".lbNguonVon").html(data.data.sTenNguonVon);
            }
        },
        error: function (data) {
            console.log(data);
        }

    });

}

function XoaDong() {
    $.each($("#tblkhvuChiTiet tbody tr"), function (index, item) {
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
function CapNhatCotSTT() {
    var count = 0;
    $("#tblkhvuChiTiet tbody tr.parent").each(function (index, tr) {
        if (!$(tr).hasClass("error-row")) {
            $(tr).find('.r_STT').text(count + 1);
            count++;
        }
    });
}


function DeleteDetailItem() {
    var dongXoa = this.closest('tr');
    var id = $(dongXoa).data('id');
    var checkXoa = $(dongXoa).attr('data-xoa');
    if (checkXoa == 1) {
        $(dongXoa).attr('data-xoa', '0');
        $(dongXoa).removeClass('error-row');
    } else {
        $(dongXoa).attr('data-xoa', '1');
        $(dongXoa).addClass('error-row');
    }

    var objXoa = lstDuAn.filter(function (x) { return x.iID_DuAnID == id })[0];
    objXoa.isDelete = !objXoa.isDelete;
    lstDuAn = lstDuAn.filter(function (x) { return x.iID_DuAnID != id });
    lstDuAn.push(objXoa);

    if (checkXoa == 1) {
        $(dongXoa).find('input').prop("disabled", "");
    } else {
        $(dongXoa).find('input').prop("disabled", "disabled");
    }

    SumValueDetailTable();
}

function RowClick(iID_DuAnID) {

    $.each($("#tblkhvuChiTiet tbody tr"), function (index, item) {
        var obj = {};
        // var bIsDelete = $(this).hasClass("error-row");//
        if (iID_DuAnID == $(item).find(".c_iID_DuAnID").val()) {
            if ($(item).hasClass("_borderCustom")) {
                $(item).removeClass("_borderCustom");
            } else {
                $(item).addClass("_borderCustom");
            }
            //$(item).css({ "border- color": "blue", "border- style": "outset" })
        }
    });
}

function loadDataExcel() {

    if (!ValidateData()) {
        return false;
    }

    var fileInput = document.getElementById('FileUpload');
    var file = fileInput.files[0];
    var formData = new FormData();
    formData.append('file', file);
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonUngDeXuat/ReadDataToFileExcel",
        data: formData,
        contentType: false,
        processData: false,
        cache: false,
        async: false,  
        success: function (r) {
            if (r.bIsComplete) {
                DrawTableDanhSachChungTu(r.fGiaTriUng);
            } else {
                DrawTableDanhSachChungTu(0);
                //var Title = 'Lỗi lấy dữ liệu từ file excel';
                //var messErr = [];
                //messErr.push(r.sMessError);
                //$.ajax({
                //    type: "POST",
                //    url: "/Modal/OpenModal",
                //    data: { Title: Title, Messages: messErr, Category: ERROR },
                //    success: function (data) {
                //        $("#divModalConfirm").html(data);
                //    }
                //});
            }
        }
    });
}

function DrawTableDanhSachChungTu(fGiaTriUng) {
    $("#tblLstDuAnImport tbody").empty();
    var sHtml = [];
    var index = 1;
    var data = GetDataViewImport();
    if (fGiaTriUng != 0) {
        sHtml.push("<tr data-id='" + data.iID_DuAnID + "'>");
        sHtml.push("    <td>" + index + "</td>");
        sHtml.push("    <td>" + data.sSoDeNghi + "</td>");
        sHtml.push("    <td>" + data.sNgayDeNghi + "</td>");
        sHtml.push("    <td>" + data.iNamKeHoach + "</td>");
        sHtml.push("    <td>" + data.sTenNguonVon + "</td>");
        sHtml.push("    <td>" + data.sTenDonVi + "</td>");
        sHtml.push("    <td style = 'text-align: right;'>" + fGiaTriUng + "</td>");
        sHtml.push("    <td style = 'text-align: center;'><i class = 'fa-solid fa fa-check' style = 'color: #5cb85c'></td>");
        sHtml.push("</tr>");
    } else {

        sHtml.push("<tr data-id='" + data.iID_DuAnID + "'>");
        sHtml.push("    <td>" + index + "</td>");
        sHtml.push("    <td>" + data.sSoDeNghi + "</td>");
        sHtml.push("    <td>" + data.dNgayDeNghi + "</td>");
        sHtml.push("    <td>" + data.iNamKeHoach + "</td>");
        sHtml.push("    <td>" + data.sTenNguonVon + "</td>");
        sHtml.push("    <td>" + data.sTenDonVi + "</td>");
        sHtml.push("    <td style = 'text-align: right;'>" + fGiaTriUng + "</td>");
        sHtml.push("    <td style = 'text-align: center;'> <i class = 'fa-solid fa fa-times' style = 'color: red'></i> </td>");
        sHtml.push("</tr>");
    }

    $("#tblLstDuAnImport tbody").html(sHtml.join());
}

function GetDataViewImport() {
    var obj = {};
    obj.iID_DonViQuanLyID = $("#iID_DonViQuanLyID :selected").data("iiddonvi");
    obj.iID_MaDonViQuanLy = $("#iID_DonViQuanLyID").val();
    obj.sTenDonVi = $("#iID_DonViQuanLyID :selected").text();
    obj.iID_NguonVonID = $("#iId_NguonVon").val();
    obj.sTenNguonVon = $("#iId_NguonVon :selected").text();
    obj.sSoDeNghi = $("#txt_SoKeHoach").val();
    obj.dNgayDeNghi = $("#dNgayDeNghi").val();
    obj.iNamKeHoach = $("#iNamKeHoach").val();

    return obj;
}

function GetDonViQuanLy() {
    var id = $("#Id").val();
    $.ajax({
        url: "/QLVonDauTu/KeHoachVonUngDeXuat/GetDonViQuanLy",
        type: "GET",
        dataType: "json",
        cache: false,
        success: function (result) {
            if (result.status) {
                $("#iID_DonViQuanLyID").append(result.datas);
                //if (id == GUID_EMPTY && bIsTongHop == false) {
                //    $("#iID_DonViQuanLyID").change(function () {
                //        GetlstDuAnByMaDonVi();
                //        //    GetDataDropDownDuAn();
                //    });
                
                var sMaDonVi = $("#sMaDonVi").val();
                if (sMaDonVi != null && sMaDonVi != "") {
                    $("#iID_DonViQuanLyID").val(sMaDonVi);
                    $("#iID_DonViQuanLyID").change();
                }
            }
        }
    });
}

function SaveImport() {
    var data = GetDataViewImport();
    var Title = "Import kế hoạch vốn ứng đề xuất";
    var Messages = [];

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonUngDeXuat/KhvuSaveImport",
        data: {
            data: data
        },
        success: function (r) {
            if (r.bIsComplete) {
                    window.location.href = "/QLVonDauTu/KeHoachVonUngDeXuat/Index";
                
            } else {
                Messages.push(r.desc);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    async: false,  
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
                window.location.href = "/QLVonDauTu/KeHoachVonUngDeXuat/Index";

            }
        }
    });
}


function ValidateData() {
    $("#btnSave").attr('disabled', true);
    var messErrors = [];
    var objDonVi = $("#iID_DonViQuanLyID").val();
    var sSoKeHoach = $("#txt_SoKeHoach").val();
    var dNgayDeNghi = $("#dNgayDeNghi").val();
    var iNamKeHoach = $("#iNamKeHoach").val();
    var iId_NguonVon = $("#iId_NguonVon").val();

    if ((objDonVi == null || objDonVi == "")) {
        messErrors.push("Chưa chọn đơn vị.");
    }
    if (sSoKeHoach == null || sSoKeHoach == "") {
        messErrors.push("Chưa nhập số kế hoạch.");
    } else {
        if (sSoKeHoach.length > 100)
            messErrors.push("Số kế hoạch tối đa 100 ký tự.")
    }
    
    if (dNgayDeNghi == null || dNgayDeNghi == "") {
        messErrors.push("Chưa nhập ngày đề nghị.");
    }
    if (iNamKeHoach == null || iNamKeHoach == "") {
        messErrors.push("Chưa nhập năm kế hoạch.");
    }
    if (iId_NguonVon == null || iId_NguonVon == "") {
        messErrors.push("Chưa nhập nguồn vốn.");
    }
    if (messErrors.length != 0) {
        alert(messErrors.join("\n"));
        $("#btnSave").attr('disabled', false);
        return false;
    }

    CheckDupicateSoQuyetDinh()
    return true;
}

function CheckDupicateSoQuyetDinh() {
    var iID_KeHoachUngID = $("#Id").val();
    var sSoQuyetDinh = $("#txt_SoKeHoach").val();
    $.ajax({
        url: "/QLVonDauTu/KeHoachVonUngDeXuat/CheckExistSoQuyetDinh",
        type: "POST",
        data: { iID_KeHoachUngID: iID_KeHoachUngID, sSoQuyetDinh: sSoQuyetDinh },
        dataType: "json",
        async: false,
        success: function (result) {
            if (result.status) {
                alert("Đã tồn tại số đề nghị " + sSoQuyetDinh + " .");
            }
            return !result.status;
        }
    });
}

function DownloadFileImport() {
    window.location.href = "/QLVonDauTu/KeHoachVonUngDeXuat/DownloadImportExample";
}