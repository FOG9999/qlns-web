var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var bIsSaveSuccess = false;
var data = [];
var lstDuAn = [];

//$(document).ready(function () {

//    $("#" + TBL_DANH_SACH_DUAN + " .cbAll_duan").change(function () {

//        if (this.checked) {
//            $("#" + TBL_DANH_SACH_DUAN + " .cb_duan").filter(function () { return $(this.parentElement.parentElement).css("display") != "none" }).prop('checked', true).trigger("change");
//        }

//        else {
//            $("#" + TBL_DANH_SACH_DUAN + " .cb_duan").filter(function () { return $(this.parentElement.parentElement).css("display") != "none" }).prop('checked', false).trigger("change");;
//        }

//    });
//});


function ResetChangePage(iCurrentPage = 1) {
    var sSoQuyetDinh = "";
    var dNgayQuyetDinhFrom = "";
    var dNgayQuyetDinhTo = "";
    var iNamKeHoach = "";
    var iID_NguonVonID = "";
    var iID_DonViQuanLyID = "";

    GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iNamKeHoach, iID_NguonVonID, iID_DonViQuanLyID, iCurrentPage, 1);
}

function ChangePage(iCurrentPage = 1) {
    var tabIndex = $('input[name=groupChungTuTongHop]:checked').val();
    var sSoQuyetDinh = $("#txtSoQuyetdinh").val();
    var dNgayQuyetDinhFrom = $("#dNgayQuyetDinhFrom").val();
    var dNgayQuyetDinhTo = $("#dNgayQuyetDinhTo").val();
    var iNamKeHoach = $("#txtNamKeHoach").val();
    var iID_NguonVonID = $("#iID_NguonVonID").val();
    var iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();

    GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iNamKeHoach, iID_NguonVonID, iID_DonViQuanLyID, iCurrentPage, tabIndex);
}

function GetListData(sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo, iNamKeHoach, iID_NguonVonID, iID_DonViQuanLyID, iCurrentPage, tabIndex) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/KeHoachVonNamDeXuat/KeHoachVonNamDeXuatSearch",
        data: { _paging: _paging, sSoQuyetDinh: sSoQuyetDinh, dNgayQuyetDinhFrom: dNgayQuyetDinhFrom, dNgayQuyetDinhTo: dNgayQuyetDinhTo, iNamKeHoach: iNamKeHoach, iID_NguonVonID: iID_NguonVonID, iID_DonViQuanLyID: iID_DonViQuanLyID, tabIndex: tabIndex },
        success: function (data) {
            $("#lstDataView").html(data);

            $("#txtSoQuyetdinh").val(sSoQuyetDinh);
            $("#dNgayQuyetDinhFrom").val(dNgayQuyetDinhFrom);
            $("#dNgayQuyetDinhTo").val(dNgayQuyetDinhTo);
            $("#txtNamKeHoach").val(iNamKeHoach);
            $("#iID_NguonVonID").val(iID_NguonVonID);
            $("#iID_DonViQuanLy").val(iID_DonViQuanLyID);
        }
    });
}

