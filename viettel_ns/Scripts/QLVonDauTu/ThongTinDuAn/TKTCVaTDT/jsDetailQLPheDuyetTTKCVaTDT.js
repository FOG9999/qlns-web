var guidEmpty = "00000000-0000-0000-0000-000000000000";
var objDuToan = {};
var lstDuAn = [];
var lstChiPhi = [];
var lstHangMuc = [];
var strCbxNguonVon = "";
var bIsDieuChinh = false;
var iIndexNguonVon = -1;
var iIndexHangMuc = -1;
var fSumNguonVon = 0, fSumChiPhi = 0;

$(document).ready(function () {
    bIsDieuChinh = $("#bIsDieuChinh").val();

    GetNguonVon();
    GetChiPhi();
    GetHangMuc();

    $("#tonggiatriconlainguonvon").html(FormatNumber(fSumNguonVon - fSumChiPhi));
});

function GetNguonVon() {
    var lstNguonVon = [];
    var iIdDuAnId = $("#iIdDuAnId").val();
    var iIdDuToanId = $("#iIdDuToanId").val();
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/GetNguonVonByDuToan",
        type: "GET",
        dataType: "json",
        async: false,
        data: { iIdDuAnId: iIdDuAnId, iIdDuToan: iIdDuToanId, bIsDieuChinh: bIsDieuChinh },
        success: function (result) {
            lstNguonVon = result.data;
            RenderNguonVon(lstNguonVon);
        }
    });
}

function GetChiPhi() {
    var lstChiPhi = [];
    var iIdDuAnId = $("#iIdDuAnId").val();
    var iIdDuToanId = $("#iIdDuToanId").val();
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/GetChiPhiByDuToan",
        type: "GET",
        dataType: "json",
        async: false,
        data: { iIdDuAnId: iIdDuAnId, iIdDuToan: iIdDuToanId, bIsDieuChinh: bIsDieuChinh },
        success: function (result) {
            lstChiPhi = result.data;
            RenderChiPhi(lstChiPhi);
        }
    });
}

function GetHangMuc() {
    lstHangMuc = [];
    var iIdDuAnId = $("#iIdDuAnId").val();
    var iIdDuToanId = $("#iIdDuToanId").val();
    $.ajax({
        url: "/QLVonDauTu/QLPheDuyetTKTCVaTDT/GetHangMucByDuToan",
        type: "GET",
        dataType: "json",
        data: { iIdDuAnId: iIdDuAnId, iIdDuToan: iIdDuToanId, bIsDieuChinh: bIsDieuChinh },
        success: function (result) {
            lstHangMuc = result.data;
        }
    });
}

function RenderNguonVon(lstNguonVon) {
    var lstStrNguonVon = [];
    var fTongDauTuTKTC = 0;
    $.each(lstNguonVon, function (index, item) {
        iIndexNguonVon++;
        lstStrNguonVon.push("<tr data-fTienPheDuyetQDDT='" + item.fTienPheDuyetQDDT + "'>");
        lstStrNguonVon.push("<td class='width-50'>" + (index + 1) + "</td>");
        lstStrNguonVon.push("<td class='iID_NguonVonID'>" + item.sTenNguonVon + "</td>");
        lstStrNguonVon.push("<td style='text-align:right'>" + FormatNumber(item.fTienPheDuyetQDDT) + "</td>");
        if (bIsDieuChinh == "True") {
            lstStrNguonVon.push("<td class='fGiaTriDieuChinh' style='text-align:right'>" + FormatNumber(item.fGiaTriDieuChinh) + "</td>");
        }
        lstStrNguonVon.push("<td class='fTienPheDuyet' style='text-align:right'>" + FormatNumber(item.fTienPheDuyet) + "</td>");
        /*lstStrNguonVon.push("<td class='width-100 text-center'></td>");*/
        lstStrNguonVon.push("</tr>");
        fTongDauTuTKTC += item.fTienPheDuyet;
        $("#fTongMucDauTu").val(FormatNumber(fTongDauTuTKTC));
    });
    $("#grdNguonVon").html(lstStrNguonVon.join(""));
    fSumNguonVon = fTongDauTuTKTC;
}

