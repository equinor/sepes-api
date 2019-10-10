//@ts-check/
import React from 'react';
import './App.css';

import * as Msal from 'msal';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';

import Sepes from './sepes.js';


import StudiesPage from './components/StudiesPage';
import CreateStudyPage from './components/CreateStudyPage';
import PodPage from './components/PodPage';

const JWT_NAME = "SepesJWT";
const sepes = new Sepes();

class App extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      tokenName: "",
      tokenId: "",
      userName: "demo@sepes.com",
      jwtTest: "Result from backend",
      sepesData: {
        suppliers: [],
        sponsors: [],
        dataset: []
      },
      studyName: "",
      page: "studies",
      selection: {
        dataset: [],
        pods: [],
      },
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
        {this.state.page === "studies" ? <StudiesPage state={this.state} changePage={this.changePage} selection={this.state.selection} /> : null}
        {this.state.page === "study" ? <CreateStudyPage state={this.state} changePage={this.changePage} /> : null}
        {this.state.page === "pod" ? <PodPage state={this.state} changePage={this.changePage} /> : null}
      </div>
    );
  }


  componentDidMount() {
    if (this.msalApp.getAccount()) {
      this.showInfo();
    }
    
    this.setState({
      sepesData: {
        suppliers: sepes.getSupplierList(),
        dataset: sepes.getDatasetList()
      }
    });
  }

  showInfo = () => {
    this.setState({
      tokenName: this.msalApp.getAccount().name,
      tokenId: this.msalApp.getAccount().accountIdentifier,
      userName: this.msalApp.getAccount().userName
    });
    console.log(this.msalApp.getAccount().userName);
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
    sepes.getData()
      .then(data => {
        this.setState({
        sepesData: {
          dataset: data.dataset,
          suppliers: data.users,
          sponsors: data.users,
        }
      });
    });
  }

  createStudy = () => {
    sepes.setStudyName(this.state.studyName);
    sepes.createStudy();
  }

  changePage = (page, selection) => {
    this.setState({
      page, selection
    });
  }
  
}

export default App;
