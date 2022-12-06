var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var tblDataGrid = [];
var TBL_DANH_SACH_THANH_TOAN_CHITIET = 'tblDanhSachThanhToanChiTiet';

var THANH_TOAN = 1;
var TAM_UNG = 0;
var THU_HOI_UNG = 2

//====================== trigger ===============================//
$("#txtNamKeHoach").trigger("change");
$("#drpLoaiKhoan").trigger("change");
$("#drpNganh").trigger("change");
$("#drpDuAn").trigger("change");
$("#drpHopDong").trigger("change");

$(document).ready(function () {
    $("#iID_ChuDauTuID").change(function () {
        if (this.value != "" && this.value != GUID_EMPTY) {
            $.ajax({
                url: "/GiaiNganThanhToan/LayDanhSachDuAnTheoChuDauTu",
                type: "POST",
                data: { iIDChuDauTuID: this.value },
                dataType: "json",
                cache: false,
                success: function (data) {
                    if (data != null && data != "") {
                        $("#drpDuAn").html(data);
                        $("#drpDuAn").trigger("change");
                    }
                },
                error: function (data) {

                }
            })
        } else {
            $("#drpDuAn option:not(:first)").remove();
            $("#drpDuAn ").trigger("change");
        }
    });

    $("#txtNamKeHoach").change(function (e) {
        $("#drpNganh").empty();
        ResetThanhToanChiTiet();
        GetDataDropdownNganh();
    });

    $("#drpHopDong").change(function (e) {
        GetDetailHopDong();
        LoadLuyKeThanhToan();
    });

    $("#drpNguonNganSach").change(function (e) {
        GetDataDropdownNganh();
    });

    $("#drpDuAn").change(function (e) {
        ResetThanhToanChiTiet();
        $("#drpNganh").empty();
        $("#drpNguonNganSach").empty();
        $("#drpHopDong").empty();
        GetDataDropdownNganh();
        GetDataDropdownNguonVon();
    });

    $("#drpNguonNganSach").change(function (e) {
        ResetThanhToanChiTiet();
    })

    $("#txtNgayDeNghi").change(function (e) {
        LoadLuyKeThanhToan();
    })

    $("#drpLoaiThanhToan").change(function (e) {
        ResetThanhToanChiTiet();
    })
});

function ResetThanhToanChiTiet() {
    $("#divBtnAdd").hide();
    $("#" + TBL_DANH_SACH_THANH_TOAN_CHITIET + " tbody").html("");
}

function GetItemData(id) {
    var dataCheck = $.map(tblDataGrid, function (n) { return n.iId == id ? n : null })[0];
    $("#bIsEdit").val("1");
    $("#drpNganhEdit").val(dataCheck.iID_NganhID)
    $("#drpDuAnEdit").val(dataCheck.iID_DuAnID);
    $("#drpHopDongEdit").val(dataCheck.iID_HopDongID);
    $("#txtThanhToanTrongNam").val(dataCheck.fGiaTriThanhToan);
    $("#txtTamUng").val(dataCheck.fGiaTriTamUng);
    $("#txtThuHoiTamUng").val(dataCheck.fGiaTriThuHoi);
    $("#drpLoaiKhoan").change();
}

var arrNganh = [];
function GetDataDropdownNganh() {
    var iIdDuAn = $("#drpDuAn").val();
    var iIdMaDonViQuanLy = $("#drpDonViQuanLy").val();
    var iIdNguonVon = $("#drpNguonNganSach").val();
    var dNgayLap = $("#txtNgayDeNghi").val();
    var iNamKeHoach = $("#txtNamKeHoach").val();

    if (iIdDuAn == "" || iIdDuAn == GUID_EMPTY
        || iIdMaDonViQuanLy == "" || iIdMaDonViQuanLy == GUID_EMPTY
        || iIdNguonVon == ""
        || dNgayLap == ""
        || iNamKeHoach == "")
        return false;

    $.ajax({
        type: "POST",
        url: "/GiaiNganThanhToan/GetDataThongTinChiTietLoaiNganSach",
        data: {
            iIdDuAn: iIdDuAn,
            iIdMaDonViQuanLy: iIdMaDonViQuanLy,
            iIdNguonVon: iIdNguonVon,
            dNgayLap: dNgayLap,
            iNamKeHoach: iNamKeHoach
        },
        success: function (r) {
            if (r.bIsComplete) {
                arrNganh = r.data;
                $("#divBtnAdd").show();
            }
        }
    });
}

