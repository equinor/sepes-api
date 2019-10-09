import React from 'react';

export default function SepesDataList(props) {
    var addRemove = (e, id) => {
        if (e.target.checked) {
            props.addItem(id, "datasetIds");
        }
        else {
            props.removeItem(id, "datasetIds");
        }
    }
    return <div> 
            <div className="study-head">
                { props.header }
            </div>{
        props.data.map((item) => (
            <div key={item.DatasetId}><label>
                <input type="checkbox" name="dataset" value={ item.DatasetId } onChange={(e) => addRemove(e, item.DatasetId)}/>
                { item.DatasetName }
                </label>
            </div>
        )) }
    </div>
}
