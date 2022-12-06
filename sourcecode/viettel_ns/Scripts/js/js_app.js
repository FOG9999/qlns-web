
function ConvertDatetimeToJSon(dateString) {
    if (dateString == undefined || dateString == null || dateString == '') return null;
    var currentTime = new Date(parseInt(dateString.substr(6)));
    var month = ("0" + (currentTime.getMonth() + 1)).slice(-2);
    var day = ("0" + currentTime.getDate()).slice(-2);
    var year = currentTime.getFullYear();
    var date = day + "/" + month + "/" + year;
    return date;
}

function NewGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

// Filter in select
function FilterInComboBox(params, data) {
    // If there are no search terms, return all of the data
    if ($.trim(params.term) === '') {
        return data;
    }

    // Do not display the item if there is no 'text' property
    if (typeof data.text === 'undefined') {
        return null;
    }

    if (data.text.toUpperCase().indexOf(UnicodeToCompositeUnicode(params.term.toUpperCase())) > -1 || data.text.toUpperCase().indexOf(CompositeUnicodeToUnicode(params.term.toUpperCase())) > -1) {
        var modifiedData = $.extend({}, data, true);
        //modifiedData.text += ' (matched)';
        return modifiedData;
    }

    return null;
}

// Chuyển đổi chuỗi Unicode tổ hợp sang chuỗi unicode dựng sẵn VD: "Một ngày đẹp trời" => "Một ngày đẹp trời".
// Nhìn 2 chuỗi thì không khác gì nhau nhưng sử dụng 2 bảng mã khác nhau là Unicode và Unicode tổ hợp nên: "Một ngày đẹp trời" != "Một ngày đẹp trời"
function CompositeUnicodeToUnicode(unicode_str) {
    unicode_str = unicode_str.replace("\u0065\u0309", "\u1EBB")    // ẻ
    unicode_str = unicode_str.replace("\u0065\u0301", "\u00E9")    // é
    unicode_str = unicode_str.replace("\u0065\u0300", "\u00E8")    // è
    unicode_str = unicode_str.replace("\u0065\u0323", "\u1EB9")    // ẹ
    unicode_str = unicode_str.replace("\u0065\u0303", "\u1EBD")    // ẽ
    unicode_str = unicode_str.replace("\u00EA\u0309", "\u1EC3")    // ể
    unicode_str = unicode_str.replace("\u00EA\u0301", "\u1EBF")    // ế
    unicode_str = unicode_str.replace("\u00EA\u0300", "\u1EC1")    // ề
    unicode_str = unicode_str.replace("\u00EA\u0323", "\u1EC7")    // ệ
    unicode_str = unicode_str.replace("\u00EA\u0303", "\u1EC5")    // ễ
    unicode_str = unicode_str.replace("\u0079\u0309", "\u1EF7")    // ỷ
    unicode_str = unicode_str.replace("\u0079\u0301", "\u00FD")    // ý
    unicode_str = unicode_str.replace("\u0079\u0300", "\u1EF3")    // ỳ
    unicode_str = unicode_str.replace("\u0079\u0323", "\u1EF5")    // ỵ
    unicode_str = unicode_str.replace("\u0079\u0303", "\u1EF9")    // ỹ
    unicode_str = unicode_str.replace("\u0075\u0309", "\u1EE7")    // ủ
    unicode_str = unicode_str.replace("\u0075\u0301", "\u00FA")    // ú
    unicode_str = unicode_str.replace("\u0075\u0300", "\u00F9")    // ù
    unicode_str = unicode_str.replace("\u0075\u0323", "\u1EE5")    // ụ
    unicode_str = unicode_str.replace("\u0075\u0303", "\u0169")    // ũ
    unicode_str = unicode_str.replace("\u01B0\u0309", "\u1EED")    // ử
    unicode_str = unicode_str.replace("\u01B0\u0301", "\u1EE9")    // ứ
    unicode_str = unicode_str.replace("\u01B0\u0300", "\u1EEB")    // ừ
    unicode_str = unicode_str.replace("\u01B0\u0323", "\u1EF1")    // ự
    unicode_str = unicode_str.replace("\u01B0\u0303", "\u1EEF")    // ữ
    unicode_str = unicode_str.replace("\u0069\u0309", "\u1EC9")    // ỉ
    unicode_str = unicode_str.replace("\u0069\u0301", "\u00ED")    // í
    unicode_str = unicode_str.replace("\u0069\u0300", "\u00EC")    // ì
    unicode_str = unicode_str.replace("\u0069\u0323", "\u1ECB")    // ị
    unicode_str = unicode_str.replace("\u0069\u0303", "\u0129")    // ĩ
    unicode_str = unicode_str.replace("\u006F\u0309", "\u1ECF")    // ỏ
    unicode_str = unicode_str.replace("\u006F\u0301", "\u00F3")    // ó
    unicode_str = unicode_str.replace("\u006F\u0300", "\u00F2")    // ò
    unicode_str = unicode_str.replace("\u006F\u0323", "\u1ECD")    // ọ
    unicode_str = unicode_str.replace("\u006F\u0303", "\u00F5")    // õ
    unicode_str = unicode_str.replace("\u01A1\u0309", "\u1EDF")    // ở
    unicode_str = unicode_str.replace("\u01A1\u0301", "\u1EDB")    // ớ
    unicode_str = unicode_str.replace("\u01A1\u0300", "\u1EDD")    // ờ
    unicode_str = unicode_str.replace("\u01A1\u0323", "\u1EE3")    // ợ
    unicode_str = unicode_str.replace("\u01A1\u0303", "\u1EE1")    // ỡ
    unicode_str = unicode_str.replace("\u00F4\u0309", "\u1ED5")    // ổ
    unicode_str = unicode_str.replace("\u00F4\u0301", "\u1ED1")    // ố
    unicode_str = unicode_str.replace("\u00F4\u0300", "\u1ED3")    // ồ
    unicode_str = unicode_str.replace("\u00F4\u0323", "\u1ED9")    // ộ
    unicode_str = unicode_str.replace("\u00F4\u0303", "\u1ED7")    // ỗ
    unicode_str = unicode_str.replace("\u0061\u0309", "\u1EA3")    // ả
    unicode_str = unicode_str.replace("\u0061\u0301", "\u00E1")    // á
    unicode_str = unicode_str.replace("\u0061\u0300", "\u00E0")    // à
    unicode_str = unicode_str.replace("\u0061\u0323", "\u1EA1")    // ạ
    unicode_str = unicode_str.replace("\u0061\u0303", "\u00E3")    // ã
    unicode_str = unicode_str.replace("\u0103\u0309", "\u1EB3")    // ẳ
    unicode_str = unicode_str.replace("\u0103\u0301", "\u1EAF")    // ắ
    unicode_str = unicode_str.replace("\u0103\u0300", "\u1EB1")    // ằ
    unicode_str = unicode_str.replace("\u0103\u0323", "\u1EB7")    // ặ
    unicode_str = unicode_str.replace("\u0103\u0303", "\u1EB5")    // ẵ
    unicode_str = unicode_str.replace("\u00E2\u0309", "\u1EA9")    // ẩ
    unicode_str = unicode_str.replace("\u00E2\u0301", "\u1EA5")    // ấ
    unicode_str = unicode_str.replace("\u00E2\u0300", "\u1EA7")    // ầ
    unicode_str = unicode_str.replace("\u00E2\u0323", "\u1EAD")    // ậ
    unicode_str = unicode_str.replace("\u00E2\u0303", "\u1EAB")    // ẫ
    unicode_str = unicode_str.replace("\u0045\u0309", "\u1EBA")    // Ẻ
    unicode_str = unicode_str.replace("\u0045\u0301", "\u00C9")    // É
    unicode_str = unicode_str.replace("\u0045\u0300", "\u00C8")    // È
    unicode_str = unicode_str.replace("\u0045\u0323", "\u1EB8")    // Ẹ
    unicode_str = unicode_str.replace("\u0045\u0303", "\u1EBC")    // Ẽ
    unicode_str = unicode_str.replace("\u00CA\u0309", "\u1EC2")    // Ể
    unicode_str = unicode_str.replace("\u00CA\u0301", "\u1EBE")    // Ế
    unicode_str = unicode_str.replace("\u00CA\u0300", "\u1EC0")    // Ề
    unicode_str = unicode_str.replace("\u00CA\u0323", "\u1EC6")    // Ệ
    unicode_str = unicode_str.replace("\u00CA\u0303", "\u1EC4")    // Ễ
    unicode_str = unicode_str.replace("\u0059\u0309", "\u1EF6")    // Ỷ
    unicode_str = unicode_str.replace("\u0059\u0301", "\u00DD")    // Ý
    unicode_str = unicode_str.replace("\u0059\u0300", "\u1EF2")    // Ỳ
    unicode_str = unicode_str.replace("\u0059\u0323", "\u1EF4")    // Ỵ
    unicode_str = unicode_str.replace("\u0059\u0303", "\u1EF8")    // Ỹ
    unicode_str = unicode_str.replace("\u0055\u0309", "\u1EE6")    // Ủ
    unicode_str = unicode_str.replace("\u0055\u0301", "\u00DA")    // Ú
    unicode_str = unicode_str.replace("\u0055\u0300", "\u00D9")    // Ù
    unicode_str = unicode_str.replace("\u0055\u0323", "\u1EE4")    // Ụ
    unicode_str = unicode_str.replace("\u0055\u0303", "\u0168")    // Ũ
    unicode_str = unicode_str.replace("\u01AF\u0309", "\u1EEC")    // Ử
    unicode_str = unicode_str.replace("\u01AF\u0301", "\u1EE8")    // Ứ
    unicode_str = unicode_str.replace("\u01AF\u0300", "\u1EEA")    // Ừ
    unicode_str = unicode_str.replace("\u01AF\u0323", "\u1EF0")    // Ự
    unicode_str = unicode_str.replace("\u01AF\u0303", "\u1EEE")    // Ữ
    unicode_str = unicode_str.replace("\u0049\u0309", "\u1EC8")    // Ỉ
    unicode_str = unicode_str.replace("\u0049\u0301", "\u00CD")    // Í
    unicode_str = unicode_str.replace("\u0049\u0300", "\u00CC")    // Ì
    unicode_str = unicode_str.replace("\u0049\u0323", "\u1ECA")    // Ị
    unicode_str = unicode_str.replace("\u0049\u0303", "\u0128")    // Ĩ
    unicode_str = unicode_str.replace("\u004F\u0309", "\u1ECE")    // Ỏ
    unicode_str = unicode_str.replace("\u004F\u0301", "\u00D3")    // Ó
    unicode_str = unicode_str.replace("\u004F\u0300", "\u00D2")    // Ò
    unicode_str = unicode_str.replace("\u004F\u0323", "\u1ECC")    // Ọ
    unicode_str = unicode_str.replace("\u004F\u0303", "\u00D5")    // Õ
    unicode_str = unicode_str.replace("\u01A0\u0309", "\u1EDE")    // Ở
    unicode_str = unicode_str.replace("\u01A0\u0301", "\u1EDA")    // Ớ
    unicode_str = unicode_str.replace("\u01A0\u0300", "\u1EDC")    // Ờ
    unicode_str = unicode_str.replace("\u01A0\u0323", "\u1EE2")    // Ợ
    unicode_str = unicode_str.replace("\u01A0\u0303", "\u1EE0")    // Ỡ
    unicode_str = unicode_str.replace("\u00D4\u0309", "\u1ED4")    // Ổ
    unicode_str = unicode_str.replace("\u00D4\u0301", "\u1ED0")    // Ố
    unicode_str = unicode_str.replace("\u00D4\u0300", "\u1ED2")    // Ồ
    unicode_str = unicode_str.replace("\u00D4\u0323", "\u1ED8")    // Ộ
    unicode_str = unicode_str.replace("\u00D4\u0303", "\u1ED6")    // Ỗ
    unicode_str = unicode_str.replace("\u0041\u0309", "\u1EA2")    // Ả
    unicode_str = unicode_str.replace("\u0041\u0301", "\u00C1")    // Á
    unicode_str = unicode_str.replace("\u0041\u0300", "\u00C0")    // À
    unicode_str = unicode_str.replace("\u0041\u0323", "\u1EA0")    // Ạ
    unicode_str = unicode_str.replace("\u0041\u0303", "\u00C3")    // Ã
    unicode_str = unicode_str.replace("\u0102\u0309", "\u1EB2")    // Ẳ
    unicode_str = unicode_str.replace("\u0102\u0301", "\u1EAE")    // Ắ
    unicode_str = unicode_str.replace("\u0102\u0300", "\u1EB0")    // Ằ
    unicode_str = unicode_str.replace("\u0102\u0323", "\u1EB6")    // Ặ
    unicode_str = unicode_str.replace("\u0102\u0303", "\u1EB4")    // Ẵ
    unicode_str = unicode_str.replace("\u00C2\u0309", "\u1EA8")    // Ẩ
    unicode_str = unicode_str.replace("\u00C2\u0301", "\u1EA4")    // Ấ
    unicode_str = unicode_str.replace("\u00C2\u0300", "\u1EA6")    // Ầ
    unicode_str = unicode_str.replace("\u00C2\u0323", "\u1EAC")    // Ậ
    unicode_str = unicode_str.replace("\u00C2\u0303", "\u1EAA")    // Ẫ
    return unicode_str
}

