$("#txtNamKeHoach").trigger("change");
$("#drpLoaiNganSach").trigger("change");
$("#drpLoaiKhoan").trigger("change");
$("#drpNganh").trigger("change");
$("#drpDuAn").trigger("change");
$("#drpHopDong").trigger("change");

var iNamKhoiTao = $("#txtiNamKhoiTao").val();
var tblDataGrid = [];

var arrNhaThau = [];
var arrChiTiet = [];
var objKhoiTao = {};
var objDuToan = {};
var objQDDT = {};
var objDuAn = {};

var bIsDuAnCu = "";
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var TBL_TIEN_LUY_KE = "tblListTienLuyKe";

$(document).ready(function () {
    GetDropdownLoaiCongTrinh();
    bIsDuAnCu = $("#bIsDuAnCu").is(":checked");

    if (bIsDuAnCu) {
        // du an da ton tai
        $("#textboxDuAn").hide();
        $("#dropDuAn").show();

        $("#sMaDuAn").attr("disabled", true);
        $("#iID_ChuDauTuID").attr("disabled", true);
        $("#txtLoaiCongTrinh").attr("disabled", true);
        $("#iID_CapPheDuyetID").attr("disabled", true);
        $("#iID_DonViQuanLyID").attr("disabled", true);
        $("#sKhoiCong").attr("disabled", true);
        $("#sKetThuc").attr("disabled", true);

        $("#sSoQDDT").attr("disabled", true);
        $("#dNgayPheDuyetQDDT").attr("disabled", true);
        $("#sCoQuanPheDuyetQDDT").attr("disabled", true);
        $("#sNguoiKyQDDT").attr("disabled", true);

        $("#sSoQuyetDinhTKDT").attr("disabled", true);
        $("#dNgayDuyetTKDT").attr("disabled", true);
        $("#sCoQuanPheDuyetTKDT").attr("disabled", true);
        $("#sNguoiKyTKDT").attr("disabled", true);

        $("#divThongTinDuAn .date").find("span").hide();
    } else {
        $("#textboxDuAn").show();
        $("#dropDuAn").hide();

        $("#sMaDuAn").removeAttr("disabled");
        $("#iID_ChuDauTuID").removeAttr("disabled");
        $("#txtLoaiCongTrinh").removeAttr("disabled");
        $("#iID_CapPheDuyetID").removeAttr("disabled");
        $("#iID_DonViQuanLyID").removeAttr("disabled");
        $("#sKhoiCong").removeAttr("disabled");
        $("#sKetThuc").removeAttr("disabled");

        $("#sSoQDDT").removeAttr("disabled");
        $("#dNgayPheDuyetQDDT").removeAttr("disabled");
        $("#sCoQuanPheDuyetQDDT").removeAttr("disabled");
        $("#sNguoiKyQDDT").removeAttr("disabled");

        $("#sSoQuyetDinhTKDT").removeAttr("disabled");
        $("#dNgayDuyetTKDT").removeAttr("disabled");
        $("#sCoQuanPheDuyetTKDT").removeAttr("disabled");
        $("#sNguoiKyTKDT").removeAttr("disabled");

        $("#divThongTinDuAn .date").find("span").show();
    }

    $("#drpLoaiNganSach").change(function (e) {
        $("#drpNganh").empty();
        GetDataDropdownNganh();
    });

    $("#txtKHVonUng, #txtVonUngDaCap").change(function (e) {
        var khVonUng = UnFormatNumber($("#txtKHVonUng").val());
        var vonUngDaCap = UnFormatNumber($("#txtVonUngDaCap").val());
        $("#txtGiaTriConPhaiUng").val(FormatNumber(khVonUng - vonUngDaCap));
    })

    $("#txtKHVonBoTriHetNamTruoc, #txtLuyKeThanhToan, #txtSoDuUng").change(function (e) {
        var khVonNamTruoc = UnFormatNumber($("#txtKHVonBoTriHetNamTruoc").val());
        var luyKeThanhToan = UnFormatNumber($("#txtLuyKeThanhToan").val());
        var soDuUng = UnFormatNumber($("#txtSoDuUng").val());
        $("#txtKeHoachVonChuaCap").val(FormatNumber(khVonNamTruoc - luyKeThanhToan - soDuUng));
    })

    tblDataGrid = JSON.parse($("#arrChiTiet").val());
    arrNhaThau = JSON.parse($("#arrChiTietNhaThau").val());
    DinhDangSo();
    RenderGridView(tblDataGrid);
    
})

