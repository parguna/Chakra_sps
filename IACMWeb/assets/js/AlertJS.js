

function Alert(error, message) {
    var control = document.getElementById("AlertMessage1_message");
    var lblTitle = document.getElementById("AlertMessage1_lblTitle");
    var lblMessage = document.getElementById("AlertMessage1_lblMessage");
    var innermessage = document.getElementById("AlertMessage1_innermessage");
    if (control != null && lblTitle != null && lblMessage != null && innermessage != null) {
        lblMessage.value = message;
        lblTitle.value = error;
        control.style.display = lblMessage.value != "" ? "block" : "none";
        innermessage.className = error == "Success" ? "alert alert-success col-md-3" : error == "Error" ? "alert alert-danger col-md-3" : "alert alert-warning col-md-3";

    }

}