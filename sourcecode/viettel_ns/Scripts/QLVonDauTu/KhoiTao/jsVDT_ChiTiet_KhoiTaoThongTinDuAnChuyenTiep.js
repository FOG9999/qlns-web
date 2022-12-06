var tblDataGrid = [];

$(document).ready(function () {
    tblDataGrid = JSON.parse($("#arrChiTiet").val());
    RenderGridView(tblDataGrid);
})

function RenderGridView(data) {
    var columns = [
        { sField: "iId", bKey: true },
        { sField: "iIdRefer", bForeignKey: true },
        { sField: "iParentId", bParentKey: true },
        //{ sTitle: " ", sField: "", iWidth: "200px", sTextAlign: "left" },
        { sTitle: "M-TM-TM-N", sField: "sTenNganh", iWidth: "200px", sTextAlign: "left" },
        { sTitle: "Nguồn vốn", sField: "sTenNguonVon", iWidth: "500px", sTextAligsn: "left" },
        { sTitle: "Loại ngân sách", sField: "sTenLoaiNguonVon", iWidth: "400px", sTextAlign: "left" },
        { sTitle: "Mã dự án", sField: "sMaDuAn", iWidth: "200px", sTextAlign: "left" },
        { sTitle: "Chủ đầu tư", sField: "sTenChuDauTu", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Số quyết định", sField: "sSoQDDT", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Ngày phê duyệt", sField: "sNgayPheDuyetQDDT", iWidth: "200px", sTextAlign: "center" },
        { sTitle: "Tổng mức đầu tư", sField: "fTongMucDauTu", iWidth: "200px", sTextAlign: "right", sClass: "sotien" },
        { sTitle: "Số quyết định", sField: "sSoQuyetDinhTKDT", iWidth: "200px", sTextAlign: "right" },
        { sTitle: "Ngày phê duyệt", sField: "sNgayPheDuyetTKDT", iWidth: "200px", sTextAlign: "center" },
        { sTitle: "Giá trị dự toán", sField: "fGiaTriDuToan", iWidth: "300px", sTextAlign: "right", sClass: "sotien" },
        { sTitle: "KH vốn bố trí hết năm trước", sField: "fKHVonHetNamTruoc", iWidth: "400px", sTextAlign: "right", sClass: "sotien" },
        { sTitle: "Kế hoạch vốn chưa cấp", sField: "fKHVonChuaCap", iWidth: "400px", sTextAlign: "right", sClass: "sotien" },
        { sTitle: "Thanh toán KLHT", sField: "fLuyKeThanhToanKLHT", iWidth: "400px", sTextAlign: "right", sClass: "sotien" },
        { sTitle: "T/Ư theo chế độ chưa thu hồi", sField: "fLuyKeThanhToanTamUng", iWidth: "400px", sTextAlign: "right", sClass: "sotien" },
        { sTitle: "Vốn thanh toán KLHT(qua KB)", sField: "fThanhToanQuaKB", iWidth: "400px", sTextAlign: "right", sClass: "sotien" },
        { sTitle: "T/Ư theo chế độ chưa thu hồi(qua KB)", sField: "fTamUngQuaKB", iWidth: "400px", sTextAlign: "right", sClass: "sotien" },
        { sTitle: "Đã cấp", sField: "fSoChuyenChiTieuDaCap", iWidth: "400px", sTextAlign: "right", sClass: "sotien" },
        { sTitle: "Chưa cấp", sField: "fSoChuyenChiTieuChuaCap", iWidth: "400px", sTextAlign: "right", sClass: "sotien" }
    ];

    var button = { bCreateButtonInParent: 0, bUpdate: 0, bDelete: 0 };
    var sHtml = GenerateTreeTable(data, columns, button)
    $("#ViewTable").css("width", $("#ViewTable").width() + "px");
    $("#ViewTable").html(sHtml);
    AddColspanTable();
    DinhDangSo();
}

//===================== update css ===========================//
function AddColspanTable() {
    var index = 0;
    $(".table-parent th").each(function () {
        ++index;
        if (index == 5) {
            $(this).attr('colspan', 2);
            $(this).html("Thông tin dự án")
            return true;
        }
        if (index == 6) {
            $(this).attr('colspan', 3);
            $(this).html("Quyết định đầu tư")
            return true;
        }
        if (index == 7) {
            $(this).attr('colspan', 3);
            $(this).html("Thiết kế dự toán")
            return true;
        }

        if (index == 15) {
            $(this).attr('colspan', 4);
            $(this).html("Thanh toán kế hoạch vốn")
            return true;
        }

        if (index == 16) {
            $(this).attr('colspan', 2);
            $(this).html("Chuyển chỉ tiêu năm sau")
            return true;
        }
        if (index)
            $(this).attr('rowspan', 2);
    });

    index = 0;
    $(".table-parent th").each(function () {
        ++index;
        if (index == 8 || index == 9 || index == 10 || index == 11 || index == 12
            || index == 17 || index == 18 || index == 19 || index == 20) {
            $(this).remove();
            return true;
        }
    });

    var row = "<tr><th width='200px'>Mã dự án</th><th width='200px'>Chủ đầu tư</th>" +
        "<th width='200px'>Số quyết định</th><th width='200px'>Ngày phê duyệt</th><th width='200px'>Tổng mức đầu tư</th>" +
        "<th width='200px'>Số quyết định</th><th width='200px'>Ngày phê duyệt</th><th width='200px'>Giá trị dự toán</th>" +
        "<th width='200px'>Thanh toán KLHT</th><th width='200px'>T/Ư theo chế độ chưa thu hồi</th><th width='200px'>Vốn thanh toán KLHT(qua KB)</th><th width='200px'>T/Ư theo chế độ chưa thu hồi(qua KB)</th>" +
        "<th width='200px'>Đã cấp</th><th width='200px'>Chưa cấp</th>";
    "</tr>";
    $(".table-parent tr:first").after(row);
    $(".table-parent .fa-caret-down").closest("tr").find("[type=button]").remove();
}

function DinhDangSo() {
    $(".sotien").each(function () {
        $(this).html(FormatNumber($(this).html().trim()) == "" ? 0 : FormatNumber($(this).html().trim()));
    })
}