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
    DinhDangSo();
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
                            htmlChiPhi += "<td align='center'> <button id='btn_chitiet_chiphi_" + x.IIDChiPhiID + "' onclick = ShowHangMuc('" + x.IIDChiPhiID + "') class='btn btn-detail btn-icon btnShowHangMuc' title='Chi tiết hạng mục'><i class='fa fa-eye fa-lg'></i></button>" +
                                "</td > ";
                        } else {
                            htmlChiPhi += "<td align='center'> <button id='btn_chitiet_chiphi_" + x.IIDChiPhiID + "' disabled onclick = ShowHangMuc('" + x.IIDChiPhiID + "') class='btn btn-detail btn-icon btnShowHangMuc' title='Chi tiết hạng mục'><i class='fa fa-eye fa-lg'></i></button>" +
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
                                htmlChiPhi += "<td align='center'> <button id='btn_chitiet_chiphi_" + x.IIDChiPhiID + "' onclick = ShowHangMucDb('" + x.IIDChiPhiID + "')  class='btn btn-detail btn-icon btnShowHangMuc' title='Chi tiết hạng mục'><i class='fa fa-eye fa-lg'></i></button>" +
                                    "</td > ";
                            } else {
                                htmlChiPhi += "<td align='center'> <button id='btn_chitiet_chiphi_" + x.IIDChiPhiID + "' disabled onclick = ShowHangMucDb('" + x.IIDChiPhiID + "') class='btn btn-detail btn-icon btnShowHangMuc' title='Chi tiết hạng mục'><i class='fa fa-eye fa-lg'></i></button>" +
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
            }
            $("#tblDanhSachPhuLucHangMuc tbody").html('');
        },
        error: function (data) {

        }
    })
}

var arrPhuLucHangMuc = [];
function EventCheckboxHangMuc() {
    //var goiThauId = $('#txtCurrentGoiThauSelected').val();
    //var idGoiThauNhaThau = $('#txtIdGoiThauNhaThau').val();
    //$(".cb_HangMuc").change(function () {
    //    hangMucId = $(this).data('hangmucid');
    //    chiPhiId = $(this).data('chiphiid');
    //    tienHangMuc = $(this).data('tienhangmuc');
    //    idfake = $(this).data('idfake');
    //    if (this.checked) {
    //        arrPhuLucHangMuc.push({ IdGoiThauNhaThau: idGoiThauNhaThau, IIDChiPhiID: chiPhiId, IIDHangMucID: hangMucId, FTienGoiThau: tienHangMuc, idFake: idfake })
    //    } else {
    //        var newArray = arrPhuLucHangMuc.filter(function (el) {
    //            return (el.IdGoiThauNhaThau == idGoiThauNhaThau && el.IIDChiPhiID == chiPhiId && el.IIDHangMucID != hangMucId && el.idFake != idfake)
    //        });
    //        arrPhuLucHangMuc = newArray;
    //    }
    //});
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
                        arrPhuLucHangMuc.push({ HangMucParentId: x.HangMucParentId, MaOrDer: x.MaOrDer, STenHangMuc: x.STenHangMuc, IdGoiThauNhaThau: x.IdGoiThauNhaThau, IIDChiPhiID: x.IIDChiPhiID, IIDHangMucID: x.IIDHangMucID, FTienGoiThau: x.FTienGoiThau, idFake: x.IdFake })
                    });
                }
            } else {
            }
        },
        error: function (data) {

        }
    })
}

function UpdateArrHangMuc(obj) {
    var value = $(obj).val();
    var idHangMucFake = $(obj).data('idfakehangmuc');
    var idGoiThauNhaThau = $(obj).data('idgoithaunhathau');
    var idChiPhi = $(obj).data('idchiphi');
    var newArray = arrPhuLucHangMuc.filter(function (el) {
        return (el.IdGoiThauNhaThau == idGoiThauNhaThau && el.IIDChiPhiID == idChiPhi && el.idFake == idHangMucFake)
    });

    arrPhuLucHangMuc = $.map(arrPhuLucHangMuc, function (el) { return (el.IdGoiThauNhaThau == idGoiThauNhaThau && el.IIDChiPhiID == idChiPhi && el.idFake == idHangMucFake) ? null : el });
    newArray.forEach(function (x) {
        arrPhuLucHangMuc.push({ HangMucParentId: x.HangMucParentId, MaOrDer: x.MaOrDer, STenHangMuc: value, IdGoiThauNhaThau: x.IdGoiThauNhaThau, IIDChiPhiID: x.IIDChiPhiID, IIDHangMucID: x.IIDHangMucID, FTienGoiThau: x.FTienGoiThau, idFake: x.idFake })
    });
}

