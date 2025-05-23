import { NavLink } from 'react-router-dom';
import styles from './layout-auth.module.scss';

export function LayoutAuth({ children }: { children: React.ReactNode }) {
    return (
        <div className={styles.mainContainer}>
            <div className={styles.container}>
                <NavLink to='/' className={styles.back}>
                    <span className={styles.arrow}></span>
                    На главную
                </NavLink>
                {children}
            </div>
        </div>
    );
}