function openPopupCapNhatTienLuyKe() {
    $("#" + TBL_TIEN_LUY_KE + " tbody").html("");
    var messErr = [];
    var iIDNguonVon = $("#drpNguonNganSach").val();
    var iIDLoaiNganSach = $("#drpLoaiNganSach").val().split("|")[1];
    var iIDNganh = $("#drpNganh").val();

    if (iIDNguonVon == "" || iIDNguonVon == GUID_EMPTY) {
        messErr.push("Chưa có thông tin nguồn vốn.");
    }
    if (iIDLoaiNganSach == "" || iIDLoaiNganSach == GUID_EMPTY) {
        messErr.push("Chưa có thông tin loại ngân sách");
    }
    if (iIDNganh == "" || iIDNganh == null || iIDNganh == GUID_EMPTY) {
        messErr.push("Chưa có thông tin ngành");
    }

    if (messErr.length > 0) {
        alert(messErr.join("\n"));
    } else {
        var nhaThau = arrNhaThau.filter(function (x) { return x.iID_NganhID == iIDNganh && x.iID_NguonVonId == iIDNguonVon && x.iID_LoaiNguonVonID == iIDLoaiNganSach });
        var html = "";
        nhaThau.forEach(function (obj) {
            var dongMoi = "";
            dongMoi += "<tr style='cursor: pointer;'>";
            dongMoi += "<td class='r_STT'></td>";
            dongMoi += "<td align='left' class='r_sTenNhaThau'><input type='hidden' class='r_iIDNhaThau' value='" + obj.iID_NhaThauID + "'/>" + obj.sTenNhaThau + "</td>";
            dongMoi += "<td align='right' class='r_fGiaTriUng'>" + (obj.fGiaTriUng == "" ? 0 : FormatNumber(obj.fGiaTriUng)) + "</td>";
            dongMoi += "<td><button class='btn btn-danger btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_TIEN_LUY_KE + "\")'>" +
                "<span class='fa fa-minus fa-lg' aria-hidden='true'></span>" +
                "</button></td>";
            dongMoi += "</tr>";
            html += dongMoi;
        })

        $("#" + TBL_TIEN_LUY_KE + " tbody").append(html);
        CapNhatCotStt(TBL_TIEN_LUY_KE);
        // tinh dong tong
        TinhLaiDongTong(TBL_TIEN_LUY_KE);
        $("#modalLuyKeThanhToanTamUng").modal("show");
    }
}

function GetDropdownLoaiCongTrinh() {
    $.ajax({
        url: "/QLVonDauTu/ChuTruongDauTu/FillDropdown",
        type: "POST",
        data: "",
        dataType: "json",
        cache: false,
        success: function (data) {
            trLoaiCongTrinh = $('#txtLoaiCongTrinh').comboTree({
                source: data,
                isMultiple: false,
                cascadeSelect: false,
                collapse: true,
                selectableLastNode: false,
                selected: [$("#drpLoaicongTrinh").val()]
            });
            trLoaiCongTrinh.onChange(function () {
                $("#drpLoaicongTrinh").val(trLoaiCongTrinh.getSelectedIds());
            });
        },
        error: function (data) {

        }
    });
}

//====================== Dropdown event ========================//
function GetDataDropdownLoaiNganSach() {
    $.ajax({
        type: "POST",
        url: "/QLKeHoachVonNam/GetDataDropdownLoaiNganSach",
        data: {
            iNamKeHoach: iNamKhoiTao
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#drpLoaiNganSach").empty()
                $.each(r.data, function (index, value) {
                    $("#drpLoaiNganSach").append("<option value='" + value.Value + "'>" + value.Text + "</option>");
                });
                $("#drpLoaiNganSach").prop("selectedIndex", -1);
            }
        }
    });
}

function GetDataDropdownNganh() {
    $.ajax({
        type: "POST",
        url: "/QLKhoiTaoThongTinDuAnChuyenTiep/GetDataThongTinChiTietLoaiNganSach",
        data: {
            sLNS: $("#drpLoaiNganSach").val().split("|")[0],
            iNamKeHoach: iNamKhoiTao
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#drpNganh").empty();
                $.each(r.data, function (index, value) {
                    $("#drpNganh").append("<option value='" + value.Value + "'>" + value.Text + "</option>");
                });
                $("#drpNganh").prop("selectedIndex", -1);
            }
        }
    });
}

function GetDataDropdownNganhEdit(iId) {
    $.ajax({
        type: "POST",
        url: "/GiaiNganThanhToan/GetDataThongTinChiTietLoaiNganSachByNganh",
        data: {
            iId_Nganh: iId,
            iNamKeHoach: iNamKhoiTao
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#drpNganh").empty();
                $.each(r.data, function (index, value) {
                    $("#drpNganh").append("<option value='" + value.Value + "'>" + value.Text + "</option>");
                });
                $("#drpNganh").prop("selectedIndex", -1);
            }
        }
    });
}

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
            iID_LoaiNguonVonID: $("#drpLoaiNganSach option:selected").val().split("|")[1],
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
                $("#iIdNhaThau").val(r.data.iID_NhaThauThucHienID);
                $("#txtDuToanGoiThau").val(r.data.fDuToanGoiThau.toLocaleString('vi-VN'))
            }
        }
    });
}

