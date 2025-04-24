import './App.scss';
import Header from './components/header/header';
import Footer from 'components/footer/footer';
import LoginPage from 'pages/login/login.page';
import MainPage from 'pages/main/main.page';
import ReceiptPage from 'pages/receipt/receipt.page';
import CollectionsPage from 'pages/collections/collections.page';
import { Route, Routes, useLocation } from 'react-router-dom';
// import { ProtectedRoute } from 'utils/protected-route';
import { CollectionPage } from 'pages/collection/collection.page';
import RegisterPage from 'pages/register/register.page';
import PassRecoveryPage from 'pages/pass-recovery/pass-recovery.page';
import { ProfilePage } from 'pages/profile/profile.page';
import MyCollectionsPage from 'pages/my-collections/my-collection.page';

function App() {
    const location = useLocation();
    const { pathname } = location;

    const authPages = ['/login', '/register', '/pass-recovery'];
    const isAuthPage = authPages.includes(pathname);

    return (
        <div className='App'>
            {!isAuthPage && <Header />}
            <Routes location={location}>
                <Route path='/' element={<MainPage />} />
                <Route path='/receipt' element={<ReceiptPage />} />
                <Route path='/collections' element={<CollectionsPage />} />
                <Route path='/collection' element={<CollectionPage />} />
                <Route path='/profile' element={<ProfilePage />} />
                <Route path='/my-collection' element={<MyCollectionsPage />} />
                <Route
                    path='/login'
                    element={
                        // <ProtectedRoute onlyUnAuth>
                        <LoginPage />
                        // </ProtectedRoute>
                    }
                />
                <Route
                    path='/register'
                    element={
                        // <ProtectedRoute onlyUnAuth>
                        <RegisterPage />
                        // </ProtectedRoute>
                    }
                />
                <Route
                    path='/pass-recovery'
                    element={
                        // <ProtectedRoute onlyUnAuth>
                        <PassRecoveryPage />
                        // </ProtectedRoute>
                    }
                />
            </Routes>
            {!isAuthPage && <Footer />}
        </div>
    );
}

export default App;
