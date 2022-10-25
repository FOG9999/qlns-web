$("#txtNamKeHoach").trigger("change");
$("#drpLoaiNganSach").trigger("change");
$("#drpLoaiKhoan").trigger("change");

$("#drpMuc").trigger("change");
$("#drpTieuMuc").trigger("change");
$("#drpTietMuc").trigger("change");
$("#txtChiTieuDuocDuyet").trigger("change");

$("#drpDonViQuanLy").trigger("change");
$("#drpNguonNganSach").trigger("change");
$("#txtNamKeHoach").trigger("change");

$("#drpDuAn").trigger("change");

var dataExclude = [1, 2, 3];
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;

//============================== Event Button ============================//

function FindDuplicateKeHoachVonNam() {
    if ($("#drpDonViQuanLy").val() != null && $("#drpNguonNganSach").val() != null && $("#txtNamKeHoach").val() != null) {
        $.ajax({
            type: "POST",
            url: "/QLKeHoachVonNam/GetPhanBoVonDuplicate",
            data: {
                iDonViQuanLy: $("#drpDonViQuanLy").val(),
                iNguonVonId: $("#drpNguonNganSach").val(),
                iNamKeHoach: $("#txtNamKeHoach").val()
            },
            success: function (r) {
                if (r.bIsComplete) {
                    if (r.data != null) {
                        $("#txtSoKeHoach").val(r.data.sSoQuyetDinh);
                        $("#txtNgayLap").val(ConvertDatetimeToJSon(r.data.dNgayQuyetDinh));
                        $("#drpLoaiNganSach").val();
                    }   
                }
            }
        });
    }
}

function EditTaiLieu_On(){
    $(".show-edit-attach-on").css("display", "");
    $(".show-edit-attach-off").css("display", "none");
}

function CancelTaiLieu() {
    $(".show-edit-attach-on").css("display", "none");
    $(".show-edit-attach-off").css("display", "");
}

function CancelSaveData() {
    location.href = "/QLVonDauTu/QLKeHoachVonNam/Index";
}

function BtnInsertDataClick() {
    location.href = "/QLVonDauTu/QLKeHoachVonNam/Insert";
}

function ChinhSuaDataList(id) {
    location.href = "/QLVonDauTu/QLKeHoachVonNam/ChinhSua/"+ id;
}

function DeleteItemList(id) {
    if (!confirm("Chấp nhận xóa bản ghi ?")) return;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKeHoachVonNam/DeleteVdtKhvPhanBoVon",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                ChangePage();
            }
        }
    });
}

function btnLaySoLieu_Click() {
    var iNamKeHoach = $("#txtNamKeHoach").val();
    var iDonViQuanLy = $("#drpDonViQuanLy").val();
    var iNguonVon = $("#drpNguonNganSach").val();
    var iLoaiNganSach = $("#drpLoaiNganSach").val().split("|")[1];
    var dNgayLap = $("#txtNgayLap").val();
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKeHoachVonNam/BtnLayDuLieu_Click",
        data: {
            iNamKeHoach: iNamKeHoach,
            iDonViQuanLy: iDonViQuanLy,
            iNguonVon: iNguonVon,
            iLoaiNganSach: iLoaiNganSach,
            dNgayLap: dNgayLap
        },
        success: function (r) {
            if (r.bIsComplete) {
                tblDataGrid.push(r.data)
                FillDataToGridViewInsert(r.data);
            }
        }
    });
}

function ClearDataInsert() {
    $("#drpCapPheDuyet").prop("selectedIndex", -1);
    //trLoaiCongTrinh.clearSelection();
    $("#drpDuAn").prop("selectedIndex", -1);
    $("#txtNamThucHienFrom").val("");
    $("#txtNamThucHienTo").val("");
    $("#txtMaKetNoi").val("");
    $("#txtGiaTriDauTu").val("");
    $("#txtVonDaBoTri").val("");
    $("#txtVonConLai").val("");
    $("#txtKeHoachUng").val("");
    $("#txtGiaTriUng").val("");
    $("#txtGiaTriThuHoi").val("");
    $("#txtVonNamTruocChuyenSang").val("");
    $("#txtChiTieuDuocDuyet").val("");
    $("#txtGhiChu").val("");
}

