var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var TBL_DANH_SACH_NGUON_VON_DV = 'tblListDMNguonNganSachDV';
var TBL_DANH_SACH_NGUON_VON_TH = 'tblListDMNguonNganSachTH';
var TBL_DANH_SACH_DVQL = 'tblListDonViQuanLy';

$(document).ready(function () {

    $("#" + TBL_DANH_SACH_DVQL + " .cbAll_DVQL").change(function () {

        if (this.checked) {
            $("#" + TBL_DANH_SACH_DVQL + " .cb_DVQL").filter(function () { return $(this.parentElement.parentElement).css("display") != "none" }).prop('checked', true).trigger("change");
        }

        else {
            $("#" + TBL_DANH_SACH_DVQL + " .cb_DVQL").filter(function () { return $(this.parentElement.parentElement).css("display") != "none" }).prop('checked', false).trigger("change");;
        }

    });

    //$("#tblListDonViQuanLy .cbAll_DVQL").change(function () {
    //    if (this.checked)
    //        $("#tblListDonViQuanLy .cb_DVQL").prop('checked', true).trigger("change");
    //    else
    //        $("#tblListDonViQuanLy .cb_DVQL").prop('checked', false).trigger("change");
    //});

    //$("#tblListNguonVon .cbAll_NguonVon").change(function () {
    //    if (this.checked)
    //        $("#tblListNguonVon .cb_NguonVon").prop('checked', true).trigger("change");
    //    else
    //        $("#tblListNguonVon .cb_NguonVon").prop('checked', false).trigger("change");
    //})

    //$("#tblListNguonVonTH .cbAll_NguonVonTH").change(function () {
    //    if (this.checked)
    //        $("#tblListNguonVonTH .cb_NguonVonTH").prop('checked', true).trigger("change");
    //    else
    //        $("#tblListNguonVonTH .cb_NguonVonTH").prop('checked', false).trigger("change");
    //});


});

function backViewKHVNDX() {
    window.location.href = "/QLVonDauTu/KeHoachVonNamDeXuat";
}

function ExportBaoCao(data, arrIdNguonVon, arrDonVi, bIsTongHop, isPdf) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonNamDeXuat/ExportBaoCao",
        data: { data: data, arrIdNguonVon: arrIdNguonVon, arrDonVi: arrDonVi, bIsTongHop: bIsTongHop, isPdf: isPdf },
        success: function (r) {
            if (r.status) {
                if (isPdf) {
                    window.open("/QLVonDauTu/KeHoachVonNamDeXuat/ExportExcel?Pdf=1", '_blank');
                } else {
                    window.open("/QLVonDauTu/KeHoachVonNamDeXuat/ExportExcel?Pdf=0", '_blank');
                }

            } else {
                var sMessage = "không có dữ liệu !";
                var sTitle = "Lỗi in báo cáo !";
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: sTitle, Messages: sMessage, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
            }
        }
    });
}

function GetDonViBCDV() {
    var arr = [];
    $("#tblListDonViQuanLy input[name='cb_DVQL']:checked").each(function () {
        arr.push($(this).val());
    });
    return arr;
}

function GetNguonVonBCDV() {
    var arr = [];
    $("#tblListNguonVon input[name='cb_NguonVon']:checked").each(function () {
        var rowValue = $(this).val();
        if (rowValue != 'on') {
            arr.push(rowValue);
        }
    });
    return arr;
}

function GetNguonVonBCTH() {
    var arr = [];
    $("#tblListNguonVonTH input[name='cb_NguonVonTH']:checked").each(function () {
        var rowValue = $(this).val();
        if (rowValue != 'on') {
            arr.push(rowValue);
        }
    });
    return arr;
}

//function exportBCTheoDonVi(data, arrIdNguonVon, arrDonVi) {
//    console.log(data);
//    console.log(arrIdNguonVon);
//    console.log(arrDonVi);
//    var isStatus = $("#txtStatusReport").val();

//    $.ajax({
//        type: "POST",
//        url: "/QLVonDauTu/KeHoachVonNamDeXuat/ExportBCTheoDonVi",
//        data: { data: data, arrIdNguonVon: arrIdNguonVon, arrDonVi: arrDonVi, isStatus: isStatus},
//        success: function (r) {
//            if (r) {
//                console.log(1);
//       /*         window.location.href = "/QLVonDauTu/KeHoachVonNamDeXuat/ExportExcel/?isStatus=" + isStatus;*/
//                window.open("/QLVonDauTu/KeHoachVonNamDeXuat/ExportExcel/?isStatus=" + isStatus, '_blank');
//            }
//        }
//    });
//}

//function exportBCTongHop(data, arrIdNguonVon, arrIdDVQL) {
//    console.log(data);
//    console.log(arrIdNguonVon);
//    console.log(arrIdDVQL);
//    var isStatus = $("#txtStatusReport").val();
//    $.ajax({
//        type: "POST",
//        url: "/QLVonDauTu/KeHoachVonNamDeXuat/ExportBCTongHop",
//        data: { data: data, arrIdNguonVon: arrIdNguonVon, arrIdDVQL: arrIdDVQL, isStatus: isStatus},
//        success: function (r) {
//            if (r) {
//                console.log(2);
//                //window.location.href = "/QLVonDauTu/KeHoachVonNamDeXuat/ExportExcel/?isStatus=" + isStatus;
//                window.open("/QLVonDauTu/KeHoachVonNamDeXuat/ExportExcel/?isStatus=" + isStatus, '_blank');
//            }
//        }
//    });
//}

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