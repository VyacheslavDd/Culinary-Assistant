import './App.scss';
import Header from './components/header/header';
import Footer from 'components/footer/footer';
import LoginPage from 'pages/login/login.page';
import MainPage from 'pages/main/main.page';
import ReceiptPage from 'pages/receipt/receipt.page';
import CollectionsPage from 'pages/collections/collections.page';
import { Outlet, Route, Routes, useLocation } from 'react-router-dom';
// import { ProtectedRoute } from 'utils/protected-route';
import { CollectionPage } from 'pages/collection/collection.page';
import RegisterPage from 'pages/register/register.page';
import PassRecoveryPage from 'pages/pass-recovery/pass-recovery.page';
import { ProfilePage } from 'pages/profile/profile.page';
import MyCollectionsPage from 'pages/my-collections/my-collection.page';
import { useEffect } from 'react';
import { useDispatch } from 'store/store';
import { fetchRecipes } from 'store/main-page.slice';
import { ProtectedRoute } from 'utils/protected-route';
import { checkUser } from 'store/user.slice';
import MyRecipesPage from 'pages/my-recipes/my-recipes.page';

function App() {
    const location = useLocation();
    const dispatch = useDispatch();
    const { pathname } = location;

    const authPages = ['/login', '/register', '/pass-recovery'];
    const isAuthPage = authPages.includes(pathname);

    useEffect(() => {
        dispatch(checkUser());
    }, [dispatch]);

    return (
        <div className='App'>
            {!isAuthPage && <Header />}
            <Routes location={location}>
                <Route path='/' element={<MainPage />} />
                <Route path='/recipe/:id' element={<ReceiptPage />} />
                <Route path='/collections' element={<CollectionsPage />} />
                <Route path='/collection/:id' element={<CollectionPage />} />
                <Route
                    path='/profile'
                    element={
                        <ProtectedRoute>
                            <Outlet />
                        </ProtectedRoute>
                    }
                >
                    <Route index element={<ProfilePage />} />
                    <Route path='my-recipes' element={<MyRecipesPage />} />
                    <Route
                        path='my-collections'
                        element={<MyCollectionsPage />}
                    />
                </Route>
                <Route
                    path='/login'
                    element={
                        <ProtectedRoute onlyUnAuth>
                            <LoginPage />
                        </ProtectedRoute>
                    }
                />
                <Route
                    path='/register'
                    element={
                        <ProtectedRoute onlyUnAuth>
                            <RegisterPage />
                        </ProtectedRoute>
                    }
                />
                <Route
                    path='/pass-recovery'
                    element={
                        <ProtectedRoute onlyUnAuth>
                            <PassRecoveryPage />
                        </ProtectedRoute>
                    }
                />
            </Routes>
            {!isAuthPage && <Footer />}
        </div>
    );
}

export default App;
