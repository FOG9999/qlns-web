var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var data_search = {};
var iThongTri = '00000000-0000-0000-0000-000000000000';
var sUrlListView = "";
var idContentView = "lstDataView";
var arr_thanhtoan = [];
var ftongpheduyetusd = 0;
var ftongpheduyetvnd = 0;
var type_screen = 0; //0: Màn hình danh sách thông tri cấp phát, 1: danh sách phê duyệt thanh toán
var type_action = 0; //0 : thêm mới, 1: sửa đổi, 2:xem
var arr_error = [];
var CONFIRM = 0;
var ERROR = 1;
var obj_data = {};

function ChangePage(iCurrentPage = 1) {
    data_search = {};
    _paging.CurrentPage = iCurrentPage;
    data_search._paging = _paging;
    if (type_screen == 0) {
        data_search.iDonVi = $('#idonvisearch').val();
        data_search.sSongThongTri = $("<div/>").text($.trim($('#imathongtrisearch').val())).html();
        data_search.dNgayLap = $('#ingaytaosearch').val();
        data_search.iNam = $('#inamthuchiensearch').val();
    }
    else {
        data_search.iThongTri = iThongTri;
        data_search.iDonVi = $('#idonvi').val();
        data_search.iNam = $('#inam').val();
        data_search.iLoaiThongTri = $('#iloaithongtri').val();
        data_search.iLoaiNoiDung = $('#iloainoidung').val();
        data_search.iTypeAction = type_action;
    }
 
    GetListData(iCurrentPage);
}

function ResetChangePage(iCurrentPage = 1) {
    data_search = {};
    _paging.CurrentPage = iCurrentPage;
    data_search._paging = _paging;
    data_search.iDonVi = GUID_EMPTY;

    GetListData(iCurrentPage);
}

function GetListData(iCurrentPage) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { data: data_search },
        success: function (data) {
            $("#" + idContentView).html(data);
            if (type_screen == 1) {
                LoadTableDanhSachPheDuyet();
            } else {
                AddConditionAfterSearch();
            }
        }
    });
}

function AddConditionAfterSearch() {
    $('#idonvisearch').val(data_search.iDonVi);
    $('#imathongtrisearch').val($("<div/>").html(data_search.sSongThongTri).text());
    $('#ingaytaosearch').val(data_search.dNgayLap);
    $('#inamthuchiensearch').val(data_search.iNamThongTri);
}

function LoadTableDanhSachPheDuyet() {
    var index_check = 0;
    var index_row = 0;
    $('#tblListPheDuyetTT tbody tr').each(function (index, tr) {
        index_row = index_row + 1;
        var idthanhtoan = $(tr).find('td').data('idthanhtoan');
        if (arr_thanhtoan.includes(idthanhtoan)) {
            $(tr).find('#icheckitem').prop("checked", true);
            index_check = index_check + 1;
        }
    });
    if (index_check == index_row && index_row >0 ) { // Số lượng bản ghi trên 1 trang là 5 bản ghi
        $('#tblListPheDuyetTT').find('#icheckheader').prop("checked", true);
    }

    $('#itongpheduyetusd').text(FormatNumber(ftongpheduyetusd, 2));
    $('#itongpheduyetvnd').text(FormatNumber(ftongpheduyetvnd, 0));
}

function onModal(id = '00000000-0000-0000-0000-000000000000') {
    sUrlListView = '/QLNH/ThongTriCapPhat/GetListThanhToanThongTri';
    idContentView = 'lstThanhToanThongTri';
    ftongpheduyetusd = 0;
    ftongpheduyetvnd = 0;
    type_screen = 1;
    arr_thanhtoan = [];
    if (id == GUID_EMPTY) {
        type_action = 0;
        data_search = {};
    }
    else {
        type_action = 1;
        iThongTri = id;
        data_search.iThongTri = iThongTri;
        data_search.iDonVi = $('#idonvi').val();
        data_search.iNam = $('#inam').val();
        data_search.iLoaiThongTri = $('#iloaithongtri').val();
        data_search.iLoaiNoiDung = $('#iloainoidung').val();
        data_search.iTypeAction = type_action;
    }
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/ThongTriCapPhat/Create",
        data: { data: data_search },
        success: function (data) {
            $('#iModalThongTri').modal('toggle');
            $('#iModalThongTri').modal('show');
            $("#iModalThongTri").empty().html(data);
            if (type_action == 1) {
                getThongTriCapPhatByID(id);
            }
        }
    });
}