function AddThongTinChiTiet() {
    if (ValidateThemChiTiet() == false)
        return;

    var objKhoiTaoChiTiet = {};
    objKhoiTaoChiTiet.iID_NguonVonID = $("#drpNguonNganSach").val();
    objKhoiTaoChiTiet.sTenNguonVon = $("#drpNguonNganSach option:selected").text();
    objKhoiTaoChiTiet.fKHVonHetNamTruoc = UnFormatNumber($("#txtKHVonBoTriHetNamTruoc").val());
    objKhoiTaoChiTiet.fLuyKeThanhToanKLHT = UnFormatNumber($("#txtLuyKeThanhToan").val());
    objKhoiTaoChiTiet.fLuyKeThanhToanTamUng = UnFormatNumber($("#txtSoDuUng").val());
    objKhoiTaoChiTiet.fThanhToanQuaKB = UnFormatNumber($("#txtVonThanhToanKLHT").val());
    objKhoiTaoChiTiet.fTamUngQuaKB = UnFormatNumber($("#txtTUTheoCheDoChuaThuHoi").val());
    objKhoiTaoChiTiet.iID_NganhID = $("#drpNganh option:selected").val();
    objKhoiTaoChiTiet.sTenNganh = $("#drpNganh option:selected").text();
    objKhoiTaoChiTiet.iID_LoaiNguonVonID = $("#drpLoaiNganSach option:selected").val().split("|")[1];
    objKhoiTaoChiTiet.iID_LoaiNguonVonIDValue = $("#drpLoaiNganSach option:selected").val();
    objKhoiTaoChiTiet.sTenLoaiNguonVon = $("#drpLoaiNganSach option:selected").text();

    objKhoiTaoChiTiet.sTenDuAn = $("#sTenDuAn").val();
    objKhoiTaoChiTiet.sMaDuAn = $("#sMaDuAn").val();
    objKhoiTaoChiTiet.iID_ChuDauTu = $("#iID_ChuDauTuID").val();
    objKhoiTaoChiTiet.sTenChuDauTu = $("#iID_ChuDauTuID option:selected").text();

    objKhoiTaoChiTiet.sSoQDDT = $("#sSoQDDT").val();
    objKhoiTaoChiTiet.dNgayPheDuyetQDDT = $("#dNgayPheDuyetQDDT").val();
    objKhoiTaoChiTiet.sNgayPheDuyetQDDT = $("#dNgayPheDuyetQDDT").val();
    objKhoiTaoChiTiet.fTongMucDauTu = UnFormatNumber($("#txtGiaTriDauTu").val());

    objKhoiTaoChiTiet.sSoQuyetDinhTKDT = $("#sSoQuyetDinhTKDT").val();
    objKhoiTaoChiTiet.dNgayPheDuyetTKDT = $("#dNgayDuyetTKDT").val();
    objKhoiTaoChiTiet.sNgayPheDuyetTKDT = $("#dNgayDuyetTKDT").val();
    objKhoiTaoChiTiet.fGiaTriDuToan = UnFormatNumber($("#txtGiaTriDuToan").val());

    objKhoiTaoChiTiet.fKHVonChuaCap = UnFormatNumber($("#txtKeHoachVonChuaCap").val());

    objKhoiTaoChiTiet.fSoChuyenChiTieuDaCap = UnFormatNumber($("#txtSoChuyenChiTieuDaCap").val());
    objKhoiTaoChiTiet.fSoChuyenChiTieuChuaCap = UnFormatNumber($("#txtSoChuyenChiTieuChuaCap").val());
    objKhoiTaoChiTiet.iId = tblDataGrid.length + 1;

    tblDataGrid = $.map(tblDataGrid, function (n) { return n.iID_NguonVonID == objKhoiTaoChiTiet.iID_NguonVonID && n.iID_NganhID == objKhoiTaoChiTiet.iID_NganhID && n.iID_LoaiNguonVonID == objKhoiTaoChiTiet.iID_LoaiNguonVonID ? null : n });

    tblDataGrid.push(objKhoiTaoChiTiet);
    RenderGridView(tblDataGrid);
    SuKienDbClickDongTable();
    ClearThongTinChiTiet();
}

function ConvertListDetailToGrid(lstData) {
    tblDataGrid = [];
    $.each(lstData, function (index, item) {
        var objThanhToan = {};
        objThanhToan.iId = item.iID_DeNghiThanhToan_ChiTietID;
        objThanhToan.iID_NganhID = item.iID_NganhID;
        objThanhToan.sNganh = item.sNganh;
        objThanhToan.iID_DuAnID = item.iID_DuAnID;
        objThanhToan.sTenDuAn = item.sTenDuAn;
        objThanhToan.iID_HopDongID = item.iID_HopDongID;
        objThanhToan.sSoHopDong = item.sSoHopDong;
        objThanhToan.iID_NhaThauID = item.iID_NhaThauID;
        objThanhToan.sTenNhaThau = item.sTenNhaThau;
        objThanhToan.fGiaTriDaThanhToan = 0;
        objThanhToan.fLuyKeThanhToanKLHT = (item.fLuyKeThanhToanKLHT ?? 0).toLocaleString('vi-VN');
        objThanhToan.fChiTieuNganSachNam = (item.fChiTieuNganSachNam ?? 0).toLocaleString('vi-VN');
        objThanhToan.sNganHang = item.sNganHang;
        objThanhToan.sDonViThuHuong = item.sDonViThuHuong;

        objThanhToan.fGiaTriTamUng = (item.fGiaTriTamUng ?? 0).toLocaleString('vi-VN');
        objThanhToan.fGiaTriThuHoi = (item.fGiaTriThuHoi ?? 0).toLocaleString('vi-VN');
        objThanhToan.fGiaTriThanhToan = (item.fGiaTriThanhToan ?? 0).toLocaleString('vi-VN');
        objThanhToan.fDuToanGoiThau = (item.fDuToanGiaGoiThau ?? 0).toLocaleString('vi-VN');

        objThanhToan.fSoThucThanhToan = ((item.fGiaTriThanhToan ?? 0) + (item.fGiaTriTamUng ?? 0)) - (item.fGiaTriThuHoi ?? 0).toLocaleString('vi-VN');

        tblDataGrid = $.map(tblDataGrid, function (n) { return n.iID_NguonVonID == objKhoiTaoChiTiet.iID_NguonVonID && n.iID_NganhID == objKhoiTaoChiTiet.iID_NganhID && n.iID_LoaiNguonVonID == objKhoiTaoChiTiet.iID_LoaiNguonVonID ? null : n });

        tblDataGrid.push(objThanhToan);
        RenderGridView(tblDataGrid);
        ClearThongTinChiTiet();
    });
    $("#ViewTable button").css("display", "none");
}

