import React, { Component } from 'react';

import PodRules from './PodRules';
import PodDataset from './PodDataset.js';
import Nsgswitch from './nsgSwitch';

import * as StudyService from "../studyService"
import Sepes from '../sepes.js';
import spinner from "../spinner.svg"


const sepes = new Sepes();

class PodPage extends Component {
    constructor(props) {
        super(props);
        this.state = {
            openInternet: false,
            incoming: [],
            outgoing: [],
            dataset: [],
            podName: "",
            podId: null
        }
    }

    render() {
        let appstate = this.props.state;
        let study = appstate.selectedStudy;
        // disables saving based on study id
        let disableSave = false;
        appstate.savingStudyIds.forEach(id => {
            if (id === study.studyId) disableSave = true;
        });

        return (
            <div>
                <header>
                    <span><b>
                        <span className="link" onClick={() => this.props.changePage("studies")}>Studies</span> > <span
                            className="link" onClick={() => this.props.changePage("study")}>{study.studyName}</span> > </b></span>
                    <input type="text" placeholder="Pod name" id="new-study-input" value={this.state.podName} onChange={(e) => this.setState({ podName: e.target.value })} />
                    <button disabled={disableSave} onClick={this.savePod}>Save</button>
                    { disableSave ? <img src={spinner} className="spinner" alt="" /> : null }
                    <span className="loggedInUser">Logged in as <b>{appstate.userName}</b></span>
                </header>
                <div className="sidebar podsidebar">
                    <div>
                        <div className="nsgSwitchclass" style={{ padding: "20px" }}>
                        <label>Open internet
                            <Nsgswitch
                            isOn={this.state.openInternet}
                            onColor="#EF476F"
                            handleToggle={this.updateNsg}
                            /></label>
                        </div>
                    </div>
                    <PodRules header="Incoming rules" data={this.state.incoming} addItem={this.addIncomingRule} removeItem={this.removeIncomingRule} />
                    <PodRules header="Outgoing rules" data={this.state.outgoing} addItem={this.addOutgoingRule} removeItem={this.removeOutgoingRule} />
                </div>
                <div id="pod-dataset-list">
                    {appstate.selection.dataset.map((item) => (
                        <PodDataset header={item} />
                    ))}
                </div>
            </div>);
    }

    componentDidMount() {
        let pod = this.props.state.selection.pod;
        if (pod.podId !== null) {
            this.setState({
                incoming: pod.incoming,
                outgoing: pod.outgoing,
                podName: pod.podName,
                openInternet: pod.openInternet,
                podId: pod.podId
            });
        }
    }

    updateNsg = () => {
        this.setState({openInternet: !this.state.openInternet});
    }

    addIncomingRule = (port, ip) => {
        this.setState({
            incoming: StudyService.addRule(port, ip, this.state.incoming)
        });
    }

    removeIncomingRule = (rule) => {
        this.setState({
            incoming: StudyService.removeRule(rule.port, rule.ip, this.state.incoming)
        });
    }

    addOutgoingRule = (port, ip) => {
        this.setState({
            outgoing: StudyService.addRule(port, ip, this.state.outgoing)
        });
    }

    removeOutgoingRule = (rule) => {
        this.setState({
            outgoing: StudyService.removeRule(rule.port, rule.ip, this.state.outgoing)
        });
    }

    savePod = () => {
        let props = this.props;
        
        // set current selected study as the base study
        let based = props.state.selectedStudy;

        // make copy of base study, will become updated study
        let study = JSON.parse(JSON.stringify(based));
        
        // create a new pod object
        let pod = {
            incoming: this.state.incoming,
            outgoing: this.state.outgoing,
            podName: this.state.podName,
            podId: this.state.podId,
            openInternet: this.state.openInternet,
            studyId: study.studyId
        }
        
        // add new pod to list of pods in updated study, or update existing pod
        if (this.state.podId === null) {
            if (typeof study.pods === "undefined") {
                study.pods = [];
            }
            
            study.pods.push(pod);
        }
        else {
            let index = study.pods.findIndex(item => item.podId === pod.podId);
            study.pods[index] = pod;
        }
        
        // disable save button
        props.setSavingState(study.studyId);

        // save study
        sepes.saveStudy(study, based)
            .then(returnValue => returnValue.json())
            .then(returnedStudy => {
                if (this.state.podId === null) {
                    let podId = returnedStudy.pods[returnedStudy.pods.length - 1].podId
                    this.setState({podId});
                }

                // Update base study
                props.updateStudy(returnedStudy);
                
                // Enable save button
                props.removeSavingState(study.studyId);
            })
            .catch(() => {
                props.removeSavingState(study.studyId);
            });
    }
}

export default PodPage;
