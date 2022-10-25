var TBL_DANH_SACH_GOI_THAU = "tblDanhSachGoiThau";
var TBL_DANH_SACH_DU_TOAN = "tblDanhSachDuToan";
var TBL_HANG_MUC_CHINH = "tblHangMucChinh";
var TBL_DANH_SACH_NGUON_VON = "tblDanhSachNguonVon";
var TBL_DANH_SACH_CHI_PHI = "tblDanhSachChiPhi";
var TBL_DANH_SACH_NGUON_VON_MODAL = "tblDanhSachNguonVonModal";
var TBL_DANH_SACH_CHI_PHI_MODAL = "tblDanhSachChiPhiModal";
var TBL_DANH_SACH_HANG_MUC_MODAL = "tblDanhSachHangMuc";

var iID_DuToanID_select = "";
var iID_GoiThauID_select = "";

var arrGoiThau = [];
var arrGoiThauNguonVon = [];
var arrGoiThauChiPhi = [];
var arrGoiThauHangMuc = [];

var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;

var iIDDuAnID = "";
var iIDKHLuaChonNTID = "";
var iID_DuToanID = "";
var loadFirst = true;

$(document).ready(function ($) {
    LoadDataComboBoxNguonNganSach()
    //EventChangeDonViQuanLy();
    //EventChangeDuAn();

    iIDKHLuaChonNTID = $("#iID_KHLuaChonNTID").val();
    iIDDuAnID = $("#iID_DuAnID_old").val();
    iID_DuToanID = $("#iID_DuToanID").val();
    GetDanhSachDuToan(iID_DuToanID);
    GetDanhSachGoiThau(iIDKHLuaChonNTID);

    $("#iID_DonViQuanLyID").trigger("change");
});

function GetDanhSachGoiThau(id) {
    $.ajax({
        url: "/QLVonDauTu/KHLuaChonNhaThau/LayThongTinGoiThauChiTietTheoKHLuaChonNhaThau",
        type: "POST",
        data: { id: id },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null) {
                // fill thong tin chi tiet du an
                if (data != null) {
                    arrGoiThau = data.lstGoiThau;
                    arrGoiThauNguonVon = data.lstGoiThauNguonVon;
                    arrGoiThauChiPhi = data.lstGoiThauChiPhi;
                    arrGoiThauHangMuc = data.lstGoiThauHangMuc;
                    VeDanhSachGoiThau();
                }
            }
        },
        error: function (data) {

        }
    });
}

function VeDanhSachGoiThau() {
    var html = "";
    var index = 1;
    arrGoiThau.forEach(function (x) {
        var dongMoi = "";
        dongMoi += "<tr style='cursor: pointer;' class='parent' data-id='" + x.iID_GoiThauID + "'>";
        dongMoi += "<td class='r_STT'>" + index + "</td>";
        dongMoi += "<input type='hidden' class='r_iID_DuToanID' value='" + x.iID_DuToanID + "'/><input type='hidden' class='r_iID_GoiThauID' value='" + x.iID_GoiThauID + "'/>";
        dongMoi += "<td class='r_sTenGoiThau' contenteditable='true'>" + x.sTenGoiThau + "</td>";
        dongMoi += "<td class='r_fGiaGoiThau sotien' align='right'>" + x.fTienTrungThau + "</td>";
        dongMoi += "<td class='r_HinhThucLuaChonNhaThau'>" + CreateHtmlSelectHinhThucChonNhaThau(x.sHinhThucChonNhaThau) + "</td>";
        dongMoi += "<td class='r_PhuongThucChonNhaThau'>" + CreateHtmlSelectPhuongThucChonNhaThau(x.sPhuongThucDauThau) + "</td>";
        dongMoi += "<td class='r_LoaiHopDong'>" + CreateHtmlSelectLoaiHopDong(x.sHinhThucHopDong) + "</td>";
        dongMoi += "<td class='r_iThoiGianThucHien sotien' contenteditable='true' align='right'>" + x.iThoiGianThucHien + "</td>";
        dongMoi += "<td><button class='btn-delete btn-icon' type='button' onclick='XoaDong(this)'><span class='fa fa-trash-o fa-lg' aria-hidden='true'></span></button></td>";

        html += dongMoi;
        index++;
    })

    $("#" + TBL_DANH_SACH_GOI_THAU + " tbody").append(html);
    DinhDangSo(TBL_DANH_SACH_GOI_THAU);
    SuKienDbClickDongTable();
}

//function EventChangeDonViQuanLy() {
//    $("#iID_DonViQuanLyID").change(function () {
//        if (this.value != "" && this.value != GUID_EMPTY) {
//            $.ajax({
//                url: "/QLVonDauTu/QLPheDuyetDuAn/LayDanhSachDuAnTheoDonViQuanLy",
//                type: "POST",
//                data: { iID_DonViQuanLyID: this.value },
//                dataType: "json",
//                cache: false,
//                success: function (data) {
//                    if (data != null && data != "") {
//                        $("#iID_DuAnID").html(data);
//                        if (loadFirst) {
//                            $("#iID_DuAnID").val(iIDDuAnID).trigger("change");
//                            loadFirst = false;
//                        } else {
//                            $("#iID_DuAnID").trigger("change");
//                        }
//                    }
//                },
//                error: function (data) {

//                }
//            })
//        } else {
//            $("#iID_DuAnID option:not(:first)").remove();
//            $("#iID_DuAnID").trigger("change");
//        }
//    });
//}

//function EventChangeDuAn() {
//    $("#iID_DuAnID").change(function () {
//        $("#fTongMucDauTuPheDuyet").val("");
//        $('#iID_ChuDauTuID option:eq(0)').prop('selected', true);
//        if (this.value != "" && this.value != GUID_EMPTY) {
//            // lay thong tin chi tiet
//            GetThongTinDuAn(this.value);
//            GetTMPDTheoChuTruongDauTu(this.value);
//            GetDanhSachDuToan(this.value);
//        } else {
//            //$("#sMaDuAn").val("");
//        }
//    });
//}

//function GetThongTinDuAn(id) {
//    $.ajax({
//        url: "/QLVonDauTu/ChuTruongDauTu/LayThongTinChiTietDuAn",
//        type: "POST",
//        data: { iID: id },
//        dataType: "json",
//        cache: false,
//        success: function (data) {
//            if (data != null) {
//                // fill thong tin chi tiet du an
//                if (data.iID_ChuDauTuID != null) {
//                    $("#iID_ChuDauTuID").val(data.iID_ChuDauTuID);
//                }
//            }
//        },
//        error: function (data) {

//        }
//    });
//}

