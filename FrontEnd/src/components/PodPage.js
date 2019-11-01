import React, {Component} from 'react'

import PodRules from './PodRules';
import PodDataset from './PodDataset.js';

import * as StudyService from "../studyService"

import Sepes from '../sepes.js';
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

            networkName: "",
            resourceGroupName: "",
            addressSpace: "",

            saveBtnDisabled: false
        }
    }
    render() {
        return (
        <div>
            <header>
                <span><b>
                    <span className="link" onClick={() => this.props.changePage("studies")}>Sepes</span> > <span 
                        className="link" onClick={() => this.props.changePage("study")}>Study</span> > Pod </b></span>
                    <link />
                <input type="text" placeholder="Pod name" id="new-study-input" value={this.state.podName} onChange={(e)=> this.setState({podName: e.target.value})} />
                { this.state.podId === null ? <button disabled={this.state.saveBtnDisabled} onClick={this.createPod}>Save</button> : null }
                <span className="loggedInUser">Logged in as <b>{ this.props.state.userName }</b></span>
            </header>
            <div className="sidebar podsidebar">
                <div>
                    <div style={{padding: "20px"}}>
                        <label><input type="checkbox" onChange={()=>console.log("Toggle internet")} />
                            Open internet for this pod
                        </label>
                    </div>
                </div>
                <PodRules header="Incoming rules" data={this.state.incoming} addItem={this.addIncomingRule} removeItem={this.removeIncomingRule}/>
                <PodRules header="Outgoing rules" data={this.state.outgoing} addItem={this.addOutgoingRule} removeItem={this.removeOutgoingRule}/>
                { this.state.networkName !== "" ? 
                <div style={{padding: 5}}>
                    <p>Network name: { this.state.networkName }</p>
                    <p>Resource group: { this.state.resourceGroupName }</p>
                    <p>Address space: { this.state.addressSpace }</p>
                </div> : null } 
            </div>
            <div id="pod-dataset-list">
                { this.props.state.selection.dataset.map((item) => (
                    <PodDataset header={item} />
                ))}
            </div>
        </div>);
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
        this.setState({saveBtnDisabled: true});
        
        console.log(`New pod: ${this.props.state.selectedStudy.StudyId}, ${this.state.podName}`)
        
        sepes.createPod(this.props.state.selectedStudy.StudyId, this.state.podName)
            .then(returnValue => returnValue.json())
            .then(json => {
                this.setState({
                    podId: parseInt(json.id),
                    networkName: json.networkName,
                    resourceGroupName: json.resourceGroupName,
                    addressSpace: json.addressSpace
                });
                console.log(json)
            })
            .catch(() => {
                this.setState({saveBtnDisabled: false});
            });
    }
}

export default PodPage;