// Chuyển đổi chuỗi unicode dựng sẵn sang chuỗi unicode tổ hợp VD: "Một ngày đẹp trời" => "Một ngày đẹp trời".
function UnicodeToCompositeUnicode(unicode_str) {
    unicode_str = unicode_str.replace("\u1EBB", "\u0065\u0309")    // ẻ
    unicode_str = unicode_str.replace("\u00E9", "\u0065\u0301")    // é
    unicode_str = unicode_str.replace("\u00E8", "\u0065\u0300")    // è
    unicode_str = unicode_str.replace("\u1EB9", "\u0065\u0323")    // ẹ
    unicode_str = unicode_str.replace("\u1EBD", "\u0065\u0303")    // ẽ
    unicode_str = unicode_str.replace("\u1EC3", "\u00EA\u0309")    // ể
    unicode_str = unicode_str.replace("\u1EBF", "\u00EA\u0301")    // ế
    unicode_str = unicode_str.replace("\u1EC1", "\u00EA\u0300")    // ề
    unicode_str = unicode_str.replace("\u1EC7", "\u00EA\u0323")    // ệ
    unicode_str = unicode_str.replace("\u1EC5", "\u00EA\u0303")    // ễ
    unicode_str = unicode_str.replace("\u1EF7", "\u0079\u0309")    // ỷ
    unicode_str = unicode_str.replace("\u00FD", "\u0079\u0301")    // ý
    unicode_str = unicode_str.replace("\u1EF3", "\u0079\u0300")    // ỳ
    unicode_str = unicode_str.replace("\u1EF5", "\u0079\u0323")    // ỵ
    unicode_str = unicode_str.replace("\u1EF9", "\u0079\u0303")    // ỹ
    unicode_str = unicode_str.replace("\u1EE7", "\u0075\u0309")    // ủ
    unicode_str = unicode_str.replace("\u00FA", "\u0075\u0301")    // ú
    unicode_str = unicode_str.replace("\u00F9", "\u0075\u0300")    // ù
    unicode_str = unicode_str.replace("\u1EE5", "\u0075\u0323")    // ụ
    unicode_str = unicode_str.replace("\u0169", "\u0075\u0303")    // ũ
    unicode_str = unicode_str.replace("\u1EED", "\u01B0\u0309")    // ử
    unicode_str = unicode_str.replace("\u1EE9", "\u01B0\u0301")    // ứ
    unicode_str = unicode_str.replace("\u1EEB", "\u01B0\u0300")    // ừ
    unicode_str = unicode_str.replace("\u1EF1", "\u01B0\u0323")    // ự
    unicode_str = unicode_str.replace("\u1EEF", "\u01B0\u0303")    // ữ
    unicode_str = unicode_str.replace("\u1EC9", "\u0069\u0309")    // ỉ
    unicode_str = unicode_str.replace("\u00ED", "\u0069\u0301")    // í
    unicode_str = unicode_str.replace("\u00EC", "\u0069\u0300")    // ì
    unicode_str = unicode_str.replace("\u1ECB", "\u0069\u0323")    // ị
    unicode_str = unicode_str.replace("\u0129", "\u0069\u0303")    // ĩ
    unicode_str = unicode_str.replace("\u1ECF", "\u006F\u0309")    // ỏ
    unicode_str = unicode_str.replace("\u00F3", "\u006F\u0301")    // ó
    unicode_str = unicode_str.replace("\u00F2", "\u006F\u0300")    // ò
    unicode_str = unicode_str.replace("\u1ECD", "\u006F\u0323")    // ọ
    unicode_str = unicode_str.replace("\u00F5", "\u006F\u0303")    // õ
    unicode_str = unicode_str.replace("\u1EDF", "\u01A1\u0309")    // ở
    unicode_str = unicode_str.replace("\u1EDB", "\u01A1\u0301")    // ớ
    unicode_str = unicode_str.replace("\u1EDD", "\u01A1\u0300")    // ờ
    unicode_str = unicode_str.replace("\u1EE3", "\u01A1\u0323")    // ợ
    unicode_str = unicode_str.replace("\u1EE1", "\u01A1\u0303")    // ỡ
    unicode_str = unicode_str.replace("\u1ED5", "\u00F4\u0309")    // ổ
    unicode_str = unicode_str.replace("\u1ED1", "\u00F4\u0301")    // ố
    unicode_str = unicode_str.replace("\u1ED3", "\u00F4\u0300")    // ồ
    unicode_str = unicode_str.replace("\u1ED9", "\u00F4\u0323")    // ộ
    unicode_str = unicode_str.replace("\u1ED7", "\u00F4\u0303")    // ỗ
    unicode_str = unicode_str.replace("\u1EA3", "\u0061\u0309")    // ả
    unicode_str = unicode_str.replace("\u00E1", "\u0061\u0301")    // á
    unicode_str = unicode_str.replace("\u00E0", "\u0061\u0300")    // à
    unicode_str = unicode_str.replace("\u1EA1", "\u0061\u0323")    // ạ
    unicode_str = unicode_str.replace("\u00E3", "\u0061\u0303")    // ã
    unicode_str = unicode_str.replace("\u1EB3", "\u0103\u0309")    // ẳ
    unicode_str = unicode_str.replace("\u1EAF", "\u0103\u0301")    // ắ
    unicode_str = unicode_str.replace("\u1EB1", "\u0103\u0300")    // ằ
    unicode_str = unicode_str.replace("\u1EB7", "\u0103\u0323")    // ặ
    unicode_str = unicode_str.replace("\u1EB5", "\u0103\u0303")    // ẵ
    unicode_str = unicode_str.replace("\u1EA9", "\u00E2\u0309")    // ẩ
    unicode_str = unicode_str.replace("\u1EA5", "\u00E2\u0301")    // ấ
    unicode_str = unicode_str.replace("\u1EA7", "\u00E2\u0300")    // ầ
    unicode_str = unicode_str.replace("\u1EAD", "\u00E2\u0323")    // ậ
    unicode_str = unicode_str.replace("\u1EAB", "\u00E2\u0303")    // ẫ
    unicode_str = unicode_str.replace("\u1EBA", "\u0045\u0309")    // Ẻ
    unicode_str = unicode_str.replace("\u00C9", "\u0045\u0301")    // É
    unicode_str = unicode_str.replace("\u00C8", "\u0045\u0300")    // È
    unicode_str = unicode_str.replace("\u1EB8", "\u0045\u0323")    // Ẹ
    unicode_str = unicode_str.replace("\u1EBC", "\u0045\u0303")    // Ẽ
    unicode_str = unicode_str.replace("\u1EC2", "\u00CA\u0309")    // Ể
    unicode_str = unicode_str.replace("\u1EBE", "\u00CA\u0301")    // Ế
    unicode_str = unicode_str.replace("\u1EC0", "\u00CA\u0300")    // Ề
    unicode_str = unicode_str.replace("\u1EC6", "\u00CA\u0323")    // Ệ
    unicode_str = unicode_str.replace("\u1EC4", "\u00CA\u0303")    // Ễ
    unicode_str = unicode_str.replace("\u1EF6", "\u0059\u0309")    // Ỷ
    unicode_str = unicode_str.replace("\u00DD", "\u0059\u0301")    // Ý
    unicode_str = unicode_str.replace("\u1EF2", "\u0059\u0300")    // Ỳ
    unicode_str = unicode_str.replace("\u1EF4", "\u0059\u0323")    // Ỵ
    unicode_str = unicode_str.replace("\u1EF8", "\u0059\u0303")    // Ỹ
    unicode_str = unicode_str.replace("\u1EE6", "\u0055\u0309")    // Ủ
    unicode_str = unicode_str.replace("\u00DA", "\u0055\u0301")    // Ú
    unicode_str = unicode_str.replace("\u00D9", "\u0055\u0300")    // Ù
    unicode_str = unicode_str.replace("\u1EE4", "\u0055\u0323")    // Ụ
    unicode_str = unicode_str.replace("\u0168", "\u0055\u0303")    // Ũ
    unicode_str = unicode_str.replace("\u1EEC", "\u01AF\u0309")    // Ử
    unicode_str = unicode_str.replace("\u1EE8", "\u01AF\u0301")    // Ứ
    unicode_str = unicode_str.replace("\u1EEA", "\u01AF\u0300")    // Ừ
    unicode_str = unicode_str.replace("\u1EF0", "\u01AF\u0323")    // Ự
    unicode_str = unicode_str.replace("\u1EEE", "\u01AF\u0303")    // Ữ
    unicode_str = unicode_str.replace("\u1EC8", "\u0049\u0309")    // Ỉ
    unicode_str = unicode_str.replace("\u00CD", "\u0049\u0301")    // Í
    unicode_str = unicode_str.replace("\u00CC", "\u0049\u0300")    // Ì
    unicode_str = unicode_str.replace("\u1ECA", "\u0049\u0323")    // Ị
    unicode_str = unicode_str.replace("\u0128", "\u0049\u0303")    // Ĩ
    unicode_str = unicode_str.replace("\u1ECE", "\u004F\u0309")    // Ỏ
    unicode_str = unicode_str.replace("\u00D3", "\u004F\u0301")    // Ó
    unicode_str = unicode_str.replace("\u00D2", "\u004F\u0300")    // Ò
    unicode_str = unicode_str.replace("\u1ECC", "\u004F\u0323")    // Ọ
    unicode_str = unicode_str.replace("\u00D5", "\u004F\u0303")    // Õ
    unicode_str = unicode_str.replace("\u1EDE", "\u01A0\u0309")    // Ở
    unicode_str = unicode_str.replace("\u1EDA", "\u01A0\u0301")    // Ớ
    unicode_str = unicode_str.replace("\u1EDC", "\u01A0\u0300")    // Ờ
    unicode_str = unicode_str.replace("\u1EE2", "\u01A0\u0323")    // Ợ
    unicode_str = unicode_str.replace("\u1EE0", "\u01A0\u0303")    // Ỡ
    unicode_str = unicode_str.replace("\u1ED4", "\u00D4\u0309")    // Ổ
    unicode_str = unicode_str.replace("\u1ED0", "\u00D4\u0301")    // Ố
    unicode_str = unicode_str.replace("\u1ED2", "\u00D4\u0300")    // Ồ
    unicode_str = unicode_str.replace("\u1ED8", "\u00D4\u0323")    // Ộ
    unicode_str = unicode_str.replace("\u1ED6", "\u00D4\u0303")    // Ỗ
    unicode_str = unicode_str.replace("\u1EA2", "\u0041\u0309")    // Ả
    unicode_str = unicode_str.replace("\u00C1", "\u0041\u0301")    // Á
    unicode_str = unicode_str.replace("\u00C0", "\u0041\u0300")    // À
    unicode_str = unicode_str.replace("\u1EA0", "\u0041\u0323")    // Ạ
    unicode_str = unicode_str.replace("\u00C3", "\u0041\u0303")    // Ã
    unicode_str = unicode_str.replace("\u1EB2", "\u0102\u0309")    // Ẳ
    unicode_str = unicode_str.replace("\u1EAE", "\u0102\u0301")    // Ắ
    unicode_str = unicode_str.replace("\u1EB0", "\u0102\u0300")    // Ằ
    unicode_str = unicode_str.replace("\u1EB6", "\u0102\u0323")    // Ặ
    unicode_str = unicode_str.replace("\u1EB4", "\u0102\u0303")    // Ẵ
    unicode_str = unicode_str.replace("\u1EA8", "\u00C2\u0309")    // Ẩ
    unicode_str = unicode_str.replace("\u1EA4", "\u00C2\u0301")    // Ấ
    unicode_str = unicode_str.replace("\u1EA6", "\u00C2\u0300")    // Ầ
    unicode_str = unicode_str.replace("\u1EAC", "\u00C2\u0323")    // Ậ
    unicode_str = unicode_str.replace("\u1EAA", "\u00C2\u0303")    // Ẫ
    return unicode_str
}

