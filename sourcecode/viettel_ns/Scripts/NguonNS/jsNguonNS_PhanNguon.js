var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;

function SearchData(iCurrentPage = 1) {
    var soChungTu = $("#txtSoChungTu").val();
    var noiDung = $("#txtNoiDung").val();
    GetListData(soChungTu, noiDung, iCurrentPage);
}

function ChangePage(iCurrentPage) {
    var soChungTu = $("#txtSoChungTu").val();
    var noiDung = $("#txtNoiDung").val();
    GetListData(soChungTu, noiDung, iCurrentPage);
}

function GetListData(soChungTu, noiDung, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { soChungTu: soChungTu, noiDung: noiDung, _paging: _paging },
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


function BtnInsertDataClick() {
    window.location.href = "/QLNguonNganSach/QLPhanNguonBTCTheoNDChiBQP/Update/";
}

function SaveData() {
    var data = {};
    data.iID_PhanNguon = $("#txt_ID_PhanNguon").val();
    data.sNoiDung = $("#txt_NoiDung").val();
    data.sSoChungTu = $("#txt_SoChungTu").val();
    data.dNgayChungTu = $("#d_NgayChungTu").val();
    data.sSoQuyetDinh = $("#txt_SoQuyetDinh").val();
    data.dNgayQuyetDinh = $("#d_NgayQuyetDinh").val();

    if (!ValidateDataNNSPhanNguon(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLPhanNguonBTCTheoNDChiBQP/NNSPhanNguonSave",
        data: { data: data },
        success: function (r) {
            if (r == "True") {
                window.location.href = "/QLNguonNganSach/QLPhanNguonBTCTheoNDChiBQP/Index";
            }
        }
    });
}

function DeleteItem(id) {
    var Title = 'Xác nhận xóa phân nguồn BTC theo nội dung chi BQP';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn xóa?');
    var FunctionName = "DeleteItemPN('" + id + "')";
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
        success: function (data) {
            $("#divModalConfirm").html(data);
        }
    });
}

function DeleteItemPN(id) {
    $.ajax({
        type: "POST",
        url: "/QLPhanNguonBTCTheoNDChiBQP/NNSPhanNguonDelete",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                SearchData();
            }
        }
    });
}

function ValidateDataNNSPhanNguon(data) {
    var sMessError = [];

    if (data.sNoiDung == null || data.sNoiDung == "") {
        sMessError.push("Nội dung chưa nhập !");
    } else if (data.sNoiDung.length > 250) {
        sMessError.push("Nội dung vượt quá số kí tự !");
    }

    if (data.sSoChungTu == null || data.sSoChungTu == "") {
        sMessError.push("Số chứng từ chưa nhập !");
    } else if (data.sSoChungTu.length > 50) {
        sMessError.push("Số chứng từ vượt quá số kí tự !");
    }

    if (data.dNgayChungTu == null || data.dNgayChungTu == "") {
        sMessError.push("Ngày chứng từ chưa nhập !");
    }

    if (sMessError != null && sMessError != "") {
        var Title = 'Lỗi lưu phân nguồn BTC theo nội dung chi BQP';
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: sMessError, Category: ERROR },
            success: function (data) {
                $("#divModalConfirm").html(data);
            }
        });
        return false;
    }

    return true;
}


function GetItemData(id) {
    window.location.href = "/QLNguonNganSach/QLPhanNguonBTCTheoNDChiBQP/Update/" + id;
}

function ViewItemDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNguonNganSach/QLPhanNguonBTCTheoNDChiBQP/ViewDetail",
        data: { id: id },
        success: function (data) {
            $("#contentPhanNguonBTC").html(data);
            $("#modalPhanNguonBTCLabel").html('Xem chi tiết thông tin ' + $("#txt_SoChungTu").html());
            $(".date").datepicker({
                todayBtn: "linked",
                language: "vi",
                autoclose: true,
                todayHighlight: true,
                format: 'dd/mm/yyyy'
            });
        }
    });
    //window.location.href = "/QLNguonNganSach/QLPhanNguonBTCTheoNDChiBQP/ViewDetail/" + id;
}