function InsertDetailData(sTrangThaiDuAnDangKy) {
    var sMessError = [];
    if ($("#drpNganh").val() == null) {
        sMessError.push("Chưa nhập Mục - Tiểu mục - Tiết mục - Ngành !");
    }
    if ($("#drpLoaicongTrinh").val() == null) {
        sMessError.push("Chưa nhập loại công trình !");
    }
    if ($("#drpDuAn").val() == null) {
        sMessError.push("Chưa nhập dự án !");
    }
    if ($("#txtChiTieuDuocDuyet").val() == null) {
        sMessError.push("Chưa nhập chỉ tiêu ngân sách được duyệt !");
    }
    if (sMessError.length != 0) {
        alert(sMessError.join("\n"));
        return false;
    }
    iIdMax++;
    var data = {};
    data.iParentId = $("#txtParentId").val();
    data.sXauNoiMa = $("#drpNganh option:selected").text();
    data.sMaKetNoi = $("#txtMaKetNoi").val();
    data.sTenDuAn = $("#drpDuAn option:selected").text();
    data.iID_DuAnID = $("#drpDuAn option:selected").val().split('|')[0];
    data.iID_NganhID = $("#drpNganh option:selected").val();
    data.iId_LoaiCongTrinh = $("#drpLoaicongTrinh").val();
    data.iId_CapPheDuyet = $("#drpCapPheDuyet").val();
    data.fGiaTriDauTu = $("#txtGiaTriDauTu").val();
    data.fVonDaBoTri = $("#txtVonDaBoTri").val().replace(".", "");
    data.fChiTieuNamTruocChuaCap = "";
    data.fThuHoiUngXDCBKhac = "";
    data.ChiTieuNganSachNam = "";
    data.dateFrom = $("#txtNamThucHienFrom").val();
    data.dateTo = $("#txtNamThucHienTo").val();
    data.sGhiChu = $("#txtGhiChu").val();
    data.fVonConLai = $("#txtVonConLai").val();
    data.sTrangThaiDuAnDangKy = sTrangThaiDuAnDangKy;
    var idCheck = $("#txtId").val();

    //data.fChiTieuNganSachDuocDuyet = $("#txtChiTieuDuocDuyet").val().replaceAll(".", "").toLocaleString('vi-VN');
    data.fChiTieuNganSachDuocDuyet = $("#txtChiTieuDuocDuyet").val();
    //tblDataGrid = $.map(tblDataGrid, function (n) { return n.iID_DuAnID != $("#drpDuAn option:selected").val().split('|')[0] ? n : null });
    tblDataGrid = $.map(tblDataGrid, function (n) { return n.iID_DuAnID != data.iID_DuAnID || (n.iID_DuAnID == data.iID_DuAnID && n.iID_NganhID != data.iID_NganhID) ? n : null });

    data.iId = iIdMax;
    tblDataGrid.push(data)
    FillDataToGridViewInsert(tblDataGrid);
    //$("#drpLoaicongTrinh").val(null);
    //$("#txtLoaiCongTrinh").text("");
    $("#txtId").val(null);
    ClearDataInsert();
}

function InsertDieuChinhData() {
    var sMessError = [];
    if ($("#drpNganh").val() == null) {
        sMessError.push("Chưa nhập Mục - Tiểu mục - Tiết mục - Ngành !");
    }
    if ($("#drpLoaicongTrinh").val() == null) {
        sMessError.push("Chưa nhập loại công trình !");
    }
    if ($("#drpDuAn").val() == null) {
        sMessError.push("Chưa nhập dự án !");
    }
    if (sMessError.length != 0) {
        alert(sMessError.join("\n"));
        return false;
    }
    iIdMax++;
    var data = {};
    data.iParentId = 1;
    data.sXauNoiMa = $("#drpNganh option:selected").text();
    data.sMaKetNoi = $("#txtMaKetNoi").val();
    data.sTenDuAn = $("#drpDuAn option:selected").text();
    data.iID_DuAnID = $("#drpDuAn option:selected").val().split('|')[0];
    data.iID_NganhID = $("#drpNganh option:selected").val();
    data.iId_LoaiCongTrinh = $("#drpLoaicongTrinh").val();
    data.iId_CapPheDuyet = $("#drpCapPheDuyet").val();
    data.fGiaTriDauTu = $("#txtGiaTriDauTu").val();
    data.fVonDaBoTri = $("#txtVonDaBoTri").val().replace(".", "");
    data.fChiTieuNamTruocChuaCap = "";
    data.fThuHoiUngXDCBKhac = "";
    data.ChiTieuNganSachNam = "";
    data.fGiaTrPhanBo = $("#txtGTTruocDieuChinh").val();
    data.snameLoaiDieuChinh = $("#sLoaiDieuChinh option:selected").text();
    data.sLoaiDieuChinh = $("#sLoaiDieuChinh option:selected").val();
    data.fGiaTriDieuChinh = $("#txtGiaTriDieuChinh").val();
    data.fGiaTriSauDieuChinh = $("#txtGTSauDieuChinh").val();
    data.sGhiChu = $("#txtGhiChu").val();
    var idCheck = $("#txtId").val();

    data.fChiTieuNganSachDuocDuyet = $("#txtChiTieuDuocDuyet").val();
    tblDataGrid = $.map(tblDataGrid, function (n) { return n.iId != $("#txtId").val() ? n : null });

    data.iId = iIdMax;
    tblDataGrid.push(data)
    FillDataToGridViewChinhSua(tblDataGrid);
    $("#drpLoaicongTrinh").val(null);
    $("#txtLoaiCongTrinh").text("");
    $("#txtId").val(null);
    $("#txtChiTieuDauNam").val(null);
    $("#txtGTTruocDieuChinh").val(null);
    $("#txtGTSauDieuChinh").val(null);
    $("#txtGiaTriDieuChinh").val(null);
    ClearDataInsert();
}

