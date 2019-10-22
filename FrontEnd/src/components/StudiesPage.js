import React, {Component} from 'react'

//import Sepes from '../sepes.js';
//const sepes = new Sepes();

class CreateStudyPage extends Component {
    constructor(props) {
        super(props);
        this.state = {
            studies: [],
            archivedStudies: [],
        }
    }
    
    render() {
        return (
        <div>
            <header>
                <span><b>Sepes</b></span>
                <span className="loggedInUser">Logged in as <b>{ this.props.state.userName }</b></span>
            </header>
            <div style={{padding: 50}}>
                <div style={{display: "table"}}>
                    <div className="study" onClick={this.newStudy}>
                        <p>New Study</p>
                    </div>
                    <div className="study" onClick={this.newStudy}>
                        <p>Equinor test study</p>
                        <p>Pods: 2</p>
                        <p>Datasets: 5</p>
                    </div>
                    <div className="study" onClick={this.newStudy}>
                        <p>Brilliant study</p>
                        <p>Pods: 1</p>
                        <p>Datasets: 1</p>
                    </div>
                </div>
                <div>
                    <button onClick={this.showArchived}>Show archived studies</button>
                </div>
                <div style={{paddingTop: 20}}>
                    { this.state.archivedStudies.map((item) => (
                        <div className="study" onClick={this.newStudy}>
                            <p>{item}</p>
                            <p>Pods: 2</p>
                            <p>Datasets: 5</p>
                        </div>
                    ))}
                </div>
            </div>
        </div>);
    }

    newStudy = () => {
        this.props.changePage("study", {});
    }

    showArchived = () => {
        this.setState({archivedStudies: ["Old study", "Good old days", "Remember when things just worked?", "Nostalgia"]})
    }
}

export default CreateStudyPage;
