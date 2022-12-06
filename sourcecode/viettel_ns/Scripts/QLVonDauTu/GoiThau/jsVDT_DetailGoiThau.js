var arrChiPhi = [];
var arrNguonVon = [];
var arrHangMuc = [];
var TBL_CHI_PHI_DAU_TU = "tblChiPhiDauTu";
var TBL_NGUON_VON_DAU_TU = "tblNguonVonDauTu";
var TBL_HANG_MUC_DAU_TU = "tblHangMucDauTu";
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';

$(document).ready(function () {
    LoadDataComboBoxNguonNganSach();
    GetThongTinNguonVonChiPhiGoiThau();


});

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
            if (data.hinhThucChonNT != null) {
                arr_HinhThucChonNhaThau = data.hinhThucChonNT;
                var ptChonNhaThauSelected = $("#ptChonNhaThauSelected").val();
                var htChonNT = CreateHtmlSelectHinhThucChonNhaThau(ptChonNhaThauSelected);
                $("#HTChonNT").html(htChonNT);
            }


            if (data.loaiHopDong != null) {
                arr_LoaiHopDong = data.loaiHopDong;
                var htHopDongSelected = $("#htHopDongSelected").val();
                var htHopDongSelect = CreateHtmlSelectLoaiHopDong(htHopDongSelected);
                $("#htHopDong").html(htHopDongSelect);
            }

            if (data.phuongThucLuaChonNT != null) {
                arr_PhuongThucChonNhaThau = data.phuongThucLuaChonNT;
                var ptDauThauSelected = $("#ptDauThauSelected").val();
                var ptChonNhaThauSelect = CreateHtmlSelectPhuongThucChonNhaThau(ptDauThauSelected);
                $("#ptDauThau").html(ptChonNhaThauSelect);
            }

        }
    });
}

function CreateHtmlSelectHinhThucChonNhaThau(value) {
    var htmlOption = "";
    arr_HinhThucChonNhaThau.forEach(x => {
        if (value != undefined && value == x.Value)
            htmlOption += "<option value='" + x.Value + "' selected>" + x.Text + "</option>";
        else
            htmlOption += "<option value='" + x.Value + "'>" + x.Text + "</option>";
    })
    return "<select disabled id='htChonNhaThau' class='form-control'>" + htmlOption + "</option>";
}

function CreateHtmlSelectPhuongThucChonNhaThau(value) {
    var htmlOption = "";
    arr_PhuongThucChonNhaThau.forEach(x => {
        if (value != undefined && value == x.Value)
            htmlOption += "<option value='" + x.Value + "' selected>" + x.Text + "</option>";
        else
            htmlOption += "<option value='" + x.Value + "'>" + x.Text + "</option>";
    })
    return "<select disabled id='ptChonNhaThau' class='form-control'>" + htmlOption + "</option>";
}

function CreateHtmlSelectLoaiHopDong(value) {
    var htmlOption = "";
    arr_LoaiHopDong.forEach(x => {
        if (value != undefined && value == x.Value)
            htmlOption += "<option value='" + x.Value + "' selected>" + x.Text + "</option>";
        else
            htmlOption += "<option value='" + x.Value + "'>" + x.Text + "</option>";
    })
    return "<select disabled id='htHopDongSelect' class='form-control'>" + htmlOption + "</option>";
}

function GetThongTinNguonVonChiPhiGoiThau() {
    var goiThauId = $("#iID_GoiThauID").val();
    $.ajax({
        url: "/QLVonDauTu/QLThongTinGoiThau/GetListChiTietGoiThau",
        type: "POST",
        data: { id: goiThauId },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data.nguonvon != null && data.nguonvon != undefined) {
                arrNguonVon = data.nguonvon;
                LoadViewNguonVon();
            }
            if (data.chiphi != null && data.chiphi != undefined) {
                arrChiPhi = data.chiphi;
                UpdateChiPhiCanEdit(arrChiPhi);
                LoadViewChiPhiByGoiThau();
            }
            if (data.hangmuc != null && data.hangmuc != undefined) {
                arrHangMuc = data.hangmuc;
            }
            if (data.hopdong != null && data.hopdong != undefined) {
                arrHopDong = data.hopdong;
                LoadHopDong();
            }
        },
        error: function (data) {

        }
    })
}

