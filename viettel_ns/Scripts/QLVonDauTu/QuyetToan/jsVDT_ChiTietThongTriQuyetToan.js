var TBL_CAP_THANH_TOAN_KPQP = "tbl_capthanhtoankpqp";
var TBL_TAM_UNG_KPQP = "tbl_tamungkpqp";
var TBL_THU_UNG_KPQP = "tbl_thuungkpqp";
var TBL_UNG_XDCB_KHAC = "tbl_ungxdcbkhac";
var TBL_THU_UNG_XDCB_KHAC = "tbl_thuungxdcbkhac";
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var iIDThongTriID = "";

$(document).ready(function () {
    iIDThongTriID = $("#iIDThongTriID").val();
    LayThongTinThongTri(DinhDangSo);
});

function DinhDangSo() {
    $(".sotien").each(function () {
        $(this).html(FormatNumber($(this).html().trim()) == "" ? 0 : FormatNumber($(this).html().trim()));
    })
}

function LayThongTinThongTri(callback) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/QLThongTriQuyetToan/LayThongTinChiTietThongTriChiTiet",
        data: { iID_ThongTriID: iIDThongTriID },
        success: function (r) {
            var button = { bUpdate: 0, bDelete: 0, bInfo: 0 };
            if (r != null) {
                var columns = [
                    { sField: "iID_ThongTriChiTietID", bKey: true },
                    { sField: "iID_Parent", bParentKey: true },

                    { sTitle: "Mục", sField: "sM", iWidth: "7%", sTextAlign: "center" },
                    { sTitle: "Tiểu mục", sField: "sTM", iWidth: "7%", sTextAlign: "center" },
                    { sTitle: "Tiết mục", sField: "sTTM", iWidth: "7%", sTextAlign: "center" },
                    { sTitle: "Ngành", sField: "sNG", iWidth: "7%", sTextAlign: "center" },
                    { sTitle: "Nội dung", sField: "sNoiDung", iWidth: "55%", sTextAlign: "left", bHaveIcon: 1 },
                    { sTitle: "Số tiền", sField: "fSoTien", iWidth: "11%", sTextAlign: "right", sClass: "sotien" }];
                if (r.lstTab1 != null) {
                    $("#" + TBL_CAP_THANH_TOAN_KPQP).html(GenerateTreeTable(r.lstTab1, columns, button, TBL_CAP_THANH_TOAN_KPQP));
                } else {
                    $("#" + TBL_CAP_THANH_TOAN_KPQP).html("");
                }

                var columnsNoTM = [
                    { sField: "iID_ThongTriChiTietID", bKey: true },
                    { sField: "iID_Parent", bParentKey: true },

                    { sTitle: "Nội dung", sField: "sNoiDung", iWidth: "55%", sTextAlign: "left", bHaveIcon: 1 },
                    { sTitle: "Số tiền", sField: "fSoTien", iWidth: "11%", sTextAlign: "right", sClass: "sotien" }];

                if (r.lstTab2 != null) {
                    $("#" + TBL_TAM_UNG_KPQP).html(GenerateTreeTable(r.lstTab2, columnsNoTM, button, TBL_TAM_UNG_KPQP));
                } else {
                    $("#" + TBL_TAM_UNG_KPQP).html("");
                }

                if (r.lstTab3 != null) {
                    $("#" + TBL_THU_UNG_KPQP).html(GenerateTreeTable(r.lstTab3, columnsNoTM, button, TBL_THU_UNG_KPQP));
                } else {
                    $("#" + TBL_THU_UNG_KPQP).html("");
                }

                if (callback)
                    callback();
            }
        }
    });
}



function Huy() {
    window.location.href = "/QLVonDauTu/QLThongTriQuyetToan/Index";
}