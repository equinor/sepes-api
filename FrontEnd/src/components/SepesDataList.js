import React from 'react';

export default function SepesDataList(props) {
    var demoAddRemove = (e, id) => {
        if (e.target.checked) {
            props.addItem(id);
        }
        else {
            props.removeItem(id);
        }
    }

    return <div> 
            <div className="study-head">
                { props.header }
            </div>{
        props.data.map((item) => (
            <div key={item.DatasetId}><label>
                <input type="checkbox" name="dataset" value={ item.DatasetId } onChange={(e) => demoAddRemove(e, item)}/>
                { item.DatasetName }
                </label>
            </div>
        )) }
    </div>
}
