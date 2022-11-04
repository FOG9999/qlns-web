var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;
var isShowSearchDMLoaiCongTrinh = true;
var arrDonvi = [];
var arrNamKeHoach = [];


$(document).ready(function ($) {
    LoadDataNamKeHoach();
    LoadDataDonvi();
    ChangeVoucher();
});
function ResetChangePage(iCurrentPage = 1) {
    var tabIndex = 0;
    if ($('input[name=groupVoucher]:checked').val() == 1) {
        $("#txtSodenghiList").val("");
        $("#txtNgaydenghiList").val("");
        $(".selectNamKeHoachList select").val(0);
        $(".selectDonViList select").val(GUID_EMPTY);
        GetListDataTongHop();
    } else {
        GetListData("", null, GUID_EMPTY, 0, iCurrentPage, tabIndex);
    }
}

function LoadDataDonvi() {
    $.ajax({
        async: false,
        url: "/QLNH/QuyetToanNienDo/GetListDonvi",
        type: "POST",
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data.status == false) {
                return;
            }

            if (data.data != null)
                arrDonvi = data.data;
        }
    });
}
function LoadDataNamKeHoach() {
    $.ajax({
        async: false,
        url: "/QLNH/QuyetToanNienDo/GetListDropDownNamKeHoach",
        type: "POST",
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data.status == false) {
                return;
            }

            if (data.data != null)
                arrNamKeHoach = data.data;
        }
    });
}
function ChangePage(iCurrentPage = 1) {
    var tabIndex = $('input[name=groupVoucher]:checked').val();
    var sSoDeNghi = $("<div/>").text($.trim($("#txtSoDeNghiFillter").val())).html();
    var dNgayDeNghi = $("#txtNgayDeNghiFillter").val();
    var iNamKeHoach = $("#slbNamKeHoachFillter").val();
    var iDonVi = $("#iDonViFillter").val();
    ChangeVoucher();
    if ($('input[name=groupVoucher]:checked').val() == 1) {
        GetListDataTongHop();
    } else {
        GetListData(sSoDeNghi, dNgayDeNghi, iDonVi, iNamKeHoach, iCurrentPage, tabIndex);
    }
}
function onDetail(id) {
    window.location.href = "/QLNH/QuyetToanNienDo/Detail?id=" + id + "&edit=false";
}
function ValidateDataPrint(data) {
    var Title = 'Lỗi in báo cáo quyết toán niên độ';
    var Messages = [];

    if (data.txtTieuDe1 == null || data.txtTieuDe1 == "") {
        Messages.push("Tiêu đề 1 chưa nhập !");
    }
    if (data.txtTieuDe2 == null || data.txtTieuDe2 == "") {
        Messages.push("Tiêu đề 2 chưa nhập !");
    }
    if (data.txtDonViCapTren == null || data.txtDonViCapTren == "") {
        Messages.push("Đơn vị cấp trên chưa nhập !");
    }
    if (data.txtDonVi == null || data.txtDonVi == "") {
        Messages.push("Đơn vị chưa nhập !");
    }
    if (data.slbDonViVND == null || data.slbDonViVND == 0) {
        Messages.push("Đơn vị VND chưa chọn !");
    }
    if (data.slbDonViUSD == null || data.slbDonViUSD == 0) {
        Messages.push("Đơn vị USD chưa chọn !");
    }

    if (Messages.length > 0) {
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: Messages, Category: ERROR },
            success: function (data) {
                $("#divModalConfirm").html(data);
            }
        });
        return false;
    }

    return true;
}
function GetListDataTongHop() {
    var sSoDeNghi = $("<div/>").text($.trim($("#txtSodenghiList").val())).html();
    var dNgayDeNghi = $("#txtNgaydenghiList").val();
    var iNamKeHoach = $(".selectNamKeHoachList select").val();
    var iDonVi = $(".selectDonViList select").val();
    
    $.ajax({
        type: "POST",
        url: "/QLNH/QuyetToanNienDo/GetListTongHopQuyetToan",
        data: { sSoDeNghi: sSoDeNghi, dNgayDeNghi: dNgayDeNghi, iNamKeHoach: iNamKeHoach, iDonVi: iDonVi },
        async: false,
        success: function (r) {
            var columns = [
                { sField: "ID", bKey: true },
                { sField: "iID_TongHopID", bParentKey: true },
                { sTitle: "Số đề nghị", sField: "sSoDeNghi", iWidth: "17%", sTextAlign: "left", bHaveIcon: 1 },
                //{ sTitle: "STT", sField: "sSTT", iWidth: "3%", sTextAlign: "center" },
                { sTitle: "Ngày đề nghị", sField: "dNgayDeNghiStr", iWidth: "18%", sTextAlign: "center" },
                { sTitle: "Năm kế hoạch", sField: "iNamKeHoach", iWidth: "10%", sTextAlign: "center" },
                { sTitle: "Đơn vị", sField: "sTenDonVi", iWidth: "20%", sTextAlign: "center" },
            ];
            var button = { bUpdate: 1, bDelete: 1, bInfo: 1 };
            var sortedData = r.data.sort((a, b) => {
                if (a.ID < b.ID) {
                    return -1;
                }
                else return 1;
            })
            var sHtml = GenerateTreeTableNCCQ(sortedData, columns, button, true, false, isShowSearchDMLoaiCongTrinh)
            $("#txtSobannghi").text(r.data.length);
            $("#ViewTable").html(sHtml);

            $("#txtSodenghiList").val($("<div/>").html(sSoDeNghi).text());
            $("#txtNgaydenghiList").val(dNgayDeNghi);
            $(".selectDonViList select").val(iDonVi);
            $(".selectNamKeHoachList select").val(iNamKeHoach);

            $('.date').datepicker({
                todayBtn: "linked",
                keyboardNavigation: false,
                forceParse: false,
                autoclose: true,
                language: 'vi',
                todayHighlight: true,
                format: "dd/mm/yyyy"
            });
            $("#txtNgaydenghiList").keydown(function (event) {
                ValidateInputKeydown(event, this, 3);
            });
            $("#iId_DonViId").select2({ width: '100%', matcher: FilterInComboBox });
            $(".selectNamKeHoachList select").select2({ width: '100%', matcher: FilterInComboBox });
        }
    });

}

