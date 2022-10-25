function ChangePage(iCurrentPage) {
    var iIdPhanNguon = $("#iIdPhanNguon").val();
    GetListData(iIdPhanNguon, iCurrentPage);
}

function GetListData(iIdPhanNguon, iCurrentPage) {
    //window.location.href = "/QLNguonNganSach/QLPhanNguonBTCTheoNDChiBQP/ViewDMNguonBTCCap/?id=" + iIdPhanNguon + "&iCurrentPage=" + iCurrentPage; 
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { iIdPhanNguon: iIdPhanNguon, iCurrentPage: iCurrentPage },
        success: function (data) {
            $("#lstDataView").html(data);
        }
    });
}

function CancelSaveData() {
    window.location.href = "/QLNguonNganSach/QLPhanNguonBTCTheoNDChiBQP/Index";
}

function CancelSaveNDChiData(id) {
    BtnViewDMNguonBTCCapClick(id);
}


function BtnViewDMNguonBTCCapClick(id) {
    window.location.href = "/QLNguonNganSach/QLPhanNguonBTCTheoNDChiBQP/ViewDMNguonBTCCap/" + id;
}

function BtnViewPhanNguonBTCTheoNDChiClick(iIdNguon, iIdPhanNguon, rSoTienBTCCap, rSoTienConLai, sNoiDung) {
    /*window.location.href = '/QLNguonNganSach/QLPhanNguonBTCTheoNDChiBQP/ViewPhanNguonBTCTheoNDChi?iIdNguon=' + iIdNguon + '&iIdPhanNguon=' + iIdPhanNguon + '&rSoTienBTCCap=' + rSoTienBTCCap + '&rSoTienConLai=' + rSoTienConLai;*/
    window.location.href = '/QLNguonNganSach/QLPhanNguonBTCTheoNDChiBQP/ViewPhanNguonBTCTheoNDChi1?iIdNguon=' + iIdNguon + '&iIdPhanNguon=' + iIdPhanNguon + '&rSoTienBTCCap=' + rSoTienBTCCap + '&rSoTienConLai=' + rSoTienConLai + '&sNoiDung=' + sNoiDung;
}



function formatMoney() {
    $('#tblListNoiDungChiChiTiet tr').each(function () {
        var soTienConLai = $(this).find(".soTienChitiet").text();
        var soTien = $(this).find(".soTienChitiet").val();
        if (soTien != undefined) {
            soTien = formatNumber2(soTien);
            $(this).find(".soTienChitiet").val(soTien);
        }
        if (soTienConLai) {
            soTienConLai = formatNumber2(soTienConLai);
            $(this).find(".soTienChitiet").text(soTienConLai);
        }
    });
}

function formatNumber2(n) {
    // format number 1000000 to 1,234,567
    if (Number(n) >= 0) {
        return n.replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ".")
    }
    return "-" + n.replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ".")
}

function formatMoneyOfBTCCap() {
    $('#tblListNguonBTCCap tr').each(function () {
        var rSoTienBTCCap = $(this).find(".rSoTienBTCCap").text();
        if (rSoTienBTCCap) {
            rSoTienBTCCap = formatNumber2(rSoTienBTCCap);
            $(this).find(".rSoTienBTCCap").text(rSoTienBTCCap);
        }

        var rSoTienBTCDaPhan = $(this).find(".rSoTienBTCDaPhan").text();
        if (rSoTienBTCDaPhan) {
            rSoTienBTCDaPhan = formatNumber2(rSoTienBTCDaPhan);
            $(this).find(".rSoTienBTCDaPhan").text(rSoTienBTCDaPhan);
        }

        var rSoTienDaChi = $(this).find(".rSoTienDaChi").text();
        if (rSoTienDaChi) {
            rSoTienDaChi = formatNumber2(rSoTienDaChi);
            $(this).find(".rSoTienDaChi").text(rSoTienDaChi);
        }

        var rSoTienConLai = $(this).find(".rSoTienConLai").text();
        if (rSoTienConLai) {
            rSoTienConLai = formatNumber2(rSoTienConLai);
            $(this).find(".rSoTienConLai").text(rSoTienConLai);
        }
    });
}

function calSoTienConLai() {
    var soTienCoTheChi = $("#rSoTienCoTheChi").val();
    var soTienConLai = 0;
    var totalSoTienChi = 0;

    $('#tblListNoiDungChiChiTiet tr').each(function () {
        var iID_NoiDungChi = $(this).find(".iIDNoiDungChi").val();
        if (iID_NoiDungChi != undefined) {
            var soTien = $(this).find(".soTienChitiet").val().replaceAll('.', '');
            if (soTien != undefined) {
                totalSoTienChi += Number(soTien);
            }

        }
    });

    soTienConLai = soTienCoTheChi - totalSoTienChi;
    var txtSoTienConLai = "";
    txtSoTienConLai = formatNumber2(soTienConLai.toString());
    if (soTienConLai >= 0) {
        $("#id_rSoTienConLai").css({ "color": "" })
    } else {
        $("#id_rSoTienConLai").css({ "color": "red" })
    }

    $("#id_rSoTienConLai").text(txtSoTienConLai);
}
