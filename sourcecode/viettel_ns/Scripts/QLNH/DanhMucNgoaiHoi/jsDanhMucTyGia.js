var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;

function ResetChangePage(iCurrentPage = 1) {
    var sMaTiGia = "";
    var sTenTiGia = "";
    var sMoTaTiGia = "";
    var sMaTienTeGoc = "";
    var dNgayTao = "";

    GetListData(dNgayTao, sMaTiGia, sTenTiGia, sMoTaTiGia, sMaTienTeGoc, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var sMaTiGia = $("<div/>").text($.trim($("#txtMaTiGia").val())).html();
    var sTenTiGia = $("<div/>").text($.trim($("#txtTenTiGia").val())).html();
    var sMoTaTiGia = $("<div/>").text($.trim($("#txtMoTaTiGia").val())).html();
    var sMaTienTeGoc = $("<div/>").text($.trim($("#txtMaTienTeGoc").val())).html();
    var dNgayLap = $("<div/>").text($.trim($("#txtNgayLapFilter").val())).html();

    GetListData(dNgayLap, sMaTiGia, sTenTiGia, sMoTaTiGia, sMaTienTeGoc, iCurrentPage);
}

function GetListData(dNgayLap, sMaTiGia, sTenTiGia, sMoTaTiGia, sMaTienTeGoc, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucTyGia/DanhMucTiGiaSearch",
        data: { _paging: _paging, dNgayLap: dNgayLap, sMaTiGia: sMaTiGia, sTenTiGia: sTenTiGia, sMoTaTiGia: sMoTaTiGia, sMaTienTeGoc: sMaTienTeGoc },
        success: function (data) {
            $("#lstDataView").html(data);

            $("#txtNgayLapFilter").val($("<div/>").html(dNgayLap).text());
            $("#txtMaTiGia").val($("<div/>").html(sMaTiGia).text());
            $("#txtTenTiGia").val($("<div/>").html(sTenTiGia).text());
            $("#txtMoTaTiGia").val($("<div/>").html(sMoTaTiGia).text());
            $("#txtMaTienTeGoc").val($("<div/>").html(sMaTienTeGoc).text());
        }
    });
}

function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucTyGia/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalTyGia").html(data);
            $("#modalTyGiaLabel").html('Xem chi tiết thông tin tỉ giá hối đoái');
            $("#contentModalTyGia tr").hover(function () {
                $(this).css("background-color", "#e7f8fe");
            }, function () {
                $(this).css("background-color", "");
            });
        }
    });
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DanhMucTyGia/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalTyGia").html(data);
            if (id == undefined || id == null || id == GUID_EMPTY) {
                $("#modalTyGiaLabel").html('Thêm mới thông tin tỉ giá hối đoái');
            } else {
                $("#modalTyGiaLabel").html('Sửa thông tin tỉ giá hối đoái');
            }
            $("#slbMaTienTeGoc").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });

            $(".colGiaTriTiGia").keydown(function (event) {
                ValidateInputKeydown(event, this, 2);
            }).blur(function (event) {
                ValidateInputFocusOut(event, this, 2, null);
            });

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

            $("#txtNgayLap").keydown(function (event) {
                ValidateInputKeydown(event, this, 3);
            }).blur(function (event) {
                setTimeout(() => {
                    if (!isShowing) ValidateInputFocusOut(event, this, 3);
                }, 0);
            });
        }
    });
}

function Delete(id) {
    var Title = 'Xác nhận xóa tỉ giá hối đoái';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn xóa?');
    var FunctionName = "DeleteItem('" + id + "')";
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
        success: function (data) {
            $("#divModalConfirm").empty().html(data);
        }
    });
}