function ChangeVoucher() {
    if ($("input[name=groupVoucher]:checked").val() == "0") {
        $("#tbListQuyetToanNienDo").css("display", "");
        $("#QuyetToanNienDo").css("display", "");
        $("#ViewTable").css("display", "none");
        $("#QuyetToanNienDoTongHop").css("display", "none");
        $("#Padding").css("display", "");

    } else {
        GetListDataTongHop();
        $("#txtSodenghiList").val("");
        $("#txtNgaydenghiList").val("");
        $(".selectNamKeHoachList select").val(0).trigger('change.select2');
        $(".selectDonViList select").val(GUID_EMPTY).trigger('change.select2');

        $("#tbListQuyetToanNienDo").css("display", "none");
        $("#ViewTable").css("display", "");
        $("#QuyetToanNienDo").css("display", "none");
        $("#QuyetToanNienDoTongHop").css("display", "");
        $("#Padding").css("display", "none");
    }
}

function printBaoCao(ext) {
    var links = [];
    var url = "";
    var txtTieuDe1 = $.trim($("#txtTieuDe1").val());
    var txtTieuDe2 = $.trim($("#txtTieuDe2").val());
    var txtDonViCapTren = $.trim($("#txtDonViCapTren").val());
    var txtDonVi = $.trim($("#txtDonVi").val());
    var slbDonViVND = $("#slbDonViVND").val();
    var slbDonViUSD = $("#slbDonViUSD").val();
    var txtIDQuyetToan = $("#txtIDQuyetToan").val();
    
    var data = {};
    data.txtTieuDe1 = txtTieuDe1;
    data.txtTieuDe2 = txtTieuDe2;
    data.txtDonViCapTren = txtDonViCapTren;
    data.txtDonVi = txtDonVi;
    data.slbDonViVND = slbDonViVND;
    data.slbDonViUSD = slbDonViUSD;

    if (!ValidateDataPrint(data)) {
        return false;
    }

    url = $("#urlExportBCChiTiet").val() +
        "?ext=" + ext + "&txtTieuDe1=" + encodeURIComponent(txtTieuDe1)
        + "&txtTieuDe2=" + encodeURIComponent(txtTieuDe2)
        + "&txtDonViCapTren=" + encodeURIComponent(txtDonViCapTren)
        + "&txtDonVi=" + encodeURIComponent(txtDonVi)
        + "&slbDonViVND=" + slbDonViVND
        + "&slbDonViUSD=" + slbDonViUSD
        + "&txtIDQuyetToan=" + txtIDQuyetToan;

    links.push(url);

    openLinks(links);
}
function InBaoCaoModal() {
    var returnError = 0;
    var listId = [];
    var setTable;
    if ($('input[name=groupVoucher]:checked').val() == 1) {
        setTable = $("#tableTongHop");
    } else {
        setTable = $("#tbListQuyetToanNienDo");
    }
    setTable.find('tr').each(function () {
        if ($(this).find('input[type="checkbox"]').is(':checked')) {
            var id = $(this).find('input[type="checkbox"]').data("id")
            listId.push(id);
        }
    });
    //alert error
    if (listId.length != 1) {
        var Title = 'Vui lòng chọn 1 quyết toán niên độ';
        var Error = "Bạn phải chọn 1 quyết toán niên độ mới được In báo cáo!"
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: [Error], Category: ERROR },
            success: function (res) {
                $("#divModalConfirm").html(res);
            }
        });
    } else {
        //show modal tong hop
        $.ajax({
            type: "POST",
            dataType: "html",
            url: "/QLNH/QuyetToanNienDo/GetModalInBaoCao",
            data: { listId: listId },
            success: function (data) {
                $("#modalQTNienDo").modal("show")
                $("#contentModalQTNienDo").empty().html(data);
                $("#modalQTNienDoLabel").empty().html('Báo cáo đề nghị quyết toán niên độ');
            }
        });
    }

}

