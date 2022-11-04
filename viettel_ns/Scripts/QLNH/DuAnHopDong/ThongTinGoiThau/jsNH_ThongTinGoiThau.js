var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;

function ResetChangePage(iCurrentPage = 1) {
    GetListData("", GUID_EMPTY, "", GUID_EMPTY, GUID_EMPTY, 0, "", iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var sTenGoiThau = $("<div/>").text($.trim($("#txtTenGoiThauFilter").val())).html();
    var iDonVi = $("#iDonVi").val();
    var maDonVi = $("<div/>").text($("#iDonVi").find("option:selected").data("madonvi")).html();
    var iChuongTrinh = $("#iChuongTrinh").val();
    var iDuAn = $("#iDuAn").val();
    var iLoai = $("#iLoai").val();
    var iThoiGianThucHien = $("<div/>").text($.trim($("#txtThoiGianThucHienFilter").val())).html();
    GetListData(sTenGoiThau, iDonVi, maDonVi, iChuongTrinh, iDuAn, iLoai, iThoiGianThucHien, iCurrentPage);
}

function ToogleRow(e) {
    let tdElement = $(e);
    let trElement = tdElement.closest('tr');
    let id = tdElement.data('id');
    let index = trElement.index();
    let hasValue = tdElement.data('ishaschild');
    let sTenHopDong = $.trim($("#txtTenHopDongFilter").val());
    let iDonvi = $("#iDonVi").val();
    let iChuongTrinh = $("#iChuongTrinh").val();
    let iDuAn = $("#iDuAn").val();
    let iLoai = $("#iLoai").val(); 
    let iThoiGianThucHien = $("#txtThoiGianThucHienFilter").val();

    // Nếu đã lấy data thì chỉ ẩn hiên thôi, chưa có data thì lấy data.
    if (!hasValue) {
        $.ajax({
            type: "POST",
            data: { id: id, sTenHopDong: sTenHopDong, iDonvi: iDonvi, iChuongTrinh: iChuongTrinh, iDuAn: iDuAn, iLoai: iLoai, iThoiGianThucHien: iThoiGianThucHien },
            url: '/QLNH/ThongTinGoiThau/GetListGoiThauById',
            success: function (rs) {
                if (rs != null) {
                    tdElement.data('ishaschild', true);

                    // Add row
                    $("#tbodyListGoiThau tr").eq(index).after(rs.datas);
                    trElement.siblings('.child-' + id).fadeToggle();
                }
            }
        });
    } else {
        trElement.siblings('.child-' + id).fadeToggle();
    }
}

function GetListData(sTenGoiThau, iDonVi, maDonVi, iChuongTrinh, iDuAn, iLoai, iThoiGianThucHien, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { sTenGoiThau: sTenGoiThau, iDonVi: iDonVi, maDonVi: maDonVi, iChuongTrinh: iChuongTrinh, iDuAn: iDuAn, iLoai: iLoai, iThoiGianThucHien: iThoiGianThucHien, _paging: _paging },
        success: function (data) {
            $("#lstDataView").html(data);

            $("#txtTenGoiThauFilter").val($("<div/>").html(sTenGoiThau).text());
            $("#iDonVi").val(iDonVi);
            $("#iChuongTrinh").val(iChuongTrinh);
            $("#iDuAn").val(iDuAn);
            $("#iLoai").val(iLoai);
            $("#txtThoiGianThucHienFilter").val($("<div/>").html(iThoiGianThucHien).text());
        }
    });
}

function OpenPackageInfo(id, isDieuChinh) {
    if (id == undefined || id == null || id == GUID_EMPTY) {
        window.location.href = "/QLNH/ThongTinGoiThau/GetPackageInfo";
    } else {
        if (isDieuChinh) {
            window.location.href = "/QLNH/ThongTinGoiThau/GetPackageInfo?id=" + id + "&isDieuChinh=" + isDieuChinh;
        } else {
            window.location.href = "/QLNH/ThongTinGoiThau/GetPackageInfo?id=" + id + "&isDieuChinh=" + isDieuChinh;
        }
    }
}

function OpenPackageInfoDetail(id) {
    window.location.href = "/QLNH/ThongTinGoiThau/GetPackageInfoDetail/" + id;
}

