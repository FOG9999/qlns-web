var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;

function ResetChangePage(iCurrentPage = 1) {
    GetListData("", GUID_EMPTY, "", GUID_EMPTY, GUID_EMPTY, "", "", GUID_EMPTY, iCurrentPage);
}

function ToggleRow(e) {
    let tdElement = $(e);
    let trElement = tdElement.closest('tr');
    let id = tdElement.data('id');
    let index = trElement.index();
    let hasValue = tdElement.data('ishaschild');
    let sTenHopDong = $("<div/>").text($.trim($("#txtTenHopDongFilter").val())).html();
    let iDonvi = $("#iDonVi").val(); 
    let iChuongTrinh = $("#iChuongTrinh").val(); 
    let iDuAn = $("#iDuAn").val(); 
    let sSoHopDong = $("<div/>").text($.trim($("#txtSoHopDongFilter").val())).html();
    let sNgayKyHopDong = $.trim($("#txtNgayKyHopDongFilter").val()); 
    let iLoaiHopDong = $.trim($("#iLoaiHopDong").val());

    // Nếu đã lấy data thì chỉ ẩn hiên thôi, chưa có data thì lấy data.
    if (!hasValue) {
        $.ajax({
            type: "POST",
            data: { id: id, sTenHopDong: sTenHopDong, iDonvi: iDonvi, iChuongTrinh: iChuongTrinh, iDuAn: iDuAn, sSoHopDong: sSoHopDong, sNgayKyHopDong: sNgayKyHopDong, iLoaiHopDong: iLoaiHopDong },
            url: '/QLNH/QLThongTinHopDong/GetListHopDongById',
            success: function (rs) {
                if (rs != null) {
                    tdElement.data('ishaschild', true);

                    // Add row
                    $("#tbodyListHopDong tr").eq(index).after(rs.datas);
                    trElement.siblings('.child-' + id).fadeToggle();
                }
            }
        });
    } else {
        trElement.siblings('.child-' + id).fadeToggle();
    }
}

function ChangePage(iCurrentPage = 1) {
    var sTenHopDong = $("<div/>").text($.trim($("#txtTenHopDongFilter").val())).html();
    var iDonVi = $("#iDonVi").val();
    var maDonVi = $("<div/>").text($("#iDonVi").find("option:selected").data("madonvi")).html();
    var iChuongTrinh = $("#iChuongTrinh").val();
    var iDuAn = $("#iDuAn").val();
    var soHopDong = $("<div/>").text($.trim($("#txtSoHopDongFilter").val())).html();
    var ngayKyHopDong = $.trim($("#txtNgayKyHopDongFilter").val());
    var iLoaiHopDong = $("#iLoaiHopDong").val();
    GetListData(sTenHopDong, iDonVi, maDonVi, iChuongTrinh, iDuAn, soHopDong, ngayKyHopDong, iLoaiHopDong, iCurrentPage);
}

function GetListData(sTenHopDong, iDonVi, maDonVi, iChuongTrinh, iDuAn, soHopDong, ngayKyHopDong, iLoaiHopDong, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { sTenHopDong: sTenHopDong, iDonVi: iDonVi, maDonVi: maDonVi, iChuongTrinh: iChuongTrinh, iDuAn: iDuAn, soHopDong: soHopDong, ngayKyHopDong: ngayKyHopDong, iLoaiHopDong: iLoaiHopDong, _paging: _paging },
        success: function (data) {
            $("#lstDataView").html(data);

            $("#txtTenHopDongFilter").val($("<div/>").html(sTenHopDong).text());
            $("#iDonVi").val(iDonVi);
            $("#iChuongTrinh").val(iChuongTrinh);
            $("#iDuAn").val(iDuAn);
            $("#txtSoHopDongFilter").val($("<div/>").html(soHopDong).text());
            $("#txtNgayKyHopDongFilter").val($("<div/>").html(ngayKyHopDong).text());
            $("#iLoaiHopDong").val(iLoaiHopDong);
        }
    });
}

