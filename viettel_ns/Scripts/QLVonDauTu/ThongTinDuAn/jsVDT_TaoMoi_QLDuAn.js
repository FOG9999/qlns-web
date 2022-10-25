var TBL_CHI_PHI_DAU_TU = "tblChiPhiDauTu";
var TBL_NGUON_VON_DAU_TU = "tblNguonVonDauTu";
var TBL_HANG_MUC_CHINH = "tblHangMucChinh";
var TBL_TAI_LIEU_DINH_KEM = "tblThongTinTaiLieuDinhKem";
var arrChiPhi = [];
var arrNguonVon = [];
var arrHangMuc = [];
var trLoaiCongTrinh;
var typeSearch = 1;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;

$(document).ready(function ($) {
    var fHanMucDauTu = FormatNumber($("#fHanMucDauTu").val());
    $("#fHanMucDauTu").val(fHanMucDauTu);

    LoadDataComboBoxNguonNganSach();
    LoadDataComboBoxLoaiCongTrinh();

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

    $("#sTenDuAn").keyup(function (event) {
        ValidateMaxLength(this, 2000);
    });

    $("#sDiaDiem").keyup(function (event) {
        ValidateMaxLength(this, 300);
    });

    $("#sMucTieu").keyup(function (event) {
        ValidateMaxLength(this, 2000);
    });

    $("#sQuyMo").keyup(function (event) {
        ValidateMaxLength(this, 2000);
    });

    $("#sKhoiCong").keydown(function (event) {
        ValidateInputKeydown(event, this, 1);
    }).blur(function (event) {
        ValidateInputFocusOut(event, this, 6);
    });

    $("#sHoanThanh").keydown(function (event) {
        ValidateInputKeydown(event, this, 1);
    }).blur(function (event) {
        ValidateInputFocusOut(event, this, 6);
    });
});


/*NinhNV start*/
function BtnCreateDataClick() {
    window.location.href = "/QLVonDauTu/QLDuAn/CreateNew/";
}

function SearchData(iCurrentPage = 1) {
    var soChungTu = $("#txtSoChungTu").val();
    var noiDung = $("#txtNoiDung").val();
    GetListData(soChungTu, noiDung, iCurrentPage);
}

function ChangePage(iCurrentPage) {
    var soChungTu = $("#txtSoChungTu").val();
    var noiDung = $("#txtNoiDung").val();
    soChungTu = "1";
    GetListData(soChungTu, noiDung, iCurrentPage);
}

function GetListData(soChungTu, noiDung, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { soChungTu: soChungTu, noiDung: noiDung, _paging: _paging },
        success: function (data) {
            $("#lstDataView").html(data);
        }
    });
}