function GetDataDropdownNguonVon() {
    var iIdDuAn = $("#drpDuAn").val();
    if (iIdDuAn == GUID_EMPTY)
        return false;

    $.ajax({
        type: "POST",
        url: "/GiaiNganThanhToan/GetDataNguonVon",
        data: {
            iIdDuAn: iIdDuAn
        },
        success: function (data) {
            if (data != null && data != "") {
                $("#drpNguonNganSach").html(data);
                $("#drpNguonNganSach").trigger("change");
            }
        }
    });
}

function GetDataDropdownHopDong() {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/GetDataDropdownHopDong",
        data: {
            iIdDuAn: $("#drpDuAn option:selected").val().split("|")[0]
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#drpHopDong").empty()
                $.each(r.data, function (index, value) {
                    $("#drpHopDong").append("<option value='" + value.Value + "'>" + value.Text + "</option>");
                });
                $("#drpHopDong").prop("selectedIndex", -1);
            }
            if ($("#bIsEdit").val() == "1") {
                $("#drpHopDong").val($("#drpHopDongEdit").val());
                $("#drpHopDong").change();
            }
        }
    });
}

var iID_NhaThauID;
function GetDetailHopDong() {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/GetDetailHopDongDuAn",
        data: {
            iID_DuAnID: $("#drpDuAn option:selected").val().split("|")[0],
            iID_HopDongID: $("#drpHopDong option:selected").val(),
            dNgayDeNghi: $("#txtNgayPheDuyet").val(),
            iNamKeHoach: $("#txtNamKeHoach").val(),
            iID_NguonVonID: $("#drpNguonNganSach option:selected").val(),
            iID_NganhID: $("#drpNganh option:selected").val()
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#txtGiaTriHD").val(r.data.fGiaTriHD.toLocaleString('vi-VN'));
                $("#txtNgayHD").val(ConvertDatetimeToJSon(r.data.dNgayHopDong));
                $("#txtNganHang").val(r.data.sNganHang);
                $("#txtGoiThau").val(r.data.sTenGoiThau);
                $("#txtNhaThau").val(r.data.sTenNhaThau);
                $("#txtTaiKhoanNhaThau").val(r.data.sSoTaiKhoanNhaThau);
                $("#txtDaThanhToanTrongNam").val(r.data.fDaThanhToanTrongNam.toLocaleString('vi-VN'));
                $("#txtDaTamUng").val(r.data.fDaTamUng.toLocaleString('vi-VN'));
                $("#txtDaThuHoi").val(r.data.fDaThuHoi.toLocaleString('vi-VN'));
                $("#txtLuyKeThanhToan").val(r.data.fLuyKeThanhToanKLHT.toLocaleString('vi-VN'));
                iID_NhaThauID = r.data.iID_NhaThauThucHienID;
                $("#txtDuToanGoiThau").val(r.data.fDuToanGoiThau.toLocaleString('vi-VN'))
            }
        }
    });
}

var bLoaded = true;
function LoadLuyKeThanhToan() {
    if (bLoaded) {
        $("#txtlkttVonTrongNuoc").val("");
        $("#txtlkttVonNgoaiNuoc").val("");

        var iID_HopDongID = $("#drpHopDong option:selected").val();
        var dNgayDeNghi = $("#txtNgayPheDuyet").val();

        if (iID_HopDongID == undefined || iID_HopDongID == "" || iID_HopDongID == GUID_EMPTY || dNgayDeNghi == "")
            return false;

        bLoaded = false;
        $.ajax({
            type: "POST",
            url: "/QLVonDauTu/GiaiNganThanhToan/LoadGiaTriThanhToan",
            data: {
                iID_HopDongID: iID_HopDongID,
                dNgayDeNghi: dNgayDeNghi,
            },
            success: function (r) {
                $("#txtlkttVonTrongNuoc").val(FormatNumber(r.luyKeTN));
                $("#txtlkttVonNgoaiNuoc").val(FormatNumber(r.luyKeNN));
                bLoaded = true;
            }
        });
    }
}