var arrDuToan = [];
function GetDanhSachDuToan(id) {
    $.ajax({
        url: "/QLVonDauTu/KHLuaChonNhaThau/LayDanhSachDuToanTheoKHLCNhaThauId",
        type: "POST",
        data: { duToanId: id },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null) {
                // fill thong tin chi tiet du an
                if (data[0] != null ) {
                    arrDuToan = data;

                    var htmlDanhSachDuToan = "";
                    data.forEach(function (x) {
                        htmlDanhSachDuToan += "<tr>";
                        htmlDanhSachDuToan += "<td align='center'><input type='checkbox' data-dutoanid='" + x.iID_DuToanID + "' name='cb_DuToan' class='cb_DuToan' checked disabled></td>";
                        htmlDanhSachDuToan += "<td align='left'>" + x.sSoQuyetDinh + "</td>";
                        htmlDanhSachDuToan += "<td align='center'>" + x.sNgayQuyetDinh + "</td>";
                        htmlDanhSachDuToan += "<td align='left'>" + x.sTenDonViQL + "</td>";
                        htmlDanhSachDuToan += "<td align='right'>" + FormatNumber(x.fTongDuToanPheDuyet) + "</td>";
                        htmlDanhSachDuToan += "</tr>";
                    })

                    $("#" + TBL_DANH_SACH_DU_TOAN + " tbody").html(htmlDanhSachDuToan);
                    EventCheckboxDuToan();

                    DisplayNguonVonChiPhi(arrDuToan.map(function (x) { return x.iID_DuToanID }), TBL_DANH_SACH_NGUON_VON, TBL_DANH_SACH_CHI_PHI);
                    DinhDangSo(TBL_DANH_SACH_CHI_PHI);
                }
            }
        },
        error: function (data) {

        }
    });
}

function DisplayNguonVonChiPhi(arrDuToanID, idDisplayNguonVon, idDisplayChiPhi) {
    var arrDuToanDisplay = arrDuToan.filter(function (x) { return arrDuToanID.indexOf(x.iID_DuToanID) > -1 });

    var arrNguonVon = [];
    var arrChiPhi = [];
    arrDuToanDisplay.forEach(function (objDuToan) {
        if (objDuToan.ListNguonVon != null && objDuToan.ListNguonVon.length > 0) {
            objDuToan.ListNguonVon.forEach(function (objNguonVon) {
                if (arrNguonVon.filter(function (x) { return x.sTenNguonVon == objNguonVon.sTenNguonVon }).length == 0) {
                    arrNguonVon.push({
                        sTenNguonVon: objNguonVon.sTenNguonVon,
                        fTienPheDuyet: parseInt(objNguonVon.fTienPheDuyet == undefined ? 0 : objNguonVon.fTienPheDuyet)
                    })
                } else {
                    var nv = arrNguonVon.filter(function (x) { return x.sTenNguonVon == objNguonVon.sTenNguonVon })[0];
                    nv.fTienPheDuyet += parseInt(objNguonVon.fTienPheDuyet == undefined ? 0 : objNguonVon.fTienPheDuyet);
                }
            })
        }

        if (objDuToan.ListChiPhi != null && objDuToan.ListChiPhi.length > 0) {
            objDuToan.ListChiPhi.forEach(function (objChiPhi) {
                if (arrChiPhi.filter(function (x) { return x.sTenChiPhi == objChiPhi.sTenChiPhi }).length == 0) {
                    arrChiPhi.push({
                        sTenChiPhi: objChiPhi.sTenChiPhi,
                        fTienPheDuyet: parseInt(objChiPhi.fTienPheDuyet == undefined ? 0 : objChiPhi.fTienPheDuyet),
                        Id: objChiPhi.Id,
                        iID_ChiPhiCha: objChiPhi.iID_ChiPhiCha
                    })
                } else {
                    var nv = arrChiPhi.filter(function (x) { return x.sTenChiPhi == objChiPhi.sTenChiPhi })[0];
                    nv.fTienPheDuyet += parseInt(objChiPhi.fTienPheDuyet == undefined ? 0 : objChiPhi.fTienPheDuyet);
                }
            })
        }
    })

    // nguon von
    var htmlNguonVon = "";
    arrNguonVon.forEach(function (x) {
        htmlNguonVon += "<tr>";
        htmlNguonVon += "<td>" + x.sTenNguonVon + "</td>";
        htmlNguonVon += "<td align='right'>" + FormatNumber(x.fTienPheDuyet) + "</td>";
        htmlNguonVon += "</tr>";
    })
    $("#" + idDisplayNguonVon + " tbody").html(htmlNguonVon);

    // chi phi  
    var arrParent = arrChiPhi.filter(function (x) { return x.iID_ChiPhiCha == null });
    arrParent.forEach(function (parent) {
        TinhTongCon(parent, arrChiPhi);
    })

    var columns = [
        { sField: "Id", bKey: true },
        { sField: "iID_ChiPhiCha", bParentKey: true },

        { sTitle: "Chi phí", sField: "sTenChiPhi", iWidth: "70%", sTextAlign: "left", bHaveIcon: 0, iMain: 1 },
        { sTitle: "Giá trị phê duyệt", sField: "fTienPheDuyet", iWidth: "30%", sTextAlign: "right", sClass: "sotien" }];
    var button = { bUpdate: 0, bDelete: 0, bInfo: 0 };
    var sHtml = GenerateTreeTableKHLuaChonNhaThau(arrChiPhi, columns, button, "", false)

    $("#" + idDisplayChiPhi).html(sHtml);
}