function onModalDetail(id) {
    sUrlListView = '/QLNH/ThongTriCapPhat/GetListThanhToanThongTri';
    idContentView = 'lstThanhToanThongTri';
    type_screen = 1;
    type_action = 2;
    iThongTri = id;

    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/ThongTriCapPhat/Detail",
        async: false,
        data: { IdThongTri: id, iTypeAction: type_action },
        success: function (data) {
            $('#iModalThongTri').modal('toggle');
            $('#iModalThongTri').modal('show');
            $("#iModalThongTri").empty().html(data);
            $("#icheckheader").attr("disabled", "disabled");
            getThongTriCapPhatByID(id);
        }
    });
}

function getThongTriCapPhatByID(id) {
    $.ajax({
        type: "POST",
        dataType: "Json",
        url: "/QLNH/ThongTriCapPhat/GetThongTriByID",
        data: { id: id},
        success: function (data) {
            if (data.status == true) {
                setValue(data.thongtri, data.chitiets);
            }
        }
    });
}

function setValue(obj, obj_chitiet) {
    $('#imathongtri').val(obj.sSoThongTri);
    $('#ingaylap').val(obj.sNgayLap);
    $('#inam').val(obj.iNamThongTri);
    $('#idonvi').val(obj.iID_DonViID);
    $('#iloaithongtri').val(obj.iLoaiThongTri);
    $('#iloainoidung').val(obj.iLoaiNoiDungChi);
    ftongpheduyetusd = obj.fThongTri_USD;
    ftongpheduyetvnd = obj.fThongTri_VND;

    obj_chitiet.forEach(function (item, index) {
        arr_thanhtoan.push(item.iID_ThanhToanID);
    })

    $('#itongpheduyetusd').text(FormatNumber(ftongpheduyetusd, 2));
    $('#itongpheduyetvnd').text(FormatNumber(ftongpheduyetvnd, 0));

    LoadTableDanhSachPheDuyet();

}


function onModalChiTietDeNghi(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DeNghiThanhToan/Detail",
        data: { id: id, isModal : true },
        success: function (data) {
            $("#contentModalChiTietThanhToan").empty().html(data);
            $("#modalChiTietThanhToanLabel").empty().html('Chi tiết đề nghị thanh toán');
        }
    });

    //window.location.href = "/QLNH/DeNghiThanhToan/Detail?id=" + id;
}

function LoadThongTinNam() {
    var today = new Date();
    var year = today.getFullYear();
    var str = "<option value=''>--Chọn năm--</option>";
    for (i = 0; i < 10; i++) {
        str = str + "<option value='" + year + "'>" + year + "</option>";

        year = year - 1;
    }
    $('#inam').append(str);
}

