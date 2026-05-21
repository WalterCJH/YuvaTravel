function check_mobile() {
    var userAgentInfo = navigator.userAgent;
    var Agents = new Array("Android", "iPhone", "SymbianOS", "Windows Phone", "iPad", "iPod");
    var flag = false;
    for (var v = 0; v < Agents.length; v++) {
        if (userAgentInfo.indexOf(Agents[v]) > 0) {
            flag = true;
            break;
        }
    }
    return flag;
};

//var host = "www.long-huei.com";
//if (window.location.hostname != host && window.location.hostname != "localhost") {
//    location.href = "https://" + host;
//}