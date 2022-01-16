var regex_number = "(?<=B673L1U)\\d+";
var regex_var = 'B673L1U\\d+=[\\s\\S]*?;';  // GET VARIABLE NAME AND VALUE
var regex_trim = "^[ \\t]+";                // GET ALL WHITE _ SPACE AND TABS
var regex_value = "(?<==)\\d+";             // GET THE VALUE OF THE VARIABLE

var vaskemaskiner = {};

// VASKEMASKINER
var vaskemaskineRequest = new XMLHttpRequest();
vaskemaskineRequest.onreadystatechange = function () {
    if (this.readyState == 4 && this.status == 200) {
        var text = vaskemaskineRequest.responseText;
        var number = parseInt(text.match(regex_number)[0]);

        text = text.match(regex_var)[0];
        text = text.replace(regex_trim);
        text = text.match(regex_value)[0];

        document.getElementById("timer-" + number).innerHTML = (parseInt(parseInt(text) / 60)) + ':' + ('0' + (parseInt(text) % 60)).slice(-2);
        if (number >= 9) return;

        updateVaskemaskine(++number);
    }
};
// JSON
var jsonRequest = new XMLHttpRequest();
jsonRequest.onreadystatechange = function () {
    if (this.readyState == 4 && this.status == 200) {
        var myArr = JSON.parse(this.responseText);
        for (var i = 0; i < myArr.length; i++) {
            vaskemaskiner[myArr[i].number] = myArr[i].ajax_id;
        }
    }
};

$(document).ready(generateVaskemaskiner()); 


function updateVaskemaskiner() {
    updateVaskemaskine(1);

    for (var i = 1; i <= 9; i++) {
    }
}
function updateVaskemaskine(number) {
    console.log("fisk");
    vaskemaskineRequest.open("GET", getAjaxLink(number), true);
    vaskemaskineRequest.send();
}

function getAjaxLink(number) {
    //return "https://backend.nortec1.dk/download/unit5/Ajax.ashx?" + vaskemaskiner[number];
    return "";
} 

function generateVaskemaskiner() {
    jsonRequest.open("GET", "SubProjects/Vaskemaskiner/Vaskemaskiner.json", true);
    jsonRequest.send(); 
}
