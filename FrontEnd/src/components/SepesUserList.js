import React, {Component} from 'react'

class SepesUserList extends Component {
    state = {
        email: "",
    }

    add = () => {
        this.props.addItem(this.state.email);
        this.setState({email: ""});
    }

    render() {
        return (
        <div className="sidebar-block"> 
            <div className="study-head">
                { this.props.header }
            </div>
            <div className="email-list">
            { this.props.data.map((item) => (
                <div key={item}>
                    { item }
                </div>
            )) }</div>
            <div className="add-email">
                <input type="text" placeholder="email address" value={this.state.email} onChange={(e) => this.setState({email: e.target.value})} />
                <button onClick={this.add}>Add</button>
            </div>
        </div>);
    }
}

export default SepesUserList;