var arrNguonVonModal = [];
var arrChiPhiModal = [];
function DisplayNguonVonChiPhiModal(iID_DuToanID, idDisplayNguonVon, idDisplayChiPhi) {
    var objDuToan = arrDuToan.filter(function (x) { return x.iID_DuToanID == iID_DuToanID })[0];

    if (objDuToan.ListNguonVon != null && objDuToan.ListNguonVon.length > 0) {
        objDuToan.ListNguonVon.forEach(function (objNguonVon) {
            if (arrNguonVonModal.filter(function (x) { return x.sTenNguonVon == objNguonVon.sTenNguonVon }).length == 0) {
                arrNguonVonModal.push({
                    iID_NguonVonID: objNguonVon.iID_NguonVonID,
                    sTenNguonVon: objNguonVon.sTenNguonVon,
                    fTienPheDuyet: parseInt(objNguonVon.fTienPheDuyet == undefined ? 0 : objNguonVon.fTienPheDuyet),
                    //fGiaTriGoiThau: 0
                })
            } else {
                var nv = arrNguonVonModal.filter(function (x) { return x.sTenNguonVon == objNguonVon.sTenNguonVon })[0];
                nv.fTienPheDuyet += parseInt(objNguonVon.fTienPheDuyet == undefined ? 0 : objNguonVon.fTienPheDuyet);
            }
        })
    }

    if (objDuToan.ListChiPhi != null && objDuToan.ListChiPhi.length > 0) {
        objDuToan.ListChiPhi.forEach(function (objChiPhi) {
            if (arrChiPhiModal.filter(function (x) { return x.sTenChiPhi == objChiPhi.sTenChiPhi }).length == 0) {
                arrChiPhiModal.push({
                    sTenChiPhi: objChiPhi.sTenChiPhi,
                    fTienPheDuyet: parseInt(objChiPhi.fTienPheDuyet == undefined ? 0 : objChiPhi.fTienPheDuyet),
                    Id: objChiPhi.Id,
                    iID_ChiPhiCha: objChiPhi.iID_ChiPhiCha,
                    fGiaTriGoiThau: 0
                })
            } else {
                var nv = arrChiPhiModal.filter(function (x) { return x.sTenChiPhi == objChiPhi.sTenChiPhi })[0];
                nv.fTienPheDuyet += parseInt(objChiPhi.fTienPheDuyet == undefined ? 0 : objChiPhi.fTienPheDuyet);
            }
        })
    }

    // update gia tri goi thau
    arrNguonVonModal.forEach(function (nv) {
        var objGoiThauNguonVon = arrGoiThauNguonVon.find(function (x) { return x.iID_DuToanID == iID_DuToanID && x.iID_GoiThauID == iID_GoiThauID_select && x.iID_NguonVonID == nv.iID_NguonVonID });
        if (objGoiThauNguonVon != undefined) {
            nv.fTienGoiThau = objGoiThauNguonVon.fTienGoiThau;
            nv.bChecked = 1
        }
    })

    arrChiPhiModal.forEach(function (cp) {
        var objGoiThauChiPhi = arrGoiThauChiPhi.find(function (x) { return x.iID_DuToanID == iID_DuToanID && x.iID_GoiThauID == iID_GoiThauID_select && x.iID_ChiPhiID == cp.Id });
        if (objGoiThauChiPhi != undefined) {
            cp.fGiaTriGoiThau = objGoiThauChiPhi.fTienGoiThau;
            cp.bChecked = 1
        }
    })

    // nguon von
    var htmlNguonVon = "";
    arrNguonVonModal.forEach(function (x) {
        htmlNguonVon += "<tr>";
        htmlNguonVon += "<td><input type='checkbox' data-nguonvonid='" + x.iID_NguonVonID + "' name='cb_NguonVonModal' class='cb_NguonVonModal' " + (x.bChecked == 1 ? "checked" : "") + "/></td>"
        htmlNguonVon += "<td>" + x.sTenNguonVon + "</td>";
        htmlNguonVon += "<td contenteditable='true' class='r_fGiaGoiThau sotien' align='right'>" + (x.fTienGoiThau == undefined ? "" : x.fTienGoiThau) + "</td>";
        htmlNguonVon += "<td align='right'>" + FormatNumber(x.fTienPheDuyet) + "</td>";
        htmlNguonVon += "</tr>";
    })
    $("#" + idDisplayNguonVon + " tbody").html(htmlNguonVon);

    // chi phi  
    //var arrParent = arrChiPhiModal.filter(function (x) { return x.iID_ChiPhiCha == null });
    //arrParent.forEach(function (parent) {
    //    TinhTongCon(parent, arrChiPhiModal);
    //    TinhTongConGiaTriGoiThau(parent, arrChiPhiModal);
    //})

    //var columns = [
    //    { sField: "Id", bKey: true },
    //    { sField: "iID_ChiPhiCha", bParentKey: true },
    //    { sTitle: "", sField: "bChecked", sType: "checkbox", iWidth: "5%" },
    //    { sTitle: "Chi phí", sField: "sTenChiPhi", iWidth: "55%", sTextAlign: "left", bHaveIcon: 0, iMain: 1 },
    //    { sTitle: "Giá trị gói thầu", sField: "fGiaTriGoiThau", iWidth: "15%", sTextAlign: "right", sClass: "sotien", bEditable: 1 },
    //    { sTitle: "Giá trị phê duyệt", sField: "fTienPheDuyet", iWidth: "15%", sTextAlign: "right", sClass: "sotien" }];
    //var button = { bUpdate: 0, bDelete: 0, bInfo: 1 };
    //var sHtml = GenerateTreeTableKHLuaChonNhaThau(arrChiPhiModal, columns, button, "", false)

    //$("#" + idDisplayChiPhi).html(sHtml);

    LoadViewChiPhiByGoiThau(arrChiPhiModal);

    // disable button view hang muc
    $("#" + TBL_DANH_SACH_CHI_PHI_MODAL + " .cb_tbody").each(function (obj) {
        var thisDong = this.parentElement.parentElement;
        var iID_ChiPhiID = $(thisDong).data("row");
        var checkboxCon = $("#" + TBL_DANH_SACH_CHI_PHI_MODAL + " tr[data-iid_chiphicha='" + iID_ChiPhiID + "']");
        if (checkboxCon.length > 0) {
            $(thisDong).find(".col-btn").html("");
        } else {
            if ($(thisDong).find("input[type='checkbox']").is(":checked") == false)
                $(thisDong).find("button").hide();
        }
    })
}

function LoadViewChiPhiByGoiThau(items) {
    $("#tblChiPhiDauTu tbody").html("");
    var html = "";
    items.forEach(x => {

        var duanChiPhiId = x.Id.toString();

        //var duanChiPhiString = duanChiPhiId.toString();
        // var stt = count;
        html += '<tr>';
        html += '<input type="hidden" class="r_IsEdit" value = "' + x.isEdit + '"/> <input type="hidden" class="r_ID" value = "' + x.Id + '"/>';
        html += "<td><input type='checkbox' class='cb_tbody'/></td>"
        html += '<td style="font-weight:bold;text-align:left;" > <input type = "text" disabled class="form-control r_TenChiPhi" value = "' + x.sTenChiPhi + '" /> </td> <input type="hidden" id="' + duanChiPhiId + '" class="r_iID_DuAn_ChiPhiID" value="' + duanChiPhiId + '"/> <input type="hidden" class="r_iID_ChiPhiParentID" value="' + x.iID_ChiPhi_Parent + '"/> <input type="hidden" class="r_iID_ChiPhiID" value = "' + x.iID_ChiPhiID + '"/>';
        html += '<td style="text-align:right"><input type="text" style="text-align: right" class="form-control r_fGiaTriGoiThau" onchange ="UpdateArrGoiThauChiPhi(\'' + duanChiPhiId + '\',this)" onkeyup="ValidateNumberKeyUp(this); " onkeypress="return ValidateNumberKeyPress(this, event); " /> </td>';
        html += '<td style="text-align:right"><input type="text" disabled style="text-align: right" class="form-control txtGiaTriChiPhi" onkeyup="ValidateNumberKeyUp(this); " onkeypress="return ValidateNumberKeyPress(this, event); " value="' + FormatNumber(x.fTienPheDuyet) + '"/> </td>';
        html += "<td align='center'>";
        if (x.isEdit) {
            html += "<button class='btn-detail' type = 'button' onclick = 'ViewDetail(\"" + duanChiPhiId + "\")' > " +
                "<i class='fa fa-eye fa-lg' aria-hidden='true'></i>" +
                "</button> ";
        }

        html += "</td></tr>";

        //count++;
    });

    $("#tblChiPhiDauTu tbody").append(html);
}

function TinhTongCon(root, arr) {
    var arrChil = arr.filter(function (x) { return x.iID_ChiPhiCha == root.Id });
    if (arrChil.length > 0) {
        root.fTienPheDuyet = 0;
        arrChil.forEach(child => {
            root.fTienPheDuyet += TinhTongCon(child, arr);
        });
    } else
        return root.fTienPheDuyet;
}

