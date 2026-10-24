//$(document).ready(function () {
//    $.getScript("../bootstrap/js/bootstrap.min.js");
//    $.getScript("../datepicker/js/bootstrap-datepicker.js");
//    $.getScript("../select/js/bootstrap-select.js");
//    $.getScript("../extendbootstrap/js/bootbox.js");
//    $.getScript("../extendbootstrap/js/bootstro.min.js");
//    $.getScript("../kendoui/js/kendo.ui.core.min.js");
//    $.getScript("../kendoui/js/kendo.all.min.js");
//    $.getScript("../select/js/bootstrap-select.js");
//    $.getScript("../js/validatr.js");
//    $.getScript("../js/Custom.js");
//});



function confirmAction(button) {
    function aspButtonCallbackFn(arg) {
        if (arg) {
            __doPostBack(button.name, "");
        }
    }
    radconfirm("Are you sure you want to delete?", aspButtonCallbackFn, 330, 180, null, "Confirm");
}
function messageDialog(message) {
    radalert(message, 330, 120, "Alert", callBackFn, null);
}
function callBackFn(arg) {
    //alert("this is the client-side callback function. The RadAlert returned: " + arg);
}

function validateAutoCompleteBox(button, controlId, message) {
    var clientId = $("[id$=_" + controlId + "]").attr("id");
    if (clientId != "") {
        var autoCompleteBox = document.getElementById(clientId);
        if (autoCompleteBox.value == "") {
            var oAlert = radalert(message, 330, 180, "Alert", callBackFn, null);
            return false;
        }
        else {
            return true;
        }
    }
}
function Alert(message) {
    var oAlert = radalert(message, 330, 180, "Alert", callBackFn, null);
}


function closeWin() {
    GetRadWindow().close();
}
function GetRadWindow() {
    var oWindow = null; if (window.radWindow)
        oWindow = window.radWindow; else if (window.frameElement.radWindow)
            oWindow = window.frameElement.radWindow; return oWindow;
}

//function ColumnHidden(sender, eventArgs) {
//    document.getElementById('<%= hdnButton.ClientID %>').click();
//}

//function ColumnShown(sender, eventArgs) {
//    document.getElementById('<%= hdnButton.ClientID %>').click();
//}

function validateAutoCompleteBoxGroup(button, controlId) {
    var message = [];
    var retMes = '';
    var boxIds = controlId.split(',');
    for (i = 0; i < boxIds.length; i++) {
        message[i] = validationAutoBoxGroup(button, boxIds[i]);
    }
    if (message != "") {
        for (r = 0; r < message.length; r++) {
            retMes = retMes + message[r];
        }
        if (retMes != "") {
            var oAlert = radalert(retMes, 330, 180, "Alert", callBackFn, null);
            return false;
        }
        else {
            return true;
        }
    }
    else {
        return true;
    }
}

function validationAutoBoxGroup(button, controlId) {
    var clientId;
    var clientNums;
    if (controlId === "UserNameAutoCompleteBox") {
        clientId = $("[id$=_" + controlId + "]")[1].id;
        return message = returnMessage(controlId, clientId,"");
    }
    else {
        clientNums = $("[id$=_" + controlId + "]").length;
        if (clientNums==2) {
            var Funmessage='';
            for (p = 1; p <= clientNums; p++) {
                clientId = $("[id$=_" + controlId + "]")[p-1].id;
                Funmessage =Funmessage+ returnMessage(controlId,clientId, p);
            }
            return Funmessage;
        }
        else {
            clientId = $("[id$=_" + controlId + "]").attr("id");
            return message = returnMessage(controlId,clientId, "");
        }

    }
    

}


function returnMessage(controlId,clientId,pos) {
    if (clientId != "") {
        var message = "";
        var autoCompleteBox = document.getElementById(clientId);
        if (autoCompleteBox.value == "") {
            switch (controlId) {
                case 'UserNameAutoCompleteBox':
                    message =  'User should be selected' + '</br>';
                    break;
                case 'FunctionAuto':
                    message = 'Function '+pos+'Value should be selected' + '</br>';
                    break;
                case 'FunctionAutoValue':
                    message ='Function Item '+pos+' should be selected' + '</br>';
                    break;
            }
            return message;
        }
        else {
            return message;
        }
    }
}
