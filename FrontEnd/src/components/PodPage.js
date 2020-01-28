import React, { Component } from 'react';

import PodRules from './PodRules';
import PodDataset from './PodDataset.js';

import * as StudyService from "../studyService"

import spinner from "../spinner.svg"

import Sepes from '../sepes.js';
import Nsgswitch from './nsgSwitch';
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
            podId: null,

            saveBtnDisabled: false
        }
    }
    render() {
        let appstate = this.props.state;
        return (

            <div>
                <header>
                    <span><b>
                        <span className="link" onClick={() => this.props.changePage("studies")}>Sepes</span> > <span
                            className="link" onClick={() => this.props.changePage("study")}>Study</span> > Pod </b></span>
                    <link />
                    <input type="text" placeholder="Pod name" id="new-study-input" value={this.state.podName} onChange={(e) => this.setState({ podName: e.target.value })} />
                    <button disabled={appstate.saving} onClick={this.createPod}>Save</button>
                    {appstate.saving ? <img src={spinner} className="spinner" alt="" /> : null}
                    <span className="loggedInUser">Logged in as <b>{this.props.state.userName}</b></span>
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
                        <div>
                            {appstate.selectedStudy.archived ? <button className="deletebtn" onClick={this.deletePod}>Delete Pod</button> : null}
                        </div>
                    </div>
                    <PodRules header="Incoming rules" data={this.state.incoming} addItem={this.addIncomingRule} removeItem={this.removeIncomingRule} />
                    <PodRules header="Outgoing rules" data={this.state.outgoing} addItem={this.addOutgoingRule} removeItem={this.removeOutgoingRule} />
                </div>
                <div id="pod-dataset-list">
                    {this.props.state.selection.dataset.map((item) => (
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
        this.setState({ openInternet: !this.state.openInternet });
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

    createPod = () => {
        let props = this.props;
        props.setSavingState(true);

        let based = props.state.selectedStudy;
        let study = JSON.parse(JSON.stringify(based));

        let pod = {
            incoming: this.state.incoming,
            outgoing: this.state.outgoing,
            podName: this.state.podName,
            podId: this.state.podId,
            openInternet: this.state.openInternet,
            studyId: study.studyId
        }

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

        sepes.createStudy(study, based)
            .then(returnValue => returnValue.json())
            .then(study => {
                if (this.state.podId === null) {
                    let podId = study.pods[study.pods.length - 1].podId
                    this.setState({
                        podId
                    });
                }
                props.setStudy(study);
                props.setSavingState(false);
            })
            .catch(() => {
                props.setSavingState(false);

            });
    }

    deletePod = () => {
        let props = this.props;
        props.setSavingState(true);

        let based = props.state.selectedStudy;
        let study = JSON.parse(JSON.stringify(based));
        //Removes pod from Study
        study.pods.splice(study.pods.findIndex(pod => pod.podId === this.state.podId), 1);
        
        //Send request to backend
        sepes.createStudy(study, based)
            .then(returnValue => returnValue.json())
            .then(study => {
                if (this.state.podId === null) {
                    let podId = study.pods[study.pods.length - 1].podId
                    this.setState({
                        podId
                    });
                }
                props.setStudy(study);
                props.setSavingState(false);
                props.changePage("studies")
            })
            .catch(() => {
                props.setSavingState(false);
                props.changePage("studies")

            });
    }
}

export default PodPage;