/*typeCheck=1: Check number (with format)
  typeCheck=2: Check date (Format: dd/MM/yyyy)
  typeCheck=other: Check with regular expression*/
function setInputFilter(textbox, inputFilter, errMsg, typeCheck, isFormat) {
    if (isFormat == undefined || isFormat == null) isFormat = true;
    if (typeCheck == undefined || typeCheck == null) typeCheck = 1;
    ["input", "keydown", "keyup", "mousedown", "mouseup", "select", "contextmenu", "drop", "focusout"].forEach(function (event) {
        textbox.addEventListener(event, function (e) {
            if ((typeCheck == 1 && inputFilter(this.value.replace(/\./gi, "")) && this.value != ","
                && (this.value.charAt(this.value.length - 1) != ".") && !(/^[0]{2,}/.test(this.value) || /^[0]{1}[1-9]+/.test(this.value)))
                || (typeCheck == 2 && inputFilter(this.value)
                    && ((this.value.length < 3 && this.value.split("/").length == 1 && !dateIsValid(this.value))
                        || (this.value.length >= 3 && this.value.length < 6 && this.value.split("/").length == 2 && this.value.charAt(2) == "/" && !dateIsValid(this.value))
                        || (this.value.length >= 6 && this.value.length < 10 && this.value.charAt(5) == "/" && !dateIsValid(this.value))
                        || (this.value.length == 10)
                        || (this.value.length < 10 && this.value.split("/").length == 3)))
                || (typeCheck != 1 && typeCheck != 2 && inputFilter(this.value))
            ) {
                // Accepted value
                if ((["keydown", "mousedown", "focusout"].indexOf(e.type) >= 0)
                    || (typeCheck == 2 && inputFilter(this.value) && this.value.length == 9 && !dateIsValid(this.value))) {
                    this.classList.remove("input-error");
                    this.setCustomValidity("");
                }
                if (typeCheck == 1 && isFormat && this.value != "") {
                    if (parseFloat(UnFormatNumber(this.value)) != 0) {
                        this.value = FormatNumber(UnFormatNumber(this.value));
                    }
                }
                if (typeCheck == 2 && inputFilter(this.value) && this.value.length == 10 && this.value.split("/").length == 3 && !dateIsValid(this.value)) {
                    this.classList.add("input-error");
                    this.setCustomValidity(errMsg);
                    this.reportValidity();
                }
                this.oldValue = this.value;
                this.oldSelectionStart = this.selectionStart;
                this.oldSelectionEnd = this.selectionEnd;
            } else if (this.hasOwnProperty("oldValue")) {
                // Rejected value - restore the previous one
                if ((typeCheck == 1 && this.value != "," && !(/^[0]{2,}/.test(this.value) || /^[0]{1}[1-9]+/.test(this.value)))
                    || (typeCheck == 2 && !(inputFilter(this.value)
                        && ((this.value.length >= 3 && this.value.length < 6 && this.value.split("/").length == 2 && this.value.charAt(2) == "/" && !dateIsValid(this.value))
                        || (this.value.length >= 6 && this.value.length < 10 && this.value.charAt(5) == "/" && !dateIsValid(this.value))
                        || (this.value.length < 10 && this.value.split("/").length == 3))))
                    || (typeCheck != 1 && typeCheck != 2 && !inputFilter(this.value))
                ) {
                    this.classList.add("input-error");
                    this.setCustomValidity(errMsg);
                    this.reportValidity();
                }
                this.value = this.oldValue;
                this.setSelectionRange(this.oldSelectionStart, this.oldSelectionEnd);
            } else {
                // Rejected value - nothing to restore
                this.value = "";
            }
        });
    });
}

