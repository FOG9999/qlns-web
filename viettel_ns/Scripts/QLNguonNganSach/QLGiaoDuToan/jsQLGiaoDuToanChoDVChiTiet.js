function SaoChepDuToan(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLGiaoDuToanChoDV/SaoChepDuToan",
        data: { id: id },
        success: function (resp) {
            resp = JSON.parse(resp);
            if (resp.listData == null || resp.listData.length <= 0) {
                return false;
            }

             
        }
    });
}