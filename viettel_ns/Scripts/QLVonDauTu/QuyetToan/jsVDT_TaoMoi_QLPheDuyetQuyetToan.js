var TBL_CHI_PHI_DAU_TU = "tblChiPhiDauTu";
var TBL_NGUON_VON_DAU_TU = "tblNguonVonDauTu";
var TBL_HANG_MUC_CHINH = "tblHangMucChinh";
var TBL_TAI_LIEU_DINH_KEM = "tblThongTinTaiLieuDinhKem";
var arrChiPhi = [];
var arrHangMuc = [];
var arrGoiThau = [];
var arrNguonVon = [];
var arrChenhLech = [];
var trLoaiCongTrinh;
var typeSearch = 1;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;
var isDetail = $("#isDetail").val();


$(document).ready(function ($) {


    $("#dNgayQuyetDinh").change(function () {
        var iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();

        if (this.value != "" && iID_DonViQuanLyID != "" && iID_DonViQuanLyID != null && iID_DonViQuanLyID != GUID_EMPTY) {
            LayDanhSachDuAnTheoDonViQuanLyVaNgayQuyetDinh(iID_DonViQuanLyID, this.value)
            ClearThongTinDuAn();
        } else {
            $("#iID_DuAnID option:not(:first)").remove();
            $("#iID_DuAnID").trigger("change");
        }
    });

    $("#iID_DonViQuanLyID").change(function () {
        var dNgayQuyetDinh = $("#dNgayQuyetDinh").val();

        if (this.value != "" && this.value != GUID_EMPTY && dNgayQuyetDinh != "") {
            LayDanhSachDuAnTheoDonViQuanLyVaNgayQuyetDinh(this.value, dNgayQuyetDinh)
            ClearThongTinDuAn();
        } else {
            $("#iID_DuAnID option:not(:first)").remove();
            $("#iID_DuAnID").trigger("change");
        }
    });

    $("#iID_DuAnID").change(function (e) {
        var data = $("#iID_DuAnID").val();
        if (data != null) {
            LoadDataComboBoxDeNghiQT(data, $("#iID_QuyetToanID").val());           
        }
    });

    $("#txt_DeNghiQuyetToan").change(function (e) {
        var data = $("#iID_DuAnID").val();
        if (data != "" && data != GUID_EMPTY)
        { 
            LayThongTinDuAn(data);
        } else {
            ClearThongTinDuAn();
        }
        XoaThemMoiChiPhi();
        XoaThemMoiNguonVon();
    });

    $("#iID_ChiPhiID").change(function (e) {
        var dNgayQuyetDinh = $("#dNgayQuyetDinh").val();
        var iID_DuAnID = $("#iID_DuAnID").val();

        if (this.value != "" && this.value != GUID_EMPTY && iID_DuAnID != null && iID_DuAnID != GUID_EMPTY && dNgayQuyetDinh != "") {
            GetGiaTriDuToan(iID_DuAnID, this.value, dNgayQuyetDinh);
        } else {
            $("#fGiaTriDuToan").text('---');
        }
    });

    $("#iID_MaNguonNganSach").change(function (e) {
        var dNgayQuyetDinh = $("#dNgayQuyetDinh").val();
        var iID_DuAnID = $("#iID_DuAnID").val();

        if (this.value != "" && this.value != GUID_EMPTY && iID_DuAnID != null && iID_DuAnID != GUID_EMPTY && dNgayQuyetDinh != "") {
            GetGiaTriDuToanNguonVon(iID_DuAnID, this.value, dNgayQuyetDinh);
        } else {
            $("#fGiaTriDuToanNguonVon").text('---');
        }
    });

    var fHanMucDauTu = FormatNumber($("#fHanMucDauTu").val());
    $("#fHanMucDauTu").val(fHanMucDauTu);
});

function ThemMoiChiPhiDauTu() {
    var tChiPhi = $("#iID_ChiPhiID :selected").html();
    var iIdChiPhi = $("#iID_ChiPhiID").val();
    var tGiaTriQuyetToan = $("#txtAddCpdtGiaTriQuyetToan").val();

    var messErr = [];
    if (iIdChiPhi == GUID_EMPTY) {
        messErr.push("Thông tin chi phí chưa có hoặc chưa chính xác.");
    }
    if (tGiaTriQuyetToan == "" || parseInt(UnFormatNumber(tGiaTriQuyetToan)) <= 0) {
        messErr.push("Giá trị quyết toán của chi phí phải lớn hơn 0.");
    }

    if (messErr.length > 0) {
        var Title = 'Lỗi thêm mới chi phí đầu tư';
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: messErr, Category: ERROR },
            success: function (data) {
                $("#divModalConfirm").html(data);
            }
        });
    } else {
        if (arrChiPhi.filter(function (x) { return x.iID_ChiPhiID == iIdChiPhi }).length > 0) {
            $("#" + TBL_CHI_PHI_DAU_TU + " tbody tr").each(function (index, row) {
                if ($(row).find(".r_iID_ChiPhi").val() == iIdChiPhi) {
                    $(row).remove();
                }
            })
            arrChiPhi = arrChiPhi.filter(function (x) { return x.iID_ChiPhiID != iIdChiPhi });
        }

        var dongMoi = "";
        dongMoi += "<tr style='cursor: pointer;'>";
        dongMoi += "<td align='center' class='r_STT'></td>";
        dongMoi += "<td><input type='hidden' class='r_iID_ChiPhi' value='" + iIdChiPhi + "'/>" + tChiPhi + "</td>";
        dongMoi += "<td class='r_GiaTriPheDuyet' align='right'>" + (tGiaTriQuyetToan == "" ? 0 : tGiaTriQuyetToan) + "</td>";
        dongMoi += "<td align='center'><button class='btn-edit btn-icon' type='button' onclick='SuaDong(this, \"" + TBL_CHI_PHI_DAU_TU + "\")'>" +
            "<i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i>" +
            "</button> ";
        dongMoi += "<button class='btn-delete btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_CHI_PHI_DAU_TU + "\")'>" +
            "<i class='fa fa-trash-o fa-lg' aria-hidden='true'></i>" +
            "</button> </td>";
        dongMoi += "</tr>";

        $("#" + TBL_CHI_PHI_DAU_TU + " tbody").append(dongMoi);
        CapNhatCotStt(TBL_CHI_PHI_DAU_TU);
        TinhLaiDongTong(TBL_CHI_PHI_DAU_TU);

        arrChiPhi.push({
            iID_ChiPhiID: iIdChiPhi,
            fTienPheDuyet: tGiaTriQuyetToan.replaceAll('.', '')
        })

        // xoa text data them moi
        XoaThemMoiChiPhi();
    }
}

