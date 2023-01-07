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
    $("#dieuChinh").hide();
    DinhDangSo();
    GetListNhaThau();
    LoadGoiThauDb();
    SetArrChiPhi();
    SetArrHangMuc();
});

function DinhDangSo() {
    $(".sotien").each(function () {
        if ($(this).is('input'))
            $(this).val(FormatNumber(UnFormatNumber($(this).val())));
        else
            $(this).html(FormatNumber(UnFormatNumber($(this).html())));
    })
}

function Cancel() {
    window.location.href = "/QLVonDauTu/QLThongTinHopDong";
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
                        var fTienGoiThau = !x.FTienGoiThau ? '' : FormatNumber(x.FTienGoiThau);
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
                            htmlChiPhi += "<td class='sotien' align='right'>" + fTienGoiThau + "</td>";
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
        idfake = $(this).data('idfake');
        if (this.checked) {
            arrPhuLucHangMuc.push({ IdGoiThauNhaThau: idGoiThauNhaThau, IIDChiPhiID: chiPhiId, IIDHangMucID: hangMucId, FTienGoiThau: tienHangMuc, idFake: idfake })
        } else {
            //var newArray = arrPhuLucHangMuc.filter(function (el) {
            //    return (el.IdGoiThauNhaThau == idGoiThauNhaThau && el.IIDChiPhiID == chiPhiId && el.IIDHangMucID != hangMucId && el.idFake != idfake)
            //});

            //arrPhuLucHangMuc = $.map(arrPhuLucHangMuc, function (el) {
            //    return
            //    ((el.IdGoiThauNhaThau != idGoiThauNhaThau || el.IIDChiPhiID != chiPhiId) || (el.IdGoiThauNhaThau == idGoiThauNhaThau && el.IIDChiPhiID == chiPhiId && el.IIDHangMucID != hangMucId && el.idFake != idfake)) ? el : null
            //});

            arrPhuLucHangMuc = $.map(arrPhuLucHangMuc, function (el) {
                return ((el.IdGoiThauNhaThau != idGoiThauNhaThau || el.IIDChiPhiID != chiPhiId) ||
                    (el.IdGoiThauNhaThau == idGoiThauNhaThau && el.IIDChiPhiID == chiPhiId && el.idFake != idfake)) ? el : null
            });
        }
    });
}

