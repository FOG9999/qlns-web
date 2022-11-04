var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var isInsert = true;
var isDelete = false;
var isTongHop = false;
var isDieuChinh = false;


//================= Index =============================//
function ChangeChungTu() {
    SearchData(1);
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

function SearchData(iCurrentPage = 1) {
    var tabIndex = $('input[name=groupChungTuTongHop]:checked').val();
    var sSoQuyetDinh = $("#sSoQuyetDinh").val();
    var dNgayQuyetDinhFrom = $("#dNgayQuyetDinhFrom").val();
    var dNgayQuyetDinhTo = $("#dNgayQuyetDinhTo").val();
    var iID_MaDonViQuanLyID = $("#iID_DonViQuanLyID").val();

    var iNamKeHoach = $("#iNamKeHoach").val();
    var iIdNguonVon = $("#iId_NguonVon").val();
    GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iID_MaDonViQuanLyID, iNamKeHoach, iIdNguonVon, iCurrentPage, tabIndex);
}

function GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iID_MaDonViQuanLyID, iNamKeHoach, iIdNguonVon, iCurrentPage, tabIndex) {
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
            tabIndex: tabIndex,
            _paging: _paging
        },
        success: function (data) {
            $("#lstDataView").html(data);

            $("#sSoQuyetDinh").val(sSoQuyetDinh);
            $("#dNgayQuyetDinhFrom").val(dNgayQuyetDinhFrom);
            $("#dNgayQuyetDinhTo").val(dNgayQuyetDinhTo);
            $("#iNamKeHoach").val(iNamKeHoach);
            $("#iId_NguonVon").val(iIdNguonVon);
            $("#iID_DonViQuanLyID").val(iID_MaDonViQuanLyID);
        }
    });
}

function GetItemKHVU(id) {
    location.href = "/QLVonDauTu/KeHoachVonUngDeXuat/Update/" + id;
}

function DeleteItemKHVU(id, sSoQuyetDinh) {
    if (confirm("Bạn có chắc chắn muốn xóa kế hoạch vốn ứng đề xuất " + sSoQuyetDinh + " ?")) {
        $.ajax({
            url: "/QLVonDauTu/KeHoachVonUngDeXuat/Delete/",
            type: "POST",
            data: { id: id },
            success: function (result) {
                ChangeChungTu();
            }
        })
    }
}

function LockItem(id, sSoQuyetDinh, iKhoa) {
    var Title = 'Xác nhận ' + (iKhoa ? 'mở' : 'khóa') + ' kế hoạch vốn ứng đề xuất';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn ' + (iKhoa ? 'mở' : 'khóa') + ' chứng từ ' + sSoQuyetDinh + '?');
    var FunctionName = "Lock('" + id + "')";
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
        success: function (data) {
            $("#divModalConfirm").html(data);
        }
    });
}

function Lock(id) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonUngDeXuat/KeHoachVonUngDeXuatLock",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                SearchData();
            }
        }
    });
}

//================ Sumary ==============================//
function onSummary() {
    var strCheck = "";
    var lstId = [];
    var isDuplicate = true;
    var isLock = 1;
    var iNamKeHoach = 0;
    var iIdNguonVon = 0;
    $.each($("#lstDataView .ck_TongHop:checkbox:checked"), function (index, child) {
        iNamKeHoach = $(child).closest("tr").data("inamkehoach");
        iIdNguonVon = $(child).closest("tr").data("iidnguonvon");
        var isLockHienTai = $(child).closest("tr").data("lock");

        if (isLockHienTai == 0)
            isLock = isLockHienTai;

        var sKey = iNamKeHoach + "-" + iIdNguonVon;
        if (strCheck == "") {
            strCheck = sKey;
        }
        else if (strCheck != sKey) {
            isDuplicate = false;
            return false;
        }
        lstId.push($(child).closest("tr").data("id"));
    });
    if (lstId.length == 0) {
        alert("Vui lòng chọn bản ghi.");
        return;
    }
    if (isLock == 0) {
        alert("Vui lòng chọn bản ghi đã khóa.");
        return;
    }
    if (!isDuplicate) {
        alert("Chứng từ tổng hợp phải cùng năm kế hoạch và nguồn vốn.");
        return;
    }
    $("#btnShowModalTongHop").click();
}