function checkChonDeNghiThanhToan(td_dong, isCheckAll, isCheck) {
    var parentElemnt = td_dong.parentElement.parentElement;
    var fpheduyetusd = $(parentElemnt).find('#ipheduyetusd').text();
    var fpheduyetvnd = $(parentElemnt).find('#ipheduyetvnd').text();
    var idthanhtoan = $(parentElemnt).find('td').data('idthanhtoan');
    var checkbox = $(parentElemnt).find('#icheckitem');
    if ((isNaN(isCheckAll) && $(checkbox).is(":checked")) || (isCheckAll == true && isCheck == true)) {
        arr_thanhtoan.push(idthanhtoan);
        $(td_dong).prop("checked", isCheckAll);
        if (fpheduyetusd != '' || fpheduyetusd != undefined) {
            ftongpheduyetusd = ftongpheduyetusd + parseFloat(UnFormatNumber(fpheduyetusd));
        }
        if (fpheduyetvnd != '' || fpheduyetvnd != undefined) {
            ftongpheduyetvnd = ftongpheduyetvnd + parseInt(UnFormatNumber(fpheduyetvnd));
        }
    }
    else {
        arr_thanhtoan = arr_thanhtoan.filter(function (elem) {
            return elem != idthanhtoan;
        });
        $(td_dong).prop("checked", isCheck);
        if (fpheduyetusd != '' || fpheduyetusd != undefined) {
            ftongpheduyetusd = ftongpheduyetusd - parseFloat(UnFormatNumber(fpheduyetusd));
        }
        if (fpheduyetvnd != '' || fpheduyetvnd != undefined) {
            ftongpheduyetvnd = ftongpheduyetvnd - parseInt(UnFormatNumber(fpheduyetvnd));
        }
    }
    var index = 0;
    var index_check = 0;
    var index_uncheck = 0;
    var lstCheck = $("#tblListPheDuyetTT tbody input:checkbox").map(function () {
        index = index + 1;
        if ($(this).is(":checked") == true) {
            index_check = index_check + 1;
        }
        else {
            index_uncheck = index_uncheck + 1;
        }
        return 1;
    }).get();

    if (index_check == index) {
        $("#tblListPheDuyetTT #icheckheader").prop("checked", true);
    }
    else {
        $("#tblListPheDuyetTT #icheckheader").prop("checked", false);
    }

    if (!isNaN(ftongpheduyetusd)) {
        $('#itongpheduyetusd').text(FormatNumber(ftongpheduyetusd, 2));
    }
    if (!isNaN(ftongpheduyetvnd)) {

        $('#itongpheduyetvnd').text(FormatNumber(ftongpheduyetvnd, 0));
    }
 
}

function checkAllDeNghiThanhToan() {
    //Nếu đang check thì thực hiện all uncheck
    var checkAll = $('#tblListPheDuyetTT').find('#icheckheader');
    var isCheckAll = true;
    var isCheck = false;
    if ($(checkAll).is(":checked")) {
        isCheck = true;
    }
    $("#tblListPheDuyetTT tbody tr").each(function (index, tr) {
        var td_checkbox = $(tr).find('#icheckitem');
        if (td_checkbox.length == 1) {
            $(tr).find('td').find('input').each(function (index, input) {
                checkChonDeNghiThanhToan(this, isCheckAll, isCheck);
            })
        }
    });
}

function onChangeThongTriCapPhat() {
    arr_thanhtoan = [];
    ftongpheduyetusd = 0;
    ftongpheduyetvnd = 0;
    ChangePage();
}

function checkTrungMaThongTri() {
    var mathongtri = $('#imathongtri').val();
    var result = false;

    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/ThongTriCapPhat/CheckTrungMaThongTri",
        data: { mathongtri: mathongtri, type_action: type_action, imathongtri: iThongTri },
        async: false,
        success: function (data) {
            var object = JSON.parse(data);
            if (object.status == true) {
                result = object.results;
            }
        }
    });
    return result;
}

function ValidateBeforeSave() {
    var result = false;
    arr_error = [];
    if ($.trim($('#imathongtri').val()) == "") {
        arr_error.push('Mã thông tri không được để trống!');
    } else if ($.trim($('#imathongtri').val()).length > 100) {
        arr_error.push('Mã thông tri nhập quá 100 kí tự!');
    } else if ($.trim($('#ingaylap').val()) == "") {
        arr_error.push('Thông tin ngày lập không được để trống!');
    } else if ($('#idonvi').val() == GUID_EMPTY) {
        arr_error.push('Chưa chọn thông tin đơn vị!');
    } else if ($.trim($('#inam').val()) == "") {
        arr_error.push('Chưa chọn năm!');
    } else if ($('#iloaithongtri').val() == "") {
        arr_error.push('Chưa chọn loại thông tri!');
    } else if ($('#iloainoidung').val() == "") {
        arr_error.push('Chưa chọn loại nội dung!');
    } else if (checkTrungMaThongTri() == true) {
        arr_error.push('Mã thông tri bị trùng!');
    } else {
        result = true;
    }
    return result;
}