function UpdateValueArrHangMuc(obj) {
    var value = $(obj).val();
    var idHangMucFake = $(obj).data('idfakehangmuc');
    var idGoiThauNhaThau = $(obj).data('idgoithaunhathau');
    var idChiPhi = $(obj).data('idchiphi');
    var newArray = arrPhuLucHangMuc.filter(function (el) {
        return (el.IdGoiThauNhaThau == idGoiThauNhaThau && el.IIDChiPhiID == idChiPhi && el.idFake == idHangMucFake)
    });

    arrPhuLucHangMuc = $.map(arrPhuLucHangMuc, function (el) { return (el.IdGoiThauNhaThau == idGoiThauNhaThau && el.IIDChiPhiID == idChiPhi && el.idFake == idHangMucFake) ? null : el });
    newArray.forEach(function (x) {
        arrPhuLucHangMuc.push({ HangMucParentId: x.HangMucParentId, MaOrDer: x.MaOrDer, STenHangMuc: x.TenHangMuc, IdGoiThauNhaThau: x.IdGoiThauNhaThau, IIDChiPhiID: x.IIDChiPhiID, IIDHangMucID: x.IIDHangMucID, FTienGoiThau: value, idFake: x.idFake })
    });
}

function AddRowHangMuc() {
    var goithauSelected = $('#txtIdGoiThauNhaThau').val();
    var chiPhiSelected = $('#txtCurrentChiPhiSelected').val();
    if (goithauSelected == '' || chiPhiSelected == '') {
        return;
    }
    var stt = $('#txtCurrentMaOrderHangMuc').val();
    var valueNext = '';
    if (stt != '') {
        valueNext = GenerateOrder(stt);
        $('#txtCurrentMaOrderHangMuc').val(valueNext);
    } else {
        valueNext = 1;
        $('#txtCurrentMaOrderHangMuc').val(1);
    }
    var idFake = CreateGuid();
    var rowHtml = "";
    rowHtml += "<tr>";
    rowHtml += "<td>" + valueNext + "</td>";
    rowHtml += "<td><input type='text' data-idgoithaunhathau = '" + goithauSelected + "' data-idchiphi = '" + chiPhiSelected + "' data-idfakehangmuc = '" + idFake + "' onchange ='UpdateArrHangMuc(this)' class='form-control txtHangMucName' maxlength='100' autocomplete='off'/></td>";
    rowHtml += "<td class='r_fGiaGoiThau sotien' align='right'>" + "</td>";
    rowHtml += "<td class='r_fGiaGoiThau sotien' align='right'> <input type='text' class='form-control col-sm-2 sotien' data-idgoithaunhathau = '" + goithauSelected + "' data-idchiphi = '" + chiPhiSelected + "' data-idfakehangmuc = '" + idFake + "' " +
        " onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' onchange ='UpdateValueArrHangMuc(this)' autocomplete='off'/>" + "</td>";
    var idRow = CreateGuid();
    rowHtml += "<td align='center'> <button style='width: 120px !important' data-idrowparent = '' data-idrow='" + idRow + "' class='btn btn-primary btnAddRowChid'><span> + Thêm dòng con</span></button>" +
        "</td > ";
    rowHtml += "</tr>";
    var countRow = $('#tblDanhSachPhuLucHangMuc tbody tr').length;
    if (countRow > 0) {
        $("#tblDanhSachPhuLucHangMuc tbody > tr").eq(countRow - 1).after(rowHtml);
    } else {
        $("#tblDanhSachPhuLucHangMuc tbody").html(rowHtml);
    }
    arrPhuLucHangMuc.push({ HangMucParentId: '', MaOrDer: valueNext, STenHangMuc: '', IdGoiThauNhaThau: goithauSelected, IIDChiPhiID: chiPhiSelected, IIDHangMucID: CreateGuid(), FTienGoiThau: 0, idFake: idFake })
}

function AddRowChild(id) {

}

function GenerateOrder(stt) {
    var index_ = stt.lastIndexOf("_");
    if (index_ < 0) {
        var number = parseInt(stt);
        return number + 1;
    } else {
        var prev = stt.slice(0, index_ + 1);
        var number = stt.slice(index_ + 1, stt.length);
        var numberNext = parseInt(number) + 1;
        var result = prev + numberNext;
        return result;
    }
}

