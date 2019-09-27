import React, {Component} from 'react'

class SepesUserList extends Component {
    addRemove = (e, id) => {
        if (e.target.checked) {
            this.props.addItem(id, "userIds");
        }
        else {
            this.props.removeItem(id, "userIds");
        }
    }

    render() {
        return (
        <div> {
            this.props.data.map((item) => (
                <div key={item.UserId}>
                    <label>
                        <input type="checkbox" name="users" value={ item.UserId } onChange={(e) => this.addRemove(e, item.UserId)} />
                        { `${item.UserName} (${item.UserEmail})`}
                    </label>
                </div>
            )) }
        </div>);
    }
}

export default SepesUserList;