function TinhTongConGiaTriGoiThau(root, arr) {
    var arrChil = arr.filter(function (x) { return x.iID_ChiPhiCha == root.Id });
    if (arrChil.length > 0) {
        root.fGiaTriGoiThau = 0;
        arrChil.forEach(child => {
            root.fGiaTriGoiThau += TinhTongConGiaTriGoiThau(child, arr);
        });
    } else
        return root.fGiaTriGoiThau;
}

function EventCheckboxDuToan() {
    $(".cb_DuToan").change(function () {
        if (this.checked) {
            $(".cb_DuToan").prop('checked', false);
            $(this).prop('checked', true);
            iID_DuToanID_select = $(this).data('dutoanid');
            DisplayNguonVonChiPhi(new Array($(this).data('dutoanid')), TBL_DANH_SACH_NGUON_VON, TBL_DANH_SACH_CHI_PHI);
        } else {
            iID_DuToanID_select = "";
            DisplayNguonVonChiPhi(arrDuToan.map(function (x) { return x.iID_DuToanID }), TBL_DANH_SACH_NGUON_VON, TBL_DANH_SACH_CHI_PHI);
        }
    });
}

var iIDChiPhiDetail = "";
function ViewDetail(iidChiPhi) {
    iIDChiPhiDetail = iidChiPhi;
    var objDuToan = arrDuToan.find(function (x) { return x.iID_DuToanID == iID_DuToanID_select });
    if (objDuToan != undefined) {
        var arrHangMucDisplay = objDuToan.ListHangMuc.filter(function (x) { return x.iID_DuAn_ChiPhi == iidChiPhi });
        LoadViewHangMuc(arrHangMucDisplay);

        /*DinhDangSo(TBL_DANH_SACH_HANG_MUC_MODAL);*/
        EventHangMucModal(iidChiPhi);
    }
}

function LoadViewHangMuc(data) {
    $("#tblHangMucChinh tbody").html("");
    //var data = GetListDataHangMuc();
    for (var i = 0; i < data.length; i++) {
        var dongMoi = "";
        dongMoi += "<tr style='cursor: pointer;' class='parent'>";
        dongMoi += "<td><input type='checkbox' checked class='cb_tbody'/></td>"
        dongMoi += "<td class='r_STT'>" + data[i].smaOrder + "</td><input type='hidden' id='" + data[i].iID_DuToan_DM_HangMucID + "' class='r_iID_DuAn_HangMucID' value='" + data[i].iID_DuToan_DM_HangMucID + "'/> <input type='hidden' class='r_HangMucID' value='" + data[i].iID_HangMucID + "'/> <input type='hidden' class='r_HangMucParentID' value='" + data[i].iID_ParentID + "'/> <input type='hidden' class='r_IsEdit' value='" + data[i].isEdit + "'/>";
        dongMoi += "<td><input type='text' disabled class='form-control txtTenHangMuc' value='" + data[i].sTenHangMuc + "'/></td>"
        dongMoi += "<td align='right'><input type='text' style='text-align: right' disabled  class='form-control r_fTienPheDuyet' value='" + FormatNumber(data[i].fTienPheDuyet) + "'/></td>";

        dongMoi += "</tr>";

        $("#tblHangMucChinh tbody").append(dongMoi);
    }
    $("#cbAll_HangMuc").prop('checked', false);

}

function EventHangMucModal() {
    $("#" + TBL_DANH_SACH_HANG_MUC_MODAL + " .cbAll").change(function () {
        if (this.checked)
            $("#" + TBL_DANH_SACH_HANG_MUC_MODAL + " .cb_tbody").prop('checked', true).trigger("change");
        else
            $("#" + TBL_DANH_SACH_HANG_MUC_MODAL + " .cb_tbody").prop('checked', false).trigger("change");
    })

    $("#" + TBL_DANH_SACH_HANG_MUC_MODAL + " .cb_tbody").change(function () {
        // checked con
        var thisDong = this.parentElement.parentElement;
        if (this.checked) {
            var iID_HangMucID = $(thisDong).data("row");
            var fTienGoiThau = $(thisDong).find(".r_fTienPheDuyet").html();
            var checkboxCon = $("#" + TBL_DANH_SACH_HANG_MUC_MODAL + " tr[data-iid_parentid='" + iID_HangMucID + "']");
            checkboxCon.each(function () {
                $(this).find("input[type='checkbox']").prop("checked", true).trigger("change");
            })

            arrGoiThauHangMuc.push({
                iID_GoiThauID: iID_GoiThauID_select,
                iID_HangMucID: iID_HangMucID,
                fTienGoiThau: parseInt(fTienGoiThau == "" ? 0 : UnFormatNumber(fTienGoiThau)),
                iID_DuToanID: iID_DuToanID_select,
                iID_ChiPhiID: iIDChiPhiDetail
            })
        } else {
            arrGoiThauHangMuc = arrGoiThauHangMuc.filter(function (x) { return x.iID_GoiThauID != iID_GoiThauID_select && x.iID_HangMucID != iID_HangMucID });
        }

        // tinh tong
        var fTongHangMuc = TinhTongHangMuc();
        $("#" + TBL_DANH_SACH_CHI_PHI_MODAL + " tr[data-id='" + iIDChiPhiDetail + "']").find(".r_fGiaTriGoiThau").html(FormatNumber(fTongHangMuc)).trigger("change");
    })
}

var fTongNguonVon = 0;
var fTongChiPhi = 0;
function EventModal() {
    $("#" + TBL_DANH_SACH_NGUON_VON_MODAL + " .cbAll_NguonVon").change(function () {
        if (this.checked)
            $("#" + TBL_DANH_SACH_NGUON_VON_MODAL + " .cb_NguonVonModal").prop('checked', true).trigger("change");
        else
            $("#" + TBL_DANH_SACH_NGUON_VON_MODAL + " .cb_NguonVonModal").prop('checked', false).trigger("change");
    })

    $("#" + TBL_DANH_SACH_CHI_PHI_MODAL + " .cbAll").change(function () {
        if (this.checked)
            $("#" + TBL_DANH_SACH_CHI_PHI_MODAL + " .cb_tbody").prop('checked', true).trigger("change");
        else
            $("#" + TBL_DANH_SACH_CHI_PHI_MODAL + " .cb_tbody").prop('checked', false).trigger("change");
    })

    // tinh tong nguon von
    $("#" + TBL_DANH_SACH_NGUON_VON_MODAL + " .cb_NguonVonModal, #" + TBL_DANH_SACH_NGUON_VON_MODAL + " .r_fGiaGoiThau").change(function () {
        TinhTongNguonVon();
        ShowTongConLai();
    })

    $("#" + TBL_DANH_SACH_NGUON_VON_MODAL + " [contenteditable]").on('focusout', function (ev) {
        TinhTongNguonVon();
        ShowTongConLai();
    });

    // tinh tong chi phi
    $("#" + TBL_DANH_SACH_CHI_PHI_MODAL + " .cb_tbody, #" + TBL_DANH_SACH_CHI_PHI_MODAL + " .r_fGiaTriGoiThau").change(function () {
        // checked con
        var thisDong = $(this).closest("tr");
        if ($(thisDong).find("input[type='checkbox']").is(":checked")) {
            $(thisDong).find("button").show();
            var iID_ChiPhiID = $(thisDong).data("row");
            var fTienGoiThau = $(thisDong).find(".r_fGiaTriGoiThau").html().trim();
            var checkboxCon = $("#" + TBL_DANH_SACH_CHI_PHI_MODAL + " tr[data-iid_chiphicha='" + iID_ChiPhiID + "']");
            checkboxCon.each(function () {
                $(this).find("input[type='checkbox']").prop("checked", true).trigger("change");
            })
        } else {
            $(thisDong).find("button").hide();
        }

        UpdateChiPhiTienGoiThau(thisDong);

        // tinh tong
        TinhTongChiPhi();
        ShowTongConLai();
    })

    $("#" + TBL_DANH_SACH_CHI_PHI_MODAL + " [contenteditable]").on('focusout', function (ev) {
        // update gia tri goi thau cha/con
        UpdateChiPhiTienGoiThau(this.parentElement);

        // tinh tong chi phi
        TinhTongChiPhi();
        ShowTongConLai();
    });
}