function SetArrHangMuc() {
    var hopDongId = $('#txtHopDongId').val();
    var isGoc = $('#txtIsGoc').val();
    $.ajax({
        url: "/QLVonDauTu/QLThongTinHopDong/LayThongTinHangMucByHopDongId",
        type: "POST",
        data: { hopdongId: hopDongId, isGoc: isGoc, listGoiThau: arrGoiThau },
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

function ShowHangMucDb(id) {
    var hopDongId = $('#txtHopDongId').val();
    var isGoc = $('#txtIsGoc').val();
    var goiThauId = $('#txtCurrentGoiThauSelected').val();
    var idGoiThauNhaThau = $('#txtIdGoiThauNhaThau').val();
    $.ajax({
        url: "/QLVonDauTu/QLThongTinHopDong/LayThongTinHangMucByHopDongId",
        type: "POST",
        data: { hopdongId: hopDongId, isGoc: isGoc, listGoiThau: arrGoiThau },
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
                                htmlChiPhi += "<td align='center'> <input type='checkbox' checked data-idfake= '" + x.IdFake + "' data-tienhangmuc= '" + x.FTienGoiThau + "' data-chiphiid='" + id + "' data-hangmucid='" + x.IIDHangMucID + "' class='cb_HangMuc'></td>";
                            } else {
                                htmlChiPhi += "<td align='center'> <input type='checkbox' data-idfake= '" + x.IdFake + "' data-tienhangmuc= '" + x.FTienGoiThau + "' data-chiphiid='" + id + "' data-hangmucid='" + x.IIDHangMucID + "' class='cb_HangMuc'></td>";
                            }
                            htmlChiPhi += "<td>" + x.MaOrDer + "</td>";
                            htmlChiPhi += "<td>" + x.STenHangMuc + "</td>";
                            htmlChiPhi += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriDuocDuyet) + "</td>";
                            htmlChiPhi += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FTienGoiThau) + "</td>";
                            /*htmlChiPhi += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriConLai) + "</td>";*/
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
        data: { iID_DuAnID: $("#txtDuAnId").val(), hopDongId: '', chiphiId: id },
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
                        /*htmlChiPhi += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriConLai) + "</td>";*/
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
        var idDropDown = '#dropdown_' + sttGoiThau + "_" + iID_GoiThau_select;
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

        },
        async: false
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
                        arrGoiThau.push({ Id: x.Id, stt: sttGoiThau, IIDGoiThauID: x.IIDGoiThauID, IIdNhaThauId: x.IIdNhaThauId, fGiaTriGoiThau: x.fGiaTriGoiThau, FGiaTriTrungThau: x.FGiaTriTrungThau })
                        var idDropDown = 'dropdown_' + sttGoiThau + "_" + x.IIDGoiThauID;
                        htmlGoiThau += "<tr>";
                        htmlGoiThau += "<td align='center'> <input type='checkbox' checked data-giatrigoithau = '" + x.fGiaTriGoiThau + "' data-giatritrungthau = '" + x.FGiaTriTrungThau + "' data-nhathauid = '" + idDropDown + "' data-sttgoithau = '" + sttGoiThau + "' data-goithauid='" + x.IIDGoiThauID + "' class='cb_DuToan'></td>";
                        htmlGoiThau += "<td>" + x.STenGoiThau + "</td>";
                        htmlGoiThau += "<td> <select id='" + idDropDown + "' data-sttgoithau = '" + sttGoiThau + "' onchange='UpdateItemNhaThau($(this))' data-goithauid='" + x.IIDGoiThauID + "' class='form-control dropdown_nhathau'> " + dropDownValue + " </select></td>";

                        htmlGoiThau += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FTienTrungThau) + "</td>";
                        htmlGoiThau += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.fGiaTriGoiThau) + "</td>";
                        htmlGoiThau += "<td class='fGiaTriGoiThau r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriTrungThau) + "</td>";
                        htmlGoiThau += "<td align='right'>" + FormatNumber(x.fGiaTriHD) +"</td>";
                        var idButton = x.IIDGoiThauID + '_' + sttGoiThau;
                        htmlGoiThau += "<td align='center'> <button id='btn_chitiet_" + idButton + "' onclick = ShowChiPhiDb('" + x.IIDGoiThauID + "'" + ",'" + x.Id + "')  title='Chi phí chi tiết' class='btn btn-detail btn-icon btnShowGoiThau'><i class='fa fa-eye fa-lg'></i></button>";
                          
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
            //SumGiaTriHopDong();
        },
        error: function (data) {

        },
        async: false
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

                        arrGoiThau.push({ stt: sttGoiThau, IIDGoiThauID: x.IIDGoiThauID, IIdNhaThauId: x.IIdNhaThauId, fGiaTriGoiThau: x.fGiaTriGoiThau, FGiaTriTrungThau: x.FGiaTriTrungThau })
                        var idDropDown = 'dropdown_' + sttGoiThau + "_" + x.IIDGoiThauID;
                        htmlGoiThau += "<tr>";
                        htmlGoiThau += "<td align='center'> <input type='checkbox' checked data-giatrigoithau = '" + x.fGiaTriGoiThau + "' data-giatritrungthau = '" + x.FGiaTriTrungThau + "' data-nhathauid = '" + idDropDown + "' data-sttgoithau = '" + sttGoiThau + "' data-goithauid='" + x.IIDGoiThauID + "' class='cb_DuToan'></td>";
                        htmlGoiThau += "<td>" + x.STenGoiThau + "</td>";
                        htmlGoiThau += "<td> <select id='" + idDropDown + "' data-sttgoithau = '" + sttGoiThau + "' onchange='UpdateItemNhaThau($(this))' data-goithauid='" + x.IIDGoiThauID + "' class='form-control dropdown_nhathau'> " + dropDownValue + " </select></td>";

                        htmlGoiThau += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FTienTrungThau) + "</td>";
                        htmlGoiThau += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.fGiaTriGoiThau) + "</td>";
                        htmlGoiThau += "<td class='r_fGiaGoiThau sotien' align='right'>" + FormatNumber(x.FGiaTriTrungThau) + "</td>";
                        htmlGoiThau += "<td>" + "</td>";
                        var idButton = x.IIDGoiThauID + '_' + sttGoiThau;
                        htmlGoiThau += "<td> <button id='btn_chitiet_" + idButton + "' disabled onclick = ShowChiPhi('" + x.IIDGoiThauID + "'" + ",'" + x.Id + "')  title='Chi phí chi tiết' class='btn btn-detail btn-icon btnShowGoiThau'><i class='fa fa-eye fa-lg'></i></button>";
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
