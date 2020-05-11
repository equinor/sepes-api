import React, {Component} from 'react';
import SepesUserList from './SepesUserList';
import SepesDataList from './SepesDataList';
import SepesPodList from './SepesPodList';

import spinner from "../spinner.svg";

import * as StudyService from "../studyService";
import Sepes from '../sepes.js';


const sepes = new Sepes();

class CreateStudyPage extends Component {
    constructor(props) {
        super(props);
        this.state = {
            pods: [],
            sponsors: [],
            suppliers: [],
            dataset: [],
            archived: false,
            studyId: null,
            studyName: "",

            data: [],
        }
    }

    render() {
        let appstate = this.props.state;
        // disables saving based on study id
        let disableSave = false;
        appstate.savingStudyIds.forEach(id => {
            if (id === this.state.studyId) disableSave = true;
        });

        return (
        <div>
            <header>
                <span><b>
                    <span className="link" onClick={() => this.props.changePage("studies")}>Studies</span> > </b>
                </span>
                <input type="text" placeholder="Study name" id="new-study-input" value={this.state.studyName} onChange={(e)=> this.setState({studyName: e.target.value})} />
                <button disabled={disableSave} onClick={this.saveStudy}>Save</button>
                { disableSave ? <img src={spinner} className="spinner" alt="" /> : null }
                <span className="loggedInUser">Logged in as <b>{ appstate.userName }</b></span>
            </header>
            <div className="sidebar">
                <div style={{padding: "20px"}}>
                    <label><input type="checkbox" checked={this.state.archived} onChange={this.updateAchived} />
                        Archive study
                    </label>
                </div>
                <SepesUserList header="Sponsors" data={this.state.sponsors} addItem={this.addSponsors} removeUser={this.removeSponsor} />
                <SepesUserList header="Suppliers" data={this.state.suppliers} addItem={this.addSuppliers} removeUser={this.removeSupplier} />
                <SepesDataList header="Dataset" data={this.state.data} addItem={this.addDataset} removeItem={this.removeDataset}/>
            </div>
            
            { typeof this.state.studyId === "number" ?  <SepesPodList className="podList" data={this.state.pods} newPod={this.newPod} openPod={this.openPod}/> : null}
        </div>);
    }

    componentDidMount() {
        let study = this.props.state.selectedStudy;

        if (study.studyId !== null) {
            this.setState({
                studyName: study.studyName,
                studyId: study.studyId,
                archived: study.archived,
                pods: study.pods,
                sponsors: study.sponsors,
                suppliers: study.suppliers
            });
        }
    }

    addSponsors = (user) => {
        this.setState({
            sponsors: StudyService.addUser(user, this.state.sponsors)
        });
    }

    addSuppliers = (user) => {
        this.setState({
            suppliers: StudyService.addUser(user, this.state.suppliers)
        });
    }

    removeSponsor = (user) => {
        this.setState({
            sponsors: StudyService.removeUser(user, this.state.sponsors)
        });
    }

    removeSupplier = (user) => {
        // Currently unable to remove users in Azure
        /*this.setState({
            suppliers: StudyService.removeUser(user, this.state.suppliers)
        });*/
    }

    addDataset = (dataset) => {
        this.setState({
            dataset: StudyService.addDataset(dataset)
        });
    }

    removeDataset = (dataset) => {
        this.setState({
            dataset: StudyService.removeDataset(dataset)
        });
    }

    newPod = () => {
        this.props.changePage("pod", {dataset: this.state.dataset, pod: {podId: null}});
    }

    openPod = (pod) => {
        this.props.changePage("pod", {dataset: this.state.dataset, pod});
    }

    updateAchived = () => {
        this.setState({archived: !this.state.archived});
    }

    saveStudy = () => {
        let props = this.props;
        let state = this.state;

        // create new or updated study object
        let study = {
            studyName: state.studyName,
            pods: state.pods,
            archived: state.archived,
            sponsors: state.sponsors,
            suppliers: state.suppliers
        }
        if (state.studyId !== null) {
            study.studyId = state.studyId;
        }
        
        // create base study based on selected study
        let based = this.state.studyId === null ? null : this.props.state.selectedStudy;
        
        // disable save button
        props.setSavingState(state.studyId);

        // save study
        sepes.saveStudy(study, based)
            .then(returnValue => returnValue.json())
            .then(returnedStudy => {
                // Set state 
                if (this.state.studyId === null) {
                    this.setState({studyId: returnedStudy.studyId});
                }

                // Update base study
                props.updateStudy(returnedStudy);

                // Enable saving
                props.removeSavingState(study.studyId);
            })
            .catch(() => {
                props.removeSavingState(study.studyId);
            });
    }
}

export default CreateStudyPage;
