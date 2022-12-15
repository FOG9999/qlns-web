var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var HOP_DONG_GIAO_VIEC = 0;
var HOP_DONG_KINH_TE = 1;

$(document).ready(function () {
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
    $("#NoiDungHopDong").keyup(function (event) {
        ValidateMaxLength(this, 300);
    });
    ChangeNhaThau();
});

$("#iID_DonViQuanLyID").change(function () {
    if (this.value != "" && this.value != GUID_EMPTY) {
        $.ajax({
            url: "/QLVonDauTu/QLThongTinHopDong/LayDuAnTheoDonViQL",
            type: "POST",
            data: { iID_DonViQuanLyID: this.value },
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data != "") {
                    $("#iID_DuAnID").html(data);
                    $("#iID_DuAnID").trigger("change");
                }
            },
            error: function (data) {

            }
        })
    } else {
        $("#iID_DuAnID option:not(:first)").remove();
        $("#iID_DuAnID").trigger("change");
    }
});


var arrPhuLucChiPhi = [];
var listPhuLucChiPhi = [];
function EventCheckboxChiPhi(idGoiThauNhaThauValue) {
    $(".cb_ChiPhi").change(function () {
        iID_ChiPhi_select = $(this).data('chiphiid');
        idGoiThauNhaThau = $(this).data('idgoithaunhathau');

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

        SumGiaTriGoiThau(iID_ChiPhi_select);
    });
}


