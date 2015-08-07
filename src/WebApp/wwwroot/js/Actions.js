var Reflux = require("reflux");

var Actions = Reflux.createActions([
    "doLogin" ,
    "register"
]);

if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = Actions;
}