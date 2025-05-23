import { Login, LayoutAuth } from 'components/auth';
import { Helmet } from 'react-helmet-async';

function LoginPage() {
    return (
        <LayoutAuth>
            <Helmet>
                <title>Вход в аккаунт</title>
            </Helmet>
            <Login />
        </LayoutAuth>
    );
}

export default LoginPage;