function Insert() {
    if (!ValidateDataInsert()) return;
    CreatePhanBoVon(1);
}

/// iViewAdd {1 : Insert; 2 : Update 3 : dieu chinh }
function CreatePhanBoVon(iViewAdd) {
    var data = {};
    data.sSoQuyetDinh = $("#txtSoKeHoach").val();
    data.dNgayQuyetDinh = $("#txtNgayLap").val();
    data.iNamKeHoach = $("#txtNamKeHoach").val();
    data.iID_DonViQuanLyID = $("#drpDonViQuanLy").val();
    data.iID_LoaiNguonVonID = $("#drpLoaiNganSach").val().split("|")[1];
    data.iID_NguonVonID = $("#drpNguonNganSach").val();

    $.ajax({
        type: "POST",
        url: "/QLKeHoachVonNam/checkWhenSaveData",
        data: { iID_PhanBoVonID: GUID_EMPTY, iID_DonViQuanLyID: data.iID_DonViQuanLyID, iID_NguonVonID: data.iID_NguonVonID, iNamKeHoach: data.iNamKeHoach, sSoQuyetDinh: data.sSoQuyetDinh },
        success: function (r) {
            if (r.bIsComplete) {
                $.ajax({
                    type: "POST",
                    url: "/QLKeHoachVonNam/InsertPhanBoVon",
                    data: {
                        data: data
                    },
                    success: function (r) {
                        if (r.bIsComplete) {
                            PhanBoVonChiTiet(r.data);
                        } else {
                            alert("Phân bổ vốn thất bại !");
                        }
                    }
                });
            } else {
                var Title = 'Lỗi lưu kế hoạch vốn năm';
                var messErr = [];
                messErr.push(r.errMes);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
                return;
            }
        }
    });

    //$.ajax({
    //    type: "POST",
    //    url: "/QLKeHoachVonNam/InsertPhanBoVon",
    //    data: {
    //        data: data
    //    },
    //    success: function (r) {
    //        if (r.bIsComplete) {
    //            PhanBoVonChiTiet(r.data);
    //        } else {
    //            alert("Phân bổ vốn thất bại !");
    //        }
    //    }
    //});
}

function PhanBoVonChiTiet(iParentId) {
    var data = [];
    $.each(tblDataGrid, function (index, value) {
        if (dataExclude.indexOf(value.iId) != -1) { return; }
        var item = {};
        item.iID_PhanBoVonID = iParentId;
        item.iID_DuAnID = value.iID_DuAnID;
        item.iID_NganhID = value.iID_NganhID;

        item.fGiaTrPhanBo = value.fChiTieuNganSachDuocDuyet.replaceAll(".", "");

        item.sGhiChu = value.sGhiChu;
        data.push(item);
    });
    $.ajax({
        type: "POST",
        url: "/QLKeHoachVonNam/InsertPhanBoVonChiTiet",
        data: {
            lstData: data
        },
        success: function (r) {
            if (r.bIsComplete) {
                alert("Tạo mới thành công kế hoạch năm !");
                location.href = "/QLVonDauTu/QLKeHoachVonNam/Index";
            }
            else {
                alert("Phân bổ vốn chi tiết thất bại !");
            }
        }
    });
}

function SaveDataDieuChinh() {
    var data = {};
    var lstDetail = [];
    data.iID_PhanBoVonID = $("#iId_PhanBoVonId").val();
    data.sSoQuyetDinh = $("#txtSoKeHoach").val();
    data.dNgayQuyetDinh = $("#txtNgayLap").val();
    data.iNamKeHoach = $("#txtNamKeHoach").val();
    data.iID_DonViQuanLyID = $("#drpDonViQuanLy").val();
    data.iID_LoaiNguonVonID = $("#drpLoaiNganSach").val().split("|")[1];
    data.iID_NguonVonID = $("#drpNguonNganSach").val();

    $.each(tblDataGrid, function (index, value) {
        if (dataExclude.indexOf(value.iId) != -1) { return; }
        var item = {};
        item.iID_DuAnID = value.iID_DuAnID;
        item.iID_NganhID = value.iID_NganhID;
        item.fGiaTrPhanBo = value.fGiaTriSauDieuChinh.replaceAll(".", "");
        item.sTrangThaiDuAnDangKy = 2;
        item.sGhiChu = value.sGhiChu;
        lstDetail.push(item);
    });

    $.ajax({
        type: "POST",
        url: "/QLKeHoachVonNam/checkWhenSaveData",
        data: { iID_PhanBoVonID: data.iID_PhanBoVonID, iID_DonViQuanLyID: data.iID_DonViQuanLyID, iID_NguonVonID: data.iID_NguonVonID, iNamKeHoach: data.iNamKeHoach, sSoQuyetDinh: data.sSoQuyetDinh },
        success: function (r) {
            if (r.bIsComplete) {
                $.ajax({
                    type: "POST",
                    url: "/QLVonDauTu/QLKeHoachVonNam/ChinhSuaPhanBoVon",
                    data: {
                        data: data,
                        lstDetail: lstDetail
                    },
                    success: function (r) {
                        if (r.bIsComplete) {
                            alert("Chỉnh sửa thành công !");
                            location.href = "/QLVonDauTu/QLKeHoachVonNam";
                        } else {
                            alert("Chỉnh sửa thất bại !")
                        }
                    }
                });
            } else {
                var Title = 'Lỗi lưu kế hoạch vốn năm';
                var messErr = [];
                messErr.push(r.errMes);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
                return;
            }
        }
    });

    //$.ajax({
    //    type: "POST",
    //    url: "/QLVonDauTu/QLKeHoachVonNam/ChinhSuaPhanBoVon",
    //    data: {
    //        data: data,
    //        lstDetail: lstDetail
    //    },
    //    success: function (r) {
    //        if (r.bIsComplete) {
    //            alert("Chỉnh sửa thành công !");
    //            location.href = "/QLVonDauTu/QLKeHoachVonNam";
    //        } else {
    //            alert("Chỉnh sửa thất bại !")
    //        }
    //    }
    //});
}

