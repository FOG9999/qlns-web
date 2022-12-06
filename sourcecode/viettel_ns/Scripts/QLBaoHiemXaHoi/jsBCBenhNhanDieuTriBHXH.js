$(document).ready(function () {
    //EventChangeThang();
    //EventChangeDonViNS();
    $('#iID_DonViBHXHParent').select2({
        width: 'resolve',
        matcher: matchStart
    });

    LoadDataComboBoxDonViBHParent();
});

function matchStart(params, data) {
    if ($.trim(params.term) === '') {
        return data;
    }

    if (typeof data.children === 'undefined') {
        return null;
    }

    var filteredChildren = [];
    $.each(data.children, function (idx, child) {
        if (child.text.toUpperCase().indexOf(params.term.toUpperCase()) == 0) {
            filteredChildren.push(child);
        }
    });

    if (filteredChildren.length) {
        var modifiedData = $.extend({}, data, true);
        modifiedData.children = filteredChildren;

        return modifiedData;
    }

    return null;
}

function LoadDataComboBoxDonViBHParent() {
    $.ajax({
        url: "/BaoHiemXaHoi/BaoCaoBHXH/GetDataComboBoxDonViBHParent",
        type: "POST",
        dataType: "json",
        //data: { id: id },
        success: function (resp) {
            $('#iID_DonViBHXHParent').select2({
                data: resp.data
            });
            //$('#iID_DonViBHXHParent').val($("#iID_ParentID").val()).trigger('change.select2');
        }
    })
}

function GetBaoCao() {
    $("#btnGetBaoCao").attr("disabled", "disabled");
    $.ajax({
        type: "POST",
        url: "/BaoHiemXaHoi/BaoCaoBHXH/BaoCaoTheoDoiQLBenhNhanNoiTru",
        //data: { iThang: $("#iThang").val(), sMaDonViNS: $("#cboDonViNS").val(), sMaDonViBHXH: $("#cboDonViBHXH").val() },
        data: { iThangBatDau: $("#iThangBatDau").val(), iThangKetThuc: $("#iThangKetThuc").val(), sMaDonViBHXHParent: $("#iID_DonViBHXHParent").val() },
        success: function (data) {
            $("#showData").html(data);
            //tinhTong();
            $("#contentReport").removeClass('hidden');
            $("#btnGetBaoCao").removeAttr("disabled");
        }
    });
}

//function EventChangeThang() {
//    $("#iThang").change(function () {
//        if (this.value != "") {
//            $.ajax({
//                url: "/BaoHiemXaHoi/QLBenhNhanBHXH/GetDonviNSByThang",
//                type: "POST",
//                data: { iThang: this.value },
//                dataType: "json",
//                cache: false,
//                success: function (data) {
//                    if (data != null && data != "") {
//                        $("#cboDonViNS").html(data);
//                        $("#cboDonViNS").trigger("change");
//                    }
//                },
//                error: function (data) {

//                }
//            })
//        } else {
//            $("#cboDonViNS option:not(:first)").remove();
//            $("#cboDonViNS").trigger("change");
//        }
//    });
//}

//function EventChangeDonViNS() {
//    $("#cboDonViNS").change(function () {
//        if (this.value != "") {
//            $.ajax({
//                url: "/BaoHiemXaHoi/QLBenhNhanBHXH/GetDonviBHXHByDonViNS",
//                type: "POST",
//                data: { sMaDonViNS: this.value },
//                dataType: "json",
//                cache: false,
//                success: function (data) {
//                    if (data != null && data != "") {
//                        $("#cboDonViBHXH").html(data);
//                    }
//                },
//                error: function (data) {

//                }
//            })
//        } else {
//            $("#cboDonViBHXH option:not(:first)").remove();
//            $("#cboDonViBHXH").trigger("change");
//        }
//    });
//}