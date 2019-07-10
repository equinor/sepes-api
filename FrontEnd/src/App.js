import React from 'react';
import './App.css';

import * as Msal from 'msal'


class App extends React.Component {
  componentDidMount() {
    if (msalApp.getAccount()) {
      showInfo();
    }
  }

  render() {
    return (
      <div className="App">
        <div>
          <button onClick={login}>Logg inn</button>
          <button onClick={logout}>Logg ut</button>
        </div>
        <div>
          <p id="tokenName"></p>
          <p id="tokenId"></p>
        </div>
      </div>
    );
  }
}

var msalConfig = {
  auth: {
    clientId: "e90cbb61-896e-4ec7-aa37-23511700e1ed",
    authority: "https://login.microsoftonline.com/3aa4a235-b6e2-48d5-9195-7fcf05b459b0"
  },
  cache: {
    cacheLocation: "localStorage",
    storeAuthStateInCookie: true
  }
};

var msalApp = new Msal.UserAgentApplication(msalConfig);

function login() {
  const loginRequest = {
    scopes: ["user.read"]/*, "user.write"*/
  };

  msalApp.loginPopup(loginRequest).then(function (loginResponse) {
    //login success
    console.log(loginResponse)
    showInfo();
  })
    .catch(function (error) {
      console.log(error);
    });
}

function logout() {
  msalApp.logout();
}

function showInfo() {
  document.getElementById("tokenName").innerText = msalApp.getAccount().name;
  document.getElementById("tokenId").innerText = msalApp.getAccount().accountIdentifier;
}

export default App;