function ShowChiPhi(id, idGoiThauNhaThau) {
    $('#txtCurrentGoiThauSelected').val(id);
    $('#txtIdGoiThauNhaThau').val(idGoiThauNhaThau);
    $.ajax({
        url: "/QLVonDauTu/QLThongTinHopDong/LayThongTinChiPhiPhuLuc",
        type: "POST",
        data: { goiThauid: id, idGoiThauNhaThau: idGoiThauNhaThau, hopDongId: '', iID_DuAnID: $("#iID_DuAnID").val() },
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

                        var disable = x.IsHasHangMuc ? 'disabled' : '';
                        var fTienGoiThau = !x.FTienGoiThau ? '' : FormatNumber(x.FTienGoiThau);

                        htmlChiPhi += "<tr id='" + x.IIDChiPhiID + "'>";
                        if (newArray.length > 0) {
                            htmlChiPhi += "<td align='center'> <input type='checkbox' checked data-tiengoithau ='" + x.FGiaTriDuocDuyet + "' data-goithauid='" + id + "' data-chiphiid='" + x.IIDChiPhiID + "' class='cb_ChiPhi'></td>";
                        } else {
                            htmlChiPhi += "<td align='center'> <input type='checkbox' data-tiengoithau ='" + x.FGiaTriDuocDuyet + "' data-goithauid='" + id + "' data-chiphiid='" + x.IIDChiPhiID + "' class='cb_ChiPhi'></td>";
                        }

                        htmlChiPhi += "<td>" + x.STenChiPhi + "</td>";
                        htmlChiPhi += "<td class='r_fGiaGoiThau sotien' align='right' >" + FormatNumber(x.FGiaTriDuocDuyet) + "</td>";
                        htmlChiPhi += "<td>" + `<input style="text-align: right;" ${disable} class="r_fGiaTrungThau sotien form-control" type="text" value='${fTienGoiThau}' onkeyup="ValidateNumberKeyUp(this);" onkeypress="return ValidateNumberKeyPress(this, event);" onblur="SumGiaTriGoiThau('${x.IIDChiPhiID}')"/>` + "</td>";//giá trị trúng thầu
                        htmlChiPhi += "<td class='fGiaTriConLai sotien' align='right'>" + FormatNumber(x.FGiaTriConLai) + "</td>";
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

var arrPhuLucHangMuc = [];
function EventCheckboxHangMuc() {
    var goiThauId = $('#txtCurrentGoiThauSelected').val();
    var idGoiThauNhaThau = $('#txtIdGoiThauNhaThau').val();
    $(".cb_HangMuc").change(function () {
        hangMucId = $(this).data('hangmucid');
        chiPhiId = $(this).data('chiphiid');
        tienHangMuc = $(this).data('tienhangmuc');
        hangmucparent = $(this).data('hangmucparent');
        var MaOrder = $(this).closest("tr").find(".MaOrder").html();
        idfake = $(this).data('idfake');
        if (this.checked) {
            arrPhuLucHangMuc.push({ IdGoiThauNhaThau: idGoiThauNhaThau, IIDChiPhiID: chiPhiId, IIDHangMucID: hangMucId, FTienGoiThau: tienHangMuc, idFake: idfake, HangMucParentId: hangmucparent, MaOrder : MaOrder  })
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
        SumGiaTriGoiThau(chiPhiId);
    });
    //SumGiaTriTrungThauChiPhi();
}

// recalculate gia tri trung thau chi phi
function SumGiaTriTrungThauChiPhi(chiphiId) {
    var rowChiPhi = $("#tblDanhSachPhuLucChiPhi tbody").find('tr#' + chiphiId);
    var indexOfChiphi = arrPhuLucChiPhi.map(cp => cp.IIDChiPhiID).indexOf(chiphiId);
    if (indexOfChiphi >= 0) {
        arrPhuLucChiPhi[indexOfChiphi].FTienGoiThau = arrPhuLucHangMuc.filter(hm => hm.IIDChiPhiID === chiphiId && hm.MaOrder.indexOf("-") == -1).map(cp => cp.FTienGoiThau)
            .reduce((pre, curr) => pre + curr, 0);
        rowChiPhi.find('.r_fGiaTrungThau').val(FormatNumber(arrPhuLucChiPhi[indexOfChiphi].FTienGoiThau));
    }
}

// recalculate gia tri goi thau
function SumGiaTriGoiThau(idChiPhi) {

    var rowChiPhi = $("#tblDanhSachPhuLucChiPhi tbody").find('tr#' + idChiPhi);
    var inputChiPhi = rowChiPhi.children(":first").children(":first");
    var idGoiThau = inputChiPhi.data("goithauid");
    var isChecked = inputChiPhi.is(':checked');
    var tienGoiThau = rowChiPhi.find('.r_fGiaTrungThau').val() == "" ? 0 : parseFloat(UnFormatNumber(rowChiPhi.find('.r_fGiaTrungThau').val()));
    var phuLucChiPhiTemp = listPhuLucChiPhi.find(x => x.IIDChiPhiID == idChiPhi);
    var phuLucChiPhi = arrPhuLucChiPhi.find(x => x.IIDChiPhiID == idChiPhi);
    var giaTriPheDuyet = rowChiPhi.find('.r_fGiaGoiThau').html() == "" ? 0 : parseFloat(UnFormatNumber(rowChiPhi.find('.r_fGiaGoiThau').html()));

    if (isChecked) {
        var rowChiPhi = $("#tblDanhSachPhuLucChiPhi tbody").find('tr#' + idChiPhi);
        var tienGoiThau = rowChiPhi.find('.r_fGiaTrungThau').val() == "" ? 0 : parseFloat(UnFormatNumber(rowChiPhi.find('.r_fGiaTrungThau').val()));
        if (!phuLucChiPhiTemp)
            listPhuLucChiPhi.push({ IIDChiPhiID: idChiPhi, FTienGoiThau: tienGoiThau, IIDGoiThau: idGoiThau });
        else {
            phuLucChiPhiTemp.FTienGoiThau = tienGoiThau;
            phuLucChiPhi.FTienGoiThau = tienGoiThau;
        }
            
    } else {
        var newArray = listPhuLucChiPhi.filter(function (el) {
            return (el.IIDGoiThau == idGoiThau && el.IIDChiPhiID != idChiPhi)
        });
        listPhuLucChiPhi = newArray;
    }

    //var goiThauId = listPhuLucChiPhi[0].IIDGoiThau;
    var rowGoiThau = $("#tblDanhSachGoiThau tbody").find('tr#' + idGoiThau);

    var indexOfGoiThau = arrGoiThau.map(cp => cp.IIDGoiThauID).indexOf(idGoiThau);

    if (indexOfGoiThau >= 0) {
        arrGoiThau[indexOfGoiThau].fGiaTriGoiThau = listPhuLucChiPhi.filter(hm => hm.IIDGoiThau === idGoiThau).map(cp => cp.FTienGoiThau)
            .reduce((pre, curr) => pre + curr, 0);
        rowGoiThau.find('.r_fGiaTriGoiThau').html(FormatNumber(arrGoiThau[indexOfGoiThau].fGiaTriGoiThau));
    }

    // tinh gia tri con lai
    var giaTriConLai = giaTriPheDuyet - tienGoiThau;
    rowChiPhi.find('.fGiaTriConLai').html(FormatNumber(giaTriConLai));
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
        var MaOrder = $(row).find('.MaOrder').html();
        if (hangmucParentId && hangmucParentId === rowParentId) {
            if (parentCheckboxStatus) {
                $(row).find('input:checkbox').prop('checked', true);
                // $(row).find('input:checkbox').change();
                arrPhuLucHangMuc.push({ IdGoiThauNhaThau: idGoiThauNhaThau, IIDChiPhiID: chiPhiId, IIDHangMucID: hangMucId, FTienGoiThau: tienHangMuc, idFake: idfake, HangMucParentId: rowParentId, MaOrder: MaOrder })
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
        var MaOrder = $(row).find('.MaOrder').html();
        
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
                    arrPhuLucHangMuc.push({ IdGoiThauNhaThau: idGoiThauNhaThau, IIDChiPhiID: chiPhiId, IIDHangMucID: hangMucId, FTienGoiThau: tienHangMuc, idFake: idfake, HangMucParentId: null, MaOrder: MaOrder })
                }
            }
        }
    })
}

function ShowHangMuc(id) {
    var goiThauId = $('#txtCurrentGoiThauSelected').val();
    var idGoiThauNhaThau = $('#txtIdGoiThauNhaThau').val();
    $.ajax({
        url: "/QLVonDauTu/QLThongTinHopDong/LayThongTinHangMucPhuLuc",
        type: "POST",
        data: { iID_DuAnID: $("#iID_DuAnID").val(), hopDongId: '', chiphiId: id, goiThauId },
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
                            htmlChiPhi += "<td align='center'> <input type='checkbox' checked data-idfake= '" + x.IdFake + "' data-tienhangmuc= '" + x.FGiaTriDuocDuyet + "' data-chiphiid='" + id + "' data-hangmucid='" + x.IIDHangMucID + "' data-hangmucparent='" + x.HangMucParentId + "' class='cb_HangMuc'></td>";
                        } else {
                            htmlChiPhi += "<td align='center'> <input type='checkbox' data-idfake= '" + x.IdFake + "' data-tienhangmuc= '" + x.FGiaTriDuocDuyet + "' data-chiphiid='" + id + "' data-hangmucid='" + x.IIDHangMucID + "' data-hangmucparent='" + x.HangMucParentId + "' class='cb_HangMuc'></td>";
                        }
                        htmlChiPhi += "<td class='MaOrder'>" + x.MaOrDer + "</td>";
                        htmlChiPhi += "<td>" + x.STenHangMuc + "</td>";
                        htmlChiPhi += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriDuocDuyet) + "</td>";
                        htmlChiPhi += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriDuocDuyet) + "</td>";//giá trị trúng thầu
                        htmlChiPhi += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriConLai) + "</td>";
                        htmlChiPhi += "</tr>";
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
        //var idDropDown = '#dropdown_' + sttGoiThau + "_" + iID_GoiThau_select;
        var dropDownnNhaThau = $(this).data('nhathauid');
        var nhaThauId = $('#' + dropDownnNhaThau).val();
        var giatrigoithau = $(this).data('giatrigoithau');
        var giatritrungthau = $(this).data('giatritrungthau');
        var idcreate = $(this).data('idcreate');
        var idButton = iID_GoiThau_select + '_' + sttGoiThau;
        if (this.checked) {
            arrGoiThau.push({ Id: idcreate, stt: sttGoiThau, IIDGoiThauID: iID_GoiThau_select, IIdNhaThauId: nhaThauId, fGiaTriGoiThau: giatrigoithau, FGiaTriTrungThau: giatritrungthau })
            $('#btn_chitiet_' + idButton).removeAttr('disabled');
            $('#btn_ShowChiPhi_' + idButton).removeAttr('disabled');
            $('#btn_XoaRowGoiThau_' + idButton).removeAttr('disabled');
            $('#txt_giatritrungthau_' + idButton).removeAttr('disabled');
            $('#txt_giatrihopdong_' + idButton).removeAttr('disabled');
        } else {
            var newArray = arrGoiThau.filter(function (el) {
                return (el.IIDGoiThauID != iID_GoiThau_select)
            });
            arrGoiThau = newArray;
            $('#btn_chitiet_' + idButton).attr("disabled", true);
            $('#btn_ShowChiPhi_' + idButton).attr("disabled", true);
            $('#btn_XoaRowGoiThau_' + idButton).attr("disabled", true);
            $('#txt_giatritrungthau_' + idButton).attr("disabled", true);
            $('#txt_giatrihopdong_' + idButton).attr('disabled', true);
        }
        SumGiaTriHopDong();

        HanleEnableGiaTriHopDong(arrGoiThau);
    });
}

