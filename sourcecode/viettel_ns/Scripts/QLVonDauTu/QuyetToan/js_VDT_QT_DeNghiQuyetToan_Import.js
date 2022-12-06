var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';

$(document).ready(function () {
    $("#txt_DonViQuanLy").select2({
        width: 'resolve',
        matcher: matchStart
    });

    $("#txt_DuAn").select2({
        width: 'resolve',
        matcher: matchStart
    });

    $("#iIdDuToanId").select2({
        width: 'resolve',
        matcher: matchStart
    });
    SetDataComboBoxDonViQuanLy();
    EventChangeValue();
});

function EventChangeValue() {
    $("#txt_DonViQuanLy").on('select2:select', function (e) {
        $('#txt_DuAn').empty();
        $('#txt_DuAn').val(null);
        $('#txt_DuAn').trigger('change');
        var data = e.params.data;
        if (data != null) {
            LoadDataComboBoxDuAn(data);
        }
    });

    $("#txt_DuAn").on('select2:select', function (e) {
        var data = e.params.data;
        if (data != null) {
            LoadDataComboboxDuToan(data.id)
        }
    });

    $("#iIdDuToanId").on('select2:select', function (e) {
        var data = e.params.data;
        if (data != null) {
            GetDuLieuDuAn(data.id);
            GetListChiPhiHangMuc(data.id);
        }
    });
}

function SetDataComboBoxDonViQuanLy() {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/GetDonViQuanLy",
        success: function (resp) {
            console.log(resp);
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

function LoadDataComboboxDuToan(iIdDuToanId) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/GetListDuToanByDuAn",
        data: { iIdDuAnId: iIdDuToanId },
        success: function (resp) {
            $("#iIdDuToanId").empty();
            if (resp.datas != null && resp.datas.length != 0) {
                $("#iIdDuToanId").select2({
                    data: resp.datas
                });
                GetDuLieuDuAn(resp.datas[0].id);
                GetListChiPhiHangMuc(resp.datas[0].id);
            }
        }
    });
}

function refreshImport() {
    $("#FileUpload").val("");
    $("#txtChiPhiThietHai").val("");
    $("#txtChiPhiKhongTaoNenTaiSan").val("");
    $("#txtTaiSanDaiHanThuocCDTQuanLy").val("");
    $("#txtTaiSanDaiHanDvKhacQuanLy").val("");
    $("#txtTaiSanNganHanThuocCDTQuanLay").val("");
    $("#txtTaiSanNganHanDvKhacQuanLy").val("");
    $("#txtGiaTriQuyetToan").val("");
    $("#" + "tblDanhSachChiTiet" + " tbody tr").each(function (tr) {
        $('*[data-parentid="null"]').find('.txtGiaTriQuyetToanAB').val("");
        $('*[data-parentid="null"]').find('.txtKetQuaKiemToan').val("");
        $('*[data-parentid="null"]').find('.txtCDTDeNghiQuyetToan').val("");
        $('*[data-parentid="null"]').find('.txtChenhLechSoVoiDuToan').html("");
        $('*[data-parentid="null"]').find('.txtChenhLechSoVoiQuyetToanAB').html("");
        $('*[data-parentid="null"]').find('.txtChenhLechSoVoiKetQuaKiemToan').html("");
    });
    $("#" + "tblDanhSachNguonVon" + " tbody tr").each(function () {
        $("tr").find('.txtTienCDTQuyetToan').val("");
    });
}

function downloadImpExp() {
    window.location.href = "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/DownloadImportExample";
}

function SaveData() {
    var data = GetDataBeforeSave();

    if (!ValidateBeforeSave(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/SaveData",
        data: { data: data },
        success: function (r) {
            if (r.status) {
                alert("Tạo mới thành công.");
                window.location.href = "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/Index";
            } else {
                alert("Lỗi khi lưu.");
            }
        }
    });
}