function LuuDuAn() {
    if (CheckLoi()) {
        var duAn = {};
        var chuTruongDauTu = {};
        var quyetDinhDauTu = {};
        var data = {};

        duAn.iID_DuAnID = $("#iID_DuAnID").val();
        duAn.iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
        duAn.sTenDuAn = $.trim($("#sTenDuAn").val());
        duAn.sMaDuAn = $("#sMaDuAn").val();
        duAn.iID_ChuDauTuID = $("#iID_ChuDauTuID").val();
        duAn.iID_CapPheDuyetID = $("#iID_CapPheDuyetID").val();
        //duAn.iID_LoaiCongTrinhID = $("#iID_LoaiCongTrinhID").val();
        duAn.iID_LoaiCongTrinhID = $('#hid_iID_LoaiCongTrinhID').val();
        duAn.sDiaDiem = $.trim($("#sDiaDiem").val());
        duAn.sSuCanThietDauTu = $("#sSuCanThietDauTu").val();
        duAn.sMucTieu = $.trim($("#sMucTieu").val());
        duAn.sQuyMo = $.trim( $("#sQuyMo").val());
        duAn.sKhoiCong = $("#sKhoiCong").val();
        duAn.sKetThuc = $("#sKetThuc").val();
        duAn.iID_NhomDuAnID = $("#iID_NhomDuAnID").val();
        duAn.iID_HinhThucQuanLyID = $("#iID_HinhThucQuanLyID").val();
        duAn.fHanMucDauTu = UnFormatNumber($("#fHanMucDauTu").val());
        duAn.bIsDuPhong = $("#bIsDuPhong").val() == 'on' ? true : false;

        if (duAn.iID_DuAnID === '00000000-0000-0000-0000-000000000000') {
            //chuTruongDauTu.sSoQuyetDinh = $("#sSoQuyetDinhCTDT").val();
            //chuTruongDauTu.dNgayQuyetDinh = $("#dNgayQuyetDinhCTDT").val();
            //chuTruongDauTu.sCoQuanPheDuyet = $("#sCoQuanPheDuyetCTDT").val();
            //chuTruongDauTu.sNguoiKy = $("#sNguoiKyCTDT").val();

            //quyetDinhDauTu.sSoQuyetDinh = $("#sSoQuyetDinhQDDT").val();
            //quyetDinhDauTu.dNgayQuyetDinh = $("#dNgayQuyetDinhQDDT").val();
            //quyetDinhDauTu.sCoQuanPheDuyet = $("#sCoQuanPheDuyetQDDT").val();
            data = {
                duAn: duAn,
                //chuTruongDauTu: chuTruongDauTu,
                //quyetDinhDauTu: quyetDinhDauTu,
                listChuTruongDauTuNguonVon: arrNguonVon,
                listDuAnHangMuc: arrHangMuc
            };
        } else {
            data = {
                duAn: duAn
            };
        }

        $.ajax({
            type: "POST",
            url: "/QLDuAn/CheckExistMaDuAn",
            data: { iID_DuAnID: duAn.iID_DuAnID, sMaDuAn: duAn.sMaDuAn },
            success: function (r) {
                if (r == "True") {
                    var Title = 'Lỗi lưu dự án';
                    var messErr = [];
                    messErr.push('Mã dự án đã tồn tại!');
                    $.ajax({
                        type: "POST",
                        url: "/Modal/OpenModal",
                        data: { Title: Title, Messages: messErr, Category: ERROR },
                        success: function (data) {
                            $("#divModalConfirm").html(data);
                        }
                    });
                    return false;
                } else {
                    $.ajax({
                        type: "POST",
                        url: "/QLDuAn/TTQLDuAnSave",
                        data: { data: data },
                        success: function (r) {
                            if (r == "True") {
                                window.location.href = "/QLVonDauTu/QLDuAn/Index";
                            }
                        }
                    });
                }
            }
        });
    }
}

function CheckLoi() {
    var sMaDuAn = $("#sMaDuAn").val();

    var messErr = [];
    if ($("#iID_DonViQuanLyID").val() == GUID_EMPTY) {
        messErr.push("Chưa chọn đơn vị quản lý!");
    }
    //if (sMaDuAn == '') {
    //    messErr.push("Mã dự án chưa được nhập!");
    //}
    if ($("#sTenDuAn").val() == '') {
        messErr.push("Tên dự án chưa được nhập!");
    }
    if ($("#iID_CapPheDuyetID").val() == GUID_EMPTY) {
        messErr.push("Chưa chọn phân cấp phê duyệt!");
    }

    if ($("#iID_DuAnID").val() === GUID_EMPTY) {
        if (arrNguonVon.length == 0) {
            messErr.push("Thông tin nguồn vốn đầu tư chưa có!");
        }
    }

    /*Check thông tin hạng mục - Kiểm tra nếu hạng mục có hạng mục con thì check hạn mức đầu tư cha = sum(hạn mức đầu tư con) */
    var tr_parent = $('#tblHangMucChinh').find('.parent');
    var messErrorHanMuc = []
    if (tr_parent.length > 0) {
        for (i = 0; i < tr_parent.length; i++) {
            var idHangMucCha = $(tr_parent[i]).find('.r_iID_DuAn_HangMucID').val();
            var sHanMucDauTu = $(tr_parent[i]).find('.fHanMucDauTuCha').text();
            var fHanMucDauTu = parseInt(UnFormatNumber(sHanMucDauTu));
            var sTenHangMuc = $(tr_parent[i]).find('.sTenHangMuc').text();
            LayHanMucDauTuCon(idHangMucCha, fHanMucDauTu, messErr, sTenHangMuc, messErrorHanMuc);
            if (messErrorHanMuc.length > 0) {
                break;
            }
        }
    }

    if (messErr.length > 0) {
        var Title = 'Lỗi lưu dự án';
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: messErr, Category: ERROR },
            success: function (data) {
                $("#divModalConfirm").html(data);
            }
        });
        return false;
    } else {
        return true;
    }
}

