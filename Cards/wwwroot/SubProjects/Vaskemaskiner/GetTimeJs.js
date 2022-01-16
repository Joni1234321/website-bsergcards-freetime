var loading = false;

// VASKEMASKINER
var number = 1;
var vaskemaskiner = {};
var vaskemaskineRequest = new XMLHttpRequest();
vaskemaskineRequest.onreadystatechange = function () {
    if (this.readyState == 4 && this.status == 200) {

        // Set initial time
        var text = vaskemaskineRequest.responseText;
        var values = text.split(",");
        maskine = {};

        maskine["time"] = values[0];
        if (values.length >= 2) maskine["status"] = values[1];

        vaskemaskiner[number] = maskine;

        // Stop recursive when over
        if (number >= 9) {
            printToView();
            setLastUpdated();
            loading = false;
            return;
        }

        // Recursive
        updateVaskemaskine(++number);
    }
};


function update() {
    loading = true;
    number = 1;
    updateVaskemaskine();
}
function updateVaskemaskine() {
    vaskemaskineRequest.open("GET", getAjaxLink(number), true);
    vaskemaskineRequest.send();
}
function getAjaxLink(number) {
    return "Vaskemaskine/Info/" + number;
}

function setLastUpdated() {
    timeSinceUpdate = 0;
    document.getElementById("last-updated").innerHTML = new Date().toLocaleTimeString();
}

$(document).ready(update());



// TIMER
var timeSinceUpdate = 0;
var updateTimer = setInterval(function () {
    timeSinceUpdate += 1;
    printToView();
}, 1000);
var refreshTimer = setInterval(function () {
    update();
}, 10000);

// PRINT
function printToView() {
    if (loading) return;

    for (var i = 1; i <= 9; i++) {
        // Time
        text = vaskemaskiner[i]["time"] - timeSinceUpdate;
        if (text < 0) text = 0;
        document.getElementById("timer-" + i).innerHTML = (parseInt(parseInt(text) / 60)) + ':' + ('0' + (parseInt(text) % 60)).slice(-2);

        // Status
        document.getElementById("status-" + i).innerHTML = vaskemaskiner[i]["status"];
    }
}