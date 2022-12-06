// Define constants
var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var isUpdate;
var dataID = [];
//============================== Event List ================================//

function GetItemDataList(id) {
    window.location.href = "/QLVonDauTu/TraoDoiDuLieu/Insert/" + id;
}

function ViewDetailList(id) {
    window.location.href = "/QLVonDauTu/TraoDoiDuLieu/Detail/" + id;
}

function GetListData(sSoChungTu, dNgayChungtu, iNamKeHoach, iIDMaDonViQuanLy, sLoaiTraoDoi, sTrangThai, iCurrentPage) {
  _paging.CurrentPage = iCurrentPage;
  $.ajax({
    type: "POST",
    dataType: "html",
    url: "/QLVonDauTu/TraoDoiDuLieu/GetListView",
    data: {
      sSoChungTu: sSoChungTu, iNamKeHoach: iNamKeHoach, dNgayChungtu: dNgayChungtu,
      iIDMaDonVi: iIDMaDonViQuanLy, sLoaiTraoDoi: sLoaiTraoDoi, sTrangThai: sTrangThai, _paging: _paging
    },
    success: function (data) {
      $("#lstDataView").html(data);
      $("#txtSoChungTu").val(sSoChungTu);
      $("#txtNamLamViec").val(iNamKeHoach);
      $("#txtNgayChungTu").val(dNgayChungtu);
      $("#drpDonViQuanLy").val(iIDMaDonViQuanLy);
      $("#drpLoaiTraoDoi").val(sLoaiTraoDoi);
      $("#drpTrangThai").val(sTrangThai);   
    }
  });
}

function ChangePage(iCurrentPage = 1) {
  var sSoChungTu = $("#txtSoChungTu").val();
  var dNgayChungtu = $("#txtNgayChungTu").val();
  var iNamKeHoach = $("#txtNamLamViec").val();
  var iIDMaDonViQuanLy = $("#drpDonViQuanLy option:selected").val();
  var sLoaiTraoDoi = $("#drpLoaiTraoDoi option:selected").val();
  var sTrangThai = $("#drpTrangThai option:selected").val();
  GetListData(sSoChungTu, dNgayChungtu, iNamKeHoach, iIDMaDonViQuanLy, sLoaiTraoDoi, sTrangThai, iCurrentPage);
}

//function DeleteItemList(id) {
//    if (!confirm("Chấp nhận xóa bản ghi ?")) return;
//    var Title = 'Xóa trao đổi dữ liệu';
//    var Messages = 'Xóa trao đổi dữ liệu thành công!'
//  $.ajax({
//    type: "POST",
//      url: "/QLVonDauTu/TraoDoiDuLieu/XoaTraoDoiDuLieu",
//    data: { id: id },
//    success: function (r) {
//        if (r == "True") {
//            $.ajax({
//                type: "POST",
//                url: "/Modal/OpenModal",
//                data: { Title: Title, Messages: Messages, Category: ERROR },
//                success: function (data) {
//                    $("#divModalConfirm").html(data);
//                }
//            });
//        ChangePage();
//      }
//    }
//  });
//}


function DeleteItemList(id) {
    var Title = 'Xác nhận xóa trao đổi dữ liệu';
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
        url: "/QLVonDauTu/TraoDoiDuLieu/XoaTraoDoiDuLieu",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                window.location.href = "/QLVonDauTu/TraoDoiDuLieu";
            }
            else {
                var Title = 'Lỗi xóa trao đổi dữ liệu';
                var Messages = [];
                Messages.push("Lỗi xóa trao đổi dữ liệu!");
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: Messages, Category: ERROR },
                    success: function (data1) {
                        $("#divModalConfirm").html(data1);
                    }
                });
            }
        }
    });
    GetListData("", "", "", "", "", "", iCurrentPage = 1);
}
//function BtnInsertDataClick() {
//    location.href = "/QLVonDauTu/TraoDoiDuLieu/Insert";
//}

