var EndpointSystem = {
    None: 0,
    CasinoManagementSystem: 28,
    LotusNotes: 2,
    PSFinance: 30
}
var AttestationType = {
    User: "User",
    Role: "Role",
    Privileged: "Privileged"
}

//Popup
function openModal() {
    $('#myModal').modal('show');
}
function openModal1() {
    $('#myModal1').modal('show');
}

function OnClientEntryAddingHandler(sender, eventArgs) {
    if (sender.get_entries().get_count() > 0) {

        eventArgs.set_cancel(true);

    }
}

function handleDateTimePickerClickEvent(sender, args) {
    args = args || window.event;
    args.cancelBubble = true;
    args.returnValue = false;
    if (args.preventDefault) args.preventDefault();
    if (args.stopPropagation) args.stopPropagation();

}

function BindControlEvents() {
    //Change all select to custom select design
    $('select').selectpicker();
    //Change all checkbox to custom checkbox design
    var checkbox = $('input[type="checkbox"]');
    $('input[type="checkbox"]').each(function () {
        if ($(this).next('.lbl').length) {
        }
        else {
            $(this).after('<label class="lbl" /></label>');
        }
    });

    //Change all radio button to custom radio button design
    if ($('input[type="radio"]').next('.lbl').length) {
    } else {
        $('input[type="radio"]').after('<label class="lbl" /></label>');
    }

    //add asterisk to reuired fields
    $('[required]').each(function () {
        $(this).parents('[class^="form-group"]').addClass("required");
    });
    //Remove Attributes
    $('.racRemoveTokenLink').attr('title', 'Remove');
    $(".action-info input[type=image]").attr("src", "");
}

//Initial bind
$(document).ready(function () {
    BindControlEvents();

});

//Re-bind for callbackvar 
prm = Sys.WebForms.PageRequestManager.getInstance();

prm.add_endRequest(function () {
    BindControlEvents();

});



// Change title to tooltip
(function () {
    var ID = "tooltip", CLS_ON = "tooltip_ON", FOLLOW = true,
    DATA = "_tooltip", OFFSET_X = -50, OFFSET_Y = 10,
    showAt = function (e) {
        var ntop = e.pageY + OFFSET_Y, nleft = e.pageX + OFFSET_X;
        $("#" + ID).html($(e.target).data(DATA)).css({
            position: "absolute", top: ntop, left: nleft, 'z-index': 9999
        }).show();
    };
    $(document).on("hover", "*[title]:not([title=''])", function (e) {
        if (!$(this).attr('title')) return;
        $(this).data(DATA, $(this).attr("title"));
        $(this).removeAttr("title").addClass(CLS_ON);
        $("<div id='" + ID + "' />").appendTo("body");
        showAt(e);
    }).on("mouseleave", "." + CLS_ON, function (e) {
        $(this).attr("title", $(this).data(DATA)).removeClass(CLS_ON);
        $("#" + ID).remove().hide();
    });
    //if (FOLLOW) { $(document).on("mousemove", "." + CLS_ON, showAt); }
}());

//tooltip (What's this)
$(document).ready(function () {

    $("body").on("mouseover", ".di_tooltip", function () {
        $(this).next(".tooltip").fadeIn();
    }).on("mouseleave", ".di_tooltip", function () {
        $(this).next(".tooltip").fadeOut();
    });

    //$(".di_tooltip").hover(function () {
    //    $(".tooltip").fadeIn();
    //});
    //$('.di_tooltip').mouseleave(function () {
    //    $(".tooltip").fadeOut();
    //});

    //Next and Previous Button for Tabs
    $('.btnNext').click(function () {
        $('.nav-tabs.nav-pills > .active').next('li').find('a').trigger('click');
    });

    $('.btnPrevious').click(function () {
        $('.nav-tabs.nav-pills > .active').prev('li').find('a').trigger('click');
    });
    $('.racRemoveTokenLink').attr('title', 'Remove');

    var col_max_height = 20; //Max Items per column
    var col_width = 200; //Pixels
    var prevCol = 0;
    $('.MultiColumn').each(function () {
        $(this).find('ul').each(function (index) {
            column = parseInt(index / col_max_height);
            $(this).css('margin-left', column * col_width + 'px')
            if (prevCol != column) {
                $(this).css('margin-top', '-280px').addClass('col_' + column);
                prevCol = column;
            } else {
                $(this).css('margin-top', '0px').addClass('col_' + column);
            }
        });
    })

});
//Validation
var availability = {

    cache: {},

    check: function (element, settings) {

        var id = element.attr('id');
        var cache = this.cache[id] = this.cache[id] || {};

        $.ajax({
            url: settings.url,
            dataType: 'json',
            data: { value: element.val() },

            success: function (data) {

                // the `data` object returns true or false
                // based on the availability of the value


                // set the value on the cache object so 
                // that it can be referenced in the next validation run
                cache.valid = data;

                // trigger validation again
                validator.validateInput(element);

            },

            failure: function () {

                // the ajax call failed so just set the field
                // as valid since we don't know for sure that it's not
                cache.valid = true;

                // trigger validation
                validator.validateInput(element);

            },

            complete: function () {

                // cache the inputs value
                cache.value = element.val();

            }
        });

    }

};

