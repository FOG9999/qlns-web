var guidEmpty = "00000000-0000-0000-0000-000000000000";
var bIsDieuChinh = false;
var sCbxPhuongThuc = "";
var sCbxHinhThuc = "";
var sCbxLoaiHD = "";
var iTypeDuToan = $("#iTypeDuToan").val();
var lstChungTuAll = [];
var itemsChungTu = [];
var itemsChungTuGoiThau = [[], [], []];
var lstChungTuGoiThau = []; // sử dụng để update cho tất cả các gói thầu, biến dùng chung

var hrefSplit = window.location.href.split('/');
var iIdKHLuaChonNhaThau = getUrlParameter('id') || hrefSplit[hrefSplit.length - 1];
var arrGoiThau = [];
var arrGoiThauNguonVon = [];
var arrGoiThauChiPhi = [];
var arrGoiThauHangMuc = [];
var arrDuAnChiPhiGoiThau = [];

var isSettingUpData = true;


$(document).ready(function () {
    if ($("#bIsDieuChinh").val() == 1)
        bIsDieuChinh = true;
    GetCbxDonVi();
    GetCbxGoiThau();
    SetupItem();
    GetChungTuDetailByKhlcnt();
    GetAllGoiThauByKhlcntId();
    ReloadPhuongThucLuaChon();

    $("#cbxChuDauTu").select2('destroy');
});

$("#cbxLoaiChungTu").change(() => {
    lstChungTuAll = [];
    itemsChungTu = [];
    itemsChungTuGoiThau = [[], [], []];
    lstChungTuGoiThau = []; // sử dụng để update cho tất cả các gói thầu, biến dùng chung

    arrGoiThau = [];
    arrGoiThauNguonVon = [];
    arrGoiThauChiPhi = [];
    arrGoiThauHangMuc = [];
    arrDuAnChiPhiGoiThau = [];

    $('#iID_DonViQuanLyID').val('').trigger('change');
    $("#cbxChuDauTu").val('').trigger('change');

    

    GetListChungTu();
})



function GetDanhSachGoiThau() {
    $.ajax({
        url: "/QLVonDauTu/KHLuaChonNhaThau/LayThongTinGoiThauChiTietTheoKHLuaChonNhaThau",
        type: "POST",
        data: { id: iIdKHLuaChonNhaThau },
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
                    arrDuAnChiPhiGoiThau = data.lstDuAnChiPhis;
                    // merge arrDuAnChiPhiGoiThau to arrGoiThauChiPhi
                    arrGoiThauChiPhi = arrGoiThauChiPhi.map(item => {
                        var chiPhiDuAn =
                            arrDuAnChiPhiGoiThau.filter(ele => ele.iID_DuAn_ChiPhi === item.iID_ChiPhiID)[0];
                        if (chiPhiDuAn) {
                            return {
                                ...item,
                                ...chiPhiDuAn
                            }
                        } else return item;
                    })
                    console.log(arrGoiThauChiPhi)
                }
            }
            arrGoiThau.forEach(gt => {
                lstChungTuGoiThau[gt.iID_GoiThauID] = [[], [], []];
                GetNguonVonGoiThauDetail(gt.iID_GoiThauID);
                GetChiPhiGoiThauDetail(gt.iID_GoiThauID);
                // calculate sum nguồn vốn
                SetLstNguonVon();
                GetTongChiTiet();
                // update lại array chung cho tất cả gói thầu
                lstChungTuGoiThau[gt.iID_GoiThauID] = [...itemsChungTuGoiThau];
            })
            arrGoiThauNguonVon = [];
            arrGoiThauChiPhi = [];
            arrGoiThauHangMuc = [];

            isSettingUpData = false;

            // call function to apply common validators
            CallFunctionCommon();
        },
        error: function (data) {
            //GetNguonVonGoiThauDetail();
            //GetChiPhiGoiThauDetail();

            // call function to apply common validators
            CallFunctionCommon();
        }
    });
}

function isGuid(value) {
    var regex = /[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}/i;
    var match = regex.exec(value);
    return match != null;
}

function GetListChungTu() {
    var iIdDuAnId = $("#iID_DuAnID").val();
    var iLoaiChungTu = $("#cbxLoaiChungTu").val();
    $("#lstChungTu").empty();
    if (!isGuid(iIdDuAnId)) {
        $("#tblDanhSachNguonVon tbody").empty();
        $("#tblDanhSachChiPhi tbody").empty();
        return;
    }
    $.ajax({
        url: "/QLVonDauTu/KHLuaChonNhaThau/GetListChungTu",
        type: "GET",
        dataType: "html",
        data: { iIdDuAnId: iIdDuAnId, iLoaiChungTu: iLoaiChungTu, iId_KHLCNhaThau: iIdKHLuaChonNhaThau },
        success: function (result) {
            if (result != null && result != "") {
                $("#lstChungTu").html(result);
                $('input:radio[name="rd_ChungTu"]').change(function () {
                    var iIdChungTu = $(this).val();
                    ConvertItemsChungTu($.map(lstChungTuAll, function (n) { return n.iID_ChungTu == iIdChungTu ? n : null }));
                });

                if (iLoaiChungTu == 1) {
                    $("#iIdDuToanId").val($("input[name=rd_ChungTu]:checked").val());  
                } else if (iLoaiChungTu == 2) {
                    $("#iIdQDDauTuId").val($("input[name=rd_ChungTu]:checked").val());
                } else {
                    $("#iID_ChuTruongDauTuID").val($("input[name=rd_ChungTu]:checked").val());
                }
                var lstChungTu = [];
                $.each($("input[name=rd_ChungTu]:checked"), function (index, item) {
                    lstChungTu.push($(item).val());
                });
                GetAllChungTuDetail(lstChungTu);
            }
        }
    });
}

function GetAllChungTuDetail(lstChungTu) {
    var iLoaiChungTu = $("#cbxLoaiChungTu").val();
    $("#tblDanhSachNguonVon tbody").empty();
    $("#tblDanhSachChiPhi tbody").empty();
    var lstAllNguonVon = [];
    var lstAllChiPhi = [];
    lstChungTuAll = [];
    $.ajax({
        url: "/QLVonDauTu/KHLuaChonNhaThau/GetAllChungTuDetail",
        type: "POST",
        dataType: "json",
        data: { lstChungTu: lstChungTu, iLoaiChungTu: iLoaiChungTu },
        success: function (result) {
            if (result == null || result.data == null || result.data.length == 0) return;
            lstChungTuAll = result.data;
            // test
            var iIdChungTu = $('input:radio[name="rd_ChungTu"]:checked').val();
            ConvertItemsChungTu($.map(lstChungTuAll, function (n) { return n.iID_ChungTu == iIdChungTu ? n : null }));
            // end test
            result.data.forEach(function (item) {
                if (item.iID_NguonVonID != 0) {
                    if (lstAllNguonVon[item.iID_NguonVonID] == null)
                        lstAllNguonVon[item.iID_NguonVonID] = item;
                    else
                        lstAllNguonVon[item.iID_NguonVonID].fGiaTriPheDuyet += item.fGiaTriPheDuyet;
                }
                else if (item.iID_ChiPhiID != null && item.iID_HangMucID == null) {
                    if (lstAllChiPhi[item.iID_ChiPhiID] == null)
                        lstAllChiPhi[item.iID_ChiPhiID] = item;
                    else
                        lstAllChiPhi[item.iID_ChiPhiID].fGiaTriPheDuyet += item.fGiaTriPheDuyet;
                }
            });
            if (lstAllNguonVon.length != 0) RenderNguonVonAll("tblDanhSachNguonVon", lstAllNguonVon);

            sortChiPhi(lstAllChiPhi);
            if (lstAllChiPhi != null && lstAllChiPhi != []) RenderNguonVonAll("tblDanhSachChiPhi", lstAllChiPhi);
            GetDanhSachGoiThau();
            getTMDTPheDuyet();
        }
    });
}

function RenderNguonVonAll(iIdTable, Items) {
    var sItem = [];
    var isChiPhi = iIdTable == "tblDanhSachChiPhi";
    var count = 1;
    Object.keys(Items).forEach(function (key) {
        sItem.push("<tr>");
        sItem.push("<td>" + (isChiPhi ? (count + '. ') : '') + Items[key].sNoiDung + "</td>");
        sItem.push("<td class='text-right'>" + FormatNumber(Items[key].fGiaTriPheDuyet) + "</td>");
        sItem.push("</tr>");

        count++;
    });
    $("#" + iIdTable + " tbody").html(sItem.join(""));
}

function GetChungTuDetailByKhlcnt() {
    var id = $("#iIdKeHoachLuaChonNhaThau").val();
    if (id == "" || id == guidEmpty) return;
    $.ajax({
        url: "/QLVonDauTu/KHLuaChonNhaThau/GetChungTuDetailByKhlcntId",
        type: "GET",
        dataType: "json",
        data: { id: id },
        success: function (result) {
        }
    });
}

