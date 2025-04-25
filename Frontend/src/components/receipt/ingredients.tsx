import { useState } from 'react';
import styles from './ingredients.module.scss';

export function Ingredients() {
    const [counter, setCounter] = useState(1);

    const decreaseCounter = () => {
        setCounter((prev) => (prev > 1 ? prev - 1 : 1));
    };

    return (
        <div className={styles.mainContainer}>
            <div className={styles.top}></div>
            <div className={styles.main}>
                <div className={styles.header}>
                    <p className={styles.title}>Ингредиенты</p>
                    <div className={styles.counter}>
                        <button
                            className={styles.button}
                            onClick={decreaseCounter}
                        >
                            -
                        </button>
                        <span className={styles.number}>{counter}</span>
                        <button
                            className={styles.button}
                            onClick={() => setCounter((prev) => prev + 1)}
                        >
                            +
                        </button>
                    </div>
                </div>

                <ul className={styles.list}>
                    <li className={styles.li}>
                        <span>Молоко</span>
                        <span>500 мл</span>
                    </li>
                    <li className={styles.li}>
                        <span>Яйца</span>
                        <span>2 шт.</span>
                    </li>
                    <li className={styles.li}>
                        <span>Мука</span>
                        <span>200 г</span>
                    </li>
                    <li className={styles.li}>
                        <span>Сахар</span>
                        <span>1 ст. л.</span>
                    </li>
                    <li className={styles.li}>
                        <span>Соль</span>
                        <span>щепотка</span>
                    </li>
                </ul>
            </div>
        </div>
    );
}
