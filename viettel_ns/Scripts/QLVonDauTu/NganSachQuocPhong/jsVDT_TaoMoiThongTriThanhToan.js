var TBL_CAP_THANH_TOAN_KPQP = "tbl_capthanhtoankpqp";
var TBL_TAM_UNG_KPQP = "tbl_tamungkpqp";
var TBL_THU_UNG_KPQP = "tbl_thuungkpqp";
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';

var lstThanhToan = [];
var lstThuHoi = [];
var lstTamUng = [];
var lstKinhPhi = [];
var lstHopThuc = [];

$(document).ready(function () {
    //DinhDangSo();
    CallFunctionCommon();    
    $("#sMaThongTri").keyup(function (event) {
        ValidateMaxLength(this,50);
    })
    $("#sMoTa").keyup(function (event) {
        ValidateMaxLength(this,500);
    })
});

$("#iID_DonViQuanLyID, #sMaNguonVon, #sNamThongTri").change(function () {
    LayNgayLapGanNhat();
})

function LayNgayLapGanNhat() {
    var sMaDonVi = $("#iID_DonViQuanLyID").val();
    var iNguonVon = $("#sMaNguonVon").val();
    var iNamThongTri = $("#sNamThongTri").val();
    if (sMaDonVi != "" && sMaDonVi != GUID_EMPTY && iNguonVon != "" && iNguonVon != 0 && iNamThongTri != "") {
        $.ajax({
            url: "/QLVonDauTu/QLThongTriThanhToan/LayNgayLapGanNhat",
            type: "POST",
            data: { iIDDonViID: sMaDonVi, iNamThongTri: iNamThongTri, iNguonVon: iNguonVon },
            dataType: "json",
            cache: false,
            success: function (data) {
                $("#sNgayLapGanNhat").val(data);
            },
            error: function (data) {

            }
        })
    } else {
        $("#sNgayLapGanNhat").val("");
    }
}

