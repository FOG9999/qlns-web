var fileInput = document.getElementById('inputFile');
var LstFile = [];
var FileIndex = 0;
var ObjectiID = $("#inputFile_ObjectID").val();
var isDetail = $("#inputIsDetail").val();

$(document).ready(function ($) {
    FileIndex = 0;
    $("#inputFile").on("change", function () {
        fnChooseFiles();
    })
    if (ObjectiID) {
        GetTaiLieuDinhKem();
    }
    if (isDetail == "1") {
        $("#label_inputFile").css("display", "none");
    }
});

function GetTaiLieuDinhKem() {
    $.ajax({
        url: "/QLVonDauTu/Attachment/GetTaiLieuDinhKemByID",
        type: "POST",
        data: { ID: ObjectiID },
        cache: false,
        dataType: "json",
        success: function (data) {
            if (data.status) {
                for (var index = 0; index < data.lstAttachment.length; index++) {
                    var obj = data.lstAttachment[index];
                    obj.index = FileIndex;
                    LstFile.push(obj);
                    FileIndex++;
                }
                FillTaiLieuDinhKemTbl();
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}

var fileMessages = [];
function fnChooseFiles() {
    if (fileInput.files.length == 0) return;
    for (var i = 0; i < fileInput.files.length; ++i) {
        if (validateFile(fileInput.files[i])) {
            var obj = {};
            obj.Id = null;
            obj.index = FileIndex;
            obj.FileName = fileInput.files[i].name;
            obj.fileContent = fileInput.files[i];
            LstFile.push(obj);
            FileIndex++;
        }
    }
    if (fileMessages.length > 0) {
        alert(fileMessages.join('\n'));
    }
    FillTaiLieuDinhKemTbl();
}

function validateFile(file) {
    const fileType = file.name.substring(file.name.lastIndexOf('.')).toLowerCase();
    if (fileType === ".pdf" || fileType === ".xlsx" || fileType === ".png" || fileType === ".jpeg" || fileType === ".jpg" || fileType === ".doc" || fileType === ".docx" || fileType === ".xls") {
        if (file.size < 1e+7) {
            return true;
        } else {
            fileMessages.push(`Tài liệu ${file.name} vượt quá dung lượng cho phép, vui lòng chọn lại`);
            return false;
        }
    } {
        fileMessages.push(`Tài liệu ${file.name} không hợp lệ`);
        return false;
    }
}

function FillTaiLieuDinhKemTbl() {
    $("#tblThongTinTaiLieuDinhKem tbody").html('');
    for (index = 0; index < LstFile.length; index++) {
        var htmlTr = "<tr>";
        htmlTr += `<td>${index + 1}</td>`;
        htmlTr += `<td>${LstFile[index].FileName}</td>`;
        htmlTr += `<td class="text-center">
                    <button type="button" class="btn-detail" onclick="XemTaiLieuDinhKem(${LstFile[index].index})"><i class="fa fa-eye fa-lg" aria-hidden="true"></i></button>
                    <button type="button" class="btn-edit" onclick="TaiTaiLieuDinhKem(${LstFile[index].index})"><i class="fa fa-download fa-lg" aria-hidden="true"></i></button>`
        if (isDetail != "1") {
            htmlTr += `<button type="button" class="btn-delete" onclick="XoaTaiLieuDinhKem(${LstFile[index].index})"><i class="fa fa-trash-o fa-lg" aria-hidden="true"></i></button>`;
        }
        htmlTr += "</td>";
        htmlTr += "</tr>";
        $("#tblThongTinTaiLieuDinhKem tbody").append(htmlTr);
    }
}

var TaiLieuIDs = [];
function UploadFile(iID, moduleType) {
    var formdata = new FormData();
    for (let i = 0; i < LstFile.length; i++) {
        if (!LstFile[i].Id) {
            formdata.append(LstFile[i].FileName, LstFile[i].fileContent);
        } else {
            TaiLieuIDs.push(LstFile[i].Id);
        }
    }
    formdata.append("ID", iID);
    formdata.append("moduleType", moduleType);

    $.ajax({
        url: "/QLVonDauTu/Attachment/UploadFiles",
        type: "POST",
        data: formdata,
        cache: false,
        contentType: false,
        processData: false,
        success: function (data) {
            console.log(data.UploadedFileCount + ' file(s) uploaded successfully');
        },
        error: function (xhr, error, status) {
            console.log(error, status);
        }
    });
    return false;
}

function DieuChinhUploadFile(iID, moduleType) {
    UploadFile(iID, moduleType);

    if (TaiLieuIDs.length > 0) {
        $.ajax({
            url: "/QLVonDauTu/Attachment/DieuChinhUploadFile",
            type: "POST",
            data: { taiLieuIDs: TaiLieuIDs, ObjectID: iID },
            dataType: "json",
            cache: false,
            success: function (data) {
                console.log(data.UploadedFileCount + ' file(s) uploaded successfully');
            },
            error: function (xhr, error, status) {
                console.log(error, status);
            }
        });
    }
    return false;
}


function XemTaiLieuDinhKem(index) {
    if (LstFile[index].Id == null) {
        alert("File chưa tải lên server");
    } else {
        //LstFile[index].FileName
        var fileType = LstFile[index].FileName.substring(LstFile[index].FileName.lastIndexOf('.')).toLowerCase();
        if (fileType === ".pdf") {
            window.open("/QLVonDauTu/Attachment/XemTaiLieuDinhKem?AttachmentId=" + LstFile[index].Id);
        } else {
            alert("Định dạng file không hỗ trợ xem chi tiết");
        }
    }
}
function TaiTaiLieuDinhKem(index) {
    if (LstFile[index].Id == null) {
        alert("File chưa tải lên server");
    } else {
        window.open("/QLVonDauTu/Attachment/DownloadTaiLieuDinhKem?AttachmentId=" + LstFile[index].Id);
    }
}
function XoaTaiLieuDinhKem(index) {
    if (LstFile[index].Id) {
        $.ajax({
            url: "/QLVonDauTu/Attachment/DeleteTaiLieuDinhKemByID",
            type: "POST",
            data: { AttachmentId: LstFile[index].Id },
            dataType: "json",
            cache: false,
            success: function (resp) {
                if (resp == null || resp.status == false) {
                    alert("Chưa xóa được đính kèm");
                }
                else {
                    alert("Xóa thành công");
                }
            },
            error: function (data) {
                alert("Chưa xóa được đính kèm");
            }
        })
    }
    var lst = $.map(LstFile, function (x) { return x.index == index ? null : x });
    LstFile = lst;
    FillTaiLieuDinhKemTbl();
}

