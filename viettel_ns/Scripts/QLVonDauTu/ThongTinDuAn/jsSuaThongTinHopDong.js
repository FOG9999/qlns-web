var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var HOP_DONG_GIAO_VIEC = 0;
var HOP_DONG_KINH_TE = 1;

$(document).ready(function () {
    $("#iID_LoaiHopDongID").val($("#iID_LoaiHopDongID").val());
    $("#iID_NhaThauID").val($("#iIDNhaThauId").val());
    if ($("#iIDGoiThauId").val() == "" || $("#iIDGoiThauId").val() == GUID_EMPTY)
        $("#cboLoaiHopDong").val(HOP_DONG_GIAO_VIEC);
    else
        $("#cboLoaiHopDong").val(HOP_DONG_KINH_TE);
  //  DinhDangSo();

    GetListNhaThau();
    LoadGoiThauDb();
    SetArrChiPhi();
    SetArrHangMuc();
    $("#iThoiGianThucHien").keydown(function (event) {
        ValidateInputKeydown(event, this, 1);
    }).blur(function (event) {
        ValidateInputFocusOut(event, this, 1);
    });
    $("#sSoHopDong").keyup(function (event) {
        ValidateMaxLength(this, 100);
    });
    $("#sTenHopDong").keyup(function (event) {
        ValidateMaxLength(this, 300);
    });
    $("#sHinhThucHopDong").keyup(function (event) {
        ValidateMaxLength(this, 300);
    });
    $("#idNoiDungHopDong").keyup(function (event) {
        ValidateMaxLength(this, 300);
    });
    ChangeNhaThau();

    $("#fGiaTriHopDong").val(FormatNumber($("#fGiaTriHopDong").val()));
});

function DinhDangSo() {
    $(".sotien").each(function () {
        if ($(this).is('input'))
            $(this).val(FormatNumber(UnFormatNumber($(this).val())));
        else
            $(this).html(FormatNumber(UnFormatNumber($(this).html())));
    })
}

function CheckLoi(hopDong) {
    var messErr = [];
    var thoiGianThucHien = $("#iThoiGianThucHien").val();
    if (CheckTrungSoHopDong(hopDong.sSoHopDong)) {
        sMessError.push("Trùng số hợp đồng, vui lòng nhập số khác");
    }
    if (hopDong.sSoHopDong == "")
        messErr.push("Số hợp đồng không được để trống");
    if (hopDong.sTenHopDong == "")
        messErr.push("Tên hợp đồng không được để trống");
    if (hopDong.dNgayHopDong == "")
        messErr.push("Ngày hợp đồng không được để trống hoặc sai định dạng");
    if (hopDong.iThoiGianThucHien == "" || isNaN(hopDong.iThoiGianThucHien))
        messErr.push("Thời gian thực hiện không được để trống");
    else if (thoiGianThucHien != hopDong.iThoiGianThucHien || hopDong.iThoiGianThucHien <= 0) {
        messErr.push("Số ngày thực hiện phải là số nguyên lớn hơn 0");
    }
    if (hopDong.fTienHopDong == "" || isNaN(hopDong.fTienHopDong))
        messErr.push("Giá trị hợp đồng không được để trống");

    if (messErr.length > 0) {
        alert(messErr.join("\n"));
        return false;
    } else {
        return true;
    }
}

function CheckTrungSoHopDong(sSoHopDong) {
    var check = false;
    $.ajax({
        url: "/QLVonDauTu/Common/KiemTraTrung",
        type: "POST",
        data: { sTable: "NH_DA_HopDong", sColumn: "sSoHopDong", val: sSoHopDong },
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

var arrPhuLucChiPhi = [];
function EventCheckboxChiPhi(idGoiThauNhaThauValue) {
    $(".cb_ChiPhi").change(function () {
        iID_ChiPhi_select = $(this).data('chiphiid');
        //idGoiThauNhaThau = $(this).data('idgoithaunhathau');
        tiengoithau = $(this).data('tiengoithau');
        if (this.checked) {
            $('#btn_chitiet_chiphi_' + iID_ChiPhi_select).removeAttr('disabled');
            arrPhuLucChiPhi.push({ IdGoiThauNhaThau: idGoiThauNhaThauValue, IIDChiPhiID: iID_ChiPhi_select, FTienGoiThau: tiengoithau })
        } else {
            $('#btn_chitiet_chiphi_' + iID_ChiPhi_select).attr("disabled", true);
            var newArray = arrPhuLucChiPhi.filter(function (el) {
                //return (el.chiphiid != iID_ChiPhi_select)
                return (el.IdGoiThauNhaThau == idGoiThauNhaThauValue && el.IIDChiPhiID != iID_ChiPhi_select)
            });
            arrPhuLucChiPhi = newArray;
        }
    });
}


function ShowChiPhi(id, idGoiThauNhaThau) {
    $('#txtCurrentGoiThauSelected').val(id);
    $('#txtIdGoiThauNhaThau').val(idGoiThauNhaThau);
    var hopDongId = $('#txtHopDongId').val();
    $.ajax({
        url: "/QLVonDauTu/QLThongTinHopDong/LayThongTinChiPhiPhuLuc",
        type: "POST",
        data: { goiThauid: id, idGoiThauNhaThau: idGoiThauNhaThau, hopDongId: hopDongId },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null && data != "") {
                if (data != null) {
                    var htmlChiPhi = "";
                    data.forEach(function (x) {
                        var newArray = arrPhuLucChiPhi.filter(function (el) {
                            //return (el.chiphiid != iID_ChiPhi_select)
                            return (el.IdGoiThauNhaThau == idGoiThauNhaThau && el.IIDChiPhiID == x.IIDChiPhiID)
                        });

                        htmlChiPhi += "<tr>";
                        if (newArray.length > 0) {
                            htmlChiPhi += "<td align='center'> <input type='checkbox' checked data-tiengoithau ='" + x.FTienGoiThau + "' data-goithauid='" + id + "' data-chiphiid='" + x.IIDChiPhiID + "' class='cb_ChiPhi'></td>";
                        } else {
                            htmlChiPhi += "<td align='center'> <input type='checkbox' data-tiengoithau ='" + x.FTienGoiThau + "' data-goithauid='" + id + "' data-chiphiid='" + x.IIDChiPhiID + "' class='cb_ChiPhi'></td>";
                        }

                        htmlChiPhi += "<td>" + x.STenChiPhi + "</td>";
                        htmlChiPhi += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriDuocDuyet) + "</td>";
                        htmlChiPhi += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriConLai) + "</td>";
                        if (newArray.length > 0) {
                            htmlChiPhi += "<td align='center'> <button id='btn_chitiet_chiphi_" + x.IIDChiPhiID + "' style='width: 120px !important' onclick = ShowHangMuc('" + x.IIDChiPhiID + "') class='btn btn-primary btnShowHangMuc'><span>Chi tiết hạng mục</span></button>" +
                                "</td > ";
                        } else {
                            htmlChiPhi += "<td align='center'> <button id='btn_chitiet_chiphi_" + x.IIDChiPhiID + "' style='width: 120px !important' disabled onclick = ShowHangMuc('" + x.IIDChiPhiID + "') class='btn btn-primary btnShowHangMuc'><span>Chi tiết hạng mục</span></button>" +
                                "</td > ";
                        }
                        htmlChiPhi += "</tr>";
                    });
                    $("#tblDanhSachPhuLucChiPhi tbody").html(htmlChiPhi);
                    EventCheckboxChiPhi(idGoiThauNhaThau);
                }
            } else {
                $("#tblDanhSachPhuLucChiPhi tbody").html('');
            }
            $("#tblDanhSachPhuLucHangMuc tbody").html('');
        },
        error: function (data) {

        }
    })
}