function Loc() {
    lstThanhToan = [];
    lstThuHoi = [];
    lstTamUng = [];
    lstKinhPhi = [];
    lstHopThuc = [];

    $("#tblThanhToan tbody").html('');
    $("#tblThuHoi tbody").html('');
    $("#tblTamUng tbody").html('');
    $("#tblKinhPhi tbody").html('');
    $("#tblHopThuc tbody").html('');
    $("#thanhtoan").removeClass("active");
    $("#thuhoi").removeClass("active");
    $("#tamung").removeClass("active");
    $("#kinhphi").removeClass("active");
    $("#hopthuc").removeClass("active");
    $("#tabThanhToan").addClass("hidden");
    $("#tabThanhToan").addClass("hidden");
    $("#tabThanhToan").addClass("hidden");
    $("#tabThanhToan").addClass("hidden");
    $("#tabHopThuc").addClass("hidden");

    var iIDMaDonVi = $("#iID_DonViQuanLyID").val();
    var iLoaiThongTri = $("#iLoaiThongTri").val();
    var iNamThongTri = $("#sNamThongTri").val();
    var dNgayTaoThongTri = $("#dNgayThongTri").val();
    var iNguonVon = $("#sMaNguonVon").val();
    //var dNgayLapGanNhat = $("#sNgayLapGanNhat").val();
    //var iNamThongTri = $("#sNamThongTri").val();

    if (iIDMaDonVi == "" || iIDMaDonVi == GUID_EMPTY) {
        alert("Thông tin đơn vị chưa có hoặc chưa chính xác");
        return;
    }

    if (iNamThongTri == "") {
        alert("Chưa nhập năm thực hiện");
        return;
    }

    if (dNgayTaoThongTri == "") {
        alert("Chưa nhập năm thực hiện");
        return;
    }

    //if (iIDMaDonVi == "" && iIDMaDonVi == GUID_EMPTY && iNguonVon == "" && iNguonVon == 0 && dNgayTaoThongTri == "" && iNamThongTri == "")
    //    alert("Chưa nhập thông tin");
    //else { alert("Chưa nhập thông tin"); }

    $.ajax({
        url: "/QLVonDauTu/QLThongTriThanhToan/Loc",
        type: "POST",
        data: {
            sMaDonVi: iIDMaDonVi,
            iLoaiThongTri: iLoaiThongTri,
            iNamKeHoach: iNamThongTri,
            dNgayThongTri: dNgayTaoThongTri,
            sMaNguonVon: iNguonVon
        },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null && data != "") {
                var bIsActive = false;


                if (data.lstThanhToan.length > 0) {
                    lstThanhToan = data.lstThanhToan;
                    var htmlThanhToan = "";
                    data.lstThanhToan.forEach(function (x) {
                        htmlThanhToan += "<tr>";
                        htmlThanhToan += " <td align=\"center\">" + x.SM + "</td>";
                        htmlThanhToan += " <td align=\"center\">" + x.STm + "</td>";
                        htmlThanhToan += " <td align=\"center\">" + x.STtm + "</td>";
                        htmlThanhToan += " <td align=\"center\">" + x.SNg + "</td>";
                        htmlThanhToan += " <td align=\"left\">" + x.STenLoaiCongTrinh + "</td>";
                        htmlThanhToan += " <td align=\"left\">" + x.STenDuAn + "</td>";
                        htmlThanhToan += " <td><input style=' text-align:right' type='text' value=" + x.FSoTien + " id='" + x.id + "' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' class='form-control sotien'/></td>";
                        htmlThanhToan += "</tr>";
                    });
                    $("#tblThanhToan tbody").html(htmlThanhToan);
                    $("#thanhtoan").addClass("active");
                    $("#tabThanhToan").removeClass("hidden");
                    $("#tabThanhToan").addClass("active");
                    bIsActive = true;
                }
                //else {
                //    $("#tblThanhToan").html('');
                //}
                if (data.lstThuHoi.length > 0) {
                    lstThuHoi = data.lstThuHoi;
                    var htmlThuHoi = "";
                    data.htmlThuHoi.forEach(function (x) {
                        htmlThuHoi += "<tr>";
                        htmlThuHoi += " <td align=\"center\">" + x.SM + "</td>";
                        htmlThuHoi += " <td align=\"center\">" + x.STm + "</td>";
                        htmlThuHoi += " <td align=\"center\">" + x.STtm + "</td>";
                        htmlThuHoi += " <td align=\"center\">" + x.SNg + "</td>";
                        htmlThuHoi += " <td align=\"left\">" + x.STenLoaiCongTrinh + "</td>";
                        htmlThuHoi += " <td align=\"left\">" + x.STenDuAn + "</td>";
                        htmlThuHoi += " <td ><input style=' text-align:right' type='text' value=" + x.FSoTien + " id='" + x.id + "' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' class='form-control sotien'/></td>";
                        htmlThuHoi += "</tr>";
                    });
                    $("#tblThuHoi tbody").html(htmlThuHoi);
                    if (bIsActive == false) {
                        $("#thuhoi").addClass("active");
                    }
                    bIsActive = true;
                    $("#tabThuHoi").removeClass("hidden");
                }
                //else {
                //    $("#tblThuHoi").html('');
                //}
                if (data.lstTamUng.length > 0) {
                    lstTamUng = data.lstTamUng;
                    var htmlTamUng = "";
                    data.lstTamUng.forEach(function (x) {
                        htmlTamUng += "<tr>";
                        htmlTamUng += " <td align=\"center\">" + x.SM + "</td>";
                        htmlTamUng += " <td align=\"center\">" + x.STm + "</td>";
                        htmlTamUng += " <td align=\"center\">" + x.STtm + "</td>";
                        htmlTamUng += " <td align=\"center\">" + x.SNg + "</td>";
                        htmlTamUng += " <td align=\"left\">" + x.STenLoaiCongTrinh + "</td>";
                        htmlTamUng += " <td align=\"left\">" + x.STenDuAn + "</td>";
                        htmlTamUng += " <td><input style=' text-align:right' type='text' value=" + x.FSoTien + " id='" + x.id + "' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' class='form-control sotien'/></td>";
                        htmlTamUng += "</tr>";

                    });
                    $("#tblTamUng tbody").html(htmlTamUng);
                    if (bIsActive == false) {
                        $("#tabTamUng").addClass("active");
                        $("#tamung").addClass("active");
                    }
                    bIsActive = true;
                    $("#tabTamUng").removeClass("hidden");
                }
                //else {
                //    $("#tblThanhToan").html('');
                //}
                if (data.lstKinhPhi.length > 0) {
                    lstKinhPhi = data.lstKinhPhi;
                    var htmlKinhPhi = "";
                    data.lstKinhPhi.forEach(function (x) {
                        htmlKinhPhi += "<tr>";
                        htmlKinhPhi += " <td align=\"center\">" + x.SM + "</td>";
                        htmlKinhPhi += " <td align=\"center\">" + x.STm + "</td>";
                        htmlKinhPhi += " <td align=\"center\">" + x.STtm + "</td>";
                        htmlKinhPhi += " <td align=\"center\">" + x.SNg + "</td>";
                        htmlKinhPhi += " <td align=\"left\">" + x.STenLoaiCongTrinh + "</td>";
                        htmlKinhPhi += " <td align=\"left\">" + x.STenDuAn + "</td>";
                        htmlKinhPhi += " <td ><input style=' text-align:right' type='text' value=" + x.FSoTien + " id='" + x.id + "' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' class='form-control sotien'/></td>";
                        htmlKinhPhi += "</tr>";

                    });
                    $("#tblKinhPhi tbody").html(htmlKinhPhi);
                    if (bIsActive == false) {
                        $("#kinhphi").addClass("active");
                        $("#tabKinhPhi").addClass("active");
                        $("#tabTamUng").addClass("hidden");
                        $("#tabHopThuc").addClass("hidden");
                        $("#tabThuHoi").addClass("hidden");
                    }
                    bIsActive = true;
                    $("#tabKinhPhi").removeClass("hidden");
                }
                //else {
                //    $("#tblThanhToan").html('');
                //}
                if (data.lstHopThuc.length > 0) {
                    lstHopThuc = data.lstHopThuc;
                    var htmlHopThuc = "";
                    data.lstHopThuc.forEach(function (x) {
                        htmlHopThuc += "<tr>";
                        htmlHopThuc += " <td align=\"center\">" + x.SM + "</td>";
                        htmlHopThuc += " <td align=\"center\">" + x.STm + "</td>";
                        htmlHopThuc += " <td align=\"center\">" + x.STtm + "</td>";
                        htmlHopThuc += " <td align=\"center\">" + x.SNg + "</td>";
                        htmlHopThuc += " <td align=\"left\">" + x.STenLoaiCongTrinh + "</td>";
                        htmlHopThuc += " <td align=\"left\">" + x.STenDuAn + "</td>";
                        htmlHopThuc += " <td><input style=' text-align:right' type='text' value=" + x.FSoTien + " id='" + x.id + "' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' class='form-control sotien'/></td>";
                        htmlHopThuc += "</tr>";
                    });
                    $("#tblHopThuc tbody").html(htmlHopThuc);
                    if (bIsActive == false) {
                        $("#hopthuc").addClass("active");
                        $("#tabHopThuc").addClass("active");
                        $("#tabTamUng").addClass("hidden");
                        $("#tabThuHoi").addClass("hidden");
                        $("#tabKinhPhi").addClass("hidden");
                    }
                    $("#tabHopThuc").removeClass("hidden");
                    bIsActive = true;
                }
                //else {
                //    $("#tblThanhToan").html('');
                //}
                //triggerCurrently();
                DinhDangSo();
            }
        }
        //$("#tblDanhSachPhuLucHangMuc tbody").html('');
        , error: function (data) {
            console.log(data);
        }
    });
}