/**
 * Hàm chặn input khi keydown
 * @param {any} event
 * @param {any} textbox
 * @param {any} type: type=1 --> loại nhập số nguyên dương
 *                    type=2 --> loại nhập số thập phân dương (ngăn cách bằng dấu phẩy)
 *                    type=3 --> loại nhập ngày tháng năm (ngăn cách bằng dấu "/")
 *                    type=4 --> loại nhập số nguyên có dấu
 *                    type=5 --> loại nhập số thập phân có dấu (ngăn cách bằng dấu phẩy)
 * @param {boolean} useEnter : Check xem có sử dụng phím Enter hay không?
 */
function ValidateInputKeydown(event, textbox, type, useEnter) {
    var charCode = (event.which) ? event.which : event.keyCode;
    var arrSpecialChar = ["!", "@", "#", "$", "%", "^", "&", "*", "(", ")"];
    useEnter = (useEnter == null || useEnter == undefined) ? false : useEnter;
    var checkKey = (charCode < 48 || (charCode > 57 && charCode < 96) || charCode > 105 || $.inArray(event.key, arrSpecialChar) !== -1)
                    && charCode != 8 // backspace
                    && charCode != 9 // tab
                    && charCode != 37 // left arrow
                    && charCode != 39 // right arrow
                    && charCode != 46 // nút delete
                    && !(useEnter && charCode == 13) // Enter
                    && !(event.shiftKey && charCode == 9) // shift + tab
                    && !(event.shiftKey && charCode == 37) // shift + left arrow
                    && !(event.shiftKey && charCode == 38) // shift + top arrow
                    && !(event.shiftKey && charCode == 39) // shift + right arrow
                    && !(event.shiftKey && charCode == 40) // shift + bottom arrow
                    && !((event.ctrlKey && event.metaKey) && charCode == 37) // ctrl + left arrow
                    && !((event.ctrlKey && event.metaKey) && charCode == 38) // ctrl + top arrow
                    && !((event.ctrlKey && event.metaKey) && charCode == 39) // ctrl + right arrow
                    && !((event.ctrlKey && event.metaKey) && charCode == 40) // ctrl + bottom arrow
                    && !((event.ctrlKey && event.metaKey) && charCode == 65) // ctrl + a
                    && !((event.ctrlKey && event.metaKey) && charCode == 67) // ctrl + c
                    && !((event.ctrlKey && event.metaKey) && charCode == 86) // ctrl + v
                    && !((event.ctrlKey && event.metaKey) && charCode == 88) // ctrl + x
                    && !((event.ctrlKey && event.metaKey) && charCode == 89) // ctrl + y
                    && !((event.ctrlKey && event.metaKey) && charCode == 90) // ctrl + z
    switch (type) {
        case 1:
            if (checkKey) {
                PreventInputKey(event);
                return false;
            }
            break;
        case 2:
            if (checkKey
                && !(textbox.value.indexOf(",") < 0 && charCode == 188) // dấu phẩy ","
            ) {
                PreventInputKey(event);
                return false;
            }
            break;
        case 3:
            if (checkKey
                && (textbox.value.split("/").length >= 3 || (charCode != 191 && charCode != 111)) // dấu gạch chéo "/" trên phím thường và numpad
            ) {
                PreventInputKey(event);
                return false;
            }
            break;
        case 4:
            if (checkKey
                && charCode != 189 // dấu trừ
                && charCode != 109 // dấu trừ numpad
            ) {
                PreventInputKey(event);
                return false;
            }
            break;
        case 5:
            if (checkKey
                && !(textbox.value.indexOf(",") < 0 && charCode == 188) // dấu phẩy ","
                && charCode != 189 // dấu trừ
                && charCode != 109 // dấu trừ numpad
            ) {
                PreventInputKey(event);
                return false;
            }
            break;
    }
    return true;
}