function ShowChiPhiDb(id, idGoiThauNhaThau) {
    $('#txtCurrentGoiThauSelected').val(id);
    $('#txtIdGoiThauNhaThau').val(idGoiThauNhaThau);
    var hopDongId = $('#txtHopDongId').val();
    $.ajax({
        url: "/QLVonDauTu/QLThongTinHopDong/LayThongTinChiPhiPhuLucByHopDongId",
        type: "POST",
        data: { hopdongId: hopDongId },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null && data != "") {
                if (data != null) {
                    var htmlChiPhi = "";
                    data.forEach(function (x) {
                        var newArray = arrPhuLucChiPhi.filter(function (el) {
                            //return (el.chiphiid != iID_ChiPhi_select)
                            return (el.IdGoiThauNhaThau == idGoiThauNhaThau && el.IIDChiPhiID == x.IIDChiPhiID)
                        });
                        if (newArray.length > 0 && x.IdGoiThauNhaThau == idGoiThauNhaThau) {
                            htmlChiPhi += "<tr>";
                            if (newArray.length > 0) {
                                htmlChiPhi += "<td align='center'> <input type='checkbox' checked data-tiengoithau ='" + x.FTienGoiThau + "' data-goithauid='" + id + "' data-chiphiid='" + x.IIDChiPhiID + "' class='cb_ChiPhi'></td>";
                            }
                            else {
                                htmlChiPhi += "<td align='center'> <input type='checkbox' data-tiengoithau ='" + x.FTienGoiThau + "' data-goithauid='" + id + "' data-chiphiid='" + x.IIDChiPhiID + "' class='cb_ChiPhi'></td>";
                            }

                            htmlChiPhi += "<td>" + x.STenChiPhi + "</td>";
                            htmlChiPhi += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriDuocDuyet) + "</td>";
                            htmlChiPhi += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriConLai) + "</td>";
                            if (newArray.length > 0) {
                                htmlChiPhi += "<td align='center'> <button id='btn_chitiet_chiphi_" + x.IIDChiPhiID + "' style='width: 120px !important' onclick = ShowHangMucDb('" + x.IIDChiPhiID + "') class='btn btn-primary btnShowHangMuc'><span>Chi tiết hạng mục</span></button>" +
                                    "</td > ";
                            } else {
                                htmlChiPhi += "<td align='center'> <button id='btn_chitiet_chiphi_" + x.IIDChiPhiID + "' style='width: 120px !important' disabled onclick = ShowHangMucDb('" + x.IIDChiPhiID + "') class='btn btn-primary btnShowHangMuc'><span>Chi tiết hạng mục</span></button>" +
                                    "</td > ";
                            }
                            htmlChiPhi += "</tr>";
                        }
                    });
                    $("#tblDanhSachPhuLucChiPhi tbody").html(htmlChiPhi);
                    EventCheckboxChiPhi(idGoiThauNhaThau);
                }
            } else {
                $("#tblDanhSachPhuLucChiPhi tbody").html('');
                alert("Không có thông tin chi phí cho gói thầu này");
            }
            $("#tblDanhSachPhuLucHangMuc tbody").html('');
        },
        error: function (data) {

        }
    })
}