function LayHanMucDauTuCon(idHangMucCha, fHanMucDauTuCha, messErr, sTenHangMuc, messErrorHanMuc) {
    var tr_child = $('tr[data-idparent=' + idHangMucCha + ']');
    var hanMucDauTuTong = 0;
    if (tr_child.length > 0) {
        for (j = 0; j < tr_child.length; j++) {
            var sHanMucDauTu = $(tr_child[j]).find(".txtHanMucDauTu").val();
            var fHanMucDauTu = parseInt(UnFormatNumber(sHanMucDauTu));
            hanMucDauTuTong = hanMucDauTuTong + fHanMucDauTu;
        }
        if (hanMucDauTuTong == fHanMucDauTuCha) {
            for (j = 0; j < tr_child.length; j++) {
                if (j < tr_child.length) {
                    var i_child = $(tr_child[j]).find('.r_iID_DuAn_HangMucID').val();
                    var sHanMucDauTu = $(tr_child[j]).find(".txtHanMucDauTu").val();
                    var fHanMucDauTu = parseInt(UnFormatNumber(sHanMucDauTu));
                    LayHanMucDauTuCon(i_child, fHanMucDauTu, messErr);
                }
                else {
                    return;
                }           
            }
        }
        else {
            messErr.push("Hạng mục " + sTenHangMuc + " có hạng mục con, hạn mức đầu tư (cha) không bằng tổng (các hạng mức đầu tư con)");
            messErrorHanMuc.push(1);
            return;
        }
    }
}

function CancelSaveData() {
    window.location.href = "/QLVonDauTu/QLDuAn/Index";
}
/*NinhNV end*/

// ComboBox Nguồn ngân sách của nguồn vốn
var arr_NguonNganSach = [];
function LoadDataComboBoxNguonNganSach() {
    $.ajax({
        url: "/QLVonDauTu/QLDuAn/GetNguonVonAll",
        type: "POST",
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data.status == false) {
                return;
            }

            if (data.data != null)
                arr_NguonNganSach = data.data;
        }
    });
}

function CreateHtmlSelectNguonVon(value) {
    var htmlOption = "";
    arr_NguonNganSach.forEach(x => {
        if (value != undefined && value == x.id)
            htmlOption += "<option value='" + x.id + "' selected>" + x.text + "</option>";
        else
            htmlOption += "<option value='" + x.id + "'>" + x.text + "</option>";
    })
    return "<select class='form-control'>" + htmlOption + "</option>";
}

function ThemMoiNguonVonDauTu() {
    var dongMoi = "";
    dongMoi += "<tr style='cursor: pointer;' class='parent'>";
    dongMoi += "<td class='r_STT'></td><input type='hidden' class='r_iID_DuAn_NguonVonID' value=''/>";
    dongMoi += "<td><input type='hidden' class='r_iID_NguonVon' value=''/><div class='sTenNguonVon' hidden></div><div class='selectNguonVon'>" + CreateHtmlSelectNguonVon() + "</div></td>";
    dongMoi += "<td class='r_GiaTriPheDuyet' align='right'><div class='fGiaTriPheDuyet' hidden></div><input type='text' class='form-control txtGiaTriPheDuyet' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);'/></td>";
    dongMoi += "<td align='center'>";
    dongMoi += "<button class='btn-save btn-icon' type = 'button' onclick = 'Luu(this, \"" + TBL_NGUON_VON_DAU_TU + "\")' > " +
        "<i class='fa fa-floppy-o fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-edit btn-icon' hidden type = 'button' onclick = 'Sua(this, \"" + TBL_NGUON_VON_DAU_TU + "\")' > " +
        "<i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-delete btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_NGUON_VON_DAU_TU + "\")'>" +
        "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
        "</button></td>";
    dongMoi += "</tr>";

    $("#" + TBL_NGUON_VON_DAU_TU + " tbody").append(dongMoi);
    CapNhatCotStt(TBL_NGUON_VON_DAU_TU);

    $(".selectNguonVon select").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
}

var arr_LoaiCongTrinh = [];
function LoadDataComboBoxLoaiCongTrinh() {
    $.ajax({
        url: "/QLVonDauTu/QLDuAn/GetLoaiCongTrinh",
        type: "POST",
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data.status == false) {
                return;
            }

            if (data.data != null)
                arr_LoaiCongTrinh = data.data;
        }
    });
}