function PreventInputKey(event) {
    if (event.preventDefault) { // W3C
        event.preventDefault();
    } else { // IE
        event.returnValue = false;
    }
    // Cancel visible action
    if (event.stopPropagation) { // W3C
        event.stopPropagation();
    } else { // IE
        event.cancelBubble = true;
    }
}

/**
 * Hàm validate khi bỏ focus ra khỏi element
 * @param {any} event
 * @param {any} textbox
 * @param {any} type: type=1 --> loại nhập số nguyên dương
 *                    type=2 --> loại nhập số thập phân dương (ngăn cách bằng dấu phẩy)
 *                    type=3 --> loại nhập ngày tháng năm (dd/MM/yyyy)
 *                    type=4 --> loại nhập số nguyên có dấu
 *                    type=5 --> loại nhập số thập phân có dấu (ngăn cách bằng dấu phẩy)
 *                    type=6 --> loại nhập năm (year)
 * @param {any} nTofix: số phần thập phân đằng sau dấu phẩy (type=2 hoặc type=5)
 * @param {any} bCoDau1000: xác định có sử dụng dấu chấm "." phân cách số hay không (type=2 hoặc type=5)
 * @param {any} isNotFormatNumber: xác định có format lại số hay không (type=2 hoặc type=5)
 */