function LoadDataComboBoxDeNghiQT(idDuAn, iIdQuyetToanId) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLPheDuyetQuyetToan/GetDeNghiQtTheoDuAn",
        data: { idDuAn: idDuAn, iIdQuyetToanId: iIdQuyetToanId },
        success: function (resp) {
            if (resp.status == true) {
                $("#txt_DeNghiQuyetToan").select2({
                    data: resp.data
                });                         
                $("#txt_DeNghiQuyetToan").trigger("change");
                if (!resp.isAdd) { $("#txt_DeNghiQuyetToan").attr('disabled', true); }      
            }


        }
    });
}

function ThemMoiNguonVonDauTu() {
    var tNguonVon = $("#iID_MaNguonNganSach :selected").html();
    var iIdNguonVon = $("#iID_MaNguonNganSach").val();
    var tGiaTriPheDuyet = $("#txtAddNvdtGiaTriQuyetToan").val();

    var messErr = [];
    if (iIdNguonVon == 0) {
        messErr.push("Thông tin nguồn vốn chưa có hoặc chưa chính xác.");
    }
    if (tGiaTriPheDuyet == "" || parseInt(UnFormatNumber(tGiaTriPheDuyet)) <= 0) {
        messErr.push("Giá trị phê duyệt của nguồn vốn phải lớn hơn 0.");
    }

    if (messErr.length > 0) {
        var Title = 'Lỗi thêm mới nguồn vốn đầu tư';
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: messErr, Category: ERROR },
            success: function (data) {
                $("#divModalConfirm").html(data);
            }
        });
        //alert(messErr.join("\n"));
    } else {
        if (arrNguonVon.filter(function (x) { return x.iID_NguonVonID == iIdNguonVon }).length > 0) {
            $("#" + TBL_NGUON_VON_DAU_TU + " tbody tr").each(function (index, row) {
                if ($(row).find(".r_iID_NguonVon").val() == iIdNguonVon) {
                    $(row).remove();
                }
            })
            arrNguonVon = arrNguonVon.filter(function (x) { return x.iID_NguonVonID != iIdNguonVon });
        }

        var dongMoi = "";
        dongMoi += "<tr style='cursor: pointer;'>";
        dongMoi += "<td align='center' class='r_STT'></td>";
        dongMoi += "<td><input type='hidden' class='r_iID_NguonVon' value='" + iIdNguonVon + "'/>" + tNguonVon + "</td>";
        dongMoi += "<td class='r_GiaTriPheDuyet' align='right'>" + (tGiaTriPheDuyet == "" ? 0 : tGiaTriPheDuyet) + "</td>";
        dongMoi += "<td align='center'><button class='btn-edit btn-icon' type='button' onclick='SuaDong(this, \"" + TBL_NGUON_VON_DAU_TU + "\")'>" +
            "<i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i>" +
            "</button> ";
        dongMoi += "<button class='btn-delete btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_NGUON_VON_DAU_TU + "\")'>" +
            "<span class='fa fa-trash-o fa-lg' aria-hidden='true'></span>" +
            "</button></td>";
        dongMoi += "</tr>";

        $("#" + TBL_NGUON_VON_DAU_TU + " tbody").append(dongMoi);
        CapNhatCotStt(TBL_NGUON_VON_DAU_TU);
        TinhLaiDongTong(TBL_NGUON_VON_DAU_TU);

        arrNguonVon.push({
            iID_NguonVonID: iIdNguonVon,
            fTienPheDuyet: tGiaTriPheDuyet.replaceAll('.', '')
        })

        // xoa text data them moi
        XoaThemMoiNguonVon();
        GetNoiDungQuyetToan(arrNguonVon);
    }
}

function GetNoiDungQuyetToan(arrNguonVon) {
    var dNgayQuyetDinh = $("#dNgayQuyetDinh").val();
    var iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
    var iID_DuAnID = $("#iID_DuAnID").val();
    var tongGiaTriPheDuyet = $("#" + TBL_NGUON_VON_DAU_TU + " .cpdt_tong_giatripheduyet").html().replaceAll('.', '');
    var data = {
        arrNguonVon: arrNguonVon,
        iID_DonViQuanLyID: iID_DonViQuanLyID,
        iID_DuAnID: iID_DuAnID,
        dNgayQuyetDinh: dNgayQuyetDinh,
        tongGiaTriPheDuyet: tongGiaTriPheDuyet
    };
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetQuyetToan/GetNoiDungQuyetToan",
        type: "POST",
        dataType: "json",
        data: { data: data },
        success: function (r) {
            if (r.bIsComplete) {
                $("#fTongGiaTriPhanBo").text(r.data.fTongGiaTriPhanBo != 0 ? FormatNumber(r.data.fTongGiaTriPhanBo) : 0);
                $("#tongGiaTriPheDuyet").text(r.data.tongGiaTriPheDuyet != 0 ? FormatNumber(r.data.tongGiaTriPheDuyet) : 0);
                $("#fTongGiaTriChenhLech").text(r.data.fTongGiaTriChenhLech != 0 ? FormatNumber(r.data.fTongGiaTriChenhLech) : 0);
                arrChenhLech = r.data.lstNoiDungQuyetToan;
                GetListDataNoiDungQuyetToan(r.data.lstNoiDungQuyetToan);
            }
        }
    })
}