function GetDataBeforeSave() {
    var data = {};

    data.iID_DeNghiQuyetToanID = $("#txt_ID_DeNghiQuyetToan").val();
    data.sSoBaoCao = $("#txt_SoBaoCao").val();
    data.dThoiGianLapBaoCao = $("#txtNgayBaoCao").val();
    data.iID_DuAnID = $("#txt_DuAn").val();
    data.dThoiGianKhoiCong = $("#txtThoiGianKhoiCong").val();
    data.dThoiGianHoanThanh = $("#txtThoiGianHoanThanh").val();
    data.fGiaTriDeNghiQuyetToan = $("#txtGiaTriQuyetToan").val() == "" ? null : parseFloat(UnFormatNumber($("#txtGiaTriQuyetToan").val()));
    data.iID_DonViID = $("#txt_DonViQuanLy").val();
    data.sMoTa = $("#txtGhiChu").val();

    data.fChiPhiThietHai = $("#txtChiPhiThietHai").val() == "" ? null : parseFloat(UnFormatNumber($("#txtChiPhiThietHai").val()));
    data.fChiPhiKhongTaoNenTaiSan = $("#txtChiPhiKhongTaoNenTaiSan").val() == "" ? null : parseFloat(UnFormatNumber($("#txtChiPhiKhongTaoNenTaiSan").val()));

    data.fTaiSanDaiHanThuocCDTQuanLy = $("#txtTaiSanDaiHanThuocCDTQuanLy").val() == "" ? null : parseFloat(UnFormatNumber($("#txtTaiSanDaiHanThuocCDTQuanLy").val()));
    data.fTaiSanDaiHanDonViKhacQuanLy = $("#txtTaiSanDaiHanDvKhacQuanLy").val() == "" ? null : parseFloat(UnFormatNumber($("#txtTaiSanDaiHanDvKhacQuanLy").val()));

    data.fTaiSanNganHanThuocCDTQuanLy = $("#txtTaiSanNganHanThuocCDTQuanLay").val() == "" ? null : parseFloat(UnFormatNumber($("#txtTaiSanNganHanThuocCDTQuanLay").val()));
    data.fTaiSanNganHanDonViKhacQuanLy = $("#txtTaiSanNganHanDvKhacQuanLy").val() == "" ? null : parseFloat(UnFormatNumber($("#txtTaiSanNganHanDvKhacQuanLy").val()));
    data.iID_DuToanID = $("#iIdDuToanId").val();
    data.listNguonVon = GetDataNguonVon();
    data.listChiPhi = GetDataChiPhi();
    data.listHangMuc = arrHangMuc;
    return data;
}

function GetDataNguonVon() {
    var lstData = [];
    $("#tblDanhSachNguonVon tbody tr").each(function (index, row) {
        var iIdNguonVonId = $(row).attr("data-id");
        //var fTienToTrinh = $(row).find(".txtTienCDTQuyetToan").val() == "" ? null : parseFloat(UnFormatNumber($(row).find(".txtTienCDTQuyetToan").val()));
        lstData.push({
            //fTienToTrinh: fTienToTrinh,
            iID_NguonVonID: iIdNguonVonId
        })
    })
    return lstData;
}

function GetDataChiPhi() {
    var lstDataChiPhi = [];
    $("#tblDanhSachChiTiet tbody tr").each(function (index, row) {
        var iID_DuAn_ChiPhi = $(row).attr("data-id");
        var fGiaTriKiemToan = $(row).find(".txtKetQuaKiemToan").val() == "" ? null : parseFloat(UnFormatNumber($(row).find(".txtKetQuaKiemToan").val()));
        var fGiaTriDeNghiQuyetToan = $(row).find(".txtCDTDeNghiQuyetToan").val() == "" ? null : parseFloat(UnFormatNumber($(row).find(".txtCDTDeNghiQuyetToan").val()));
        var fGiaTriQuyetToanAB = $(row).find(".txtGiaTriQuyetToanAB").val() == "" ? null : parseFloat(UnFormatNumber($(row).find(".txtGiaTriQuyetToanAB").val()));
        lstDataChiPhi.push({
            iID_DuAn_ChiPhi: iID_DuAn_ChiPhi,
            fGiaTriKiemToan: fGiaTriKiemToan,
            fGiaTriDeNghiQuyetToan: fGiaTriDeNghiQuyetToan,
            fGiaTriQuyetToanAB: fGiaTriQuyetToanAB
        })
    })
    return lstDataChiPhi;
}

function ValidateBeforeSave(data) {
    var message = [];
    var title = 'Lỗi lưu đề nghị quyết toán';

    if (data.sSoBaoCao == null || data.sSoBaoCao == "") {
        message.push("Vui lòng nhập số báo cáo.");
    }
    if (data.dThoiGianLapBaoCao == null || data.dThoiGianLapBaoCao == "") {
        message.push("Vui lòng chọn ngày báo cáo.");
    }
    if (data.iID_DonViID == null || data.iID_DonViID == "") {
        message.push("Vui lòng chọn đơn vị.");
    }
    if (data.iID_DuAnID == null || data.iID_DuAnID == "") {
        message.push("Vui lòng chọn dự án.");
    }
    if (data.iID_DuToanID == null || data.iID_DuToanID == "") {
        message.push("Vui lòng chọn dự toán.");
    }

    if (message != null && message != undefined && message.length > 0) {
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: title, Messages: message, Category: 1 },
            success: function (data) {
                $("#divModalConfirm").html(data);
            }
        });
        return false;
    }

    return true;
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