function TongHopModal() {
    //get all id row checkbox
    var returnError = 0;
    var listId = [];
    var listNamKeHoach = [];
    var listTongHop = [];
    var setTable;
    if ($('input[name=groupVoucher]:checked').val() == 1) {
        setTable = $("#tableTongHop");
    } else {
        setTable = $("#tbListQuyetToanNienDo");
    }
    setTable.find('tr').each(function () {
        if ($(this).find('input[type="checkbox"]').is(':checked')) {
            $(this).find("td").each(function (index) {
                const fieldName = $(this).data("getname");
                if (fieldName !== undefined) {
                    const fieldValue = $(this).data("getvalue");
                    if (listNamKeHoach.indexOf(fieldValue) === -1) {
                        listNamKeHoach.push(fieldValue);
                    }
                }
                const fieldTongHopName = $(this).data("gettonghop");
                if (fieldTongHopName !== undefined) {
                    const fieldValue = $(this).data("gettonghopvalue");
                    if (fieldValue != '' && fieldValue != null) {
                        listTongHop.push(fieldValue);
                    }
                }
            });
            var id = $(this).find('input[type="checkbox"]').data("id")
            var lock = $(this).find('input[type="checkbox"]').data("islock")
            console.log(lock)
            if (lock == false || lock == "False") {
                returnError++;
            } else {
                if (listId.indexOf(id) === -1) {
                    listId.push(id);
                }
            }
        }
    });
    //alert error
    if (returnError > 0) {
        var Title = 'Vui lòng khóa quyết toán niên độ';
        var Error = "Bạn phải khóa quyết toán niên độ mới Tổng hợp!"
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: [Error], Category: ERROR },
            success: function (res) {
                $("#divModalConfirm").html(res);
            }
        });
    } else if (listTongHop.length > 0) {
        var Title = 'Vui lòng chọn bản ghi chưa tổng hợp';
        var Error = "Bạn phải chọn bản ghi chưa tổng hợp!"
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: [Error], Category: ERROR },
            success: function (res) {
                $("#divModalConfirm").html(res);
            }
        });
    } else if (listNamKeHoach.length > 1) {
        var Title = 'Vui lòng chọn cùng năm kế hoạch';
        var Error = "Bạn phải chọn cùng năm kế hoạch mới Tổng hợp!"
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: [Error], Category: ERROR },
            success: function (res) {
                $("#divModalConfirm").html(res);
            }
        });
    }
    else {
        //show modal tong hop
        if (listId.length > 0 && listId != null) {
            _paging.CurrentPage = 1;
            $.ajax({
                type: "POST",
                dataType: "html",
                url: "/QLNH/QuyetToanNienDo/GetModalTongHop",
                data: { listId: listId, _paging: _paging, namKeHoach: listNamKeHoach[0] },
                success: function (data) {
                    $("#modalQTNienDo").modal("show")
                    $("#contentModalQTNienDo").empty().html(data);
                    $("#modalQTNienDoLabel").empty().html('Tổng hợp đề nghị quyết toán niên độ');
                    $('.date')
                        .datepicker({
                            todayBtn: "linked",
                            keyboardNavigation: false,
                            forceParse: false,
                            autoclose: true,
                            language: 'vi',
                            todayHighlight: true,
                        });
                }
            });
        } else {
            var Title = 'Vui lòng tích quyết toán';
            var Error = "Bạn phải tích quyết toán niên độ mới Tổng hợp!"
            $.ajax({
                type: "POST",
                url: "/Modal/OpenModal",
                data: { Title: Title, Messages: [Error], Category: ERROR },
                success: function (res) {
                    $("#divModalConfirm").html(res);
                }
            });
        }

    }

}
function GetListData(sSoDeNghi, dNgayDeNghi, iDonVi, iNamKeHoach, iCurrentPage, tabIndex) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { sSoDeNghi: sSoDeNghi, dNgayDeNghi: dNgayDeNghi, iDonVi: iDonVi, iNamKeHoach: iNamKeHoach, _paging: _paging, tabIndex: tabIndex },
        success: function (data) {
            $("#lstDataView").html(data);
            $("#txtSoDeNghiFillter").val($("<div/>").html(sSoDeNghi).text());
            $("#iDonViFillter").val(iDonVi);
            $("#txtNgayDeNghiFillter").val(dNgayDeNghi);
            $("#slbNamKeHoachFillter").val(iNamKeHoach);
        }
    });
}

