var Reflux = require("reflux");
var $ = require("jquery");
require("../jquerytoken.js");
var Actions = require("../Actions.js");

var LoginStore = require("./loginStore.jsx");
var user = { Name : "" };

var accountStore = Reflux.createStore({
    

    init: function () {
        this.listenTo(Actions.doLogin, this.login);
    },

    login: function (userName, userPassword, remember) {
        warteFor(["LoginStore"], function () {

        });
    },

    User: user

});


if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = accountStore;
}