function CreateHtmlSelectLoaiCongTrinh(value) {
    var htmlOption = "";
    arr_LoaiCongTrinh.forEach(x => {
        if (value != undefined && value == x.id)
            htmlOption += "<option value='" + x.id + "' selected>" + x.text + "</option>";
        else
            htmlOption += "<option value='" + x.id + "'>" + x.text + "</option>";
    })
    return "<select class='form-control'>" + htmlOption + "</option>";
}

function ThemMoiHangMuc() {
    var dongMoi = "";
    dongMoi += "<tr style='cursor: pointer;' class='parent'>";
    dongMoi += "<td class='r_STT'></td><input type='hidden' class='r_iID_DuAn_HangMucID' value=''/>";
    dongMoi += "<td class='r_sTenHangMuc'><div class='sTenHangMuc' hidden></div><input type='text' class='form-control txtTenHangMuc'/></td>"
    dongMoi += "<td><input type='hidden' class='r_iID_LoaiCongTrinhID' value=''/><div class='sTenLoaiCongTrinh' hidden></div><div class='selectLoaiCongTrinh'>" + CreateHtmlSelectLoaiCongTrinh() + "</div></td>";
    dongMoi += "<td class='r_HanMucDauTu' align='right'><div class='fHanMucDauTu fHanMucDauTuCha' hidden></div><input type='text' class='form-control txtHanMucDauTu' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);'/></td>";
    dongMoi += "<td align='center'>";

    dongMoi += "<button class='btn-add-child btn-icon' hidden type = 'button' onclick = 'ThemMoiHangMucCon(this)' > " +
        "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" +
        "</button> ";

    dongMoi += "<button class='btn-save btn-icon' type = 'button' onclick = 'Luu(this, \"" + TBL_HANG_MUC_CHINH + "\")' > " +
        "<i class='fa fa-floppy-o fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-edit btn-icon' hidden type = 'button' onclick = 'Sua(this, \"" + TBL_HANG_MUC_CHINH + "\")' > " +
        "<i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-delete btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_HANG_MUC_CHINH + "\")'>" +
        "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
        "</button></td>";
    dongMoi += "</tr>";

    $("#" + TBL_HANG_MUC_CHINH + " tbody").append(dongMoi);
    CapNhatCotStt(TBL_HANG_MUC_CHINH);

    $(".txtTenHangMuc").keyup(function (event) {
        ValidateMaxLength(this, 300);
    });

    $(".selectLoaiCongTrinh select").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    TrimSpaceAllTextInput();
}

function ThemMoiHangMucCon(nutThem) {
    var dongHienTai = nutThem.parentElement.parentElement;
    var iIDDuAnHangMucID = $(dongHienTai).find(".r_iID_DuAn_HangMucID").val();
    var dongMoi = "";
    dongMoi += "<tr style='cursor: pointer;' class='child' data-idparent='" + iIDDuAnHangMucID + "'>";
    dongMoi += "<td class='r_STT'></td><input type='hidden' class='r_iID_DuAn_HangMucID' value=''/>";
    dongMoi += "<td class='r_sTenHangMuc'><div class='sTenHangMuc' hidden></div><input type='text' class='form-control txtTenHangMuc'/></td>"
    dongMoi += "<td><input type='hidden' class='r_iID_LoaiCongTrinhID' value=''/><div class='sTenLoaiCongTrinh' hidden></div><div class='selectLoaiCongTrinh'>" + CreateHtmlSelectLoaiCongTrinh() + "</div></td>";
    dongMoi += "<td class='r_HanMucDauTu' align='right'><div class='fHanMucDauTu fHanMucDauTuCon' hidden></div><input type='text' class='form-control txtHanMucDauTu' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);'/></td>";
    dongMoi += "<td align='center'>";

    dongMoi += "<button class='btn-add-child btn-icon' hidden type = 'button' onclick = 'ThemMoiHangMucCon(this)' > " +
        "<i class='fa fa-plus fa-lg' aria-hidden='true'></i>" +
        "</button> ";

    dongMoi += "<button class='btn-save btn-icon' type = 'button' onclick = 'Luu(this, \"" + TBL_HANG_MUC_CHINH + "\")' > " +
        "<i class='fa fa-floppy-o fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-edit btn-icon' hidden type = 'button' onclick = 'Sua(this, \"" + TBL_HANG_MUC_CHINH + "\")' > " +
        "<i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i>" +
        "</button> ";
    dongMoi += "<button class='btn-delete btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_HANG_MUC_CHINH + "\")'>" +
        "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
        "</button></td>";
    dongMoi += "</tr>";

    /* Lấy dòng con có STT lớn nhất */
    var tr_child = $('tr[data-idparent=' + iIDDuAnHangMucID + ']');
    var dongConCuoiCung = tr_child[tr_child.length - 1];
    if (tr_child.length > 0) {
        $(dongConCuoiCung).after(dongMoi);
    }
    else {
        $(dongHienTai).after(dongMoi);
    }

    CapNhatCotSttCon(TBL_HANG_MUC_CHINH);
    $(".txtTenHangMuc").keyup(function (event) {
        ValidateMaxLength(this, 300);
    });

    $(".selectLoaiCongTrinh select").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    TrimSpaceAllTextInput();
}

