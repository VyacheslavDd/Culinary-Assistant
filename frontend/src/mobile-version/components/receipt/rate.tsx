import { useEffect, useState } from 'react';
import styles from './rate.module.scss';
import favorite from '../../../assets/svg/mink_fav.svg';
import unfavorite from '../../../assets/svg/mink.svg';

type props = {
    currentRating: number;
    onRate: (rating: number) => void;
    disabled?: boolean;
    title?: string;
};

export function Ratings(props: props) {
    const { currentRating, onRate, disabled = false, title = 'рецепт' } = props;
    const [rating, setRating] = useState(currentRating);
    const [hoverRating, setHoverRating] = useState(0);

    const handleClick = (ratingValue: number) => {
        if (!disabled) {
            onRate(ratingValue);
        }
    };

    const handleMouseEnter = (ratingValue: number) => {
        if (!disabled) {
            setHoverRating(ratingValue);
        }
    };

    const handleMouseLeave = () => {
        if (!disabled) {
            setHoverRating(0);
        }
    };

    useEffect(() => {
        if (currentRating !== rating) {
            setRating(currentRating);
        }
    }, [currentRating, rating]);

    return (
        <div className={styles.mainContainer}>
            <p className={styles.title}>Оцените {title}</p>
            <div className={styles.stars}>
                {[...Array(10)].map((_, index) => {
                    const ratingValue = index + 1;
                    return (
                        <button
                            key={index}
                            className={styles.star}
                            onClick={() => handleClick(ratingValue)}
                            onMouseEnter={() => handleMouseEnter(ratingValue)}
                            onMouseLeave={handleMouseLeave}
                            disabled={disabled}
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
