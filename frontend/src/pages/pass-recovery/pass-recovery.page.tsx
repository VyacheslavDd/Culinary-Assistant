import { LayoutAuth, PassRecovery } from 'components/auth';
import { Helmet } from 'react-helmet-async';

function PassRecoveryPage() {
    return (
        <LayoutAuth>
            <Helmet>
                <title>Восстановление пароля</title>
            </Helmet>
            <PassRecovery />
        </LayoutAuth>
    );
}

export default PassRecoveryPage;