// event
function Luu(nutLuu, idBang) {
    var dongHienTai = nutLuu.parentElement.parentElement;
    if (idBang == TBL_NGUON_VON_DAU_TU) {
        var messErr = [];
        var iIDDuAnNguonVonID = $(dongHienTai).find(".r_iID_DuAn_NguonVonID").val();
        var iIDNguonVonID = $(dongHienTai).find(".selectNguonVon select").val();
        var sTenNguonVon = $(dongHienTai).find(".selectNguonVon select :selected").html();
        var sGiaTriPheDuyet = $(dongHienTai).find(".txtGiaTriPheDuyet").val();
        var fGiaTriPheDuyet = parseInt(UnFormatNumber(sGiaTriPheDuyet));

        if (iIDDuAnNguonVonID == "")
            iIDDuAnNguonVonID = uuidv4();

        if (iIDNguonVonID == 0 || iIDNguonVonID == "") {
            messErr.push("Thông tin nguồn vốn chưa có hoặc chưa chính xác.");
        }

        if (arrNguonVon.filter(function (x) { return x.iID_NguonVonID == iIDNguonVonID && x.iID_DuAn_NguonVonID != iIDDuAnNguonVonID }).length > 0) {
            messErr.push("Tên nguồn vốn đã tồn tại.");
        }

        if (isNaN(fGiaTriPheDuyet) || fGiaTriPheDuyet == "" || fGiaTriPheDuyet <= 0) {
            messErr.push("Giá trị phê duyệt của nguồn vốn phải lớn hơn 0.");
        }

        if (messErr.length > 0) {
            PopupModal("Lỗi", messErr, ERROR);
            return;
        }

        $(dongHienTai).find(".r_iID_DuAn_NguonVonID").val(iIDDuAnNguonVonID);
        $(dongHienTai).find(".r_iID_NguonVon").val(iIDNguonVonID);

        $(dongHienTai).find(".sTenNguonVon").html(sTenNguonVon);
        $(dongHienTai).find(".sTenNguonVon").show();
        $(dongHienTai).find(".selectNguonVon select").hide();
        $(dongHienTai).find(".selectNguonVon").hide();

        $(dongHienTai).find(".fGiaTriPheDuyet").html(sGiaTriPheDuyet);
        $(dongHienTai).find(".fGiaTriPheDuyet").show();
        $(dongHienTai).find(".txtGiaTriPheDuyet").hide();

        $(dongHienTai).find("button.btn-edit").show();
        $(dongHienTai).find("button.btn-save").hide();

        arrNguonVon = arrNguonVon.filter(function (x) { return x.iID_DuAn_NguonVonID != iIDDuAnNguonVonID });

        arrNguonVon.push({
            iID_DuAn_NguonVonID: iIDDuAnNguonVonID,
            Id: iIDDuAnNguonVonID,
            iID_NguonVonID: iIDNguonVonID,
            fThanhTien: fGiaTriPheDuyet
        })
    } else if (idBang == TBL_HANG_MUC_CHINH) {
        var messErr = [];
        var iIDDuAnHangMucID = $(dongHienTai).find(".r_iID_DuAn_HangMucID").val();
        var iIDParentID = $(dongHienTai).data("idparent");
        var sTenHangMuc = $(dongHienTai).find(".txtTenHangMuc").val();
        var iIDLoaiCongTrinhID = $(dongHienTai).find(".selectLoaiCongTrinh select").val();
        var sTenLoaiCongTrinh = $(dongHienTai).find(".selectLoaiCongTrinh select :selected").html();
        var sHanMucDauTu = $(dongHienTai).find(".txtHanMucDauTu").val();
        var fHanMucDauTu = parseInt(UnFormatNumber(sHanMucDauTu));

        if (iIDDuAnHangMucID == "")
            iIDDuAnHangMucID = uuidv4();

        if (sTenHangMuc == "") {
            messErr.push("Tên hạng mục chưa có hoặc chưa chính xác.");
        }

        if (arrHangMuc.filter(function (x) { return x.sTenHangMuc == sTenHangMuc && x.iID_DuAn_HangMucID != iIDDuAnHangMucID }).length > 0) {
            messErr.push("Tên hạng mục đã tồn tại.");
        }

        if (iIDLoaiCongTrinhID == 0 || iIDLoaiCongTrinhID == "") {
            messErr.push("Thông tin loại công trình chưa có hoặc chưa chính xác.");
        }
        if (isNaN(fHanMucDauTu) || fHanMucDauTu == "" || fHanMucDauTu <= 0) {
            messErr.push("Hạn mức đầu tư phải lớn hơn 0.");
        }
        arrHangMuc.filter(function (x) {
            if (x.iID_DuAn_HangMucID == iIDParentID) {
                if (fHanMucDauTu > x.fTienHangMuc) {
                    messErr.push("Hạn mức con vượt hạn mức cha.");
                }
            }
        })
        if (messErr.length > 0) {
            PopupModal("Lỗi", messErr, ERROR);
            return;
        }

        $(dongHienTai).find(".r_iID_DuAn_HangMucID").val(iIDDuAnHangMucID);

        $(dongHienTai).find(".sTenHangMuc").html(sTenHangMuc);
        $(dongHienTai).find(".sTenHangMuc").show();
        $(dongHienTai).find(".txtTenHangMuc").hide();

        $(dongHienTai).find(".r_iID_LoaiCongTrinhID").val(iIDLoaiCongTrinhID);
        $(dongHienTai).find(".sTenLoaiCongTrinh").html(sTenLoaiCongTrinh);
        $(dongHienTai).find(".sTenLoaiCongTrinh").show();
        $(dongHienTai).find(".selectLoaiCongTrinh select").hide();
        $(dongHienTai).find(".selectLoaiCongTrinh").hide();

        $(dongHienTai).find(".fHanMucDauTu").html(sHanMucDauTu);
        $(dongHienTai).find(".fHanMucDauTu").show();
        $(dongHienTai).find(".txtHanMucDauTu").hide();

        $(dongHienTai).find("button.btn-edit").show();
        $(dongHienTai).find("button.btn-save").hide();
        $(dongHienTai).find("button.btn-add-child").show();

        arrHangMuc = arrHangMuc.filter(function (x) { return x.iID_DuAn_HangMucID != iIDDuAnHangMucID });

        arrHangMuc.push({
            iID_DuAn_HangMucID: iIDDuAnHangMucID,
            iID_ParentID: iIDParentID,
            sTenHangMuc: sTenHangMuc,
            iID_LoaiCongTrinhID: iIDLoaiCongTrinhID,
            sTenLoaiCongTrinh: sTenLoaiCongTrinh,
            fTienHangMuc: fHanMucDauTu
        })
    }
    TinhLaiDongTong(idBang);
}

