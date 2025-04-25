import { useState } from 'react';
import styles from './rate.module.scss';
import favorite from '../../assets/svg/mink_fav.svg';
import unfavorite from '../../assets/svg/mink.svg';

export function Ratings() {
    const [rating, setRating] = useState(0);
    const [hoverRating, setHoverRating] = useState(0);

    return (
        <div className={styles.mainContainer}>
            <p className={styles.title}>Оцените рецепт</p>
            <div className={styles.stars}>
                {[...Array(10)].map((_, index) => {
                    const ratingValue = index + 1;
                    return (
                        <button
                            key={index}
                            className={styles.star}
                            onClick={() => setRating(ratingValue)}
                            onMouseEnter={() => setHoverRating(ratingValue)}
                            onMouseLeave={() => setHoverRating(0)}
                            aria-label={`Оценка ${ratingValue}`}
                            aria-pressed={ratingValue <= rating}
                        >
                            {ratingValue <= (hoverRating || rating) ? (
                                <img src={favorite} alt='' aria-hidden='true' />
                            ) : (
                                <img
                                    src={unfavorite}
                                    alt=''
                                    aria-hidden='true'
                                />
                            )}
                        </button>
                    );
                })}
            </div>
        </div>
    );
}
