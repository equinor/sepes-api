import React, {Component} from 'react'

import Sepes from '../sepes.js';
const sepes = new Sepes();

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
                    { this.state.studies.map((item) => (
                        <div className="study" onClick={this.newStudy}>
                            <p>{item.StudyName}</p>
                        </div>
                    ))}
                </div>
                <div>
                    <button onClick={this.showArchived}>Show archived studies</button>
                </div>
                <div style={{paddingTop: 30, display: "table"}}>
                    { this.state.archivedStudies.map((item) => (
                        <div className="study" onClick={this.newStudy}>
                            <p>{item.StudyName}</p>
                            <p>Pods: 2</p>
                            <p>Datasets: 5</p>
                        </div>
                    ))}
                </div>
            </div>
        </div>);
    }

    componentDidMount() {
        sepes.getStudies(false).then(response => response.json())
            .then(json => {
                console.log("fetch studies");
                console.log(json);
                this.setState({studies: json});
            });
    }

    newStudy = () => {
        this.props.changePage("study", {});
    }

    showArchived = () => {
        //this.setState({archivedStudies: ["Old study", "Good old days", "Remember when things just worked?", "Nostalgia"]})
        sepes.getStudies(true)
            .then(response => response.json())
            .then(json => {
                console.log("fetch archived studies");
                console.log(json);
                this.setState({archivedStudies: json});
            });
    }
}

export default CreateStudyPage;