//===================== Event button ==========================//

function CancelSaveData() {
    location.href = "/QLVonDauTu/GiaiNganThanhToan";
}

function Insert() {
    if (!ValidateDataInsert()) return;
    SaveDeNghiThanhToan();
}

function SaveDeNghiThanhToan() {
    var data = {};
    data.sSoDeNghi = $("#txtSoDeNghi").val();
    data.dNgayDeNghi = $("#txtNgayDeNghi").val();
    data.iID_DonViQuanLyID = $("#drpDonViQuanLy option:selected").val();
    data.sNguoiLap = $("#txtNguoiLap").val();
    data.iNamKeHoach = $("#txtNamKeHoach").val();
    data.iID_NguonVonID = $("#drpNguonNganSach option:selected").val();

    data.fGiaTriThanhToanTN = parseInt($("#txtdntuVonTrongNuoc").val() == "" ? 0 : UnFormatNumber($("#txtdntuVonTrongNuoc").val()));
    data.fGiaTriThanhToanNN = parseInt($("#txtdntuVonNgoaiNuoc").val() == "" ? 0 : UnFormatNumber($("#txtdntuVonNgoaiNuoc").val()));
    data.fGiaTriThuHoiTN = parseInt($("#txtthtuVonTrongNuoc").val() == "" ? 0 : UnFormatNumber($("#txtthtuVonTrongNuoc").val()));
    data.fGiaTriThuHoiNN = parseInt($("#txtthtuVonNgoaiNuoc").val() == "" ? 0 : UnFormatNumber($("#txtthtuVonNgoaiNuoc").val()));
    data.sGhiChu = $("#txtNoiDung").val();

    data.iLoaiThanhToan = $("#drpLoaiThanhToan option:selected").val();
    data.iID_DuAnId = $("#drpDuAn option:selected").val();
    data.iID_HopDongId = $("#drpHopDong option:selected").val();
    data.iID_NhaThauId = iID_NhaThauID;
    data.iCoQuanThanhToan = $("#drpCoQuanThanhToan option:selected").val();
    data.fThueGiaTriGiaTang = parseInt($("#txtthtuThueGTGT").val() == "" ? 0 : UnFormatNumber($("#txtthtuThueGTGT").val()));
    data.fChuyenTienBaoHanh = parseInt($("#txtthtuTienBaoHanh").val() == "" ? 0 : UnFormatNumber($("#txtthtuTienBaoHanh").val()));

    $.ajax({
        type: "POST",
        url: "/GiaiNganThanhToan/InsertDeNghiThanhToan",
        data: {
            data: data
        },
        success: function (r) {
            if (r.bIsComplete) {
                SavePheDuyetThanhToanChiTiet(r.data);
            } else {
                alert("Tạo thanh toán thất bại !");
            }
        }
    });
}

function SavePheDuyetThanhToanChiTiet(iID_DeNghiThanhToanID) {
    var data = GetListThanhToanChiTiet(iID_DeNghiThanhToanID);

    $.ajax({
        type: "POST",
        url: "/GiaiNganThanhToan/InsertPheDuyetThanhToanChiTiet",
        data: {
            lstData: data
        },
        success: function (r) {
            if (r.bIsComplete) {
                alert("Tạo mới thành công phiếu thanh toán !");
                location.href = "/QLVonDauTu/GiaiNganThanhToan";
            }
            else {
                alert("Tạo phiếu thanh toán chi tiết thất bại !");
            }
        }
    });
}

