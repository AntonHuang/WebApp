var React = React || require('react');
var Actions = Actions || require("./Actions.js");
var Reflux = Reflux || require("reflux");

var listMember = React.createClass({

    onFindMemberDone: function (data) {
        //this.AccountID = this.AccountID || React.findDOMNode(this.refs.AccountID);
       // this.AccountID.value = date.NextAccountID;
        console.debug("onFindMemberDone", data);
        alert("查询成功！");
    },

    onFindMemberFail: function () {
        alert("查询出错！");
    },

    componentWillMount: function () {
        Actions.findMemberDone.listen(this.onFindMemberDone);
        Actions.findMemberFail.listen(this.onFindMemberFail);

        this.CurrentPage = 1;
        this.pageSize = 10;
    },

    handleSubmit: function (e) {
        e.preventDefault();
        var accountID = React.findDOMNode(this.refs.FAccountID).value.trim();
        var referenceID = React.findDOMNode(this.refs.FReferenceID).value.trim();
        var name = React.findDOMNode(this.refs.FName).value.trim();
        var cardID = React.findDOMNode(this.refs.FCardID).value.trim();
        var phone = React.findDOMNode(this.refs.FPhone).value.trim();

        Actions.findMember(accountID, referenceID, name, cardID, phone, this.CurrentPage, this.pageSize);
        return;
    },

    render: function () {
        return (
          <div className="row">
               <section>
                  <form className="form-horizontal" method="post" role="form" onSubmit={this.handleSubmit}>
                        <h4>查询会员信息</h4>
                        <hr />
                        <div className="form-group">
                             <label className="col-md-2 control-label" htmlFor="FName">姓名：</label>
                             <div className="col-md-3">
                                 <input className="form-control" id="FName" ref="FName" type="text" />
                             </div>
                             <label className="col-md-2 control-label" htmlFor="FPhone">电话：</label>
                              <div className="col-md-3">
                                  <input className="form-control" id="FPhone" ref="FPhone" type="text" />
                              </div>
                        </div>

                        <div className="form-group">
                             <label className="col-md-2 control-label" htmlFor="FCardID">身份证号：</label>
                             <div className="col-md-3">
                                 <input className="form-control" id="FCardID" ref="FCardID" type="text" />

                            </div>

                            <label className="col-md-2 control-label" htmlFor="FReferenceID">推荐人ID号：</label>
                            <div className="col-md-3">
                                <input className="form-control" id="FReferenceID" ref="FReferenceID"
                                       type="text" />

                            </div>

                        </div>

                        <div className="form-group">
                        <label className="col-md-2 control-label" htmlFor="FAccountID">会员ID号：</label>
                        <div className="col-md-3">
                            <input className="form-control uneditable-input"
                                    id="FAccountID" ref="FAccountID" type="text" />
                        </div>

                            <div className="col-md-offset-2 col-md-3">
                                <button type="submit" className="btn btn-default btn-block btn-primary">查找</button>
                            </div>
                        </div>
                </form>
            </section>
            </div>
        );
    }
});


if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = listMember;
}