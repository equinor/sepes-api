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
            <div style={{padding: 50, paddingRight: 0}}>
                <div style={{display: "table"}}>
                    <div className="study" onClick={this.newStudy}>
                        <p style={{fontWeight: "bold"}}>New Study</p>
                    </div>
                    { this.props.state.studies.map((item) => (
                        <div className="study" onClick={() => this.openStudy(item)}>
                            <p style={{fontWeight: "bold"}}>{item.studyName}</p>
                            <p>Pods: {item.pods.length}</p>
                            <p>Users: {item.suppliers.length}</p>
                        </div>
                    ))}
                </div>
                <div>
                    <button onClick={this.showArchived}>Show archived studies</button>
                </div>
                <div style={{paddingTop: 30, display: "table"}}>
                    { this.state.archivedStudies.map((item) => (
                        <div className="study" onClick={() => this.openStudy(item)}>
                            <p style={{fontWeight: "bold"}}>{item.StudyName}</p>
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

                this.props.setStudies(json);
            });
    }

    newStudy = () => {
        this.props.setStudy({studyId: null, studyName: ""});
        this.props.changePage("study", {});
    }

    openStudy = (study) => {
        this.props.setStudy(study);
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
