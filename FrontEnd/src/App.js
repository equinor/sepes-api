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
      page: "none",
      selection: {
        dataset: [],
        pods: [],
      },
      selectedStudy: {
        StudyId: null,
        StudyName: "",
      },
      // used to disable save button for a study and its pods when saving a study or pod, based on study id
      savingStudyId: -1,
      
      
      jwtTest: "Result from backend",
      studies: [],
      archived: []
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
        {this.state.page === "studies" ? 
          <StudiesPage state={this.state} 
            changePage={this.changePage} 
            selection={this.state.selection} 
            setStudy={this.setSelectedStudy} 
            setStudies={this.setStudies} 
            setArchived={this.setArchived} /> : null}

        {this.state.page === "study" ? 
          <CreateStudyPage 
            state={this.state} 
            changePage={this.changePage} 
            setStudy={this.setSelectedStudy}
            setSavingState={this.setSavingState} /> : null}
            
        {this.state.page === "pod" ? 
          <PodPage 
            state={this.state} 
            changePage={this.changePage} 
            setStudy={this.setSelectedStudy}
            setSavingState={this.setSavingState} /> : null}
      </div>
    );
  }


  componentDidMount() {
    if (this.msalApp.getAccount()) {
      this.showInfo();
      let account = this.msalApp.getAccount();

      sepes.getSepesToken(account.userName, account.accountIdentifier)
        .then(respnse => respnse.text())
        .then(jwt => {
          // Backend login success
          // Store JWT from backend
          localStorage.setItem(JWT_NAME, jwt);
          this.setState({tokenId: jwt, page: "studies"});
          this.appInsights.trackEvent({name: 'Login Sepes success'});
        });
    }
    else {
      if (typeof this.props.noLogin === "undefined") {
        this.login();
      }
      
    }
  }

  showInfo = () => {
    this.setState({
      tokenName: this.msalApp.getAccount().name,
      tokenId: this.msalApp.getAccount().accountIdentifier,
      userName: this.msalApp.getAccount().userName
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
      return sepes.getSepesToken(loginResponse.account.name, loginResponse.idToken.rawIdToken);
    })
    .then(respnse => respnse.text())
      .then(jwt => {
      // Backend login success
      // Store JWT from backend
      localStorage.setItem(JWT_NAME, jwt);
      this.setState({tokenId: jwt, page: "studies"});
      this.appInsights.trackEvent({name: 'Login Sepes success'});
    })
    .catch(() => {
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
    fetch(process.env.REACT_APP_SEPES_BASE_URL+"/api/values", {
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

  setSelectedStudy = (study) => {
    this.setState({
      selectedStudy: study
    });
  }

  changePage = (page, selection) => {
    this.setState({
      page, selection
    });
  }

  setStudies = (studies) => {
    this.setState({
      studies
    });
  }
  
  setArchived = (archivedStudies) => {
    this.setState({
      archived: archivedStudies
    });
  }

  setSavingState = (savingStudyId) => {
    this.setState({
      savingStudyId
    });
  }
  
}

export default App;
