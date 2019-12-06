import React, {Component} from 'react'
import SepesUserList from './SepesUserList'
import SepesDataList from './SepesDataList'
import SepesPodList from './SepesPodList'

import * as StudyService from "../studyService"

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
            saveBtnDisabled: false,
        }
    }

    render() {
        return (
        <div>
            <header>
                <span><b>
                    <span className="link" onClick={() => this.props.changePage("studies")}>Sepes</span> > </b>
                </span>
                <input type="text" placeholder="Study name" id="new-study-input" value={this.state.studyName} onChange={(e)=> this.setState({studyName: e.target.value})} />
                { this.state.studyId === null ? <button disabled={this.state.saveBtnDisabled} onClick={this.saveStudy}>Save</button> : null }
                <span className="loggedInUser">Logged in as <b>{ this.props.state.userName }</b></span>
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
        //let study = StudyService.getCurrentStudy();

        if (study.studyId !== null) {
            this.setState({
                studyName: study.studyName,
                studyId: typeof(study.studyId) === "undefined" || study.studyId === null ? null : study.studyId,
                archived: typeof(study.srchived) === "undefined" || study.archived === false ? false : true,
                pods: study.pods,
                sponsors: study.sponsors,
                suppliers: study.suppliers
            });
        }
        

        sepes.getData().then(response => response.json())
            .then(json => {
                console.log("fetch dataset");
                console.log(json);
                this.setState({data: json});
            });
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
        this.setState({
            suppliers: StudyService.removeUser(user, this.state.suppliers)
        });
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

    setPods = (pods) => {
        //if (pods !== null && typeof(pods) !== "undefined") {
            this.setState({pods});
        //}
    }

    updateAchived = () => {
        if (this.state.studyId !== null) {
            let archive = !this.state.archived;
            this.setState({archived: archive});
            sepes.updateStudy(this.state.studyId, archive);
        }
    }

    saveStudy = () => {
        this.setState({saveBtnDisabled: true});

        let state = this.state;
        let study = {
            studyName: state.studyName,
            userIds: [],
            datasetIds: state.dataset,
            archived: false,
            sponsors: state.sponsors,
            suppliers: state.suppliers
        }

        let based = this.state.studyId === null ? null : this.props.state.selectedStudy;
        
        sepes.createStudy(study, based)
            .then(returnValue => returnValue.json())
            .then(json => {
                this.setState({studyId: json.studyId});
                this.setState({saveBtnDisabled: false});
            })
            .catch(() => {
                this.setState({saveBtnDisabled: false});
            });
    }
}

export default CreateStudyPage;