function UpdateItemNhaThau(item) {
    var goithauid = $(item).data('goithauid');
    var sttgoithau = $(item).data('sttgoithau');
    $.each(arrGoiThau, function () {
        if (this.stt == sttgoithau && this.IIDGoiThauID == goithauid) {
            this.IIdNhaThauId = $(item).val();

            // Or: `this.baz = [11, 22, 33];`
        }
    });
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
    var tientrungthau = UnFormatNumber(currentRow.find("td:eq(3)").text());
    var giatrigoithau = UnFormatNumber(currentRow.find("td:eq(4)").text());
    var giatritrungthau = UnFormatNumber(currentRow.find("td:eq(5)").text());
    var giatrihopdong = UnFormatNumber(currentRow.find("td:eq(6)").text());
    var newId = CreateIdGoiThau();
    var idDropDown = 'dropdown_' + sttGoiThau + "_" + newId;
    var idButton = goithauSelectedId + '_' + sttGoiThau;
    var htmlGoiThau = "";
    htmlGoiThau += `<tr id=${newId}>`;
    htmlGoiThau += "<td align='center'> <input type='checkbox' data-idcreate= '" + newId + "' data-giatrigoithau = '" + giatrigoithau + "' data-giatritrungthau = '" + giatritrungthau + "' data-nhathauid = '" + idDropDown + "' data-sttgoithau = '" + sttGoiThau + "' data-goithauid='" + goithauSelectedId + "' class='cb_DuToan'></td>";
    htmlGoiThau += "<td>" + tengoithau + "</td>";
    htmlGoiThau += "<td> <select id='" + idDropDown + "' data-sttgoithau = '" + sttGoiThau + "' onchange='UpdateItemNhaThau($(this))' data-goithauid='" + goithauSelectedId + "' class='form-control dropdown_nhathau'> " + dropDownValue + " </select></td>";
    htmlGoiThau += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(tientrungthau) + "</td>";
    htmlGoiThau += "<td class='r_fGiaGoiThau sotien r_fGiaTriGoiThau' align='right'>" + FormatNumber(giatrigoithau) + "</td>";
    //htmlGoiThau += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(giatritrungthau) + "</td>";
    htmlGoiThau += "<td align='center'> <input id='txt_giatritrungthau_" + idButton + "' type='text' disabled class='r_fGiaGoiThau form-control sotien' value='" + FormatNumber(giatritrungthau) + "'onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
    htmlGoiThau += "<td align='center'> <input id='txt_giatrihopdong_" + idButton + "' type='text' disabled class='r_fGiaTriHopDong form-control sotien' value='" + FormatNumber(giatrihopdong) + "'onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>"
    htmlGoiThau += "<td align='center' class='r_giaTriConLai'>" + "</td>";
    //var idButton = goithauSelectedId + '_' + sttGoiThau;
    htmlGoiThau += "<td> <button id='btn_chitiet_" + idButton + "' disabled onclick = ShowChiPhi('" + goithauSelectedId + "'" + ",'" + newId + "') title='Chi phí chi tiết' class='btn btn-detail btn-icon btnShowGoiThau'><i class='fa fa-eye fa-lg'></i></button>" +
        "<button id='btn_ShowChiPhi_" + idButton + "' data-goithauid = '" + goithauSelectedId + "' data-goithaunhathauid = '" + newId + "' onclick='AddRowGoiThau(this)' disabled class='btn btn-edit btn-icon btnShowGoiThau'><i class='fa fa-plus fa-lg'></i></button>" +
        "<button id='btn_XoaRowGoiThau_" + idButton + "' data-goithauid = '" + goithauSelectedId + "' data-goithaunhathauid = '" + newId + "' onclick=\"XoaRowGoiThau(this," +`\'${idButton}\'`+")\" disabled class='btn btn-delete btn-icon btnShowGoiThau'><i class='fa fa-trash-o fa-lg'></i></button>"
    "</td> ";
    htmlGoiThau += "</tr>";
    $("#tblDanhSachGoiThau tbody > tr").eq(rowIndex).after(htmlGoiThau);
    EventGridGoiThau();
}
function XoaRowGoiThau(item, idButton) {
    $(item).closest("tr").addClass("error-row");
    $(item).css('display', 'none');
    $(item).closest('tr').find('#btn_ShowChiPhi_'+idButton).prop('disabled', true);
    $(item).closest('tr').find('#btn_chitiet_'+idButton).prop('disabled', true);
}

