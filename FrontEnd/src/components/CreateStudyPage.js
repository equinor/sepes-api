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
        }
    }
    render() {
        return (
        <div>
            <header>
                <span><b>
                    <span className="link" onClick={() => this.props.changePage("studies")}>Sepes</span> > </b>
                </span>
                <input type="text" placeholder="Study name" id="new-study-input"/>
                <button>Save</button>
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
                <SepesDataList header="Dataset" data={sepes.getDatasetList()} addItem={this.addDataset} removeItem={this.removeDataset}/>
            </div>
            <SepesPodList data={this.state.pods} newPod={this.newPod} />
        </div>);
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
            dataset: [...this.state.dataset, dataset]
        });
    }

    removeDataset = (dataset) => {
        let newArray = [...this.state.dataset];
        newArray.splice(newArray.indexOf(dataset), 1)
        this.setState({
            dataset: newArray
        });
    }

    newPod = () => {
        this.props.changePage("pod", {dataset: this.state.dataset});
    }


    updateAchived = () => {
        this.setState({archived: !this.state.archived})
        sepes.updateStudy(this.state.studyId, this.state.archived);
    }
}

export default CreateStudyPage;