var arrPhuLucHangMuc = [];
function EventCheckboxHangMuc() {
    var goiThauId = $('#txtCurrentGoiThauSelected').val();
    var idGoiThauNhaThau = $('#txtIdGoiThauNhaThau').val();
    $(".cb_HangMuc").change(function () {
        hangMucId = $(this).data('hangmucid');
        chiPhiId = $(this).data('chiphiid');
        tienHangMuc = $(this).data('tienhangmuc');
        hangmucparent = $(this).data('hangmucparent');
        idfake = $(this).data('idfake');
        if (this.checked) {
            arrPhuLucHangMuc.push({ IdGoiThauNhaThau: idGoiThauNhaThau, IIDChiPhiID: chiPhiId, IIDHangMucID: hangMucId, FTienGoiThau: tienHangMuc, idFake: idfake, HangMucParentId: hangmucparent })
            if (!hangmucparent) {
                checkHangMucParentThenCheckChildren(idGoiThauNhaThau, hangMucId, true);
            } else {
                checkAllHangMucChildrenThenCheckParent(idGoiThauNhaThau, hangMucId, hangmucparent, true);
            }
        } else {
            var newArray = arrPhuLucHangMuc.filter(function (el) {
                return (el.IdGoiThauNhaThau == idGoiThauNhaThau && el.IIDChiPhiID == chiPhiId && el.IIDHangMucID != hangMucId && el.idFake != idfake)
            });
            arrPhuLucHangMuc = newArray;
            if (!hangmucparent) {
                checkHangMucParentThenCheckChildren(idGoiThauNhaThau, hangMucId, false);
            } else {
                checkAllHangMucChildrenThenCheckParent(idGoiThauNhaThau, hangMucId, hangmucparent, false);
            }
        }
        SumGiaTriTrungThauChiPhi(chiPhiId);
    });
}

function SetArrHangMuc() {
    var hopDongId = $('#txtHopDongId').val();
    var isGoc = $('#txtIsGoc').val();
    $.ajax({
        url: "/QLVonDauTu/QLThongTinHopDong/LayThongTinHangMucByHopDongId",
        type: "POST",
        data: { hopdongId: hopDongId, isGoc: isGoc },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null && data != "") {
                if (data != null) {
                    data.forEach(function (x) {
                        arrPhuLucHangMuc.push({ IdGoiThauNhaThau: x.IdGoiThauNhaThau, IIDChiPhiID: x.IIDChiPhiID, IIDHangMucID: x.IIDHangMucID, FTienGoiThau: x.FTienGoiThau, idFake: x.IdFake })
                    });
                }
            } else {
            }
        },
        error: function (data) {

        }
    })
}

// recalculate gia tri trung thau chi phi
function SumGiaTriTrungThauChiPhi(chiphiId) {
    var rowChiPhi = $("#tblDanhSachPhuLucChiPhi tbody").find('tr#' + chiphiId);
    var indexOfChiphi = arrPhuLucChiPhi.map(cp => cp.IIDChiPhiID).indexOf(chiphiId);
    if (indexOfChiphi >= 0) {
        arrPhuLucChiPhi[indexOfChiphi].FTienGoiThau = arrPhuLucHangMuc.filter(hm => hm.IIDChiPhiID === chiphiId).map(cp => cp.FTienGoiThau)
            .reduce((pre, curr) => pre + curr, 0);
        rowChiPhi.find('.r_fGiaTrungThau').html(FormatNumber(arrPhuLucChiPhi[indexOfChiphi].FTienGoiThau));
    }
}


// if check parent -> check all children, uncheck parent -> uncheck all children
function checkHangMucParentThenCheckChildren(idGoiThauNhaThau, hangmucParentId, parentCheckboxStatus) {
    var allHangMucRows = $("#tblDanhSachPhuLucHangMuc tbody").children();
    allHangMucRows.each((index, row) => {
        var rowParentId = $(row).find('input:checkbox').data('hangmucparent');
        var hangMucId = $(row).find('input:checkbox').data('hangmucid');
        var chiPhiId = $(row).find('input:checkbox').data('chiphiid');
        var tienHangMuc = $(row).find('input:checkbox').data('tienhangmuc');
        var idfake = $(row).find('input:checkbox').data('idfake');
        if (hangmucParentId && hangmucParentId === rowParentId) {
            if (parentCheckboxStatus) {
                $(row).find('input:checkbox').prop('checked', true);
                // $(row).find('input:checkbox').change();
                arrPhuLucHangMuc.push({ IdGoiThauNhaThau: idGoiThauNhaThau, IIDChiPhiID: chiPhiId, IIDHangMucID: hangMucId, FTienGoiThau: tienHangMuc, idFake: idfake, HangMucParentId: rowParentId })
            }
            else {
                $(row).find('input:checkbox').prop('checked', false);
                // $(row).find('input:checkbox').change();
                var newArray = arrPhuLucHangMuc.filter(function (el) {
                    return (el.IdGoiThauNhaThau == idGoiThauNhaThau && el.IIDChiPhiID == chiPhiId && el.IIDHangMucID != hangMucId && el.idFake != idfake)
                });
                arrPhuLucHangMuc = newArray;
            }
        }
    })
}

