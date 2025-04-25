import styles from './best.module.scss';
import left from '../../../assets/svg/button_left.svg';
import right from '../../../assets/svg/button_right.svg';
import { ListMinCollections } from '../list-min/list-min';

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
                    <ListMinCollections theme='light' />
                    <div className={styles.buttons}>
                        <button className={styles.button}>
                            <img src={left} alt='left' />
                        </button>
                        <button className={styles.button}>
                            <img src={right} alt='right' />
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}
