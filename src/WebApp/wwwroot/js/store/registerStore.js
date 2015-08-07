var Reflux = require("reflux");
var $ = require("jquery");
require("../jquerytoken.js");
var Actions = require("../Actions.js");

var registerStore = Reflux.createStore({
    init: function () {
        this.listenTo(Actions.register, this.register);
    },

    register: function (userName, userPassword, confirmPS) {
        var requestData = { Email: userName, Password: userPassword, ConfirmPassword: confirmPS };
        $.ajaxAntiForgery({
            type: "POST",
            url: "/Account/Register",
            data: requestData,
            dataType: "json"
        }).success(function () {
            console.debug("done");
        }).error(function () {
            console.debug("error");
        });
    }
});


if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = registerStore;
}