function GetListThanhToanChiTiet(iID_DeNghiThanhToanID) {
    var lstData = [];
    $("#" + TBL_DANH_SACH_THANH_TOAN_CHITIET + " tbody tr").each(function () {
        var iID_PheDuyetThanhToan_ChiTietID = $(this).find(".r_iID_ThanhToanChiTietID").val();
        var iLoai = $(this).find(".r_Loai select").val();
        var iID_NganhID = $(this).find(".r_Ng select").val();
        var sNg = $(this).find(".r_Ng select option:selected").text();
        var iLoaiNamKH = $(this).find(".r_Nam select").val();
        var fGiaTriNgoaiNuoc = parseInt(UnFormatNumber($(this).find(".r_vonnuocngoai").text()));
        var fGiaTriTrongNuoc = parseInt(UnFormatNumber($(this).find(".r_vontrongnuoc").text()));
        var sGhiChu = $(this).find(".r_ghichu").text();

        var arrTextNg = sNg.split("-");
        var sM = "", sTM = "", sTTM = "", sNG = "";
        if (arrTextNg.length == 4) {
            sM = arrTextNg[0].trim();
            sTM = arrTextNg[1].trim();
            sTTM = arrTextNg[2].trim();
            sNG = arrTextNg[3].trim();
        }

        lstData.push({
            iID_DeNghiThanhToanID: iID_DeNghiThanhToanID,
            iID_PheDuyetThanhToan_ChiTietID: iID_PheDuyetThanhToan_ChiTietID,
            iLoai: iLoai,
            iID_NganhID: iID_NganhID,
            sM: sM,
            sTM: sTM,
            sTTM: sTTM,
            sNG: sNG,
            iLoaiNamKH: iLoaiNamKH,
            fGiaTriNgoaiNuoc: fGiaTriNgoaiNuoc,
            fGiaTriTrongNuoc: fGiaTriTrongNuoc,
            sGhiChu: sGhiChu
        })
    })
    return lstData;
}

//=========================== validate ===========//
function ValidateDataInsert() {
    var sMessError = [];
    if ($("#drpDonViQuanLy").val() == null || $("#drpDonViQuanLy").val() == GUID_EMPTY) {
        sMessError.push("Chưa nhập đơn vị quản lý.");
    }
    if ($("#iID_ChuDauTuID").val() == null || $("#iID_ChuDauTuID").val() == GUID_EMPTY) {
        sMessError.push("Chưa nhập chủ đầu tư.");
    }
    if ($("#txtSoDeNghi").val().trim() == "") {
        sMessError.push("Chưa nhập số đề nghị.");
    }
    if ($("#txtNgayDeNghi").val().trim() == "") {
        sMessError.push("Chưa nhập ngày đề nghị.");
    }
    if ($("#txtNamKeHoach").val().trim() == "") {
        sMessError.push("Chưa nhập năm kế hoạch.");
    }
    if ($("#drpNguonNganSach").val() == null) {
        sMessError.push("Chưa nhập nguồn vốn.");
    }
    if ($("#drpDuAn").val() == null || $("#drpDuAn").val() == GUID_EMPTY) {
        sMessError.push("Chưa nhập nhập dự án.");
    }
    if ($("#" + TBL_DANH_SACH_THANH_TOAN_CHITIET + " tbody tr").length == 0) {
        sMessError.push("Chưa nhập thông tin chi tiết !");
    }
    if (sMessError.length != 0) {
        alert(sMessError.join('\n'));
        return false;
    }
    return true;
}

function ThemMoiThanhToanChiTiet() {
    var iID_ThanhToanChiTietID = uuidv4();
    var dongMoi = "";
    dongMoi += "<tr style='cursor: pointer;' class='parent'>";
    dongMoi += "<td class='r_STT' align='center'></td>";
    dongMoi += "<input type='hidden' class='r_iID_ThanhToanChiTietID' value='" + iID_ThanhToanChiTietID + "'/>";
    dongMoi += "<td class='r_Loai'>" + CreateHtmlSelectLoai() + "</td>";
    dongMoi += "<td class='r_Ng'>" + CreateHtmlSelectNganh() + "</td>";
    dongMoi += "<td class='r_Nam'>" + CreateHtmlSelectNam() + "</td>";
    dongMoi += "<td class='r_tongtien sotien' align='right'></td>";
    dongMoi += "<td class='r_vontrongnuoc sotien' contenteditable='true' align='right'></td>";
    dongMoi += "<td class='r_vonnuocngoai sotien' contenteditable='true' align='right'></td>";
    dongMoi += "<td class='r_KeHoachVon'>" + CreateHtmlSelectKeHoachVon() + "</td>";
    dongMoi += "<td class='r_ghichu' contenteditable='true'>";
    dongMoi += "<td align='center'><button class='btn-delete btn-icon' type='button' onclick='XoaDong(this)'><span class='fa fa-trash-o fa-lg' aria-hidden='true'></span></button></td>";
    $("#" + TBL_DANH_SACH_THANH_TOAN_CHITIET + " tbody").append(dongMoi);
    CapNhatCotStt(TBL_DANH_SACH_THANH_TOAN_CHITIET);

    EventValidate();

    $("#tblDanhSachThanhToanChiTiet tbody tr:last-child td.r_Loai select").trigger("change");

}

