function Loc() {
    var sMaDonVi = $("#iID_DonViQuanLyID").val();
    var iNamKeHoach = $("#iNamKeHoach").val();
    if (sMaDonVi == "") {
        alert("Thông tin đơn vị chưa có hoặc chưa chính xác");
        return;
    }
    if (iNamKeHoach == "") {
        alert("Năm thực hiện chưa có");
        return;
    }
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/BaoCaoDuToanNSQPNam/LayDataBaoCaoDuToanNSQPNam",
        data: { iNamKeHoach: iNamKeHoach, sMaDonVi: sMaDonVi },
        success: function (r) {
            var button = { bUpdate: 0, bDelete: 0, bInfo: 0 };
            if (r != null) {
                var columns = [
                    { sField: "iID", bKey: true },
                    { sField: "iID_Parent", bParentKey: true },

                    { sTitle: "Mục", sField: "sM", iWidth: "7%", sTextAlign: "center" },
                    { sTitle: "Tiểu mục", sField: "sTM", iWidth: "7%", sTextAlign: "center" },
                    { sTitle: "Tiết mục", sField: "sTTM", iWidth: "7%", sTextAlign: "center" },
                    { sTitle: "Ngành", sField: "sNG", iWidth: "7%", sTextAlign: "center" },
                    { sTitle: "Tên đơn vị, dự án, công trình", sField: "sTenDuAn", iWidth: "55%", sTextAlign: "left", bHaveIcon: 1 },
                    { sTitle: "Phân cấp", sField: "fGiaTrPhanBo", iWidth: "11%", sTextAlign: "right", sClass: "sotien" }];


                $("#tbl_data").html(GenerateTreeTable(r, columns, button, "tlb_data"));
                DinhDangSo();
            } else {
                $("#tbl_data").html("");
            }
        }
    })
}

function xuatFile() {
    $.ajax({
        type: 'Get',
        url: "/QLVonDauTu/BaoCaoDuToanNSQPNam/XuatFile",

    });
}

function DinhDangSo() {
    $(".sotien").each(function () {
        $(this).html(FormatNumber($(this).html().trim()) == "" ? 0 : FormatNumber($(this).html().trim()));
    })
}