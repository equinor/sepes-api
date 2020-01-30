import React from 'react';

export default function StudyList(props) {
    return <React.Fragment>
        {
            props.studies.map((item) => (
                <div key={item.studyId} className="study" onClick={() => props.openStudy(item)}>
                    <p style={{fontWeight: "bold"}}>{item.studyName}</p>
                    <p>Pods: {item.pods.length}</p>
                    <p>Users: {item.suppliers.length}</p>
                </div>
            ))}
    </React.Fragment>
}
