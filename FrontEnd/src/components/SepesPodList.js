import React from 'react';

import addSymbol from '../plus1.svg';

export default function SepesPodList(props) {
    return <div className="podlist">
        <div className="pod" onClick={props.newPod}>
            <p style={{ fontWeight: "bold" }}>New Pod</p>
            <p><img src={addSymbol} style={{ width: 60 }} /></p>
        </div>
        {
            props.data.map((item) => (
                <div key={item.podId} className="pod" onClick={() => props.openPod(item)}>
                    <p style={{ fontWeight: "bold" }}>{item.podName}</p>
                    <p style={{ fontSize: 10 }}>
                        Incoming
                    {item.incoming.map(rule => (
                            <div>{rule.port} {rule.ip}</div>
                        ))}</p>
                    <p style={{ fontSize: 10 }}>
                        Outgoing
                    {item.outgoing.map(rule => (
                            <div>{rule.port} {rule.ip}</div>
                        ))}</p>
                </div>
            ))}
    </div>
}
