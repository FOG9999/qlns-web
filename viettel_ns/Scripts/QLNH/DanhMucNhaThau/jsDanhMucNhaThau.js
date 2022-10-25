var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;

function ResetChangePage(iCurrentPage = 1) {
    var iLoai = 0;
    var sMaNhaThau = ""
    var sTenNhaThau = "";
    var sDiaChi = "";
    var sDaiDien = "";
    var sChucVu = "";
    var sDienThoai = "";
    var sSoFax = "";
    var sEmail = "";
    var sWebsite = "";

    GetListData(iLoai, sMaNhaThau, sTenNhaThau, sDiaChi, sDaiDien, sChucVu, sDienThoai, sSoFax, sEmail, sWebsite, iCurrentPage);
}

function ImportNT() {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucNhaThau/ImportNhaThau",
        success: function (data) {
            $("#contentModalNhaThau").empty().html(data);
            $("#modalNhaThauLabel").empty().html('Import nhà thầu');
        }
    });
}

function DownloadFileTemplate() {
    var url = "/QLNH/DanhMucNhaThau/DownloadTemplateImport";
    var arrLink = [];
    arrLink.push(url);
    openLinks(arrLink);
}

function ChangeFile(element) {
    var filePath = $(element).val();
    var fileNameArr = filePath.split("\\");
    $("#lblUpload span").empty().html(fileNameArr[fileNameArr.length - 1]);
}

function RefreshImport() {
    $("#inpFileUpload").val("");
    $("#lblUpload span").empty().html("Lựa chọn file excel");
    $("#tblNhaThauListImport tbody").empty();
}

function ChangePage(iCurrentPage = 1) {
    var iLoai = $("#slbILoai").val();
    var sMaNhaThau = $("<div/>").text($.trim($("#txt_MaNhaThau").val())).html();
    var sTenNhaThau = $("<div/>").text($.trim($("#txt_TenNhaThau").val())).html();
    var sDiaChi = $("<div/>").text($.trim($("#txt_Diachi").val())).html();
    var sDaiDien = $("<div/>").text($.trim($("#txt_DaiDien").val())).html();
    var sChucVu = $("<div/>").text($.trim($("#txt_ChucVu").val())).html();
    var sDienThoai = $("<div/>").text($.trim($("#txt_SoDienThoai").val())).html();
    var sSoFax = $("<div/>").text($.trim($("#txt_SoFax").val())).html();
    var sEmail = $("<div/>").text($.trim($("#txt_Email").val())).html();
    var sWebsite = $("<div/>").text($.trim($("#txt_Website").val())).html();
    GetListData(iLoai, sMaNhaThau, sTenNhaThau, sDiaChi, sDaiDien, sChucVu, sDienThoai, sSoFax, sEmail, sWebsite, iCurrentPage);
}

function GetListData(iLoai, sMaNhaThau, sTenNhaThau, sDiaChi, sDaiDien, sChucVu, sDienThoai, sSoFax, sEmail, sWebsite, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucNhaThau/DanhMucNhaThauSearch",
        data: { _paging: _paging, iLoai: iLoai, sMaNhaThau: sMaNhaThau, sTenNhaThau: sTenNhaThau, sDiaChi: sDiaChi, sDaiDien: sDaiDien, sChucVu: sChucVu, sDienThoai: sDienThoai, sSoFax: sSoFax, sEmail: sEmail, sWebsite: sWebsite},
        success: function (data) {
            $("#lstDataView").empty().html(data);

            $("#slbILoai").val(iLoai);
            $("#txt_MaNhaThau").val($("<div/>").html(sMaNhaThau).text());
            $("#txt_TenNhaThau").val($("<div/>").html(sTenNhaThau).text());
            $("#txt_Diachi").val($("<div/>").html(sDiaChi).text());
            $("#txt_DaiDien").val($("<div/>").html(sDaiDien).text());
            $("#txt_ChucVu").val($("<div/>").html(sChucVu).text());
            $("#txt_SoDienThoai").val($("<div/>").html(sDienThoai).text());
            $("#txt_SoFax").val($("<div/>").html(sSoFax).text());
            $("#txt_Email").val($("<div/>").html(sEmail).text());
            $("#txt_Website").val($("<div/>").html(sWebsite).text());
        }
    });
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucNhaThau/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalNhaThau").html(data);
            if (id == undefined || id == null || id == GUID_EMPTY) {
                $("#modalNhaThauLabel").html('Thêm mới nhà thầu');
            }
            else {
                $("#modalNhaThauLabel").html('Sửa thông tin nhà thầu');
            }

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

            $("#txtdNgayCapCMND").keydown(function (event) {
                ValidateInputKeydown(event, this, 3);
            }).blur(function (event) {
                setTimeout(() => {
                    if (!isShowing) ValidateInputFocusOut(event, this, 3);
                }, 0);
            });

            $("#slbsiLoai").select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });
        }
    });
}