function Xoa(id) {
    var Title = 'Xác nhận xóa quyết toán niên độ';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn xóa?');
    var FunctionName = "Delete('" + id + "')";
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
        success: function (data) {
            $("#divModalConfirm").empty().html(data);
        }
    });
}
function Delete(id) {
    $.ajax({
        type: "POST",
        url: "/QLNH/QuyetToanNienDo/Xoa",
        data: { id: id },
        success: function (data) {
            if (data) {
                if (data.bIsComplete) {
                    ChangePage();
                } else {
                    if (data.sMessError != "") {
                        var Title = 'Lỗi xóa quyết toán niên độ';
                        $.ajax({
                            type: "POST",
                            url: "/Modal/OpenModal",
                            data: { Title: Title, Messages: [data.sMessError], Category: ERROR },
                            success: function (res) {
                                $("#divModalConfirm").html(res);
                            }
                        });
                    }
                }
            }
        }
    });
}

function LockItem(id, sSoDeNghi, iKhoa, iTongHop) {
    if (iKhoa && iTongHop != null && iTongHop != "" && iTongHop != "null") {
        var Title = 'Bạn không thể mở khóa !';
        var Error = 'Bạn không thể mở khóa vì đã có quyết toán niên độ tổng hợp với quyết toán niên độ này!';
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: [Error], Category: ERROR },
            success: function (res) {
                $("#divModalConfirm").html(res);
            }
        });
    } else {
        var Title = 'Xác nhận ' + (iKhoa ? 'mở' : 'khóa') + ' quyết toán niên độ';
        var Messages = [];
        Messages.push('Bạn có chắc chắn muốn ' + (iKhoa ? 'mở' : 'khóa') + ' quyết toán niên độ số ' + $("<div/>").text(sSoDeNghi).html() + '?');
        var FunctionName = "Lock('" + id + "')";
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
            success: function (data) {
                $("#divModalConfirm").html(data);
            }
        });
    }

}
function Lock(id) {
    $.ajax({
        type: "POST",
        url: "/QLNH/QuyetToanNienDo/LockQuyetToan",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                ChangePage(1);
            }
        }
    });
}
function OpenModalDetail(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/QuyetToanNienDo/GetModalDetail",
        data: { id: id },
        success: function (data) {
            $("#contentModalQTNienDo").empty().html(data);
            $("#modalQTNienDoLabel").empty().html('Xem chi tiết thông tin quyết toán niên độ');
        }
    });
}
function OpenModal(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/QuyetToanNienDo/GetModal",
        data: { id: id },
        success: function (data) {
            $("#contentModalQTNienDo").empty().html(data);
            if (id == undefined || id == null || id == GUID_EMPTY) {
                $("#modalQTNienDoLabel").empty().html('Thêm mới đề nghị quyết toán niên độ');
            } else {
                $("#modalQTNienDoLabel").empty().html('Sửa đề nghị quyết toán niên độ');
            }
        }
    });
}

