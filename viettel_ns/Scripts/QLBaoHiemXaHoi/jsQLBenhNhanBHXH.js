$(document).ready(function () {
    var day = new Date();
    var month = day.getMonth() + 1;
    //$("#iThang").val(month);
    //EventChangeThang();
    EventChangeDonViNS();
});


function ResetChangePage(iCurrentPage = 1) {
    var sMaDonViBHXH = $("#cboDonViBHXH option:first").val();
    var iThangFrom = "";
    var iThangTo = "";
    var sMaDonViNS = $("#cboDonViNS option:first").val();
    var iSoNgayDieuTri ="";
    var sHoTen = "";
    var sMaThe = "";

    //SetValueFormExport(sSoChungTu, sNoiDung, sMaLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo);

    GetListData(sMaDonViBHXH, iThangFrom, iThangTo, iCurrentPage, sMaDonViNS, iSoNgayDieuTri, sHoTen, sMaThe);
}

function ChangePage(iCurrentPage = 1) {
    var sMaDonViBHXH = $("#cboDonViBHXH").val();
    var iThangFrom = $("#txtThangFrom").val();
    var iThangTo = $("#txtThangTo").val();
    var sMaDonViNS = $("#cboDonViNS").val();
    var iSoNgayDieuTri = $("#txtSoNgayDieuTri").val();
    var sHoTen = $("#txtHoTen").val();
    var sMaThe = $("#txtMaThe").val();

    //SetValueFormExport(sSoChungTu, sNoiDung, sMaLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo);

    GetListData(sMaDonViBHXH, iThangFrom, iThangTo, iCurrentPage, sMaDonViNS, iSoNgayDieuTri, sHoTen, sMaThe);
}

function GetListData(sMaDonViBHXH, iThangFrom, iThangTo, iCurrentPage, sMaDonViNS, iSoNgayDieuTri, sHoTen, sMaThe) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/BaoHiemXaHoi/QLBenhNhanBHXH/QLBenhNhanBHXHListView",
        data: { sMaDonViBHXH: sMaDonViBHXH, sMaDonViNS: sMaDonViNS, iThangFrom: iThangFrom, iThangTo: iThangTo, iSoNgayDieuTri: iSoNgayDieuTri, sHoTen: sHoTen, sMaThe: sMaThe, _paging: _paging },
        success: function (data) {
            $("#lstDataView").html(data);

            //$("#txtThangFrom").val(iThangFrom);
            //$("#txtThangTo").val(iThangTo);
            ////$("#cboDonViNS").html($("#sMaDVNS").val());
            //$("#cboDonViNS").val(sMaDonViNS).trigger("change");
            //$("#cboDonViNS").trigger("change");
            //$("#cboDonViBHXH").val(sMaDonViBHXH);
            //$("#cboDonViBHXH").trigger("change");
            

            //SetValueFormExport(sSoChungTu, sNoiDung, sMaLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhFrom, dNgayQuyetDinhTo);
        }
    });
}

function ImportBenhNhanBHXH() {
    window.location.href = "/BaoHiemXaHoi/QLBenhNhanBHXH/ImportBenhNhanBHXH/";
}

function EventChangeThang() {
    $("#iThang").change(function () {
        if (this.value != "") {
            $.ajax({
                url: "/BaoHiemXaHoi/QLBenhNhanBHXH/GetDonviNSByThang",
                type: "POST",
                data: { iThang: this.value },
                dataType: "json",
                cache: false,
                success: function (data) {
                    if (data != null && data != "") {
                        $("#cboDonViNS").html(data);
                        $("#cboDonViNS").trigger("change");
                    }
                },
                error: function (data) {

                }
            })
        } else {
            $("#cboDonViNS option:not(:first)").remove();
            $("#cboDonViNS").trigger("change");
        }
    });
}

function EventChangeDonViNS() {
    $("#cboDonViNS").change(function () {
        $("#cboDonViBHXH option:not(:first)").remove();
        if (this.value != "") {
            $.ajax({
                url: "/BaoHiemXaHoi/QLBenhNhanBHXH/GetDataComboBoxDonViBHParent",
                type: "POST",
                data: { sMaDonViNS: this.value },
                dataType: "json",
                cache: false,
                success: function (data) {
                    //if (data != null && data != "") {
                    //    $("#cboDonViBHXH").html(data);
                    //}
                    $('#cboDonViBHXH').select2({
                        data: data.data
                    });
                    //$('#cboDonViBHXH').val($("#sMaDVBHXH").val()).trigger('change.select2');
                },
                error: function (data) {

                }
            })
        } else {
            $("#cboDonViBHXH option:not(:first)").remove();
            $("#cboDonViBHXH").trigger("change");
        }
    });
}