//============================== Event List ================================//
function GetItemDataList(id) {
    window.location.href = "/QLVonDauTu/QLKeHoachVonNam/Update/" + id;
}

function GetListData(sSoKeHoach, iNamKeHoach, dNgayLapFrom, dNgayLapTo, sDonViQuanLy, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/QLKeHoachVonNam/VDTKHVPhanBoVonView",
        data: { sSoKeHoach: sSoKeHoach, iNamKeHoach: iNamKeHoach, dNgayLapFrom: dNgayLapFrom, dNgayLapTo: dNgayLapTo, sDonViQuanLy: sDonViQuanLy, _paging: _paging },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtSoKeHoach").val(sSoKeHoach);
            $("#txtNamKeHoach").val(iNamKeHoach);
            $("#txtNgayLapFrom").val(dNgayLapFrom);
            $("#txtNgayLapTo").val(dNgayLapTo);
            $("#drpDonViQuanLy").val(sDonViQuanLy);

        }
    });
}

function ChangePage(iCurrentPage = 1) {
    var sSoKeHoach = $("#txtSoKeHoach").val();
    var iNamKeHoach = $("#txtNamKeHoach").val();
    var dNgayLapFrom = $("#txtNgayLapFrom").val();
    var dNgayLapTo = $("#txtNgayLapTo").val();
    var sDonViQuanLy = $("#drpDonViQuanLy").val();
    GetListData(sSoKeHoach, iNamKeHoach, dNgayLapFrom, dNgayLapTo, sDonViQuanLy, iCurrentPage);
}

//============================== Event Insert===============================//

function GetThongTinDauTuDuAn(isEditView) {
    var iNamKeHoach = $("#txtNamKeHoach").val();
    var iDonViQuanLy = $("#drpDonViQuanLy").val();
    var iNguonVon = $("#drpNguonNganSach").val();
    var iLoaiNganSach = $("#drpLoaiNganSach").val().split("|")[1];
    var dNgayLap = $("#txtNgayLap").val();
    var iNganh = $("#drpNganh").val();
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKeHoachVonNam/GetThongTinDauTuDuAn",
        data: {
            iId: $("#drpDuAn").val().split("|")[0],
            iNamKeHoach: iNamKeHoach,
            iDonViQuanLy: iDonViQuanLy,
            iNguonVon: iNguonVon,
            iLoaiNganSach: iLoaiNganSach,
            dNgayLap: dNgayLap,
            iNganh: iNganh
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#txtParentId").val(r.data.iParentId);
                $("#txtXauNoiMa").val(r.data.sXauNoiMa);

                $("#txtMaKetNoi").val(r.data.sMaKetNoi);
                $("#txtGiaTriDauTu").val((r.data.fGiaTriDauTu ?? 0).toLocaleString('vi-VN'));
                $("#txtVonDaBoTri").val((r.data.fVonDaBoTri ?? 0).toLocaleString('vi-VN'));
                $("#txtVonConLai").val(Number((r.data.fGiaTriDauTu ?? 0) - (r.data.fVonDaBoTri ?? 0)).toLocaleString('vi-VN'));
                $("#drpCapPheDuyet").val(r.data.iID_CapPheDuyetID);
                $("#txtChiTieuDauNam").val(r.data.fChiTieuDauNam.toLocaleString('vi-VN'));
                $("#txtNamThucHienFrom").val(r.data.sKhoiCong);
                $("#txtNamThucHienTo").val(r.data.sKetThuc);
                
                //if (isEditView) {
                //    $("#drpCapPheDuyet").val($("#data.iId_CapPheDuyet").val());
                //}
            }
        }
    });
}

function GetDataDropdownLoaiAndKhoanByLoaiNganSach() {
    $.ajax({
        type: "POST",
        url: "/QLKeHoachVonNam/GetDataDropdownLoaiAndKhoanByLoaiNganSach",
        data: {
            sLNS: $("#drpLoaiNganSach").val().split("|")[0],
            iNamKeHoach: $("#txtNamKeHoach").val()
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#drpLoaiKhoan").empty()
                $.each(r.data, function (index, value) {
                    $("#drpLoaiKhoan").append("<option value='" + value.Value + "'>" + value.Text + "</option>");
                });
                if (r.data.length > 0) {
                    $("#drpLoaiKhoan").prop("selectedIndex", 0);
                    $("#drpLoaiKhoan").trigger("change");
                }
                //$("#drpLoaiKhoan").prop("selectedIndex", -1);
            }
        }
    });
}

