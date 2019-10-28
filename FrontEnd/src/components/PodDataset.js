import React from 'react';

export default function PodDataset(props) {
    return  <div className="dataset"> 
                <p className="dataset-header">Dataset: {props.header}</p>
                <p>Connection info: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras blandit vestibulum urna, eget elementum quam porttitor laoreet. 
                    Vestibulum egestas ipsum nec urna auctor, in vulputate mi sodales. Donec vitae sapien eget tellus scelerisque condimentum.</p>
                <div className="dataset-buttons">
                    <button>Load</button>
                    <button>Lock</button>
                    <button>Unlock</button>
                </div>
            </div>
}
