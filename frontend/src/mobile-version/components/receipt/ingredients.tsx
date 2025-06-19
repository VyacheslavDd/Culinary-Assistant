import { useState } from 'react';
import styles from './ingredients.module.scss';
import { Ingredient } from 'types';
import { transformMeasure, transformName } from 'utils/transform';

type props = {
    ingredients: Ingredient[];
};

export function Ingredients(props: props) {
    const { ingredients } = props;
    const [counter, setCounter] = useState(1);

    const decreaseCounter = () => {
        setCounter((prev) => (prev > 1 ? prev - 1 : 1));
    };

    const getAdjustedIngredient = (ingredient: Ingredient) => {
        return {
            ...ingredient,
            numericValue: parseFloat((ingredient.numericValue * counter).toFixed(2)),
        };
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
                            aria-label='Уменьшить количество порций'
                        >
                            -
                        </button>
                        <span className={styles.number}>{counter}</span>
                        <button
                            className={styles.button}
                            onClick={() => setCounter((prev) => prev + 1)}
                            aria-label='Увеличить количество порций'
                        >
                            +
                        </button>
                    </div>
                </div>

                <ul className={styles.list}>
                    {ingredients.map((ingredient) => {
                        const adjustedIngredient =
                            getAdjustedIngredient(ingredient);
                        return (
                            <li className={styles.li} key={ingredient.name}>
                                <span>{transformName(ingredient.name)}</span>
                                <span>
                                    {adjustedIngredient.numericValue}{' '}
                                    {transformMeasure(
                                        adjustedIngredient.measure
                                    )}
                                </span>
                            </li>
                        );
                    })}
                </ul>
            </div>
        </div>
    );
}