function BtnViewDMNguonBTCCapClick(id) {
    window.location.href = "/QLNguonNganSach/QLPhanNguonBTCTheoNDChiBQP/ViewDMNguonBTCCap/?id=" + id; 
}

function BtnViewPhanNguonBTCTheoNDChiClick(iIdNguon, iIdPhanNguon, rSoTienBTCCap, rSoTienConLai, sNoiDung) {
    /*window.location.href = '/QLNguonNganSach/QLPhanNguonBTCTheoNDChiBQP/ViewPhanNguonBTCTheoNDChi?iIdNguon=' + iIdNguon + '&iIdPhanNguon=' + iIdPhanNguon + '&rSoTienBTCCap=' + rSoTienBTCCap + '&rSoTienConLai=' + rSoTienConLai;*/
    window.location.href = '/QLNguonNganSach/QLPhanNguonBTCTheoNDChiBQP/ViewPhanNguonBTCTheoNDChi1?iIdNguon=' + iIdNguon + '&iIdPhanNguon=' + iIdPhanNguon + '&rSoTienBTCCap=' + rSoTienBTCCap + '&rSoTienConLai=' + rSoTienConLai + '&sNoiDung=' + sNoiDung;
}

function SaveNDChiData() {
    var data = [];
    var iID_PhanNguon = $("#iIdPhanNguon").val();
    $('#tblListNoiDungChiChiTiet tr').each(function () {
        var objNoiDungChiChiTiet = {}
        objNoiDungChiChiTiet.iID_NoiDungChi = $(this).find(".iIDNoiDungChi").val();
        if (objNoiDungChiChiTiet.iID_NoiDungChi != undefined) {
            objNoiDungChiChiTiet.SoTien = $(this).find(".soTienChitiet").val().replaceAll('.', '');
            objNoiDungChiChiTiet.GhiChu = $(this).find(".ghiChuChiTiet").val();
            if (objNoiDungChiChiTiet.SoTien > 0 && objNoiDungChiChiTiet.GhiChu) {
                objNoiDungChiChiTiet.iID_Nguon = $("#iID_Nguon").val();
                objNoiDungChiChiTiet.iID_PhanNguon = $("#iIdPhanNguon").val();
                data.push(objNoiDungChiChiTiet);
            }

        }
    });

    if (data.length > 0) {
        $.ajax({
            type: "POST",
            url: "/QLPhanNguonBTCTheoNDChiBQP/NNSPhanNguonNDChiChiTietSave",
            data: { data: data },
            success: function (r) {
                if (r == "True") {
                    window.location.href = "/QLNguonNganSach/QLPhanNguonBTCTheoNDChiBQP/ViewDMNguonBTCCap/" + iID_PhanNguon;
                } else {
                    alert("Có lỗi trong quá trình thêm mới!");
                }
            }
        });
    }
    else {
        alert("Bạn chưa nhập các khoản chi!");
    }
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

function formatMoneyOfLstPhanNguon() {
    $('#tblListNNSPhanNguon tr').each(function () {
        var rSoTienNNSPhanNguon = $(this).find(".rSoTienNNSPhanNguon").text();
        if (rSoTienNNSPhanNguon) {
            rSoTienNNSPhanNguon = formatNumber2(rSoTienNNSPhanNguon);
            $(this).find(".rSoTienNNSPhanNguon").text(rSoTienNNSPhanNguon);
        }
    });
}

function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNguonNganSach/QLPhanNguonBTCTheoNDChiBQP/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentPhanNguonBTC").html(data);
            if (id == null || id == GUID_EMPTY || id == undefined) {
                $("#modalPhanNguonBTCLabel").html('Thêm mới phân nguồn BTC theo nội dung chi BQP');
            }
            else {
                $("#modalPhanNguonBTCLabel").html('Cập nhật thông tin ' + $("#txt_SoChungTu").val());
            }
            $(".date").datepicker({
                todayBtn: "linked",
                language: "vi",
                autoclose: true,
                todayHighlight: true,
                format: 'dd/mm/yyyy'
            });
        }
    });
}