import React from 'react';
import './App.css';

import * as Msal from 'msal';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';


class App extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      tokenName: "",
      tokenId: "",
    }
    this.msalConfig = {
      auth: {
        clientId: "e90cbb61-896e-4ec7-aa37-23511700e1ed",
        authority: "https://login.microsoftonline.com/3aa4a235-b6e2-48d5-9195-7fcf05b459b0"
      },
      cache: {
        cacheLocation: "localStorage",
        storeAuthStateInCookie: true
      }
    };
    this.msalApp = new Msal.UserAgentApplication(this.msalConfig);
    this.appInsights = new ApplicationInsights({ config: {
      instrumentationKey: process.env.REACT_APP_INSTRUMENTATION_KEY
    } });
    this.appInsights.loadAppInsights();
  }

  render() {
    return (
      <div className="App">
        <div>
          <button className="btn" onClick={() => this.login()}>Logg inn</button>
          <button className="btn" onClick={this.logout}>Logg ut</button>
        </div>
        <div>
          <p id="tokenName">{this.state.tokenName}</p>
          <p id="tokenId">{this.state.tokenId}</p>
        </div>
      </div>
    );
  }

  componentDidMount() {
    if (this.msalApp.getAccount()) {
      this.showInfo();
    }
  }

  showInfo = () => {
    this.setState({
      tokenName: this.msalApp.getAccount().name,
      tokenId: this.msalApp.getAccount().accountIdentifier
    });
  }

  login = () => {
    const loginRequest = {
      scopes: ["user.read"]/*, "user.write"*/
    };

    var self = this;

    this.msalApp.loginPopup(loginRequest).then(function (loginResponse) {
      //login success
      self.setState({
        tokenName: loginResponse.account.name,
        tokenId: loginResponse.account.accountIdentifier
      });

      self.appInsights.setAuthenticatedUserContext(loginResponse.account.name);
      self.appInsights.trackEvent({name: 'Login'});
    }).catch(function (error) {
      console.log(error);
      self.appInsights.trackTrace({message: 'Login Error'});
    });
  }

  logout = () => {
    this.msalApp.logout();
    this.appInsights.clearAuthenticatedUserContext();
    this.appInsights.trackEvent({name: 'Logout'});
  }
}

export default App;