//Table
function CreateHtmlSelectDonVi(value) {
    var htmlOption = "<option value='" + GUID_EMPTY + "' selected>-- Chọn đơn vị --</option>";
    arrDonvi.forEach(x => {
        if (value != undefined && value == x.id)
            htmlOption += "<option value='" + x.id + "' selected>" + $("<div/>").text(x.text).html() + "</option>";
        else
            htmlOption += "<option value='" + x.id + "'>" + $("<div/>").text(x.text).html() + "</option>";
    })
    return "<select class='form-control' id='iId_DonViId'>" + htmlOption + "</option>";
}
function CreateHtmlSelectNamKeHoach(value) {
    var htmlOption = "<option value='0' selected>-- Chọn năm --</option>";
    arrNamKeHoach.forEach(x => {
        if (value != undefined && value == x.id)
            htmlOption += "<option value='" + x.id + "' selected>" + $("<div/>").text(x.text).html() + "</option>";
        else
            htmlOption += "<option value='" + x.id + "'>" + $("<div/>").text(x.text).html() + "</option>";
    })
    return "<select class='form-control'>" + htmlOption + "</option>";
}
function GenerateTreeTableNCCQ(data, columns, button, idTable, haveSttColumn = true, isShowSearchDMLoaiCongTrinh = false) {
    var sParentKey = '';
    var sKeyRefer = "";
    var sKey = ''
    var TableView = [];
    var sItemHiden = [];
    var iTotalCol = 1;
    TableView.push("<table class='table table-bordered table-parent' id='tableTongHop' style='margin-bottom: 0px'>");
    TableView.push("<thead class='header-search'>");
    TableView.push("<tr>");
    TableView.push("<th width='5%'></th>");
    /*TableView.push("<th width='3%'></th>");*/
    TableView.push("<th width='17%'><input type='text' class='form-control clearable' maxlength='100' placeholder='Số đề nghị' id='txtSodenghiList' autocomplete='off' /></th>");
    TableView.push("<th width='18%'><div class='input-group date'><input type='text' id='txtNgaydenghiList' class='form-control' value='' autocomplete='off' placeholder='dd/MM/yyyy' /><span class='btn-default input-group-addon input-calendar'><i class='fa fa-calendar' aria-hidden='true'></i></span></div ></th>");
    TableView.push("<th width='10%'><div class='selectNamKeHoachList'>" + CreateHtmlSelectNamKeHoach() + "</div></th>");
    TableView.push("<th width='20%'><div class='selectDonViList'>" + CreateHtmlSelectDonVi() + "</div></th>");
    TableView.push("<th width='25%'><button class='btn btn-info' onclick='GetListDataTongHop()'><i class='fa fa-search'></i>Tìm kiếm</button> </th>");
    TableView.push("</tr>");
    TableView.push("</thead>");

    TableView.push("<thead>");
    TableView.push("<th width='5%'></th>");

    $.each(columns, function (indexItem, value) {

        if (value.bKey) {
            sKey = value.sField;
        }

        if (value.bForeignKey) {
            sKeyRefer = value.sField;
        }

        if (value.bParentKey) {
            sParentKey = value.sField;
        }

        if (value.sTitle != null && value.sTitle != undefined && value.sTitle != '') {
            iTotalCol++;
            TableView.push("    <th width='" + value.iWidth + "'>" + value.sTitle + "</th>");
        } else {
            sItemHiden.push(value.sField);
        }
    });
    if (sKeyRefer == "") {
        sKeyRefer = sKey;
    }
    if (button.bUpdate == 1 || button.bDelete == 1 || button.bInfo == 1) {
        TableView.push("<th width='25%'>Thao tác</th>");
        iTotalCol++;
    }

    TableView.push("    </thead>");
    TableView.push("    <tbody>");

    if (data == undefined || data == null || data == []) {
        return TableView.join(" ");
    }

    var objCheck = SoftData(data, sKey, sKeyRefer, sParentKey);
    var index = 0;
    var sSpace = "";
    $.each(objCheck.result, function (indexItem, value) {
        var itemChild = $.map(data, function (n) { return n[sKey] == value ? n : null })[0];
        var dataRef = RecursiveTable(iTotalCol, index, itemChild, objCheck.parentData, columns, sItemHiden, button, sKey, sKeyRefer, data, sSpace, idTable, haveSttColumn)

        TableView.push(dataRef.sView);
        index = dataRef.index;
    });

    TableView.push("    </tbody>");
    TableView.push("</table>");
    return TableView.join(' ');
}

