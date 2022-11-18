var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;

function ResetChangePage(iCurrentPage = 1) {
    var fGiaDeNghiTu = "";
    var fGiaDeNghiDen = "";
    var sSoBaoCao = "";
    var sTenDuAn = "";
    var sMaDuAn = "";
    $("#txt_GiaDeNghiTuSearch").val("");
    $("#txt_GiaDeNghiDenSearch").val("");
    $("#txt_GiaDeNghiTuViewSearch").val("");
    $("#txt_GiaDeNghiDenViewSearch").val("");
    $("#txt_SoBaoCaoSearch").val("");
    $("#txt_TenDuAnSearch").val("");
    $("#txt_MaDuAnSearch").val("");

    GetValueFormExport(sSoBaoCao, fGiaDeNghiTu, fGiaDeNghiDen, sTenDuAn, sMaDuAn);

    GetListData(sSoBaoCao, fGiaDeNghiTu, fGiaDeNghiDen, sTenDuAn, sMaDuAn, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    //var fGiaDeNghiTu = $("#txt_GiaDeNghiTuSearch").val();
    //var fGiaDeNghiDen = $("#txt_GiaDeNghiDenSearch").val();
    var fGiaDeNghiTu = $("#txt_GiaDeNghiTuViewSearch").val();
    var fGiaDeNghiDen = $("#txt_GiaDeNghiDenViewSearch").val();
    var Title = 'Lỗi tìm kiếm đề nghị quyết toán';
    var messErr = [];

    if (!(/^\d*$/.test(fGiaDeNghiTu))) {
        messErr.push("Giá trị đề nghị quyết toán từ phải là số !");
    }

    if (!(/^\d*$/.test(fGiaDeNghiDen))) {
        messErr.push("Giá trị đề nghị quyết toán đến phải là số !");
    }

    if (messErr.length > 0) {
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: messErr, Category: ERROR },
            success: function (data) {
                $("#divModalConfirm").html(data);

                return false;
            }
        });
    }

    var sSoBaoCao = $("#txt_SoBaoCaoSearch").val();
    var sTenDuAn = $("#txt_TenDuAnSearch").val();
    var sMaDuAn = $("#txt_MaDuAnSearch").val();

    GetValueFormExport(sSoBaoCao, fGiaDeNghiTu, fGiaDeNghiDen, sTenDuAn, sMaDuAn);

    GetListData(sSoBaoCao, fGiaDeNghiTu, fGiaDeNghiDen, sTenDuAn, sMaDuAn, iCurrentPage);
}

function GetValueFormExport(sSoBaoCao, fGiaDeNghiTu, fGiaDeNghiDen, sTenDuAn, sMaDuAn) {

    $("#txt_GiaDeNghiTuExcel").val(fGiaDeNghiTu);
    $("#txt_GiaDeNghiDenExcel").val(fGiaDeNghiDen);
    $("#txt_SoBaoCaoExcel").val(sSoBaoCao);
    $("#txt_TenDuAnExcel").val(sTenDuAn);
    $("#txt_MaDuAnExcel").val(sMaDuAn);

}

function CancelSaveData() {
    window.location.href = "/DanhMuc/DMLoaiChiPhi/Index";
}

function EventOpenModal(id) {
    $("#txt_DonViQuanLy").on('select2:select', function (e) {
        $('#txt_DuAn').empty();
        $('#txt_DuAn').val(null);
        $('#txt_DuAn').trigger('change');
        $("#idFormTenDuAn").attr('hidden', true);
        $("#idFormChuDauTu").attr('hidden', true);
        var data = e.params.data;
        if (data != null) {
            LoadDataComboBoxDuAn(data);
        }
    });

    $("#txt_DuAn").on('select2:select', function (e) {
        $("#idFormTenDuAn").attr('hidden', true);
        $("#idFormChuDauTu").attr('hidden', true);
        var data = e.params.data;
        if (data != null) {
            GetDuLieuDuAn(data.id);
        }
    });
}

function GetDuLieuDuAn(idDuAn) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/GetDuLieuDuAnByIdDonViQuanLy",
        data: { idDuAn: idDuAn },
        success: function (resp) {
            if (resp.status == true) {
                if (resp.data.sTenDuAn != null && resp.data.sTenDuAn != "" && resp.data.sTenChuDauTu != null && resp.data.sTenChuDauTu != "") {
                    //$("#txt_TenDuAn").html(resp.data.sTenDuAn);
                    //$("#txt_ChuDauTu").html(resp.data.sTenChuDauTu);
                    $("#txt_TenDuAn").val(resp.data.sTenDuAn);
                    $("#txt_TenDuAn").attr('disabled', true);
                    $("#txt_ChuDauTu").val(resp.data.sTenChuDauTu);
                    $("#txt_ChuDauTu").attr('disabled', true);

                    $("#idFormTenDuAn").attr('hidden', false);
                    $("#idFormChuDauTu").attr('hidden', false);
                }
            }
        }
    });
}

