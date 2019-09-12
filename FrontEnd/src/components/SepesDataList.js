import React from 'react';

export default function SepesDataList(props) {
    return <div> {
        props.data.map((item) => (
            <div><label><input type="checkbox" name={ Object.keys(props) } value={ item } />{ item }</label></div>
        )) }
    </div>
}