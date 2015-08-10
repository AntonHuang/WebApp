var Reflux = require("reflux");
var $ = require("jquery");
require("../jquerytoken.js");
var Actions = require("../Actions.js");

var registerStore = Reflux.createStore({
    init: function () {
        this.listenTo(Actions.register, this.register);
    },

   
});


if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = registerStore;
}