function Sua(nutSua, idBang) {
    var dongHienTai = nutSua.parentElement.parentElement;
    if (idBang == TBL_NGUON_VON_DAU_TU) {
        var messErr = [];
        var iIDDuAnNguonVonID = $(dongHienTai).find(".r_iID_DuAn_NguonVonID").val();
        var iIDNguonVonID = $(dongHienTai).find(".selectNguonVon select").val();
        var sGiaTriPheDuyet = $(dongHienTai).find(".txtGiaTriPheDuyet").val();
        var fGiaTriPheDuyet = parseInt(UnFormatNumber(sGiaTriPheDuyet));

        $(dongHienTai).find(".sTenNguonVon").hide();
        $(dongHienTai).find(".selectNguonVon").html(CreateHtmlSelectNguonVon(iIDNguonVonID));
        $(dongHienTai).find(".selectNguonVon select").show();
        $(dongHienTai).find(".selectNguonVon").show();

        $(dongHienTai).find(".fGiaTriPheDuyet").hide();
        $(dongHienTai).find(".txtGiaTriPheDuyet").show();

        $(dongHienTai).find("button.btn-edit").hide();
        $(dongHienTai).find("button.btn-save").show();

        $(dongHienTai).find(".selectNguonVon select").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
    } else if (idBang == TBL_HANG_MUC_CHINH) {
        var messErr = [];
        var iIDDuAnHangMucID = $(dongHienTai).find(".r_iID_DuAn_HangMucID").val();
        var sTenHangMuc = $(dongHienTai).find(".txtTenHangMuc").val();
        var iIDLoaiCongTrinhID = $(dongHienTai).find(".selectLoaiCongTrinh select").val();
        var sTenLoaiCongTrinh = $(dongHienTai).find(".selectLoaiCongTrinh select :selected").html();
        var sHanMucDauTu = $(dongHienTai).find(".txtHanMucDauTu").val();
        var fHanMucDauTu = parseInt(UnFormatNumber(sHanMucDauTu));

        $(dongHienTai).find(".sTenHangMuc").hide();
        $(dongHienTai).find(".txtTenHangMuc").show();

        $(dongHienTai).find(".sTenLoaiCongTrinh").hide();
        $(dongHienTai).find(".selectLoaiCongTrinh").html(CreateHtmlSelectLoaiCongTrinh(iIDLoaiCongTrinhID));
        $(dongHienTai).find(".selectLoaiCongTrinh select").show();
        $(dongHienTai).find(".selectLoaiCongTrinh").show();

        $(dongHienTai).find(".fHanMucDauTu").hide();
        $(dongHienTai).find(".txtHanMucDauTu").show();

        $(dongHienTai).find("button.btn-edit").hide();
        $(dongHienTai).find("button.btn-save").show();
        $(dongHienTai).find("button.btn-add-child").hide();

        $(dongHienTai).find(".selectLoaiCongTrinh select").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });

    }
    TinhLaiDongTong(idBang);
}