function RenderChiPhi(lstChiPhi) {
    var lstStrChiPhi = [];
    $.each(lstChiPhi, function (index, item) {
        lstStrChiPhi.push("<tr data-id='" + item.iID_DuAn_ChiPhi + "' data-parent='" + item.iID_ChiPhi_Parent + "' data-fTienPheDuyetQDDT='" + item.fTienPheDuyetQDDT + "' data-iIDChiPhiID='" + item.iID_ChiPhiID + "'>");
        lstStrChiPhi.push("<td>" + item.sTenChiPhi + "</td>");
        lstStrChiPhi.push("<td style='text-align:right'>" + FormatNumber(item.fTienPheDuyetQDDT) + "</td>");
        if (bIsDieuChinh == "True") {
            lstStrChiPhi.push("<td class='fGiaTriDieuChinh' style='text-align:right'>" + FormatNumber(item.fGiaTriDieuChinh) + "</td>");
        }
        lstStrChiPhi.push("<td style='text-align:right'>" + FormatNumber(item.fTienPheDuyet) + "</td>");

        lstStrChiPhi.push("<td class='width-200 text-center'>");
        if (!item.isParent) {
            lstStrChiPhi.push("<button type='button' class='btn-edit' onclick='ViewHangMuc(\"" + item.iID_DuAn_ChiPhi + "\")'><i class='fa fa-eye fa-lg' aria-hidden='true'></i></button>")
        }
        lstStrChiPhi.push("</td>");
        lstStrChiPhi.push("</tr>");
        fSumChiPhi += item.fTienPheDuyet;
    });
    $("#grdChiPhi").html(lstStrChiPhi.join(""));
}

function RenderHangMuc(lstHangMucByChiPhi) {
    var lstStrHangMuc = [];
    var fGiaTriPheDuyetDuAn = 0;
    var fGiaTriPheDuyetTKTC = 0;
    var fGiaTriChenhLech = 0;
    var tongGTPD = 0, tongGTPDTktc =0, tongChenhLech = 0;

    $.each(lstHangMucByChiPhi, function (index, item) {
        lstStrHangMuc.push("<tr data-id='" + item.iID_HangMucID + "' data-parent='" + item.iID_ParentID + "' data-fTienPheDuyetQDDT='" + item.fTienPheDuyetQDDT + "' data-iIDLoaiCongTrinhID='" + item.iID_LoaiCongTrinhID + "' data-iIdHangMucPhanChia='" + item.iID_HangMucPhanChia + "' data-bIsParent='" + item.isParent + "'>");
        lstStrHangMuc.push("<td class='width-50 smaOrder'>" + item.smaOrder + "</td>")
        lstStrHangMuc.push("<td>" + item.sTenHangMuc + "</td>");
        lstStrHangMuc.push("<td class='sLoaiCongTrinh'>" + (item.sTenLoaiCongTrinh == null ? "" : item.sTenLoaiCongTrinh) + "</td>")
        lstStrHangMuc.push("<td style='text-align:right'>" + FormatNumber(item.fTienPheDuyetQDDT) + "</td>");
        if (bIsDieuChinh == "True") {
            lstStrHangMuc.push("<td class='fGiaTriDieuChinh' style='text-align:right'>" + FormatNumber(item.fGiaTriDieuChinh) + "</td>");
        }
        lstStrHangMuc.push("<td style='text-align:right'>" + FormatNumber(item.fTienPheDuyet) + "</td>");

        if (item.fTienPheDuyet != null && item.fTienPheDuyet != 0) {
            var fChenhLech = item.fTienPheDuyet - item.fTienPheDuyetQDDT;
            fGiaTriChenhLech = fGiaTriChenhLech + fChenhLech;
            lstStrHangMuc.push("<td class='fChenhLech' style='text-align:right'>" + FormatNumber(fChenhLech) + "</td>");
            tongChenhLech += parseFloat(fChenhLech);
        } else {
            lstStrHangMuc.push("<td class='fChenhLech' style='text-align:right'></td>");
        }

        lstStrHangMuc.push("<td class='width-100 text-center'>");
        lstStrHangMuc.push("</td>");
        lstStrHangMuc.push("</tr>");
        if (item.iID_ParentID == null) {
            fGiaTriPheDuyetDuAn = fGiaTriPheDuyetDuAn + item.fTienPheDuyetQDDT;
            fGiaTriPheDuyetTKTC = fGiaTriPheDuyetTKTC + item.fTienPheDuyet;
        }
        tongGTPD += (item.fTienPheDuyetQDDT == null || item.fTienPheDuyetQDDT == "") ? 0 : parseFloat(item.fTienPheDuyetQDDT);
        tongGTPDTktc += (item.fTienPheDuyet == null || item.fTienPheDuyet == "") ? 0 : parseFloat(item.fTienPheDuyet);
        
    });
    $("#tblHangMucChinh").html(lstStrHangMuc.join(""));

    /* In đậm với các  root cha*/
    $("#tblHangMucChinh tr").each(function (tr) {
        if ($(this).data('parent') == null) {
            $(this).find('td').css('font-weight', 'bold');
        }
    })
    fGiaTriChenhLech = fGiaTriPheDuyetTKTC - fGiaTriPheDuyetDuAn;
    $(".cpdt_tong_giatripheduyetduan").html('<b>' + FormatNumber(fGiaTriPheDuyetDuAn) +'</b>');
    $(".cpdt_tong_giatripheduyet").html('<b>' + FormatNumber(fGiaTriPheDuyetTKTC) +'</b>');
    if (fGiaTriChenhLech > 0) {
        $(".cpdt_tong_giatrichenhlech").html('<b>' + FormatNumber(fGiaTriChenhLech) +'</b>');
    }
    $("#tblHangMucChinh").html(lstStrHangMuc.join(""));
    $("#tblHangMucCSS .fTongGiaTriPD").html(FormatNumber(tongGTPD));
    $("#tblHangMucCSS .fTongGiaTriPDTKTC").html(FormatNumber(tongGTPDTktc));
    $("#tblHangMucCSS .fTongChenhLech").html(FormatNumber(tongChenhLech));
}

