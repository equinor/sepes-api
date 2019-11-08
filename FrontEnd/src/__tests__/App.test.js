import React from 'react';
import ReactDOM from 'react-dom';
import { act } from "react-dom/test-utils";
import App from '../App';


let div = null;

beforeEach(() => {
  // setup a DOM element as a render target
  div = document.createElement("div");
  document.body.appendChild(div);
});


it('renders without crashing', () => {
  ReactDOM.render(<App noLogin={true} />, div);
  ReactDOM.unmountComponentAtNode(div);
});  
