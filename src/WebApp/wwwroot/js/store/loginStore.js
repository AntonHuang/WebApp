var Reflux = require("reflux");
var $ = require("jquery");
require("../jquerytoken.js");
var Actions = require("../Actions.js");

var RouterStore = require('../RouterStore');


var loginStore = Reflux.createStore({
    init: function () {
        this.listenTo(Actions.doLogin, this.login);
    },

    login: function (userName, userPassword, remember){
        var requestData = { Email: userName, Password: userPassword, RememberMe: remember };
        $.ajaxAntiForgery({
            type: "POST",
            url: "/Account/Login",
            data: requestData,
            dataType: "json"
        }).success(function() {
            console.debug("done!");
            RouterStore.get().transitionTo("/");
        }).complete(function() {
           console.debug("done complete!");
        }).error(function(XMLHttpRequest, textStatus, errorThrown){
            console.debug("textStatus = " + textStatus + "errorThrown = " + errorThrown +  "response = " + XMLHttpRequest.responseText);
        });
    }
});


if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = loginStore;
}