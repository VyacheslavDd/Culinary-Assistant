import { Navigate, useLocation } from 'react-router-dom';
import { useSelector } from '../store/store';
import { selectIsAuthChecked, selectUser } from '../store/user.slice';
import { Preloader } from '../components/preloader';

type ProtectedRouteProps = {
    onlyUnAuth?: boolean;
    children: React.ReactElement;
};

export const ProtectedRoute = ({
    onlyUnAuth,
    children,
}: ProtectedRouteProps) => {
    const isAuthChecked = useSelector(selectIsAuthChecked);
    const user = useSelector(selectUser);
    const location = useLocation();

    if (!isAuthChecked) {
        return <Preloader />;
    }

    if (!onlyUnAuth && !user) {
        return <Navigate replace to='/login' state={{ from: location }} />;
    }

    if (onlyUnAuth && user) {
        const from = location.state?.from || { pathname: '/' };

        return <Navigate replace to={from} />;
    }

    return children;
};
