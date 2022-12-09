var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;

function ResetChangePage(iCurrentPage = 1) {
    var sMaNhaThau = "";
    var sTenNhaThau = "";
    var sDiaChi = "";
    var sDaiDien = "";
    var sChucVu = "";
    var sDienThoai = "";
    var sFax = "";
    var sEmail = "";
    var sWebsite = "";
    var sSoTaiKhoan = "";
    var sNganHang = "";
    var sMaSoThue = "";
    var sNguoiLienHe = "";
    var sDienThoaiLienHe = "";
    var sMaNganHang = "";

    GetListData(sMaNhaThau, sTenNhaThau, sDiaChi, sDaiDien, sChucVu, sDienThoai, sFax, sEmail, sWebsite, sSoTaiKhoan, sNganHang, sMaSoThue, sNguoiLienHe, sDienThoaiLienHe, sMaNganHang, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var sMaNhaThau = $("#txtMaNhaThau").val();
    var sTenNhaThau = $("#txtTenNhaThau").val();
    var sDiaChi = $("#txtDiaChi").val();
    var sDaiDien = $("#txtDaiDien").val();
    var sChucVu = $("#txtChucVu").val();
    var sDienThoai = $("#txtDienThoai").val();
    var sFax = $("#txtFax").val();
    var sEmail = $("#txtEmail").val();
    var sWebsite = $("#txtWebsite").val();
    var sSoTaiKhoan = $("#txtSoTaiKhoan").val();
    var sNganHang = $("#txtNganHang").val();
    var sMaSoThue = $("#txtMaSoThue").val();
    var sNguoiLienHe = $("#txtNguoiLienHe").val();
    var sDienThoaiLienHe = $("#txtDienThoaiLienHe").val();
    var sMaNganHang = $("#txtMaNganHang").val();

    GetListData(sMaNhaThau, sTenNhaThau, sDiaChi, sDaiDien, sChucVu, sDienThoai, sFax, sEmail, sWebsite, sSoTaiKhoan, sNganHang, sMaSoThue, sNguoiLienHe, sDienThoaiLienHe, sMaNganHang, iCurrentPage);
}

function GetListData(sMaNhaThau, sTenNhaThau, sDiaChi, sDaiDien, sChucVu, sDienThoai, sFax, sEmail, sWebsite, sSoTaiKhoan, sNganHang, sMaSoThue, sNguoiLienHe, sDienThoaiLienHe, sMaNganHang, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLDMNhaThau/DanhMucNhaThauSearch",
        data: { _paging: _paging, sMaNhaThau: sMaNhaThau, sTenNhaThau: sTenNhaThau, sDiaChi: sDiaChi, sDaiDien: sDaiDien, sChucVu: sChucVu, sDienThoai: sDienThoai, sFax: sFax, sEmail: sEmail, sWebsite: sWebsite, sSoTaiKhoan: sSoTaiKhoan, sNganHang: sNganHang, sMaSoThue: sMaSoThue, sNguoiLienHe: sNguoiLienHe, sDienThoaiLienHe: sDienThoaiLienHe, sMaNganHang: sMaNganHang },
        success: function (data) {
            $("#lstDataView").html(data);
            
            $("#txtMaNhaThau").val(sMaNhaThau);
            $("#txtTenNhaThau").val(sTenNhaThau);
            $("#txtDiaChi").val(sDiaChi);
            $("#txtDaiDien").val(sDaiDien);
            $("#txtChucVu").val(sChucVu);
            $("#txtDienThoai").val(sDienThoai);
            $("#txtFax").val(sFax);
            $("#txtEmail").val(sEmail);
            $("#txtWebsite").val(sWebsite);
            $("#txtSoTaiKhoan").val(sSoTaiKhoan);
            $("#txtNganHang").val(sNganHang);
            $("#txtMaSoThue").val(sMaSoThue);
            $("#txtNguoiLienHe").val(sNguoiLienHe);
            $("#txtDienThoaiLienHe").val(sDienThoaiLienHe);
            $("#txtMaNganHang").val(sMaNganHang);
        }
    });
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLDMNhaThau/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalNhaThau").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalNhaThauLabel").html('Thêm mới nhà thầu');
            }
            else {
                $("#modalNhaThauLabel").html('Sửa thông tin nhà thầu');
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

function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLDMNhaThau/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalNhaThau").html(data);
            $("#modalNhaThauLabel").html('Chi tiết nhà thầu');
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

function DeleteItem(id) {
    var Title = 'Xác nhận xóa nhà thầu';
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
        url: "/QLVonDauTu/QLDMNhaThau/NhaThauDelete",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                ChangePage();
            }
        }
    });
}

