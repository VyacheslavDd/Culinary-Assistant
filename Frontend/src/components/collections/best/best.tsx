import { CardCollMin } from '../card-min/card-coll-min';
import styles from './best.module.scss';

export function Best() {
    return (
        <div className={styles.mainContainer}>
            <div className={styles.container}>
                <div className={styles.header}>
                    <h1 className={styles.title}>Лучшие подборки</h1>
                    <p className={styles.subtitle}>
                        Самые популярные среди нашего сообщества подборки
                    </p>
                </div>
                <div className={styles.content}>
                    <ul className={styles.list}>
                        <li className={styles.item}>
                            <CardCollMin theme='light' />
                        </li>
                        <li className={styles.item}>
                            <CardCollMin theme='light' />
                        </li>
                        <li className={styles.item}>
                            <CardCollMin theme='light' />
                        </li>
                    </ul>
                    <div className={styles.buttons}>
                        <button className={styles.button}>&lt;</button>
                        <button className={styles.button}>&gt;</button>
                    </div>
                </div>
            </div>
        </div>
    );
}