function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucNhaThau/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalNhaThau").html(data);
            $("#modalNhaThauLabel").html('Chi tiết nhà thầu');
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
        url: "/QLNH/DanhMucNhaThau/NhaThauDelete",
        data: { id: id },
        success: function (r) {
            if (r.bIsComplete) {
                ChangePage();
            } else {
                var Title = 'Lỗi xóa nhà thầu';
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

function Save() {
    var data = {};
    data.Id = $("#Id_NhaThauModal").val();
    data.iLoai = $("#slbsiLoai").val();
    data.sMaNhaThau = $("<div/>").text($.trim($("#txtsMaNhaThau").val())).html();
    data.sTenNhaThau = $("<div/>").text($.trim($("#txtsTenNhaThau").val())).html();
    data.sDiaChi = $("<div/>").text($.trim($("#txtsDiaChi").val())).html();
    data.sMaSoThue = $("<div/>").text($.trim($("#txtsMaSoThue").val())).html();
    data.sDaiDien = $("<div/>").text($.trim($("#txtsDaiDien").val())).html();
    data.sChucVu = $("<div/>").text($.trim($("#txtsChucVu").val())).html();
    data.sDienThoai = $("<div/>").text($.trim($("#txtsDienThoai").val())).html();
    data.sFax = $("<div/>").text($.trim($("#txtsFax").val())).html();
    data.sEmail = $("<div/>").text($.trim($("#txtsEmail").val())).html();
    data.sWebsite = $("<div/>").text($.trim($("#txtsWebsite").val())).html();
    data.sSoTaiKhoan = $("<div/>").text($.trim($("#txtsSoTK").val())).html();
    data.sNganHang = $("<div/>").text($.trim($("#txtsNganHang").val())).html();
    data.sMaNganHang = $("<div/>").text($.trim($("#txtsMaNganHang").val())).html();
    data.sNguoiLienHe = $("<div/>").text($.trim($("#txtsNguoiLH").val())).html();
    data.sDienThoaiLienHe = $("<div/>").text($.trim($("#txtsDTLH").val())).html();
    data.sSoCMND = $("<div/>").text($.trim($("#txtsSoCMND").val())).html();
    data.sNoiCapCMND = $("<div/>").text($.trim($("#txtsNoiCapCMND").val())).html();
    data.dNgayCapCMND = $("<div/>").text($.trim($("#txtdNgayCapCMND").val())).html();

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucNhaThau/NhaThauSave",
        data: { data: data },
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/QLNH/DanhMucNhaThau";
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

function ConfirmRemoveRowImport(index) {
    var Title = 'Xác nhận xóa dòng dữ liệu nhà thầu';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn xóa?');
    var FunctionName = "RemoveRow('" + index + "')";
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
        success: function (data) {
            $("#divModalConfirm").empty().html(data);
        }
    });
}

function RemoveRow(index) {
    $("#btn-delete" + index).closest("tr").remove();
}

function ValidateFileImport() {
    var Title = 'Lỗi lấy dữ liệu từ file excel';
    var Messages = [];

    var has_file = $("#inpFileUpload").val() != '';
    if (!has_file) {
        Messages.push("Vui lòng chọn file excel dữ liệu !");
    }
    else {
        var ext = $("#inpFileUpload").val().split(".");
        ext = ext[ext.length - 1].toLowerCase();
        var arrayExtensions = ["xls", "xlsx"];
        if (arrayExtensions.lastIndexOf(ext) == -1) {
            Messages.push("Chọn file không đúng định dạng !");
        }
    }

    if (Messages.length > 0) {
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

function LoadDataExcel() {
    if (!ValidateFileImport()) {
        return false;
    }

    var fileInput = document.getElementById('inpFileUpload');
    var file = fileInput.files[0];
    var formData = new FormData();
    formData.append('file', file);
    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucNhaThau/LoadDataExcel",
        data: formData,
        contentType: false,
        processData: false,
        cache: false,
        async: false,
        success: function (r) {
            if (r.bIsComplete) {
                $("#tblNhaThauListImport tbody").empty().html(r.data);
                $(".selectLoai").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
                var isShowing = false;
                $('.inputDate').datepicker({
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

                $(".inputDate").keydown(function (event) {
                    ValidateInputKeydown(event, this, 3);
                }).blur(function (event) {
                    setTimeout(() => {
                        if (!isShowing) ValidateInputFocusOut(event, this, 3);
                    }, 0);
                });
            } else {
                var Title = 'Lỗi lấy dữ liệu từ file excel';
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
    var Title = 'Lỗi thêm mới/chỉnh sửa nhà thầu';
    var Messages = [];

    if (data.sMaNhaThau == null || data.sMaNhaThau == "") {
        Messages.push("Mã nhà thầu chưa nhập !");
    }

    if (data.sTenNhaThau == null || data.sTenNhaThau == "") {
        Messages.push("Tên nhà thầu chưa nhập !");
    }

    if (data.iLoai == null || data.iLoai == 0) {
        Messages.push("Loại nhà thầu chưa chọn !");
    }

    if ($.trim($("#txtsEmail").val()) != "" && !CheckEmail($.trim($("#txtsEmail").val()))) {
        Messages.push("Email không đúng định dạng!");
    }

    if ($.trim($("#txtsDienThoai").val()) != "" && !CheckPhoneOrFax($.trim($("#txtsDienThoai").val()))) {
        Messages.push("Số điện thoại không đúng định dạng!");
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

    if ($.trim($("#txtsSoCMND").val()) != "" && !CheckAlphanumeric($.trim($("#txtsSoCMND").val()))) {
        Messages.push("Số CMND nhập không hợp lệ!");
    }

    if ($.trim($("#txtsSoTK").val()) != "" && !CheckAlphanumeric($.trim($("#txtsSoTK").val()))) {
        Messages.push("Số tài khoản nhập không hợp lệ!");
    }

    if ($.trim($("#txtdNgayCapCMND").val()) != "" && !dateIsValid($.trim($("#txtdNgayCapCMND").val()))) {
        Messages.push("Ngày cấp CMND nhập không hợp lệ!");
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
    if ($.trim($("#txtsMaNganHang").val()) != "" && $.trim($("#txtsMaNganHang").val()).length > 100) {
        Messages.push("Mã ngân hàng vượt quá 100 kí tự !");
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
    if ($.trim($("#txtsSoCMND").val()) != "" && $.trim($("#txtsSoCMND").val()).length > 50) {
        Messages.push("Số CMND vượt quá 50 kí tự !");
    }
    if ($.trim($("#txtsNoiCapCMND").val()) != "" && $.trim($("#txtsNoiCapCMND").val()).length > 255) {
        Messages.push("Nơi cấp CMND vượt quá 255 kí tự !");
    }

    if (Messages.length > 0) {
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

function SaveImport() {
    if ($("#tblNhaThauListImport tbody tr").length == 0) return;
    UpdateRow();

    var validateObj = ValidateDataImport();
    if (!validateObj.check) {
        if (validateObj.messages.length > 0) {
            var title = 'Lỗi nhập dữ liệu nhà thầu';
            $.ajax({
                type: "POST",
                url: "/Modal/OpenModal",
                data: { Title: title, Messages: validateObj.messages, Category: ERROR },
                success: function (data) {
                    $("#divModalConfirm").html(data);
                }
            });
        }
        return;
    }

    if (validateObj.dataArray.length == 0) {
        var title = 'Lỗi import dữ liệu nhà thầu';
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: title, Messages: ["Không có dữ liệu để import"], Category: ERROR },
            success: function (data) {
                $("#divModalConfirm").html(data);
            }
        });
        return;
    }

    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucNhaThau/SaveImport",
        data: { nhaThauList: validateObj.dataArray },
        success: function (r) {
            if (r && r.bIsComplete) {
                window.location.href = "/QLNH/DanhMucNhaThau";
            } else {
                var Title = 'Lỗi import dữ liệu nhà thầu';
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

function ValidateDataImport() {
    var returnObj = {};
    var check = true;
    var messages = [];
    var data = {};
    var dataArray = [];
    $("#tblNhaThauListImport tbody tr.correct").each(function () {
        if (!check) return;
        data = {};
        var index = $(this).data("index");
        if ($(this).find("#slbLoai" + index).length == 1) {
            if (loai == "" || loai == "0") {
                check = false;
                messages.push("Loại chưa chọn !");
                document.getElementById("slbLoai" + index).scrollIntoView();
                return;
            } else {
                data.iLoai = $(this).find("#slbLoai" + index).val();
            }
        } else {
            data.iLoai = $("<div/>").text($.trim($("#spanLoai" + index).data("maloai"))).html();
        }

        if ($("#spanMaNhaThau" + index).length > 0) {
            data.sMaNhaThau = $("<div/>").text($.trim($("#spanMaNhaThau" + index).data("manhathau"))).html();
        } else {
            if ($.trim($("#txtMaNhaThau" + index).val()).length == 0) {
                check = false;
                messages.push("Mã nhà thầu/đơn vị ủy thác chưa được nhập !");
                document.getElementById("txtMaNhaThau" + index).scrollIntoView();
                return;
            }

            if ($.trim($("#txtMaNhaThau" + index).val()).length > 100) {
                check = false;
                messages.push("Mã nhà thầu/đơn vị ủy thác vượt quá 100 kí tự!");
                document.getElementById("txtMaNhaThau" + index).scrollIntoView();
                return;
            }
            data.sMaNhaThau = $("<div/>").text($.trim($("#txtMaNhaThau" + index).val())).html();
        }

        if ($("#spanDonViUyThac" + index).length > 0) {
            data.sTenNhaThau = $("<div/>").text($.trim($("#spanDonViUyThac" + index).data("tennhathau"))).html();
        } else {
            if ($.trim($("#txtDonViUyThac" + index).val()).length == 0) {
                check = false;
                messages.push("Tên nhà thầu/đơn vị ủy thác chưa được nhập !");
                document.getElementById("txtDonViUyThac" + index).scrollIntoView();
                return;
            }

            if ($.trim($("#txtDonViUyThac" + index).val()).length > 300) {
                check = false;
                messages.push("Tên nhà thầu/đơn vị ủy thác vượt quá 300 kí tự !");
                document.getElementById("txtDonViUyThac" + index).scrollIntoView();
                return;
            }
            data.sTenNhaThau = $("<div/>").text($.trim($("#txtDonViUyThac" + index).val())).html();
        }

        if ($("#spanDiaChi" + index).length > 0) {
            data.sDiaChi = $("<div/>").text($.trim($("#spanDiaChi" + index).data("diachi"))).html();
        } else {
            if ($.trim($("#txtDiaChi" + index).val()).length > 300) {
                check = false;
                messages.push("Địa chỉ vượt quá 300 kí tự !");
                document.getElementById("txtDiaChi" + index).scrollIntoView();
                return;
            }
            data.sDiaChi = $("<div/>").text($.trim($("#txtDiaChi" + index).val())).html();
        }

        if ($("#spanDaiDien" + index).length > 0) {
            data.sDaiDien = $("<div/>").text($.trim($("#spanDaiDien" + index).data("daidien"))).html();
        } else {
            if ($.trim($("#txtDaiDien" + index).val()).length > 100) {
                check = false;
                messages.push("Đại diện vượt quá 100 kí tự!");
                document.getElementById("txtDaiDien" + index).scrollIntoView();
                return;
            }
            data.sDaiDien = $("<div/>").text($.trim($("#txtDaiDien" + index).val())).html();
        }

        if ($("#spanChucVu" + index).length > 0) {
            data.sChucVu = $("<div/>").text($.trim($("#spanChucVu" + index).data("chucvu"))).html();
        } else {
            if ($.trim($("#txtChucVu" + index).val()).length > 100) {
                check = false;
                messages.push("Chức vụ vượt quá 100 kí tự!");
                document.getElementById("txtChucVu" + index).scrollIntoView();
                return;
            }
            data.sChucVu = $("<div/>").text($.trim($("#txtChucVu" + index).val())).html();
        }

        if ($("#spanSoDienThoai" + index).length > 0) {
            data.sDienThoai = $("<div/>").text($.trim($("#spanSoDienThoai" + index).data("phone"))).html();
        } else {
            if ($.trim($("#txtSoDienThoai" + index).val()).length > 100) {
                check = false;
                messages.push("Số điện thoại vượt quá 100 kí tự!");
                document.getElementById("txtSoDienThoai" + index).scrollIntoView();
                return;
            }

            if ($.trim($("#txtSoDienThoai" + index).val()) != "" && !CheckPhoneOrFax($.trim($("#txtSoDienThoai" + index).val()))) {
                check = false;
                messages.push("Số điện thoại không hợp lệ!");
                document.getElementById("txtSoDienThoai" + index).scrollIntoView();
                return;
            }
            data.sDienThoai = $("<div/>").text($.trim($("#txtSoDienThoai" + index).val())).html();
        }

        if ($("#spanSoFax" + index).length > 0) {
            data.sFax = $("<div/>").text($.trim($("#spanSoFax" + index).data("fax"))).html();
        } else {
            if ($.trim($("#txtSoFax" + index).val()).length > 100) {
                check = false;
                messages.push("Số fax vượt quá 100 kí tự!");
                document.getElementById("txtSoFax" + index).scrollIntoView();
                return;
            }

            if ($.trim($("#txtSoFax" + index).val()) != "" && !CheckPhoneOrFax($.trim($("#txtSoFax" + index).val()))) {
                check = false;
                messages.push("Số fax không hợp lệ!");
                document.getElementById("txtSoFax" + index).scrollIntoView();
                return;
            }
            data.sFax = $("<div/>").text($.trim($("#txtSoFax" + index).val())).html();
        }

        if ($("#spanEmail" + index).length > 0) {
            data.sEmail = $("<div/>").text($.trim($("#spanEmail" + index).data("email"))).html();
        } else {
            if ($.trim($("#txtEmail" + index).val()).length > 100) {
                check = false;
                messages.push("Email vượt quá 100 kí tự!");
                document.getElementById("txtEmail" + index).scrollIntoView();
                return;
            }

            if ($.trim($("#txtEmail" + index).val()) != "" && !CheckEmail($.trim($("#txtEmail" + index).val()))) {
                check = false;
                messages.push("Email nhập không hợp lệ!");
                document.getElementById("txtEmail" + index).scrollIntoView();
                return;
            }
            data.sEmail = $("<div/>").text($.trim($("#txtEmail" + index).val())).html();
        }

        if ($("#spanWebsite" + index).length > 0) {
            data.sWebsite = $("<div/>").text($.trim($("#spanWebsite" + index).data("website"))).html();
        } else {
            if ($.trim($("#txtWebsite" + index).val()).length > 300) {
                check = false;
                messages.push("Tên website vượt quá 300 kí tự!");
                document.getElementById("txtWebsite" + index).scrollIntoView();
                return;
            }
            data.sWebsite = $("<div/>").text($.trim($("#txtWebsite" + index).val())).html();
        }

        if ($("#spanSoTaiKhoan" + index).length > 0) {
            data.sSoTaiKhoan = $("<div/>").text($.trim($("#spanSoTaiKhoan" + index).data("sotaikhoan"))).html();
        } else {
            if ($.trim($("#txtSoTaiKhoan" + index).val()).length > 100) {
                check = false;
                messages.push("Số tài khoản vượt quá 100 kí tự!");
                document.getElementById("txtSoTaiKhoan" + index).scrollIntoView();
                return;
            }

            if ($.trim($("#txtSoTaiKhoan" + index).val()) != "" && !CheckAlphanumeric($.trim($("#txtSoTaiKhoan" + index).val()))) {
                check = false;
                messages.push("Số tài khoản nhập không hợp lệ!");
                document.getElementById("txtSoTaiKhoan" + index).scrollIntoView();
                return;
            }
            data.sSoTaiKhoan = $("<div/>").text($.trim($("#txtSoTaiKhoan" + index).val())).html();
        }

        if ($("#spanNganHang" + index).length > 0) {
            data.sNganHang = $("<div/>").text($.trim($("#spanNganHang" + index).data("bank"))).html();
        } else {
            if ($.trim($("#txtNganHang" + index).val()).length > 100) {
                check = false;
                messages.push("Tên ngân hàng vượt quá 100 kí tự!");
                document.getElementById("txtNganHang" + index).scrollIntoView();
                return;
            }
            data.sNganHang = $("<div/>").text($.trim($("#txtNganHang" + index).val())).html();
        }

        if ($("#spanMaNganHang" + index).length > 0) {
            data.sMaNganHang = $("<div/>").text($.trim($("#spanMaNganHang" + index).data("bankcode"))).html();
        } else {
            if ($.trim($("#txtMaNganHang" + index).val()).length > 100) {
                check = false;
                messages.push("Mã ngân hàng vượt quá 100 kí tự!");
                document.getElementById("txtMaNganHang" + index).scrollIntoView();
                return;
            }
            data.sMaNganHang = $("<div/>").text($.trim($("#txtMaNganHang" + index).val())).html();
        }

        if ($("#spanMaSoThue" + index).length > 0) {
            data.sMaSoThue = $("<div/>").text($.trim($("#spanMaSoThue" + index).data("taxcode"))).html();
        } else {
            if ($.trim($("#txtMaSoThue" + index).val()).length > 100) {
                check = false;
                messages.push("Mã số thuế vượt quá 100 kí tự!");
                document.getElementById("txtMaSoThue" + index).scrollIntoView();
                return;
            }

            if ($.trim($("#txtMaSoThue" + index).val()) != "" && !CheckAlphanumeric($.trim($("#txtMaSoThue" + index).val()))) {
                check = false;
                messages.push("Mã số thuế nhập không hợp lệ!");
                document.getElementById("txtMaSoThue" + index).scrollIntoView();
                return;
            }
            data.sMaSoThue = $("<div/>").text($.trim($("#txtMaSoThue" + index).val())).html();
        }

        if ($("#spanNguoiLienHe" + index).length > 0) {
            data.sNguoiLienHe = $("<div/>").text($.trim($("#spanNguoiLienHe" + index).data("nguoilienhe"))).html();
        } else {
            if ($.trim($("#txtNguoiLienHe" + index).val()).length > 300) {
                check = false;
                messages.push("Tên người liên hệ vượt quá 300 kí tự!");
                document.getElementById("txtNguoiLienHe" + index).scrollIntoView();
                return;
            }
            data.sNguoiLienHe = $("<div/>").text($.trim($("#txtNguoiLienHe" + index).val())).html();
        }

        if ($("#spansdtNguoiLH" + index).length > 0) {
            data.sDienThoaiLienHe = $("<div/>").text($.trim($("#spansdtNguoiLH" + index).data("phonelienhe"))).html();
        } else {
            if ($.trim($("#txtsdtNguoiLH" + index).val()).length > 100) {
                check = false;
                messages.push("Số điện thoại người liên hệ vượt quá 100 kí tự!");
                document.getElementById("txtsdtNguoiLH" + index).scrollIntoView();
                return;
            }

            if ($.trim($("#txtsdtNguoiLH" + index).val()) != "" && !CheckPhoneOrFax($.trim($("#txtsdtNguoiLH" + index).val()))) {
                check = false;
                messages.push("Số điện thoại người liên hệ không hợp lệ!");
                document.getElementById("txtsdtNguoiLH" + index).scrollIntoView();
                return;
            }
            data.sDienThoaiLienHe = $("<div/>").text($.trim($("#txtsdtNguoiLH" + index).val())).html();
        }

        if ($("#spanSoCMND" + index).length > 0) {
            data.sSoCMND = $("<div/>").text($.trim($("#spanSoCMND" + index).data("cmnd"))).html();
        } else {
            if ($.trim($("#txtSoCMND" + index).val()).length > 100) {
                check = false;
                messages.push("Số CMND vượt quá 100 kí tự!");
                document.getElementById("txtSoCMND" + index).scrollIntoView();
                return;
            }

            if ($.trim($("#txtSoCMND" + index).val()) != "" && !CheckAlphanumeric($.trim($("#txtSoCMND" + index).val()))) {
                check = false;
                messages.push("Số CMND nhập không hợp lệ!");
                document.getElementById("txtSoCMND" + index).scrollIntoView();
                return;
            }
            data.sSoCMND = $("<div/>").text($.trim($("#txtSoCMND" + index).val())).html();
        }

        if ($("#spanNoiCap" + index).length > 0) {
            data.sNoiCapCMND = $("<div/>").text($.trim($("#spanNoiCap" + index).data("noicapcmnd"))).html();
        } else {
            if ($.trim($("#txtNoiCap" + index).val()).length > 255) {
                check = false;
                messages.push("Số CMND vượt quá 255 kí tự!");
                document.getElementById("txtNoiCap" + index).scrollIntoView();
                return;
            }
            data.sNoiCapCMND = $("<div/>").text($.trim($("#txtNoiCap" + index).val())).html();
        }

        if ($("#spanNgayCap" + index).length > 0) {
            data.dNgayCapCMND = $("<div/>").text($.trim($("#spanNgayCap" + index).data("ngaycap"))).html();
        } else {
            if ($.trim($("#txtNgayCap" + index).val()) != "" && !dateIsValid($.trim($("#txtNgayCap" + index).val()))) {
                check = false;
                messages.push("Ngày cấp CMND không hợp lệ !");
                document.getElementById("txtNgayCap" + index).scrollIntoView();
                return;
            }
            data.dNgayCapCMND = $("<div/>").text($.trim($("#txtNgayCap" + index).val())).html();
        }
        
        dataArray.push(data);

    });

    returnObj.check = check;
    returnObj.messages = messages;
    returnObj.dataArray = dataArray;
    return returnObj;
}

function UpdateRow() {
    $("#tblNhaThauListImport tbody tr.wrong").each(function () {
        var index = $(this).data("index");
        if (ValidateRowWrong(this, index)) {
            $(this).removeClass("wrong").addClass("correct");
            $(this).find(".status-icon").empty().html('<i class="fa fa-check fa-lg" style="color:green;" aria-hidden="true"></i>');
            $(this).find(".cellMessageError").empty();
        }
    });
}

function ValidateRowWrong(element, index) {
    var check = true;
    $(element).find("td.cellWrong").each(function () {
        if ($(this).find("#txtMaNhaThau" + index).length == 1
            && ($.trim($(this).find("#txtMaNhaThau" + index).val()) == "" || $.trim($(this).find("#txtMaNhaThau" + index).val()).length > 100)) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtDonViUyThac" + index).length == 1
            && ($.trim($(this).find("#txtDonViUyThac" + index).val()) == "" || $.trim($(this).find("#txtDonViUyThac" + index).val()).length > 300)) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtDiaChi" + index).length == 1 && $.trim($(this).find("#txtDiaChi" + index).val()).length > 300) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtDaiDien" + index).length == 1 && $.trim($(this).find("#txtDaiDien" + index).val()).length > 100) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtChucVu" + index).length == 1 && $.trim($(this).find("#txtChucVu" + index).val()).length > 100) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtSoDienThoai" + index).length == 1
            && (($.trim($(this).find("#txtSoDienThoai" + index).val()) != "" && !CheckPhoneOrFax($.trim($(this).find("#txtSoDienThoai" + index).val())))
                || $.trim($(this).find("#txtSoDienThoai" + index).val()).length > 100)) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtSoFax" + index).length == 1
            && (($.trim($(this).find("#txtSoFax" + index).val()) != "" && !CheckPhoneOrFax($.trim($(this).find("#txtSoFax" + index).val())))
                || $.trim($(this).find("#txtSoFax" + index).val()).length > 100)) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtEmail" + index).length == 1
            && (($.trim($(this).find("#txtEmail" + index).val()) != "" && !CheckEmail($.trim($(this).find("#txtEmail" + index).val())))
                || $.trim($(this).find("#txtEmail" + index).val()).length > 100)) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtWebsite" + index).length == 1 && $.trim($(this).find("#txtWebsite" + index).val()).length > 300) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtSoTaiKhoan" + index).length == 1
            && (($.trim($(this).find("#txtSoTaiKhoan" + index).val()) != "" && !CheckAlphanumeric($.trim($(this).find("#txtSoTaiKhoan" + index).val())))
            || ($.trim($(this).find("#txtSoTaiKhoan" + index).val()).length > 100))) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtNganHang" + index).length == 1 && $.trim($(this).find("#txtNganHang" + index).val()).length > 100) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtMaNganHang" + index).length == 1 && $.trim($(this).find("#txtMaNganHang" + index).val()).length > 100) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtMaSoThue" + index).length == 1
            && (($.trim($(this).find("#txtMaSoThue" + index).val()) != "" && !CheckAlphanumeric($.trim($(this).find("#txtMaSoThue" + index).val())))
            || ($.trim($(this).find("#txtMaSoThue" + index).val()).length > 100))) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtNguoiLienHe" + index).length == 1 && $.trim($(this).find("#txtNguoiLienHe" + index).val()).length > 300) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtsdtNguoiLH" + index).length == 1
            && (($.trim($(this).find("#txtsdtNguoiLH" + index).val()) != "" && !CheckPhoneOrFax($.trim($(this).find("#txtsdtNguoiLH" + index).val())))
            || $.trim($(this).find("#txtsdtNguoiLH" + index).val()).length > 100)) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtSoCMND" + index).length == 1
            && (($.trim($(this).find("#txtSoCMND" + index).val()) != "" && !CheckAlphanumeric($.trim($(this).find("#txtSoCMND" + index).val())))
                || ($.trim($(this).find("#txtSoCMND" + index).val()).length > 50))) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtNoiCap" + index).length == 1 && $.trim($(this).find("#txtNoiCap" + index).val()).length > 255) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtNgayCap" + index).length == 1
            && $.trim($(this).find("#txtNgayCap" + index).val()) != ""
            && !dateIsValid($.trim($(this).find("#txtNgayCap" + index).val()))) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if (check) $(this).removeClass("cellWrong");
    });
    return check;
}