function loadDataExcel() {
    if (!ValidateData()) {
        return false;
    }
    var sumGiaTriDeNghiQuyetToan = 0;
    var sumGiaTriQuyetToanAB = 0;
    var sumGiaTriKiemToan = 0;
    var iIdDuToanId = $("#iIdDuToanId").val();
    var fileInput = document.getElementById('FileUpload');
    var file = fileInput.files[0];
    var formData = new FormData();
    formData.append('file', file);
    formData.append('iIdDuToanId', iIdDuToanId);
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/LoadDataExcel",
        data: formData,
        contentType: false,
        processData: false,
        cache: false,
        async: false,
        success: function (r) {
            if (r.bIsComplete) {
                document.getElementById("txtChiPhiThietHai").value = r.dataImportChiPhiKhac[0].sChiPhiThietHai;
                document.getElementById("txtChiPhiKhongTaoNenTaiSan").value = r.dataImportChiPhiKhac[0].sChiPhiKhongTaoNenTaiSan;
                document.getElementById("txtTaiSanDaiHanThuocCDTQuanLy").value = r.dataImportTaiSan[0].sTaiSanDaiHanThuocCDTQuanLy;
                document.getElementById("txtTaiSanDaiHanDvKhacQuanLy").value = r.dataImportTaiSan[0].sTaiSanDaiHanDonViKhacQuanLy;
                document.getElementById("txtTaiSanNganHanThuocCDTQuanLay").value = r.dataImportTaiSan[0].sTaiSanNganHanThuocCDTQuanLy;
                document.getElementById("txtTaiSanNganHanDvKhacQuanLy").value = r.dataImportTaiSan[0].sTaiSanNganHanDonViKhacQuanLy;
                r.listChiPhi.forEach(function (itemCpIp) {
                    $("#" + "tblDanhSachChiTiet" + " tbody tr").each(function (tr) {
                        $('*[data-id="' + itemCpIp.iID_DuAn_ChiPhi + '"]').find('.txtGiaTriQuyetToanAB').val(itemCpIp.sGiaTriQuyetToanAB);
                        $('*[data-id="' + itemCpIp.iID_DuAn_ChiPhi + '"]').find('.txtKetQuaKiemToan').val(itemCpIp.sGiaTriKiemToan);
                        $('*[data-id="' + itemCpIp.iID_DuAn_ChiPhi + '"]').find('.txtCDTDeNghiQuyetToan').val(itemCpIp.sGiaTriDeNghiQuyetToan);
                        $('*[data-id="' + itemCpIp.iID_DuAn_ChiPhi + '"]').find('.txtChenhLechSoVoiDuToan').html(itemCpIp.sChenhLenhSoVoiDuToan);
                        $('*[data-id="' + itemCpIp.iID_DuAn_ChiPhi + '"]').find('.txtChenhLechSoVoiQuyetToanAB').html(itemCpIp.sChenhLenhSoVoiQuyetToanAB);
                        $('*[data-id="' + itemCpIp.iID_DuAn_ChiPhi + '"]').find('.txtChenhLechSoVoiKetQuaKiemToan').html(itemCpIp.sChenhLechSoVoiKetQuaKiemToan);
                    });
                    sumGiaTriDeNghiQuyetToan += itemCpIp.fGiaTriDeNghiQuyetToan;
                    sumGiaTriQuyetToanAB += itemCpIp.fGiaTriQuyetToanAB;
                    sumGiaTriKiemToan += itemCpIp.fGiaTriKiemToan;
                })                
                document.getElementById("txtGiaTriQuyetToan").value = sumGiaTriDeNghiQuyetToan;
                $('*[data-id="' + "-1" + '"]').find('.txtTongGiaTriQuyetToanAB').html(FormatNumber(sumGiaTriQuyetToanAB));
                $('*[data-id="' + "-1" + '"]').find('.txtTongKetQuaKiemToan').html(FormatNumber(sumGiaTriKiemToan));
                $('*[data-id="' + "-1" + '"]').find('.txtTongCDTDeNghiQuyetToan').html(FormatNumber(sumGiaTriDeNghiQuyetToan));

                r.listNguonVon.forEach(function (itemNvIp) {
                    $("#" + "tblDanhSachNguonVon" + " tbody tr").each(function (tr) {
                        $('*[data-id="' + itemNvIp.iID_NguonVonID + '"]').find('.txtTienCDTQuyetToan').val(itemNvIp.sTienCDTQuyetToan);
                    });
                })
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

function ValidateData() {
    var Title = 'Lỗi lấy dữ liệu từ file excel';
    var Messages = [];

    var has_file = $("#FileUpload").val() != '';
    if (!has_file) {
        Messages.push("Đ/c chưa chọn file excel dữ liệu !");
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

function GetDuLieuDuAn(iIdDuToan) {
    $("#txt_TenDuAn").html("");
    $("#txt_ChuDauTu").html("");
    if (iIdDuToan != null && iIdDuToan != "" && iIdDuToan != GUID_EMPTY) {

        $.ajax({
            type: "POST",
            url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/GetDuLieuDuAnByIdDonViQuanLy",
            data: { iIdDuToanId: iIdDuToan, iIdDuAnId: $("#txt_DuAn").val() },
            success: function (resp) {
                if (resp.status == true) {
                    //if (resp.data.sTenDuAn != null && resp.data.sTenDuAn != "") {
                    //    $("#txt_TenDuAn").html(resp.data.sTenDuAn);
                    //}
                    if (resp.data.sTenChuDauTu != null && resp.data.sTenChuDauTu != "") {
                        /*$("#txt_ChuDauTu").html("Chủ đầu tư: " + resp.data.sTenChuDauTu);*/
                        document.getElementById("txt_ChuDauTu").value = resp.data.sTenChuDauTu;
                    }
                    var html = "";
                    if (resp.lstNguonVon != null && resp.lstNguonVon.length > 0) {
                        resp.lstNguonVon.forEach(function (item) {
                            html += "<tr data-id='" + item.iID_NguonVonID + "'>";
                            html += "<td>" + item.sTenNguonVon + "</td>";
                            html += "<td class='text-right'>" + item.sTienPheDuyet + "</td>";
                            //html += "<td><input type='text' class='form-control txtTienCDTQuyetToan text-right' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
                            html += "</tr>";
                        })
                    }
                    $("#tblDanhSachNguonVon tbody").html(html);
                    if (resp.sKhoiCong != null && resp.sKetThuc) {
                        document.getElementById("txtThoiGianKhoiCong").value = resp.sKhoiCong;
                        document.getElementById("txtThoiGianHoanThanh").value = resp.sKetThuc;
                    }
                }
            }
        });
    }
}

var arrChiPhi = [];
var arrHangMuc = [];
function GetListChiPhiHangMuc(iIdDuToan) {
    arrChiPhi = [];
    arrHangMuc = [];
    if (iIdDuToan != null && iIdDuToan != "" && iIdDuToan != GUID_EMPTY) {
        $.ajax({
            type: "POST",
            url: "/QLVonDauTu/VDT_QT_DeNghiQuyetToan/GetListChiPhiHangMucTheoDuAn",
            data: { iIdDuToanId: iIdDuToan },
            success: function (resp) {
                if (resp.lstChiPhi != null && resp.lstChiPhi.length > 0) {
                    arrChiPhi = resp.lstChiPhi;
                }
                if (resp.lstHangMuc != null && resp.lstHangMuc.length > 0) {
                    arrHangMuc = resp.lstHangMuc;
                }
                DrawTableChiPhiHangMuc(resp.sumGiaTriQuyetToanAB, resp.sumKetQuaKiemToan, resp.sumCDTDeNghiQuyetToan);
            }
        });
    }
}

function DrawTableChiPhiHangMuc(sumGiaTriQuyetToanAB, sumKetQuaKiemToan, sumCDTDeNghiQuyetToan) {
    var html = "";

    //them dong tong so
    html += "<tr style='font-weight:bold' data-id='-1'>";
    html += "<td></td>";
    html += "<td>Tổng số</td>";
    html += "<td></td>";
    html += "<td></td>";
    html += "<td class='text-right txtTongGiaTriQuyetToanAB'>" + sumGiaTriQuyetToanAB + "</td>";
    html += "<td class='text-right txtTongKetQuaKiemToan'>" + sumKetQuaKiemToan + "</td>";
    html += "<td class='text-right txtTongCDTDeNghiQuyetToan'>" + sumCDTDeNghiQuyetToan + "</td>";
    html += "<td></td>";
    html += "<td></td>";
    html += "<td></td>";
    html += "</tr>";

    arrChiPhi.forEach(function (itemCp) {
        var arrChiPhiChild = arrChiPhi.filter(x => x.iID_ChiPhi_Parent == itemCp.iID_DuAn_ChiPhi);
        var arrHangMucByChiPhi = arrHangMuc.filter(x => x.iID_DuAn_ChiPhi == itemCp.iID_DuAn_ChiPhi);
        var disabled = "", isBold = "";
        if ((arrChiPhiChild != null && arrChiPhiChild.length > 0) || (arrHangMucByChiPhi != null && arrHangMucByChiPhi.length > 0)) {
            isBold = "font-weight: bold;";
            disabled = "disabled";
        }
        html += "<tr data-loai='1' style='" + isBold + "' data-id='" + itemCp.iID_DuAn_ChiPhi + "' data-parentid='" + itemCp.iID_ChiPhi_Parent + "'>";
        html += "<td class='stt'></td>";
        html += "<td>Chi phí</td>";
        html += "<td>" + itemCp.sTenChiPhi + "</td>";
        html += "<td class='text-right'>" + itemCp.sTienPheDuyet + "</td>";
        html += "<td><input type='text' class='form-control clearable text-right txtGiaTriQuyetToanAB' " + disabled + " onchange='changeGiaTri(this)' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
        html += "<td><input type='text' class='form-control clearable text-right txtKetQuaKiemToan' " + disabled + " onchange='changeGiaTri(this)' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
        html += "<td><input type='text' class='form-control clearable text-right txtCDTDeNghiQuyetToan' " + disabled + " onchange='changeGiaTri(this)' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
        html += "<td class='text-right txtChenhLechSoVoiDuToan'></td>";
        html += "<td class='text-right txtChenhLechSoVoiQuyetToanAB'></td>";
        html += "<td class='text-right txtChenhLechSoVoiKetQuaKiemToan'></td>";
        html += "</tr>";

        // list hang muc
        if (arrHangMucByChiPhi != null && arrHangMucByChiPhi.length > 0) {
            arrHangMucByChiPhi.forEach(function (itemHm) {
                disabled = "";
                isBold = "";
                var arrHangMucChild = arrHangMucByChiPhi.filter(x => x.iID_ParentID == itemHm.iID_HangMucID);
                if (arrHangMucChild != null && arrHangMucChild.length > 0) {
                    isBold = "font-weight: bold;";
                    disabled = "disabled";
                }
                html += "<tr style='" + isBold + "' data-loai='2' data-id='" + itemHm.iID_HangMucID + "' data-parentid='" + itemHm.iID_ParentID + "' data-chiphi='" + itemHm.iID_DuAn_ChiPhi + "'>";
                html += "<td class='stt'></td>";
                html += "<td style='font-style: italic'>Hạng mục</td>";
                html += "<td style='font-style: italic'>" + itemHm.sTenHangMuc + "</td>";
                html += "<td class='text-right'>" + itemHm.sTienPheDuyet + "</td>";
                html += "<td><input type='text' class='form-control clearable text-right txtGiaTriQuyetToanAB' " + disabled + " onchange='changeGiaTri(this)' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
                html += "<td><input type='text' class='form-control clearable text-right txtKetQuaKiemToan' " + disabled + " onchange='changeGiaTri(this)' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
                html += "<td><input type='text' class='form-control clearable text-right txtCDTDeNghiQuyetToan' " + disabled + " onchange='changeGiaTri(this)' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
                html += "<td class='text-right txtChenhLechSoVoiDuToan'></td>";
                html += "<td class='text-right txtChenhLechSoVoiQuyetToanAB'></td>";
                html += "<td class='text-right txtChenhLechSoVoiKetQuaKiemToan'></td>";
                html += "</tr>";
            })
        }
    })
    $("#tblDanhSachChiTiet tbody").html(html);
    $(".div_ChiTiet").show();
}

function changeGiaTri(input) {
    var dongHienTai = $(input).closest("tr");
    var loai = $(dongHienTai).attr("data-loai");
    var id = $(dongHienTai).attr("data-id");
    var parentId = $(dongHienTai).attr("data-parentid");
    var iIdDuAnChiPhi = "";

    var objThis = "";
    if (loai == 1)
        objThis = arrChiPhi.filter(x => x.iID_DuAn_ChiPhi == id)[0];
    else if (loai == 2)
        objThis = arrHangMuc.filter(x => x.iID_HangMucID == id)[0];

    var sGiaTriQuyetToanAB = $(dongHienTai).find(".txtGiaTriQuyetToanAB").val();
    var fGiaTriQuyetToanAB = sGiaTriQuyetToanAB == "" ? 0 : parseInt(UnFormatNumber(sGiaTriQuyetToanAB));

    var sGiaTriKiemToan = $(dongHienTai).find(".txtKetQuaKiemToan").val();
    var fGiaTriKiemToan = sGiaTriKiemToan == "" ? 0 : parseInt(UnFormatNumber(sGiaTriKiemToan));

    var sGiaTriDeNghiQuyetToan = $(dongHienTai).find(".txtCDTDeNghiQuyetToan").val();
    var fGiaTriDeNghiQuyetToan = sGiaTriDeNghiQuyetToan == "" ? 0 : parseInt(UnFormatNumber(sGiaTriDeNghiQuyetToan));

    var fChenhLenhSoVoiDuToan = fGiaTriDeNghiQuyetToan - objThis.fTienPheDuyet;
    var fChenhLenhSoVoiQuyetToanAB = fGiaTriDeNghiQuyetToan - fGiaTriQuyetToanAB;
    var fChenhLechSoVoiKetQuaKiemToan = fGiaTriDeNghiQuyetToan - fGiaTriKiemToan;

    objThis.fGiaTriQuyetToanAB = fGiaTriQuyetToanAB;
    objThis.fGiaTriKiemToan = fGiaTriKiemToan;
    objThis.fGiaTriDeNghiQuyetToan = fGiaTriDeNghiQuyetToan;

    objThis.fChenhLenhSoVoiDuToan = fChenhLenhSoVoiDuToan;
    objThis.fChenhLenhSoVoiQuyetToanAB = fChenhLenhSoVoiQuyetToanAB;
    objThis.fChenhLechSoVoiKetQuaKiemToan = fChenhLechSoVoiKetQuaKiemToan;

    if (loai == 2) {
        iIdDuAnChiPhi = $(dongHienTai).attr("data-chiphi");

        arrHangMuc = arrHangMuc.filter(function (x) { return x.iID_HangMucID != objThis.iID_HangMucID });
        arrHangMuc.push(objThis);

        CalculateDataHangMucByChiPhi(id);
        CalculateTienConLaiHangMuc(iIdDuAnChiPhi);
    }

    if (loai == 1) {
        arrChiPhi = arrChiPhi.filter(function (x) { return x.iID_DuAn_ChiPhi != objThis.iID_DuAn_ChiPhi });
        arrChiPhi.push(objThis);

        CalculateDataChiPhi(id);
    }

    var sumGiaTriQuyetToanAB = 0;
    var sumGiaTriKiemToan = 0;
    var sumGiaTriDeNghiQuyetToan = 0;
    var fGiaTriQuyetToanAB = 0;
    var fGiaTriKiemToan = 0;
    var fGiaTriDeNghiQuyetToan = 0;
    //$("#tblDanhSachChiTiet tbody tr").each(function (index, row) {
    //    fGiaTriDeNghiQuyetToan = $(row).find(".txtCDTDeNghiQuyetToan").val() == "" ? null : parseFloat(UnFormatNumber($(row).find(".txtCDTDeNghiQuyetToan").val()));
    //    sumGiaTriDeNghiQuyetToan += fGiaTriDeNghiQuyetToan
    //})
    //document.getElementById("txtGiaTriQuyetToan").value = sumGiaTriDeNghiQuyetToan;
    $("#" + "tblDanhSachChiTiet" + " tbody tr").each(function (index, row) {
        if (($(row).filter("[data-loai='1']").find(".txtCDTDeNghiQuyetToan").val())) {
            fGiaTriQuyetToanAB = parseFloat(UnFormatNumber($(row).filter("[data-loai='1']").find(".txtGiaTriQuyetToanAB").val() == "" ? 0 : $(row).filter("[data-loai='1']").find(".txtGiaTriQuyetToanAB").val()));
            fGiaTriKiemToan = parseFloat(UnFormatNumber($(row).filter("[data-loai='1']").find(".txtKetQuaKiemToan").val() == "" ? 0 : $(row).filter("[data-loai='1']").find(".txtKetQuaKiemToan").val()));
            fGiaTriDeNghiQuyetToan = parseFloat(UnFormatNumber($(row).filter("[data-loai='1']").find(".txtCDTDeNghiQuyetToan").val() == "" ? 0 : $(row).filter("[data-loai='1']").find(".txtCDTDeNghiQuyetToan").val()));

            sumGiaTriQuyetToanAB += fGiaTriQuyetToanAB;
            sumGiaTriKiemToan += fGiaTriKiemToan;
            sumGiaTriDeNghiQuyetToan += fGiaTriDeNghiQuyetToan;
        }
    });
    document.getElementById("txtGiaTriQuyetToan").value = sumGiaTriDeNghiQuyetToan;

    $('*[data-id="' + "-1" + '"]').find('.txtTongGiaTriQuyetToanAB').html(FormatNumber(sumGiaTriQuyetToanAB));
    $('*[data-id="' + "-1" + '"]').find('.txtTongKetQuaKiemToan').html(FormatNumber(sumGiaTriKiemToan));
    $('*[data-id="' + "-1" + '"]').find('.txtTongCDTDeNghiQuyetToan').html(FormatNumber(sumGiaTriDeNghiQuyetToan));

    $(dongHienTai).find(".txtChenhLechSoVoiDuToan").html(FormatNumber(fChenhLenhSoVoiDuToan));
    $(dongHienTai).find(".txtChenhLechSoVoiQuyetToanAB").html(FormatNumber(fChenhLenhSoVoiQuyetToanAB));
    $(dongHienTai).find(".txtChenhLechSoVoiKetQuaKiemToan").html(FormatNumber(fChenhLechSoVoiKetQuaKiemToan));
}

function CalculateDataHangMucByChiPhi(itemId) {
    var objItem = arrHangMuc.find(x => x.iID_HangMucID == itemId);
    if (objItem == undefined || objItem.iID_ParentID == "" || objItem.iID_ParentID == null) {
        return;
    }
    var objParentItem = arrHangMuc.find(x => x.iID_HangMucID == objItem.iID_ParentID);
    var arrChildSameParent = arrHangMuc.filter(function (x) { return x.iID_ParentID == objItem.iID_ParentID && x.iID_ParentID != "" && x.iID_ParentID != null });
    if (arrChildSameParent != null && arrChildSameParent.length > 0) {
        CalculateTotalParentHangMuc(objParentItem, arrChildSameParent);
    }

    if (objParentItem.iID_ParentID != "" && objParentItem.iID_ParentID != null) {
        CalculateDataHangMucByChiPhi(objParentItem.iID_HangMucID, arrHangMuc);
    }
}

function CalculateTotalParentHangMuc(objParentItem, arrChild) {
    var parentId = objParentItem.iID_HangMucID;

    var sumGiaTriQuyetToanAB = 0, sumGiaTriKiemToan = 0, sumGiaTriDeNghiQuyetToan = 0;
    arrChild.forEach(x => {
        sumGiaTriQuyetToanAB += x.fGiaTriQuyetToanAB;
        sumGiaTriKiemToan += x.fGiaTriKiemToan;
        sumGiaTriDeNghiQuyetToan += x.fGiaTriDeNghiQuyetToan;
    });

    var fChenhLenhSoVoiDuToan = sumGiaTriDeNghiQuyetToan - objParentItem.fTienPheDuyet;
    var fChenhLenhSoVoiQuyetToanAB = sumGiaTriDeNghiQuyetToan - sumGiaTriQuyetToanAB;
    var fChenhLechSoVoiKetQuaKiemToan = sumGiaTriDeNghiQuyetToan - sumGiaTriKiemToan;

    $('*[data-id="' + parentId + '"]').find('.txtGiaTriQuyetToanAB').val(FormatNumber(sumGiaTriQuyetToanAB));
    $('*[data-id="' + parentId + '"]').find('.txtKetQuaKiemToan').val(FormatNumber(sumGiaTriKiemToan));
    $('*[data-id="' + parentId + '"]').find('.txtCDTDeNghiQuyetToan').val(FormatNumber(sumGiaTriDeNghiQuyetToan));

    $('*[data-id="' + parentId + '"]').find(".txtChenhLechSoVoiDuToan").html(FormatNumber(fChenhLenhSoVoiDuToan));
    $('*[data-id="' + parentId + '"]').find(".txtChenhLechSoVoiQuyetToanAB").html(FormatNumber(fChenhLenhSoVoiQuyetToanAB));
    $('*[data-id="' + parentId + '"]').find(".txtChenhLechSoVoiKetQuaKiemToan").html(FormatNumber(fChenhLechSoVoiKetQuaKiemToan));

    var parentItemNew = objParentItem;

    objParentItem.fGiaTriQuyetToanAB = sumGiaTriQuyetToanAB;
    objParentItem.fGiaTriKiemToan = sumGiaTriKiemToan;
    objParentItem.fGiaTriDeNghiQuyetToan = sumGiaTriDeNghiQuyetToan;

    objParentItem.fChenhLenhSoVoiDuToan = fChenhLenhSoVoiDuToan;
    objParentItem.fChenhLenhSoVoiQuyetToanAB = fChenhLenhSoVoiQuyetToanAB;
    objParentItem.fChenhLechSoVoiKetQuaKiemToan = fChenhLechSoVoiKetQuaKiemToan;

    arrHangMuc = arrHangMuc.filter(function (x) { return x.iID_HangMucID != objParentItem.iID_HangMucID });
    arrHangMuc.push(parentItemNew);
}

function CalculateTienConLaiHangMuc(idChiPhi) {
    var sumGiaTriQuyetToanAB = 0, sumGiaTriKiemToan = 0, sumGiaTriDeNghiQuyetToan = 0;

    var arrHMParent = arrHangMuc.filter(function (x) { return (x.iID_ParentID == "" || x.iID_ParentID == null) && x.iID_DuAn_ChiPhi == idChiPhi });
    if (arrHMParent != null && arrHMParent.length > 0) {
        arrHMParent.forEach(x => {
            if (x.fTienPheDuyet != null || x.fTienPheDuyet != "") {
                sumGiaTriQuyetToanAB += x.fGiaTriQuyetToanAB;
                sumGiaTriKiemToan += x.fGiaTriKiemToan;
                sumGiaTriDeNghiQuyetToan += x.fGiaTriDeNghiQuyetToan;
            }
        });
    }

    // ipdate gia tri cho chiphi
    var objChiPhi = arrChiPhi.filter(x => x.iID_DuAn_ChiPhi == idChiPhi)[0];

    var fChenhLenhSoVoiDuToan = sumGiaTriDeNghiQuyetToan - objChiPhi.fTienPheDuyet;
    var fChenhLenhSoVoiQuyetToanAB = sumGiaTriDeNghiQuyetToan - sumGiaTriQuyetToanAB;
    var fChenhLechSoVoiKetQuaKiemToan = sumGiaTriDeNghiQuyetToan - sumGiaTriKiemToan;

    $('*[data-id="' + idChiPhi + '"]').find('.txtGiaTriQuyetToanAB').val(FormatNumber(sumGiaTriQuyetToanAB));
    $('*[data-id="' + idChiPhi + '"]').find('.txtKetQuaKiemToan').val(FormatNumber(sumGiaTriKiemToan));
    $('*[data-id="' + idChiPhi + '"]').find('.txtCDTDeNghiQuyetToan').val(FormatNumber(sumGiaTriDeNghiQuyetToan));

    $('*[data-id="' + idChiPhi + '"]').find(".txtChenhLechSoVoiDuToan").html(FormatNumber(fChenhLenhSoVoiDuToan));
    $('*[data-id="' + idChiPhi + '"]').find(".txtChenhLechSoVoiQuyetToanAB").html(FormatNumber(fChenhLenhSoVoiQuyetToanAB));
    $('*[data-id="' + idChiPhi + '"]').find(".txtChenhLechSoVoiKetQuaKiemToan").html(FormatNumber(fChenhLechSoVoiKetQuaKiemToan));

    objChiPhi.fGiaTriQuyetToanAB = sumGiaTriQuyetToanAB;
    objChiPhi.fGiaTriKiemToan = sumGiaTriKiemToan;
    objChiPhi.fGiaTriDeNghiQuyetToan = sumGiaTriDeNghiQuyetToan;

    objChiPhi.fChenhLenhSoVoiDuToan = fChenhLenhSoVoiDuToan;
    objChiPhi.fChenhLenhSoVoiQuyetToanAB = fChenhLenhSoVoiQuyetToanAB;
    objChiPhi.fChenhLechSoVoiKetQuaKiemToan = fChenhLechSoVoiKetQuaKiemToan;

    arrChiPhi = arrChiPhi.filter(x => x.iID_DuAn_ChiPhi != objChiPhi.iID_DuAn_ChiPhi);
    arrChiPhi.push(objChiPhi);

    CalculateDataChiPhi(objChiPhi.iID_DuAn_ChiPhi);
}

function CalculateDataChiPhi(itemId) {
    var objItem = arrChiPhi.find(x => x.iID_DuAn_ChiPhi == itemId);
    if (objItem == undefined) {
        return;
    }
    if (objItem.iID_ChiPhi_Parent != null || objItem.iID_ChiPhi_Parent != "") {
        var objParentItem = arrChiPhi.find(x => x.iID_DuAn_ChiPhi == objItem.iID_ChiPhi_Parent);
        if (objParentItem != null) {
            var arrChildSameParent = arrChiPhi.filter(function (x) { return x.iID_ChiPhi_Parent == objItem.iID_ChiPhi_Parent && x.iID_ChiPhi_Parent != "" && x.iID_ChiPhi_Parent != null });
            if (arrChildSameParent != null && arrChildSameParent.length > 0) {
                CalculateTotalParent(objParentItem, arrChildSameParent);
            }
        }

        CalculateDataChiPhi(objItem.iID_ChiPhi_Parent);
    }
}