// check all children -> check parent, uncheck all children -> uncheck parent
function checkAllHangMucChildrenThenCheckParent(idGoiThauNhaThau, hangmucChildId, hangmucParentId, childCheckboxStatus) {
    var allHangMucRows = $("#tblDanhSachPhuLucHangMuc tbody").children();
    var currentParentId =
        allHangMucRows.each((index, row) => {
            var rowParentId = $(row).find('input:checkbox').data('hangmucparent');
            var hangMucId = $(row).find('input:checkbox').data('hangmucid');
            var chiPhiId = $(row).find('input:checkbox').data('chiphiid');
            var tienHangMuc = $(row).find('input:checkbox').data('tienhangmuc');
            var idfake = $(row).find('input:checkbox').data('idfake');

            if (hangMucId == hangmucParentId) {
                // if uncheck child -> uncheck parent if checked
                if (!childCheckboxStatus) {
                    $(row).find('input:checkbox').prop('checked', false);
                    var newArray = arrPhuLucHangMuc.filter(function (el) {
                        return (el.IdGoiThauNhaThau == idGoiThauNhaThau && el.IIDChiPhiID == chiPhiId && el.IIDHangMucID != hangMucId && el.idFake != idfake)
                    });
                    arrPhuLucHangMuc = newArray;
                }
                // check parent if all children checked
                else {
                    // find all children
                    var allChildrenWithSameParent = allHangMucRows.filter((ind, hm) => {
                        var parent = $(hm).find('input:checkbox').data('hangmucparent');
                        return parent == hangMucId;
                    })
                    var isAllChildrenChecked =
                        allChildrenWithSameParent.filter((ind, hm) => $(hm).find('input:checkbox').prop('checked')).length ===
                        allChildrenWithSameParent.length;
                    if (isAllChildrenChecked) {
                        $(row).find('input:checkbox').prop('checked', true);
                        arrPhuLucHangMuc.push({ IdGoiThauNhaThau: idGoiThauNhaThau, IIDChiPhiID: chiPhiId, IIDHangMucID: hangMucId, FTienGoiThau: tienHangMuc, idFake: idfake, HangMucParentId: null })
                    }
                }
            }
        })
}

function ShowHangMucDb(id) {
    var hopDongId = $('#txtHopDongId').val();
    var isGoc = $('#txtIsGoc').val();
    var goiThauId = $('#txtCurrentGoiThauSelected').val();
    var idGoiThauNhaThau = $('#txtIdGoiThauNhaThau').val();
    $.ajax({
        url: "/QLVonDauTu/QLThongTinHopDong/LayThongTinHangMucByHopDongId",
        type: "POST",
        data: { hopdongId: hopDongId, isGoc: isGoc },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null && data != "") {
                if (data != null) {
                    var htmlChiPhi = "";
                    data.forEach(function (x) {
                        var newArray = arrPhuLucHangMuc.filter(function (el) {
                            return (el.IdGoiThauNhaThau == idGoiThauNhaThau && el.IIDChiPhiID == id && el.IIDHangMucID == x.IIDHangMucID && el.idFake == x.IdFake)
                        });
                        if (newArray.length > 0) {
                            htmlChiPhi += "<tr>";
                            if (newArray.length > 0) {
                                htmlChiPhi += "<td align='center'> <input type='checkbox' checked data-idfake= '" + x.IdFake + "' data-tienhangmuc= '" + x.FTienGoiThau + "' data-chiphiid='" + id + "' data-hangmucid='" + x.IIDHangMucID + "' data-hangmucparent='" + x.HangMucParentId + "' class='cb_HangMuc'></td>";
                            } else {
                                htmlChiPhi += "<td align='center'> <input type='checkbox' data-idfake= '" + x.IdFake + "' data-tienhangmuc= '" + x.FTienGoiThau + "' data-chiphiid='" + id + "' data-hangmucid='" + x.IIDHangMucID + "' data-hangmucparent='" + x.HangMucParentId + "' class='cb_HangMuc'></td>";
                            }
                            htmlChiPhi += "<td>" + x.MaOrDer + "</td>";
                            htmlChiPhi += "<td>" + x.STenHangMuc + "</td>";
                            htmlChiPhi += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriDuocDuyet) + "</td>";
                            htmlChiPhi += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriConLai) + "</td>";
                            htmlChiPhi += "</tr>";
                        }
                    });
                    $("#tblDanhSachPhuLucHangMuc tbody").html(htmlChiPhi);
                    EventCheckboxHangMuc();
                }
            } else {
                $("#tblDanhSachPhuLucHangMuc tbody").html('');
            }
        },
        error: function (data) {

        }
    })
}

function ShowHangMuc(id) {
    var goiThauId = $('#txtCurrentGoiThauSelected').val();
    var idGoiThauNhaThau = $('#txtIdGoiThauNhaThau').val();
    $.ajax({
        url: "/QLVonDauTu/QLThongTinHopDong/LayThongTinHangMucPhuLuc",
        type: "POST",
        data: { iID_DuAnID: $("#txtDuAnId").val(), hopDongId: '', chiphiId: id},
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null && data != "") {
                if (data != null) {
                    var htmlChiPhi = "";
                    data.forEach(function (x) {

                        var newArray = arrPhuLucHangMuc.filter(function (el) {
                            return (el.IdGoiThauNhaThau == idGoiThauNhaThau && el.IIDChiPhiID == id && el.IIDHangMucID == x.IIDHangMucID && el.idFake == x.IdFake)
                        });

                        htmlChiPhi += "<tr>";
                        if (newArray.length > 0) {
                            htmlChiPhi += "<td align='center'> <input type='checkbox' checked data-idfake= '" + x.IdFake + "' data-tienhangmuc= '" + x.FTienGoiThau + "' data-chiphiid='" + id + "' data-hangmucid='" + x.IIDHangMucID + "' class='cb_HangMuc'></td>";
                        } else {
                            htmlChiPhi += "<td align='center'> <input type='checkbox' data-idfake= '" + x.IdFake + "' data-tienhangmuc= '" + x.FTienGoiThau + "' data-chiphiid='" + id + "' data-hangmucid='" + x.IIDHangMucID + "' class='cb_HangMuc'></td>";
                        }

                        htmlChiPhi += "<td>" + x.MaOrDer + "</td>";
                        htmlChiPhi += "<td>" + x.STenHangMuc + "</td>";
                        htmlChiPhi += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriDuocDuyet) + "</td>";
                        htmlChiPhi += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriConLai) + "</td>";
                        htmlChiPhi += "</tr>";
                    });

                    $("#tblDanhSachPhuLucHangMuc tbody").html(htmlChiPhi);
                    EventCheckboxHangMuc();
                }
            } else {
                $("#tblDanhSachPhuLucHangMuc tbody").html('');
                alert("Không có thông tin hạng mục của chi phí này")
            }
        },
        error: function (data) {

        }
    })
}

