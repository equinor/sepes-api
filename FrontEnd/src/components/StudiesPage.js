import React, {Component} from 'react'

import Sepes from '../sepes.js';
import addSymbol from '../plus1.svg';
import StudyList from './StudyList.js';

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
                <span><b>Studies</b></span>
                <span className="loggedInUser">Logged in as <b>{ this.props.state.userName }</b></span>
            </header>
            <div style={{padding: 50, paddingRight: 0}}>
                <div style={{display: "table"}}>
                    <div className="study" onClick={this.newStudy}>
                        <p style={{fontWeight: "bold"}}>New Study</p>
                        <p><img src={addSymbol} alt={"+"} style={{width: 60}}/></p>
                    </div>
                    <StudyList studies={this.state.studies} openStudy={this.openStudy} />
                </div>
                <div>
                    <button onClick={this.showArchived}>Show archived studies</button>
                </div>
                <div style={{paddingTop: 30, display: "table"}}>
                    <StudyList studies={this.state.archivedStudies} openStudy={this.openStudy} />
                </div>
            </div>
        </div>);
    }

    componentDidMount() {
        sepes.getStudies(false).then(response => response.json())
            .then(json => {
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
        sepes.getStudies(true)
            .then(response => response.json())
            .then(json => {
                this.setState({archivedStudies: json});
            });
    }
}

export default CreateStudyPage;