function GetListDataNoiDungQuyetToan(lstNoiDungQuyetToan) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLPheDuyetQuyetToan/GetListDataNoiDungQuyetToan",
        data: { lstNoiDungQuyetToan: lstNoiDungQuyetToan },
        success: function (data) {
            $("#lstDataView").html(data);
        }
    });
}

function CapNhatCotStt(idBang) {
    $("#" + idBang + " tbody tr").each(function (index, tr) {
        $(tr).find('.r_STT').text(index + 1);
    });
}

function TinhLaiDongTong(idBang) {
    var tongGiaTriPheDuyet = 0;

    $("#" + idBang + " .r_GiaTriPheDuyet").each(function () {
        tongGiaTriPheDuyet += parseInt(UnFormatNumber($(this).html()));
    });

    $("#" + idBang + " .cpdt_tong_giatripheduyet").html(FormatNumber(tongGiaTriPheDuyet));
}

function XoaDong(nutXoa, idBang) {
    var dongXoa = nutXoa.parentElement.parentElement;
    if (idBang == TBL_CHI_PHI_DAU_TU) {
        var iID_ChiPhiID = $(dongXoa).find(".r_iID_ChiPhi").val();
        arrChiPhi = arrChiPhi.filter(function (x) { return x.iID_ChiPhiID != iID_ChiPhiID });
    } else if (idBang == TBL_NGUON_VON_DAU_TU) {
        var iID_NguonVonID = $(dongXoa).find(".r_iID_NguonVon").val();
        arrNguonVon = arrNguonVon.filter(function (x) { return x.iID_NguonVonID != iID_NguonVonID });
    } else if (idBang == TBL_HANG_MUC_CHINH) {
        var iId_PTHMCHangMuc = $(dongXoa).find(".r_iID_HangMucChinh").val();
        arrChenhLech = arrChenhLech.filter(function (x) { return x.iId_PTHMCHangMuc != iId_PTHMCHangMuc });
    }
    dongXoa.parentNode.removeChild(dongXoa);
    CapNhatCotStt(idBang);
    TinhLaiDongTong(idBang);
}
/*NinhNV start*/

function Luu() {
    if (CheckLoi()) {
        var quyetToan = {};
        var data = {};

        quyetToan.iID_QuyetToanID = $("#iID_QuyetToanID").val();
        quyetToan.sSoQuyetDinh = $("#sSoQuyetDinh").val();
        quyetToan.dNgayQuyetDinh = $("#dNgayQuyetDinh").val();
        quyetToan.sCoQuanPheDuyet = $("#sCoQuanPheDuyet").val();
        quyetToan.sNguoiKy = $("#sNguoiKy").val();
        quyetToan.fChiPhiThietHai = $("#fChiPhiThietHai").val();
        quyetToan.fChiPhiKhongTaoNenTaiSan = $("#fChiPhiKhongTaoNenTaiSan").val();
        quyetToan.fTaiSanDaiHanThuocCDTQuanLy = $("#fTaiSanDaiHanThuocCDTQuanLy").val();
        quyetToan.fTaiSanDaiHanDonViKhacQuanLy = $("#fTaiSanDaiHanDonViKhacQuanLy").val();
        quyetToan.fTaiSanNganHanThuocCDTQuanLy = $("#fTaiSanNganHanThuocCDTQuanLy").val();
        quyetToan.fTaiSanNganHanDonViKhacQuanLy = $("#fTaiSanNganHanDonViKhacQuanLy").val();
        quyetToan.iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
        quyetToan.iID_DuAnID = $("#iID_DuAnID").val();
        quyetToan.sNoiDung = $("#sNoiDung").val();
        quyetToan.iID_DeNghiQuyetToanID = $("#txt_DeNghiQuyetToan").val();
        //quyetToan.fTienQuyetToanPheDuyet = UnFormatNumber($("#id_tong_giatripheduyet_nguonvon").html());

        if (quyetToan.iID_QuyetToanID === GUID_EMPTY) {
        }

        data = {
            quyetToan: quyetToan,
            listChiPhi: arrChiPhi,
            listHangMuc: arrHangMuc,
            listGoiThau: arrGoiThau
        };

        $.ajax({
            type: "POST",
            url: "/QLPheDuyetQuyetToan/QLPheDuyetQuyetToanSave",
            data: { data: data },
            success: function (r) {
                if (r == "True") {
                    if (quyetToan.iID_QuyetToanID != null && quyetToan.iID_QuyetToanID != GUID_EMPTY) {
                        alert("Cập nhật thành công.");
                        
                    } else {
                        alert("Tạo mới thành công.");
                    }
                    window.location.href = "/QLVonDauTu/QLPheDuyetQuyetToan/Index";
                } else {
                    alert("Lỗi khi lưu.");
                }
            }
        });
    }
}

