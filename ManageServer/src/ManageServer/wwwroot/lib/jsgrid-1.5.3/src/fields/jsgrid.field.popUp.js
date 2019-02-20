(function (jsGrid, $, undefined) {

    var Field = jsGrid.Field;

    function PopUpField(config) {
        Field.call(this, config);
    }

    PopUpField.prototype = new Field({

        itemTemplate: function (value, item) {
            return btnDisabledPermission(value, item);
        },
        editTemplate: function (value, item) {
            return btnPermission(value, item);
        },
    });

    jsGrid.fields.popUp = jsGrid.PopUpField = PopUpField;

}(jsGrid, jQuery));

function btnDisabledPermission(value, item) {
    var $btnPermission = $("<button class='btn btn-default btnPermission'>").prop('disabled', true);
    return $btnPermission.text("授權設定");
}

function btnPermission(value, item) {
    var $btnPermission;
    $btnPermission = $("<button class='btn btn-default btnPermission'>");
    return $btnPermission.text("授權設定");
}
