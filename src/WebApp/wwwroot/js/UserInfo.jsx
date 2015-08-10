var React = React || require('react');
var Reflux = Reflux || require("reflux");
var Actions = Actions || require("./Actions.js");

var AccountStore = require("./store/Account.js");


var userInfo = React.createClass({
    getInitialState: function() {
        return { currentUser: AccountStore.getCurrentUser() };
     },

    onStatusChange: function (user) {
        this.setState({
            currentUser: user
        });
    },

    componentDidMount: function () {
        this.unsubscribe = AccountStore.listen(this.onStatusChange);
    },

    componentWillUnmount: function () {
        this.unsubscribe();
    },

    render: function () {
        return <div> Holle {this.state.currentUser.Name} </div>;
    }
});




if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = userInfo;
}