var React = React || require('react');
var Actions = Actions || require("./Actions.js");
var Reflux = Reflux || require("reflux");


var memberPointItem = React.createClass({

    render: function () {
        if (this.prop.MemberPoint) {
            return (<div className="row">
            <div className="col-md-12">
               <label className="col-md-2 control-label" htmlFor="Name">姓名：</label>
                <div className="col-md-2" >
                     <input className="form-control uneditable-input" type="text" 
                            id="Name" ref="Name" defaultValue = {this.props.MemberPoint.Name} />
                </div>

                <label className="col-md-2 control-label" htmlFor="MemberID">会员ID号：</label>
                <div className="col-md-2">
                     <input className="form-control uneditable-input" type="text"
                            id="MemberID" ref="MemberID" defaultValue={this.props.MemberPoint.MemberID} />
                </div>
                <label className="col-md-2 control-label" htmlFor="Point">积分：</label>
                <div className="col-md-2">
                     <input className="form-control uneditable-input" type="text" 
                            id="Point" ref="Point" defaultValue={this.props.MemberPoint.Point} />
                </div></div></div>
            );
        }else{
            return (<span></span>);
        }

    }

});


var saleMemberPoint = React.createClass({
    componentWillUpdate: function () {
        if (this.props && this.props.MemberPointItems) {
            this.state.SelfMP = {
                Name: this.props.MemberPointItems.MemberName || "",
                MemberID: this.props.MemberPointItems.MemberID || "",
                Point: this.props.MemberPointItems.PointCount ,
            }
            this.state.Up1MP = {
                Name: this.props.MemberPointItems.Up1Name || "",
                MemberID: this.props.MemberPointItems.Up1ID || "",
                Point: this.props.MemberPointItems.Up1PointCount ,
            }
            this.state.Up2ID = {
                Name: this.props.MemberPointItems.Up2Name || "",
                MemberID: this.props.MemberPointItems.Up2ID || "",
                Point: this.props.MemberPointItems.Up2PointCount ,
            }
        }
    },

    render: function () {
        if (!this.props || !this.props.MemberPointItems) {
            return (<span></span>);
        }

        return (
    <div className="row">
        <div className="col-md-12">
            <section>
                <hr />
                <h5>积分信息</h5>
                 <div className="form-group">
                    <label className="col-md-2 control-label" htmlFor="MattressID">购买人积分信息：</label>
                    <div className="col-md-10">
                        <memberPointItem MemberPoint = {this.state.SelfMP }></memberPointItem>
                    </div>
                 </div>
                <div className="form-group">
                    <label className="col-md-2 control-label" htmlFor="MattressID">上一级积分信息：</label>
                    <div className="col-md-10">
                        <memberPointItem MemberPoint={this.state.Up1MP}></memberPointItem>
                    </div>
                </div>
                <div className="form-group">
                    <label className="col-md-2 control-label" htmlFor="MattressID">上两级积分信息：</label>
                    <div className="col-md-10">
                        <memberPointItem MemberPoint={this.state.Up2MP}></memberPointItem>
                    </div>
                </div>
            </section>
        </div>
    </div>);
    
    }
});