function ShowTongConLai() {
    $("#" + TBL_DANH_SACH_NGUON_VON_MODAL + " .rTongNguonVon").html(fTongNguonVon == 0 ? 0 : FormatNumber(fTongNguonVon));
    var rConLai = fTongNguonVon - fTongChiPhi;
    $("#" + TBL_DANH_SACH_NGUON_VON_MODAL + " .rConLai").html(rConLai == 0 ? 0 : FormatNumber(fTongNguonVon - fTongChiPhi));
}

function UpdateChiPhiTienGoiThau(row) {
    var iID_ChiPhiID = $(row).data("row");
    var fGiaTriGoiThau = $(row).find(".r_fGiaTriGoiThau").html().trim();

    var cp = arrChiPhiModal.filter(function (x) { return x.Id == iID_ChiPhiID });
    cp[0].fGiaTriGoiThau = parseInt(fGiaTriGoiThau == "" ? 0 : UnFormatNumber(fGiaTriGoiThau));

    var arrParent = arrChiPhiModal.filter(function (x) { return x.iID_ChiPhiCha == null });
    arrParent.forEach(function (parent) {
        TinhTongGoiThauCon(parent, arrChiPhiModal);
    })

    arrChiPhiModal.forEach(function (x) {
        $("#" + TBL_DANH_SACH_CHI_PHI_MODAL + " tr[data-row='" + x.Id + "']").find(".r_fGiaTriGoiThau").html(FormatNumber(x.fGiaTriGoiThau));
    })
}

function TinhTongGoiThauCon(root, arr) {
    var arrChil = arr.filter(function (x) { return x.iID_ChiPhiCha == root.Id });
    if (arrChil.length > 0) {
        root.fGiaTriGoiThau = 0;
        arrChil.forEach(child => {
            root.fGiaTriGoiThau += TinhTongGoiThauCon(child, arr);
        });
    } else
        return (root.fGiaTriGoiThau == undefined || root.fGiaTriGoiThau == "") ? 0 : parseInt(root.fGiaTriGoiThau);
}

function TinhTongNguonVon() {
    fTongNguonVon = 0;
    $("#" + TBL_DANH_SACH_NGUON_VON_MODAL + " .cb_NguonVonModal:checked").each(function (obj) {
        var thisDong = this.parentElement.parentElement;
        var nv = $(thisDong).find(".r_fGiaGoiThau").html().trim();
        fTongNguonVon += parseInt(nv == "" ? 0 : UnFormatNumber(nv));
    })
}

function TinhTongChiPhi(row) {
    fTongChiPhi = 0;
    $("#" + TBL_DANH_SACH_CHI_PHI_MODAL + " .cb_tbody:checked").each(function (obj) {
        var thisDong = this.parentElement.parentElement;
        var iID_ChiPhiID = $(thisDong).data("row");
        var checkboxCon = $("#" + TBL_DANH_SACH_CHI_PHI_MODAL + " tr[data-iid_chiphicha='" + iID_ChiPhiID + "']");
        if (checkboxCon.length > 0)
            return;
        var cp = $(thisDong).find(".r_fGiaTriGoiThau").html().trim();
        fTongChiPhi += parseInt(cp == "" ? 0 : UnFormatNumber(cp));
    })
}

function TinhTongHangMuc(row) {
    var fTongHangMuc = 0;
    $("#" + TBL_DANH_SACH_HANG_MUC_MODAL + " .cb_tbody:checked").each(function (obj) {
        var thisDong = this.parentElement.parentElement;
        var iID_HangMucID = $(thisDong).data("row");
        var checkboxCon = $("#" + TBL_DANH_SACH_HANG_MUC_MODAL + " tr[data-iid_parentid='" + iID_HangMucID + "']");
        if (checkboxCon.length > 0)
            return;
        var hm = $(thisDong).find(".r_fTienPheDuyet").html().trim();
        fTongHangMuc += parseInt(hm == "" ? 0 : UnFormatNumber(hm));
    })
    return fTongHangMuc;
}

function GetTMPDTheoChuTruongDauTu(id) {
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetDuAn/GetTMPDTheoChuTruongDT",
        type: "POST",
        data: { idDuAn: id },
        dataType: "json",
        cache: false,
        success: function (resp) {
            if (resp.status == true) {
                $("#fTongMucDauTuPheDuyet").val(FormatNumber(resp.data));
            }
        }
    });
}