//TongHop

function OpenModal(id, isTonghop) {

    var strCheck = "";
    var lstId = [];
    var isDuplicate = true;
    var isLock = 1;
    var iNamKeHoach = 0;
    var iIdNguonVon = 0;
    if (isTonghop) {
        $("#txtIstongHop").val(1);
    }
    $.each($("#lstDataView .ck_TongHop:checkbox:checked"), function (index, child) {
        iNamKeHoach = $(child).closest("tr").data("inamkehoach");
        iIdNguonVon = $(child).closest("tr").data("iidnguonvon");
        var isLockHienTai = $(child).closest("tr").data("lock");

        if (isLockHienTai == 0)
            isLock = isLockHienTai;

        var sKey = iNamKeHoach + "-" + iIdNguonVon;
        if (strCheck == "") {
            strCheck = sKey;
        }
        else if (strCheck != sKey) {
            isDuplicate = false;
            return false;
        }
        lstId.push($(child).closest("tr").data("id"));
    });
    if (lstId.length == 0) {
        alert("Vui lòng chọn bản ghi.");
        return;
    }
    if (isLock == 0) {
        alert("Vui lòng chọn bản ghi đã khóa.");
        return;
    }
    if (!isDuplicate) {
        alert("Chứng từ tổng hợp phải cùng năm kế hoạch và nguồn vốn.");
        return;
    }
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/KeHoachVonUngDeXuat/GetModel",
        data: {
            id: id,
            listKHVids: lstId,
            isTonghop: isTonghop
        },
        success: function (data) {
            $("#contentModalKHVonUngNamDeXuat").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalKHVonUngNamDeXuatLabel").html('Tổng hợp kế hoạch vốn ứng đề xuất');
            }
            else {

                $("#modalKHVonUngNamDeXuatLabel").html('Sửa kế hoạch vốn năm đề xuất');
            }
            $(".date").datepicker({
                todayBtn: "linked",
                language: "vi",
                autoclose: true,
                todayHighlight: true,
                format: 'dd/mm/yyyy'
            });

        }
    });
}

//================= Insert =============================//
function GetDataDetail() {
    var id = $("#Id").val();
    if (!(id == GUID_EMPTY || id == "" || id == undefined || id == null)) {
        isInsert = false;
    }

    var sMaDonVi = $("#iID_DonViQuanLyID :selected").val();
    if (isDieuChinh) {
        $("#iID_DonViQuanLyID").attr('disabled', true);
        $("#iNamKeHoach").attr('disabled', true);
        $("#iId_NguonVon").attr('disabled', true);

    } else {
        if (id != guidEmpty) {
            if (!bIsTongHop)
                $("#iID_DonViQuanLyID").attr('disabled', true);
            $("#iNamKeHoach").attr('disabled', true);
            $("#iId_NguonVon").attr('disabled', true);
            $("#dNgayDeNghi").attr('disabled', true);
            $(".create_thdt span.input-group-addon").hide();
        } else {
            if (!bIsTongHop)
                $("#iID_DonViQuanLyID").attr('disabled', false);
            $("#iNamKeHoach").attr('disabled', false);
            $("#iId_NguonVon").attr('disabled', false);
            $("#dNgayDeNghi").attr('disabled', false);
        }
    }

    $.ajax({
        url: "/QLVonDauTu/KeHoachVonUngDeXuat/GetKeHoachVonUngChiTiet",
        type: "POST",
        data: {
            id: id,
            bIsTongHop: bIsTongHop,
            sMaDonVi: sMaDonVi
        },
        dataType: "json",
        cache: false,
        success: function (result) {
            if (result.status) {
                lstDuAn = [];

                result.lstDetail.forEach(x => {
                    var newItem = {
                        iID_DuAnID: x.iID_DuAnID,
                        sMaDuAn: x.sMaDuAn,
                        sTenDuAn: x.sTenDuAn,
                        /*                      sGiaTriDeNghi = x.sGiaTriDeNghi,*/
                        fTongMucDauTu: x.fTongMucDauTu,
                        sTongMucDauTu: Number(x.fTongMucDauTu).toLocaleString('vi-VN'),
                        Id: x.Id,
                        isDelete: x.isDelete
                        //sGhiChu = x.sGhiChu,
                        //sTrangThaiDuAnDangKy = x.sTrangThaiDuAnDangKy

                    };

                    lstDuAn.push(newItem);
                });

                DrawTableDuAnChiTiet();
                SumValueDetailTable();
            }
        }
    });
}