//HTML5 Form Validation 
jQuery(function ($) {
    $('form').validatr();
});


//wizard validation
function OnClientButtonClicking(sender, args) {
    var next = args.get_command();
    if (next == 1 || next == 2 || next == 4) {
        $(".rwzActive :input").each(function (index) {
            if ($.validatr.validateElement($(this)) == false) {

                var parent = $(this).parent();
                var errmessage = "<div class='validatr-message'>" +
                    $(this)[0].validationMessage +
                    "</div>";


                parent.append(errmessage);
                return args.set_cancel(true); //to cancel     
            }
            else {
                var div = $(this).parent();
                $(div).find('div.validatr-message').remove();

            }
        })
        if (next == 2) {  // This section is for RadGrid validation
            var clientId = $("[id$=_FormGrid]").attr("id");

            if (clientId != null && clientId != "") {
                var radGrid = document.getElementById(clientId);
                if (radGrid.innerText == "\t") {
                    var oAlert = radalert("Please select at least 1 form", 330, 180, "Alert", callBackFunction, null);
                    return args.set_cancel(true);
                }
            }
            //var clientId = $("[id$=_grdFunctions").attr("id");
            //if (clientId != null && clientId != "") {
            //    var radGrid = document.getElementById(clientId);
            //    if (radGrid.innerText == "\t" || radGrid.innerText.indexOf("No Functions Available") > -1) {
            //        var oAlert = radalert("Please select minimum 2 functions", 330, 180, "Alert", callBackFunction, null);
            //        return args.set_cancel(true);
            //    }
            //}
        }
        // This section is for RadGrid First Step validation and check the radGrid count
        if (next == 1) {
            var clientId = $("[id$=_FormGrid]").attr("id");
            if (clientId != null && clientId != "") {
                var radGrid = document.getElementById(clientId);
                var radGridText = radGrid.innerText.trim();
                var splitradGrid = radGridText.split("\n");

                if (radGridText.trim() == null || radGridText.trim() == '') {
                    var oAlert = radalert("Please enter at least one EndPoint SyStem", 330, 180, "Alert", callBackFunction, null);
                    return args.set_cancel(true);
                }
            }
        }
    }
    else if (next == 0) {
        $(".rwzActive :input").each(function (index) {
            if ($.validatr.validateElement($(this)) == true) {
                var div = $(this).parent();
                $(div).find('div.validatr-message').remove();
            }
        })
    }
}
function callBackFunction(args) {
}
//remove error message when keypress
$('input').keyup(function () {
    var div = $(this).parent();
    $(div).find('div.validatr-message').remove();
});

//Allow the alpha character and numbers with underscore and semicolon
function AllowLetterNumber(e) {
    var key; var keychar;
    if (window.event) key = window.event.keyCode;
    else if (e) key = e.which;
    else return true;
    keychar = String.fromCharCode(key);
    keychar = keychar.toLowerCase();
    // control keys
    if ((key == null) || (key == 0) || (key == 8) || (key == 9) || (key == 13) || (key == 27) || (key == 32))
        return true;
        // alphas and numbers
    else if ((("abcdefghijklmnopqrstuvwxyz0123456789_;").indexOf(keychar) > -1)) return true;
    else return false;
}

//Allow the alpha character and numbers with underscore, semicolon and dot
function AllowLetterNumberDot(e) {
    var key; var keychar;
    if (window.event) key = window.event.keyCode;
    else if (e) key = e.which;
    else return true;
    keychar = String.fromCharCode(key);
    keychar = keychar.toLowerCase();
    // control keys
    if ((key == null) || (key == 0) || (key == 8) || (key == 9) || (key == 13) || (key == 27) || (key == 32))
        return true;
        // alphas and numbers
    else if ((("abcdefghijklmnopqrstuvwxyz0123456789_;.").indexOf(keychar) > -1)) return true;
    else return false;
}

