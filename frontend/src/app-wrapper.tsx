import { useState, useEffect } from 'react';
import App from './App';
import AppMobile from 'mobile-version/App';

const AppWrapper = () => {
    const [isMobile, setIsMobile] = useState(window.innerWidth < 768);

    useEffect(() => {
        const handleResize = () => {
            setIsMobile(window.innerWidth < 768);
        };

        window.addEventListener('resize', handleResize);
        return () => window.removeEventListener('resize', handleResize);
    }, []);

    return isMobile ? <AppMobile /> : <App />;
};

export default AppWrapper;