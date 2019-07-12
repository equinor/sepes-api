import React from 'react';
import './App.css';

import * as Msal from 'msal'


class App extends React.Component {
  state = {
    tokenName: "",
    tokenId: "",
  }
  
  msalConfig = {
    auth: {
      clientId: "e90cbb61-896e-4ec7-aa37-23511700e1ed",
      authority: "https://login.microsoftonline.com/3aa4a235-b6e2-48d5-9195-7fcf05b459b0"
    },
    cache: {
      cacheLocation: "localStorage",
      storeAuthStateInCookie: true
    }
  };
  msalApp = new Msal.UserAgentApplication(this.msalConfig)

  render() {
    return (
      <div className="App">
        <div>
          <button className="btn" onClick={this.login}>Logg inn</button>
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
    })
  }

  login = () => {
    const loginRequest = {
      scopes: ["user.read"]/*, "user.write"*/
    };
  
    this.msalApp.loginPopup(loginRequest).then(function (loginResponse) {
      //login success
      console.log(loginResponse)
      this.showInfo();
    })
      .catch(function (error) {
        console.log(error);
      });
  }

  logout = () => {
    this.msalApp.logout();
  }
}

export default App;