function LoadDataComboBoxDonViQuanLy() {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/GetDonViQuanLy",
        success: function (resp) {
            if (resp.status == true) {
                $("#txt_DonViQuanLy").select2({
                    data: resp.data
                });
            }
        }
    });
}

function LoadDataComboBoxDuAn(idDonVi) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/GetDuAnTheoDonViQuanLy",
        data: { idDonVi: idDonVi.id },
        success: function (resp) {
            if (resp.status == true) {
                $("#txt_DuAn").select2({
                    data: resp.data
                });
            }
        }
    });
}

function SetValueDuAn(idDuAn) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/GetDuLieuDuAnByIdDonViQuanLy",
        data: { idDuAn: idDuAn },
        success: function (resp) {
            if (resp.status == true) {
                if (resp.data.sTenDuAn != null && resp.data.sTenDuAn != "" && resp.data.sTenChuDauTu != null && resp.data.sTenChuDauTu != "") {
                    //$("#txt_TenDuAn").html(resp.data.sTenDuAn);
                    //$("#txt_ChuDauTu").html(resp.data.sTenChuDauTu);
                    $("#txt_TenDuAn").val(resp.data.sTenDuAn);
                    $("#txt_TenDuAn").attr('disabled', true);
                    $("#txt_ChuDauTu").val(resp.data.sTenChuDauTu);
                    $("#txt_ChuDauTu").attr('disabled', true);

                    $("#idFormTenDuAn").attr('hidden', false);
                    $("#idFormChuDauTu").attr('hidden', false);
                }
            }
        }
    });
}

function SetValueDonViQuanLy(idDuAn) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/GetDuLieuDonViQuanLyByIdDuAn",
        data: { idDuAn: idDuAn },
        success: function (resp) {
            if (resp.status == true) {
                $("#txt_DonViQuanLy").val(resp.data.iID_Ma);
                $("#txt_DonViQuanLy").trigger('change');
            }
        }
    });
}

function ViewCreate() {
    window.location.href = "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/Create";
}

function ViewUpdate(id) {
    location.href = "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/Update/" + id;
}

function ViewDetail(id) {
    location.href = "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/Detail/" + id;
}

function ViewInBaoCao() {
    window.location.href = "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/ViewInBaoCao";
}

function printBC(isExcel) {
    var data = {};
    data.iID_DeNghiQuyetToanID = $("#iID_DeNghiQuyetToanID").val();
    data.iItemLoaiBC = $("#Value").val();
    data.txt_TieuDe = $("#txt_TieuDe").val();
    data.txt_DiaDiem = $("#txt_DiaDiem").val();
    data.txt_KinhGui = $("#txt_KinhGui").val();
    data.dNgayChungTu = $("#dNgayChungTu").val();
    data.txt_TinhHinh = $("#txt_TinhHinh").val();
    data.txt_NhanXet = $("#txt_NhanXet").val();
    data.txt_KienNghi = $("#txt_KienNghi").val();
    data.fDonViTinh = $("#ValueItem").val();
    data.sDonViTinh = $("#ValueItem option:selected").text();
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/ExportBC",
        data: { data: data },
        success: function (r) {
            if (r == "True") {
                if (isExcel) {
                    window.open("/QLVonDauTu/VDT_QT_DeNghiQuyetToan/ExportExcelFile");
                }
                else window.open("/QLVonDauTu/VDT_QT_DeNghiQuyetToan/ExportExcel");
            }
            else {
                window.open("/QLVonDauTu/KeHoachTrungHanDeXuat/ViewInBaoCao");
            }
        }
    });
}

function ExportExcel() {
    var links = [];
    var url = "";
    var listId = [];
    var ext = "xlsx"
    $("#lstDataView").find('tr').each(function () {
        if ($(this).find('input[type="checkbox"]').is(':checked')) {
            var id = $(this).find('input[type="checkbox"]').data("id")
            if (!ValidateDataPrint(id)) {
                return;
            }
            listId.push(id)
        }
    });
    for (var i = 0; i < listId.length; i++) {
        var iId = listId[i];
        links = [];

            url = $("#urlExportBCChiTiet").val() +
                "?ext=" + ext + "&listId=" + iId
            url = unescape(url);
            links.push(url);
            openLinks(links);
    }
}
 
// Validate form print
function ValidateDataPrint(listId) {
    var Title = 'Lỗi xuất file !';
    var Messages = [];

    if (listId == '') {
        Messages.push("Vui lòng chọn bản ghi!");
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

function backViewDeNghiQT() {
    window.location.href = "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/Index/";
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalVDTQTDeNghiQuyetToan").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalVDTQTDeNghiQuyetToanLabel").html('Thêm mới đề nghị quyết toán');
            }
            else {
                $("#modalVDTQTDeNghiQuyetToanLabel").html('Cập nhật đề nghị quyết toán');
            }

            $(".date").datepicker({
                todayBtn: "linked",
                language: "vi",
                autoclose: true,
                todayHighlight: true,
                format: 'dd/mm/yyyy'
            });

            $("#txt_DonViQuanLy").select2({
                width: 'resolve',
                matcher: matchStart
            });

            $("#txt_DuAn").select2({
                width: 'resolve',
                matcher: matchStart
            });

            LoadDataComboBoxDonViQuanLy();
            EventOpenModal(id);

            if (id != null && id != GUID_EMPTY) {
                $("#idFormDuAn").attr('hidden', true);
                $("#txt_DonViQuanLy").attr('disabled', true);
                SetValueComboBoxDuAn(id);
            }
        }
    });
}

