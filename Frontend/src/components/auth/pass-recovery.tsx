import styles from './auth.module.scss';

export function PassRecovery() {
    return (
        <div className={styles.mainContainer}>
            <div className={styles.container}>
                <div className={styles.header}>
                    <h1 className={styles.title}>Восстановление пароля</h1>
                    <h2 className={styles.subtitle}>
                        Введите адрес электронной почты, на которую будет
                        отправлено письмо для сброса
                    </h2>
                </div>
                <div className={styles.form}>
                    <div className={styles.inputContainer}>
                        <input
                            type='text'
                            placeholder='Email'
                            className={styles.input}
                        />
                        <span className={`${styles.email} ${styles.icon}`} />
                    </div>
                </div>
                <div className={styles.buttons}>
                    <button type='submit' className={styles.button}>
                        Отправить письмо
                    </button>
                </div>
            </div>
        </div>
    );
}