function OpenModal(id, isDieuChinh, isTongHop, bIsDetail, isUpdate) {
    $("#modalKHVonNamDeXuat").attr('tabindex', '');
    var lstDataChecked = JSON.parse(sessionStorage.getItem('dataVonNamDeXuatChecked'));
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/KeHoachVonNamDeXuat/GetModal",
        data: { id: id, isTongHop: isTongHop, bIsDetail, lstItem: lstDataChecked },
        success: function (data) {
            if (isTongHop) {
                var jsonData = null;
                try {
                    jsonData = JSON.parse(data);
                }
                catch (error) {
                    jsonData = null;
                }

                if (jsonData != null && jsonData.bIsComplete == false) {
                    //document.getElementById('modalKHVonNamDeXuat').classList.toggle("hidden");
                  //  $('#modalKHVonNamDeXuat').modal('toggle');
                    var Title = 'Lỗi tổng hợp nhu cầu vốn đơn vị đề xuất!';
                    var messErr = [];
                    messErr.push(jsonData.sMessError);
                    $.ajax({
                        type: "POST",
                        url: "/Modal/OpenModal",
                        data: { Title: Title, Messages: messErr, Category: ERROR },
                        success: function (data) {
                            $("#divModalConfirm").html(data);
                        }
                    });
                    return;

                }
               // $("#btnShowModalTongHop").click();
            }
            $("#contentModalKHVonNamDeXuat").html(data);
            $("#txt_bIsDetail").val(bIsDetail);
            $("#txt_DieuChinh").val(isDieuChinh);
            $("#txt_TongHop").val(isTongHop);
           
            if (id == null || id == GUID_EMPTY || id == undefined) {
                if (isTongHop === 'true') {
                    $("#modalKHVonNamDeXuatLabel").html('Tổng hợp nhu cầu vốn đơn vị đề xuất');
                }
                else {
                    $("#modalKHVonNamDeXuatLabel").html('Thêm mới nhu cầu vốn đơn vị đề xuất');
                }
            }
            else {
                if (isDieuChinh === 'true') {
                    $("#modalKHVonNamDeXuatLabel").html('Điều chỉnh nhu cầu vốn đơn vị đề xuất');
                } else {
                    if (isUpdate) {
                        $("#modalKHVonNamDeXuatLabel").html('Sửa kế nhu cầu vốn đơn vị đề xuất');

                    } else {
                        $("#modalKHVonNamDeXuatLabel").html('Sửa kế nhu cầu vốn đơn vị đề xuất');

                    }
                }
            }
            $(".date").datepicker({
                todayBtn: "linked",
                language: "it",
                autoclose: true,
                todayHighlight: true,
                format: 'dd/mm/yyyy'
            });
            $("#btnShowModalTongHop").click();
        }
    });
}

function LockItem(id, sSoQuyetDinh, iKhoa) {
    var Title = 'Xác nhận ' + (iKhoa ? 'mở' : 'khóa') + ' nhu cầu vốn đơn vị đề xuất';
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
        url: "/QLVonDauTu/KeHoachVonNamDeXuat/KeHoachVonNamDeXuatLock",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                ChangePage(1);
            }
        }
    });
}

function PopupModal(title, message, category) {
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: title, Messages: message, Category: category },
        success: function (data) {
            $("#divModalConfirm").html(data);
            $('#confirmModal').on('hidden.bs.modal', function () {
                if (bIsSaveSuccess) {
                    location.reload();
                }
            })
        }
    });
}

function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/KeHoachVonNamDeXuat/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalKHVonNamDeXuat").html(data);
            $("#modalKHVonNamDeXuatLabel").html('Chi tiết nhu cầu vốn đơn vị đề xuất');
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
function OpenExport(id) {

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonNamDeXuat/KeHoachVonNamDeXuatExport",
        data: { iID_KeHoachVonNamDeXuatID: id },
        success: function (data) {
            if (data.status) {
                window.location.href = "/QLVonDauTu/KeHoachVonNamDeXuat/KehoachVonNamDeXuatExportExcel";
            }
        }
    });
}

function EventCheckboxNotChange() {
    var listDuAnIds = [];
        $.each($("#tblDuAn [type=checkbox]"), function (index, item) {
            if (item.checked) {
                listDuAnIds.push($(item).data('idduan'));
            }
        })
    return listDuAnIds;
}

