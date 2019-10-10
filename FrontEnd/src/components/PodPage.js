import React, {Component} from 'react'

//import Sepes from '../sepes.js';
import PodRules from './PodRules';
import PodDataset from './PodDataset.js';
//const sepes = new Sepes();

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
                    <span onClick={() => this.props.changePage("study")}> Study</span> > Pod > </b></span>
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
                <PodRules header="Incoming rules" data={this.state.incoming} addItem={this.addIncomingRule} removeItem={this.removeIncomingRule}/>
                <PodRules header="Outgoing rules" data={this.state.outgoing} addItem={this.addOutgoingRule} removeItem={this.removeOutgoingRule}/>
            </div>
            <div id="pod-dataset-list">
                { this.props.state.selection.dataset.map((item) => (
                    <PodDataset header={item} />
                ))}
            </div>
        </div>);
    }

    addIncomingRule = (port, ip) => {
        if (this.findIndex(this.state.incoming, {port, ip}) === -1) {
            this.setState({
                incoming: [...this.state.incoming, {port, ip}]
            });
        }
    }

    removeIncomingRule = (rule) => {
        let newArray = [...this.state.incoming];
        newArray.splice(this.findIndex(newArray, rule), 1);
        this.setState({
            incoming: newArray
        });
    }

    removeOutgoingRule = (rule) => {
        let newArray = [...this.state.outgoing];
        newArray.splice(this.findIndex(newArray, rule), 1);
        this.setState({
            outgoing: newArray
        });
    }

    addOutgoingRule = (port, ip) => {
        if (this.findIndex(this.state.incoming, {port, ip}) === -1) {
            this.setState({
                outgoing: [...this.state.outgoing, {port, ip}]
            });
        }
    }

    findIndex = (array, rule) => {
        return array.findIndex((item) => (item.port === rule.port && item.ip === rule.ip));
    }
}

export default PodPage;