var sellMattress = React.createClass({

    mixins: [Reflux.ListenerMixin],

    getInitialState: function () {
        return {
            Today: this.today(),
            MattressTypes: [],
            MemberPointItems: null,
            sellMattressNo : ""
        };
    },

    onSellMattressDone: function (data) {
        console.debug("onSellMattressDone", data);
        alert("添加成功！");
        
        this.setState({
            sellMattressNo: data.saleToCustomerID,
            MemberPointItems: data.memberPointItems
        })
    },

    onSellMattressFail: function (data) {
        console.debug("onSellMattressFail", data);

        var msg = "";
        if ("MattressID is Exist." === data) {
            msg = "床垫编号已经添加过了！";
        } else if ("MattressTypeID is not Exist." === data) {
            msg = "床垫型号不存在！";
        } else if ("CustomerID is not Exist." === data) {
            msg = "购买人ID号不存在！";
        }

        alert("添加失败！" + msg);
    },

    onListMattressTypeDone: function (data) {
        console.debug("onListMattressTypeDone", data);
        if (!data || data.length == 0) {
            alert("没找到床垫型号！");
        }

        this.setState({MattressTypes:data });

    },

    onListMattressTypeFail: function (data) {
        console.debug("onListMattressTypeFail", data);
        alert("没找到床垫型号！");
    },

    componentWillMount: function () {
        this.listenTo(Actions.sellMattressDone, this.onSellMattressDone);
        this.listenTo(Actions.sellMattressFail, this.onSellMattressFail);
        this.listenTo(Actions.listMattressTypeDone, this.onListMattressTypeDone);
        this.listenTo(Actions.listMattressTypeFail, this.onListMattressTypeFail);
        Actions.listMattressType();
    },

    today: function () {
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0!
        var yyyy = today.getFullYear();

        if (dd < 10) {
            dd = '0' + dd
        }

        if (mm < 10) {
            mm = '0' + mm
        }

        return yyyy + '-' + mm + '-' + dd;
    },

    componentWillUpdate: function () {
        this.state.Today = this.today();
    },

    handleSubmit: function (e) {
        e.preventDefault();
        var MattressID = React.findDOMNode(this.refs.MattressID).value.trim();
        var MattressTypeID = React.findDOMNode(this.refs.MattressTypeID).value.trim();
        var DeliveryAddress = React.findDOMNode(this.refs.DeliveryAddress).value.trim();
        var CustomerID = React.findDOMNode(this.refs.CustomerID).value.trim();
        var SaleDate = React.findDOMNode(this.refs.SaleDate).value.trim();
        var Gifts = React.findDOMNode(this.refs.Gifts).value.trim();

        if (!MattressID) {
            alert("床垫编号不能留空！");
            return;
        }

        if (!MattressTypeID) {
            alert("床垫型号不能留空！");
            return;
        }

        if (!CustomerID) {
            alert("购买人ID号不能留空！");
            return;
        }

        Actions.sellMattress(MattressID, MattressTypeID, DeliveryAddress, CustomerID, SaleDate, Gifts);
    },

    render: function () {
        return (
          <div className="row">
            <div className="col-md-12">
               <section>
                  <form className="form-horizontal" method="post" role="form" onSubmit={this.handleSubmit}>
                        <h4>添加床垫信息</h4>
                        <hr />
                        <div className="text-danger"></div>
                        <div className="form-group">
                            <label className="col-md-2 control-label" htmlFor="MattressID">床垫编号：</label>
                            <div className="col-md-4">
                                <input className="form-control uneditable-input" type="text" autoFocus
                                       id="MattressID" ref="MattressID"/>
                            </div>

                            <label className="col-md-2 control-label" htmlFor="MattressTypeID">床垫型号：</label>
                            <div className="col-md-4">
                                <select className="form-control" id="MattressTypeID" ref="MattressTypeID">
                                    {this.state.MattressTypes.map(function(result) {
                                        return <option value={result.ID} >{result.Name}</option>;
                                    })}
                                </select>
                            </div>

                        </div>
                        <div className="form-group">
                                 <label className="col-md-2 control-label" htmlFor="DeliveryAddress">送货地址：</label>
                                 <div className="col-md-4">
                                     <input className="form-control" id="DeliveryAddress" ref="DeliveryAddress" type="text" />

                                 </div>

                                <label className="col-md-2 control-label" htmlFor="CustomerID">购买人ID号：</label>
                                 <div className="col-md-4">
                                     <input className="form-control" id="CustomerID" ref="CustomerID" type="text" />

                                 </div>
                            </div>

                            <div className="form-group">
                                  <label className="col-md-2 control-label" htmlFor="SaleDate">购买时间：</label>
                                  <div className="col-md-4">
                                      <input className="form-control" id="SaleDate" ref="SaleDate" type="date" 
                                             defaultValue={this.state.Today} />

                                  </div>
                            </div>
                            <div className="form-group">
                                <label className="col-md-2 control-label" htmlFor="Gifts">赠送礼品：</label>
                                  <div className="col-md-4">
                                      <textarea className="form-control" id="Gifts" ref="Gifts" rows="3" />
                                  </div>
                            </div>


                              <div className="form-group">
                                  <div className="col-md-offset-2 col-md-10">
                                      <button type="submit" className="btn btn-default">保存</button>
                                  </div>
                              </div>
                  </form>
               </section>

                <saleMemberPoint MemberPointItems={this.state.MemberPointItems}/>
            </div>
          </div>
        );
    }
});





if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = sellMattress;
}