import { LayoutAuth, Register } from 'components/auth';
import { Helmet } from 'react-helmet-async';

function RegisterPage() {
    return (
        <LayoutAuth>
            <Helmet>
                <title>Регистрация</title>
            </Helmet>
            <Register />
        </LayoutAuth>
    );
}

export default RegisterPage;