function GetTongMucDauTuTKTC() {
    var fSum = 0;
    $("#fTongMucDauTuTKTC").val(fSum);
    $.each($("#grdNguonVon tr").find(".fTienPheDuyet"), function (index, item) {
        var sItem = $(item).val().replaceAll(".", "");
        if (sItem == "") return false;
        fSum += parseFloat(sItem);
        $("#fTongMucDauTuTKTC").val(FormatNumber(fSum));
    })
}

function ViewHangMuc(iIdChiPhi) {
    var sTenChiPhi = $($("#grdChiPhi").find("[data-id='" + iIdChiPhi + "'] .sTenChiPhi")[0]).val();
    $("#txtTenChiPhi").text(sTenChiPhi);
    $("#txtIdChiPhiHangMuc").val(iIdChiPhi);
    RenderHangMuc($.map(lstHangMuc, function (n) { return n.iID_DuAn_ChiPhi == iIdChiPhi ? n : null }));
    $("#modal-listdetailchiphi").modal("show");
}

function HideModalChiTietChiPhi() {
    $("#modal-listdetailchiphi").modal("hide");
}

function RecusiveIndexHangMucByParent(iIdParentId) {
    var itemLast = $("#tblHangMucChinh").find("[data-parent='" + iIdParentId + "']").last();
    var iId = $(itemLast).data('id');
    var currentIndex = $(itemLast).index();
    var childLength = $("#tblHangMucChinh").find("[data-parent='" + iId + "']").length;
    if (childLength == 0) return currentIndex;
    return RecusiveIndexHangMucByParent(iIdParentId);
}

function RecusiveIndexChiPhiByParent(iIdParentId) {
    var itemLast = $("#grdChiPhi").find("[data-parent='" + iIdParentId + "']").last();
    var iId = $(itemLast).data('id');
    var currentIndex = $(itemLast).index();
    var childLength = $("#grdChiPhi").find("[data-parent='" + iId + "']").length;
    if (childLength == 0) return currentIndex;
    return RecusiveIndexChiPhiByParent(iIdParentId);
}

function ResumChiPhi(id) {
    ResumGrid("grdChiPhi", id);
}

function TriggerChangeGiaTriChiPhi() {
    $("#grdChiPhi .fTienPheDuyet").on("change", function () {
        ResumChiPhi($(this).closest("tr").data("parent"));
    });
}

function ResumHangMuc(id) {
    ResumGrid("tblHangMucChinh", id);
}

function TriggerChangeGiaTriHangMuc() {
    $("#tblHangMucChinh .fTienPheDuyet").on("change", function () {
        ResumHangMuc($(this).closest("tr").data("parent"));
        SetChenhLechHangMuc($(this).closest("tr"))
    });
}

function SetChenhLechHangMuc(row) {
    var fTienPheDuyet = 0;
    var sTienPheDuyet = $(row).find(".fTienPheDuyet").val();
    if (sTienPheDuyet != undefined && sTienPheDuyet != null && sTienPheDuyet != '') {
        fTienPheDuyet = parseFloat(sTienPheDuyet.toString().replaceAll(".", ""));
    }

    var fGiaTriQDDT = 0
    var sGiaTriQDDT = $(row).data("ftienpheduyetqddt");
    if (sGiaTriQDDT != undefined && sGiaTriQDDT != null && sGiaTriQDDT != '') {
        fGiaTriQDDT = parseFloat(sGiaTriQDDT);
    }

    $(row).find("fChenhLech").text(FormatNumber(fTienPheDuyet - fGiaTriQDDT));
}

function ResumGrid(iIdGrid, id) {
    if (id == undefined || id == null || id == "") return;
    var fSum = 0;
    $.each($("#" + iIdGrid + " [data-parent='" + id + "']").closest("tr"), function (index, item) {
        var sGiaTri = $(item).find(".fTienPheDuyet").val();
        if (sGiaTri != null && sGiaTri != "") {
            fSum += parseFloat(sGiaTri.replaceAll(".", ""));
        }
    });
    $("#" + iIdGrid + " [data-id='" + id + "'] .fTienPheDuyet").val(FormatNumber(fSum));
    var parentId = $("#" + iIdGrid + " [data-id='" + id + "']").data("parent");
    ResumGrid(iIdGrid, parentId);
}

function Cancel() {
    window.location.href = "/QLVonDauTu/QLPheDuyetTKTCVaTDT/Index";
}