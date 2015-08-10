var Reflux = require("reflux");
var $ = require("jquery");
require("../jquerytoken.js");
var Actions = require("../Actions.js");
var RouterStore = require('../RouterStore');

$(document).ajaxError(function (event, XMLHttpRequest, settings, thrownError) {
    console.debug("event = " + event + "  errorThrown = " + thrownError + "  response = " + XMLHttpRequest.responseText);
    if (XMLHttpRequest.status === 401) {
        console.debug("401 status. Redirect to login page.");
        RouterStore.get().transitionTo("login");
    }
});

var User = {Name: ""}

var accountStore = Reflux.createStore({
    
    init: function () {
        this.listenTo(Actions.retrieveUserInfo, this.getUserInfo);
        this.listenTo(Actions.doLogin, this.login);
        this.listenTo(Actions.register, this.register);
    },

    getCurrentUser: function () {
        return User;
    },

    getUserInfo: function(){
        $.ajax({
            type: "GET",
            url: "/Account/UserInfo"
        }).success(function (data) {
            console.debug("getUserInfo done!");
            User = data;
            trigger(User);
        });
    },

    login: function (userName, userPassword, remember){
        var requestData = { Email: userName, Password: userPassword, RememberMe: remember };
        $.ajaxAntiForgery({
            type: "POST",
            url: "/Account/Login",
            data: requestData,
            dataType: "json"
        }).success(function (data) {
            console.debug("login done!");
            User = data;
            trigger(User);
            RouterStore.get().goBack();
        });
    },

    register: function (userName, userPassword, confirmPS) {
        var requestData = { Email: userName, Password: userPassword, ConfirmPassword: confirmPS };
        $.ajaxAntiForgery({
            type: "POST",
            url: "/Account/Register",
            data: requestData,
            dataType: "json"
        }).success(function () {
            console.debug("register done!");
            RouterStore.get().transitionTo("login");
        });
    }


});


if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = accountStore;
}