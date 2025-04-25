import { useNavigate } from 'react-router-dom';
import styles from './layout-header.module.scss';

type Props = {
    children: React.ReactNode;
};

export function LayoutHeader(props: Props) {
    const { children } = props;
    const navigate = useNavigate();

    const handleBack = () => {
        navigate(-1); 
    };

    return (
        <div className={styles.mainContainer}>
            <button onClick={handleBack} className={styles.back}>
                <span className={styles.arrow}></span>
                Назад
            </button>
            {children}
        </div>
    );
}