function RenderGridView(data) {
    var columns = [
        { sField: "iId", bKey: true },
        { sField: "iIdRefer", bForeignKey: true },
        { sField: "iParentId", bParentKey: true },
        { sTitle: " ", sField: "", iWidth: "200px", sTextAlign: "left" },
        { sTitle: "M-TM-TM-N", sField: "sTenNganh", iWidth: "200px", sTextAlign: "left" },
        { sTitle: "Nguồn vốn", sField: "sTenNguonVon", iWidth: "500px", sTextAligsn: "left" },
        { sTitle: "Loại ngân sách", sField: "sTenLoaiNguonVon", iWidth: "400px", sTextAlign: "left" },
        { sTitle: "Mã dự án", sField: "sMaDuAn", iWidth: "200px", sTextAlign: "left" },
        { sTitle: "Chủ đầu tư", sField: "sTenChuDauTu", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Số quyết định", sField: "sSoQDDT", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Ngày phê duyệt", sField: "sNgayPheDuyetQDDT", iWidth: "200px", sTextAlign: "center" },
        { sTitle: "Tổng mức đầu tư", sField: "fTongMucDauTu", iWidth: "200px", sTextAlign: "right", sClass: "sotien" },
        { sTitle: "Số quyết định", sField: "sSoQuyetDinhTKDT", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Ngày phê duyệt", sField: "sNgayPheDuyetTKDT", iWidth: "200px", sTextAlign: "center" },
        { sTitle: "Giá trị dự toán", sField: "fGiaTriDuToan", iWidth: "300px", sTextAlign: "right", sClass: "sotien" },
        { sTitle: "KH vốn bố trí hết năm trước", sField: "fKHVonHetNamTruoc", iWidth: "400px", sTextAlign: "right", sClass: "sotien" },
        { sTitle: "Kế hoạch vốn chưa cấp", sField: "fKHVonChuaCap", iWidth: "400px", sTextAlign: "right", sClass: "sotien" },
        { sTitle: "Thanh toán KLHT", sField: "fLuyKeThanhToanKLHT", iWidth: "400px", sTextAlign: "right", sClass: "sotien" },
        { sTitle: "T/Ư theo chế độ chưa thu hồi", sField: "fLuyKeThanhToanTamUng", iWidth: "400px", sTextAlign: "right", sClass: "sotien" },
        { sTitle: "Vốn thanh toán KLHT(qua KB)", sField: "fThanhToanQuaKB", iWidth: "400px", sTextAlign: "right", sClass: "sotien" },
        { sTitle: "T/Ư theo chế độ chưa thu hồi(qua KB)", sField: "fTamUngQuaKB", iWidth: "400px", sTextAlign: "right", sClass: "sotien" },
        { sTitle: "Đã cấp", sField: "fSoChuyenChiTieuDaCap", iWidth: "400px", sTextAlign: "right", sClass: "sotien" },
        { sTitle: "Chưa cấp", sField: "fSoChuyenChiTieuChuaCap", iWidth: "400px", sTextAlign: "right", sClass: "sotien" }
    ];

    var button = { bCreateButtonInParent: 0, bUpdate: 0, bDelete: 1 };
    var sHtml = GenerateTreeTable(data, columns, button)
    $("#ViewTable").css("width", $("#ViewTable").width() + "px");
    $("#ViewTable").html(sHtml);
    AddColspanTable();
    addTableNhaThau();
    DinhDangSo("ViewTable");
    SuKienDbClickDongTable();
}

//===================== Event button ==========================//

function Huy() {
    location.href = "/QLVonDauTu/QLKhoiTaoThongTinDuAnChuyenTiep";
}

function LuuData() {
    if (!ValidateLuuKhoiTao()) return;

    objKhoiTao = {
        iID_KhoiTaoID: $("#iID_KhoiTaoID").val(),
        iNamKhoiTao: iNamKhoiTao,
        iID_DonViID: $("#iID_DonViQuanLyID").val(),
        fKHVonUng: UnFormatNumber($("#txtKHVonUng").val()),
        fVonUngDaCap: UnFormatNumber($("#txtVonUngDaCap").val()),
        fVonUngDaThuHoi: UnFormatNumber($("#txtVonUngDaThuHoi").val()),
        fGiaTriConPhaiUng: UnFormatNumber($("#txtGiaTriConPhaiUng").val()),
        bIsDuAnCu: $("#bIsDuAnCu").is(":checked")
    };

    objDuToan = {
        iID_DuToanID: $("#iID_DuToanID").val(),
        sSoQuyetDinh: $("#sSoQuyetDinhTKDT").val(),
        dNgayQuyetDinh: $("#dNgayDuyetTKDT").val(),
        sCoQuanPheDuyet: $("#sCoQuanPheDuyetTKDT").val(),
        sNguoiKy: $("#sNguoiKyTKDT").val(),
        fTongDuToanPheDuyet: 0,
        bActive: true,
        bLaThayThe: false,
        bIsGoc: true,
        bLaTongDuToan: true
    }

    objQDDT = {
        iID_QDDauTuID: $("#iID_QDDauTuID").val(),
        sSoQuyetDinh: $("#sSoQDDT").val(),
        dNgayQuyetDinh: $("#dNgayPheDuyetQDDT").val(),
        sCoQuanPheDuyet: $("#sCoQuanPheDuyetQDDT").val(),
        sNguoiKy: $("#sNguoiKyQDDT").val(),
        fTongMucDauTuPheDuyet: 0,
        bActive: true,
        bIsGoc: true,
        bLaThayThe: false
    }

    objDuAn = {
        iID_DuAnID: $("#iID_DuAnID").val(),
        sTenDuAn: $("#sTenDuAn").val(),
        sMaDuAn: $("#sMaDuAn").val(),
        iID_ChuDauTuID: $("#iID_ChuDauTuID").val(),
        iID_CapPheDuyetID: $("#iID_CapPheDuyetID").val(),
        iID_LoaiCongTrinhID: $("#drpLoaicongTrinh").val(),
        iID_DonViQuanLyID: $("#iID_DonViQuanLyID").val(),
        sKhoiCong: $("#sKhoiCong").val(),
        sKetThuc: $("#sKetThuc").val()
    }

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKhoiTaoThongTinDuAnChuyenTiep/LuuKhoiTao",
        data: {
            objKhoiTao: objKhoiTao, lstKhoiTaoChiTiet: tblDataGrid, objDuToan: objDuToan, objQDDT: objQDDT, objDuAn: objDuAn, lstNhaThau: arrNhaThau,
            isUpdate: 1
        },
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/QLVonDauTu/QLKhoiTaoThongTinDuAnChuyenTiep";
            }
            else {
                alert("Sửa thông tin khởi tạo dự án chuyển tiếp thất bại.");
                return false;
            }
        }
    });
    return true;
}

