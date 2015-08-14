var React = React || require('react');
var Actions = Actions || require("./Actions.js");
var Reflux = Reflux || require("reflux");

var ListMember = require("./listMember.jsx");
var ModifyMember = require("./modifyMember.jsx");


var updateMember = React.createClass({

    onMemberTableRowClick: function (selectedMember) {
        this.props.modifyMemberComp.setState(selectedMember);
    },

    componentDidMount: function () {
        this.props.modifyMemberComp = this.refs.modifyMemberComp;
    },

    render: function () {
        return (
            <div>
                <ListMember onMemberTableRowClick={this.onMemberTableRowClick}/>
                <hr />
                <ModifyMember ref="modifyMemberComp" />
            </div>
        );
    }
    
});


if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = updateMember;
}
