// Write your Javascript code.
function dialogForm(value, item, confirmFunction) {
    dialog = $("#dialog-form").dialog({
        autoOpen: true,
        height: 400,
        width: 600,
        modal: true,
        buttons: {
            "確定": function(){
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