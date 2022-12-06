var dicFile = [];
var ERROR = 1;

//#region Add file Client
function ResetChangePage(iCurrentPage = 1) {
    var iThang = "";
    var sTenFile = "";
    var sMoTa = "";

    //SetValueFormExport(sSoChungTu, sNoiDung, sMaLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo);

    GetListData(iThang, sTenFile, sMoTa, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var iThang = $("#iThangSearch").val();
    var sTenFile = $("#txtTenFileSearch").val();
    var sMoTa = $("#txtMoTaSearch").val();

    //SetValueFormExport(sSoChungTu, sNoiDung, sMaLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo);

    GetListData(iThang, sTenFile, sMoTa, iCurrentPage);
}

function GetListData(iThang, sTenFile, sMoTa, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/BaoHiemXaHoi/QLBenhNhanBHXH/ImportFileBHXHListView",
        data: { iThang: iThang, sTenFile: sTenFile, sMoTa: sMoTa, _paging: _paging},
        success: function (data) {
            $("#lstDataView").html(data);
            $("#iThangSearch").val(iThang);
            $("#txtTenFileSearch").val(sTenFile);
            $("#txtMoTaSearch").val(sMoTa);
            $("#btnFooterFile").addClass("hidden");
            dicFile = [];
            //SetValueFormExport(sSoChungTu, sNoiDung, sMaLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo);
        }
    });
}

function AddFile() {
    if (!ValidateXML()) {
        return false;
    }
    var fileInput = document.getElementById('FileUpload');
    dicFile.push({ key: $("#iThang").val(), file: fileInput.files[0], sMoTa: $("#sMoTa").val() });

    var htmlTxt = [];
    dicFile.forEach(function (value, index) {
        var txt =  "<tr style='cursor: pointer'>"
            + "<td align='center'>"+ (index + 1) + "</td>"
            + "<td align='center'>" + value.key + "</td>"
            + "<td align='left'>" + value.file.name + "</td>"
            + "<td align='left'>" +value.sMoTa+ "</td>"
            + "<td align='center' style='vertical-align: middle !important' class='col-sm-12'>"
            + "    <button type='button' class='btn-delete' onclick='DeleteItem(" + value.key + ")'><i class='fa fa-trash-o fa-lg' aria-hidden='true'></i></button>"
            + "</td>"
            + "</tr>"
        htmlTxt.push(txt);
    });
    $("#lstFile").html(htmlTxt.join());
    $("#iThang").val("");
    $("#FileUpload").val("");
    $("#sMoTa").val("");
    $("#btnFooterFile").removeClass("hidden");
}

function DeleteItemBNSai(iLine, iID_ImportID) {
    $("#" + iLine + "_" + iID_ImportID).remove();
}

function DeleteItem(iThang) {
    dicFile = dicFile.filter(obj => obj.key != iThang);
    if (dicFile.length == 0) {
        $("#btnFooterFile").addClass("hidden");
    }
    var htmlTxt = [];
    dicFile.forEach(function (value, index) {
        var txt = "<tr style='cursor: pointer'>"
            + "<td align='center'>" + (index + 1) + "</td>"
            + "<td align='center'>" + value.key + "</td>"
            + "<td align='left'>" + value.file.name + "</td>"
            + "<td align='left'>" + value.sMoTa + "</td>"
            + "<td align='center' style='vertical-align: middle !important' class='col-sm-12'>"
            + "    <button type='button' class='btn-delete' onclick='DeleteItem(" + value.key + ")'><i class='fa fa-trash-o fa-lg' aria-hidden='true'></i></button>"
            + "</td>"
            + "</tr>"
        htmlTxt.push(txt);
    });
    $("#lstFile").html(htmlTxt.join());
}

function Cancel() {
    $("#btnFooterFile").addClass("hidden");
}

function SubmitFile() {
    var formData = new FormData();
    dicFile.forEach(function (obj, i) {
        formData.append(obj.key + '|' + obj.sMoTa , obj.file);
    });
    $.ajax({
        type: "POST",
        url: "/BaoHiemXaHoi/QLBenhNhanBHXH/LoadDataXML",
        data: formData,
        contentType: false,
        processData: false,
        cache: false,
        async: false,
        success: function (data) {
            $("#lstDataView").html(data);
            $("#cardSearch").addClass("hidden");
            $("#btnFooterFile").addClass("hidden");
            $("#btnFooterSave").removeClass("hidden");
            $('.nav-tabs a[href="#danhsachDung"]').tab('show');
        }
    });
}