function CheckLoi() {
    var sSoQuyetDinh = $("#sSoQuyetDinh").val();
    var dNgayQuyetDinh = $("#dNgayQuyetDinh").val();
    var iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
    var iID_DuAnID = $("#iID_DuAnID").val();

    var messErr = [];
    if (sSoQuyetDinh == '') {
        messErr.push("Thông tin số quyết định phê duyệt chưa có hoặc chưa chính xác!");
    }
    if (dNgayQuyetDinh == '') {
        messErr.push("Thông tin ngày quyết định phê duyệt chưa có hoặc chưa chính xác!");
    }
    if (iID_DonViQuanLyID == GUID_EMPTY) {
        messErr.push("Thông tin đơn vị quản lý chưa có hoặc chưa chính xác!");
    }
    if (iID_DuAnID == GUID_EMPTY || iID_DuAnID == null) {
        messErr.push("Thông tin nội dung chưa có hoặc chưa chính xác!");
    }
    if (arrGoiThau.length == 0 && arrChiPhi.length == 0) {
        messErr.push("Thông tin giá trị chi tiết phê duyệt chưa có hoặc chưa chính xác!");
    }
    //if (arrNguonVon.length == 0) {
    //    messErr.push("Thông tin giá trị nguồn vốn phê duyệt chưa có hoặc chưa chính xác!");
    //}
    
    var tongChiPHiPheDuyet = $("#id_tong_giatripheduyet_chiphi").html();
    var tongNguonVonPheDuyet = $("#id_tong_giatripheduyet_nguonvon").html();
    //if (tongChiPHiPheDuyet != undefined && tongNguonVonPheDuyet != undefined && tongChiPHiPheDuyet != '' && tongNguonVonPheDuyet != '') {
    //    if (parseInt(UnFormatNumber(tongChiPHiPheDuyet)) != parseInt(UnFormatNumber(tongNguonVonPheDuyet))) {
    //        messErr.push("Giá trị phê duyệt của danh sách chi phí và nguồn vốn không bằng nhau!");
    //    }
    //}
    if (messErr.length > 0) {
        var Title = 'Lỗi lưu thông tin phê duyệt quyết toán';
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

function checkExistMaDuAn(sMaDuAn) {
    $.ajax({
        type: "POST",
        url: "/QLDuAn/CheckExistMaDuAn",
        data: { sMaDuAn: sMaDuAn },
        success: function (r) {
            if (r == "True") {
                alert("Mã dự án đã tồn tại!");
            }
        }
    });
}

function XoaThemMoiChiPhi() {
    // xoa text data them moi
    $("#iID_ChiPhiID").prop("selectedIndex", 0);
    $("#fGiaTriDuToan").text("---");
    $("#txtAddCpdtGiaTriQuyetToan").val("");
}

function XoaThemMoiNguonVon() {
    // xoa text data them moi
    $("#iID_MaNguonNganSach").prop("selectedIndex", 0);
    $("#fGiaTriDuToanNguonVon").text('---');
    $("#txtAddNvdtGiaTriQuyetToan").val("");
}


function CancelSaveData() {
    window.location.href = "/QLVonDauTu/QLPheDuyetQuyetToan/Index";
}

function SuaDong(nutSua, idBang) {
    var dongSua = nutSua.parentElement.parentElement;
    if (idBang == TBL_CHI_PHI_DAU_TU) {
        var iID_ChiPhiID = $(dongSua).find(".r_iID_ChiPhi").val();
        var fGiaTriPheDuyet = $(dongSua).find(".r_GiaTriPheDuyet").text();
        $("#iID_ChiPhiID").val(iID_ChiPhiID);
        $("#txtAddCpdtGiaTriQuyetToan").val(fGiaTriPheDuyet);
        $("#iID_ChiPhiID").trigger("change");
    } else if (idBang == TBL_NGUON_VON_DAU_TU) {
        var r_iID_NguonVon = $(dongSua).find(".r_iID_NguonVon").val();
        var fGiaTriPheDuyet = $(dongSua).find(".r_GiaTriPheDuyet").text();
        $("#iID_MaNguonNganSach").val(r_iID_NguonVon);
        $("#txtAddNvdtGiaTriQuyetToan").val(fGiaTriPheDuyet);
        $("#iID_MaNguonNganSach").trigger("change");
    } else if (idBang == TBL_HANG_MUC_CHINH) {
        var r_iID_HangMucChinh = $(dongSua).find(".r_iID_HangMucChinh").val();
        var r_txt_HangMucChinh = $(dongSua).find(".r_txt_HangMucChinh").text();
        var fGiaTriPheDuyet = $(dongSua).find(".r_GiaTriPheDuyet").text();

        $("#iID_HangMucChinh").val(r_iID_HangMucChinh);
        $("#txtAddPTHMCHangMuc").val(r_txt_HangMucChinh);
        $("#txtAddPTHMCGiaTriPheDuyet").val(fGiaTriPheDuyet);
    }
}

function LayDanhSachDuAnTheoDonViQuanLyVaNgayQuyetDinh(iID_DonViQuanLyID, dNgayQuyetDinh) {
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetQuyetToan/LayDanhSachDuAnTheoDonViQuanLyVaNgayQuyetDinh",
        type: "POST",
        data: { iID_DonViQuanLyID: iID_DonViQuanLyID, dNgayQuyetDinh: dNgayQuyetDinh },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null && data != "") {
                $("#iID_DuAnID").html(data);
                $("#iID_DuAnID").prop("selectedIndex", 0);
            }
        },
        error: function (data) {

        }
    })
}