function GenerateOrderChild(stt, number) {
    return stt + '_' + number;
}

var countIndexInsert;
function CountRowChild(id) {
    var allRow = document.getElementsByClassName("btnAddRowChid");
    for (var i = 0; i < allRow.length; i++) {

        var valueParent = $(allRow[i]).data("idrowparent");
        var valueCurrent = $(allRow[i]).data("idrow");
        if (valueParent != '' && id != '' && valueParent == id) {
            CountRowChild(valueCurrent);
            countIndexInsert++;
        }
    }
    return countIndexInsert;
}

function CountRowChildCurrent(id) {
    var allRow = document.getElementsByClassName("btnAddRowChid");
    var count = 0
    for (var i = 0; i < allRow.length; i++) {

        var valueParent = $(allRow[i]).data("idrowparent");
        var valueCurrent = $(allRow[i]).data("idrow");
        if (valueParent != '' && id != '' && valueParent == id) {
            count++;
        }
    }
    return count;
}

$("#tblDanhSachPhuLucHangMuc").on('click', '.btnAddRowChid', function (x) {
    countIndexInsert = 0;
    var currentRow = $(this).closest("tr");
    var rowIndex = $(this).closest("tr").index();
    var idRow = $(this).data("idrow");
    var col1value = currentRow.find("td:eq(0)").text();
    var numberChild = CountRowChild(idRow);
    var hangmucId = $(this).data("hangmucid");
    var nextValue = GenerateOrderChild(col1value, CountRowChildCurrent(idRow) + 1);
    var goithauSelected = $('#txtIdGoiThauNhaThau').val();
    var chiPhiSelected = $('#txtCurrentChiPhiSelected').val();
    var idFake = CreateGuid();

    var rowHtml = "";
    rowHtml += "<tr>";
    rowHtml += "<td> " + nextValue + " </td>";

    rowHtml += "<td><input type='text' data-idgoithaunhathau = '" + goithauSelected + "' data-idchiphi = '" + chiPhiSelected + "' data-idfakehangmuc = '" + idFake + "' onchange ='UpdateArrHangMuc(this)' class='form-control txtHangMucName' maxlength='100' autocomplete='off'/></td>";
    rowHtml += "<td class='r_fGiaGoiThau sotien' align='right'>" + "</td>";
    rowHtml += "<td class='r_fGiaGoiThau sotien' align='right'> <input type='text' class='form-control col-sm-2 sotien' data-idgoithaunhathau = '" + goithauSelected + "' data-idchiphi = '" + chiPhiSelected + "' data-idfakehangmuc = '" + idFake + "' " +
        " onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' onchange ='UpdateValueArrHangMuc(this)' autocomplete='off'/>" + "</td>";

    var newIdRow = CreateGuid();
    rowHtml += "<td align='center'> <button style='width: 120px !important' data-idrowparent = '" + idRow + "' data-idrow='" + newIdRow + "' class='btn btn-primary btnAddRowChid'><span> + Thêm dòng con</span></button>" +
        "</td > ";
    rowHtml += "</tr>";
    $("#tblDanhSachPhuLucHangMuc tbody > tr").eq(rowIndex + numberChild).after(rowHtml);

    arrPhuLucHangMuc.push({ HangMucParentId: hangmucId, MaOrDer: nextValue, STenHangMuc: '', IdGoiThauNhaThau: goithauSelected, IIDChiPhiID: chiPhiSelected, IIDHangMucID: CreateGuid(), FTienGoiThau: 0, idFake: idFake });
});

