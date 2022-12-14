var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var bIsSaveSuccess = false;
var TBL_DANH_SACH_DVTHDA = 'tblListDonViQuanLy';


// Index
$(document).ready(function () {
    $("#" + TBL_DANH_SACH_DVTHDA + " .cbAll_DVQL").change(function () {

        if (this.checked) {
            $("#" + TBL_DANH_SACH_DVTHDA + " .cb_DVQL").filter(function () { return $(this.parentElement.parentElement).css("display") != "none" }).prop('checked', true).trigger("change");
        }

        else {
            $("#" + TBL_DANH_SACH_DVTHDA + " .cb_DVQL").filter(function () { return $(this.parentElement.parentElement).css("display") != "none" }).prop('checked', false).trigger("change");;
        }

    })
})


ResetChangePage = (iCurrentPage = 1) => {
    var sSoQuyetDinh = "";
    var dNgayQuyetDinhFrom = "";
    var dNgayQuyetDinhTo = "";
    var iNamKeHoach = "";
    var iID_NguonVonID = "";
    var iID_DonViQuanLyID = "";

    GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iNamKeHoach, iID_NguonVonID, iID_DonViQuanLyID, iCurrentPage);
}
ChangePage = (iCurrentPage = 1) => {
    var sSoQuyetDinh = "";
    var dNgayQuyetDinhFrom = "";
    var dNgayQuyetDinhTo = "";
    var iNamKeHoach = "";
    var iID_NguonVonID = "";
    var iID_DonViQuanLyID = "";

    GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iNamKeHoach, iID_NguonVonID, iID_DonViQuanLyID, iCurrentPage);
}

ChangePage = (iCurrentPage = 1) => {
    var sSoQuyetDinh = $("<div/>").text($.trim($("#txtSoQuyetdinh").val())).html();
    var dNgayQuyetDinhFrom = $("#dNgayQuyetDinhFrom").val();
    var dNgayQuyetDinhTo = $("#dNgayQuyetDinhTo").val();
    var iNamKeHoach = $("#txtNamKeHoach").val();
    var iID_NguonVonID = $("#iID_NguonVonID").val();
    var iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();

    GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iNamKeHoach, iID_NguonVonID, iID_DonViQuanLyID, iCurrentPage);
}

GetListData = (sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iNamKeHoach, iID_NguonVonID, iID_DonViQuanLyID, iCurrentPage) => {
    _paging.CurrentPage = iCurrentPage;

    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/KeHoachVonNamDuocDuyet/KeHoachVonNamDuocDuyetSearch",
        data: { _paging: _paging, sSoQuyetDinh: sSoQuyetDinh, dNgayQuyetDinhFrom: dNgayQuyetDinhFrom, dNgayQuyetDinhTo: dNgayQuyetDinhTo, iNamKeHoach: iNamKeHoach, iID_NguonVonID: iID_NguonVonID, iID_DonViQuanLyID: iID_DonViQuanLyID },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtSoQuyetdinh").val($("<div/>").html(sSoQuyetDinh).text());
            $("#dNgayQuyetDinhFrom").val(dNgayQuyetDinhFrom);
            $("#dNgayQuyetDinhTo").val(dNgayQuyetDinhTo);
            $("#txtNamKeHoach").val(iNamKeHoach);
            $("#iID_NguonVonID").val(iID_NguonVonID);
            $("#iID_DonViQuanLy").val(iID_DonViQuanLyID);
        }
    });
}

// Index

// Delete voucher
DeleteItem = (id) => {
    var Title = 'Xác nhận xóa chứng từ';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn xóa?');
    var FunctionName = "Delete('" + id + "')";
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
        success: function (data) {
            $("#divModalConfirm").html(data);
        }
    });
}

function Delete(id) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonNamDuocDuyet/KeHoach5NamDuocDuyetDelete",
        data: { id: id },
        success: function (data) {
            if (data.status == true) {
                ChangePage();
            }
        }
    });
}
// Delete voucher


// Insert And Update

OpenModal = (id, idDonViQuanLy = GUID_EMPTY, isModified = false, isView = false) => {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/KeHoachVonNamDuocDuyet/GetModal",
        data: { id: id, idDonViQuanLy: idDonViQuanLy, isModified: isModified, isView: isView },
        success: function (data) {
            $("#contentModalKHVonNamDuocDuyetPoupup").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalKHVonNamDuocDuyetLabel").html('Thêm mới dự toán được giao');
                var dt = new Date().toLocaleDateString('en-GB');
                $("#dNgayQuyetDinhModal").val(dt);
            }
            else {
                if ($("#VoucherModified").val() == "true") {
                    $("#modalKHVonNamDuocDuyetLabel").html('Điều chỉnh dự toán được giao');
                    $("#txtSoKeHoachModal").val("");
                    var dt = new Date().toLocaleDateString('en-GB');
                    $("#dNgayQuyetDinhModal").val(dt);
                }
                else if ($("#VoucherView").val() == "true") {
                    $("#modalKHVonNamDuocDuyetLabel").html('Xem dự toán được giao');
                }
                else {
                    $("#modalKHVonNamDuocDuyetLabel").html('Sửa dự toán được giao');
                }
            }
            $(".date").datepicker({
                todayBtn: "linked",
                language: "it",
                autoclose: true,
                todayHighlight: true,
                format: 'dd/mm/yyyy'
            });
        }
    });
}

