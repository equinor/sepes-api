import React, {Component} from 'react'
import SepesUserList from './SepesUserList'
import SepesDataList from './SepesDataList'
import SepesPodList from './SepesPodList'

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
        return (
        <div>
            <header>
                <span><b>
                    <span className="link" onClick={() => this.props.changePage("studies")}>Sepes</span> > </b>
                </span>
                <input type="text" placeholder="Study name" id="new-study-input" value={this.state.studyName} onChange={(e)=> this.setState({studyName: e.target.value})} />
                { this.state.studyId === null ? <button onClick={this.saveStudy}>Save</button> : null }
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
            <SepesPodList data={this.state.pods} newPod={this.newPod} />
        </div>);
    }

    componentDidMount() {
        let study = this.props.state.selectedStudy;
        console.log(study.StudyName)
        this.setState({
            studyName: study.StudyName,
            studyId: typeof(study.StudyId) === "undefined" || study.StudyId === null ? null : study.StudyId,
            archived: typeof(study.Archived) === "undefined" || study.Archived === false ? false : true
        });

        sepes.initStudy();
        sepes.getData().then(response => response.json())
            .then(json => {
                console.log("fetch dataset");
                console.log(json);
                this.setState({data: json});
            });
        
        /*console.log("get pods for study "+study.StudyId);
        if (typeof study.StudyId === 'number') {
            console.log("get pods for study "+study.StudyId);
            sepes.getPods(study.StudyId)
                .then(response => response.json())
                .then(pods => {
                    console.log("fetch pods");
                    console.log(pods);
                    if (pods !== null && typeof(pods) !== "undefined" && pods !== "undefined") {
                        this.setState({pods});
                    }
                });
        }*/
    }

    addSponsors = (user) => {
        this.setState({
            sponsors: [...this.state.sponsors, user]
        });
    }

    addSuppliers = (user) => {
        this.setState({
            suppliers: [...this.state.suppliers, user]
        });
    }

    removeSponsor = (user) => {
        let index = this.state.sponsors.indexOf(user);
        let newArray = [...this.state.sponsors];
        newArray.splice(index, 1);
        this.setState({
            sponsors: newArray
        });
    }

    removeSupplier = (user) => {
        let index = this.state.suppliers.indexOf(user);
        let newArray = [...this.state.suppliers];
        newArray.splice(index, 1);
        this.setState({
            suppliers: newArray
        });
    }

    addDataset = (dataset) => {
        this.setState({
            dataset: [...this.state.dataset, dataset.DatasetName]
        });

        // update this
        sepes.addItemToStudy(dataset.DatasetId, "datasetIds");
    }

    removeDataset = (dataset) => {
        let newArray = [...this.state.dataset];
        newArray.splice(newArray.indexOf(dataset), 1)
        this.setState({
            dataset: newArray
        });

        // update this
        sepes.removeItemFromStudy(dataset.DatasetId, "datasetIds");
    }

    newPod = () => {
        this.props.changePage("pod", {dataset: this.state.dataset});
    }


    updateAchived = () => {
        if (this.state.studyId !== null) {
            let archive = !this.state.archived;
            this.setState({archived: archive});
            sepes.updateStudy(this.state.studyId, archive);
        }
    }

    saveStudy = () => {
        sepes.setStudyName(this.state.studyName);
        sepes.createStudy()
            .then(returnValue => returnValue.text())
            .then(id => {
                if (parseInt(id) !== -1) {
                    this.setState({studyId: parseInt(id)});
                }
            });
    }
}

export default CreateStudyPage;