function LayThongTinDuAn(iID_DuAnID) {
    var dNgayQuyetDinh = $("#dNgayQuyetDinh").val();
    var iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
    var idDeNghiQuyetToan = $("#txt_DeNghiQuyetToan").val();
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetQuyetToan/LayThongTinDuAn",
        type: "POST",
        dataType: "json",
        data: { iID_DonViQuanLyID: iID_DonViQuanLyID, iID_DuAnID: iID_DuAnID, dNgayQuyetDinh: dNgayQuyetDinh, idDeNghiQuyetToan: idDeNghiQuyetToan },
        success: function (r) {
            if (r.bIsComplete) {
                $("#sDiaDiem").text(r.data.sDiaDiem);
                var sKhoiCong = r.data.sKhoiCong != null ? r.data.sKhoiCong : '';
                var sKetThuc = r.data.sKetThuc != null ? r.data.sKetThuc : '';
                var sThoiGianTH = sKhoiCong + ' - ' + sKetThuc ;
                $("#sThoiGianTH").text(sThoiGianTH);
                $("#fTongMucDauTuPheDuyet").text(r.data.fTongMucDauTuPheDuyet != 0 ? FormatNumber(r.data.fTongMucDauTuPheDuyet) : 0);
                $("#fGiaTriUng").text(r.data.fGiaTriUng != 0 ? FormatNumber(r.data.fGiaTriUng) : 0);
                $("#fLKSoVonDaTamUng").text(r.data.fLKSoVonDaTamUng != 0 ? FormatNumber(r.data.fLKSoVonDaTamUng) : 0);
                $("#fLKThuHoiUng").text(r.data.fLKThuHoiUng != 0 ? FormatNumber(r.data.fLKThuHoiUng) : 0);
                $("#fConPhaiThuHoi").text(r.data.fConPhaiThuHoi != 0 ? FormatNumber(r.data.fConPhaiThuHoi) : 0);
                $("#sDuToan").text(r.data.sDuToan);
                //$("#fLKKHVUDuocDuyet").val(r.data.fLKKHVUDuocDuyet > 0 ? FormatNumber(r.data.fLKKHVUDuocDuyet) : 0);
                //$("#fLKSoVonDaTamUng").val(r.data.fLKSoVonDaTamUng > 0 ? FormatNumber(r.data.fLKSoVonDaTamUng) : 0);
                //$("#fLKThuHoiUng").val(r.data.fLKThuHoiUng > 0 ? FormatNumber(r.data.fLKThuHoiUng) : 0);
                
                if (r.loaidengi == 1) {
                    GetListChiPhiHangMuc(r.data.iID_DuToanID, $("#txt_DeNghiQuyetToan").val());
                }
                else
                {
                    GetListGoiThau($("#txt_DeNghiQuyetToan").val());
                }   
            }
        }
    })
}

function GetListChiPhiHangMuc(iID_DuToanID, iID_DeNghiQuyetToanID) {
    arrChiPhi = [];
    arrHangMuc = [];
    var iID_QuyetToanID = $("#iID_QuyetToanID").val();
    if (iID_DuToanID != null && iID_DuToanID != "" && iID_DuToanID != GUID_EMPTY) {
        $.ajax({
            type: "POST",
            url: "/QLVonDauTu/QLPheDuyetQuyetToan/GetListChiPhiHangMucTheoDuAn",
            data: { iID_DuToanID: iID_DuToanID, iID_DeNghiQuyetToanID: iID_DeNghiQuyetToanID, iID_QuyetToanID: iID_QuyetToanID },
            success: function (resp) {
                if (resp.lstChiPhi != null && resp.lstChiPhi.length > 0) {
                    arrChiPhi = resp.lstChiPhi;
                }
                if (resp.lstHangMuc != null && resp.lstHangMuc.length > 0) {
                    arrHangMuc = resp.lstHangMuc;
                }
                DrawTableChiPhiHangMuc(resp.sumGiaTriThamTra, resp.sumGiaTriQuyetToan);
            }
        });
    }
}

function GetListGoiThau(iID_DeNghiQuyetToanID) {
    arrGoiThau = [];
    var iID_QuyetToanID = $("#iID_QuyetToanID").val();
    if (iID_DeNghiQuyetToanID != null && iID_DeNghiQuyetToanID != "" && iID_DeNghiQuyetToanID != GUID_EMPTY) {
        $.ajax({
            type: "POST",
            url: "/QLVonDauTu/QLPheDuyetQuyetToan/GetListGoiThau",
            data: { iID_DeNghiQuyetToanID: iID_DeNghiQuyetToanID, iID_QuyetToanID: iID_QuyetToanID },
            success: function (resp) {
                if (resp.lstGoiThau != null && resp.lstGoiThau.length > 0) {
                    arrGoiThau = resp.lstGoiThau;
                }
                DrawTableGoiThau(resp.sumGiaTriThamTra, resp.sumGiaTriQuyetToan);
            }
        });
    }
}

