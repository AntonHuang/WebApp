var React = React || require('react');
var Actions = Actions || require("./Actions.js");
var Reflux = Reflux || require("reflux");

var modifyMember = React.createClass({

    onModifyMemberDone: function (data) {
        this.AccountID = this.AccountID || React.findDOMNode(this.refs.AccountID);
        this.AccountID.value = date.NextAccountID;

        alert("修改成功！");
    },

    onModifyMemberFail: function () {
        alert("修改会员信息出错！");
    },

    componentWillMount: function () {
        Actions.modifyMemberDone.listen(this.onModifyMemberDone);
        Actions.modifyMemberFail.listen(this.onModifyMemberFail);

        this.props = {
            User: {
                ID: "USERID",
                Name: "Anton",
                Reference: "PRef",
                Address: "Address",
                Phone: "Phne",
                Level: "level1",
                IDCard: "iDCCC"
            }
        }

        this.User = this.props.User;
    },

    handleSubmit: function (e) {
        e.preventDefault();
        var accountID = this.User.ID;
        var referenceID = React.findDOMNode(this.refs.ReferenceID).value.trim();
        var name = React.findDOMNode(this.refs.Name).value.trim();
        var cardID = React.findDOMNode(this.refs.CardID).value.trim();
        var address = React.findDOMNode(this.refs.Address).value.trim();
        var phone = React.findDOMNode(this.refs.Phone).value.trim();
        var level = React.findDOMNode(this.refs.Level).value.trim();

        Actions.modifyMember(accountID, referenceID, name, cardID, address, phone, level);
        return;
    },

    render: function () {
        return (
          <div className="row">
            <div className="col-md-8">
               <section>
                  <form className="form-horizontal" method="post" role="form" onSubmit={this.handleSubmit}>
                        <h4>修改会员信息</h4>
                        <hr />
                        <div className="text-danger"></div>
                        <div className="form-group">
                            <label className="col-md-3 control-label" htmlFor="AccountID">会员ID号：</label>
                            <div className="col-md-9">
                                <input className="form-control uneditable-input"
                                       id="AccountID" ref="AccountID"type="text" readOnly
                                       defaultValue = {this.User.ID} />
                            </div>
                        </div>
                        <div className="form-group">
                            <label className="col-md-3 control-label" htmlFor="ReferenceID">推荐人ID号：</label>
                            <div className="col-md-9">
                                <input className="form-control" id="ReferenceID" ref="ReferenceID"
                                       type="text" readOnly defaultValue = {this.User.Reference}/>

                            </div>
                        </div>

                        <div className="form-group">
                             <label className="col-md-3 control-label" htmlFor="Name">姓名：</label>
                             <div className="col-md-9">
                                 <input className="form-control" id="Name" ref="Name" type="text"
                                         readOnly defaultValue = {this.User.Name} />

                             </div>
                        </div>

                        <div className="form-group">
                             <label className="col-md-3 control-label" htmlFor="CardID">身份证号：</label>
                             <div className="col-md-9">
                                 <input className="form-control" id="CardID" ref="CardID" type="text"
                                         readOnly defaultValue = {this.User.IDCard} />

                             </div>
                        </div>

                        <div className="form-group">
                              <label className="col-md-3 control-label" htmlFor="Address">联系地址：</label>
                              <div className="col-md-9">
                                  <input className="form-control" id="Address" ref="Address" type="text"
                                          autoFocus defaultValue = {this.User.Address}/>

                              </div>
                        </div>

                        <div className="form-group">
                              <label className="col-md-3 control-label" htmlFor="Phone">电话：</label>
                              <div className="col-md-9">
                                  <input className="form-control" id="Phone" ref="Phone" type="text"
                                           defaultValue = {this.User.Phone}/>

                              </div>
                        </div>

                        <div className="form-group">
                              <label className="col-md-3 control-label" htmlFor="Level">会员类别：</label>
                              <div className="col-md-9">
                                  <select className="form-control" id="Level" ref="Level"  
                                          defaultValue = {this.User.Level} >
                                    <option value="level0">普通会员</option>
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
    module.exports = modifyMember;
}