function RecursiveTable(iTotalCol, index, item, dicTree, columns, sItemHiden, button, sKey, sKeyRefer, data, sSpace, idTable, haveSttColumn = true) {
    index++;
    var TableView = [];
    TableView.push("<td width='5%'><input type='checkbox' name='cbQuyetToan' id='cbQuyetToan' data-id='" + item.ID + "' data-islock='" + item.bIsKhoa + "'/></td>");

    $.each(columns, function (indexItem, value) {
        if (value.sTitle != null && value.sTitle != undefined && value.sTitle != '') {
            if (value.sField == "iNamKeHoach") {
                TableView.push("<td align='" + value.sTextAlign + "' style='width:" + value.iWidth + "' data-getname='iNamKeHoach' data-getvalue='" + item['iNamKeHoach'] + "' class='" + (value.sClass == undefined ? "" : value.sClass) + "'>")
            } else {
                TableView.push("<td align='" + value.sTextAlign + "' style='width:" + value.iWidth + "' class='" + (value.sClass == undefined ? "" : value.sClass) + "'>")

            }
            // lay dong dau tien de hien thi icon va collape
            if (value.bHaveIcon == 1) {
                // neu co nhanh con , thi hien icon folder , neu khong co thi hien icon text
                if (dicTree[item[sKeyRefer]] != null) {
                    if (idTable != null && idTable != undefined) {
                        TableView.push(sSpace + "<i class='fa fa-caret-down' aria-hidden='true' data-toggle='collapse' data-target='#item-" + item[sKeyRefer] + "_" + idTable + "' aria-expanded='false' aria-controls='#item-" + item[sKeyRefer] + "_" + idTable + "'></i>")
                    } else {
                        TableView.push(sSpace + "<i class='fa fa-caret-down' aria-hidden='true' data-toggle='collapse' data-target='#item-" + item[sKeyRefer] + "' aria-expanded='false' aria-controls='#item-" + item[sKeyRefer] + "'></i>")
                    }

                    TableView.push("<i class='fa fa-folder-open' style='color:#ffc907'></i>")
                } else {
                    TableView.push(sSpace + "&nbsp;&nbsp;<i class='fa fa-file-text' style='color:#ffc907'></i>")
                }
                isFirst = false;
            }
            var itemText = [];
            $.each(value.sField.split('-'), function (indexField, valueField) {
                itemText.push(item[valueField]);
            });
            if (value.bHaveIcon != 1)
                TableView.push(itemText.join(" - "));
            else
                TableView.push(itemText.join(" - "));
            TableView.push("</td>");
        }
    });
    TableView.push("<td align='center' data-gettonghop='iID_TongHopID' data-gettonghopvalue='" + item.iID_TongHopID + "' hidden></td>")
    TableView.push(CreateButtonTable(item, sKey, sKeyRefer, button));
    TableView.push("</tr>");

    if (dicTree[item[sKeyRefer]] != null) {
        TableView.push("<tr>");
        if (idTable != null && idTable != undefined) {
            TableView.push("    <td colspan='" + iTotalCol + "' class='table-child collapse in' aria-expanded='true' style='padding:0px;' id='item-" + item[sKeyRefer] + "_" + idTable + "'>");
        } else {
            TableView.push("    <td colspan='" + iTotalCol + "' class='table-child collapse in' aria-expanded='true' style='padding:0px;' id='item-" + item[sKeyRefer] + "'>");
        }

        TableView.push("        <table class='table table-bordered'>");
        sSpace = sSpace + "&nbsp;&nbsp&nbsp;&nbsp"
        $.each(dicTree[item[sKeyRefer]], function (indexItem, value) {
            var itemChild = $.map(data, function (n) { return n[sKey] == value ? n : null })[0];
            var dataRef = RecursiveTable(iTotalCol, index, itemChild, dicTree, columns, sItemHiden, button, sKey, sKeyRefer, data, sSpace, idTable, haveSttColumn)

            TableView.push(dataRef.sView)
            index = dataRef.index;
        });

        TableView.push("        </table>");
        TableView.push("    </td>");
        TableView.push("</tr>");
    }
    return { sView: TableView.join(" "), index: index };
}

