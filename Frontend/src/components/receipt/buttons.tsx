import styles from './buttons.module.scss';

export function ButtonsReceipt() {
    return (
        <div className={styles.mainContainer}>
            <button className={`button ${styles.buttonShare}`}>
                <span className={`${styles.icon} ${styles.like}`}></span>
                Нравится
            </button>
            <button className={`button ${styles.buttonShare}`}>
                <span className={`${styles.icon} ${styles.share}`}></span>
                Поделиться
            </button>
            <button className={styles.buttonAdd}>
                <span className={`${styles.icon} ${styles.add}`}></span>Добавить
                в подборку{' '}
            </button>
        </div>
    );
}