function GetDataDropdownLoaiNganSach(isEdit) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKeHoachVonNam/GetDataDropdownLoaiNganSach",
        data: {
            iNamKeHoach: $("#txtNamKeHoach").val()
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#drpLoaiNganSach").empty()
                $.each(r.data, function (index, value) {
                    $("#drpLoaiNganSach").append("<option value='" + value.Value + "'>" + value.Text + "</option>");
                });
                if (r.data.length > 0) {
                    $("#drpLoaiNganSach").prop("selectedIndex", 0);
                    $("#drpLoaiNganSach").trigger("change");
                }
                //$("#drpLoaiNganSach").prop("selectedIndex", -1);
                if (isEdit) {
                    var dataChoose = $.map(r.data, function (n) { return n.Value.indexOf($("#drpLoaiNganSachOld").val()) != -1 ? n.Value : null })[0];
                    $("#drpLoaiNganSach").val(dataChoose);
                } else {
                    FindDuplicateKeHoachVonNam();
                }
            }
        }
    });
}

function GetDataDropdownNganh(isEdit) {
    $("#drpNganh").empty();
    if ($("#drpLoaiKhoan").val() == undefined) return;
    var sLoai = $("#drpLoaiKhoan").val().split('|')[0];
    var sKhoan = $("#drpLoaiKhoan").val().split('|')[1];
    $.ajax({
        type: "POST",
        url: "/QLKeHoachVonNam/GetDataThongTinChiTietLoaiNganSach",
        data: {
            sLNS: $("#drpLoaiNganSach").val().split("|")[0],
            sLoai: sLoai,
            sKhoan: sKhoan,
            iNamKeHoach: $("#txtNamKeHoach").val()
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#drpNganh").empty();
                $.each(r.data, function (index, value) {
                    $("#drpNganh").append("<option value='" + value.Value + "'>" + value.Text + "</option>");
                });
                if (r.data.length > 0) {
                    $("#drpNganh").prop("selectedIndex", 0);
                }
                //$("#drpNganh").prop("selectedIndex", -1);
                if (isEdit) {
                    $("#drpNganh").val($("#drpNganhOld").val());
                    GetDataDropDownDuAnByLoaiCongTrinh(true);
                }
            }
        }
    });
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
                selectableLastNode: false
            });
            trLoaiCongTrinh.onChange(function () {
                $("#drpLoaicongTrinh").val(trLoaiCongTrinh.getSelectedIds());
                GetDataDropDownDuAnByLoaiCongTrinh();
            });
        },
        error: function (data) {

        }
    });
}

function GetDataDropDownDuAnByLoaiCongTrinh(isEdit) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKeHoachVonNam/GetDataDropDownDuAnByLoaiCongTrinh",
        data: {
            iId: $("#drpLoaicongTrinh").val()
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#drpDuAn").empty();
                $.each(r.data, function (index, value) {
                    $("#drpDuAn").append("<option value='" + value.Value + "'>" + value.Text + "</option>");
                });
                if (r.data.length > 0) {
                    $("#drpDuAn").prop("selectedIndex", 0);
                }
                //$("#drpDuAn").prop("selectedIndex", -1);
                if (isEdit) {
                    var itemChoose = $.map(r.data, function (n) { return n.Value.indexOf($("#drpDuAnOld").val()) != -1 ? n.Value : null })[0];
                    $("#drpDuAn").val(itemChoose);
                    $("#drpDuAn").change();
                }
            }
        }
    });
}

function GetDataDropdownNganhEdit(iId, iNamKeHoach) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/GiaiNganThanhToan/GetDataThongTinChiTietLoaiNganSachByNganh",
        data: {
            iId_Nganh: iId,
            iNamKeHoach: iNamKeHoach
        },
        success: function (r) {
            if (r.bIsComplete) {
                $("#drpNganh").empty();
                $.each(r.data, function (index, value) {
                    $("#drpNganh").append("<option value='" + value.Value + "'>" + value.Text + "</option>");
                });
                $("#drpNganh").prop("selectedIndex", -1);
                $("#drpNganh").val($("#drpNganhOld").val());
                GetDataDropDownDuAnByLoaiCongTrinh(true);
            }
        }
    });
}

