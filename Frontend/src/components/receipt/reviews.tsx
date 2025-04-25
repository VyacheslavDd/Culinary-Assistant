import styles from './reviews.module.scss';
import photo from '../../assets/img/Photo.png';

export function Reviews() {
    return (
        <div className={styles.mainContainer}>
            <div className={styles.container}>
                <p className={styles.title}>Все отзывы</p>
                <p className={styles.count}>8</p>
            </div>
            <div className={styles.reviews}>
                {Array.from({ length: 8 }).map((_, index) => (
                    <div className={styles.card} key={index}>
                    <p className={styles.rating}>10/10</p>
                    <p className={styles.text}>
                    Очень вкусные блины получились, буду готовить еще!
                    </p>
                    <div className={styles.info}>
                        <a className={styles.name} href='#'>
                            <img
                                src={photo}
                                alt='person'
                                className={styles.img}
                            />
                            <p>Анна А.</p>
                        </a>
                        <p className={styles.date}>16 дек 2024</p>
                    </div>
                </div>
                ))}
                
            </div>
        </div>
    );
}
