//@ts-check/
import React from 'react';
import './App.css';

import * as Msal from 'msal';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';

import sepes from './sepes.js';


import SepesDataList from './components/SepesDataList'

const JWT_NAME = "SepesJWT";

class App extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      tokenName: "",
      tokenId: "",
      jwtTest: "Result from backend",
      sepesData: {
        suppliers: [],
        sponsors: [],
        dataset: []
      }
    }
    this.msalConfig = {
      auth: {
        clientId: process.env.REACT_APP_AUTH_CLIENT_ID,
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
          <button className="btn" onClick={this.getSepesStudyData}>Get study data</button>
        </div>
        <div>
          <p id="tokenName">{this.state.tokenName}</p>
          <p id="tokenId">{this.state.tokenId}</p>
          <p>{this.state.jwtTest}</p>
        </div>
        <div>
          <h2>Create study</h2>
          <div>
            <input id="studyName" name="studyName" type="text" placeholder="Name of the study"></input>
          </div>
          <div>
            <h3>Sponsor</h3>
            <input list="sponsorlist"></input>
            <datalist id="sponsorlist">
              <option value="Extraterrestrial Overlord" />
              <option value="Finance department" />
              <option value="Equinor Businnes" />
              <option value="Ole Martin" />
            </datalist>
          </div>
          <div>
            <h3>Suppliers</h3>
            
          </div>
          <div>
            <h3>Dataset</h3>
            <SepesDataList data={this.state.sepesData.dataset} />
          </div>
        </div>
      </div>
    );
  }

  componentDidMount() {
    if (this.msalApp.getAccount()) {
      this.showInfo();
    }
    /*
    this.setState({
      sepesData: {
        suppliers: sepes.getSupplierList(),
        sponsors: sepes.getSponsorList(),
        dataset: sepes.getDatasetList()
      }
    });*/
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

    // First login to Azure
    this.msalApp.loginPopup(loginRequest)
      .then(loginResponse => {
      //login success
      this.setState({
        tokenName: loginResponse.account.name
      });

      // Track our login 
      this.appInsights.setAuthenticatedUserContext(loginResponse.account.name);
      this.appInsights.trackEvent({name: 'Login Azure success'});
      return loginResponse;
    })
    .then(loginResponse => {
      // Login to backend using token from Azure to get a JWT
      return fetch(process.env.REACT_APP_SEPES_LOGIN_URL, {
        method: "post",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({"Usename": loginResponse.account.name, "idToken": loginResponse.idToken.rawIdToken, "Expiration": "later"})
      });
    })
    .then(respnse => respnse.text())
      .then(jwt => {
      // Backend login success
      // Store JWT from backend
      this.setState({tokenId: jwt});
      localStorage.setItem(JWT_NAME, jwt);
      this.appInsights.trackEvent({name: 'Login Sepes success'});
    })
    .catch(error => {
      console.error(error);
      this.appInsights.trackTrace({message: 'Login Error'});
    });
  }

  logout = () => {
    this.msalApp.logout();
    localStorage.removeItem(JWT_NAME);
    this.appInsights.clearAuthenticatedUserContext();
    this.appInsights.trackEvent({name: 'Logout'});
  }

  testSepesAPI = () => {
    fetch(process.env.REACT_APP_SEPES_TEST_URL, {
      method: "get",
      headers: { "Authorization": "Bearer " + localStorage.getItem(JWT_NAME) }
    }).then(data => data.text())
    .then(data => {
      if(data.length === 0) {
        this.setState({
          jwtTest: "FAIL"
        });
      } else {
        this.setState({
          jwtTest: data
        });
      }
    }).catch(error => {
      console.log(error);
      this.setState({
        jwtTest: "FAIL"
      });
    })
  }

  getSepesStudyData = () => {
    // Get Sepes data
    sepes.getData()
      .then(data => {
        console.log(data);
        this.setState({
        sepesData: {
          dataset: data
        }
      });
    });
  }
  
}

export default App;
