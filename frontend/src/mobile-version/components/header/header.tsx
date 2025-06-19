import styles from './header.module.scss';
import { NavLink, useLocation, useNavigate } from 'react-router-dom';
import { selectIsAuthenticated } from 'store/user.slice';
import { useSelector } from 'store/store';
import { ReactComponent as CollectionsIcon } from '../../../assets/svg/collections_mobile.svg';
import { ReactComponent as CollectionsActiveIcon } from '../../../assets/svg/collections_active_mobile.svg';
import { ReactComponent as HomeIcon } from '../../../assets/svg/home_mobile.svg';
import { ReactComponent as HomeActiveIcon } from '../../../assets/svg/home_active_mobile.svg';
import { ReactComponent as UserIcon } from '../../../assets/svg/user_mobile.svg';
import { ReactComponent as UserActiveIcon } from '../../../assets/svg/user_active_mobile.svg';

function Header() {
    const isAuthenticated = useSelector(selectIsAuthenticated);
    const location = useLocation();

    const isActive = (path: string) => location.pathname === path;

    return (
        <nav className={styles.mobileNav}>
            <NavLink
                to='/collections'
                className={`${styles.navItem} ${
                    isActive('/collections') ? styles.active : ''
                }`}
            >
                {isActive('/collections') ? (
                    <>
                        <CollectionsActiveIcon className={styles.icon} />
                        <span className={styles.label}>Подборки</span>
                    </>
                ) : (
                    <CollectionsIcon className={styles.icon} />
                )}
            </NavLink>

            <NavLink
                to='/'
                className={`${styles.navItem} ${
                    isActive('/') ? styles.active : ''
                }`}
            >
                {isActive('/') ? (
                    <>
                        <HomeActiveIcon className={styles.icon} />
                        <span className={styles.label}>Главная</span>
                    </>
                ) : (
                    <HomeIcon className={styles.icon} />
                )}
            </NavLink>

            <NavLink
                to={isAuthenticated ? '/profile' : '/login'}
                className={`${styles.navItem} ${
                    isActive('/profile') ? styles.active : ''
                }`}
            >
                {isActive('/profile') ? (
                    <>
                        <UserActiveIcon className={styles.icon} />
                        <span className={styles.label}>Профиль</span>
                    </>
                ) : (
                    <UserIcon className={styles.icon} />
                )}
            </NavLink>
        </nav>
    );
}

export default Header;