function LayThongTinDeNghiThanhToan(iIDMaDonVi, iNguonVon, dNgayLapGanNhat, dNgayTaoThongTri, iNamThongTri, callback) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLThongTriThanhToan/GetDeNghiThanhToanChiTiet",
        data: { iID_MaDonVi: iIDMaDonVi, iNguonVon: iNguonVon, dNgayLapGanNhat: dNgayLapGanNhat, dNgayTaoThongTri: dNgayTaoThongTri, iNamThongTri: iNamThongTri },
        success: function (data) {
            var button = { bUpdate: 0, bDelete: 0, bInfo: 0 };
            if (data != null && data.data != null) {
                var columnsTab1 = [
                    { sField: "iID_DeNghiThanhToan_ChiTietID", bKey: true },
                    { sField: "iID_Parent", bParentKey: true },

                    { sTitle: "Mục", sField: "sM", iWidth: "7%", sTextAlign: "center" },
                    { sTitle: "Tiểu mục", sField: "sTM", iWidth: "7%", sTextAlign: "center" },
                    { sTitle: "Tiết mục", sField: "sTTM", iWidth: "7%", sTextAlign: "center" },
                    { sTitle: "Ngành", sField: "sNG", iWidth: "7%", sTextAlign: "center" },
                    { sTitle: "Nội dung", sField: "sNoiDung", iWidth: "55%", sTextAlign: "left", bHaveIcon: 1 },
                    { sTitle: "Số tiền", sField: "fGiaTriThanhToan", iWidth: "11%", sTextAlign: "right", sClass: "sotien" }];
                $("#" + TBL_CAP_THANH_TOAN_KPQP).html(GenerateTreeTable(data.data, columnsTab1, button, TBL_CAP_THANH_TOAN_KPQP));

                var columnsTab2 = [
                    { sField: "iID_DeNghiThanhToan_ChiTietID", bKey: true },
                    { sField: "iID_Parent", bParentKey: true },

                    { sTitle: "Nội dung", sField: "sNoiDung", iWidth: "55%", sTextAlign: "left", bHaveIcon: 1 },
                    { sTitle: "Số tiền", sField: "fGiaTriTamUng", iWidth: "11%", sTextAlign: "right", sClass: "sotien" }];
                $("#" + TBL_TAM_UNG_KPQP).html(GenerateTreeTable(data.data, columnsTab2, button, TBL_TAM_UNG_KPQP));

                var columnsTab3 = [
                    { sField: "iID_DeNghiThanhToan_ChiTietID", bKey: true },
                    { sField: "iID_Parent", bParentKey: true },

                    { sTitle: "Nội dung", sField: "sNoiDung", iWidth: "55%", sTextAlign: "left", bHaveIcon: 1 },
                    { sTitle: "Số tiền", sField: "fGiaTriThuHoi", iWidth: "11%", sTextAlign: "right", sClass: "sotien" }];
                $("#" + TBL_THU_UNG_KPQP).html(GenerateTreeTable(data.data, columnsTab3, button, TBL_THU_UNG_KPQP));
            } else {
                $("#" + TBL_CAP_THANH_TOAN_KPQP).html("");
                $("#" + TBL_TAM_UNG_KPQP).html("");
                $("#" + TBL_THU_UNG_KPQP).html("");
            }
            console.log(data);
            if (callback)
                callback();
        }
    });
}