var sttGoiThau = 1;
var arrGoiThau = [];
function EventCheckboxGoiThau() {
    $(".cb_DuToan").change(function () {
        iID_GoiThau_select = $(this).data('goithauid');
        var sttGoiThau = $(this).data('sttgoithau');
        var idDropDown = '#dropdown_' + sttGoiThau + "_" + iID_GoiThau_select;
        var dropDownnNhaThau = $(this).data('nhathauid');
        var nhaThauId = $('#' + dropDownnNhaThau).val();
        var giatrigoithau = $(this).data('giatrigoithau');
        var giatrihopdong = $(this).data('giatrihopdong');
        var giatritrungthau = $(this).data('giatritrungthau');
        var idcreate = $(this).data('idcreate');
        var idButton = iID_GoiThau_select + '_' + sttGoiThau;
        if (this.checked) {
            arrGoiThau.push({ Id: idcreate, stt: sttGoiThau, IIDGoiThauID: iID_GoiThau_select, IIdNhaThauId: nhaThauId, fGiaTriGoiThau: giatrigoithau, fGiaTriHD: giatrihopdong, FGiaTriTrungThau: giatritrungthau })
            $('#btn_chitiet_' + idButton).removeAttr('disabled');
            $('#btn_ShowChiPhi_' + idButton).removeAttr('disabled');
        } else {
            var newArray = arrGoiThau.filter(function (el) {
                return (el.IIDGoiThauID != iID_GoiThau_select)
            });
            arrGoiThau = newArray;
            $('#btn_chitiet_' + idButton).attr("disabled", true);
            $('#btn_ShowChiPhi_' + idButton).attr("disabled", true);
        }
    });
}

function UpdateItemNhaThau(item) {
    var goithauid = $(item).data('goithauid');
    var sttgoithau = $(item).data('sttgoithau');
    $.each(arrGoiThau, function () {
        if (this.stt == sttgoithau && this.IIDGoiThauID == goithauid) {
            this.IIdNhaThauId = $(item).val();
        }
    });
}

var arrNhaThau = [];
function GetListNhaThau() {
    var hopDongId = $('#txtHopDongId').val();
    $.ajax({
        url: "/QLVonDauTu/QLThongTinHopDong/GetListNhaThau",
        type: "POST",
        data: {},
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null && data != "") {
                if (data != null) {
                    data.forEach(function (x) {
                        arrNhaThau.push({ iID_NhaThauID: x.iID_NhaThauID, STenNhaThau: x.sTenNhaThau })
                    });
                }
            } else {

            }
        },
        error: function (data) {

        }
    })
}

function SetArrChiPhi() {
    var hopDongId = $('#txtHopDongId').val();
    $.ajax({
        url: "/QLVonDauTu/QLThongTinHopDong/LayThongTinChiPhiPhuLucByHopDongId",
        type: "POST",
        data: { hopdongId: hopDongId },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null && data != "") {
                if (data != null) {
                    //var htmlChiPhi = "";
                    data.forEach(function (x) {
                        arrPhuLucChiPhi.push({ IdGoiThauNhaThau: x.IdGoiThauNhaThau, IIDChiPhiID: x.IIDChiPhiID, FTienGoiThau: x.FTienGoiThau })
                    });
                    //$("#tblDanhSachPhuLucChiPhi tbody").html(htmlChiPhi);
                    //EventCheckboxChiPhi(idGoiThauNhaThau);
                }
            } else {
                // $("#tblDanhSachPhuLucChiPhi tbody").html('');
            }
            // $("#tblDanhSachPhuLucHangMuc tbody").html('');
        },
        error: function (data) {

        }
    })
}

function CreateDropDownNhaThau(nhathauId) {
    var html = '';
    arrNhaThau.forEach(function (x) {
        if (x.iID_NhaThauID == nhathauId) {
            html += '<option selected value = "' + x.iID_NhaThauID + ' ">' + x.STenNhaThau + '</option>';
        } else {
            html += '<option value = "' + x.iID_NhaThauID + '">' + x.STenNhaThau + '</option>';
        }
    });
    return html;
}

