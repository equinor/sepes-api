import React from 'react';

export default function SepesDataList(props) {
    return <div> {
        props.data.map((item) => (
            <div key={item.DatasetId}><label><input type="checkbox" name="dataset" value={ item.DatasetId } />{ item.DatasetName }</label></div>
        )) }
    </div>
}