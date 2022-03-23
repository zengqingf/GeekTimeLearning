var host = "192.168.2.92:5000"

function getApiUrl(route){
    return "http://" + host + "/"+route+"/api/"
}

function getSendJsonData(rawdata, method) {
    data = {
        'data': rawdata
    }
    if (method !== undefined) {
        data['method'] = method;
    }
    return data
}

function postData(route, jsondata, method, success) {
    jsondata = getSendJsonData(jsondata,method)
    let url = getApiUrl(route)
    let successwrap = function (rawdata){
        js = $.parseJSON(rawdata);
        code = js['code']
        if (code !== 200){
            console.log("code="+code+" msg="+js['msg']);
            return
        }
        data = js['data']
        success(data)
    }
    $.post({
        url: url,
        contentType: "application/json",
        data: JSON.stringify(jsondata),
        success: successwrap
    });
    
}
function getData(route, jsondata, method, success) {
    jsondata = getSendJsonData(jsondata,method)
    let url = getApiUrl(route)
    let successwrap = function (rawdata){
        js = $.parseJSON(rawdata);
        code = js['code']
        if (code !== 200){
            console.log("code="+code+" msg="+js['msg']);
            return
        }
        data = js['data']
        success(data)
    }
    $.get({
        url: url,
        contentType: "application/json",
        data: JSON.stringify(jsondata),
        success: successwrap
    });
    
}
function getQueryVariable(variable)
{
    var query = window.location.search.substring(1);
    var vars = query.split("&");
    for (var i=0;i<vars.length;i++) {
            var pair = vars[i].split("=");
            if(pair[0] == variable){return pair[1];}
    }
    return('');
}