ValidateData = (data) => {
    var Title = 'Lỗi thêm mới dự toán được giao';
    var Messages = [];

    if(data.l)

    if (data.iID_DonViQuanLyID == null || data.iID_DonViQuanLyID == "" || data.iID_DonViQuanLyID == GUID_EMPTY) {
        Messages.push("Đơn vị quản lý chưa chọn !");
    }

    if (data.sSoQuyetDinh == null || data.sSoQuyetDinh == "") {
        Messages.push("Số quyết định chưa nhập !");
    }
    if (data.sSoQuyetDinh.length>100 ) {
        Messages.push("Số quyết định nhiều hơn 100 kí tự !");
    }

    if (data.dNgayQuyetDinh == null || data.dNgayQuyetDinh == "") {
        Messages.push("Ngày lập chưa nhập !");
    }

    if (data.iID_NguonVonID == null || data.iID_NguonVonID == "" || data.iID_NguonVonID == GUID_EMPTY) {
        Messages.push("Nguồn vốn chưa chọn !");
    }

    if (data.iNamKeHoach == null || data.iNamKeHoach == "") {
        Messages.push("Năm kế hoạch chưa nhập !");
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
// Insert And Update

Save = () => {
    var data = {};
    data.iID_KeHoachVonNam_DuocDuyetID = $("#iID_KHVonNamDuocDuyetIDModal").val();
    data.iID_DonViQuanLyID = $("#iID_DonViQuanLyIDModal").val();
    data.sSoQuyetDinh = $("<div/>").text($.trim($("#txtSoKeHoachModal").val())).html();
    data.dNgayQuyetDinh = $("#dNgayQuyetDinhModal").val();
    data.iID_NguonVonID = $("#iID_NguonVonIDModal").val()
    data.iNamKeHoach = $("#txtNamKeHoachModal").val();
    data.iLoaiDuToan = $("#iLoaiDuToan").val();

    var isModified = $("#VoucherModified").val() == "true" ? true : false;

    if (!ValidateData(data)) {
        return false;
    }
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonNamDuocDuyet/KeHoachVonNamDuocDuyetSave",
        data: { data: data, isModified: isModified },
        success: function (r) {
            if (r.bIsComplete) {
                bIsSaveSuccess = true;
                $("#modalKHVonNamDeXuat").modal("hide");
                //PopupModal("Thông báo", "Lưu dữ liệu thành công", ERROR, r.iID);
                window.location.href = "/QLVonDauTu/KeHoachVonNamDuocDuyet/Detail?id=" + r.iID + "&isDetail=" + false;

            } else {
                var Title = 'Lỗi lưu dự toán được giao';
                var messErr = [];
                messErr.push(r.sMessError);
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

//popup
function PopupModal(title, message, category, iID) {
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: title, Messages: message, Category: category },
        success: function (data) {
            $("#divModalConfirm").html(data);
            $('#confirmModal').on('hidden.bs.modal', function () {
                if (bIsSaveSuccess) {
                    window.location.href = "/QLVonDauTu/KeHoachVonNamDuocDuyet/Detail?id=" + iID + "&isDetail=" + false;
                }
            })
        }
    });
}

// Details
ChiTietKeHoachVonNamDuocDuyet = (id,isDetail) => {
    window.location.href = "/QLVonDauTu/KeHoachVonNamDuocDuyet/Detail?id=" + id + "&isDetail=" + isDetail;
}
// Details

// Export
OpenExport = (id) => {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonNamDuocDuyet/KeHoachVonNamDuocDuyetExport",
        data: { idKeHoachVonNamDuocDuyet: id },
        success: function (data) {
            if (data.status) {
                window.location.href = "/QLVonDauTu/KeHoachVonNamDuocDuyet/ExportExcel";
            }
        }
    });
}
//Export

function SendFile(id, idDonVi, nam) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/KeHoachVonNamDuocDuyet/SendFile",
        data: { id: id, idDonVi: idDonVi, nam: nam },
        success: function (data) {            
            $.ajax({
                type: "POST",
                url: "/Modal/OpenModal",
                data: { Title: "Thông báo", Messages: "Gửi file thành công!", Category: 1 },
                async: false,
                success: function (data) {
                    $("#divModalConfirm").html(data);
                }
            });
        }
    });
}

// View In báo cáo
ViewInBaoCao = () => {
    window.location.href = "/QLVonDauTu/KeHoachVonNamDuocDuyet/ViewInBaoCao/";
}

BackIndex = () => {
    window.location.href = "/QLVonDauTu/KeHoachVonNamDuocDuyet/Index";
}