function GetDonViQuanLy() {
    var id = $("#Id").val();
    $.ajax({
        url: "/QLVonDauTu/KeHoachVonUngDeXuat/GetDonViQuanLy",
        type: "GET",
        dataType: "json",
        cache: false,
        async: false,
        success: function (result) {
            if (result.status) {
                $("#iID_DonViQuanLyID").append(result.datas);
                if (id == GUID_EMPTY && bIsTongHop == false) {
                    var sMaDonVi = $("#iID_DonViQuanLyID").val();
                    $("#iID_DonViQuanLyID").change(function () {
                        GetlstDuAnByMaDonVi();
                        //    GetDataDropDownDuAn();
                    });
                }

                var sMaDonVi = $("#sMaDonVi").val();
                if (sMaDonVi != null && sMaDonVi != "") {
                    $("#iID_DonViQuanLyID").val(sMaDonVi);
                    $("#iID_DonViQuanLyID").change();
                }
            }
        }
    });
    GetlstDuAnByMaDonVi();
}

var lstDuAn = [];
function GetDataDropDownDuAn() {
    lstDuAn = [];
    var sMaDonVi = $("#iID_DonViQuanLyID").val();
    var dNgayDeNghi = $("#dNgayDeNghi").val();
    var sTongHop = $("#sTongHop").val();

    $.ajax({
        url: "/QLVonDauTu/KeHoachVonUngDeXuat/GetDuAnByCondition",
        type: "POST",
        data: { sMaDonVi: sMaDonVi, dNgayDeNghi: dNgayDeNghi, sTongHop: sTongHop },
        dataType: "json",
        cache: false,
        success: function (result) {
            $("#iID_DuAnID").empty();
            if (result.status) {
                result.lstDuAn.forEach(x => {
                    var newItem = {
                        iID_DuAnID: x.iID_DuAnID,
                        sMaDuAn: x.sMaDuAn,
                        sTenDuAn: x.sTenDuAn,
                        fGiaTriDeNghi: x.fGiaTriDeNghi,
                        sGiaTriDeNghi: x.sGiaTriDeNghi,
                        fTongMucDauTu: x.fTongMucDauTu,
                        sTongMucDauTu: x.sTongMucDauTu,
                        sTrangThaiDuAnDangKy: x.sTrangThaiDuAnDangKy,
                        sGhiChu: x.sGhiChu,
                        Id: x.Id,

                    };
                    lstDuAn.push(newItem);
                })

                DrawTableDuAn();
                SumValueDetailTable();
            }
        }
    });
};

var lstDuAn = [];
function GetlstDuAnByMaDonVi() {
    lstDuAn = [];
    var sMaDonVi = $("#iID_DonViQuanLyID").val();

    $.ajax({
        url: "/QLVonDauTu/KeHoachVonUngDeXuat/GetDuAnByMaDonVi",
        type: "POST",
        data: { sMaDonVi: sMaDonVi },
        dataType: "json",
        cache: false,
        success: function (result) {
            $("#iID_DuAnID").empty();
            if (result.status) {
                result.lstDuAn.forEach(x => {
                    var newItem = {
                        iID_DuAnID: x.iID_DuAnID,
                        sMaDuAn: x.sMaDuAn,
                        sTenDuAn: x.sTenDuAn,
                        fGiaTriDeNghi: x.fGiaTriDeNghi,
                        sGiaTriDeNghi: x.sGiaTriDeNghi,
                        fTongMucDauTu: x.fTongMucDauTu,
                        sTongMucDauTu: x.sTongMucDauTu,
                        sTrangThaiDuAnDangKy: x.sTrangThaiDuAnDangKy,
                        sGhiChu: x.sGhiChu,
                        Id: x.Id,
                    };
                    lstDuAn.push(newItem);
                })

                DrawTableDuAn();
                SumValueDetailTable();
            }
        }
    });
};