function GetAllGoiThauByKhlcntId() {
    var id = $("#iIdKeHoachLuaChonNhaThau").val();
    var isDieuChinh = $("#bIsDieuChinh").val() == 1 ? true : false;
    if (id == "" || id == guidEmpty) return;
    $.ajax({
        url: "/QLVonDauTu/KHLuaChonNhaThau/GetAllGoiThauByKhlcntId",
        type: "GET",
        dataType: "json",
        data: { id: id, bIsDieuChinh: bIsDieuChinh },
        success: function (result) {
            if (result.data == null) return;
            var sItem = []
            result.data.forEach(function (item) {
                if (isDieuChinh) {
                    var sDisable = '';
                    if (isExistGoiThau(item.iID_GoiThauID)) sDisable = 'disabled';
                    sItem.push("<tr data-id='" + item.iID_GoiThauID + "' data-parentid='" + item.iID_ParentID + "' ondblclick='ViewGoiThauDetail(\"" + item.iID_GoiThauID + "\")'>");
                    sItem.push("<td class='width-200'><input type='text'class='form-control sTenGoiThau' value='" + item.sTenGoiThau + "'></td>");
                    sItem.push(`<td class='text-right'> <input onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' type='text' class= 'form-control fGiaTri text-right' value ='${FormatNumber(item.fTienTrungThau)}' ${sDisable}></td>`);
                    //sItem.push("<td>" + sCbxHinhThuc + "</td>");
                    //sItem.push("<td>" + sCbxPhuongThuc + "</td>");
                    //sItem.push("<td '><input type='text' class='form-control sThoiGianTCLCNT' 'text-right' value='" + item.sThoiGianTCLCNT + "'></td>");
                    sItem.push("<td>" + sCbxLoaiHD + "</td>");
                    sItem.push("<td '><input type='text' class='form-control iThoiGianThucHien' 'text-right' value='" + item.iThoiGianThucHien + "'></td>");
                    //sItem.push("<td class='width-50 text-center'><button class='btn-detail'><i class='fa fa-eye fa-lg' aria-hidden='true' onclick=DetailGoiThau('" + item.iID_GoiThauID + "')></i></button><button class='btn-delete'><i class='fa fa-trash-o fa-lg' aria-hidden='true' onclick=DeleteGoiThau($(this))></i></button></td>");
                    sItem.push("<td class='width-50 text-center'><button class='btn-detail'><i class='fa fa-eye fa-lg' aria-hidden='true' onclick=ViewGoiThauDetail('" + item.iID_GoiThauID + "')></i></button><button class='btn-delete'><i class='fa fa-trash-o fa-lg' aria-hidden='true' onclick=DeleteGoiThau($(this))></i></button></td>");
                    sItem.push("</tr>");
                } else {
                    var sDisable = '';
                    if (isExistGoiThau(item.iID_GoiThauID)) sDisable = 'disabled';
                    sItem.push("<tr data-id='" + item.iID_GoiThauID + "' data-parentid='" + item.iID_ParentID + "' ondblclick='ViewGoiThauDetail(\"" + item.iID_GoiThauID + "\")'>");
                    sItem.push("<td class='width-200'><input type='text'class='form-control sTenGoiThau' value='" + item.sTenGoiThau + "'></td>");
                    sItem.push(`<td class='text-right'> <input onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' type='text' class= 'form-control fGiaTri text-right' value ='${FormatNumber(item.fTienTrungThau)}' ${sDisable}></td>`);
                    //sItem.push("<td>" + sCbxHinhThuc + "</td>");
                    //sItem.push("<td>" + sCbxPhuongThuc + "</td>");
                    //sItem.push("<td '><input type='text' class='form-control sThoiGianTCLCNT' 'text-right' value='" + item.sThoiGianTCLCNT + "'></td>");
                    sItem.push("<td>" + sCbxLoaiHD + "</td>");
                    sItem.push("<td '><input type='text' class='form-control iThoiGianThucHien' 'text-right' value='" + item.iThoiGianThucHien + "'></td>");
                    //sItem.push("<td class='width-50 text-center'><button class='btn-detail'><i class='fa fa-eye fa-lg' aria-hidden='true' onclick=DetailGoiThau('" + item.iID_GoiThauID + "')></i></button><button class='btn-delete'><i class='fa fa-trash-o fa-lg' aria-hidden='true' onclick=DeleteGoiThau($(this))></i></button></td>");
                    sItem.push("<td class='width-50 text-center'><button class='btn-detail'><i class='fa fa-eye fa-lg' aria-hidden='true' onclick=ViewGoiThauDetail('" + item.iID_GoiThauID + "')></i></button><button class='btn-delete'><i class='fa fa-trash-o fa-lg' aria-hidden='true' onclick=DeleteGoiThau($(this))></i></button></td>");
                    sItem.push("</tr>");
                }
            });
            $("#tblGoiThau").html(sItem.join(''));

            $.each($("#tblGoiThau tr"), function (index, item) {
                var goithauId = $(item).data("id");
                var itemGoiThau = $.map(result.data, function (n) { return n.iID_GoiThauID == goithauId ? n : null });
                if (itemGoiThau == null || itemGoiThau.length == 0) return false;
                //$(item).find(".sHinhThucChonNhaThau").val(itemGoiThau[0].sHinhThucChonNhaThau);
                //$(item).find(".sPhuongThucDauThau").val(itemGoiThau[0].sPhuongThucDauThau);
                $(item).find(".sHinhThucHopDong").val(itemGoiThau[0].sHinhThucHopDong);
            });

            // call function to apply common validators
            CallFunctionCommon();
        }
    });
}

function SetupItem() {
    var iIdKeHoachLuaChonNhaThau = $("#iIdKeHoachLuaChonNhaThau").val();
    if (iIdKeHoachLuaChonNhaThau != guidEmpty) {
        if (bIsDieuChinh)
            $("#cbxLoaiChungTu").attr('disabled', true);
        $("#iID_DonViQuanLyID").attr('disabled', true);
        $("#iID_DuAnID").attr('disabled', true);
    } else {
        $("#cbxLoaiChungTu").attr('disabled', false);
        $("#iID_DonViQuanLyID").attr('disabled', false);
        $("#iID_DuAnID").attr('disabled', false);
    }
}

function GetCbxDonVi() {
    $.ajax({
        url: "/QLVonDauTu/KHLuaChonNhaThau/GetCbxDonViQuanLy",
        type: "GET",
        dataType: "json",
        success: function (result) {
            if (result.data != null && result.data != "") {
                $("#iID_DonViQuanLyID").append(result.data);
            }
            var sMaDonVi = $("#iIdMaDonVi").val();
            if (sMaDonVi != null && sMaDonVi != '') {
                $("#iID_DonViQuanLyID").val(sMaDonVi);
                $("#iID_DonViQuanLyID").change();
            }
        }
    });
}

function GetCbxGoiThau() {
    sCbxPhuongThuc = "<select class='sPhuongThucDauThau form-control'></select>";
    sCbxHinhThuc = "<select class='sHinhThucChonNhaThau form-control'></select>";
    sCbxLoaiHD = "<select class='sHinhThucHopDong form-control'></select>";

    $.ajax({
        url: "/QLVonDauTu/KHLuaChonNhaThau/GetDataDropdown",
        type: "POST",
        async: false,
        dataType: "json",
        success: function (result) {
            if (result != null) {
                sCbxPhuongThuc = "<select class='sPhuongThucDauThau form-control'>";
                if (result.phuongThucLuaChonNT != null) {
                    result.phuongThucLuaChonNT.forEach(function (item) {
                        sCbxPhuongThuc += "<option value='" + item.Value + "'>" + item.Text + "</option>";
                    });
                }
                sCbxPhuongThuc += "</select>";

                sCbxHinhThuc = "<select class='sHinhThucChonNhaThau form-control'>";
                if (result.hinhThucChonNT != null) {
                    result.hinhThucChonNT.forEach(function (item) {
                        sCbxHinhThuc += "<option value='" + item.Value + "'>" + item.Text + "</option>";
                    });
                }
                sCbxHinhThuc += "</select>";

                sCbxLoaiHD = "<select class='sHinhThucHopDong form-control'>";
                if (result.loaiHopDong != null) {
                    result.loaiHopDong.forEach(function (item) {
                        sCbxLoaiHD += "<option value='" + item.Value + "'>" + item.Text + "</option>";
                    });
                }
                sCbxLoaiHD += "</select>";
            }
        }
    });
}

function getCanCu(iLoaiChungTu) {
    var title = iLoaiChungTu == 2 ? "Danh sách QĐĐT" : "Danh sách dự toán";
    var opShowGTPheDuyet;
    if (iLoaiChungTu == 1) {
        opShowGTPheDuyet = "GT phê duyệt TKTC";
    }
    else if (iLoaiChungTu == 2) {
        opShowGTPheDuyet = "GT phê duyệt PDDA";
    }
    else {
        opShowGTPheDuyet = "GT phê duyệt CTĐT";
    }
    $("#idShowTitle").html(title);
    $(".opShowGTPheDuyet").html(opShowGTPheDuyet);


}
getCanCu(iLoaiChungTu = $("#cbxLoaiChungTu").val());

function GetDuAn() {
    $("#tblGoiThau").html('');

    lstDuAn = [];
    $("#iID_DuAnID").html("<option value='' data-sMaDuAn='' data-fTongMucDauTu='0' data-iIDMaCDT=''>--Chọn--</option>");
    $("#fTongMucDauTuPheDuyet").val(0);
    var sDonVi = $("#iID_DonViQuanLyID").val();
    var iLoaiChungTu = $("#cbxLoaiChungTu").val();
    getCanCu(iLoaiChungTu);
    if (sDonVi == null || sDonVi == "" || iLoaiChungTu == null || iLoaiChungTu == "") return;

    $.ajax({
        url: "/QLVonDauTu/KHLuaChonNhaThau/FindDuAn",
        type: "GET",
        dataType: "json",
        data: { iID_MaDonViQuanLyID: sDonVi, iLoaiChungTu: iLoaiChungTu },
        success: function (result) {
            lstDuAn = result.data;
            if (lstDuAn != null || lstDuAn != []) {
                $.each(lstDuAn, function (index, item) {
                    $("#iID_DuAnID").append("<option value='" + item.iID_DuAnID + "' data-smaduan='" + item.sMaDuAn + "' data-ftongmucdautu='" + item.fTongMucDauTu + "' data-iidmacdt='" + item.iID_MaCDT + "'>" + item.sMaDuAn + ' - ' + item.sTenDuAn + "</option>");
                });
                var iIdDuAn = $("#iIdDuAnId").val();
                if (iIdDuAn != guidEmpty) {
                    $("#iID_DuAnID").val(iIdDuAn);
                } else {
                    $("#iID_DuAnID").val(null);
                }
                $("#iID_DuAnID").change();
            }

        }
    });
}

// lấy giá trị TMĐT phê duyệt theo dự toán được chọn 
function getTMDTPheDuyet() {
    // lấy theo dự toán được chọn     
    let fGiaTriPheDuyet = UnFormatNumber($("#lstChungTu").find('input:radio[name = "rd_ChungTu"]:checked').closest('tr').children().last().text());
    $("#fTongMucDauTuPheDuyet").val(FormatNumber(fGiaTriPheDuyet));
}

function ChooseDuAn() {
    $("#fTongMucDauTuPheDuyet").val(0);
    var iIdDuAn = $("#iID_DuAnID option:selected").val();
    if (iIdDuAn != undefined && iIdDuAn != '') {
        $("#cbxChuDauTu").val($("#iID_DuAnID").find(':selected').data("iidmacdt"));

        // đây là lấy theo thông tin dự án (ftongmucdautu)
        //$("#fTongMucDauTuPheDuyet").val(FormatNumber($("#iID_DuAnID").find(':selected').data("ftongmucdautu")));

        GetListChungTu();
    } else {
        $("#cbxChuDauTu").val('').trigger('change');
        GetListChungTu();
    }
}