//===================== Reset data ============================//
function ClearThongTinChiTiet() {
    $("#drpNguonNganSach").prop("selectedIndex", 0);
    $("#drpLoaiNganSach").prop("selectedIndex", 0).trigger("change");

    $("#txtGiaTriDauTu").val("");
    $("#txtGiaTriDuToan").val("");
    $("#txtSoDuUng").val("");
    $("#txtKHVonBoTriHetNamTruoc").val("");
    $("#txtLuyKeThanhToan").val("");
    $("#txtSoChuyenChiTieuDaCap").val("");
    $("#txtSoChuyenChiTieuChuaCap").val("");
    $("#txtVonThanhToanKLHT").val("");
    $("#txtTUTheoCheDoChuaThuHoi").val("");
    $("#txtKeHoachVonChuaCap").val("");
}

//===================== update css ===========================//
function AddColspanTable() {
    var index = 0;
    $(".table-parent th").each(function () {
        ++index;
        if (index == 6) {
            $(this).attr('colspan', 2);
            $(this).html("Thông tin dự án")
            return true;
        }
        if (index == 7) {
            $(this).attr('colspan', 3);
            $(this).html("Quyết định đầu tư")
            return true;
        }
        if (index == 8) {
            $(this).attr('colspan', 3);
            $(this).html("Thiết kế dự toán")
            return true;
        }

        if (index == 16) {
            $(this).attr('colspan', 4);
            $(this).html("Thanh toán kế hoạch vốn")
            return true;
        }

        if (index == 17) {
            $(this).attr('colspan', 2);
            $(this).html("Chuyển chỉ tiêu năm sau")
            return true;
        }
        if (index)
            $(this).attr('rowspan', 2);
    });

    index = 0;
    $(".table-parent th").each(function () {
        ++index;
        if (index == 9 || index == 10 || index == 11 || index == 12 || index == 13
            || index == 18 || index == 19 || index == 20 || index == 21) {
            $(this).remove();
            return true;
        }
    });

    var row = "<tr><th width='200px'>Mã dự án</th><th width='200px'>Chủ đầu tư</th>" +
        "<th width='200px'>Số quyết định</th><th width='200px'>Ngày phê duyệt</th><th width='200px'>Tổng mức đầu tư</th>" +
        "<th width='200px'>Số quyết định</th><th width='200px'>Ngày phê duyệt</th><th width='200px'>Giá trị dự toán</th>" +
        "<th width='200px'>Thanh toán KLHT</th><th width='200px'>T/Ư theo chế độ chưa thu hồi</th><th width='200px'>Vốn thanh toán KLHT(qua KB)</th><th width='200px'>T/Ư theo chế độ chưa thu hồi(qua KB)</th>" +
        "<th width='200px'>Đã cấp</th><th width='200px'>Chưa cấp</th>";
    "</tr>";
    $(".table-parent tr:first").after(row);
    $(".table-parent .fa-caret-down").closest("tr").find("[type=button]").remove();
}

//============================== Event Edit =================================//
function SaveThongTinChung() {
    data = {};
    data.iID_DeNghiThanhToanID = $("#iID_DeNghiThanhToanID").val();
    data.sSoPheDuyet = $("#txtSoPheDuyet").val();
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/UpdateDeNghiThanhToan",
        data: {
            data: data
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#txtSoPheDuyetOld").val(data.sSoPheDuyet);
                EditThongTinChung_Off();
            }
        }
    });
}

function GetNganSachThanhToanChiTiet() {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/GetGiaiNganThanhToanChiTiet",
        data: {
            iId: $("#iID_DeNghiThanhToanID").val()
        },
        success: function (r) {
            if (r.bIsComplete) {
                ConvertListDetailToGrid(r.data);
            }
        }
    });
}

function CancelSaveThongTinChung() {
    $("#txtSoPheDuyet").val($("#txtSoPheDuyetOld").val());
    EditThongTinChung_Off();
}

function EditThongTinChung_On() {
    $("button.show-edit-on").css("display", "");
    $("button.show-edit-off").css("display", "none");

    $("#txtSoPheDuyet").removeAttr("disabled");
}

function EditThongTinChung_Off() {
    $("button.show-edit-on").css("display", "none");
    $("button.show-edit-off").css("display", "");

    $("#txtSoPheDuyet").attr("disabled", "disabled")
}

function SaveThongTinChiTiet() {
    if (UpdateDataDetail()) {
        EditThongTinChiTiet_Off();
    }
}

function CancelSaveThongChiTiet() {
    EditThongTinChiTiet_Off();
    GetNganSachThanhToanChiTiet();
}

function EditThongTinChiTiet_On() {
    $("button.show-edit-detail-on").css("display", "");
    $("button.show-edit-detail-off").css("display", "none");

    $("#pnlThongTinChiTiet").css("display", "");
    $("#ViewTable button").css("display", "");
}