//var lstDuAn = [];
//function GetlstDuAnByMaDonVi() {
//    var lstDuAn = [];
//    var sMaDonVi = $("#iID_DonViQuanLyID").val();
//    $.ajax({
//        url: "/QLVonDauTu/KeHoachVonUngDeXuat/GetDuAnByMaDonVi",
//        type: "POST",
//        data: { sMaDonVi: sMaDonVi },
//        dataType: "json",
//        cache: false,
//        success: function (result) {
//            $("#iID_DuAnID").empty();
//            if (result.status) {
//                result.lstDuAn.forEach(x => {
//                    var newItem = {
//                        iID_DuAnID: x.iID_DuAnID,
//                        sMaDuAn: x.sMaDuAn,
//                        sTenDuAn: x.sTenDuAn,
//                        fGiaTriDeNghi: x.fGiaTriDeNghi,
//                        sGiaTriDeNghi: x.sGiaTriDeNghi,
//                        fTongMucDauTu: x.fTongMucDauTu,
//                        sTongMucDauTu: x.sTongMucDauTu,
//                        sTrangThaiDuAnDangKy: x.sTrangThaiDuAnDangKy,
//                        sGhiChu: x.sGhiChu,
//                    };
//                    lstDuAn.push(newItem);
//                })

//                DrawTableDuAn();
//                SumValueDetailTable();
//            }
//        }
//    });

//}

var lstDuAnId = [];
function EventCheckbox() {
    $("#tblLstDuAn [type=checkbox]").on("change", function () {
        lstDuAnId = [];
        $.each($("#tblLstDuAn [type=checkbox]"), function (index, item) {
            if (item.checked) {
                lstDuAnId.push($(item).data('id'));
            }
        })
    });
    lstDuAnId = [];
    $.each($("#tblLstDuAn [type=checkbox]"), function (index, item) {
        if (item.checked) {
            lstDuAnId.push($(item).data('id'));
        }
    });
}

function LoadDefaultDuAnChecked() {
    if (lstDuAn != undefined && lstDuAn != "") {
        $.each(lstDuAnId, function (item, value) {
            $("[data-id='" + value.iID_DuAnID + "']").prop("checked", false);
        });
    }
}

function DrawTableDuAn() {
    $("#tblLstDuAn tbody").empty();
    var sHtml = [];
    $.each(lstDuAn, function (index, value) {
        sHtml.push("<tr data-id='" + value.iID_DuAnID + "'>");
        sHtml.push(`    <td><input type='checkbox' data-id='${value.iID_DuAnID}'/></td>'`);
        sHtml.push("    <td>" + value.sMaDuAn + "</td>");
        sHtml.push("    <td>" + value.sTenDuAn + "</td>");
        sHtml.push("    <td class='text-right' class='sTongMucDauTu'>" + value.sTongMucDauTu + "</td>");
        //if (bIsTongHop) {
        //    sHtml.push("    <td style='text-align:center'> "
        //        + "          </td > ")
        //} else {
        //    sHtml.push("    <td style='text-align:center'> "
        //        + "             <button class= 'btn-delete'> <i class='fa fa-trash-o fa-lg' aria-hidden='true' onclick=\"DeleteDetailItem(this)\"></i></button >"
        //        + "          </td > ")
        //}
        sHtml.push("</tr>");
    });
    $("#tblLstDuAn tbody").html(sHtml.join());
    EventCheckbox();
    LoadDefaultDuAnChecked();

}


