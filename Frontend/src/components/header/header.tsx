import styles from './header.module.scss';
import logo from '../../assets/svg/logo.svg';

function Header() {
    return (
        <header className={styles.header}>
            <img src={logo} alt='logo' className={styles.logo} />
            <nav>
                <ul className={styles.menu}>
                    <li>
                        <a href='#' className={styles.a}>Все рецепты</a>
                    </li>
                    <li>
                        <a href='#' className={styles.a}>Подборки рецептов</a>
                    </li>
                </ul>
            </nav>
            <button type='button' className={styles.button}>Войти</button>
        </header>
    );
}

export default Header;