function EditThongTinChiTiet_Off() {
    $("button.show-edit-detail-on").css("display", "none");
    $("button.show-edit-detail-off").css("display", "");

    $("#pnlThongTinChiTiet").css("display", "none");
    $("#ViewTable button").css("display", "none");
}

//=======================Tao moi=============================//
function ThemMoiGiaTriUng() {
    var iIDNguonVon = $("#drpNguonNganSach").val();
    var iIDLoaiNganSach = $("#drpLoaiNganSach option:selected").val().split("|")[1];
    var iIDNganh = $("#drpNganh").val();

    var sTenNhaThau = $("#iID_NhaThauID :selected").html();
    var iIDNhaThau = $("#iID_NhaThauID").val();
    var fGiaTriUng = $("#txtGiaTriUng").val();

    var messErr = [];
    if (iIDNhaThau == "" || iIDNhaThau == GUID_EMPTY) {
        messErr.push("Thông tin nhà thầu chưa có hoặc chưa chính xác.");
    }
    if (fGiaTriUng == "" || parseInt(UnFormatNumber(fGiaTriUng) <= 0)) {
        messErr.push("Giá trị ứng chưa có và phải lớn hơn 0.");
    }

    if (messErr.length > 0) {
        alert(messErr.join("\n"));
    } else {
        if (arrNhaThau.filter(function (x) { return x.iID_NganhID == iIDNganh && x.iID_NguonVonId == iIDNguonVon && x.iID_LoaiNguonVonID == iIDLoaiNganSach && x.iID_NhaThauID == iIDNhaThau }).length > 0) {
            $("#" + TBL_TIEN_LUY_KE + " tbody tr").each(function (index, row) {
                if ($(row).find(".r_iIDNhaThau").val() == iIDNhaThau) {
                    $(row).remove();
                }
            })
            arrNhaThau = arrNhaThau.filter(function (x) { return !(x.iID_NganhID == iIDNganh && x.iID_NguonVonId == iIDNguonVon && x.iID_LoaiNguonVonID == iIDLoaiNganSach && x.iID_NhaThauID == iIDNhaThau) });
        }

        var dongMoi = "";
        dongMoi += "<tr style='cursor: pointer;'>";
        dongMoi += "<td class='r_STT'></td>";
        dongMoi += "<td align='left' class='r_sTenNhaThau'><input type='hidden' class='r_iIDNhaThau' value='" + iIDNhaThau + "'/>" + sTenNhaThau + "</td>";
        dongMoi += "<td align='right' class='r_fGiaTriUng'>" + (fGiaTriUng == "" ? 0 : fGiaTriUng) + "</td>";
        dongMoi += "<td><button class='btn btn-danger btn-icon' type='button' onclick='XoaDong(this, \"" + TBL_TIEN_LUY_KE + "\")'>" +
            "<span class='fa fa-minus fa-lg' aria-hidden='true'></span>" +
            "</button></td>";
        dongMoi += "</tr>";

        $("#" + TBL_TIEN_LUY_KE + " tbody").append(dongMoi);
        CapNhatCotStt(TBL_TIEN_LUY_KE);
        // tinh dong tong
        TinhLaiDongTong(TBL_TIEN_LUY_KE);

        // xoa text data them moi
        $("#iID_NhaThauID").prop("selectedIndex", 0);
        $("#txtGiaTriUng").val("");
        SuKienDbClickDongTable();
        //EnableButtonLuu();

        arrNhaThau.push({
            iID_NganhID: iIDNganh,
            iID_NguonVonId: iIDNguonVon,
            iID_LoaiNguonVonID: iIDLoaiNganSach,
            iID_NhaThauID: iIDNhaThau,
            sTenNhaThau: sTenNhaThau,
            fGiaTriUng: UnFormatNumber(fGiaTriUng)
        })
    }
}

function CapNhatTienLuyKe() {
    var tongGiaTriUng = $("#" + TBL_TIEN_LUY_KE + " .tong_giatriung").html();
    $("#txtSoDuUng").val(tongGiaTriUng).trigger("change");
}

function CapNhatCotStt(idBang) {
    $("#" + idBang + " tbody tr").each(function (index, tr) {
        $(tr).find('.r_STT').text(index + 1);
    });
}

function SuKienDbClickDongTable() {
    $("#" + TBL_TIEN_LUY_KE + " td").dblclick(function () {
        var $this = $(this);
        var row = $this.closest("tr");
        var riIDNhaThau = row.find('.r_iIDNhaThau').val();
        var rGiaTriUng = row.find('.r_fGiaTriUng ').text();

        // fill len o chinh sua
        $("#iID_NhaThauID").val(riIDNhaThau);
        $("#txtGiaTriUng").val(rGiaTriUng);
    });

    $("#ViewTable td").dblclick(function () {
        var $this = $(this);
        var row = $this.closest("tr");
        var index = row.data("iid");

        // fill len o chinh sua
        var data = $.map(tblDataGrid, function (n) { return n.iId == index ? n : null })[0];
        $("#drpNguonNganSach").val(data.iID_NguonVonID);
        $("#txtKHVonBoTriHetNamTruoc").val(FormatNumber(data.fKHVonHetNamTruoc)).trigger("change");
        $("#txtLuyKeThanhToan").val(FormatNumber(data.fLuyKeThanhToanKLHT)).trigger("change");
        $("#txtSoDuUng").val(FormatNumber(data.fLuyKeThanhToanTamUng)).trigger("change");
        $("#txtVonThanhToanKLHT").val(FormatNumber(data.fThanhToanQuaKB));
        $("#txtTUTheoCheDoChuaThuHoi").val(FormatNumber(data.fTamUngQuaKB));

        $("#drpLoaiNganSach").val(data.iID_LoaiNguonVonIDValue.toLowerCase()).trigger("change");
        setTimeout(function () { $("#drpNganh").val(data.iID_NganhID); }, 500);

        $("#txtGiaTriDauTu").val(FormatNumber(data.fTongMucDauTu));
        $("#txtGiaTriDuToan").val(FormatNumber(data.fGiaTriDuToan));

        $("#txtSoChuyenChiTieuDaCap").val(FormatNumber(data.fSoChuyenChiTieuDaCap));
        $("#txtSoChuyenChiTieuChuaCap").val(FormatNumber(data.fSoChuyenChiTieuChuaCap));
    });

    $("#ViewTable td").click(function (event) {
        event.stopPropagation();
        var $target = $(event.target);
        if ($target.closest("td").attr("colspan") > 1) {
            $target.slideUp();
            $target.closest("tr").prev().find("td:eq(1)").html("+");
        } else {
            $target.closest("tr").next().slideToggle();
            if ($target.closest("tr").find("td:eq(1)").html() == "+")
                $target.closest("tr").find("td:eq(1)").html("-");
            else
                $target.closest("tr").find("td:eq(1)").html("+");
        }
    });
}