function DrawTableGoiThau(sumGiaTriThamTra, sumGiaTriQuyetToan) {
    var html = "";
    var index = 1;

    //them dong tong so
    html += "<tr style='font-weight:bold' data-id='-1'>";
    html += "<td></td>";
    html += "<td>Tổng số</td>";
    html += "<td></td>";
    html += "<td></td>";
    html += "<td></td>";
    html += "<td class='text-right txtTongSoThamTra'>" + sumGiaTriThamTra + "</td>";
    html += "<td class='text-right txtTongSoQuyetToan'>" + sumGiaTriQuyetToan + "</td>";
    html += "<td></td>";
    html += "</tr>";

    arrGoiThau.forEach(function (itemCp) {
        var disabled = "", isBold = "";
        html += "<tr data-loai='1' style='" + isBold + "' data-id='" + itemCp.iID_DuAn_GoiThau + "' data-parentid=''>";
        html += "<td class='stt text-center'>"+ index++ +"</td>";
        html += "<td>Gói thầu</td>";
        html += "<td>" + itemCp.sTenGoiThau + "</td>";
        html += "<td class='text-right'>" + itemCp.sTienPheDuyet + "</td>";
        html += "<td class='text-right'>" + itemCp.sGiaTriDeNghiQuyetToan + "</td>";
        html += "<td><input type='text' class='form-control clearable text-right txtGiaTriThamTra' " + disabled + " value='" + itemCp.sGiaTriThamTra + "' onchange='changeGiaTriGoiThau(this)' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
        html += "<td><input type='text' class='form-control clearable text-right txtGiaTriQuyetToan' " + disabled + " value='" + itemCp.sGiaTriQuyetToan + "' onchange='changeGiaTriGoiThau(this)' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
        html += "<td class='text-right txtChenhLechSoVoiDuToan'>" + itemCp.sChenhLenhSoVoiDuToanNhap + "</td>";
        html += "<td class='text-right txtChenhLechSoVoiDeNghi'>" + itemCp.sChenhLechSoVoiDeNghi + "</td>";
        html += "</tr>";
    })
    $("#tblDanhSachChiTiet tbody").html(html);
    $(".div_ChiTiet").show();
    if (isDetail == 1) {
        $(".detail-panel").find("input").attr("disabled", "disabled");
    }
}

function DrawTableChiPhiHangMuc(sumGiaTriThamTra, sumGiaTriQuyetToan) {
    var html = "";
    //them dong tong so
    html += "<tr style='font-weight:bold' data-id='-1'>";
    html += "<td></td>";
    html += "<td>Tổng số</td>";
    html += "<td></td>";
    html += "<td></td>";
    html += "<td></td>";
    html += "<td class='text-right txtTongSoThamTra'>" + sumGiaTriThamTra + "</td>";
    html += "<td class='text-right txtTongSoQuyetToan'>" + sumGiaTriQuyetToan + "</td>";
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
        html += "<td class='text-right'>" + itemCp.sGiaTriDeNghiQuyetToan + "</td>";
        html += "<td><input type='text' class='form-control clearable text-right txtGiaTriThamTra' " + disabled + " value='" + itemCp.sGiaTriThamTra + "' onchange='changeGiaTri(this)' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
        html += "<td><input type='text' class='form-control clearable text-right txtGiaTriQuyetToan' " + disabled + " value='" + itemCp.sGiaTriQuyetToan + "' onchange='changeGiaTri(this)' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";        
        html += "<td class='text-right txtChenhLechSoVoiDuToan'>" + itemCp.sChenhLenhSoVoiDuToanNhap + "</td>";
        html += "<td class='text-right txtChenhLechSoVoiDeNghi'>" + itemCp.sChenhLechSoVoiDeNghi + "</td>";
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
                html += "<td class='text-right'>" + itemHm.sGiaTriDeNghiQuyetToan + "</td>";
                html += "<td><input type='text' class='form-control clearable text-right txtGiaTriThamTra' value='" + itemHm.sGiaTriThamTra + "' " + disabled + " onchange='changeGiaTri(this)' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
                html += "<td><input type='text' class='form-control clearable text-right txtGiaTriQuyetToan' value='" + itemHm.sGiaTriQuyetToan + "' " + disabled + " onchange='changeGiaTri(this)' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";                
                html += "<td class='text-right txtChenhLechSoVoiDuToan'>" + itemHm.sChenhLenhSoVoiDuToanNhap + "</td>";
                html += "<td class='text-right txtChenhLechSoVoiDeNghi'>" + itemHm.sChenhLechSoVoiDeNghi + "</td>";                                
                html += "</tr>";
            })
        }
    })
    $("#tblDanhSachChiTiet tbody").html(html);
    $(".div_ChiTiet").show();
    if (isDetail == 1) {
        $(".detail-panel").find("input").attr("disabled", "disabled");
    }
}

function ClearThongTinDuAn() {
    $("#sDuToan").text('---');
    $("#sDiaDiem").text('---');
    $("#sThoiGianTH").text('---' + ' - ' + '---');
    $("#fTongMucDauTuPheDuyet").text('---');
    $("#fGiaTriUng").text('---');
    $("#fLKSoVonDaTamUng").text('---');
    $("#fLKThuHoiUng").text('---');
    $("#fConPhaiThuHoi").text('---');
}

function GetGiaTriDuToan(iID_DuAnID, iID_ChiPhiID, dNgayQuyetDinh) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLPheDuyetQuyetToan/GetGiaTriDuToan",
        data: { iID_DuAnID: iID_DuAnID, iID_ChiPhiID: iID_ChiPhiID, dNgayQuyetDinh: dNgayQuyetDinh },
        success: function (r) {
            if (r > 0) {
                $("#fGiaTriDuToan").text(FormatNumber(r));
            } else {
                $("#fGiaTriDuToan").text(r);
            }
        }
    });
}

function GetGiaTriDuToanNguonVon(iID_DuAnID, iID_MaNguonNganSach, dNgayQuyetDinh) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLPheDuyetQuyetToan/GetGiaTriDuToanNguonVon",
        data: { iID_DuAnID: iID_DuAnID, iID_MaNguonNganSach: iID_MaNguonNganSach, dNgayQuyetDinh: dNgayQuyetDinh },
        success: function (r) {
            if (r > 0) {
                $("#fGiaTriDuToanNguonVon").text(FormatNumber(r));
            } else {
                $("#fGiaTriDuToanNguonVon").text(r);
            }
        }
    });
}