function GetData() {
    obj_data.ID = iThongTri;
    obj_data.sSoThongTri = $("<div/>").text($.trim($('#imathongtri').val())).html();
    obj_data.dNgayLap = $('#ingaylap').val();
    obj_data.iID_DonViID = $('#idonvi').val();
    obj_data.iNamThongTri = $('#inam').val();
    obj_data.iLoaiThongTri = $('#iloaithongtri').val();
    obj_data.iLoaiNoiDungChi = $('#iloainoidung').val();
    obj_data.sThongTri_USD = $('#itongpheduyetusd').text() == "" ? $('#itongpheduyetusd').text() : UnFormatNumber($('#itongpheduyetusd').text());
    obj_data.sThongTri_VND = $('#itongpheduyetvnd').text() == "" ? $('#itongpheduyetvnd').text() : UnFormatNumber($('#itongpheduyetvnd').text());
    obj_data.sIdThanhToans = arr_thanhtoan.length > 0 ? arr_thanhtoan.join(',') : '';
}

function SaveData() {
    if (ValidateBeforeSave()) {
        GetData();
        $.ajax({
            type: "POST",
            dataType: "html",
            url: "/QLNH/ThongTriCapPhat/SaveData",
            data: { data: obj_data, type_action: type_action },
            async: false,
            success: function (data) {
                if (data == 'True') {
                    $('#iModalThongTri').modal('toggle');
                    $('#iModalThongTri').modal('hide');
                    //Set lại phân trang 
                    sUrlListView = "/QLNH/ThongTriCapPhat/ThongTriCapPhatSearch";
                    idContentView = "lstDataView";
                    type_screen = 0;
                    ChangePage(1);
                }
            }
        });
    }
    else {
        showErr(ERROR);
    }
}


function onModalThongBaoXoa(id) {
    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTriCapPhat/onModelThongBaoXoa",
        data: { id: id },
        success: function (data) {
            $('#iModalThongTri').modal('toggle');
            $('#iModalThongTri').modal('show');
            $("#iModalThongTri").html(data);
        }
    });
}

function DeleteData(id) {
    $.ajax({
        url: "/ThongTriCapPhat/DeleteThongTriCapPhat",
        type: "POST",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                ResetChangePage(1);
            }
            else {
                arrError.push('Xóa dữ liệu không thành công');
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: "Thông báo", Messages: arr_error, Category: CONFIRM },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
            }
        }
    })

}

function showErr(CONFIRM) {
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: "Thông báo", Messages: arr_error, Category: CONFIRM },
        success: function (data) {
            $("#divModalConfirm").html(data);
            arrError = [];
        }
    });
}

function ChoiceAllCheckBox() {
    var checkAll = $('#tblListThongTriCapPhat').find('.checkbox-header');
    $("#tblListThongTriCapPhat tbody tr .itemcheckbox").prop("checked", $(checkAll).is(":checked"));
}

function ChoiceCheckboxRow() {
    var countCheckbox = $("#tblListThongTriCapPhat tbody tr .itemcheckbox").length;
    var countCheckboxChecked = $("#tblListThongTriCapPhat tbody tr .itemcheckbox:checked").length;
    $('#tblListThongTriCapPhat').find('.checkbox-header').prop("checked", countCheckbox == countCheckboxChecked);
}

function ViewInBaoCao() {
    arr_error = [];
    var arrLink = [];
    var arrIdThongTri= [];
    $("#tblListThongTriCapPhat tbody tr").each(function (index, tr) {
        var checkbox = $(tr).find('.itemcheckbox');
        if ($(checkbox).is(":checked")) {
            arrIdThongTri.push($(tr).data('idthongtri'));
        }
    });
    if (arrIdThongTri.length > 0) {
        for (i = 0; i < arrIdThongTri.length; i++) {
            var idThongTri = arrIdThongTri[i];
            arrLink.push("/QLNH/ThongTriCapPhat/ExportBaoCaoThongTriCapPhat?idThongTri=" + idThongTri);
        }
        openLinks(arrLink);
    }
    else {
        arr_error.push("Chưa chọn thông tri cấp phát");
        showErr(CONFIRM);
    }
}