function DrawTableDuAnChiTiet() {
    $("#tblLstDuAn tbody").empty();
    var sHtml = [];
    var lst = lstDuAn.filter(x => (x.Id != undefined || x.Id != null || x.Id != GUID_EMPTY));
    //lstDuAn.forEach(x => x.iID_DuAnID
        
    //    lstDuAn.push(newItem);
    //})


    $.each(lstDuAn, function (index, value) {
        if (value.Id == undefined || value.Id == null || value.Id == GUID_EMPTY) {
            sHtml.push("<tr data-id='" + value.iID_DuAnID + "'>");
            sHtml.push(`    <td><input type='checkbox' data-id='${value.iID_DuAnID}' /></td>'`);
            sHtml.push("    <td>" + value.sMaDuAn + "</td>");
            sHtml.push("    <td>" + value.sTenDuAn + "</td>");
            sHtml.push("    <td class='text-right' class='sTongMucDauTu'>" + value.sTongMucDauTu + "</td>");
            //if (bIsTongHop) {
            //    sHtml.push("    <td style='text-align:center'> "
            //        + "          </td > ")
            //} else {
            //    sHtml.push("    <td style='text-align:center'> "
            //        + "             <button class= 'btn-delete'> <i class='fa fa-trash-o fa-lg' aria-hidden='true' onclick=\"DeleteDetailItem(this)\"></i></button >"
            //        + "          </td > ")
            //}
            sHtml.push("</tr>");
        } else {
            sHtml.push("<tr data-id='" + value.iID_DuAnID + "'>");
            sHtml.push(`    <td><input type='checkbox' checked data-id='${value.iID_DuAnID}' /></td>'`);
            sHtml.push("    <td>" + value.sMaDuAn + "</td>");
            sHtml.push("    <td>" + value.sTenDuAn + "</td>");
            sHtml.push("    <td class='text-right' class='sTongMucDauTu'>" + value.sTongMucDauTu + "</td>");
            //if (bIsTongHop) {
            //    sHtml.push("    <td style='text-align:center'> "
            //        + "          </td > ")
            //} else {
            //    sHtml.push("    <td style='text-align:center'> "
            //        + "             <button class= 'btn-delete'> <i class='fa fa-trash-o fa-lg' aria-hidden='true' onclick=\"DeleteDetailItem(this)\"></i></button >"
            //        + "          </td > ")
            //}
            sHtml.push("</tr>");
        }

    });
    $("#tblLstDuAn tbody").html(sHtml.join());
    EventCheckbox();
    LoadDefaultDuAnChecked();

}

function DrawTableKHVUChiTiet() {
    $("#tblLstDuAn tbody").empty();
    var sHtml = [];
    $.each(lstDuAn, function (index, value) {
        sHtml.push("<tr data-id='" + value.iID_DuAnID + "' onClick='RowClick(\"" + value.iID_DuAnID + "\")'>");
        sHtml.push(`    <td></td>'`);
        sHtml.push("    <td>" + value.sMaDuAn + "</td>");
        sHtml.push("    <td>" + value.sTenDuAn + "</td>");
        sHtml.push("    <td class='text-right' class='sTongMucDauTu'>" + value.sTongMucDauTu + "</td>");
        sHtml.push("    <td class='text-right' class='sTongMucDauTu'>" + value.sTongMucDauTu + "</td>");
        sHtml.push("    <td class='text-right' class='sTongMucDauTu'>" + value.sTongMucDauTu + "</td>");
        //if (bIsTongHop) {
        //    sHtml.push("    <td style='text-align:center'> "
        //        + "          </td > ")
        //} else {
        //    sHtml.push("    <td style='text-align:center'> "
        //        + "             <button class= 'btn-delete'> <i class='fa fa-trash-o fa-lg' aria-hidden='true' onclick=\"DeleteDetailItem(this)\"></i></button >"
        //        + "          </td > ")
        //}
        sHtml.push("</tr>");
    });
    $("#tblLstDuAn tbody").html(sHtml.join());
    EventCheckbox();
    LoadDefaultDuAnChecked();

}