//function XuatFile(id) {
//    location.href = "/QLVonDauTu/TraoDoiDuLieu/XuatFile?id=" + id;
//}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/TraoDoiDuLieu/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalTraoDoiDuLieu").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalTraoDoiDuLieuLabel").html('Thêm mới trao đổi dữ liệu');
            }
            else {
               
                $("#modalTraoDoiDuLieuLabel").html('Sửa kế trao đổi dữ liệu');             
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

function Save() {
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
    var isUpdate = false;

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/TraoDoiDuLieu/TraoDoiDuLieuSave",
        data: { data: data, isUpdate: isUpdate },
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/QLVonDauTu/TraoDoiDuLieu/";
            } else {
                var Title = 'Lỗi lưu trao đổi dữ liệu';
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

function BtnTranferClick() {
    var lstDataChecked = JSON.parse(sessionStorage.getItem('datatddlChecked'));
    var lstChecked = $("#lstDataView [type=checkbox]:checked");
    var lstID = [];

    $.each(lstChecked, function (index, item) {
        lstID.push($(item).data("idchecked"));
    });

    //$.each(lstDataChecked, function (index,item) {
    //    lstID.push(item.ID);
    //});
   // $($("#lstDataView [type=checkbox]:checked")[1]).data("idchecked")

    var messErr = [];
    var Title = "Chuyển trạng thái";
    $.ajax({
        type: "POST",
        dataType: "json",
        url: "/QLVonDauTu/TraoDoiDuLieu/ChuyentrangThai",
        data: { listId: lstID },
        success: function (data) {
            if (data.bIsComplete) {
                messErr.push(data.sMessError);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
                ChangePage();
            } else {
                if (data.sMessError == null) {
                    alert("Chuyeern that bai");
                    ChangePage();
                } else {
                    messErr.push(data.sMessError);
                    $.ajax({
                        type: "POST",
                        url: "/Modal/OpenModal",
                        data: { Title: Title, Messages: messErr, Category: ERROR },
                        success: function (data) {
                            $("#divModalConfirm").html(data);
                        }
                    });
                    ChangePage();
                }
              
            }
        }
    });
    GetListData("", "", "", "", "", "", iCurrentPage = 1);
    //location.reload();

}

function SetState(key) {
    var idItem = "." + key + ":checked";
    var elementValue = $(idItem).val();

    var itemValue = {
        isChecked: false,
        ID: null
    };
    itemValue.isChecked = (elementValue == "on") ? true : false;
    itemValue.ID = key;
    dataID.push(itemValue);

    sessionStorage.setItem('datatddlChecked', JSON.stringify(dataID));
}

function ValidateData(data) {
    var Title = 'Lỗi thêm mới trao đổi dữ liệu';
    var Messages = [];

    if (data.iID_DonViQuanLyID == null || data.iID_DonViQuanLyID == "") {
        Messages.push("Đơn vị quản lý chưa chọn !");
    }

    //if (data.iID_KeHoach5NamDeXuatID == null || data.iID_KeHoach5NamDeXuatID == "") {
    //    Messages.push("Chứng từ đề xuất chưa chọn !");
    //}

    if (data.sSoChungTu == null || data.sSoChungTu == "") {
        Messages.push("Số kế hoạch chưa nhập !");
    }

    if (data.dNgayChungTu == null || data.dNgayChungTu == "") {
        Messages.push("Ngày chứng từ chưa nhập !");
    }

    if (data.iNamLamViec == null || data.iNamLamViec == "") {
        Messages.push("Năm kế hoạch chưa nhập !");
    }

    if (data.iThoiGian == null || data.iThoiGian == "") {
        Messages.push("Quý chưa chọn !");
    }

    if (data.iID_NguonVonID == null || data.iID_NguonVonID == "") {
        Messages.push("Nguồn vốn chưa chọn !");
    }

    if (data.iLoaiTraoDoi == null || data.iLoaiTraoDoi == "") {
        Messages.push("Loại trao đổi chưa chọn !");
    }

    if (data.iLoaiThongtri == null || data.iLoaiThongtri == "") {
        Messages.push("Loại thông tri chưa chọn !");
    }

    if (data.iLoaiDuToan == null || data.iLoaiDuToan == "") {
        Messages.push("Loại dự toán chưa chọn !");
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

function BtnInsertDataClick() {
    location.href = "/QLVonDauTu/TraoDoiDuLieu/Insert";
}

function Refresh() {
    location.reload();
}