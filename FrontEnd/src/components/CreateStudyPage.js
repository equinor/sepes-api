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
                <SepesDataList header="Dataset" data={sepes.getDatasetList()} addItem={sepes.addItemToStudy} removeItem={sepes.removeItemFromStudy}/>
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

    newPod = () => {
        this.props.changePage("pod");
    }
}

export default CreateStudyPage;