//Allow the alpha character and numbers with underscore 
function AllowAlphaNumber(e) {
    var key; var keychar;
    if (window.event) key = window.event.keyCode;
    else if (e) key = e.which;
    else return true;
    keychar = String.fromCharCode(key);
    keychar = keychar.toLowerCase();
    // control keys
    if ((key == null) || (key == 0) || (key == 8) || (key == 9) || (key == 13) || (key == 27) || (key == 32))
        return true;
        // alphas and numbers
    else if ((("abcdefghijklmnopqrstuvwxyz0123456789_").indexOf(keychar) > -1)) return true;
    else return false;
}
function AllowNumberupto(obj, max) {
    if (obj != null && obj.value) {
        if (obj.value > max) {
            //obj.value = "";
            return false;
        }
        return true;
    }
    return true;
}

function AllowAlphaNumbericWithDot(e) {
    var key; var keychar;
    if (window.event) key = window.event.keyCode;
    else if (e) key = e.which;
    else return true;
    keychar = String.fromCharCode(key);
    keychar = keychar.toLowerCase();
    // control keys
    if ((key == null) || (key == 0) || (key == 8) || (key == 9) || (key == 13) || (key == 27) || (key == 32))
        return true;
        // alphas and numbers
    else if ((("abcdefghijklmnopqrstuvwxyz0123456789_.").indexOf(keychar) > -1)) return true;
    else return false;
}

function AllowAlphaNumberic(e) {
    var specialKeys = new Array();
    specialKeys.push(8); //Backspace
    specialKeys.push(9); //Tab
    specialKeys.push(46); //Delete
    specialKeys.push(36); //Home
    specialKeys.push(35); //End
    specialKeys.push(37); //Left
    specialKeys.push(39); //Right

    var keyCode = e.keyCode == 0 ? e.charCode : e.keyCode;
    var ret = ((keyCode >= 48 && keyCode <= 57) || (keyCode >= 65 && keyCode <= 90) || (keyCode >= 97 && keyCode <= 122) || (specialKeys.indexOf(e.keyCode) != -1 && e.charCode != e.keyCode));
    if (keyCode == 32 || keyCode == 95) {
        ret = true;
    }

    return ret;
}
function Preventspaces(e) {
    var ret = true;
    if (e.keyCode == 32)
        ret = false;
    return ret;
}


function ValidateIPaddress(obj) {
    var ipaddress = obj.value;
    if (/^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/.test(ipaddress)) {
        return (true)
    }
    //messageDialog("You have entered an invalid IP address Format!");
    return (false)
}


