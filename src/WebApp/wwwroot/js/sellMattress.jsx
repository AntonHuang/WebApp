var React = React || require('react');
var Actions = Actions || require("./Actions.js");
var Reflux = Reflux || require("reflux");

var sellMattress = React.createClass({

    onNextAccountIDDone: function (data) {
        this.AccountID = this.AccountID || React.findDOMNode(this.refs.AccountID);
        this.AccountID.value = data.NextAccountID;
    },

    onNextAccountIDFail: function () {
        alert("无法生成新的会员ID。");
    },

    componentWillMount: function () {
        Actions.nextAccountIDDone.listen(this.onNextAccountIDDone);
        Actions.nextAccountIDFail.listen(this.onNextAccountIDFail);

        Actions.nextAccountID();
    },

    handleSubmit: function (e) {
        e.preventDefault();
        var accountID = React.findDOMNode(this.refs.AccountID).value.trim();
        var referenceID = React.findDOMNode(this.refs.ReferenceID).value.trim();
        var name = React.findDOMNode(this.refs.Name).value.trim();
        var cardID = React.findDOMNode(this.refs.CardID).value.trim();
        var address = React.findDOMNode(this.refs.Address).value.trim();
        var phone = React.findDOMNode(this.refs.Phone).value.trim();
        var level = React.findDOMNode(this.refs.Level).value.trim();
        if (!accountID) {
            alert("会员ID不能留空！");
            return;
        }

        Actions.register(accountID, referenceID, name, cardID, address, phone, level);
        return;
    },

    render: function () {
        return (
          <div className="row">
            <div className="col-md-12">
               <section>
                  <form className="form-horizontal" method="post" role="form" onSubmit={this.handleSubmit}>
                        <h4>注册会员信息</h4>
                        <hr />
                        <div className="text-danger"></div>
                        <div className="form-group">
                            <label className="col-md-3 control-label" htmlFor="MattressID">床垫编号：</label>
                            <div className="col-md-3">
                                <input className="form-control uneditable-input" type="text" autoFocus
                                       id="MattressID" ref="MattressID"/>
                            </div>

                            <label className="col-md-3 control-label" htmlFor="MattressTypeID">床垫型号：</label>
                            <div className="col-md-3">
                                <input className="form-control" id="MattressTypeID" ref="MattressTypeID" type="text" />
                            </div>

                        </div>
                        <div className="form-group">
                                 <label className="col-md-3 control-label" htmlFor="DeliveryAddress">送货地址：</label>
                                 <div className="col-md-3">
                                     <input className="form-control" id="DeliveryAddress" ref="DeliveryAddress" type="text" />

                                 </div>

                                <label className="col-md-3 control-label" htmlFor="CustomerID">购买人ID号：</label>
                                 <div className="col-md-3">
                                     <input className="form-control" id="CustomerID" ref="CustomerID" type="text" />

                                 </div>
                            </div>

                            <div className="form-group">
                                  <label className="col-md-3 control-label" htmlFor="SaleDate">购买时间：</label>
                                  <div className="col-md-3">
                                      <input className="form-control" id="SaleDate" ref="SaleDate" type="text" />

                                  </div>

                                <label className="col-md-3 control-label" htmlFor="Gifts">赠送礼品：</label>
                                  <div className="col-md-3">
                                      <textarea  className="form-control" id="Gifts" ref="Gifts" rows="3" />
                                  </div>
                            </div>


                            <div className="form-group">
                                  <label className="col-md-3 control-label" htmlFor="Level">会员类别：</label>
                                  <div className="col-md-3">
                                      <select className="form-control" id="Level" ref="Level">
                                        <option value="level0" selected>普通会员</option>
                                        <option value="level1">高级会员</option>
                                      </select>

                                  </div>
                            </div>


                              <div className="form-group">
                                  <div className="col-md-offset-3 col-md-9">
                                      <button type="submit" className="btn btn-default">保存</button>
                                  </div>
                              </div>
                  </form>
               </section>
            </div>
          </div>
        );
    }
});


if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = sellMattress;
}