function InsertGoiThau() {
    var sItem = []
    var iIdGoiThau = NewGuid();
    sItem.push("<tr data-id='" + iIdGoiThau + "' ondblclick='ViewGoiThauDetail(\"" + iIdGoiThau + "\")'>");
    sItem.push("<td class='width-200'><input type='text' class='form-control sTenGoiThau' value=''></td>");
    //sItem.push("<td  width-200'><input type='text' class='form-control' id='sTenGoiThau' value='" + item.sTenGoiThau + "'></td>");
    sItem.push("<td class='text-right'><input type='text' class='form-control fGiaTri text-right' value='' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' ></td>"); 
    //sItem.push("<td width=155.27px >" + sCbxHinhThuc + "</td>");
    //sItem.push("<td width=155.27px >" + sCbxPhuongThuc + "</td>");
    //sItem.push("<td width=155.27px><input type='text' class='form-control sThoiGianTCLCNT' value=''></td>");
    sItem.push("<td >" + sCbxLoaiHD + "</td>");
    sItem.push("<td ><input type='text' class='form-control iThoiGianThucHien' value=''></td>");
    //sItem.push("<td ><input type='text' class='form-control' id ='iThoiGianThucHien' value='" + item.iThoiGianThucHien + "'></td>");
    sItem.push("<td class='width-50 text-center'><button class='btn-delete'><i class='fa fa-trash-o fa-lg' aria-hidden='true' onclick='DeleteGoiThau($(this))'></i></button></td>");
    sItem.push("</tr>");
    $("#tblGoiThau").append(sItem.join(''));
    ReloadPhuongThucLuaChon();
}

function DeleteGoiThau(item) {
    $(item).closest("tr").addClass("error-row");
    $(item).css('display', 'none');
}

function ViewGoiThauDetail(iIdGoiThauId) {
    $("#iIdGoiThau").val(iIdGoiThauId);
    $("#modalChiTietGoiThau").modal("show");
    $("#tblHangMucChinh tbody").empty();
    // GetDanhSachGoiThau(iIdGoiThauId);

    if (!lstChungTuGoiThau[iIdGoiThauId]) {
        // update lại array chung cho tất cả gói thầu
        lstChungTuGoiThau[iIdGoiThauId] = [[], [], []];
    }

    GetNguonVonGoiThauDetail(iIdGoiThauId);
    GetChiPhiGoiThauDetail(iIdGoiThauId);
    // calculate sum nguồn vốn
    SetLstNguonVon();
    GetTongChiTiet();
}

// isOnBlur: check nếu đang trong sự kiện blur thì mình sẽ sử dụng mảng itemsChungTuGoiThau, vì mảng này là mảng được update
function getNguonVonForCurrentGoiThau(idGoiThau, nguonVon, isOnBlur) {
    let nguonVonConLaiForCurrentGoiThau = nguonVon.fGiaTriPheDuyet;
    for (let goiThau in lstChungTuGoiThau) {
        // trừ đi nguon von đã được sử dụng cho các gói thâu khác
        if (goiThau !== idGoiThau) {
            if (lstChungTuGoiThau[goiThau][0].length > 0) {
                lstChungTuGoiThau[goiThau][0].forEach(nv => {
                    if (nv.iID_NguonVonID === nguonVon.iID_NguonVonID)
                        nguonVonConLaiForCurrentGoiThau -= nv.fGiaTriGoiThau;
                });
            }
        }
        // trừ đi nguồn vốn đang được sử dụng cho gói thầu hiện tại
        else {
            if (isOnBlur) {
                itemsChungTuGoiThau[0].forEach(nv => {
                    if (nv.iID_NguonVonID === nguonVon.iID_NguonVonID)
                        nguonVonConLaiForCurrentGoiThau -= nv.fGiaTriGoiThau;
                });
            } else {
                lstChungTuGoiThau[goiThau][0].forEach(nv => {
                    if (nv.iID_NguonVonID === nguonVon.iID_NguonVonID)
                        nguonVonConLaiForCurrentGoiThau -= nv.fGiaTriGoiThau;
                });
            }
        }
    }
    return nguonVonConLaiForCurrentGoiThau;
}

function getChiPhiForCurrentGoiThau(idGoiThau, chiphi) {
    let chiPhiConLai = chiphi.fGiaTriPheDuyet;
    for (let goiThau in lstChungTuGoiThau) {
        // trừ đi chi phí đã chọn cho các gói thầu khác
        if (goiThau !== idGoiThau && lstChungTuGoiThau[goiThau][1].length > 0) {
            lstChungTuGoiThau[goiThau][1].forEach(cp => {
                if (cp.iID_ChiPhiID === chiphi.iID_ChiPhiID)
                    chiPhiConLai -= cp.fGiaTriGoiThau;
            });
        }
        // trừ đi chi phí đã chọn cho các gói thầu hiện tại
        if (goiThau === idGoiThau) {
            lstChungTuGoiThau[goiThau][1].forEach(cp => {
                if (cp.iID_ChiPhiID === chiphi.iID_ChiPhiID)
                    chiPhiConLai -= cp.fGiaTriGoiThau;
            });
        }
    }
    return chiPhiConLai;
}

function recalculateGiaTriNguonVonConLaiOnBlur(idGoiThau) {
    var lstNguonVon = $.map(itemsChungTu[0], function (n) { return n.iID_NguonVonID != 0 ? n : null });
    $("#tblDanhSachNguonVonModal .fGiaTriConLai").each((ind, item) => {
        $(item).text(FormatNumber(getNguonVonForCurrentGoiThau(idGoiThau, lstNguonVon[ind], true)));
    });
}

function validateGiaTriConLaiBeforeSave(currentGoiThau) {
    // validate nguon von
    var lstNguonVon = $.map(itemsChungTu[0], function (n) { return n.iID_NguonVonID != 0 ? n : null });
    var lstNguonVonConLai = [];
    lstNguonVon.forEach((item) => {
        lstNguonVonConLai.push(getNguonVonForCurrentGoiThau(currentGoiThau, item, true));
    })
    var numOfInValid = lstNguonVonConLai.filter(nv => nv < 0).length;
    if (numOfInValid > 0) {
        alert("Nguồn vốn còn lại không được nhỏ hơn 0")
        return false;
    }
    return true;
}

function GetNguonVonGoiThauDetail(iIdGoiThau) {
    var lstNguonVon = $.map(itemsChungTu[0], function (n) { return n.iID_NguonVonID != 0 ? n : null });
    var sItem = [], arrNguonVon, usingArrGoiThauNguonVon = true;
    // chỉ dùng arrGoiThauNguonVon 1 lần
    if (arrGoiThauNguonVon.length > 0) {
        arrNguonVon = [...arrGoiThauNguonVon.filter(nv => nv.iID_GoiThauID === iIdGoiThau)];
    }
    else {
        arrNguonVon = lstChungTuGoiThau[iIdGoiThau][0];
        usingArrGoiThauNguonVon = false;
    }
    lstNguonVon.forEach(function (item) {
        var indexInArrGoiThauNguonVon = arrNguonVon.map(ele => ele.iID_NguonVonID).indexOf(item.iID_NguonVonID);
        if (getNguonVonForCurrentGoiThau(iIdGoiThau, item) >= 0) {
            sItem.push("<tr data-id='" + item.iID_NguonVonID + "'>");
            sItem.push("<td class='width-50 text-center'><input type='checkbox' class='ck_NguonVon' value='" + item.iID_NguonVonID + (indexInArrGoiThauNguonVon >= 0 ? '\' checked ' : '\'') + "></td>");
            sItem.push("<td class='sNoiDung'>" + item.sNoiDung + "</td>");
            sItem.push("<td class='fGiaTriPheDuyet text-right'>" + FormatNumber(item.fGiaTriPheDuyet) + "</td>");
            sItem.push(`<td><input type=\'text\' ${indexInArrGoiThauNguonVon >= 0 ? "" : "disabled"} onkeyup=\'ValidateNumberKeyUp(this);\' onkeypress=\'return ValidateNumberKeyPress(this, event);\' class=\'fGiaTriGoiThau form-control\' style=\'text-align:right\' value=\'${indexInArrGoiThauNguonVon >= 0 ? (usingArrGoiThauNguonVon ? FormatNumber(arrNguonVon[indexInArrGoiThauNguonVon].fTienGoiThau) : FormatNumber(arrNguonVon[indexInArrGoiThauNguonVon].fGiaTriGoiThau)) : ""}\'/></td>`);
            sItem.push("<td class='fGiaTriConLai text-right'>" + FormatNumber(getNguonVonForCurrentGoiThau(iIdGoiThau, item)) + "</td>");
            sItem.push("</tr>");
        }
    });
    // neu update thi tinh tong luon
    if (arrNguonVon.length > 0) {
        SetLstNguonVon();
        GetTongChiTiet();
    }
    $("#tblDanhSachNguonVonModal tbody").html(sItem.join(""));

    $("#tblDanhSachNguonVonModal .ck_NguonVon").change(function () {
        var currentId = $(this).val();
        if (this.checked) {
            $(this).closest("tr").find(".fGiaTriGoiThau").prop("disabled", false);
            $(this).closest("tr").find(".fGiaTriGoiThau").val(FormatNumber(calculateNguonVonBySelectedChiPhi()));
        } else {
            $(this).closest("tr").find(".fGiaTriGoiThau").prop("disabled", true);
            $(this).closest("tr").find(".fGiaTriGoiThau").val('');
        }
        SetLstNguonVon();
        GetTongChiTiet();
    });

    $("#tblDanhSachNguonVonModal .fGiaTriGoiThau").blur(function () {
        SetLstNguonVon();
        GetTongChiTiet();
        recalculateGiaTriNguonVonConLaiOnBlur(iIdGoiThau)
    });
}

// tính toán tổng nguồn vốn dựa trên các chi phí đã chọn
function calculateNguonVonBySelectedChiPhi() {
    let sum = 0;
    $.each($('#tblDanhSachChiPhiModal tbody').find(".ck_ChiPhi:checkbox:checked"), (index, cp) => {
        sum += parseFloat($(cp).closest("tr").find(".fGiaTriGoiThau").val().replaceAll(".", ""));
    });
    return sum;
}