function UpdateDuAn(hienTai) {
    var dongHienTai = hienTai.closest("tr");
    var id = $(dongHienTai).attr("data-id");

    var sGiaTriDeNghi = $(dongHienTai).find(".txtGiaTriDeNghi").val();
    var fGiaTriDeNghi = sGiaTriDeNghi == "" ? 0 : parseInt(UnFormatNumber(sGiaTriDeNghi));
    var sGhiChu = $(dongHienTai).find(".txtGhiChu").val();

    var objDuAn = lstDuAn.filter(x => x.iID_DuAnID == id)[0];
    lstDuAn = lstDuAn.filter(function (x) { return x.iID_DuAnID != id });
    objDuAn.sGiaTriDeNghi = sGiaTriDeNghi;
    objDuAn.fGiaTriDeNghi = fGiaTriDeNghi;
    objDuAn.sGhiChu = sGhiChu;

    lstDuAn.push(objDuAn);
    SumValueDetailTable();
}

function GetDataDetailByChungTuTongHopCreate() {
    lstDuAn = [];

    $.ajax({
        url: "/QLVonDauTu/KeHoachVonUngDeXuat/GetDuAnByChungTuTongHop",
        type: "POST",
        data: { lstChungTuId: lstIdTongHop },
        dataType: "json",
        cache: false,
        success: function (result) {
            lstDetail = [];
            $("#ViewTable").empty();
            if (result.status) {
                result.lstDuAn.forEach(x => {
                    var newItem = {
                        iID_DuAnID: x.iID_DuAnID,
                        sMaDuAn: x.sMaDuAn,
                        sTenDuAn: x.sTenDuAn,
                        fGiaTriDeNghi: x.fGiaTriDeNghi,
                        sGiaTriDeNghi: Number(x.fGiaTriDeNghi).toLocaleString('vi-VN'),
                        fTongMucDauTu: x.fTongMucDauTu,
                        sTongMucDauTu: Number(x.fTongMucDauTu).toLocaleString('vi-VN'),
                        sTrangThaiDuAnDangKy: x.sTrangThaiDuAnDangKy,
                        sGhiChu: x.sGhiChu,
                        Id:x.Id
                    };
                    lstDuAn.push(newItem);
                })

                DrawTableDuAn();
                SumValueDetailTable();
                DrawTableDanhSachChungTu(result.lstKhvu);
            }
        }
    });
}