function Luu() {
    if (CheckLoi()) {
        var objKHLuaChonNhaThau = {};
        arrGoiThau = [];
        var data = {};

        objKHLuaChonNhaThau.Id = iIDKHLuaChonNTID;
        objKHLuaChonNhaThau.sSoQuyetDinh = $("#sSoQuyetDinh").val();
        objKHLuaChonNhaThau.dNgayQuyetDinh = $("#dNgayQuyetDinh").val();
        objKHLuaChonNhaThau.sMoTa = $("#sMoTa").val();

        // add goi thau
        $("#" + TBL_DANH_SACH_GOI_THAU + " tbody tr").each(function () {
            arrGoiThau.push({
                iID_GoiThauID: $(this).find(".r_iID_GoiThauID").val(),
                iID_DuToanID: $(this).find(".r_iID_DuToanID").val(),
                iID_DuAnID: $("#iID_DuAnID").val(),
                sTenGoiThau: $(this).find(".r_sTenGoiThau").html(),
                sHinhThucChonNhaThau: $(this).find(".r_HinhThucLuaChonNhaThau select option:selected").text(),
                sPhuongThucDauThau: $(this).find(".r_PhuongThucChonNhaThau select option:selected").text(),
                sHinhThucHopDong: $(this).find(".r_LoaiHopDong select option:selected").text(),
                //sLoaiGoiThau
                sSoQuyetDinh: $("#sSoQuyetDinh").val(),
                dNgayQuyetDinh: $("#dNgayQuyetDinh").val(),
                iThoiGianThucHien: parseInt($(this).find(".r_iThoiGianThucHien").html() == "" ? 0 : UnFormatNumber($(this).find(".r_iThoiGianThucHien").html())),
                fTienTrungThau: parseInt($(this).find(".r_fGiaGoiThau ").html() == "" ? 0 : UnFormatNumber($(this).find(".r_fGiaGoiThau ").html()))
            })
        })

        data = {
            objKHLuaChonNhaThau: objKHLuaChonNhaThau,
            lstGoiThau: arrGoiThau,
            lstGoiThauNguonVon: arrGoiThauNguonVon,
            lstGoiThauChiPhi: arrGoiThauChiPhi,
            lstGoiThauHangMuc: arrGoiThauHangMuc
        };


        $.ajax({
            type: "POST",
            url: "/KHLuaChonNhaThau/Luu",
            data: { data: data },
            success: function (r) {
                if (r.bIsComplete) {
                    window.location.href = "/QLVonDauTu/KHLuaChonNhaThau";
                }
                else {
                    alert("Tạo kế hoạch lựa chọn nhà thầu thất bại.");
                    return false;
                }
            }
        });
    }
}

function CheckLoi() {
    var messErr = [];
    if ($("#sSoQuyetDinh").val() == "") {
        messErr.push("Số quyết định chưa được nhập.");
    }

    if ($("#dNgayQuyetDinh").val() == "") {
        messErr.push("Ngày quyết định chưa được nhập.");
    }

    if ($("#iID_DonViQuanLyID").val() == GUID_EMPTY) {
        messErr.push("Đơn vị quản lý chưa được nhập.");
    }

    if ($("#iID_DuAnID").val() === GUID_EMPTY) {
        messErr.push("Dự án chưa được nhập.");
    }

    if (messErr.length > 0) {
        var Title = 'Lỗi lưu kế hoạch ';
        PopupModal("Lỗi", messErr.join("\r"), ERROR);
        return false;
    } else {
        return true;
    }
}

function CancelSaveData() {
    window.location.href = "/QLVonDauTu/KHLuaChonNhaThau/Index";
}

function ThemMoiGoiThau() {
    var arrDuToanID = $(".cb_DuToan:checked").map(function () {
        return $(this).data('dutoanid');
    });
    if (arrDuToanID.length > 0) {
        var iID_DuToanID = arrDuToanID[0];
        var iID_GoiThauID = uuidv4();
        var dongMoi = "";
        dongMoi += "<tr style='cursor: pointer;' class='parent' data-id='" + iID_GoiThauID + "'>";
        dongMoi += "<td class='r_STT'></td>";
        dongMoi += "<input type='hidden' class='r_iID_DuToanID' value='" + iID_DuToanID + "'/><input type='hidden' class='r_iID_GoiThauID' value='" + iID_GoiThauID + "'/>";
        dongMoi += "<td class='r_sTenGoiThau' contenteditable='true'></td>";
        dongMoi += "<td class='r_fGiaGoiThau sotien' align='right'></td>";
        dongMoi += "<td class='r_HinhThucLuaChonNhaThau'>" + CreateHtmlSelectHinhThucChonNhaThau() + "</td>";
        dongMoi += "<td class='r_PhuongThucChonNhaThau'>" + CreateHtmlSelectPhuongThucChonNhaThau() + "</td>";
        dongMoi += "<td class='r_LoaiHopDong'>" + CreateHtmlSelectLoaiHopDong() + "</td>";
        dongMoi += "<td class='r_iThoiGianThucHien sotien' contenteditable='true' align='right'></td>";
        dongMoi += "<td><button class='btn-delete btn-icon' type='button' onclick='XoaDong(this)'><span class='fa fa-trash-o fa-lg' aria-hidden='true'></span></button></td>";
        $("#" + TBL_DANH_SACH_GOI_THAU + " tbody").append(dongMoi);
        CapNhatCotStt(TBL_DANH_SACH_GOI_THAU);

        EventValidate();
        SuKienDbClickDongTable();
    } else {
        PopupModal("Lỗi", "Chưa chọn Dự toán", ERROR);
        return;
    }
}

// event
function EventValidate() {
    $("td.sotien[contenteditable='true']").on("keypress", function (event) {
        return ValidateNumberKeyPress(this, event);
    })
    $("td.sotien[contenteditable='true']").on("focusout", function (event) {
        $(this).html(FormatNumber($(this).html() == "" ? 0 : UnFormatNumber($(this).html())));
    })
}

