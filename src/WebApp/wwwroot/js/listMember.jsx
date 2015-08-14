var React = React || require('react');
var Griddle = require('griddle-react');
var GriddleWithCallback = require("./griddleWithCallback.jsx");
var Actions = Actions || require("./Actions.js");
var Reflux = Reflux || require("reflux");


var tableNoDataMessage = "没找到匹配的结果，请选择合适的查询条件。";
var tableColumns = ["MemberID", "Name", "Phone", "ReferenceID"];
var tablecolumsMeteData = [
 {
     "columnName": "MemberID",
     "order": 1,
     "locked": false,
     "visible": true,
     "displayName": "会员ID"
 },
  {
      "columnName": "Name",
      "order": 2,
      "locked": false,
      "visible": true,
      "displayName": "姓名"
  },
  {
      "columnName": "Phone",
      "order": 3,
      "locked": false,
      "visible": true,
      "displayName": "电话"
  },
  {
      "columnName": "ReferenceID",
      "order": 4,
      "locked": false,
      "visible": true,
      "displayName": "推荐人会员ID"
  }, ]


var listMember = React.createClass({

    getInitialState: function () {
        return {
            totalResults: 0,
            tableCallback: null
        };
    },

    loadData: function (filterString, sortColumn, sortAscending, page, pageSize, callback) {
        //alert("5 loadData ");
        // alert("this.state.totalResults = " + this.state.totalResults);
        // alert("page =" + page + " pageSize = " + pageSize);

        if (this.state.totalResults > 0) {
            this.state.tableCallback = callback;
            this.retrieveMembers(page, pageSize, this.state.lastFilterData);
        } else {
            callback({
                results: [],
                pageSize: this.pageSize
            });
        }

        /*
        swapiModule.getStarships(page, function(data){
            callback({
                results: data.results,
                totalResults: data.count,
                pageSize: pageSize
            });
        });*/
    },

    onFindMemberDone: function (data) {

        var pageSize = 10;
        var maxPage = Math.ceil(data.TotalSize / pageSize)
        var page = 0;
        // If the current page is larger than the max page, reset the page.
        if (page >= maxPage) {
            page = maxPage - 1;
        }

        var tableData = {
            results: data.Members,
            totalResults: data.TotalSize,
            pageSize: pageSize
        };

        this.state.totalResults = data.TotalSize;

        if (typeof (this.state.tableCallback) === 'function') {
            this.state.tableCallback(tableData);
        } else {
            tableData.page = page,
            tableData.maxPage = maxPage,
            //this.memberDataTable = this.memberDataTable || this.refs.memberDataTable;
            //this.memberDataTable.forceUpdate();
            this.props.memberDataTable.setState(tableData);
        }
    },

    onFindMemberFail: function () {
        alert("查询出错！");
    },

    onModifyMemberDone: function (data) {
        //this.props.memberDataTable = this.props.memberDataTable || this.refs.memberDataTable;
        this.props.memberDataTable.setPage(
            this.props.memberDataTable.state.page,
            this.props.memberDataTable.state.pageSize);
    },

    componentWillMount: function () {
        Actions.findMemberDone.listen(this.onFindMemberDone);
        Actions.findMemberFail.listen(this.onFindMemberFail);
        Actions.modifyMemberDone.listen(this.onModifyMemberDone);

        this.CurrentPage = 0;
        this.pageSize = 10;
    },

    componentDidMount: function () {
        this.props.memberDataTable = this.refs.memberDataTable;

        this.props.FAccountID = this.refs.FAccountID;
        this.props.FReferenceID = this.refs.FReferenceID;
        this.props.FName = this.refs.FName;
        this.props.FCardID = this.refs.FCardID;
        this.props.FPhone = this.refs.FPhone;
    },

    onTableRowClick: function (rowD, e) {
        //console.debug("onTableRowClick", rowD, e);
        if (typeof (this.props.onMemberTableRowClick) === 'function') {
            this.props.onMemberTableRowClick(rowD.props.data);
        }
    },

    retrieveMembers: function (pageIdx, pageSize, filterData) {

        if (filterData) {
            Actions.findMember(filterData.accountID, filterData.referenceID, filterData.name,
                filterData.cardID, filterData.phone, pageIdx, pageSize);
        } else {
            var accountID = React.findDOMNode(this.props.FAccountID).value.trim();
            var referenceID = React.findDOMNode(this.props.FReferenceID).value.trim();
            var name = React.findDOMNode(this.props.FName).value.trim();
            var cardID = React.findDOMNode(this.props.FCardID).value.trim();
            var phone = React.findDOMNode(this.props.FPhone).value.trim();

            this.state.lastFilterData = {
                accountID: accountID,
                referenceID: referenceID,
                name: name,
                cardID: cardID,
                phone: phone,
            };

            Actions.findMember(accountID, referenceID, name, cardID, phone, pageIdx, pageSize);
        }
    },

    handleSubmit: function (e) {
        e.preventDefault();
        this.state.totalResults = 0;
        this.state.tableCallback = null;
        this.retrieveMembers(this.CurrentPage, this.pageSize);

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
                             <div className="col-md-2">
                                 <input className="form-control" id="FName" ref="FName" type="text" autoFocus />
                             </div>
                             <label className="col-md-2 control-label" htmlFor="FPhone">电话：</label>
                              <div className="col-md-2">
                                  <input className="form-control" id="FPhone" ref="FPhone" type="text" />
                              </div>
                            <label className="col-md-2 control-label" htmlFor="FCardID">身份证号：</label>
                             <div className="col-md-2">
                                 <input className="form-control" id="FCardID" ref="FCardID" type="text" />

                             </div>
                        </div>

                        <div className="form-group">

                            <label className="col-md-2 control-label" htmlFor="FReferenceID">推荐人ID号：</label>
                            <div className="col-md-2">
                                <input className="form-control" id="FReferenceID" ref="FReferenceID"
                                       type="text" />

                            </div>


                        <label className="col-md-2 control-label" htmlFor="FAccountID">会员ID号：</label>
                        <div className="col-md-2">
                            <input className="form-control uneditable-input"
                                   id="FAccountID" ref="FAccountID" type="text" />
                        </div>

                            <div className="col-md-offset-2 col-md-2">
                                <button type="submit" className="btn btn-default btn-block btn-primary">查找</button>
                            </div>
                        </div>
                  </form>
               </section>

             <GriddleWithCallback showFilter={false} showSettings={false} enableSort={false}
                                  tableClassName= "table-hover table table-striped"
                                  noDataMessage={tableNoDataMessage}
                                  columns={tableColumns}
                                  columnMetadata={tablecolumsMeteData}
                                  getExternalResults={this.loadData} ref="memberDataTable"
                                  onRowClick={this.onTableRowClick}
                                  nextText="下一页"
                                  previousText="上一页" />
          </div>
        );
    }
});


if (typeof exports === "object" && typeof module !== "undefined") {
    module.exports = listMember;
}