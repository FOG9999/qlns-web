var lstThanhToan = [];
var lstThuHoi = [];
var lstTamUng = [];
var lstKinhPhi = [];
var lstHopThuc = [];
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var iID_ThongTriID = GUID_EMPTY


$(document).ready(function () {
    iID_ThongTriID = $("#iID_ThongTriID").val();
    layChiTiet();

    // hide năm ngân sách nếu loại cấp phát là thanh toán hoặc tạm ứng
    if (['1', '2'].indexOf($('#loaiThongTri').val()) >= 0)
        $('.divNamNganSach').hide();
});


function removeCurrentActibveTab() {
    $('.tab-pane').each((index, x) => {
        if (x.classList.contains('active')) {
            console.log('xxx')
            x.classList.remove('active')
        }
    })
}

function layChiTiet() {
    $.ajax({
        url: "/QLVonDauTu/GiaiNganThanhToan/GetThongTriChiTiet",
        type: "POST",
        data: { id: iID_ThongTriID },
        dataType: "json",
        cache: false,
        success: function (data) {
            removeCurrentActibveTab();
            if (data != null && data != "") {
                var bIsActive = false;
                //var temp = Html.Raw(x.FSoTien == 0 ? "" : x.FSoTien.ToString("###,###"));
                if (data.lstThanhToan.length > 0) {
                    lstThanhToan = data.lstThanhToan;
                    var htmlThanhToan = "";
                    data.lstThanhToan.forEach(function (x) {
                        htmlThanhToan += "<tr>";
                        htmlThanhToan += " <td align=\"center\">" + x.SM  + "</td>";
                        htmlThanhToan += " <td align=\"center\">" + x.STm + "</td>";
                        htmlThanhToan += " <td align=\"center\">" + x.STtm + "</td>";
                        htmlThanhToan += " <td align=\"center\">" + x.SNg + "</td>";
                        htmlThanhToan += " <td align=\"center\">" + x.STenLoaiCongTrinh + "</td>";
                        htmlThanhToan += " <td align=\"center\">" + x.STenDuAn + "</td>";
                        htmlThanhToan += " <td> <input disabled style=' text-align:right' class='form-control currently'  value=" + x.FSoTien + " id='" + x.id + "'></td>";
                        htmlThanhToan += "</tr>";
                    });
                    $("#tblThanhToan tbody").html(htmlThanhToan);
                    $("#thanhtoan").addClass("active");
                    $("#tabThanhToan").removeClass("hidden");
                    $("#tabThanhToan").addClass("active");
                    bIsActive = true;
                }
                //else {
                //    $("#tblThanhToan").html('');
                //}
                if (data.lstThuHoi.length > 0) {
                    lstThuHoi = data.lstThuHoi;
                    var htmlThuHoi = "";
                    data.lstThuHoi.forEach(function (x) {
                        htmlThuHoi += "<tr>";
                        htmlThuHoi += "<input class='id_input' value=" + x.IIdDeNghiThanhToanId + "/>";
                        htmlThuHoi += " <td align=\"center\">" + x.SM + "</td>";
                        htmlThuHoi += " <td align=\"center\">" + x.STm + "</td>";
                        htmlThuHoi += " <td align=\"center\">" + x.STtm + "</td>";
                        htmlThuHoi += " <td align=\"center\">" + x.SNg + "</td>";
                        htmlThuHoi += " <td align=\"center\">" + x.STenLoaiCongTrinh + "</td>";
                        htmlThuHoi += " <td align=\"center\">" + x.STenDuAn + "</td>";
                        htmlThuHoi += " <td> <input disabled style=' text-align:right' class='form-control currently'  value=" + x.FSoTien + " id='" + x.id + "'></td>";
                        htmlThuHoi += "</tr>";
                    });
                    $("#tblThuHoi tbody").html(htmlThuHoi);
                    if (bIsActive == false) {
                        $("#thuhoi").addClass("active");
                    }
                    bIsActive = true;
                    $("#tabThuHoi").removeClass("hidden");
                }
                //else {
                //    $("#tblThuHoi").html('');
                //}
                if (data.lstTamUng.length > 0) {
                    lstTamUng = data.lstTamUng;
                    var htmlTamUng = "";
                    data.lstTamUng.forEach(function (x) {
                        htmlTamUng += "<tr>";
                        htmlTamUng += " <td align=\"center\">" + x.SM + "</td>";
                        htmlTamUng += " <td align=\"center\">" + x.STm + "</td>";
                        htmlTamUng += " <td align=\"center\">" + x.STtm + "</td>";
                        htmlTamUng += " <td align=\"center\">" + x.SNg + "</td>";
                        htmlTamUng += " <td align=\"center\">" + x.STenLoaiCongTrinh + "</td>";
                        htmlTamUng += " <td align=\"center\">" + x.STenDuAn + "</td>";
                        htmlTamUng += " <td> <input disabled style=' text-align:right' class='form-control currently'  value=" + x.FSoTien + " id='" + x.id + "'></td>";
                        htmlTamUng += "</tr>";

                    });
                    $("#tblTamUng tbody").html(htmlTamUng);
                    if (bIsActive == false) {
                        $("#tabTamUng").addClass("active");
                        $("#tamung").addClass("active");
                    }
                    bIsActive = true;
                    $("#tabTamUng").removeClass("hidden");
                }
                //else {
                //    $("#tblThanhToan").html('');
                //}
                if (data.lstKinhPhi.length > 0) {
                    lstKinhPhi = data.lstKinhPhi;
                    var htmlKinhPhi = "";
                    data.lstKinhPhi.forEach(function (x) {
                        htmlKinhPhi += "<tr>";
                        htmlKinhPhi += " <td align=\"center\">" + x.SM + "</td>";
                        htmlKinhPhi += " <td align=\"center\">" + x.STm + "</td>";
                        htmlKinhPhi += " <td align=\"center\">" + x.STtm + "</td>";
                        htmlKinhPhi += " <td align=\"center\">" + x.SNg + "</td>";
                        htmlKinhPhi += " <td align=\"center\">" + x.STenLoaiCongTrinh + "</td>";
                        htmlKinhPhi += " <td align=\"center\">" + x.STenDuAn + "</td>";
                        htmlKinhPhi += " <td> <input disabled style=' text-align:right' class='form-control currently' value=" + x.FSoTien + " id='" + x.id + "'></td>";
                        htmlKinhPhi += "</tr>";

                    });
                    $("#tblKinhPhi tbody").html(htmlKinhPhi);
                    if (bIsActive == false) {
                        $("#kinhphi").addClass("active");
                        $("#tabKinhPhi").addClass("active");
                    }
                    bIsActive = true;
                    $("#tabKinhPhi").removeClass("hidden");
                }
                //else {
                //    $("#tblThanhToan").html('');
                //}
                if (data.lstHopThuc.length > 0) {
                    lstHopThuc = data.lstHopThuc;
                    var htmlHopThuc = "";
                    data.lstHopThuc.forEach(function (x) {
                        htmlHopThuc += "<tr>";
                        htmlHopThuc += " <td align=\"center\">" + x.SM + "</td>";
                        htmlHopThuc += " <td align=\"center\">" + x.STm + "</td>";
                        htmlHopThuc += " <td align=\"center\">" + x.STtm + "</td>";
                        htmlHopThuc += " <td align=\"center\">" + x.SNg + "</td>";
                        htmlHopThuc += " <td align=\"center\">" + x.STenLoaiCongTrinh + "</td>";
                        htmlHopThuc += " <td align=\"center\">" + x.STenDuAn + "</td>";
                        htmlHopThuc += " <td> <input disabled style=' text-align:right' class='form-control currently' value=" + x.FSoTien + " id='" + x.id + "'></td>";
                        htmlHopThuc += "</tr>";
                    });
                    $("#tblHopThuc tbody").html(htmlHopThuc);
                    if (bIsActive == false) {
                        $("#hopthuc").addClass("active");
                        $("#tabHopThuc").addClass("active");
                    }
                    $("#tabHopThuc").removeClass("hidden");
                    bIsActive = true;
                }
                //else {
                //    $("#tblThanhToan").html('');
                //}
                //triggerCurrently();
                DinhDangSo();
            }

        },
        error: function (data) {

        }
    });
}


function Luu() { 
    $.ajax({
        url: "/QLVonDauTu/QLThongTriThanhToan/SaveThongTriChiTiet",
        type: "POST",
        data: {
            thongTriId: iID_ThongTriID,
            lstThanhToan: lstThanhToan,
            lstThuHoi: lstThuHoi,
            lstTamUng: lstTamUng,
            lstKinhPhi: lstKinhPhi,
            lstHopThuc: lstHopThuc
        },
        dataType: "json",
        cache: false,
        async: false,
        success: function (data) {
            console.log(data);
            window.location.href = "/QLVonDauTu/GiaiNganThanhToan";
        },
        error: function (data) {
            console.log(data);
            window.location.href = "/QLVonDauTu/GiaiNganThanhToan";
        }
    });
}

function DinhDangSo() {
    $(".currently").each(function () {
        $(this).val(FormatNumber($(this).val().trim()) == "" ? 0 : FormatNumber($(this).val().trim()));
    })
}

function CancelSaveData() {
    window.location.href = "/QLVonDauTu/GiaiNganThanhToan";
}