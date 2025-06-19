import styles from './reviews.module.scss';
import photo from '../../../assets/img/Photo.png';
import { Feedback } from 'types';
import { transformUpdatedAt } from 'utils/transform';

type props = {
    feedbacks: Feedback[];
    loading: boolean;
    error: string | null;
};

export function Reviews(props: props) {
    const { feedbacks, loading, error } = props;

    if (loading) return <p>Загрузка отзывов...</p>;
    if (error) return <p className='error'>Ошибка: {error}</p>;

    return (
        <div className={styles.mainContainer}>
            <div className={styles.container}>
                <p className={styles.title}>Все отзывы</p>
                <p className={styles.count}>{feedbacks.length}</p>
            </div>
            <div className={styles.reviews}>
                {feedbacks.map((feedback) => (
                    <div className={styles.card} key={feedback.id}>
                        <p className={styles.rating}>{feedback.rate}/10</p>
                        <p className={styles.text}>{feedback.text}</p>
                        <div className={styles.info}>
                            <div className={styles.name}>
                                <img
                                    src={
                                        feedback.userProfilePictureUrl ===
                                        'none'
                                            ? photo
                                            : feedback.userProfilePictureUrl
                                    }
                                    alt='person'
                                    className={styles.img}
                                />
                                <p>{feedback.userLogin}</p>
                            </div>
                            <p className={styles.date}>
                                {transformUpdatedAt(feedback.updatedAt)}
                            </p>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}