function SetArrsData() {
    arrChiPhi = arrChiPhiTemp;
    CapNhatCotStt(TBL_CHI_PHI_DAU_TU);
    TinhLaiDongTong(TBL_CHI_PHI_DAU_TU);

    arrNguonVon = arrNguonVonTemp;
    CapNhatCotStt(TBL_NGUON_VON_DAU_TU);
    TinhLaiDongTong(TBL_NGUON_VON_DAU_TU);

    GetNoiDungQuyetToan(arrNguonVon);
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


    var sGiaTriThamTra = $(dongHienTai).find(".txtGiaTriThamTra").val();
    var fGiaTriThamTra = sGiaTriThamTra == "" ? 0 : parseInt(UnFormatNumber(sGiaTriThamTra));

    var sGiaTriQuyetToan = $(dongHienTai).find(".txtGiaTriQuyetToan").val();
    var fGiaTriQuyetToan = sGiaTriQuyetToan == "" ? 0 : parseInt(UnFormatNumber(sGiaTriQuyetToan));

    var fChenhLechSoVoiDuToanNhap = fGiaTriQuyetToan - objThis.fTienPheDuyet;
    var fChenhLechSoVoiDeNghi = fGiaTriQuyetToan - objThis.fGiaTriDeNghiQuyetToan;

    objThis.fGiaTriThamTra = fGiaTriThamTra;
    objThis.fGiaTriQuyetToan = fGiaTriQuyetToan;

    objThis.fChenhLechSoVoiDuToanNhap = fChenhLechSoVoiDuToanNhap;
    objThis.fChenhLechSoVoiDeNghi = fChenhLechSoVoiDeNghi;

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
    var sumGiaTriThamTra = 0, sumGiaTriQuyetToan = 0;
    arrChiPhi.forEach(x => {
        sumGiaTriThamTra += x.fGiaTriThamTra;
        sumGiaTriQuyetToan += x.fGiaTriQuyetToan;
    });
    $('*[data-id="' + "-1" + '"]').find('.txtTongSoThamTra').html(FormatNumber(sumGiaTriThamTra));
    $('*[data-id="' + "-1" + '"]').find('.txtTongSoQuyetToan').html(FormatNumber(sumGiaTriQuyetToan));

    $(dongHienTai).find(".txtChenhLechSoVoiDuToan").html(FormatNumber(fChenhLechSoVoiDuToanNhap));
    $(dongHienTai).find(".txtChenhLechSoVoiDeNghi").html(FormatNumber(fChenhLechSoVoiDeNghi));
}

function changeGiaTriGoiThau(input) {
    var dongHienTai = $(input).closest("tr");
    var id = $(dongHienTai).attr("data-id");
    var iIdDuAnGoiThau = "";

    var objThis = arrGoiThau.filter(x => x.iID_DuAn_GoiThau == id)[0];

    var sGiaTriThamTra = $(dongHienTai).find(".txtGiaTriThamTra").val();
    var fGiaTriThamTra = sGiaTriThamTra == "" ? 0 : parseInt(UnFormatNumber(sGiaTriThamTra));

    var sGiaTriQuyetToan = $(dongHienTai).find(".txtGiaTriQuyetToan").val();
    var fGiaTriQuyetToan = sGiaTriQuyetToan == "" ? 0 : parseInt(UnFormatNumber(sGiaTriQuyetToan));

    var fChenhLechSoVoiDuToanNhap = fGiaTriQuyetToan - objThis.fTienPheDuyet;
    var fChenhLechSoVoiDeNghi = fGiaTriQuyetToan - objThis.fGiaTriDeNghiQuyetToan;

    objThis.fGiaTriThamTra = fGiaTriThamTra;
    objThis.fGiaTriQuyetToan = fGiaTriQuyetToan;

    objThis.fChenhLechSoVoiDuToanNhap = fChenhLechSoVoiDuToanNhap;
    objThis.fChenhLechSoVoiDeNghi = fChenhLechSoVoiDeNghi;

    arrGoiThau = arrGoiThau.filter(function (x) { return x.iID_DuAn_GoiThau != objThis.iID_DuAn_GoiThau });
    arrGoiThau.push(objThis);

    var sumGiaTriThamTra = 0, sumGiaTriQuyetToan = 0;
    arrGoiThau.forEach(x => {
        sumGiaTriThamTra += x.fGiaTriThamTra;
        sumGiaTriQuyetToan += x.fGiaTriQuyetToan;
    });
    $('*[data-id="' + "-1" + '"]').find('.txtTongSoThamTra').html(FormatNumber(sumGiaTriThamTra));
    $('*[data-id="' + "-1" + '"]').find('.txtTongSoQuyetToan').html(FormatNumber(sumGiaTriQuyetToan));

    $(dongHienTai).find(".txtChenhLechSoVoiDuToan").html(FormatNumber(fChenhLechSoVoiDuToanNhap));
    $(dongHienTai).find(".txtChenhLechSoVoiDeNghi").html(FormatNumber(fChenhLechSoVoiDeNghi));
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

    var sumGiaTriThamTra = 0, sumGiaTriQuyetToan = 0;
    arrChild.forEach(x => {
        sumGiaTriThamTra += x.fGiaTriThamTra;
        sumGiaTriQuyetToan += x.fGiaTriQuyetToan;     
    });

    var fChenhLechSoVoiDuToanNhap = sumGiaTriQuyetToan - objParentItem.fTienPheDuyet;
    var fChenhLechSoVoiDeNghi = sumGiaTriQuyetToan - objParentItem.fGiaTriDeNghiQuyetToan;

    $('*[data-id="' + parentId + '"]').find('.txtGiaTriThamTra').val(FormatNumber(sumGiaTriThamTra));
    $('*[data-id="' + parentId + '"]').find('.txtGiaTriQuyetToan').val(FormatNumber(sumGiaTriQuyetToan));    

    $('*[data-id="' + parentId + '"]').find(".txtChenhLechSoVoiDuToan").html(FormatNumber(fChenhLechSoVoiDuToanNhap));
    $('*[data-id="' + parentId + '"]').find(".txtChenhLechSoVoiDeNghi").html(FormatNumber(fChenhLechSoVoiDeNghi));    

    var parentItemNew = objParentItem;

    objParentItem.fGiaTriThamTra = sumGiaTriThamTra;
    objParentItem.fGiaTriQuyetToan = sumGiaTriQuyetToan;

    objParentItem.fChenhLechSoVoiDuToanNhap = fChenhLechSoVoiDuToanNhap;
    objParentItem.fChenhLenhSoVoiDeNghi = fChenhLechSoVoiDeNghi;   

    arrHangMuc = arrHangMuc.filter(function (x) { return x.iID_HangMucID != objParentItem.iID_HangMucID });
    arrHangMuc.push(parentItemNew);
}

