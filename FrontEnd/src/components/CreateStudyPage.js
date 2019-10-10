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
        }
    }
    render() {
        return (
        <div>
            <header>
                <span><b>
                    <span onClick={() => this.props.changePage("studies")}>Sepes</span> > </b>
                </span>
                <input type="text" placeholder="Study name" id="new-study-input"/>
                <button>Save</button>
                <span className="loggedInUser">Logged in as <b>{ this.props.state.userName }</b></span>
            </header>
            <div className="sidebar">
                <SepesUserList header="Sponsors" data={this.state.sponsors} addItem={this.addSponsors} />
                <SepesUserList header="Suppliers" data={this.state.suppliers} addItem={this.addSuppliers} />
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
}

export default CreateStudyPage;
