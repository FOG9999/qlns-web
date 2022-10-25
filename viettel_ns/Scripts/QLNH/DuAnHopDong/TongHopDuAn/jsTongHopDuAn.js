var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;

function ResetChangePage(iCurrentPage = 1) {
    GetListData(GUID_EMPTY, GUID_EMPTY, iCurrentPage);
}

function ChangePage(iCurrentPage = 1) {
    var iDonVi = $("#slbsDonViFilter").val();
    var iBQuanLy = $("#slbBQuanLyFilter").val();

    GetListData(iDonVi, iBQuanLy, iCurrentPage);
}

function GetListData(iDonVi, iBQuanLy, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { _paging: _paging, iID_DonViID: iDonVi, iID_BQuanLyID:iBQuanLy },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#slbsDonViFilter").val(iDonVi) ;
            $("#slbBQuanLyFilter").val(iBQuanLy) ;
        }
    });
}