function CalculateTienConLaiHangMuc(idChiPhi) {
    var sumGiaTriThamTra = 0, sumGiaTriQuyetToan = 0;

    var arrHMParent = arrHangMuc.filter(function (x) { return (x.iID_ParentID == "" || x.iID_ParentID == null) && x.iID_DuAn_ChiPhi == idChiPhi });
    if (arrHMParent != null && arrHMParent.length > 0) {
        arrHMParent.forEach(x => {
            if (x.fTienPheDuyet != null || x.fTienPheDuyet != "") {
                sumGiaTriThamTra += x.fGiaTriThamTra;
                sumGiaTriQuyetToan += x.fGiaTriQuyetToan;
            }
        });
    }

    // ipdate gia tri cho chiphi
    var objChiPhi = arrChiPhi.filter(x => x.iID_DuAn_ChiPhi == idChiPhi)[0];

    var fChenhLechSoVoiDuToanNhap = sumGiaTriQuyetToan - objChiPhi.fTienPheDuyet;
    var fChenhLechSoVoiDeNghi = sumGiaTriQuyetToan - objChiPhi.fGiaTriDeNghiQuyetToan;

    $('*[data-id="' + idChiPhi + '"]').find('.txtGiaTriThamTra').val(FormatNumber(sumGiaTriThamTra));
    $('*[data-id="' + idChiPhi + '"]').find('.txtGiaTriQuyetToan').val(FormatNumber(sumGiaTriQuyetToan));

    $('*[data-id="' + idChiPhi + '"]').find(".txtChenhLechSoVoiDuToan").html(FormatNumber(fChenhLechSoVoiDuToanNhap));
    $('*[data-id="' + idChiPhi + '"]').find(".txtChenhLechSoVoiDeNghi").html(FormatNumber(fChenhLechSoVoiDeNghi));  

    objChiPhi.fGiaTriThamTra = sumGiaTriThamTra;
    objChiPhi.fGiaTriQuyetToan = sumGiaTriQuyetToan;    

    objChiPhi.fChenhLechSoVoiDuToanNhap = fChenhLechSoVoiDuToanNhap;
    objChiPhi.fChenhLechSoVoiDeNghi = fChenhLechSoVoiDeNghi;    

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

function CalculateTotalParent(objParentItem, arrChild) {
    var parentId = objParentItem.iID_DuAn_ChiPhi;

    var sumGiaTriThamTra = 0, sumGiaTriQuyetToan = 0;
    arrChild.forEach(x => {
        sumGiaTriThamTra += x.fGiaTriThamTra;
        sumGiaTriQuyetToan += x.fGiaTriQuyetToan;       
    });

    var fChenhLechSoVoiDuToanNhap = sumGiaTriQuyetToan - objParentItem.fTienPheDuyet;
    var fChenhLechSoVoiDeNghi = sumGiaTriQuyetToan - objParentItem.fGiaTriDeNghiQuyetToan;

    $('*[data-id="' + parentId + '"]').find('.txtGiaTriThamTra').val(FormatNumber(sumGiaTriThamTra));
    $('*[data-id="' + parentId + '"]').find('.txtGiaTriQuyetToan').val(FormatNumber(sumGiaTriQuyetToan));

    $('*[data-id="' + parentId + '"]').find(".txtChenhLechSoVoiDuToan").html(FormatNumber(fChenhLechSoVoiDuToanNhap));
    $('*[data-id="' + parentId + '"]').find(".txtChenhLechSoVoiDeNghi").html(FormatNumber(fChenhLechSoVoiDeNghi));

    var parentItemNew = objParentItem;

    objParentItem.fGiaTriThamTra = sumGiaTriThamTra;
    objParentItem.fGiaTriQuyetToan = sumGiaTriQuyetToan;  

    objParentItem.fChenhLechSoVoiDuToanNhap = fChenhLechSoVoiDuToanNhap;
    objParentItem.fChenhLechSoVoiDeNghi = fChenhLechSoVoiDeNghi;

    arrChiPhi = arrChiPhi.filter(function (x) { return x.iID_DuAn_ChiPhi != objParentItem.iID_DuAn_ChiPhi });
    arrChiPhi.push(parentItemNew);
}
/*NinhNV end*/