function Xoa(id) {
    var Title = 'Xác nhận xóa thông tin gói thầu';
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
        url: "/QLNH/ThongTinGoiThau/Xoa",
        data: { id: id },
        success: function (data) {
            if (data) {
                if (data.bIsComplete) {
                    ChangePage();
                } else {
                    if (data.sMessError != "") {
                        var Title = 'Lỗi xóa thông tin gói thầu';
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
        url: "/QLNH/ThongTinGoiThau/GetChuongTrinhTheoDonVi",
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
        url: "/QLNH/ThongTinGoiThau/GetDuAnTheoChuongTrinh",
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

function ChangeSelectLoai(element) {
    var index = $(element).data("index");
    var loaiVal = $(element).val();

    $.ajax({
        type: "POST",
        url: "/QLNH/ThongTinGoiThau/GetNhaThauTheoLoai",
        data: { iLoai: loaiVal },
        success: function (data) {
            if (data) {
                $("#slbNhaThau" + index).empty().html(data);
                $("#slbNhaThau" + index).select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });
            }
        }
    });

    if (loaiVal == 1 || loaiVal == 3) {
        $(element).closest("tr").find(".divDuAn").removeClass("hidden");
    } else {
        $("#slbDuAn" + index).val(GUID_EMPTY);
        $("#slbDuAn" + index).select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });
        $(element).closest("tr").find(".divDuAn").addClass("hidden");
    }

    switch (loaiVal) {
        case "1":
        case "2":
            $(".divLoaiTrongNuoc" + index).show();
            break;
        case "3":
        case "4":
            $(".divLoaiTrongNuoc" + index).hide();
            $("#slbHinhThucCNT" + index).val(GUID_EMPTY).trigger('change');
            $("#slbPhuongThucCNT" + index).val(GUID_EMPTY).trigger('change');
            $("#slbLoaiHopDong" + index).val(GUID_EMPTY).trigger('change');
            break;
        default:
            $(".divLoaiTrongNuoc" + index).hide();
            $("#slbHinhThucCNT" + index).val(GUID_EMPTY).trigger('change');
            $("#slbPhuongThucCNT" + index).val(GUID_EMPTY).trigger('change');
            $("#slbLoaiHopDong" + index).val(GUID_EMPTY).trigger('change');
            break;
    }
}

function ImportThongTinGoiThau() {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/ThongTinGoiThau/OpenModalImport",
        success: function (data) {
            $("#contentModalTTGoiThau").empty().html(data);
            $("#modalGoiThauLabel").empty().html('Import thông tin gói thầu');
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
    $("#tblGoiThauListImport tbody").empty();
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
        url: "/QLNH/ThongTinGoiThau/LoadDataExcel",
        data: formData,
        contentType: false,
        processData: false,
        cache: false,
        async: false,
        success: function (r) {
            if (r.bIsComplete) {
                $("#tblGoiThauListImport tbody").empty().html(r.data);
                $(".selectDonVi").select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });
                $(".selectChuongTrinh").select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });
                $(".selectLoai").select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });
                $(".selectDuAn").select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });
                $(".selectLoaiHopDong").select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });
                $(".selectNhaThau").select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });
                $(".selectTienTe").select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });
                $(".selectHinhThucCNT").select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });
                $(".selectPhuongThucCNT").select2({ width: '100%', dropdownAutoWidth: true, matcher: FilterInComboBox });

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
    var url = "/QLNH/ThongTinGoiThau/DownloadTemplateImport";
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
    var Title = 'Xác nhận xóa dòng dữ liệu thông tin gói thầu';
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
    if ($("#tblGoiThauListImport tbody tr").length == 0) return;
    UpdateRow();

    var validateObj = ValidateDataImport();
    if (!validateObj.check) {
        if (validateObj.messages.length > 0) {
            var title = 'Lỗi nhập dữ liệu thông tin gói thầu';
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
        var title = 'Lỗi import dữ liệu thông tin gói thầu';
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
        url: "/QLNH/ThongTinGoiThau/SaveImport",
        data: { packageList: validateObj.dataArray },
        success: function (r) {
            if (r && r.bIsComplete) {
                window.location.href = "/QLNH/ThongTinGoiThau";
            } else {
                var Title = 'Lỗi import dữ liệu thông tin gói thầu';
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
    $("#tblGoiThauListImport tbody tr.correct").each(function () {
        if (!check) return;
        data = {};
        var index = $(this).data("index");
        if ($("#spanTenGoiThau" + index).length > 0) {
            data.sTenGoiThau = $("#spanTenGoiThau" + index).html();
        } else {
            if ($.trim($("#txtTenGoiThau" + index).val()).length == 0) {
                check = false;
                messages.push("Tên gói thầu chưa được nhập !");
                document.getElementById("txtTenGoiThau" + index).scrollIntoView();
                return;
            }
            if ($.trim($("#txtTenGoiThau" + index).val()).length > 300) {
                check = false;
                messages.push("Tên gói thầu vượt quá 300 kí tự !");
                document.getElementById("txtTenGoiThau" + index).scrollIntoView();
                return;
            }
            data.sTenGoiThau = $("<div/>").text($.trim($("#txtTenGoiThau" + index).val())).html();
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
        data.iID_NhaThauThucHienID = $("#spanMaNhaThau" + index).length > 0 ? $("#spanMaNhaThau" + index).data("id") : ($("#slbNhaThau" + index).val() == GUID_EMPTY ? null : $("#slbNhaThau" + index).val());

        var loai = "";
        if ($(this).find("#slbLoai" + index).length == 1) {
            loai = $(this).find("#slbLoai" + index).val();
            if (loai == "" || loai == "0") {
                check = false;
                messages.push("Loại chưa chọn !");
                document.getElementById("slbLoai" + index).scrollIntoView();
                return;
            } else {
                data.iPhanLoai = $(this).find("#slbLoai" + index).val();
            }
        } else {
            loai = $("<div/>").text($.trim($("#spanLoai" + index).data("maloai"))).html();
            data.iPhanLoai = loai;
        }

        if (["1", "2", "3", "4"].indexOf(loai) > -1) {
            if ($("#txtSoQuyetDinh1" + index).length == 1) {
                if ($.trim($("#txtSoQuyetDinh1" + index).val()).length > 100) {
                    check = false;
                    messages.push("Số quyết định Kế hoạch lựa chọn nhà thầu/ Phương án nhập khẩu vượt quá 100 kí tự !");
                    document.getElementById("txtSoQuyetDinh1" + index).scrollIntoView();
                    return;
                }
            }
            if ($("#txtNgayQuyetDinh1" + index).length == 1) {
                if ($.trim($("#txtNgayQuyetDinh1" + index).val()) != "" && !dateIsValid($.trim($("#txtNgayQuyetDinh1" + index).val()))) {
                    check = false;
                    messages.push("Ngày quyết định Kế hoạch lựa chọn nhà thầu/ Phương án nhập khẩu không hợp lệ !");
                    document.getElementById("txtNgayQuyetDinh1" + index).scrollIntoView();
                    return;
                }
            }
            if ($("#txtSoQuyetDinh2" + index).length == 1) {
                if ($.trim($("#txtSoQuyetDinh2" + index).val()).length > 100) {
                    check = false;
                    messages.push("Số quyết định Kết quả lựa chọn nhà thầu/ Kết quả đàm phán vượt quá 100 kí tự !");
                    document.getElementById("txtSoQuyetDinh2" + index).scrollIntoView();
                    return;
                }
            }
            if ($("#txtNgayQuyetDinh2" + index).length == 1) {
                if ($.trim($("#txtNgayQuyetDinh2" + index).val()) != "" && !dateIsValid($.trim($("#txtNgayQuyetDinh2" + index).val()))) {
                    check = false;
                    messages.push("Ngày quyết định Kết quả lựa chọn nhà thầu/ Kết quả đàm phán không hợp lệ !");
                    document.getElementById("txtNgayQuyetDinh2" + index).scrollIntoView();
                    return;
                }
            }
            if (loai == "1" || loai == "2") {
                data.sSoKeHoachLCNT = $("#spanSoQuyetDinh1" + index).length > 0 ? $("<div/>").text($.trim($("#spanSoQuyetDinh1" + index).data("content"))).html() : $("<div/>").text($.trim($("#txtSoQuyetDinh1" + index).val())).html();
                data.sSoKetQuaLCNT = $("#spanSoQuyetDinh2" + index).length > 0 ? $("<div/>").text($.trim($("#spanSoQuyetDinh2" + index).data("content"))).html() : $("<div/>").text($.trim($("#txtSoQuyetDinh2" + index).val())).html();
                data.dNgayKeHoachLCNT = $("#spanNgayQuyetDinh1" + index).length > 0 ? $("<div/>").text($.trim($("#spanNgayQuyetDinh1" + index).data("content"))).html() : $("<div/>").text($.trim($("#txtNgayQuyetDinh1" + index).val())).html();
                data.dNgayKetQuaLCNT = $("#spanNgayQuyetDinh2" + index).length > 0 ? $("<div/>").text($.trim($("#spanNgayQuyetDinh2" + index).data("content"))).html() : $("<div/>").text($.trim($("#txtNgayQuyetDinh2" + index).val())).html();
                data.iID_HinhThucChonNhaThauID = $("#spanMaHinhThucCNT" + index).length > 0 ? $("#spanMaHinhThucCNT" + index).data("id") : ($("#slbHinhThucCNT" + index).val() == GUID_EMPTY ? null : $("#slbHinhThucCNT" + index).val());
                data.iID_PhuongThucChonNhaThauID = $("#spanMaPhuongThucCNT" + index).length > 0 ? $("#spanMaPhuongThucCNT" + index).data("id") : ($("#slbPhuongThucCNT" + index).val() == GUID_EMPTY ? null : $("#slbPhuongThucCNT" + index).val());
                data.iID_LoaiHopDongID = $("#spanMaLoaiHopDong" + index).length > 0 ? $("#spanMaLoaiHopDong" + index).data("id") : ($("#slbLoaiHopDong" + index).val() == GUID_EMPTY ? null : $("#slbLoaiHopDong" + index).val());
            } else if (loai == "3" || loai == "4") {
                data.sSoPANK = $("#spanSoQuyetDinh1" + index).length > 0 ? $("<div/>").text($.trim($("#spanSoQuyetDinh1" + index).data("content"))).html() : $("<div/>").text($.trim($("#txtSoQuyetDinh1" + index).val())).html();
                data.sSoKetQuaDamPhan = $("#spanSoQuyetDinh2" + index).length > 0 ? $("<div/>").text($.trim($("#spanSoQuyetDinh2" + index).data("content"))).html() : $("<div/>").text($.trim($("#txtSoQuyetDinh2" + index).val())).html();
                data.dNgayPANK = $("#spanNgayQuyetDinh1" + index).length > 0 ? $("<div/>").text($.trim($("#spanNgayQuyetDinh1" + index).data("content"))).html() : $("<div/>").text($.trim($("#txtNgayQuyetDinh1" + index).val())).html();
                data.dNgayKetQuaDamPhan = $("#spanNgayQuyetDinh2" + index).length > 0 ? $("<div/>").text($.trim($("#spanNgayQuyetDinh2" + index).data("content"))).html() : $("<div/>").text($.trim($("#txtNgayQuyetDinh2" + index).val())).html();
            }
        }

        data.sThanhToanBang = $("#spanMaTienTe" + index).length > 0 ? $("#spanMaTienTe" + index).data("matien") : $("#slbTienTe" + index).val();

        if ($(this).find("#slbDonVi" + index).val() == "" || $(this).find("#slbDonVi" + index).val() == GUID_EMPTY) {
            check = false;
            messages.push("Đơn vị quản lý chưa chọn !");
            document.getElementById("slbDonVi" + index).scrollIntoView();
            return;
        } else {
            data.iID_DonViID = $(this).find("#slbDonVi" + index).val();
            data.iID_MaDonVi = $(this).find("#slbDonVi" + index).find("option:selected").data("madonvi");
        }
        if ($(this).find("#slbChuongTrinh" + index).val() == "" || $(this).find("#slbChuongTrinh" + index).val() == GUID_EMPTY) {
            check = false;
            messages.push("Tên chương trình chưa chọn !");
            document.getElementById("slbChuongTrinh" + index).scrollIntoView();
            return;
        } else {
            data.iID_KHCTBQP_ChuongTrinhID = $(this).find("#slbChuongTrinh" + index).val();
        }
        if (loai == "1" || loai == "3") {
            if ($(this).find("#slbDuAn" + index).val() == "" || $(this).find("#slbDuAn" + index).val() == GUID_EMPTY) {
                check = false;
                messages.push("Tên dự án chưa chọn !");
                document.getElementById("slbDuAn" + index).scrollIntoView();
                return;
            } else {
                data.iID_DuAnID = $(this).find("#slbDuAn" + index).val();
            }
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
    $("#tblGoiThauListImport tbody tr.wrong").each(function () {
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
        if ($(this).find("#txtTenGoiThau" + index).length == 1
            && ($.trim($(this).find("#txtTenGoiThau" + index).val()) == "" || $.trim($(this).find("#txtTenGoiThau" + index).val()).length > 300)) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtSoQuyetDinh1" + index).length == 1 && $.trim($(this).find("#txtSoQuyetDinh1" + index).val()).length > 100) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtSoQuyetDinh2" + index).length == 1 && $.trim($(this).find("#txtSoQuyetDinh2" + index).val()).length > 100) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtNgayQuyetDinh1" + index).length == 1
            && $.trim($(this).find("#txtNgayQuyetDinh1" + index).val()) != ""
            && !dateIsValid($.trim($(this).find("#txtNgayQuyetDinh1" + index).val()))) {
            check = false;
            $(this).addClass("cellWrong");
            return;
        }
        if ($(this).find("#txtNgayQuyetDinh2" + index).length == 1
            && $.trim($(this).find("#txtNgayQuyetDinh2" + index).val()) != ""
            && !dateIsValid($.trim($(this).find("#txtNgayQuyetDinh2" + index).val()))) {
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
