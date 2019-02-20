(function (jsGrid, $, undefined) {

    var Field = jsGrid.Field;

    function PopButtonField(config) {
        Field.call(this, config);
    }

    PopButtonField.prototype = new Field({
      
    });

    jsGrid.fields.popButton = jsGrid.PopButtonField = PopButtonField;

}(jsGrid, jQuery));

function btnDisabledPermission(value, item) {
    var $btnPermission = $("<button class='btn btn-default btnPermission'>").prop('disabled', true);
    return $btnPermission.text("授權設定");
}

function btnPermission(value, item) {
    var $btnPermission;
    if (item.isAdmin == true) {
        $btnPermission = $("<button class='btn btn-default btnPermission'>").prop('disabled', true);
    } else {
        $btnPermission = $("<button class='btn btn-default btnPermission'>");
    }

    return $btnPermission.text("授權設定")
                        .on("click", function () {
                            dialogForm();
                            getUserID(value, item);
                            setUserPermission(value, item);
                            checkOrRemoveCheckAll();
                        });
}