function CreateButtonTable(item, sKey, sKeyRefer, button) {
    var lstKey = [];
    lstKey.push("'" + item[sKey] + "'");
    if (button.bAddReferKey) {
        lstKey.push("'" + item[sKeyRefer] + "'");
    }
    var sParam = lstKey.join(",");


    var TableView = [];
    TableView.push("<td align='center' class='col-sm-12 col-btn' style='width:25%'>");
    TableView.push("<button type='button' class='btn-detail' onclick='OpenModalDetail(`" + item.ID + "`)' data-toggle='modal' data-target='#modalQTNienDo'><i class='fa fa-eye fa-lg' aria-hidden='true'></i></button>");
    if (item.sTongHop != null && item.sTongHop != "") {

        if (item.bIsKhoa == false) {
            TableView.push("<button type='button' class='btn-delete' onclick='Xoa(`" + item.ID + "`)' ><i class='fa fa-trash-o fa-lg' aria-hidden='true'></i></button>");
            TableView.push("<button type='button' class='btn-edit' onclick='LockItem(`" + item.ID + "`, `" + item.sSoDeNghi + "`, " + item.bIsKhoa + ",`" + item.iID_TongHopID + "`)' > <i class='fa fa-lock fa-lg' aria-hidden='true'></i></button>");
            TableView.push("<button type='button' class='btn-edit' onclick='OpenModal(`" + item.ID + "`)' data-toggle='modal' data-target='#modalQTNienDo' > <i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i></button>");
        } else {
            TableView.push("<button type='button' class='btn-edit' onclick='LockItem(`" + item.ID + "`, `" + item.sSoDeNghi + "`, " + item.bIsKhoa + ",`" + item.iID_TongHopID + "`)' > <i class='fa fa-unlock fa-lg' aria-hidden='true'></i></button>");
        }
    }

    TableView.push("</td>")


    return TableView.join(' ');
}

function SoftData(data, sKey, sKeyRefer, sParent) {
    var result = [];
    var parentData = [];
    $.each(data, function (indexItem, value) {

        var itemCheck = $.map(data, function (n) { return n[sParent] == value[sKeyRefer] ? n : null })[0];

        if (itemCheck != null && value[sKeyRefer] != null) {
            if (parentData[value[sKeyRefer]] == undefined) {
                parentData[value[sKeyRefer]] = [];
            }
        }

        if (value[sParent] == null) {
            result.push(value[sKey])
            return true;
        } else {
            if (parentData[value[sParent]] == undefined) {
                parentData[value[sParent]] = [];
            }
            parentData[value[sParent]].push(value[sKey]);
        }
    });
    var objResult = { parentData: parentData, result: result };
    return objResult;
}