function LoadHopDong() {
    var htmlHopDong = "";
    arrHopDong.forEach(function (x) {
        htmlHopDong += "<tr>";
        htmlHopDong += "<td>" + x.SSoHopDong + "</td>";
        htmlHopDong += "<td>" + x.NgayHopDong + "</td>";
        htmlHopDong += "<td>" + (!x.SHinhThucHopDong ? "" : x.SHinhThucHopDong) + "</td>";
        htmlHopDong += "<td>" + (!x.STenNhaThau ? "" : x.STenNhaThau) + "</td>";

        htmlHopDong += "<td contenteditable='true' class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTri) + "</td>";
        /*htmlNguonVon += "<td align='right' class='r_fGiaPheDuyetNV'>" + FormatNumber(x.fTienPheDuyet) + "</td>";*/
        htmlHopDong += "</tr>";
    });
    $("#tblDanhSachHopDong tbody").html(htmlHopDong);
}

function LoadViewNguonVon() {
    var htmlNguonVon = "";
    arrNguonVon.forEach(function (x) {
        htmlNguonVon += "<tr>";
        htmlNguonVon += "<td>" + x.sTenNguonVon + "</td>";
   /*     htmlNguonVon += "<td align='right' class='r_fGiaPheDuyetNV'>" + FormatNumber(x.fTienPheDuyet) + "</td>";*/
        htmlNguonVon += "<td contenteditable='true' class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.fTienGoiThau) + "</td>";

        htmlNguonVon += "</tr>";
    });

    $("#tblDanhSachNguonVon tbody").html(htmlNguonVon);
}

function LoadViewChiPhiByGoiThau() {
    $("#tblChiPhiDauTu tbody").html("");
    var html = "";
    arrChiPhi.forEach(x => {

        var duanChiPhiId = x.iID_ChiPhiID;

        //var duanChiPhiString = duanChiPhiId.toString();
        // var stt = count;
        html += '<tr>';
        html += '<input type="hidden" class="r_IsEdit" value = "' + x.isEdit + '"/> <input type="hidden" class="r_ID" value = "' + x.Id + '"/>';
        html += '<td style="font-weight:bold;text-align:left;" > ' + (!x.sTenChiPhi ? "" : x.sTenChiPhi) + ' </td> ';
      /*  html += '<td style="text-align:right"><input type="text" disabled style="text-align: right" class="form-control txtGiaTriChiPhi" onkeyup="ValidateNumberKeyUp(this); " onkeypress="return ValidateNumberKeyPress(this, event); " value="' + FormatNumber(x.fTienPheDuyet) + '"/> </td>';*/
        html += '<td style="text-align:right"> ' + FormatNumber(x.fTienGoiThau) + ' </td>';

        html += "<td class='width-50' align='center'>";
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


function UpdateChiPhiCanEdit(arrayChiPhiParam) {

    arrayChiPhiParam.forEach(obj => {
        var checkIsParent = CheckChiPhiIsParent(obj.iID_ChiPhiID);
        if (checkIsParent) {
            obj.isEdit = false;
        } else {
            obj.isEdit = true;
        }

        if (obj.iID_ChiPhi_Parent == null) {
            obj.iID_ChiPhi_Parent = "";
        }

    });
}

function CheckChiPhiIsParent(itemId) {

    var listChild = [];
    listChild = arrChiPhi.filter(function (x) { return x.iID_ChiPhi_Parent == itemId });
    if (listChild.length > 0) {
        return true;
    } else {
        return false;
    }
}

function ViewDetail(iidChiPhi) {

    LoadViewHangMuc();
}

function LoadViewHangMuc() {
    if (arrHangMuc.length < 1) {
        return;
    }
    data = arrHangMuc
    $("#tblHangMucChinh tbody").html("");
    //var data = GetListDataHangMuc();
    for (var i = 0; i < data.length; i++) {
        var dongMoi = "";
        dongMoi += "<tr style='cursor: pointer;' class='parent'>";

        dongMoi += "<td class='r_STT width-50'>" + data[i].maOrder + "</td><input type='hidden' id='" + data[i].iID_DuToan_DM_HangMucID + "' class='r_iID_DuAn_HangMucID' value='" + data[i].iID_DuToan_DM_HangMucID + "'/> <input type='hidden' class='r_HangMucID' value='" + data[i].iID_HangMucID + "'/> <input type='hidden' class='r_HangMucParentID' value='" + data[i].iID_ParentID + "'/> <input type='hidden' class='r_IsEdit' value='" + data[i].isEdit + "'/>";
        dongMoi += "<td>" + (!data[i].sTenHangMuc ? "" : data[i].sTenHangMuc) + "</td>"

        dongMoi += "<td align='right'> " + FormatNumber(data[i].fTienGoiThau) + "</td>";

        dongMoi += "</tr>";

        $("#tblHangMucChinh tbody").append(dongMoi);
    }
}



function Huy() {
    window.location.href = "/QLVonDauTu/QLThongTinGoiThau/Index";
}