function LoadGoiThauDb() {
    var hopDongId = $('#txtHopDongId').val();
    $.ajax({
        url: "/QLVonDauTu/QLThongTinHopDong/LayThongTinChiTietGoiThauDb",
        type: "POST",
        data: { hopDongId: hopDongId },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null && data != "") {
                //var dropDownValue = $('#iID_NhaThauID').html();
                if (data.goithau != null) {
                    var htmlGoiThau = "";
                    data.goithau.forEach(function (x) {
                        var dropDownValue = CreateDropDownNhaThau(x.IIdNhaThauId);
                        arrGoiThau.push({ Id: x.Id, stt: sttGoiThau, IIDGoiThauID: x.IIDGoiThauID, IIdNhaThauId: x.IIdNhaThauId, fGiaTriGoiThau: x.fGiaTriGoiThau, fGiaTriHD: x.fGiaTriHD, FGiaTriTrungThau: x.FGiaTriTrungThau })
                        var idDropDown = 'dropdown_' + sttGoiThau + "_" + x.IIDGoiThauID;
                        htmlGoiThau += "<tr>";
                        htmlGoiThau += "<td align='center'> <input type='checkbox' checked data-giatrigoithau = '" + x.fGiaTriGoiThau + "' data-giatritrungthau = '" + x.FGiaTriTrungThau + "' data-giatrihopdong = '" + x.fGiaTriHD + "' data-nhathauid = '" + idDropDown + "' data-sttgoithau = '" + sttGoiThau + "' data-goithauid='" + x.IIDGoiThauID + "' class='cb_DuToan'></td>";
                        htmlGoiThau += "<td>" + x.STenGoiThau + "</td>";
                        htmlGoiThau += "<td> <select id='" + idDropDown + "' data-sttgoithau = '" + sttGoiThau + "' onchange='UpdateItemNhaThau($(this))' data-goithauid='" + x.IIDGoiThauID + "' class='form-control dropdown_nhathau'> " + dropDownValue + " </select></td>";

                        htmlGoiThau += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FTienTrungThau) + "</td>";
                        htmlGoiThau += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.fGiaTriGoiThau) + "</td>";
                        htmlGoiThau += "<td class='fGiaTriGoiThau r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriTrungThau) + "</td>";
                        htmlGoiThau += "<td class='fGiaTriHD r_fGiaTriHD sotien' align='right'>" + FormatNumber(x.fGiaTriHD) + "</td>";
                        htmlGoiThau += "<td class='r_giaTriConLai' align='right'>" + FormatNumber(x.FTienTrungThau - x.FGiaTriTrungThau) + "</td>";
                        var idButton = x.IIDGoiThauID + '_' + sttGoiThau;
                        htmlGoiThau += "<td> <button id='btn_chitiet_" + idButton + "' onclick = ShowChiPhiDb('" + x.IIDGoiThauID + "'" + ",'" + x.Id + "')  title='Chi phí chi tiết' class='btn btn-detail btn-icon btnShowGoiThau'><i class='fa fa-eye fa-lg'></i></button>" +
                            "<button id='btn_ShowChiPhi_" + idButton + "' data-goithauid='" + x.IIDGoiThauID + "' data-goithaunhathauid='" + x.Id + "' onclick='AddRowGoiThau(this)' class='btn btn-edit btn-icon btnShowGoiThau'><i class='fa fa-plus fa-lg'></i></button>" +
                            "<button id='btn_XoaRowGoiThau_" + idButton + "' data-goithauid = '" + x.IIDGoiThauID + "' data-goithaunhathauid = '" + x.Id + "' onclick=\"XoaRowGoiThau(this," + `\'${idButton}\'` +")\" class='btn btn-delete btn-icon btnShowGoiThau'><i class='fa fa-trash-o fa-lg'></i></button>"
                        "</td > ";
                        htmlGoiThau += "</tr>";
                        sttGoiThau++;
                    });
                    $("#tblDanhSachGoiThau tbody").html(htmlGoiThau);
                    CaculatorGiaTriHopDong();
                    EventCheckboxGoiThau();
                    $(".dropdown_nhathau").select2({ width: '100%', matcher: FilterInComboBox });
                } else {
                    $("#tblDanhSachGoiThau tbody").html('');
                    arrGoiThau = [];
                    arrPhuLucHangMuc = [];
                    arrPhuLucChiPhi = [];
                }
            } else {
                $("#tblDanhSachGoiThau tbody").html('');
                arrGoiThau = [];
                arrPhuLucHangMuc = [];
                arrPhuLucChiPhi = [];
            }
            if (arrGoiThau.length > 0) SumGiaTriHopDong(); 
        },
        error: function (data) {

        }
    })
}

