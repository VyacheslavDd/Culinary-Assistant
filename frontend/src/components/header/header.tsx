import styles from './header.module.scss';
import logo from '../../assets/svg/logo.svg';
import { NavLink, useNavigate } from 'react-router-dom';
import { selectIsAuthenticated } from 'store/user.slice';
import { useSelector } from 'store/store';

function Header() {
    const navigate = useNavigate();
    const isAuthenticated = useSelector(selectIsAuthenticated);

    return (
        <header className={styles.header}>
            <NavLink to='/' className={styles.logoLink}>
                <img src={logo} alt='logo' className={styles.logo} />
            </NavLink>

            <nav>
                <ul className={styles.menu}>
                    <li>
                        <NavLink to='/' className={styles.a}>
                            Все рецепты
                        </NavLink>
                    </li>
                    <li>
                        <NavLink to='/collections' className={styles.a}>
                            Подборки рецептов
                        </NavLink>
                    </li>
                </ul>
            </nav>
            {isAuthenticated ? (
                <button
                    type='button'
                    className={styles.button}
                    onClick={() => navigate('/profile')}
                >
                    Мой профиль
                </button>
            ) : (
                <button
                    type='button'
                    className={styles.button}
                    onClick={() => navigate('/login')}
                >
                    Войти
                </button>
            )}
            {/* <button
                type='button'
                className={styles.button}
                onClick={() => navigate('/login')}
            >
                Войти
            </button> */}
        </header>
    );
}

export default Header;