function SetLstNguonVon() {
    itemsChungTuGoiThau[0] = [];
    $.each($("#tblDanhSachNguonVonModal tbody").find(".ck_NguonVon:checkbox:checked"), function (index, child) {
        var obj = {};
        obj.iID_NguonVonID = $(child).closest("tr").data("id");
        obj.sNoiDung = $(child).closest("tr").find(".sNoiDung").text();
        obj.iThuTu = $(child).closest("tr").index();
        obj.fGiaTriPheDuyet = parseFloat($(child).closest("tr").find(".fGiaTriPheDuyet").text().replaceAll(".", ""));
        obj.fGiaTriGoiThau = parseFloat($(child).closest("tr").find(".fGiaTriGoiThau").val().replaceAll(".", ""));
        itemsChungTuGoiThau[0].push(obj);
    });
}

function GetChiPhiGoiThauDetail(iIdGoiThau) {
    var lstNguonVon = $.map(itemsChungTu[1], function (n) { return n.iID_NguonVonID == 0 && n.iID_HangMucID == null ? n : null });
    $("#tblDanhSachChiPhiModal tbody").empty();
    var sItem = [], arrChiPhi = [];
    // chỉ dùng arrGoiThauChiPhi một lần
    if (arrGoiThauChiPhi.length > 0) {
        arrChiPhi = [...arrGoiThauChiPhi.filter(nv => nv.iID_GoiThauID === iIdGoiThau)];
    } else {
        arrChiPhi = lstChungTuGoiThau[iIdGoiThau][1];
    }

    sortChiPhi(lstNguonVon);

    var countSTTChiPhi = 1;
    lstNguonVon.forEach(function (item) {
        // nếu chi phí này đã được check cho gói thầu khác và có giá trị còn lại <= 0 => ẩn đi
        // nếu chưa được check cho gói thâu nào thì cứ hiện
        let currGiaTriConLai = getChiPhiForCurrentGoiThau(iIdGoiThau, item);        
        var parentIndex = arrChiPhi.map(ele => {
            if (ele.iID_ChiPhi) return ele.iID_ChiPhi;
            else return ele.iID_ChiPhiID;
        }
        ).indexOf(item.iID_ParentId);

        var childIndex = arrChiPhi.map(ele => {
            if (ele.iID_ChiPhi) return ele.iID_ChiPhi;
            else return ele.iID_ChiPhiID;
        }).indexOf(item.iID_ChiPhiID);

        // danh sách các chi phí đã được check
        var lstAllGoiThauChiPhiChecked = [];
        for (var gtID in lstChungTuGoiThau) {
            var lstChiPhiIDs = lstChungTuGoiThau[gtID][1].map(x => x.iID_ChiPhiID);
            lstAllGoiThauChiPhiChecked.push(...lstChiPhiIDs);
        }

        // 3 điều kiện hiển thị:
        // nếu chi phí này được check cho gói thầu hiện tại, hoặc chi phí cha được check cho gói thầu hiện tại
        // nếu chi phí này có giá trị phê duyệt và chưa được assign hết tiền
        // nếu chi phí này chưa được assign cho gói thầu nào
        if ((childIndex >= 0 || parentIndex >= 0) || (currGiaTriConLai > 0 && item.fGiaTriPheDuyet > 0) || lstAllGoiThauChiPhiChecked.indexOf(item.iID_ChiPhiID) < 0) {
            sItem.push("<tr data-id='" + item.iID_ChiPhiID + "' data-parent='" + item.iID_ParentId + "'>");
            sItem.push("<td class='width-50 text-center'><input type='checkbox' class='ck_ChiPhi' value='" + item.iID_ChiPhiID +
                (childIndex >= 0 || parentIndex >= 0 ? "\' checked " : "\'") + " onClick='CheckChiPhi(this)'></td>");
            sItem.push("<td class='width-50 text-center'>" + (countSTTChiPhi++) + "</td>")
            sItem.push("<td class='sNoiDung'>" + item.sNoiDung + "</td>");
            sItem.push("<td class='fGiaTriPheDuyet text-right'>" + FormatNumber(item.fGiaTriPheDuyet) + "</td>");
            // update ke hoach lua chon nha thau
            if (childIndex < 0 && parentIndex < 0)
                sItem.push(
                    "<td><input type='text' disabled='disabled' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' class='fGiaTriGoiThau form-control' style='text-align:right' value='0'/></td>");
            else {
                sItem.push(
                    "<td><input type='text' onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' class='fGiaTriGoiThau form-control' style='text-align:right' value='" +
                    (childIndex >= 0
                        ? (arrChiPhi[childIndex].fGiaTriGoiThau ? arrChiPhi[childIndex].fGiaTriGoiThau : arrChiPhi[childIndex].fTienGoiThau)
                        : (arrChiPhi[parentIndex].fGiaTriGoiThau ? arrChiPhi[parentIndex].fGiaTriGoiThau : arrChiPhi[parentIndex].fTienGoiThau)) +
                    "'/></td>");
            }
            sItem.push("<td class='fGiaTriConLai text-right'>" + FormatNumber(currGiaTriConLai) + "</td>");
            sItem.push("<td class='width-50'><button class='btn-primary' onclick='GetHangMucGoiThauDetail(\"" + iIdGoiThau + "\", \"" + item.iID_ChiPhiID + "\")' " + (childIndex >= 0 || parentIndex >= 0 ? " " : " disabled") + "><i class='fa fa-pencil-square-o fa-lg' aria-hidden='true')'></i></button></td>");
            sItem.push("</tr>");
        }        
    });
    $("#tblDanhSachChiPhiModal tbody").html(sItem.join(""));

    // update itemsChungTuGoiThau trong trường hợp sửa
    if (iIdKHLuaChonNhaThau) {
        // update chi phí
        $("#tblDanhSachChiPhiModal .fGiaTriGoiThau").each(function (index, item) {
            CheckChiPhi($(item).closest("tr").find(".ck_ChiPhi")[0]);
        });
        // update hạng mục
        itemsChungTuGoiThau[2] = $.map(itemsChungTuGoiThau[2], function (n) { return n.iID_ChiPhiID == $("#iIdChiPhiChoose").val() ? null : n }); // lấy những thằng cũ
        // cập nhật mảng itemschungtugoithau, thì loop qua các chi phí đang được check, tạo bảng hạng mục, nhưng k hiển thị, cho từng chi phí
        itemsChungTuGoiThau[1].forEach(chiphi => {
            GetHangMucGoiThauDetail(iIdGoiThau, chiphi.iID_ChiPhiID, true);
        });
    }

    $("#tblDanhSachChiPhiModal .fGiaTriGoiThau").change(function () {
        CheckChiPhi($(this).closest("tr").find(".ck_ChiPhi")[0]);
    });
}

function checkIfHangMucIsCheckedForOtherGoiThau(hangmucId, idGoiThau) {
    let returnVal = false;
    for (let goiThauId in lstChungTuGoiThau) {
        if (goiThauId !== idGoiThau) {
            var arrOfIndexHangMuc = lstChungTuGoiThau[goiThauId][2].map(hm => hm.iID_HangMucID);
            if (arrOfIndexHangMuc.indexOf(hangmucId) >= 0) {
                returnVal = true;
            }
        }
    }
    return returnVal;
}

// hàm kiểm tra xem có phải con, cháu j k, return true nếu elder is parent or grandpa,... else otherwise
function checkRelative(elder, child, arraySource) {

}

function getGiaTriGoiThauHangMuc(hm) {
    // hàm này được gọi kể cả khi chưa setup được lstChungTuGoiThau, khi đó cần phải lấy data từ API, flow tương tự trường hợp lấy từ lstChungTuGoiThau
    if (isSettingUpData) {
        let hmSelected = arrGoiThauHangMuc.find(x => x.iID_HangMucID == hm.iID_HangMucID);
        if (hmSelected) {
            return hmSelected.fTienGoiThau
        }
        else {
            // merge itemsChungTu and arrGoiThauHangMuc
            arrGoiThauHangMuc.map(h => {
                let h1 = itemsChungTu[2].find(x => x.iID_HangMucID == h.iID_HangMucID);
                return {
                    ...h,
                    iID_ParentId: h1.iID_ParentId
                }
            })
            let children = arrGoiThauHangMuc.filter(h => h.iID_ParentId == hm.iID_HangMucID);
            return children.map(x => x.fGiaTriGoiThau).reduce((a, b) => a += b, 0);
        }
    }
    else {
        let listHM = lstChungTuGoiThau[$('#iIdGoiThau').val()][2];
        let hmSelected = listHM.filter(h => h.iID_HangMucID == hm.iID_HangMucID)[0];
        // nếu hạng mục này được check -> lấy giá trị trong mảng lstChungTuGoiThau
        if (hmSelected) {
            return hmSelected.fGiaTriGoiThau;
        }
        // nếu hạng mục này k có trong lstChungTuGoiThau, tính tổng các con của nó 
        else {
            let children = listHM.filter(h => h.iID_ParentId == hm.iID_HangMucID);
            return children.map(x => x.fGiaTriGoiThau).reduce((a, b) => a += b, 0);
        }
    }    
}

