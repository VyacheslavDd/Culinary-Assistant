import { useEffect, useRef, useState } from 'react';
import styles from './review.module.scss';

export function Review() {
    const [review, setReview] = useState('');
    const [isSubmitted, setIsSubmitted] = useState(false);
    const textareaRef = useRef<HTMLTextAreaElement>(null);

    useEffect(() => {
        const textarea = textareaRef.current;
        if (textarea) {
            textarea.style.height = 'auto';
            textarea.style.height = `${textarea.scrollHeight}px`;
        }
    }, [review]);

    const handleChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
        setReview(e.target.value);
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (isValidReview()) {
            setIsSubmitted(true);
            console.log('Отзыв отправлен:', review);
        }
    };

    const isValidReview = () => {
        return review.length >= 10 && review.length <= 200;
    };

    return (
        <div className={styles.mainContainer}>
            <div className={styles.header}>
                <p className={styles.title}>Ваш отзыв</p>
                <p className={styles.subtitle}>от 10 до 200 символов</p>
            </div>
            <div className={styles.textareaContainer}>
                <textarea
                    ref={textareaRef}
                    value={review}
                    onChange={handleChange}
                    placeholder='Напишите свой отзыв о рецепте'
                    className={styles.textarea}
                    minLength={10}
                    maxLength={200}
                    required
                ></textarea>
                <div className={styles.buttonContainer}>
                    <p>{review.length}/200</p>
                    {isValidReview() && (
                        <button type='submit' className={styles.button}>
                            <span className={styles.icon}></span>
                            Отправить отзыв
                        </button>
                    )}
                </div>
            </div>
        </div>
    );
}
