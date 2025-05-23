import { ReactNode } from 'react';
import { useSelector } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import { selectIsAuthenticated } from 'store/user.slice';

type ButtonWrapperProps = {
    onAuthenticatedAction: () => void;
    children: ReactNode;
};

export function ButtonWrapper({
    onAuthenticatedAction,
    children,
}: ButtonWrapperProps) {
    const isAuthenticated = useSelector(selectIsAuthenticated);
    const navigate = useNavigate();

    const handleClick = (e: React.MouseEvent) => {
        e.stopPropagation();
        if (!isAuthenticated) {
            navigate('/login');
        } else {
            onAuthenticatedAction();
        }
    };

    return (
        <button
            style={{ backgroundColor: 'transparent', border: 'none' }}
            onClick={handleClick}
        >
            {children}
        </button>
    );
}