function Save() {
    var data = {};
    data.iID_KeHoachVonNamDeXuatID = $("#iID_KHVonNamDeXuatIDModal").val();
    data.iID_DonViQuanLyID = $("#iID_DonViQuanLyIDModal").val();
    data.sSoQuyetDinh = $("#txtSoKeHoachModal").val();
    data.dNgayQuyetDinh = $("#dNgayQuyetDinhModal").val();
    data.iID_NguonVonID = $("#iID_NguonVonIDModal").val()
    data.iNamKeHoach = $("#txtNamKeHoachModal").val();
    data.sNguoiLap = $("#txtNguoiLapModal").val();
    data.sTruongPhong = $("#txtTruongPhongModal").val();
    var isDieuChinh = $("#txt_DieuChinh").val();
    var isTongHop = $('#txt_TongHop').val();
    var bIsDetail = $("#txt_bIsDetail").val();
    if (!ValidateData(data)) {
        return false;
    }
    var strDuAnID = EventCheckboxNotChange().toString();
    //var lstDuAnChecked = JSON.parse(sessionStorage.getItem('DuAnChecked'));

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonNamDeXuat/KeHoachVonNamDeXuatSave",
        data: { data: data, isDieuChinh: isDieuChinh, isTongHop: isTongHop, bIsDetail: bIsDetail, strDuAnID: strDuAnID},
        success: function (r) {
            if (r.bIsComplete) {
                if (isTongHop) {
            //        window.location.href = "/QLVonDauTu/KeHoachVonNamDeXuat/Detail?id=" + r.iID + "&isDieuChinh=" + isDieuChinh; + "&bIsDetail=" + bIsDetail;
                    window.location.href = "/QLVonDauTu/KeHoachVonNamDeXuat/Detail?id=" + r.iID + "&isDieuChinh=" + isDieuChinh; 
                } else {
                    window.location.href = "/QLVonDauTu/KeHoachVonNamDeXuat/Detail?id=" + r.iID + "&isDieuChinh=" + isDieuChinh;
                }
                
            } else {
                var Title = 'Lỗi lưu nhu cầu vốn đơn vị đề xuất';
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

function ValidateData(data) {
    var Title = 'Lỗi thêm mới nhu cầu vốn đơn vị đề xuất';
    var Messages = [];

    if (data.iID_DonViQuanLyID == null || data.iID_DonViQuanLyID == "") {
        Messages.push("Đơn vị quản lý chưa chọn !");
    }

    if (data.sSoQuyetDinh == null || data.sSoQuyetDinh == "") {
        Messages.push("Số kế hoạch chưa nhập !");
    }

    if (data.dNgayQuyetDinh == null || data.dNgayQuyetDinh == "") {
        Messages.push("Ngày lập chưa nhập !");
    }

    if (data.iID_NguonVonID == null || data.iID_NguonVonID == "") {
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

function DeleteItem(id, sSoQuyetDinh = '') {
    var Title = 'Xác nhận xóa nhu cầu vốn đơn vị đề xuất';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn xóa chứng từ ' + sSoQuyetDinh + '?');
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
        url: "/QLVonDauTu/KeHoachVonNamDeXuat/KeHoachVonNamDeXuatDelete",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                ChangePage(1);
            }
        }
    });
}

function ViewInBaoCao(isStatus) {
    window.location.href = "/QLVonDauTu/KeHoachVonNamDeXuat/ViewInBaoCao/?isStatus=" + isStatus;
}

function ImportKHVNDX() {
    window.location.href = "/QLVonDauTu/KeHoachVonNamDeXuat/ImportKHVNDX/";
}

//Chi tiết kế hoạch vốn năm đề xuất
function ChiTietKeHoachVonNamDeXuat(id, isDieuChinh, isDetail) {
    window.location.href = "/QLVonDauTu/KeHoachVonNamDeXuat/Detail?id=" + id + "&isDieuChinh=" + isDieuChinh + "&bIsDetail=" + isDetail;
}

function ChangeChungTu() {
    ChangePage(1);
}

function SetState(key) {
    var idItem = "." + key + ":checked";
    var elementValue = $(idItem).val();

    var itemValue = {
        isChecked: false,
        iID_KeHoachVonNamDeXuatID: null
    };
    itemValue.isChecked = (elementValue == "on") ? true : false;
    itemValue.iID_KeHoachVonNamDeXuatID = key;
    data.push(itemValue);

    sessionStorage.setItem('dataVonNamDeXuatChecked', JSON.stringify(data));
}

function ChooseDuAn(key) {
    var idItem = "." + key + ":checked";
    var elementValue = $(idItem).val();

    var itemValue = {
        IsChecked: false,
        IDDuAnID: null
    };

    itemValue.IsChecked = (elementValue == "on") ? true : false;
    itemValue.IDDuAnID = key;
    data.push(itemValue);
    sessionStorage.setItem('DuAnChecked', JSON.stringify(data));
}

function handleSearch() {
    // Declare variables
    var input, filter, table, tr, td, i, txtValue;
    input = document.getElementById("search");
    filter = removeVietnameseTones(input.value.toUpperCase());
    table = document.getElementById("tblListDuAn");
    tr = table.getElementsByTagName("tr");

    // Loop through all table rows, and hide those who don't match the search query
    for (i = 1; i < tr.length; i++) {
        td = tr[i].getElementsByTagName("td")[2];
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