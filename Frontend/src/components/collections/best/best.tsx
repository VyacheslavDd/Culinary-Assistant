import styles from './best.module.scss';
import left from '../../../assets/svg/button_left.svg';
import right from '../../../assets/svg/button_right.svg';
import { ListMinCollections } from '../list-min/list-min';
import { ShortCollection } from 'types/short-collections.type';
import { useState } from 'react';

type props = {
    collections: ShortCollection[];
};

const VISIBLE_COUNT = 3;

export function Best(props: props) {
    const { collections } = props;
    const [startIndex, setStartIndex] = useState(0);

    const handlePrev = () => {
        setStartIndex((prev) => Math.max(prev - VISIBLE_COUNT, 0));
    };

    const handleNext = () => {
        setStartIndex((prev) =>
            Math.min(prev + VISIBLE_COUNT, collections.length - VISIBLE_COUNT)
        );
    };

    const visibleCollections = collections.slice(
        startIndex,
        startIndex + VISIBLE_COUNT
    );

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
                    <ListMinCollections
                        theme='light'
                        items={visibleCollections}
                    />
                    <div className={styles.buttons}>
                        <button
                            className={styles.button}
                            onClick={handlePrev}
                            disabled={startIndex === 0}
                        >
                            <img src={left} alt='left' />
                        </button>
                        <button
                            className={styles.button}
                            onClick={handleNext}
                            disabled={
                                startIndex + VISIBLE_COUNT >= collections.length
                            }
                        >
                            <img src={right} alt='right' />
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}
