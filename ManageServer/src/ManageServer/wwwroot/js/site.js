// Write your Javascript code.
function generateUUID() {
    var d = new Date().getTime();
    var uuid = 'xxxxxxxx-xxxx-xxxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = (d + Math.random() * 16) % 16 | 0;
        d = Math.floor(d / 16);
        return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
    return uuid;
}

function dialogForm(action, value, item, confirmFunction) {
    dialog = $("#dialog-form").dialog({
        autoOpen: true,
        dialogClass: "dlg-no-close",
        height: 400,
        width: 600,
        modal: true,
        buttons: {
            "確定": function () {
                confirmFunction(action);
            },
            "取消": function () {
                dialog.dialog("close");
            }
        }
    });
}

function checkOrRemoveCheckAll() {
    if ($('input[name="inpCheckItem"]:checked').length === $('input[name="inpCheckItem"]').length) {
        $('#inpCheckAll').prop("checked", true);

    } else {
        $('#inpCheckAll').prop("checked", false);
    }
}

//ErrorMessage
function errorMessage(jqXHR) {
    if (jqXHR.responseText == "IPIsNull") {
        alert("Update Error:[IP]不能為空。");
    }
    else if (jqXHR.responseText == "NameIsNull") {
        alert("Update Error:[伺服器名稱]不能為空。");
    }
    else if (jqXHR.responseText == "LoginIDIsNull") {
        alert("Update Error:[登入者帳號]不能為空。");
    }
    else if (jqXHR.responseText == "PasswordIsNull") {
        alert("Update Error:[密碼]不能為空。");
    }
    else if (jqXHR.responseText == "OSIsNull") {
        alert("Update Error:[系統]不能為空。");
    }
    else if (jqXHR.responseText == "HostIPIsNull") {
        alert("Update Error:[本機IP]不能為空。");
    }
    else if (jqXHR.responseText == "DBNoConnect") {
        alert("Update error：未連接上資料庫。")
    }
    else if (jqXHR.responseText == "Insert Error.") {
        alert("Insert error：新增資料發生錯誤")
    }
    else if (jqXHR.responseText == "Update Error.") {
        alert("Update error：更新資料發生錯誤。")
    }
    else if (jqXHR.responseText == "Delete Error.") {
        alert(alert("Delete error：刪除資料發生錯誤。"));
    }
    else if (jqXHR.responseText == "GetServerInfo Error.") {
        alert("Inital error：資料庫伺服器發生錯誤，無法取得資料。");
    }
    else {
        alert("發生錯誤。");
        console.error(jqXHR.responseText);
    }
}