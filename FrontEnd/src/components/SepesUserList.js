import React from 'react';

export default function SepesUserList(props) {
    return <div> {
        props.data.map((item) => (
            <div key={item.UserId}><label><input type="checkbox" name="users" value={ item.UserId } />{ `${item.UserName} (${item.UserEmail})`}</label></div>
        )) }
    </div>
}