//============================== Event Edit =================================//
function SaveThongTinChung() {
    data = {};
    data.iID_PhanBoVonID = $("#iId_PhanBoVonId").val();
    data.sSoQuyetDinh = $("#txtSoKeHoach").val();
    data.dNgayQuyetDinh = $("#txtNgayLap").val();
    data.iNamKeHoach = $("#txtNamKeHoach").val();

    var iID_DonViQuanLyID = $("#drpDonViQuanLy").val();
    var iID_NguonVonID = $("#drpNguonNganSach").val();
    $.ajax({
        type: "POST",
        url: "/QLKeHoachVonNam/checkWhenSaveData",
        data: { iID_PhanBoVonID: data.iID_PhanBoVonID, iID_DonViQuanLyID: iID_DonViQuanLyID, iID_NguonVonID: iID_NguonVonID, iNamKeHoach: data.iNamKeHoach, sSoQuyetDinh: data.sSoQuyetDinh },
        success: function (r) {
            if (r.bIsComplete) {
                $.ajax({
                    type: "POST",
                    url: "/QLVonDauTu/QLKeHoachVonNam/UpdatePhanBoVon",
                    data: {
                        data: data
                    },
                    success: function (r) {
                        if (r.bIsComplete) {
                            $("#txtSoQuyetDinhOld").val(data.sSoQuyetDinh);
                            $("#txtNgayLapOld").val(data.dNgayQuyetDinh);
                            EditThongTinChung_Off();
                        }
                    }
                });
            } else {
                var Title = 'Lỗi lưu kế hoạch vốn năm';
                var messErr = [];
                messErr.push(r.errMes);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
                return;
            }
        }
    });

    //$.ajax({
    //    type: "POST",
    //    url: "/QLVonDauTu/QLKeHoachVonNam/UpdatePhanBoVon",
    //    data: {
    //        data: data
    //    },
    //    success: function (r) {
    //        if (r.bIsComplete) {
    //            $("#txtSoQuyetDinhOld").val(data.sSoQuyetDinh);
    //            $("#txtNgayLapOld").val(data.dNgayQuyetDinh);
    //            EditThongTinChung_Off();
    //        }
    //    }
    //});
}

function CancelSaveThongTinChung() {
    $("#txtSoKeHoach").val($("#txtSoQuyetDinhOld").val());
    $("#txtNgayLap").val($("#txtNgayLapOld").val());
    EditThongTinChung_Off();
}

function EditThongTinChung_On() {
    $("button.show-edit-on").css("display", "");
    $("button.show-edit-off").css("display", "none");

    $("#txtSoKeHoach").removeAttr("disabled");
    $("#txtNgayLap").removeAttr("disabled");
}

function EditThongTinChung_Off() {
    $("button.show-edit-on").css("display", "none");
    $("button.show-edit-off").css("display", "");

    $("#txtSoKeHoach").attr("disabled", "disabled")
    $("#txtNgayLap").attr("disabled", "disabled")
}

function SaveThongTinChiTiet() {
    if (UpdateDataDetail()) {
        EditThongTinChiTiet_Off();
    }
}

function CancelSaveThongChiTiet() {
    EditThongTinChiTiet_Off();
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

function GetInfoPhanBoVonChiTietInGridViewByPhanBoVonID(id, bChinhSua) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKeHoachVonNam/GetInfoPhanBoVonChiTietInGridViewByPhanBoVonID",
        data: { iId: id },
        success: function (r) {
            if (r.bIsComplete) {
                ConvertDataGridviewEdit(r.data, bChinhSua);
                GetDataDropdownNganhEdit(r.data[0].iID_NganhID, $("#txtNamKeHoach").val());
            }
        }
    });
}

function ConvertDataGridviewEdit(lstData, bChinhSua) {
    tblDataGrid = [];
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKeHoachVonNam/GetDataGridViewDefault",
        success: function (r) {
            if (r.bIsComplete) {
                var index = 1;
                $.each(r.data, function (index, item) {
                    var itemAdd = item;
                    itemAdd.sReferId = tblDataGrid.length + 1;
                    itemAdd.iId = tblDataGrid.length + 1;
                    tblDataGrid.push(itemAdd);
                    index++;
                });

                iIdMax = tblDataGrid.length;
                $.each(lstData, function (index, value) {
                    iIdMax++;
                    var data = {};
                    data.iId = iIdMax;
                    data.iParentId = 1;
                    data.sXauNoiMa = value.sXauNoiMa;
                    data.sMaKetNoi = value.sMaKetNoi;
                    data.sTenDuAn = value.sTenDuAn;
                    data.iID_DuAnID = value.iID_DuAnID;
                    data.iId_LoaiCongTrinh = value.iID_LoaiCongTrinhID;
                    data.sTenLoaiCongTrinh = value.sTenLoaiCongTrinh;
                    data.fGiaTrPhanBo = (value.fGiaTrPhanBo??0).toLocaleString('vi-VN');
                    data.fGiaTriDauTu = (value.fGiaTriDauTu??0).toLocaleString('vi-VN');
                    data.fVonDaBoTri = (value.fVonDaBoTri ?? 0).toLocaleString('vi-VN');
                    data.fGiaTriSauDieuChinh = (value.fGiaTrPhanBo??0).toLocaleString('vi-VN');
                    data.fGiaTriDieuChinh = 0;
                    data.fChiTieuNamTruocChuaCap = 0;
                    data.fChiTieuNganSachDuocDuyet = (value.fGiaTrPhanBo??0).toLocaleString('vi-VN');
                    data.fThuHoiUngXDCBKhacv = 0;
                    data.ChiTieuNganSachNam = 0;
                    data.sGhiChu = value.sGhiChu;
                    data.iID_NganhID = value.iID_NganhID;
                    data.sTrangThaiDuAnDangKy = 2;
                    tblDataGrid.push(data);
                });
                if (bChinhSua) {
                    FillDataToGridViewChinhSua(tblDataGrid);
                } else {
                    FillDataToGridViewUpdate(tblDataGrid);
                    EditThongTinChiTiet_Off();
                }
            }
        }
    });
}

