import React from 'react';

export default function SepesPodList(props) {
    return <div className="podlist"> {
        props.data.map((item) => (
            <div key={item.PodId} className="pod">
                { item.PodName }
            </div>
        )) }
        <div className="pod" onClick={props.newPod}>
            New Pod
        </div>
    </div>
}
