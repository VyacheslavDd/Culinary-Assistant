import styles from './auth.module.scss';
import { NavLink } from 'react-router-dom';

export function Login() {
    return (
        <div className={styles.mainContainer}>
            <div className={styles.container}>
                <div className={styles.header}>
                    <h1 className={styles.title}>Вход</h1>
                    <h2 className={styles.subtitle}>
                        Рады, что Вы снова с нами!
                    </h2>
                </div>
                <div className={styles.form}>
                    <div className={styles.inputContainer}>
                        <input
                            autoComplete='off'
                            type='text'
                            placeholder='Логин'
                            className={styles.input}
                        />
                        <span className={`${styles.email} ${styles.icon}`} />
                        <p className={`${styles.error_text} ${styles.active}`}>
                            <span className={styles.error} />
                            Поле не может быть пустым.
                        </p>
                    </div>
                    <div className={styles.inputContainer}>
                        <input
                            autoComplete='off'
                            type='password'
                            placeholder='Пароль'
                            className={styles.input}
                        />
                        <span className={`${styles.lock} ${styles.icon}`} />
                        <p className={`${styles.error_text} ${styles.active}`}>
                            <span className={styles.error} />
                            Поле не может быть пустым.
                        </p>
                    </div>
                </div>
                <div className={styles.buttons}>
                    <button type='submit' className={styles.button}>
                        Войти
                    </button>
                    <p>
                        Забыли пароль?{' '}
                        <NavLink to='/pass-recovery' className={styles.link}>
                            Восстановить пароль
                        </NavLink>
                    </p>
                    <p>
                        Нет аккаунта?{' '}
                        <NavLink to='/register' className={styles.link}>
                            Зарегистрироваться
                        </NavLink>
                    </p>
                </div>
            </div>
        </div>
    );
}