function blockSpecialChar(e) {
    var k = e.keyCode;
    var returnValue = ((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || (k >= 48 && k <= 57));
    if (e.keyCode == 32)
        returnValue = false;

    return returnValue
}


function IPAddressKeyOnly(e) {
    var keyCode = e.keyCode == 0 ? e.charCode : e.keyCode;
    if (keyCode != 46 && keyCode > 31 && (keyCode < 48 || keyCode > 57))
        return false;
    return true;
}
function AllowdotsandNumeric(e) {
    var specialKeys = new Array();

    specialKeys.push(8); //Backspace  

    function numericOnly(elementRef) {

        var keyCodeEntered = (event.which) ? event.which : (window.event.keyCode) ? window.event.keyCode : -1;

        if ((keyCodeEntered >= 48) && (keyCodeEntered <= 57)) {

            return true;
            if (keyCode == 32)
                returnValue = false;

            return returnValue
        }
    }
}

function AllowNumeric(e, min, max) {
    var key; var keychar;
    if (window.event) key = window.event.keyCode;
    else if (e) key = e.which;
    else return true;
    keychar = String.fromCharCode(key);
    keychar = keychar.toLowerCase();
    // control keys
    if ((key == null) || (key == 0) || (key == 8) || (key == 9) || (key == 13) || (key == 27))
        return true;

    //Check max Character If maximum character is -1 ,so no need to valudate the maximum value
    if ((max != -1) && (min != -1)) {
        if (((keychar) <= max) && (keychar >= min))
            return true;
        else return false;
    }
        //Numbers
    else if ((("0123456789").indexOf(keychar) > -1)) return true;
    else return false;
}
function validate(evt) {
    var theEvent = evt || window.event;
    var key = theEvent.keyCode || theEvent.which;
    key = String.fromCharCode(key);
    var regex = /[0-9]|\./;
    if (!regex.test(key)) {
        theEvent.returnValue = false;
        if (theEvent.preventDefault) theEvent.preventDefault();
    }
}
//ellipsis tooltip
$.expr[':'].truncated = function (obj) {
    var element = $(obj);

    return (element[0].scrollHeight > (element.innerHeight() + 1)) || (element[0].scrollWidth > (element.innerWidth() + 1));

};


$(document).ready(function () {
    $("td").mouseenter(function () {
        var cella = $(this);
        var isTruncated = cella.filter(":truncated").length > 0;
        if (isTruncated) cella.attr("title", cella.text());
        else cella.attr("title", null);
    });
    //$('[data-toggle="tab"]').mouseenter(function () {
    //    $(this).tab('show');
    //});


    $('.di-md-nav a').hover(function (e) {
        e.preventDefault()
        $(this).tab('show')
    });

});



function allowNumerics(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

//RadChart Dashboard
(function (global) {
    var chart;

    function ChartLoad(sender, args) {
        chart = sender.get_kendoWidget(); //store a reference to the Kendo Chart widget, we will use its methods
    }

    global.chartLoad = ChartLoad;

    function resizeChart() {
        if (chart)
            chart.resize(); //redraw the chart so it takes the new size of its container when it changes (e.g., browser window size change, parent container size change)
    }


    //this logic ensures that the chart resizing will happen only once, at most - every 200ms
    //to prevent calling the handler too often if old browsers fire the window.onresize event multiple times
    var TO = false;
    window.onresize = function () {
        if (TO !== false)
            clearTimeout(TO);
        TO = setTimeout(resizeChart, 200);
    }

})(window);


function preventstartingspace(sender) {
    if ((sender.value.length == 0 || sender.selectionStart == 0) && (window.event.keyCode == 32)) {
        return false;
    }
    else if (sender.value.charCodeAt(0) == 32) {
        sender.value = sender.value.substring(1);
    }
    return true;
}

function allowAlphabetswithdots(e, t) {
    try {
        if (window.event) {
            var charCode = window.event.keyCode;
        }
        else if (e) {
            var charCode = e.which;
        }
        else { return true; }
        if ((charCode > 64 && charCode < 91) || (charCode > 96 && charCode < 123) || (charCode == 46))
            return true;
        else
            return false;
    }
    catch (err) {
        alert(err.Description);
    }
}

//Redirect to a Home on Cancel Button Click
function Redirect() {
    window.location.href = '/Home/Home.aspx';
    return false;
}
//$(".rgRow rgSelectedRow").click(function () {
//    $(".action-info input[type=image]").prop("src", false);
//});

//$(".rgSelectedRow .action-info input").removeAttr("src");

//Change tile icon to white when hovered
$(".tile-link").hover(function () {
    $(this).find(".tile-icon-img").attr("src", function (index, attr) {
        return attr.replace(".png", "-w.png");
    });
}, function () {
    $(this).find(".tile-icon-img").attr("src", function (index, attr) {
        return attr.replace("-w.png", ".png");
    });
});

function showAlert(message, type) {
    $(".txtAlertMsg").text(message);
    if (type == "Success") {
        $(".txtAlertTitle").text("Success");
        $(".alertInnerMessage").removeClass("alert-danger alert-warning fade").addClass("alert-success");
    } else if (type == "Error") {
        $(".txtAlertTitle").text("Error");
        $(".alertInnerMessage").removeClass("alert-warning alert-success fade").addClass("alert-danger");
    } else if (type == "Warning") {
        $(".txtAlertTitle").text("Warning");
        $(".alertInnerMessage").removeClass("alert-danger alert-success fade").addClass("alert-warning ");
    }
    $(".alertMessageDiv").show();
    $(".alert").show();
    setTimeout(function () { $(".alert").hide(); }, 8000);
}

function applyHeaderStyle() {
    var tableLayout = $(".rgMasterTable").css("table-layout");
    if (tableLayout == "auto") {
        $(".RadGrid_IACMSkin .rgHeader, .RadGrid_IACMSkin .rgHeader a").css("text-overflow", "clip");
        $(".RadGrid_IACMSkin .rgHeader, .RadGrid_IACMSkin .rgHeader a").css("white-space", "nowrap");
    }
}