function updateGiaTriConLaiGoiThau(goiThau, goiThauIndex) {
    var giaTriConLai = arrGoiThau[goiThauIndex].fGiaTriGoiThau - arrGoiThau[goiThauIndex].FGiaTriTrungThau;
    $(goiThau).closest('tr').find('.r_giaTriConLai').html(FormatNumber(giaTriConLai));
}

function updateGiaTriTrungThauGoiThau(goiThau) {
    var giaTriTrungThau = $(goiThau).val().replaceAll('.','');
    var goiThauId = $(goiThau).closest('tr').find('.cb_DuToan').data('goithauid');
    var goiThauIndex = arrGoiThau.map(it => it.IIDGoiThauID).indexOf(goiThauId);
    if (goiThauIndex >= 0) {
        arrGoiThau[goiThauIndex].FGiaTriTrungThau = parseInt(giaTriTrungThau);
        updateGiaTriConLaiGoiThau(goiThau, goiThauIndex);
    }
}

function updateGiaTriHopDongGoiThau(goiThau) {
    var fGiaTriHD = $(goiThau).val().replaceAll('.', '');
    var goiThauId = $(goiThau).closest('tr').find('.cb_DuToan').data('goithauid');
    var goiThauIndex = arrGoiThau.map(it => it.IIDGoiThauID).indexOf(goiThauId);
    if (goiThauIndex >= 0) {
        arrGoiThau[goiThauIndex].fGiaTriHD = parseInt(fGiaTriHD);
    }
}