// isAddingToItemsChungTuGoiThau: nếu đang cập nhật itemschungtuGoithau -> true
function GetHangMucGoiThauDetail(iIdGoiThau, iIdChiPhi, isAddingToItemsChungTuGoiThau) {
    $("#iIdChiPhiChoose").val(iIdChiPhi);
    var lstHangMuc = ($.map(itemsChungTu[2], function (n) { return n.iID_NguonVonID == 0 && n.iID_ChiPhiID == iIdChiPhi && n.iID_HangMucID != null ? n : null })).sort(sortHangMucByMaOrder);
    if (!isAddingToItemsChungTuGoiThau) {
        $("#tblHangMucChinh tbody").css("display", "block");
    }
    else {
        $("#tblHangMucChinh tbody").css("display", "none");
    }
    $("#tblHangMucChinh tbody").empty();
    var sItem = [], arrHangMuc = [];
    if (arrGoiThauHangMuc.length > 0) {
        arrHangMuc = [...arrGoiThauHangMuc.filter(nv => nv.iID_GoiThauID === iIdGoiThau)];
    } else {
        arrHangMuc = lstChungTuGoiThau[iIdGoiThau][2];
    }
    lstHangMuc.forEach(function (item) {
        // nếu hạng mục này đã được check cho gói thầu khác, thì sẽ k cho hiển thị 
        if (!checkIfHangMucIsCheckedForOtherGoiThau(item.iID_HangMucID, iIdGoiThau)) {
            var parentIndex = arrHangMuc.map(ele => ele.iID_HangMucID).indexOf(item.iID_ParentId);
            var childIndex = arrHangMuc.map(ele => ele.iID_HangMucID).indexOf(item.iID_HangMucID);
            var boldStyle = item.iID_ParentId ? '' : 'font-weight: bold;';   

            sItem.push(`<tr style="${boldStyle}" data-id='` + item.iID_HangMucID + "' data-parent='" + item.iID_ParentId + "'>");
            if (childIndex >= 0 || parentIndex >= 0) {
                sItem.push("<td class='width-50 text-center'><input type='checkbox' class='ck_HangMuc' value='" +
                    item.iID_HangMucID +
                    "' checked onClick='CheckHangMuc(this)'></td>");
            } else {
                sItem.push("<td class='width-50 text-center'><input type='checkbox' class='ck_HangMuc' value='" +
                    item.iID_HangMucID +
                    "' onClick='CheckHangMuc(this)'></td>");
            }
            sItem.push("<td class='sMaOrder width-50'>" + item.sMaOrder + "</td>");
            sItem.push("<td class='sNoiDung'>" + item.sNoiDung + "</td>");
            sItem.push("<td class='fGiaTriPheDuyet text-right'>" + FormatNumber(item.fGiaTriPheDuyet) + "</td>");
            sItem.push("<td class='fGiaTriGoiThau text-right'>" + FormatNumber(getGiaTriGoiThauHangMuc(item)) +"</td>");
            sItem.push("<td class='fGiaTriConLai text-right'>" +
                (FormatNumber(item.fGiaTriPheDuyet - getGiaTriGoiThauHangMuc(item)) ? FormatNumber(item.fGiaTriPheDuyet - getGiaTriGoiThauHangMuc(item)) : 0) +
                "</td>");
            sItem.push("</tr>");
        }
    });
    $("#tblHangMucChinh tbody").html(sItem.join(""));
    // update hạng mục trong itemschungtugoithau
    itemsChungTuGoiThau[2] = $.map(itemsChungTuGoiThau[2], function (n) { return n.iID_ChiPhiID == $("#iIdChiPhiChoose").val() ? null : n });
    $.each($("#tblHangMucChinh tbody").find(".ck_HangMuc:checkbox:checked"), function (index, child) {
        var obj = {};
        obj.iID_NguonVonID = null;
        obj.iID_ChiPhiID = $("#iIdChiPhiChoose").val();
        obj.iID_HangMucID = $(child).closest("tr").data("id");
        obj.iID_ParentId = $(child).closest("tr").data("parent");
        obj.sNoiDung = $(child).closest("tr").find(".sNoiDung").text();
        obj.iThuTu = $(child).closest("tr").index();
        obj.sMaOrder = $(child).closest("tr").find(".sMaOrder").text();
        obj.fGiaTriPheDuyet = parseFloat($(child).closest("tr").find(".fGiaTriPheDuyet").text().replaceAll(".", ""));
        obj.fGiaTriGoiThau = parseFloat($(child).closest("tr").find(".fGiaTriGoiThau").text().replaceAll(".", ""));
        itemsChungTuGoiThau[2].push(obj);
    });
}

function GetTongChiTiet() {
    var fTongNguonVon = 0;
    $.each($("#tblDanhSachNguonVonModal tbody").find(".ck_NguonVon:checkbox:checked"), function (index, item) {
        var sGiaTri = $(item).closest("tr").find(".fGiaTriGoiThau").val().replaceAll(".", "");
        if (sGiaTri != "")
            fTongNguonVon += parseFloat(sGiaTri);
    });

    var fTongChiPhi = 0;
    $.each($("#tblDanhSachChiPhiModal tbody").find("[data-parent='null'] .ck_ChiPhi:checkbox:checked"), function (index, item) {
        var sGiaTri = $(item).closest("tr").find(".fGiaTriGoiThau").val().replaceAll(".", "");
        if (sGiaTri != "")
            fTongChiPhi += parseFloat(sGiaTri);
    });

    if (fTongNguonVon == 0)
        $(".rTongNguonVon").text("0");
    else
        $(".rTongNguonVon").text(FormatNumber(fTongNguonVon));

    if (fTongNguonVon - fTongChiPhi == 0)
        $(".rConLai").text("0");
    else
        $(".rConLai").text(FormatNumber(fTongNguonVon - fTongChiPhi));
}

function ConvertItemsChungTu(lstChungTu) {
    var lstNguonVon = $.map(lstChungTu, function (n) { return n.iID_NguonVonID != 0 ? n : null });
    var lstChiPhi = $.map(lstChungTu, function (n) { return n.iID_ChiPhiID != null && n.iID_HangMucID == null ? n : null });
    var lstHangMuc = $.map(lstChungTu, function (n) { return n.iID_HangMucID != null ? n : null });

    var lstChiPhiConvert = [];
    $.map(lstChiPhi, function (n) { return n.iID_ParentId == null ? n : null }).forEach(function (item) {
        RecursiveChiPhi(item, lstChiPhi).forEach(function (itemChild) {
            lstChiPhiConvert.push(itemChild);
        });
    })

    var lstHangMucConvert = [];
    $.map(lstHangMuc, function (n) { return n.iID_ParentId == null ? n : null }).forEach(function (item) {
        RecursiveHangMuc(item, lstHangMuc).forEach(function (itemChild) {
            lstHangMucConvert.push(itemChild);
        });
    })

    itemsChungTu = [];
    itemsChungTu.push(lstNguonVon);
    itemsChungTu.push(lstChiPhiConvert);
    itemsChungTu.push(lstHangMucConvert);
}

function RecursiveChiPhi(itemChiPhi, lstChiPhi) {
    var lstChild = [];
    lstChild.push(itemChiPhi);
    $.map(lstChiPhi, function (n) { return n.iID_ParentId == itemChiPhi.iID_ChiPhiID && n.iID_HangMucID == null ? n : null }).forEach(function (item) {
        RecursiveChiPhi(item, lstChiPhi).forEach(function (itemChild) {
            lstChild.push(itemChild);
        });
    });
    return lstChild;
}

function RecursiveHangMuc(itemHangMuc, lstHangMuc) {
    var lstChild = [];
    lstChild.push(itemHangMuc);
    $.map(lstHangMuc, function (n) { return n.iID_ChiPhiID == itemHangMuc.iID_ChiPhiID && n.iID_ParentId == itemHangMuc.iID_HangMucID ? n : null }).forEach(function (item) {
        RecursiveHangMuc(item, lstHangMuc).forEach(function (itemChild) {
            lstChild.push(itemChild);
        });
    });
    return lstChild;
}

//- Check chi phi

function CheckAllChiPhi(item) {
    $.each($("#tblDanhSachChiPhiModal tbody").find("[data-parent='null'] .ck_ChiPhi"), function (index, parent) {
        parent.checked = item.checked;
        CheckChiPhi(parent);
    });
}

function recursiveUpdateGiaTriChiPhi_ToggleCheck(item) {
    // 1. nếu chi phí hiện tại k có con thì giá trị gói thầu cho chi phí hiện tại = giá trị còn lại của chi phí đó
    // 2. nếu chi phí hiện tại có con
    // 2.1. recursiveUpdateGiaTriChiPhi_ToggleCheck qua các con để update giá trị gói thầu cho các con
    // 2.2. update giá trị của chi phí hiện tại khi đã update xong các con
    // 3. nếu chi phí hiện tại có bố
    // 3.1. recursiveUpdateGiaTriChiPhi_ToggleCheck bố cho đến khi bố = null
    if (item.checked) {
        let fGiaTriConLai = parseFloat($(item).closest("tr").find(".fGiaTriConLai").text().replaceAll(".", ""));
        let giaTriGoiThau = 0;

        let childCounter = 0;
        $.each($("#tblDanhSachChiPhiModal tbody [data-parent='" + $(item).val() + "'] .ck_ChiPhi:checkbox:checked"), function (index, child) {
            childCounter++;
        });

        if (childCounter > 0) {
            $.each($("#tblDanhSachChiPhiModal tbody [data-parent='" + $(item).val() + "'] .ck_ChiPhi:checkbox:checked"), function (index, child) {
                recursiveUpdateGiaTriChiPhi_ToggleCheck(child);
                let gtrGoiThauChildAfter = $(child).closest("tr").find(".fGiaTriGoiThau").val().replaceAll(".", "");
                giaTriGoiThau += parseFloat(gtrGoiThauChildAfter);
            });
            let giaTriConLai = fGiaTriConLai - giaTriGoiThau;
            $($(item).closest("tr").find(".fGiaTriConLai")[0]).text(FormatNumber(giaTriConLai));
            $(item).closest("tr").find(".fGiaTriGoiThau").val(FormatNumber(giaTriGoiThau));
            
        }
        else {
            giaTriGoiThau = fGiaTriConLai;
            $($(item).closest("tr").find(".fGiaTriConLai")[0]).text(FormatNumber(0));
            $(item).closest("tr").find(".fGiaTriGoiThau").val(FormatNumber(giaTriGoiThau));
        }
    }
    else {

    }
}

function updateGiaTriChiPhiWhenChecked(item) {
    if (item) {
        if (item.checked) {
            //let giaTriDuocDuyet = parseFloat($(item).closest("tr").find(".fGiaTriPheDuyet").text().replaceAll(".", ""));
            let fGiaTriConLai = parseFloat($(item).closest("tr").find(".fGiaTriConLai").text().replaceAll(".", ""));
            let giaTriGoiThau = parseFloat($(item).closest("tr").find(".fGiaTriGoiThau").val().replaceAll(".", ""));
            // nếu các hạng mục của chi phí này chưa được checked cái nào thì update giá trị chi phí
            // nếu k có con -> giá trị chi phí = giá trị còn lại
            // nếu có con -> giá trị gói thầu bằng tổng giá trị gói thầu của các con được check
            let lstHangMucCheckedOfCurrentCp = itemsChungTuGoiThau[2].filter(h => h.iID_ChiPhiID == $(item).val());
            if (lstHangMucCheckedOfCurrentCp.length == 0) {
                $.each($("#tblDanhSachChiPhiModal tbody [data-parent='" + $(item).val() + "'] .ck_ChiPhi:checkbox:checked"), function (index, child) {
                    var sGiaTri = $(child).closest("tr").find(".fGiaTriGoiThau").val().replaceAll(".", "");
                    if (sGiaTri != "")
                        giaTriGoiThau += parseFloat(sGiaTri);
                });
                $(item).closest("tr").find(".fGiaTriGoiThau").val(FormatNumber(giaTriGoiThau));
            }
            if (giaTriGoiThau == 0) {
                $(item).closest("tr").find(".fGiaTriGoiThau").val(FormatNumber(fGiaTriConLai));
                giaTriGoiThau = fGiaTriConLai;
            }
            let giaTriConLai = fGiaTriConLai - giaTriGoiThau;
            $($(item).closest("tr").find(".fGiaTriConLai")[0]).text(FormatNumber(giaTriConLai));
        }
    }    
}