function UpdateDataDetail() {
    var data = [];
    var iParentId = $("#iId_PhanBoVonId").val();
    $.each(tblDataGrid, function (index, value) {
        if (dataExclude.indexOf(value.iId) != -1) { return; }
        var item = {};
        item.iID_PhanBoVonID = iParentId;
        item.iID_DuAnID = value.iID_DuAnID;
        item.iID_NganhID = value.iID_NganhID;
        item.fGiaTrPhanBo = value.fChiTieuNganSachDuocDuyet.replaceAll(".", "");
        item.sGhiChu = value.sGhiChu;
        data.push(item);
    });
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKeHoachVonNam/DeleteKeHoachNamChiTietByKeHoachNamId",
        data: {
            iId: iParentId
        },
        success: function (r) {
            if (!r.bIsComplete) {
                alert("Cập nhật phiếu thanh toán chi tiết thất bại !");
                return false;
            } else {
                CreatePhieuChiTietInEditView(data);
            }
        }
    });
}

function CreatePhieuChiTietInEditView(data) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKeHoachVonNam/InsertPhanBoVonChiTiet",
        data: {
            lstData: data
        },
        success: function (r) {
            if (r.bIsComplete) {
                alert("Cập nhật thành công kế hoạch năm !");
                EditThongTinChiTiet_Off();
                return true;
            }
            else {
                alert("Cập nhật chi tiết kế hoạch thất bại !");
                return false
            }
        }
    });
}

//=============================== Gridview ==============================//

function FillDataToGridViewInsert(data) {
    var columns = [
        { sField: "iId", bKey: true },
        { sField: "sReferId", bForeignKey: true },
        { sField: "iParentId", bParentKey: true },
        { sTitle: "M-TM-TM-N", sField: "sXauNoiMa", iWidth: "200px", sTextAlign: "center" },
        { sTitle: "Mã kết nối", sField: "sMaKetNoi", iWidth: "200px", sTextAligsn: "left" },
        { sTitle: "Dự án", sField: "sTenDuAn", iWidth: "400px", sTextAlign: "left", bHaveIcon: 1 },
        { sTitle: "Giá trị đầu tư", sField: "fGiaTriDauTu", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Vốn đã bố trí", sField: "fVonDaBoTri", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Chỉ tiêu năm trước chưa cấp", sField: "fChiTieuNamTruocChuaCap", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Chỉ tiêu ngân sách được duyệt", sField: "fChiTieuNganSachDuocDuyet", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Thu hồi ứng XDCB khác", sField: "fThuHoiUngXDCBKhac", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Chỉ tiêu ngân sách năm", sField: "ChiTieuNganSachNam", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Ghi chú", sField: "sGhiChu", iWidth: "300px", sTextAlign: "left" }];

    var button = { bCreateButtonInParent: false, bUpdate: 1, bDelete: 1 };
    var sHtml = GenerateTreeTable(data, columns, button)
    $("#ViewTable").html(sHtml);
    ClearButtonInParent();
    GetValueSumToTalColumn(5);
    GetValueSumToTalColumn(6);
    GetValueSumToTalColumn(7);
    GetValueSumToTalColumn(8);
    GetValueSumToTalColumn(9);
    GetValueSumToTalColumn(10);
    
}

function FillDataToGridViewUpdate(data) {
    var columns = [
        { sField: "iId", bKey: true },
        { sField: "sReferId", bForeignKey: true },
        { sField: "iParentId", bParentKey: true },
        { sTitle: "M-TM-TM-N", sField: "sXauNoiMa", iWidth: "200px", sTextAlign: "center" },
        { sTitle: "Mã kết nối", sField: "sMaKetNoi", iWidth: "200px", sTextAligsn: "left" },
        { sTitle: "Dự án", sField: "sTenDuAn", iWidth: "400px", sTextAlign: "left", bHaveIcon: 1 },
        { sTitle: "Loại công trình", sField: "sTenLoaiCongTrinh", iWidth: "200px", sTextAligsn: "left" },
        { sTitle: "Giá trị đầu tư", sField: "fGiaTriDauTu", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Vốn đã bố trí", sField: "fVonDaBoTri", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Chỉ tiêu năm trước chưa cấp", sField: "fChiTieuNamTruocChuaCap", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Chỉ tiêu ngân sách được duyệt", sField: "fChiTieuNganSachDuocDuyet", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Thu hồi ứng XDCB khác", sField: "fThuHoiUngXDCBKhac", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Chỉ tiêu ngân sách năm", sField: "ChiTieuNganSachNam", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Ghi chú", sField: "sGhiChu", iWidth: "300px", sTextAlign: "left" }];

    var button = { bCreateButtonInParent: false, bUpdate: 1, bDelete: 1 };
    var sHtml = GenerateTreeTable(data, columns, button)
    $("#ViewTable").html(sHtml);
    ClearButtonInParent();
    GetValueSumToTalColumn(6);
    GetValueSumToTalColumn(7);
    GetValueSumToTalColumn(8);
    GetValueSumToTalColumn(9);
    GetValueSumToTalColumn(10);
    GetValueSumToTalColumn(11);
}

