import React, {Component} from 'react'

import Sepes from '../sepes.js';
import PodRules from './PodRules';
import PodDataset from './PodDataset.js';
const sepes = new Sepes();

class PodPage extends Component {
    constructor(props) {
        super(props);
        this.state = {
            openInternet: false,
            incoming: [],
            outgoing: [],
            dataset: [],
        }
    }
    render() {
        return (
        <div>
            <header>
                <span><b>
                    <span onClick={() => this.props.changePage("studies")}>Sepes</span> > 
                    <span onClick={() => this.props.changePage("study")}>Study</span> > Pods > </b></span>
                <input type="text" placeholder="Pod name" id="new-study-input"/>
                <button>Save</button>
                <span className="loggedInUser">Logged in as <b>{ this.props.state.userName }</b></span>
            </header>
            <div className="sidebar podsidebar">
                <div>
                    <div style={{padding: "20px"}}>
                        <label><input type="checkbox" onChange="" />
                            Open internet for this pod
                        </label>
                    </div>
                </div>
                <PodRules header="Incoming rules" data={this.state.incoming} addItem={this.addIncomingRule} removeItem={sepes.removeItemFromStudy}/>
                <PodRules header="Outgoing rules" data={this.state.outgoing} addItem={this.addOutgoingRule} removeItem={sepes.removeItemFromStudy}/>
            </div>
            <div id="pod-dataset-list">
                <PodDataset header="Goliat" />
                <PodDataset header="Gullfaks" />
                <PodDataset header="Sleipner" />
            </div>
        </div>);
    }

    addIncomingRule = (port, ip) => {
        this.setState({
            incoming: [...this.state.incoming, {port, ip}]
        });
    }

    addOutgoingRule = (port, ip) => {
        this.setState({
            outgoing: [...this.state.outgoing, {port, ip}]
        });
    }
}

export default PodPage;
