import React, {Component} from 'react'

//import Sepes from '../sepes.js';
//const sepes = new Sepes();

class CreateStudyPage extends Component {
    constructor(props) {
        super(props);
        this.state = {
            studies: [],
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
                <div className="study" onClick={this.newStudy}>
                    <p>Equinor test study</p>
                    <p>Pods: 2</p>
                    <p>Datasets: 5</p>
                </div>
                <div className="study" onClick={this.newStudy}>
                    <p>Brilliant study</p>
                    <p>Pods: 1</p>
                    <p>Datasets: 1</p>
                </div>
                <div className="study" onClick={this.newStudy}>
                    <p>New Study</p>
                </div>
            </div>
        </div>);
    }

    newStudy = () => {
        this.props.changePage("study");
    }
}

export default CreateStudyPage;
