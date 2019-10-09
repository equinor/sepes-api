import React, {Component} from 'react'

export default class PodRules extends Component  {
    constructor(props) {
        super(props);
        this.state = {
            port: "",
            ip: "",
        }
    }

    add = () => {
        this.props.addItem(this.state.port, this.state.ip);
        this.setState({
            port: "",
        });
    }


    render() {
        return <div> 
            <div className="study-head">
                { this.props.header }
            </div>
            <div style={{padding: 20}}>
                    <table>
                        <thead>
                            <tr>
                                <th>Port</th>
                                <th>IP address</th>
                                <th>Delete</th>
                            </tr>
                        </thead>
                        <tbody>
                        { this.props.data.map((item) => (
                            <tr key={item.port+""+item.ip}>
                                <td>{item.port}</td>
                                <td>{item.ip}</td>
                                <td>x</td>
                            </tr>
                        )) }
                        </tbody>
                    </table>

            <div style={{paddingTop: 20}}>
                    <input type="text" placeholder="Port" value={this.state.port} onChange={(e) => this.setState({port: e.target.value})} />
                    <input type="text" placeholder="IP address" value={this.state.ip} onChange={(e) => this.setState({ip: e.target.value})} />
                    <button onClick={this.add}>Add</button>
                </div>
        </div>
    </div>
    }
}
