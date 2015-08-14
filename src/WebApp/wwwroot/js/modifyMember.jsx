var React = React || require('react');
var Actions = Actions || require("./Actions.js");
var Reflux = Reflux || require("reflux");

var modifyMember = React.createClass({

    getInitialState: function () {

        console.debug("getInitialState", this.state);

        return {
                MemberID: "",
                Name: "",
                ReferenceID: "",
                Address: "",
                Phone: "",
                Level: "level1",
                IDCard: ""
        }
    },

    onModifyMemberDone: function (data) {
        alert("修改成功！");
    },

    onModifyMemberFail: function () {
        alert("修改会员信息出错！");
    },

    componentWillMount: function () {
        Actions.modifyMemberDone.listen(this.onModifyMemberDone);
        Actions.modifyMemberFail.listen(this.onModifyMemberFail);
    },

    componentDidMount: function () {
        this.props.AccountID = this.refs.AccountID;
        this.props.ReferenceID = this.refs.ReferenceID;
        this.props.Name = this.refs.Name;
        this.props.CardID = this.refs.CardID;
        this.props.Address = this.refs.Address;
        this.props.Phone = this.refs.Phone;
        this.props.Level = this.refs.Level;
    },

    componentDidUpdate: function () {
        console.debug("componentWillUpdate", this.state);
        React.findDOMNode(this.props.AccountID).value = this.state.MemberID || "";
        React.findDOMNode(this.props.ReferenceID).value = this.state.ReferenceID || "";
        React.findDOMNode(this.props.Name).value = this.state.Name || "";
        React.findDOMNode(this.props.CardID).value = this.state.IDCard || "";
        React.findDOMNode(this.props.Address).value = this.state.Address || "";
        React.findDOMNode(this.props.Phone).value = this.state.Phone || "";
        React.findDOMNode(this.props.Level).value = this.state.Level || "";
    },



    handleSubmit: function (e) {
        e.preventDefault();
        var accountID = this.state.MemberID;
        // var referenceID = React.findDOMNode(this.props.ReferenceID).value.trim();
        // var name = React.findDOMNode(this.props.Name).value.trim();
        // var cardID = React.findDOMNode(this.props.CardID).value.trim();
        var address = React.findDOMNode(this.props.Address).value.trim();
        var phone = React.findDOMNode(this.props.Phone).value.trim();
        var level = React.findDOMNode(this.props.Level).value.trim();

        if (!accountID) {
            alert("请选择要修改的会员。");
        }

        Actions.modifyMember(accountID,  address, phone, level);
        return;
    },

    render: function () {
        return (
          <div className="row">
            <div className="col-md-12">
               <section>
                  <form className="form-horizontal" method="post" role="form" onSubmit={this.handleSubmit}>
                        <h4>修改会员信息</h4>
                        <hr />
                        <div className="text-danger"></div>
                        <div className="form-group">
                            <label className="col-md-3 control-label" htmlFor="AccountID">会员ID号：</label>
                            <div className="col-md-3">
                                <input className="form-control uneditable-input"
                                       id="AccountID" ref="AccountID"type="text" readOnly
                                       defaultValue = {this.state.MemberID} />
                            </div>

                            <label className="col-md-3 control-label" htmlFor="ReferenceID">推荐人ID号：</label>
                            <div className="col-md-3">
                                <input className="form-control" id="ReferenceID" ref="ReferenceID"
                                       type="text" readOnly defaultValue={this.state.ReferenceID} />

                            </div>

                        </div>

                        <div className="form-group">
                             <label className="col-md-3 control-label" htmlFor="Name">姓名：</label>
                             <div className="col-md-3">
                                 <input className="form-control" id="Name" ref="Name" type="text"
                                         readOnly defaultValue = {this.state.Name} />

                             </div>

                            <label className="col-md-3 control-label" htmlFor="CardID">身份证号：</label>
                             <div className="col-md-3">
                                 <input className="form-control" id="CardID" ref="CardID" type="text"
                                        readOnly defaultValue={this.state.IDCard} />

                             </div>
                        </div>


                        <div className="form-group">
                              <label className="col-md-3 control-label" htmlFor="Address">联系地址：</label>
                              <div className="col-md-3">
                                  <input className="form-control" id="Address" ref="Address" type="text"
                                           defaultValue = {this.state.Address}/>

                              </div>

                             <label className="col-md-3 control-label" htmlFor="Phone">电话：</label>
                              <div className="col-md-3">
                                  <input className="form-control" id="Phone" ref="Phone" type="text"
                                         defaultValue={this.state.Phone} />

                              </div>
                        </div>


                        <div className="form-group">
                              <label className="col-md-3 control-label" htmlFor="Level">会员类别：</label>
                              <div className="col-md-3">
                                  <select className="form-control" id="Level" ref="Level"  
                                          defaultValue = {this.state.Level} >
                                    <option value="level0">普通会员</option>
                                    <option value="level1">高级会员</option>
                                  </select>
                              </div>

                              <div className="col-md-offset-3 col-md-3">
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