function ShowHangMucDb(id) {
    var hopDongId = $('#txtHopDongId').val();
    var isGoc = $('#txtIsGoc').val();
    var goiThauId = $('#txtCurrentGoiThauSelected').val();
    var idGoiThauNhaThau = $('#txtIdGoiThauNhaThau').val();
    $('#txtCurrentChiPhiSelected').val(id);

    var newArray = arrPhuLucHangMuc.filter(function (el) {
        return (el.IdGoiThauNhaThau == idGoiThauNhaThau && el.IIDChiPhiID == id)
    });
    var htmlChiPhi = "";
    newArray.forEach(function (x) {
        //var newArray = arrPhuLucHangMuc.filter(function (el) {
        //    return (el.IdGoiThauNhaThau == idGoiThauNhaThau && el.IIDChiPhiID == id && el.IIDHangMucID == x.IIDHangMucID && el.idFake == x.IdFake)
        //});
        htmlChiPhi += "<tr>";

        htmlChiPhi += "<td>" + x.MaOrDer + "</td>";

        htmlChiPhi += "<td><input type='text' data-idgoithaunhathau = '" + x.IdGoiThauNhaThau + "' data-idchiphi = '" + x.IIDChiPhiID + "' data-idfakehangmuc = '" + x.idFake + "' onchange ='UpdateArrHangMuc(this)' class='form-control' value = '" + x.STenHangMuc + "' maxlength='100' autocomplete='off'/></td>";

        htmlChiPhi += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriDuocDuyet) + "</td>";
        htmlChiPhi += "<td class='r_fGiaGoiThau sotien' align='right'> <input type='text' class='form-control col-sm-2 sotien' data-idgoithaunhathau = '" + x.IdGoiThauNhaThau + "' data-idchiphi = '" + x.IIDChiPhiID + "' data-idfakehangmuc = '" + x.IdFake + "' " +
            " onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' onchange ='UpdateValueArrHangMuc(this)' autocomplete='off'/>" + "</td>";
        var idRow = CreateGuid();
        htmlChiPhi += "<td align='center'> <button style='width: 120px !important' data-hangmucid='" + x.IIDHangMucID + "' data-idrowparent = '' data-idrow='" + idRow + "' onclick = AddRowChild('" + x.IIDChiPhiID + "') class='btn btn-primary btnAddRowChid'><span> + Thêm dòng con</span></button>" +
            "</td > ";
        htmlChiPhi += "</tr>";
        $('#txtCurrentMaOrderHangMuc').val(x.MaOrDer);
    });
    $("#tblDanhSachPhuLucHangMuc tbody").html(htmlChiPhi);
}

function CreateGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

function ShowHangMuc(id) {
    var goiThauId = $('#txtCurrentGoiThauSelected').val();
    var idGoiThauNhaThau = $('#txtIdGoiThauNhaThau').val();
    $.ajax({
        url: "/QLVonDauTu/QLThongTinHopDong/LayThongTinHangMucPhuLuc",
        type: "POST",
        data: { iID_DuAnID: $("#iID_DuAnID").val(), hopDongId: '', chiphiId: id },
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
                        //if (newArray.length > 0) {
                        //    htmlChiPhi += "<td align='center'> <input type='checkbox' checked data-idfake= '" + x.IdFake + "' data-tienhangmuc= '" + x.FTienGoiThau + "' data-chiphiid='" + id + "' data-hangmucid='" + x.IIDHangMucID + "' class='cb_HangMuc'></td>";
                        //} else {
                        //    htmlChiPhi += "<td align='center'> <input type='checkbox' data-idfake= '" + x.IdFake + "' data-tienhangmuc= '" + x.FTienGoiThau + "' data-chiphiid='" + id + "' data-hangmucid='" + x.IIDHangMucID + "' class='cb_HangMuc'></td>";
                        //}

                        htmlChiPhi += "<td>" + x.MaOrDer + "</td>";
                        htmlChiPhi += "<td>" + x.STenHangMuc + "</td>";
                        htmlChiPhi += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriDuocDuyet) + "</td>";
                        htmlChiPhi += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriConLai) + "</td>";
                        htmlChiPhi += "<td align='center'> <button style='width: 120px !important' onclick = AddRowChild('" + x.IIDChiPhiID + "') class='btn btn-primary btnAddRowChid'><span> + Thêm dòng con</span></button>" +
                            "</td > ";
                        htmlChiPhi += "</tr>";
                        $('#txtCurrentMaOrderHangMuc').val(x.MaOrDer);
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
        var giatritrungthau = $(this).data('giatritrungthau');
        var giatrihopdong = $(this).data('giatrihopdong');
        if (this.checked) {
            arrGoiThau.push({ stt: sttGoiThau, IIDGoiThauID: iID_GoiThau_select, IIdNhaThauId: nhaThauId, fGiaTriGoiThau: giatrigoithau, fGiaTriHD: giatrihopdong, FGiaTriTrungThau: giatritrungthau })
            $('#btn_chitiet_' + iID_GoiThau_select).removeAttr('disabled');
            $('#btn_ShowChiPhi_' + iID_GoiThau_select).removeAttr('disabled');

        } else {
            var newArray = arrGoiThau.filter(function (el) {
                return (el.IIDGoiThauID != iID_GoiThau_select)
            });
            arrGoiThau = newArray;
            $('#btn_chitiet_' + iID_GoiThau_select).attr("disabled", true);
            $('#btn_ShowChiPhi_' + iID_GoiThau_select).attr("disabled", true);
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
                    data.forEach(function (x) {
                        arrPhuLucChiPhi.push({ IdGoiThauNhaThau: x.IdGoiThauNhaThau, IIDChiPhiID: x.IIDChiPhiID, FTienGoiThau: x.FTienGoiThau })
                    });
                }
            } else {
            }
        },
        error: function (data) {

        }
    })
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