function SetValueComboBoxDuAn(id) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/SetValueComboBoxDuAn",
        data: { id: id },
        success: function (resp) {
            if (resp.status == true) {
                if (resp.data != null) {
                    SetValueDuAn(resp.data.iID_DuAnID);
                    SetValueDonViQuanLy(resp.data.iID_DuAnID);
                }
            }
        }
    });
}

function matchStart(params, data) {
    if ($.trim(params.term) === '') {
        return data;
    }

    if (typeof data.children === 'undefined') {
        return null;
    }


    var filteredChildren = [];
    $.each(data.children, function (idx, child) {
        if (child.text.toUpperCase().indexOf(params.term.toUpperCase()) == 0) {
            filteredChildren.push(child);
        }
    });

    if (filteredChildren.length) {
        var modifiedData = $.extend({}, data, true);
        modifiedData.children = filteredChildren;

        return modifiedData;
    }

    return null;
}

function SaveData() {
    if (!(/^\d*$/.test($("#txt_GiaTriQuyetToan").val()))) {

        var Title = 'Lỗi lưu đề nghị quyết toán';
        var messErr = [];
        messErr.push("Giá trị quyết toán phải là số !");
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: messErr, Category: ERROR },
            success: function (data) {
                $("#divModalConfirm").html(data);
            }
        });

        return false;
    }

    var data = {};

    data.iID_DeNghiQuyetToanID = $("#txt_ID_DeNghiQuyetToan").val();
    data.sSoBaoCao = $("#txt_SoBaoCao").val();
    data.dThoiGianLapBaoCao = $("#txtNgayNhanModal").val();
    data.sNguoiLap = $("#txt_NguoiNhan").val();
    data.dThoiGianNhanBaoCao = $("#txtNgayDuyetModal").val();
    data.sNguoiNhan = $("#txt_NguoiDuyet").val();
    data.iID_DuAnID = $("#txt_DuAn").val();
    data.dThoiGianKhoiCong = $("#txtThoiGianKhoiCongModal").val();
    data.dThoiGianHoanThanh = $("#txtThoiGianHoanThanhModal").val();
    data.sMoTa = $("#txt_GhiChu").val();
    data.fGiaTriDeNghiQuyetToan = parseInt($("#txt_GiaTriQuyetToan").val());

    //if (!ValidateData(data)) {
    //    return false;
    //}

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/SaveData",
        data: { data: data },
        success: function (r) {
            if (r.status) {
                window.location.href = "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/Index";
            } else {
                var Title = 'Lỗi lưu đề nghị quyết toán';
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

function DeleteItem(id) {
    var Title = 'Xác nhận xóa đề nghị quyết toán';
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
        url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/Delete",
        data: { id: id },
        success: function (r) {
            if (r.status == false) {
                var Title = 'Lỗi xóa đề nghị quyết toán';
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
            ChangePage();
        }
    });
}

function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/GetModalDetail",
        data: { id: id },
        success: function (data) {
            console.log(data);
            $("#contentModalVDTQTDeNghiQuyetToan").html(data);
            $("#modalVDTQTDeNghiQuyetToanLabel").html('Chi tiết đề nghị quyết toán');
        }
    });
}

function GetListData(sSoBaoCao, fGiaDeNghiTu, fGiaDeNghiDen, sTenDuAn, sMaDuAn, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { _paging: _paging, sSoBaoCao: sSoBaoCao, sGiaDeNghiTu: fGiaDeNghiTu, sGiaDeNghiDen: fGiaDeNghiDen, sTenDuAn: sTenDuAn, sMaDuAn: sMaDuAn },
        success: function (data) {
            $("#lstDataView").html(data);

            $("#txt_GiaDeNghiTuSearch").val(fGiaDeNghiTu);
            $("#txt_GiaDeNghiDenSearch").val(fGiaDeNghiDen);
            $("#txt_GiaDeNghiTuViewSearch").val(fGiaDeNghiTu);
            $("#txt_GiaDeNghiDenViewSearch").val(fGiaDeNghiDen);
            $("#txt_SoBaoCaoSearch").val(sSoBaoCao);
            $("#txt_TenDuAnSearch").val(sTenDuAn);
            $("#txt_MaDuAnSearch").val(sMaDuAn);

            GetValueFormExport(sSoBaoCao, fGiaDeNghiTu, fGiaDeNghiDen, sTenDuAn, sMaDuAn);
        }
    });
}

function ImportDNQT() {
    window.location.href = "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/ImportDNQT";
}