ValidateDataBaoCao = (data, arrIdDVQL) => {
    var Title = 'Lỗi in báo cáo';
    var Messages = [];

    if (data.sLoaiChungTu == null || data.sLoaiChungTu == "") {
        Messages.push("Loại chứng từ chưa chọn !");
    }

    if (data.sLoaiChungTu == "1") {
        if (data.sLoaiCongTrinh == null || data.sLoaiCongTrinh == "") {
            Messages.push("Loại công trình chưa chọn !");
        }
        if (data.sNguonVon == null || data.sNguonVon == "") {
            Messages.push("Nguồn vốn chưa chọn !");
        }
    }

    if (data.iNamLamViec == null || data.iNamLamViec == "") {
        Messages.push("Năm kế hoạch chưa nhập !");
    }

    if (arrIdDVQL.length == 0) {
        Messages.push("Đơn vị quản lý chưa chọn !");
    }

    if (data.sValueDonViTinh == null || data.sValueDonViTinh == "") {
        Messages.push("Đơn vị tính chưa chọn !");
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

PrintBaoCao = (isPdf = true) => {
    var data = {};
    var strIdLoaiCongTrinhTH = "";
    if ($("#iID_LoaiCongTrinh :selected").val() == GUID_EMPTY) {
        $("#iID_LoaiCongTrinh option").each(function () {
            if (this.value != GUID_EMPTY) {
                if (strIdLoaiCongTrinhTH == "") {
                    strIdLoaiCongTrinhTH += this.value;
                } else {
                    strIdLoaiCongTrinhTH += "," + this.value;
                }
            }
        });
        data.sLoaiCongTrinh = strIdLoaiCongTrinhTH;
    } else {
        data.sLoaiCongTrinh = $("#iID_LoaiCongTrinh :selected").val();
    }

    data.txtHeader1 = $("#txtHeader1").val();
    data.txtHeader2 = $("#txtHeader2").val();
    data.iNamLamViec = $("#txtNamKeHoach").val();
    data.sValueDonViTinh = $("#ValueItemDonViTinh :selected").val();
    data.sLoaiChungTu = $("#ValueItemLoaiChungTu :selected").val();
    data.sNguonVon = $("#iID_MaNguonNganSach :selected").val();
    data.sDonViTinh = $("#ValueItemDonViTinh :selected").html();

    var arrIdDVQL = [];
    $("#tblListDonViQuanLy input[type=checkbox]:checked").each(function () {
        var rowValue = $(this).val();
        if (rowValue != 'on') {
            arrIdDVQL.push(rowValue);
        }
    });

    if (!ValidateDataBaoCao(data, arrIdDVQL)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonNamDuocDuyet/PrintBaoCao",
        data: { dataReport: data, arrIdDVQL: arrIdDVQL, isPdf: isPdf},
        success: function (data) {
            if (data.status) {
                //window.location.href = "/QLVonDauTu/KeHoachVonNamDuocDuyet/ExportReport/?pdf=" + data.isPdf;
                window.open("/QLVonDauTu/KeHoachVonNamDuocDuyet/ExportReport/?pdf=" + data.isPdf, '_blank');
            }
            else {
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: "Lỗi in báo cáo", Messages: data.listErrMess, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
                return false;
            }
        }
    });
}


// View In báo cáo

function handleSearch() {
    // Declare variables
    var input, filter, table, tr, td, i, txtValue;
    input = document.getElementById("search");
    filter = removeVietnameseTones(input.value.toUpperCase());
    table = document.getElementById("tblListDonViQuanLy");
    tr = table.getElementsByTagName("tr");

    // Loop through all table rows, and hide those who don't match the search query
    for (i = 1; i < tr.length; i++) {
        td = tr[i].getElementsByTagName("td")[1];
        if (td) {
            txtValue = td.textContent || td.innerText;
            if (removeVietnameseTones(txtValue.toUpperCase()).indexOf(filter) > -1) {
                tr[i].style.display = "";
            } else {
                tr[i].style.display = "none";
            }
        }
    }
}


function removeVietnameseTones(str) {
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
    str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
    str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
    str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
    str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
    str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
    str = str.replace(/Đ/g, "D");
    // Some system encode vietnamese combining accent as individual utf-8 characters
    // Một vài bộ encode coi các dấu mũ, dấu chữ như một kí tự riêng biệt nên thêm hai dòng này
    str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, ""); // ̀ ́ ̃ ̉ ̣  huyền, sắc, ngã, hỏi, nặng
    str = str.replace(/\u02C6|\u0306|\u031B/g, ""); // ˆ ̆ ̛  Â, Ê, Ă, Ơ, Ư
    // Remove extra spaces
    // Bỏ các khoảng trắng liền nhau
    str = str.replace(/ + /g, " ");
    str = str.trim();
    // Remove punctuations
    // Bỏ dấu câu, kí tự đặc biệt
    str = str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'|\"|\&|\#|\[|\]|~|\$|_|`|-|{|}|\||\\/g, " ");
    return str;
}