import styles from './auth.module.scss';

export function Register() {
    return (
        <div className={styles.mainContainer}>
            <div className={styles.container}>
                <div className={styles.header}>
                    <h1 className={styles.title}>Регистрация</h1>
                    <h2 className={styles.subtitle}>
                        Мы будем очень рады видеть Вас в нашем сообществе!
                    </h2>
                </div>
                <div className={styles.form}>
                    <div className={styles.inputContainer}>
                        <input
                            type='text'
                            placeholder='Логин'
                            className={styles.input}
                        />
                        <span className={`${styles.person} ${styles.icon}`} />
                        <p className={`${styles.error_text} ${styles.active}`}>
                            <span
                                className={styles.error}
                            />
                            Поле не может быть пустым.
                        </p>
                    </div>
                    <div className={styles.inputContainer}>
                        <input
                            type='password'
                            placeholder='Email/телефон'
                            className={styles.input}
                        />
                        <span className={`${styles.email} ${styles.icon}`} />
                        <p className={`${styles.error_text} ${styles.active}`}>
                            <span
                                className={styles.error}
                            />
                            Поле не может быть пустым.
                        </p>
                    </div>
                    <div className={styles.inputContainer}>
                        <input
                            type='password'
                            placeholder='Пароль'
                            className={styles.input}
                        />
                        <span className={`${styles.lock} ${styles.icon}`} />
                        <p className={`${styles.error_text} ${styles.active}`}>
                            <span
                                className={styles.error}
                            />
                            Поле не может быть пустым.
                        </p>
                    </div>
                    <div className={styles.inputContainer}>
                        <input
                            type='password'
                            placeholder='Подтвердите пароль'
                            className={styles.input}
                        />
                        <span className={`${styles.lock} ${styles.icon}`} />
                        <p className={`${styles.error_text} ${styles.active}`}>
                            <span
                                className={styles.error}
                            />
                            Поле не может быть пустым.
                        </p>
                    </div>
                </div>
                <div className={styles.buttons}>
                    <button type='submit' className={styles.button}>
                        Зарегистрироваться
                    </button>
                    <p>
                        Уже есть аккаунт?{' '}
                        <a href='/' className={styles.link}>
                            Войти
                        </a>
                    </p>
                </div>
            </div>
        </div>
    );
}
