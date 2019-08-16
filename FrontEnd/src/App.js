import React from 'react';
import './App.css';

import * as Msal from 'msal';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';

const JWT_NAME = "SepesJWT";

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
          <button className="btn" onClick={this.login}>Logg inn</button>
          <button className="btn" onClick={this.logout}>Logg ut</button>
          <button className="btn" onClick={this.testSepesAPI}>Test backend API</button>
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

    this.msalApp.loginPopup(loginRequest)
      .then(loginResponse => {
      //login success
      this.setState({
        tokenName: loginResponse.account.name,
        tokenId: loginResponse.account.accountIdentifier
      });

      this.appInsights.setAuthenticatedUserContext(loginResponse.account.name);
      this.appInsights.trackEvent({name: 'Login Azure success'});
      console.log(loginResponse);
      return loginResponse.idToken.rawIdToken;
    }).then(rawIdToken => {
      console.log(rawIdToken);
      return fetch(process.env.REACT_APP_SEPES_LOGIN_URL, {
        method: "post",
        headers: { "Content-Type" : "application/json" },
        body: JSON.stringify({"Usename":"Tester","idToken":"aoiuy12331","Expiration":"later"})
      });
    }).then(respnse => respnse.text())
      .then(jwt => {
      console.log(jwt);
      localStorage.setItem(JWT_NAME, jwt);
      this.appInsights.trackEvent({name: 'Login Sepes success'});
    }).catch(error => {
      console.error(error);
      this.appInsights.trackTrace({message: 'Login Error'});
    });
  }

  logout = () => {
    this.msalApp.logout();
    this.appInsights.clearAuthenticatedUserContext();
    this.appInsights.trackEvent({name: 'Logout'});
    localStorage.removeItem(JWT_NAME);
  }

  testSepesAPI() {
    fetch(process.env.REACT_APP_SEPES_TEST_URL, {
      method: "post",
      headers: { Authorization: localStorage.getItem(JWT_NAME) }
    }).then(data => {
        console.log(data);
      });
  }
}

export default App;