function ValidateXML() {
    var Title = 'Lỗi lấy dữ liệu từ file XML';
    var Messages = [];

    var has_file = $("#FileUpload").val() != '';
    if (!has_file) {
        Messages.push("Đ/c chưa chọn file XML dữ liệu !");
    }
    else {
        var ext = $("#FileUpload").val().split(".");
        ext = ext[ext.length - 1].toLowerCase();
        var arrayExtensions = ["xml"];
        if (arrayExtensions.lastIndexOf(ext) == -1) {
            Messages.push("Đ/c chọn file không đúng định dạng !");
        }
    }

    var iThang = $("#iThang").val();
    if (iThang == null || iThang == undefined || iThang == '') {
        Messages.push("Đ/c chưa chọn tháng !");
    }

    var objThang = dicFile.find((o) => { return o["key"] === iThang })

    if (objThang != null && objThang != undefined) {
        Messages.push("Đ/c không chọn trùng tháng !");
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
//#endregion Add file Client

//#region Save data file XML to database
function GoBack() {
    window.location.href = "/BaoHiemXaHoi/QLBenhNhanBHXH";
}

function SaveData() {

    var lstIdImport = $("#lstIdImport").val();

    $.ajax({
        type: "POST",
        url: "/BaoHiemXaHoi/QLBenhNhanBHXH/BenhNhanBHXHSave",
        data: { lstBenhNhanUpDate: lstDataUpdate, lstIdImport: lstIdImport/*, sMaDonViBHXH: sMaDonViBHXH*//*, iThang: iThang, sMoTa: sMoTa*/ },
        success: function (r) {
            if (r.bIsComplete) {
                if (r.numberBNLoi > 0) {
                    window.location.href = "/BaoHiemXaHoi/QLBenhNhanBHXH/DanhSachBenhNhanLoi";
                }
                else {
                    window.location.href = "/BaoHiemXaHoi/QLBenhNhanBHXH";
                }
            } else {
                var Title = 'Lỗi lưu bệnh nhân BHXH';
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

function ValidateData(danhsach, danhSachDung/*, sMaDonViBHXH, iThang*/) {
    var checkMaDonVi = true;
    var checkBenhNhan = true;
    //var checkNgayThang = true;
    //var checkNumber = true;
    var Title = 'Lỗi import danh sách bệnh nhân điều trị BHXH';
    var Messages = [];
    var lstMaDonViBHXH = $("#lstMaDonVi").val();
    //var lstMaDonViBHXH = ($("#lstMaDonVi").val()).split(',');
    //if (sMaDonViBHXH == null || sMaDonViBHXH == undefined || sMaDonViBHXH == '') {
    //    Messages.push("Đơn vị BHXH chưa chọn !");
    //}

    //if (iThang == null || iThang == undefined || iThang == '') {
    //    Messages.push("Tháng chưa chọn !");
    //}

    $.each(danhsach, function (index, value) {
        if (lstMaDonViBHXH.indexOf(value.iID_MaDonVi) == -1) {
            checkMaDonVi = false;
        }

        //if ($.inArray(value.iID_MaDonVi, lstMaDonViBHXH) == -1) {
        //    checkMaDonVi = false
        //}

        //if (danhsach.filter(x => x.sMaThe == value.sMaThe && x.sNgayRaVien == value.sNgayRaVien && x.sNgayVaoVien == value.sNgayVaoVien).length > 1) {
        //    checkBenhNhan = false;
        //}

        //if (danhSachDung.filter(x => x.sMaThe == value.sMaThe && x.sNgayRaVien == value.sNgayRaVien && x.sNgayVaoVien == value.sNgayVaoVien).length > 0) {
        //    checkBenhNhan = false;
        //}

        //if (!CheckDateTime(value.sNgaySinh) || !CheckDateTime(value.sNgayRaVien) || !CheckDateTime(value.sNgayVaoVien)) {
        //    checkNgayThang = false;
        //}

        //if (!CheckNumber(value.iSTT) || !CheckNumber(value.iSoNgayDieuTri)) {
        //    checkNumber = false;
        //}
    });

    if (!checkMaDonVi) {
        Messages.push("Không tồn tại mã đơn vị !");
    }

    //if (!checkBenhNhan) {
    //    Messages.push("Trùng bệnh nhân !");
    //}

    //if (!checkNgayThang) {
    //    Messages.push("Ngày tháng không đúng định dạng !");
    //}

    //if (!checkNumber) {
    //    Messages.push("Số thứ tự hoặc số ngày nằm viện không đúng định dạng !");
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

function CheckDateTime(sDateTime) {
    if (sDateTime.length != 8) {
        return false;
    }

    var year = sDateTime.substring(0, 4);
    var month = sDateTime.substring(4, 6);
    var day = sDateTime.substring(6, 8);

    if (!CheckNumber(year) || !CheckNumber(month) || !CheckNumber(day)) {
        return false;
    }

    var d = new Date(year, month - 1, day);

    if (!(d && (d.getMonth() + 1) == month)) {
        return false;
    }

    return true;
}

function CheckNumber(sNumber) {
    if ($.isNumeric(sNumber) && Math.floor(sNumber) == sNumber && sNumber >= 0) {
        return true;
    }
    else {
        return false;
    }
}
//#endregion Save data file XML to database

function GetDataTemp(iCurrentPage = 1) {
    $.ajax({
        type: "POST",
        url: "/BaoHiemXaHoi/QLBenhNhanBHXH/LoadDataTable",
        data: { lstImportId: lstImportId, iPage: iCurrentPage },
        success: function (data) {
            if (data.bIsComplete) {
                RenderDataTable(data.lstDataTable, data.lstError);
            }
        }
    });
}

function RenderDataTable(lstDanhSach, lstError) {
    var htmlTableDung = [];
    var htmlTableSai = [];
    lstDanhSach.forEach(function (value, index) {
        if (value.bError) {
            var currentErros = $.map(lstError.filter(obj => obj.iLine == value.iLine && obj.iID_ImportID == value.iID_ImportID), function (obj) { return obj.sPropertyName;});

            var txtBNSai = "<tr id='" + (value.iLine + "_" + value.iID_ImportID) + "'>"
                + "<td align='center'><input type='text' class='form-control iSTT " + (currentErros.indexOf("STT") != -1 ? "error" : "") +"' value='" + value.sSTT + "' maxlength='10' autocomplete='off' /></td>"
                + "<td align='center'><input type='text' class='form-control sTen " + (currentErros.indexOf("ho_ten") != -1 ? "error" : "") +"' value='" + value.sHoTen + "' maxlength='100' autocomplete='off' /></td>"
                + "<td align='center'><input type='text' class='form-control sNgaySinh " + (currentErros.indexOf("NGAYSINH") != -1 ? "error" : "") +"' value='" + value.sNgaySinh + "' maxlength='8' autocomplete='off' /></td>"
                + "<td align='center'><input type='text' class='form-control sGioiTinh " + (currentErros.indexOf("GIOITINH") != -1 ? "error" : "") +"' value='" + value.sGioiTinh + "' maxlength='10' autocomplete='off' /></td>"
                + "<td align='center'><input type='text' class='form-control sCapBac " + (currentErros.indexOf("CAPBAC") != -1 ? "error" : "") +"' value='" + value.sCapBac + "' maxlength='100' autocomplete='off' /></td>"
                + "<td align='center'><input type='text' class='form-control sMaThe " + (currentErros.indexOf("MATHE") != -1 ? "error" : "") +"' value='" + value.sMaThe + "' maxlength='15' autocomplete='off' /></td>"
                + "<td align='center'><input type='text' class='form-control sNgayVaoVien " + (currentErros.indexOf("NGAY_VAO_VIEN") != -1 ? "error" : "") +"' value='" + value.sNgayVaoVien + "' maxlength='8' autocomplete='off' /></td>"
                + "<td align='center'><input type='text' class='form-control sNgayRaVien " + (currentErros.indexOf("NGAY_RA_VIEN") != -1 ? "error" : "") +"' value='" + value.sNgayRaVien + "' maxlength='8' autocomplete='off' /></td>"
                + "<td align='center'><input type='text' class='form-control sSoNgayDieuTri " + (currentErros.indexOf("SO_NGAY_DTRI") != -1 ? "error" : "") +"' value='" + value.sSoNgayDieuTri + "' maxlength='10' autocomplete='off' /></td>"
                + "<td align='center'><input type='text' class='form-control sMaBV " + (currentErros.indexOf("MABV") != -1 ? "error" : "") +"' value='" + value.sMaBV + "' maxlength='5' autocomplete='off' /></td>"
                + "<td align='center'><input type='text' class='form-control sTenBenhVien " + (currentErros.indexOf("TENBV") != -1 ? "error" : "") +"' value='" + value.sTenBV + "' maxlength='100' autocomplete='off' /></td>"
                + "<td align='center'><input type='text' class='form-control sMaDV " + (currentErros.indexOf("MADV") != -1 ? "error" : "") +"' value='" + value.sMaDV + "' maxlength='10' autocomplete='off' /></td>"
                + "<td align='center'><input type='text' class='form-control sTenDonVi " + (currentErros.indexOf("TENDV") != -1 ? "error" : "") +"' value='" + value.sTenDonVi + "' maxlength='200' autocomplete='off' /></td>"
                + "<td align='center'><input type='text' class='form-control sGhiChu " + (currentErros.indexOf("GHICHU") != -1 ? "error" : "") +"' value='" + value.sGhiChu + "' maxlength='100' autocomplete='off' /></td>"
                + "<td><button type='button' class='btn-delete' onclick=\"DeleteItemBNSai(" + value.iLine + ", '"+value.iID_ImportID+"')\"><i class='fa fa-trash-o fa-lg' aria-hidden='true'></i></button></td>";
            htmlTableSai.push(txtBNSai);
        }
        else {
            var txt = "<tr id='" + (value.iLine + "_" + value.iID_ImportID) + "'>"
                + "<td align='center' class='iSTT'>" + value.sSTT + "</td>"
                + "<td align='left' class='sTen'>" + value.sHoTen + "</td>"
                + "<td align='center' class='sNgaySinh'>" + value.sNgaySinh + "</td>"
                + "<td align='center' class='sGioiTinh'>" + value.sGioiTinh + "</td>"
                + "<td align='left' class='sCapBac'>" + value.sCapBac + "</td>"
                + "<td align='left' class='sMaThe'>" + value.sMaThe + "</td>"
                + "<td align='center' class='sNgayVaoVien'>" + value.sNgayVaoVien + "</td>"
                + "<td align='center' class='sNgayRaVien'>" + value.sNgayRaVien + "</td>"
                + "<td align='center' class='sSoNgayDieuTri'>" + value.sSoNgayDieuTri + "</td>"
                + "<td align='left' class='sMaBV'>" + value.sMaBV + "</td>"
                + "<td align='left' class='sTenBenhVien'>" + value.sTenBV + "</td>"
                + "<td align='left' class='sMaDV'>" + value.sMaDV + "</td>"
                + "<td align='left' class='sTenDonVi'>" + value.sTenDonVi + "</td>"
                + "<td align='left' class='sGhiChu'>" + value.sGhiChu + "</td>"
                + "</tr>";
            htmlTableDung.push(txt);
        }
    });
    $("#tBodyDung").append(htmlTableDung.join());
    $("#tBodySai").append(htmlTableSai.join());
    $("#tBodySai .form-control").blur(function (e) {
        ChangeDataItem(this);
    });

}

function ChangeDataItem(item) {
    var row = item.closest("tr");
    var iLine = $(row).attr('id').split("_")[0];
    var iIdImportId = $(row).attr('id').split("_")[1];
    lstDataUpdate.find((o) => { return o.iLine != iLine && o.iID_ImportID == iIdImportId });

    var obj = {};
    obj.sSTT = $(row).find(".iSTT").val();
    obj.sHoTen = $(row).find(".sTen").val();
    obj.sNgaySinh = $(row).find(".sNgaySinh").val();
    obj.sGioiTinh = $(row).find(".sGioiTinh").val();
    obj.sCapBac = $(row).find(".sCapBac").val();
    obj.sMaThe = $(row).find(".sMaThe").val();
    obj.sNgayVaoVien = $(row).find(".sNgayVaoVien").val();
    obj.sNgayRaVien = $(row).find(".sNgayRaVien").val();
    obj.sSoNgayDieuTri = $(row).find(".sSoNgayDieuTri").val();
    obj.sMaBV = $(row).find(".sMaBV").val();
    obj.sTenBV = $(row).find(".sTenBenhVien").val();
    obj.sMaDV = $(row).find(".sMaDV").val();
    obj.sTenDonVi = $(row).find(".sTenDonVi").val();
    obj.sGhiChu = $(row).find(".sGhiChu").val();
    obj.iLine = iLine;
    obj.iID_ImportID = iIdImportId;
    lstDataUpdate.push(obj);
}