function OpenContractInfo(id, isDieuChinh) {
    if (id == undefined || id == null || id == GUID_EMPTY) {
        window.location.href = "/QLNH/QLThongTinHopDong/GetContractInfo";
    } else {
        if (isDieuChinh) {
            window.location.href = "/QLNH/QLThongTinHopDong/GetContractInfo?id=" + id + "&isDieuChinh=" + isDieuChinh;
        } else {
            window.location.href = "/QLNH/QLThongTinHopDong/GetContractInfo?id=" + id + "&isDieuChinh=" + isDieuChinh;
        }
    }
}

function OpenContractInfoDetail(id) {
    window.location.href = "/QLNH/QLThongTinHopDong/GetContractInfoDetail/" + id;
}

function Xoa(id) {
    var Title = 'Xác nhận xóa thông tin hợp đồng';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn xóa?');
    var FunctionName = "Delete('" + id + "')";
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
        success: function (data) {
            $("#divModalConfirm").empty().html(data);
        }
    });
}

function Delete(id) {
    $.ajax({
        type: "POST",
        url: "/QLNH/QLThongTinHopDong/Xoa",
        data: { id: id },
        success: function (data) {
            if (data) {
                if (data.bIsComplete) {
                    ChangePage();
                } else {
                    if (data.sMessError != "") {
                        var Title = 'Lỗi xóa thông tin hợp đồng';
                        $.ajax({
                            type: "POST",
                            url: "/Modal/OpenModal",
                            data: { Title: Title, Messages: [data.sMessError], Category: ERROR },
                            success: function (res) {
                                $("#divModalConfirm").html(res);
                            }
                        });
                    }
                }
            }
        }
    });
}

function ChangeSelectDonVi(element) {
    var iDonVi;
    var maDonVi;
    if (element && $(element).length > 0) {
        iDonVi = $(element).val();
        maDonVi = $("<div/>").text($(element).find("option:selected").data("madonvi")).html();
    } else {
        iDonVi = $("#iDonVi").val();
        maDonVi = $("<div/>").text($("#iDonVi").find("option:selected").data("madonvi")).html();
    }
    $.ajax({
        type: "POST",
        url: "/QLNH/QLThongTinHopDong/GetChuongTrinhTheoDonVi",
        data: { iDonVi: iDonVi, maDonVi: maDonVi },
        success: function (data) {
            if (data) {
                if (element && $(element).length > 0) {
                    var index = $(element).data("index");
                    $("#slbChuongTrinh" + index).empty().html(data.htmlCT);
                    $("#slbChuongTrinh" + index).select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });
                    $("#slbDuAn" + index).empty().html(data.htmlDA);
                    $("#slbDuAn" + index).select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });
                } else {
                    $("#iChuongTrinh").empty().html(data.htmlCT);
                    $("#iDuAn").empty().html(data.htmlDA);
                    $("#iChuongTrinh").select2({ dropdownAutoWidth: true, matcher: FilterInComboBox });
                    $("#iDuAn").select2({ dropdownAutoWidth: true, matcher: FilterInComboBox });
                }
            }

        }
    });
}

function ChangeSelectChuongTrinh(element) {
    var iChuongTrinh;
    if (element && $(element).length > 0) {
        iChuongTrinh = $(element).val();
    } else {
        iChuongTrinh = $("#iChuongTrinh").val();
    }
    $.ajax({
        type: "POST",
        url: "/QLNH/QLThongTinHopDong/GetDuAnTheoChuongTrinh",
        data: { iChuongTrinh: iChuongTrinh },
        success: function (data) {
            if (data) {
                if (element && $(element).length > 0) {
                    var index = $(element).data("index");
                    $("#slbDuAn" + index).empty().html(data);
                    $("#slbDuAn" + index).select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });
                } else {
                    $("#iDuAn").empty().html(data);
                    $("#iDuAn").select2({ dropdownAutoWidth: true, matcher: FilterInComboBox });
                }
            }
        }
    });
}