function FillDataToGridViewChinhSua(data) {
    var columns = [
        { sField: "iId", bKey: true },
        { sField: "sReferId", bForeignKey: true },
        { sField: "iParentId", bParentKey: true },
        { sTitle: "M-TM-TM-N", sField: "sXauNoiMa", iWidth: "200px", sTextAlign: "left" },
        { sTitle: "Dự án", sField: "sTenDuAn", iWidth: "400px", sTextAlign: "left", bHaveIcon: 1 },
        { sTitle: "Mã kết nối", sField: "sMaKetNoi", iWidth: "200px", sTextAligsn: "left" },

        { sTitle: "Giá trị trước điều chỉnh", sField: "fGiaTrPhanBo", iWidth: "200px", sTextAligsn: "left" },
        { sTitle: "Loại điều chỉnh", sField: "snameLoaiDieuChinh", iWidth: "200px", sTextAlign: "left" },
        { sTitle: "Giá trị điều chỉnh", sField: "fGiaTriDieuChinh", iWidth: "200px", sTextAlign: "left" },
        { sTitle: "Giá trị sau điều chỉnh", sField: "fGiaTriSauDieuChinh", iWidth: "200px", sTextAlign: "left" }];

    var button = { bCreateButtonInParent: false, bUpdate: 1 };
    var sHtml = GenerateTreeTable(data, columns, button)
    $("#ViewTable").html(sHtml);
    ClearButtonInParent();
    GetValueSumToTalColumn(5);
    GetValueSumToTalColumn(6);
    GetValueSumToTalColumn(7);
    GetValueSumToTalColumn(8);
}

function ClearButtonInParent() {
    var iIdExlude = [1, 2, 3];
    $("tr").each(function (index, value) {
        var dataRow = $(this).data("row");
        if (iIdExlude.indexOf(dataRow) != -1) {
            $(this).find("button").remove();
            $(this).find("i").removeClass("fa-file-text");
            $(this).find("i").addClass("fa-folder-open");
        }
    });
}

function GetDataGridViewDefault() {
    tblDataGrid = [];
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLKeHoachVonNam/GetDataGridViewDefault",
        success: function (r) {
            if (r.bIsComplete) {
                var index = 1;
                $.each(r.data, function (index, item) {
                    var itemAdd = item;
                    itemAdd.sReferId = tblDataGrid.length + 1;
                    itemAdd.iId = tblDataGrid.length + 1;
                    tblDataGrid.push(itemAdd);
                    index++;
                });

                FillDataToGridViewInsert(tblDataGrid);
            }
        }
    });
}

function DeleteItem(iId) {
    tblDataGrid = $.map(tblDataGrid, function (n) { return n.iId != iId ? n : null });
    FillDataToGridViewInsert(tblDataGrid);
}

function GetValueSumToTalColumn(index) {
    var fTotalValue = 0;
    $.each($("#ViewTable table td tr td:nth-child(" + index + ")"), function (index, value) {
        if ($.isNumeric($(value).html().replaceAll(".", ""))){
            fTotalValue += Number($(value).html().replaceAll(".", ""));
        }
    });
    $("#SumTable td:nth-child("+index+")").html(fTotalValue.toLocaleString('vi-VN'));
}

//=========================== validate ===========//
function ValidateDataInsert() {
    var sMessError = [];
    if ($("#drpDonViQuanLy").val() == null) {
        sMessError.push("Chưa nhập đơn vị quản lý !");
    }
    if ($("#txtSoKeHoach").val().trim() == "") {
        sMessError.push("Chưa nhập số kế hoạch !");
    }
    if ($("#txtNgayLap").val().trim() == "") {
        sMessError.push("Chưa nhập ngày lập !");
    }
    if ($("#drpNguonNganSach").val() == null) {
        sMessError.push("Chưa nhập nguồn vốn !");
    }
    if ($("#drpLoaiNganSach").val() == null) {
        sMessError.push("Chưa nhập loại ngân sách !");
    }
    if ($("#txtNamKeHoach").val() == null) {
        sMessError.push("Chưa nhập năm kế hoạch !");
    }
    var lstThongTinchiTiet = $.map(tblDataGrid, function (n) { return dataExclude.indexOf(n.iId) == -1 ? n : null });
    if (lstThongTinchiTiet.length == 0) {
        sMessError.push("Chưa nhập thông tin chi tiết !");
    }
    if (sMessError.length != 0) {
        alert(sMessError.join('\n'));
        return false;
    }
    return true;
}