function CheckChiPhi(item) {
    
    RecursiveCheckChildChiPhi(item, item.checked);
    RecursiveCheckParentChiPhi(item, item.checked);

    //recursiveUpdateGiaTriChiPhi_ToggleCheck(item);
    
    var fTong = 0
    $.each($("#tblDanhSachChiPhiModal tbody").find("[data-parent='null'] .ck_ChiPhi:checkbox:checked"), function (index, parent) {
        if ($(parent).closest("tr").find(".fGiaTriGoiThau").text().replaceAll(".", "") != "")
            fTong += parseFloat($(parent).closest("tr").find(".fGiaTriGoiThau").text().replaceAll(".", ""));
    });

    itemsChungTuGoiThau[1] = [];
    $.each($("#tblDanhSachChiPhiModal tbody").find(".ck_ChiPhi:checkbox:checked"), function (index, child) {
        var obj = {};
        obj.iID_ChiPhiID = $(child).closest("tr").data("id");
        obj.iID_ParentId = $(child).closest("tr").data("parent");
        obj.sNoiDung = $(child).closest("tr").find(".sNoiDung").text();
        obj.iThuTu = $(child).closest("tr").index();
        obj.fGiaTriPheDuyet = parseFloat($(child).closest("tr").find(".fGiaTriPheDuyet").text().replaceAll(".", ""));
        obj.fGiaTriGoiThau = parseFloat($(child).closest("tr").find(".fGiaTriGoiThau").val().replaceAll(".", ""));
        itemsChungTuGoiThau[1].push(obj);
    });
    GetTongChiTiet();
    //updateGiaTriChiPhiWhenChecked(item);
    //// update giá trị gói thầu của chi phí cha
    //var parentId = $(item).closest("tr").data("parent");
    //var parentItem = $("#tblDanhSachChiPhiModal tbody [data-id='" + parentId + "'] .ck_ChiPhi");
    //updateGiaTriChiPhiWhenChecked(parentItem[0]);
}

function RecursiveCheckParentChiPhi(item, bIsCheck) {
    var parentId = $(item).closest("tr").data("parent");
    if (parentId == null) return;

    var fGiaTri = 0;
    if ($(item).closest("tr").find(".fGiaTriGoiThau").val().replaceAll(".", "") != "")
        fGiaTri = parseFloat($(item).closest("tr").find(".fGiaTriGoiThau").val().replaceAll(".", ""))
    var parentItem = $("#tblDanhSachChiPhiModal tbody [data-id='" + parentId + "'] .ck_ChiPhi");    

    var fGiaTriPheDuyet = parseFloat($("#tblDanhSachChiPhiModal tbody [data-id='" + parentId + "'] .fGiaTriPheDuyet").text().replaceAll(".", ""));
    var fGiaTriGoiThau = 0;
    $.each($("#tblDanhSachChiPhiModal tbody [data-parent='" + parentId + "'] .ck_ChiPhi:checkbox:checked"), function (index, child) {
        var sGiaTri = $(child).closest("tr").find(".fGiaTriGoiThau").val().replaceAll(".", "");
        if (sGiaTri != "")
            fGiaTriGoiThau += parseFloat(sGiaTri);
    });

    /*updateGiaTriChiPhiWhenChecked($(parentItem)[0]);*/

    if (fGiaTriGoiThau == 0) {
        $(parentItem).closest("tr").find(".fGiaTriGoiThau").val("0");
        $(parentItem).closest("tr").find(".fGiaTriConLai").text(FormatNumber(fGiaTriPheDuyet));
    } else {
        $(parentItem).closest("tr").find(".fGiaTriGoiThau").val(FormatNumber(fGiaTriGoiThau));
        if (fGiaTriPheDuyet - fGiaTriGoiThau != 0)
            $(parentItem).closest("tr").find(".fGiaTriConLai").text(FormatNumber(getChiPhiForCurrentGoiThau($("#iIdGoiThau").val(), item)));
        else
            $(parentItem).closest("tr").find(".fGiaTriConLai").text("0")
    }
    // update giá trị còn lại (bắt buộc) dựa trên các gói thầu
    var lstChiPhi = $.map(itemsChungTu[1], function (n) { return n.iID_NguonVonID == 0 && n.iID_HangMucID == null ? n : null });
    var currChiPhi = lstChiPhi.filter(cp => cp.iID_ChiPhiID === $(item).val())[0];
    $(item).closest("tr").find(".fGiaTriConLai").text(FormatNumber(getChiPhiForCurrentGoiThau($("#iIdGoiThau").val(), currChiPhi)));

    $(parentItem).closest("tr").find(".fGiaTriGoiThau").prop("disabled", true);
    $(parentItem).closest("tr").find("button").prop("disabled", true);

    if ($("#tblDanhSachChiPhiModal tbody").find("[data-parent='" + parentId + "'] .ck_ChiPhi:checkbox:checked").length == 0) {
        if (bIsCheck) {
            $(parentItem).closest("tr").find(".fGiaTriGoiThau").prop("disabled", false);
            $(parentItem).closest("tr").find("button").prop("disabled", false);
        }
    } else if (!bIsCheck) {
        parentItem[0].checked = !bIsCheck;
    }

    RecursiveCheckParentChiPhi(parentItem, bIsCheck);
}

function RecursiveCheckChildChiPhi(item, bIsCheck) {
    $(item).closest("tr").find(".fGiaTriGoiThau").prop("disabled", true);
    $(item).closest("tr").find("button").prop("disabled", true);
    var iId = $(item).val();
    var fSumTong = 0;
    var countChild = 0;
    $.each($("#tblDanhSachChiPhiModal tbody").find("[data-parent='" + iId + "'] .ck_ChiPhi"), function (index, child) {
        child.checked = bIsCheck;
        fSumTong += RecursiveCheckChildChiPhi($(child), bIsCheck);
        //updateGiaTriChiPhiWhenChecked($(child)[0]);
        countChild++;
    });
    if (bIsCheck && fSumTong == 0) {
        if ($(item).closest("tr").find(".fGiaTriGoiThau").val().replaceAll(".", "") != "")
            fSumTong = parseFloat($(item).closest("tr").find(".fGiaTriGoiThau").val().replaceAll(".", ""));
    }
    var fGiaTriPheDuyet = parseFloat($(item).closest("tr").find(".fGiaTriPheDuyet").text().replaceAll(".", ""));
    var lstHangMuc = $.map(itemsChungTu[2], function (n) { return n.iID_NguonVonID == 0 && n.iID_ChiPhiID == iId && n.iID_HangMucID != null ? n : null });

    if (bIsCheck) {
        if (countChild == 0) {
            $(item).closest("tr").find(".fGiaTriGoiThau").prop("disabled", false);
            $(item).closest("tr").find("button").prop("disabled", false);
        } else {
            $(item).closest("tr").find(".fGiaTriGoiThau").prop("disabled", true);
            $(item).closest("tr").find("button").prop("disabled", true);
        }

        if (lstHangMuc.length > 0)
            $(item).closest("tr").find(".fGiaTriGoiThau").prop("disabled", true);
        else
            $(item).closest("tr").find(".fGiaTriGoiThau").prop("disabled", false);

        if (fSumTong == 0)
            $(item).closest("tr").find(".fGiaTriGoiThau").val("0");
        else
            $(item).closest("tr").find(".fGiaTriGoiThau").val(FormatNumber(fSumTong));

        if (fGiaTriPheDuyet - fSumTong == 0)
            $(item).closest("tr").find(".fGiaTriConLai").text("0");
        else
            $(item).closest("tr").find(".fGiaTriConLai").text(FormatNumber(getChiPhiForCurrentGoiThau($("#iIdGoiThau").val(), item)));
    } else {
        $(item).closest("tr").find(".fGiaTriGoiThau").val("0");
        $(item).closest("tr").find(".fGiaTriConLai").text(FormatNumber(fGiaTriPheDuyet));
    }
    // update giá trị còn lại (bắt buộc) dựa trên các gói thầu
    var lstChiPhi = $.map(itemsChungTu[1], function (n) { return n.iID_NguonVonID == 0 && n.iID_HangMucID == null ? n : null });
    var currChiPhi = lstChiPhi.filter(cp => cp.iID_ChiPhiID === $(item).val())[0];
    $(item).closest("tr").find(".fGiaTriConLai").text(FormatNumber(getChiPhiForCurrentGoiThau($("#iIdGoiThau").val(), currChiPhi)));
    return fSumTong;
}

//- Check hang muc
function CheckAllHangMuc(item) {
    $.each($("#tblHangMucChinh tbody").find("[data-parent='null'] .ck_HangMuc"), function (index, parent) {
        parent.checked = item.checked;
        CheckHangMuc(parent);
    });
}

function CheckHangMuc(item) {
    RecursiveCheckChildHangMuc(item, item.checked);
    RecursiveCheckParentHangMuc(item, item.checked);
    var fTong = 0
    $.each($("#tblHangMucChinh tbody").find("[data-parent='null'] .ck_HangMuc"), function (index, parent) {
        let fGiaTriParentTxt = $(parent).closest("tr").find(".fGiaTriGoiThau").text().replaceAll(".", "");
        fTong += parseFloat(fGiaTriParentTxt ? fGiaTriParentTxt : 0);
    });

    var rowChiPhi = $("#tblDanhSachChiPhiModal [data-id='" + $("#iIdChiPhiChoose").val() + "']");
    var fGiaTriPheDuyet = parseFloat($(rowChiPhi).find(".fGiaTriPheDuyet").text().replaceAll(".", ""));
    $(rowChiPhi).find(".fGiaTriGoiThau").val(FormatNumber(fTong));
    if (fTong == 0) {
        $(rowChiPhi).find(".fGiaTriGoiThau").prop("disabled", false);
    } else {
        $(rowChiPhi).find(".fGiaTriGoiThau").prop("disabled", true);
    }
    var lstChiPhi = $.map(itemsChungTu[1], function (n) { return n.iID_NguonVonID == 0 && n.iID_HangMucID == null ? n : null });
    var currChiPhi = lstChiPhi.filter(cp => cp.iID_ChiPhiID === $($(rowChiPhi)[0]).data('id'))[0];
    $(rowChiPhi).find(".fGiaTriConLai").text(FormatNumber(getChiPhiForCurrentGoiThau($("#iIdGoiThau").val(), currChiPhi) - fTong));
    //CheckChiPhi(rowChiPhi.find(".ck_ChiPhi")[0]);

    itemsChungTuGoiThau[2] = $.map(itemsChungTuGoiThau[2], function (n) { return n.iID_ChiPhiID == $("#iIdChiPhiChoose").val() ? null : n });
    $.each($("#tblHangMucChinh tbody").find(".ck_HangMuc:checkbox:checked"), function (index, child) {
        var obj = {};
        obj.iID_NguonVonID = null;
        obj.iID_ChiPhiID = $("#iIdChiPhiChoose").val();
        obj.iID_HangMucID = $(child).closest("tr").data("id");
        obj.iID_ParentId = $(child).closest("tr").data("parent");
        obj.sNoiDung = $(child).closest("tr").find(".sNoiDung").text();
        obj.iThuTu = $(child).closest("tr").index();
        obj.sMaOrder = $(child).closest("tr").find(".sMaOrder").text();
        obj.fGiaTriPheDuyet = parseFloat($(child).closest("tr").find(".fGiaTriPheDuyet").text().replaceAll(".", ""));
        obj.fGiaTriGoiThau = parseFloat($(child).closest("tr").find(".fGiaTriGoiThau").text().replaceAll(".", ""));
        itemsChungTuGoiThau[2].push(obj);
    });
}

