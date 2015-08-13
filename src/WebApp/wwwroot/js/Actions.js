var Reflux = require("reflux");

var Actions = Reflux.createActions([
    "retrieveUserInfo",
    "doLogin",
    "doLogout",
    "loadedUserInfo",
    "changePassword",
    "changePasswordDone",
    "changePasswordFail",
    "nextAccountID",
    "nextAccountIDDone",
    "nextAccountIDFail",
    "modifyMember",
    "modifyMemberDone",
    "modifyMemberFail",
    "register",

    "findMember",
    "findMemberDone",
    "findMemberFail"
]);

if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = Actions;
}