function ValidateInputFocusOut(event, textbox, type, nTofix, bCoDau1000, isNotFormatNumber) {
    if (textbox.value == "") return true;
    switch (type) {
        case 1:
            if (!/^\d*$/.test(textbox.value)) {
                textbox.setCustomValidity("Không đúng định dạng số nguyên!");
                textbox.reportValidity();
                return false;
            } else {
                textbox.setCustomValidity("");
            }
            break;
        case 2:
            var txtValue = UnFormatInputNumber(textbox.value);
            if (!/^\d+(\.\d*)?$/.test(txtValue) || (typeof nTofix != "undefined" && nTofix != null && txtValue.split(".").length == 2 && txtValue.split(".")[1].length > 2)) {
                if (typeof nTofix != "undefined" && nTofix != null) {
                    textbox.setCustomValidity("Không đúng định dạng số thập phân có " + nTofix + " chữ số sau dấu phẩy!");
                } else {
                    textbox.setCustomValidity("Không đúng định dạng số thập phân!");
                }
                textbox.reportValidity();
                return false;
            } else {
                textbox.setCustomValidity("");
                if (!isNotFormatNumber) {
                    textbox.value = UnFormatInputNumber(textbox.value);
                    textbox.value = parseFloat(textbox.value);
                    textbox.value = FormatInputNumber(textbox.value, nTofix, bCoDau1000);
                }
            }
            break;
        case 3:
            if (!dateIsValid(textbox.value)) {
                textbox.setCustomValidity("Không đúng định dạng dd/MM/yyyy hoặc không hợp lệ!");
                textbox.reportValidity();
                return false;
            } else {
                textbox.setCustomValidity("");
            }
            break;
        case 4:
            if (!/^[-]?\d*$/.test(textbox.value) || textbox.value == "-") {
                textbox.setCustomValidity("Không đúng định dạng nguyên!");
                textbox.reportValidity();
                return false;
            } else {
                textbox.setCustomValidity("");
            }
            break;
        case 5:
            var txtValue = UnFormatInputNumber(textbox.value);
            if (!/^[-]?\d+(\.\d*)?$/.test(txtValue) || (typeof nTofix != "undefined" && nTofix != null && txtValue.split(".").length == 2 && txtValue.split(".")[1].length > nTofix)) {
                if (typeof nTofix != "undefined" && nTofix != null) {
                    textbox.setCustomValidity("Không đúng định dạng số thập phân có " + nTofix + " chữ số sau dấu phẩy!");
                } else {
                    textbox.setCustomValidity("Không đúng định dạng số thập phân!");
                }
                textbox.reportValidity();
                return false;
            } else {
                textbox.setCustomValidity("");
                if (!isNotFormatNumber) {
                    textbox.value = UnFormatInputNumber(textbox.value);
                    textbox.value = parseFloat(textbox.value);
                    textbox.value = FormatInputNumber(textbox.value, nTofix, bCoDau1000);
                }
            }
            break;
        case 6:
            if (!/^\d*$/.test(textbox.value) || parseInt(textbox.value) < 1000 || parseInt(textbox.value) > 9999) {
                textbox.setCustomValidity("Không đúng định dạng năm!");
                textbox.reportValidity();
                return false;
            } else {
                textbox.setCustomValidity("");
            }
            break;
    }
    return true;
}