function DinhDangSo() {
    $(".sotien").each(function () {
        $(this).val(FormatNumber($(this).val().trim()) == "" ? 0 : FormatNumber($(this).val().trim()));
    })
}

function XoaTextThongTri() {
    $("#iID_DonViQuanLyID").prop("selectedIndex", 0).trigger("change");
    $("#sMaThongTri").val("");
    $("#sNgayLapGanNhat").val("");
    $("#dNgayThongTri").val("");
    $("#sNamThongTri").val("");
    $("#sNguoiLap").val("");
    $("#sTruongPhong").val("");
    $("#sThuTruongDonVi").val("");
}

function Huy() {
    window.location.href = "/QLVonDauTu/QLThongTriThanhToan/Index";
}

function Luu() {
    var thongTri = {};
    thongTri.bThanhToan = true;
    thongTri.iID_DonViID = $("#iID_DonViQuanLyID").val();
    thongTri.iNamThongTri = $("#sNamThongTri").val();
    thongTri.sMaNguonVon = $("#sMaNguonVon").val();
    thongTri.iLoaiThongTri = $("#iLoaiThongTri").val();
    thongTri.sMaThongTri = $("#sMaThongTri").val();
    thongTri.dNgayThongTri = $("#dNgayThongTri").val();
    thongTri.sMoTa = $("#sMoTa").val();
    thongTri.iNamNganSach = $("#iNamNganSach").val();

    if (CheckLoi(thongTri)) {
        //var doiTuong = { thongTri: thongTri };
        $.ajax({
            url: "/QLVonDauTu/QLThongTriThanhToan/Luu",
            type: "POST",
            data: { model: thongTri, bReloadChiTiet: 0 },
            dataType: "json",
            cache: false,
            async: false,
            success: function (data) {
                if (lstThanhToan.length > 0 || lstThuHoi.length > 0 || lstTamUng.length > 0 || lstKinhPhi.length > 0 || lstHopThuc.length > 0) {
                    if (data != null && data != "") {
                        $.ajax({
                            url: "/QLVonDauTu/QLThongTriThanhToan/SaveThongTriChiTiet",
                            type: "POST",
                            data: {
                                thongTriId: data,
                                lstThanhToan: lstThanhToan,
                                lstThuHoi: lstThuHoi,
                                lstTamUng: lstTamUng,
                                lstKinhPhi: GetValueDataInGrid(lstKinhPhi),
                                lstHopThuc: GetValueDataInGrid(lstHopThuc)
                            },
                            dataType: "json",
                            cache: false,
                            async: false,
                            success: function (data) {
                                console.log(data);
                                if (data != null && data == true) {
                                    window.location.href = "/QLVonDauTu/QLThongTriThanhToan/Index";
                                }
                            },
                            error: function (data) {
                                console.log(data);
                            }
                        })
                    }
                } else {
                    window.location.href = "/QLVonDauTu/QLThongTriThanhToan/Index";
                }
            },
            error: function (data) {

            }
        })
    }
}

