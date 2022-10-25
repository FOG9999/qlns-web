/// data : list data input

//columns {
//    bKey: is primary key
//    bParentKey : is key check tree cha con
//    bForeignKey : is reference key
//    bHaveIcon : hien thi icon cha con
//    sTitle : Title hien thi(neu khong dien thi khong hien thi cot)
//    sField: Ten truong du lieu vat ly
//    iWidth: string width column(vi du : 10px or 10%)
//    sTextAlign: text - align css
//}

//button{
//    bAddReferKey : add refer key vao button 
//    bInfo : is create button view detail
//    bUpdate: is create button update
//    bDelete : is create button delete
//}

function GenerateTreeTableNCCQ(data, columns, button, idTable, haveSttColumn = true, isShowSearchDMLoaiCongTrinh = false) {
    var sParentKey = '';
    var sKeyRefer = "";
    var sKey = ''
    var TableView = [];
    var sItemHiden = [];
    var iTotalCol = 1;
    TableView.push("<table id='tblNhuCauChiQuyTongHop' class='table table-bordered table-parent' style='margin-bottom: 0px'>");
    TableView.push("<thead class='header-search'>");
    TableView.push("<tr>");
    TableView.push("<th width='15px'></th>");
    /*TableView.push("<th width='3%'></th>");*/
    TableView.push("<th width='16%'><input type='text' class='form-control clearable' placeholder='Số đề nghị' maxlength='100' id='txtSodenghiList' autocomplete='off' /></th>");
    TableView.push("<th width='10%'><div class='input-group date'><input type='text' id='txtNgaydenghiList' class='form-control'value=''autocomplete='off' placeholder='dd/MM/yyyy' /><span class='btn-default input-group-addon input-calendar'><i class='fa fa-calendar' aria-hidden='true'></i></span></div ></th>");
    TableView.push("<th width='13%'><input type='hidden' class='iBQuanLyList' /><div class='sBQuanLyList' hidden></div><div class='selectBQuanLyList'>" + CreateHtmlSelectBQuanLy() + "</div></th>");
    TableView.push("<th width='13%'><input type='hidden' class='iDonViList' /><div class='sDonViList' hidden></div><div class='selectDonViList'>" + CreateHtmlSelectDonVi() + "</div></th>");
    TableView.push("<th width='15%'><div style='display: flex'><div class='input-group' style='width:100%'><input type='hidden' class='iQuyList' /><div class='' hidden></div><div class='selectiQuyList'>" + CreateHtmlSelectQuy() + "</div></div><span style='margin: 8px 5px 0px 5px;'>-</span><div class='input-group'><input type='text' placeholder='Năm' class='form-control clearable gr_search' maxlength='4' id='txtNamList'/></div></div></th>");
    TableView.push("<th width='8%'></th>");
    TableView.push("<th width='8%'></th>");
    TableView.push("<th width='300px'><button class='btn btn-info' onclick='GetListData_By_Name()'><i class='fa fa-search'></i>Tìm kiếm</button> </th>");
    TableView.push("</tr>");
    TableView.push("</thead>");
        
    TableView.push("<thead>");
    TableView.push("<th width='10px'></th>");
    
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
        TableView.push("<th width='300px'>Thao tác</th>");
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
    var isFirst = true;
    var lstHiddenData = [];
    TableView.push("<td width='15px'><input type='checkbox' data-id='" + item.ID + "' class='" + item.ID + "'/></td>");

    $.each(columns, function (indexItem, value) {
        if (value.sTitle != null && value.sTitle != undefined && value.sTitle != '') {
            TableView.push("<td align='" + value.sTextAlign + "' style='width:" + value.iWidth + "' class='" + (value.sClass == undefined ? "" : value.sClass) + "'>")
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
    TableView.push("<td align='center' class='col-sm-12 col-btn' style='width:300px'>");
    if (item.sTongHop == null || item.sTongHop == "") {
        TableView.push("<button type='button' class='btn-detail' onclick='OpenModalDetail(`" + item.ID + "`)' data-toggle='modal' data-target='#modalNhuCauChiQuy'><i class='fa fa-eye fa-lg' aria-hidden='true'></i></button>");

        TableView.push("<button type='button' class='btn-delete invisible' onclick='Delete(`" + item.ID + "`)' data-toggle='modal' data-target='#modalNhuCauChiQuy'><i class='fa fa-trash-o fa-lg' aria-hidden='true'></i></button>");
        TableView.push("<button type='button' class='btn-edit invisible' onclick='LockItem(`" + item.ID + "`, `" + item.sSoDeNghi + "`, `" + item.bIsKhoa + "`)' > <i class='fa fa-lock fa-lg' aria-hidden='true'></i></button>");
        TableView.push("<button type='button' class='btn-edit invisible' onclick='LockItem(`" + item.ID + "`, `" + item.sSoDeNghi + "`, `" + item.bIsKhoa + "`)' > <i class='fa fa-lock fa-lg' aria-hidden='true'></i></button>");
    }
    else {

        TableView.push("<button type='button' class='btn-detail' onclick='OpenModalDetailTongHop(`" + item.ID + "`)' data-toggle='modal' data-target='#modalNhuCauChiQuy'><i class='fa fa-eye fa-lg' aria-hidden='true'></i></button>");
        if (item.bIsKhoa == false) {
            TableView.push("<button type='button' class='btn-edit' onclick='OpenModalGeneral(`" + item.ID + "`, `false`, `true`)' data-toggle='modal' data-target='#modalNhuCauChiQuy' > <i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i></button>");
            TableView.push("<button type='button' class='btn-delete' onclick='Delete(`" + item.ID + "`)' data-toggle='modal' data-target='#modalNhuCauChiQuy'><i class='fa fa-trash-o fa-lg' aria-hidden='true'></i></button>");
            TableView.push("<button type='button' class='btn-edit' onclick='LockItem(`" + item.ID + "`, `" + item.sSoDeNghi + "`, `" + item.bIsKhoa + "`)' > <i class='fa fa-lock fa-lg' aria-hidden='true'></i></button>");
        }
        else {
            TableView.push("<button type='button' class='btn-delete invisible' onclick='Delete(`" + item.ID + "`)' data-toggle='modal' data-target='#modalNhuCauChiQuy'><i class='fa fa-trash-o fa-lg' aria-hidden='true'></i></button>");
            TableView.push("<button type='button' class='btn-edit invisible' onclick='LockItem(`" + item.ID + "`, `" + item.sSoDeNghi + "`, `" + item.bIsKhoa + "`)' > <i class='fa fa-lock fa-lg' aria-hidden='true'></i></button>");
            if (item.iID_TongHopID == null) {
                TableView.push("<button type='button' class='btn-edit' onclick='LockItem(`" + item.ID + "`, `" + item.sSoDeNghi + "`, `" + item.bIsKhoa + "`)' > <i class='fa fa-unlock fa-lg' aria-hidden='true'></i></button>");
            } else {
                TableView.push("<button type='button' class='btn-edit invisible' onclick='LockItem(`" + item.ID + "`, `" + item.sSoDeNghi + "`, `" + item.bIsKhoa + "`)' > <i class='fa fa-lock fa-lg' aria-hidden='true'></i></button>");
            }
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