function ChangeBQPSelectImport(element) {
    var value = $(element).val();
    var index = $(element).data('index');
    $.ajax({
        type: "POST",
        url: "/QLNH/QLThongTinHopDong/GetChuongTrinhTheoQDTongThe",
        data: { id: value },
        success: function (data) {
            if (data) {
                $("#slbChuongTrinh" + index).empty().html(data.htmlChuongTrinh);
                $("#slbBQuanLy" + index).empty().html(data.htmlQuanLy);
                $("#slbDonVi" + index).empty().html(data.htmlDonVi);
                $("#slbDuAn").empty().html(data.htmlDA);
                $("#slbChuongTrinh").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
                $("#slbBQuanLy").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
                $("#slbDonVi").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
                $("#slbDuAn").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
            }
        }
    });
}

function ChangeChuongTrinhSelectImport(element) {
    var value = $(element).val();
    var index = $(element).data('index');
    var idBqp = $("#slbKHTongTheBQP" + index).val();
    $.ajax({
        type: "POST",
        url: "/QLNH/QLThongTinHopDong/GetBQuanLyTheoChuongTrinh",
        data: { id: value, idBqp: idBqp },
        success: function (data) {
            if (data) {
                $("#slbBQuanLy" + index).empty().html(data.htmlQuanLy);
                $("#slbDonVi" + index).empty().html(data.htmlDonVi);
                $("#slbDuAn").empty().html(data.htmlDA);
                $("#slbBQuanLy").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
                $("#slbDonVi").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
                $("#slbDuAn").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
            }
        }
    });
}

function ChangeBQuanLySelectImport(element) {
    var value = $(element).val();
    var index = $(element).data('index');
    var idChuongTrinh = $("#slbChuongTrinh" + index).val();
    var idBqp = $("#slbKHTongTheBQP" + index).val();
    $.ajax({
        type: "POST",
        url: "/QLNH/QLThongTinHopDong/GetDonViTheoBQuanLy",
        data: { id: value, idBqp: idBqp, idChuongTrinh: idChuongTrinh },
        success: function (data) {
            if (data) {
                $("#slbDonVi" + index).empty().html(data.htmlDonVi);
                $("#slbDuAn").empty().html(data.htmlDA);
                $("#slbDonVi").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
                $("#slbDuAn").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
            }
        }
    });
}

function ChangeDonViImport(element) {
    var id = $(element).val();
    var index = $(element).data('index');
    var idBQuanLy = $("#slbBQuanLy" + index).val();
    var idChuongTrinh = $("#slbChuongTrinh" + index).val();
    var idBqp = $("#slbKHTongTheBQP" + index).val();
    $.ajax({
        type: "POST",
        url: "/QLNH/QLThongTinHopDong/GetDuAnTheoDonVi",
        data: { id: id, idBqp: idBqp, idChuongTrinh: idChuongTrinh, idBQuanLy: idBQuanLy },
        success: function (data) {
            if (data) {
                $("#slbDuAn" + index).empty().html(data.htmlDA);
                $("#slbDuAn" + index).select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
            }
        }
    });
}

function ChangeSelectLoai(element) {
    var index = $(element).data("index");
    var loaiVal = $(element).val();
    if (loaiVal == 1 || loaiVal == 3) {
        $(element).closest("tr").find(".divDuAn").removeClass("hidden");
    } else {
        $("#slbDuAn" + index).val(GUID_EMPTY);
        $(element).closest("tr").find(".divDuAn").addClass("hidden");
    }
}

function ImportThongTinHopDong() {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/QLThongTinHopDong/OpenModalImport",
        success: function (data) {
            $("#contentModalTTHopDong").empty().html(data);
            $("#modalHopDongLabel").empty().html('Import thông tin hợp đồng');
        }
    });
}

function ChangeFile(element) {
    var filePath = $(element).val();
    var fileNameArr = filePath.split("\\");
    $("#lblUpload span").empty().html(fileNameArr[fileNameArr.length - 1]);
}