function DeleteItem(id) {
    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucTyGia/TyGiaDelete",
        data: { id: id },
        success: function (r) {
            if (r && r.bIsComplete) {
                ChangePage();
            } else {
                var Title = 'Lỗi xóa tỉ giá';
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

function GetDataTiGiaChiTiet() {
    var dataTiGiaChiTiet = [];
    var obj;
    $.each($("#tblTiGiaChiTiet tbody tr"), function (index, item) {
        var fTiGia = $(item).find(".colGiaTriTiGia").val();
        if (fTiGia == null || fTiGia == "") return;
        var fTiGiaUnformat = UnFormatNumber(fTiGia);
        if (!/^\d+(\.\d+)?$/.test(fTiGiaUnformat)) return;
        obj = {};
        obj.ID = $(item).data("idtgct");
        obj.iID_TiGiaID = ($("#iID_TyGiaModal").val() == "" || $("#iID_TyGiaModal").val() == GUID_EMPTY) ? null : $("#iID_TyGiaModal").val();
        obj.iID_TienTeID = $(item).data("idtiente");
        obj.sMaTienTeQuyDoi = $(item).find(".colTienTeQuyDoi").html();
        obj.fTiGia = fTiGiaUnformat;
        dataTiGiaChiTiet.push(obj);
    });
    return dataTiGiaChiTiet;
}

function Save() {
    var dataTiGia = {};
    dataTiGia.ID = $("#iID_TyGiaModal").val();
    dataTiGia.sMaTiGia = $("<div/>").text($.trim($("#txtsMaTyGia").val())).html();
    dataTiGia.sTenTiGia = $("<div/>").text($.trim($("#txtsTenTyGia").val())).html();
    dataTiGia.sMoTaTiGia = $("<div/>").text($.trim($("#txtsMotaTyGia").val())).html();
    dataTiGia.iID_TienTeGocID = $("#slbMaTienTeGoc").val();
    dataTiGia.sMaTienTeGoc = $("<div/>").text($.trim($("#slbMaTienTeGoc option:selected").data("matiente"))).html();
    dataTiGia.dThangLapTiGia = $("<div/>").text($.trim($("#txtNgayLap").val())).html();

    if (!ValidateData(dataTiGia)) {
        return;
    }
    var dataTiGiaChiTietParam = GetDataTiGiaChiTiet();

    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucTyGia/TyGiaSave",
        data: { dataTiGia: dataTiGia, dataTiGiaChiTietParam: dataTiGiaChiTietParam },
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/QLNH/DanhMucTyGia";
            } else {
                var Title = 'Lỗi lưu tỉ giá';
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
    var Title = 'Lỗi thêm mới/chỉnh sửa tỉ giá';
    var Messages = [];

    if (data.sMaTiGia == null || data.sMaTiGia == "") {
        Messages.push("Mã tỉ giá chưa nhập !");
    }

    if (data.sTenTiGia == null || data.sTenTiGia == "") {
        Messages.push("Tên tỉ giá chưa nhập !");
    }

    if (data.iID_TienTeGocID == null || data.iID_TienTeGocID == GUID_EMPTY) {
        Messages.push("Mã tiền tệ gốc chưa chọn !");
    }

    if ($.trim($("#txtNgayLap").val()) != "" && !dateIsValid($.trim($("#txtNgayLap").val()))) {
        Messages.push("Ngày lập tỉ giá không hợp lệ !");
    }

    if ($.trim($("#txtsMaTyGia").val()) != "" && $.trim($("#txtsMaTyGia").val()).length > 50) {
        Messages.push("Mã tỉ giá vượt quá 50 kí tự !");
    }

    if ($.trim($("#txtsTenTyGia").val()) != "" && $.trim($("#txtsTenTyGia").val()).length > 50) {
        Messages.push("Tên tỉ giá vượt quá 50 kí tự !");
    }

    if (Messages.length > 0) {
        //$.ajax({
        //    type: "POST",
        //    url: "/Modal/OpenModal",
        //    data: { Title: Title, Messages: Messages, Category: ERROR },
        //    async: false,
        //    success: function (data) {
        //        $("#divModalConfirm").empty().html(data);
        //    }
        //});
        alert(Messages.join("\n"));
        return false;
    }
    return true;
}

function ChangeMaTienTeGocSelect() {
    var idTiGia = $("#iID_TyGiaModal").val();
    if (idTiGia != "" && idTiGia != GUID_EMPTY) return;
    var idTienTeGoc = $("#slbMaTienTeGoc").val();
    if (idTienTeGoc == GUID_EMPTY) {
        $("#tblTiGiaChiTiet tbody").empty();
        return;
    }
    var idTienTeGocCu = $("#hidTienTeGocID").val();
    var maTienTeGoc = $("<div/>").text($("#slbMaTienTeGoc option:selected").data("matiente")).html();
    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucTyGia/ChangeMaTienTeGoc",
        data: { idTiGia: idTiGia, idTienTeGoc: idTienTeGoc, maTienTeGoc: maTienTeGoc, idTienTeGocCu: idTienTeGocCu },
        success: function (res) {
            if (res) {
                $("#tblTiGiaChiTiet tbody").empty().html(res.htmlStr);

                $(".colGiaTriTiGia").keydown(function (event) {
                    ValidateInputKeydown(event, this, 2);
                }).blur(function (event) {
                    ValidateInputFocusOut(event, this, 2, null);
                });
            }
        }
    });
}