function GetValueDataInGrid(lstData) {
    if (lstData == null || lstData.length == 0) return [];
    var results = [];
    $.each(lstData, function (index, item) {
        var obj = item;
        obj.FSoTien = UnFormatNumber($("#" + item.id).val());
        results.push(obj);
    });
    return results;
}

function CheckLoi(doiTuong) {
    var messErr = [];
    if (doiTuong.iID_DonViID == "" || doiTuong.iID_DonViID == GUID_EMPTY)
        messErr.push("Đơn vị quản lý chưa có hoặc chưa chính xác.");

    if (doiTuong.sMaNguonVon == "" || doiTuong.sMaNguonVon == 0)
        messErr.push("Nguồn vốn chưa có hoặc chưa chính xác.");

    if (doiTuong.sMaThongTri == "")
        messErr.push("Mã thông tri chưa có hoặc chưa chính xác.");

    if (doiTuong.dNgayThongTri == "")
        messErr.push("Ngày tạo thông tri chưa có hoặc chưa chính xác.");

    if (doiTuong.iNamThongTri == "")
        messErr.push("Năm thực hiện chưa có hoặc chưa chính xác.");

    if (doiTuong.sNguoiLap == "")
        messErr.push("Người lập thông tri chưa có hoặc chưa chính xác.");

    if (doiTuong.sThuTruongDonVi == "")
        messErr.push("Thủ trưởng đơn vị chưa có hoặc chưa chính xác.");

    if (KiemTraTrungMaThongTri(doiTuong.sMaThongTri) == true)
        messErr.push("Mã thông tri đã tồn tại, vui lòng nhập mã khác.");

    //TODO save thanh toan chi tiet
    /*
    if ($("#" + TBL_CAP_THANH_TOAN_KPQP + " table tr").length == 0
        && $("#" + TBL_TAM_UNG_KPQP + " table tr").length == 0
        && $("#" + TBL_TAM_UNG_KPQP + " table tr").length == 0
    )
        messErr.push("Không có dữ liệu chi tiết.");
    */
    if (messErr.length > 0) {
        alert(messErr.join("\n"));
        return false;
    } else {
        return true;
    }
}

function KiemTraTrungMaThongTri(sMaThongTri) {
    var check = false;
    $.ajax({
        url: "/QLVonDauTu/QLThongTriThanhToan/KiemTraTrungMaThongTri",
        type: "POST",
        data: { sMaThongTri: sMaThongTri },
        dataType: "json",
        async: false,
        cache: false,
        success: function (data) {
            check = data;
        },
        error: function (data) {

        }
    })
    return check;
}

function triggerCurrently() {
    $(".currently").on("change", function () {
        if ($(this).val() == null) {
            $(this).val(0);
        } else {
            if ($.isNumeric($(this).val())) {
                var sNumber = Number($(this).val()).toLocaleString('vi-VN');
                $(this).val(sNumber);
            }
            else {
                $(this).val(0);
            }
        }
    });
}