function RefreshImport() {
    $("#inpFileUpload").val("");
    $("#lblUpload span").empty().html("Lựa chọn file excel");
    $("#tblHopDongListImport tbody").empty();
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
        url: "/QLNH/QLThongTinHopDong/LoadDataExcel",
        data: formData,
        contentType: false,
        processData: false,
        cache: false,
        async: false,
        success: function (r) {
            if (r.bIsComplete) {
                $("#tblHopDongListImport tbody").empty().html(r.data);
                $(".selectBQP").select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });
                $(".selectChuongTrinh").select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });
                $(".selectBQuanLy").select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });
                $(".selectDonVi").select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });
                $(".selectLoai").select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });
                $(".selectDuAn").select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });
                $(".selectLoaiHopDong").select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });
                $(".selectNhaThau").select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });

                $(".inputThoiGianThucHien").keydown(function (event) {
                    ValidateInputKeydown(event, this, 1);
                }).blur(function (event) {
                    ValidateInputFocusOut(event, this, 1);
                });

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

function DownloadFileTemplate() {
    var url = "/QLNH/QLThongTinHopDong/DownloadTemplateImport";
    var arrLink = [];
    arrLink.push(url);
    openLinks(arrLink);
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

function ConfirmRemoveRowImport(index) {
    var Title = 'Xác nhận xóa dòng dữ liệu thông tin hợp đồng';
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

function SaveImport() {
    if ($("#tblHopDongListImport tbody tr").length == 0) return;
    UpdateRow();

    var validateObj = ValidateDataImport();
    if (!validateObj.check) {
        if (validateObj.messages.length > 0) {
            var title = 'Lỗi nhập dữ liệu thông tin hợp đồng';
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
        var title = 'Lỗi import dữ liệu thông tin hợp đồng';
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
        url: "/QLNH/QLThongTinHopDong/SaveImport",
        data: { contractList: validateObj.dataArray },
        success: function (r) {
            if (r && r.bIsComplete) {
                window.location.href = "/QLNH/QLThongTinHopDong";
            } else {
                var Title = 'Lỗi import dữ liệu thông tin hợp đồng';
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
    $("#tblHopDongListImport tbody tr.correct").each(function () {
        if (!check) return;
        data = {};
        var index = $(this).data("index");
        if ($("#spanTenHopDong" + index).length > 0) {
            data.sTenHopDong = $("#spanTenHopDong" + index).html();
        } else {
            if ($.trim($("#txtTenHopDong" + index).val()).length > 300) {
                check = false;
                messages.push("Tên hợp đồng vượt quá 300 kí tự !");
                document.getElementById("txtTenHopDong" + index).scrollIntoView();
                return;
            }
            data.sTenHopDong = $("<div/>").text($.trim($("#txtTenHopDong" + index).val())).html();
        }
        if ($("#spanSoHopDong" + index).length > 0) {
            data.sSoHopDong = $("#spanSoHopDong" + index).html();
        } else {
            if ($.trim($("#txtSoHopDong" + index).val()).length == 0) {
                check = false;
                messages.push("Số hợp đồng chưa được nhập !");
                document.getElementById("txtSoHopDong" + index).scrollIntoView();
                return;
            }

            if ($.trim($("#txtSoHopDong" + index).val()).length > 100) {
                check = false;
                messages.push("Số hợp đồng vượt quá 100 kí tự !");
                document.getElementById("txtSoHopDong" + index).scrollIntoView();
                return;
            }
            data.sSoHopDong = $("<div/>").text($.trim($("#txtSoHopDong" + index).val())).html();
        }
        if ($("#spanNgayKiHopDong" + index).length > 0) {
            data.dNgayHopDong = $("#spanNgayKiHopDong" + index).html();
        } else {
            if ($.trim($("#txtNgayKiHopDong" + index).val()) != "" && !dateIsValid($.trim($("#txtNgayKiHopDong" + index).val()))) {
                check = false;
                messages.push("Ngày kí hợp đồng không hợp lệ !");
                document.getElementById("txtNgayKiHopDong" + index).scrollIntoView();
                return;
            }
            data.dNgayHopDong = $("<div/>").text($.trim($("#txtNgayKiHopDong" + index).val())).html();
        }
        if ($("#spanMaLoaiHopDong" + index).length > 0) {
            data.iID_LoaiHopDongID = $("#spanMaLoaiHopDong" + index).data("id");
        } else {
            data.iID_LoaiHopDongID = $("#slbLoaiHopDong" + index).val() == GUID_EMPTY ? null : $("#slbLoaiHopDong" + index).val();
        }
        if ($("#spanMaNhaThau" + index).length > 0) {
            data.iID_NhaThauThucHienID = $("#spanMaNhaThau" + index).data("id");
        } else {
            data.iID_NhaThauThucHienID = $("#slbNhaThau" + index).val() == GUID_EMPTY ? null : $("#slbNhaThau" + index).val();
        }
        var startDateThucHien = "", endDateThucHien = "";
        if ($("#spanThoiGianThucHienTu" + index).length > 0) {
            data.dKhoiCongDuKien = $("#spanThoiGianThucHienTu" + index).html();
            startDateThucHien = $("#spanThoiGianThucHienTu" + index).html();
        } else {
            if ($.trim($("#txtThoiGianThucHienTu" + index).val()) != "" && !dateIsValid($.trim($("#txtThoiGianThucHienTu" + index).val()))) {
                check = false;
                messages.push("Thời gian thực hiện từ không hợp lệ !");
                document.getElementById("txtThoiGianThucHienTu" + index).scrollIntoView();
                return;
            }
            data.dKhoiCongDuKien = $("<div/>").text($.trim($("#txtThoiGianThucHienTu" + index).val())).html();
            startDateThucHien = $.trim($("#txtThoiGianThucHienTu" + index).val());
        }
        if ($("#spanThoiGianThucHienDen" + index).length > 0) {
            data.dKetThucDuKien = $("#spanThoiGianThucHienDen" + index).html();
            endDateThucHien = $("#spanThoiGianThucHienDen" + index).html();
        } else {
            if ($.trim($("#txtThoiGianThucHienDen" + index).val()) != "" && !dateIsValid($.trim($("#txtThoiGianThucHienDen" + index).val()))) {
                check = false;
                messages.push("Thời gian thực hiện đến không hợp lệ !");
                document.getElementById("txtThoiGianThucHienDen" + index).scrollIntoView();
                return;
            }
            data.dKetThucDuKien = $("<div/>").text($.trim($("#txtThoiGianThucHienDen" + index).val())).html();
            endDateThucHien = $.trim($("#txtThoiGianThucHienDen" + index).val());
        }
        if (!CompareDatetime(startDateThucHien, endDateThucHien)) {
            check = false;
            messages.push("Thời gian bắt đầu thực hiện hợp đồng phải nhỏ hơn thời gian kết thúc thực hiện hợp đồng !");
            if ($("#txtThoiGianThucHienTu" + index).length > 0) {
                document.getElementById("txtThoiGianThucHienTu" + index).scrollIntoView();
            } else {
                document.getElementById("txtThoiGianThucHienDen" + index).scrollIntoView();
            }
            return;
        }

        if ($("#spanThoiGianThucHien" + index).length > 0) {
            data.iThoiGianThucHien = $("#spanThoiGianThucHien" + index).html();
        } else {
            if (!/^\d*$/.test($.trim($("#txtThoiGianThucHien" + index).val()))) {
                check = false;
                messages.push("Thời gian thực hiện (ngày) không hợp lệ !");
                document.getElementById("txtThoiGianThucHien" + index).scrollIntoView();
                return;
            }
            data.iThoiGianThucHien = $("<div/>").text($.trim($("#txtThoiGianThucHien" + index).val())).html();
        }
        if ($(this).find("#slbKHTongTheBQP" + index).val() == "" || $(this).find("#slbKHTongTheBQP" + index).val() == GUID_EMPTY) {
            check = false;
            messages.push("Số quyết định tổng thể TTCP chưa chọn !");
            document.getElementById("slbKHTongTheBQP" + index).scrollIntoView();
            return;
        } else {
            data.iID_NH_KHChiTietBQP = $(this).find("#slbKHTongTheBQP" + index).val();
        }
        if ($(this).find("#slbChuongTrinh" + index).val() == "" || $(this).find("#slbChuongTrinh" + index).val() == GUID_EMPTY) {
            check = false;
            messages.push("Tên chương trình chưa chọn !");
            document.getElementById("slbChuongTrinh" + index).scrollIntoView();
            return;
        } else {
            data.iID_KHCTBQP_ChuongTrinhID = $(this).find("#slbChuongTrinh" + index).val();
        }
        if ($(this).find("#slbBQuanLy" + index).val() == "" || $(this).find("#slbBQuanLy" + index).val() == GUID_EMPTY) {
            check = false;
            messages.push("B quản lý chưa chọn !");
            document.getElementById("slbBQuanLy" + index).scrollIntoView();
            return;
        } else {
            data.iID_BQuanLyID = $(this).find("#slbBQuanLy" + index).val();
        }
        if ($(this).find("#slbDonVi" + index).val() == "" || $(this).find("#slbDonVi" + index).val() == GUID_EMPTY) {
            check = false;
            messages.push("Đơn vị quản lý chưa chọn !");
            document.getElementById("slbDonVi" + index).scrollIntoView();
            return;
        } else {
            data.iID_DonViID = $(this).find("#slbDonVi" + index).val();
            data.iID_MaDonVi = $("<div/>").text($(this).find("#slbDonVi" + index).find("option:selected").data("madonvi")).html();
        }
        if ($(this).find("#slbLoai" + index).val() == "" || $(this).find("#slbLoai" + index).val() == 0) {
            check = false;
            messages.push("Loại chưa chọn !");
            document.getElementById("slbLoai" + index).scrollIntoView();
            return;
        } else {
            data.iPhanLoai = $(this).find("#slbLoai" + index).val();
        }
        if ($(this).find("#slbLoai" + index).val() == 1 || $(this).find("#slbLoai" + index).val() == 3) {
            if ($(this).find("#slbDuAn" + index).val() == "" || $(this).find("#slbDuAn" + index).val() == GUID_EMPTY) {
                check = false;
                messages.push("Tên dự án chưa chọn !");
                document.getElementById("slbDuAn" + index).scrollIntoView();
                return;
            } else {
                data.iID_DuAnID = $(this).find("#slbDuAn" + index).val();
                //data.iID_BQuanLyID = $(this).find("#slbDuAn" + index).find("option:selected").data("bql");
            }
        } else {
            data.iID_DuAnID = null;
            //data.iID_BQuanLyID = null;
        }
        data.iLanDieuChinh = 0;

        dataArray.push(data);
    });

    returnObj.check = check;
    returnObj.messages = messages;
    returnObj.dataArray = dataArray;
    return returnObj;
}

function UpdateRow() {
    $("#tblHopDongListImport tbody tr.wrong").each(function () {
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
        if ($(this).find("#txtTenHopDong" + index).length == 1 && $.trim($(this).find("#txtTenHopDong" + index).val()).length > 300) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtSoHopDong" + index).length == 1
            && ($.trim($(this).find("#txtSoHopDong" + index).val()) == "" || $.trim($(this).find("#txtSoHopDong" + index).val()).length > 100)) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtNgayKiHopDong" + index).length == 1
            && $.trim($(this).find("#txtNgayKiHopDong" + index).val()) != ""
            && !dateIsValid($.trim($(this).find("#txtNgayKiHopDong" + index).val()))) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtThoiGianThucHienTu" + index).length == 1
            && $.trim($(this).find("#txtThoiGianThucHienTu" + index).val()) != ""
            && !dateIsValid($.trim($(this).find("#txtThoiGianThucHienTu" + index).val()))) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtThoiGianThucHienDen" + index).length == 1
            && $.trim($(this).find("#txtThoiGianThucHienDen" + index).val()) != ""
            && !dateIsValid($.trim($(this).find("#txtThoiGianThucHienDen" + index).val()))) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtThoiGianThucHien" + index).length == 1 && !/^\d*$/.test($.trim($(this).find("#txtThoiGianThucHien" + index).val()))) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if (check) $(this).removeClass("cellWrong");
    });
    return check;
}