function LoadGoiThau() {
    $("#sDiaDiem").val("");
    $("#sThoiGianThucHien").val("");
    $("#fTongMucDauTu").val("");
    var hopDongId = $('#txtHopDongId').val();
    var duanId = $('#txtDuAnId').val();
    $.ajax({
        url: "/QLVonDauTu/QLThongTinHopDong/LayThongTinChiTietDuAn",
        type: "POST",
        data: { iID_DuAnID: duanId, hopDongId: hopDongId },
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data != null && data != "") {

                var dropDownValue = $('#iID_NhaThauID').html();

                if (data.goithau != null) {
                    var htmlGoiThau = "";
                    data.goithau.forEach(function (x) {

                        arrGoiThau.push({ stt: sttGoiThau, IIDGoiThauID: x.IIDGoiThauID, IIdNhaThauId: x.IIdNhaThauId, fGiaTriGoiThau: x.fGiaTriGoiThau, fGiaTriHD: x.fGiaTriHD, FGiaTriTrungThau: x.FGiaTriTrungThau })
                        var idDropDown = 'dropdown_' + sttGoiThau + "_" + x.IIDGoiThauID;
                        htmlGoiThau += "<tr>";
                        htmlGoiThau += "<td align='center'> <input type='checkbox' checked data-giatrigoithau = '" + x.fGiaTriGoiThau + "' data-giatritrungthau = '" + x.FGiaTriTrungThau + "' data-giatrihopdong = '" + x.fGiaTriHD  + "' data-nhathauid = '" + idDropDown + "' data-sttgoithau = '" + sttGoiThau + "' data-goithauid='" + x.IIDGoiThauID + "' class='cb_DuToan'></td>";
                        htmlGoiThau += "<td>" + x.STenGoiThau + "</td>";
                        htmlGoiThau += "<td> <select id='" + idDropDown + "' data-sttgoithau = '" + sttGoiThau + "' onchange='UpdateItemNhaThau($(this))' data-goithauid='" + x.IIDGoiThauID + "' class='form-control dropdown_nhathau'> " + dropDownValue + " </select></td>";

                        htmlGoiThau += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FTienTrungThau) + "</td>";
                        htmlGoiThau += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.fGiaTriGoiThau) + "</td>";
                        htmlGoiThau += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriTrungThau) + "</td>";
                        htmlGoiThau += "<td class='r_fGiaTriHD sotien' align='right'>" + FormatNumber(x.fGiaTriHD) + "</td>";
                        htmlGoiThau += "<td>" + "</td>";
                        var idButton = x.IIDGoiThauID + '_' + sttGoiThau;
                        htmlGoiThau += "<td> <button id='btn_chitiet_" + idButton + "' disabled onclick = ShowChiPhi('" + x.IIDGoiThauID + "'" + ",'" + x.Id + "')  title='Chi phí chi tiết' class='btn btn-detail btn-icon btnShowGoiThau'><i class='fa fa-eye fa-lg'></i></button>" +
                            "<button id='btn_ShowChiPhi_" + idButton + "' data-goithauid='" + x.IIDGoiThauID + "' data-goithaunhathauid='" + x.Id + "'  onclick='AddRowGoiThau(this)' disabled class='btn btn-edit btn-icon btnShowGoiThau'><i class='fa fa-plus fa-lg'></i></button>" +
                            "<button id='btn_XoaRowGoiThau_" + idButton + "' data-goithauid = '" + x.IIDGoiThauID + "' data-goithaunhathauid = '" + x.Id + "' onclick=\"XoaRowGoiThau(this," + `\'${idButton}\'` +")\" disabled class='btn btn-delete btn-icon btnShowGoiThau'><i class='fa fa-trash-o fa-lg'></i></button>"
                        "</td > ";
                        htmlGoiThau += "</tr>";

                        sttGoiThau++;
                    });
                    $("#tblDanhSachGoiThau tbody").html(htmlGoiThau);
                    EventCheckboxGoiThau();
                } else {
                    $("#tblDanhSachGoiThau tbody").html('');
                    arrGoiThau = [];
                    arrPhuLucHangMuc = [];
                    arrPhuLucChiPhi = [];
                }
            } else {
                $("#tblDanhSachGoiThau tbody").html('');
                arrGoiThau = [];
                arrPhuLucHangMuc = [];
                arrPhuLucChiPhi = [];
            }
        },
        error: function (data) {

        }
    })

    if (duanId != "") {
        //lay thong tin gói thầu
        $.ajax({
            url: "/QLVonDauTu/QLThongTinHopDong/LayGoiThauTheoDuAnId",
            type: "POST",
            data: { iID_DuAnID: duanId },
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data != "") {
                    $("#iID_GoiThauID").html(data);
                }
            },
            error: function (data) {

            }
        })
    } else {
        $("#iID_GoiThauID option:not(:first)").remove();
        $("#iID_GoiThauID").trigger("change");
    }

}

function Save() {
    var hopDong = {};

    hopDong.iID_HopDongID = $("#iIDHopDongId").val();
    //Thông tin hợp đồng
    hopDong.sSoHopDong = $("#sSoHopDong").val();
    hopDong.sTenHopDong = $("#sTenHopDong").val();
    hopDong.dNgayHopDong = $("#dNgayHopDong").val();
    hopDong.iThoiGianThucHien = parseInt(UnFormatNumber($("#iThoiGianThucHien").val()));
    hopDong.dKhoiCongDuKien = $("#dKhoiCongDuKien").val();
    hopDong.dKetThucDuKien = $("#dKetThucDuKien").val();
    hopDong.iID_LoaiHopDongID = $("#iID_LoaiHopDongID").val();
    hopDong.sHinhThucHopDong = $("#sHinhThucHopDong").val();
    hopDong.fTienHopDong = parseFloat(UnFormatNumber($("#fGiaTriHopDong").val()));
    hopDong.NoiDungHopDong = $("#idNoiDungHopDong").val();
    hopDong.sSoTaiKhoan = $("#sStkNhaThau").val();
    hopDong.sNganHang = $("#sNganHang").val();
    hopDong.dThoiGianBaoLanhHopDongTu = $("#dBaoLanhHDTu").val();
    hopDong.dThoiGianBaoLanhHopDongDen = $("#dBaoLanhHDDen").val();
    //Thông tin gói thầu
    hopDong.iID_NhaThauThucHienID = $("#iID_NhaThauID").val();

    if (CheckLoi(hopDong)) {
        $.ajax({
            url: "/QLVonDauTu/QLThongTinHopDong/Save",
            type: "POST",
            data: { model: hopDong, goithau: arrGoiThau, chiphi: arrPhuLucChiPhi, hangmuc: arrPhuLucHangMuc },
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data.status == true) {
                    alert(data.sMessage);
                    window.location.href = "/QLVonDauTu/QLThongTinHopDong";
                }
            },
            error: function (data) {

            }
        })
    }
}

function CreateGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

function CreateIdGoiThau() {
    var newId = CreateGuid();
    var checkArray = arrGoiThau.filter(function (el) {
        return (el.Id == newId)
    });
    if (checkArray.length > 0) {
        newId = CreateIdGoiThau();
    }
    return newId;
}

