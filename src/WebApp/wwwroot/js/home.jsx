var React = React || require('react');
var Reflux = Reflux || require("reflux");
var Actions = Actions || require("./Actions.js");

var AccountStore = require("./store/Account.js");


var MemberInfo = React.createClass({

    render: function () {

        if (!this.props.user || !this.props.user.ID) {
            return (<span>请先登录</span>);
        }

        return (
            <div className="row">
                <h4>{this.props.user.Name}</h4>
                <span>{this.props.user.Level}</span>
                <span>{this.props.user.RegisterDate}</span> 
                入会
            </div>
        );
    }
});

var Home = React.createClass({

    getInitialState: function () {
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

    logout: function () {
        Actions.doLogout();
    },

    render: function () {
        return (
           <MemberInfo user={this.state.currentUser} />
        );
    }
});


if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = Home;
}