function TinhLaiDongTong(idBang) {
    var tongGiaTriUng = 0;
    $("#" + idBang + " .r_fGiaTriUng").each(function () {
        tongGiaTriUng += parseInt(UnFormatNumber($(this).html()));
    });
    $("#" + idBang + " .tong_giatriung").html(FormatNumber(tongGiaTriUng));
}

function XoaDong(nutXoa, idBang) {
    var dongXoa = nutXoa.parentElement.parentElement;
    if (idBang == TBL_TIEN_LUY_KE) {
        var iIDNguonVon = $("#drpNguonNganSach").val();
        var iIDLoaiNganSach = $("#drpLoaiNganSach").val();
        var iIDNganh = $("#drpNganh").val();

        var iIDNhaThau = $(dongXoa).find(".r_iIDNhaThau").val();
        arrNhaThau = arrNhaThau.filter(function (x) { return !(x.iID_NganhID == iIDNganh && x.iID_NguonVonId == iIDNguonVon && x.iID_LoaiNguonVonID == iIDLoaiNganSach && x.iID_NhaThauID == iIDNhaThau) });
    }
    dongXoa.parentNode.removeChild(dongXoa);
    CapNhatCotStt(idBang);
    TinhLaiDongTong(idBang);
}

function ValidateThemChiTiet() {
    var measage = [];
    var iIDNguonVon = $("#drpNguonNganSach").val();
    var iIDLoaiNganSach = $("#drpLoaiNganSach").val();
    var iIDNganh = $("#drpNganh").val();
    var iIDPhanCapPheDuyet = $("#iID_CapPheDuyetID").val();
    var fKHVChuaCapBQP = UnFormatNumber($("#txtKeHoachVonChuaCap").val());
    var iIDDonViQuanLy = $("#iID_DonViQuanLyID").val();

    if ($("#bIsDuAnCu").is(":checked") == false) {
        if ($("#sTenDuAn").val() == "")
            measage.push("Tên dự án chưa có hoặc chưa chính xác.")

        if ($("#sMaDuAn").val() == "")
            measage.push("Mã dự án chưa có hoặc chưa chính xác.")

        if ($("#drpLoaicongTrinh").val == "")
            measage.push("Loại công trình chưa có hoặc chưa chính xác.")

        if (iIDPhanCapPheDuyet == "" || iIDPhanCapPheDuyet == GUID_EMPTY)
            measage.push("Phân câp phê duyệt chưa có hoặc chưa chính xác.")

        if (iIDDonViQuanLy == "" || iIDDonViQuanLy == GUID_EMPTY)
            measage.push("Đơn vị quản lý chưa có hoặc chưa chính xác.")

        if ($("#sSoQDDT").val() == "")
            measage.push("Số QĐĐT chưa có hoặc chưa chính xác.")

        if ($("#dNgayPheDuyetQDDT").val() == "")
            measage.push("Ngày phê duyệt QĐĐT chưa có hoặc chưa chính xác.")

        if ($("#sNguoiKyQDDT").val() == "")
            measage.push("Người ký QĐĐT chưa có hoặc chưa chính xác.")

        if ($("#sCoQuanPheDuyetQDDT").val() == "")
            measage.push("Cơ quan phê duyệt QĐĐT chưa có hoặc chưa chính xác.")
    }

    if (iIDNguonVon == "" || iIDNguonVon == GUID_EMPTY)
        measage.push('Thông tin nguồn vốn chưa có hoặc chưa chính xác.');

    if (iIDLoaiNganSach == "" || iIDLoaiNganSach == GUID_EMPTY)
        measage.push('Thông tin loại ngân sách chưa có hoặc chưa chính xác.');

    if (fKHVChuaCapBQP < 0)
        measage.push("Giá trị kế hoạch vốn chưa cấp không được nhỏ hơn 0");

    if ($("#txtGiaTriDauTu").val() == "")
        measage.push('Thông tin giá trị quyết định đầu tư chưa có hoặc chưa chính xác.');

    if (iIDNganh == "" || iIDNganh == null || iIDNganh == GUID_EMPTY)
        measage.push('Thông tin nghành chưa có hoặc chưa chính xác.');

    if ($("#txtKHVonBoTriHetNamTruoc").val() == "")
        measage.push('Thông tin kế hoạch bố trí hết năm trước chưa có hoặc chưa chính xác.');

    if ($("#txtLuyKeThanhToan").val() == "")
        measage.push('Thông tin lũy kế thanh toán khối lượng hoàn thành chưa có hoặc chưa chính xác.');

    if ($("#txtSoDuUng").val() == "")
        measage.push('Thông tin lũy kế thanh toán tạm ứng chưa có hoặc chưa chính xác.');

    if ($("#txtKeHoachVonChuaCap").val() == "")
        measage.push("Kế hoạch vốn chưa cấp (qua BQP) chưa có hoặc chưa chính xác.");

    if (measage.length > 0) {
        alert(measage.join("\n"));
        return false;
    }
    else return true;
}

