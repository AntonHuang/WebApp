var React = React || require('react');
var Actions = Actions || require("./Actions.js");
var Reflux = Reflux || require("reflux");
var ToggleDisplay = require("react-toggle-display");


var SellMattress = require("./sellMattress.jsx");
var SellMattressResult = require("./sellMattressResult.jsx");

var sellMattressTask = React.createClass({

    mixins: [Reflux.ListenerMixin],

    getInitialState: function () {
        this.sellMattressData = {
            MattressID: "",
            MattressTypeName: "",
            DeliveryAddress: "",
            CustomerID: "",
            SaleDate: "",
            Gifts: ""
        };
        this.MemberPointItems = {
            MemberName: "",
            MemberID: "",
            PointCount: "",
            Up1Name: "",
            Up1ID: "",
            Up1PointCount: "",
            Up2Name: "",
            Up2ID: "",
            Up2PointCount: ""
        };

        return {
            sellMattressNo: "",
            showResult: false
        };
    },

    onSellMattressDone: function (data) {
        console.debug("onSellMattressDone", data);
        alert("添加成功！");

        this.sellMattressData = data.sellMattressData,
        this.MemberPointItems = data.memberPointItems,

        this.setState({
            sellMattressNo: data.saleToCustomerID,
            showResult: data.saleToCustomerID !== ""
        });
    },

    componentWillUpdate: function (newProps, newState) {
        console.debug("componentWillUpdate", this.state);
    },

    componentDidUpdate: function (newProps, newState) {
        console.debug("componentDidUpdate", this.state);
    },
     
    componentWillMount: function () {
        this.listenTo(Actions.sellMattressDone, this.onSellMattressDone);
    },

    render: function () {
        return (
            <div className="row">
                <div className="col-md-12">
                    <ToggleDisplay show={ !this.state.showResult } >
                        <SellMattress />
                    </ToggleDisplay>
                    <ToggleDisplay show={ this.state.showResult }>
                        <SellMattressResult sellMattressData={this.sellMattressData}
                                    MemberPointItems={this.MemberPointItems} />
                    </ToggleDisplay>
                </div>
            </div>
        );
    }

});



if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = sellMattressTask;
}