function SuKienDbClickDongTable() {
    $("#" + TBL_DANH_SACH_GOI_THAU + " td").dblclick(function () {
        arrNguonVonModal = [];
        arrChiPhiModal = [];
        var $this = $(this);
        var row = $this.closest("tr");
        var rGoiThauID = row.find('.r_iID_GoiThauID').val();
        var rDuToanID = $("#iID_DuToanID").val();

        iID_GoiThauID_select = rGoiThauID;
        iID_DuToanID_select = rDuToanID;

        DisplayNguonVonChiPhiModal(new Array(rDuToanID), TBL_DANH_SACH_NGUON_VON_MODAL, TBL_DANH_SACH_CHI_PHI_MODAL);

        TinhTongNguonVon();
        TinhTongChiPhi();
        ShowTongConLai();

        EventValidate();
        $("#modalChiTietGoiThau").modal();
        $(".modal-content").scrollTop(0);
        EventModal();
        DinhDangSo("modalChiTietGoiThau");
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
    CapNhatCotStt(TBL_DANH_SACH_GOI_THAU);
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

var arr_HinhThucChonNhaThau = [];
var arr_PhuongThucChonNhaThau = [];
var arr_LoaiHopDong = [];
function LoadDataComboBoxNguonNganSach() {
    $.ajax({
        url: "/QLVonDauTu/KHLuaChonNhaThau/GetDataDropdown",
        type: "POST",
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data.hinhThucChonNT != null)
                arr_HinhThucChonNhaThau = data.hinhThucChonNT;
            if (data.loaiHopDong != null)
                arr_LoaiHopDong = data.loaiHopDong;
            if (data.phuongThucLuaChonNT != null)
                arr_PhuongThucChonNhaThau = data.phuongThucLuaChonNT;
        }
    });
}

function CreateHtmlSelectHinhThucChonNhaThau(value) {
    var htmlOption = "";
    arr_HinhThucChonNhaThau.forEach(x => {
        if (value != undefined && value == x.id)
            htmlOption += "<option value='" + x.id + "' selected>" + x.text + "</option>";
        else
            htmlOption += "<option value='" + x.id + "'>" + x.text + "</option>";
    })
    return "<select class='form-control'>" + htmlOption + "</option>";
}

function CreateHtmlSelectPhuongThucChonNhaThau(value) {
    var htmlOption = "";
    arr_PhuongThucChonNhaThau.forEach(x => {
        if (value != undefined && value == x.id)
            htmlOption += "<option value='" + x.id + "' selected>" + x.text + "</option>";
        else
            htmlOption += "<option value='" + x.id + "'>" + x.text + "</option>";
    })
    return "<select class='form-control'>" + htmlOption + "</option>";
}

function CreateHtmlSelectLoaiHopDong(value) {
    var htmlOption = "";
    arr_LoaiHopDong.forEach(x => {
        if (value != undefined && value == x.id)
            htmlOption += "<option value='" + x.id + "' selected>" + x.text + "</option>";
        else
            htmlOption += "<option value='" + x.id + "'>" + x.text + "</option>";
    })
    return "<select class='form-control'>" + htmlOption + "</option>";
}

function GenerateTreeTableKHLuaChonNhaThau(data, columns, button, idTable, haveSttColumn = true) {
    var sParentKey = '';
    var sKeyRefer = "";
    var sKey = ''
    var TableView = [];
    var sItemHiden = [];
    var iTotalCol = haveSttColumn == true ? 1 : 0;
    TableView.push("<table class='table table-bordered table-parent' style='margin-bottom: 0px'>");
    TableView.push("    <thead>");
    if (haveSttColumn == true)
        TableView.push("    <th width='3%'>STT</th>");
    $.each(columns, function (indexItem, value) {

        if (value.bKey) {
            sKey = value.sField;
        }

        if (value.bForeignKey) {
            sKeyRefer = value.sField;
        }

        if (value.bParentKey) {
            sParentKey = value.sField;
        }

        if (value.sTitle != null && value.sTitle != undefined && (value.sTitle != '' || value.sType == "checkbox")) {
            iTotalCol++;
            if (value.sType == "checkbox")
                TableView.push("    <th width='" + value.iWidth + "'><input type='checkbox' name='cbAll' class='cbAll'/></th>");
            else
                TableView.push("    <th width='" + value.iWidth + "'>" + value.sTitle + "</th>");
        } else {
            sItemHiden.push(value.sField);
        }
    });
    if (sKeyRefer == "") {
        sKeyRefer = sKey;
    }
    if (button.bUpdate == 1 || button.bDelete == 1 || button.bInfo == 1) {
        TableView.push("<th width='300px'></th>");
        iTotalCol++;
    }

    TableView.push("    </thead>");
    TableView.push("    <tbody>");

    if (data == undefined || data == null || data == []) {
        return TableView.join(" ");
    }

    var objCheck = SoftData(data, sKey, sKeyRefer, sParentKey);
    var index = 0;
    var sSpace = "";
    $.each(objCheck.result, function (indexItem, value) {
        var itemChild = $.map(data, function (n) { return n[sKey] == value ? n : null })[0];
        var dataRef = RecursiveTableKHLuaChonNhaThau(iTotalCol, index, itemChild, objCheck.parentData, columns, sItemHiden, button, sKey, sKeyRefer, data, sSpace, idTable, haveSttColumn)

        TableView.push(dataRef.sView);
        index = dataRef.index;
    });

    TableView.push("    </tbody>");
    TableView.push("</table>");
    return TableView.join(' ');
}

function RecursiveTableKHLuaChonNhaThau(iTotalCol, index, item, dicTree, columns, sItemHiden, button, sKey, sKeyRefer, data, sSpace, idTable, haveSttColumn = true) {
    index++;
    var TableView = [];
    var isFirst = true;
    var lstHiddenData = [];
    $.each(sItemHiden, function (indexItem, value) {
        if (item[value] != undefined) {
            lstHiddenData.push("data-" + value + " = '" + item[value] + "'");
        }
    });
    TableView.push("<tr data-row='" + item[sKey] + "' " + lstHiddenData.join(" ") + ">");
    if (haveSttColumn)
        TableView.push("<td align='center' style='width:3%'>" + index + "</td>");
    $.each(columns, function (indexItem, value) {

        if (value.sTitle != null && value.sTitle != undefined && (value.sTitle != '' || (value.sType == 'checkbox'))) {
            TableView.push("<td align='" + value.sTextAlign + "' style='width:" + value.iWidth + "' class='" + (value.sClass == undefined ? "" : value.sClass) + " r_" + value.sField + "' " + (value.bEditable == 1 ? "contenteditable='true'" : "") + ">")
            // lay dong dau tien de hien thi icon va collape
            if (value.bHaveIcon == 1) {
                // neu co nhanh con , thi hien icon folder , neu khong co thi hien icon text
                if (dicTree[item[sKeyRefer]] != null) {
                    if (idTable != null && idTable != undefined) {
                        TableView.push(sSpace + "<i class='fa fa-caret-down' aria-hidden='true' data-toggle='collapse' data-target='#item-" + item[sKeyRefer] + "_" + idTable + "' aria-expanded='false' aria-controls='#item-" + item[sKeyRefer] + "_" + idTable + "'></i>")
                    } else {
                        TableView.push(sSpace + "<i class='fa fa-caret-down' aria-hidden='true' data-toggle='collapse' data-target='#item-" + item[sKeyRefer] + "' aria-expanded='false' aria-controls='#item-" + item[sKeyRefer] + "'></i>")
                    }

                    TableView.push("<i class='fa fa-folder-open' style='color:#ffc907'></i>")
                } else {
                    TableView.push(sSpace + "&nbsp;&nbsp;<i class='fa fa-file-text' style='color:#ffc907'></i>")
                }
                isFirst = false;
            }
            var itemText = [];
            if (value.sType == 'checkbox')
                itemText.push("<input type='checkbox' class='cb_tbody' data-id='" + item[sKeyRefer] + "' " + (item[value.sField] == 1 ? "checked" : "") + " />");
            else if (value.sField != "" && value.sField != undefined && value.sField != "")
                $.each(value.sField.split('-'), function (indexField, valueField) {
                    if (value.bEditable == 1 && item[valueField] == 0)
                        itemText.push("");
                    else
                        itemText.push(item[valueField]);
                });
            if (value.bHaveIcon != 1 && value.iMain == 1)
                TableView.push(sSpace + itemText.join(" - "));
            else
                TableView.push(itemText.join(" - "));
            TableView.push("</td>");
        }
    });
    TableView.push(CreateButtonTable(item, sKey, sKeyRefer, button));
    TableView.push("</tr>");

    if (dicTree[item[sKeyRefer]] != null) {
        TableView.push("<tr>");
        if (idTable != null && idTable != undefined) {
            TableView.push("    <td colspan='" + iTotalCol + "' class='table-child collapse in' aria-expanded='true' style='padding:0px;' id='item-" + item[sKeyRefer] + "_" + idTable + "'>");
        } else {
            TableView.push("    <td colspan='" + iTotalCol + "' class='table-child collapse in' aria-expanded='true' style='padding:0px;' id='item-" + item[sKeyRefer] + "'>");
        }

        TableView.push("        <table class='table table-bordered'>");
        sSpace = sSpace + "&nbsp;&nbsp&nbsp;&nbsp"
        $.each(dicTree[item[sKeyRefer]], function (indexItem, value) {
            var itemChild = $.map(data, function (n) { return n[sKey] == value ? n : null })[0];
            var dataRef = RecursiveTableKHLuaChonNhaThau(iTotalCol, index, itemChild, dicTree, columns, sItemHiden, button, sKey, sKeyRefer, data, sSpace, idTable, haveSttColumn)

            TableView.push(dataRef.sView)
            index = dataRef.index;
        });

        TableView.push("        </table>");
        TableView.push("    </td>");
        TableView.push("</tr>");
    }
    return { sView: TableView.join(" "), index: index };
}

function CreateButtonTable(item, sKey, sKeyRefer, button) {

    var lstKey = [];
    lstKey.push("'" + item[sKey] + "'");
    if (button.bAddReferKey) {
        lstKey.push("'" + item[sKeyRefer] + "'");
    }
    var sParam = lstKey.join(",");


    var TableView = [];
    if (button.bUpdate == 1 || button.bDelete == 1 || button.bInfo == 1) {
        TableView.push("<td align='center' class='col-sm-12 col-btn' style='width:300px'>");
        if (button.bInfo == 1) {
            TableView.push("<button type='button' class='btn-detail' onclick=\"ViewDetail(" + sParam + ")\"><i class='fa fa-eye fa-lg' aria-hidden='true'></i></button>");
        }
        if (button.bUpdate == 1) {
            TableView.push("<button type='button' class='btn-edit' onclick=\"GetItemData(" + sParam + ")\"><i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i></button>");
        }
        if (button.bDelete == 1) {
            TableView.push("<button type='button' class='btn-delete' onclick=\"DeleteItem(" + sParam + ")\"><i class='fa fa-trash-o fa-lg' aria-hidden='true'></i></button>");
        }
        TableView.push("</td>")
    }
    return TableView.join(' ');
}

function SoftData(data, sKey, sKeyRefer, sParent) {
    var result = [];
    var parentData = [];
    $.each(data, function (indexItem, value) {

        var itemCheck = $.map(data, function (n) { return n[sParent] == value[sKeyRefer] ? n : null })[0];

        if (itemCheck != null && value[sKeyRefer] != null) {
            if (parentData[value[sKeyRefer]] == undefined) {
                parentData[value[sKeyRefer]] = [];
            }
        }

        if (value[sParent] == null) {
            result.push(value[sKey])
            return true;
        } else {
            if (parentData[value[sParent]] == undefined) {
                parentData[value[sParent]] = [];
            }
            parentData[value[sParent]].push(value[sKey]);
        }
    });
    var objResult = { parentData: parentData, result: result };
    return objResult;
}

function DinhDangSo(id) {
    if (typeof (id) == 'undefined') {
        $(".sotien").each(function () {
            if ($(this).is('input'))
                $(this).val(FormatNumber($(this).val().trim()) == "" ? 0 : FormatNumber($(this).val().trim()));
            else {
                if (this.hasAttribute("contenteditable"))
                    $(this).html(FormatNumber($(this).html().trim()));
                else $(this).html(FormatNumber($(this).html().trim()) == "" ? 0 : FormatNumber($(this).html().trim()));
            }

        })
    } else {
        $("#" + id + " .sotien").each(function () {
            if ($(this).is('input'))
                $(this).val(FormatNumber($(this).val().trim()) == "" ? 0 : FormatNumber($(this).val().trim()));
            else {
                if (this.hasAttribute("contenteditable"))
                    $(this).html(FormatNumber($(this).html().trim()));
                else
                    $(this).html(FormatNumber($(this).html().trim()) == "" ? 0 : FormatNumber($(this).html().trim()));
            }
        })
    }
}

// validate luu chi tiet goi thau
function EventValidateLuuChiTietGoiThau() {
    var messErr = [];
    if (arrGoiThauChiPhi.length == 0) {
        messErr.push("Chưa chọn chi phí.");
    }

    if (arrGoiThauNguonVon.length == 0) {
        messErr.push("Chưa chọn nguồn vốn.");
    }

    if (arrGoiThauChiPhi.length > 0 && arrGoiThauNguonVon.length > 0) {
        if (fTongNguonVon != fTongChiPhi)
            messErr.push("Tổng chi phí không bằng tổng nguồn vốn.");
    }

    if (messErr.length > 0) {
        alert(messErr.join("\r"));
        return false;
    } else {
        return true;
    }
}

function LuuChiTietGoiThau() {
    // goi thau nguon von
    arrGoiThauNguonVon = arrGoiThauNguonVon.filter(function (x) { return x.iID_GoiThauID != iID_GoiThauID_select });
    $("#" + TBL_DANH_SACH_NGUON_VON_MODAL + " input[type='checkbox']:checked").each(function () {
        var thisDong = this.parentElement.parentElement;
        var iID_NguonVonID = $(this).data("nguonvonid");
        var fTienGoiThau = $(thisDong).find(".r_fGiaGoiThau").html().trim();

        arrGoiThauNguonVon.push({
            iID_GoiThauID: iID_GoiThauID_select,
            fTienGoiThau: parseInt(fTienGoiThau == "" ? 0 : UnFormatNumber(fTienGoiThau)),
            iID_NguonVonID: iID_NguonVonID,
            iID_DuToanID: iID_DuToanID_select
        })
    })

    // goi thau chi phi
    arrGoiThauChiPhi = arrGoiThauChiPhi.filter(function (x) { return x.iID_GoiThauID != iID_GoiThauID_select });
    $("#" + TBL_DANH_SACH_CHI_PHI_MODAL + " input[type='checkbox']:checked").each(function () {
        var thisDong = this.parentElement.parentElement;
        var iID_ChiPhiID = $(this).data("id");
        var fTienGoiThau = $(thisDong).find(".r_fGiaTriGoiThau").html().trim();

        arrGoiThauChiPhi.push({
            iID_GoiThauID: iID_GoiThauID_select,
            fTienGoiThau: parseInt(fTienGoiThau == "" ? 0 : UnFormatNumber(fTienGoiThau)),
            iID_ChiPhiID: iID_ChiPhiID,
            iID_DuToanID: iID_DuToanID_select
        })
    })

    if (EventValidateLuuChiTietGoiThau()) {
        $("#modalChiTietGoiThau").modal("hide");

        $("#" + TBL_DANH_SACH_GOI_THAU + " tr[data-id='" + iID_GoiThauID_select + "']").find(".r_fGiaGoiThau").html(FormatNumber(fTongNguonVon));
    }
}

// event close modal
$('#modalChiTietGoiThau').on('hidden.bs.modal', function (e) {
    $("#" + TBL_DANH_SACH_NGUON_VON_MODAL + " tbody").html("");
    $("#" + TBL_DANH_SACH_CHI_PHI_MODAL + " tbody").html("");
    $("#" + TBL_DANH_SACH_HANG_MUC_MODAL + " tbody").html("");

    fTongChiPhi = 0;
    fTongNguonVon = 0;
})