function ValidateLuuKhoiTao() {
    var measage = [];
    var iIDPhanCapPheDuyet = $("#iID_CapPheDuyetID").val();
    var iIDDonViQuanLy = $("#iID_DonViQuanLyID").val();

    //Năm khởi tạo
    if ($("#txtiNamKhoiTao").val() == "") {
        measage.push('Năm khởi tạo chưa có hoặc chưa chính xác.');
    }
    //Thông tin dự án
    if ($("#bIsDuAnCu").is(":checked") == false) {
        if ($("#sTenDuAn").val() == "") {
            measage.push('Tên dự án chưa có hoặc chưa chính xác.');
        }
        if ($("#sMaDuAn").val() == "") {
            measage.push('Mã dự án chưa có hoặc chưa chính xác.');
        }
        if (iIDPhanCapPheDuyet == "" || iIDPhanCapPheDuyet == GUID_EMPTY) {
            measage.push('Phân cấp phê duyệt chưa có hoặc chưa chính xác.');
        }
        if ($("#drpLoaicongTrinh").val == "") {
            measage.push('Loại công trình chưa có hoặc chưa chính xác.');
        }
        if (iIDDonViQuanLy == "" || iIDDonViQuanLy == GUID_EMPTY) {
            measage.push('Đơn vị quản lý chưa có hoặc chưa chính xác.');
        }
        if ($("#sSoQDDT").val() == "")
            meg.push("Số QĐĐT chưa có hoặc chưa chính xác.")

        if ($("#dNgayPheDuyetQDDT").val() == "")
            meg.push("Ngày phê duyệt QĐĐT chưa có hoặc chưa chính xác.")

        if ($("#sNguoiKyQDDT").val() == "")
            meg.push("Người ký QĐĐT chưa có hoặc chưa chính xác.")

        if ($("#sCoQuanPheDuyetQDDT").val() == "")
            meg.push("Cơ quan phê duyệt QĐĐT chưa có hoặc chưa chính xác.")
    }

    if (tblDataGrid.length <= 0) {
        measage.push('Thông tin chi tiết chưa có hoặc chưa chính xác.');
    }

    if (measage.length > 0) {
        alert(measage.join("\n"));
        return false;
    }
    else return true;
}

function DinhDangSo(id) {
    if (typeof (id) == 'undefined') {
        $(".sotien").each(function () {
            if ($(this).is('input'))
                $(this).val(FormatNumber($(this).val().trim()) == "" ? 0 : FormatNumber($(this).val().trim()));
            else
                $(this).html(FormatNumber($(this).html().trim()) == "" ? 0 : FormatNumber($(this).html().trim()));
        })
    } else {
        $("#" + id + " .sotien").each(function () {
            if ($(this).is('input'))
                $(this).val(FormatNumber($(this).val().trim()) == "" ? 0 : FormatNumber($(this).val().trim()));
            else
                $(this).html(FormatNumber($(this).html().trim()) == "" ? 0 : FormatNumber($(this).html().trim()));
        })
    }
}

function DeleteItem(index) {
    tblDataGrid = $.map(tblDataGrid, function (n) { return n.iId != index ? n : null });
    RenderGridView(tblDataGrid);
    SuKienDbClickDongTable();
}

function addTableNhaThau() {
    $(".table-parent tbody tr").each(function () {
        var row = this.closest("tr");
        var index = row.dataset["iid"];
        var data = $.map(tblDataGrid, function (n) { return n.iId == index ? n : null })[0];

        var nhaThau = arrNhaThau.filter(function (x) { return x.iID_NganhID == data.iID_NganhID && x.iID_NguonVonId == data.iID_NguonVonID && x.iID_LoaiNguonVonID == data.iID_LoaiNguonVonID });
        var html = "";
        nhaThau.forEach(function (obj, index) {
            var dongMoi = "";
            dongMoi += "<tr style='cursor: pointer;'>";
            dongMoi += "<td class='r_STT'>" + (index + 1) + "</td>";
            dongMoi += "<td align='left' class='r_sTenNhaThau'><input type='hidden' class='r_iIDNhaThau' value='" + obj.iID_NhaThauID + "'/>" + obj.sTenNhaThau + "</td>";
            dongMoi += "<td align='right' class='r_fGiaTriUng'>" + (obj.fGiaTriUng == "" ? 0 : FormatNumber(obj.fGiaTriUng)) + "</td>";
            dongMoi += "</tr>";
            html += dongMoi;
        })

        var tblNhaThau = "<table class='table table-bordered'>" +
            "<thead>" +
            "<th width='5%'></th>" +
            "<th width='50%'>Tên nhà thầu</th>" +
            "<th width='40%'>Giá trị ứng</th>" +
            "</thead>" +
            "<tbody>" + html +
            "</tbody> " +
            "</table> ";

        var newTr = "<tr class='trChild' hidden><td colspan='3'></td><td colspan='7'>" + tblNhaThau + "</td></tr>";
        $(this).find("td:eq(1)").html("+");
        $(this).after(newTr);
    })
}