function RecursiveCheckParentHangMuc(item, bIsCheck) {
    var parentId = $(item).closest("tr").data("parent");
    if (parentId == null) return;

    var parentItem = $("#tblHangMucChinh tbody [data-id='" + parentId + "'] .ck_HangMuc");
    var fGiaTriPheDuyet = parseFloat($("#tblHangMucChinh tbody [data-id='" + parentId + "'] .fGiaTriPheDuyet").text().replaceAll(".", ""));
    var fGiaTriGoiThau = 0;
    $.each($("#tblHangMucChinh tbody [data-parent='" + parentId + "'] .ck_HangMuc"), function (index, child) {
        var sGiaTri = $(child).closest("tr").find(".fGiaTriGoiThau").text().replaceAll(".", "");
        if (sGiaTri != "")
            fGiaTriGoiThau += parseFloat(sGiaTri);
    });
    if (fGiaTriGoiThau == fGiaTriPheDuyet) { parentItem[0].checked = bIsCheck; }
    else if (!bIsCheck) {
        parentItem[0].checked = false;
    }
    if (!bIsCheck && $("#tblHangMucChinh tbody").find("[data-parent='" + parentId + "'] .ck_HangMuc:checkbox:checked").length != 0) {
        parentItem[0].checked = bIsCheck;
    }
    if (fGiaTriGoiThau == 0) {
        $(parentItem).closest("tr").find(".fGiaTriGoiThau").text("0");
        $(parentItem).closest("tr").find(".fGiaTriConLai").text(FormatNumber(fGiaTriPheDuyet));
    }
    else {
        $(parentItem).closest("tr").find(".fGiaTriGoiThau").text(FormatNumber(fGiaTriGoiThau));
        if (fGiaTriPheDuyet - fGiaTriGoiThau != 0)
            $(parentItem).closest("tr").find(".fGiaTriConLai").text(FormatNumber(fGiaTriPheDuyet - fGiaTriGoiThau));
        else
            $(parentItem).closest("tr").find(".fGiaTriConLai").text("0");
    }
    RecursiveCheckParentHangMuc(parentItem, bIsCheck);
}

function RecursiveCheckChildHangMuc(item, bIsCheck) {
    var iId = $(item).val();
    var fSumTong = 0;
    $.each($("#tblHangMucChinh tbody").find("[data-parent='" + iId + "'] .ck_HangMuc"), function (index, child) {
        child.checked = bIsCheck;
        fSumTong += RecursiveCheckChildHangMuc($(child), bIsCheck);
    });
    if (bIsCheck && fSumTong == 0) {
        fSumTong = parseFloat($(item).closest("tr").find(".fGiaTriPheDuyet").text().replaceAll(".", ""));
    }
    var fGiaTriPheDuyet = parseFloat($(item).closest("tr").find(".fGiaTriPheDuyet").text().replaceAll(".", ""));
    if (bIsCheck) {
        $(item).closest("tr").find(".fGiaTriGoiThau").text(FormatNumber(fSumTong));
        if (fGiaTriPheDuyet - fSumTong == 0)
            $(item).closest("tr").find(".fGiaTriConLai").text("0");
        else
            $(item).closest("tr").find(".fGiaTriConLai").text(FormatNumber(fGiaTriPheDuyet - fSumTong));
    } else {
        $(item).closest("tr").find(".fGiaTriGoiThau").text("0");
        $(item).closest("tr").find(".fGiaTriConLai").text(FormatNumber(fGiaTriPheDuyet));
    }
    return fSumTong;
}

// lưu lại giá trị các chi phí đã chọn
function saveFGiaTriGoiThau_AllCheckedChiPhi(goithauID) {
    $('#tblDanhSachChiPhiModal tbody ').find(".ck_ChiPhi:checkbox:checked").each((index, cp) => {
        let lstCP = itemsChungTuGoiThau[1];
        for (let i = 0; i < lstCP.length; i++) {
            if (lstCP[i].iID_ChiPhiID == $(cp).val()) {
                lstCP[i].fGiaTriGoiThau = parseInt(UnFormatNumber($(cp).closest('tr').find('input.fGiaTriGoiThau').val()));
            }
        }
    });
}

// lưu lại giá trị các hạng mục đã chọn
function saveFGiaTriGoiThau_AllCheckedHangMuc(goithauID) {
    $('#tblDanhSachHangMuc tbody ').find(".ck_HangMuc:checkbox:checked").each((index, cp) => {
        let lstCP = itemsChungTuGoiThau[2];
        for (let i = 0; i < lstCP.length; i++) {
            if (lstCP[i].iID_ChiPhiID == $(cp).val()) {
                lstCP[i].fGiaTriGoiThau = parseInt(UnFormatNumber($(cp).closest('tr').find('input.fGiaTriGoiThau').val()));
            }
        }
    });
}

//------ Event Window
function SaveDetailGoiThau() {
    var sConLai = $(".rConLai").text().replaceAll(".", "");
    var fConLai = 0;
    if (sConLai != "")
        fConLai = parseFloat(sConLai);
    // Bỏ validate tổng nguồn vốn = tổng chi phí
    /*
    if (fConLai != 0) {
        alert("Tổng nguồn vốn phải bằng tổng chi phí !");
        return;
    }
    */

    if (!validateGiaTriConLaiBeforeSave($("#iIdGoiThau").val())) {
        return;
    }

    lstChungTuGoiThau[$("#iIdGoiThau").val()] = [];
    if (itemsChungTuGoiThau == null || (itemsChungTuGoiThau[0].length == 0 && itemsChungTuGoiThau[1].length == 0 && itemsChungTuGoiThau[2].length == 0)) {
        alert("Chưa chọn thông tin chi tiết gói thầu !");
        return;
    }
    saveFGiaTriGoiThau_AllCheckedChiPhi($("#iIdGoiThau").val());
    saveFGiaTriGoiThau_AllCheckedHangMuc($("#iIdGoiThau").val());
    lstChungTuGoiThau[$("#iIdGoiThau").val()] = [...itemsChungTuGoiThau];
    $("#tblGoiThau [data-id='" + $("#iIdGoiThau").val() + "'] .fGiaTri").val($(".rTongNguonVon").text());
    if ($('.ck_NguonVon:checked').length > 0) {
        $("#tblGoiThau [data-id='" + $("#iIdGoiThau").val() + "'] .fGiaTri").prop('disabled', true);
    } else {
        $("#tblGoiThau [data-id='" + $("#iIdGoiThau").val() + "'] .fGiaTri").prop('disabled', false);
    }
    alert("Cập nhật thành công !");
    $("#modalChiTietGoiThau").modal("hide");
}

// check trùng số quyết định
function checkExistSoQuyetDinh() {
    let checkVal = $("#sSoQuyetDinh").val();
    var iid_KHLCNT = $("#iIdKeHoachLuaChonNhaThau").val();
    return $.ajax({
        url: "/QLVonDauTu/KHLuaChonNhaThau/CheckExistSoQuyetDinh",
        type: "GET",
        data: { checkVal: checkVal, id: iid_KHLCNT },
        dataType: "json"
    })
}

async function SaveKHLCNT() {
    if (!ValidateKHLCNT()) return;
    let res = await checkExistSoQuyetDinh();
    if (res.isExisted) {
        let errMsg = "Số quyết định đã tồn tại";
        let element = $("#sSoQuyetDinh").get(0);
        element.classList.add("input-error");
        element.setCustomValidity(errMsg);
        element.reportValidity();
        return;
    }
    var objKeHoachLCNT = SetKeHoachLuaChonNhaThau();
    var lstGoiThau = SetGoiThau();

    var obj = {};
    obj.objKHLuaChonNhaThau = objKeHoachLCNT;
    obj.lstGoiThau = lstGoiThau;
    obj.lstDetail = SetGoiThauDetail(obj.lstGoiThau);
    obj.isDieuChinh = bIsDieuChinh;
    if (obj.isDieuChinh == 1) {
        obj.isDieuChinh = true;
    }

    $.ajax({
        url: "/QLVonDauTu/KHLuaChonNhaThau/OnSave",
        type: "POST",
        data: { data: obj },
        dataType: "json",
        async: false,
        success: function (result) {
            if (result.bIsComplete) {
                alert("Cập nhật dữ liệu thành công !");
                location.href = "/QLVonDauTu/KHLuaChonNhaThau";
            } else {
                if (result.messError)
                    alert(result.messError);
                else
                    alert("Có lỗi xảy ra khi lưu dữ liệu !");
            }
        }
    })

}