function CreateDropDownNhaThau(nhathauId) {
    var html = '';
    arrNhaThau.forEach(function (x) {
        if (x.iID_NhaThauID == nhathauId) {
            html += '<option selected value = "' + x.iID_NhaThauID + '">' + x.STenNhaThau + '</option>';
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
                        htmlGoiThau += "<td>" + "</td>";
                        htmlGoiThau += "<td align='center'> <button id='btn_chitiet_" + x.IIDGoiThauID + "' onclick = ShowChiPhiDb('" + x.IIDGoiThauID + "'" + ",'" + x.Id + "')  title='Chi phí chi tiết' class='btn btn-detail btn-icon btnShowGoiThau'><i class='fa fa-eye fa-lg'></i></button>" +
                            "</td > ";
                        //htmlGoiThau += "<td> <button id='btn_ShowChiPhi_" + x.IIDGoiThauID + "' class='btn btn-primary btnShowGoiThau'><span>+</span></button>"
                        //"</td > ";
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
            SumGiaTriHopDong()
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
                        htmlGoiThau += "<td align='center'> <input type='checkbox' checked data-giatrigoithau = '" + x.fGiaTriGoiThau + "' data-giatritrungthau = '" + x.FGiaTriTrungThau + "' data-giatrihopdong = '" + x.fGiaTriHD + "' data-nhathauid = '" + idDropDown + "' data-sttgoithau = '" + sttGoiThau + "' data-goithauid='" + x.IIDGoiThauID + "' class='cb_DuToan'></td>";
                        htmlGoiThau += "<td>" + x.STenGoiThau + "</td>";
                        htmlGoiThau += "<td> <select id='" + idDropDown + "' data-sttgoithau = '" + sttGoiThau + "' onchange='UpdateItemNhaThau($(this))' data-goithauid='" + x.IIDGoiThauID + "' class='form-control dropdown_nhathau'> " + dropDownValue + " </select></td>";

                        htmlGoiThau += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FTienTrungThau) + "</td>";
                        htmlGoiThau += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.fGiaTriGoiThau) + "</td>";
                        htmlGoiThau += "<td class='fGiaTriGoiThau r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriTrungThau) + "</td>";
                        htmlGoiThau += "<td class='fGiaTriHD r_fGiaTriHD sotien' align='right'>" + FormatNumber(x.fGiaTriHD) + "</td>";
                        htmlGoiThau += "<td>" + "</td>";
                        htmlGoiThau += "<td> <button id='btn_chitiet_" + x.IIDGoiThauID + "' onclick = ShowChiPhiDb('" + x.IIDGoiThauID + "'" + ",'" + x.Id + "')  title='Chi phí chi tiết' class='btn btn-detail btn-icon btnShowGoiThau'><i class='fa fa-eye fa-lg'></i></button>" +
                            "</td > ";
                        htmlGoiThau += "<td> <button id='btn_ShowChiPhi_" + x.IIDGoiThauID + "' class='btn btn-primary btnShowGoiThau'><span>+</span></button>"
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
            data: { model: hopDong, goithau: arrGoiThau, chiphi: arrPhuLucChiPhi, hangmuc: arrPhuLucHangMuc, isDieuChinh: true },
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data.status == true) {
                    window.location.href = "/QLVonDauTu/QLThongTinHopDong";
                }
            },
            error: function (data) {

            }
        })
    }
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
    var fTong = 0;
    $.each($("#tblDanhSachGoiThau tbody").find(".cb_DuToan:checkbox:checked"), function (index, child) {
        var fGiaTriPheDuyet = UnFormatNumber($(child).closest("tr").find(".fGiaTriHD").text());
        if (fGiaTriPheDuyet != undefined && fGiaTriPheDuyet != null && fGiaTriPheDuyet != "")
            fTong += parseFloat(fGiaTriPheDuyet);
    });
    $("#fGiaTriHopDong").val(FormatNumber(fTong));
}

function ChangeNhaThau() {
    $("#iID_NhaThauID").change(function () {
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