function AddRowGoiThau(obj) {
    if (arrGoiThau.length == 0) {
        return;
    }
    sttGoiThau++;
    var goithauSelectedId = $(obj).data('goithauid');
    var goithaunhathauSelectedId = $(obj).data('goithaunhathauid');

    var currentRow = $(obj).closest("tr");
    var rowIndex = $(obj).closest("tr").index();
    var dropDownValue = $('#iID_NhaThauID').html();
    var tengoithau = currentRow.find("td:eq(1)").text();
    var tientrungthau = currentRow.find("td:eq(3)").text();
    var giatrigoithau = currentRow.find("td:eq(4)").text();
    var giatritrungthau = currentRow.find("td:eq(5)").text();
    var giatrihopdong = currentRow.find("td:eq(6)").text();
    var newId = CreateIdGoiThau();
    var idDropDown = 'dropdown_' + sttGoiThau + "_" + newId;
    var htmlGoiThau = "";
    htmlGoiThau += "<tr>";
    htmlGoiThau += "<td align='center'> <input type='checkbox' data-idcreate= '" + newId + "' data-giatrigoithau = '" + giatrigoithau + "' data-giatritrungthau = '" + giatritrungthau + "' data-giatrihopdong = '" + giatrihopdong + "' data-nhathauid = '" + idDropDown + "' data-sttgoithau = '" + sttGoiThau + "' data-goithauid='" + goithauSelectedId + "' class='cb_DuToan'></td>";
    htmlGoiThau += "<td>" + tengoithau + "</td>";
    htmlGoiThau += "<td> <select id='" + idDropDown + "' data-sttgoithau = '" + sttGoiThau + "' onchange='UpdateItemNhaThau($(this))' data-goithauid='" + goithauSelectedId + "' class='form-control dropdown_nhathau'> " + dropDownValue + " </select></td>";
    htmlGoiThau += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(tientrungthau) + "</td>";
    htmlGoiThau += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(giatrigoithau) + "</td>";
    htmlGoiThau += "<td class='fGiaTriGoiThau r_fGiaGoiThau sotien' align='right'>" + FormatNumber(giatritrungthau) + "</td>";
    htmlGoiThau += "<td class='fGiaTriHD r_fGiaTriHD sotien' align='right'>" + FormatNumber(giatrihopdong) + "</td>";
    htmlGoiThau += "<td>" + "</td>";
    var idButton = goithauSelectedId + '_' + sttGoiThau;
    htmlGoiThau += "<td> <button id='btn_chitiet_" + idButton + "' disabled onclick = ShowChiPhi('" + goithauSelectedId + "'" + ",'" + newId + "') title='Chi phí chi tiết' class='btn btn-detail btn-icon btnShowGoiThau'><i class='fa fa-eye fa-lg'></i></button>" +
        "<button id='btn_ShowChiPhi_" + idButton + "' data-goithauid = '" + goithauSelectedId + "' data-goithaunhathauid = '" + newId + "' onclick='AddRowGoiThau(this)' disabled class='btn btn-edit btn-icon btnShowGoiThau'><i class='fa fa-plus fa-lg'></i></button>" +
        "<button id='btn_XoaRowGoiThau_" + idButton + "' data-goithauid = '" + goithauSelectedId + "' data-goithaunhathauid = '" + newId + "' onclick=\"XoaRowGoiThau(this," + `\'${idButton}\'` +")\" disabled class='btn btn-delete btn-icon btnShowGoiThau'><i class='fa fa-trash-o fa-lg'></i></button>"
    "</td> ";
    htmlGoiThau += "</tr>";
    $("#tblDanhSachGoiThau tbody > tr").eq(rowIndex).after(htmlGoiThau);
    EventCheckboxGoiThau();
    SumGiaTriHopDong();
}
function XoaRowGoiThau(item, idButton) {
    $(item).closest("tr").addClass("error-row");
    $(item).css('display', 'none');
    $(item).closest('tr').find('#btn_ShowChiPhi_' + idButton).prop('disabled', true);
    $(item).closest('tr').find('#btn_chitiet_' + idButton).prop('disabled', true);
}

function Cancel() {
    window.location.href = "/QLVonDauTu/QLThongTinHopDong";
}

function SumGiaTriHopDong() {
    $("#tblDanhSachGoiThau .cb_DuToan").on("change", function () {
        CaculatorGiaTriHopDong();
    });

    $("#tblDanhSachGoiThau .fGiaTriGoiThau").on("change", function () {
        CaculatorGiaTriHopDong();
    })
}

function CaculatorGiaTriHopDong() {
   
    if ($("#tblDanhSachGoiThau tbody").find(".cb_DuToan:checkbox:checked").length > 0) {
        var fTong = 0;
        $.each($("#tblDanhSachGoiThau tbody").find(".cb_DuToan:checkbox:checked"), function (index, child) {
            var fGiaTriPheDuyet = UnFormatNumber($(child).closest("tr").find(".fGiaTriHD").text());
            if (fGiaTriPheDuyet != undefined && fGiaTriPheDuyet != null && fGiaTriPheDuyet != "")
                fTong += parseFloat(fGiaTriPheDuyet);
        });

        $("#fGiaTriHopDong").val(FormatNumber(fTong));
    }
    HanleEnableGiaTriHopDong(arrGoiThau);
}

function ChangeNhaThau() {
    $("#iID_NhaThauID").change( function () {
        var iID_NhaThauID = $("#iID_NhaThauID option:selected").val();
        $.ajax({
            url: "/QLVonDauTu/QLThongTinHopDong/GetSoTaiKhoanNhaThauByIdNhaThau",
            type: "POST",
            data: { iID_NhaThauID: iID_NhaThauID },
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data != "") {
                    $("#sStkNhaThau").val(data.data);
                }
            },
            error: function (data) {

            }
        })
    });
}

function HanleEnableGiaTriHopDong(arrGoiThau) {
    var isEnable = $("#tblDanhSachGoiThau tbody").find(".cb_DuToan:checkbox:checked").length > 0;
    if (isEnable) {
        $("#fGiaTriHopDong").prop('disabled', true);
    } else {
        $("#fGiaTriHopDong").prop('disabled', false);
    }

}
