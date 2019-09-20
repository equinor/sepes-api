import React from 'react';

export default function SepesDataList(props) {
    return <div> {
        props.data.map((item) => (
            <div key={item.DatasetID}><label><input type="checkbox" name="dataset" value={ item.DatasetID } />{ item.DatasetName }</label></div>
        )) }
    </div>
}