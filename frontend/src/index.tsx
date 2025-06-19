import React from 'react';
import ReactDOM from 'react-dom/client';
import { HelmetProvider } from 'react-helmet-async';
import { Provider } from 'react-redux';
import { BrowserRouter } from 'react-router-dom';
import './index.scss';
import AppWrapper from './app-wrapper';
import store from 'store/store';

const root = ReactDOM.createRoot(
    document.getElementById('root') as HTMLElement
);
root.render(
    <React.StrictMode>
        <HelmetProvider>
            <Provider store={store}>
                <BrowserRouter>
                    <AppWrapper />
                </BrowserRouter>
            </Provider>
        </HelmetProvider>
    </React.StrictMode>
);
