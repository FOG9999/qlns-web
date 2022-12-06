var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var arrError = [];
var CONFIRM = 0;
var ERROR = 1;
function GetBaoCao() {
    if (Validate()) {
        var iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
        var iID_NguonVonID = $("#iID_NguonVonID").val();
        var iNamKeHoach = $("#iNamKeHoach").val();
        $.ajax({
            type: "POST",
            url: "/QLVonDauTu/KetQuaGiaiNganVonDauTuNguonVonNSQP/GetKetQuaGiaiNganChiKinhPhiDauTu",
            data: { iID_DonViQuanLyID: iID_DonViQuanLyID, iID_NguonVonID: iID_NguonVonID, iNamKeHoach: iNamKeHoach },
            success: function (data) {
                $("#showData").html(data);
                var sTenDonViQL = $("#iID_DonViQuanLyID :selected").html();
                $("#id_donviquanly").html(sTenDonViQL);
                var iNguonVon = $("#iID_NguonVonID :selected").text();
                var arrsNuonVon = iNguonVon.split(".");
                var sTenNguonVon = arrsNuonVon[1];
                $("#idNganSach").text(sTenNguonVon.toUpperCase());
                $("#contentReport").removeClass('hidden');
            }
        });
    }
    else {
        showErr(ERROR);
    }
  
}


function exportBCKetQuaGiaiNganVonDauTuNguonVonNSQP() {
    if (Validate()) {
        var iID_DonViQuanLyID = $("#iID_DonViQuanLyID").val();
        var sTenDonVi = $("#iID_DonViQuanLyID :selected").text();
        var iID_NguonVonID = $("#iID_NguonVonID").val();
        var iNamKeHoach = $("#iNamKeHoach").val();
        var iNguonVon = $("#iID_NguonVonID :selected").text();
        var arrsNuonVon = iNguonVon.split(".");
        var sTenNguonVon = arrsNuonVon[1];
        window.location.href = "/QLVonDauTu/KetQuaGiaiNganVonDauTuNguonVonNSQP/ExportExcel?iID_DonViQuanLyID=" + iID_DonViQuanLyID + '&sTenDonVi=' + sTenDonVi + '&sTenNguonVon=' + sTenNguonVon + '&iID_NguonVonID=' + iID_NguonVonID + '&iNamKeHoach=' + iNamKeHoach;    }
    else
    {
        showErr(ERROR);
    }
   
}

function Validate() {
    var result = true;
    if ($("#iNamKeHoach").val() == "") {
        arrError.push("Nhập năm kế hoạch");
        result = false;
    }
    return result;
}

function showErr(type) {
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: "Thông báo", Messages: arrError, Category: type },
        success: function (data) {
            $("#divModalConfirm").html(data);
            arrError = [];
        }
    });
}