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
                    <div className="card" onClick={this.newStudy}>
                        <p style={{fontWeight: "bold"}}>New Study</p>
                        <p><img src={addSymbol} alt={"+"} style={{width: 60}}/></p>
                    </div>
                    <StudyList studies={this.state.studies} openStudy={this.openStudy} />
                </div>
                <div>
                    <button onClick={this.showArchived} id="show-archived">Show archived studies</button>
                </div>
                <div style={{paddingTop: 30, display: "table"}}>
                    <StudyList studies={this.state.archivedStudies} openStudy={this.openStudy} />
                </div>
            </div>
        </div>);
    }

    componentDidMount() {
        // get studies from backend
        sepes.getStudies(false).then(response => response.json())
            .then(json => {
                this.setState({studies: json});

                this.props.setStudies(json);
            });
    }

    newStudy = () => {
        // set current study as a new study with no id and no name
        this.props.setStudy({studyId: null, studyName: ""});
        
        this.props.changePage("study", {});
    }

    openStudy = (study) => {
        // set selected study as current study, then open the study page
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
