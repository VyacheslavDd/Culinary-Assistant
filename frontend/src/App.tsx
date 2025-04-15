// import Header from 'components/header/header';
import './App.scss';
import Header from './components/header/header';
// import MainPage from 'pages/main/main';
// import Footer from 'components/footer/footer';
import { LayoutAuth } from 'components/auth/layout-auth';
import { Login } from 'components/auth/login';
import Footer from 'components/footer/footer';
import MainPage from 'pages/main/main.page';
import ReceiptPage from 'pages/receipt/receipt.page';

function App() {
    return (
        <div className='App'>
            {/* <MainPage /> */}
            <Header />
            <ReceiptPage />
            <Footer />
        </div>
    );
}

export default App;