function Save() {
    var data = {};
    data.iID_NhaThauID = $("#iID_NhaThauModal").val();
    data.sMaNhaThau = $("#txtsMaNhaThau").val();
    data.sTenNhaThau = $("#txtsTenNhaThau").val();
    data.sDiaChi = $("#txtsDiaChi").val();
    data.sMaSoThue = $("#txtsMaSoThue").val()
    data.sDaiDien = $("#txtsDaiDien").val();
    data.sChucVu = $("#txtsChucVu").val();
    data.sDienThoai = $("#txtsDienThoai").val();
    data.sFax = $("#txtsFax").val();
    data.sEmail = $("#txtsEmail").val();
    data.sWebsite = $("#txtsWebsite").val();
    data.sSoTaiKhoan = $("#txtsSoTK").val();
    data.sNganHang = $("#txtsNganHang").val();
    data.sNguoiLienHe = $("#txtsNguoiLH").val();
    data.sDienThoaiLienHe = $("#txtsDTLH").val();
    data.sMaNganHang = $("#txtsMaNganHang").val();
    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLDMNhaThau/NhaThauSave",
        data: { data: data},
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/QLVonDauTu/QLDMNhaThau";
            } else {
                var Title = 'Lỗi lưu nhà thầu';
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
    var Title = 'Lỗi thêm mới nhà thầu';
    if ($("#iID_NhaThauModal").val() != GUID_EMPTY) {
        Title = 'Lỗi chỉnh sửa nhà thầu';
    }

    var Messages = [];

    if (data.sMaNhaThau == null || data.sMaNhaThau == "") {
        Messages.push("Mã nhà thầu chưa nhập !");
    }

    if (data.sTenNhaThau == null || data.sTenNhaThau == "") {
        Messages.push("Tên nhà thầu chưa nhập !");
    }

    if ($.trim($("#txtsEmail").val()) != "" && !CheckEmail($.trim($("#txtsEmail").val()))) {
        Messages.push("Email không đúng định dạng!");
    }

    if ($.trim($("#txtsFax").val()) != "" && !CheckPhoneOrFax($.trim($("#txtsFax").val()))) {
        Messages.push("Số fax nhập không hợp lệ!");
    }

    if ($.trim($("#txtsDTLH").val()) != "" && !CheckPhoneOrFax($.trim($("#txtsDTLH").val()))) {
        Messages.push("Số điện thoại liên hệ không đúng định dạng!");
    }

    if ($.trim($("#txtsMaSoThue").val()) != "" && !CheckAlphanumeric($.trim($("#txtsMaSoThue").val()))) {
        Messages.push("Mã số thuế nhập không hợp lệ!");
    }

    if ($.trim($("#txtsSoTK").val()) != "" && !CheckAlphanumeric($.trim($("#txtsSoTK").val()))) {
        Messages.push("Số tài khoản nhập không hợp lệ!");
    }

    if ($.trim($("#txtsMaNhaThau").val()) != "" && $.trim($("#txtsMaNhaThau").val()).length > 100) {
        Messages.push("Mã nhà thầu vượt quá 100 kí tự !");
    }

    if ($.trim($("#txtsTenNhaThau").val()) != "" && $.trim($("#txtsTenNhaThau").val()).length > 300) {
        Messages.push("Tên nhà thầu vượt quá 300 kí tự !");
    }

    if ($.trim($("#txtsDiaChi").val()) != "" && $.trim($("#txtsDiaChi").val()).length > 300) {
        Messages.push("Địa chỉ vượt quá 300 kí tự !");
    }
    if ($.trim($("#txtsDaiDien").val()) != "" && $.trim($("#txtsDaiDien").val()).length > 100) {
        Messages.push("Đại diện vượt quá 100 kí tự !");
    }

    if ($.trim($("#txtsWebsite").val()) != "" && $.trim($("#txtsWebsite").val()).length > 300) {
        Messages.push("Website vượt quá 300 kí tự !");
    }

    if ($.trim($("#txtsMaSoThue").val()) != "" && $.trim($("#txtsMaSoThue").val()).length > 100) {
        Messages.push("Mã số thuế vượt quá 100 kí tự !");
    }

    if ($.trim($("#txtsNganHang").val()) != "" && $.trim($("#txtsNganHang").val()).length > 100) {
        Messages.push("Ngân hàng vượt quá 100 kí tự !");
    }

    if ($.trim($("#txtsSoTK").val()) != "" && $.trim($("#txtsSoTK").val()).length > 100) {
        Messages.push("Số tài khoản vượt quá 100 kí tự !");
    }

    if ($.trim($("#txtsChucVu").val()) != "" && $.trim($("#txtsChucVu").val()).length > 100) {
        Messages.push("Chức vụ vượt quá 100 kí tự !");
    }

    if ($.trim($("#txtsDienThoai").val()) != "" && $.trim($("#txtsDienThoai").val()).length > 100) {
        Messages.push("Điện thoại vượt quá 100 kí tự !");
    }

    if ($.trim($("#txtsFax").val()) != "" && $.trim($("#txtsFax").val()).length > 100) {
        Messages.push("Fax vượt quá 100 kí tự !");
    }

    if ($.trim($("#txtsNguoiLH").val()) != "" && $.trim($("#txtsNguoiLH").val()).length > 300) {
        Messages.push("Người liên hệ vượt quá 300 kí tự !");
    }
    if ($.trim($("#txtsEmail").val()) != "" && $.trim($("#txtsEmail").val()).length > 100) {
        Messages.push("Email vượt quá 100 kí tự !");
    }
    if ($.trim($("#txtsDTLH").val()) != "" && $.trim($("#txtsDTLH").val()).length > 100) {
        Messages.push("Số điện thoại liên hệ vượt quá 100 kí tự !");
    }


    //if (CheckExistMaNhaThau(data.sMaNhaThau)) {
    //    Messages.push("Đã tồn tại mã nhà thầu !");
    //}

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

function CheckExistMaNhaThau(sMaNhaThau) {

}