function CreateHtmlSelectLoai(value) {
    var htmlOption = "";
    var loaiThanhToan = $("#drpLoaiThanhToan").val();

    if (loaiThanhToan == THANH_TOAN) {
        htmlOption += "<option value='1'>Thanh toán</option>";
        htmlOption += "<option value='2'>Thu hồi ứng</option>";
    } else if (loaiThanhToan == TAM_UNG) {
        htmlOption += "<option value='3'>Tạm ứng</option>";
    }
    return "<select class='form-control'>" + htmlOption + "</option>";
}

function CreateHtmlSelectNam(loaiThanhToan) {
    var htmlOption = "";
    if (loaiThanhToan == THANH_TOAN || loaiThanhToan == TAM_UNG) {
        htmlOption += "<option value='1'>Năm nay</option>";
    } else if (loaiThanhToan == THU_HOI_UNG) {
        htmlOption += "<option value='1'>Năm nay</option>";
        htmlOption += "<option value='0'>Năm trước</option>";
    }
    return "<select class='form-control'>" + htmlOption + "</option>";
}

function CreateHtmlSelectKeHoachVon(value) {
    var htmlOption = "";
    //var loaiThanhToan = $("#drpLoaiThanhToan").val();

    //if (loaiThanhToan == THANH_TOAN) {
    //    htmlOption += "<option value='1'>Thanh toán</option>";
    //    htmlOption += "<option value='2'>Thu hồi ứng</option>";
    //} else if (loaiThanhToan == TAM_UNG) {
    //    htmlOption += "<option value='3'>Tạm ứng</option>";
    //}
    return "<select class='form-control'>" + htmlOption + "</option>";
}

function CreateHtmlSelectNganh(value) {
    var htmlOption = "";
    arrNganh.forEach(x => {
        if (value != undefined && value == x.Value)
            htmlOption += "<option value='" + x.Value + "' selected>" + x.Text + "</option>";
        else
            htmlOption += "<option value='" + x.Value + "'>" + x.Text + "</option>";
    })
    return "<select class='form-control'>" + htmlOption + "</option>";
}

function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

function CapNhatCotStt(idBang) {
    $("#" + idBang + " tbody tr.parent").each(function (index, tr) {
        $(tr).find('.r_STT').text(index + 1);
    });
}

function XoaDong(nutXoa) {
    var dongXoa = nutXoa.parentElement.parentElement;
    dongXoa.parentNode.removeChild(dongXoa);
    CapNhatCotStt(TBL_DANH_SACH_THANH_TOAN_CHITIET);
}

// event
function EventValidate() {
    $("td.sotien[contenteditable='true']").on("keypress", function (event) {
        return ValidateNumberKeyPress(this, event);
    })
    $("td.sotien[contenteditable='true']").on("focusout", function (event) {
        $(this).html(FormatNumber($(this).html() == "" ? 0 : UnFormatNumber($(this).html())));

        // tinh tong so tien
        var thisDong = this.parentElement.parentElement;

        var fTrongNuoc = $(thisDong).find(".r_vontrongnuoc").html().trim();
        var fNuocNgoai = $(thisDong).find(".r_vonnuocngoai").html().trim();

        var fTong = parseInt(fTrongNuoc == "" ? 0 : UnFormatNumber(fTrongNuoc))
            + parseInt(fNuocNgoai == "" ? 0 : UnFormatNumber(fNuocNgoai));

        $(thisDong).find(".r_tongtien").html(FormatNumber(fTong));
    })

    $("td[contenteditable='true']").on("keydown", function (e) {
        var key = e.keyCode || e.charCode;
        if (key == 13) {
            $(this).blur();
        }
    });

    $("td.r_Loai select").change(function (e) {
        var thisDong = this.parentElement.parentElement;
        $(thisDong).find(".r_Nam").html(CreateHtmlSelectNam($(this).val()));
    })
}