var character_KieuSoTiengViet = true;
function UnFormatInputNumber(value) {
    value = value.toString();
    if (value == "") return value;
    if (character_KieuSoTiengViet) {
        value = value.replace(/\./gi, "");
        value = value.replace(/\,/gi, ".");
    } else {
        value = value.replace(/\,/gi, "");
    }
    return value;
}

//Định dạng số nhập vào
var character_DauCach1000 = ".";
var character_DauCachThapPhan = ",";
function FormatInputNumber(fnum, nToFixed, bCoDau1000) {
    if (fnum == null || fnum == 0) {
        return "";
    }
    var orgfnum = fnum.toString();


    if (typeof nToFixed != "undefined" && nToFixed != null && nToFixed >= 0) {
        var dCham = orgfnum.indexOf(".");
        var strChuanSo = orgfnum;
        if (dCham > 0) {
            strChuanSo = orgfnum.substr(0, dCham);
        }
        else {
            dCham = orgfnum.length - 1;
        }
        if (nToFixed > 0) {
            strChuanSo = strChuanSo + '.';
        }
        for (var i1 = dCham + 1; i1 < dCham + 1 + nToFixed; i1++) {
            if (i1 < orgfnum.length) {
                strChuanSo = strChuanSo + orgfnum.charAt(i1);;
            }
            else {
                strChuanSo = strChuanSo + '0';
            }
        }
        orgfnum = strChuanSo;
    }

    if (typeof bCoDau1000 == "undefined" || bCoDau1000 == null) {
        bCoDau1000 = true;
    }

    var snum = orgfnum;
    var flagneg = false;

    if (snum.length > 0 && snum.charAt(0) == "-") {
        flagneg = true;
        snum = snum.substr(1, snum.length - 1);
    }

    psplit = snum.split(".");

    var cnum = psplit[0], parr = '', j = cnum.length, d = 0;

    for (var i = j - 1; i >= 0; i--) {
        d = d + 1;
        parr = cnum.charAt(i) + parr;
        if (bCoDau1000 && d % 3 == 0 && i > 0) parr = character_DauCach1000 + parr;
    }
    snum = parr;

    if (orgfnum.indexOf(".") != -1) {
        snum += character_DauCachThapPhan + psplit[1]; //sửa dấu "." thành dấu ","
    }

    if (flagneg == true) {
        snum = "-" + snum;
    }

    return snum;
}

