var React = require('react');

var Router = require('react-router')
  , RouteHandler = Router.RouteHandler
  , Route = Router.Route
  , DefaultRoute = Router.DefaultRoute;

var ReactBootstrap = require('react-bootstrap')
  , Nav = ReactBootstrap.Nav
  , ListGroup = ReactBootstrap.ListGroup;

var ReactRouterBootstrap = require('react-router-bootstrap')
  , NavItemLink = ReactRouterBootstrap.NavItemLink
  , ButtonLink = ReactRouterBootstrap.ButtonLink
  , ListGroupItemLink = ReactRouterBootstrap.ListGroupItemLink;


require("./store/loginStore.js");
require("./store/registerStore.js");
var Login = require("./login.jsx");
var Register = require("./register.jsx");
var ForgotPassword = require("./forgotPassword.jsx");

var App = React.createClass({
    render: function () {
        return (
            <RouteHandler />
        );
    }
});

var Account = React.createClass({
    render: function () {
        return (
           <RouteHandler />
        );
    }
});

var Home = React.createClass({
    render: function () {
       return (
          <div className="row">
            <div className="col-md-3">
                <ListGroup>
                    <ListGroupItemLink to="login">
                        Login!
                    </ListGroupItemLink>
                    <ListGroupItemLink to="destination" params={{ someparam: 'hello' }}>
                        Linky!
                    </ListGroupItemLink>
                </ListGroup>
            </div>
            <div className="col-md-9">
              <RouteHandler />
            </div>
          </div>
        );
    }
});

var Destination = React.createClass({
    render: function () {
        return <div>You made it!</div>;
    }
});

var routes = (
  <Route handler={App} path="/">
    <Route handler={Account} path="account">
      <Route name="login" path="login" handler={Login} />
      <Route name="register" path="register" handler={Register} />
      <Route name="forgotPassword" path="forgotpassword" handler={ForgotPassword} />
    </Route>
    <Route handler={Home} >
       <Route name="destination" path="destination/:someparam" handler={Destination} />
    </Route>
    <DefaultRoute handler={Home} />
  </Route>
);

Router.run(routes, function (Handler) {
    React.render(<Handler />, document.getElementById("app_main"));
});
