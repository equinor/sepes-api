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

      // the pod and datasets selected in a study
      selection: {
        dataset: [],
        pod: null,
      },
      selectedStudy: {
        StudyId: null,
        StudyName: "",
      },
      // used to disable save button for a study and its pods when saving a study or pod, based on study id
      savingStudyIds: [],

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
            updateStudy={this.updateSelectedStudy}
            setSavingState={this.setSavingState}
            removeSavingState={this.removeSavingState} /> : null}
            
        {this.state.page === "pod" ? 
          <PodPage 
            state={this.state} 
            changePage={this.changePage} 
            updateStudy={this.updateSelectedStudy}
            setSavingState={this.setSavingState}
            removeSavingState={this.removeSavingState} /> : null}
      </div>
    );
  }


  componentDidMount() {
    // login to azure
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

  // set current study to be shown on study page
  setSelectedStudy = (study) => {
    this.setState({
      selectedStudy: study
    });
  }

  // updates current study if id of study is the same as current study
  updateSelectedStudy = (study) => {
    if (study.studyId === this.state.selectedStudy.studyId || this.state.selectedStudy.studyId == null) {
      this.setSelectedStudy(study)
    }
  }

  // changes the page to said page with selected pod and dataset
  changePage = (page, selection) => {
    this.setState({
      page, selection
    });
  }

  // set list of studies
  setStudies = (studies) => {
    this.setState({
      studies
    });
  }
  
  // set list of archived studies
  setArchived = (archivedStudies) => {
    this.setState({
      archived: archivedStudies
    });
  }

  // Adds an id to a list of study ids that are curently being saved to disable saving for that study
  setSavingState = (studyId) => {
    this.setState({
      savingStudyIds: [...this.state.savingStudyIds, studyId]
    });
  }

  // Removes id from list of study ids to enable saving
  removeSavingState = (studyId) => {
    let newArray = [...this.state.savingStudyIds];
    newArray.splice(newArray.indexOf(studyId), 1);
    this.setState({
      savingStudyIds: newArray
    });
  }
  
}

export default App;