function dateIsValid(dateStr) {
    const regex = /^\d{2}\/\d{2}\/\d{4}$/;

    if (dateStr.match(regex) === null) {
        return false;
    }

    const [day, month, year] = dateStr.split('/');

    const isoFormattedStr = `${year}-${month}-${day}`;

    const date = new Date(isoFormattedStr);

    const timestamp = date.getTime();

    if (typeof timestamp !== 'number' || Number.isNaN(timestamp)) {
        return false;
    }

    return date.toISOString().startsWith(isoFormattedStr);
}

function CompareDatetime(txtTuNgay, txtDenNgay) {
    if (txtTuNgay == "" || txtDenNgay == "") return true;
    var dTuParts = txtTuNgay.split("/");
    var dDenParts = txtDenNgay.split("/");
    var dTuNgay = new Date(dTuParts[2], dTuParts[1] - 1, dTuParts[0]);
    var dDenNgay = new Date(dDenParts[2], dDenParts[1] - 1, dDenParts[0]);
    if (dTuNgay > dDenNgay) {
        return false;
    }
    return true;
}

function CheckEmail(mail) {
    return /^[a-zA-Z0-9]+(([-._]{1}[a-zA-Z0-9]+)*)@(([a-zA-Z0-9\-]+\.)+[a-zA-Z0-9]{2,})$/.test(mail);
}

function CheckPhoneOrFax(phone) {
    return /^(([+]?([\s]?[0-9]+))|([(][+]?([\s]?[0-9]+)[)]))?([\s]?[(]?[0-9]+[)])?([-]?[\s]?[0-9])+$/.test(phone);
}

function CheckAlphanumeric(value) {
    return /^[a-zA-Z0-9]*$/.test(value);
}

function EscapeHtml(unsafe) {
    unsafe = (typeof unsafe === "undefined" || unsafe == null) ? "" : unsafe;
    return unsafe.toString()
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}

