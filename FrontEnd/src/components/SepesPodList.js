import React from 'react';

import addSymbol from '../plus1.svg';

export default function SepesPodList(props) {
    return <div className="podlist">
        <div className="pod" onClick={props.newPod}>
            <p style={{ fontWeight: "bold" }}>New Pod</p>
            <p><img src={addSymbol} alt={"+"} style={{ width: 60 }} /></p>
        </div>
        {
            props.data.map((item) => (
                <div key={item.podId} className="pod" onClick={() => props.openPod(item)}>
                    <p style={{ fontWeight: "bold" }}>{item.podName}</p>
                    <div style={{ fontSize: 10 }}>
                        Incoming
                    {item.incoming.map(rule => (
                            <div key={rule.port+rule.ip}> {rule.port} {rule.ip}</div>
                        ))}</div>
                    <div style={{ fontSize: 10 }}>
                        Outgoing
                    {item.outgoing.map(rule => (
                            <div key={rule.port+rule.ip}> {rule.port} {rule.ip}</div>
                        ))}</div>
                </div>
            ))}
    </div>
}