$("#iID_DuAnID").change(function () {
    $("#sDiaDiem").val("");
    $("#sThoiGianThucHien").val("");
    $("#fTongMucDauTu").val("");
    if (this.value != "" && this.value != GUID_EMPTY) {
        // lay thong tin chi tiet du an
        $.ajax({
            url: "/QLVonDauTu/QLThongTinHopDong/LayThongTinChiTietDuAn",
            type: "POST",
            data: { iID_DuAnID: this.value, hopDongId: '' },
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data != "") {
                    // fill thong tin chi tiet du an
                    $("#sDiaDiem").val(data.duan.sDiaDiem);
                    var khoiCong = '';
                    var ketThuc = '';
                    if (data.duan.sKhoiCong != null)
                        khoiCong = data.duan.sKhoiCong;
                    if (data.duan.sKetThuc != null)
                        ketThuc = data.duan.sKetThuc;
                    $("#sThoiGianThucHien").val(khoiCong + '-' + ketThuc);
                    if (data.duan.fTongMucDauTu != null)
                        $("#fTongMucDauTu").val(data.duan.fTongMucDauTu.toLocaleString('vi-VN'));

                    var dropDownValue = $('#iID_NhaThauID').html();

                    if (data.goithau != null) {
                        var htmlGoiThau = "";
                        data.goithau.forEach(function (x) {
                            var idDropDown = 'dropdown_' + sttGoiThau + "_" + x.IIDGoiThauID;
                            var idButton = x.IIDGoiThauID + '_' + sttGoiThau;
                            htmlGoiThau += `<tr id=${x.IIDGoiThauID}>`;
                            htmlGoiThau += "<td align='center'> <input type='checkbox' data-idcreate= '" + x.Id + "' data-giatrigoithau = '" + x.FTienTrungThau + "' data-giatritrungthau = '" + x.FGiaTriTrungThau + "' data-nhathauid = '" + idDropDown + "' data-sttgoithau = '" + sttGoiThau + "' data-goithauid='" + x.IIDGoiThauID + "' class='cb_DuToan'></td>";
                            htmlGoiThau += "<td>" + x.STenGoiThau + "</td>";

                            htmlGoiThau += "<td> <select id='" + idDropDown + "' data-sttgoithau = '" + sttGoiThau + "' onchange='UpdateItemNhaThau($(this))' data-goithauid='" + x.IIDGoiThauID + "' class='form-control dropdown_nhathau'> " + dropDownValue + " </select></td>";

                            htmlGoiThau += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FTienTrungThau) + "</td>";
                            htmlGoiThau += "<td class='r_fGiaGoiThau sotien r_fGiaTriGoiThau' align='right'>" + FormatNumber(x.fGiaTriGoiThau) + "</td>";
                            //htmlGoiThau += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriTrungThau) + "</td>";
                            htmlGoiThau += "<td align='center'> <input id='txt_giatritrungthau_" + idButton + "' type='text' disabled class='r_fGiaGoiThau form-control sotien' onblur='updateGiaTriTrungThauGoiThau(this)' value='" + FormatNumber(x.FGiaTriTrungThau) + "'onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>";
                            htmlGoiThau += "<td align='center'> <input id='txt_giatrihopdong_" + idButton + "' type='text' disabled class='r_fGiaTriHopDong form-control sotien' onblur='updateGiaTriHopDongGoiThau(this)' value='" + FormatNumber(x.fGiaTriHD) + "'onkeyup='ValidateNumberKeyUp(this);' onkeypress='return ValidateNumberKeyPress(this, event);' autocomplete='off' /></td>"
                            htmlGoiThau += "<td align='center' class='r_giaTriConLai'>" + "</td>";
                            //var idButton = x.IIDGoiThauID + '_' + sttGoiThau;
                            htmlGoiThau += "<td> <button id='btn_chitiet_" + idButton + "' disabled onclick = ShowChiPhi('" + x.IIDGoiThauID + "'" + ",'" + x.Id + "') title='Chi phí chi tiết' class='btn btn-detail btn-icon btnShowGoiThau'><i class='fa fa-eye fa-lg'></i></button>" +
                                "<button id='btn_ShowChiPhi_" + idButton + "' data-goithauid='" + x.IIDGoiThauID + "' data-goithaunhathauid='" + x.Id + "' onclick='AddRowGoiThau(this)' disabled class='btn btn-edit btn-icon btnShowGoiThau'><i class='fa fa-plus fa-lg'></i></button>" +
                                "<button id='btn_XoaRowGoiThau_" + idButton + "' data-goithauid='" + x.IIDGoiThauID + "' data-goithaunhathauid='" + x.Id + "' onclick=\"XoaRowGoiThau(this," + `\'${idButton}\'` +")\" disabled class='btn btn-delete btn-icon btnShowGoiThau'><i class='fa fa-trash-o fa-lg'></i></button>"
                            "</td > ";
                            htmlGoiThau += "</tr>";
                            sttGoiThau++;
                        });
                        $("#tblDanhSachGoiThau tbody").html(htmlGoiThau);
                        EventGridGoiThau();
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
            },
            error: function (data) {

            }
        })
    } else {
        $("#sDiaDiem").val("");
        $("#sThoiGianThucHien").val("");
        $("#fTongMucDauTu").val("");
        $("#tblDanhSachGoiThau tbody").html('');
        arrGoiThau = [];
        arrPhuLucHangMuc = [];
        arrPhuLucChiPhi = [];
    }

    if (this.value != "" && this.value != GUID_EMPTY) {
        //lay thong tin gói thầu
        $.ajax({
            url: "/QLVonDauTu/QLThongTinHopDong/LayGoiThauTheoDuAnId",
            type: "POST",
            data: { iID_DuAnID: this.value },
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
})

$("#iID_GoiThauID").change(function () {
    $("#fTienTrungThau").val("");
    //$("#fGiaTriHopDong").val("");
    $("#iID_NhaThauID").val(GUID_EMPTY);
    $("#iID_NhaThauID").trigger("change");
    if (this.value != "" && this.value != GUID_EMPTY) {
        // lay thong tin chi tiet du an
        $.ajax({
            url: "/QLVonDauTu/QLThongTinHopDong/LayThongTinChiTietGoiThau",
            type: "POST",
            data: { iID_GoiThauID: this.value },
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data != "") {
                    // fill thong tin chi tiet gói thầu
                    if (data.fTienTrungThau != null) {
                        $("#fGiaTriHopDong").val(data.fTienTrungThau.toLocaleString('vi-VN'));
                        $("#fTienTrungThau").val(data.fTienTrungThau.toLocaleString('vi-VN'));
                    }
                    if (data.iID_NhaThauID != null && data.iID_NhaThauID != "" && data.iID_NhaThauID != GUID_EMPTY) {
                        $("#iID_NhaThauID").val(data.iID_NhaThauID);
                        $("#iID_NhaThauID").trigger("change");
                        $("#iID_NhaThauID").attr("disabled", "disabled");
                        $("#sSoTaiKhoan").attr("disabled", "disabled");
                        $("#sNganHang").attr("disabled", "disabled");
                    }
                }
            },

            error: function (data) {

            }
        })
    } else {
        $("#fTienTrungThau").val("");
        //$("#fGiaTriHopDong").val("");
        $("#iID_NhaThauID").removeAttr("disabled", "disabled");
        $("#sSoTaiKhoan").removeAttr("disabled", "disabled");
        $("#sNganHang").removeAttr("disabled", "disabled");
    }
})

