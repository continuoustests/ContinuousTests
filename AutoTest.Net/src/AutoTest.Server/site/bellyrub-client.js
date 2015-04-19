var createBellyRubClient = function() {
    var belly = new Object();
    belly.host = '';
    belly.handlers = new Object();
    belly.handlers['belly:get-window-title'] = function (msg, responseDelegate) {
        console.log('getting window title: ' + document.title);
        responseDelegate({ title: document.title });
    };
    belly.onconnected = function () {
    };
    belly.ondisconnected = function () {
    };
    belly.connect = function () {
        var getParameters = function getUrlVars() {
            var vars = {};
            var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function(m,key,value) {
                vars[key] = value;
            });
            return vars;
        }
        var host = getParameters()['channel']+'/chat';
        belly.host = host;
        var client = new WebSocket(host);
        client.onopen = function () {
            belly.onconnected();
        };
        client.onmessage = function (event) {
            var payload = JSON.parse(event.data);
            if (payload.subject in belly.handlers){
                var responseDelegate = function (body) {
                    belly.send(payload.token, body);
                };
                belly.handlers[payload.subject](payload.body, responseDelegate);
            }
        };
        client.onclose = function () {
            belly.ondisconnected();
        };
        belly.client = client;
    };
    belly.disconnect = function () {
        belly.client.close();
    };
    belly.send = function (subject, body) {
        var str = JSON.stringify({subject: subject, token: '', body: body});
        belly.client.send(str);
    };
    belly.request = function (subject, body, responseHandler) {
        var newUuid = function () {
            var s4 = function () {
                return Math.floor((1 + Math.random()) * 0x10000)
                    .toString(16)
                    .substring(1);
            };
            return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
                s4() + '-' + s4() + s4() + s4();
        };
        var response = null;
        var token = newUuid();
        belly.handlers[token] = function (msgBody) {
            responseHandler(msgBody);
        };
        var str = JSON.stringify({subject: subject, token: token, body: body});
        belly.client.send(str);
    }
    return belly;
};