function CapNhatCotStt(idBang) {
    $("#" + idBang + " tbody tr.parent").each(function (index, tr) {
        $(tr).find('.r_STT').text(index + 1);
    });
}

function CapNhatCotSttCon(idBang) {
    $("#" + idBang + " tbody tr").each(function (index, tr) {
        var sttParent = $(tr).find('.r_STT').text();
        var iIDParentID = $(tr).find('.r_iID_DuAn_HangMucID').val();
        $("#" + idBang + " tbody tr.child[data-idparent='" + iIDParentID + "']").each(function (index, tr) {
            $(tr).find('.r_STT').text(sttParent + "-" + (index + 1));
        });
    });
}

function TinhLaiDongTong(idBang) {
    var fTong = 0;
    if (idBang == TBL_NGUON_VON_DAU_TU) {
        $("#" + idBang + " .fGiaTriPheDuyet").each(function () {
            fTong += parseInt(UnFormatNumber($(this).html()));
        });
        $("#fHanMucDauTu").val(FormatNumber(fTong));
    } else if (idBang == TBL_HANG_MUC_CHINH) {
        $("#" + idBang + " .fHanMucDauTuCha").each(function () {
            fTong += parseInt(UnFormatNumber($(this).html()));
        });
    }
    $("#" + idBang + " .cpdt_tong_giatripheduyet").html(FormatNumber(fTong));
}

function XoaDong(nutXoa, idBang) {
    var dongXoa = nutXoa.parentElement.parentElement;
    if (idBang == TBL_NGUON_VON_DAU_TU) {
        var iIDDuAnNguonVonID = $(dongXoa).find(".r_iID_DuAn_NguonVonID").val();
        arrNguonVon = arrNguonVon.filter(function (x) { return x.iID_DuAn_NguonVonID != iIDDuAnNguonVonID });
    } else if (idBang == TBL_HANG_MUC_CHINH) {
        var iiDDuAnHangMucID = $(dongXoa).find('.r_iID_DuAn_HangMucID').val();
        if ($("#" + idBang + " tbody tr.child[data-idparent='" + iiDDuAnHangMucID + "']").length > 0) {
            PopupModal("Lỗi", "Tồn tại hạng mục con, không thể xóa.", ERROR);
            return;
        }
        arrHangMuc = arrHangMuc.filter(function (x) { return x.iID_DuAn_HangMucID != iiDDuAnHangMucID });
    }
    dongXoa.parentNode.removeChild(dongXoa);
    CapNhatCotStt(idBang);
    TinhLaiDongTong(idBang);
}

function PopupModal(title, message, category) {
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: title, Messages: message, Category: category },
        success: function (data) {
            $("#divModalConfirm").html(data);
        }
    });
}

function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}