$("#iID_NhaThauID").change(function () {
    $("#sSoTaiKhoan").val("");
    $("#sNganHang").val("");
    if (this.value != "" && this.value != GUID_EMPTY) {
        $.ajax({
            url: "/QLVonDauTu/QLThongTinHopDong/LayThongTinChiTietNhaThau",
            type: "POST",
            data: { iID_NhaThauID: this.value },
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data != "") {
                    $("#sSoTaiKhoan").val(data.sSoTaiKhoan);
                    $("#sNganHang").val(data.sNganHang);
                }
            },
            error: function (data) {

            }
        })
    } else {
        $("#sSoTaiKhoan").val("");
        $("#sNganHang").val("");
    }
});

$("#cboLoaiHopDong").change(function () {
    if ($("#cboLoaiHopDong").val() == HOP_DONG_GIAO_VIEC) {
        $("#iID_GoiThauID").val(GUID_EMPTY);
        $("#iID_GoiThauID").trigger("change");
        $("#dvGoiThau").hide();
    }
    else {
        $("#dvGoiThau").show();
    }
});

function CheckLoi(hopDong) {
    var messErr = [];
    var loaiHopDong = $("#cboLoaiHopDong").val();
    var thoiGianThucHien = $("#iThoiGianThucHien").val();
    var strThoiGianThucHien = thoiGianThucHien.replace(/\./g, '');

    if (CheckTrungSoHopDong(hopDong.sSoHopDong)) {
        sMessError.push("Trùng số hợp đồng, vui lòng nhập số khác");
    }

    if (hopDong.iID_DuAnID == "" || hopDong.iID_DuAnID == GUID_EMPTY)
        messErr.push("Hãy chọn dự án");
    if (hopDong.sSoHopDong == "")
        messErr.push("Số hợp đồng không được để trống");
    if (hopDong.sTenHopDong == "")
        messErr.push("Tên hợp đồng không được để trống");
    if (hopDong.dNgayHopDong == "")
        messErr.push("Ngày hợp đồng không được để trống hoặc sai định dạng");
    if (hopDong.iThoiGianThucHien == "" || isNaN(hopDong.iThoiGianThucHien))
        messErr.push("Thời gian thực hiện không được để trống");
    else if (strThoiGianThucHien != hopDong.iThoiGianThucHien || hopDong.iThoiGianThucHien <= 0) {
        messErr.push("Số ngày thực hiện phải là số nguyên lớn hơn 0");
    }
    if (hopDong.fTienHopDong == "" || isNaN(hopDong.fTienHopDong))
        messErr.push("Giá trị hợp đồng không được để trống");
    if (loaiHopDong == "")
        messErr.push("Hãy chọn loại hợp đồng");
    if (loaiHopDong == HOP_DONG_KINH_TE && (hopDong.iID_GoiThauID == "" || hopDong.iID_GoiThauID == GUID_EMPTY))
        messErr.push("Hãy chọn gói thầu");

    if (hopDong.dKhoiCongDuKien > hopDong.dKetThucDuKien)
        messErr.push("Thời gian kết thúc không được nhỏ hơn Thời gian bắt đầu");

    var newArray = arrGoiThau.filter(function (el) {
        return (el.IIdNhaThauId == '' || el.IIdNhaThauId == '00000000-0000-0000-0000-000000000000')
    });

    if (newArray.length > 0)
        messErr.push("Vui lòng chọn nhà thầu");

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
        url: "/QLVonDauTu/QLThongTinHopDong/CheckTrungSoHopDong",
        type: "POST",
        data: { val: sSoHopDong },
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

function Save() {
    var hopDong = {};
    //Thông tin dự án
    hopDong.iID_DuAnID = $("#iID_DuAnID").val();

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
    hopDong.NoiDungHopDong = $("#NoiDungHopDong").val();
    //Thông tin gói thầu
    hopDong.iID_GoiThauID = $("#iID_GoiThauID").val();
    hopDong.iID_NhaThauThucHienID = $("#iID_NhaThauID").val();
    hopDong.sSoTaiKhoan = $("#sStkNhaThau").val();
    hopDong.sNganHang = $("#sNganHang").val();
    hopDong.dThoiGianBaoLanhHopDongTu = $("#dBaoLanhHDTu").val();
    hopDong.dThoiGianBaoLanhHopDongDen = $("#dBaoLanhHDDen").val();

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

function Cancel() {
    window.location.href = "/QLVonDauTu/QLThongTinHopDong";
}

function EventGridGoiThau() {
    $(".r_fGiaTriHopDong").on("blur", function () {
        SumGiaTriHopDong();
    });
    EventCheckboxGoiThau();
}

function SumGiaTriHopDong() {
    var fTong = 0;
    $.each($("#tblDanhSachGoiThau tbody").find(".cb_DuToan:checkbox:checked"), function (index, child) {
        var fGiaTriPheDuyet = UnFormatNumber($(child).closest("tr").find(".r_fGiaTriHopDong").val());
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

function HanleEnableGiaTriHopDong(arrGoiThau)
{
    var isEnable = $("#tblDanhSachGoiThau tbody").find(".cb_DuToan:checkbox:checked").length > 0;
    if (isEnable) {
        $("#fGiaTriHopDong").prop('disabled', true);
    } else {
        $("#fGiaTriHopDong").prop('disabled', false);
    }

}