function ValidateKHLCNT() {
    var sMessError = [];
    if ($("#cbxLoaiChungTu").val() == undefined || $("#cbxLoaiChungTu").val() == null) {
        sMessError.push("Chưa chọn căn cứ !");
    }
    if ($("#sSoQuyetDinh").val() == undefined || $("#sSoQuyetDinh").val() == null || $("#sSoQuyetDinh").val().trim() == "") {
        sMessError.push("Chưa điền số quyết định !");
    }
    if ($("#dNgayQuyetDinh").val() == undefined || $("#dNgayQuyetDinh").val() == null || $("#dNgayQuyetDinh").val().trim() == "") {
        sMessError.push("Chưa điền ngày quyết định !");
    }
    if ($("#iID_DonViQuanLyID").val() == undefined || $("#iID_DonViQuanLyID").val() == null) {
        sMessError.push("Chưa chọn đơn vị quản lý !");
    }
    if ($("#iID_DuAnID").val() == undefined || $("#iID_DuAnID").val() == null) {
        sMessError.push("Chưa chọn dự án !");
    }
    if ($(".sTenGoiThau ").val() == "" || $(".sTenGoiThau ").val() == undefined || $(".sTenGoiThau").val() == null) {
        sMessError.push("Tên gói thầu chưa được chọn !");
    }
    //if ($("#fGiaTri ").val() == "" || $(".fGiaTri ").val() == undefined || $(".fGiaTri").val() == null) {
    //    sMessError.push("Giá trị gói thầu chưa được chọn !");
    //}
    //if ($("#iThoiGianThucHien ").val() == "" || $("#iThoiGianThucHien ").val() == undefined || $("#iThoiGianThucHien").val() == null) {
    //    sMessError.push("Thời gian thực hiện chưa được chọn !");
    //}   

    if ($('input:radio[name="rd_ChungTu"]:checked').length == 0) {
        sMessError.push("Chưa chọn chứng từ !");
    }
    if ($("#tblGoiThau tr").not(".error-row").length == 0) {
        sMessError.push("Chưa nhập gói thầu !");
    }

    //if (!$(".sHinhThucChonNhaThau").val()) {
    //    sMessError.push("Hình thức chọn nhà thầu chưa được chọn !");
    //}

    //if (!$(".sPhuongThucDauThau").val()) {
    //    sMessError.push("Phương thức lựa chọn chưa được chọn !");
    //}

    if (!$(".sHinhThucHopDong").val()) {
        sMessError.push("Loại hợp đồng chưa được chọn !");
    }

    if (sMessError.length != 0) {
        alert(sMessError.join("\n"));
        return false;
    }
    return true;
}

function SetKeHoachLuaChonNhaThau() {
    //if (bIsDieuChinh == 1) {
    //    bIsDieuChinh = true
    //}
    var iLoaiChungTu = $("#cbxLoaiChungTu").val();
    var obj = {};
    if (!bIsDieuChinh) {
        obj.Id = $("#iIdKeHoachLuaChonNhaThau").val();
    } else {
        obj.Id = guidEmpty;
        obj.iID_ParentID = $("#iIdKeHoachLuaChonNhaThau").val();
    }
    obj.sSoQuyetDinh = $("#sSoQuyetDinh").val();
    obj.dNgayQuyetDinh = $("#dNgayQuyetDinh").val();
    obj.iID_DonViQuanLyID = $("#iID_DonViQuanLyID").find(":selected").data("iiddonvi");
    obj.iID_MaDonViQuanLy = $("#iID_DonViQuanLyID").val();
    obj.iID_DuAnID = $("#iID_DuAnID").val();
    obj.sMoTa = $("#sMoTa").val();
    if (iLoaiChungTu == 1)
        obj.iID_DuToanID = $('input:radio[name="rd_ChungTu"]:checked').val();
    else if (iLoaiChungTu == 2)
        obj.iID_QDDauTuID = $('input:radio[name="rd_ChungTu"]:checked').val();
    else obj.iID_ChuTruongDauTuID = $('input:radio[name="rd_ChungTu"]:checked').val();
    return obj;
}

function SetGoiThau() {
    var lstGoiThau = [];
    $.each($("#tblGoiThau tr").not(".error-row"), function (index, item) {
        objGoiThau = {};
        objGoiThau.iID_GoiThauID = $(item).data("id");
        objGoiThau.iID_DuAnID = $("#iID_DuAnID").val();
        //objGoiThau.sTenGoiThau = $(item).find("#sTenGoiThauinput").val();
        objGoiThau.sTenGoiThau = $(item).find(".sTenGoiThau").val();
        //objGoiThau.sHinhThucChonNhaThau = $(item).find(".sHinhThucChonNhaThau").val();
        //objGoiThau.sPhuongThucDauThau = $(item).find(".sPhuongThucDauThau").val();
        //objGoiThau.sThoiGianTCLCNT = $(item).find(".sThoiGianTCLCNT").val();
        objGoiThau.sHinhThucHopDong = $(item).find(".sHinhThucHopDong").val();
        //objGoiThau.iThoiGianThucHien = $(item).find("#iThoiGianThucHien input").val();
        objGoiThau.iThoiGianThucHien = $(item).find(".iThoiGianThucHien").val();
        var sGiaTri = $(item).find(".fGiaTri").val().replaceAll(".", "");
        if (sGiaTri != "")
            objGoiThau.fTienTrungThau = parseFloat(sGiaTri);
        else
            objGoiThau.fTienTrungThau = 0;
        objGoiThau.iID_KHLCNhaThau = $("#iIdKeHoachLuaChonNhaThau").val();
        lstGoiThau.push(objGoiThau);
    });
    return lstGoiThau;
}

function SetGoiThauDetail(lstGoiThau) {
    var lstDetail = [];

    $.each(lstGoiThau, function (index, item) {

        // nguon von
        if (lstChungTuGoiThau[item.iID_GoiThauID] == null) return false;

        if (lstChungTuGoiThau[item.iID_GoiThauID][0] != null) {
            lstChungTuGoiThau[item.iID_GoiThauID][0].forEach(function (child) {
                var objNguonVon = {};
                objNguonVon.iID_GoiThauID = item.iID_GoiThauID;
                objNguonVon.iID_NguonVonID = child.iID_NguonVonID;
                objNguonVon.fGiaTriPheDuyet = child.fGiaTriPheDuyet;
                objNguonVon.fGiaTriGoiThau = child.fGiaTriGoiThau;
                lstDetail.push(objNguonVon);
            });
        }

        if (lstChungTuGoiThau[item.iID_GoiThauID][1] != null) {
            lstChungTuGoiThau[item.iID_GoiThauID][1].forEach(function (child) {
                var objNguonVon = {};
                objNguonVon.iID_GoiThauID = item.iID_GoiThauID;
                objNguonVon.iID_ChiPhiID = child.iID_ChiPhiID;
                objNguonVon.iID_ParentId = child.iID_ParentId;
                objNguonVon.sNoiDung = child.sNoiDung;
                objNguonVon.fGiaTriPheDuyet = child.fGiaTriPheDuyet;
                objNguonVon.fGiaTriGoiThau = child.fGiaTriGoiThau;
                lstDetail.push(objNguonVon);
            });
        }

        if (lstChungTuGoiThau[item.iID_GoiThauID][2] != null) {
            lstChungTuGoiThau[item.iID_GoiThauID][2].forEach(function (child) {
                var objNguonVon = {};
                objNguonVon.iID_GoiThauID = item.iID_GoiThauID;
                objNguonVon.iID_ChiPhiID = child.iID_ChiPhiID;
                objNguonVon.iID_HangMucID = child.iID_HangMucID;
                objNguonVon.iID_ParentId = child.iID_ParentId;
                objNguonVon.sNoiDung = child.sNoiDung;
                objNguonVon.sMaOrder = child.sMaOrder;
                objNguonVon.iThuTu = child.iThuTu;
                objNguonVon.fGiaTriPheDuyet = child.fGiaTriPheDuyet;
                objNguonVon.fGiaTriGoiThau = child.fGiaTriGoiThau;
                lstDetail.push(objNguonVon);
            });
        }
    });

    return lstDetail;
}

function CancelSaveData() {
    window.location.href = "/QLVonDauTu/KHLuaChonNhaThau/Index";
}

function DetailGoiThau(id) {
    window.location.href = "/QLVonDauTu/QLThongTinGoiThau/Detail/" + id;
}

function ReloadPhuongThucLuaChon() {
    $(".sHinhThucChonNhaThau ").change(function () {

        var changeLine = this.parentElement.parentElement;
        var dataHTCNT = $(this).val();
        if (dataHTCNT == "Chỉ định thầu rút gọn") {
            $(changeLine).find(".sPhuongThucDauThau").val("");
        }
    });
}

function getUrlParameter(sParam) {
    var sPageURL = window.location.search.substring(1),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
        }
    }
    return false;
};

function isExistGoiThau(iIdGoiThau) {
    var lstNguonVon = $.map(itemsChungTu[0], function (n) { return n.iID_NguonVonID != 0 ? n : null });
    // chỉ dùng arrGoiThauNguonVon 1 lần
    var arrNguonVon = [];
    if (arrGoiThauNguonVon.length > 0) {
        arrNguonVon = [...arrGoiThauNguonVon.filter(nv => nv.iID_GoiThauID === iIdGoiThau)];
    }
    else {
        if (lstChungTuGoiThau[iIdGoiThau])
            arrNguonVon = lstChungTuGoiThau[iIdGoiThau][0];
    }

    var countNguonVon = 0;
    lstNguonVon.forEach(function (item) {
        var indexInArrGoiThauNguonVon = arrNguonVon.map(ele => ele.iID_NguonVonID).indexOf(item.iID_NguonVonID);
        if (indexInArrGoiThauNguonVon >= 0) countNguonVon++;
    });

    return countNguonVon > 0;
}

function sortHangMucByMaOrder(a, b) {
    var currentMaOrder = a.sMaOrder.toLowerCase();
    var nextMaOrder = b.sMaOrder.toLowerCase();
    return ((currentMaOrder < nextMaOrder) ? -1 : ((currentMaOrder > nextMaOrder) ? 1 : 0));
}

const sortChiPhi = (array) => {
    var keyArray = Object.keys(array);
    
    for (let i = 0; i < keyArray.length; i++) {
        isOrdered = true;
        for (let x = 0; x < keyArray.length - 1 - i; x++) {
            if (array[keyArray[x]].iThuTu > array[keyArray[x + 1]].iThuTu) {
                [array[keyArray[x]], array[keyArray[x + 1]] = array[keyArray[x + 1]], array[keyArray[x]]];

                var temp = array[keyArray[x + 1]]
                array[keyArray[x + 1]] = array[keyArray[x]]
                array[keyArray[x]] = temp;
            }
        }
    }

    return array;

    //Object.keys(Items).forEach(function (key) {
    //    sItem.push("<tr>");
    //    sItem.push("<td>" + (isChiPhi ? (count + '. ') : '') + Items[key].sNoiDung + "</td>");
    //    sItem.push("<td class='text-right'>" + FormatNumber(Items[key].fGiaTriPheDuyet) + "</td>");
    //    sItem.push("</tr>");

    //    count++;
    //});
}