function DrawTableDanhSachChungTu(lstChungTu) {
    $("#tblDanhSachChungTu tbody").empty();
    var sHtml = [];
    $.each(lstChungTu, function (index, value) {
        sHtml.push("<tr>");
        sHtml.push("    <td>" + (value.sSoDeNghi != null ? value.sSoDeNghi : '') + "</td>");
        sHtml.push("    <td class='text-center'>" + (value.sNgayDeNghi != null ? value.sNgayDeNghi : '') + "</td>");
        sHtml.push("    <td class='text-center'>" + (value.iNamKeHoach != null ? value.iNamKeHoach : '') + "</td>");
        sHtml.push("    <td>" + (value.sTenNguonVon != null ? value.sTenNguonVon : '') + "</td>");
        sHtml.push("    <td>" + (value.sTenDonVi != null ? value.sTenDonVi : '') + "</td>");
        sHtml.push("    <td class='text-right'>" + (value.sSumGiaTriDeNghi != null ? value.sSumGiaTriDeNghi : '') + "</td>");
        sHtml.push("</tr>");
    });
    $("#tblDanhSachChungTu tbody").html(sHtml.join());
    $("#div_DanhSachChungTu").show();
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

function SumValueDetailTable() {
    var fSumTongMucDauTu = 0;
    var fSumGiaTriDeNghi = 0;
    $.each(lstDuAn, function (index, item) {
        if (item.isDelete == undefined || item.isDelete == false) {
            fSumTongMucDauTu += parseFloat(item.fTongMucDauTu);
            fSumGiaTriDeNghi += parseFloat(item.fGiaTriDeNghi);
        }
    });
    $("#fTongMucDauTu").text(Number(fSumTongMucDauTu).toLocaleString('vi-VN'));
    $("#fTongThanhToan").text(Number(fSumGiaTriDeNghi).toLocaleString('vi-VN'));
}

function ValidateData() {
    $("#btnSave").attr('disabled', true);
    var messErrors = [];
    var objDonVi = $("#iID_DonViQuanLyID").val();
    var sSoKeHoach = $("#sSoDeNghi").val();
    var dNgayDeNghi = $("#dNgayDeNghi").val();
    var iNamKeHoach = $("#iNamKeHoach").val();
    var iId_NguonVon = $("#iId_NguonVon").val();

    if (!bIsTongHop && (objDonVi == null || objDonVi == "")) {
        messErrors.push("Chưa chọn đơn vị.");
    }
    if (sSoKeHoach == null || sSoKeHoach == "") {
        messErrors.push("Chưa nhập số kế hoạch.");
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
    objDeXuat = {};
    objDeXuat.Id = $("#Id").val();
    objDeXuat.sSoDeNghi = sSoKeHoach;
    objDeXuat.dNgayDeNghi = dNgayDeNghi;
    objDeXuat.iNamKeHoach = iNamKeHoach;
    if (bIsTongHop) {
        objDeXuat.sTongHop = lstIdTongHop.join(",")
    }

    objDeXuat.iID_DonViQuanLyID = $("#iID_DonViQuanLyID :selected").data("iiddonvi");
    objDeXuat.iID_MaDonViQuanLy = objDonVi;

    objDeXuat.iID_NguonVonID = iId_NguonVon;

    if (lstDuAn == null || lstDuAn.length == 0) {
        alert("Chưa nhập dữ liệu dự án đề xuất.");
        $("#btnSave").attr('disabled', false);
        return false;
    }
    return true;
}

function CheckDupicateSoQuyetDinh() {
    var iID_KeHoachUngID = $("#Id").val();
    if (isDieuChinh)
        iID_KeHoachUngID = GUID_EMPTY;
    var sSoQuyetDinh = $("#sSoDeNghi").val();
    var check = false;
    $.ajax({
        url: "/QLVonDauTu/KeHoachVonUngDeXuat/CheckExistSoQuyetDinh",
        type: "POST",
        data: { iID_KeHoachUngID: iID_KeHoachUngID, sSoQuyetDinh: sSoQuyetDinh },
        dataType: "json",
        async: false,
        success: function (result) {
            if (result.status) {
                alert("Đã tồn tại số đề nghị " + sSoQuyetDinh + " .");
                check = true;
            }       
        }
    });
    return check;
}



function Insert() {
    objDeXuat = {};
    var selectedDuAn = [];
    lstDuAn.forEach(duAn => {
        if (lstDuAnId.indexOf(duAn.iID_DuAnID) >= 0) {
            selectedDuAn.push(duAn);
        }
    })
    if (!ValidateData()) {
        return;
    } else {    
        EventCheckbox();
    } 
    
    if (CheckDupicateSoQuyetDinh()) {
        return false;
    }
    if ($("#sTongHop").val().length != 0) {
        isTongHop = true;
    }
    $.ajax({
        url: "/QLVonDauTu/KeHoachVonUngDeXuat/QLKeHoachVonUngDxSave",
        type: "POST",
        data: {
            data: objDeXuat,
            lstData: selectedDuAn,
            isInsert: isInsert,
            isDieuChinh: isDieuChinh
        },
        dataType: "json",
        async: false,
        success: function (result) {
            if (result.status) {
                var messAlert = "Cập nhật dữ liệu thành công.";
                if (result.isDieuChinh) {
                    messAlert = "Điều chỉnh dữ liệu thành công.";
                } else {
                    if (result.isinsert)
                        messAlert = "Tạo mới dữ liệu thành công.";

                }
                alert(messAlert);
                //if (result.isinsert) {
                //    window.location.href = "/QLVonDauTu/KeHoachVonUngDeXuat/Update/" + result.ID;
                //}
                //else {
                //    window.location.href = "/QLVonDauTu/KeHoachVonUngDeXuat";
                //}
                $.ajax({
                    type: "POST",
                    dataType: "json",
                    url: "/QLVonDauTu/KeHoachVonUngDeXuat/GetListDuAnKHVUDC",
                    data: {
                        listData: lstDuAnId
                    },
                    success: function (data) {
                        if (data.status) {
                            window.location.href = "/QLVonDauTu/KeHoachVonUngDeXuat/DetailChiTiet?idKHVU=" + result.ID + "&isUpdate=" + !result.isinsert + "&isTongHop=" + isTongHop + "&isDieuChinh=" + isDieuChinh;

                        } else {
                            alert("Chưa có dự án");
                        }

                    }
                });

            } else {
                alert("Có lỗi xảy ra khi lưu dữ liệu.");
            }
            $("#btnSave").attr('disabled', false);
        }
    })
}

function CancelSaveData() {
    window.location.href = "/QLVonDauTu/KeHoachVonUngDeXuat";
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

function Save() {
    var obj = {};
    var listIds = [];
    var listFGiaTriUng = [];
    obj.iID_DonViQuanLyID = $("#txt_iID_DonViQuanLyID").val();
    obj.iID_MaDonViQuanLy = $("#txt_iID_MaDonViQuanLy").val();
    obj.iID_NguonVonID = $("#txt_iID_NguonVonID").val();
    obj.sSoDeNghi = $("#txtSoKeHoachModal").val();
    obj.dNgayDeNghi = $("#dNgayDeNghiModal").val();
    obj.iNamKeHoach = $("#txtNamKeHoachModal").val();
    obj.Id = $("#Id").val();

    $.each($("#tblDataTongHop tbody tr"), function (index, child) {
        var idChild = $(child).find(".iIdKhvChild").val();
        var objfGiatriUng = {};
        var fGiatriUng = UnFormatNumber($(child).find(".fGiatriUngChild").text());
        if (idChild != null | idChild != undefined || idChild != GUID_EMPTY || idChild != "") {
            listIds.push(idChild);
        }
        if (fGiatriUng != null | fGiatriUng != undefined || fGiatriUng != "") {
            objfGiatriUng.fGiaTriDeNghi = fGiatriUng;
            listFGiaTriUng.push(objfGiatriUng);
        }
        //iIdNguonVon = $(child).closest("tr").data("iidnguonvon");
        //    var isLockHienTai = $(child).closest("tr").data("lock");
    });

    $.ajax({
        url: "/QLVonDauTu/KeHoachVonUngDeXuat/QLKeHoachVonUngDxSave",
        type: "POST",
        data: {
            data: obj,
            lstData: listFGiaTriUng,
            isTongHop: true,
            listIdChild: listIds
        },
        dataType: "json",
        async: false,
        success: function (result) {
            if (result.status) {
                //    alert("Đã tồn tại số đề nghị " + sSoQuyetDinh + " .");
                var messAlert = "Cập nhật dữ liệu thành công.";
                if (obj.Id == GUID_EMPTY)
                    messAlert = "Tạo mới dữ liệu thành công.";
                alert(messAlert);
                if ($("#txtIstongHop").val() == 1) isTongHop = true;
                window.location.href = "/QLVonDauTu/KeHoachVonUngDeXuat/DetailChiTiet?idKHVU=" + result.ID + "&isUpdate=" + !result.isinsert + "&isTongHop=" + isTongHop;
                //if (result.isinsert) {
                //    window.location.href = "/QLVonDauTu/KeHoachVonUngDeXuat/Update/" + result.ID;
                //}
                //else {
                //    window.location.href = "/QLVonDauTu/KeHoachVonUngDeXuat";
                //}

            } else {
                alert("Có lỗi xảy ra khi lưu dữ liệu.");

            }
        }
    });
}

// import
function ImportKHVUDeXuat() {
    window.location.href = "/QLVonDauTu/KeHoachVonUngDeXuat/ViewImport/";
}

//export
function ExportKHVUDeXuat(id) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonUngDeXuat/KeHoachVonUngDeXuatExport",
        data: { id: id },
        success: function (data) {
            if (data.bIsComplete) {
                window.location.href = "/QLVonDauTu/KeHoachVonUngDeXuat/ExportExcelKHVUDeXuat";
            }
        }
    });
}

// Dieu chinh KehoachVonUngDX

function DieuChinhKHVU(id, isDieuChinh) {
    location.href = "/QLVonDauTu/KeHoachVonUngDeXuat/Update?id=" + id + "&isDieuChinh=" + isDieuChinh;
}
